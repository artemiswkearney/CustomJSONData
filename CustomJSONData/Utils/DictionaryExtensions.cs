namespace CustomJSONData
{
    using System;
    using System.Collections.Generic;

    public static class DictionaryExtensions
    {
        public static Dictionary<string, object> Copy(this Dictionary<string, object> dictionary)
        {
            return dictionary != null ? new Dictionary<string, object>(dictionary) : new Dictionary<string, object>();
        }

        public static T Get<T>(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out object value))
            {
                Type underlyingType = Nullable.GetUnderlyingType(typeof(T));
                if (underlyingType != null)
                {
                    return (T)Convert.ChangeType(value, underlyingType);
                }
                else
                {
                    if (value is IConvertible)
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    else
                    {
                        return (T)value;
                    }
                }
            }

            return default;
        }
    }
}
