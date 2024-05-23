using System.Configuration;

/// <summary>
/// Summary description for MobileApiConstants
/// </summary>
public class MobileApiConstants
{
	public MobileApiConstants()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static readonly string JooxTaxId = ConfigurationManager.AppSettings["JooxTaxId"].ToString();
    public static readonly string JooxBillerCode = ConfigurationManager.AppSettings["0000000000129BillerCode"].ToString();
}