﻿using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.Group.Commands.CreateGroup;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Group.Commands;

public class CreateGroup : CommonTest
{
    [Fact]
    public async void CreateGroup_ShouldBe_Success()
    {
        ClearDataBase();

        var speciality = Fixture.Create<Speciality>();
        var curator = Fixture.Create<Teacher>();

        await Context.Specialities.AddAsync(speciality);
        await Context.Teachers.AddAsync(curator);

        var command = Fixture.Build<CreateGroupCommand>().With(x => x.CuratorId, curator.Id)
            .With(x => x.SpecialityId, speciality.Id).Create();

        var handler = new CreateGroupCommandHandler(Context);

        var groupId = await handler.Handle(command, CancellationToken.None);
        Context.Groups.Should().HaveCount(1);
    }

    [Fact]
    public async void CreateGroup_ShouldBe_SpecialityNotFoundException()
    {
        ClearDataBase();

        var curator = Fixture.Create<Teacher>();

        await Context.Teachers.AddAsync(curator);

        var command = Fixture.Build<CreateGroupCommand>().With(x => x.CuratorId, curator.Id).Create();

        var handler = new CreateGroupCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    [Fact]
    public async void CreateGroup_ShouldBe_TeacherNotFoundException()
    {
        ClearDataBase();

        var speciality = Fixture.Create<Speciality>();

        await Context.Specialities.AddAsync(speciality);

        var command = Fixture.Build<CreateGroupCommand>().With(x => x.SpecialityId, speciality.Id).Create();

        var handler = new CreateGroupCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<TeacherNotFoundException>();
    }
}