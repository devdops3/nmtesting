using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DataPackInquiryIntegrationResponse
/// </summary>
public class DataPackInquiryIntegrationResponse
{
    public string Status { get; set; }
    public List<DataPackageDetailIntegrationResponse> Packages_list { get; set; }
}