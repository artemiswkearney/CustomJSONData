using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomJSONData.Trees;

namespace CustomJSONData.Events
{
    public class EventSystemInstance
    {
        public readonly string id;
        EventSystemInstance(string id)
        {
            this.id = id;
        }
        public static EventSystemInstance create(string id)
        {
            if (id == null) throw new Exception("id must not be null");
            return new EventSystemInstance(id);
        }

        public delegate dynamic MinimalHandler(dynamic args);
        public delegate dynamic AheadHandler(dynamic args, float aheadTime);
        public delegate dynamic ManualNextHandler(dynamic args, MinimalHandler next);
        public delegate dynamic FullHandler(dynamic args, MinimalHandler next, float aheadTime);

        public FullHandler toFullHandler(MinimalHandler h) => (args, next, aheadTime) => mergeTrees(h(args), next(args), false);
        public FullHandler toFullHandler(AheadHandler h) => (a, n, t) => mergeTrees(h(a, t), n(a));
        public FullHandler toFullHandler(ManualNextHandler h) => (a, n, t) => h(a, n);

        /// <summary>
        /// TODO: document handler registration
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="priority"></param>
        /// <param name="aheadTime"></param>
        public void register(FullHandler handler, Priority priority = null, float aheadTime = 0)
        {
            if (priority == null) priority = Priority.DEFAULT;
            //TODO: implement handler registration
        }

        public void register(MinimalHandler handler, Priority priority = null, float aheadTime = 0) => register(toFullHandler(handler), priority, aheadTime);
        public void register(AheadHandler handler, Priority priority = null, float aheadTime = 0) => register(toFullHandler(handler), priority, aheadTime);
        public void register(ManualNextHandler handler, Priority priority = null, float aheadTime = 0) => register(toFullHandler(handler), priority, aheadTime);

        /// <summary>
        /// TODO: document event invocation
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="args"></param>
        /// <param name="originInfo"></param>
        /// <returns></returns>
        public dynamic invoke(string eventName, dynamic args, dynamic originInfo)
        {
            // TODO: event invocation
            throw new NotImplementedException();
        }
    }
}
