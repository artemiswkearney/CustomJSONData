namespace CustomJSONData.HarmonyPatches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using CustomJSONData;
    using CustomJSONData.CustomBeatmap;
    using HarmonyLib;

    [HarmonyPatch(typeof(BeatmapDataLoader))]
    [HarmonyPatch("GetBeatmapDataFromBeatmapSaveData")]
    internal static class BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData
    {
        private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance;

        private static readonly MethodInfo _createBombNoteData = SymbolExtensions.GetMethodInfo(() => NoteData.CreateBombNoteData(0, 0, 0));
        private static readonly MethodInfo _createBombCustomNoteData = SymbolExtensions.GetMethodInfo(() => CustomNoteData.CreateBombNoteData(0, 0, 0, null));
        private static readonly MethodInfo _createBasicNoteData = SymbolExtensions.GetMethodInfo(() => NoteData.CreateBasicNoteData(0, 0, 0, 0, 0));
        private static readonly MethodInfo _createBasicCustomNoteData = SymbolExtensions.GetMethodInfo(() => CustomNoteData.CreateBasicNoteData(0, 0, 0, 0, 0, null));
        private static readonly MethodInfo _getNoteCustomData = SymbolExtensions.GetMethodInfo(() => GetNoteCustomData(null));

        private static readonly ConstructorInfo _waypointDataCtor = typeof(WaypointData).GetConstructors().First();
        private static readonly ConstructorInfo _customWaypointDataCtor = typeof(CustomWaypointData).GetConstructors(FLAGS).First();
        private static readonly MethodInfo _getWaypointCustomData = SymbolExtensions.GetMethodInfo(() => GetWaypointCustomData(null));

        private static readonly ConstructorInfo _obstacleDataCtor = typeof(ObstacleData).GetConstructors().First();
        private static readonly ConstructorInfo _customObstacleDataCtor = typeof(CustomObstacleData).GetConstructors(FLAGS).First();
        private static readonly MethodInfo _getObstacleCustomData = SymbolExtensions.GetMethodInfo(() => GetObstacleCustomData(null));

        private static readonly ConstructorInfo _eventDataCtor = typeof(BeatmapEventData).GetConstructors().First();
        private static readonly ConstructorInfo _customEventDataCtor = typeof(CustomBeatmapEventData).GetConstructors(FLAGS).First();
        private static readonly MethodInfo _getEventCustomData = SymbolExtensions.GetMethodInfo(() => GetEventCustomData(null));

        private static readonly MethodInfo _createCustomBeatmapData = SymbolExtensions.GetMethodInfo(() => CreateCustomBeatmapData(null, null, 0, 0));

        internal static CustomBeatmapSaveData CustomBeatmapSaveData { get; set; }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            FieldInfo bpmChangesDataField = null;

            bool foundBombNoteData = false;
            bool foundBasicNoteData = false;
            bool foundWaypointData = false;
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
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 6));
                }

                if (!foundBasicNoteData &&
                    instructionList[i].opcode == OpCodes.Call &&
                    instructionList[i].operand == _createBasicNoteData)
                {
                    foundBasicNoteData = true;
                    instructionList[i].operand = _createBasicCustomNoteData;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getNoteCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 6));
                }

                if (!foundWaypointData &&
                    instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _waypointDataCtor)
                {
                    foundWaypointData = true;
                    instructionList[i].operand = _customWaypointDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getWaypointCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 7));
                }

                if (!foundObstacleData &&
                    instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _obstacleDataCtor)
                {
                    foundObstacleData = true;
                    instructionList[i].operand = _customObstacleDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getObstacleCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 8));
                }

                if (instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _eventDataCtor)
                {
                    instructionList[i].operand = _customEventDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getEventCustomData));
                    if (!foundEventData)
                    {
                        instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 21));
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

                // we look for this specifically because it happens after the bpm changes have been loaded
                if (!foundBeatmapData &&
                    bpmChangesDataField != null &&
                    instructionList[i].opcode == OpCodes.Stfld &&
                    ((FieldInfo)instructionList[i].operand).Name == "bpmChangesDataIdx")
                {
                    foundBeatmapData = true;
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                    instructionList.Insert(i + 2, new CodeInstruction(OpCodes.Ldloc_0));
                    instructionList.Insert(i + 3, new CodeInstruction(OpCodes.Ldfld, bpmChangesDataField));
                    instructionList.Insert(i + 4, new CodeInstruction(OpCodes.Ldarg_S, 7));
                    instructionList.Insert(i + 5, new CodeInstruction(OpCodes.Ldarg_S, 8));
                    instructionList.Insert(i + 6, new CodeInstruction(OpCodes.Call, _createCustomBeatmapData));
                    instructionList.Insert(i + 7, new CodeInstruction(OpCodes.Stloc_1));
                }
            }
