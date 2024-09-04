namespace SmsProviders.Kavenegar;

public class KavenegarClientOptions
{
    public const string ConfigurationSection =
        nameof(KavenegarClientOptions);

    public string BaseAddress { get; init; }
    public string ApiKey { get; init; }
}