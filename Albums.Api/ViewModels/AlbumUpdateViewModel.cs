namespace Albums.Api.ViewModels;
public class AlbumUpdateViewModel
{
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public int? Year { get; set; }
    public List<string>? TrackList { get; set; }  
}
