using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace IceCreamProject.Extensions;
public static class SessionExtensions
{
    // Lưu object vào session
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        var serializedValue = JsonConvert.SerializeObject(value);
        session.SetString(key, serializedValue);
    }

    // Lấy object từ session
    public static T? GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonConvert.DeserializeObject<T>(value);
    }
}
