using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomJSONData.Events
{
    public class Priority
    {
        /// <summary>
        /// A Priority of a lower sandboxingLevel n is always considered higher than a Priority of a sandboxingLevel > n, regardless of other fields.
        /// </summary>
        public readonly int sandboxingLevel = 0;

        /// <summary>
        /// The default sorting metric for priorities. 
        /// </summary>
        public readonly int numeric = 0;

        /// <summary>
        /// Overrides numeric priority, guaranteeing the handler with this Priority will run before handlers of the same sandboxingLevel registered through EventSystemInstances with ids in this collection, except where this would create cycles. To break cycles, constraints are removed in ascending order of priority. Can be null.
        /// </summary>
        public readonly ReadOnlyCollection<string> before;

        /// <summary>
        /// Overrides numeric priority, guaranteeing the handler with this Priority will run after handlers of the same sandboxingLevel registered through EventSystemInstances with ids in this collection, except where this would create cycles. To break cycles, constraints are removed in ascending order of priority. Can be null.
        /// </summary>
        public readonly ReadOnlyCollection<string> after;

        public Priority(int numeric, List<string> before = null, List<string> after = null, int sandboxingLevel = 0)
        {
            this.sandboxingLevel = sandboxingLevel;
            this.numeric = numeric;
            this.before = before?.AsReadOnly();
            this.after = after?.AsReadOnly();
        }

        public static readonly Priority DEFAULT = new Priority(0);
        public static readonly Priority SANDBOXED = new Priority(0, null, null, 1);
    }
}
