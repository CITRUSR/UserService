using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Persistance.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder
            .HasOne(e => e.Group)
            .WithMany(e => e.Students)
            .HasForeignKey(e => e.GroupId)
            .IsRequired();

        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.HasIndex(x => x.SsoId);

        builder.Property(x => x.DroppedOutAt).HasColumnType("date");
    }
}
