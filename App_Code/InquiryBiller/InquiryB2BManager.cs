using Newtonsoft.Json;

/// <summary>
/// Summary description for Inquiry2Biller
/// </summary>
public class InquiryB2BManager
{
	public InquiryB2BManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string Inquiry2Biller_B2BOrder(InquiryBillerRequest request)
    {
        Utils.WriteLog_Biller("MapTaxID : " + request.TaxId);

        var forwarderService = new ForwarderService.ServiceClient();

        var transactionResponse = forwarderService.GetTransactionByToken(request.Ref1);
        if (!transactionResponse._success)
            return Utils.getErrorRes("06", transactionResponse._message,request.TaxId);

        var result = JsonConvert.DeserializeObject<TblTransaction>(transactionResponse._result);

        var reference1 = result.Refid1;
        var reference3 = result.Refid3;

        inquiryResponseModel inqresmdl = new inquiryResponseModel();
        string rescode = "00";
        string  resdecs = "Success";
        inqresmdl.ResCode = rescode;
        inqresmdl.ResDesc = resdecs;
        inqresmdl.taxID = request.TaxId;
        inqresmdl.merchantname = request.MerchantName;
        inqresmdl.merchantlogo = request.MerchantLogo;
        inqresmdl.billername = request.BillerName;
        inqresmdl.billerlogo = request.BillerLogo;
        inqresmdl.ref1 = reference1;
        inqresmdl.ref2 = result.Refid2;
        inqresmdl.ref3 = reference3;
        inqresmdl.ref4 = result.Transactionid.ToString();
        inqresmdl.ref5 = result.Refid5;
        inqresmdl.ref1Name = request.Ref1Name;
        inqresmdl.ref2Name = request.Ref2Name;
        inqresmdl.ref3Name = request.Ref3Name;
        inqresmdl.ref4Name = request.Ref4Name;
        inqresmdl.ref5Name = request.Ref5Name;
        inqresmdl.amount = result.Transactionamount.ToString();
        inqresmdl.serviceFee = result.Servicefee.ToString();
        inqresmdl.status = request.Status;
        inqresmdl.expiry = request.Expiry;
        inqresmdl.productDescription = request.ProductDesc;
        inqresmdl.imgUrl = request.ImgUrl;
        return Utils.getInquiryRes(inqresmdl);
    }


}