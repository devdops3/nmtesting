using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SayaManager
/// </summary>
public class SayaManager : IeService
{
    private readonly A2AAPIWCF.ServiceClient _desktopApiWcf;
    private readonly MobileAPIWCFManager _mobileApiWdcf;
	public SayaManager()
	{
        _desktopApiWcf = new A2AAPIWCF.ServiceClient();
        _mobileApiWdcf = new MobileAPIWCFManager();
	}

    public string Inquiry(inquiryModel model)
    {
        string messageId = model.messageid + " | ";
        string responseXMLString = string.Empty;
        List<Package> packages = new List<Package>();
        Utils.WriteLog_Biller(messageId + "Start SAYA Inquiry Method.");
        try
        {
            var userId = model.ref1;
            var userIdWithCountryCode = GetUserIdWithCountryCode(userId);
            Utils.WriteLog_Biller(messageId + "SAYA UserIdWithCountryCode : " + userIdWithCountryCode);
            var inquiryRequest = new 
            {
                Token = TokenManager.GetOAuthToken().Token,
                PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
                BillerCode = ConfigurationManager.AppSettings["SayaBillerCode"].ToString(),
                Detail = "{ 'UserId' : '" + userIdWithCountryCode + "'}"
            };
            string json = JsonConvert.SerializeObject(inquiryRequest);
            Utils.WriteLog_Biller(messageId + "EBA inquiry Request for SAYA : " + json);
            var responseJson = Utils.PostEba(json, BISConstants.EBAInquiryUrl);
            Utils.WriteLog_Biller(messageId + "Eba inquriy response for SAYA : " + responseJson);
            var inquiryResponse = JsonConvert.DeserializeObject<EbaInquiryRes>(responseJson);

            if (!string.IsNullOrEmpty(responseJson) & inquiryResponse.ErrorCode == "00")
            {
                string responseCode = inquiryResponse.ErrorCode;
                string responseDescription = inquiryResponse.ErrorMessage;
                string detailJson = inquiryResponse.Detail.Replace(@"\", "").Replace("\"[", "[").Replace("]\"", "]");
                var detailResponse = JsonConvert.DeserializeObject<SayaInquiryDetailRes>(detailJson);
                if (detailResponse.Package.Count > 0)
                    detailResponse.Package = GetPackagesWithAgentFee(detailResponse.Package, model.servicePercent, model.serviceFlatFee);
                responseXMLString = getInquiryResXML(model, detailResponse);
            }
            else
            {
                Utils.WriteLog_Biller(messageId + "Error occurred from EBA : " + inquiryResponse.ErrorMessage);
                return Utils.getErrorRes(inquiryResponse.ErrorCode, inquiryResponse.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(messageId + "Exception error occurred in Saya Inquiry : " + ex.Message);
            return Utils.getErrorRes("99", string.Empty);
        }
        Utils.WriteLog_Biller(messageId + "End SAYA Inquiry Method.");
        return responseXMLString;
    }

    private string GetUserIdWithCountryCode(string userId) 
    {
        return "95" + userId.Substring(1, userId.Length - 1);
    }

    private List<Package> GetPackagesWithAgentFee(List<Package> packages, string servicePercent, string serviceFlatFlee)
    {
        Utils.WriteLog_Biller("In Saya GetPackagesWithAgentFee method");
        for (var count = 0; count < packages.Count; count++)
        {
            var serviceFeeDbl = Utils.getFee(double.Parse(packages[count].Amount), float.Parse(servicePercent), double.Parse(serviceFlatFlee));
            var serviceFee = serviceFeeDbl.ToString("###0.00");
            packages[count].AgentFee = serviceFee;
            packages[count].Name = HttpUtility.HtmlEncode(packages[count].Name);
        }
        Utils.WriteLog_Biller("Saya Packagelist : " + JsonConvert.SerializeObject(packages));
        return packages;
    }

    private string getInquiryResXML(inquiryModel model, SayaInquiryDetailRes detail)
    {
        var response = new SayaInquiryResponseXML
        {
            Version = "1.0",
            TimeStamp = System.DateTime.Now.ToString("yyyyMMddhhmmssffff"),
            ResCode = "00",
            ResDesc = "Success",
            Ref1 = model.ref1,
            Ref2 = (!string.IsNullOrEmpty(detail.UserName)) ? HttpUtility.HtmlEncode(detail.UserName) : "-",
            Ref3 = string.Empty,
            Ref4 = string.Empty,
            Ref5 = string.Empty,
            Ref1Name = model.ref1Name,
            Ref2Name = model.ref2Name,
            Ref3Name = string.Empty,
            Ref4Name = string.Empty,
            Ref5Name = string.Empty,
            Amount = string.Empty,
            AgentFee = string.Empty,
            Status = "Success",
            BillerName = model.billername,
            BillerLogo = model.billerlogo,
            ImageURL = model.imgUrl,
            TaxID = model.taxID,
            Packages = detail.Package
        };

        var resultXMLString = (new XMLSerializationService<SayaInquiryResponseXML>()).SerializeData(response);
        return resultXMLString;
    }

    public string Confirm(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo)
    {
        var messageId = confirmResponseModel.messageid;
        Utils.WriteLog_Biller("This is SAYA biller Confirm : messageId :" + messageId);

        var confirmReq = PopulateEBAConfirmReq(confirmResponseModel, responseInfo);
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
            var detail = JsonConvert.DeserializeObject<SayaConfirmDetailRes>(confirmRes.Detail);

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
                Ref1Name = "Reg Mobile No.",
                Ref2Name = "Package",
                Ref4Name = "Ref",
                Ref1Value = confirmResponseModel.ref1,
                Ref2Value = confirmResponseModel.ref3,
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

    private EbaConfirmReq PopulateEBAConfirmReq(ConfirmResponseModel confirmResponse, ResponseInfo responseInfo)
    {
        var userId = GetUserIdWithCountryCode(confirmResponse.ref1);
        var token = TokenManager.GetOAuthToken();
        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = responseInfo.txnID.ToString(),
            BillerCode = responseInfo.billerCode,
            TransactionAmount = responseInfo.amount,
            Detail = "{'UserId':'" + userId
                    + "', 'ProductCode':'" + confirmResponse.ref4 
                    + "'}"
        };
        return confirmReq;
    }

    private bool UpdateTransactionStatus(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, out string errMsg, out int batchID)
    {
        return _desktopApiWcf.ConfirmUpdate(responseInfo.txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, HttpUtility.HtmlEncode(confirmResponseModel.ref3), confirmResponseModel.ref4,
                    confirmResponseModel.ref5, "", "PA", "Paid Successfully", responseInfo.agentID, confirmResponseModel.email, responseInfo.agentAmount, responseInfo.agentFeeDbl,
                    responseInfo.isAgreement, responseInfo.smsStatus, responseInfo.availablebalance, out errMsg, out batchID);
    }

    private string UpdateErrorStatus(long txnID, string errMsg, string messageId)
    {
        Utils.WriteLog_Biller(messageId + " Error in ConfirmUpdate : " + errMsg);
        string responseDescription = "Error in update database";
        string responseCode = "06";
        if (!_desktopApiWcf.updateError(txnID, "ER", responseDescription, out errMsg))
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

    private ConfirmResponseModel PopulateConfirmResponse(SayaConfirmDetailRes confirmDetail, ConfirmResponseModel confirmResponseModel)
    {
        confirmResponseModel.ref2Name = string.Empty;
        return confirmResponseModel;
    }
}