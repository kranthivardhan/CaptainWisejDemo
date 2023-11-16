#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Wisej.Web;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.IO;

#endregion

namespace Captain.Common.Views.Forms
{
    public partial class PdfViewerNewForm : Form
    {
        FileStream stream = null;
        public PdfViewerNewForm(string strFileName)
        {
            //string strpath = GetSiteUrl();
            //strpath = strpath.Replace("///", "/");
            //strFileName = strFileName.Replace(" ", "%20");
            ////URI uri = new URI(string.replace(" ", "%20"));
            //strpath = strpath + "ViewPdfForm.aspx?Name=" + strFileName;

            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            //pnlDetails.Visible = false;
            ////this.htmlBox1.Dock = Wisej.Web.DockStyle.Fill;
            //this.pnlDetails.Dock = Wisej.Web.DockStyle.None;

            //System.Drawing.Image IMG;
            //if (strFileName.ToUpper().Contains(".JPG") || strFileName.ToUpper().Contains(".JPEG") || strFileName.ToUpper().Contains(".PNG") || strFileName.ToUpper().Contains(".BMP"))
            //{
            
            pdfViewer.Visible = false;
            htmlPanel1.Visible = true;
            //htmlPanel1.Dock = DockStyle.Fill;

            string strpath = GetSiteUrl();
            strpath = strpath.Replace("///", "/");
            strFileName = strFileName.Replace(" ", "%20");
            //URI uri = new URI(string.replace(" ", "%20"));
            strpath = strpath + "ViewPdfForm.aspx?Name=" + strFileName;

            this.htmlPanel1.Html = "<HTML><iframe src=" + strpath + " style='display: block; height: 100vh; width: 100vw;border: none;'></iframe></HTML>";

            this.htmlPanel1.Dock = DockStyle.Fill;
            this.pnlDetails.Dock = DockStyle.None;

            //picboxImage.Visible = true;
            //pdfViewer.Visible = false;
            //try
            //{
            //    //image.BeginInit();
            //    //image.CacheOption = BitmapCacheOption.OnLoad;
            //    if (picboxImage.Image != null) picboxImage.Image.Dispose();
            //    stream = new FileStream(strFileName, FileMode.Open, FileAccess.Read);
            //    picboxImage.Image = System.Drawing.Image.FromStream(stream);
            //    //stream.Dispose();
            //    picboxImage.Visible = true;
            //}
            //catch (Exception ex)
            //{
            //}

            //}
            //else if (strFileName.ToUpper().Contains(".PDF"))
            //{
            //    //this.htmlBox1.Html = "<HTML><iframe src=" + strpath + " height=100% width=100%></iframe></HTML>";

            //    picboxImage.Visible = false;
            //    pdfViewer.Visible = true;
            //    this.pdfViewer.PdfSource = strFileName;
            //}
        }

        public PdfViewerNewForm(string strFileName, string strtype)
        {
            //Alaa ::
            //string strpath = GetSiteUrl();
            //strpath = strpath.Replace("///", "/");
            //strFileName = strFileName.Replace(" ", "%20");
            //URI uri = new URI(string.replace(" ", "%20"));
            //strpath = strpath + "CaptchaImage.aspx";

            InitializeComponent();
            //pnlDetails.Visible = false;
            //this.htmlBox1.Dock = Wisej.Web.DockStyle.Fill;
            //this.pnlDetails.Dock = Wisej.Web.DockStyle.None;

            //System.Drawing.Image IMG;
            //if (strFileName.ToUpper().Contains(".JPG") || strFileName.ToUpper().Contains(".JPEG") || strFileName.ToUpper().Contains(".PNG") || strFileName.ToUpper().Contains(".BMP"))
            //{
            //    try
            //    {
            //        IMG = System.Drawing.Image.FromFile(strFileName);
            //        this.htmlBox1.Html = "<HTML><iframe src=" + strpath + " height= 100% width = 100%></iframe></HTML>";
            //        this.Size = new Size(IMG.Width, IMG.Height);
            //        this.htmlBox1.Dock = DockStyle.Fill;
            //        this.Dock = DockStyle.Fill;
            //    }
            //    catch (Exception ex)
            //    {
            //        this.htmlBox1.Html = "<HTML><iframe src=" + strpath + " height=100% width=100%></iframe></HTML>";
            //    }

            //}
            //else
            //{
            //this.htmlBox1.Html = "<HTML><iframe src=" + strpath + " height=100% width=100%></iframe></HTML>";
            // }
        }

