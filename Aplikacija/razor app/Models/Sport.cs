using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Sport")]
    public class Sport
    {
        [Key] public int ID { get; set; }
        [Required] public string Ime { get; set; }
    }
}