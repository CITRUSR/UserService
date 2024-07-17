using FluentValidation;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;

public class DropOutStudentCommandValidator : AbstractValidator<DropOutStudentCommand>
{
    public DropOutStudentCommandValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.DroppedOutTime).NotNull();
    }
}