namespace CustomJSONData.CustomBeatmap
{
    using System;
    using System.Collections.Generic;

    public class CustomBeatmapData : BeatmapData
    {
        public static event Action<CustomBeatmapData> CustomBeatmapDataWasCreated;

        public List<CustomEventData> customEventsData { get; }
        public dynamic customData { get; private set; }
        public dynamic beatmapCustomData { get; private set; }
        public dynamic levelCustomData { get; private set; }

        public CustomBeatmapData(int numberOfLines) : base(numberOfLines)
        {
            customEventsData = new List<CustomEventData>(200);
        }

        // In a perfect world, these would be overrides. Instead, we harmony patch
        internal new CustomBeatmapData GetCopy()
        {
            CustomBeatmapData customBeatmapData = new CustomBeatmapData(_beatmapLinesData.Length);
            CopyBeatmapObjects(this, customBeatmapData);
            CopyBeatmapEvents(this, customBeatmapData);
            CopyAvailableSpecialEventsPerKeywordDictionary(this, customBeatmapData);
            CopyCustomData(this, customBeatmapData);
            return customBeatmapData;
        }

        internal new CustomBeatmapData GetCopyWithoutEvents()
        {
            CustomBeatmapData customBeatmapData = new CustomBeatmapData(_beatmapLinesData.Length);
            CopyBeatmapObjects(this, customBeatmapData);
            CopyCustomData(this, customBeatmapData);
            return customBeatmapData;
        }

        internal new CustomBeatmapData GetCopyWithoutBeatmapObjects()
        {
            CustomBeatmapData customBeatmapData = new CustomBeatmapData(_beatmapLinesData.Length);
            CopyBeatmapEvents(this, customBeatmapData);
            CopyAvailableSpecialEventsPerKeywordDictionary(this, customBeatmapData);
            CopyCustomData(this, customBeatmapData);
            return customBeatmapData;
        }

        // The 'src' variable is completely unneccessary because this is an instance method but i'm just matching the base game methods
        internal void CopyCustomData(CustomBeatmapData src, CustomBeatmapData dst)
        {
            foreach (CustomEventData customEventData in src.customEventsData)
            {
                dst.AddCustomEventData(customEventData);
            }
            dst.SetCustomData(src.customData);
            dst.SetLevelCustomData(src.beatmapCustomData, src.levelCustomData);
        }

        internal void AddCustomEventData(CustomEventData customEventData)
        {
            customEventsData.Add(customEventData);
        }

        internal void SetCustomData(dynamic customData)
        {
            this.customData = customData;
        }

        internal void SetLevelCustomData(dynamic beatmapCustomData, dynamic levelCustomData)
        {
            this.beatmapCustomData = beatmapCustomData;
            this.levelCustomData = levelCustomData;
            CustomBeatmapDataWasCreated?.Invoke(this);
        }
    }
}
