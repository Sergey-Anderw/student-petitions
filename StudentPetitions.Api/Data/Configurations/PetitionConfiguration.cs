using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentPetitions.Api.Entities;

namespace StudentPetitions.Api.Data.Configurations;

public sealed class PetitionConfiguration : IEntityTypeConfiguration<Petition>
{
    public void Configure(EntityTypeBuilder<Petition> builder)
    {
        builder.ToTable("Petitions");

        builder.HasKey(petition => petition.Id);

        builder.Property(petition => petition.StudentId)
            .IsRequired();

        builder.Property(petition => petition.PetitionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(petition => petition.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(petition => petition.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(petition => petition.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(petition => petition.CreatedAt)
            .IsRequired();

        builder.Property(petition => petition.UpdatedAt)
            .IsRequired();

        builder.Property(petition => petition.ReviewedBy)
            .HasMaxLength(100);

        builder.Property(petition => petition.ReviewedAt)
            .IsRequired(false);

        builder.Property(petition => petition.ReviewComment)
            .HasMaxLength(2000);

        builder.HasIndex(petition => petition.StudentId);

        builder.HasIndex(petition => petition.Status);

        builder.HasIndex(petition => petition.PetitionType);

        builder.HasIndex(petition => petition.CreatedAt);

        builder.HasOne(petition => petition.Student)
            .WithMany(student => student.Petitions)
            .HasForeignKey(petition => petition.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
