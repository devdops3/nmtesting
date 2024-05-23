/// <summary>
/// Summary description for OneTwoThreeRemit
/// </summary>
public class OneTwoThreeRemit
{
	public OneTwoThreeRemit()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static decimal GetServiceFee(decimal CashInFee, decimal CashOutFee, int PaidBy, int RemittanceType)
    {
        decimal ServiceFee = 0;
        switch (PaidBy)
        {
            case (1):
                {
                    if (RemittanceType == 1)
                    {
                        ServiceFee = CashInFee + CashOutFee;
                    }
                    else
                    {
                        ServiceFee = 0;
                    }
                }
                break;
            case (2):
                {
                    if (RemittanceType == 1)
                    {
                        ServiceFee = 0;
                    }
                    else
                    {
                        ServiceFee = CashInFee + CashOutFee;
                    }

                }
                break;
            case (3):
                {
                    if (RemittanceType == 1)
                    {
                        ServiceFee = CashInFee;
                    }
                    else
                    {
                        ServiceFee = CashOutFee;
                    }
                }
                break;
            default:
                break;
        }

        return ServiceFee;
    }

    public static decimal GetTransactionAmount(decimal Amount, decimal CashInFee, decimal CashOutFee, int PaidBy, int RemittanceType)
    {
        decimal transactionamount = 0;

        switch (PaidBy)
        {
            case (1): //Payer
                {
                    if (RemittanceType == 1) //CashIn
                    {
                        transactionamount = Amount + CashOutFee;
                    }
                    else // CashOut
                    {
                        transactionamount = Amount + CashOutFee;
                    }
                }
                break;
            case (2): // Payee
                {
                    if (RemittanceType == 1) // CashIn
                    {
                        transactionamount = Amount - CashInFee;
                    }
                    else // CashOut
                    {
                        transactionamount = Amount - CashInFee;
                    }

                }
                break;
            case (3)://Share
                {
                    if (RemittanceType == 1) // CashIn
                    {
                        transactionamount = Amount;
                    }
                    else // CashOut
                    {
                        transactionamount = Amount;
                    }
                }
                break;
            default:
                break;
        }

        return transactionamount;
    }

    public static decimal GetPartnerAmount(decimal partnerFee, decimal servicefee, decimal transactionamount, int remittanceType)
    {
        decimal PartnerAmount = 0;

        if (remittanceType == 1)
        {
            PartnerAmount = (transactionamount + servicefee) - partnerFee;
        }
        else
        {
            PartnerAmount = (transactionamount - servicefee) + partnerFee;

        }

        return PartnerAmount;
    }

}