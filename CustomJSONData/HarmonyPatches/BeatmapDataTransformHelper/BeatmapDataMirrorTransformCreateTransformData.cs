namespace CustomJSONData.HarmonyPatches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using CustomJSONData.CustomBeatmap;
    using HarmonyLib;

    [HarmonyPatch(typeof(BeatmapDataMirrorTransform))]
    [HarmonyPatch("CreateTransformedData")]
    internal class BeatDataMirrorTransformCreateTransformData
    {
        private static CustomBeatmapData CopyCustomData(CustomBeatmapData result, IReadonlyBeatmapData beatmapData)
        {
            if (beatmapData is CustomBeatmapData customBeatmapData)
            {
                customBeatmapData.CopyCustomData(customBeatmapData, result);
            }
            
            // keep result on the stack
            return result;
        }

        private static readonly ConstructorInfo _beatmapDataCtor = typeof(BeatmapData).GetConstructors().First();
        private static readonly ConstructorInfo _customBeatmapDataCtor = typeof(CustomBeatmapData).GetConstructors().First();
        private static readonly MethodInfo _copyCustomData = SymbolExtensions.GetMethodInfo(() => CopyCustomData(null, null));

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            bool foundCtor = false;
#pragma warning disable CS0252
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (!foundCtor &&
                    instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _beatmapDataCtor)
                {
                    foundCtor = true;
                    instructionList[i].operand = _customBeatmapDataCtor;
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                    instructionList.Insert(i + 2, new CodeInstruction(OpCodes.Call, _copyCustomData));
                }
            }
#pragma warning restore CS0252
            if (!foundCtor)
            {
                Logger.Log("Failed to patch BeatDataTransformHelper!", IPA.Logging.Logger.Level.Error);
            }

            return instructionList.AsEnumerable();
        }
    }
}
