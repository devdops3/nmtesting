using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Summary description for RequestResponseModel
/// </summary>
public class ConfirmResponseModel
{
	public string version{get;set;}
    public string timestamp{get;set;}
    public string taxID{get;set;}
    public string email{get;set;}
    public string password{get;set;}
    public string messageid{get;set;}
    public string billername{get;set;}
    public string billerlogo{get;set;}
    public string billersource { get; set; }
    public string rescode{get;set;}
    public string resdesc { get; set; }
    public string ref1{get;set;}
    public string ref2{get;set;}
    public string ref3{get;set;}
    public string ref4{get;set;}
    public string ref5{get;set;}
    public string ref6 { get; set; }
    public string mobileno{get;set;}
    public string ref1Name{get;set;}
    public string ref2Name{get;set;}
    public string ref3Name{get;set;}
    public string ref4Name{get;set;}
    public string ref5Name{get;set;}
    public string entryMode { get; set; }
    public string approvalCode { get; set; }
    public string batchID{get;set;}
    public string availablebalance{get;set;}
    public string txnID{get;set;}
    public string TodayTxnCount{get;set;}
    public string TodayTxnAmount{get;set;}
    public string smsMsg{get;set;}
    public string Mid { get; set; }
    public string Tid { get; set; }
    public bool isVoidable { get; set; }
}

public class SkyNetConfirmResponseModel : ConfirmResponseModel
{
    public string PaymentType { get; set; }
}

public class inquiryResponseModel
{
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string taxID { get; set; }
    public string merchantname { get; set; }
    public string merchantlogo { get; set; }
    public string billerlogo { get; set; }
    public string billername { get; set; }
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }
    public string ref6 { get; set; }
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }
    public string ref6Name { get; set; }
    public string amount { get; set; }
    public string serviceFee { get; set; }
    public string status { get; set; }
    public string expiry { get; set; }
    public string productDescription { get; set; }
    public string imgUrl { get; set; }
}

public class SkyNetInquiryResponse : inquiryResponseModel
{
    public string PaymentType { get; set; }
}

public class inquiryResponseModelCanalPlus
{
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string taxID { get; set; }
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }    
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }    
    public string amount { get; set; }
    public string serviceFee { get; set; }
    public string status { get; set; }
    public string expiry { get; set; }
    public string productDescription { get; set; }
    public string imgUrl { get; set; }
    public string billerName { get; set; }
    public string billerLogo { get; set; }
}

public class inquiryPackagesResponseModelCanalPlus
{
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
//>>>>>>> feature/CanalPlusChangePackage
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }
    public List<CanalPlusPackage> ResponsePackage { get; set; }
    public string SessionID { get; set; }
}

public class inquiryChangePackageVerifyResponseModelCanalPlus
{
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }
    public string IDBase { get; set; }
    public string ContractNumber { get; set; }
    public List<CanalPlusPackage> Package { get; set; }
    public List<Duration> Duration { get; set; }
    public string PackageAmount { get; set; }
    public string TransactionAmount { get; set; }
    public string ServiceFee { get; set; }
    public string SessionID { get; set; }
    public string Expiry { get; set; }
    public string BillerName { get; set; }
    public string BillerLogo { get; set; }
    public string imgURL { get; set; }
    public string TaxID { get; set; }
}

public class RequestResponsePackage
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public List<Duration> Duration { get; set; }
    }

public class RequestResponseDuration
    {
        public string Code { get; set; }
        public string Label { get; set; }
    }

public class EBACanalPlusReq
{
    public string Token { get; set; }
    public string Channel { get; set; }    
    public string CardNumber { get; set; }
}

public class EBACanalPlusGetPackagesReq
{
    public string Token { get; set; }
    public string Channel { get; set; }
    public string CardNumber { get; set; }
    public string AccountRef { get; set; }
    public string eTopupDistributorID { get; set; }
    public string ProductID { get; set; }
}

