using azure_functions.Domain;
using Microsoft.EntityFrameworkCore;

namespace azure_functions.Infrastructure
{
    public class NoteDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public NoteDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
