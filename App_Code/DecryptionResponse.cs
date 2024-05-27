using System.Configuration;

/// <summary>
/// Summary description for DecryptionResponse
/// </summary>
public class DecryptionResponse
{
	public DecryptionResponse()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    private static string channelKey = ConfigurationManager.AppSettings["123RemitChannelKey"].ToString();

    public static CashInInquiryResponse DecryptCashinInInquiryResponse(CashInInquiryResponse response)
    {
        response.Channel = Utils.AESDecryptText(response.Channel, channelKey);
        response.Amount = Utils.AESDecryptText(response.Amount, channelKey);
        response.PayerFee = Utils.AESDecryptText(response.PayerFee, channelKey);
        response.PayeeFee = Utils.AESDecryptText(response.PayeeFee, channelKey);
        response.TxnRef = Utils.AESDecryptText(response.TxnRef, channelKey);
        return response;
    }

    public static CashInConfirmResponse DecryptCashinInConfirmResponse(CashInConfirmResponse response)
    {
        response.Channel = Utils.AESDecryptText(response.Channel, channelKey);
        response.Amount = Utils.AESDecryptText(response.Amount, channelKey);
        response.TxnRef = Utils.AESDecryptText(response.TxnRef, channelKey);
        return response;
    }

    public static CashOutInquiryResponse DecryptCashinOutInquiryResponse(CashOutInquiryResponse response)
    {
        response.Channel = Utils.AESDecryptText(response.Channel, channelKey);
        response.PayerName = Utils.AESDecryptText(response.PayerName, channelKey);
        response.PayerPhone = Utils.AESDecryptText(response.PayerPhone, channelKey);
        response.PayeeName = Utils.AESDecryptText(response.PayeeName, channelKey);
        response.PayeePhone = Utils.AESDecryptText(response.PayeePhone, channelKey);
        response.PayeeNRC = Utils.AESDecryptText(response.PayeeNRC, channelKey);
        response.Amount = Utils.AESDecryptText(response.Amount, channelKey);
        response.PayerFee = Utils.AESDecryptText(response.PayerFee, channelKey);
        response.PayeeFee = Utils.AESDecryptText(response.PayeeFee, channelKey);
        response.TxnRef = Utils.AESDecryptText(response.TxnRef, channelKey);
        response.PaidBy = Utils.AESDecryptText(response.PaidBy, channelKey);
        return response;
    }

    public static CashOutConfirmResponse DecryptCashOutConfirmResponse(CashOutConfirmResponse response)
    {
        response.Channel = Utils.AESDecryptText(response.Channel, channelKey);
        response.Amount = Utils.AESDecryptText(response.Amount, channelKey);
        response.TxnRef = Utils.AESDecryptText(response.TxnRef, channelKey);
        response.PaidBy = Utils.AESDecryptText(response.PaidBy, channelKey);
        return response;
    }

}