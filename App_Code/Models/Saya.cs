using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

/// <summary>
/// Summary description for Sayacs
/// </summary>
///

public class SayaInquiryDetailRes
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public List<Package> Package { get; set; }
}

public class SayaConfirmDetailRes
{
    public string UserId { get; set; }
    public string ProductCode { get; set; }
    public string UserName { get; set; }
    public string Amount { get; set; }
}

public class Package
{
    public string ProductCode { get;set; }
    public string Name { get; set; }
    public string Amount { get; set; }
    public string AgentFee { get; set; }
}

[XmlRoot(ElementName = "InquiryRes")]
public class SayaInquiryResponseXML : InquiryResponse
{
    public List<Package> Packages { get; set; }
}