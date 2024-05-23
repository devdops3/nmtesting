using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ELoadResponse
/// </summary>
public class ELoadIntegrationResponse
{
    public int Amount { get; set; }
    public string Invoice_number { get; set; }
    public string Operator { get; set; }
    public string Phone_number { get; set; }
    public string Reason { get; set; }
    public int Selling_price { get; set; }
    public string Status { get; set; }
}