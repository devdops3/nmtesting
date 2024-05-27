using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class AyaPayInterfaceResponse
{
    public string QrSchemeRefNo { get; set; }
    public string NearMePaymentRefNo { get; set; }
    public string PaymentSchemeRefNo { get; set; }
    public string ResCode { get; set; }
    public string ResDescription { get; set; }
    public string FailReason { get; set; }
    public string ReferenceNo { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
    public Dictionary<string, string> QrInfo { get; set; }
}