using static CustomJSONData.Trees;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBeatmapData : BeatmapData
    {
        public CustomEventData[] customEventData { get; internal set; }
        public dynamic beatmapCustomData { get; internal set; }
        public dynamic levelCustomData { get; internal set; }

        public CustomBeatmapData(BeatmapLineData[] beatmapLinesData, BeatmapEventData[] beatmapEventData, CustomEventData[] customEventData, dynamic customData, dynamic levelCustomData)
                          : base(beatmapLinesData, beatmapEventData)
        {
            this.customEventData = customEventData;
            this.beatmapCustomData = customData;
            this.levelCustomData = levelCustomData;
        }

        public override BeatmapData GetCopy()
        {
            BeatmapLineData[] beatmapLineDataCopy = GetBeatmapLineDataCopy();
            BeatmapEventData[] beatmapEventDataCopy = GetBeatmapEventDataCopy();
            CustomEventData[] customEventDataCopy = GetCustomEventDataCopy();
            return new CustomBeatmapData(beatmapLineDataCopy, beatmapEventDataCopy, customEventDataCopy, copy(beatmapCustomData), copy(levelCustomData));
        }

        private CustomEventData[] GetCustomEventDataCopy()
        {
            CustomEventData[] array = new CustomEventData[this.customEventData.Length];
            for (int i = 0; i < this.customEventData.Length; i++)
            {
                CustomEventData customEventData = this.customEventData[i];
                array[i] = customEventData;
            }
            return array;
        }
    }
}