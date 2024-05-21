using EscapeRoomAPI.Payloads.Requests;
using FluentValidation;

namespace EscapeRoomAPI.Validations;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        // Adding validations

        RuleFor(x => x.Username)
            .NotNull()
            .NotEmpty()
            .WithMessage("Chưa nhập tài khoản kìa (•‿•)");
        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty()
            .WithMessage("Chưa nhập mật khẩu kìa (¬‿¬)");
        RuleFor(x => x.Email)
            // Only apply email format whenever email is not empty
            .NotEmpty().When(x => x.Email != null)
            .EmailAddress()
            .WithMessage("Wrong email format");
    }
}