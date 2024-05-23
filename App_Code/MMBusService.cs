using System;
using System.Configuration;
using log4net;
using System.Reflection;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for MMBusService
/// </summary>
public class MMBusService
{
    #region <-- Log -->
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void writeLog(string msg)
    {
        string maskSensitiveString = maskSensitiveData(msg);
        if (!string.IsNullOrEmpty(maskSensitiveString))
        {
            Logger.writeLog(maskSensitiveString, ref log);
        }
        else
        {
            Logger.writeLog(msg, ref log);
        }
    }
    #endregion 

    public static string maskSensitiveData(string value)
    {
        string regularExpressionPattern = @"<Password>(.*?)<\/Password>";
        Regex regex = new Regex(regularExpressionPattern, RegexOptions.Singleline);
        MatchCollection collection = regex.Matches(value);
        if (collection.Count > 0)
        {
            Match m = collection[0];
            var stripped = m.Groups[1].Value;
            if (!string.IsNullOrEmpty(stripped))
            {
                return value.Replace(stripped, "XXXX-XXXX-XXXX");
            }

        }
        return "";
    }
	public MMBusService()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public double AddAgentAmountToTotalAmount(double netAmount)
    {
        var agentFeePercent = Convert.ToDouble(ConfigurationManager.AppSettings["MMBusAgentPercent"].ToString());
        writeLog("Net Amount : " + netAmount);
        var netTransactionPercent = 100 - agentFeePercent;
        writeLog("Net Transaction Percent : " + netTransactionPercent);
        var totalAmount = (netAmount * 100) / netTransactionPercent;
        writeLog("Total Amount : " + totalAmount);
        return totalAmount;
    }
}