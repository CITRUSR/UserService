using FluentValidation;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;

public class DropOutStudentCommandValidator : AbstractValidator<DropOutStudentCommand>
{
    public DropOutStudentCommandValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.DroppedOutTime).NotEqual(DateTime.MinValue);
    }
}
