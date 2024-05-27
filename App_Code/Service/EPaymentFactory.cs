public class EPaymentFactory
{

    public IEPayment CreateEPayment(string ePaymentType, string paymentMode)
    {

        switch (paymentMode)
        {
            case "B2C":
                return ResolveB2CEPayment(ePaymentType);
            case "C2B":
                return ResolveC2BEPayment(ePaymentType);
            default:
                return null;
        }
    }

    private IEPayment ResolveB2CEPayment(string ePaymentType)
    {
        switch (ePaymentType)
        {
            case BISConstants.MoMoneyPay:
                return new MoMoney();
            case BISConstants.APlus:
                return new APlus();
            default:
                return null;
        }
    }
    private IEPayment ResolveC2BEPayment(string ePaymentType)
    {
        switch (ePaymentType)
        {
            case "AYAPay":
                return new AyaPayC2B();
            default:
                return null;
        }
    }
}