public class CashInInquiryRequest
{

    public string Token { get; set; }

    public string Channel { get; set; }

    public string PayerName { get; set; }

    public string PayerEmail { get; set; }

    public string PayerPhone { get; set; }

    public string PayerNRC { get; set; }

    public string PayeeName { get; set; }

    public string PayeeEmail { get; set; }

    public string PayeePhone { get; set; }

    public string PayeeNRC { get; set; }

    public string Amount { get; set; }

    public string Remark { get; set; }

    public string PaidBy { get; set; }

    public string DivisionID { get; set; }

    public string TownshipID { get; set; }

    public string BranchID { get; set; }
}

public class CashInConfirmRequest
{

    public string Token { get; set; }

    public string Channel { get; set; }

    public string Amount { get; set; }

    public string TxnRef { get; set; }

    public string OrderId { get; set; }

    public string PayerMobileNumber { get; set; }
}

public class CashOutInquiryRequest
{

    public string Token { get; set; }

    public string Channel { get; set; }

    public string DigitalCode { get; set; }

    public string BranchCode { get; set; }

    public string DivisionID { get; set; }

    public string TownshipID { get; set; }

}

public class CashOutConfirmRequest
{

    public string Token { get; set; }

    public string Channel { get; set; }

    public string DigitalCode { get; set; }

    public string Amount { get; set; }

    public string TxnRef { get; set; }

    public string BranchCode { get; set; }

    public string DivisionID { get; set; }

    public string TownshipID { get; set; }

}

public class CashInInquiryResponse
{


    public string Channel { get; set; }


    public string Amount { get; set; }


    public string PayerFee { get; set; }


    public string PayeeFee { get; set; }


    public string TxnRef { get; set; }


    public string ResponseCode { get; set; }


    public string TransactionStatus { get; set; }
}

public class CashInConfirmResponse
{


    public string Channel { get; set; }


    public string Amount { get; set; }


    public string TxnRef { get; set; }


    public string ResponseCode { get; set; }


    public string TransactionStatus { get; set; }


}

public class CashOutInquiryResponse
{


    public string Channel { get; set; }


    public string PayerName { get; set; }


    public string PayerPhone { get; set; }


    public string PayeeName { get; set; }


    public string PayeePhone { get; set; }


    public string PayeeNRC { get; set; }


    public string Amount { get; set; }


    public string PayerFee { get; set; }


    public string PayeeFee { get; set; }


    public string TxnRef { get; set; }


    public string PaidBy { get; set; }


    public string ResponseCode { get; set; }


    public string TransactionStatus { get; set; }

}

public class CashOutConfirmResponse
{


    public string Channel { get; set; }

    public string Amount { get; set; }

    public string TxnRef { get; set; }

    public string PaidBy { get; set; }

    public string ResponseCode { get; set; }

    public string TransactionStatus { get; set; }

}

public class GetInquiryCashInResult
{
    public CashInInquiryResponse InquiryCashInResult { get; set; }

}
public class GetConfirmCashInResult
{
    public CashInConfirmResponse ConfirmCashInResult { get; set; }

}
public class GetInquiryCashOutResult
{
    public CashOutInquiryResponse InquiryCashOutResult { get; set; }

}
public class GetConfrimCashOutResult
{
    public CashOutConfirmResponse ConfrimCashOutResult { get; set; }

}

public static class VerificationRemittanceResponse
{
    public static string VerifyResponse(int responsecode)
    {
        string responsedesc = string.Empty;
        switch (responsecode)
        {
            case 6:
                responsedesc = "Invalid Amount";
                break;
            case 9:
                responsedesc = "Invalid Code";
                break;
            case 10:
                responsedesc = "Already Cash out!";
                break;
            case 18:
                responsedesc = "Remittance code has been expired!";
                break;
            default:
                responsedesc = "System Error!";
                break;
        }
        return responsedesc;
    }
}
