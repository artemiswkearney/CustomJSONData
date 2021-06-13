namespace CustomJSONData
{
    using System.Collections.Generic;

    public static class CollectionExtensions
    {
        public static Dictionary<string, object> Copy(this Dictionary<string, object> dictionary)
        {
            return dictionary != null ? new Dictionary<string, object>(dictionary) : new Dictionary<string, object>();
        }

        public static T Get<T>(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out object value))
            {
                return (T)value;
            }

            return default;
        }
    }
}
