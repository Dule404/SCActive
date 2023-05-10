using System.Collections.Generic;
using System.Globalization;
using backend.Enums;
using backend.Models;
using backend.Models.UserAuth;
using Microsoft.AspNetCore.Http;

namespace backend.Services.Interfaces
{
    public interface ICachingData
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
        public void RemoveAll();
        public void RemoveAllTrenerPage();
        public List<Proizvod> Products { get; set; }
        public ProductCategory Category { get;set;}
        public int Init { get; set; }
        public Dictionary<ProductCategory,int> NumOfSynced { get; set; }
        
    }
}