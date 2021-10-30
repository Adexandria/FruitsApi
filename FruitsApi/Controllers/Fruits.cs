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
using FruitsApi.Controller;




// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FruitsApi.Controllers
{
    [Route("api/fruits")]
    public class Fruits : ControllerBase
    {
        // Add your Computer Vision subscription key and endpoint to your environment variables. 
        // Close/reopen your project for them to take effect.
        readonly TextToString textToString;
        public Fruits(TextToString textToString)
        {
            this.textToString = textToString;
        }
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
            try
            {
                if (file == null)
                {
                    return this.StatusCode(StatusCodes.Status404NotFound, "file not found");
                }
                var fruitName = await textToString.Post(file);
                if (fruitName == null)
                {
                    return BadRequest("Fruit not found");
                }
                var fruitProperties = await GetProperties(fruitName);
                return Ok(fruitProperties);
            }
            catch (Exception e)
            {

               return BadRequest(e.Message);
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
     
      
         
    }
}
