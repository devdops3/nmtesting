using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

/// <summary>
/// Summary description for OkDollar
/// </summary>
public class OkDollar
{
    
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }

    public OkDollarInterfaceResponse ConfirmOkDollar(string reqJson, string url)
    {
        writeLog("This is ConfirmOkDollar");
        writeLog("OkDollarInterface Request" + " : " + reqJson);

        string result = string.Empty;
        OkDollarInterfaceResponse okRes = new OkDollarInterfaceResponse();

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

            writeLog("OkDollarInterface Response" + " : " + result);
            
            okRes = JsonConvert.DeserializeObject<OkDollarInterfaceResponse>(result);

        }
        catch (Exception ex)
        {
            writeLog("Exception occur when request to OkDollarInterface:" + ex.Message);
            result = string.Empty;
        }

        return okRes;
    }
}

public class OkDollarInterfaceRequest
{
    public string NearMePaymentRefNo { get; set; }
    public decimal Amount { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
    public Dictionary<string, string> PaymentProfileInfo { get; set; }
}

public class OkDollarInterfaceResponse
{
    public string NearMePaymentRefNo { get; set; }
    public string PaymentSchemeRefNo { get; set; }
    public string ResCode { get; set; }
    public string ResDescription { get; set; }
    public string FailReason { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
}