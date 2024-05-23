using System;

/// <summary>
/// Summary description for SimulatorService
/// </summary>
public class SimulatorService
{

    private readonly A2AAPIWCF.ServiceClient _agentWCF;
    public SimulatorService()
    {
        _agentWCF = new A2AAPIWCF.ServiceClient();
    }

    public string ConfrimELoadToEBA(ConfirmResponseModel confirmResponseModel, string amount, int agentId, long txnID,
       double agentAmount, string isAgreement, double agentFeeDbl, string smsStatus, double availableBalance,
       string appType, string topupType, string agentName, string MapTaxID, string serviceFee, string totalAmount
       , string branchCode, string senderName, string billerCode, string messageId, string ref1, string ref2, string ref3, string ref4,
        string ref5, string email, string mobileNo, string billerName, string password, string ref1Name, string ref2Name, string ref3Name,
        string ref4Name, string ref5Name, string billerLogo, string todayTxnAmount, string todayTxnCount)
    {
        string rescode = "01";
        string resdecs = "Airtime Topup is NOT Available.";
        mobileNo = ref3;
        ref3 = "Airtime";
        string responseresult = string.Empty;
        string timediff = "-2";
        if (validateTelenorAirtime(ref3, ref2, timediff, out resdecs))
        {
            #region <-- Request To Telenor AirTimeWCF -->


            responseresult = "OK";
            ref4 = responseresult + " " + DateTime.Now.ToShortDateString();
          resdecs = responseresult.ToString().Trim();
            Utils.WriteLog_Biller("Response Description:" + resdecs);
            Utils.WriteLog_Biller("Response Result From Telenor AirTime:" + responseresult);
            Utils.WriteLog_Biller("Length:" + responseresult.Length);
            #endregion
            
            #region <-- This Is Telenor AirTime -->
            if (responseresult.ToString().Trim() == "OK")
            {
                Utils.WriteLog_Biller("Response OK");
                // ref4 = ref4 + " " + expiry;
                //double availablebalance = 0;
                //double ledgerbalance = 0;
                //string errMsg = null;
                var amt = double.Parse((double.Parse(ref2)).ToString("#,##0.00"));
                string errMsg = string.Empty;
                int batchID;
                if (!_agentWCF.ConfirmUpdate(txnID, ref1, ref2, ref3, ref4, mobileNo, "", "PA", "Paid Successfully", agentId, email,
                    agentAmount, agentFeeDbl, isAgreement, smsStatus, availableBalance, out  errMsg, out  batchID))
                {
                    Utils.WriteLog_Biller("Error in ConfirmUpdate : " + errMsg);
                }
                else
                {
                    Utils.WriteLog_Biller("After update = AgentAmount : " + agentAmount + " | Balance : " + availableBalance.ToString() + "| smsStatus:" + smsStatus);

                }
                Utils.WriteLog_Biller("After update = AgentAmount : " + agentAmount + " | Balance : " + availableBalance.ToString());
                rescode = "00";
                resdecs = "Success";

                var smsMsg = string.Empty;

                ConfirmResponseModel conRes = new ConfirmResponseModel();
                conRes.taxID = MapTaxID;
                conRes.email = email;
                conRes.password = password;
                conRes.messageid = messageId;
                conRes.billerlogo = billerLogo;
                conRes.billername = billerName;
                conRes.rescode = rescode;
                conRes.resdesc = resdecs;
                conRes.ref1 = ref1;
                conRes.ref2 = ref2;
                conRes.ref3 = ref3;
                conRes.ref4 = ref4;
                conRes.ref5 = mobileNo;
                conRes.ref1Name = ref1Name;
                conRes.ref2Name = ref2Name;
                conRes.ref3Name = ref3Name;
                conRes.ref4Name = ref4Name;
                conRes.ref5Name = ref5Name;
                conRes.batchID = string.Empty;
                conRes.availablebalance = availableBalance.ToString();
                conRes.txnID = txnID.ToString();
                conRes.TodayTxnCount = todayTxnCount;
                conRes.TodayTxnAmount = todayTxnAmount;
                conRes.smsMsg = smsMsg;
                return Utils.getConfirmRes(conRes);
            }
            else
            {
                return GetErrorResponseWithAddBalance("06", "Transaction is not successful please tryagain!", txnID, resdecs, agentId, agentAmount, isAgreement);
            }
            #endregion

        }
        else
        {
            return GetErrorResponseWithAddBalance("06", "Transaction is not successful please tryagain!", txnID, resdecs, agentId, agentAmount, isAgreement);
        }
    }

