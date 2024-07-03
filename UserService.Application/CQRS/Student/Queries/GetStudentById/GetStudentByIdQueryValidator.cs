using FluentValidation;
using UserService.Application.CQRS.Student.Quereis;

namespace UserService.Application.CQRS.Student.Queries.GetStudent;

public class GetStudentByIdQueryValidator : AbstractValidator<GetStudentByIdQuery>
{
    public GetStudentByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotNull();
    }
}