using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;

public class SaveAndLoadFunc : MonoBehaviour
{
    const int TOKEN_LEN = 16;

    public void WriteFile(string path, string content, string key)
    {
        using (FileStream outfs = new FileStream(Path.Combine(path, "data.bin"), FileMode.Create))
        using (BinaryWriter binw = new BinaryWriter(outfs))
        using (StringReader strr = new StringReader(content))
        using (AesManaged aesManaged = new AesManaged())
        {
            aesManaged.BlockSize = 128;
            aesManaged.KeySize = 128;
            aesManaged.Mode = CipherMode.CBC;
            aesManaged.Padding = PaddingMode.PKCS7;

            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, 16);
            byte[] salt = new byte[16];
            salt = rfc2898DeriveBytes.Salt;

            byte[] bufkey = rfc2898DeriveBytes.GetBytes(16);
            aesManaged.Key = bufkey;

            XorShift xorShift = new XorShift((System.UInt32)System.Environment.TickCount);
            System.UInt32 rawtoken = xorShift.GetRand();
            byte[] token = new byte[TOKEN_LEN];

            token[0] = (byte)((rawtoken >> 24) & 0b0000_0000_0000_0000_0000_0000_1111_1111);
            token[1] = (byte)((rawtoken >> 16) & 0b0000_0000_0000_0000_0000_0000_1111_1111);
            token[2] = (byte)((rawtoken >> 8) & 0b0000_0000_0000_0000_0000_0000_1111_1111);
            token[3] = (byte)((rawtoken) & 0b0000_0000_0000_0000_0000_0000_1111_1111);

            aesManaged.GenerateIV();

            binw.Write(salt, 0, 16);
            binw.Write(aesManaged.IV, 0, 16);
            binw.Write(token, 0, TOKEN_LEN);

            System.Text.Encoder encoder = System.Text.Encoding.ASCII.GetEncoder();
            byte[] bbuf = new byte[1024];
            char[] cbuf = new char[1024];
            using (ICryptoTransform crypto = aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV))
            using (CryptoStream cryptos = new CryptoStream(outfs, crypto, CryptoStreamMode.Write))
            {
                cryptos.Write(token, 0, TOKEN_LEN);

                int len;
                int bused, cused;
                bool comp;
                int pos = 0;
                while ((len = strr.ReadBlock(cbuf, 0, cbuf.Length)) > 0)
                {
                    encoder.Convert(cbuf, 0, len, bbuf, 0, len, false, out cused, out bused, out comp);
                    cryptos.Write(bbuf, 0, bused);
                }
                cryptos.FlushFinalBlock();
            }
        }
    }


    public string ReadFile(string path, string key)
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        using (FileStream infs = new FileStream(Path.Combine(path, "data.bin"), FileMode.Open))
        using (StringWriter strw = new StringWriter(stringBuilder))
        using (AesManaged aesManaged = new AesManaged())
        {
            aesManaged.BlockSize = 128;
            aesManaged.KeySize = 128;
            aesManaged.Mode = CipherMode.CBC;
            aesManaged.Padding = PaddingMode.PKCS7;

            byte[] salt = new byte[16];
            infs.Read(salt, 0, 16);
            byte[] iv = new byte[16];
            infs.Read(iv, 0, 16);
            byte[] trueTokenB = new byte[TOKEN_LEN];
            infs.Read(trueTokenB, 0, TOKEN_LEN);
            System.UInt32 trueToken =
                (((System.UInt32)trueTokenB[0]) << 24) +
                (((System.UInt32)trueTokenB[1]) << 16) +
                (((System.UInt32)trueTokenB[2]) << 8) +
                (((System.UInt32)trueTokenB[3]));

            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, salt);
            byte[] keyBuf = rfc2898DeriveBytes.GetBytes(16);
            aesManaged.Key = keyBuf;
            aesManaged.IV = iv;

            int len = 0;
            byte[] bbuf = new byte[4096];
            char[] cbuf = new char[4096];
            using (ICryptoTransform decrypter = aesManaged.CreateDecryptor(aesManaged.Key, aesManaged.IV))
            {
                using (CryptoStream cryptos = new CryptoStream(infs, decrypter, CryptoStreamMode.Read))
                {
                    byte[] decryptedTokenB = new byte[16];
                    cryptos.Read(decryptedTokenB, 0, decryptedTokenB.Length);
                    System.UInt32 decryptedToken =
                        (((System.UInt32)decryptedTokenB[0]) << 24) +
                        (((System.UInt32)decryptedTokenB[1]) << 16) +
                        (((System.UInt32)decryptedTokenB[2]) << 8) +
                        (((System.UInt32)decryptedTokenB[3]));

                    if (trueToken != decryptedToken)
                    {
                        return "Exception will occurs.";
                    }

                    System.Text.Decoder decoder = System.Text.Encoding.ASCII.GetDecoder();
                    while ((len = cryptos.Read(bbuf, 0, bbuf.Length)) > 0)
                    {
                        int bused, cused;
                        bool comp;
                        decoder.Convert(bbuf, 0, len, cbuf, 0, len, false, out bused, out cused, out comp);
                        strw.Write(cbuf, 0, cused);
                    }
                }
            }
        }
        return stringBuilder.ToString();
    }
}

public class XorShift
{
    System.UInt32 x;
    System.UInt32 y;
    System.UInt32 z;
    System.UInt32 w;

    public XorShift(System.UInt32 seed)
    {
        w = seed;
        x = w << 13;
        y = (w >> 9) ^ (x << 6);
        z = y >> 7;
    }

    public System.UInt32 GetRand()
    {
        System.UInt32 t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        return w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
    }
}