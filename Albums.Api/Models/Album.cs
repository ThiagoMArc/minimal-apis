namespace Albums.Api.Models;

public class Album 
{
    public Guid Id {get; set;} 
    public string Title {get; set;} 
    public string Artist {get; set;}
    public List<string> Tracklist { get; set; }
    public int Year {get; set;}
}