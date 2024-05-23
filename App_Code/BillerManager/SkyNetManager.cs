using System;
using System.Collections;
using System.Configuration;
using Newtonsoft.Json;
using EbaReqResModel;
using A2AAPIWCF;
/// <summary>
/// Summary description for SkyNetManager
/// </summary>
public class SkyNetManager
{
    public SkyNetManager()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string GetSkyNetPackages(string requestXML)
    {
        try
        {

            string code = string.Empty, description = string.Empty;
            string version = string.Empty;
            string timeStamp = string.Empty;
            string messageId = string.Empty;
            string ref1 = string.Empty;
            string paymentType = string.Empty;

            Hashtable ht = Utils.getHTableFromXML(requestXML);

            if (!IsValidSkyNetPackagesRequest(ht, out code, out description, out version, out timeStamp, out messageId, out ref1, out paymentType)) return Utils.getErrorRes(code, description);

            var skyNetInquiryUrl = ConfigurationManager.AppSettings["MobileLegendInquiryUrl"].ToString();

            if (paymentType == SkyNetPackageType.subscription.ToString())
            {
                Utils.WriteLog_Biller("GetSkyNetPackages | MessageId : " + messageId + ", " + paymentType + ", Package");
                var inquiryRequest = new
                {
                    Token = TokenManager.GetOAuthToken().Token,
                    PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
                    BillerCode = ConfigurationManager.AppSettings["SkynetBillerSubscriptionCode"].ToString(),
                    Detail = "{ 'SmartCard_or_ChipSet' : '" + ref1 + "'}"
                };


                string json = JsonConvert.SerializeObject(inquiryRequest);

                Utils.WriteLog_Biller("GetSkyNetPackages | MessageId : " + messageId + ", " + paymentType + ", Eba inquriy request for Skynet Packages" + " : " + json);

                string responseJson = Utils.PostEba(json, skyNetInquiryUrl);

                Utils.WriteLog_Biller("GetSkyNetPackages | MessageId : " + messageId + ", " + paymentType + ", Eba inquriy response for Skynet Packages" + " : " + responseJson);

                var inquiryResponse = JsonConvert.DeserializeObject<SkynetEnquiryResponseModel>(responseJson);
                if (!string.IsNullOrEmpty(responseJson) & inquiryResponse.ErrorCode == "00")
                {
                    string detailJson = inquiryResponse.Detail.Replace(@"\", "").Replace("\"[", "[").Replace("]\"", "]");
                    Utils.WriteLog_Biller("GetSkyNetPackages | MessageId : " + messageId + ", " + paymentType + ",  Eba inquriy response for Skynet Packages after replacing special characters" + " : " + detailJson);
                    var detailResponse = JsonConvert.DeserializeObject<SkynetEquiryDetail>(detailJson);

                    var skyNetPackageResponse = new SkyNetPackageRes
                    {
                        Version = version,
                        TimeStamp = timeStamp,
                        ResCode = code,
                        ResDesc = description,
                        Packages = detailResponse.Package
                    };
                    var xmlString = (new XMLSerializationService<SkyNetPackageRes>()).SerializeData(skyNetPackageResponse);

                    Utils.WriteLog_Biller("GetSkyNetPackages | MessageId : " + messageId + ", " + paymentType + ",  XML String" + " : " + xmlString);
                    return xmlString;
                }
                else
                {
                    return Utils.getErrorRes(inquiryResponse.ErrorCode, inquiryResponse.ErrorMessage);
                }
            }
            else
            {
                Utils.WriteLog_Biller("GetSkyNetPackages | MessageId : " + messageId + ", " + paymentType + ",  Package");
                var inquiryRequest = new
                {
                    Token = TokenManager.GetOAuthToken().Token,
                    PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
                    BillerCode = ConfigurationManager.AppSettings["SkynetBillerPayPerViewCode"].ToString(),
                    Detail = "{ 'SmartCard_or_ChipSet' : '" + ref1 + "'}"
                };


                string json = JsonConvert.SerializeObject(inquiryRequest);

                Utils.WriteLog_Biller("GetSkyNetPackages | MessageId : " + messageId + ", " + paymentType + ", Eba inquriy request for Skynet Packages" + " : " + json);

                string responseJson = Utils.PostEba(json, skyNetInquiryUrl);

                Utils.WriteLog_Biller("GetSkyNetPackages | MessageId : " + messageId + ", " + paymentType + ",  Eba inquriy response for Skynet Packages" + " : " + responseJson);

                var inquiryResponse = JsonConvert.DeserializeObject<SkynetEnquiryResponseModel>(responseJson);
                if (!string.IsNullOrEmpty(responseJson) & inquiryResponse.ErrorCode == "00")
                {
                    string detailJson = inquiryResponse.Detail.Replace(@"\", "").Replace("\"[", "[").Replace("]\"", "]");
                    Utils.WriteLog_Biller("GetSkyNetPackages | MessageId : " + messageId + ", " + paymentType + ", Eba inquriy response for Skynet Packages after replacing special characters" + " : " + detailJson);
                    var detailResponse = JsonConvert.DeserializeObject<SkynetPayPerViewEquiryDetail>(detailJson);

                    var skyNetPackageResponse = new SkyNetPackagePayPerViewRes { Version = version, TimeStamp = timeStamp, ResCode = code, ResDesc = description, Packages = detailResponse.Package };
                    var xmlString = (new XMLSerializationService<SkyNetPackagePayPerViewRes>()).SerializeData(skyNetPackageResponse);

                    Utils.WriteLog_Biller("GetSkyNetPackages | MessageId : " + messageId + ", " + paymentType + ", XML String" + " : " + xmlString);
                    return xmlString;
                }
                else
                {
                    return Utils.getErrorRes(inquiryResponse.ErrorCode, inquiryResponse.ErrorMessage);
                }
            }
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller("GetSkyNetPackages | error" + " : " + ex.Message);
        }
        return Utils.getErrorRes("06", "Get SkyNet Package failed");
    }

