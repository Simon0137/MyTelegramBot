using Microsoft.EntityFrameworkCore;

namespace SimonUtils.DbUtil
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=AppDB.db;Cache=Shared");
        }
    }
}
