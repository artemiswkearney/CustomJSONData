using HarmonyLib;
using System.Collections.Generic;
using CustomJSONData.CustomBeatmap;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataStaticLightsTransform), "CreateTransformedData")]
    class BeatmapDataStaticLightsTransformCreateTransformedData
    {
        static bool Prefix(BeatmapData beatmapData, ref BeatmapData __result)
        {
            BeatmapEventData[] beatmapEventData = beatmapData.beatmapEventData;
            List<BeatmapEventData> list = new List<BeatmapEventData>(beatmapEventData.Length);
            list.Add(new BeatmapEventData(0f, BeatmapEventType.Event0, 1));
            list.Add(new BeatmapEventData(0f, BeatmapEventType.Event4, 1));
            foreach (BeatmapEventData beatmapEventData2 in beatmapEventData)
            {
                if (beatmapEventData2.type.IsRotationEvent())
                {
                    list.Add(beatmapEventData2);
                }
            }

            if (beatmapData is CustomBeatmapData customBeatmap)
                __result = new CustomBeatmapData(beatmapData.GetBeatmapLineDataCopy(), list.ToArray(), customBeatmap.customEventData, customBeatmap.beatmapCustomData, customBeatmap.levelCustomData);
            else
                __result = new BeatmapData(beatmapData.GetBeatmapLineDataCopy(), list.ToArray());
            return false;
        }
    }
}
