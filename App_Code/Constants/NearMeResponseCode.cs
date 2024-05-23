using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for NearMeResponseCode
/// </summary>
public class NearMeResponseCode
{
	public NearMeResponseCode()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static readonly string Success = "00";
    public static readonly string Failed = "01";
    public static readonly string Timeout = "03";
    public static readonly string AuthFailed = "05";
    public static readonly string InvalidReq = "06";
    public static readonly string InvalidMsg = "07";
    public static readonly string NoCustomerBound = "09"; 
    public static readonly string APIErrorFor123 = "97";
    public static readonly string SystemError = "99";
}