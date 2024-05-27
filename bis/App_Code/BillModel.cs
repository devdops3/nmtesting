using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Xml.Serialization;




public class registrationinfo
    {
        public string AgentID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string shoptype { get; set; }
        public string shopname { get; set; }
        public string mboileno { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string nrc { get; set; }
        public string secretword { get; set; }
        public string divisionid { get; set; }
        public string townshipid { get; set; }
        public string deviceID { get; set; }
        public string deviceToken { get; set; }
        public string isShop { get; set; }
        public string latitudeLoc { get; set; }
        public string longitudeLoc { get; set; }
        public string DeviceInfo { get; set; }
        
    }

    public class registrationinfoV2
    {
        public string AgentID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string shoptype { get; set; }
        public string shopname { get; set; }
        public string mboileno { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string nrc { get; set; }
        public string secretword { get; set; }
        public string divisionid { get; set; }
        public string townshipid { get; set; }
        public string deviceID { get; set; }
        public string deviceToken { get; set; }
        public string isShop { get; set; }
        public string latitudeLoc { get; set; }
        public string longitudeLoc { get; set; }
        public string DeviceInfo { get; set; }
        public string SelfiePhoto { get; set; }
        public string SelfiePhotoWithId { get; set; }

    }

    #region CNP
    public class ReqBillAmount
    {
        public string apiKey { get; set; }
        public string billRefNo { get; set; }
        public string custRefNo { get; set; }
        public string locale { get; set; }
        public string billingMerchantCode { get; set; }
        public string currencyCode { get; set; }
    }
    public class ResBillAmount
    {
        public string status { get; set; }
        public string Codescription { get; set; }
        public string Coamount { get; set; }
        public string Cocurrency { get; set; }
        public string Cdescription { get; set; }
        public string Ccode { get; set; }
        public string Camount { get; set; }
        public string Ccurrency { get; set; }
        public string totalAmount { get; set; }
        public string name { get; set; }
        public string amount { get; set; }
        public string failcode {get;set; }
        public string message { get; set; }
        public string custRefNo { get; set; }
        public string billRefNo { get; set; }
        public string paymentDueDate { get; set; }
        public string paidstatus { get; set; }
    }
    
    public class ReqPaymentByCash
    {
        public string apiKey { get; set; }
        public string billingMerchantCode { get; set; }
        public string billAmount { get; set; }
        public string chargesAmount { get; set; }
        public string currencyCode { get; set; }
        public string custRefNo { get; set; }
        public string billRefNo { get; set; }
        public string transferRefNo1 { get; set; }
        public string locale { get; set; }
        public string transferRefNo2 { get; set; }
        public string extRefNo1 { get; set; }
    }
    
    public class ResPaymentByCash
    {
        public string status { get; set; }
        public string txnId { get; set; }
        public string txnDate { get; set; }
        public string authCode { get; set; }
        public string merchantCode { get; set; }
        public string PbillAmount { get; set; }
        public string paymentType { get; set; }
        public string customerReferenceNo { get; set; }
        public string Pcurrency { get; set; }
        public string Cdescription { get; set; }
        public string Ccode { get; set; }
        public string Camount { get; set; }
        public string Ccurrency { get; set; }
        public string Codescription { get; set; }
        public string Coamount { get; set; }
        public string Cocurrency { get; set; }
        public string resbillAmount { get; set; }
        public string totalBillAmount { get; set; }
    }

    public class ResError
    {
        public string Status { get; set; }
        public string errorMessage { get; set; }
        public string errorCode { get; set; }
      
    }

    #endregion
    #region ESBA

    #region Telcopin
    public class pinRes
    {
        public string ExpiryDate { get; set; }
        public string PIN { get; set; }
        public string ResponseCode { get; set; }
        public string TransactionID { get; set; }
        public string SerialNumber { get; set; }
        public string TransactionStatus { get; set; }
    }
    public class GetTelcoPINResults
    {
        public pinRes GetTelcoPINResult { get; set; }

    }

    public class iFlixRes
    {
        public string pin { get; set; }
        public string serialNumber { get; set; }
        public string ServiceFee { get; set; }
        public string Amount { get; set; }
        public string ProductStatus { get; set; }
        public string TotalAmount { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }

    }
    public class pinReq
    {
        public string Token { get; set; }
        public string Channel { get; set; }
        public string BillerCode { get; set; }
        public string NetworkType { get; set; }
        public string PriceType { get; set; }
        public string ChannelRefID { get; set; }      

    }

    public class iflixReq
    {
        public string Token { get; set; }
        public string Channel { get; set; }       
        public string ChannelRefID { get; set; }
        public string Amount { get; set; }
        public string BillerCode { get; set; }
    }

    #endregion 
    #region GiftCard
    public class giftcardInqReq
    {
        public string Token { get; set; }
        public string Channel { get; set; }
        public string BillerCode { get; set; }
        public string PriceType { get; set; }
    }

    public class EnquiryGiftCardPINResults
    {
        public giftcardInqRes EnquiryGiftCardPINResult { get; set; }
    }

    public class GetGiftCardPINResults
    {
        public giftCardConfirmRes GetGiftCardPINResult { get; set; }
    }

public class giftCardConfirmRes
{
 public string ExpiryDate { get; set; }
        public string PIN { get; set; }
        public string ResponseCode { get; set; }
        public string TransactionID { get; set; }
        public string SerialNumber { get; set; }
        public string TransactionStatus { get; set; }
}
      

    public class giftcardInqRes
    {
        public string Amount { get; set; }
        public string ResponseCode { get; set; }
    }


    public class giftcardConfirmReq
    { 
        public string Token{get;set;}
        public string Channel{get;set;}
        public string BillerCode{get;set;}
        public string PriceType{get;set;}
        public string ChannelRefID { get; set; }
    }
    #endregion

    #region AirTime
    public class EABAirTimeTopUpReq
    {
        public string Token { get; set; }
        public string Channel { get; set; }
        public string BillerCode { get; set; }
        public string NetworkType { get; set; }
        public string ChannelRefID { get; set; }
        public string MobileNumber { get; set; }
        public string Amount { get; set; }
    }

    public class AirTimeTopUpResults
    {
        public eabAirtimeTopUpResponse AirTimeTopUpResult { get; set; }
    }

    public class eabAirtimeTopUpResponse
    {

        public int AirTimeToupTransactionID { get; set; }


        public string ChannelRefID { get; set; }


        public string MobileNumber { get; set; }


        public decimal Amount { get; set; }


        public int TransactionStatus { get; set; }


        public string TransactionDescription { get; set; }


        public int ResponseCode { get; set; }


        public string ResponseDescription { get; set; }

    }


    public class ebaAirTimeTopUpEnquiryResults
    {


        public eabAirtimeTopUpInqRes EnquiryAirtimeTopUpResult { get; set; }
    }

    public class EABAirTimeTopUpEnquiryReq
    {
        public string Token { get; set; }
        public string Channel { get; set; }
        public string ChannelRefID { get; set; }
    }

    public class eabAirtimeTopUpInqRes
    {
        public string AirtimeTransactionID { get; set; }
        public string ChannelRefID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }

    }
    #endregion

    #region viber out

    public class GiftCardAirTimeEnquiryRequest
    {
        public string Token { get; set; }
        public string Channel { get; set; }
        public string BillerCode { get; set; }
        public string CreditAmount { get; set; }
    }

  
    public class EnquiryGiftCardAirTimeResults
    {

       public GiftCardAirTimeinqRes EnquiryGiftCardAirTimeResult { get; set; }
    }

    public class GiftCardAirTimeinqRes
    {
        public string BillerCode { get; set; }

        public decimal PayAmount { get; set; }

        public string CreditAmount { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseDescription { get; set; }
    }

    public class GiftCardAirTimeConfirmRequest
    {
        public string Token { get; set; }
        public string Channel { get; set; }
        public string BillerCode { get; set; }
        public string CreditAmount { get; set; }
        public string PayAmount { get; set; }
        public string UserRefNumber { get; set; }
        public string ChannelRefNo { get; set; }
    }

    public class GiftCardAirTimeTopUpResponses
    {
        public GiftCardAirTimeTopUpRes TopUpGiftCardAirTimeResult { get; set; }
    }

    public class GiftCardAirTimeTopUpRes
    {

        public string GiftCardAirtimeTransactionID { get; set; }
        public string BillerCode { get; set; }
        public string ChannelRefNo { get; set; }
        public string UserRefNumber { get; set; }
        public string Amount { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionDescription { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }

    }


  
    
    #endregion

    #endregion
    public class confirmRes
    { 
    public string taxID{get;set;}
        public string email{get;set;}
        public string password{get;set;}
        public string messageid{get;set;}
        public string billerName{get;set;}
        public string billerlogo{get;set;}
        public string rescode{get;set;}
        public string resdecs{get;set;}
        public string ref1{get;set;}
         public string ref2{get;set;}
         public string ref3{get;set;}
         public string ref4{get;set;}
         public string ref5{get;set;}
        public string ref1Name{get;set;}
         public string ref2Name{get;set;}
         public string ref3Name{get;set;}
         public string ref4Name{get;set;}
         public string ref5Name{get;set;}
         public string batchID { get; set; }
        public string avaliableBalance{get;set;}
        public string txnID{get;set;}
        public string TodayTxnCount{get;set;}
        public string TodayTxnAmount{get;set;}
        public string smsMsg{get;set;}
    }

    #region Titan Source
    [Serializable]
    public class TitanInquiryResultSet
    {
        public string MeterNumber { get; set; }
        public string ConsumerReferenceNo { get; set; }
        public string ConsumerName { get; set; }
        public string AccountNo { get; set; }
        public string MonthName { get; set; }
        public string TotalUnitUsed { get; set; }
        public string DueDate { get; set; }
        public string TownshipCode { get; set; }
        public string TownshipName { get; set; }
        public string Amount { get; set; }
        public string BillNo { get; set; }
        public string Status { get; set; }
        public string BankName { get; set; }
        public bool IsPortal { get; set; }
        public string TransactionNumber { get; set; }
        public string ReferenceNo { get; set; }
    }

    [Serializable]
    public class TitanConfirmResultSet
    {  
        public string ResponseInfo { get; set; }
    }
    #endregion
    #region Rent 2 Own
    public class Rent2OwnInquiryRequest
    {
        public string ContractNumber { get; set; }
        public string TimeStamp { get; set; }
        public string AgentCode { get; set; }
        public string HashValue { get; set; }
        [DefaultValue("")]
        public string PaymentType { get; set; }
    }

    public class Rent2OwnInquiryResponse
    {
        public string ContractNumber { get; set; }
        public string ContractDescription { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string CustomerName { get; set; }
        public string Amount { get; set; }
        public string DueDate { get; set; }
        public string HashValue { get; set; }
    }

    public class Rent2OwnConfirmRequest
    {
        public string TransactionRefNumber { get; set; }
        public string ContractNumber { get; set; }
        public string Amount { get; set; }
        public string MobileNumber { get; set; }
        public string AgentCode { get; set; }
        [DefaultValue("")]
        public string PaymentType { get; set; }
        public string HashValue { get; set; }
    }

    public class Rent2OwnConfirmResponse
    {
        public string TransactionRefNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string InvoiceNumber { get; set; }
        public string HashValue { get; set; }
    }
    #endregion

#region 4TV
    public class fourTVResponse
    {
        public string status { get; set; }
        public string message { get; set; }

        public List<productDetailData> response { get; set; }
    }

    public class fourTVProductListRes
    {
        public fourTVProductListRes(string json)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsonObject = serializer.Deserialize<dynamic>(json);
            status = (int)jsonObject["status"];
            message = (string)jsonObject["message"];
            Dictionary<string, object> response = jsonObject["response"];
            responseDetail = getResponseDetail(response);
        }

        private Dictionary<string, productDetailData> getResponseDetail(Dictionary<string, object> response)
        {
            var result = new Dictionary<string, productDetailData>();
            foreach (var r in response)
            {
                var key = r.Key;
                var value = new productDetailData();
                var temp = (Dictionary<string, object>)r.Value;
                foreach (var v in temp)
                {
                    //products_name
                    //products_price
                    //products_unit
                    //products_unit_length
                    //products_3rdparty_id
                    var tempKey = v.Key;
                    var tempValue = v.Value;
                    if (tempKey == "products_name")
                    {
                        value.products_name = tempValue.ToString();
                    }
                    else if (tempKey == "products_price")
                    {
                        value.products_price = tempValue.ToString();
                    }
                    else if (tempKey == "products_unit")
                    {
                        value.products_unit = tempValue.ToString();
                    }
                    else if (tempKey == "products_unit_length")
                    {
                        value.products_unit_length = tempValue.ToString();
                    }
                    else if (tempKey == "products_3rdparty_id")
                    {
                        value.products_3rdparty_id = tempValue.ToString();
                    }
                }
                result.Add(key, value);
            }
            return result;
        }

        public int status { get; set; }
        public string message { get; set; }

        public Dictionary<string, productDetailData> responseDetail { get; set; }
    }

    public class fourTVViewerListRes
    {
        public fourTVViewerListRes(string json)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsonObject = serializer.Deserialize<dynamic>(json);
            status = (int)jsonObject["status"];
            message = (string)jsonObject["message"];
            Dictionary<string, object> response = jsonObject["response"];
            responseDetail = getResponseDetail(response);
        }

        private Dictionary<string, viewerDetailData> getResponseDetail(Dictionary<string, object> response)
        {
            var result = new Dictionary<string, viewerDetailData>();
            foreach (var r in response)
            {
                var key = r.Key;
                var value = new viewerDetailData();
                var temp = (Dictionary<string, object>)r.Value;
                foreach (var v in temp)
                {
                    //products_name
                    //products_price
                    //products_unit
                    //products_unit_length
                    //products_3rdparty_id
                    var tempKey = v.Key;
                    var tempValue = v.Value;

                    if (tempKey == "viewers_firstname")
                    {
                        value.viewerfirstname = (tempValue != null ? tempValue.ToString() : string.Empty);
                    }
                    else if (tempKey == "viewers_lastname")
                    {
                        value.viewerlastname = (tempValue != null ? tempValue.ToString() : string.Empty);
                    }
                    else if (tempKey == "viewers_phonenum")
                    {
                        value.viewerphone = (tempValue != null ? tempValue.ToString() : string.Empty);
                    }
                    else if (tempKey == "viewers_is_active")
                    {
                        value.vieweractive = (tempValue != null ? tempValue.ToString() : string.Empty);
                    }

                }
                result.Add(key, value);
            }
            return result;
        }

        public int status { get; set; }
        public string message { get; set; }
        public Dictionary<string, viewerDetailData> responseDetail { get; set; }
    }

    public class SolarHomeInquiryRes
    {
        public string Active { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Device_Type { get; set; }
        public string Date_Input { get; set; }
        public string Test_Account { get; set; }
        public string AccountNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
    }

    public class CanalPlusPackage
    {
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public List<Duration> Durations { get; set; }
        public List<AddOnPackage> AddOnPackage { get; set; }
    }

    public class Duration
    {
        public string Code { get; set; }
        public string Label { get; set; }
    }
    
    public class AddOnPackage : Duration
    {}

    public class viewerDetailData
    {
        public string viewerfirstname { get; set; }
        public string viewerlastname { get; set; }
        public string viewerphone { get; set; }
        public string vieweractive { get; set; }
    }
    public class productDetailData
    {

        public string products_name { get; set; }


        public string products_price { get; set; }


        public string products_unit { get; set; }


        public string products_unit_length { get; set; }


        public string products_3rdparty_id { get; set; }
    }

    public class fuorTVSubscribeRes
    {
        public int status { get; set; }
        public string message { get; set; }

        public scribeResDetail response { get; set; }
    }
    public class scribeResDetail
    {
        public DateTime expire_date { get; set; }
    }

#endregion

    #region OfflinePayent
    public class RootObject
    {
        #region Offline Payment API
        public string OfflinePaymentInquiryResult { get; set; }

        public List<OfflinePaymentInquiryResp> response { get; set; }
        #endregion
    }

    public class OfflinePaymentInquiryRequest
    {
        public string ChannelCode { get; set; }

        public string BillerCode { get; set; }

        public string BillRefNo { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? BillFromDate { get; set; }

        public DateTime? BillEndDate { get; set; }

        public string ContactName { get; set; }

        public string ContactPhone { get; set; }
        public string ChannelRefNo { get; set; }

    }

    public class OfflinePaymentInquiryResp
    {
        public string resCode { get; set; }

        public string resDesc { get; set; }

        public string IsEncrypt { get; set; }
    }

    #endregion

#region Telenor BroadBand

    public class TelenorBBReq
    {
        public string Token { get; set; }
        public string Channel { get; set; }
        public string RegisteredMobileNo { get; set; }
    }

    public class TelenorBBRes
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string RegisteredMobileNo { get; set; }
        public List<CPE> CPE { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }

    }
    public class CPE
    {
        public string IMEI { get; set; }
        public string ExpiredOn { get; set; }
    }


#endregion


public class CanalPlusPackageDetailRes
{
    public string CardNumberOrSerialNumber { get; set; }
    public string IdBase { get; set; }
    public string ContractNumber { get; set; }
    public string SubscriberNumber { get; set; }
    public string SessionID { get; set; }
    public List<CanalPlusPackage> Packages { get; set; }
}

public class CanalPlusUpgradeInquiryDetailRes
{
    public string CardNumberOrSerialNumber { get; set; }
    public string IdBase { get; set; }
    public string SubscriberNumber { get; set; }
    public string ContractNumber { get; set; }
    public string SessionID { get; set; }
    public string EndDate { get; set; }
    public string Amount { get; set; }
    public List<CanalPlusInquiryDetailPackage> Package { get; set; }
}
public class CanalPlusInquiryDetailPackage
{
    public string ProductCode { get; set; }
    public string Description { get; set; }
    public Duration Duration { get; set; }
    public AddOnPackage AddOnPackage { get; set; }
}
public class CanalPlusConfirmDetailPackage : CanalPlusInquiryDetailPackage
{
}
public class CanalPlusUpgradeConfirmDetailRes
{
    public string EndDate { get; set; }
    public string Amount { get; set; }
    public string CanalPlusRefID { get; set; }
    public List<CanalPlusConfirmDetailPackage> Package { get; set; }
}
public class Response 
{
    public string Version { get; set; }
    public string TimeStamp { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string TaxID { get; set; }
    public string Ref1 { get; set; }
    public string Ref2 { get; set; }
    public string Ref3 { get; set; }
    public string Ref4 { get; set; }
    public string Ref5 { get; set; }
    public string Ref1Name { get; set; }
    public string Ref2Name { get; set; }
    public string Ref3Name { get; set; }
    public string Ref4Name { get; set; }
    public string Ref5Name { get; set; }
    public string AgentFee { get; set; }
}

public class InquiryResponse : Response
{
    public string Amount { get; set; }
    public string Status { get; set; }
    public string ProductDesc { get; set; }
    public string BillerName { get; set; }
    public string BillerLogo { get; set; }
    public string ImageURL { get; set; }
}

public class ConfirmCanalPlusResponse : Response
{
    public string Balance { get; set; }
    public string TodayTxnCount { get; set; }
    public string TodayTxnAmount { get; set; }
    public string SMS { get; set; }
}
[XmlRoot(ElementName = "InquiryRes")] 
public class CanalPlusRenewalInquiryResponse : InquiryResponse
{
    public string SessionID { get; set; }
    public string Expiry { get; set; }
}

[XmlRoot(ElementName = "InquiryRes")] 
public class CanalPlusUpgradeInquiryResponse : InquiryResponse
{
    public string PackageCode { get; set; }
    public string DurationCode { get; set; }
    public string TotalAmount { get; set; }
    public string SessionID { get; set; }
}

[XmlRoot(ElementName = "ConfirmRes")]
public class CanlaPlusUpgradeConfirmResponse : ConfirmCanalPlusResponse
{
    public string TxnID { get; set; }
}
