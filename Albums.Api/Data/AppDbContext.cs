using Albums.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Albums.Api.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    private string _dbConn = "";
    public DbSet<Album> Albums { get; set; }  

    public AppDbContext(string connection)
    {
        _dbConn = connection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
         => options.UseNpgsql(_dbConn); 
}
