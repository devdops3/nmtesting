using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using A2AAPIWCF;
using System.Configuration;
using Newtonsoft.Json;
using MessagingService;

/// <summary>
/// Summary description for TelcoGETManager
/// </summary>
public class TelCoGETManager
{
    string errorMessage;
    int batchID;
    private readonly ServiceClient _desktopWCF;
    private readonly MobileAPIWCFManager _mobileApiWcf;
    private readonly GETIntegrationService _getIntegrationService;

    public TelCoGETManager()
	{
        _desktopWCF = new ServiceClient();
        _mobileApiWcf = new MobileAPIWCFManager();
        _getIntegrationService = new GETIntegrationService();
	}

    public string ProcessELoad(ConfirmResponseModel confirmResponseModel, ELoadProcessRequestModel eLoadProcessRequestModel)
    {
        var logAppender = "ProcessELoadForGET | " + eLoadProcessRequestModel.MessageId + " | ";
        Utils.WriteLog_Biller(logAppender + "This is " + eLoadProcessRequestModel.BillerCode + " ELoad for GET");

        AuthenticationModel authenticationResponse = _getIntegrationService.GetToken();

        if (authenticationResponse == null)
        {
            var responseDescription = "Authentication Failed";
            var responseCode = NearMeResponseCode.AuthFailed;
            Utils.WriteLog_Biller(logAppender + responseDescription);
            return _mobileApiWcf.GetErrorResponseWithAddBalance(responseCode, responseDescription, eLoadProcessRequestModel.TxnId, responseDescription,
                eLoadProcessRequestModel.AgentId, eLoadProcessRequestModel.AgentAmount, eLoadProcessRequestModel.IsAgreement);
        }

        var eLoadRequest = new ELoadRequest()
        {
            PhoneNumber = eLoadProcessRequestModel.MobileNumber,
            OrderId = "NearMe" + eLoadProcessRequestModel.TxnId,
            Amount = int.Parse(eLoadProcessRequestModel.Amount)
        };

        string jsonRequest = JsonConvert.SerializeObject(eLoadRequest);
        Utils.WriteLog_Biller(logAppender + "Json Request:" + jsonRequest);

        ELoadResponse eLoadResponse = _getIntegrationService.ProcessForELoad(authenticationResponse.Authtoken, authenticationResponse.AuthId, eLoadRequest);

        string jsonResponse = JsonConvert.SerializeObject(eLoadResponse);
        Utils.WriteLog_Biller(logAppender + "Json Response:" + jsonResponse);

        if (!IsELoadResponseSuccess(eLoadResponse))
        {
            var responseDescription = (string.IsNullOrEmpty(jsonResponse)) ? "Failed" : eLoadResponse.Description;
            var responseCode = NearMeResponseCode.Failed;
            return _mobileApiWcf.GetErrorResponseWithAddBalance(responseCode, responseDescription, eLoadProcessRequestModel.TxnId,
                responseDescription, eLoadProcessRequestModel.AgentId, eLoadProcessRequestModel.AgentAmount, eLoadProcessRequestModel.IsAgreement);
        }

        var errMsg = string.Empty;

        confirmResponseModel.ref4 = eLoadProcessRequestModel.TxnId.ToString()+" "+DateTime.Now.ToShortDateString();
        confirmResponseModel.ref5 = eLoadProcessRequestModel.MobileNumber;
        confirmResponseModel.ref3 = "Airtime";
        if (!ConfirmUpdate(eLoadProcessRequestModel.TxnId, confirmResponseModel, eLoadProcessRequestModel.MobileNumber, eLoadProcessRequestModel.AgentId, eLoadProcessRequestModel.AgentAmount,
            eLoadProcessRequestModel.AgentFeeDbl, eLoadProcessRequestModel.IsAgreement, eLoadProcessRequestModel.SmsStatus, eLoadProcessRequestModel.AvailableBalance, 
            logAppender, out errMsg))
        {
            return UpdateError(eLoadProcessRequestModel.TxnId, errMsg, logAppender);
        }

        Utils.WriteLog_Biller(logAppender + "After update = AgentAmount : " + eLoadProcessRequestModel.AgentAmount + " | Balance : " + eLoadProcessRequestModel.AvailableBalance.ToString() + "| smsStatus:" + eLoadProcessRequestModel.SmsStatus);

        #region <-- Response Back To Client -->
        confirmResponseModel.rescode = NearMeResponseCode.Success;
        confirmResponseModel.resdesc = "Success";
        confirmResponseModel.availablebalance = eLoadProcessRequestModel.AvailableBalance.ToString();
        confirmResponseModel.txnID = eLoadProcessRequestModel.TxnId.ToString();

        return Utils.getConfirmRes(confirmResponseModel);
        #endregion
    }

