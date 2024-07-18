using MediatR;

namespace UserService.API.Services;

public class SpecialityService(IMediator mediator) : UserService.SpecialityService.SpecialityServiceBase
{
    private readonly IMediator _mediator = mediator;
}