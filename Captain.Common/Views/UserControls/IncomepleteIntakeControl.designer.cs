using Captain.Common.Views.Controls.Compatibility;
using Wisej.Web;

namespace Captain.Common.Views.UserControls
{
    partial class IncomepleteIntakeControl
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

        #region Wisej UserControl Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle6 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle10 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle7 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle8 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle9 = new Wisej.Web.DataGridViewCellStyle();
            this.btnOk = new Wisej.Web.Button();
            this.btnClose = new Wisej.Web.Button();
            this.label24 = new Wisej.Web.Label();
            this.txtLine1 = new Wisej.Web.TextBox();
            this.label1 = new Wisej.Web.Label();
            this.pnlDetails = new Wisej.Web.Panel();
            this.label2 = new Wisej.Web.Label();
            this.gvwIncompletedata = new Wisej.Web.DataGridView();
            this.gvchkSelect = new Wisej.Web.DataGridViewCheckBoxColumn();
            this.gvtDesc = new Wisej.Web.DataGridViewTextBoxColumn();
            this.gvtCode = new Wisej.Web.DataGridViewTextBoxColumn();
            this.gvtLine1 = new Wisej.Web.DataGridViewTextBoxColumn();
            this.gvtLine2 = new Wisej.Web.DataGridViewTextBoxColumn();
            this.gvtLine3 = new Wisej.Web.DataGridViewTextBoxColumn();
            this.gvtLetterDate = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlControl = new Wisej.Web.Panel();
            this.pnlTop = new Wisej.Web.Panel();
            this.pnlParams = new Wisej.Web.Panel();
            this.pnlBottom = new Wisej.Web.Panel();
            this.pnlSaveB = new Wisej.Web.Panel();
            this.pnlSave = new Wisej.Web.Panel();
            this.spacer1 = new Wisej.Web.Spacer();
            this.spacer3 = new Wisej.Web.Spacer();
            this.pnlgvwIncompletedata = new Wisej.Web.Panel();
            this.spacer2 = new Wisej.Web.Spacer();
            this.pnlLetter = new Wisej.Web.Panel();
            this.pnlLabels = new Wisej.Web.Panel();
            this.pnlTLeft = new Wisej.Web.Panel();
            this.pnllblMessage5 = new Wisej.Web.Panel();
            this.lblMessage5 = new Wisej.Web.Label();
            this.pnllblMessage4 = new Wisej.Web.Panel();
            this.lblMessage4 = new Wisej.Web.Label();
            this.pnllblMessage3 = new Wisej.Web.Panel();
            this.lblMessage3 = new Wisej.Web.Label();
            this.pnllblMessage2 = new Wisej.Web.Panel();
            this.pnlTRight = new Wisej.Web.Panel();
            this.pnllblCaseWorker = new Wisej.Web.Panel();
            this.label5 = new Wisej.Web.Label();
            this.lblCaseWorker = new Wisej.Web.Label();
            this.spacer5 = new Wisej.Web.Spacer();
            this.cmbCaseWorker = new Captain.Common.Views.Controls.Compatibility.ComboBoxEx();
            this.pnllblletterdate = new Wisej.Web.Panel();
            this.lblLetterDateReq = new Wisej.Web.Label();
            this.lblletterdate = new Wisej.Web.Label();
            this.spacer4 = new Wisej.Web.Spacer();
            this.dtletterDate = new Wisej.Web.DateTimePicker();
            this.pnllblMessage1 = new Wisej.Web.Panel();
            this.lblMessage1 = new Wisej.Web.Label();
            this.panel5 = new Wisej.Web.Panel();
            this.panel6 = new Wisej.Web.Panel();
            this.panel7 = new Wisej.Web.Panel();
            this.panel8 = new Wisej.Web.Panel();
            this.lblMessage2 = new Wisej.Web.Label();
            this.txtLine1.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvwIncompletedata)).BeginInit();
            this.pnlControl.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlParams.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlSaveB.SuspendLayout();
            this.pnlSave.SuspendLayout();
            this.pnlgvwIncompletedata.SuspendLayout();
            this.pnlLetter.SuspendLayout();
            this.pnlLabels.SuspendLayout();
            this.pnlTLeft.SuspendLayout();
            this.pnllblMessage5.SuspendLayout();
            this.pnllblMessage4.SuspendLayout();
            this.pnllblMessage3.SuspendLayout();
            this.pnllblMessage2.SuspendLayout();
            this.pnlTRight.SuspendLayout();
            this.pnllblCaseWorker.SuspendLayout();
            this.pnllblletterdate.SuspendLayout();
            this.pnllblMessage1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.AppearanceKey = "button-ok";
            this.btnOk.Dock = Wisej.Web.DockStyle.Right;
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(626, 5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 24);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "&Save";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnClose
            // 
            this.btnClose.AppearanceKey = "button-error";
            this.btnClose.Dock = Wisej.Web.DockStyle.Right;
            this.btnClose.Location = new System.Drawing.Point(704, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 24);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "&Cancel";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(4, 354);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(4, 14);
            this.label24.TabIndex = 8;
            // 
            // txtLine1
            // 
            this.txtLine1.Controls.Add(this.label24);
            this.txtLine1.Location = new System.Drawing.Point(72, 4);
            this.txtLine1.MaxLength = 240;
            this.txtLine1.Multiline = true;
            this.txtLine1.Name = "txtLine1";
            this.txtLine1.ReadOnly = true;
            this.txtLine1.Size = new System.Drawing.Size(708, 57);
            this.txtLine1.TabIndex = 5;
            this.txtLine1.Leave += new System.EventHandler(this.txtLine1_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Details";
            // 
            // pnlDetails
            // 
            this.pnlDetails.Controls.Add(this.label2);
            this.pnlDetails.Controls.Add(this.txtLine1);
            this.pnlDetails.Controls.Add(this.label1);
            this.pnlDetails.Dock = Wisej.Web.DockStyle.Top;
            this.pnlDetails.Location = new System.Drawing.Point(0, 0);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(795, 66);
            this.pnlDetails.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.CssStyle = "0";
            this.label2.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.Location = new System.Drawing.Point(8, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 11);
            this.label2.TabIndex = 0;
            this.label2.Text = "(Max 3 lines)";
            // 
            // gvwIncompletedata
            // 
            this.gvwIncompletedata.AllowUserToResizeColumns = false;
            this.gvwIncompletedata.AllowUserToResizeRows = false;
            this.gvwIncompletedata.AutoSizeRowsMode = Wisej.Web.DataGridViewAutoSizeRowsMode.AllCells;
            this.gvwIncompletedata.BackColor = System.Drawing.Color.FromArgb(253, 253, 253);
            this.gvwIncompletedata.BorderStyle = Wisej.Web.BorderStyle.None;
            dataGridViewCellStyle6.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.gvwIncompletedata.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.gvwIncompletedata.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.gvchkSelect,
            this.gvtDesc,
            this.gvtCode,
            this.gvtLine1,
            this.gvtLine2,
            this.gvtLine3,
            this.gvtLetterDate});
            this.gvwIncompletedata.CssStyle = "border-radius:8px; border:1px solid #ececec; ";
            dataGridViewCellStyle10.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle10.WrapMode = Wisej.Web.DataGridViewTriState.False;
            this.gvwIncompletedata.DefaultCellStyle = dataGridViewCellStyle10;
            this.gvwIncompletedata.Dock = Wisej.Web.DockStyle.Left;
            this.gvwIncompletedata.Location = new System.Drawing.Point(0, 0);
            this.gvwIncompletedata.MultiSelect = false;
            this.gvwIncompletedata.Name = "gvwIncompletedata";
            this.gvwIncompletedata.RowHeadersWidth = 14;
            this.gvwIncompletedata.RowHeadersWidthSizeMode = Wisej.Web.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gvwIncompletedata.ScrollBars = Wisej.Web.ScrollBars.Vertical;
            this.gvwIncompletedata.ShowColumnVisibilityMenu = false;
            this.gvwIncompletedata.Size = new System.Drawing.Size(794, 180);
            this.gvwIncompletedata.TabIndex = 3;
            this.gvwIncompletedata.SelectionChanged += new System.EventHandler(this.gvwIncompletedata_SelectionChanged);
            this.gvwIncompletedata.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.gvwIncompletedata_CellClick);
            // 
            // gvchkSelect
            // 
            dataGridViewCellStyle7.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.NullValue = false;
            this.gvchkSelect.DefaultCellStyle = dataGridViewCellStyle7;
            this.gvchkSelect.HeaderText = "   ";
            this.gvchkSelect.Name = "gvchkSelect";
            this.gvchkSelect.ReadOnly = true;
            this.gvchkSelect.ShowInVisibilityMenu = false;
            this.gvchkSelect.Width = 40;
            // 
            // gvtDesc
            // 
            dataGridViewCellStyle8.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.gvtDesc.DefaultCellStyle = dataGridViewCellStyle8;
            dataGridViewCellStyle9.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.gvtDesc.HeaderStyle = dataGridViewCellStyle9;
            this.gvtDesc.HeaderText = "Incomplete Intake Reason(s)";
            this.gvtDesc.Name = "gvtDesc";
            this.gvtDesc.ReadOnly = true;
            this.gvtDesc.Width = 540;
            // 
            // gvtCode
            // 
            this.gvtCode.HeaderText = "gvtCode";
            this.gvtCode.Name = "gvtCode";
            this.gvtCode.ReadOnly = true;
            this.gvtCode.ShowInVisibilityMenu = false;
            this.gvtCode.Visible = false;
            this.gvtCode.Width = 10;
            // 
            // gvtLine1
            // 
            this.gvtLine1.HeaderText = "gvtLine1";
            this.gvtLine1.Name = "gvtLine1";
            this.gvtLine1.ReadOnly = true;
            this.gvtLine1.ShowInVisibilityMenu = false;
            this.gvtLine1.Visible = false;
            this.gvtLine1.Width = 10;
            // 
            // gvtLine2
            // 
            this.gvtLine2.HeaderText = "gvtLine2";
            this.gvtLine2.Name = "gvtLine2";
            this.gvtLine2.ReadOnly = true;
            this.gvtLine2.ShowInVisibilityMenu = false;
            this.gvtLine2.Visible = false;
            this.gvtLine2.Width = 10;
            // 
            // gvtLine3
            // 
            this.gvtLine3.HeaderText = "gvtLine3";
            this.gvtLine3.Name = "gvtLine3";
            this.gvtLine3.ReadOnly = true;
            this.gvtLine3.ShowInVisibilityMenu = false;
            this.gvtLine3.Visible = false;
            this.gvtLine3.Width = 10;
            // 
            // gvtLetterDate
            // 
            this.gvtLetterDate.HeaderText = "gvtLetterDate";
            this.gvtLetterDate.Name = "gvtLetterDate";
            this.gvtLetterDate.ShowInVisibilityMenu = false;
            this.gvtLetterDate.Visible = false;
            this.gvtLetterDate.Width = 20;
            // 
            // pnlControl
            // 
            this.pnlControl.Controls.Add(this.pnlTop);
            this.pnlControl.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlControl.Location = new System.Drawing.Point(0, 25);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Size = new System.Drawing.Size(1300, 488);
            this.pnlControl.TabIndex = 8;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.pnlParams);
            this.pnlTop.Dock = Wisej.Web.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new Wisej.Web.Padding(10, 5, 0, 0);
            this.pnlTop.Size = new System.Drawing.Size(1300, 430);
            this.pnlTop.TabIndex = 10;
            // 
            // pnlParams
            // 
            this.pnlParams.Controls.Add(this.pnlBottom);
            this.pnlParams.Controls.Add(this.spacer3);
            this.pnlParams.Controls.Add(this.pnlgvwIncompletedata);
            this.pnlParams.Controls.Add(this.spacer2);
            this.pnlParams.Controls.Add(this.pnlLetter);
            this.pnlParams.Dock = Wisej.Web.DockStyle.Left;
            this.pnlParams.Location = new System.Drawing.Point(10, 5);
            this.pnlParams.Name = "pnlParams";
            this.pnlParams.Size = new System.Drawing.Size(795, 425);
            this.pnlParams.TabIndex = 9;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.pnlSaveB);
            this.pnlBottom.Controls.Add(this.pnlDetails);
            this.pnlBottom.CssStyle = "border-radius:8px; border:1px solid #ececec; ";
            this.pnlBottom.Dock = Wisej.Web.DockStyle.Top;
            this.pnlBottom.Location = new System.Drawing.Point(0, 296);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(795, 100);
            this.pnlBottom.TabIndex = 10;
            // 
            // pnlSaveB
            // 
            this.pnlSaveB.Controls.Add(this.pnlSave);
            this.pnlSaveB.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlSaveB.Location = new System.Drawing.Point(0, 66);
            this.pnlSaveB.Name = "pnlSaveB";
            this.pnlSaveB.Size = new System.Drawing.Size(795, 34);
            this.pnlSaveB.TabIndex = 9;
            // 
            // pnlSave
            // 
            this.pnlSave.AppearanceKey = "panel-grdo";
            this.pnlSave.Controls.Add(this.btnOk);
            this.pnlSave.Controls.Add(this.spacer1);
            this.pnlSave.Controls.Add(this.btnClose);
            this.pnlSave.Dock = Wisej.Web.DockStyle.Left;
            this.pnlSave.Location = new System.Drawing.Point(0, 0);
            this.pnlSave.Name = "pnlSave";
            this.pnlSave.Padding = new Wisej.Web.Padding(5, 5, 15, 5);
            this.pnlSave.Size = new System.Drawing.Size(794, 34);
            this.pnlSave.TabIndex = 8;
            // 
            // spacer1
            // 
            this.spacer1.Dock = Wisej.Web.DockStyle.Right;
            this.spacer1.Location = new System.Drawing.Point(701, 5);
            this.spacer1.Name = "spacer1";
            this.spacer1.Size = new System.Drawing.Size(3, 24);
            // 
            // spacer3
            // 
            this.spacer3.Dock = Wisej.Web.DockStyle.Top;
            this.spacer3.Location = new System.Drawing.Point(0, 291);
            this.spacer3.Name = "spacer3";
            this.spacer3.Size = new System.Drawing.Size(795, 5);
            // 
            // pnlgvwIncompletedata
            // 
            this.pnlgvwIncompletedata.Controls.Add(this.gvwIncompletedata);
            this.pnlgvwIncompletedata.CssStyle = "border-radius:8px; border:1px solid #ececec; ";
            this.pnlgvwIncompletedata.Dock = Wisej.Web.DockStyle.Top;
            this.pnlgvwIncompletedata.Location = new System.Drawing.Point(0, 111);
            this.pnlgvwIncompletedata.Name = "pnlgvwIncompletedata";
            this.pnlgvwIncompletedata.Size = new System.Drawing.Size(795, 180);
            this.pnlgvwIncompletedata.TabIndex = 8;
            // 
            // spacer2
            // 
            this.spacer2.Dock = Wisej.Web.DockStyle.Top;
            this.spacer2.Location = new System.Drawing.Point(0, 106);
            this.spacer2.Name = "spacer2";
            this.spacer2.Size = new System.Drawing.Size(795, 5);
            // 
            // pnlLetter
            // 
            this.pnlLetter.Controls.Add(this.pnlLabels);
            this.pnlLetter.CssStyle = "border-radius:8px; border:1px solid #ececec; ";
            this.pnlLetter.Dock = Wisej.Web.DockStyle.Top;
            this.pnlLetter.Location = new System.Drawing.Point(0, 0);
            this.pnlLetter.Name = "pnlLetter";
            this.pnlLetter.Size = new System.Drawing.Size(795, 106);
            this.pnlLetter.TabIndex = 0;
            // 
            // pnlLabels
            // 
            this.pnlLabels.Controls.Add(this.pnlTRight);
            this.pnlLabels.Controls.Add(this.pnlTLeft);
            this.pnlLabels.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlLabels.Location = new System.Drawing.Point(0, 0);
            this.pnlLabels.Name = "pnlLabels";
            this.pnlLabels.Size = new System.Drawing.Size(795, 106);
            this.pnlLabels.TabIndex = 10;
            // 
            // pnlTLeft
            // 
            this.pnlTLeft.Controls.Add(this.panel8);
            this.pnlTLeft.Controls.Add(this.panel7);
            this.pnlTLeft.Controls.Add(this.panel6);
            this.pnlTLeft.Controls.Add(this.panel5);
            this.pnlTLeft.Dock = Wisej.Web.DockStyle.Left;
            this.pnlTLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlTLeft.Name = "pnlTLeft";
            this.pnlTLeft.Size = new System.Drawing.Size(392, 106);
            this.pnlTLeft.TabIndex = 1;
            // 
            // pnllblMessage5
            // 
            this.pnllblMessage5.Controls.Add(this.lblMessage5);
            this.pnllblMessage5.Dock = Wisej.Web.DockStyle.Fill;
            this.pnllblMessage5.Location = new System.Drawing.Point(0, 0);
            this.pnllblMessage5.Name = "pnllblMessage5";
            this.pnllblMessage5.Padding = new Wisej.Web.Padding(15, 0, 0, 0);
            this.pnllblMessage5.Size = new System.Drawing.Size(392, 31);
            this.pnllblMessage5.TabIndex = 3;
            // 
            // lblMessage5
            // 
            this.lblMessage5.AutoSize = true;
            this.lblMessage5.Dock = Wisej.Web.DockStyle.Fill;
            this.lblMessage5.Font = new System.Drawing.Font("@defaultBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblMessage5.ForeColor = System.Drawing.Color.Orange;
            this.lblMessage5.Location = new System.Drawing.Point(15, 0);
            this.lblMessage5.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblMessage5.Name = "lblMessage5";
            this.lblMessage5.Size = new System.Drawing.Size(377, 31);
            this.lblMessage5.TabIndex = 1;
            this.lblMessage5.Text = "....";
            this.lblMessage5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnllblMessage4
            // 
            this.pnllblMessage4.Controls.Add(this.lblMessage4);
            this.pnllblMessage4.Dock = Wisej.Web.DockStyle.Fill;
            this.pnllblMessage4.Location = new System.Drawing.Point(0, 0);
            this.pnllblMessage4.Name = "pnllblMessage4";
            this.pnllblMessage4.Padding = new Wisej.Web.Padding(15, 0, 0, 0);
            this.pnllblMessage4.Size = new System.Drawing.Size(392, 25);
            this.pnllblMessage4.TabIndex = 2;
            // 
            // lblMessage4
            // 
            this.lblMessage4.AutoSize = true;
            this.lblMessage4.Dock = Wisej.Web.DockStyle.Fill;
            this.lblMessage4.Font = new System.Drawing.Font("@defaultBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblMessage4.ForeColor = System.Drawing.Color.Orange;
            this.lblMessage4.Location = new System.Drawing.Point(15, 0);
            this.lblMessage4.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblMessage4.Name = "lblMessage4";
            this.lblMessage4.Size = new System.Drawing.Size(377, 25);
            this.lblMessage4.TabIndex = 1;
            this.lblMessage4.Text = "....";
            this.lblMessage4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnllblMessage3
            // 
            this.pnllblMessage3.Controls.Add(this.lblMessage3);
            this.pnllblMessage3.Dock = Wisej.Web.DockStyle.Fill;
            this.pnllblMessage3.Location = new System.Drawing.Point(0, 0);
            this.pnllblMessage3.Name = "pnllblMessage3";
            this.pnllblMessage3.Padding = new Wisej.Web.Padding(15, 0, 0, 0);
            this.pnllblMessage3.Size = new System.Drawing.Size(392, 25);
            this.pnllblMessage3.TabIndex = 1;
            // 
            // lblMessage3
            // 
            this.lblMessage3.AutoSize = true;
            this.lblMessage3.Dock = Wisej.Web.DockStyle.Fill;
            this.lblMessage3.Font = new System.Drawing.Font("@defaultBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblMessage3.ForeColor = System.Drawing.Color.Orange;
            this.lblMessage3.Location = new System.Drawing.Point(15, 0);
            this.lblMessage3.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblMessage3.Name = "lblMessage3";
            this.lblMessage3.Size = new System.Drawing.Size(377, 25);
            this.lblMessage3.TabIndex = 1;
            this.lblMessage3.Text = "....";
            this.lblMessage3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnllblMessage2
            // 
            this.pnllblMessage2.Controls.Add(this.lblMessage2);
            this.pnllblMessage2.Dock = Wisej.Web.DockStyle.Fill;
            this.pnllblMessage2.Location = new System.Drawing.Point(0, 0);
            this.pnllblMessage2.Name = "pnllblMessage2";
            this.pnllblMessage2.Padding = new Wisej.Web.Padding(15, 0, 0, 0);
            this.pnllblMessage2.Size = new System.Drawing.Size(392, 25);
            this.pnllblMessage2.TabIndex = 0;
            // 
            // pnlTRight
            // 
            this.pnlTRight.Controls.Add(this.pnllblCaseWorker);
            this.pnlTRight.Controls.Add(this.pnllblletterdate);
            this.pnlTRight.Controls.Add(this.pnllblMessage1);
            this.pnlTRight.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlTRight.Location = new System.Drawing.Point(392, 0);
            this.pnlTRight.Name = "pnlTRight";
            this.pnlTRight.Size = new System.Drawing.Size(403, 106);
            this.pnlTRight.TabIndex = 0;
            // 
            // pnllblCaseWorker
            // 
            this.pnllblCaseWorker.Controls.Add(this.label5);
            this.pnllblCaseWorker.Controls.Add(this.lblCaseWorker);
            this.pnllblCaseWorker.Controls.Add(this.spacer5);
            this.pnllblCaseWorker.Controls.Add(this.cmbCaseWorker);
            this.pnllblCaseWorker.Dock = Wisej.Web.DockStyle.Top;
            this.pnllblCaseWorker.Location = new System.Drawing.Point(0, 64);
            this.pnllblCaseWorker.Name = "pnllblCaseWorker";
            this.pnllblCaseWorker.Padding = new Wisej.Web.Padding(0, 3, 25, 3);
            this.pnllblCaseWorker.Size = new System.Drawing.Size(403, 31);
            this.pnllblCaseWorker.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = Wisej.Web.DockStyle.Right;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(6, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(9, 25);
            this.label5.TabIndex = 28;
            this.label5.Text = "*";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblCaseWorker
            // 
            this.lblCaseWorker.AutoSize = true;
            this.lblCaseWorker.Dock = Wisej.Web.DockStyle.Right;
            this.lblCaseWorker.Location = new System.Drawing.Point(15, 3);
            this.lblCaseWorker.Name = "lblCaseWorker";
            this.lblCaseWorker.Size = new System.Drawing.Size(74, 25);
            this.lblCaseWorker.TabIndex = 4;
            this.lblCaseWorker.Text = "Case Worker";
            this.lblCaseWorker.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spacer5
            // 
            this.spacer5.Dock = Wisej.Web.DockStyle.Right;
            this.spacer5.Location = new System.Drawing.Point(89, 3);
            this.spacer5.Name = "spacer5";
            this.spacer5.Size = new System.Drawing.Size(20, 25);
            // 
            // cmbCaseWorker
            // 
            this.cmbCaseWorker.Dock = Wisej.Web.DockStyle.Right;
            this.cmbCaseWorker.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbCaseWorker.Enabled = false;
            this.cmbCaseWorker.FormattingEnabled = true;
            this.cmbCaseWorker.Location = new System.Drawing.Point(109, 3);
            this.cmbCaseWorker.Name = "cmbCaseWorker";
            this.cmbCaseWorker.Size = new System.Drawing.Size(269, 25);
            this.cmbCaseWorker.TabIndex = 22;
            // 
            // pnllblletterdate
            // 
            this.pnllblletterdate.Controls.Add(this.lblLetterDateReq);
            this.pnllblletterdate.Controls.Add(this.lblletterdate);
            this.pnllblletterdate.Controls.Add(this.spacer4);
            this.pnllblletterdate.Controls.Add(this.dtletterDate);
            this.pnllblletterdate.Dock = Wisej.Web.DockStyle.Top;
            this.pnllblletterdate.Location = new System.Drawing.Point(0, 33);
            this.pnllblletterdate.Name = "pnllblletterdate";
            this.pnllblletterdate.Padding = new Wisej.Web.Padding(0, 3, 25, 3);
            this.pnllblletterdate.Size = new System.Drawing.Size(403, 31);
            this.pnllblletterdate.TabIndex = 1;
            // 
            // lblLetterDateReq
            // 
            this.lblLetterDateReq.AutoSize = true;
            this.lblLetterDateReq.Dock = Wisej.Web.DockStyle.Right;
            this.lblLetterDateReq.ForeColor = System.Drawing.Color.Red;
            this.lblLetterDateReq.Location = new System.Drawing.Point(150, 3);
            this.lblLetterDateReq.Name = "lblLetterDateReq";
            this.lblLetterDateReq.Size = new System.Drawing.Size(9, 25);
            this.lblLetterDateReq.TabIndex = 28;
            this.lblLetterDateReq.Text = "*";
            this.lblLetterDateReq.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblLetterDateReq.Visible = false;
            // 
            // lblletterdate
            // 
            this.lblletterdate.Dock = Wisej.Web.DockStyle.Right;
            this.lblletterdate.Location = new System.Drawing.Point(159, 3);
            this.lblletterdate.Name = "lblletterdate";
            this.lblletterdate.Size = new System.Drawing.Size(83, 25);
            this.lblletterdate.TabIndex = 0;
            this.lblletterdate.Text = "Letter Sent On";
            this.lblletterdate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spacer4
            // 
            this.spacer4.Dock = Wisej.Web.DockStyle.Right;
            this.spacer4.Location = new System.Drawing.Point(242, 3);
            this.spacer4.Name = "spacer4";
            this.spacer4.Size = new System.Drawing.Size(20, 25);
            // 
            // dtletterDate
            // 
            this.dtletterDate.CustomFormat = "MM/dd/yyyy";
            this.dtletterDate.Dock = Wisej.Web.DockStyle.Right;
            this.dtletterDate.Enabled = false;
            this.dtletterDate.Format = Wisej.Web.DateTimePickerFormat.Custom;
            this.dtletterDate.Location = new System.Drawing.Point(262, 3);
            this.dtletterDate.Name = "dtletterDate";
            this.dtletterDate.ShowCheckBox = true;
            this.dtletterDate.Size = new System.Drawing.Size(116, 25);
            this.dtletterDate.TabIndex = 1;
            // 
            // pnllblMessage1
            // 
            this.pnllblMessage1.Controls.Add(this.lblMessage1);
            this.pnllblMessage1.Dock = Wisej.Web.DockStyle.Top;
            this.pnllblMessage1.Location = new System.Drawing.Point(0, 0);
            this.pnllblMessage1.Name = "pnllblMessage1";
            this.pnllblMessage1.Padding = new Wisej.Web.Padding(15, 0, 25, 0);
            this.pnllblMessage1.Size = new System.Drawing.Size(403, 33);
            this.pnllblMessage1.TabIndex = 0;
            // 
            // lblMessage1
            // 
            this.lblMessage1.AutoSize = true;
            this.lblMessage1.Dock = Wisej.Web.DockStyle.Fill;
            this.lblMessage1.Font = new System.Drawing.Font("@defaultBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblMessage1.ForeColor = System.Drawing.Color.Red;
            this.lblMessage1.Location = new System.Drawing.Point(15, 0);
            this.lblMessage1.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblMessage1.Name = "lblMessage1";
            this.lblMessage1.Size = new System.Drawing.Size(363, 33);
            this.lblMessage1.TabIndex = 0;
            this.lblMessage1.Text = ".....";
            this.lblMessage1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.pnllblMessage2);
            this.panel5.Dock = Wisej.Web.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(392, 25);
            this.panel5.TabIndex = 1;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.pnllblMessage3);
            this.panel6.Dock = Wisej.Web.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 25);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(392, 25);
            this.panel6.TabIndex = 2;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.pnllblMessage4);
            this.panel7.Dock = Wisej.Web.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(0, 50);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(392, 25);
            this.panel7.TabIndex = 3;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.pnllblMessage5);
            this.panel8.Dock = Wisej.Web.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 75);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(392, 31);
            this.panel8.TabIndex = 4;
            // 
            // lblMessage2
            // 
            this.lblMessage2.AutoSize = true;
            this.lblMessage2.Dock = Wisej.Web.DockStyle.Fill;
            this.lblMessage2.Font = new System.Drawing.Font("@defaultBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblMessage2.ForeColor = System.Drawing.Color.Orange;
            this.lblMessage2.Location = new System.Drawing.Point(15, 0);
            this.lblMessage2.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblMessage2.Name = "lblMessage2";
            this.lblMessage2.Size = new System.Drawing.Size(377, 25);
            this.lblMessage2.TabIndex = 1;
            this.lblMessage2.Text = ".....";
            this.lblMessage2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // IncomepleteIntakeControl
            // 
            this.Controls.Add(this.pnlControl);
            this.Name = "IncomepleteIntakeControl";
            this.Size = new System.Drawing.Size(1300, 513);
            this.Controls.SetChildIndex(this.pnlControl, 0);
            this.txtLine1.ResumeLayout(false);
            this.txtLine1.PerformLayout();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvwIncompletedata)).EndInit();
            this.pnlControl.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlParams.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlSaveB.ResumeLayout(false);
            this.pnlSave.ResumeLayout(false);
            this.pnlgvwIncompletedata.ResumeLayout(false);
            this.pnlLetter.ResumeLayout(false);
            this.pnlLabels.ResumeLayout(false);
            this.pnlTLeft.ResumeLayout(false);
            this.pnllblMessage5.ResumeLayout(false);
            this.pnllblMessage5.PerformLayout();
            this.pnllblMessage4.ResumeLayout(false);
            this.pnllblMessage4.PerformLayout();
            this.pnllblMessage3.ResumeLayout(false);
            this.pnllblMessage3.PerformLayout();
            this.pnllblMessage2.ResumeLayout(false);
            this.pnllblMessage2.PerformLayout();
            this.pnlTRight.ResumeLayout(false);
            this.pnllblCaseWorker.ResumeLayout(false);
            this.pnllblCaseWorker.PerformLayout();
            this.pnllblletterdate.ResumeLayout(false);
            this.pnllblletterdate.PerformLayout();
            this.pnllblMessage1.ResumeLayout(false);
            this.pnllblMessage1.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private Button btnOk;
        private Button btnClose;
        private Label label24;
        private TextBox txtLine1;
        private Label label1;
        private Panel pnlDetails;
        private DataGridView gvwIncompletedata;
        private Panel pnlControl;
        private DataGridViewCheckBoxColumn gvchkSelect;
        private DataGridViewTextBoxColumn gvtDesc;
        private DataGridViewTextBoxColumn gvtCode;
        private DataGridViewTextBoxColumn gvtLine1;
        private DataGridViewTextBoxColumn gvtLine2;
        private DataGridViewTextBoxColumn gvtLine3;
        private Panel pnlLetter;
        private Label lblMessage1;
        private DataGridViewTextBoxColumn gvtLetterDate;
        private Label lblCaseWorker;
        private ComboBoxEx cmbCaseWorker;
        private DateTimePicker dtletterDate;
        private Label lblletterdate;
        private Label label5;
        private Label lblLetterDateReq;
        private Label lblMessage4;
        private Label lblMessage3;
        private Label label2;
        private Label lblMessage5;
        private Panel pnlgvwIncompletedata;
        private Panel pnlParams;
        private Panel pnlSaveB;
        private Panel pnlSave;
        private Spacer spacer1;
        private Panel pnlTop;
        private Panel pnlBottom;
        private Spacer spacer3;
        private Spacer spacer2;
        private Panel pnlLabels;
        private Panel pnlTRight;
        private Panel pnllblCaseWorker;
        private Panel pnllblletterdate;
        private Panel pnllblMessage1;
        private Spacer spacer5;
        private Spacer spacer4;
        private Panel pnlTLeft;
        private Panel pnllblMessage4;
        private Panel pnllblMessage3;
        private Panel pnllblMessage2;
        private Panel pnllblMessage5;
        private Panel panel8;
        private Panel panel7;
        private Panel panel6;
        private Panel panel5;
        private Label lblMessage2;
    }
}