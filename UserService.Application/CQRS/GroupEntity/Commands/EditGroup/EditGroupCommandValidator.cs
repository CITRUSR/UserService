using FluentValidation;

namespace UserService.Application.CQRS.GroupEntity.Commands.EditGroup;

public class EditGroupCommandValidator : AbstractValidator<EditGroupCommand>
{
    public EditGroupCommandValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.CuratorId).NotNull();
        RuleFor(x => x.SpecialityId).NotNull();
        RuleFor(x => x.CurrentCourse).NotNull().InclusiveBetween(byte.MinValue, byte.MaxValue);
        RuleFor(x => x.CurrentSemester).NotNull().InclusiveBetween(byte.MinValue, byte.MaxValue);
        RuleFor(x => x.SubGroup).NotNull().InclusiveBetween(byte.MinValue, byte.MaxValue);
    }
}