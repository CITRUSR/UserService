using FluentValidation;

namespace UserService.Application.CQRS.Student.Commands.DropOutStudent;

public class DropOutStudentCommandValidator : AbstractValidator<DropOutStudentCommand>
{
    public DropOutStudentCommandValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.DroppedOutTime).NotNull();
    }
}