using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Proizvod")]
    public class Proizvod
    {
        [Key] public int ID { get; set; }
        public string Ime { get; set; }
        public string Cena { get; set; }
        public int Kategorija { get; set; }
        public string Opis { get; set; }
        public string Slika0 { get; set; }
        public string? Slika1 { get; set; }
        public string? Slika2 { get; set; }
        public string? Slika3 { get; set; }
        
        public string Velicina { get; set; }
        public int Kolicina { get; set; }
        public bool Kvarljivo { get; set; }
        public DateTime? DatumIstekaRoka { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}