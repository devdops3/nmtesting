using System.Configuration;
using System.Data;
using System.Xml;

/// <summary>
/// Summary description for YPayHelper
/// </summary>

public class YpayMeterBillInfoRequest
    {
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string MeterNo { get; set; }
        public string BillDate { get; set; }
        public string BankSecurityCode { get; set; }
        public string TownshipCode { get; set; }

        public void setData(DataSet ds)
        {
            EasyPay.Encryption uncode = new EasyPay.Encryption();
            const string code = "7fcd9ea4-ec70-4a2c-aedd-1bd6328a5ad1";
            this.BankID = ds.Tables[0].Rows[0][0].ToString();
            this.BankName = ds.Tables[0].Rows[0][1].ToString();
            this.MeterNo = uncode.Encrypt(ds.Tables[0].Rows[0][2].ToString(), code);
            this.BillDate = uncode.Encrypt(ds.Tables[0].Rows[0][3].ToString(), code);
            this.BankSecurityCode = uncode.Encrypt(ds.Tables[0].Rows[0][4].ToString(), code);
            this.TownshipCode = uncode.Encrypt(ds.Tables[0].Rows[0][5].ToString(), code);


        }
    }

    public class YpayMeterBillInfoResponse
    {
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string MeterNo { get; set; }
        public string EbillingSecurityCode { get; set; }
        public string OwnerName { get; set; }
        public string BillMonth { get; set; }
        public string TownshipCode { get; set; }
        public string BillCharge { get; set; }
        public string CurrencyCode { get; set; }
        public string LastDate { get; set; }
        public string Status { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }


        public void setData(YpayService.MeterBillResponse response, out string xml, out string billCharege, out string OwnerName, out string lastDate, out string MeterStatus, out string meterResCode, out string meterResDesc)
        {

            EasyPay.Encryption uncode = new EasyPay.Encryption();

            string code = ConfigurationManager.AppSettings["MeterEncryptKEY"].ToString();
            XmlDocument xd = new XmlDocument();
            xd.CreateCDataSection(response.Status);
            this.BankID = response.BankID;
            this.BankName = response.BankName;
            this.BillCharge = uncode.Decrypt(response.BillCharge, code);
            billCharege = this.BillCharge;
            this.BillMonth = uncode.Decrypt(response.BillDate, code);
            //this.CurrencyCode = uncode.Decrypt(response.CurrencyCode, code);
            this.EbillingSecurityCode = response.SecurityCode;
            this.MeterNo = uncode.Decrypt(response.MeterNo, code);
            this.OwnerName = uncode.Decrypt(response.OwnerName, code);
            OwnerName = this.OwnerName;
            this.TownshipCode = uncode.Decrypt(response.TownshipCode, code);
            this.LastDate = uncode.Decrypt(response.LastDate, code);
            lastDate = this.LastDate;
            this.Status = uncode.Decrypt(response.Status, code);
            MeterStatus = this.Status;
            this.ResponseCode = uncode.DecryptKey(response.ResponseCode);
            meterResCode = this.ResponseCode;
            this.ResponseDescription = uncode.DecryptKey(response.ResponseDescription);
            meterResDesc = this.ResponseDescription;

            xml = "<MeterBillResponse> " +
        "<BankID> " + this.BankID + "</BankID>" +
        "<BankName>" + this.BankName + "</BankName>" +
        "<MeterNo>" + this.MeterNo + "</MeterNo>" +
                                            "<EbillingSecurityCode>" + this.EbillingSecurityCode + "</EbillingSecurityCode>" +
        "<OwnerName>" + this.OwnerName + "</OwnerName>" +
        "<BillMonth>" + this.BillMonth + "</BillMonth>" +
        "<TownshipCode>" + this.TownshipCode + "</TownshipCode>" +
        "<BillCharge>" + this.BillCharge + "</BillCharge>" +
        "<CurrencyCode>" + this.CurrencyCode + "</CurrencyCode>" +
    "<LastDate>" + this.LastDate + "</LastDate>" +
    "<Status>" + this.Status + "</Status>" +
        "<ResponseCode>" + this.ResponseCode + "</ResponseCode>" +
      "<ResponseDescription>" + this.ResponseDescription + "</ResponseDescription>" +

      "</MeterBillResponse>";


        }
    }

    public class YpayMeterBillPaymentRequest
    {
        EasyPay.Encryption uncode = new EasyPay.Encryption();
        const string code = "7fcd9ea4-ec70-4a2c-aedd-1bd6328a5ad1";
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string MeterNo { get; set; }
        public string BillMonth { get; set; }
        public string BankSecurityCode { get; set; }
        public string TownshipCode { get; set; }
        public string BillCharge { get; set; }
        public string CurrencyCode { get; set; }

        public void setData(DataSet ds)
        {
            this.BankID = ds.Tables[0].Rows[0][0].ToString();
            this.BankName = ds.Tables[0].Rows[0][1].ToString();
            this.MeterNo = uncode.Encrypt(ds.Tables[0].Rows[0][2].ToString(), code);
            this.BillMonth = uncode.Encrypt(ds.Tables[0].Rows[0][3].ToString(), code);
            this.BankSecurityCode = uncode.Encrypt(ds.Tables[0].Rows[0][4].ToString(), code);
            this.TownshipCode = uncode.Encrypt(ds.Tables[0].Rows[0][5].ToString(), code);
            this.BillCharge = uncode.Encrypt(ds.Tables[0].Rows[0][6].ToString(), code);
            this.CurrencyCode = uncode.Encrypt(ds.Tables[0].Rows[0][7].ToString(), code);

        }

    }

    public class YpayMeterBillPaymentResponse
    {
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string MeterNo { get; set; }
        public string EbillingSecurityCode { get; set; }
        public string OwnerName { get; set; }
        public string BillMonth { get; set; }
        public string TownshipCode { get; set; }
        public string BillCharge { get; set; }
        public string CurrencyCode { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string ApprovalCode { get; set; }

        public void setData(YpayService.PaidMeterBillResponse response, out string xml, out string meterPaidResCode, out string meterPaidResDesc, out string approvalCode)
        {

            EasyPay.Encryption uncode = new EasyPay.Encryption();
            string code = ConfigurationManager.AppSettings["MeterEncryptKEY"].ToString(); ;
            this.BankID = response.BankID;
            this.BankName = response.BankName;
            this.MeterNo = uncode.Decrypt(response.MeterNo, code);
            this.EbillingSecurityCode = response.SecurityCode;
            this.OwnerName = uncode.Decrypt(response.OwnerName, code);
            this.BillMonth = uncode.Decrypt(response.BillDate, code);
            this.TownshipCode = uncode.Decrypt(response.TownshipCode, code);
            this.BillCharge = uncode.Decrypt(response.BillCharge, code);
            //this.CurrencyCode = response.CurrencyCode;
            this.ResponseCode = uncode.DecryptKey(response.ResponseCode);
            meterPaidResCode = this.ResponseCode;
            this.ResponseDescription = uncode.DecryptKey(response.ResponseDescription);
            meterPaidResDesc = this.ResponseDescription;
            this.ApprovalCode = uncode.Decrypt(response.ApprovalCode, code);
            approvalCode = this.ApprovalCode;
            //this.Status = uncode.Decrypt(response, code);


            xml = "<PaidResponse> " +
   "<BankID> " + this.BankID + "</BankID>" +
   "<BankName>" + this.BankName + "</BankName>" +
   "<MeterNo>" + this.MeterNo + "</MeterNo>" +
    "<EbillingSecurityCode>" + this.EbillingSecurityCode + "</EbillingSecurityCode>" +
   "<OwnerName>" + this.OwnerName + "</OwnerName>" +
   "<BillMonth>" + this.BillMonth + "</BillMonth>" +
   "<TownshipCode>" + this.TownshipCode + "</TownshipCode>" +
   "<BillCharge>" + this.BillCharge + "</BillCharge>" +
   "<CurrencyCode>" + this.CurrencyCode + "</CurrencyCode>" +
   "<ResponseCode>" + this.ResponseCode + "</ResponseCode>" +
 "<ResponseDescription>" + this.ResponseDescription + "</ResponseDescription>" +
 "<ApprovalCode>" + this.ApprovalCode + "</ApprovalCode>" +
 "</PaidResponse>";

        }
    }
