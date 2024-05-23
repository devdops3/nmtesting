using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

/// <summary>
/// Summary description for SSLPost123
/// </summary>
public class SSLPost123
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
    #endregion Log
    public bool post2Biller(string backendUrl, string messageId, long txnId, string compressEncrypt, out string responseData, out string errMsg)
    {
        writeLog("MessageId : " + messageId + " transactionId : " + txnId + " 123 Post 2 Biller Started");
        errMsg = "";
        SSLPost sslPost = new SSLPost();
        responseData = "";
        bool sendSuccess = false;


        if (!postData(backendUrl, compressEncrypt, messageId, txnId, out responseData, out errMsg))
        {
            errMsg = "To URL : " + backendUrl + "<br> Error Message : " + errMsg + "<br> Time stamp : " + DateTime.Now.ToString();

        }
        else
        {

            sendSuccess = true;
        }
        writeLog("MessageId : " + messageId + " transactionId : " + txnId + " 123 Post 2 Biller End");
        return sendSuccess;
        
    }

    public bool postData(string postURL, string data2Post, string messageId, long txnId, out string responseData, out string errMsg)
    {
        writeLog("MessageId : " + messageId + " transactionId : " + txnId + " 123 PostData Started");
        responseData = "";
        errMsg = "";

        HttpWebResponse response = null;

        ServicePointManager.CertificatePolicy = new CertPolicy();
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postURL);
        string requestTimeout = ConfigurationManager.AppSettings["123TimeOut"].ToString();

        request.Method = "POST";
        request.UserAgent = "SinaptIQ WebBot";
        // request.ContentType = "application/xml; charset=utf-8";
        request.ContentType = "application/x-www-form-urlencoded";
        request.KeepAlive = true;
        int.Parse(requestTimeout);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        if (data2Post != "")
        {
            //writeLog("data2post:" + data2Post);
            StreamWriter postwriter = new StreamWriter(request.GetRequestStream());
            postwriter.Write(data2Post);
            postwriter.Close();
        }
        else
        {
            request.ContentLength = 0L;
        }
        writeLog("MessageId : " + messageId + " transactionId : " + txnId + " First Time Attempt");
        response = (HttpWebResponse)request.GetResponse();
        responseData = new StreamReader(response.GetResponseStream(), Encoding.ASCII).ReadToEnd();
        responseData = responseData.Trim();

        writeLog("MessageId : " + messageId + " transactionId : " + txnId + " First Time RES DATA : " + responseData);
        if (response != null)
        {
            response.Close();
        }

        writeLog("MessageId : " + messageId + " transactionId : " + txnId + " 123 PostData End");
        return true;
    }
}