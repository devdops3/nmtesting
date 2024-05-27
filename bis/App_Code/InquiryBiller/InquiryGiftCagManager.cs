/// <summary>
/// Summary description for InquiryGiftCagManager
/// </summary>
public class InquiryGiftCagManager
{
	public InquiryGiftCagManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    
    public string Inquiry2GiftCagBiller(InquiryGiftCardRequest request)
    {
        Utils.WriteLog_Biller("This is " + request.TaxId + "Inquiry.");
        inquiryResponseModel inqresmdl = new inquiryResponseModel();

        inqresmdl.ResCode = request.ResCode;
        inqresmdl.ResDesc = request.ResDesc;
        inqresmdl.taxID = request.TaxId;
        inqresmdl.merchantname = request.MerchantName;
        inqresmdl.merchantlogo = request.MerchantLogo;
        inqresmdl.billername = request.BillerName;
        inqresmdl.billerlogo = request.BillerLogo;
        inqresmdl.ref1 = string.IsNullOrEmpty(request.Ref1Name) ? request.BillerName : request.Ref1;             //Ref1 is CardType or PackageName or BillerName
        inqresmdl.ref2 = request.Ref2;
        inqresmdl.ref3 = request.Ref3;
        inqresmdl.ref4 = request.Ref4;
        inqresmdl.ref5 = request.Ref5;
        inqresmdl.ref1Name = request.Ref1Name;
        inqresmdl.ref2Name = request.Ref2Name;
        inqresmdl.ref3Name = request.Ref3Name;
        inqresmdl.ref4Name = request.Ref4;
        inqresmdl.ref5Name = request.Ref5Name; //** Note ** Mobile assign to Ref 5 as product code althought portal setup MobileNo (Deno Case)
        inqresmdl.amount = request.Amount;
        inqresmdl.serviceFee = request.ServiceFee;
        inqresmdl.status = request.Status;
        inqresmdl.expiry = request.Expiry;
        inqresmdl.productDescription = request.ProductDesc;
        inqresmdl.imgUrl = request.ImageUrl;
        return Utils.getInquiryRes(inqresmdl);
    }
}