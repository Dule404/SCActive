using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using backend.Enums;

namespace backend.Models
{
    [Table("Clan")]
    public class Clan
    {
        [Key] public int ID { get; set; }
        public int Role { get; set; }
        [Required] public string Ime { get; set; }
        [Required] public string Prezime { get; set; }
        [Required] public DateTime DatumRodjenja { get; set; }
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$")]
        [Required,DataType(DataType.EmailAddress)] public string Email { get; set; }
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")]
        [Required,MinLength(8)] public string Lozinka { get; set; }
        public Sport Sport { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public string Telefon { get; set; }
        
        public string Slika { get; set; }
        
        [JsonIgnore]
        public List<Clan> ListaKlijenata { get; set; }

        [JsonIgnore]
        public List<ZahtevPersonalniTrener> ListaZahteva { get; set; }

        public Clan()
        {
            
        }

        public Clan(PersonalniTrener pt)
        {
            ID = pt.ID;
            Role = (int)UserCategory.PersonalTrainer;
            Ime = pt.Ime;
            Prezime = pt.Prezime;
            DatumRodjenja = pt.DatumRodjenja;
            Email = pt.Email;
            Lozinka = pt.Lozinka;
            Sport = pt.Sport;
            DatumRegistracije = pt.DatumRegistracije;
            Telefon = pt.Telefon;
            ListaKlijenata = pt.ListaKlijenata;
            ListaZahteva = pt.ListaZahteva;
            Slika = pt.Slika;
        }
        public Clan(Administrator pt)
        {
            ID = pt.ID;
            Role = (int)UserCategory.Administrator;
            Ime = pt.Ime;
            Prezime = pt.Prezime;
            Email = pt.Email;
            Lozinka = pt.Lozinka;
            Slika = pt.Slika;
        }
    }
}