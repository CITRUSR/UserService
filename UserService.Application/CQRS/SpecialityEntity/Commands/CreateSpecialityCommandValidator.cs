using FluentValidation;

namespace UserService.Application.CQRS.SpecialityEntity.Commands;

public class CreateSpecialityCommandValidator : AbstractValidator<CreateSpecialityCommand>
{
    public CreateSpecialityCommandValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(256);
        RuleFor(x => x.Abbreavation).NotNull().NotEmpty().MaximumLength(10);
        RuleFor(x => x.DurationMonths).GreaterThanOrEqualTo((byte)1);
    }
}