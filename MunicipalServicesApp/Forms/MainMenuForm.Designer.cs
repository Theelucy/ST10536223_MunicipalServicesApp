using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class MainMenuForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel tlpDashboard;

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblAppTitle;
        private System.Windows.Forms.Label lblAppSubtitle;
        private System.Windows.Forms.Label lblDateTime;

        private System.Windows.Forms.Panel pnlStats;
        private System.Windows.Forms.Panel pnlCardTotal;
        private System.Windows.Forms.Label lblCardTotalValue;
        private System.Windows.Forms.Label lblCardTotalCaption;
        private System.Windows.Forms.Panel pnlCardPending;
        private System.Windows.Forms.Label lblCardPendingValue;
        private System.Windows.Forms.Label lblCardPendingCaption;
        private System.Windows.Forms.Panel pnlCardResolved;
        private System.Windows.Forms.Label lblCardResolvedValue;
        private System.Windows.Forms.Label lblCardResolvedCaption;

        private System.Windows.Forms.Label lblQuickActionsHeader;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnReportIssues;
        private System.Windows.Forms.Button btnLocalEvents;
        private System.Windows.Forms.Button btnServiceStatus;

        private System.Windows.Forms.Label lblRecentHeader;
        private System.Windows.Forms.Panel pnlRecentContainer;
        private System.Windows.Forms.ListView lvRecentIssues;
        private System.Windows.Forms.ColumnHeader chRef;
        private System.Windows.Forms.ColumnHeader chCategory;
        private System.Windows.Forms.ColumnHeader chLocation;
        private System.Windows.Forms.ColumnHeader chStatus;
        private System.Windows.Forms.ColumnHeader chDate;
        private System.Windows.Forms.Label lblEmptyState;

        private System.Windows.Forms.Label lblFooter;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tlpDashboard = new System.Windows.Forms.TableLayoutPanel();

            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.lblAppSubtitle = new System.Windows.Forms.Label();
            this.lblAppTitle = new System.Windows.Forms.Label();

            this.pnlStats = new System.Windows.Forms.Panel();
            this.pnlCardTotal = new System.Windows.Forms.Panel();
            this.lblCardTotalValue = new System.Windows.Forms.Label();
            this.lblCardTotalCaption = new System.Windows.Forms.Label();
            this.pnlCardPending = new System.Windows.Forms.Panel();
            this.lblCardPendingValue = new System.Windows.Forms.Label();
            this.lblCardPendingCaption = new System.Windows.Forms.Label();
            this.pnlCardResolved = new System.Windows.Forms.Panel();
            this.lblCardResolvedValue = new System.Windows.Forms.Label();
            this.lblCardResolvedCaption = new System.Windows.Forms.Label();

            this.lblQuickActionsHeader = new System.Windows.Forms.Label();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnReportIssues = new System.Windows.Forms.Button();
            this.btnLocalEvents = new System.Windows.Forms.Button();
            this.btnServiceStatus = new System.Windows.Forms.Button();

            this.lblRecentHeader = new System.Windows.Forms.Label();
            this.pnlRecentContainer = new System.Windows.Forms.Panel();
            this.lvRecentIssues = new System.Windows.Forms.ListView();
            this.chRef = new System.Windows.Forms.ColumnHeader();
            this.chCategory = new System.Windows.Forms.ColumnHeader();
            this.chLocation = new System.Windows.Forms.ColumnHeader();
            this.chStatus = new System.Windows.Forms.ColumnHeader();
            this.chDate = new System.Windows.Forms.ColumnHeader();
            this.lblEmptyState = new System.Windows.Forms.Label();

            this.lblFooter = new System.Windows.Forms.Label();

            this.tlpDashboard.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlStats.SuspendLayout();
            this.pnlCardTotal.SuspendLayout();
            this.pnlCardPending.SuspendLayout();
            this.pnlCardResolved.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.pnlRecentContainer.SuspendLayout();
            this.SuspendLayout();
            //
            // tlpDashboard
            //
            this.tlpDashboard.ColumnCount = 1;
            this.tlpDashboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDashboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDashboard.RowCount = 7;
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpDashboard.Location = new System.Drawing.Point(0, 0);
            this.tlpDashboard.Name = "tlpDashboard";
            this.tlpDashboard.Padding = new System.Windows.Forms.Padding(0);
            this.tlpDashboard.Size = new System.Drawing.Size(960, 700);
            this.tlpDashboard.TabIndex = 0;
            this.tlpDashboard.Controls.Add(this.pnlHeader, 0, 0);
            this.tlpDashboard.Controls.Add(this.pnlStats, 0, 1);
            this.tlpDashboard.Controls.Add(this.lblQuickActionsHeader, 0, 2);
            this.tlpDashboard.Controls.Add(this.pnlActions, 0, 3);
            this.tlpDashboard.Controls.Add(this.lblRecentHeader, 0, 4);
            this.tlpDashboard.Controls.Add(this.pnlRecentContainer, 0, 5);
            this.tlpDashboard.Controls.Add(this.lblFooter, 0, 6);
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(31, 45, 61);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Controls.Add(this.lblDateTime);
            this.pnlHeader.Controls.Add(this.lblAppSubtitle);
            this.pnlHeader.Controls.Add(this.lblAppTitle);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(960, 88);
            this.pnlHeader.TabIndex = 0;
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Location = new System.Drawing.Point(28, 14);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(300, 32);
            this.lblAppTitle.TabIndex = 0;
            this.lblAppTitle.Text = "Municipal Services";
            //
            // lblAppSubtitle
            //
            this.lblAppSubtitle.AutoSize = true;
            this.lblAppSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblAppSubtitle.ForeColor = System.Drawing.Color.FromArgb(189, 195, 199);
            this.lblAppSubtitle.Location = new System.Drawing.Point(30, 52);
            this.lblAppSubtitle.Name = "lblAppSubtitle";
            this.lblAppSubtitle.Size = new System.Drawing.Size(300, 17);
            this.lblAppSubtitle.TabIndex = 1;
            this.lblAppSubtitle.Text = "Report issues. Stay informed. Get updates.";
            //
            // lblDateTime
            //
            this.lblDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblDateTime.ForeColor = System.Drawing.Color.FromArgb(189, 195, 199);
            this.lblDateTime.Location = new System.Drawing.Point(700, 34);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(230, 17);
            this.lblDateTime.TabIndex = 2;
            this.lblDateTime.Text = "Today";
            this.lblDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlStats
            //
            this.pnlStats.BackColor = System.Drawing.Color.FromArgb(241, 244, 247);
            this.pnlStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlStats.Controls.Add(this.pnlCardResolved);
            this.pnlStats.Controls.Add(this.pnlCardPending);
            this.pnlStats.Controls.Add(this.pnlCardTotal);
            this.pnlStats.Location = new System.Drawing.Point(0, 88);
            this.pnlStats.Margin = new System.Windows.Forms.Padding(0);
            this.pnlStats.Name = "pnlStats";
            this.pnlStats.Size = new System.Drawing.Size(960, 128);
            this.pnlStats.TabIndex = 1;
            //
            // pnlCardTotal
            //
            this.pnlCardTotal.BackColor = System.Drawing.Color.FromArgb(232, 244, 253);
            this.pnlCardTotal.Controls.Add(this.lblCardTotalValue);
            this.pnlCardTotal.Controls.Add(this.lblCardTotalCaption);
            this.pnlCardTotal.Location = new System.Drawing.Point(28, 14);
            this.pnlCardTotal.Name = "pnlCardTotal";
            this.pnlCardTotal.Size = new System.Drawing.Size(280, 100);
            this.pnlCardTotal.TabIndex = 0;
            //
            // lblCardTotalValue
            //
            this.lblCardTotalValue.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblCardTotalValue.ForeColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.lblCardTotalValue.Location = new System.Drawing.Point(18, 14);
            this.lblCardTotalValue.Name = "lblCardTotalValue";
            this.lblCardTotalValue.Size = new System.Drawing.Size(160, 44);
            this.lblCardTotalValue.TabIndex = 0;
            this.lblCardTotalValue.Text = "0";
            //
            // lblCardTotalCaption
            //
            this.lblCardTotalCaption.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblCardTotalCaption.ForeColor = System.Drawing.Color.FromArgb(90, 100, 110);
            this.lblCardTotalCaption.Location = new System.Drawing.Point(19, 62);
            this.lblCardTotalCaption.Name = "lblCardTotalCaption";
            this.lblCardTotalCaption.Size = new System.Drawing.Size(240, 30);
            this.lblCardTotalCaption.TabIndex = 1;
            this.lblCardTotalCaption.Text = "Total Issues Reported";
            //
            // pnlCardPending
            //
            this.pnlCardPending.BackColor = System.Drawing.Color.FromArgb(253, 245, 230);
            this.pnlCardPending.Controls.Add(this.lblCardPendingValue);
            this.pnlCardPending.Controls.Add(this.lblCardPendingCaption);
            this.pnlCardPending.Location = new System.Drawing.Point(340, 14);
            this.pnlCardPending.Name = "pnlCardPending";
            this.pnlCardPending.Size = new System.Drawing.Size(280, 100);
            this.pnlCardPending.TabIndex = 1;
            //
            // lblCardPendingValue
            //
            this.lblCardPendingValue.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblCardPendingValue.ForeColor = System.Drawing.Color.FromArgb(243, 156, 18);
            this.lblCardPendingValue.Location = new System.Drawing.Point(18, 14);
            this.lblCardPendingValue.Name = "lblCardPendingValue";
            this.lblCardPendingValue.Size = new System.Drawing.Size(160, 44);
            this.lblCardPendingValue.TabIndex = 0;
            this.lblCardPendingValue.Text = "0";
            //
            // lblCardPendingCaption
            //
            this.lblCardPendingCaption.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblCardPendingCaption.ForeColor = System.Drawing.Color.FromArgb(90, 100, 110);
            this.lblCardPendingCaption.Location = new System.Drawing.Point(19, 62);
            this.lblCardPendingCaption.Name = "lblCardPendingCaption";
            this.lblCardPendingCaption.Size = new System.Drawing.Size(240, 30);
            this.lblCardPendingCaption.TabIndex = 1;
            this.lblCardPendingCaption.Text = "In Progress / Pending";
            //
            // pnlCardResolved
            //
            this.pnlCardResolved.BackColor = System.Drawing.Color.FromArgb(230, 250, 239);
            this.pnlCardResolved.Controls.Add(this.lblCardResolvedValue);
            this.pnlCardResolved.Controls.Add(this.lblCardResolvedCaption);
            this.pnlCardResolved.Location = new System.Drawing.Point(652, 14);
            this.pnlCardResolved.Name = "pnlCardResolved";
            this.pnlCardResolved.Size = new System.Drawing.Size(280, 100);
            this.pnlCardResolved.TabIndex = 2;
            //
            // lblCardResolvedValue
            //
            this.lblCardResolvedValue.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblCardResolvedValue.ForeColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.lblCardResolvedValue.Location = new System.Drawing.Point(18, 14);
            this.lblCardResolvedValue.Name = "lblCardResolvedValue";
            this.lblCardResolvedValue.Size = new System.Drawing.Size(160, 44);
            this.lblCardResolvedValue.TabIndex = 0;
            this.lblCardResolvedValue.Text = "0";
            //
            // lblCardResolvedCaption
            //
            this.lblCardResolvedCaption.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblCardResolvedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 100, 110);
            this.lblCardResolvedCaption.Location = new System.Drawing.Point(19, 62);
            this.lblCardResolvedCaption.Name = "lblCardResolvedCaption";
            this.lblCardResolvedCaption.Size = new System.Drawing.Size(240, 30);
            this.lblCardResolvedCaption.TabIndex = 1;
            this.lblCardResolvedCaption.Text = "Resolved";
            //
            // lblQuickActionsHeader
            //
            this.lblQuickActionsHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblQuickActionsHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblQuickActionsHeader.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.lblQuickActionsHeader.Location = new System.Drawing.Point(0, 216);
            this.lblQuickActionsHeader.Margin = new System.Windows.Forms.Padding(0);
            this.lblQuickActionsHeader.Name = "lblQuickActionsHeader";
            this.lblQuickActionsHeader.Padding = new System.Windows.Forms.Padding(28, 8, 0, 0);
            this.lblQuickActionsHeader.Size = new System.Drawing.Size(960, 34);
            this.lblQuickActionsHeader.TabIndex = 2;
            this.lblQuickActionsHeader.Text = "Quick Actions";
            //
            // pnlActions
            //
            this.pnlActions.BackColor = System.Drawing.Color.FromArgb(241, 244, 247);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlActions.Controls.Add(this.btnServiceStatus);
            this.pnlActions.Controls.Add(this.btnLocalEvents);
            this.pnlActions.Controls.Add(this.btnReportIssues);
            this.pnlActions.Location = new System.Drawing.Point(0, 250);
            this.pnlActions.Margin = new System.Windows.Forms.Padding(0);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(960, 150);
            this.pnlActions.TabIndex = 3;
            //
            // btnReportIssues
            //
            this.btnReportIssues.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnReportIssues.FlatAppearance.BorderSize = 0;
            this.btnReportIssues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReportIssues.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnReportIssues.ForeColor = System.Drawing.Color.White;
            this.btnReportIssues.Location = new System.Drawing.Point(28, 12);
            this.btnReportIssues.Name = "btnReportIssues";
            this.btnReportIssues.Size = new System.Drawing.Size(280, 126);
            this.btnReportIssues.TabIndex = 0;
            this.btnReportIssues.Text = "Report Issues\n\nAvailable now";
            this.btnReportIssues.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnReportIssues.UseVisualStyleBackColor = false;
            this.btnReportIssues.Click += new System.EventHandler(this.btnReportIssues_Click);
            //
            // btnLocalEvents
            //
            this.btnLocalEvents.BackColor = System.Drawing.Color.FromArgb(210, 214, 218);
            this.btnLocalEvents.Enabled = false;
            this.btnLocalEvents.FlatAppearance.BorderSize = 0;
            this.btnLocalEvents.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLocalEvents.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnLocalEvents.ForeColor = System.Drawing.Color.FromArgb(110, 118, 125);
            this.btnLocalEvents.Location = new System.Drawing.Point(340, 12);
            this.btnLocalEvents.Name = "btnLocalEvents";
            this.btnLocalEvents.Size = new System.Drawing.Size(280, 126);
            this.btnLocalEvents.TabIndex = 1;
            this.btnLocalEvents.Text = "Local Events and\nAnnouncements\n\nComing in Part 2";
            this.btnLocalEvents.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnLocalEvents.UseVisualStyleBackColor = false;
            //
            // btnServiceStatus
            //
            this.btnServiceStatus.BackColor = System.Drawing.Color.FromArgb(210, 214, 218);
            this.btnServiceStatus.Enabled = false;
            this.btnServiceStatus.FlatAppearance.BorderSize = 0;
            this.btnServiceStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServiceStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnServiceStatus.ForeColor = System.Drawing.Color.FromArgb(110, 118, 125);
            this.btnServiceStatus.Location = new System.Drawing.Point(652, 12);
            this.btnServiceStatus.Name = "btnServiceStatus";
            this.btnServiceStatus.Size = new System.Drawing.Size(280, 126);
            this.btnServiceStatus.TabIndex = 2;
            this.btnServiceStatus.Text = "Service Request\nStatus\n\nComing in Part 2";
            this.btnServiceStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnServiceStatus.UseVisualStyleBackColor = false;
            //
            // lblRecentHeader
            //
            this.lblRecentHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRecentHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblRecentHeader.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.lblRecentHeader.Location = new System.Drawing.Point(0, 400);
            this.lblRecentHeader.Margin = new System.Windows.Forms.Padding(0);
            this.lblRecentHeader.Name = "lblRecentHeader";
            this.lblRecentHeader.Padding = new System.Windows.Forms.Padding(28, 8, 0, 0);
            this.lblRecentHeader.Size = new System.Drawing.Size(960, 34);
            this.lblRecentHeader.TabIndex = 4;
            this.lblRecentHeader.Text = "Recently Reported Issues";
            //
            // pnlRecentContainer
            //
            this.pnlRecentContainer.BackColor = System.Drawing.Color.FromArgb(241, 244, 247);
            this.pnlRecentContainer.Controls.Add(this.lvRecentIssues);
            this.pnlRecentContainer.Controls.Add(this.lblEmptyState);
            this.pnlRecentContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRecentContainer.Location = new System.Drawing.Point(0, 434);
            this.pnlRecentContainer.Margin = new System.Windows.Forms.Padding(0);
            this.pnlRecentContainer.Name = "pnlRecentContainer";
            this.pnlRecentContainer.Padding = new System.Windows.Forms.Padding(28, 0, 28, 10);
            this.pnlRecentContainer.Size = new System.Drawing.Size(960, 210);
            this.pnlRecentContainer.TabIndex = 5;
            //
            // lvRecentIssues
            //
            this.lvRecentIssues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chRef,
            this.chCategory,
            this.chLocation,
            this.chStatus,
            this.chDate});
            this.lvRecentIssues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRecentIssues.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lvRecentIssues.FullRowSelect = true;
            this.lvRecentIssues.GridLines = true;
            this.lvRecentIssues.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvRecentIssues.Location = new System.Drawing.Point(28, 0);
            this.lvRecentIssues.MultiSelect = false;
            this.lvRecentIssues.Name = "lvRecentIssues";
            this.lvRecentIssues.Size = new System.Drawing.Size(904, 200);
            this.lvRecentIssues.TabIndex = 0;
            this.lvRecentIssues.UseCompatibleStateImageBehavior = false;
            this.lvRecentIssues.View = System.Windows.Forms.View.Details;
            //
            // chRef
            //
            this.chRef.Text = "Ref #";
            this.chRef.Width = 70;
            //
            // chCategory
            //
            this.chCategory.Text = "Category";
            this.chCategory.Width = 170;
            //
            // chLocation
            //
            this.chLocation.Text = "Location";
            this.chLocation.Width = 300;
            //
            // chStatus
            //
            this.chStatus.Text = "Status";
            this.chStatus.Width = 140;
            //
            // chDate
            //
            this.chDate.Text = "Reported";
            this.chDate.Width = 190;
            //
            // lblEmptyState
            //
            this.lblEmptyState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEmptyState.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.lblEmptyState.ForeColor = System.Drawing.Color.FromArgb(150, 156, 162);
            this.lblEmptyState.Location = new System.Drawing.Point(28, 0);
            this.lblEmptyState.Name = "lblEmptyState";
            this.lblEmptyState.Size = new System.Drawing.Size(904, 200);
            this.lblEmptyState.TabIndex = 1;
            this.lblEmptyState.Text = "No issues reported yet. Click \"Report Issues\" to submit your first report.";
            this.lblEmptyState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEmptyState.Visible = false;
            //
            // lblFooter
            //
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFooter.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFooter.ForeColor = System.Drawing.Color.Gray;
            this.lblFooter.Location = new System.Drawing.Point(0, 644);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Padding = new System.Windows.Forms.Padding(28, 0, 0, 0);
            this.lblFooter.Size = new System.Drawing.Size(960, 28);
            this.lblFooter.TabIndex = 6;
            this.lblFooter.Text = "Municipal Services Application  -  PoE Part 1";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MainMenuForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(241, 244, 247);
            this.ClientSize = new System.Drawing.Size(960, 700);
            this.Controls.Add(this.tlpDashboard);
            this.MinimumSize = new System.Drawing.Size(820, 600);
            this.Name = "MainMenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Municipal Services Application - Home";
            this.Load += new System.EventHandler(this.MainMenuForm_Load);
            this.Activated += new System.EventHandler(this.MainMenuForm_Activated);
            this.tlpDashboard.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlStats.ResumeLayout(false);
            this.pnlCardTotal.ResumeLayout(false);
            this.pnlCardPending.ResumeLayout(false);
            this.pnlCardResolved.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.pnlRecentContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
