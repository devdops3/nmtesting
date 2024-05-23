using log4net;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;

public class APlus : IEPayment
{
    private static A2AAPIWCF.ServiceClient _agentWCF;

    public APlus()
    {
        _agentWCF = new A2AAPIWCF.ServiceClient();
    }

    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void WriteLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }

    public string Confirm(EPaymentMerchant merchant,
                          long txnId,
                          string ref1,
                          string ref2,
                          string ref3,
                          string ref4,
                          string ref5, 
                          string email, 
                          string password, 
                          string amount, 
                          string branchCode, 
                          string agentName, 
                          string appType,
                          string productDesc)
    {
        WriteLog("########## A+ Wallet | Payment - START ##########");
        string response = string.Empty;
        try
        {
            APlusPayInterfacePayRequest data = new APlusPayInterfacePayRequest()
            {
                NearMeRefId = txnId.ToString(),
                Username = merchant.ClientId,
                Password = merchant.ClientSecret,
                MerchantUserId = merchant.MerchantId,
                SequenceNo = GetSequenceNo(),
                Amount = amount,
                QrString = ref1
            };

            string request = JsonConvert.SerializeObject(data);
            WriteLog(string.Concat("Payment Request Data : ", request));

            string url = ConfigurationManager.AppSettings["APlusPayInterfaceURL"];
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(request);
                streamWriter.Flush();
                streamWriter.Close();
            }

            string resultString = string.Empty;
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                resultString = streamReader.ReadToEnd();
            }
            WriteLog(string.Concat("Payment Response Data : ", resultString));

            var result = JsonConvert.DeserializeObject<APlusPayInterfacePayResponse>(resultString);

            if (result == null) return response;

            string status = result.Code == "00" ? "AP" : "RE";
            string desc = result.Code == "00" ? "Approved" : result.Message;
            string paymentReferenceNo = result.Code == "00" ? result.Data.ReferIntegrationId : null;

            var errMsg = string.Empty;
            if (!_agentWCF.UpdateMerchantTransaction(txnId, status, desc, paymentReferenceNo, string.Empty, out errMsg))
            {
                Utils.WriteLog_Biller("Error in UpdateMerchantTransaction : " + errMsg);
                var errorMessage = "Transaction Failed";
                return Utils.getErrorRes("06", errorMessage);
            }
            if (status == "RE")
            {
                return Utils.getErrorRes("06", desc);
            }

            ConfirmResponseModel confirmResponse = new ConfirmResponseModel();
            confirmResponse.email = email;
            confirmResponse.password = password;
            confirmResponse.rescode = result.Code;
            confirmResponse.resdesc = result.Message;
            confirmResponse.ref1 = productDesc;
            confirmResponse.ref2 = ref1;
            confirmResponse.ref3 = string.Empty;
            confirmResponse.ref4 = string.Empty;
            confirmResponse.ref5 = ref5;
            confirmResponse.ref1Name = "Payment Type";
            confirmResponse.ref2Name = string.Empty;
            confirmResponse.ref3Name = string.Empty;
            confirmResponse.ref4Name = string.Empty;
            confirmResponse.ref5Name = string.Empty;
            confirmResponse.txnID = txnId.ToString();
            confirmResponse.smsMsg = string.Empty;

            response = Utils.GetMerchantAcceptanceRes(confirmResponse);
        }
        catch (Exception exception)
        {
            WriteLog(string.Concat("Exception in Payment - Message : ", exception.Message, " StackTrace : ", exception.StackTrace));
        }
        finally
        {
            WriteLog("########## A+ Wallet Payment - END ##########");
        }
        return response;
    }

    #region Extension Methods
    private string GetSequenceNo()
    {
        Guid sequenceNo = Guid.NewGuid();
        return sequenceNo.ToString("N");
    }
    #endregion
}