    public string ProcessEPin(ConfirmResponseModel confirmResponseModel, EPinProcessRequestModel ePinProcessRequestModel)
    {
        var logAppender = "ProcessEPinForGET | " + ePinProcessRequestModel.MessageId + " | ";
        Utils.WriteLog_Biller(logAppender + "This is " + ePinProcessRequestModel.BillerCode + " EPin for GET");

        AuthenticationModel authenticationResponse = _getIntegrationService.GetToken();

        if (authenticationResponse == null)
        {
            var responseDescription = "Authentication Failed";
            var responseCode = NearMeResponseCode.AuthFailed;
            Utils.WriteLog_Biller(logAppender + responseDescription);
            return _mobileApiWcf.GetErrorResponseWithAddBalance(responseCode, responseDescription, ePinProcessRequestModel.TxnId, responseDescription,
                ePinProcessRequestModel.AgentId, ePinProcessRequestModel.AgentAmount, ePinProcessRequestModel.IsAgreement);
        }

        string productCode = PopulateProductCode(ePinProcessRequestModel.BillerCode, ePinProcessRequestModel.Amount);        

        var ePinRequest = new EPinRequest()
        {
            OrderId = "NearMe" + ePinProcessRequestModel.TxnId,
            ProductCode = productCode
        };

        string jsonRequest = JsonConvert.SerializeObject(ePinRequest);
        Utils.WriteLog_Biller(logAppender + "Json Request:" + jsonRequest);

        EPinResponse ePinResponse = _getIntegrationService.ProcessForEPin(authenticationResponse.Authtoken, authenticationResponse.AuthId, ePinRequest);

        string jsonResponse = JsonConvert.SerializeObject(ePinResponse);
        Utils.WriteLog_Biller(logAppender + "Json Response:" + jsonResponse);

        if (!IsEPinResponseSuccess(ePinResponse))
        {
            var responseDescription = (string.IsNullOrEmpty(jsonResponse)) ? "Failed" : ePinResponse.Description;
            var responseCode = NearMeResponseCode.Failed;
            return _mobileApiWcf.GetErrorResponseWithAddBalance(responseCode, responseDescription, ePinProcessRequestModel.TxnId, responseDescription,
                ePinProcessRequestModel.AgentId, ePinProcessRequestModel.AgentAmount, ePinProcessRequestModel.IsAgreement);
        }

        var smsMsg = string.Empty;

        Utils.WriteLog_Biller(logAppender + "$$$$$$$$$$$$$$$$$ EPIN from GET OK $$$$$$$$$$$$$$$$$$$$$$$");

        var ref4 = ePinResponse.Pin + " " + ePinResponse.ExpiryDate;
        ePinProcessRequestModel.Ref3 = ePinResponse.SerialNumber;

        confirmResponseModel.ref1 = ePinProcessRequestModel.Ref1;
        confirmResponseModel.ref2 = ePinProcessRequestModel.Ref2;
        confirmResponseModel.ref3 = ePinProcessRequestModel.Ref3;
        confirmResponseModel.ref4 = ref4;

        var amt = double.Parse((double.Parse(ePinProcessRequestModel.Ref2)).ToString("#,##0.00"));

        #region <-- Update Transaction -->

        var errMsg = string.Empty;
        if (!ConfirmUpdate(ePinProcessRequestModel.TxnId, confirmResponseModel, ePinProcessRequestModel.MobileNumber, ePinProcessRequestModel.AgentId, ePinProcessRequestModel.AgentAmount,
            ePinProcessRequestModel.AgentFeeDbl, ePinProcessRequestModel.IsAgreement, ePinProcessRequestModel.SmsStatus, ePinProcessRequestModel.AvailableBalance, logAppender, out errMsg))
        {
            return UpdateError(ePinProcessRequestModel.TxnId, errMsg, logAppender);
        }

        Utils.WriteLog_Biller(logAppender + "After update = AgentAmount : " + ePinProcessRequestModel.AgentAmount + " | Balance : " + ePinProcessRequestModel.AvailableBalance.ToString() + "| smsStatus:" + ePinProcessRequestModel.SmsStatus);

        #endregion

        ePinProcessRequestModel.Ref3 = Utils.maskString(ePinProcessRequestModel.Ref3);

        smsMsg = SendSMS(ePinProcessRequestModel.AppType, ePinProcessRequestModel.TopupType, ref4, ePinProcessRequestModel.AgentName,
            ePinProcessRequestModel.MapTaxId, ePinProcessRequestModel.BillerName, ePinProcessRequestModel.Ref3, ePinProcessRequestModel.Amount,
            ePinProcessRequestModel.BranchCode, logAppender, ePinProcessRequestModel.MobileNumber, ePinProcessRequestModel.SenderName, ePinProcessRequestModel.TxnId);

        #region <-- Response Back To Client -->
        var confirmResponse = CreateConfirmResponse(ePinProcessRequestModel.MapTaxId, ePinProcessRequestModel.Email, ePinProcessRequestModel.Password,
            ePinProcessRequestModel.MessageId, ePinProcessRequestModel.BillerName, ePinProcessRequestModel.BillerLogo, ePinResponse.Status, ePinResponse.Description,
            ePinProcessRequestModel.Ref1, ePinProcessRequestModel.Ref2, ePinProcessRequestModel.Ref3, ref4, ePinProcessRequestModel.MobileNumber, ePinProcessRequestModel.Ref1Name, ePinProcessRequestModel.Ref2Name,
            ePinProcessRequestModel.Ref3Name, ePinProcessRequestModel.Ref4Name, ePinProcessRequestModel.Ref5Name, ePinProcessRequestModel.AvailableBalance, ePinProcessRequestModel.TxnId,
            ePinProcessRequestModel.TodayTxnAmount, ePinProcessRequestModel.TodayTxnCount, smsMsg);
        return Utils.getConfirmRes(confirmResponse);
        #endregion

    }

