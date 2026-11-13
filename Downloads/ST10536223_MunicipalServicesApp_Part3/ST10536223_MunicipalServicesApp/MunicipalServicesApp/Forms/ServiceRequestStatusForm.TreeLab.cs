using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Controls;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    // Tab 5 - Tree Lab: the same reference numbers stored in a plain binary search tree, an AVL tree
    // and a red-black tree, drawn side by side. Reference numbers arrive in order (1, 2, 3, ...), which is
    // the worst case for a plain BST - so the picture shows exactly why self-balancing trees matter.
    public partial class ServiceRequestStatusForm
    {
        private BinaryTreeView _bstView, _avlView, _rbView;
        private Label _lblBstStats, _lblAvlStats, _lblRbStats, _lblLabNote, _lblLabMessage;
        private TextBox _txtLabId;
        private int? _labSearchId;

        private TabPage BuildTreeLabTab()
        {
            var page = new TabPage("Tree Lab") { BackColor = AppTheme.Background, Padding = new Padding(10) };

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = Color.White };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));

            var bar = new Panel { Dock = DockStyle.Fill };
            bar.Controls.Add(MakeLabel("Search all three trees for reference #", new Font("Segoe UI", 9.5F, FontStyle.Bold), AppTheme.PrimaryDark, 12, 15, 250, 20));
            _txtLabId = new TextBox { Font = AppTheme.FontBody, Location = new Point(262, 11), Size = new Size(80, 26) };
            _txtLabId.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; RunLabSearch(); } };
            var go = MakeButton("Search", AppTheme.Primary, 90);
            go.Height = 28; go.Location = new Point(350, 10);
            go.Click += (s, e) => RunLabSearch();
            var clear = MakeButton("Clear", AppTheme.PrimaryDark, 70);
            clear.Height = 28; clear.Location = new Point(448, 10);
            clear.Click += (s, e) => { _labSearchId = null; _txtLabId.Clear(); _lblLabMessage.Text = ""; RefreshTreeLabTab(); };
            _lblLabMessage = MakeLabel("", new Font("Segoe UI", 9F, FontStyle.Italic), AppTheme.TextMuted, 530, 15, 620, 20);
            bar.Controls.AddRange(new Control[] { _txtLabId, go, clear, _lblLabMessage });

            var trees = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            for (int i = 0; i < 3; i++) trees.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            trees.Controls.Add(MakeTreeColumn("Binary search tree", "No balancing", out _bstView, out _lblBstStats), 0, 0);
            trees.Controls.Add(MakeTreeColumn("AVL tree", "Heights of subtrees never differ by more than 1", out _avlView, out _lblAvlStats), 1, 0);
            trees.Controls.Add(MakeTreeColumn("Red-black tree", "Colour rules keep it within 2x the ideal height", out _rbView, out _lblRbStats), 2, 0);

            _lblLabNote = new Label { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(60, 70, 80), Padding = new Padding(12, 6, 12, 4) };

            root.Controls.Add(bar, 0, 0);
            root.Controls.Add(trees, 0, 1);
            root.Controls.Add(_lblLabNote, 0, 2);
            page.Controls.Add(root);
            return page;
        }

        private static Panel MakeTreeColumn(string title, string subtitle, out BinaryTreeView view, out Label stats)
        {
            var panel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(4), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            view = new BinaryTreeView { Dock = DockStyle.Fill, EmptyMessage = "Load sample requests to see the tree." };
            stats = new Label { Dock = DockStyle.Bottom, Height = 58, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(60, 70, 80), BackColor = Color.FromArgb(247, 249, 250), Padding = new Padding(8, 5, 8, 3) };
            var header = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.FromArgb(247, 249, 250) };
            header.Controls.Add(MakeLabel(title, new Font("Segoe UI", 10.5F, FontStyle.Bold), AppTheme.PrimaryDark, 8, 4, 300, 20));
            header.Controls.Add(MakeLabel(subtitle, new Font("Segoe UI", 8F, FontStyle.Italic), AppTheme.TextMuted, 8, 25, 300, 16));

            // Last added is docked first: header on top, stats on the bottom, tree fills the middle.
            panel.Controls.Add(view);
            panel.Controls.Add(stats);
            panel.Controls.Add(header);
            return panel;
        }

        private void RunLabSearch()
        {
            string digits = new string(_txtLabId.Text.Where(char.IsDigit).ToArray());
            int id;
            if (digits.Length == 0 || digits.Length > 9 || !int.TryParse(digits, out id))
            {
                _lblLabMessage.Text = "Enter a reference number such as 12.";
                _lblLabMessage.ForeColor = Color.FromArgb(192, 57, 43);
                return;
            }

            _labSearchId = id;
            LookupResult r = _service.Lookup(id);
            _lblLabMessage.ForeColor = r.Found ? AppTheme.Success : Color.FromArgb(192, 57, 43);
            _lblLabMessage.Text = r.Found
                ? "Found #" + id + ".  Comparisons - BST: " + r.BstComparisons + ", AVL: " + r.AvlComparisons + ", red-black: " + r.RbComparisons + ".  (Orange = the path each search took.)"
                : "No request #" + id + ". Orange shows where each search ended.";
            RefreshTreeLabTab();
        }

        private void RefreshTreeLabTab()
        {
            BinarySearchTree<int, RequestInfo> bst = _service.BstById;
            AvlTree<int, RequestInfo> avl = _service.AvlById;
            RedBlackTree<int, RequestInfo> rb = _service.RbById;

            Func<SearchTreeBase<int, RequestInfo>, List<int>> path =
                t => _labSearchId.HasValue ? t.SearchPath(_labSearchId.Value) : new List<int>();

            _bstView.SetNodes(bst.GetLayout(k => k.ToString(), path(bst)));
            _avlView.SetNodes(avl.GetLayout(k => k.ToString(), path(avl)));
            _rbView.SetNodes(rb.GetLayout(k => k.ToString(), path(rb)));

            _lblBstStats.Text = Stats(bst, "Search order is valid: " + (bst.IsValidSearchTree() ? "yes" : "NO"));
            _lblAvlStats.Text = Stats(avl, "Balanced everywhere: " + (avl.IsBalanced() ? "yes" : "NO"));
            _lblRbStats.Text = Stats(rb, "Red-black rules hold: " + (rb.IsValidRedBlackTree() ? "yes" : "NO") + "   |   black-height " + rb.BlackHeight);

            int n = _service.TotalCount;
            if (n == 0)
            {
                _lblLabNote.Text = "No requests yet. Click \"Load Sample Requests\" to build the three trees from real data.";
                return;
            }

            _lblLabNote.Text =
                "Reference numbers arrive in order (1, 2, 3 ...), so every new key lands to the right of the previous one. The plain binary search tree " +
                "therefore becomes a chain " + bst.Height + " levels deep for " + n + " requests, and finding the newest one takes " + n + " comparisons. " +
                "The AVL tree needs only " + avl.Height + " levels and the red-black tree " + rb.Height + " (the smallest possible is " + avl.IdealHeight + ").";
        }

        private static string Stats(SearchTreeBase<int, RequestInfo> tree, string validity)
        {
            return tree.Count + " nodes   |   height " + tree.Height + " (smallest possible " + tree.IdealHeight + ")   |   rotations " + tree.RotationCount + "\n" + validity;
        }
    }
}
