﻿using Grpc.Core;
using Mapster;
using MediatR;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.RecoverySpecialities;
using UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;

namespace UserService.API.Services;

public class SpecialityService(IMediator mediator)
    : UserService.SpecialityService.SpecialityServiceBase
{
    private readonly IMediator _mediator = mediator;

    public override async Task<SpecialityShortInfo> CreateSpeciality(
        CreateSpecialityRequest request,
        ServerCallContext context
    )
    {
        var command = new CreateSpecialityCommand(
            request.Name,
            request.Abbreviation,
            (decimal)request.Cost,
            (byte)request.DurationMonths
        );

        var speciality = await _mediator.Send(command);

        return speciality.Adapt<SpecialityShortInfo>();
    }

    public override async Task<DeleteSpecialitiesResponse> DeleteSpecialities(
        DeleteSpecialitiesRequest request,
        ServerCallContext context
    )
    {
        var command = new DeleteSpecialitiesCommand([.. request.Ids]);

        var specialities = await _mediator.Send(command);

        return specialities.Adapt<DeleteSpecialitiesResponse>();
    }

    public override async Task<SpecialityModel> GetSpecialityById(
        GetSpecialityByIdRequest request,
        ServerCallContext context
    )
    {
        var query = new GetSpecialityByIdQuery(request.Id);

        var speciality = await _mediator.Send(query);

        return speciality.Adapt<SpecialityModel>();
    }

    public override async Task<GetSpecialitiesResponse> GetSpecialities(
        GetSpecialitiesRequest request,
        ServerCallContext context
    )
    {
        var query = new GetSpecialitiesQuery
        {
            PageSize = request.PageSize,
            Page = request.Page,
            SearchString = request.SearchString,
            SortState =
                (Application.CQRS.SpecialityEntity.Queries.GetSpecialities.SpecialitySortState)
                    request.SortState,
            DeletedStatus = (UserService.Application.Enums.DeletedStatus)request.DeletedStatus,
        };

        var specialities = await _mediator.Send(query);

        return specialities.Adapt<GetSpecialitiesResponse>();
    }

    public override async Task<SoftDeleteSpecialitiesResponse> SoftDeleteSpecialities(
        SoftDeleteSpecialitiesRequest request,
        ServerCallContext context
    )
    {
        var command = new SoftDeleteSpecialitiesCommand([.. request.Ids]);

        var specialities = await _mediator.Send(command);

        return specialities.Adapt<SoftDeleteSpecialitiesResponse>();
    }

    public override async Task<RecoverySpecialitiesResponse> RecoverySpecialities(
        RecoverySpecialitiesRequest request,
        ServerCallContext context
    )
    {
        var command = new RecoverySpecialitiesCommand([.. request.Ids]);

        var specialities = await _mediator.Send(command);

        return specialities.Adapt<RecoverySpecialitiesResponse>();
    }

    public override async Task<SpecialityShortInfo> EditSpeciality(
        EditSpecialityRequest request,
        ServerCallContext context
    )
    {
        var command = new EditSpecialityCommand(
            request.Id,
            request.Name,
            request.Abbreavation,
            (decimal)request.Cost,
            (byte)request.DurationMonths,
            request.IsDeleted
        );

        var speciality = await _mediator.Send(command);

        return speciality.Adapt<SpecialityShortInfo>();
    }
}
