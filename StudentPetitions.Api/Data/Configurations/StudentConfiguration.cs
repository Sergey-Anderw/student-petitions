using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentPetitions.Api.Entities;

namespace StudentPetitions.Api.Data.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(student => student.Id);

        builder.Property(student => student.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(student => student.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(student => student.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(student => student.StudentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(student => student.CreatedAt)
            .IsRequired();

        builder.HasIndex(student => student.Email)
            .IsUnique();

        builder.HasIndex(student => student.StudentNumber)
            .IsUnique();
    }
}
