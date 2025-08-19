using System;
using LibraryManagement.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly LibraryDbContext _context;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, LibraryDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Seed Books
        if (!_context.Books.Any())
        {
            var books = new List<Book>
            {
                new Book("978-0-547-92822-7", "The Hobbit", "J.R.R. Tolkien", "Houghton Mifflin", 1937, "Fantasy", 3),
                new Book("978-0-553-21311-4", "1984", "George Orwell", "Secker & Warburg", 1949, "Dystopian Fiction", 2),
                new Book("978-0-7432-7356-5", "To Kill a Mockingbird", "Harper Lee", "J.B. Lippincott & Co.", 1960, "Fiction", 4),
                new Book("978-0-452-28423-4", "Pride and Prejudice", "Jane Austen", "T. Egerton", 1813, "Romance", 2),
                new Book("978-0-316-76948-0", "The Catcher in the Rye", "J.D. Salinger", "Little, Brown and Company", 1951, "Fiction", 3)
            };

            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} books", books.Count);
        }

        // Seed Members
        if (!_context.Members.Any())
        {
            var members = new List<Member>
            {
                new Member("MEM20250001", "John", "Doe", "john.doe@email.com", "+1234567890", "123 Main St, City", Domain.Enums.MembershipType.Adult),
                new Member("MEM20250002", "Jane", "Smith", "jane.smith@email.com", "+1234567891", "456 Oak Ave, City", Domain.Enums.MembershipType.Student),
                new Member("MEM20250003", "Bob", "Johnson", "bob.johnson@email.com", "+1234567892", "789 Pine St, City", Domain.Enums.MembershipType.Senior),
                new Member("MEM20250004", "Alice", "Brown", "alice.brown@email.com", "+1234567893", "321 Elm St, City", Domain.Enums.MembershipType.Staff)
            };

            _context.Members.AddRange(members);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} members", members.Count);
        }

        _logger.LogInformation("Database seeding completed successfully");
    }
}