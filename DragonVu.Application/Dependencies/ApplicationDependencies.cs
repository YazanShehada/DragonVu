using DragonVu.Application.Features.Questions.Services;
using LiteBus.Commands;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Messaging;
using LiteBus.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace DragonVu.Application.Dependencies;

public static class ApplicationDependencies
{
    /// <summary>
    /// Registers the application layer: LiteBus handlers and domain services.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IQuestionImageService, QuestionImageService>();

        services.AddLiteBus(liteBus =>
        {
            var applicationAssembly = typeof(ApplicationDependencies).Assembly;

            liteBus.AddMessaging(_ => { });
            liteBus.AddCommands(commands => commands.RegisterFromAssembly(applicationAssembly));
            liteBus.AddQueries(queries => queries.RegisterFromAssembly(applicationAssembly));
        });

        return services;
    }
}
