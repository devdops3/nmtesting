using System.Collections.Generic;


#region <-- Gift Cards -->

public class GiftCardsInquiryResquest
    {
        public string Token { get; set; }
        public string PartnerCode { get; set; }
        public string BillerCode { get; set; }
        public string Detail { get; set; }

        public GiftCardsInquiryResquest(string token, string partnerCode, string billerCode, string detail)
        {
            Token = token;
            PartnerCode = partnerCode;
            BillerCode = billerCode;
            Detail = detail;
        }
    }

    public class GiftCardsConfirmResquest
    {
        public string Token { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerRefNo { get; set; }
        public string BillerCode { get; set; }
        public double TransactionAmount { get; set; }
        public string Detail { get; set; }

        public GiftCardsConfirmResquest(string token, string partnerCode, string partnerRefNo, string billerCode, double transactionAmount, string detail)
        {
            Token = token;
            PartnerCode = partnerCode;
            PartnerRefNo = partnerRefNo;
            BillerCode = billerCode;
            TransactionAmount = transactionAmount;
            Detail = detail;
        }
    }

public class OoredooFtthDetail
{
    public string CustomerBillingID { get; set; }
    public double Amount { get; set; }
    public string InvoiceNumber { get; set; }
    public string CustomerName { get; set; }

    public OoredooFtthDetail(string customerBillingID, double amount, string invoiceNumber, string customerName)
    {
        CustomerBillingID = customerBillingID;
        Amount = amount;
        InvoiceNumber = invoiceNumber;
        CustomerName = customerName;
    }
}
public class ProductEnquiryRequest
{
    public string Token { get; set; }
    public string BillerCode { get; set; }
    public string PartnerCode { get; set; }
}

public class EnquiryGiftCardRequest
{
    public string Token { get; set; }
    public string PartnerCode { get; set; }
    public string BillerCode { get; set; }
    public string Detail { get; set; }
    public EnquiryGiftCardRequest(string token, string partnerCode, string billerCode, string detail)
    {
        Token = token;
        PartnerCode = partnerCode;
        BillerCode = billerCode;
        Detail = detail;
    }
}

public class EnquiryGiftCardResponse
    {
        public string TransactionStatus { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public double PartnerAmount { get; set; }
        public double TransactionAmount { get; set; }
        public string Detail { get; set; }
    }

    public class GiftCardsInquiryResponse
    {
        public string TransactionStatus { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public double PartnerAmount { get; set; }
        public double TransactionAmount { get; set; }
        public string Detail { get; set; }
    }

    public class GiftCardsConfirmResponse
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

    public class EbaConfirmResponse
    {
        public string TransactionStatus { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Detail { get; set; }
        public string EBARefNo { get; set; }
        public string PartnerRefNo { get; set; }
        public double PartnerAmount { get; set; }
        public double TransactionAmount { get; set; }
        public double PartnerBalance { get; set; }
    }


public class Detail
{
    public string Deno { get; set; }
    public string PinId { get; set; }
    public string SerialNumber { get; set; }
    public string ClearPin { get; set; }
    public string ExpiryDate { get; set; }
}

public class Ticket
{
    public string ticketId { get; set; }
    public string eventTitle { get; set; }
    public string eventId { get; set; }
    public string ticketType { get; set; }
    public string ticketDescription { get; set; }
    public string availableTicketQty { get; set; }
    public string minimumTicketQty { get; set; }
    public string maximumTicketQty { get; set; }
    public string price { get; set; }
    public string saleEndDate { get; set; }
    public string name { get; set; }
    public string phone { get; set; }
    public string qty { get; set; }
    public string nrcNumber { get; set; }
}

public class Order : Ticket
{
    public string orderId { get; set; }
}

public class MinTheinKhaDetail
{
    public string ServiceId { get; set; }
    public string AstrologerId { get; set; }
    public List<string> ProductIds { get; set; }
    public string Amount { get; set; }
    public string Name { get; set; }
    public string BirthTime { get; set; }
    public string Gender { get; set; }
    public string Township { get; set; }
    public string PhoneNo { get; set; }
    public string QuestionDetail { get; set; }

}

public class DateMm 
{
    public string Year { get; set; }
    public string Month { get; set; }
    public string MonthPeriod { get; set; }
    public string Day { get; set; }
    public string DayName { get; set; }

}

public class DateEn 
{
    public string Year { get; set; }
    public string Month { get; set; }
    public string Day { get; set; }
}

#endregion