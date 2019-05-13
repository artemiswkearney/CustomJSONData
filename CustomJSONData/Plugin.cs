using CustomJSONData.Events;
using Harmony;
using IllusionPlugin;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace CustomJSONData
{
    public class Plugin : IPlugin
    {
        public string Name => "CustomJSONData";
        public string Version => "0.0.1";
        public static readonly List<string> licenses = new List<string>()
        {
        };
        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            var harmony = HarmonyInstance.Create("com.arti.BeatSaber.CustomJSONData");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
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
    }
}
