using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SMGFConstants
/// </summary>
public class SMGFConstants
{
	public SMGFConstants()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static readonly string RFP = ConfigurationManager.AppSettings["SMGF_RFP"].ToString();
    public static readonly string OTP = ConfigurationManager.AppSettings["SMGF_OTP"].ToString();
    public static readonly string AgentCode = ConfigurationManager.AppSettings["SMGF_AgentCode"].ToString();
    public static readonly string SecretKey = ConfigurationManager.AppSettings["SMGF_SecretKey"].ToString();
    public static readonly string InquiryUrl = ConfigurationManager.AppSettings["SMGF_InquiryUrl"].ToString();
    public static readonly string ConfirmUrl = ConfigurationManager.AppSettings["SMGF_ConfirmUrl"].ToString();
    public static readonly string Success = "00";
}