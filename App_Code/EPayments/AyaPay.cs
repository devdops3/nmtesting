using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;


public class AyaPay
{
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public AyaPay()
    {
        
    }

    public AyaPayInterfaceResponse Confirm(string reqJson, string url)
    {
        log.Info("AYAPay Confirm");
        log.Info("Request: "+reqJson);
        AyaPayInterfaceResponse ayaPayInterfaceResponse=null;
        string result = string.Empty;
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

            log.Info("AyaPayInterface Response" + " : " + result);

            ayaPayInterfaceResponse = JsonConvert.DeserializeObject<AyaPayInterfaceResponse>(result);
        }catch(Exception ex)
        {
            log.Error("Error In AYAPay Confirm:", ex);
            ayaPayInterfaceResponse = new AyaPayInterfaceResponse()
            {
                ResCode = "01",
                FailReason = "AYAPay Confirm Request Failed."
            };
        }
        return ayaPayInterfaceResponse;
    }
    public AyaPayInterfaceResponse Refund(long transactionId)
    {
        DataSet ds;
        string errMsg = "";
        A2AAPIWCF.ServiceClient agentWCF = new A2AAPIWCF.ServiceClient();
        AdminWcf.ServiceClient adminWCF = new AdminWcf.ServiceClient();
        agentWCF.GetMerchantTransactionById(transactionId, out errMsg, out ds);
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            DataRow transactionRow = ds.Tables[0].Rows[0];
            adminWCF.getMerchantFeeProfileByAgentCodeAndPaymentType(transactionRow["AgentCode"]+"", transactionRow["PaymentType"]+"", out ds, out errMsg);

            var dataTable = ds.Tables[0];
            var dataRow = dataTable.Rows[0];

            var paymentProfileInfo = new Dictionary<string, string>();
            paymentProfileInfo.Add("shopId", dataRow["MerchantId"].ToString());
            paymentProfileInfo.Add("phone", dataRow["ClientId"].ToString());
            paymentProfileInfo.Add("paymentSchemeRefNo", dataRow["MerchantReferenceId"].ToString());
            AyaPayInterfaceRequest ayaPayInterfaceRequest = new AyaPayInterfaceRequest 
            { 
                NearMePaymentRefNo = "NearMe" + transactionId,
                PaymentProfileInfo = paymentProfileInfo
            };
            string ayaPayReq = JsonConvert.SerializeObject(ayaPayInterfaceRequest);
            string ayaPayInterfaceUrl = ConfigurationManager.AppSettings["AYAPayInterfaceUrl"];
            RefundRequest(ayaPayReq, ayaPayInterfaceUrl);
        }

            throw new NotImplementedException();
    }
    public AyaPayInterfaceResponse RefundRequest(string reqJson, string url)
    {
        log.Info("AYAPay Refund");
        log.Info("Request: " + reqJson);
        AyaPayInterfaceResponse ayaPayInterfaceResponse = null;
        string result = string.Empty;
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

            log.Info("AyaPayInterface Response" + " : " + result);

            ayaPayInterfaceResponse = JsonConvert.DeserializeObject<AyaPayInterfaceResponse>(result);
        }
        catch (Exception ex)
        {
            log.Error("Error In AYAPay Refund:", ex);
            ayaPayInterfaceResponse = new AyaPayInterfaceResponse()
            {
                ResCode = "01",
                FailReason = "AYAPay Refund Request Failed."
            };
        }
        return ayaPayInterfaceResponse;
    }


}