    public List<DataPackDetail> InquiryForDataPack(string telCoProvider)
    {
        List<DataPackDetail> dataPacks = new List<DataPackDetail>();
        List<DataPackDetail> dataPackResult = new List<DataPackDetail>();
        var logAppender = "InquiryForDataPack | ";
        Utils.WriteLog_Biller(logAppender + "This is Inquiry request for DataPack for GET");

        // Get DataPackages from Cache
        string dataPacksString = CacheService.GetData(CacheKeyConsts.GETDataPackKey);
        if (!string.IsNullOrWhiteSpace(dataPacksString))
        {
            dataPacks = JsonConvert.DeserializeObject<List<DataPackDetail>>(dataPacksString);
        }
        else
        {
            AuthenticationModel authenticationResponse = _getIntegrationService.GetToken();

            if (authenticationResponse == null)
            {
                var responseDescription = "Authentication Failed";
                var responseCode = NearMeResponseCode.AuthFailed;
                Utils.WriteLog_Biller(logAppender + responseDescription);                
            }

            Utils.WriteLog_Biller(logAppender + "Authenticated!");
            dataPacks = _getIntegrationService.RequestDataPackageList(authenticationResponse.Authtoken,authenticationResponse.AuthId);
            if (dataPacks == null || dataPacks.Count == 0)
            {
                Utils.WriteLog_Biller(logAppender + "Something wrong. Value is null!");
                return dataPacks;
            }

            string expireString = ConfigurationManager.AppSettings["RedisDataPackageExpireSeconds"].ToString();
            string dataPackagesString = JsonConvert.SerializeObject(dataPacks);
            bool result = CacheService.SaveData(CacheKeyConsts.GETDataPackKey, dataPackagesString, int.Parse(expireString));
            Utils.WriteLog_Biller(logAppender + "Result DataPackages have been saved to cache : " + result);
        }

        dataPackResult = dataPacks.Where(dp => dp.OperatorName.Contains(telCoProvider)).ToList();
        return dataPackResult;
    }

