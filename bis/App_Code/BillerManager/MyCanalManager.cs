using A2AAPIWCF;
using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Configuration;

/// <summary>
/// Summary description for MyCanalManager
/// </summary>
public class MyCanalManager
{
    public MyCanalManager()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string ConfrimToEBA(ConfirmResponseModel confirmResponseModel, string amount, ResponseInfo responseInfo)
    {
        var messageId = confirmResponseModel.messageid;
        Utils.WriteLog_Biller("This is mycanal biller Confirm : messageId : " + messageId);

        var confirmReq = PopulateEBAConfirmReq(responseInfo.txnID, amount, confirmResponseModel.ref5);
        var jsonres = ConfirmEBA(confirmReq, messageId);
        var responseDescription = string.Empty;
        var responseCode = string.Empty;
        if (string.IsNullOrEmpty(jsonres))
        {
            responseDescription = "No Response From EBA";
            responseCode = "06";

            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(responseCode, responseDescription, responseInfo.txnID, responseDescription, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }

        var confirmRes = JsonConvert.DeserializeObject<EbaConfirmRes>(jsonres);
        var errMsg = string.Empty;
        var batchID = default(int);
        if (IsSuccess(confirmRes.ErrorCode))
        {
            var detail = JsonConvert.DeserializeObject<SmileCinemaEBAConfirmDetailResponse>(confirmRes.Detail);

            confirmResponseModel = PopulateConfirmResponse(detail, confirmResponseModel);

            if (!UpdateTransactionStatus(confirmResponseModel, responseInfo, out errMsg, out batchID))
            {
                return UpdateErrorStatus(responseInfo.txnID, errMsg);
            }
            else
            {
                Utils.WriteLog_Biller("After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString() + "| smsStatus:" + responseInfo.smsStatus);
            }

            Utils.WriteLog_Biller("After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString());

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

    private void SendSMS(ResponseInfo responseInfo, ConfirmResponseModel confirmResponseModel, SmileCinemaEBAConfirmDetailResponse detail, string amount, out string smsMsg)
    {
        smsMsg = string.Empty;
        if (responseInfo.appType == "CS" || responseInfo.appType == "MS")
        {
            Utils.WriteLog_Biller("appType is CS or MS");
            if (string.IsNullOrEmpty(responseInfo.topupType) || responseInfo.topupType == "S") //topup type is null or S
            {
                Utils.WriteLog_Biller("topupType is null or Not S");
                SMSHelper smsH = new SMSHelper();
                MessagingService.MessagingServiceClient smsWcf = new MessagingService.MessagingServiceClient();

                smsMsg = smsH.getMessageBiller(responseInfo.agentName, responseInfo.MapTaxID, confirmResponseModel.billername, "Serial No.", "Code", "Expiry", "Ref", detail.SerialNumber, detail.ClearPin, detail.ExpiryDate, responseInfo.txnID.ToString(), double.Parse(amount).ToString("#,##0.00"), responseInfo.serviceFee, double.Parse(responseInfo.totalAmount).ToString("#,##0.00"), responseInfo.branchCode);

                try
                {

                    Utils.WriteLog_Biller("Mobile No :" + confirmResponseModel.ref5 + "| Message :" + smsMsg + "| Sender Name :" + responseInfo.sendername + "|txn ID :" + responseInfo.txnID);
                    smsWcf.SendSms(responseInfo.txnID.ToString(), smsMsg, confirmResponseModel.ref5, responseInfo.sendername);
                    Utils.WriteLog_Biller("sendSMSWithTxnID ends.");
                }
                catch (Exception ex)
                {
                    Utils.WriteLog_Biller(string.Format("Error in SendSms: {0}", ex.ToString()));
                }
            }
        }

    }

    private string UpdateErrorStatus(long txnID, string errMsg)
    {
        Utils.WriteLog_Biller("Error in ConfirmUpdate : " + errMsg);
        string responseDescription = "Error in update database";
        string responseCode = "06";
        if (!(new ServiceClient()).updateError(txnID, "ER", responseDescription, out errMsg))
        {
            Utils.WriteLog_Biller("Error in updateError : " + errMsg);
        }
        return Utils.getErrorRes(responseCode, "Transaction fail");

    }

    private bool IsSuccess(string errorCode)
    {
        if (errorCode == "00")
            return true;
        else return false;
    }

    private EbaConfirmReq PopulateEBAConfirmReq(long txnID, string amount, string productCode)
    {
        var token = TokenManager.GetOAuthToken();
        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = txnID.ToString(),
            BillerCode = ConfigurationManager.AppSettings["myCanalBillerCode"].ToString(),
            TransactionAmount = amount,
            Detail = "{'ProductCode':'" + productCode + "'}"
        };

        return confirmReq;

    }

    private string ConfirmEBA(EbaConfirmReq confirmReq, string messageId)
    {
        string ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();

        string jsonReq = JsonConvert.SerializeObject(confirmReq);
        Utils.WriteLog_Biller("myCanal EBA Confirm Jason Request:" + jsonReq + "MessageId : " + messageId);

        SSLPost post = new SSLPost();
        string jsonres = Utils.PostEba(jsonReq, ebaUrl);
        Utils.WriteLog_Biller("myCanal EBA Confirm Jason Response:" + jsonres + "MessageId : " + messageId);

        return jsonres;

    }

    private ConfirmResponseModel PopulateConfirmResponse(SmileCinemaEBAConfirmDetailResponse confirmDetail, ConfirmResponseModel confirmResponseModel)
    {
        string aesKey = string.Empty;
        aesKey = ConfigurationManager.AppSettings["EsbaAesKey"].ToString();
        confirmDetail.ClearPin = Utils.AESDecryptText(confirmDetail.ClearPin, aesKey);
        if (string.IsNullOrEmpty(confirmDetail.ExpiryDate))
        {
            confirmDetail.ExpiryDate = "-";
        }
        Utils.WriteLog_Biller("myCanal ExpiryDate : " + confirmDetail.ExpiryDate);
        confirmResponseModel.ref5 = confirmResponseModel.ref3;
        confirmResponseModel.ref1 = confirmDetail.SerialNumber;
        confirmResponseModel.ref2 = confirmDetail.ClearPin + " " + confirmDetail.ExpiryDate;
        confirmResponseModel.ref3 = confirmDetail.ProductDescription;

        return confirmResponseModel;
    }
}