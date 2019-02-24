using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TreeType = System.Dynamic.ExpandoObject;
using TreeDict = System.Collections.Generic.IDictionary<string, object>;

namespace CustomJSONData
{
    /// <summary>
    /// Utility functions for creating and manipulating Trees, which are dynamics guaranteed to have the following properties:
    /// - Implements all the features of the ExpandoObject class that would be visible to standard C# code (notably including implementation of IDictionary\<string, object\>).
    /// - No members of a Tree will be Trees with the original Tree as a member.
    /// - All Trees will be of the same underlying type; however, the actual type used is an implementation detail and should not be relied upon.
    /// </summary>
    public static class Trees
    {
        /// <summary>
        /// Used to construct a new Tree, as it does not exist as a proper class (by design).
        /// </summary>
        /// <returns>An empty Tree.</returns>
        public static dynamic Tree() => new TreeType();

        /// <summary>
        /// Used to more easily construct a Tree with an initial set of members.
        /// </summary>
        /// <param name="contents">The members to initialize the new Tree with.</param>
        /// <returns>A new Tree containing the provided members.</returns>
        public static dynamic Tree(List<KeyValuePair<string, object>> contents)
        {
            dynamic t = Tree();
            foreach(var p in contents)
            {
                t[p.Key] = p.Value;
            }
            return t;
        }

        /// <summary>
        /// Makes a semi-deep copy of a tree - members of non-Tree reference types in copy(t) will refer to the same objects as in t, but Tree members will be recursively copied.
        /// </summary>
        /// <param name="t">The tree to copy.</param>
        /// <returns>A copy of t.</returns>
        public static dynamic copy(TreeType t)
        {
            dynamic result = Tree();
            foreach(var p in t)
            {
                result[p.Key] = p.Value is TreeType ? copy(p.Value as TreeType) : p.Value;
            }
            return result;
        }

        /// <summary>
        /// Makes a shallow copy of a tree - all members of reference types, including sub-Trees, will refer to the same objects as in t.
        /// </summary>
        /// <param name="t">The tree to copy.</param>
        /// <returns>A copy of t.</returns>
        public static dynamic shallowCopy(TreeType t) => Tree(t.ToList());

        public static dynamic at(this TreeDict tree, string memberName)
        {
            tree.TryGetValue(memberName, out object result);
            return result;
        }

        /// <summary>
        /// Combines the contents of the two trees, recursively merging subtrees where appropriate.
        /// When both Trees contain members of the same name:
        /// - If that member is of the same underlying type as a Tree on both arguments, those Trees will be merged with the same priorities as the Trees they were members of.
        /// - Otherwise, the member from highPriority will be used.
        /// </summary>
        /// <param name="highPriority">The Tree to keep the members of in case of conflict</param>
        /// <param name="lowPriority">The Tree to drop the members of in case of conflict</param>
        /// <param name="copySubtrees">If true, all Trees encountered as members in only one of highPriority and lowPriority will be copied as with copy(Tree) rather than copied by reference. Increases performance cost by the cost of those copies.</param>
        /// <returns>A tree consisting of highPriority and lowPriority merged</returns>
        public static dynamic mergeTrees(TreeType highPriority, TreeType lowPriority, bool copySubtrees = true)
        {
            dynamic result = copySubtrees ? copy(lowPriority) : shallowCopy(lowPriority);
            foreach(KeyValuePair<string, object> pair in (TreeDict)highPriority)
            {
                if (pair.Value is TreeType)
                {
                    if (at((TreeDict)lowPriority, pair.Key) is TreeType)
                    {
                        result[pair.Key] = mergeTrees(pair.Value as TreeType, ((TreeDict)lowPriority)[pair.Key] as TreeType);
                    }
                    else result[pair.Key] = copySubtrees ? copy(pair.Value as TreeType) : pair.Value;
                }
                else
                {
                    result[pair.Key] = pair.Value;
                }
            }
            return result;
        }
    }
}
