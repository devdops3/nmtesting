using System;
using System.Collections;
using System.Collections.Generic;

namespace EbaReqResModel
{
    public class EbaInquiryReq
    {
        public string Token { get; set; }
        public string PartnerCode { get; set; }
        public string BillerCode { get; set; }
        public string Detail { get; set; }
    }

    public class EbaInquiryRes
    {
        public string TransactionStatus { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string PartnerAmount { get; set; }
        public string TransactionAmount { get; set; }
        public string Detail { get; set; }
    }

    public class EbaConfirmReq
    {
        public string Token { get; set; }
        public string PartnerCode { get; set; }
        public string BillerCode { get; set; }
        public string PartnerRefNo { get; set; }
        public string TransactionAmount { get; set; }
        public string Detail { get; set; }
    }

    public class EbaConfirmRes
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

    public class MescDetail
    {
        public string Townshipcode { get; set; }
        public string LedgerNo { get; set; }
        public string MeterNo { get; set; }
        public string UniqueId { get; set; }
        public string EpcRefNo { get; set; }
        public string CustomerName { get; set; }
        public string TownshipNameMM { get; set; }
        public string TownshipNameEn { get; set; }
        public string Address { get; set; }
        public string PreviousUnit { get; set; }
        public string CurrentUnit { get; set; }
        public string UsedUnit { get; set; }
        public string ReadingDate { get; set; }
        public string DueDate { get; set; }
        public string MaintenanceFees { get; set; }
        public string HorsePower { get; set; }
        public string HorsePowerCharges { get; set; }
        public string TotalAmount { get; set; }
        public string BillingPeriod { get; set; }
    }

    public class MobileLegendEBADetailResponse
    {
        public string CustomerId { get; set; }
        public string SessionId { get; set; }
        public string UserName { get; set; }
        public string ProductCode { get; set; }
    }

    public class EasyMicrofinanceDetailResponse 
    {
        public ClientInfo clientInfo { get; set; }
        public List<Loan> Loans { get; set; }
    }

    public class ClientInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string LoanCycle { get; set; }
    }

    public class Loan
    {
        public string LoanId { get; set; }
        public string LoanName { get; set; }
        public string OriginalLoanAmount { get; set; }
        public string InterestRate { get; set; }
        public string InstallationAmount { get; set; }
        public string UnpaidAmount { get; set; }
        public string PrincipalDue { get; set; }
        public string CurrentPaid { get; set; }
        public string CurrentInterestPaid { get; set; }
        public string CurrentPenaltyPaid { get; set; }
        public string CurrentTaxOnInterestPaid { get; set; }
        public string AccountState { get; set; }
        public string RepaymentPeriodCount { get; set; }
        public string RepaymentPeriodUnit { get; set; }
        public string RepaymentInstallments { get; set; }
    }

 
    public class _5BBDetail
    {
        public string CustomerName { get; set; }
        public string CustomerAccountNo { get; set; }
        public string InvoiceID { get; set; }
        public string Amount { get; set; }
    }

    public class YescDetail
    {
        public string MeterRefNo { get; set; }
        public string MeterBillDetailId { get; set; }
        public string MeterNo { get; set; }
        public string LedgerNo { get; set; }
        public string DueDate { get; set; }
        public string Amount { get; set; }
        public string TransactionRefId { get; set; }
        public string PartnerServiceFee { get; set; }
        public string TotalAmount { get; set; }
        public string TransactionDate { get; set; }
    }

    public class SaiSaiPayDetail
    {
        public string Type { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string Amount { get; set; }
        public string Remark { get; set; }
    }

    public class MSPTopUpDetail
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string BillerReferenceNo { get; set; }
        public string Amount { get; set; }
    }

    public class AnadaEloadDetail
    {
        public string SubscriberId { get; set; }
    }

    public class QuicKyatDetail
    {
        public string AccountId { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string Nrc { get; set; }
        public string Amount { get; set; }
    }

    public class YadanarponTeleportDetail
    {
        public string BillType { get; set; }
        public string RegionCode { get; set; }
        public string CustomerBillingId { get; set; }
        public string RegionNameMM { get; set; }
        public string RegionNameEn { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string ContactNo { get; set; }
    }

    public class AeonDetail
    {
        public string AgreementNo { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerName { get; set; }
        public string RepaymentAmount { get; set; }
        public string PaidAmount { get; set; }
    }

    public class HanaMicrofinanceDetailResponse
    {
        public string ChannelReferenceId { get; set; }
        public string BillerReferenceId { get; set; }
        public string LoanAccountNumber { get; set; }
        public string Amount { get; set; }
        public string DueDate { get; set; }
        public string CustomerName { get; set; }
        public string OfficeName { get; set; }
    }

    public class EBAGiftCardEPinDetail
    {
        public string ProductCode { get; set; }
        public string PinId { get; set; }
        public string SerialNumber { get; set; }
        public string ClearPin { get; set; }
        public string ExpiryDate { get; set; }
        public string ProductAmount { get; set; }
        public string ProductDescription { get; set; }
    }

    public class EBAPinDetail
    {
        public string ProductCode { get; set; }
        public string CardNumber { get; set; }
        public string CardPin { get; set; }
        public string ActivationCode { get; set; }
        public string ActivationUrl { get; set; }
        public string ExpiryDate { get; set; }
    }

    public class DataPackDetail
    {
        public string Deno { get; set; }
        public string MobileNumber { get; set; }
    }

}