    public bool IsValidSkyNetPackagesRequest(Hashtable ht, out string code, out string description, out string version, out string timeStamp, out string messageId, out string cardId, out string paymentType)
    {
        code = "00";
        description = "Success";
        version = string.Empty;
        timeStamp = string.Empty;
        messageId = string.Empty;
        cardId = string.Empty;
        paymentType = string.Empty;


        if (ht.ContainsKey("Version"))
        {
            version = ht["Version"].ToString();
        }
        else
        {
            code = "06";
            description = "Invalid Request";
            Utils.WriteLog_Biller("Error in Validation : Missing XML <Version> Tag");
            return false;
        }
        if (ht.ContainsKey("TimeStamp"))
        {
            timeStamp = ht["TimeStamp"].ToString();
        }
        else
        {
            code = "06";
            description = "Invalid Request";
            Utils.WriteLog_Biller("Error in Validation : Missing XML <TimeStamp> Tag");
            return false;
        }

        if (ht.ContainsKey("MessageID"))
        {
            messageId = ht["MessageID"].ToString();
        }
        else
        {
            code = "06";
            description = "Invalid Request";
            Utils.WriteLog_Biller("Error in Validation : Missing XML <MessageID> Tag");
            return false;
        }
        if (ht.ContainsKey("Ref1"))
        {
            cardId = ht["Ref1"].ToString();
        }
        else
        {
            code = "06";
            description = "Invalid Request";
            Utils.WriteLog_Biller("Error in Validation : Missing XML <Ref1> Tag");
            return false;
        }

        if (ht.ContainsKey("PaymentType"))
        {
            paymentType = ht["PaymentType"].ToString();
        }
        else
        {
            code = "06";
            description = "Invalid Request";
            Utils.WriteLog_Biller("Error in Validation : Missing XML <PaymentType> Tag");
            return false;
        }


        return true;
    }

