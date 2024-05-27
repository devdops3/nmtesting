using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using A2AAPIWCF;

/// <summary>
/// Summary description for InquiryEBAGiftCardEPinManager
/// </summary>
public class EBAGiftCardEPinManager
{
	public EBAGiftCardEPinManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string getInquiryResponse(inquiryResponseModel responseModel, string messageId, string billerCode)
    {
        var logMessageId = "MessageId : " + messageId + " | ";
        var inquiryRequest = new
        {
            Token = TokenManager.GetOAuthToken().Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            BillerCode = billerCode,
            Detail = "{ 'Deno' : '" + responseModel.ref2 + "'}"
        };

        string json = JsonConvert.SerializeObject(inquiryRequest);

        Utils.WriteLog_Biller(logMessageId + "Eba inquriy request for BillerCode " + billerCode + " : " + json);

        string responseJson = Utils.PostEba(json, BISConstants.EBAInquiryUrl);

        Utils.WriteLog_Biller(logMessageId + "Eba inquriy response for BillerCode " + billerCode + " : " + responseJson);

        var inquiryResponse = JsonConvert.DeserializeObject<EbaInquiryRes>(responseJson);

        if (!string.IsNullOrEmpty(responseJson) & inquiryResponse.ErrorCode == "00")
        {
            responseModel.ResCode = inquiryResponse.ErrorCode;
            responseModel.ResDesc = inquiryResponse.ErrorMessage;
            responseModel.amount = inquiryResponse.TransactionAmount;

            return Utils.getInquiryRes(responseModel);
        }
        else
        {
            return Utils.getErrorRes(inquiryResponse.ErrorCode, inquiryResponse.ErrorMessage);
        }
    }

