using FluentValidation;

namespace UserService.Application.CQRS.Teacher.Commands.CreateTeacher;

public class CreateTeacherCommandValidator : AbstractValidator<CreateTeacherCommand>
{
    public CreateTeacherCommandValidator()
    {
        RuleFor(x => x.FirstName).NotNull().NotEmpty();
        RuleFor(x => x.LastName).NotNull().NotEmpty();
        RuleFor(x => x.RoomId).NotNull();
        RuleFor(x => x.SsoId).NotNull();
    }
}