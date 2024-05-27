using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for _663Result
/// </summary>

public class Response663
{
    public bool Result { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
    public string AgentCode { get; set; }
    public string AgentName { get; set; }
    public string Amount { get; set; }
    public string RefID { get; set; }
    public string responseCts { get; set; }
    public string RequestCts { get; set; }
    public string ClientType { get; set; }
    public string TransType { get; set; }
    public string ExterResponse { get; set; }
}

public class Request663
{
    public string transactionType { get; set; }
    public string agentMobileNo { get; set; }
    public string receiverMobileNo { get; set; }
    public string txnAmount { get; set; }
    public string oTp { get; set; }
    public string txnID { get; set; }
}

public class InquiryRequest
{
    public string Token { get; set; }
    public string RegisteredMobileNo { get; set; }
    public string Channel { get; set; }

    public InquiryRequest(string token, string registeredMobileNo, string channel)
    {
        Token = token;
        RegisteredMobileNo = registeredMobileNo;
        Channel = channel;
    }
}

public class InquiryEbaResponse
{
    public string RegisteredMobileNo { get; set; }
    public string CustomerID { get; set; }
    public string CustomerName { get; set; }
    public string PlanType { get; set; }
    public string Plans { get; set; }
    public List<Plans> Packages { get; set; }
}

public class InquiryMptDataPackageResquest
{
    public string Token { get; set; }
    public string PartnerCode { get; set; }
    public string BillerCode { get; set; }
    public string Detail { get; set; }

    public InquiryMptDataPackageResquest(string token, string partnerCode, string billerCode, string detail)
    {
        Token = token;
        PartnerCode = partnerCode;
        BillerCode = billerCode;
        Detail = detail;
    }
}

public class ConfirmMptDataPackageResquest
{
    public string Token { get; set; }
    public string PartnerCode { get; set; }
    public string PartnerRefNo { get; set; }
    public string BillerCode { get; set; }
    public string TransactionAmount { get; set; }
    public string Detail { get; set; }

    public ConfirmMptDataPackageResquest(string token, string partnerCode, string partnerRefNo, string billerCode, double transactionAmount, string detail)
    {
        Token = token;
        PartnerCode = partnerCode;
        PartnerRefNo = partnerRefNo;
        BillerCode = billerCode;
        TransactionAmount = transactionAmount.ToString("0.0");
        Detail = detail;
    }
}

public class InquiryMptDataPackageResponse
{
    public string TransactionStatus { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public double PartnerAmount { get; set; }
    public double TransactionAmount { get; set; }
    public string Detail { get; set; }
}

public class ConfirmMptDataPackageResponse
{
    public string TransactionStatus { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string Detail { get; set; }
    public string EBARefNo { get; set; }
    public string PartnerRefNo { get; set; }
    public double PartnerAmount { get; set; }
    public double TransactionAmount { get; set; }
}

public class Data
{
    public string DataPack { get; set; }
}

public class Packagelists
{
    public List<packageList> PackageLists { get; set; }
}

public class OoredooPackages
{
    public string offerID { get; set; }
    public string offerName { get; set; }
    public string validity { get; set; }
    public string price { get; set; }
}

public class BillerProduct
{
    public string code { get; set; }
    public string description { get; set; }
    public string billingAmount { get; set; }
    public string Transactionamount { get; set; }
    public string PartnerAmount { get; set; }
}

public class ProductEnquiryResponse
{
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string BillerCategory { get; set; }
    public string BillerCode { get; set; }
    public string BillerName { get; set; }
    public string BillerCurrency { get; set; }
    public string PartnerCurrency { get; set; }
    public string TransactionCurrency { get; set; }
    public List<BillerProduct> billerProduct { get; set; }
}

public class packageList 
{
    public string packageCode { get; set; }
    public string packageName { get; set; }
    public double price { get; set; }
}

public class Plans
{
    public string devices { get; set; }
    public dynamic expired_on { get; set; }
    public List<packs> Packs { get; set; }
}

public class packs
{
    public string price { get; set; }
    public string description { get; set; }
    public string product_id { get; set; }
}

public class ConfirmRequest
{
    public string Token { get; set; }
    public string Channel { get; set; }
    public string ChannelRefId { get; set; }
    public string CustomerId { get; set; }
    public string Amount { get; set; }
    public string Devices { get; set; }

