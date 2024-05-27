using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for AYAPayCashInManager
/// </summary>
public class AYAPayCashInManager : IeService
{
    private readonly A2AAPIWCF.ServiceClient _desktopApiWcf;
    private readonly MobileAPIWCFManager _mobileApiWdcf;
    private readonly MerchantCredentials merchantCredentials;
    private string accessToken = string.Empty;
    private string userToken = string.Empty;
	public AYAPayCashInManager()
	{
        _desktopApiWcf = new A2AAPIWCF.ServiceClient();
        _mobileApiWdcf = new MobileAPIWCFManager();
        merchantCredentials = GetMerchantCredentials();
	}

    public string Inquiry(inquiryModel model)
    {
        var messageId = model.messageid + " | ";
        var resultInquiryXMLString = string.Empty;
        try 
        {
            Utils.WriteLog_Biller(messageId + "Start AyaPayCashIn Inquiry!");
            var createTransactionRequest = PrepareCreateRequest(model);

            Utils.WriteLog_Biller(messageId + "AyaPayCashIn Inquiry Request : " + createTransactionRequest);
            var responseString = PostAPI(createTransactionRequest, AyaPayCashInConsts.EnquiryUrl);
            Utils.WriteLog_Biller(messageId + "AyaPayCashIn Inquiry Response : " + responseString);

            var baseResponse = JsonConvert.DeserializeObject<BaseResponse>(responseString.ToString());
            if (!baseResponse.Err.Equals(AyaPayCashInConsts.SuccessCode)) return Utils.getErrorRes(baseResponse.Err, baseResponse.Message);

            var createTransactionResponse = JsonConvert.DeserializeObject<CashInResponse<CreateTransactionResponse>>(responseString.ToString());

            resultInquiryXMLString = PrepareXMLInquiryResponse(createTransactionResponse, model);
            Utils.WriteLog_Biller(messageId + "End AyaPayCashIn Inquiry!");
            return resultInquiryXMLString;
        }
        catch(Exception ex)
        {
            Utils.WriteLog_Biller(messageId + "Exception error occurred in AyaPayCashIn Inquiry : " + ex.Message);
            return Utils.getErrorRes("99", string.Empty);
        }
    }

    private string PrepareCreateRequest(inquiryModel model)
    {
        var request = new CreateTrasactionRequest() 
        {
            RECEIVERPHONE = model.ref1,
            RECEIVERCLIENT = AyaPayCashInConsts.Customer,
            SENDERPHONE = merchantCredentials.Phone,
            SENDERCLIENT = AyaPayCashInConsts.Merchant,
            AMOUNT = Convert.ToInt32(model.amount),
            CURRENCY = AyaPayCashInConsts.Currency,
            MessageType = AyaPayCashInConsts.MessageType,
            SERVICEID = AyaPayCashInConsts.ServiceId,
            TRANSREF = GetUniqueTxnRefDigit(10)
        };
        return JsonConvert.SerializeObject(request);
    }

    private string PrepareXMLInquiryResponse(CashInResponse<CreateTransactionResponse> createTransactionResponse, inquiryModel model)
    {
        var response = new UtilsInquiryResponse() 
        {
            Version = "1.0",
            TimeStamp = DateTime.Now.ToString("yyyyMMddhhmmssffff"),
            ResCode = "00",
            ResDesc = "Success",
            TaxID = model.taxID,
            Ref1 = model.ref1,
            Ref2 = createTransactionResponse.data.TransRefId,
            Ref3 = string.Empty,
            Ref4 = string.Empty,
            Ref5 = string.Empty,
            Ref1Name = model.ref1Name,
            Ref2Name = string.Empty,
            Ref3Name = string.Empty,
            Ref4Name = string.Empty,
            Ref5Name = string.Empty,
            Amount = model.amount,
            AgentFee = model.serviceFee,
            Status = model.status,
            Expiry = model.expiry,
            ProductDesc = model.productDescription,
            ImageURL = model.imgUrl
        };
        var resultXMLString = (new XMLSerializationService<UtilsInquiryResponse>()).SerializeData(response);
        return resultXMLString;
    }

