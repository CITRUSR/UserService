using FluentValidation;

namespace UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;

public class GraduateGroupsCommandValidator : AbstractValidator<GraduateGroupsCommand>
{
    public GraduateGroupsCommandValidator()
    {
        RuleFor(x => x.GraduatedTime).NotEqual(DateTime.MinValue);
        RuleFor(x => x.GroupsId).NotEmpty();
    }
}
