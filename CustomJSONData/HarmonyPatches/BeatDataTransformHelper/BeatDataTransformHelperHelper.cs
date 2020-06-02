using CustomJSONData.CustomBeatmap;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomJSONData.HarmonyPatches
{
    internal class BeatDataTransformHelperHelper
    {
        internal static void PostfixHelper(ref BeatmapData __result, BeatmapData beatmapData)
        {
            if (beatmapData is CustomBeatmapData customBeatmapData)
            {
                __result = new CustomBeatmapData(__result.beatmapLinesData, __result.beatmapEventData,
                    customBeatmapData.customEventData, customBeatmapData.customData, customBeatmapData.beatmapCustomData, customBeatmapData.levelCustomData);
            }
        }

        private static readonly MethodInfo _lineIndexGetter = typeof(BeatmapObjectData).GetProperty("lineIndex").GetGetMethod();
        private static readonly MethodInfo _clampLineIndex = SymbolExtensions.GetMethodInfo(() => ClampLineIndex(0));

        internal static IEnumerable<CodeInstruction> TranspilerHelper(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            bool foundGetter = false;
#pragma warning disable CS0252
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (instructionList[i].opcode == OpCodes.Callvirt &&
                    instructionList[i].operand == _clampLineIndex)
                {
                    foundGetter = true;
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Call, _clampLineIndex));
                }
            }
#pragma warning restore CS0252
            if (!foundGetter) Logger.Log("Failed to patch BeatDataTransformHelper!", IPA.Logging.Logger.Level.Error);
            return instructionList.AsEnumerable();
        }

        private static int ClampLineIndex(int input)
        {
            return Math.Min(Math.Max(input, 0), 3);
        }
    }
}