using FluentValidation;

namespace UserService.Application.CQRS.Group.Commands.TransferGroupToNextSemester;

public class TransferGroupToNextSemesterCommandValidator : AbstractValidator<TransferGroupToNextSemesterCommand>
{
    public TransferGroupToNextSemesterCommandValidator()
    {
        RuleFor(x => x.GroupId).NotNull();
    }
}