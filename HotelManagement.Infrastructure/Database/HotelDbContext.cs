using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;


namespace HotelManagement.Infrastructure
{
    public class HotelDbContext : DbContext
    {
        private string connectionString;
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }
        private static readonly ILoggerFactory loggerFactory = new LoggerFactory(new[] {
              new DebugLoggerProvider()
        });
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<RoomBooking> RoomBookings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomCategory> RoomCategories { get; set; }
        public DbSet<History> Historys { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemStorage> ItemStorages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<HtUser> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Right> Rights { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<RegisterCode> RegisterCodes { get; set; }
        public DbSet<RightMapRole> RightMapRoles { get; set; }
        public DbSet<RightMapUser> RightMapUsers { get; set; }
        public DbSet<UserMapRole> UserMapRoles { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }

        public HotelDbContext()
        {
            connectionString = Utils.GetConfig("ConnectionStrings:DefaultConnection");
            Console.WriteLine(connectionString);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            optionsBuilder.UseLoggerFactory(loggerFactory).EnableSensitiveDataLogging();
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.ApplyConfigurationsFromAssembly(typeof(HotelDbContext).Assembly);
        //}
    }
}
