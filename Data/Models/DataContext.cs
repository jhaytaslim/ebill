using Microsoft.EntityFrameworkCore;
using ebill.Data.Models;

namespace ebill.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    public DbSet<Product> products { get; set; }
    public DbSet<Item> items { get; set; }
    public DbSet<Settings> settings { get; set; }
}