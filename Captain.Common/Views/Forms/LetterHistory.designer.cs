using Wisej.Web;

namespace Captain.Common.Views.Forms
{
    partial class LetterHistory
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

        #region Wisej Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LetterHistory));
            this.panel2 = new Wisej.Web.Panel();
            this.gvApp = new Wisej.Web.DataGridView();
            this.AppDet = new Wisej.Web.DataGridViewTextBoxColumn();
            this.RecentDate = new Wisej.Web.DataGridViewTextBoxColumn();
            this.RecentWorker = new Wisej.Web.DataGridViewTextBoxColumn();
            this.gvCode = new Wisej.Web.DataGridViewTextBoxColumn();
            this.gvStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.btnExit = new Wisej.Web.Button();
            this.panel1 = new Wisej.Web.Panel();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvApp)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gvApp);
            this.panel2.Location = new System.Drawing.Point(0, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(591, 290);
            this.panel2.TabIndex = 1;
            // 
            // gvApp
            // 
            this.gvApp.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.FormatProvider = new System.Globalization.CultureInfo("en-IN");
            dataGridViewCellStyle3.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.gvApp.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.gvApp.ColumnHeadersHeightSizeMode = Wisej.Web.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvApp.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
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
            this.gvApp.Size = new System.Drawing.Size(591, 290);
            this.gvApp.TabIndex = 0;
            this.gvApp.CellValueChanged += new Wisej.Web.DataGridViewCellEventHandler(this.gvApp_CellValueChanged);
            this.gvApp.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.gvApp_CellClick);
            // 
            // AppDet
            // 
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle4.FormatProvider = new System.Globalization.CultureInfo("en-IN");
            this.AppDet.DefaultCellStyle = dataGridViewCellStyle4;
            this.AppDet.HeaderText = " ";
            this.AppDet.Name = "AppDet";
            this.AppDet.ReadOnly = true;
            this.AppDet.ShowInVisibilityMenu = false;
            this.AppDet.Width = 280;
            // 
            // RecentDate
            // 
            this.RecentDate.HeaderText = "Printed Date";
            this.RecentDate.Name = "RecentDate";
            this.RecentDate.ReadOnly = true;
            this.RecentDate.Width = 132;
            // 
            // RecentWorker
            // 
            this.RecentWorker.HeaderText = "Printed Worker";
            this.RecentWorker.Name = "RecentWorker";
            this.RecentWorker.ReadOnly = true;
            this.RecentWorker.Width = 147;
            // 
            // gvCode
            // 
            this.gvCode.HeaderText = "gvCode";
            this.gvCode.Name = "gvCode";
            this.gvCode.ShowInVisibilityMenu = false;
            this.gvCode.Visible = false;
            this.gvCode.Width = 20;
            // 
            // gvStatus
            // 
            this.gvStatus.HeaderText = "gvStatus";
            this.gvStatus.Name = "gvStatus";
            this.gvStatus.ShowInVisibilityMenu = false;
            this.gvStatus.Visible = false;
            this.gvStatus.Width = 20;
            // 
            // btnExit
            // 
            this.btnExit.AppearanceKey = "button-cancel";
            this.btnExit.Dock = Wisej.Web.DockStyle.Right;
            this.btnExit.Location = new System.Drawing.Point(528, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(57, 27);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "&Exit";
            this.btnExit.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.AppearanceKey = "panel-grdo";
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Dock = Wisej.Web.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 293);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new Wisej.Web.Padding(4);
            this.panel1.Size = new System.Drawing.Size(589, 35);
            this.panel1.TabIndex = 3;
            // 
            // LetterHistory
            // 
            this.ClientSize = new System.Drawing.Size(589, 328);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Image)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LetterHistory";
            this.Text = "Print Applicants";
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvApp)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Panel panel2;
        private DataGridView gvApp;
        private DataGridViewTextBoxColumn AppDet;
        private Button btnExit;
        private DataGridViewTextBoxColumn RecentDate;
        private DataGridViewTextBoxColumn RecentWorker;
        private DataGridViewTextBoxColumn gvCode;
        private DataGridViewTextBoxColumn gvStatus;
        private Panel panel1;
    }
}