using Albums.Api.Models;

namespace Albums.Api.Data;
public class AlbumRepository : IAlbumRepository
{
    private readonly IAppDbContext _context;

    public AlbumRepository(IAppDbContext context)
    {
        _context = context;
    }

    public void Add(Album album) => _context.Albums.Add(album);

    public void Delete(Album album) => _context.Albums.Remove(album);

    public IQueryable<Album> GetAll() => _context.Albums.AsQueryable();
    
    public async Task<Album?> GetByIdAsync(Guid id) => await _context.Albums.FindAsync(id);

    public async Task<int> SaveChangesAsync() => await (_context as AppDbContext).SaveChangesAsync();

    public void Update(Album album) => _context.Albums.Update(album);
    
    public IEnumerable<Album> Where(Func<Album, bool> predicate) => _context.Albums.Where(predicate);
}
