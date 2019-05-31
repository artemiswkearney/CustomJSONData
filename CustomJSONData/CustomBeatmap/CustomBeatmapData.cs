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
        public CustomEventData[] customEventData { get; protected set; }
        public dynamic customData { get; protected set; }
        public dynamic levelCustomData { get; protected set; }

        public CustomBeatmapData(BeatmapLineData[] beatmapLinesData, BeatmapEventData[] beatmapEventData, CustomEventData[] customEventData, dynamic customData, dynamic levelCustomData) : base(beatmapLinesData, beatmapEventData)
        {
            this.customEventData = customEventData;
            this.customData = customData;
            this.levelCustomData = levelCustomData;
        }

        public override BeatmapData GetCopy()
        {
            BeatmapData baseCopy = base.GetCopy();
            CustomEventData[] copiedEvents = new CustomEventData[customEventData.Length];
            for (int i = 0; i < customEventData.Length; i++)
            {
                copiedEvents[i] = customEventData[i].GetCopy();
            }
            return new CustomBeatmapData(baseCopy.beatmapLinesData, baseCopy.beatmapEventData, copiedEvents, copy(customData), copy(levelCustomData));
        }
    }
}
