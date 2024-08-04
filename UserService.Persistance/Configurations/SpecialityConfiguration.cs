using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Persistance.Configurations;

public class SpecialityConfiguration : IEntityTypeConfiguration<Speciality>
{
    public void Configure(EntityTypeBuilder<Speciality> builder)
    {
        builder
            .HasMany(e => e.Groups)
            .WithOne(e => e.Speciality)
            .HasForeignKey(e => e.SpecialityId)
            .IsRequired();
    }
}
