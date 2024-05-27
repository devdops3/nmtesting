using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using log4net;
using System.Reflection;
using System.Security.Cryptography;
using System.Data;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for Utils
/// </summary>
public class Utils
{
	public Utils()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }
    public static Hashtable getHTableFromXML(string requestXML)
    {
        Hashtable ht = new Hashtable();
        XmlDocument xdoc = new XmlDocument();
        xdoc.LoadXml(requestXML);
        XmlNodeList xnl = xdoc.ChildNodes;
        if (xnl.Count > 0)
        {

            XmlNode rootNode;
            if (xnl.Count > 1)
                rootNode = xnl.Item(1);
            else

                rootNode = xnl.Item(0);

            XmlNodeList subNodes = rootNode.ChildNodes;
            if (subNodes.Count >= 1)
            {
                foreach (XmlNode xn in subNodes)
                {
                    ht.Add(xn.Name, xn.InnerText);
                }
            }
            else
            {
                ht = null;
            }
        }
        else
        {
            ht = null;
        }

        return ht;
    }
    public static string getTimeStamp()
    {
        //string format = "ddMMyyyyhhmmss";//"ddMMyyyyhhmmsstt";


        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
    }

    public static DateTime getStrToDate(string dtStr, string format)
    {
        return DateTime.ParseExact(dtStr, format, CultureInfo.InvariantCulture);
    }
    public static double getAmountDBL(string amt)
    {
        if (amt.Length > 2)
        {
            amt = amt.Substring(0, amt.Length - 2) + "." + amt.Substring(amt.Length - 2, 2);
            return double.Parse(amt);
        }
        return double.Parse(amt);
        //return amt;
    }
    public static string getFrom12DigitToOrginal(string amt)
    {
        //if (amt.Length > 2)
        //{
        //    amt = amt.Substring(0, amt.Length - 2);// +"." + amt.Substring(amt.Length - 2, 2);
        //    return double.Parse(amt).ToString("##0.00");
        //}
        //return "0";

        if (amt.Length > 2)
        {
            amt = amt.Substring(0, amt.Length - 2) + "." + amt.Substring(amt.Length - 2, 2);
            return double.Parse(amt).ToString("##0.00");
        }
        return double.Parse(amt).ToString("##0.00");

        //return amt;
    }
    public static double getFee(double amount, float feePercent, double feeFlat)
    {
        double agentFee = 0;
        //agentFee = 0; 
        if (feePercent > 0)
        {
            agentFee = (feePercent / 100) * amount;
        }


        if (feeFlat > 0)
        {
            agentFee += feeFlat;
        }



        // merAmount = amount - (feeFlat + agentFee);


        //feeFlat = roundAmount(feeFlat);
        agentFee = roundAmount(agentFee);

        return agentFee;

    }

    public static double getFeeNotRound(double amount, float feePercent,
                         double feeFlat
                         )
    {
        double agentFee = 0;
        //agentFee = 0; 
        if (feePercent > 0)
        {
            agentFee = (feePercent / 100) * amount;
        }


        if (feeFlat > 0)
        {
            agentFee += feeFlat;
        }



        // merAmount = amount - (feeFlat + agentFee);


        //feeFlat = roundAmount(feeFlat);
        agentFee = roundAmountPoint2(agentFee);

        return agentFee;

    }
    public static double roundAmountPoint2(double amt)
    {
        //string amtStr = amt.ToString("##0.00");
        //try
        //{
        //    return double.Parse(amtStr);
        //}
        //catch
        //{
        //    return 0.0;
        //}

        amt = Math.Round(amt, 2);
        return amt;
    }
    public static double roundAmount(double amt)
    {
        //string amtStr = amt.ToString("##0.00");
        //try
        //{
        //    return double.Parse(amtStr);
        //}
        //catch
        //{
        //    return 0.0;
        //}

        amt = Math.Round(amt, 0);
        return amt;
    }
    public static string getFromOrginalTo12Digit(string amt)
    {
        //if (amt.Length > 2)
        //{
        //    amt = amt.Substring(0, amt.Length - 2);// +"." + amt.Substring(amt.Length - 2, 2);
        //    return double.Parse(amt).ToString();
        //}
        string[] strArr = amt.Split('.');
        if (strArr.Length ==2)
        {
            amt = strArr[0] + strArr[1];
        }
        else
        {
            amt = amt + "00";
        }
        return amt.PadLeft(12, '0');
        //return amt;
    }
    public static string getAmountFromDBL(double amt)
    {
        string[] strAmt = amt.ToString().Split('.');
        string resultAmt = "";
        if (strAmt.Length > 1)
        {
            resultAmt = strAmt[0] + strAmt[1].PadRight(2, '0');
        }
        else
        {
            resultAmt = strAmt[0] + "00";
        }
        return resultAmt.PadLeft(12, '0');
    }

    public static string changeStatusToName(string statusChk)
    {
        string dataOutput = null;
        switch (statusChk)
        {
            case "PE": dataOutput = "Pending"; break;
            case "EX": dataOutput = "Expired"; break;
            case "PA": dataOutput = "Paid"; break;
            case "PM": dataOutput = "Paid(More Mismatched)"; break;
            case "PL": dataOutput = "Paid(Less Mismatched)"; break;
            case "CA": dataOutput = "Canceled"; break;
            case "RE": dataOutput = "Rejected"; break;
            case "MM": dataOutput = "MismatchedAmount"; break;
            case "RF": dataOutput = "RefNotFound"; break;
            case "BC": dataOutput = "Browser Closed"; break;
            case "FA": dataOutput = "Failed"; break;
            case "VO": dataOutput = "Voided"; break;
            case "IN": dataOutput = "Browser Closed"; break;
            case "NA": dataOutput = "No-Action"; break;
        }
        return dataOutput;
    }

    public Image byteArrayToImage(byte[] fileBytes)
    {
        using (MemoryStream fileStream = new MemoryStream(fileBytes))
        {
            return Image.FromStream(fileStream);
        }
    }

    public static void ByteArrayToImageFilebyMemoryStream(byte[] imageBytes)
    {
       // MemoryStream ms = new MemoryStream(imageByte);

       // ms = new MemoryStream(imageByte, 0, imageByte.Length);
       // ms.Seek(0, SeekOrigin.Begin);
       // System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

       //// Image image = Image.FromStream(ms);
       // image.Save(@"C:\Users\Administrator\imageTest.png");
        //byte[] buffer = GetImageBytes();

        //byte[] imageBytes = Convert.FromBase64String(base64String);
        //MemoryStream ms = new MemoryStream(imageBytes, 0,
        //  imageBytes.Length);

        //// Convert byte[] to Image
        //ms.Write(imageBytes, 0, imageBytes.Length);
        //Image image = Image.FromStream(ms, true);


        Bitmap bmp;
        using (var ms = new MemoryStream(imageBytes))
        {
            bmp = new Bitmap(ms);
        }
       // return image;

        //var bitmap = new Bitmap(150, 150, PixelFormat.Format32bppArgb);
        //var bitmap_data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        //Marshal.Copy(imageByte, 0, bitmap_data.Scan0, imageByte.Length);
        //bitmap.UnlockBits(bitmap_data);
        //var result = bitmap as Image;
    }
    public static bool  Base64Decode(string base64EncodedData,string fileName)
    {
         bool result = false;
        try
        {
           
            byte[] test = System.Convert.FromBase64String(base64EncodedData);

            byte[] last = Decompress(test);
            MemoryStream ms = new MemoryStream(last);
            Image image = Image.FromStream(ms);
            if (!File.Exists(fileName))
            {
                //File.Delete(fileName);
                writeLog("File Started to create : " + fileName);
                image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                result = true;
                writeLog("File Created OK");
            }
            else
            {
                writeLog("File Existed : " + fileName);
                result = false;

            }
        }
        catch (Exception ex)
        {
            writeLog("Error In Decoding an image err:"+ex.Message);
            result = false;
        }
        // pictureBox1.Image = Image.FromStream(ms);
        return result;

    }

    static byte[] Decompress(byte[] gzip)
    {
        using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
        {
            const int size = 4096;
            byte[] buffer = new byte[size];
            using (MemoryStream memory = new MemoryStream())
            {
                int count = 0;
                do
                {
                    count = stream.Read(buffer, 0, size);
                    if (count > 0)
                    {
                        memory.Write(buffer, 0, count);
                    }
                }
                while (count > 0);
                return memory.ToArray();
            }
        }
    }
    public static void byteArrayToImageFilebyMemoryStream(byte[] imageByte, string fileName)
    {
        // string folerName = ConfigurationManager.AppSettings["BillerLogoFolder"].ToString();
        MemoryStream ms = new MemoryStream(imageByte);
        Image image = Image.FromStream(ms);
        if (!File.Exists(fileName))
        {
            //File.Delete(fileName);
            writeLog("File Started to create : " + fileName);
            image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            writeLog("File Created OK");
        }
        else
        {
            writeLog("File Existed : " + fileName);
        }

    }
    public static string hashOneCashInqReq(string messageid, string agentCode, string Ref1, string requestedby)
    {
        ArrayList ar = new ArrayList();
        ar.Add(messageid);
        ar.Add(agentCode);
        ar.Add(Ref1);
        ar.Add(requestedby);
        ar.Sort();

        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }
        string hashstr = generateHashValue(sb.ToString(), ConfigurationManager.AppSettings["OneCashSecretKey"].ToString());
        return hashstr;
    }

    public  static string hashOneCashInqRes(string messageid, string ResCode, string ResDesc, string Ref1, string Ref2, string Amount, string ProductDesc)
    {
        ArrayList ar = new ArrayList();
        ar.Add(messageid);
        ar.Add(ResCode);
        ar.Add(ResDesc);
        ar.Add(Ref1);
        ar.Add(Ref2);
        ar.Add(Amount);
        ar.Add(ProductDesc);

        ar.Sort();

        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }
        string hashstr = generateHashValue(sb.ToString(), ConfigurationManager.AppSettings["OneCashSecretKey"].ToString());
        return hashstr;
    }

    public static string hashstrInquiry(string version, string timestamp, string agentcode, string inquiry, string ref1, string ref2, string amount)
    {
        ArrayList ar = new ArrayList();
        ar.Add(version);
        ar.Add(timestamp);
        ar.Add(inquiry);
        ar.Add(agentcode);
        // ar.Add(password);
        ar.Add(ref1);
        ar.Add(ref2);
        ar.Add(amount);
        ar.Sort();

        //ArrayList ar = new ArrayList();
        //ar.Add(version);
        //ar.Add(timeStamp);
        //ar.Add(inquirytype);
        //ar.Add(agentcode);
        //ar.Add(password);
        //ar.Add(ref1);
        //ar.Add(ref2);
        //ar.Add(amount);

        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }
       // writeLog("hashstrInquiry Signature : " + sb.ToString());
        string hashstr = generateHashValue(sb.ToString(), ConfigurationManager.AppSettings["123SecretKey"].ToString());
        return hashstr;
    }
    public static string changeStatusCodeToCode(string code)
    {
        string resCode = null;
        switch (code)
        {
            //  case "PE": code = "01"; break;
            case "BP_ST_00": resCode = "00"; break;
            case "BP_ST_01": resCode = "01"; break;
            case "BP_ST_02": resCode = "02"; break;
            case "BP_ST_03": resCode = "03"; break;
            case "BP_ST_04": resCode = "04"; break;
            case "BP_ST_05": resCode = "09"; break;
            case "BP_ST_08": resCode = "08"; break;
            case "BP_ST_98": resCode = "98"; break;
            case "BP_ST_99": resCode = "99"; break;
            case "BP_ST_96": resCode = "96"; break;
            default:resCode=code; break;
            //           00	Success
            //01	Ref Not Found
            //02	Paid Already
            //03	Expired Already
            //04	Amount Mismatched
            //05	Authentication Failed
            //06	Invalid Request
            //07	Invalid Message
            //08	DB Error
            //98	General Error
            //99	System Error

            // case "": code = "Browser Closed"; break;

        }
        return resCode;
    }
    public static string hashstrOneCashConfirmReq(string messageid, string agentCode, string Ref1, string Ref2, string amount, string confirmedby, string locLattitude, string locLongitude)
    {
        ArrayList ar = new ArrayList();
        ar.Add(messageid);
        ar.Add(agentCode);
        ar.Add(Ref1);
        ar.Add(Ref2);
        ar.Add(amount);
        ar.Add(confirmedby);
        ar.Add(locLattitude);
        ar.Add(locLongitude);
        ar.Sort();



        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }

       // writeLog("hashstrOneCashConfirmReq Signature : " + sb.ToString());

        string hashstr = generateHashValue(sb.ToString(), ConfigurationManager.AppSettings["OneCashSecretKey"].ToString());
        return hashstr;
    }

    public static string hashstrOneCashConfirmResCC(string messageid, string resCode, string resDesc, string ref1)
    {
        ArrayList ar = new ArrayList();
        ar.Add(messageid);
        ar.Add(resCode);
        ar.Add(resDesc);
        ar.Add(ref1);
        ar.Sort();

        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }


       // writeLog("hashstrOneCashConfirmResCC Signature : " + sb.ToString());
        string hashstr = generateHashValue(sb.ToString(), ConfigurationManager.AppSettings["OneCashSecretKey"].ToString());
        return hashstr;
    }

    
        public static string hashstrReq123(string version, string timestamp,string messageid,string agentcode,string ref1)
    {
        ArrayList ar = new ArrayList();
        ar.Add(version);
        ar.Add(timestamp);
        ar.Add(messageid);
        ar.Add(agentcode);
        ar.Add(ref1);      
        ar.Sort();



        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }
      //  writeLog("hashstrConfirm Signature : " + sb.ToString());


        string hashstr = generateHashValue(sb.ToString(), ConfigurationManager.AppSettings["123SecretKey"].ToString());
        return hashstr;
    }
        public static bool verifyHashValue(string hashValue, string signatureString, string secretKey)
        {
            bool result = false;
            string hashValueTmp = getHMAC(signatureString, secretKey);
            if (hashValue.ToUpper() == hashValueTmp.ToUpper())
            {
                result = true;
            }
            else
            {
                writeLog("Signature : " + signatureString);
                writeLog("HashValue mismatched! OneStop's HashValue | 1-2-3's HashValue : " + hashValue + " | " + hashValueTmp);
                result = false;
            }

            return result;
        }

   // hashstrConfirmReq123
        //(version, ts, messageid, agentcode, ref1,ref2,amountTo123,confirmedby,locLatitude,locLongitude);
        public static string hashstrConfirmReq123(string version, string timestamp,string messageid,string agentcode, string ref1, string ref2, string amount, string confirmedby, string lacti, string longi)
        {
           


            ArrayList ar = new ArrayList();
            ar.Add(version);
            ar.Add(timestamp);
            ar.Add(messageid);
            ar.Add(agentcode);
           
            ar.Add(ref1);
            ar.Add(ref2);
            ar.Add(amount);
            ar.Add(confirmedby);
            ar.Add(lacti);
            ar.Add(longi);
            ar.Sort();



            StringBuilder sb = new StringBuilder();
            foreach (string item in ar)
            {
                sb.Append(item);
            }
            //  writeLog("hashstrConfirm Signature : " + sb.ToString());


            string hashstr = generateHashValue(sb.ToString(), ConfigurationManager.AppSettings["123SecretKey"].ToString());
            return hashstr;
        }
  
    public static string hashstrConfirm123(string version, string timestamp, string agentcode, string password,string ref1, string ref2, string amount, string longi, string lacti)
    {
        ArrayList ar = new ArrayList();
        ar.Add(version);
        ar.Add(timestamp);
        ar.Add(agentcode);
        ar.Add(password);
        ar.Add(ref1);
        ar.Add(ref2);
        ar.Add(amount);
        ar.Add(longi);
        ar.Add(lacti);
        ar.Sort();



        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }
      //  writeLog("hashstrConfirm Signature : " + sb.ToString());


        string hashstr = generateHashValue(sb.ToString(), ConfigurationManager.AppSettings["123SecretKey"].ToString());
        return hashstr;
    }
    public static string hashstrConfirm1Cash(string MessageID, string AgentCode, string Amount, string ConfirmedBy, string CashInIPAddress, string CashInNote, string LocLatitude, string LocLongitude)
    {
        ArrayList ar = new ArrayList();
        ar.Add(MessageID);
        ar.Add(AgentCode);
        ar.Add(Amount);
        //ar.Add(password);
        ar.Add(ConfirmedBy);
        ar.Add(CashInIPAddress);
        ar.Add(CashInNote);
        ar.Add(LocLatitude);
        ar.Add(LocLongitude);
        ar.Sort();


       

        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }
        //writeLog("hashstrConfirm Signature : " + sb.ToString());


        string hashstr = generateHashValue(sb.ToString(), ConfigurationManager.AppSettings["OneCashSecretKey"].ToString());
        return hashstr;
    }
    #region TzPutet
    public static string hashstrConfirmPutetReq(string version, string ts, string messageid,string pinType, string amount)
    {
        //ArrayList ar = new ArrayList();
        //ar.Add(version);
        //ar.Add(ts);
        //ar.Add(messageid);
        //ar.Add(pinType);
        //ar.Add(amount);
       
       


        //StringBuilder sb = new StringBuilder();
        //foreach (string item in ar)
        //{
        //    sb.Append(item);
        //}


        //string key = ConfigurationManager.AppSettings["PutetSecretKey"].ToString();

        //string hashstr = generateHashValue(sb.ToString(), key);
        //return hashstr;

        ArrayList ar = new ArrayList();
        ar.Add(version);
        ar.Add(ts);
        ar.Add(messageid);
        ar.Add(pinType);
        ar.Add(amount);
      


        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }


        string key = ConfigurationManager.AppSettings["PutetSecretKey"].ToString();

        string hashstr = generateHashValue(sb.ToString(), key);
        return hashstr;
    }


    public static string hashstrConfirmPutetRes(string version, string ts, string messageid,string rescode, string resdec, string ref2, string amount, string pin, string expiry, string Serialno)
    {
        ArrayList ar = new ArrayList();
        ar.Add(version);
        ar.Add(ts);
        ar.Add(messageid);
        ar.Add(rescode);
        ar.Add(resdec);
        ar.Add(ref2);
        ar.Add(amount);
        ar.Add(pin);
        ar.Add(expiry);
        ar.Add(Serialno);
       



        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }


        string key = ConfigurationManager.AppSettings["PutetSecretKey"].ToString();

        string hashstr = generateHashValue(sb.ToString(), key);
        return hashstr;
    }
    #region AWba Repayment
    public static string GethashKeyawbaInqReq(string version, string ts, string messageid, string customerid, string slipno)
    {
        ArrayList ar = new ArrayList();
        ar.Add(version);
        ar.Add(ts);
        ar.Add(messageid);
        ar.Add(customerid);
        ar.Add(slipno);


        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }


        string key = ConfigurationManager.AppSettings["awbasecret_key"].ToString();

        string hashstr = generateHashValue(sb.ToString(), key);
        return hashstr.ToLower();
    }
    public static string GethashKeyawbaInqRes(string version, string ts, string messageid, string rescode, string resdec, string ref2, string amt, string ref3, string expiry, string ref1)
    {
        ArrayList ar = new ArrayList();
        ar.Add(version);
        ar.Add(ts);
        ar.Add(messageid);
        ar.Add(rescode);
        ar.Add(resdec);
        ar.Add(ref2);
        ar.Add(amt);
        ar.Add(ref3);
        ar.Add(expiry);
        ar.Add(ref1);


        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }


        string key = ConfigurationManager.AppSettings["awbasecret_key"].ToString();

        string hashstr = generateHashValue(sb.ToString(), key);
        return hashstr.ToLower();
    }
    public static string GethashKeyawbaConfirmReq(string version, string ts, string messageid, string CustomerID, string SlipNo, string Amount)
    {
        ArrayList ar = new ArrayList();
        ar.Add(version);
        ar.Add(ts);
        ar.Add(messageid);
        ar.Add(CustomerID);
        ar.Add(SlipNo);
        ar.Add(Amount);


        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }


        string key = ConfigurationManager.AppSettings["awbasecret_key"].ToString();

        string hashstr = generateHashValue(sb.ToString(), key);
        return hashstr.ToLower();
    }

    public static string GethashKeyconfirmres(string version, string ts, string messageid, string ref2, string ref1, string rescode)
    {
        //GethashKeyres(version, ts, msgid, ref2, ref1, rescode);
        ArrayList ar = new ArrayList();
        ar.Add(version);
        ar.Add(ts);
        ar.Add(messageid);

        ar.Add(ref2);

        ar.Add(ref1);
        ar.Add(rescode);



        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }


        string key = ConfigurationManager.AppSettings["awbasecret_key"].ToString();

        string hashstr = generateHashValue(sb.ToString(), key);
        return hashstr.ToLower();
    }
    public static string GetAwbaResDesc(string rescode)
    {
        string desc = string.Empty;
        switch(rescode){
            case"00":
                desc="Approved";
                break;
                case"01":
                desc="Paid Already";
                break;
                case"02":
                desc="Expired(Due)";
                break;
                case"03":
                desc="Amount Mismatch";
                break;
                case"04":
                desc="Authentication Fail";
                break;
                case"05":
                desc="Invalid Request";
                break;
            case"06":
                desc="Invalid Customer ID";
                break;
                case"07":
                desc="Invalid Slip No";
                break;
                case"08":
                desc="MFI DB Error";
                break;
            case"98":
        desc="MFI General Error";
        break;
            case"99":
                desc="MFI System Error";
                break;
        }

      
        return desc;
    }
    #endregion
    public static string generateHashValue(string signatureString, string secretKey)
    {
        return getHMAC(signatureString, secretKey);
    }
    private static string getHMAC(string signatureString, string secretKey)
    {

        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

        byte[] keyByte = encoding.GetBytes(secretKey);

        HMACSHA1 hmac = new HMACSHA1(keyByte);


        byte[] messageBytes = encoding.GetBytes(signatureString);

        byte[] hashmessage = hmac.ComputeHash(messageBytes);

        return ByteArrayToHexString(hashmessage);

    }

    public static string ByteArrayToHexString(byte[] Bytes)
    {
        StringBuilder Result = new StringBuilder();
       
       string HexAlphabet = "0123456789ABCDEF";

        foreach (byte B in Bytes)
        {
            Result.Append(HexAlphabet[(int)(B >> 4)]);
            Result.Append(HexAlphabet[(int)(B & 0xF)]);
        }

        return Result.ToString();
    }
    #endregion

    #region EasyPoint Redeem

