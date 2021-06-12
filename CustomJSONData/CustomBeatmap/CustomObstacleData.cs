namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;

    public class CustomObstacleData : ObstacleData
    {
        private CustomObstacleData(float time, int lineIndex, ObstacleType obstacleType, float duration, int width, Dictionary<string, object> customData)
            : base(time, lineIndex, obstacleType, duration, width)
        {
            this.customData = customData;
        }

        public Dictionary<string, object> customData { get; }

        public override BeatmapObjectData GetCopy()
        {
            return new CustomObstacleData(time, lineIndex, obstacleType, duration, width, new Dictionary<string, object>(customData));
        }
    }
}
