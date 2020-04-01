using HarmonyLib;
using IPA;
using IPA.Logging;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace CustomJSONData
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static readonly List<string> licenses = new List<string>()
        {
        };
        public static Logger logger;
        [Init]
        public void Init(Logger l)
        {
            logger = l;
        }
        [OnStart]
        public void OnApplicationStart()
        {
            var harmony = new Harmony("com.arti.BeatSaber.CustomJSONData");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
