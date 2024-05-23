using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Reflection;

/// <summary>
/// Summary description for UABPay
/// </summary>
public class UABPayManager
{
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }

    public UABPayInterfaceResponse ConfirmUABPay(string reqJson, string url)
    {
        writeLog("This is Confirm UAB Pay");
        writeLog("UABPayInterface Request" + " : " + reqJson);

        string result = string.Empty;
        var uabPayResponse = new UABPayInterfaceResponse();

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

            writeLog("UABPayInterface Response" + " : " + result);

            uabPayResponse = JsonConvert.DeserializeObject<UABPayInterfaceResponse>(result);

        }
        catch (Exception ex)
        {
            writeLog("Exception occur when request to UABPayInterface:" + ex.Message);
            result = string.Empty;
        }

        return uabPayResponse;
    }
}