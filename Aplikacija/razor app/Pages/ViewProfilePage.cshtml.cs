using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using backend.Enums;
using backend.Models;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using backend.Models.DbResponse;

namespace backend.Pages
{
    public class ViewProfilePage : PageModel
    {
        private readonly ResourceManager _resourceManager;
        private readonly IDatabaseService _databaseService;
        private ISessionDataService _sessionDataService;
        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session;
        private SessionData user;
        public ICachingData CachingData;

        public Clan Clan { get; set; }
        public Clan CurrentClan { get; set; }
        public PersonalniTrener CurrentTrener { get; set; }
        public Administrator Admin { get; set; }
        public PersonalniTrener PersonalniTrener { get; set; }
        public int UserViewID { get; set; }
        public string Slika { get; set; }
        public UserCategory UserCategory { get; set; }
        public int btn { get; set; }

        public ViewProfilePage(IDatabaseService databaseService, ISessionDataService sessionDataService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseService = databaseService;
            _sessionDataService = sessionDataService;
            _httpContextAccessor = httpContextAccessor;
            _resourceManager = new ResourceManager(typeof(Translations.korisnikedit));
            _session = _httpContextAccessor.HttpContext.Session;
            user = _sessionDataService.GetSessionData(_session);
            CachingData = user.CachingData;
            if(CachingData.Clans.Any())
                CurrentClan=CachingData.Clans.First();
            btn = CachingData.Btn;

        }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                _session = _httpContextAccessor.HttpContext.Session;
                user = _sessionDataService.GetSessionData(_session);
                CachingData = user.CachingData;
                btn = CachingData.Btn;


                if (user.Id < 0)
                {
                    return RedirectToPage("/Prijava");
                }

                UserViewID = CachingData.proizvodID;

                var resp1 = await _databaseService.GetClanove(new List<int> { UserViewID });
                if(resp1.Status)
                {
                    Clan = resp1.Data.FirstOrDefault() as Clan;
                    if (Clan.Role == 1)
                        UserCategory = UserCategory.PersonalTrainer;
                }
                else
                {
                    return Redirect("/Index?culture=" + CultureInfo.CurrentCulture);
                }
                if (user.TipKorisnika == UserCategory.DefaultUser)
                {
                    var resp=await _databaseService.GetClanove(new List<int> { user.Id });
                    if(resp.Status)
                    {
                        CurrentClan = resp.Data.FirstOrDefault() as Clan;
                        CachingData.Clans.Clear();
                        CachingData.Clans.Add(resp.Data.FirstOrDefault() as Clan);
                        CachingData.Btn = 0;
                    }
                    var vecPostoji = await _databaseService.GetPersonalneTrenereByClanID(CurrentClan.ID);
                    if (vecPostoji.Status)
                    {
                        var p=vecPostoji.Data.FirstOrDefault() as PersonalniTrener;
                        if(p.ID==Clan.ID)
                            CachingData.Btn = 10;
                    }
                }
                if (user.TipKorisnika == UserCategory.PersonalTrainer)
                {
                    var resp = await _databaseService.GetClanove(new List<int> { user.Id });
                    if (resp.Status)
                    {
                        CurrentTrener = new PersonalniTrener(resp.Data.FirstOrDefault() as Clan);
                        CachingData.Clans.Clear();
                        CachingData.Clans.Add(resp.Data.FirstOrDefault() as Clan);
                    }
                }
                if (user.TipKorisnika == UserCategory.Administrator)
                {
                    var resp = await _databaseService.GetAdministrator(new List<int> { user.Id });
                    if (resp.Status)
                    {
                        CachingData.Clans.Clear();
                        CachingData.Clans.Add(resp.Data.FirstOrDefault() as Clan);
                    }
                }
            }
            catch (Exception ex)
            {
                return Redirect("/Index?culture=" + CultureInfo.CurrentCulture);
            }

            await _sessionDataService.SetSessionDataAsync(_session, user);
            return Page();
        }

        public async Task<IActionResult> OnPost(int idz)
        {
            var users = await _sessionDataService.GetSessionDataAsync(_session);
            try
            {
                /*
                var vecPostoji = await _databaseService.GetPersonalneTrenereByClanID(idz);
                if(vecPostoji.Status)
                {
                    CachingData.Btn = 10;
                    await _sessionDataService.SetSessionDataAsync(_session, user);
                    return Redirect("/ViewProfilePage?culture=" + CultureInfo.CurrentCulture);
                }
                */
                var resp = await _databaseService.RequestPersonalniTrener(idz, CurrentClan.ID);
                if(resp.Status)
                {
                    users.CachingData.CPT = 10;
                    //Disable button for send request!
                }
                else
                {
                    if (resp.Data != null)
                    {
                        //zahtev je vec poslat
                        users.CachingData.CPT = 11;
                        await _sessionDataService.SetSessionDataAsync(_session, users);
                    }
                }
            }
            catch(Exception ex)
            {
                await _sessionDataService.SetSessionDataAsync(_session, users);
                return Redirect("/Index?culture=" + CultureInfo.CurrentCulture);
            }
            await _sessionDataService.SetSessionDataAsync(_session, users);
            return Redirect("/ViewProfilePage?culture=" + CultureInfo.CurrentCulture);
        }
        public RedirectResult OnPostProfile()
        {
            return Redirect("/Korisnik?culture=" + CultureInfo.CurrentCulture);
        }

        public async Task<IActionResult> OnPostLogOut()
        {
            var _userSessionData = new SessionData();
            await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session, _userSessionData);
            return Redirect("/Index?culture=" + CultureInfo.CurrentCulture);
        }
    }
}