namespace CustomJSONData.HarmonyPatches
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using CustomJSONData.CustomBeatmap;
    using HarmonyLib;
    using static CustomJSONData.Trees;

    [HarmonyPatch(typeof(BeatmapDataLoader))]
    [HarmonyPatch("GetBeatmapDataFromBeatmapSaveData")]
    internal class BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData
    {
        internal static CustomBeatmapSaveData customBeatmapSaveData;

        private static readonly MethodInfo _createBombNoteData = SymbolExtensions.GetMethodInfo(() => NoteData.CreateBombNoteData(0, 0, 0));
        private static readonly MethodInfo _createBombCustomNoteData = SymbolExtensions.GetMethodInfo(() => CustomNoteData.CreateBombNoteData(0, 0, 0, null));
        private static readonly MethodInfo _createBasicNoteData = SymbolExtensions.GetMethodInfo(() => NoteData.CreateBasicNoteData(0, 0, 0, 0, 0));
        private static readonly MethodInfo _createBasicCustomNoteData = SymbolExtensions.GetMethodInfo(() => CustomNoteData.CreateBasicNoteData(0, 0, 0, 0, 0, null));
        private static readonly MethodInfo _createLongNoteData = SymbolExtensions.GetMethodInfo(() => NoteData.CreateLongNoteData(0, 0, 0, 0, 0, 0));
        private static readonly MethodInfo _createLongCustomNoteData = SymbolExtensions.GetMethodInfo(() => CustomNoteData.CreateLongNoteData(0, 0, 0, 0, 0, 0, null));
        private static readonly MethodInfo _getNoteCustomData = SymbolExtensions.GetMethodInfo(() => GetNoteCustomData(null));
        private static readonly MethodInfo _getLongNoteCustomData = SymbolExtensions.GetMethodInfo(() => GetLongNoteCustomData(null));

        private static readonly ConstructorInfo _obstacleDataCtor = typeof(ObstacleData).GetConstructors().First();
        private static readonly ConstructorInfo _customObstacleDataCtor = typeof(CustomObstacleData).GetConstructors().First();
        private static readonly MethodInfo _getObstacleCustomData = SymbolExtensions.GetMethodInfo(() => GetObstacleCustomData(null));

        private static readonly ConstructorInfo _eventDataCtor = typeof(BeatmapEventData).GetConstructors().First();
        private static readonly ConstructorInfo _customEventDataCtor = typeof(CustomBeatmapEventData).GetConstructors().First();
        private static readonly MethodInfo _getEventCustomData = SymbolExtensions.GetMethodInfo(() => GetEventCustomData(null));

        private static readonly MethodInfo _processRemainingData = typeof(BeatmapData).GetMethod("ProcessRemainingData");
        private static readonly MethodInfo _createCustomBeatmapData = SymbolExtensions.GetMethodInfo(() => CreateCustomBeatmapData(null, null, 0, 0));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            FieldInfo bpmChangesDataField = null;

            bool foundBombNoteData = false;
            bool foundBasicNoteData = false;
            bool foundLongNoteData = false;
            bool foundObstacleData = false;
            bool foundEventData = false;
            bool foundBeatmapData = false;
#pragma warning disable CS0252
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (!foundBombNoteData &&
                    instructionList[i].opcode == OpCodes.Call &&
                    instructionList[i].operand == _createBombNoteData)
                {
                    foundBombNoteData = true;
                    instructionList[i].operand = _createBombCustomNoteData;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getNoteCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 11));
                }
                if (!foundBasicNoteData &&
                    instructionList[i].opcode == OpCodes.Call &&
                    instructionList[i].operand == _createBasicNoteData)
                {
                    foundBasicNoteData = true;
                    instructionList[i].operand = _createBasicCustomNoteData;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getNoteCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 11));
                }
                if (!foundLongNoteData &&
                    instructionList[i].opcode == OpCodes.Call &&
                    instructionList[i].operand == _createLongNoteData)
                {
                    foundLongNoteData = true;
                    instructionList[i].operand = _createLongCustomNoteData;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getLongNoteCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 12));
                }
                
                if (!foundObstacleData &&
                    instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _obstacleDataCtor)
                {
                    foundObstacleData = true;
                    instructionList[i].operand = _customObstacleDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getObstacleCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 13));
                }
                if (instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _eventDataCtor)
                {
                    instructionList[i].operand = _customEventDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getEventCustomData));
                    if (!foundEventData)
                    {
                        instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 20));
                    }
                    else
                    {
                        instructionList.Insert(i, new CodeInstruction(OpCodes.Ldnull));
                    }

                    foundEventData = true;
                }

                if (bpmChangesDataField == null &&
                    instructionList[i].opcode == OpCodes.Stfld &&
                    ((FieldInfo)instructionList[i].operand).Name == "bpmChangesData")
                {
                    bpmChangesDataField = (FieldInfo)instructionList[i].operand;
                }
                if (!foundBeatmapData &&
                    bpmChangesDataField != null &&
                    instructionList[i].opcode == OpCodes.Stfld &&
                    ((FieldInfo)instructionList[i].operand).Name == "bpmChangesDataIdx") // we look for this specifically because it happens after the bpm changes have been loaded
                {
                    foundBeatmapData = true;
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                    instructionList.Insert(i + 2, new CodeInstruction(OpCodes.Ldloc_0));
                    instructionList.Insert(i + 3, new CodeInstruction(OpCodes.Ldfld, bpmChangesDataField));
                    instructionList.Insert(i + 4, new CodeInstruction(OpCodes.Ldarg_S, 6));
                    instructionList.Insert(i + 5, new CodeInstruction(OpCodes.Ldarg_S, 7));
                    instructionList.Insert(i + 6, new CodeInstruction(OpCodes.Call, _createCustomBeatmapData));
                    instructionList.Insert(i + 7, new CodeInstruction(OpCodes.Stloc_1));
                }
            }
