using EbaReqResModel;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Configuration;
using A2AAPIWCF;
using System.Web.Script.Serialization;
using Polly;
using System.ServiceModel;
using MessagingService;
using Polly.Retry;
using log4net;

/// <summary>
/// Summary description for TelCoManager
/// </summary>
public class TelCoManager : IeService
{
    private readonly ServiceClient _desktopWCF;

    public TelCoManager()
    {
        _desktopWCF = new ServiceClient();
    }

    public string Inquiry(inquiryModel model)
    {
        throw new NotImplementedException();
    }

    public string Confirm(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo)
    {
        string rescode = string.Empty;
        string resdecs = string.Empty;
        string confirmRes = string.Empty;
        
        var logAppender = confirmResponseModel.messageid + " | " + "txnId : " + responseInfo.txnID + " | taxId : " + responseInfo.MapTaxID + " | ";
        Utils.WriteLog_Biller(logAppender + "This is TelCoManager for" + responseInfo.billerCode + " biller Confirm : messageId :" + confirmResponseModel.messageid);

        BillerTypeEnum billerType = GetBillerType(responseInfo.MapTaxID);
        switch (billerType)
        {
            case BillerTypeEnum.EPIN:
                return ConfirmEPin(confirmResponseModel, responseInfo, logAppender);
            case BillerTypeEnum.ELOAD:
                return ConfirmELoad(confirmResponseModel, responseInfo, logAppender);
            case BillerTypeEnum.DATAPACKAGE:
                return ConfirmDataPack(confirmResponseModel, responseInfo, logAppender);
            default:
                rescode = "06";
                resdecs = "Invalid Biller Service!";
                return GetErrorResponseWithAddBalance(logAppender, rescode, resdecs, responseInfo.txnID, resdecs, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }      
    }

    public string GetErrorResponseWithAddBalance(string logAppender, string rescode, string resdesc, long txnID, string logerrormessage, int agentID, double amount, string isAgreement, string taxId = "")
    {
        Utils.WriteLog_Biller("Update Error With Add Balance");
        string errMsg = string.Empty;
        double availableBalance = 0;
        double ledgerBalance = 0;
        if (!_desktopWCF.UpdateErrorWithAddingBalance(txnID, TxnStatusConstants.Error, resdesc, agentID, amount, isAgreement, out errMsg, out availableBalance, out ledgerBalance))
        {
            Utils.WriteLog_Biller(logAppender + "Error in update Error with Add Balance : " + errMsg);
        }
        Utils.WriteLog_Biller(logAppender + "After Update Error With Add Balance| TxnID:" + txnID + "|agentID:" + agentID + "AvailableBalance:" + availableBalance + "LedgerBalance:" + ledgerBalance);
        //Processing is failed because of internal server error, please try again
        return Utils.getErrorRes(rescode, resdesc, taxId);
    }

    #region Private Methods
    private bool IsValidPhoneNumberV2(string phoneNo, string taxId)
    {
        if (taxId == ConfigurationManager.AppSettings["OoredooELoadTaxId"].ToString())
        {
            string OoredooNumberlist = ConfigurationManager.AppSettings["OoredooNumber"].ToString();
            return isValidPhoneNumber(phoneNo, OoredooNumberlist);
        }
        return true;
    }

    private bool IsELoadBiller(string mapTaxId)
    {
        return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ELoadBillerList"].ToString().Split(',').FirstOrDefault(x => x == mapTaxId));
    }

    private bool IsEPinBiller(string mapTaxId)
    {
        return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["EPinBillerList"].ToString().Split(',').FirstOrDefault(x => x == mapTaxId));
    }

