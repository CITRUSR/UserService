using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Persistance.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.HasMany(e => e.Groups)
            .WithOne(e => e.Curator)
            .HasForeignKey(e => e.CuratorId)
            .IsRequired();

        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.HasIndex(x => x.SsoId);
        builder.Property(x => x.FiredAt).HasColumnType("timestamp");
    }
}