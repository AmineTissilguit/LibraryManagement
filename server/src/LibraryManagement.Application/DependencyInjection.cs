using System.Reflection;
using FluentValidation;
using LibraryManagement.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibraryManagement.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        builder.Services.AddValidatorsFromAssembly(assembly);

        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

    }
}
