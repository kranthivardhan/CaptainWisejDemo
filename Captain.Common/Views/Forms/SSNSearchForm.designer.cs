﻿using Captain.Common.Views.Controls.Compatibility;
using Wisej.Web;


namespace Captain.Common.Views.Forms
{
    partial class SSNSearchForm
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
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle10 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle5 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle6 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle7 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle8 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle9 = new Wisej.Web.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SSNSearchForm));
            this.label4 = new Wisej.Web.Label();
            this.gvwSSNSearch = new Captain.Common.Views.Controls.Compatibility.DataGridViewEx();
            this.SsnNo = new Captain.Common.Views.Controls.Compatibility.DataGridViewDateTimeColumn();
            this.SsnName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.phone = new Wisej.Web.DataGridViewMaskedTextBoxColumn();
            this.address = new Wisej.Web.DataGridViewTextBoxColumn();
            this.AppKey = new Wisej.Web.DataGridViewTextBoxColumn();
            this.MemSeq = new Wisej.Web.DataGridViewTextBoxColumn();
            this.btnSearch = new Wisej.Web.Button();
            this.mskSSNNO = new Wisej.Web.MaskedTextBox();
            this.label3 = new Wisej.Web.Label();
            this.label2 = new Wisej.Web.Label();
            this.mskTelePhone = new Wisej.Web.MaskedTextBox();
            this.Addresspanel = new Wisej.Web.Panel();
            this.DOBReq = new Wisej.Web.Label();
            this.lblDOB = new Wisej.Web.Label();
            this.dtBirth = new Wisej.Web.DateTimePicker();
            this.lblssnreq = new Wisej.Web.Label();
            this.label6 = new Wisej.Web.Label();
            this.label5 = new Wisej.Web.Label();
            this.txtAlias = new Captain.Common.Views.Controls.Compatibility.TextBoxWithValidation();
            this.cbduplicates = new Wisej.Web.CheckBox();
            this.lblSSN = new Wisej.Web.Label();
            this.txtLastName = new Captain.Common.Views.Controls.Compatibility.TextBoxWithValidation();
            this.txtFirstName = new Captain.Common.Views.Controls.Compatibility.TextBoxWithValidation();
            this.label11 = new Wisej.Web.Label();
            this.txtState = new Captain.Common.Views.Controls.Compatibility.TextBoxWithValidation();
            this.txtCity = new Captain.Common.Views.Controls.Compatibility.TextBoxWithValidation();
            this.label10 = new Wisej.Web.Label();
            this.label9 = new Wisej.Web.Label();
            this.txtStreet = new Captain.Common.Views.Controls.Compatibility.TextBoxWithValidation();
            this.txtHouseNo = new Captain.Common.Views.Controls.Compatibility.TextBoxWithValidation();
            this.label7 = new Wisej.Web.Label();
            this.lblFooter = new Wisej.Web.Label();
            this.lblApp = new Wisej.Web.Label();
            this.btnSSNSelect = new Wisej.Web.Button();
            this.panel1 = new Wisej.Web.Panel();
            this.btnMember = new Wisej.Web.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvwSSNSearch)).BeginInit();
            this.Addresspanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-58, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 14);
            this.label4.TabIndex = 2;
            this.label4.Text = "Site No";
            // 
            // gvwSSNSearch
            // 
            this.gvwSSNSearch.AllowUserToResizeColumns = false;
            this.gvwSSNSearch.AllowUserToResizeRows = false;
            this.gvwSSNSearch.AutoSizeRowsMode = Wisej.Web.DataGridViewAutoSizeRowsMode.AllCells;
            this.gvwSSNSearch.BackColor = System.Drawing.Color.FromArgb(253, 253, 253);
            dataGridViewCellStyle1.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.gvwSSNSearch.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gvwSSNSearch.ColumnHeadersHeightSizeMode = Wisej.Web.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvwSSNSearch.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.SsnNo,
            this.SsnName,
            this.phone,
            this.address,
            this.AppKey,
            this.MemSeq});
            dataGridViewCellStyle10.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.gvwSSNSearch.DefaultCellStyle = dataGridViewCellStyle10;
            this.gvwSSNSearch.Dock = Wisej.Web.DockStyle.Fill;
            this.gvwSSNSearch.Location = new System.Drawing.Point(0, 123);
            this.gvwSSNSearch.MultiSelect = false;
            this.gvwSSNSearch.Name = "gvwSSNSearch";
            this.gvwSSNSearch.ReadOnly = true;
            this.gvwSSNSearch.RowHeadersWidth = 20;
            this.gvwSSNSearch.RowHeadersWidthSizeMode = Wisej.Web.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gvwSSNSearch.ScrollBars = Wisej.Web.ScrollBars.Vertical;
            this.gvwSSNSearch.Size = new System.Drawing.Size(762, 309);
            this.gvwSSNSearch.TabIndex = 0;
            this.gvwSSNSearch.TabStop = false;
            this.gvwSSNSearch.SelectionChanged += new System.EventHandler(this.gvwSSNSearch_SelectionChanged);
            // 
            // SsnNo
            // 
            dataGridViewCellStyle2.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.SsnNo.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewCellStyle3.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.SsnNo.HeaderStyle = dataGridViewCellStyle3;
            this.SsnNo.HeaderText = "DOB";
            this.SsnNo.Name = "SsnNo";
            this.SsnNo.ReadOnly = true;
            // 
            // SsnName
            // 
            dataGridViewCellStyle4.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.SsnName.DefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.SsnName.HeaderStyle = dataGridViewCellStyle5;
            this.SsnName.HeaderText = "Name";
            this.SsnName.Name = "SsnName";
            this.SsnName.ReadOnly = true;
            this.SsnName.Width = 240;
            // 
            // phone
            // 
            dataGridViewCellStyle6.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.phone.DefaultCellStyle = dataGridViewCellStyle6;
            dataGridViewCellStyle7.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.phone.HeaderStyle = dataGridViewCellStyle7;
            this.phone.HeaderText = "Phone";
            this.phone.Mask = "(999) 000-0000";
            this.phone.Name = "phone";
            this.phone.ReadOnly = true;
            this.phone.Width = 120;
            // 
            // address
            // 
            dataGridViewCellStyle8.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.address.DefaultCellStyle = dataGridViewCellStyle8;
            dataGridViewCellStyle9.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.address.HeaderStyle = dataGridViewCellStyle9;
            this.address.HeaderText = "Address";
            this.address.Name = "address";
            this.address.ReadOnly = true;
            this.address.Width = 260;
            // 
            // AppKey
            // 
            this.AppKey.HeaderText = "AppKey";
            this.AppKey.Name = "AppKey";
            this.AppKey.ReadOnly = true;
            this.AppKey.ShowInVisibilityMenu = false;
            this.AppKey.Visible = false;
            // 
            // MemSeq
            // 
            this.MemSeq.HeaderText = "MemSeq";
            this.MemSeq.Name = "MemSeq";
            this.MemSeq.ReadOnly = true;
            this.MemSeq.ShowInVisibilityMenu = false;
            this.MemSeq.Visible = false;
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnSearch.Location = new System.Drawing.Point(660, 93);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(83, 25);
            this.btnSearch.TabIndex = 12;
            this.btnSearch.Text = "S&earch";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // mskSSNNO
            // 
            this.mskSSNNO.Location = new System.Drawing.Point(80, 8);
            this.mskSSNNO.Mask = "000-00-0000";
            this.mskSSNNO.Name = "mskSSNNO";
            this.mskSSNNO.Size = new System.Drawing.Size(98, 25);
            this.mskSSNNO.TabIndex = 1;
            this.mskSSNNO.TextMaskFormat = Wisej.Web.MaskFormat.ExcludePromptAndLiterals;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label3.Location = new System.Drawing.Point(444, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 14);
            this.label3.TabIndex = 1;
            this.label3.Text = "Last Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.Location = new System.Drawing.Point(222, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "First Name";
            // 
            // mskTelePhone
            // 
            this.mskTelePhone.Location = new System.Drawing.Point(293, 64);
            this.mskTelePhone.Mask = "(999) 000-0000";
            this.mskTelePhone.Name = "mskTelePhone";
            this.mskTelePhone.Size = new System.Drawing.Size(108, 25);
            this.mskTelePhone.TabIndex = 9;
            this.mskTelePhone.TextMaskFormat = Wisej.Web.MaskFormat.ExcludePromptAndLiterals;
            // 
            // Addresspanel
            // 
            this.Addresspanel.Controls.Add(this.DOBReq);
            this.Addresspanel.Controls.Add(this.lblDOB);
            this.Addresspanel.Controls.Add(this.dtBirth);
            this.Addresspanel.Controls.Add(this.lblssnreq);
            this.Addresspanel.Controls.Add(this.label6);
            this.Addresspanel.Controls.Add(this.mskTelePhone);
            this.Addresspanel.Controls.Add(this.label5);
            this.Addresspanel.Controls.Add(this.txtAlias);
            this.Addresspanel.Controls.Add(this.cbduplicates);
            this.Addresspanel.Controls.Add(this.lblSSN);
            this.Addresspanel.Controls.Add(this.txtLastName);
            this.Addresspanel.Controls.Add(this.btnSearch);
            this.Addresspanel.Controls.Add(this.mskSSNNO);
            this.Addresspanel.Controls.Add(this.txtFirstName);
            this.Addresspanel.Controls.Add(this.label11);
            this.Addresspanel.Controls.Add(this.label3);
            this.Addresspanel.Controls.Add(this.label2);
            this.Addresspanel.Controls.Add(this.txtState);
            this.Addresspanel.Controls.Add(this.txtCity);
            this.Addresspanel.Controls.Add(this.label10);
            this.Addresspanel.Controls.Add(this.label9);
            this.Addresspanel.Controls.Add(this.txtStreet);
            this.Addresspanel.Controls.Add(this.txtHouseNo);
            this.Addresspanel.Controls.Add(this.label7);
            this.Addresspanel.Dock = Wisej.Web.DockStyle.Top;
            this.Addresspanel.Location = new System.Drawing.Point(0, 0);
            this.Addresspanel.Name = "Addresspanel";
            this.Addresspanel.Size = new System.Drawing.Size(762, 123);
            this.Addresspanel.TabIndex = 1;
            // 
            // DOBReq
            // 
            this.DOBReq.AutoSize = true;
            this.DOBReq.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.DOBReq.ForeColor = System.Drawing.Color.FromArgb(251, 255, 0, 0);
            this.DOBReq.Location = new System.Drawing.Point(471, 64);
            this.DOBReq.Name = "DOBReq";
            this.DOBReq.Size = new System.Drawing.Size(9, 14);
            this.DOBReq.TabIndex = 28;
            this.DOBReq.Text = "*";
            this.DOBReq.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.DOBReq.Visible = false;
            // 
            // lblDOB
            // 
            this.lblDOB.AutoSize = true;
            this.lblDOB.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblDOB.Location = new System.Drawing.Point(444, 68);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size(31, 14);
            this.lblDOB.TabIndex = 0;
            this.lblDOB.Text = "DOB";
            // 
            // dtBirth
            // 
            this.dtBirth.Checked = false;
            this.dtBirth.CustomFormat = "MM/dd/yyyy";
            this.dtBirth.Format = Wisej.Web.DateTimePickerFormat.Custom;
            this.dtBirth.Location = new System.Drawing.Point(512, 64);
            this.dtBirth.MaximumSize = new System.Drawing.Size(0, 25);
            this.dtBirth.MinimumSize = new System.Drawing.Size(0, 25);
            this.dtBirth.Name = "dtBirth";
            this.dtBirth.ShowCheckBox = true;
            this.dtBirth.Size = new System.Drawing.Size(116, 25);
            this.dtBirth.TabIndex = 10;
            // 
            // lblssnreq
            // 
            this.lblssnreq.AutoSize = true;
            this.lblssnreq.ForeColor = System.Drawing.Color.FromArgb(253, 255, 0, 0);
            this.lblssnreq.Location = new System.Drawing.Point(43, 9);
            this.lblssnreq.Name = "lblssnreq";
            this.lblssnreq.Size = new System.Drawing.Size(9, 14);
            this.lblssnreq.TabIndex = 16;
            this.lblssnreq.Text = "*";
            this.lblssnreq.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label6.Location = new System.Drawing.Point(222, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 14);
            this.label6.TabIndex = 15;
            this.label6.Text = "Phone";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label5.Location = new System.Drawing.Point(17, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 14);
            this.label5.TabIndex = 14;
            this.label5.Text = "Alias";
            // 
            // txtAlias
            // 
            this.txtAlias.Location = new System.Drawing.Point(80, 65);
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(98, 25);
            this.txtAlias.TabIndex = 8;
            // 
            // cbduplicates
            // 
            this.cbduplicates.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cbduplicates.Location = new System.Drawing.Point(514, 93);
            this.cbduplicates.Name = "cbduplicates";
            this.cbduplicates.Size = new System.Drawing.Size(122, 21);
            this.cbduplicates.TabIndex = 11;
            this.cbduplicates.Text = "Show Duplicates";
            this.cbduplicates.Visible = false;
            // 
            // lblSSN
            // 
            this.lblSSN.AutoSize = true;
            this.lblSSN.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblSSN.Location = new System.Drawing.Point(17, 12);
            this.lblSSN.Name = "lblSSN";
            this.lblSSN.Size = new System.Drawing.Size(29, 14);
            this.lblSSN.TabIndex = 3;
            this.lblSSN.Text = "SSN";
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(512, 8);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(231, 25);
            this.txtLastName.TabIndex = 3;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(293, 8);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(108, 25);
            this.txtFirstName.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label11.Location = new System.Drawing.Point(637, 40);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(33, 14);
            this.label11.TabIndex = 0;
            this.label11.Text = "State";
            // 
            // txtState
            // 
            this.txtState.Location = new System.Drawing.Point(676, 36);
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(67, 25);
            this.txtState.TabIndex = 7;
            // 
            // txtCity
            // 
            this.txtCity.Location = new System.Drawing.Point(512, 36);
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(116, 25);
            this.txtCity.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label10.Location = new System.Drawing.Point(444, 40);
            this.label10.MaximumSize = new System.Drawing.Size(0, 21);
            this.label10.MinimumSize = new System.Drawing.Size(0, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(25, 18);
            this.label10.TabIndex = 0;
            this.label10.Text = "City";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label9.Location = new System.Drawing.Point(222, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 14);
            this.label9.TabIndex = 0;
            this.label9.Text = "Street";
            // 
            // txtStreet
            // 
            this.txtStreet.Location = new System.Drawing.Point(293, 36);
            this.txtStreet.Name = "txtStreet";
            this.txtStreet.Size = new System.Drawing.Size(108, 25);
            this.txtStreet.TabIndex = 5;
            // 
            // txtHouseNo
            // 
            this.txtHouseNo.Location = new System.Drawing.Point(80, 37);
            this.txtHouseNo.Name = "txtHouseNo";
            this.txtHouseNo.Size = new System.Drawing.Size(98, 25);
            this.txtHouseNo.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label7.Location = new System.Drawing.Point(17, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 14);
            this.label7.TabIndex = 0;
            this.label7.Text = "House No";
            // 
            // lblFooter
            // 
            this.lblFooter.AutoSize = true;
            this.lblFooter.Dock = Wisej.Web.DockStyle.Left;
            this.lblFooter.Font = new System.Drawing.Font("@defaultBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblFooter.Location = new System.Drawing.Point(15, 5);
            this.lblFooter.MinimumSize = new System.Drawing.Size(0, 18);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(4, 25);
            this.lblFooter.TabIndex = 13;
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblApp
            // 
            this.lblApp.AutoSize = true;
            this.lblApp.Location = new System.Drawing.Point(8, 369);
            this.lblApp.Name = "lblApp";
            this.lblApp.Size = new System.Drawing.Size(4, 14);
            this.lblApp.TabIndex = 14;
            // 
            // btnSSNSelect
            // 
            this.btnSSNSelect.Dock = Wisej.Web.DockStyle.Right;
            this.btnSSNSelect.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnSSNSelect.Location = new System.Drawing.Point(672, 5);
            this.btnSSNSelect.Name = "btnSSNSelect";
            this.btnSSNSelect.Size = new System.Drawing.Size(75, 25);
            this.btnSSNSelect.TabIndex = 1;
            this.btnSSNSelect.Text = "&Select";
            this.btnSSNSelect.Click += new System.EventHandler(this.btnSSNSelect_Click);
            // 
            // panel1
            // 
            this.panel1.AppearanceKey = "panel-grdo";
            this.panel1.Controls.Add(this.btnMember);
            this.panel1.Controls.Add(this.btnSSNSelect);
            this.panel1.Controls.Add(this.lblFooter);
            this.panel1.Dock = Wisej.Web.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 432);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new Wisej.Web.Padding(15, 5, 15, 5);
            this.panel1.Size = new System.Drawing.Size(762, 35);
            this.panel1.TabIndex = 2;
            // 
            // btnMember
            // 
            this.btnMember.Dock = Wisej.Web.DockStyle.Left;
            this.btnMember.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnMember.Location = new System.Drawing.Point(19, 5);
            this.btnMember.Name = "btnMember";
            this.btnMember.Size = new System.Drawing.Size(126, 25);
            this.btnMember.TabIndex = 14;
            this.btnMember.Text = "Add New Member";
            this.btnMember.Visible = false;
            this.btnMember.Click += new System.EventHandler(this.btnMember_Click);
            // 
            // SSNSearchForm
            // 
            this.ClientSize = new System.Drawing.Size(762, 467);
            this.Controls.Add(this.gvwSSNSearch);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblApp);
            this.Controls.Add(this.Addresspanel);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.Icon = ((System.Drawing.Image)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SSNSearchForm";
            this.Text = "Household Member Search";
            ((System.ComponentModel.ISupportInitialize)(this.gvwSSNSearch)).EndInit();
            this.Addresspanel.ResumeLayout(false);
            this.Addresspanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label label4;
        private DataGridViewEx gvwSSNSearch;
        private DataGridViewTextBoxColumn SsnName;
        private Button btnSearch;
        private MaskedTextBox mskSSNNO;
        private TextBoxWithValidation txtLastName;
        private TextBoxWithValidation txtFirstName;
        private Label label3;
        private Label label2;
        private TextBoxWithValidation txtAlias;
        private MaskedTextBox mskTelePhone;
        private Panel Addresspanel;
        private Label label11;
        private TextBoxWithValidation txtState;
        private TextBoxWithValidation txtCity;
        private Label label10;
        private Label label9;
        private TextBoxWithValidation txtStreet;
        private TextBoxWithValidation txtHouseNo;
        private Label label7;
        private DataGridViewTextBoxColumn address;
        private CheckBox cbduplicates;
        private DataGridViewTextBoxColumn AppKey;
        private DataGridViewTextBoxColumn MemSeq;
        private Label label6;
        private Label label5;
        private Label lblSSN;
        private Label lblFooter;
        private DataGridViewMaskedTextBoxColumn phone;
        private Label lblApp;
        private Label lblssnreq;
        private Label lblDOB;
        private Label DOBReq;
        private DateTimePicker dtBirth;
        private Button btnSSNSelect;
        private Panel panel1;
        private DataGridViewDateTimeColumn SsnNo;
        private Button btnMember;
    }
}