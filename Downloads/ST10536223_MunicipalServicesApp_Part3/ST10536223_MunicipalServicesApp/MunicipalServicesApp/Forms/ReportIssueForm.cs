using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Forms
{
    /// <summary>
    /// "Report Issues" screen. Lets a resident capture a location, category,
    /// description and optional media for a municipal issue, and shows a
    /// live "My Reported Issues" grid of everything reported this session.
    ///
    /// Engagement strategy implemented: REAL-TIME FEEDBACK / PROGRESS TRACKING.
    /// - While the resident is filling in the form, a progress bar fills up as
    ///   each required field is completed, giving immediate positive feedback
    ///   and encouraging them to finish the report.
    /// - After submission, a status timer simulates the issue moving through
    ///   its municipal lifecycle (Submitted -> Under Review -> In Progress ->
    ///   Resolved). Both the status label and the matching row in the "My
    ///   Reported Issues" grid update together, so the resident can see - in
    ///   real time - that their report is being acted on.
    /// </summary>
    public partial class ReportIssueForm : Form
    {
        // List<T> is used to store attachment file paths for the current report:
        // the number of attachments is unknown up front and only needs to be
        // added to and iterated over, which List<string> supports directly.
        private readonly List<string> _attachedFilePaths = new List<string>();

        private readonly IssueStatus[] _statusSequence =
        {
            IssueStatus.Submitted,
            IssueStatus.UnderReview,
            IssueStatus.InProgress,
            IssueStatus.Resolved
        };

        private int _currentStatusIndex;
        private Issue _currentIssue;
        private DataGridViewRow _currentReportRow;

        public ReportIssueForm()
        {
            InitializeComponent();
        }

        private void ReportIssueForm_Load(object sender, EventArgs e)
        {
            SetUpReportsGrid();
            LoadExistingReportsIntoGrid();
            UpdateProgress();
        }

        private void btnBackTop_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Fires whenever the user edits Location, Category or Description.
        /// Recalculates the completion progress bar - this is the "encourage
        /// active participation" half of the chosen engagement strategy.
        /// </summary>
        private void InputChanged(object sender, EventArgs e)
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            int completed = 0;
            const int totalFields = 3;

            if (!string.IsNullOrWhiteSpace(txtLocation.Text)) completed++;
            if (cmbCategory.SelectedIndex > 0) completed++;
            if (!string.IsNullOrWhiteSpace(rtbDescription.Text)) completed++;

            int percent = (int)Math.Round((completed / (double)totalFields) * 100);
            progressBarCompletion.Value = Math.Min(percent, 100);

            if (percent == 0)
            {
                lblProgressPercent.Text = "0% complete - fill in the fields above to continue";
            }
            else if (percent < 100)
            {
                lblProgressPercent.Text = percent + "% complete - keep going!";
            }
            else
            {
                lblProgressPercent.Text = "100% complete - ready to submit";
            }
        }

        private void btnAttachMedia_Click(object sender, EventArgs e)
        {
            if (openFileDialogMedia.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDialogMedia.FileNames)
                {
                    if (!_attachedFilePaths.Contains(fileName))
                    {
                        _attachedFilePaths.Add(fileName);
                        lstAttachments.Items.Add(Path.GetFileName(fileName));
                    }
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string location = txtLocation.Text.Trim();
            string description = rtbDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(location))
            {
                MessageBox.Show(this, "Please enter the location of the issue.",
                    "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLocation.Focus();
                return;
            }

            if (cmbCategory.SelectedIndex <= 0)
            {
                MessageBox.Show(this, "Please select a category for the issue.",
                    "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show(this, "Please provide a description of the issue.",
                    "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbDescription.Focus();
                return;
            }

            string category = cmbCategory.SelectedItem.ToString();

            _currentIssue = IssueRepository.AddIssue(location, category, description,
                new List<string>(_attachedFilePaths));

            _currentReportRow = AddReportRow(_currentIssue);

            MessageBox.Show(this,
                "Thank you! Your issue has been reported successfully." + Environment.NewLine +
                "Reference number: #" + _currentIssue.Id,
                "Report submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SetFormEnabled(false);
            btnSubmit.Visible = false;
            btnReportAnother.Visible = true;

            _currentStatusIndex = 0;
            UpdateStatusLabel();
            statusTimer.Start();
        }

        private void SetFormEnabled(bool enabled)
        {
            txtLocation.Enabled = enabled;
            cmbCategory.Enabled = enabled;
            rtbDescription.Enabled = enabled;
            btnAttachMedia.Enabled = enabled;
        }

        /// <summary>
        /// Simulates the municipality processing the report, ticking the
        /// status forward every few seconds so the resident sees continuous,
        /// real-time progress on their submitted issue - both on the status
        /// label and in the matching row of the reports grid.
        /// </summary>
        private void statusTimer_Tick(object sender, EventArgs e)
        {
            if (_currentStatusIndex >= _statusSequence.Length - 1)
            {
                statusTimer.Stop();
                return;
            }

            _currentStatusIndex++;
            if (_currentIssue != null)
            {
                _currentIssue.Status = _statusSequence[_currentStatusIndex];
            }
            UpdateStatusLabel();
            UpdateReportRowStatus(_currentReportRow, _statusSequence[_currentStatusIndex]);

            if (_statusSequence[_currentStatusIndex] == IssueStatus.Resolved)
            {
                statusTimer.Stop();
            }
        }

        private void UpdateStatusLabel()
        {
            IssueStatus status = _statusSequence[_currentStatusIndex];
            string text;

            switch (status)
            {
                case IssueStatus.Submitted:
                    text = "Status: Submitted - your report has been received.";
                    break;
                case IssueStatus.UnderReview:
                    text = "Status: Under Review - a municipal officer is reviewing your report.";
                    break;
                case IssueStatus.InProgress:
                    text = "Status: In Progress - work has started on resolving this issue.";
                    break;
                default:
                    text = "Status: Resolved - thank you for helping improve your community!";
                    break;
            }

            lblStatus.Text = text;
            lblStatus.ForeColor = IssueStatusHelper.GetColor(status);
        }

        private void btnReportAnother_Click(object sender, EventArgs e)
        {
            statusTimer.Stop();

            txtLocation.Clear();
            cmbCategory.SelectedIndex = 0;
            rtbDescription.Clear();
            lstAttachments.Items.Clear();
            _attachedFilePaths.Clear();
            _currentIssue = null;
            _currentReportRow = null;

            lblStatus.Text = "Status: not submitted yet";
            lblStatus.ForeColor = Color.FromArgb(127, 140, 141);

            SetFormEnabled(true);
            btnReportAnother.Visible = false;
            btnSubmit.Visible = true;

            UpdateProgress();
            txtLocation.Focus();
        }

        // ----------------------------------------------------------------
        // "My Reported Issues" grid
        // ----------------------------------------------------------------

        private void SetUpReportsGrid()
        {
            dgvMyReports.AutoGenerateColumns = false;
            dgvMyReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRef",
                HeaderText = "Ref #",
                Width = 60
            });
            dgvMyReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCategory",
                HeaderText = "Category",
                Width = 150,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            });
            dgvMyReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLocation",
                HeaderText = "Location",
                Width = 150,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvMyReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStatus",
                HeaderText = "Status",
                Width = 110
            });
            dgvMyReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDate",
                HeaderText = "Reported",
                Width = 110
            });
        }

        /// <summary>
        /// Populates the grid with any issues already reported earlier in
        /// this application session (IssueRepository persists across forms
        /// for as long as the app is running).
        /// </summary>
        private void LoadExistingReportsIntoGrid()
        {
            foreach (Issue issue in IssueRepository.GetAllIssues())
            {
                AddReportRow(issue);
            }
        }

        private DataGridViewRow AddReportRow(Issue issue)
        {
            int rowIndex = dgvMyReports.Rows.Add(
                "#" + issue.Id,
                issue.Category,
                issue.Location,
                IssueStatusHelper.GetShortText(issue.Status),
                issue.DateReported.ToString("dd MMM, HH:mm"));

            DataGridViewRow row = dgvMyReports.Rows[rowIndex];
            row.Cells["colStatus"].Style.ForeColor = IssueStatusHelper.GetColor(issue.Status);
            row.Cells["colStatus"].Style.Font = new Font(dgvMyReports.Font, FontStyle.Bold);
            return row;
        }

        private void UpdateReportRowStatus(DataGridViewRow row, IssueStatus status)
        {
            if (row == null)
            {
                return;
            }

            row.Cells["colStatus"].Value = IssueStatusHelper.GetShortText(status);
            row.Cells["colStatus"].Style.ForeColor = IssueStatusHelper.GetColor(status);
        }
    }
}
