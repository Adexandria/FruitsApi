using FruitsApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Text_Speech.Services;

namespace FruitsApi.Controller
{
    public class TextToString
    {
        private readonly IBlob blob;

        readonly IConfiguration configuration;


        public TextToString(IBlob blob, IConfiguration configuration)
        {
            this.blob = blob;
            this.configuration = configuration;
        }
      
        [NonAction]
        public async Task<string> Post(IFormFile image)
        {
            try
            {
                if (image == null)
                {
                    return "file not found";
                }
                

                await blob.Upload(image);
                var downloadedfile = await blob.Download(image);

                string analyzedDetails = await MakeAnalysisRequest(downloadedfile);
                Analysis analysis = JsonConvert.DeserializeObject<Analysis>(analyzedDetails);
                var fruitname = await GetFruitName(analysis);
                return fruitname;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        // To process the data on the images and retrieve the texts in string
        [NonAction]
        private async Task<string> MakeAnalysisRequest(byte[] file)
        {
            try
            {
                var client = GetClient();
                string requestParameters = "visualFeatures=Tags";
                string uri = configuration["Endpoint"] + "vision/v3.2/analyze" + "?" + requestParameters;
                HttpResponseMessage httpResponse;
                using (ByteArrayContent Bytecontent = new ByteArrayContent(file))
                {
                    Bytecontent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponse = await client.PostAsync(uri, Bytecontent);
                }
                string responseContent = await httpResponse.Content.ReadAsStringAsync();
                var stringContent = JToken.Parse(responseContent).ToString();
                return stringContent;
            }
            catch (Exception e)
            {
                
                return e.Message;
            }
        }
      
        [NonAction]
        private HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            var key = configuration["SubscriptionKey"];
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key",key );
            return client;
        }

        [NonAction]
        private async Task<string> GetFruitName(Analysis analysis)
        {
            var names = analysis.Tags.TakeWhile(s => s.Confidence >= 0.90).Select(s=>s.Name).ToList();
            var content = await GetAllFruits();
            IEnumerable <Description> fruits = JsonConvert.DeserializeObject<IEnumerable<Description>>(content);
            var fruitNames = fruits.Select(s => s.Name).ToList();
            int left = 0;
            int right = names.Count;
            while(left < right)
            {
                var name = names[left];
                if (name == "grape")
                {
                    name = "grapes";
                }
                if (fruitNames.Contains(name,StringComparer.OrdinalIgnoreCase))
                {
                    return name;
                }
                left++;
            } 
            return null;
        }

        private async Task<string> GetAllFruits()
        {
            HttpClient client = new HttpClient();
            string uri = "https://www.fruityvice.com";

            string requestParameter = $"/api/fruit/all";
            string uriBase = uri + requestParameter;
            HttpResponseMessage httpResponse;
            httpResponse = await client.GetAsync(uriBase);
            string contentString = await httpResponse.Content.ReadAsStringAsync();
            var newContent = JToken.Parse(contentString).ToString();
            return newContent;
        }
    }
}
