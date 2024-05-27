using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

/// <summary>
/// Summary description for SendSMSInfobipService
/// </summary>
public class SendSMSInfobipService
{
    public bool SendSMS(SendSmsInfobipRequest sendSmsInfobipRequest)
    {
        HttpService _service = new HttpService();

        string smsUrl = ConfigurationManager.AppSettings["SendSmsInfobipInterfaceApiUrl"].ToString();
        string requestData = JsonConvert.SerializeObject(sendSmsInfobipRequest);

        Utils.WriteLog_Biller("Send SMS Request to SendSmsInfobipInterface API : " + requestData);

        Utils.WriteLog_Biller("SMS sending.....");
        string response = _service.Post(requestData, smsUrl);
        Utils.WriteLog_Biller("SMS sent");

        Utils.WriteLog_Biller("Send SMS Response Status from SendSmsInfobipInterface API : " + response);
        return true;
    }

    public class SendSmsInfobipRequest
    {
        public string ToMobile { get; set; }
        public string Message { get; set; }
    }
}