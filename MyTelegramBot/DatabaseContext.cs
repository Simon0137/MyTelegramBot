using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace MyTelegramBot
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<MyUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=AppDB.db;Cache=Shared");
        }
    }
}
