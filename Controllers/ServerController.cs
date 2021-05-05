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

namespace Server.Controllers
{
    [ApiController]
    public class ServerController : ControllerBase
    {
        [HttpPost("/GetFile")]
        public IActionResult GetFile(IFormFile file)
        {
            string path = @"S:\Work\Serv\Server\Upload";
            string filePath = Path.Combine(path, file.FileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyToAsync(fileStream);               
            }
            string type = file.Headers.Values.Last();
            string root = @"S:\Work\Serv\Server\Upload\Uncrypt" + type;
            Crypted.DecryptFile(filePath, root);
            FileInfo Delete = new FileInfo(filePath);
            Delete.Delete();
            FileStream stream = new FileStream(root, FileMode.Open, FileAccess.Read);
            using (stream)
            {
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, array.Length);
                return File(array, "text/plain", file.FileName);
            }
        }
    }
}

