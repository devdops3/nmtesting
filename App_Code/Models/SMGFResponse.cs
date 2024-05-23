using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

/// <summary>
/// Summary description for SMGFResponse
/// </summary>
public class SMGFInquiryResponse
{
	public SMGFInquiryResponse()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string ContractNumber { get; set; }
    public string ContractDescription { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseCodeName { get; set; }
    public string ResponseDescription { get; set; }
    public string CustomerName { get; set; }
    public string Amount { get; set; }
    public string DueDate { get; set; }
    public string HashValue { get; set; }
}
public class SMGFConfirmResponse
{
    public string TransactionRefNumber { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
    public string ResponseCodeName { get; set; }
    public string InvoiceNumber { get; set; }
    public string HashValue { get; set; }
}

[XmlRoot(ElementName = "InquiryRes")] 
public class SMGFInquiryResponseXML
{
    public string Version { get; set; }
    public string TimeStamp { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string TaxID { get; set; }
    public string MerchantName { get; set; }
    public string MerchantLogo { get; set; }
    public string BillerName { get; set; }
    public string BillerLogo { get; set; }
    public string Ref1 { get; set; }
    public string Ref2 { get; set; }
    public string Ref3 { get; set; }
    public string Ref4 { get; set; }
    public string Ref5 { get; set; }
    public string Ref6 { get; set; }
    public string Ref1Name { get; set; }
    public string Ref2Name { get; set; }
    public string Ref3Name { get; set; }
    public string Ref4Name { get; set; }
    public string Ref5Name { get; set; }
    public string Ref6Name { get; set; }
    public string Amount { get; set; }
    public string AgentFee { get; set; }
    public string Status { get; set; }
    public string Expiry { get; set; }
    public string ProductDesc { get; set; }
    public string ImageURL { get; set; }
}
