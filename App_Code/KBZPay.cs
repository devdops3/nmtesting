using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

public class KBZPay
{
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }

    public KBZPayInterfaceResponse ConfirmKBZPay(string reqJson, string url)
    {
        writeLog("This is ConfirmKBZPay");
        writeLog("KBZPayInterface Request" + " : " + reqJson);

        string result = string.Empty;
        KBZPayInterfaceResponse kbzPayRes = new KBZPayInterfaceResponse();

        try
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(reqJson);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            writeLog("KBZPayInterface Response" + " : " + result);

            kbzPayRes = JsonConvert.DeserializeObject<KBZPayInterfaceResponse>(result);

        }
        catch (Exception ex)
        {
            writeLog("Exception occur when request to KBZPayInterface:" + ex.Message);
            result = string.Empty;
        }

        return kbzPayRes;
    }
}

public class KBZPayInterfaceRequest
{
    public string NearMePaymentRefNo { get; set; }
    public decimal Amount { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
    public Dictionary<string, string> PaymentProfileInfo { get; set; }
}

public class KBZPayInterfaceResponse
{
    public string NearMePaymentRefNo { get; set; }
    public string PaymentSchemeRefNo { get; set; }
    public string ResCode { get; set; }
    public string ResDescription { get; set; }
    public string FailReason { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
}