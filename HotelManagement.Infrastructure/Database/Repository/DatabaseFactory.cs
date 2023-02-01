using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelManagement.Infrastructure
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly DbContext _dataContext;
        private readonly IConfiguration _configuration;

        public DatabaseFactory() => _dataContext = new HotelDbContext();

        public DbContext GetDbContext() => _dataContext;
    }
}
