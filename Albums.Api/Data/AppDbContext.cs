using MongoDB.Driver;

namespace Albums.Api.Data;

public class AppDbContext : IAppDbContext
{
    private readonly IMongoDatabase _database;
    
    public AppDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Album> Albums => _database.GetCollection<Album>("albums");
}
