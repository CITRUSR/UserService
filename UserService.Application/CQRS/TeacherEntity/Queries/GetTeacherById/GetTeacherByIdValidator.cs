using FluentValidation;

namespace UserService.Application.CQRS.TeacherEntity.Queries.GetTeacherById;

public class GetTeacherByIdValidator : AbstractValidator<GetTeacherByIdQuery>
{
    public GetTeacherByIdValidator()
    {
        RuleFor(x => x.TeacherId).NotEqual(Guid.Empty);
    }
}
