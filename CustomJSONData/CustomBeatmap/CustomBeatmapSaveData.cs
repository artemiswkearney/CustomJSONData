using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

using CustomDataConverter = Newtonsoft.Json.Converters.ExpandoObjectConverter;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBeatmapSaveData
    {
        // internal class CustomDataConverter : ExpandoObjectConverter { }
        public CustomBeatmapSaveData() { }
        public CustomBeatmapSaveData(List<EventData> events, List<NoteData> notes, List<ObstacleData> obstacles)
        {
            _version = kCurrentVersion;
            _events = events;
            _notes = notes;
            _obstacles = obstacles;
        }

        [JsonIgnore]
        public string version
        {
            get
            {
                return _version;
            }
        }

        [JsonIgnore]
        public List<EventData> events
        {
            get
            {
                return _events;
            }
        }

        [JsonIgnore]
        public List<CustomEventData> customEvents
        {
            get
            {
                return _customEvents;
            }
        }

        [JsonIgnore]
        public List<NoteData> notes
        {
            get
            {
                return _notes;
            }
        }

        [JsonIgnore]
        public List<ObstacleData> obstacles
        {
            get
            {
                return _obstacles;
            }
        }

        public virtual string SerializeToJSONString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static CustomBeatmapSaveData DeserializeFromJSONString(string stringData)
        {
            CustomBeatmapSaveData beatmap = JsonConvert.DeserializeObject<CustomBeatmapSaveData>(stringData, new CustomDataConverter());
            if (beatmap == null || beatmap.version != kCurrentVersion)
            {
                // return null;
            }
            return beatmap;
        }

        protected const string kCurrentVersion = "2.0.0";

        [JsonProperty]
        protected string _version;

        [JsonProperty]
        protected List<EventData> _events;

        [JsonProperty]
        protected List<CustomEventData> _customEvents;

        [JsonProperty]
        protected List<NoteData> _notes;

        [JsonProperty]
        protected List<ObstacleData> _obstacles;

        [Serializable]
        public class EventData : BeatmapSaveData.ITime
        {
            public EventData() { }
            public EventData(float time, BeatmapEventType type, int value)
            {
                this._time = time;
                this._type = type;
                this._value = value;
            }

            [JsonIgnore]
            public float time
            {
                get
                {
                    return _time;
                }
            }

            [JsonIgnore]
            public BeatmapEventType type
            {
                get
                {
                    return _type;
                }
            }

            [JsonIgnore]
            public int value
            {
                get
                {
                    return _value;
                }
            }

            [JsonIgnore]
            public dynamic customData
            {
                get
                {
                    return _customData;
                }
            }

            public void MoveTime(float offset)
            {
                _time += offset;
            }

            [JsonProperty]
            protected float _time;

            [JsonProperty]
            protected BeatmapEventType _type;

            [JsonProperty]
            protected int _value;

            [JsonProperty]
            [JsonConverter(typeof(CustomDataConverter))]
            protected dynamic _customData;

            public static implicit operator BeatmapSaveData.EventData(EventData ed)
            {
                return new BeatmapSaveData.EventData(ed.time, ed.type, ed.value);
            }
        }

        [Serializable]
        public class CustomEventData : BeatmapSaveData.ITime
        {
            public CustomEventData() { }
            public CustomEventData(float time, string type, dynamic data)
            {
                this._time = time;
                this._type = type;
                this._data = data;
            }

            [JsonIgnore]
            public float time
            {
                get
                {
                    return _time;
                }
            }

            [JsonIgnore]
            public string type
            {
                get
                {
                    return _type;
                }
            }

            [JsonIgnore]
            public dynamic data
            {
                get
                {
                    return _data;
                }
            }

            public void MoveTime(float offset)
            {
                _time += offset;
            }

            [JsonProperty]
            protected float _time;

            [JsonProperty]
            protected string _type;

            [JsonProperty]
            [JsonConverter(typeof(CustomDataConverter))]
            protected dynamic _data;
        }

        [Serializable]
        public class NoteData : BeatmapSaveData.ITime
        {
            public NoteData() { }
            public NoteData(float time, int lineIndex, NoteLineLayer lineLayer, NoteType type, NoteCutDirection cutDirection)
            {
                this._time = time;
                this._lineIndex = lineIndex;
                this._lineLayer = lineLayer;
                this._type = type;
                this._cutDirection = cutDirection;
            }

            [JsonIgnore]
            public float time
            {
                get
                {
                    return this._time;
                }
            }

            [JsonIgnore]
            public int lineIndex
            {
                get
                {
                    return this._lineIndex;
                }
            }

            [JsonIgnore]
            public NoteLineLayer lineLayer
            {
                get
                {
                    return this._lineLayer;
                }
            }

            [JsonIgnore]
            public NoteType type
            {
                get
                {
                    return this._type;
                }
            }

            [JsonIgnore]
            public NoteCutDirection cutDirection
            {
                get
                {
                    return this._cutDirection;
                }
            }

            [JsonProperty]
            protected float _time;

            [JsonProperty]
            protected int _lineIndex;

            [JsonProperty]
            protected NoteLineLayer _lineLayer;

            [JsonProperty]
            protected NoteType _type;

            [JsonProperty]
            protected NoteCutDirection _cutDirection;

            [JsonProperty]
            [JsonConverter(typeof(CustomDataConverter))]
            public dynamic _customData;

            public static implicit operator BeatmapSaveData.NoteData(NoteData nd)
            {
                return new BeatmapSaveData.NoteData(nd.time, nd.lineIndex, nd.lineLayer, nd.type, nd.cutDirection);
            }

            public void MoveTime(float offset)
            {
                _time += offset;
            }
        }

        [Serializable]
        public class ObstacleData : BeatmapSaveData.ITime
        {
            public ObstacleData() { }
            public ObstacleData(float time, int lineIndex, ObstacleType type, float duration, int width)
            {
                this._time = time;
                this._lineIndex = lineIndex;
                this._type = type;
                this._duration = duration;
                this._width = width;
            }

            [JsonIgnore]
            public float time
            {
                get
                {
                    return this._time;
                }
            }

            [JsonIgnore]
            public int lineIndex
            {
                get
                {
                    return this._lineIndex;
                }
            }

            [JsonIgnore]
            public ObstacleType type
            {
                get
                {
                    return this._type;
                }
            }

            [JsonIgnore]
            public float duration
            {
                get
                {
                    return this._duration;
                }
            }

            [JsonIgnore]
            public int width
            {
                get
                {
                    return this._width;
                }
            }

            [JsonProperty]
            protected float _time;

            [JsonProperty]
            protected int _lineIndex;

            [JsonProperty]
            protected ObstacleType _type;

            [JsonProperty]
            protected float _duration;

            [JsonProperty]
            protected int _width;

            [JsonProperty]
            [JsonConverter(typeof(CustomDataConverter))]
            public dynamic _customData;
 
            public static implicit operator BeatmapSaveData.ObstacleData(ObstacleData od)
            {
                return new BeatmapSaveData.ObstacleData(od.time, od.lineIndex, od.type, od.duration, od.width);
            }

            public void MoveTime(float offset)
            {
                _time += offset;
            }
        }

        /*
        /// <summary>
        /// Used to deserialize vanilla Beat Saber lighting events, as Newtonsoft.JSON doesn't get them right on its own
        /// </summary>
        public class BeatmapEventConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(BeatmapSaveData.EventData);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                return new BeatmapSaveData.EventData((float)jo["_time"], (BeatmapEventType)((int)jo["_type"]), (int)jo["_value"]);
            }

            public override bool CanWrite => false;

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
        */
    }
}
