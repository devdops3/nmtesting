using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;

/// <summary>
/// Summary description for AyaPayC2B
/// </summary>
public class AyaPayC2B : IEPayment
{
	private static A2AAPIWCF.ServiceClient _agentWCF;
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public AyaPayC2B()
    {
        _agentWCF = new A2AAPIWCF.ServiceClient();
    }

    public string Confirm(EPaymentMerchant merchant, long txnId, string ref1, string ref2, string ref3, string ref4, string ref5, string email, string password, string amount, string branchCode, string agentName, string appType, string productDesc)
    {
        string status = string.Empty;
        string desc = string.Empty;
        var errMsg = string.Empty;
        string errmsg = string.Empty;
        var smsMsg = string.Empty;
        string qr = string.Empty;
        string paymentRefNo = string.Empty;
        string transExpiredTime = string.Empty;
        var logAppender = "MessageId : " + " | Aya Pay C2B| ";


        Dictionary<string, string> paymentProfileInfo = new Dictionary<string, string>();
        paymentProfileInfo.Add("phone", merchant.ClientId);
        paymentProfileInfo.Add("password", merchant.ClientSecret);

        var req = new AyaPayInterfaceRequest()
        {
            NearMePaymentRefNo = ConfigurationManager.AppSettings["Prefix"].ToString() + txnId,
            Amount = Decimal.Parse(amount),
            PaymentProfileInfo = paymentProfileInfo
        };
        writeLog(logAppender + "QR Request Created");

        string ayaPayReq = JsonConvert.SerializeObject(req);
        string ayaPayInterfaceUrl = ConfigurationManager.AppSettings["AyaPayC2BInterfaceUrl"];
        var ayaPay = new AyaPay();
        var ayaPayResponse = ayaPay.Confirm(ayaPayReq, ayaPayInterfaceUrl);
        writeLog(logAppender + "QR Request Confirmed");
        var referenceNo = string.Empty;
        if (ayaPayResponse.ResCode == "00")
        {
            writeLog(logAppender + "QR Confirm Success");
            status = "PE";
            desc = "Pending";
            paymentRefNo = ayaPayResponse.QrSchemeRefNo;
            qr = ayaPayResponse.QrInfo["MerDqrCode"].ToString();
            transExpiredTime = ayaPayResponse.QrInfo["TransExpiredTime"].ToString();
            referenceNo = ayaPayResponse.ReferenceNo;
        }
        else
        {
            writeLog(logAppender + "QR Confirm Fail");
            status = "RE";
            desc = ayaPayResponse.FailReason;
        }

        if (!_agentWCF.UpdateC2BMerchantTransaction(txnId, status, desc, paymentRefNo, qr, referenceNo, out errMsg))
        {
            writeLog(logAppender + "Error in UpdateC2BMerchantTransaction : " + errMsg);
            var errorMessage = "Transaction Failed";
            return Utils.getErrorRes("06", errorMessage);
        }

        writeLog(logAppender + "Transaction Updated");

        if (status == "RE")
        {
            return Utils.getErrorRes("06", desc);
        }

        var conRes = new ConfirmResponseModel();
        conRes.email = email;
        conRes.password = password;
        conRes.rescode = ayaPayResponse.ResCode;
        conRes.resdesc = ayaPayResponse.FailReason;
        conRes.ref1 = productDesc;
        conRes.ref2 = qr;
        conRes.ref3 = string.Empty;
        conRes.ref4 = transExpiredTime;
        conRes.ref5 = ref5;
        conRes.ref1Name = "Payment Type";
        conRes.ref2Name = "QR";
        conRes.ref3Name = string.Empty;
        conRes.ref4Name = "Txn Expired Time";
        conRes.ref5Name = "Mobile No.";
        conRes.txnID = txnId.ToString();
        conRes.smsMsg = smsMsg;
        return Utils.GetMerchantAcceptanceRes(conRes);
    }

    private static void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }
}