using System;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Summary description for SkynetEnquiryResponseModel
/// </summary>
public class SkynetEnquiryResponseModel
{
    public SkynetEnquiryResponseModel()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string TransactionStatus { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string PartnerAmount { get; set; }
    public string TransactionAmount { get; set; }
    public string Detail { get; set; }
}

public enum SkyNetPackageType
{
    payperview,
    subscription
}

public class SkynetEquiryDetail
{
    public string SmartCard_or_ChipSet { get; set; }
    public string SubscriptionNumber { get; set; }
    public string RemainingDays { get; set; }
    public string CurrentProduct { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public List<SkynetPackage> Package { get; set; }
}

public class SkynetPayPerViewEquiryDetail
{
    public string SmartCard_or_ChipSet { get; set; }
    public List<SkynetPayPerViewPackage> Package { get; set; }
}

public class SkynetPackage
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Product_Code { get; set; }
    public string Pay_Amount { get; set; }
}

[XmlRoot(ElementName = "SkynetPackage")]
public class SkynetPayPerViewPackage
{
    public string Name { get; set; }
    public string Product_Code { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Pay_Amount { get; set; }
}

public class SkyNetPackageRes
{
    public string Version { get; set; }
    public string TimeStamp { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public List<SkynetPackage> Packages { get; set; }
}

[XmlRoot(ElementName = "SkyNetPackageRes")] 
public class SkyNetPackagePayPerViewRes
{
    public string Version { get; set; }
    public string TimeStamp { get; set; }
    public string ResCode { get; set; }
    public string ResDesc { get; set; }
    public List<SkynetPayPerViewPackage> Packages { get; set; }
}

public class SkyNetConfirmResponse
{
    public string TransactionStatus { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string Detail { get; set; }
    public string EBARefNo { get; set; }
    public string PartnerRefNo { get; set; }
    public string PartnerAmount { get; set; }
    public string TransactionAmount { get; set; }
    public string PartnerBalance { get; set; }

}

public class SkyNetConfirmDetailResponse
{
    public string SmartCard_or_ChipSet { get; set; }
    public string ProductCode { get; set; }
    public string RemainingDays { get; set; }
    public string ExpiryDate { get; set; }
   }


public class SkyNetConfirmPayPerViewDetailResponse
{
    public string SmartCard_or_ChipSet { get; set; }
    public string ProductCode { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string SubscriptionNumber { get; set; }
    public string ExecutedOn { get; set; }
}