using System;
using System.Collections;
using System.Configuration;
using System.Text;
//using BillsAPI.Model;
public class BillPayment
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region Comment
        
        #endregion
        public void EnquiryBillAmount(ReqBillAmount req,out ResBillAmount res,out ResError err,out string errMsg)
        {
            log.Debug("This is Payee EnquiryBillAmount Request");
            res = new ResBillAmount();
            err = new ResError();
            errMsg = string.Empty;

            try
            {
               
                string merchanturl = ConfigurationManager.AppSettings["MerchantAPIUrl"].ToString();
                string getBillAmounturl = ConfigurationManager.AppSettings["getBillAmountUrl"].ToString();
                string xid = Guid.NewGuid().ToString();
                StringBuilder strPostData = new StringBuilder();
                strPostData.Append(merchanturl);
                strPostData.Append(getBillAmounturl);
                strPostData.Append("?xid=");
                strPostData.Append(xid);
                strPostData.Append("&apiKey=");
                strPostData.Append(req.apiKey);
                strPostData.Append("&billRefNo=");
                strPostData.Append(req.billRefNo);
                strPostData.Append("&custRefNo=");
                strPostData.Append(req.custRefNo);
                strPostData.Append("&locale=");
                strPostData.Append(req.locale);
                strPostData.Append("&billingMerchantCode=");
                strPostData.Append(req.billingMerchantCode);
                strPostData.Append("&currencyCode=");
                strPostData.Append(req.currencyCode);



                SSLPost sslPost = new SSLPost();                
                string responseMsg = string.Empty;
                log.Debug("Request Data:"+strPostData.ToString());
                if (sslPost.postDatautil(strPostData.ToString(), out responseMsg, out errMsg))
                {
                    log.Debug("Inquiry Response Message From Payee:" + responseMsg);
                    responseMsg = responseMsg.Replace("[", "");
                    responseMsg = responseMsg.Replace("]", "");

                    Hashtable hashtable = Utils.ConvertJSONtoHashTable(responseMsg);
                    if (hashtable.ContainsKey("status"))
                    {
                        res.status = hashtable["status"].ToString();
                    }
                    if (hashtable.ContainsKey("totalAmount"))
                    {
                        res.totalAmount = hashtable["totalAmount"].ToString();
                    }
                    if (hashtable.ContainsKey("name"))
                    {
                        res.name = hashtable["name"].ToString();
                    }
                    if (hashtable.ContainsKey("amount"))
                    {
                        res.amount = hashtable["amount"].ToString();
                    }
                    if (hashtable.ContainsKey("message"))
                    {
                        res.message = hashtable["message"].ToString();
                    }
                    if (hashtable.ContainsKey("custRefNo"))
                    {
                        res.custRefNo = hashtable["custRefNo"].ToString();
                    }
                    if (hashtable.ContainsKey("billRefNo"))
                    {
                        res.billRefNo = hashtable["billRefNo"].ToString();
                    }

                    if (hashtable.ContainsKey("code"))
                    {
                        res.failcode = hashtable["code"].ToString();
                    }
                    res.Codescription = Utils.ReadJson("commission", "description", "N", responseMsg);
                    res.Coamount = Utils.ReadJson("commission", "amount", "N", responseMsg);
                    res.Cocurrency = Utils.ReadJson("commission", "currency", "N", responseMsg);
                    res.Cdescription = Utils.ReadJson("charges", "description", "N", responseMsg);
                    res.Ccode = Utils.ReadJson("charges", "code", "N", responseMsg);
                    res.Camount = Utils.ReadJson("charges", "amount", "N", responseMsg);
                    res.Ccurrency = Utils.ReadJson("charges", "currency", "N", responseMsg);

                    res.Camount = (res.Camount == string.Empty ? "0" : res.Camount);
                    res.Coamount = (res.Coamount == string.Empty ? "0" : res.Coamount);

                    //if (string.IsNullOrEmpty(res.status))
                    //{
                    //    if (hashtable.ContainsKey("Status"))
                    //    {
                    //        res.status = hashtable["Status"].ToString();
                    //    }
                    //    if (hashtable.ContainsKey("errorMessage"))
                    //    {
                    //        err.errorMessage = hashtable["errorMessage"].ToString();
                    //    }
                    //    if (hashtable.ContainsKey("errorCode"))
                    //    {
                    //        err.errorCode = hashtable["errorCode"].ToString();
                    //    }
                    //}
                   

                }
                else
                {
                    log.Debug("No Response");
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                log.Debug("Error in EnquiryBillAmount " + ex.Message);
            }
        }

        public void PaymentByCash(ReqPaymentByCash req,out ResPaymentByCash res,out ResError err,out string errMsg)
        {
           
            res = new ResPaymentByCash();
            err = new ResError();
            errMsg = string.Empty;

            try
            {
                string merchanturl = ConfigurationManager.AppSettings["MerchantAPIUrl"].ToString();
                string getBillAmounturl = ConfigurationManager.AppSettings["getPaymentUrl"].ToString();
                string xid = Guid.NewGuid().ToString();
                StringBuilder strPostData = new StringBuilder();
                strPostData.Append(merchanturl);
                strPostData.Append(getBillAmounturl);
                strPostData.Append("?xid=");
                strPostData.Append(xid);
                strPostData.Append("&apiKey=");
                strPostData.Append(req.apiKey);
                strPostData.Append("&billingMerchantCode=");
                strPostData.Append(req.billingMerchantCode);
                strPostData.Append("&billAmount=");
                strPostData.Append(req.billAmount);
                strPostData.Append("&chargesAmount=");
                strPostData.Append(req.chargesAmount);
                strPostData.Append("&currencyCode=");
                strPostData.Append(req.currencyCode);
                strPostData.Append("&custRefNo=");
                strPostData.Append(req.custRefNo);
                strPostData.Append("&billRefNo=");
                strPostData.Append(req.billRefNo);

                strPostData.Append("&transferRefNo1=");
                strPostData.Append(req.transferRefNo1);
                strPostData.Append("&transferRefNo1=");
                strPostData.Append(req.transferRefNo2);
                strPostData.Append("&extRefNo1=");
                strPostData.Append(req.extRefNo1);

                SSLPost sslPost = new SSLPost();                
                string responseMsg = string.Empty;
                log.Debug("Confirm Request Message" + strPostData.ToString());
                if (sslPost.postDatautil(strPostData.ToString(), out responseMsg, out errMsg))
                {
                    log.Debug("Confirm Response Message From Payee:" + responseMsg);
                    responseMsg = responseMsg.Replace("[", "");
                    responseMsg = responseMsg.Replace("]", "");

                    Hashtable hashtable = Utils.ConvertJSONtoHashTable(responseMsg);
                    if (hashtable.ContainsKey("status"))
                    {
                        res.status = hashtable["status"].ToString();
                    }

                    if (hashtable.ContainsKey("txnId"))
                    {
                        res.txnId = hashtable["txnId"].ToString();
                    }
                    if (hashtable.ContainsKey("txnDate"))
                    {
                        res.txnDate = hashtable["txnDate"].ToString();
                    }
                    if (hashtable.ContainsKey("billAmount"))
                    {
                        res.resbillAmount = hashtable["billAmount"].ToString();
                    }
                    if (hashtable.ContainsKey("totalBillAmount"))
                    {
                        res.totalBillAmount = hashtable["totalBillAmount"].ToString();
                    }
                    if (hashtable.ContainsKey("code"))
                    {
                        err.errorCode = hashtable["code"].ToString();
                    }
                    res.authCode = Utils.ReadJson("paymentDetails", "authCode", "N", responseMsg);
                    res.merchantCode = Utils.ReadJson("paymentDetails", "merchantCode", "N", responseMsg);
                    res.PbillAmount = Utils.ReadJson("paymentDetails", "billAmount", "N", responseMsg);
                    res.paymentType = Utils.ReadJson("paymentDetails", "paymentType", "N", responseMsg);
                    res.customerReferenceNo = Utils.ReadJson("paymentDetails", "customerReferenceNo", "N", responseMsg);
                    res.Pcurrency = Utils.ReadJson("paymentDetails", "currency", "N", responseMsg);

                    res.Codescription = Utils.ReadJson("commission", "description", "N", responseMsg);
                    res.Coamount = Utils.ReadJson("commission", "amount", "N", responseMsg);
                    res.Cocurrency = Utils.ReadJson("commission", "currency", "N", responseMsg);
                    res.Cdescription = Utils.ReadJson("charges", "description", "N", responseMsg);
                    res.Ccode = Utils.ReadJson("charges", "code", "N", responseMsg);
                    res.Camount = Utils.ReadJson("charges", "amount", "N", responseMsg);
                    res.Ccurrency = Utils.ReadJson("charges", "currency", "N", responseMsg);

                    if (string.IsNullOrEmpty(res.status))
                    {
                        if (hashtable.ContainsKey("Status"))
                        {
                            res.status = hashtable["Status"].ToString();
                        }
                        if (hashtable.ContainsKey("errorMessage"))
                        {
                            err.errorMessage = hashtable["errorMessage"].ToString();
                        }
                        if (hashtable.ContainsKey("errorCode"))
                        {
                            err.errorCode = hashtable["errorCode"].ToString();
                        }
                        
                    }

                }
                else
                {
                    log.Debug("No Response");
                    log.Debug("Error on Post to CNP:"+errMsg.ToString());
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
            }
        }

        public void BillDetails(ReqBillAmount req, out ResBillAmount res, out ResError err, out string errMsg)
        {
            res = new ResBillAmount();
            err = new ResError();
            errMsg = string.Empty;

            try
            {

                string merchanturl = ConfigurationManager.AppSettings["MerchantAPIUrl"].ToString();
                string getBillAmounturl = ConfigurationManager.AppSettings["getBillDetails"].ToString();
                string xid = Guid.NewGuid().ToString();
                StringBuilder strPostData = new StringBuilder();
                strPostData.Append(merchanturl);
                strPostData.Append(getBillAmounturl);
                strPostData.Append("?xid=");
                strPostData.Append(xid);
                strPostData.Append("&apiKey=");
                strPostData.Append(req.apiKey);
                strPostData.Append("&billRefNo=");
                strPostData.Append(req.billRefNo);
                strPostData.Append("&custRefNo=");
                strPostData.Append(req.custRefNo);
                strPostData.Append("&locale=");
                strPostData.Append(req.locale);
                strPostData.Append("&billingMerchantCode=");
                strPostData.Append(req.billingMerchantCode);


                SSLPost sslPost = new SSLPost();
                string responseMsg = string.Empty;
                    log.Debug("Request Data:"+strPostData.ToString());
                
                  
                if (sslPost.postDatautil(strPostData.ToString(), out responseMsg, out errMsg))
                {
                    log.Debug("Inquiry Response Message From Payee:" + responseMsg);
                    responseMsg = responseMsg.Replace("[", "");
                    responseMsg = responseMsg.Replace("]", "");

                    Hashtable hashtable = Utils.ConvertJSONtoHashTable(responseMsg);
                    if (hashtable.ContainsKey("status"))
                    {
                        res.status = hashtable["status"].ToString();
                    }
                    if (hashtable.ContainsKey("message"))
                    {
                        err.errorMessage = hashtable["message"].ToString();
                    }
                    if (hashtable.ContainsKey("code"))
                    {
                        err.errorCode = hashtable["code"].ToString();
                    }
                    if (hashtable.ContainsKey("paymentDueDate"))
                    {
                        res.paymentDueDate = hashtable["paymentDueDate"].ToString();
                    }
                    if (hashtable.ContainsKey("isPaid"))
                    {
                        res.paidstatus = hashtable["isPaid"].ToString();
                    }


                    //if (string.IsNullOrEmpty(res.status))
                    //{
                    //    if (hashtable.ContainsKey("Status"))
                    //    {
                    //        res.status = hashtable["Status"].ToString();
                    //    }
                        //if (hashtable.ContainsKey("message"))
                        //{
                        //    err.errorMessage = hashtable["message"].ToString();
                        //}
                    


                   // }


                }
                else
                {
                    log.Debug("No Response");
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                log.Debug("Error in EnquiryBillAmount " + ex.Message);
            }
        }

    }
