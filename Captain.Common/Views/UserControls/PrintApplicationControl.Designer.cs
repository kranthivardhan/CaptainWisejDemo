namespace Captain.Common.Views.UserControls
{
    partial class PrintApplicationControl
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

        #region Wisej Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle5 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle6 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle7 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle8 = new Wisej.Web.DataGridViewCellStyle();
            this.pnlCompleteForm = new Wisej.Web.Panel();
            this.pnlSave = new Wisej.Web.Panel();
            this.btnSave = new Wisej.Web.Button();
            this.spacer1 = new Wisej.Web.Spacer();
            this.btnCancel = new Wisej.Web.Button();
            this.pnldgvApplications = new Wisej.Web.Panel();
            this.dgvApplications = new Wisej.Web.DataGridView();
            this.dgvForm = new Wisej.Web.DataGridViewTextBoxColumn();
            this.dgvEnable = new Wisej.Web.DataGridViewCheckBoxColumn();
            this.dgvDisplayName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.dgvAgency = new Wisej.Web.DataGridViewTextBoxColumn();
            this.dgvMode = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlApp = new Wisej.Web.Panel();
            this.pnlCompleteForm.SuspendLayout();
            this.pnlSave.SuspendLayout();
            this.pnldgvApplications.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvApplications)).BeginInit();
            this.pnlApp.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCompleteForm
            // 
            this.pnlCompleteForm.Controls.Add(this.pnlApp);
            this.pnlCompleteForm.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlCompleteForm.Location = new System.Drawing.Point(0, 25);
            this.pnlCompleteForm.Name = "pnlCompleteForm";
            this.pnlCompleteForm.Padding = new Wisej.Web.Padding(10, 5, 10, 10);
            this.pnlCompleteForm.Size = new System.Drawing.Size(1195, 671);
            this.pnlCompleteForm.TabIndex = 2;
            // 
            // pnlSave
            // 
            this.pnlSave.AppearanceKey = "panel-grdo";
            this.pnlSave.Controls.Add(this.btnSave);
            this.pnlSave.Controls.Add(this.spacer1);
            this.pnlSave.Controls.Add(this.btnCancel);
            this.pnlSave.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlSave.Location = new System.Drawing.Point(0, 305);
            this.pnlSave.Name = "pnlSave";
            this.pnlSave.Padding = new Wisej.Web.Padding(15, 5, 15, 5);
            this.pnlSave.Size = new System.Drawing.Size(1175, 35);
            this.pnlSave.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.AppearanceKey = "button-ok";
            this.btnSave.Dock = Wisej.Web.DockStyle.Right;
            this.btnSave.Location = new System.Drawing.Point(1007, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "&Save";
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // spacer1
            // 
            this.spacer1.Dock = Wisej.Web.DockStyle.Right;
            this.spacer1.Location = new System.Drawing.Point(1082, 5);
            this.spacer1.Name = "spacer1";
            this.spacer1.Size = new System.Drawing.Size(3, 25);
            // 
            // btnCancel
            // 
            this.btnCancel.AppearanceKey = "button-error";
            this.btnCancel.Dock = Wisej.Web.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(1085, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnldgvApplications
            // 
            this.pnldgvApplications.Controls.Add(this.dgvApplications);
            this.pnldgvApplications.Dock = Wisej.Web.DockStyle.Fill;
            this.pnldgvApplications.Location = new System.Drawing.Point(0, 0);
            this.pnldgvApplications.Name = "pnldgvApplications";
            this.pnldgvApplications.Size = new System.Drawing.Size(1175, 305);
            this.pnldgvApplications.TabIndex = 0;
            // 
            // dgvApplications
            // 
            this.dgvApplications.AutoSizeRowsMode = Wisej.Web.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvApplications.BackColor = System.Drawing.Color.FromArgb(253, 253, 253);
            this.dgvApplications.ColumnHeadersHeight = 25;
            this.dgvApplications.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.dgvForm,
            this.dgvEnable,
            this.dgvDisplayName,
            this.dgvAgency,
            this.dgvMode});
            this.dgvApplications.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvApplications.Enabled = false;
            this.dgvApplications.Location = new System.Drawing.Point(0, 0);
            this.dgvApplications.Name = "dgvApplications";
            this.dgvApplications.RowHeadersWidth = 20;
            this.dgvApplications.RowHeadersWidthSizeMode = Wisej.Web.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvApplications.ShowColumnVisibilityMenu = false;
            this.dgvApplications.Size = new System.Drawing.Size(1175, 305);
            this.dgvApplications.TabIndex = 0;
            this.dgvApplications.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.dgvApplications_CellClick);
            // 
            // dgvForm
            // 
            dataGridViewCellStyle1.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.dgvForm.DefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.dgvForm.HeaderStyle = dataGridViewCellStyle2;
            this.dgvForm.HeaderText = "Form";
            this.dgvForm.Name = "dgvForm";
            this.dgvForm.ReadOnly = true;
            this.dgvForm.Width = 300;
            // 
            // dgvEnable
            // 
            dataGridViewCellStyle3.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.NullValue = false;
            this.dgvEnable.DefaultCellStyle = dataGridViewCellStyle3;
            dataGridViewCellStyle4.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            this.dgvEnable.HeaderStyle = dataGridViewCellStyle4;
            this.dgvEnable.HeaderText = "Enable";
            this.dgvEnable.Name = "dgvEnable";
            this.dgvEnable.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.dgvEnable.Width = 45;
            // 
            // dgvDisplayName
            // 
            dataGridViewCellStyle5.Padding = new Wisej.Web.Padding(10, 0, 0, 0);
            dataGridViewCellStyle5.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.dgvDisplayName.DefaultCellStyle = dataGridViewCellStyle5;
            dataGridViewCellStyle6.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.Padding = new Wisej.Web.Padding(10, 0, 0, 0);
            this.dgvDisplayName.HeaderStyle = dataGridViewCellStyle6;
            this.dgvDisplayName.HeaderText = "Form Display Name";
            this.dgvDisplayName.Name = "dgvDisplayName";
            this.dgvDisplayName.Width = 310;
            // 
            // dgvAgency
            // 
            dataGridViewCellStyle7.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.dgvAgency.DefaultCellStyle = dataGridViewCellStyle7;
            dataGridViewCellStyle8.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            this.dgvAgency.HeaderStyle = dataGridViewCellStyle8;
            this.dgvAgency.HeaderText = "Agency";
            this.dgvAgency.Name = "dgvAgency";
            this.dgvAgency.ReadOnly = true;
            this.dgvAgency.ShowInVisibilityMenu = false;
            // 
            // dgvMode
            // 
            this.dgvMode.HeaderText = "Mode";
            this.dgvMode.Name = "dgvMode";
            this.dgvMode.ShowInVisibilityMenu = false;
            this.dgvMode.Visible = false;
            // 
            // pnlApp
            // 
            this.pnlApp.Controls.Add(this.pnldgvApplications);
            this.pnlApp.Controls.Add(this.pnlSave);
            this.pnlApp.CssStyle = "border-radius:8px; border:1px solid #ececec;";
            this.pnlApp.Dock = Wisej.Web.DockStyle.Top;
            this.pnlApp.Location = new System.Drawing.Point(10, 5);
            this.pnlApp.Name = "pnlApp";
            this.pnlApp.Size = new System.Drawing.Size(1175, 340);
            this.pnlApp.TabIndex = 2;
            // 
            // PrintApplicationControl
            // 
            this.Controls.Add(this.pnlCompleteForm);
            this.Name = "PrintApplicationControl";
            this.Size = new System.Drawing.Size(1195, 696);
            this.Controls.SetChildIndex(this.pnlCompleteForm, 0);
            this.pnlCompleteForm.ResumeLayout(false);
            this.pnlSave.ResumeLayout(false);
            this.pnldgvApplications.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvApplications)).EndInit();
            this.pnlApp.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Wisej.Web.Panel pnlCompleteForm;
        private Wisej.Web.Panel pnldgvApplications;
        private Wisej.Web.DataGridView dgvApplications;
        private Wisej.Web.DataGridViewTextBoxColumn dgvForm;
        private Wisej.Web.DataGridViewCheckBoxColumn dgvEnable;
        private Wisej.Web.DataGridViewTextBoxColumn dgvDisplayName;
        private Wisej.Web.Panel pnlSave;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Spacer spacer1;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.DataGridViewTextBoxColumn dgvAgency;
        private Wisej.Web.DataGridViewTextBoxColumn dgvMode;
        private Wisej.Web.Panel pnlApp;
    }
}
