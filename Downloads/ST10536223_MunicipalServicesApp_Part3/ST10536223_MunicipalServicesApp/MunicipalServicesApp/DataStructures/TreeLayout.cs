using System;
using System.Collections.Generic;
using System.Drawing;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// One node of a tree, ready to be drawn: where it sits (in-order column x
    /// depth row), its text, its colour and which node is its parent.
    /// </summary>
    public class TreeDrawNode
    {
        public string Label { get; set; }
        public int Depth { get; set; }
        public int Order { get; set; }          // in-order position -> horizontal column
        public int ParentIndex { get; set; }    // index in the layout list, -1 for the root
        public Color Fill { get; set; }
        public bool Highlight { get; set; }
        public string EdgeLabel { get; set; }   // text drawn on the edge from the parent
    }

    /// <summary>
    /// Turns any binary tree into a flat list of <see cref="TreeDrawNode"/>s.
    /// Horizontal position is the node's in-order rank, vertical position is its
    /// depth - the simplest layout that never makes two nodes overlap.
    /// </summary>
    public static class TreeLayout
    {
        public static List<TreeDrawNode> Build<TNode>(
            TNode root,
            Func<TNode, TNode> getLeft,
            Func<TNode, TNode> getRight,
            Func<TNode, string> label,
            Func<TNode, Color> fill,
            Func<TNode, bool> highlight,
            string leftEdgeLabel,
            string rightEdgeLabel) where TNode : class
        {
            var list = new List<TreeDrawNode>();
            var counter = new int[1];
            if (root != null)
            {
                Visit(root, -1, 0, null, list, counter, getLeft, getRight, label, fill, highlight,
                    leftEdgeLabel, rightEdgeLabel);
            }
            return list;
        }

        private static void Visit<TNode>(
            TNode node, int parentIndex, int depth, string edgeLabel,
            List<TreeDrawNode> list, int[] counter,
            Func<TNode, TNode> getLeft, Func<TNode, TNode> getRight,
            Func<TNode, string> label, Func<TNode, Color> fill, Func<TNode, bool> highlight,
            string leftEdgeLabel, string rightEdgeLabel) where TNode : class
        {
            var draw = new TreeDrawNode
            {
                Label = label(node),
                Depth = depth,
                ParentIndex = parentIndex,
                Fill = fill(node),
                Highlight = highlight != null && highlight(node),
                EdgeLabel = edgeLabel
            };
            int myIndex = list.Count;
            list.Add(draw);

            TNode left = getLeft(node);
            TNode right = getRight(node);

            if (left != null)
            {
                Visit(left, myIndex, depth + 1, leftEdgeLabel, list, counter, getLeft, getRight,
                    label, fill, highlight, leftEdgeLabel, rightEdgeLabel);
            }

            draw.Order = counter[0]++;

            if (right != null)
            {
                Visit(right, myIndex, depth + 1, rightEdgeLabel, list, counter, getLeft, getRight,
                    label, fill, highlight, leftEdgeLabel, rightEdgeLabel);
            }
        }
    }
}
