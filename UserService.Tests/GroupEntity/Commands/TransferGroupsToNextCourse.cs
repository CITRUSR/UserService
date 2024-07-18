﻿using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.GroupEntity.Commands;

public class TransferGroupsToNextCourse : CommonTest
{
    [Fact]
    public async void TransferGroupsToNextCourse_ShouldBe_SuccessWithList()
    {
        var courses = await Arrange(2);

        var command = new TransferGroupsToNextCourseCommand
        {
            IdGroups = Context.Groups.Select(x => x.Id).ToList()
        };

        var ids = await Action(command);

        Assert(ids, courses);
    }

    [Fact]
    public async void TransferGroupsToNextCourse_ShouldBe_SuccessWithoutList()
    {
        var courses = await Arrange(2);

        var command = new TransferGroupsToNextCourseCommand();

        var ids = await Action(command);

        Assert(ids, courses);
    }

    [Fact]
    public async void TransferGroupsToNextCourse_ShouldBe_GroupCourseOutOfRangeException()
    {
        var courses = await Arrange(4);

        var command = new TransferGroupsToNextCourseCommand();

        Func<Task> act = () => Action(command);

        await act.Should().ThrowAsync<GroupCourseOutOfRangeException>();
    }

    private async Task<Dictionary<Group, byte>> Arrange(int CurrentCourse)
    {
        ClearDataBase();

        var speciality = Fixture.Build<Speciality>()
            .With(x => x.DurationMonths, 46)
            .Create();

        var group1 = Fixture.Build<Group>()
            .With(x => x.CurrentCourse, CurrentCourse)
            .With(x => x.Speciality, speciality)
            .Create();

        var group2 = Fixture.Build<Group>()
            .With(x => x.CurrentCourse, CurrentCourse)
            .With(x => x.Speciality, speciality)
            .Create();

        await Context.Groups.AddAsync(group1);
        await Context.Groups.AddAsync(group2);
        await Context.SaveChangesAsync(CancellationToken.None);

        return Context.Groups.ToDictionary(x => x, x => x.CurrentCourse);
    }

    private async Task<List<int>> Action(TransferGroupsToNextCourseCommand command)
    {
        var handler = new TransferGroupsToNextCourseCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }

    private void Assert(List<int> ids, Dictionary<Group, byte> courses)
    {
        foreach (var group in Context.Groups.Where(x => ids.Contains(x.Id)))
        {
            courses[group].Should().Be(--group.CurrentCourse);
        }
    }
}