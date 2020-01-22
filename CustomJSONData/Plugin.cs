using Harmony;
using IPA;
using IPA.Logging;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace CustomJSONData
{
    public class Plugin : IBeatSaberPlugin
    {
        public string Name => "CustomJSONData";
        public string Version => "0.0.3";
        public static readonly List<string> licenses = new List<string>()
        {
        };
        public static Logger logger;
        public void Init(Logger l)
        {
            logger = l;
        }
        public void OnApplicationStart()
        {
            var harmony = HarmonyInstance.Create("com.arti.BeatSaber.CustomJSONData");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        public void OnApplicationQuit()
        {
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
        }

        public void OnSceneUnloaded(Scene scene)
        {
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
        }
    }
}
