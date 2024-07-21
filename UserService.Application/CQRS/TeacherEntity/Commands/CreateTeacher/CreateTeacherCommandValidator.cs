using FluentValidation;

namespace UserService.Application.CQRS.TeacherEntity.Commands.CreateTeacher;

public class CreateTeacherCommandValidator : AbstractValidator<CreateTeacherCommand>
{
    public CreateTeacherCommandValidator()
    {
        RuleFor(x => x.FirstName).NotNull().NotEmpty().MaximumLength(32);
        RuleFor(x => x.LastName).NotNull().NotEmpty().MaximumLength(32);
        RuleFor(x => x.PatronymicName).MaximumLength(32);
        RuleFor(x => x.SsoId).NotEqual(Guid.Empty);
    }
}