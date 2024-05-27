using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

/// <summary>
/// Summary description for SaiSaiPay
/// </summary>
public class SaiSaiPay
{
    
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }

    public SaiSaiPayInterfaceResponse ConfirmSaiSaiPay(string reqJson, string url)
    {
        writeLog("This is ConfirmSaiSaiPay");
        writeLog("SaiSaiPayInterface Request" + " : " + reqJson);

        string result = string.Empty;
        SaiSaiPayInterfaceResponse saiSaiPayRes = new SaiSaiPayInterfaceResponse();

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

            writeLog("SaiSaiPayInterface Response" + " : " + result);
            
            saiSaiPayRes = JsonConvert.DeserializeObject<SaiSaiPayInterfaceResponse>(result);

        }
        catch (Exception ex)
        {
            writeLog("Exception occur when request to SaiSaiPayInterface:" + ex.Message);
            result = string.Empty;
        }

        return saiSaiPayRes;
    }
}

public class SaiSaiPayInterfaceRequest
{
    public string NearMePaymentRefNo { get; set; }
    public decimal Amount { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
    public Dictionary<string, string> PaymentProfileInfo { get; set; }
}

public class SaiSaiPayInterfaceResponse
{
    public string NearMePaymentRefNo { get; set; }
    public string PaymentSchemeRefNo { get; set; }
    public string ResCode { get; set; }
    public string ResDescription { get; set; }
    public string FailReason { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
}