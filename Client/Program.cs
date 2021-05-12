using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Crypt;

namespace Client
{
    public class Program
    {       
        static async Task Main(string[] args)
        {
            //D:\Labs\Serv\Text.txt
            Console.WriteLine("Введите путь к файлу: ");
            string path = Convert.ToString(Console.ReadLine());
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                string cryptppath = fileInf.DirectoryName +"\\"+"Crypted" + fileInf.Extension;
                string uncryptppath = fileInf.DirectoryName + "\\" + "FromServerCrypt.Zip";
                string decryptppath = fileInf.DirectoryName + "\\" + "FromServerDeCrypted.Zip";
                Crypted.EncryptFile(path,cryptppath);
                HttpClient httpClient = new HttpClient();
                var content = new MultipartFormDataContent();
                using (var fstream = File.OpenRead(cryptppath))
                {
                    var streamContent = new StreamContent(fstream);
                    streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file"
                        ,
                        FileName = Path.GetFileName(cryptppath),
                    };
                    streamContent.Headers.Add("type",fileInf.Extension);
                    content.Add(streamContent);
                    HttpResponseMessage response = await httpClient.PostAsync("https://localhost:44308/GetFile", content);                
                    response.EnsureSuccessStatusCode();
                    httpClient.Dispose();
                    string sd = await response.Content.ReadAsStringAsync();
                    using (FileStream stream = new FileStream(uncryptppath, FileMode.OpenOrCreate))
                    {
                        byte[] array = await response.Content.ReadAsByteArrayAsync();
                        stream.Write(array, 0, array.Length);
                    }                   
                }
                FileInfo Delete = new FileInfo(cryptppath);
                Delete.Delete();
                Crypted.DecryptFile(uncryptppath, decryptppath);
                Delete = new FileInfo(uncryptppath);
                Delete.Delete();
                string file = "UncryptFile" + fileInf.Extension;
                string cig = "SignedFile.cig";
                using (var zip = ZipFile.Open(decryptppath, ZipArchiveMode.Update))
                {
                    var entfile = zip.GetEntry(file);
                    var entcig = zip.GetEntry(cig);
                    var tempFile = Path.GetTempFileName();
                    var tempCig = Path.GetTempFileName();
                    entfile.ExtractToFile(tempFile, true);
                    entcig.ExtractToFile(tempCig, true);
                    byte[] hash = File.ReadAllBytes(tempFile);
                    byte[] signhash = File.ReadAllBytes(tempCig);
                    Console.WriteLine();
                    Console.WriteLine(SignDoc.Verify(hash, signhash));
                }
            }
            Console.ReadKey();
        }       
     }
}
