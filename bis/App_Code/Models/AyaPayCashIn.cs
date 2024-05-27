using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AyaPayCashIn
/// </summary>
public class AccessTokenResponse
{
    [JsonProperty("access_token")]
    public string Access_Token { set; get; }
    [JsonProperty("scope")]
    public string Scope { set; get; }
    [JsonProperty("token_type")]
    public string Token_Type { set; get; }
    [JsonProperty("expires_in")]
    public string Expires_In { set; get; }
}
public class MerchantCredentials
{
    public string Phone { get; set; }
    public string Password { get; set; }
}

public class BaseResponse
{
    [JsonProperty("err")]
    public string Err { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }
}

public class UserToken
{
    [JsonProperty("token")]
    public string Token { get; set; }
    [JsonProperty("expiredAt")]
    public string ExpiredAt { get; set; }
}

public class UserTokenResponse : BaseResponse
{
    public UserToken token { get; set; }
}

public class CreateTrasactionRequest
{
    public string RECEIVERPHONE { get; set; }
    public string RECEIVERCLIENT { get; set; }
    public string SENDERPHONE { get; set; }
    public string SENDERCLIENT { get; set; }
    public int AMOUNT { get; set; }
    public string CURRENCY { get; set; }
    public string MessageType { get; set; }
    public string SERVICEID { get; set; }
    public string TRANSREF { get; set; }
}

public class CreateTransactionResponse
{
    [JsonProperty("errorCode")]
    public long ErrorCode { get; set; }
    [JsonProperty("errorMsg")]
    public string ErrorMsg { get; set; }
    [JsonProperty("AMOUNT")]
    public string Amount { get; set; }
    [JsonProperty("TOTALAMOUNT")]
    public string TotalAmount { get; set; }
    [JsonProperty("TRANSREFID")]
    public string TransRefId { get; set; }
    [JsonProperty("TOTALFEE")]
    public string TotalFee { get; set; }
    //public string Fees { get; set; }
    [JsonProperty("TOTALDISCOUNT")]
    public string TotalDiscount { get; set; }
    //public string Discounts { get; set; }
    [JsonProperty("DEBITFEE")]
    public string DebitFee { get; set; }
    [JsonProperty("CREDITFEE")]
    public string CreditFee { get; set; }
    [JsonProperty("CREDITDISCOUNT")]
    public string CreditDiscount { get; set; }
    [JsonProperty("DEBITDISCOUNT")]
    public string DebitDiscount { get; set; }
}

public class ExecuteTransactionRequest
{
    public string MessageType { get; set; }
    [JsonProperty("TRANSREFID")]
    public string TransRefId { get; set; }
    [JsonProperty("PIN")]
    public string Pin { get; set; }
    [JsonProperty("OTP")]
    public string Otp { get; set; }
}

public class ExecuteTransactionResponse
{
    [JsonProperty("errorCode")]
    public long ErrorCode { get; set; }
    [JsonProperty("errorMsg")]
    public string ErrorMsg { get; set; }
    [JsonProperty("transaction")]
    public ExecuteTransactionResponseData Transaction { get; set; }
}

public class ExecuteTransactionResponseData
{
    public string code { get; set; }
    public string name { get; set; }
    public string transRefId { get; set; }
    public string desc { get; set; }
    public string origAmount { get; set; }
    public string amount { get; set; }
    public string currency { get; set; }
}

public class CashInResponse<T> : BaseResponse
{
    public T data { get; set; }
}
