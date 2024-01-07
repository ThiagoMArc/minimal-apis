using Flunt.Notifications;
using Flunt.Validations;

namespace Albums.Api.ViewModels;

public class AlbumViewModel : Notifiable<Notification>
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public int Year { get; set; }
    public List<string> TrackList { get; set; }


    public Album ToEntity()
    {
        Validate();
        
        return new Album
        {
            Title = Title,
            Artist = Artist,
            Year = Year,
            Tracklist = TrackList
        };
    }

    public void Validate()
    {
        AddNotifications(new Contract<Notification>()
                    .Requires()
                    .IsNotEmpty(Title, "Album must have a title")
                    .IsNotEmpty(Artist, "Artist must have a title")
                    .IsGreaterThan(Year, 0, "Year must be greater than 0")
                    .IsNotEmpty(TrackList, "Tracklist must be informed"));
    }

}