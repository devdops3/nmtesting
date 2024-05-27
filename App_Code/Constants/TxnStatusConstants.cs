using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TxnStatusConstants
/// </summary>
public class TxnStatusConstants
{
	public TxnStatusConstants()
	{
		//
		// TODO: Add constructor logic here
		//       
	}

    public static readonly string Processing = "PR";
    public static readonly string Paid = "PA";
    public static readonly string Error = "ER";    
    public static readonly string Void = "VO";
    public static readonly string Refund = "RF";
    public static readonly string Cancel = "CA";
}