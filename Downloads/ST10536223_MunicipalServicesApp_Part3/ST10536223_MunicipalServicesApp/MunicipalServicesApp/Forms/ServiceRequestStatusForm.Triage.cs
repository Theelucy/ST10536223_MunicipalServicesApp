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
    // Tab 4 - Triage: a plain binary tree. Every internal node asks one yes/no question
    // (left = Yes, right = No) and every leaf is a priority, so a request is classified in
    // at most three questions. Pick a request to see the path it takes.
    public partial class ServiceRequestStatusForm
    {
        private RequestInfo _selectedRequest;
        private BinaryTreeView _triageView;
        private ComboBox _cmbTriage;
        private RichTextBox _rtbTriageSteps;
        private Label _lblTriageFacts;
        private TextBox _txtTraversals;
        private readonly List<RequestInfo> _triageChoices = new List<RequestInfo>();
        private bool _syncingTriage;

        private TabPage BuildTriageTab()
        {
            var page = new TabPage("Triage") { BackColor = AppTheme.Background, Padding = new Padding(10) };

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = Color.White };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 56F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 44F));

            var bar = new Panel { Dock = DockStyle.Fill, Size = new Size(1156, 50) };
            bar.Controls.Add(MakeLabel("Show the decision path for:", new Font("Segoe UI", 9.5F, FontStyle.Bold), AppTheme.PrimaryDark, 12, 15, 190, 20));
            _cmbTriage = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontBody, Location = new Point(204, 11), Size = new Size(930, 26), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            _cmbTriage.SelectedIndexChanged += CmbTriage_SelectedIndexChanged;
            bar.Controls.Add(_cmbTriage);

            _triageView = new BinaryTreeView { Dock = DockStyle.Fill, EmptyMessage = "(empty tree)" };

            var info = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            _rtbTriageSteps = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(247, 249, 250), Font = new Font("Segoe UI", 9.5F), Margin = new Padding(10, 4, 5, 8) };

            var facts = new Panel { Dock = DockStyle.Fill, Margin = new Padding(5, 4, 10, 8) };
            _lblTriageFacts = new Label { Dock = DockStyle.Top, Height = 74, Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(60, 70, 80) };
            _txtTraversals = new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Font = new Font("Segoe UI", 9F), BackColor = Color.FromArgb(247, 249, 250), BorderStyle = BorderStyle.None };
            facts.Controls.Add(_txtTraversals);
            facts.Controls.Add(_lblTriageFacts);

            info.Controls.Add(_rtbTriageSteps, 0, 0);
            info.Controls.Add(facts, 1, 0);

            root.Controls.Add(bar, 0, 0);
            root.Controls.Add(_triageView, 0, 1);
            root.Controls.Add(info, 0, 2);
            page.Controls.Add(root);
            return page;
        }

        private void RefreshTriageTab()
        {
            _syncingTriage = true;
            _cmbTriage.Items.Clear();
            _triageChoices.Clear();
            foreach (RequestInfo r in _service.All().OrderBy(x => x.Id))
            {
                _triageChoices.Add(r);
                string text = r.Issue.Description ?? "";
                if (text.Length > 60) text = text.Substring(0, 60) + "...";
                _cmbTriage.Items.Add("#" + r.Id + "  " + r.Issue.Category + "  -  " + text + "   [" + r.PriorityLabel + "]");
            }
            _syncingTriage = false;

            BinaryTree<TriageNode> tree = TriageTree.Tree;
            _lblTriageFacts.Text = "The triage tree has " + tree.Count + " nodes (" + tree.LeafCount + " outcomes) and is " + tree.Height +
                                   " levels deep, so any request is classified in at most " + (tree.Height - 1) + " questions.\n" +
                                   "Left branch = Yes, right branch = No. Orange outline = the path taken.";

            _txtTraversals.Text =
                "Traversals of the triage tree:\r\n" +
                "Pre-order    " + Codes(tree.PreOrder()) + "\r\n" +
                "In-order     " + Codes(tree.InOrder()) + "\r\n" +
                "Post-order   " + Codes(tree.PostOrder()) + "\r\n" +
                "Level-order  " + Codes(tree.LevelOrder()) + "\r\n\r\n" +
                "Key:\r\n" +
                string.Join("\r\n", tree.PreOrder().Select(n => n.Code + "  =  " + n.Text));

            SyncTriageSelection();
        }

        // Called whenever the request selected on the Track tab changes.
        private void SyncTriageSelection()
        {
            if (_cmbTriage == null || _triageChoices.Count == 0)
            {
                DrawTriage(null);
                return;
            }

            int index = _selectedRequest == null ? -1 : _triageChoices.FindIndex(r => r.Id == _selectedRequest.Id);
            if (index < 0) index = 0;

            _syncingTriage = true;
            _cmbTriage.SelectedIndex = index;
            _syncingTriage = false;
            DrawTriage(_triageChoices[index]);
        }

        private void CmbTriage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingTriage || _cmbTriage.SelectedIndex < 0) return;
            DrawTriage(_triageChoices[_cmbTriage.SelectedIndex]);
        }

        private void DrawTriage(RequestInfo request)
        {
            var path = request == null ? new HashSet<string>() : new HashSet<string>(request.Triage.Codes);

            _triageView.SetNodes(TriageTree.Tree.GetLayout(
                n => n.Code,
                n => n.IsLeaf ? PriorityColor(n.Priority) : Color.FromArgb(52, 73, 94),
                n => path.Contains(n.Code),
                "Yes", "No"));

            _rtbTriageSteps.Clear();
            if (request == null)
            {
                AppendRich(_rtbTriageSteps, "No requests yet. Load the sample requests to see how each one is triaged.", false);
                return;
            }

            AppendRich(_rtbTriageSteps, "Request #" + request.Id + "  -  " + request.Issue.Category + "\n", true);
            AppendRich(_rtbTriageSteps, request.Issue.Description + "\n\n", false);
            AppendRich(_rtbTriageSteps, "Decisions, root to leaf:\n", true);
            foreach (string step in request.Triage.Steps) AppendRich(_rtbTriageSteps, "  " + step + "\n", false);
        }

        private static string Codes(List<TriageNode> nodes)
        {
            return string.Join(", ", nodes.Select(n => n.Code));
        }
    }
}
