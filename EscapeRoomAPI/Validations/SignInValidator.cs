using EscapeRoomAPI.Extensions;
using EscapeRoomAPI.Payloads.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EscapeRoomAPI.Validations;

public class SignInValidator : AbstractValidator<SignInRequest>
{
    public SignInValidator()
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
