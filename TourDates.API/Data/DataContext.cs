using Microsoft.EntityFrameworkCore;
using TourDates.API.Models;

namespace TourDates.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Value> Values { get; set; }
    }
}