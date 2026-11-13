using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// A generic binary heap stored in an array (a "complete binary tree" in
    /// disguise: the children of index i live at 2i+1 and 2i+2).
    ///
    /// The supplied comparison decides which item must come out first: an item
    /// that compares as "smaller" is always nearer the top. Passing a
    /// comparison that puts critical requests first therefore gives a
    /// priority queue of service requests; the same class powers Prim's
    /// minimum-spanning-tree algorithm (cheapest edge first).
    ///
    ///   Insert        O(log n)    Peek          O(1)
    ///   ExtractTop    O(log n)    Build-heap    O(n)
    /// </summary>
    public class BinaryHeap<T>
    {
        private readonly List<T> _items = new List<T>();
        private readonly Comparison<T> _comparison;

        public BinaryHeap(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }
            _comparison = comparison;
        }

        /// <summary>Builds a heap from existing items in O(n) (Floyd's build-heap).</summary>
        public BinaryHeap(IEnumerable<T> items, Comparison<T> comparison) : this(comparison)
        {
            _items.AddRange(items);
            for (int i = _items.Count / 2 - 1; i >= 0; i--)
            {
                SiftDown(i);
            }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsEmpty
        {
            get { return _items.Count == 0; }
        }

        public void Insert(T item)
        {
            _items.Add(item);
            SiftUp(_items.Count - 1);
        }

        public T Peek()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("The heap is empty.");
            }
            return _items[0];
        }

        public T ExtractTop()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("The heap is empty.");
            }

            T top = _items[0];
            int last = _items.Count - 1;
            _items[0] = _items[last];
            _items.RemoveAt(last);

            if (_items.Count > 0)
            {
                SiftDown(0);
            }
            return top;
        }

        /// <summary>The items in heap (array) order - useful for drawing the heap as a tree.</summary>
        public T[] ToArray()
        {
            return _items.ToArray();
        }

        /// <summary>
        /// All items in priority order, without disturbing this heap
        /// (a heap-sort of a copy: O(n log n)).
        /// </summary>
        public List<T> ToSortedList()
        {
            var copy = new BinaryHeap<T>(_items, _comparison);
            var result = new List<T>(copy.Count);
            while (!copy.IsEmpty)
            {
                result.Add(copy.ExtractTop());
            }
            return result;
        }

        /// <summary>Checks the heap property: no child may come out before its parent.</summary>
        public bool IsValidHeap()
        {
            for (int i = 1; i < _items.Count; i++)
            {
                int parent = (i - 1) / 2;
                if (_comparison(_items[i], _items[parent]) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_comparison(_items[index], _items[parent]) >= 0)
                {
                    break;
                }
                Swap(index, parent);
                index = parent;
            }
        }

        private void SiftDown(int index)
        {
            int count = _items.Count;
            while (true)
            {
                int left = 2 * index + 1;
                int right = left + 1;
                int best = index;

                if (left < count && _comparison(_items[left], _items[best]) < 0)
                {
                    best = left;
                }
                if (right < count && _comparison(_items[right], _items[best]) < 0)
                {
                    best = right;
                }
                if (best == index)
                {
                    break;
                }
                Swap(index, best);
                index = best;
            }
        }

        private void Swap(int a, int b)
        {
            T temp = _items[a];
            _items[a] = _items[b];
            _items[b] = temp;
        }
    }
}
