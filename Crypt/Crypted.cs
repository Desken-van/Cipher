using System;
using System.IO;
using System.Security.Cryptography;

namespace Crypt
{
    public class Crypted
    {
        static byte[] IV = new byte[] { 54, 224, 148, 107, 99, 29, 171, 219 };
        static byte[] Key = new byte[] { 227, 192, 107, 97, 130, 19, 122, 45, 172, 13, 149, 138, 249, 119, 163, 68, 179, 40, 149, 172, 160, 10, 90, 130 };

        public static void EncryptFile(string inputFile, string outputFile)
        {
            using (TripleDES tdes = TripleDESCryptoServiceProvider.Create())
            {
                tdes.IV = IV;
                tdes.Key = Key;
                using (var inputStream = File.OpenRead(inputFile))
                using (var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var encStream = new CryptoStream(outputStream, tdes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    outputStream.SetLength(0);
                    inputStream.CopyTo(encStream);
                }
            }
        }
        public static void DecryptFile(string inputFile, string outputFile)
        {
            using (TripleDES tdes = TripleDESCryptoServiceProvider.Create())
            {
                tdes.IV = IV;
                tdes.Key = Key;
                using (var inputStream = File.OpenRead(inputFile))
                using (var decStream = new CryptoStream(inputStream, tdes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    decStream.CopyTo(outputStream);
                }
            }
        }
    }   
}
