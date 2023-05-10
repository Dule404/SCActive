using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using backend.Enums;
using backend.Models;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace backend.Services
{
    public class CachingData : ICachingData
    {
        public List<Clan> Clans { get; set; }
        public int CC { get; set; }
        public List<PersonalniTrener> PersonalniTreners { get;  set;}
        public List<ZahtevPersonalniTrener> ZahteviPersonalniTrener { get; set; }
        public int CPT { get; set; }
        public  int Btn { get;  set;}
        public CultureInfo currentCulture { get; set; }
        public string CurrentPage { get;  set;}
        public int proizvodID { get;  set;}
        public string Poruka { get; set;}
        public List<Proizvod> Products { get; set; }
        public ProductCategory Category { get;set;}
        public int Init { get; set; }
        public Dictionary<ProductCategory,int> NumOfSynced { get; set; }
        
        public void RemoveAll()
        {
            CC = 0;
            Clans = new List<Clan>();
            CPT = 0;
            PersonalniTreners = new List<PersonalniTrener>();
            Poruka = string.Empty;
            Btn = 0;
            Products = new List<Proizvod>();
            Category = ProductCategory.Sweater;
            Init = 0;
            NumOfSynced = new Dictionary<ProductCategory, int>();
            CurrentPage = string.Empty;
        }

        public void RemoveAllTrenerPage()
        {
            RemoveAll();
            ZahteviPersonalniTrener = new List<ZahtevPersonalniTrener>();
        }


        public CachingData()
        {
            Clans = new List<Clan>();
            PersonalniTreners = new List<PersonalniTrener>();
            CC = CPT = 0;
            Btn = 0;
            CurrentPage = String.Empty;
            ZahteviPersonalniTrener = new List<ZahtevPersonalniTrener>();
            proizvodID = -99;
            currentCulture = CultureInfo.CurrentCulture;
            Poruka = string.Empty;
            Products = new List<Proizvod>();
            Category = ProductCategory.Sweater;
            Init = 0;
            NumOfSynced = new Dictionary<ProductCategory, int>();
        }

        public CachingData(ICachingData s)
        {
            Clans = s.Clans;
            PersonalniTreners = s.PersonalniTreners;
            CC = s.CC;
            CPT = s.CPT;
            Btn = s.Btn;
            CurrentPage = s.CurrentPage;
            ZahteviPersonalniTrener = s.ZahteviPersonalniTrener;
            proizvodID = s.proizvodID;
            currentCulture = s.currentCulture;
            Poruka = s.Poruka;
            Products = s.Products;
            Category = s.Category;
            Init = s.Init;
            NumOfSynced = s.NumOfSynced;
        }
    }
}