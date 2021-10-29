using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Text_Speech.Services
{
    public  interface IBlob
    {
        Task Upload(IFormFile model);
        Task<byte[]> Download(IFormFile file);
/*        Task UploadFile(FileStream file);
      
        Task UploadStream(Stream model);
        Uri GetUri(string file);
        
        Task<string[]> DownloadFile(string file);*/
    }
}
