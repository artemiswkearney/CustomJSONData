using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomJSONData.CustomBeatmap
{
    class CustomBeatmapSaveData
    {
        public CustomBeatmapSaveData() { }
        public CustomBeatmapSaveData(List<BeatmapSaveData.EventData> events, List<NoteData> notes, List<ObstacleData> obstacles)
        {
            this._version = kCurrentVersion;
            this._events = events;
            this._notes = notes;
            this._obstacles = obstacles;
        }

        [JsonIgnore]
        public string version
        {
            get
            {
                return this._version;
            }
        }

        [JsonIgnore]
        public float beatsPerMinute
        {
            get
            {
                return this._beatsPerMinute;
            }
        }

        [JsonIgnore]
        public float noteJumpSpeed
        {
            get
            {
                return this._noteJumpSpeed;
            }
        }

        [JsonIgnore]
        public List<BeatmapSaveData.EventData> events
        {
            get
            {
                return this._events;
            }
        }

        [JsonIgnore]
        public List<NoteData> notes
        {
            get
            {
                return this._notes;
            }
        }

        [JsonIgnore]
        public List<ObstacleData> obstacles
        {
            get
            {
                return this._obstacles;
            }
        }

        public virtual string SerializeToJSONString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static CustomBeatmapSaveData DeserializeFromJSONString(string stringData)
        {
            CustomBeatmapSaveData beatmap = JsonConvert.DeserializeObject<CustomBeatmapSaveData>(stringData, new ExpandoObjectConverter());
            if (beatmap == null || beatmap.version != kCurrentVersion)
            {
                return null;
            }
            return beatmap;
        }

        protected const string kCurrentVersion = "Custom_1.0.0";

        [JsonProperty]
        protected string _version;

        [JsonProperty]
        protected float _beatsPerMinute;

        [JsonProperty]
        protected float _noteJumpSpeed;

        [JsonProperty]
        public dynamic customData;

        [JsonProperty]
        protected List<BeatmapSaveData.EventData> _events;

        [JsonProperty]
        protected List<NoteData> _notes;

        [JsonProperty]
        protected List<ObstacleData> _obstacles;

        [Serializable]
        public class NoteData
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
            public dynamic customData;
        }

        [Serializable]
        public class ObstacleData
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
            public dynamic customData;
        }
    }
}
