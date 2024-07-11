using FluentValidation;

namespace UserService.Application.CQRS.Student.Queries.GetStudentBySsoId;

public class GetStudentBySsoIdQueryValidator : AbstractValidator<GetStudentBySsoIdQuery>
{
    public GetStudentBySsoIdQueryValidator()
    {
        RuleFor(x => x.SsoId).NotNull();
    }
}