using FluentValidation;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;

public class GetStudentBySsoIdQueryValidator : AbstractValidator<GetStudentBySsoIdQuery>
{
    public GetStudentBySsoIdQueryValidator()
    {
        RuleFor(x => x.SsoId).NotEqual(Guid.Empty);
    }
}
