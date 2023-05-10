using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Enums;
using backend.Models;
using backend.Models.DbResponse;

namespace backend.Services.Interfaces
{
    public interface IDatabaseService
    {
        public Task<DbResponse> GetClanByEmail(string email, string password = null);
        public Task<DbResponse> PostClan (Clan clan);
        public Task<DbResponse> PostPersonalniTrener (PersonalniTrener personalniTrener);
        public Task<DbResponse> PostPost(Post post);
        public Task<DbResponse> PostSport(Sport sport);
        public Task<DbResponse> PostPorudzbina(Porudzbina porudzbina);
        public Task<DbResponse> PostKomentar(Kontakt con);
        public Task<DbResponse> GetClanove (List<int> ids=null,int page = 0,int count = 15);
        public Task<DbResponse> GetPersonalneTrenere (List<int> ids=null,int page = 0,int count = 15);
        public Task<DbResponse> GetAdministrator(List<int> ids=null);
        public Task<DbResponse> GetProizvod(int id);
        public Task<DbResponse> GetProductsCount(ProductCategory cat = ProductCategory.All);
        public Task<DbResponse> UpdateProduct(int id, string ime, string opis, string cena, int kolicina, int kategorija);
        public Task<DbResponse> RemoveProduct(int id);
        public Task<DbResponse> GetProducts(ProductCategory car,int page = 0,int count = 15, List<int> ids=null);
        public Task<DbResponse> Get25RandomProducts();
        public Task<DbResponse> ChangeQuantity(int proizvodID, int kolicina);
        public Task<DbResponse> LogUser(string email, string password);
        public Task<DbResponse> EmailCheck(string email); //unused
        public Task<DbResponse> RequestPersonalniTrener(int idPersonalniTrener,int idClan);
        public Task<DbResponse> SearchPersonalniTrener(string query);
        public Task<DbResponse> PrikaziZahtevePersonalnogTrenera(int idPersonalniTrener);
        public Task<DbResponse> PersonalniTrenerPrihvatiZahtev(int idPersonalniTrener,int idClan);
        public Task<DbResponse> PersonalniTrenerOdbijZahtev(int idPersonalniTrener, int idClan);
        public Task<DbResponse> PrikaziClanovePersonalnogTrenera(int idPersonalniTrener,int page= 0, int count = 15);
        public Task<DbResponse> PersonalniTrenerObrisiClana(int idPersonalniTrener, int idClan);
        public Task<DbResponse> GetPersonalneTrenereByClanID (int id);
        public Task<DbResponse> UpdateClan(Clan clan);
        public Task<DbResponse> UpdateAdmin(Administrator administrator,string slika=null);
        public Task<DbResponse> RemoveClan(int id);
        public Task<DbResponse> GetPosts(int page, int count, int clanid = -1);
        public Task<DbResponse> SearchThings(string value);
        public Task<DbResponse> AddPicToUser(string value,int id);

        public Task<DbResponse> ObrisiZahteve(int id);
    }
}