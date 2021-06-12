namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;

    public class CustomEventData
    {
        public CustomEventData(float time, string type, Dictionary<string, object> data)
        {
            this.time = time;
            this.type = type;
            this.data = data;
        }

        public string type { get; }

        public float time { get; }

        public Dictionary<string, object> data { get; }
    }
}
