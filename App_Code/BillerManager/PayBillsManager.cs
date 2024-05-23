using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for PayBillsManager
/// </summary>
public class PayBillsManager
{
    private static A2AAPIWCF.ServiceClient _agentWCF = new A2AAPIWCF.ServiceClient();
    private static fraudWs.Service1Client _fraudWCF = new fraudWs.Service1Client();


	public PayBillsManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string Confrim(ConfirmResponseModel confirmResponseModel, string amount, int agentId, long txnID,
                            double agentAmount, string isAgreement, double agentFeeDbl, string smsStatus, double availablebalance,
                            string appType, string topupType, string agentName, string MapTaxID, string serviceFee, string totalAmount
                            , string branchCode, string senderName, string agentUserUniqueID, string agentCode)
    {
        var logMessage = "MessageId : " + confirmResponseModel.messageid + " PayBills Confirm. ";
        Utils.WriteLog_Biller(logMessage);
        int batchid = 0;
        int agentIDplusForChannel = 0;
        double readdedamount = 0;
        string errmsg = string.Empty;
        DataSet dsAgnt;
        bool AddAmountToUser = false;
        string inquiryMobileNo = string.Empty;
        string paymentMobileNo = string.Empty;
        double defaultServiceFee = 0;
        try
        {
            Utils.WriteLog_Biller(logMessage + "PayBills Service Fee : " + defaultServiceFee);
            inquiryMobileNo = confirmResponseModel.ref3.Split()[0].ToString();
            paymentMobileNo = confirmResponseModel.ref3.Split()[1].ToString();
            Utils.WriteLog_Biller("InquiryMobileNo : " + inquiryMobileNo + ", PaymentMobileNo : " + paymentMobileNo);
            if (!_agentWCF.getAgentNameByAgentCode(confirmResponseModel.ref2, out dsAgnt, out errmsg))//Ref1 is agentCode
            {
                Utils.WriteLog_Biller(logMessage + "Error in getAgentNameByAgentCode : " + errmsg);
            }

            if (dsAgnt.Tables[0].Rows.Count > 0)
            {
                if (dsAgnt.Tables[0].Rows[0]["AGREEMENTTYPE"].ToString() != "Y")
                {
                    Utils.WriteLog_Biller(logMessage + "Before update = AgentAmount : " + agentAmount + " | Balance : " + availablebalance.ToString());
                    int agentIDPlus = 0;
                    agentIDPlus = int.Parse(dsAgnt.Tables[0].Rows[0]["AGENTID"].ToString());
                    agentIDplusForChannel = agentIDPlus;
                    if (agentIDPlus > 0)
                    {
                        double avalBal = 0;
                        double ledeBal = 0;
                        double amt = double.Parse((double.Parse(amount)).ToString("#,##0.00"));

                        #region <-- Add Balance Into Payee -->
                        if (_agentWCF.addFund2OneStopPlus(agentIDPlus, amt, out avalBal, out ledeBal, out errmsg))
                        {
                            readdedamount = amt;
                            AddAmountToUser = true;
                            Utils.WriteLog_Biller(logMessage + "After addFund = AgentIDPlus : " + agentIDPlus.ToString() + " |Topuped Amount :" + amt.ToString() + "| AgentAmount : " + agentAmount + " | Balance : " + availablebalance.ToString());

                            Utils.WriteLog_Biller(logMessage + "Fund Added OK to AgentID : " + agentIDPlus);

                            #region <-- Update Transaction Region -->
                            if (!_agentWCF.ConfirmUpdate(txnID, 
                                confirmResponseModel.ref1, confirmResponseModel.ref2, 
                                confirmResponseModel.ref3, confirmResponseModel.ref4, 
                                confirmResponseModel.ref5, string.Empty, "PA", "Paid Successfully", 
                                agentId, confirmResponseModel.email, agentAmount, 
                                agentFeeDbl, isAgreement, smsStatus, availablebalance, 
                                out  errmsg, out  batchid))
                            {
                                Utils.WriteLog_Biller(logMessage + "Error in ConfirmUpdate : " + errmsg);
                            }
                            else
                            {
                                Utils.WriteLog_Biller(logMessage + "After update = AgentAmount : " + agentAmount + " | Balance : " + availablebalance.ToString() + "| smsStatus:" + smsStatus);

                                if (!_agentWCF.insertAgentTopupByPayBills(
                                    ConfigurationManager.AppSettings["PayBillsPaymentType"].ToString(),
                                    amt, agentIDPlus, agentUserUniqueID,
                                    "AP", 
                                    ConfigurationManager.AppSettings["PayBillsPayment"].ToString(),
                                    txnID,
                                    agentCode, 
                                    confirmResponseModel.ref4,
                                    defaultServiceFee.ToString(), agentName,
                                    confirmResponseModel.ref1,
                                    confirmResponseModel.ref2,
                                    inquiryMobileNo,
                                    out errmsg))
                                {
                                    Utils.WriteLog_Biller(logMessage + "Error in insertAgentTopup : " + errmsg);
                                }
                                else
                                {
                                    Utils.WriteLog_Biller("Error in insertAgentTopupByPayBills : " + errmsg);
                                }
                            }
                            #endregion

                            #region <-- Push Noti Region -->
                            try
                            {
                                // push noti
                                pushNotiWCF.ServiceClient pushnoti = new pushNotiWCF.ServiceClient();
                                AdminWcf.ServiceClient _admWS = new AdminWcf.ServiceClient();
                               
                                string notiMsg = "You have successfully reloaded " + amount + " Ks to your account.";
                                string notiType = "Login Device";

                                // get agent users information
                                if (dsAgnt.Tables[2].Rows.Count > 0)
                                {
                                   var dt = dsAgnt.Tables[2];
                                    var agentUserInfoList = (from DataRow row in dt.Rows
                                                         select new AgentUserInfo
                                                         {
                                                             DeviceToken = row["DEVICETOKEN"] == DBNull.Value ? string.Empty : Convert.ToString(row["DEVICETOKEN"]),
                                                             DeviceInfo = row["DEVICEINFO"] == DBNull.Value ? string.Empty : Convert.ToString(row["DEVICEINFO"]),
                                                             AgentUserId = row["AGENTUSERID"] == DBNull.Value ? string.Empty : Convert.ToString(row["AGENTUSERID"])
                                                         }).ToList();

                                    var ds = new DataSet();
                                    // add noti into notification table
                                    if (_admWS.pushNotification(ConfigurationManager.AppSettings["PayBillsPushNotiTitle"].ToString(), notiMsg, "NearMe", DateTime.Now.ToString(), "Notification", out errmsg, out ds))
                                    {
                                        var ii = ds.Tables[0].Rows[0].ItemArray[0];
                                        int id = Convert.ToInt32(ii);
                                        string agentUserNotiId = string.Empty;
                                        Utils.WriteLog_Biller(logMessage + "Pushed Noti to Agent: " + confirmResponseModel.ref2 + ", NotiId: " + id);
                                        foreach (AgentUserInfo agentUserInfo in agentUserInfoList)
                                        {
                                            if (_admWS.AddNotiProfile(id, notiType, agentUserInfo.AgentUserId, out errmsg))
                                            {
                                                if (_admWS.AddAgentNotiList(agentUserInfo.AgentUserId, id, out errmsg, out agentUserNotiId))
                                                {
                                                    if (agentUserInfo.DeviceInfo.StartsWith("iOS"))
                                                    {
                                                        Utils.WriteLog_Biller(logMessage + "Device Info is  " + agentUserInfo.DeviceInfo + "PushToApple For agent user  " + agentUserInfo.AgentUserId);
                                                        pushnoti.PushToApple(agentUserInfo.DeviceToken, notiMsg, 0, out errmsg);
                                                    }
                                                    else
                                                    {
                                                        Utils.WriteLog_Biller(logMessage + "Device Info is  " + agentUserInfo.DeviceInfo + "PushToAndroid For agent user  " + agentUserInfo.AgentUserId);
                                                        pushnoti.PushToAndroid(agentUserInfo.DeviceToken, notiMsg, ConfigurationManager.AppSettings["PayBillsPushNotiTitle"].ToString(), agentUserNotiId, out errmsg);
                                                    }
                                                }
                                                else
                                                {
                                                    Utils.WriteLog_Biller(logMessage + "Failed to AddAgentNotiList!");
                                                }
                                            }
                                            else
                                            {
                                                Utils.WriteLog_Biller(logMessage + "Failed to AddNotiProfile!");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Utils.WriteLog_Biller(logMessage + "Failed to add noti into notification table!");
                                    }
                                }
                                else
                                {
                                    Utils.WriteLog_Biller(logMessage + "No agent user in the agent :" + confirmResponseModel.ref2);
                                }
                            }
                            catch (Exception ex)
                            {
                                Utils.WriteLog_Biller(logMessage + "Exception error occure at NearMe+ push notification: " + ex.Message);
                            }

                            #endregion

                            #region <-- Send SMS -->
                            string smsMsg = string.Empty;
                            if (appType == "CS" || appType == "MS")
                            {
                                Utils.WriteLog_Biller("appType is CS or MS");
                                if (string.IsNullOrEmpty(topupType) || topupType == "S") //topup type is null or S
                                {
                                    Utils.WriteLog_Biller("topupType is null or Not S");
                                    SMSHelper smsH = new SMSHelper();
                                    MessagingService.MessagingServiceClient smsWcf = new MessagingService.MessagingServiceClient();

                                    smsMsg = smsH.getMessageBiller(agentName, MapTaxID, 
                                        confirmResponseModel.ref5, 
                                        confirmResponseModel.ref1Name, 
                                        confirmResponseModel.ref3Name, 
                                        string.Empty, "Ref", 
                                        confirmResponseModel.ref1,
                                        paymentMobileNo, string.Empty, txnID.ToString(), 
                                        double.Parse(amount).ToString("#,##0.00"), 
                                        serviceFee, double.Parse(totalAmount).ToString("#,##0.00"), 
                                        branchCode);

                                    try
                                    {

                                        Utils.WriteLog_Biller("Mobile No :" + paymentMobileNo + "| Message :" + smsMsg + "| Sender Name :" + senderName + "|txn ID :" + txnID);
                                        smsWcf.SendSms(txnID.ToString(), smsMsg, paymentMobileNo, senderName);
                                        Utils.WriteLog_Biller("sendSMSWithTxnID ends.");
                                    }
                                    catch (Exception ex)
                                    {
                                        Utils.WriteLog_Biller(string.Format("Error in SendSms: {0}", ex.ToString()));
                                    }
                                }
                            }

                            #endregion

                            #region <-- Success Confirm Response to Client -->

                            Task.Factory.StartNew(() => _fraudWCF.CheckSender(txnID, agentId, branchCode, confirmResponseModel.email, decimal.Parse(amt.ToString()), ConfigurationManager.AppSettings["PayBillsPayment"].ToString()));
                            Task.Factory.StartNew(() => _fraudWCF.CheckReceiver(agentIDPlus, decimal.Parse(amt.ToString()), ConfigurationManager.AppSettings["PayBillsPayment"].ToString(), txnID.ToString()));

                            confirmResponseModel.rescode = "00";
                            confirmResponseModel.resdesc = "Success";
                            confirmResponseModel.availablebalance = availablebalance.ToString();
                            confirmResponseModel.txnID = txnID.ToString();
                            confirmResponseModel.smsMsg = smsMsg;
                            confirmResponseModel.batchID = batchid.ToString();
                            confirmResponseModel.ref3 = paymentMobileNo;
                            #endregion
                        }
                        else
                        {
                            Utils.WriteLog_Biller(logMessage + "Error in addFund2OneStopPlus for PayBills : " + errmsg);
                            Utils.WriteLog_Biller(logMessage + "Fund Added NOT OK to AgentID for PayBills : " + agentIDPlus);
                            var rescode = "10";
                            var resdecs = "Adding Fund Failed";
                            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(rescode, resdecs, txnID, resdecs, agentId, agentAmount, isAgreement);

                        }
                        #endregion
                    }
                    else
                    {
                        Utils.WriteLog_Biller("No AgentID for PayBills : " + agentIDPlus.ToString());
                    }
                }

                else
                {
                    return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance("01", "Agent NOT Applicable", txnID, string.Empty, agentId, agentAmount, isAgreement);
                }
            }
            else
            {
                var rescode = "01";
                var resdecs = errmsg;
                return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(rescode, resdecs, txnID, resdecs, agentId, agentAmount, isAgreement);
            }
            return Utils.getConfirmRes(confirmResponseModel);

        }
        catch (Exception ex)
        {
            if (MapTaxID == ConfigurationManager.AppSettings["PayBillsTaxId"].ToString() && AddAmountToUser)
            {
                Utils.WriteLog_Biller(logMessage + "Error ConfirmToBiller : " + ex.ToString());
                string rescode = "97";
                string resdecs = "BIS API Error";
                return ChannelPlusGetErrorResponse(
                    logMessage, rescode, resdecs, txnID, resdecs, agentId, 
                    agentIDplusForChannel, agentAmount, readdedamount, isAgreement);
            }
            else
            {
                Utils.WriteLog_Biller(logMessage + "Error ConfirmToBiller : " + ex.ToString());
                var rescode = "97";
                var resdecs = "BIS API Error";
                return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(rescode, resdecs, txnID, resdecs, agentId, agentAmount, isAgreement);
            }

        }
    }

    public string ChannelPlusGetErrorResponse(string logMessage, string rescode, string resdesc, long txnID, string logerrormessage, int agentID, int agentIDPlus, double amount, double readdedamount, string isAgreement)
    {
        Utils.WriteLog_Biller(logMessage + "Update Error With Add Balance");
        string errMsg = string.Empty;
        double availableBalance = 0;
        double ledgerBalance = 0;
        if (!_agentWCF.UpdateErrorWithAddingBalance(txnID, "ER", logerrormessage, agentID, amount, isAgreement, out errMsg, out availableBalance, out ledgerBalance))
        {
            Utils.WriteLog_Biller(logMessage + "Error in update Error with Add Balance : " + errMsg);
        }
        if (!_agentWCF.minusChannelPlusAmt(txnID, "ER", logerrormessage, agentIDPlus, readdedamount, isAgreement, out errMsg, out availableBalance, out ledgerBalance))
        {
            Utils.WriteLog_Biller(logMessage + "Error in minusChannelPlusAmt : " + errMsg);
        }

        Utils.WriteLog_Biller(logMessage + "After Update Error With Add Balance| TxnID:" + txnID + "|agentID:" + agentID + "AvailableBalance:" + availableBalance + "LedgerBalance:" + ledgerBalance);
        return Utils.getErrorRes(rescode, resdesc);
    }

}