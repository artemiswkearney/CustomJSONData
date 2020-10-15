namespace CustomJSONData.HarmonyPatches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(BeatmapObjectCallbackController))]
    [HarmonyPatch("Start")]
    internal class BeatmapObjectCallbackControllerStart
    {
        private static void Prefix(BeatmapObjectCallbackController __instance)
        {
            __instance.gameObject.AddComponent<CustomEventCallbackController>()._beatmapObjectCallbackController = __instance;
        }
    }

    [HarmonyPatch(typeof(BeatmapObjectCallbackController))]
    [HarmonyPatch("SetNewBeatmapData")]
    internal class BeatmapObjectCallbackControllerSetNewBeatmapData
    {
        private static void Postfix(BeatmapObjectCallbackController __instance, BeatmapData beatmapData)
        {
            __instance.GetComponent<CustomEventCallbackController>()?.SetNewBeatmapData(beatmapData);
        }
    }
}
