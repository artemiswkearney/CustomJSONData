using CustomJSONData.CustomBeatmap;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static CustomJSONData.Trees;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataLoader))]
    [HarmonyPatch("GetBeatmapDataFromBeatmapSaveData")]
    internal class BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData
    {
        internal static List<CustomBeatmapSaveData.CustomEventData> customEventsSaveData;

        private static readonly ConstructorInfo NoteDataCtor = typeof(NoteData).GetConstructors().First();
        private static readonly ConstructorInfo CustomNoteDataCtor = typeof(CustomNoteData).GetConstructors().First();
        private static readonly MethodInfo NoteCustomData = SymbolExtensions.GetMethodInfo(() => GetNoteCustomData(null));

        private static readonly ConstructorInfo ObstacleDataCtor = typeof(ObstacleData).GetConstructors().First();
        private static readonly ConstructorInfo CustomObstacleDataCtor = typeof(CustomObstacleData).GetConstructors().First();
        private static readonly MethodInfo ObstacleCustomData = SymbolExtensions.GetMethodInfo(() => GetObstacleCustomData(null));

        private static readonly ConstructorInfo EventDataCtor = typeof(BeatmapEventData).GetConstructors().First();
        private static readonly ConstructorInfo CustomEventDataCtor = typeof(CustomBeatmapEventData).GetConstructors().First();
        private static readonly MethodInfo EventCustomData = SymbolExtensions.GetMethodInfo(() => GetEventCustomData(null));

        private static readonly ConstructorInfo BeatmapDataCtor = typeof(BeatmapData).GetConstructors().First();
        private static readonly MethodInfo InjectCustom = SymbolExtensions.GetMethodInfo(() => InjectCustomData(null, null, null, null, 0, 0));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            bool foundNoteData = false;
            bool foundObstacleData = false;
            bool foundEventData = false;
            bool foundBeatmapData = false;
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
                if (!foundBeatmapData &&
                    instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == BeatmapDataCtor)
                {
                    foundBeatmapData = true;
                    instructionList[i] = new CodeInstruction(OpCodes.Call, InjectCustom);
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldarg_0));
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Ldloc_2));
                    instructionList.Insert(i + 2, new CodeInstruction(OpCodes.Ldarg_S, 5));
                    instructionList.Insert(i + 3, new CodeInstruction(OpCodes.Ldarg_S, 6));
                }
            }
#pragma warning restore CS0252
            if (!foundNoteData || !foundObstacleData || !foundEventData || !foundBeatmapData) Logger.Log("Failed to patch GetBeatmapDataFromBeatmapSaveData in BeatmapDataLoader!", IPA.Logging.Logger.Level.Error);
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

        private static BeatmapData InjectCustomData(BeatmapLineData[] beatmapLineData, BeatmapEventData[] beatmapEventData,
            BeatmapDataLoader beatmapDataLoader, dynamic RawBPMChanges, float shuffle, float shufflePeriod)
        {
            List<CustomEventData> customEvents = new List<CustomEventData>(customEventsSaveData.Count);
            foreach (CustomBeatmapSaveData.CustomEventData customEventData in customEventsSaveData)
            {
                int num = 0;
                float time = customEventData.time;

                // BeatmapDataLoader's BPMChangeData is private so we get to do a crap top of reflection to convert it to our BPMChangeData
                Type genericList = typeof(List<>);
                Type BPMChangeData = Type.GetType("BeatmapDataLoader+BPMChangeData,Main");
                Type constructedType = genericList.MakeGenericType(new Type[] { BPMChangeData });
                List<BPMChangeData> BPMChanges = new List<BPMChangeData>();
                foreach (object i in RawBPMChanges as IEnumerable)
                {
                    float bpmChangeStartTime = (float)i.GetType().GetField("bpmChangeStartTime").GetValue(i);
                    float bpmChangeStartBPMTime = (float)i.GetType().GetField("bpmChangeStartBPMTime").GetValue(i);
                    float bpm = (float)i.GetType().GetField("bpm").GetValue(i);

                    BPMChanges.Add(new BPMChangeData(bpmChangeStartTime, bpmChangeStartBPMTime, bpm));
                }

                while (num < BPMChanges.Count - 1 && BPMChanges[num + 1].bpmChangeStartBPMTime < time)
                {
                    num++;
                }
                BPMChangeData bpmchangeData = BPMChanges[num];
                float realTime = bpmchangeData.bpmChangeStartTime + beatmapDataLoader.GetRealTimeFromBPMTime(time - bpmchangeData.bpmChangeStartBPMTime, bpmchangeData.bpm, shuffle, shufflePeriod);

                customEvents.Add(new CustomEventData(realTime, customEventData.type, customEventData.data ?? Tree()));
            }
            return new CustomBeatmapData(beatmapLineData, beatmapEventData, customEvents.ToArray(), Tree(), Tree());
        }

        private struct BPMChangeData
        {
            public BPMChangeData(float bpmChangeStartTime, float bpmChangeStartBPMTime, float bpm)
            {
                this.bpmChangeStartTime = bpmChangeStartTime;
                this.bpmChangeStartBPMTime = bpmChangeStartBPMTime;
                this.bpm = bpm;
            }

            public readonly float bpmChangeStartTime;

            public readonly float bpmChangeStartBPMTime;

            public readonly float bpm;
        }
    }

    [HarmonyPatch(typeof(BeatmapDataLoader))]
    [HarmonyPatch("GetBeatmapDataFromJson")]
    internal static class BeatmapDataLoaderGetBeatmapDataFromJson
    {
        private static readonly MethodInfo GetBeatmapData = typeof(BeatmapDataLoader).GetMethod("GetBeatmapDataFromBeatmapSaveData");
        private static readonly MethodInfo StoreSaveData = SymbolExtensions.GetMethodInfo(() => StoreCustomEventsSaveData(null));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            bool foundGetBeatmapData = false;
#pragma warning disable CS0252
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (!foundGetBeatmapData &&
                    instructionList[i].opcode == OpCodes.Call &&
                    instructionList[i].operand == GetBeatmapData)
                {
                    foundGetBeatmapData = true;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_3));
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Call, StoreSaveData));
                }
            }
#pragma warning restore CS0252
            if (!foundGetBeatmapData) Logger.Log("Failed to patch GetBeatmapDataFromJson in BeatmapDataLoader!", IPA.Logging.Logger.Level.Error);
            return instructionList.AsEnumerable();
        }

        private static void StoreCustomEventsSaveData(BeatmapSaveData beatmapSaveData)
        {
            if (beatmapSaveData is CustomBeatmapSaveData customBeatmapSaveData)
                BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData.customEventsSaveData = customBeatmapSaveData.customEvents;
        }
    }
}