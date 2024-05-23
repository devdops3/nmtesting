using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AuthenticationModel
/// </summary>
public class AuthenticationModel
{
    public string AuthId { get; set; }
    public string Authtoken { get; set; }
    public int ExpireIn { get; set; }
    public string Status { get; set; }
    public DateTime GeneratedTime { get; set; }
}