public class EBACanalPlusChangePackageVerifyReq
{
    public string Token { get; set; }
    public string CardNumber { get; set; }
    public string Channel { get; set; }
    public string ChannelRefID { get; set; }
    public string IDBase { get; set; }
    public string NumSubscriber { get; set; }
    public string NumContract { get; set; }
    public string Package { get; set; }
    public string Duration { get; set; }
    public string SessionID { get; set; }
}

public class EBACanalPlusConfirmReq
{
    public string Token { get; set; }
    public string Channel { get; set; }
    public string ChannelRefID { get; set; }
    public string CardNumber { get; set; }
    public string idBase { get; set; }
    public string SubscriberNumber { get; set; }
    public string ContractNumber { get; set; }
    public string Amount { get; set; }
}

public class EBACanalPlusChangePackageConfirmReq
{
    public string Token { get; set; }
    public string CardNumber { get; set; }
    public string Channel { get; set; }
    public string ChannelRefID { get; set; }
    public string idBase { get; set; }
    public string NumSubscriber { get; set; }
    public string NumContract { get; set; }
    public string Amount { get; set; }
    public string TotalAmount { get; set; }
    public string Package { get; set; }
    public string Duration { get; set; }
    public string SessionID { get; set; }
}

public class EBACanalPlusChangePackageConfirmRes
{
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }
    public string mobileno { get; set; }
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }
    public string PackageAmount { get; set; }
    public string CanalPlusRefID { get; set; }
    public string ServiceFee { get; set; }
    public string TransactionAmount { get; set; }
    public List<CanalPlusPackage> Package { get; set; }
    public List<Duration> Duration { get; set; }
    public string txnID { get; set; }  
    public string EndDate { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
    public string TodayTxnCount { get; set; }
    public string TodayTxnAmount { get; set; }
    public string smsMsg { get; set; }
}

#region SolarHome
public class inquiryResponseModelSolarHome
{
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string taxID { get; set; }
    public string amount { get; set; }
    public string serviceFee { get; set; }
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }
    public string Active { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string TestAccount { get; set; }
    public string imgUrl { get; set; }
    public string billerName { get; set; }
    public string billerLogo { get; set; }
}

public class SolarHomeConfirmResponse
{
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public string Trans_ID { get; set; }
    public string DataTime { get; set; }
    public string Amount { get; set; }
    public string TodayTxnCount { get; set; }
    public string TodayTxnAmount { get; set; }
    public string Datetime { get; set; }
    public string Mobile { get; set; }
    public string AccountNumber { get; set; }
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }

}
#endregion

public class inquiryResponseModel123Remit
{
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string taxID { get; set; }
    public string merchantname { get; set; }
    public string merchantlogo { get; set; }
    public string billerlogo { get; set; }
    public string billername { get; set; }
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }
    public string ref6 { get; set; }
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }
    public string ref6Name { get; set; }
    public string amount { get; set; }
    public string serviceFee { get; set; }
    public string status { get; set; }
    public string expiry { get; set; }
    public string productDescription { get; set; }
    public string imgUrl { get; set; }

}

public class loginRequestModel
{
    public string version { get; set; }
    public string timeStamp { get; set; }
    public string messageid { get; set; }
    public string userid { get; set; }
    public string password { get; set; }
    public bool  Result { get; set; }
    public string rescode { get; set; }
    public string resdesc { get; set; }
}

//<RepaymentServiceFeeReq>
//<Version>1.0</Version>
//<TimeStamp>yyyyMMddhhmmssffff</TimeStamp>
//<MessageID>768866yyhhhhhh</MessageID>
//<Email>user@agent.com</Email>
//<Password>xxxxxxxxxx</Password>
//<TaxID>0000000000012</TaxID>  
//<Amount>10000</Amount> 
//</RepaymentServiceFeeReq>
//<RepaymentServiceFeeRes>
//<Version>1.0</Version>
//<TimeStamp>yyyyMMddhhmmssffff</TimeStamp>
//<MessageID>768866yyhhhhhh</MessageID>
//<ResCode>00</ResCode>
//<ResDesc></ResDesc>
//<Amount>10000</Amount>
//<ServiceFee> 200 </ServiceFee> 
//</RepaymentServiceFeeRes>

