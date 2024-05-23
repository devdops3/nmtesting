using System;
using System.Configuration;
using System.Linq;
using System.Text;

/// <summary>
/// Summary description for SMSHelper
/// </summary>
public class SMSHelper
{
    public SMSHelper()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string ChannelCode = ConfigurationManager.AppSettings["ChannelCodeForSMS"].ToString();
    public string ChannelCodeat = ConfigurationManager.AppSettings["ChannelCodeForSMS"].ToString() + " at ";
    public string getMessageTopup(string agentName, string taxID, string biller, string pin, string serialNo, string expiry, string amount, string shopCode)
    {
        StringBuilder sb = new StringBuilder();


        if (taxID == "2222222222222" || taxID == "0000000000024")//MPT-CDMA 800
        {
            sb.Append("*166*" + pin + "#");
        }
        else if (taxID == "4444444444444") //MEC-CDMA 800
        {
            sb.Append("*124*" + pin + "#");
        }
        else
        {
            sb.Append("*123*" + pin + "#");
        }
        sb.Append("\n");
        GetApkDownloadLink(sb);

        return sb.ToString();


    }

    public string getMessageBiller1Stop(string agentName, string taxID, string biller, string ref1Name, string ref2Name, string ref3Name, string ref4Name,
        string ref1Value, string ref2Value, string ref3Value, string ref4Value, string amount, string serviceFee, string totalAmount, string shopCode)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(ChannelCodeat + agentName);
        sb.Append(" *" + biller);

        sb.Append(" *" + "Partner " + " : " + ref1Value);

        sb.Append("(" + ref2Value + ")");

        if (!String.IsNullOrEmpty(ref3Name))
            sb.Append(" *" + ref3Name + " : " + ref3Value);

        if (!String.IsNullOrEmpty(ref4Name) && ref4Name != "Ref")
            sb.Append(" *" + ref4Name + " : " + ref4Value);

        sb.Append(" *Amount : " + amount);
        if (Convert.ToDouble(serviceFee) > 0)
        {
            sb.Append(" *Customer Fee : " + serviceFee);
            sb.Append(" *Total : " + totalAmount);
        }

        if (!String.IsNullOrEmpty(ref4Name) && ref4Name == "Ref")
        {
            sb.Append(" *" + ref4Name + " : " + ref4Value);
        }

       string smsMsg = "";

