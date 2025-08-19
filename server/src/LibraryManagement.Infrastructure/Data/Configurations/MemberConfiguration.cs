using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.MembershipNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(m => m.MembershipNumber)
            .IsUnique();

        builder.Property(m => m.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(m => m.Email)
            .IsUnique();

        builder.Property(m => m.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.MembershipType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt);

        builder.Ignore(m => m.DomainEvents);
    }
}