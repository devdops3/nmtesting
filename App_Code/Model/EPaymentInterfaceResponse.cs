using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EPaymentInterfaceResponse
/// </summary>
public class EPaymentInterfaceResponse
{
    public string NearMePaymentRefNo { get; set; }
    public string PaymentSchemeRefNo { get; set; }
    public string ResCode { get; set; }
    public string ResDescription { get; set; }
    public string FailReason { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
}
