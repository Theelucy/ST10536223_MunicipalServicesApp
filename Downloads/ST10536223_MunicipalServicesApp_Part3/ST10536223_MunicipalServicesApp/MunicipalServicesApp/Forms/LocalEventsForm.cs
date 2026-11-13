using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Forms
{
    public partial class LocalEventsForm : Form
    {
        private TextBox txtSearch;
        private ComboBox cmbCategory;
        private ListBox lstRecommendations;
        private DataGridView dgvEvents;
        private Label lblAnnouncement;
        private Label lblResultsSummary;
        private Button btnBack;
        private Button btnClearFilters;

        public LocalEventsForm()
        {
            InitializeComponent();
            this.Load += LocalEventsForm_Load;
        }

        private void InitializeComponent()
        {
            this.BackColor = AppTheme.Background;
            this.ClientSize = new Size(1100, 760);
            this.MinimumSize = new Size(900, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Local Events and Announcements";

            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = AppTheme.HeaderDark
            };

            var lblTitle = new Label
            {
                AutoSize = true,
                Font = AppTheme.FontTitle,
                ForeColor = Color.White,
                Location = new Point(24, 24),
                Text = "Local Events and Announcements"
            };

            var lblSubtitle = new Label
            {
                AutoSize = true,
                Font = AppTheme.FontSubtitle,
                ForeColor = Color.FromArgb(210, 220, 230),
                Location = new Point(24, 58),
                Text = "Upcoming municipal activities, community events and local notices"
            };

            titlePanel.Controls.Add(lblTitle);
            titlePanel.Controls.Add(lblSubtitle);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18)
            };

            var filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 74,
                BackColor = Color.White,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

            var searchLabel = new Label
            {
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.PrimaryDark,
                Location = new Point(12, 12),
                Text = "Search events"
            };

            txtSearch = new TextBox
            {
                Width = 330,
                Height = 28,
                Location = new Point(120, 8),
                Font = AppTheme.FontBody
            };

            var categoryLabel = new Label
            {
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.PrimaryDark,
                Location = new Point(470, 12),
                Text = "Filter by"
            };

            cmbCategory = new ComboBox
            {
                Width = 190,
                Height = 28,
                Location = new Point(550, 8),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = AppTheme.FontBody
            };

            btnClearFilters = new Button
            {
                Text = "Clear Filters",
                Width = 120,
                Height = 30,
                Location = new Point(760, 6),
                BackColor = AppTheme.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = AppTheme.FontButton
            };
            btnClearFilters.FlatAppearance.BorderSize = 0;

            btnBack = new Button
            {
                Text = "Back to Main Menu",
                Width = 170,
                Height = 30,
                Location = new Point(890, 6),
                BackColor = AppTheme.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = AppTheme.FontButton
            };
            btnBack.FlatAppearance.BorderSize = 0;

            filterPanel.Controls.Add(searchLabel);
            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(categoryLabel);
            filterPanel.Controls.Add(cmbCategory);
            filterPanel.Controls.Add(btnClearFilters);
            filterPanel.Controls.Add(btnBack);

            var announcementPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = AppTheme.TintBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 12, 0, 0),
                Padding = new Padding(12)
            };

            var lblAnnouncementTitle = new Label
            {
                AutoSize = true,
                Font = AppTheme.FontSectionHeader,
                ForeColor = AppTheme.PrimaryDark,
                Location = new Point(12, 12),
                Text = "Municipal Announcement"
            };

            lblAnnouncement = new Label
            {
                AutoSize = false,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.PrimaryDark,
                Location = new Point(12, 40),
                Size = new Size(1030, 52),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Loading announcement..."
            };

            announcementPanel.Controls.Add(lblAnnouncementTitle);
            announcementPanel.Controls.Add(lblAnnouncement);

            var splitPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 14, 0, 0)
            };
            splitPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            splitPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));

            var recommendationPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12)
            };

            var lblRecommendationTitle = new Label
            {
                AutoSize = false,
                Font = AppTheme.FontSectionHeader,
                ForeColor = AppTheme.PrimaryDark,
                Text = "Recommended for you",
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 8)
            };

            lstRecommendations = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = AppTheme.PrimaryDark,
                ItemHeight = 24
            };

            recommendationPanel.Controls.Add(lblRecommendationTitle);
            recommendationPanel.Controls.Add(lstRecommendations);

            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12)
            };

            var lblGridHeader = new Label
            {
                AutoSize = true,
                Font = AppTheme.FontSectionHeader,
                ForeColor = AppTheme.PrimaryDark,
                Text = "Upcoming Event Schedule"
            };

            lblResultsSummary = new Label
            {
                AutoSize = true,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextMuted,
                Location = new Point(320, 0)
            };

            dgvEvents = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 18, 0, 0),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                Font = new Font("Segoe UI", 10F),
                // Improve readability: distribute columns, add padding and a slightly larger row height
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
                RowTemplate = { Height = 30 },
                DefaultCellStyle = { Padding = new Padding(6, 4, 6, 4) },
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = { Padding = new Padding(6, 4, 6, 4), BackColor = Color.White }
            };

            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", HeaderText = "Event", Width = 210, ReadOnly = true });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 130, ReadOnly = true });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date", Width = 120, ReadOnly = true });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Location", HeaderText = "Location", Width = 180, ReadOnly = true });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Priority", HeaderText = "Priority", Width = 80, ReadOnly = true });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 260, ReadOnly = true });

            gridPanel.Controls.Add(lblGridHeader);
            gridPanel.Controls.Add(lblResultsSummary);
            gridPanel.Controls.Add(dgvEvents);

            splitPanel.Controls.Add(recommendationPanel, 0, 0);
            splitPanel.Controls.Add(gridPanel, 1, 0);

            contentPanel.Controls.Add(splitPanel);
            contentPanel.Controls.Add(announcementPanel);
            contentPanel.Controls.Add(filterPanel);

            this.Controls.Add(contentPanel);
            this.Controls.Add(titlePanel);

            txtSearch.TextChanged += TxtSearch_TextChanged;
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            btnClearFilters.Click += BtnClearFilters_Click;
            btnBack.Click += BtnBack_Click;
        }

        private void LocalEventsForm_Load(object sender, EventArgs e)
        {
            var categories = LocalEventRepository.GetCategories();
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("All Categories");
            foreach (var category in categories)
            {
                cmbCategory.Items.Add(category);
            }
            cmbCategory.SelectedIndex = 0;
            UpdateDisplay();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void BtnClearFilters_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbCategory.SelectedIndex = 0;
            UpdateDisplay();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateDisplay()
        {
            string category = cmbCategory.SelectedItem as string ?? "All Categories";
            List<LocalEvent> filtered = LocalEventRepository.FilterEvents(txtSearch.Text, category);

            dgvEvents.Rows.Clear();
            foreach (var localEvent in filtered)
            {
                int rowIndex = dgvEvents.Rows.Add();
                dgvEvents.Rows[rowIndex].Cells["Title"].Value = localEvent.Title;
                dgvEvents.Rows[rowIndex].Cells["Category"].Value = localEvent.Category;
                dgvEvents.Rows[rowIndex].Cells["Date"].Value = localEvent.EventDate.ToString("ddd, dd MMM");
                dgvEvents.Rows[rowIndex].Cells["Location"].Value = localEvent.Location;
                dgvEvents.Rows[rowIndex].Cells["Priority"].Value = GetPriorityLabel(localEvent.Priority);
                dgvEvents.Rows[rowIndex].Cells["Description"].Value = localEvent.Description;
            }

            lblResultsSummary.Text = filtered.Count == 0
                ? "No events match your search."
                : "Showing " + filtered.Count + " events";

            lblResultsSummary.Location = new Point(320, 0);
            var recommendations = LocalEventRepository.GetRecommendedEvents(txtSearch.Text);
            lstRecommendations.Items.Clear();
            if (recommendations.Count == 0)
            {
                lstRecommendations.Items.Add("No recommendations yet. Try another keyword.");
            }
            else
            {
                foreach (var localEvent in recommendations)
                {
                    string eventText = string.Format("{0} ({1}) - {2}",
                        localEvent.Title,
                        localEvent.Category,
                        localEvent.EventDate.ToString("ddd, dd MMM"));
                    lstRecommendations.Items.Add(eventText);
                }
            }

            lblAnnouncement.Text = LocalEventRepository.GetNextAnnouncement();
        }

        private static string GetPriorityLabel(int priority)
        {
            if (priority >= 5) return "High";
            if (priority >= 3) return "Medium";
            return "Low";
        }
    }
}
