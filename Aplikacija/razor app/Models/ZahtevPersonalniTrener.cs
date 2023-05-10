using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("ZahtevPersonalniTrener")]
    public class ZahtevPersonalniTrener
    {
        [Key]
        public int ID { get; set; }

        public int PTID { get; set; }

        public int CID { get; set; }
        [NotMapped]
        public Clan Clan { get; set; }
        [NotMapped]
        public PersonalniTrener PersonalniTrener { get; set; }

    }
}