public class RepaymentServiceFeesReqMdl
{
    public string Version { get; set; }
    public string TimeStamp { get; set; }
    public string MessageID { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string TaxID { get; set; }
    public string Amount { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public bool Result { get; set; }
}

public class RepaymentServiceFeesResMdl
{
    public string Version { get; set; }
    public string TimeStamp { get; set; }
    public string MessageID { get; set; }
    public string Amount { get; set; }
    public string ServiceFee { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
   

   
}

#region Pahtama Group
public class PGPendingInvoiceRequest
{
    public string Token { get; set; }
    public string PartnerCode { get; set; }
    public string CustomerID { get; set; }
}

public class PGPendingInvoiceResponse
{
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }
    public string PartnerCode { get; set; }
    public string CustomerID { get; set; }
    public string CustomerName { get; set; }
    public List<PGInvoice> Invoices { get; set; }
    public string Amount { get; set; }
    public string ServiceFee { get; set; }
    public string ErrorCode { get; set; }
    public string FailReason { get; set; }
    public string AgentCode { get; set; }
}

public class PGInvoice
{
    public string InvoiceNumber { get; set; }
    public string BranchCode { get; set; }
    public string Address { get; set; }
    public string InvoiceDate { get; set; }
    public string Amount { get; set; }
    public string DueDate { get; set; }
    public string Remark { get; set; }
}

public class PGConfirmPendingRequest
{
    public string Token { get; set; }
    public string PartnerCode { get; set; }
    public string CustomerID { get; set; }    
    public List<PGReqConfirmInvoice> Invoices { get; set; }
}

public class PGReqConfirmInvoice
{
    public string InvoiceNumber { get; set; }
    public string PaymentReference { get; set; }
    public string Amount { get; set; }
    public string PaymentMethod { get; set; }
}

public class PGResConfirmInvoice
{
    public string InvoiceNumber { get; set; }
    public string Amount { get; set; }
    public string PaymentReference { get; set; }
    public string Status { get; set; }
}

public class PGConfirmPendingInvoiceResponse
{
    public string PartnerCode { get; set; }
    public string CustomerID { get; set; }
    public string TxnID { get; set; }
    public List<PGResConfirmInvoice> Invoices { get; set; }
    public string ErrorCode { get; set; }
    public string FailReason { get; set; }
    public string smsMsg { get; set; }
}

//THIS IS FOR CONVERTING XML TO C# OBJs

[XmlRoot(ElementName = "Invoice")]
public class Invoice
{
    [XmlElement(ElementName = "Amount")]
    public string Amount { get; set; }
    [XmlElement(ElementName = "InvoiceNumber")]
    public string InvoiceNumber { get; set; }
}

[XmlRoot(ElementName = "Invoices")]
public class Invoices
{
    [XmlElement(ElementName = "Invoice")]
    public List<Invoice> Invoice { get; set; }
}
#endregion

public class ErrorResponse
{
    public string Version { get; set; }
    public string TimeStamp { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
}

public class TelenorBBInquiryResModel
{
    public string taxID { get; set; }
    public string merchantname { get; set; }
    public string merchantlogo { get; set; }
    public string billerlogo { get; set; }
    public string billername { get; set; }   
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }
    public string amount { get; set; }
    public string serviceFee { get; set; }
    public string status { get; set; }
    public string expiry { get; set; }
    public string productDescription { get; set; }
    public string imgUrl { get; set; }
    public List<CPE> ResponseCPEList { get; set; }
}

public class TelenorBBConfirmReq
{
    public string Token { get; set; }
    public string Channel { get; set; }
    public string ChannelRefID { get; set; }
    public string CustomerID { get; set; }
    public string Amount { get; set; }
    public string IMEI { get; set; }
}

public class TelenorBBConfirmRes
{
    public string CustomerID { get; set; }
    public string IMEI { get; set; }
    public string TotalAmount { get; set; }
    public string serviceFee { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
}

public class AeonConfirmReq
{
    public string Token { get; set; }
    public string Channel { get; set; }
    public string ChannelRefId { get; set; }
    public string AgreementNo { get; set; }
    public string CustomerName { get; set; }
    public string PaidAmount { get; set; }
}

public class AeonConfirmRes
{
    public int AeonTransactionID { get; set; }
    public string AggrementNo { get; set; }
    public string CustomerName { get; set; }
    public decimal TransactionAmount { get; set; }
    public int TransactionStatus { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
}

public class FtthOrWtthInquiryResponse
{
    public string taxID { get; set; }
    public string version { get; set; }
    public string merchantname { get; set; }
    public string merchantlogo { get; set; }
    public string billerlogo { get; set; }
    public string billername { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string serviceFee { get; set; }
    public string servicePercentFee { get; set; }
    public string RegisteredMobielNo { get; set; }
    public InquiryEbaResponse InquiryDetails { get; set; }
}

public class FtthOrWtthConfirmResponse
{
    public string TransactionId { get; set; }
    public string RegisteredMobielNo { get; set; }
    public string Device { get; set; }
    public string ExpiryDate { get; set; }
    public string CustomerId { get; set; }
    public string Amount { get; set; }
    public string CustomerMobileNo { get; set; }
    public string DateTime { get; set; }
}

public class MptDataPackageInquiryResponse
{
    public string taxID { get; set; }
    public string version { get; set; }
    public string billerlogo { get; set; }
    public string billername { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string serviceFee { get; set; }
    public string servicePercentFee { get; set; }
    public InquiryMptDataPackageResponse PackageDetails { get; set; }
    public List<packageList> PackageLists { get; set; }
}

public class OoredooDataPackageInquiryResponse
{
    public string taxID { get; set; }
    public string version { get; set; }
    public string billerlogo { get; set; }
    public string billername { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string serviceFee { get; set; }
    public string servicePercentFee { get; set; }
    public InquiryMptDataPackageResponse PackageDetails { get; set; }
    public List<OoredooPackages> PackageLists { get; set; }
}

public class ParamiGasInquiryResponse
{
    public string taxID { get; set; }
    public string version { get; set; }
    public string billerlogo { get; set; }
    public string billername { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string serviceFlatFee { get; set; }
    public string servicePercentFee { get; set; }
    public List<ParamiPackageList> PackageDetails { get; set; }
}

public class ParamiPackageList
{
    public string packageCode { get; set; }
    public double price { get; set; }
}

public class ParamiData
{
    public string Package { get; set; }
}

public class EventListResponse
{
    public string taxID { get; set; }
    public string version { get; set; }
    public string billerlogo { get; set; }
    public string billername { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string serviceFee { get; set; }
    public string servicePercentFee { get; set; }
    public List<Ticket> TicketList { get; set; }
}

public class B2BCancelResponseModel
{
    public string version { get; set; }
    public string timestamp { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string taxID { get; set; }
  
}

public class inquiryModel
{
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string taxID { get; set; }
    public string merchantname { get; set; }
    public string merchantlogo { get; set; }
    public string billerlogo { get; set; }
    public string billername { get; set; }
    public string ref1 { get; set; }
    public string ref2 { get; set; }
    public string ref3 { get; set; }
    public string ref4 { get; set; }
    public string ref5 { get; set; }
    public string ref6 { get; set; }
    public string ref1Name { get; set; }
    public string ref2Name { get; set; }
    public string ref3Name { get; set; }
    public string ref4Name { get; set; }
    public string ref5Name { get; set; }
    public string ref6Name { get; set; }
    public string amount { get; set; }
    public string serviceFee { get; set; }
    public string status { get; set; }
    public string expiry { get; set; }
    public string productDescription { get; set; }
    public string imgUrl { get; set; }
    public string messageid { get; set; }
    public string servicePercent { get; set; }
    public string serviceFlatFee { get; set; }
}
