using FluentValidation;

namespace UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.CuratorId).NotNull();
        RuleFor(x => x.SpecialityId).NotNull();
        RuleFor(x => x.CurrentCourse).NotNull().InclusiveBetween((byte)1, byte.MaxValue);
        RuleFor(x => x.CurrentSemester).NotNull().InclusiveBetween((byte)1, byte.MaxValue);
        RuleFor(x => x.SubGroup).NotNull().InclusiveBetween((byte)1, byte.MaxValue);
        RuleFor(x => x.StartedAt).NotNull();
    }
}