using FluentValidation;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;

public class TransferGroupsToNextSemesterCommandValidator : AbstractValidator<TransferGroupsToNextSemesterCommand>
{
    public TransferGroupsToNextSemesterCommandValidator()
    {
        RuleFor(x => x.IdGroups).NotNull().NotEmpty();
    }
}