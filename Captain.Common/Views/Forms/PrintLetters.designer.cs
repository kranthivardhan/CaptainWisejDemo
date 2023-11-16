using Wisej.Web;

namespace Captain.Common.Views.Forms
{
    partial class PrintLetters
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region WISEJ Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintLetters));
            this.panel1 = new Wisej.Web.Panel();
            this.lblworker = new Wisej.Web.Label();
            this.lblDate = new Wisej.Web.Label();
            this.lblName = new Wisej.Web.Label();
            this.lblAppNo = new Wisej.Web.Label();
            this.lblN = new Wisej.Web.Label();
            this.lblApp = new Wisej.Web.Label();
            this.panel2 = new Wisej.Web.Panel();
            this.gvApp = new Wisej.Web.DataGridView();
            this.Check = new Wisej.Web.DataGridViewCheckBoxColumn();
            this.AppDet = new Wisej.Web.DataGridViewTextBoxColumn();
            this.RecentDate = new Wisej.Web.DataGridViewTextBoxColumn();
            this.RecentWorker = new Wisej.Web.DataGridViewTextBoxColumn();
            this.gvCode = new Wisej.Web.DataGridViewTextBoxColumn();
            this.gvStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.btnPrev = new Wisej.Web.Button();
            this.btnExit = new Wisej.Web.Button();
            this.btnHistory = new Wisej.Web.Button();
            this.panel3 = new Wisej.Web.Panel();
            this.spacer1 = new Wisej.Web.Spacer();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvApp)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblworker);
            this.panel1.Controls.Add(this.lblDate);
            this.panel1.Controls.Add(this.lblName);
            this.panel1.Controls.Add(this.lblAppNo);
            this.panel1.Controls.Add(this.lblN);
            this.panel1.Controls.Add(this.lblApp);
            this.panel1.Dock = Wisej.Web.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(694, 56);
            this.panel1.TabIndex = 0;
            // 
            // lblworker
            // 
            this.lblworker.AutoSize = true;
            this.lblworker.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblworker.Location = new System.Drawing.Point(426, 20);
            this.lblworker.MaximumSize = new System.Drawing.Size(100, 50);
            this.lblworker.MinimumSize = new System.Drawing.Size(0, 35);
            this.lblworker.Name = "lblworker";
            this.lblworker.Size = new System.Drawing.Size(84, 35);
            this.lblworker.TabIndex = 0;
            this.lblworker.Text = "Recent Printed Worker";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblDate.Location = new System.Drawing.Point(318, 20);
            this.lblDate.MaximumSize = new System.Drawing.Size(100, 50);
            this.lblDate.MinimumSize = new System.Drawing.Size(0, 35);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(84, 35);
            this.lblDate.TabIndex = 0;
            this.lblDate.Text = "Recent Printed Date";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblName.Location = new System.Drawing.Point(60, 33);
            this.lblName.MinimumSize = new System.Drawing.Size(0, 18);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(37, 18);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // lblAppNo
            // 
            this.lblAppNo.AutoSize = true;
            this.lblAppNo.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblAppNo.Location = new System.Drawing.Point(60, 8);
            this.lblAppNo.MinimumSize = new System.Drawing.Size(0, 18);
            this.lblAppNo.Name = "lblAppNo";
            this.lblAppNo.Size = new System.Drawing.Size(33, 18);
            this.lblAppNo.TabIndex = 0;
            this.lblAppNo.Text = "App#";
            // 
            // lblN
            // 
            this.lblN.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblN.Location = new System.Drawing.Point(8, 33);
            this.lblN.Name = "lblN";
            this.lblN.Size = new System.Drawing.Size(45, 18);
            this.lblN.TabIndex = 0;
            this.lblN.Text = "Name :";
            // 
            // lblApp
            // 
            this.lblApp.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblApp.Location = new System.Drawing.Point(8, 8);
            this.lblApp.Name = "lblApp";
            this.lblApp.Size = new System.Drawing.Size(42, 19);
            this.lblApp.TabIndex = 0;
            this.lblApp.Text = "App# :";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gvApp);
            this.panel2.Dock = Wisej.Web.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 56);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(694, 288);
            this.panel2.TabIndex = 1;
            // 
            // gvApp
            // 
            this.gvApp.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.gvApp.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gvApp.ColumnHeadersHeightSizeMode = Wisej.Web.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvApp.ColumnHeadersVisible = false;
            this.gvApp.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.Check,
            this.AppDet,
            this.RecentDate,
            this.RecentWorker,
            this.gvCode,
            this.gvStatus});
            this.gvApp.Dock = Wisej.Web.DockStyle.Fill;
            this.gvApp.Location = new System.Drawing.Point(0, 0);
            this.gvApp.Name = "gvApp";
            this.gvApp.RowHeadersVisible = false;
            this.gvApp.RowHeadersWidth = 25;
            this.gvApp.ScrollBars = Wisej.Web.ScrollBars.Vertical;
            this.gvApp.Size = new System.Drawing.Size(694, 288);
            this.gvApp.TabIndex = 0;
            this.gvApp.CellValueChanged += new Wisej.Web.DataGridViewCellEventHandler(this.gvApp_CellValueChanged);
            this.gvApp.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.gvApp_CellClick);
            // 
            // Check
            // 
            dataGridViewCellStyle2.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = false;
            this.Check.DefaultCellStyle = dataGridViewCellStyle2;
            this.Check.HeaderText = " ";
            this.Check.Name = "Check";
            this.Check.Width = 30;
            // 
            // AppDet
            // 
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.AppDet.DefaultCellStyle = dataGridViewCellStyle3;
            this.AppDet.HeaderText = "Application Details";
            this.AppDet.Name = "AppDet";
            this.AppDet.ReadOnly = true;
            this.AppDet.Width = 280;
            // 
            // RecentDate
            // 
            this.RecentDate.Name = "RecentDate";
            this.RecentDate.ReadOnly = true;
            this.RecentDate.Width = 115;
            // 
            // RecentWorker
            // 
            this.RecentWorker.Name = "RecentWorker";
            this.RecentWorker.ReadOnly = true;
            this.RecentWorker.Width = 135;
            // 
            // gvCode
            // 
            this.gvCode.Name = "gvCode";
            this.gvCode.Visible = false;
            this.gvCode.Width = 20;
            // 
            // gvStatus
            // 
            this.gvStatus.Name = "gvStatus";
            this.gvStatus.Visible = false;
            this.gvStatus.Width = 20;
            // 
            // btnPrev
            // 
            this.btnPrev.Dock = Wisej.Web.DockStyle.Right;
            this.btnPrev.Location = new System.Drawing.Point(569, 4);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(59, 27);
            this.btnPrev.TabIndex = 1;
            this.btnPrev.Text = "Pre&view";
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnExit
            // 
            this.btnExit.AppearanceKey = "button-cancel";
            this.btnExit.Dock = Wisej.Web.DockStyle.Right;
            this.btnExit.Location = new System.Drawing.Point(633, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(57, 27);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "&Exit";
            this.btnExit.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnHistory
            // 
            this.btnHistory.Dock = Wisej.Web.DockStyle.Left;
            this.btnHistory.Location = new System.Drawing.Point(4, 4);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(59, 27);
            this.btnHistory.TabIndex = 3;
            this.btnHistory.Text = "&History";
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // panel3
            // 
            this.panel3.AppearanceKey = "panel-grdo";
            this.panel3.Controls.Add(this.btnPrev);
            this.panel3.Controls.Add(this.btnHistory);
            this.panel3.Controls.Add(this.spacer1);
            this.panel3.Controls.Add(this.btnExit);
            this.panel3.Dock = Wisej.Web.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 344);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new Wisej.Web.Padding(4);
            this.panel3.Size = new System.Drawing.Size(694, 35);
            this.panel3.TabIndex = 4;
            // 
            // spacer1
            // 
            this.spacer1.Dock = Wisej.Web.DockStyle.Right;
            this.spacer1.Location = new System.Drawing.Point(628, 4);
            this.spacer1.Name = "spacer1";
            this.spacer1.Size = new System.Drawing.Size(5, 27);
            // 
            // PrintLetters
            // 
            this.ClientSize = new System.Drawing.Size(694, 379);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Image)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintLetters";
            this.Text = "PrintApplicants";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvApp)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label lblName;
        private Label lblAppNo;
        private Label lblN;
        private Label lblApp;
        private Panel panel2;
        private DataGridView gvApp;
        private DataGridViewCheckBoxColumn Check;
        private DataGridViewTextBoxColumn AppDet;
        private Button btnPrev;
        private Button btnExit;
        private Label lblworker;
        private Label lblDate;
        private DataGridViewTextBoxColumn RecentDate;
        private DataGridViewTextBoxColumn RecentWorker;
        private DataGridViewTextBoxColumn gvCode;
        private DataGridViewTextBoxColumn gvStatus;
        private Button btnHistory;
        private Panel panel3;
        private Spacer spacer1;
    }
}