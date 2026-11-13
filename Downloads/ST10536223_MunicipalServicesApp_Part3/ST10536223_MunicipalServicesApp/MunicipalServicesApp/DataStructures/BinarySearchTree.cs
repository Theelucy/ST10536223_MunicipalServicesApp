using System;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// A plain binary search tree: smaller keys go left, larger keys go right.
    /// Search is O(height). With random keys the height is about log n, but
    /// with keys that arrive already sorted (such as request reference numbers
    /// 1, 2, 3, ...) every new key goes to the right and the tree degenerates
    /// into a linked list of height n, so search becomes O(n). That weakness
    /// is why the AVL and red-black trees exist.
    /// </summary>
    public class BinarySearchTree<TKey, TValue> : SearchTreeBase<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public override string Name
        {
            get { return "Binary search tree"; }
        }

        public override void Insert(TKey key, TValue value)
        {
            if (Root == null)
            {
                Root = new SearchTreeNode<TKey, TValue> { Key = key, Value = value };
                Count = 1;
                return;
            }

            SearchTreeNode<TKey, TValue> current = Root;
            while (true)
            {
                int cmp = key.CompareTo(current.Key);
                if (cmp == 0)
                {
                    current.Value = value;
                    return;
                }

                if (cmp < 0)
                {
                    if (current.Left == null)
                    {
                        current.Left = new SearchTreeNode<TKey, TValue> { Key = key, Value = value };
                        Count++;
                        return;
                    }
                    current = current.Left;
                }
                else
                {
                    if (current.Right == null)
                    {
                        current.Right = new SearchTreeNode<TKey, TValue> { Key = key, Value = value };
                        Count++;
                        return;
                    }
                    current = current.Right;
                }
            }
        }
    }
}
