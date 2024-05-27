using A2AAPIWCF;
using EbaReqResModel;
using Newtonsoft.Json;
using System;

/// <summary>
/// Summary description for EasyMicrofinanceManager
/// </summary>
public class EasyMicrofinanceManager
{
	public EasyMicrofinanceManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string GetInquiryResponse(inquiryResponseModel inquiryResponseModel)
    {

        Utils.WriteLog_Biller("This is Easy MicorFinance Inquiry.");


        var detail = new
        {
            LoanAccountNumber=inquiryResponseModel.ref1
        };
        var inquiryRequest = new
        {
            Token = TokenManager.GetOAuthToken().Token,
            PartnerCode = BISConstants.ESBAChannel,
            BillerCode = BISConstants.EasyMicroFinanceBillerCode,
            Detail = "{ 'LoanAccountNumber' : '" + detail.LoanAccountNumber+ "'}"
        };


        string json = JsonConvert.SerializeObject(inquiryRequest);

        Utils.WriteLog_Biller("Eba inquriy request for Easy Microfinance" + " : " + json);

        string responseJson = Utils.PostEba(json, BISConstants.EBAInquiryUrl);

        Utils.WriteLog_Biller("Eba inquriy response for Easy Microfinance" + " : " + responseJson);

        var inquiryResponse = JsonConvert.DeserializeObject<EbaInquiryRes>(responseJson);

        if (!string.IsNullOrEmpty(responseJson) & inquiryResponse.ErrorCode == "00")
        {
            string responseCode = inquiryResponse.ErrorCode;
            string responseDescription = inquiryResponse.ErrorMessage;
            string detailJson = inquiryResponse.Detail.Replace("\\", "");
            var detailResponse = JsonConvert.DeserializeObject<EasyMicrofinanceDetailResponse>(detailJson);

            inquiryResponseModel.ResCode = inquiryResponse.ErrorCode;
            inquiryResponseModel.ResDesc = inquiryResponse.ErrorMessage;
            inquiryResponseModel.ref1 = detail.LoanAccountNumber;
            inquiryResponseModel.ref2 = detailResponse.Loans[0].LoanId;
            inquiryResponseModel.ref3 = detailResponse.clientInfo.FullName;
          
            inquiryResponseModel.amount = detailResponse.Loans[0].InstallationAmount;
            inquiryResponseModel.status = inquiryResponse.TransactionStatus;
            

            return Utils.getInquiryRes(inquiryResponseModel);
        }
        else
        {
            return Utils.getErrorRes(inquiryResponse.ErrorCode, inquiryResponse.ErrorMessage);
        }
    }

    public string ConfrimToEBA(ConfirmResponseModel confirmResponseModel, string amount, int agentId, long txnID,
        double agentAmount, string isAgreement, double agentFeeDbl, string smsStatus, double availablebalance,
        string appType, string topupType, string agentName, string MapTaxID, string serviceFee, string totalAmount
        , string branchCode, string senderName)
    {
        Utils.WriteLog_Biller("This is Easy Microfinance from EBA Confirm");
        var token = TokenManager.GetOAuthToken();

        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = BISConstants.ESBAChannel,
            PartnerRefNo = confirmResponseModel.txnID.ToString(),
            BillerCode = BISConstants.EasyMicroFinanceBillerCode,
            TransactionAmount = amount,
            Detail = "{'LoanAccountNumber':'" + confirmResponseModel.ref1 + "', 'LoanId':'" + confirmResponseModel.ref2 + "', 'MobileNumber':'" + confirmResponseModel.ref5 + "', 'Amount':'" + amount + "'}"
        };

        string jsonReq = JsonConvert.SerializeObject(confirmReq);
        Utils.WriteLog_Biller("EBA Easy Microfinance Confirm Jason Request:" + jsonReq);

        SSLPost post = new SSLPost();
        string jsonres = Utils.PaymentRequest(jsonReq, BISConstants.EBAConfirmUrl);
        Utils.WriteLog_Biller("EBA Easy Microfinance Confirm Jason Response:" + jsonres);

