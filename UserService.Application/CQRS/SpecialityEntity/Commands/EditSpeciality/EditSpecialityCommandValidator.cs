using FluentValidation;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;

public class EditSpecialityCommandValidator : AbstractValidator<EditSpecialityCommand>
{
    public EditSpecialityCommandValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(256);
        RuleFor(x => x.Abbrevation).NotNull().NotEmpty().MaximumLength(10);
        RuleFor(x => x.DurationMonths).InclusiveBetween((byte)1, byte.MaxValue);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}