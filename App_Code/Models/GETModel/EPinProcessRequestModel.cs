using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EPinProcessRequestModel
/// </summary>
public class EPinProcessRequestModel
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
    public string AppType { get; set; }
    public string TopupType { get; set; }
    public string AgentName { get; set; }
    public string MapTaxId { get; set; }
    public string BranchCode { get; set; }
    public string SenderName { get; set; }
    public string BillerCode { get; set; }
    public string MessageId { get; set; }
    public string Ref1 { get; set; }
    public string Ref2 { get; set; }
    public string Ref3 { get; set; }
    public string Email { get; set; }
    public string BillerName { get; set; }
    public string Password { get; set; }
    public string Ref1Name { get; set; }
    public string Ref2Name { get; set; }
    public string Ref3Name { get; set; }
    public string Ref4Name { get; set; }
    public string Ref5Name { get; set; }
    public string BillerLogo { get; set; }
    public string TodayTxnAmount { get; set; }
    public string TodayTxnCount { get; set; }
}