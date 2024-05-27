using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

/// <summary>
/// Summary description for MoMoneyManager
/// </summary>
public class MoMoneyManager
{
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }

    public EPaymentInterfaceResponse Confirm(string reqJson, string url)
    {
        writeLog("This is ConfirmMoMoney");
        writeLog("MoMoney Interface Request" + " : " + reqJson);

        string result = string.Empty;
        var moMoneyResponse = new EPaymentInterfaceResponse();
        var paymentInterfaceTimeout = int.Parse(ConfigurationManager.AppSettings["MoMoneyInterfaceTimeoutMilliseconds"].ToString());

        try
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = paymentInterfaceTimeout;

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

            writeLog("MoMoney Interface Response" + " : " + result);

            moMoneyResponse = JsonConvert.DeserializeObject<EPaymentInterfaceResponse>(result);

        }
        catch (Exception ex)
        {
            writeLog("Exception occur when request to MoMoney Interface:" + ex.Message);
        }

        return moMoneyResponse;
    }
}