    public string ConfirmEPin(ConfirmResponseModel confirmResponseModel, string amount, int agentId, long txnID,
       double agentAmount, string isAgreement, double agentFeeDbl, string smsStatus, double availableBalance,
       string appType, string topupType, string agentName, string MapTaxID, string serviceFee, string totalAmount
       , string branchCode, string senderName, string billerCode, string messageId, string ref1, string ref2, string ref3, string ref4,
        string ref5, string email, string mobileNo, string billerName, string password, string ref1Name, string ref2Name, string ref3Name,
        string ref4Name, string ref5Name, string billerLogo, string todayTxnAmount, string todayTxnCount)
    {
        Utils.WriteLog_Biller("Simulator Y");
        int batchid = 0;
        string smsMsg = string.Empty;
        //double availablebalance = 0;
        //double ledgerbalance = 0;
        //string errMsg = null;
        //reqCardType = ref1;
        //reqCardPrice = ref2;
        mobileNo = ref3;
        ref3 = new Random().Next(00000000, 99999999).ToString() + new Random().Next(00000000, 99999999).ToString();
        ref4 = new Random().Next(00000000, 99999999).ToString() + new Random().Next(00000000, 99999999).ToString();
        ref4 = ref4 + " 6/12/2016";
        string errMsg;
        //amt = double.Parse((double.Parse(ref2)).ToString("#,##0.00"));
        if (!_agentWCF.InsertTransactionLog(txnID, "TopupReq", "Test Req", out errMsg))
        {
            Utils.WriteLog_Biller("Error in InsertTransactionLog Req : " + errMsg);
        }
        if (!_agentWCF.InsertTransactionLog(txnID, "TopupRes", "Test Res", out errMsg))
        {
            Utils.WriteLog_Biller("Error in InsertTransactionLog Res : " + errMsg);
        }

        Utils.WriteLog_Biller("After update = AgentAmount : " + agentAmount + " | Balance : " + availableBalance.ToString());

        if (appType == "CS" || appType == "MS")
        {
            if (string.IsNullOrEmpty(topupType) || topupType == "S")//topup type is null or S
            {
                SMSHelper smsH = new SMSHelper();
                MessagingService.MessagingServiceClient smsWcf = new MessagingService.MessagingServiceClient();
                string[] words = ref4.Split(' ');
                string PIN = words[0].ToString();
                string Expiry = words[1].ToString();
                smsMsg = smsH.getMessageTopup(agentName, MapTaxID, billerName, PIN, ref3, Expiry, double.Parse(amount).ToString("#,###.00"), branchCode);
                try
                {
                    Utils.WriteLog_Biller("sendSMSWithTxnID starts.");
                    smsWcf.SendSms(txnID.ToString(), smsMsg, mobileNo, senderName);
                    Utils.WriteLog_Biller("sendSMSWithTxnID ends.");
                }
                catch
                {
                }
            }

        }

        int batchID;
        if (!_agentWCF.ConfirmUpdate(txnID,
                ref1, ref2, ref3, ref4, mobileNo, "", "PA", "Paid Successfully", agentId, email,
                agentAmount, agentFeeDbl, isAgreement, smsStatus, availableBalance, out  errMsg, out  batchID))
        {
            Utils.WriteLog_Biller("Error in ConfirmUpdate : " + errMsg);
        }
        else
        {
            Utils.WriteLog_Biller("After update = AgentAmount : " + agentAmount + " | Balance : " + availableBalance.ToString() + "| smsStatus:" + smsStatus);

        }

        string rescode = "00";
        string resdecs = "Success";

        var confirmres = new ConfirmResponseModel{
        taxID = MapTaxID,
        email = email,
        password = password,
        messageid = messageId,
        billername = billerName,
        billerlogo = billerLogo,
        rescode = rescode,
        resdesc = resdecs,
        ref1 = ref1,
        ref2 = ref2,
        ref3 = ref3,
        ref4 = ref4,
        ref5 = mobileNo,
        ref1Name = ref1Name,
        ref2Name = ref2Name,
        ref3Name = ref3Name,
        ref5Name = ref5Name,
        batchID = batchid.ToString(),
        availablebalance = availableBalance.ToString(),
        txnID = txnID.ToString(),
        TodayTxnAmount = todayTxnAmount,
        TodayTxnCount = todayTxnCount,
        smsMsg = smsMsg
    };
        return Utils.getConfirmRes(confirmres);
    }

    private  bool validateTelenorAirtime(string phoneno, string amount, string timediff, out string resErr)
    {
        resErr = string.Empty;
        bool result = false;
        double tlnAmount = double.Parse(amount);

        if (tlnAmount < 1000 || tlnAmount > 100000)
        {
            resErr = "Input amount must be between 1,000 and 100,000.";

        }
        else if ((tlnAmount % 1000) != 0)
        {
            resErr = "Invalid topup amount.Please enter amount in thousand.!";

        }
        else if (_agentWCF.checkTelenoreTimeInterval(phoneno, amount, timediff))//check is there have same toupuamount and mobilenumber existed in transaction table before 2 min of current time or not?
        {
            resErr = "Please try again after 2 minutes.";

        }
        else
        {
            result = true;
        }
        return result;
    }

    private string GetErrorResponseWithAddBalance(string rescode, string resdesc, long txnID, string logerrormessage, int agentID, double amount, string isAgreement, string taxId = "")
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