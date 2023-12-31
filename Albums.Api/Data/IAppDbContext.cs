using Albums.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Albums.Api.Data;
public interface IAppDbContext
{
    DbSet<Album> Albums { get;}
}
