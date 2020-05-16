using CustomJSONData.CustomBeatmap;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static CustomJSONData.Trees;

namespace CustomJSONData.HarmonyExtensions
{
    [HarmonyPatch(typeof(BeatmapDataLoader))]
    [HarmonyPatch("GetBeatmapDataFromBeatmapSaveData")]
    internal class BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData
    {
        private static void Postfix(ref BeatmapData __result)
        {
            __result = new CustomBeatmapData(__result.beatmapLinesData, __result.beatmapEventData, new CustomEventData[0], Tree(), Tree());
        }

        private static readonly ConstructorInfo NoteDataCtor = typeof(NoteData).GetConstructors().First();
        private static readonly ConstructorInfo CustomNoteDataCtor = typeof(CustomNoteData).GetConstructors().First();
        private static readonly MethodInfo NoteCustomData = SymbolExtensions.GetMethodInfo(() => GetNoteCustomData(null));

        private static readonly ConstructorInfo ObstacleDataCtor = typeof(ObstacleData).GetConstructors().First();
        private static readonly ConstructorInfo CustomObstacleDataCtor = typeof(CustomObstacleData).GetConstructors().First();
        private static readonly MethodInfo ObstacleCustomData = SymbolExtensions.GetMethodInfo(() => GetObstacleCustomData(null));

        private static readonly ConstructorInfo EventDataCtor = typeof(BeatmapEventData).GetConstructors().First();
        private static readonly ConstructorInfo CustomEventDataCtor = typeof(CustomBeatmapEventData).GetConstructors().First();
        private static readonly MethodInfo EventCustomData = SymbolExtensions.GetMethodInfo(() => GetEventCustomData(null));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            bool foundNoteData = false;
            bool foundObstacleData = false;
            bool foundEventData = false;
#pragma warning disable CS0252
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == NoteDataCtor)
                {
                    foundNoteData = true;
                    instructionList[i].operand = CustomNoteDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, NoteCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 18));
                }
                if (instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == ObstacleDataCtor)
                {
                    foundObstacleData = true;
                    instructionList[i].operand = CustomObstacleDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, ObstacleCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 29));
                }
                if (instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == EventDataCtor)
                {
                    instructionList[i].operand = CustomEventDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, EventCustomData));
                    if (!foundEventData) instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 38));
                    else instructionList.Insert(i, new CodeInstruction(OpCodes.Ldnull));

                    foundEventData = true;
                }
            }
#pragma warning restore CS0252
            if (!foundNoteData || !foundObstacleData || !foundEventData) Logger.Log("Failed to patch GetBeatmapDataFromBeatmapSaveData in BeatmapDataLoader!", IPA.Logging.Logger.Level.Error);
            return instructionList.AsEnumerable();
        }

        private static dynamic GetNoteCustomData(BeatmapSaveData.NoteData noteSaveData)
        {
            if (noteSaveData is CustomBeatmapSaveData.NoteData customNoteSaveData)
            {
                dynamic customData = customNoteSaveData.customData;
                if (customData != null) return customData;
            }
            return Tree();
        }

        private static dynamic GetObstacleCustomData(BeatmapSaveData.ObstacleData obstacleSaveData)
        {
            if (obstacleSaveData is CustomBeatmapSaveData.ObstacleData customObstacleSaveData)
            {
                dynamic customData = customObstacleSaveData.customData;
                if (customData != null) return customData;
            }
            return Tree();
        }

        private static dynamic GetEventCustomData(BeatmapSaveData.EventData eventSaveData)
        {
            if (eventSaveData is CustomBeatmapSaveData.EventData customEventSaveData)
            {
                dynamic customData = customEventSaveData.customData;
                if (customData != null) return customData;
            }
            return Tree();
        }
    }
}