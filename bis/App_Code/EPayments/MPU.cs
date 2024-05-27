using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;

/// <summary>
/// Summary description for MPU
/// </summary>
public class Mpu
{
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly A2AAPIWCF.ServiceClient _agentWCF = new A2AAPIWCF.ServiceClient();

    public Mpu()
    {
    }

    public MpuInterfaceResponse TransactionRequest(string reqJson)
    {
        string url = ConfigurationManager.AppSettings["MPUInterfaceUrl"];
        string result = string.Empty;
        string errMsg = "";
        string batchNo = "";
        string traceNo = "";
        
        _agentWCF.GetBatchNBatchNo(out errMsg, out batchNo, out traceNo);

        MpuInterfaceResponse mpuRes = new MpuInterfaceResponse();
        try
        {
            
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(reqJson);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            WriteLog("MPUInterface Response" + " : " + result);

            mpuRes = JsonConvert.DeserializeObject<MpuInterfaceResponse>(result);

        }
        catch (Exception ex)
        {
            WriteLog("Exception occur when request to MPUInterface:" + ex.Message);
            mpuRes.ResCode = "06";
            mpuRes.ResDescription = "Error";
            mpuRes.FailReason = "Error";
        }
        return mpuRes;
    }

    
    public void SaveCardTransaction(long taxId ,MpuInterfaceRequest mpuInterfaceRequest,MpuInterfaceResponse mpuInterfaceResponse)
    {
        var requestPaymentInfo = mpuInterfaceRequest.PaymentInfo;
        var responsePaymentInfo = mpuInterfaceResponse.PaymentInfo;
        string errMsg;

        if(!_agentWCF.InsertCardTransaction(taxId, responsePaymentInfo["transactionDateTime"],  responsePaymentInfo["transactionRefNo"], requestPaymentInfo["merchantId"]
            , requestPaymentInfo["terminalId"], "051", "00", responsePaymentInfo["traceNo"], responsePaymentInfo["batchNo"]
            , requestPaymentInfo["cardNo"], requestPaymentInfo["expiry"], requestPaymentInfo["pin"], mpuInterfaceRequest.Amount
            ,"104", requestPaymentInfo["trackData"], requestPaymentInfo["chipData"], responsePaymentInfo["approvalCode"], out errMsg))
        {
            WriteLog("Error in SaveCardTransaction: " + errMsg);
        }
            
    }

    public System.Data.DataSet GetCardTransactionByMerchantTransactionId(long merchentTransactionId)
    {
        string errMsg;
        System.Data.DataSet ds;

        if (!_agentWCF.GetCardTransactionByMerchentTransactionId(merchentTransactionId,out errMsg,out ds))
        {
            WriteLog(errMsg);
        }
        return ds;
    }

    public Dictionary<string,string> PrepareVoidPaymentInfo(System.Data.DataRow cardTransactionRow,string batchNo,string traceNo)
    {
        Dictionary<string, string> paymentInfo = new Dictionary<string, string>
        {
            { "cardNo", cardTransactionRow["CardNo"].ToString() },
            { "expiry", cardTransactionRow["Expiry"].ToString() },
            { "pin", cardTransactionRow["Pin"].ToString() },
            { "traceNo", traceNo },
            { "batchNo", batchNo },
            { "trackData", cardTransactionRow["TrackData"] + "" },
            { "chipData", cardTransactionRow["ChipData"] + "" },
            { "transactionRefNo", cardTransactionRow["TransactionRefNo"].ToString() },
            { "processType", "V" },
            { "originalMessage", cardTransactionRow["BatchNo"].ToString() + cardTransactionRow["TraceNo"].ToString() +cardTransactionRow["TransactionDateTime"].ToString().Substring(0,4) }
        };
        return paymentInfo;
    }

    public string MaskCardNumber(string cardNo)
    {
        if(cardNo.Length == 16)
        {
            cardNo = cardNo.Remove(6, 6).Insert(6, "xxxxxx");
        }else if(cardNo.Length == 19)
        {
            cardNo = cardNo.Remove(6, 9).Insert(6, "xxxxxxxxx");
        }
        
        return cardNo;
    }

    private static void WriteLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }



}