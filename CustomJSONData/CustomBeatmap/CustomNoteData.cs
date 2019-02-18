using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomNoteData : NoteData
    {
        public CustomNoteData(int id, float time, int lineIndex, NoteLineLayer noteLineLayer, NoteLineLayer startNoteLineLayer, NoteType noteType, NoteCutDirection cutDirection, float timeToNextBasicNote, float timeToPrevBasicNote, dynamic customData) : base(id, time, lineIndex, noteLineLayer, startNoteLineLayer, noteType, cutDirection, timeToNextBasicNote, timeToPrevBasicNote)
        {
            this.customData = customData;
        }
        public dynamic customData;
    }
}
