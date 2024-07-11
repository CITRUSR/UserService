using FluentValidation;

namespace UserService.Application.CQRS.Group.Commands.CreateGroup;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.CuratorId).NotNull();
        RuleFor(x => x.SpecialityId).NotNull();
        RuleFor(x => x.CurrentCourse).NotNull();
        RuleFor(x => x.CurrentSemester).NotNull();
        RuleFor(x => x.SubGroup).NotNull();
        RuleFor(x => x.StartedAt).NotNull();
    }
}