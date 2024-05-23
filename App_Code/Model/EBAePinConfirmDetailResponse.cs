using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EBAePinConfirmDetailResponse
/// </summary>
public class EBAePinConfirmDetailResponse
{
	public EBAePinConfirmDetailResponse()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string Deno { get; set; }
    public string PinId { get; set; }
    public string SerialNumber { get; set; }
    public string ClearPin { get; set; }
    public string ExpiryDate { get; set; }
}