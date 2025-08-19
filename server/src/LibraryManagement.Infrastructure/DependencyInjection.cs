using System;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibraryManagement.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=library.db";

        builder.Services.AddDbContext<LibraryDbContext>(options =>
            options.UseSqlite(connectionString));

        builder.Services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<LibraryDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

    }
}
