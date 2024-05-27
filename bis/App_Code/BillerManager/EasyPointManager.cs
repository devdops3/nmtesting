using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using A2AAPIWCF;

/// <summary>
/// Summary description for EasyPointManager
/// </summary>
public class EasyPointManager
{
	public EasyPointManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string ConfrimToEBA(ConfirmResponseModel confirmResponseModel, string amount, ResponseInfo responseInfo)
    {
        var messageId = confirmResponseModel.messageid + " | ";
        Utils.WriteLog_Biller("This is easyPoint biller Confirm : messageId :" + messageId);

        var confirmReq = PopulateEBAConfirmReq(responseInfo.txnID, amount);
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
            var detail = JsonConvert.DeserializeObject<EBAePinConfirmDetailResponse>(confirmRes.Detail);

            confirmResponseModel = PopulateConfirmResponse(detail, confirmResponseModel);

            if (!UpdateTransactionStatus(confirmResponseModel, responseInfo, out errMsg, out batchID))
            {
                return UpdateErrorStatus(responseInfo.txnID, errMsg, messageId);
            }
            else
            {
                Utils.WriteLog_Biller(messageId + "After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString() + "| smsStatus:" + responseInfo.smsStatus + "| MessageId : " + messageId);
            }

            Utils.WriteLog_Biller(messageId + "After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString() + "| MessageId : " + messageId);

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

    private void SendSMS(ResponseInfo responseInfo, ConfirmResponseModel confirmResponseModel, EBAePinConfirmDetailResponse detail, string amount, out string smsMsg)
    {
        var messageId = confirmResponseModel.messageid + " | ";
        smsMsg = string.Empty;
        SMSHelper smsH = new SMSHelper();

        smsMsg = smsH.getMessageBiller(responseInfo.agentName, responseInfo.MapTaxID, confirmResponseModel.billername, "Code", "Description", "", "Ref", confirmResponseModel.ref3, confirmResponseModel.ref2, "", responseInfo.txnID.ToString(),
       double.Parse(amount).ToString("#,##0.00"), responseInfo.serviceFee, double.Parse(responseInfo.totalAmount).ToString("#,##0.00"), responseInfo.branchCode);

        if (responseInfo.appType == "CS" || responseInfo.appType == "MS")
        {
            Utils.WriteLog_Biller(messageId + "appType is CS or MS");
            if (string.IsNullOrEmpty(responseInfo.topupType) || responseInfo.topupType == "S") //topup type is null or S
            {
                Utils.WriteLog_Biller(messageId + "topupType is null or Not S");
                MessagingService.MessagingServiceClient smsWcf = new MessagingService.MessagingServiceClient();
                try
                {

                    Utils.WriteLog_Biller(messageId + "Mobile No :" + confirmResponseModel.ref5 + "| Message :" + smsMsg + "| Sender Name :" + responseInfo.sendername + "|txn ID :" + responseInfo.txnID);
                    smsWcf.SendSms(responseInfo.txnID.ToString(), smsMsg, confirmResponseModel.ref5, responseInfo.sendername);
                    Utils.WriteLog_Biller(messageId + "sendSMSWithTxnID ends.");
                }
                catch (Exception ex)
                {
                    Utils.WriteLog_Biller(string.Format("{0} Error in SendSms: {1}", messageId, ex.ToString()));
                }
            }
        }

    }

    private string UpdateErrorStatus(long txnID, string errMsg, string messageId)
    {
        Utils.WriteLog_Biller(messageId + "Error in ConfirmUpdate : " + errMsg);
        string responseDescription = "Error in update database";
        string responseCode = "06";
        if (!(new ServiceClient()).updateError(txnID, "ER", responseDescription, out errMsg))
        {
            Utils.WriteLog_Biller(messageId + "Error in updateError : " + errMsg);
        }
        return Utils.getErrorRes(responseCode, "Transaction fail");

    }

    private bool IsSuccess(string errorCode)
    {
        if (errorCode == "00")
            return true;
        else return false;
    }

    private EbaConfirmReq PopulateEBAConfirmReq(long txnID, string amount)
    {
        var token = TokenManager.GetOAuthToken();
        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = txnID.ToString(),
            BillerCode = ConfigurationManager.AppSettings["EasyPointBillerCode"].ToString(),
            TransactionAmount = amount,
            Detail = "{'Deno':'" + amount + "'}"
        };

        return confirmReq;

    }

    private string ConfirmEBA(EbaConfirmReq confirmReq, string messageId)
    {
        string ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();

        string jsonReq = JsonConvert.SerializeObject(confirmReq);
        Utils.WriteLog_Biller("EasyPoint EBA Confirm Jason Request:" + jsonReq + "MessageId : " + messageId);

        SSLPost post = new SSLPost();
        string jsonres = Utils.PostEba(jsonReq, ebaUrl);
        Utils.WriteLog_Biller("EasyPoint EBA Confirm Jason Response:" + jsonres + "MessageId : " + messageId);

        return jsonres;

    }

    private ConfirmResponseModel PopulateConfirmResponse(EBAePinConfirmDetailResponse confirmDetail, ConfirmResponseModel confirmResponseModel)
    {
        string aesKey = string.Empty;
        aesKey = ConfigurationManager.AppSettings["EsbaAesKey"].ToString();
        confirmDetail.ClearPin = Utils.AESDecryptText(confirmDetail.ClearPin, aesKey);
        if (string.IsNullOrEmpty(confirmDetail.ExpiryDate))
        {
            confirmDetail.ExpiryDate = "-";
        }
        confirmResponseModel.ref1 = confirmDetail.SerialNumber;
        confirmResponseModel.ref3 = confirmDetail.ClearPin;

        return confirmResponseModel;
    }

}