    public string ProcessDataPackage(ConfirmResponseModel confirmResponseModel, DataPackProcessRequestModel dataPackProcessRequestModel)
    {
        var logAppender = "ProcessDataPackageForGET | " + dataPackProcessRequestModel.MessageId + " | ";
        Utils.WriteLog_Biller(logAppender + "This is BillerCode - " + dataPackProcessRequestModel.BillerCode );

        AuthenticationModel authenticationResponse = _getIntegrationService.GetToken();

        if (authenticationResponse == null)
        {
            var responseDescription = "Authentication Failed";
            var responseCode = NearMeResponseCode.AuthFailed;
            Utils.WriteLog_Biller(logAppender + responseDescription);
            return _mobileApiWcf.GetErrorResponseWithAddBalance(responseCode, responseDescription, dataPackProcessRequestModel.TxnId, responseDescription,
                dataPackProcessRequestModel.AgentId, dataPackProcessRequestModel.AgentAmount, dataPackProcessRequestModel.IsAgreement);
        }

        var dataPackRequest = new DataPackRequest()
        {
            PackageCode = dataPackProcessRequestModel.PackageCode,
            PhoneNumber = dataPackProcessRequestModel.MobileNumber,
            OrderId = "NearMe" + dataPackProcessRequestModel.TxnId
        };

        string jsonRequest = JsonConvert.SerializeObject(dataPackRequest);
        Utils.WriteLog_Biller(logAppender + "Json Request:" + jsonRequest);

        DataPackResponse dataPackResponse = _getIntegrationService.ProcessForDataPackage(authenticationResponse.Authtoken, authenticationResponse.AuthId, dataPackRequest);

        string jsonResponse = JsonConvert.SerializeObject(dataPackResponse);
        Utils.WriteLog_Biller(logAppender + "Json Response:" + jsonResponse);

        if (!IsDataPackageResponseSuccess(dataPackResponse))
        {
            var responseDescription = (string.IsNullOrEmpty(jsonResponse)) ? "Failed" : dataPackResponse.Description;
            var responseCode = NearMeResponseCode.Failed;
            return _mobileApiWcf.GetErrorResponseWithAddBalance(responseCode, responseDescription, dataPackProcessRequestModel.TxnId,
                responseDescription, dataPackProcessRequestModel.AgentId, dataPackProcessRequestModel.AgentAmount, dataPackProcessRequestModel.IsAgreement);
        }

        var errMsg = string.Empty;

        confirmResponseModel.ref1 = GetRef1ForUpdate(confirmResponseModel.taxID,confirmResponseModel.ref1);
        confirmResponseModel.ref2 = GetRef2ForDataPack(confirmResponseModel.taxID, confirmResponseModel.ref2, dataPackProcessRequestModel.Amount);
        confirmResponseModel.ref3 = "Airtime";
        confirmResponseModel.ref5 = dataPackProcessRequestModel.MobileNumber;        

        if (!ConfirmUpdate(dataPackProcessRequestModel.TxnId, confirmResponseModel, dataPackProcessRequestModel.MobileNumber, dataPackProcessRequestModel.AgentId, dataPackProcessRequestModel.AgentAmount,
            dataPackProcessRequestModel.AgentFeeDbl, dataPackProcessRequestModel.IsAgreement, dataPackProcessRequestModel.SmsStatus, dataPackProcessRequestModel.AvailableBalance,
            logAppender, out errMsg))
        {
            return UpdateError(dataPackProcessRequestModel.TxnId, errMsg, logAppender);
        }

        Utils.WriteLog_Biller(logAppender + "After update = AgentAmount : " + dataPackProcessRequestModel.AgentAmount + " | Balance : " + dataPackProcessRequestModel.AvailableBalance.ToString() + "| smsStatus:" + dataPackProcessRequestModel.SmsStatus);

        #region <-- Response Back To Client -->
        confirmResponseModel.ref1 = GetRef1ForResponse(confirmResponseModel.taxID);
        confirmResponseModel.rescode = NearMeResponseCode.Success;
        confirmResponseModel.resdesc = "Success";
        confirmResponseModel.availablebalance = dataPackProcessRequestModel.AvailableBalance.ToString();
        confirmResponseModel.txnID = dataPackProcessRequestModel.TxnId.ToString();

        return Utils.getConfirmRes(confirmResponseModel);
        #endregion
    }

    #region Private Methods

