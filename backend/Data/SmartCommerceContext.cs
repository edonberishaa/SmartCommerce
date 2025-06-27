using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class SmartCommerceContext : DbContext
    {
        public SmartCommerceContext(DbContextOptions<SmartCommerceContext> options) : base(options)
        {
        }
        public DbSet<Sale> Sales { get; set; }
    }
}
