using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    /// <summary>
    /// The Service Request Status page. It is a tabbed workspace in which each tab
    /// uses a different data structure for a genuine job:
    ///
    ///   Track Requests   AVL / BST / red-black trees (lookup by reference number),
    ///                    red-black date index (range filter), graph (related requests)
    ///   Priority Queue   binary heap (most urgent request first)
    ///   Areas            general tree (Municipality - Ward - Category - Request)
    ///   Triage           binary decision tree (how priorities are decided)
    ///   Tree Lab         BST vs AVL vs red-black, drawn and measured side by side
    ///   Area Network     graph, BFS / DFS, minimum spanning tree (Kruskal and Prim)
    ///
    /// Each tab lives in its own file (ServiceRequestStatusForm.Track.cs, ...).
    /// </summary>
    public partial class ServiceRequestStatusForm : Form
    {
        private ServiceRequestService _service;
        private TabControl _tabs;
        private Label _lblTotalValue, _lblPendingValue, _lblResolvedValue, _lblCriticalValue;
        private Label _lblEmptyHint;
        private Button _btnBack, _btnLoadSample;

        public ServiceRequestStatusForm()
        {
            InitializeComponent();
            Load += ServiceRequestStatusForm_Load;
        }

        // ----------------------------------------------------------------
        // Shell
        // ----------------------------------------------------------------

        private void InitializeComponent()
        {
            SuspendLayout();
            BackColor = AppTheme.Background;
            ClientSize = new Size(1180, 780);
            MinimumSize = new Size(1000, 680);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Municipal Services Application - Service Request Status";

            var header = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = AppTheme.HeaderDark };
            header.Controls.Add(new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(24, 10),
                Text = "Service Request Status"
            });
            header.Controls.Add(new Label
            {
                AutoSize = true,
                Font = AppTheme.FontSubtitle,
                ForeColor = Color.FromArgb(210, 220, 230),
                Location = new Point(26, 46),
                Text = "Track requests by reference number, see what is most urgent, and explore how the data is organised"
            });

            var stats = new Panel { Dock = DockStyle.Top, Height = 92, BackColor = AppTheme.Background };
            stats.Controls.Add(MakeStatCard("Total requests", out _lblTotalValue, AppTheme.Primary, 16));
            stats.Controls.Add(MakeStatCard("Pending", out _lblPendingValue, AppTheme.Warning, 226));
            stats.Controls.Add(MakeStatCard("Resolved", out _lblResolvedValue, AppTheme.Success, 436));
            stats.Controls.Add(MakeStatCard("Critical and open", out _lblCriticalValue, Color.FromArgb(192, 57, 43), 646));

            _lblEmptyHint = new Label
            {
                AutoSize = false,
                Location = new Point(866, 14),
                Size = new Size(300, 62),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = AppTheme.TextMuted,
                Text = "",
                TextAlign = ContentAlignment.MiddleLeft
            };
            stats.Controls.Add(_lblEmptyHint);

            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 56, BackColor = AppTheme.Background };
            _btnBack = MakeButton("Back to Main Menu", AppTheme.Success, 170);
            _btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnBack.Location = new Point(bottom.Width - 190, 10);
            _btnBack.Click += (s, e) => Close();

            _btnLoadSample = MakeButton("Load Sample Requests", AppTheme.Primary, 190);
            _btnLoadSample.Location = new Point(16, 10);
            _btnLoadSample.Click += BtnLoadSample_Click;

            bottom.Controls.Add(_btnBack);
            bottom.Controls.Add(_btnLoadSample);
            bottom.Resize += (s, e) => { _btnBack.Location = new Point(bottom.Width - _btnBack.Width - 16, 10); };

            _tabs = new TabControl { Dock = DockStyle.Fill, Font = AppTheme.FontBody, Padding = new Point(14, 6) };
            _tabs.TabPages.Add(BuildTrackTab());
            _tabs.TabPages.Add(BuildPriorityTab());
            _tabs.TabPages.Add(BuildAreasTab());
            _tabs.TabPages.Add(BuildTriageTab());
            _tabs.TabPages.Add(BuildTreeLabTab());
            _tabs.TabPages.Add(BuildNetworkTab());

            // Docking is applied last-added-first: header and stats take the top, the bottom bar takes the
            // bottom, and the tab control fills what is left.
            Controls.Add(_tabs);
            Controls.Add(bottom);
            Controls.Add(stats);
            Controls.Add(header);
            ResumeLayout(false);
        }

        private void ServiceRequestStatusForm_Load(object sender, EventArgs e)
        {
            RefreshAll();
        }

        /// <summary>Rebuilds every data structure from the current issues and redraws every tab.</summary>
        private void RefreshAll()
        {
            _service = new ServiceRequestService();

            _lblTotalValue.Text = _service.TotalCount.ToString();
            _lblPendingValue.Text = _service.PendingCount.ToString();
            _lblResolvedValue.Text = _service.ResolvedCount.ToString();
            _lblCriticalValue.Text = _service.CriticalOpenCount.ToString();

            _btnLoadSample.Enabled = !SampleRequestLoader.HasLoaded;
            _btnLoadSample.Text = SampleRequestLoader.HasLoaded ? "Sample requests loaded" : "Load Sample Requests";
            _btnLoadSample.BackColor = SampleRequestLoader.HasLoaded ? Color.FromArgb(189, 195, 199) : AppTheme.Primary;
            _btnLoadSample.ForeColor = SampleRequestLoader.HasLoaded ? Color.FromArgb(70, 80, 90) : Color.White;
            _lblEmptyHint.Text = _service.TotalCount == 0
                ? "No requests yet. Report one from the main menu, or click \"Load Sample Requests\" to explore."
                : "";

            RefreshTrackTab();
            RefreshPriorityTab();
            RefreshAreasTab();
            RefreshTriageTab();
            RefreshTreeLabTab();
            RefreshNetworkTab();
        }

        private void BtnLoadSample_Click(object sender, EventArgs e)
        {
            int added = SampleRequestLoader.Load();
            if (added > 0)
            {
                RefreshAll();
                MessageBox.Show(this,
                    added + " sample service requests were added. They also appear on the home dashboard.",
                    "Sample requests loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ----------------------------------------------------------------
        // Shared helpers
        // ----------------------------------------------------------------

        private static Panel MakeStatCard(string title, out Label valueLabel, Color accent, int x)
        {
            var panel = new Panel { Size = new Size(200, 72), Location = new Point(x, 10), BackColor = Color.White };
            panel.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accent });
            panel.Controls.Add(new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = AppTheme.TextMuted,
                Location = new Point(16, 8),
                Text = title
            });
            valueLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = accent,
                Location = new Point(14, 28),
                Text = "0"
            };
            panel.Controls.Add(valueLabel);
            return panel;
        }

        private static Button MakeButton(string text, Color color, int width)
        {
            var button = new Button
            {
                Text = text,
                Width = width,
                Height = 36,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private static Label MakeLabel(string text, Font font, Color color, int x, int y, int width, int height)
        {
            return new Label
            {
                Text = text,
                Font = font,
                ForeColor = color,
                Location = new Point(x, y),
                Size = new Size(width, height),
                AutoEllipsis = true
            };
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.AutoGenerateColumns = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Font = new Font("Segoe UI", 9.5F);
            grid.RowTemplate.Height = 26;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.PrimaryDark,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                SelectionBackColor = AppTheme.PrimaryDark
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White,
                ForeColor = Color.Black,
                SelectionBackColor = Color.FromArgb(214, 234, 248),
                SelectionForeColor = Color.Black
            };
        }

        private static Color PriorityColor(int priority)
        {
            switch (priority)
            {
                case 1: return Color.FromArgb(192, 57, 43);
                case 2: return Color.FromArgb(230, 126, 34);
                case 3: return Color.FromArgb(41, 128, 185);
                default: return Color.FromArgb(127, 140, 141);
            }
        }

        private static string Plural(int n, string singular, string plural)
        {
            return n + " " + (n == 1 ? singular : plural);
        }
    }
}
