public interface IEPayment
{
    string Confirm(EPaymentMerchant merchant,
                long txnId,
                string ref1,
                string ref2,
                string ref3,
                string ref4,
                string ref5,
                string email,
                string password,
                string amount,
                string branchCode,
                string agentName,
                string appType,
                string productDesc);
}