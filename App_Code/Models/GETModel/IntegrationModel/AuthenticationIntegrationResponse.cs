using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AuthenticationResponse
/// </summary>
public class AuthenticationIntegrationResponse
{
    public string AuthId { get; set; }
    public string Authtoken { get; set; }
    public int Expire_in { get; set; }
    public string GrantType { get; set; }
    public string Status { get; set; }
}