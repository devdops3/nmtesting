using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for ConfirmManager
/// </summary>
public class ConfirmB2BManager
{
    #region <-- Variable -->
    private static A2AAPIWCF.ServiceClient _agentWCF = new A2AAPIWCF.ServiceClient();
    private static ForwarderService.ServiceClient _forwarderService = new ForwarderService.ServiceClient();
    #endregion

    public ConfirmB2BManager()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string Confirm2Biller_B2BOrder(ConfirmBillerRequest request)
    {
        string response=string.Empty;
        if (request.TaxId == ConfigurationManager.AppSettings["MarketplaceTempTaxId"])
        {
            response = B2BOrderV1(request);
        }
        else if (request.TaxId == ConfigurationManager.AppSettings["MarketplaceTaxId"])
        {
            response = B2BOrderV2(request);
        }

        return response;
    }

    public string B2BOrderCancelV2(string txnId, string taxId)
    {
        Utils.WriteLog_Biller("MapTaxID : " + taxId);
        var ds = new DataSet();
        var errMsg = string.Empty;
        var rescode = string.Empty;
        var resdecs = string.Empty;
        
        var existTxn = _agentWCF.GetTxn(Convert.ToInt64(txnId), out ds, out errMsg);
        if (!existTxn) return Utils.getErrorRes("91", "Transaction not found.", txnId);

        var data = ds.Tables[0].Rows;
        if (data.Count > 0)
        {
            var txnStatus = data[0]["TRANSACTIONSTATUS"].ToString();
            if (txnStatus == "PA" || txnStatus == "ER") return Utils.getErrorRes("91", "Duplicate transaction.", taxId);

            TxnInfo txnInfo = new TxnInfo();

            txnInfo.Ref1 = data[0]["REFID1"].ToString();
            txnInfo.Ref2 = data[0]["REFID2"].ToString();
            txnInfo.Ref3 = data[0]["REFID3"].ToString();
            txnInfo.Ref4 = data[0]["REFID4"].ToString();
            txnInfo.Ref5 = data[0]["REFID5"].ToString();

            txnInfo.TxnId = data[0]["TRANSACTIONID"].ToString();
            txnInfo.TxnAmount = data[0]["TRANSACTIONAMOUNT"].ToString();
            txnInfo.AgentAmt = data[0]["AGENTAMOUNT"].ToString();
            txnInfo.ServiceFee = data[0]["SERVICEFEE"].ToString();
            txnInfo.AgentFee = data[0]["AGENTFEE"].ToString();
            txnInfo.AgentUserId = data[0]["AgentUserId"].ToString();


            var sgs = new StringBuilder();
            sgs.Append(txnInfo.Ref3);
            sgs.Append(txnInfo.AgentUserId);
            sgs.Append(txnInfo.Ref1);
            sgs.Append(txnInfo.TxnId);
            sgs.Append(txnInfo.TxnAmount);
            sgs.Append("06");

            var sKey = ConfigurationManager.AppSettings["B2BOrderV2SecreteKey"];
            var hashValue = EncryptionManager.GetHmac(sgs.ToString(), sKey);

            txnInfo.HashValue = hashValue;
            txnInfo.ResponseCode = "06";
            txnInfo.ResponseDesc = "Payment Canceled";

            Utils.WriteLog_Biller("Update Transaction Cancel");
         
            #region <-- Update Transaction -->
            if (!_agentWCF.updateError(Convert.ToInt64(txnInfo.TxnId), "CA", "Canceled", out errMsg))
            {
                Utils.WriteLog_Biller("Error in updateError : " + errMsg);

                 return Utils.getErrorRes("99", "System Error.", taxId);
            }
            #endregion

            #region Call Confirm Api Req
            else
            {
                //get dynamic Json Req
                var notify_Req = ConfigurationManager.AppSettings["B2BOrderV2NotifyPaymentReq"].ToString().Split(',');
                JObject _ConfirmJsonReq = Get_B2BDynamicJsonReq(txnInfo, notify_Req);

                Utils.WriteLog_Biller("Notify Request to B2B :" + JsonConvert.SerializeObject(_ConfirmJsonReq));
                Utils.WriteLog_Biller("Url " + txnInfo.Ref2);

                var b2bOrderRespoonse = Utils.PostMarketplace(JsonConvert.SerializeObject(_ConfirmJsonReq), txnInfo.Ref2);
                Utils.WriteLog_Biller("Notify Respond from B2B:" + b2bOrderRespoonse);

              
                #region <-- Response Back To Client -->

                B2BCancelResponseModel b2bCancelRes = new B2BCancelResponseModel();

                b2bCancelRes.ResCode = "00";
                b2bCancelRes.ResDesc = txnInfo.ResponseDesc;
                b2bCancelRes.taxID = taxId;

                return Utils.getB2BCancelRes(b2bCancelRes);


                #endregion

            #endregion
               
            }
        }
        else
        {
            return Utils.getErrorRes("91", "Transaction not found.", taxId);
        }
    }

  
    #region Private
    private string B2BOrderV1(ConfirmBillerRequest request)
    {
        Utils.WriteLog_Biller("MapTaxID : " + request.TaxId);
        var ds = new DataSet();
        var errMsg = string.Empty;
        var availableBalance = 0.0;
        var ledgerBalance = 0.0;
        var rescode = string.Empty;
        var resdecs = string.Empty;
        int batchID = 0;

        var existTxn = _agentWCF.GetTxn(Convert.ToInt64(request.Ref4), out ds, out errMsg);
        if (!existTxn) return Utils.getErrorRes("91", "Transaction not found.", request.TaxId);

        var data = ds.Tables[0].Rows;
        if (data.Count > 0)
        {
            var txnStatus = data[0]["TRANSACTIONSTATUS"].ToString();
            if (txnStatus == "PA" || txnStatus == "ER") return Utils.getErrorRes("91", "Duplicate transaction.", request.TaxId);

            TxnInfo txnInfo = new TxnInfo();

            txnInfo.Ref1 = data[0]["REFID1"].ToString();
            txnInfo.Ref2 = data[0]["REFID2"].ToString();
            txnInfo.Ref3 = data[0]["REFID3"].ToString();
            txnInfo.Ref4 = data[0]["REFID4"].ToString();
            txnInfo.Ref5 = data[0]["REFID5"].ToString();

            txnInfo.TxnId = data[0]["TRANSACTIONID"].ToString();
            txnInfo.TxnAmount = data[0]["TRANSACTIONAMOUNT"].ToString();
            txnInfo.AgentAmt = data[0]["AGENTAMOUNT"].ToString();
            txnInfo.ServiceFee = data[0]["SERVICEFEE"].ToString();
            txnInfo.AgentFee = data[0]["AGENTFEE"].ToString();

            if (request.IsAgreement == "N")
            {
                var updateTxn = _agentWCF.minusAgentAmt(Convert.ToInt64(txnInfo.TxnId), request.AgentID, Convert.ToDouble(txnInfo.AgentAmt), request.IsAgreement, out errMsg, out availableBalance, out ledgerBalance);
                if (!updateTxn) return Utils.getErrorRes("91", errMsg, request.TaxId);
            }

            var sgs = new StringBuilder();
            sgs.Append(txnInfo.Ref1);
            sgs.Append(txnInfo.TxnId);
            sgs.Append(txnInfo.TxnAmount);
            sgs.Append("00");

            var sKey = ConfigurationManager.AppSettings["B2BOrderSecreteKey"];
            var hashValue = EncryptionManager.GetHmac(sgs.ToString(), sKey);

            txnInfo.HashValue = hashValue;
            txnInfo.ResponseCode = "00";
            txnInfo.ResponseDesc = "Success";


            //get dynamic Json Req
            var notify_Req = ConfigurationManager.AppSettings["B2BOrderNotifyPaymentReq"].ToString().Split(',');
            JObject _ConfirmJsonReq = Get_B2BDynamicJsonReq(txnInfo, notify_Req);

            Utils.WriteLog_Biller("Notify Request to B2B :" + JsonConvert.SerializeObject(_ConfirmJsonReq));
            Utils.WriteLog_Biller("Url " + txnInfo.Ref2);

            var b2bOrderRespoonse = Utils.PostEba(JsonConvert.SerializeObject(_ConfirmJsonReq), txnInfo.Ref2);
            Utils.WriteLog_Biller("Notify Respond from B2B:" + b2bOrderRespoonse);

            if (string.IsNullOrEmpty(b2bOrderRespoonse))
            {
                Utils.WriteLog_Biller("Notify Request 2nd Time to B2B :" + JsonConvert.SerializeObject(_ConfirmJsonReq));
                Utils.WriteLog_Biller("Url " + txnInfo.Ref2);

                b2bOrderRespoonse = Utils.PostEba(JsonConvert.SerializeObject(_ConfirmJsonReq), txnInfo.Ref2);
                Utils.WriteLog_Biller("Notify Respond 2nd Time from B2B:" + b2bOrderRespoonse);

                if (string.IsNullOrEmpty(b2bOrderRespoonse))
                {
                    resdecs = "No Response From Marketplace side.";
                    rescode = "06";

                    return Service.GetErrorResponseWithAddBalance(rescode, resdecs, Convert.ToInt64(txnInfo.TxnId), resdecs, request.AgentID, Convert.ToDouble(txnInfo.AgentAmt), request.IsAgreement, request.TaxId);
                }
            }

;

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            dynamic mResult = serializer.Deserialize(b2bOrderRespoonse, typeof(object));

            var responseCode = (string)mResult["ResponseCode"];
            var responseDesc = (string)mResult["ResponseDesc"];
            var InvoiceNo = (string)mResult["InvoiceNo"];
           if (responseCode == "00")
            {

                string respond_HashValue = string.Empty;

                //get dynmaic json Response 
                StringBuilder notifySg = Get_HashValue(mResult, out respond_HashValue);
                Utils.WriteLog_Biller("Signature String HashValue :" + notifySg.ToString());

                var notifyHashValue = EncryptionManager.GetHmac(notifySg.ToString(), sKey);

                Utils.WriteLog_Biller("Encrypt HashValue :" + notifyHashValue);

                if (notifyHashValue != respond_HashValue)       //for order = OrderDetailUrl
                {
                    resdecs = "Hash Value mismatch";
                    rescode = "06";
                    return Service.GetErrorResponseWithAddBalance(rescode, resdecs, Convert.ToInt64(txnInfo.TxnId), resdecs, request.AgentID, Convert.ToDouble(txnInfo.AgentAmt), request.IsAgreement, request.TaxId);
                }

                #region <-- Update Transaction -->

                txnInfo.Ref1 = InvoiceNo;
                Utils.WriteLog_Biller("Update Transaction");
                if (!_agentWCF.ConfirmUpdate(Convert.ToInt64(txnInfo.TxnId), txnInfo.Ref1, txnInfo.Ref2, txnInfo.Ref3, string.Empty, request.PhoneNumber, "", "PA", "Success", request.AgentID, request.Email, Convert.ToDouble(txnInfo.AgentAmt), Convert.ToDouble(txnInfo.AgentFee), request.IsAgreement, "N", availableBalance, out errMsg, out batchID))
                {
                    Utils.WriteLog_Biller("Error in ConfirmUpdate : " + errMsg);
                }

                Utils.WriteLog_Biller("After update = AgentAmount : " + request.AgentAmt + " | Balance : " + availableBalance.ToString(System.Globalization.CultureInfo.InvariantCulture) + "| smsStatus:" + request.SMSStatus);
                var sms = string.Empty;

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
                confirmres.ref1 = Utils.ReplaceAmpersandString(txnInfo.Ref1);
                confirmres.ref2 = string.Empty;
                confirmres.ref3 = string.Empty;
                confirmres.ref4 = string.Empty;
                confirmres.ref5 = request.PhoneNumber;
                confirmres.ref1Name = request.Ref1Name;
                confirmres.ref2Name = request.Ref2Name;
                confirmres.ref3Name = request.Ref3Name;
                confirmres.ref4Name = request.Ref4Name;
                confirmres.ref5Name = request.Ref5Name;
                confirmres.availablebalance = availableBalance.ToString();
                confirmres.txnID = txnInfo.TxnId;
                confirmres.TodayTxnAmount = request.TodayTxnAmount;
                confirmres.TodayTxnCount = request.TodayTxnCount;
                confirmres.smsMsg = sms;

                return Utils.getConfirmRes(confirmres);


                #endregion
            }
            else
            {
                var resCode = "91";
                return Service.GetErrorResponseWithAddBalance(resCode, responseDesc, Convert.ToInt64(txnInfo.TxnId), responseDesc, request.AgentID, Convert.ToDouble(txnInfo.AgentAmt), request.IsAgreement, request.TaxId);
            }
        }
        else
        {
            return Utils.getErrorRes("91", "Transaction not found.", request.TaxId);
        }
    }

