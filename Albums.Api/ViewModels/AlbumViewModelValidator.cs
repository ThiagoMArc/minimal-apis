using FluentValidation;

namespace Albums.Api.ViewModels.Album
{
    public class AlbumViewModelValidator : AbstractValidator<AlbumViewModel>
    {
        public AlbumViewModelValidator()
        {
            RuleFor(a => a.Title).NotEmpty().WithMessage("Album must have a title");
            RuleFor(a => a.Artist).NotEmpty().WithMessage("Artist must have a title");
            RuleFor(a => a.Year).GreaterThan(0).WithMessage("Year must be a positive number");
            RuleFor(a => a.TrackList).NotEmpty().WithMessage("Tracklist must be informed");
            RuleForEach(a => a.TrackList).NotEmpty().WithMessage("Name of the track can not be empty"); 
        }
    }
}