#pragma warning restore CS0252
            if (!foundBombNoteData || !foundBasicNoteData || !foundLongNoteData || !foundObstacleData || !foundEventData || !foundBeatmapData)
            {
                Logger.Log("Failed to patch GetBeatmapDataFromBeatmapSaveData in BeatmapDataLoader!", IPA.Logging.Logger.Level.Error);
            }

            return instructionList.AsEnumerable();
        }

        private static dynamic GetNoteCustomData(BeatmapSaveData.NoteData noteSaveData)
        {
            if (noteSaveData is CustomBeatmapSaveData.NoteData customNoteSaveData)
            {
                dynamic customData = customNoteSaveData.customData;
                if (customData != null)
                {
                    return customData;
                }
            }

            return Tree();
        }

        private static dynamic GetLongNoteCustomData(BeatmapSaveData.LongNoteData noteSaveData)
        {
            if (noteSaveData is CustomBeatmapSaveData.LongNoteData customNoteSaveData)
            {
                dynamic customData = customNoteSaveData.customData;
                if (customData != null)
                {
                    return customData;
                }
            }

            return Tree();
        }

        private static dynamic GetObstacleCustomData(BeatmapSaveData.ObstacleData obstacleSaveData)
        {
            if (obstacleSaveData is CustomBeatmapSaveData.ObstacleData customObstacleSaveData)
            {
                dynamic customData = customObstacleSaveData.customData;
                if (customData != null)
                {
                    return customData;
                }
            }

            return Tree();
        }

        private static dynamic GetEventCustomData(BeatmapSaveData.EventData eventSaveData)
        {
            if (eventSaveData is CustomBeatmapSaveData.EventData customEventSaveData)
            {
                dynamic customData = customEventSaveData.customData;
                if (customData != null)
                {
                    return customData;
                }
            }

            return Tree();
        }

        private static CustomBeatmapData CreateCustomBeatmapData(BeatmapDataLoader beatmapDataLoader, dynamic RawBPMChanges, float shuffle, float shufflePeriod)
        {
            List<CustomBeatmapSaveData.CustomEventData> customEventsSaveData = customBeatmapSaveData.customEvents;
            customEventsSaveData = customEventsSaveData.OrderBy(x => x.time).ToList();

            CustomBeatmapData customBeatmapData = new CustomBeatmapData(4);

            foreach (CustomBeatmapSaveData.CustomEventData customEventData in customEventsSaveData)
            {
                // BeatmapDataLoader's BPMChangeData is private so we get to do a crap top of reflection to convert it to our BPMChangeData
                Type BPMChangeData = Type.GetType("BeatmapDataLoader+BpmChangeData,Main");
                List<BPMChangeData> BPMChanges = new List<BPMChangeData>();
                foreach (object i in RawBPMChanges as IEnumerable)
                {
                    float bpmChangeStartTime = (float)BPMChangeData.GetField("bpmChangeStartTime").GetValue(i);
                    float bpmChangeStartBPMTime = (float)BPMChangeData.GetField("bpmChangeStartBpmTime").GetValue(i);
                    float bpm = (float)BPMChangeData.GetField("bpm").GetValue(i);

                    BPMChanges.Add(new BPMChangeData(bpmChangeStartTime, bpmChangeStartBPMTime, bpm));
                }

                // Same math from BeatmapDataLoader
                int bpmChangesDataIdx = 0;
                float time = customEventData.time;
                while (bpmChangesDataIdx < BPMChanges.Count - 1 && BPMChanges[bpmChangesDataIdx + 1].bpmChangeStartBPMTime < time)
                {
                    bpmChangesDataIdx++;
                }
                BPMChangeData bpmchangeData = BPMChanges[bpmChangesDataIdx];
                float realTime = bpmchangeData.bpmChangeStartTime + beatmapDataLoader.GetRealTimeFromBPMTime(time - bpmchangeData.bpmChangeStartBPMTime, bpmchangeData.bpm, shuffle, shufflePeriod);

                customBeatmapData.AddCustomEventData(new CustomEventData(realTime, customEventData.type, customEventData.data ?? Tree()));
            }
            customBeatmapData.SetCustomData(customBeatmapSaveData.customData);

            return customBeatmapData;
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
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 4));
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Call, _storeCustomEventsSaveData));
                }
            }
#pragma warning restore CS0252
            if (!foundGetBeatmapData)
            {
                Logger.Log("Failed to patch GetBeatmapDataFromJson in BeatmapDataLoader!", IPA.Logging.Logger.Level.Error);
            }

            return instructionList.AsEnumerable();
        }

        private static void StoreCustomEventsSaveData(BeatmapSaveData beatmapSaveData)
        {
            if (beatmapSaveData is CustomBeatmapSaveData customBeatmapSaveData)
            {
                BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData.customBeatmapSaveData = customBeatmapSaveData;
            }
        }
    }
}
