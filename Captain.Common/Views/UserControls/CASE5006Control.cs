/************************************************************************
 * Conversion On        :   11/25/2022
 * Converted By         :   Kranthi
 * Last Modification On :   11/25/2022
 * **********************************************************************/

#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Web.Configuration;
using Captain.Common.Views.Forms.Base;
using Captain.Common.Utilities;
using Captain.Common.Menus;
using System.Data.SqlClient;
using Captain.Common.Model.Data;
using Captain.Common.Model.Objects;
using Captain.Common.Views.UserControls.Base;
using Captain.Common.Exceptions;
using System.Diagnostics;
using Captain.Common.Views.Forms;
using System.Text.RegularExpressions;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Drawing;
using Wisej.Web;
#endregion

namespace Captain.Common.Views.UserControls
{
    public partial class CASE5006Control : BaseUserControl
    {

        #region private variables

        private CaptainModel _model = null;
        private PrivilegesControl _screenPrivileges = null;
        private AlertCodes alertCodesUserControl = null;
        private int strIndex = 0;
        #endregion

        public CASE5006Control(BaseForm baseForm, PrivilegeEntity privileges)
        {
            InitializeComponent();
            BaseForm = baseForm;
            Privileges = privileges;
            _model = new CaptainModel();

            GetSelectedProgram();
            GetFillControlData();
            Fill_CaseWorker();

            btnSearch_Click();

            propAgencyControlDetails = _model.ZipCodeAndAgency.GetAgencyControlFile("00");
            if (propAgencyControlDetails != null)
            {
                ReferalConn = propAgencyControlDetails.RefConn.Trim();
                if (ReferalConn == "Y")
                    Ref_Connected.Visible = true;
            }
            Fill_Languages_List();
            Fill_Contacts_Grid(string.Empty);
            propReportPath = _model.lookupDataAccess.GetReportPath();
            Fill_Sel_Hierarchy_SPs();
            strFolderPath = Consts.Common.ReportFolderLocation + BaseForm.UserID + "\\";
            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(Pb_Add_Contact, "Add Contact");
            tooltip.SetToolTip(Pb_Delete_Contact, "Delete Contact");
            tooltip.SetToolTip(Pb_Edit_Contact, "Edit Contact");
            tooltip.SetToolTip(Pb_Cont_Notes, "Add Progress Notes");
            tooltip.SetToolTip(picReferAdd, "Add Agency Referrals");
            tooltip.SetToolTip(picReferEdit, "Edit Agency Referrals");
            tooltip.SetToolTip(picReferDelete, "Delete Agency Referrals");
            ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            ProgramDefinition = programEntity;
            IsServiceStop = false;
            List<SERVSTOPEntity> SERVSTOPList = _model.CaseSumData.GetSERVSTOPDet(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, string.Empty, string.Empty);
            if (SERVSTOPList.Count > 0)
            {
                SERVSTOPEntity Entity = SERVSTOPList.Find(u => Convert.ToDateTime(u.TDate.Trim()) >= Convert.ToDateTime(DateTime.Now.ToShortDateString()) && Convert.ToDateTime(u.FDate.Trim()) <= Convert.ToDateTime(DateTime.Now.ToShortDateString()));
                if (Entity != null) IsServiceStop = true;
            }
            alertCodesUserControl = new AlertCodes(BaseForm, privileges, programEntity);
            alertCodesUserControl.Dock = DockStyle.Fill;
            pnlAlertcode.Controls.Add(alertCodesUserControl);
            ACTREFS_List = null;
            Get_ReferrTo_Data();
            Fill_ReferrTo_Data();

            PopulateToolbar(oToolbarMnustrip);
            // tooltip.SetToolTip(Hepl, "Help");
        }


        #region properties

        public BaseForm BaseForm { get; set; }

        public PrivilegeEntity Privileges { get; set; }

        public ToolBarButton ToolBarEdit { get; set; }

        public string propReportPath { get; set; }
        public bool IsServiceStop { get; set; }

        public ToolBarButton ToolBarNew { get; set; }

        public ToolBarButton ToolBarDel { get; set; }

        public ToolBarButton ToolBarPrint { get; set; }

        public ToolBarButton ToolBarHelp { get; set; }

        public string MainMenuAgency { get; set; }

        public string MainMenuDept { get; set; }

        public string MainMenuProgram { get; set; }

        public string MainMenuYear { get; set; }

        public string MainMenuAppNo { get; set; }

        public string MainMenuHIE { get; set; }

        public List<FldcntlHieEntity> CntlContactEntity { get; set; }

        public List<FldcntlHieEntity> CntlCAEntity { get; set; }

        public List<FldcntlHieEntity> CntlMSEntity { get; set; }

        // public List<ACTREFSEntity> ACTREFS_List { get; set; }

        public ProgramDefinitionEntity ProgramDefinition { get; set; }

        public string propRefDate { get; set; }

        public string propReferFormTo { get; set; }

        public string propCode { get; set; }
        public AgencyControlEntity propAgencyControlDetails { get; set; }
        public string ReferalConn { get; set; }

        #endregion

        string MainMenuHierarchy = null;
        string Sql_SP_Result_Message = string.Empty, Tmp_SPM_Sequence = string.Empty;

        string Img_Saved = "icon-save";
        string Img_Blank = "blank";
        string Img_Tick = "icon-gridtick";


        private void GetFillControlData()
        {
            string HIE = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg;
            CntlContactEntity = _model.FieldControls.GetFLDCNTLHIE("CASE0061", HIE, "FLDCNTL");
            CntlCAEntity = _model.FieldControls.GetFLDCNTLHIE("CASE0063", HIE, "FLDCNTL");
            CntlMSEntity = _model.FieldControls.GetFLDCNTLHIE("CASE0064", HIE, "FLDCNTL");
        }


        public void Refresh()
        {
            GetSelectedProgram();
            RefreshAlertCode();
            btnSearch_Click();
            GetActReferData();
            Fill_ReferrTo_Data();
            Fill_Contacts_Grid(string.Empty);
        }


        public override void PopulateToolbar(ToolBar toolBar)
        {
            base.PopulateToolbar(toolBar);

            bool toolbarButtonInitialized = ToolBarNew != null;
            ToolBarButton divider = new ToolBarButton();
            divider.Style = ToolBarButtonStyle.Separator;

            if (ToolBarNew == null)
            {
                ToolBarNew = new ToolBarButton();
                ToolBarNew.Tag = "New";
                ToolBarNew.ToolTipText = "Add Service Plan";
                ToolBarNew.Enabled = true;
                ToolBarNew.ImageSource = "captain-add";
                ToolBarNew.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarNew.Click += new EventHandler(OnToolbarButtonClicked);

                ToolBarEdit = new ToolBarButton();
                ToolBarEdit.Tag = "Edit";
                ToolBarEdit.ToolTipText = "Edit Service Plan";
                ToolBarEdit.Enabled = true;
                ToolBarEdit.ImageSource = "captain-edit";
                ToolBarEdit.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarEdit.Click += new EventHandler(OnToolbarButtonClicked);

                ToolBarDel = new ToolBarButton();
                ToolBarDel.Tag = "Delete";
                ToolBarDel.ToolTipText = "Delete Service Plan";
                ToolBarDel.Enabled = true;
                ToolBarDel.ImageSource = "captain-delete";
                ToolBarDel.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarDel.Click += new EventHandler(OnToolbarButtonClicked);

                ToolBarPrint = new ToolBarButton();
                ToolBarPrint.Tag = "Print";
                ToolBarPrint.ToolTipText = "Print Service Plan";
                ToolBarPrint.Enabled = true;
                ToolBarPrint.ImageSource = "captain-print";
                ToolBarPrint.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarPrint.Click += new EventHandler(OnToolbarButtonClicked);

                ToolBarHelp = new ToolBarButton();
                ToolBarHelp.Tag = "Help";
                ToolBarHelp.ToolTipText = "Service Plan Help";
                ToolBarHelp.Enabled = true;
                ToolBarHelp.ImageSource = "icon-help";
                ToolBarHelp.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarHelp.Click += new EventHandler(OnToolbarButtonClicked);
            }
            if (Privileges.AddPriv.Equals("false"))   // Yeswanth
            {
                ToolBarNew.Enabled = Pb_Add_Contact.Visible = picReferAdd.Visible = false;
            }
            if (Privileges.ChangePriv.Equals("false"))
            {
                ToolBarEdit.Enabled = Pb_Edit_Contact.Visible = picReferEdit.Visible = false;
            }
            if (Privileges.DelPriv.Equals("false"))
            {
                ToolBarDel.Enabled = Pb_Delete_Contact.Visible = picReferDelete.Visible = false;
            }

            if (IsServiceStop)
            {
                ToolBarNew.Enabled = Pb_Add_Contact.Visible = picReferAdd.Visible = false;
                ToolBarEdit.Enabled = Pb_Edit_Contact.Visible = picReferEdit.Visible = false;
                ToolBarDel.Enabled = Pb_Delete_Contact.Visible = picReferDelete.Visible = false;
            }

            toolBar.Buttons.AddRange(new ToolBarButton[]
            {
                ToolBarNew,
                ToolBarEdit,
                ToolBarDel,
                ToolBarPrint,
                ToolBarHelp
            });
        }

