using Application.Interfaces;
using Application.Interfaces.Queries;
using Application.Services;
using Domain.Interfaces;
using Infra.Context;
using Infra.Queries;
using Infra.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Infra.IoC;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionApi
{
    public static IServiceCollection AddInfrastructureApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ApplicationDbContext>();

        #region Services
        services.AddScoped<IAwardsService, AwardsService>();
        #endregion

        #region Queries
        services.AddScoped<IAwardsQuery, AwardsQuery>();
        #endregion

        #region Repository
        services.AddScoped<IAwardsRepository, AwardsRepository>();
        #endregion

        return services;
    }
}