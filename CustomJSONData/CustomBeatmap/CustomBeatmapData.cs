using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomJSONData.CustomBeatmap
{
    class CustomBeatmapData : BeatmapData
    {
        public CustomBeatmapData(BeatmapLineData[] beatmapLinesData, BeatmapEventData[] beatmapEventData, dynamic customData) : base(beatmapLinesData, beatmapEventData)
        {
            this.customData = customData;
        }
        public dynamic customData;
    }
}
