namespace CustomJSONData.CustomBeatmap
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class CustomBeatmapData : BeatmapData
    {
        private static MethodInfo _copyBeatmapObjects;
        private static MethodInfo _copyBeatmapEvents;
        private static MethodInfo _copyAvailableSpecialEventsPerKeywordDictionary;

        public CustomBeatmapData(int numberOfLines)
            : base(numberOfLines)
        {
            customEventsData = new List<CustomEventData>(200);
        }

        public static event Action<CustomBeatmapData> CustomBeatmapDataWasCreated;

        public List<CustomEventData> customEventsData { get; }

        public Dictionary<string, object> customData { get; private set; }

        public Dictionary<string, object> beatmapCustomData { get; private set; }

        public Dictionary<string, object> levelCustomData { get; private set; }

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

            dst.SetCustomData(src.customData.Copy());
            dst.SetLevelCustomData(src.beatmapCustomData.Copy(), src.levelCustomData.Copy());
        }

        public override BeatmapData GetCopy()
        {
            CustomBeatmapData customBeatmapData = new CustomBeatmapData(_beatmapLinesData.Length);
            CopyBeatmapObjects(this, customBeatmapData);
            CopyBeatmapEvents(this, customBeatmapData);
            CopyAvailableSpecialEventsPerKeywordDictionary(this, customBeatmapData);
            CopyCustomData(this, customBeatmapData);
            return customBeatmapData;
        }

        public override BeatmapData GetCopyWithoutEvents()
        {
            CustomBeatmapData customBeatmapData = new CustomBeatmapData(_beatmapLinesData.Length);
            CopyBeatmapObjects(this, customBeatmapData);
            CopyCustomData(this, customBeatmapData);
            return customBeatmapData;
        }

        public override BeatmapData GetCopyWithoutBeatmapObjects()
        {
            CustomBeatmapData customBeatmapData = new CustomBeatmapData(_beatmapLinesData.Length);
            CopyBeatmapEvents(this, customBeatmapData);
            CopyAvailableSpecialEventsPerKeywordDictionary(this, customBeatmapData);
            CopyCustomData(this, customBeatmapData);
            return customBeatmapData;
        }

        internal void AddCustomEventData(CustomEventData customEventData)
        {
            customEventsData.Add(customEventData);
        }

        internal void SetCustomData(Dictionary<string, object> customData)
        {
            this.customData = customData;
        }

        internal void SetLevelCustomData(Dictionary<string, object> beatmapCustomData, Dictionary<string, object> levelCustomData)
        {
            this.beatmapCustomData = beatmapCustomData;
            this.levelCustomData = levelCustomData;
            CustomBeatmapDataWasCreated?.Invoke(this);
        }
    }
}
