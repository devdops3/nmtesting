using MessagingService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Summary description for OnePayCashInManager
/// </summary>
public class OnePayCashInManager : IeService
{
	private readonly A2AAPIWCF.ServiceClient _desktopApiWcf;
    private readonly MobileAPIWCFManager _mobileApiWdcf;
    private readonly MessagingServiceClient smsWcf;
    public OnePayCashInManager()
    {
        _desktopApiWcf = new A2AAPIWCF.ServiceClient();
        _mobileApiWdcf = new MobileAPIWCFManager();
        smsWcf = new MessagingServiceClient();
    }

    public string Inquiry(inquiryModel model)
    {
        var messageId = model.messageid + " | ";
        try
        {
            Utils.WriteLog_Biller(messageId + "Start OnePay CashIn Inquiry.");
            var jsonRequest = PopulateInquiryRequest(model, messageId);
            Utils.WriteLog_Biller(messageId + "Start Calling OnePay CashIn Inquiry.");
            var jsonResponse = Post(OnePayConstants.InquiryUrl, jsonRequest);
            Utils.WriteLog_Biller(messageId + "OnePay CashIn Inquiry Json Response : " + jsonResponse);
            Utils.WriteLog_Biller(messageId + "End Calling OnePay CashIn Inquiry.");
            var inquiryResponse = JsonConvert.DeserializeObject<OnePayInquiryResponse>(jsonResponse);
            string[] invalidErrorCode = { "012", "014", "062", "409", "501", "060" };
            if (!inquiryResponse.ResponseCode.Equals(OnePayConstants.Success))
            {
                if (invalidErrorCode.Contains(inquiryResponse.ResponseCode))
                {
                    return Utils.getErrorRes("99", string.Empty);
                }
                return Utils.getErrorRes(inquiryResponse.ResponseCode, inquiryResponse.ResponseDescription);
            }
            var signatureString = inquiryResponse.InquiryDetail.AgentID + inquiryResponse.InquiryDetail.SubAgentID +
                                  inquiryResponse.InquiryDetail.AgentName + inquiryResponse.InquiryDetail.OriginalAmount +
                                  inquiryResponse.InquiryDetail.AgentCommission.ToString("0.00") + inquiryResponse.InquiryDetail.TotalAmount.ToString("0.00") +
                                  inquiryResponse.InquiryDetail.CustomerCharges.ToString("0.00") + inquiryResponse.InquiryDetail.InvoiceNo +
                                  inquiryResponse.InquiryDetail.SequenceNo + inquiryResponse.InquiryDetail.ReceiverNo;
            var manualHashValue = GetHashValue(signatureString).ToUpper();
            if (!manualHashValue.Equals(inquiryResponse.InquiryDetail.HashValue))
            {
                Utils.WriteLog_Biller(messageId + "HashValue Mismatched between manualHashValue and responseHashValue!!!");
                Utils.WriteLog_Biller(messageId + "ManualHashValue : " + manualHashValue + " | ResponseHashValue : " + inquiryResponse.InquiryDetail.HashValue);
                return Utils.getErrorRes("99", string.Empty);
            }
            var responseXML = PopulateInquiryResponseXML(model);
            Utils.WriteLog_Biller(messageId + "OnePay CashIn Inquiry : " + responseXML);
            Utils.WriteLog_Biller(messageId + "OnePay CashIn Inquiry.");
            return responseXML;
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(messageId + "Exception occurred in OnePay CashIn Inquiry : " + ex.Message);
            Utils.WriteLog_Biller(messageId + "OnePay CashIn Inquiry.");
            return Utils.getErrorRes("99", string.Empty);
        }
    }

