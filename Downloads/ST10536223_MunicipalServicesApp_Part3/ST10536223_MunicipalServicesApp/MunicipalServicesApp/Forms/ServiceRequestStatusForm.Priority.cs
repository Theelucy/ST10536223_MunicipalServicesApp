using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Controls;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    // Tab 2 - Priority Queue: a binary heap keeps the request that most needs a crew at the top.
    //   Insert / extract-top are O(log n); finding the next request is O(1) (just peek at the root).
    public partial class ServiceRequestStatusForm
    {
        private const int MaxHeapNodesDrawn = 15;

        private DataGridView _dgvQueue;
        private Label _lblQueueInfo, _lblAssignMessage;
        private Button _btnAssign;
        private BinaryTreeView _heapView;
        private string _lastAssignMessage = "";

        // A slot in the drawn heap: its request plus links to its two children (index 2i+1 and 2i+2).
        private class HeapSlot
        {
            public RequestInfo Info;
            public HeapSlot Left, Right;
            public bool IsTop;
        }

        private TabPage BuildPriorityTab()
        {
            var page = new TabPage("Priority Queue") { BackColor = AppTheme.Background, Padding = new Padding(10) };

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54F));

            // ---------------- left: the ordered queue ----------------
            var left = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 0), Size = new Size(519, 505) };

            _dgvQueue = new DataGridView();
            StyleGrid(_dgvQueue);
            _dgvQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "Pos", HeaderText = "#", Width = 36 });
            _dgvQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ref", HeaderText = "Ref #", Width = 56 });
            _dgvQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "Priority", HeaderText = "Priority", Width = 72 });
            _dgvQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 100 });
            _dgvQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "Area", HeaderText = "Area", Width = 72 });
            _dgvQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "Waiting", HeaderText = "Waiting", Width = 70 });

            var top = new Panel { Dock = DockStyle.Top, Size = new Size(519, 120), BackColor = Color.White };
            top.Controls.Add(MakeLabel("Requests waiting for a crew", new Font("Segoe UI", 11F, FontStyle.Bold), AppTheme.PrimaryDark, 12, 8, 330, 22));
            _lblQueueInfo = MakeLabel("", new Font("Segoe UI", 9F), Color.FromArgb(60, 70, 80), 12, 32, 495, 36);
            _lblQueueInfo.AutoEllipsis = false;
            _btnAssign = MakeButton("Assign next request to a crew", Color.FromArgb(192, 57, 43), 250);
            _btnAssign.Location = new Point(12, 72);
            _btnAssign.Click += BtnAssign_Click;
            _lblAssignMessage = MakeLabel("", new Font("Segoe UI", 8.5F, FontStyle.Italic), AppTheme.Success, 270, 72, 240, 40);
            _lblAssignMessage.AutoEllipsis = false;
            top.Controls.AddRange(new Control[] { _lblQueueInfo, _btnAssign, _lblAssignMessage });

            left.Controls.Add(_dgvQueue);
            left.Controls.Add(top);

            // ---------------- right: the heap drawn as a tree ----------------
            var right = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0), Size = new Size(624, 505) };
            var title = MakeLabel("The heap, drawn as the tree it really is", new Font("Segoe UI", 11F, FontStyle.Bold), AppTheme.PrimaryDark, 12, 8, 480, 22);
            var note = new Label
            {
                Location = new Point(12, 32),
                Size = new Size(600, 56),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = AppTheme.TextMuted,
                Text = "Each request sits at or above its two children, so the most urgent one is always at the top (ringed). " +
                       "The queue is stored in a plain array: the children of position i are at 2i+1 and 2i+2. " +
                       "Colour shows priority: red Critical, orange High, blue Medium, grey Low. First 15 shown."
            };
            _heapView = new BinaryTreeView
            {
                Location = new Point(8, 92),
                Size = new Size(608, 405),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                EmptyMessage = "No requests are waiting for a crew."
            };
            right.Controls.AddRange(new Control[] { title, note, _heapView });

            root.Controls.Add(left, 0, 0);
            root.Controls.Add(right, 1, 0);
            page.Controls.Add(root);
            return page;
        }

        private void RefreshPriorityTab()
        {
            BinaryHeap<RequestInfo> heap = _service.TriageQueue;

            // ToSortedList() copies the heap and extracts from the copy, so the real queue is untouched.
            List<RequestInfo> ordered = heap.ToSortedList();

            _dgvQueue.Rows.Clear();
            for (int i = 0; i < ordered.Count; i++)
            {
                RequestInfo r = ordered[i];
                int row = _dgvQueue.Rows.Add(i + 1, "#" + r.Id, r.PriorityLabel, r.Issue.Category, r.Area,
                    r.AgeDays == 0 ? "today" : r.AgeDays + " d");
                _dgvQueue.Rows[row].Cells["Priority"].Style.ForeColor = PriorityColor(r.Priority);
                _dgvQueue.Rows[row].Cells["Priority"].Style.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                _dgvQueue.Rows[row].Tag = r;
            }
            _dgvQueue.ClearSelection();

            if (ordered.Count == 0)
            {
                _lblQueueInfo.Text = "Nothing is waiting. Every request is with a crew or resolved.";
            }
            else
            {
                RequestInfo next = ordered[0];
                _lblQueueInfo.Text = Plural(ordered.Count, "request is", "requests are") + " waiting.  Next up: #" + next.Id + " (" + next.PriorityLabel +
                                     ", " + next.Issue.Category + ").  The queue is ordered by priority, then by how long each has waited.";
            }
            _btnAssign.Enabled = ordered.Count > 0;
            _lblAssignMessage.Text = _lastAssignMessage;

            _heapView.SetNodes(BuildHeapLayout(heap.ToArray()));
        }

        private void BtnAssign_Click(object sender, EventArgs e)
        {
            RequestInfo assigned = _service.AssignNextToCrew();
            if (assigned == null) return;

            _lastAssignMessage = "#" + assigned.Id + " (" + assigned.PriorityLabel + ") is now with a crew: In Progress.";
            _selectedId = assigned.Id;
            RefreshAll(); // statuses changed, so rebuild every structure from the issue list
        }

        /// <summary>Turns the heap's array into a drawable tree: item i has children 2i+1 and 2i+2.</summary>
        private static List<TreeDrawNode> BuildHeapLayout(RequestInfo[] items)
        {
            int n = Math.Min(items.Length, MaxHeapNodesDrawn);
            if (n == 0) return new List<TreeDrawNode>();

            var slots = new HeapSlot[n];
            for (int i = 0; i < n; i++) slots[i] = new HeapSlot { Info = items[i], IsTop = i == 0 };
            for (int i = 0; i < n; i++)
            {
                if (2 * i + 1 < n) slots[i].Left = slots[2 * i + 1];
                if (2 * i + 2 < n) slots[i].Right = slots[2 * i + 2];
            }

            return TreeLayout.Build<HeapSlot>(
                slots[0],
                s => s.Left,
                s => s.Right,
                s => "#" + s.Info.Id,
                s => PriorityColor(s.Info.Priority),
                s => s.IsTop,
                null,
                null);
        }
    }
}
