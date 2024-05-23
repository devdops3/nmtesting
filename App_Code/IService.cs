using System.Runtime.Serialization;
using System.ServiceModel;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
[ServiceContract]
public interface IService
{
    [OperationContract]
    string GetSkyNetPackages(string requestXML);

    [OperationContract]
    string GetData(int value);

    [OperationContract]
    CompositeType GetDataUsingDataContract(CompositeType composite);

    [OperationContract]
    string inquiry2Biller(string reqXml);

    [OperationContract]
    string getCanalPlusPackages(string reqXml);

    [OperationContract]
    string ConfirmToBiller(string reqXml);

    [OperationContract]
    string SystemSettingReq(string reqXml);

    [OperationContract]
    string CountryListReq(string reqXml);

    [OperationContract]
    string RegisterReq(string reqXml);

    [OperationContract]
    string RepaymentServiceFeeReq(string reqXml);

    [OperationContract]
    bool doSMS(long txnID, string mobileNo);

    [OperationContract]
    bool SendUnpairEmail(string userID);

    [OperationContract]
    string OTPReq(string reqXml);
    [OperationContract]
    string OTPReqV2(string reqXml);

    [OperationContract]
    string RegisterUser(string reqXml);

    [OperationContract]
    string UpdateProfileReq(string reqXml);

    [OperationContract]
    string UpdateProfileReqV2(string reqXml);

    [OperationContract]
    bool InsertTransactionForPaymentAPI(string taxID, string agentID, string email, string ref1, string ref2,
        string amount, string version, string locLatitude, string locLongitude, string productdesc, string appType,
        string messageid, string paymentMethod, string agentCode, string agentBranchCode, out string agentTxnID);

    [OperationContract]
    bool UpdateTransactionForPaymentAPI(long txnID, string email, string ref1, string ref2, string isAgreement,
        double availablebalance);

    [OperationContract]
    bool ConfirmRequestToPG(long txnID);

    [OperationContract]
    string UpdateErrorWithAddBalance(string rescode, string resdesc, long txnID, string logerrormessage, int agentID,
        double amount, string isAgreement);

    [OperationContract]
    bool InsertTransactionInvoice(string reqXml, long txnID, string paymentMethod);

    [OperationContract]
    string TelenorBBInquiry(string reqXML);

    [OperationContract]
    string FtthOrWtthInquiry(string reqXML);

    [OperationContract]
    string MptPackageInquiry(string reqXML);

    [OperationContract]
    string ParamiGasPackageInquiry(string reqXML);

    [OperationContract]
    string GetGiftCardDenoList(string taxId);

    [OperationContract]
    string GetProductList(string taxId);

    [OperationContract]
    string OoredooPackageInquiry(string reqXML);

    [OperationContract]
    string MyTelPackageInquiry(string reqXML);

    [OperationContract]
    string EventInquiry(string reqXml);

    [OperationContract]
    string MerchantAcceptanceReq(string reqXml);

    [OperationContract]
    string EPaymentVoidReq(string reqXml);

    [OperationContract]
    string GetAstrologerList(string taxId, string serviceId);

    [OperationContract]
    string B2BCancel(string reqXml);
}

// Use a data contract as illustrated in the sample below to add composite types to service operations.
[DataContract]
public class CompositeType
{
	bool boolValue = true;
	string stringValue = "Hello ";

	[DataMember]
	public bool BoolValue
	{
		get { return boolValue; }
		set { boolValue = value; }
	}

	[DataMember]
	public string StringValue
	{
		get { return stringValue; }
		set { stringValue = value; }
	}
}
