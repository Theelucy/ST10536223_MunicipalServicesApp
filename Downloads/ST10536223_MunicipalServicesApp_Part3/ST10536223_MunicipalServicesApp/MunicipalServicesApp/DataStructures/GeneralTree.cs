using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>A node in a general (n-ary) tree: any number of children.</summary>
    public class TreeNode<T>
    {
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

        public TreeNode(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
        public TreeNode<T> Parent { get; private set; }

        public List<TreeNode<T>> Children
        {
            get { return _children; }
        }

        public bool IsLeaf
        {
            get { return _children.Count == 0; }
        }

        public TreeNode<T> AddChild(T value)
        {
            var child = new TreeNode<T>(value);
            child.Parent = this;
            _children.Add(child);
            return child;
        }

        public int Depth
        {
            get
            {
                int depth = 0;
                for (TreeNode<T> n = Parent; n != null; n = n.Parent)
                {
                    depth++;
                }
                return depth;
            }
        }
    }

    /// <summary>
    /// A basic (general) tree used for hierarchies. In the Service Request
    /// Status feature it models the municipality's structure:
    /// Municipality -> Ward -> Category -> individual requests.
    /// </summary>
    public class GeneralTree<T>
    {
        private readonly TreeNode<T> _root;

        public GeneralTree(T rootValue)
        {
            _root = new TreeNode<T>(rootValue);
        }

        public TreeNode<T> Root
        {
            get { return _root; }
        }

        /// <summary>Depth-first (pre-order): a node, then each child's whole subtree in turn.</summary>
        public List<TreeNode<T>> DepthFirst()
        {
            var result = new List<TreeNode<T>>();
            var stack = new Stack<TreeNode<T>>();
            stack.Push(_root);
            while (stack.Count > 0)
            {
                TreeNode<T> node = stack.Pop();
                result.Add(node);
                for (int i = node.Children.Count - 1; i >= 0; i--)
                {
                    stack.Push(node.Children[i]); // reversed so the first child is visited first
                }
            }
            return result;
        }

        /// <summary>Breadth-first (level order): the root, then all depth-1 nodes, then depth-2...</summary>
        public List<TreeNode<T>> BreadthFirst()
        {
            var result = new List<TreeNode<T>>();
            var queue = new Queue<TreeNode<T>>();
            queue.Enqueue(_root);
            while (queue.Count > 0)
            {
                TreeNode<T> node = queue.Dequeue();
                result.Add(node);
                foreach (TreeNode<T> child in node.Children)
                {
                    queue.Enqueue(child);
                }
            }
            return result;
        }

        public int Count
        {
            get { return DepthFirst().Count; }
        }

        public int LeafCount
        {
            get
            {
                int leaves = 0;
                foreach (TreeNode<T> n in DepthFirst())
                {
                    if (n.IsLeaf) leaves++;
                }
                return leaves;
            }
        }

        /// <summary>Number of levels (a lone root has height 1).</summary>
        public int Height
        {
            get
            {
                int height = 0;
                foreach (TreeNode<T> n in DepthFirst())
                {
                    height = Math.Max(height, n.Depth + 1);
                }
                return height;
            }
        }

        public TreeNode<T> Find(Func<T, bool> predicate)
        {
            foreach (TreeNode<T> n in DepthFirst())
            {
                if (predicate(n.Value)) return n;
            }
            return null;
        }

        public List<T> PathFromRoot(TreeNode<T> node)
        {
            var path = new List<T>();
            for (TreeNode<T> n = node; n != null; n = n.Parent)
            {
                path.Insert(0, n.Value);
            }
            return path;
        }
    }
}
