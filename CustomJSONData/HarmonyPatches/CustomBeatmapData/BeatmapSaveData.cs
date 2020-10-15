namespace CustomJSONData.HarmonyPatches
{
    using CustomJSONData.CustomBeatmap;
    using HarmonyLib;

    [HarmonyPatch(typeof(BeatmapSaveData))]
    [HarmonyPatch("DeserializeFromJSONString")]
    internal class BeatmapSaveDataDeserializeFromJSONString
    {
        private static bool Prefix(ref BeatmapSaveData __result, string stringData)
        {
            __result = CustomBeatmapSaveData.DeserializeFromJSONString(stringData);
            return false;
        }
    }
}
