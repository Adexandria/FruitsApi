using FruitsApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        readonly static string computerVisionSubscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
        readonly static string computerVisonEndpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");
        readonly string computerVisionUriBase = computerVisonEndpoint + "vision/v3.0/analyze";

     
        public TextToString(IBlob blob)
        {
            this.blob = blob;
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
                var fruitname = GetFruitName(analysis);
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
                var client = GetClient(computerVisionSubscriptionKey);
                string requestParameters = "visualFeatures=Tags";
                string uri = computerVisionUriBase + "?" + requestParameters;
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
        private HttpClient GetClient(string key)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            return client;
        }

        [NonAction]
        private string GetFruitName(Analysis analysis)
        {
            int left = 0;
            int right = analysis.Tags.Count;
            while(left < right)
            {
                if(analysis.Tags[left].name == "fruit" || analysis.Tags[left].name == "natural foods" 
                    || analysis.Tags[left].name == "food")
                {
                    left++;
                }
                else
                {
                    return analysis.Tags[left].name;
                }
            }
            return null;
        }

    }
}
