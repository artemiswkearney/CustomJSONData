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
        internal static CustomBeatmapSaveData customBeatmapSaveData;

        private static readonly ConstructorInfo _noteDataCtor = typeof(NoteData).GetConstructors().First();
        private static readonly ConstructorInfo _customNoteDataCtor = typeof(CustomNoteData).GetConstructors().First();
        private static readonly MethodInfo _getNoteCustomData = SymbolExtensions.GetMethodInfo(() => GetNoteCustomData(null));

        private static readonly ConstructorInfo _obstacleDataCtor = typeof(ObstacleData).GetConstructors().First();
        private static readonly ConstructorInfo _customObstacleDataCtor = typeof(CustomObstacleData).GetConstructors().First();
        private static readonly MethodInfo _getObstacleCustomData = SymbolExtensions.GetMethodInfo(() => GetObstacleCustomData(null));

        private static readonly ConstructorInfo _eventDataCtor = typeof(BeatmapEventData).GetConstructors().First();
        private static readonly ConstructorInfo _customEventDataCtor = typeof(CustomBeatmapEventData).GetConstructors().First();
        private static readonly MethodInfo _getEventCustomData = SymbolExtensions.GetMethodInfo(() => GetEventCustomData(null));

        private static readonly ConstructorInfo _beatmapDataCtor = typeof(BeatmapData).GetConstructors().First();
        private static readonly MethodInfo _injectCustomData = SymbolExtensions.GetMethodInfo(() => InjectCustomData(null, null, null, null, 0, 0));

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
                    instructionList[i].operand == _noteDataCtor)
                {
                    foundNoteData = true;
                    instructionList[i].operand = _customNoteDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getNoteCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 18));
                }
                if (instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _obstacleDataCtor)
                {
                    foundObstacleData = true;
                    instructionList[i].operand = _customObstacleDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getObstacleCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 29));
                }
                if (instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _eventDataCtor)
                {
                    instructionList[i].operand = _customEventDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getEventCustomData));
                    if (!foundEventData) instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 38));
                    else instructionList.Insert(i, new CodeInstruction(OpCodes.Ldnull));

                    foundEventData = true;
                }
                if (!foundBeatmapData &&
                    instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _beatmapDataCtor)
                {
                    foundBeatmapData = true;
                    instructionList[i] = new CodeInstruction(OpCodes.Call, _injectCustomData);
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
            List<CustomBeatmapSaveData.CustomEventData> customEventsSaveData = customBeatmapSaveData.customEvents;
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
                    float bpmChangeStartTime = (float)BPMChangeData.GetField("bpmChangeStartTime").GetValue(i);
                    float bpmChangeStartBPMTime = (float)BPMChangeData.GetField("bpmChangeStartBPMTime").GetValue(i);
                    float bpm = (float)BPMChangeData.GetField("bpm").GetValue(i);

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
            customEvents.Sort((CustomEventData x, CustomEventData y) =>
            {
                if (x.time == y.time)
                {
                    return 0;
                }
                if (x.time <= y.time)
                {
                    return -1;
                }
                return 1;
            });
            return new CustomBeatmapData(beatmapLineData, beatmapEventData, customEvents.ToArray(), customBeatmapSaveData.customData ?? Tree(), Tree(), Tree());
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
        private static readonly MethodInfo _getBeatmapData = typeof(BeatmapDataLoader).GetMethod("GetBeatmapDataFromBeatmapSaveData");
        private static readonly MethodInfo _storeCustomEventsSaveData = SymbolExtensions.GetMethodInfo(() => StoreCustomEventsSaveData(null));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            bool foundGetBeatmapData = false;
#pragma warning disable CS0252
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (!foundGetBeatmapData &&
                    instructionList[i].opcode == OpCodes.Call &&
                    instructionList[i].operand == _getBeatmapData)
                {
                    foundGetBeatmapData = true;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_3));
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Call, _storeCustomEventsSaveData));
                }
            }
#pragma warning restore CS0252
            if (!foundGetBeatmapData) Logger.Log("Failed to patch GetBeatmapDataFromJson in BeatmapDataLoader!", IPA.Logging.Logger.Level.Error);
            return instructionList.AsEnumerable();
        }

        private static void StoreCustomEventsSaveData(BeatmapSaveData beatmapSaveData)
        {
            if (beatmapSaveData is CustomBeatmapSaveData customBeatmapSaveData)
                BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData.customBeatmapSaveData = customBeatmapSaveData;
        }
    }
}