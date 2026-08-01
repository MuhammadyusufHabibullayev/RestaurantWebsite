using Microsoft.EntityFrameworkCore;

namespace RestaurantWebsite.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    public DbSet<MenuItem> MenuItems {  get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }
    public DbSet<TableReservation> TableReservations {  get; set; }

    }
}
