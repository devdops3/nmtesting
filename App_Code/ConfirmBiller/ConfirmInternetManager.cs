using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for ConfirmInternetManager
/// </summary>
public class ConfirmInternetManager
{
    #region Variable
    private static A2AAPIWCF.ServiceClient _agentWCF = new A2AAPIWCF.ServiceClient();

    #endregion

	public ConfirmInternetManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string Confirm2InternetManager(ConfrimReq request)
    {
        Service service = new Service();
        string resdecs = string.Empty;
        string rescode = string.Empty;
        var errMsg = string.Empty;
        int batchID = 0;
        string smsMsg = string.Empty;

        Utils.WriteLog_Biller("This is " + request.TaxId + " from EBA Confirm");
        var ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();

        // var billerCode = service.GetBillerCode(request.TaxId);
        //var ebaResponse = service.ConfirmGiftCard(request.TxnId.ToString(), Convert.ToDouble(request.Amount), billerCode).Result;
        if (TokenManager.IsTokenNullOrExpire(TokenManager.Token))
        {
            TokenManager.Token = TokenManager.GetOAuthToken();
        }

        DynamicInfo dynamicInfo = new DynamicInfo();
        dynamicInfo.TaxId = request.TaxId;
        dynamicInfo.Ref1 = request.Ref1;
        dynamicInfo.Ref2 = request.Ref2;
        dynamicInfo.Ref3 = request.Ref3;
        dynamicInfo.amount = request.Amount;

        string billerCodeName = request.TaxId + "BillerCode";
        EbaConfirmReq confirmReq = new EbaConfirmReq()
        {
            Token = TokenManager.Token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = request.TxnId.ToString(),
            BillerCode = ConfigurationManager.AppSettings[billerCodeName].ToString(),
            TransactionAmount = request.Amount,
            Detail = JsonConvert.SerializeObject(Utils.Get_DynamicJsonReq(dynamicInfo, "ReqDetail"))
        };
        EbaConfirmRes confirmRes = new EbaConfirmRes();

        var json_serializer1 = new JavaScriptSerializer();
        string jsonReq = json_serializer1.Serialize(confirmReq);
        Utils.WriteLog_Biller("EBA  " + request.TaxId + " JasonReq:" + jsonReq);

        SSLPost post = new SSLPost();
        string jsonres = Utils.PaymentRequest(jsonReq, ebaUrl);
        Utils.WriteLog_Biller("EBA " + request.TaxId + " JasonRes:" + jsonres);

        if (string.IsNullOrEmpty(jsonres))
        {
            resdecs = "No Response From EBA";
            rescode = "06";

            return Service.GetErrorResponseWithAddBalance(rescode, resdecs, request.TxnId, resdecs, request.AgentId, request.AgentAmount, request.IsAgreement, request.TaxId);
        }

        var json_serializerpinres = new JavaScriptSerializer();
        confirmRes = json_serializerpinres.Deserialize<EbaConfirmRes>(jsonres);

        if (confirmRes.ErrorCode == "00")
        {
            #region <-- Update Transaction -->
           

            if (!_agentWCF.ConfirmUpdate(request.TxnId, request.Ref1, request.Ref2, request.Ref3, request.Ref4, request.Ref5, "", "PA", "Paid Successfully", request.AgentId, request.Email, request.AgentAmount, request.AgentFee, request.IsAgreement, request.SMSStatus, request.AvailableBalance, out errMsg, out batchID))
            {
                Utils.WriteLog_Biller("Error in ConfirmUpdate : " + errMsg);
                resdecs = "Error in update database";
                rescode = "06";
                if (!_agentWCF.updateError(request.TxnId, "ER", resdecs, out errMsg))
                {
                    Utils.WriteLog_Biller("Error in updateError : " + errMsg);
                }
                return Utils.getErrorRes(rescode, "Transaction fail", request.TaxId);
            }
            else
            {
                Utils.WriteLog_Biller("After update = AgentAmount : " + request.AgentAmount + " | Balance : " + request.AvailableBalance.ToString() + "| smsStatus:" + request.SMSStatus);
            }

            #endregion

            #region <-- Send SMS -->

            if (request.AppType == "CS" || request.AppType == "MS")
            {
                if (string.IsNullOrEmpty(request.TopupType) || request.TopupType == "S")
                {
                    SMSHelper smsH = new SMSHelper();
                    MessagingService.MessagingServiceClient smsWcf = new MessagingService.MessagingServiceClient();

                    #region dynamic sms customized
                    string ref1Name = "", ref2Name = "", ref3Name = "";
                    var smsResult=Utils.Get_DynamicJsonReq(dynamicInfo, "SmsCustomized");

                    if (smsResult != null)
                    {
                        ref1Name = (string)smsResult["Ref1Name"];
                        ref2Name = (string)smsResult["Ref2Name"];
                        ref3Name = (string)smsResult["Ref3Name"];
                    }
                    else
                    {
                        ref1Name = request.Ref1Name;
                        ref2Name = request.Ref2Name;
                        ref3Name = request.Ref3Name;
                    }
                    
                    #endregion


                     smsMsg = smsH.getMessageBiller(request.AgentName, request.TaxId, request.BillerName, ref1Name != string.Empty ? request.Ref1Name : string.Empty, ref2Name != string.Empty ? request.Ref2Name : string.Empty, ref3Name != string.Empty ? request.Ref3Name : string.Empty, "Ref", ref1Name != string.Empty ? request.Ref1 : string.Empty, ref2Name != string.Empty ? request.Ref2 : string.Empty, ref3Name != string.Empty ? request.Ref3 : string.Empty, request.TxnId.ToString(), double.Parse(request.Amount).ToString("#,##0.00"), request.ServiceFee, double.Parse(request.TotalAmount).ToString("#,##0.00"), request.BranchCode);

                    try
                    {
                        Utils.WriteLog_Biller("Mobile No :" + request.Ref5 + "| Message :" + smsMsg + "| Sender Name :" + request.SenderName + "|txn ID :" + request.TxnId);
                        smsWcf.SendSms(request.TxnId.ToString(), smsMsg, request.Ref5, request.SenderName);
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

            rescode = "00";
            resdecs = "Success";
            ConfirmResponseModel confirmres = new ConfirmResponseModel();
            confirmres.taxID = request.TaxId;
            confirmres.email = request.Email;
            confirmres.password = request.Password;
            confirmres.messageid = request.MessageId;
            confirmres.billername = request.BillerName;
            confirmres.billerlogo = request.BillerLogo;
            confirmres.rescode = confirmRes.ErrorCode;
            confirmres.resdesc = confirmRes.ErrorMessage;
            confirmres.ref1 = request.Ref1;
            confirmres.ref2 = request.Ref2;
            confirmres.ref3 = request.Ref3;
            confirmres.ref4 = request.Ref4;
            confirmres.ref5 = request.Ref5;
            //confirmres.ref6 = Utils.ReplaceAmpersandString(imgnrc);
            confirmres.ref1Name = request.Ref1Name;
            confirmres.ref2Name = request.Ref2Name;
            confirmres.ref3Name = request.Ref3Name;
            confirmres.ref4Name = request.Ref4Name;
            confirmres.ref5Name = request.Ref5Name;
            confirmres.availablebalance = request.AvailableBalance.ToString();
            confirmres.txnID = request.TxnId.ToString();
            confirmres.TodayTxnAmount = request.TodayTxnAmount;
            confirmres.TodayTxnCount = request.TodayTxnCount;
            confirmres.smsMsg = smsMsg;

            return Utils.getConfirmRes(confirmres);

            #endregion
        }

        else
        {
            resdecs = Utils.EsbResponseDescription(confirmRes.ErrorCode);
            return Service.GetErrorResponseWithAddBalance(confirmRes.ErrorCode.ToString(),
                confirmRes.ErrorMessage, request.TxnId, resdecs, request.AgentId, request.AgentAmount,
                request.IsAgreement, request.TaxId);
        }

    }


 

}