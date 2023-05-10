using backend.ViewComponents;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models;
using backend.Filter;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [CustomActionFilter]
    public class PicturesController : Controller
    {
        private readonly ICloudStorage _storage;

        public PicturesController( ICloudStorage storage, IDatabaseService databaseService)
        {
            _storage = storage;
        }

        [Route("AddPicture")]
        [HttpPost]
        public async Task<IActionResult> postPicture([FromForm] IFormFile file)
        {
            try
            {
                var v = await _storage.UploadAsync(file);
                return Ok(new { url = v});
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("DeletePicture")]
        [HttpDelete]
        public async Task<IActionResult> deletePicture(ImageView model)
        {
            try
            {
                await _storage.DeleteImage(model.Url);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
