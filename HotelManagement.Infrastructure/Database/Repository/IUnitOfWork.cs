using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;

        int Save();

        Task<int> SaveAsync(CancellationToken cancellationToken = default(CancellationToken));

        bool CheckConnection();

        List<dynamic> SelectByCommand(string rawSQL, List<SqlParameter> listParameter);

        DbContext GetDbContext();
    }
}
