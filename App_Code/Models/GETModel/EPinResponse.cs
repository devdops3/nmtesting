using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EPinResponse
/// </summary>
public class EPinResponse
{
    public string ExpiryDate { get; set; }
    public string OrderNumber { get; set; }
    public string Pin { get; set; }
    public string Description { get; set; }
    public string SerialNumber { get; set; }
    public string Status { get; set; }
}