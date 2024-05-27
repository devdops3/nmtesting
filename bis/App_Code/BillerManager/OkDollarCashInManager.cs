using MessagingService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

public class OkDollarCashInManager
{
    private readonly A2AAPIWCF.ServiceClient _desktopApiWcf;
    private readonly MessagingServiceClient smsWcf;

    private string baseURL = ConfigurationManager.AppSettings["OkDollarCashInBaseUrl"];
    private string endPoint;

    public OkDollarCashInManager()
    {
        _desktopApiWcf = new A2AAPIWCF.ServiceClient();
        smsWcf = new MessagingServiceClient();
    }

    public string Inquiry(OkDollarCashIn.ResponseXML responseXML, string messageId, string servicePercent, string serviceFlatFee)
    {
        string response = string.Empty;
        try
        {
            #region Authentication
            string authenticationResponseText = GetAuthenticationResposne(messageId);
            if (string.IsNullOrEmpty(authenticationResponseText))
            {
                return Utils.getErrorRes("99", response);
            }

            var authenticationResponse = JsonConvert.DeserializeObject<OkDollarCashIn.OkDollarResponse<OkDollarCashIn.AuthenticationResponse>>(authenticationResponseText);
            if (authenticationResponse.Code != (int)HttpStatusCode.OK)
            {
                if (OkDollarConstants.ResponseErrorCodes.Contains(authenticationResponse.Code))
                {
                    return Utils.getErrorRes(authenticationResponse.Code.ToString(), authenticationResponse.Message, responseXML.TaxID);
                }
                return Utils.getErrorRes("99", string.Empty);
            }
            #endregion

            #region GetInformation
            string getInformationResponseText = GetInformationResposne(authenticationResponse.Data.Token, responseXML, messageId);
            if (string.IsNullOrEmpty(getInformationResponseText))
            {
                return Utils.getErrorRes("99", response);
            }
            var getInformationResponse = JsonConvert.DeserializeObject<OkDollarCashIn.OkDollarResponse<OkDollarCashIn.GetUserInformationResponse>>(getInformationResponseText);
            if (getInformationResponse.Code != (int)HttpStatusCode.OK)
            {
                if (OkDollarConstants.ResponseErrorCodes.Contains(getInformationResponse.Code))
                {
                    return Utils.getErrorRes(getInformationResponse.Code.ToString(), getInformationResponse.Message, responseXML.TaxID);
                }
                return Utils.getErrorRes("99", string.Empty);
            }
            #endregion

            response = (new XMLSerializationService<OkDollarCashIn.ResponseXML>()).SerializeData(responseXML);
            Utils.WriteLog_Biller("InquiryRES XML : " + response);

            return response;
        }
        catch (Exception exception)
        {
            Utils.WriteLog_Biller(messageId + "Exception occurred in Ok Dollar Cash In Inquiry : " + exception.Message);
            response = Utils.getErrorRes("99", string.Empty);
        }
        finally
        {
            Utils.WriteLog_Biller("########## " + messageId + " | Ok Dollar Cash In Inquiry - End ##########");
        }
        return response;
    }

