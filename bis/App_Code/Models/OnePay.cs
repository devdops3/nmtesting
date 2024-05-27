using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

/// <summary>
/// Summary description for OnePay
/// </summary>
public class OnePayInquiryRequest
{
    public string AgentID { get; set; }
    public string SubAgentID { get; set; }
    public string InvoiceNo { get; set; }
    public string SequenceNo { get; set; }
    public string ReceiverNo { get; set; }
    public string Amount { get; set; }
    public int ExpiredSeconds { get; set; }
    public string RequestTimeStamp { get; set; }
    public string HashValue { get; set; }
}

public class OnePayInquiryResponse
{
    public InquiryDetail InquiryDetail { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
    public string NextAllowedTime { get; set; }
}

public class InquiryDetail
{
    public string AgentID { get; set; }
    public string SubAgentID { get; set; }
    public string AgentName { get; set; }
    public string OriginalAmount { get; set; }
    public double AgentCommission { get; set; }
    public double CustomerCharges { get; set; }
    public double TotalAmount { get; set; }
    public string InvoiceNo { get; set; }
    public string SequenceNo { get; set; }
    public string ReceiverNo { get; set; }
    public string OnePayUserName { get; set; }
    public string KYCStatus { get; set; }
    public string HashValue { get; set; }
}

public class OnePayConfirmRequest
{
    public string AgentID { get; set; }
    public string SubAgentID { get; set; }
    public string SequenceNo { get; set; }
    public string ReceiverNo { get; set; }
    public string Amount { get; set; }
    public string RequestTimeStamp { get; set; }
    public string HashValue { get; set; }
}

public class OnePayConfirmResponse
{
    public string ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
    public string SequenceNo { get; set; }
    public string OnePayUserName { get; set; }
}

[XmlRoot(ElementName = "InquiryRes")] 
public class OnePayInquiryResponseXML : InquiryResponse
{
    public string Expiry { get; set; }
}
