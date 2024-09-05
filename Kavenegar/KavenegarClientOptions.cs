namespace SmsProviders.Kavenegar;

public class KavenegarClientOptions
{
    public const string ConfigurationSection =
        nameof(KavenegarClientOptions);

    public string ApiKey { get; init; }
    public Uri BaseAddress { get; init; } = new("https://api.kavenegar.com/v1/");

    public string LineNumber { get; init; }

    public string RequestUrl => $"{BaseAddress.AbsoluteUri}{ApiKey}/";
}