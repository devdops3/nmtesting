using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Configuration;
using A2AAPIWCF;
using System.Web.Script.Serialization;
using Polly;
using System.ServiceModel;
using MessagingService;
using Polly.Retry;
using System.Threading.Tasks;

/// <summary>
/// Summary description for TelcoEBAManager
/// </summary>
public class TelCoEBAManager
{
    string errorMessage;
    int batchID;
    private string mobileNo = "";
    private readonly ServiceClient _desktopWCF;
    private readonly MobileAPIWCFManager _mobileApiWcf;

    public TelCoEBAManager()
    {
        _desktopWCF = new ServiceClient();
        _mobileApiWcf = new MobileAPIWCFManager();
    }

    public string ConfrimELoadToEBA(ConfirmResponseModel confirmResponseModel, string amount, int agentId, long txnID,
       double agentAmount, string isAgreement, double agentFeeDbl, string smsStatus, double availablebalance,
      string billerCode, string messageId)
    {
        var logAppender = "ConfrimELoadToEBA | " + messageId + " | ";

        Utils.WriteLog_Biller(logAppender + "This is " + billerCode + " ELoad EBA Confirm");
        string ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();

        var token = TokenManager.GetOAuthToken();

        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = confirmResponseModel.txnID.ToString(),
            BillerCode = billerCode,
            TransactionAmount = amount,
            Detail = "{'Deno':'" + confirmResponseModel.ref2 + "', 'MobileNumber':'" + confirmResponseModel.ref3 + "'}"
        };

        string jsonReq = JsonConvert.SerializeObject(confirmReq);
        Utils.WriteLog_Biller(logAppender + "Json Request:" + jsonReq);

        string jsonres = Utils.PaymentRequest(jsonReq, ebaUrl);
        Utils.WriteLog_Biller(logAppender + "Json Response:" + jsonres);

        if (string.IsNullOrEmpty(jsonres))
        {
            var responseDescription = "No Response From EBA";
            var responseCode = "06";
            return _mobileApiWcf.GetErrorResponseWithAddBalance(responseCode, responseDescription, txnID, responseDescription, agentId, agentAmount, isAgreement);
        }

        var confirmRes = JsonConvert.DeserializeObject<EbaConfirmRes>(jsonres);
        var errMsg = string.Empty;

        if (!IsSuccess(confirmRes.TransactionStatus))
        {
            var responseDescription = Utils.EsbResponseDescription(confirmRes.ErrorCode);
            return _mobileApiWcf.GetErrorResponseWithAddBalance(confirmRes.ErrorCode, confirmRes.ErrorMessage,
                txnID, responseDescription, agentId, agentAmount, isAgreement);
        }

        confirmResponseModel.ref4 = txnID.ToString() + ":" + confirmRes.EBARefNo + " " + DateTime.Now.ToShortDateString();
        var phoneNo = confirmResponseModel.ref3;
        confirmResponseModel.ref5 = phoneNo;
        confirmResponseModel.ref3 = "Airtime";
        if (!ConfirmUpdate(txnID, confirmResponseModel, phoneNo, agentId, agentAmount, agentFeeDbl, isAgreement, smsStatus, availablebalance, logAppender, out errMsg))
        {
            return UpdateError(txnID, errMsg, logAppender);
        }

        Utils.WriteLog_Biller(logAppender + "After update = AgentAmount : " + agentAmount + " | Balance : " + availablebalance.ToString() + "| smsStatus:" + smsStatus);

        #region <-- Response Back To Client -->
        confirmResponseModel.rescode = "00";
        confirmResponseModel.resdesc = "Success";
        confirmResponseModel.availablebalance = availablebalance.ToString();
        confirmResponseModel.txnID = txnID.ToString();

        return Utils.getConfirmRes(confirmResponseModel);

