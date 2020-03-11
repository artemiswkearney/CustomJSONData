using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static CustomJSONData.Trees;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBeatmapDataLoader
    {
        // Token: 0x0600125F RID: 4703 RVA: 0x000404A0 File Offset: 0x0003E6A0
        public static CustomBeatmapData GetBeatmapDataFromBeatmapSaveData(List<CustomBeatmapSaveData.NoteData> notesSaveData, List<CustomBeatmapSaveData.ObstacleData> obstaclesSaveData, 
                                                                          List<CustomBeatmapSaveData.EventData> eventsSaveData, float beatsPerMinute, float shuffle, float shufflePeriod, 
                                                                          List<CustomBeatmapSaveData.CustomEventData> customEventsSaveData, dynamic customData, dynamic customLevelData)
        {
            try
            {
                // why is IPA's reflectionutil missing InvokeMethod :(
                MethodInfo GetRealTimeFromBPMTime = typeof(BeatmapDataLoader).GetMethod("GetRealTimeFromBPMTime", BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo ProcessBasicNotesInTimeRow = typeof(BeatmapDataLoader).GetMethod("ProcessBasicNotesInTimeRow", BindingFlags.Static | BindingFlags.NonPublic);

                // remainder of this method copied from SongLoaderPlugin@12de0cf, with changes:
                // BeatmapData, NoteData, EventData, and ObstacleData replaced with their Custom counterparts
                // customData fields propagated to new objects
                List<BeatmapObjectData>[] array = new List<BeatmapObjectData>[4];
                List<BeatmapEventData> list = new List<BeatmapEventData>(eventsSaveData.Count);
                for (int i = 0; i < 4; i++)
                {
                    array[i] = new List<BeatmapObjectData>(3000);
                }
                int num = 0;
                CustomNoteData noteData = null;
                float num2 = -1f;
                List<CustomNoteData> list2 = new List<CustomNoteData>(4);
                float num3 = 0f;
                foreach (CustomBeatmapSaveData.NoteData noteData2 in notesSaveData)
                {
                    float realTimeFromBPMTime = Convert.ToSingle(GetRealTimeFromBPMTime.Invoke(null, new object[] { noteData2.time, beatsPerMinute, shuffle, shufflePeriod }));
                    if (num3 > realTimeFromBPMTime)
                    {
                        Debug.LogError("Notes are not ordered.");
                    }
                    num3 = realTimeFromBPMTime;
                    int lineIndex = noteData2.lineIndex;
                    NoteLineLayer lineLayer = noteData2.lineLayer;
                    NoteLineLayer startNoteLineLayer = NoteLineLayer.Base;
                    if (noteData != null && noteData.lineIndex == lineIndex && Mathf.Abs(noteData.time - realTimeFromBPMTime) < 0.0001f)
                    {
                        if (noteData.startNoteLineLayer == NoteLineLayer.Base)
                        {
                            startNoteLineLayer = NoteLineLayer.Upper;
                        }
                        else
                        {
                            startNoteLineLayer = NoteLineLayer.Top;
                        }
                    }
                    NoteType type = noteData2.type;
                    NoteCutDirection cutDirection = noteData2.cutDirection;
                    if (list2.Count > 0 && list2[0].time < realTimeFromBPMTime - 0.001f && type.IsBasicNote())
                    {
                        ProcessBasicNotesInTimeRow.Invoke(null, new object[] { list2.Cast<NoteData>().ToList(), realTimeFromBPMTime });
                        num2 = list2[0].time;
                        list2.Clear();
                    }
                    CustomNoteData noteData3 = new CustomNoteData(num++, realTimeFromBPMTime, lineIndex, lineLayer, startNoteLineLayer, type, cutDirection, float.MaxValue, realTimeFromBPMTime - num2, noteData2._customData ?? Tree());
                    int number = lineIndex;
                    if (number < 0)
                        number = 0;
                    if (number > 3)
                        number = 3;
                    array[number].Add(noteData3);
                    noteData = noteData3;
                    if (noteData3.noteType.IsBasicNote())
                    {
                        list2.Add(noteData);
                    }
                }
                ProcessBasicNotesInTimeRow.Invoke(null, new object[] { list2.Cast<NoteData>().ToList(), float.MaxValue });
                foreach (CustomBeatmapSaveData.ObstacleData obstacleData in obstaclesSaveData)
                {
                    float realTimeFromBPMTime2 = Convert.ToSingle(GetRealTimeFromBPMTime.Invoke(null, new object[] { obstacleData.time, beatsPerMinute, shuffle, shufflePeriod }));
                    int lineIndex2 = obstacleData.lineIndex;
                    ObstacleType type2 = obstacleData.type;
                    float realTimeFromBPMTime3 = Convert.ToSingle(GetRealTimeFromBPMTime.Invoke(null, new object[] { obstacleData.duration, beatsPerMinute, shuffle, shufflePeriod }));
                    int width = obstacleData.width;
                    CustomObstacleData item = new CustomObstacleData(num++, realTimeFromBPMTime2, lineIndex2, type2, realTimeFromBPMTime3, width, obstacleData._customData ?? Tree());
                    int number2 = lineIndex2;
                    if (number2 < 0)
                        number2 = 0;
                    if (number2 > 3)
                        number2 = 3;
                    array[number2].Add(item);
                }
                foreach (CustomBeatmapSaveData.EventData eventData in eventsSaveData)
                {
                    float realTimeFromBPMTime4 = Convert.ToSingle(GetRealTimeFromBPMTime.Invoke(null, new object[] { eventData.time, beatsPerMinute, shuffle, shufflePeriod }));
                    BeatmapEventType type3 = eventData.type;
                    int value = eventData.value;
                    CustomBeatmapEventData item2 = new CustomBeatmapEventData(realTimeFromBPMTime4, type3, value, eventData.customData ?? Tree());
                    list.Add(item2);
                }
                if (list.Count == 0)
                {
                    list.Add(new CustomBeatmapEventData(0f, BeatmapEventType.Event0, 1, Tree()));
                    list.Add(new CustomBeatmapEventData(0f, BeatmapEventType.Event4, 1, Tree()));
                }
                var customEvents = new Dictionary<string, List<CustomEventData>>(customEventsSaveData.Count);
                foreach (CustomBeatmapSaveData.CustomEventData customEventData in customEventsSaveData)
                {
                    if (!customEvents.ContainsKey(customEventData.type)) customEvents[customEventData.type] = new List<CustomEventData>();
                    customEvents[customEventData.type].Add(new CustomEventData(Convert.ToSingle(GetRealTimeFromBPMTime.Invoke(null, new object[] { customEventData.time, beatsPerMinute, shuffle, shufflePeriod })), customEventData.type, customEventData.data ?? Tree()));
                }
                foreach (var pair in customEvents)
                {
                    pair.Value.Sort((x, y) => x.time.CompareTo(y.time));
                }
                BeatmapLineData[] array2 = new BeatmapLineData[4];
                for (int j = 0; j < 4; j++)
                {
                    array[j].Sort(delegate (BeatmapObjectData x, BeatmapObjectData y)
                    {
                        if (x.time == y.time)
                        {
                            return 0;
                        }
                        return (x.time <= y.time) ? -1 : 1;
                    });
                    array2[j] = new BeatmapLineData();
                    array2[j].beatmapObjectsData = array[j].ToArray();
                }
                return new CustomBeatmapData(array2, list.ToArray(), customEvents, customData, customLevelData);
            } catch (Exception e)
            {
                Plugin.logger.Critical("Exception loading CustomBeatmap!");
                Plugin.logger.Critical(e);
                Debug.LogError("Exception loading CustomBeatmap!");
                Debug.LogError(e);
                throw e;
            }
        }
    }
}
