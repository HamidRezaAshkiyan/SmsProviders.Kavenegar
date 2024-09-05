using System.Web;

using SmsProviders.Kavenegar.Extensions;
using SmsProviders.Kavenegar.Models;

namespace SmsProviders.Kavenegar;

public record Result(int Status, string Message);
public record BaseReturnResult<T>(Result Return, T Entries);

public class KavenegarClient : IKavenegarClient
{
    private readonly HttpClient _client;

    public KavenegarClient(HttpClient client)
        => _client = client;

    // Send SMS
    public async Task<BaseReturnResult<List<SendResult>>> SendSmsAsync(string receptor, string message, string lineNumber)
    {
        string requestUri = Routes.GetSendRoute(receptor, message, lineNumber);

        return await _client.GetRequestAsync<BaseReturnResult<List<SendResult>>>(requestUri);
    }

    // Get Delivery Status
    public async Task<BaseReturnResult<List<StatusResult>>> GetDeliveryStatusAsync(long messageId)
    {
        string requestUri = Routes.GetDeliveryStatusRoute(messageId);

        return await _client.GetRequestAsync<BaseReturnResult<List<StatusResult>>>(requestUri);
    }

    // Receive Incoming Messages (Inbox)
    public async Task<BaseReturnResult<List<ReceiveResult>>?> ReceiveMessagesAsync(string lineNumber, int isRead = 0)
    {
        string requestUri = Routes.GetReceiveRoute(lineNumber, isRead);

        return await _client.GetRequestAsync<BaseReturnResult<List<ReceiveResult>>>(requestUri);
    }

    // Get Account Info
    public async Task<BaseReturnResult<AccountConfigResult>?> GetAccountInfoAsync()
    {
        string requestUri = Routes.GetAccountInfoRoute();

        return await _client.GetRequestAsync<BaseReturnResult<AccountConfigResult>>(requestUri);
    }
}

internal static class Routes
{
    internal static string GetSendRoute(string receptor, string message, string lineNumber)
        => //baseAddress + 
        "sms/send.json" +
        $"?receptor={receptor}" +
        $"&sender={lineNumber}" +
        $"&message={HttpUtility.UrlEncode(message)}";

    internal static string GetDeliveryStatusRoute(long messageId)
            => //baseAddress +
             "sms/status.json" +
            $"?messageid={messageId}";

    internal static string GetReceiveRoute(string lineNumber, int isRead)
        => //baseAddress +
            "sms/receive.json" +
            $"?linenumber={lineNumber}" +
            $"&isread={isRead}";

    internal static string GetAccountInfoRoute()
        => //baseAddress +
           "account/info.json";
}