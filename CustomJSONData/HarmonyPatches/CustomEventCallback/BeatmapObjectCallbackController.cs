using HarmonyLib;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapObjectCallbackController))]
    [HarmonyPatch("Start")]
    internal class BeatmapObjectCallbackControllerStart
    {
        private static void Prefix(BeatmapObjectCallbackController __instance)
        {
            __instance.gameObject.AddComponent<CustomEventCallbackController>()._beatmapObjectCallbackController = __instance;
        }
    }
}