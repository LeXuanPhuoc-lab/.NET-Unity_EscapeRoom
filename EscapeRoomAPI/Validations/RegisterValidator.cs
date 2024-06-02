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
    }
}