using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using backend.Enums;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionDataService _sessionDataService;
        public HomeController(IDatabaseService databaseService, IHttpContextAccessor httpContextAccessor, ISessionDataService sessionDataService)
        {
            _databaseService = databaseService;
            _httpContextAccessor = httpContextAccessor;
            _sessionDataService = sessionDataService;
        }
        
        [Route ("GetPt/{count}/{page}")]
        [HttpGet]
        public async Task<ActionResult> GetPt(int count, int page)
        {
            List<PersonalniTrener> p;
            object o;
            try
            {
                var res = await _databaseService.GetPersonalneTrenere(null, page, count);
                if (res.Status)
                {
                    p = res.Data as List<PersonalniTrener>;
                    o = new { list = p, status = 1 };
                    if (!p.Any())
                    {
                       o = new { list = p, status = 0 };
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e.Message);
            }

            return Ok(o);
        }

        [Route("GetClanove/{count}/{page}")]
        [HttpGet]
        public async Task<ActionResult> GetClanove(int count, int page)
        {
            List<Clan> p;
            object o;
            try
            {
                var res = await _databaseService.GetClanove(null, page, count);
                if (res.Status)
                {
                    p = res.Data as List<Clan>;
                    o = new { list = p, status = 1 };
                    if (!p.Any())
                    {
                        o = new { list = p, status = 0 };
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(o);
        }

        [Route ("GetProdudcts/{count}/{page}")]
        [HttpGet]
        public async Task<ActionResult> GetProdudcts(int count, int page)
        {
            List<Proizvod> p;
            object o;
            try
            {
                var res = await _databaseService.GetProducts(ProductCategory.All, page, count);
                if (res.Status)
                {
                    p = res.Data as List<Proizvod>;
                    o = new { list = p, status = 1 };
                    if (!p.Any())
                    {
                        o = new { list = p, status = 0 };
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(o);
        }
        
        [Route ("GetPosts/{count}/{page}")]
        [HttpGet]
        public async Task<ActionResult> GetPosts(int count, int page)
        {
            List<Post> p;
            object o;
            try
            {
                var res = await _databaseService.GetPosts(page, count);
                if (res.Status)
                {
                    p = res.Data as List<Post>;
                    o = new { list = p, status = 1 };
                    if (!p.Any())
                    {
                        o = new { list = p, status = 0 };
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(o);
        }
        [Route ("GetPostsForUser/{id}/{count}/{page}")]
        [HttpGet]
        public async Task<ActionResult> GetPostsForUser(int id,int count,int page)
        {
            List<Post> p;
            object o;
            try
            {
                var res = await _databaseService.GetPosts(page, count,id);
                if (res.Status)
                {
                    p = res.Data as List<Post>;
                    o = new { list = p, status = 1 };
                    if (!p.Any())
                    {
                        o = new { list = p, status = 0 };
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(o);
        }
        [Route ("GoToPage/{id}")]
        [HttpGet]
        public async Task<IActionResult> GoToPage(int id)
        {
            string res = string.Empty;
            try
            {
                var ses = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
                ses.CachingData.proizvodID = id;
                await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session, ses);
                res = "ViewProfilePage?culture="+CultureInfo.CurrentCulture;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(new { resp = res});
        }
        [Route ("GoToPageProduct/{id}")]
        [HttpGet]
        public async Task<IActionResult> GoToPageProduct(int id)
        {
            string res = string.Empty;
            try
            {
                var ses = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
                ses.CachingData.proizvodID = id;
                await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session, ses);
                res = "Proizvod?culture="+CultureInfo.CurrentCulture;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(new { resp = res});
        }

    }
}
