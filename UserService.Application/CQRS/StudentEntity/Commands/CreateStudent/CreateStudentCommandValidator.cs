using FluentValidation;

namespace UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().NotNull().MaximumLength(32).Matches(@"\A\S+\z");
        RuleFor(x => x.LastName).NotEmpty().NotNull().MaximumLength(32).Matches(@"\A\S+\z");
        RuleFor(x => x.PatronymicName).MaximumLength(32).Matches(@"\A\S+\z");
    }
}
