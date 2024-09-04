using System.Net;
using System.Text;
using System.Web;

using Newtonsoft.Json;

using SmsProviders.Kavenegar.Exceptions;
using SmsProviders.Kavenegar.Models;
using SmsProviders.Kavenegar.Models.Enums;
using SmsProviders.Kavenegar.Utils;

namespace SmsProviders.Kavenegar;

internal class ReturnResult
{
    public Result @Return { get; set; }
    public object entries { get; set; }
}

internal class Result
{
    public int status { get; set; }
    public string message { get; set; }
}

internal class ReturnSend
{
    public Result @Return { get; set; }
    public List<SendResult> entries { get; set; }
}

internal class ReturnStatus
{
    public Result result { get; set; }
    public List<StatusResult> entries { get; set; }
}

internal class ReturnStatusLocalMessageId
{
    public Result result { get; set; }
    public List<StatusLocalMessageIdResult> entries { get; set; }
}

internal class ReturnReceive
{
    public Result result { get; set; }
    public List<ReceiveResult> entries { get; set; }
}

internal class ReturnCountOutbox
{
    public Result result { get; set; }
    public List<CountOutboxResult> entries { get; set; }
}

internal class ReturnCountInbox
{
    public Result result { get; set; }
    public List<CountInboxResult> entries { get; set; }

}

internal class ReturnCountPostalCode
{
    public Result result { get; set; }
    public List<CountPostalCodeResult> entries { get; set; }
}

internal class ReturnAccountInfo
{
    public Result result { get; set; }
    public AccountInfoResult entries { get; set; }
}

internal class ReturnAccountConfig
{
    public Result result { get; set; }
    public AccountConfigResult entries { get; set; }
}

public class KavenegarApi : IKavenegarApi
{
    private string _apikey;
    private readonly int _returnCode = 200;
    private readonly string _returnMessage = "";
    private const string Apipath = "https://api.kavenegar.com/v1/{0}/{1}/{2}.{3}";
    public KavenegarApi(string apikey)
    {
        _apikey = apikey;
    }

    public string ApiKey
    {
        set => _apikey = value;
        get => _apikey;
    }

    public int ReturnCode => _returnCode;

    public string ReturnMessage => _returnMessage;

    private string GetApiPath(string _base, string method, string output)
    {
        return string.Format(Apipath, _apikey, _base, method, output);
    }

    private static string Execute(string path, Dictionary<string, object> _params)
    {

        string responseBody = "";
        string postdata = "";

        byte[] byteArray;
        if (_params != null)
        {
            postdata = _params.Keys.Aggregate(postdata,
                (current, key) => current + string.Format("{0}={1}&", key, _params[key]));
            byteArray = Encoding.UTF8.GetBytes(postdata);
        }
        else
        {
            byteArray = [];
        }
        HttpClient client = new();

        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(path);
        webRequest.Method = "POST";
        webRequest.Timeout = -1;
        webRequest.ContentType = "application/x-www-form-urlencoded";
        webRequest.ContentLength = byteArray.Length;
        using (Stream webpageStream = webRequest.GetRequestStream())
        {
            webpageStream.Write(byteArray, 0, byteArray.Length);
        }
        HttpWebResponse webResponse;
        try
        {
            using (webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                using StreamReader reader = new(webResponse.GetResponseStream());
                responseBody = reader.ReadToEnd();
            }
            JsonConvert.DeserializeObject<ReturnResult>(responseBody);
            return responseBody;
        }
        catch (WebException webException)
        {
            webResponse = (HttpWebResponse)webException.Response;
            using (StreamReader reader = new(webResponse.GetResponseStream()))
            {
                responseBody = reader.ReadToEnd();
            }
            try
            {
                ReturnResult result = JsonConvert.DeserializeObject<ReturnResult>(responseBody);
                throw new ApiException(result.Return.message, result.Return.status);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpException(ex.Message, (int)((HttpWebResponse)webException.Response).StatusCode);
            }
        }
    }
    public List<SendResult> Send(string sender, List<string> receptor, string message)
    {
        return Send(sender, receptor, message, MessageType.MobileMemory, DateTime.MinValue);
    }

