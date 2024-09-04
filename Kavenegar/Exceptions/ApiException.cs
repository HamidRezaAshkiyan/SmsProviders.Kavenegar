using Kavenegar.Models.Enums;

namespace Kavenegar.Exceptions;

public class ApiException : KavenegarException
{
    private readonly MetaCode _result;
    public ApiException(string message, int code)
     : base(message)
    {
        _result = (MetaCode)code;
    }

    public MetaCode Code => _result;

}