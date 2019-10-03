using CustomJSONData.CustomBeatmap;
using CustomJSONData.CustomLevelInfo;

namespace CustomJSONData.Utils
{
    public static class Extensions
    {
        public static dynamic getBeatmapCustomData(this IDifficultyBeatmap difficultyBeatmap)
        {
            if (difficultyBeatmap is CustomDifficultyBeatmap cd)
                if (cd.beatmapData is CustomBeatmapData cb)
                    return cb.beatmapCustomData;
            return null;
        }
        public static dynamic getLevelCustomData(this IDifficultyBeatmap difficultyBeatmap)
        {
            if (difficultyBeatmap is CustomDifficultyBeatmap cd)
                if (cd.beatmapData is CustomBeatmapData cb)
                    return cb.levelCustomData;
            return null;
        }
        public static dynamic getLevelCustomData(this IPreviewBeatmapLevel previewBeatmapLevel)
        {
            if (previewBeatmapLevel is CustomPreviewBeatmapLevel cpbl)
                if (cpbl.standardLevelInfoSaveData is CustomLevelInfoSaveData clisd)
                    return clisd.customData;
            return null;
        }
    }
}