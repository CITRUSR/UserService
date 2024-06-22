using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Persistance.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasMany(e => e.Students)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupId)
            .IsRequired();
        
        builder.HasOne(e => e.Speciality)
            .WithMany(e => e.Groups)
            .HasForeignKey(e => e.SpecialityId)
            .IsRequired();
    }
}