namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    internal class CustomLevelInfoSaveData : StandardLevelInfoSaveData
    {
        internal CustomLevelInfoSaveData(
            string version,
            string songName,
            string songSubName,
            string songAuthorName,
            string levelAuthorName,
            float beatsPerMinute,
            float songTimeOffset,
            float shuffle,
            float shufflePeriod,
            float previewStartTime,
            float previewDuration,
            string songFilename,
            string coverImageFilename,
            string environmentName,
            string allDirectionsEnvironmentName,
            DifficultyBeatmapSet[] difficultyBeatmapSets,
            Dictionary<string, object> customData,
            Dictionary<string, Dictionary<string, object>> beatmapCustomDatasByFilename)
            : base(
                  songName,
                  songSubName,
                  songAuthorName,
                  levelAuthorName,
                  beatsPerMinute,
                  songTimeOffset,
                  shuffle,
                  shufflePeriod,
                  previewStartTime,
                  previewDuration,
                  songFilename,
                  coverImageFilename,
                  environmentName,
                  allDirectionsEnvironmentName,
                  difficultyBeatmapSets)
        {
            _version = version;
            this.customData = customData;
            this.beatmapCustomDatasByFilename = beatmapCustomDatasByFilename;
        }

        internal Dictionary<string, object> customData { get; }

        internal Dictionary<string, Dictionary<string, object>> beatmapCustomDatasByFilename { get; }

        internal static CustomLevelInfoSaveData Deserialize(string path)
        {
            string version = string.Empty;
            string songName = string.Empty;
            string songSubName = string.Empty;
            string songAuthorName = string.Empty;
            string levelAuthorName = string.Empty;
            float beatsPerMinute = default;
            float songTimeOffset = default;
            float shuffle = default;
            float shufflePeriod = default;
            float previewStartTime = default;
            float previewDuration = default;
            string songFilename = string.Empty;
            string coverImageFilename = string.Empty;
            string environmentName = string.Empty;
            string allDirectionsEnvrionmentName = string.Empty;
            List<DifficultyBeatmapSet> difficultyBeatmapSets = new List<DifficultyBeatmapSet>();
            Dictionary<string, object> customData = new Dictionary<string, object>();
            Dictionary<string, Dictionary<string, object>> beatmapCustomDatasByFilename = new Dictionary<string, Dictionary<string, object>>();

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
                                version = reader.ReadAsString();
                                break;

                            case "_songName":
                                songName = reader.ReadAsString();
                                break;

                            case "_songSubName":
                                songSubName = reader.ReadAsString();
                                break;

                            case "songAuthorName":
                                songAuthorName = reader.ReadAsString();
                                break;

                            case "_levelAuthorName":
                                levelAuthorName = reader.ReadAsString();
                                break;

                            case "_beatsPerMinute":
                                beatsPerMinute = (float)reader.ReadAsDouble();
                                break;

                            case "_songTimeOffset":
                                songTimeOffset = (float)reader.ReadAsDouble();
                                break;

                            case "_shuffle":
                                shuffle = (float)reader.ReadAsDouble();
                                break;

                            case "_shufflePeriod":
                                shufflePeriod = (float)reader.ReadAsDouble();
                                break;

                            case "_previewStartTime":
                                previewStartTime = (float)reader.ReadAsDouble();
                                break;

                            case "_previewDuration":
                                previewDuration = (float)reader.ReadAsDouble();
                                break;

                            case "_songFilename":
                                songFilename = reader.ReadAsString();
                                break;

                            case "_coverImageFilename":
                                coverImageFilename = reader.ReadAsString();
                                break;

                            case "_environmentName":
                                environmentName = reader.ReadAsString();
                                break;

                            case "_allDirectionsEnvironmentName":
                                allDirectionsEnvrionmentName = reader.ReadAsString();
                                break;

                            case "_difficultyBeatmapSets":
                                reader.ReadObjectArray(() =>
                                {
                                    string beatmapCharacteristicName = string.Empty;
                                    List<DifficultyBeatmap> difficultyBeatmaps = new List<DifficultyBeatmap>();
                                    reader.ReadObject(objectName =>
                                    {
                                        switch (objectName)
                                        {
                                            case "_beatmapCharacteristicName":
                                                beatmapCharacteristicName = reader.ReadAsString();
                                                break;

                                            case "_difficultyBeatmaps":
                                                reader.ReadObjectArray(() =>
                                                {
                                                    string difficulty = string.Empty;
                                                    int difficultyRank = default;
                                                    string beatmapFilename = string.Empty;
                                                    float noteJumpMovementSpeed = default;
                                                    float noteJumpStartBeatOffset = default;
                                                    Dictionary<string, object> data = new Dictionary<string, object>();
                                                    reader.ReadObject(difficultyBeatmapObjectName =>
                                                    {
                                                        switch (difficultyBeatmapObjectName)
                                                        {
                                                            case "_difficulty":
                                                                difficulty = reader.ReadAsString();
                                                                break;

                                                            case "_difficultyRank":
                                                                difficultyRank = (int)reader.ReadAsInt32();
                                                                break;

                                                            case "_beatmapFilename":
                                                                beatmapFilename = reader.ReadAsString();
                                                                break;

                                                            case "_noteJumpMovementSpeed":
                                                                noteJumpMovementSpeed = (float)reader.ReadAsDouble();
                                                                break;

                                                            case "_noteJumpStartBeatOffset":
                                                                noteJumpStartBeatOffset = (float)reader.ReadAsDouble();
                                                                break;

                                                            case "_customData":
                                                                reader.ReadToDictionary(data);
                                                                break;

                                                            default:
                                                                reader.Skip();
                                                                break;
                                                        }
                                                    });

                                                    beatmapCustomDatasByFilename[beatmapFilename] = data;
                                                    difficultyBeatmaps.Add(new DifficultyBeatmap(difficulty, difficultyRank, beatmapFilename, noteJumpMovementSpeed, noteJumpStartBeatOffset));
                                                });

                                                break;

                                            default:
                                                reader.Skip();
                                                break;
                                        }
                                    });

                                    difficultyBeatmapSets.Add(new DifficultyBeatmapSet(beatmapCharacteristicName, difficultyBeatmaps.ToArray()));
                                });

                                break;

                            case "_customData":
                                reader.ReadToDictionary(customData);
                                break;
                        }
                    }
                }
            }

            return new CustomLevelInfoSaveData(
                version,
                songName,
                songSubName,
                songAuthorName,
                levelAuthorName,
                beatsPerMinute,
                songTimeOffset,
                shuffle,
                shufflePeriod,
                previewStartTime,
                previewDuration,
                songFilename,
                coverImageFilename,
                environmentName,
                allDirectionsEnvrionmentName,
                difficultyBeatmapSets.ToArray(),
                customData,
                beatmapCustomDatasByFilename);
        }
    }
}
