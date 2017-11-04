using System.Data.Common;

namespace FBFCheckManagement.Infrastructure.EntityFramework
{
    public interface IDatabaseType
    {
        DbConnection Connectionstring();
    }
}