    public ConfirmRequest(string token, string channel, string channelRefID, string customerID, string amount, string devices)
    {
        Token = token;
        Channel = channel;
        ChannelRefId = channelRefID;
        CustomerId = customerID;
        Amount = amount;
        Devices = devices;
    }
}

public class ConfirmResponse
{
    public string CustomerID { get; set; }
    public string Devices { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal serviceFee { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
}

public class GetPackageRequest
{
    public string Token { get; set; }
    public string PartnerCode { get; set; }
    public string BillerCode { get; set; }
    public string Detail { get; set; }

    public GetPackageRequest(string token, string partnerCode, string billerCode, string detail)
    {
        Token = token;
        PartnerCode = partnerCode;
        BillerCode = billerCode;
        Detail = detail;
    }
}

public class GetPackageResponse
{
    public string TransactionStatus { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public double PartnerAmount { get; set; }
    public double TransactionAmount { get; set; }
    public string Detail { get; set; }
}

public class ConfirmPackageResquest
{
    public string Token { get; set; }
    public string PartnerCode { get; set; }
    public string PartnerRefNo { get; set; }
    public string BillerCode { get; set; }
    public double TransactionAmount { get; set; }
    public string Detail { get; set; }

    public ConfirmPackageResquest(string token, string partnerCode, string partnerRefNo, string billerCode, double transactionAmount, string detail)
    {
        Token = token;
        PartnerCode = partnerCode;
        PartnerRefNo = partnerRefNo;
        BillerCode = billerCode;
        TransactionAmount = transactionAmount;
        Detail = detail;
    }
}

public class ConfirmPackageResponse
{
    public string TransactionStatus { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string Detail { get; set; }
    public string EBARefNo { get; set; }
    public string PartnerRefNo { get; set; }
    public double PartnerAmount { get; set; }
    public double TransactionAmount { get; set; }
}

public class ParamiGasPackage
{
    public string Type { get; set; }
    public string Package { get; set; }
}


public class TblTransaction
{
    public long Transactionid { get; protected set; }
    public int? Billerid { get; protected set; }
    public DateTime? Transactiondatetime { get; protected set; }
    public DateTime? Completeddatetime { get; protected set; }
    public DateTime? Updateddatetime { get; protected set; }
    public decimal? Transactionamount { get; protected set; }
    public decimal? Servicefee { get; protected set; }
    public decimal? Agentfee { get; protected set; }
    public decimal? Onestopfee { get; protected set; }
    public decimal? Agentamount { get; protected set; }
    public DateTime? Transactionexpirydatetime { get; protected set; }
    public string Productdesc { get; protected set; }
    public string Transactionstatus { get; protected set; }
    public string Statusdesc { get; protected set; }
    public string Apptypeid { get; protected set; }
    public string Transactioncurrencycode { get; protected set; }
    public string Transactioncountrycode { get; protected set; }
    public string Transactionclientip { get; protected set; }
    public string Responsecode { get; protected set; }
    public string Responsedesc { get; protected set; }
    public string Approvalcode { get; protected set; }
    public string Agentcode { get; protected set; }
    public string Agentbranchcode { get; protected set; }
    public string Channelcode { get; protected set; }
    public string Agentuserid { get; protected set; }
    public int? Batchid { get; protected set; }
    public string Servicecode { get; protected set; }
    public string Refid1 { get; protected set; }
    public string Refid2 { get; protected set; }
    public string Refid3 { get; protected set; }
    public string Refid4 { get; protected set; }
    public string Refid5 { get; protected set; }
    public string Sversion { get; protected set; }
    public string Latitude { get; protected set; }
    public string Longitude { get; protected set; }
    public decimal? Availablebalance { get; protected set; }
    public string Iscreditreset { get; protected set; }
    public string Smsstatus { get; protected set; }
    public string Messageid { get; protected set; }
    public string Deviceid { get; protected set; }
    public string Deviceinfo { get; protected set; }
    public decimal? BillerAmount { get; protected set; }
    public decimal? BillerDiscountFee { get; protected set; }
    public decimal? BillerServiceFee { get; protected set; }

    public TblTransaction(long tRansactionid, int? bIllerid, DateTime? tRansactiondatetime,
        DateTime? cOmpleteddatetime, DateTime? uPdateddatetime, decimal? tRansactionamount, decimal? sErvicefee,
        decimal? aGentfee, decimal? oNestopfee, decimal? aGentamount, DateTime? tRansactionexpirydatetime,
        string pRoductdesc, string tRansactionstatus, string sTatusdesc, string aPptypeid,
        string tRansactioncurrencycode, string tRansactioncountrycode, string tRansactionclientip,
        string rEsponsecode, string rEsponsedesc, string aPprovalcode, string aGentcode, string aGentbranchcode,
        string cHannelcode, string aGentuserid, int? bAtchid, string sErvicecode, string rEfid1, string rEfid2,
        string rEfid3, string rEfid4, string rEfid5, string sVersion, string lAtitude, string lOngitude,
        decimal? aVailablebalance, string iScreditreset, string sMsstatus, string mEssageid, string dEviceid,
        string dEviceinfo, decimal? billerAmount, decimal? billerDiscountFee, decimal? billerServiceFee)
    {
        Transactionid = tRansactionid;
        Billerid = bIllerid;
        Transactiondatetime = tRansactiondatetime;
        Completeddatetime = cOmpleteddatetime;
        Updateddatetime = uPdateddatetime;
        Transactionamount = tRansactionamount;
        Servicefee = sErvicefee;
        Agentfee = aGentfee;
        Onestopfee = oNestopfee;
        Agentamount = aGentamount;
        Transactionexpirydatetime = tRansactionexpirydatetime;
        Productdesc = pRoductdesc;
        Transactionstatus = tRansactionstatus;
        Statusdesc = sTatusdesc;
        Apptypeid = aPptypeid;
        Transactioncurrencycode = tRansactioncurrencycode;
        Transactioncountrycode = tRansactioncountrycode;
        Transactionclientip = tRansactionclientip;
        Responsecode = rEsponsecode;
        Responsedesc = rEsponsedesc;
        Approvalcode = aPprovalcode;
        Agentcode = aGentcode;
        Agentbranchcode = aGentbranchcode;
        Channelcode = cHannelcode;
        Agentuserid = aGentuserid;
        Batchid = bAtchid;
        Servicecode = sErvicecode;
        Refid1 = rEfid1;
        Refid2 = rEfid2;
        Refid3 = rEfid3;
        Refid4 = rEfid4;
        Refid5 = rEfid5;
        Sversion = sVersion;
        Latitude = lAtitude;
        Longitude = lOngitude;
        Availablebalance = aVailablebalance;
        Iscreditreset = iScreditreset;
        Smsstatus = sMsstatus;
        Messageid = mEssageid;
        Deviceid = dEviceid;
        Deviceinfo = dEviceinfo;
        BillerAmount = billerAmount;
        BillerDiscountFee = billerDiscountFee;
        BillerServiceFee = billerServiceFee;
    }
}

public class NotifyRequest
{
    public string NearMeReference { get; set; }
    public string TransactionReference { get; set; }
    public string AgentAmount { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseDesc { get; set; }
    public string ErrorMsg { get; set; }
    public string HashValue { get; set; }

    public NotifyRequest(string nearMeReference, string transactionReference, string agentAmount, string responseCode, string responseDesc, string errorMsg, string hashValue)
    {
        NearMeReference = nearMeReference;
        TransactionReference = transactionReference;
        AgentAmount = agentAmount;
        ResponseCode = responseCode;
        ResponseDesc = responseDesc;
        ErrorMsg = errorMsg;
        HashValue = hashValue;
    }
}

public class EbaFlightNotifyRequest
{
    public string Token { get; set; }
    public string PartnerCode { get; set; }
    public string BillerCode { get; set; }
    public string PartnerRefNo { get; set; }
    public string TransactionAmount { get; set; }
    public string Detail { get; set; }

    public EbaFlightNotifyRequest(string token, string partnerCode, string billerCode, string partnerRefNo, string transactionAmount, string detail)
    {
        Token = token;
        PartnerCode = partnerCode;
        BillerCode = billerCode;
        PartnerRefNo = partnerRefNo;
        TransactionAmount = transactionAmount;
        Detail = detail;
    }
}

public class FlightDetail
{
    public string BookingSuccessCode { get; set; }
    public string ContactEmail { get; set; }
    public string Route { get; set; }
    public string DepartureDate { get; set; }
    public string TicketCount { get; set; }
}

public class DetailFlight
{
    public string refCode { get; set; }
    public string refID { get; set; }
    public string amount { get; set; }
    public string contactEmail { get; set; }
    public string route { get; set; }
    public string departureDate { get; set; }
    public string ticketCount { get; set; }
    public string mobileNumber { get; set; }
}


public static class EncryptionManager
{
    public static string GetHmac(string signatureString, string secretKey)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] keyByte = encoding.GetBytes(secretKey);
        HMACSHA256 hmac = new HMACSHA256(keyByte);
        byte[] messageBytes = encoding.GetBytes(signatureString);
        byte[] hashmessage = hmac.ComputeHash(messageBytes);
        return ByteArrayToHexString(hashmessage);
    }

    private static string ByteArrayToHexString(byte[] bytes)
    {
        const string hexAlphabet = "0123456789ABCDEF";
        var result = new StringBuilder();

        foreach (var byt in bytes)
        {
            result.Append(hexAlphabet[(int)(byt >> 4)]);
            result.Append(hexAlphabet[(int)(byt & 0xF)]);
        }

        return result.ToString();
    }

    public static string ConvertFieldsToSignatureString(IList<KeyValuePair<string, object>> data)
    {
        return data.Aggregate(string.Empty, (current, field) => current + string.Format("{0}{1}{2}{3}", field.Key, "=", field.Value, "|")).TrimEnd('|');
    }
}

public class NotifyResponse
{
    public string PartnerReference { get; set; }
    public string AgentCode { get; set; }
    public string TransactionReference { get; set; }
    public string BookingUrl { get; set; }
    public string AgentAmount { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseDesc { get; set; }
    public string ErrorMsg { get; set; }
    public string HashValue { get; set; }

    public NotifyResponse(string partnerReference, string agentCode, string transactionReference, string bookingUrl,
        string agentAmount, string responseCode, string responseDesc, string errorMsg, string hashValue)
    {
        PartnerReference = partnerReference;
        AgentCode = agentCode;
        TransactionReference = transactionReference;
        BookingUrl = bookingUrl;
        AgentAmount = agentAmount;
        ResponseCode = responseCode;
        ResponseDesc = responseDesc;
        ErrorMsg = errorMsg;
        HashValue = hashValue;
    }
}




    