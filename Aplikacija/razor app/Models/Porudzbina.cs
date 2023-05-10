using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Porudzbina")]
    public class Porudzbina
    {
        [Key] public int ID { get; set; }
        public int IDKorisnika { get; set; }
        public string Narucilac { get; set; }
        public string Adresa { get; set; }
        public string BrojTelefona { get; set; }
        public List<Proizvod> Proizvodi { get; set; }
        public DateTime DatumPorudzbine { get; set; }
    }
}