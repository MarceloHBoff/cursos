using Microsoft.EntityFrameworkCore;
using Mimic.Models;

namespace Mimic.Database
{
    public class MimicContext : DbContext
    {
        public MimicContext(DbContextOptions<MimicContext> options) : base(options) { }

        public DbSet<Word> Words { get; set; }
    }
}