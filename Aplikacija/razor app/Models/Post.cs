using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Post")]
    public class Post
    {
        [Key] public int ID { get; set; }
        
        public int ClanID { get; set; }
        [Required]public string Message { get; set; }
        
        public DateTime Created { get; set; }
        
        [NotMapped]
        public Clan Clan { get; set; }
    }
}