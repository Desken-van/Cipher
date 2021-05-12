using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Crypt
{
    public class SignDoc
    {
        public static byte[] Sign(byte[] file)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Find(X509FindType.FindBySubjectName, "localhost", true).Cast<X509Certificate2>().FirstOrDefault();
            RSA csp = null;
            csp = (RSA)cert.PrivateKey;
            if (csp == null)
            {
                throw new Exception("No valid cert was found");
            }
            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] hash = sha1.ComputeHash(file);
            store.Close();
            return csp.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pss);
        }
        public static bool Verify(byte[] file, byte[] signature)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Find(X509FindType.FindBySubjectName, "localhost", true).Cast<X509Certificate2>().FirstOrDefault();
            RSA csp = (RSA)cert.PublicKey.Key;
            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] hash = sha1.ComputeHash(file);
            string textFromFile = Encoding.Default.GetString(hash);
            Console.WriteLine(textFromFile);
            Console.WriteLine();
            textFromFile = Encoding.Default.GetString(signature);
            Console.WriteLine(textFromFile);
            Console.WriteLine();
            return csp.VerifyHash(hash, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pss);
        }
        public static void WriteFile(string path, byte[] fileContent)
        {
            File.WriteAllBytes(path, fileContent);
        }
    }
}
