using System;
using System.Linq;
namespace Kavenegar.Utils;

public class StringHelper
{
    public static string Join(string delimeter, string[] items)
    {
        string result = items.Aggregate("", (current, obj) => current + (obj + ","));
        return result[..^1];
    }

    public static string Join(string delimeter, long[] items)
    {
        string result = items.Aggregate("", (current, obj) => current + (obj.ToString() + ","));
        return result[..^1];
    }
}