    public string Confirm(ConfirmResponseModel model, ResponseInfo responseInfo)
    {
        var messageId = model.messageid + " | ";
        var resutlConfirmXMLString = string.Empty;
        var errMsg = string.Empty;
        var batchID = default(int);
        try 
        {
            Utils.WriteLog_Biller(messageId + "Start AyaPayCashIn Confirm!");
            var executeTransactionRequest = PrepareExecuteRequest(model);

            Utils.WriteLog_Biller(messageId + "AyaPayCashIn Confirm Request : " + executeTransactionRequest);
            var responseString = PostAPI(executeTransactionRequest, AyaPayCashInConsts.ConfirmUrl);
            Utils.WriteLog_Biller(messageId + "AyaPayCashIn Confirm Response : " + responseString);

            var baseResponse = JsonConvert.DeserializeObject<BaseResponse>(responseString.ToString());
            if (!baseResponse.Err.Equals(AyaPayCashInConsts.SuccessCode))
            {
                return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(baseResponse.Err, baseResponse.Message,
                        responseInfo.txnID, baseResponse.Message, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
            }
            var createTransactionResponse = JsonConvert.DeserializeObject<CashInResponse<ExecuteTransactionResponse>>(responseString.ToString());

            if (!UpdateTransactionStatus(model, responseInfo, out errMsg, out batchID))
            {
                return UpdateErrorStatus(responseInfo.txnID, errMsg, model.messageid);
            }

            resutlConfirmXMLString = PrepareXMLConfirmResponse(createTransactionResponse, model, responseInfo);
            Utils.WriteLog_Biller(messageId + "End AyaPayCashIn Confirm!");
            return resutlConfirmXMLString;

        }
        catch(Exception ex)
        {
            Utils.WriteLog_Biller(messageId + "Exception error occurred in AyaPayCashIn Confirm : TxnId : " + model.txnID.ToString() + ex.Message);
            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance("99", string.Empty,
            responseInfo.txnID, string.Empty, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }
    }

    private bool UpdateTransactionStatus(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, out string errMsg, out int batchID)
    {
        return _desktopApiWcf.ConfirmUpdate(responseInfo.txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                    confirmResponseModel.ref5, "", "PA", "Paid Successfully", responseInfo.agentID, confirmResponseModel.email, responseInfo.agentAmount, responseInfo.agentFeeDbl,
                    responseInfo.isAgreement, responseInfo.smsStatus, responseInfo.availablebalance, out errMsg, out batchID);
    }

    private string UpdateErrorStatus(long txnID, string errMsg, string messageId)
    {
        Utils.WriteLog_Biller(messageId + "Error in ConfirmUpdate : AyaPayCashIn txnId : " + txnID.ToString() + errMsg);
        string responseDescription = "Error in update database";
        string responseCode = "06";
        if (!_desktopApiWcf.updateError(txnID, "ER", responseDescription, out errMsg))
        {
            Utils.WriteLog_Biller(messageId + "Error in updateError : AyaPayCashIn txnId : " + txnID.ToString() + errMsg);
        }
        return Utils.getErrorRes(responseCode, "Transaction fail");
    }

    public string PrepareExecuteRequest(ConfirmResponseModel model)
    {
        var request = new ExecuteTransactionRequest() 
        {
            MessageType = AyaPayCashInConsts.MessageType,
            TransRefId = model.ref2,
            Pin = string.Empty,
            Otp = string.Empty
        };
        return JsonConvert.SerializeObject(request);
    }

    private string PrepareXMLConfirmResponse(CashInResponse<ExecuteTransactionResponse> executeTransactionResponse, ConfirmResponseModel model, ResponseInfo responseInfo)
    {
        var response = new UtilsConfirmResponse()
        {
            Version = "1.0",
            TimeStamp = DateTime.Now.ToString("yyyyMMddhhmmssffff"),
            Email = model.email,
            TaxID = model.taxID,
            ResCode = "00",
            ResDesc = "Success",
            Ref1 = model.ref1,
            Ref2 = string.Empty,
            Ref3 = string.Empty,
            Ref4 = string.Empty,
            Ref5 = string.Empty,
            Ref1Name = model.ref1Name,
            Ref2Name = string.Empty,
            Ref3Name = string.Empty,
            Ref4Name = string.Empty,
            Ref5Name = string.Empty,
            BatchID = model.batchID,
            Balance = responseInfo.availablebalance.ToString(),
            TxnID = model.txnID.ToString(),
            TodayTxnCount = model.TodayTxnCount,
            TodayTxnAmount = model.TodayTxnAmount,
            SMS = string.Empty
        };
        var resultXMLString = (new XMLSerializationService<UtilsConfirmResponse>()).SerializeData(response);
        return resultXMLString;
    }

    private string PostAPI(string jsonRequest, string url)
    {
        var tokens = GenerateToken();
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Token", "Bearer " + tokens.Item1);
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokens.Item2);
        var content = new StringContent(jsonRequest);
        var httpResponse = client.PostAsync(url, content).Result;
        string responseString = httpResponse.Content.ReadAsStringAsync().Result;
        return responseString;
    }

    public string GetAccessToken()
    {
        Utils.WriteLog_Biller("Start AyaPayCashIn GetAccessToken");
        try
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + AyaPayCashInConsts.AuthCode);
            var payload = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"}
            };
            var httpResponse = client.PostAsync(AyaPayCashInConsts.AccessTokenUrl, new FormUrlEncodedContent(payload)).Result;
            string responseString = httpResponse.Content.ReadAsStringAsync().Result;
            Utils.WriteLog_Biller("AyaPayCashIn GetAccessToken Response : " + responseString);

