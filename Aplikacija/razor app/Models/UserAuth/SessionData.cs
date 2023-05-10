using System.Collections.Generic;
using System.Globalization;
using backend.Enums;
using backend.Services;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace backend.Models.UserAuth
{
    public class SessionData
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public List<Proizvod> Korpa { get; set; }
        public string Slika { get; set; }
        public List<int> UkupnaKolicina { get; set; }
        public UserCategory TipKorisnika { get; set; }
        
        public ICachingData CachingData
        { get;
            set;
        }
        
        public SessionData()
        {
            Id = -1;
            Ime = Prezime = Email = string.Empty;
            Korpa = new List<Proizvod>();
            UkupnaKolicina = new List<int>();
            TipKorisnika = UserCategory.NotLogged;
            CachingData = new CachingData();
            Slika = string.Empty;
        }
        public SessionData(SessionData sessionData)
        {
            Id = sessionData.Id;
            Ime = sessionData.Ime;
            Prezime = sessionData.Prezime;
            Email = sessionData.Email;
            Korpa = sessionData.Korpa;
            UkupnaKolicina = sessionData.UkupnaKolicina;
            TipKorisnika = sessionData.TipKorisnika;
            CachingData = sessionData.CachingData;
            Slika = sessionData.Slika;
        }

        public void SetCC(int cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.CC = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetCPT(int cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.CPT = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetBtn(int cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.Btn = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetClans(List<Clan> cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.Clans = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void AddRangeClans(List<Clan> cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.Clans.AddRange(cc);
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void AddClan(Clan cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.Clans.Add(cc);
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetPersonalniTreners(List<PersonalniTrener> cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.PersonalniTreners = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void AddRangePersonalniTreners(List<PersonalniTrener> cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.PersonalniTreners.AddRange(cc);
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void AddPersonalniTreners(PersonalniTrener cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.PersonalniTreners.Add(cc);
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetZahteviPersonalniTrener(List<ZahtevPersonalniTrener> cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.ZahteviPersonalniTrener = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetcurrentCulture(CultureInfo cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.currentCulture = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetCurrentPage(string cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.CurrentPage = cc;
           sessionDataService.SetSessionDataAsync(session,this).Wait();
        }
        public void SetProizvodID(int cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.proizvodID = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetPoruka(string cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.CurrentPage = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetProducts(List<Proizvod> cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.Products = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetCategory(ProductCategory cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.Category = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetInit(int cc,ISessionDataService sessionDataService,ISession session)
        {
            CachingData.Init = cc;
            sessionDataService.SetSessionDataAsync(session,this);
        }
        public void SetDictionary(Dictionary<ProductCategory,int> NumOfSynced, ISessionDataService sessionDataService, ISession httpContextSession)
        {
            CachingData.NumOfSynced = NumOfSynced;
            sessionDataService.SetSessionDataAsync(httpContextSession, this);
        }
    }
}