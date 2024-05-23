/// <summary>
/// Summary description for ConfirmBillerRequest
/// </summary>
public class ConfirmBillerRequest
{
    public string TaxId { get; set; }
    
    public string Ref4 { get; set; }
    public string IsAgreement { get; set; }
    public int AgentID { get; set; }
    public string AgentName { get; set; }
    public string PhoneNumber { get; set; }
    public double AgentAmt { get; set; }
    public string Email { get; set; }
    public bool IsSMS { get; set; }
    public string AppType { get; set; }
    public string TopupType { get; set; }
    public string BillerName { get; set; }
    public string BillerLogo { get; set; }
    public string SMSStatus { get; set; }
    public string Password { get; set; }
    public string MessageId { get; set; }
    
    public string Ref1Name { get; set; }

    public string Ref2Name { get; set; }

    public string Ref3Name { get; set; }

    public string Ref4Name { get; set; }

    public string Ref5Name { get; set; }
    public string TodayTxnAmount { get; set; }
    public string TodayTxnCount { get; set; }

   
}