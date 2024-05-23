using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Linq;
using System.Text;
using System.Configuration;
/// <summary>
/// Summary description for TripleDES
/// </summary>
public class TripleDES
{

    private byte[] cipherText = null;
    private BufferedBlockCipher cipher = null;
    private readonly string finalKey = ConfigurationManager.AppSettings["3DESFinalKey"].ToString(); 


    public String get3DESEncryptedMessage1(String clearText, String md5_1, String md5_2)
    {
       byte[] almostCompletedMessageByte = hexToBytes(
                stringToHex("  " + clearText + " ")
                        + (md5_1 != null && !md5_1.Trim().Equals("", StringComparison.InvariantCultureIgnoreCase) ? md5_1 : "")
               + (md5_2 != null && !md5_2.Trim().Equals("", StringComparison.InvariantCultureIgnoreCase) ? stringToHex(" ") + md5_2 : ""));

        byte[] CompleteMessageByte = almostCompletedMessageByte;

        cipherText = performEncrypt(this.hexToBytes(finalKey), CompleteMessageByte);
        String ctS = Encoding.ASCII.GetString(Hex.Encode(cipherText));

        return (ctS);

    }

    public String get3DESEncryptedMessage1(String clearText, String md5_1)
    {

        return this.get3DESEncryptedMessage1(clearText, md5_1, null);
    }

    public byte[] generateMD5(byte[] input)
    {
        var md5 = new MD5Digest();

        md5.BlockUpdate(input, 0, input.Length);
        byte[] digest = new byte[md5.GetDigestSize()];
        md5.DoFinal(digest, 0);
        return digest;
    }

    private byte[] performEncrypt(byte[] key, byte[] ptBytes)
    {
        cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(getEngineInstance()));

        cipher.Init(true, new KeyParameter(key));

        byte[] rv = new byte[cipher.GetOutputSize(ptBytes.Length)];
        Console.WriteLine("Key Cipher  " + rv);

        int oLen = cipher.ProcessBytes(ptBytes, 0, ptBytes.Length, rv, 0);
        try
        {
            cipher.DoFinal(rv, oLen);
        }
        catch (CryptoException ce)
        {

            status(ce.ToString());
        }
        return rv;
    }

    private int whichCipher()
    {
        return 1; 
    }

    private IBlockCipher getEngineInstance()
    {
        // returns a block cipher according to the current
        // state of the radio button lists. This is only
        // done prior to encryption.
        IBlockCipher rv = null;

        switch (whichCipher())
        {
            case 0:
                rv = new DesEngine();
                break;
            case 1:
                rv = new DesEdeEngine();
                break;
            case 2:
                rv = new IdeaEngine();
                break;
            case 3:
                rv = new RijndaelEngine();
                break;
            case 4:
                rv = new TwofishEngine();
                break;
            default:
                rv = new DesEngine();
                break;
        }
        return rv;
    }

    public void message(String s)
    {
        Console.WriteLine("M:" + s);
    }

    public void status(String s)
    {
        Console.WriteLine("S:" + s);
    }

    public byte[] convertStringToByteArray(String stringToConvert)
    {


        byte[] theByteArray = Encoding.ASCII.GetBytes(stringToConvert);

        Console.WriteLine(theByteArray.Length);
        return theByteArray;

    }

    public String convertByteArrayToString(byte[] byteArray)
    {
        String value = Encoding.ASCII.GetString(byteArray);

        Console.WriteLine(value);
        return value;
    }


    public byte[] hexToBytes(String hex)
    {
        string hexString = hex.ToUpper();

        return Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                .ToArray();
    }

    public string bytesToHex(byte[] ba)
    {
        string hex = BitConverter.ToString(ba);
        return hex.Replace("-", "");
    }

    public String stringToHex(String str)
    {
        char b;
        StringBuilder output = new StringBuilder();
        char[] chars = new char[str.Length];
        for (int i = 0; i < chars.Length; i++)
        {
            b = str[i];
            int value = Convert.ToInt32(b);
            // Convert the decimal value to a hexadecimal value in string form.
            string hexOutput = String.Format("{0:X}", value);
            output.Append(hexOutput);
        }
        return output.ToString().ToUpper();
    }

    public String doDESEncryption(String inputString)
    {
        DesEdeEngine engine = new DesEdeEngine();
        BufferedBlockCipher myCipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));

        byte[] key = this.hexToBytes(finalKey);
        byte[] input = Encoding.ASCII.GetBytes(inputString);

        myCipher.Init(true, new KeyParameter(key));

        byte[] cipherText = new byte[myCipher.GetOutputSize(input.Length)];

        int outputLen = myCipher.ProcessBytes(input, 0, input.Length, cipherText, 0);
        try
        {
            myCipher.DoFinal(cipherText, outputLen);
        }
        catch (CryptoException ce)
        {
            Console.WriteLine(ce.Message);
            return null;
        }

        byte[] cipherBytes = Hex.Encode(cipherText);
        return Encoding.ASCII.GetString(cipherBytes).ToUpper();

    }


}