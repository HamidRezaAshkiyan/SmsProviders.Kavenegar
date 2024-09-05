namespace SmsProviders.Kavenegar.Extensions;

public static partial class IServiceCollectionExtensions
{
    /*public static IServiceCollection AddKavenegarClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<KavenegarClient>(client =>
        {
            KavenegarClientOptions options = new();
            configuration.GetSection(KavenegarClientOptions.ConfigurationSection).Bind(options);

            client.BaseAddress = new Uri(options.BaseAddress, UriKind.Absolute);
            client.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
        });

        return services;
    }*/
}