using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Controls;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    // Tab 1 - Track Requests: find a request by its unique reference number and follow its progress.
    //   AVL / BST / red-black trees ... lookup by reference number (cost compared)
    //   red-black date index .......... the list order and the "reported between" filter
    //   graph + breadth-first search .. "related requests"
    public partial class ServiceRequestStatusForm
    {
        private TextBox _txtTrackId;
        private Label _lblTrackMessage, _lblGridCount;
        private ComboBox _cmbStatusFilter;
        private CheckBox _chkDateFilter;
        private DateTimePicker _dtpFrom, _dtpTo;
        private DataGridView _dgvRequests;

        private Label _lblDetailTitle, _lblDetailMeta, _lblDetailPriority, _lblLookupCost;
        private StatusStepper _stepper;
        private RichTextBox _rtbDetail;
        private ListBox _lstRelated;

        private int? _selectedId;
        private bool _loadingGrid;

        private TabPage BuildTrackTab()
        {
            var page = new TabPage("Track Requests") { BackColor = AppTheme.Background, Padding = new Padding(10) };

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43F));

            // ---------------- left: search, filters, list ----------------
            var left = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 0), Size = new Size(646, 505) };

            _dgvRequests = new DataGridView();
            StyleGrid(_dgvRequests);
            _dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ref", HeaderText = "Ref #", Width = 60 });
            _dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { Name = "Priority", HeaderText = "Priority", Width = 75 });
            _dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 110 });
            _dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { Name = "Area", HeaderText = "Area", Width = 80 });
            _dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 });
            _dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { Name = "Reported", HeaderText = "Reported", Width = 120 });
            _dgvRequests.SelectionChanged += DgvRequests_SelectionChanged;

            var toolbar = new Panel { Dock = DockStyle.Top, Size = new Size(646, 100), BackColor = Color.White };
            toolbar.Controls.Add(MakeLabel("Track a request by its reference number", new Font("Segoe UI", 9.5F, FontStyle.Bold), AppTheme.PrimaryDark, 12, 8, 330, 20));

            _txtTrackId = new TextBox { Font = AppTheme.FontBody, Location = new Point(12, 32), Size = new Size(130, 26) };
            _txtTrackId.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; TrackRequest(); }
            };
            var btnTrack = MakeButton("Track", AppTheme.Primary, 90);
            btnTrack.Height = 28;
            btnTrack.Location = new Point(150, 31);
            btnTrack.Click += (s, e) => TrackRequest();
            _lblTrackMessage = MakeLabel("Enter a number such as 12 or #12.", new Font("Segoe UI", 9F, FontStyle.Italic), AppTheme.TextMuted, 250, 35, 390, 20);

            _cmbStatusFilter = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontBody, Location = new Point(12, 66), Size = new Size(130, 26) };
            _cmbStatusFilter.Items.AddRange(new object[] { "All statuses", "Submitted", "Under Review", "In Progress", "Resolved" });
            _cmbStatusFilter.SelectedIndex = 0;
            _cmbStatusFilter.SelectedIndexChanged += (s, e) => ApplyFilters();

            _chkDateFilter = new CheckBox { Text = "Reported between", Font = new Font("Segoe UI", 9F), Location = new Point(152, 68), Size = new Size(150, 22) };
            _chkDateFilter.CheckedChanged += (s, e) =>
            {
                _dtpFrom.Enabled = _chkDateFilter.Checked;
                _dtpTo.Enabled = _chkDateFilter.Checked;
                ApplyFilters();
            };
            _dtpFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 9F), Location = new Point(300, 66), Size = new Size(98, 24), Enabled = false, Value = DateTime.Today.AddDays(-14) };
            _dtpTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 9F), Location = new Point(430, 66), Size = new Size(98, 24), Enabled = false, Value = DateTime.Today };
            _dtpFrom.ValueChanged += (s, e) => ApplyFilters();
            _dtpTo.ValueChanged += (s, e) => ApplyFilters();
            _lblGridCount = MakeLabel("", new Font("Segoe UI", 9F, FontStyle.Bold), AppTheme.Primary, 534, 69, 106, 20);
            _lblGridCount.AutoEllipsis = false;
            _lblGridCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            toolbar.Controls.AddRange(new Control[] { _txtTrackId, btnTrack, _lblTrackMessage, _cmbStatusFilter, _chkDateFilter, _dtpFrom, MakeLabel("and", AppTheme.FontCardCaption, Color.Black, 402, 70, 28, 18), _dtpTo, _lblGridCount });

            left.Controls.Add(_dgvRequests);
            left.Controls.Add(toolbar);

            // ---------------- right: the selected request ----------------
            // The size matters: controls anchored left+right keep their margins relative to this starting size.
            var right = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0), Size = new Size(497, 505) };
            const AnchorStyles wide = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _lblDetailTitle = MakeLabel("Select a request to see its progress", new Font("Segoe UI", 12.5F, FontStyle.Bold), AppTheme.PrimaryDark, 16, 12, 440, 26);
            _lblDetailTitle.Anchor = wide;
            _stepper = new StatusStepper { Location = new Point(10, 44), Size = new Size(450, 66), Anchor = wide };
            _lblDetailMeta = MakeLabel("", new Font("Segoe UI", 9F), AppTheme.TextMuted, 16, 114, 440, 20);
            _lblDetailMeta.Anchor = wide;
            _lblDetailPriority = MakeLabel("", new Font("Segoe UI", 10F, FontStyle.Bold), AppTheme.Primary, 16, 136, 440, 22);
            _lblDetailPriority.Anchor = wide;

            _rtbDetail = new RichTextBox
            {
                Location = new Point(16, 162),
                Size = new Size(444, 128),
                Anchor = wide,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(247, 249, 250),
                Font = new Font("Segoe UI", 9F)
            };

            _lblLookupCost = new Label
            {
                Location = new Point(16, 298),
                Size = new Size(444, 52),
                Anchor = wide,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(60, 70, 80),
                BackColor = Color.FromArgb(232, 244, 253),
                Padding = new Padding(8, 5, 8, 5)
            };

            var relatedHeader = MakeLabel("Related requests (breadth-first search of the relationship graph)", new Font("Segoe UI", 9F, FontStyle.Bold), AppTheme.PrimaryDark, 16, 358, 444, 20);
            relatedHeader.Anchor = wide;
            _lstRelated = new ListBox
            {
                Location = new Point(16, 380),
                Size = new Size(444, 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.FixedSingle,
                HorizontalScrollbar = true
            };

            right.Controls.AddRange(new Control[] { _lblDetailTitle, _stepper, _lblDetailMeta, _lblDetailPriority, _rtbDetail, _lblLookupCost, relatedHeader, _lstRelated });

            root.Controls.Add(left, 0, 0);
            root.Controls.Add(right, 1, 0);
            page.Controls.Add(root);
            return page;
        }

        private void RefreshTrackTab()
        {
            ApplyFilters();
        }

        // ---------------- filtering (uses the red-black date index) ----------------

        private void ApplyFilters()
        {
            if (_service == null || _dgvRequests == null) return;

            // Newest first for the user. The source list comes from an in-order walk (or a range query) of the red-black date tree.
            List<RequestInfo> list = _chkDateFilter.Checked
                ? _service.ReportedBetween(_dtpFrom.Value, _dtpTo.Value)
                : _service.Chronological();
            list = Enumerable.Reverse(list).ToList();

            if (_cmbStatusFilter.SelectedIndex > 0)
            {
                var wanted = (IssueStatus)(_cmbStatusFilter.SelectedIndex - 1);
                list = list.Where(r => r.Issue.Status == wanted).ToList();
            }

            _loadingGrid = true;
            _dgvRequests.Rows.Clear();
            foreach (RequestInfo r in list)
            {
                int row = _dgvRequests.Rows.Add(
                    "#" + r.Id,
                    r.PriorityLabel,
                    r.Issue.Category,
                    r.Area,
                    IssueStatusHelper.GetShortText(r.Issue.Status),
                    r.Issue.DateReported.ToString("dd MMM yyyy HH:mm"));

                DataGridViewRow gridRow = _dgvRequests.Rows[row];
                gridRow.Tag = r;
                gridRow.Cells["Priority"].Style.ForeColor = PriorityColor(r.Priority);
                gridRow.Cells["Priority"].Style.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                gridRow.Cells["Status"].Style.ForeColor = IssueStatusHelper.GetColor(r.Issue.Status);
                gridRow.Cells["Status"].Style.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            }
            _loadingGrid = false;

            _lblGridCount.Text = "Showing " + list.Count + "/" + _service.TotalCount;

            // Keep the same request selected if it is still listed; otherwise fall back to the first row.
            _dgvRequests.ClearSelection();
            DataGridViewRow toSelect = null;
            if (_selectedId.HasValue)
            {
                toSelect = _dgvRequests.Rows.Cast<DataGridViewRow>()
                    .FirstOrDefault(r => ((RequestInfo)r.Tag).Id == _selectedId.Value);
            }
            if (toSelect == null && _dgvRequests.Rows.Count > 0 && !_selectedId.HasValue)
            {
                toSelect = _dgvRequests.Rows[0];
            }

            if (toSelect != null)
            {
                toSelect.Selected = true;
                _dgvRequests.CurrentCell = toSelect.Cells["Ref"];
                ShowDetail((RequestInfo)toSelect.Tag);
            }
            else if (_dgvRequests.Rows.Count == 0 && !_selectedId.HasValue)
            {
                ClearDetail();
            }
        }

        private void DgvRequests_SelectionChanged(object sender, EventArgs e)
        {
            if (_loadingGrid || _dgvRequests.SelectedRows.Count == 0) return;
            var info = _dgvRequests.SelectedRows[0].Tag as RequestInfo;
            if (info != null && info.Id != _selectedId)
            {
                ShowDetail(info);
            }
        }

        // ---------------- tracking by reference number ----------------

        private void TrackRequest()
        {
            string digits = new string(_txtTrackId.Text.Where(char.IsDigit).ToArray());
            int id;
            if (digits.Length == 0 || digits.Length > 9 || !int.TryParse(digits, out id))
            {
                SetTrackMessage("Please enter a reference number such as 12 or #12.", Color.FromArgb(192, 57, 43));
                return;
            }

            LookupResult result = _service.Lookup(id);
            if (!result.Found)
            {
                SetTrackMessage("No request found with reference #" + id + ".", Color.FromArgb(192, 57, 43));
                return;
            }

            // If the filters are hiding it, clear them so the match can be shown.
            bool visible = _dgvRequests.Rows.Cast<DataGridViewRow>().Any(r => ((RequestInfo)r.Tag).Id == id);
            if (!visible)
            {
                _loadingGrid = true;
                _cmbStatusFilter.SelectedIndex = 0;
                _chkDateFilter.Checked = false;
                _loadingGrid = false;
            }

            _selectedId = id;
            ApplyFilters();

            SetTrackMessage("Found #" + id + " in " + Plural(result.AvlComparisons, "comparison", "comparisons") + " (AVL tree).", AppTheme.Success);
        }

        private void SetTrackMessage(string text, Color color)
        {
            _lblTrackMessage.Text = text;
            _lblTrackMessage.ForeColor = color;
        }

        // ---------------- details panel ----------------

        private void ShowDetail(RequestInfo info)
        {
            _selectedId = info.Id;
            _selectedRequest = info;

            Issue issue = info.Issue;
            _lblDetailTitle.Text = "Request #" + info.Id + "  -  " + issue.Category;
            _stepper.SetStatus(issue.Status);

            int age = info.AgeDays;
            _lblDetailMeta.Text = info.Area + "   |   reported " + issue.DateReported.ToString("dd MMM yyyy HH:mm") +
                                  " (" + (age == 0 ? "today" : Plural(age, "day", "days") + " ago") + ")";
            _lblDetailPriority.Text = "Priority: " + info.PriorityLabel + "   (decided by " + string.Join(" > ", info.Triage.Codes) + ")";
            _lblDetailPriority.ForeColor = PriorityColor(info.Priority);

            _rtbDetail.Clear();
            AppendRich(_rtbDetail, "Location: ", true);
            AppendRich(_rtbDetail, issue.Location + "\n\n", false);
            AppendRich(_rtbDetail, "Description: ", true);
            AppendRich(_rtbDetail, issue.Description + "\n\n", false);
            AppendRich(_rtbDetail, "How the priority was decided:\n", true);
            foreach (string step in info.Triage.Steps) AppendRich(_rtbDetail, "  " + step + "\n", false);
            if (issue.AttachmentPaths != null && issue.AttachmentPaths.Count > 0)
            {
                AppendRich(_rtbDetail, "\nAttachments: " + issue.AttachmentPaths.Count, false);
            }

            LookupResult cost = _service.Lookup(info.Id);
            _lblLookupCost.Text =
                "Search cost for reference #" + info.Id + " (key comparisons / tree height):\n" +
                "Binary search tree " + cost.BstComparisons + " / " + _service.BstById.Height +
                "     AVL tree " + cost.AvlComparisons + " / " + _service.AvlById.Height +
                "     Red-black tree " + cost.RbComparisons + " / " + _service.RbById.Height;

            _lstRelated.Items.Clear();
            List<RelatedRequest> related = _service.RelatedTo(info.Id);
            if (related.Count == 0)
            {
                _lstRelated.Items.Add("No related requests.");
            }
            foreach (RelatedRequest r in related)
            {
                string link = r.LinkWeight == 1 ? "same ward, same category"
                            : r.LinkWeight == 2 ? "same ward"
                            : "same category, nearby ward";
                _lstRelated.Items.Add(Plural(r.Level, "hop", "hops") + ":  #" + r.Info.Id + "  " + r.Info.Issue.Category +
                                      " (" + r.Info.Area + ")  [" + IssueStatusHelper.GetShortText(r.Info.Issue.Status) + "]  - " + link);
            }

            SyncTriageSelection();
        }

        private void ClearDetail()
        {
            _selectedRequest = null;
            _lblDetailTitle.Text = "Select a request to see its progress";
            _stepper.SetStatus(null);
            _lblDetailMeta.Text = "";
            _lblDetailPriority.Text = "";
            _rtbDetail.Clear();
            _lblLookupCost.Text = "";
            _lstRelated.Items.Clear();
        }

        private static void AppendRich(RichTextBox box, string text, bool bold)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionFont = new Font("Segoe UI", 9F, bold ? FontStyle.Bold : FontStyle.Regular);
            box.AppendText(text);
        }
    }
}
