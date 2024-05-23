using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ELoadProcessRequestModel
/// </summary>
public class ELoadProcessRequestModel
{
    public string MobileNumber { get; set; }
    public string Amount { get; set; }
    public int AgentId { get; set; }
    public long TxnId { get; set; }
    public double AgentAmount { get; set; }
    public string IsAgreement { get; set; }
    public double AgentFeeDbl { get; set; }
    public string SmsStatus { get; set; }
    public double AvailableBalance { get; set; }
    public string BillerCode { get; set; }
    public string MessageId { get; set; }
}