        public PdfViewerNewForm(string strFileName, string strApplicantNo, string struserid, string strFileType)
        {
            InitializeComponent();

            pdfViewer.Visible = false;

            htmlPanel1.Visible = true;
            htmlPanel1.Dock = DockStyle.Fill;

            string strpath = GetSiteUrl();
            strpath = strpath.Replace("///", "/");
            strFileName = strFileName.Replace(" ", "%20");
            // URI uri = new URI(string.replace(" ", "%20"));
            strpath = strpath + "signature.aspx?id=" + strFileName + "&actid=" + strApplicantNo + "&userid=" + struserid + "&filetype=" + strFileType;

            this.htmlPanel1.Html = "<HTML><iframe src=" + strpath + " style='display: block; height: 100vh; width: 100vw;border: none;'></iframe></HTML>";

            pnlDetails.Visible = false;


            // this.htmlBox1.Dock = Wisej.Web.DockStyle.Fill;
            //this.pnlDetails.Dock = Wisej.Web.DockStyle.None;
            //this.htmlPanel1.ImageSource = strpath; // "<HTML><iframe src=" + strpath + " height=440px width=761></iframe></HTML>";
        }

        public PdfViewerNewForm(string strFileName, DataSet Result_dataSet, DataTable Result_table, string report_name, string report_to_process, string reportpath, string strUserId, bool Detail_Rep_Required, string scr_Code)
        {
            //string strpath = "http://localhost:60865///"; //GetSiteUrl(); //
            //strpath = strpath.Replace("///", "/");
            //strFileName = strFileName.Replace(" ", "%20");
            ////URI uri = new URI(string.replace(" ", "%20"));
            //strpath = strpath + "ViewPdfForm.aspx?Name=" + strFileName;

            InitializeComponent();
            htmlPanel1.Visible = false;
            pdfViewer.Visible = true;

            UserId = strUserId;
            Scr_Code = scr_Code;
            Report_To_Process = report_to_process;
            Result_Table = Result_table;
            Report_Name = report_name;
            Result_DataSet = Result_dataSet;
            ReportPath = reportpath;

            if (Detail_Rep_Required)
            {
                //this.htmlBox1.Dock = Wisej.Web.DockStyle.Fill;
                pnlDetails.Visible = true;
                switch (Scr_Code)
                {
                    case "RNGB0004": Btn_Bypass.Visible = Btn_SNP_Details.Visible = Btn_MST_Details.Visible = true; break; // 
                    case "RNGB0014":
                        //this.Btn_Bypass.Location = new System.Drawing.Point(2, 507);
                        Btn_Bypass.Text = "Detail Report"; Btn_Bypass.Visible = true; break;
                }
            }
            else
            {
                //this.htmlBox1.Dock = Wisej.Web.DockStyle.Fill;
                pnlDetails.Visible = false;

                //this.pdfViewer.PdfSource = strFileName;
            }
            this.pdfViewer.PdfSource = strFileName;
            //this.htmlBox1.Html = "<HTML><iframe src=" + strpath + " height=100% width=100%></iframe></HTML>";
        }

        public PdfViewerNewForm(string strFileName, DataSet Result_dataSet, DataTable Result_table, string report_name, string report_to_process, string reportpath, string strUserId, bool Detail_Rep_Required, string scr_Code, string bypass_name, string mst_Rep_name, string ind_rep_name)
        {
            //string strpath = "http://localhost:60865///"; //GetSiteUrl(); //
            //strpath = strpath.Replace("///", "/");
            //strFileName = strFileName.Replace(" ", "%20");
            ////URI uri = new URI(string.replace(" ", "%20"));
            //strpath = strpath + "ViewPdfForm.aspx?Name=" + strFileName;
            InitializeComponent();
            htmlPanel1.Visible = false;
            pdfViewer.Visible = true;

            UserId = strUserId;
            Scr_Code = scr_Code;
            Report_To_Process = report_to_process;
            Result_Table = Result_table;
            Report_Name = report_name;
            Result_DataSet = Result_dataSet;
            ReportPath = reportpath;
            Bypass_Rep_Name = bypass_name; App_Rep_Name = mst_Rep_name; Ind_Rep_Name = ind_rep_name;

            if (Detail_Rep_Required)
            {
                //this.htmlBox1.Dock = Wisej.Web.DockStyle.Fill;
                pnlDetails.Visible = true;
                switch (Scr_Code)
                {
                    case "RNGB0004": Btn_Bypass.Visible = Btn_SNP_Details.Visible = Btn_MST_Details.Visible = true; break; // 
                    case "RNGB0014":
                        //this.Btn_Bypass.Location = new System.Drawing.Point(2, 507);
                        Btn_Bypass.Text = "Detail Report"; Btn_Bypass.Visible = true; break;
                }
            }
            else
            {
                //this.htmlBox1.Dock = Wisej.Web.DockStyle.Fill;
                pnlDetails.Visible = false;

                //this.pdfViewer.PdfSource = strFileName;
            }
            this.pdfViewer.PdfSource = strFileName;
            //this.htmlBox1.Html = "<HTML><iframe src=" + strpath + " height=100% width=100%></iframe></HTML>";
        }

