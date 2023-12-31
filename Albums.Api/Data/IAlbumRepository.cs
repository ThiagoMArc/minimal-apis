using Albums.Api.Models;

namespace Albums.Api.Data;
public interface IAlbumRepository
{
    IQueryable<Album> GetAll();
    Task<Album?> GetByIdAsync(Guid id);
    IEnumerable<Album> Where(Func<Album, bool> predicate);
    void Add(Album album);
    void Update(Album album);
    void Delete(Album album);
    Task<int> SaveChangesAsync();
}
