using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyTasks.Models;

namespace MyTasks.Database
{
    public class MyTasksContext : IdentityDbContext<User>
    {
        public MyTasksContext(DbContextOptions<MyTasksContext> options) : base(options) { }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<Token> Token { get; set; }
    }
}