using System.Xml.Serialization;

public class OkDollarCashIn
{
    #region Request
    public class AuthenticationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProjectId { get; set; }
    }
    public class GetUserInformationRequest
    {
        public string MobileNumber { get; set; }
        public string Channel { get; set; }
    }
    public class PaymentRequest 
    {
        public string NearmeMerchantID { get; set; }
        public string TransactionId { get; set; }
        public string DestinationNumber { get; set; }
        public string Amount { get; set; }
        public string Comments { get; set; }
        public string MobileNumber { get; set; }
        public string SimId { get; set; }
        public string MsId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CellId { get; set; }
        public string ProfileImg { get; set; }
        public string ProjectId { get; set; }
        public int Channel { get; set; }
    }
    public class QueryRequest
    {
        public string NearmeMerchantID { get; set; }
        public string TransactionId { get; set; }
        public string DestinationNumber { get; set; }
        public int Amount { get; set; }
        public string Comments { get; set; }
        public string MobileNumber { get; set; }
        public string SimId { get; set; }
        public string MsId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CellId { get; set; }
        public string ProfileImg { get; set; }
        public string ProjectId { get; set; }
        public int Channel { get; set; }
    }
    #endregion

    #region Response
    public class OkDollarResponse<T> where T : class
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public T Data { get; set; } 
    }
    public class AuthenticationResponse {
        public string Token { get; set; }
        public string ExpriesIn { get; set; }
    }
    public class GetUserInformationResponse {
        public string MercantName { get; set; }
        public string MobileNumber { get; set; }
        public int TransactionLimit { get; set; }
        public string AccountType { get; set; }
    }

    public class PaymentResponse
    {
        public string OkTransactionId { get; set; }
        public string DestinationNumber { get; set; }
        public string NearmeMerchantID { get; set; }
        public string Amount { get; set; }
        public string RequestCts { get; set; }
        public string ResponseCts { get; set; }
    }
    public class QueryResponse
    {
        public string OkTransactionId { get; set; }
        public string DestinationNumber { get; set; }
        public string NearmeMerchantID { get; set; }
        public string Amount { get; set; }
        public string RequestCts { get; set; }
        public string ResponseCts { get; set; }
    }

    [XmlRoot(ElementName = "InquiryRes")]
    public class ResponseXML
    {
        public string Version { get; set; }
        public string TimeStamp { get; set; }
        public string ResCode { get; set; }
        public string ResDesc { get; set; }
        public string TaxID { get; set; }
        public string MerchantName { get; set; }
        public string MerchantLogo { get; set; }
        public string BillerName { get; set; }
        public string BillerLogo { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public string Ref6 { get; set; }
        public string Ref1Name { get; set; }
        public string Ref2Name { get; set; }
        public string Ref3Name { get; set; }
        public string Ref4Name { get; set; }
        public string Ref5Name { get; set; }
        public string Ref6Name { get; set; }
        public string Amount { get; set; }
        public string AgentFee { get; set; }
        public string Status { get; set; }
        public string Expiry { get; set; }
        public string ProductDesc { get; set; }
        public string ImageURL { get; set; }
    }
    #endregion
}