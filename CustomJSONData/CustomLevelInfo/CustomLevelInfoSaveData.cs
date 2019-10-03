using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomJSONData.Trees;
using Newtonsoft.Json;

using CustomDataConverter = Newtonsoft.Json.Converters.ExpandoObjectConverter;

namespace CustomJSONData.CustomLevelInfo
{
    public class CustomLevelInfoSaveData : StandardLevelInfoSaveData
    {
        protected dynamic _customData;
        public dynamic customData
        {
            get
            {
                return _customData;
            }
        }
        public Dictionary<string, dynamic> beatmapCustomDatasByFilename { get; protected set; }

        public CustomLevelInfoSaveData(string songName, string songSubName, string songAuthorName, string levelAuthorName, float beatsPerMinute, float songTimeOffset, 
                                       float shuffle, float shufflePeriod, float previewStartTime, float previewDuration, string songFilename, string coverImageFilename, 
                                       string environmentName, DifficultyBeatmapSet[] difficultyBeatmapSets, dynamic customData, Dictionary<string, dynamic> beatmapCustomDatasByFilename) 
                                : base(songName, songSubName, songAuthorName, levelAuthorName, beatsPerMinute, songTimeOffset, shuffle, shufflePeriod, previewStartTime, 
                                        previewDuration, songFilename, coverImageFilename, environmentName, difficultyBeatmapSets)
        {
            _customData = customData;
            this.beatmapCustomDatasByFilename = beatmapCustomDatasByFilename;
        }
        public static StandardLevelInfoSaveData DeserializeFromJSONString(string stringData, StandardLevelInfoSaveData standardSaveData)
        {
            // StandardLevelInfoSaveData standardSaveData = StandardLevelInfoSaveData.DeserializeFromJSONString(stringData);
            if (standardSaveData.version != "2.0.0") return standardSaveData;
            Dictionary<string, dynamic> beatmapsByFilename = new Dictionary<string, dynamic>();
            OopsAllCustomDatas customDatas = JsonConvert.DeserializeObject<OopsAllCustomDatas>(stringData, new CustomDataConverter());
            DifficultyBeatmapSet[] customBeatmapSets = new DifficultyBeatmapSet[standardSaveData.difficultyBeatmapSets.Length];
            for (int i = 0; i < standardSaveData.difficultyBeatmapSets.Length; i++)
            {
                var standardBeatmapSet = standardSaveData.difficultyBeatmapSets[i];
                DifficultyBeatmap[] customBeatmaps = new DifficultyBeatmap[standardBeatmapSet.difficultyBeatmaps.Length];
                for (int j = 0; j < standardBeatmapSet.difficultyBeatmaps.Length; j++)
                {
                    var standardBeatmap = standardBeatmapSet.difficultyBeatmaps[j];
                    DifficultyBeatmap customBeatmap = new DifficultyBeatmap(standardBeatmap.difficulty, standardBeatmap.difficultyRank, standardBeatmap.beatmapFilename, standardBeatmap.noteJumpMovementSpeed, 
                                                                            standardBeatmap.noteJumpStartBeatOffset, customDatas._difficultyBeatmapSets[i]._difficultyBeatmaps[j]._customData ?? Tree());
                    customBeatmaps[j] = customBeatmap;
                    beatmapsByFilename[customBeatmap.beatmapFilename] = customBeatmap.customData;
                }
                customBeatmapSets[i] = new DifficultyBeatmapSet(standardBeatmapSet.beatmapCharacteristicName, customBeatmaps);
            }
            CustomLevelInfoSaveData result = new CustomLevelInfoSaveData(standardSaveData.songName, standardSaveData.songSubName, standardSaveData.songAuthorName, standardSaveData.levelAuthorName, 
                                                                         standardSaveData.beatsPerMinute, standardSaveData.songTimeOffset, standardSaveData.shuffle, standardSaveData.shufflePeriod, 
                                                                         standardSaveData.previewStartTime, standardSaveData.previewDuration, standardSaveData.songFilename, standardSaveData.coverImageFilename, 
                                                                         standardSaveData.environmentName, customBeatmapSets, customDatas._customData ?? Tree(), beatmapsByFilename);
            return result;
        }

        public new class DifficultyBeatmap : StandardLevelInfoSaveData.DifficultyBeatmap
        {
            protected dynamic _customData;
            public dynamic customData
            {
                get
                {
                    return _customData;
                }
            }
            public DifficultyBeatmap(string difficultyName, int difficultyRank, string beatmapFilename, float noteJumpMovementSpeed, float noteJumpStartBeatOffset, dynamic customData) 
                              : base(difficultyName, difficultyRank, beatmapFilename, noteJumpMovementSpeed, noteJumpStartBeatOffset)
            {
                _customData = customData;
            }
        }

        /// <summary>
        ///  Kinda hacky, but saves me writing a ton of converters.
        ///  Inspired by StandardLevelInfoSaveData.VersionCheck.
        /// </summary>
        [Serializable]
        private class OopsAllCustomDatas
        {
#pragma warning disable 0649
            [JsonProperty]
            [JsonConverter(typeof(CustomDataConverter))]
            public dynamic _customData;

            [JsonProperty]
            public List<DifficultyBeatmapSet> _difficultyBeatmapSets;

            [Serializable]
            public class DifficultyBeatmapSet
            {
                [JsonProperty]
                public List<DifficultyBeatmap> _difficultyBeatmaps;
            }

            [Serializable]
            public class DifficultyBeatmap
            {
                [JsonProperty]
                [JsonConverter(typeof(CustomDataConverter))]
                public dynamic _customData;
            }
        }
#pragma warning restore 0649
    }
}