    private string B2BOrderV2(ConfirmBillerRequest request)
    {
        Utils.WriteLog_Biller("MapTaxID : " + request.TaxId);
        var ds = new DataSet();
        var errMsg = string.Empty;
        var availableBalance = 0.0;
        var ledgerBalance = 0.0;
        var rescode = string.Empty;
        var resdecs = string.Empty;
        int batchID = 0;

        var existTxn = _agentWCF.GetTxn(Convert.ToInt64(request.Ref4), out ds, out errMsg);
        if (!existTxn) return Utils.getErrorRes("91", "Transaction not found.", request.TaxId);

        var data = ds.Tables[0].Rows;
        if (data.Count > 0)
        {
            var txnStatus = data[0]["TRANSACTIONSTATUS"].ToString();
            if (txnStatus == "PA" || txnStatus == "ER") return Utils.getErrorRes("91", "Duplicate transaction.", request.TaxId);

            TxnInfo txnInfo = new TxnInfo();

            txnInfo.Ref1 = data[0]["REFID1"].ToString();
            txnInfo.Ref2 = data[0]["REFID2"].ToString();
            txnInfo.Ref3 = data[0]["REFID3"].ToString();
            txnInfo.Ref4 = data[0]["REFID4"].ToString();
            txnInfo.Ref5 = data[0]["REFID5"].ToString();

            txnInfo.TxnId = data[0]["TRANSACTIONID"].ToString();
            txnInfo.TxnAmount = data[0]["TRANSACTIONAMOUNT"].ToString();
            txnInfo.AgentAmt = data[0]["AGENTAMOUNT"].ToString();
            txnInfo.ServiceFee = data[0]["SERVICEFEE"].ToString();
            txnInfo.AgentFee = data[0]["AGENTFEE"].ToString();
            txnInfo.AgentUserId = data[0]["AgentUserId"].ToString();

            if (request.IsAgreement == "N")
            {
                var updateTxn = _agentWCF.minusAgentAmt(Convert.ToInt64(txnInfo.TxnId), request.AgentID, Convert.ToDouble(txnInfo.AgentAmt), request.IsAgreement, out errMsg, out availableBalance, out ledgerBalance);
                if (!updateTxn) return Utils.getErrorRes("91", errMsg, request.TaxId);
            }

            var sgs = new StringBuilder();
            sgs.Append(txnInfo.Ref3);
            sgs.Append(txnInfo.AgentUserId);
            sgs.Append(txnInfo.Ref1);
            sgs.Append(txnInfo.TxnId);
            sgs.Append(txnInfo.TxnAmount);
            sgs.Append("00");

            var sKey = ConfigurationManager.AppSettings["B2BOrderV2SecreteKey"];
            var hashValue = EncryptionManager.GetHmac(sgs.ToString(), sKey);

            txnInfo.HashValue = hashValue;
            txnInfo.ResponseCode = "00";
            txnInfo.ResponseDesc = "Success";


            //get dynamic Json Req
            var notify_Req = ConfigurationManager.AppSettings["B2BOrderV2NotifyPaymentReq"].ToString().Split(',');
            JObject _ConfirmJsonReq = Get_B2BDynamicJsonReq(txnInfo, notify_Req);

           Utils.WriteLog_Biller("Notify Request to B2B :" + JsonConvert.SerializeObject(_ConfirmJsonReq));
            Utils.WriteLog_Biller("Url " + txnInfo.Ref4);

            var b2bOrderRespoonse = Utils.PostMarketplace(JsonConvert.SerializeObject(_ConfirmJsonReq), txnInfo.Ref4);
            Utils.WriteLog_Biller("Notify Respond from B2B:" + b2bOrderRespoonse);

            if (string.IsNullOrEmpty(b2bOrderRespoonse))
            {
                Utils.WriteLog_Biller("Notify Request 2nd Time to B2B :" + JsonConvert.SerializeObject(_ConfirmJsonReq));
                Utils.WriteLog_Biller("Url " + txnInfo.Ref2);

                b2bOrderRespoonse = Utils.PostMarketplace(JsonConvert.SerializeObject(_ConfirmJsonReq), txnInfo.Ref4);
                Utils.WriteLog_Biller("Notify Respond 2nd Time from B2B:" + b2bOrderRespoonse);

                if (string.IsNullOrEmpty(b2bOrderRespoonse))
                {
                    resdecs = "No Response From Marketplace side.";
                    rescode = "06";

                    return Service.GetErrorResponseWithAddBalance(rescode, resdecs, Convert.ToInt64(txnInfo.TxnId), resdecs, request.AgentID, Convert.ToDouble(txnInfo.AgentAmt), request.IsAgreement, request.TaxId);
                }
            }

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            dynamic mResult = serializer.Deserialize(b2bOrderRespoonse, typeof(object));

            var responseCode = (string)mResult["ResponseCode"];
            var responseDesc = (string)mResult["ResponseDesc"];
            var InvoiceNo = (string)mResult["MarketplaceRef"];
           if (responseCode == "00")
            {

                string respond_HashValue = string.Empty;

                //get dynmaic json Response 
                StringBuilder notifySg = Get_HashValueV2(mResult, out respond_HashValue);
                Utils.WriteLog_Biller("Signature String HashValue :" + notifySg.ToString());

                var notifyHashValue = EncryptionManager.GetHmac(notifySg.ToString(), sKey);

                Utils.WriteLog_Biller("Encrypt HashValue :" + notifyHashValue);

                if (notifyHashValue != respond_HashValue)       //for order = OrderDetailUrl
                {
                    resdecs = "Hash Value mismatch";
                    rescode = "06";
                    return Service.GetErrorResponseWithAddBalance(rescode, resdecs, Convert.ToInt64(txnInfo.TxnId), resdecs, request.AgentID, Convert.ToDouble(txnInfo.AgentAmt), request.IsAgreement, request.TaxId);
                }

                #region <-- Update Transaction -->

                txnInfo.Ref1 = InvoiceNo;
                Utils.WriteLog_Biller("Update Transaction");
                if (!_agentWCF.ConfirmUpdate(Convert.ToInt64(txnInfo.TxnId), txnInfo.Ref1, txnInfo.Ref2, txnInfo.Ref3, string.Empty, request.PhoneNumber, "", "PA", "Success", request.AgentID, request.Email, Convert.ToDouble(txnInfo.AgentAmt), Convert.ToDouble(txnInfo.AgentFee), request.IsAgreement, "N", availableBalance, out errMsg, out batchID))
                {
                    Utils.WriteLog_Biller("Error in ConfirmUpdate : " + errMsg);
                }
                else
                {
                    var updateB2BResponse = _forwarderService.UpdateB2BInvoice("PAID", "", txnInfo.Ref1);
                    if (updateB2BResponse == null)
                    {
                        Utils.WriteLog_Biller("Error in UpdateB2B : " + errMsg);
                    }
                }

                Utils.WriteLog_Biller("After update = AgentAmount : " + request.AgentAmt + " | Balance : " + availableBalance.ToString(System.Globalization.CultureInfo.InvariantCulture) + "| smsStatus:" + request.SMSStatus);
                var sms = string.Empty;

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
                confirmres.ref1 = Utils.ReplaceAmpersandString(txnInfo.Ref1);
                confirmres.ref2 = string.Empty;
                confirmres.ref3 = string.Empty;
                confirmres.ref4 = string.Empty;
                confirmres.ref5 = request.PhoneNumber;
                confirmres.ref1Name = request.Ref1Name;
                confirmres.ref2Name = request.Ref2Name;
                confirmres.ref3Name = request.Ref3Name;
                confirmres.ref4Name = request.Ref4Name;
                confirmres.ref5Name = request.Ref5Name;
                confirmres.availablebalance = availableBalance.ToString();
                confirmres.txnID = txnInfo.TxnId;
                confirmres.TodayTxnAmount = request.TodayTxnAmount;
                confirmres.TodayTxnCount = request.TodayTxnCount;
                confirmres.smsMsg = sms;

                return Utils.getConfirmRes(confirmres);


                #endregion
            }
            else
            {
                var resCode = "91";
                return Service.GetErrorResponseWithAddBalance(resCode, responseDesc, Convert.ToInt64(txnInfo.TxnId), responseDesc, request.AgentID, Convert.ToDouble(txnInfo.AgentAmt), request.IsAgreement, request.TaxId);
            }
        }
        else
        {
            return Utils.getErrorRes("91", "Transaction not found.", request.TaxId);
        }
    }

