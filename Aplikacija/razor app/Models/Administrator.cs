using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Administrator")]
    public class Administrator
    {
        [Key] public int ID { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Lozinka { get; set; }
        [NotMapped]
        public string Slika { get; set; }

        public Administrator()
        {
            
        }

        public Administrator(Clan c)
        {
            ID = c.ID;
            Ime = c.Ime;
            Prezime = c.Prezime;
            Email = c.Email;
            Lozinka = c.Lozinka;
            Slika = c.Slika;
        }
    }
}