using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentsByGroupId;

public record GetStudentsByGroupIdQuery(int GroupId) : IRequest<List<StudentViewModel>>;