    public SendResult Send(string sender, string receptor, string message)
    {
        return Send(sender, receptor, message, MessageType.MobileMemory, DateTime.MinValue);
    }
    public SendResult Send(string sender, string receptor, string message, MessageType type, DateTime date)
    {
        List<string> receptors = [receptor];
        return Send(sender, receptors, message, type, date)[0];
    }
    public List<SendResult> Send(string sender, List<string> receptor, string message, MessageType type, DateTime date)
    {
        return Send(sender, receptor, message, type, date, null);
    }
    public SendResult Send(string sender, string receptor, string message, MessageType type, DateTime date, string localid)
    {
        List<string> receptors = [receptor];
        List<string> localids = [localid];
        return Send(sender, receptors, message, type, date, localids)[0];
    }
    public SendResult Send(string sender, string receptor, string message, string localid)
    {
        return Send(sender, receptor, message, MessageType.MobileMemory, DateTime.MinValue, localid);
    }
    public List<SendResult> Send(string sender, List<string> receptors, string message, string localid)
    {
        List<string> localids = [];
        for (int i = 0; i <= receptors.Count - 1; i++)
        {
            localids.Add(localid);
        }
        return Send(sender, receptors, message, MessageType.MobileMemory, DateTime.MinValue, localids);
    }
    public List<SendResult> Send(string sender, List<string> receptor, string message, MessageType type, DateTime date, List<string> localids)
    {
        string path = GetApiPath("sms", "send", "json");
        Dictionary<string, object> param = new()
        {
            {"sender", HttpUtility.UrlEncode(sender)},
            {"receptor", HttpUtility.UrlEncode(StringHelper.Join(",", receptor.ToArray()))},
            {"message", HttpUtility.UrlEncode(message)},
            {"type", (int) type},
            {"date", date == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(date)}
        };
        if (localids != null && localids.Count > 0)
        {
            param.Add("localid", StringHelper.Join(",", localids.ToArray()));
        }
        string responseBody = Execute(path, param);
        ReturnSend l = JsonConvert.DeserializeObject<ReturnSend>(responseBody);
        return l.entries;
    }

    public List<SendResult> SendArray(List<string> senders, List<string> receptors, List<string> messages)
    {
        List<MessageType> types = [];
        for (int i = 0; i <= senders.Count - 1; i++)
        {
            types.Add(MessageType.MobileMemory);
        }
        return SendArray(senders, receptors, messages, types, DateTime.MinValue, null);
    }

    public List<SendResult> SendArray(string sender, List<string> receptors, List<string> messages, MessageType type, DateTime date)
    {
        List<string> senders = [];
        for (int i = 0; i < receptors.Count; i++)
        {
            senders.Add(sender);
        }
        List<MessageType> types = [];
        for (int i = 0; i <= senders.Count - 1; i++)
        {
            types.Add(MessageType.MobileMemory);
        }
        return SendArray(senders, receptors, messages, types, date, null);
    }

    public List<SendResult> SendArray(string sender, List<string> receptors, List<string> messages, MessageType type, DateTime date, string localmessageids)
    {
        List<string> senders = [];
        for (int i = 0; i < receptors.Count; i++)
        {
            senders.Add(sender);
        }
        List<MessageType> types = [];
        for (int i = 0; i <= senders.Count - 1; i++)
        {
            types.Add(MessageType.MobileMemory);
        }
        return SendArray(senders, receptors, messages, types, date, [localmessageids]);
    }

    public List<SendResult> SendArray(string sender, List<string> receptors, List<string> messages, string localmessageid)
    {
        List<string> senders = [];
        for (int i = 0; i < receptors.Count; i++)
        {
            senders.Add(sender);
        }

        return SendArray(senders, receptors, messages, localmessageid);
    }

    public List<SendResult> SendArray(List<string> senders, List<string> receptors, List<string> messages, string localmessageid)
    {
        List<MessageType> types = [];
        for (int i = 0; i <= receptors.Count - 1; i++)
        {
            types.Add(MessageType.MobileMemory);
        }
        List<string> localmessageids = [];
        for (int i = 0; i <= receptors.Count - 1; i++)
        {
            localmessageids.Add(localmessageid);
        }
        return SendArray(senders, receptors, messages, types, DateTime.MinValue, localmessageids);
    }

