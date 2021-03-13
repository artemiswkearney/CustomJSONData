namespace CustomJSONData.CustomBeatmap
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using static CustomJSONData.Trees;

    public class CustomBeatmapData : BeatmapData
    {
        public static event Action<CustomBeatmapData> CustomBeatmapDataWasCreated;

        public List<CustomEventData> customEventsData { get; }
        public dynamic customData { get; private set; }
        public dynamic beatmapCustomData { get; private set; }
        public dynamic levelCustomData { get; private set; }

        private static MethodInfo CopyBeatmapObjectsMethod
        {
            get
            {
                if (_copyBeatmapObjects == null)
                {
                    _copyBeatmapObjects = typeof(BeatmapData).GetMethod("CopyBeatmapObjects", BindingFlags.Static | BindingFlags.NonPublic);
                }

                return _copyBeatmapObjects;
            }
        }

        private static MethodInfo CopyBeatmapEventsMethod
        {
            get
            {
                if (_copyBeatmapEvents == null)
                {
                    _copyBeatmapEvents = typeof(BeatmapData).GetMethod("CopyBeatmapEvents", BindingFlags.Static | BindingFlags.NonPublic);
                }

                return _copyBeatmapEvents;
            }
        }

        private static MethodInfo CopyAvailableSpecialEventsPerKeywordDictionaryMethod
        {
            get
            {
                if (_copyAvailableSpecialEventsPerKeywordDictionary == null)
                {
                    _copyAvailableSpecialEventsPerKeywordDictionary = typeof(BeatmapData).GetMethod("CopyAvailableSpecialEventsPerKeywordDictionary", BindingFlags.Static | BindingFlags.NonPublic);
                }

                return _copyAvailableSpecialEventsPerKeywordDictionary;
            }
        }

        private static MethodInfo _copyBeatmapObjects;
        private static MethodInfo _copyBeatmapEvents;
        private static MethodInfo _copyAvailableSpecialEventsPerKeywordDictionary;

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

        public static void CopyBeatmapObjects(IReadonlyBeatmapData src, BeatmapData dst)
        {
            CopyBeatmapObjectsMethod.Invoke(null, new object[] { src, dst });
        }

        public static void CopyBeatmapEvents(IReadonlyBeatmapData src, BeatmapData dst)
        {
            CopyBeatmapEventsMethod.Invoke(null, new object[] { src, dst });
        }

        public static void CopyAvailableSpecialEventsPerKeywordDictionary(IReadonlyBeatmapData src, BeatmapData dst)
        {
            CopyAvailableSpecialEventsPerKeywordDictionaryMethod.Invoke(null, new object[] { src, dst });
        }

        public static void CopyCustomData(CustomBeatmapData src, CustomBeatmapData dst)
        {
            foreach (CustomEventData customEventData in src.customEventsData)
            {
                dst.AddCustomEventData(customEventData);
            }
            dst.SetCustomData(Trees.copy(src?.customData ?? Tree()));
            dst.SetLevelCustomData(Trees.copy(src?.beatmapCustomData ?? Tree()), Trees.copy(src?.levelCustomData ?? Tree()));
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
