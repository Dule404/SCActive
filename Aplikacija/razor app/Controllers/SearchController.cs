using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace backend.Controllers
{   
    [ApiController]
    [Route("[controller]")]
    public class SearchController :ControllerBase
    {
            private readonly IDatabaseService _databaseService;

            public SearchController(IDatabaseService databaseService)
            {
                _databaseService = databaseService;
            }
           
            [HttpGet]
            [Route ("SearchThing/{thing}")]
            public async Task<IActionResult> SearchThing(string thing)
            {
                var res = new List<object>();
                object o;
                try
                {
                    var resp = await _databaseService.SearchThings(thing);
                    if (resp.Status)
                    {
                        res = resp.Data as List<object>;
                    }
                    o = new { list = res, status = 1 };
                    if (!res.Any())
                    {
                        o = new { list = res, status = 0 };
                    }
                }
                catch (Exception e)
                {
                    return UnprocessableEntity(e.Message);
                }

                return Ok(o);
            }
        }
    }