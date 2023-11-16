using Wisej.Web;
using Captain.Common.Views.Controls.Compatibility;

namespace Captain.Common.Views.Forms
{
    partial class DIMSCOREREPORT
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

        #region Visual WebGui Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DIMSCOREREPORT));
            this.CmbYear = new Wisej.Web.ComboBox();
            this.Txt_HieDesc = new Wisej.Web.TextBox();
            this.pnlHie = new Wisej.Web.Panel();
            this.spacer3 = new Wisej.Web.Spacer();
            this.Pb_Search_Hie = new Wisej.Web.PictureBox();
            this.btnSaveParameters = new Wisej.Web.Button();
            this.btnGetParameters = new Wisej.Web.Button();
            this.BtnGenPdf = new Wisej.Web.Button();
            this.BtnPdfPrev = new Wisej.Web.Button();
            this.pnlGenerateBtns = new Wisej.Web.Panel();
            this.spacer2 = new Wisej.Web.Spacer();
            this.spacer1 = new Wisej.Web.Spacer();
            this.lblRepOpt = new Wisej.Web.Label();
            this.lblFrom = new Wisej.Web.Label();
            this.lblTo = new Wisej.Web.Label();
            this.lblRepType = new Wisej.Web.Label();
            this.rdoSelectApplicant = new Wisej.Web.RadioButton();
            this.rdoall = new Wisej.Web.RadioButton();
            this.pnlReportType = new Wisej.Web.Panel();
            this.lblApplicantReq = new Wisej.Web.Label();
            this.btnBrowse = new Wisej.Web.Button();
            this.txtApplicant = new Wisej.Web.TextBox();
            this.lblApplicant = new Wisej.Web.Label();
            this.cmbdimension = new Wisej.Web.ComboBox();
            this.lblHealthInsurance = new Wisej.Web.Label();
            this.cmbPressgcat = new Wisej.Web.ComboBox();
            this.lblFrmDt = new Wisej.Web.Label();
            this.lblToDt = new Wisej.Web.Label();
            this.dtpToDt = new Wisej.Web.DateTimePicker();
            this.dtpFrmDate = new Wisej.Web.DateTimePicker();
            this.cmbPressGroup1 = new Wisej.Web.ComboBox();
            this.lblPreass = new Wisej.Web.Label();
            this.pnlCompleteForm = new Wisej.Web.Panel();
            this.pnlDates = new Wisej.Web.Panel();
            this.pnlDimension = new Wisej.Web.Panel();
            this.pnlApplicant = new Wisej.Web.Panel();
            this.panel1 = new Wisej.Web.Panel();
            this.txtTo = new Captain.Common.Views.Controls.Compatibility.TextBoxWithValidation();
            this.txtfrom = new Captain.Common.Views.Controls.Compatibility.TextBoxWithValidation();
            this.pnlPreassCmb = new Wisej.Web.Panel();
            this.pnlHieandFilter = new Wisej.Web.Panel();
            this.pnlFilter = new Wisej.Web.Panel();
            this.pnlHie.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pb_Search_Hie)).BeginInit();
            this.pnlGenerateBtns.SuspendLayout();
            this.pnlReportType.SuspendLayout();
            this.pnlCompleteForm.SuspendLayout();
            this.pnlDates.SuspendLayout();
            this.pnlDimension.SuspendLayout();
            this.pnlApplicant.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlPreassCmb.SuspendLayout();
            this.pnlHieandFilter.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // CmbYear
            // 
            this.CmbYear.Dock = Wisej.Web.DockStyle.Left;
            this.CmbYear.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.CmbYear.FormattingEnabled = true;
            this.CmbYear.Location = new System.Drawing.Point(732, 9);
            this.CmbYear.Name = "CmbYear";
            this.CmbYear.Size = new System.Drawing.Size(64, 25);
            this.CmbYear.TabIndex = 0;
            this.CmbYear.Visible = false;
            this.CmbYear.SelectedIndexChanged += new System.EventHandler(this.CmbYear_SelectedIndexChanged);
            // 
            // Txt_HieDesc
            // 
            this.Txt_HieDesc.BackColor = System.Drawing.Color.Transparent;
            this.Txt_HieDesc.BorderStyle = Wisej.Web.BorderStyle.None;
            this.Txt_HieDesc.Dock = Wisej.Web.DockStyle.Left;
            this.Txt_HieDesc.Font = new System.Drawing.Font("defaultBold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.Txt_HieDesc.ForeColor = System.Drawing.Color.White;
            this.Txt_HieDesc.Location = new System.Drawing.Point(15, 9);
            this.Txt_HieDesc.Name = "Txt_HieDesc";
            this.Txt_HieDesc.ReadOnly = true;
            this.Txt_HieDesc.Size = new System.Drawing.Size(700, 25);
            this.Txt_HieDesc.TabIndex = 0;
            this.Txt_HieDesc.TabStop = false;
            this.Txt_HieDesc.TextAlign = Wisej.Web.HorizontalAlignment.Center;
            // 
            // pnlHie
            // 
            this.pnlHie.BackColor = System.Drawing.Color.FromArgb(253, 11, 70, 117);
            this.pnlHie.Controls.Add(this.CmbYear);
            this.pnlHie.Controls.Add(this.spacer3);
            this.pnlHie.Controls.Add(this.Txt_HieDesc);
            this.pnlHie.Dock = Wisej.Web.DockStyle.Left;
            this.pnlHie.Location = new System.Drawing.Point(0, 0);
            this.pnlHie.Name = "pnlHie";
            this.pnlHie.Padding = new Wisej.Web.Padding(15, 9, 9, 9);
            this.pnlHie.Size = new System.Drawing.Size(802, 43);
            this.pnlHie.TabIndex = 0;
            // 
            // spacer3
            // 
            this.spacer3.Dock = Wisej.Web.DockStyle.Left;
            this.spacer3.Location = new System.Drawing.Point(715, 9);
            this.spacer3.Name = "spacer3";
            this.spacer3.Size = new System.Drawing.Size(17, 25);
            // 
            // Pb_Search_Hie
            // 
            this.Pb_Search_Hie.BackColor = System.Drawing.Color.FromArgb(244, 244, 244);
            this.Pb_Search_Hie.CssStyle = "border-radius:25px";
            this.Pb_Search_Hie.Cursor = Wisej.Web.Cursors.Hand;
            this.Pb_Search_Hie.Dock = Wisej.Web.DockStyle.Left;
            this.Pb_Search_Hie.ImageSource = "captain-filter";
            this.Pb_Search_Hie.Location = new System.Drawing.Point(9, 9);
            this.Pb_Search_Hie.Name = "Pb_Search_Hie";
            this.Pb_Search_Hie.Padding = new Wisej.Web.Padding(4, 5, 4, 4);
            this.Pb_Search_Hie.Size = new System.Drawing.Size(25, 25);
            this.Pb_Search_Hie.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.Pb_Search_Hie.ToolTipText = "Select Hierarchy";
            this.Pb_Search_Hie.Click += new System.EventHandler(this.Pb_Search_Hie_Click);
            // 
            // btnSaveParameters
            // 
            this.btnSaveParameters.AppearanceKey = "button-reports";
            this.btnSaveParameters.Dock = Wisej.Web.DockStyle.Left;
            this.btnSaveParameters.Location = new System.Drawing.Point(128, 5);
            this.btnSaveParameters.Name = "btnSaveParameters";
            this.btnSaveParameters.Size = new System.Drawing.Size(110, 25);
            this.btnSaveParameters.TabIndex = 20;
            this.btnSaveParameters.Text = "&Save Parameters";
            this.btnSaveParameters.Click += new System.EventHandler(this.btnSaveParameters_Click);
            // 
            // btnGetParameters
            // 
            this.btnGetParameters.AppearanceKey = "button-reports";
            this.btnGetParameters.Dock = Wisej.Web.DockStyle.Left;
            this.btnGetParameters.Location = new System.Drawing.Point(15, 5);
            this.btnGetParameters.Name = "btnGetParameters";
            this.btnGetParameters.Size = new System.Drawing.Size(110, 25);
            this.btnGetParameters.TabIndex = 19;
            this.btnGetParameters.Text = "Get &Parameters";
            this.btnGetParameters.Click += new System.EventHandler(this.btnGetParameters_Click);
            // 
            // BtnGenPdf
            // 
            this.BtnGenPdf.AppearanceKey = "button-reports";
            this.BtnGenPdf.Dock = Wisej.Web.DockStyle.Right;
            this.BtnGenPdf.Location = new System.Drawing.Point(671, 5);
            this.BtnGenPdf.Name = "BtnGenPdf";
            this.BtnGenPdf.Size = new System.Drawing.Size(80, 25);
            this.BtnGenPdf.TabIndex = 21;
            this.BtnGenPdf.Text = "&Generate";
            this.BtnGenPdf.Click += new System.EventHandler(this.BtnGenPdf_Click);
            // 
            // BtnPdfPrev
            // 
            this.BtnPdfPrev.AppearanceKey = "button-reports";
            this.BtnPdfPrev.Dock = Wisej.Web.DockStyle.Right;
            this.BtnPdfPrev.Font = new System.Drawing.Font("@buttonTextFont", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.BtnPdfPrev.Location = new System.Drawing.Point(754, 5);
            this.BtnPdfPrev.Name = "BtnPdfPrev";
            this.BtnPdfPrev.Size = new System.Drawing.Size(80, 25);
            this.BtnPdfPrev.TabIndex = 22;
            this.BtnPdfPrev.Text = "Pre&view";
            this.BtnPdfPrev.Click += new System.EventHandler(this.BtnPdfPrev_Click);
            // 
            // pnlGenerateBtns
            // 
            this.pnlGenerateBtns.AppearanceKey = "panel-grdo";
            this.pnlGenerateBtns.Controls.Add(this.btnSaveParameters);
            this.pnlGenerateBtns.Controls.Add(this.spacer2);
            this.pnlGenerateBtns.Controls.Add(this.BtnGenPdf);
            this.pnlGenerateBtns.Controls.Add(this.spacer1);
            this.pnlGenerateBtns.Controls.Add(this.btnGetParameters);
            this.pnlGenerateBtns.Controls.Add(this.BtnPdfPrev);
            this.pnlGenerateBtns.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlGenerateBtns.Location = new System.Drawing.Point(0, 241);
            this.pnlGenerateBtns.Name = "pnlGenerateBtns";
            this.pnlGenerateBtns.Padding = new Wisej.Web.Padding(15, 5, 15, 5);
            this.pnlGenerateBtns.Size = new System.Drawing.Size(849, 35);
            this.pnlGenerateBtns.TabIndex = 18;
            // 
            // spacer2
            // 
            this.spacer2.Dock = Wisej.Web.DockStyle.Left;
            this.spacer2.Location = new System.Drawing.Point(125, 5);
            this.spacer2.Name = "spacer2";
            this.spacer2.Size = new System.Drawing.Size(3, 25);
            // 
            // spacer1
            // 
            this.spacer1.Dock = Wisej.Web.DockStyle.Right;
            this.spacer1.Location = new System.Drawing.Point(751, 5);
            this.spacer1.Name = "spacer1";
            this.spacer1.Size = new System.Drawing.Size(3, 25);
            // 
            // lblRepOpt
            // 
            this.lblRepOpt.Location = new System.Drawing.Point(15, 8);
            this.lblRepOpt.Name = "lblRepOpt";
            this.lblRepOpt.Size = new System.Drawing.Size(92, 16);
            this.lblRepOpt.TabIndex = 0;
            this.lblRepOpt.Text = "Preass Category";
            // 
            // lblFrom
            // 
            this.lblFrom.Location = new System.Drawing.Point(354, 8);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(30, 14);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "From";
            // 
            // lblTo
            // 
            this.lblTo.Location = new System.Drawing.Point(469, 8);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(14, 14);
            this.lblTo.TabIndex = 0;
            this.lblTo.Text = "To";
            // 
            // lblRepType
            // 
            this.lblRepType.Location = new System.Drawing.Point(15, 8);
            this.lblRepType.Name = "lblRepType";
            this.lblRepType.Size = new System.Drawing.Size(69, 16);
            this.lblRepType.TabIndex = 0;
            this.lblRepType.Text = "Report Type";
            // 
            // rdoSelectApplicant
            // 
            this.rdoSelectApplicant.AutoSize = false;
            this.rdoSelectApplicant.Location = new System.Drawing.Point(193, 5);
            this.rdoSelectApplicant.Name = "rdoSelectApplicant";
            this.rdoSelectApplicant.Size = new System.Drawing.Size(131, 20);
            this.rdoSelectApplicant.TabIndex = 9;
            this.rdoSelectApplicant.Text = "Selected Applicant";
            this.rdoSelectApplicant.CheckedChanged += new System.EventHandler(this.rdoSelectApplicant_CheckedChanged);
            // 
            // rdoall
            // 
            this.rdoall.AutoSize = false;
            this.rdoall.Checked = true;
            this.rdoall.Location = new System.Drawing.Point(135, 5);
            this.rdoall.Name = "rdoall";
            this.rdoall.Size = new System.Drawing.Size(41, 20);
            this.rdoall.TabIndex = 8;
            this.rdoall.TabStop = true;
            this.rdoall.Text = "All";
            this.rdoall.CheckedChanged += new System.EventHandler(this.rdoall_CheckedChanged);
            // 
            // pnlReportType
            // 
            this.pnlReportType.Controls.Add(this.rdoSelectApplicant);
            this.pnlReportType.Controls.Add(this.rdoall);
            this.pnlReportType.Controls.Add(this.lblRepType);
            this.pnlReportType.Controls.Add(this.lblApplicantReq);
            this.pnlReportType.Dock = Wisej.Web.DockStyle.Top;
            this.pnlReportType.Location = new System.Drawing.Point(0, 114);
            this.pnlReportType.Name = "pnlReportType";
            this.pnlReportType.Size = new System.Drawing.Size(849, 27);
            this.pnlReportType.TabIndex = 7;
            // 
            // lblApplicantReq
            // 
            this.lblApplicantReq.ForeColor = System.Drawing.Color.Red;
            this.lblApplicantReq.Location = new System.Drawing.Point(84, 6);
            this.lblApplicantReq.Name = "lblApplicantReq";
            this.lblApplicantReq.Size = new System.Drawing.Size(7, 11);
            this.lblApplicantReq.TabIndex = 28;
            this.lblApplicantReq.Text = "*";
            this.lblApplicantReq.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblApplicantReq.Visible = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Enabled = false;
            this.btnBrowse.Location = new System.Drawing.Point(234, 5);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(65, 25);
            this.btnBrowse.TabIndex = 12;
            this.btnBrowse.Text = "&Browse";
            this.btnBrowse.ToolTipText = "Select Applicant";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtApplicant
            // 
            this.txtApplicant.Enabled = false;
            this.txtApplicant.Location = new System.Drawing.Point(135, 5);
            this.txtApplicant.MaxLength = 9;
            this.txtApplicant.Name = "txtApplicant";
            this.txtApplicant.Size = new System.Drawing.Size(71, 25);
            this.txtApplicant.TabIndex = 11;
            this.txtApplicant.TextAlign = Wisej.Web.HorizontalAlignment.Right;
            this.txtApplicant.Leave += new System.EventHandler(this.txtApplicant_Leave);
            // 
            // lblApplicant
            // 
            this.lblApplicant.Location = new System.Drawing.Point(15, 8);
            this.lblApplicant.Name = "lblApplicant";
            this.lblApplicant.Size = new System.Drawing.Size(104, 16);
            this.lblApplicant.TabIndex = 5;
            this.lblApplicant.Text = "Applicant Number";
            // 
            // cmbdimension
            // 
            this.cmbdimension.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbdimension.FormattingEnabled = true;
            this.cmbdimension.Location = new System.Drawing.Point(135, 5);
            this.cmbdimension.Name = "cmbdimension";
            this.cmbdimension.Size = new System.Drawing.Size(192, 25);
            this.cmbdimension.TabIndex = 14;
            // 
            // lblHealthInsurance
            // 
            this.lblHealthInsurance.Location = new System.Drawing.Point(15, 8);
            this.lblHealthInsurance.Name = "lblHealthInsurance";
            this.lblHealthInsurance.Size = new System.Drawing.Size(62, 14);
            this.lblHealthInsurance.TabIndex = 0;
            this.lblHealthInsurance.Text = "Dimension";
            // 
            // cmbPressgcat
            // 
            this.cmbPressgcat.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbPressgcat.FormattingEnabled = true;
            this.cmbPressgcat.Location = new System.Drawing.Point(135, 5);
            this.cmbPressgcat.Name = "cmbPressgcat";
            this.cmbPressgcat.Size = new System.Drawing.Size(192, 25);
            this.cmbPressgcat.TabIndex = 4;
            this.cmbPressgcat.SelectedIndexChanged += new System.EventHandler(this.cmbPressgcat_SelectedIndexChanged);
            // 
            // lblFrmDt
            // 
            this.lblFrmDt.Location = new System.Drawing.Point(15, 8);
            this.lblFrmDt.Name = "lblFrmDt";
            this.lblFrmDt.RightToLeft = Wisej.Web.RightToLeft.No;
            this.lblFrmDt.Size = new System.Drawing.Size(60, 14);
            this.lblFrmDt.TabIndex = 0;
            this.lblFrmDt.Text = "From Date";
            // 
            // lblToDt
            // 
            this.lblToDt.Location = new System.Drawing.Point(283, 8);
            this.lblToDt.Name = "lblToDt";
            this.lblToDt.RightToLeft = Wisej.Web.RightToLeft.No;
            this.lblToDt.Size = new System.Drawing.Size(44, 14);
            this.lblToDt.TabIndex = 0;
            this.lblToDt.Text = "To Date";
            // 
            // dtpToDt
            // 
            this.dtpToDt.AutoSize = false;
            this.dtpToDt.Checked = false;
            this.dtpToDt.CustomFormat = "MM/dd/yyyy";
            this.dtpToDt.Format = Wisej.Web.DateTimePickerFormat.Custom;
            this.dtpToDt.Location = new System.Drawing.Point(338, 5);
            this.dtpToDt.Name = "dtpToDt";
            this.dtpToDt.ShowCheckBox = true;
            this.dtpToDt.Size = new System.Drawing.Size(116, 25);
            this.dtpToDt.TabIndex = 17;
            // 
            // dtpFrmDate
            // 
            this.dtpFrmDate.AutoSize = false;
            this.dtpFrmDate.Checked = false;
            this.dtpFrmDate.CustomFormat = "MM/dd/yyyy";
            this.dtpFrmDate.Format = Wisej.Web.DateTimePickerFormat.Custom;
            this.dtpFrmDate.Location = new System.Drawing.Point(135, 5);
            this.dtpFrmDate.Name = "dtpFrmDate";
            this.dtpFrmDate.ShowCheckBox = true;
            this.dtpFrmDate.Size = new System.Drawing.Size(116, 25);
            this.dtpFrmDate.TabIndex = 16;
            // 
            // cmbPressGroup1
            // 
            this.cmbPressGroup1.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbPressGroup1.FormattingEnabled = true;
            this.cmbPressGroup1.Location = new System.Drawing.Point(135, 11);
            this.cmbPressGroup1.Name = "cmbPressGroup1";
            this.cmbPressGroup1.Size = new System.Drawing.Size(192, 25);
            this.cmbPressGroup1.TabIndex = 1;
            this.cmbPressGroup1.SelectedIndexChanged += new System.EventHandler(this.cmbPressGroup1_SelectedIndexChanged);
            // 
            // lblPreass
            // 
            this.lblPreass.Location = new System.Drawing.Point(15, 15);
            this.lblPreass.Name = "lblPreass";
            this.lblPreass.Size = new System.Drawing.Size(77, 16);
            this.lblPreass.TabIndex = 0;
            this.lblPreass.Text = "Preass Group";
            // 
            // pnlCompleteForm
            // 
            this.pnlCompleteForm.Controls.Add(this.pnlDates);
            this.pnlCompleteForm.Controls.Add(this.pnlDimension);
            this.pnlCompleteForm.Controls.Add(this.pnlApplicant);
            this.pnlCompleteForm.Controls.Add(this.pnlReportType);
            this.pnlCompleteForm.Controls.Add(this.panel1);
            this.pnlCompleteForm.Controls.Add(this.pnlPreassCmb);
            this.pnlCompleteForm.Controls.Add(this.pnlHieandFilter);
            this.pnlCompleteForm.Controls.Add(this.pnlGenerateBtns);
            this.pnlCompleteForm.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlCompleteForm.Location = new System.Drawing.Point(0, 0);
            this.pnlCompleteForm.Name = "pnlCompleteForm";
            this.pnlCompleteForm.Size = new System.Drawing.Size(849, 276);
            this.pnlCompleteForm.TabIndex = 0;
            // 
            // pnlDates
            // 
            this.pnlDates.Controls.Add(this.dtpToDt);
            this.pnlDates.Controls.Add(this.lblFrmDt);
            this.pnlDates.Controls.Add(this.lblToDt);
            this.pnlDates.Controls.Add(this.dtpFrmDate);
            this.pnlDates.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlDates.Location = new System.Drawing.Point(0, 205);
            this.pnlDates.Name = "pnlDates";
            this.pnlDates.Size = new System.Drawing.Size(849, 36);
            this.pnlDates.TabIndex = 15;
            // 
            // pnlDimension
            // 
            this.pnlDimension.Controls.Add(this.cmbdimension);
            this.pnlDimension.Controls.Add(this.lblHealthInsurance);
            this.pnlDimension.Dock = Wisej.Web.DockStyle.Top;
            this.pnlDimension.Location = new System.Drawing.Point(0, 173);
            this.pnlDimension.Name = "pnlDimension";
            this.pnlDimension.Size = new System.Drawing.Size(849, 32);
            this.pnlDimension.TabIndex = 13;
            // 
            // pnlApplicant
            // 
            this.pnlApplicant.Controls.Add(this.btnBrowse);
            this.pnlApplicant.Controls.Add(this.txtApplicant);
            this.pnlApplicant.Controls.Add(this.lblApplicant);
            this.pnlApplicant.Dock = Wisej.Web.DockStyle.Top;
            this.pnlApplicant.Location = new System.Drawing.Point(0, 141);
            this.pnlApplicant.Name = "pnlApplicant";
            this.pnlApplicant.Size = new System.Drawing.Size(849, 32);
            this.pnlApplicant.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtTo);
            this.panel1.Controls.Add(this.cmbPressgcat);
            this.panel1.Controls.Add(this.txtfrom);
            this.panel1.Controls.Add(this.lblTo);
            this.panel1.Controls.Add(this.lblFrom);
            this.panel1.Controls.Add(this.lblRepOpt);
            this.panel1.Dock = Wisej.Web.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(849, 32);
            this.panel1.TabIndex = 3;
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(501, 5);
            this.txtTo.MaxLength = 3;
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(51, 25);
            this.txtTo.TabIndex = 6;
            // 
            // txtfrom
            // 
            this.txtfrom.Location = new System.Drawing.Point(395, 5);
            this.txtfrom.MaxLength = 3;
            this.txtfrom.Name = "txtfrom";
            this.txtfrom.Size = new System.Drawing.Size(47, 25);
            this.txtfrom.TabIndex = 5;
            // 
            // pnlPreassCmb
            // 
            this.pnlPreassCmb.Controls.Add(this.cmbPressGroup1);
            this.pnlPreassCmb.Controls.Add(this.lblPreass);
            this.pnlPreassCmb.Dock = Wisej.Web.DockStyle.Top;
            this.pnlPreassCmb.Location = new System.Drawing.Point(0, 43);
            this.pnlPreassCmb.Name = "pnlPreassCmb";
            this.pnlPreassCmb.Size = new System.Drawing.Size(849, 39);
            this.pnlPreassCmb.TabIndex = 0;
            // 
            // pnlHieandFilter
            // 
            this.pnlHieandFilter.BackColor = System.Drawing.Color.FromArgb(253, 11, 70, 117);
            this.pnlHieandFilter.Controls.Add(this.pnlFilter);
            this.pnlHieandFilter.Controls.Add(this.pnlHie);
            this.pnlHieandFilter.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHieandFilter.HeaderForeColor = System.Drawing.Color.FromArgb(11, 70, 117);
            this.pnlHieandFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlHieandFilter.Name = "pnlHieandFilter";
            this.pnlHieandFilter.Size = new System.Drawing.Size(849, 43);
            this.pnlHieandFilter.TabIndex = 0;
            // 
            // pnlFilter
            // 
            this.pnlFilter.BackColor = System.Drawing.Color.FromArgb(253, 11, 70, 117);
            this.pnlFilter.Controls.Add(this.Pb_Search_Hie);
            this.pnlFilter.CssStyle = "border-radius:25px";
            this.pnlFilter.Dock = Wisej.Web.DockStyle.Left;
            this.pnlFilter.Location = new System.Drawing.Point(802, 0);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Padding = new Wisej.Web.Padding(9);
            this.pnlFilter.Size = new System.Drawing.Size(44, 43);
            this.pnlFilter.TabIndex = 0;
            // 
            // DIMSCOREREPORT
            // 
            this.ClientSize = new System.Drawing.Size(849, 276);
            this.Controls.Add(this.pnlCompleteForm);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.Icon = ((System.Drawing.Image)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DIMSCOREREPORT";
            this.Text = "DIMSCOREREPORT";
            this.pnlHie.ResumeLayout(false);
            this.pnlHie.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pb_Search_Hie)).EndInit();
            this.pnlGenerateBtns.ResumeLayout(false);
            this.pnlReportType.ResumeLayout(false);
            this.pnlCompleteForm.ResumeLayout(false);
            this.pnlDates.ResumeLayout(false);
            this.pnlDimension.ResumeLayout(false);
            this.pnlDimension.PerformLayout();
            this.pnlApplicant.ResumeLayout(false);
            this.pnlApplicant.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlPreassCmb.ResumeLayout(false);
            this.pnlPreassCmb.PerformLayout();
            this.pnlHieandFilter.ResumeLayout(false);
            this.pnlFilter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComboBox CmbYear;
        private TextBox Txt_HieDesc;
        private Panel pnlHie;
        private PictureBox Pb_Search_Hie;
        private Button btnSaveParameters;
        private Button btnGetParameters;
        private Button BtnGenPdf;
        private Button BtnPdfPrev;
        private Panel pnlGenerateBtns;
        private Label lblRepOpt;
        private Label lblFrom;
        private Label lblTo;
        private TextBoxWithValidation txtfrom;
        private TextBoxWithValidation txtTo;
        private Label lblRepType;
        private RadioButton rdoSelectApplicant;
        private RadioButton rdoall;
        private Panel pnlReportType;
        private Label lblApplicantReq;
        private Button btnBrowse;
        private TextBox txtApplicant;
        private Label lblApplicant;
        private ComboBox cmbdimension;
        private Label lblHealthInsurance;
        private ComboBox cmbPressgcat;
        private Label lblFrmDt;
        private Label lblToDt;
        private DateTimePicker dtpToDt;
        private DateTimePicker dtpFrmDate;
        private ComboBox cmbPressGroup1;
        private Label lblPreass;
        private Panel pnlCompleteForm;
        private Spacer spacer2;
        private Spacer spacer1;
        private Panel pnlHieandFilter;
        private Panel pnlFilter;
        private Panel panel1;
        private Panel pnlPreassCmb;
        private Panel pnlApplicant;
        private Panel pnlDates;
        private Panel pnlDimension;
        private Spacer spacer3;
    }
}