using Newtonsoft.Json;

namespace IceCreamProject.Extensions
{
	public static class SessionExtensions
	{
		public static void SetObjectAsJson(this ISession session, string key, object value)
		{
			var json = JsonConvert.SerializeObject(value);
			session.SetString(key, json);
		}

		public static T? GetObjectFromJson<T>(this ISession session, string key)
		{
			var value = session.GetString(key);
			return value == null ? default : JsonConvert.DeserializeObject<T>(value);
		}
	}
}
