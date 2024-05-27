using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

/// <summary>
/// Summary description for MerchantAcceptanceReq_C2B
/// </summary>
public class MerchantAcceptanceReq_C2B
{
	public MerchantAcceptanceReq_C2B()
	{
		//
		// TODO: Add constructor logic here
		//
	}
}

public class CBPay
{
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }

    public CBPayInterfaceResponse ConfirmCBPay(string reqJson, string url)
    {
        writeLog("This is ConfirmCBPay");
        writeLog("CBPayInterface Request" + " : " + reqJson);

        string result = string.Empty;
        CBPayInterfaceResponse cbPayRes = new CBPayInterfaceResponse();

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

            writeLog("CBPayInterface Response" + " : " + result);

            cbPayRes = JsonConvert.DeserializeObject<CBPayInterfaceResponse>(result);

        }
        catch (Exception ex)
        {
            writeLog("Exception occur when request to CBPayInterface:" + ex.Message);
            result = string.Empty;
        }

        return cbPayRes;
    }
}
    

public class CBPayInterfaceRequest
{
    public string NearMePaymentRefNo { get; set; }
    public decimal Amount { get; set; }
    public Dictionary<string, string> PaymentProfileInfo { get; set; }
}

public class CBPayInterfaceResponse
{
    public string NearMePaymentRefNo { get; set; }
    public string QrSchemeRefNo { get; set; }
    public string ResCode { get; set; }
    public string ResDescription { get; set; }
    public string FailReason { get; set; }
    public string ReferenceNo { get; set; }
    public Dictionary<string, string> QrInfo { get; set; }
}