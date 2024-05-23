using A2AAPIWCF;
using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GrabRideManager
/// </summary>
public class GrabRideManager : IeService
{
    public string Inquiry(inquiryModel model)
    {
        throw new NotImplementedException();
    }

    public string Confirm(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo)
    {
        var messageId = confirmResponseModel.messageid;
        Utils.WriteLog_Biller("This is " + responseInfo.billerCode + " biller Confirm : messageId :" + messageId);

        var confirmReq = PopulateEBAConfirmReq(responseInfo.txnID, responseInfo.amount, confirmResponseModel.ref2, responseInfo.billerCode);
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
            var detail = JsonConvert.DeserializeObject<EBAPinDetail>(confirmRes.Detail);

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

            #region Send Sms
            string smsMsg = string.Empty;
            var smsData = new SmsData()
            {
                BillerName = confirmResponseModel.billername,
                MobileNo = confirmResponseModel.ref5,
                MessageId = confirmResponseModel.messageid,
                Ref1Name = "Activation Url",
                Ref2Name = "Expiry",
                Ref4Name = "Ref",
                Ref1Value = detail.ActivationUrl,
                Ref2Value = detail.ExpiryDate,
                Ref4Value = responseInfo.txnID.ToString()
            };

            SmsService smsService = new SmsService();
            smsService.SendSMS(responseInfo, smsData, out smsMsg);

            #endregion

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

    private EbaConfirmReq PopulateEBAConfirmReq(long txnID, string amount, string productCode, string billerCode)
    {
        var token = TokenManager.GetOAuthToken();
        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = txnID.ToString(),
            BillerCode = billerCode,
            TransactionAmount = amount,
            Detail = "{'ProductCode':'" + productCode + "'}"
        };

        return confirmReq;
    }

    private bool UpdateTransactionStatus(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, out string errMsg, out int batchID)
    {
        ServiceClient A2AAPIwcfService = new ServiceClient();
        return A2AAPIwcfService.ConfirmUpdate(responseInfo.txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                    confirmResponseModel.ref5, "", "PA", "Paid Successfully", responseInfo.agentID, confirmResponseModel.email, responseInfo.agentAmount, responseInfo.agentFeeDbl,
                    responseInfo.isAgreement, responseInfo.smsStatus, responseInfo.availablebalance, out errMsg, out batchID);
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

    private ConfirmResponseModel PopulateConfirmResponse(EBAPinDetail confirmDetail, ConfirmResponseModel confirmResponseModel)
    {
        if (string.IsNullOrEmpty(confirmDetail.ExpiryDate))
        {
            confirmDetail.ExpiryDate = "-";
            confirmResponseModel.ref4Name = string.Empty;
        }
        confirmResponseModel.ref5 = confirmResponseModel.ref3;
        confirmResponseModel.ref3 = confirmDetail.CardNumber;
        confirmResponseModel.ref4 = confirmDetail.ActivationUrl + " " + confirmDetail.ExpiryDate;

        return confirmResponseModel;
    }
}