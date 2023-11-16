using Wisej.Web;
using Wisej.Web.Ext.AspNetControl;

namespace Captain.Common.Views.Forms
{
    partial class CASB2012_AdhocRDLCForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CASB2012_AdhocRDLCForm));
            this.btnGetSummary = new Wisej.Web.Button();
            this.btnNotePad = new Wisej.Web.Button();
            this.btnPreviewReport = new Wisej.Web.Button();
            this.btnGenExcel = new Wisej.Web.Button();
            this.flowLayoutPanel1 = new Wisej.Web.FlowLayoutPanel();
            this.reportViewer1 = new Wisej.AspNetReportViewer.AspNetReportViewer();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGetSummary
            // 
            this.btnGetSummary.Font = new System.Drawing.Font("@buttonTextFont", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnGetSummary.Location = new System.Drawing.Point(460, 8);
            this.btnGetSummary.Name = "btnGetSummary";
            this.btnGetSummary.Size = new System.Drawing.Size(95, 25);
            this.btnGetSummary.TabIndex = 3;
            this.btnGetSummary.Text = "Get &Summary";
            this.btnGetSummary.Visible = false;
            this.btnGetSummary.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnNotePad
            // 
            this.btnNotePad.Font = new System.Drawing.Font("@buttonTextFont", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnNotePad.Location = new System.Drawing.Point(329, 8);
            this.btnNotePad.Name = "btnNotePad";
            this.btnNotePad.Size = new System.Drawing.Size(125, 25);
            this.btnNotePad.TabIndex = 2;
            this.btnNotePad.Text = "&Generate NotePad";
            this.btnNotePad.Visible = false;
            this.btnNotePad.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnPreviewReport
            // 
            this.btnPreviewReport.BackColor = System.Drawing.Color.FromName("@window");
            this.btnPreviewReport.Font = new System.Drawing.Font("@buttonTextFont", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnPreviewReport.Location = new System.Drawing.Point(213, 8);
            this.btnPreviewReport.Name = "btnPreviewReport";
            this.btnPreviewReport.Size = new System.Drawing.Size(110, 25);
            this.btnPreviewReport.TabIndex = 1;
            this.btnPreviewReport.Text = "&Preview Reports";
            this.btnPreviewReport.Visible = false;
            this.btnPreviewReport.Click += new System.EventHandler(this.btnPreviewReport_Click);
            // 
            // btnGenExcel
            // 
            this.btnGenExcel.Font = new System.Drawing.Font("@buttonTextFont", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnGenExcel.Location = new System.Drawing.Point(561, 8);
            this.btnGenExcel.Name = "btnGenExcel";
            this.btnGenExcel.Size = new System.Drawing.Size(100, 25);
            this.btnGenExcel.TabIndex = 4;
            this.btnGenExcel.Text = "Generate &Excel";
            this.btnGenExcel.Visible = false;
            this.btnGenExcel.Click += new System.EventHandler(this.btnGenExcel_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AppearanceKey = "panel-grdo";
            this.flowLayoutPanel1.Controls.Add(this.btnGenExcel);
            this.flowLayoutPanel1.Controls.Add(this.btnGetSummary);
            this.flowLayoutPanel1.Controls.Add(this.btnNotePad);
            this.flowLayoutPanel1.Controls.Add(this.btnPreviewReport);
            this.flowLayoutPanel1.Dock = Wisej.Web.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = Wisej.Web.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 512);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new Wisej.Web.Padding(5, 5, 150, 5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(819, 35);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.TabStop = true;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = Wisej.Web.DockStyle.Fill;
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ShowExportControls = true;
            this.reportViewer1.Size = new System.Drawing.Size(819, 512);
            this.reportViewer1.TabIndex = 0;
            this.reportViewer1.Text = "aspNetReportViewerControl1";
            this.reportViewer1.Toggle += new System.ComponentModel.CancelEventHandler(this.reportViewer1_Toggle);
            this.reportViewer1.BookmarkNavigation += new Microsoft.Reporting.WebForms.BookmarkNavigationEventHandler(this.reportViewer1_BookmarkNavigation);
            this.reportViewer1.Init += new System.EventHandler(this.reportViewer1_Init);
            this.reportViewer1.Load += new System.EventHandler(this.reportViewer1_Load);
            // 
            // CASB2012_AdhocRDLCForm
            // 
            this.ClientSize = new System.Drawing.Size(819, 547);
            this.Controls.Add(this.reportViewer1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Image)(resources.GetObject("$this.Icon")));
            this.LiveResize = true;
            this.Name = "CASB2012_AdhocRDLCForm";
            this.Text = "CASB2012_AdhocRDLCForm";
            this.FormClosing += new Wisej.Web.FormClosingEventHandler(this.CASB2012_AdhocRDLCForm_FormClosing);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Wisej.AspNetReportViewer.AspNetReportViewer reportViewer1;
        private Button btnGetSummary;
        private Button btnNotePad;
        private Button btnPreviewReport;
        private Button btnGenExcel;
        private FlowLayoutPanel flowLayoutPanel1;
    }
}