namespace CustomJSONData
{
    using System.Reflection;
    using HarmonyLib;
    using IPA;
    using IPALogger = IPA.Logging.Logger;

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
            Harmony harmony = new Harmony("com.arti.BeatSaber.CustomJSONData");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
