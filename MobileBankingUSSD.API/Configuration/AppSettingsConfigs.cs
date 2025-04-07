namespace MobileBankingUSSD.API.Configuration;

internal static class AppSettingsConfigs
{
    internal static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name) ??
               throw new InvalidOperationException($"The connection string {name} was not found");
    }
    
    internal static  T GetValueOrThrow<T>(this IConfiguration configuration, string name)
    {
        return configuration.GetValue<T?>(name) ??
               throw new InvalidOperationException($"The connection string {name} was not found");
    }
}