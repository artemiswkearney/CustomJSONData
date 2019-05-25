using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomEventData
    {
        public string type { get; protected set; }
        public float time { get; protected set; }
        public dynamic data { get; protected set; }
        public CustomEventData(float time, string type, dynamic data)
        {
            this.time = time;
            this.type = type;
            this.data = data;
        }
        public CustomEventData GetCopy()
        {
            return new CustomEventData(time, type, Trees.copy(data));
        }
    }
}
