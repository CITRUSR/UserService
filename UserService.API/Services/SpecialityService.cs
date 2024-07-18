using Grpc.Core;
using MediatR;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;

namespace UserService.API.Services;

public class SpecialityService(IMediator mediator) : UserService.SpecialityService.SpecialityServiceBase
{
    private readonly IMediator _mediator = mediator;

    public override async Task<CreateSpecialityResponse> CreateSpeciality(CreateSpecialityRequest request,
        ServerCallContext context)
    {
        decimal cost = new CustomTypes.DecimalValue(request.Cost.Units, request.Cost.Nanos);

        var command = new CreateSpecialityCommand(request.Name, request.Abbreavation,
            cost, (byte)request.DurationMonths);

        var id = await _mediator.Send(command);

        return new CreateSpecialityResponse
        {
            Id = id,
        };
    }
}