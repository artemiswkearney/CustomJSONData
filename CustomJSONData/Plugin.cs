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
            @"
(from https://github.com/pardeike/Harmony; patch priority algorithm and ""instance"" concept used in event system)

MIT License

Copyright (c) 2017 Andreas Pardeike

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.",
        };
        public EventSystemInstance events;
        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            events = EventSystemInstance.create("com.arti.BeatSaber.CustomJSONData");
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
