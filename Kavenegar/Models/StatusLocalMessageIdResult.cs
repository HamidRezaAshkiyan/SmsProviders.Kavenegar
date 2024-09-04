using SmsProviders.Kavenegar.Models.Enums;
namespace SmsProviders.Kavenegar.Models;

public class StatusLocalMessageIdResult
{
    public long Messageid { get; set; }
    public long Localid { get; set; }
    public MessageStatus Status { get; set; }
    public string Statustext { get; set; }
}