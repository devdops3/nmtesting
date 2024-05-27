using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

/// <summary>
/// Summary description for UtilsModel
/// </summary>
[XmlRoot(ElementName = "InquiryRes")]
public class UtilsInquiryResponse
{
    public string Version { get; set; }
    public string TimeStamp { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string TaxID { get; set; }
    public string Ref1 { get; set; }
    public string Ref2 { get; set; }
    public string Ref3 { get; set; }
    public string Ref4 { get; set; }
    public string Ref5 { get; set; }
    public string Ref1Name { get; set; }
    public string Ref2Name { get; set; }
    public string Ref3Name { get; set; }
    public string Ref4Name { get; set; }
    public string Ref5Name { get; set; }
    public string Amount { get; set; }
    public string AgentFee { get; set; }
    public string Status { get; set; }
    public string Expiry { get; set; }
    public string ProductDesc { get; set; }
    public string ImageURL { get; set; }
}

[XmlRoot(ElementName = "ConfirmRes")]
public class UtilsConfirmResponse
{
    public string Version { get; set; }
    public string TimeStamp { get; set; }
    public string Email { get; set; }
    public string TaxID { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public string Ref1 { get; set; }
    public string Ref2 { get; set; }
    public string Ref3 { get; set; }
    public string Ref4 { get; set; }
    public string Ref5 { get; set; }
    public string Ref1Name { get; set; }
    public string Ref2Name { get; set; }
    public string Ref3Name { get; set; }
    public string Ref4Name { get; set; }
    public string Ref5Name { get; set; }
    public string BatchID { get; set; }
    public string Balance { get; set; }
    public string TxnID { get; set; }
    public string TodayTxnCount { get; set; }
    public string TodayTxnAmount { get; set; }
    public string SMS { get; set; }
}