using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Net.Http;

/// <summary>
/// Summary description for SSLPost
/// </summary>
public class SSLPost
{
    public class CertPolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest request, int problem)
        {
            return true;
        }
    }
    public class MyPolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(
                ServicePoint srvPoint
                , X509Certificate certificate
                , WebRequest request
                , int certificateProblem)
        {
            //Return True to force the certificate to be accepted. 
            return true;
        } // end CheckValidationResult 
    }
    #region Log
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }
    #endregion
    public bool postXMLData(string postURL, string data2Post, out string responseData, out string errMsg)
    {
        responseData = "";
        errMsg = "";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postURL);
            byte[] bytes;

            bytes = Encoding.ASCII.GetBytes(data2Post);
            request.ContentType = "application/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                responseData = new StreamReader(responseStream).ReadToEnd();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            errMsg = ex.ToString();
            return false;
        }
    }
    public bool postData(string postURL, string data2Post, out string responseData, out string errMsg)
    {
        responseData = "";
        errMsg = "";


        HttpWebResponse response = null;


        try
        {
            ServicePointManager.CertificatePolicy = new CertPolicy();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postURL);
            string requestTimeout = ConfigurationManager.AppSettings["123TimeOut"].ToString();

            request.Method = "POST";
            request.UserAgent = "SinaptIQ WebBot";
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
            int.Parse(requestTimeout);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            if (data2Post != "")
            {
                StreamWriter postwriter = new StreamWriter(request.GetRequestStream());
                postwriter.Write(data2Post);
                postwriter.Close();
            }
            else
            {
                request.ContentLength = 0L;
            }
            writeLog("First Time Attempt");
            response = (HttpWebResponse)request.GetResponse();
            responseData = new StreamReader(response.GetResponseStream(), Encoding.ASCII).ReadToEnd();
            responseData = responseData.Trim();

            writeLog("First Time RES DATA : " + responseData);
        }
        catch (Exception e)
        {
            writeLog(e.Message);
            writeLog("Second Time Attempt");

            try
            {
                ServicePointManager.CertificatePolicy = new CertPolicy();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postURL);

                request.Method = "POST";
                request.UserAgent = "SinaptIQ WebBot";
                request.ContentType = "application/x-www-form-urlencoded";
                request.KeepAlive = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                if (data2Post != "")
                {
                    StreamWriter postwriter = new StreamWriter(request.GetRequestStream());
                    postwriter.Write(data2Post);
                    postwriter.Close();
                }
                else
                {
                    request.ContentLength = 0L;
                }
                response = (HttpWebResponse)request.GetResponse();
                responseData = new StreamReader(response.GetResponseStream(), Encoding.ASCII).ReadToEnd();
                responseData = responseData.Trim();

                writeLog("Second Time RES DATA : " + responseData);
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
                errMsg = ex.Message;
                return false;
            }


        }
        finally
        {
            if (response != null)
            {
                response.Close();
            }
        }
        return true;
    }

    public bool postDataCanalPlus(string postURL, string data2Post, out string responseData, out string errMsg)
    {
        responseData = "";
        errMsg = "";


        HttpWebResponse response = null;


        try
        {
            string requestTimeout = ConfigurationManager.AppSettings["EBARequestTimeOut"].ToString();
            var request = (HttpWebRequest)WebRequest.Create(postURL);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Timeout = int.Parse(requestTimeout);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            if (data2Post != "")
            {
                StreamWriter postwriter = new StreamWriter(request.GetRequestStream());
                postwriter.Write(data2Post);
                postwriter.Close();
            }
            else
            {
                request.ContentLength = 0L;
            }
            response = (HttpWebResponse)request.GetResponse();
            responseData = new StreamReader(response.GetResponseStream(), Encoding.ASCII).ReadToEnd();
            responseData = responseData.Trim();
        }
        catch (Exception e)
        {
            responseData = e.Message;
            errMsg = e.Message;
            return false;
        }
        finally
        {
            if (response != null)
            {
                response.Close();
            }
        }
        return true;
    }

    public bool postDateSolarHome(string apiUrl, string data2Post, long txnID, out string responseData)
    {
        writeLog("$$$$$$$$$$$$$$$ POSTING TO DATA SOLAR txnID : " + txnID + " $$$$$$$$$$$$$$$");
        HttpClient client = new HttpClient();
        responseData = "";
        try
        {
            var content = new StringContent(data2Post, Encoding.UTF8, "application/json");
            var result = client.PostAsync(apiUrl, content).Result;
            responseData = result.Content.ReadAsStringAsync().Result;
        }
        catch (Exception ex)
        {
            writeLog("Solar Home Confirm Response error : " + txnID + " " + ex.ToString());
            return false;
        }
        return true;
    }

    public bool postDataPG(string apiUrl, string data2Post, string token, out string responseData, out string errMsg)
    {
        writeLog("$$$$$$$$$$$$$$$ POSTING TO DATA Pahtama Group $$$$$$$$$$$$$$$");
        responseData = "";
        errMsg = "";
        try
        {

            ServicePointManager.CertificatePolicy = new MyPolicy();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            //Create an instance of the WebRequest class
            WebRequest objRequest = WebRequest.Create(apiUrl);
            objRequest.Timeout = 2 * 60000; //In milliseconds - in this case 60 seconds
            objRequest.Method = "POST";
            objRequest.ContentLength = data2Post.Length;
            objRequest.ContentType = "application/json";
            objRequest.Headers.Add("Authorization", "Bearer " + token);
            //Create an instance of the StreamWriter class and attach the WebRequest object to it - here's where we do the posting 
            StreamWriter postWriter = new StreamWriter(objRequest.GetRequestStream());
            postWriter.Write(data2Post);
            postWriter.Close();

            //Create an instance of the WebResponse class and get the output to the rawOutput string
            WebResponse objResponse = objRequest.GetResponse();
            StreamReader sr = new StreamReader(objResponse.GetResponseStream());
            responseData = sr.ReadToEnd();
            writeLog("Response from  Pahtama Group: " + responseData);
            sr.Close();
        }
        catch (Exception ex)
        {
            writeLog(ex.ToString());
            errMsg = ex.ToString();
            return false;
        }

        return true;
    }

    public string postTo663(string url, string PostData)
    {
        HttpWebResponse response = null;
        string responseString = string.Empty;

        try
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            var postData = PostData;

            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            response = (HttpWebResponse)request.GetResponse();

            responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        catch (Exception ex)
        {
            responseString = ex.Message;
        }
        finally
        {
            if (response != null)
            {
                response.Close();
            }
        }

        return responseString.ToString();
    }
    public bool postDatautil(string postURL, out string responseData, out string errMsg)
    {
        responseData = "";
        errMsg = "";


        HttpWebResponse response = null;

        try
        {
            ServicePointManager.CertificatePolicy = new CertPolicy();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postURL);
            request.Method = "POST";
            request.ContentType = "application/xml; charset=utf-8";
            request.Timeout = 2097151;
            response = (HttpWebResponse)request.GetResponse();
            responseData = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            responseData = responseData.Trim();
        }
        catch (Exception e)
        {
            responseData = e.Message;
            errMsg = e.Message;
            return false;
        }
        finally
        {
            if (response != null)
            {
                response.Close();
            }
        }
        return true;
    }
    public bool post2Putet(string backendUrl, string compressEncrypt, out string responseData, out string errMsg)
    {
        errMsg = "";
        var sslPost = new SSLPost();
        responseData = "";
        bool sendSuccess = false;


        if (!sslPost.postXMLData(backendUrl, compressEncrypt, out responseData, out errMsg))
        {
            errMsg = "To URL : " + backendUrl + "<br> Error Message : " + errMsg + "<br> Time stamp : " + DateTime.Now.ToString();

        }
        else
        {
            sendSuccess = true;
        }

        return sendSuccess;
    }
    public bool post2EasyPoint(string backendUrl, string compressEncrypt, out string responseData, out string errMsg)
    {
        SinaptIQPKCS7.PKCS7 pkcs7 = new SinaptIQPKCS7.PKCS7();
        string encryptedMsg = pkcs7.encryptMessage(compressEncrypt, pkcs7.getPublicCert(ConfigurationManager.AppSettings["publicKeyPath"].ToString()));

        errMsg = "";
        SSLPost sslPost = new SSLPost();
        responseData = "";
        bool sendSuccess = false;

        if (!sslPost.postData(backendUrl, encryptedMsg, out responseData, out errMsg))
        {
            errMsg = "To URL : " + backendUrl + "<br> Error Message : " + errMsg + "<br> Time stamp : " + DateTime.Now.ToString();

        }
        else
        {
            writeLog("easypoints response before decrypt:" + responseData);
            responseData = pkcs7.decryptMessage(responseData, pkcs7.getPrivateCert(ConfigurationManager.AppSettings["privateKeyPath"].ToString(), ConfigurationManager.AppSettings["privateKeyPWD"].ToString()));
            sendSuccess = true;
        }

        return sendSuccess;
    }
    public bool post2Biller(string backendUrl, string compressEncrypt, out string responseData, out string errMsg)
    {
        errMsg = "";
        SSLPost sslPost = new SSLPost();
        responseData = "";
        bool sendSuccess = false;


        if (!sslPost.postData(backendUrl, compressEncrypt, out responseData, out errMsg))
        {
            errMsg = "To URL : " + backendUrl + "<br> Error Message : " + errMsg + "<br> Time stamp : " + DateTime.Now.ToString();
        }
        else
        {
            sendSuccess = true;
        }

        return sendSuccess;
    }

    public bool postDataJson(string postURL, string data2Post, out string responseData, out string errMsg)
    {
        responseData = "";
        errMsg = "";


        HttpWebResponse response = null;

        try
        {
            ServicePointManager.CertificatePolicy = new CertPolicy();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postURL);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Timeout = 2097151;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            StreamWriter postWriter = new StreamWriter(request.GetRequestStream());
            postWriter.Write(data2Post);
            postWriter.Close();

            response = (HttpWebResponse)request.GetResponse();
            responseData = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            responseData = responseData.Trim();
        }
        catch (Exception e)
        {
            responseData = e.Message;
            errMsg = e.Message;
            return false;
        }
        finally
        {
            if (response != null)
            {
                response.Close();
            }
        }
        return true;
    }

    public bool sendRequest(string payload, string paymentURL, out string encResponse, out string err)
    {
        encResponse = "";
        err = "";
        try
        {
            WebRequest objRequest = WebRequest.Create(paymentURL);
            objRequest.Timeout = 600000; //In milliseconds
            objRequest.Method = "POST";
            objRequest.ContentLength = payload.Length;
            objRequest.ContentType = "application/x-www-formurlencoded";
            StreamWriter postWriter = new StreamWriter(objRequest.GetRequestStream());
            postWriter.Write(payload);
            postWriter.Close();
            WebResponse objResponse = objRequest.GetResponse();
            StreamReader sr = new
            StreamReader(objResponse.GetResponseStream());
            encResponse = sr.ReadToEnd();
            sr.Close();
            return true;
        }
        catch (Exception ex)
        {
            err = ex.Message.ToString();
            return false;
        }
    }

    // 123Remit
    public static string SendRequestJson(string jsonReq, string url)
    {

        HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
        ServicePointManager.CertificatePolicy = new CertPolicy();
        objRequest.Timeout = 2 * 60000; //In milliseconds - in this case 60 seconds
        objRequest.Method = "POST";
        objRequest.ContentType = "application/json";
        objRequest.ContentLength = jsonReq.Length;

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        StreamWriter postWriter = new StreamWriter(objRequest.GetRequestStream());
        objRequest.KeepAlive = true;
        postWriter.Write(jsonReq);
        postWriter.Close();

        //Create an instance of the WebResponse class and get the output to the rawOutput string
        WebResponse objResponse = objRequest.GetResponse();
        StreamReader sr = new StreamReader(objResponse.GetResponseStream());
        string rawOutput = sr.ReadToEnd();
        sr.Close();

        return rawOutput;

    }

    // AWBA MIT
    public bool postToMitAwba(string postURL, string data2Post, out string responseData, out string errMsg)
    {
        responseData = "";
        errMsg = "";


        HttpWebResponse response = null;

        try
        {
            ServicePointManager.CertificatePolicy = new CertPolicy();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postURL);
            request.Method = "POST";
            request.UserAgent = "SinaptIQ WebBot";
            request.ContentType = "text/plain";
            request.MediaType = "text/plain";
            request.Timeout = 1000000;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            request.KeepAlive = true;

            if (data2Post != "")
            {
                StreamWriter postwriter = new StreamWriter(request.GetRequestStream());
                postwriter.Write(data2Post);
                postwriter.Close();
            }
            else
            {
                request.ContentLength = 0L;
            }
            response = (HttpWebResponse)request.GetResponse();
            responseData = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            responseData = responseData.Trim();

        }
        catch (Exception e)
        {
            responseData = e.Message;
            errMsg = e.Message;

            return false;
        }
        finally
        {
            if (response != null)
            {
                response.Close();
            }
        }
        return true;
    }

}