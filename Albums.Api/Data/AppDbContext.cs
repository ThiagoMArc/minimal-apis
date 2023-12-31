using Albums.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Albums.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<Album> Albums { get; set; }  

    protected override void OnConfiguring(DbContextOptionsBuilder options)
         => options.UseSqlite("DataSource=app.db;Cache=Shared"); 
}
