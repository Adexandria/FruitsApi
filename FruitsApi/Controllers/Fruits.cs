using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;




// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FruitsApi.Controllers
{
    [Route("api/fruits")]
    public class Fruits : ControllerBase
    {
        // Add your Computer Vision subscription key and endpoint to your environment variables. 
        // Close/reopen your project for them to take effect.
        static string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
        static string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");
        public static IHostingEnvironment _environment;
        string uriBase = endpoint + "vision/v3.0/analyze";
        public Fruits(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        /*
        * AUTHENTICATE
        * Creates a Computer Vision client used by each example.
        */
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] IFormFile file)
        {
          var details = await MakeAnalysisRequest(file.FileName);
            return Ok(details);
        }
        public async Task<string> MakeAnalysisRequest(string image)
        { 
          
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                 string requestParameters = "visualFeatures=Tags";
                string uri = uriBase + "?" + requestParameters;
                HttpResponseMessage httpResponse;
                byte[] bytedata = GetImageAsByteArray(image);
                using (ByteArrayContent content = new ByteArrayContent(bytedata))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponse = await client.PostAsync(uri, content);
                }
                string contentString = await httpResponse.Content.ReadAsStringAsync();
               var newContent= JToken.Parse(contentString).ToString();
                return newContent;
              
                
            }
            catch (Exception e)
            {

                return e.Message;
            }
        }

        private byte[] GetImageAsByteArray(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName,FileMode.Open,FileAccess.Read))
            {
                BinaryReader binary = new BinaryReader(fileStream);
                return binary.ReadBytes((int)fileStream.Length);

            }
        }

         
    }
}
