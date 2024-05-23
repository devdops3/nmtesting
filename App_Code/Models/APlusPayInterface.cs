public class APlusPayInterfacePayRequest
{
    public string NearMeRefId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string MerchantUserId { get; set; }
    public string SequenceNo { get; set; }
    public string Amount { get; set; }
    public string QrString { get; set; }
    public string Fee { get; set; }
}

public class APlusPayInterfacePayResponse
{
    public string Code { get; set; }
    public string Description { get; set; }
    public string Message { get; set; }
    public APlusPayInterfacePayResponseData Data { get; set; }
}

public class APlusPayInterfacePayResponseData
{
    public string Channel { get; set; }
    public string ReferIntegrationId { get; set; }
    public string WalletUserID { get; set; }
    public string Amount { get; set; }
    public string Discount { get; set; }
    public string Fee { get; set; }
    public string TransactionNo { get; set; }
    public string RespCode { get; set; }
    public string RespDescription { get; set; }
}