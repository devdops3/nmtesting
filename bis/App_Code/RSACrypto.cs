using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Configuration;

public class RSACrypto
    {
        public static String EncryptData(String originalData)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(originalData);

            var rsaCsp = readPublicKeyFromFile(ConfigurationManager.AppSettings["MyKyatpublicKey"].ToString());

            byte[] ciphertext = rsaCsp.Encrypt(dataToEncrypt, false);

            string returnValue = Convert.ToBase64String(ciphertext);

            return ConvertStringToHex(returnValue);
            
        }
        public static string ConvertStringToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += string.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }
        public static String DecryptData(String encryptedData, string checkSum)
        {
            string return_check_sum_value = calculateCheckSum(encryptedData, ConfigurationManager.AppSettings["checkSumKey"].ToString());
            if (return_check_sum_value == checkSum)
            {
                
                 //Hex to String
                string data_string = HexString2Ascii(encryptedData).Replace("Ú", "");
                 //Base 64 decode
                byte[] decodedResult = decodeFrom64(data_string);

                string key = readPrivateKeyFromFile(ConfigurationManager.AppSettings["MyKyatprivateKey"].ToString());

                var rsaCsp = CreateRsaProviderFromPrivateKey(key);

                byte[] ciphertext = rsaCsp.Decrypt(decodedResult, false);
               
                string returnValue = Encoding.UTF8.GetString(ciphertext);

                return returnValue;
            }
            return string.Empty;
        }
        static private byte[] decodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
            = Convert.FromBase64String(encodedData);

            return encodedDataAsBytes;
        }
        private static string HexString2Ascii(string hexString)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= hexString.Length - 2; i += 2)
            {
                sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber))));
            }
            return sb.ToString();
        }
        public static RSACryptoServiceProvider readPublicKeyFromFile(String fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                byte[] data = new byte[fs.Length];
                byte[] res = null;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPem("PUBLIC KEY", data);
                }
                try
                {
                    RSACryptoServiceProvider rsa = DecodeX509PublicKey(res);
                    return rsa;
                }
                catch 
                {
                    return null;
                }
            }
        }
        private static RSACryptoServiceProvider CreateRsaProviderFromPrivateKey(string privateKey)
        {
            var privateKeyBits = System.Convert.FromBase64String(privateKey);

            var RSA = new RSACryptoServiceProvider();
            var RSAparams = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                RSAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            RSA.ImportParameters(RSAparams);
            return RSA;
        }
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
                {
                    highbyte = binr.ReadByte();
                    lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;
                }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }
        private static byte[] GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = String.Format("-----BEGIN {0}-----\\n", type);
            string footer = String.Format("-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));
            return Convert.FromBase64String(base64);
        }
        public static RSACryptoServiceProvider DecodeX509PublicKey(byte[] x509key)
        {
            byte[] SeqOID = { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 };
            MemoryStream ms = new MemoryStream(x509key);
            BinaryReader reader = new BinaryReader(ms);
            if (reader.ReadByte() == 0x30)
                ReadASNLength(reader); //skip the size
            else
                return null;
        int identifierSize;
        if (reader.ReadByte() == 0x30)
                identifierSize = ReadASNLength(reader);
            else
                return null;
            if (reader.ReadByte() == 0x06) //is the next element an object identifier?
            {
                int oidLength = ReadASNLength(reader);
                byte[] oidBytes = new byte[oidLength];
                reader.Read(oidBytes, 0, oidBytes.Length);
                if (!oidBytes.SequenceEqual(SeqOID)) //is the object identifier rsaEncryption PKCS#1?
                    return null;
                int remainingBytes = identifierSize - 2 - oidBytes.Length;
                reader.ReadBytes(remainingBytes);
            }
            if (reader.ReadByte() == 0x03) //is the next element a bit string?
            {
                ReadASNLength(reader); //skip the size
                reader.ReadByte(); //skip unused bits indicator
                if (reader.ReadByte() == 0x30)
                {
                    ReadASNLength(reader); //skip the size
                    if (reader.ReadByte() == 0x02) //is it an integer?
                    {
                        int modulusSize = ReadASNLength(reader);
                        byte[] modulus = new byte[modulusSize];
                        reader.Read(modulus, 0, modulus.Length);
                        if (modulus[0] == 0x00) //strip off the first byte if it's 0
                        {
                            byte[] tempModulus = new byte[modulus.Length - 1];
                            Array.Copy(modulus, 1, tempModulus, 0, modulus.Length - 1);
                            modulus = tempModulus;
                        }
                        if (reader.ReadByte() == 0x02) //is it an integer?
                        {
                            int exponentSize = ReadASNLength(reader);
                            byte[] exponent = new byte[exponentSize];
                            reader.Read(exponent, 0, exponent.Length);
                            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                            RSAParameters RSAKeyInfo = new RSAParameters();
                            RSAKeyInfo.Modulus = modulus;
                            RSAKeyInfo.Exponent = exponent;
                            RSA.ImportParameters(RSAKeyInfo);
                            return RSA;
                        }
                    }
                }
            }
            return null;
        }
        public static int ReadASNLength(BinaryReader reader)
        {
            //Note: this method only reads lengths up to 4 bytes long as
            //this is satisfactory for the majority of situations.
            int length = reader.ReadByte();
            if ((length & 0x00000080) == 0x00000080) //is the length greater than 1 byte
            {
                int count = length & 0x0000000f;
                byte[] lengthBytes = new byte[4];
                reader.Read(lengthBytes, 4 - count, count);
                Array.Reverse(lengthBytes); //
                length = BitConverter.ToInt32(lengthBytes, 0);
            }
            return length;
        }
        public static string readPrivateKeyFromFile(String fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                byte[] data = new byte[fs.Length];
                string res = string.Empty;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPemForPrivate("PRIVATE KEY", data);
                }
                try
                {

                    return res;
                }
                catch 
                {
                    return null;
                }
            }
        }
        private static string GetPemForPrivate(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = string.Format("-----BEGIN {0}-----\\n", type);
            string footer = string.Format("-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));
            return base64;
        }
        #region Check Sum        
        public static string calculateCheckSum(string mydata, string mykey)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            string secretKey = mykey;
            string content = mydata;
            byte[] secretKeyBArr = Encoding.UTF8.GetBytes(secretKey);
            byte[] contentBArr = Encoding.UTF8.GetBytes(content);
            hmacsha1.Key = secretKeyBArr;
            byte[] final = hmacsha1.ComputeHash(contentBArr);
            string result = BytesToHexString(final).ToLower();
            return result;
        }
        public static string BytesToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int t = 0; t < data.Length; t++)
            {
                sb.AppendFormat("{0:X2}", data[t]);
            }
            return sb.ToString();
        }
        #endregion
    }
