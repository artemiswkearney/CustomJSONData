namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;

    public class CustomWaypointData : WaypointData
    {
        public CustomWaypointData(float time, int lineIndex, NoteLineLayer noteLineLayer, OffsetDirection offsetDirection, Dictionary<string, object> customData)
            : base(time, lineIndex, noteLineLayer, offsetDirection)
        {
            this.customData = customData;
        }

        public Dictionary<string, object> customData { get; }

        public override BeatmapObjectData GetCopy()
        {
            return new CustomWaypointData(time, lineIndex, noteLineLayer, offsetDirection, new Dictionary<string, object>(customData));
        }
    }
}
