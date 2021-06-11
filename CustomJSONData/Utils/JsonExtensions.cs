namespace CustomJSONData.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.IO;
    using CustomJSONData.CustomBeatmap;
    using Newtonsoft.Json;

    internal static class JsonExtensions
    {
        internal static object ReadAsExpandoObject(this JsonReader reader) => ReadObject(reader);

        internal static object ReadAsExpandoObjectWithCustomEvents(this JsonReader reader, out List<CustomBeatmapSaveData.CustomEventData> customEventDatas) => ReadObjectWithCustomEvents(reader, out customEventDatas);

        private static object ReadValue(JsonReader reader)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return ReadObject(reader);
                case JsonToken.StartArray:
                    return ReadList(reader);
                default:
                    return reader.Value;
            }
        }

        private static IList<object> ReadList(JsonReader reader)
        {
            IList<object> list = new List<object>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        break;
                    default:
                        list.Add(ReadValue(reader));
                        break;
                    case JsonToken.EndArray:
                        return list;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading ExpandoObject.");
        }

        private static object ReadObject(JsonReader reader)
        {
            IDictionary<string, object> expandoObject = new ExpandoObject();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string propertyName = reader.Value.ToString();
                        if (!reader.Read())
                        {
                            throw new JsonSerializationException("Unexpected end when reading ExpandoObject.");
                        }

                        expandoObject[propertyName] = ReadValue(reader);
                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.EndObject:
                        return expandoObject;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading ExpandoObject.");
        }

        private static object ReadObjectWithCustomEvents(JsonReader reader, out List<CustomBeatmapSaveData.CustomEventData> customEventDatas)
        {
            IDictionary<string, object> expandoObject = new ExpandoObject();
            customEventDatas = new List<CustomBeatmapSaveData.CustomEventData>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string propertyName = reader.Value.ToString();
                        if (propertyName == "_customEvents")
                        {
                            reader.Read(); // StartArray
                            reader.Read(); // StartObject (hopefully)

                            while (reader.TokenType == JsonToken.StartObject)
                            {
                                CustomBeatmapSaveData.CustomEventData customEventData = new CustomBeatmapSaveData.CustomEventData();
                                reader.Read();
                                while (reader.TokenType == JsonToken.PropertyName)
                                {
                                    switch (reader.Value)
                                    {
                                        case "_time":
                                            customEventData._time = (float)reader.ReadAsDouble();
                                            break;

                                        case "_type":
                                            customEventData._type = reader.ReadAsString();
                                            break;

                                        case "_data":
                                            customEventData._data = reader.ReadAsExpandoObject();
                                            break;

                                        default:
                                            reader.Skip();
                                            break;
                                    }

                                    reader.Read();
                                }

                                customEventDatas.Add(customEventData);
                                reader.Read();
                            }
                        }
                        else
                        {
                            if (!reader.Read())
                            {
                                throw new JsonSerializationException("Unexpected end when reading ExpandoObject.");
                            }

                            expandoObject[propertyName] = ReadValue(reader);
                        }

                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.EndObject:
                        return expandoObject;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading ExpandoObject.");
        }
    }
}
