using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    // Tab 3 - Areas: a general (n-ary) tree. Municipality -> Ward -> Category -> Request.
    // Every level groups the one below it, so "everything in Ward 5" or "all water problems in Ward 5"
    // is one branch of the tree instead of a scan of the whole request list.
    public partial class ServiceRequestStatusForm
    {
        private TreeView _tvAreas;
        private Label _lblAreaStats;
        private TextBox _txtDfs, _txtBfs;
        private RichTextBox _rtbAreaDetail;

        private TabPage BuildAreasTab()
        {
            var page = new TabPage("Areas") { BackColor = AppTheme.Background, Padding = new Padding(10) };

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));

            // ---------------- left: the tree ----------------
            var left = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 0), Size = new Size(635, 505) };
            _tvAreas = new TreeView
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle,
                ItemHeight = 24,
                ShowLines = true,
                HideSelection = false
            };
            _tvAreas.AfterSelect += TvAreas_AfterSelect;

            var bar = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.White };
            var expand = MakeButton("Expand all", AppTheme.Primary, 110);
            expand.Height = 30; expand.Location = new Point(10, 8);
            expand.Click += (s, e) => _tvAreas.ExpandAll();
            var collapse = MakeButton("Collapse all", AppTheme.PrimaryDark, 110);
            collapse.Height = 30; collapse.Location = new Point(128, 8);
            collapse.Click += (s, e) => { _tvAreas.CollapseAll(); if (_tvAreas.Nodes.Count > 0) _tvAreas.Nodes[0].Expand(); };
            bar.Controls.AddRange(new Control[] { expand, collapse, MakeLabel("Click a ward, category or request", new Font("Segoe UI", 9F, FontStyle.Italic), AppTheme.TextMuted, 252, 14, 250, 20) });

            left.Controls.Add(_tvAreas);
            left.Controls.Add(bar);

            // ---------------- right: facts and traversals ----------------
            var right = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Size = new Size(520, 505) };
            const AnchorStyles wide = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            var h = new Font("Segoe UI", 10F, FontStyle.Bold);
            var small = new Font("Segoe UI", 9F);

            right.Controls.Add(MakeLabel("About this tree", new Font("Segoe UI", 11F, FontStyle.Bold), AppTheme.PrimaryDark, 14, 10, 400, 22));
            _lblAreaStats = new Label { Location = new Point(14, 34), Size = new Size(490, 38), Anchor = wide, Font = small, ForeColor = Color.FromArgb(60, 70, 80) };

            var dfsHeader = MakeLabel("Depth-first order (finishes one branch before the next)", h, AppTheme.PrimaryDark, 14, 78, 430, 20);
            dfsHeader.Anchor = wide;
            _txtDfs = new TextBox { Location = new Point(14, 100), Size = new Size(490, 50), Anchor = wide, Multiline = true, ReadOnly = true, Font = small, BackColor = Color.FromArgb(247, 249, 250), BorderStyle = BorderStyle.None };
            var bfsHeader = MakeLabel("Breadth-first order (one level at a time)", h, AppTheme.PrimaryDark, 14, 158, 430, 20);
            bfsHeader.Anchor = wide;
            _txtBfs = new TextBox { Location = new Point(14, 180), Size = new Size(490, 50), Anchor = wide, Multiline = true, ReadOnly = true, Font = small, BackColor = Color.FromArgb(247, 249, 250), BorderStyle = BorderStyle.None };

            var detailHeader = MakeLabel("Selected item", h, AppTheme.PrimaryDark, 14, 242, 430, 20);
            detailHeader.Anchor = wide;
            _rtbAreaDetail = new RichTextBox
            {
                Location = new Point(14, 264),
                Size = new Size(490, 235),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(247, 249, 250),
                Font = small
            };

            right.Controls.AddRange(new Control[] { _lblAreaStats, dfsHeader, _txtDfs, bfsHeader, _txtBfs, detailHeader, _rtbAreaDetail });

            root.Controls.Add(left, 0, 0);
            root.Controls.Add(right, 1, 0);
            page.Controls.Add(root);
            return page;
        }

        private void RefreshAreasTab()
        {
            GeneralTree<AreaNode> tree = _service.AreaTree;

            _tvAreas.BeginUpdate();
            _tvAreas.Nodes.Clear();
            _tvAreas.Nodes.Add(ToViewNode(tree.Root));
            _tvAreas.Nodes[0].Expand();
            foreach (System.Windows.Forms.TreeNode ward in _tvAreas.Nodes[0].Nodes) ward.Expand();
            _tvAreas.EndUpdate();

            _lblAreaStats.Text = tree.Count + " nodes, " + tree.LeafCount + " leaves, " + tree.Height +
                                 " levels deep. Each request is a leaf; the levels above group it by category and ward.";

            _txtDfs.Text = string.Join("  >  ", tree.DepthFirst().Take(14).Select(n => ShortText(n.Value))) + "  ...";
            _txtBfs.Text = string.Join("  >  ", tree.BreadthFirst().Take(14).Select(n => ShortText(n.Value))) + "  ...";
            _rtbAreaDetail.Text = _service.TotalCount == 0 ? "No requests yet." : "Select a node in the tree.";
        }

        private static System.Windows.Forms.TreeNode ToViewNode(DataStructures.TreeNode<AreaNode> node)
        {
            var view = new System.Windows.Forms.TreeNode(node.Value.Text) { Tag = node.Value };

            if (node.Value.Kind == AreaNodeKind.Ward || node.Value.Kind == AreaNodeKind.Municipality)
            {
                view.NodeFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            }
            if (node.Value.Kind == AreaNodeKind.Request)
            {
                view.ForeColor = IssueStatusHelper.GetColor(node.Value.Request.Issue.Status);
            }

            foreach (DataStructures.TreeNode<AreaNode> child in node.Children)
            {
                view.Nodes.Add(ToViewNode(child));
            }
            return view;
        }

        private static string ShortText(AreaNode n)
        {
            if (n.Kind == AreaNodeKind.Request) return "#" + n.Request.Id;
            int paren = n.Text.IndexOf(" (", StringComparison.Ordinal);
            return paren > 0 ? n.Text.Substring(0, paren) : n.Text;
        }

        private void TvAreas_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node.Tag as AreaNode;
            if (node == null) return;

            _rtbAreaDetail.Clear();
            if (node.Kind == AreaNodeKind.Request)
            {
                RequestInfo r = node.Request;
                AppendRich(_rtbAreaDetail, "Request #" + r.Id + "  -  " + r.Issue.Category + "\n", true);
                AppendRich(_rtbAreaDetail, r.Area + "   |   " + IssueStatusHelper.GetShortText(r.Issue.Status) + "   |   Priority: " + r.PriorityLabel + "\n\n", false);
                AppendRich(_rtbAreaDetail, r.Issue.Description + "\n\n", false);
                AppendRich(_rtbAreaDetail, "Path from the root: " + string.Join(" > ",
                    _service.AreaTree.PathFromRoot(FindNode(node)).Select(ShortText)), false);
                return;
            }

            // A ward / category: summarise everything below it.
            var below = new List<RequestInfo>();
            CollectRequests(e.Node, below);
            AppendRich(_rtbAreaDetail, e.Node.Text + "\n", true);
            AppendRich(_rtbAreaDetail, Plural(below.Count, "request", "requests") + " in this branch, " +
                                       below.Count(r => r.Issue.Status != IssueStatus.Resolved) + " still open.\n", false);
            foreach (IGrouping<int, RequestInfo> g in below.GroupBy(r => r.Priority).OrderBy(grp => grp.Key))
            {
                AppendRich(_rtbAreaDetail, "  " + TriageTree.PriorityNames[g.Key] + ": " + g.Count() + "\n", false);
            }
        }

        private DataStructures.TreeNode<AreaNode> FindNode(AreaNode value)
        {
            return _service.AreaTree.DepthFirst().First(n => n.Value == value);
        }

        private static void CollectRequests(System.Windows.Forms.TreeNode node, List<RequestInfo> into)
        {
            var value = node.Tag as AreaNode;
            if (value != null && value.Kind == AreaNodeKind.Request) into.Add(value.Request);
            foreach (System.Windows.Forms.TreeNode child in node.Nodes) CollectRequests(child, into);
        }
    }
}
