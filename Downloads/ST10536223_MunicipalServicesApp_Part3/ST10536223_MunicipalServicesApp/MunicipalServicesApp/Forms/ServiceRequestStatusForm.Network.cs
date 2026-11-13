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
    // Tab 6 - Area Network: a weighted graph of the wards and the depot (edge weight = km).
    //   Graph traversal ........ breadth-first (fewest roads from the depot) and depth-first (a possible inspection route)
    //   Minimum spanning tree .. the cheapest set of roads that still connects every ward (Kruskal and Prim)
    //   Connected components ... clusters of related requests
    public partial class ServiceRequestStatusForm
    {
        private GraphView _graphView;
        private ComboBox _cmbMst, _cmbTraversal, _cmbStart;
        private Label _lblMstSummary;
        private ListBox _lstMst, _lstTraversal, _lstClusters;
        private bool _loadingNetwork;

        private TabPage BuildNetworkTab()
        {
            var page = new TabPage("Area Network") { BackColor = AppTheme.Background, Padding = new Padding(10) };

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 372F));

            // ---------------- left: the picture ----------------
            var left = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0, 0, 10, 0) };
            _graphView = new GraphView { Dock = DockStyle.Fill };
            var legend = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(60, 70, 80),
                BackColor = Color.FromArgb(247, 249, 250),
                Padding = new Padding(10, 5, 10, 3),
                Text = "Numbers on the roads are kilometres. Ward colour: grey = no open requests, orange = 1 to 2, red = 3 or more; the white badge is the open-request count. " +
                       "Thick green roads form the minimum spanning tree; dark tags show the order a traversal visited each ward."
            };
            left.Controls.Add(_graphView);
            left.Controls.Add(legend);

            // ---------------- right: controls ----------------
            var right = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Size = new Size(372, 505) };
            const AnchorStyles wide = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            var h = new Font("Segoe UI", 10F, FontStyle.Bold);

            right.Controls.Add(MakeLabel("Minimum spanning tree", h, AppTheme.PrimaryDark, 12, 8, 330, 22));
            _cmbMst = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontBody, Location = new Point(12, 32), Size = new Size(340, 26), Anchor = wide };
            _cmbMst.Items.AddRange(new object[] { "Hide", "Kruskal's algorithm (sort roads, skip loops)", "Prim's algorithm (grow from the depot)" });
            _cmbMst.SelectedIndex = 1;
            _cmbMst.SelectedIndexChanged += (s, e) => { if (!_loadingNetwork) UpdateMst(); };
            _lblMstSummary = new Label { Location = new Point(12, 62), Size = new Size(340, 62), Anchor = wide, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(60, 70, 80) };
            _lstMst = new ListBox { Location = new Point(12, 128), Size = new Size(340, 108), Anchor = wide, Font = new Font("Segoe UI", 9F), BorderStyle = BorderStyle.FixedSingle };

            right.Controls.Add(MakeLabel("Traversal", h, AppTheme.PrimaryDark, 12, 244, 330, 22));
            _cmbTraversal = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontBody, Location = new Point(12, 268), Size = new Size(168, 26) };
            _cmbTraversal.Items.AddRange(new object[] { "None", "Breadth-first", "Depth-first" });
            _cmbTraversal.SelectedIndex = 1;
            _cmbTraversal.SelectedIndexChanged += (s, e) => { if (!_loadingNetwork) UpdateTraversal(); };
            _cmbStart = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontBody, Location = new Point(188, 268), Size = new Size(164, 26), Anchor = wide };
            foreach (string v in AreaNetwork.Graph.Vertices) _cmbStart.Items.Add(v);
            _cmbStart.SelectedItem = AreaNetwork.Depot;
            _cmbStart.SelectedIndexChanged += (s, e) => { if (!_loadingNetwork) UpdateTraversal(); };
            _lstTraversal = new ListBox { Location = new Point(12, 298), Size = new Size(340, 100), Anchor = wide, Font = new Font("Segoe UI", 9F), BorderStyle = BorderStyle.FixedSingle };

            right.Controls.Add(MakeLabel("Incident clusters (possible shared faults)", new Font("Segoe UI", 9.5F, FontStyle.Bold), AppTheme.PrimaryDark, 12, 406, 345, 20));
            _lstClusters = new ListBox
            {
                Location = new Point(12, 430),
                Size = new Size(340, 68),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 8.5F),
                BorderStyle = BorderStyle.FixedSingle,
                HorizontalScrollbar = true
            };

            right.Controls.AddRange(new Control[] { _cmbMst, _lblMstSummary, _lstMst, _cmbTraversal, _cmbStart, _lstTraversal, _lstClusters });

            root.Controls.Add(left, 0, 0);
            root.Controls.Add(right, 1, 0);
            page.Controls.Add(root);
            return page;
        }

        private void RefreshNetworkTab()
        {
            _loadingNetwork = true;

            Graph<string> network = AreaNetwork.Graph;
            _graphView.SetGraph(network, AreaNetwork.PositionOf);

            var badges = new Dictionary<string, int>();
            foreach (string v in network.Vertices)
            {
                if (v != AreaNetwork.Depot) badges[v] = _service.OpenCountInArea(v);
            }
            _graphView.SetBadges(badges);

            _lstClusters.Items.Clear();
            List<IncidentCluster> clusters = _service.IncidentClusters();
            if (clusters.Count == 0)
            {
                _lstClusters.Items.Add("No linked requests yet.");
            }
            foreach (IncidentCluster c in clusters)
            {
                _lstClusters.Items.Add(c.Category + ": " + Plural(c.Requests.Count, "request", "requests") + " in " + string.Join(", ", c.Areas));
                _lstClusters.Items.Add("     " + string.Join(", ", c.Requests.Select(r => "#" + r.Id)));
            }

            _loadingNetwork = false;
            UpdateMst();
            UpdateTraversal();
        }

        private void UpdateMst()
        {
            Graph<string> network = AreaNetwork.Graph;
            MstResult<string> kruskal = network.MinimumSpanningTreeKruskal();
            MstResult<string> prim = network.MinimumSpanningTreePrim(AreaNetwork.Depot);
            double all = network.Edges.Sum(e => e.Weight);

            _lstMst.Items.Clear();
            if (_cmbMst.SelectedIndex == 0)
            {
                _graphView.SetMst(null);
                _lblMstSummary.Text = "The network has " + network.EdgeCount + " roads totalling " + all.ToString("0.0") + " km. " +
                                      "Choose an algorithm to see the cheapest set of roads that still reaches every ward.";
                return;
            }

            MstResult<string> shown = _cmbMst.SelectedIndex == 1 ? kruskal : prim;
            _graphView.SetMst(shown.Edges);

            double saved = (1 - shown.TotalWeight / all) * 100;
            _lblMstSummary.Text = shown.Algorithm + " keeps " + shown.Edges.Count + " of " + network.EdgeCount + " roads: " +
                                  shown.TotalWeight.ToString("0.0") + " km instead of " + all.ToString("0.0") + " km (" + saved.ToString("0") + "% less). " +
                                  "Kruskal gives " + kruskal.TotalWeight.ToString("0.0") + " km and Prim " + prim.TotalWeight.ToString("0.0") +
                                  " km, so the two algorithms agree.";

            int step = 1;
            foreach (Edge<string> e in shown.Edges)
            {
                _lstMst.Items.Add(step++ + ".  " + e.From + "  -  " + e.To + "    " + e.Weight.ToString("0.0") + " km");
            }
        }

        private void UpdateTraversal()
        {
            _lstTraversal.Items.Clear();
            if (_cmbTraversal.SelectedIndex == 0 || _cmbStart.SelectedItem == null)
            {
                _graphView.SetVisitOrder(null);
                return;
            }

            string start = (string)_cmbStart.SelectedItem;
            List<TraversalStep<string>> steps = _cmbTraversal.SelectedIndex == 1
                ? AreaNetwork.Graph.BreadthFirst(start)
                : AreaNetwork.Graph.DepthFirst(start);

            _graphView.SetVisitOrder(steps.Select(s => s.Vertex));

            int n = 1;
            foreach (TraversalStep<string> s in steps)
            {
                _lstTraversal.Items.Add(n++ + ".  " + s.Vertex + "   (" + s.Level + " " + (_cmbTraversal.SelectedIndex == 1 ? "road" + (s.Level == 1 ? "" : "s") + " away" : "level" + (s.Level == 1 ? "" : "s") + " deep") + ")");
            }
        }
    }
}
