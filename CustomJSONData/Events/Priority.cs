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
        /// The default sorting metric for priorities. 
        /// </summary>
        public readonly int numeric = 0;

        /// <summary>
        /// Overrides numeric priority, guaranteeing the handler with this Priority will run before handlers registered through EventSystemInstances with ids in this collection, except where this would create cycles. To break cycles, constraints are removed in ascending order of priority. Can be null.
        /// </summary>
        public readonly ReadOnlyCollection<string> before;

        /// <summary>
        /// Overrides numeric priority, guaranteeing the handler with this Priority will run after handlers registered through EventSystemInstances with ids in this collection, except where this would create cycles. To break cycles, constraints are removed in ascending order of priority. Can be null.
        /// </summary>
        public readonly ReadOnlyCollection<string> after;

        public Priority(int numeric, List<string> before = null, List<string> after = null)
        {
            this.numeric = numeric;
            this.before = before?.AsReadOnly();
            this.after = after?.AsReadOnly();
        }

        public static readonly Priority DEFAULT = new Priority(0);
        public static readonly Priority HIGH = new Priority(1000);
        public static readonly Priority LOW = new Priority(-1000);

        public static implicit operator Priority(int p) => new Priority(p);

        /// <summary>
        /// While it's fine to explicitly convert an int to a priority, converting back is explicit because it can lose information
        /// </summary>
        public static explicit operator int(Priority p) => p.numeric;
    }
}
