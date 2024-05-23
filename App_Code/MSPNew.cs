using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;

// For MSP
public class MspNew
{
    #region Log
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private void WriteLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }
    #endregion
    public async Task<MspResponse> MspPost(string json, string token, string url)
    {
        try
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResponseMessage = client.PostAsync(url, body).Result;

            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                var response = await httpResponseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MspResponse>(response);
            }

            var failureResponse = await httpResponseMessage.Content.ReadAsStringAsync();
            var msg = JsonConvert.DeserializeObject<MspFailResponse>(failureResponse);
            return new MspResponse
            {
                responseCode = "01",
                responseDesc = msg.message,
                transactionId = string.Empty
            };
        }
        catch 
        {
        }

        return null;
    }

    public string GetTokenMsp(string clientId, string clientSecreteKey)
    {
        try
        {
            var tokenUrl = ConfigurationManager.AppSettings["MspTokenUrl"];

            var form = new Dictionary<string, string> { { "grant_type", "client_credentials" }, { "client_id", clientId }, { "client_secret", clientSecreteKey } };

            var client = new HttpClient();
            
            var tokenResponse = client.PostAsync(tokenUrl, new FormUrlEncodedContent(form)).Result;
            var jsonContent = tokenResponse.Content.ReadAsStringAsync().Result;
            OauthTokenResponse tokenResp = JsonConvert.DeserializeObject<OauthTokenResponse>(jsonContent);
            WriteLog("Token : " + tokenResp.access_token);
            return tokenResp.access_token;
        }
        catch
        {
            return string.Empty;
        }
    }

    public string GetSessionId(string msptoken, string handShakeUrl, string merchantId, string clientId, string clientSecrete)
    {
        var aSCPublic = ConfigurationManager.AppSettings["NearMeMSPPrivateCert"];
        var aSCSEC = ConfigurationManager.AppSettings["NearMeMSPPublicCert"];
        var aSCPub = ConfigurationManager.AppSettings["MSPPbulicCert"];
        var pwd = ConfigurationManager.AppSettings["NearMePassCode"];

        var merchantpublic = File.ReadAllBytes(aSCPublic);
        var merchantpublicstr = Encoding.UTF8.GetString(merchantpublic);
        merchantpublicstr = "{ \"publicCert\": \"" + merchantpublicstr + "\" }";

        var requestbyte = Encoding.UTF8.GetBytes(merchantpublicstr);
        var pgp = new Pgp();
        var publicKey = File.ReadAllBytes(aSCPub);
        var encrBytes = pgp.Encrypt(requestbyte, publicKey);
        var requestString = Encoding.UTF8.GetString(encrBytes);

        var client = new HttpClient();
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + msptoken);
        var body = new StringContent(requestString, Encoding.UTF8, "application/json");
        var httpResponseMessage = client.PostAsync(handShakeUrl, body).Result;

        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            var response = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var privateKeyBytes = File.ReadAllBytes(aSCSEC);
            var responseByte = Encoding.UTF8.GetBytes(response);
            string decrMessage;
            using (var privateKey = new MemoryStream(privateKeyBytes))
            {
                var decrBytes = Pgp.Decrypt(responseByte, privateKey, pwd);
                decrMessage = Encoding.UTF8.GetString(decrBytes);
            }

            if (!string.IsNullOrEmpty(decrMessage))
            {
                if (decrMessage != "ERROR")
                {
                    var idResponse = JsonConvert.DeserializeObject<CertificateIdResponse>(decrMessage);
                    WriteLog("idResponse : " + idResponse.certificateId);
                    var path = ConfigurationManager.AppSettings["SessionPath"];
                    if (!File.Exists(path + merchantId + ".txt"))
                    {
                        File.Create(path + merchantId + ".txt").Close();
                    }

                    File.WriteAllText(path + merchantId + ".txt", string.Empty);

                    using (StreamWriter w = File.AppendText(path + merchantId + ".txt"))
                    {

                        w.WriteLine("{0}", idResponse.certificateId);
                        w.WriteLine("{0}", msptoken);
                        w.WriteLine("{0}", DateTime.Now);
                        w.WriteLine("{0}", clientId);
                        w.WriteLine("{0}", clientSecrete);
                        w.Flush();
                        w.Close();
                    }

                    return idResponse.certificateId;
                }

                return decrMessage;
            }
        }

        var failureResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;
        WriteLog("Fail : " + failureResponse);
        return string.Empty;
    }

    public MspResponse ConfirmMsp(string mobileNumber, string amount, string msptoken, string sessionId, string qr, string confirmUrl, string handshakeUrl)
    {
        var aSCSEC = ConfigurationManager.AppSettings["NearMeMSPPublicCert"];
        var aSCPub = ConfigurationManager.AppSettings["MSPPbulicCert"];
        var pwd = ConfigurationManager.AppSettings["NearMePassCode"];

        var url = confirmUrl + qr + "/process";
        var inputString = "{ \"receiverMobileNo\": \"" + mobileNumber + "\", \"amount\": " + amount + " }";
        var pgp = new Pgp();
        var input = Encoding.UTF8.GetBytes(inputString);
        var publicKey = File.ReadAllBytes(aSCPub);
        var encrBytes = pgp.Encrypt(input, publicKey);
        var encrString = Encoding.UTF8.GetString(encrBytes);

        if (string.IsNullOrEmpty(sessionId))
        {
            return new MspResponse
            {
                transactionId = string.Empty,
                responseCode = "06",
                responseDesc = "Get session Id failed."
            };
        }

        if (sessionId == "ERROR")
        {
            return new MspResponse
            {
                transactionId = string.Empty,
                responseCode = "06",
                responseDesc = "Get session Id failed."
            };
        }

        var client = new HttpClient();
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + msptoken);
        client.DefaultRequestHeaders.Add("X-session", sessionId);
        var body = new StringContent(encrString, Encoding.UTF8, "application/json");
        WriteLog("Post Body : " + encrString);
        WriteLog("Post Url : " + url);
        var httpResponseMessage = client.PostAsync(url, body).Result;
        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            WriteLog("Post to msp is ok.");
            var response = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var privateKeyBytes = File.ReadAllBytes(aSCSEC);
            var responseByte = Encoding.UTF8.GetBytes(response);
            string decrMessage;
            using (var privateKey = new MemoryStream(privateKeyBytes))
            {
                var decrBytes = Pgp.Decrypt(responseByte, privateKey, pwd);
                decrMessage = Encoding.UTF8.GetString(decrBytes);
                WriteLog("DecrMessage : " + decrMessage);
            }

            if(decrMessage == "ERROR") return new MspResponse
            {
                transactionId = string.Empty,
                responseCode = "06",
                responseDesc = "Processing is failed."
            };

            var res = JsonConvert.DeserializeObject<MspResponse>(decrMessage);
            WriteLog("Confirm response : " + res);
            return res;
        }

        var failMsg = httpResponseMessage.Content.ReadAsStringAsync().Result;
        WriteLog("Confirm failed : " + failMsg);
        return new MspResponse
        {
            transactionId = string.Empty,
            responseCode = "06",
            responseDesc = failMsg
        };
    }

    public class OauthTokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }

    }

    public class CertificateIdResponse
    {
        public string certificateId { get; set; }
    }

    public class Token
    {
        public string access_token { get; set; }
    }

    public class MspRequest
    {
        public string amount { get; set; }
        public string receiverMobileNo { get; set; }
    }

    public class MspResponse
    {
        public string transactionId { get; set; }
        public string responseCode { get; set; }
        public string responseDesc { get; set; }
    }

    public class MspFailResponse
    {
        public string message { get; set; }
    }

}




