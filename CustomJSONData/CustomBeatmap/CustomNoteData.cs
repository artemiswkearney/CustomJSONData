namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;

    public class CustomNoteData : NoteData
    {
        private CustomNoteData(
            float time,
            int lineIndex,
            NoteLineLayer noteLineLayer,
            NoteLineLayer startNoteLineLayer,
            ColorType colorType,
            NoteCutDirection cutDirection,
            float timeToNextColorNote,
            float timeToPrevColorNote,
            int flipLineIndex,
            float flipYSide,
            float duration,
            Dictionary<string, object> customData)
                       : base(time, lineIndex, noteLineLayer, startNoteLineLayer, colorType, cutDirection, timeToNextColorNote, timeToPrevColorNote, flipLineIndex, flipYSide, duration)
        {
            this.customData = customData;
        }

        public Dictionary<string, object> customData { get; }

        public override BeatmapObjectData GetCopy()
        {
            return new CustomNoteData(time, lineIndex, noteLineLayer, startNoteLineLayer, colorType, cutDirection, timeToNextColorNote, timeToPrevColorNote, flipLineIndex, flipYSide, duration, new Dictionary<string, object>(customData));
        }

        internal static CustomNoteData CreateBombNoteData(float time, int lineIndex, NoteLineLayer noteLineLayer, Dictionary<string, object> customData)
        {
            return new CustomNoteData(time, lineIndex, noteLineLayer, noteLineLayer, ColorType.None, NoteCutDirection.None, 0f, 0f, lineIndex, 0f, 0f, customData);
        }

        internal static CustomNoteData CreateBasicNoteData(float time, int lineIndex, NoteLineLayer noteLineLayer, ColorType colorType, NoteCutDirection cutDirection, Dictionary<string, object> customData)
        {
            return new CustomNoteData(time, lineIndex, noteLineLayer, noteLineLayer, colorType, cutDirection, 0f, 0f, lineIndex, 0f, 0f, customData);
        }
    }
}
