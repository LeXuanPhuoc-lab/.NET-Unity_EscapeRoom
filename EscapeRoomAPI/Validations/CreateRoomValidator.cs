using EscapeRoomAPI.Payloads.Requests;
using FluentValidation;

namespace EscapeRoomAPI.Validations;

public class CreateRoomValidator : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomValidator()
    {
        // Adding validations

        RuleFor(x => x.TotalPlayer)
            .LessThanOrEqualTo(5)
            .WithMessage("Tối đa 5 chiến hữu thôi bạn eyy!");
    }
}