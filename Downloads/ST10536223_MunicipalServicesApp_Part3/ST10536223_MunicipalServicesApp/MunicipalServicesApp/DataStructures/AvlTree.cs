using System;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// A self-balancing binary search tree (Adelson-Velsky and Landis). After
    /// every insert, each node's two subtrees are checked: if their heights
    /// differ by more than 1 the node is rotated back into balance. That
    /// guarantees height &lt;= 1.44 * log2(n), so search is always O(log n) -
    /// even for keys that arrive in sorted order.
    /// </summary>
    public class AvlTree<TKey, TValue> : SearchTreeBase<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public override string Name
        {
            get { return "AVL tree"; }
        }

        public override void Insert(TKey key, TValue value)
        {
            Root = Insert(Root, key, value);
        }

        private SearchTreeNode<TKey, TValue> Insert(SearchTreeNode<TKey, TValue> node, TKey key, TValue value)
        {
            if (node == null)
            {
                Count++;
                return new SearchTreeNode<TKey, TValue> { Key = key, Value = value };
            }

            int cmp = key.CompareTo(node.Key);
            if (cmp < 0)
            {
                node.Left = Insert(node.Left, key, value);
            }
            else if (cmp > 0)
            {
                node.Right = Insert(node.Right, key, value);
            }
            else
            {
                node.Value = value;
                return node;
            }

            return Rebalance(node);
        }

        private SearchTreeNode<TKey, TValue> Rebalance(SearchTreeNode<TKey, TValue> node)
        {
            UpdateHeight(node);
            int balance = HeightOf(node.Left) - HeightOf(node.Right);

            if (balance > 1)
            {
                // Left-heavy. If the left child leans right, rotate it left first (left-right case).
                if (HeightOf(node.Left.Left) < HeightOf(node.Left.Right))
                {
                    node.Left = RotateLeft(node.Left);
                }
                return RotateRight(node);
            }

            if (balance < -1)
            {
                // Right-heavy. If the right child leans left, rotate it right first (right-left case).
                if (HeightOf(node.Right.Right) < HeightOf(node.Right.Left))
                {
                    node.Right = RotateRight(node.Right);
                }
                return RotateLeft(node);
            }

            return node;
        }

        private SearchTreeNode<TKey, TValue> RotateRight(SearchTreeNode<TKey, TValue> y)
        {
            SearchTreeNode<TKey, TValue> x = y.Left;
            y.Left = x.Right;
            x.Right = y;
            UpdateHeight(y);
            UpdateHeight(x);
            RotationCount++;
            return x;
        }

        private SearchTreeNode<TKey, TValue> RotateLeft(SearchTreeNode<TKey, TValue> x)
        {
            SearchTreeNode<TKey, TValue> y = x.Right;
            x.Right = y.Left;
            y.Left = x;
            UpdateHeight(x);
            UpdateHeight(y);
            RotationCount++;
            return y;
        }

        private static int HeightOf(SearchTreeNode<TKey, TValue> node)
        {
            return node == null ? 0 : node.Height;
        }

        private static void UpdateHeight(SearchTreeNode<TKey, TValue> node)
        {
            node.Height = 1 + Math.Max(HeightOf(node.Left), HeightOf(node.Right));
        }

        /// <summary>Checks the AVL rule everywhere: stored heights are correct and no node is out of balance by more than 1.</summary>
        public bool IsBalanced()
        {
            return CheckBalance(Root);
        }

        private bool CheckBalance(SearchTreeNode<TKey, TValue> node)
        {
            if (node == null) return true;
            int left = HeightOf(node.Left);
            int right = HeightOf(node.Right);
            if (Math.Abs(left - right) > 1) return false;
            if (node.Height != 1 + Math.Max(left, right)) return false;
            return CheckBalance(node.Left) && CheckBalance(node.Right);
        }
    }
}