//    Version + TimeStamp + MessageID + PartnerID + DigitalCode 
//Secret Key(Test) : 746D7SCHAIQ0QUZ0MRJWU0PQ3AD7PJ8B

     public static string hashstrRedeemEnquiryEasyPointReq(string version,string timestamp,string messageid, string agentid,string digitalcode,string key)
    {
       
        ArrayList ar = new ArrayList();
        ar.Add(version);
        ar.Add(timestamp);
        ar.Add(messageid);
        ar.Add(agentid);
        ar.Add(digitalcode);
      



        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }


        writeLog("Signature sting " + sb.ToString() + "Key" + key);

        string hashstr = generateHashValue(sb.ToString(), key);
        return hashstr;
    }

     public static string hashstrRedeemEnquiryEasyPointRes(string version, string timestamp, string messageid, string agentid,string partneragentid, string digitalcode,string ResCode,string tranamt,string tranref,string key)
     {
        // Version + TimeStamp + MessageID + PartnerID + DigitalCode + ResCode + TransactionAmount +  TranRef 

         ArrayList ar = new ArrayList();
         ar.Add(version);
         ar.Add(timestamp);
         ar.Add(messageid);
         ar.Add(agentid);
         ar.Add(partneragentid);
         ar.Add(digitalcode);
         ar.Add(ResCode);
         ar.Add(tranamt);
         ar.Add(tranref);

         StringBuilder sb = new StringBuilder();
         foreach (string item in ar)
         {
             sb.Append(item);
         }

         writeLog("Signature string:" + sb.ToString() + "Key:" + key);

        

         string hashstr = generateHashValue(sb.ToString(), key);
         return hashstr;
     }

     public static string hashstrRedeemConfirmEasyPointReq(string version, string timestamp, string messageid, string agentid, string digitalcode,string agnref,string amtesp,string key)
     {
         // Version + TimeStamp + MessageID + PartnerID + DigitalCode + TranRef
         ArrayList ar = new ArrayList();
         ar.Add(version);
         ar.Add(timestamp);
         ar.Add(messageid);
         ar.Add(agentid);
         ar.Add(digitalcode);
         ar.Add(agnref);
         ar.Add(amtesp);




         StringBuilder sb = new StringBuilder();
         foreach (string item in ar)
         {
             sb.Append(item);
         }


         writeLog("Signature string:" + sb.ToString() + "Key:" + key);

         string hashstr = generateHashValue(sb.ToString(), key);
         return hashstr;
     }

     public static string hashstrRedeemConfirmEasyPointRes(string version, string timestamp, string messageid, string agentid,string partneragnid, string digitalcode,string rescode, string agnref,string amttoesp,string key)
     {
       // Version + TimeStamp + MessageID + PartnerID + DigitalCode + ResCode + TranRef
         ArrayList ar = new ArrayList();
         ar.Add(version);
         ar.Add(timestamp);
         ar.Add(messageid);
         ar.Add(agentid);
         ar.Add(partneragnid);
         ar.Add(digitalcode);
         ar.Add(rescode);
         ar.Add(agnref);
         ar.Add(amttoesp);




         StringBuilder sb = new StringBuilder();
         foreach (string item in ar)
         {
             sb.Append(item);
         }


        

         string hashstr = generateHashValue(sb.ToString(), key);
         return hashstr;
     }

    #endregion

    #region EasyPoint DigitalCode
    public static string hashstrConfirmEasyPointReq(string messageid, string agentid, string amounttoeasypoint,string mail, string latitude ,string longitude,string key)
    {
       
        ArrayList ar = new ArrayList();
        ar.Add(messageid);
        ar.Add(agentid);
        ar.Add(amounttoeasypoint);
        ar.Add(mail);
        ar.Add(latitude);
        ar.Add(longitude);



        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }


        

        string hashstr = generateHashValue(sb.ToString(), key);
        return hashstr;
    }
   // hashstrConfirmeasypointRes(messageid, agentID.ToString(), "00", "Success", time, time, expirys);
   // msgid, agnid, rescode, resdesc, ref3, ref1, expiry,amt
    public static string hashstrConfirmeasypointRes(string messageid, string agentid, string rescode, string resdec, string digital, string serial, string expiry,string amount,string key)
    {
        ArrayList ar = new ArrayList();
        ar.Add(messageid);
        ar.Add(agentid);
        ar.Add(rescode);
        ar.Add(resdec);
        ar.Add(digital);
        ar.Add(serial);
        ar.Add(expiry);
        ar.Add(amount);
       




        StringBuilder sb = new StringBuilder();
        foreach (string item in ar)
        {
            sb.Append(item);
        }


     

        string hashstr = generateHashValue(sb.ToString(), key);
        return hashstr;
    }
 
    #endregion
    public bool getHashCode(string billerid)
    {
        bool result = false;

        // after creat biller need to code here
        return result;
    }
    public bool checkBillerID(string taxid, out string billerid)
    {

        billerid = string.Empty;
        bool result = true;
        DataSet ds = null;

        try
        {

            // need to check 


        }
        catch { }



        return result;

    }

    public static string getSHA1(string input)
    {
        var sha1 = SHA1Managed.Create();
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] outputBytes = sha1.ComputeHash(inputBytes);
        return BitConverter.ToString(outputBytes).Replace("-", "").ToLower();
    }

    public static Hashtable getMerchantCodeHashtable(string merchantCode)
    {
        Hashtable hashtable = new Hashtable();
        string[] strArray = merchantCode.Split(";".ToCharArray());
        for (int i = 0; i < strArray.Length; i++)
        {
            string[] strArray2 = strArray[i].Split(":".ToCharArray());
            if (strArray2.Length > 1)
            {
                hashtable.Add(strArray2[0], strArray2[1]);
            }
        }
        return hashtable;
    }

    public static string getQUrl(string URL)
    {
        string errMsg = "";
        //  writeLog("getQUrl : " + URL);
        string qPayAPI = ConfigurationManager.AppSettings["qPayAPI"].ToString();
        string quickpayURL = URL;


        //call api here
        SSLPost sslPost = new SSLPost();
        qPayAPI = qPayAPI + "&action=shorturl&format=xml&url=" + quickpayURL;
        writeLog("qPayAPI : " + qPayAPI);
        string resData = "";
        if (!sslPost.postData(qPayAPI, "", out resData, out errMsg))
        {
            writeLog("Error in postData : " + errMsg);
        }
        else
        {
            Hashtable htQpay = Utils.getHTableFromXML2(resData);
            if (htQpay.ContainsKey("shorturl"))
                quickpayURL = htQpay["shorturl"].ToString();
        }
        return quickpayURL;
    }
    public static Hashtable getHTableFromXML2(string requestXML)
    {
        Hashtable ht = new Hashtable();
        XmlDocument xdoc = new XmlDocument();
        xdoc.LoadXml(requestXML);
        XmlNodeList xnl = xdoc.ChildNodes;
        if (xnl.Count > 0)
        {

            XmlNode rootNode;
            if (xnl.Count > 1)
                rootNode = xnl.Item(1);
            else

                rootNode = xnl.Item(0);

            XmlNodeList subNodes = rootNode.ChildNodes;

            if (subNodes.Count >= 1)
            {
                foreach (XmlNode xn in subNodes)
                {
                    //if (xn.InnerText.Length == 1)
                    //{
                    ht.Add(xn.Name, xn.InnerText);
                    //}
                    //else
                    //{
                    //for (int i = 0; i < xn.ChildNodes.Count; i++)
                    //{
                    //    ht.Add(xn.ChildNodes[i].Name, xn.ChildNodes[i].InnerText);
                    //}
                    //}

                }
            }
            else
            {
                ht = null;
            }
        }
        else
        {
            ht = null;
        }

        return ht;
    }

    public static Hashtable ConvertJSONtoHashTable(string strJSON)
    {
        Hashtable ht = new Hashtable();
        try
        {
            ht = JsonConvert.DeserializeObject<Hashtable>(strJSON);
        }
        catch (Exception ex)
        {
            log.Debug("Error in ConvertJSONtoHashTable : " + ex.Message.ToString());
            return null;
        }
        return ht;
    }

    public static T Deserialize<T>(string json)
    {
        T obj = Activator.CreateInstance<T>();
        MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        obj = (T)serializer.ReadObject(ms);
        ms.Close();
        return obj;
    }
    public static string ReadJson(string jsonRoot, string key, string option, string strJson)
    {
        string tempstr = "";
        try
        {
            JObject o = JObject.Parse(strJson);
            JObject oReq = (JObject)o[jsonRoot];

            if (option == "Y")
            {
                JObject oKey = (JObject)oReq[key];
                tempstr = JsonConvert.SerializeObject(oKey);
            }
            else
            {
                tempstr = oReq[key].ToString();
            }
        }
        catch 
        {
            return string.Empty;
        }
        
        return tempstr;
    }

    public static Hashtable getBilleridHashtable(string billerid)
    {
        Hashtable hashtable = new Hashtable();
        string[] strArray = billerid.Split(";".ToCharArray());
        for (int i = 0; i < strArray.Length; i++)
        {
            string[] strArray2 = strArray[i].Split(":".ToCharArray());
            if (strArray2.Length > 1)
            {
                hashtable.Add(strArray2[0], strArray2[1]);
            }
        }
        return hashtable;
    }
    public static string legacyEncrypt(string clearText)
    {
        string EncryptionKey = ConfigurationManager.AppSettings["legacyKey"].ToString();
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }

    public static string legacyDecrypt(string cipherText)
    {
        string EncryptionKey = ConfigurationManager.AppSettings["legacyKey"].ToString();
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }
    public class Constant
    {
        public const string MINPWDLENGTH = "minimum_password_length";
        public const string MINPWDHISTORY = "minimum_password_history";
        public const string MINUPPPERCHAR = "minimum_uppercase_char";
        public const string MINNUMCHAR = "minimum_numeric_char";
        public const string MINSPECCHAR = "minimum_special_char";

        public const string PASSWORDEXPIRY = "password_expiry_period";
        public const string FIRSTLOGINENFORCED = "firsttime_login_enforced";
        public const string MAXLOGINATTEMPT = "max_login_attempt";
    }
   
    //string version, string timeStamp, string email, string password, string messageid, string billerName, string billerLogo, string rescode, string resdesc, string ref1, string ref2, string ref3, string ref4, string ref5,
    //    string ref1Name, string ref2Name, string ref3Name, string ref4Name, string ref5Name, string batchid, string availablebalance, string txnID, string TodayTxnCount, string TodayTxnAmount, string smsMsg
    // 
    public static string getConfirmRes(ConfirmResponseModel confirmRes)
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder sblog = new StringBuilder();
        sb.Append("<ConfirmRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");// 
        sb.Append("<Email>" + confirmRes.email + "</Email>");
        sb.Append("<Password>" + confirmRes.password + "</Password>");
        sb.Append("<TaxID>" +confirmRes.taxID + "</TaxID>");
        sb.Append("<MessageID>" + confirmRes.messageid + "</MessageID>");
        sb.Append("<BillerName>" + confirmRes.billername + "</BillerName>");
        sb.Append("<BillerLogo>" + confirmRes.billerlogo + "</BillerLogo>");
        sb.Append("<ResCode>" + confirmRes.rescode + "</ResCode>");
        sb.Append("<ResDesc>" + confirmRes.resdesc + "</ResDesc>");
        sb.Append("<Ref1>" + HttpUtility.HtmlEncode(confirmRes.ref1) + "</Ref1>");
        sb.Append("<Ref2>" + HttpUtility.HtmlEncode(confirmRes.ref2) + "</Ref2>");
        sb.Append("<Ref3>" + HttpUtility.HtmlEncode(confirmRes.ref3) + "</Ref3>");
        sb.Append("<Ref4>" + HttpUtility.HtmlEncode(confirmRes.ref4) + "</Ref4>");
        sb.Append("<Ref5>" + HttpUtility.HtmlEncode(confirmRes.ref5) + "</Ref5>");
        sb.Append("<Ref6>" + HttpUtility.HtmlEncode(confirmRes.ref6) + "</Ref6>");
        sb.Append("<Ref1Name>" + confirmRes.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + confirmRes.ref2Name + "</Ref2Name>");
        sb.Append("<Ref3Name>" + confirmRes.ref3Name + "</Ref3Name>");
        sb.Append("<Ref4Name>" + confirmRes.ref4Name + "</Ref4Name>");
        sb.Append("<Ref5Name>" + confirmRes.ref5Name + "</Ref5Name>");
        sb.Append("<BatchID>" + confirmRes.batchID + "</BatchID>");
        sb.Append("<Balance>" + confirmRes.availablebalance + "</Balance>");
        sb.Append("<TxnID>" + confirmRes.txnID + "</TxnID>");
        sb.Append("<TodayTxnCount>" + confirmRes.TodayTxnCount + "</TodayTxnCount>");
        sb.Append("<TodayTxnAmount>" + confirmRes.TodayTxnAmount + "</TodayTxnAmount>");
        sb.Append("<SMS>" + confirmRes.smsMsg + "</SMS>");
        sb.Append("</ConfirmRes>");

        #region for Log
        sblog.Append("<ConfirmRes>");
        sblog.Append("<Version>1.0</Version>");
        sblog.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");// 
        sblog.Append("<Email>" + confirmRes.email + "</Email>");
        sblog.Append("<TaxID>" + confirmRes.taxID + "</TaxID>");
        sblog.Append("<MessageID>" + confirmRes.messageid + "</MessageID>");
        sblog.Append("<BillerName>" + confirmRes.billername + "</BillerName>");
        sblog.Append("<ResCode>" + confirmRes.rescode + "</ResCode>");
        sblog.Append("<ResDesc>" + confirmRes.resdesc + "</ResDesc>");
        sblog.Append("<Ref1>" + confirmRes.ref1 + "</Ref1>");
        sblog.Append("<Ref2>" + confirmRes.ref2 + "</Ref2>");
        sblog.Append("<Ref3>" + confirmRes.ref3 + "</Ref3>");
        sblog.Append("<Ref4>" + confirmRes.ref4 + "</Ref4>");
        sblog.Append("<Ref5>" + confirmRes.ref5 + "</Ref5>");
        sblog.Append("<Ref1Name>" + confirmRes.ref1Name + "</Ref1Name>");
        sblog.Append("<Ref2Name>" + confirmRes.ref2Name + "</Ref2Name>");
        sblog.Append("<Ref3Name>" + confirmRes.ref3Name + "</Ref3Name>");
        sblog.Append("<Ref4Name>" + confirmRes.ref4Name + "</Ref4Name>");
        sblog.Append("<Ref5Name>" + confirmRes.ref5Name + "</Ref5Name>");
        sblog.Append("<BatchID>" + confirmRes.batchID + "</BatchID>");
        sblog.Append("<Balance>" + confirmRes.availablebalance + "</Balance>");
        sblog.Append("<TxnID>" + confirmRes.txnID + "</TxnID>");
        sblog.Append("<TodayTxnCount>" + confirmRes.TodayTxnCount + "</TodayTxnCount>");
        sblog.Append("<TodayTxnAmount>" + confirmRes.TodayTxnAmount + "</TodayTxnAmount>");
        sblog.Append("<SMS>" + confirmRes.smsMsg + "</SMS>");
        sblog.Append("</ConfirmRes>");

        #endregion
        writeLog("RES XML : " + sblog.ToString());

        return sb.ToString();
    }

    public static string GetMerchantAcceptanceRes(ConfirmResponseModel confirmRes)
    {
        var sb = new StringBuilder();
        
        sb.Append("<MerchantAcceptanceRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");// 
        sb.Append("<Email>" + confirmRes.email + "</Email>");
        sb.Append("<Password>" + confirmRes.password + "</Password>");
        sb.Append("<MessageID>" + confirmRes.messageid + "</MessageID>");
        sb.Append("<ResCode>" + confirmRes.rescode + "</ResCode>");
        sb.Append("<ResDesc>" + confirmRes.resdesc + "</ResDesc>");
        sb.Append("<Ref1>" + confirmRes.ref1 + "</Ref1>");
        sb.Append("<Ref2>" + confirmRes.ref2 + "</Ref2>");
        sb.Append("<Ref3>" + confirmRes.ref3 + "</Ref3>");
        sb.Append("<Ref4>" + confirmRes.ref4 + "</Ref4>");
        sb.Append("<Ref5>" + confirmRes.ref5 + "</Ref5>");
        sb.Append("<Ref6>" + confirmRes.ref6 + "</Ref6>");
        sb.Append("<Ref1Name>" + confirmRes.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + confirmRes.ref2Name + "</Ref2Name>");
        sb.Append("<Ref3Name>" + confirmRes.ref3Name + "</Ref3Name>");
        sb.Append("<Ref4Name>" + confirmRes.ref4Name + "</Ref4Name>");
        sb.Append("<Ref5Name>" + confirmRes.ref5Name + "</Ref5Name>");
        sb.Append("<TID>" + confirmRes.Tid + "</TID>");
        sb.Append("<MID>" + confirmRes.Mid + "</MID>");
        sb.Append("<ApprovalCode>" + confirmRes.approvalCode + "</ApprovalCode>");
        sb.Append("<TxnID>" + confirmRes.txnID + "</TxnID>");
        sb.Append("<SMS>" + confirmRes.smsMsg + "</SMS>");
        sb.Append("<IsVoidable>" + confirmRes.isVoidable + "</IsVoidable>");
        sb.Append("</MerchantAcceptanceRes>");

        writeLog("RES XML : " + sb.ToString());

        return sb.ToString();
    }

    public static string GetEPaymentVoidRes(ConfirmResponseModel confirmRes)
    {
        var sb = new StringBuilder();

        sb.Append("<EPaymentVoidRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");// 
        sb.Append("<Email>" + confirmRes.email + "</Email>");
        sb.Append("<Password>" + confirmRes.password + "</Password>");
        sb.Append("<MessageID>" + confirmRes.messageid + "</MessageID>");
        sb.Append("<ResCode>" + confirmRes.rescode + "</ResCode>");
        sb.Append("<ResDesc>" + confirmRes.resdesc + "</ResDesc>");
        sb.Append("<Ref1>" + confirmRes.ref1 + "</Ref1>");
        sb.Append("<Ref2>" + confirmRes.ref2 + "</Ref2>");
        sb.Append("<Ref3>" + confirmRes.ref3 + "</Ref3>");
        sb.Append("<Ref4>" + confirmRes.ref4 + "</Ref4>");
        sb.Append("<Ref5>" + confirmRes.ref5 + "</Ref5>");
        sb.Append("<Ref6>" + confirmRes.ref6 + "</Ref6>");
        sb.Append("<Ref1Name>" + confirmRes.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + confirmRes.ref2Name + "</Ref2Name>");
        sb.Append("<Ref3Name>" + confirmRes.ref3Name + "</Ref3Name>");
        sb.Append("<Ref4Name>" + confirmRes.ref4Name + "</Ref4Name>");
        sb.Append("<Ref5Name>" + confirmRes.ref5Name + "</Ref5Name>");
        sb.Append("<TID>" + confirmRes.Tid + "</TID>");
        sb.Append("<MID>" + confirmRes.Mid + "</MID>");
        sb.Append("<TxnID>" + confirmRes.txnID + "</TxnID>");
        sb.Append("<SMS>" + confirmRes.smsMsg + "</SMS>");
        sb.Append("</EPaymentVoidRes>");

        writeLog("RES XML : " + sb.ToString());

        return sb.ToString();
    }

    public static string getChangePackageConfirmRes(EBACanalPlusChangePackageConfirmRes confirmRes)
    {
        StringBuilder sblog = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        sb.Append("<ConfirmRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");// 
        sb.Append("<Ref1>" + confirmRes.ref1 + "</Ref1>");
        sb.Append("<Ref2>" + confirmRes.ref2 + "</Ref2>");
        sb.Append("<Ref3>" + confirmRes.ref3 + "</Ref3>");
        sb.Append("<Ref4>" + confirmRes.ref4 + "</Ref4>");
        sb.Append("<Ref5>" + confirmRes.ref5 + "</Ref5>");
        sb.Append("<Ref1Name>" + confirmRes.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + confirmRes.ref2Name + "</Ref2Name>");
        sb.Append("<Ref3Name>" + confirmRes.ref3Name + "</Ref3Name>");
        sb.Append("<Ref4Name>" + confirmRes.ref4Name + "</Ref4Name>");
        sb.Append("<Ref5Name>" + confirmRes.ref5Name + "</Ref5Name>");
        sb.Append("<PackageAmount>" + confirmRes.PackageAmount + "</PackageAmount>");
        sb.Append("<CanalPlusRefID>" + confirmRes.CanalPlusRefID + "</CanalPlusRefID>");
        sb.Append("<AgentFee>" + confirmRes.ServiceFee + "</AgentFee>");
        sb.Append("<TransactionAmount>" + confirmRes.TransactionAmount + "</TransactionAmount>");
        sb.Append("<PackageCode>" + confirmRes.Package[0].ProductCode + "</PackageCode>");
        sb.Append("<PackageLabel>" + confirmRes.Package[0].Description + "</PackageLabel>");
        sb.Append("<DurationCode>" + confirmRes.Duration[0].Code + "</DurationCode>");
        sb.Append("<DurationLabel>" + confirmRes.Duration[0].Label + "</DurationLabel>");
        sb.Append("<TxnID>" + confirmRes.txnID + "</TxnID>");
        sb.Append("<TodayTxnCount>" + confirmRes.TodayTxnCount + "</TodayTxnCount>");
        sb.Append("<TodayTxnAmount>" + confirmRes.TodayTxnAmount + "</TodayTxnAmount>");
        sb.Append("<SMS>" + confirmRes.smsMsg + "</SMS>");
        sb.Append("<ResCode>" + confirmRes.ResponseCode + "</ResCode>");
        sb.Append("<ResDesc>" + confirmRes.ResponseDescription + "</ResDesc>");
        sb.Append("</ConfirmRes>");

        #region For Log
        sblog.Append("<ConfirmRes>");
        sblog.Append("<Version>1.0</Version>");
        sblog.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");// 
        sblog.Append("<Ref1>" + confirmRes.ref1 + "</Ref1>");
        sblog.Append("<Ref2>" + confirmRes.ref2 + "</Ref2>");
        sblog.Append("<Ref3>" + confirmRes.ref3 + "</Ref3>");
        sblog.Append("<Ref4>" + confirmRes.ref4 + "</Ref4>");
        sblog.Append("<Ref5>" + confirmRes.ref5 + "</Ref5>");
        sblog.Append("<Ref1Name>" + confirmRes.ref1Name + "</Ref1Name>");
        sblog.Append("<Ref2Name>" + confirmRes.ref2Name + "</Ref2Name>");
        sblog.Append("<Ref3Name>" + confirmRes.ref3Name + "</Ref3Name>");
        sblog.Append("<Ref4Name>" + confirmRes.ref4Name + "</Ref4Name>");
        sblog.Append("<Ref5Name>" + confirmRes.ref5Name + "</Ref5Name>");
        sblog.Append("<PackageAmount>" + confirmRes.PackageAmount + "</PackageAmount>");
        sblog.Append("<CanalPlusRefID>" + confirmRes.CanalPlusRefID + "</CanalPlusRefID>");
        sblog.Append("<AgentFee>" + confirmRes.ServiceFee + "</AgentFee>");
        sblog.Append("<TransactionAmount>" + confirmRes.TransactionAmount + "</TransactionAmount>");
        sblog.Append("<PackageCode>" + confirmRes.Package[0].ProductCode + "</PackageCode>");
        sblog.Append("<PackageLabel>" + confirmRes.Package[0].Description + "</PackageLabel>");
        sblog.Append("<DurationCode>" + confirmRes.Duration[0].Code + "</DurationCode>");
        sblog.Append("<DurationLabel>" + confirmRes.Duration[0].Label + "</DurationLabel>");
        sblog.Append("<TodayTxnCount>" + confirmRes.TodayTxnCount + "</TodayTxnCount>");
        sblog.Append("<TodayTxnAmount>" + confirmRes.TodayTxnAmount + "</TodayTxnAmount>");
        sblog.Append("<ResCode>" + confirmRes.ResponseCode + "</ResCode>");
        sblog.Append("<ResDesc>" + confirmRes.ResponseDescription + "</ResDesc>");
        sblog.Append("</ConfirmRes>");

        writeLog("RES XML : " + sblog.ToString());
        #endregion

        return sb.ToString();
    }

    public static string getInquiryRes(inquiryResponseModel inqueryRes)
    {
        StringBuilder sbLog = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        sb.Append("<InquiryRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sb.Append("<MerchantName>" + inqueryRes.merchantname + "</MerchantName>");
        sb.Append("<MerchantLogo>" + inqueryRes.merchantlogo + "</MerchantLogo>");
        sb.Append("<BillerName>" + inqueryRes.billername + "</BillerName>");
        sb.Append("<BillerLogo>" + inqueryRes.billerlogo + "</BillerLogo>");
        sb.Append("<Ref1>" + HttpUtility.HtmlEncode(inqueryRes.ref1) + "</Ref1>");
        sb.Append("<Ref2>" + HttpUtility.HtmlEncode(inqueryRes.ref2) + "</Ref2>");
        sb.Append("<Ref3>" + HttpUtility.HtmlEncode(inqueryRes.ref3) + "</Ref3>");
        sb.Append("<Ref4>" + HttpUtility.HtmlEncode(inqueryRes.ref4) + "</Ref4>");
        sb.Append("<Ref5>" + HttpUtility.HtmlEncode(inqueryRes.ref5) + "</Ref5>");
        sb.Append("<Ref6>" + HttpUtility.HtmlEncode(inqueryRes.ref6) + "</Ref6>");
        sb.Append("<Ref1Name>" + inqueryRes.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + inqueryRes.ref2Name + "</Ref2Name>");
        sb.Append("<Ref3Name>" + inqueryRes.ref3Name + "</Ref3Name>");
        sb.Append("<Ref4Name>" + inqueryRes.ref4Name + "</Ref4Name>");
        sb.Append("<Ref5Name>" + inqueryRes.ref5Name + "</Ref5Name>");
        sb.Append("<Ref6Name>" + inqueryRes.ref6Name + "</Ref6Name>");
        sb.Append("<Amount>" + inqueryRes.amount + "</Amount>");
        sb.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");
        
        sb.Append("<Status>" + inqueryRes.status + "</Status>");
        sb.Append("<Expiry>" + inqueryRes.expiry + "</Expiry>");
        sb.Append("<ProductDesc>" + HttpUtility.HtmlEncode(inqueryRes.productDescription) + "</ProductDesc>");
        sb.Append("<ImageURL>" + inqueryRes.imgUrl + "</ImageURL>");
        sb.Append("</InquiryRes>");


        #region only For Log
        sbLog.Append("<InquiryRes>");
        sbLog.Append("<Version>1.0</Version>");
        sbLog.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sbLog.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sbLog.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sbLog.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sbLog.Append("<Ref1>" + inqueryRes.ref1 + "</Ref1>");
        sbLog.Append("<Ref2>" + inqueryRes.ref2 + "</Ref2>");
        sbLog.Append("<Ref3>" + inqueryRes.ref3 + "</Ref3>");
        sbLog.Append("<Ref4>" + inqueryRes.ref4 + "</Ref4>");
        sbLog.Append("<Ref5>" + inqueryRes.ref5 + "</Ref5>");
        sbLog.Append("<Ref1Name>" + inqueryRes.ref1Name + "</Ref1Name>");
        sbLog.Append("<Ref2Name>" + inqueryRes.ref2Name + "</Ref2Name>");
        sbLog.Append("<Ref3Name>" + inqueryRes.ref3Name + "</Ref3Name>");
        sbLog.Append("<Ref4Name>" + inqueryRes.ref4Name + "</Ref4Name>");
        sbLog.Append("<Ref5Name>" + inqueryRes.ref5Name + "</Ref5Name>");
        sbLog.Append("<Amount>" + inqueryRes.amount + "</Amount>");
        sbLog.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");
        sbLog.Append("<Status>" + inqueryRes.status + "</Status>");
        sbLog.Append("<Expiry>" + inqueryRes.expiry + "</Expiry>");
        sbLog.Append("<ProductDesc>" + inqueryRes.productDescription + "</ProductDesc>");
        sbLog.Append("<ImageURL>" + inqueryRes.imgUrl + "</ImageURL>");
        sbLog.Append("</InquiryRes>");
        writeLog("RES XML : " + sbLog.ToString());
        #endregion
        return sb.ToString();

    }

    public static string getInquiryResCanalPlus(inquiryResponseModelCanalPlus inqueryRes)
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder sbLog = new StringBuilder();
        sb.Append("<InquiryRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");       
        sb.Append("<Ref1>" + inqueryRes.ref1 + "</Ref1>");
        sb.Append("<Ref2>" + inqueryRes.ref2 + "</Ref2>");
        sb.Append("<Ref3>" + inqueryRes.ref3 + "</Ref3>");
        sb.Append("<Ref4>" + inqueryRes.ref4 + "</Ref4>");
        sb.Append("<Ref5>" + inqueryRes.ref5 + "</Ref5>");
        sb.Append("<Ref1Name>" + inqueryRes.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + inqueryRes.ref2Name + "</Ref2Name>");
        sb.Append("<Ref3Name>" + inqueryRes.ref3Name + "</Ref3Name>");
        sb.Append("<Ref4Name>" + inqueryRes.ref4Name + "</Ref4Name>");
        sb.Append("<Ref5Name>" + inqueryRes.ref5Name + "</Ref5Name>");
        sb.Append("<Amount>" + inqueryRes.amount + "</Amount>");
        sb.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");
        sb.Append("<Status>" + inqueryRes.status + "</Status>");
        sb.Append("<Expiry>" + inqueryRes.expiry + "</Expiry>");
        sb.Append("<ProductDesc>" + inqueryRes.productDescription + "</ProductDesc>");
        sb.Append("<ImageURL>" + inqueryRes.imgUrl + "</ImageURL>");
        sb.Append("<BillerName>" + inqueryRes.billerName + "</BillerName>");
        sb.Append("<BillerLogo>" + inqueryRes.billerLogo + "</BillerLogo>");
        sb.Append("</InquiryRes>");


        #region only For Log
        sbLog.Append("<InquiryRes>");
        sbLog.Append("<Version>1.0</Version>");
        sbLog.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sbLog.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sbLog.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sbLog.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sbLog.Append("<Ref1>" + inqueryRes.ref1 + "</Ref1>");
        sbLog.Append("<Ref2>" + inqueryRes.ref2 + "</Ref2>");
        sbLog.Append("<Ref3>" + inqueryRes.ref3 + "</Ref3>");
        sbLog.Append("<Ref4>" + inqueryRes.ref4 + "</Ref4>");
        sbLog.Append("<Ref5>" + inqueryRes.ref5 + "</Ref5>");
        sbLog.Append("<Ref1Name>" + inqueryRes.ref1Name + "</Ref1Name>");
        sbLog.Append("<Ref2Name>" + inqueryRes.ref2Name + "</Ref2Name>");
        sbLog.Append("<Ref3Name>" + inqueryRes.ref3Name + "</Ref3Name>");
        sbLog.Append("<Ref4Name>" + inqueryRes.ref4Name + "</Ref4Name>");
        sbLog.Append("<Ref5Name>" + inqueryRes.ref5Name + "</Ref5Name>");
        sbLog.Append("<Amount>" + inqueryRes.amount + "</Amount>");
        sbLog.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");
        sbLog.Append("<Status>" + inqueryRes.status + "</Status>");
        sbLog.Append("<Expiry>" + inqueryRes.expiry + "</Expiry>");
        sbLog.Append("<ProductDesc>" + inqueryRes.productDescription + "</ProductDesc>");
        sbLog.Append("<ImageURL>" + inqueryRes.imgUrl + "</ImageURL>");
        sbLog.Append("</InquiryRes>");
        writeLog("RES XML : " + sbLog.ToString());
        #endregion
        return sb.ToString();

    }

    #region  SolarHome
    public static string getInquiryResSolarHome(inquiryResponseModelSolarHome inqueryRes)
    {
        StringBuilder sbLog = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        sb.Append("<InquiryRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sb.Append("<Amount>" + inqueryRes.amount + "</Amount>");
        sb.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");
        sb.Append("<Active>" + inqueryRes.Active + "</Active>");
        sb.Append("<Description>" + inqueryRes.Description + "</Description>");
        sb.Append("<Name>" + inqueryRes.Name + "</Name>");
        sb.Append("<Ref1>" + inqueryRes.ref1 + "</Ref1>");
        sb.Append("<Ref2>" + inqueryRes.ref2 + "</Ref2>");
        sb.Append("<Ref3>" + inqueryRes.ref3 + "</Ref3>");
        sb.Append("<Ref4>" + inqueryRes.ref4 + "</Ref4>");
        sb.Append("<Ref5>" + inqueryRes.ref5 + "</Ref5>");
        sb.Append("<Ref1Name>" + inqueryRes.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + inqueryRes.ref2Name + "</Ref2Name>");
        sb.Append("<Ref3Name>" + inqueryRes.ref3Name + "</Ref3Name>");
        sb.Append("<Ref4Name>" + inqueryRes.ref4Name + "</Ref4Name>");
        sb.Append("<Ref5Name>" + inqueryRes.ref5Name + "</Ref5Name>");
                sb.Append("<BillerName>" + inqueryRes.billerName + "</BillerName>");
        sb.Append("<TestAccount>" + inqueryRes.TestAccount + "</TestAccount>");
        sb.Append("</InquiryRes>");


        #region only For Log
        sbLog.Append("<InquiryRes>");
        sbLog.Append("<Version>1.0</Version>");
        sbLog.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sbLog.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sbLog.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sbLog.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sbLog.Append("<Amount>" + inqueryRes.amount + "</Amount>");
        sbLog.Append("<Active>" + inqueryRes.Active + "</Active>");
        sbLog.Append("<Description>" + inqueryRes.Description + "</Description>");
        sbLog.Append("<Name>" + inqueryRes.Name + "</Name>");
        sbLog.Append("<Ref1>" + inqueryRes.ref1 + "</Ref1>");
        sbLog.Append("<Ref2>" + inqueryRes.ref2 + "</Ref2>");
        sbLog.Append("<Ref3>" + inqueryRes.ref3 + "</Ref3>");
        sbLog.Append("<Ref4>" + inqueryRes.ref4 + "</Ref4>");
        sbLog.Append("<Ref5>" + inqueryRes.ref5 + "</Ref5>");
        sbLog.Append("<Ref1Name>" + inqueryRes.ref1Name + "</Ref1Name>");
        sbLog.Append("<Ref2Name>" + inqueryRes.ref2Name + "</Ref2Name>");
        sbLog.Append("<Ref3Name>" + inqueryRes.ref3Name + "</Ref3Name>");
        sbLog.Append("<Ref4Name>" + inqueryRes.ref4Name + "</Ref4Name>");
        sbLog.Append("<Ref5Name>" + inqueryRes.ref5Name + "</Ref5Name>");
        sbLog.Append("<BillerName>" + inqueryRes.billerName + "</BillerName>");
        sbLog.Append("<TestAccount>" + inqueryRes.TestAccount + "</TestAccount>");
        sbLog.Append("</InquiryRes>");
        writeLog("RES XML : " + sbLog.ToString());
        #endregion
        return sb.ToString();

    }

    public static string getConfirmResSolarHome(SolarHomeConfirmResponse confirmRes)
    {
        StringBuilder sbLog = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        sb.Append("<ConfirmRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + confirmRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + confirmRes.ResDesc + "</ResDesc>");
        sb.Append("<Status>" + confirmRes.Status + "</Status>");
        sb.Append("<Message>" + confirmRes.ResDesc + "</Message>");
        sb.Append("<TxnID>" + confirmRes.Trans_ID + "</TxnID>");
        sb.Append("<Ref1>" + confirmRes.ref1 + "</Ref1>");
        sb.Append("<Ref2>" + confirmRes.ref2 + "</Ref2>");
        sb.Append("<Ref3>" + confirmRes.ref3 + "</Ref3>");
        sb.Append("<Ref4>" + confirmRes.ref4 + "</Ref4>");
        sb.Append("<Ref5>" + confirmRes.ref3 + "</Ref5>");
        sb.Append("<Ref1Name>" + confirmRes.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + confirmRes.ref2Name + "</Ref2Name>");
        sb.Append("<Ref3Name>" + confirmRes.ref3Name + "</Ref3Name>");
        sb.Append("<Ref4Name>" + confirmRes.ref4Name + "</Ref4Name>");
        sb.Append("<Ref5Name>" + confirmRes.ref5Name + "</Ref5Name>");
        sb.Append("<TodayTxnCount>" + confirmRes.TodayTxnCount + "</TodayTxnCount>");
        sb.Append("<TodayTxnAmount>" + confirmRes.TodayTxnAmount + "</TodayTxnAmount>");
        sb.Append("<SMS>" + confirmRes.Message + "</SMS>");
        sb.Append("</ConfirmRes>");

        #region only For Log
        //sbLog.Append("<ConfirmRes>");
        //sbLog.Append("<Version>1.0</Version>");
        //sbLog.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        //sbLog.Append("<ResCode>" + confirmRes.ResCode + "</ResCode>");
        //sbLog.Append("<ResDesc>" + confirmRes.ResDesc + "</ResDesc>");
        //sbLog.Append("<Status>" + confirmRes.Status + "</Status>");
        //sbLog.Append("<Message>" + confirmRes.ResDesc + "</Message>");
        //sbLog.Append("<TxnID>" + confirmRes.Trans_ID + "</TxnID>");
        //sbLog.Append("<Ref1>" + confirmRes.Amount + "</Ref1>");
        //sbLog.Append("</ConfirmRes>");
        writeLog("RES XML : " + sb.ToString());
        #endregion
        return sb.ToString();
    }
    #endregion

    #region Phatama Group
    public static string getPendingInvoiceByCustomerIDResPG(PGPendingInvoiceResponse pgPendingInvoiceResp)
    {
        StringBuilder sbLog = new StringBuilder();
        StringBuilder sb = new StringBuilder();

        sb.Append("<InquiryPGRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        if (pgPendingInvoiceResp.ErrorCode.Equals("0"))
        {
            sb.Append("<ResCode>00</ResCode>");
            sb.Append("<ResDesc>Success</ResDesc>");    
        }
        sb.Append("<Ref1>" + pgPendingInvoiceResp.AgentCode + "</Ref1>");
        sb.Append("<Ref2>" + pgPendingInvoiceResp.CustomerID + "</Ref2>");
        sb.Append("<Ref1Name>" + pgPendingInvoiceResp.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + pgPendingInvoiceResp.ref2Name + "</Ref2Name>");
        sb.Append("<CustomerName>" + pgPendingInvoiceResp.CustomerName + "</CustomerName>");
        sb.Append("<Invoices>");
        if (pgPendingInvoiceResp.Invoices.Count > 0)
        {
            foreach (PGInvoice pgInv in pgPendingInvoiceResp.Invoices)
            {
                sb.Append("<Invoice>");
                sb.Append("<Address>" + pgInv.Address + "</Address>");
                sb.Append("<Amount>" + pgInv.Amount + "</Amount>");
                sb.Append("<BranchCode>" + pgInv.BranchCode + "</BranchCode>");
                if (!string.IsNullOrEmpty(pgInv.DueDate))
                {
                    DateTime tmpDate = DateTime.Parse(pgInv.DueDate);
                    sb.Append("<DueDate>" + tmpDate.ToShortDateString() + "</DueDate>");
                }
                
                sb.Append("<InvoiceDate>" + pgInv.InvoiceDate + "</InvoiceDate>");
                sb.Append("<InvoiceNumber>" + pgInv.InvoiceNumber + "</InvoiceNumber>");
                sb.Append("<Remark>" + pgInv.Remark + "</Remark>");
                sb.Append("</Invoice>");
            }
        }
        sb.Append("</Invoices>");
        sb.Append("<Amount>" + pgPendingInvoiceResp.Amount + "</Amount>");
        sb.Append("<AgentFee>" + pgPendingInvoiceResp.ServiceFee + "</AgentFee>");
        sb.Append("<FailReason>" + pgPendingInvoiceResp.FailReason + "</FailReason>");
        sb.Append("</InquiryPGRes>");
        writeLog("InquiryPGRes " +sb.ToString());
        return sb.ToString();
    }

    public static string getConfirmPendingInvoiceRes(PGConfirmPendingInvoiceResponse pgConfirmPendingInvoiceResp)
    {
        StringBuilder sbLog = new StringBuilder();
        StringBuilder sb = new StringBuilder();

        sb.Append("<ConfirmPGRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        if (pgConfirmPendingInvoiceResp.ErrorCode.Equals("0"))
        {
            sb.Append("<ResCode>00</ResCode>");
            sb.Append("<ResDesc>Success</ResDesc>");    
        }
        else
        {
            sb.Append("<ResCode>" + pgConfirmPendingInvoiceResp.ErrorCode + "</ResCode>");
            sb.Append("<ResDesc>" + pgConfirmPendingInvoiceResp.FailReason + "</ResDesc>");    
        }
        sb.Append("<PartnerCode>" + pgConfirmPendingInvoiceResp.PartnerCode + "</PartnerCode>");
        sb.Append("<CustomerID>" + pgConfirmPendingInvoiceResp.CustomerID + "</CustomerID>");
        sb.Append("<TxnID>" + pgConfirmPendingInvoiceResp.TxnID + "</TxnID>");
        sb.Append("<Invoices>");
        if (pgConfirmPendingInvoiceResp.Invoices.Count > 0)
        {
            foreach (PGResConfirmInvoice inv in pgConfirmPendingInvoiceResp.Invoices)
            {
                sb.Append("<Invoice>");
                sb.Append("<InvoiceNumber>" + inv.InvoiceNumber + "</InvoiceNumber>");
                sb.Append("<Amount>" + inv.Amount + "</Amount>");
                sb.Append("<PaymentReference>" + inv.PaymentReference + "</PaymentReference>");
                sb.Append("<Status>" + inv.Status + "</Status>");
                sb.Append("</Invoice>");
            }
        }
        sb.Append("</Invoices>");
        //sb.Append("<SMS>" + pgConfirmPendingInvoiceResp.smsMsg + "</SMS>");
        sb.Append("</ConfirmPGRes>");


        return sb.ToString();
    }
    #endregion

    public static string getInquiryReswithRef6(inquiryResponseModel inqueryRes)
    {
        StringBuilder sbLog = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        sb.Append("<InquiryRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sb.Append("<MerchantName>" + inqueryRes.merchantname + "</MerchantName>");
        sb.Append("<MerchantLogo>" + inqueryRes.merchantlogo + "</MerchantLogo>");
        sb.Append("<BillerName>" + inqueryRes.billername + "</BillerName>");
        sb.Append("<BillerLogo>" + inqueryRes.billerlogo + "</BillerLogo>");
        sb.Append("<Ref1>" + inqueryRes.ref1 + "</Ref1>");
        sb.Append("<Ref2>" + inqueryRes.ref2 + "</Ref2>");
        sb.Append("<Ref3>" + inqueryRes.ref3 + "</Ref3>");
        sb.Append("<Ref4>" + inqueryRes.ref4 + "</Ref4>");
        sb.Append("<Ref5>" + inqueryRes.ref5 + "</Ref5>");
        sb.Append("<Ref6>" + inqueryRes.ref6 + "</Ref6>");
        sb.Append("<Ref1Name>" + inqueryRes.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + inqueryRes.ref2Name + "</Ref2Name>");
        sb.Append("<Ref3Name>" + inqueryRes.ref3Name + "</Ref3Name>");
        sb.Append("<Ref4Name>" + inqueryRes.ref4Name + "</Ref4Name>");
        sb.Append("<Ref5Name>" + inqueryRes.ref5Name + "</Ref5Name>");
        sb.Append("<Ref6Name>" + inqueryRes.ref6Name + "</Ref6Name>");
        sb.Append("<Amount>" + inqueryRes.amount + "</Amount>");
        sb.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");

        sb.Append("<Status>" + inqueryRes.status + "</Status>");
        sb.Append("<Expiry>" + inqueryRes.expiry + "</Expiry>");
        sb.Append("<ProductDesc>" + inqueryRes.productDescription + "</ProductDesc>");
        sb.Append("<ImageURL>" + inqueryRes.imgUrl + "</ImageURL>");
        sb.Append("</InquiryRes>");

        #region only For Log

        sbLog.Append("<InquiryRes>");
        sbLog.Append("<Version>1.0</Version>");
        sbLog.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sbLog.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sbLog.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sbLog.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sbLog.Append("<Ref1>" + inqueryRes.ref1 + "</Ref1>");
        sbLog.Append("<Ref2>" + inqueryRes.ref2 + "</Ref2>");
        sbLog.Append("<Ref3>" + inqueryRes.ref3 + "</Ref3>");
        sbLog.Append("<Ref4>" + inqueryRes.ref4 + "</Ref4>");
        sbLog.Append("<Ref5>" + inqueryRes.ref5 + "</Ref5>");
        sbLog.Append("<Ref6>" + inqueryRes.ref6 + "</Ref6>");
        sbLog.Append("<Ref1Name>" + inqueryRes.ref1Name + "</Ref1Name>");
        sbLog.Append("<Ref2Name>" + inqueryRes.ref2Name + "</Ref2Name>");
        sbLog.Append("<Ref3Name>" + inqueryRes.ref3Name + "</Ref3Name>");
        sbLog.Append("<Ref4Name>" + inqueryRes.ref4Name + "</Ref4Name>");
        sbLog.Append("<Ref5Name>" + inqueryRes.ref5Name + "</Ref5Name>");
        sbLog.Append("<Ref6Name>" + inqueryRes.ref6Name + "</Ref6Name>");
        sbLog.Append("<Amount>" + inqueryRes.amount + "</Amount>");
        sbLog.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");

        sbLog.Append("<Status>" + inqueryRes.status + "</Status>");
        sbLog.Append("<Expiry>" + inqueryRes.expiry + "</Expiry>");
        sbLog.Append("<ProductDesc>" + inqueryRes.productDescription + "</ProductDesc>");
        sbLog.Append("<ImageURL>" + inqueryRes.imgUrl + "</ImageURL>");
        sbLog.Append("</InquiryRes>");
        writeLog("RES XML : " + sbLog.ToString());
        #endregion
        return sb.ToString();

    }
    public static string getRepaymentServiceFeeRes(RepaymentServiceFeesResMdl response)
    {
      

        StringBuilder responseXML = new StringBuilder();
        responseXML.Append("<RepaymentServiceFeeRes>");
        responseXML.Append("<Version>{0}</Version>");
        responseXML.Append("<TimeStamp>{1}</TimeStamp>");
        responseXML.Append("<MessageID>{2}</MessageID>");
        responseXML.Append("<Amount>{3}</Amount>");
        responseXML.Append("<ServiceFee>{4}</ServiceFee>");
        responseXML.Append("<ResCode>{5}</ResCode>");
        responseXML.Append("<ResDesc>{6}</ResDesc>");
        responseXML.Append("</RepaymentServiceFeeRes>");
        return string.Format(responseXML.ToString(), response.Version, System.DateTime.Now.ToString("yyyyMMddhhmmssffff"), response.MessageID, response.Amount, response.ServiceFee, response.ResCode, response.ResDesc);
        //<RepaymentServiceFeeRes>
        //<Version>1.0</Version>
        //<TimeStamp>yyyyMMddhhmmssffff</TimeStamp>
        //<MessageID>768866yyhhhhhh</MessageID>
        //<ResCode>00</ResCode>
        //<ResDesc></ResDesc>
        //<Amount>10000</Amount>
        //<ServiceFee> 200 </ServiceFee> 
        //</RepaymentServiceFeeRes>
    }

    public static string getB2BCancelRes(B2BCancelResponseModel b2bCancelRes)
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder sblog = new StringBuilder();
        sb.Append("<B2BCancelRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");// 
        sb.Append("<ResCode>" + b2bCancelRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + b2bCancelRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + b2bCancelRes.taxID + "</TaxID>");
        sb.Append("</B2BCancelRes>");

        writeLog("RES XML : " + sb.ToString());

        return sb.ToString();
    }

    #region ESBA
    public static string oAuthRequest()
    {
        writeLog("$$$$$$$$$$$$$$This Is oAuthRequest$$$$$$$$$$$$$$$$$$$");
        string clientOuthSecurityString = ConfigurationManager.AppSettings["OautKey"].ToString();// "1Stop:7ggrYkmFX73xDEbHOos9";
        //string useURL = ConfigurationManager.AppSettings["OautUrl"].ToString();// string.Format("https://sandbox-api.2c2p.com:8080/oauth/token");
        string useURL = ConfigurationManager.AppSettings["OautUrl"].ToString();// string.Format("https://uat-oauth.2c2p.com/oauth/token");        
        string host = ConfigurationManager.AppSettings["OautHost"].ToString();
        writeLog("URL:" + useURL);
        HttpWebRequest objRequest = (HttpWebRequest)(WebRequest.Create(useURL));
        objRequest.Method = "POST";
        objRequest.ProtocolVersion = HttpVersion.Version11;

        string strAuth = stringToBase64(clientOuthSecurityString);
        //string strAuth = stringToBase64("1Stop:7ggrYkmFX73xDEbHOos9");
        //string strAuth = stringToBase64("EasyBillsAggregator:1z50as!-_dqfVh_aIo");
        objRequest.Headers.Add("Authorization", " Basic " + strAuth);
        objRequest.ContentType = "application/json"; //"text/plain; charset=utf-8";
        objRequest.Host = host;
        objRequest.ContentLength = 29;
        //objRequest.Expect = "100-continue";            
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        StreamWriter postWriter = new StreamWriter(objRequest.GetRequestStream());
        postWriter.Write("grant_type=client_credentials");
        postWriter.Close();
        HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
        StreamReader sr = new StreamReader(objResponse.GetResponseStream());
        string rawOutput = sr.ReadToEnd();
        sr.Close();

        string[] tokenInfoArray = rawOutput.Split('"');
        string token = tokenInfoArray[3];
        writeLog("$$$$$$$$$ Request OK Token:" + token);
        return token;
    }

    public static string pgOAuthRequest()
    {
        writeLog("$$$$$$$$$$$$$$This Is pgOAuthRequest$$$$$$$$$$$$$$$$$$$");
        string clientOuthSecurityString = ConfigurationManager.AppSettings["PGOautKey"].ToString();// "1Stop:7ggrYkmFX73xDEbHOos9";
        //string useURL = ConfigurationManager.AppSettings["OautUrl"].ToString();// string.Format("https://sandbox-api.2c2p.com:8080/oauth/token");
        string useURL = ConfigurationManager.AppSettings["OautUrl"].ToString();// string.Format("https://uat-oauth.2c2p.com/oauth/token");        
        string host = ConfigurationManager.AppSettings["OautHost"].ToString();
        writeLog("URL:" + useURL);
        HttpWebRequest objRequest = (HttpWebRequest)(WebRequest.Create(useURL));
        objRequest.Method = "POST";
        objRequest.ProtocolVersion = HttpVersion.Version11;

        string strAuth = stringToBase64(clientOuthSecurityString);
        //string strAuth = stringToBase64("1Stop:7ggrYkmFX73xDEbHOos9");
        //string strAuth = stringToBase64("EasyBillsAggregator:1z50as!-_dqfVh_aIo");
        objRequest.Headers.Add("Authorization", " Basic " + strAuth);
        objRequest.ContentType = "application/json"; //"text/plain; charset=utf-8";
        objRequest.Host = host;
        objRequest.ContentLength = 29;
        //objRequest.Expect = "100-continue";            
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        StreamWriter postWriter = new StreamWriter(objRequest.GetRequestStream());
        postWriter.Write("grant_type=client_credentials");
        postWriter.Close();
        HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
        StreamReader sr = new StreamReader(objResponse.GetResponseStream());
        string rawOutput = sr.ReadToEnd();
        sr.Close();

        string[] tokenInfoArray = rawOutput.Split('"');
        string token = tokenInfoArray[3];
        writeLog("$$$$$$$$$ Request OK Token:" + token);
        return token;
    }

    //private static string oAuthRequest(string clientOuthSecurityString)
    //{
    //    //UAT link
    //    string useURL = string.Format("https://sandbox-api.2c2p.com:8080/oauth/token");
       
    //    ////Production link
    //    ////string useURL = string.Format("https://oauth.2c2p.com/oauth/token");

    //    HttpWebRequest objRequest = (HttpWebRequest)(WebRequest.Create(useURL));
    //    objRequest.Method = "POST";
    //    objRequest.ProtocolVersion = HttpVersion.Version11;

    //    string strAuth = stringToBase64(clientOuthSecurityString);
    //    //string strAuth = stringToBase64("1Stop:7ggrYkmFX73xDEbHOos9");
    //    //string strAuth = stringToBase64("EasyBillsAggregator:1z50as!-_dqfVh_aIo");
    //    objRequest.Headers.Add("Authorization", " Basic " + strAuth);
    //    objRequest.ContentType = "application/json"; //"text/plain; charset=utf-8";
    //    objRequest.Host = "sandbox-api.2c2p.com";
    //    objRequest.ContentLength = 29;
    //    //objRequest.Expect = "100-continue";            
    //    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;// | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
    //    StreamWriter postWriter = new StreamWriter(objRequest.GetRequestStream());
    //    postWriter.Write("grant_type=client_credentials");
    //    postWriter.Close();
    //    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
    //    StreamReader sr = new StreamReader(objResponse.GetResponseStream());
    //    string rawOutput = sr.ReadToEnd();
    //    sr.Close();

    //    // { "access_token":"zIyldXK_K_v-UWbkdWCc3cd-i141bCjk8jZ0r8MiKS5Wip3pzK5NBk00esH3n-wg8RoM1hbBB0mqRhbzOOakFrGdG-6fGitS7jnJOxRq3ojavQV1rehUyplywPOxFlL31OvOKiIZw3V5xTJ5P6-_4lXvMAN9MjMWEwgbVOPg4ws-i1V_DI--MkaJ9jIHmDmBknKD5Jsuj5F7a4e2CwhOUyS8_K4","token_type":"bearer","expires_in":1799}


    //    //{ "access_token":"b-RoTGLEb-wdYYve3B_vfBU_bVujQ3CDnzmWHPkqw4-sWx6okefa7ca7WUIq1oC6lu1hAapVZDvZM32jteSVpIpbdZQ1aYhqMFWSeqMRNG9DwdY0874CVncyC5HmAfMyvOR7XfSMyEgo-I9xpya4VKms5jvQnO9OQwPAOajOFDXuNiQbFAEyv2E-TGYoJXltB6xTTd2GORLHfzMP9qxU3xyOX6lACmxAio8VUPTzg9Yf88nP880SNM3iIi_KQS3v2OMxmA","token_type":"bearer","expires_in":1799}

    //    string[] tokenInfoArray = rawOutput.Split('"');
    //    string token = tokenInfoArray[3];

    //    return token;
    //}
    private static string stringToBase64(string st)
    {
        byte[] b = new byte[st.Length];
        for (int i = 0; i < st.Length; i++)
        {
            b[i] = Convert.ToByte(st[i]);
        }
        return Convert.ToBase64String(b);
    }

    public static string pinRequest(string json,string method)
    {
        string result = string.Empty;

        string useURL = ConfigurationManager.AppSettings["EsbaUrl"].ToString() + method;//string.Format("http://172.30.1.25/AggregatorWebPinService/PinService.svc/GetTelcoPIN");

        try
        {
            string requestTimeout=ConfigurationManager.AppSettings["EBARequestTimeOut"].ToString();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(useURL);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(requestTimeout) ;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
        }
        catch(Exception ex)
        {
            writeLog("Exception occur when request to EBA:"+ex.Message);
            result = string.Empty;
        }
        return result;
    }

    public static string PaymentRequest(string json, string url)
    {
        var result = string.Empty;

        try
        {
            var requestTimeout = ConfigurationManager.AppSettings["EBARequestTimeOut"].ToString();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(requestTimeout);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            writeLog("Exception occur when request to EBA:" + ex.Message);
            result = string.Empty;
        }
        return result;
    }


    public static string EBADataRequest(string json, string URL)
    {
        string result = string.Empty;

        string useURL = URL;

        try
        {
            string requestTimeout = ConfigurationManager.AppSettings["EBARequestTimeOut"].ToString();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(useURL);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(requestTimeout);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            writeLog("Exception occur when request to EBA:" + ex.Message);
            result = string.Empty;
        }
        return result;
    }

    public static string AESDecryptText(string input, string key)
    {
        // Get the bytes of the string
        byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        keyBytes = SHA256.Create().ComputeHash(keyBytes);

        byte[] bytesDecrypted = AESDecrypt(bytesToBeDecrypted, keyBytes);

        string result = Encoding.UTF8.GetString(bytesDecrypted);

        return result;
    }

    public static byte[] AESDecrypt(byte[] bytesToBeDecrypted, byte[] keyBytes)
    {
        byte[] decryptedBytes = null;

        // Set your salt here, change it to meet your flavor:
        // The salt bytes must be at least 8 bytes.
        byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(keyBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();
            }
        }

        return decryptedBytes;
    }

    public static string EsbResponseDescription(string resCode)
    {
        string resDesc=string.Empty;
        switch (resCode)
        {
            case "0":
                 resDesc = "SUCCESS";
                break;
            case "1":
                 resDesc = "Out Of Stock";
                break;
            case "2":
                resDesc = "Out Of Stock";//"Insufficient Funds";
                break;
            case "3":
                resDesc = "Time Out";
                break;
            case "4":
                resDesc = "Invalid Channel";
                break;
            case "5":
                resDesc = "Invalid Price";
                break;
            case "6":
                resDesc = "Invalid Network Type";
                break;   
            case "7":
                resDesc = "Invalid Biller";
                break;
            case "8":
                resDesc = "Invalid Card Type";
                break;
            case "9":
                resDesc = "Invalid Source System Router";
                break;
            case "10":
                resDesc = "Invalid Token";
                break;
            case "11":
                resDesc = "System Busy";//Token Timeout
                break;
            case "12":
                resDesc = "System Malfunction";
                break;
            case "13":
                resDesc = "Hash Balance Not Equal";
                break;
            case "14":
                resDesc = "Channel Discount Rate Not Found";
                break;
        }
        return resDesc;

                

        //    0	SUCCESS (PAID)
        //1	Out Of Stock
        //2	Insufficient Funds
        //3	Time Out
        //4	Invalid Channel
        //5	Invalid Price
        //6	Invalid Network Type
        //7	Invalid Biller
        //8	Invalid Card Type
        //9	Invalid Source System Router
        //10	Invalid Token
        //11	Token Time Out
        //12	System Malfunction

    }

    public static string GiftCardEnquiryResponseCode(string resCode)
    {
         string resDesc=string.Empty;
         switch (resCode)
         {
             case "0":
                 resDesc = "SUCCESS";
                 break;
             case "1":
                 resDesc = "Fail";
                 break;
             case "2":
                 resDesc = "Invalid Channel";
                 break;
             case "3":
                 resDesc = "Invalid Biller";
                 break;
             case "4":
                 resDesc = "Invalid Token";
                 break;
             case "5":
                 resDesc = "Token Time Out";
                 break;
             case "6":
                 resDesc = "System Malfunction";
                 break;
             case "7":
                 resDesc = "Invalid Price Type";
                 break;
         }

         return resDesc;
       
    }

    public static string EsbAirtimeResponseDescription(string resCode)
    {

         string resDesc="Fail";
        switch (resCode)
        {
            case "0":
                 resDesc = "SUCCESS";
                break;
            case "1":
                 resDesc = "Fail To Topup";
                break;
            case "2":
                resDesc = "Submit Success";
                break;
            case "3":
                resDesc = "Submit Fail";
                break;
            case "4":
                resDesc = "Insufficient Funds";
                break;
            case "5":
                resDesc = "Time Out";
                break;
            case "6":
                resDesc = "Invalid Channel";
                break;   
            case "7":
                resDesc = "Invalid Network Type";
                break;
            case "8":
                resDesc = "Invalid Biller";
                break;
            case "9":
                resDesc = "Invalid Source System Router";
                break;
            case "10":
                resDesc = "Invalid Token";
                break;
            case "11":
                resDesc = "Token Time Out";
                break;
            case "12":
                resDesc = "System Malfunction";
                break;
            case "13":
                resDesc = "Hash Balance Not Equal";
                break;
            case "14":
                resDesc = "Channel Discount Rate Not Found";
                break;
                 case "15":
                resDesc = "Mobile Not Found";
                break;
                 case "16":
                resDesc = "Invalid Amount";
                break;
                 case "17":
                resDesc = "Channel Ref ID Not Found";
                break;
        }
        return resDesc;
         
       
    }
   
    public static string getErrorRes(string code,string desp, string taxId="")
    {
        StringBuilder sberror = new StringBuilder();
        writeLog("Util getErrorRes code : " + code + " desp : " + desp + " taxId : " + taxId);
        sberror.Append("<Error>");
        sberror.Append("<Version>1.0</Version>");
        sberror.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sberror.Append("<ResCode>" + code + "</ResCode>");
        sberror.Append("<ResDesc>" + getErrorMessage(code, desp) + "</ResDesc>");
        sberror.Append("<TaxId>" + taxId + "</TaxId>");
        sberror.Append("</Error>");
        writeLog("RES XML : " + sberror.ToString());
        return sberror.ToString();
    }

    public static string getErrorMessage(string errCode, string errMsg)
    {
        if (errCode.Equals("99") || errCode.Equals("97") || string.IsNullOrEmpty(errMsg))
        {
            return getCustomErrorMessage();
        }
        else 
        {
            var unknownErrorMsg = ConfigurationManager.AppSettings["UnknownErrorMessages"].Split(';').Where(x => x.Equals(errMsg)).FirstOrDefault();
            bool IsunexceptedErrorMsg = Regex.IsMatch(errMsg.ToLower(), @"\bunexpected\b");
            bool IsInternalErrorMsg = Regex.IsMatch(errMsg.ToLower(), @"\binternal error\b");
            if (!string.IsNullOrEmpty(unknownErrorMsg) || IsunexceptedErrorMsg || IsInternalErrorMsg)
            {
                return getCustomErrorMessage();
            }
            return errMsg;

        }
    }
    public static string getCustomErrorMessage()
    {
        var customErrorMessage = ConfigurationManager.AppSettings["CustomErrorMessage"].ToString();
        return customErrorMessage;
    }

    public static string getCardPaymentErrorRes(string code, ConfirmResponseModel confirmRes)
    {
        StringBuilder sberror = new StringBuilder();
        sberror.Append("<Error>");
        sberror.Append("<Version>1.0</Version>");
        sberror.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");// 
        sberror.Append("<Email>" + confirmRes.email + "</Email>");
        sberror.Append("<Password>" + confirmRes.password + "</Password>");
        sberror.Append("<MessageID>" + confirmRes.messageid + "</MessageID>");
        sberror.Append("<ResCode>" + code + "</ResCode>");
        sberror.Append("<ResDesc>" + confirmRes.resdesc + "</ResDesc>");
        sberror.Append("<Ref1>" + confirmRes.ref1 + "</Ref1>");
        sberror.Append("<Ref2>" + confirmRes.ref2 + "</Ref2>");
        sberror.Append("<Ref3>" + confirmRes.ref3 + "</Ref3>");
        sberror.Append("<Ref4>" + confirmRes.ref4 + "</Ref4>");
        sberror.Append("<Ref5>" + confirmRes.ref5 + "</Ref5>");
        sberror.Append("<Ref6>" + confirmRes.ref6 + "</Ref6>");
        sberror.Append("<Ref1Name>" + confirmRes.ref1Name + "</Ref1Name>");
        sberror.Append("<Ref2Name>" + confirmRes.ref2Name + "</Ref2Name>");
        sberror.Append("<Ref3Name>" + confirmRes.ref3Name + "</Ref3Name>");
        sberror.Append("<Ref4Name>" + confirmRes.ref4Name + "</Ref4Name>");
        sberror.Append("<Ref5Name>" + confirmRes.ref5Name + "</Ref5Name>");
        sberror.Append("<TxnID>" + confirmRes.txnID + "</TxnID>");
        sberror.Append("<SMS>" + confirmRes.smsMsg + "</SMS>");
        sberror.Append("<IsVoidable>" + confirmRes.isVoidable + "</IsVoidable>");
        sberror.Append("</Error>");

        writeLog("RES XML : " + sberror.ToString());
        return sberror.ToString();
    }



    #endregion
    #region TitanSource Utilities

    /// <summary>
    /// Request web service
    /// </summary>
    /// <param name="completeUrl"></param>
    /// <returns></returns>
    public static Stream CallTitanSourceApi(string completeUrl, ref HttpWebResponse response)
    {
        Uri uri = new Uri(completeUrl);

        // Create a request for the URL.         
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.AbsoluteUri);

        // If required by the server, set the credentials.
        request.Credentials = CredentialCache.DefaultCredentials;
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        // If you have a proxy configured.
        WebProxy proxyObject = new WebProxy();
        request.Proxy = proxyObject;
        request.Timeout = 180000;
        request.Accept = @"text/html, application/xhtml+xml, */*";
        request.Headers.Add("Accept-Language", "en-GB");
        request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";

        //Get the response.
        response = (HttpWebResponse)request.GetResponse();

       
        // Get the stream containing content returned by the server.
        Stream dataStream = response.GetResponseStream();
      
        return dataStream;
    }

    /// <summary>
    /// To Encrypt Parameters
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="Passphrase"></param>
    /// <returns></returns>
    public static string EncryptString(string Message, string Passphrase)
    {
        byte[] Results;

        System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

        MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
        byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

        TDESKey = GetKeyAs24Bytes(TDESKey);

        TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

        TDESAlgorithm.Key = TDESKey;
        TDESAlgorithm.Mode = CipherMode.ECB;
        TDESAlgorithm.Padding = PaddingMode.PKCS7;

        byte[] DataToEncrypt = UTF8.GetBytes(Message);

        try
        {
            ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
            Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
        }
        finally
        {
            TDESAlgorithm.Clear();
            HashProvider.Clear();
        }

        string encryptedKey = Convert.ToBase64String(Results);

        encryptedKey = SafeEncrypt(encryptedKey);

        return encryptedKey;
    }

    /// <summary>
    /// Important !
    /// </summary>
    /// <param name="encryptedKey"></param>
    /// <returns></returns>
    public static string SafeEncrypt(string encryptedKey)
    {
        string x = encryptedKey;
        x = x.Replace('+', '_');
        x = x.Replace('/', '-');
        x = x.Replace('=', '$');
        return x;
    }

    public static byte[] GetKeyAs24Bytes(byte[] key)
    {
        byte[] keyBytes = new byte[24]; // a Triple DES key is a byte[24] array

        for (int i = 0; i < key.Length && i < keyBytes.Length; i++)
            keyBytes[i] = (byte)key[i];

        return keyBytes;
    }

    public static TitanInquiryResultSet MapTitanInquiryResponse(string responseXML)
    {
       
        writeLog("Mapping Titan Inquiry Response");
        writeLog("Respone stream:" + responseXML.ToString());
        XmlDocument doc = new XmlDocument();
        try
        {
            writeLog("start Parsing XML");
            doc.LoadXml(responseXML);
        }
        catch(Exception ex)
        {
            writeLog("Exception error in parsing xml:" + ex.ToString());
        }
        writeLog("Titan Response:"+doc.InnerXml.ToString());
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
        nsmgr.AddNamespace("x", "http://schemas.datacontract.org/2004/07/YESBService.Common");

        TitanInquiryResultSet result = new TitanInquiryResultSet();
        result.AccountNo = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:AccountNo", nsmgr).InnerText;
        result.ConsumerReferenceNo = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:ConsumerReferenceNo", nsmgr).InnerText;
        result.Amount = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:Amount", nsmgr).InnerText;
        result.BankName = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:BankName", nsmgr).InnerText;
        result.BillNo = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:BillNo", nsmgr).InnerText;
        result.ConsumerName = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:ConsumerName", nsmgr).InnerText;
        result.DueDate = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:DueDate", nsmgr).InnerText;
        result.MeterNumber = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:MeterNumber", nsmgr).InnerText;
        result.MonthName = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:MonthName", nsmgr).InnerText;
        result.Status = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:Status", nsmgr).InnerText;
        result.TotalUnitUsed = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:TotalUnitUsed", nsmgr).InnerText;
        result.TownshipCode = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:TownshipCode", nsmgr).InnerText;
        result.TownshipName = doc.DocumentElement.SelectSingleNode("//x:ResultSet/x:TownshipName", nsmgr).InnerText;
       
       

        return result;
    }


    public static TitanConfirmResultSet  MapTitanConfirmResponse(Stream confirmResponseStream)
    {
        TitanConfirmResultSet confirmresData=new TitanConfirmResultSet();
        /*Get the reference Number here.*/

                    XmlDocument doc = new XmlDocument();
                    //Load stream into xml document.
                    doc.Load(confirmResponseStream);

                    XmlNamespaceManager namespaceMgr = new XmlNamespaceManager(doc.NameTable);
                    namespaceMgr.AddNamespace("x", "http://schemas.microsoft.com/2003/10/Serialization/");

                   confirmresData.ResponseInfo  = doc.DocumentElement.SelectSingleNode("//x:string", namespaceMgr).InnerText;

                   confirmResponseStream.Close();
                   confirmResponseStream.Dispose();
                   return confirmresData;
    }
    #endregion

    #region Rent2Own
    public class CertPolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest request, int problem)
        {
            return true;
        }
    }
    public static string Rent2OwnRequest(string json, string uri)
    {
        string useURL = string.Format(uri);

        string result = string.Empty;
        ServicePointManager.CertificatePolicy = new CertPolicy();
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(useURL);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {


            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            result = streamReader.ReadToEnd();
        }
        return result;
    }


    #endregion

    #region offLinePayment
    public static string offlinePayRequest(string json, string method)
    {
        string url = ConfigurationSettings.AppSettings["offlineAPIUrl"].ToString();

        //string postData = Utility.PKCS7Encrypt("123456789012");
        string postData = "";
        string errDetails = "";
        postData = json;//Encryption.ClientEncryptDecryptString(jsonReq, "E", out errDetails);

        string useURL = url + method;

        //Create an instance of the WebRequest class
        WebRequest objRequest = (HttpWebRequest)WebRequest.Create(useURL);
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        objRequest.Timeout = 2 * 60000; //In milliseconds - in this case 60 seconds
        objRequest.Method = "POST";
        objRequest.ContentLength = postData.Length;
        objRequest.ContentType = "application/json";
        //objRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

        //Create an instance of the StreamWriter class and attach the WebRequest object to it - here's where we do the posting 
        StreamWriter postWriter = new StreamWriter(objRequest.GetRequestStream());
        postWriter.Write(postData);
        postWriter.Close();

        //Create an instance of the WebResponse class and get the output to the rawOutput string
        WebResponse objResponse = objRequest.GetResponse();
        StreamReader sr = new StreamReader(objResponse.GetResponseStream());
        string rawOutput = sr.ReadToEnd();

        sr.Close();

        //HttpContext.Current.Session["LastResponseTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        return rawOutput;



    }

    public static OfflinePaymentInquiryResp DecryptOfflinePaymentResponse(OfflinePaymentInquiryResp inqres)
    {
        writeLog("Start Encrypt");
        string OutputStr = string.Empty;

        string PublicKey = ConfigurationSettings.AppSettings["offlinePublicCert"].ToString();
        string PrivateKey = ConfigurationSettings.AppSettings["offlinePrivateCert"].ToString();
        string PrivatePWD = ConfigurationSettings.AppSettings["offlinePass"].ToString();

        try
        {
            SinaptIQPKCS7.PKCS7 PKCS7 = new SinaptIQPKCS7.PKCS7();


            string decryptResCode = PKCS7.decryptMessage(inqres.resCode, PKCS7.getPrivateCert(PrivateKey, PrivatePWD));
            string decryptResDesc = PKCS7.decryptMessage(inqres.resDesc.ToString(), PKCS7.getPrivateCert(PrivateKey, PrivatePWD));

            inqres.resCode = decryptResCode;
            inqres.resDesc = decryptResDesc;





        }
        catch (Exception ex)
        {
            writeLog("error in Decrypting offline response" + ex.Message);
            // Log.Error("", "ClientEncryptDecryptString (err) : " + ex.ToString());
        }

        return inqres;
    }


    #endregion

    #region
    #region SHA256 for 663
    public static string GenerateSHA256String(string inputString)
    {
        SHA256 sha256 = SHA256Managed.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(inputString);
        byte[] hash = sha256.ComputeHash(bytes);
        return GetStringFromHash(hash);
    }
    private static string GetStringFromHash(byte[] hash)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            result.Append(hash[i].ToString("X2"));
        }
        return result.ToString();
    }
    #endregion
    #endregion    


    public static string getTelenorBBList(TelenorBBInquiryResModel inqueryRes)
    {
        StringBuilder sbLog = new StringBuilder();
        StringBuilder sb = new StringBuilder();

        sb.Append("<TelenorBBInquiryRes>");
        sb.Append("<Version>1.0</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sb.Append("<MerchantName>" + inqueryRes.merchantname + "</MerchantName>");
        sb.Append("<MerchantLogo>" + inqueryRes.merchantlogo + "</MerchantLogo>");
        sb.Append("<BillerName>" + inqueryRes.billername + "</BillerName>");
        sb.Append("<BillerLogo>" + inqueryRes.billerlogo + "</BillerLogo>");
        sb.Append("<Ref1>" + inqueryRes.ref1 + "</Ref1>");
        sb.Append("<Ref2>" + inqueryRes.ref2 + "</Ref2>");
        sb.Append("<Ref3>" + inqueryRes.ref3 + "</Ref3>");
        sb.Append("<Ref4>" + inqueryRes.ref4 + "</Ref4>");
        sb.Append("<Ref5>" + inqueryRes.ref5 + "</Ref5>");
        sb.Append("<Ref1Name>" + inqueryRes.ref1Name + "</Ref1Name>");
        sb.Append("<Ref2Name>" + inqueryRes.ref2Name + "</Ref2Name>");
        sb.Append("<Ref3Name>" + inqueryRes.ref3Name + "</Ref3Name>");
        sb.Append("<Ref4Name>" + inqueryRes.ref4Name + "</Ref4Name>");
        sb.Append("<Ref5Name>" + inqueryRes.ref5Name + "</Ref5Name>");
        sb.Append("<CPES>");
        foreach (CPE CPE in inqueryRes.ResponseCPEList)
        {
            sb.Append("<CPE>");
            sb.Append("<IMEI>" + CPE.IMEI + "</IMEI>");
            sb.Append("<ExpiredOn>" + CPE.ExpiredOn + "</ExpiredOn>");
            sb.Append("</CPE>");
        }

        sb.Append("</CPES>");
        sb.Append("<Amount>" + inqueryRes.amount + "</Amount>");
        sb.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");

        sb.Append("<Status>" + inqueryRes.status + "</Status>");
        sb.Append("<Expiry>" + inqueryRes.expiry + "</Expiry>");
        sb.Append("<ProductDesc>" + inqueryRes.productDescription + "</ProductDesc>");
        sb.Append("<ImageURL>" + inqueryRes.imgUrl + "</ImageURL>");
        sb.Append("</TelenorBBInquiryRes>");
        return sb.ToString();
    }

    public static string GetFtthOrWtthInquiryList(FtthOrWtthInquiryResponse inqueryRes)
    {
        StringBuilder sbLog = new StringBuilder();
        StringBuilder sb = new StringBuilder();

        sb.Append("<FtthOrWtthInquiryRes>");
        sb.Append("<Version>" + inqueryRes.version + "</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sb.Append("<MerchantName>" + inqueryRes.merchantname + "</MerchantName>");
        sb.Append("<MerchantLogo>" + inqueryRes.merchantlogo + "</MerchantLogo>");
        sb.Append("<BillerName>" + inqueryRes.billername + "</BillerName>");
        sb.Append("<BillerLogo>" + inqueryRes.billerlogo + "</BillerLogo>");
        sb.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");
        sb.Append("<AgentPercentFee>" + inqueryRes.servicePercentFee + "</AgentPercentFee>");
        sb.Append("<RegisteredMobielNo>" + inqueryRes.RegisteredMobielNo + "</RegisteredMobielNo>");
        sb.Append("<InquiryDetails>");
        sb.Append("<CustomerID>" + inqueryRes.InquiryDetails.CustomerID + "</CustomerID>");
        sb.Append("<CustomerName>" + ReplaceAmpersandString(inqueryRes.InquiryDetails.CustomerName) + "</CustomerName>");
        sb.Append("<PlanType>" + inqueryRes.InquiryDetails.PlanType + "</PlanType>");
        sb.Append("<Plans>");
        foreach (var plan in inqueryRes.InquiryDetails.Packages)
        {
            sb.Append("<Plan>");
            sb.Append("<Devices>" + plan.devices + "</Devices>");
            sb.Append("<ExpiredOn>" + plan.expired_on + "</ExpiredOn>");
            sb.Append("<Packs>");
            foreach (var pack in plan.Packs)
            {
                sb.Append("<Pack>");
                sb.Append("<Price>" + pack.price + "</Price>");
                sb.Append("<Description>" + pack.description + "</Description>");
                sb.Append("<ProductId>" + pack.product_id + "</ProductId>");
                sb.Append("</Pack>");
            }
            sb.Append("</Packs>");
            sb.Append("</Plan>");
        }
        sb.Append("</Plans>");
        sb.Append("</InquiryDetails>");
        sb.Append("</FtthOrWtthInquiryRes>");

        writeLog("APN inquiry response for FTTHORWTTX :" + sb.ToString());
        return sb.ToString();
    }

    public static string GetMptDataPackageInquiryList(MptDataPackageInquiryResponse inqueryRes)
    {
        var sb = new StringBuilder();

        sb.Append("<MptPackageInquiryRes>");
        sb.Append("<Version>" + inqueryRes.version + "</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sb.Append("<BillerName>" + inqueryRes.billername + "</BillerName>");
        sb.Append("<BillerLogo></BillerLogo>");
        sb.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");
        sb.Append("<AgentPercentFee>" + inqueryRes.servicePercentFee + "</AgentPercentFee>");
        sb.Append("<InquiryDetails>");
        sb.Append("<TransactionStatus>" + inqueryRes.PackageDetails.TransactionStatus + "</TransactionStatus>");
        sb.Append("<ErrorCode>" + inqueryRes.PackageDetails.ErrorCode + "</ErrorCode>");
        sb.Append("<ErrorMessage>" + inqueryRes.PackageDetails.ErrorMessage + "</ErrorMessage>");
        sb.Append("<PartnerAmount>" + inqueryRes.PackageDetails.PartnerAmount + "</PartnerAmount>");
        sb.Append("<TransactionAmount>" + inqueryRes.PackageDetails.TransactionAmount + "</TransactionAmount>");
        
        sb.Append("<DataPacks>");
        foreach (var plan in inqueryRes.PackageLists)
        {
            sb.Append("<DataPack>");
            sb.Append("<packageCode>" + plan.packageCode + "</packageCode>");
            sb.Append("<packageName>" + plan.packageName + "</packageName>");
            sb.Append("<price>" + plan.price + "</price>");
            sb.Append("</DataPack>");
        }
        sb.Append("</DataPacks>");
        sb.Append("</InquiryDetails>");
        sb.Append("</MptPackageInquiryRes>");

        writeLog("APN inquiry response for Mpt Data Package :" + sb.ToString());

        sb.Replace("<BillerLogo></BillerLogo>", "<BillerLogo>" + inqueryRes.billerlogo + "</BillerLogo>");
        return sb.ToString();
    }

    public static string GetMyTelDataPackageInquiryList(MptDataPackageInquiryResponse inqueryRes)
    {
        var sb = new StringBuilder();

        sb.Append("<MyTelPackageInquiryRes>");
        sb.Append("<Version>" + inqueryRes.version + "</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sb.Append("<BillerName>" + inqueryRes.billername + "</BillerName>");
        sb.Append("<BillerLogo></BillerLogo>");
        sb.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");
        sb.Append("<AgentPercentFee>" + inqueryRes.servicePercentFee + "</AgentPercentFee>");
        sb.Append("<InquiryDetails>");
        sb.Append("<TransactionStatus>" + inqueryRes.PackageDetails.TransactionStatus + "</TransactionStatus>");
        sb.Append("<ErrorCode>" + inqueryRes.PackageDetails.ErrorCode + "</ErrorCode>");
        sb.Append("<ErrorMessage>" + inqueryRes.PackageDetails.ErrorMessage + "</ErrorMessage>");
        sb.Append("<PartnerAmount>" + inqueryRes.PackageDetails.PartnerAmount + "</PartnerAmount>");
        sb.Append("<TransactionAmount>" + inqueryRes.PackageDetails.TransactionAmount + "</TransactionAmount>");

        sb.Append("<DataPacks>");
        foreach (var plan in inqueryRes.PackageLists)
        {
            sb.Append("<DataPack>");
            sb.Append("<packageName>" + plan.packageName + "</packageName>");
            sb.Append("<price>" + plan.price + "</price>");
            sb.Append("</DataPack>");
        }
        sb.Append("</DataPacks>");
        sb.Append("</InquiryDetails>");
        sb.Append("</MyTelPackageInquiryRes>");

        writeLog("APN inquiry response for MyTel Data Package :" + sb.ToString());
         sb.Replace("<BillerLogo></BillerLogo>", "<BillerLogo>" + inqueryRes.billerlogo + "</BillerLogo>");
        return sb.ToString();
    }

    public static string EventInquiry(EventListResponse inqueryRes)
    {
        var sb = new StringBuilder();

        sb.Append("<EventInquiryRes>");
        sb.Append("<Version>" + inqueryRes.version + "</Version>");
        sb.Append("<TimeStamp>" + DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sb.Append("<BillerName>" + inqueryRes.billername + "</BillerName>");
        sb.Append("<BillerLogo>" + inqueryRes.billerlogo + "</BillerLogo>");
        sb.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");
        sb.Append("<AgentPercentFee>" + inqueryRes.servicePercentFee + "</AgentPercentFee>");
        sb.Append("<TicketList>");

        foreach (var ticket in inqueryRes.TicketList)
        {
            sb.Append("<Ticket>");
            sb.Append("<TicketId>" + ticket.ticketId + "</TicketId>");
            sb.Append("<EventTitle>" + ticket.eventTitle + "</EventTitle>");
            sb.Append("<EventId>" + ticket.eventId + "</EventId>");
            sb.Append("<TicketType>" + ticket.ticketType + "</TicketType>");
            sb.Append("<TicketDescription>" + ticket.ticketDescription + "</TicketDescription>");
            sb.Append("<AvailableTicketQty>" + ticket.availableTicketQty + "</AvailableTicketQty>");
            sb.Append("<MinimumTicketQty>" + ticket.minimumTicketQty + "</MinimumTicketQty>");
            sb.Append("<MaximumTicketQty>" + ticket.maximumTicketQty + "</MaximumTicketQty>");
            sb.Append("<Price>" + ticket.price + "</Price>");
            sb.Append("<SaleEndDate>" + ticket.saleEndDate + "</SaleEndDate>");
            sb.Append("</Ticket>");
        }
        sb.Append("</TicketList>");
        sb.Append("</EventInquiryRes>");

        writeLog("APN inquiry response for MyanPwel :" + sb.ToString());
        return sb.ToString();
    }

    public static string GetOoredooDataPackageInquiryList(OoredooDataPackageInquiryResponse inqueryRes)
    {
        var sb = new StringBuilder();

        sb.Append("<OoredooPackageInquiryRes>");
        sb.Append("<Version>" + inqueryRes.version + "</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sb.Append("<BillerName>" + inqueryRes.billername + "</BillerName>");
        sb.Append("<BillerLogo></BillerLogo>");
        sb.Append("<AgentFee>" + inqueryRes.serviceFee + "</AgentFee>");
        sb.Append("<AgentPercentFee>" + inqueryRes.servicePercentFee + "</AgentPercentFee>");
        sb.Append("<InquiryDetails>");
        sb.Append("<TransactionStatus>" + inqueryRes.PackageDetails.TransactionStatus + "</TransactionStatus>");
        sb.Append("<ErrorCode>" + inqueryRes.PackageDetails.ErrorCode + "</ErrorCode>");
        sb.Append("<ErrorMessage>" + inqueryRes.PackageDetails.ErrorMessage + "</ErrorMessage>");
        sb.Append("<PartnerAmount>" + inqueryRes.PackageDetails.PartnerAmount + "</PartnerAmount>");
        sb.Append("<TransactionAmount>" + inqueryRes.PackageDetails.TransactionAmount + "</TransactionAmount>");

        sb.Append("<DataPacks>");
        foreach (var plan in inqueryRes.PackageLists)
        {
            sb.Append("<DataPack>");
            sb.Append("<offerId>" + plan.offerID + "</offerId>");
            sb.Append("<offerName>" + plan.offerName + "</offerName>");
            sb.Append("<validity>" + plan.validity + "</validity>");
            sb.Append("<price>" + plan.price + "</price>");
            sb.Append("</DataPack>");
        }
        sb.Append("</DataPacks>");
        sb.Append("</InquiryDetails>");
        sb.Append("</OoredooPackageInquiryRes>");

        writeLog("APN inquiry response for Ooredoo Data Package :" + sb.ToString());
        sb.Replace("<BillerLogo></BillerLogo>", "<BillerLogo>" + inqueryRes.billerlogo + "</BillerLogo>");
        return sb.ToString();
    }

    public static string GetParamiGasPackageList(ParamiGasInquiryResponse inqueryRes)
    {
        StringBuilder sbLog = new StringBuilder();
        StringBuilder sb = new StringBuilder();

        sb.Append("<ParamiGasInquiryRes>");
        sb.Append("<Version>" + inqueryRes.version + "</Version>");
        sb.Append("<TimeStamp>" + System.DateTime.Now.ToString("yyyyMMddhhmmssffff") + "</TimeStamp>");
        sb.Append("<ResCode>" + inqueryRes.ResCode + "</ResCode>");
        sb.Append("<ResDesc>" + inqueryRes.ResDesc + "</ResDesc>");
        sb.Append("<TaxID>" + inqueryRes.taxID + "</TaxID>");
        sb.Append("<BillerName>" + inqueryRes.billername + "</BillerName>");
        sb.Append("<BillerLogo>" + inqueryRes.billerlogo + "</BillerLogo>");
        sb.Append("<serviceFlatFee>" + inqueryRes.serviceFlatFee + "</serviceFlatFee>");
        sb.Append("<servicePercentFee>" + inqueryRes.servicePercentFee + "</servicePercentFee>");
        sb.Append("<Package>");
        foreach (var plan in inqueryRes.PackageDetails)
        {
            sb.Append("<DataPack>");
            sb.Append("<packageCode>" + plan.packageCode + "</packageCode>");
            sb.Append("<price>" + plan.price + "</price>");
            sb.Append("</DataPack>");
        }
        sb.Append("</Package>");
        sb.Append("</ParamiGasInquiryRes>");

        writeLog("APN inquiry response for ParamiGas :" + sb.ToString());
        return sb.ToString();
    }

   
    public static string maskString(string sourceString)
    {
        string result= "XX" + sourceString.Substring(2);
        return result;
    }

    public static string PostEba(string json, string url)
    {
        var result = string.Empty;

        try
        {
            var requestTimeout = ConfigurationManager.AppSettings["EBARequestTimeOut"].ToString();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(requestTimeout);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            writeLog("Exception occur when request to EBA:" + ex.Message);
            result = string.Empty;
        }

        return result;
    }

    public static string ReplaceAmpersandString(string str)
    {
        if (str.Contains("&"))
        {
            str = str.Replace("&", "&amp;");
        }

        return str;
    }

    public static bool ValidateAmountLimitation(string name, decimal amount, out string code, out string desc)
    {
        writeLog(string.Format("Start Validate Amount Limitation. TaxId/PaymentType: {0}, Amount: {1}.", name, amount)); 
        code = string.Empty;
        desc = string.Empty;
        bool isValid = true;
        string minAmount = string.Empty, maxAmount= string.Empty;
        if (name == "A+ Wallet")
        {
            minAmount = ConfigurationManager.AppSettings[string.Format("{0}MinAmount", "APlusWallet")];
            maxAmount = ConfigurationManager.AppSettings[string.Format("{0}MaxAmount", "APlusWallet")];
        }
        else
        {
            minAmount = ConfigurationManager.AppSettings[string.Format("{0}MinAmount", name)];
            maxAmount = ConfigurationManager.AppSettings[string.Format("{0}MaxAmount", name)];
        }
        
        if (!string.IsNullOrEmpty(minAmount) && !string.IsNullOrEmpty(maxAmount))
        {
            if (amount < Convert.ToDecimal(minAmount))
            {
                isValid = false;
                code = "06";
                desc = string.Format("Invalid Amount. Your amount must be over {0} Ks.", minAmount);
            }
            else if (amount > Convert.ToDecimal(maxAmount))
            {
                isValid = false;
                code = "06";
                desc = string.Format("Invalid Amount. Your amount must be under {0} Ks.", maxAmount);
            }
        }
        else
        {
            writeLog("Min Amount and Max Amount does not exist"); 
        }

        writeLog(string.Format("End Validate Amount Limitation. IsValidAmount: {0}.", isValid));
        return isValid;
    }

    public static string ReplaceSpecialCharacters(string str)
    {
        return str.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
    }



    #region <-- Log -->
    public static void WriteLog_Biller(string msg)
    {
        string maskSensitiveString = maskSensitiveData(msg);
        if (!string.IsNullOrEmpty(maskSensitiveString))
        {
            Logger.writeLog(maskSensitiveString, ref log);
        }
        else
        {
            Logger.writeLog(msg, ref log);
        }
    }
    private static string maskSensitiveData(string value)
    {
        string regularExpressionPattern = @"<Password>(.*?)<\/Password>";
        Regex regex = new Regex(regularExpressionPattern, RegexOptions.Singleline);
        MatchCollection collection = regex.Matches(value);
        if (collection.Count > 0)
        {
            Match m = collection[0];
            var stripped = m.Groups[1].Value;
            if (!string.IsNullOrEmpty(stripped))
            {
                return value.Replace(stripped, "XXXX-XXXX-XXXX");
            }

        }
        return "";
    }

    #endregion



    #region  Request Detail Dynamic
    public static JObject Get_DynamicJsonReq(DynamicInfo dynamicInfo, string postText)
    {
        JObject _JsonReq = new JObject();
        try
        {
            var billerReqDetail = ConfigurationManager.AppSettings[dynamicInfo.TaxId + postText].ToString().Split(',');


            foreach (string item in billerReqDetail)
            {
                string[] _data = item.Split(':');
                if (_data.Count() == 2)
                {
                    var property = dynamicInfo.GetType().GetProperties().Where(x => string.Equals(x.Name.ToLower().Trim(), _data[1].ToLower().Trim())).Select(x => x.Name).FirstOrDefault();

                    object detailValue = null;
                    if (!string.IsNullOrEmpty(property))
                    {
                        detailValue = dynamicInfo.GetType().GetProperty(property).GetValue(dynamicInfo, null);

                    }
                    _JsonReq.Add(_data[0].Trim(), detailValue == null ? string.Empty : detailValue.ToString());

                }
            }
        }
        catch (Exception ex)
        {
            _JsonReq = null;

        }
        //string _billerconfirmReqDetail = dynamicInfo.TaxId + "ConfirmReqDetail";

        return _JsonReq;
    }

    #endregion
    #region  Marketplace Post
    public static string PostMarketplace(string json, string url)
    {
        var result = string.Empty;

        try
        {
            var requestTimeout = ConfigurationManager.AppSettings["EBARequestTimeOut"].ToString();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(requestTimeout);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {

            writeLog("Exception occur when request to EBA:" + ex.Message);
            result = string.Empty;
        }

        return result;
    }
    #endregion
}