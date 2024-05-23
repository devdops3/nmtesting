/// <summary>
/// Summary description for CreditLimitModel
/// </summary>
public class CreditLimitModel
{
    public string credittermstart { get; set; }
    public string creditterm { get; set; }
    public double avlBal { get; set; }
    public double ledBal { get; set; }
    public string creditlimit { get; set; }
    public string totalAgentAmount { get; set; }
}
public class CreditLimitResultModel
{
    public string rescode { get; set; }
    public string resdesc { get; set; }
    public bool result { get; set; }
}

