using FluentValidation;

namespace Albums.Api.ViewModels
{
    public class AlbumUpdateViewModelValidator : AbstractValidator<AlbumUpdateViewModel>
    {
        public AlbumUpdateViewModelValidator()
        {
            RuleFor(a => a.Artist).NotEmpty().WithMessage("Artist must be informed");
            RuleFor(a => a.Title).NotEmpty().WithMessage("Album title must be informed");
            RuleFor(a => a.Year).GreaterThan(0).WithMessage("Year must be a positive number");
            RuleFor(a => a.TrackList).NotEmpty().WithMessage("Tracklist must be informed");
            RuleForEach(a => a.TrackList).NotEmpty().WithMessage("Track name must be informed");
        }
    }
}