using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DataPackageIntegrationResponse
/// </summary>
public class DataPackageDetailIntegrationResponse
{
    public string Code { get; set; }
    public string Detail { get; set; }
    public bool Is_active { get; set; }
    public string Name { get; set; }
    public string Operator_name { get; set; }
    public string Package_type { get; set; }
    public long Price { get; set; }
    public string Validity { get; set; }
    public string Volume { get; set; }
}