        private void OnToolbarButtonClicked(object sender, EventArgs e)
        {
            ToolBarButton btn = (ToolBarButton)sender;
            StringBuilder executeCode = new StringBuilder();
            string Hierarchy = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg;
            executeCode.Append(Consts.Javascript.BeginJavascriptCode);
            if (btn.Tag == null) { return; }
            try
            {
                switch (btn.Tag.ToString())
                {
                    // BaseForm baseForm, string mode, string hierarchy,string year, string hieDesc, string site, string schDate, string type,
                    case Consts.ToolbarActions.New:
                        if (AdminControlValidation("CASE0006"))
                        {
                            Added_Edited_SPCode = string.Empty;
                            CASE5006Form CASE4006_Add = new CASE5006Form(BaseForm, "Add", "New SP", "1", BaseForm.BaseYear, Privileges, Hierarchy, BaseForm.BaseYear, BaseForm.BaseApplicationNo, CntlCAEntity, CntlMSEntity);
                            CASE4006_Add.FormClosed += new FormClosedEventHandler(SP_AddForm_Closed);
                            CASE4006_Add.StartPosition = FormStartPosition.CenterScreen;
                            CASE4006_Add.ShowDialog();
                        }
                        break;
                    case Consts.ToolbarActions.Edit:
                        if (AdminControlValidation("CASE0006"))
                        {
                            if (SPGrid.Rows.Count > 0)
                            {
                                if (Can_Access_SP_in_SelHie(SPGrid.CurrentRow.Cells["SP_Code"].Value.ToString()))
                                {
                                    if (SPGrid.CurrentRow.Cells["SP_Valid"].Value.ToString() == "Y")
                                    {
                                        CASE5006Form CASE4006_Edit = new CASE5006Form(BaseForm, "Edit", SPGrid.CurrentRow.Cells["SP_Code"].Value.ToString(), SPGrid.CurrentRow.Cells["SP_Sequence"].Value.ToString(), SPGrid.CurrentRow.Cells["SP_Year"].Value.ToString(), Privileges, Hierarchy, BaseForm.BaseYear, BaseForm.BaseApplicationNo, CntlCAEntity, CntlMSEntity);
                                        CASE4006_Edit.FormClosed += new FormClosedEventHandler(SP_AddForm_Closed);
                                        CASE4006_Edit.StartPosition = FormStartPosition.CenterScreen;
                                        CASE4006_Edit.ShowDialog();
                                    }
                                    else
                                        MessageBox.Show("Please Validate Service Plan to Edit ", "CAP Systems");
                                }
                                else
                                    MessageBox.Show("Selected Service Plan not Accessible in this Hierarchy ", "CAP Systems");
                            }
                        }
                        break;
                    case Consts.ToolbarActions.Print:
                        On_SaveForm_Closed();
                        break;
                    case Consts.ToolbarActions.Delete:
                        if (SPGrid.Rows.Count > 0)
                        {
                            if ((int.Parse(SPGrid.CurrentRow.Cells["CA_Count"].Value.ToString()) == 0) && (int.Parse(SPGrid.CurrentRow.Cells["MS_Count"].Value.ToString()) == 0))
                                //MessageBox.Show(Consts.Messages.AreYouSureYouWantToDelete.GetMessage() + "\n Posting for Service Plan : " + SPGrid.CurrentRow.Cells["SP_Code"].Value.ToString(), Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, Delete_Selected_SPM, true);
                                MessageBox.Show("Are you sure you want to delete the Service Plan? ", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Delete_Selected_SPM);
                            else
                                MessageBox.Show("You Can’t delete this Service Plan \n As CA and/or MS Details already posted ", "CAP Systems");
                        }
                        break;




                    //    // TMS00110_Delete = new (BaseForm, "Delete", MainMenuHierarchy, MainMenuYear, Pass_Site, Pass_Date, Privileges); //string site, string schDate, 
                    //    //TMS00110_Delete.ShowDialog();
                    //    if (SPGrid.Rows.Count > 0) //(CanDeleteHeaderCode())
                    //    {
                    //        if (CanDeleteSP())
                    //            MessageBox.Show(Consts.Messages.AreYouSureYouWantToDelete.GetMessage() + "\nService Plan : " + SPGrid.CurrentRow.Cells["Code"].Value.ToString(), Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, OnDeleteMessageBoxClicked, true);
                    //        else
                    //            MessageBox.Show("You can not Delete Service Plan: '" + SPGrid.CurrentRow.Cells["Code"].Value.ToString() + "' \n CA/MS(s) Associated with this Service Plan", "CAP Systems", MessageBoxButtons.OK);
                    //    }
                    //    //else
                    //    //    MessageBox.Show("Please Select 'Site' \n To Delete Appointment", "CAP Systems", MessageBoxButtons.OK);
                    //    break;
                    case Consts.ToolbarActions.Help:
                        Application.Navigate(CommonFunctions.BuildHelpURLS(Privileges.Program, 0, BaseForm.BusinessModuleID.ToString()), target: "_blank");
                        break;
                }
                executeCode.Append(Consts.Javascript.EndJavascriptCode);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogAndDisplayMessageToUser(new StackFrame(true), ex, QuantumFaults.None, ExceptionSeverityLevel.High);
            }
        }


        private void SP_AddForm_Closed(object sender, FormClosedEventArgs e)
        {
            CASE5006Form form = sender as CASE5006Form;
            if (form.DialogResult == DialogResult.OK)
            {
                string[] From_Results = new string[2];
                From_Results = form.GetSelected_SP_Code();
                Added_Edited_SPCode = From_Results[0];

                if (From_Results[0].Equals("Add"))
                    MessageBox.Show("Service Plan Posting Successful", "CAP Systems");

                btnSearch_Click();
            }
        }


        private void Delete_Selected_SPM(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                CASESPMEntity Search_Entity = new CASESPMEntity(true);
                Search_Entity.Rec_Type = "D";
                Search_Entity.agency = BaseForm.BaseAgency;
                Search_Entity.dept = BaseForm.BaseDept;
                Search_Entity.program = BaseForm.BaseProg;

                //if (!string.IsNullOrEmpty(MainMenuYear))
                //    Search_Entity.year = MainMenuYear;
                Search_Entity.year = SPGrid.CurrentRow.Cells["SP_Year"].Value.ToString();
                Search_Entity.app_no = BaseForm.BaseApplicationNo;
                Search_Entity.service_plan = SPGrid.CurrentRow.Cells["SP_Code"].Value.ToString();
                Search_Entity.Seq = SPGrid.CurrentRow.Cells["SP_Sequence"].Value.ToString();

                Search_Entity.lstc_operator = BaseForm.UserID;

                //Search_Entity.caseworker = Search_Entity.site = null;
                //Search_Entity.startdate = Search_Entity.estdate = Search_Entity.compdate = null;
                //Search_Entity.sel_branches = Search_Entity.have_addlbr = Search_Entity.date_lstc = null;
                //Search_Entity.date_add = Search_Entity.add_operator = null;
                //Search_Entity.Sp0_Desc = Search_Entity.Sp0_Validatetd = null;

                if (_model.SPAdminData.UpdateCASESPM(Search_Entity, "Delete", out Sql_SP_Result_Message, out Tmp_SPM_Sequence))
                {
                    MessageBox.Show("Service Plan Deleteted Successfully", "CAP Systems");
                    btnSearch_Click();
                }
                else
                    MessageBox.Show(Sql_SP_Result_Message, "CAP Systems");
                //MessageBox.Show("Unsuccessful Service Plan Delete \n Reason : " + Sql_SP_Result_Message, "CAP Systems");
            }
        }


        //private bool Delete_Selected_SPM()
        //{
        //    bool Delete_Status = false;

        //    CASESPMEntity Search_Entity = new CASESPMEntity();
        //    Search_Entity.Rec_Type = "D";
        //    Search_Entity.agency = MainMenuAgency;
        //    Search_Entity.dept = MainMenuDept;
        //    Search_Entity.program = MainMenuProgram;
        //    Search_Entity.year = MainMenuYear;
        //    Search_Entity.app_no = MainMenuAppNo;
        //    Search_Entity.service_plan = SPGrid.CurrentRow.Cells["SP_Code"].Value.ToString();

        //    Search_Entity.caseworker = Search_Entity.site = null;
        //    Search_Entity.startdate = Search_Entity.estdate = Search_Entity.compdate = null;
        //    Search_Entity.sel_branches = Search_Entity.have_addlbr = Search_Entity.date_lstc = null;
        //    Search_Entity.lstc_operator = Search_Entity.date_add = Search_Entity.add_operator = null;
        //    Search_Entity.Sp0_Desc = Search_Entity.Sp0_Validatetd = null;

        //    if (_model.SPAdminData.UpdateCASESPM(Search_Entity, "Delete"))
        //        MessageBox.Show("Service Plan Posting Successful", "CAP Systems");

        //    return Delete_Status;
        //}





        private void CASE4006Control_Load(object sender, EventArgs e)
        {

        }

        private void CmbSPValid_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void GetSelectedProgram()
        {
            if (BaseForm.ContentTabs.TabPages[0].Controls[0] is MainMenuControl)
            {
                MainMenuControl mainMenuControl = (BaseForm.ContentTabs.TabPages[0].Controls[0] as MainMenuControl);
                MainMenuAgency = BaseForm.BaseAgency;
                MainMenuDept = BaseForm.BaseDept;
                MainMenuProgram = BaseForm.BaseProg;

                MainMenuYear = "    ";
                if (!string.IsNullOrEmpty(BaseForm.BaseYear))
                    MainMenuYear = BaseForm.BaseYear;
                MainMenuAppNo = BaseForm.BaseApplicationNo;

                MainMenuHierarchy = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg;
                MainMenuHIE = mainMenuControl.AgencyName + "     " + mainMenuControl.DeptName + "     " + mainMenuControl.ProgramName;
            }
        }

