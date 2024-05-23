using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for MobileApiConstants
/// </summary>
public class BISConstants
{
    public BISConstants()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static readonly string EasyMicroFinanceTaxId = ConfigurationManager.AppSettings["EasyMicrofinanceTaxId"].ToString();
    public static readonly string EasyMicroFinanceBillerCode = ConfigurationManager.AppSettings["EasyMicrofinanceBillerCode"].ToString();
    public static readonly string ESBAChannel = ConfigurationManager.AppSettings["EsbaChannel"].ToString();
    public static readonly string EBAInquiryUrl = ConfigurationManager.AppSettings["MobileLegendInquiryUrl"].ToString();
    public static readonly string EBAConfirmUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();
    public static readonly string iTunesTaxId = ConfigurationManager.AppSettings["iTunesTaxId"].ToString();
    public static readonly string iTunesBillerCode = ConfigurationManager.AppSettings["iTunesBillerCode"].ToString();
    public static readonly string GooglePlayTaxId = ConfigurationManager.AppSettings["GooglePlayTaxId"].ToString();
    public static readonly string GooglePlayBillerCode = ConfigurationManager.AppSettings["GooglePlayBillerCode"].ToString();
    public static readonly string MyPlayTaxId = ConfigurationManager.AppSettings["MyPlayTaxId"].ToString();
    public static readonly string MyPlayBillerCode = ConfigurationManager.AppSettings["MyPlayBillerCode"].ToString();
    public static readonly string SteamWalletTaxId = ConfigurationManager.AppSettings["SteamWalletTaxId"].ToString();
    public static readonly string SteamWalletBillerCode = ConfigurationManager.AppSettings["StemWalletBillerCode"].ToString();
    public static readonly string VakokTaxId = ConfigurationManager.AppSettings["VakokTaxId"].ToString();
    public static readonly string VakokBillerCode = ConfigurationManager.AppSettings["VakokBillerCode"].ToString();
    public static readonly string SteamWalletSGDTaxId = ConfigurationManager.AppSettings["SteamWalletSGDMapTaxId"].ToString();
    public static readonly string SteamWalletSGDBillerCode = ConfigurationManager.AppSettings["SteamWalletSGDBillerCode"].ToString();
    public static readonly string CanalPlusBillerCode = ConfigurationManager.AppSettings["CanalPlusBillerCode"].ToString();
    public static readonly List<string> ELoadBillerList = ConfigurationManager.AppSettings["ELoadBillerList"].ToString().Split(',').ToList();
    public static readonly List<string> EPinBillerList = ConfigurationManager.AppSettings["EPinBillerList"].ToString().Split(',').ToList();
    public const string MoMoneyPay = "MoMoney";
    public const string GrabRideTaxId = "0000000000141";
    public const string GrabFoodTaxId = "0000000000142";
    public const string ShweStreamTaxId = "0000000000143";
    public const string OnePayCashInTaxId = "0000000000144";
    public const string AtomDataPackTaxId = "0000000000071";
    public const string SayaTaxId = "0000000000145";
    public const string APlus = "A+ Wallet";
    public const string AyaPayCashIn = "0000000000146";
}