        var responseDescription = string.Empty;
        var responseCode = string.Empty;
        if (string.IsNullOrEmpty(jsonres))
        {
            responseDescription = "No Response From EBA";
            responseCode = "06";

            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(responseCode, responseDescription, txnID, responseDescription, agentId, agentAmount, isAgreement);
        }


        var confirmRes = JsonConvert.DeserializeObject<EbaConfirmRes>(jsonres);
        var errMsg = string.Empty;
        var batchID = default(int);
        if (confirmRes.ErrorCode == "00")
        {
            if (!(new ServiceClient()).ConfirmUpdate(txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                    confirmResponseModel.ref5, "", "PA", "Paid Successfully", agentId, confirmResponseModel.email, agentAmount, agentFeeDbl,
                    isAgreement, smsStatus, availablebalance, out errMsg, out batchID))
            {
                Utils.WriteLog_Biller("Error in ConfirmUpdate : " + errMsg);
                responseDescription = "Error in update database";
                responseCode = "06";
                if (!(new ServiceClient()).updateError(txnID, "ER", responseDescription, out errMsg))
                {
                    Utils.WriteLog_Biller("Error in updateError : " + errMsg);
                }
                return Utils.getErrorRes(responseCode, "Transaction fail");
            }
            else
            {
                Utils.WriteLog_Biller("After update = AgentAmount : " + agentAmount + " | Balance : " + availablebalance.ToString() + "| smsStatus:" + smsStatus);
            }

            Utils.WriteLog_Biller("After update = AgentAmount : " + agentAmount + " | Balance : " + availablebalance.ToString());

            var detail = JsonConvert.DeserializeObject<MobileLegendEBAConfirmDetailResponse>(confirmRes.Detail);

            #region <-- Send SMS -->
            string smsMsg = string.Empty;
            if (appType == "CS" || appType == "MS")
            {
                Utils.WriteLog_Biller("appType is CS or MS");
                if (string.IsNullOrEmpty(topupType) || topupType == "S") //topup type is null or S
                {
                    Utils.WriteLog_Biller("topupType is null or Not S");
                    SMSHelper smsH = new SMSHelper();
                    MessagingService.MessagingServiceClient smsWcf = new MessagingService.MessagingServiceClient();

                    smsMsg = smsH.getMessageBiller(agentName, MapTaxID, confirmResponseModel.billername, string.Empty, confirmResponseModel.ref2Name, confirmResponseModel.ref3Name, "Ref", string.Empty,
                          confirmResponseModel.ref2, confirmResponseModel.ref3, txnID.ToString(), double.Parse(amount).ToString("#,##0.00"), serviceFee, double.Parse(totalAmount).ToString("#,##0.00"), branchCode);

                    try
                    {

                        Utils.WriteLog_Biller("Mobile No :" + confirmResponseModel.ref5 + "| Message :" + smsMsg + "| Sender Name :" + senderName + "|txn ID :" + txnID);
                        smsWcf.SendSms(txnID.ToString(), smsMsg, confirmResponseModel.ref5, senderName);
                        Utils.WriteLog_Biller("sendSMSWithTxnID ends.");
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLog_Biller(string.Format("Error in SendSms: {0}", ex.ToString()));
                    }
                }
            }

            #endregion

            #region <-- Response Back To Client -->
            confirmResponseModel.rescode = confirmRes.ErrorCode;
            confirmResponseModel.resdesc = confirmRes.ErrorMessage;
            confirmResponseModel.availablebalance = availablebalance.ToString();
            confirmResponseModel.txnID = txnID.ToString();
            confirmResponseModel.smsMsg = smsMsg;
            return Utils.getConfirmRes(confirmResponseModel);

            #endregion
        }
        else
        {
            responseDescription = Utils.EsbResponseDescription(confirmRes.ErrorCode);
            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(confirmRes.ErrorCode, confirmRes.ErrorMessage,
                txnID, responseDescription, agentId, agentAmount, isAgreement);
        }
    }
}