    public string Confirm(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, string amount, string messageId)
    {
        string response = string.Empty;
        try
        {
            #region Authentication
            string authenticationResponseText = GetAuthenticationResposne(messageId);
            if (string.IsNullOrEmpty(authenticationResponseText))
            {
                return Utils.getErrorRes("99", response);
            }

            var authenticationResponse = JsonConvert.DeserializeObject<OkDollarCashIn.OkDollarResponse<OkDollarCashIn.AuthenticationResponse>>(authenticationResponseText);
            if (authenticationResponse.Code != (int)HttpStatusCode.OK)
            {
                if (OkDollarConstants.ResponseErrorCodes.Contains(authenticationResponse.Code))
                {
                    return new MobileAPIWCFManager().GetErrorResponseWithAddBalance(authenticationResponse.Code.ToString(), authenticationResponse.Message, responseInfo.txnID, authenticationResponse.Message, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
                }
                return Utils.getErrorRes("99", string.Empty);
            }
            #endregion

            #region Payment
            string getPaymentResponseText = GetPaymentResponse(authenticationResponse.Data.Token, confirmResponseModel, amount, messageId);

            if (string.IsNullOrEmpty(getPaymentResponseText)) return Utils.getErrorRes("99", getPaymentResponseText);

            var getPaymentResponse = JsonConvert.DeserializeObject<OkDollarCashIn.OkDollarResponse<OkDollarCashIn.PaymentResponse>>(getPaymentResponseText);

            if (getPaymentResponse.Code != (int)HttpStatusCode.OK)
            {
                if (OkDollarConstants.ResponseErrorCodes.Contains(getPaymentResponse.Code))
                {
                    return new MobileAPIWCFManager().GetErrorResponseWithAddBalance(getPaymentResponse.Code.ToString(), getPaymentResponse.Message, responseInfo.txnID, getPaymentResponse.Message, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
                }

                #region Query
                string getQueryResponseText = GetQueryResponse(authenticationResponse.Data.Token, confirmResponseModel, amount, messageId);

                if (string.IsNullOrEmpty(getQueryResponseText)) return Utils.getErrorRes("99", getQueryResponseText);

                var getQueryResponse = JsonConvert.DeserializeObject<OkDollarCashIn.OkDollarResponse<OkDollarCashIn.PaymentResponse>>(getQueryResponseText);

                if (getQueryResponse.Code != (int)HttpStatusCode.OK)
                {
                    if (OkDollarConstants.ResponseErrorCodes.Contains(getQueryResponse.Code))
                    {
                        return new MobileAPIWCFManager().GetErrorResponseWithAddBalance(getQueryResponse.Code.ToString(), getQueryResponse.Message, responseInfo.txnID, getQueryResponse.Message, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
                    }
                    return Utils.getErrorRes("99", string.Empty);
                }
                #endregion
            }
            #endregion

            string errorMessage = string.Empty;
            int batchID = 0;

            if (!UpdateTransactionStatus(confirmResponseModel, responseInfo, out errorMessage, out batchID))
            {
                return UpdateErrorStatus(responseInfo.txnID, errorMessage, messageId);
            }

            Utils.WriteLog_Biller(messageId + "| Ok Dollar Cash In Confirm After Update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString() + "| smsStatus:" + responseInfo.smsStatus);

            confirmResponseModel.rescode = "00";
            confirmResponseModel.resdesc = "Success";

            response = Utils.getConfirmRes(confirmResponseModel);
        }
        catch (Exception exception)
        {
            Utils.WriteLog_Biller(messageId + "Exception occurred in Ok Dollar Cash In Confirm : " + exception.Message);
            return Utils.getErrorRes("99", string.Empty);
        }
        finally
        {
            Utils.WriteLog_Biller("########## " + messageId + " | Ok Dollar Cash In Confirm - End ##########");
        }

        return response;
    }

    #region Extension Method
    private string GetAuthenticationResposne(string messageId)
    {
        Utils.WriteLog_Biller("########## " + messageId + " | Ok Dollar Cash In GetAuthenticationResponse - Start ##########");
        string response = string.Empty;
        try
        {
            var request = new OkDollarCashIn.AuthenticationRequest
            {
                Username = ConfigurationManager.AppSettings["OkDollarCashInUsername"],
                Password = ConfigurationManager.AppSettings["OkDollarCashInPassword"],
                ProjectId = ConfigurationManager.AppSettings["OkDollarCashInProjectId"]
            };
            var requestJsonString = JsonConvert.SerializeObject(request);
            Utils.WriteLog_Biller(messageId + " OK Dollar Cash In GetAuthenticationResponse Request : " + requestJsonString);
            endPoint = baseURL + ConfigurationManager.AppSettings["OkDollarCashInAuthenticationUrl"];

            response = Post(endPoint, requestJsonString);
            Utils.WriteLog_Biller(messageId + " | OK Dollar Cash In GetAuthenticationResponse Resposne : " + response);
        }
        catch (Exception exception)
        {
            Utils.WriteLog_Biller(messageId + "Exception occurred in Ok Dollar Cash In GetAuthenticationResponse : " + exception.Message);
            throw exception;
        }
        finally
        {
            Utils.WriteLog_Biller("########## " + messageId + " | Ok Dollar Cash In GetAuthenticationResponse - End ##########");
        }
        return response;
    }

    private string GetInformationResposne(string token, OkDollarCashIn.ResponseXML responseXML, string messageId)
    {
        Utils.WriteLog_Biller("########## " + messageId + " | Ok Dollar Cash In GetInformationResponse - Start ##########");
        string response = string.Empty;
        try
        {
            string mobileNumber = "0095" + responseXML.Ref1.Substring(1);
            Utils.WriteLog_Biller(messageId + " OK Dollar Cash In GetInformationResponse Request Mobile Number is: " + mobileNumber);
            var request = new OkDollarCashIn.GetUserInformationRequest
            {
                MobileNumber = mobileNumber,
                Channel = ConfigurationManager.AppSettings["OkDollarBankTransactionChannel"]
            };
            var requestJsonString = JsonConvert.SerializeObject(request);
            Utils.WriteLog_Biller(messageId + " OK Dollar Cash In GetInformationResponse Request : " + requestJsonString);
            endPoint = baseURL + ConfigurationManager.AppSettings["OkDollarCashInGetUserInformationUrl"];
            response = Post(endPoint, requestJsonString, token);
            Utils.WriteLog_Biller(messageId + " | OK Dollar Cash In GetInformationResponse Resposne : " + response);
        }
        catch (Exception exception)
        {
            Utils.WriteLog_Biller(messageId + "Exception occurred in Ok Dollar Cash In GetInformationResponse : " + exception.Message);
            throw exception;
        }
        finally
        {
            Utils.WriteLog_Biller("########## " + messageId + " | Ok Dollar Cash In GetInformationResponse - End ##########");
        }
        return response;
    }

    private string GetPaymentResponse(string token, ConfirmResponseModel confirmResponseModel, string amount, string messageId)
    {
        Utils.WriteLog_Biller("########## " + messageId + " | Ok Dollar Cash In GetPaymentResponse - Start ##########");
        string response = string.Empty;
        try
        {
            var request = GetPaymentRequest(confirmResponseModel, amount);
            var requestJsonString = JsonConvert.SerializeObject(request);
            Utils.WriteLog_Biller(messageId + " OK Dollar Cash In GetPayment Request : " + requestJsonString);
            endPoint = baseURL + ConfigurationManager.AppSettings["OkDollarCashInPaymentUrl"];
            response = Post(endPoint, requestJsonString, token);
            Utils.WriteLog_Biller(messageId + " | OK Dollar Cash In GetPaymentResponse Resposne : " + response);
        }
        catch (Exception exception)
        {
            Utils.WriteLog_Biller(messageId + "Exception occurred in Ok Dollar Cash In GetPaymentResponse : " + exception.Message);
            throw exception;
        }
        finally
        {
            Utils.WriteLog_Biller("########## " + messageId + " | Ok Dollar Cash In GetPaymentResponse - End ##########");
        }
        return response;
    }

    private string GetQueryResponse(string token, ConfirmResponseModel confirmResponseModel, string amount, string messageId)
    {
        Utils.WriteLog_Biller("########## " + messageId + " | Ok Dollar Cash In GetQueryResponse - Start ##########");
        string response = string.Empty;
        try
        {
            var request = GetPaymentRequest(confirmResponseModel, amount);
            var requestJsonString = JsonConvert.SerializeObject(request);
            Utils.WriteLog_Biller(messageId + " OK Dollar Cash In GetQueryResponse Request : " + requestJsonString);
            endPoint = baseURL + ConfigurationManager.AppSettings["OkDollarCashInQueryUrl"];
            response = Post(endPoint, requestJsonString, token);
            Utils.WriteLog_Biller(messageId + " | OK Dollar Cash In GetQueryResponse Resposne : " + response);
        }
        catch (Exception exception)
        {
            Utils.WriteLog_Biller(messageId + "Exception occurred in Ok Dollar Cash In GetQueryResponse : " + exception.Message);
            throw exception;
        }
        finally
        {
            Utils.WriteLog_Biller("########## " + messageId + " | Ok Dollar Cash In GetQueryResponse - End ##########");
        }
        return response;
    }

    private OkDollarCashIn.PaymentRequest GetPaymentRequest(ConfirmResponseModel confirmResponseModel, string amount)
    {
        string mobileNumber = "0095" + confirmResponseModel.ref1.Substring(1);
        Utils.WriteLog_Biller(" OK Dollar Cash In GetPaymentRequest Mobile Number is: " + mobileNumber);
        return new OkDollarCashIn.PaymentRequest
        {
            NearmeMerchantID = ConfigurationManager.AppSettings["OkDollarCashInNearMeMerchantID"],
            TransactionId = confirmResponseModel.txnID,
            DestinationNumber = mobileNumber,
            Amount = amount,
            MobileNumber = mobileNumber,
            ProjectId = ConfigurationManager.AppSettings["OkDollarCashInProjectId"],
            Channel = int.Parse(ConfigurationManager.AppSettings["OkDollarBankTransactionChannel"])
        };
    }

    private string Post(string url, string jsonRequest, string token = null)
    {
        var result = string.Empty;
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        if (!string.IsNullOrEmpty(token))
        {
            httpWebRequest.Headers.Add("token", token);
        }

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

    private bool UpdateTransactionStatus(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, out string errorMessage, out int batchId)
    {
        return _desktopApiWcf.ConfirmUpdate(responseInfo.txnID,
                                           confirmResponseModel.ref1,
                                           confirmResponseModel.ref2,
                                           confirmResponseModel.ref3,
                                           confirmResponseModel.ref4,
                                           confirmResponseModel.ref5,
                                           "",
                                           "PA",
                                           "Paid Successfully",
                                           responseInfo.agentID,
                                           confirmResponseModel.email,
                                           responseInfo.agentAmount,
                                           responseInfo.agentFeeDbl,
                                           responseInfo.isAgreement,
                                           responseInfo.smsStatus,
                                           responseInfo.availablebalance,
                                           out errorMessage,
                                           out batchId);
    }

    private string UpdateErrorStatus(long txnID, string errorMessage, string messageId)
    {
        Utils.WriteLog_Biller(messageId + "Error occurred in Ok Dollar Cash In Confirm : " + errorMessage);
        string responseDescription = "Error occured in database update";
        string responseCode = "06";
        if (!_desktopApiWcf.updateError(txnID, "ER", responseDescription, out errorMessage))
        {
            Utils.WriteLog_Biller(messageId + "Error in updateError : " + errorMessage);
        }
        return Utils.getErrorRes(responseCode, "Transaction fail");
    }
    #endregion

}