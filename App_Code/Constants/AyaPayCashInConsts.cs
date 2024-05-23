using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AyaPayCashInConsts
/// </summary>
public class AyaPayCashInConsts
{
	public AyaPayCashInConsts()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static readonly string BaseUrl = ConfigurationManager.AppSettings["AyaPayCashInBaseUrl"].ToString();
    public static readonly string AccessTokenUrl = BaseUrl + ConfigurationManager.AppSettings["AyaPayCashInAccessTokenUrl"].ToString();
    public static readonly string MerchantLoginUrl = BaseUrl + ConfigurationManager.AppSettings["AyaPayCashInMerchantLoginUrl"].ToString();
    public static readonly string EnquiryUrl = BaseUrl + ConfigurationManager.AppSettings["AyaPayCashInEnquiryUrl"].ToString();
    public static readonly string ConfirmUrl = BaseUrl + ConfigurationManager.AppSettings["AyaPayCashInConfirmUrl"].ToString();
    public static readonly string MerchantCredentials = ConfigurationManager.AppSettings["AyaPayCashInMerchantCredentials"].ToString();
    public static readonly string AuthCode = ConfigurationManager.AppSettings["AyaPayCashInAuthCode"].ToString();
    public static readonly string ServiceId = ConfigurationManager.AppSettings["AyaPayCashInServiceId"].ToString();
    public static readonly string MessageType = ConfigurationManager.AppSettings["AyaPayCashInMessageType"].ToString();
    public static readonly string Customer = "customer";
    public static readonly string Merchant = "merchant";
    public static readonly string Currency = "MMK";
    public static readonly string SuccessCode = "200";
}