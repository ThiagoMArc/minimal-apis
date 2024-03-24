namespace Albums.Api.ViewModels;

public class AlbumViewModel
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public int Year { get; set; }
    public List<string> TrackList { get; set; }

    public Models.Album ToEntity()
    {       
        return new Models.Album
        {
            Id = Guid.NewGuid(), 
            Title = Title, 
            Artist = Artist, 
            Year = Year,
            Tracklist = TrackList
        };
    }
}