using MessagingService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Summary description for SMGFBillerManager
/// </summary>
public class SMGFBillerManager
{
    private readonly A2AAPIWCF.ServiceClient _desktopApiWcf;
    private readonly MobileAPIWCFManager _mobileApiWdcf;
    private readonly MessagingServiceClient smsWcf;
    public SMGFBillerManager()
    {
        _desktopApiWcf = new A2AAPIWCF.ServiceClient();
        _mobileApiWdcf = new MobileAPIWCFManager();
        smsWcf = new MessagingServiceClient();
    }

    #region Inquiry

    public string Inquiry(SMGFInquiryResponseXML responseModel, string messageId, string servicePercent, string serviceFlatFee)
    {
        try
        {
            var jsonRequest = PopulateInquiryRequest(responseModel, messageId);
            Utils.WriteLog_Biller(messageId + "Start Calling SMGF Inquiry Post.");
            Utils.WriteLog_Biller(messageId + "SMGF Inquiry Json Request : " + jsonRequest);
            var jsonResponse = Post(SMGFConstants.InquiryUrl, jsonRequest);
            Utils.WriteLog_Biller(messageId + "SMGF Inquiry Json Response : " + jsonResponse);
            Utils.WriteLog_Biller(messageId + "End Calling SMGF Inquiry Post.");
            var inquiryResponse = JsonConvert.DeserializeObject<SMGFInquiryResponse>(jsonResponse);
            string[] invalidErrorCode = { "09", "12" };
            if (!inquiryResponse.ResponseCode.Equals(SMGFConstants.Success))
            {
                if (invalidErrorCode.Contains(inquiryResponse.ResponseCode))
                {
                    return Utils.getErrorRes("99", string.Empty);
                }
                return Utils.getErrorRes(inquiryResponse.ResponseCode, inquiryResponse.ResponseDescription);
            }
            var manualHashValue = Utils.generateHashValue(inquiryResponse.ContractNumber + inquiryResponse.ContractDescription + inquiryResponse.ResponseCode + inquiryResponse.ResponseDescription + inquiryResponse.CustomerName + inquiryResponse.Amount + inquiryResponse.DueDate, SMGFConstants.SecretKey).ToLower();
            if (!manualHashValue.Equals(inquiryResponse.HashValue))
            {
                Utils.WriteLog_Biller(messageId + "HashValue Mismatched between manualHashValue and responseHashValue!!!");
                Utils.WriteLog_Biller(messageId + "ManualHashValue : " + manualHashValue  + " | ResponseHashValue : " + inquiryResponse.HashValue);
                return Utils.getErrorRes("99", string.Empty);
            }
            var responseXML = PopulateInquiryResponseXML(responseModel, inquiryResponse, servicePercent, serviceFlatFee);
            Utils.WriteLog_Biller(messageId + "SMGF Inquiry Response : " + responseXML);
            Utils.WriteLog_Biller(messageId + "End SMGF Inquiry.");
            return responseXML;
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(messageId + "Exception occurred in SMGF Inquiry Method : " + ex.Message);
            return Utils.getErrorRes("99", string.Empty);
        }
    }

    private string PopulateInquiryRequest(SMGFInquiryResponseXML responseModel, string messageId)
    {
        Utils.WriteLog_Biller(messageId + "Start Populating SMGF Inquiry Request.");
        var request = new SMGFInquiryRequest 
        { 
            ContractNumber = responseModel.Ref1,
            TimeStamp = DateTime.Now.ToString("yyyyMMddhhmmssffff"),
            AgentCode = SMGFConstants.AgentCode,
            PaymentType = responseModel.Ref5,
            HashValue = GetHashValue(responseModel.Ref1)
        };
        var jsonString = JsonConvert.SerializeObject(request);
        Utils.WriteLog_Biller(messageId + "End Populating SMGF Inquiry Request.");
        return jsonString;
    }

