using FluentValidation;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;

public class DropOutStudentsCommandValidator : AbstractValidator<DropOutStudentsCommand>
{
    public DropOutStudentsCommandValidator()
    {
        RuleForEach(x => x.StudentIds).NotEqual(Guid.Empty);
        RuleFor(x => x.DroppedOutTime).NotEqual(DateTime.MinValue);
    }
}
