using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }
  
    protected void Page_Load(object sender, EventArgs e)
    {

        //string reqxml = @"<InquiryReq><Version>3.0</Version><TimeStamp>202001021628371440</TimeStamp><MessageID>93f06082-a19f-46f0-a8c1-649b91921d36</MessageID><Email>09692922675</Email><Password>PFnco5X7hSb2lH9Qv1b9HFLzm10wOTY5MjkyMjY3NQ==</Password><RequestedBy>09692922675</RequestedBy><LoginType>MS</LoginType><TaxID>0000000000093</TaxID><Ref1>097752679513</Ref1><Ref2>AA</Ref2><Ref3></Ref3><Ref4></Ref4><Ref5></Ref5><Amount></Amount><TopupType>S</TopupType><IsQR></IsQR><DeviceUID>356696595626247</DeviceUID><Token>4cc73763-978a-48db-97a2-c00d522811f1</Token></InquiryReq>";

        //string reqxml = @"<ConfirmReq><Version>1.0</Version><TimeStamp>201712121337291930</TimeStamp><MessageID>" + DateTime.Now + @"</MessageID><Email>bba111</Email><Password>Bi9x1WraNvW+y+o+WlpZW8b/nrpiYmExMTE=</Password><TaxID>5555555555555</TaxID><Ref1></Ref1><Ref2>1000</Ref2><Ref3>09789323948</Ref3><Ref4></Ref4><Ref5></Ref5><Ref6></Ref6><Amount>1000</Amount><TopupType>S</TopupType><LocLatitude>16.8507067</LocLatitude><LocLongitude>96.1598924</LocLongitude><AppType>MS</AppType><AgentFee></AgentFee><ProductDesc></ProductDesc></ConfirmReq>";

        //string reqxml = @"<InquiryReq><Version>1.0</Version><TimeStamp>201712121337291930</TimeStamp><MessageID>" + DateTime.Now + @"</MessageID><Email>kamyli</Email><Password>/420M6HRnM9N764scFZNMC8cbKdrYW15bGk=</Password><RequestedBy>kamyli</RequestedBy><TaxID>0000000000052</TaxID><Ref1>1111111111111</Ref1><Ref2></Ref2><Ref3></Ref3><Ref4></Ref4><Ref5></Ref5><Amount></Amount><TopupType>S</TopupType><IsQR></IsQR></InquiryReq>";

        //string reqxml = @"<inquiryreq><version>1.0</version><timestamp>201712290912441030</timestamp><MessageID>" + DateTime.Now + @"</MessageID><email>kamyli</email><password>/420m6hrnm9n764scfznmc8cbkdryw15bgk=</password><requestedby>kamyli</requestedby><taxid>0000000000052</taxid><ref1>1111111111111</ref1><ref2></ref2><ref3></ref3><ref4></ref4><ref5></ref5><amount></amount><topuptype>s</topuptype><isqr></isqr></inquiryreq>";

        //string reqxml = @"<InquiryReq><Version>1.0</Version><TimeStamp>201712291014233510</TimeStamp><MessageID>05466335-ab31-4bea-8a0a-1d166cf3080a</MessageID><Email>kamyli</Email><Password>/420M6HRnM9N764scFZNMC8cbKdrYW15bGk=</Password><RequestedBy>kamyli</RequestedBy><TaxID>0000000000031</TaxID><Ref1>1111111</Ref1><Ref2></Ref2><Ref3>40607</Ref3><Ref4></Ref4><Ref5></Ref5><Amount></Amount><TopupType>S</TopupType><IsQR></IsQR></InquiryReq>";

        //string reqxml = @"<InquiryReq><Version>1.0</Version><TimeStamp>201712290912441030</TimeStamp><MessageID>" + DateTime.Now + @"</MessageID><Email>kamyli</Email><Password>/420M6HRnM9N764scFZNMC8cbKdrYW15bGk=</Password><RequestedBy>kamyli</RequestedBy><TaxID>0000000000022</TaxID><Ref1>1111111111111</Ref1><Ref2>1000</Ref2><Ref3></Ref3><Ref4></Ref4><Ref5></Ref5><Amount>1000</Amount><TopupType>S</TopupType><IsQR></IsQR></InquiryReq>";

        //string reqxml = @"<ConfirmReq><Version>1.0</Version><TimeStamp>201801031421582550</TimeStamp><MessageID>" + DateTime.Now + @"</MessageID><Email>kamyli</Email><Password>/420M6HRnM9N764scFZNMC8cbKdrYW15bGk=</Password><RequestedBy>kamyli</RequestedBy><TaxID>5555555555555</TaxID><Ref1>5555555555555</Ref1><Ref2>1000</Ref2><Ref3>09794128495</Ref3><Ref4></Ref4><Ref5></Ref5><Ref6></Ref6><Amount>1000</Amount><TopupType>S</TopupType><LocLatitude>16.8489457</LocLatitude><LocLongitude>96.1601554</LocLongitude><AppType>MS</AppType><AgentFee></AgentFee><ProductDesc></ProductDesc></ConfirmReq>";

        //string reqxml = @"<InquiryReq><Version>1.0</Version><TimeStamp>201712290912441030</TimeStamp><MessageID>" + DateTime.Now + @"</MessageID><Email>kamyli</Email><Password>/420M6HRnM9N764scFZNMC8cbKdrYW15bGk=</Password><RequestedBy>kamyli</RequestedBy><TaxID>0000000000052</TaxID><Ref1>1111111111111</Ref1><Ref2></Ref2><Ref3></Ref3><Ref4></Ref4><Ref5></Ref5><Amount></Amount><TopupType>S</TopupType><IsQR></IsQR></InquiryReq>";

        //string reqxml = @"<ConfirmReq><Version>1.0</Version><TimeStamp>201801031421582550</TimeStamp><MessageID>" + DateTime.Now + @"</MessageID><Email>kamyli</Email><Password>/420M6HRnM9N764scFZNMC8cbKdrYW15bGk=</Password><RequestedBy>kamyli</RequestedBy><TaxID>0000000000052</TaxID><Ref1>1111111111111</Ref1><Ref2>120250</Ref2><Ref3>EXCEL(Excellence)</Ref3><Ref4>1</Ref4><Ref5>Dev11g</Ref5><Ref6>09969782887</Ref6><Amount>30500</Amount><TopupType>S</TopupType><LocLatitude>16.8489457</LocLatitude><LocLongitude>96.1601554</LocLongitude><AppType>MS</AppType><AgentFee></AgentFee><ProductDesc></ProductDesc></ConfirmReq>";

        //string reqxml = @"<InquiryReq><Version>1.0</Version><TimeStamp>201801080930512180</TimeStamp><MessageID>4638a317-8a17-459a-95af-d4eb01f9322f</MessageID><Email>kamyli</Email><Password>/420M6HRnM9N764scFZNMC8cbKdrYW15bGk=</Password><RequestedBy>kamyli</RequestedBy><TaxID>0000000000031</TaxID><Ref1>1234567</Ref1><Ref2></Ref2><Ref3>40607</Ref3><Ref4></Ref4><Ref5></Ref5><Amount></Amount><TopupType>S</TopupType><IsQR></IsQR></InquiryReq> ";

        //string reqxml = @"<ConfirmReq> <Version>2.0</Version> <TimeStamp>201801101034336205</TimeStamp> <MessageID>" + DateTime.Now + @"</MessageID> <Email>use122</Email> <Password>WjDE1wQf/wIoebSs9PoQIXN+6Tp1c2UxMjI=</Password> <TaxID>0000000000052</TaxID> <Ref1>200003500000826</Ref1> <Ref2>2500348</Ref2> <Ref3>Renew Htoo Shall DTH 2018  (1 Months)</Ref3> <Ref4>1</Ref4> <Ref5>Dev</Ref5><Ref6>09969782887</Ref6> <Amount>7900.0</Amount><AgentFee>0.00</AgentFee><ProductDesc></ProductDesc> <LocLatitude>90</LocLatitude> <LocLongitude>80</LocLongitude> <AppType>CP</AppType><TopupType></TopupType> </ConfirmReq>";

        //string reqxml = @"<InquiryReq><Version>1.0</Version><TimeStamp>201801101117567960</TimeStamp><MessageID>" + DateTime.Now + @"</MessageID><Email>kamyli</Email><Password>/420M6HRnM9N764scFZNMC8cbKdrYW15bGk=</Password><RequestedBy>kamyli</RequestedBy><TaxID>0000000000052</TaxID><Ref1>200003500000826</Ref1><Ref2></Ref2><Ref3></Ref3><Ref4></Ref4><Ref5>RFP</Ref5><Amount></Amount><TopupType>S</TopupType><IsQR></IsQR></InquiryReq>";

        //string reqxml = @"<ConfirmReq><Version>3.0</Version><TimeStamp>202003281121584380</TimeStamp>  <MessageID>" + Guid.NewGuid() + "</MessageID><Email>09692922675</Email><Password>PFnco5X7hSb2lH9Qv1b9HFLzm10wOTY5MjkyMjY3NQ==</Password><TaxID>0000000000098</TaxID><Ref1>0/Pick a Baby Naming</Ref1><Ref2>33/ဆရာမေဒၚေခ်ာေဆြဇင္သင္/MGMG</Ref2><Ref3>Male/09774941523/15,16/4:00 AM</Ref3><Ref4>No-44/45/46/47, Zizawa street(M), Yuzana Condo : Yangon</Ref4><Ref5>Y/1368/TaKu/LaSan/1/Sat</Ref5><Ref6>NearMeTesting</Ref6><Amount>1000</Amount><TopupType>S</TopupType><LocLatitude>0.0</LocLatitude><LocLongitude>0.0</LocLongitude><LoginType>POS</LoginType><AppType>MS</AppType><AgentFee>0</AgentFee><ProductDesc /><TerminalId>D1V0980000452</TerminalId><DeviceUID>D1V0980000452357412506722847</DeviceUID><Token>6a7c7b41-3ce9-4039-be66-b175a9d829df</Token></ConfirmReq>";

        //        const string reqxml = @"<InquiryReq>
        //   <Version>3.0</Version>
        //   <TimeStamp>201904290948403140</TimeStamp>
        //   <MessageID>abad3321-e632-4913-94a3-6d57bf306f54</MessageID>
        //   <Email>pai111</Email>
        //   <Password>3mvuhLILmGeKs09z1IAcqnXDCRBwYWkxMTE=</Password>
        //   <RequestedBy>09421968771</RequestedBy>
        //   <LoginType>MS</LoginType>
        //   <TaxID>1230000000000</TaxID>
        //   <Ref1>0000006182402617</Ref1>
        //   <Ref2>0000006182402617</Ref2>
        //   <Ref3 />
        //   <Ref4 />
        //   <Ref5 />
        //   <Amount>5100</Amount>
        //   <TopupType>S</TopupType>
        //   <IsQR>Y</IsQR>
        //   <DeviceUID>357695696625247</DeviceUID>
        //   <Token>19fd61db-894e-4c6e-8904-173d8220ef3b</Token>
        //</InquiryReq>";
        //string reqxml = @"<FtthOrWtthInquiry><Version>1.0</Version><TimeStamp>201911251023475511</TimeStamp><Email>pai111</Email><Password>VYqHBJkBec3rFFhnEwwiYCwEk8dwYWkxMTE=</Password><RequestedBy>pai111</RequestedBy><MessageID>e7ae8acb-d984-4254-88a7-1a517a5ff794</MessageID><TaxID>0000000000070</TaxID><MobileNumber>09791001339</MobileNumber></FtthOrWtthInquiry> ";

        
        //Service bisService = new Service();
        //string response = bisService.MerchantAcceptanceReq("<MerchantAcceptanceReq><Version>3.0</Version><TimeStamp>202004061536335010</TimeStamp><MessageID>" + DateTime.Now + @"</MessageID><Email>09692922675</Email><Password>PFnco5X7hSb2lH9Qv1b9HFLzm10wOTY5MjkyMjY3NQ==</Password><Ref1>0881354744</Ref1><Ref2>MSP</Ref2><Ref3/><Ref4/><Ref5>09972622152</Ref5><Amount>100</Amount>      <LoginType>MS</LoginType><AppType>MS</AppType><ProductDesc>MSP</ProductDesc><TerminalId></TerminalId><DeviceUID>353798985780243</DeviceUID><Token>292be3de-8fe6-48d9-8331-6a058d576505</Token></MerchantAcceptanceReq> ");
        //string response = bisService.FtthOrWtthInquiry(reqxml);
        //string response = bisService.GetGiftCardDenoList("0000000000094");
        //string response = bisService.ConfirmToBiller(reqxml);
        //string response = bisService.FtthOrWtthInquiry(reqxml);
        //string a = response;
       // SinaptIQPKCS7.PKCS7 pkcs7 = new SinaptIQPKCS7.PKCS7();
       // string responseData = "MIIDdAYJKoZIhvcNAQcDoIIDZTCCA2ECAQAxggGJMIIBhQIBADBtMFcxCzAJBgNVBAYTAkJFMRkwFwYDVQQKExBHbG9iYWxTaWduIG52LXNhMS0wKwYDVQQDEyRHbG9iYWxTaWduIERvbWFpbiBWYWxpZGF0aW9uIENBIC0gRzICEhEhA15l/dxNlXsRGbuMSnZnYjANBgkqhkiG9w0BAQEFAASCAQAemnmxUpZuyzCluc7F45dBoqQE5mRuB9IOTbljFlJnZWcaEwG7PEtwSvZ1swgdS9ArBYkRMVe8RE+DqjIT+IeGyaQpQOo2qxVxJvUQFCmJgYpSEhZLKvPIxdnE5zRNe3V1JOCI3163daNlB6ZFPp72i7QafXbr8/iVYuC5o8QYqKqqvKod2ICKgb8b5rNnHaqEnj6J39y3Jg7kvHMSgU0FCVf6x+GkRXWZzZl/9jxGSdN5vJHga8n3QPAU/hAjA+FHyL+53TptZfwWXVoK1iPWoiAY223STvMqwos0g0gQr2A1aWNV7CN85CMAVCDMT3OVCsRKx+wvEr3bSA4UwBIrMIIBzQYJKoZIhvcNAQcBMBQGCCqGSIb3DQMHBAgyI6Z84bgU0oCCAagH6zTmvgmDWeW2CiJgEOGRbeY4kHaUGXmjDKqSx6U6ObLRI7yDgUrhY9yX2rl63xCeSlPp+jYo11HesBrkkmIev1UnyF2Uv9Lh9w609dFQZh4SobZ0D+3qUqP8kjJCdNz57Q/6T2rPeBcaVmKzmrUJ4wnYN2EqhRuINJicKZr0sYbMLop5kt7qx8/VZ/D3Xd/rRqdFkvoNwVmi8P3HuvtcNgKH6j4ykjN/uWv1ISBZ7k/Vw0pnBs08yXGom77nxXlTIAUrF0sd6PCWzLnHsiXYzXkJ6V35bREtkSfV1eHNF8ahcsSigmhn9QU/eKZcv+Ta6U8Ix4RSJWoN0hJ3KuxcCyeZTXbW31phYVdPPWHwL3hAawgx9CFbdFs235IZG5PgLiO1Ki+8HOA3UyRJ6MBrRBPKILWv5e5ZPP1iuWmcwTbPdzQN6tCR3wzMj0SdVch08QbirBbeMHz8GIWY9F9KgvvH/xEVgiWMPR/JkvfxAnxpWlY0wF84w3ppxY6SK2O7VxPOJ8/MB1zBf9ISOuqP0mvyJKvsDnbh6dndsaGKzFUPLk3UvKqP";
       //responseData=responseData.Replace("\r\n", string.Empty);
       // responseData = pkcs7.decryptMessage(responseData, pkcs7.getPrivateCert(ConfigurationManager.AppSettings["privateKeyPath"].ToString(), ConfigurationManager.AppSettings["privateKeyPWD"].ToString()));

        //string CreditLimit="200000";
        //string test = int.Parse(CreditLimit).ToString("#,##0.00");
        //string creditlimitFormat = int.Parse(CreditLimit).ToString("#,##0");
        //string branchName = "Test Branch";
        //string subject = ConfigurationManager.AppSettings["subjectResetBalance"].ToString() + " at " + System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        //subject = string.Format(subject, branchName);
  
        //DateTime DueDate;
        //DueDate = (DateTime.Parse(DateTime.Now.ToString()).AddDays(-1.5));
        //DueDate = new DateTime(DueDate.Date.Year, DueDate.Date.Month, DueDate.Date.Day, 15, 00, 00);
      //string result=  Utils.maskString("0000000000029");
      //  MessagingService.MessagingServiceClient smswcf = new MessagingService.MessagingServiceClient();
      //  smswcf.sendSMS("09789323948", "thetZaw", "EN");
      //  int value1 = 1000;
      //  int value2 = 2000;
       // int result = 0;
       // result = value1 - value2;
       // string errMsg=string.Empty;
       // A2AAPIWCF.ServiceClient service = new A2AAPIWCF.ServiceClient();
       // bool response=service.IsDuplicatedCNPYESCPayment(out  errMsg, "62212282258", DateTime.Now, DateTime.Now.AddDays(-20));

       // string newbillers = ConfigurationManager.AppSettings["NewBiller"].ToString();
       // string newappver = ConfigurationManager.AppSettings["newappver"].ToString();
       // string whatis = "what is this";
       // whatis.Replace(" ","-");
       //string appvar= newappver.Replace(".","");
        


       // string appvar2 = appvar;
       // string appvar3 = newappver;
       // string cerpathweb = "http://www.google.com";
       // string shopweb = string.Empty;
       // shopweb= Utils.getQUrl(cerpathweb);

      //  string s="thetzawwin&^^*^&";
       // bool result =IsASCII(s);
        //<ConfirmReq><Version>1.0</Version><TimeStamp>201506021506550821</TimeStamp><MessageID>619ac95f-299c-4c3b-806f-87ccab00f001</MessageID><Email>luluyb1u1</Email><Password>gaW1en4HEIl+/4Ve4c1CBEP7GGtsdWx1eWIxdTE=</Password><TaxID>5555555555555</TaxID><Ref1></Ref1><Ref2>1000</Ref2><Ref3>095119837</Ref3><Ref4>0104033434</Ref4>0104<Ref5></Ref5><Amount>1000</Amount><TopupType>S</TopupType><LocLatitude>16.8020251</LocLatitude><LocLongitude>96.1616852</LocLongitude><AppType>MS</AppType><AgentFee></AgentFee><ProductDesc></ProductDesc></ConfirmReq>
       // string xml = "<InquiryReq><Version>1.0</Version><TimeStamp>201510131448060740</TimeStamp><MessageID>a7ca5f5-5e60-42fd-bd07-7b2002a1cdfc</MessageID><Email>uluyb1u1</Email><Password>FHQBa6EY+X6lfxZpt+2PifBhc3ZsdWx1eWIxdTE=</Password><RequestedBy>luluyb1u1</RequestedBy><TaxID>0000000000031</TaxID><Ref1>01450181254</Ref1><Ref2></Ref2><Ref3>1</Ref3><Ref4></Ref4><Ref5></Ref5><Amount></Amount><TopupType>S</TopupType><IsQR></IsQR></InquiryReq>";
      //  string xml = @"<InquiryReq><Version>1.0</Version><TimeStamp>201703010952177420</TimeStamp><MessageID>8a891dec-66eb-4430-b4d9-56cfbe84dab6</MessageID><Email>luluyb1u1</Email><Password>FHQBa6EY+X6lfxZpt+2PifBhc3ZsdWx1eWIxdTE=</Password><RequestedBy>luluyb1u1</RequestedBy><TaxID>0000000000023</TaxID><Ref1>000063-00008-00001</Ref1><Ref2>‎‎0000000044</Ref2><Ref3></Ref3><Ref4></Ref4><Ref5></Ref5><Amount></Amount><TopupType>S</TopupType><IsQR></IsQR></InquiryReq>";
      //  string confirm = @"<ConfirmReq><Version>1.0</Version><TimeStamp>201703031055170990</TimeStamp><MessageID>2984a-719b-421a-a779-8600ce1294</MessageID><Email>luluyb1u1</Email><Password>FHQBa6EY+X6lfxZpt+2PifBhc3ZsdWx1eWIxdTE=</Password><TaxID>0000000000023</TaxID><Ref1>000063-00008-00001</Ref1><Ref2>0000000044</Ref2><Ref3>09789323948</Ref3><Ref4></Ref4><Ref5>300000.67</Ref5><Ref6></Ref6><Amount>10000</Amount><TopupType></TopupType><LocLatitude>16.8022379</LocLatitude><LocLongitude>96.1614398</LocLongitude><AppType>MS</AppType><AgentFee>300.00</AgentFee><ProductDesc>0.00:0.00:L-S-PA-DZ-PS-TO-100:Loan Summer crop Paddy Dry Zone Paw San Topaz:15052017</ProductDesc></ConfirmReq>";
      //  // string xml1 = @"<ConfirmReq><Version>1.0</Version><TimeStamp>201605231107140918</TimeStamp><MessageID>0b94f-84-0a-e277737</MessageID><Email>luluyb1u1</Email><Password>FHQBa6EY+X6lfxZpt+2PifBhc3ZsdWx1eWIxdTE=</Password><TaxID>0000000000022</TaxID><Ref1>0104033434</Ref1><Ref2>0104</Ref2><Ref3>09789323948</Ref3><Ref4></Ref4><Ref5></Ref5><Ref6></Ref6><Amount>1000</Amount><TopupType>S</TopupType><LocLatitude>16.802385</LocLatitude><LocLongitude>96.161566</LocLongitude><AppType>MS</AppType><AgentFee></AgentFee><ProductDesc>null</ProductDesc></ConfirmReq>";
      // // string xml = @"<InquiryReq><Version>1.0</Version><TimeStamp>201512170203490469</TimeStamp><Email>desk11</Email><Password>WpB0YyjHdkM0f/hae/Rqkbg6b1xkZXNrMTE=</Password><TaxID>0000000000022</TaxID><InquiryType>M</InquiryType><Ref1>1541597</Ref1><Ref2>0104</Ref2><Ref3></Ref3><Amount>84100</Amount><MessageID>f69e089c-6fa-4c83-a529-37a8774b53d6</MessageID><IsQR></IsQR><RequestedBy>desk11</RequestedBy></InquiryReq>";
      //////  Service s = new Service();
      //Service s = new Service();

      //string res = s.ConfirmToBiller(confirm);// s.inquiry2Biller(xml);//
      // string ress = res;
    //    fraudWs.Service1Client fraudWS = new fraudWs.Service1Client();
    //    fraudWS.CheckSender(123456, 19322, "B0001", "makha1", 500000, "1STOP+");
    //  s.ConfirmToBiller(xml1);

                     //start airtime topup
        //string amount = "1000";
        //string recipient = string.Empty;
        //for (int i = 1; i <= 4; i++)
        //{
        //    try
        //    {
        //        recipient = (i == 1 ? "09794937567" : i == 2 ? "09796277873" : i == 3 ? "09789323948" : "09795987135");
        //        string url = string.Empty; //"http://10.162.32.214/airtime.php?amount=" + amount + "&recipient=" + recipient;
        //        ServicePointManager.CertificatePolicy = new CertPolicy();
        //        writeLog("Request(" + i + ")" + "Start Time :" + DateTime.Now.ToString());
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Timeout = 2097151;
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        writeLog("Request(" + i + ")" + "Response Time :" + DateTime.Now.ToString());
        //        Stream resStream = response.GetResponseStream();
        //        // Open the stream using a StreamReader for easy access.
        //        StreamReader reader = new StreamReader(resStream);
        //        // Read the content fully up to the end.
        //        string responseFromServer = reader.ReadToEnd();
        //        writeLog(responseFromServer);
        //        // Clean up the streams.
        //        reader.Close();
        //        resStream.Close();
        //        response.Close();
        //    }
        //    catch(Exception ex)
        //    {
        //        writeLog("Exception Error" + ex.Message);
        //    }
        //}
        //string msg = string.Empty;
        //sendmail("MPT", "1000", out msg);
    
    }
     bool IsASCII(string value)
    {
        // ASCII encoding replaces non-ascii with question marks, so we use UTF8 to see if multi-byte sequences are there
        if (Encoding.UTF8.GetByteCount(value) == value.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool sendmail(string card, string price, out string msg)
    {
       // writeLog("Ok in entering sendmail method");
        StringBuilder sb = new StringBuilder();
        sb.Append(ConfigurationManager.AppSettings["greeting"].ToString() + "<br/>");
        sb.Append(ConfigurationManager.AppSettings["msgbody"].ToString() + "<br/><br/>");
        sb.Append("<table>");
        sb.Append("<tr><td>Card </td><td>:</td><td><strong>" + card + "</strong> .</td><td>Price </td><td>:</td><td><strong>" + price + "</strong> .</td></tr>");



        sb.Append("</table></br>");


        sb.Append("<strong>Thank You & Regards</Strong>, <br/>");
        sb.Append("Auto Alert Mail");

        //  EmailApiModel mailapi = new EmailApiModel();
        string fromEmailAddress = ConfigurationManager.AppSettings["fromEmail"].ToString();
        string toEmail = "thetzaw@2c2.com";//ConfigurationManager.AppSettings["toEmail"].ToString();
        string subject = ConfigurationManager.AppSettings["subject"].ToString() + " at " + System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        string msgBody = sb.ToString();

       // emailAPIWS.EmailAPISoapClient emailApi = new emailAPIWS.EmailAPISoapClient();
        AwsEmailAPI.ServiceSoapClient emailApi = new AwsEmailAPI.ServiceSoapClient();
        string errMsg = string.Empty;
        bool sent = false;
        //sent = emailApi.sendEmail(toEmail, "", "", fromEmailAddress, fromEmailAddress, subject, msgBody
        // , "", "", "", false, "", "", true, out errMsg);
        emailApi.sendAWSMail(fromEmailAddress, fromEmailAddress, toEmail, "", "", subject, msgBody);
        if (sent)
        {

            msg = errMsg;
           // writeLog("Return ture email was sent" + msg);
            return true;

        }
        else
        {
            msg = errMsg;
            return false;

        }
    }
    public class CertPolicy : ICertificatePolicy
    {
        public CertPolicy()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        // Methods
        public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest request, int problem)
        {

            return true;
        }
    }
}