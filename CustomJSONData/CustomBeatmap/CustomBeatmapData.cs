using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBeatmapData : BeatmapData
    {
        public CustomBeatmapData(BeatmapLineData[] beatmapLinesData, BeatmapEventData[] beatmapEventData, dynamic customData,
            List<string> warnings, List<string> suggestions, List<string> requirements,
            CustomBeatmapSaveData.RGBColor leftColor, CustomBeatmapSaveData.RGBColor rightColor, int? noteJumpStartBeatOffset
            ) : base(beatmapLinesData, beatmapEventData)
        {
            this.customData = customData;
            this.warnings = warnings;
            this.suggestions = suggestions;
            this.requirements = requirements;
            this.leftColor = leftColor;
            this.rightColor = rightColor;
            this.noteJumpStartBeatOffset = noteJumpStartBeatOffset;
        }
        public dynamic customData;
        public List<string> warnings, suggestions, requirements;
        public CustomBeatmapSaveData.RGBColor leftColor, rightColor;
        public int? noteJumpStartBeatOffset;
    }
}
