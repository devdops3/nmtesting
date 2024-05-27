using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SmsService
/// </summary>
public class SmsService
{
    public void SendSMS(ResponseInfo responseInfo, SmsData data, out string smsMsg)
    {
        var messageId = data.MessageId + " | ";
        smsMsg = string.Empty;
        if (responseInfo.appType == "CS" || responseInfo.appType == "MS")
        {
            Utils.WriteLog_Biller(messageId + "appType is CS or MS");
            if (string.IsNullOrEmpty(responseInfo.topupType) || responseInfo.topupType == "S") //topup type is null or S
            {
                Utils.WriteLog_Biller(messageId + "topupType is null or Not S");
                SMSHelper smsH = new SMSHelper();
                MessagingService.MessagingServiceClient smsWcf = new MessagingService.MessagingServiceClient();
                smsMsg = smsH.getMessageBiller(responseInfo.agentName, responseInfo.MapTaxID, data.BillerName, 
                                               data.Ref1Name, data.Ref2Name, data.Ref3Name, data.Ref4Name, 
                                               data.Ref1Value, data.Ref2Value, data.Ref3Value, data.Ref4Value,
                                               double.Parse(responseInfo.amount).ToString("#,##0.00"), 
                                               responseInfo.serviceFee, double.Parse(responseInfo.totalAmount).ToString("#,##0.00"), 
                                               responseInfo.branchCode);
                try
                {

                    Utils.WriteLog_Biller(messageId + "Mobile No :" + data.MobileNo + "| Message :" + smsMsg + "| Sender Name :" + responseInfo.sendername + "|txn ID :" + responseInfo.txnID);

                    smsWcf.SendSms(responseInfo.txnID.ToString(), smsMsg, data.MobileNo, responseInfo.sendername);
                    Utils.WriteLog_Biller(messageId + "sendSMSWithTxnID ends.");
                }
                catch (Exception ex)
                {
                    Utils.WriteLog_Biller(string.Format("{0} Error in SendSms: {1}", messageId, ex.ToString()));
                }
            }
        }
    }
}