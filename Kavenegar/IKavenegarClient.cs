
using SmsProviders.Kavenegar.Models;

namespace SmsProviders.Kavenegar;
public interface IKavenegarClient
{
    Task<BaseReturnResult<AccountConfigResult>?> GetAccountInfoAsync();
    Task<BaseReturnResult<List<StatusResult>>> GetDeliveryStatusAsync(long messageId);
    Task<BaseReturnResult<List<ReceiveResult>>?> ReceiveMessagesAsync(string lineNumber, int isRead = 0);
    Task<BaseReturnResult<List<SendResult>>> SendSmsAsync(string receptor, string lineNumber, string message);
}