using Grpc.Core;
using MediatR;
using UserService.API.Mappers;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;
using UserService.Domain.Entities;

namespace UserService.API.Services;

public class SpecialityService(
    IMediator mediator,
    IMapper<Speciality, SpecialityModel> mapper,
    IMapper<Speciality, SpecialityViewModel> mapperViewModel
) : UserService.SpecialityService.SpecialityServiceBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper<Speciality, SpecialityModel> _mapper = mapper;
    private readonly IMapper<Speciality, SpecialityViewModel> _mapperViewModel = mapperViewModel;

    public override async Task<CreateSpecialityResponse> CreateSpeciality(
        CreateSpecialityRequest request,
        ServerCallContext context
    )
    {
        decimal cost = new CustomTypes.DecimalValue(request.Cost.Units, (int)request.Cost.Nanos);

        var command = new CreateSpecialityCommand(
            request.Name,
            request.Abbreavation,
            cost,
            (byte)request.DurationMonths
        );

        var id = await _mediator.Send(command);

        return new CreateSpecialityResponse { Id = id, };
    }

    public override async Task<DeleteSpecialityResponse> DeleteSpeciality(
        DeleteSpecialityRequest request,
        ServerCallContext context
    )
    {
        var command = new DeleteSpecialityCommand([.. request.Ids]);

        var specialities = await _mediator.Send(command);

        return new DeleteSpecialityResponse
        {
            Specialities = { specialities.Select(x => _mapperViewModel.Map(x)) },
        };
    }

    public override async Task<SpecialityModel> GetSpecialityById(
        GetSpecialityByIdRequest request,
        ServerCallContext context
    )
    {
        var query = new GetSpecialityByIdQuery(request.Id);

        var speciality = await _mediator.Send(query);

        return _mapper.Map(speciality);
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
            DeletedStatus =
                (Application.CQRS.SpecialityEntity.Queries.GetSpecialities.SpecialityDeletedStatus)
                    request.DeletedStatus,
        };

        var specialities = await _mediator.Send(query);

        return new GetSpecialitiesResponse
        {
            Specialities = { specialities.Items.Select(x => _mapper.Map(x)) },
            LastPage = specialities.MaxPage,
        };
    }

    public override async Task<SoftDeleteSpecialitiesResponse> SoftDeleteSpecialities(
        SoftDeleteSpecialitiesRequest request,
        ServerCallContext context
    )
    {
        var command = new SoftDeleteSpecialitiesCommand([.. request.Ids]);

        var specialities = await _mediator.Send(command);

        return new SoftDeleteSpecialitiesResponse
        {
            Specialities = { specialities.Select(x => _mapperViewModel.Map(x)) },
        };
    }

    public override async Task<EditSpecialityResponse> EditSpeciality(
        EditSpecialityRequest request,
        ServerCallContext context
    )
    {
        decimal cost = new CustomTypes.DecimalValue(request.Cost.Units, (int)request.Cost.Nanos);

        var command = new EditSpecialityCommand(
            request.Id,
            request.Name,
            request.Abbreavation,
            cost,
            (byte)request.DurationMonths,
            request.IsDeleted
        );

        var id = await _mediator.Send(command);

        return new EditSpecialityResponse { Id = id, };
    }
}
