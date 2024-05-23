using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EPinOrderDetailResponse
/// </summary>
public class EPinOrderDetailIntegrationResponse
{
    public string Expiry_date { get; set; }
    public string OrderId { get; set; }
    public string Order_status { get; set; }
    public string Pin { get; set; }
    public string Serial_number { get; set; }
    public string StatusCode { get; set; }
    public string Status { get; set; }
    public string MsgId { get; set; }
    public string Reason { get; set; }
}