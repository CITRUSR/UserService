using FluentValidation;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;

public class TransferGroupsToNextCourseCommandValidator : AbstractValidator<TransferGroupsToNextCourseCommand>
{
    public TransferGroupsToNextCourseCommandValidator()
    {
        RuleFor(x => x.IdGroups).NotEmpty();
    }
}