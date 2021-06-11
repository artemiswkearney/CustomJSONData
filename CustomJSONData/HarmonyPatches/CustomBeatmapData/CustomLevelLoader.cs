namespace CustomJSONData.HarmonyPatches
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.IO;
    using CustomJSONData.CustomBeatmap;
    using CustomJSONData.CustomLevelInfo;
    using CustomJSONData.Utils;
    using Newtonsoft.Json;
    using HarmonyLib;
    using static CustomJSONData.Trees;

    [HarmonyPatch(typeof(CustomLevelLoader))]
    [HarmonyPatch("LoadBeatmapDataBeatmapData")]
    internal class CustomLevelLoaderLoadBeatmapDataBeatmapData
    {
        private static bool Prefix(ref BeatmapData __result, string customLevelPath, string difficultyFileName, StandardLevelInfoSaveData standardLevelInfoSaveData, BeatmapDataLoader ____beatmapDataLoader)
        {
            string path = Path.Combine(customLevelPath, difficultyFileName);
            if (File.Exists(path))
            {
                CustomBeatmapSaveData saveData = new CustomBeatmapSaveData();
                using (JsonTextReader reader = new JsonTextReader(new StreamReader(path)))
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            switch (reader.Value)
                            {
                                default:
                                    reader.Skip();
                                    break;

                                case "_version":
                                    saveData._version = reader.ReadAsString();
                                    break;

                                case "_events":
                                    reader.Read(); // StartArray
                                    reader.Read(); // StartObject (hopefully)
                                    List<CustomBeatmapSaveData.EventData> events = new List<CustomBeatmapSaveData.EventData>();

                                    while (reader.TokenType == JsonToken.StartObject)
                                    {
                                        CustomBeatmapSaveData.EventData eventData = new CustomBeatmapSaveData.EventData();
                                        reader.Read();
                                        while (reader.TokenType == JsonToken.PropertyName)
                                        {
                                            switch (reader.Value)
                                            {
                                                case "_time":
                                                    eventData._time = (float)reader.ReadAsDouble();
                                                    break;

                                                case "_type":
                                                    eventData._type = (BeatmapSaveData.BeatmapEventType)reader.ReadAsInt32();
                                                    break;

                                                case "_value":
                                                    eventData._value = (int)reader.ReadAsInt32();
                                                    break;
                                                
                                            case "_customData":
                                                eventData._customData = reader.ReadAsExpandoObject();
                                                break;
                                                default:
                                                    reader.Skip();
                                                    break;
                                            }

                                            reader.Read();
                                        }

                                        events.Add(eventData);
                                        reader.Read();
                                    }

                                    saveData._events = events;

                                    break;

                                case "_notes":
                                    reader.Read(); // StartArray
                                    reader.Read(); // StartObject (hopefully)
                                    List<CustomBeatmapSaveData.NoteData> notes = new List<CustomBeatmapSaveData.NoteData>();

                                    while (reader.TokenType == JsonToken.StartObject)
                                    {
                                        CustomBeatmapSaveData.NoteData noteData = new CustomBeatmapSaveData.NoteData();
                                        reader.Read();
                                        while (reader.TokenType == JsonToken.PropertyName)
                                        {
                                            switch (reader.Value)
                                            {
                                                case "_time":
                                                    noteData._time = (float)reader.ReadAsDouble();
                                                    break;

                                                case "_lineIndex":
                                                    noteData._lineIndex = (int)reader.ReadAsInt32();
                                                    break;

                                                case "_lineLayer":
                                                    noteData._lineLayer = (NoteLineLayer)reader.ReadAsInt32();
                                                    break;

                                                case "_type":
                                                    noteData._type = (BeatmapSaveData.NoteType)reader.ReadAsInt32();
                                                    break;

                                                case "_cutDirection":
                                                    noteData._cutDirection = (NoteCutDirection)reader.ReadAsInt32();
                                                    break;

                                                case "_customData":
                                                    noteData._customData = reader.ReadAsExpandoObject();
                                                    break;
                                                default:
                                                    reader.Skip();
                                                    break;
                                            }

                                            reader.Read();
                                        }

                                        notes.Add(noteData);
                                        reader.Read();
                                    }

                                    saveData._notes = notes;

                                    break;

                                case "_obstacles":
                                    reader.Read(); // StartArray
                                    reader.Read(); // StartObject (hopefully)
                                    List<CustomBeatmapSaveData.ObstacleData> obstacles = new List<CustomBeatmapSaveData.ObstacleData>();

                                    while (reader.TokenType == JsonToken.StartObject)
                                    {
                                        CustomBeatmapSaveData.ObstacleData obstacleData = new CustomBeatmapSaveData.ObstacleData();
                                        reader.Read();
                                        while (reader.TokenType == JsonToken.PropertyName)
                                        {
                                            switch (reader.Value)
                                            {
                                                case "_time":
                                                    obstacleData._time = (float)reader.ReadAsDouble();
                                                    break;

                                                case "_lineIndex":
                                                    obstacleData._lineIndex = (int)reader.ReadAsInt32();
                                                    break;

                                                case "_type":
                                                    obstacleData._type = (ObstacleType)reader.ReadAsInt32();
                                                    break;

                                                case "_duration":
                                                    obstacleData._duration = (float)reader.ReadAsDouble();
                                                    break;

                                                case "_width":
                                                    obstacleData._width = (int)reader.ReadAsInt32();
                                                    break;

                                                case "_customData":
                                                    obstacleData._customData = reader.ReadAsExpandoObject();
                                                    break;

                                                default:
                                                    reader.Skip();
                                                    break;

                                            }

                                            reader.Read();
                                        }

                                        obstacles.Add(obstacleData);
                                        reader.Read();
                                    }

                                    saveData._obstacles = obstacles;

                                    break;

                                case "_customData":
                                    saveData._customData = reader.ReadAsExpandoObjectWithCustomEvents(out List<CustomBeatmapSaveData.CustomEventData> customEventDatas);
                                    saveData.customEvents = customEventDatas;

                                    break;
                            }
                        }
                    }
                }

                {
                    List<BeatmapSaveData.NoteData> notes = saveData.notes;
                    List<BeatmapSaveData.ObstacleData> obstacles = saveData.obstacles;
                    List<BeatmapSaveData.EventData> events = saveData.events;
                    List<BeatmapSaveData.WaypointData> waypoints = saveData.waypoints;
                    BeatmapSaveData.SpecialEventKeywordFiltersData specialEventsKeywordFilters = saveData.specialEventsKeywordFilters;

                    BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData.customBeatmapSaveData = saveData;
                    __result = ____beatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(notes, waypoints, obstacles, events, specialEventsKeywordFilters, standardLevelInfoSaveData.beatsPerMinute, standardLevelInfoSaveData.shuffle, standardLevelInfoSaveData.shufflePeriod);
                }
            }

            return false;
        }

        private static void Postfix(BeatmapData __result, string difficultyFileName, StandardLevelInfoSaveData standardLevelInfoSaveData)
        {
            if (__result != null && __result is CustomBeatmapData customBeatmapData && standardLevelInfoSaveData is CustomLevelInfoSaveData lisd)
            {
                customBeatmapData.SetLevelCustomData(at(lisd.beatmapCustomDatasByFilename, difficultyFileName) ?? Tree(), lisd.customData ?? Tree());
            }
        }
    }
}
