using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using A2AAPIWCF;
using System.Configuration;

/// <summary>
/// Summary description for ViberOutManager
/// </summary>
public class ViberOutManager
{
	public ViberOutManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string ConfrimToEBA(ConfirmResponseModel confirmResponseModel, string amount, ResponseInfo responseInfo)
    {
        var messageId = confirmResponseModel.messageid + " | ";
        Utils.WriteLog_Biller("This is ViberOut biller Confirm : messageId : " + messageId);

        var confirmReq = PopulateEBAConfirmReq(responseInfo.txnID, amount, confirmResponseModel.ref2, confirmResponseModel.ref5);
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
            confirmResponseModel.ref4 = string.Format("SUCCESS {0}", confirmRes.EBARefNo);

            if (!UpdateTransactionStatus(confirmResponseModel, responseInfo, out errMsg, out batchID))
            {
                return UpdateErrorStatus(responseInfo.txnID, errMsg, confirmResponseModel.messageid);
            }
            else
            {
                Utils.WriteLog_Biller(messageId + "After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString() + "| smsStatus:" + responseInfo.smsStatus);
            }

            Utils.WriteLog_Biller(messageId + "After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString());

            string smsMsg = string.Empty;

            #region <-- Response Back To Client -->
            confirmResponseModel.rescode = confirmRes.ErrorCode;
            confirmResponseModel.resdesc = confirmRes.ErrorMessage;
            confirmResponseModel.availablebalance = responseInfo.availablebalance.ToString();
            confirmResponseModel.txnID = responseInfo.txnID.ToString();

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

    private EbaConfirmReq PopulateEBAConfirmReq(long txnID, string amount, string deno, string mobileNo)
    {
        var token = TokenManager.GetOAuthToken();
        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = txnID.ToString(),
            BillerCode = ConfigurationManager.AppSettings["ViberOutBillerCode"].ToString(),
            TransactionAmount = amount,
            Detail = "{'Deno':'" + deno
                    + "', 'MobileNumber':'" + mobileNo
                    + "'}"
        };

        return confirmReq;

    }

    private string ConfirmEBA(EbaConfirmReq confirmReq, string messageId)
    {
        string ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();

        string jsonReq = JsonConvert.SerializeObject(confirmReq);
        Utils.WriteLog_Biller("ViberOut EBA Confirm Jason Request:" + jsonReq + "MessageId : " + messageId);

        SSLPost post = new SSLPost();
        string jsonres = Utils.PostEba(jsonReq, ebaUrl);
        Utils.WriteLog_Biller("ViberOut EBA Confirm Jason Response:" + jsonres + "MessageId : " + messageId);

        return jsonres;

    }
}