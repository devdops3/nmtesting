/// <summary>
/// Summary description for MPT_Eload
/// </summary>
public class MyanPay_Eload
{
    //public MPT_Eload()
    //{
    //    //
    //    // TODO: Add constructor logic here
    //    //
    //}
    public static MyanPay_ELoadResponse ELoadMyanPay(MyanPay_ELoadRequest request)
    {
        TopupService.ResellerServiceClient service = new TopupService.ResellerServiceClient();
        TopupService.TopUpRequest requestObj = new TopupService.TopUpRequest();
        TopupService.TopUpResponse responseObj = new TopupService.TopUpResponse();
        MyanPay_ELoadResponse response = new MyanPay_ELoadResponse();
        requestObj = MapRequest(request);
        responseObj = service.QuickTopUp(requestObj);
        response = MapResponse(responseObj);
        return response;
    }
    private static TopupService.TopUpRequest MapRequest(MyanPay_ELoadRequest request)
    {
        TopupService.TopUpRequest requestobj = new TopupService.TopUpRequest();
        requestobj.CredentialPassword = request.CredentialPassword;
        requestobj.CredentialUserName = request.CredentialUserName;
        requestobj.MobileNumber = request.MobileNumber;
        requestobj.OrderNumber = request.OrderNumber;
        requestobj.TopUpAmount = request.TopUpAmount;
        requestobj.UserEmail = request.UserEmail;
        return requestobj;
    }
    private static MyanPay_ELoadResponse MapResponse(TopupService.TopUpResponse responseObj)
    {
        MyanPay_ELoadResponse response = new MyanPay_ELoadResponse();
        response.InvoiceNumber = responseObj.InvoiceNumber;
        response.OrderNumber = responseObj.OrderNumber;
        response.UniqueResellerID = responseObj.UniqueResellerID;
        response.ResponseMessage = responseObj.ResponseMessage;
        response.Status = responseObj.Status;
        response.DateTime = responseObj.DateTime;

        return response;

    }

    public static MPT_EloadInquiryResponse InquiryELoadMPT(MPT_ELoadInquiryRequest request)
    {
        TopupService.ResellerServiceClient service = new TopupService.ResellerServiceClient();
        TopupService.InquiryQRequest inqrequestObj = new TopupService.InquiryQRequest();
        TopupService.InquiryQResponse inqresObj = new TopupService.InquiryQResponse();

        MPT_EloadInquiryResponse inqres = new MPT_EloadInquiryResponse();

        inqrequestObj = MapInquiryRequest(request);

        inqresObj = service.InquiryTopUp(inqrequestObj);
        inqres = MapInquiryResponse(inqresObj);
        return inqres;
        //MPTELoadService.ResellerServiceClient service = new MPTELoadService.ResellerServiceClient();
        //MPTELoadService.TopUpRequest requestObj = new MPTELoadService.TopUpRequest();
        //MPTELoadService.TopUpResponse responseObj = new MPTELoadService.TopUpResponse();

        //MPT_ELoadResponse response = new MPT_ELoadResponse();

        //requestObj = MapRequest(request);
        //responseObj = service.QuickTopUp(requestObj);
        //response = MapResponse(responseObj);


    }

    private static TopupService.InquiryQRequest MapInquiryRequest(MPT_ELoadInquiryRequest request)
    {
        TopupService.InquiryQRequest inqrequestobj = new TopupService.InquiryQRequest();

        inqrequestobj.InvoiceNumber = request.invoiceNumber;
        inqrequestobj.OrderNumber = request.OrderNumber;
        return inqrequestobj;
    }

    private static MPT_EloadInquiryResponse MapInquiryResponse(TopupService.InquiryQResponse responseObj)
    {
        MPT_EloadInquiryResponse response = new MPT_EloadInquiryResponse();
        response.InvoiceNumber = responseObj.InvoiceNumber;
        response.OrderNumber = responseObj.OrderNumber;
        response.DeliveryStatus = responseObj.DeliveryStatus;
        response.DeliveryReport = responseObj.DeliveryReport;


        return response;

    }

}