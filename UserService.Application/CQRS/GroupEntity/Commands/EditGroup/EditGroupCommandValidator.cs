using FluentValidation;

namespace UserService.Application.CQRS.GroupEntity.Commands.EditGroup;

public class EditGroupCommandValidator : AbstractValidator<EditGroupCommand>
{
    public EditGroupCommandValidator()
    {
        RuleFor(x => x.CuratorId).NotEqual(Guid.Empty);
        RuleFor(x => x.CurrentCourse).InclusiveBetween(byte.MinValue, byte.MaxValue);
        RuleFor(x => x.CurrentSemester).InclusiveBetween(byte.MinValue, byte.MaxValue);
        RuleFor(x => x.SubGroup).InclusiveBetween(byte.MinValue, byte.MaxValue);
    }
}