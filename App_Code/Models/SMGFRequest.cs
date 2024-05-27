using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SMGFRequest
/// </summary>
public class SMGFInquiryRequest
{
    public SMGFInquiryRequest()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string ContractNumber { get; set; }
    public string TimeStamp { get; set; }
    public string AgentCode { get; set; }
    public string PaymentType { get; set; }
    public string HashValue { get; set; }
}
public class SMGFConfirmRequest
{
    public string TransactionRefNumber { get; set; }
    public string ContractNumber { get; set; }
    public string Amount { get; set; }
    public string MobileNumber { get; set; }
    public string AgentCode { get; set; }
    public string PaymentType { get; set; }
    public string HashValue { get; set; }
}