/// <summary>
/// Summary description for ConfrimReq
/// </summary>
public class ConfrimReq
{
	public ConfrimReq()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string TaxId { get; set; }
    public long TxnId { get; set; }
    public string Amount { get; set; }
    public int AgentId { get; set; }
    public double AgentAmount { get; set; }
    public string IsAgreement { get; set; }
    public string MobileNo { get; set; }
    public string Email { get; set; }
    public double AgentFee { get; set; }
    public double AvailableBalance { get; set; }
    public string SMSStatus { get; set; }
    public string AppType { get; set; }
    public string TopupType { get; set; }
    public string SmsMsg { get; set; }
    public string AgentName { get; set; }
    public string BillerName { get; set; }
    public string ServiceFee { get; set; }
    public string TotalAmount { get; set; }
    public string BranchCode { get; set; }
    public string SenderName { get; set; }
    public string Password { get; set; }
    public string MessageId { get; set; }
    public string BillerLogo { get; set; }

    public string Ref1 { get; set; }
    public string Ref2 { get; set; }
    public string Ref3 { get; set; }
    public string Ref4 { get; set; }
    public string Ref5 { get; set; }

    public string Ref1Name { get; set; }
    public string Ref2Name { get; set; }

    public string Ref3Name { get; set; }

    public string Ref4Name { get; set; }

    public string Ref5Name { get; set; }
    public string TodayTxnAmount { get; set; }
    public string TodayTxnCount { get; set; }
}