using FluentValidation;

namespace UserService.Application.CQRS.Group.Queries.GetGroupById;

public class GetGroupByIdQueryValidator : AbstractValidator<GetGroupByIdQuery>
{
    public GetGroupByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotNull();
    }
}