using A2AAPIWCF;

/// <summary>
/// Summary description for MobileAPIWCFManager
/// </summary>
public class MobileAPIWCFManager
{

    ServiceClient _agentWCF;
    public MobileAPIWCFManager()
    {
        _agentWCF = new ServiceClient();
    }

    public string GetErrorResponseWithAddBalance(string rescode, string resdesc, long txnID, string logerrormessage, int agentID, double amount, string isAgreement, string taxId = "")
    {
        Utils.WriteLog_Biller("Update Error With Add Balance");
        string errMsg = string.Empty;
        double availableBalance = 0;
        double ledgerBalance = 0;
        if (!_agentWCF.UpdateErrorWithAddingBalance(txnID, "ER", resdesc, agentID, amount, isAgreement, out errMsg, out availableBalance, out ledgerBalance))
        {
            Utils.WriteLog_Biller("Error in update Error with Add Balance : " + errMsg);
        }
        Utils.WriteLog_Biller("After Update Error With Add Balance| TxnID:" + txnID + "|agentID:" + agentID + "AvailableBalance:" + availableBalance + "LedgerBalance:" + ledgerBalance);
        return Utils.getErrorRes(rescode, resdesc, taxId);//"Processing is failed because of internal server error, please try again.");
    }
}