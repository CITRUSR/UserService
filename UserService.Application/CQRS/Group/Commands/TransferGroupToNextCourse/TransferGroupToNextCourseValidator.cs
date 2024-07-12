using FluentValidation;

namespace UserService.Application.CQRS.Group.Commands.TransferGroupToNextCourse;

public class TransferGroupToNextCourseValidator : AbstractValidator<TransferGroupToNextCourseCommand>
{
    public TransferGroupToNextCourseValidator()
    {
        RuleFor(x => x.GroupId).NotNull();
    }
}