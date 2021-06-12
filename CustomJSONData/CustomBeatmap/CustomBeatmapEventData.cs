namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;

    public class CustomBeatmapEventData : BeatmapEventData
    {
        private CustomBeatmapEventData(float time, BeatmapEventType type, int value, Dictionary<string, object> customData)
            : base(time, type, value)
        {
            this.customData = customData;
        }

        public Dictionary<string, object> customData { get; }
    }
}