    private JObject Get_B2BDynamicJsonReq(TxnInfo txnInfo, string[] notify_Req)
    {
        JObject _ConfirmJsonReq = new JObject();
        foreach (string item in notify_Req)
        {
            string[] _data = item.Split(':');
            if (_data.Count() == 2)
            {
                var property = txnInfo.GetType().GetProperties().Where(x => string.Equals(x.Name.ToLower().Trim(), _data[1].ToLower().Trim())).Select(x => x.Name).FirstOrDefault();

                object detailValue = null;
                if (!string.IsNullOrEmpty(property))
                {
                    detailValue = txnInfo.GetType().GetProperty(property).GetValue(txnInfo, null);

                }
                _ConfirmJsonReq.Add(_data[0].Trim(), detailValue == null ? string.Empty : detailValue.ToString());

            }
        }
        return _ConfirmJsonReq;
    }


    private StringBuilder Get_HashValue(dynamic mResult, out string respond_HashValue)
    {
        var notifySg = new StringBuilder();
        notifySg.Append(mResult.PartnerRef);
        notifySg.Append(mResult.TransactionRef);
        notifySg.Append(mResult.InvoiceNo);
        notifySg.Append(mResult.Amount);
        notifySg.Append(mResult.ResponseCode);

        respond_HashValue = mResult.HashValue;
       
        
        return notifySg;
    }


    private StringBuilder Get_HashValueV2(dynamic mResult, out string respond_HashValue)
    {
        var notifySg = new StringBuilder();
        notifySg.Append(mResult.PartnerId);
        notifySg.Append(mResult.UserId);
        notifySg.Append(mResult.MarketplaceRef);
        notifySg.Append(mResult.PartnerRef);
        notifySg.Append(mResult.Amount);
        notifySg.Append(mResult.ResponseCode);

        respond_HashValue = mResult.HashValue;


        return notifySg;
    }

 
    #endregion


}

