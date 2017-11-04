using System.Data.Entity;
using FBFCheckManagement.Application.Domain;

namespace FBFCheckManagement.Infrastructure.EntityFramework
{
    public class FBFDbContext : DbContext
    {
        //public FBFDbContext()
        //{


        //}

        public FBFDbContext(IDatabaseType databaseType) :
            base(databaseType.Connectionstring(), true)
        {

        }

        public DbSet<Check> Checks { get; set; }
        public DbSet<Bank> Banks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}