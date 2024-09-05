namespace SmsProviders.Kavenegar;

public class KavenegarClient
{
    private readonly HttpClient _client;

    public KavenegarClient(HttpClient client)
        => _client = client;
}
