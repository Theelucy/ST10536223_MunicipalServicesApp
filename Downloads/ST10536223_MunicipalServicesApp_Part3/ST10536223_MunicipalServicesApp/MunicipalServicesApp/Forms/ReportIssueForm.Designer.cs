namespace MunicipalServicesApp.Forms
{
    partial class ReportIssueForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblFormTitle;
        private System.Windows.Forms.Button btnBackTop;

        private System.Windows.Forms.SplitContainer splitMain;

        private System.Windows.Forms.Panel pnlFormFields;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.RichTextBox rtbDescription;
        private System.Windows.Forms.Label lblAttachments;
        private System.Windows.Forms.Button btnAttachMedia;
        private System.Windows.Forms.ListBox lstAttachments;
        private System.Windows.Forms.Label lblEngagementHeader;
        private System.Windows.Forms.ProgressBar progressBarCompletion;
        private System.Windows.Forms.Label lblProgressPercent;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnReportAnother;

        private System.Windows.Forms.Panel pnlReports;
        private System.Windows.Forms.Label lblReportsHeader;
        private System.Windows.Forms.DataGridView dgvMyReports;

        private System.Windows.Forms.OpenFileDialog openFileDialogMedia;
        private System.Windows.Forms.Timer statusTimer;

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
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnBackTop = new System.Windows.Forms.Button();
            this.lblFormTitle = new System.Windows.Forms.Label();

            this.splitMain = new System.Windows.Forms.SplitContainer();

            this.pnlFormFields = new System.Windows.Forms.Panel();
            this.lblLocation = new System.Windows.Forms.Label();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.rtbDescription = new System.Windows.Forms.RichTextBox();
            this.lblAttachments = new System.Windows.Forms.Label();
            this.btnAttachMedia = new System.Windows.Forms.Button();
            this.lstAttachments = new System.Windows.Forms.ListBox();
            this.lblEngagementHeader = new System.Windows.Forms.Label();
            this.progressBarCompletion = new System.Windows.Forms.ProgressBar();
            this.lblProgressPercent = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnReportAnother = new System.Windows.Forms.Button();

            this.pnlReports = new System.Windows.Forms.Panel();
            this.lblReportsHeader = new System.Windows.Forms.Label();
            this.dgvMyReports = new System.Windows.Forms.DataGridView();

            this.openFileDialogMedia = new System.Windows.Forms.OpenFileDialog();
            this.statusTimer = new System.Windows.Forms.Timer(this.components);

            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.pnlFormFields.SuspendLayout();
            this.pnlReports.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyReports)).BeginInit();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(31, 45, 61);
            this.pnlHeader.Controls.Add(this.btnBackTop);
            this.pnlHeader.Controls.Add(this.lblFormTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1040, 64);
            this.pnlHeader.TabIndex = 0;
            //
            // lblFormTitle
            //
            this.lblFormTitle.AutoSize = true;
            this.lblFormTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblFormTitle.ForeColor = System.Drawing.Color.White;
            this.lblFormTitle.Location = new System.Drawing.Point(24, 18);
            this.lblFormTitle.Name = "lblFormTitle";
            this.lblFormTitle.Size = new System.Drawing.Size(160, 25);
            this.lblFormTitle.TabIndex = 0;
            this.lblFormTitle.Text = "Report an Issue";
            //
            // btnBackTop
            //
            this.btnBackTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackTop.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.btnBackTop.FlatAppearance.BorderSize = 0;
            this.btnBackTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackTop.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBackTop.ForeColor = System.Drawing.Color.White;
            this.btnBackTop.Location = new System.Drawing.Point(880, 15);
            this.btnBackTop.Name = "btnBackTop";
            this.btnBackTop.Size = new System.Drawing.Size(140, 32);
            this.btnBackTop.TabIndex = 1;
            this.btnBackTop.Text = "Back to Main Menu";
            this.btnBackTop.UseVisualStyleBackColor = false;
            this.btnBackTop.Click += new System.EventHandler(this.btnBackTop_Click);
            //
            // splitMain
            //
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitMain.IsSplitterFixed = true;
            this.splitMain.Location = new System.Drawing.Point(0, 64);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Controls.Add(this.pnlFormFields);
            this.splitMain.Panel2.Controls.Add(this.pnlReports);
            this.splitMain.Size = new System.Drawing.Size(1040, 636);
            this.splitMain.SplitterDistance = 486;
            this.splitMain.SplitterWidth = 6;
            this.splitMain.TabIndex = 1;
            //
            // pnlFormFields
            //
            this.pnlFormFields.AutoScroll = true;
            this.pnlFormFields.BackColor = System.Drawing.Color.White;
            this.pnlFormFields.Controls.Add(this.lblLocation);
            this.pnlFormFields.Controls.Add(this.txtLocation);
            this.pnlFormFields.Controls.Add(this.lblCategory);
            this.pnlFormFields.Controls.Add(this.cmbCategory);
            this.pnlFormFields.Controls.Add(this.lblDescription);
            this.pnlFormFields.Controls.Add(this.rtbDescription);
            this.pnlFormFields.Controls.Add(this.lblAttachments);
            this.pnlFormFields.Controls.Add(this.btnAttachMedia);
            this.pnlFormFields.Controls.Add(this.lstAttachments);
            this.pnlFormFields.Controls.Add(this.lblEngagementHeader);
            this.pnlFormFields.Controls.Add(this.progressBarCompletion);
            this.pnlFormFields.Controls.Add(this.lblProgressPercent);
            this.pnlFormFields.Controls.Add(this.lblStatus);
            this.pnlFormFields.Controls.Add(this.btnSubmit);
            this.pnlFormFields.Controls.Add(this.btnReportAnother);
            this.pnlFormFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFormFields.Location = new System.Drawing.Point(0, 0);
            this.pnlFormFields.Name = "pnlFormFields";
            this.pnlFormFields.Padding = new System.Windows.Forms.Padding(30, 20, 30, 30);
            this.pnlFormFields.Size = new System.Drawing.Size(486, 636);
            this.pnlFormFields.TabIndex = 0;
            //
            // lblLocation
            //
            this.lblLocation.AutoSize = true;
            this.lblLocation.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblLocation.Location = new System.Drawing.Point(30, 20);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(100, 19);
            this.lblLocation.TabIndex = 0;
            this.lblLocation.Text = "Location *";
            //
            // txtLocation
            //
            this.txtLocation.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtLocation.Location = new System.Drawing.Point(30, 44);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(420, 25);
            this.txtLocation.TabIndex = 1;
            this.txtLocation.TextChanged += new System.EventHandler(this.InputChanged);
            //
            // lblCategory
            //
            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCategory.Location = new System.Drawing.Point(30, 88);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(100, 19);
            this.lblCategory.TabIndex = 2;
            this.lblCategory.Text = "Category *";
            //
            // cmbCategory
            //
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Items.AddRange(new object[] {
            "-- Select a category --",
            "Sanitation",
            "Roads and Potholes",
            "Water and Sewage",
            "Electricity",
            "Waste Management",
            "Parks and Public Spaces",
            "Other"});
            this.cmbCategory.Location = new System.Drawing.Point(30, 112);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(300, 27);
            this.cmbCategory.TabIndex = 3;
            this.cmbCategory.SelectedIndex = 0;
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.InputChanged);
            //
            // lblDescription
            //
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblDescription.Location = new System.Drawing.Point(30, 156);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(120, 19);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "Description *";
            //
            // rtbDescription
            //
            this.rtbDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rtbDescription.Location = new System.Drawing.Point(30, 180);
            this.rtbDescription.Name = "rtbDescription";
            this.rtbDescription.Size = new System.Drawing.Size(420, 110);
            this.rtbDescription.TabIndex = 5;
            this.rtbDescription.Text = "";
            this.rtbDescription.TextChanged += new System.EventHandler(this.InputChanged);
            //
            // lblAttachments
            //
            this.lblAttachments.AutoSize = true;
            this.lblAttachments.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAttachments.Location = new System.Drawing.Point(30, 302);
            this.lblAttachments.Name = "lblAttachments";
            this.lblAttachments.Size = new System.Drawing.Size(160, 19);
            this.lblAttachments.TabIndex = 6;
            this.lblAttachments.Text = "Attachments (optional)";
            //
            // btnAttachMedia
            //
            this.btnAttachMedia.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnAttachMedia.FlatAppearance.BorderSize = 0;
            this.btnAttachMedia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAttachMedia.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnAttachMedia.ForeColor = System.Drawing.Color.White;
            this.btnAttachMedia.Location = new System.Drawing.Point(30, 326);
            this.btnAttachMedia.Name = "btnAttachMedia";
            this.btnAttachMedia.Size = new System.Drawing.Size(190, 34);
            this.btnAttachMedia.TabIndex = 7;
            this.btnAttachMedia.Text = "Attach Image / Document";
            this.btnAttachMedia.UseVisualStyleBackColor = false;
            this.btnAttachMedia.Click += new System.EventHandler(this.btnAttachMedia_Click);
            //
            // lstAttachments
            //
            this.lstAttachments.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lstAttachments.FormattingEnabled = true;
            this.lstAttachments.ItemHeight = 15;
            this.lstAttachments.Location = new System.Drawing.Point(30, 366);
            this.lstAttachments.Name = "lstAttachments";
            this.lstAttachments.Size = new System.Drawing.Size(420, 64);
            this.lstAttachments.TabIndex = 8;
            //
            // lblEngagementHeader
            //
            this.lblEngagementHeader.AutoSize = true;
            this.lblEngagementHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblEngagementHeader.ForeColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.lblEngagementHeader.Location = new System.Drawing.Point(30, 452);
            this.lblEngagementHeader.Name = "lblEngagementHeader";
            this.lblEngagementHeader.Size = new System.Drawing.Size(160, 19);
            this.lblEngagementHeader.TabIndex = 9;
            this.lblEngagementHeader.Text = "Your Report Progress";
            //
            // progressBarCompletion
            //
            this.progressBarCompletion.Location = new System.Drawing.Point(30, 476);
            this.progressBarCompletion.Name = "progressBarCompletion";
            this.progressBarCompletion.Size = new System.Drawing.Size(420, 22);
            this.progressBarCompletion.TabIndex = 10;
            //
            // lblProgressPercent
            //
            this.lblProgressPercent.AutoSize = true;
            this.lblProgressPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProgressPercent.ForeColor = System.Drawing.Color.DimGray;
            this.lblProgressPercent.Location = new System.Drawing.Point(30, 502);
            this.lblProgressPercent.Name = "lblProgressPercent";
            this.lblProgressPercent.Size = new System.Drawing.Size(280, 15);
            this.lblProgressPercent.TabIndex = 11;
            this.lblProgressPercent.Text = "0% complete - fill in the fields above to continue";
            //
            // lblStatus
            //
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(127, 140, 141);
            this.lblStatus.Location = new System.Drawing.Point(30, 528);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(420, 40);
            this.lblStatus.TabIndex = 12;
            this.lblStatus.Text = "Status: not submitted yet";
            //
            // btnSubmit
            //
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnSubmit.FlatAppearance.BorderSize = 0;
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(30, 580);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(160, 42);
            this.btnSubmit.TabIndex = 13;
            this.btnSubmit.Text = "Submit Report";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            //
            // btnReportAnother
            //
            this.btnReportAnother.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnReportAnother.FlatAppearance.BorderSize = 0;
            this.btnReportAnother.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReportAnother.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnReportAnother.ForeColor = System.Drawing.Color.White;
            this.btnReportAnother.Location = new System.Drawing.Point(200, 580);
            this.btnReportAnother.Name = "btnReportAnother";
            this.btnReportAnother.Size = new System.Drawing.Size(200, 42);
            this.btnReportAnother.TabIndex = 14;
            this.btnReportAnother.Text = "Report Another Issue";
            this.btnReportAnother.UseVisualStyleBackColor = false;
            this.btnReportAnother.Visible = false;
            this.btnReportAnother.Click += new System.EventHandler(this.btnReportAnother_Click);
            //
            // pnlReports
            //
            this.pnlReports.BackColor = System.Drawing.Color.FromArgb(241, 244, 247);
            this.pnlReports.Controls.Add(this.dgvMyReports);
            this.pnlReports.Controls.Add(this.lblReportsHeader);
            this.pnlReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlReports.Location = new System.Drawing.Point(0, 0);
            this.pnlReports.Name = "pnlReports";
            this.pnlReports.Padding = new System.Windows.Forms.Padding(20, 16, 20, 16);
            this.pnlReports.Size = new System.Drawing.Size(548, 636);
            this.pnlReports.TabIndex = 0;
            //
            // lblReportsHeader
            //
            this.lblReportsHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblReportsHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblReportsHeader.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.lblReportsHeader.Location = new System.Drawing.Point(20, 16);
            this.lblReportsHeader.Name = "lblReportsHeader";
            this.lblReportsHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.lblReportsHeader.Size = new System.Drawing.Size(508, 38);
            this.lblReportsHeader.TabIndex = 0;
            this.lblReportsHeader.Text = "My Reported Issues";
            //
            // dgvMyReports
            //
            this.dgvMyReports.AllowUserToAddRows = false;
            this.dgvMyReports.AllowUserToDeleteRows = false;
            this.dgvMyReports.AllowUserToResizeRows = false;
            this.dgvMyReports.BackgroundColor = System.Drawing.Color.White;
            this.dgvMyReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMyReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMyReports.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvMyReports.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvMyReports.Location = new System.Drawing.Point(20, 54);
            this.dgvMyReports.MultiSelect = false;
            this.dgvMyReports.Name = "dgvMyReports";
            this.dgvMyReports.ReadOnly = true;
            this.dgvMyReports.RowHeadersVisible = false;
            this.dgvMyReports.RowTemplate.Height = 28;
            this.dgvMyReports.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMyReports.Size = new System.Drawing.Size(508, 566);
            this.dgvMyReports.TabIndex = 1;
            //
            // openFileDialogMedia
            //
            this.openFileDialogMedia.Filter = "Images and Documents|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.pdf;*.doc;*.docx|All files" +
    "|*.*";
            this.openFileDialogMedia.Multiselect = true;
            this.openFileDialogMedia.Title = "Attach images or documents related to the issue";
            //
            // statusTimer
            //
            this.statusTimer.Interval = 4000;
            this.statusTimer.Tick += new System.EventHandler(this.statusTimer_Tick);
            //
            // ReportIssueForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1040, 700);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(900, 560);
            this.Name = "ReportIssueForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Municipal Services Application - Report an Issue";
            this.Load += new System.EventHandler(this.ReportIssueForm_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.pnlFormFields.ResumeLayout(false);
            this.pnlFormFields.PerformLayout();
            this.pnlReports.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyReports)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
