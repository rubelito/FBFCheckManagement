namespace FBFCheckManagement.Infrastructure.EntityFramework
{
    public class DbCreator
    {
        public void Create(){
            var connection = new EfSQLite("SQLiteDb");
            using (var ams = new FBFDbContext(connection))
            {
                if (ams.Database.Exists())
                {
                    ams.Database.Delete();
                }
                ams.Database.Create();
            }
        }
    }
}