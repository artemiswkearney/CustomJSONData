using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomJSONData.CustomBeatmap
{
    class CustomObstacleData : ObstacleData
    {
        public CustomObstacleData(int id, float time, int lineIndex, ObstacleType obstacleType, float duration, int width, dynamic customData) : base(id, time, lineIndex, obstacleType, duration, width)
        {
            this.customData = customData;
        }
        public dynamic customData;
    }
}
