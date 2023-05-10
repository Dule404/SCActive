using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    public class DbContextSCActive : DbContext
    {
        public DbSet<Clan> Clanovi { get; set; }
        public DbSet<Porudzbina> Porudzbine { get; set; }
        public DbSet<Proizvod> Proizvodi { get; set; }
        public DbSet<Sport> Sportovi { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ZahtevPersonalniTrener> ZahtevPersonalniTreners { get; set; }
        public DbSet<Kontakt> Kontakti { get; set; }
        public DbContextSCActive(DbContextOptions options) : base(options)
        {
        }
    }
}