    public List<SendResult> SendArray(List<string> senders, List<string> receptors, List<string> messages, List<MessageType> types, DateTime date, List<string> localmessageids)
    {
        string path = GetApiPath("sms", "sendarray", "json");
        string jsonSenders = JsonConvert.SerializeObject(senders);
        string jsonReceptors = JsonConvert.SerializeObject(receptors);
        string jsonMessages = JsonConvert.SerializeObject(messages);
        string jsonTypes = JsonConvert.SerializeObject(types);
        Dictionary<string, object> param = new()
        {
            {"message", jsonMessages},
            {"sender", jsonSenders},
            {"receptor", jsonReceptors},
            {"type", jsonTypes},
            {"date", date == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(date)}
        };
        if (localmessageids != null && localmessageids.Count > 0)
        {
            param.Add("localmessageids", StringHelper.Join(",", localmessageids.ToArray()));
        }

        string responsebody = Execute(path, param);
        ReturnSend l = JsonConvert.DeserializeObject<ReturnSend>(responsebody);
        if (l.entries == null)
        {
            return [];
        }
        return l.entries;
    }

    public List<StatusResult> Status(List<string> messageids)
    {
        string path = GetApiPath("sms", "status", "json");
        Dictionary<string, object> param = new()
        {
        {"messageid", StringHelper.Join(",", messageids.ToArray())}
    };
        string responsebody = Execute(path, param);
        ReturnStatus l = JsonConvert.DeserializeObject<ReturnStatus>(responsebody);
        if (l.entries == null)
        {
            return [];
        }
        return l.entries;
    }

    public StatusResult Status(string messageid)
    {
        List<string> ids = [messageid];
        List<StatusResult> result = Status(ids);
        return result.Count == 1 ? result[0] : null;
    }

    public List<StatusLocalMessageIdResult> StatusLocalMessageId(List<string> messageids)
    {
        string path = GetApiPath("sms", "statuslocalmessageid", "json");
        Dictionary<string, object> param = new() { { "localid", StringHelper.Join(",", messageids.ToArray()) } };
        string responsebody = Execute(path, param);
        ReturnStatusLocalMessageId l = JsonConvert.DeserializeObject<ReturnStatusLocalMessageId>(responsebody);
        return l.entries;
    }

    public StatusLocalMessageIdResult StatusLocalMessageId(string messageid)
    {
        List<StatusLocalMessageIdResult> result = StatusLocalMessageId([messageid]);
        return result.Count == 1 ? result[0] : null;
    }

    public List<SendResult> Select(List<string> messageids)
    {
        string path = GetApiPath("sms", "select", "json");
        Dictionary<string, object> param = new() { { "messageid", StringHelper.Join(",", messageids.ToArray()) } };
        string responsebody = Execute(path, param);
        ReturnSend l = JsonConvert.DeserializeObject<ReturnSend>(responsebody);
        if (l.entries == null)
        {
            return [];
        }
        return l.entries;
    }

    public SendResult Select(string messageid)
    {
        List<string> ids = [messageid];
        List<SendResult> result = Select(ids);
        return result.Count == 1 ? result[0] : null;
    }

    public List<SendResult> SelectOutbox(DateTime startdate)
    {
        return SelectOutbox(startdate, DateTime.MaxValue);
    }

    public List<SendResult> SelectOutbox(DateTime startdate, DateTime enddate)
    {
        return SelectOutbox(startdate, enddate, null);
    }

    public List<SendResult> SelectOutbox(DateTime startdate, DateTime enddate, string sender)
    {
        string path = GetApiPath("sms", "selectoutbox", "json");
        Dictionary<string, object> param = new()
        {
         {"startdate", startdate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(startdate)},
         {"enddate", enddate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(enddate)},
         {"sender", sender}
     };
        string responsebody = Execute(path, param);
        ReturnSend l = JsonConvert.DeserializeObject<ReturnSend>(responsebody);
        return l.entries;
    }

    public List<SendResult> LatestOutbox(long pagesize)
    {
        return LatestOutbox(pagesize, "");
    }

    public List<SendResult> LatestOutbox(long pagesize, string sender)
    {
        string path = GetApiPath("sms", "latestoutbox", "json");
        Dictionary<string, object> param = new() { { "pagesize", pagesize }, { "sender", sender } };
        string responsebody = Execute(path, param);
        ReturnSend l = JsonConvert.DeserializeObject<ReturnSend>(responsebody);
        return l.entries;
    }

