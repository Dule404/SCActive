using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace backend.Services.Interfaces
{
    public interface ICloudStorage
    {
        Task<string> UploadAsync(IFormFile file);
        Task DeleteImage(string name);
    }
}
