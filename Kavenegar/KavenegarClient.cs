using System.Net.Http.Json;
using System.Web;

using SmsProviders.Kavenegar.Models;

namespace SmsProviders.Kavenegar;

public record Result(int Status, string Message);
public record BaseReturnResult<T>(Result Return, T Entries);

public class KavenegarClient : IKavenegarClient
{
    private readonly HttpClient _client;
    private readonly KavenegarClientOptions _options;

    public KavenegarClient(HttpClient client, KavenegarClientOptions options)
    {
        _client = client;
        _options = options;
    }

    // Send SMS
    public async Task<BaseReturnResult<List<SendResult>>> SendSmsAsync(string receptor, string message)
    {
        string url = $"{_options.RequestUrl}" + $"/sms/send.json" +
            $"?receptor={receptor}" +
            $"&sender={_options.LineNumber}" +
            $"&message={HttpUtility.UrlEncode(message)}";

        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BaseReturnResult<List<SendResult>>>();
    }

    // Get Delivery Status
    public async Task<BaseReturnResult<List<StatusResult>>> GetDeliveryStatusAsync(long messageId)
    {
        string url = $"{_options.RequestUrl}" + $"/sms/status.json" +
            $"?messageid={messageId}";

        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BaseReturnResult<List<StatusResult>>>();
    }

    // Receive Incoming Messages (Inbox)
    public async Task<BaseReturnResult<List<ReceiveResult>>?> ReceiveMessagesAsync(string lineNumber, int isRead = 0)
    {
        string url = $"{_options.RequestUrl}" + $"/sms/receive.json" +
            $"?linenumber={lineNumber}" +
            $"&isread={isRead}";

        HttpResponseMessage response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BaseReturnResult<List<ReceiveResult>>>();
    }

    // Get Account Info
    public async Task<BaseReturnResult<AccountConfigResult>?> GetAccountInfoAsync()
    {
        string url = $"{_options.RequestUrl}" + $"/account/info.json";

        HttpResponseMessage response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BaseReturnResult<AccountConfigResult>>();
    }
}