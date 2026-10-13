using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Forms
{
    public partial class ServiceRequestStatusForm : Form
    {
        private DataGridView dgvStatus;
        private Button btnBack;
        private Label lblTotalValue;
        private Label lblPendingValue;
        private Label lblResolvedValue;

        public ServiceRequestStatusForm()
        {
            InitializeComponent();
            this.Load += ServiceRequestStatusForm_Load;
        }

        private void InitializeComponent()
        {
            this.BackColor = AppTheme.Background;
            this.ClientSize = new Size(980, 700);
            this.MinimumSize = new Size(800, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Service Request Status";

            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = AppTheme.HeaderDark
            };

            var lblTitle = new Label
            {
                AutoSize = true,
                Font = AppTheme.FontTitle,
                ForeColor = Color.White,
                Location = new Point(24, 20),
                Text = "Service Request Status"
            };

            var lblSubtitle = new Label
            {
                AutoSize = true,
                Font = AppTheme.FontSubtitle,
                ForeColor = Color.FromArgb(210, 220, 230),
                Location = new Point(24, 56),
                Text = "Track the status of resident service requests and municipal actions"
            };

            titlePanel.Controls.Add(lblTitle);
            titlePanel.Controls.Add(lblSubtitle);

            var statPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 118,
                Padding = new Padding(16),
                BackColor = AppTheme.Background
            };

            var totalCard = CreateStatCard("Total", out lblTotalValue, AppTheme.Primary);
            totalCard.Location = new Point(16, 10);

            var pendingCard = CreateStatCard("Pending", out lblPendingValue, AppTheme.Warning);
            pendingCard.Location = new Point(226, 10);

            var resolvedCard = CreateStatCard("Resolved", out lblResolvedValue, AppTheme.Success);
            resolvedCard.Location = new Point(436, 10);

            statPanel.Controls.Add(totalCard);
            statPanel.Controls.Add(pendingCard);
            statPanel.Controls.Add(resolvedCard);

            var tablePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                BackColor = AppTheme.Background
            };

            dgvStatus = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = AppTheme.FontBody,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = AppTheme.PrimaryDark,
                    ForeColor = Color.White,
                    Font = AppTheme.FontBodyBold
                }
            };

            dgvStatus.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ref", HeaderText = "Ref #", Width = 80, ReadOnly = true });
            dgvStatus.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 160, ReadOnly = true });
            dgvStatus.Columns.Add(new DataGridViewTextBoxColumn { Name = "Location", HeaderText = "Location", Width = 220, ReadOnly = true });
            dgvStatus.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 170, ReadOnly = true });
            dgvStatus.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Reported", Width = 150, ReadOnly = true });
            dgvStatus.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 200, ReadOnly = true });

            btnBack = new Button
            {
                Text = "Back to Main Menu",
                Width = 170,
                Height = 35,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = AppTheme.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = AppTheme.FontButton,
                Margin = new Padding(0, 8, 0, 0)
            };
            btnBack.FlatAppearance.BorderSize = 0;

            tablePanel.Controls.Add(dgvStatus);
            tablePanel.Controls.Add(btnBack);

            this.Controls.Add(tablePanel);
            this.Controls.Add(statPanel);
            this.Controls.Add(titlePanel);

            btnBack.Click += BtnBack_Click;
        }

        private void ServiceRequestStatusForm_Load(object sender, EventArgs e)
        {
            RefreshStatusView();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshStatusView()
        {
            dgvStatus.Rows.Clear();
            var issues = IssueRepository.GetAllIssues();

            int pending = 0;
            int resolved = 0;

            foreach (var issue in issues)
            {
                if (issue.Status == IssueStatus.Resolved)
                {
                    resolved++;
                }
                else
                {
                    pending++;
                }

                int rowIndex = dgvStatus.Rows.Add();
                dgvStatus.Rows[rowIndex].Cells["Ref"].Value = "#" + issue.Id;
                dgvStatus.Rows[rowIndex].Cells["Category"].Value = issue.Category;
                dgvStatus.Rows[rowIndex].Cells["Location"].Value = issue.Location;
                dgvStatus.Rows[rowIndex].Cells["Status"].Value = IssueStatusHelper.GetShortText(issue.Status);
                dgvStatus.Rows[rowIndex].Cells["Date"].Value = issue.DateReported.ToString("dd MMM yyyy");
                dgvStatus.Rows[rowIndex].Cells["Description"].Value = issue.Description;
                dgvStatus.Rows[rowIndex].DefaultCellStyle.ForeColor = IssueStatusHelper.GetColor(issue.Status);
                dgvStatus.Rows[rowIndex].DefaultCellStyle.SelectionForeColor = IssueStatusHelper.GetColor(issue.Status);
            }

            if (dgvStatus.Rows.Count == 0)
            {
                int blankRow = dgvStatus.Rows.Add();
                dgvStatus.Rows[blankRow].Cells["Ref"].Value = "-";
                dgvStatus.Rows[blankRow].Cells["Category"].Value = "No reports yet";
                dgvStatus.Rows[blankRow].Cells["Location"].Value = "-";
                dgvStatus.Rows[blankRow].Cells["Status"].Value = "-";
                dgvStatus.Rows[blankRow].Cells["Date"].Value = "-";
                dgvStatus.Rows[blankRow].Cells["Description"].Value = "Report an issue to see it here.";
            }

            lblTotalValue.Text = IssueRepository.GetTotalCount().ToString();
            lblPendingValue.Text = pending.ToString();
            lblResolvedValue.Text = resolved.ToString();
        }

        private static Panel CreateStatCard(string title, out Label valueLabel, Color accentColor)
        {
            var panel = new Panel
            {
                Size = new Size(190, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextMuted,
                Location = new Point(14, 14),
                AutoSize = true,
                Text = title
            };

            valueLabel = new Label
            {
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = accentColor,
                Location = new Point(14, 34),
                AutoSize = true,
                Text = "0"
            };

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(valueLabel);
            return panel;
        }
    }
}
