using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Celia.io.Core.StaticObjects.WebAPI_Core.Controllers
{
    public class ImageTestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> SaveAsync(IEnumerable<IFormFile> files)
        {
            try
            {
                // The Name of the Upload component is "files"
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length <= 0)
                            continue;
                        //var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                        //// Some browsers send file names with full path.
                        //// We are only interested in the file name.
                        //var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                        // var physicalPath = Path.Combine(HostingEnvironment.WebRootPath, "App_Data", fileName);

                        // The files are not actually saved in this demo
                        //using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                        //{
                        //    await file.CopyToAsync(fileStream);
                        //}
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://localhost:52190");

                        using (var content = new MultipartFormDataContent())
                        {
                            //public IFormFile FormFile { get; set; } 
                            //public string ObjectId { get; set; } 
                            //[Required]
                            //public string StorageId { get; set; } 
                            //public string FilePath { get; set; } 
                            //public string Extension { get; set; }

                            content.Add(new StreamContent(file.OpenReadStream())
                            {
                                Headers =
                                {
                                    ContentLength = file.Length,
                                    ContentType = new MediaTypeHeaderValue(file.ContentType)
                                }
                            }
                            , "FormFile", file.FileName);
                            //content.Add(new StringContent(Guid.NewGuid().ToString()), "ObjectId");
                            //content.Add(new StringContent("test"), "StorageId");

                            client.DefaultRequestHeaders.Add("appId", "br.com");
                            client.DefaultRequestHeaders.Add("appSecret", "79faf82271944fe38c4f1d99be71bc9c");
                            var response = await client.PostAsync(
                                "/api/Images/uploadimg?storageId=bzgsoft-internal", content);

                            if(response.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                throw new Exception($"{response.StatusCode}  {response.ReasonPhrase}");
                            } 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            // Return an empty string to signify success
            return Content("");
        }
    }
}
