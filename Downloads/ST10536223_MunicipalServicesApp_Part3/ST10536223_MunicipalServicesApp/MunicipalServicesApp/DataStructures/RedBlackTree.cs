using System;
using System.Drawing;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// A red-black tree: a binary search tree whose nodes are coloured so that
    ///   1. the root is black,
    ///   2. a red node never has a red child, and
    ///   3. every path from a node down to a missing child passes through the
    ///      same number of black nodes.
    /// Those rules keep the longest path at most twice the shortest, so the
    /// height stays within 2 * log2(n + 1). Compared with an AVL tree it is
    /// balanced a little less strictly, but needs fewer rotations per insert,
    /// which suits a data set where new requests are added all the time.
    /// </summary>
    public class RedBlackTree<TKey, TValue> : SearchTreeBase<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public override string Name
        {
            get { return "Red-black tree"; }
        }

        public override void Insert(TKey key, TValue value)
        {
            SearchTreeNode<TKey, TValue> parent = null;
            SearchTreeNode<TKey, TValue> current = Root;

            while (current != null)
            {
                parent = current;
                int cmp = key.CompareTo(current.Key);
                if (cmp == 0)
                {
                    current.Value = value;
                    return;
                }
                current = cmp < 0 ? current.Left : current.Right;
            }

            var node = new SearchTreeNode<TKey, TValue>
            {
                Key = key,
                Value = value,
                Parent = parent,
                IsRed = true // new nodes start red so black-heights stay equal
            };

            if (parent == null)
            {
                Root = node;
            }
            else if (key.CompareTo(parent.Key) < 0)
            {
                parent.Left = node;
            }
            else
            {
                parent.Right = node;
            }

            Count++;
            FixAfterInsert(node);
        }

        private void FixAfterInsert(SearchTreeNode<TKey, TValue> node)
        {
            // Only a red parent can break a rule (red node with red child).
            while (node != Root && node.Parent.IsRed)
            {
                SearchTreeNode<TKey, TValue> parent = node.Parent;
                SearchTreeNode<TKey, TValue> grand = parent.Parent; // exists: a red parent is never the root

                if (parent == grand.Left)
                {
                    SearchTreeNode<TKey, TValue> uncle = grand.Right;
                    if (uncle != null && uncle.IsRed)
                    {
                        // Red uncle: recolour and push the problem two levels up.
                        parent.IsRed = false;
                        uncle.IsRed = false;
                        grand.IsRed = true;
                        node = grand;
                    }
                    else
                    {
                        if (node == parent.Right)
                        {
                            node = parent;          // inner child: rotate it to the outside first
                            RotateLeft(node);
                            parent = node.Parent;
                        }
                        parent.IsRed = false;
                        grand.IsRed = true;
                        RotateRight(grand);
                    }
                }
                else
                {
                    SearchTreeNode<TKey, TValue> uncle = grand.Left;
                    if (uncle != null && uncle.IsRed)
                    {
                        parent.IsRed = false;
                        uncle.IsRed = false;
                        grand.IsRed = true;
                        node = grand;
                    }
                    else
                    {
                        if (node == parent.Left)
                        {
                            node = parent;
                            RotateRight(node);
                            parent = node.Parent;
                        }
                        parent.IsRed = false;
                        grand.IsRed = true;
                        RotateLeft(grand);
                    }
                }
            }

            Root.IsRed = false;
        }

        private void RotateLeft(SearchTreeNode<TKey, TValue> x)
        {
            SearchTreeNode<TKey, TValue> y = x.Right;
            x.Right = y.Left;
            if (y.Left != null) y.Left.Parent = x;

            y.Parent = x.Parent;
            if (x.Parent == null) Root = y;
            else if (x == x.Parent.Left) x.Parent.Left = y;
            else x.Parent.Right = y;

            y.Left = x;
            x.Parent = y;
            RotationCount++;
        }

        private void RotateRight(SearchTreeNode<TKey, TValue> x)
        {
            SearchTreeNode<TKey, TValue> y = x.Left;
            x.Left = y.Right;
            if (y.Right != null) y.Right.Parent = x;

            y.Parent = x.Parent;
            if (x.Parent == null) Root = y;
            else if (x == x.Parent.Right) x.Parent.Right = y;
            else x.Parent.Left = y;

            y.Right = x;
            x.Parent = y;
            RotationCount++;
        }

        protected override Color NodeColor(SearchTreeNode<TKey, TValue> node)
        {
            return node.IsRed ? Color.FromArgb(192, 57, 43) : Color.FromArgb(33, 37, 41);
        }

        /// <summary>Black nodes on any root-to-leaf path (all paths are equal in a valid tree).</summary>
        public int BlackHeight
        {
            get
            {
                int count = 0;
                for (SearchTreeNode<TKey, TValue> n = Root; n != null; n = n.Left)
                {
                    if (!n.IsRed) count++;
                }
                return count;
            }
        }

        /// <summary>Verifies all three red-black rules plus parent links and key order.</summary>
        public bool IsValidRedBlackTree()
        {
            if (Root == null) return true;
            if (Root.IsRed || Root.Parent != null) return false;
            return IsValidSearchTree() && BlackHeightOf(Root) >= 0;
        }

        // Returns the black-height of the subtree, or -1 if any rule is broken.
        private int BlackHeightOf(SearchTreeNode<TKey, TValue> node)
        {
            if (node == null) return 1;

            if (node.IsRed)
            {
                if ((node.Left != null && node.Left.IsRed) || (node.Right != null && node.Right.IsRed))
                {
                    return -1;
                }
            }
            if (node.Left != null && node.Left.Parent != node) return -1;
            if (node.Right != null && node.Right.Parent != node) return -1;

            int left = BlackHeightOf(node.Left);
            int right = BlackHeightOf(node.Right);
            if (left < 0 || right < 0 || left != right) return -1;

            return left + (node.IsRed ? 0 : 1);
        }
    }
}
