using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Data.Configurations;

public class BorrowingTransactionConfiguration : IEntityTypeConfiguration<BorrowingTransaction>
{
    public void Configure(EntityTypeBuilder<BorrowingTransaction> builder)
    {
        builder.HasKey(bt => bt.Id);

        builder.Property(bt => bt.BorrowDate)
            .IsRequired();

        builder.Property(bt => bt.DueDate)
            .IsRequired();

        builder.Property(bt => bt.ReturnDate);

        builder.Property(bt => bt.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(bt => bt.FineAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(bt => bt.CreatedAt)
            .IsRequired();

        builder.Property(bt => bt.UpdatedAt);

        builder.HasOne(bt => bt.Book)
            .WithMany()
            .HasForeignKey(bt => bt.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bt => bt.Member)
            .WithMany()
            .HasForeignKey(bt => bt.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(bt => bt.DomainEvents);
    }
}