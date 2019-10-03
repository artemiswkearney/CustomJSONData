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
    /// - Implements all the features of the ExpandoObject class that would be visible to standard C# code (notably including implementation of<see cref="IDictionary{string, object}"/>).
    /// - No members of a Tree will be Trees with the original Tree as a member.
    /// - All Trees will be of the same underlying type; however, the actual type used is an implementation detail and should not be relied upon.
    /// 
    /// Note that calling many of the functions in this class on invalid Trees (those that contain themselves directly or indirectly) is undefined behavior, and may lead to infinite loops 
    /// or stack overflows. To avoid inadvertently creating invalid Trees, we recommend never adding a subtree to a Tree that's not newly created, although validating defensively with 
    /// <see cref="isTree(object)"/> is also an option.
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
            TreeDict t = (TreeDict)Tree();
            foreach(var p in contents)
            {
                t[p.Key] = p.Value;
            }
            return t;
        }

        /// <summary>
        /// Makes a semi-deep copy of a tree - members of non-Tree reference types in copy(<paramref name="t"/>) will refer to the same objects as in <paramref name="t"/>, but Tree members will be recursively copied.
        /// </summary>
        /// <param name="t">The tree to copy.</param>
        /// <returns>A copy of t.</returns>
        public static dynamic copy(this TreeType t)
        {
            dynamic result = Tree();
            foreach(var p in t)
            {
                (result as TreeDict)[p.Key] = p.Value is TreeType ? copy(p.Value as TreeType) : p.Value;
            }
            return result;
        }

        /// <summary>
        /// Makes a shallow copy of a tree - all members of reference types, including sub-Trees, will refer to the same objects as in <paramref name="t"/>.
        /// </summary>
        /// <param name="t">The tree to copy.</param>
        /// <returns>A copy of <paramref name="t"/>.</returns>
        public static dynamic shallowCopy(this TreeType t) => Tree(t.ToList());

        /// <summary>
        /// Safely accesses a member of a tree, returning null if the member does not exist or if <paramref name="tree"/> is null.
        /// </summary>
        /// <param name="tree">The tree to access a member of</param>
        /// <param name="memberName">The name of the member to be accessed</param>
        /// <returns>The value of <paramref name="tree"/>'s member <paramref name="memberName"/>, or null if tree is null or no such member exists</returns>
        public static dynamic at(this TreeDict tree, string memberName)
        {
            if (tree == null) return null;
            tree.TryGetValue(memberName, out object result);
            return result;
        }

        /// <summary>
        /// Safely accesses a member of a tree, returning <paramref name="defaultValue"/> if the member does not exist or if <paramref name="tree"/> is null.
        /// If <typeparamref name="T"/> is a value type and you want <paramref name="defaultValue"/> to be null, use <see cref="getNullable{T}(TreeDict, string, T?)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the member to be returned</typeparam>
        /// <param name="tree">The tree to access a member of</param>
        /// <param name="memberName">The name of the member to be accessed</param>
        /// <param name="defaultValue">The value to be returned if <paramref name="tree"/> is null or has no member <paramref name="memberName"/></param>
        /// <returns>The value of <paramref name="tree"/>'s member <paramref name="memberName"/>, or <paramref name="defaultValue"/> if tree is null or no such member exists</returns>
        public static T get<T>(this TreeDict tree, string memberName, T defaultValue = default)
        {
            if (tree == null) return defaultValue;
            if (!tree.TryGetValue(memberName, out dynamic result))
                return defaultValue;
            try
            {
                return (T)result;
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// Safely accesses a member of a tree, returning <paramref name="defaultValue"/> if the member does not exist or if <paramref name="tree"/> is null.
        /// Use <see cref="get{T}(TreeDict, string, T)"/> unless <typeparamref name="T"/> is a value type and you want a <see cref="Nullable{T}"/> as a result.
        /// </summary>
        /// <typeparam name="T">The type of the member to be returned</typeparam>
        /// <param name="tree">The tree to access a member of</param>
        /// <param name="memberName">The name of the member to be accessed</param>
        /// <param name="defaultValue">The value to be returned if <paramref name="tree"/> is null or has no member <paramref name="memberName"/></param>
        /// <returns>The value of <paramref name="tree"/>'s member <paramref name="memberName"/>, or <paramref name="defaultValue"/> if tree is null or no such member exists</returns>
        public static T? getNullable<T>(this TreeDict tree, string memberName, T? defaultValue = null) where T : struct
        {
            if (tree == null) return defaultValue;
            if (!tree.TryGetValue(memberName, out dynamic result))
                return defaultValue;
            try
            {
                return (T?)(T)result ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// Safely accesses a member of a tree, returning <paramref name="defaultValue"/> if the member does not exist or if <paramref name="tree"/> is null.
        /// If <typeparamref name="T"/> is a value type and you want <paramref name="defaultValue"/> to be null, use <see cref="getNullable{T}(object, string, T?)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the member to be returned</typeparam>
        /// <param name="tree">The tree to access a member of</param>
        /// <param name="memberName">The name of the member to be accessed</param>
        /// <param name="defaultValue">The value to be returned if <paramref name="tree"/> is null or has no member <paramref name="memberName"/></param>
        /// <returns>The value of <paramref name="tree"/>'s member <paramref name="memberName"/>, or <paramref name="defaultValue"/> if tree is null or no such member exists</returns>
        public static T get<T>(this object tree, string memberName, T defaultValue = default(T))
        {
            try
            {
                if ((TreeDict)tree == null) return defaultValue;
                return get((TreeDict)tree, memberName, defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// Safely accesses a member of a tree, returning <paramref name="defaultValue"/> if the member does not exist or if <paramref name="tree"/> is null.
        /// Use <see cref="get{T}(object, string, T)"/> unless <typeparamref name="T"/> is a value type and you want a <see cref="Nullable{T}"/> as a result.
        /// </summary>
        /// <typeparam name="T">The type of the member to be returned</typeparam>
        /// <param name="tree">The tree to access a member of</param>
        /// <param name="memberName">The name of the member to be accessed</param>
        /// <param name="defaultValue">The value to be returned if <paramref name="tree"/> is null or has no member <paramref name="memberName"/></param>
        /// <returns>The value of <paramref name="tree"/>'s member <paramref name="memberName"/>, or <paramref name="defaultValue"/> if tree is null or no such member exists</returns>
        public static T? getNullable<T>(this object tree, string memberName, T? defaultValue = null) where T : struct
        {
            try
            {
                if ((TreeDict)tree == null) return defaultValue;
                return getNullable((TreeDict)tree, memberName, defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Evaluates <paramref name="func"/> and returns its result. If <paramref name="func"/> throws an exception, returns null instead.
        /// </summary>
        /// <param name="func">The function to be evaluated.</param>
        /// <returns></returns>
        public static dynamic tryNull(Func<dynamic> func)
        {
            try { return func(); } catch { return null; }
        }

        /// <summary>
        /// Combines the contents of the two trees, recursively merging subtrees where appropriate.
        /// When both Trees contain members of the same name:
        /// - If that member is of the same underlying type as a Tree on both arguments, those Trees will be merged with the same priorities as the Trees they were members of.
        /// - Otherwise, the member from highPriority will be used.
        /// If either Tree is null, returns the other.
        /// If both are null, returns null.
        /// </summary>
        /// <param name="highPriority">The Tree to keep the members of in case of conflict</param>
        /// <param name="lowPriority">The Tree to drop the members of in case of conflict</param>
        /// <param name="copySubtrees">If true, all Trees encountered as members in only one of highPriority and lowPriority will be copied as with copy(Tree) rather than copied by reference. 
        /// Increases performance cost by the cost of those copies.</param>
        /// <returns>A tree consisting of highPriority and lowPriority merged</returns>
        public static dynamic mergeTrees(TreeType highPriority, TreeType lowPriority, bool copySubtrees = true)
        {
            if (highPriority == null) return lowPriority;
            if (lowPriority == null) return highPriority;
            dynamic result = copySubtrees ? copy(lowPriority) : shallowCopy(lowPriority);
            foreach(KeyValuePair<string, object> pair in highPriority)
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

        /// <summary>
        /// An extension method form of <see cref="mergeTrees(TreeType, TreeType, bool)"/>.
        /// </summary>
        /// <seealso cref="mergeTrees(TreeType, TreeType, bool)"/>
        public static dynamic merge(this TreeType highPriority, TreeType lowPriority, bool copySubtrees = true) => mergeTrees(highPriority, lowPriority, copySubtrees);

        /// <summary>
        /// Maps an <see cref="Action{Tree}"/> over a tree and its subtrees.
        /// <paramref name="action"/> will first be called on <paramref name="tree"/>, then on each subtree as it's encountered in a depth-first search.
        /// </summary>
        /// <param name="tree">The tree to map over</param>
        /// <param name="action">The Action to map over the tree</param>
        public static void map(this TreeType tree, Action<dynamic> action)
        {
            action(tree);
            foreach (var pair in tree)
            {
                if (pair.Value is TreeType t)
                {
                    map(t, action);
                }
            }
        }

        /// <summary>
        /// Maps a <see cref="Func{Tree, Tree}"/> over <paramref name="tree"/> and its subtrees, replacing those subtrees with the result of mapping <paramref name="func"/> over them.
        /// If <paramref name="doCopy"/> is true, the Trees <paramref name="func"/> is applied to will be <see cref="copy(TreeType)">copies</see> of <paramref name="tree"/> and its subtrees.
        /// </summary>
        /// <param name="tree">The tree to map over.</param>
        /// <param name="func">The transformation to apply to each subtree.</param>
        /// <param name="doCopy">Whether to copy the tree before transforming it.</param>
        /// <returns></returns>
        public static dynamic map(this TreeType tree, Func<dynamic, dynamic> func, bool doCopy = true)
        {
            TreeType newTree = func(doCopy ? copy(tree) : tree);
            var treeKeys = new List<string>();
            foreach (var pair in newTree)
            {
                if (pair.Value is TreeType)
                {
                    treeKeys.Add(pair.Key);
                }
            }
            foreach(var key in treeKeys)
            {
                ((TreeDict)newTree)[key] = map(((TreeDict)newTree)[key] as TreeType, func, false);
            }
            return newTree;
        }

        /// <summary>
        /// Checks whether an object is of the right type to be a Tree. Does *not* validate whether the object is a *valid* Tree; use <see cref="isTree(object)"/> for this purpose.
        /// </summary>
        /// <param name="o">The object to typecheck.</param>
        /// <returns>Whether <paramref name="o"/> is of the right type to be a Tree.</returns>
        public static bool isTreeType(this object o)
        {
            return o is TreeType;
        }

        /// <summary>
        /// Checks whether an object is a valid Tree. (Valid Trees are all of the same underlying type, and do not contain themselves directly or indirectly.) This involves recursively validating 
        /// all subtrees; if performance is critical, consider whether <see cref="isTreeType(object)"/> suits your needs better.
        /// </summary>
        /// <param name="o">The object to validate.</param>
        /// <returns>Whether <paramref name="o"/> meets all the criteria to be a Tree.</returns>
        public static bool isTree(this object o)
        {
            // Validates a member of a Tree (or a Tree itself) to make sure it doesn't contain itself or any of its parent Trees.
            // ACCUMULATOR: parents
            // Remembers: all trees that are parents of child
            bool isTreeInternal(object child, HashSet<object> parents)
            {
                // no need to validate non-TreeTypes
                if (!isTreeType(child)) return true;
                // if this child is one of its own parents, there's a subtree loop; invalid tree
                if (parents.Contains(child)) return false;

                // update accumulator
                var newSeen = new HashSet<object> { child };
                newSeen.UnionWith(parents);

                // if all subtrees are valid, this tree is valid
                return (child as TreeDict).All(p => isTreeInternal(p.Value, newSeen));
            }

            return isTreeType(o) && isTreeInternal(o, new HashSet<object>());
        }
    }
}
