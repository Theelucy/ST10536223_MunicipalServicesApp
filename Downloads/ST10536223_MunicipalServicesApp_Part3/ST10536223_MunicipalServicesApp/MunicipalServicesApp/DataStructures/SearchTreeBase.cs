using System;
using System.Collections.Generic;
using System.Drawing;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// A node shared by the three search trees. The extra fields are only used
    /// by the tree that needs them: Height by the AVL tree; IsRed and Parent
    /// by the red-black tree.
    /// </summary>
    public class SearchTreeNode<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public SearchTreeNode<TKey, TValue> Left;
        public SearchTreeNode<TKey, TValue> Right;
        public SearchTreeNode<TKey, TValue> Parent;   // red-black only
        public int Height = 1;                        // AVL only
        public bool IsRed;                            // red-black only
    }

    /// <summary>
    /// Everything the BST, AVL and red-black trees have in common: searching,
    /// ordered traversal, range queries, height and drawing. Only the way a new
    /// key is inserted (and so how the tree stays balanced) differs, so each
    /// subclass implements just <see cref="Insert"/>.
    /// </summary>
    public abstract class SearchTreeBase<TKey, TValue> where TKey : IComparable<TKey>
    {
        protected SearchTreeNode<TKey, TValue> Root;

        public int Count { get; protected set; }

        /// <summary>How many rotations balancing has needed so far.</summary>
        public int RotationCount { get; protected set; }

        public abstract string Name { get; }

        /// <summary>Adds a key. If the key already exists its value is replaced.</summary>
        public abstract void Insert(TKey key, TValue value);

        /// <summary>
        /// Looks a key up, counting how many keys were compared on the way down.
        /// That count is the cost of the search, which is what makes the
        /// trees comparable.
        /// </summary>
        public bool TryFind(TKey key, out TValue value, out int comparisons)
        {
            comparisons = 0;
            SearchTreeNode<TKey, TValue> node = Root;
            while (node != null)
            {
                comparisons++;
                int cmp = key.CompareTo(node.Key);
                if (cmp == 0)
                {
                    value = node.Value;
                    return true;
                }
                node = cmp < 0 ? node.Left : node.Right;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>The keys visited while searching for the given key (found or not).</summary>
        public List<TKey> SearchPath(TKey key)
        {
            var path = new List<TKey>();
            SearchTreeNode<TKey, TValue> node = Root;
            while (node != null)
            {
                path.Add(node.Key);
                int cmp = key.CompareTo(node.Key);
                if (cmp == 0) break;
                node = cmp < 0 ? node.Left : node.Right;
            }
            return path;
        }

        /// <summary>All entries in ascending key order (an in-order traversal).</summary>
        public List<KeyValuePair<TKey, TValue>> InOrder()
        {
            var result = new List<KeyValuePair<TKey, TValue>>();
            var stack = new Stack<SearchTreeNode<TKey, TValue>>();
            SearchTreeNode<TKey, TValue> current = Root;
            while (current != null || stack.Count > 0)
            {
                while (current != null)
                {
                    stack.Push(current);
                    current = current.Left;
                }
                current = stack.Pop();
                result.Add(new KeyValuePair<TKey, TValue>(current.Key, current.Value));
                current = current.Right;
            }
            return result;
        }

        /// <summary>
        /// Values whose keys lie between low and high (inclusive), in key order.
        /// Whole subtrees that cannot contain a match are skipped, so the cost is
        /// O(height + matches) instead of scanning every entry.
        /// </summary>
        public List<TValue> Range(TKey low, TKey high)
        {
            var result = new List<TValue>();
            var stack = new Stack<SearchTreeNode<TKey, TValue>>();
            SearchTreeNode<TKey, TValue> current = Root;
            while (current != null || stack.Count > 0)
            {
                while (current != null)
                {
                    stack.Push(current);
                    // Only go left if there could be smaller keys still >= low.
                    current = low.CompareTo(current.Key) < 0 ? current.Left : null;
                }
                current = stack.Pop();
                if (current.Key.CompareTo(high) > 0)
                {
                    break; // everything after this is larger still
                }
                if (current.Key.CompareTo(low) >= 0)
                {
                    result.Add(current.Value);
                }
                current = current.Right;
            }
            return result;
        }

        /// <summary>Number of levels on the longest path (empty tree = 0).</summary>
        public int Height
        {
            get
            {
                int height = 0;
                var level = new List<SearchTreeNode<TKey, TValue>>();
                if (Root != null) level.Add(Root);
                while (level.Count > 0)
                {
                    height++;
                    var next = new List<SearchTreeNode<TKey, TValue>>();
                    foreach (SearchTreeNode<TKey, TValue> n in level)
                    {
                        if (n.Left != null) next.Add(n.Left);
                        if (n.Right != null) next.Add(n.Right);
                    }
                    level = next;
                }
                return height;
            }
        }

        /// <summary>The smallest possible height for this many nodes: ceil(log2(n + 1)).</summary>
        public int IdealHeight
        {
            get { return Count == 0 ? 0 : (int)Math.Ceiling(Math.Log(Count + 1, 2)); }
        }

        /// <summary>True if every key is larger than everything to its left and smaller than everything to its right.</summary>
        public bool IsValidSearchTree()
        {
            return CheckOrder(Root, default(TKey), false, default(TKey), false);
        }

        private bool CheckOrder(SearchTreeNode<TKey, TValue> node, TKey min, bool hasMin, TKey max, bool hasMax)
        {
            if (node == null) return true;
            if (hasMin && node.Key.CompareTo(min) <= 0) return false;
            if (hasMax && node.Key.CompareTo(max) >= 0) return false;
            return CheckOrder(node.Left, min, hasMin, node.Key, true) &&
                   CheckOrder(node.Right, node.Key, true, max, hasMax);
        }

        protected virtual Color NodeColor(SearchTreeNode<TKey, TValue> node)
        {
            return Color.FromArgb(41, 128, 185);
        }

        /// <summary>Flat description of the tree for drawing; nodes whose keys are in <paramref name="highlight"/> are marked.</summary>
        public List<TreeDrawNode> GetLayout(Func<TKey, string> label, ICollection<TKey> highlight)
        {
            return TreeLayout.Build<SearchTreeNode<TKey, TValue>>(
                Root,
                n => n.Left,
                n => n.Right,
                n => label(n.Key),
                n => NodeColor(n),
                n => highlight != null && highlight.Contains(n.Key),
                null,
                null);
        }
    }
}
