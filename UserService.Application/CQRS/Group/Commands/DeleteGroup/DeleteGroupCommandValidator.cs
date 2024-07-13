using FluentValidation;

namespace UserService.Application.CQRS.Group.Commands.DeleteGroup;

public class DeleteGroupCommandValidator : AbstractValidator<DeleteGroupCommand>
{
    public DeleteGroupCommandValidator()
    {
        RuleFor(x => x.Id).NotNull();
    }
}