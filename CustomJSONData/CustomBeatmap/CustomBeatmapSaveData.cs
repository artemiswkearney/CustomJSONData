namespace CustomJSONData.CustomBeatmap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class CustomBeatmapSaveData : BeatmapSaveData
    {
        public CustomBeatmapSaveData(List<BeatmapSaveData.EventData> events, List<BeatmapSaveData.NoteData> notes,
            List<BeatmapSaveData.LongNoteData> longNotes, List<BeatmapSaveData.ObstacleData> obstacles) : base(events, notes, longNotes, obstacles)
        {
            // Default values for these fields
            // We deserialize using NullValueHandling.Ignore so that these fields do not get overwritten if they are missing in the json string
            _version = string.Empty;
            _customData = Trees.Tree();
            _events = new List<EventData>();
            _longNotes = new List<LongNoteData>();
            _notes = new List<NoteData>();
            _obstacles = new List<ObstacleData>();
        }

        [JsonIgnore]
        public List<CustomEventData> customEvents { get; protected set; } = new List<CustomEventData>();

        [JsonIgnore]
        public dynamic customData { get; protected set; }

        [Serializable]
        private class CustomEventsSaveData
        {
#pragma warning disable 0649

            [JsonProperty]
            public CustomData _customData;

            [Serializable]
            public class CustomData
            {
                [JsonProperty]
                public List<CustomEventData> _customEvents;
            }

#pragma warning restore 0649
        }

        public new static CustomBeatmapSaveData DeserializeFromJSONString(string stringData)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>() { new ExpandoObjectConverter() }
            };
            CustomBeatmapSaveData beatmap = JsonConvert.DeserializeObject<CustomBeatmapSaveData>(stringData, settings);

            CustomEventsSaveData customEvents = JsonConvert.DeserializeObject<CustomEventsSaveData>(stringData);
            if (customEvents._customData != null && customEvents._customData._customEvents != null)
            {
                beatmap.customEvents = customEvents._customData._customEvents;
            }

            return beatmap;
        }

        [JsonProperty]
        protected new string _version
        {
            get => base._version;
            set => base._version = value;
        }

        [JsonProperty]
        protected new List<EventData> _events
        {
            get => base._events.Cast<EventData>().ToList();
            set => base._events = value.Cast<BeatmapSaveData.EventData>().ToList();
        }

        [JsonProperty]
        [JsonConverter(typeof(ExpandoObjectConverter))]
        protected dynamic _customData
        {
            get => customData;
            set => customData = value;
        }

        [JsonProperty]
        protected new List<NoteData> _notes
        {
            get => base._notes.Cast<NoteData>().ToList();
            set => base._notes = value.Cast<BeatmapSaveData.NoteData>().ToList();
        }

        [JsonProperty]
        protected new List<LongNoteData> _longNotes
        {
            get => base._longNotes.Cast<LongNoteData>().ToList();
            set => base._longNotes = value.Cast<BeatmapSaveData.LongNoteData>().ToList();
        }

        [JsonProperty]
        protected new List<ObstacleData> _obstacles
        {
            get => base._obstacles.Cast<ObstacleData>().ToList();
            set => base._obstacles = value.Cast<BeatmapSaveData.ObstacleData>().ToList();
        }

        [Serializable]
        public new class EventData : BeatmapSaveData.EventData
        {
            public EventData(float time, BeatmapEventType type, int value) : base(time, type, value)
            {
            }

            [JsonIgnore]
            public dynamic customData => _customData;

            [JsonProperty]
            protected new float _time
            {
                get => base._time;
                set => base._time = value;
            }

            [JsonProperty]
            protected new BeatmapEventType _type
            {
                get => base._type;
                set => base._type = value;
            }

            [JsonProperty]
            protected new int _value
            {
                get => base._value;
                set => base._value = value;
            }

            [JsonProperty]
            [JsonConverter(typeof(ExpandoObjectConverter))]
            protected dynamic _customData;
        }

        [Serializable]
        public class CustomEventData : ITime
        {
            public CustomEventData(float time, string type, dynamic data)
            {
                _time = time;
                _type = type;
                _data = data;
            }

            [JsonIgnore]
            public float time => _time;

            [JsonIgnore]
            public string type => _type;

            [JsonIgnore]
            public dynamic data => _data;

            public void MoveTime(float offset)
            {
                _time += offset;
            }

            [JsonProperty]
            protected float _time;

            [JsonProperty]
            protected string _type;

            [JsonProperty]
            [JsonConverter(typeof(ExpandoObjectConverter))]
            protected dynamic _data;
        }

        [Serializable]
        public new class NoteData : BeatmapSaveData.NoteData
        {
            public NoteData(float time, int lineIndex, NoteLineLayer lineLayer, NoteType type, NoteCutDirection cutDirection) : base(time, lineIndex, lineLayer, type, cutDirection)
            {
            }

            [JsonIgnore]
            public dynamic customData => _customData;

            [JsonProperty]
            protected new float _time
            {
                get => base._time;
                set => base._time = value;
            }

            [JsonProperty]
            protected new int _lineIndex
            {
                get => base._lineIndex;
                set => base._lineIndex = value;
            }

            [JsonProperty]
            protected new NoteLineLayer _lineLayer
            {
                get => base._lineLayer;
                set => base._lineLayer = value;
            }

            [JsonProperty]
            protected new NoteType _type
            {
                get => base._type;
                set => base._type = value;
            }

            [JsonProperty]
            protected new NoteCutDirection _cutDirection
            {
                get => base._cutDirection;
                set => base._cutDirection = value;
            }

            [JsonProperty]
            [JsonConverter(typeof(ExpandoObjectConverter))]
            protected dynamic _customData;
        }

        [Serializable]
        public new class LongNoteData : BeatmapSaveData.LongNoteData
        {
            public LongNoteData(float time, int lineIndex, NoteLineLayer lineLayer, LongNoteType type, NoteCutDirection cutDirection, float duration)
                : base(time, lineIndex, lineLayer, type, cutDirection, duration)
            {
            }

            [JsonIgnore]
            public dynamic customData => _customData;

            [JsonProperty]
            protected new float _time
            {
                get => base._time;
                set => base._time = value;
            }

            [JsonProperty]
            protected new int _lineIndex
            {
                get => base._lineIndex;
                set => base._lineIndex = value;
            }

            [JsonProperty]
            protected new NoteLineLayer _lineLayer
            {
                get => base._lineLayer;
                set => base._lineLayer = value;
            }

            [JsonProperty]
            protected new LongNoteType _type
            {
                get => base._type;
                set => base._type = value;
            }

            [JsonProperty]
            protected new NoteCutDirection _cutDirection
            {
                get => base._cutDirection;
                set => base._cutDirection = value;
            }

            [JsonProperty]
            protected new float _duration
            {
                get => base._duration;
                set => base._duration = value;
            }

            [JsonProperty]
            [JsonConverter(typeof(ExpandoObjectConverter))]
            protected dynamic _customData;
        }

        [Serializable]
        public new class ObstacleData : BeatmapSaveData.ObstacleData
        {
            public ObstacleData(float time, int lineIndex, ObstacleType type, float duration, int width) : base(time, lineIndex, type, duration, width)
            {
            }

            [JsonIgnore]
            public dynamic customData => _customData;

            [JsonProperty]
            protected new float _time
            {
                get => base._time;
                set => base._time = value;
            }

            [JsonProperty]
            protected new int _lineIndex
            {
                get => base._lineIndex;
                set => base._lineIndex = value;
            }

            [JsonProperty]
            protected new ObstacleType _type
            {
                get => base._type;
                set => base._type = value;
            }

            [JsonProperty]
            protected new float _duration
            {
                get => base._duration;
                set => base._duration = value;
            }

            [JsonProperty]
            protected new int _width
            {
                get => base._width;
                set => base._width = value;
            }

            [JsonProperty]
            [JsonConverter(typeof(ExpandoObjectConverter))]
            protected dynamic _customData;
        }
    }
}
