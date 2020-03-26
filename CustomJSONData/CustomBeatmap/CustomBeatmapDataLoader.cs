using IPA.Utilities;
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
                                                                          List<CustomBeatmapSaveData.EventData> eventsSaveData, float startBPM, float shuffle, float shufflePeriod, 
                                                                          List<CustomBeatmapSaveData.CustomEventData> customEventsSaveData, dynamic customData, dynamic customLevelData,
                                                                          BeatmapDataLoader beatmapDataLoader)
        {
            try
            {
                // IPA's reflectionutil sucks
                object _notesInTimeRowProcessor = beatmapDataLoader.GetType().GetField("_notesInTimeRowProcessor", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(beatmapDataLoader);
                MethodInfo ProcessBasicNotesInTimeRow = _notesInTimeRowProcessor.GetType().GetMethod("ProcessBasicNotesInTimeRow", BindingFlags.Instance | BindingFlags.Public);
                MethodInfo ProcessNotesInTimeRow = _notesInTimeRowProcessor.GetType().GetMethod("ProcessNotesInTimeRow", BindingFlags.Instance | BindingFlags.Public);

                // remainder of this method copied from SongLoaderPlugin@12de0cf, with changes:
                // BeatmapData, NoteData, EventData, and ObstacleData replaced with their Custom counterparts
                // customData fields propagated to new objects
                List<BeatmapObjectData>[] array = new List<BeatmapObjectData>[4];
                List<BeatmapEventData> list = new List<BeatmapEventData>(eventsSaveData.Count);
                List<BPMChangeData> list2 = new List<BPMChangeData>();
                list2.Add(new BPMChangeData(0f, 0f, startBPM));
                BPMChangeData bpmchangeData = list2[0];
                foreach (CustomBeatmapSaveData.EventData eventData in eventsSaveData)
                {
                    if (eventData.type.IsBPMChangeEvent())
                    {
                        float time = eventData.time;
                        int value = eventData.value;
                        float bpmChangeStartTime = bpmchangeData.bpmChangeStartTime + beatmapDataLoader.GetRealTimeFromBPMTime(time - bpmchangeData.bpmChangeStartBPMTime, (float)value, shuffle, shufflePeriod);
                        list2.Add(new BPMChangeData(bpmChangeStartTime, time, (float)value));
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    array[i] = new List<BeatmapObjectData>(3000);
                }
                int num = 0;
                float num2 = -1f;
                List<CustomNoteData> list3 = new List<CustomNoteData>(4);
                List<CustomNoteData> list4 = new List<CustomNoteData>(4);
                int num3 = 0;
                foreach (CustomBeatmapSaveData.NoteData noteData in notesSaveData)
                {
                    float time2 = noteData.time;
                    while (num3 < list2.Count - 1 && list2[num3 + 1].bpmChangeStartBPMTime < time2)
                    {
                        num3++;
                    }
                    BPMChangeData bpmchangeData2 = list2[num3];
                    float num4 = bpmchangeData2.bpmChangeStartTime + beatmapDataLoader.GetRealTimeFromBPMTime(time2 - bpmchangeData2.bpmChangeStartBPMTime, bpmchangeData2.bpm, shuffle, shufflePeriod);
                    int lineIndex = noteData.lineIndex;
                    NoteLineLayer lineLayer = noteData.lineLayer;
                    NoteLineLayer startNoteLineLayer = NoteLineLayer.Base;
                    NoteType type = noteData.type;
                    NoteCutDirection cutDirection = noteData.cutDirection;
                    if (list3.Count > 0 && list3[0].time < num4 - 0.001f && type.IsBasicNote())
                    {
                        ProcessBasicNotesInTimeRow.Invoke(_notesInTimeRowProcessor, new object[] { list3.Cast<NoteData>().ToList(), num4 });
                        num2 = list3[0].time;
                        list3.Clear();
                    }
                    if (list4.Count > 0 && list4[0].time < num4 - 0.001f)
                    {
                        ProcessNotesInTimeRow.Invoke(_notesInTimeRowProcessor, new object[] { list4.Cast<NoteData>().ToList() });
                        list4.Clear();
                    }
                    CustomNoteData noteData2 = new CustomNoteData(num++, num4, lineIndex, lineLayer, startNoteLineLayer, type, cutDirection, float.MaxValue, num4 - num2, noteData._customData ?? Tree());
                    int lineNum = lineIndex > 3 ? 3 : lineIndex < 0 ? 0 : lineIndex;
                    array[lineNum].Add(noteData2);
                    CustomNoteData item = noteData2;
                    if (noteData2.noteType.IsBasicNote())
                    {
                        list3.Add(item);
                    }
                    list4.Add(item);
                }
                ProcessBasicNotesInTimeRow.Invoke(_notesInTimeRowProcessor, new object[] { list3.Cast<NoteData>().ToList(), float.MaxValue });
                ProcessNotesInTimeRow.Invoke(_notesInTimeRowProcessor, new object[] { list4.Cast<NoteData>().ToList() });
                num3 = 0;
                foreach (CustomBeatmapSaveData.ObstacleData obstacleData in obstaclesSaveData)
                {
                    float time3 = obstacleData.time;
                    while (num3 < list2.Count - 1 && list2[num3 + 1].bpmChangeStartBPMTime < time3)
                    {
                        num3++;
                    }
                    BPMChangeData bpmchangeData3 = list2[num3];
                    float time4 = bpmchangeData3.bpmChangeStartTime + beatmapDataLoader.GetRealTimeFromBPMTime(time3 - bpmchangeData3.bpmChangeStartBPMTime, bpmchangeData3.bpm, shuffle, shufflePeriod);
                    int lineIndex2 = obstacleData.lineIndex;
                    ObstacleType type2 = obstacleData.type;
                    float realTimeFromBPMTime = beatmapDataLoader.GetRealTimeFromBPMTime(obstacleData.duration, startBPM, shuffle, shufflePeriod);
                    int width = obstacleData.width;
                    CustomObstacleData item2 = new CustomObstacleData(num++, time4, lineIndex2, type2, realTimeFromBPMTime, width, obstacleData._customData ?? Tree());
                    int lineNum = lineIndex2 > 3 ? 3 : lineIndex2 < 0 ? 0 : lineIndex2;
                    array[lineNum].Add(item2);
                }
                foreach (CustomBeatmapSaveData.EventData eventData2 in eventsSaveData)
                {
                    float time5 = eventData2.time;
                    while (num3 < list2.Count - 1 && list2[num3 + 1].bpmChangeStartBPMTime < time5)
                    {
                        num3++;
                    }
                    BPMChangeData bpmchangeData4 = list2[num3];
                    float time6 = bpmchangeData4.bpmChangeStartTime + beatmapDataLoader.GetRealTimeFromBPMTime(time5 - bpmchangeData4.bpmChangeStartBPMTime, bpmchangeData4.bpm, shuffle, shufflePeriod);
                    BeatmapEventType type3 = eventData2.type;
                    int value2 = eventData2.value;
                    CustomBeatmapEventData item3 = new CustomBeatmapEventData(time6, type3, value2, eventData2.customData ?? Tree());
                    list.Add(item3);
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
                    float time7 = customEventData.time;
                    while (num3 < list2.Count - 1 && list2[num3 + 1].bpmChangeStartBPMTime < time7)
                    {
                        num3++;
                    }
                    BPMChangeData bpmchangeData5 = list2[num3];
                    float time8 = bpmchangeData5.bpmChangeStartTime + beatmapDataLoader.GetRealTimeFromBPMTime(time7 - bpmchangeData5.bpmChangeStartBPMTime, bpmchangeData5.bpm, shuffle, shufflePeriod);
                    customEvents[customEventData.type].Add(new CustomEventData(time8, customEventData.type, customEventData.data ?? Tree()));
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

        private struct BPMChangeData
        {
            // Token: 0x0600044B RID: 1099 RVA: 0x00010D98 File Offset: 0x0000EF98
            public BPMChangeData(float bpmChangeStartTime, float bpmChangeStartBPMTime, float bpm)
            {
                this.bpmChangeStartTime = bpmChangeStartTime;
                this.bpmChangeStartBPMTime = bpmChangeStartBPMTime;
                this.bpm = bpm;
            }

            // Token: 0x04000493 RID: 1171
            public readonly float bpmChangeStartTime;

            // Token: 0x04000494 RID: 1172
            public readonly float bpmChangeStartBPMTime;

            // Token: 0x04000495 RID: 1173
            public readonly float bpm;
        }
    }
}