    public string GetInquiryResponse(SkyNetInquiryResponse inquiryResponseModel, string messageId, string taxId)
    {
        var skyNetInquiryUrl = ConfigurationManager.AppSettings["MobileLegendInquiryUrl"].ToString();
        var logAppender = "Skynet, GetInquiryResponse, MessageId : " + messageId + ", PackageType : " + inquiryResponseModel.PaymentType + " TaxId : " + taxId + ", Eba inquriy request for Skynet" + " : ";
        if (inquiryResponseModel.PaymentType == SkyNetPackageType.subscription.ToString())
        {
            var inquiryRequest = new
                {
                    Token = TokenManager.GetOAuthToken().Token,
                    PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
                    BillerCode = ConfigurationManager.AppSettings["SkynetBillerSubscriptionCode"].ToString(),
                    Detail = "{ 'SmartCard_or_ChipSet' : '" + inquiryResponseModel.ref1 + "'}"
                };


            string json = JsonConvert.SerializeObject(inquiryRequest);

            Utils.WriteLog_Biller(logAppender + json);

            string responseJson = Utils.PostEba(json, skyNetInquiryUrl);

            Utils.WriteLog_Biller(logAppender + responseJson);

            var inquiryResponse = JsonConvert.DeserializeObject<SkynetEnquiryResponseModel>(responseJson);

            if (!string.IsNullOrEmpty(responseJson) & inquiryResponse.ErrorCode == "00")
            {
                string responseCode = inquiryResponse.ErrorCode;
                string responseDescription = inquiryResponse.ErrorMessage;
                string detailJson = inquiryResponse.Detail.Replace("\\", "").Replace("'", "\"").Replace("\"[", "[").Replace("]\"", "]");
                Utils.WriteLog_Biller(logAppender + ", Eba inquriy response for Skynet Packages after replacing special characters" + " : " + detailJson);
                var detailResponse = JsonConvert.DeserializeObject<SkynetEquiryDetail>(detailJson);

                inquiryResponseModel.ResCode = inquiryResponse.ErrorCode;
                inquiryResponseModel.ResDesc = inquiryResponse.ErrorMessage;
                inquiryResponseModel.ref2 = detailResponse.SubscriptionNumber;
                inquiryResponseModel.status = inquiryResponse.TransactionStatus;
                inquiryResponseModel.ref4Name = string.Empty;


                return Utils.getInquiryRes(inquiryResponseModel);
            }
            else
            {
                return Utils.getErrorRes(inquiryResponse.ErrorCode, inquiryResponse.ErrorMessage);
            }
        }
        else
        {
            return Utils.getInquiryRes(inquiryResponseModel);

        }
    }