    public CountOutboxResult CountOutbox(DateTime startdate)
    {
        return CountOutbox(startdate, DateTime.MaxValue, 10);
    }

    public CountOutboxResult CountOutbox(DateTime startdate, DateTime enddate)
    {
        return CountOutbox(startdate, enddate, 0);
    }

    public CountOutboxResult CountOutbox(DateTime startdate, DateTime enddate, int status)
    {
        string path = GetApiPath("sms", "countoutbox", "json");
        Dictionary<string, object> param = new()
        {
         {"startdate", startdate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(startdate)},
         {"enddate", enddate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(enddate)},
         {"status", status}
     };
        string responsebody = Execute(path, param);
        ReturnCountOutbox l = JsonConvert.DeserializeObject<ReturnCountOutbox>(responsebody);
        if (l.entries == null || l.entries[0] == null)
        {
            return new CountOutboxResult();
        }
        return l.entries[0];
    }

    public List<StatusResult> Cancel(List<string> ids)
    {
        string path = GetApiPath("sms", "cancel", "json");
        Dictionary<string, object> param = new()
        {
        {"messageid", StringHelper.Join(",", ids.ToArray())}
    };
        string responsebody = Execute(path, param);
        ReturnStatus l = JsonConvert.DeserializeObject<ReturnStatus>(responsebody);
        return l.entries;
    }

    public StatusResult Cancel(string messageid)
    {
        List<string> ids = [messageid];
        List<StatusResult> result = Cancel(ids);
        return result.Count == 1 ? result[0] : null;
    }

    public List<ReceiveResult> Receive(string line, int isread)
    {
        string path = GetApiPath("sms", "receive", "json");
        Dictionary<string, object> param = new() { { "linenumber", line }, { "isread", isread } };
        string responsebody = Execute(path, param);
        ReturnReceive l = JsonConvert.DeserializeObject<ReturnReceive>(responsebody);
        if (l.entries == null)
        {
            return [];
        }
        return l.entries;
    }

    public CountInboxResult CountInbox(DateTime startdate, string linenumber)
    {
        return CountInbox(startdate, DateTime.MaxValue, linenumber, 0);
    }

    public CountInboxResult CountInbox(DateTime startdate, DateTime enddate, string linenumber)
    {
        return CountInbox(startdate, enddate, linenumber, 0);
    }

    public CountInboxResult CountInbox(DateTime startdate, DateTime enddate, string linenumber, int isread)
    {
        string path = GetApiPath("sms", "countoutbox", "json");
        Dictionary<string, object> param = new()
        {
        {"startdate", startdate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(startdate)},
        {"enddate", enddate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(enddate)},
        {"linenumber", linenumber},
        {"isread", isread}
    };
        string responsebody = Execute(path, param);
        ReturnCountInbox l = JsonConvert.DeserializeObject<ReturnCountInbox>(responsebody);
        return l.entries[0];
    }

    public List<CountPostalCodeResult> CountPostalCode(long postalcode)
    {
        string path = GetApiPath("sms", "countpostalcode", "json");
        Dictionary<string, object> param = new() { { "postalcode", postalcode } };
        string responsebody = Execute(path, param);
        ReturnCountPostalCode l = JsonConvert.DeserializeObject<ReturnCountPostalCode>(responsebody);
        return l.entries;
    }

    public List<SendResult> SendByPostalCode(long postalcode, string sender, string message, long mcistartIndex, long mcicount, long mtnstartindex, long mtncount)
    {
        return SendByPostalCode(postalcode, sender, message, mcistartIndex, mcicount, mtnstartindex, mtncount, DateTime.MinValue);
    }

    public List<SendResult> SendByPostalCode(long postalcode, string sender, string message, long mcistartIndex, long mcicount, long mtnstartindex, long mtncount, DateTime date)
    {
        string path = GetApiPath("sms", "sendbypostalcode", "json");
        Dictionary<string, object> param = new()
        {
        {"postalcode", postalcode},
        {"sender", sender},
        {"message", HttpUtility.UrlEncodeUnicode(message)},
        {"mcistartIndex", mcistartIndex},
        {"mcicount", mcicount},
        {"mtnstartindex", mtnstartindex},
        {"mtncount", mtncount},
        {"date", date == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(date)}
    };
        string responsebody = Execute(path, param);
        ReturnSend l = JsonConvert.DeserializeObject<ReturnSend>(responsebody);
        return l.entries;
    }

