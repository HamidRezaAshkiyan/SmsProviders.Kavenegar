namespace SmsProviders.Kavenegar.Utils;

public class StringHelper
{
    public static string Join(string delimiter, string[] items)
    {
        string result = items.Aggregate("", (current, obj) => current + obj + ",");
        return result[..^1];
    }

    public static string Join(string delimiter, long[] items)
    {
        string result = items.Aggregate("", (current, obj) => current + obj.ToString() + ",");
        return result[..^1];
    }
}