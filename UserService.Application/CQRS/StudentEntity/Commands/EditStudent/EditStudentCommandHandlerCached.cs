using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.EditStudent;

public class EditStudentCommandHandlerCached(
    ICacheService cacheService,
    EditStudentCommandHandler handler
) : IRequestHandler<EditStudentCommand, Student>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly EditStudentCommandHandler _handler = handler;

    public async Task<Student> Handle(
        EditStudentCommand request,
        CancellationToken cancellationToken
    )
    {
        var student = await _handler.Handle(request, cancellationToken);

        await _cacheService.RemoveAsync(
            CacheKeys.ById<Student, Guid>(student.Id),
            cancellationToken
        );

        for (int i = 0; i < CacheConstants.PagesForCaching; i++)
        {
            await _cacheService.RemovePagesWithObjectAsync<Student, Guid>(
                student.Id,
                (cachedStudent, i) => cachedStudent.Id == i,
                cancellationToken
            );
        }

        return student;
    }
}
