using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Controls
{
    /// <summary>
    /// A four-step progress tracker: Submitted - Under Review - In Progress - Resolved.
    /// Steps already reached are filled in; the current step is ringed.
    /// </summary>
    public class StatusStepper : Control
    {
        private static readonly string[] Labels = { "Submitted", "Under Review", "In Progress", "Resolved" };
        private IssueStatus? _status;

        public StatusStepper()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Color.White;
            Height = 70;
        }

        public void SetStatus(IssueStatus? status)
        {
            _status = status;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int current = _status.HasValue ? (int)_status.Value : -1;
            const float r = 11f;
            float left = 46f;
            float right = Math.Max(left + 10, Width - 46f);
            float y = 22f;
            float step = (right - left) / 3f;

            Color reached = _status.HasValue ? IssueStatusHelper.GetColor(_status.Value) : Color.Gray;
            Color idle = Color.FromArgb(205, 210, 215);

            using (var linePending = new Pen(idle, 4f))
            using (var lineDone = new Pen(reached, 4f))
            {
                for (int i = 0; i < 3; i++)
                {
                    g.DrawLine(i < current ? lineDone : linePending, left + i * step, y, left + (i + 1) * step, y);
                }
            }

            using (var font = new Font("Segoe UI", 8.5F))
            using (var boldFont = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            using (var white = new SolidBrush(Color.White))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                for (int i = 0; i < 4; i++)
                {
                    float x = left + i * step;
                    bool done = i <= current;
                    var box = new RectangleF(x - r, y - r, 2 * r, 2 * r);

                    using (var fill = new SolidBrush(done ? reached : idle))
                    {
                        g.FillEllipse(fill, box);
                    }
                    if (i == current)
                    {
                        using (var ring = new Pen(reached, 2.5f))
                        {
                            g.DrawEllipse(ring, x - r - 4, y - r - 4, 2 * r + 8, 2 * r + 8);
                        }
                    }
                    g.DrawString(done ? "\u2713" : (i + 1).ToString(), boldFont, white, box, format);

                    using (var textBrush = new SolidBrush(i == current ? Color.FromArgb(44, 62, 80) : Color.FromArgb(130, 138, 146)))
                    {
                        var labelBox = new RectangleF(x - step / 2f, y + r + 8, step, 18);
                        g.DrawString(Labels[i], i == current ? boldFont : font, textBrush, labelBox, format);
                    }
                }
            }
        }
    }
}
