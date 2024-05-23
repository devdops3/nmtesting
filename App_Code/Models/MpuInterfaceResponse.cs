using System.Collections.Generic;

public class MpuInterfaceResponse
{
    public string NearMePaymentRefNo { get; set; }
    public string TransactionRefNo { get; set; }
    public string ResCode { get; set; }
    public string ResDescription { get; set; }
    public string FailReason { get; set; }
    public Dictionary<string, string> PaymentInfo { get; set; }
}