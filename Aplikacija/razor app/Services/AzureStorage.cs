using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace backend.Services
{
    public class AzureStorage : ICloudStorage
    {
        private readonly IStorageConnectionFactory _storageConnectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionDataService _sessionDataService;
        private SessionData _sessionData;

        public AzureStorage(IStorageConnectionFactory storageConnectionFactory, IHttpContextAccessor httpContextAccessor, ISessionDataService sessionDataService)
        {
            _storageConnectionFactory = storageConnectionFactory;
            _httpContextAccessor = httpContextAccessor;
            _sessionDataService = sessionDataService;
            _sessionData =  _sessionDataService.GetSessionData(_httpContextAccessor.HttpContext.Session);
        }

        public async Task DeleteImage(string name)
        {
            try
            {
                Uri uri = new Uri(name);
                string filename = Path.GetFileName(uri.LocalPath);
                var blobContainer = await _storageConnectionFactory.GetContainer();
                var blob = blobContainer.GetBlockBlobReference(filename);
                await blob.DeleteIfExistsAsync();
            }
            catch
            {
                //ignore
            }
        }

        public async Task<string> UploadAsync([FromForm]IFormFile file)
        {
            try
            {
                var blobContainer = await _storageConnectionFactory.GetContainer();
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(GetRandomBlobName(file.FileName));
                using (var stream = file.OpenReadStream())
                {
                    await blob.UploadFromStreamAsync(stream);
                }

                return blob.Uri.AbsoluteUri;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary> 
        /// string GetRandomBlobName(string filename): Generates a unique random file name to be uploaded  
        /// </summary> 
        private string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }
    }
}
