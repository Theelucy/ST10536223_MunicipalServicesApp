using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MunicipalServicesApp.DataStructures;

namespace MunicipalServicesApp.Controls
{
    /// <summary>
    /// Draws any binary tree described by a list of <see cref="TreeDrawNode"/>s.
    /// A node's column is its in-order position and its row is its depth, so
    /// the picture shows the real shape of the tree - a tall thin chain for an
    /// unbalanced search tree, a short bushy tree for a balanced one.
    /// </summary>
    public class BinaryTreeView : Control
    {
        private List<TreeDrawNode> _nodes = new List<TreeDrawNode>();
        private string _emptyMessage = "(no data)";

        public BinaryTreeView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Color.White;
        }

        public string EmptyMessage
        {
            get { return _emptyMessage; }
            set { _emptyMessage = value; Invalidate(); }
        }

        public void SetNodes(List<TreeDrawNode> nodes)
        {
            _nodes = nodes ?? new List<TreeDrawNode>();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_nodes.Count == 0)
            {
                using (var font = new Font("Segoe UI", 9.5F, FontStyle.Italic))
                using (var brush = new SolidBrush(Color.Gray))
                using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(_emptyMessage, font, brush, ClientRectangle, format);
                }
                return;
            }

            int columns = 0;
            int maxDepth = 0;
            foreach (TreeDrawNode n in _nodes)
            {
                columns = Math.Max(columns, n.Order + 1);
                maxDepth = Math.Max(maxDepth, n.Depth);
            }

            const float margin = 12f;
            float innerW = Math.Max(1, Width - 2 * margin);
            float innerH = Math.Max(1, Height - 2 * margin);
            float colW = innerW / columns;
            float rowH = maxDepth == 0 ? 0 : Math.Min(innerH / (maxDepth + 0.6f), 64f);

            float radius = Math.Min(colW * 0.5f, 15f);
            if (maxDepth > 0) radius = Math.Min(radius, rowH * 0.48f);
            radius = Math.Max(radius, 4.5f);

            float fontSize = Math.Max(5.5f, Math.Min(9f, radius * 0.9f));
            var points = new PointF[_nodes.Count];
            for (int i = 0; i < _nodes.Count; i++)
            {
                float x = margin + (_nodes[i].Order + 0.5f) * colW;
                float y = margin + radius + _nodes[i].Depth * rowH;
                points[i] = new PointF(x, y);
            }

            // Edges first so the nodes sit on top of them.
            using (var normal = new Pen(Color.FromArgb(150, 160, 170), 1.4f))
            using (var hot = new Pen(Color.FromArgb(243, 156, 18), 3f))
            using (var edgeFont = new Font("Segoe UI", 8F, FontStyle.Italic))
            using (var edgeBrush = new SolidBrush(Color.FromArgb(90, 100, 110)))
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    int parent = _nodes[i].ParentIndex;
                    if (parent < 0) continue;

                    bool onPath = _nodes[i].Highlight && _nodes[parent].Highlight;
                    g.DrawLine(onPath ? hot : normal, points[parent], points[i]);

                    if (!string.IsNullOrEmpty(_nodes[i].EdgeLabel))
                    {
                        float mx = (points[parent].X + points[i].X) / 2f;
                        float my = (points[parent].Y + points[i].Y) / 2f;
                        g.DrawString(_nodes[i].EdgeLabel, edgeFont, edgeBrush, mx + 3, my - 9);
                    }
                }
            }

            using (var font = new Font("Segoe UI", fontSize, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.White))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var ring = new Pen(Color.FromArgb(243, 156, 18), 3.2f))
            using (var outline = new Pen(Color.White, 1.2f))
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    TreeDrawNode n = _nodes[i];
                    SizeF textSize = g.MeasureString(n.Label ?? "", font);
                    float halfW = Math.Max(radius, textSize.Width / 2f + 5f);
                    var box = new RectangleF(points[i].X - halfW, points[i].Y - radius, halfW * 2, radius * 2);

                    using (var fill = new SolidBrush(n.Fill))
                    using (GraphicsPath path = RoundedRect(box, radius))
                    {
                        g.FillPath(fill, path);
                        g.DrawPath(n.Highlight ? ring : outline, path);
                    }

                    if (radius >= 6.5f || halfW > radius)
                    {
                        g.DrawString(n.Label ?? "", font, textBrush, box, format);
                    }
                }
            }
        }

        private static GraphicsPath RoundedRect(RectangleF r, float radius)
        {
            float d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
