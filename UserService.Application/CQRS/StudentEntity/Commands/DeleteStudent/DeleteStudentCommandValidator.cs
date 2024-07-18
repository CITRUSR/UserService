using FluentValidation;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;

public class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentCommandValidator()
    {
        RuleFor(x => x.Id).NotNull();
    }
}