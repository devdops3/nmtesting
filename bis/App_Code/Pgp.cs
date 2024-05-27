using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;

public class Pgp
{
    public static PgpPublicKey ReadPublicKey(Stream inputStream)
    {
        inputStream = PgpUtilities.GetDecoderStream(inputStream);
        PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(inputStream);
        foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
        {
            foreach (PgpPublicKey k in kRing.GetPublicKeys())
            {
                if (k.IsEncryptionKey)
                    return k;
            }
        }

        throw new ArgumentException("Can't find encryption key in key ring.");
    }

    private static PgpPrivateKey FindSecretKey(PgpSecretKeyRingBundle pgpSec, long keyId, char[] pass)
    {
        PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyId);
        if (pgpSecKey == null)
            return null;

        return pgpSecKey.ExtractPrivateKey(pass);
    }

    public static byte[] Decrypt(byte[] inputData, Stream keyIn, string passCode)
    {
        byte[] error = Encoding.ASCII.GetBytes("ERROR");

        Stream inputStream = new MemoryStream(inputData);
        inputStream = PgpUtilities.GetDecoderStream(inputStream);
        MemoryStream decoded = new MemoryStream();

        try
        {
            PgpObjectFactory pgpF = new PgpObjectFactory(inputStream);
            PgpEncryptedDataList enc;
            PgpObject o = pgpF.NextPgpObject();


            if (o is PgpEncryptedDataList)
                enc = (PgpEncryptedDataList)o;
            else
                enc = (PgpEncryptedDataList)pgpF.NextPgpObject();


            PgpPrivateKey sKey = null;
            PgpPublicKeyEncryptedData pbe = null;
            PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(
                PgpUtilities.GetDecoderStream(keyIn));
            foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
            {
                sKey = FindSecretKey(pgpSec, pked.KeyId, passCode.ToCharArray());
                if (sKey != null)
                {
                    pbe = pked;
                    break;
                }
            }
            if (sKey == null)
                throw new ArgumentException("secret key for message not found.");

            Stream clear = pbe.GetDataStream(sKey);
            PgpObjectFactory plainFact = new PgpObjectFactory(clear);
            PgpObject message = plainFact.NextPgpObject();

            if (message is PgpCompressedData)
            {
                PgpCompressedData cData = (PgpCompressedData)message;
                PgpObjectFactory pgpFact = new PgpObjectFactory(cData.GetDataStream());
                message = pgpFact.NextPgpObject();
            }
            if (message is PgpLiteralData)
            {
                PgpLiteralData ld = (PgpLiteralData)message;
                Stream unc = ld.GetInputStream();
                PipeAll(unc, decoded);
            }
            else if (message is PgpOnePassSignatureList)
                throw new PgpException("encrypted message contains a signed message - not literal data.");
            else
                throw new PgpException("message is not a simple encrypted file - type unknown.");



            return decoded.ToArray();
        }
        catch 
        {
            return error;
        }
    }


    public static byte[] Encrypt(byte[] inputData, PgpPublicKey passPhrase, bool withIntegrityCheck, bool armor)
    {
        byte[] processedData = Compress(inputData, PgpLiteralData.Console, CompressionAlgorithmTag.Uncompressed);

        MemoryStream bOut = new MemoryStream();
        Stream output = bOut;

        if (armor)
            output = new ArmoredOutputStream(output);

        PgpEncryptedDataGenerator encGen = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());
        encGen.AddMethod(passPhrase);

        Stream encOut = encGen.Open(output, processedData.Length);

        encOut.Write(processedData, 0, processedData.Length);
        encOut.Close();

        if (armor)
            output.Close();

        return bOut.ToArray();
    }

    public byte[] Encrypt(byte[] inputData, byte[] publicKey)
    {
        Stream publicKeyStream = new MemoryStream(publicKey);

        PgpPublicKey encKey = ReadPublicKey(publicKeyStream);

        return Encrypt(inputData, encKey, true, true);
    }

    private static byte[] Compress(byte[] clearData, string fileName, CompressionAlgorithmTag algorithm)
    {
        MemoryStream bOut = new MemoryStream();

        PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(algorithm);
        Stream cos = comData.Open(bOut);
        PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
        Stream pOut = lData.Open(
            cos,                    // the compressed output stream
            PgpLiteralData.Binary,
            fileName,               // "filename" to store
            clearData.Length,       // length of clear data
            DateTime.UtcNow         // current time
            );

        pOut.Write(clearData, 0, clearData.Length);
        pOut.Close();

        comData.Close();

        return bOut.ToArray();
    }

    private const int BufferSize = 512;

    public static void PipeAll(Stream inStr, Stream outStr)
    {
        byte[] bs = new byte[BufferSize];
        int numRead;
        while ((numRead = inStr.Read(bs, 0, bs.Length)) > 0)
        {
            outStr.Write(bs, 0, numRead);
        }
    }



}