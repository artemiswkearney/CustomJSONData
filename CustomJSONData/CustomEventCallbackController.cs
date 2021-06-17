namespace CustomJSONData
{
    using System;
    using System.Collections.Generic;
    using CustomJSONData.CustomBeatmap;
    using IPA.Utilities;
    using UnityEngine;

    public class CustomEventCallbackController : MonoBehaviour
    {
        private static readonly FieldAccessor<BeatmapObjectCallbackController, IAudioTimeSource>.Accessor _audioTimeSourceAccessor = FieldAccessor<BeatmapObjectCallbackController, IAudioTimeSource>.GetAccessor("_audioTimeSource");
        private static readonly FieldAccessor<BeatmapObjectCallbackController, float>.Accessor _spawningStartTimeAccessor = FieldAccessor<BeatmapObjectCallbackController, float>.GetAccessor("_spawningStartTime");

        private readonly List<CustomEventCallbackData> _customEventCallbackData = new List<CustomEventCallbackData>();

        private BeatmapObjectCallbackController _beatmapObjectCallbackController;

        public delegate void CustomEventCallback(CustomEventData eventData);

        public static event Action<CustomEventCallbackController> didInitEvent;

        public CustomBeatmapData BeatmapData { get; private set; }

        public BeatmapObjectCallbackController BeatmapObjectCallbackController => _beatmapObjectCallbackController;

        public IAudioTimeSource AudioTimeSource => _audioTimeSourceAccessor(ref _beatmapObjectCallbackController);

        public float SpawningStartTime => _spawningStartTimeAccessor(ref _beatmapObjectCallbackController);

        public CustomEventCallbackData AddCustomEventCallback(CustomEventCallback callback, float aheadTime = 0, bool callIfBeforeStartTime = true)
        {
            CustomEventCallbackData customEventCallbackData = new CustomEventCallbackData(callback, aheadTime, callIfBeforeStartTime);
            _customEventCallbackData.Add(customEventCallbackData);
            return customEventCallbackData;
        }

        public void RemoveBeatmapEventCallback(CustomEventCallbackData callbackData)
        {
            _customEventCallbackData?.Remove(callbackData);
        }

        internal void SetNewBeatmapData(IReadonlyBeatmapData beatmapData)
        {
            BeatmapData = (CustomBeatmapData)beatmapData;

            foreach (CustomEventCallbackData customEventCallbackData in _customEventCallbackData)
            {
                customEventCallbackData.nextEventIndex = 0;
            }
        }

        internal void Init(BeatmapObjectCallbackController beatmapObjectCallbackController, IReadonlyBeatmapData beatmapData)
        {
            _beatmapObjectCallbackController = beatmapObjectCallbackController;
            SetNewBeatmapData(beatmapData);
            didInitEvent?.Invoke(this);
        }

        private void LateUpdate()
        {
            if (_beatmapObjectCallbackController.enabled && BeatmapData != null)
            {
                for (int l = 0; l < _customEventCallbackData.Count; l++)
                {
                    CustomEventCallbackData customEventCallbackData = _customEventCallbackData[l];
                    while (customEventCallbackData.nextEventIndex < BeatmapData.customEventsData.Count)
                    {
                        CustomEventData customEventData = BeatmapData.customEventsData[customEventCallbackData.nextEventIndex];
                        if (customEventData.time - customEventCallbackData.aheadTime >= AudioTimeSource.songTime)
                        {
                            break;
                        }

                        // skip events before song start
                        if (customEventData.time >= SpawningStartTime || customEventCallbackData.callIfBeforeStartTime)
                        {
                            customEventCallbackData.callback(customEventData);
                        }

                        customEventCallbackData.nextEventIndex++;
                    }
                }
            }
        }

        public class CustomEventCallbackData
        {
            public CustomEventCallbackData(CustomEventCallback callback, float aheadTime, bool callIfBeforeStartTime)
            {
                this.callback = callback;
                this.aheadTime = aheadTime;
                this.callIfBeforeStartTime = callIfBeforeStartTime;
                nextEventIndex = 0;
            }

            public CustomEventCallback callback { get; set; }

            public float aheadTime { get; set; }

            public int nextEventIndex { get; set; }

            public bool callIfBeforeStartTime { get; set; }
        }
    }
}