    public string ConfrimToEBA(SkyNetConfirmResponseModel confirmResponseModel, string amount, int agentId,
        double agentAmount, string isAgreement, double agentFeeDbl, string smsStatus, double availablebalance,
        string appType, string topupType, string agentName, string MapTaxID, string serviceFee, string totalAmount
        , string branchCode, string senderName, string messageId)
    {
        var logAppender = "MessageId : " + messageId + " | TxnId : " + confirmResponseModel.taxID + " | Skynet | ConfirmToEBA | " + confirmResponseModel.PaymentType + " | ";
        Utils.WriteLog_Biller("Skynet, ConfrimToEBA, This is Skynet from EBA Confirm");
        string ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();

        var token = TokenManager.GetOAuthToken();

        if (confirmResponseModel.PaymentType == SkyNetPackageType.subscription.ToString())
        {
            var confirmReq = new EbaConfirmReq()
            {
                Token = token.Token,
                PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
                PartnerRefNo = confirmResponseModel.txnID.ToString(),
                BillerCode = ConfigurationManager.AppSettings["SkynetBillerSubscriptionCode"].ToString(),
                TransactionAmount = amount,
                Detail = "{ 'SmartCard_or_ChipSet' : '" + confirmResponseModel.ref1 + "','ProductCode':'" + confirmResponseModel.ref3 + "'}"
            };

            string jsonReq = JsonConvert.SerializeObject(confirmReq);
            Utils.WriteLog_Biller(logAppender + "EBA Skynet Confirm Jason Request:" + jsonReq);

            SSLPost post = new SSLPost();
            string jsonres = Utils.PaymentRequest(jsonReq, ebaUrl);
            Utils.WriteLog_Biller(logAppender + "EBA Skynet Confirm Jason Response:" + jsonres);

            var responseDescription = string.Empty;
            var responseCode = string.Empty;
            if (string.IsNullOrEmpty(jsonres))
            {
                responseDescription = "No Response From EBA";
                responseCode = "06";

                return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(responseCode, responseDescription, long.Parse(confirmResponseModel.txnID), responseDescription, agentId, agentAmount, isAgreement);
            }

            var confirmRes = JsonConvert.DeserializeObject<SkyNetConfirmResponse>(jsonres);
            var errMsg = string.Empty;
            var batchID = default(int);

            if (!IsSuccess(confirmRes.ErrorCode))
            {
                responseDescription = Utils.EsbResponseDescription(confirmRes.ErrorCode);
                return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(confirmRes.ErrorCode, confirmRes.ErrorMessage,
                    long.Parse(confirmResponseModel.txnID), responseDescription, agentId, agentAmount, isAgreement);
            }
            var detail = JsonConvert.DeserializeObject<SkyNetConfirmDetailResponse>(confirmRes.Detail);
            confirmResponseModel.ref4 = detail.ExpiryDate;
            if (!(new ServiceClient()).ConfirmUpdate(long.Parse(confirmResponseModel.txnID), confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                    confirmResponseModel.ref5, "", "PA", "Paid Successfully", agentId, confirmResponseModel.email, agentAmount, agentFeeDbl,
                    isAgreement, smsStatus, availablebalance, out errMsg, out batchID))
            {
                Utils.WriteLog_Biller(logAppender + "Error in ConfirmUpdate : " + errMsg);
                responseDescription = "Error in update database";
                responseCode = "06";
                if (!(new ServiceClient()).updateError(long.Parse(confirmResponseModel.txnID), "ER", responseDescription, out errMsg))
                {
                    Utils.WriteLog_Biller(logAppender + "ConfrimToEBA, Error in updateError : " + errMsg);
                }
                return Utils.getErrorRes(responseCode, "Transaction fail");
            }

            Utils.WriteLog_Biller(logAppender + "After update = AgentAmount : " + agentAmount + " | Balance : " + availablebalance.ToString() + "| smsStatus:" + smsStatus);




            #region <-- Send SMS -->
            string smsMessage = string.Empty;
            smsMessage = SendSMS(appType, logAppender, topupType, agentName, MapTaxID, confirmResponseModel, long.Parse(confirmResponseModel.txnID), amount, serviceFee, totalAmount, branchCode, senderName);

            #endregion

            #region <-- Response Back To Client -->
            PopulateResponse(confirmResponseModel, confirmRes, availablebalance, long.Parse(confirmResponseModel.txnID), detail, smsMessage);
            return Utils.getConfirmRes(confirmResponseModel);

            #endregion
        }
        else
        {
            var confirmReq = new EbaConfirmReq()
            {
                Token = token.Token,
                PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
                PartnerRefNo = confirmResponseModel.txnID.ToString(),
                BillerCode = ConfigurationManager.AppSettings["SkynetBillerPayPerViewCode"].ToString(),
                TransactionAmount = amount,
                Detail = "{ 'SmartCard_or_ChipSet' : '" + confirmResponseModel.ref1 + "','ProductCode':'" + confirmResponseModel.ref2 + "','StartDate':'" + confirmResponseModel.ref3 + "','EndDate':'" + confirmResponseModel.ref4 + "'}"
            };

            string jsonReq = JsonConvert.SerializeObject(confirmReq);
            Utils.WriteLog_Biller(logAppender + "EBA Skynet Confirm Jason Request:" + jsonReq);

            SSLPost post = new SSLPost();
            string jsonres = Utils.PaymentRequest(jsonReq, ebaUrl);
            Utils.WriteLog_Biller(logAppender + "EBA Skynet Confirm Jason Response:" + jsonres);

            var responseDescription = string.Empty;
            var responseCode = string.Empty;
            if (string.IsNullOrEmpty(jsonres))
            {
                responseDescription = "No Response From EBA";
                responseCode = "06";

                return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(responseCode, responseDescription, long.Parse(confirmResponseModel.txnID), responseDescription, agentId, agentAmount, isAgreement);
            }

            var confirmRes = JsonConvert.DeserializeObject<SkyNetConfirmResponse>(jsonres);
            var errMsg = string.Empty;
            var batchID = default(int);

            if (!IsSuccess(confirmRes.ErrorCode))
            {
                responseDescription = Utils.EsbResponseDescription(confirmRes.ErrorCode);
                return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(confirmRes.ErrorCode, confirmRes.ErrorMessage,
                    long.Parse(confirmResponseModel.txnID), responseDescription, agentId, agentAmount, isAgreement);
            }

            if (!(new ServiceClient()).ConfirmUpdate(long.Parse(confirmResponseModel.txnID), confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                    confirmResponseModel.ref5, "", "PA", "Paid Successfully", agentId, confirmResponseModel.email, agentAmount, agentFeeDbl,
                    isAgreement, smsStatus, availablebalance, out errMsg, out batchID))
            {
                Utils.WriteLog_Biller(logAppender + "Error in ConfirmUpdate : " + errMsg);
                responseDescription = "Error in update database";
                responseCode = "06";
                if (!(new ServiceClient()).updateError(long.Parse(confirmResponseModel.txnID), "ER", responseDescription, out errMsg))
                {
                    Utils.WriteLog_Biller(logAppender + "ConfrimToEBA, Error in updateError : " + errMsg);
                }
                return Utils.getErrorRes(responseCode, "Transaction fail");
            }

            Utils.WriteLog_Biller(logAppender + "After update = AgentAmount : " + agentAmount + " | Balance : " + availablebalance.ToString() + "| smsStatus:" + smsStatus);

            var detail = JsonConvert.DeserializeObject<SkyNetConfirmPayPerViewDetailResponse>(confirmRes.Detail);

            #region <-- Send SMS -->
            string smsMessage = string.Empty;
            smsMessage = SendSMS(appType, logAppender, topupType, agentName, MapTaxID, confirmResponseModel, long.Parse(confirmResponseModel.txnID), amount, serviceFee, totalAmount, branchCode, senderName);

            #endregion

            #region <-- Response Back To Client -->
            PopulateResponse(confirmResponseModel, confirmRes, availablebalance, long.Parse(confirmResponseModel.txnID), detail, smsMessage);
            return Utils.getConfirmRes(confirmResponseModel);

            #endregion
        }
    }

