using FluentValidation;

namespace UserService.Application.Student.Commands.CreateStudent;

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().NotNull().MaximumLength(32).Matches(@"\A\S+\z");
        RuleFor(x => x.LastName).NotEmpty().NotNull().MaximumLength(32).Matches(@"\A\S+\z");
        RuleFor(x => x.PatronymicName).MaximumLength(32).Matches(@"\A\S+\z");
        RuleFor(x => x.GroupId).NotNull();
    }
}