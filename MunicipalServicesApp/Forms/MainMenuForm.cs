using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Forms
{
    /// <summary>
    /// The application's home screen: a small dashboard showing live stats
    /// and the most recently reported issues, plus the three top-level
    /// municipal service tasks. Only "Report Issues" is implemented in this
    /// part of the PoE; the other two are visibly present but disabled, as
    /// required by the brief.
    /// </summary>
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dddd, d MMMM yyyy");
            RefreshDashboard();
        }

        /// <summary>
        /// Refreshes the stat cards and recent-issues list every time this
        /// form becomes the active window - including when the user returns
        /// here after closing the Report Issues screen - so the dashboard
        /// always reflects the latest reports.
        /// </summary>
        private void MainMenuForm_Activated(object sender, EventArgs e)
        {
            RefreshDashboard();
        }

        private void RefreshDashboard()
        {
            int total = IssueRepository.GetTotalCount();
            int resolved = IssueRepository.GetCountByStatus(IssueStatus.Resolved);
            int pending = total - resolved;

            lblCardTotalValue.Text = total.ToString();
            lblCardPendingValue.Text = pending.ToString();
            lblCardResolvedValue.Text = resolved.ToString();

            RefreshRecentIssuesList();
        }

        private void RefreshRecentIssuesList()
        {
            lvRecentIssues.Items.Clear();
            List<Issue> issues = IssueRepository.GetAllIssues();

            if (issues.Count == 0)
            {
                lvRecentIssues.Visible = false;
                lblEmptyState.Visible = true;
                return;
            }

            lblEmptyState.Visible = false;
            lvRecentIssues.Visible = true;

            // Show the five most recently reported issues, newest first.
            int start = issues.Count - 1;
            int shown = 0;
            for (int i = start; i >= 0 && shown < 5; i--, shown++)
            {
                Issue issue = issues[i];
                var item = new ListViewItem("#" + issue.Id);
                item.SubItems.Add(issue.Category);
                item.SubItems.Add(issue.Location);
                item.SubItems.Add(IssueStatusHelper.GetShortText(issue.Status));
                item.SubItems.Add(issue.DateReported.ToString("g"));
                item.ForeColor = IssueStatusHelper.GetColor(issue.Status);
                lvRecentIssues.Items.Add(item);
            }
        }

        private void btnReportIssues_Click(object sender, EventArgs e)
        {
            using (var reportForm = new ReportIssueForm())
            {
                this.Hide();
                reportForm.ShowDialog();
                this.Show();
            }

            RefreshDashboard();
        }
    }
}
