namespace CustomJSONData.CustomBeatmap
{
    public class CustomObstacleData : ObstacleData
    {
        public CustomObstacleData(int id, float time, int lineIndex, ObstacleType obstacleType, float duration, int width, dynamic customData) : base(id, time, lineIndex, obstacleType, duration, width)
        {
            this.customData = customData;
        }

        public dynamic customData;

        public override BeatmapObjectData GetCopy()
        {
            return new CustomObstacleData(id, time, lineIndex, obstacleType, duration, width, Trees.copy(customData));
        }
    }
}