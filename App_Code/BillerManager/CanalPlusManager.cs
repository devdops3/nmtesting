using A2AAPIWCF;
using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for CanalPlusManager
/// </summary>
public class CanalPlusManager
{
	public CanalPlusManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string getPackagesRes(string cardNumber, string loginTye)
    {
        Utils.WriteLog_Biller("In Get CanalPlus Packages");
        var process = "CheckAccount";

        var inquiryReq = new
        {
            Token = TokenManager.GetOAuthToken().Token,
            PartnerCode = BISConstants.ESBAChannel,
            BillerCode = BISConstants.CanalPlusBillerCode,
            Detail = "{'Process' : '" + process
                    + "', 'CardNumberOrSerialNumber' : '" + cardNumber
                    + "'}"
        };

        string json = JsonConvert.SerializeObject(inquiryReq);

        Utils.WriteLog_Biller("Eba Get CanalPlus Package request" + " : " + json);

        string responseJson = Utils.PostEba(json, BISConstants.EBAInquiryUrl);

        Utils.WriteLog_Biller("Eba Get CanalPlus Package response" + " : " + responseJson);

        var inquiryResponse = JsonConvert.DeserializeObject<EbaInquiryRes>(responseJson);
        var packages = new List<CanalPlusPackage>();
        if (!string.IsNullOrEmpty(responseJson) & inquiryResponse.ErrorCode == "00")
        {
            var detail = JsonConvert.DeserializeObject<CanalPlusPackageDetailRes>(inquiryResponse.Detail);
            StringBuilder sbLog = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            var resCode = "00";
            var resDesc = "Success";
            sb.Append("<GetCanalPlusPackagesRes>");
            sb.Append("<Version>1.0</Version>");
            sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
            sb.Append("<ResCode>" + resCode + "</ResCode>");
            sb.Append("<ResDesc>" + resDesc + "</ResDesc>");
            sb.Append("<Ref1>" + cardNumber + "</Ref1>");
            sb.Append("<Ref2>" + detail.SubscriberNumber + "</Ref2>");
            sb.Append("<Ref3>" + string.Empty + "</Ref3>");
            sb.Append("<Ref4>" + detail.ContractNumber + "</Ref4>");
            sb.Append("<Ref5>" + detail.IdBase + "</Ref5>");
            sb.Append("<Ref1Name>" + string.Empty + "</Ref1Name>");
            sb.Append("<Ref2Name>" + string.Empty + "</Ref2Name>");
            sb.Append("<Ref3Name>" + string.Empty + "</Ref3Name>");
            sb.Append("<Ref4Name>" + string.Empty + "</Ref4Name>");
            sb.Append("<Ref5Name>" + string.Empty + "</Ref5Name>");
            sb.Append("<Packages>");
            foreach (CanalPlusPackage package in detail.Packages)
            {
                sb.Append("<Package>");
                sb.Append("<Code>" + package.ProductCode + "</Code>");
                sb.Append("<Label>" + HttpUtility.HtmlEncode(package.Description) + "</Label>");
                sb.Append("<Durations>");
                package.Durations = package.Durations.OrderBy(duration => Convert.ToInt32(duration.Code)).ToList();
                foreach (Duration duration in package.Durations)
                {
                    sb.Append("<Duration><Code>" + duration.Code + "</Code>");
                    sb.Append("<Label>" + duration.Label + "</Label></Duration>");
                }
                sb.Append("</Durations>");
                sb.Append("<AddOnPackages>");
                if (package.AddOnPackage != null)
                {
                    if (loginTye.Equals("CP"))
                    {
                        sb.Append("<AddOnPackage><Code>00</Code>");
                        sb.Append("<Label>Select</Label></AddOnPackage>");
                    }
                    foreach (AddOnPackage addon in package.AddOnPackage)
                    {
                        sb.Append("<AddOnPackage><Code>" + addon.Code + "</Code>");
                        sb.Append("<Label>" + HttpUtility.HtmlEncode(addon.Label) + "</Label></AddOnPackage>");
                    }
                }
                else 
                {
                    if (loginTye.Equals("CP"))
                    {
                        sb.Append("<AddOnPackage><Code>00</Code>");
                        sb.Append("<Label>Select</Label></AddOnPackage>");
                    }
                }
                sb.Append("</AddOnPackages>");
                sb.Append("</Package>");
            }

            sb.Append("</Packages>");
            sb.Append("<SessionID>" + detail.SessionID + "</SessionID>");
            sb.Append("</GetCanalPlusPackagesRes>");
            Utils.WriteLog_Biller("GetCanalPlusPackages Res : " + sb.ToString());
            return sb.ToString();

        }
        else
        {
            return Utils.getErrorRes(inquiryResponse.ErrorCode, inquiryResponse.ErrorMessage);
        }
    }

