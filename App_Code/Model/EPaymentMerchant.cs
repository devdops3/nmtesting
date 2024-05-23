using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EPaymentMerchant
/// </summary>
public class EPaymentMerchant
{
    public string MerchantId { get; set; }
    public string SecretKey { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string ProfileName { get; set; }
    public string Terminal { get; set; }
}