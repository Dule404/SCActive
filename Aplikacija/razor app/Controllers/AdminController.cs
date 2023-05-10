using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using backend.Filter;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [CustomActionFilter]
    public class AdminController : ControllerBase
    {
        private readonly DbContextSCActive _contextScActive;
        private readonly IHashService _hashService;
        private readonly IDatabaseService _databaseService;

        public AdminController(DbContextSCActive contextScActive, IHashService hashService, IDatabaseService databaseService)
        {
            _contextScActive = contextScActive;
            _hashService = hashService;
            _databaseService = databaseService;
        }

        [Route ("AddClan")]
        [HttpPost]
        public async Task<ActionResult> AddClan([FromBody]List<Clan> clans)
        {
            try{
                foreach (var c in clans)
                {

                    await _databaseService.PostClan(c);
                }
                return Ok($"Clans added.");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }
        [Route ("AddPt")]
        [HttpPost]
        public async Task<ActionResult> AddPt([FromBody]List<PersonalniTrener> pt)
        {
            
            try{
                foreach (var c in pt)
                {

                    await _databaseService.PostPersonalniTrener(c);
                }
                return Ok($"Personaltrainers added.");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }
        [Route ("AddPosts")]
        [HttpPost]
        public async Task<ActionResult> AddPosts([FromBody]List<Post> posts)
        {
            
            try{
                await _contextScActive.Posts.AddRangeAsync(posts);
                await _contextScActive.SaveChangesAsync();
                return Ok($"Posts added.");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [Route ("AddProduct")]
        [HttpPost]
        public async Task<ActionResult> AddPet([FromBody]Proizvod proizvod)
        {
            
            try{
                _contextScActive.Proizvodi.Add(proizvod);
                await _contextScActive.SaveChangesAsync();
                return Ok($"Product is added. ID = {proizvod.ID}");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }
        [Route ("AddAdmin")]
        [HttpPost]
        public async Task<ActionResult> PostAdmin([FromBody]Administrator administrator)
        {
            try
            {
                administrator.Lozinka = _hashService.HashString(administrator.Lozinka);
                await _contextScActive.Clanovi.AddRangeAsync(new Clan(administrator));
                await _contextScActive.SaveChangesAsync();
                return Ok($"Administrator is added. ID = {administrator.ID}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Route("ChangeProduct/{id}/{ime}/{opis}/{cena}/{kolicina}/{kategorija}")]
        [HttpPut]
        public async Task<ActionResult> ChangeProduct(int id, string ime, string opis, string cena, int kolicina, int kategorija)
        {
            try 
            {
                await _databaseService.UpdateProduct(id, ime, opis, cena, kolicina, kategorija);
                await _contextScActive.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [Route("DeleteProduct/{id}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                await _databaseService.RemoveProduct(id);
                await _contextScActive.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Route("DeleteClan/{id}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteClan(int id)
        {
            try
            {
                await _databaseService.RemoveClan(id);
                await _contextScActive.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}