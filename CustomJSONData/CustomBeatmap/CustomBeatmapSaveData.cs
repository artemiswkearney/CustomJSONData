namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    internal class CustomBeatmapSaveData : BeatmapSaveData
    {
        private CustomBeatmapSaveData(
            string version,
            List<BeatmapSaveData.EventData> events,
            List<BeatmapSaveData.NoteData> notes,
            List<BeatmapSaveData.WaypointData> waypoints,
            List<BeatmapSaveData.ObstacleData> obstacles,
            SpecialEventKeywordFiltersData specialEventsKeywordFilters,
            Dictionary<string, object> customData,
            List<CustomEventData> customEvents)
            : base(events, notes, waypoints, obstacles, specialEventsKeywordFilters)
        {
            _version = version;
            this.customData = customData;
            this.customEvents = customEvents;
        }

        internal List<CustomEventData> customEvents { get; }

        internal Dictionary<string, object> customData { get; }

        internal static CustomBeatmapSaveData Deserialize(string path)
        {
            string version = string.Empty;
            Dictionary<string, object> customData = new Dictionary<string, object>();
            List<CustomEventData> customEvents = new List<CustomEventData>();
            List<EventData> events = new List<EventData>();
            List<NoteData> notes = new List<NoteData>();
            List<WaypointData> waypoints = new List<WaypointData>();
            List<ObstacleData> obstacles = new List<ObstacleData>();
            List<SpecialEventsForKeyword> keywords = new List<SpecialEventsForKeyword>();

            using (JsonTextReader reader = new JsonTextReader(new StreamReader(path)))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        switch (reader.Value)
                        {
                            default:
                                reader.Skip();
                                break;

                            case "_version":
                                version = reader.ReadAsString();
                                break;

                            case "_events":
                                reader.ReadObjectArray(() =>
                                {
                                    float time = default;
                                    BeatmapEventType type = default;
                                    int value = default;
                                    Dictionary<string, object> data = new Dictionary<string, object>();
                                    reader.ReadObject(objectName =>
                                    {
                                        switch (objectName)
                                        {
                                            case "_time":
                                                time = (float)reader.ReadAsDouble();
                                                break;

                                            case "_type":
                                                type = (BeatmapEventType)reader.ReadAsInt32();
                                                break;

                                            case "_value":
                                                value = (int)reader.ReadAsInt32();
                                                break;

                                            case "_customData":
                                                reader.ReadToDictionary(data);
                                                break;

                                            default:
                                                reader.Skip();
                                                break;
                                        }
                                    });

                                    events.Add(new EventData(time, type, value, data));
                                });

                                break;

                            case "_notes":
                                reader.ReadObjectArray(() =>
                                {
                                    float time = default;
                                    int lineIndex = default;
                                    NoteLineLayer lineLayer = default;
                                    NoteType type = default;
                                    NoteCutDirection cutDirection = default;
                                    Dictionary<string, object> data = new Dictionary<string, object>();
                                    reader.ReadObject(objectName =>
                                    {
                                        switch (objectName)
                                        {
                                            case "_time":
                                                time = (float)reader.ReadAsDouble();
                                                break;

                                            case "_lineIndex":
                                                lineIndex = (int)reader.ReadAsInt32();
                                                break;

                                            case "_lineLayer":
                                                lineLayer = (NoteLineLayer)reader.ReadAsInt32();
                                                break;

                                            case "_type":
                                                type = (NoteType)reader.ReadAsInt32();
                                                break;

                                            case "_cutDirection":
                                                cutDirection = (NoteCutDirection)reader.ReadAsInt32();
                                                break;

                                            case "_customData":
                                                reader.ReadToDictionary(data);
                                                break;

                                            default:
                                                reader.Skip();
                                                break;
                                        }
                                    });

                                    notes.Add(new NoteData(time, lineIndex, lineLayer, type, cutDirection, data));
                                });

                                break;

                            case "_waypoints":
                                reader.ReadObjectArray(() =>
                                {
                                    float time = default;
                                    int lineIndex = default;
                                    NoteLineLayer lineLayer = default;
                                    OffsetDirection offsetDirection = default;
                                    Dictionary<string, object> data = new Dictionary<string, object>();
                                    reader.ReadObject(objectName =>
                                    {
                                        switch (objectName)
                                        {
                                            case "_time":
                                                time = (float)reader.ReadAsDouble();
                                                break;

                                            case "_lineIndex":
                                                lineIndex = (int)reader.ReadAsInt32();
                                                break;

                                            case "_lineLayer":
                                                lineLayer = (NoteLineLayer)reader.ReadAsInt32();
                                                break;

                                            case "_offsetDirection":
                                                offsetDirection = (OffsetDirection)reader.ReadAsInt32();
                                                break;

                                            case "_customData":
                                                reader.ReadToDictionary(data);
                                                break;

                                            default:
                                                reader.Skip();
                                                break;
                                        }
                                    });

                                    waypoints.Add(new WaypointData(time, lineIndex, lineLayer, offsetDirection, data));
                                });

                                break;

                            case "_obstacles":
                                reader.ReadObjectArray(() =>
                                {
                                    float time = default;
                                    int lineIndex = default;
                                    ObstacleType type = default;
                                    float duration = default;
                                    int width = default;
                                    Dictionary<string, object> data = new Dictionary<string, object>();
                                    reader.ReadObject(objectName =>
                                    {
                                        switch (objectName)
                                        {
                                            case "_time":
                                                time = (float)reader.ReadAsDouble();
                                                break;

                                            case "_lineIndex":
                                                lineIndex = (int)reader.ReadAsInt32();
                                                break;

                                            case "_type":
                                                type = (ObstacleType)reader.ReadAsInt32();
                                                break;

                                            case "_duration":
                                                duration = (float)reader.ReadAsDouble();
                                                break;

                                            case "_width":
                                                width = (int)reader.ReadAsInt32();
                                                break;

                                            case "_customData":
                                                reader.ReadToDictionary(data);
                                                break;

                                            default:
                                                reader.Skip();
                                                break;
                                        }
                                    });

                                    obstacles.Add(new ObstacleData(time, lineIndex, type, duration, width, data));
                                });

                                break;

                            case "_specialEventsKeywordFilters":
                                reader.ReadObject(propertyName =>
                                {
                                    if (propertyName.Equals("_keywords"))
                                    {
                                        reader.ReadObjectArray(() =>
                                        {
                                            string keyword = string.Empty;
                                            List<BeatmapEventType> specialEvents = new List<BeatmapEventType>();
                                            reader.ReadObject(objectName =>
                                            {
                                                switch (objectName)
                                                {
                                                    case "_keyword":
                                                        keyword = reader.ReadAsString();
                                                        break;

                                                    case "_specialEvents":
                                                        if (reader.TokenType != JsonToken.StartArray)
                                                        {
                                                            throw new JsonSerializationException("Was not array.");
                                                        }

                                                        reader.Read();
                                                        while (reader.TokenType != JsonToken.EndArray)
                                                        {
                                                            specialEvents.Add((BeatmapEventType)reader.ReadAsInt32());

                                                            if (!reader.Read())
                                                            {
                                                                throw new JsonSerializationException("Unexpected end when reading _specialEvents.");
                                                            }
                                                        }

                                                        break;

                                                    default:
                                                        reader.Skip();
                                                        break;
                                                }
                                            });

                                            keywords.Add(new SpecialEventsForKeyword(keyword, specialEvents));
                                        });
                                    }
                                    else
                                    {
                                        reader.Skip();
                                    }
                                });

                                break;

                            case "_customData":
                                reader.ReadToDictionary(customData, propertyName =>
                                {
                                    if (propertyName == "_customEvemts")
                                    {
                                        reader.ReadObjectArray(() =>
                                        {
                                            float time = default;
                                            string type = string.Empty;
                                            Dictionary<string, object> data = new Dictionary<string, object>();
                                            reader.ReadObject(objectName =>
                                            {
                                                switch (objectName)
                                                {
                                                    case "_time":
                                                        time = (float)reader.ReadAsDouble();
                                                        break;

                                                    case "_type":
                                                        type = reader.ReadAsString();
                                                        break;

                                                    case "_data":
                                                        reader.ReadToDictionary(data);
                                                        break;

                                                    default:
                                                        reader.Skip();
                                                        break;
                                                }
                                            });

                                            customEvents.Add(new CustomEventData(time, type, data));
                                        });
                                    }
                                });

                                break;
                        }
                    }
                }
            }

            return new CustomBeatmapSaveData(
                version,
                events.Cast<BeatmapSaveData.EventData>().ToList(),
                notes.Cast<BeatmapSaveData.NoteData>().ToList(),
                waypoints.Cast<BeatmapSaveData.WaypointData>().ToList(),
                obstacles.Cast<BeatmapSaveData.ObstacleData>().ToList(),
                new SpecialEventKeywordFiltersData(keywords),
                customData,
                customEvents);
        }

        internal new class EventData : BeatmapSaveData.EventData
        {
            internal EventData(float time, BeatmapEventType type, int value, Dictionary<string, object> customData)
                : base(time, type, value)
            {
                this.customData = customData;
            }

            internal Dictionary<string, object> customData { get; }
        }

        internal class CustomEventData
        {
            internal CustomEventData(float time, string type, Dictionary<string, object> data)
            {
                this.time = time;
                this.type = type;
                this.data = data;
            }

            internal float time { get; }

            internal string type { get; }

            internal Dictionary<string, object> data { get; }
        }

        internal new class NoteData : BeatmapSaveData.NoteData
        {
            internal NoteData(float time, int lineIndex, NoteLineLayer lineLayer, NoteType type, NoteCutDirection cutDirection, Dictionary<string, object> customData)
                : base(time, lineIndex, lineLayer, type, cutDirection)
            {
                this.customData = customData;
            }

            internal Dictionary<string, object> customData { get; }
        }

        internal new class WaypointData : BeatmapSaveData.WaypointData
        {
            public WaypointData(float time, int lineIndex, NoteLineLayer lineLayer, OffsetDirection offsetDirection, Dictionary<string, object> customData)
                : base(time, lineIndex, lineLayer, offsetDirection)
            {
                this.customData = customData;
            }

            internal Dictionary<string, object> customData { get; }
        }

        internal new class ObstacleData : BeatmapSaveData.ObstacleData
        {
            public ObstacleData(float time, int lineIndex, ObstacleType type, float duration, int width, Dictionary<string, object> customData)
                : base(time, lineIndex, type, duration, width)
            {
                this.customData = customData;
            }

            internal Dictionary<string, object> customData { get; }
        }
    }
}
