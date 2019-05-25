using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBeatmapData : BeatmapData
    {
        public CustomEventData[] customEventData { get; protected set; }

        public CustomBeatmapData(BeatmapLineData[] beatmapLinesData, BeatmapEventData[] beatmapEventData, CustomEventData[] customEventData) : base(beatmapLinesData, beatmapEventData)
        {
            this.customEventData = customEventData;
        }

        public override BeatmapData GetCopy()
        {
            BeatmapData baseCopy = base.GetCopy();
            CustomEventData[] copiedEvents = new CustomEventData[customEventData.Length];
            for (int i = 0; i < customEventData.Length; i++)
            {
                copiedEvents[i] = customEventData[i].GetCopy();
            }
            return new CustomBeatmapData(baseCopy.beatmapLinesData, baseCopy.beatmapEventData, copiedEvents);
        }
    }
}
