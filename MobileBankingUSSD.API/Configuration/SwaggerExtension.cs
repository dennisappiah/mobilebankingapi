using Microsoft.OpenApi.Models;

namespace MobileBankingUSSD.API.Configuration;

internal static class SwaggerExtension
{
    internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MobileBanking API",
                Version = "v1",
                Description = "MobileBanking API built with ASP.NET Core Web API",
            });
            
            opt.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
        });
        
        return services;
    }
}