using FluentValidation;

namespace UserService.Application.CQRS.Group.Commands.GraduateGroup;

public class GraduateGroupCommandValidator : AbstractValidator<GraduateGroupCommand>
{
    public GraduateGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).NotNull();
        RuleFor(x => x.GraduatedTime).NotNull();
    }
}