using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    [Table("PersonalniTrener")]
    public class PersonalniTrener
    {
        [Key] public int ID { get; set; }
        [Required] public string Ime { get; set; }
        [Required] public string Prezime { get; set; }
        [Required] public DateTime DatumRodjenja { get; set; }
        [Required] public string Email { get; set; }
        [Required,MinLength(8)] public string Lozinka { get; set; }
        [Required] public Sport Sport { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public string Telefon { get; set; }

        [JsonIgnore]
        public List<Clan> ListaKlijenata { get; set; }

        [JsonIgnore]
        public List<ZahtevPersonalniTrener> ListaZahteva { get; set; }
        [NotMapped]
        public string Slika { get; set; }
        public PersonalniTrener()
        {
            
        }
        public PersonalniTrener(Clan pt)
        {
            ID = pt.ID;
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
    }
}