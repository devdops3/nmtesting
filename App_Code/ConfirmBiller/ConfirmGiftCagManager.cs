using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for ConfirmBillerManager
/// </summary>
public class ConfirmGiftCagManager
{
    #region Variable
    private static A2AAPIWCF.ServiceClient _agentWCF = new A2AAPIWCF.ServiceClient();

    #endregion


    public ConfirmGiftCagManager()
	{
	}


    //Gift Category Ref format => Ref 1 = CartType, Ref2 = CartPrice, Ref3 = SerialNo. , Ref4= Pin-Expiry, Ref5= MobileNo. but When mobile assign to Ref5 as product code and Ref3 as mobileNo....So, when response to mobile, need to change as portal set up
    public string Confirm2GiftCagBiller(ConfrimGiftCardRequest request)
    {
        Service service = new Service();
        string resdecs = string.Empty;
        string rescode = string.Empty;
        var errMsg = string.Empty;
        int batchID = 0;
        string smsMsg = string.Empty;

        Utils.WriteLog_Biller("This is " + request.TaxId + " from EBA Confirm");
        var ebaUrl = ConfigurationManager.AppSettings["GiftCardConfirmUrl"].ToString();

       // var billerCode = service.GetBillerCode(request.TaxId);
        //var ebaResponse = service.ConfirmGiftCard(request.TxnId.ToString(), Convert.ToDouble(request.Amount), billerCode).Result;
        if (TokenManager.IsTokenNullOrExpire(TokenManager.Token))
        {
            TokenManager.Token = TokenManager.GetOAuthToken();
        }
        string billerCodeName = request.TaxId + "BillerCode";
        EbaConfirmReq confirmReq = new EbaConfirmReq()
        {
            Token = TokenManager.Token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = request.TxnId.ToString(),
            BillerCode = ConfigurationManager.AppSettings[billerCodeName].ToString(),
            TransactionAmount = request.Amount,
            Detail = "{'ProductCode':'" + request.Ref5 + "', 'Amount':'" + request.Ref2 + "'}"
        };
        EbaConfirmRes confirmRes = new EbaConfirmRes();

        var json_serializer1 = new JavaScriptSerializer();
        string jsonReq = json_serializer1.Serialize(confirmReq);
        Utils.WriteLog_Biller("EBA  " + request.TaxId + " JasonReq:" + jsonReq);

        SSLPost post = new SSLPost();
        string jsonres = Utils.PaymentRequest(jsonReq, ebaUrl);
        Utils.WriteLog_Biller("EBA " +request.TaxId+ " JasonRes:" + jsonres);

        if (string.IsNullOrEmpty(jsonres))
        {
            resdecs = "No Response From EBA";
            rescode = "06";

            return  Service.GetErrorResponseWithAddBalance(rescode, resdecs, request.TxnId, resdecs, request.AgentId, request.AgentAmount, request.IsAgreement,request.TaxId);
        }

        var json_serializerpinres = new JavaScriptSerializer();
        confirmRes = json_serializerpinres.Deserialize<EbaConfirmRes>(jsonres);

        if (confirmRes.ErrorCode == "00")
        {
            #region <-- Update Transaction -->
            var detail = JsonConvert.DeserializeObject<Detail>(confirmRes.Detail);

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EncryptedPinList"].ToString().Split(',').Where(x => x == request.TaxId).FirstOrDefault()))
            {
                string aesKey = string.Empty;
                aesKey = ConfigurationManager.AppSettings["EsbaAesKey"].ToString();
                detail.ClearPin = Utils.AESDecryptText(detail.ClearPin, aesKey);
            }


            request.Ref5 = request.Ref3;   //Assign Mobile  No to Ref5
            request.Ref3 = detail.SerialNumber;
            //request.Ref2 = detail.PinId;
            request.Ref4 = string.Format("{0} {1}", detail.ClearPin, string.IsNullOrEmpty(detail.ExpiryDate) ? "-" : detail.ExpiryDate);
            

            if (!_agentWCF.ConfirmUpdate(request.TxnId, request.Ref1, request.Ref2, request.Ref3, request.Ref4, request.Ref5, "", "PA", "Paid Successfully", request.AgentId, request.Email, request.AgentAmount, request.AgentFee, request.IsAgreement, "N", request.AvailableBalance, out errMsg, out batchID))
                
            {
                Utils.WriteLog_Biller("Error in ConfirmUpdate : " + errMsg);
                resdecs = "Error in update database";
                rescode = "06";
                if (!_agentWCF.updateError(request.TxnId, "ER", resdecs, out errMsg))
                {
                    Utils.WriteLog_Biller("Error in updateError : " + errMsg);
                }
                return Utils.getErrorRes(rescode, "Transaction fail",request.TaxId);
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

                    smsMsg = smsH.getMessageBiller(request.AgentName, request.TaxId, request.BillerName, "Code", "Expiry", "", "Ref", detail.ClearPin, string.IsNullOrEmpty(detail.ExpiryDate) ? "-" : detail.ExpiryDate, "", request.TxnId.ToString(), double.Parse(request.Amount).ToString("#,##0.00"), request.ServiceFee, double.Parse(request.TotalAmount).ToString("#,##0.00"), request.BranchCode);

                    try
                    {
                        Utils.WriteLog_Biller("Mobile No :" + request.Ref5 + "| Message :" + smsMsg + "| Sender Name :" + request.SenderName + "|txn ID :" + request.TxnId);
                        smsWcf.SendSms(request.TxnId.ToString(), smsMsg, request.Ref5, request.SenderName);
                        Utils.WriteLog_Biller("sendSMSWithTxnID ends.");
                    }
                    catch
                    {

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
            confirmres.rescode = rescode;
            confirmres.resdesc = resdecs;
            confirmres.ref1 = request.Ref1;
            confirmres.ref2 = request.Ref2;
            confirmres.ref3 = request.Ref3;
            confirmres.ref4 = request.Ref4;
            confirmres.ref5 = request.Ref5;
            confirmres.ref1Name =request.Ref1Name;
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
            return Service.GetErrorResponseWithAddBalance(confirmRes.ErrorCode.ToString(),
                confirmRes.ErrorMessage, request.TxnId, resdecs, request.AgentId, request.AgentAmount,
                request.IsAgreement,request.TaxId);
        }

    }
}