    public List<CanalPlusPackage> GeneratePackageList(string packageListStr)
    {
        Utils.WriteLog_Biller("##### Start GeneratePackageList #####");
        var packageList = new List<CanalPlusPackage>();

        packageList = JsonConvert.DeserializeObject<List<CanalPlusPackage>>(packageListStr);
        Utils.WriteLog_Biller("Canal Plus package list : " + JsonConvert.SerializeObject(packageList));
        Utils.WriteLog_Biller("##### End GeneratePackageList #####");
        return packageList;
    }

    public string getInquiryRes(inquiryResponseModel inquiryResponseModel, string canalPlusReqType, string messageid, string servicePercent, string serviceFlatFee, string sessionID)
    {
        try
        {
            Utils.WriteLog_Biller(messageid + " ##### Start CanalPlus Inquiry #####");
            var process = "VerifyRenewalOffer";
            var detail = string.Empty;
            if(inquiryResponseModel.ref3.Split(',').Count() == 3)
            {
                var package = inquiryResponseModel.ref3.Split(',')[0].ToString();
                var duration = inquiryResponseModel.ref3.Split(',')[1].ToString();
                var addOnPackage = inquiryResponseModel.ref3.Split(',')[2].ToString();
                detail = "{ 'Process' : '" + process
                            + "', 'CardNumberOrSerialNumber' : '" + inquiryResponseModel.ref1
                            + "', 'IdBase' : '" + inquiryResponseModel.ref5
                            + "', 'SubscriberNumber' : '" + inquiryResponseModel.ref2
                            + "', 'ContractNumber' : '" + inquiryResponseModel.ref4
                            + "', 'Package' : '" + package
                            + "', 'Duration' : '" + duration
                            + "', 'AddOnPackage' : '" + addOnPackage
                            + "', 'SessionID' : '" + sessionID
                            + "'}";
            }
            else
            {
                var package = inquiryResponseModel.ref3.Split(',')[0].ToString();
                var duration = inquiryResponseModel.ref3.Split(',')[1].ToString();
                detail = "{ 'Process' : '" + process
                            + "', 'CardNumberOrSerialNumber' : '" + inquiryResponseModel.ref1
                            + "', 'IdBase' : '" + inquiryResponseModel.ref5
                            + "', 'SubscriberNumber' : '" + inquiryResponseModel.ref2
                            + "', 'ContractNumber' : '" + inquiryResponseModel.ref4
                            + "', 'Package' : '" + package
                            + "', 'Duration' : '" + duration
                            + "', 'SessionID' : '" + sessionID
                            + "'}";
            }
            var inquiryRequest = new
            {
                Token = TokenManager.GetOAuthToken().Token,
                PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
                BillerCode = BISConstants.CanalPlusBillerCode,
                Detail = detail
            };

            string json = JsonConvert.SerializeObject(inquiryRequest);

            Utils.WriteLog_Biller(messageid + "Eba inquriy request for CanalPlus" + " : " + json);

            string responseJson = Utils.PostEba(json, BISConstants.EBAInquiryUrl);

            Utils.WriteLog_Biller(messageid + "Eba inquriy response for CanalPlus" + " : " + responseJson);

            var inquiryResponse = JsonConvert.DeserializeObject<EbaInquiryRes>(responseJson);

            if (!string.IsNullOrEmpty(responseJson) & inquiryResponse.ErrorCode.Equals("00"))
            {
                inquiryResponseModel.ResCode = inquiryResponse.ErrorCode;
                inquiryResponseModel.ResDesc = inquiryResponse.ErrorMessage;
                var detailResponse = JsonConvert.DeserializeObject<CanalPlusUpgradeInquiryDetailRes>(inquiryResponse.Detail);
                double serviceFeeDbl = Utils.getFee(double.Parse(detailResponse.Amount), float.Parse(servicePercent), double.Parse(serviceFlatFee));
                string serviceFee = serviceFeeDbl.ToString("###0.00");

                var resultXMLString = getUpgradeXmLInquiryRes(inquiryResponseModel, detailResponse, serviceFee, inquiryResponse.TransactionAmount);

                Utils.WriteLog_Biller(messageid + " Eba inquriy response XML for CanalPlus" + resultXMLString);
                Utils.WriteLog_Biller(messageid + " ##### End CanalPlus Inquiry #####");
                return resultXMLString;
            }
            else
            {
                return Utils.getErrorRes(inquiryResponse.ErrorCode, inquiryResponse.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller("Error in CanalPlus Inquiry Process : " + ex.Message);
            return Utils.getErrorRes("99", string.Empty);
        }
    }

    public string getConfirmRes(ConfirmResponseModel confirmResponseModel, string amount, ResponseInfo responseInfo, string canalPlusConfirmReqType, string sessionId, string packageCode, string durationCode)
    {
        try 
        {
            string messageId = confirmResponseModel.messageid + " | ";
            Utils.WriteLog_Biller(messageId + "Start CanalPlus Biller Confirm");

            var confirmReq = PopulateUpgradeEBAConfirmReq(responseInfo.txnID, amount, confirmResponseModel, sessionId, packageCode, durationCode);
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
                var detail = JsonConvert.DeserializeObject<CanalPlusUpgradeConfirmDetailRes>(confirmRes.Detail);
                confirmResponseModel = PopulateUpgradeConfirmResponse(detail, confirmResponseModel);

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
                SendSMS(responseInfo, confirmResponseModel, detail.EndDate, amount, out smsMsg);
                confirmResponseModel.smsMsg = smsMsg;

                return getUpgradeXMLConfirmRes(confirmResponseModel, detail, responseInfo, packageCode);

            }
            else
            {
                responseDescription = Utils.EsbResponseDescription(confirmRes.ErrorCode);
                return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(confirmRes.ErrorCode, confirmRes.ErrorMessage,
                    responseInfo.txnID, responseDescription, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
            }

        }
        catch(Exception ex)
        {
            Utils.WriteLog_Biller("Error in CanalPlus getConfirmRes : " + ex.Message);
            return Utils.getErrorRes("99", string.Empty);
        }
    }

    #region Private Methods
    private string ConfirmEBA(EbaConfirmReq confirmReq, string messageId)
    {
        string ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();

        string jsonReq = JsonConvert.SerializeObject(confirmReq);
        Utils.WriteLog_Biller("CanalPlus EBA Renewal Confirm Jason Request:" + jsonReq + "MessageId : " + messageId);

        SSLPost post = new SSLPost();
        string jsonres = Utils.PostEba(jsonReq, ebaUrl);
        Utils.WriteLog_Biller("CanalPlus EBA Renewal Confirm Jason Response:" + jsonres + "MessageId : " + messageId);

        return jsonres;

    }
    
    private EbaConfirmReq PopulateUpgradeEBAConfirmReq(long txnID, string amount, ConfirmResponseModel responseModel, string sessionId, string packageCode, string durationCode)
    {
        var detail = string.Empty;
        string addOnPackageCode = string.Empty;
        string PackageCode = string.Empty;
        PackageCode = packageCode.Split(',')[0].ToString();
        var token = TokenManager.GetOAuthToken();
        if (packageCode.Split(',').Count() == 2)
        {
            addOnPackageCode = packageCode.Split(',')[1].ToString();
            detail = "{ 'CardNumberOrSerialNumber' : '" + responseModel.ref1
                    + "', 'IdBase' : '" + responseModel.ref5
                    + "', 'SubscriberNumber' : '" + responseModel.ref2
                    + "', 'ContractNumber' : '" + responseModel.ref4
                    + "', 'Package' : '" + PackageCode
                    + "', 'Duration' : '" + durationCode
                    + "', 'AddOnPackage' : '" + addOnPackageCode
                    + "', 'SessionID' : '" + sessionId
                    + "', 'Amount' : '" + amount
                    + "'}";
        }
        else 
        {
            detail = "{ 'CardNumberOrSerialNumber' : '" + responseModel.ref1
                    + "', 'IdBase' : '" + responseModel.ref5
                    + "', 'SubscriberNumber' : '" + responseModel.ref2
                    + "', 'ContractNumber' : '" + responseModel.ref4
                    + "', 'Package' : '" + PackageCode
                    + "', 'Duration' : '" + durationCode
                    + "', 'SessionID' : '" + sessionId
                    + "', 'Amount' : '" + amount
                    + "'}";
        }
        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = txnID.ToString(),
            BillerCode = ConfigurationManager.AppSettings["CanalPlusBillerCode"].ToString(),
            TransactionAmount = amount,
            Detail = detail
        };

        return confirmReq;

    }

    private bool IsSuccess(string errorCode)
    {
        if (errorCode == "00")
            return true;
        else return false;
    }

    private bool UpdateTransactionStatus(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, out string errMsg, out int batchID)
    {
        ServiceClient A2AAPIwcfService = new ServiceClient();
        return A2AAPIwcfService.ConfirmUpdate(responseInfo.txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                    confirmResponseModel.ref5, "", "PA", "Paid Successfully", responseInfo.agentID, confirmResponseModel.email, responseInfo.agentAmount, responseInfo.agentFeeDbl,
                    responseInfo.isAgreement, responseInfo.smsStatus, responseInfo.availablebalance, out errMsg, out batchID);
    }

    private void SendSMS(ResponseInfo responseInfo, ConfirmResponseModel confirmResponseModel, string endDate, string amount, out string smsMsg)
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

                smsMsg = smsH.getMessageBiller(responseInfo.agentName,
                    responseInfo.MapTaxID, confirmResponseModel.billername,
                    confirmResponseModel.ref1Name, confirmResponseModel.ref3Name,
                    "Expiry", "Ref", confirmResponseModel.ref1,
                    confirmResponseModel.ref3, endDate, responseInfo.txnID.ToString(),
                            double.Parse(amount).ToString("#,##0.00"), responseInfo.serviceFee, double.Parse(responseInfo.totalAmount).ToString("#,##0.00"), responseInfo.branchCode);

                try
                {

                    Utils.WriteLog_Biller("Mobile No :" + confirmResponseModel.ref6 + "| Message :" + smsMsg + "| Sender Name :" + responseInfo.sendername + "|txn ID :" + responseInfo.txnID);
                    smsWcf.SendSms(responseInfo.txnID.ToString(), smsMsg, confirmResponseModel.ref6, responseInfo.sendername);
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

    private ConfirmResponseModel PopulateUpgradeConfirmResponse(CanalPlusUpgradeConfirmDetailRes confirmDetail, ConfirmResponseModel confirmResponseModel)
    {
        Utils.WriteLog_Biller("Populate CanalPlus Confirm Response ref6 : " + confirmResponseModel.ref6 + ", MessageId : " + confirmResponseModel.messageid);
        confirmResponseModel.ref4 = confirmDetail.EndDate;
        confirmResponseModel.ref5 = confirmResponseModel.ref6;
        return confirmResponseModel;
    }

    private string getUpgradeXMLConfirmRes(ConfirmResponseModel confirmRes, CanalPlusUpgradeConfirmDetailRes detailResponse, ResponseInfo responseInfo, string packagecode)
    {
        var response = new CanlaPlusUpgradeConfirmResponse
        {
            Version = "1.0",
            TimeStamp = System.DateTime.Now.ToString("yyyyMMddhhmmssffff"),
            Ref1 = confirmRes.ref1,
            Ref2 = confirmRes.ref2,
            Ref3 = HttpUtility.HtmlEncode(confirmRes.ref3),
            Ref4 = detailResponse.EndDate,
            Ref5 = confirmRes.ref5,
            Ref1Name = confirmRes.ref1Name,
            Ref2Name = confirmRes.ref2Name,
            Ref3Name = confirmRes.ref3Name,
            Ref4Name = confirmRes.ref4Name,
            Ref5Name = confirmRes.ref5Name,
            AgentFee = responseInfo.serviceFee,
            Balance = responseInfo.availablebalance.ToString(),
            TxnID = confirmRes.txnID,
            TodayTxnCount = confirmRes.TodayTxnCount,
            TodayTxnAmount = confirmRes.TodayTxnAmount,
            SMS = confirmRes.smsMsg,
            ResCode = "00",
            ResDesc = "Success"
        };

        var resultXMLString = (new XMLSerializationService<CanlaPlusUpgradeConfirmResponse>()).SerializeData(response);

        Utils.WriteLog_Biller("This is Upgrade CanalPlus biller Confirm Response : " + resultXMLString);
        return resultXMLString;
    }

    private string getUpgradeXmLInquiryRes(inquiryResponseModel inquiryResponseModel, CanalPlusUpgradeInquiryDetailRes detailResponse, string serviceFee, string totalAmount)
    {
        var addOnPackageCode = string.Empty;
        var addOnPackageLabelRef3 = string.Empty;
        if(inquiryResponseModel.ref3.Split(',').Count() == 3)
        {
            addOnPackageCode = "," + detailResponse.Package[0].AddOnPackage.Code;
            addOnPackageLabelRef3 = " + " + detailResponse.Package[0].AddOnPackage.Label;
        }
        var packageCode = inquiryResponseModel.ref3.Split(',')[0];
        var packageDescription = detailResponse.Package[0].Description;
        var durationCode = detailResponse.Package[0].Duration.Code;
        var durationLabel = detailResponse.Package[0].Duration.Label;
 
        var response = new CanalPlusUpgradeInquiryResponse();
        response.Version = "1.0";
        response.TimeStamp = System.DateTime.Now.ToString("yyyyMMddhhmmssffff");
        response.ResCode = inquiryResponseModel.ResCode;
        response.ResDesc = inquiryResponseModel.ResDesc;
        response.Ref1 = detailResponse.CardNumberOrSerialNumber;
        response.Ref2 = detailResponse.SubscriberNumber;
        response.Ref3 = packageDescription + " (" + durationLabel + ")" + addOnPackageLabelRef3;
        response.Ref3 = HttpUtility.HtmlEncode(response.Ref3);
        response.Ref4 = detailResponse.ContractNumber;
        response.Ref5 = detailResponse.IdBase;
        response.Ref1Name = inquiryResponseModel.ref1Name;
        response.Ref2Name = inquiryResponseModel.ref2Name;
        response.Ref3Name = inquiryResponseModel.ref3Name;
        response.Ref4Name = string.Empty;
        response.Ref5Name = string.Empty;
        response.PackageCode = packageCode + addOnPackageCode;
        response.DurationCode = durationCode;
        response.Amount = detailResponse.Amount;
        response.TotalAmount = totalAmount;
        response.AgentFee = serviceFee;
        response.SessionID = detailResponse.SessionID;
        response.Status = "Success";
        response.BillerName = inquiryResponseModel.billername;
        response.BillerLogo = inquiryResponseModel.billerlogo;
        response.ImageURL = inquiryResponseModel.imgUrl;
        response.TaxID = inquiryResponseModel.taxID;

        var resultXMLString = (new XMLSerializationService<CanalPlusUpgradeInquiryResponse>()).SerializeData(response);
        return resultXMLString;
    }
    #endregion

}