    private string GetHashValue(string contractNumber)
    {
        var result = Utils.generateHashValue(contractNumber + DateTime.Now.ToString("yyyyMMddhhmmssffff") + SMGFConstants.AgentCode, SMGFConstants.SecretKey).ToLower();
        return result;
    }

    private string PopulateInquiryResponseXML(SMGFInquiryResponseXML responseModel, SMGFInquiryResponse response, string servicePercent, string serviceFlatFee)
    {
        var serviceFeeDbl = Utils.getFee(double.Parse(response.Amount), float.Parse(servicePercent), double.Parse(serviceFlatFee));
        var serviceFee = serviceFeeDbl.ToString("###0.00"); 
        responseModel.Ref2 = HttpUtility.HtmlEncode(response.CustomerName);
        responseModel.Ref3 = HttpUtility.HtmlEncode(response.ContractDescription);
        responseModel.Ref4 = response.DueDate;
        responseModel.Ref5 = responseModel.Ref5;
        responseModel.Ref3Name = "Description";
        responseModel.Ref4Name = "DueDate";
        responseModel.Ref5Name = string.Empty;
        responseModel.Amount = response.Amount;
        responseModel.Expiry = response.DueDate;
        responseModel.AgentFee = serviceFee;
        var resultXMLString = (new XMLSerializationService<SMGFInquiryResponseXML>()).SerializeData(responseModel);
        return resultXMLString;
    }

    #endregion

