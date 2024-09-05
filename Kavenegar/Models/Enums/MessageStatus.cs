namespace SmsProviders.Kavenegar.Models.Enums;

public enum MessageStatus
{
    /// <summary>
    /// در صف ارسال قرار دارد
    /// </summary>
    InQueue = 1,

    /// <summary>
    /// زمان بندی شده (ارسال در تاریخ معین)
    /// </summary>
    Scheduled = 2,

    /// <summary>
    /// ارسال شده به مخابرات
    /// </summary>
    SentToTelecom = 4,

    /// <summary>
    /// ارسال شده به مخابرات (همانند وضعیت 4)
    /// </summary>
    SentToTelecomAgain = 5,

    /// <summary>
    /// خطا در ارسال پیام که توسط سر شماره پیش می آید و به معنی عدم رسیدن پیامک می‌باشد (Failed)
    /// </summary>
    Failed = 6,

    /// <summary>
    /// رسیده به گیرنده (Delivered)
    /// </summary>
    Delivered = 10,

    /// <summary>
    /// نرسیده به گیرنده ، این وضعیت به دلایلی از جمله خاموش یا خارج از دسترس بودن گیرنده اتفاق می افتد (Undelivered)
    /// </summary>
    Undelivered = 11,

    /// <summary>
    /// ارسال پیام از سمت کاربر لغو شده یا در ارسال آن مشکلی پیش آمده که هزینه آن به حساب برگشت داده می‌شود
    /// </summary>
    Cancelled = 13,

    /// <summary>
    /// بلاک شده است، عدم تمایل گیرنده به دریافت پیامک از خطوط تبلیغاتی که هزینه آن به حساب برگشت داده می‌شود
    /// </summary>
    Blocked = 14,

    /// <summary>
    /// شناسه پیامک نامعتبر است (به این معنی که شناسه پیام در پایگاه داده کاوه نگار ثبت نشده است یا متعلق به شما نمی‌باشد)
    /// </summary>
    InvalidMessageId = 100
}