    private bool IsDataPackageBiller(string mapTaxId)
    {
        return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["DataPackBillerList"].ToString().Split(',').FirstOrDefault(x => x == mapTaxId));
    }

    private BillerTypeEnum GetBillerType(string mapTaxId)
    {
        if (IsEPinBiller(mapTaxId))
        {
            return BillerTypeEnum.EPIN;
        }
        else if (IsELoadBiller(mapTaxId))
        {
            return BillerTypeEnum.ELOAD;
        }
        else if (IsDataPackageBiller(mapTaxId))
        {
            return BillerTypeEnum.DATAPACKAGE;
        }
        else
        {
            return BillerTypeEnum.NONE;
        }
    }

    private EPinProcessRequestModel PopulateEPinProcessModel(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo)
    {
        EPinProcessRequestModel ePinProcessRequestModel = new EPinProcessRequestModel()
                {
                    MobileNumber = confirmResponseModel.ref3,
                    Amount = responseInfo.amount, 
                    AgentId = responseInfo.agentID, 
                    TxnId = responseInfo.txnID, 
                    AgentAmount = responseInfo.agentAmount,
                    IsAgreement = responseInfo.isAgreement,
                    AgentFeeDbl = responseInfo.agentFeeDbl,
                    SmsStatus = responseInfo.smsStatus,
                    AvailableBalance = responseInfo.availablebalance,
                    AppType = responseInfo.appType,
                    TopupType = responseInfo.topupType,
                    AgentName = responseInfo.agentName,
                    MapTaxId = responseInfo.MapTaxID,
                    BranchCode = responseInfo.branchCode,
                    SenderName = responseInfo.sendername,
                    BillerCode = responseInfo.billerCode,
                    MessageId = confirmResponseModel.messageid,
                    Ref1 = confirmResponseModel.ref1,
                    Ref2= confirmResponseModel.ref2,
                    Ref3 = confirmResponseModel.ref3,
                    Email = confirmResponseModel.email,
                    BillerName= confirmResponseModel.billername,
                    Password = confirmResponseModel.password,
                    Ref1Name = confirmResponseModel.ref1Name,
                    Ref2Name = confirmResponseModel.ref2Name,
                    Ref3Name = confirmResponseModel.ref3Name,
                    Ref4Name = confirmResponseModel.ref4Name,
                    Ref5Name = confirmResponseModel.ref5Name,
                    BillerLogo = confirmResponseModel.billerlogo,
                    TodayTxnAmount = confirmResponseModel.TodayTxnAmount,
                    TodayTxnCount = confirmResponseModel.TodayTxnCount
                };
        return ePinProcessRequestModel;
    }

    private ELoadProcessRequestModel PopulateELoadProcessModel(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo)
    {
        ELoadProcessRequestModel eLoadProcessRequestModel = new ELoadProcessRequestModel()
        {
            MobileNumber = confirmResponseModel.ref3,
            Amount = responseInfo.amount,
            AgentId = responseInfo.agentID,
            TxnId = responseInfo.txnID,
            AgentAmount = responseInfo.agentAmount,
            IsAgreement = responseInfo.isAgreement,
            AgentFeeDbl = responseInfo.agentFeeDbl,
            SmsStatus = responseInfo.smsStatus,
            AvailableBalance = responseInfo.availablebalance,
            BillerCode = responseInfo.billerCode,
            MessageId = confirmResponseModel.messageid
        };
        return eLoadProcessRequestModel;
    }

    private DataPackProcessRequestModel PopulateDataPackageProcessModel(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo)
    {
        DataPackProcessRequestModel dataPackProcessRequestModel = new DataPackProcessRequestModel()
        {
            MobileNumber = confirmResponseModel.ref3,
            PackageCode = GetDataPackCode(responseInfo.MapTaxID, confirmResponseModel.ref1, confirmResponseModel.ref2,confirmResponseModel.ref4),
            Amount = responseInfo.amount,
            AgentId = responseInfo.agentID,
            TxnId = responseInfo.txnID,
            AgentAmount = responseInfo.agentAmount,
            IsAgreement = responseInfo.isAgreement,
            AgentFeeDbl = responseInfo.agentFeeDbl,
            SmsStatus = responseInfo.smsStatus,
            AvailableBalance = responseInfo.availablebalance,
            BillerCode = responseInfo.billerCode,
            MessageId = confirmResponseModel.messageid
        };
        return dataPackProcessRequestModel;
    }

    private string GetDataPackCode(string mapTaxId, string ref1, string ref2, string ref4)
    {
        if (mapTaxId == ConfigurationManager.AppSettings["MyTelDataPackage"].ToString())
        {
            return ref4;
        }
        else if (mapTaxId == ConfigurationManager.AppSettings["AtomDataPackTaxId"].ToString())
        {
            return ref2;
        }
        else
        {
            return ref1;
        }
    }

    private string ConfirmEPin(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, string logAppender)
    {
        string confirmRes = string.Empty;
        string rescode = string.Empty;
        string resdecs = string.Empty;
        string mobileNo = confirmResponseModel.ref3;

        switch (confirmResponseModel.billersource.ToUpper())
        {
            case BillerConstants.GET:
                var telCoGETManager = new TelCoGETManager();
                EPinProcessRequestModel ePinProcessRequestModel = PopulateEPinProcessModel(confirmResponseModel, responseInfo);
                confirmRes = telCoGETManager.ProcessEPin(confirmResponseModel, ePinProcessRequestModel);
                return confirmRes;

            case BillerConstants.EBA :  
                var telCoEBAManager = new TelCoEBAManager();
                confirmRes = telCoEBAManager.ConfrimEPinToEBA(confirmResponseModel, responseInfo.amount, responseInfo.agentID, responseInfo.txnID, responseInfo.agentAmount,
                    responseInfo.isAgreement, responseInfo.agentFeeDbl, responseInfo.smsStatus, responseInfo.availablebalance, responseInfo.appType, responseInfo.topupType,
                    responseInfo.agentName, responseInfo.MapTaxID, responseInfo.branchCode, responseInfo.sendername, responseInfo.billerCode, confirmResponseModel.messageid,
                    confirmResponseModel.ref1, confirmResponseModel.ref2, confirmResponseModel.ref3, confirmResponseModel.email, confirmResponseModel.billername,
                    confirmResponseModel.password, confirmResponseModel.ref1Name, confirmResponseModel.ref2Name, confirmResponseModel.ref3Name,
                    confirmResponseModel.ref4Name, confirmResponseModel.ref5Name, confirmResponseModel.billerlogo, confirmResponseModel.TodayTxnAmount, confirmResponseModel.TodayTxnCount);                        
                return confirmRes;
            
            default:
                rescode = "06";
                resdecs = "Invalid Biller Provider!";
                return GetErrorResponseWithAddBalance(logAppender, rescode, resdecs, responseInfo.txnID, resdecs, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }          
    }

    private string ConfirmELoad(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, string logAppender)
    {
        string confirmRes = string.Empty;
        string rescode = string.Empty;
        string resdecs = string.Empty;
        string mobileNo = confirmResponseModel.ref3;

        if (!IsValidPhoneNumberV2(mobileNo, responseInfo.MapTaxID))
        {
            rescode = "06";
            resdecs = "Invalid Mobile Number!";
            return GetErrorResponseWithAddBalance(logAppender, rescode, resdecs, responseInfo.txnID, resdecs, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }

        switch (confirmResponseModel.billersource.ToUpper())
        {
            case BillerConstants.GET:   
                var telCoGETManager = new TelCoGETManager();
                ELoadProcessRequestModel eLoadProcessRequestModel = PopulateELoadProcessModel(confirmResponseModel, responseInfo);
                confirmRes = telCoGETManager.ProcessELoad(confirmResponseModel, eLoadProcessRequestModel);
                return confirmRes;

            case BillerConstants.EBA:
                var telCoEBAManager = new TelCoEBAManager();
                confirmRes  = telCoEBAManager.ConfrimELoadToEBA(confirmResponseModel, responseInfo.amount, responseInfo.agentID, responseInfo.txnID, responseInfo.agentAmount,
                    responseInfo.isAgreement, responseInfo.agentFeeDbl, responseInfo.smsStatus, responseInfo.availablebalance, responseInfo.billerCode, confirmResponseModel.messageid);
                return confirmRes;

            default:
                rescode = "06";
                resdecs = "Invalid Biller Provider!";
                return GetErrorResponseWithAddBalance(logAppender, rescode, resdecs, responseInfo.txnID, resdecs, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }       
    }

    private string ConfirmDataPack(ConfirmResponseModel confirmResponseModel, ResponseInfo responseInfo, string logAppender)
    {
        string confirmRes = string.Empty;
        string rescode = string.Empty;
        string resdecs = string.Empty;
        string mobileNo = confirmResponseModel.ref3;

        switch (confirmResponseModel.billersource.ToUpper())
        {
            case BillerConstants.GET:
                var telCoGETManager = new TelCoGETManager();
                DataPackProcessRequestModel dataPackProcessRequestModel = PopulateDataPackageProcessModel(confirmResponseModel, responseInfo);
                confirmRes = telCoGETManager.ProcessDataPackage(confirmResponseModel, dataPackProcessRequestModel);
                return confirmRes;

            case BillerConstants.EBA:
                var telCoEBAManager = new TelCoEBAManager();
                confirmRes  = telCoEBAManager.ConfirmDataPack(confirmResponseModel, responseInfo);
                return confirmRes;

            default:
                rescode = "06";
                resdecs = "Invalid Biller Provider!";
                return GetErrorResponseWithAddBalance(logAppender, rescode, resdecs, responseInfo.txnID, resdecs, responseInfo.agentID, responseInfo.agentAmount, responseInfo.isAgreement);
        }
    }
    #endregion

    #region <-- MyanPay AirTime -->

    public static bool isValidPhoneNumber(string mobilenumber, string numberList)
    {
        bool result = false;
        string[] lstNumber = numberList.Split(':');
        for (int i = 0; i < lstNumber.Count(); i++)
        {
            if (mobilenumber.Contains(lstNumber[i]))
            {
                result = true;
                break;
            }
        }
        return result;
    }
    #endregion
}