            AccessTokenResponse result = null;
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<AccessTokenResponse>(responseString.ToString());
            }
            accessToken = result.Access_Token;
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller("Exception error occurred in AyaPayCashIn GetAccessToken : " + ex.Message);
        }
        Utils.WriteLog_Biller("End AyaPayCashIn GetAccessToken");
        return accessToken;
    }

    public string GetUserToken()
    {
        Utils.WriteLog_Biller("Start AyaPayCashIn GetUserToken");
        var userToken = string.Empty;
        try 
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Token", "Bearer " + accessToken);
            var payload = new Dictionary<string, string>
            {
                {"phone", merchantCredentials.Phone},
                {"password", merchantCredentials.Password}
            };
            var httpResponse = client.PostAsync(AyaPayCashInConsts.MerchantLoginUrl, new FormUrlEncodedContent(payload)).Result;
            string responseString = httpResponse.Content.ReadAsStringAsync().Result;

            Utils.WriteLog_Biller("AyaPayCashIn GetUserToken Response : " + responseString.ToString());

            UserTokenResponse result = null;
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<UserTokenResponse>(responseString.ToString());

            }
            userToken = result.token.Token;
        }
        catch(Exception ex)
        {
            Utils.WriteLog_Biller("Exception error occurred in AyaPayCashIn GetUserToken : " + ex.Message);
        }
        Utils.WriteLog_Biller("End AyaPayCashIn GetUserToken");
        return userToken;
    }

    public Tuple<string, string> GenerateToken()
    {
        accessToken = GetAccessToken();
        userToken = GetUserToken();
        var tokens = new Tuple<string, string>(accessToken, userToken);
        return tokens;
    }

    private MerchantCredentials GetMerchantCredentials()
    {
        var merchantCredentials = new MerchantCredentials();
        merchantCredentials.Phone = AyaPayCashInConsts.MerchantCredentials.Split(':')[0];
        merchantCredentials.Password = AyaPayCashInConsts.MerchantCredentials.Split(':')[1];
        return merchantCredentials;
    }

    public static string GetUniqueTxnRefDigit(int maxSize)
    {
        var prefix = GetTxnRefNoPrefix();
        char[] chars = new char[36];
        chars = "1234567890".ToCharArray();
        byte[] data = new byte[1];
        using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
        }
        StringBuilder result = new StringBuilder(maxSize);

        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length)]);
        }
        return prefix + result.ToString();
    }

    private static string GetTxnRefNoPrefix()
    {
        string invoiceNumberprefix = string.Empty;
        var currentDate = DateTime.UtcNow;

        string currentYear = currentDate.Year.ToString();

        string currentMonth = currentDate.ToString("MM");
        string currentDay = currentDate.ToString("dd");
        string currentHour = currentDate.ToString("hh");
        string currentMinute = currentDate.ToString("mm");
        string currentSecond = currentDate.ToString("ss");
        string currentMilliSecond = currentDate.ToString("fff");
        invoiceNumberprefix = currentYear + currentMonth + currentDay + currentHour + currentMinute + currentSecond + currentMilliSecond;

        return invoiceNumberprefix;
    }
}