    private bool IsSuccess(string responseCode)
    {
        return responseCode == "00";
    }


    private void PopulateResponse(SkyNetConfirmResponseModel confirmResponseModel, SkyNetConfirmResponse confirmRes, double availablebalance, long txnId, SkyNetConfirmPayPerViewDetailResponse detail, string smsMessage)
    {
        confirmResponseModel.rescode = confirmRes.ErrorCode;
        confirmResponseModel.resdesc = confirmRes.ErrorMessage;
        confirmResponseModel.availablebalance = availablebalance.ToString();
        confirmResponseModel.txnID = txnId.ToString();

        confirmResponseModel.ref1 = detail.ProductCode;
        confirmResponseModel.ref2 = detail.StartDate;
        confirmResponseModel.ref3 = detail.EndDate;
        confirmResponseModel.ref4 = detail.SubscriptionNumber;
        confirmResponseModel.smsMsg = smsMessage;
    }

    private void PopulateResponse(SkyNetConfirmResponseModel confirmResponseModel, SkyNetConfirmResponse confirmRes, double availablebalance, long txnId, SkyNetConfirmDetailResponse detail, string smsMessage)
    {
        confirmResponseModel.rescode = confirmRes.ErrorCode;
        confirmResponseModel.resdesc = confirmRes.ErrorMessage;
        confirmResponseModel.availablebalance = availablebalance.ToString();
        confirmResponseModel.txnID = txnId.ToString();

        confirmResponseModel.ref4 = detail.ExpiryDate;
        confirmResponseModel.smsMsg = smsMessage;
    }

    private string SendSMS(string appType, string logAppender, string topupType, string agentName, string mapTaxId, ConfirmResponseModel confirmResponseModel,
        long txnId, string amount, string serviceFee, string totalAmount, string branchCode, string senderName)
    {
        string smsMessage = string.Empty;
        if (appType == "CS" || appType == "MS")
        {

            Utils.WriteLog_Biller(logAppender + "appType is CS or MS");
            if (string.IsNullOrEmpty(topupType) || topupType == "S") //topup type is null or S
            {
                Utils.WriteLog_Biller(logAppender + "topupType is null or Not S");
                var smsH = new SMSHelper();
                MessagingService.MessagingServiceClient smsWcf = new MessagingService.MessagingServiceClient();

                smsMessage = smsH.GetMessageBiller(txnId.ToString(), agentName, mapTaxId, confirmResponseModel.billername, confirmResponseModel.ref1Name, null, confirmResponseModel.ref3Name, confirmResponseModel.ref4Name, confirmResponseModel.ref1,
                     confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4, double.Parse(amount).ToString("#,##0.00"), serviceFee.ToString(), double.Parse(totalAmount).ToString("#,##0.00"), branchCode);
                try
                {
                    Utils.WriteLog_Biller(logAppender + "Mobile No :" + confirmResponseModel.ref5 + "| Message :" + smsMessage + "| Sender Name :" + senderName + "|txn ID :" + txnId);
                    smsWcf.SendSms(txnId.ToString(), smsMessage, confirmResponseModel.ref5, senderName);
                    Utils.WriteLog_Biller(logAppender + "sendSMSWithTxnID ends.");
                }
                catch (Exception ex)
                {
                    Utils.WriteLog_Biller(string.Format(logAppender + "Error in SendSms: {0}", ex.ToString()));
                }
            }
        }
        return smsMessage;
    }
}