using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections;
/// <summary>
/// Summary description for RouteAPI
/// </summary>
public class RouteAPI
{
    public RouteAPI()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    string onetwothreeapilink = ConfigurationManager.AppSettings["123APILink"].ToString();
    string oneCashapilink = ConfigurationManager.AppSettings["OneCashAPILink"].ToString();
    string putetapiLink = ConfigurationManager.AppSettings["PutetAPILink"].ToString();
    string easyPointapilink = ConfigurationManager.AppSettings["easyPointsUrl"].ToString();
    string easyPointRedeemLink = ConfigurationManager.AppSettings["easyPointRedeemUrl"].ToString();
    private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private void writeLog(string msg)
    {
        Logger.writeLog(msg, ref log);
    }
    public string Apicalling123(string xml)
    {

        string errMsg = string.Empty;
        string responsedata = string.Empty;
        string result = null;

        string encode = zCompress(xml);

        SSLPost sslPost = new SSLPost();

        if (!sslPost.post2Biller(onetwothreeapilink, encode, out responsedata, out errMsg))
            writeLog("Error in post2Biller : " + errMsg);
        responsedata = this.zDecompress(responsedata);

       
        result = System.Xml.Linq.XDocument.Parse(responsedata).ToString();
       

        return result;
    }
    public string Apicalling123Confirm(string xml, string messageId, long txnId)
    {

        string errMsg = string.Empty;
        string responsedata = string.Empty;
        string result = null;

        string encode = zCompress(xml);

        var sslPost123 = new SSLPost123();

        if (!sslPost123.post2Biller(onetwothreeapilink, messageId, txnId, encode, out responsedata, out errMsg))
            writeLog("Error in post2Biller : " + errMsg);
        responsedata = this.zDecompress(responsedata);


        result = System.Xml.Linq.XDocument.Parse(responsedata).ToString();


        return result;
    }
    public string ApicallingOneCash(string xml)
    {

        string errMsg = string.Empty;
        string responsedata = string.Empty;
        string result = null;

        string encode = zCompress(xml);

        SSLPost sslPost = new SSLPost();

        sslPost.post2Biller(oneCashapilink, encode, out responsedata, out errMsg);
        responsedata = this.zDecompress(responsedata);


        result = System.Xml.Linq.XDocument.Parse(responsedata).ToString();

        return result;
    }

    public string ApicallingPutet(string xml)
    {

        string errMsg = string.Empty;
        string responsedata = string.Empty;
        string result = null;

        SSLPost sslPost = new SSLPost();

        sslPost.post2Putet(putetapiLink, xml, out responsedata, out errMsg);
        result = System.Xml.Linq.XDocument.Parse(responsedata).ToString();

        return result;
    }


    public string ApicallingEasypoint(string xml)
    {

        string errMsg = string.Empty;
        string responsedata = string.Empty;
        string result = null;

        SSLPost sslPost = new SSLPost();

        sslPost.post2EasyPoint(easyPointapilink, xml, out responsedata, out errMsg);
        result = System.Xml.Linq.XDocument.Parse(responsedata).ToString();

        return result;
    }

    public string RedeemApicallingEasypoint(string xml)
    {

        string errMsg = string.Empty;
        string responsedata = string.Empty;
        string result = null;
        SSLPost sslPost = new SSLPost();

        sslPost.post2EasyPoint(easyPointRedeemLink, xml, out responsedata, out errMsg);
        writeLog("EasyPoints Redeem Response:"+responsedata);
        result = System.Xml.Linq.XDocument.Parse(responsedata).ToString();

        return result;
    }

    public string ApicallingMyKyat(string url, string signatureString,int type,string authKey)
    {
        string MyKyatAPI = ConfigurationManager.AppSettings["MyKyatAPI"].ToString();
        // 1.Encrypt 
        string encryptData = RSACrypto.EncryptData(signatureString.ToString());

        writeLog("MyKyat  Req-encryptData: " + encryptData);
        // Check Sum
        string checkSum = RSACrypto.calculateCheckSum(encryptData, ConfigurationManager.AppSettings["checkSumKey"].ToString());

        writeLog("MyKyat  Req-CheckSum : " + checkSum);
        // Json Request
        JObject json = new JObject();

        if (type == 0)
        {
            json = new JObject();
            json.Add("appId", ConfigurationManager.AppSettings["appId"].ToString());
            json.Add("terminalId", ConfigurationManager.AppSettings["terminalId"].ToString());
            json.Add("providerId", ConfigurationManager.AppSettings["providerId"].ToString());
            json.Add("data", encryptData);
            json.Add("checkSum", checkSum);
        }
        else
        {
            json = new JObject();
            json.Add("authKey", authKey);
            json.Add("terminalId", ConfigurationManager.AppSettings["terminalId"].ToString());
            json.Add("providerId", ConfigurationManager.AppSettings["providerId"].ToString());
            json.Add("data", encryptData);
            json.Add("checkSum", checkSum);
        }            
      

        writeLog("MyKyat  Req-json : " + json.ToString());

        SSLPost sslPost = new SSLPost();
        string responseMsg = string.Empty;
        string errMsg = string.Empty;
        string decryptData = string.Empty;

        writeLog("URL:" + MyKyatAPI + url + json.ToString());

        if (sslPost.postDataJson(MyKyatAPI + url, json.ToString(), out responseMsg, out errMsg))
        {
            writeLog("responseMsg : " + responseMsg);

            writeLog("responseMsg : " + errMsg);

            Hashtable hashtable = Utils.ConvertJSONtoHashTable(responseMsg);

            if (hashtable.ContainsKey("checkSum"))
            {
                checkSum = hashtable["checkSum"].ToString();
            }

            if (hashtable.ContainsKey("data"))
            {
                encryptData = hashtable["data"].ToString();
            }

            writeLog("MyKyat  Res-encrypteddata : " + encryptData);

            writeLog("MyKyat  Res-checkSum : " + checkSum);

            decryptData = RSACrypto.DecryptData(encryptData, checkSum);

            writeLog("MyKyat  Res-decryptData : " + decryptData);
        }

        writeLog("responseMsg : " + responseMsg);

        writeLog("responseMsg : " + errMsg);

        return decryptData;
    }

    #region Base64 zCompress and zDecompress
    public string zCompress(string str)
    {
        try
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            Deflater dfl = new Deflater();

            Stream s = new DeflaterOutputStream(ms, dfl);
            s.Write(b, 0, b.Length);
            s.Flush();
            s.Close();
            byte[] c = (byte[])ms.ToArray();
            return Convert.ToBase64String(c);
        }
        catch
        {
            return null;
        }
    }

    public string zDecompress(string str)
    {
        byte[] b = Convert.FromBase64String(str);
        string ret = "";
        int l = 0;
        byte[] w = new byte[1024];
        Stream s = new InflaterInputStream(new MemoryStream(b));

        try
        {
            while (true)
            {
                int i = s.Read(w, 0, w.Length);
                if (i > 0)
                {
                    l += i;
                    ret += Encoding.ASCII.GetString(w, 0, i);
                }
                else
                {
                    break;
                }
            }
            s.Flush();
            s.Close();

            return ret;
        }
        catch
        {
            return null;
        }
    }
    #endregion
}