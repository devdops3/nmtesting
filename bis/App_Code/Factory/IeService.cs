using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for IeService
/// </summary>
public interface IeService
{
    string Inquiry(inquiryModel model);
    string Confirm(ConfirmResponseModel model, ResponseInfo responseInfo);
}