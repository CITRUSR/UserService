﻿using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.SpecialityEntity.Queries;

public class GetSpecialityById : CommonTest
{
    [Fact]
    public async void GetSpecialityById_ShouldBe_Success()
    {
        ClearDataBase();

        var speciality = Fixture.Create<Speciality>();

        await Context.Specialities.AddAsync(speciality);
        await Context.SaveChangesAsync();

        var query = new GetSpecialityByIdQuery(speciality.Id);

        var specialityRes = await Action(query);

        Context.Specialities.Find(speciality.Id).Should().BeEquivalentTo(specialityRes);
    }

    [Fact]
    public async void GetSpecialityById_ShouldBe_SpecialityNotFoundException()
    {
        var query = new GetSpecialityByIdQuery(123);

        Func<Task> act = async () => await Action(query);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    private async Task<Speciality> Action(GetSpecialityByIdQuery query)
    {
        var handler = new GetSpecialityByIdQueryHandler(Context);

        return await handler.Handle(query, CancellationToken.None);
    }
}