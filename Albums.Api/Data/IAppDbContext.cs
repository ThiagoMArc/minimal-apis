using MongoDB.Driver;

namespace Albums.Api.Data;
public interface IAppDbContext
{
    IMongoCollection<Album> Albums {get;}
}
