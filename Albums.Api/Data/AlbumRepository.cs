
using MongoDB.Driver;

namespace Albums.Api.Data;
public class AlbumRepository : IAlbumRepository
{
    private readonly IAppDbContext _context;

    public AlbumRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Create(Album album)
    {
        await _context.Albums.InsertOneAsync(album);
    }

    public async Task Delete(string id)
    {
        var filter = FindById(id);
        await _context.Albums.DeleteOneAsync(filter);
    }

    public async Task<IEnumerable<Album>> GetAll()
    {
        return await _context.Albums.Find(_ => true).ToListAsync();
    }   

    public async Task<Album> GetById(string id)
    {
        var filter = FindById(id);
        return await _context.Albums.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Album>> Where(FilterDefinition<Album> filter)
    {
        return await _context.Albums.Find(filter).ToListAsync();
    }

    public async Task Update(string id, Album album)
    {
        await _context.Albums.ReplaceOneAsync(m => m.Id == id, album);
    }

    private FilterDefinition<Album> FindById(string id)
    {
        return Builders<Album>.Filter.Eq(m => m.Id, id);
    }
}