        public string UserId { get; set; }
        public string Bypass_Rep_Name { get; set; }
        public string App_Rep_Name { get; set; }
        public string Ind_Rep_Name { get; set; }

        public static string GetSiteUrl()
        {
            string url = string.Empty;
            url = Application.Url;
            //HttpRequest request = HttpContext.Current.Request;


            //if (request.IsSecureConnection)
            //    url = "https://";
            //else
            //    url = "http://";

            //return url + HttpContext.Current.Request.Url.Authority + "/" + HttpContext.Current.Request.ApplicationPath + "/";

            return url;

        }

        #region properties

        public string Scr_Code { get; set; }

        public string Report_Name { get; set; }

        public string Report_To_Process { get; set; }

        public DataTable Result_Table { get; set; }

        public DataTable Summary_table { get; set; }

        public DataSet Result_DataSet { get; set; }

        public bool Detail_Rep_Required { get; set; }

        public string Main_Rep_Name { get; set; }

        public string ReportPath { get; set; }

        #endregion

        private void Btn_Bypass_Click(object sender, EventArgs e)
        {

            switch (Scr_Code)
            {
                case "RNGB0004":
                    //Report_Name = "RNGB0004_Bypass_RdlC_" + DateTime.Now.ToString("MMddyyyy") + "_" + DateTime.Now.ToString("HHmm") + ".rdlc";
                    Report_Name = Bypass_Rep_Name;
                    break;
                case "RNGB0014":
                    Report_Name = Report_Name;
                    break;
            }

            //CASB2012_AdhocRDLCForm RDLC_Form = new CASB2012_AdhocRDLCForm(Summary_table, Result_Table, Report_Name, "Result Table", ReportPath);
            CASB2012_AdhocRDLCForm RDLC_Form;
            if (Scr_Code == "RNGB0004")
                RDLC_Form = new CASB2012_AdhocRDLCForm(Result_DataSet.Tables[2], Summary_table, Report_Name, "Result Table", ReportPath, UserId, "RNG");
            else
                RDLC_Form = new CASB2012_AdhocRDLCForm(Result_Table, Summary_table, Report_Name, "Result Table", ReportPath, UserId, "RNG");
            //RDLC_Form.FormClosed += new FormClosedEventHandler(Delete_Dynamic_RDLC_File);
            RDLC_Form.ShowDialog();
        }

        private void Btn_SNP_Details_Click(object sender, EventArgs e)
        {
            //Report_Name = "RNGB0004_SNP_IND_RdlC_" + DateTime.Now.ToString("MMddyyyy") + "_" + DateTime.Now.ToString("HHmm") + ".rdlc";
            Report_Name = Ind_Rep_Name;
            CASB2012_AdhocRDLCForm RDLC_Form = new CASB2012_AdhocRDLCForm(Result_DataSet.Tables[3], Summary_table, Report_Name, "Result Table", ReportPath, UserId, "RNG"); //8, 5
            //RDLC_Form.FormClosed += new FormClosedEventHandler(Delete_Dynamic_RDLC_File);
            RDLC_Form.StartPosition = FormStartPosition.CenterScreen;
            RDLC_Form.ShowDialog();
        }

        private void Btn_MST_Details_Click(object sender, EventArgs e)
        {
            //Report_Name = "RNGB0004_MST_FAM_RdlC_" + DateTime.Now.ToString("MMddyyyy") + "_" + DateTime.Now.ToString("HHmm")+ ".rdlc";
            Report_Name = App_Rep_Name;
            CASB2012_AdhocRDLCForm RDLC_Form = new CASB2012_AdhocRDLCForm(Result_DataSet.Tables[4], Summary_table, Report_Name, "Result Table", ReportPath, UserId, "RNG"); //8, 8
            //RDLC_Form.FormClosed += new FormClosedEventHandler(Delete_Dynamic_RDLC_File);
            RDLC_Form.StartPosition = FormStartPosition.CenterScreen;
            RDLC_Form.ShowDialog();
        }

        private void PdfViewerNewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (stream != null)
                stream.Dispose();
        }
    }
}