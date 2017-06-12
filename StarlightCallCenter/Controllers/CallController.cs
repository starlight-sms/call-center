using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http;
using System.IO;
using Amazon.Lex;
using Amazon;
using Amazon.Lex.Model;

namespace StarlightCallCenter.Controllers
{
    public class CallController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Response.ContentType = "text/xml";
            base.OnActionExecuting(context);
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Record(string recordingUrl, int recordingDuration, string digits)
        {
            var response = await lexClient.PostContentAsync(new PostContentRequest
            {
                InputStream = await DownloadUrl(recordingUrl), 
                Accept = "audio/*", 
                BotName = "StarlightCallCenter", 
                BotAlias = "Prod", 
                ContentType = "audio/l16; rate=8000; channels=1", 
                UserId = "ZhouJie", 
            });
            var filename = "/Record/" + GetNewRandomFileName();
            using (var file = System.IO.File.Create("wwwroot" + filename))
            {
                response.AudioStream.CopyTo(file);
            }
            await Task.Delay(1000);
            ViewBag.FileName = filename;
            return View();
        }

        public static HttpClient hc = new HttpClient();
        public static AmazonLexClient lexClient = new AmazonLexClient(RegionEndpoint.USEast1);

        private static string GetNewRandomFileName()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "_")
                .Replace("/", "-")
                + ".mp3";
        }

        private static async Task<Stream> DownloadUrl(string url)
        {
            var response = await hc.GetAsync(url);
            return await response.Content.ReadAsStreamAsync();
        }

        private static async Task<string> GetLocalRecordUrl(string recordingUrl)
        {
            var stream = await DownloadUrl(recordingUrl);

            var filename = "/Record/" + GetNewRandomFileName();
            using (var file = System.IO.File.Create("wwwroot" + filename))
            {
                stream.CopyTo(file);
            }

            return filename;
        }
    }
}
