using FluentValidation;

namespace EscapeRoomAPI.Validations
{
    public class ImageFileValidator : AbstractValidator<IFormFile>
    {
        public ImageFileValidator()
        {
            RuleFor(x => x.ContentType).NotNull().Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
                .WithMessage("File type '.jpeg / .jpg / .png' are required");
        }
    }
}