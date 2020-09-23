using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        string uriBase = endpoint + "vision/v3.0/analyze";

        /*
        * AUTHENTICATE
        * Creates a Computer Vision client used by each example.
        */
        [SwaggerOperation("PostImage")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
     
        [HttpPost("PostImage")]
        public async Task<IActionResult> Post(IFormFile file)
        {       
                 if(file == null)
            {
                return this.StatusCode(StatusCodes.Status404NotFound, "file not found");
            }
            string details = await MakeAnalysisRequest(file.FileName);
            var fruitName = ProcessAnalysisResult(details);
            var fruitProperties = await GetProperties(fruitName);
            return Ok(fruitProperties);
           
            
        }       
        [NonAction]
        private async Task<string> MakeAnalysisRequest(string image)
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
        [NonAction]
        private async Task<string> GetProperties(string name)
        {
            HttpClient client = new HttpClient();
            string uri = "https://www.fruityvice.com";
            string requestParameter = $"/api/fruit/{name}";
            string uriBase = uri + requestParameter;
            HttpResponseMessage httpResponse;
            httpResponse = await client.GetAsync(uriBase);
            string contentString = await httpResponse.Content.ReadAsStringAsync();
            var newContent = JToken.Parse(contentString).ToString();
            return newContent;
        }
        private byte[] GetImageAsByteArray(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName,FileMode.Open,FileAccess.Read))
            {
                BinaryReader binary = new BinaryReader(fileStream);
                return binary.ReadBytes((int)fileStream.Length);

            }
        }
        private static string ProcessAnalysisResult(string result)
        { 
            var resultSplit = result.Split(',', '\r', '\n', '\"', '{', '}', ' ', ':', '[', ']');
            var newResults =resultSplit.Skip(20).Take(40).ToArray();
            var newName = newResults.ElementAt(10);
            if (newName == "fruit")
            {
            var newresult = resultSplit.Skip(70).Take(20).ToArray();
            var name = newresult.ElementAt(17);
            if(name == "")
            {
                for (int i = 1; i < newresult.Length - 1;)
                {
                    if (newresult[i] == "")
                    {
                        i++;
                    }
                    else
                    {
                        return newresult[i];
                    }
                }
            }
            return name;
                 
              
            }

            return newName;
        }
      
         
    }
}
