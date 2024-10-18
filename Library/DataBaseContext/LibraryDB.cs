using Library.Model;
using Microsoft.EntityFrameworkCore;

namespace Library.DataBaseContext
{
    public class LibraryDB : DbContext
    {
        public LibraryDB(DbContextOptions options) : base(options) 
        {

        }

        public DbSet<Books> Books { get; set; }
        public DbSet<Genres> Genres { get; set; }
        public DbSet<Readers> Readers { get; set; }
        public DbSet<Rent_story> Rent_story { get; set;}
    }
}
