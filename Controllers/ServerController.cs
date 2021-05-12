using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Crypt;
using System.Security.Cryptography;
using Ionic.Zip;

namespace Server.Controllers
{
    [ApiController]
    public class ServerController : ControllerBase
    {
        [HttpPost("/GetFile")]
        public IActionResult GetFile(IFormFile file)
        {
            string path = @"D:\Labs\Serv\Server\Upload";
            string filePath = Path.Combine(path, file.FileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyToAsync(fileStream);               
            }
            string type = file.Headers.Values.Last();
            string root = @"D:\Labs\Serv\Server\Upload\UncryptFile" + type;
            string sign = @"D:\Labs\Serv\Server\Upload\SignedFile.cig";
            string cryptppath = @"D:\Labs\Serv\Server\Upload\CryptZip.zip";
            string zipstr = @"D:\Labs\Serv\Server\Upload\ZIP.zip";
            Crypted.DecryptFile(filePath, root);
            FileInfo Delete = new FileInfo(filePath);
            Delete.Delete();
            FileStream stream = new FileStream(root, FileMode.Open, FileAccess.Read);
            using (stream)
            {
                byte[] array = new byte[stream.Length];                
                stream.Read(array, 0, array.Length);
                byte[] signed = SignDoc.Sign(array);
                SignDoc.WriteFile(sign, signed);     
            }
            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(path+"\\");
                zip.Save(zipstr);
            }
            Crypted.EncryptFile(zipstr,cryptppath);
            byte[] result;
            FileStream rstream = new FileStream(cryptppath, FileMode.Open, FileAccess.Read);
            using (rstream)
            {
                byte[] array = new byte[rstream.Length];
                rstream.Read(array, 0, array.Length);
                Delete = new FileInfo(root);
                Delete.Delete();
                Delete = new FileInfo(sign);
                Delete.Delete();
                Delete = new FileInfo(zipstr);
                Delete.Delete();
                result = array;      
            }
            Delete = new FileInfo(cryptppath);
            Delete.Delete();
            return File(result, "application/zip", "CryptZip.zip");
        }      
    }
}

