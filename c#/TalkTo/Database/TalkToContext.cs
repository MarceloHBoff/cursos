using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalkTo.Models;

namespace TalkTo.Database
{
    public class TalkToContext : IdentityDbContext<User>
    {
        public TalkToContext(DbContextOptions<TalkToContext> options) : base(options) { }

        public DbSet<Message> Message { get; set; }
        public DbSet<Token> Token { get; set; }
    }
}