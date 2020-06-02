using HarmonyLib;
using IPA;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;

namespace CustomJSONData
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        [Init]
        public void Init(IPALogger l)
        {
            Logger.logger = l;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            var harmony = new Harmony("com.noodle.BeatSaber.CustomJSONData");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}