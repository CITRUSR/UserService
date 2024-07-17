using FluentValidation;

namespace UserService.Application.CQRS.StudentEntity.Commands.EditStudent;

public class EditStudentCommandValidator : AbstractValidator<EditStudentCommand>
{
    public EditStudentCommandValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.FirstName).NotEmpty().NotNull().MaximumLength(32).Matches(@"\A\S+\z");
        RuleFor(x => x.LastName).NotEmpty().NotNull().MaximumLength(32).Matches(@"\A\S+\z");
        RuleFor(x => x.PatronymicName).MaximumLength(32).Matches(@"\A\S+\z");
        RuleFor(x => x.GroupId).NotNull();
    }
}