using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Resources;
using System.Threading.Tasks;
using backend.Enums;

namespace backend.Pages
{
    public class Prodavnica : PageModel
    {
        private readonly ISessionDataService _sessionDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDatabaseService _databaseService;
        private readonly ITranslatorService _translatorService;
        private ISession _session;
        private ICachingData _cachingData;
        private SessionData _sessionData;
        public List<Models.Proizvod> Products { get; set; }
        public ProductCategory Category { get; set; }

        public Prodavnica(IDatabaseService databaseService, IHttpContextAccessor httpContextAccessor, ISessionDataService sessionDataService, ITranslatorService translatorService)
        {
            _databaseService = databaseService;
            _httpContextAccessor = httpContextAccessor;
            _sessionDataService = sessionDataService;
            _translatorService = translatorService;
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            _cachingData = _sessionData.CachingData;
            Products = _cachingData.Products;
            Category = _cachingData.Category;
        }

        public async Task OnGet()
        {
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            _cachingData = _sessionData.CachingData;
            Products = _cachingData.Products;
            Category = _cachingData.Category;
            if (_cachingData.Init != 1)
            {
                var resp = await _databaseService.GetProducts(Category);
                _cachingData.NumOfSynced.Add(Category, 1);
                if (resp.Status)
                {
                    _cachingData.Products.AddRange(resp.Data as List<Models.Proizvod>);
                }
                _cachingData.Init = 1;
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
        }

        public async Task<IActionResult> OnPostHandler(int id1)
        {
            _cachingData.proizvodID = id1;
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return RedirectToPage("/Proizvod");
        }
        public async Task<IActionResult> OnPostProduct(ProductCategory id1,CultureInfo id2)
        {
            _cachingData.Category = id1;
            if (_cachingData.NumOfSynced.ContainsKey(id1))
            {
                await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
                return Redirect("/Prodavnica?culture=" +id2.Name);
            }

            _cachingData.NumOfSynced.Add(id1, 1);
            var resp = await _databaseService.GetProducts(id1);
            if (resp.Status)
            {
                _cachingData.Products.AddRange(resp.Data as List<Models.Proizvod>);
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/Prodavnica?culture=" +id2.Name);
        }    
        public async Task<IActionResult> OnPostSS(ProductCategory id1,CultureInfo id2)
        {
            _cachingData.Category = id1;
            var resp = await _databaseService.GetProducts(Category,_cachingData.NumOfSynced[id1]++);
            if (resp.Status)
            {
                _cachingData.Products.AddRange(resp.Data as List<Models.Proizvod>);
                _cachingData.Products = _cachingData.Products.OrderBy(x => x.ID).ToList();
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/Prodavnica?culture=" +id2.Name);
        }
        public RedirectResult OnPostProfile()
        {
            return Redirect("/Korisnik?culture=" + CultureInfo.CurrentCulture);
        }

        public async Task<IActionResult> OnPostLogOut()
        {
            var userSessionData = new SessionData();
            await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session, userSessionData);
            return Redirect("/Index?culture=" + CultureInfo.CurrentCulture);
        }
    }
}
