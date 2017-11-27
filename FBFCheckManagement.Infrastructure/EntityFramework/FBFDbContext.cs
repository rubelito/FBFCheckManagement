using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
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
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
    }
}