    private bool ConfirmUpdate(long txnID, ConfirmResponseModel confirmResponseModel, string phoneNo, int agentId, double agentAmount,
        double agentFeeDbl, string isAgreement, string smsStatus, double availablebalance, string logAppender, out string errMsg)
    {
        var retryService = new RetryService<Exception>();
        var policy = retryService.CreatePolicy(3, 1, false, logAppender);
        errMsg = string.Empty;

        bool result = policy.Execute(() => _desktopWCF.ConfirmUpdate(txnID, confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.ref4,
                  phoneNo, "", TxnStatusConstants.Paid, "Paid Successfully", agentId, confirmResponseModel.email, agentAmount, agentFeeDbl,
                  isAgreement, smsStatus, availablebalance, out errorMessage, out batchID));

        if (!result)
        {
            errMsg = errorMessage;
        }

        return result;
    }

    private string UpdateError(long txnID, string errMsg, string logAppender)
    {
        Utils.WriteLog_Biller(logAppender + " Error in ConfirmUpdate : " + errMsg);
        var responseDescription = "Error in update database";
        var responseCode = NearMeResponseCode.InvalidReq;
        if (!_desktopWCF.updateError(txnID, TxnStatusConstants.Error, responseDescription, out errMsg))
        {
            Utils.WriteLog_Biller(logAppender + "Error in updateError : " + errMsg);
        }
        return Utils.getErrorRes(responseCode, "Transaction fail");
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
        responseCode = NearMeResponseCode.Success;
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

    private string PopulateProductCode(string billerCode, string amount)
    {
        string productCode = string.Empty;
        string operatorName = (billerCode.Remove(billerCode.Length - 4, 4)).ToUpper();
        if (operatorName == "TELENOR") operatorName = "ATOM";

        productCode = "epin." + operatorName + "." + amount;
        return productCode;
    }

    private bool IsELoadResponseSuccess(ELoadResponse eLoadResponse)
    {
        if (eLoadResponse == null || eLoadResponse.Status != TelCoStatusEnum.Success.ToString()) return false;
        return true;
    }

    private bool IsEPinResponseSuccess(EPinResponse ePinResponse)
    {
        if (ePinResponse == null || ePinResponse.Status != TelCoStatusEnum.Success.ToString()) return false;
        return true;
    }

    private bool IsDataPackageResponseSuccess(DataPackResponse dataPackResponse)
    {
        if (dataPackResponse == null || dataPackResponse.Status != TelCoStatusEnum.Success.ToString()) return false;
        return true;
    }

    private string GetRef1ForResponse(string taxId)
    {
        var mptDataPackTaxId = ConfigurationManager.AppSettings["MptDataPackage"].ToString();
        var ooredooDataPackTaxId = ConfigurationManager.AppSettings["OoredooDataPackage"].ToString();
        var myTelDataPackTaxId = ConfigurationManager.AppSettings["MyTelDataPackage"].ToString();
        var atomDataPackTaxId = ConfigurationManager.AppSettings["AtomDataPackTaxId"].ToString();

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
        if (taxId == atomDataPackTaxId)
        {
            return "GSM";
        }

        return "Unknown Biller";
    }

    private string GetRef1ForUpdate(string taxId, string ref1)
    {
        var mptDataPackTaxId = ConfigurationManager.AppSettings["MptDataPackage"].ToString();
        var ooredooDataPackTaxId = ConfigurationManager.AppSettings["OoredooDataPackage"].ToString();
        var myTelDataPackTaxId = ConfigurationManager.AppSettings["MyTelDataPackage"].ToString();
        var atomDataPackTaxId = ConfigurationManager.AppSettings["AtomDataPackTaxId"].ToString();

        if (taxId == mptDataPackTaxId || taxId == ooredooDataPackTaxId || taxId == atomDataPackTaxId)
        {
            return ref1;
        }
        if (taxId == myTelDataPackTaxId)
        {
            return "(MyTel-GSM)";
        }

        return "unknown Biller";
    }

    //Specific for ATOM Datapack
    private string GetRef2ForDataPack(string taxId, string ref2, string amount)
    {
        var atomDataPackTaxId = ConfigurationManager.AppSettings["AtomDataPackTaxId"].ToString();

        if (taxId == atomDataPackTaxId)
        {
            return amount;
        }

        return ref2;
    }
    #endregion
}