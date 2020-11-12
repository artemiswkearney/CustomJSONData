namespace CustomJSONData.CustomBeatmap
{
    public class CustomWaypointData : WaypointData
    {
        public dynamic customData { get; private set; }

        public CustomWaypointData(float time, int lineIndex, NoteLineLayer noteLineLayer, OffsetDirection offsetDirection, dynamic customData) : base(time, lineIndex, noteLineLayer, offsetDirection)
        {
            this.customData = customData;
        }

        public override BeatmapObjectData GetCopy()
        {
            return new CustomWaypointData(time, lineIndex, noteLineLayer, offsetDirection, Trees.copy(customData));
        }
    }
}
