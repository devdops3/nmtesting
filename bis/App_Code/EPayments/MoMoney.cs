using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MoMoneyService
/// </summary>
public class MoMoney : IEPayment
{
    private static A2AAPIWCF.ServiceClient _agentWCF;
    public MoMoney()
    {
        _agentWCF = new A2AAPIWCF.ServiceClient();
    }
    public string Confirm(EPaymentMerchant merchant, long txnId, string ref1, string ref2, string ref3, string ref4, string ref5, 
        string email, string password, string amount, string branchCode, string agentName, string appType
        ,string productDesc)
    {
        Utils.WriteLog_Biller("This is MoMoney Pay");
        Dictionary<string, string> paymentInfo = new Dictionary<string, string>();
        paymentInfo.Add("Auth_Code", ref1);
        paymentInfo.Add("DeviceId", ref2);

        Dictionary<string, string> paymentProfileInfo = new Dictionary<string, string>();
        paymentProfileInfo.Add("MerchantId", merchant.MerchantId);
        paymentProfileInfo.Add("SecretKey", merchant.SecretKey);
        paymentProfileInfo.Add("ClientId", merchant.ClientId);
        paymentProfileInfo.Add("ClientSecret", merchant.ClientSecret);

        var request = new EPaymentInterfaceRequest()
        {
            NearMePaymentRefNo = "NearMe" + txnId,
            Amount = Decimal.Parse(amount),
            PaymentInfo = paymentInfo,
            PaymentProfileInfo = paymentProfileInfo
        };

        string moMoneyPayRequest = JsonConvert.SerializeObject(request);
        string moMoneyPayInterfaceUrl = ConfigurationManager.AppSettings["MoMoneyInterfaceUrl"];
        var moMoneyPayManager = new MoMoneyManager();
        var moMoneyPayResponse = moMoneyPayManager.Confirm(moMoneyPayRequest, moMoneyPayInterfaceUrl);

        var status = string.Empty;
        var desc = string.Empty;
        var paymentRefNo = string.Empty;
        if (moMoneyPayResponse.ResCode == "00")
        {
            status = "AP";
            desc = "Approved";
            paymentRefNo = moMoneyPayResponse.PaymentSchemeRefNo;
        }
        else
        {
            status = "RE";
            desc = moMoneyPayResponse.FailReason;
        }

        var errMsg = string.Empty;
        if (!_agentWCF.UpdateMerchantTransaction(txnId, status, desc, paymentRefNo, string.Empty, out errMsg))
        {
            Utils.WriteLog_Biller("Error in UpdateMerchantTransaction : " + errMsg);
            var errorMessage = "Transaction Failed";
            return Utils.getErrorRes("06", errorMessage);
        }
        if (status == "RE")
        {
            return Utils.getErrorRes("06", desc);
        }

        var smsMsg = string.Empty;
        if (status == "AP" && appType == "MS" && !string.IsNullOrEmpty(ref5))
        {
            var sms = new SMSHelper();
            var msgService = new MessagingService.MessagingServiceClient();
            smsMsg = sms.GetMerchantMessage(txnId.ToString(), agentName, string.Empty, ref2, "PaymentType", "PaymentCode", string.Empty, string.Empty, ref2, ref1, string.Empty, string.Empty, amount, branchCode);
            var isSend = msgService.SendSms("", smsMsg, ref5, "NearMe");
            if (isSend._success)
            {
                Utils.WriteLog_Biller("Send sms successfully");
                _agentWCF.updateSMSStatusforMerchant(Convert.ToInt64(txnId), "Y", out errMsg);
            }
        }

        var conRes = new ConfirmResponseModel();
        conRes.email = email;
        conRes.password = password;
        conRes.rescode = moMoneyPayResponse.ResCode;
        conRes.resdesc = moMoneyPayResponse.FailReason;
        conRes.ref1 = productDesc;
        conRes.ref2 = ref1;
        conRes.ref3 = string.Empty;
        conRes.ref4 = string.Empty;
        conRes.ref5 = ref5;
        conRes.ref1Name = "Payment Type";
        conRes.ref2Name = string.Empty;
        conRes.ref3Name = string.Empty;
        conRes.ref4Name = string.Empty;
        conRes.ref5Name = "Mobile No.";
        conRes.txnID = txnId.ToString();
        conRes.smsMsg = smsMsg;
        return Utils.GetMerchantAcceptanceRes(conRes);
    }
}