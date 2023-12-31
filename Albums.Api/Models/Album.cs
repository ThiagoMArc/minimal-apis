using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Album 
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id {get; set;} = "";

    [BsonElement("title")]
    public string Title {get; set;} = "";

    [BsonElement("artist")] 
    public string Artist {get; set;} = "";

    [BsonElement("year")]
    public int Year {get; set;}
}