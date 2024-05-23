using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for OnePayConstants
/// </summary>
public class OnePayConstants
{
	public OnePayConstants()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static readonly string SecretKey = ConfigurationManager.AppSettings["OnePaySecretKey"].ToString();
    public static readonly string InquiryUrl = ConfigurationManager.AppSettings["OnePayInquiryUrl"].ToString();
    public static readonly string ConfirmUrl = ConfigurationManager.AppSettings["OnePayConfirmUrl"].ToString();
    public static readonly string AgentID = ConfigurationManager.AppSettings["OnePayAgentId"].ToString();
    public static readonly string InvoiceNo = ConfigurationManager.AppSettings["OnePayInvoiceNo"].ToString();
    public static readonly int ExpiredSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["OnePayExpiredSeconds"].ToString());
    public static readonly string Success = "000";
}