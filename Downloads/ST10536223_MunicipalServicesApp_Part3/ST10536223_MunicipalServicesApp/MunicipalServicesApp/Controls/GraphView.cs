using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MunicipalServicesApp.DataStructures;

namespace MunicipalServicesApp.Controls
{
    /// <summary>
    /// Draws the ward road network: circles for wards and the depot, lines for
    /// roads labelled with their length. Edges in the minimum spanning tree are
    /// drawn thick and green, wards show how many open requests they have, and
    /// numbered tags show the order a traversal visited them in.
    /// </summary>
    public class GraphView : Control
    {
        private Graph<string> _graph;
        private Func<string, PointF> _positionOf;
        private readonly HashSet<string> _mstEdges = new HashSet<string>();
        private readonly Dictionary<string, int> _badges = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _visitOrder = new Dictionary<string, int>();

        public GraphView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Color.White;
        }

        public static string EdgeKey(string a, string b)
        {
            return string.CompareOrdinal(a, b) <= 0 ? a + "|" + b : b + "|" + a;
        }

        public void SetGraph(Graph<string> graph, Func<string, PointF> positionOf)
        {
            _graph = graph;
            _positionOf = positionOf;
            Invalidate();
        }

        public void SetMst(IEnumerable<Edge<string>> edges)
        {
            _mstEdges.Clear();
            if (edges != null)
            {
                foreach (Edge<string> e in edges) _mstEdges.Add(EdgeKey(e.From, e.To));
            }
            Invalidate();
        }

        public void SetBadges(Dictionary<string, int> badges)
        {
            _badges.Clear();
            foreach (KeyValuePair<string, int> kv in badges) _badges[kv.Key] = kv.Value;
            Invalidate();
        }

        public void SetVisitOrder(IEnumerable<string> order)
        {
            _visitOrder.Clear();
            if (order != null)
            {
                int n = 1;
                foreach (string v in order) _visitOrder[v] = n++;
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            if (_graph == null) return;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            const float margin = 38f;
            float w = Math.Max(1, Width - 2 * margin);
            float h = Math.Max(1, Height - 2 * margin);
            Func<string, PointF> screen = v =>
            {
                PointF p = _positionOf(v);
                return new PointF(margin + p.X * w, margin + p.Y * h);
            };

            using (var thin = new Pen(Color.FromArgb(200, 206, 212), 1.5f))
            using (var thick = new Pen(Color.FromArgb(39, 174, 96), 4.5f))
            using (var smallFont = new Font("Segoe UI", 7.5F))
            using (var boldFont = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            using (var grey = new SolidBrush(Color.FromArgb(120, 128, 136)))
            using (var green = new SolidBrush(Color.FromArgb(30, 130, 76)))
            using (var labelBack = new SolidBrush(Color.FromArgb(235, 255, 255, 255)))
            {
                foreach (Edge<string> edge in _graph.Edges)
                {
                    PointF a = screen(edge.From);
                    PointF b = screen(edge.To);
                    bool inMst = _mstEdges.Contains(EdgeKey(edge.From, edge.To));
                    g.DrawLine(inMst ? thick : thin, a, b);

                    string text = edge.Weight.ToString("0.0");
                    Font font = inMst ? boldFont : smallFont;
                    SizeF size = g.MeasureString(text, font);
                    float mx = (a.X + b.X) / 2f - size.Width / 2f;
                    float my = (a.Y + b.Y) / 2f - size.Height / 2f;
                    g.FillRectangle(labelBack, mx, my, size.Width, size.Height);
                    g.DrawString(text, font, inMst ? green : grey, mx, my);
                }
            }

            const float r = 19f;
            using (var nameFont = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            using (var badgeFont = new Font("Segoe UI", 8F, FontStyle.Bold))
            using (var orderFont = new Font("Segoe UI", 8F, FontStyle.Bold))
            using (var white = new SolidBrush(Color.White))
            using (var dark = new SolidBrush(Color.FromArgb(31, 45, 61)))
            using (var ring = new Pen(Color.White, 2f))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                foreach (string v in _graph.Vertices)
                {
                    PointF p = screen(v);
                    int open;
                    _badges.TryGetValue(v, out open);

                    Color fill;
                    if (v == "Depot") fill = Color.FromArgb(31, 45, 61);
                    else if (open >= 3) fill = Color.FromArgb(192, 57, 43);
                    else if (open >= 1) fill = Color.FromArgb(230, 126, 34);
                    else fill = Color.FromArgb(127, 140, 141);

                    var box = new RectangleF(p.X - r, p.Y - r, 2 * r, 2 * r);
                    using (var brush = new SolidBrush(fill))
                    {
                        g.FillEllipse(brush, box);
                        g.DrawEllipse(ring, box);
                    }

                    string shortName = v.StartsWith("Ward ") ? "W" + v.Substring(5) : v;
                    g.DrawString(shortName, nameFont, white, box, format);

                    if (open > 0 && v != "Depot")
                    {
                        var bubble = new RectangleF(p.X + r - 9, p.Y - r - 6, 17, 17);
                        g.FillEllipse(white, bubble);
                        using (var pen = new Pen(Color.FromArgb(192, 57, 43), 1.5f)) g.DrawEllipse(pen, bubble);
                        using (var red = new SolidBrush(Color.FromArgb(192, 57, 43)))
                        {
                            g.DrawString(open.ToString(), badgeFont, red, bubble, format);
                        }
                    }

                    int order;
                    if (_visitOrder.TryGetValue(v, out order))
                    {
                        var tag = new RectangleF(p.X - 11, p.Y + r - 2, 22, 15);
                        g.FillRectangle(dark, tag);
                        g.DrawString(order.ToString(), orderFont, white, tag, format);
                    }
                }
            }
        }
    }
}
