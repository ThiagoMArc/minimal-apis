using MongoDB.Driver;

namespace Albums.Api.Data;
public interface IAlbumRepository
{
    Task Create(Album movie);
    Task Update(string id, Album movie);
    Task Delete(string id);
    Task<Album> GetById(string id);
    Task<IEnumerable<Album>> GetAll();
    Task<IEnumerable<Album>> Where(FilterDefinition<Album> filter);
}
