using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBeatmapEventData : BeatmapEventData
    {
        public dynamic customData { get; private set; }
        public CustomBeatmapEventData(float time, BeatmapEventType type, int value, dynamic customData) : base(time, type, value)
        {
            this.customData = customData;
        }
        public BeatmapEventData GetCopy()
        {
            return new CustomBeatmapEventData(time, type, value, Trees.copy(customData));
        }
    }
}
