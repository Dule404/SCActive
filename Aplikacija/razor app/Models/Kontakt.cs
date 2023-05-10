using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Kontakt")]
    public class Kontakt
    {
        [Key] public int ID { get; set; }
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$")]
        [Required,DataType(DataType.EmailAddress)] public string Email { get; set; }
        [Required] public string Poruka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
    }
}