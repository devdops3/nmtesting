namespace MotherFinanceModel
{
    public class MotherFinanceModel
    {

    }
    public class MotherFinanceInquiryReq
    {
        public string Token { get; set; }
        public string PartnerCode { get; set; }
        public string BillerCode { get; set; }
        public string Detail { get; set; }
    }

    public class MotherFinanceInquiryRes
    {
        public string TransactionStatus { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string PartnerAmount { get; set; }
        public string TransactionAmount { get; set; }
        public string Detail { get; set; }
    }

    public class MotherFinanceConfirmReq
    {
        public string Token { get; set; }
        public string PartnerCode { get; set; }
        public string BillerCode { get; set; }
        public string PartnerRefNo { get; set; }
        public string TransactionAmount { get; set; }
        public string Detail { get; set; }
    }

    public class MotherFinanceConfirmRes
    {
        public string TransactionStatus { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Detail { get; set; }
        public string EBARefNo { get; set; }
        public string PartnerRefNo { get; set; }
        public string PartnerAmount { get; set; }
        public string TransactionAmount { get; set; }
        public string PartnerBalance { get; set; }

    }

    public class Detail
    {
        public string PaymentScheduleId { get; set; }
        public string LoanCategoryId { get; set; }
        public string LoanCategoryDesc { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string Amount { get; set; }
        public string LateFee { get; set; }
        public string Id { get; set; }
        public bool IsPartialPayment { get; set; }
    }
}