using CustomJSONData.CustomLevelInfo;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelInfoSaveData))]
    [HarmonyPatch("DeserializeFromJSONString")]
    internal class StandardLevelInfoSaveDataDeserializeFromJSONString
    {
        private static readonly MethodInfo CustomDeserialize = SymbolExtensions.GetMethodInfo(() => CustomLevelInfoSaveData.DeserializeFromJSONString(null, null));
        private static readonly MethodInfo FromJson = SymbolExtensions.GetMethodInfo(() => UnityEngine.JsonUtility.FromJson<StandardLevelInfoSaveData>(null));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            bool foundDeserialize = false;
#pragma warning disable CS0252
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (!foundDeserialize &&
                    instructionList[i].opcode == OpCodes.Call &&
                    instructionList[i].operand == FromJson)
                {
                    foundDeserialize = true;
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Call, CustomDeserialize));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldarg_0));
                }
            }
#pragma warning restore CS0252
            if (!foundDeserialize) Logger.Log("Failed to patch DeserializeFromJSONString in StandardLevelInfoSaveData!", IPA.Logging.Logger.Level.Error);
            return instructionList.AsEnumerable();
        }
    }
}