#pragma warning restore CS0252
            if (!foundBombNoteData || !foundBasicNoteData || /*!foundLongNoteData ||*/ !foundWaypointData || !foundObstacleData || !foundEventData || !foundBeatmapData)
            {
                Logger.Log("Failed to patch GetBeatmapDataFromBeatmapSaveData in BeatmapDataLoader!", IPA.Logging.Logger.Level.Error);
            }

            return instructionList.AsEnumerable();
        }

        private static Dictionary<string, object> GetNoteCustomData(BeatmapSaveData.NoteData noteSaveData)
        {
            if (noteSaveData is CustomBeatmapSaveData.NoteData customNoteSaveData)
            {
                return new Dictionary<string, object>(customNoteSaveData.customData);
            }

            return new Dictionary<string, object>();
        }

        private static Dictionary<string, object> GetWaypointCustomData(BeatmapSaveData.WaypointData waypointData)
        {
            if (waypointData is CustomBeatmapSaveData.WaypointData customWaypointData)
            {
                return new Dictionary<string, object>(customWaypointData.customData);
            }

            return new Dictionary<string, object>();
        }

        private static Dictionary<string, object> GetObstacleCustomData(BeatmapSaveData.ObstacleData obstacleSaveData)
        {
            if (obstacleSaveData is CustomBeatmapSaveData.ObstacleData customObstacleSaveData)
            {
                return new Dictionary<string, object>(customObstacleSaveData.customData);
            }

            return new Dictionary<string, object>();
        }

        private static Dictionary<string, object> GetEventCustomData(BeatmapSaveData.EventData eventSaveData)
        {
            if (eventSaveData is CustomBeatmapSaveData.EventData customEventSaveData)
            {
                return new Dictionary<string, object>(customEventSaveData.customData);
            }

            return new Dictionary<string, object>();
        }

        private static CustomBeatmapData CreateCustomBeatmapData(BeatmapDataLoader beatmapDataLoader, List<BeatmapDataLoader.BpmChangeData> bpmChanges, float shuffle, float shufflePeriod)
        {
            List<CustomBeatmapSaveData.CustomEventData> customEventsSaveData = CustomBeatmapSaveData.customEvents;
            customEventsSaveData = customEventsSaveData.OrderBy(x => x.time).ToList();

            CustomBeatmapData customBeatmapData = new CustomBeatmapData(4);

            foreach (CustomBeatmapSaveData.CustomEventData customEventData in customEventsSaveData)
            {
                // Same math from BeatmapDataLoader
                int bpmChangesDataIdx = 0;
                float time = customEventData.time;
                while (bpmChangesDataIdx < bpmChanges.Count - 1 && bpmChanges[bpmChangesDataIdx + 1].bpmChangeStartBpmTime < time)
                {
                    bpmChangesDataIdx++;
                }

                BeatmapDataLoader.BpmChangeData bpmchangeData = bpmChanges[bpmChangesDataIdx];
                float realTime = bpmchangeData.bpmChangeStartTime + beatmapDataLoader.GetRealTimeFromBPMTime(time - bpmchangeData.bpmChangeStartBpmTime, bpmchangeData.bpm, shuffle, shufflePeriod);

                customBeatmapData.AddCustomEventData(new CustomEventData(realTime, customEventData.type, customEventData.data));
            }

            customBeatmapData.SetCustomData(CustomBeatmapSaveData.customData);

            return customBeatmapData;
        }
    }
}