        List<Captain.Common.Utilities.ListItem> CaseWorker_List = new List<Captain.Common.Utilities.ListItem>();
        private void Fill_CaseWorker()
        {
            try
            {


                //DataSet ds2 = Captain.DatabaseLayer.AgyTab.GetHierarchyNames(MainMenuAgency, MainMenuDept, MainMenuProgram);
                DataSet ds2 = Captain.DatabaseLayer.AgyTab.GetHierarchyNames(BaseForm.BaseAgency, "**", "**");
                string strNameFormat = null, strCwFormat = null;
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    strNameFormat = ds2.Tables[0].Rows[0]["HIE_CN_FORMAT"].ToString();
                    strCwFormat = ds2.Tables[0].Rows[0]["HIE_CW_FORMAT"].ToString();
                }

                DataSet ds1 = Captain.DatabaseLayer.CaseMst.GetCaseWorker(strCwFormat, BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
                if (ds1.Tables.Count > 0)
                {
                    DataTable dt1 = ds1.Tables[0];
                    if (dt1.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt1.Rows)
                            CaseWorker_List.Add(new Captain.Common.Utilities.ListItem(dr["NAME"].ToString().Trim(), dr["PWH_CASEWORKER"].ToString().Trim()));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        string Added_Edited_SPCode = string.Empty;
        List<CASESPMEntity> CASESPM_List = new List<CASESPMEntity>();
        private void btnSearch_Click()
        {
            SPGrid.Rows.Clear();
            CASESPMEntity Search_Entity = new CASESPMEntity(true);

            Search_Entity.agency = BaseForm.BaseAgency;
            Search_Entity.dept = BaseForm.BaseDept;
            Search_Entity.program = BaseForm.BaseProg;

            //Search_Entity.year = BaseForm.BaseYear;        
            Search_Entity.year = null;                // Year will be always Four-Spaces in CASESPM
            Search_Entity.app_no = BaseForm.BaseApplicationNo;

            //Search_Entity.service_plan = Search_Entity.caseworker = Search_Entity.site = null;
            //Search_Entity.startdate = Search_Entity.estdate = Search_Entity.compdate = null;
            //Search_Entity.sel_branches = Search_Entity.have_addlbr = Search_Entity.date_lstc = null;
            //Search_Entity.lstc_operator = Search_Entity.date_add = Search_Entity.add_operator = null;
            //Search_Entity.Sp0_Desc = Search_Entity.Sp0_Validatetd = null;
            CASESPM_List = _model.SPAdminData.Browse_CASESPM(Search_Entity, "Browse");

            Fill_SP_Grid(Added_Edited_SPCode);
        }


        private void Fill_SP_Grid(string Sel_SP_Code)
        {
            try
            {


                int TmpCount = 0;
                Btn_Triggers.Visible = false;

                if (!string.IsNullOrEmpty(Sel_SP_Code))
                    Sel_SP_Code = "000000".Substring(0, (6 - Sel_SP_Code.Length)) + Sel_SP_Code;

                if (CASESPM_List.Count > 0)
                {
                    string Tmp_SPCoce = null, Start_Date = null;
                    int Sel_SP_Index = 0;

                    //List<CASESPMEntity> Tmp_CASESPM_List = new List<CASESPMEntity>();
                    //List<Captain.Common.Utilities.ListItem> Sort_list = new List<Captain.Common.Utilities.ListItem>(); 
                    //foreach (CASESPMEntity Entity in CASESPM_List)
                    //    Sort_list.Add(new Captain.Common.Utilities.ListItem(Entity.startdate.ToString(), Entity.year+Entity.Seq));

                    //Sort_list.Sort(delegate(Captain.Common.Utilities.ListItem p1, Captain.Common.Utilities.ListItem p2) { return p1.Text.CompareTo(p2.Text); });

                    int rowIndex = 0;
                    //foreach (Captain.Common.Utilities.ListItem List in Sort_list)
                    {
                        foreach (CASESPMEntity Entity in CASESPM_List)
                        {
                            //if (List.Value.ToString() == Entity.year + Entity.Seq)
                            {
                                Tmp_SPCoce = Entity.service_plan.ToString();
                                Tmp_SPCoce = "000000".Substring(0, (6 - Tmp_SPCoce.Length)) + Tmp_SPCoce;

                                if (!string.IsNullOrEmpty(Entity.startdate))
                                    Start_Date = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Entity.startdate.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                                if (Entity.have_addlbr == "Y")
                                    rowIndex = SPGrid.Rows.Add(Tmp_SPCoce, Entity.Sp0_Desc, Entity.Site_Desc, Get_CaseWorker_DESC(Entity.caseworker), Start_Date, Img_Tick, Entity.Sp0_Validatetd, Entity.CA_Postings_Cnt, Entity.MS_Postings_Cnt, Entity.Seq, Entity.year);
                                else
                                    rowIndex = SPGrid.Rows.Add(Tmp_SPCoce, Entity.Sp0_Desc, Entity.Site_Desc, Get_CaseWorker_DESC(Entity.caseworker), Start_Date, Img_Blank, Entity.Sp0_Validatetd, Entity.CA_Postings_Cnt, Entity.MS_Postings_Cnt, Entity.Seq, Entity.year);

                                if (Sel_SP_Code.Trim() == Tmp_SPCoce)
                                    Sel_SP_Index = TmpCount;

                                set_SP_Tooltip(TmpCount, Entity);

                                TmpCount++;

                                if (Entity.Sp0_Validatetd == "N" || Entity.Sp0_Validatetd != "Y")
                                    SPGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red; //Color.Peru; //Color.DarkTurquoise;

                                //break;
                            }
                        }

                    }
                    if (TmpCount > 0)
                    {
                        //SPGrid.Sort(SPGrid.Columns[4], ListSortDirection.Descending);
                        if (string.IsNullOrEmpty(Sel_SP_Code))
                            SPGrid.Rows[0].Tag = 0;
                        else
                        {
                            SPGrid.CurrentCell = SPGrid.Rows[Sel_SP_Index].Cells[1];

                            int scrollPosition = 0;
                            scrollPosition = SPGrid.CurrentCell.RowIndex;
                            //int CurrentPage = (scrollPosition / SPGrid.ItemsPerPage);
                            //CurrentPage++;
                            // SPGrid.CurrentPage = CurrentPage;
                            // SPGrid.FirstDisplayedScrollingRowIndex = scrollPosition;
                        }

                        //if (BaseForm.BusinessModuleID == "08")  //Commented by Sudheer on 10/12/2016
                        //    Btn_Triggers.Visible = true;
                    }
                }
                if(SPGrid.Rows.Count>0)
                    SPGrid.Rows[0].Selected = true;
                ////else
                ////    MessageBox.Show("No Service Plans Posted for this Applicant ", "CAP Systems");
            }
            catch (Exception ex)
            {
            }
        }

        private string Get_CaseWorker_DESC(string Worker_Code)
        {
            string DESC = null;
            foreach (Captain.Common.Utilities.ListItem List in CaseWorker_List)
            {
                if (List.Value.ToString().Trim() == Worker_Code.Trim())
                {
                    DESC = List.Text; break;
                }
            }

            return DESC;
        }

        private void set_SP_Tooltip(int rowIndex, CASESPMEntity Entity)
        {
            string toolTipText = "Added By     : " + Entity.add_operator.Trim() + " on " + Entity.date_add.ToString() + "\n" +
                                 "Modified By  : " + Entity.lstc_operator.Trim() + " on " + Entity.date_lstc.ToString();

            foreach (DataGridViewCell cell in SPGrid.Rows[rowIndex].Cells)
                cell.ToolTipText = toolTipText;
        }

        private void set_Contact_Tooltip(int rowIndex, CASECONTEntity Entity)
        {
            string toolTipText = "Added By     : " + Entity.Add_Operator.Trim() + " on " + Entity.Add_Date.ToString() + "\n" +
                                 "Modified By  : " + Entity.Lsct_Operator.Trim() + " on " + Entity.Lstc_Date.ToString();

            foreach (DataGridViewCell cell in ContactGrid.Rows[rowIndex].Cells)
                cell.ToolTipText = toolTipText;
        }


        private void set_Reffer_Tooltip(int rowIndex, ACTREFSEntity Entity)
        {
            string toolTipText = "Added By     : " + Entity.Add_Operator.Trim() + " on " + Entity.Add_Date.ToString() + "\n" +
                                 "Modified By  : " + Entity.Lsct_Operator.Trim() + " on " + Entity.Lstc_Date.ToString();

            foreach (DataGridViewCell cell in Ref_Grid.Rows[rowIndex].Cells)
                cell.ToolTipText = toolTipText;
        }


        private void Txt_SPCode_LostFocus(object sender, EventArgs e)
        {

        }



        CASECONTEntity Cont_Search_Entity = new CASECONTEntity();
        List<CASECONTEntity> CASECONT_List = new List<CASECONTEntity>();

        private void Fill_Contacts_Grid(string Sel_Cont_Seq)
        {
            try
            {
                //    //CASECONTEntity Cont_Search_Entity = new CASECONTEntity();
                //    //List<CASECONTEntity> CASECONT_List = new List<CASECONTEntity>();

                Cont_Search_Entity.Agency = BaseForm.BaseAgency;
                Cont_Search_Entity.Dept = BaseForm.BaseDept;
                Cont_Search_Entity.Program = BaseForm.BaseProg;
                Cont_Search_Entity.App_no = BaseForm.BaseApplicationNo;

                Cont_Search_Entity.Year = "    ";
                if (!string.IsNullOrEmpty(BaseForm.BaseYear))
                    Cont_Search_Entity.Year = BaseForm.BaseYear;

                Cont_Search_Entity.Contact_No = Cont_Search_Entity.Contact_Name = Cont_Search_Entity.CaseWorker = Cont_Search_Entity.Cont_Date = null;
                Cont_Search_Entity.Duration_Type = Cont_Search_Entity.Time = Cont_Search_Entity.Time_Starts = Cont_Search_Entity.Time_Ends = null;
                Cont_Search_Entity.Duration = Cont_Search_Entity.How_Where = null;
                Cont_Search_Entity.Language = Cont_Search_Entity.Interpreter = Cont_Search_Entity.Refer_From = Cont_Search_Entity.BillTO = Cont_Search_Entity.BillTo_UOM = null;
                Cont_Search_Entity.Cust1_Code = Cont_Search_Entity.Cust1_Value = Cont_Search_Entity.Cust2_Code = Cont_Search_Entity.Cust2_Value = Cont_Search_Entity.Cust3_Code = null;

                Cont_Search_Entity.Cust3_Value = Cont_Search_Entity.Bridge = Cont_Search_Entity.Lsct_Operator = Cont_Search_Entity.Lstc_Date = Cont_Search_Entity.Add_Date = null;
                Cont_Search_Entity.Add_Operator = null;

                int TmpCount = 0, Cont_Sel_Index = 0;
                CASECONT_List = _model.SPAdminData.Browse_CASECONT(Cont_Search_Entity, "Browse");

                Pb_Delete_Contact.Visible = false;
                Pb_Edit_Contact.Visible = false;

                ContactGrid.Rows.Clear();
                if (CASECONT_List.Count > 0)
                {
                    string Tmp_SPCoce = null, Start_Date = null;
                    int rowIndex = 0;
                    foreach (CASECONTEntity Entity in CASECONT_List)
                    {

                        rowIndex = 0;
                        //Tmp_SPCoce = Entity.service_plan.ToString();
                        //Tmp_SPCoce = "000000".Substring(0, (6 - Tmp_SPCoce.Length)) + Tmp_SPCoce;

                        Start_Date = CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Entity.Cont_Date.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);



                        if (int.Parse(Entity.Notes_Count) > 0)
                            //rowIndex = ContactGrid.Rows.Add(Start_Date, Entity.Contact_Name, Entity.How_Where, Entity.Duration, Get_Language_Desc(Entity.Language), Get_CaseWorker_DESC(Entity.CaseWorker), Entity.Seq, Img_Tick); //changed by sudheer 12/14/2016
                            rowIndex = ContactGrid.Rows.Add(Start_Date, Entity.Contact_Name, Entity.How_Where, Entity.Duration, Get_Language_Desc(Entity.Language), Get_CaseWorker_DESC(Entity.CaseWorker), Img_Tick, Entity.Seq);
                        else
                            //rowIndex = ContactGrid.Rows.Add(Start_Date, Entity.Contact_Name, Entity.How_Where, Entity.Duration, Get_Language_Desc(Entity.Language), Get_CaseWorker_DESC(Entity.CaseWorker), Entity.Seq, Img_Blank); //changed by sudheer 12/14/2016
                            rowIndex = ContactGrid.Rows.Add(Start_Date, Entity.Contact_Name, Entity.How_Where, Entity.Duration, Get_Language_Desc(Entity.Language), Get_CaseWorker_DESC(Entity.CaseWorker), Img_Blank, Entity.Seq);

                        if (Sel_Cont_Seq == Entity.Seq)
                            Cont_Sel_Index = TmpCount;

                        set_Contact_Tooltip(TmpCount, Entity);
                        TmpCount++;

                        //if (Entity.Sp0_Validatetd == "N" || Entity.Sp0_Validatetd != "Y")
                        //    SPGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red; //Color.Peru; //Color.DarkTurquoise;

                    }
                    if (TmpCount > 0)
                    {

                        if (string.IsNullOrEmpty(Sel_Cont_Seq))
                            ContactGrid.Rows[0].Tag = 0;
                        else
                        {
                            ContactGrid.CurrentCell = ContactGrid.Rows[Cont_Sel_Index].Cells[1];

                            int scrollPosition = 0;
                            scrollPosition = ContactGrid.CurrentCell.RowIndex;
                            //int CurrentPage = (scrollPosition / ContactGrid.ItemsPerPage);
                            //CurrentPage++;
                            //ContactGrid.CurrentPage = CurrentPage;
                            //ContactGrid.FirstDisplayedScrollingRowIndex = scrollPosition;

                            string Tmp_Seq = ContactGrid.CurrentRow.Cells["Cont_Seq"].Value.ToString();
                            Sel_Cont_Notes_Key = MainMenuHierarchy + BaseForm.BaseYear + BaseForm.BaseApplicationNo + "0000".Substring(0, (4 - Tmp_Seq.Length)) + Tmp_Seq;
                        }

                        if (Privileges.DelPriv.Equals("true"))
                            Pb_Delete_Contact.Visible = true;
                        if (Privileges.ChangePriv.Equals("true"))
                            Pb_Edit_Contact.Visible = true;

                        Pb_Cont_Notes.Visible = true;
                    }
                }
                else
                {
                    Pb_Delete_Contact.Visible = false;
                    Pb_Cont_Notes.Visible = false;
                    Pb_Edit_Contact.Visible = false;
                }

                if(ContactGrid.Rows.Count>0)
                    ContactGrid.Rows[0].Selected = true;
            }
            catch (Exception ex)
            {
            }
        }

        private void Btn_NewContact_Click(object sender, EventArgs e)
        {
            //if (AdminControlValidation("CASE0062"))
            //{
            string Hierarchy = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg;
            CASE0006_ContactsForm CASE4006_Add = new CASE0006_ContactsForm(BaseForm, "Add", Cont_Search_Entity, Privileges, Hierarchy, BaseForm.BaseYear, BaseForm.BaseApplicationNo, CntlContactEntity);

            CASE4006_Add.FormClosed += new FormClosedEventHandler(Contact_Closed_Closed);
            CASE4006_Add.StartPosition = FormStartPosition.CenterScreen;
            CASE4006_Add.ShowDialog();
            //}
        }

        private void Contact_Closed_Closed(object sender, FormClosedEventArgs e)
        {
            CASE0006_ContactsForm form = sender as CASE0006_ContactsForm;
            if (form.DialogResult == DialogResult.OK)
            {
                Fill_Contacts_Grid(form.GetSelected_Contact_Code());
            }
        }



        private void ContactGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (ContactGrid.Rows.Count > 0)
                {
                    int ColIdx = ContactGrid.CurrentCell.ColumnIndex;
                    int RowIdx = ContactGrid.CurrentCell.RowIndex;

                    if (e.ColumnIndex == 6)
                    {
                        foreach (CASECONTEntity Entity in CASECONT_List)
                        {
                            if (Entity.Seq == ContactGrid.CurrentRow.Cells["Cont_Seq"].Value.ToString())
                                Cont_Search_Entity = Entity;
                        }

                        string Hierarchy = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg;

                        CASE0006_ContactsForm CASE4006_Add = new CASE0006_ContactsForm(BaseForm, "Edit", Cont_Search_Entity, Privileges, Hierarchy, BaseForm.BaseYear, BaseForm.BaseApplicationNo, CntlContactEntity);
                        CASE4006_Add.StartPosition = FormStartPosition.CenterScreen;
                        CASE4006_Add.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Pb_Delete_Contact_Click(object sender, EventArgs e)
        {

            //if ((int.Parse(SPGrid.CurrentRow.Cells["CA_Count"].Value.ToString()) == 0) && (int.Parse(SPGrid.CurrentRow.Cells["MS_Count"].Value.ToString()) == 0))
            string Confirm_Msg = "On " + ContactGrid.CurrentRow.Cells["Date"].Value.ToString() + "\n For : " + ContactGrid.CurrentRow.Cells["Name"].Value.ToString();
            MessageBox.Show(Consts.Messages.AreYouSureYouWantToDelete.GetMessage() + "\n Contact : " + Confirm_Msg, Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Delete_Selected_Contact);
        }

        private void Pb_Edit_Contact_Click(object sender, EventArgs e)
        {
            //if (AdminControlValidation("CASE0062"))
            //{
            foreach (CASECONTEntity Entity in CASECONT_List)
            {
                if (Entity.Seq == ContactGrid.CurrentRow.Cells["Cont_Seq"].Value.ToString())
                    Cont_Search_Entity = Entity;
            }

            string Hierarchy = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg;

            CASE0006_ContactsForm CASE4006_Add = new CASE0006_ContactsForm(BaseForm, "Edit", Cont_Search_Entity, Privileges, Hierarchy, BaseForm.BaseYear, BaseForm.BaseApplicationNo, CntlContactEntity);
            CASE4006_Add.FormClosed += new FormClosedEventHandler(Contact_Closed_Closed);
            CASE4006_Add.StartPosition = FormStartPosition.CenterScreen;
            CASE4006_Add.ShowDialog();
            // }
        }


        private void Delete_Selected_Contact(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                foreach (CASECONTEntity Entity in CASECONT_List)
                {
                    if (Entity.Seq == ContactGrid.CurrentRow.Cells["Cont_Seq"].Value.ToString())
                    {
                        Cont_Search_Entity = Entity; break;
                    }
                }

                Cont_Search_Entity.Rec_Type = "D";
                int New_Cont_Seq = 0;
                if (_model.SPAdminData.UpdateCASECONT(Cont_Search_Entity, Privileges.Program + "1", "Delete", out New_Cont_Seq, out Sql_SP_Result_Message))
                {
                    MessageBox.Show("Contact Deleted Successfully", "CAP Systems");
                    Fill_Contacts_Grid(string.Empty);
                }
                else
                    MessageBox.Show(Sql_SP_Result_Message, "CAP Systems");
            }
        }

        List<CommonEntity> LanguagesList = new List<CommonEntity>();
        private void Fill_Languages_List()
        {
            LanguagesList = _model.lookupDataAccess.GetPrimaryLanguage();
        }

        private string Get_Language_Desc(string LAng_Code)
        {
            string Lang_Desc = null;
            foreach (CommonEntity Entity in LanguagesList)
            {
                if (Entity.Code == LAng_Code)
                {
                    Lang_Desc = Entity.Desc; break;
                }
            }

            return Lang_Desc;
        }

        private void Pb_Cont_Notes_Click(object sender, EventArgs e)
        {
            //ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, "Add", Privileges, Sel_Cont_Notes_Key); //"CONT"+ ContactGrid.CurrentRow.Cells["Cont_Seq"].Value.ToString());
            ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, "Edit", Privileges, Sel_Cont_Notes_Key, "CONT"); //"CONT"+ ContactGrid.CurrentRow.Cells["Cont_Seq"].Value.ToString());
            Prog_Form.FormClosed += new FormClosedEventHandler(On_PROGNOTES_Closed);
            Prog_Form.StartPosition = FormStartPosition.CenterScreen;
            Prog_Form.ShowDialog();
        }

        private void On_PROGNOTES_Closed(object sender, FormClosedEventArgs e)
        {
            ProgressNotes_Form form = sender as ProgressNotes_Form;
            if (form.DialogResult == DialogResult.OK)
            {
                ContactGrid.CurrentRow.Cells["Cont_Notes"].Value = Img_Tick;
                Get_PROG_Notes_Status();
            }
        }


        List<CASESP1Entity> SP_Hierarchies = new List<CASESP1Entity>();
        private void Fill_Sel_Hierarchy_SPs()
        {
            SP_Hierarchies = _model.SPAdminData.Browse_CASESP1(null, BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
        }

        private bool Can_Access_SP_in_SelHie(string Sel_SP)
        {
            bool Can_Access = false;

            int SP_Code = int.Parse(Sel_SP);

            foreach (CASESP1Entity Entity in SP_Hierarchies)
            {
                if (SP_Code.ToString() == Entity.Code)
                {
                    Can_Access = true; break;
                }
            }

            return Can_Access;
        }


        string Sel_Cont_Notes_Key = null;
        private void ContactGrid_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (ContactGrid.Rows.Count > 0)
                {
                    //Sel_Cont_Notes_Key = MainMenuHierarchy + BaseForm.BaseYear + BaseForm.BaseApplicationNo + "CONT" + ContactGrid.CurrentRow.Cells["Cont_Seq"].Value.ToString();

                    string Tmp_Seq = string.Empty;
                    if (!string.IsNullOrEmpty(ContactGrid.CurrentRow.Cells["Cont_Seq"].Value.ToString().Trim())) //changed by sudheer on 12/13/2016
                        Tmp_Seq = ContactGrid.CurrentRow.Cells["Cont_Seq"].Value.ToString();
                    Sel_Cont_Notes_Key = MainMenuHierarchy + BaseForm.BaseYear + BaseForm.BaseApplicationNo + "0000".Substring(0, (4 - Tmp_Seq.Length)) + Tmp_Seq;
                    Get_PROG_Notes_Status();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void Get_PROG_Notes_Status()
        {
            List<CaseNotesEntity> caseNotesEntity = new List<CaseNotesEntity>();

            //caseNotesEntity = _model.TmsApcndata.GetCaseNotesScreenFieldName(Privileges.Program, Hierarchy + Year + App_No + "CONT" + Pass_Entity.Seq);
            caseNotesEntity = _model.TmsApcndata.GetCaseNotesScreenFieldName("CASE00061", Sel_Cont_Notes_Key);
            Pb_Cont_Notes.ImageSource = "captain-casenotesadd";

            if (caseNotesEntity.Count > 0)
                Pb_Cont_Notes.ImageSource = "captain-casenotes";
        }



        // Begin Report Section........................
        string Agency = null;
        string strFolderPath = string.Empty;
        string Random_Filename = null; string PdfName = null;
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
            //Header.HeaderRows = 1;

            PdfPTable table = new PdfPTable(4);
            table.TotalWidth = 500f;
            table.WidthPercentage = 100;
            table.LockedWidth = true;
            float[] widths = new float[] { 15f, 80f, 15f, 15f };
            table.SetWidths(widths);
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            //table.SpacingAfter = 15f;


            int X_Pos, Y_Pos;
            CASEACTEntity CA_Pass_Entity = new CASEACTEntity();
            List<CASEACTEntity> SP_Activity_Details = new List<CASEACTEntity>();
            CASEACTEntity Search_Activity_Details = new CASEACTEntity();
            List<CASEMSEntity> SP_MS_Details = new List<CASEMSEntity>();
            CASEMSEntity Search_MS_Details = new CASEMSEntity();
            List<CASESPMEntity> CaseSPM_List = new List<CASESPMEntity>();
            CASESPMEntity Search_CaseSPM_Entity = new CASESPMEntity(true);
            Search_CaseSPM_Entity.agency = BaseForm.BaseAgency; Search_CaseSPM_Entity.dept = BaseForm.BaseDept; Search_CaseSPM_Entity.program = BaseForm.BaseProg;
            Search_CaseSPM_Entity.app_no = BaseForm.BaseApplicationNo;
            CaseSPM_List = _model.SPAdminData.Browse_CASESPM(Search_CaseSPM_Entity, "Browse");
            DataSet dsSP_CaseSP2 = new DataSet(); DataTable dtSP_CaseSP2 = new DataTable();
            bool First = true; string SerVicePlan = null, Priv_ServicePlan = null; string SP_Desc = null;
            string CAMSDesc = null; string CaDate = null; string CaDate_Follow_on = null; string Branch = null, Priv_Branch = null;
            if (CaseSPM_List.Count > 0)
            {
                foreach (CASESPMEntity Entity in CaseSPM_List)
                {
                    SerVicePlan = Entity.service_plan.ToString().Trim();
                    string Branch_SPM = Entity.sel_branches.ToString().Trim();
                    int length = Entity.sel_branches.Length;
                    DataSet dsSP_Services = DatabaseLayer.SPAdminDB.Browse_CASESP0(SerVicePlan, null, null, null, null, null, null, null, null);
                    DataRow drSP_Services = dsSP_Services.Tables[0].Rows[0];
                    Priv_Branch = null;

                    for (int i = 0; i < length;)
                    {
                        string Temp_Branch = Entity.sel_branches.Substring(i, 1);
                        dsSP_CaseSP2 = DatabaseLayer.SPAdminDB.Browse_CASESP2(SerVicePlan, Temp_Branch, null, null);
                        dtSP_CaseSP2 = dsSP_CaseSP2.Tables[0];
                        if (SerVicePlan != Priv_ServicePlan)
                        {
                            SP_Desc = drSP_Services["sp0_description"].ToString().Trim();
                            Priv_Branch = null;
                            Priv_ServicePlan = SerVicePlan;
                        }

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
                                        //else
                                        //    Service_desc = "Additional Branch";
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

                                    string[] Header1 = { "Type#", "Description", "Date", "FUp-Date" };
                                    for (int j = 0; j < Header1.Length; ++j)
                                    {
                                        PdfPCell cell = new PdfPCell(new Phrase(Header1[j], TblFontBold));
                                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //cell.FixedHeight = 15f;
                                        cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                        cell.BorderWidthBottom = 0.7f;
                                        table.AddCell(cell);
                                    }

                                    Priv_Branch = Branch;
                                    First = false;

                                }

                                string CAMSType = dr["sp2_type"].ToString();

                                if (CAMSType == "CA")
                                {
                                    DataSet dsCAMAST = DatabaseLayer.SPAdminDB.Browse_CAMAST(null, dr["sp2_cams_code"].ToString().Trim(), null, null);
                                    if (dsCAMAST.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow drCAMAST = dsCAMAST.Tables[0].Rows[0];

                                        CAMSDesc = drCAMAST["CA_DESC"].ToString();
                                        CA_Pass_Entity.Agency = BaseForm.BaseAgency;
                                        CA_Pass_Entity.Dept = BaseForm.BaseDept;
                                        CA_Pass_Entity.Program = BaseForm.BaseProg;


                                        //CA_Pass_Entity.Year = BaseYear;                        
                                        CA_Pass_Entity.Year = Entity.year;                             // Year will be always Four-Spaces in CASEACT
                                        CA_Pass_Entity.App_no = BaseForm.BaseApplicationNo;
                                        CA_Pass_Entity.ACT_Code = dr["sp2_cams_code"].ToString().Trim();
                                        CA_Pass_Entity.Service_plan = Entity.service_plan;
                                        CA_Pass_Entity.Branch = Branch.Trim(); CA_Pass_Entity.Group = dr["sp2_orig_grp"].ToString().Trim();
                                        CA_Pass_Entity.ACT_Date = CA_Pass_Entity.ACT_Seq = CA_Pass_Entity.Site = CA_Pass_Entity.Fund1 = null;
                                        CA_Pass_Entity.Fund2 = CA_Pass_Entity.Fund3 = CA_Pass_Entity.Caseworker = CA_Pass_Entity.Vendor_No = null;
                                        CA_Pass_Entity.Check_Date = CA_Pass_Entity.Check_No = CA_Pass_Entity.Cost = CA_Pass_Entity.Followup_On = null;
                                        CA_Pass_Entity.Followup_Comp = CA_Pass_Entity.Followup_By = CA_Pass_Entity.Refer_Data = CA_Pass_Entity.Cust_Code1 = null;
                                        CA_Pass_Entity.Cust_Value1 = CA_Pass_Entity.Cust_Code2 = CA_Pass_Entity.Cust_Value2 = CA_Pass_Entity.Cust_Code3 = null;
                                        CA_Pass_Entity.Cust_Value3 = CA_Pass_Entity.Lstc_Date = CA_Pass_Entity.Lsct_Operator = CA_Pass_Entity.Add_Date = null;
                                        CA_Pass_Entity.Add_Operator = CA_Pass_Entity.ACT_ID = null; CA_Pass_Entity.Bulk = CA_Pass_Entity.Act_PROG = null;
                                        CA_Pass_Entity.Cust_Code4 = CA_Pass_Entity.Cust_Value4 = CA_Pass_Entity.Cust_Code5 = CA_Pass_Entity.Cust_Value5 = null;
                                        CA_Pass_Entity.Units = CA_Pass_Entity.UOM = CA_Pass_Entity.Curr_Grp = null;
                                        CA_Pass_Entity.SPM_Seq = Entity.Seq;

                                        SP_Activity_Details = _model.SPAdminData.Browse_CASEACT(CA_Pass_Entity, "Browse");
                                        SP_Activity_Details = SP_Activity_Details.OrderByDescending(u => Convert.ToDateTime(u.ACT_Date.Trim())).ToList();
                                        if (SP_Activity_Details.Count > 0)
                                        {
                                            string Priv_Type = null, Priv_Cams_Desc = null;
                                            foreach (CASEACTEntity entity in SP_Activity_Details)
                                            {
                                                CaDate = LookupDataAccess.Getdate(entity.ACT_Date).ToString();
                                                CaDate_Follow_on = LookupDataAccess.Getdate(entity.Followup_On).ToString();

                                                if (CAMSType.Trim() != Priv_Type)
                                                {
                                                    PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                    table.AddCell(RowType);
                                                    Priv_Type = CAMSType.Trim();
                                                }
                                                else
                                                {
                                                    PdfPCell RowType = new PdfPCell(new Phrase("", TableFont));
                                                    RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowType.Border = iTextSharp.text.Rectangle.BOX;
                                                    table.AddCell(RowType);
                                                }
                                                if (CAMSDesc.Trim() != Priv_Cams_Desc)
                                                {
                                                    PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                    table.AddCell(RowDesc);
                                                    Priv_Cams_Desc = CAMSDesc.Trim();
                                                }
                                                else
                                                {
                                                    PdfPCell RowDesc = new PdfPCell(new Phrase("", TableFont));
                                                    RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                                    RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                                    table.AddCell(RowDesc);
                                                    Priv_Cams_Desc = CAMSDesc.Trim();
                                                }
                                                PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                                RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                                table.AddCell(RowDate);

                                                PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                                RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                                RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
                                                table.AddCell(RowDate_Foolow);
                                            }
                                        }
                                        else
                                        {
                                            PdfPCell RowType = new PdfPCell(new Phrase("Service", TableFont));
                                            RowType.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowType.Border = iTextSharp.text.Rectangle.BOX;
                                            table.AddCell(RowType);

                                            PdfPCell RowDesc = new PdfPCell(new Phrase(CAMSDesc.Trim(), TableFont));
                                            RowDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDesc.Border = iTextSharp.text.Rectangle.BOX;
                                            table.AddCell(RowDesc);

                                            PdfPCell RowDate = new PdfPCell(new Phrase(CaDate, TableFont));
                                            RowDate.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate.Border = iTextSharp.text.Rectangle.BOX;
                                            table.AddCell(RowDate);

                                            PdfPCell RowDate_Foolow = new PdfPCell(new Phrase(CaDate_Follow_on, TableFont));
                                            RowDate_Foolow.HorizontalAlignment = Element.ALIGN_LEFT;
                                            RowDate_Foolow.Border = iTextSharp.text.Rectangle.BOX;
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

                                        CAMSDesc = drMSMast["MS_DESC"].ToString();
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
                                        Search_MS_Details.Year = Entity.year;                              // Year will be always Four-Spaces in CASEMS
                                        Search_MS_Details.App_no = BaseForm.BaseApplicationNo;
                                        Search_MS_Details.MS_Code = dr["sp2_cams_code"].ToString().Trim();
                                        Search_MS_Details.SPM_Seq = Entity.Seq;

                                        Search_MS_Details.Service_plan = Entity.service_plan;
                                        Search_MS_Details.Branch = Branch.Trim(); Search_MS_Details.Group = dr["sp2_orig_grp"].ToString().Trim();
                                        Search_MS_Details.ID = Search_MS_Details.Date = Search_MS_Details.CaseWorker = Search_MS_Details.Site = null;
                                        Search_MS_Details.Result = Search_MS_Details.OBF = Search_MS_Details.Add_Operator = null;
                                        Search_MS_Details.Lstc_Date = Search_MS_Details.Lsct_Operator = Search_MS_Details.Add_Date = Search_MS_Details.Bulk =
                                        Search_MS_Details.Acty_PROG = Search_MS_Details.Curr_Grp = null;

                                        SP_MS_Details = _model.SPAdminData.Browse_CASEMS(Search_MS_Details, "Browse");
                                        SP_MS_Details = SP_MS_Details.OrderByDescending(u => Convert.ToDateTime(u.Date.Trim())).ToList();
                                        if (SP_MS_Details.Count > 0)
                                        {

                                            foreach (CASEMSEntity entity in SP_MS_Details)
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
                        i++;
                    }
                    if (drSP_Services["SP0_ALLOW_ADLBRANCH"].ToString() == "Y")
                    {
                        List<CASESPM2Entity> casespm2List = new List<CASESPM2Entity>();
                        CASESPM2Entity Search_Entity2 = new CASESPM2Entity();

                        Search_Entity2.Agency = BaseForm.BaseAgency;
                        Search_Entity2.Dept = BaseForm.BaseDept;
                        Search_Entity2.Prog = BaseForm.BaseProg;
                        Search_Entity2.Year = Entity.year;
                        //Search_Entity2.Year = null;                         // Year will be always Four-Spaces in CASESPM2
                        Search_Entity2.App = BaseForm.BaseApplicationNo;
                        Search_Entity2.Spm_Seq = Entity.Seq;

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
                                        CA_Pass_Entity.Agency = BaseForm.BaseAgency;
                                        CA_Pass_Entity.Dept = BaseForm.BaseDept;
                                        CA_Pass_Entity.Program = BaseForm.BaseProg;


                                        //CA_Pass_Entity.Year = BaseYear;                        
                                        CA_Pass_Entity.Year = Entity.year;                             // Year will be always Four-Spaces in CASEACT
                                        CA_Pass_Entity.App_no = BaseForm.BaseApplicationNo;
                                        CA_Pass_Entity.ACT_Code = Spm2.CamCd.Trim().Trim();
                                        CA_Pass_Entity.Service_plan = SerVicePlan;
                                        CA_Pass_Entity.Branch = Spm2.Branch; CA_Pass_Entity.Group = Spm2.Group.Trim();
                                        CA_Pass_Entity.ACT_Date = CA_Pass_Entity.ACT_Seq = CA_Pass_Entity.Site = CA_Pass_Entity.Fund1 = null;
                                        CA_Pass_Entity.Fund2 = CA_Pass_Entity.Fund3 = CA_Pass_Entity.Caseworker = CA_Pass_Entity.Vendor_No = null;
                                        CA_Pass_Entity.Check_Date = CA_Pass_Entity.Check_No = CA_Pass_Entity.Cost = CA_Pass_Entity.Followup_On = null;
                                        CA_Pass_Entity.Followup_Comp = CA_Pass_Entity.Followup_By = CA_Pass_Entity.Refer_Data = CA_Pass_Entity.Cust_Code1 = null;
                                        CA_Pass_Entity.Cust_Value1 = CA_Pass_Entity.Cust_Code2 = CA_Pass_Entity.Cust_Value2 = CA_Pass_Entity.Cust_Code3 = null;
                                        CA_Pass_Entity.Cust_Value3 = CA_Pass_Entity.Lstc_Date = CA_Pass_Entity.Lsct_Operator = CA_Pass_Entity.Add_Date = null;
                                        CA_Pass_Entity.Add_Operator = CA_Pass_Entity.ACT_ID = null; CA_Pass_Entity.Bulk = CA_Pass_Entity.Act_PROG = null;
                                        CA_Pass_Entity.Cust_Code4 = CA_Pass_Entity.Cust_Value4 = CA_Pass_Entity.Cust_Code5 = CA_Pass_Entity.Cust_Value5 = null;
                                        CA_Pass_Entity.Units = CA_Pass_Entity.UOM = CA_Pass_Entity.Curr_Grp = null;
                                        CA_Pass_Entity.SPM_Seq = Entity.Seq; // Added By Yeswanth on 11/22/2013
                                        SP_Activity_Details = _model.SPAdminData.Browse_CASEACT(CA_Pass_Entity, "Browse");
                                        if (SP_Activity_Details.Count > 0)
                                        {
                                            string Priv_Type = null, Priv_Cams_Desc = null;
                                            foreach (CASEACTEntity entity in SP_Activity_Details)
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
                                        Search_MS_Details.Year = Entity.year;                              // Year will be always Four-Spaces in CASEMS
                                        Search_MS_Details.App_no = BaseForm.BaseApplicationNo;
                                        Search_MS_Details.MS_Code = Spm2.CamCd.Trim().Trim();
                                        Search_MS_Details.SPM_Seq = Entity.Seq;

                                        Search_MS_Details.Service_plan = SerVicePlan;
                                        Search_MS_Details.Branch = Spm2.Branch; Search_MS_Details.Group = Spm2.Group.Trim();
                                        Search_MS_Details.ID = Search_MS_Details.Date = Search_MS_Details.CaseWorker = Search_MS_Details.Site = null;
                                        Search_MS_Details.Result = Search_MS_Details.OBF = Search_MS_Details.Add_Operator = null;
                                        Search_MS_Details.Lstc_Date = Search_MS_Details.Lsct_Operator = Search_MS_Details.Add_Date = Search_MS_Details.Bulk =
                                        Search_MS_Details.Acty_PROG = Search_MS_Details.Curr_Grp = null;

                                        SP_MS_Details = _model.SPAdminData.Browse_CASEMS(Search_MS_Details, "Browse");
                                        if (SP_MS_Details.Count > 0)
                                        {

                                            foreach (CASEMSEntity entity in SP_MS_Details)
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

                    }

                }
                if (table.Rows.Count > 0)
                    document.Add(Header);
                document.Add(table);
            }
            else
            {
                PdfContentByte cb = writer.DirectContent;
                cb.BeginText();
                cb.SetFontAndSize(FontFactory.GetFont(FontFactory.TIMES).BaseFont, 15);
                cb.SetColorFill(BaseColor.RED);
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Services plans are not defined", 300, 650, 0);
                cb.EndText();

            }

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

        private void picReferAdd_Click(object sender, EventArgs e)
        {
            //if (AdminControlValidation("CASE0061"))
            //{
            GetActReferData();
            AgencyReferral_SubForm Ref_Form = new AgencyReferral_SubForm("Detail", ACTREFS_List, string.Empty, string.Empty, "Add", BaseForm);

            //            AgencyReferral_SubForm Ref_Form = new AgencyReferral_SubForm("Detail", Sel_REFS_List);
            Ref_Form.FormClosed += new FormClosedEventHandler(On_Referral_Select_Closed);
            Ref_Form.StartPosition = FormStartPosition.CenterScreen;
            Ref_Form.ShowDialog();
            // }
        }

        private void picReferEdit_Click(object sender, EventArgs e)
        {
            //if (AdminControlValidation("CASE0061"))
            //{
            GetReferData();
            if (propRefDate != string.Empty)
            {
                AgencyReferral_SubForm Ref_Form = new AgencyReferral_SubForm("Detail", ACTREFS_List, propRefDate, propReferFormTo, Consts.Common.Edit, BaseForm);

                //            AgencyReferral_SubForm Ref_Form = new AgencyReferral_SubForm("Detail", Sel_REFS_List);
                Ref_Form.FormClosed += new FormClosedEventHandler(On_Referral_Select_Closed);
                Ref_Form.StartPosition = FormStartPosition.CenterScreen;
                Ref_Form.ShowDialog();
            }
            //  }
        }

        private void picReferDelete_Click(object sender, EventArgs e)
        {
            GetReferData();
            if (propCode != string.Empty)
            {
                MessageBox.Show(Consts.Messages.AreYouSureYouWantToDelete.GetMessage(), Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: MessageBoxHandler);
            }

        }

        private void MessageBoxHandler(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                if (DeleteReferData(propCode, propReferFormTo, propRefDate))
                {
                    Get_ReferrTo_Data();
                    Fill_ReferrTo_Data();
                }
                //else
                //{
                //    MessageBox.Show("You can’t delete this member, as there are Dependents", Consts.Common.ApplicationCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}

            }
        }

        private void On_Referral_Select_Closed(object sender, FormClosedEventArgs e)
        {
            string SelRef_Name = null;

            AgencyReferral_SubForm form = sender as AgencyReferral_SubForm;
            if (form.DialogResult == DialogResult.OK)
            {
                //Sel_REFS_List = form.GetSelected_Referral_Entity();
                ACTREFS_List = form.GetSelected_Referral_Entity();
                DeleteReferData(string.Empty, form.Referfromto, form.ReferDate);
                Update_ReferrTo_in_CASEREFS(ACTREFS_List);
                Get_ReferrTo_Data();
                Fill_ReferrTo_Data();
            }
        }


        private void Update_ReferrTo_in_CASEREFS(List<ACTREFSEntity> ACTREFSentity)
        {
            ACTREFSEntity Search_ACTREF_Entity = new ACTREFSEntity(true);
            foreach (ACTREFSEntity dr in ACTREFSentity)
            {
                //Search_REFS_Entity.Rec_Type = "I";
                //Search_REFS_Entity.Code = dr.Cells["Ref_Code"]. Value.ToString();
                //Search_REFS_Entity.Service = Pass_CA_Entity.ACT_Code;


                Search_ACTREF_Entity.Rec_Type = "I";

                Search_ACTREF_Entity.Code = dr.Code;
                Search_ACTREF_Entity.Agency = BaseForm.BaseAgency;
                Search_ACTREF_Entity.Dept = BaseForm.BaseDept;
                Search_ACTREF_Entity.Program = BaseForm.BaseProg;
                Search_ACTREF_Entity.Year = BaseForm.BaseYear;
                Search_ACTREF_Entity.ApplNo = BaseForm.BaseApplicationNo;
                Search_ACTREF_Entity.Date = dr.Date;
                Search_ACTREF_Entity.Type = dr.Type;
                Search_ACTREF_Entity.Connected = dr.Connected;
                Search_ACTREF_Entity.Add_Operator = BaseForm.UserID;
                Search_ACTREF_Entity.Lsct_Operator = BaseForm.UserID;


                //_model.SPAdminData.UpdateCASEREFS(Search_REFS_Entity, "Insert");
                _model.SPAdminData.UpdateACTREFS(Search_ACTREF_Entity, "Insert", out Sql_SP_Result_Message);
            }
        }


        //Sel_REFS_List = _model.SPAdminData.Browse_CASEREFS(Search_REFS_Entity, "Browse");

        //   ACTREFSEntity Search_ACTREF_Entity = new ACTREFSEntity(true);
        //   Search_ACTREF_Entity.Act_ID = Pass_CA_Entity.ACT_ID.Trim();

        //   ACTREFS_List = _model.SPAdminData.Browse_ACTREFS(Search_ACTREF_Entity, "Browse");


        private void Fill_ReferrTo_Data()
        {
            bool Ref_Exists = false;
            string Active_Stat = "N";
            Ref_Grid.Rows.Clear();
            int rowIndex = 0;
            //foreach (CASEREFSEntity Entity in Sel_REFS_List)
            //{
            string strConnected = string.Empty;
            List<CommonEntity> commonconneted = BaseForm.BaseAgyTabsEntity.FindAll(u => u.AgyCode == "S0070" && u.Active == "Y").ToList();

            if (ACTREFS_List != null)
            {
                strConnected = string.Empty;
                foreach (ACTREFSEntity Entity in ACTREFS_List)
                {
                    Ref_Exists = false;
                    string strRefKey = Entity.Agency + Entity.Dept + Entity.Program + (Entity.Year.ToString().Trim() == string.Empty ? "    " : Entity.Year) + Entity.ApplNo + LookupDataAccess.Getdate(Entity.Date) + Entity.Code + Entity.Type;
                    foreach (CASEREFEntity Entity1 in CASEREFREF_List)
                    {
                        Active_Stat = Entity1.Active;
                        if (Entity1.Code == Entity.Code)
                        {
                            Ref_Exists = true;
                            if (commonconneted.Count > 0)
                            {
                                if (Entity.Connected.ToUpper().Trim() == "N" || Entity.Connected.ToUpper().Trim() == string.Empty)
                                    strConnected = string.Empty;
                                else
                                {
                                    CommonEntity connectddesc = commonconneted.Find(u => u.Code.ToUpper().Trim() == Entity.Connected.ToUpper().Trim());
                                    if (connectddesc != null)
                                    {
                                        strConnected = connectddesc.Desc;
                                    }
                                    else
                                    {
                                        strConnected = string.Empty;
                                    }

                                }

                            }
                            rowIndex = Ref_Grid.Rows.Add(Entity.Date, Entity.Type == "T" ? "Referred To" : "Referred From", Entity.Type, Entity1.Code, Entity1.Name1.Trim(), Entity1.City, Entity1.State, Entity1.Active, strConnected, strRefKey);
                            Ref_Grid.Rows[rowIndex].Tag = Entity;
                            break;
                        }
                    }
                    if (!Ref_Exists)
                    {
                        if (commonconneted.Count > 0)
                        {
                            if (Entity.Connected.ToUpper().Trim() == "N" || Entity.Connected.ToUpper().Trim() == string.Empty)
                                strConnected = string.Empty;
                            else
                                strConnected = commonconneted.Find(u => u.Code.ToUpper().Trim() == Entity.Connected.ToUpper().Trim()).Desc;
                        }
                        rowIndex = Ref_Grid.Rows.Add(Entity.Date, Entity.Type == "T" ? "Referred To" : "Referred From", Entity.Type, Entity.Code, "Not Defined in 'CASEREF'", " ", " ", " ", strConnected, strRefKey);
                        Ref_Grid.Rows[rowIndex].Tag = Entity;
                    }

                    if (Active_Stat != "Y")
                        Ref_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.MediumVioletRed;  // Color.Red;

                    set_Reffer_Tooltip(rowIndex, Entity);
                }
                if (ACTREFS_List.Count > 0)
                {
                    if (Privileges.ChangePriv.Equals("true"))
                        picReferEdit.Visible = true;

                    if (Privileges.DelPriv.Equals("true"))
                        picReferDelete.Visible = true;

                }
                else
                    picReferEdit.Visible = picReferDelete.Visible = false;
            }
            if(Ref_Grid.Rows.Count>0)
                Ref_Grid.Rows[0].Selected = true;
            //}
        }

        private void GetReferData()
        {
            propRefDate = string.Empty;
            propReferFormTo = string.Empty;
            propCode = string.Empty;
            if (Ref_Grid.Rows.Count > 0)
            {

                foreach (DataGridViewRow item in Ref_Grid.Rows)
                {
                    if (item.Selected)
                    {
                        propRefDate = item.Cells["RefDate"].Value.ToString();
                        propReferFormTo = item.Cells["Referfromto"].Value.ToString();
                        propCode = item.Cells["Ref_Code"].Value.ToString();
                    }

                }
            }
        }


        List<CASEREFEntity> CASEREFREF_List = new List<CASEREFEntity>();
        List<ACTREFSEntity> ACTREFS_List = new List<ACTREFSEntity>();
        CASEREFSEntity Search_REFS_Entity = new CASEREFSEntity(true);
        private void Get_ReferrTo_Data()
        {
            CASEREFEntity Search_REF_Entity = new CASEREFEntity(true);
            CASEREFREF_List = _model.SPAdminData.Browse_CASEREF(Search_REF_Entity, "Browse");
            GetActReferData();
        }

        public void GetActReferData()
        {
            ACTREFSEntity Search_ACTREF_Entity = new ACTREFSEntity(true);
            Search_ACTREF_Entity.Agency = BaseForm.BaseAgency;
            Search_ACTREF_Entity.Dept = BaseForm.BaseDept;
            Search_ACTREF_Entity.Program = BaseForm.BaseProg;
            Search_ACTREF_Entity.Year = BaseForm.BaseYear;
            Search_ACTREF_Entity.ApplNo = BaseForm.BaseApplicationNo;
            ACTREFS_List = _model.SPAdminData.Browse_ACTREFS(Search_ACTREF_Entity, "Browse");

        }

        private bool DeleteReferData(string strCode, string strReferType, string strReferDate)
        {
            ACTREFSEntity Search_ACTREF_Entity = new ACTREFSEntity(true);
            Search_ACTREF_Entity.Agency = BaseForm.BaseAgency;
            Search_ACTREF_Entity.Dept = BaseForm.BaseDept;
            Search_ACTREF_Entity.Program = BaseForm.BaseProg;
            Search_ACTREF_Entity.Year = BaseForm.BaseYear;
            Search_ACTREF_Entity.ApplNo = BaseForm.BaseApplicationNo;
            if (strCode != string.Empty)
            {
                Search_ACTREF_Entity.Code = strCode;
                Search_ACTREF_Entity.Rec_Type = "DS";
            }
            else
                Search_ACTREF_Entity.Rec_Type = "D";
            Search_ACTREF_Entity.Date = strReferDate;
            Search_ACTREF_Entity.Type = strReferType;
            return _model.SPAdminData.UpdateACTREFS(Search_ACTREF_Entity, "Delete", out Sql_SP_Result_Message);

        }


        // End Report Section..................

        public void RefreshAlertCode()
        {
            pnlAlertcode.Controls.Clear();
            alertCodesUserControl = new AlertCodes(BaseForm, Privileges, ProgramDefinition);
            alertCodesUserControl.Dock = DockStyle.Fill;
            pnlAlertcode.Controls.Add(alertCodesUserControl);
        }

        private void Btn_Triggers_Click(object sender, EventArgs e)
        {
            if (SPGrid.Rows.Count > 0)
            {
                ADMN4016PrintForm printForm = new ADMN4016PrintForm(BaseForm, "Print", "TRIGGER", Privileges, SPGrid.CurrentRow.Cells["SP_Code"].Value.ToString());
                printForm.StartPosition = FormStartPosition.CenterScreen;
                printForm.ShowDialog();
                //List<CT_Triggers_Entity> Trig_List = new List<CT_Triggers_Entity>();
                //Trig_List = _model.SPAdminData.Get_CT_Trigger_Report(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, null, null, null, null, SPGrid.CurrentRow.Cells["SP_Code"].Value.ToString());
            }
        }


        private void Ref_Grid_MenuClick(object objSource, MenuItemEventArgs objArgs)
        {
            try
            {
                if (Ref_Grid.Rows.Count > 0)
                {
                    ACTREFSEntity ActApplicant = GetSelectedRow();
                    if (ActApplicant != null)
                    {
                        ACTREFSEntity SearchEntity = new ACTREFSEntity();

                        SearchEntity.Rec_Type = "U";
                        SearchEntity.Agency = BaseForm.BaseAgency;
                        SearchEntity.Dept = BaseForm.BaseDept;
                        SearchEntity.Program = BaseForm.BaseProg;
                        SearchEntity.Year = BaseForm.BaseYear;
                        SearchEntity.ApplNo = BaseForm.BaseApplicationNo;
                        SearchEntity.Date = ActApplicant.Date;
                        SearchEntity.Code = ActApplicant.Code;
                        SearchEntity.Type = ActApplicant.Type;
                        //SearchEntity.Connected = "N";
                        SearchEntity.Lsct_Operator = BaseForm.UserID;
                        //if (objArgs.MenuItem.Text.Trim() == "Connected")
                        SearchEntity.Connected = objArgs.MenuItem.Tag.ToString();

                        if (_model.SPAdminData.UpdateACTREFS(SearchEntity, "Update", out Sql_SP_Result_Message))
                        {
                            GetActReferData();
                            Fill_ReferrTo_Data();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public ACTREFSEntity GetSelectedRow()
        {
            ACTREFSEntity caseSnpEntity = null;
            if (Ref_Grid != null)
            {
                foreach (DataGridViewRow dr in Ref_Grid.SelectedRows)
                {
                    if (dr.Selected)
                    {
                        strIndex = dr.Index;
                        caseSnpEntity = dr.Tag as ACTREFSEntity;
                        break;
                    }
                }
            }
            return caseSnpEntity;
        }

        //private void contextMenu1_Popup(object sender, EventArgs e)
        //{
        //    if (Ref_Grid.Rows.Count > 0)
        //    {
        //        contextMenu1.MenuItems.Clear();
        //        if (Ref_Grid.Rows[0].Tag is ACTREFSEntity)
        //        {

        //            ACTREFSEntity drow = Ref_Grid.SelectedRows[0].Tag as ACTREFSEntity;
        //            //if (!(CaseMST.FamilySeq.Equals(drow.FamilySeq)))
        //            //{
        //            contextMenu1.MenuItems.Clear();
        //            MenuItem menuLst = new MenuItem();
        //            menuLst.Text = " ";
        //            contextMenu1.MenuItems.Add(menuLst);
        //            menuLst.Text = "Connected";
        //            contextMenu1.MenuItems.Add(menuLst);
        //            //}
        //            //else
        //            //    contextMenu2.MenuItems.Clear();

        //        }
        //    }
        //}

        private void contextMenu2_Popup(object sender, EventArgs e)
        {
            if (ReferalConn == "Y")
            {
                if (Ref_Grid.Rows.Count > 0)
                {
                    contextMenu2.MenuItems.Clear();
                    if (Ref_Grid.Rows[0].Tag is ACTREFSEntity)
                    {
                        List<CommonEntity> commonconneted = BaseForm.BaseAgyTabsEntity.FindAll(u => u.AgyCode == "S0070" && u.Active == "Y").ToList();

                        foreach (CommonEntity dr in commonconneted)
                        {
                            string resDesc = dr.Desc.ToString().Trim();

                            MenuItem menuItem = new MenuItem();
                            menuItem.Text = resDesc;
                            menuItem.Tag = dr.Code;

                            contextMenu2.MenuItems.Add(menuItem);
                        }
                        //ACTREFSEntity drow = Ref_Grid.SelectedRows[0].Tag as ACTREFSEntity;
                        ////if (!(CaseMST.FamilySeq.Equals(drow.FamilySeq)))
                        ////{
                        //contextMenu2.MenuItems.Clear();
                        //MenuItem menuLst = new MenuItem();
                        //menuLst.Text = " ";
                        //contextMenu2.MenuItems.Add(menuLst);
                        //MenuItem menuLst1 = new MenuItem();
                        //menuLst1.Text = "Connected";
                        //contextMenu2.MenuItems.Add(menuLst1);
                        //}
                        //else
                        //    contextMenu2.MenuItems.Clear();

                    }
                }
            }
        }

        private void pb_ReferedNotes_Click(object sender, EventArgs e)
        {
            ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, "Edit", Privileges, Sel_Ref_Notes_Key, "REFERRED"); //"CONT"+ ContactGrid.CurrentRow.Cells["Cont_Seq"].Value.ToString());
            Prog_Form.FormClosed += new FormClosedEventHandler(On_PROGNOTESRef_Closed);
            Prog_Form.StartPosition = FormStartPosition.CenterScreen;
            Prog_Form.ShowDialog();
        }

        string Sel_Ref_Notes_Key = null;
        private void Ref_Grid_SelectionChanged(object sender, EventArgs e)
        {

            try
            {
                if (Ref_Grid.Rows.Count > 0)
                {
                    Sel_Ref_Notes_Key = string.Empty;
                    if (!string.IsNullOrEmpty(Ref_Grid.CurrentRow.Cells["gvtRefKey"].Value.ToString().Trim()))
                        Sel_Ref_Notes_Key = Ref_Grid.CurrentRow.Cells["gvtRefKey"].Value.ToString().Trim();
                    Get_PROG_Notes_RefStatus();
                }
            }
            catch (Exception ex)
            {
            }

        }

        private void Get_PROG_Notes_RefStatus()
        {
            List<CaseNotesEntity> caseNotesEntity = new List<CaseNotesEntity>();

            //caseNotesEntity = _model.TmsApcndata.GetCaseNotesScreenFieldName(Privileges.Program, Hierarchy + Year + App_No + "CONT" + Pass_Entity.Seq);
            caseNotesEntity = _model.TmsApcndata.GetCaseNotesScreenFieldName("CASE00065", Sel_Ref_Notes_Key);
            pb_ReferedNotes.ImageSource = "captain-casenotesadd";

            if (caseNotesEntity.Count > 0)
                pb_ReferedNotes.ImageSource = "captain-casenotes";
        }

        private void On_PROGNOTESRef_Closed(object sender, FormClosedEventArgs e)
        {
            //ProgressNotes_Form form = sender as ProgressNotes_Form;
            //if (form.DialogResult == DialogResult.OK)
            //{

            Get_PROG_Notes_RefStatus();
            //  }
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
                    if (ScaFldsHie.FindAll(u => u.Active != "Y").Count > 0)
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