using FluentValidation;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;

public class GetGroupByIdQueryValidator : AbstractValidator<GetGroupByIdQuery>
{
    public GetGroupByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotNull();
    }
}