        #endregion
    }

    public string ConfrimEPinToEBA(ConfirmResponseModel confirmResponseModel, string amount, int agentId, long txnID,
       double agentAmount, string isAgreement, double agentFeeDbl, string smsStatus, double availablebalance,
       string appType, string topupType, string agentName, string MapTaxID, string branchCode, string senderName,
       string billerCode, string messageId, string ref1, string ref2, string ref3,
       string email, string billerName, string password, string ref1Name, string ref2Name, string ref3Name,
        string ref4Name, string ref5Name, string billerLogo, string todayTxnAmount, string todayTxnCount)
    {
        var logAppender = "ConfrimEPinToEBA | " + messageId + " | ";
        Utils.WriteLog_Biller(logAppender + "This is " + billerCode + " EPin EBA Confirm");
        string ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();

        var token = TokenManager.GetOAuthToken();

        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = confirmResponseModel.txnID.ToString(),
            BillerCode = billerCode,
            TransactionAmount = amount,
            Detail = "{'Deno':'" + confirmResponseModel.ref2 + "'}"
        };

        string jsonReq = JsonConvert.SerializeObject(confirmReq);
        Utils.WriteLog_Biller(logAppender + "Json Request:" + jsonReq);

        string jsonres = Utils.PaymentRequest(jsonReq, ebaUrl);
        Utils.WriteLog_Biller(logAppender + "Json Response:" + jsonres);

        var responseDescription = string.Empty;
        var responseCode = string.Empty;
        if (string.IsNullOrEmpty(jsonres))
        {
            responseDescription = "No Response From EBA";
            responseCode = "06";
            return _mobileApiWcf.GetErrorResponseWithAddBalance(responseCode, responseDescription, txnID, responseDescription, agentId, agentAmount, isAgreement);
        }

        var serializer = new JavaScriptSerializer();
        var ebaConfirmResponse = serializer.Deserialize<EbaConfirmRes>(jsonres);
        var smsMsg = string.Empty;

        if (!IsSuccess(ebaConfirmResponse.TransactionStatus))
        {
            responseDescription = Utils.EsbResponseDescription(ebaConfirmResponse.TransactionStatus);
            return _mobileApiWcf.GetErrorResponseWithAddBalance(ebaConfirmResponse.TransactionStatus, "Out of Stock", txnID, responseDescription, agentId, agentAmount, isAgreement);
        }

        var pinResponse = serializer.Deserialize<TelComEPinDetailResponse>(ebaConfirmResponse.Detail);
        Utils.WriteLog_Biller(logAppender + "$$$$$$$$$$$$$$$$$ GET TELENORPIN FROM ESBA OK $$$$$$$$$$$$$$$$$$$$$$$");

        string aesKey = ConfigurationManager.AppSettings["EsbaAesKey"].ToString();
        pinResponse.ClearPin = Utils.AESDecryptText(pinResponse.ClearPin, aesKey);
        var mobileNo = ref3;
        var ref4 = pinResponse.ClearPin + " " + pinResponse.ExpiryDate;
        ref3 = pinResponse.SerialNumber;

        confirmResponseModel.ref1 = ref1;
        confirmResponseModel.ref2 = ref2;
        confirmResponseModel.ref3 = ref3;
        confirmResponseModel.ref4 = ref4;

        var amt = double.Parse((double.Parse(ref2)).ToString("#,##0.00"));

        #region <-- Update Transaction -->

        var errMsg = string.Empty;
        if (!ConfirmUpdate(txnID, confirmResponseModel, mobileNo, agentId, agentAmount, agentFeeDbl, isAgreement, smsStatus, availablebalance, logAppender, out errMsg))
        {
            return UpdateError(txnID, errMsg, logAppender);
        }

        Utils.WriteLog_Biller(logAppender + "After update = AgentAmount : " + agentAmount + " | Balance : " + availablebalance.ToString() + "| smsStatus:" + smsStatus);

        #endregion

        ref3 = Utils.maskString(ref3);

        smsMsg = SendSMS(appType, topupType, ref4, agentName, MapTaxID, billerName, ref3, amount, branchCode, logAppender, mobileNo, senderName, txnID);

        #region <-- Response Back To Client -->
        var confirmResponse = CreateConfirmResponse(MapTaxID, email, password, messageId, billerName, billerLogo, responseCode, responseDescription, ref1, ref2, ref3, ref4, mobileNo, ref1Name, ref2Name,
            ref3Name, ref4Name, ref5Name, availablebalance, txnID, todayTxnAmount, todayTxnCount, smsMsg);
        return Utils.getConfirmRes(confirmResponse);
        #endregion

    }

    public string ConfirmDataPack(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo)
    {
        var logAppender = "TelCoEBAManager:ConfirmDataPack | " + confirmResponseModel.messageid + " | ";
        string ref3 = string.Empty;
        string rescode = string.Empty;
        string resdecs = string.Empty;

        //To confirm for switch case

        #region <-- Atom Data Package -->
        if (responseInfo.MapTaxID == ConfigurationManager.AppSettings["TelenorPackageTaxId"].ToString())
        {
            Utils.WriteLog_Biller(logAppender + "This is Telenor data package from EBA");
            return ConfirmAtomDataPack(confirmResponseModel, responseInfo);
        }
        #endregion

        #region <-- MPT, Ooredoo, MyTel Data Package -->
        else
        {
            string code = GetDataPackCode(confirmResponseModel);
            Utils.WriteLog_Biller(logAppender + "This is data package from EBA for " + confirmResponseModel.billername);
            return ConfirmDataPackage(confirmResponseModel, responseInfo, code);
        }
        #endregion
    }

    private string GetDataPackCode(ConfirmResponseModel confirmResponse)
    {
        var myTelDataPackTaxId = ConfigurationManager.AppSettings["MyTelDataPackage"].ToString();
        if (confirmResponse.taxID == myTelDataPackTaxId)
        {
            return confirmResponse.ref4;
        }
        else
        {
            return confirmResponse.ref1;
        }
    }

    #region Private Methods
    private bool IsSuccess(string statusCode)
    {
        return statusCode == "0";
    }

    private bool ConfirmUpdate(long txnID, ConfirmResponseModel confirmResponseModel, string phoneNo, int agentId, double agentAmount,
        double agentFeeDbl, string isAgreement, string smsStatus, double availablebalance, string logAppender, out string errMsg)
    {
        var retryService = new RetryService<Exception>();
        var policy = retryService.CreatePolicy(3, 1, false, logAppender);
        errMsg = string.Empty;

        bool result = policy.Execute(() => _desktopWCF.ConfirmUpdate(txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                  phoneNo, "", "PA", "Paid Successfully", agentId, confirmResponseModel.email, agentAmount, agentFeeDbl,
                  isAgreement, smsStatus, availablebalance, out errorMessage, out batchID));

        if (!result)
        {
            errMsg = errorMessage;
        }

        return result;
    }

    private string SendSMS(string appType, string topupType, string ref4, string agentName, string MapTaxID, string billerName
        , string ref3, string amount, string branchCode, string logAppender, string mobileNo, string senderName, long txnID)
    {
        if ((appType == "CS" || appType == "MS") && (string.IsNullOrEmpty(topupType) || topupType == "S"))
        {
            var smsHelper = new SMSHelper();
            var smsWcf = new MessagingServiceClient();
            string[] words = ref4.Split(' ');
            string PIN = words[0].ToString();
            string Expiry = words[1].ToString();

            var smsMsg = smsHelper.getMessageTopup(agentName, MapTaxID, billerName, PIN, ref3, Expiry, double.Parse(amount).ToString("#,###.00"), branchCode);

            try
            {
                Utils.WriteLog_Biller(logAppender + "sendSMSWithTxnID starts.");
                Utils.WriteLog_Biller(logAppender + "Mobile No :" + mobileNo + "| Message :" + smsMsg + "| Sender Name :" + senderName + "|txn ID :" + txnID);
                smsWcf.SendSms(txnID.ToString(), smsMsg, mobileNo, senderName);
                Utils.WriteLog_Biller(logAppender + "sendSMSWithTxnID ends.");
            }
            catch (Exception exception)
            {
                Utils.WriteLog_Biller(logAppender + "Exception : " + exception.Message);
            }
            return smsMsg;
        }

        return string.Empty;
    }

    private ConfirmResponseModel CreateConfirmResponse(string MapTaxID, string email, string password, string messageId, string billerName,
        string billerLogo, string responseCode, string responseDescription, string ref1, string ref2, string ref3, string ref4, string mobileNo,
        string ref1Name, string ref2Name, string ref3Name, string ref4Name, string ref5Name, double availableBalance, long txnID, string todayTxnAmount,
        string todayTxnCount, string smsMsg)
    {
        responseCode = "00";
        responseDescription = "Success";
        var confirmResponse = new ConfirmResponseModel
        {
            taxID = MapTaxID,
            email = email,
            password = password,
            messageid = messageId,
            billername = billerName,
            billerlogo = billerLogo,
            rescode = responseCode,
            resdesc = responseDescription,
            ref1 = ref1,
            ref2 = ref2,
            ref3 = ref3,
            ref4 = ref4,
            ref5 = mobileNo,
            ref1Name = ref1Name,
            ref2Name = ref2Name,
            ref3Name = ref3Name,
            ref4Name = ref4Name,
            ref5Name = ref5Name,
            availablebalance = availableBalance.ToString(),
            txnID = txnID.ToString(),
            TodayTxnAmount = todayTxnAmount,
            TodayTxnCount = todayTxnCount,
            smsMsg = smsMsg
        };
        return confirmResponse;
    }

    private string UpdateError(long txnID, string errMsg, string logAppender)
    {
        Utils.WriteLog_Biller(logAppender + " Error in ConfirmUpdate : " + errMsg);
        var responseDescription = "Error in update database";
        var responseCode = "06";
        if (!_desktopWCF.updateError(txnID, "ER", responseDescription, out errMsg))
        {
            Utils.WriteLog_Biller(logAppender + "Error in updateError : " + errMsg);
        }
        return Utils.getErrorRes(responseCode, "Transaction fail");
    }

    private string PopulateConfirmRequest(ConfirmResponseModel confirmResponse, string amount, string billerCode)
    {
        Utils.WriteLog_Biller(confirmResponse.messageid + " | Start Populating Atom Datapack Confirm Request");
        var token = TokenManager.GetOAuthToken();
        var confirmReq = new EbaConfirmReq()
        {
            Token = token.Token,
            PartnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString(),
            PartnerRefNo = confirmResponse.txnID.ToString(),
            BillerCode = billerCode,
            TransactionAmount = amount,
            Detail = "{'Deno':'" + confirmResponse.ref2
                        + "', 'MobileNumber':'" + confirmResponse.ref3 + "'}"
        };
        var jsonReqest = JsonConvert.SerializeObject(confirmReq);
        Utils.WriteLog_Biller(confirmResponse.messageid + " | Atom Datapack EBA Json Confirm Request : " + jsonReqest);
        Utils.WriteLog_Biller(confirmResponse.messageid + " | End Populating Atom Datapack Confirm Request");
        return jsonReqest;
    }

    private string ConfirmEBA(string confirmReq, string messageId)
    {
        string ebaUrl = ConfigurationManager.AppSettings["EbaConfirmUrl"].ToString();

        SSLPost post = new SSLPost();
        string jsonres = Utils.PostEba(confirmReq, ebaUrl);
        Utils.WriteLog_Biller(messageId + "Atom DataPack EBA Confirm Jason Response:" + jsonres);

        return jsonres;

    }

    private ConfirmMptDataPackageResponse Confirm(string txnId, double amount, string packageCode, string phoneNumber, string taxId)
    {
        var token = TokenManager.GetOAuthToken();
        var partnerCode = ConfigurationManager.AppSettings["EsbaChannel"].ToString();
        var ebaUrl = ConfigurationManager.AppSettings["MptDataPackageConfirmUrl"].ToString();
        var partnerRefNo = txnId;
        var billerCode = string.Empty;
        dynamic dynamicData;

        if (taxId == ConfigurationManager.AppSettings["OoredooDataPackage"].ToString())
        {
            billerCode = ConfigurationManager.AppSettings["OoredooBillerCode"].ToString();
            const string jsonData = "{\'MSISDN\':\'\',\'OfferId\':\'\'}";
            dynamicData = JsonConvert.DeserializeObject<dynamic>(jsonData);
            dynamicData.MSISDN = phoneNumber;
            dynamicData.OfferId = packageCode;
        }

        else if (taxId == ConfigurationManager.AppSettings["MyTelDataPackage"].ToString())
        {
            billerCode = ConfigurationManager.AppSettings["MyTelBillerCode"].ToString();
            const string jsonData = "{\'Package\':\'\',\'Deno\':\'\',\'MobileNumber\':\'\'}";
            dynamicData = JsonConvert.DeserializeObject<dynamic>(jsonData);
            dynamicData.Package = packageCode;
            dynamicData.Deno = amount.ToString("0.0");
            dynamicData.MobileNumber = phoneNumber;
        }

        else
        {
            billerCode = ConfigurationManager.AppSettings["MptBillerCode"].ToString();
            const string jsonData = "{\'endUserId\':\'\',\'packageCode\':\'\',\'Price\':\'\'}";
            dynamicData = JsonConvert.DeserializeObject<dynamic>(jsonData);
            dynamicData.endUserId = phoneNumber;
            dynamicData.packageCode = packageCode;
            dynamicData.Price = amount.ToString("0.0");
        }


        var data = JsonConvert.SerializeObject(dynamicData);
        data = data.Replace("\"", "'");
        var model = new ConfirmMptDataPackageResquest(token.Token, partnerCode, partnerRefNo, billerCode, amount, data);
        var json = JsonConvert.SerializeObject(model);
        Utils.WriteLog_Biller("Eba Confirm request for " + billerCode + " : " + json);
        var response = Utils.PostEba(json, ebaUrl);

        Utils.WriteLog_Biller("Eba Confirm response for " + billerCode + " : " + response);

        if (string.IsNullOrEmpty(response)) return null;

        return JsonConvert.DeserializeObject<ConfirmMptDataPackageResponse>(response);
    }

    private string GetErrorResponseWithAddBalance(string rescode, string resdesc, long txnID, string logerrormessage, int agentID, double amount, string isAgreement, string taxId = "")
    {
        Utils.WriteLog_Biller("Update Error With Add Balance");
        string errMsg = string.Empty;
        double availableBalance = 0;
        double ledgerBalance = 0;
        if (!_desktopWCF.UpdateErrorWithAddingBalance(txnID, "ER", resdesc, agentID, amount, isAgreement, out errMsg, out availableBalance, out ledgerBalance))
        {
            Utils.WriteLog_Biller("Error in update Error with Add Balance : " + errMsg);
        }
        Utils.WriteLog_Biller("After Update Error With Add Balance| TxnID:" + txnID + "|agentID:" + agentID + "AvailableBalance:" + availableBalance + "LedgerBalance:" + ledgerBalance);
        //Processing is failed because of internal server error, please try again
        return Utils.getErrorRes(rescode, resdesc, taxId);
    }

    private string ConfirmDataPackage(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, string code)
    {
        string rescode = string.Empty;
        string resdecs = string.Empty;
        string errMsg = string.Empty;


        var mobileNo = confirmResponseModel.ref3;
        confirmResponseModel.ref5 = mobileNo;
        confirmResponseModel.ref3 = "Airtime";
        var amount = Convert.ToDouble(confirmResponseModel.ref2);

        var logAppender = "TelCoEBAManager:ConfirmDataPackage | " + confirmResponseModel.messageid + " | " + confirmResponseModel.billername + " | ";
        Utils.WriteLog_Biller(logAppender + "Start");

        var ebaResponse = Confirm(responseInfo.txnID.ToString(), amount, code,
            mobileNo, responseInfo.MapTaxID);

        if (ebaResponse == null)
        {
            resdecs = "No Response From EBA";
            rescode = "01";
            Utils.WriteLog_Biller(logAppender + "DataPack Confirm Response From EBA is Null");
            return GetErrorResponseWithAddBalance(rescode, resdecs, responseInfo.txnID, resdecs, responseInfo.agentID, responseInfo.agentAmount,
                responseInfo.isAgreement);
        }

        if (IsSuccess(ebaResponse.TransactionStatus))
        {
            #region <-- Update Transaction -->

            var ref1forUpdate = GetRef1ForUpdate(confirmResponseModel.taxID, confirmResponseModel.ref1);

            if (!_desktopWCF.ConfirmUpdate(responseInfo.txnID, ref1forUpdate, confirmResponseModel.ref2,
                confirmResponseModel.ref3, confirmResponseModel.ref4, mobileNo, "", "PA", "Paid Successfully", responseInfo.agentID,
                confirmResponseModel.email, responseInfo.agentAmount, responseInfo.agentFeeDbl, responseInfo.isAgreement, responseInfo.smsStatus,
                responseInfo.availablebalance, out errMsg, out batchID))
            {
                Utils.WriteLog_Biller(logAppender + "Error in ConfirmUpdate : " + errMsg);
                resdecs = "Error in update database";
                rescode = "06";
                if (!_desktopWCF.updateError(responseInfo.txnID, "ER", resdecs, out errMsg))
                {
                    Utils.WriteLog_Biller(logAppender + "Error in updateError : " + errMsg);
                }
                return Utils.getErrorRes(rescode, "Transaction fail");
            }
            else
            {
                Utils.WriteLog_Biller(logAppender + "After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : "
                    + responseInfo.availablebalance.ToString() + "| smsStatus:" + responseInfo.smsStatus);
            }

            #endregion

            Utils.WriteLog_Biller(logAppender + "After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : "
                 + responseInfo.availablebalance.ToString());
            #region <-- Response Back To Client -->


            confirmResponseModel.ref1 = GetRef1ForResponse(confirmResponseModel.taxID);
            confirmResponseModel.rescode = rescode = "00";
            confirmResponseModel.resdesc = "Success";
            confirmResponseModel.availablebalance = responseInfo.availablebalance.ToString();
            confirmResponseModel.txnID = responseInfo.txnID.ToString();
            return Utils.getConfirmRes(confirmResponseModel);
            #endregion
        }

        else
        {
            return GetErrorResponseWithAddBalance(ebaResponse.TransactionStatus,
                                ebaResponse.ErrorMessage, responseInfo.txnID, resdecs, responseInfo.agentID, responseInfo.agentAmount,
                                responseInfo.isAgreement);
        }
    }

    private string GetRef1ForResponse(string taxId)
    {
        var mptDataPackTaxId = ConfigurationManager.AppSettings["MptDataPackage"].ToString();
        var ooredooDataPackTaxId = ConfigurationManager.AppSettings["OoredooDataPackage"].ToString();
        var myTelDataPackTaxId = ConfigurationManager.AppSettings["MyTelDataPackage"].ToString();

        if (taxId == mptDataPackTaxId)
        {
            return "(MPT-GSM)";
        }
        if (taxId == ooredooDataPackTaxId)
        {
            return "(Ooredoo-GSM)";
        }
        if (taxId == myTelDataPackTaxId)
        {
            return "(MyTel-GSM)";
        }

        return "unknown Biller";
    }
    private string GetRef1ForUpdate(string taxId, string ref1)
    {
        var mptDataPackTaxId = ConfigurationManager.AppSettings["MptDataPackage"].ToString();
        var ooredooDataPackTaxId = ConfigurationManager.AppSettings["OoredooDataPackage"].ToString();
        var myTelDataPackTaxId = ConfigurationManager.AppSettings["MyTelDataPackage"].ToString();

        if (taxId == mptDataPackTaxId)
        {
            return ref1;
        }
        if (taxId == ooredooDataPackTaxId)
        {
            return ref1;
        }
        if (taxId == myTelDataPackTaxId)
        {
            return "(MyTel-GSM)";
        }

        return "unknown Biller";
    }

    private string ConfirmAtomDataPack(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo)
    {
        var messageId = confirmResponseModel.messageid + " | ";
        Utils.WriteLog_Biller(messageId + "Start " + responseInfo.billerCode + " Confirm region.");

        var confirmReq = PopulateConfirmRequest(confirmResponseModel, responseInfo.amount, responseInfo.billerCode);
        Utils.WriteLog_Biller(messageId + "Start Calling Atom DataPack Confirm.");
        var jsonres = ConfirmEBA(confirmReq, messageId);
        Utils.WriteLog_Biller(messageId + "End Calling Atom DataPack Confirm.");
        var responseDescription = string.Empty;
        var responseCode = string.Empty;
        if (string.IsNullOrEmpty(jsonres))
        {
            responseDescription = "No Response From EBA";
            responseCode = "99";
            Utils.WriteLog_Biller(messageId + "Atom DataPack Confirm Response From EBA is Null");
            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(responseCode, responseDescription, responseInfo.txnID, responseDescription, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }

        var confirmRes = JsonConvert.DeserializeObject<EbaConfirmRes>(jsonres);
        var errMsg = string.Empty;
        if (!IsSuccess(confirmRes.TransactionStatus))
        {
            return (new MobileAPIWCFManager()).GetErrorResponseWithAddBalance(confirmRes.ErrorCode, confirmRes.ErrorMessage,
                    responseInfo.txnID, responseDescription, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }
        var detail = JsonConvert.DeserializeObject<DataPackDetail>(confirmRes.Detail);
        confirmResponseModel.ref5 = confirmResponseModel.ref3;
        confirmResponseModel.ref3 = "Airtime";

        if (!ConfirmUpdate(responseInfo.txnID, confirmResponseModel, confirmResponseModel.ref5, responseInfo.agentID, responseInfo.agentAmount,
            responseInfo.agentFeeDbl, responseInfo.isAgreement, responseInfo.smsStatus, responseInfo.availablebalance, messageId, out errMsg))
        {
            return UpdateError(responseInfo.txnID, errMsg, messageId);
        }

        Utils.WriteLog_Biller("After update = AgentAmount : " + responseInfo.agentAmount + " | Balance : " + responseInfo.availablebalance.ToString() + "| MessageId : " + messageId);
        confirmResponseModel.rescode = "00";
        confirmResponseModel.resdesc = "Success";
        confirmResponseModel.availablebalance = responseInfo.availablebalance.ToString();
        confirmResponseModel.txnID = responseInfo.txnID.ToString();

        Utils.WriteLog_Biller(messageId + "End " + responseInfo.billerCode + " Confirm region.");
        return Utils.getConfirmRes(confirmResponseModel);
    }
    #endregion
}