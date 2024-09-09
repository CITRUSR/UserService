using FluentValidation;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudents;

public class DeleteStudentsCommandValidator : AbstractValidator<DeleteStudentsCommand>
{
    public DeleteStudentsCommandValidator()
    {
        RuleForEach(x => x.StudentIds).NotEqual(Guid.Empty);
    }
}
