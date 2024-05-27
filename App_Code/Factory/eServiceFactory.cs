using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for eServiceFactory
/// </summary>
public class eServiceFactory
{
    public IeService GetBillerManager(string taxId)
    {
        // Check the taxId include in ELoad/EPin biller list
        if (IsTelCoBiller(taxId))
            return new TelCoManager();

        switch (taxId)
        {          
            case BISConstants.ShweStreamTaxId :
                return new ShweStreamManager();
            case BISConstants.OnePayCashInTaxId :
                return new OnePayCashInManager();
            case BISConstants.SayaTaxId :
                return new SayaManager();
            case BISConstants.AyaPayCashIn:
                return new AYAPayCashInManager();
            case BISConstants.GrabRideTaxId:
                return new GrabRideManager();
            case BISConstants.GrabFoodTaxId:
                return new GrabRideManager();
            default:
                return null;
        }       
    }

    public bool IsTelCoBiller(string taxId) 
    {
        List<string> BillerList = ConfigurationManager.AppSettings["ELoadBillerList"].ToString().Split(',').ToList();
        BillerList.AddRange(ConfigurationManager.AppSettings["EPinBillerList"].ToString().Split(',').ToList());
        BillerList.AddRange(ConfigurationManager.AppSettings["DataPackBillerList"].ToString().Split(',').ToList());
        return (BillerList.Contains(taxId)) ? true : false;           
    }
}