        if (sb.ToString().Length > 159)
        {
            smsMsg = sb.ToString().Substring(0, 159);
        }
        else
        {
            smsMsg = sb.ToString();
        }
        sb.Append(" ").ToString();
        smsMsg = GetApkDownloadLink(sb);
        return smsMsg;
    }
    public string getMessageBillerMercyCrops(string agentName, string taxID, string biller, string ref1Name, string ref2Name, string ref3Name, string ref4Name,
       string ref1Value, string ref2Value, string ref3Value, string ref4Value, string amount, string serviceFee, string totalAmount, string shopCode)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(ChannelCodeat + agentName);
        sb.Append(" *" + biller);
       
        sb.Append(" *" + "Partner Code" + " : " + ref2Value);
        
        if (!String.IsNullOrEmpty(ref1Name))
            sb.Append(" *" + ref1Name + " : " + ref1Value);

        sb.Append(" *Amount : " + amount);
        if (Convert.ToDouble(serviceFee) > 0)
        {
            sb.Append(" *Customer Fee : " + serviceFee);
            sb.Append(" *Total : " + totalAmount);
        }

        if (!String.IsNullOrEmpty(ref4Name))
            sb.Append(" *" + ref4Name + " : " + ref4Value);
       
        string smsMsg = "";

        if (sb.ToString().Length > 159)
        {
            smsMsg = sb.ToString().Substring(0, 159);
        }
        else
        {
            smsMsg = sb.ToString();
        }
        sb.Append(" ").ToString();
        smsMsg = GetApkDownloadLink(sb);
        return smsMsg;
    }
    public string getMessageBiller(string agentName, string taxID, string biller, string ref1Name, string ref2Name, string ref3Name, string ref4Name,
        string ref1Value, string ref2Value, string ref3Value, string ref4Value, string amount, string serviceFee, string totalAmount, string shopCode)
    {
        StringBuilder sb = new StringBuilder();
        string txnId = string.Empty;
        if (ref4Value.Contains("|"))
        {
            var r4 = ref4Value.Split('|').ToList();
            txnId = r4.FirstOrDefault();
            ref4Value = r4.LastOrDefault();
        }

        sb.Append(ChannelCodeat + agentName);
        sb.Append(" *" + biller);

        if (taxID == ConfigurationManager.AppSettings["MotherFinanceTaxId"].ToString())
        {
            sb.Append(" *" + ref4Name + " : " + ref4Value);
        }

        if (!String.IsNullOrEmpty(ref1Name)) sb.Append(" *" + ref1Name + " : " + ref1Value);

        if (!String.IsNullOrEmpty(ref2Name)) sb.Append(" *" + ref2Name + " : " + ref2Value);

        if (!String.IsNullOrEmpty(ref3Name)) sb.Append(" *" + ref3Name + " : " + ref3Value);

        if (taxID == "0000000000021")
        {
            if (!string.IsNullOrEmpty(shopCode))
            {
                string[] date = shopCode.Split(' ');
                sb.Append(" *PayForDate :" + date[0] + "To" + date[1]);
            }
        }
        if (!String.IsNullOrEmpty(ref4Name) && taxID != ConfigurationManager.AppSettings["MotherFinanceTaxId"].ToString() && ref4Name != "Ref")
            sb.Append(" *" + ref4Name + " : " + ref4Value);

        sb.Append(" *Amount : " + amount);

        if (Convert.ToDouble(serviceFee) > 0)
        {
            sb.Append(" *Customer Fee : " + serviceFee);
            sb.Append(" *Total : " + totalAmount);
        }
        if (!String.IsNullOrEmpty(txnId)) sb.Append(" *Ref" + " : " + txnId);

        if (!String.IsNullOrEmpty(ref4Name) && ref4Name == "Ref")
        {
            sb.Append(" *" + ref4Name + " : " + ref4Value);
        }

        string smsMsg = "";

        if (sb.ToString().Length > 259)
        {
            smsMsg = sb.ToString().Substring(0, 259);
        }
        else
        {
            smsMsg = sb.ToString();
        }
        sb.Append(" ").ToString();
        smsMsg = GetApkDownloadLink(sb);
        return smsMsg;
    }

    public string getMessageBillerForSkyNet(string agentName, string taxID, string biller, string ref1Name, string ref2Name, string ref3Name, string ref4Name,string txnId,
        string ref1Value, string ref2Value, string ref3Value, string ref4Value, string amount, string serviceFee, string totalAmount, string shopCode)
    {
        StringBuilder sb = new StringBuilder();
       if (ref4Value.Contains("|"))
        {
            var r4 = ref4Value.Split('|').ToList();
            txnId = r4.FirstOrDefault();
            ref4Value = r4.LastOrDefault();
        }

        sb.Append(ChannelCodeat + agentName);
        sb.Append(" *" + biller);

        if (taxID == ConfigurationManager.AppSettings["MotherFinanceTaxId"].ToString())
        {
            sb.Append(" *" + ref4Name + " : " + ref4Value);
        }

        if (!String.IsNullOrEmpty(ref1Name)) sb.Append(" *" + ref1Name + " : " + ref1Value);

        if (!String.IsNullOrEmpty(ref2Name)) sb.Append(" *" + ref2Name + " : " + ref2Value);

        if (!String.IsNullOrEmpty(ref3Name)) sb.Append(" *" + ref3Name + " : " + ref3Value);

        if (!String.IsNullOrEmpty(ref4Name)) sb.Append(" *" + ref4Name + " : " + ref4Value);

        sb.Append(" *Amount : " + amount);

        if (Convert.ToDouble(serviceFee) > 0)
        {
            sb.Append(" *Customer Fee : " + serviceFee);
            sb.Append(" *Total : " + totalAmount);
        }
        if (!String.IsNullOrEmpty(txnId)) sb.Append(" *Ref" + " : " + txnId);

        if (!String.IsNullOrEmpty(ref4Name) && ref4Name == "Ref")
        {
            sb.Append(" *" + ref4Name + " : " + ref4Value);
        }

       string smsMsg = "";

        if (sb.ToString().Length > 259)
        {
            smsMsg = sb.ToString().Substring(0, 259);
        }
        else
        {
            smsMsg = sb.ToString();
        }

        return smsMsg;
    }

    public string GetMerchantMessage(string txnId, string agentName, string taxId, string biller, string ref1Name, string ref2Name, string ref3Name, string ref4Name, string ref1Value, string ref2Value, string ref3Value, string ref4Value, string amount, string shopCode)
    {
        var sb = new StringBuilder();

        sb.Append(ChannelCodeat + agentName);
        sb.Append(" *" + biller);

        if (!String.IsNullOrEmpty(ref1Name)) sb.Append(" *" + ref1Name + " : " + ref1Value);

        if (!String.IsNullOrEmpty(ref2Name)) sb.Append(" *" + ref2Name + " : " + ref2Value);

        if (!String.IsNullOrEmpty(ref3Name)) sb.Append(" *" + ref3Name + " : " + ref3Value);

        if (!String.IsNullOrEmpty(ref4Name)) sb.Append(" *" + ref4Name + " : " + ref4Value);

        sb.Append(" *Amount : " + amount);

        if (!String.IsNullOrEmpty(txnId)) sb.Append(" *" + "Ref" + " : " + txnId);

        var smsMsg = sb.ToString();
        sb.Append(" ").ToString();
        smsMsg = GetApkDownloadLink(sb);
        return smsMsg;
    }

    public string GetMinTheinKhaMessageBiller(string txnId, string agentName, string taxId, string biller, string ref1Name, string ref1Value, string amount, string serviceFee, string totalAmount, string shopCode, string desc)
    {
        var sb = new StringBuilder();

        sb.Append(ChannelCodeat + agentName);
        sb.Append(" *" + biller);

        if (!String.IsNullOrEmpty(ref1Name)) sb.Append(" *" + ref1Name + " : " + ref1Value);

        sb.Append(" *Amount : " + amount);

        if (Convert.ToDouble(serviceFee) > 0)
        {
            sb.Append(" *Customer Fee : " + serviceFee);
            sb.Append(" *Total : " + totalAmount);
        }

        if (!String.IsNullOrEmpty(txnId)) sb.Append(" *" + "Ref" + " : " + txnId);

        if (!String.IsNullOrEmpty(desc)) sb.Append(" *" + "Desc" + " : " + desc);

        var smsMsg = sb.ToString();
        sb.Append(" ").ToString();
        smsMsg = GetApkDownloadLink(sb);
        return smsMsg;
    }

    public string GetMessageBiller(string txnId, string agentName, string taxId, string biller, string ref1Name, string ref2Name, string ref3Name, string ref4Name, string ref1Value, string ref2Value, string ref3Value, string ref4Value, string amount, string serviceFee, string totalAmount, string shopCode)
    {
        var sb = new StringBuilder();

        sb.Append(ChannelCodeat + agentName);
        sb.Append(" *" + biller);

        if (!String.IsNullOrEmpty(ref1Name)) sb.Append(" *" + ref1Name + " : " + ref1Value);

        if (!String.IsNullOrEmpty(ref2Name)) sb.Append(" *" + ref2Name + " : " + ref2Value);

        if (!String.IsNullOrEmpty(ref3Name)) sb.Append(" *" + ref3Name + " : " + ref3Value);

        if (!String.IsNullOrEmpty(ref4Name) && ref4Name != "Ref") sb.Append(" *" + ref4Name + " : " + ref4Value);

        sb.Append(" *Amount : " + amount);

        if (Convert.ToDouble(serviceFee) > 0)
        {
            sb.Append(" *Customer Fee : " + serviceFee);

            sb.Append(" *Total : " + totalAmount);
        }

        if (!String.IsNullOrEmpty(txnId)) sb.Append(" *" + "Ref" + " : " + txnId);

        if (!String.IsNullOrEmpty(ref4Name) && ref4Name == "Ref") sb.Append(" *" + ref4Name + " : " + ref4Value);

        var smsMsg = sb.ToString();
        sb.Append(" ").ToString();
        smsMsg = GetApkDownloadLink(sb);

        return smsMsg;
    }

    public string GetOoredooFtthMessage(string agentName, string biller, string txnDateTime, string ref1Name, string ref2Name, string ref4Name,
    string ref1Value, string ref2Value, string ref4Value, string amount, string serviceFee, string totalAmount, string shopCode)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(ChannelCodeat + agentName);
        sb.Append(" *" + biller);

        if (!String.IsNullOrEmpty(ref1Name)) sb.Append(" *" + ref1Name + " : " + ref1Value);

        if (!String.IsNullOrEmpty(ref2Name)) sb.Append(" *" + ref2Name + " : " + ref2Value);
    
        sb.Append(" *Amount : " + amount);

        if (Convert.ToDouble(serviceFee) > 0)
        {
            sb.Append(" *Customer Fee : " + serviceFee);
            sb.Append(" *Total : " + totalAmount);
        }

        if (!String.IsNullOrEmpty(ref4Name)) sb.Append(" *" + ref4Name + " : " + ref4Value);

        string smsMsg = "";

        if (sb.ToString().Length > 259)
        {
            smsMsg = sb.ToString().Substring(0, 259);
        }
        else
        {
            smsMsg = sb.ToString();
        }
        sb.Append(" ").ToString();
        smsMsg = GetApkDownloadLink(sb);
        return smsMsg;
    }

    public string getMessageBillerGGI(string agentName, string taxID, string biller, string ref1Name, string ref2Name, string ref3Name, string ref4Name,
       string ref1Value, string ref2Value, string ref3Value, string ref4Value, string amount, string serviceFee, string totalAmount, string shopCode)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(ChannelCodeat + agentName);
        sb.Append(" *" + biller);
      
        if (taxID == "0000000000020" || taxID == "0000000000021" || taxID == "0000000000022" || taxID == "0000000000029")
        {
            if (!String.IsNullOrEmpty(ref1Name))
                sb.Append(" *" + ref1Name + ": " + ref1Value);
            sb.Append(" *" + "Name" + ": " + ref2Value);
            sb.Append(" *" + "Receipt No" + ": " + ref3Value);

            sb.Append(" *Amount: " + amount);
            if (Convert.ToDouble(serviceFee) > 0)
            {
                sb.Append(" *Customer Fee: " + serviceFee);
                sb.Append(" *Total: " + totalAmount);
            }

            sb.Append(" *" + "Ref" + ": " + ref4Value);
        }
        else if (taxID == "0000000000023")
        {
            if (!String.IsNullOrEmpty(ref1Name))
                sb.Append(" *" + "Slip No" + " : " + ref1Value);
            sb.Append(" *" + "Customer ID" + " : " + ref2Value);
            sb.Append(" *" + "Receipt No" + " : " + ref3Value);
            sb.Append(" *Amount : " + amount);
            if (Convert.ToDouble(serviceFee) > 0)
            {
                sb.Append(" *Customer Fee : " + serviceFee);
                sb.Append(" *Total : " + totalAmount);
            }

            sb.Append(" *" + "Ref" + " : " + ref4Value);
        }
        else
        {

            if (!String.IsNullOrEmpty(ref1Name))
                sb.Append(" *" + ref1Name + " : " + ref1Value);
            sb.Append(" *Amount : " + amount);
            if (Convert.ToDouble(serviceFee) > 0)
            {
                sb.Append(" *Customer Fee : " + serviceFee);
                sb.Append(" *Total : " + totalAmount);
            }
            sb.Append(" *" + "Ref" + " : " + ref2Value);
            sb.Append(" *" + "Expiry Date" + " : " + ref3Value);
        }

        string smsMsg = "";

        smsMsg = sb.ToString();
        sb.Append(" ").ToString();
        smsMsg = GetApkDownloadLink(sb);
        return smsMsg;
    }

    public string getsuccessregsmsmessagebody(string userid, string password)
    {
        StringBuilder sb = new StringBuilder();
        string smsMsg = string.Empty;
        
        sb.Append(ChannelCode + " registration is successfully done.");
        sb.Append(" *Login ID : " + userid);
        sb.Append(" *Password : " + password);

        if (sb.ToString().Length > 159)
        {
            smsMsg = sb.ToString().Substring(0, 159);
        }
        else
        {
            smsMsg = sb.ToString();
        }
        return smsMsg;
    }

    public string getMessagelegacyMusic(string agentName, string taxID, string biller, string ref1Name, string ref2Name, string ref3Name, string ref4Name,
    string ref1Value, string ref2Value, string ref3Value, string ref4Value, string amount, string serviceFee, string totalAmount, string shopCode)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(ChannelCodeat + agentName);
        sb.Append(" *" + biller);
        sb.Append(" *" + ref1Name + " : " + ref1Value);

        sb.Append(" *Amount : " + amount);
        if (Convert.ToDouble(serviceFee) > 0)
        {
            sb.Append(" *Customer Fee : " + serviceFee);
            sb.Append(" *Total : " + totalAmount);
        }

        if (!String.IsNullOrEmpty(ref4Value))
            sb.Append(" *" + ref4Name + " : " + ref4Value);

        string smsMsg = "";

        if (sb.ToString().Length > 159)
        {
            smsMsg = sb.ToString().Substring(0, 159);
        }
        else
        {
            smsMsg = sb.ToString();
        }

        sb.Append(" ").ToString();
        smsMsg = GetApkDownloadLink(sb);
        return smsMsg;
    }


    private string GetApkDownloadLink(StringBuilder message)
    {
        return message.Append("*App : " + ConfigurationManager.AppSettings["ApkDownloadLink"].ToString()).ToString();
    }

}