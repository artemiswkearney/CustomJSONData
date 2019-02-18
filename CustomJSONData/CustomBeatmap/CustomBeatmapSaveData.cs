using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using UnityEngine;

using CustomDataConverter = Newtonsoft.Json.Converters.ExpandoObjectConverter;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBeatmapSaveData
    {
        // internal class CustomDataConverter : ExpandoObjectConverter { }
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

        protected const string kCurrentVersion = "1.5.0";

        [JsonProperty]
        protected string _version;

        [JsonProperty]
        protected float _beatsPerMinute;

        [JsonProperty]
        protected float _noteJumpSpeed;

        [JsonProperty]
        protected float _shuffle;

        [JsonProperty]
        protected float _shufflePeriod;

        [JsonProperty]
        [JsonConverter(typeof(CustomDataConverter))]
        public dynamic customData;

        [JsonProperty]
        protected List<BeatmapSaveData.EventData> _events;

        [JsonProperty]
        protected List<NoteData> _notes;

        [JsonProperty]
        protected List<ObstacleData> _obstacles;

        // All fields below this line originate from SongLoaderPlugin
        [JsonProperty]
        public List<string> _warnings;

        [JsonProperty]
        public List<string> _suggestions;

        [JsonProperty]
        public List<string> _requirements;

        [JsonProperty]
        public RGBColor _colorLeft;

        [JsonProperty]
        public RGBColor _colorRight;

        [JsonProperty]
        public int? _noteJumpStartBeatOffset = null;

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
            public dynamic customData;

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
            public dynamic customData;
 
            public static implicit operator BeatmapSaveData.ObstacleData(ObstacleData od)
            {
                return new BeatmapSaveData.ObstacleData(od.time, od.lineIndex, od.type, od.duration, od.width);
            }

            public void MoveTime(float offset)
            {
                _time += offset;
            }
        }

        [Serializable]
        public class RGBColor
        {
            // Scale is 0-1
            [JsonProperty]
            public float r;

            [JsonProperty]
            public float g;

            [JsonProperty]
            public float b;
        }
    }
}
