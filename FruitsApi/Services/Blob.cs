﻿using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Text_Speech.Services
{
    public class Blob : IBlob
    {
        private readonly string Container = "textimages";
        private readonly BlobServiceClient _blobServiceClient;
        public Blob(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task Upload(IFormFile model)
        {
            var blobClient = GetBlobServiceClient(model.FileName);
            await blobClient.UploadAsync(model.OpenReadStream(), overwrite: true);

        }
        public async Task<byte[]> Download(IFormFile file)
        {
            var blobClient = GetBlobServiceClient(file.FileName);
            var download = blobClient.Download();
            using MemoryStream ms = new MemoryStream();
            await download.Value.Content.CopyToAsync(ms);
            return ms.ToArray();

        }
        /*         public async Task UploadFile(FileStream file)
                {
                    var blobClient = GetBlobServiceClient("Document.Docx");
                    await blobClient.UploadAsync(file, overwrite: true);

                }

                public async Task UploadStream(Stream model)
                {
                    var blobClient = GetBlobServiceClient("Audio.mp3");
                    await blobClient.UploadAsync(model,overwrite:true);    

                }
                public Uri GetUri(string file) 
                {
                    var blobClient = GetBlobServiceClient(file);
                    if (blobClient.ExistsAsync().Result)
                    {
                        var s = blobClient.Uri;
                        return s;
                    }

                    return null;
                }

                public async Task<string[]> DownloadFile(string file)
                {
                    var blobClient = GetBlobServiceClient(file);
                    var files =  blobClient.Download();
                    string[] result;
                    using (MemoryStream ms = new MemoryStream())
                    { 
                        await files.Value.Content.CopyToAsync(ms);
                         result = Encoding.
                          ASCII.
                          GetString(ms.ToArray()).
                          Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    }

                    return result;
                }*/
        private BlobClient GetBlobServiceClient(string name) 
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Container);
            var blobClient = blobContainer.GetBlobClient(name);
            return blobClient;
        }
    }
}