    private string PopulateInquiryResponseXML(inquiryModel inquiryModel)
    {
        var responseXML = new OnePayInquiryResponseXML() 
        { 
            Version = "1.0",
            TimeStamp = System.DateTime.Now.ToString("yyyyMMddhhmmssffff"),
            ResCode = "00",
            ResDesc = "Success",
            TaxID = inquiryModel.taxID,
            BillerName = inquiryModel.billername,
            BillerLogo = inquiryModel.billerlogo,
            Ref1 = inquiryModel.ref1,
            Ref2 = inquiryModel.ref2,
            Ref3 = inquiryModel.ref3,
            Ref4 = inquiryModel.ref4,
            Ref5 = inquiryModel.ref5,
            Ref1Name = inquiryModel.ref1Name,
            Amount = inquiryModel.amount,
            AgentFee = inquiryModel.serviceFee,
            Status = inquiryModel.status,
            Expiry = inquiryModel.expiry,
            ProductDesc = inquiryModel.productDescription,
            ImageURL = inquiryModel.imgUrl
        };
        var resultXMLString = (new XMLSerializationService<OnePayInquiryResponseXML>()).SerializeData(responseXML);
        return resultXMLString;
    }

    private string PopulateInquiryRequest(inquiryModel responseModel, string messageId)
    {
        Utils.WriteLog_Biller(messageId + "Start Populating OnePay CashIn Inquiry Request.");
        var currentTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var sequenceNo = GetSequenceNo().ToUpper();
        var mobileNo = GetMobileNumberWithCountryCode(responseModel.ref1);
        var signatureString = OnePayConstants.AgentID + responseModel.ref1 + OnePayConstants.InvoiceNo + sequenceNo + mobileNo + responseModel.amount + OnePayConstants.ExpiredSeconds + currentTimeStamp;
        var hashValue = GetHashValue(signatureString);
        var request = new OnePayInquiryRequest
        {
            AgentID = OnePayConstants.AgentID,
            SubAgentID = responseModel.ref1,
            InvoiceNo = OnePayConstants.InvoiceNo,
            SequenceNo = sequenceNo,
            ReceiverNo = mobileNo,
            Amount = responseModel.amount,
            ExpiredSeconds = OnePayConstants.ExpiredSeconds,
            RequestTimeStamp = currentTimeStamp,
            HashValue = hashValue
        };
        responseModel.ref2 = sequenceNo;
        var jsonString = JsonConvert.SerializeObject(request);
        Utils.WriteLog_Biller(messageId + "OnePay CashIn Inquiry Json Request : " + jsonString);
        Utils.WriteLog_Biller(messageId + "End Populating OnePay CashIn Inquiry Request.");
        return jsonString;
    }

    private string GetMobileNumberWithCountryCode(string mobileNo)
    {
        mobileNo = "95" + mobileNo.Substring(1, mobileNo.Length - 1);
        return mobileNo;
    }

    private string GetSequenceNo()
    {
        Guid sequenceNo = Guid.NewGuid();
        return sequenceNo.ToString("N");
    }

    private string GetHashValue(string signatureString)
    {
        Utils.WriteLog_Biller("OnePay CashIn SignatureString : " + signatureString);
        var secretKey = OnePayConstants.SecretKey;
        var result = Utils.generateHashValue(signatureString, secretKey).ToUpper();
        return result;
    }

