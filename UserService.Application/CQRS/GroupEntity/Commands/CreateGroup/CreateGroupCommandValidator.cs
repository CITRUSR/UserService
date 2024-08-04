using FluentValidation;

namespace UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.CuratorId).NotEqual(Guid.Empty);
        RuleFor(x => x.CurrentCourse).InclusiveBetween((byte)1, byte.MaxValue);
        RuleFor(x => x.CurrentSemester).InclusiveBetween((byte)1, byte.MaxValue);
        RuleFor(x => x.SubGroup).InclusiveBetween((byte)1, byte.MaxValue);
    }
}