    public AccountInfoResult AccountInfo()
    {
        string path = GetApiPath("account", "info", "json");
        string responsebody = Execute(path, null);
        ReturnAccountInfo l = JsonConvert.DeserializeObject<ReturnAccountInfo>(responsebody);
        return l.entries;
    }

    public AccountConfigResult AccountConfig(string apilogs, string dailyreport, string debugmode, string defaultsender, int? mincreditalarm, string resendfailed)
    {
        string path = GetApiPath("account", "config", "json");
        Dictionary<string, object> param = new()
        {
        {"apilogs", apilogs},
        {"dailyreport", dailyreport},
        {"debugmode", debugmode},
        {"defaultsender", defaultsender},
        {"mincreditalarm", mincreditalarm},
        {"resendfailed", resendfailed}
    };
        string responsebody = Execute(path, param);
        ReturnAccountConfig l = JsonConvert.DeserializeObject<ReturnAccountConfig>(responsebody);
        return l.entries;
    }

    public SendResult VerifyLookup(string receptor, string token, string template)
    {
        return VerifyLookup(receptor, token, null, null, template, VerifyLookupType.Sms);
    }
    public SendResult VerifyLookup(string receptor, string token, string template, VerifyLookupType type)
    {
        return VerifyLookup(receptor, token, null, null, template, type);
    }
    public SendResult VerifyLookup(string receptor, string token, string token2, string token3, string template)
    {
        return VerifyLookup(receptor, token, token2, token3, template, VerifyLookupType.Sms);
    }
    public SendResult VerifyLookup(string receptor, string token, string token2, string token3, string token10, string template)
    {
        return VerifyLookup(receptor, token, token2, token3, token10, template, VerifyLookupType.Sms);
    }
    public SendResult VerifyLookup(string receptor, string token, string token2, string token3, string template, VerifyLookupType type)
    {
        return VerifyLookup(receptor, token, token2, token3, null, template, type);
    }

    public SendResult VerifyLookup(string receptor, string token, string token2, string token3, string token10, string template, VerifyLookupType type)
    {
        return VerifyLookup(receptor, token, token2, token3, token10, null, template, type);
    }

    public SendResult VerifyLookup(string receptor, string token, string token2, string token3, string token10, string token20, string template, VerifyLookupType type)
    {
        string path = GetApiPath("verify", "lookup", "json");
        Dictionary<string, object> param = new()
        {
        {"receptor", receptor},
        {"template", template},
        {"token", token},
        {"token2", token2},
        {"token3", token3},
        {"token10", token10},
        {"token20", token20},
        {"type", type},
    };
        string responsebody = Execute(path, param);
        ReturnSend l = JsonConvert.DeserializeObject<ReturnSend>(responsebody);
        return l.entries[0];
    }


    #region << CallMakeTTS >>

    public SendResult CallMakeTTS(string message, string receptor)
    {
        return CallMakeTTS(message, [receptor], null, null)[0];
    }
    public List<SendResult> CallMakeTTS(string message, List<string> receptor)
    {
        return CallMakeTTS(message, receptor, null, null);
    }

    public List<SendResult> CallMakeTTS(string message, List<string> receptor, DateTime? date, List<string> localid)
    {
        string path = GetApiPath("call", "maketts", "json");
        Dictionary<string, object> param = new()
        {
            {"receptor", StringHelper.Join(",", receptor.ToArray())},
            {"message", HttpUtility.UrlEncodeUnicode(message)},
        };
        if (date != null)
            param.Add("date", DateHelper.DateTimeToUnixTimestamp(date.Value));
        if (localid != null && localid.Count > 0)
            param.Add("localid", StringHelper.Join(",", localid.ToArray()));
        string responseBody = Execute(path, param);

        return JsonConvert.DeserializeObject<ReturnSend>(responseBody).entries;
    }

    #endregion << CallMakeTTS >>

}