namespace CustomJSONData
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal static class JsonExtensions
    {
        internal static void ReadToDictionary(this JsonReader reader, Dictionary<string, object> dictionary, Func<string, bool> specialCase = null) => ObjectReadObject(reader, dictionary, specialCase);

        internal static void ReadObject(this JsonReader reader, Action<object> action)
        {
            reader.Read();
            while (reader.TokenType == JsonToken.PropertyName)
            {
                action(reader.Value);

                reader.Read();
            }
        }

        internal static void ReadObjectArray(this JsonReader reader, Action action)
        {
            reader.Read(); // StartArray
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonSerializationException("Was not array.");
            }

            reader.Read(); // StartObject (hopefully)

            while (reader.TokenType == JsonToken.StartObject)
            {
                action();
                reader.Read();
            }

            if (reader.TokenType != JsonToken.EndArray)
            {
                throw new JsonSerializationException("Unexpected end when reading array.");
            }
        }

        private static object ObjectReadValue(JsonReader reader)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return ObjectReadObject(reader);

                case JsonToken.StartArray:
                    return ObjectReadList(reader);

                default:
                    return reader.Value;
            }
        }

        private static IList<object> ObjectReadList(JsonReader reader)
        {
            IList<object> list = new List<object>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        break;

                    default:
                        list.Add(ObjectReadValue(reader));
                        break;

                    case JsonToken.EndArray:
                        return list;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading Dictionary.");
        }

        private static object ObjectReadObject(JsonReader reader, Dictionary<string, object> dictionary = null, Func<string, bool> specialCase = null)
        {
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, object>();
            }

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string propertyName = reader.Value.ToString();

                        if (specialCase != null && !specialCase(propertyName))
                        {
                            break;
                        }

                        if (!reader.Read())
                        {
                            throw new JsonSerializationException("Unexpected end when reading Dictionary.");
                        }

                        dictionary[propertyName] = ObjectReadValue(reader);

                        break;

                    case JsonToken.Comment:
                        break;

                    case JsonToken.EndObject:
                        return dictionary;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading Dictionary.");
        }
    }
}