    public string Confirm(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo)
    {
        var messageId = confirmResponseModel.messageid + " | ";
        Utils.WriteLog_Biller(messageId + "Start " + responseInfo.billerCode + " Confirm region.");

        var confirmReq = PopulateConfirmRequest(confirmResponseModel, responseInfo.amount);
        Utils.WriteLog_Biller(messageId + "Start Calling OnePay CashIn Confirm.");
        var jsonres = Post(OnePayConstants.ConfirmUrl, confirmReq);
        Utils.WriteLog_Biller(messageId + "OnePay CashIn Json Confirm Request : " + jsonres);
        Utils.WriteLog_Biller(messageId + "End Calling OnePay CashIn Confirm.");
        var responseDescription = string.Empty;
        var responseCode = string.Empty;
        if (string.IsNullOrEmpty(jsonres))
        {
            responseDescription = "No Response From OnePay";
            responseCode = "99";
            Utils.WriteLog_Biller(messageId + "OnePay CashIn Confirm Response is Null");
            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(responseCode, responseDescription, responseInfo.txnID, responseDescription, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }

        var confirmRes = JsonConvert.DeserializeObject<OnePayConfirmResponse>(jsonres);
        var errMsg = string.Empty;
        var batchID = default(int);
        string[] invalidErrorCode = { "012", "014", "062", "409", "501" };
        if (!confirmRes.ResponseCode.Equals(OnePayConstants.Success))
        {
            if (invalidErrorCode.Contains(confirmRes.ResponseCode))
            {
                confirmRes.ResponseCode = "99";
                confirmRes.ResponseDescription = string.Empty;
            }
            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(confirmRes.ResponseCode, confirmRes.ResponseDescription,
                    responseInfo.txnID, responseDescription, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }
        confirmResponseModel = PopulateConfirmResponse(confirmRes, confirmResponseModel);
        if (!UpdateTransactionStatus(confirmResponseModel, responseInfo, out errMsg, out batchID))
        {
            return UpdateErrorStatus(responseInfo.txnID, errMsg, confirmResponseModel.messageid);
        }
        Utils.WriteLog_Biller("After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString() + "| MessageId : " + messageId);
        confirmResponseModel.rescode = "00";
        confirmResponseModel.resdesc = "Success";
        confirmResponseModel.availablebalance = responseInfo.availablebalance.ToString();
        confirmResponseModel.txnID = responseInfo.txnID.ToString();

        Utils.WriteLog_Biller(messageId + "End " + responseInfo.billerCode + " Confirm region.");
        return Utils.getConfirmRes(confirmResponseModel);

    }

    private ConfirmResponseModel PopulateConfirmResponse(OnePayConfirmResponse confirmResponse, ConfirmResponseModel responseModel)
    {
        responseModel.ref3 = confirmResponse.OnePayUserName;
        return responseModel;
    }

    private bool UpdateTransactionStatus(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, out string errMsg, out int batchID)
    {
        return _desktopApiWcf.ConfirmUpdate(responseInfo.txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                    confirmResponseModel.ref5, "", "PA", "Paid Successfully", responseInfo.agentID, confirmResponseModel.email, responseInfo.agentAmount, responseInfo.agentFeeDbl,
                    responseInfo.isAgreement, responseInfo.smsStatus, responseInfo.availablebalance, out errMsg, out batchID);
    }

    private string UpdateErrorStatus(long txnID, string errMsg, string messageId)
    {
        Utils.WriteLog_Biller(messageId + "Error in ConfirmUpdate : " + errMsg);
        string responseDescription = "Error in update database";
        string responseCode = "06";
        if (!_desktopApiWcf.updateError(txnID, "ER", responseDescription, out errMsg))
        {
            Utils.WriteLog_Biller(messageId + "Error in updateError : " + errMsg);
        }
        return Utils.getErrorRes(responseCode, "Transaction fail");

    }


    private bool IsSuccess(string errorCode)
    {
        return errorCode == OnePayConstants.Success;
    }

    private string PopulateConfirmRequest(ConfirmResponseModel confirmResponse, string amount)
    {
        Utils.WriteLog_Biller(confirmResponse.messageid + " | Start Populating OnePay CashIn Confirm Request");
        var mobileNo = GetMobileNumberWithCountryCode(confirmResponse.ref1);
        var currentTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var signatureString = OnePayConstants.AgentID + confirmResponse.ref1 + confirmResponse.ref2 + mobileNo + amount + currentTimeStamp;
        var hashValue = GetHashValue(signatureString);
        var model = new OnePayConfirmRequest()
        {
            AgentID = OnePayConstants.AgentID,
            SubAgentID = confirmResponse.ref1,
            SequenceNo = confirmResponse.ref2,
            ReceiverNo = mobileNo,
            Amount = amount,
            RequestTimeStamp = currentTimeStamp,
            HashValue = hashValue
        };
        var jsonReqest = JsonConvert.SerializeObject(model);
        Utils.WriteLog_Biller(confirmResponse.messageid + " | OnePay CashIn Json Confirm Request : " + jsonReqest);
        Utils.WriteLog_Biller(confirmResponse.messageid + " | End Populating OnePay CashIn Confirm Request");
        return jsonReqest;
    }

    private string Post(string url, string jsonRequest)
    {
        var result = string.Empty;
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            streamWriter.Write(jsonRequest);
            streamWriter.Flush();
            streamWriter.Close();
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            result = streamReader.ReadToEnd();
        }
        return result;
    }
}