using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomJSONData.Trees;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBeatmapData : BeatmapData
    {
        public Dictionary<string, List<CustomEventData>> customEventData { get; protected set; }
        public dynamic beatmapCustomData { get; protected set; }
        public dynamic levelCustomData { get; protected set; }

        public CustomBeatmapData(BeatmapLineData[] beatmapLinesData, BeatmapEventData[] beatmapEventData, Dictionary<string, List<CustomEventData>> customEventData, dynamic customData, dynamic levelCustomData) : base(beatmapLinesData, beatmapEventData)
        {
            this.customEventData = customEventData;
            this.beatmapCustomData = customData;
            this.levelCustomData = levelCustomData;
        }

        public override BeatmapData GetCopy()
        {
            BeatmapData baseCopy = base.GetCopy();
            var copiedEvents = new Dictionary<string, List<CustomEventData>>();
            foreach (var pair in customEventData)
            {
                copiedEvents[pair.Key] = pair.Value.Select(e => e.GetCopy()).ToList();
            }
            return new CustomBeatmapData(baseCopy.beatmapLinesData, baseCopy.beatmapEventData, copiedEvents, copy(beatmapCustomData), copy(levelCustomData));
        }
    }
}
