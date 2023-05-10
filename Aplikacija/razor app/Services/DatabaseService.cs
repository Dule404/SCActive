using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using backend.Enums;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Models.DbResponse;

namespace backend.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly DbContextSCActive _contextScActive;
        private readonly IHashService _hashService;
        private readonly IDateTimeService _dateTimeService;

        public DatabaseService(DbContextSCActive contextScActive, IHashService hashService,
            IDateTimeService dateTimeService)
        {
            _contextScActive = contextScActive;
            _hashService = hashService;
            _dateTimeService = dateTimeService;
        }

        #region HelpersMethods
        public async Task<DbResponse> GetClanByEmail(string email, string password = null)
        {
            DbResponse response;
            List<Clan> result = new List<Clan>();
            Clan c;
            try
            {
                if (password == null)
                    c = await _contextScActive.Clanovi.Where(x => x.Email == email).FirstOrDefaultAsync();
                else
                    c = await _contextScActive.Clanovi.Where(x => x.Email == email && x.Lozinka == password)
                    .FirstOrDefaultAsync();
                if (c != null)
                    result.Add(c);
                if (!result.Any())
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = result.ToList() };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        #endregion

        public async Task<DbResponse> PostClan(Clan clan)
        {
            DbResponse response;
            try
            {
                clan.DatumRegistracije = _dateTimeService.GetDateTimeNow();
                clan.Lozinka = _hashService.HashString(clan.Lozinka);
                clan.Role = (int)UserCategory.DefaultUser;

                await _contextScActive.Clanovi.AddRangeAsync(clan);
                await _contextScActive.SaveChangesAsync();

                response = new DbResponse
                    { Status = true, Message = new[] { "Success" }, Data = new List<object> { clan.ID } };
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }
        public async Task<DbResponse> PostPost(Post post)
        {
            DbResponse response;
            try
            {
                post.Created = _dateTimeService.GetDateTimeNow();

                await _contextScActive.Posts.AddRangeAsync(post);
                await _contextScActive.SaveChangesAsync();

                response = new DbResponse
                    { Status = true, Message = new[] { "Success" }, Data = new List<object> { post.ID } };
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> PostPersonalniTrener(PersonalniTrener personalniTrener)
        {
            DbResponse response;
            try
            {
                personalniTrener.DatumRegistracije = _dateTimeService.GetDateTimeNow();
                personalniTrener.Lozinka = _hashService.HashString(personalniTrener.Lozinka);
                personalniTrener.ListaKlijenata = new List<Clan>();
                var pT = new Clan(personalniTrener);

                await _contextScActive.Clanovi.AddRangeAsync(pT);
                await _contextScActive.SaveChangesAsync();
                response = new DbResponse
                {
                    Status = true, Message = new[] { "Success" }, Data = new List<object> { pT.ID }
                };
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> PostSport(Sport sport)
        {
            DbResponse response;
            try
            {
                var s = _contextScActive.Sportovi.Where(x => x.Ime == sport.Ime).FirstOrDefault();
                if (s == null)
                {
                    await _contextScActive.Sportovi.AddRangeAsync(sport);
                    await _contextScActive.SaveChangesAsync();

                    response = new DbResponse
                    {
                        Status = true, Message = new[] { "Success" }, Data = new List<object> { sport.ID }
                    };
                }
                else
                {
                    response = new DbResponse
                    {
                        Status = false, Message = new[] { "Success" }, Data = new List<object> { s }
                    };
                }
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            await Task.CompletedTask;
            return response;
        }

        public async Task<DbResponse> PostPorudzbina(Porudzbina porudzbina)
        {
            DbResponse response;
            try
            {
                await _contextScActive.Porudzbine.AddAsync(porudzbina);
                response = new DbResponse
                {
                    Status = true,
                    Message = new[] { "Success" },
                    Data = null
                };
                await _contextScActive.SaveChangesAsync();
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }
            await Task.CompletedTask;
            return response;
        }

        public async Task<DbResponse> GetPersonalneTrenere(List<int> ids=null,int page = 0,int count = 15)
        {
            DbResponse response;
            ConcurrentBag<PersonalniTrener> result = new ConcurrentBag<PersonalniTrener>();
            try
            {
                if(ids != null)
                    foreach (var id in ids)
                    {
                        PersonalniTrener c = new PersonalniTrener(await _contextScActive.Clanovi.Include(p => p.Sport)
                            .Where(x => x.ID == id && x.Role == (int)UserCategory.PersonalTrainer)
                            .FirstOrDefaultAsync());
                        result.Add(c);
                    }
                else
                {
                    Parallel.ForEach(_contextScActive.Clanovi.Include(p => p.Sport)
                        .Where(x => x.Role == (int)UserCategory.PersonalTrainer).OrderByDescending(x => x.ID)
                        .Skip(page * count).Take(count).ToList(),c =>
                    {   
                        result.Add(new PersonalniTrener(c));
                    });
                }
                if (!result.Any())
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = result.ToList() };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> GetAdministrator(List<int> ids = null)
        {
            DbResponse response;
            var result = new List<Administrator>();
            try
            {
                
                if (ids !=null)
                    foreach (var id in ids)
                    {
                        var c = new Administrator(await _contextScActive.Clanovi
                            .Where(x => x.ID == id).FirstOrDefaultAsync());
                        result.Add(c);
                    }
                if (!result.Any())
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = result.ToList() };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> GetClanove(List<int> ids=null,int page = 0,int count = 15)
        {
            
            DbResponse response;
            List<Clan> result = new List<Clan>();
            try
            {
                
                if (ids !=null)
                    foreach (var id in ids)
                    {
                        Clan c = await _contextScActive.Clanovi
                                .Where(x => x.ID == id).FirstOrDefaultAsync();
                        if (c != null)
                            result.Add(c);
                    }
                else
                {
                    result.AddRange(_contextScActive.Clanovi.OrderByDescending(x=>x.ID).Skip(page * count).Take(count));
                }
                if (!result.Any())
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = result.ToList() };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> GetProizvod(int id)
        {
            DbResponse response;
            try
            {
                    Proizvod p = await _contextScActive.Proizvodi.Where(x => x.ID == id).FirstOrDefaultAsync();
                if (p == null)
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = new[] { p } };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            await Task.CompletedTask;
            return response;
        }

        public async Task<DbResponse> GetProductsCount(ProductCategory cat = ProductCategory.All)
        {
            DbResponse response;
            var res = new List<int>();
            try
            {
                res.Add(await _contextScActive.Proizvodi.CountAsync(x=>x.Kategorija == (int)ProductCategory.Sweater 
                                                                       || x.Kategorija == (int)ProductCategory.Tracksuits 
                                                                            || x.Kategorija == (int)ProductCategory.Shorts  
                                                                        || x.Kategorija == (int)ProductCategory.Shirt
                                                                       || x.Kategorija == (int)ProductCategory.Socks));
                res.Add(await _contextScActive.Proizvodi.CountAsync(x=> x.Kategorija == (int)ProductCategory.Sneakers || x.Kategorija == (int)ProductCategory.FlipFlops));
                res.Add(await _contextScActive.Proizvodi.CountAsync(x=>x.Kategorija == (int)ProductCategory.Gear));
                res.Add(await _contextScActive.Proizvodi.CountAsync(x=>x.Kategorija == (int)ProductCategory.Supplements));
                if (cat != ProductCategory.All)
                    res.Add(await _contextScActive.Proizvodi.CountAsync(x=>x.Kategorija == (int)cat));
                if (!res.Any())
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = new[] { res } };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            await Task.CompletedTask;
            return response;
        }

        public async Task<DbResponse> GetProducts(ProductCategory cat, int page = 0, int count = 15, List<int> ids = null)
        {
            DbResponse response;
            var result = new List<Proizvod>();
            try
            {
                
                if (ids !=null)
                    foreach (var id in ids)
                    {
                        Proizvod c = await _contextScActive.Proizvodi
                            .Where(x => x.ID == id).FirstOrDefaultAsync();
                        if (c != null)
                            result.Add(c);
                    }
                else if (cat != ProductCategory.All)
                {
                    result.AddRange(_contextScActive.Proizvodi.Where(x=>x.Kategorija == (int)cat).OrderByDescending(x=>x.ID).Skip(page * count).Take(count));
                }
                else
                {
                    result.AddRange(_contextScActive.Proizvodi.OrderByDescending(x=>x.CreatedDate).Skip(page*count).Take(count));
                }
                if (!result.Any())
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = result.ToList() };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> GetAllProducts()
        {
            DbResponse response;
            List<Proizvod> result;
            try
            {
                result = await _contextScActive.Proizvodi.ToListAsync();
                if (!result.Any())
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data =result};
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            await Task.CompletedTask;
            return response;
        }

        public async Task<DbResponse> Get25RandomProducts()
        {
            DbResponse response = null;
            List<Proizvod> listaProizvoda = new List<Proizvod>();
            Proizvod tmp = new Proizvod();
            int num=0;
            List<int> nums = new List<int>();
            Random r=new Random();

            try
            {
                listaProizvoda = _contextScActive.Proizvodi.OrderBy(x => x.CreatedDate).Take(25).ToList();
                response = new DbResponse
                {
                    Status = true,
                    Message = new[] { "Success" },
                    Data = listaProizvoda
                };
            }
            catch (Exception e)
            {
                    response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            await Task.CompletedTask;
            return response;
        }

        public async Task<DbResponse> ChangeQuantity(int proizvodId, int kolicina)
        {
            DbResponse response;
            try
            {
                Proizvod p = await _contextScActive.Proizvodi.Where(x => x.ID == proizvodId).FirstOrDefaultAsync();
                if (p == null)
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                {
                    p.ID = p.ID;
                    p.Ime = p.Ime;
                    p.Cena = p.Cena;
                    p.Kategorija = p.Kategorija;
                    p.Opis = p.Opis;
                    p.Slika0 = p.Slika0;
                    p.Slika1 = p.Slika1;
                    p.Slika2 = p.Slika2;
                    p.Slika3 = p.Slika3;
                    p.Velicina = p.Velicina;
                    p.Kolicina = p.Kolicina - kolicina;
                    p.Kvarljivo = p.Kvarljivo;
                    p.DatumIstekaRoka = p.DatumIstekaRoka;
                    await _contextScActive.SaveChangesAsync();
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = new[] { p } };
                }
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            await Task.CompletedTask;
            return response;
        }

        public async Task<DbResponse> LogUser(string email, string password)
        {
            DbResponse response;
            object result = null;
            DbResponse resp ;
            password = _hashService.HashString(password);
            try
            {
                resp = await GetClanByEmail(email, password);
                if (resp.Status)
                {
                    result = resp.Data as List<Clan>;
                }
                
                if (result == null)
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse
                        { Status = true, Message = new[] { "Success" }, Data = new[] { result } };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> EmailCheck(string email)
        {
            DbResponse response;
            object result = null;
            DbResponse resp;
            try
            {
                resp = await GetClanByEmail(email);
                if (resp.Status)
                {
                    result = resp.Data as List<Clan>;
                }

                if (result == null)
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse
                        { Status = true, Message = new[] { "Success" }, Data = new[] { result } };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> RequestPersonalniTrener(int idPersonalniTrener, int idClan)
        {
            DbResponse response;
            
            try
            {
                var clan = await GetClanove(new List<int> { idClan });
                var ptrener = await GetPersonalneTrenere(new List<int> { idPersonalniTrener });
                var postojeciZahtev = _contextScActive.ZahtevPersonalniTreners.Where(x=>x.CID==idClan && x.PTID==idPersonalniTrener).FirstOrDefault();

                if(postojeciZahtev!=null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Zahtev je vec poslat." }, Data = new[] { new { zahtev = 1} } };
                }

                if(ptrener == null || ptrener.Status==false)
                {
                    return new DbResponse { Status = false, Message = new[] { "Personalni trener ne postoji" }, Data=null };
                }

                if (clan == null || clan.Status==false)
                {
                    return new DbResponse { Status = false, Message = new[] { "Clan ne postoji" }, Data = null };
                }

                ZahtevPersonalniTrener zpt = new ZahtevPersonalniTrener();

                zpt.CID = idClan;
                zpt.PTID = idPersonalniTrener;

                _contextScActive.ZahtevPersonalniTreners.Add(zpt);
                await _contextScActive.SaveChangesAsync();
                response = new DbResponse { Status = true, Message = new[] { "Zahtev je poslat." }, Data = null };
            }
            catch(Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }
            return response;
        }

        public async Task<DbResponse> SearchPersonalniTrener(string query)
        {
            DbResponse response;

            try
            {
                var ptrener = await _contextScActive.Clanovi.Where(x => (x.Ime.Contains(query) || x.Prezime.Contains(query) || x.Sport.Ime.Contains(query)) && x.Role == (int)UserCategory.PersonalTrainer).ToListAsync();
                if (ptrener == null)
                {
                    return new DbResponse { Status=false, Message = new[] {"Nema rezultata"}, Data=null};
                }

                var pt = new ConcurrentBag<PersonalniTrener>();
                Parallel.ForEach(ptrener,c =>
                {   
                    pt.Add(new PersonalniTrener(c));
                });
                /*List<PersonalniTrener> pt = ptrener;*/
                response = new DbResponse { Status = true, Message = new[] { "Rezultati" }, Data = pt.ToList() };
            }
            catch (Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }
            return response;
        }

        public async Task<DbResponse> PrikaziZahtevePersonalnogTrenera(int idPersonalniTrener)
        {
            DbResponse response;
            try
            {
                var Clans = _contextScActive.Clanovi;
                var zahtevi = _contextScActive.ZahtevPersonalniTreners.Where(x => x.PTID == idPersonalniTrener)
                    .SelectMany(zahtev => Clans.Where(y=>y.ID == zahtev.CID).Select(x => new ZahtevPersonalniTrener()
                    {
                        ID = zahtev.ID,
                        CID = zahtev.CID,
                        Clan = x,
                        PTID = zahtev.PTID,
                        PersonalniTrener = new PersonalniTrener(Clans.Where(c=>c.ID == zahtev.PTID).FirstOrDefault())
                    })).ToList();
                
                if (zahtevi.Count <= 0)
                {
                    return new DbResponse { Status = false, Message = new [] { "Nema zahteva" }, Data = null };
                }
                var pt = _contextScActive.Clanovi.Where(x => x.ID == idPersonalniTrener).FirstOrDefault();
                if(pt == null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Trener ne postoji" }, Data = null };
                }

                response = new DbResponse { Status = true, Message = new[] { "Rezultati" }, Data = zahtevi };
            }
            catch(Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message}, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> PersonalniTrenerPrihvatiZahtev(int idPersonalniTrener, int idClan)
        {
            DbResponse response;
            try
            {
                var clan = _contextScActive.Clanovi.Where(x => x.ID == idClan).FirstOrDefault();
                var pt = _contextScActive.Clanovi.Where(x => x.ID == idPersonalniTrener).FirstOrDefault();
                var zpt = _contextScActive.ZahtevPersonalniTreners
                    .Where(x => x.PTID == idPersonalniTrener && x.CID == idClan).FirstOrDefault();

                if (clan == null)
                {
                   return new DbResponse { Status = false, Message = new[] { "Clan ne postoji." }, Data = null };
                }
                if (pt == null)
                {
                    return new DbResponse
                        { Status = false, Message = new[] { "Personalni trener ne postoji." }, Data = null };
                }
                if (zpt == null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Zahtev ne postoji." }, Data = null };
                }
                
                if (pt.ListaKlijenata == null)
                {
                    pt.ListaKlijenata = new List<Clan>();
                }

                pt.ListaKlijenata.Add(clan);
                _contextScActive.ZahtevPersonalniTreners.Remove(zpt);
                await _contextScActive.SaveChangesAsync();
                response = new DbResponse
                {
                    Status = true, Message = new[] { "Zahtev je prihvacen,clan dodat u listu klijenata." }, Data = null
                };
            }
            catch (Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }

            return response;
            
        }

        public async Task<DbResponse> PersonalniTrenerOdbijZahtev(int idPersonalniTrener, int idClan)
        {
            DbResponse response;
            try
            {
                var clan = _contextScActive.Clanovi.Where(x => x.ID == idClan).FirstOrDefault();
                var pt = _contextScActive.Clanovi.Where(x => x.ID == idPersonalniTrener).FirstOrDefault();
                var zpt = _contextScActive.ZahtevPersonalniTreners.Where(x => x.PTID == idPersonalniTrener && x.CID == idClan).FirstOrDefault();

                if (clan == null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Clan ne postoji." }, Data = null };
                }
                if (pt == null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Personalni trener ne postoji." }, Data = null };
                }
                if (zpt == null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Zahtev ne postoji." }, Data = null };
                }

                _contextScActive.ZahtevPersonalniTreners.Remove(zpt);
                await _contextScActive.SaveChangesAsync();
                response = new DbResponse { Status = true, Message = new[] { "Zahtev je odbijen" }, Data = null };
            }
            catch (Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }
            return response;
        }

        public async Task<DbResponse> PrikaziClanovePersonalnogTrenera(int idPersonalniTrener,int page= 0, int count = 15)
        {
            DbResponse response;
            try
            {
                List<Clan> result = new List<Clan>();
                var personalniTrener = _contextScActive.Clanovi
                                                     .Include(x => x.ListaKlijenata)
                                                     .Where(x => x.ID == idPersonalniTrener).FirstOrDefault();
                if (personalniTrener == null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Personalni trener ne postoji." }, Data = null };
                }
                else
                {
                    result.AddRange(personalniTrener.ListaKlijenata.OrderByDescending(x=>x.ID).Skip(page*count).Take(count));
                }
                response = new DbResponse { Status = true, Message = new[] { "Pronadjena su dokumenta." }, Data = result.ToList() };
            }
            catch(Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }

            await Task.CompletedTask;
            return response;
        }

        public async Task<DbResponse> PersonalniTrenerObrisiClana(int idPersonalniTrener,int idClan)
        {
            DbResponse response;
            try
            {
                var personalniTrener = _contextScActive.Clanovi
                                                     .Include(x => x.ListaKlijenata)
                                                     .Where(x => x.ID == idPersonalniTrener).FirstOrDefault();
                var clan = _contextScActive.Clanovi.Where(x=>x.ID==idClan).FirstOrDefault();

                if(personalniTrener==null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Personalni trener ne postoji." }, Data = null };
                }
                if(clan==null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Clan nije pronadjen." }, Data = null };
                }
                personalniTrener.ListaKlijenata.Remove(clan);
                await _contextScActive.SaveChangesAsync();
                response = new DbResponse { Status = true, Message = new[] { "Clan je uspesno obrisan." }, Data = null };
            }
            catch(Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }
            return response;
        }

        public async Task<DbResponse> GetPersonalneTrenereByClanID(int id)
        {
            DbResponse response;
            List<PersonalniTrener> result = new List<PersonalniTrener>();
            PersonalniTrener c;
            try
            {
                var r = _contextScActive.Clanovi.Include(p=>p.ListaKlijenata).Include(p=>p.Sport).Where(x => x.Role == 1).ToList();
                if(r==null)
                {
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                }
                else
                {
                    r.ForEach(x =>
                    {
                        if (x.ListaKlijenata != null)
                        {
                            if (x.ListaKlijenata.Find(y => y.ID == id) != null)
                            {
                                result.Add(new PersonalniTrener(x));
                            }
                        }
                    });
                    if (!result.Any())
                        response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                    else
                        response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = result.ToList() };
                }
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> PostKomentar(Kontakt con)
        {
            DbResponse response;
            try
            {
                await _contextScActive.Kontakti.AddAsync(con);
                response = new DbResponse
                {
                    Status = true,
                    Message = new[] { "Success" },
                    Data = null
                };
                await _contextScActive.SaveChangesAsync();
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }
            await Task.CompletedTask;
            return response;
        }

        public async Task<DbResponse> UpdateClan(Clan clan)
        {
            DbResponse response;
            try
            {
                var c = _contextScActive.Clanovi.Where(p => p.ID == clan.ID).FirstOrDefault();
                if (c == null)
                {
                    response = new DbResponse { Status = false, Message = new[] { "Clan nije pronadjen." }, Data = null };
                }
                else
                {
                    if (clan.Ime != null)
                        c.Ime = clan.Ime;
                    if (clan.Prezime != null)
                        c.Prezime = clan.Prezime;
                    if (clan.Email != null)
                        c.Email = clan.Email;
                    if (clan.Lozinka != null)
                        c.Lozinka = _hashService.HashString(clan.Lozinka);
                    if (clan.Telefon != null)
                        c.Telefon = clan.Telefon;
                    if (clan.DatumRodjenja != DateTime.MinValue)
                        c.DatumRodjenja = clan.DatumRodjenja;
                    if (clan.Slika != null)
                        c.Slika = clan.Slika;

                    await _contextScActive.SaveChangesAsync();
                    response = new DbResponse { Status = true, Message = new[] { "Clan je azuriran." }, Data = new List<Clan> { c } };
                }
            }
            catch(Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }
            await Task.CompletedTask;
            return response;
        }

        public async Task<DbResponse> UpdateAdmin(Administrator administrator,string slika=null)
        {
            DbResponse response;
            try
            {
                var a = _contextScActive.Clanovi.Where(p => p.ID == administrator.ID).FirstOrDefault();
                if(a==null)
                {
                    response = new DbResponse { Status = false, Message = new[] { "Admin nije pronadjen." }, Data = null };
                }
                else
                {
                    if (administrator.Ime != null)
                        a.Ime = administrator.Ime;
                    if (administrator.Prezime != null)
                        a.Prezime = administrator.Prezime;
                    if (administrator.Email != null)
                        a.Email = administrator.Email;
                    if (administrator.Lozinka != null)
                        a.Lozinka = _hashService.HashString(administrator.Lozinka);
                    if (slika != null)
                        a.Slika = slika;

                    await _contextScActive.SaveChangesAsync();
                    response = new DbResponse { Status = true, Message = new[] {"Administrator je azuriran."}, Data = new List<Clan> { a } };
                }
            }
            catch(Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }
            return response;
        }

        public async Task<DbResponse> UpdateProduct(int id, string ime, string opis, string cena, int kolicina, int kategorija)
        {
            DbResponse response;
            try
            {
                var product = _contextScActive.Proizvodi.Where(p => p.ID == id).FirstOrDefault();
                if (product == null)
                {
                    response = new DbResponse { Status = false, Message = new[] { "Proizvod nije pronadjen." }, Data = null };
                }
                else
                {
                    product.Ime = ime;
                    product.Opis = opis;
                    product.Cena = cena;
                    product.Kategorija = kategorija;
                    product.Kolicina = kolicina;

                    await _contextScActive.SaveChangesAsync();
                    response = new DbResponse { Status = true, Message = new[] { "Proivod je azuriran." }, Data = (IEnumerable<object>)product  };
                }
            }
            catch (Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }
            return response;
        }

        public async Task<DbResponse> RemoveProduct(int id)
        {
            DbResponse response;
            try
            {
                var product = await _contextScActive.Proizvodi.FindAsync(id);
                if (product == null)
                {
                    response = new DbResponse { Status = false, Message = new[] { "Proizvod nije pronadjen." }, Data = null };
                }
                else
                {
                    _contextScActive.Proizvodi.Remove(product);
                    await _contextScActive.SaveChangesAsync();
                    response = new DbResponse { Status = true, Message = new[] { "Proivod je uklonjen." }, Data = (IEnumerable<object>)product };
                }
            }
            catch (Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }
            return response;
        }

        public async Task<DbResponse> RemoveClan(int id)
        {
            DbResponse response;
            try
            {
                var clan = await _contextScActive.Clanovi.FindAsync(id);
                if (clan == null)
                {
                    response = new DbResponse { Status = false, Message = new[] { "Clan nije pronadjen." }, Data = null };
                }
                else
                {
                    _contextScActive.Clanovi.Remove(clan);
                    await _contextScActive.SaveChangesAsync();
                    response = new DbResponse { Status = true, Message = new[] { "Clan je uklonjen." }, Data = (IEnumerable<object>)clan };
                }
            }
            catch (Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }
            return response;
        }

        public async Task<DbResponse> GetPosts(int page, int count, int clanid = -1)
        {
            DbResponse response;
            var result = new List<Post>();
            try
            {
                var Clans = _contextScActive.Clanovi;
                if (clanid != -1)
                    result = _contextScActive.Posts.Where(x=>x.ClanID == clanid).OrderByDescending(x=>x.Created).Skip(page * count).Take(count).SelectMany(post => Clans.Where(y=>y.ID == clanid).Select(x=> new Post()
                    {
                        ID = post.ID,
                        Clan = x,
                        ClanID = clanid,
                        Created = post.Created,
                        Message = post.Message
                    }) ).ToList();
                else
                {
                    result.AddRange(_contextScActive.Posts.OrderByDescending(x=>x.Created).Skip(page * count).Take(count).Select(post=> new Post()
                    {
                        Clan = Clans.Where(x=>x.ID == post.ClanID).FirstOrDefault(),
                        ID= post.ID,
                        ClanID = post.ClanID,
                        Created = post.Created,
                        Message = post.Message 
                    }));
                }
                if (!result.Any())
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = result.ToList() };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> SearchThings(string value)
        {
            DbResponse response;
            var result = new List<object>();
            try
            {
                var Clans = _contextScActive.Clanovi;
                var Proizvodi = _contextScActive.Proizvodi;

                
                result.AddRange(Clans.Include(x=>x.Sport).Where(x=>x.Ime.Contains(value) || x.Prezime.Contains(value)  || x.Email.Contains(value) || x.Sport.Ime.Contains(value)).Select(x=> new
                {
                    id = x.ID,
                    type = (int)SearchCategory.User,
                    role = x.Role,
                    title = x.Ime + " " + x.Prezime,
                    image = x.Slika,
                    content = x.Sport.Ime,
                }));
                result.AddRange(Proizvodi.Where(x=>x.Ime.Contains(value)).Select(x=> new
                {
                    id = x.ID,
                    type = (int)SearchCategory.Product,
                    role = -1,
                    title = x.Ime,
                    image = x.Slika0,
                    content = x.Cena,
                }));
                if (!result.Any())
                    response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
                else
                    response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = result.ToList() };
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }
        public async Task<DbResponse> AddPicToUser(string value,int id)
        {
            DbResponse response;
            try
            { 
                
               var c = _contextScActive.Clanovi.Where(x=>x.ID == id).FirstOrDefault();
               if (c != null)
               {
                   c.Slika = value;
                   _contextScActive.Clanovi.Update(c);
                   await _contextScActive.SaveChangesAsync();
                   response = new DbResponse { Status = true, Message = new[] { "Success" }, Data = new[] { new { id } } };
               }
               else
               {
                   response = new DbResponse { Status = false, Message = new[] { "Not found" }, Data = null };
               }
            }
            catch (Exception e)
            {
                response = new DbResponse { Status = false, Message = new[] { e.Message }, Data = null };
            }

            return response;
        }

        public async Task<DbResponse> ObrisiZahteve(int id)
        {
            DbResponse response;
            try
            {
                var clan = _contextScActive.Clanovi.Where(x => x.ID == id).FirstOrDefault();
                var zpt = _contextScActive.ZahtevPersonalniTreners.Where(x => x.CID == id).ToList();

                if (clan == null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Clan ne postoji." }, Data = null };
                }
                if (zpt == null)
                {
                    return new DbResponse { Status = false, Message = new[] { "Zahtevu ne postoje." }, Data = null };
                }

                foreach(var pom in zpt)
                {
                    _contextScActive.ZahtevPersonalniTreners.Remove(pom);
                }
                await _contextScActive.SaveChangesAsync();
                response = new DbResponse { Status = true, Message = new[] { "Zahtevi su obrisani." }, Data = null };
            }
            catch (Exception ex)
            {
                response = new DbResponse { Status = false, Message = new[] { ex.Message }, Data = null };
            }
            return response;
        }
    }
}