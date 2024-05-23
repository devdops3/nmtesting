using A2AAPIWCF;
using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Configuration;

/// <summary>
/// Summary description for HanaMicrofinanceManager
/// </summary>
public class HanaMicrofinanceManager
{
    public HanaMicrofinanceManager()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string GetInquiryResponse(inquiryResponseModel inquiryResponseModel)
    {

        Utils.WriteLog_Biller("This is Hana MicorFinance Inquiry.");

        string hanaBillercode = string.Empty;
        string hanaPaymentId = string.Empty;
        string hanaPaymentType = string.Empty;

        Utils.WriteLog_Biller("Hana Microfinance Ref2 for Inquiry Request:" + inquiryResponseModel.ref2);

        hanaPaymentType = inquiryResponseModel.ref2;

        if (inquiryResponseModel.ref2 == "Loan Payment")
        {
            hanaBillercode = ConfigurationManager.AppSettings["HanaMicrofinanceLoanBillerCode"].ToString();
            hanaPaymentId = ConfigurationManager.AppSettings["HanaMicrofinanceLoanID"].ToString();
        }

        if (inquiryResponseModel.ref2 == "Saving Payment")
        {
            hanaBillercode = ConfigurationManager.AppSettings["HanaMicrofinanceSavingBillerCode"].ToString();
            hanaPaymentId = ConfigurationManager.AppSettings["HanaMicrofinanceSavingID"].ToString();
        }

        Utils.WriteLog_Biller("Hana Microfinance Inquiry hanaPayment Id:" + hanaPaymentId);

        var detail = new
        {
            LoanAccountNumber = inquiryResponseModel.ref1
        };

        var inquiryRequest = new
        {
            Token = TokenManager.GetOAuthToken().Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            BillerCode = hanaBillercode,
            Detail = "{ 'LoanAccountNumber' : '" + detail.LoanAccountNumber + "'}"
        };


        string json = JsonConvert.SerializeObject(inquiryRequest);

        Utils.WriteLog_Biller("Eba inquriy request for Hana Microfinance" + " : " + json);

        string responseJson = Utils.PostEba(json, BISConstants.EBAInquiryUrl);

        Utils.WriteLog_Biller("Eba inquriy response for Hana Microfinance" + " : " + responseJson);

        var inquiryResponse = JsonConvert.DeserializeObject<EbaInquiryRes>(responseJson);

        Utils.WriteLog_Biller("After Deserialize, Eba inquriy response for Hana Microfinance" + " : " + inquiryResponse);

        if (!string.IsNullOrEmpty(responseJson) & inquiryResponse.ErrorCode == "00")
        {
            string responseCode = inquiryResponse.ErrorCode;
            string responseDescription = inquiryResponse.ErrorMessage;
            string detailJson = inquiryResponse.Detail.Replace("\\", "");
            var detailResponse = JsonConvert.DeserializeObject<HanaMicrofinanceDetailResponse>(detailJson);

            inquiryResponseModel.ResCode = inquiryResponse.ErrorCode;
            inquiryResponseModel.ResDesc = inquiryResponse.ErrorMessage;
            inquiryResponseModel.ref1 = detail.LoanAccountNumber;
            inquiryResponseModel.ref2 = detailResponse.CustomerName;
            inquiryResponseModel.ref3 = detailResponse.OfficeName;
            inquiryResponseModel.ref4 = detailResponse.ChannelReferenceId + "|" + detailResponse.BillerReferenceId + "|" + hanaPaymentType;
            inquiryResponseModel.ref1Name = hanaPaymentId;

            inquiryResponseModel.amount = detailResponse.Amount;
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
        Utils.WriteLog_Biller("This is Hana Microfinance from EBA Confirm");
        string ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();
        var token = TokenManager.GetOAuthToken();

        //ChannelReferenceId, BillerReferenceId
        string[] confirmReqObj = new string[3];
        confirmReqObj = confirmResponseModel.ref4.Split('|');

        string hanaPaymentType = string.Empty;
        if (!string.IsNullOrEmpty(confirmReqObj[2]))
            hanaPaymentType = confirmReqObj[2];

        Utils.WriteLog_Biller("Hana Microfinance Ref4 for Confirm Request:" + confirmResponseModel.ref4);

        string hanaBillercode = string.Empty;
        string hanaPaymentId = string.Empty;

        if (confirmReqObj[2] == "Loan Payment")
        {
            hanaBillercode = ConfigurationManager.AppSettings["HanaMicrofinanceLoanBillerCode"].ToString();
            hanaPaymentId = ConfigurationManager.AppSettings["HanaMicrofinanceLoanID"].ToString();
        }

        if (confirmReqObj[2] == "Saving Payment")
        {
            hanaBillercode = ConfigurationManager.AppSettings["HanaMicrofinanceSavingBillerCode"].ToString();
            hanaPaymentId = ConfigurationManager.AppSettings["HanaMicrofinanceSavingID"].ToString();
        }

        Utils.WriteLog_Biller("Hana Microfinance Confirm hanaPayment Id:" + hanaPaymentId);

        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = confirmResponseModel.txnID.ToString(),
            BillerCode = hanaBillercode,
            TransactionAmount = amount,
            Detail = "{'LoanAccountNumber':'" + confirmResponseModel.ref1
                        + "', 'MobileNumber':'" + confirmResponseModel.ref5
                        + "', 'Amount':'" + amount
                        + "', 'OfficeName':'" + confirmResponseModel.ref3
                        + "', 'ChannelReferenceId':'" + confirmReqObj[0]
                        + "', 'BillerReferenceId':'" + confirmReqObj[1]
                        + "', 'CustomerName':'" + confirmResponseModel.ref2 + "'}"
        };

        string jsonReq = JsonConvert.SerializeObject(confirmReq);
        Utils.WriteLog_Biller("EBA Hana Microfinance Confirm Jason Request:" + jsonReq);

        SSLPost post = new SSLPost();
        string jsonres = Utils.PaymentRequest(jsonReq, BISConstants.EBAConfirmUrl);
        Utils.WriteLog_Biller("EBA Hana Microfinance Confirm Jason Response:" + jsonres);

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
            //hanaPaymentId is to know for Saving Id or Loan Id
            if (!(new ServiceClient()).ConfirmUpdate(txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, hanaPaymentId,
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

                    smsMsg = smsH.getMessageBiller(agentName, MapTaxID, confirmResponseModel.billername, hanaPaymentId, string.Empty, confirmResponseModel.ref3Name, "Ref", confirmResponseModel.ref1,
                          string.Empty, confirmResponseModel.ref3, txnID.ToString(), double.Parse(amount).ToString("#,##0.00"), serviceFee, double.Parse(totalAmount).ToString("#,##0.00"), branchCode);

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
            //Ref1Name is dynamic => Saving Id or Loan Id
            confirmResponseModel.ref1Name = hanaPaymentId;
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