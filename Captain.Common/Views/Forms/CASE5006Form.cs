/************************************************************************
 * Conversion On    :   11/25/2022      * Converted By     :   Kranthi
 * Modified On      :   12/06/2022      * Modified By      :   Kranthi
 * **********************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using Captain.Common.Utilities;
using Captain.Common.Views.Forms.Base;
using Captain.Common.Menus;
using Captain.Common.Views.Forms;
using System.Data.SqlClient;
using Captain.Common.Views.Controls;
using Captain.Common.Model.Objects;
using Captain.Common.Model.Data;
using System.Text.RegularExpressions;
using Captain.Common.Views.UserControls;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Captain.DatabaseLayer;
using Wisej.Web;
#endregion

namespace Captain.Common.Views.Forms
{
    public partial class CASE5006Form : Form
    {
        #region private variables

        private ErrorProvider _errorProvider = null;
        //private GridControl _intakeHierarchy = null;
        private bool boolChangeStatus = false;
        private CaptainModel _model = null;
        private List<HierarchyEntity> _selectedHierarchies = null;

        #endregion


        public CASE5006Form(BaseForm baseForm, string mode, string sp_code, string sp_sequence, string spm_year, PrivilegeEntity privileges, string hierarchy, string year, string app_no, List<FldcntlHieEntity> cntlCAEntity, List<FldcntlHieEntity> cntlMSEntity)
        {
            InitializeComponent();
            BaseForm = baseForm;
            Privileges = privileges;
            Mode = mode;
            CntlCAEntity = cntlCAEntity;
            CntlMSEntity = cntlMSEntity;
            //M_Hierarchy = hierarchy;
            //M_Year = year;
            SP_Code = sp_code;
            Sp_Sequence = sp_sequence;
            Spm_Year = spm_year;
            this.Text = privileges.PrivilegeName;
            Hierarchy = hierarchy;


            if (string.IsNullOrEmpty(year))
                year = "    ";
            Year = year;
            App_No = app_no;
            _model = new CaptainModel();
            _errorProvider = new ErrorProvider(this);
            _errorProvider.BlinkRate = 3;
            _errorProvider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            _errorProvider.Icon = null;
            propReportPath = _model.lookupDataAccess.GetReportPath();
            ProgramDefinitionEntity Caedep_List = new ProgramDefinitionEntity();
            Caedep_List = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            propResultsList = _model.SPAdminData.Get_AgyRecs("Results");
            propSearch_Entity = _model.SPAdminData.Browse_CASESP0List(null, null, null, null, null, null, null, null, null);
            // this.pictureBox1.Image = new Gizmox.WebGUI.Common.Resources.ImageResourceHandle("CoiSlogo.jpg"); //new Gizmox.WebGUI.Common.Resources.IconResourceHandle(resources.GetString("pictureBox1.Image"));
            //string Img_Tick = new Gizmox.WebGUI.Common.Resources.ImageResourceHandle("tick.ico");
            if (Caedep_List.DepYear.Trim() != BaseForm.BaseYear.Trim() && (BaseForm.UserProfile.Security == "R" || BaseForm.UserProfile.Security == "C"))
            {
                Privileges.AddPriv = Privileges.ChangePriv = Privileges.DelPriv = "false";
                Dep_Year_Mismatch = true;
                Btn_Save.Visible = false;
            }

            Enable_SPM_Controls_On_FLDCNTL();

            Get_App_MST_Details();

            Fill_All_DropDowns();

            Fill_Applicant_SPs();
            Fill_SP_DropDowns();
            Fill_Def_Program_Combo();

            //Fill_Additional_CAMS_Details(SP_Code);
            strFolderPath = Consts.Common.ReportFolderLocation + BaseForm.UserID + "\\";

            List<SERVSTOPEntity> SERVSTOPList = new List<SERVSTOPEntity>();
            SERVSTOPList = _model.CaseSumData.GetSERVSTOPDet(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, string.Empty, string.Empty);
            if (SERVSTOPList.Count == 0)
            {
                SERVSTOPList = _model.CaseSumData.GetSERVSTOPDet(BaseForm.BaseAgency, BaseForm.BaseDept, "**", string.Empty, string.Empty);
                if (SERVSTOPList.Count == 0)
                {
                    SERVSTOPList = _model.CaseSumData.GetSERVSTOPDet(BaseForm.BaseAgency, "**", "**", string.Empty, string.Empty);
                    if (SERVSTOPList.Count == 0)
                        SERVSTOPList = _model.CaseSumData.GetSERVSTOPDet("**", "**", "**", string.Empty, string.Empty);
                }
            }
            if (SERVSTOPList.Count > 0)
            {
                SERVStopEntity = SERVSTOPList[0];
                //SERVSTOPList.Find(u => Convert.ToDateTime(u.TDate.Trim()) >= Convert.ToDateTime(DateTime.Now.ToShortDateString()) && Convert.ToDateTime(u.FDate.Trim()) <= Convert.ToDateTime(DateTime.Now.ToShortDateString()));
                //if (Entity != null) IsServiceStop = true;
            }

            this.Text = privileges.PrivilegeName + " - " + Mode;
            if (Mode.Equals("Add"))
            {
                Set_Controls_TO_Add_Mode();
                Btn_Bulk_Posting.Visible = Tools["tlSave"].Visible = false;
                Auto_Post_Panel.Visible = true;
                this.Auto_Post_Panel.Location = new System.Drawing.Point(1, -1);
            }
            else
            {
                //Start_Date.Enabled = false;
                Auto_Post_Panel.Visible = true;
                Fill_SP_Controls(SP_Code);
                Tools["tlSave"].Visible = true;
            }

            GetAgencyDetails();
            if (dtAgency.Rows.Count > 0)
            {
                if (dtAgency.Rows[0]["ACR_CA_PVOUCHER"].ToString().Trim() == "Y")
                    Tools["tlVoucher"].Visible = true;
            }

            this.SP2_Panel.Dock = DockStyle.Fill;

            //Link_To_SP.Location = new System.Drawing.Point(539, 7);
            //Link_To_Add.Location = new System.Drawing.Point(539, 7);


            //this.SP2_Panel.Location = new System.Drawing.Point(2, -1);
            //this.SP2_Panel.Size = new System.Drawing.Size(761, 244);

            ////this.PbDelete.Location = new System.Drawing.Point(744, 49);
            //this.SP_CAMS_Grid.Size = new System.Drawing.Size(739, 213);

            //this.panel5.Size = new System.Drawing.Size(765, 24);

            //this.Pb_SP2_Max.Location = new System.Drawing.Point(744, 0);
            ////this.SP2_Desc.Width = 526;// 540;
            //this.SP2_Desc.Width = 446;// 540;
            //this.gvMsResult.Width = 80;
            //this.SP2_Comp_Date.Width = 73;//66;
            //this.SP2_Follow_Date.Width = 73;//66;

            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(Pb_SPM2_Max, "Maximize Addtional CAMS Panel");
            tooltip.SetToolTip(Pb_SPM2_Min, "Minimize Addtional CAMS Panel");
            tooltip.SetToolTip(Pb_SP2_Max, "Maximize Service Plan CAMS Panel");
            tooltip.SetToolTip(Pb_SP2_Min, "Minimize Service Plan CAMS Panel");
            //tooltip.SetToolTip(Hepl, "Help");


            tooltip.SetToolTip(Pb_Add_CA, "Post Selected CA/MS");
            tooltip.SetToolTip(Pb_Edit, "Edit CA/MS Posting");
            tooltip.SetToolTip(PbDelete, "Delete CA/MS Posting");
            tooltip.SetToolTip(PB_SP2_Notes, "Add Progress Notes");
            //tooltip.SetToolTip(pictureBox3, "Service Plan");
            //tooltip.SetToolTip(PbVoucher, "Voucher");

            List<PrivilegeEntity> userPrivilege = _model.UserProfileAccess.GetScreensByUserID(BaseForm.BusinessModuleID, BaseForm.UserID, BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg);
            CaseSerPrivileges = Privileges;
            CaseSerPrivileges = userPrivilege.Find(u => u.Program == "CASE9006");

            if (CaseSerPrivileges != null)
            {
                if (CaseSerPrivileges.ViewPriv.Equals("true"))
                {
                    if (Mode.Equals("Edit"))
                        Btn_Bulk_Posting.Visible = true;
                }
                else
                {
                    Btn_Bulk_Posting.Visible = false;
                }
            }


        }

        bool Start_Date_Enable_SW = false, Dep_Year_Mismatch = false;
        List<FldcntlHieEntity> SMP_Cntl_List = new List<FldcntlHieEntity>();
        private void Enable_SPM_Controls_On_FLDCNTL()
        {
            SMP_Cntl_List = _model.FieldControls.GetFLDCNTLHIE("CASE0062", BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg, "FLDCNTL");
            if (!SMP_Cntl_List.Exists(u => u.Enab.Equals("Y")) && Mode == "Add")
                Btn_Save.Enabled = false;

            bool required = false, enabled = false;
            foreach (FldcntlHieEntity entity in SMP_Cntl_List)
            {
                required = entity.Req.Equals("Y") ? true : false;
                enabled = entity.Enab.Equals("Y") ? true : false;

                switch (entity.FldCode)
                {
                    case Consts.CASE0006.SMP_SP:
                        if (enabled) { CmbSP.Enabled = Lbl_SP.Enabled = true; if (required) Lbl_SP_Req.Visible = true; } else { CmbSP.Enabled = Lbl_SP.Enabled = false; Lbl_SP_Req.Visible = false; }
                        break;
                    case Consts.CASE0006.SMP_Site:
                        if (enabled) { Lbl_Site.Enabled = CmbSite.Enabled = true; if (required) Lbl_Site_Req.Visible = true; } else { Lbl_Site.Enabled = Lbl_Site_Req.Enabled = false; CmbSite.Visible = false; }
                        break;
                    case Consts.CASE0006.SMP_CaseWorker:
                        if (enabled) { Lbl_CaseWorker.Enabled = CmbWorker.Enabled = true; if (required) Lbl_CaseWorker_Req.Visible = true; } else { CmbWorker.Enabled = Lbl_CaseWorker.Enabled = false; Lbl_CaseWorker_Req.Visible = false; }
                        break;

                    case Consts.CASE0006.SMP_Act_Prog:
                        if (enabled) { Pb_SPM_Prog.Visible = Lbl_Program.Enabled = true; if (required) Lbl_Program_Req.Visible = true; } else { Pb_SPM_Prog.Visible = Lbl_Program.Enabled = false; Lbl_Program_Req.Visible = false; }
                        break;
                    case Consts.CASE0006.SMP_Start_Date:
                        Start_Date_Enable_SW = enabled;
                        if (enabled) { Start_Date.Enabled = Lbl_StartDate.Enabled = true; if (required) Lbl_StartDate_Req.Visible = true; } else { Start_Date.Enabled = Lbl_StartDate.Enabled = false; Lbl_StartDate_Req.Visible = false; }
                        break;
                    case Consts.CASE0006.SMP_Est_Complete_Date:
                        if (enabled) { Est_Date.Enabled = Lbl_Est_CompleteDate.Enabled = true; if (required) Lbl_Est_CompleteDate_Req.Visible = true; } else { Est_Date.Enabled = Lbl_Est_CompleteDate.Enabled = false; Lbl_Est_CompleteDate_Req.Visible = false; }
                        break;
                    case Consts.CASE0006.SMP_Actual_Complete_Date:
                        if (enabled) { Act_Date.Enabled = Lbl_Actual_CompleteDate.Enabled = true; if (required) Lbl_Actual_CompleteDate_Req.Visible = true; } else { Act_Date.Enabled = Lbl_Actual_CompleteDate.Enabled = false; Lbl_Actual_CompleteDate_Req.Visible = false; }
                        break;
                    case Consts.CASE0006.SMP_Sel_Branches:
                        if (enabled) { Branches_Grid.Enabled = true; if (required) this.SP_Branches.HeaderText = "Selected Branches of Service *"; } else { Branches_Grid.Enabled = false; }
                        break;
                }
            }

        }
        List<SPCommonEntity> propResultsList = new List<SPCommonEntity>();

        public string MsResultDescription(string strid)
        {
            string strresultdesc = string.Empty;
            if (propResultsList.Count > 0)
            {
                try
                {
                    strresultdesc = propResultsList.Find(u => u.Code.Trim() == strid.Trim()).Desc;
                }
                catch (Exception ex)
                {

                }

            }
            return strresultdesc;
        }
        private void Switch_To_Edit_Mode()
        {
            Mode = "Edit";
            Auto_Post_Panel.Visible = false;
            this.Size = new System.Drawing.Size(840, this.Size.Height + MainPanel2.Height);
            //this.Size = new System.Drawing.Size(767, 496); //767, 496
            //this.Size = new System.Drawing.Size(758, 475); //767, 496
            // DatePanel.Location = new System.Drawing.Point(116, 89); //(116, 74);
            MainPanel2.Visible = true;
            //Start_Date.Enabled = false;

            //this.SP2_Desc.Width = 526;// 540;
            //this.SP2_Desc.Width = 446;// 540;
            //this.gvMsResult.Width = 80;
            //this.SP2_Comp_Date.Width = 73;//66;
            //this.SP2_Follow_Date.Width = 73;//66;

           // this.Hepl.Location = new System.Drawing.Point(740, 20);
            //this.panel2.Location = new System.Drawing.Point(1, 464);

            //this.Branches_Grid.Location = new System.Drawing.Point(377, 27);
            //this.Save_Cancel_Panel.Location = new System.Drawing.Point(589, 0);

            //this.CmbSP.Size = new System.Drawing.Size(482, 21);
            //this.CmbSite.Size = this.CmbWorker.Size = new System.Drawing.Size(283, 21);

            //Txt_SPM_Program.Size = new System.Drawing.Size(263, 19);
            //this.panel8.Location = new System.Drawing.Point(337, 69);

            //this.MainPanel.Size = new System.Drawing.Size(765, 164);
            //this.panel2.Size = new System.Drawing.Size(765, 30);

            CA_Auto_Details_Filled = false; MS_Auto_Details_Filled = false;



            Fill_Applicant_SPs();
            Fill_SP_DropDowns();
            Fill_SP_Controls((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString()));
            Search_Entity.service_plan = (((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString());
            Branches_Grid_SelectionChanged(Branches_Grid, EventArgs.Empty);
            Btn_Bulk_Posting.Visible = Tools["tlSave"].Visible = true;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Update();
        }

        DataTable Dt_Programs = new DataTable();
        private void Fill_Def_Program_Combo()
        {
            //DataSet ds = Captain.DatabaseLayer.MainMenu.GetGlobalHierarchies_Latest(BaseForm.UserID, "3", BaseForm.BaseAgency, BaseForm.BaseDept, " "); // Verify it Once

            //if (ds.Tables.Count > 0)
            //{
            //    if (ds.Tables[0].Rows.Count > 0)
            //    {
            //        DataTable Dt_Programs = ds.Tables[0];

            //        List<Captain.Common.Utilities.ListItem> listItem = new List<Captain.Common.Utilities.ListItem>();
            //        listItem.Add(new Captain.Common.Utilities.ListItem("   ", "0"));
            //        int TmpRows = 0; string Tmp_Hierarchy = "";
            //        int ProgIndex = 0;
            //        try
            //        {
            //            foreach (DataRow dr in Dt_Programs.Rows)
            //            {
            //                //Tmp_Hierarchy = dr["Agy"].ToString() + dr["Dept"].ToString() + dr["Prog"].ToString();
            //                if (Dt_Programs.Columns.Count >= 4)
            //                    Tmp_Hierarchy = dr["Agy"].ToString() + dr["Dept"].ToString() + dr["Prog"].ToString();
            //                else
            //                    Tmp_Hierarchy = BaseForm.BaseAgency + BaseForm.BaseDept + dr["Prog"].ToString();
            //                listItem.Add(new Captain.Common.Utilities.ListItem(dr["Prog"] + " - " + dr["Name"], Tmp_Hierarchy));
            //                TmpRows++;
            //            }

            //            if (TmpRows > 0)
            //            {
            //                Cmb_Def_Prog.Items.AddRange(listItem.ToArray());
            //                //Cmb_Def_Prog.Enabled = true;
            //                //if (Cmb_Def_Prog.Items.Count > 1)
            //                //    SetComboBoxValue(Cmb_Def_Prog, BaseForm.BaseAgency+BaseForm.BaseDept+BaseForm.BaseProg);
            //                //else
            //                    Cmb_Def_Prog.SelectedIndex = 0;
            //            }
            //        }
            //        catch (Exception ex) { }
            //    }
            //    else
            //        MessageBox.Show("Programs Are Not Defined", "CAP Systems");
            //}
        }

        #region properties

        public BaseForm BaseForm { get; set; }

        public string Mode { get; set; }

        public string SP_Code { get; set; }

        public PrivilegeEntity CaseSerPrivileges { get; set; }

        public SERVSTOPEntity SERVStopEntity { get; set; }

        public List<FldcntlHieEntity> CntlCAEntity { get; set; }

        public List<FldcntlHieEntity> CntlMSEntity { get; set; }
        List<CASESP0Entity> propSearch_Entity { get; set; }
        public string propReportPath { get; set; }
        //public string M_Hierarchy { get; set; }

        ////public string M_HieDesc { get; set; }

        //public string M_Year { get; set; }

        public string SchSite { get; set; }

        public string SchDate { get; set; }

        public string SchType { get; set; }

        public string Hierarchy { get; set; }

        public string Year { get; set; }

        public string App_No { get; set; }

        public string Sp_Sequence { get; set; }

        public string Spm_Year { get; set; }

        public PrivilegeEntity Privileges { get; set; }

        public DataGridViewContentAlignment Alignment { get; set; }

        public DataSet dsAgency { get; set; }

        public DataTable dtAgency { get; set; }

        #endregion

        string Sql_SP_Result_Message = string.Empty;

        string Img_Saved = "icon-save";
        string Img_Blank = "blank";
        string Img_Tick = "icon-gridtick";
        string Img_Add = "captain-add";
        string Img_Edit = "captain-edit";

        private void Hepl_Click(object sender, EventArgs e)
        {
            // Help.ShowHelp(this, Context.Server.MapPath("~\\Resources\\HelpFiles\\Captain_Help.chm"), HelpNavigator.KeywordIndex, "CASE0006_Add");
        }

        private void Set_Controls_TO_Add_Mode()
        {
            Btn_Maintain_Add.Visible = MainPanel2.Visible = false;
            CmbSP.Width = CmbSP.Width + 200;

            //// this.Hepl.Location = new System.Drawing.Point(700, 20);
            //this.panel2.Location = new System.Drawing.Point(1, 219);

            //this.DatePanel.Location = new System.Drawing.Point(70, 89); //(65, 74);
            //this.Branches_Grid.Location = new System.Drawing.Point(329, 27);
            //this.Save_Cancel_Panel.Location = new System.Drawing.Point(545, 0);

            //this.CmbSP.Size = new System.Drawing.Size(628, 21);
            //this.CmbSite.Size = this.CmbWorker.Size = new System.Drawing.Size(232, 21);

            //Txt_SPM_Program.Size = new System.Drawing.Size(215, 21);
            //this.panel8.Location = new System.Drawing.Point(287, 69);
            //this.MainPanel.Size = new System.Drawing.Size(722, 164);
            //this.panel2.Size = new System.Drawing.Size(722, 30);
            //this.Size = new System.Drawing.Size(726, 250);
            ////this.Program_Panel.Location = new System.Drawing.Point(1, 464);
            ////Program_Panel.Visible = true;

            this.Size = new System.Drawing.Size(840, this.Size.Height - MainPanel2.Height);
        }

        string Tmp_SPM_Sequence = "1";
        private void Fill_Applicant_SPs()
        {
            Search_Entity.agency = Hierarchy.Substring(0, 2);
            Search_Entity.dept = Hierarchy.Substring(2, 2);
            Search_Entity.program = Hierarchy.Substring(4, 2);
            //Search_Entity.year = BaseYear;
            Search_Entity.year = null;                          // Year will be always Four-Spaces in CASESPM
            Search_Entity.app_no = App_No;

            Search_Entity.service_plan = Search_Entity.caseworker = Search_Entity.site = null;
            Search_Entity.startdate = Search_Entity.estdate = Search_Entity.compdate = null;
            Search_Entity.sel_branches = Search_Entity.have_addlbr = Search_Entity.date_lstc = null;
            Search_Entity.lstc_operator = Search_Entity.date_add = Search_Entity.add_operator = null;
            Search_Entity.Sp0_Desc = Search_Entity.Sp0_Validatetd = Search_Entity.Def_Program = Search_Entity.SPM_MassClose =
            Search_Entity.Seq = Search_Entity.Bulk_Post = null;

            CASESPM_SP_List = _model.SPAdminData.Browse_CASESPM(Search_Entity, "Browse");
        }


        private void Pb_SPM2_Max_Click(object sender, EventArgs e)
        {
            SP2_Panel.Visible = Pb_SPM2_Max.Visible = false;
            //this.MainPanel2.Location = new System.Drawing.Point(1, 84);
            //this.MainPanel2.Size = new System.Drawing.Size(765, 380);
            //this.panel4.Size = new System.Drawing.Size(765, 22);

            //this.Pb_SPM2_Delete.Location = new System.Drawing.Point(750, 48);

            //this.SPM2_Panel.Size = new System.Drawing.Size(722, 383);      // Don't Delete
            //this.ADD_CAMS_Grid.Size = new System.Drawing.Size(715, 375);

            // this.SPM2_Desc.Width = 530;
            //Pb_SPM2_Min.Visible = true;
        }

        private void Pb_SP2_Max_Click(object sender, EventArgs e)
        {
            SPM2_Panel.Visible = Pb_SP2_Max.Visible = false;
            MainPanel.Visible = false;
            //this.MainPanel2.Location = new System.Drawing.Point(1, 82);
            //this.MainPanel2.Size = new System.Drawing.Size(765, 383);
            ////this.panel5.Size = new System.Drawing.Size(765, 22);

            ////this.PbDelete.Location = new System.Drawing.Point(750, 49);

            //this.SP2_Panel.Location = new System.Drawing.Point(0, -1);
            //this.SP2_Panel.Size = new System.Drawing.Size(765, 383);
            //this.SP_CAMS_Grid.Size = new System.Drawing.Size(739, 350);

            ////this.panel5.Size = new System.Drawing.Size(761, 22);
            //this.Pb_SP2_Min.Location = new System.Drawing.Point(744, 0);

            //this.SP2_Desc.Width = 526;
            Pb_SP2_Min.Visible = true;
        }

        private void Pb_SP2_Min_Click(object sender, EventArgs e)
        {
            
            Pb_SP2_Min.Visible = false;
            MainPanel.Visible = true;

            //this.PbDelete.Location = new System.Drawing.Point(369, 49);

            ////this.panel5.Size = new System.Drawing.Size(386, 22);
            //this.MainPanel2.Location = new System.Drawing.Point(1, 219); //2, 219
            //this.MainPanel2.Size = new System.Drawing.Size(765, 246);

            ////this.SP2_Panel.Location = new System.Drawing.Point(377, -1);   // Don't Delete 
            ////this.SP2_Panel.Size = new System.Drawing.Size(387, 246);

            ////this.SP2_Panel.Location = new System.Drawing.Point(2, -1);
            ////this.SP2_Panel.Size = new System.Drawing.Size(761, 244);


            ////this.SPM2_Panel.Size = new System.Drawing.Size(378, 246);
            ////this.SP2_Desc.Width = 150;
            ////this.SP_CAMS_Grid.Size = new System.Drawing.Size(366, 213);

            //this.SP2_Panel.Location = new System.Drawing.Point(2, -1); //ooooooooooooooo
            //this.SP2_Panel.Size = new System.Drawing.Size(761, 244);

            ////this.PbDelete.Location = new System.Drawing.Point(725, 49);
            //this.SP_CAMS_Grid.Size = new System.Drawing.Size(739, 213);
            ////this.SP2_Desc.Width = 526; // 540
            //this.SP2_Desc.Width = 446;// 540;
            //this.gvMsResult.Width = 80;

            //this.panel5.Size = new System.Drawing.Size(765, 24);
            //this.Pb_SP2_Max.Location = new System.Drawing.Point(744, 0);

            Pb_SP2_Max.Visible = true;
            this.Size = new System.Drawing.Size(840, this.Size.Height);
            // this.Size = new System.Drawing.Size(756, 476);
            //this.Size = new System.Drawing.Size(767, 496); //767, 496

            //SPM2_Panel.Visible = true;
        }

        private void Pb_SPM2_Min_Click(object sender, EventArgs e)
        {
            Pb_SPM2_Min.Visible = false;
            //this.Pb_SPM2_Delete.Location = new System.Drawing.Point(363, 48);

            //this.panel4.Size = new System.Drawing.Size(374, 26);
            //this.MainPanel2.Location = new System.Drawing.Point(1, 219);
            //this.MainPanel2.Size = new System.Drawing.Size(764, 246);

            //this.SPM2_Desc.Width = 150;
            //this.ADD_CAMS_Grid.Size = new System.Drawing.Size(366, 213);
            SP2_Panel.Visible = Pb_SPM2_Max.Visible = true;
        }

        CASESP0Entity SP_Header_Rec;
        List<CASESPMEntity> CASESPM_SP_List;
        CASESPMEntity Search_Entity = new CASESPMEntity();
        private void Fill_SP_Controls(string Code)
        {
            this.CmbSP.SelectedIndexChanged -= new System.EventHandler(this.CmbSP_SelectedIndexChanged);

            List<CASESPMEntity> CASESPM_List = new List<CASESPMEntity>();

            Search_Entity.agency = Hierarchy.Substring(0, 2);
            Search_Entity.dept = Hierarchy.Substring(2, 2);
            Search_Entity.program = Hierarchy.Substring(4, 2);
            //Search_Entity.year = null;                                // Year will be always Four-Spaces in CASESPM
            Search_Entity.year = Spm_Year;
            Search_Entity.app_no = App_No;

            Search_Entity.service_plan = Search_Entity.caseworker = Search_Entity.site =
            Search_Entity.startdate = Search_Entity.estdate = Search_Entity.compdate =
            Search_Entity.sel_branches = Search_Entity.have_addlbr = Search_Entity.date_lstc =
            Search_Entity.lstc_operator = Search_Entity.date_add = Search_Entity.add_operator = Search_Entity.SPM_MassClose =
            Search_Entity.Sp0_Desc = Search_Entity.Sp0_Validatetd = Search_Entity.Def_Program = Search_Entity.Bulk_Post = null;
            Search_Entity.Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013

            //Search_Entity.App_Not_EqualTo = null;

            Search_Entity.service_plan = Code;

            CASESPM_List = _model.SPAdminData.Browse_CASESPM(Search_Entity, "Browse");

            if (CASESPM_List.Count > 0)
            {
                Search_Entity = CASESPM_List[0];

                SetComboBoxValue(CmbSite, Search_Entity.site.Trim());
                SetComboBoxValue(CmbWorker, Search_Entity.caseworker.Trim());
                //SetComboBoxValue(CmbSP, Search_Entity.service_plan);
                //SetComboBoxValue(Cmb_Def_Prog, Search_Entity.Def_Program);
                Txt_SPM_Program.Text = Set_SP_Program_Text(Search_Entity.Def_Program);

                Start_Date.Value = Act_Date.Value = Est_Date.Value = DateTime.Today;
                Start_Date.Checked = Act_Date.Checked = Est_Date.Checked = false;

                if (!string.IsNullOrEmpty(Search_Entity.startdate))
                {
                    Start_Date.Value = Convert.ToDateTime(Search_Entity.startdate);
                    Start_Date.Checked = true;
                }
                if (!string.IsNullOrEmpty(Search_Entity.compdate))
                {
                    Act_Date.Value = Convert.ToDateTime(Search_Entity.compdate);
                    Act_Date.Checked = true;
                }
                if (!string.IsNullOrEmpty(Search_Entity.estdate))
                {
                    Est_Date.Value = Convert.ToDateTime(Search_Entity.estdate);
                    Est_Date.Checked = true;
                }

                SP_CAMS_Details = _model.SPAdminData.Browse_CASESP2(Code, null, null, null, "CASE4006");

                Get_App_CASEACT_List();
                Get_App_CASEMS_List();

                if (SP_Activity_Details.Count == 0 && SP_MS_Details.Count == 0)
                    Start_Date.Enabled = Start_Date_Enable_SW;
                else
                    Start_Date.Enabled = false;



                Fill_Branch_Grid(Search_Entity.service_plan);
                Fill_SP_CAMS_Details(Search_Entity.service_plan, "P", null);
                // buttonAllowAMDSwitch();
                //Fill_Additional_CAMS_Details(Search_Entity.service_plan);
            }
            this.CmbSP.SelectedIndexChanged += new System.EventHandler(this.CmbSP_SelectedIndexChanged);
        }


        private void Fill_Branch_Grid(string Code)
        {
            SP_Header_Rec = _model.SPAdminData.Browse_CASESP0(Code, null, null, null, null, null, null, null, null);

            this.Branches_Grid.SelectionChanged -= new System.EventHandler(this.Branches_Grid_SelectionChanged);

            Branches_Grid.Rows.Clear();

            Btn_AutoPost_CA.Visible = Btn_AutoPost_MS.Visible = Link_To_Add.Visible = Btn_Maintain_Add.Visible = false;

            //if (SP_Header_Rec.Allow_Add_Branch == "Y" && Mode.Equals("Edit"))
            //    Link_To_Add.Visible = Btn_Maintain_Add.Visible = true;

            if (string.IsNullOrEmpty(Search_Entity.sel_branches))
                Search_Entity.sel_branches = string.Empty;

            int Branch_Count = 0;

            if (SP_Header_Rec.Auto_Post_CA == "Y" && SP_Header_Rec.Auto_Post_SP == "Y")
            {
                Btn_AutoPost_CA.Visible = true;
            }

            if (SP_Header_Rec.Auto_Post_MS == "Y" && SP_Header_Rec.Auto_Post_SP == "Y")
            {
                if (SP_Header_Rec.Auto_Post_CA != "Y")
                    this.Btn_AutoPost_MS.Location = new System.Drawing.Point(3, 1);
                else
                    this.Btn_AutoPost_MS.Location = new System.Drawing.Point(81, 1);

                Btn_AutoPost_MS.Visible = true;
            }


            if (!string.IsNullOrEmpty(SP_Header_Rec.BpCode.Trim()))  // Yeswanth
            {
                //if (Search_Entity.sel_branches.Contains(SP_Header_Rec.BpCode.Trim()))
                Branches_Grid.Rows.Add(true, SP_Header_Rec.BpDesc.Trim(), SP_Header_Rec.BpCode.Trim(), "Y");
                Branches_Grid.Rows[0].Cells["Branch_Sel"].ReadOnly = true;
                Branches_Grid.Rows[0].Cells["SP_Branches"].ReadOnly = true;
                Branches_Grid.Rows[0].DefaultCellStyle.ForeColor = Color.Blue;

                //else
                //    Branches_Grid.Rows.Add(false, SP_Header_Rec.BpDesc.Trim(), SP_Header_Rec.BpCode.Trim());
            }


            if (!string.IsNullOrEmpty(SP_Header_Rec.B1Code.Trim()))
            {
                Branch_Count++;
                if (Search_Entity.sel_branches.Contains(SP_Header_Rec.B1Code.Trim()))
                    Branches_Grid.Rows.Add(true, SP_Header_Rec.B1Desc.Trim(), SP_Header_Rec.B1Code.Trim(), "Y");
                else
                {
                    Branches_Grid.Rows.Add(false, SP_Header_Rec.B1Desc.Trim(), SP_Header_Rec.B1Code.Trim(), "N");
                    Branches_Grid.Rows[Branch_Count].DefaultCellStyle.ForeColor = Color.MediumVioletRed; //Color.Peru; //Color.DarkTurquoise;
                }
            }

            if (!string.IsNullOrEmpty(SP_Header_Rec.B2Code.Trim()))
            {
                Branch_Count++;
                if (Search_Entity.sel_branches.Contains(SP_Header_Rec.B2Code.Trim()))
                    Branches_Grid.Rows.Add(true, SP_Header_Rec.B2Desc.Trim(), SP_Header_Rec.B2Code.Trim(), "Y");
                else
                {
                    Branches_Grid.Rows.Add(false, SP_Header_Rec.B2Desc.Trim(), SP_Header_Rec.B2Code.Trim(), "N");
                    Branches_Grid.Rows[Branch_Count].DefaultCellStyle.ForeColor = Color.MediumVioletRed; //Color.Peru; //Color.DarkTurquoise;
                }
            }

            if (!string.IsNullOrEmpty(SP_Header_Rec.B3Code.Trim()))
            {
                Branch_Count++;
                if (Search_Entity.sel_branches.Contains(SP_Header_Rec.B3Code.Trim()))
                    Branches_Grid.Rows.Add(true, SP_Header_Rec.B3Desc.Trim(), SP_Header_Rec.B3Code.Trim(), "Y");
                else
                {
                    Branches_Grid.Rows.Add(false, SP_Header_Rec.B3Desc.Trim(), SP_Header_Rec.B3Code.Trim(), "N");
                    Branches_Grid.Rows[Branch_Count].DefaultCellStyle.ForeColor = Color.MediumVioletRed; //Color.Peru; //Color.DarkTurquoise;
                }
            }

            if (!string.IsNullOrEmpty(SP_Header_Rec.B4Code.Trim()))
            {
                Branch_Count++;
                if (Search_Entity.sel_branches.Contains(SP_Header_Rec.B4Code.Trim()))
                    Branches_Grid.Rows.Add(true, SP_Header_Rec.B4Desc.Trim(), SP_Header_Rec.B4Code.Trim(), "Y");
                else
                {
                    Branches_Grid.Rows.Add(false, SP_Header_Rec.B4Desc.Trim(), SP_Header_Rec.B4Code.Trim(), "N");
                    Branches_Grid.Rows[Branch_Count].DefaultCellStyle.ForeColor = Color.MediumVioletRed; //Color.Peru; //Color.DarkTurquoise;
                }
            }

            if (!string.IsNullOrEmpty(SP_Header_Rec.B5Code.Trim()))
            {
                Branch_Count++;
                if (Search_Entity.sel_branches.Contains(SP_Header_Rec.B5Code.Trim()))
                    Branches_Grid.Rows.Add(true, SP_Header_Rec.B5Desc.Trim(), SP_Header_Rec.B5Code.Trim(), "Y");
                else
                {
                    Branches_Grid.Rows.Add(false, SP_Header_Rec.B5Desc.Trim(), SP_Header_Rec.B5Code.Trim(), "N");
                    Branches_Grid.Rows[Branch_Count].DefaultCellStyle.ForeColor = Color.MediumVioletRed; //Color.Peru; //Color.DarkTurquoise;
                }
            }

            if (SP_Header_Rec.Allow_Add_Branch == "Y" && Mode.Equals("Edit"))
            {
                //Link_To_Add.Visible = Btn_Maintain_Add.Visible = true;
                if (Spm_Year == BaseForm.BaseYear && Privileges.AddPriv.Equals("true"))
                    Btn_Maintain_Add.Visible = true;

                {
                    Fill_Additional_CAMS_Details(SP_Code);
                    if (ADD_CAMA_Details.Count > 0)
                    {
                        Branch_Count++;
                        //    if (Search_Entity.sel_branches.Contains(SP_Header_Rec.B5Code.Trim()))

                        Branches_Grid.Rows.Add(true, "Additional Branch", "9", "Y");
                        Branches_Grid.Rows[Branch_Count].Cells["Branch_Sel"].ReadOnly = true;
                        Branches_Grid.Rows[Branch_Count].Cells["SP_Branches"].ReadOnly = true;
                        //Branches_Grid.Rows[Branch_Count].DefaultCellStyle.ForeColor = Color.Blue; 
                    }

                    //else
                    //{
                    //    Branches_Grid.Rows.Add(false, SP_Header_Rec.B5Desc.Trim(), SP_Header_Rec.B5Code.Trim(), "N");
                    //    Branches_Grid.Rows[Branch_Count].DefaultCellStyle.ForeColor = Color.MediumVioletRed; //Color.Peru; //Color.DarkTurquoise;
                    //}
                }
            }
            //buttonAllowAMDSwitch();
            if (Branches_Grid.Rows.Count > 0)
                Branches_Grid.Rows[0].Selected = true;

            this.Branches_Grid.SelectionChanged += new System.EventHandler(this.Branches_Grid_SelectionChanged);
        }


        private void Get_App_CASEACT_List()
        {
            // Can Use Search Entity here Rao Can-Delete

            //CA_Pass_Entity.Agency = Hierarchy.Substring(0, 2);
            //CA_Pass_Entity.Dept = Hierarchy.Substring(2, 2);
            //CA_Pass_Entity.Program = Hierarchy.Substring(4, 2);

            ////CA_Pass_Entity.Year = BaseYear;                        
            //CA_Pass_Entity.Year = Spm_Year;                             // Year will be always Four-Spaces in CASEACT
            //CA_Pass_Entity.App_no = App_No;
            //CA_Pass_Entity.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013

            //CA_Pass_Entity.Service_plan = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString()));
            //CA_Pass_Entity.Branch = CA_Pass_Entity.Group  = CA_Pass_Entity.ACT_Code = null;
            //CA_Pass_Entity.ACT_Date = CA_Pass_Entity.ACT_Seq = CA_Pass_Entity.Site = CA_Pass_Entity.Fund1 = null;
            //CA_Pass_Entity.Fund2 = CA_Pass_Entity.Fund3 = CA_Pass_Entity.Caseworker = CA_Pass_Entity.Vendor_No = null;
            //CA_Pass_Entity.Check_Date = CA_Pass_Entity.Check_No = CA_Pass_Entity.Cost = CA_Pass_Entity.Followup_On = null;
            //CA_Pass_Entity.Followup_Comp = CA_Pass_Entity.Followup_By = CA_Pass_Entity.Refer_Data = CA_Pass_Entity.Cust_Code1 = null;
            //CA_Pass_Entity.Cust_Value1 = CA_Pass_Entity.Cust_Code2 = CA_Pass_Entity.Cust_Value2 = CA_Pass_Entity.Cust_Code3 = null;
            //CA_Pass_Entity.Cust_Value3 = CA_Pass_Entity.Lstc_Date = CA_Pass_Entity.Lsct_Operator = CA_Pass_Entity.Add_Date = null;
            //CA_Pass_Entity.Add_Operator = CA_Pass_Entity.ACT_ID = CA_Pass_Entity.Bulk = CA_Pass_Entity.Act_PROG = null;
            //SP_Activity_Details = _model.SPAdminData.Browse_CASEACT(CA_Pass_Entity, "Browse");

            CASEACTEntity Search_Enty = new CASEACTEntity(true);
            Search_Enty.Agency = Hierarchy.Substring(0, 2);
            Search_Enty.Dept = Hierarchy.Substring(2, 2);
            Search_Enty.Program = Hierarchy.Substring(4, 2);
            Search_Enty.Year = Spm_Year;                             // Year will be always Four-Spaces in CASEACT
            Search_Enty.App_no = App_No;
            Search_Enty.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013
            Search_Enty.Service_plan = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString()));

            SP_Activity_Details = _model.SPAdminData.Browse_CASEACT(Search_Enty, "Browse");

            SP_Activity_Details = SP_Activity_Details.OrderByDescending(u => Convert.ToDateTime(u.ACT_Date.Trim())).ToList();

        }

        private void Get_App_Additional_CASEACT_List()
        {
            // Can Use Search Entity here Rao Can-Delete
            //CA_Pass_Entity.Agency = Hierarchy.Substring(0, 2);
            //CA_Pass_Entity.Dept = Hierarchy.Substring(2, 2);
            //CA_Pass_Entity.Program = Hierarchy.Substring(4, 2);

            ////CA_Pass_Entity.Year = BaseYear;                        
            //CA_Pass_Entity.Year = Spm_Year;                             // Year will be always Four-Spaces in CASEACT
            //CA_Pass_Entity.App_no = App_No;
            //CA_Pass_Entity.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));

            //CA_Pass_Entity.Branch = "9";
            //CA_Pass_Entity.Service_plan = CA_Pass_Entity.Group = CA_Pass_Entity.ACT_Code = null;
            //CA_Pass_Entity.ACT_Date = CA_Pass_Entity.ACT_Seq = CA_Pass_Entity.Site = CA_Pass_Entity.Fund1 = null;
            //CA_Pass_Entity.Fund2 = CA_Pass_Entity.Fund3 = CA_Pass_Entity.Caseworker = CA_Pass_Entity.Vendor_No = null;
            //CA_Pass_Entity.Check_Date = CA_Pass_Entity.Check_No = CA_Pass_Entity.Cost = CA_Pass_Entity.Followup_On = null;
            //CA_Pass_Entity.Followup_Comp = CA_Pass_Entity.Followup_By = CA_Pass_Entity.Refer_Data = CA_Pass_Entity.Cust_Code1 = null;
            //CA_Pass_Entity.Cust_Value1 = CA_Pass_Entity.Cust_Code2 = CA_Pass_Entity.Cust_Value2 = CA_Pass_Entity.Cust_Code3 = null;
            //CA_Pass_Entity.Cust_Value3 = CA_Pass_Entity.Lstc_Date = CA_Pass_Entity.Lsct_Operator = CA_Pass_Entity.Add_Date = null;
            //CA_Pass_Entity.Add_Operator = CA_Pass_Entity.ACT_ID = CA_Pass_Entity.Bulk = CA_Pass_Entity.Act_PROG = null;
            //SP_Additional_Activity_Details = _model.SPAdminData.Browse_CASEACT(CA_Pass_Entity, "Browse");

            CASEACTEntity Search_Enty = new CASEACTEntity(true);

            Search_Enty.Agency = Hierarchy.Substring(0, 2);
            Search_Enty.Dept = Hierarchy.Substring(2, 2);
            Search_Enty.Program = Hierarchy.Substring(4, 2);
            Search_Enty.Year = Spm_Year;                             // Year will be always Four-Spaces in CASEACT
            Search_Enty.App_no = App_No;
            Search_Enty.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));
            Search_Enty.Branch = "9";

            SP_Additional_Activity_Details = _model.SPAdminData.Browse_CASEACT(Search_Enty, "Browse");
        }


        private void Get_App_CASEMS_List()
        {
            // Can Use Search Entity here Rao Can-Delete
            //Search_MS_Details.Agency = Hierarchy.Substring(0, 2);
            //Search_MS_Details.Dept = Hierarchy.Substring(2, 2);
            //Search_MS_Details.Program = Hierarchy.Substring(4, 2);
            ////Search_MS_Details.Year = BaseYear; 
            //Search_MS_Details.Year = Spm_Year;                              // Year will be always Four-Spaces in CASEMS
            //Search_MS_Details.App_no = App_No;
            //Search_MS_Details.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));

            //Search_MS_Details.Service_plan = Search_MS_Details.Branch = Search_MS_Details.Group = Search_MS_Details.MS_Code = null;
            //Search_MS_Details.ID  = Search_MS_Details.Date = Search_MS_Details.CaseWorker = Search_MS_Details.Site = null;
            //Search_MS_Details.Result = Search_MS_Details.OBF = Search_MS_Details.Add_Operator = null;
            //Search_MS_Details.Lstc_Date = Search_MS_Details.Lsct_Operator = Search_MS_Details.Add_Date = Search_MS_Details.Bulk =
            //Search_MS_Details.Acty_PROG = null;
            //SP_MS_Details = _model.SPAdminData.Browse_CASEMS(Search_MS_Details, "Browse");

            CASEMSEntity Search_Enty = new CASEMSEntity(true);
            Search_Enty.Agency = Hierarchy.Substring(0, 2);
            Search_Enty.Dept = Hierarchy.Substring(2, 2);
            Search_Enty.Program = Hierarchy.Substring(4, 2);
            Search_Enty.Year = Spm_Year;                              // Year will be always Four-Spaces in CASEMS
            Search_Enty.App_no = App_No;
            Search_Enty.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));
            Search_Enty.Service_plan = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString()));

            SP_MS_Details = _model.SPAdminData.Browse_CASEMS(Search_Enty, "Browse");
            SP_MS_Details = SP_MS_Details.OrderByDescending(u => Convert.ToDateTime(u.Date.Trim())).ToList();
        }

        private void Get_App_Additional_CASEMS_List()
        {
            // Can Use Search Entity here Rao Can-Delete
            //Search_MS_Details.Agency = Hierarchy.Substring(0, 2);
            //Search_MS_Details.Dept = Hierarchy.Substring(2, 2);
            //Search_MS_Details.Program = Hierarchy.Substring(4, 2);
            ////Search_MS_Details.Year = BaseYear; 
            //Search_MS_Details.Year = Spm_Year;                              // Year will be always Four-Spaces in CASEMS
            //Search_MS_Details.App_no = App_No;
            //Search_MS_Details.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));

            //Search_MS_Details.Branch = "9";
            //Search_MS_Details.Service_plan = Search_MS_Details.Group = Search_MS_Details.MS_Code = null;
            //Search_MS_Details.ID = Search_MS_Details.Date = Search_MS_Details.CaseWorker = Search_MS_Details.Site = null;
            //Search_MS_Details.Result = Search_MS_Details.OBF = Search_MS_Details.Add_Operator = null;
            //Search_MS_Details.Lstc_Date = Search_MS_Details.Lsct_Operator = Search_MS_Details.Add_Date = Search_MS_Details.Bulk =
            //Search_MS_Details.Acty_PROG = null;

            CASEMSEntity Search_Enty = new CASEMSEntity(true);
            Search_Enty.Agency = Hierarchy.Substring(0, 2);
            Search_Enty.Dept = Hierarchy.Substring(2, 2);
            Search_Enty.Program = Hierarchy.Substring(4, 2);
            //Search_MS_Details.Year = BaseYear; 
            Search_Enty.Year = Spm_Year;                              // Year will be always Four-Spaces in CASEMS
            Search_Enty.App_no = App_No;
            Search_Enty.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));
            Search_Enty.Service_plan = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString()));
            Search_Enty.Branch = "9";

            SP_Additional_MS_Details = _model.SPAdminData.Browse_CASEMS(Search_Enty, "Browse");
        }


        int Pub_SP2_Cnt = 0, Pub_SP2_CAMS_Posting_Cnt = 0, Pub_SPM2_Cnt = 0, Pub_SPM2_CAMS_Posting_Cnt = 0;
        List<CASESP2Entity> SP_CAMS_Details = new List<CASESP2Entity>();
        List<CASEACTEntity> SP_Activity_Details = new List<CASEACTEntity>();
        List<CASEACTEntity> SP_Additional_Activity_Details = new List<CASEACTEntity>();
        CASEACTEntity Search_Activity_Details = new CASEACTEntity();
        List<CASEMSEntity> SP_MS_Details = new List<CASEMSEntity>();
        List<CASEMSEntity> SP_Additional_MS_Details = new List<CASEMSEntity>();
        CASEMSEntity Search_MS_Details = new CASEMSEntity();
        private void Fill_SP_CAMS_Details(string sp_Code, string Branch_Code, string Sel_CAMS_Key) //07032014
        {
            if (!Cb_Show_All_Postings.Checked)
            {
                Clear_SP_CAMS_Grid();
                Pub_SP2_Cnt = Pub_SP2_CAMS_Posting_Cnt = 0;
            }
            //SP_CAMS_Grid.Rows.Clear();

            string SP_Readonly_Sw = "N";
            CASESP0Entity casesp0data = propSearch_Entity.Find(u => u.Code == sp_Code);
            if (casesp0data != null)
            {
                if (casesp0data.Sp0ReadOnly == "Y") SP_Readonly_Sw = "Y";
            }

            if (SP_CAMS_Details.Count > 0)
            {
                int rowIndex = 0, Sel_CAMS_Index = 0;
                bool CASEACT_Exists = false, CASEMS_Exists = false, CA_Template_Set = false, MS_Template_Set = false; ;
                string Comp_date = null, Followup = " ", Notes_Exists = "N", Notes_Key = null, CAMS_DESC = null, CA_Template_SW = "N", MS_Template_SW = "N";
                string Add_Date = null, Add_Opr = null, Lstc_Date = null, Lstc_Opr = null, Posted_Year = null, Tmp_MS_ID = " ", CAMS_Active_Status = "";

                foreach (CASESP2Entity Entity in SP_CAMS_Details)
                {
                    if (Entity.Branch == Branch_Code)
                    {
                        CASEACT_Exists = CASEMS_Exists = false;
                        Add_Date = Add_Opr = Lstc_Date = Lstc_Opr = null;
                        Comp_date = Followup = Notes_Key = " "; Posted_Year = "    ";
                        Notes_Exists = "N";

                        //Notes_Key = ("000000".Substring(0, (6 - Entity.ServPlan.Length)) + Entity.ServPlan) + Sp_Sequence + Entity.Type1 + Entity.Branch +
                        //            ("000000".Substring(0, (6 - Entity.Orig_Grp.ToString().Length)) + Entity.Orig_Grp.ToString()) + Entity.CamCd.Trim();
                        Notes_Key = Entity.ServPlan.Trim() + Sp_Sequence + Entity.Branch.Trim() +
                                    Entity.Orig_Grp.ToString() + Entity.Type1 + Entity.CamCd.Trim();

                        CAMS_Active_Status = "True";
                        if (!Entity.CAMS_Active.Equals("True") || Entity.SP2_CAMS_Active == "I")
                            CAMS_Active_Status = "False";

                        if (Entity.Type1 == "CA")
                        {
                            foreach (CASEACTEntity ActEnt in SP_Activity_Details)
                            {
                                //CASEACT_Exists = false;
                                if (ActEnt.Service_plan == Entity.ServPlan &&
                                    ActEnt.Branch == Entity.Branch &&
                                    ActEnt.Group == Entity.Orig_Grp.ToString() &&
                                   ActEnt.ACT_Code.Trim() == Entity.CamCd.Trim() &&
                                   ActEnt.SPM_Seq.Trim() == Sp_Sequence)
                                {
                                    Comp_date = ActEnt.ACT_Date; Followup = ActEnt.Followup_On;
                                    Add_Date = ActEnt.Add_Date; Add_Opr = ActEnt.Add_Operator;
                                    Lstc_Date = ActEnt.Lstc_Date; Lstc_Opr = ActEnt.Lsct_Operator;
                                    Posted_Year = ActEnt.Year;

                                    Notes_Exists = "N";
                                    if (int.Parse(ActEnt.Notes_Count) > 0)
                                        Notes_Exists = "Y";

                                    if (!string.IsNullOrEmpty(Comp_date))
                                        Comp_date = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Comp_date.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                                    if (!string.IsNullOrEmpty(ActEnt.Followup_Comp.Trim()))
                                        Followup = string.Empty;
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(Followup))
                                            Followup = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Followup.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);
                                    }

                                    CAMS_DESC = Entity.CAMS_Desc;
                                    if (CASEACT_Exists)
                                        CAMS_DESC = " ";

                                    CASEACT_Exists = true; //break;

                                    CA_Template_SW = "N";
                                    if (Act_Template_List.Count > 0 && !CA_Template_Set)  // To Set Selected Template CA Bold
                                    {
                                        if (Act_Template_List[0].Branch == Entity.Branch && Act_Template_List[0].Group == Entity.Orig_Grp.ToString() &&
                                           Act_Template_List[0].ACT_Code.Trim() == Entity.CamCd.Trim() && Act_Template_List[0].SPM_Seq.Trim() == Sp_Sequence)
                                        {
                                            //SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                                            CA_Template_Set = true; CA_Template_SW = "Y";
                                            Act_Template_List.Clear();
                                            Act_Template_List.Add(new CASEACTEntity(ActEnt));
                                        }
                                    }

                                    //rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + CAMS_DESC) : CAMS_DESC), Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Entity.Branch, Entity.Orig_Grp, Notes_Exists, Notes_Key + ActEnt.ACT_Seq, ActEnt.ACT_Seq, Entity.CAMS_Desc, ActEnt.Year, Entity.CAMS_Active, ActEnt.ACT_ID, Entity.Curr_Grp, Entity.Branch, CA_Template_SW);
                                    rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + CAMS_DESC) : CAMS_DESC), string.Empty, Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Entity.Branch, Entity.Orig_Grp, Notes_Exists, Notes_Key + ActEnt.ACT_Seq + ActEnt.ACT_ID, ActEnt.ACT_Seq, Entity.CAMS_Desc, ActEnt.Year, CAMS_Active_Status, ActEnt.ACT_ID, Entity.Curr_Grp, Entity.Branch, CA_Template_SW);
                                    set_CAMS_Tooltip(rowIndex, Add_Date, Add_Opr, Lstc_Date, Lstc_Opr);
                                    Pub_SP2_Cnt++; Pub_SP2_CAMS_Posting_Cnt++;

                                    if (CA_Template_SW == "Y")
                                        SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);

                                    if (!string.IsNullOrEmpty(Sel_CAMS_Key))
                                    {
                                        if (Sel_CAMS_Key == Entity.Orig_Grp.ToString() + Entity.Type1 + Entity.CamCd.Trim() + ActEnt.ACT_Seq.Trim())
                                            Sel_CAMS_Index = rowIndex;
                                    }

                                    if (!Entity.CAMS_Active.Equals("True") || Entity.SP2_CAMS_Active == "I")
                                        SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;

                                    //Commented by Sudheer on 04/21/2018
                                    //if (Spm_Year != BaseForm.BaseYear)
                                    //    SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;

                                }
                            }
                            if (!CASEACT_Exists && Entity.SP2_CAMS_Active == "A" && Entity.CAMS_Active.Equals("True"))
                            {
                                //rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Entity.Branch, Entity.Orig_Grp, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, Entity.CAMS_Active, " ", Entity.Curr_Grp, Entity.Branch, "N");
                                rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), string.Empty, Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Entity.Branch, Entity.Orig_Grp, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, CAMS_Active_Status, " ", Entity.Curr_Grp, Entity.Branch, "N");
                                //Commented by Sudheer on 04/21/2018
                                //SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;
                                Pub_SP2_Cnt++;
                            }

                            //if (!Entity.CAMS_Active.Equals("True") || Entity.SP2_CAMS_Active == "I")
                            //    SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red; 

                        }
                        else
                        {
                            Tmp_MS_ID = " "; MS_Template_SW = "N";
                            foreach (CASEMSEntity MSEnt in SP_MS_Details)
                            {
                                if (MSEnt.Service_plan == Entity.ServPlan &&
                                    MSEnt.Branch == Entity.Branch &&
                                    MSEnt.Group == Entity.Orig_Grp.ToString() &&
                                   MSEnt.MS_Code.Trim() == Entity.CamCd.Trim())
                                {
                                    Comp_date = MSEnt.Date; Followup = MSEnt.MS_FUP_Date;
                                    Add_Date = MSEnt.Add_Date; Add_Opr = MSEnt.Add_Operator;
                                    Lstc_Date = MSEnt.Lstc_Date; Lstc_Opr = MSEnt.Lsct_Operator;
                                    Posted_Year = MSEnt.Year;

                                    Notes_Exists = "N";
                                    if (int.Parse(MSEnt.Notes_Count) > 0)
                                        Notes_Exists = "Y";

                                    if (!string.IsNullOrEmpty(Sel_CAMS_Key))
                                    {
                                        if (Sel_CAMS_Key == Entity.Orig_Grp.ToString() + Entity.Type1 + Entity.CamCd.Trim())
                                            Sel_CAMS_Index = rowIndex;
                                    }

                                    Tmp_MS_ID = MSEnt.ID;

                                    CAMS_DESC = Entity.CAMS_Desc;
                                    if (CASEACT_Exists)
                                        CAMS_DESC = " ";

                                    CASEACT_Exists = true;

                                    MS_Template_SW = "N";
                                    if (MS_Template_List.Count > 0 && !MS_Template_Set)  // To Set Selected Template CA Bold
                                    {
                                        if (MS_Template_List[0].Branch == Entity.Branch && MS_Template_List[0].Group == Entity.Orig_Grp.ToString() &&
                                           MS_Template_List[0].MS_Code.Trim() == Entity.CamCd.Trim() && MS_Template_List[0].SPM_Seq.Trim() == Sp_Sequence)
                                        {
                                            //SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                                            MS_Template_Set = true; MS_Template_SW = "Y";
                                            MS_Template_List.Clear();
                                            MS_Template_List.Add(new CASEMSEntity(MSEnt));
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(Comp_date.Trim()))
                                        Comp_date = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Comp_date.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                                    if (!string.IsNullOrEmpty(MSEnt.MS_Comp_Date.Trim()))
                                        Followup = string.Empty;
                                    else
                                    {
                                        Followup = MSEnt.MS_FUP_Date;
                                        if (!string.IsNullOrEmpty(Followup))
                                            Followup = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Followup.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);
                                    }

                                    if (CASEACT_Exists)
                                    {

                                        //rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + CAMS_DESC) : CAMS_DESC), Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Entity.Branch, Entity.Orig_Grp, Notes_Exists, Notes_Key + Comp_date, " ", Entity.CAMS_Desc, Posted_Year, Entity.CAMS_Active, Tmp_MS_ID, Entity.Curr_Grp, Entity.Branch, MS_Template_SW);
                                        rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + CAMS_DESC) : CAMS_DESC), MsResultDescription(MSEnt.Result), Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Entity.Branch, Entity.Orig_Grp, Notes_Exists, Notes_Key + Comp_date, " ", Entity.CAMS_Desc, Posted_Year, CAMS_Active_Status, Tmp_MS_ID, Entity.Curr_Grp, Entity.Branch, MS_Template_SW);
                                        set_CAMS_Tooltip(rowIndex, Add_Date, Add_Opr, Lstc_Date, Lstc_Opr);
                                        Pub_SP2_Cnt++; Pub_SP2_CAMS_Posting_Cnt++;

                                        if (MS_Template_SW == "Y")
                                            SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);

                                        SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue; //Color.Peru; //Color.DarkTurquoise;

                                        if (!Entity.CAMS_Active.Equals("True") || Entity.SP2_CAMS_Active == "I")
                                            SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;

                                        //Commented by Sudheer on 04/21/2018
                                        //if (Spm_Year != BaseForm.BaseYear)
                                        //    SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;

                                    }

                                    //break;
                                }
                            }

                            //if (!string.IsNullOrEmpty(Comp_date.Trim()))
                            //    Comp_date = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Comp_date.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                            //if (CASEACT_Exists)
                            //{
                            //    rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Entity.Branch, Entity.Orig_Grp, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, Entity.CAMS_Active, Tmp_MS_ID, Entity.Curr_Grp, Entity.Branch, MS_Template_SW);
                            //    set_CAMS_Tooltip(rowIndex, Add_Date, Add_Opr, Lstc_Date, Lstc_Opr);
                            //    Pub_SP2_Cnt++; Pub_SP2_CAMS_Posting_Cnt++;

                            //    if(MS_Template_SW == "Y")
                            //        SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                            //}
                            //else
                            //{
                            //    rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Entity.Branch, Entity.Orig_Grp, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, Entity.CAMS_Active, " ", Entity.Curr_Grp, Entity.Branch, "N");
                            //    SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;
                            //    Pub_SP2_Cnt++;
                            //}

                            if (!CASEACT_Exists && Entity.SP2_CAMS_Active == "A" && Entity.CAMS_Active.Equals("True"))
                            {

                                //rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Entity.Branch, Entity.Orig_Grp, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, Entity.CAMS_Active, " ", Entity.Curr_Grp, Entity.Branch, "N");
                                rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), string.Empty, Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Entity.Branch, Entity.Orig_Grp, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, CAMS_Active_Status, " ", Entity.Curr_Grp, Entity.Branch, "N");
                                //Commented by Sudheer on 04/21/2018
                                //SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;
                                Pub_SP2_Cnt++;

                                SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue; //Color.Peru; //Color.DarkTurquoise;

                                //if (!Entity.CAMS_Active.Equals("True") || Entity.SP2_CAMS_Active == "I")
                                //    SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red; 
                            }
                        }

                        //if (Entity.Type1 == "MS" && Entity.SP2_CAMS_Active == "A" && Entity.CAMS_Active.Equals("True"))
                        //{
                        //    SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue; //Color.Peru; //Color.DarkTurquoise;

                        //    //if (!Entity.CAMS_Active.Equals("True") || Entity.SP2_CAMS_Active == "I")
                        //    //    SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red; 
                        //}


                        rowIndex++;
                    }
                }


                if (Pub_SP2_Cnt > 0)
                {
                    SP_CAMS_Grid.Update();
                    SP_CAMS_Grid.CurrentCell = SP_CAMS_Grid.Rows[Sel_CAMS_Index].Cells[1];

                    int scrollPosition = 0;
                    scrollPosition = SP_CAMS_Grid.CurrentCell.RowIndex;
                    //int CurrentPage = (scrollPosition / SP_CAMS_Grid.ItemsPerPage);
                    //CurrentPage++;
                    //SP_CAMS_Grid.CurrentPage = CurrentPage;
                    //SP_CAMS_Grid.FirstDisplayedScrollingRowIndex = scrollPosition;

                    if (Privileges.DelPriv.Equals("true"))
                        PbDelete.Visible = true;
                    else
                        PbDelete.Visible = false;

                    if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C")
                    {
                        if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_SW"].Value.ToString() == "Y")
                            PB_SP2_Notes.ImageSource = "captain-casenotes";
                        else
                            PB_SP2_Notes.ImageSource = "captain-casenotesadd"; ;

                        if (Privileges.AddPriv.Equals("true") && Sw_ReadOnly == "N")
                            Pb_Edit.Visible = true;

                        PB_SP2_Notes.Visible = true;

                        //if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA" &&
                        //    Spm_Year == BaseForm.BaseYear && Privileges.AddPriv.Equals("true"))
                        //    Pb_Add_CA.Visible = true;
                        //else
                        //    Pb_Add_CA.Visible = false;    // Commented to Allow Multiple postings for MS

                        if (Privileges.AddPriv.Equals("true"))
                        {
                            if (Sw_ReadOnly == "Y")
                            {
                                Pb_SPM2_Add.Visible = false;
                                Pb_SPM2_Edit.Visible = false;
                                Pb_SPM2_Delete.Visible = false;
                                Pb_Add_CA.Visible = false;
                                Pb_Edit.Visible = false;
                                PbDelete.Visible = false;
                                Btn_AutoPost_CA.Visible = Btn_AutoPost_MS.Visible = Btn_Bulk_Posting.Visible = Btn_Maintain_Add.Visible = false;
                            }
                            else
                            {
                                Pb_Add_CA.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        Pb_Add_CA.Visible = false;
                        if (Spm_Year == BaseForm.BaseYear && Privileges.AddPriv.Equals("true") && SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() != "Branch")
                        {
                            if (Sw_ReadOnly == "Y")
                            {
                                Pb_SPM2_Add.Visible = false;
                                Pb_SPM2_Edit.Visible = false;
                                Pb_SPM2_Delete.Visible = false;
                                Pb_Add_CA.Visible = false;
                                Pb_Edit.Visible = false;
                                PbDelete.Visible = false;
                                Btn_AutoPost_CA.Visible = Btn_AutoPost_MS.Visible = Btn_Bulk_Posting.Visible = Btn_Maintain_Add.Visible = false;
                            }
                            else
                            {
                                Pb_Add_CA.Visible = true;
                            }
                        }

                        Pb_Edit.Visible = PB_SP2_Notes.Visible = false;
                    }

                    Sel_CAMS_Notes_Key = Hierarchy + Spm_Year + App_No + SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_Key"].Value.ToString();
                }
            }

            if (Pub_SP2_Cnt == 0)
                Pb_Add_CA.Visible = Pb_Edit.Visible = PbDelete.Visible = PB_SP2_Notes.Visible = false;
            else if (Privileges.AddPriv.Equals("true") && SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() != "Branch")
            {
                if (Sw_ReadOnly == "Y")
                {
                    Pb_SPM2_Add.Visible = false;
                    Pb_SPM2_Edit.Visible = false;
                    Pb_SPM2_Delete.Visible = false;
                    Pb_Add_CA.Visible = false;
                    Pb_Edit.Visible = false;
                    PbDelete.Visible = false;
                    Btn_AutoPost_CA.Visible = Btn_AutoPost_MS.Visible = Btn_Bulk_Posting.Visible = Btn_Maintain_Add.Visible = false;
                }
                else
                {
                    Pb_Add_CA.Visible = true;
                }
            }

            // buttonAllowAMDSwitch();

        }

        private void Fill_All_DropDowns()
        {
            CmbSite.Items.Clear();
            CmbSite.ColorMember = "FavoriteColor";

            List<Captain.Common.Utilities.ListItem> listItem1 = new List<Captain.Common.Utilities.ListItem>();
            listItem1.Add(new Captain.Common.Utilities.ListItem("   ", "0", " ", Color.White));

            //DataSet ds = Captain.DatabaseLayer.Lookups.GetCaseSite();
            DataSet ds = Captain.DatabaseLayer.CaseMst.GetSiteByHIE(BaseForm.BaseAgency, string.Empty, string.Empty);
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                if (Mode.Equals("Add"))
                {
                    DataView dv = new DataView(dt);
                    dv.RowFilter = "SITE_ACTIVE='Y'";
                    dt = dv.ToTable();
                }
            }

            foreach (DataRow dr in dt.Rows)
                listItem1.Add(new Captain.Common.Utilities.ListItem(dr["SITE_NAME"].ToString(), dr["SITE_NUMBER"].ToString().Trim(), dr["SITE_ACTIVE"].ToString().Trim(), (dr["SITE_ACTIVE"].ToString().Trim().Equals("Y") ? Color.Black : Color.Red)));

            //CmbSite.Items.Add(new Captain.Common.Utilities.ListItem(" ", "0"," ", Color.White));

            if (BaseForm.BaseAgencyControlDetails.SiteSecurity == "1")
            {
                List<HierarchyEntity> userHierarchy = _model.UserProfileAccess.GetUserHierarchyByID(BaseForm.UserID);
                HierarchyEntity hierarchyEntity = new HierarchyEntity(); List<CaseSiteEntity> selsites = new List<CaseSiteEntity>();
                foreach (HierarchyEntity Entity in userHierarchy)
                {
                    if (Entity.InActiveFlag == "N")
                    {
                        if (Entity.Agency == BaseForm.BaseAgency && Entity.Dept == BaseForm.BaseDept && Entity.Prog == BaseForm.BaseProg)
                            hierarchyEntity = Entity;
                        else if (Entity.Agency == BaseForm.BaseAgency && Entity.Dept == BaseForm.BaseDept && Entity.Prog == "**")
                            hierarchyEntity = Entity;
                        else if (Entity.Agency == BaseForm.BaseAgency && Entity.Dept == "**" && Entity.Prog == "**")
                            hierarchyEntity = Entity;
                        else if (Entity.Agency == "**" && Entity.Dept == "**" && Entity.Prog == "**")
                        { hierarchyEntity = null; }
                    }
                }

                if (hierarchyEntity != null)
                {
                    if (hierarchyEntity.Sites.Length > 0)
                    {
                        string[] Sites = hierarchyEntity.Sites.Split(',');
                        List<Captain.Common.Utilities.ListItem> listItemSite = new List<Captain.Common.Utilities.ListItem>();
                        listItemSite.Add(new Captain.Common.Utilities.ListItem("   ", "0", " ", Color.White));
                        for (int i = 0; i < Sites.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(Sites[i].ToString().Trim()))
                            {
                                foreach (Captain.Common.Utilities.ListItem casesite in listItem1) //Site_List)//ListcaseSiteEntity)
                                {
                                    if (Sites[i].ToString() == casesite.Value.ToString())
                                    {
                                        listItemSite.Add(casesite);
                                        //break;
                                    }
                                    // Sel_Site_Codes += "'" + casesite.SiteNUMBER + "' ,";
                                }
                            }
                        }
                        //strsiteRoomNames = hierarchyEntity.Sites;
                        listItem1 = listItemSite;
                    }
                }
            }

            //foreach (DataRow dr in dt.Rows)
            //{
            //    CmbSite.Items.Add(new Captain.Common.Utilities.ListItem(dr["SITE_NAME"].ToString(), dr["SITE_NUMBER"].ToString().Trim(), dr["SITE_ACTIVE"].ToString().Trim(), (dr["SITE_ACTIVE"].ToString().Trim().Equals("Y") ? Color.Green : Color.Red)));
            //}
            CmbSite.Items.AddRange(listItem1.ToArray());
            CmbSite.SelectedIndex = 0;
            //DataSet ds2 = Captain.DatabaseLayer.AgyTab.GetHierarchyNames(Hierarchy.Substring(0, 2), Hierarchy.Substring(2, 2), Hierarchy.Substring(4, 2));
            DataSet ds2 = Captain.DatabaseLayer.AgyTab.GetHierarchyNames(Hierarchy.Substring(0, 2), "**", "**");
            string strNameFormat = null, strCwFormat = null;
            if (ds2.Tables[0].Rows.Count > 0)
            {
                strNameFormat = ds2.Tables[0].Rows[0]["HIE_CN_FORMAT"].ToString();
                strCwFormat = ds2.Tables[0].Rows[0]["HIE_CW_FORMAT"].ToString();
            }

            CmbWorker.Items.Clear();

            CmbWorker.ColorMember = "FavoriteColor";

            List<Captain.Common.Utilities.ListItem> listItem = new List<Captain.Common.Utilities.ListItem>();
            //CmbWorker.Items.Insert(0, new ListItem("All", "**"));

            listItem.Add(new Captain.Common.Utilities.ListItem(" ", "0", " ", Color.White));
            DataSet ds1 = Captain.DatabaseLayer.CaseMst.GetCaseWorker(strCwFormat, BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            if (ds1.Tables.Count > 0)
            {
                DataTable dt1 = ds1.Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt1.Rows)
                        listItem.Add(new Captain.Common.Utilities.ListItem(dr["NAME"].ToString().Trim(), dr["PWH_CASEWORKER"].ToString().Trim(), dr["PWH_INACTIVE"].ToString(), (dr["PWH_INACTIVE"].ToString().Equals("Y")) ? Color.Red : Color.Black));
                }
            }
            CmbWorker.Items.AddRange(listItem.ToArray());


            if (Mode.Equals("Add"))
            {
                if (CmbSite.Items.Count > 0 && (!string.IsNullOrEmpty(App_MST_Entity.Site.Trim())))
                    SetComboBoxValue(CmbSite, App_MST_Entity.Site.Trim());
                else
                    CmbSite.SelectedIndex = 0;

                if (CmbWorker.Items.Count > 0 && (!string.IsNullOrEmpty(App_MST_Entity.IntakeWorker.Trim())))
                    SetComboBoxValue(CmbWorker, App_MST_Entity.IntakeWorker.Trim());
                else
                    CmbWorker.SelectedIndex = 0;
            }
        }


        private void SetComboBoxValue(ComboBox comboBox, string value)
        {
            if (string.IsNullOrEmpty(value) || value == " ")
                value = "0";
            if (comboBox != null && comboBox.Items.Count > 0)
            {
                foreach (Captain.Common.Utilities.ListItem li in comboBox.Items)
                {
                    if (li.Value.Equals(value) || li.Text.Equals(value))
                    {
                        comboBox.SelectedItem = li;
                        break;
                    }
                }
            }
        }

        private void Fill_SP_DropDowns()
        {
            this.CmbSP.SelectedIndexChanged -= new System.EventHandler(this.CmbSP_SelectedIndexChanged);
            CmbSP.Items.Clear();
            List<CASESP1Entity> SP_Hierarchies = new List<CASESP1Entity>();
            ACR_SERV_Hies = "N";
            if (!string.IsNullOrEmpty(BaseForm.BaseAgencyControlDetails.ServicePlanHiecontrol.Trim()))
            {
                if (BaseForm.BaseAgencyControlDetails.ServicePlanHiecontrol.Trim() == "Y")
                    ACR_SERV_Hies = "S";
            }

            if (ACR_SERV_Hies == "Y" || ACR_SERV_Hies == "S")
            {
                if (BaseForm.BaseAgencyControlDetails.SerPlanAllow.Trim() == "D")
                    SP_Hierarchies = _model.SPAdminData.CASESP1_SerPlans(null, Hierarchy.Substring(0, 2), Hierarchy.Substring(2, 2), null, BaseForm.UserID);
                else
                    SP_Hierarchies = _model.SPAdminData.CASESP1_SerPlans(null, Hierarchy.Substring(0, 2), null, null, BaseForm.UserID);
            }
            else
                SP_Hierarchies = _model.SPAdminData.Browse_CASESP1(null, Hierarchy.Substring(0, 2), Hierarchy.Substring(2, 2), Hierarchy.Substring(4, 2));


            CmbSP.ColorMember = "FavoriteColor";

            if (SP_Hierarchies.Count > 0)
            {
                bool SP_Exists = false, Allow_Dups = false;
                string Tmp_SP_Desc = null, Tmp_SP_Code = null, SP_Valid = null, SPM_Start_Date = " ", SP_DESC = " ", spm_posting_year = "";
                int Tmp_Sel_Index = 0, Itr_Index = 0;

                if (Mode.Equals("Edit"))
                {
                    foreach (CASESPMEntity Entity1 in CASESPM_SP_List)
                    {
                        SP_DESC = " ";
                        //foreach (CASESP1Entity Entity in SP_Hierarchies)  // 08122012
                        //{
                        //    if (Entity1.service_plan == Entity.Code)
                        //    {
                        //        SP_Exists = true;
                        //        SP_Valid = Entity1.Sp0_Validatetd;
                        //        SPM_Start_Date = Entity1.startdate;
                        //        SP_DESC = Entity.SP_Desc;

                        //        if (Entity.SP_Allow_Dups == "Y")
                        //            Allow_Dups = true;

                        //        break;
                        //    }
                        //}

                        CASESP1Entity casesp1data = SP_Hierarchies.Find(u => u.Code == Entity1.service_plan);
                        if (casesp1data != null)
                        {
                            SP_Exists = true;
                            if (Entity1.Sp0_Validatetd.ToUpper() == "Y" && Entity1.Sp0_Active.ToUpper() == "Y")
                            {
                                SP_Valid = "Y";
                            }
                            else
                            {
                                SP_Valid = "N";
                            }
                            SPM_Start_Date = Entity1.startdate;
                            SP_DESC = casesp1data.SP_Desc;

                            if (casesp1data.SP_Allow_Dups == "Y")
                                Allow_Dups = true;
                        }

                        if ((Mode.Equals("Edit") && SP_Exists))// || (Mode.Equals("Add") && !SP_Exists) || (Mode.Equals("Add") && Allow_Dups) ||
                        {
                            Tmp_SP_Code = "000000".Substring(0, (6 - Entity1.service_plan.Length)) + Entity1.service_plan;

                            if (SP_Code == Tmp_SP_Code && Entity1.Seq == Sp_Sequence && Spm_Year == Entity1.year)
                                Tmp_Sel_Index = Itr_Index;

                            if (!string.IsNullOrEmpty(SPM_Start_Date.Trim()))
                                SPM_Start_Date = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(SPM_Start_Date).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);
                            else
                                SPM_Start_Date = " ";

                            spm_posting_year = "";
                            spm_posting_year = (string.IsNullOrEmpty(Entity1.year.ToString().Trim()) ? "" : " - PY" + Entity1.year.ToString().Trim());

                            //if(Mode.Equals("Add"))
                            //    CmbSP.Items.Add(new Captain.Common.Utilities.ListItem(Tmp_SP_Code + " - " + Entity.SP_Desc.Trim(), Entity.Code.ToString(), SP_Valid, (SP_Valid.Equals("Y") ? Color.Green : Color.Red)));
                            //else
                            CmbSP.Items.Add(new Captain.Common.Utilities.ListItem(Tmp_SP_Code + " - " + SPM_Start_Date + " - " + SP_DESC.Trim() + spm_posting_year, Entity1.service_plan.ToString(), SP_Valid, (SP_Valid.Equals("Y") ? Color.Black : Color.Red), Entity1.Seq, Entity1.year));
                            Itr_Index++;
                        }
                    }

                    if (CmbSP.Items.Count > 0)
                    {
                        CmbSP.SelectedIndex = Tmp_Sel_Index;
                        SP_Programs_List = _model.lookupDataAccess.Get_SerPlan_Prog_List(BaseForm.UserProfile.UserID, ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), ACR_SERV_Hies);
                    }
                }


                if (Mode.Equals("Add"))
                {
                    foreach (CASESP1Entity Entity in SP_Hierarchies)  // 08122012
                    {
                        SP_Exists = Allow_Dups = false;
                        Tmp_SP_Desc = null;
                        // SP_Valid = Entity.SP_validated;
                        SPM_Start_Date = " ";
                        if (Entity.SP_validated.ToUpper() == "Y" && Entity.Sp0_Active.ToUpper() == "Y")
                        {
                            SP_Valid = "Y";
                        }
                        else
                        {
                            SP_Valid = "N";
                        }
                        foreach (CASESPMEntity Entity1 in CASESPM_SP_List)
                        {
                            if (Entity1.service_plan == Entity.Code)
                            {
                                SP_Exists = true;
                                if (Entity1.Sp0_Validatetd.ToUpper() == "Y" && Entity1.Sp0_Active.ToUpper() == "Y")
                                {
                                    SP_Valid = "Y";
                                }
                                else
                                {
                                    SP_Valid = "N";
                                }
                                SPM_Start_Date = Entity1.startdate;

                                if (Entity.SP_Allow_Dups == "Y")
                                    Allow_Dups = true;

                                break;
                            }
                        }

                        if (SP_Valid.ToUpper() == "Y")
                        {
                            if ((Mode.Equals("Add") && !SP_Exists) || (Mode.Equals("Add") && Allow_Dups))// || (Mode.Equals("Edit") && SP_Exists))
                            {
                                ////Tmp_SP_Desc = string.Empty;
                                ////Tmp_SP_Desc = Entity.SP_Desc.Trim();
                                //if (Tmp_SP_Desc.Length > 150)
                                //    Tmp_SP_Desc = Tmp_SP_Desc.Substring(0, 150);
                                //Tmp_SP_Desc = String.Format("{0,-70}", Tmp_SP_Desc) + String.Format("{0,6}", " - " + Entity.Code.ToString());


                                //if (string.IsNullOrEmpty(SPM_Start_Date.Trim()))
                                //    SPM_Start_Date = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(SPM_Start_Date).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                                Tmp_SP_Code = "000000".Substring(0, (6 - Entity.Code.Length)) + Entity.Code;

                                //if(Mode.Equals("Add"))
                                //    CmbSP.Items.Add(new Captain.Common.Utilities.ListItem(Tmp_SP_Code + " - " + Entity.SP_Desc.Trim(), Entity.Code.ToString(), SP_Valid, (SP_Valid.Equals("Y") ? Color.Green : Color.Red)));
                                //else
                                if (propSearch_Entity.Count > 0)
                                {
                                    CASESP0Entity casesp0data = propSearch_Entity.Find(u => u.Code == Entity.Code);
                                    if (casesp0data != null)
                                    {
                                        if (casesp0data.Sp0ReadOnly != "Y" && casesp0data.NoSPM != "Y")
                                        {
                                            CmbSP.Items.Add(new Captain.Common.Utilities.ListItem(Tmp_SP_Code + " - " + Entity.SP_Desc.Trim(), Entity.Code.ToString(), SP_Valid, (SP_Valid.Equals("Y") ? Color.Black : Color.Red)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.CmbSP.SelectedIndexChanged += new System.EventHandler(this.CmbSP_SelectedIndexChanged);
            if (Mode.Equals("Add") && CmbSP.Items.Count == 1) CmbSP.SelectedIndex = 0;

        }

        List<CASESPM2Entity> ADD_CAMA_Details = new List<CASESPM2Entity>();
        private void Fill_Additional_CAMS_Details(string Code)
        {
            ADD_CAMS_Grid.Rows.Clear();
            ADD_CAMA_Details.Clear();
            CASESPM2Entity Search_Entity2 = new CASESPM2Entity();

            Search_Entity2.Agency = Hierarchy.Substring(0, 2);
            Search_Entity2.Dept = Hierarchy.Substring(2, 2);
            Search_Entity2.Prog = Hierarchy.Substring(4, 2);
            //Search_Entity2.Year = BaseYear;
            Search_Entity2.Year = Spm_Year;                         // Year will be always Four-Spaces in CASESPM2
            Search_Entity2.App = App_No;
            Search_Entity2.Spm_Seq = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString();

            Search_Entity2.ServPlan = Search_Entity2.Branch = Search_Entity2.Group = null;
            Search_Entity2.Type1 = Search_Entity2.CamCd = Search_Entity2.Curr_Group = null;
            Search_Entity2.SelOrdinal = Search_Entity2.DateLstc = Search_Entity2.lstcOperator = null;
            Search_Entity2.Dateadd = Search_Entity2.addoperator = null;

            Search_Entity2.ServPlan = Code;


            ADD_CAMA_Details = _model.SPAdminData.Browse_CASESPM2(Search_Entity2, "Browse");

            //if (ADD_CAMA_Details.Count > 0)
            //    Fill_Additional_CAMS_Details_Grid();
        }

        string Sel_SPM2_Notes_Key = string.Empty;
        private void Fill_Additional_CAMS_Details_Grid()
        {
            if (!Cb_Show_All_Postings.Checked)
            {
                Clear_SP_CAMS_Grid();
                Pub_SPM2_Cnt = Pub_SPM2_CAMS_Posting_Cnt = 0;
            }
            //SP_CAMS_Grid.Rows.Clear();

            if (SP_Readonly_Sw == "Y") Sw_ReadOnly = "Y"; else if (((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).ID.ToString() == "N") Sw_ReadOnly = "Y"; else Sw_ReadOnly = "N";

            Fill_Additional_CAMS_Details(SP_Code);
            if (ADD_CAMA_Details.Count > 0)
            {
                int rowIndex = 0, Sel_CAMS_Index = 0, Tmp_Row_Ordinal = 0;
                bool CASEACT_Exists = false, CASEMS_Exists = false, CA_Template_Set = false, MS_Template_Set = false; ;
                string Comp_date = null, Followup = " ", Notes_Exists = "N", Notes_Key = null, CAMS_DESC = null, CA_Template_SW = "N", MS_Template_SW = "N";
                string Add_Date = null, Add_Opr = null, Lstc_Date = null, Lstc_Opr = null, Posted_Year = null, Tmp_MS_ID = " ", CAMS_Active_Status = "";

                if (Cb_Show_All_Postings.Checked)
                {
                    rowIndex = SP_CAMS_Grid.Rows.Add(false, "Additional Branch", " ", " ", " ", Img_Add, "Branch", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "N");
                    SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;
                    SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                }


                foreach (CASESPM2Entity Entity in ADD_CAMA_Details)
                {
                    if (Entity.Branch == "9")
                    {
                        CASEACT_Exists = CASEMS_Exists = false;
                        Add_Date = Add_Opr = Lstc_Date = Lstc_Opr = null;
                        Comp_date = Followup = Notes_Key = " "; Posted_Year = "    ";
                        Notes_Exists = "N";
                        Tmp_Row_Ordinal++;

                        //Notes_Key = ("000000".Substring(0, (6 - Entity.ServPlan.Length)) + Entity.ServPlan) + Sp_Sequence + Entity.Type1 + Entity.Branch +
                        //            ("000000".Substring(0, (6 - Entity.Group.ToString().Length)) + Entity.Group.ToString()) + Entity.CamCd.Trim();
                        Notes_Key = Entity.ServPlan.Trim() + Sp_Sequence + Entity.Branch.Trim() +
                                    Entity.Group.ToString() + Entity.Type1 + Entity.CamCd.Trim();

                        CAMS_Active_Status = "True";
                        if (!Entity.CAMS_Active.Equals("True"))// || Entity.SP2_CAMS_Active == "I")
                            CAMS_Active_Status = "False";

                        if (Entity.Type1 == "CA")
                        {
                            foreach (CASEACTEntity ActEnt in SP_Activity_Details)
                            {
                                //CASEACT_Exists = false;
                                if (ActEnt.Service_plan == Entity.ServPlan &&
                                    ActEnt.Branch == Entity.Branch &&
                                    ActEnt.Group == Entity.Group.ToString() &&
                                   ActEnt.ACT_Code.Trim() == Entity.CamCd.Trim())
                                {
                                    Comp_date = ActEnt.ACT_Date; Followup = ActEnt.Followup_On;
                                    Add_Date = ActEnt.Add_Date; Add_Opr = ActEnt.Add_Operator;
                                    Lstc_Date = ActEnt.Lstc_Date; Lstc_Opr = ActEnt.Lsct_Operator;
                                    Posted_Year = ActEnt.Year;

                                    Notes_Exists = "N";
                                    if (int.Parse(ActEnt.Notes_Count) > 0)
                                        Notes_Exists = "Y";

                                    if (!string.IsNullOrEmpty(Comp_date))
                                        Comp_date = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Comp_date.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);
                                    if (!string.IsNullOrEmpty(ActEnt.Followup_On.Trim()))
                                        Followup = string.Empty;
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(Followup))
                                            Followup = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Followup.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);
                                    }

                                    CAMS_DESC = Entity.CAMS_Desc;
                                    if (CASEACT_Exists)
                                        CAMS_DESC = " ";

                                    CASEACT_Exists = true; //break;

                                    CA_Template_SW = "N";
                                    if (Act_Template_List.Count > 0 && !CA_Template_Set)  // To Set Selected Template CA Bold
                                    {
                                        if (Act_Template_List[0].Branch == Entity.Branch && Act_Template_List[0].Group == Entity.Group.ToString() &&
                                           Act_Template_List[0].ACT_Code.Trim() == Entity.CamCd.Trim() && Act_Template_List[0].SPM_Seq.Trim() == Sp_Sequence)
                                        {
                                            CA_Template_Set = true; CA_Template_SW = "Y";
                                            Act_Template_List.Clear();
                                            Act_Template_List.Add(new CASEACTEntity(ActEnt));
                                        }
                                    }


                                    //rowIndex = ADD_CAMS_Grid.Rows.Add(false, CAMS_DESC, Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Entity.SelOrdinal, Entity.Group, Notes_Exists, Notes_Key + ActEnt.ACT_Seq, ActEnt.ACT_Seq, Entity.CAMS_Desc, ActEnt.Year, "Y", ActEnt.ACT_ID, Entity.Curr_Group); // CA Active Status

                                    //rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + CAMS_DESC) : CAMS_DESC), Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Entity.SelOrdinal, Entity.Group, Notes_Exists, Notes_Key + ActEnt.ACT_Seq, ActEnt.ACT_Seq, Entity.CAMS_Desc, ActEnt.Year, "Y", ActEnt.ACT_ID, Entity.Curr_Group); // CA Active Status
                                    //rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + CAMS_DESC) : CAMS_DESC), Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists, Notes_Key + ActEnt.ACT_Seq, ActEnt.ACT_Seq, Entity.CAMS_Desc, ActEnt.Year, "Y", ActEnt.ACT_ID, Entity.Curr_Group, Entity.Branch, "N"); // CA Active Status
                                    rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + CAMS_DESC) : CAMS_DESC), string.Empty, Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists, Notes_Key + ActEnt.ACT_Seq + ActEnt.ACT_ID, ActEnt.ACT_Seq, Entity.CAMS_Desc, ActEnt.Year, CAMS_Active_Status, ActEnt.ACT_ID, Entity.Curr_Group, Entity.Branch, "N"); // CA Active Status
                                    set_CAMS_Tooltip(rowIndex, Add_Date, Add_Opr, Lstc_Date, Lstc_Opr);
                                    Pub_SPM2_Cnt++; Pub_SPM2_CAMS_Posting_Cnt++;

                                    if (CA_Template_SW == "Y")
                                        SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);

                                    if (!Entity.CAMS_Active.Equals("True"))  //  || Entity.SP2_CAMS_Active == "I" Additional Branch Not yet defined
                                        SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;

                                    //Commented by Sudheer on 04/21/2018
                                    //if (Spm_Year != BaseForm.BaseYear)
                                    //    SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;

                                    //if (!string.IsNullOrEmpty(Sel_CAMS_Key))
                                    //{
                                    //    if (Sel_CAMS_Key == Entity.Group.ToString() + Entity.Type1 + Entity.CamCd.Trim() + ActEnt.ACT_Seq.Trim())
                                    //        Sel_CAMS_Index = rowIndex;
                                    //}
                                }
                            }
                            if (!CASEACT_Exists && Entity.CAMS_Active.Equals("True"))
                            {
                                //rowIndex = ADD_CAMS_Grid.Rows.Add(false, Entity.CAMS_Desc, Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Entity.SelOrdinal, Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", " ", Entity.Curr_Group);  // CA Active Status
                                //SP_CAMS_Grid.Rows[rowIndex].Cells["Del_2"].ReadOnly = true;

                                //rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", " ", Entity.Curr_Group, Entity.Branch, "N");  // CA Active Status
                                rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), string.Empty, Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, CAMS_Active_Status, " ", Entity.Curr_Group, Entity.Branch, "N");  // CA Active Status
                                //Commented by Sudheer on 04/21/2018
                                //SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;

                                //if (!Entity.CAMS_Active.Equals("True"))  //  || Entity.SP2_CAMS_Active == "I" Additional Branch Not yet defined
                                //    SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red; 

                                Pub_SPM2_Cnt++;
                            }
                        }
                        else
                        {
                            Tmp_MS_ID = " "; MS_Template_SW = "N";
                            foreach (CASEMSEntity MSEnt in SP_MS_Details)
                            {
                                if (MSEnt.Service_plan == Entity.ServPlan &&
                                    MSEnt.Branch == Entity.Branch &&
                                    MSEnt.Group == Entity.Group.ToString() &&
                                   MSEnt.MS_Code.Trim() == Entity.CamCd.Trim())
                                {
                                    Comp_date = MSEnt.Date; Followup = MSEnt.MS_FUP_Date;
                                    Add_Date = MSEnt.Add_Date; Add_Opr = MSEnt.Add_Operator;
                                    Lstc_Date = MSEnt.Lstc_Date; Lstc_Opr = MSEnt.Lsct_Operator;
                                    Posted_Year = MSEnt.Year;

                                    Notes_Exists = "N";
                                    if (int.Parse(MSEnt.Notes_Count) > 0)
                                        Notes_Exists = "Y";

                                    //if (!string.IsNullOrEmpty(Sel_CAMS_Key))
                                    //{
                                    //    if (Sel_CAMS_Key == Entity.Group.ToString() + Entity.Type1 + Entity.CamCd.Trim())
                                    //        Sel_CAMS_Index = rowIndex;
                                    //}

                                    Tmp_MS_ID = MSEnt.ID;

                                    CAMS_DESC = Entity.CAMS_Desc;
                                    if (CASEACT_Exists)
                                        CAMS_DESC = " ";

                                    CASEACT_Exists = true;

                                    MS_Template_SW = "N";
                                    if (MS_Template_List.Count > 0 && !MS_Template_Set)  // To Set Selected Template CA Bold
                                    {
                                        if (MS_Template_List[0].Branch == Entity.Branch && MS_Template_List[0].Group == Entity.Group.ToString() &&
                                           MS_Template_List[0].MS_Code.Trim() == Entity.CamCd.Trim() && MS_Template_List[0].SPM_Seq.Trim() == Sp_Sequence)
                                        {
                                            MS_Template_Set = true; MS_Template_SW = "Y";
                                            MS_Template_List.Clear();
                                            MS_Template_List.Add(new CASEMSEntity(MSEnt));
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(Comp_date.Trim()))
                                        Comp_date = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Comp_date.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                                    if (!string.IsNullOrEmpty(MSEnt.MS_Comp_Date.Trim()))
                                        Followup = string.Empty;
                                    else
                                    {
                                        Followup = MSEnt.MS_FUP_Date;
                                        if (!string.IsNullOrEmpty(Followup))
                                            Followup = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Followup.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);
                                    }

                                    //if (CASEACT_Exists)
                                    {
                                        //rowIndex = ADD_CAMS_Grid.Rows.Add(false, Entity.CAMS_Desc, Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Entity.SelOrdinal, Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", Tmp_MS_ID, Entity.Curr_Group);  // MS Active Status
                                        //rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + CAMS_DESC) : CAMS_DESC), Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists + Comp_date, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", Tmp_MS_ID, Entity.Curr_Group, Entity.Branch, MS_Template_SW);  // MS Active Status
                                        rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + CAMS_DESC) : CAMS_DESC), MsResultDescription(MSEnt.Result), Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists + Comp_date, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, CAMS_Active_Status, Tmp_MS_ID, Entity.Curr_Group, Entity.Branch, MS_Template_SW);  // MS Active Status
                                        set_CAMS_Tooltip(rowIndex, Add_Date, Add_Opr, Lstc_Date, Lstc_Opr);
                                        Pub_SPM2_Cnt++; Pub_SPM2_CAMS_Posting_Cnt++;


                                        if (MS_Template_SW == "Y")
                                            SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);

                                        SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue;

                                        if (!Entity.CAMS_Active.Equals("True"))
                                            SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;

                                        //Commented by Sudheer on 04/21/2018
                                        //if (Spm_Year != BaseForm.BaseYear)
                                        //    SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;
                                    }


                                    //break;
                                }
                            }

                            //if (!string.IsNullOrEmpty(Comp_date.Trim()))
                            //    Comp_date = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Comp_date.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                            //if (CASEACT_Exists)
                            //{
                            //    //rowIndex = ADD_CAMS_Grid.Rows.Add(false, Entity.CAMS_Desc, Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Entity.SelOrdinal, Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", Tmp_MS_ID, Entity.Curr_Group);  // MS Active Status
                            //    rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), Comp_date, Followup, Img_Edit, Entity.Type1, Entity.CamCd.Trim(), "C", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", Tmp_MS_ID, Entity.Curr_Group, Entity.Branch, MS_Template_SW);  // MS Active Status
                            //    set_CAMS_Tooltip(rowIndex, Add_Date, Add_Opr, Lstc_Date, Lstc_Opr);
                            //    Pub_SPM2_Cnt++; Pub_SPM2_CAMS_Posting_Cnt++;

                            //    if (MS_Template_SW == "Y")
                            //        SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                            //}
                            //else
                            //{
                            //    //rowIndex = ADD_CAMS_Grid.Rows.Add(false, Entity.CAMS_Desc, Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Entity.SelOrdinal, Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", " ", Entity.Curr_Group);  // MS Active Status
                            //    //SP_CAMS_Grid.Rows[rowIndex].Cells["Del_2"].ReadOnly = true;
                            //    rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", " ", Entity.Curr_Group, Entity.Branch, MS_Template_SW);  // MS Active Status
                            //    SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;
                            //    Pub_SPM2_Cnt++;
                            //}

                            if (!CASEACT_Exists && Entity.CAMS_Active.Equals("True"))
                            {
                                //rowIndex = ADD_CAMS_Grid.Rows.Add(false, Entity.CAMS_Desc, Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Entity.SelOrdinal, Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", " ", Entity.Curr_Group);  // MS Active Status
                                //SP_CAMS_Grid.Rows[rowIndex].Cells["Del_2"].ReadOnly = true;
                                //rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", " ", Entity.Curr_Group, Entity.Branch, MS_Template_SW);  // MS Active Status
                                rowIndex = SP_CAMS_Grid.Rows.Add(false, (Cb_Show_All_Postings.Checked ? ("     " + Entity.CAMS_Desc) : Entity.CAMS_Desc), string.Empty, Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, CAMS_Active_Status, " ", Entity.Curr_Group, Entity.Branch, MS_Template_SW);  // MS Active Status
                                //Commented by Sudheer on 04/21/2018
                                //SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;
                                Pub_SPM2_Cnt++;

                                SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue;
                            }

                            //if (Spm_Year != BaseForm.BaseYear)
                            //    SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;
                        }

                        //if (Entity.Type1 == "MS")
                        //{
                        //    //ADD_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue; //Color.Peru; //Color.DarkTurquoise;
                        //    SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue; //Color.Peru; //Color.DarkTurquoise;

                        //    if (!Entity.CAMS_Active.Equals("True"))
                        //        SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red; 
                        //}

                        rowIndex++;
                    }
                }


                if (rowIndex > 0)
                {
                    SP_CAMS_Grid.Update();
                    SP_CAMS_Grid.CurrentCell = SP_CAMS_Grid.Rows[Sel_CAMS_Index].Cells[1];

                    int scrollPosition = 0;
                    scrollPosition = SP_CAMS_Grid.CurrentCell.RowIndex;
                    // int CurrentPage = (scrollPosition / SP_CAMS_Grid.ItemsPerPage);
                    //CurrentPage++;
                    //SP_CAMS_Grid.CurrentPage = CurrentPage;
                    // SP_CAMS_Grid.FirstDisplayedScrollingRowIndex = scrollPosition;

                    if (Privileges.DelPriv.Equals("true") && Sw_ReadOnly == "N")
                        Pb_SPM2_Delete.Visible = true;
                    else
                        Pb_SPM2_Delete.Visible = false;

                    if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C")
                    {

                        if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_SW"].Value.ToString() == "Y")
                            Pb_SPM2_Notes.ImageSource = "captain-casenotes";
                        else
                            Pb_SPM2_Notes.ImageSource = "captain-casenotesadd"; ;
                        Pb_SPM2_Edit.Visible = Pb_SPM2_Notes.Visible = true;

                        if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA" && Sw_ReadOnly == "N")
                            Pb_SPM2_Add.Visible = true;
                        else
                            Pb_SPM2_Add.Visible = false;
                    }
                    else
                    {
                        if (Sw_ReadOnly == "Y") Pb_SPM2_Add.Visible = true; else Pb_SPM2_Add.Visible = false;
                        Pb_SPM2_Edit.Visible = PB_SP2_Notes.Visible = false;
                    }

                    Sel_SPM2_Notes_Key = Hierarchy + "    " + App_No + SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_Key"].Value.ToString();

                    Prepare_Menu_items();
                }
            }

            if (Pub_SPM2_Cnt == 0)
                Pb_Add_CA.Visible = Pb_Edit.Visible = PbDelete.Visible = PB_SP2_Notes.Visible = false;
            else if (Privileges.AddPriv.Equals("true") && SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() != "Branch")
            {
                if (Sw_ReadOnly == "Y")
                {
                    Pb_SPM2_Add.Visible = false;
                    Pb_SPM2_Edit.Visible = false;
                    Pb_SPM2_Delete.Visible = false;
                    Pb_Add_CA.Visible = false;
                    Pb_Edit.Visible = false;
                    PbDelete.Visible = false;
                    Btn_AutoPost_CA.Visible = Btn_AutoPost_MS.Visible = Btn_Bulk_Posting.Visible = Btn_Maintain_Add.Visible = false;
                }
                else
                {
                    Pb_Add_CA.Visible = true;
                }
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            CASE4006_AddCAMS_Form test = new CASE4006_AddCAMS_Form(BaseForm, Privileges, ADD_CAMA_Details, ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), Spm_Year, Sp_Sequence);
            test.FormClosed += new FormClosedEventHandler(Additional_Branch_Maintenance_Closed);
            test.StartPosition = FormStartPosition.CenterScreen;
            test.ShowDialog();
        }

        private void Additional_Branch_Maintenance_Closed(object sender, FormClosedEventArgs e)
        {
            string SelRef_Name = null;

            CASE4006_AddCAMS_Form form = sender as CASE4006_AddCAMS_Form;
            if (form.DialogResult == DialogResult.OK)
            {
                bool Rec_Found = false;
                string CA_Sequence = "1";

                Sel_Del_Count = 0;
                //string Sel_CAMS_Key = SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim() +
                //                      SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim() +
                //                      SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim() +
                //                      (SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA" ? CA_Sequence : string.Empty);
                //Fill_SP_CAMS_Details(Search_Entity.service_plan, Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString(), Sel_CAMS_Key.Trim());
                //Fill_Additional_CAMS_Details(SP_Code);

                if (Cb_Show_All_Postings.Checked)
                {
                    Clear_SP_CAMS_Grid();
                    Fill_Branch_Grid(SP_Code);
                    Cb_Show_All_Postings_CheckedChanged(Cb_Show_All_Postings, EventArgs.Empty);
                }
                else
                {
                    if (Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString() != "9")
                    {
                        Fill_Branch_Grid(SP_Code);
                        Branches_Grid_SelectionChanged(Branches_Grid, EventArgs.Empty);
                    }
                    else
                        Fill_Additional_CAMS_Details_Grid();
                }
            }
        }

        string SP_Readonly_Sw = "N"; string Sw_ReadOnly = "N";
        private void CmbSP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString()))
                _errorProvider.SetError(CmbSP, null);
            //Rao 5555
            this.Cb_Show_All_Postings.CheckedChanged -= new System.EventHandler(this.Cb_Show_All_Postings_CheckedChanged);
            CA_Auto_Details_Filled = MS_Auto_Details_Filled = Cb_Show_All_Postings.Checked = false;
            this.Cb_Show_All_Postings.CheckedChanged += new System.EventHandler(this.Cb_Show_All_Postings_CheckedChanged);
            ACR_SERV_Hies = "N";
            if (!string.IsNullOrEmpty(BaseForm.BaseAgencyControlDetails.ServicePlanHiecontrol.Trim()))
            {
                if (BaseForm.BaseAgencyControlDetails.ServicePlanHiecontrol.Trim() == "Y")
                    ACR_SERV_Hies = "S";
            }
            SP_Programs_List = _model.lookupDataAccess.Get_SerPlan_Prog_List(BaseForm.UserProfile.UserID, ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), ACR_SERV_Hies);


            if (((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).ID.ToString() == "N" && Mode.Equals("Edit"))
            {
                MessageBox.Show("Selected Service Plan is not Validated \n So you can not make postings for this SP", "CAP Systems");
                Btn_Save.Visible = SP2_SavePanel.Visible = false;
            }
            else
            {
                if (!Dep_Year_Mismatch)
                    Btn_Save.Visible = SP2_SavePanel.Visible = true;
            }



            CASESP0Entity casesp0data = propSearch_Entity.Find(u => u.Code == ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString());
            if (casesp0data != null)
            {
                if (casesp0data.Sp0ReadOnly == "Y") SP_Readonly_Sw = "Y";
            }

            if (SP_Readonly_Sw == "Y") Sw_ReadOnly = "Y"; else if (((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).ID.ToString() == "N") Sw_ReadOnly = "Y"; else Sw_ReadOnly = "N";

            if (Mode.Equals("Add"))
            {
                Fill_Branch_Grid(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString());
                buttonAllowAMDSwitch("Y");

                //List<HierarchyEntity> caseHierarchy = _model.lookupDataAccess.Get_SerPlan_Prog_List(BaseForm.UserProfile.UserID, ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString());
                //caseHierarchy = caseHierarchy.FindAll(u => u.Agency == BaseForm.BaseAgency);

                if (SP_Programs_List.Count > 0 && SP_Programs_List.Count == 1) Txt_SPM_Program.Text = Set_SP_Program_Text(SP_Programs_List[0].Code.ToString().Replace("-", "").ToString());

            }
            else
            {
                string Tmp_SP_Code = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString();
                SP_Code = "000000".Substring(0, (6 - Tmp_SP_Code.Length)) + Tmp_SP_Code;

                if (!string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).ValueDisplayCode.ToString()))
                    Spm_Year = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).ValueDisplayCode.ToString();
                else
                    Spm_Year = "    ";
                Sp_Sequence = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString();
                //Switch_To_Edit_Mode();

                if (Spm_Year != BaseForm.BaseYear)
                    Pb_Add_CA.Visible = Btn_Maintain_Add.Visible = false;
                else
                {
                    if (Privileges.AddPriv.Equals("true") && Sw_ReadOnly == "N")
                        Pb_Add_CA.Visible = Btn_Maintain_Add.Visible = true;
                }

                if (Cb_Show_All_Postings.Checked)
                {
                    Cb_Show_All_Postings_CheckedChanged(Cb_Show_All_Postings, EventArgs.Empty);
                }
                else
                {
                    Fill_SP_Controls(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString());
                    buttonAllowAMDSwitch("Y");
                }

                if (SP_Activity_Details.Count == 0 && SP_MS_Details.Count == 0)
                    Start_Date.Enabled = Start_Date_Enable_SW;
                else
                    Start_Date.Enabled = false;
            }

        }

        bool Refresh_Control = false;
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            Search_Entity.Def_Program = Search_Entity.caseworker = Search_Entity.site = Search_Entity.startdate = Search_Entity.estdate = Search_Entity.compdate = null;

            if (Mode.Equals("Add"))
            {
                Search_Entity.agency = Hierarchy.Substring(0, 2);
                Search_Entity.dept = Hierarchy.Substring(2, 2);
                Search_Entity.program = Hierarchy.Substring(4, 2);
                //Search_Entity.year = Year;                             
                //Search_Entity.year = "    ";                             // Year will be always Four-Spaces in CASESPM 
                Search_Entity.year = BaseForm.BaseYear;
                Search_Entity.app_no = App_No;
                Search_Entity.Bulk_Post = "N";

                if (!string.IsNullOrEmpty(CmbSP.Text))
                    Search_Entity.service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString();
            }

            if (!string.IsNullOrEmpty(CmbWorker.Text))
                Search_Entity.caseworker = ((Captain.Common.Utilities.ListItem)CmbWorker.SelectedItem).Value.ToString();
            if (!string.IsNullOrEmpty(CmbSite.Text))
                Search_Entity.site = ((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Value.ToString();

            //if (!string.IsNullOrEmpty(Cmb_Def_Prog.Text))
            //{
            //    if(((Captain.Common.Utilities.ListItem)Cmb_Def_Prog.SelectedItem).Value.ToString() != "0")
            //        Search_Entity.Def_Program = ((Captain.Common.Utilities.ListItem)Cmb_Def_Prog.SelectedItem).Value.ToString();
            //}

            if (!string.IsNullOrEmpty(Txt_SPM_Program.Text))
                Search_Entity.Def_Program = Txt_SPM_Program.Text.Trim().Substring(0, 6);

            if (Start_Date.Checked)
                Search_Entity.startdate = Start_Date.Value.ToShortDateString();
            if (Est_Date.Checked)
                Search_Entity.estdate = Est_Date.Value.ToShortDateString();
            if (Act_Date.Checked)
            {
                Search_Entity.compdate = Act_Date.Value.ToShortDateString();
                Search_Entity.SPM_MassClose = Search_Entity.SPM_MassClose;
            }
            else
                Search_Entity.SPM_MassClose = "N";

            Search_Entity.sel_branches = null;
            int Branch_Loop_Cnt = 0;
            foreach (DataGridViewRow dr in Branches_Grid.Rows)
            {

                dr.Cells["Branch_Status"].Value = "N";

                if (dr.Cells["Branch_Sel"].Value.ToString() == true.ToString())
                {
                    Search_Entity.sel_branches += dr.Cells["Branch_Code"].Value.ToString();
                    dr.Cells["Branch_Status"].Value = "Y";

                    if (dr.Cells["Branch_Code"].Value.ToString() == "P")
                        Branches_Grid.Rows[Branch_Loop_Cnt].DefaultCellStyle.ForeColor = System.Drawing.Color.Blue;
                    else
                        Branches_Grid.Rows[Branch_Loop_Cnt].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                }
                else
                    Branches_Grid.Rows[Branch_Loop_Cnt].DefaultCellStyle.ForeColor = System.Drawing.Color.MediumVioletRed;
                Branch_Loop_Cnt++;
            }

            Search_Entity.lstc_operator = BaseForm.UserID;

            if (Mode.Equals("Add"))
                Insert_Sel_SP_Details();
            else
                Update_Sel_SP_Details();
        }



        private void Insert_Sel_SP_Details()
        {
            if (Validate_ADD_Mode())
            {
                Search_Entity.Rec_Type = "I";
                if (_model.SPAdminData.UpdateCASESPM(Search_Entity, "Insert", out Sql_SP_Result_Message, out Tmp_SPM_Sequence))
                {
                    Update_AutoPost_CAMS(Tmp_SPM_Sequence);
                    ////MessageBox.Show("Service Plan Posting Successful", "CAP Systems");
                    SP_Code = "000000".Substring(0, (6 - Search_Entity.service_plan.Length)) + Search_Entity.service_plan;
                    Spm_Year = Search_Entity.year;
                    Sp_Sequence = Tmp_SPM_Sequence;
                    Switch_To_Edit_Mode();
                    Refresh_Control = true;
                    //this.DialogResult = DialogResult.OK;
                    //this.Close();
                }
                else
                    MessageBox.Show("Exception : " + Sql_SP_Result_Message, "CAP Systems");
            }
        }

        private void Update_AutoPost_CAMS(string New_Spm_Seq)
        {
            if (CA_Auto_Details_Filled || MS_Auto_Details_Filled)
            {
                int New_CAID = 1, New_CA_Seq = 1;
                string Ser_Plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString();
                SP_CAMS_Details = _model.SPAdminData.Browse_CASESP2(Ser_Plan, null, null, null);

                SP_CAMS_Details = SP_CAMS_Details.FindAll(u => u.SP2_CAMS_Active == "A"); //ADDED BY SUDHEER ON 10/03/2020 AS PER OCO DECUMENT

                CA_Auto_Details.Rec_Type = MS_Auto_Details.Rec_Type = "I";
                CA_Auto_Details.Agency = MS_Auto_Details.Agency = BaseForm.BaseAgency;
                CA_Auto_Details.Dept = MS_Auto_Details.Dept = BaseForm.BaseDept;
                CA_Auto_Details.Program = MS_Auto_Details.Program = BaseForm.BaseProg;
                CA_Auto_Details.Year = MS_Auto_Details.Year = BaseForm.BaseYear;
                CA_Auto_Details.App_no = MS_Auto_Details.App_no = BaseForm.BaseApplicationNo;

                CA_Auto_Details.Service_plan = MS_Auto_Details.Service_plan = Ser_Plan;
                CA_Auto_Details.SPM_Seq = MS_Auto_Details.SPM_Seq = New_Spm_Seq;
                foreach (CASESP2Entity Ent in SP_CAMS_Details)
                {
                    if (Ent.SP2_Auto_Post == "Y")
                    {
                        if (Ent.Type1 == "CA")
                        {
                            CA_Auto_Details.ACT_Code = Ent.CamCd;
                            CA_Auto_Details.Branch = Ent.Branch;
                            CA_Auto_Details.Group = Ent.Orig_Grp.ToString();
                            CA_Auto_Details.Curr_Grp = Ent.Curr_Grp.ToString();
                            _model.SPAdminData.UpdateCASEACT(CA_Auto_Details, "Insert", out New_CAID, out New_CA_Seq, out Sql_SP_Result_Message);
                        }

                        if (Ent.Type1 == "MS")
                        {
                            MS_Auto_Details.ID = "0";
                            MS_Auto_Details.MS_Code = Ent.CamCd;
                            MS_Auto_Details.Branch = Ent.Branch;
                            MS_Auto_Details.Group = Ent.Orig_Grp.ToString();
                            MS_Auto_Details.Curr_Grp = Ent.Curr_Grp.ToString();

                            _model.SPAdminData.UpdateCASEMS(MS_Auto_Details, "Insert", out New_CAID, out Sql_SP_Result_Message);
                            Update_MSOBO_Benefitig_Members(New_CAID);
                        }
                    }
                }

                if (Mode.Equals("Edit"))
                {
                    CA_Auto_Details_Filled = false; MS_Auto_Details_Filled = false;
                }

            }
        }

        private void Update_MSOBO_Benefitig_Members(int MSID)
        {
            CASEMSOBOEntity Save_Ent = new CASEMSOBOEntity();
            foreach (CASEMSOBOEntity Ent in OBO_Auto_List)
            {
                Save_Ent.Rec_Type = "I";
                Save_Ent.ID = MSID.ToString();
                Save_Ent.CLID = Ent.CLID;
                Save_Ent.Fam_Seq = Ent.Fam_Seq;
                Save_Ent.Seq = "1";

                _model.SPAdminData.UpdateCASEMSOBO(Save_Ent, "Insert", out Sql_SP_Result_Message);
            }
        }

        private void Update_Sel_SP_Details()
        {
            if (Validate_Edit_Mode())
            {
                Search_Entity.Rec_Type = "U";
                if (_model.SPAdminData.UpdateCASESPM(Search_Entity, "Update", out Sql_SP_Result_Message, out Tmp_SPM_Sequence))
                {
                    if (SP2_SavePanel.Visible == false) Update_QuickPost_CAMS(Tmp_SPM_Sequence);
                    Update_AutoPost_CAMS(Tmp_SPM_Sequence);
                    Refresh_Control = true;
                    //this.DialogResult = DialogResult.OK;
                    MessageBox.Show("Service Plan Updated Successfully", "CAP Systems");
                    Fill_Applicant_SPs();
                    Fill_SP_DropDowns();
                    Fill_SP_Controls((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString()));
                }
                else
                    MessageBox.Show("Exception : " + Sql_SP_Result_Message, "CAP Systems");
            }
        }

        private void Save_QuickPost(DialogResult dialogResult)
        {


            if (dialogResult == DialogResult.Yes)
            {
                Update_QuickPost_CAMS(Tmp_SPM_Sequence);

                Get_App_CASEACT_List();
                Get_App_CASEMS_List();

                Fill_SP_CAMS_Details(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), "P", null);
            }
        }


        bool Future_Date_Soft_Edit = false;
        private bool Validate_ADD_Mode()
        {
            bool isValid = true;

            if ((!CA_Auto_Details_Filled && Btn_AutoPost_CA.Visible) || (!MS_Auto_Details_Filled && Btn_AutoPost_MS.Visible))
            {
                string Error_Msg = "";

                if ((!CA_Auto_Details_Filled && Btn_AutoPost_CA.Visible) && (!MS_Auto_Details_Filled && Btn_AutoPost_MS.Visible))
                    Error_Msg = "Both CA and MS Auto Post Details ";
                else if ((!CA_Auto_Details_Filled && Btn_AutoPost_CA.Visible) && !Btn_AutoPost_MS.Visible)
                    Error_Msg = "CA Auto Post Details ";
                else if (Btn_AutoPost_CA.Visible && (!MS_Auto_Details_Filled && Btn_AutoPost_MS.Visible))
                    Error_Msg = "MS Auto Post Details ";

                if (Btn_AutoPost_MS.Visible)
                    _errorProvider.SetError(Btn_AutoPost_MS, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Error_Msg.Replace(Consts.Common.Colon, string.Empty)));
                else
                    _errorProvider.SetError(Btn_AutoPost_CA, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Error_Msg.Replace(Consts.Common.Colon, string.Empty)));

                isValid = false;
            }
            else
            {
                if (Btn_AutoPost_MS.Visible)
                    _errorProvider.SetError(Btn_AutoPost_MS, null);
                else
                    _errorProvider.SetError(Btn_AutoPost_CA, null);
            }



            if (Lbl_SP_Req.Visible && (CmbSP.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Text.Trim()))))
            {
                _errorProvider.SetError(CmbSP, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Service Plan".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(CmbSP, null);


            //if (String.IsNullOrEmpty(CmbSP.Text)) //((ListItem)CmbSP.SelectedItem).Value.ToString())
            //{
            //    _errorProvider.SetError(CmbSP, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Service Plan".Replace(Consts.Common.Colon, string.Empty)));
            //    isValid = false;
            //}
            //else
            //    _errorProvider.SetError(CmbSP, null);

            //Added bu Sudheer on 03/01/2021
            if (Lbl_StartDate_Req.Visible)
            {
                if (!(BaseForm.UserProfile.Security == "P" || BaseForm.UserProfile.Security == "B"))
                {
                    if (SERVStopEntity != null && Start_Date.Checked)
                    {
                        if (Convert.ToDateTime(SERVStopEntity.TDate.Trim()) >= Start_Date.Value && Convert.ToDateTime(SERVStopEntity.FDate.Trim()) <= Start_Date.Value)
                        {
                            _errorProvider.SetError(Start_Date, string.Format(" " + Lbl_StartDate.Text + " Should not be between " + LookupDataAccess.Getdate(SERVStopEntity.FDate.Trim()) + " and " + LookupDataAccess.Getdate(SERVStopEntity.TDate.Trim()).Replace(Consts.Common.Colon, string.Empty)));
                            isValid = false;
                        }

                    }
                }
                else
                {
                    if (Lbl_StartDate_Req.Visible && (!Start_Date.Checked))
                    {
                        _errorProvider.SetError(Start_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Lbl_StartDate.Text.Replace(Consts.Common.Colon, string.Empty)));
                        isValid = false;
                    }
                    else
                    {
                        bool Future_Date_Flg = false;
                        if (Start_Date.Value > DateTime.Today && !Future_Date_Soft_Edit)
                        {
                            //MessageBox.Show("' " + lblActivityDate.Text + "' Should not be Future Date", "CAPSYSTEMS");

                            MessageBox.Show("You are about to post a future date for 'Service Plan Start Date'. \n Do you want to proceed?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, 
                                onclose:Allow_Post_Future_Date);

                            //_errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Future Date".Replace(Consts.Common.Colon, string.Empty)));
                            isValid = false;
                            Future_Date_Flg = true;
                        }
                        else
                        {
                            Future_Date_Soft_Edit = false;
                            _errorProvider.SetError(Act_Date, null);
                        }



                        //_errorProvider.SetError(Start_Date, null);
                    }
                }
            }

            //if (Lbl_StartDate_Req.Visible && (!Start_Date.Checked))
            //{
            //    _errorProvider.SetError(Start_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Lbl_StartDate.Text.Replace(Consts.Common.Colon, string.Empty)));
            //    isValid = false;
            //}
            else
            {
                bool Future_Date_Flg = false;
                if (Start_Date.Value > DateTime.Today && !Future_Date_Soft_Edit)
                {
                    //MessageBox.Show("' " + lblActivityDate.Text + "' Should not be Future Date", "CAPSYSTEMS");

                    MessageBox.Show("You are about to post a future date for 'Service Plan Start Date'. \n Do you want to proceed?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, 
                        onclose:Allow_Post_Future_Date);

                    //_errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Future Date".Replace(Consts.Common.Colon, string.Empty)));
                    isValid = false;
                    Future_Date_Flg = true;
                }
                else
                {
                    Future_Date_Soft_Edit = false;
                    _errorProvider.SetError(Act_Date, null);
                }



                //_errorProvider.SetError(Start_Date, null);
            }

            if (Lbl_Est_CompleteDate_Req.Visible && (!Est_Date.Checked))
            {
                _errorProvider.SetError(Est_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Lbl_Est_CompleteDate.Text.Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(Est_Date, null);

            if (Lbl_Actual_CompleteDate_Req.Visible && (!Act_Date.Checked))
            {
                _errorProvider.SetError(Act_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Lbl_Actual_CompleteDate.Text.Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(Act_Date, null);


            //if (!Start_Date.Checked)
            //{
            //    _errorProvider.SetError(Start_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Start Date".Replace(Consts.Common.Colon, string.Empty)));
            //    isValid = false;
            //}
            //else
            //    _errorProvider.SetError(Start_Date, null);


            if (Lbl_Site_Req.Visible && (CmbSite.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Text.Trim()))))
            {
                _errorProvider.SetError(CmbSite, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Site".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(CmbSite, null);

            if (Lbl_CaseWorker_Req.Visible && (CmbWorker.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbWorker.SelectedItem).Text.Trim()))))
            {
                _errorProvider.SetError(CmbWorker, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Case Worker".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(CmbWorker, null);

            //if (Lbl_Program_Req.Visible && (Cmb_Def_Prog.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Text.Trim()))))
            //{
            //    _errorProvider.SetError(Cmb_Def_Prog, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Program".Replace(Consts.Common.Colon, string.Empty)));
            //    isValid = false;
            //}
            //else
            //    _errorProvider.SetError(Cmb_Def_Prog, null);

            if (Lbl_Program_Req.Visible && ((string.IsNullOrEmpty(Txt_SPM_Program.Text.Trim()))))
            {
                _errorProvider.SetError(panel8, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Program".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(panel8, null);


            if (this.SP_Branches.HeaderText.Contains("*"))
            {
                bool Branch_Selected = false;
                foreach (DataGridViewRow dr in Branches_Grid.Rows)
                {
                    if (dr.Cells["Branch_Sel"].Value.ToString() == true.ToString())
                    {
                        Branch_Selected = true; break;
                    }
                }

                if (!Branch_Selected)
                {
                    _errorProvider.SetError(Branches_Grid, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Branches".Replace(Consts.Common.Colon, string.Empty)));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(Branches_Grid, null);
            }
            else
                _errorProvider.SetError(Branches_Grid, null);


            if (Est_Date.Checked && Est_Date.Value < Start_Date.Value)
            {
                _errorProvider.SetError(Est_Date, string.Format("' " + Lbl_Est_CompleteDate.Text + "' Should not be Prior to 'Start Date'".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(Est_Date, null);

            if (Act_Date.Checked && Act_Date.Value < Start_Date.Value)
            {
                _errorProvider.SetError(Act_Date, string.Format("' " + Lbl_Actual_CompleteDate.Text + "' Should not be Prior to 'Start Date'".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(Act_Date, null);



            return isValid;
        }

        private void Allow_Post_Future_Date(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                Future_Date_Soft_Edit = true;
                //if (CAMS_FLG == "CA")
                Btn_Save_Click(Btn_Save, EventArgs.Empty);
                //else
                //    Btn_MS_Save_Click(Btn_MS_Save, EventArgs.Empty);
            }
        }

        private bool Validate_Edit_Mode()
        {
            bool isValid = true;

            _errorProvider.SetError(Act_Date, null);
            if (Lbl_StartDate_Req.Visible && (!Start_Date.Checked))
            {
                _errorProvider.SetError(Start_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Lbl_StartDate.Text.Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
            {

                bool Future_Date_Flg = false;
                if (Start_Date.Value > DateTime.Today && !Future_Date_Soft_Edit)
                {
                    //MessageBox.Show("' " + lblActivityDate.Text + "' Should not be Future Date", "CAPSYSTEMS");

                    MessageBox.Show("You are about to post a future date for 'Service Plan Start Date'. \n Do you want to proceed?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,onclose:Allow_Post_Future_Date);

                    //_errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Future Date".Replace(Consts.Common.Colon, string.Empty)));
                    isValid = false;
                    Future_Date_Flg = true;
                }
                else
                {
                    Future_Date_Soft_Edit = false;
                    _errorProvider.SetError(Act_Date, null);
                }

                //_errorProvider.SetError(Start_Date, null);
            }

            if (Lbl_Est_CompleteDate_Req.Visible && (!Est_Date.Checked))
            {
                _errorProvider.SetError(Est_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Lbl_Est_CompleteDate.Text.Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(Est_Date, null);

            if (Lbl_Actual_CompleteDate_Req.Visible && (!Act_Date.Checked))
            {
                _errorProvider.SetError(Act_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Lbl_Actual_CompleteDate.Text.Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(Act_Date, null);




            if (Lbl_Site_Req.Visible && (CmbSite.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Text.Trim()))))
            {
                _errorProvider.SetError(CmbSite, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Site".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(CmbSite, null);

            if (Lbl_CaseWorker_Req.Visible && (CmbWorker.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbWorker.SelectedItem).Text.Trim()))))
            {
                _errorProvider.SetError(CmbWorker, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Case Worker".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(CmbWorker, null);

            //if (Lbl_Program_Req.Visible && (Cmb_Def_Prog.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Text.Trim()))))
            //{
            //    _errorProvider.SetError(Cmb_Def_Prog, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Program".Replace(Consts.Common.Colon, string.Empty)));
            //    isValid = false;
            //}
            //else
            //    _errorProvider.SetError(Cmb_Def_Prog, null);

            if (Lbl_Program_Req.Visible && (string.IsNullOrEmpty(Txt_SPM_Program.Text.Trim())))
            {
                _errorProvider.SetError(panel8, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Program".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(panel8, null);


            if (this.SP_Branches.HeaderText.Contains("*"))
            {
                bool Branch_Selected = false;
                foreach (DataGridViewRow dr in Branches_Grid.Rows)
                {
                    if (dr.Cells["Branch_Sel"].Value.ToString() == true.ToString())
                    {
                        Branch_Selected = true; break;
                    }
                }

                if (!Branch_Selected)
                {
                    _errorProvider.SetError(Branches_Grid, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Branches".Replace(Consts.Common.Colon, string.Empty)));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(Branches_Grid, null);
            }
            else
                _errorProvider.SetError(Branches_Grid, null);

            //if (String.IsNullOrEmpty(CmbSP.Text)) //((ListItem)CmbSP.SelectedItem).Value.ToString())
            //{
            //    _errorProvider.SetError(CmbSP, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Service Plan".Replace(Consts.Common.Colon, string.Empty)));
            //    isValid = false;
            //}
            //else
            //    _errorProvider.SetError(CmbSP, null);

            //if (!Start_Date.Checked)
            //{
            //    _errorProvider.SetError(Start_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Start Date".Replace(Consts.Common.Colon, string.Empty)));
            //    isValid = false;
            //}
            //else
            //    _errorProvider.SetError(Start_Date, null);

            if (Est_Date.Checked && Est_Date.Value < Start_Date.Value)
            {
                _errorProvider.SetError(Est_Date, string.Format("' " + Lbl_Est_CompleteDate.Text + "' Should not be Prior to 'Start Date'".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(Est_Date, null);

            if (Act_Date.Checked && Act_Date.Value < Start_Date.Value)
            {
                _errorProvider.SetError(Act_Date, string.Format("' " + Lbl_Actual_CompleteDate.Text + "' Should not be Prior to 'Start Date'".Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
                _errorProvider.SetError(Act_Date, null);


            if (Act_Date.Checked)
            {
                DataSet ds = SPAdminDB.Get_SpmCompletDt(Search_Entity.agency, Search_Entity.dept, Search_Entity.program, Search_Entity.year, Search_Entity.app_no, Search_Entity.service_plan, Search_Entity.Seq, string.Empty);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["completedt"].ToString()))
                    {
                        if (Act_Date.Value < (Convert.ToDateTime(ds.Tables[0].Rows[0]["completedt"].ToString())))
                        {
                            _errorProvider.SetError(Act_Date, string.Format("' " + Lbl_Actual_CompleteDate.Text + "' Should not be Prior to 'CA/MS Comp.Date'".Replace(Consts.Common.Colon, string.Empty)));
                            isValid = false;
                        }
                    }
                }
            }

            //if (String.IsNullOrEmpty(Txtdesc.Text.Trim()))
            //{
            //    _errorProvider.SetError(Txtdesc, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Service Plan Description".Replace(Consts.Common.Colon, string.Empty)));
            //    isValid = false;
            //}
            //else
            //    _errorProvider.SetError(Txtdesc, null);

            //if ((Mode.Equals("Add") && BranchGrid.Rows[0].Cells["Row_status"].Value.ToString() != "A") ||
            //   (Mode.Equals("Edit") && BranchGrid.Rows[0].Cells["Row_status"].Value.ToString() == "D"))
            //{
            //    _errorProvider.SetError(BranchGrid, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Primary Branch".Replace(Consts.Common.Colon, string.Empty)));
            //    isValid = false;
            //}
            //else
            //    _errorProvider.SetError(BranchGrid, null);

            return isValid;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (Refresh_Control)
                this.DialogResult = DialogResult.OK;
            this.Close();
        }


        public void Prepare_Search_Entity(string Operation_Type, string CAMS_Type, string Record_Status, string CAMS_Code, string Posting_Mode, string Group, string Branch, string Seq, string MS_Post_Date)
        {
            bool is_Additional_Branch = (Operation_Type == "Normal" ? false : true);
            if (CAMS_Type == "CA")
            {
                CA_Pass_Entity.Rec_Type = "I";
                CA_Pass_Entity.Agency = Search_Entity.agency = Hierarchy.Substring(0, 2);
                CA_Pass_Entity.Dept = Search_Entity.dept = Hierarchy.Substring(2, 2);
                CA_Pass_Entity.Program = Search_Entity.program = Hierarchy.Substring(4, 2);
                CA_Pass_Entity.Year = Search_Entity.year = Spm_Year; // Year;        
                //CA_Pass_Entity.Year = Search_Entity.year = "    ";        // Year will be always Four-Spaces in CASEACT 
                CA_Pass_Entity.App_no = Search_Entity.app_no = App_No;

                CA_Pass_Entity.Service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString();
                CA_Pass_Entity.SPM_Seq = "1";
                CA_Pass_Entity.ACT_Code = CA_Pass_Entity.Branch = CA_Pass_Entity.Group = null;
                //CA_Pass_Entity.Group = (is_Additional_Branch ? ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Group"].Value.ToString().Trim() : SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim());

                if (CAMS_Code != "Auto")
                {
                    CA_Pass_Entity.SPM_Seq = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString();

                    CA_Pass_Entity.ACT_Code = CAMS_Code; //SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim();

                    CA_Pass_Entity.Branch = SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Branch"].Value.ToString().Trim();
                    //CA_Pass_Entity.Group = (is_Additional_Branch ? ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Group"].Value.ToString().Trim() : SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim());
                    CA_Pass_Entity.Group = SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim();
                }
                //CA_Pass_Entity.Type = "CA";
                CA_Pass_Entity.ACT_Seq = "1";

                CA_Pass_Entity.ACT_Date = CA_Pass_Entity.Site = CA_Pass_Entity.Fund1 = null;
                CA_Pass_Entity.Fund2 = CA_Pass_Entity.Fund3 = CA_Pass_Entity.Caseworker = CA_Pass_Entity.Vendor_No = null;
                CA_Pass_Entity.Check_Date = CA_Pass_Entity.Check_No = CA_Pass_Entity.Cost = CA_Pass_Entity.Followup_On = null;
                CA_Pass_Entity.Followup_Comp = CA_Pass_Entity.Followup_By = CA_Pass_Entity.Refer_Data = CA_Pass_Entity.Cust_Code1 = null;
                CA_Pass_Entity.Cust_Value1 = CA_Pass_Entity.Cust_Code2 = CA_Pass_Entity.Cust_Value2 = CA_Pass_Entity.Cust_Code3 = null;
                CA_Pass_Entity.Cust_Value3 = CA_Pass_Entity.Lstc_Date = CA_Pass_Entity.Lsct_Operator = CA_Pass_Entity.Add_Date = null;
                CA_Pass_Entity.Add_Operator = CA_Pass_Entity.UOM = CA_Pass_Entity.Units =
                CA_Pass_Entity.Cust_Code4 = CA_Pass_Entity.Cust_Value4 = CA_Pass_Entity.Cust_Code5 = CA_Pass_Entity.Cust_Value5 = null;
                CA_Pass_Entity.Curr_Grp = SP_CAMS_Grid.CurrentRow.Cells["SP2_Curr_Grp"].Value.ToString().Trim();


                //CASEACTEntity New_CA_Pass_Entity = new CASEACTEntity();
                if (Record_Status == "C" && (Posting_Mode.Equals("Edit") || Posting_Mode.Equals("Delete")))
                {
                    foreach (CASEACTEntity ActEnt in (true ? SP_Activity_Details : SP_Activity_Details))
                    {
                        if (((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString() == ActEnt.Service_plan &&
                            ActEnt.ACT_Code.Trim() == CAMS_Code.Trim() &&
                            ActEnt.Group.Trim() == Group &&
                            ActEnt.Branch.Trim() == Branch &&
                            ActEnt.ACT_Seq.Trim() == Seq)
                        {
                            CA_Pass_Entity = new CASEACTEntity(ActEnt);
                            break;
                        }
                    }
                }
            }
            else
            {

                MS_Pass_Entity.Rec_Type = "I";
                MS_Pass_Entity.Agency = Hierarchy.Substring(0, 2);
                MS_Pass_Entity.Dept = Hierarchy.Substring(2, 2);
                MS_Pass_Entity.Program = Hierarchy.Substring(4, 2);
                MS_Pass_Entity.Year = Spm_Year; //Year;                          
                //MS_Pass_Entity.Year = "    ";                          // Year will be always Four-Spaces in CASEMS 
                MS_Pass_Entity.App_no = App_No;

                MS_Pass_Entity.ID = "0";
                MS_Pass_Entity.Service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString();

                MS_Pass_Entity.SPM_Seq = "1";
                MS_Pass_Entity.MS_Code = MS_Pass_Entity.Branch = MS_Pass_Entity.Group = null;
                if (CAMS_Code != "Auto")
                {
                    MS_Pass_Entity.SPM_Seq = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString();
                    //MS_Pass_Entity.MS_Code = (is_Additional_Branch ? ADD_CAMS_Grid.CurrentRow.Cells["SPM2_CAMS_Code"].Value.ToString().Trim() : SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim());
                    //MS_Pass_Entity.Branch = (is_Additional_Branch ? "9" : SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch"].Value.ToString().Trim());
                    //MS_Pass_Entity.Group = (is_Additional_Branch ? ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Group"].Value.ToString().Trim() : SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim());

                    MS_Pass_Entity.MS_Code = SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim();
                    MS_Pass_Entity.Branch = SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Branch"].Value.ToString().Trim();  //(Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString() == "9" ? "9" : SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch"].Value.ToString().Trim()); // SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch"].Value.ToString().Trim();
                    MS_Pass_Entity.Group = SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim();
                }

                MS_Pass_Entity.Date = MS_Pass_Entity.CaseWorker = MS_Pass_Entity.Site = null;
                MS_Pass_Entity.Result = MS_Pass_Entity.OBF = MS_Pass_Entity.Add_Operator = null;
                MS_Pass_Entity.Lstc_Date = MS_Pass_Entity.Lsct_Operator = MS_Pass_Entity.Add_Date = null;

                MS_Pass_Entity.Curr_Grp = SP_CAMS_Grid.CurrentRow.Cells["SP2_Curr_Grp"].Value.ToString().Trim();

                //MS_Pass_Entity.Date = SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value.ToString().Trim();
                string MS_Date = MS_Post_Date, Date_To_Compare = "    ";

                if (Record_Status == "C" && (Posting_Mode.Equals("Edit") || Posting_Mode.Equals("Delete")))//CAMA_Post_Mode == "Edit")
                {
                    foreach (CASEMSEntity MSEnt in SP_MS_Details)
                    {
                        if (!string.IsNullOrEmpty(MSEnt.Date.Trim()))
                            Date_To_Compare = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(MSEnt.Date.Trim()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                        if (((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString() == MSEnt.Service_plan &&
                            MSEnt.MS_Code.Trim() == CAMS_Code.Trim() &&
                            MSEnt.Group.Trim() == Group &&
                            MSEnt.Branch.Trim() == Branch &&
                            Date_To_Compare.Trim() == MS_Date.Trim())
                        {
                            MS_Pass_Entity = new CASEMSEntity(MSEnt);
                            break;
                        }
                    }
                }
            }
        }

        CASEACTEntity CA_Pass_Entity = new CASEACTEntity();
        CASEMSEntity MS_Pass_Entity = new CASEMSEntity();
        private void SP_CAMS_Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (SP_CAMS_Grid.Rows.Count > 0 && e.RowIndex != -1)
            {
                int ColIdx = SP_CAMS_Grid.CurrentCell.ColumnIndex;
                int RowIdx = SP_CAMS_Grid.CurrentCell.RowIndex;

                switch (e.ColumnIndex)
                {
                    case 0:
                        if (SP2_SavePanel.Visible == false) { Select_All_To_QuickPost(); }
                        else if (!Del_Col_Header_Clicked)
                            Select_All_To_Delete();
                        break;
                        //Commented by Sudheer on 10/29/2021    
                        //case 4: Add_Edit_CAMS_Details("Add");
                        //    break;
                }

                string Sw_ReadOnly = "N";
                if (SP_Readonly_Sw == "Y") Sw_ReadOnly = "Y"; else if (((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).ID.ToString() == "N") Sw_ReadOnly = "Y";

                if (Privileges.AddPriv.Equals("true") && Sw_ReadOnly == "N")
                    Pb_Add_CA.Visible = true;
                else Pb_Add_CA.Visible = false;

                //if ((SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA" || SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "A") &&
                //     Privileges.AddPriv.Equals("true") && Spm_Year == BaseForm.BaseYear)
                //     Pb_Add_CA.Visible = true;
                //else
                //     Pb_Add_CA.Visible = false;   // Commented to Allow Multiple Postings For MS

                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C" && Privileges.ChangePriv.Equals("true"))
                    Pb_Edit.Visible = true;
                else
                    Pb_Edit.Visible = false;


                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C") // && Privileges.ChangePriv.Equals("true")) Commented on 08212014
                {
                    if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_SW"].Value.ToString() == "Y")
                        PB_SP2_Notes.ImageSource = "captain-casenotes";
                    else
                        PB_SP2_Notes.ImageSource = "captain-casenotesadd"; ;

                    PB_SP2_Notes.Visible = true;
                }
                else
                    PB_SP2_Notes.Visible = false;

                buttonAllowAMDSwitch("Y");
                //if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C")
                //    Pb_Edit.Visible = true;
                //else
                //    Pb_Edit.Visible = false;
            }
        }


        private void Add_Edit_CAMS_Details(string Posting_Mode)  // sindhe
        {
            Prepare_Search_Entity("Normal",
                                  SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString(),
                                  SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString(),
                                  SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString(),
                                  Posting_Mode,
                                  SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim(),
                                  SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Branch"].Value.ToString().Trim(), //SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Branch"].Value.ToString(),  == "9" ? "9" : SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch"].Value.ToString().Trim()),
                                  SP_CAMS_Grid.CurrentRow.Cells["SP2_CA_Seq"].Value.ToString().Trim(),
                                  SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value.ToString().Trim());

            string[] Split_Str = Regex.Split(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Text.ToString(), "-");
            string SP_Date = (!string.IsNullOrEmpty(Split_Str[1].Trim()) ? Split_Str[1] : "");
            string SPM_Site = string.Empty;
            if (!string.IsNullOrEmpty(CmbSite.Text))
                SPM_Site = ((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Value.ToString();

            string strEndDate = string.Empty;

            if (Act_Date.Checked == true)
            {
                strEndDate = Act_Date.Value.ToShortDateString();
            }

            if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA")
            {
                CASE2006_CAMSForm PostCA_Form;
                PostCA_Form = new CASE2006_CAMSForm(BaseForm, "CA", SP_CAMS_Grid.CurrentRow.Cells["SP2_Dup_Desc"].Value.ToString(), Hierarchy, Year, CA_Pass_Entity, Privileges, App_MST_Entity.Site.Trim(), App_MST_Entity.IntakeWorker.Trim(), CntlCAEntity, SP_Header_Rec, Act_Template_List, SP_Date, SPM_Site, strEndDate);   // 08022012
                PostCA_Form.FormClosed += new FormClosedEventHandler(Add_Edit_CAMS_Closed);
                PostCA_Form.StartPosition = FormStartPosition.CenterScreen;
                PostCA_Form.ShowDialog();
            }
            else
            {
                CASE2006_CAMSForm PostMS_Form;
                PostMS_Form = new CASE2006_CAMSForm(BaseForm, "MS", SP_CAMS_Grid.CurrentRow.Cells["SP2_Dup_Desc"].Value.ToString(), Hierarchy, Year, MS_Pass_Entity, Privileges, App_MST_Entity.Site, App_MST_Entity.IntakeWorker, CntlMSEntity, SP_Header_Rec, MS_Template_List, SP_Date, SPM_Site, strEndDate);
                PostMS_Form.FormClosed += new FormClosedEventHandler(Add_Edit_CAMS_Closed);
                PostMS_Form.StartPosition = FormStartPosition.CenterScreen;
                PostMS_Form.ShowDialog();
            }
        }

        string gbl_sel_CAMS_KEY = "";
        private void Add_Edit_CAMS_Closed(object sender, FormClosedEventArgs e)
        {
            string SelRef_Name = null;

            CASE2006_CAMSForm form = sender as CASE2006_CAMSForm;
            if (form.DialogResult == DialogResult.OK)
            {
                bool Rec_Found = false;
                string CA_Sequence = "1", Processing_Rec_Type = SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString();

                Refresh_Control = true;
                if (Processing_Rec_Type == "CA")
                {
                    ////if(CA_Pass_Entity.Rec_Type == "I")     // No need of Alert B/Z After Save we are asking for Progress Notes
                    ////    MessageBox.Show("Critical Activity Posting Inserted Successfully", "CAP Systems");
                    ////else
                    ////    MessageBox.Show("Critical Activity Posting Updated Successfully", "CAP Systems");
                    Get_App_CASEACT_List();
                    CA_Sequence = string.IsNullOrEmpty(form.Get_CA_Sequence().Trim()) ? string.Empty : form.Get_CA_Sequence().Trim();
                }
                else
                {
                    ////if (MS_Pass_Entity.Rec_Type == "I")    // No need of Alert B/Z After Save we are asking for Progress Notes
                    ////    MessageBox.Show("Milestone Posting Inserted Successfully", "CAP Systems");
                    ////else
                    ////    MessageBox.Show("Milestone Posting Updated Successfully", "CAP Systems");
                    Get_App_CASEMS_List();
                }

                Sel_Del_Count = 0;
                string Sel_CAMS_Key = SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim() +
                                      SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim() +
                                      SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim() +
                                      (SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA" ? CA_Sequence : string.Empty);
                gbl_sel_CAMS_KEY = Sel_CAMS_Key;


                //if (Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString() != "9")
                //    Fill_SP_CAMS_Details(Search_Entity.service_plan, Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString(), Sel_CAMS_Key.Trim());
                //else
                //    Fill_Additional_CAMS_Details_Grid(); // Rao

                if (Cb_Show_All_Postings.Checked)
                    Cb_Show_All_Postings_CheckedChanged(Cb_Show_All_Postings, EventArgs.Empty);
                else
                {
                    if (Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString() == "9")
                        Fill_Additional_CAMS_Details_Grid();
                    else
                        Fill_SP_CAMS_Details(Search_Entity.service_plan, Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString(), Sel_CAMS_Key.Trim());
                }


                if (SP_Activity_Details.Count == 0 && SP_MS_Details.Count == 0)
                    Start_Date.Enabled = Start_Date_Enable_SW;
                else
                    Start_Date.Enabled = false;

                //if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA")
                //{
                //    CASEACTEntity Edited_Entity = form.GetEdited_CA_Entity();

                //    foreach (CASEACTEntity ActEnt in SP_Activity_Details)
                //    {
                //        if (ActEnt.ACT_Code.Trim() == SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim())
                //        {
                //            CA_Pass_Entity = Edited_Entity;
                //            SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value = Edited_Entity.ACT_Date;
                //            SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value = Edited_Entity.Followup_On;
                //            Rec_Found = true;
                //            break;
                //        }
                //    }
                //}
                //else
                //{
                //    CASEMSEntity Edited_Entity = form.GetEdited_MS_Entity();

                //    foreach (CASEMSEntity MSEnt in SP_MS_Details)
                //    {
                //        if (MSEnt.MS_Code.Trim() == SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim())
                //        {
                //            MS_Pass_Entity = Edited_Entity;
                //            SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value = Edited_Entity.Date;
                //            SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value = " ";
                //            Rec_Found = true;
                //            break;
                //        }
                //    }
                //}

                //if (Rec_Found)
                //{
                //    SP_CAMS_Grid.CurrentRow.Cells["Del_1"].ReadOnly = false;
                //    SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value = "C";
                //}
            }
        }

        private void Add_Edit_Additional_CAMS_Closed(object sender, FormClosedEventArgs e)
        {
            string SelRef_Name = null;

            CASE2006_CAMSForm form = sender as CASE2006_CAMSForm;
            if (form.DialogResult == DialogResult.OK)
            {
                bool Rec_Found = false;
                string CA_Sequence = "1";


                if (ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Type"].Value.ToString() == "CA")
                {
                    Get_App_CASEACT_List();
                    CA_Sequence = string.IsNullOrEmpty(form.Get_CA_Sequence().Trim()) ? string.Empty : form.Get_CA_Sequence().Trim();
                }
                else
                    Get_App_CASEMS_List();

                Sel_Del_Count = 0;
                //string Sel_CAMS_Key = SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim() +
                //                      SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim() +
                //                      SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim() +
                //                      (SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA" ? CA_Sequence : string.Empty);
                //Fill_SP_CAMS_Details(Search_Entity.service_plan, Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString(), Sel_CAMS_Key.Trim());
                Fill_Additional_CAMS_Details(SP_Code);
            }
        }


        private void Select_All_To_Delete()
        {
            if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C")
            {
                if (Spm_Year != BaseForm.BaseYear)
                {
                    MessageBox.Show("Cannot be selected as this SPM is created in PY" + Spm_Year, "CAPSYSTEMS");
                }

                string Tmp = "false";
                Tmp = SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value.ToString();
                if (Tmp == "True")
                {
                    SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value = true;
                    Sel_Del_Count++;
                }
                else
                {
                    SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value = false;
                    if (Sel_Del_Count > 0)
                        Sel_Del_Count--;
                }

                //if (Sel_Del_Count > 0 && Privileges.DelPriv.Equals("true"))
                //    PbDelete.Enabled = true;
                //else
                //    PbDelete.Enabled = false;

                //if (Privileges.DelPriv.Equals("true"))
                //    PbDelete.Visible = true;
                //else
                //    PbDelete.Visible = false;

            }
            else SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value = false;

            Del_Col_Header_Clicked = false;
        }


        bool Sel_All_To_Delete = false, Del_Col_Header_Clicked = false; bool Sel_All_To_Quick = false;
        int Sel_Del_Count = 0;
        private void SP_CAMS_Grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 0 && SP_CAMS_Grid.Rows.Count > 0 && Spm_Year == BaseForm.BaseYear)
            {
                int Tmp_Loop_Cnt = 0;
                if (SP2_SavePanel.Visible == false)
                {
                    switch (Sel_All_To_Quick)
                    {
                        case true:
                            foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
                                dr.Cells["Del_1"].Value = false;

                            //PbDelete.Visible = false;  
                            Sel_All_To_Quick = false;
                            break;
                        case false:
                            foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
                            {
                                if ((dr.Cells["SP2_Operation"].Value.ToString() != "C" || dr.Cells["SP2_Comp_Date"].Value.ToString() == string.Empty) && dr.Cells["SP2_Type"].Value.ToString() == QuickPost)
                                {
                                    dr.Cells["Del_1"].Value = true;
                                }
                            }
                            Sel_All_To_Quick = true;
                            break;
                    }
                }
                else
                {
                    switch (Sel_All_To_Delete)
                    {
                        case true:
                            foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
                                dr.Cells["Del_1"].Value = false;

                            //PbDelete.Visible = false;  
                            Del_Col_Header_Clicked = Sel_All_To_Delete = false;
                            Sel_Del_Count = 0;
                            break;

                        case false:
                            foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
                            {
                                if (dr.Cells["SP2_Operation"].Value.ToString() == "C")
                                {
                                    dr.Cells["Del_1"].Value = true;
                                    Tmp_Loop_Cnt++;
                                }
                            }

                            Sel_Del_Count = Tmp_Loop_Cnt;
                            //PbDelete.Visible = true;  
                            Sel_All_To_Delete = true; //Del_Col_Header_Clicked = 
                            break;
                    }
                }
                // Del_Col_Header_Clicked = true;
            }
        }

        private void Pb_SPM2_Delete_Click(object sender, EventArgs e)
        {
            if (Sel_Del_Count < 1)
                AlertBox.Show("Please Select Atleast One CA/MS To Delete");
            else
                MessageBox.Show(Consts.Messages.AreYouSureYouWantToDelete.GetMessage() + "  Selected CA/MS", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    onclose:Delete_Selected_Additinal_CAMS);

        }

        private void Delete_Selected_Additinal_CAMS(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                bool Delete_Status = true; int CountCA = 0, CountMS = 0; bool VouchMSG = false;
                int New_MS_ID = 0, Tmp_CA_Seq = 0;
                foreach (DataGridViewRow dr in ADD_CAMS_Grid.Rows)
                {
                    if (dr.Cells["Del_2"].Value.ToString() == true.ToString())
                    {
                        Prepare_Search_Entity("Additional",
                                              dr.Cells["SPM2_Type"].Value.ToString(),
                                              dr.Cells["SPM2_Operation"].Value.ToString(),
                                              dr.Cells["SPM2_CAMS_Code"].Value.ToString(),
                                              "Delete",
                                              dr.Cells["SPM2_Group"].Value.ToString().Trim(),
                                              "9",
                                              dr.Cells["SPM2_CA_Seq"].Value.ToString().Trim(),
                                              dr.Cells["SP2_Comp_Date"].Value.ToString().Trim());

                        if (dr.Cells["SPM2_Type"].Value.ToString() == "CA")
                        {
                            CASEACTEntity Search_Enty = new CASEACTEntity(true);
                            Search_Enty.Agency = BaseForm.BaseAgency;
                            Search_Enty.Dept = BaseForm.BaseDept;
                            Search_Enty.Program = BaseForm.BaseProg;
                            Search_Enty.Year = BaseForm.BaseYear;                             // Year will be always Four-Spaces in CASEACT
                            Search_Enty.App_no = BaseForm.BaseApplicationNo;
                            Search_Enty.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013
                            Search_Enty.Service_plan = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString()));
                            Search_Enty.ACT_Code = CA_Pass_Entity.ACT_Code;
                            Search_Enty.ACT_Seq = CA_Pass_Entity.ACT_Seq;

                            List<CASEACTEntity> Sel_SP_Activity_Details = _model.SPAdminData.Browse_CASEACT(Search_Enty, "Browse");

                            if (Sel_SP_Activity_Details.Count > 0)
                            {
                                if (string.IsNullOrEmpty(Sel_SP_Activity_Details[0].VOUCHNO.Trim()) || BaseForm.UserProfile.Security == "B")
                                {
                                    CA_Pass_Entity.Rec_Type = "D";
                                    if (!_model.SPAdminData.UpdateCASEACT(CA_Pass_Entity, "Delete", out New_MS_ID, out Tmp_CA_Seq, out Sql_SP_Result_Message))
                                        Delete_Status = false;
                                    else CountCA++;
                                }
                                else { Delete_Status = false; VouchMSG = true; }
                                //else
                                //{
                                //    MessageBox.Show("This CA can't be deleted,Voucher has Generated for this CA");
                                //    Delete_Status = false;
                                //}
                            }
                        }
                        else
                        {
                            MS_Pass_Entity.Rec_Type = "D";
                            if (!_model.SPAdminData.UpdateCASEMS(MS_Pass_Entity, "Delete", out New_MS_ID, out Sql_SP_Result_Message))
                                Delete_Status = false;
                            else CountMS++;
                        }
                    }
                }

                if (Delete_Status)
                {
                    Sel_Del_Count = 0;
                    string CAMSG = CountCA == 0 ? "" : CountCA == 1 ? CountCA.ToString() + " CA" : CountCA.ToString() + " CA's";
                    string MSMSG = CountMS == 0 ? "" : CountMS == 1 ? CountMS.ToString() + " MS" : CountMS.ToString() + " MS's";
                    if (CountCA > 0 && CountMS > 0) MSMSG = " & " + MSMSG.Trim();
                    if (CountCA > 0 || CountMS > 0)
                        AlertBox.Show(CAMSG + MSMSG + "Postings Deleted Successfully");
                    //MessageBox.Show("Selected CA/MS(s) Postings Deleted Successfully", "CAP Systems");
                    Clear_SP_CAMS_Grid();
                    //SP_CAMS_Grid.Rows.Clear();
                    if (Cb_Show_All_Postings.Checked)
                        Cb_Show_All_Postings_CheckedChanged(Cb_Show_All_Postings, EventArgs.Empty);
                    else
                        Fill_SP_Controls(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString());
                }
                else if (VouchMSG)
                {
                    string CAMSG = CountCA == 0 ? "" : CountCA == 1 ? CountCA.ToString() + " CA " : CountCA.ToString() + " CA's ";
                    string MSMSG = CountMS == 0 ? "" : CountMS == 1 ? CountMS.ToString() + " MS " : CountMS.ToString() + " MS's ";
                    if (CountCA > 0 && CountMS > 0) MSMSG = " & " + MSMSG.Trim();
                    if (CountCA > 0 && CountMS > 0)
                        AlertBox.Show(CAMSG + MSMSG + "Postings Deleted Successfully");
                }
                else
                    AlertBox.Show(Sql_SP_Result_Message);
                //MessageBox.Show("Selected CA/MS(s) Postings Deleted Successfully" + Sql_SP_Result_Message, "CAP Systems");

                //PbDelete.Visible = false;  //08222012
                //PbDelete.Enabled = false;
            }
        }

        private void PbDelete_Click(object sender, EventArgs e)
        {
            if (Sel_Del_Count < 1)
                AlertBox.Show("Please Select Atleast One CA/MS To Delete");
            else
                MessageBox.Show(Consts.Messages.AreYouSureYouWantToDelete.GetMessage() + "  Selected CA/MS", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Delete_Selected_CAMS);
        }

        private void Delete_Selected_CAMS(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                bool Delete_Status = true;
                int New_MS_ID = 0, Tmp_CA_Seq = 0; bool VouchMSG = false; int CountCA = 0, CountMS = 0;
                foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
                {
                    if (dr.Cells["Del_1"].Value.ToString() == true.ToString())
                    {
                        Prepare_Search_Entity("Normal",
                                              dr.Cells["SP2_Type"].Value.ToString(),
                                              dr.Cells["SP2_Operation"].Value.ToString(),
                                              dr.Cells["SP2_CAMS_Code"].Value.ToString(),
                                              "Delete",
                                              dr.Cells["SP2_Group"].Value.ToString().Trim(),
                                              dr.Cells["SP2_CAMS_Branch"].Value.ToString().Trim(), //SP2_Branch
                                              dr.Cells["SP2_CA_Seq"].Value.ToString().Trim(),
                                              dr.Cells["SP2_Comp_Date"].Value.ToString().Trim());

                        if (dr.Cells["SP2_Type"].Value.ToString() == "CA")
                        {
                            CASEACTEntity Search_Enty = new CASEACTEntity(true);
                            Search_Enty.Agency = BaseForm.BaseAgency;
                            Search_Enty.Dept = BaseForm.BaseDept;
                            Search_Enty.Program = BaseForm.BaseProg;
                            Search_Enty.Year = BaseForm.BaseYear;                             // Year will be always Four-Spaces in CASEACT
                            Search_Enty.App_no = BaseForm.BaseApplicationNo;
                            Search_Enty.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013
                            Search_Enty.Service_plan = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString()));
                            Search_Enty.ACT_Code = CA_Pass_Entity.ACT_Code;
                            Search_Enty.ACT_Seq = CA_Pass_Entity.ACT_Seq;
                            CA_Pass_Entity.ACT_Date = LookupDataAccess.Getdate(CA_Pass_Entity.ACT_Date.Trim());
                            List<CASEACTEntity> Sel_SP_Activity_Details = _model.SPAdminData.Browse_CASEACT(Search_Enty, "Browse");

                            if (Sel_SP_Activity_Details.Count > 0)
                            {
                                if (string.IsNullOrEmpty(Sel_SP_Activity_Details[0].VOUCHNO.Trim()) || BaseForm.UserProfile.Security == "B")
                                {
                                    CA_Pass_Entity.Rec_Type = "D";
                                    if (!_model.SPAdminData.UpdateCASEACT(CA_Pass_Entity, "Delete", out New_MS_ID, out Tmp_CA_Seq, out Sql_SP_Result_Message))
                                        Delete_Status = false;
                                    else CountCA++;

                                }
                                else { Delete_Status = false; VouchMSG = true; }
                                //else
                                //{
                                //    MessageBox.Show("You can't be delete,Voucher# " + Sel_SP_Activity_Details[0].VOUCHNO.Trim() + "is already Generated for this CA");
                                //    Delete_Status = false;
                                //}
                            }



                        }
                        else
                        {
                            MS_Pass_Entity.Date = LookupDataAccess.Getdate(MS_Pass_Entity.Date.Trim());
                            MS_Pass_Entity.Rec_Type = "D";
                            if (!_model.SPAdminData.UpdateCASEMS(MS_Pass_Entity, "Delete", out New_MS_ID, out Sql_SP_Result_Message))
                                Delete_Status = false;
                            else CountMS++;
                        }
                    }
                }

                if (Delete_Status)
                {
                    Refresh_Control = true;
                    Sel_Del_Count = 0;
                    string CAMSG = CountCA == 0 ? "" : CountCA == 1 ? CountCA.ToString() + " CA" : CountCA.ToString() + " CA's";
                    string MSMSG = CountMS == 0 ? "" : CountMS == 1 ? CountMS.ToString() + " MS" : CountMS.ToString() + " MS's";
                    if (CountCA > 0 && CountMS > 0) MSMSG = " & " + MSMSG.Trim();
                    if (CountCA > 0 || CountMS > 0)
                        AlertBox.Show(CAMSG + MSMSG + "Postings Deleted Successfully");
                    //MessageBox.Show("Selected CA/MS(s) Postings Deleted Successfully", "CAP Systems");

                    Clear_SP_CAMS_Grid();
                    //SP_CAMS_Grid.Rows.Clear();
                    //Fill_SP_Controls(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString());
                    Get_App_CASEACT_List();
                    Get_App_CASEMS_List();

                    if (Cb_Show_All_Postings.Checked)
                        Cb_Show_All_Postings_CheckedChanged(Cb_Show_All_Postings, EventArgs.Empty);
                    else
                    {
                        if (Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString() != "9")
                            Fill_SP_CAMS_Details(Search_Entity.service_plan, Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString(), string.Empty);
                        else
                            Fill_Additional_CAMS_Details_Grid();
                    }
                    //Fill_SP_Controls(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString());

                }
                else if (VouchMSG)
                {
                    string CAMSG = CountCA == 0 ? "" : CountCA == 1 ? CountCA.ToString() + " CA " : CountCA.ToString() + " CA's ";
                    string MSMSG = CountMS == 0 ? "" : CountMS == 1 ? CountMS.ToString() + " MS " : CountMS.ToString() + " MS's ";
                    if (CountCA > 0 && CountMS > 0) MSMSG = " & " + MSMSG.Trim();
                    if (CountCA > 0 || CountMS > 0)
                        AlertBox.Show(CAMSG + MSMSG + "Postings Deleted Successfully");
                }
                else
                    AlertBox.Show(Sql_SP_Result_Message);
                //MessageBox.Show("Selected CA/MS(s) Postings Deleted Successfully" + Sql_SP_Result_Message, "CAP Systems");

                //PbDelete.Visible = false;  //08222012
                //PbDelete.Enabled = false;

                if (SP_Activity_Details.Count == 0 && SP_MS_Details.Count == 0)
                    Start_Date.Enabled = Start_Date_Enable_SW;
                else
                    Start_Date.Enabled = false;
            }
        }


        private void Branches_Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (Mode.Equals("Edit") && !Cb_Show_All_Postings.Checked)
            {
                Pub_SP2_Cnt = Pub_SP2_CAMS_Posting_Cnt = Pub_SPM2_Cnt = Pub_SPM2_CAMS_Posting_Cnt = 0;
                if (Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString() != "9")
                {
                    Pb_Add_CA.Visible = Pb_Edit.Visible = PbDelete.Visible = PB_SP2_Notes.Visible = false;
                    if (Branches_Grid.CurrentRow.Cells["Branch_Status"].Value.ToString() == "Y")
                        Fill_SP_CAMS_Details(Search_Entity.service_plan, Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString(), null);
                    else
                    {
                        MessageBox.Show("Selected 'Branch' is not included in SP-Posting for this Applicant \n CA/MS(s) Associations will be Displayed for 'Branches which are Posted' only", "CAP Systems");
                        Clear_SP_CAMS_Grid();
                        //SP_CAMS_Grid.Rows.Clear();
                    }
                    Sel_Del_Count = 0;
                }
                else
                {
                    if (Spm_Year != BaseForm.BaseYear)
                        PbDelete.Visible = false;
                    else
                        PbDelete.Visible = true;

                    Fill_Additional_CAMS_Details_Grid();
                }
            }

            if (SP_Activity_Details.Count == 0 && SP_MS_Details.Count == 0)
                Start_Date.Enabled = Start_Date_Enable_SW;
            else
                Start_Date.Enabled = false;
        }

        private void set_CAMS_Tooltip(int rowIndex, string Add_Date, string Add_Opr, string Lstc_Date, string Lstc_Opr)
        {
            string toolTipText = "Added By     : " + Add_Opr + " on " + Add_Date + "\n" +
                                 "Modified By  : " + Lstc_Opr + " on " + Lstc_Date;

            foreach (DataGridViewCell cell in SP_CAMS_Grid.Rows[rowIndex].Cells)
                cell.ToolTipText = toolTipText;
        }

        string CAMA_Post_Mode = "Edit";
        private void Pb_Add_Edit_Click(object sender, EventArgs e)
        {
            //string CAMA_Post_Mode = "Edit";
            CAMA_Post_Mode = "Edit";
            if (sender == Pb_Add_CA)
            {
                CAMA_Post_Mode = "Add";
                //CA_Pass_Entity.ACT_Date = CA_Pass_Entity.Site = CA_Pass_Entity.Fund1 = null;
                //CA_Pass_Entity.Fund2 = CA_Pass_Entity.Fund3 = CA_Pass_Entity.Caseworker = CA_Pass_Entity.Vendor_No = null;
                //CA_Pass_Entity.Check_Date = CA_Pass_Entity.Check_No = CA_Pass_Entity.Cost = CA_Pass_Entity.Followup_On = null;
                //CA_Pass_Entity.Followup_Comp = CA_Pass_Entity.Followup_By = CA_Pass_Entity.Refer_Data = CA_Pass_Entity.Cust_Code1 = null;
                //CA_Pass_Entity.Cust_Value1 = CA_Pass_Entity.Cust_Code2 = CA_Pass_Entity.Cust_Value2 = CA_Pass_Entity.Cust_Code3 = null;
                //CA_Pass_Entity.Cust_Value3 = CA_Pass_Entity.Lstc_Date = CA_Pass_Entity.Lsct_Operator = CA_Pass_Entity.Add_Date = null;
                //CA_Pass_Entity.Add_Operator = null;
            }

            if (SP_CAMS_Grid.Rows.Count > 0)
            {
                string Service_Type = "'Critical Activity'";
                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "MS")
                    Service_Type = "'Milestone'";

                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Active_Stat"].Value.ToString() == "False" && (!string.IsNullOrEmpty(SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim())))
                {
                    //if
                    MessageBox.Show("You are Not suppose to Post Inactive " + Service_Type, "CAP Systems");
                }
                //MessageBox.Show("You are suppose to Post Inactive " + Service_Type + " \n  Are you sure you want to post selected " + Service_Type, Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, Inactive_CAMS_Posting_Confirmation, true);
                else
                {
                    //if (Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString() != "9")
                    Add_Edit_CAMS_Details(CAMA_Post_Mode);
                    //else
                    //    Add_Edit_Additional_CAMS_Details(CAMA_Post_Mode);
                }
            }
        }

        private void Inactive_CAMS_Posting_Confirmation(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
                Add_Edit_CAMS_Details(CAMA_Post_Mode);
        }

        private void Start_Date_CheckedChanged(object sender, EventArgs e)
        {
            if (Start_Date.Checked)
                _errorProvider.SetError(DatePanel, null);
        }

        private void PB_SP2_Notes_Click(object sender, EventArgs e)
        {
            //ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, "Add", Privileges, Sel_CAMS_Notes_Key); //SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_Key"].Value.ToString());
            ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, "Edit", Privileges, (SPM2_Panel.Visible ? Sel_SPM2_Notes_Key : Sel_CAMS_Notes_Key)); //SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_Key"].Value.ToString());
            Prog_Form.FormClosed += new FormClosedEventHandler(On_PROGNOTES_Closed);
            Prog_Form.StartPosition = FormStartPosition.CenterScreen;
            Prog_Form.ShowDialog();

        }

        private void On_PROGNOTES_Closed(object sender, FormClosedEventArgs e)
        {
            ProgressNotes_Form form = sender as ProgressNotes_Form;
            if (form.DialogResult == DialogResult.OK)
            {
                //PB_SP2_Notes.ImageSource = "captain-casenotes";
                SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_SW"].Value = "Y";
                PB_SP2_Notes.ImageSource = "captain-casenotes";
            }
        }

        CaseMstEntity App_MST_Entity = new CaseMstEntity();
        private void Get_App_MST_Details()
        {
            App_MST_Entity = _model.CaseMstData.GetCaseMST(Hierarchy.Substring(0, 2), Hierarchy.Substring(2, 2), Hierarchy.Substring(4, 2), BaseForm.BaseYear, App_No);
            //App_MST_Entity = _model.CaseMstData.GetCaseMST(Hierarchy.Substring(0, 2), Hierarchy.Substring(2, 2), Hierarchy.Substring(4, 2), Year, App_No);
        }

        public string[] GetSelected_SP_Code()
        {
            string[] Added_Edited_SPCode = new string[2];

            if (!(string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString())))
            {
                Added_Edited_SPCode[0] = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString();
                Added_Edited_SPCode[1] = Mode;
            }

            return Added_Edited_SPCode;
        }

        private void Branches_Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Branches_Grid.Rows.Count > 0)
            {
                int ColIdx = Branches_Grid.CurrentCell.ColumnIndex;
                int RowIdx = Branches_Grid.CurrentCell.RowIndex;

                if (e.ColumnIndex == 0)
                {
                    if (Branches_Grid.CurrentRow.Cells["Branch_Sel"].Value.ToString() == false.ToString())
                    {
                        if (Privileges.DelPriv.Equals("true"))
                        {
                            if (Branches_Grid.CurrentRow.Cells["Branch_Status"].Value.ToString() == "Y")
                            {
                                if (Get_Postings_For_Sel_Branch())
                                {
                                    Branches_Grid.CurrentRow.Cells["Branch_Sel"].Value = true;
                                    //MessageBox.Show("You can not Delete selected Branch  \n CA/MS Postings Exists for selected Branch", "CAP Systems");
                                    MessageBox.Show("The Selected Branch cannot be unchecked as there are CA and MS records posted to it…  \n please remove the CA and MS records first and you can then uncheck the Branch", "CAP Systems");

                                }
                                //else // brain asked to comment this on 07/27/2014
                                //    MessageBox.Show(Consts.Messages.AreYouSureYouWantToDelete.GetMessage() + "  this Branch \n CA/MS(S) Associations display will be restricted if you Uncheck the branch", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, UnCheck_Selected_Branch, true); 

                                //Branches_Grid.CurrentRow.Cells["Branch_Sel"].Value = false;
                            }
                        }
                        else
                        {
                            if (Branches_Grid.CurrentRow.Cells["Branch_Status"].Value.ToString() == "Y")
                            {
                                Branches_Grid.CurrentRow.Cells["Branch_Sel"].Value = true;
                                MessageBox.Show("You do not have privilige to Delete selected Branch", "CAP Systems");
                            }
                            else
                                Branches_Grid.CurrentRow.Cells["Branch_Sel"].Value = false;
                        }
                    }
                    else
                    {
                        if (Privileges.AddPriv.Equals("true") || Privileges.ChangePriv.Equals("true"))
                            Branches_Grid.CurrentRow.Cells["Branch_Sel"].Value = true;
                        else
                        {
                            Branches_Grid.CurrentRow.Cells["Branch_Sel"].Value = false;
                            MessageBox.Show("You do not have privilige to Add/Edit selected Branch", "CAP Systems");
                        }
                    }

                }
            }
        }

        private void UnCheck_Selected_Branch(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                Branches_Grid.CurrentRow.Cells["Branch_Status"].Value = "N";
                Branches_Grid.Rows[Branches_Grid.CurrentRow.Index].DefaultCellStyle.ForeColor = Color.MediumVioletRed;
                Pb_Add_CA.Visible = PbDelete.Visible = false;
                Clear_SP_CAMS_Grid();
                //SP_CAMS_Grid.Rows.Clear();
            }
            else
                Branches_Grid.CurrentRow.Cells["Branch_Sel"].Value = true;
        }


        private bool Get_Postings_For_Sel_Branch()
        {
            bool Postings_Exists = false;

            foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
            {
                if (dr.Cells["SP2_Operation"].Value.ToString() == "C")
                {
                    Postings_Exists = true; break;
                }
            }

            return Postings_Exists;
        }


        string Sel_CAMS_Notes_Key = null;
        private void SP_CAMS_Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (SP_CAMS_Grid.Rows.Count > 0)
            {

                string Tmp_Notes_Key_New = "";
                try
                {
                    Tmp_Notes_Key_New = SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString();
                }
                catch (Exception ex) { Tmp_Notes_Key_New = "Branch"; goto Bypass_Loop; }


                //string Tmp_Notes_Key1 = SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_Key"].Value;
                //string Tmp_Notes_Key = SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_Key"].Value.ToString();
                Sel_CAMS_Notes_Key = Hierarchy + Spm_Year + App_No + (Tmp_Notes_Key_New != "Branch" ? SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_Key"].Value.ToString() : "");

                //if ((SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA" || SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "A") &&
                //     Privileges.AddPriv.Equals("true") && Spm_Year == BaseForm.BaseYear)
                //    Pb_Add_CA.Visible = true;
                //else
                //    Pb_Add_CA.Visible = false;        // Commented to allow multiple MS Postings on 

                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C" && Privileges.ChangePriv.Equals("true"))
                    Pb_Edit.Visible = true;
                else
                    Pb_Edit.Visible = false;


                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C") // && Privileges.ChangePriv.Equals("true")) Commented on 08212014
                {

                    if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_SW"].Value.ToString() == "Y")
                        PB_SP2_Notes.ImageSource = "captain-casenotes";
                    else
                        PB_SP2_Notes.ImageSource = "captain-casenotesadd"; ;

                    PB_SP2_Notes.Visible = true;
                }
                else
                    PB_SP2_Notes.Visible = false;

                Curr_Group = 0;
                if (!string.IsNullOrEmpty(SP_CAMS_Grid.CurrentRow.Cells["SP2_Curr_Grp"].Value.ToString().Trim()))
                    Curr_Group = int.Parse(SP_CAMS_Grid.CurrentRow.Cells["SP2_Curr_Grp"].Value.ToString());
                Prepare_Menu_items();

                if (Cb_Show_All_Postings.Checked)
                {
                    string Curr_Branch = SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Branch"].Value.ToString();

                    foreach (DataGridViewRow dr in Branches_Grid.Rows)
                    {
                        if (dr.Cells["Branch_Code"].Value.ToString() == Curr_Branch)
                            Branches_Grid.Rows[dr.Index].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                        else
                            Branches_Grid.Rows[dr.Index].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F);
                    }
                }

                buttonAllowAMDSwitch("N");
            }

        Bypass_Loop:
            string tmp = "";

        }

        //private bool Check_For_CurrentYear_MS()
        //{
        //    bool MS_Exists_In_This_Year = false;

        //    if (SP_CAMS_Grid.Rows.Count > 0)
        //    {
        //        if (SP_CAMS_Grid.CurrentRow.Cells["SP2_TYpe"].Value.ToString() == "MS")
        //        {
        //            foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
        //            {
        //                if (dr.Cells["SP2_Year"].Value.ToString() == Year &&
        //                    dr.Cells["SP2_Branch"].Value.ToString() == SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch"].Value.ToString() &&
        //                    dr.Cells["SP2_Group"].Value.ToString() == SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString() &&
        //                    dr.Cells["SP2_CAMS_Code"].Value.ToString() == SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString())
        //                {
        //                    MS_Exists_In_This_Year = true; break;
        //                }
        //            }
        //        }
        //    }

        //    return MS_Exists_In_This_Year;
        //}


        string strFolderPath = string.Empty;
        string Random_Filename = null; string PdfName = null;
        private void On_SaveForm_Closed1()
        {
            Random_Filename = null;
            PdfName = "Pdf File";
            PdfName = "SPREPAPP_" + BaseForm.BaseApplicationNo;
            //PdfName = strFolderPath + PdfName;
            PdfName = propReportPath + BaseForm.UserID + "\\" + PdfName;
            try
            {
                if (!Directory.Exists(propReportPath + BaseForm.UserID.Trim()))
                { DirectoryInfo di = Directory.CreateDirectory(propReportPath + BaseForm.UserID.Trim()); }
            }
            catch (Exception ex)
            {
                CommonFunctions.MessageBoxDisplay("Error");
            }

            try
            {
                string Tmpstr = PdfName + ".pdf";
                if (File.Exists(Tmpstr))
                    File.Delete(Tmpstr);
            }
            catch (Exception ex)
            {
                int length = 8;
                string newFileName = System.Guid.NewGuid().ToString();
                newFileName = newFileName.Replace("-", string.Empty);

                Random_Filename = PdfName + newFileName.Substring(0, length) + ".pdf";
            }


            if (!string.IsNullOrEmpty(Random_Filename))
                PdfName = Random_Filename;
            else
                PdfName += ".pdf";

            FileStream fs = new FileStream(PdfName, FileMode.Create);

            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            //document.SetPageSize(new iTextSharp.text.Rectangle(iTextSharp.text.PageSize.A4.Width, iTextSharp.text.PageSize.A4.Height));
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            string Name = BaseForm.BaseApplicationName;
            string AppNo = BaseForm.BaseApplicationNo;

            //BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, false);
            BaseFont bf_Times = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1250, false);
            iTextSharp.text.Font fc = new iTextSharp.text.Font(bfTimes, 10, 2);
            iTextSharp.text.Font fc1 = new iTextSharp.text.Font(bf_Times, 10, 2, BaseColor.BLUE);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bfTimes, 10);
            //iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 8, 3);
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(bf_Times, 10);

            //DataSet dsSP_Services = DatabaseLayer.SPAdminDB.Browse_CASESP0(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), null, null, null, null, null, null, null);
            //DataRow drSP_Services = dsSP_Services.Tables[0].Rows[0];

            PdfPTable Header = new PdfPTable(2);
            Header.TotalWidth = 500f;
            Header.WidthPercentage = 100;
            Header.LockedWidth = true;
            float[] Headerwidths = new float[] { 50f, 17f };
            Header.SetWidths(Headerwidths);
            Header.HorizontalAlignment = Element.ALIGN_CENTER;
            Header.SpacingAfter = 07f;

            PdfPCell AppName = new PdfPCell(new Phrase("Applicant Name :" + Name, fc1));
            AppName.HorizontalAlignment = Element.ALIGN_LEFT;
            AppName.Border = iTextSharp.text.Rectangle.NO_BORDER;
            Header.AddCell(AppName);

            PdfPCell Application = new PdfPCell(new Phrase("App# :" + AppNo, fc1));
            Application.HorizontalAlignment = Element.ALIGN_RIGHT;
            Application.Border = iTextSharp.text.Rectangle.NO_BORDER;
            Header.AddCell(Application);
            //document.Add(Header);
            PdfPTable table = new PdfPTable(4);
            table.TotalWidth = 500f;
            table.WidthPercentage = 100;
            table.LockedWidth = true;
            float[] widths = new float[] { 15f, 80f, 15f, 15f };
            table.SetWidths(widths);
            table.HorizontalAlignment = Element.ALIGN_CENTER;


            string Year = null;
            if (!string.IsNullOrEmpty(BaseForm.BaseYear.Trim()))
                Year = BaseForm.BaseYear;


            //table.SpacingAfter = 15f;sp0_pbranch_desc

            List<CASESPMEntity> CaseSPM_List = new List<CASESPMEntity>();
            CASESPMEntity Search_CaseSPM_Entity = new CASESPMEntity(true);
            List<CASEACTEntity> CaseAct_Details = new List<CASEACTEntity>();
            List<CASEMSEntity> CaseMS_Details = new List<CASEMSEntity>();
            CASEACTEntity CA_Entity = new CASEACTEntity();
            Search_CaseSPM_Entity.agency = BaseForm.BaseAgency; Search_CaseSPM_Entity.dept = BaseForm.BaseDept; Search_CaseSPM_Entity.program = BaseForm.BaseProg; Search_CaseSPM_Entity.year = Spm_Year;
            Search_CaseSPM_Entity.app_no = BaseForm.BaseApplicationNo; Search_CaseSPM_Entity.service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString().Trim();
            Search_CaseSPM_Entity.Sp0_Validatetd = Search_CaseSPM_Entity.Def_Program = null; Search_CaseSPM_Entity.Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));
            CaseSPM_List = _model.SPAdminData.Browse_CASESPM(Search_CaseSPM_Entity, "Browse");
            //DataSet dsSP_CaseSP2 = DatabaseLayer.SPAdminDB.Browse_CASESP2(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), null,null, null);
            //DataTable dtSP_CaseSP2 = dsSP_CaseSP2.Tables[0];
            DataSet dsSP_CaseSP2 = new DataSet(); DataTable dtSP_CaseSP2 = new DataTable();
            string CAMSDesc = null; string CaDate = null; string CaDate_Follow_on = null;
            string SerVicePlan = null, Priv_ServicePlan = null; string SP_Desc = null;
            bool First = true; string Desc = null; string Branch = null, Priv_Branch = null, SP_Plan_desc = null;
            if (CaseSPM_List.Count > 0)
            {
                foreach (CASESPMEntity Entity in CaseSPM_List)
                {
                    SerVicePlan = Entity.service_plan.ToString().Trim();
                    string Branch_SPM = Entity.sel_branches.ToString().Trim();
                    int length = Entity.sel_branches.Length;
                    DataSet dsSP_Services = DatabaseLayer.SPAdminDB.Browse_CASESP0(SerVicePlan, null, null, null, null, null, null, null, null);
                    DataRow drSP_Services = dsSP_Services.Tables[0].Rows[0];
                    SP_Desc = drSP_Services["sp0_description"].ToString().Trim();
                    for (int i = 0; i < length;)
                    {
                        string Temp_Branch = Entity.sel_branches.Substring(i, 1);
                        dsSP_CaseSP2 = DatabaseLayer.SPAdminDB.Browse_CASESP2(SerVicePlan, Temp_Branch, null, null);
                        dtSP_CaseSP2 = dsSP_CaseSP2.Tables[0];

                        Priv_Branch = null;

                        if (dtSP_CaseSP2.Rows.Count > 0)
                        {
                            DataView dv = dtSP_CaseSP2.DefaultView;
                            dv.Sort = "sp2_branch DESC";
                            dtSP_CaseSP2 = dv.ToTable();
                            foreach (DataRow dr in dtSP_CaseSP2.Rows)
                            {
                                CaDate = null; CaDate_Follow_on = null;
                                Branch = dr["sp2_branch"].ToString().Trim();
                                if (SerVicePlan == dr["sp2_serviceplan"].ToString() && Branch != Priv_Branch)
                                {
                                    if (table.Rows.Count > 0)
                                        document.Add(Header);
                                    //Header.DeleteBodyRows();
                                    document.Add(table);
                                    table.DeleteBodyRows();
                                    if (!First)
                                        document.NewPage();
                                    string Service_desc = drSP_Services["sp0_pbranch_desc"].ToString();
                                    if (!First)
                                    {
                                        if (Branch.Trim() == drSP_Services["sp0_branch1_code"].ToString().Trim())
                                            Service_desc = drSP_Services["sp0_branch1_desc"].ToString();
                                        else if (Branch.Trim() == drSP_Services["sp0_branch2_code"].ToString().Trim())
                                            Service_desc = drSP_Services["sp0_branch2_desc"].ToString();
                                        else if (Branch.Trim() == drSP_Services["sp0_branch3_code"].ToString().Trim())
                                            Service_desc = drSP_Services["sp0_branch3_desc"].ToString();
                                        else if (Branch.Trim() == drSP_Services["sp0_branch4_code"].ToString().Trim())
                                            Service_desc = drSP_Services["sp0_branch4_desc"].ToString();
                                        else if (Branch.Trim() == drSP_Services["sp0_branch5_code"].ToString().Trim())
                                            Service_desc = drSP_Services["sp0_branch5_desc"].ToString();
                                    }
                                    PdfPCell SP_Desc_Header = new PdfPCell(new Phrase("Service: " + SP_Desc.Trim(), fc1));
                                    SP_Desc_Header.HorizontalAlignment = Element.ALIGN_LEFT;
                                    SP_Desc_Header.Colspan = 2;
                                    SP_Desc_Header.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    table.AddCell(SP_Desc_Header);

                                    PdfPCell SP_Desc_Date = new PdfPCell(new Phrase("Start Date: " + LookupDataAccess.Getdate(Entity.startdate.Trim()), fc1));
                                    SP_Desc_Date.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    SP_Desc_Date.Colspan = 2;
                                    SP_Desc_Date.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    table.AddCell(SP_Desc_Date);

                                    PdfPCell ServiceDesc = new PdfPCell(new Phrase("Branch :" + Service_desc.Trim(), TblFontBold));
                                    ServiceDesc.HorizontalAlignment = Element.ALIGN_CENTER;
                                    ServiceDesc.Colspan = 4;
                                    ServiceDesc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    table.AddCell(ServiceDesc);


                                    PdfPCell Headercell1 = new PdfPCell(new Phrase("Type", TblFontBold));
                                    Headercell1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    Headercell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    Headercell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    Headercell1.BorderWidthBottom = 0.7f;
                                    table.AddCell(Headercell1);

                                    PdfPCell Headercell2 = new PdfPCell(new Phrase("Description", TblFontBold));
                                    // cell.BackgroundColor = new BaseColor(190, 120, 204);
                                    Headercell2.HorizontalAlignment = Element.ALIGN_CENTER;
                                    Headercell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    Headercell2.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    Headercell2.BorderWidthBottom = 0.7f;
                                    table.AddCell(Headercell2);

                                    PdfPCell Headercell3 = new PdfPCell(new Phrase("Date", TblFontBold));
                                    // cell.BackgroundColor = new BaseColor(190, 120, 204);
                                    Headercell3.HorizontalAlignment = Element.ALIGN_CENTER;
                                    Headercell3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    Headercell3.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    Headercell3.BorderWidthBottom = 0.7f;
                                    table.AddCell(Headercell3);

                                    PdfPCell Headercell4 = new PdfPCell(new Phrase("FUp-Date", TblFontBold));
                                    // cell.BackgroundColor = new BaseColor(190, 120, 204);
                                    Headercell4.HorizontalAlignment = Element.ALIGN_CENTER;
                                    Headercell4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    Headercell4.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    Headercell4.BorderWidthBottom = 0.7f;
                                    table.AddCell(Headercell4);
                                    //table.HeaderRows = 2;
                                    Priv_Branch = Branch;
                                    First = false;

                                }

                                string CAMSType = dr["sp2_type"].ToString();

                                if (CAMSType == "CA")
                                {
                                    //List<CAMASTEntity> CAMASTList = new List<CAMASTEntity>();
                                    DataSet dsCAMAST = DatabaseLayer.SPAdminDB.Browse_CAMAST(null, dr["sp2_cams_code"].ToString().Trim(), null, null);
                                    if (dsCAMAST.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow drCAMAST = dsCAMAST.Tables[0].Rows[0];

                                        CAMSDesc = drCAMAST["CA_DESC"].ToString().Trim();
                                        //DataSet dsCaseAct=DatabaseLayer.SPAdminDB.Browse_CASEACT()
                                        CA_Entity.Agency = BaseForm.BaseAgency;
                                        CA_Entity.Dept = BaseForm.BaseDept;
                                        CA_Entity.Program = BaseForm.BaseProg;


                                        //CA_Pass_Entity.Year = BaseYear;                        
                                        CA_Entity.Year = Spm_Year;                             // Year will be always Four-Spaces in CASEACT
                                        CA_Entity.App_no = BaseForm.BaseApplicationNo;
                                        CA_Entity.ACT_Code = dr["sp2_cams_code"].ToString().Trim();
                                        CA_Entity.Service_plan = SerVicePlan;
                                        CA_Entity.Branch = Branch.Trim(); CA_Entity.Group = dr["sp2_orig_grp"].ToString().Trim();
                                        CA_Entity.ACT_Date = CA_Entity.ACT_Seq = CA_Entity.Site = CA_Entity.Fund1 = null;
                                        CA_Entity.Fund2 = CA_Entity.Fund3 = CA_Entity.Caseworker = CA_Entity.Vendor_No = null;
                                        CA_Entity.Check_Date = CA_Entity.Check_No = CA_Entity.Cost = CA_Entity.Followup_On = null;
                                        CA_Entity.Followup_Comp = CA_Entity.Followup_By = CA_Entity.Refer_Data = CA_Entity.Cust_Code1 = null;
                                        CA_Entity.Cust_Value1 = CA_Entity.Cust_Code2 = CA_Entity.Cust_Value2 = CA_Entity.Cust_Code3 = null;
                                        CA_Entity.Cust_Value3 = CA_Entity.Lstc_Date = CA_Entity.Lsct_Operator = CA_Entity.Add_Date = null;
                                        CA_Entity.Add_Operator = CA_Entity.ACT_ID = null; CA_Entity.Bulk = CA_Entity.Act_PROG = null;
                                        CA_Entity.Cust_Code4 = CA_Entity.Cust_Value4 = CA_Entity.Cust_Code5 = CA_Entity.Cust_Value5 = null;
                                        CA_Entity.Units = CA_Entity.UOM = CA_Entity.Curr_Grp = null;
                                        CA_Entity.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013

                                        CaseAct_Details = _model.SPAdminData.Browse_CASEACT(CA_Entity, "Browse");
                                        CaseAct_Details = CaseAct_Details.OrderByDescending(u => Convert.ToDateTime(u.ACT_Date.Trim())).ToList();
                                        if (CaseAct_Details.Count > 0)
                                        {
                                            string Priv_Type = null, Priv_Cams_Desc = null;
                                            foreach (CASEACTEntity entity in CaseAct_Details)
                                            {
                                                CaDate = LookupDataAccess.Getdate(entity.ACT_Date).ToString();
                                                CaDate_Follow_on = LookupDataAccess.Getdate(entity.Followup_On).ToString();

                                                if (CAMSType.Trim() != Priv_Type)
                                                {
                                                    PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowType.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowType);
                                                    Priv_Type = CAMSType.Trim();
                                                }
                                                else
                                                {
                                                    PdfPCell RowType = new PdfPCell(new Phrase("", TableFont));
                                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowType.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowType);
                                                }
                                                if (CAMSDesc.Trim() != Priv_Cams_Desc)
                                                {
                                                    PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowDesc.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowDesc);
                                                    Priv_Cams_Desc = CAMSDesc.Trim();
                                                }
                                                else
                                                {
                                                    PdfPCell RowDesc = new PdfPCell(new Phrase("", TableFont));
                                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowDesc.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowDesc);
                                                    Priv_Cams_Desc = CAMSDesc.Trim();
                                                }
                                                PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                                RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate);

                                                PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                                RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate_Foolow);
                                            }
                                        }
                                        else
                                        {

                                            PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                            RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowType.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowType.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowType);

                                            PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                            RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDesc.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDesc);

                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Foolow);
                                        }

                                    }
                                }
                                else
                                {
                                    DataSet MSMast = DatabaseLayer.SPAdminDB.Browse_MSMAST(null, dr["sp2_cams_code"].ToString().Trim(), null, null, null);
                                    if (MSMast.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow drMSMast = MSMast.Tables[0].Rows[0];


                                        CAMSDesc = drMSMast["MS_DESC"].ToString().Trim();
                                        string MSType = drMSMast["MS_TYPE"].ToString();
                                        string Type_Desc = string.Empty;
                                        //if (MSType == "M")
                                        //    Type_Desc = "Milestone";
                                        //else 
                                            Type_Desc = "Outcome";

                                        Search_MS_Details.Agency = BaseForm.BaseAgency;
                                        Search_MS_Details.Dept = BaseForm.BaseDept;
                                        Search_MS_Details.Program = BaseForm.BaseProg;
                                        //Search_MS_Details.Year = BaseYear; 
                                        Search_MS_Details.Year = Spm_Year;                              // Year will be always Four-Spaces in CASEMS
                                        Search_MS_Details.App_no = BaseForm.BaseApplicationNo;
                                        Search_MS_Details.MS_Code = dr["sp2_cams_code"].ToString().Trim();
                                        Search_MS_Details.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));

                                        Search_MS_Details.Service_plan = SerVicePlan;
                                        Search_MS_Details.Branch = Branch.Trim(); Search_MS_Details.Group = dr["sp2_orig_grp"].ToString().Trim();
                                        Search_MS_Details.ID = Search_MS_Details.Date = Search_MS_Details.CaseWorker = Search_MS_Details.Site = null;
                                        Search_MS_Details.Result = Search_MS_Details.OBF = Search_MS_Details.Add_Operator = null;
                                        Search_MS_Details.Lstc_Date = Search_MS_Details.Lsct_Operator = Search_MS_Details.Add_Date = Search_MS_Details.Bulk =
                                        Search_MS_Details.Acty_PROG = Search_MS_Details.Curr_Grp = null;

                                        CaseMS_Details = _model.SPAdminData.Browse_CASEMS(Search_MS_Details, "Browse");
                                        CaseMS_Details = CaseMS_Details.OrderByDescending(u => Convert.ToDateTime(u.Date.Trim())).ToList();
                                        if (CaseMS_Details.Count > 0)
                                        {

                                            foreach (CASEMSEntity entity in CaseMS_Details)
                                            {
                                                CaDate = LookupDataAccess.Getdate(entity.Date);
                                                CaDate_Follow_on = LookupDataAccess.Getdate(entity.MS_FUP_Date).ToString();

                                                PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                                RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowType.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowType);

                                                PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                                RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDesc.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDesc);

                                                PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                                RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate);

                                                PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                                RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate_Foolow);
                                            }
                                        }
                                        else
                                        {
                                            PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                            RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowType.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowType.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowType);

                                            PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                            RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDesc.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDesc);

                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Foolow);
                                        }

                                    }
                                }

                            }
                        }
                        else
                        {
                            First = false;
                        }
                        i++;
                    }

                    if (drSP_Services["SP0_ALLOW_ADLBRANCH"].ToString() == "Y")
                    {
                        List<CASESPM2Entity> casespm2List = new List<CASESPM2Entity>();
                        CASESPM2Entity Search_Entity2 = new CASESPM2Entity();

                        Search_Entity2.Agency = Hierarchy.Substring(0, 2);
                        Search_Entity2.Dept = Hierarchy.Substring(2, 2);
                        Search_Entity2.Prog = Hierarchy.Substring(4, 2);
                        Search_Entity2.Year = BaseForm.BaseYear;
                        Search_Entity2.Year = Spm_Year;                         // Year will be always Four-Spaces in CASESPM2
                        Search_Entity2.App = App_No;
                        Search_Entity2.Spm_Seq = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString();

                        Search_Entity2.ServPlan = Search_Entity2.Branch = Search_Entity2.Group = null;
                        Search_Entity2.Type1 = Search_Entity2.CamCd = Search_Entity2.Curr_Group = null;
                        Search_Entity2.SelOrdinal = Search_Entity2.DateLstc = Search_Entity2.lstcOperator = null;
                        Search_Entity2.Dateadd = Search_Entity2.addoperator = null;

                        Search_Entity2.ServPlan = SerVicePlan;

                        casespm2List = _model.SPAdminData.Browse_CASESPM2(Search_Entity2, "Browse");

                        if (casespm2List.Count > 0)
                        {
                            if (table.Rows.Count > 0)
                                document.Add(Header);
                            document.Add(table);
                            table.DeleteBodyRows();
                            document.NewPage();

                            PdfPCell SP_Desc_Header = new PdfPCell(new Phrase("Service: " + SP_Desc.Trim(), fc1));
                            SP_Desc_Header.HorizontalAlignment = Element.ALIGN_LEFT;
                            SP_Desc_Header.Colspan = 2;
                            SP_Desc_Header.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            table.AddCell(SP_Desc_Header);

                            PdfPCell SP_Desc_Date = new PdfPCell(new Phrase("Start Date: " + LookupDataAccess.Getdate(Entity.startdate.Trim()), fc1));
                            SP_Desc_Date.HorizontalAlignment = Element.ALIGN_RIGHT;
                            SP_Desc_Date.Colspan = 2;
                            SP_Desc_Date.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            table.AddCell(SP_Desc_Date);

                            PdfPCell ServiceDesc = new PdfPCell(new Phrase("Branch :" + "Additional Branch", TblFontBold));
                            ServiceDesc.HorizontalAlignment = Element.ALIGN_CENTER;
                            ServiceDesc.Colspan = 4;
                            ServiceDesc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            table.AddCell(ServiceDesc);


                            PdfPCell Headercell1 = new PdfPCell(new Phrase("Type", TblFontBold));
                            Headercell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            Headercell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            Headercell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                            Headercell1.BorderWidthBottom = 0.7f;
                            table.AddCell(Headercell1);

                            PdfPCell Headercell2 = new PdfPCell(new Phrase("Description", TblFontBold));
                            // cell.BackgroundColor = new BaseColor(190, 120, 204);
                            Headercell2.HorizontalAlignment = Element.ALIGN_CENTER;
                            Headercell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            Headercell2.BackgroundColor = BaseColor.LIGHT_GRAY;
                            Headercell2.BorderWidthBottom = 0.7f;
                            table.AddCell(Headercell2);

                            PdfPCell Headercell3 = new PdfPCell(new Phrase("Date", TblFontBold));
                            // cell.BackgroundColor = new BaseColor(190, 120, 204);
                            Headercell3.HorizontalAlignment = Element.ALIGN_CENTER;
                            Headercell3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            Headercell3.BackgroundColor = BaseColor.LIGHT_GRAY;
                            Headercell3.BorderWidthBottom = 0.7f;
                            table.AddCell(Headercell3);

                            PdfPCell Headercell4 = new PdfPCell(new Phrase("FUp-Date", TblFontBold));
                            // cell.BackgroundColor = new BaseColor(190, 120, 204);
                            Headercell4.HorizontalAlignment = Element.ALIGN_CENTER;
                            Headercell4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            Headercell4.BackgroundColor = BaseColor.LIGHT_GRAY;
                            Headercell4.BorderWidthBottom = 0.7f;
                            table.AddCell(Headercell4);

                            foreach (CASESPM2Entity Spm2 in casespm2List)
                            {
                                string CAMSType = Spm2.Type1; CaDate_Follow_on = null;
                                CaDate = null;
                                if (CAMSType == "CA")
                                {
                                    //List<CAMASTEntity> CAMASTList = new List<CAMASTEntity>();
                                    DataSet dsCAMAST = DatabaseLayer.SPAdminDB.Browse_CAMAST(null, Spm2.CamCd.Trim(), null, null);
                                    if (dsCAMAST.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow drCAMAST = dsCAMAST.Tables[0].Rows[0];

                                        CAMSDesc = drCAMAST["CA_DESC"].ToString().Trim();
                                        //DataSet dsCaseAct=DatabaseLayer.SPAdminDB.Browse_CASEACT()
                                        CA_Entity.Agency = BaseForm.BaseAgency;
                                        CA_Entity.Dept = BaseForm.BaseDept;
                                        CA_Entity.Program = BaseForm.BaseProg;


                                        //CA_Pass_Entity.Year = BaseYear;                        
                                        CA_Entity.Year = Spm_Year;                             // Year will be always Four-Spaces in CASEACT
                                        CA_Entity.App_no = BaseForm.BaseApplicationNo;
                                        CA_Entity.ACT_Code = Spm2.CamCd.Trim().Trim();
                                        CA_Entity.Service_plan = SerVicePlan;
                                        CA_Entity.Branch = Spm2.Branch; CA_Entity.Group = Spm2.Group.Trim();
                                        CA_Entity.ACT_Date = CA_Entity.ACT_Seq = CA_Entity.Site = CA_Entity.Fund1 = null;
                                        CA_Entity.Fund2 = CA_Entity.Fund3 = CA_Entity.Caseworker = CA_Entity.Vendor_No = null;
                                        CA_Entity.Check_Date = CA_Entity.Check_No = CA_Entity.Cost = CA_Entity.Followup_On = null;
                                        CA_Entity.Followup_Comp = CA_Entity.Followup_By = CA_Entity.Refer_Data = CA_Entity.Cust_Code1 = null;
                                        CA_Entity.Cust_Value1 = CA_Entity.Cust_Code2 = CA_Entity.Cust_Value2 = CA_Entity.Cust_Code3 = null;
                                        CA_Entity.Cust_Value3 = CA_Entity.Lstc_Date = CA_Entity.Lsct_Operator = CA_Entity.Add_Date = null;
                                        CA_Entity.Add_Operator = CA_Entity.ACT_ID = null; CA_Entity.Bulk = CA_Entity.Act_PROG = null;
                                        CA_Entity.Cust_Code4 = CA_Entity.Cust_Value4 = CA_Entity.Cust_Code5 = CA_Entity.Cust_Value5 = null;
                                        CA_Entity.Units = CA_Entity.UOM = CA_Entity.Curr_Grp = null;
                                        CA_Entity.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013
                                        CaseAct_Details = _model.SPAdminData.Browse_CASEACT(CA_Entity, "Browse");
                                        if (CaseAct_Details.Count > 0)
                                        {
                                            string Priv_Type = null, Priv_Cams_Desc = null;
                                            foreach (CASEACTEntity entity in CaseAct_Details)
                                            {
                                                CaDate = LookupDataAccess.Getdate(entity.ACT_Date).ToString();
                                                CaDate_Follow_on = LookupDataAccess.Getdate(entity.Followup_On).ToString();

                                                if (CAMSType.Trim() != Priv_Type)
                                                {
                                                    PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowType.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowType);
                                                    Priv_Type = CAMSType.Trim();
                                                }
                                                else
                                                {
                                                    PdfPCell RowType = new PdfPCell(new Phrase("", TableFont));
                                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowType.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowType);
                                                }
                                                if (CAMSDesc.Trim() != Priv_Cams_Desc)
                                                {
                                                    PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowDesc.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowDesc);
                                                    Priv_Cams_Desc = CAMSDesc.Trim();
                                                }
                                                else
                                                {
                                                    PdfPCell RowDesc = new PdfPCell(new Phrase("", TableFont));
                                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowDesc.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowDesc);
                                                    Priv_Cams_Desc = CAMSDesc.Trim();
                                                }
                                                PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                                RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate);

                                                PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                                RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate_Foolow);
                                            }
                                        }
                                        else
                                        {

                                            PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                            RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowType.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowType.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowType);

                                            PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                            RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDesc.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDesc);

                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Foolow);
                                        }

                                    }
                                }
                                else
                                {
                                    DataSet MSMast = DatabaseLayer.SPAdminDB.Browse_MSMAST(null, Spm2.CamCd.Trim(), null, null, null);
                                    string Type_Desc = string.Empty;
                                    if (MSMast.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow drMSMast = MSMast.Tables[0].Rows[0];


                                        CAMSDesc = drMSMast["MS_DESC"].ToString().Trim();
                                        string MSType = drMSMast["MS_TYPE"].ToString();

                                        //if (MSType == "M")
                                        //    Type_Desc = "Milestone";
                                        //else 
                                            Type_Desc = "Outcome";
                                    }
                                    else { Type_Desc = "Outcome"; CAMSDesc = string.Empty; }

                                    Search_MS_Details.Agency = BaseForm.BaseAgency;
                                    Search_MS_Details.Dept = BaseForm.BaseDept;
                                    Search_MS_Details.Program = BaseForm.BaseProg;
                                    //Search_MS_Details.Year = BaseYear; 
                                    Search_MS_Details.Year = Spm_Year;                              // Year will be always Four-Spaces in CASEMS
                                    Search_MS_Details.App_no = BaseForm.BaseApplicationNo;
                                    Search_MS_Details.MS_Code = Spm2.CamCd.Trim().Trim();
                                    Search_MS_Details.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));

                                    Search_MS_Details.Service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString().Trim();
                                    Search_MS_Details.Branch = Spm2.Branch; Search_MS_Details.Group = Spm2.Group.Trim();
                                    Search_MS_Details.ID = Search_MS_Details.Date = Search_MS_Details.CaseWorker = Search_MS_Details.Site = null;
                                    Search_MS_Details.Result = Search_MS_Details.OBF = Search_MS_Details.Add_Operator = null;
                                    Search_MS_Details.Lstc_Date = Search_MS_Details.Lsct_Operator = Search_MS_Details.Add_Date = Search_MS_Details.Bulk =
                                    Search_MS_Details.Acty_PROG = Search_MS_Details.Curr_Grp = null;

                                    CaseMS_Details = _model.SPAdminData.Browse_CASEMS(Search_MS_Details, "Browse");
                                    if (CaseMS_Details.Count > 0)
                                    {

                                        foreach (CASEMSEntity entity in CaseMS_Details)
                                        {
                                            CaDate = LookupDataAccess.Getdate(entity.Date);
                                            CaDate_Follow_on = LookupDataAccess.Getdate(entity.MS_FUP_Date).ToString();

                                            PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                            RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowType.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowType.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowType);

                                            PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                            RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDesc.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDesc);

                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Foolow);
                                        }
                                    }
                                    else
                                    {
                                        PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                        RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowType.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowType.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowType);

                                        PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                        RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDesc.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDesc);

                                        PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                        RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate);

                                        PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                        RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate_Foolow);
                                    }

                                    //}
                                }
                            }


                        }

                    }
                }
            }
            else
            {
                DataSet dsSP_Services = DatabaseLayer.SPAdminDB.Browse_CASESP0(SerVicePlan, null, null, null, null, null, null, null, null);
                DataRow drSP_Services = dsSP_Services.Tables[0].Rows[0];
                SP_Desc = drSP_Services["sp0_description"].ToString().Trim();
                if (drSP_Services["SP0_ALLOW_ADLBRANCH"].ToString() == "Y")
                {
                    List<CASESPM2Entity> casespm2List = new List<CASESPM2Entity>();
                    CASESPM2Entity Search_Entity2 = new CASESPM2Entity();

                    Search_Entity2.Agency = Hierarchy.Substring(0, 2);
                    Search_Entity2.Dept = Hierarchy.Substring(2, 2);
                    Search_Entity2.Prog = Hierarchy.Substring(4, 2);
                    Search_Entity2.Year = Spm_Year;
                    //Search_Entity2.Year = null;                         // Year will be always Four-Spaces in CASESPM2
                    Search_Entity2.App = App_No;
                    Search_Entity2.Spm_Seq = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString();

                    Search_Entity2.ServPlan = Search_Entity2.Branch = Search_Entity2.Group = null;
                    Search_Entity2.Type1 = Search_Entity2.CamCd = Search_Entity2.Curr_Group = null;
                    Search_Entity2.SelOrdinal = Search_Entity2.DateLstc = Search_Entity2.lstcOperator = null;
                    Search_Entity2.Dateadd = Search_Entity2.addoperator = null;

                    Search_Entity2.ServPlan = SerVicePlan;

                    casespm2List = _model.SPAdminData.Browse_CASESPM2(Search_Entity2, "Browse");

                    if (casespm2List.Count > 0)
                    {
                        if (table.Rows.Count > 0)
                            document.Add(Header);
                        //document.Add(table);
                        //table.DeleteBodyRows();
                        //document.NewPage();

                        PdfPCell SP_Desc_Header = new PdfPCell(new Phrase("Service: " + SP_Desc.Trim(), fc1));
                        SP_Desc_Header.HorizontalAlignment = Element.ALIGN_CENTER;
                        SP_Desc_Header.Colspan = 4;
                        SP_Desc_Header.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        table.AddCell(SP_Desc_Header);

                        PdfPCell ServiceDesc = new PdfPCell(new Phrase("Branch :" + "Additional Branch", TblFontBold));
                        ServiceDesc.HorizontalAlignment = Element.ALIGN_CENTER;
                        ServiceDesc.Colspan = 4;
                        ServiceDesc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        table.AddCell(ServiceDesc);


                        PdfPCell Headercell1 = new PdfPCell(new Phrase("Type", TblFontBold));
                        Headercell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        Headercell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        Headercell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                        Headercell1.BorderWidthBottom = 0.7f;
                        table.AddCell(Headercell1);

                        PdfPCell Headercell2 = new PdfPCell(new Phrase("Description", TblFontBold));
                        // cell.BackgroundColor = new BaseColor(190, 120, 204);
                        Headercell2.HorizontalAlignment = Element.ALIGN_CENTER;
                        Headercell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        Headercell2.BackgroundColor = BaseColor.LIGHT_GRAY;
                        Headercell2.BorderWidthBottom = 0.7f;
                        table.AddCell(Headercell2);

                        PdfPCell Headercell3 = new PdfPCell(new Phrase("Date", TblFontBold));
                        // cell.BackgroundColor = new BaseColor(190, 120, 204);
                        Headercell3.HorizontalAlignment = Element.ALIGN_CENTER;
                        Headercell3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        Headercell3.BackgroundColor = BaseColor.LIGHT_GRAY;
                        Headercell3.BorderWidthBottom = 0.7f;
                        table.AddCell(Headercell3);

                        PdfPCell Headercell4 = new PdfPCell(new Phrase("FUp-Date", TblFontBold));
                        // cell.BackgroundColor = new BaseColor(190, 120, 204);
                        Headercell4.HorizontalAlignment = Element.ALIGN_CENTER;
                        Headercell4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        Headercell4.BackgroundColor = BaseColor.LIGHT_GRAY;
                        Headercell4.BorderWidthBottom = 0.7f;
                        table.AddCell(Headercell4);

                        foreach (CASESPM2Entity Spm2 in casespm2List)
                        {
                            string CAMSType = Spm2.Type1;

                            if (CAMSType == "CA")
                            {
                                //List<CAMASTEntity> CAMASTList = new List<CAMASTEntity>();
                                DataSet dsCAMAST = DatabaseLayer.SPAdminDB.Browse_CAMAST(null, Spm2.CamCd.Trim(), null, null);
                                if (dsCAMAST.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drCAMAST = dsCAMAST.Tables[0].Rows[0];

                                    CAMSDesc = drCAMAST["CA_DESC"].ToString().Trim();
                                    //DataSet dsCaseAct=DatabaseLayer.SPAdminDB.Browse_CASEACT()
                                    CA_Entity.Agency = BaseForm.BaseAgency;
                                    CA_Entity.Dept = BaseForm.BaseDept;
                                    CA_Entity.Program = BaseForm.BaseProg;


                                    //CA_Pass_Entity.Year = BaseYear;                        
                                    CA_Entity.Year = Spm_Year;                             // Year will be always Four-Spaces in CASEACT
                                    CA_Entity.App_no = BaseForm.BaseApplicationNo;
                                    CA_Entity.ACT_Code = Spm2.CamCd.Trim().Trim();
                                    CA_Entity.Service_plan = SerVicePlan;
                                    CA_Entity.Branch = Spm2.Branch; CA_Entity.Group = Spm2.Group.Trim();
                                    CA_Entity.ACT_Date = CA_Entity.ACT_Seq = CA_Entity.Site = CA_Entity.Fund1 = null;
                                    CA_Entity.Fund2 = CA_Entity.Fund3 = CA_Entity.Caseworker = CA_Entity.Vendor_No = null;
                                    CA_Entity.Check_Date = CA_Entity.Check_No = CA_Entity.Cost = CA_Entity.Followup_On = null;
                                    CA_Entity.Followup_Comp = CA_Entity.Followup_By = CA_Entity.Refer_Data = CA_Entity.Cust_Code1 = null;
                                    CA_Entity.Cust_Value1 = CA_Entity.Cust_Code2 = CA_Entity.Cust_Value2 = CA_Entity.Cust_Code3 = null;
                                    CA_Entity.Cust_Value3 = CA_Entity.Lstc_Date = CA_Entity.Lsct_Operator = CA_Entity.Add_Date = null;
                                    CA_Entity.Add_Operator = CA_Entity.ACT_ID = null; CA_Entity.Bulk = CA_Entity.Act_PROG = null;
                                    CA_Entity.Cust_Code4 = CA_Entity.Cust_Value4 = CA_Entity.Cust_Code5 = CA_Entity.Cust_Value5 = null;
                                    CA_Entity.Units = CA_Entity.UOM = CA_Entity.Curr_Grp = null;
                                    CA_Pass_Entity.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013
                                    CaseAct_Details = _model.SPAdminData.Browse_CASEACT(CA_Entity, "Browse");
                                    if (CaseAct_Details.Count > 0)
                                    {
                                        string Priv_Type = null, Priv_Cams_Desc = null;
                                        foreach (CASEACTEntity entity in CaseAct_Details)
                                        {
                                            CaDate = LookupDataAccess.Getdate(entity.ACT_Date).ToString();
                                            CaDate_Follow_on = LookupDataAccess.Getdate(entity.Followup_On).ToString();

                                            if (CAMSType.Trim() != Priv_Type)
                                            {
                                                PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                                RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowType.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowType);
                                                Priv_Type = CAMSType.Trim();
                                            }
                                            else
                                            {
                                                PdfPCell RowType = new PdfPCell(new Phrase("", TableFont));
                                                RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowType.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowType);
                                            }
                                            if (CAMSDesc.Trim() != Priv_Cams_Desc)
                                            {
                                                PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                                RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDesc.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDesc);
                                                Priv_Cams_Desc = CAMSDesc.Trim();
                                            }
                                            else
                                            {
                                                PdfPCell RowDesc = new PdfPCell(new Phrase("", TableFont));
                                                RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDesc.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDesc);
                                                Priv_Cams_Desc = CAMSDesc.Trim();
                                            }
                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Foolow);
                                        }
                                    }
                                    else
                                    {

                                        PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                        RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowType.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowType.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowType);

                                        PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                        RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDesc.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDesc);

                                        PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                        RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate);

                                        PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                        RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate_Foolow);
                                    }

                                }
                            }
                            else
                            {
                                DataSet MSMast = DatabaseLayer.SPAdminDB.Browse_MSMAST(null, Spm2.CamCd.Trim(), null, null, null);
                                string Type_Desc = string.Empty;
                                if (MSMast.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drMSMast = MSMast.Tables[0].Rows[0];


                                    CAMSDesc = drMSMast["MS_DESC"].ToString().Trim();
                                    string MSType = drMSMast["MS_TYPE"].ToString();

                                    //if (MSType == "M")
                                    //    Type_Desc = "Milestone";
                                    //else 
                                        Type_Desc = "Outcome";
                                }
                                else
                                {
                                    Type_Desc = "Outcome"; CAMSDesc = string.Empty;
                                }

                                Search_MS_Details.Agency = BaseForm.BaseAgency;
                                Search_MS_Details.Dept = BaseForm.BaseDept;
                                Search_MS_Details.Program = BaseForm.BaseProg;
                                //Search_MS_Details.Year = BaseYear; 
                                Search_MS_Details.Year = Spm_Year;                              // Year will be always Four-Spaces in CASEMS
                                Search_MS_Details.App_no = BaseForm.BaseApplicationNo;
                                Search_MS_Details.MS_Code = Spm2.CamCd.Trim().Trim();
                                Search_MS_Details.SPM_Seq = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString();
                                Search_MS_Details.Service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString().Trim();
                                Search_MS_Details.Branch = Spm2.Branch; Search_MS_Details.Group = Spm2.Group.Trim();
                                Search_MS_Details.ID = Search_MS_Details.Date = Search_MS_Details.CaseWorker = Search_MS_Details.Site = null;
                                Search_MS_Details.Result = Search_MS_Details.OBF = Search_MS_Details.Add_Operator = null;
                                Search_MS_Details.Lstc_Date = Search_MS_Details.Lsct_Operator = Search_MS_Details.Add_Date = Search_MS_Details.Bulk =
                                Search_MS_Details.Acty_PROG = Search_MS_Details.Curr_Grp = null;

                                CaseMS_Details = _model.SPAdminData.Browse_CASEMS(Search_MS_Details, "Browse");
                                if (CaseMS_Details.Count > 0)
                                {

                                    foreach (CASEMSEntity entity in CaseMS_Details)
                                    {
                                        CaDate = LookupDataAccess.Getdate(entity.Date);
                                        CaDate_Follow_on = LookupDataAccess.Getdate(entity.MS_FUP_Date).ToString();

                                        PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                        RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowType.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowType.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowType);

                                        PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                        RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDesc.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDesc);

                                        PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                        RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate);

                                        PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                        RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate_Foolow);
                                    }
                                }
                                else
                                {
                                    PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                    //RowType.BorderWidthBottom = 0.7f;
                                    table.AddCell(RowType);

                                    PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                    //RowDesc.BorderWidthBottom = 0.7f;
                                    table.AddCell(RowDesc);

                                    PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                    RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                    RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                    //RowDate.BorderWidthBottom = 0.7f;
                                    table.AddCell(RowDate);

                                    PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                    RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                    RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                    //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                    table.AddCell(RowDate_Foolow);
                                }

                                //}
                            }
                        }


                    }

                }
            }
            if (table.Rows.Count > 0)
                document.Add(Header);
            document.Add(table);
            document.Close();
            fs.Close();
            fs.Dispose();

            if (BaseForm.BaseAgencyControlDetails.ReportSwitch.ToUpper() == "Y")
            {
                PdfViewerNewForm objfrm = new PdfViewerNewForm(PdfName);
                objfrm.FormClosed += new FormClosedEventHandler(On_Delete_PDF_File);
                objfrm.StartPosition = FormStartPosition.CenterScreen;
                objfrm.ShowDialog();
            }
            else
            {
                FrmViewer objfrm = new FrmViewer(PdfName);
                objfrm.FormClosed += new FormClosedEventHandler(On_Delete_PDF_File);
                objfrm.StartPosition = FormStartPosition.CenterScreen;
                objfrm.ShowDialog();
            }
        }


        private void On_SaveForm_Closed()
        {
            Random_Filename = null;
            PdfName = "Pdf File";
            PdfName = "SPREPAPP_" + BaseForm.BaseApplicationNo;
            //PdfName = strFolderPath + PdfName;
            PdfName = propReportPath + BaseForm.UserID + "\\" + PdfName;
            try
            {
                if (!Directory.Exists(propReportPath + BaseForm.UserID.Trim()))
                { DirectoryInfo di = Directory.CreateDirectory(propReportPath + BaseForm.UserID.Trim()); }
            }
            catch (Exception ex)
            {
                CommonFunctions.MessageBoxDisplay("Error");
            }

            try
            {
                string Tmpstr = PdfName + ".pdf";
                if (File.Exists(Tmpstr))
                    File.Delete(Tmpstr);
            }
            catch (Exception ex)
            {
                int length = 8;
                string newFileName = System.Guid.NewGuid().ToString();
                newFileName = newFileName.Replace("-", string.Empty);

                Random_Filename = PdfName + newFileName.Substring(0, length) + ".pdf";
            }


            if (!string.IsNullOrEmpty(Random_Filename))
                PdfName = Random_Filename;
            else
                PdfName += ".pdf";

            FileStream fs = new FileStream(PdfName, FileMode.Create);

            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            //document.SetPageSize(new iTextSharp.text.Rectangle(iTextSharp.text.PageSize.A4.Width, iTextSharp.text.PageSize.A4.Height));
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            string Name = BaseForm.BaseApplicationName;
            string AppNo = BaseForm.BaseApplicationNo;

            //BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, false);
            BaseFont bf_Times = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1250, false);
            iTextSharp.text.Font fc = new iTextSharp.text.Font(bfTimes, 10, 2);
            iTextSharp.text.Font fc1 = new iTextSharp.text.Font(bf_Times, 10, 2, BaseColor.BLUE);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bfTimes, 10);
            //iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 8, 3);
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(bf_Times, 10);

            //DataSet dsSP_Services = DatabaseLayer.SPAdminDB.Browse_CASESP0(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), null, null, null, null, null, null, null);
            //DataRow drSP_Services = dsSP_Services.Tables[0].Rows[0];

            PdfPTable Header = new PdfPTable(2);
            Header.TotalWidth = 550f;
            Header.WidthPercentage = 100;
            Header.LockedWidth = true;
            float[] Headerwidths = new float[] { 50f, 17f };
            Header.SetWidths(Headerwidths);
            Header.HorizontalAlignment = Element.ALIGN_CENTER;
            Header.SpacingAfter = 07f;

            PdfPCell AppName = new PdfPCell(new Phrase("Applicant Name :" + Name, fc1));
            AppName.HorizontalAlignment = Element.ALIGN_LEFT;
            AppName.Border = iTextSharp.text.Rectangle.NO_BORDER;
            Header.AddCell(AppName);

            PdfPCell Application = new PdfPCell(new Phrase("App# :" + AppNo, fc1));
            Application.HorizontalAlignment = Element.ALIGN_RIGHT;
            Application.Border = iTextSharp.text.Rectangle.NO_BORDER;
            Header.AddCell(Application);
            //document.Add(Header);
            PdfPTable table = new PdfPTable(5);
            table.TotalWidth = 550f;
            table.WidthPercentage = 100;
            table.LockedWidth = true;
            float[] widths = new float[] { 15f, 80f, 15f, 15f, 25f };
            table.SetWidths(widths);
            table.HorizontalAlignment = Element.ALIGN_CENTER;


            string Year = null;
            if (!string.IsNullOrEmpty(BaseForm.BaseYear.Trim()))
                Year = BaseForm.BaseYear;


            //table.SpacingAfter = 15f;sp0_pbranch_desc

            List<CASESPMEntity> CaseSPM_List = new List<CASESPMEntity>();
            CASESPMEntity Search_CaseSPM_Entity = new CASESPMEntity(true);
            List<CASEACTEntity> CaseAct_Details = new List<CASEACTEntity>();
            List<CASEMSEntity> CaseMS_Details = new List<CASEMSEntity>();
            CASEACTEntity CA_Entity = new CASEACTEntity();
            Search_CaseSPM_Entity.agency = BaseForm.BaseAgency; Search_CaseSPM_Entity.dept = BaseForm.BaseDept; Search_CaseSPM_Entity.program = BaseForm.BaseProg; Search_CaseSPM_Entity.year = Spm_Year;
            Search_CaseSPM_Entity.app_no = BaseForm.BaseApplicationNo; Search_CaseSPM_Entity.service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString().Trim();
            Search_CaseSPM_Entity.Sp0_Validatetd = Search_CaseSPM_Entity.Def_Program = null; Search_CaseSPM_Entity.Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));
            CaseSPM_List = _model.SPAdminData.Browse_CASESPM(Search_CaseSPM_Entity, "Browse");
            //DataSet dsSP_CaseSP2 = DatabaseLayer.SPAdminDB.Browse_CASESP2(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), null,null, null);
            //DataTable dtSP_CaseSP2 = dsSP_CaseSP2.Tables[0];
            DataSet dsSP_CaseSP2 = new DataSet(); DataTable dtSP_CaseSP2 = new DataTable();
            string CAMSDesc = null; string CaDate = null; string CaDate_Follow_on = null;
            string SerVicePlan = null, Priv_ServicePlan = null; string SP_Desc = null;
            bool First = true; string Desc = null; string Branch = null, Priv_Branch = null, SP_Plan_desc = null;
            if (CaseSPM_List.Count > 0)
            {
                foreach (CASESPMEntity Entity in CaseSPM_List)
                {
                    SerVicePlan = Entity.service_plan.ToString().Trim();
                    string Branch_SPM = Entity.sel_branches.ToString().Trim();
                    int length = Entity.sel_branches.Length;
                    DataSet dsSP_Services = DatabaseLayer.SPAdminDB.Browse_CASESP0(SerVicePlan, null, null, null, null, null, null, null, null);
                    DataRow drSP_Services = dsSP_Services.Tables[0].Rows[0];
                    SP_Desc = drSP_Services["sp0_description"].ToString().Trim();
                    for (int i = 0; i < length;)
                    {
                        string Temp_Branch = Entity.sel_branches.Substring(i, 1);
                        dsSP_CaseSP2 = DatabaseLayer.SPAdminDB.Browse_CASESP2(SerVicePlan, Temp_Branch, null, null);
                        dtSP_CaseSP2 = dsSP_CaseSP2.Tables[0];

                        Priv_Branch = null;

                        if (dtSP_CaseSP2.Rows.Count > 0)
                        {
                            DataView dv = dtSP_CaseSP2.DefaultView;
                            dv.Sort = "sp2_branch DESC";
                            dtSP_CaseSP2 = dv.ToTable();
                            foreach (DataRow dr in dtSP_CaseSP2.Rows)
                            {
                                CaDate = null; CaDate_Follow_on = null;
                                Branch = dr["sp2_branch"].ToString().Trim();
                                if (SerVicePlan == dr["sp2_serviceplan"].ToString() && Branch != Priv_Branch)
                                {
                                    if (table.Rows.Count > 0)
                                        document.Add(Header);
                                    //Header.DeleteBodyRows();
                                    document.Add(table);
                                    table.DeleteBodyRows();
                                    if (!First)
                                        document.NewPage();
                                    string Service_desc = drSP_Services["sp0_pbranch_desc"].ToString();
                                    if (!First)
                                    {
                                        if (Branch.Trim() == drSP_Services["sp0_branch1_code"].ToString().Trim())
                                            Service_desc = drSP_Services["sp0_branch1_desc"].ToString();
                                        else if (Branch.Trim() == drSP_Services["sp0_branch2_code"].ToString().Trim())
                                            Service_desc = drSP_Services["sp0_branch2_desc"].ToString();
                                        else if (Branch.Trim() == drSP_Services["sp0_branch3_code"].ToString().Trim())
                                            Service_desc = drSP_Services["sp0_branch3_desc"].ToString();
                                        else if (Branch.Trim() == drSP_Services["sp0_branch4_code"].ToString().Trim())
                                            Service_desc = drSP_Services["sp0_branch4_desc"].ToString();
                                        else if (Branch.Trim() == drSP_Services["sp0_branch5_code"].ToString().Trim())
                                            Service_desc = drSP_Services["sp0_branch5_desc"].ToString();
                                    }
                                    PdfPCell SP_Desc_Header = new PdfPCell(new Phrase("Service: " + SP_Desc.Trim(), fc1));
                                    SP_Desc_Header.HorizontalAlignment = Element.ALIGN_LEFT;
                                    SP_Desc_Header.Colspan = 2;
                                    SP_Desc_Header.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    table.AddCell(SP_Desc_Header);

                                    PdfPCell SP_Desc_Date = new PdfPCell(new Phrase("Start Date: " + LookupDataAccess.Getdate(Entity.startdate.Trim()), fc1));
                                    SP_Desc_Date.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    SP_Desc_Date.Colspan = 3;
                                    SP_Desc_Date.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    table.AddCell(SP_Desc_Date);

                                    PdfPCell ServiceDesc = new PdfPCell(new Phrase("Branch :" + Service_desc.Trim(), TblFontBold));
                                    ServiceDesc.HorizontalAlignment = Element.ALIGN_CENTER;
                                    ServiceDesc.Colspan = 5;
                                    ServiceDesc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    table.AddCell(ServiceDesc);


                                    PdfPCell Headercell1 = new PdfPCell(new Phrase("Type", TblFontBold));
                                    Headercell1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    Headercell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    Headercell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    Headercell1.BorderWidthBottom = 0.7f;
                                    table.AddCell(Headercell1);

                                    PdfPCell Headercell2 = new PdfPCell(new Phrase("Description", TblFontBold));
                                    // cell.BackgroundColor = new BaseColor(190, 120, 204);
                                    Headercell2.HorizontalAlignment = Element.ALIGN_CENTER;
                                    Headercell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    Headercell2.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    Headercell2.BorderWidthBottom = 0.7f;
                                    table.AddCell(Headercell2);

                                    PdfPCell Headercell3 = new PdfPCell(new Phrase("Date", TblFontBold));
                                    // cell.BackgroundColor = new BaseColor(190, 120, 204);
                                    Headercell3.HorizontalAlignment = Element.ALIGN_CENTER;
                                    Headercell3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    Headercell3.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    Headercell3.BorderWidthBottom = 0.7f;
                                    table.AddCell(Headercell3);

                                    PdfPCell Headercell4 = new PdfPCell(new Phrase("FUp-Date", TblFontBold));
                                    // cell.BackgroundColor = new BaseColor(190, 120, 204);
                                    Headercell4.HorizontalAlignment = Element.ALIGN_CENTER;
                                    Headercell4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    Headercell4.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    Headercell4.BorderWidthBottom = 0.7f;
                                    table.AddCell(Headercell4);

                                    PdfPCell Headercell5 = new PdfPCell(new Phrase("Result", TblFontBold));
                                    // cell.BackgroundColor = new BaseColor(190, 120, 204);
                                    Headercell5.HorizontalAlignment = Element.ALIGN_CENTER;
                                    Headercell5.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                    Headercell5.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    Headercell5.BorderWidthBottom = 0.7f;
                                    table.AddCell(Headercell5);
                                    //table.HeaderRows = 2;
                                    Priv_Branch = Branch;
                                    First = false;

                                }

                                string CAMSType = dr["sp2_type"].ToString();

                                if (CAMSType == "CA")
                                {
                                    //List<CAMASTEntity> CAMASTList = new List<CAMASTEntity>();
                                    DataSet dsCAMAST = DatabaseLayer.SPAdminDB.Browse_CAMAST(null, dr["sp2_cams_code"].ToString().Trim(), null, null);
                                    if (dsCAMAST.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow drCAMAST = dsCAMAST.Tables[0].Rows[0];

                                        CAMSDesc = drCAMAST["CA_DESC"].ToString().Trim();
                                        //DataSet dsCaseAct=DatabaseLayer.SPAdminDB.Browse_CASEACT()
                                        CA_Entity.Agency = BaseForm.BaseAgency;
                                        CA_Entity.Dept = BaseForm.BaseDept;
                                        CA_Entity.Program = BaseForm.BaseProg;


                                        //CA_Pass_Entity.Year = BaseYear;                        
                                        CA_Entity.Year = Spm_Year;                             // Year will be always Four-Spaces in CASEACT
                                        CA_Entity.App_no = BaseForm.BaseApplicationNo;
                                        CA_Entity.ACT_Code = dr["sp2_cams_code"].ToString().Trim();
                                        CA_Entity.Service_plan = SerVicePlan;
                                        CA_Entity.Branch = Branch.Trim(); CA_Entity.Group = dr["sp2_orig_grp"].ToString().Trim();
                                        CA_Entity.ACT_Date = CA_Entity.ACT_Seq = CA_Entity.Site = CA_Entity.Fund1 = null;
                                        CA_Entity.Fund2 = CA_Entity.Fund3 = CA_Entity.Caseworker = CA_Entity.Vendor_No = null;
                                        CA_Entity.Check_Date = CA_Entity.Check_No = CA_Entity.Cost = CA_Entity.Followup_On = null;
                                        CA_Entity.Followup_Comp = CA_Entity.Followup_By = CA_Entity.Refer_Data = CA_Entity.Cust_Code1 = null;
                                        CA_Entity.Cust_Value1 = CA_Entity.Cust_Code2 = CA_Entity.Cust_Value2 = CA_Entity.Cust_Code3 = null;
                                        CA_Entity.Cust_Value3 = CA_Entity.Lstc_Date = CA_Entity.Lsct_Operator = CA_Entity.Add_Date = null;
                                        CA_Entity.Add_Operator = CA_Entity.ACT_ID = null; CA_Entity.Bulk = CA_Entity.Act_PROG = null;
                                        CA_Entity.Cust_Code4 = CA_Entity.Cust_Value4 = CA_Entity.Cust_Code5 = CA_Entity.Cust_Value5 = null;
                                        CA_Entity.Units = CA_Entity.UOM = CA_Entity.Curr_Grp = null;
                                        CA_Entity.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013

                                        CaseAct_Details = _model.SPAdminData.Browse_CASEACT(CA_Entity, "Browse");
                                        CaseAct_Details = CaseAct_Details.OrderByDescending(u => Convert.ToDateTime(u.ACT_Date.Trim())).ToList();
                                        if (CaseAct_Details.Count > 0)
                                        {
                                            string Priv_Type = null, Priv_Cams_Desc = null;
                                            foreach (CASEACTEntity entity in CaseAct_Details)
                                            {
                                                CaDate = LookupDataAccess.Getdate(entity.ACT_Date).ToString();
                                                CaDate_Follow_on = LookupDataAccess.Getdate(entity.Followup_On).ToString();

                                                if (CAMSType.Trim() != Priv_Type)
                                                {
                                                    PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowType.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowType);
                                                    Priv_Type = CAMSType.Trim();
                                                }
                                                else
                                                {
                                                    PdfPCell RowType = new PdfPCell(new Phrase("", TableFont));
                                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowType.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowType);
                                                }
                                                if (CAMSDesc.Trim() != Priv_Cams_Desc)
                                                {
                                                    PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowDesc.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowDesc);
                                                    Priv_Cams_Desc = CAMSDesc.Trim();
                                                }
                                                else
                                                {
                                                    PdfPCell RowDesc = new PdfPCell(new Phrase("", TableFont));
                                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowDesc.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowDesc);
                                                    Priv_Cams_Desc = CAMSDesc.Trim();
                                                }
                                                PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                                RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate);

                                                PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                                RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate_Foolow);

                                                PdfPCell RowDate_Result = new PdfPCell(new Phrase("", TableFont));
                                                RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate_Result);
                                            }
                                        }
                                        else
                                        {

                                            PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                            RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowType.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowType.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowType);

                                            PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                            RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDesc.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDesc);

                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Foolow);

                                            PdfPCell RowDate_Result = new PdfPCell(new Phrase("", TableFont));
                                            RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Result);
                                        }

                                    }
                                }
                                else
                                {
                                    DataSet MSMast = DatabaseLayer.SPAdminDB.Browse_MSMAST(null, dr["sp2_cams_code"].ToString().Trim(), null, null, null);

                                    if (MSMast.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow drMSMast = MSMast.Tables[0].Rows[0];


                                        CAMSDesc = drMSMast["MS_DESC"].ToString().Trim();
                                        string MSType = drMSMast["MS_TYPE"].ToString();
                                        string Type_Desc = string.Empty;
                                        //if (MSType == "M")
                                        //    Type_Desc = "Milestone";
                                        //else 
                                            Type_Desc = "Outcome";

                                        Search_MS_Details.Agency = BaseForm.BaseAgency;
                                        Search_MS_Details.Dept = BaseForm.BaseDept;
                                        Search_MS_Details.Program = BaseForm.BaseProg;
                                        //Search_MS_Details.Year = BaseYear; 
                                        Search_MS_Details.Year = Spm_Year;                              // Year will be always Four-Spaces in CASEMS
                                        Search_MS_Details.App_no = BaseForm.BaseApplicationNo;
                                        Search_MS_Details.MS_Code = dr["sp2_cams_code"].ToString().Trim();
                                        Search_MS_Details.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));

                                        Search_MS_Details.Service_plan = SerVicePlan;
                                        Search_MS_Details.Branch = Branch.Trim(); Search_MS_Details.Group = dr["sp2_orig_grp"].ToString().Trim();
                                        Search_MS_Details.ID = Search_MS_Details.Date = Search_MS_Details.CaseWorker = Search_MS_Details.Site = null;
                                        Search_MS_Details.Result = Search_MS_Details.OBF = Search_MS_Details.Add_Operator = null;
                                        Search_MS_Details.Lstc_Date = Search_MS_Details.Lsct_Operator = Search_MS_Details.Add_Date = Search_MS_Details.Bulk =
                                        Search_MS_Details.Acty_PROG = Search_MS_Details.Curr_Grp = null;

                                        CaseMS_Details = _model.SPAdminData.Browse_CASEMS(Search_MS_Details, "Browse");
                                        CaseMS_Details = CaseMS_Details.OrderByDescending(u => Convert.ToDateTime(u.Date.Trim())).ToList();
                                        if (CaseMS_Details.Count > 0)
                                        {

                                            foreach (CASEMSEntity entity in CaseMS_Details)
                                            {
                                                CaDate = LookupDataAccess.Getdate(entity.Date);
                                                CaDate_Follow_on = LookupDataAccess.Getdate(entity.MS_FUP_Date).ToString();

                                                PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                                RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowType.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowType);

                                                PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                                RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDesc.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDesc);

                                                PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                                RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate);

                                                PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                                RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate_Foolow);

                                                PdfPCell RowDate_Result = new PdfPCell(new Phrase(MsResultDescription(entity.Result.Trim()), TableFont));
                                                RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate_Result);
                                            }
                                        }
                                        else
                                        {
                                            PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                            RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowType.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowType.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowType);

                                            PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                            RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDesc.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDesc);

                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Foolow);

                                            PdfPCell RowDate_Result = new PdfPCell(new Phrase("", TableFont));
                                            RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Result);
                                        }

                                    }
                                }

                            }
                        }
                        else
                        {
                            First = false;
                        }
                        i++;
                    }

                    if (drSP_Services["SP0_ALLOW_ADLBRANCH"].ToString() == "Y")
                    {
                        List<CASESPM2Entity> casespm2List = new List<CASESPM2Entity>();
                        CASESPM2Entity Search_Entity2 = new CASESPM2Entity();

                        Search_Entity2.Agency = Hierarchy.Substring(0, 2);
                        Search_Entity2.Dept = Hierarchy.Substring(2, 2);
                        Search_Entity2.Prog = Hierarchy.Substring(4, 2);
                        Search_Entity2.Year = BaseForm.BaseYear;
                        Search_Entity2.Year = Spm_Year;                         // Year will be always Four-Spaces in CASESPM2
                        Search_Entity2.App = App_No;
                        Search_Entity2.Spm_Seq = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString();

                        Search_Entity2.ServPlan = Search_Entity2.Branch = Search_Entity2.Group = null;
                        Search_Entity2.Type1 = Search_Entity2.CamCd = Search_Entity2.Curr_Group = null;
                        Search_Entity2.SelOrdinal = Search_Entity2.DateLstc = Search_Entity2.lstcOperator = null;
                        Search_Entity2.Dateadd = Search_Entity2.addoperator = null;

                        Search_Entity2.ServPlan = SerVicePlan;

                        casespm2List = _model.SPAdminData.Browse_CASESPM2(Search_Entity2, "Browse");

                        if (casespm2List.Count > 0)
                        {
                            if (table.Rows.Count > 0)
                                document.Add(Header);
                            document.Add(table);
                            table.DeleteBodyRows();
                            document.NewPage();

                            PdfPCell SP_Desc_Header = new PdfPCell(new Phrase("Service: " + SP_Desc.Trim(), fc1));
                            SP_Desc_Header.HorizontalAlignment = Element.ALIGN_LEFT;
                            SP_Desc_Header.Colspan = 2;
                            SP_Desc_Header.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            table.AddCell(SP_Desc_Header);

                            PdfPCell SP_Desc_Date = new PdfPCell(new Phrase("Start Date: " + LookupDataAccess.Getdate(Entity.startdate.Trim()), fc1));
                            SP_Desc_Date.HorizontalAlignment = Element.ALIGN_RIGHT;
                            SP_Desc_Date.Colspan = 3;
                            SP_Desc_Date.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            table.AddCell(SP_Desc_Date);

                            PdfPCell ServiceDesc = new PdfPCell(new Phrase("Branch :" + "Additional Branch", TblFontBold));
                            ServiceDesc.HorizontalAlignment = Element.ALIGN_CENTER;
                            ServiceDesc.Colspan = 5;
                            ServiceDesc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            table.AddCell(ServiceDesc);


                            PdfPCell Headercell1 = new PdfPCell(new Phrase("Type", TblFontBold));
                            Headercell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            Headercell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            Headercell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                            Headercell1.BorderWidthBottom = 0.7f;
                            table.AddCell(Headercell1);

                            PdfPCell Headercell2 = new PdfPCell(new Phrase("Description", TblFontBold));
                            // cell.BackgroundColor = new BaseColor(190, 120, 204);
                            Headercell2.HorizontalAlignment = Element.ALIGN_CENTER;
                            Headercell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            Headercell2.BackgroundColor = BaseColor.LIGHT_GRAY;
                            Headercell2.BorderWidthBottom = 0.7f;
                            table.AddCell(Headercell2);

                            PdfPCell Headercell3 = new PdfPCell(new Phrase("Date", TblFontBold));
                            // cell.BackgroundColor = new BaseColor(190, 120, 204);
                            Headercell3.HorizontalAlignment = Element.ALIGN_CENTER;
                            Headercell3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            Headercell3.BackgroundColor = BaseColor.LIGHT_GRAY;
                            Headercell3.BorderWidthBottom = 0.7f;
                            table.AddCell(Headercell3);

                            PdfPCell Headercell4 = new PdfPCell(new Phrase("FUp-Date", TblFontBold));
                            // cell.BackgroundColor = new BaseColor(190, 120, 204);
                            Headercell4.HorizontalAlignment = Element.ALIGN_CENTER;
                            Headercell4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            Headercell4.BackgroundColor = BaseColor.LIGHT_GRAY;
                            Headercell4.BorderWidthBottom = 0.7f;
                            table.AddCell(Headercell4);

                            PdfPCell Headercell5 = new PdfPCell(new Phrase("Result", TblFontBold));
                            // cell.BackgroundColor = new BaseColor(190, 120, 204);
                            Headercell5.HorizontalAlignment = Element.ALIGN_CENTER;
                            Headercell5.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            Headercell5.BackgroundColor = BaseColor.LIGHT_GRAY;
                            Headercell5.BorderWidthBottom = 0.7f;
                            table.AddCell(Headercell5);

                            foreach (CASESPM2Entity Spm2 in casespm2List)
                            {
                                string CAMSType = Spm2.Type1; CaDate_Follow_on = null;
                                CaDate = null;
                                if (CAMSType == "CA")
                                {
                                    //List<CAMASTEntity> CAMASTList = new List<CAMASTEntity>();
                                    DataSet dsCAMAST = DatabaseLayer.SPAdminDB.Browse_CAMAST(null, Spm2.CamCd.Trim(), null, null);
                                    if (dsCAMAST.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow drCAMAST = dsCAMAST.Tables[0].Rows[0];

                                        CAMSDesc = drCAMAST["CA_DESC"].ToString().Trim();
                                        //DataSet dsCaseAct=DatabaseLayer.SPAdminDB.Browse_CASEACT()
                                        CA_Entity.Agency = BaseForm.BaseAgency;
                                        CA_Entity.Dept = BaseForm.BaseDept;
                                        CA_Entity.Program = BaseForm.BaseProg;


                                        //CA_Pass_Entity.Year = BaseYear;                        
                                        CA_Entity.Year = Spm_Year;                             // Year will be always Four-Spaces in CASEACT
                                        CA_Entity.App_no = BaseForm.BaseApplicationNo;
                                        CA_Entity.ACT_Code = Spm2.CamCd.Trim().Trim();
                                        CA_Entity.Service_plan = SerVicePlan;
                                        CA_Entity.Branch = Spm2.Branch; CA_Entity.Group = Spm2.Group.Trim();
                                        CA_Entity.ACT_Date = CA_Entity.ACT_Seq = CA_Entity.Site = CA_Entity.Fund1 = null;
                                        CA_Entity.Fund2 = CA_Entity.Fund3 = CA_Entity.Caseworker = CA_Entity.Vendor_No = null;
                                        CA_Entity.Check_Date = CA_Entity.Check_No = CA_Entity.Cost = CA_Entity.Followup_On = null;
                                        CA_Entity.Followup_Comp = CA_Entity.Followup_By = CA_Entity.Refer_Data = CA_Entity.Cust_Code1 = null;
                                        CA_Entity.Cust_Value1 = CA_Entity.Cust_Code2 = CA_Entity.Cust_Value2 = CA_Entity.Cust_Code3 = null;
                                        CA_Entity.Cust_Value3 = CA_Entity.Lstc_Date = CA_Entity.Lsct_Operator = CA_Entity.Add_Date = null;
                                        CA_Entity.Add_Operator = CA_Entity.ACT_ID = null; CA_Entity.Bulk = CA_Entity.Act_PROG = null;
                                        CA_Entity.Cust_Code4 = CA_Entity.Cust_Value4 = CA_Entity.Cust_Code5 = CA_Entity.Cust_Value5 = null;
                                        CA_Entity.Units = CA_Entity.UOM = CA_Entity.Curr_Grp = null;
                                        CA_Entity.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013
                                        CaseAct_Details = _model.SPAdminData.Browse_CASEACT(CA_Entity, "Browse");
                                        if (CaseAct_Details.Count > 0)
                                        {
                                            string Priv_Type = null, Priv_Cams_Desc = null;
                                            foreach (CASEACTEntity entity in CaseAct_Details)
                                            {
                                                CaDate = LookupDataAccess.Getdate(entity.ACT_Date).ToString();
                                                CaDate_Follow_on = LookupDataAccess.Getdate(entity.Followup_On).ToString();

                                                if (CAMSType.Trim() != Priv_Type)
                                                {
                                                    PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowType.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowType);
                                                    Priv_Type = CAMSType.Trim();
                                                }
                                                else
                                                {
                                                    PdfPCell RowType = new PdfPCell(new Phrase("", TableFont));
                                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowType.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowType);
                                                }
                                                if (CAMSDesc.Trim() != Priv_Cams_Desc)
                                                {
                                                    PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowDesc.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowDesc);
                                                    Priv_Cams_Desc = CAMSDesc.Trim();
                                                }
                                                else
                                                {
                                                    PdfPCell RowDesc = new PdfPCell(new Phrase("", TableFont));
                                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                    //RowDesc.BorderWidthBottom = 0.7f;
                                                    table.AddCell(RowDesc);
                                                    Priv_Cams_Desc = CAMSDesc.Trim();
                                                }
                                                PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                                RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate);

                                                PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                                RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate_Foolow);

                                                PdfPCell RowDate_Result = new PdfPCell(new Phrase("", TableFont));
                                                RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDate_Result);
                                            }
                                        }
                                        else
                                        {

                                            PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                            RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowType.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowType.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowType);

                                            PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                            RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDesc.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDesc);

                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Foolow);

                                            PdfPCell RowDate_Result = new PdfPCell(new Phrase("", TableFont));
                                            RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Result);
                                        }

                                    }
                                }
                                else
                                {
                                    DataSet MSMast = DatabaseLayer.SPAdminDB.Browse_MSMAST(null, Spm2.CamCd.Trim(), null, null, null);
                                    string Type_Desc = string.Empty;
                                    if (MSMast.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow drMSMast = MSMast.Tables[0].Rows[0];


                                        CAMSDesc = drMSMast["MS_DESC"].ToString().Trim();
                                        string MSType = drMSMast["MS_TYPE"].ToString();

                                        //if (MSType == "M")
                                        //    Type_Desc = "Milestone";
                                        //else 
                                            Type_Desc = "Outcome";
                                    }
                                    else { Type_Desc = "Outcome"; CAMSDesc = string.Empty; }

                                    Search_MS_Details.Agency = BaseForm.BaseAgency;
                                    Search_MS_Details.Dept = BaseForm.BaseDept;
                                    Search_MS_Details.Program = BaseForm.BaseProg;
                                    //Search_MS_Details.Year = BaseYear; 
                                    Search_MS_Details.Year = Spm_Year;                              // Year will be always Four-Spaces in CASEMS
                                    Search_MS_Details.App_no = BaseForm.BaseApplicationNo;
                                    Search_MS_Details.MS_Code = Spm2.CamCd.Trim().Trim();
                                    Search_MS_Details.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString()));

                                    Search_MS_Details.Service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString().Trim();
                                    Search_MS_Details.Branch = Spm2.Branch; Search_MS_Details.Group = Spm2.Group.Trim();
                                    Search_MS_Details.ID = Search_MS_Details.Date = Search_MS_Details.CaseWorker = Search_MS_Details.Site = null;
                                    Search_MS_Details.Result = Search_MS_Details.OBF = Search_MS_Details.Add_Operator = null;
                                    Search_MS_Details.Lstc_Date = Search_MS_Details.Lsct_Operator = Search_MS_Details.Add_Date = Search_MS_Details.Bulk =
                                    Search_MS_Details.Acty_PROG = Search_MS_Details.Curr_Grp = null;

                                    CaseMS_Details = _model.SPAdminData.Browse_CASEMS(Search_MS_Details, "Browse");
                                    if (CaseMS_Details.Count > 0)
                                    {

                                        foreach (CASEMSEntity entity in CaseMS_Details)
                                        {
                                            CaDate = LookupDataAccess.Getdate(entity.Date);
                                            CaDate_Follow_on = LookupDataAccess.Getdate(entity.MS_FUP_Date).ToString();

                                            PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                            RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowType.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowType.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowType);

                                            PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                            RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDesc.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDesc);

                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Foolow);

                                            PdfPCell RowDate_Result = new PdfPCell(new Phrase(MsResultDescription(entity.Result.Trim()), TableFont));
                                            RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Result);
                                        }
                                    }
                                    else
                                    {
                                        PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                        RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowType.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowType.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowType);

                                        PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                        RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDesc.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDesc);

                                        PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                        RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate);

                                        PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                        RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate_Foolow);

                                        PdfPCell RowDate_Result = new PdfPCell(new Phrase("", TableFont));
                                        RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate_Result);
                                    }

                                    //}
                                }
                            }


                        }

                    }
                }
            }
            else
            {
                DataSet dsSP_Services = DatabaseLayer.SPAdminDB.Browse_CASESP0(SerVicePlan, null, null, null, null, null, null, null, null);
                DataRow drSP_Services = dsSP_Services.Tables[0].Rows[0];
                SP_Desc = drSP_Services["sp0_description"].ToString().Trim();
                if (drSP_Services["SP0_ALLOW_ADLBRANCH"].ToString() == "Y")
                {
                    List<CASESPM2Entity> casespm2List = new List<CASESPM2Entity>();
                    CASESPM2Entity Search_Entity2 = new CASESPM2Entity();

                    Search_Entity2.Agency = Hierarchy.Substring(0, 2);
                    Search_Entity2.Dept = Hierarchy.Substring(2, 2);
                    Search_Entity2.Prog = Hierarchy.Substring(4, 2);
                    Search_Entity2.Year = Spm_Year;
                    //Search_Entity2.Year = null;                         // Year will be always Four-Spaces in CASESPM2
                    Search_Entity2.App = App_No;
                    Search_Entity2.Spm_Seq = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString();

                    Search_Entity2.ServPlan = Search_Entity2.Branch = Search_Entity2.Group = null;
                    Search_Entity2.Type1 = Search_Entity2.CamCd = Search_Entity2.Curr_Group = null;
                    Search_Entity2.SelOrdinal = Search_Entity2.DateLstc = Search_Entity2.lstcOperator = null;
                    Search_Entity2.Dateadd = Search_Entity2.addoperator = null;

                    Search_Entity2.ServPlan = SerVicePlan;

                    casespm2List = _model.SPAdminData.Browse_CASESPM2(Search_Entity2, "Browse");

                    if (casespm2List.Count > 0)
                    {
                        if (table.Rows.Count > 0)
                            document.Add(Header);
                        //document.Add(table);
                        //table.DeleteBodyRows();
                        //document.NewPage();

                        PdfPCell SP_Desc_Header = new PdfPCell(new Phrase("Service: " + SP_Desc.Trim(), fc1));
                        SP_Desc_Header.HorizontalAlignment = Element.ALIGN_CENTER;
                        SP_Desc_Header.Colspan = 5;
                        SP_Desc_Header.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        table.AddCell(SP_Desc_Header);

                        PdfPCell ServiceDesc = new PdfPCell(new Phrase("Branch :" + "Additional Branch", TblFontBold));
                        ServiceDesc.HorizontalAlignment = Element.ALIGN_CENTER;
                        ServiceDesc.Colspan = 5;
                        ServiceDesc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        table.AddCell(ServiceDesc);


                        PdfPCell Headercell1 = new PdfPCell(new Phrase("Type", TblFontBold));
                        Headercell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        Headercell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        Headercell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                        Headercell1.BorderWidthBottom = 0.7f;
                        table.AddCell(Headercell1);

                        PdfPCell Headercell2 = new PdfPCell(new Phrase("Description", TblFontBold));
                        // cell.BackgroundColor = new BaseColor(190, 120, 204);
                        Headercell2.HorizontalAlignment = Element.ALIGN_CENTER;
                        Headercell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        Headercell2.BackgroundColor = BaseColor.LIGHT_GRAY;
                        Headercell2.BorderWidthBottom = 0.7f;
                        table.AddCell(Headercell2);

                        PdfPCell Headercell3 = new PdfPCell(new Phrase("Date", TblFontBold));
                        // cell.BackgroundColor = new BaseColor(190, 120, 204);
                        Headercell3.HorizontalAlignment = Element.ALIGN_CENTER;
                        Headercell3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        Headercell3.BackgroundColor = BaseColor.LIGHT_GRAY;
                        Headercell3.BorderWidthBottom = 0.7f;
                        table.AddCell(Headercell3);

                        PdfPCell Headercell4 = new PdfPCell(new Phrase("FUp-Date", TblFontBold));
                        // cell.BackgroundColor = new BaseColor(190, 120, 204);
                        Headercell4.HorizontalAlignment = Element.ALIGN_CENTER;
                        Headercell4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        Headercell4.BackgroundColor = BaseColor.LIGHT_GRAY;
                        Headercell4.BorderWidthBottom = 0.7f;
                        table.AddCell(Headercell4);

                        PdfPCell Headercell5 = new PdfPCell(new Phrase("Result", TblFontBold));
                        // cell.BackgroundColor = new BaseColor(190, 120, 204);
                        Headercell5.HorizontalAlignment = Element.ALIGN_CENTER;
                        Headercell5.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        Headercell5.BackgroundColor = BaseColor.LIGHT_GRAY;
                        Headercell5.BorderWidthBottom = 0.7f;
                        table.AddCell(Headercell5);

                        foreach (CASESPM2Entity Spm2 in casespm2List)
                        {
                            string CAMSType = Spm2.Type1;

                            if (CAMSType == "CA")
                            {
                                //List<CAMASTEntity> CAMASTList = new List<CAMASTEntity>();
                                DataSet dsCAMAST = DatabaseLayer.SPAdminDB.Browse_CAMAST(null, Spm2.CamCd.Trim(), null, null);
                                if (dsCAMAST.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drCAMAST = dsCAMAST.Tables[0].Rows[0];

                                    CAMSDesc = drCAMAST["CA_DESC"].ToString().Trim();
                                    //DataSet dsCaseAct=DatabaseLayer.SPAdminDB.Browse_CASEACT()
                                    CA_Entity.Agency = BaseForm.BaseAgency;
                                    CA_Entity.Dept = BaseForm.BaseDept;
                                    CA_Entity.Program = BaseForm.BaseProg;


                                    //CA_Pass_Entity.Year = BaseYear;                        
                                    CA_Entity.Year = Spm_Year;                             // Year will be always Four-Spaces in CASEACT
                                    CA_Entity.App_no = BaseForm.BaseApplicationNo;
                                    CA_Entity.ACT_Code = Spm2.CamCd.Trim().Trim();
                                    CA_Entity.Service_plan = SerVicePlan;
                                    CA_Entity.Branch = Spm2.Branch; CA_Entity.Group = Spm2.Group.Trim();
                                    CA_Entity.ACT_Date = CA_Entity.ACT_Seq = CA_Entity.Site = CA_Entity.Fund1 = null;
                                    CA_Entity.Fund2 = CA_Entity.Fund3 = CA_Entity.Caseworker = CA_Entity.Vendor_No = null;
                                    CA_Entity.Check_Date = CA_Entity.Check_No = CA_Entity.Cost = CA_Entity.Followup_On = null;
                                    CA_Entity.Followup_Comp = CA_Entity.Followup_By = CA_Entity.Refer_Data = CA_Entity.Cust_Code1 = null;
                                    CA_Entity.Cust_Value1 = CA_Entity.Cust_Code2 = CA_Entity.Cust_Value2 = CA_Entity.Cust_Code3 = null;
                                    CA_Entity.Cust_Value3 = CA_Entity.Lstc_Date = CA_Entity.Lsct_Operator = CA_Entity.Add_Date = null;
                                    CA_Entity.Add_Operator = CA_Entity.ACT_ID = null; CA_Entity.Bulk = CA_Entity.Act_PROG = null;
                                    CA_Entity.Cust_Code4 = CA_Entity.Cust_Value4 = CA_Entity.Cust_Code5 = CA_Entity.Cust_Value5 = null;
                                    CA_Entity.Units = CA_Entity.UOM = CA_Entity.Curr_Grp = null;
                                    CA_Pass_Entity.SPM_Seq = ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())); // Added By Yeswanth on 11/22/2013
                                    CaseAct_Details = _model.SPAdminData.Browse_CASEACT(CA_Entity, "Browse");
                                    if (CaseAct_Details.Count > 0)
                                    {
                                        string Priv_Type = null, Priv_Cams_Desc = null;
                                        foreach (CASEACTEntity entity in CaseAct_Details)
                                        {
                                            CaDate = LookupDataAccess.Getdate(entity.ACT_Date).ToString();
                                            CaDate_Follow_on = LookupDataAccess.Getdate(entity.Followup_On).ToString();

                                            if (CAMSType.Trim() != Priv_Type)
                                            {
                                                PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                                RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowType.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowType);
                                                Priv_Type = CAMSType.Trim();
                                            }
                                            else
                                            {
                                                PdfPCell RowType = new PdfPCell(new Phrase("", TableFont));
                                                RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowType.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowType);
                                            }
                                            if (CAMSDesc.Trim() != Priv_Cams_Desc)
                                            {
                                                PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                                RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDesc.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDesc);
                                                Priv_Cams_Desc = CAMSDesc.Trim();
                                            }
                                            else
                                            {
                                                PdfPCell RowDesc = new PdfPCell(new Phrase("", TableFont));
                                                RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                //RowDesc.BorderWidthBottom = 0.7f;
                                                table.AddCell(RowDesc);
                                                Priv_Cams_Desc = CAMSDesc.Trim();
                                            }
                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Foolow);

                                            PdfPCell RowDate_Result = new PdfPCell(new Phrase("", TableFont));
                                            RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                            //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                            table.AddCell(RowDate_Result);
                                        }
                                    }
                                    else
                                    {

                                        PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                        RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowType.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowType.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowType);

                                        PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                        RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDesc.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDesc);

                                        PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                        RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate);

                                        PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                        RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate_Foolow);

                                        PdfPCell RowDate_Result = new PdfPCell(new Phrase("", TableFont));
                                        RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate_Result);
                                    }

                                }
                            }
                            else
                            {
                                DataSet MSMast = DatabaseLayer.SPAdminDB.Browse_MSMAST(null, Spm2.CamCd.Trim(), null, null, null);
                                string Type_Desc = string.Empty;
                                if (MSMast.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drMSMast = MSMast.Tables[0].Rows[0];


                                    CAMSDesc = drMSMast["MS_DESC"].ToString().Trim();
                                    string MSType = drMSMast["MS_TYPE"].ToString();

                                    //if (MSType == "M")
                                    //    Type_Desc = "Milestone";
                                    //else 
                                        Type_Desc = "Outcome";
                                }
                                else
                                {
                                    Type_Desc = "Outcome"; CAMSDesc = string.Empty;
                                }

                                Search_MS_Details.Agency = BaseForm.BaseAgency;
                                Search_MS_Details.Dept = BaseForm.BaseDept;
                                Search_MS_Details.Program = BaseForm.BaseProg;
                                //Search_MS_Details.Year = BaseYear; 
                                Search_MS_Details.Year = Spm_Year;                              // Year will be always Four-Spaces in CASEMS
                                Search_MS_Details.App_no = BaseForm.BaseApplicationNo;
                                Search_MS_Details.MS_Code = Spm2.CamCd.Trim().Trim();
                                Search_MS_Details.SPM_Seq = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString();
                                Search_MS_Details.Service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString().Trim();
                                Search_MS_Details.Branch = Spm2.Branch; Search_MS_Details.Group = Spm2.Group.Trim();
                                Search_MS_Details.ID = Search_MS_Details.Date = Search_MS_Details.CaseWorker = Search_MS_Details.Site = null;
                                Search_MS_Details.Result = Search_MS_Details.OBF = Search_MS_Details.Add_Operator = null;
                                Search_MS_Details.Lstc_Date = Search_MS_Details.Lsct_Operator = Search_MS_Details.Add_Date = Search_MS_Details.Bulk =
                                Search_MS_Details.Acty_PROG = Search_MS_Details.Curr_Grp = null;

                                CaseMS_Details = _model.SPAdminData.Browse_CASEMS(Search_MS_Details, "Browse");
                                if (CaseMS_Details.Count > 0)
                                {

                                    foreach (CASEMSEntity entity in CaseMS_Details)
                                    {
                                        CaDate = LookupDataAccess.Getdate(entity.Date);
                                        CaDate_Follow_on = LookupDataAccess.Getdate(entity.MS_FUP_Date).ToString();

                                        PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                        RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowType.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowType.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowType);

                                        PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                        RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDesc.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDesc);

                                        PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                        RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate);

                                        PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                        RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate_Foolow);

                                        PdfPCell RowDate_Result = new PdfPCell(new Phrase(MsResultDescription(entity.Result.Trim()), TableFont));
                                        RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                        RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                        //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                        table.AddCell(RowDate_Result);
                                    }
                                }
                                else
                                {
                                    PdfPCell RowType = new PdfPCell(new Phrase(Type_Desc, TblFontBold));
                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                    //RowType.BorderWidthBottom = 0.7f;
                                    table.AddCell(RowType);

                                    PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TblFontBold));
                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                    //RowDesc.BorderWidthBottom = 0.7f;
                                    table.AddCell(RowDesc);

                                    PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TblFontBold));
                                    RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                    RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                    //RowDate.BorderWidthBottom = 0.7f;
                                    table.AddCell(RowDate);

                                    PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TblFontBold));
                                    RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                    RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                    //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                    table.AddCell(RowDate_Foolow);

                                    PdfPCell RowDate_Result = new PdfPCell(new Phrase("", TableFont));
                                    RowDate_Result.HorizontalAlignment = Element.ALIGN_LEFT;
                                    RowDate_Result.Border = iTextSharp.text.Rectangle.BOX;
                                    //RowDate_Foolow.BorderWidthBottom = 0.7f;
                                    table.AddCell(RowDate_Result);
                                }

                                //}
                            }
                        }


                    }

                }
            }
            if (table.Rows.Count > 0)
                document.Add(Header);
            document.Add(table);
            document.Close();
            fs.Close();
            fs.Dispose();

            if (BaseForm.BaseAgencyControlDetails.ReportSwitch.ToUpper() == "Y")
            {
                PdfViewerNewForm objfrm = new PdfViewerNewForm(PdfName);
                objfrm.FormClosed += new FormClosedEventHandler(On_Delete_PDF_File);
                objfrm.StartPosition = FormStartPosition.CenterScreen;
                objfrm.ShowDialog();
            }
            else
            {
                FrmViewer objfrm = new FrmViewer(PdfName);
                objfrm.FormClosed += new FormClosedEventHandler(On_Delete_PDF_File);
                objfrm.StartPosition = FormStartPosition.CenterScreen;
                objfrm.ShowDialog();
            }
        }


        private void On_Delete_PDF_File(object sender, FormClosedEventArgs e)
        {
            System.IO.File.Delete(PdfName);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            On_SaveForm_Closed();
        }

        private void Btn_Bulk_Posting_Click(object sender, EventArgs e)
        {
            bool Delete_Status = true;
            int New_MS_ID = 0, Tmp_CA_Seq = 0;

            List<Captain.Common.Utilities.ListItem> listItem = new List<Captain.Common.Utilities.ListItem>();
            foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
            {
                if (dr.Cells["Del_1"].Value.ToString() == true.ToString())
                    listItem.Add(new Captain.Common.Utilities.ListItem(dr.Cells["SP2_Type"].Value.ToString().Trim() + dr.Cells["SP2_CAMS_Code"].Value.ToString().Trim() + dr.Cells["SP2_Group"].Value.ToString().Trim().Trim() + dr.Cells["SP2_CAMS_Branch"].Value.ToString().Trim(), dr.Cells["SP2_CAMS_ID"].Value.ToString().Trim(), dr.Cells["SP2_Comp_Date"].Value.ToString().Trim(), string.Empty));
            }

            if (listItem.Count > 0)
            {
                string[] Split_Str = Regex.Split(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Text.ToString(), "-");
                string SP_Date = (!string.IsNullOrEmpty(Split_Str[1].Trim()) ? Split_Str[1] : "");

                //CASE0006_Bulk_Posting Bulk_Posting_Form = new CASE0006_Bulk_Posting(BaseForm, ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), Sp_Sequence, SP_Date, listItem);
                CASE0006_Bulk_Posting_Latest Bulk_Posting_Form = new CASE0006_Bulk_Posting_Latest(BaseForm, ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), Sp_Sequence, SP_Date, listItem, string.Empty);
                Bulk_Posting_Form.StartPosition = FormStartPosition.CenterScreen;
                Bulk_Posting_Form.ShowDialog();
            }
            else
                MessageBox.Show("Please select at least one activity to post", "CAP Systems");
        }


        private void Link_To_Add_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SP2_Panel.Visible = false;
            Sel_Del_Count = 0;

            this.SPM2_Panel.Size = new System.Drawing.Size(761, 244);
            this.ADD_CAMS_Grid.Size = new System.Drawing.Size(739, 213);

            this.panel4.Size = new System.Drawing.Size(765, 24);
            this.Pb_SPM2_Max.Location = new System.Drawing.Point(744, 0);
            this.SPM2_Desc.Width = 540;
            this.SPM2_Comp_Date.Width = 66;
            SPM2_Panel.Visible = true;
        }

        private void ADD_CAMS_Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ADD_CAMS_Grid.Rows.Count > 0 && e.RowIndex != -1)
            {
                int ColIdx = ADD_CAMS_Grid.CurrentCell.ColumnIndex;
                int RowIdx = ADD_CAMS_Grid.CurrentCell.RowIndex;

                switch (e.ColumnIndex)
                {
                    case 0:
                        if (!Del_Col_Header_Clicked)
                            Select_All_Additional_To_Delete();
                        break;
                    case 4:
                        Add_Edit_CAMS_Details("Add");
                        break;
                }
                if (ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Type"].Value.ToString() == "CA" || ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Operation"].Value.ToString() == "A" && Privileges.AddPriv.Equals("true") && Sw_ReadOnly == "N")
                    Pb_SPM2_Add.Visible = true;
                else
                    Pb_SPM2_Add.Visible = false;

                if (ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Operation"].Value.ToString() == "C" && Privileges.ChangePriv.Equals("true"))
                    Pb_SPM2_Edit.Visible = true;
                else
                    Pb_SPM2_Edit.Visible = false;

                if (ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Operation"].Value.ToString() == "C" && Privileges.ChangePriv.Equals("true"))
                {

                    if (ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Notes_SW"].Value.ToString() == "Y")
                        Pb_SPM2_Notes.ImageSource = "captain-casenotes";
                    else
                        Pb_SPM2_Notes.ImageSource = "captain-casenotesadd"; ;
                    Pb_SPM2_Notes.Visible = true;
                }
                else
                    Pb_SPM2_Notes.Visible = false;

                //if (ADD_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C")
                //    Pb_Edit.Visible = true;
                //else
                //    Pb_Edit.Visible = false;
            }
        }

        private void Select_All_Additional_To_Delete()
        {
            if (ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Operation"].Value.ToString() == "C")
            {
                string Tmp = "false";
                Tmp = ADD_CAMS_Grid.CurrentRow.Cells["Del_2"].Value.ToString();
                if (Tmp == "True")
                {
                    ADD_CAMS_Grid.CurrentRow.Cells["Del_2"].Value = true;
                    Sel_Del_Count++;
                }
                else
                {
                    ADD_CAMS_Grid.CurrentRow.Cells["Del_2"].Value = false;
                    if (Sel_Del_Count > 0)
                        Sel_Del_Count--;
                }

                //if (Sel_Del_Count > 0 && Privileges.DelPriv.Equals("true"))
                //    PbDelete.Enabled = true;
                //else
                //    PbDelete.Enabled = false;

                //if (Privileges.DelPriv.Equals("true"))
                //    PbDelete.Visible = true;
                //else
                //    PbDelete.Visible = false;

            }

            Del_Col_Header_Clicked = false;
        }

        private void ADD_CAMS_Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (ADD_CAMS_Grid.Rows.Count > 0)
                Sel_SPM2_Notes_Key = Hierarchy + "    " + App_No + ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Notes_Key"].Value.ToString();
        }

        //string CAMA_Post_Mode = "Edit";
        private void Pb_SPM2_Add_Click(object sender, EventArgs e)
        {
            //string CAMA_Post_Mode = "Edit";
            CAMA_Post_Mode = "Edit";
            if (sender == Pb_Add_CA || sender == Pb_SPM2_Add)
                CAMA_Post_Mode = "Add";

            if (ADD_CAMS_Grid.Rows.Count > 0)
            {
                string Service_Type = "'Critical Activity'";
                if (ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Type"].Value.ToString() == "MS")
                    Service_Type = "'Milestone'";

                if (ADD_CAMS_Grid.CurrentRow.Cells["SPM2_CAMS_Active_Stat"].Value.ToString() == "False")
                    MessageBox.Show("You are suppose to Inactive " + Service_Type + " \n  Are you sure you want to post selected " + Service_Type, Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question, onclose:Inactive_CAMS_Posting_Confirmation) ;
                else
                    Add_Edit_Additional_CAMS_Details(CAMA_Post_Mode);
            }
        }



        private void Add_Edit_Additional_CAMS_Details(string Posting_Mode)  // sindhe
        {
            Prepare_Search_Entity("Additional",
                                  ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Type"].Value.ToString(),
                                  ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Operation"].Value.ToString(),
                                  ADD_CAMS_Grid.CurrentRow.Cells["SPM2_CAMS_Code"].Value.ToString(),
                                  Posting_Mode,
                                  ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Group"].Value.ToString().Trim(),
                                  "9",
                                  ADD_CAMS_Grid.CurrentRow.Cells["SPM2_CA_Seq"].Value.ToString().Trim(),
                                  ADD_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value.ToString().Trim());
            string[] Split_Str = Regex.Split(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Text.ToString(), "-");
            string SP_Date = (!string.IsNullOrEmpty(Split_Str[1].Trim()) ? Split_Str[1] : "");
            //string SPM_Site = ((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Value.ToString();
            string SPM_Site = string.Empty;
            if (!string.IsNullOrEmpty(CmbSite.Text))
                SPM_Site = ((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Value.ToString();

            string strEndDate = string.Empty;

            if (Act_Date.Checked == true)
            {
                strEndDate = Act_Date.Value.ToShortDateString();
            }

            if (ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Type"].Value.ToString() == "CA")
            {
                CASE2006_CAMSForm PostCA_Form;
                PostCA_Form = new CASE2006_CAMSForm(BaseForm, "CA", ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Dup_Desc"].Value.ToString(), Hierarchy, Year, CA_Pass_Entity, Privileges, App_MST_Entity.Site.Trim(), App_MST_Entity.IntakeWorker.Trim(), CntlCAEntity, SP_Header_Rec, Act_Template_List, SP_Date, SPM_Site, strEndDate);   // 08022012
                PostCA_Form.FormClosed += new FormClosedEventHandler(Add_Edit_Additional_CAMS_Closed);
                PostCA_Form.StartPosition = FormStartPosition.CenterScreen;
                PostCA_Form.ShowDialog();
            }
            else
            {
                CASE2006_CAMSForm PostMS_Form;
                PostMS_Form = new CASE2006_CAMSForm(BaseForm, "MS", ADD_CAMS_Grid.CurrentRow.Cells["SPM2_Dup_Desc"].Value.ToString(), Hierarchy, Year, MS_Pass_Entity, Privileges, App_MST_Entity.Site, App_MST_Entity.IntakeWorker, CntlMSEntity, SP_Header_Rec, MS_Template_List, SP_Date, SPM_Site, strEndDate);
                PostMS_Form.FormClosed += new FormClosedEventHandler(Add_Edit_Additional_CAMS_Closed);
                PostMS_Form.StartPosition = FormStartPosition.CenterScreen;
                PostMS_Form.ShowDialog();
            }
        }

        private void Link_To_SP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SPM2_Panel.Visible = false;
            Sel_Del_Count = 0;

            this.SP2_Panel.Location = new System.Drawing.Point(2, -1);
            this.SP2_Panel.Size = new System.Drawing.Size(761, 244);
            this.SP_CAMS_Grid.Size = new System.Drawing.Size(739, 213);
            this.panel5.Size = new System.Drawing.Size(765, 24);

            this.Pb_SP2_Max.Location = new System.Drawing.Point(744, 0);
            // this.SP2_Desc.Width = 526;// 540;
            this.SP2_Desc.Width = 446;// 540;
            this.gvMsResult.Width = 80;
            this.SP2_Comp_Date.Width = 73;//66;
            this.SP2_Follow_Date.Width = 73;//66;

            SP2_Panel.Visible = true;
        }

        List<PopUp_Menu_L1_Entity> listItem_L1_New = new List<PopUp_Menu_L1_Entity>();
        List<PopUp_Menu_L2_Entity> listItem_L2_New = new List<PopUp_Menu_L2_Entity>();
        List<PopUp_Menu_L3_Entity> listItem_L3_New = new List<PopUp_Menu_L3_Entity>();

        List<PopUp_Menu_L1_Entity> listItem_L1_Menu = new List<PopUp_Menu_L1_Entity>();
        List<PopUp_Menu_L2_Entity> listItem_L2_Menu = new List<PopUp_Menu_L2_Entity>();
        List<PopUp_Menu_L3_Entity> listItem_L3_Menu = new List<PopUp_Menu_L3_Entity>();

        private void Prepare_Menu_items()
        {
            bool Group_Exists = false;
            listItem_L1_New.Clear();
            listItem_L2_New.Clear();
            listItem_L3_New.Clear();

            if (ADD_CAMA_Details.Count > 0)
            {
                foreach (CASESPM2Entity Entity in ADD_CAMA_Details)
                {
                    Group_Exists = false;

                    foreach (PopUp_Menu_L2_Entity Ent_2 in listItem_L2_New)
                    {
                        if (Ent_2.Grp_Code.ToString() == Entity.Curr_Group)
                        {
                            Group_Exists = true; break;
                        }
                    }

                    if (!Group_Exists)
                        listItem_L2_New.Add(new PopUp_Menu_L2_Entity(" ", int.Parse(Entity.Curr_Group)));    // Rao

                    listItem_L3_New.Add(new PopUp_Menu_L3_Entity(" ", Entity, 0));    // Rao
                }
            }

            listItem_L1_New.Add(new PopUp_Menu_L1_Entity("M", "Move"));
            if (listItem_L2_New.Count > 1)
                listItem_L1_New.Add(new PopUp_Menu_L1_Entity("C", "Copy"));
        }

        int Priv_Group = int.MaxValue, Curr_Group;
        private void contextMenu1_Popup(object sender, EventArgs e)
        {
            contextMenu1.MenuItems.Clear();

            if (SP_CAMS_Grid.Rows.Count > 0)
            {
                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value.ToString().Trim() != string.Empty)
                {
                    MenuItem Menu_L1 = new MenuItem();
                    Menu_L1.Text = "Quick Post Row";
                    Menu_L1.Tag = "Q";
                    contextMenu1.MenuItems.Add(Menu_L1);
                }
            }

            if (SP_CAMS_Grid.Rows.Count > 0 && Branches_Grid.CurrentRow.Cells["Branch_Code"].Value.ToString() == "9" && !Cb_Show_All_Postings.Checked)//&& ADD_CAMS_Grid.Rows) // Rao Test
            {
                listItem_L1_Menu.Clear();
                foreach (PopUp_Menu_L1_Entity Ent_1 in listItem_L1_New)
                {
                    MenuItem Menu_L1 = new MenuItem();
                    Menu_L1.Text = Ent_1.Cat_Desc;
                    Menu_L1.Tag = Ent_1.Cat_Code;
                    listItem_L1_Menu.Add(new PopUp_Menu_L1_Entity(Ent_1.Cat_Code, Ent_1.Cat_Desc));
                }

                listItem_L2_Menu.Clear();
                foreach (PopUp_Menu_L2_Entity Ent_2 in listItem_L2_New)
                {
                    MenuItem Menu_Move_L2 = new MenuItem();
                    Menu_Move_L2.Text = Ent_2.Grp_Desc;
                    Menu_Move_L2.Tag = "M " + Ent_2.Grp_Code;

                    if (Can_Add_Group_to_Menu(Ent_2.Grp_Code, "M"))
                    {
                        //contextMenu1.MenuItems[0].MenuItems.Add(Menu_Move_L2);
                        listItem_L2_Menu.Add(new PopUp_Menu_L2_Entity("M", Ent_2.Grp_Code));
                    }

                    if (listItem_L2_New.Count > 1)
                    {
                        MenuItem Menu_Copy_L2 = new MenuItem();
                        Menu_Copy_L2.Text = Ent_2.Grp_Desc;
                        Menu_Copy_L2.Tag = "C " + Ent_2.Grp_Code;
                        if (Curr_Group.ToString() != Ent_2.Grp_Code.ToString())
                        {
                            if (Can_Add_Group_to_Menu(Ent_2.Grp_Code, "M"))
                            {
                                //contextMenu1.MenuItems[1].MenuItems.Add(Menu_Copy_L2);
                                listItem_L2_Menu.Add(new PopUp_Menu_L2_Entity("C", Ent_2.Grp_Code));
                            }
                        }
                    }
                }

                int index = 0, Tmp_Cpy_index = 0;
                string Tmp_Desc = SP_CAMS_Grid.CurrentRow.Cells["SP2_Desc"].Value.ToString();
                bool Grp_Bypassed = false;

                listItem_L3_Menu.Clear();
                CASESP2Entity Tmp_Entity = new CASESP2Entity();
                foreach (PopUp_Menu_L2_Entity Ent_2 in listItem_L2_New)
                {
                    foreach (PopUp_Menu_L3_Entity Ent_3 in listItem_L3_New)
                    {
                        if (Ent_2.Grp_Code == Ent_3.Curr_Grp)
                        {
                            MenuItem Menu_Move_L3 = new MenuItem();

                            Menu_Move_L3.Text = Ent_3.CAMS_Seq + " " + Ent_3.Type + "   " + Ent_3.CAMS_Desc.Trim();
                            Menu_Move_L3.Tag = Ent_3.CAMS_Seq + " " + Ent_3.Type;

                            Tmp_Entity.Branch = Ent_3.Branch;
                            Tmp_Entity.Orig_Grp = Ent_3.Orig_Grp;
                            Tmp_Entity.Type1 = Ent_3.Type;
                            Tmp_Entity.CamCd = Ent_3.CAMS_Code;
                            Tmp_Entity.Row = Ent_3.CAMS_Seq;
                            Tmp_Entity.Curr_Grp = Ent_3.Curr_Grp;
                            Tmp_Entity.CAMS_Desc = Ent_3.CAMS_Desc;

                            if (Ent_3.CAMS_Code.Trim() != SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim() ||
                                Ent_3.Type != SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim())
                            {
                                listItem_L3_Menu.Add(new PopUp_Menu_L3_Entity("M", Tmp_Entity, Ent_2.Grp_Code));
                            }

                            if (Curr_Group != Ent_2.Grp_Code)                     // To Bypass Selected Group in Copy
                            {
                                listItem_L3_Menu.Add(new PopUp_Menu_L3_Entity("C", Tmp_Entity, Ent_2.Grp_Code));
                            }
                            else
                                Grp_Bypassed = true;
                        }
                    }
                    //index++;

                    //if (!Grp_Bypassed)
                    //    Tmp_Cpy_index++;
                }
                Priv_Group = Curr_Group;
                //} 09220212


                bool Can_Add_Move_T0_Menu = false, Can_Add_Copy_T0_Menu = false;
                foreach (PopUp_Menu_L1_Entity Ent_1 in listItem_L1_Menu)
                {
                    foreach (PopUp_Menu_L2_Entity Ent_2 in listItem_L2_Menu)
                    {
                        if (Can_Add_Move_T0_Menu && Can_Add_Copy_T0_Menu)
                            break;

                        if (Ent_1.Cat_Code == "M" && Ent_1.Cat_Code == Ent_2.Cat_Code)
                            Can_Add_Move_T0_Menu = true;

                        if (Ent_1.Cat_Code == "C" && Ent_1.Cat_Code == Ent_2.Cat_Code)
                            Can_Add_Copy_T0_Menu = true;
                    }
                }

                contextMenu1.MenuItems.Clear();
                foreach (PopUp_Menu_L1_Entity Ent_1 in listItem_L1_Menu)
                {
                    MenuItem Menu_L1 = new MenuItem();
                    Menu_L1.Text = Ent_1.Cat_Desc;
                    Menu_L1.Tag = Ent_1.Cat_Code;

                    if (Ent_1.Cat_Code == "M" && Can_Add_Move_T0_Menu)
                    {
                        contextMenu1.MenuItems.Add(Menu_L1);
                        Prepare_Move_Related_Groups();
                    }

                    if (Ent_1.Cat_Code == "C" && Can_Add_Copy_T0_Menu)
                    {
                        contextMenu1.MenuItems.Add(Menu_L1);
                        Prepare_Copy_Related_Groups();
                    }

                }

                int i = 0, j = 0; ;
                bool Sub_item_Added = false;
                foreach (PopUp_Menu_L2_Entity Ent_2 in listItem_L2_Menu)
                {
                    Sub_item_Added = false;
                    if (Ent_2.Cat_Code == "M")
                    {
                        foreach (PopUp_Menu_L3_Entity Ent_3 in listItem_L3_Menu)
                        {
                            if (Ent_2.Grp_Desc == Ent_3.Belongs_To && Ent_2.Cat_Code == Ent_3.Cat_Code)
                            {
                                MenuItem Menu_Move_L3 = new MenuItem();

                                Menu_Move_L3.Text = Ent_3.CAMS_Seq + " " + Ent_3.Type + "   " + Ent_3.CAMS_Desc.Trim();
                                Menu_Move_L3.Tag = Ent_3.CAMS_Seq + " " + Ent_3.Type;

                                contextMenu1.MenuItems[0].MenuItems[i].MenuItems.Add(Menu_Move_L3);    // To Bypass Selected CA/MS
                                Sub_item_Added = true;
                            }
                        }
                        if (Sub_item_Added)
                            i++;
                    }
                }


                foreach (PopUp_Menu_L2_Entity Ent_2 in listItem_L2_Menu)
                {
                    Sub_item_Added = false;
                    if (Ent_2.Cat_Code == "C")
                    {
                        foreach (PopUp_Menu_L3_Entity Ent_3 in listItem_L3_Menu)
                        {
                            if ((Ent_2.Grp_Desc == Ent_3.Belongs_To && Ent_2.Grp_Code.ToString() != SP_CAMS_Grid.CurrentRow.Cells["SP2_Curr_Grp"].Value.ToString().Trim()) &&
                                (Ent_2.Cat_Code == Ent_3.Cat_Code))
                            {
                                MenuItem Menu_Move_L3 = new MenuItem();

                                Menu_Move_L3.Text = Ent_3.CAMS_Seq + " " + Ent_3.Type + "   " + Ent_3.CAMS_Desc.Trim();
                                Menu_Move_L3.Tag = Ent_3.CAMS_Seq + " " + Ent_3.Type;

                                contextMenu1.MenuItems[1].MenuItems[j].MenuItems.Add(Menu_Move_L3);    // To Bypass Selected CA/MS
                                Sub_item_Added = true;
                            }
                        }
                        if (Sub_item_Added)
                            j++;
                    }
                }
            }
            System.Windows.Forms.VScrollBar vScroller = new System.Windows.Forms.VScrollBar();
        }


        private bool Can_Add_Group_to_Menu(int Grp_Code, string Category)
        {
            bool Can_Add = true;

            int i = 0;

            if (Category.Equals("M"))
            {
                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Curr_Grp"].Value.ToString() != Grp_Code.ToString())
                {
                    foreach (CASESPM2Entity Entity in ADD_CAMA_Details) // CAMA_Details) //////Branch_CAMS_Details) 
                    {

                        if (Grp_Code == int.Parse(Entity.Curr_Group) &&
                            Entity.CamCd.Trim() == SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim() &&
                            Entity.Type1 == SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim())// &&
                        {
                            Can_Add = false; break;
                        }
                    }
                }
                else
                {
                    foreach (CASESPM2Entity Entity in ADD_CAMA_Details)
                    {
                        if (Grp_Code == int.Parse(Entity.Curr_Group))
                            i++;
                    }
                    if (i <= 1)
                        Can_Add = false;
                }
            }
            return Can_Add;
        }

        private void Prepare_Move_Related_Groups()
        {
            //bool Can_Add_Move_T0_Menu = false, Can_Add_Copy_T0_Menu = false;
            contextMenu1.MenuItems[0].MenuItems.Clear();
            foreach (PopUp_Menu_L2_Entity Ent_2 in listItem_L2_Menu)
            {
                foreach (PopUp_Menu_L3_Entity Ent_3 in listItem_L3_Menu)
                {
                    if (Ent_2.Cat_Code == "M" && Ent_2.Cat_Code == Ent_3.Cat_Code && Ent_2.Grp_Code == Ent_3.Curr_Grp)
                    {
                        MenuItem Menu_Move_L2 = new MenuItem();
                        Menu_Move_L2.Text = Ent_2.Grp_Desc;
                        Menu_Move_L2.Tag = "M " + Ent_2.Grp_Code;

                        contextMenu1.MenuItems[0].MenuItems.Add(Menu_Move_L2); break;
                    }
                }
            }
        }

        private void Prepare_Copy_Related_Groups()
        {
            //bool Can_Add_Move_T0_Menu = false, Can_Add_Copy_T0_Menu = false;
            contextMenu1.MenuItems[1].MenuItems.Clear();
            foreach (PopUp_Menu_L2_Entity Ent_2 in listItem_L2_Menu)
            {
                foreach (PopUp_Menu_L3_Entity Ent_3 in listItem_L3_Menu)
                {
                    if (Ent_2.Cat_Code == "C" && Ent_2.Cat_Code == Ent_3.Cat_Code && Ent_2.Grp_Code == Ent_3.Curr_Grp &&
                        Ent_2.Grp_Code.ToString() != SP_CAMS_Grid.CurrentRow.Cells["SP2_Curr_Grp"].Value.ToString().Trim()) // 12052012 SP2_Group
                    {
                        MenuItem Menu_Move_L2 = new MenuItem();
                        Menu_Move_L2.Text = Ent_2.Grp_Desc;
                        Menu_Move_L2.Tag = "C " + Ent_2.Grp_Code;

                        contextMenu1.MenuItems[1].MenuItems.Add(Menu_Move_L2); break;
                    }
                }
            }
        }

        string[] CAMS_Move_Copy_Details = new string[4];
        private void ADD_CAMS_Grid_MenuClick(object objSource, MenuItemEventArgs objArgs)
        {
            CAMS_Move_Copy_Details[0] = CAMS_Move_Copy_Details[1] = CAMS_Move_Copy_Details[2] = CAMS_Move_Copy_Details[3] = null;
            string[] Split_Array = new string[2];

            if (objArgs.MenuItem.Tag is string)
            {
                Split_Array = Regex.Split(objArgs.MenuItem.Tag.ToString(), " ");
                CAMS_Move_Copy_Details[0] = Split_Array[0];         // Move to Sequence       
                CAMS_Move_Copy_Details[1] = Split_Array[1];         // Selected Record type

                Split_Array = null;
                Split_Array = Regex.Split(objArgs.MenuItem.Parent.Tag.ToString(), " ");
                CAMS_Move_Copy_Details[2] = Split_Array[0];        // Operation to Perform (Move 'M' / Coyp 'C')
                CAMS_Move_Copy_Details[3] = Split_Array[1];        // Current Group of Selected Record

                Move_Copy_SelectedCAMS();

                if (ADD_CAMA_Details.Count > 0 && Row_Shifted) //CAMA_Details
                {
                    foreach (CASESPM2Entity Entity in ADD_CAMA_Details) //CAMA_Details
                    {
                        if (_model.SPAdminData.UpdateCASESPM2(Entity, "RowChange", out Sql_SP_Result_Message, "RowChange"))
                            Row_Shifted = true;
                    }
                }

                if (Row_Shifted)
                {
                    //Update_CASESP0_Branches();
                    //Fill_SP_CAMS_Details_Grid();
                    //Fill_Branch_CAMS_Details_Grid();

                    //SP_Header_Rec.Validate = "N";
                    //BtnValidate.Visible = true;
                    //Refresh_ADMN20_Control = true;
                }
            }
        }

        bool Row_Shifted = false;
        private void Move_Copy_SelectedCAMS()
        {

            if (ADD_CAMA_Details.Count > 0)
            {
                List<CASESPM2Entity> New_CAMS_Details = new List<CASESPM2Entity>();

                Row_Shifted = false;
                string Original_Grp_Chec_Str = string.Empty;
                CASESPM2Entity New_Rec_To_CopyMove = new CASESPM2Entity();
                foreach (CASESPM2Entity Entity in ADD_CAMA_Details) // Collect Selected CA/MS To Move /Copy into New_Rec_To_CopyMove List
                {
                    if (Entity.CamCd.Trim() == SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim() &&
                        Entity.Type1 == SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim() &&
                        Entity.Group.ToString() == SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim()) // 12052013 SP2_Group Original Group
                    {
                        //New_Rec_To_CopyMove = Entity;

                        New_Rec_To_CopyMove.Agency = BaseForm.BaseAgency;
                        New_Rec_To_CopyMove.Dept = BaseForm.BaseDept;
                        New_Rec_To_CopyMove.Prog = BaseForm.BaseProg;
                        New_Rec_To_CopyMove.Year = Spm_Year;
                        New_Rec_To_CopyMove.App = BaseForm.BaseApplicationNo;

                        New_Rec_To_CopyMove.ServPlan = Entity.ServPlan;
                        New_Rec_To_CopyMove.Spm_Seq = Sp_Sequence;
                        New_Rec_To_CopyMove.Branch = Entity.Branch;
                        New_Rec_To_CopyMove.Type1 = Entity.Type1;
                        New_Rec_To_CopyMove.CamCd = Entity.CamCd;
                        New_Rec_To_CopyMove.SelOrdinal = Entity.SelOrdinal;
                        New_Rec_To_CopyMove.DateLstc = Entity.DateLstc;
                        New_Rec_To_CopyMove.lstcOperator = BaseForm.UserID;
                        New_Rec_To_CopyMove.Dateadd = Entity.Dateadd;
                        New_Rec_To_CopyMove.addoperator = Entity.addoperator;
                        if (CAMS_Move_Copy_Details[2] == "C")
                        {
                            New_Rec_To_CopyMove.Rec_Type = "I";
                            New_Rec_To_CopyMove.addoperator = New_Rec_To_CopyMove.addoperator = BaseForm.UserID;
                            New_Rec_To_CopyMove.Group = CAMS_Move_Copy_Details[3]; //int.Parse(CAMS_Move_Copy_Details[3]

                            Original_Grp_Chec_Str = string.Empty;
                            foreach (CASESPM2Entity Ent in ADD_CAMA_Details)
                            {
                                if (Ent.CamCd.Trim() == Entity.CamCd.Trim() && Ent.Type1 == Entity.Type1.Trim())
                                    Original_Grp_Chec_Str += Ent.Group.ToString() + ",";
                            }

                            if (Original_Grp_Chec_Str.Contains(New_Rec_To_CopyMove.Group))
                            {
                                for (int i = 1; ; i++)
                                {
                                    if (!Original_Grp_Chec_Str.Contains(i.ToString()))
                                    { New_Rec_To_CopyMove.Group = i.ToString(); break; }
                                }
                            }


                            //if (CAMS_Move_Copy_Details[3] == Entity.Group) //if (int.Parse(CAMS_Move_Copy_Details[3]) == Entity.Group)
                            //    New_Rec_To_CopyMove.Group = (int.Parse(New_Rec_To_CopyMove.Group) + 1).ToString();
                        }
                        else
                        {
                            New_Rec_To_CopyMove.Rec_Type = "U";
                            New_Rec_To_CopyMove.addoperator = BaseForm.UserID;
                            New_Rec_To_CopyMove.Group = Entity.Group;
                        }
                        New_Rec_To_CopyMove.CAMS_Desc = Entity.CAMS_Desc;
                        New_Rec_To_CopyMove.Shift_Count = Entity.Shift_Count;

                        SP_Header_Rec.Validate = "N";
                        break;
                    }
                }


                bool Can_Add_Rec = true; ;
                int Tmp_loop_Cnt = 1, Tmp_Grp_Cnt = 1;
                foreach (CASESPM2Entity Entity in ADD_CAMA_Details)
                {
                    Entity.Curr_Group = "0"; Can_Add_Rec = true;

                    if (!Row_Shifted && (Entity.SelOrdinal.ToString() == CAMS_Move_Copy_Details[0]) &&   // Adds to entity when moved from Lower Sequence to Heigher Sequence
                        (int.Parse(SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch"].Value.ToString()) > int.Parse(CAMS_Move_Copy_Details[0])))  //SP2_CAMS_Branch
                    //(int.Parse(SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch_SelOrdinal"].Value.ToString()) > int.Parse(CAMS_Move_Copy_Details[0])))
                    {
                        New_CAMS_Details.Add(new CASESPM2Entity(New_Rec_To_CopyMove)); Row_Shifted = true;
                    }

                    //if (CAMS_Move_Copy_Details[2] == "M" && Entity.SelOrdinal.ToString() == SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch"].Value.ToString()) //To Bypass Selected CA/MS in Move
                    //    Can_Add_Rec = false;

                    if (CAMS_Move_Copy_Details[2] == "M" && Tmp_loop_Cnt.ToString() == SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch"].Value.ToString()) //To Bypass Selected CA/MS in Move //SP2_CAMS_Branch
                        Can_Add_Rec = false;


                    Entity.Shift_Count = Tmp_loop_Cnt++;

                    if (Can_Add_Rec)
                        New_CAMS_Details.Add(new CASESPM2Entity(Entity));


                    if (!Row_Shifted && (Entity.SelOrdinal.ToString() == CAMS_Move_Copy_Details[0]) &&   // Adds to entity when moved from Heigher Sequence to Lower Sequence
                        (int.Parse(SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch"].Value.ToString()) < int.Parse(CAMS_Move_Copy_Details[0]))) //SP2_Branch
                    {
                        New_CAMS_Details.Add(new CASESPM2Entity(New_Rec_To_CopyMove)); Row_Shifted = true;
                    }

                }

                string Priv_Type = null; int Tmp_Ordinal_Cnt = 1;
                foreach (CASESPM2Entity Entity in New_CAMS_Details)
                {
                    if (Tmp_loop_Cnt > 0)
                    {
                        if (Entity.Type1 == "CA" && Priv_Type == "MS")
                        {
                            Tmp_Grp_Cnt++;
                            // Tmp_Ordinal_Cnt = 1;
                        }
                    }

                    Entity.Curr_Group = Tmp_Grp_Cnt.ToString();
                    //Entity.SelOrdinal = Tmp_Ordinal_Cnt.ToString();
                    Tmp_loop_Cnt++; //Tmp_Ordinal_Cnt++;
                    Priv_Type = Entity.Type1;
                }

                ADD_CAMA_Details.Clear();
                //foreach (CASESPM2Entity Entity in New_CAMS_Details)
                //    ADD_CAMA_Details.Add(new CASESPM2Entity(Entity));

                ADD_CAMA_Details = New_CAMS_Details;
                if (ADD_CAMA_Details.Count > 0)
                {
                    //this.SP_CAMS_Grid.SelectionChanged -= new System.EventHandler(this.SP_CAMS_Grid_SelectionChanged);
                    //SP_CAMS_Grid.Rows.Clear();
                    //pictureBox2.Visible = false;
                    Tmp_loop_Cnt = 1;
                    foreach (CASESPM2Entity Entity in ADD_CAMA_Details) ////// CAMA_Details) //////Branch_CAMS_Details
                    {
                        int rowIndex = 0;

                        Entity.SelOrdinal = Tmp_loop_Cnt.ToString(); //88888888
                        //rowIndex = SP_CAMS_Grid.Rows.Add(false, Entity.CAMS_Desc, Entity.SelOrdinal, Entity.Type1, Entity.Curr_Group, Entity.Group, Entity.CamCd);

                        //rowIndex = SP_CAMS_Grid.Rows.Add(false, Entity.CAMS_Desc, Comp_date, Followup, Img_Add, Entity.Type1, Entity.CamCd.Trim(), "A", Tmp_Row_Ordinal.ToString(), Entity.Group, Notes_Exists, Notes_Key, " ", Entity.CAMS_Desc, Posted_Year, "Y", " ", Entity.Curr_Group);  // CA Active Status


                        Tmp_loop_Cnt++;
                    }
                    //SP_CAMS_Grid.Rows[0].Tag = 0;
                    //if (SP_CAMS_Grid.Rows.Count > 9)
                    //    pictureBox2.Visible = true;
                    //Prepare_Menu_items();

                    //this.SP_CAMS_Grid.SelectionChanged += new System.EventHandler(this.SP_CAMS_Grid_SelectionChanged);
                }
            }
        }

        string QuickPost = string.Empty;
        private void SP_CAMS_Grid_MenuClick(object objSource, MenuItemEventArgs objArgs)
        {
            CAMS_Move_Copy_Details[0] = CAMS_Move_Copy_Details[1] = CAMS_Move_Copy_Details[2] = CAMS_Move_Copy_Details[3] = null;
            string[] Split_Array = new string[2];

            if (objArgs.MenuItem.Tag is string)
            {
                Split_Array = Regex.Split(objArgs.MenuItem.Tag.ToString(), " ");
                if (Split_Array[0].ToString() == "Q")
                {
                    SP2_SavePanel.Visible = false; Cb_Show_All_Postings.Visible = false; Btn_Maintain_Add.Visible = false; Auto_Post_Panel.Visible = false;
                    QuickPost = string.Empty;

                    foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
                    {
                        if (dr.Cells["SP2_Comp_Date"].Value.ToString().Trim() == string.Empty)
                            dr.Cells["Del_1"].ReadOnly = false;
                    }

                    string Notes_Field_Name = null;

                    CASEACTEntity SelSP_Det = new CASEACTEntity(true);
                    CASEMSEntity SelMS_det = new CASEMSEntity(true);
                    if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim() == "CA")
                        SelSP_Det = SP_Activity_Details.Find(u => u.ACT_ID.Equals(SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_ID"].Value.ToString().Trim()));
                    else
                        SelMS_det = SP_MS_Details.Find(u => u.ID.Equals(SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_ID"].Value.ToString().Trim()));
                    CA_QuickPost_Details.Rec_Type = MS_QuickPost_Details.Rec_Type = "I";
                    if (SelSP_Det != null && SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim() == "CA")
                    {
                        QuickPost = SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim();

                        CA_QuickPost_Details = SelSP_Det;

                        CA_QuickPost_Details.Agency = BaseForm.BaseAgency;
                        CA_QuickPost_Details.Dept = BaseForm.BaseDept;
                        CA_QuickPost_Details.Program = BaseForm.BaseProg;
                        CA_QuickPost_Details.Year = BaseForm.BaseYear;
                        CA_QuickPost_Details.App_no = BaseForm.BaseApplicationNo;
                        CA_QuickPost_Details.Bulk = "Q";

                        CA_QuickPost_Details.ACT_ID = SelSP_Det.ACT_ID;
                        CA_QuickPost_Details.Branch = CA_QuickPost_Details.Group = "";
                        CA_QuickPost_Details.Service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(); ;
                        CA_QuickPost_Details.SPM_Seq = SelSP_Det.SPM_Seq; CA_QuickPost_Details.ACT_Seq = "1";
                        CA_QuickPost_Details.Act_PROG = SelSP_Det.Act_PROG; CA_QuickPost_Details.ACT_Date = LookupDataAccess.Getdate(SelSP_Det.ACT_Date.Trim());
                        CA_QuickPost_Details.Site = SelSP_Det.Site; CA_QuickPost_Details.Caseworker = SelSP_Det.Caseworker;


                        List<CAOBOEntity> SelCAOBO_List = new List<CAOBOEntity>();
                        CAOBOEntity Search_OBO_Entity = new CAOBOEntity();
                        Search_OBO_Entity.ID = SelMS_det.ID;
                        Search_OBO_Entity.Seq = Search_OBO_Entity.CLID = Search_OBO_Entity.Fam_Seq = null;

                        SelCAOBO_List = _model.SPAdminData.Browse_CAOBO(Search_OBO_Entity, "Browse");

                        MS_QuickPost_Details.ID = "0";


                        CAOBOEntity Tmp = new CAOBOEntity();
                        foreach (CAOBOEntity dr in SelCAOBO_List)
                        {
                            //if (dr.Cells["MS_Sel"].Value.ToString() == true.ToString())
                            //{
                            Tmp.ID = SelMS_det.ID;
                            Tmp.CLID = dr.CLID.ToString();
                            Tmp.Fam_Seq = dr.Fam_Seq.ToString();
                            Tmp.Seq = "1";

                            CAOBO_List.Add(new CAOBOEntity(Tmp));
                            //}
                        }

                        Notes_Field_Name = Hierarchy + Spm_Year + App_No + SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_Key"].Value.ToString();
                    }

                    if (SelMS_det != null && SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim() == "MS")
                    {
                        QuickPost = SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim();

                        MS_QuickPost_Details = SelMS_det;

                        MS_QuickPost_Details.Agency = BaseForm.BaseAgency;
                        MS_QuickPost_Details.Dept = BaseForm.BaseDept;
                        MS_QuickPost_Details.Program = BaseForm.BaseProg;
                        MS_QuickPost_Details.Year = BaseForm.BaseYear;
                        MS_QuickPost_Details.App_no = BaseForm.BaseApplicationNo;


                        MS_QuickPost_Details.Service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString();

                        MS_QuickPost_Details.Branch = MS_QuickPost_Details.Group = "";
                        MS_QuickPost_Details.SPM_Seq = SelMS_det.SPM_Seq;
                        MS_QuickPost_Details.Bulk = "Q";

                        MS_QuickPost_Details.Date = LookupDataAccess.Getdate(SelMS_det.Date.Trim()); MS_QuickPost_Details.Result = SelMS_det.Result; MS_QuickPost_Details.OBF = SelMS_det.OBF;

                        List<CASEMSOBOEntity> CASEMSOBO_List = new List<CASEMSOBOEntity>();
                        CASEMSOBOEntity Search_OBO_Entity = new CASEMSOBOEntity();
                        Search_OBO_Entity.ID = SelMS_det.ID;
                        Search_OBO_Entity.Seq = Search_OBO_Entity.CLID = Search_OBO_Entity.Fam_Seq = null;

                        CASEMSOBO_List = _model.SPAdminData.Browse_CASEMSOBO(Search_OBO_Entity, "Browse");

                        MS_QuickPost_Details.ID = "0";


                        CASEMSOBOEntity Tmp = new CASEMSOBOEntity();
                        foreach (CASEMSOBOEntity dr in CASEMSOBO_List)
                        {
                            //if (dr.Cells["MS_Sel"].Value.ToString() == true.ToString())
                            //{
                            Tmp.ID = SelMS_det.ID;
                            Tmp.CLID = dr.CLID.ToString();
                            Tmp.Fam_Seq = dr.Fam_Seq.ToString();
                            Tmp.Seq = "1";

                            OBO_List.Add(new CASEMSOBOEntity(Tmp));
                            //}
                        }

                        Notes_Field_Name = Hierarchy + Spm_Year + App_No + SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_Key"].Value.ToString();

                    }

                    caseNotesEntity = _model.TmsApcndata.GetCaseNotesScreenFieldName((SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim() == "CA" ? "CASE00063" : "CASE00064"), Notes_Field_Name.Trim());

                    QuickMsgPanel.Visible = true; MainPanel.Enabled = false; //MainPanel.Visible = true;
                    this.QuickMsgPanel.Size = new System.Drawing.Size(720, 150);
                    this.QuickMsgPanel.Location = new System.Drawing.Point(7, 12);
                    this.txtQuickMsg.Size = new System.Drawing.Size(720, 150);
                    this.txtQuickMsg.Location = new System.Drawing.Point(45, 65);
                    ////txtQuickMsg.Visible = true;
                    txtQuickMsg.Enabled = false;

                    txtQuickMsg.Text = "Quick Post Mode this will copy the details for... \n" + SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim() + ": '" + SP_CAMS_Grid.CurrentRow.Cells["SP2_Desc"].Value.ToString().Trim() + "' dated " + SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value.ToString().Trim() + ". \n Please check the rows you would like to copy this data to.";

                    // MessageBox.Show("Quick Post Mode this will copy the details for " + SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString().Trim() + ": '" + SP_CAMS_Grid.CurrentRow.Cells["SP2_Desc"].Value.ToString().Trim() + "' dated " + SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value.ToString().Trim() + ".\n Please check the rows you would like to copy this data to.", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, Save_QuickPost, true);
                }
                else
                {
                    CAMS_Move_Copy_Details[0] = Split_Array[0];         // Move to Sequence       
                    CAMS_Move_Copy_Details[1] = Split_Array[1];         // Selected Record type

                    Split_Array = null;
                    Split_Array = Regex.Split(objArgs.MenuItem.Parent.Tag.ToString(), " ");
                    CAMS_Move_Copy_Details[2] = Split_Array[0];        // Operation to Perform (Move 'M' / Coyp 'C')
                    CAMS_Move_Copy_Details[3] = Split_Array[1];        // Current Group of Selected Record

                    Move_Copy_SelectedCAMS();

                    if (ADD_CAMA_Details.Count > 0 && Row_Shifted) //CAMA_Details
                    {
                        foreach (CASESPM2Entity Entity in ADD_CAMA_Details) //CAMA_Details
                        {
                            if (_model.SPAdminData.UpdateCASESPM2(Entity, "RowChange", out Sql_SP_Result_Message, "RowChange"))
                                Row_Shifted = true;
                        }
                    }

                    if (Row_Shifted)
                    {
                        Fill_Additional_CAMS_Details_Grid();
                        //Update_CASESP0_Branches();
                        //Fill_SP_CAMS_Details_Grid();
                        //Fill_Branch_CAMS_Details_Grid();

                        //SP_Header_Rec.Validate = "N";
                        //BtnValidate.Visible = true;
                        //Refresh_ADMN20_Control = true;
                    }
                }
            }
        }

        private void Cb_Show_All_Postings_CheckedChanged(object sender, EventArgs e)
        {
            if (Cb_Show_All_Postings.Checked)
            {
                Pb_Add_CA.Visible = Pb_Edit.Visible = PbDelete.Visible = PB_SP2_Notes.Visible = false;
                Clear_SP_CAMS_Grid();
                //SP_CAMS_Grid.Rows.Clear();

                int rowIndex = 0;
                foreach (DataGridViewRow dr in Branches_Grid.Rows)
                {
                    if (dr.Cells["Branch_Status"].Value.ToString() == "Y" && dr.Cells["Branch_Code"].Value.ToString() != "9")
                    {
                        // Rao 06192015
                        rowIndex = SP_CAMS_Grid.Rows.Add(false, dr.Cells["SP_Branches"].Value.ToString(), " ", " ", " ", Img_Add, "Branch", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "N");
                        SP_CAMS_Grid.Rows[rowIndex].Cells["Del_1"].ReadOnly = true;
                        SP_CAMS_Grid.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);

                        //Fill_SP_CAMS_Details(Search_Entity.service_plan, dr.Cells["Branch_Code"].Value.ToString(), CAMA_Post_Mode == "Edit" ? gbl_sel_CAMS_KEY : null);
                        Fill_SP_CAMS_Details(Search_Entity.service_plan, dr.Cells["Branch_Code"].Value.ToString(), CAMA_Post_Mode == "Edit" ? gbl_sel_CAMS_KEY : null);
                    }
                }

                if (SP_Header_Rec.Allow_Add_Branch == "Y" && Mode.Equals("Edit"))
                    Fill_Additional_CAMS_Details_Grid();

            }
            else
            {
                foreach (DataGridViewRow dr in Branches_Grid.Rows)
                    Branches_Grid.Rows[dr.Index].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F);

                Branches_Grid_SelectionChanged(Branches_Grid, EventArgs.Empty);
            }

            //if (SP_Activity_Details.Count == 0 && SP_MS_Details.Count > 0)
            //    Start_Date.Enabled = false;
            //else
            //    Start_Date.Enabled = Start_Date_Enable_SW;

            if (SP_Activity_Details.Count == 0 && SP_MS_Details.Count == 0)
                Start_Date.Enabled = Start_Date_Enable_SW;
            else
                Start_Date.Enabled = false;

            buttonAllowAMDSwitch("Y");

        }

        private void SP_CAMS_Grid_CellClick(object objSender, KeyEventArgs objArgs)
        {
            //int ColIdx = SP_CAMS_Grid.CurrentCell.ColumnIndex;
            //int RowIdx = SP_CAMS_Grid.CurrentCell.RowIndex;
            //if (SP_CAMS_Grid.Rows.Count > 0 && RowIdx != -1)
            //{
            //    switch (ColIdx)
            //    {
            //        case 0: if (!Del_Col_Header_Clicked)
            //                Select_All_To_Delete();
            //            break;
            //        case 4: Add_Edit_CAMS_Details("Add");
            //            break;
            //    }

            //    if ((SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA" || SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "A") &&
            //         Privileges.AddPriv.Equals("true") && Spm_Year == BaseForm.BaseYear)
            //        Pb_Add_CA.Visible = true;
            //    else
            //        Pb_Add_CA.Visible = false;

            //    if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C" && Privileges.ChangePriv.Equals("true"))
            //        Pb_Edit.Visible = true;
            //    else
            //        Pb_Edit.Visible = false;


            //    if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C" && Privileges.ChangePriv.Equals("true"))
            //    {

            //        if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_SW"].Value.ToString() == "Y")
            //            PB_SP2_Notes.ImageSource = "captain-casenotes";
            //        else
            //            PB_SP2_Notes.ImageSource = "captain-casenotesadd";;
            //        PB_SP2_Notes.Visible = true;
            //    }
            //    else
            //        PB_SP2_Notes.Visible = false;
            //}
        }

        private void SP_CAMS_Grid_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            //int ColIdx = SP_CAMS_Grid.CurrentCell.ColumnIndex;
            //int RowIdx = SP_CAMS_Grid.CurrentCell.RowIndex;
            //if (SP_CAMS_Grid.Rows.Count > 0 && e.RowIndex != -1)
            //{
            //    switch (e.ColumnIndex)
            //    {
            //        case 0: if (!Del_Col_Header_Clicked)
            //                Select_All_To_Delete();
            //            break;
            //        case 4: Add_Edit_CAMS_Details("Add");
            //            break;
            //    }

            //    if ((SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == "CA" || SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "A") &&
            //         Privileges.AddPriv.Equals("true") && Spm_Year == BaseForm.BaseYear)
            //        Pb_Add_CA.Visible = true;
            //    else
            //        Pb_Add_CA.Visible = false;

            //    if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C" && Privileges.ChangePriv.Equals("true"))
            //        Pb_Edit.Visible = true;
            //    else
            //        Pb_Edit.Visible = false;


            //    if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C" && Privileges.ChangePriv.Equals("true"))
            //    {

            //        if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Notes_SW"].Value.ToString() == "Y")
            //            PB_SP2_Notes.ImageSource = "captain-casenotes";
            //        else
            //            PB_SP2_Notes.ImageSource = "captain-casenotesadd";;
            //        PB_SP2_Notes.Visible = true;
            //    }
            //    else
            //        PB_SP2_Notes.Visible = false;

            //}
        }


        private void Clear_SP_CAMS_Grid()
        {
            this.SP_CAMS_Grid.SelectionChanged -= new System.EventHandler(this.SP_CAMS_Grid_SelectionChanged);
            SP_CAMS_Grid.Rows.Clear();
            this.SP_CAMS_Grid.SelectionChanged += new System.EventHandler(this.SP_CAMS_Grid_SelectionChanged);
        }

        List<CASEACTEntity> Act_Template_List = new List<CASEACTEntity>();
        List<CASEMSEntity> MS_Template_List = new List<CASEMSEntity>();
        private void SP_CAMS_Grid_DoubleClick(object sender, EventArgs e)
        {
            if (SP_CAMS_Grid.Rows.Count > 0)// && e.RowIndex != -1)
            {
                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() == "C")
                //Privileges.AddPriv.Equals("true") && Spm_Year == BaseForm.BaseYear)
                {
                    string Processing_Row_Type = SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString();
                    if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Template"].Value.ToString() == "Y")
                    {                                                   // To remove Template
                        if (Processing_Row_Type == "CA")
                            Act_Template_List.Clear();
                        else
                            MS_Template_List.Clear();

                        SP_CAMS_Grid.CurrentRow.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F);
                        SP_CAMS_Grid.CurrentRow.Cells["SP2_Template"].Value = "N";
                    }
                    else
                    {                                                   // To Set Template
                        if ((Act_Template_List.Count > 0 && Processing_Row_Type == "CA") ||
                            (MS_Template_List.Count > 0 && Processing_Row_Type == "MS"))
                        {
                            foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
                            {
                                if (Processing_Row_Type == dr.Cells["SP2_Type"].Value.ToString())
                                {
                                    //if (dr.Cells["SP2_Template"].Value.ToString() == "Y")
                                    dr.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F);

                                    dr.Cells["SP2_Template"].Value = "N";
                                }
                            }
                        }

                        Prepare_Search_Entity("Normal",
                                              SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString(),
                                              SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString(),
                                              SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString(),
                                              "Edit",
                                              SP_CAMS_Grid.CurrentRow.Cells["SP2_Group"].Value.ToString().Trim(),
                                              SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Branch"].Value.ToString().Trim(), //SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Branch"].Value.ToString(),  == "9" ? "9" : SP_CAMS_Grid.CurrentRow.Cells["SP2_Branch"].Value.ToString().Trim()),
                                              SP_CAMS_Grid.CurrentRow.Cells["SP2_CA_Seq"].Value.ToString().Trim(),
                                              SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value.ToString().Trim());

                        SP_CAMS_Grid.CurrentRow.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                        SP_CAMS_Grid.CurrentRow.Cells["SP2_Template"].Value = "Y";

                        if (Processing_Row_Type == "CA")
                        {
                            Act_Template_List.Clear();
                            Act_Template_List.Add(new CASEACTEntity(CA_Pass_Entity));
                        }
                        else
                        {
                            MS_Template_List.Clear();
                            MS_Template_List.Add(new CASEMSEntity(MS_Pass_Entity));
                        }
                    }
                }
                else
                    MessageBox.Show("You Can Select only Posted CA/MS as Template", "CAP Systems");

            }
        }

        Wisej.Web.Form _cASE5006Form;
        private void CASE4006Form_FormClosed(object sender, FormClosedEventArgs e)
        {


            if (Refresh_Control)
                this.DialogResult = DialogResult.OK;
        }

        private void Pb_SPM_Prog_Click(object sender, EventArgs e)
        {
            try
            {
                if (CmbSP.SelectedItem != null)
                {
                    string Sel_Prog = (!string.IsNullOrEmpty(Txt_SPM_Program.Text.Trim()) ? Txt_SPM_Program.Text.Trim().Substring(0, 6) : "");
                    //HierarchieSelectionFormNew hierarchieSelectionForm = new HierarchieSelectionFormNew(BaseForm, Sel_Prog, ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString());
                    HierarchieSelectionFormNew hierarchieSelectionForm = new HierarchieSelectionFormNew(BaseForm, Sel_Prog, ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), ACR_SERV_Hies);
                    hierarchieSelectionForm.FormClosed += new FormClosedEventHandler(OnHierarchieFormClosed);
                    hierarchieSelectionForm.StartPosition = FormStartPosition.CenterScreen;
                    hierarchieSelectionForm.ShowDialog();
                }
            }
            catch (Exception ex) { }
        }

        string Sel_SPM_Program = "";
        List<HierarchyEntity> SP_Programs_List = new List<HierarchyEntity>();
        private void OnHierarchieFormClosed(object sender, FormClosedEventArgs e)
        {
            // HierarchieSelectionForm form = sender as HierarchieSelectionForm;
            HierarchieSelectionFormNew form = sender as HierarchieSelectionFormNew;

            if (form.DialogResult == DialogResult.OK)
                Txt_SPM_Program.Text = Sel_SPM_Program = form.Selected_SerPlan_Prog();
        }


        private string Set_SP_Program_Text(string Prog_Code)
        {
            string Tmp_Hierarchy = "";
            Sel_SPM_Program = "";

            foreach (HierarchyEntity Ent in SP_Programs_List)
            {
                Tmp_Hierarchy = Ent.Agency.Trim() + Ent.Dept.Trim() + Ent.Prog.Trim();
                if (Prog_Code == Tmp_Hierarchy)
                {
                    Sel_SPM_Program = Tmp_Hierarchy + " - " + Ent.HirarchyName.Trim();
                    break;
                }
            }

            return Sel_SPM_Program;
        }

        private void PbVoucher_Click(object sender, EventArgs e)
        {
            if (SP_CAMS_Grid.Rows.Count > 0)
            {
                //DataGridView dv = new DataGridView();
                //dv = SP_CAMS_Grid;
                bool IsFalse = false;
                foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
                {
                    if (dr.Cells["SP2_Type"].Value.ToString().Trim() == "CA" && !(string.IsNullOrEmpty(dr.Cells["SP2_Comp_Date"].Value.ToString().Trim())))
                    {
                        IsFalse = true; break;
                        //dv.Rows.Remove(dr);
                    }
                }

                if (IsFalse)
                {
                    PaymentVoucher_Filling PMF = new PaymentVoucher_Filling(BaseForm, SP_CAMS_Grid, Privileges, ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString())), ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())));
                    PMF.StartPosition = FormStartPosition.CenterScreen;
                    PMF.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No CA Posting Found", "CAP Systems");
                }
            }
        }

        private void GetAgencyDetails()
        {
            dsAgency = DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(BaseForm.BaseAgency, null, null, null, null, null, null);
            dtAgency = dsAgency.Tables[0];
        }

        private void Btn_AutoPost_CA_Click(object sender, EventArgs e)
        {
            if (!Start_Date.Checked)
            {
                MessageBox.Show("Please provide '" + Lbl_StartDate.Text + "'", "CAP Systems");
                return;
            }
            string Hie = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg;
            if (!CA_Auto_Details_Filled)
            {
                CA_Auto_Details.Rec_Type = "I";
                CA_Auto_Details.Act_PROG = "";

                if (!string.IsNullOrEmpty(Txt_SPM_Program.Text.Trim()))
                    CA_Auto_Details.Act_PROG = Txt_SPM_Program.Text.Substring(0, 6);
            }
            else
            {
                if (string.IsNullOrEmpty(CA_Auto_Details.Act_PROG.Trim()))
                    CA_Auto_Details.Act_PROG = "";

                if (string.IsNullOrEmpty(CA_Auto_Details.Act_PROG.Trim()))
                {
                    if (Txt_SPM_Program.Text.Trim().Length > 0)
                        CA_Auto_Details.Act_PROG = Txt_SPM_Program.Text.Substring(0, 6);
                }
            }

            CA_Auto_Details.Agency = BaseForm.BaseAgency;
            CA_Auto_Details.Dept = BaseForm.BaseDept;
            CA_Auto_Details.Program = BaseForm.BaseProg;
            CA_Auto_Details.Year = BaseForm.BaseYear;
            CA_Auto_Details.App_no = BaseForm.BaseApplicationNo;
            CA_Auto_Details.Bulk = "A";

            CA_Auto_Details.ACT_Code = CA_Auto_Details.Branch = CA_Auto_Details.Group = "";
            CA_Auto_Details.Service_plan = MS_Auto_Details.Service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(); ;
            CA_Auto_Details.ACT_Seq = CA_Auto_Details.SPM_Seq = "1";


            string strEndDate = string.Empty;

            if (Act_Date.Checked == true)
            {
                strEndDate = Act_Date.Value.ToShortDateString();
            }

            List<CASESP2Entity> SP2_CAMS_Details = _model.SPAdminData.Browse_CASESP2(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), null, null, null);
            if (SP2_CAMS_Details.Count > 0)
                SP2_CAMS_Details = SP2_CAMS_Details.FindAll(u => u.SP2_CAMS_Active == "A" && u.Type1 == "CA" && u.SP2_Auto_Post == "Y"); //ADDED BY SUDHEER ON 10/03/2020 AS PER OCO DECUMENT

            if (SP2_CAMS_Details.Count > 0)
            {
                string SPM_Site = string.Empty;
                CASE2006_CAMSForm PostCA_Form = new CASE2006_CAMSForm(BaseForm, "CA", "Auto Post CA", Hie, BaseForm.BaseYear, CA_Auto_Details, Privileges, App_MST_Entity.Site.Trim(), App_MST_Entity.IntakeWorker.Trim(), CntlCAEntity, SP_Header_Rec, Act_Template_List, Start_Date.Value.ToShortDateString(), SPM_Site, strEndDate);   // 08022012
                PostCA_Form.FormClosed += new FormClosedEventHandler(Get_CA_AutoPost_Details);
                PostCA_Form.StartPosition = FormStartPosition.CenterScreen;
                PostCA_Form.ShowDialog();
            }
            else
                MessageBox.Show("No Active Services  for this Service Plan " + ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Text.ToString());
        }

        private void Btn_AutoPost_MS_Click(object sender, EventArgs e)
        {
            if (!Start_Date.Checked)
            {
                MessageBox.Show("Please provide '" + Lbl_StartDate.Text + "'", "CAP Systems");
                return;
            }

            if (!MS_Auto_Details_Filled)
            {
                MS_Auto_Details.Rec_Type = "I";
                MS_Auto_Details.Acty_PROG = "";

                if (!string.IsNullOrEmpty(Txt_SPM_Program.Text.Trim()))
                    MS_Auto_Details.Acty_PROG = Txt_SPM_Program.Text.Substring(0, 6);
            }
            else
            {
                if (string.IsNullOrEmpty(MS_Auto_Details.Date.Trim()))
                    MS_Auto_Details.Date = Start_Date.Value.ToShortTimeString();

                if (string.IsNullOrEmpty(MS_Auto_Details.Acty_PROG.Trim()))
                    MS_Auto_Details.Acty_PROG = "";

                if (string.IsNullOrEmpty(MS_Auto_Details.Acty_PROG.Trim()))
                {
                    if (Txt_SPM_Program.Text.Trim().Length > 0)
                        MS_Auto_Details.Acty_PROG = Txt_SPM_Program.Text.Substring(0, 6);
                }

            }

            string Hie = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg;
            MS_Auto_Details.Agency = BaseForm.BaseAgency;
            MS_Auto_Details.Dept = BaseForm.BaseDept;
            MS_Auto_Details.Program = BaseForm.BaseProg;
            MS_Auto_Details.Year = BaseForm.BaseYear;
            MS_Auto_Details.App_no = BaseForm.BaseApplicationNo;

            MS_Auto_Details.ID = "0";
            MS_Auto_Details.Service_plan = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString();

            MS_Auto_Details.MS_Code = MS_Auto_Details.Branch = MS_Auto_Details.Group = "";
            MS_Auto_Details.SPM_Seq = "1";
            MS_Auto_Details.Bulk = "A";

            string SPM_Site = string.Empty;
            foreach (CASEMSOBOEntity Ent in OBO_Auto_List)
                SPM_Site = (SPM_Site + Ent.CLID + ", ");

            string strEndDate = string.Empty;

            if (Act_Date.Checked == true)
            {
                strEndDate = Act_Date.Value.ToShortDateString();
            }

            List<CASESP2Entity> SP2_CAMS_Details = _model.SPAdminData.Browse_CASESP2(((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString(), null, null, null);
            if (SP2_CAMS_Details.Count > 0)
                SP2_CAMS_Details = SP2_CAMS_Details.FindAll(u => u.SP2_CAMS_Active == "A" && u.Type1 == "MS" && u.SP2_Auto_Post == "Y"); //ADDED BY SUDHEER ON 10/03/2020 AS PER OCO DECUMENT

            if (SP2_CAMS_Details.Count > 0)
            {
                CASE2006_CAMSForm PostMS_Form = new CASE2006_CAMSForm(BaseForm, "MS", "Auto Post MS", Hie, BaseForm.BaseYear, MS_Auto_Details, Privileges, App_MST_Entity.Site, App_MST_Entity.IntakeWorker, CntlMSEntity, SP_Header_Rec, MS_Template_List, Start_Date.Value.ToShortDateString(), SPM_Site, strEndDate);
                PostMS_Form.FormClosed += new FormClosedEventHandler(Get_MS_AutoPost_Details);
                PostMS_Form.StartPosition = FormStartPosition.CenterScreen;
                PostMS_Form.ShowDialog();
            }
            else
            {
                MessageBox.Show("No Active Milestones for this Service Plan " + ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Text.ToString());
            }

        }

        bool CA_Auto_Details_Filled = false;
        CASEACTEntity CA_Auto_Details = new CASEACTEntity(true);
        private void Get_CA_AutoPost_Details(object sender, FormClosedEventArgs e)
        {
            CASE2006_CAMSForm form = sender as CASE2006_CAMSForm;
            if (form.DialogResult == DialogResult.OK)
            {
                CA_Auto_Details = form.Get_CA_AutoPost_Details();

                if (CA_Auto_Details.Act_PROG == null)
                    CA_Auto_Details.Act_PROG = "";

                CA_Auto_Details.Rec_Type = "U";
                CA_Auto_Details.Bulk = "A";
                CA_Auto_Details_Filled = true;

                if (Btn_AutoPost_MS.Visible)
                    _errorProvider.SetError(Btn_AutoPost_MS, null);
                else
                    _errorProvider.SetError(Btn_AutoPost_CA, null);
            }
        }

        bool MS_Auto_Details_Filled = false;
        CASEMSEntity MS_Auto_Details = new CASEMSEntity(true);
        List<CASEMSOBOEntity> OBO_Auto_List = new List<CASEMSOBOEntity>();
        private void Get_MS_AutoPost_Details(object sender, FormClosedEventArgs e)
        {
            CASE2006_CAMSForm form = sender as CASE2006_CAMSForm;
            if (form.DialogResult == DialogResult.OK)
            {
                MS_Auto_Details = form.Get_MS_AutoPost_Details();
                if (MS_Auto_Details.Acty_PROG == null)
                    MS_Auto_Details.Acty_PROG = "";

                MS_Auto_Details.Rec_Type = "U";
                OBO_Auto_List.Clear();
                OBO_Auto_List = form.Get_MSOBO_AutoPost_Details();

                MS_Auto_Details_Filled = true;
                _errorProvider.SetError(Btn_AutoPost_MS, null);
            }
        }


        private void buttonAllowAMDSwitch(string strMsgSwitch)
        {
            string strCode = string.Empty;
            if (((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString() != null)
                strCode = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString();
            if (propSearch_Entity.Count > 0)
            {
                CASESP0Entity casesp0data = propSearch_Entity.Find(u => u.Code == strCode);
                if (casesp0data != null)
                {
                    if (casesp0data.Sp0ReadOnly == "Y")
                    {
                        Pb_SPM2_Add.Visible = false;
                        Pb_SPM2_Edit.Visible = false;
                        Pb_SPM2_Delete.Visible = false;
                        Pb_Add_CA.Visible = false;
                        Pb_Edit.Visible = false;
                        PbDelete.Visible = false;
                        Btn_AutoPost_CA.Visible = Btn_AutoPost_MS.Visible = Btn_Bulk_Posting.Visible = Btn_Maintain_Add.Visible = false;
                        if (strMsgSwitch == "Y")
                            MessageBox.Show("Cannot be selected as this SP Read-Only", "CAPSYSTEMS");
                    }
                }

                if (Sw_ReadOnly == "Y" && SP_Readonly_Sw == "N")
                {
                    Pb_SPM2_Add.Visible = false;
                    Pb_SPM2_Edit.Visible = false;
                    Pb_SPM2_Delete.Visible = false;
                    Pb_Add_CA.Visible = false;
                    Pb_Edit.Visible = false;
                    PbDelete.Visible = false;
                    Btn_AutoPost_CA.Visible = Btn_AutoPost_MS.Visible = Btn_Bulk_Posting.Visible = Btn_Maintain_Add.Visible = false;
                }
            }
        }

        string ACR_SERV_Hies = "N";
        private void CASE4006Form_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(BaseForm.BaseAgencyControlDetails.ServicePlanHiecontrol.Trim()))
            {
                if (BaseForm.BaseAgencyControlDetails.ServicePlanHiecontrol.Trim() == "Y")
                    ACR_SERV_Hies = "S";
            }

            AdminControlValidation("CASE0006");
            if (Mode.Equals("Edit"))
            {
                buttonAllowAMDSwitch("Y");
            }
        }

        CASEACTEntity CA_QuickPost_Details = new CASEACTEntity(true);
        CASEMSEntity MS_QuickPost_Details = new CASEMSEntity(true);

        private void CASE5006Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Mode.Equals("Add"))
            {
                if (Refresh_Control)
                {
                    if (CmbSP.Items.Count > 0)
                    {
                        string strValue = ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value == null ? string.Empty : ((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString();
                        if (strValue != string.Empty)
                        {
                            Get_App_CASEACT_List();
                            Get_App_CASEMS_List();

                            if ((SP_Activity_Details.Count == 0) && (SP_MS_Details.Count == 0))
                            {

                                DialogResult result = MessageBox.Show("You have not added an Activity or Milestone. Are you sure you want to close this screen?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose:MessageBoxHandler);
                                _cASE5006Form = (Wisej.Web.Form)sender;
                                if (result == DialogResult.Yes)
                                {
                                    e.Cancel = false;
                                }
                                else
                                {
                                    e.Cancel = true;

                                }
                            }
                        }
                    }
                }
            }
            else
            {

                if ((SP_Activity_Details.Count == 0) && (SP_MS_Details.Count == 0))
                {

                    DialogResult result = MessageBox.Show("You have not added an Activity or Milestone. Are you sure you want to close this screen?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: MessageBoxHandler);
                    _cASE5006Form = (Wisej.Web.Form)sender;
                    if (result == DialogResult.Yes)
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;

                    }
                }
            }
        }

        private void MessageBoxHandler(DialogResult dialogResult)
        {
            // Set DialogResult value of the Form as a text for label
            if (dialogResult == DialogResult.Yes)
            {
                _cASE5006Form.FormClosing -= CASE5006Form_FormClosing;
                _cASE5006Form.Close();
            }
        }


        List<CAOBOEntity> CAOBO_List = new List<CAOBOEntity>();
        List<CASEMSOBOEntity> OBO_List = new List<CASEMSOBOEntity>();

        private void CASE5006Form_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (e.Tool.Name == "tlVoucher") {
                if (SP_CAMS_Grid.Rows.Count > 0)
                {
                    //DataGridView dv = new DataGridView();
                    //dv = SP_CAMS_Grid;
                    bool IsFalse = false;
                    foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
                    {
                        if (dr.Cells["SP2_Type"].Value.ToString().Trim() == "CA" && !(string.IsNullOrEmpty(dr.Cells["SP2_Comp_Date"].Value.ToString().Trim())))
                        {
                            IsFalse = true; break;
                            //dv.Rows.Remove(dr);
                        }
                    }

                    if (IsFalse)
                    {
                        PaymentVoucher_Filling PMF = new PaymentVoucher_Filling(BaseForm, SP_CAMS_Grid, Privileges, ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).Value.ToString())), ((((Captain.Common.Utilities.ListItem)CmbSP.SelectedItem).DefaultValue.ToString())));
                        PMF.StartPosition = FormStartPosition.CenterScreen;
                        PMF.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No CA Posting Found", "CAP Systems");
                    }
                }
            }
            if (e.Tool.Name == "tlSave")
            {
                On_SaveForm_Closed();
            }
            if (e.Tool.Name == "tlHelp")
            {
                Application.Navigate(CommonFunctions.BuildHelpURLS(Privileges.Program, 1, BaseForm.BusinessModuleID.ToString()), target: "_blank");
            }
        }

        //private void btnServiceAdmin_Click(object sender, EventArgs e)
        //{
        //    CASE4006_Usage case4006usageform = new CASE4006_Usage(BaseForm, Privileges, string.Empty, Mode);
        //    case4006usageform.ShowDialog();
        //}

        List<CaseNotesEntity> caseNotesEntity = new List<CaseNotesEntity>();
        private void Update_QuickPost_CAMS(string New_Spm_Seq)
        {
            int New_CAID = 1, New_CA_Seq = 1; string Notes_Field_Name = null;
            foreach (DataGridViewRow dr in SP_CAMS_Grid.Rows)
            {
                if (dr.Cells["Del_1"].Value.ToString() == true.ToString())
                {
                    //Prepare_Search_Entity("Normal",
                    //                      dr.Cells["SP2_Type"].Value.ToString(),
                    //                      dr.Cells["SP2_Operation"].Value.ToString(),
                    //                      dr.Cells["SP2_CAMS_Code"].Value.ToString(),
                    //                      "Delete",
                    //                      dr.Cells["SP2_Group"].Value.ToString().Trim(),
                    //                      dr.Cells["SP2_CAMS_Branch"].Value.ToString().Trim(), //SP2_Branch
                    //                      dr.Cells["SP2_CA_Seq"].Value.ToString().Trim(),
                    //                      dr.Cells["SP2_Comp_Date"].Value.ToString().Trim());

                    if (dr.Cells["SP2_Type"].Value.ToString() == "CA")
                    {
                        CA_QuickPost_Details.Rec_Type = "I";
                        CA_QuickPost_Details.ACT_Code = dr.Cells["SP2_CAMS_Code"].Value.ToString();
                        CA_QuickPost_Details.Branch = dr.Cells["SP2_CAMS_Branch"].Value.ToString().Trim();
                        CA_QuickPost_Details.Group = dr.Cells["SP2_Group"].Value.ToString().Trim();
                        CA_QuickPost_Details.Curr_Grp = dr.Cells["SP2_Curr_Grp"].Value.ToString().Trim();
                        CA_QuickPost_Details.Lsct_Operator = CA_QuickPost_Details.Add_Operator = BaseForm.UserID;
                        _model.SPAdminData.UpdateCASEACT(CA_QuickPost_Details, "Insert", out New_CAID, out New_CA_Seq, out Sql_SP_Result_Message);

                        Update_CAOBO_Quick_Benefitig_Members(New_CAID);

                        if (caseNotesEntity.Count > 0)
                        {
                            Notes_Field_Name = (BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) + BaseForm.BaseYear + BaseForm.BaseApplicationNo + CA_QuickPost_Details.Service_plan + CA_QuickPost_Details.SPM_Seq + CA_QuickPost_Details.Branch +
                            CA_QuickPost_Details.Group + "CA" + CA_QuickPost_Details.ACT_Code + CA_QuickPost_Details.ACT_Seq;

                            CaseNotesEntity caseNotesDetails = new CaseNotesEntity();
                            caseNotesDetails.ScreenName = caseNotesEntity[0].ScreenName;
                            caseNotesDetails.FieldName = Notes_Field_Name.Trim();
                            caseNotesDetails.AppliCationNo = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + (!string.IsNullOrEmpty(BaseForm.BaseYear.Trim()) ? BaseForm.BaseYear : "    ") + BaseForm.BaseApplicationNo;
                            caseNotesDetails.Data = caseNotesEntity[0].Data;
                            caseNotesDetails.LstcOperation = caseNotesDetails.AddOperator = BaseForm.UserID;
                            caseNotesDetails.Mode = "I";
                            _model.SPAdminData.UpdatePROGNOTES(caseNotesDetails, "Insert", out Sql_SP_Result_Message);
                        }
                    }
                    else if (dr.Cells["SP2_Type"].Value.ToString() == "MS")
                    {
                        MS_QuickPost_Details.Rec_Type = "I";
                        MS_QuickPost_Details.MS_Code = dr.Cells["SP2_CAMS_Code"].Value.ToString();
                        MS_QuickPost_Details.Branch = dr.Cells["SP2_CAMS_Branch"].Value.ToString().Trim();
                        MS_QuickPost_Details.Group = dr.Cells["SP2_Group"].Value.ToString().Trim();
                        MS_QuickPost_Details.Curr_Grp = dr.Cells["SP2_Curr_Grp"].Value.ToString().Trim();
                        MS_QuickPost_Details.Lsct_Operator = MS_QuickPost_Details.Add_Operator = BaseForm.UserID;
                        _model.SPAdminData.UpdateCASEMS(MS_QuickPost_Details, "Insert", out New_CAID, out Sql_SP_Result_Message);

                        Update_MSOBO_Quick_Benefitig_Members(New_CAID);

                        if (caseNotesEntity.Count > 0)
                        {
                            Notes_Field_Name = (BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) + BaseForm.BaseYear + BaseForm.BaseApplicationNo + MS_QuickPost_Details.Service_plan + MS_QuickPost_Details.SPM_Seq + MS_QuickPost_Details.Branch.Trim() +
                             MS_QuickPost_Details.Group.ToString() + "MS" + MS_QuickPost_Details.MS_Code.Trim() + CommonFunctions.ChangeDateFormat(Convert.ToDateTime(MS_QuickPost_Details.Date.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);


                            CaseNotesEntity caseNotesDetails = new CaseNotesEntity();
                            caseNotesDetails.ScreenName = caseNotesEntity[0].ScreenName;
                            caseNotesDetails.FieldName = Notes_Field_Name.Trim();
                            caseNotesDetails.AppliCationNo = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + (!string.IsNullOrEmpty(BaseForm.BaseYear.Trim()) ? BaseForm.BaseYear : "    ") + BaseForm.BaseApplicationNo;
                            caseNotesDetails.Data = caseNotesEntity[0].Data;
                            caseNotesDetails.LstcOperation = caseNotesDetails.AddOperator = BaseForm.UserID;
                            caseNotesDetails.Mode = "I";
                            _model.SPAdminData.UpdatePROGNOTES(caseNotesDetails, "Insert", out Sql_SP_Result_Message);
                        }
                    }
                }
            }

            SP2_SavePanel.Visible = true; Cb_Show_All_Postings.Visible = true; Btn_Maintain_Add.Visible = true; Auto_Post_Panel.Visible = true;
            QuickMsgPanel.Visible = false; MainPanel.Enabled = true;
            CA_QuickPost_Details = new CASEACTEntity(true); MS_QuickPost_Details = new CASEMSEntity(true);

        }

        private void Update_MSOBO_Quick_Benefitig_Members(int MSID)
        {
            CASEMSOBOEntity Save_Ent = new CASEMSOBOEntity();
            foreach (CASEMSOBOEntity Ent in OBO_List)
            {
                Save_Ent.Rec_Type = "I";
                Save_Ent.ID = MSID.ToString();
                Save_Ent.CLID = Ent.CLID;
                Save_Ent.Fam_Seq = Ent.Fam_Seq;
                Save_Ent.Seq = "1";

                _model.SPAdminData.UpdateCASEMSOBO(Save_Ent, "Insert", out Sql_SP_Result_Message);
            }


        }

        private void Update_CAOBO_Quick_Benefitig_Members(int CAID)
        {
            if (CAOBO_List.Count > 0)
            {
                CAOBOEntity Save_Ent = new CAOBOEntity();
                Save_Ent.ID = CAID.ToString();
                Save_Ent.Rec_Type = "S";
                _model.SPAdminData.UpdateCAOBO(Save_Ent, "Delete", out Sql_SP_Result_Message);

                foreach (CAOBOEntity Ent in CAOBO_List)
                {
                    Save_Ent.Rec_Type = "I";
                    Save_Ent.ID = CAID.ToString();
                    Save_Ent.CLID = Ent.CLID;
                    Save_Ent.Fam_Seq = Ent.Fam_Seq;
                    Save_Ent.Seq = "1";

                    _model.SPAdminData.UpdateCAOBO(Save_Ent, "Insert", out Sql_SP_Result_Message);
                }
            }

        }

        private void Select_All_To_QuickPost()
        {
            if (QuickPost == "CA")
            {
                List<CASEACTEntity> SelSP_Det = new List<CASEACTEntity>();

                // SelSP_Det = SP_Activity_Details.FindAll(u => (Convert.ToDateTime(u.ACT_Date.Trim()).ToShortDateString()).Equals(Convert.ToDateTime(CA_QuickPost_Details.ACT_Date.ToString().Trim()).ToShortDateString()));
                SelSP_Det = SP_Activity_Details.FindAll(u => u.ACT_Code.Trim().Equals(SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim()) && (Convert.ToDateTime(u.ACT_Date.Trim()).ToShortDateString()).Equals(Convert.ToDateTime(CA_QuickPost_Details.ACT_Date.ToString().Trim()).ToShortDateString()));

                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() != "C" || SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value.ToString() == string.Empty || SelSP_Det.Count == 0)
                {
                    //if (Spm_Year != BaseForm.BaseYear) CA_QuickPost_Details
                    //{
                    //    MessageBox.Show("Cannot be selected as this SPM is created in PY" + Spm_Year, "CAPSYSTEMS");
                    //}
                    string Tmp = "false";
                    Tmp = SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value.ToString();
                    if (Tmp == "True" && SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == QuickPost)
                    {
                        SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value = true;
                        //Sel_Del_Count++;
                    }
                    else
                    {
                        SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value = false;
                        //if (Sel_Del_Count > 0)
                        //    Sel_Del_Count--;
                    }
                }
                else
                {
                    SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value = false;
                    //if (Sel_Del_Count > 0)
                    //    Sel_Del_Count--;
                }
            }
            else if (QuickPost == "MS")
            {
                List<CASEMSEntity> SelMS_det = new List<CASEMSEntity>();
                SelMS_det = SP_MS_Details.FindAll(u => u.MS_Code.Trim().Equals(SP_CAMS_Grid.CurrentRow.Cells["SP2_CAMS_Code"].Value.ToString().Trim()) && (Convert.ToDateTime(u.Date.Trim()).ToShortDateString()).Equals(Convert.ToDateTime(MS_QuickPost_Details.Date.ToString().Trim()).ToShortDateString()));
                if (SP_CAMS_Grid.CurrentRow.Cells["SP2_Operation"].Value.ToString() != "C" || SP_CAMS_Grid.CurrentRow.Cells["SP2_Comp_Date"].Value.ToString() == string.Empty || SelMS_det.Count == 0)
                {
                    //if (Spm_Year != BaseForm.BaseYear) CA_QuickPost_Details
                    //{
                    //    MessageBox.Show("Cannot be selected as this SPM is created in PY" + Spm_Year, "CAPSYSTEMS");
                    //}
                    string Tmp = "false";
                    Tmp = SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value.ToString();
                    if (Tmp == "True" && SP_CAMS_Grid.CurrentRow.Cells["SP2_Type"].Value.ToString() == QuickPost)
                    {
                        SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value = true;
                        //Sel_Del_Count++;
                    }
                    else
                    {
                        SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value = false;
                        //if (Sel_Del_Count > 0)
                        //    Sel_Del_Count--;
                    }
                }
                else
                    SP_CAMS_Grid.CurrentRow.Cells["Del_1"].Value = false;
            }

        }

        #region  ValidationData

        public bool AdminControlValidation(string strScreenCode)
        {
            bool boolvalidation = true;
            bool boolAllowClientIntake = true;
            bool boolAllowCustom = true;
            bool boolAllowIncomeVer = true;
            bool boolDisplayClientIntake = true;
            bool boolDisplayCustom = true;
            bool boolDisplayIncomeVer = true;
            string strclientIntakeRequired = string.Empty;
            string strVerRequired = string.Empty;
            string strclientDisMsg = string.Empty;
            string strCustomDisMsg = string.Empty;
            string strVerDisMsg = string.Empty;
            string strMsg = string.Empty;
            if (BaseForm.BaseAgencyControlDetails.RomaSwitch.ToUpper() == "Y")
            {
                List<ScaFldsHieEntity> ScaFldsHiedata = _model.FieldControls.GETSCAFLDSHIEDATA(strScreenCode, string.Empty, string.Empty);
                if (ScaFldsHiedata.Count > 0)
                {
                    List<ScaFldsHieEntity> ScaFldsHie = ScaFldsHiedata.FindAll(u => u.ScrHie == BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg);
                    if (ScaFldsHie.Count == 0)
                    {
                        ScaFldsHie = ScaFldsHiedata.FindAll(u => u.ScrHie == BaseForm.BaseAgency + BaseForm.BaseDept + "**");
                        if (ScaFldsHie.Count == 0)
                        {
                            ScaFldsHie = ScaFldsHiedata.FindAll(u => u.ScrHie == BaseForm.BaseAgency + "****");
                        }
                        if (ScaFldsHie.Count == 0)
                        {
                            ScaFldsHie = ScaFldsHiedata.FindAll(u => u.ScrHie == "******");
                        }
                    }
                    if (ScaFldsHie.FindAll(u => u.Active == "Y").Count > 0)
                    {
                        if (ScaFldsHie.Count > 0)
                        {
                            int intvalidcount = 0;
                            ScaFldsHieEntity ScaFldscase2001data = ScaFldsHie.Find(u => u.ScahCode == "S0001");
                            ScaFldsHieEntity ScaFldscustomdata = ScaFldsHie.Find(u => u.ScahCode == "S0002");
                            ScaFldsHieEntity ScaFldscaseverdata = ScaFldsHie.Find(u => u.ScahCode == "S0003");
                            CaseSnpEntity snpdata = BaseForm.BaseCaseSnpEntity.Find(u => u.FamilySeq == BaseForm.BaseCaseMstListEntity[0].FamilySeq);
                            if (ScaFldscase2001data.Sel.ToUpper() == "Y")
                            {

                                boolAllowClientIntake = _model.CaseMstData.CheckRequiredCase2001ControlsData(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg, BaseForm.BaseCaseSnpEntity, BaseForm.BaseCaseMstListEntity[0], out strclientIntakeRequired);
                                if (!boolAllowClientIntake)
                                {
                                    boolDisplayClientIntake = false;
                                    intvalidcount = +1;
                                    strMsg = ScaFldscase2001data.Msg;
                                    strclientDisMsg = ScaFldscase2001data.Msg;
                                    if (ScaFldscase2001data.Active.ToUpper() == "Y")
                                        boolAllowClientIntake = true;

                                }
                            }
                            if (ScaFldscustomdata.Sel.ToUpper() == "Y")
                            {
                                boolAllowCustom = _model.CaseMstData.CheckRequiredCustomQuestionsData(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg, snpdata);
                                if (!boolAllowCustom)
                                {
                                    boolDisplayCustom = false;
                                    intvalidcount = intvalidcount + 1;
                                    strMsg = strMsg + "\n" + ScaFldscustomdata.Msg;
                                    strCustomDisMsg = ScaFldscustomdata.Msg;
                                    if (ScaFldscustomdata.Active.ToUpper() == "Y")
                                        boolAllowCustom = true;
                                }
                            }

                            if (ScaFldscaseverdata.Sel.ToUpper() == "Y")
                            {
                                List<CaseVerEntity> caseVerList = _model.CaseMstData.GetCASEVeradpyalst(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty, string.Empty);
                                if (caseVerList.Count > 0)
                                {
                                    boolAllowIncomeVer = _model.CaseMstData.CheckRequiredCaseverdata(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg, caseVerList[0], out strVerRequired);
                                    if (!boolAllowIncomeVer)
                                    {
                                        boolDisplayIncomeVer = false;
                                        intvalidcount = intvalidcount + 1;
                                        strMsg = strMsg + "\n" + ScaFldscaseverdata.Msg;
                                        strVerDisMsg = ScaFldscaseverdata.Msg;
                                        if (ScaFldscaseverdata.Active.ToUpper() == "Y")
                                            boolAllowIncomeVer = true;
                                    }
                                }
                                else
                                {

                                    boolAllowIncomeVer = false;
                                    boolDisplayIncomeVer = false;
                                    intvalidcount = intvalidcount + 1;
                                    strMsg = strMsg + "\nIncome Verification Data Does Not Exist";
                                    strVerRequired = "Income Verification Data Does Not Exist";
                                    strVerDisMsg = ScaFldscaseverdata.Msg;
                                    if (ScaFldscaseverdata.Active.ToUpper() == "Y")
                                        boolAllowIncomeVer = true;

                                }
                            }
                            string strIncompleteMsg = _model.CaseMstData.DisplayIncomeMsgs(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, snpdata, BaseForm.BaseCaseMstListEntity[0]);
                            strMsg = strMsg + "\n" + strIncompleteMsg;
                            if (boolAllowClientIntake == true && boolAllowCustom == true && boolAllowIncomeVer == true)
                                boolvalidation = true;
                            else
                                boolvalidation = false;
                            if (intvalidcount > 0)
                            {
                                ScaFldsHieEntity ScaFldscaseGerdata = ScaFldsHie.Find(u => u.ScahCode == "S0004");
                                if (boolvalidation == false)
                                {
                                    if (ScaFldscaseGerdata != null)
                                    {
                                        if (ScaFldscaseGerdata.Sel.ToUpper() == "Y")
                                            strMsg = ScaFldscaseGerdata.Msg;
                                    }
                                }
                                AdminControlMessageForm objForm = new AdminControlMessageForm(BaseForm, strMsg, strclientIntakeRequired, strVerRequired, boolDisplayClientIntake, boolDisplayCustom, boolDisplayIncomeVer, strclientDisMsg, strCustomDisMsg, strVerDisMsg, strIncompleteMsg, true, string.Empty);
                                objForm.StartPosition = FormStartPosition.CenterScreen;
                                objForm.ShowDialog();
                                // CommonFunctions.MessageBoxDisplay(strMsg);
                            }

                        }

                    }

                }

            }

            return boolvalidation;
        }

        #endregion


    }
}