    #region Confirm
    public string Confirm(ConfirmResponseModel responseModel, ResponseInfo responseInfo, string amount)
    {
        var messageId = responseModel.messageid + " | ";
        try
        {
            var jsonRequest = PopulateConfirmRequest(responseModel, amount);
            Utils.WriteLog_Biller(messageId + "Start Calling SMGF Confirm Post.");
            Utils.WriteLog_Biller(messageId + "SMGF Confirm Json Request : " + jsonRequest);
            var jsonResponse = Post(SMGFConstants.ConfirmUrl, jsonRequest);
            Utils.WriteLog_Biller(messageId + "SMGF Confirm Json Response : " + jsonResponse);
            Utils.WriteLog_Biller(messageId + "End Calling SMGF Confirm Post.");
            var confirmResponse = JsonConvert.DeserializeObject<SMGFConfirmResponse>(jsonResponse);
            string[] invalidErrorCode = {"09", "12"};
            if (!confirmResponse.ResponseCode.Equals(SMGFConstants.Success))
            {
                if (invalidErrorCode.Contains(confirmResponse.ResponseCode))
                {
                    confirmResponse.ResponseCode = "99";
                }
                return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(confirmResponse.ResponseCode, confirmResponse.ResponseCodeName,
                responseInfo.txnID, confirmResponse.ResponseDescription, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
            }
            var manualHashValue = Utils.generateHashValue(confirmResponse.TransactionRefNumber + confirmResponse.ResponseCode + confirmResponse.ResponseDescription + confirmResponse.InvoiceNumber, SMGFConstants.SecretKey).ToLower();
            if (!manualHashValue.Equals(confirmResponse.HashValue))
            {
                Utils.WriteLog_Biller(messageId + "HashValue Mismatched between manualHashValue and responseHashValue!!!");
                Utils.WriteLog_Biller(messageId + "ManualHashValue : " + manualHashValue + " | ResponseHashValue : " + confirmResponse.HashValue);
                return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance("99", confirmResponse.ResponseCodeName,
                responseInfo.txnID, confirmResponse.ResponseDescription, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
            }
            var batchID = 0;
            var errMsg = string.Empty;
            responseModel.ref5 = responseModel.ref3;
            responseModel.ref3 = confirmResponse.InvoiceNumber;
            if (!UpdateTransactionStatus(responseModel, responseInfo, out errMsg, out batchID))
            {
                return UpdateErrorStatus(responseInfo.txnID, errMsg, messageId);
            }
            Utils.WriteLog_Biller(messageId + "After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString() + "| smsStatus:" + responseInfo.smsStatus);

            string smsMsg = string.Empty;
            SendSMS(responseInfo, responseModel, confirmResponse, amount, out smsMsg, messageId);

            responseModel.rescode = "00";
            responseModel.resdesc = "Success";
            responseModel.smsMsg = smsMsg;
            Utils.WriteLog_Biller(messageId + "End SMGF Confirm.");
            return Utils.getConfirmRes(responseModel);
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(messageId + "Exception occurred in SMGF Confirm Method : " + ex.Message);
            return Utils.getErrorRes("99", string.Empty);
        }      
    }

    private string PopulateConfirmRequest(ConfirmResponseModel responseModel, string amount)
    {
        var messageId = responseModel.messageid + " | ";
        Utils.WriteLog_Biller(messageId + "Start Populating SMGF Confirm Request.");
        var request = new SMGFConfirmRequest 
        {
            TransactionRefNumber = responseModel.txnID,
            ContractNumber = responseModel.ref1,
            Amount = amount,
            MobileNumber = responseModel.ref3,
            AgentCode = SMGFConstants.AgentCode,
            PaymentType = responseModel.ref5,
            HashValue = Utils.generateHashValue(responseModel.txnID + responseModel.ref1 + amount + responseModel.ref3 + SMGFConstants.AgentCode , SMGFConstants.SecretKey).ToLower()
        };
        var jsonString = JsonConvert.SerializeObject(request);
        Utils.WriteLog_Biller(messageId + "End Populating SMGF Confirm Request.");
        return jsonString;
    }

    private bool UpdateTransactionStatus(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, out string errMsg, out int batchID)
    {
        return _desktopApiWcf.ConfirmUpdate(responseInfo.txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                    confirmResponseModel.ref5, "", "PA", "Paid Successfully", responseInfo.agentID, confirmResponseModel.email, responseInfo.agentAmount, responseInfo.agentFeeDbl,
                    responseInfo.isAgreement, responseInfo.smsStatus, responseInfo.availablebalance, out errMsg, out batchID);
    }

    private void SendSMS(ResponseInfo responseInfo, ConfirmResponseModel confirmResponseModel, SMGFConfirmResponse detail, string amount, out string smsMsg, string messageId)
    {
        smsMsg = string.Empty;
        if (responseInfo.appType == "CS" || responseInfo.appType == "MS")
        {
            Utils.WriteLog_Biller(messageId + "appType is CS or MS");
            if (string.IsNullOrEmpty(responseInfo.topupType) || responseInfo.topupType == "S") //topup type is null or S
            {
                Utils.WriteLog_Biller(messageId + "topupType is null or Not S");

                SMSHelper smsH = new SMSHelper();
                smsMsg = smsH.getMessageBiller(responseInfo.agentName, responseInfo.MapTaxID, confirmResponseModel.billername, "Contract No", "Name", "Receipt No", "Ref", confirmResponseModel.ref1, confirmResponseModel.ref2, detail.InvoiceNumber, responseInfo.txnID.ToString(), double.Parse(amount).ToString("#,##0.00"), responseInfo.serviceFee, double.Parse(responseInfo.totalAmount).ToString("#,##0.00"), responseInfo.branchCode);
                try
                {
                    Utils.WriteLog_Biller(messageId + "Mobile No :" + confirmResponseModel.ref5 + "| Message :" + smsMsg + "| Sender Name :" + responseInfo.sendername + "|txn ID :" + responseInfo.txnID);
                    smsWcf.SendSms(responseInfo.txnID.ToString(), smsMsg, confirmResponseModel.ref5, responseInfo.sendername);
                    Utils.WriteLog_Biller(messageId + "sendSMSWithTxnID ends.");
                }
                catch (Exception ex)
                {
                    Utils.WriteLog_Biller(string.Format("{0}Error in SendSms: {1}", messageId, ex.ToString()));
                }
            }
        }
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
    #endregion

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