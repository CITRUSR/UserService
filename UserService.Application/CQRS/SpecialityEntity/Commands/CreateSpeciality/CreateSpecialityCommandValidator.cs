using FluentValidation;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;

public class CreateSpecialityCommandValidator : AbstractValidator<CreateSpecialityCommand>
{
    public CreateSpecialityCommandValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(256);
        RuleFor(x => x.Abbreavation).NotNull().NotEmpty().MaximumLength(10);
        RuleFor(x => x.DurationMonths).InclusiveBetween((byte)1, byte.MaxValue);
    }
}
