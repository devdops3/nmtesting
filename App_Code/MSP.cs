using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

/// <summary>
/// Summary description for MSP
/// </summary>
public class MSP
{

    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }

    public MSPInterfaceResponse ConfirmMSP(string reqJson, string url)
    {
        writeLog("This is ConfirmMSP");
        writeLog("MSPInterface Request" + " : " + reqJson);

        string result = string.Empty;
        MSPInterfaceResponse okRes = new MSPInterfaceResponse();

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

            writeLog("MSPInterface Response" + " : " + result);

            okRes = JsonConvert.DeserializeObject<MSPInterfaceResponse>(result);

        }
        catch (Exception ex)
        {
            writeLog("Exception occur when request to MSPInterface:" + ex.Message);
            result = string.Empty;
        }

        return okRes;
    }
}

public class MSPInterfaceRequest
{
    public string NearMePaymentRefNo { get; set; }
    public decimal Amount { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
    public Dictionary<string, string> PaymentProfileInfo { get; set; }
}

public class MSPInterfaceResponse
{
    public string NearMePaymentRefNo { get; set; }
    public string PaymentSchemeRefNo { get; set; }
    public string ResCode { get; set; }
    public string ResDescription { get; set; }
    public string FailReason { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
}