    public string ConfrimToEBA(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, string amount, string billerCode)
    {
        var messageId = confirmResponseModel.messageid;
        Utils.WriteLog_Biller("This is " + billerCode + " biller Confirm : messageId :" + messageId);

        var confirmReq = PopulateEBAConfirmReq(responseInfo.txnID, amount, billerCode, confirmResponseModel.ref2);
        var jsonres = ConfirmEBA(confirmReq, messageId);
        var responseDescription = string.Empty;
        var responseCode = string.Empty;
        if (string.IsNullOrEmpty(jsonres))
        {
            responseDescription = "No Response From EBA";
            responseCode = "97";

            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(responseCode, responseDescription, responseInfo.txnID, responseDescription, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }

        var confirmRes = JsonConvert.DeserializeObject<EbaConfirmRes>(jsonres);
        var errMsg = string.Empty;
        var batchID = default(int);
        if (IsSuccess(confirmRes.ErrorCode))
        {
            var detail = JsonConvert.DeserializeObject<EBAGiftCardEPinDetail>(confirmRes.Detail);

            confirmResponseModel = PopulateConfirmResponse(detail, confirmResponseModel);

            if (!UpdateTransactionStatus(confirmResponseModel, responseInfo, out errMsg, out batchID))
            {
                return UpdateErrorStatus(responseInfo.txnID, errMsg, confirmResponseModel.messageid);
            }
            else
            {
                Utils.WriteLog_Biller("After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString() + "| smsStatus:" + responseInfo.smsStatus + "| MessageId : " + messageId);
            }

            Utils.WriteLog_Biller("After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString() + "| MessageId : " + messageId);

            string smsMsg = string.Empty;
            SendSMS(responseInfo, confirmResponseModel, detail, amount, out smsMsg);

            #region <-- Response Back To Client -->
            confirmResponseModel.rescode = confirmRes.ErrorCode;
            confirmResponseModel.resdesc = confirmRes.ErrorMessage;
            confirmResponseModel.availablebalance = responseInfo.availablebalance.ToString();
            confirmResponseModel.txnID = responseInfo.txnID.ToString();

            confirmResponseModel.smsMsg = smsMsg;
            return Utils.getConfirmRes(confirmResponseModel);

            #endregion
        }
        else
        {
            responseDescription = Utils.EsbResponseDescription(confirmRes.ErrorCode);
            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(confirmRes.ErrorCode, confirmRes.ErrorMessage,
                responseInfo.txnID, responseDescription, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }

    }

    private bool UpdateTransactionStatus(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, out string errMsg, out int batchID)
    {
        ServiceClient A2AAPIwcfService = new ServiceClient();
        return A2AAPIwcfService.ConfirmUpdate(responseInfo.txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                    confirmResponseModel.ref5, "", "PA", "Paid Successfully", responseInfo.agentID, confirmResponseModel.email, responseInfo.agentAmount, responseInfo.agentFeeDbl,
                    responseInfo.isAgreement, responseInfo.smsStatus, responseInfo.availablebalance, out errMsg, out batchID);
    }

    private void SendSMS(ResponseInfo responseInfo, ConfirmResponseModel confirmResponseModel, EBAGiftCardEPinDetail detail, string amount, out string smsMsg)
    {
        var messageId = confirmResponseModel.messageid + " | ";
        smsMsg = string.Empty;
        if (responseInfo.appType == "CS" || responseInfo.appType == "MS")
        {
            Utils.WriteLog_Biller(messageId + "appType is CS or MS");
            if (string.IsNullOrEmpty(responseInfo.topupType) || responseInfo.topupType == "S") //topup type is null or S
            {
                Utils.WriteLog_Biller(messageId + "topupType is null or Not S");
                SMSHelper smsH = new SMSHelper();
                MessagingService.MessagingServiceClient smsWcf = new MessagingService.MessagingServiceClient();
                Utils.WriteLog_Biller(messageId + "ExpiryDate : " + detail.ExpiryDate);
                Utils.WriteLog_Biller(messageId  +"clearPin : " + detail.ClearPin);
                smsMsg = smsH.getMessageBiller(responseInfo.agentName, responseInfo.MapTaxID, confirmResponseModel.billername, "Code", "Expiry", "", "Ref", detail.ClearPin, detail.ExpiryDate, "", responseInfo.txnID.ToString(),
                       double.Parse(amount).ToString("#,##0.00"), responseInfo.serviceFee, double.Parse(responseInfo.totalAmount).ToString("#,##0.00"), responseInfo.branchCode);

                try
                {

                    Utils.WriteLog_Biller(messageId + "Mobile No :" + confirmResponseModel.ref5 + "| Message :" + smsMsg + "| Sender Name :" + responseInfo.sendername + "|txn ID :" + responseInfo.txnID);
                    smsWcf.SendSms(responseInfo.txnID.ToString(), smsMsg, confirmResponseModel.ref5, responseInfo.sendername);
                    Utils.WriteLog_Biller(messageId + "sendSMSWithTxnID ends.");
                }
                catch (Exception ex)
                {
                    Utils.WriteLog_Biller(string.Format("{0} Error in SendSms: {1}",messageId, ex.ToString()));
                }
            }
        }

    }

    private string UpdateErrorStatus(long txnID, string errMsg, string messageId)
    {
        Utils.WriteLog_Biller(messageId + " Error in ConfirmUpdate : " + errMsg);
        string responseDescription = "Error in update database";
        string responseCode = "06";
        if (!(new ServiceClient()).updateError(txnID, "ER", responseDescription, out errMsg))
        {
            Utils.WriteLog_Biller(messageId + " Error in updateError : " + errMsg);
        }
        return Utils.getErrorRes(responseCode, "Transaction fail");

    }

    private bool IsSuccess(string errorCode)
    {
        if (errorCode == "00")
            return true;
        else return false;
    }

    private EbaConfirmReq PopulateEBAConfirmReq(long txnID, string amount, string billerCode, string deno)
    {
        var token = TokenManager.GetOAuthToken();
        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = txnID.ToString(),
            BillerCode = billerCode,
            TransactionAmount = amount,
            Detail = "{'Deno':'" + deno + "'}"
        };

        return confirmReq;

    }

    private string ConfirmEBA(EbaConfirmReq confirmReq, string messageId)
    {
        string ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();

        string jsonReq = JsonConvert.SerializeObject(confirmReq);
        Utils.WriteLog_Biller(confirmReq.BillerCode + " EBA Confirm Jason Request:" + jsonReq + "MessageId : " + messageId);

        SSLPost post = new SSLPost();
        string jsonres = Utils.PostEba(jsonReq, ebaUrl);
        Utils.WriteLog_Biller(confirmReq.BillerCode + " EBA Confirm Jason Response:" + jsonres + "MessageId : " + messageId);

        return jsonres;

    }

    private ConfirmResponseModel PopulateConfirmResponse(EBAGiftCardEPinDetail confirmDetail, ConfirmResponseModel confirmResponseModel)
    {
        confirmDetail.ClearPin = getClearPin(confirmDetail.ClearPin);

        if (string.IsNullOrEmpty(confirmDetail.ExpiryDate))
            confirmDetail.ExpiryDate = "-";
        confirmResponseModel.ref3 = confirmDetail.SerialNumber;
        confirmResponseModel.ref4 = confirmDetail.ClearPin  + " " + confirmDetail.ExpiryDate;

        return confirmResponseModel;
    }

    private string getClearPin(string encryptedPin)
    {
        string aesKey = string.Empty;
        aesKey = ConfigurationManager.AppSettings["EsbaAesKey"].ToString();
        encryptedPin = Utils.AESDecryptText(encryptedPin, aesKey);
        return encryptedPin;
    }

}