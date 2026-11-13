using System;
using System.Collections.Generic;
using System.Drawing;

namespace MunicipalServicesApp.DataStructures
{
    public class BinaryTreeNode<T>
    {
        public BinaryTreeNode(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
        public BinaryTreeNode<T> Left { get; set; }
        public BinaryTreeNode<T> Right { get; set; }

        public bool IsLeaf
        {
            get { return Left == null && Right == null; }
        }
    }

    /// <summary>
    /// A plain binary tree: every node has at most two children (left / right)
    /// and there is NO ordering rule between them. In the Service Request
    /// Status feature it is the triage decision tree - each internal node asks
    /// a yes/no question (left = yes, right = no) and each leaf is a priority.
    ///
    /// The four classic traversals are provided; each visits every node once, O(n).
    /// </summary>
    public class BinaryTree<T>
    {
        public BinaryTree(BinaryTreeNode<T> root)
        {
            Root = root;
        }

        public BinaryTreeNode<T> Root { get; private set; }

        /// <summary>Node, then left subtree, then right subtree.</summary>
        public List<T> PreOrder()
        {
            var result = new List<T>();
            var stack = new Stack<BinaryTreeNode<T>>();
            if (Root != null) stack.Push(Root);
            while (stack.Count > 0)
            {
                BinaryTreeNode<T> n = stack.Pop();
                result.Add(n.Value);
                if (n.Right != null) stack.Push(n.Right);
                if (n.Left != null) stack.Push(n.Left);
            }
            return result;
        }

        /// <summary>Left subtree, then node, then right subtree.</summary>
        public List<T> InOrder()
        {
            var result = new List<T>();
            var stack = new Stack<BinaryTreeNode<T>>();
            BinaryTreeNode<T> current = Root;
            while (current != null || stack.Count > 0)
            {
                while (current != null)
                {
                    stack.Push(current);
                    current = current.Left;
                }
                current = stack.Pop();
                result.Add(current.Value);
                current = current.Right;
            }
            return result;
        }

        /// <summary>Left subtree, then right subtree, then node.</summary>
        public List<T> PostOrder()
        {
            var result = new List<T>();
            var stack = new Stack<BinaryTreeNode<T>>();
            BinaryTreeNode<T> last = null;
            BinaryTreeNode<T> current = Root;
            while (current != null || stack.Count > 0)
            {
                if (current != null)
                {
                    stack.Push(current);
                    current = current.Left;
                }
                else
                {
                    BinaryTreeNode<T> peek = stack.Peek();
                    if (peek.Right != null && last != peek.Right)
                    {
                        current = peek.Right;
                    }
                    else
                    {
                        result.Add(peek.Value);
                        last = stack.Pop();
                    }
                }
            }
            return result;
        }

        /// <summary>Level by level, left to right (uses a queue).</summary>
        public List<T> LevelOrder()
        {
            var result = new List<T>();
            var queue = new Queue<BinaryTreeNode<T>>();
            if (Root != null) queue.Enqueue(Root);
            while (queue.Count > 0)
            {
                BinaryTreeNode<T> n = queue.Dequeue();
                result.Add(n.Value);
                if (n.Left != null) queue.Enqueue(n.Left);
                if (n.Right != null) queue.Enqueue(n.Right);
            }
            return result;
        }

        public int Count
        {
            get { return PreOrder().Count; }
        }

        public int LeafCount
        {
            get
            {
                int leaves = 0;
                var stack = new Stack<BinaryTreeNode<T>>();
                if (Root != null) stack.Push(Root);
                while (stack.Count > 0)
                {
                    BinaryTreeNode<T> n = stack.Pop();
                    if (n.IsLeaf) leaves++;
                    if (n.Left != null) stack.Push(n.Left);
                    if (n.Right != null) stack.Push(n.Right);
                }
                return leaves;
            }
        }

        /// <summary>Number of levels on the longest root-to-leaf path (empty tree = 0).</summary>
        public int Height
        {
            get
            {
                int height = 0;
                var level = new List<BinaryTreeNode<T>>();
                if (Root != null) level.Add(Root);
                while (level.Count > 0)
                {
                    height++;
                    var next = new List<BinaryTreeNode<T>>();
                    foreach (BinaryTreeNode<T> n in level)
                    {
                        if (n.Left != null) next.Add(n.Left);
                        if (n.Right != null) next.Add(n.Right);
                    }
                    level = next;
                }
                return height;
            }
        }

        public List<TreeDrawNode> GetLayout(Func<T, string> label, Func<T, Color> fill,
            Func<T, bool> highlight, string leftEdgeLabel, string rightEdgeLabel)
        {
            return TreeLayout.Build<BinaryTreeNode<T>>(
                Root,
                n => n.Left,
                n => n.Right,
                n => label(n.Value),
                n => fill(n.Value),
                n => highlight != null && highlight(n.Value),
                leftEdgeLabel,
                rightEdgeLabel);
        }
    }
}
