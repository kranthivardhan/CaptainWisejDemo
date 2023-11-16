/************************************************************************
 * Conversion On    :   12/14/2022      * Converted By     :   Kranthi
 * Modified On      :   12/14/2022      * Modified By      :   Kranthi
 * **********************************************************************/

#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Collections;
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
using System.IO;
using System.Linq;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Text.RegularExpressions;
using System.Globalization;
using Wisej.Web;
using DevExpress.DataProcessing.InMemoryDataProcessor;

#endregion

namespace Captain.Common.Views.Forms
{
    public partial class PrintLetters : Form
    {
        #region private variables

        private ErrorProvider _errorProvider = null;
        private CaptainModel _model = null;
        private string strNameFormat = string.Empty;
        private string strCwFormat = string.Empty;
        private string strYear = "    ";
        private int strIndex = 0;

        #endregion

        public PrintLetters(BaseForm baseForm, PrivilegeEntity privileges, string Form_Name, string eligStatus, List<CASESPMEntity> casespmList, string strServicePlan, string strServiceSeq,string SecSourceName)
        {
            InitializeComponent();

            BaseForm = baseForm;
            Privileges = privileges;
            FormName = Form_Name;
            EligStatus = eligStatus;
            CASESPM_SP_List = casespmList;
            SecondarySourceName = SecSourceName;
            _model = new CaptainModel();

            this.Text = "Print Letters";
            lblAppNo.Text = BaseForm.BaseApplicationNo;
            lblName.Text = BaseForm.BaseApplicationName;
            propReportPath = _model.lookupDataAccess.GetReportPath();
            DataSet dsAgency = Captain.DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(BaseForm.BaseAgency, null, null, null, null, null, null);
            if (dsAgency != null && dsAgency.Tables[0].Rows.Count > 0)
            {
                AGYShortName = dsAgency.Tables[0].Rows[0]["ACR_SHORT_NAME"].ToString().Trim();
            }

            if (!string.IsNullOrEmpty(BaseForm.BaseYear.Trim()))
                CEAPCNTL_List = _model.SPAdminData.GetCEAPCNTLData(string.Empty, BaseForm.BaseYear, string.Empty, string.Empty);

            SPMCode = strServicePlan;
            SpmSeq = strServiceSeq;
            Getdata();
            FillGrid();
            strNameFormat = BaseForm.BaseHierarchyCnFormat.ToString();
            strFolderPath = Consts.Common.ReportFolderLocation + BaseForm.UserID + "\\";


        }

        public PrintLetters(BaseForm baseForm, PrivilegeEntity privileges, string Form_Name, string Agency, string Dept, string Program, string Year, string Value)
        {
            InitializeComponent();

            BaseForm = baseForm;
            Privileges = privileges;
            FormName = Form_Name;
            PAgency = Agency; PDept = Dept; PProgram = Program; PYear = Year; PValue = Value;
            _model = new CaptainModel();

            this.Text = "Print Letters";
            lblAppNo.Text = BaseForm.BaseApplicationNo;
            lblName.Text = BaseForm.BaseApplicationName;
            propReportPath = _model.lookupDataAccess.GetReportPath();

            strNameFormat = BaseForm.BaseHierarchyCnFormat.ToString();
            //FillGrid();

            strFolderPath = Consts.Common.ReportFolderLocation + BaseForm.UserID + "\\";

            Getdata();

        }


        #region properties

        public BaseForm BaseForm { get; set; }
        public PrivilegeEntity Privileges { get; set; }
        public string FormName { get; set; }
        public string EligStatus { get; set; }
        public string PAgency { get; set; }
        public string PDept { get; set; }
        public string PProgram { get; set; }
        public string PYear { get; set; }
        public string PValue { get; set; }
        public string AGYShortName { get; set; }
        public string SPMCode { get; set; }
        public string SpmSeq { get; set; }

        public string SecondarySourceName { get; set; }

        public List<CaseServicesEntity> PropSerViceEntity { get; set; }
        public string propReportPath { get; set; }
        public List<FldcntlHieEntity> preassesCntlEntity { get; set; }
        public List<TMS81ReportEntity> ReportDetails { get; set; }
        public List<AgyTabEntity> AgyList { get; set; }
        public AgyTabEntity AgyMain { get; set; }
        public List<CAMASTEntity> CAMASTList { get; set; }

        public List<CommonEntity> IncomeInterValList { get; set; }

        public List<CASESPMEntity> CASESPM_SP_List { get; set; }

        #endregion
        List<CEAPCNTLEntity> CEAPCNTL_List = new List<CEAPCNTLEntity>();
        List<HierarchyEntity> propCaseworkerList = new List<HierarchyEntity>();
        HierarchyEntity CaseWorker = new HierarchyEntity();
        private void Getdata()
        {
            CaseServicesEntity SearchEntity = new CaseServicesEntity(true);
            //SearchEntity.Agency = BaseForm.BaseAgency;
            //SearchEntity.Dept = BaseForm.BaseDept;
            //SearchEntity.Program = BaseForm.BaseProg;
            //SearchEntity.Application = "ES";
            PropSerViceEntity = _model.CaseMstData.Browse_CASESER(SearchEntity, "Browse");

            propCaseworkerList = _model.CaseMstData.GetCaseWorker("I", BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);

            if (propCaseworkerList.Count > 0)
                CaseWorker = propCaseworkerList.Find(u => u.UserID == BaseForm.UserID);

            CAMASTList = _model.SPAdminData.Browse_CAMAST("Code", null, null, null);

            AgyList = _model.Agytab.GetAgyTab(string.Empty);

            Get_Vendor_List();

            //IncomeInterValList = CommonFunctions.AgyTabsFilterCodeStatus(BaseForm.BaseAgyTabsEntity, "S0015", string.Empty, string.Empty, string.Empty, string.Empty);


        }

        List<CASEVDDEntity> CaseVddlist = new List<CASEVDDEntity>();
        private void Get_Vendor_List()
        {
            CASEVDDEntity Search_Entity = new CASEVDDEntity(true);
            CaseVddlist = _model.SPAdminData.Browse_CASEVDD(Search_Entity, "Browse");
        }

        private string Get_Vendor_Name(string VendorNo)
        {
            string Vend_Name = string.Empty;
            foreach (CASEVDDEntity Entity in CaseVddlist)
            {
                if (Entity.Code == VendorNo)
                {
                    Vend_Name = Entity.Name.Trim(); break;
                }
            }

            return Vend_Name;
        }

        private string CaseworkerName(string WorkerCode)
        {
            string Desc = string.Empty;

            if (propCaseworkerList.Count > 0)
            {
                HierarchyEntity WorkerList = propCaseworkerList.Find(u => u.CaseWorker.Trim() == WorkerCode);
                if (WorkerList != null)
                    Desc = WorkerList.HirarchyName.Trim();
                else Desc = WorkerCode;
            }

            return Desc;
        }

        string DEPState = string.Empty;
        private void FillGrid()
        {
            gvApp.Rows.Clear();
            int rowIndex = 0; DEPState = string.Empty;

            if (BaseForm.BaseAgencyControlDetails.AgyShortName.ToUpper() == "PCS" || BaseForm.BaseAgencyControlDetails.AgyShortName.ToUpper()=="NCCAA" || BaseForm.BaseAgencyControlDetails.AgyShortName.ToUpper() == "FTW")
            {
                List<LETRHISTCEntity> LetterList = _model.SPAdminData.GetLetrHistData(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear.Trim(), BaseForm.BaseApplicationNo);

                if (LetterList.Count > 0) LetterList = LetterList.OrderByDescending(u => u.DATE_ADD).ThenBy(u => u.LETR_CODE).ToList();

                LETRHISTCEntity LetterHist = new LETRHISTCEntity();
                List<LETRHISTCEntity> SelLetterList = new List<LETRHISTCEntity>();
                string LetDate = string.Empty, LetWorker = string.Empty;

                if (LetterList.Count > 0)
                {
                    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "1");
                    if (SelLetterList.Count > 0)
                    {
                        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                        LetterHist = SelLetterList[0];
                    }

                }
                if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }


                gvApp.Rows.Add(false, "Eligibilty Letter", LetDate, LetWorker, "1", "");
                LetterHist = null;
                if (LetterList.Count > 0)
                {
                    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "2");
                    if (SelLetterList.Count > 0)
                    {
                        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                        LetterHist = SelLetterList[0];
                    }

                }
                if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }

                gvApp.Rows.Add(false, "CEAP Priority Rating Form", LetDate, LetWorker, "2", "");
            }
            else
            {

                DataSet ds = Captain.DatabaseLayer.MainMenu.GetCaseDepForHierarchy(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
                DataTable casedep = ds.Tables[0];

                List<LETRHISTCEntity> LetterList = _model.SPAdminData.GetLetrHistData(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear.Trim(), BaseForm.BaseApplicationNo);

                if (LetterList.Count > 0) LetterList = LetterList.OrderByDescending(u => u.DATE_ADD).ThenBy(u => u.LETR_CODE).ToList();

                LETRHISTCEntity LetterHist = new LETRHISTCEntity();
                List<LETRHISTCEntity> SelLetterList = new List<LETRHISTCEntity>();
                string LetDate = string.Empty, LetWorker = string.Empty;

                if (casedep.Rows.Count > 0) DEPState = casedep.Rows[0]["DEP_STATE"].ToString().Trim();

                if (LetterList.Count > 0)
                {
                    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "1");
                    if (SelLetterList.Count > 0)
                    {
                        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                        LetterHist = SelLetterList[0];
                    }

                }
                if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }

                if (EligStatus == "P")
                {
                    gvApp.Rows.Add(true, "Delay-in-eligibility determination", LetDate, LetWorker, "1", "P");
                    rowIndex = 1;
                }
                else
                    gvApp.Rows.Add(false, "Delay-in-eligibility determination", LetDate, LetWorker, "1", "P");

                if (LetterList.Count > 0)
                {
                    LetterHist = new LETRHISTCEntity();
                    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "2").OrderByDescending(u => u.DATE_ADD.Trim()).ToList();
                    if (SelLetterList.Count > 0)
                    {
                        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                        LetterHist = SelLetterList[0];
                    }
                }
                if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }
                if (EligStatus == "E")
                {
                    gvApp.Rows.Add(true, "Eligibility/Denial Notification", LetDate, LetWorker, "2", "E");
                    rowIndex = 2;
                }
                else
                    gvApp.Rows.Add(false, "Eligibility/Denial Notification", LetDate, LetWorker, "2", "E");

                //if (LetterList.Count > 0)
                //{
                //    LetterHist = new LETRHISTCEntity();
                //    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "3").OrderByDescending(u => u.DATE_ADD.Trim()).ToList();
                //    if (SelLetterList.Count > 0)
                //    {
                //        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                //        LetterHist = SelLetterList[0];
                //    }
                //}
                //if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }
                //if (EligStatus == "D")
                //{
                //    gvApp.Rows.Add(true, "Denial Notice", LetDate, LetWorker, "3", "D");
                //    rowIndex = 3;
                //}
                //else
                //    gvApp.Rows.Add(false, "Denial Notice", LetDate, LetWorker, "3", "D");

                if (LetterList.Count > 0)
                {
                    LetterHist = new LETRHISTCEntity();
                    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "4").OrderByDescending(u => u.DATE_ADD.Trim()).ToList();
                    if (SelLetterList.Count > 0)
                    {
                        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                        LetterHist = SelLetterList[0];
                    }
                }
                if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }
                gvApp.Rows.Add(false, "Right to Appeal Notice", LetDate, LetWorker, "4", "D");

                if (LetterList.Count > 0)
                {
                    LetterHist = new LETRHISTCEntity();
                    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "5").OrderByDescending(u => u.DATE_ADD.Trim()).ToList();
                    if (SelLetterList.Count > 0)
                    {
                        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                        LetterHist = SelLetterList[0];
                    }
                }
                if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }
                gvApp.Rows.Add(false, "CEAP Benefit fulfillment form", LetDate, LetWorker, "5", "");

                if (LetterList.Count > 0)
                {
                    LetterHist = new LETRHISTCEntity();
                    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "6").OrderByDescending(u => u.DATE_ADD.Trim()).ToList();
                    if (SelLetterList.Count > 0)
                    {
                        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                        LetterHist = SelLetterList[0];
                    }
                }
                if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }
                gvApp.Rows.Add(false, "Client satisfaction survey", LetDate, LetWorker, "6", "");

                if (LetterList.Count > 0)
                {
                    LetterHist = new LETRHISTCEntity();
                    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "7").OrderByDescending(u => u.DATE_ADD.Trim()).ToList();
                    if (SelLetterList.Count > 0)
                    {
                        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                        LetterHist = SelLetterList[0];
                    }
                }
                if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }
                gvApp.Rows.Add(false, "Termination Notification", LetDate, LetWorker, "7", "");

                if (LetterList.Count > 0)
                {
                    LetterHist = new LETRHISTCEntity();
                    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "8").OrderByDescending(u => u.DATE_ADD.Trim()).ToList();
                    if (SelLetterList.Count > 0)
                    {
                        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                        LetterHist = SelLetterList[0];
                    }
                }
                if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }
                gvApp.Rows.Add(false, "CEAP Priority Rating Form", LetDate, LetWorker, "8", "");

                if (LetterList.Count > 0)
                {
                    LetterHist = new LETRHISTCEntity();
                    SelLetterList = LetterList.FindAll(u => u.LETR_CODE == "9").OrderByDescending(u => u.DATE_ADD.Trim()).ToList();
                    if (SelLetterList.Count > 0)
                    {
                        SelLetterList = SelLetterList.OrderByDescending(u => Convert.ToDateTime(u.DATE_ADD.Trim())).ToList();
                        LetterHist = SelLetterList[0];
                    }
                }
                if (LetterHist != null) { LetDate = LetterHist.DATE_ADD.Trim(); LetWorker = CaseworkerName(LetterHist.WORKER.Trim()); } else { LetDate = string.Empty; LetWorker = string.Empty; }
                gvApp.Rows.Add(false, "Declaration of Income Statement", LetDate, LetWorker, "9", "");

                if (rowIndex == 1)
                    gvApp.CurrentCell = gvApp.Rows[0].Cells[0];
                else if (rowIndex == 2)
                    gvApp.CurrentCell = gvApp.Rows[1].Cells[0];
                else if (rowIndex == 3)
                    gvApp.CurrentCell = gvApp.Rows[2].Cells[0];

            }
        }

        private void gvApp_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridView AppGrid = sender as DataGridView;
                string selectedHIE = AppGrid.SelectedRows[0].Cells["AppDet"].Value.ToString();
                bool isSelect = false;
                if (AppGrid.SelectedRows[0].Cells["Check"].Value.ToString().Equals(Consts.YesNoVariants.True, StringComparison.CurrentCultureIgnoreCase))
                {
                    isSelect = true;
                }
                foreach (DataGridViewRow dr in AppGrid.Rows)
                {
                    string rowCode = dr.Cells["AppDet"].Value.ToString();
                    if (!rowCode.Equals(selectedHIE))
                    {
                        dr.Cells["Check"].Value = "false";
                        dr.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        dr.DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void gvApp_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //DataGridView AppGrid = sender as DataGridView;
            //string selectedHIE = AppGrid.SelectedRows[0].Cells["AppDet"].Value.ToString();
            //bool isSelect = false;
            //if (AppGrid.SelectedRows[0].Cells["Check"].Value.ToString().Equals(Consts.YesNoVariants.True, StringComparison.CurrentCultureIgnoreCase))
            //{
            //    isSelect = true;
            //}
            //foreach (DataGridViewRow dr in AppGrid.Rows)
            //{
            //    string rowCode = dr.Cells["AppDet"].Value.ToString();
            //    if (!rowCode.Equals(selectedHIE))
            //    {
            //        dr.Cells["Check"].Value = "false";
            //        dr.DefaultCellStyle.ForeColor = Color.Black;
            //    }
            //    else
            //    {
            //        dr.DefaultCellStyle.ForeColor = Color.Black;
            //    }
            //}

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (BaseForm.BaseAgencyControlDetails.AgyShortName.ToUpper() == "PCS")
            {
                switch (gvApp.CurrentRow.Cells["gvCode"].Value.ToString())
                {
                    case "1":
                        if (CASESPM_SP_List.Count > 0)
                        {
                            CASESPM_SP_List = CASESPM_SP_List.FindAll(u => u.SPM_EligStatus != string.Empty);

                            if (CASESPM_SP_List[0].SPM_EligStatus.Trim() == "P")
                            {
                                CommonFunctions.MessageBoxDisplay("Letter will not be printed for the Pending Status.");
                            }
                            else if (CASESPM_SP_List[0].SPM_EligStatus.Trim() == "E"
                                     || CASESPM_SP_List[0].SPM_EligStatus.Trim() == "M"
                                     || CASESPM_SP_List[0].SPM_EligStatus.Trim() == "N"
                                     || CASESPM_SP_List[0].SPM_EligStatus.Trim() == "S")    // Elig Letter prints only for the Eligible,SSI categorical and Means Tested Statuses - 08/04/2022
                            {                               
                                    On_EligLetterNew();
                                    SavePrintRecord();
                                    FillGrid();                             
                            }
                            else if (CASESPM_SP_List[0].SPM_EligStatus.Trim() == "D")
                            {
                                On_PCS_DeniedLetter();
                                SavePrintRecord();
                                FillGrid();
                            }
                        }
                        else
                        {
                            CommonFunctions.MessageBoxDisplay("Elig Record not existed so Pdf file is not generated.");
                        }

                        break;
                    case "2":
                        if (BaseForm.BaseYear == "2022")
                            On_PCS_PriorityRankingForm2022();
                        else
                            On_PCS_PriorityRankingForm();
                        SavePrintRecord();
                        FillGrid();
                        break;
                }
            }
            else if (BaseForm.BaseAgencyControlDetails.AgyShortName.ToUpper() == "NCCAA")
            {
                switch (gvApp.CurrentRow.Cells["gvCode"].Value.ToString())
                {
                    case "1":
                        if (CASESPM_SP_List.Count > 0)
                        {
                            CASESPM_SP_List = CASESPM_SP_List.FindAll(u => u.SPM_EligStatus != string.Empty);

                            if (CASESPM_SP_List[0].SPM_EligStatus.Trim() == "P")
                            {
                                CommonFunctions.MessageBoxDisplay("Letter will not be printed for the Pending Status.");
                            }
                            else if (CASESPM_SP_List[0].SPM_EligStatus.Trim() == "E"
                                     || CASESPM_SP_List[0].SPM_EligStatus.Trim() == "M"
                                     || CASESPM_SP_List[0].SPM_EligStatus.Trim() == "S")    // Elig Letter prints only for the Eligible,SSI categorical and Means Tested Statuses - 08/04/2022
                            {
                                On_NCCAAEligLetterNew();
                                //On_FORTWORTHEligLetterNew();
                                SavePrintRecord();
                                FillGrid();
                            }
                            else if (CASESPM_SP_List[0].SPM_EligStatus.Trim() == "D")
                            {
                                On_PCS_DeniedLetter();
                                SavePrintRecord();
                                FillGrid();
                            }
                        }
                        else
                        {
                            CommonFunctions.MessageBoxDisplay("Elig Record not existed so Pdf file is not generated.");
                        }

                        break;
                    case "2":
                        On_NCCAA_PriorityRankingForm();
                        SavePrintRecord();
                        FillGrid();
                        break;
                }
            }
            else if (BaseForm.BaseAgencyControlDetails.AgyShortName.ToUpper() == "FTW")
            {
                switch (gvApp.CurrentRow.Cells["gvCode"].Value.ToString())
                {
                    case "1":
                        if (CASESPM_SP_List.Count > 0)
                        {
                            CASESPM_SP_List = CASESPM_SP_List.FindAll(u => u.SPM_EligStatus != string.Empty);

                            if (CASESPM_SP_List[0].SPM_EligStatus.Trim() == "P")
                            {
                                CommonFunctions.MessageBoxDisplay("Letter will not be printed for the Pending Status.");
                            }
                            else if (CASESPM_SP_List[0].SPM_EligStatus.Trim() == "E"
                                     || CASESPM_SP_List[0].SPM_EligStatus.Trim() == "M"
                                     || CASESPM_SP_List[0].SPM_EligStatus.Trim() == "S")    // Elig Letter prints only for the Eligible,SSI categorical and Means Tested Statuses - 08/04/2022
                            {
                                //NCCAA_RFP();
                                On_FORTWORTHEligLetterNew();
                                SavePrintRecord();
                                FillGrid();
                            }
                            else if (CASESPM_SP_List[0].SPM_EligStatus.Trim() == "D")
                            {
                                On_PCS_DeniedLetter();
                                SavePrintRecord();
                                FillGrid();
                            }
                        }
                        else
                        {
                            CommonFunctions.MessageBoxDisplay("Elig Record not existed so Pdf file is not generated.");
                        }

                        break;
                    case "2":
                        On_Fortworth_PriorityRankingForm();
                        SavePrintRecord();
                        FillGrid();
                        break;
                }
            }
            else
            {
                switch (gvApp.CurrentRow.Cells["gvCode"].Value.ToString())
                {

                    case "1": On_Delay_Eligibility(); break;
                    case "2":
                        if (EligStatus == "Eligible")
                        {
                            On_EligLetterNew();
                           
                        }
                        else if (EligStatus == "Denied") On_Denial_Notice();
                        else if (EligStatus == "Pending") CommonFunctions.MessageBoxDisplay("Eligibility Status is Pending so Pdf file is not generated.");
                        break;
                    //case "3": if (EligStatus == "Denied") On_Denial_Notice(); break;
                    case "4": On_Right_Appeal(); break;
                    case "5": if (BaseForm.BaseCaseMstListEntity[0].Language.Trim() == "01") On_Benefit_Fullfilment_English(); else On_Benefit_Fullfilment_Spanish(); break;
                    case "6": if (BaseForm.BaseCaseMstListEntity[0].Language.Trim() == "01") On_Client_Satisfaction_Survey_English(); else On_Client_Satisfaction_Survey_Spanish(); break;
                    case "7": On_Termination_Notice(); break;
                    case "8": On_CEAP_PriorityRankingForm(); break;
                    case "9": On_DIS(); break;
                }
                SavePrintRecord();
                FillGrid();
            }




        }

        private void On_Delay_Eligibility()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            ReaderName = propReportPath + "\\" + "Delay_in_Eligibility_Determination.pdf";



            PdfName = "Delay_in_Eligibility_Determination";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 150; Y_Pos = 692;

                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName.ToLower()), TableFont), X_Pos, Y_Pos, 0);
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 300, Y_Pos, 0);

                X_Pos = 250; Y_Pos = Y_Pos - 25;
                string Phone = string.Empty;
                MaskedTextBox mskPhn = new MaskedTextBox();
                mskPhn.Mask = "(000)000-0000";
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Area.Trim()) || !string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Phone.Trim()))
                {
                    mskPhn.Text = BaseForm.BaseCaseMstListEntity[0].Area.Trim() + BaseForm.BaseCaseMstListEntity[0].Phone.Trim();
                    Phone = mskPhn.Text.Trim();
                }
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Phone, TableFont), X_Pos, Y_Pos, 0);

                if (BaseForm.BaseCaseMstListEntity[0].Language == "01")
                {
                    X_Pos = 280; Y_Pos = 153;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.AddDays(10).ToShortDateString(), TblFontBold), X_Pos, Y_Pos, 0);
                }
                else
                {
                    X_Pos = 430; Y_Pos = 128;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.AddDays(10).ToShortDateString(), TblFontItalic), X_Pos, Y_Pos, 0);
                }

                X_Pos = 60; Y_Pos = 63;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(LookupDataAccess.GetMemberName(BaseForm.UserProfile.FirstName.Trim(), BaseForm.UserProfile.MI.Trim(), BaseForm.UserProfile.LastName.Trim(), "3")), TableFont), X_Pos, Y_Pos, 0);
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 210, Y_Pos, 0);

            }
            catch (Exception ex) { }


            Hstamper.Close();

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

        private void On_Eligibility_Notification()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()=="01")
            ReaderName = propReportPath + "\\" + "Eligibility_Notification.pdf";



            PdfName = "Eligibility_Notification";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 80; Y_Pos = 682;

                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName.ToLower()), TableFont), X_Pos, Y_Pos, 0);

                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, TableFont), X_Pos + 280, Y_Pos, 0);

                string Apt = string.Empty; string Floor = string.Empty; string HN = string.Empty; string Suffix = string.Empty; string Street = string.Empty;
                string Zip = string.Empty;
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Apt.Trim()))
                    Apt = "Apt  " + BaseForm.BaseCaseMstListEntity[0].Apt.Trim() + "   ";
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Flr.Trim()))
                    Floor = "Flr  " + BaseForm.BaseCaseMstListEntity[0].Flr.Trim() + "   ";
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Street.Trim()))
                    Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + " ";
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim()))
                    Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim();
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim()))
                    HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + " ";
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Zip.Trim()) && BaseForm.BaseCaseMstListEntity[0].Zip != "0")
                    Zip = "00000".Substring(0, 5 - BaseForm.BaseCaseMstListEntity[0].Zip.Trim().Length) + BaseForm.BaseCaseMstListEntity[0].Zip.Trim();
                string Comma = string.Empty;
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim()) && (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Apt.Trim()) || !string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Flr.Trim())))
                    Comma = ", ";

                string Address = HN + Street + Suffix + Comma + Apt + Floor + ", " + BaseForm.BaseCaseMstListEntity[0].City.Trim() + ", " + BaseForm.BaseCaseMstListEntity[0].State.Trim() + " " + Zip;

                string VendorName = string.Empty;
                if (CASESPM_SP_List.Count > 0)
                {
                    if (!string.IsNullOrEmpty(CASESPM_SP_List[0].SPM_Vendor.Trim()))
                        VendorName = Get_Vendor_Name(CASESPM_SP_List[0].SPM_Vendor.Trim());
                }


                Y_Pos = Y_Pos - 15;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase((HN + Street + Suffix).ToLower()), TableFont), X_Pos, Y_Pos, 0);

                Y_Pos = Y_Pos - 15;
                if (!string.IsNullOrEmpty(Apt.Trim()) || !string.IsNullOrEmpty(Floor.Trim()))
                {
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase((Apt + Floor).ToLower()), TableFont), X_Pos, Y_Pos, 0);

                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(VendorName, TableFont), X_Pos + 280, Y_Pos, 0);

                    Y_Pos = Y_Pos - 15;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase((BaseForm.BaseCaseMstListEntity[0].City.Trim() + ", " + BaseForm.BaseCaseMstListEntity[0].State.Trim() + " " + Zip).ToLower()), TableFont), X_Pos, Y_Pos, 0);



                }
                else
                {
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase((BaseForm.BaseCaseMstListEntity[0].City.Trim() + ", " + BaseForm.BaseCaseMstListEntity[0].State.Trim() + " " + Zip).ToLower()), TableFont), X_Pos, Y_Pos, 0);
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(VendorName, TableFont), X_Pos + 280, Y_Pos, 0);
                }
                //X_Pos = 250; Y_Pos = Y_Pos - 25;
                //MaskedTextBox mskPhn = new MaskedTextBox();
                //mskPhn.Mask = "(000)000-0000";
                //mskPhn.Text = BaseForm.BaseCaseMstListEntity[0].Area.Trim() + BaseForm.BaseCaseMstListEntity[0].Phone.Trim();
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskPhn.Text, TableFont), X_Pos, Y_Pos, 0);

            }
            catch (Exception ex) { }


            Hstamper.Close();

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

        private void On_Denial_Notice()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()=="01")
            ReaderName = propReportPath + "\\" + "Denial_Notice.pdf";



            PdfName = "Denial_Notice";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            //iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Consts.Icons.ico_Tick);
            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 80; Y_Pos = 682;

                PrintAddress(TableFont, cb);

                string VendorName = string.Empty;
                if (CASESPM_SP_List.Count > 0)
                {
                    if (!string.IsNullOrEmpty(CASESPM_SP_List[0].SPM_Vendor.Trim()))
                        VendorName = Get_Vendor_Name(CASESPM_SP_List[0].SPM_Vendor.Trim());
                }

                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(VendorName, TableFont), X_Pos + 280, Y_Pos, 0);

                //TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName), TableFont), X_Pos, Y_Pos, 0);
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 300, Y_Pos, 0);

                //X_Pos = 250; Y_Pos = Y_Pos - 25;
                //MaskedTextBox mskPhn = new MaskedTextBox();
                //mskPhn.Mask = "(000)000-0000";
                //mskPhn.Text = BaseForm.BaseCaseMstListEntity[0].Area.Trim() + BaseForm.BaseCaseMstListEntity[0].Phone.Trim();
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskPhn.Text, TableFont), X_Pos, Y_Pos, 0);

                X_Pos = 40; Y_Pos = 175;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase((LookupDataAccess.GetMemberName(BaseForm.UserProfile.FirstName.Trim(), BaseForm.UserProfile.MI.Trim(), BaseForm.UserProfile.LastName.Trim(), "3")), TableFont), X_Pos, Y_Pos, 0);
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 445, Y_Pos, 0);

            }
            catch (Exception ex) { }


            Hstamper.Close();

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

        private void On_Right_Appeal()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()=="01")
            ReaderName = propReportPath + "\\" + "Right_to_Appeal.pdf";



            PdfName = "Right_to_Appeal";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 150; Y_Pos = 692;

                //TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName), TableFont), X_Pos, Y_Pos, 0);
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 300, Y_Pos, 0);

                //X_Pos = 250; Y_Pos = Y_Pos - 25;
                //MaskedTextBox mskPhn = new MaskedTextBox();
                //mskPhn.Mask = "(000)000-0000";
                //mskPhn.Text = BaseForm.BaseCaseMstListEntity[0].Area.Trim() + BaseForm.BaseCaseMstListEntity[0].Phone.Trim();
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskPhn.Text, TableFont), X_Pos, Y_Pos, 0);

            }
            catch (Exception ex) { }


            Hstamper.Close();

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

        private void On_Benefit_Fullfilment_English()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()=="01")
            ReaderName = propReportPath + "\\" + "CEAP_Benefit_Fulfillment_Eng.pdf";



            PdfName = "CEAP_Benefit_Fulfillment";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 135; Y_Pos = 552;

                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName.ToLower()), TableFont), X_Pos, Y_Pos, 0);
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 300, Y_Pos, 0);

                X_Pos = 395; Y_Pos = 387;


                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseYear, TableFont), X_Pos, Y_Pos, 0);

                //byte[] data = Hreader.GetPageContent(1);
                //string xyz = PdfEncodings.ConvertToString(data, PdfObject.TEXT_PDFDOCENCODING);
                //string abc = xyz.Replace("2017", BaseForm.BaseYear);
                //Hreader.SetPageContent(1, PdfEncodings.ConvertToBytes(abc, PdfObject.TEXT_PDFDOCENCODING));


                //PdfDictionary dict = Hreader.GetPageN(1);
                //PdfObject obj = dict.GetDirectObject(iTextSharp.text.pdf.PdfName.CONTENTS);

                //PRStream stream = (PRStream)obj;
                //byte[] data = PdfReader.GetStreamBytes(stream);
                //string dd = new string(System.Text.Encoding.UTF8.GetString(data).ToCharArray());
                //dd = dd.Replace("2017", BaseForm.BaseYear);
                //stream.SetData(System.Text.Encoding.UTF8.GetBytes(dd));

                //var form = Hstamper.AcroFields;
                //var fieldKeys = form.Fields.Keys;
                //foreach (string fieldKey in fieldKeys)
                //{
                //    var value = Hreader.AcroFields.GetField(fieldKey);
                //    form.SetField(fieldKey, value.Replace("2017", BaseForm.BaseYear));
                //}

                //// Textfeld unbearbeitbar machen (sieht aus wie normaler text)
                //Hstamper.FormFlattening = true;



                //X_Pos = 250; Y_Pos = Y_Pos - 25;
                //MaskedTextBox mskPhn = new MaskedTextBox();
                //mskPhn.Mask = "(000)000-0000";
                //mskPhn.Text = BaseForm.BaseCaseMstListEntity[0].Area.Trim() + BaseForm.BaseCaseMstListEntity[0].Phone.Trim();
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskPhn.Text, TableFont), X_Pos, Y_Pos, 0);

                Y_Pos = 130; X_Pos = 55;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(LookupDataAccess.GetMemberName(BaseForm.UserProfile.FirstName.Trim(), BaseForm.UserProfile.MI.Trim(), BaseForm.UserProfile.LastName.Trim(), "3")), TableFont), X_Pos, Y_Pos, 0);
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 320, Y_Pos, 0);

            }
            catch (Exception ex) { }


            Hstamper.Close();

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

        private void On_Benefit_Fullfilment_Spanish()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()=="01")
            ReaderName = propReportPath + "\\" + "CEAP_Benefit_Fulfillment_Spa.pdf";



            PdfName = "CEAP_Benefit_Fulfillment";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 175; Y_Pos = 552;

                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName.ToLower()), TableFont), X_Pos, Y_Pos, 0);
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 300, Y_Pos, 0);

                //X_Pos = 250; Y_Pos = Y_Pos - 25;
                //MaskedTextBox mskPhn = new MaskedTextBox();
                //mskPhn.Mask = "(000)000-0000";
                //mskPhn.Text = BaseForm.BaseCaseMstListEntity[0].Area.Trim() + BaseForm.BaseCaseMstListEntity[0].Phone.Trim();
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskPhn.Text, TableFont), X_Pos, Y_Pos, 0);

                Y_Pos = 130; X_Pos = 55;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(LookupDataAccess.GetMemberName(BaseForm.UserProfile.FirstName.Trim(), BaseForm.UserProfile.MI.Trim(), BaseForm.UserProfile.LastName.Trim(), "3")), TableFont), X_Pos, Y_Pos, 0);
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 320, Y_Pos, 0);

            }
            catch (Exception ex) { }


            Hstamper.Close();

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

        private void On_Client_Satisfaction_Survey_English()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()=="01")
            ReaderName = propReportPath + "\\" + "Client_Satisfaction_Survey_Eng.pdf";



            PdfName = "Client_Satisfaction_Survey";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 510; Y_Pos = 57;

                int pageCount = Hreader.NumberOfPages;
                for (int i = 1; i <= pageCount; i++)
                {
                    if (i == 2)
                    {
                        cb = Hstamper.GetOverContent(i);
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, TableFont), X_Pos, Y_Pos, 0);
                    }
                }

                ////TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                //    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName), TableFont), X_Pos, Y_Pos, 0);
                //    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 300, Y_Pos, 0);

                //    //X_Pos = 250; Y_Pos = Y_Pos - 25;
                //    //MaskedTextBox mskPhn = new MaskedTextBox();
                //    //mskPhn.Mask = "(000)000-0000";
                //    //mskPhn.Text = BaseForm.BaseCaseMstListEntity[0].Area.Trim() + BaseForm.BaseCaseMstListEntity[0].Phone.Trim();
                //    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskPhn.Text, TableFont), X_Pos, Y_Pos, 0);

            }
            catch (Exception ex) { }


            Hstamper.Close();

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

        private void On_Client_Satisfaction_Survey_Spanish()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()=="01")
            ReaderName = propReportPath + "\\" + "Client_Satisfaction_Survey_Spa.pdf";



            PdfName = "Client_Satisfaction_Survey";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 500; Y_Pos = 53;

                int pageCount = Hreader.NumberOfPages;
                for (int i = 1; i <= pageCount; i++)
                {
                    if (i == 2)
                    {
                        cb = Hstamper.GetOverContent(i);
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, TableFont), X_Pos, Y_Pos, 0);
                    }
                }

            }
            catch (Exception ex) { }


            Hstamper.Close();

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

        private void On_Termination_Notice()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()=="01")
            ReaderName = propReportPath + "\\" + "Termination_Notification.pdf";



            PdfName = "Termination_Notification";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 150; Y_Pos = 692;

                PrintAddress(TableFont, cb);
                string VendorName = string.Empty;
                if (CASESPM_SP_List.Count > 0)
                {
                    if (!string.IsNullOrEmpty(CASESPM_SP_List[0].SPM_Vendor.Trim()))
                        VendorName = Get_Vendor_Name(CASESPM_SP_List[0].SPM_Vendor.Trim());
                }

                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(VendorName, TableFont), X_Pos + 280, Y_Pos, 0);

                //TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName), TableFont), X_Pos, Y_Pos, 0);
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 300, Y_Pos, 0);

                //X_Pos = 250; Y_Pos = Y_Pos - 25;
                //MaskedTextBox mskPhn = new MaskedTextBox();
                //mskPhn.Mask = "(000)000-0000";
                //mskPhn.Text = BaseForm.BaseCaseMstListEntity[0].Area.Trim() + BaseForm.BaseCaseMstListEntity[0].Phone.Trim();
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskPhn.Text, TableFont), X_Pos, Y_Pos, 0);

                X_Pos = 40; Y_Pos = 218;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase((LookupDataAccess.GetMemberName(BaseForm.UserProfile.FirstName.Trim(), BaseForm.UserProfile.MI.Trim(), BaseForm.UserProfile.LastName.Trim(), "3")), TableFont), X_Pos, Y_Pos, 0);
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 445, Y_Pos, 0);

            }
            catch (Exception ex) { }


            Hstamper.Close();

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

        private void On_CEAP_PriorityRankingForm()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            ReaderName = propReportPath + "\\" + "CEAP_Priority_Rating.pdf";



            PdfName = "CEAP_Priority_Rating";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);
            //_image_Checked.ScalePercent(60f);

            List<CustomQuestionsEntity> custResponses = new List<CustomQuestionsEntity>();
            custResponses = _model.CaseMstData.CAPS_CASEUSAGE_GET(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty);

            ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            int intfromage = 0; int inttoage = 0;
            if (programEntity != null)
            {
                intfromage = Convert.ToInt16(programEntity.DepSENFromAge == string.Empty ? "0" : programEntity.DepSENFromAge);
                inttoage = Convert.ToInt16(programEntity.DepSENToAge == string.Empty ? "0" : programEntity.DepSENToAge);
            }
            double doublesertotal = 0;
            CustomQuestionsEntity responsetot = custResponses.Find(u => u.USAGE_MONTH.Equals("TOT"));
            if (responsetot != null)
            {
                doublesertotal = Convert.ToDouble(responsetot.USAGE_TOTAL == string.Empty ? "0" : responsetot.USAGE_TOTAL);
            }

            double doubleTotalAmount = Convert.ToDouble(BaseForm.BaseCaseMstListEntity[0].ProgIncome == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].ProgIncome);
            double totaldive = (doublesertotal / doubleTotalAmount) * 100;
            totaldive = Math.Round(totaldive, 2);
            try
            {
                X_Pos = 30; Y_Pos = 760;

                X_Pos = 150; Y_Pos -= 90;

                int inttotalcount = 0;


                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);


                X_Pos = 500;


                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].ApplNo, TblFontBold), X_Pos, Y_Pos, 0);

                List<CaseSnpEntity> casesnpEligbulity = BaseForm.BaseCaseSnpEntity.FindAll(u => u.DobNa.Equals("0") && u.Status == "A");
                List<CaseSnpEntity> casesnpElder = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) >= Convert.ToDecimal(intfromage)) && (Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) <= Convert.ToDecimal(inttoage))));
                int inteldercount = 0;
                if (casesnpElder.Count > 0)
                {
                    inteldercount = 4;
                }
                inttotalcount = inttotalcount + inteldercount;

                List<CaseSnpEntity> casesnpyounger = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) <= Convert.ToDecimal(5))));
                int intyoungercount = 0;
                if(casesnpyounger.Count>0)
                {
                    intyoungercount = 4;
                }
                inttotalcount = inttotalcount + intyoungercount;

                List<CaseSnpEntity> casesnpdisable = casesnpEligbulity.FindAll(u => u.Disable.ToString().ToUpper() == "Y" && u.Status == "A");
                int intdisablecount = 0;
                if(casesnpdisable.Count>0)
                {
                    intdisablecount = 4;
                }
                inttotalcount = inttotalcount + intdisablecount;

                int intNoneabove = 0;
                if (inttotalcount == 0)
                {
                    inttotalcount = inttotalcount + intNoneabove;
                    intNoneabove = 1;
                }
                int intfity = 0; int intsenvtyfive = 0; int inttwentyfive = 0; int inttwentytofifty = 0; int intfiftyone = 0;
                int intmstpoverty = Convert.ToInt32(BaseForm.BaseCaseMstListEntity[0].Poverty == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].Poverty);

                if (intmstpoverty <= 50)
                {
                    inttotalcount = inttotalcount + 10;
                    intfity = 10;
                }
                else if (intmstpoverty >= 51 && intmstpoverty <= 75)
                {
                    inttotalcount = inttotalcount + 7;
                    intsenvtyfive = 7;
                }
                else if (intmstpoverty >= 76 && intmstpoverty <= 125)
                {
                    inttotalcount = inttotalcount + 3;
                    inttwentyfive = 3;
                }
                else if (intmstpoverty >= 126 && intmstpoverty <= 150)
                {
                    inttotalcount = inttotalcount + 1;
                    inttwentytofifty = 1;
                }
                else if (intmstpoverty <= 151)
                {

                    intfiftyone = 0;
                }

                int intExceedYes = 0; int intExceedNo = 0;
                if (doublesertotal > 1000)
                {
                    inttotalcount = inttotalcount + 4;
                    intExceedYes = 4;
                }
                else
                {
                    inttotalcount = inttotalcount + 1;
                    intExceedNo = 1;
                }


                int intthirty = 0; int inttwenty = 0; int inteleven = 0; int intsix = 0; int intfive = 0;
                if (doubleTotalAmount == 0 || doublesertotal == 0)
                {
                    if (doubleTotalAmount == 0)
                    {
                        inttotalcount = inttotalcount + 12;
                        intthirty = 12;
                    }
                    else
                        intfive = 0;
                }
                else
                {

                    if (totaldive >= 30)
                    {
                        inttotalcount = inttotalcount + 12;
                        intthirty = 12;
                    }
                    else if (totaldive >= 20 && totaldive <= 29.99)
                    {
                        inttotalcount = inttotalcount + 9;
                        inttwenty = 9;
                    }
                    else if (totaldive >= 11 && totaldive <= 19.99)
                    {
                        inttotalcount = inttotalcount + 6;
                        inteleven = 6;
                    }
                    else if (totaldive >= 6 && totaldive <= 10.99)
                    {
                        inttotalcount = inttotalcount + 3;
                        intsix = 3;
                    }
                    else if (totaldive <= 5.99)
                    {
                        if (doubleTotalAmount == 0 || doublesertotal == 0)
                        {
                            intfive = 0;
                        }
                        else
                        {
                            inttotalcount = inttotalcount + 1;
                            intfive = 1;
                        }
                    }
                }

                X_Pos = 535;
                Y_Pos -= 45;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inteldercount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intdisablecount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intyoungercount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intNoneabove.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 42;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfity.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsenvtyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwentyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwentytofifty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfiftyone.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                X_Pos = 65; Y_Pos -= 45;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(doublesertotal.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                X_Pos = 200;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(doubleTotalAmount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                X_Pos = 400;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(totaldive.ToString().ToUpper() == "NAN" ? string.Empty : totaldive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                X_Pos = 535;
                Y_Pos -= 37;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intthirty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwenty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inteleven.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsix.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                Y_Pos -= 40;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intExceedYes.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intExceedNo.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 30;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttotalcount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                if (inttotalcount >= 17)
                {
                    X_Pos = 40;
                    Y_Pos -= 30;
                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }
                else if (inttotalcount >= 11 && inttotalcount <= 16)
                {
                    X_Pos = 40;
                    Y_Pos -= 66;
                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }
                else if (inttotalcount <= 10)
                {
                    X_Pos = 40;
                    Y_Pos -= 93;
                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }


                StringBuilder strMstAppl = new StringBuilder();
                strMstAppl.Append("<Applicants>");
                strMstAppl.Append("<Details MSTApplDetails = \"" + BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + (BaseForm.BaseYear.Trim() == string.Empty ? "    " : BaseForm.BaseYear.Trim()) + BaseForm.BaseApplicationNo + "\" MST_RANK1 = \"" + inttotalcount.ToString() + "\" MST_RANK2 = \"" + "0" + "\" MST_RANK3 = \"" + "0" + "\" MST_RANK4 = \"" + "0" + "\" MST_RANK5 = \"" + "0" + "\" MST_RANK6 = \"" + "0" + "\"   />");
                strMstAppl.Append("</Applicants>");

                if (_model.CaseMstData.UpdateCaseMstRanks(strMstAppl.ToString(), "Single"))
                {
                    BaseForm.BaseCaseMstListEntity[0].Rank1 = inttotalcount.ToString();
                }



            }
            catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

            Hstamper.Close();

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

        private void On_DIS()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()=="01")
            ReaderName = propReportPath + "\\" + "DIS.pdf";



            PdfName = "Declaration_of_Income";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 150; Y_Pos = 692;

                //TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName), TableFont), X_Pos, Y_Pos, 0);
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 300, Y_Pos, 0);

                //X_Pos = 250; Y_Pos = Y_Pos - 25;
                //MaskedTextBox mskPhn = new MaskedTextBox();
                //mskPhn.Mask = "(000)000-0000";
                //mskPhn.Text = BaseForm.BaseCaseMstListEntity[0].Area.Trim() + BaseForm.BaseCaseMstListEntity[0].Phone.Trim();
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskPhn.Text, TableFont), X_Pos, Y_Pos, 0);

            }
            catch (Exception ex) { }


            Hstamper.Close();

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

        private void PrintAddress(iTextSharp.text.Font TableFont, PdfContentByte cb)
        {
            X_Pos = 80; Y_Pos = 682;

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName.ToLower()), TableFont), X_Pos, Y_Pos, 0);

            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, TableFont), X_Pos + 280, Y_Pos, 0);

            string Apt = string.Empty; string Floor = string.Empty; string HN = string.Empty; string Suffix = string.Empty; string Street = string.Empty;
            string Zip = string.Empty;
            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Apt.Trim()))
                Apt = "Apt  " + BaseForm.BaseCaseMstListEntity[0].Apt.Trim() + "   ";
            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Flr.Trim()))
                Floor = "Flr  " + BaseForm.BaseCaseMstListEntity[0].Flr.Trim() + "   ";
            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Street.Trim()))
                Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + " ";
            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim()))
                Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim();
            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim()))
                HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + " ";
            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Zip.Trim()) && BaseForm.BaseCaseMstListEntity[0].Zip != "0")
                Zip = "00000".Substring(0, 5 - BaseForm.BaseCaseMstListEntity[0].Zip.Trim().Length) + BaseForm.BaseCaseMstListEntity[0].Zip.Trim();
            string Comma = string.Empty;
            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim()) && (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Apt.Trim()) || !string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Flr.Trim())))
                Comma = ", ";

            string Address = HN + Street + Suffix + Comma + Apt + Floor + ", " + BaseForm.BaseCaseMstListEntity[0].City.Trim() + ", " + BaseForm.BaseCaseMstListEntity[0].State.Trim() + " " + Zip;


            Y_Pos = Y_Pos - 15;
            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase((HN + Street + Suffix).ToLower()), TableFont), X_Pos, Y_Pos, 0);

            Y_Pos = Y_Pos - 15;
            if (!string.IsNullOrEmpty(Apt.Trim()) || !string.IsNullOrEmpty(Floor.Trim()))
            {
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase((Apt + Floor).ToLower()), TableFont), X_Pos, Y_Pos, 0);

                Y_Pos = Y_Pos - 15;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase((BaseForm.BaseCaseMstListEntity[0].City.Trim() + ", " + BaseForm.BaseCaseMstListEntity[0].State.Trim() + " " + Zip).ToLower()), TableFont), X_Pos, Y_Pos, 0);
            }
            else
            {
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase((BaseForm.BaseCaseMstListEntity[0].City.Trim() + ", " + BaseForm.BaseCaseMstListEntity[0].State.Trim() + " " + Zip).ToLower()), TableFont), X_Pos, Y_Pos, 0);
            }
        }

        private void SavePrintRecord()
        {
            LETRHISTCEntity Entity = new LETRHISTCEntity();
            string Msg = string.Empty;
            Entity.AGENCY = BaseForm.BaseAgency;
            Entity.DEPT = BaseForm.BaseDept;
            Entity.PROGRAM = BaseForm.BaseProg;
            Entity.YEAR = BaseForm.BaseYear;
            Entity.APPNO = BaseForm.BaseApplicationNo;
            Entity.LETR_CODE = gvApp.CurrentRow.Cells["gvCode"].Value.ToString();
            Entity.DATE = DateTime.Now.ToShortDateString();
            Entity.SEQ = "1";

            if (CaseWorker != null)
                Entity.WORKER = CaseWorker.CaseWorker.Trim();
            Entity.ADD_OPERATOR = BaseForm.UserID;

            _model.SPAdminData.InsertLETRHIST(Entity, out Msg);
        }

        PdfContentByte cb;
        int X_Pos, Y_Pos;
        string strFolderPath = string.Empty;
        string Random_Filename = null; string PdfName = "Pdf File";


        private void On_Delete_PDF_File(object sender, FormClosedEventArgs e)
        {
            System.IO.File.Delete(PdfName);
        }

        private void SetLine()
        {
            cb.SetLineWidth(2f);
            cb.SetLineCap(5);
            cb.MoveTo(X_Pos, Y_Pos);
            cb.LineTo(780, Y_Pos);
            cb.Stroke();
        }

        DataTable dtIncome = new DataTable();
        private string Get_IncomeType_Desc(string Type_Code)
        {
            string Income_Desc = string.Empty;
            foreach (DataRow drIncome in dtIncome.Rows)
            {
                if (Type_Code == drIncome["Code"].ToString().Trim())
                {
                    Income_Desc = drIncome["LookUpDesc"].ToString().Trim(); break;
                }
            }

            return Income_Desc;
        }

        DataTable dtCaseSNP = new DataTable();
        private string Get_Member_Name(string Mem_Seq, string NameFormat)
        {
            string Member_NAme = string.Empty;
            foreach (DataRow drCaseSnp in dtCaseSNP.Rows)
            {
                if (Mem_Seq == drCaseSnp["SNP_FAMILY_SEQ"].ToString().Trim())
                {
                    if (NameFormat == "First")
                    {
                        Member_NAme = drCaseSnp["SNP_NAME_IX_FI"].ToString().Trim(); break;
                    }
                    else
                        Member_NAme = LookupDataAccess.GetMemberName(drCaseSnp["SNP_NAME_IX_FI"].ToString().Trim(), drCaseSnp["SNP_NAME_IX_MI"].ToString().Trim(), drCaseSnp["SNP_NAME_IX_LAST"].ToString().Trim(), strNameFormat) + "  "; break;
                }
            }

            return Member_NAme;
        }



        int pageNumber = 1;
        private void CheckBottomBorderReached(Document document, PdfWriter writer)
        {
            if (Y_Pos <= 20)
            {
                cb.EndText();
                //cb.BeginText();
                //Y_Pos = 07;
                //X_Pos = 20;
                //cb.SetFontAndSize(FontFactory.GetFont(FontFactory.TIMES).BaseFont, 12);
                //cb.SetCMYKColorFill(0, 0, 0, 255);
                ////PrintRec(DateTime.Now.ToLocalTime().ToString(), 130);
                //Y_Pos = 07;
                //X_Pos = 550;
                //PrintRec("Page:", 28);
                //PrintRec(pageNumber.ToString(), 15);
                //cb.EndText();

                document.NewPage();
                pageNumber = writer.PageNumber - 1;

                //cb.BeginText();

                //X_Pos = 50;
                //Y_Pos -= 5;

                //cb.EndText();

                Y_Pos = 770;
                X_Pos = 40;                                                           //modified

                cb.BeginText();

            }
        }

        private void CheckBottomBorderReachedLetterHead(PdfStamper Hstamper)
        {
            if (Y_Pos <= 20)
            {
                cb.EndText();
                //cb.BeginText();
                //Y_Pos = 07;
                //X_Pos = 20;
                //cb.SetFontAndSize(FontFactory.GetFont(FontFactory.TIMES).BaseFont, 12);
                //cb.SetCMYKColorFill(0, 0, 0, 255);
                ////PrintRec(DateTime.Now.ToLocalTime().ToString(), 130);
                //Y_Pos = 07;
                //X_Pos = 550;
                //PrintRec("Page:", 28);
                //PrintRec(pageNumber.ToString(), 15);
                //cb.EndText();
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                ////document.SetPageSize(new iTextSharp.text.Rectangle(iTextSharp.text.PageSize.A4.Width, iTextSharp.text.PageSize.A4.Height));

                document.NewPage();
                //pageNumber = Hstamper.PageNumber - 1;

                //cb.BeginText();

                //X_Pos = 50;
                //Y_Pos -= 5;

                //cb.EndText();

                Y_Pos = 770;
                X_Pos = 90;                                                           //modified

                cb.BeginText();

            }
        }



        private void PrintSpaceCell(PdfPTable table, int Spacesnum, iTextSharp.text.Font TableFont, float Height)
        {
            if (Spacesnum == 1)
            {
                PdfPCell S2 = new PdfPCell(new Phrase("", TableFont));
                S2.HorizontalAlignment = Element.ALIGN_LEFT;
                if (Height > 0)
                    S2.FixedHeight = Height;
                S2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.AddCell(S2);
            }
            else if (Spacesnum == 2)
            {
                PdfPCell S2 = new PdfPCell(new Phrase("", TableFont));
                S2.HorizontalAlignment = Element.ALIGN_LEFT;
                S2.Colspan = 2;
                if (Height > 0)
                    S2.FixedHeight = Height;
                S2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.AddCell(S2);
            }
            else if (Spacesnum == 3)
            {
                PdfPCell S2 = new PdfPCell(new Phrase("", TableFont));
                S2.HorizontalAlignment = Element.ALIGN_LEFT;
                S2.Colspan = 3;
                if (Height > 0)
                    S2.FixedHeight = Height;
                S2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.AddCell(S2);
            }
            else if (Spacesnum == 4)
            {
                PdfPCell S2 = new PdfPCell(new Phrase("", TableFont));
                S2.HorizontalAlignment = Element.ALIGN_LEFT;
                S2.Colspan = 4;
                if (Height > 0)
                    S2.FixedHeight = Height;
                S2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.AddCell(S2);
            }
            else if (Spacesnum == 6)
            {
                PdfPCell S2 = new PdfPCell(new Phrase("", TableFont));
                S2.HorizontalAlignment = Element.ALIGN_LEFT;
                S2.Colspan = 6;
                if (Height > 0)
                    S2.FixedHeight = Height;
                S2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.AddCell(S2);
            }
            else if (Spacesnum == 7)
            {
                PdfPCell S2 = new PdfPCell(new Phrase("", TableFont));
                S2.HorizontalAlignment = Element.ALIGN_LEFT;
                S2.Colspan = 7;
                if (Height > 0)
                    S2.FixedHeight = Height;
                S2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.AddCell(S2);
            }
            else if (Spacesnum == 10)
            {
                PdfPCell S2 = new PdfPCell(new Phrase("", TableFont));
                S2.HorizontalAlignment = Element.ALIGN_LEFT;
                S2.Colspan = 10;
                if (Height > 0)
                    S2.FixedHeight = Height;
                S2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.AddCell(S2);
            }
            else if (Spacesnum == 15)
            {
                PdfPCell S2 = new PdfPCell(new Phrase("", TableFont));
                S2.HorizontalAlignment = Element.ALIGN_LEFT;
                S2.Colspan = 15;
                if (Height > 0)
                    S2.FixedHeight = Height;
                S2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.AddCell(S2);
            }
            else if (Spacesnum == 12)
            {
                PdfPCell S2 = new PdfPCell(new Phrase("", TableFont));
                S2.HorizontalAlignment = Element.ALIGN_LEFT;
                S2.Colspan = 12;
                if (Height > 0)
                    S2.FixedHeight = Height;
                S2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.AddCell(S2);
            }
        }

        public string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        public string HumanisedDate(string date1)
        {
            DateTime date = Convert.ToDateTime(date1.Trim());

            string ordinal;

            switch (date.Day)
            {
                case 1:
                case 21:
                case 31:
                    ordinal = "st";
                    break;
                case 2:
                case 22:
                    ordinal = "nd";
                    break;
                case 3:
                case 23:
                    ordinal = "rd";
                    break;
                default:
                    ordinal = "th";
                    break;
            }

            return string.Format("{0:MMMM dd}{1} ", date, ordinal);
        }

        private string SetLeadingZeros(string TmpSeq)
        {
            int Seq_len = TmpSeq.Trim().Length;
            string TmpCode = null;
            TmpCode = TmpSeq.ToString().Trim();
            switch (Seq_len)
            {
                case 8: TmpCode = "0" + TmpCode; break;
                case 7: TmpCode = "00" + TmpCode; break;
                case 6: TmpCode = "000" + TmpCode; break;
                case 5: TmpCode = "0000" + TmpCode; break;
                case 4: TmpCode = "00000" + TmpCode; break;
                case 3: TmpCode = "000000" + TmpCode; break;
                case 2: TmpCode = "0000000" + TmpCode; break;
                case 1: TmpCode = "00000000" + TmpCode; break;
                    //default: MessageBox.Show("Table Code should not be blank", "CAP Systems", MessageBoxButtons.OK);  TxtCode.Focus();
                    //    break;
            }
            return (TmpCode);
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            LetterHistory letterHist = new LetterHistory(BaseForm, Privileges, "CASE0016", string.Empty);
            letterHist.StartPosition = FormStartPosition.CenterScreen;
            letterHist.ShowDialog();
        }

        public string GetIncomeIntervalDesc(string Interval)
        {
            string Desc = string.Empty;

            if (IncomeInterValList.Count > 0)
            {
                CommonEntity IncInterval = IncomeInterValList.Find(u => u.Code.Trim().Equals(Interval.Trim()));

                if (IncInterval != null) Desc = IncInterval.Desc.Trim();

            }

            return Desc;
        }


        private void On_Shrinkpage()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()=="01")
            ReaderName = propReportPath + "\\" + "Client_Satisfaction_Survey_Eng.pdf";



            PdfName = "Client_Satisfaction_Survey";//form.GetFileName();
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

            //PdfReader Hreader = new PdfReader(ReaderName);

            //PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            //Hstamper.Writer.SetPageSize(PageSize.A5);
            //PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);

            try
            {
                X_Pos = 500; Y_Pos = 60;

                AddwhiteSpaceinPage(ReaderName, PdfName, TableFont);


                //int page = 1;
                //float factor = .9f;
                //shrink(Hstamper, page, factor);

                ////Hreader.SetPageContent(1, Hreader.GetPageContent(1), PdfStream.BEST_COMPRESSION);

                //iTextSharp.text.Rectangle box = Hreader.GetCropBox(page);
                //box.Top = box.Top - factor * box.Height;

                //PdfContentByte cb = Hstamper.GetOverContent(page);
                //cb.SetColorFill(BaseColor.YELLOW);
                //cb.SetColorStroke(BaseColor.RED);
                //cb.Rectangle(box.Left, box.Bottom, box.Width, box.Height);
                //cb.FillStroke();
                //cb.SetColorFill(BaseColor.BLACK);

                //ColumnText ct = new ColumnText(cb);

                //ct.AddElement(new Paragraph("This is some text added to the front page of the front page of this document."));

                //ct.SetSimpleColumn(box.Left, box.Bottom, box.Width, box.Height);
                //ct.Go();



                //int pageCount = Hreader.NumberOfPages;
                //for (int i = 1; i <= pageCount; i++)
                //{
                //    if(i==2)
                //    {
                //        cb = Hstamper.GetOverContent(i);
                //        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, TableFont), X_Pos, Y_Pos, 0);
                //    }
                //}

                ////TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                //    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(ti.ToTitleCase(BaseForm.BaseApplicationName), TableFont), X_Pos, Y_Pos, 0);
                //    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString(), TableFont), X_Pos + 300, Y_Pos, 0);

                //    //X_Pos = 250; Y_Pos = Y_Pos - 25;
                //    //MaskedTextBox mskPhn = new MaskedTextBox();
                //    //mskPhn.Mask = "(000)000-0000";
                //    //mskPhn.Text = BaseForm.BaseCaseMstListEntity[0].Area.Trim() + BaseForm.BaseCaseMstListEntity[0].Phone.Trim();
                //    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskPhn.Text, TableFont), X_Pos, Y_Pos, 0);

            }
            catch (Exception ex) { }


            //Hstamper.Close();

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


        public void shrink(PdfStamper stamper, int page, float factor)
        {
            iTextSharp.text.Rectangle crop = stamper.Reader.GetCropBox(page);
            float diffX = crop.Right * (1 - factor);
            float diffY = crop.Top * (1 - factor);
            PdfDictionary pageN = stamper.Reader.GetPageN(page);
            //stamper.MarkUsed(pageN);
            PdfArray ar = null;
            //PdfObject content = PdfReader.GetPdfObject(pageN.Get(PdfName.CONTENTS), pageN);
            //if (content == null)
            //    return;
            //if (content.IsArray())
            //{
            //    ar = new PdfArray((PdfArray)content);
            //    pageN.Put(PdfName.CONTENTS, ar);
            //}
            //else if (content.IsStream())
            //{
            //    ar = new PdfArray();
            //    ar.Add(pageN.Get(PdfName.CONTENTS));
            //    pageN.Put(PdfName.CONTENTS, ar);
            //}
            //else
            //    return;
            ByteBuffer out_p = new ByteBuffer();
            out_p.Append(factor).Append(" 0 0 ").Append(factor).Append(' ').Append(diffX).Append(' ').Append(diffY).Append(" cm ");
            PdfStream stream = new PdfStream(out_p.ToByteArray());
            ar.AddFirst(stamper.Writer.AddToBody(stream).IndirectReference);
            out_p.Reset();
        }

        private void AddwhiteSpaceinPage(string inputFile, string outputFile, iTextSharp.text.Font TableFont)
        {
            try
            {
                var inputPdf = new PdfReader(inputFile);   // Get input document

                int pageCount = inputPdf.NumberOfPages;

                //if (end < start || end > pageCount)
                //    end = pageCount;

                var inputDoc = new Document(inputPdf.GetPageSize(1)); //GetPageSizeWithRotation

                using (var fs = new FileStream(outputFile, FileMode.Create))
                {
                    var outputWriter = PdfWriter.GetInstance(inputDoc, fs);
                    inputDoc.Open();

                    PdfContentByte cb1 = outputWriter.DirectContent;

                    // Copy pages from input to output document
                    for (int i = 1; i <= pageCount; i++)
                    {
                        var existingRec = inputPdf.GetPageSize(i);
                        var newRec = new iTextSharp.text.Rectangle(0.0f, 0.0f, existingRec.Width, existingRec.Height);
                        if (i == 1)
                            newRec = new iTextSharp.text.Rectangle(0.0f, 0.0f, existingRec.Width, existingRec.Height + 100);

                        inputDoc.SetPageSize(newRec);
                        inputDoc.NewPage();

                        PdfImportedPage page = outputWriter.GetImportedPage(inputPdf, i);
                        int rotation = inputPdf.GetPageRotation(i);

                        if (rotation == 90 || rotation == 270)
                        {
                            cb1.AddTemplate(page, 0, -1f, 1f, 0, 0, inputPdf.GetPageSize(i).Height);
                        }
                        else
                        {
                            cb1.AddTemplate(page, 1f, 0, 0, 1f, 0, 13);
                        }

                        ColumnText.ShowTextAligned(cb1, PdfContentByte.ALIGN_LEFT, new Phrase("Sudheer", TableFont), 50, 870, 0);
                        ColumnText.ShowTextAligned(cb1, PdfContentByte.ALIGN_LEFT, new Phrase("Sudheer", TableFont), 50, 855, 0);
                    }

                    inputDoc.Close();
                }
            }
            catch
            {
            }
        }

        #region PCS Priority Sheet Form

        private void On_PCS_PriorityRankingForm2022()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            ReaderName = propReportPath + "\\" + "PCS_Priority_Ranking_2022.pdf";



            PdfName = "PCS_Priority_Sheet";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);
            //_image_Checked.ScalePercent(60f);

            List<CustomQuestionsEntity> custResponses = new List<CustomQuestionsEntity>();
            custResponses = _model.CaseMstData.CAPS_CASEUSAGE_GET(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty);

            ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            int intfromage = 0; int inttoage = 0;
            if (programEntity != null)
            {
                intfromage = Convert.ToInt16(programEntity.DepSENFromAge == string.Empty ? "0" : programEntity.DepSENFromAge);
                inttoage = Convert.ToInt16(programEntity.DepSENToAge == string.Empty ? "0" : programEntity.DepSENToAge);
            }
            double doublesertotal = 0;
            CustomQuestionsEntity responsetot = custResponses.Find(u => u.USAGE_MONTH.Equals("TOT"));
            if (responsetot != null)
            {
                doublesertotal = Convert.ToDouble(responsetot.USAGE_TOTAL == string.Empty ? "0" : responsetot.USAGE_TOTAL);
            }

            double doubleTotalAmount = Convert.ToDouble(BaseForm.BaseCaseMstListEntity[0].ProgIncome == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].ProgIncome);
            double totaldive = (doublesertotal / doubleTotalAmount) * 100;
            totaldive = Math.Round(totaldive, 2);
            try
            {
                X_Pos = 30; Y_Pos = 760;

                X_Pos = 100; Y_Pos -= 140;

                int inttotalcount = 0;


                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, TblFontBold), X_Pos, Y_Pos, 0);


                X_Pos = 280;


                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);

                List<CaseSnpEntity> casesnpEligbulity = BaseForm.BaseCaseSnpEntity.FindAll(u => u.DobNa.Equals("0") && u.Status == "A");
                List<CaseSnpEntity> casesnpElder = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) >= Convert.ToDecimal(intfromage)) && (Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) <= Convert.ToDecimal(inttoage))));
                int inteldercount = 0;//casesnpElder.Count * 3;
                if (casesnpElder.Count > 0) inteldercount = 3;
                inttotalcount = inttotalcount + inteldercount;

                List<CaseSnpEntity> casesnpyounger = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) <= Convert.ToDecimal(5))));
                int intyoungercount = 0; //casesnpyounger.Count * 3;
                if (casesnpyounger.Count > 0) intyoungercount = 3;
                inttotalcount = inttotalcount + intyoungercount;

                List<CaseSnpEntity> casesnpdisable = casesnpEligbulity.FindAll(u => u.Disable.ToString().ToUpper() == "Y" && u.Status == "A");
                int intdisablecount = 0; //casesnpdisable.Count * 3;
                if (casesnpdisable.Count > 0) intdisablecount = 3;
                inttotalcount = inttotalcount + intdisablecount;

                int intNoneabove = 0;
                if (inttotalcount == 0)
                {
                    inttotalcount = inttotalcount + intNoneabove;
                    intNoneabove = 1;
                }
                int intfity = 0; int intsenvtyfive = 0; int intonefiftyfive = 0;
                int intmstpoverty = Convert.ToInt32(BaseForm.BaseCaseMstListEntity[0].Poverty == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].Poverty);

                if (intmstpoverty <= 50)
                {
                    inttotalcount = inttotalcount + 5;
                    intfity = 5;
                }
                else if (intmstpoverty >= 51 && intmstpoverty <= 75)
                {
                    inttotalcount = inttotalcount + 4;
                    intsenvtyfive = 4;
                }
                else if (intmstpoverty >= 76 && intmstpoverty <= 150)
                {
                    inttotalcount = inttotalcount + 3;
                    intonefiftyfive = 3;
                }
                //else if (intmstpoverty >= 126 && intmstpoverty <= 150)
                //{
                //    inttotalcount = inttotalcount + 1;
                //    inttwentytofifty = 1;
                //}
                //else if (intmstpoverty <= 151)
                //{

                //    intfiftyone = 0;
                //}

                int int1000plus = 0; int int500above = 0; int int250above = 0; int int250below = 0;
                if (doublesertotal > 1000)
                {
                    inttotalcount = inttotalcount + 5;
                    int1000plus = 5;
                }
                else if (doublesertotal >= 500 && doublesertotal <= 999.99)
                {
                    inttotalcount = inttotalcount + 4;
                    int500above = 4;
                }
                else if (doublesertotal >= 250 && doublesertotal <= 499.99)
                {
                    inttotalcount = inttotalcount + 3;
                    int250above = 3;
                }
                else if (doublesertotal >= 1 && doublesertotal <= 249.99)
                {
                    inttotalcount = inttotalcount + 2;
                    int250below = 2;
                }



                int intthirty = 0; int intseventto29 = 0; int intelevento16 = 0; int intsixtoten = 0; int intfive = 0;
                if (doubleTotalAmount == 0 || doublesertotal == 0)
                {
                    if (doubleTotalAmount == 0)
                    {
                        inttotalcount = inttotalcount + 8;
                        intthirty = 8;
                    }
                    else
                        intfive = 0;
                }
                else
                {

                    if (totaldive >= 30)
                    {
                        inttotalcount = inttotalcount + 8;
                        intthirty = 8;
                    }
                    else if (totaldive >= 17 && totaldive <= 29.99)
                    {
                        inttotalcount = inttotalcount + 7;
                        intseventto29 = 7;
                    }
                    else if (totaldive >= 11 && totaldive <= 16.99)
                    {
                        inttotalcount = inttotalcount + 6;
                        intelevento16 = 6;
                    }
                    else if (totaldive >= 6 && totaldive <= 10.99)
                    {
                        inttotalcount = inttotalcount + 2;
                        intsixtoten = 2;
                    }
                    else if (totaldive <= 5.99)
                    {
                        //if (doubleTotalAmount == 0 || doublesertotal == 0)
                        if (doublesertotal == 0)
                        {
                            intfive = 0;

                        }
                        else
                        {
                            inttotalcount = inttotalcount + 1;
                            intfive = 1;
                        }
                    }
                }

                X_Pos = 510;
                Y_Pos -= 58;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inteldercount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intdisablecount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intyoungercount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                // Energy Burden

                X_Pos = 510;
                Y_Pos -= 45;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intthirty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intseventto29.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intelevento16.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsixtoten.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);



                // Consumption Rate

                X_Pos = 510;
                Y_Pos -= 44;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int1000plus.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int500above.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int250above.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int250below.ToString(), TblFontBold), X_Pos, Y_Pos, 0);



                // Poverty 

                X_Pos = 510;
                Y_Pos -= 43;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfity.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsenvtyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intonefiftyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);



                Y_Pos -= 40;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("0".ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                Y_Pos -= 36;
                X_Pos = 510;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttotalcount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                // Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intNoneabove.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                ////Y_Pos -= 42;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfity.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsenvtyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwentyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwentytofifty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfiftyone.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                Y_Pos -= 60;
                if (inttotalcount >= 20)
                {
                    X_Pos = 40;

                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }
                else if (inttotalcount >= 11 && inttotalcount <= 19)
                {
                    X_Pos = 205;

                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }
                else if (inttotalcount <= 10)
                {
                    X_Pos = 370;
                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }

                StringBuilder strMstAppl = new StringBuilder();
                strMstAppl.Append("<Applicants>");
                strMstAppl.Append("<Details MSTApplDetails = \"" + BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + (BaseForm.BaseYear.Trim() == string.Empty ? "    " : BaseForm.BaseYear.Trim()) + BaseForm.BaseApplicationNo + "\" MST_RANK1 = \"" + inttotalcount.ToString() + "\" MST_RANK2 = \"" + "0" + "\" MST_RANK3 = \"" + "0" + "\" MST_RANK4 = \"" + "0" + "\" MST_RANK5 = \"" + "0" + "\" MST_RANK6 = \"" + "0" + "\"   />");
                strMstAppl.Append("</Applicants>");

                if (_model.CaseMstData.UpdateCaseMstRanks(strMstAppl.ToString(), "Single"))
                {
                    BaseForm.BaseCaseMstListEntity[0].Rank1 = inttotalcount.ToString();
                }



            }
            catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

            Hstamper.Close();

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

        private void On_PCS_PriorityRankingForm()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            ReaderName = propReportPath + "\\" + "PCS_Priority_Ranking_2023.pdf";



            PdfName = "PCS_Priority_Sheet";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 10, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            //iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Images\\Tick_icon.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);
            //_image_Checked.ScalePercent(60f);

            List<CustomQuestionsEntity> custResponses = new List<CustomQuestionsEntity>();
            custResponses = _model.CaseMstData.CAPS_CASEUSAGE_GET(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty);

            ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            int intfromage = 0; int inttoage = 0;
            if (programEntity != null)
            {
                intfromage = Convert.ToInt16(programEntity.DepSENFromAge == string.Empty ? "0" : programEntity.DepSENFromAge);
                inttoage = Convert.ToInt16(programEntity.DepSENToAge == string.Empty ? "0" : programEntity.DepSENToAge);
            }
            double doublesertotal = 0;
            CustomQuestionsEntity responsetot = custResponses.Find(u => u.USAGE_MONTH.Equals("TOT"));
            if (responsetot != null)
            {
                doublesertotal = Convert.ToDouble(responsetot.USAGE_TOTAL == string.Empty ? "0" : responsetot.USAGE_TOTAL);
            }

            double doubleTotalAmount = Convert.ToDouble(BaseForm.BaseCaseMstListEntity[0].ProgIncome == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].ProgIncome);
            double totaldive = (doublesertotal / doubleTotalAmount) * 100;
            totaldive = Math.Round(totaldive, 2);
            try
            {
                X_Pos = 30; Y_Pos = 760;

                X_Pos = 100; Y_Pos -= 140;

                int inttotalcount = 0;


                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, TblFontBold), X_Pos, Y_Pos, 0);


                X_Pos = 280;


                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);

                List<CaseSnpEntity> casesnpEligbulity = BaseForm.BaseCaseSnpEntity.FindAll(u => u.DobNa.Equals("0") && u.Status == "A");
                //List<CaseSnpEntity> casesnpElder = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) >= Convert.ToDecimal(intfromage)) && (Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) <= Convert.ToDecimal(inttoage))));
                List<CaseSnpEntity> casesnpElder = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) >= Convert.ToDecimal(60))));
                int inteldercount = 0;
                if (casesnpElder.Count > 0) inteldercount = 4;
                inttotalcount = inttotalcount + inteldercount;

                List<CaseSnpEntity> casesnpyounger = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) <= Convert.ToDecimal(5))));
                int intyoungercount = 0;
                if (casesnpyounger.Count > 0) intyoungercount = 4;
                inttotalcount = inttotalcount + intyoungercount;

                List<CaseSnpEntity> casesnpdisable = casesnpEligbulity.FindAll(u => u.Disable.ToString().ToUpper() == "Y" && u.Status == "A");
                int intdisablecount = 0;
                if (casesnpdisable.Count > 0) intdisablecount = 4;
                inttotalcount = inttotalcount + intdisablecount;

                int intNoneabove = 0;
                if (inttotalcount == 0)
                {
                    inttotalcount = inttotalcount + intNoneabove;
                    intNoneabove = 1;
                }
                int intfity = 0; int intsenvtyfive = 0; int intonefiftyfive = 0;
                int intmstpoverty = Convert.ToInt32(BaseForm.BaseCaseMstListEntity[0].Poverty == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].Poverty);

                if (intmstpoverty <= 50)
                {
                    inttotalcount = inttotalcount + 4;
                    intfity = 4;
                }
                else if (intmstpoverty >= 51 && intmstpoverty <= 75)
                {
                    inttotalcount = inttotalcount + 3;
                    intsenvtyfive = 3;
                }
                else if (intmstpoverty >= 76 && intmstpoverty <= 150)
                {
                    inttotalcount = inttotalcount + 2;
                    intonefiftyfive = 2;
                }
                //else if (intmstpoverty >= 126 && intmstpoverty <= 150)
                //{
                //    inttotalcount = inttotalcount + 1;
                //    inttwentytofifty = 1;
                //}
                //else if (intmstpoverty <= 151)
                //{

                //    intfiftyone = 0;
                //}

                int int1000plus = 0; int int500above = 0; int int250above = 0; int int250below = 0;
                if (doublesertotal >= 1000)
                {
                    inttotalcount = inttotalcount + 4;
                    int1000plus = 4;
                }
                //else if (doublesertotal >= 500 && doublesertotal <= 999.99)
                //{
                //    inttotalcount = inttotalcount + 4;
                //    int500above = 4;
                //}
                //else if (doublesertotal >= 250 && doublesertotal <= 499.99)
                //{
                //    inttotalcount = inttotalcount + 3;
                //    int250above = 3;
                //}
                //else if (doublesertotal >= 1 && doublesertotal <= 249.99)
                //{
                //    inttotalcount = inttotalcount + 2;
                //    int250below = 2;
                //}



                int intthirty = 0; int intseventto29 = 0; int intelevento16 = 0; int intsixtoten = 0; int intfive = 0;
                //if (doubleTotalAmount == 0 || doublesertotal == 0)
                //{
                //    if (doubleTotalAmount == 0)
                //    {
                //        inttotalcount = inttotalcount + 8;
                //        intthirty = 8;
                //    }
                //    else
                //        intfive = 0;
                //}
                //else
                {

                    if (totaldive >= 11)
                    {
                        inttotalcount = inttotalcount + 5;
                        intthirty = 5;
                    }
                    //else if (totaldive >= 17 && totaldive <= 29.99)
                    //{
                    //    inttotalcount = inttotalcount + 7;
                    //    intseventto29 = 7;
                    //}
                    //else if (totaldive >= 11 && totaldive <= 16.99)
                    //{
                    //    inttotalcount = inttotalcount + 6;
                    //    intelevento16 = 6;
                    //}
                    //else if (totaldive >= 6 && totaldive <= 10.99)
                    //{
                    //    inttotalcount = inttotalcount + 2;
                    //    intsixtoten = 2;
                    //}
                    //else if (totaldive <= 5.99)
                    //{
                    //    if (doubleTotalAmount == 0 || doublesertotal == 0)
                    //    {
                    //        intfive = 0;

                    //    }
                    //    else
                    //    {
                    //        inttotalcount = inttotalcount + 1;
                    //        intfive = 1;
                    //    }
                    //}
                }

                X_Pos = 510;
                Y_Pos -= 58;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inteldercount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intdisablecount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intyoungercount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                // Energy Burden

                X_Pos = 510;
                Y_Pos -= 43;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intthirty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intseventto29.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intelevento16.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsixtoten.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);



                // Consumption Rate

                X_Pos = 510;
                Y_Pos -= 43;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int1000plus.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int500above.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int250above.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int250below.ToString(), TblFontBold), X_Pos, Y_Pos, 0);



                // Poverty 

                X_Pos = 510;
                Y_Pos -= 43;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfity.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsenvtyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intonefiftyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                CaseSnpEntity SnpEntity = BaseForm.BaseCaseSnpEntity.Find(u => u.FamilySeq == BaseForm.BaseCaseMstListEntity[0].FamilySeq);
                string Fname = string.Empty; string dob = string.Empty;
                if (SnpEntity != null)
                {
                    Fname = SnpEntity.NameixFi; dob = SnpEntity.AltBdate.Trim();
                }


                //DataSet ds = Captain.DatabaseLayer.MainMenu.MainMenuSearch("ALS", "ALL", string.Empty, string.Empty, string.Empty, string.Empty, Fname, string.Empty,
                //              string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, dob, BaseForm.UserID, string.Empty, string.Empty);
                //int FirstTime = 0;
                //if(ds.Tables[0].Rows.Count>0)
                //{
                //    if(ds.Tables[0].Rows.Count==1) FirstTime = 3;

                //    //foreach(DataRow dr in ds.Tables[0].Rows)
                //    //{
                //    //    if(dr["Agency"].ToString()!=BaseForm.BaseAgency && dr["Dept"].ToString() != BaseForm.BaseDept && dr["Prog"].ToString() != BaseForm.BaseProg && dr["SnpYear"].ToString() != BaseForm.BaseYear)
                //    //    {
                //    //        FirstTime = 3;
                //    //        break;
                //    //    }
                //    //}
                //}


                Y_Pos -= 42;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("0", TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("0", TblFontBold), X_Pos, Y_Pos, 0);


                Y_Pos -= 36;
                X_Pos = 510;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttotalcount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                // Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intNoneabove.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                ////Y_Pos -= 42;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfity.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsenvtyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwentyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwentytofifty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfiftyone.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                Y_Pos -= 60;
                if (inttotalcount >= 20)
                {
                    X_Pos = 40;

                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }
                else if (inttotalcount >= 11 && inttotalcount <= 19)
                {
                    X_Pos = 205;

                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }
                else if (inttotalcount <= 10)
                {
                    X_Pos = 370;
                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }

                StringBuilder strMstAppl = new StringBuilder();
                strMstAppl.Append("<Applicants>");
                strMstAppl.Append("<Details MSTApplDetails = \"" + BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + (BaseForm.BaseYear.Trim() == string.Empty ? "    " : BaseForm.BaseYear.Trim()) + BaseForm.BaseApplicationNo + "\" MST_RANK1 = \"" + inttotalcount.ToString() + "\" MST_RANK2 = \"" + "0" + "\" MST_RANK3 = \"" + "0" + "\" MST_RANK4 = \"" + "0" + "\" MST_RANK5 = \"" + "0" + "\" MST_RANK6 = \"" + "0" + "\"   />");
                strMstAppl.Append("</Applicants>");

                if (_model.CaseMstData.UpdateCaseMstRanks(strMstAppl.ToString(), "Single"))
                {
                    BaseForm.BaseCaseMstListEntity[0].Rank1 = inttotalcount.ToString();
                }



            }
            catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

            Hstamper.Close();

            if (BaseForm.BaseAgencyControlDetails.ReportSwitch.ToUpper() == "Y")
            {
                PdfViewerNewForm objfrm = new PdfViewerNewForm(PdfName);
                objfrm.FormClosed += new FormClosedEventHandler(On_Delete_PDF_File);
                objfrm.ShowDialog();
            }
            else
            {
                FrmViewer objfrm = new FrmViewer(PdfName);
                objfrm.FormClosed += new FormClosedEventHandler(On_Delete_PDF_File);
                objfrm.ShowDialog();
            }

        }


        #endregion

        #region NCCAA Priority Sheet Form

        private void On_NCCAA_PriorityRankingForm()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            ReaderName = propReportPath + "\\" + "NCCAA_Priority_Rating.pdf";



            PdfName = "NCCAA_Priority_Rating";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            //iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Images\\Tick_icon.png"));
            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);
            //_image_Checked.ScalePercent(60f);

            List<CustomQuestionsEntity> custResponses = new List<CustomQuestionsEntity>();
            custResponses = _model.CaseMstData.CAPS_CASEUSAGE_GET(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty);

            ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            int intfromage = 0; int inttoage = 0;
            if (programEntity != null)
            {
                intfromage = Convert.ToInt16(programEntity.DepSENFromAge == string.Empty ? "0" : programEntity.DepSENFromAge);
                inttoage = Convert.ToInt16(programEntity.DepSENToAge == string.Empty ? "0" : programEntity.DepSENToAge);
            }
            double doublesertotal = 0;
            CustomQuestionsEntity responsetot = custResponses.Find(u => u.USAGE_MONTH.Equals("TOT"));
            if (responsetot != null)
            {
                doublesertotal = Convert.ToDouble(responsetot.USAGE_TOTAL == string.Empty ? "0" : responsetot.USAGE_TOTAL);
            }

            double doubleTotalAmount = Convert.ToDouble(BaseForm.BaseCaseMstListEntity[0].ProgIncome == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].ProgIncome);
            double totaldive = (doublesertotal / doubleTotalAmount) * 100;
            totaldive = Math.Round(totaldive, 2);
            try
            {
                X_Pos = 30; Y_Pos = 760;

                X_Pos = 100; Y_Pos -= 105;

                int inttotalcount = 0;

                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("App# ", TblFontBold), X_Pos - 50, Y_Pos, 0);
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, TblFontBold), X_Pos, Y_Pos, 0);


                X_Pos = 280;


                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Name: " + BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);

                List<CaseSnpEntity> casesnpEligbulity = BaseForm.BaseCaseSnpEntity.FindAll(u => u.DobNa.Equals("0") && u.Status == "A");
                //List<CaseSnpEntity> casesnpElder = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) >= Convert.ToDecimal(intfromage)) && (Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) <= Convert.ToDecimal(inttoage))));
                List<CaseSnpEntity> casesnpElder = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) >= Convert.ToDecimal(60))));
                int inteldercount = 0;
                if(casesnpElder.Count>0) inteldercount = 5;
                inttotalcount = inttotalcount + inteldercount;

                List<CaseSnpEntity> casesnpyounger = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) <= Convert.ToDecimal(5))));
                int intyoungercount = 0; 
                if(casesnpyounger.Count>0) intyoungercount = 5;
                inttotalcount = inttotalcount + intyoungercount;

                List<CaseSnpEntity> casesnpdisable = casesnpEligbulity.FindAll(u => u.Disable.ToString().ToUpper() == "Y" && u.Status == "A");
                int intdisablecount = 0; 
                if(casesnpdisable.Count>0) intdisablecount = 5;
                inttotalcount = inttotalcount + intdisablecount;

                List<CaseSnpEntity> casesnpvetran = casesnpEligbulity.FindAll(u => u.MilitaryStatus.ToString().ToUpper() == "V" && u.Status == "A");
                int intVetCount = 0;
                if(casesnpvetran.Count>0)intVetCount = 5;
                inttotalcount = inttotalcount + intVetCount;

                //int intNoneabove = 0;
                //if (inttotalcount == 0)
                //{
                //    inttotalcount = inttotalcount + intNoneabove;
                //    intNoneabove = 1;
                //}
                int intfity = 0; int intsenvtyfive = 0; int intonefiftyfive = 0;
                int intmstpoverty = Convert.ToInt32(BaseForm.BaseCaseMstListEntity[0].Poverty == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].Poverty);

                if (intmstpoverty <= 50)
                {
                    inttotalcount = inttotalcount + 5;
                    intfity = 5;
                }
                else if (intmstpoverty >= 51 && intmstpoverty <= 75)
                {
                    inttotalcount = inttotalcount + 5;
                    intsenvtyfive = 5;
                }
                else if (intmstpoverty >= 76 && intmstpoverty <= 150)
                {
                    inttotalcount = inttotalcount + 5;
                    intonefiftyfive = 5;
                }
                //else if (intmstpoverty >= 126 && intmstpoverty <= 150)
                //{
                //    inttotalcount = inttotalcount + 1;
                //    inttwentytofifty = 1;
                //}
                //else if (intmstpoverty <= 151)
                //{

                //    intfiftyone = 0;
                //}

                int int1000plus = 0; int int500above = 0; int int250above = 0; int int250below = 0;
                if (doublesertotal > 1000)
                {
                    inttotalcount = inttotalcount + 5;
                    int1000plus = 5;
                }
                //else if (doublesertotal >= 500 && doublesertotal <= 999.99)
                //{
                //    inttotalcount = inttotalcount + 4;
                //    int500above = 4;
                //}
                //else if (doublesertotal >= 250 && doublesertotal <= 499.99)
                //{
                //    inttotalcount = inttotalcount + 3;
                //    int250above = 3;
                //}
                //else if (doublesertotal >= 1 && doublesertotal <= 249.99)
                //{
                //    inttotalcount = inttotalcount + 2;
                //    int250below = 2;
                //}



                int intthirty = 0; int intseventto29 = 0; int intelevento16 = 0; int intsixtoten = 0; int intfive = 0;
                if (doubleTotalAmount == 0 || doublesertotal == 0)
                {
                    //if (doubleTotalAmount == 0)
                    //{
                    //    inttotalcount = inttotalcount + 8;
                    //    intthirty = 8;
                    //}
                    //else
                    //    intfive = 0;
                }
                else
                {

                    //if (totaldive >= 30)
                    //{
                    //    inttotalcount = inttotalcount + 8;
                    //    intthirty = 8;
                    //}
                    //else if (totaldive >= 17 && totaldive <= 29.99)
                    //{
                    //    inttotalcount = inttotalcount + 7;
                    //    intseventto29 = 7;
                    //}
                    if (totaldive >= 11) //&& totaldive <= 16.99
                    {
                        inttotalcount = inttotalcount + 6;
                        intelevento16 = 6;
                    }
                    //else if (totaldive >= 6 && totaldive <= 10.99)
                    //{
                    //    inttotalcount = inttotalcount + 2;
                    //    intsixtoten = 2;
                    //}
                    //else if (totaldive <= 5.99)
                    //{
                    //    if (doubleTotalAmount == 0 || doublesertotal == 0)
                    //    {
                    //        intfive = 0;

                    //    }
                    //    else
                    //    {
                    //        inttotalcount = inttotalcount + 1;
                    //        intfive = 1;
                    //    }
                    //}
                }

                X_Pos = 510;
                Y_Pos -= 58;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inteldercount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 25;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intdisablecount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 25;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intyoungercount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 20;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intVetCount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                // Energy Burden

                X_Pos = 510;
                Y_Pos -= 36;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intthirty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intseventto29.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intelevento16.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsixtoten.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(doublesertotal.ToString(), TblFontBold), 210, 476, 0);
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(doubleTotalAmount.ToString(), TblFontBold), 210, 461, 0);

                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(doublesertotal.ToString("0.00"), TblFontBold), 240, Y_Pos - 14, 0);
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(doubleTotalAmount.ToString("0.00"), TblFontBold), 240, Y_Pos - 28, 0);

                // Consumption Rate

                X_Pos = 510;
                Y_Pos -= 63;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int1000plus.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int500above.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int250above.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int250below.ToString(), TblFontBold), X_Pos, Y_Pos, 0);



                // Poverty 

                X_Pos = 510;
                Y_Pos -= 52;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfity.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 23;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsenvtyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 23;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intonefiftyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);



                //Y_Pos -= 40;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("0".ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                Y_Pos -= 32;
                X_Pos = 510;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttotalcount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                // Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intNoneabove.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                ////Y_Pos -= 42;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfity.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsenvtyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwentyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwentytofifty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfiftyone.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                Y_Pos -= 24;
                if (inttotalcount >= 29)
                {
                    X_Pos = 140;

                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }
                else if (inttotalcount >= 19 && inttotalcount <= 28)
                {
                    X_Pos = 330;

                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }
                else if (inttotalcount <= 18)
                {
                    X_Pos = 510;
                    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                    cb.AddImage(_image_Tick);
                }

                StringBuilder strMstAppl = new StringBuilder();
                strMstAppl.Append("<Applicants>");
                strMstAppl.Append("<Details MSTApplDetails = \"" + BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + (BaseForm.BaseYear.Trim() == string.Empty ? "    " : BaseForm.BaseYear.Trim()) + BaseForm.BaseApplicationNo + "\" MST_RANK1 = \"" + inttotalcount.ToString() + "\" MST_RANK2 = \"" + "0" + "\" MST_RANK3 = \"" + "0" + "\" MST_RANK4 = \"" + "0" + "\" MST_RANK5 = \"" + "0" + "\" MST_RANK6 = \"" + "0" + "\"   />");
                strMstAppl.Append("</Applicants>");

                if (_model.CaseMstData.UpdateCaseMstRanks(strMstAppl.ToString(), "Single"))
                {
                    BaseForm.BaseCaseMstListEntity[0].Rank1 = inttotalcount.ToString();
                }



            }
            catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

            Hstamper.Close();

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


        #endregion

        #region FORTWORTH Priority Sheet Form

        private void On_Fortworth_PriorityRankingForm()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            ReaderName = propReportPath + "\\" + "FORTWORTH_Priority_Rating.pdf";



            PdfName = "FORTWORTH_Priority_Rating";//form.GetFileName();
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

            PdfReader Hreader = new PdfReader(ReaderName);

            PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
            Hstamper.Writer.SetPageSize(PageSize.A4);
            PdfContentByte cb = Hstamper.GetOverContent(1);


            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
            iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
            iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

            //iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Images\\Tick_icon.png"));
            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
            // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_Tick.ScalePercent(60f);
            //_image_Checked.ScalePercent(60f);

            List<CustomQuestionsEntity> custResponses = new List<CustomQuestionsEntity>();
            custResponses = _model.CaseMstData.CAPS_CASEUSAGE_GET(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty);

            ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            int intfromage = 0; int inttoage = 0;
            if (programEntity != null)
            {
                intfromage = Convert.ToInt16(programEntity.DepSENFromAge == string.Empty ? "0" : programEntity.DepSENFromAge);
                inttoage = Convert.ToInt16(programEntity.DepSENToAge == string.Empty ? "0" : programEntity.DepSENToAge);
            }
            double doublesertotal = 0;
            CustomQuestionsEntity responsetot = custResponses.Find(u => u.USAGE_MONTH.Equals("TOT"));
            if (responsetot != null)
            {
                doublesertotal = Convert.ToDouble(responsetot.USAGE_TOTAL == string.Empty ? "0" : responsetot.USAGE_TOTAL);
            }

            double doubleTotalAmount = Convert.ToDouble(BaseForm.BaseCaseMstListEntity[0].ProgIncome == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].ProgIncome);
            double totaldive = (doublesertotal / doubleTotalAmount) * 100;
            totaldive = Math.Round(totaldive, 2);
            try
            {
                X_Pos = 30; Y_Pos = 760;

                X_Pos = 150; Y_Pos -= 135;

                int inttotalcount = 0;

                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(doublesertotal.ToString(), TblFontBold), X_Pos - 70, Y_Pos, 0);
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(doubleTotalAmount.ToString(), TblFontBold), X_Pos-200, Y_Pos, 0);


                X_Pos = 240;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(doubleTotalAmount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                X_Pos = 390;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(totaldive.ToString() + "%", TblFontBold), X_Pos, Y_Pos, 0);

                List<CaseSnpEntity> casesnpEligbulity = BaseForm.BaseCaseSnpEntity.FindAll(u => u.DobNa.Equals("0") && u.Status == "A");
                List<CaseSnpEntity> casesnpElder = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) >= Convert.ToDecimal(60)))); //&& (Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) <= Convert.ToDecimal(inttoage))));
                int inteldercount = 0;
                if (casesnpElder.Count > 0) inteldercount = 4;
                //inttotalcount = inttotalcount + inteldercount;

                List<CaseSnpEntity> casesnpyounger = casesnpEligbulity.FindAll(u => ((Convert.ToDecimal(u.Age == string.Empty ? 0 : Convert.ToDecimal(u.Age)) <= Convert.ToDecimal(5))));
                int intyoungercount = 0;
                if (casesnpyounger.Count > 0) intyoungercount = 4;
                //inttotalcount = inttotalcount + intyoungercount;

                List<CaseSnpEntity> casesnpdisable = casesnpEligbulity.FindAll(u => u.Disable.ToString().ToUpper() == "Y" && u.Status == "A");
                int intdisablecount = 0;
                if (casesnpdisable.Count > 0) intdisablecount = 4;
                //inttotalcount = inttotalcount + intdisablecount;

                List<CaseSnpEntity> casesnpvetran = casesnpEligbulity.FindAll(u => u.MilitaryStatus.ToString().ToUpper() == "V" && u.Status == "A");
                int intVetCount = 0;
                if (casesnpvetran.Count > 0) intVetCount = 4;
                //inttotalcount = inttotalcount + intVetCount;

                //int intNoneabove = 0;
                //if (inttotalcount == 0)
                //{
                //    inttotalcount = inttotalcount + intNoneabove;
                //    intNoneabove = 1;
                //}
                int intfity = 0; int intsenvtyfive = 0; int intonefiftyfive = 0;
                int intmstpoverty = Convert.ToInt32(BaseForm.BaseCaseMstListEntity[0].Poverty == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].Poverty);

                if (intmstpoverty <= 50)
                {
                    inttotalcount = inttotalcount + 4;
                    intfity = 4;
                }
                else if (intmstpoverty >= 51 && intmstpoverty <= 75)
                {
                    inttotalcount = inttotalcount + 3;
                    intsenvtyfive = 3;
                }
                else if (intmstpoverty >= 76 && intmstpoverty <= 150)
                {
                    inttotalcount = inttotalcount + 2;
                    intonefiftyfive = 2;
                }
                //else if (intmstpoverty >= 126 && intmstpoverty <= 150)
                //{
                //    inttotalcount = inttotalcount + 1;
                //    inttwentytofifty = 1;
                //}
                //else if (intmstpoverty <= 151)
                //{

                //    intfiftyone = 0;
                //}

                int int1000plus = 0; int int500above = 0; int int250above = 0; int int250below = 0;
                if (doublesertotal > 1000)
                {
                    inttotalcount = inttotalcount + 4;
                    int1000plus = 4;
                }
                //else if (doublesertotal >= 500 && doublesertotal <= 999.99)
                //{
                //    inttotalcount = inttotalcount + 4;
                //    int500above = 4;
                //}
                //else if (doublesertotal >= 250 && doublesertotal <= 499.99)
                //{
                //    inttotalcount = inttotalcount + 3;
                //    int250above = 3;
                //}
                //else if (doublesertotal >= 1 && doublesertotal <= 249.99)
                //{
                //    inttotalcount = inttotalcount + 2;
                //    int250below = 2;
                //}


                int Householcnt = 0;
                if (intdisablecount > 0) Householcnt = 4;
                else if (inteldercount > 0) Householcnt = 4;
                else if (intyoungercount > 0) Householcnt = 4;
                else if (intVetCount > 0) Householcnt = 4;

                if (Householcnt > 0) { inttotalcount = inttotalcount + Householcnt; }

                int intthirty = 0; int intseventto29 = 0; int intelevento16 = 0; int intsixtoten = 0; int intfive = 0;
                if (doubleTotalAmount == 0 || doublesertotal == 0)
                {
                    //if (doubleTotalAmount == 0)
                    //{
                    //    inttotalcount = inttotalcount + 8;
                    //    intthirty = 8;
                    //}
                    //else
                    //    intfive = 0;
                }
                else
                {
                    if (totaldive >= 5 && totaldive <= 7.99)
                    {
                        inttotalcount = inttotalcount + 2;
                        intelevento16 = 2;
                    }
                    else if (totaldive >= 8 && totaldive <= 10.99)
                    {
                        inttotalcount = inttotalcount + 3;
                        intelevento16 = 3;
                    }
                    else if (totaldive >= 11)
                    {
                        inttotalcount = inttotalcount + 5;
                        intelevento16 = 5;
                    }

                }

                int Poverty = 0;
                if (intfity > 0) { Poverty = intfity; }
                else if (intsenvtyfive > 0) { Poverty = intsenvtyfive; }
                else if (intonefiftyfive > 0) { Poverty = intonefiftyfive; }

                X_Pos = 470;
                Y_Pos -= 58;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inteldercount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 25;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intdisablecount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 17;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intelevento16.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                Y_Pos -= 20;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intVetCount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                // Energy Burden

                X_Pos = 470;
                Y_Pos -= 50;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intthirty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intseventto29.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int1000plus.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsixtoten.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(doublesertotal.ToString(), TblFontBold), 210, 476, 0);
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(doubleTotalAmount.ToString(), TblFontBold), 210, 461, 0);


                Y_Pos -= 103;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Householcnt.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                // Consumption Rate

                //X_Pos = 510;
                Y_Pos -= 72;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Poverty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int500above.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int250above.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 14;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(int250below.ToString(), TblFontBold), X_Pos, Y_Pos, 0);



                // Poverty 

                X_Pos = 470;
                Y_Pos -= 28;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfity.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 23;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsenvtyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                //Y_Pos -= 23;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intonefiftyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);



                //Y_Pos -= 40;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("0".ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                //Y_Pos -= 32;
                //X_Pos = 510;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttotalcount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);


                // Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intNoneabove.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                ////Y_Pos -= 42;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfity.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intsenvtyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwentyfive.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(inttwentytofifty.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                //Y_Pos -= 17;
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intfiftyone.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                if (inteldercount > 0 || intyoungercount > 0 || intdisablecount > 0)
                {
                    X_Pos = 60; Y_Pos -= 27;
                    if (inttotalcount >= 14)
                    {
                        _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                        cb.AddImage(_image_Tick);
                    }
                    else if (inttotalcount <= 13)
                    {
                        Y_Pos -= 24;
                        _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 34);
                        cb.AddImage(_image_Tick);
                    }
                }
                else
                {
                    X_Pos = 60; Y_Pos -= 27;
                    if (inttotalcount >= 14)
                    {
                        X_Pos = 305;
                        _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                        cb.AddImage(_image_Tick);
                    }
                    else if (inttotalcount <= 13)
                    {
                        Y_Pos -= 24; X_Pos = 305;
                        _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 34);
                        cb.AddImage(_image_Tick);
                    }
                }

                Y_Pos -= 85; X_Pos = 185;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);

                X_Pos = 455;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, TblFontBold), X_Pos, Y_Pos, 0);


                //Y_Pos -= 24;
                //if (inttotalcount >= 29)
                //{
                //    X_Pos = 140;

                //    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                //    cb.AddImage(_image_Tick);
                //}
                //else if (inttotalcount >= 19 && inttotalcount <= 28)
                //{
                //    X_Pos = 330;

                //    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                //    cb.AddImage(_image_Tick);
                //}
                //else if (inttotalcount <= 18)
                //{
                //    X_Pos = 305;
                //    _image_Tick.SetAbsolutePosition(X_Pos, Y_Pos - 10);
                //    cb.AddImage(_image_Tick);
                //}



                StringBuilder strMstAppl = new StringBuilder();
                strMstAppl.Append("<Applicants>");
                strMstAppl.Append("<Details MSTApplDetails = \"" + BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + (BaseForm.BaseYear.Trim() == string.Empty ? "    " : BaseForm.BaseYear.Trim()) + BaseForm.BaseApplicationNo + "\" MST_RANK1 = \"" + inttotalcount.ToString() + "\" MST_RANK2 = \"" + "0" + "\" MST_RANK3 = \"" + "0" + "\" MST_RANK4 = \"" + "0" + "\" MST_RANK5 = \"" + "0" + "\" MST_RANK6 = \"" + "0" + "\"   />");
                strMstAppl.Append("</Applicants>");

                if (_model.CaseMstData.UpdateCaseMstRanks(strMstAppl.ToString(), "Single"))
                {
                    BaseForm.BaseCaseMstListEntity[0].Rank1 = inttotalcount.ToString();
                }



            }
            catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

            Hstamper.Close();

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


        #endregion


        #region  Elig Letter

        private void On_PCS_EligLet()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;
            if (BaseForm.BaseCaseMstListEntity[0].Language == "02")
            {
                ReaderName = propReportPath + "\\" + "PCS_EligLet_Spanish.pdf";
            }
            else
            {
                ReaderName = propReportPath + "\\" + "PCS_EligLet.pdf";
            }


            PdfName = "PCS_EligLet";//form.GetFileName();
            if (SPMCode != string.Empty)
            {
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

                PdfReader Hreader = new PdfReader(ReaderName);

                PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
                Hstamper.Writer.SetPageSize(PageSize.A4);
                PdfContentByte cb = Hstamper.GetOverContent(1);


                BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
                iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
                BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
                iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
                BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

                iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 11);
                iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
                iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
                iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
                iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
                iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

                iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
                // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

                _image_Tick.ScalePercent(60f);
                //_image_Checked.ScalePercent(60f);

                Get_Vendor_List();

                Get_App_CASEACT_List();

                try
                {

                    string HN = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim())) HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + " ";
                    string Direction = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Direction.Trim())) Direction = BaseForm.BaseCaseMstListEntity[0].Direction.Trim() + " ";
                    string Street = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Street.Trim())) Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + " ";
                    string Suffix = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim())) Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + " ";
                    string Address = HN + Direction + Street + Suffix;


                    X_Pos = 60; Y_Pos = 760;

                    X_Pos = 120; Y_Pos -= 103;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 460;


                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].ApplNo, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 120; Y_Pos -= 14;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Address, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 120; Y_Pos -= 14;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].City + " " + BaseForm.BaseCaseMstListEntity[0].State + " " + BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 120; Y_Pos -= 14;





                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(LookupDataAccess.GetPhoneFormat(BaseForm.BaseCaseMstListEntity[0].Area + BaseForm.BaseCaseMstListEntity[0].Phone), TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 120; Y_Pos -= 92;

                    string strAccountNumber = string.Empty;
                    string strVendorName = string.Empty;
                    if (SP_ElectricActivity_Details.Count > 0)
                    {
                        strAccountNumber = SP_ElectricActivity_Details[0].Account;
                        if (CaseVddlist.Count > 0)
                        {
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_ElectricActivity_Details[0].Vendor_No);
                            if (vddname != null)
                            {
                                strVendorName = vddname.Name;
                            }
                        }
                    }
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strVendorName, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 370;

                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAccountNumber, TblFontBold), X_Pos, Y_Pos, 0);


                    Y_Pos -= 20;






                    decimal ResAmount = 0; string ChkNo = string.Empty; string ChkDate = string.Empty; string ServsDate = string.Empty; string SeekDate = string.Empty;
                    CASEACTEntity CaseactRec = new CASEACTEntity();

                    ResAmount = 0; ChkNo = string.Empty; ChkDate = string.Empty; ServsDate = string.Empty; SeekDate = string.Empty; CaseactRec = new CASEACTEntity();
                    string strAmount = string.Empty;
                    List<CASEACTEntity> CaseactList = new List<CASEACTEntity>();
                    string strElec = "E";
                    if (SP_ElectricActivity_Details.Count > 0)
                    {
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "1" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }

                        Y_Pos -= 14;
                        X_Pos = 165;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 400;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 165;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "2" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 400;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 165;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "3" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 400;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "9" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 165;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "4" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 400;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "10" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 165;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "5" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        strAmount = string.Empty;
                        X_Pos = 400;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "11" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 165;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "6" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 400;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "12" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        List<CASEACTEntity> SPToatalDetails = SP_ElectricActivity_Details.FindAll(u => u.Cost != "");
                        decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                        Y_Pos -= 14;
                        X_Pos = 400;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(dectotalAmount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                    }




                    if (SP_OtherActivity_Details.Count > 0)
                    {


                        Y_Pos = 359;
                        strAccountNumber = string.Empty;
                        strVendorName = string.Empty;

                        strAccountNumber = SP_OtherActivity_Details[0].Account;
                        if (CaseVddlist.Count > 0)
                        {
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_OtherActivity_Details[0].Vendor_No);
                            if (vddname != null)
                            {
                                strVendorName = vddname.Name;
                            }
                        }

                        X_Pos = 120;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strVendorName, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 370;

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAccountNumber, TblFontBold), X_Pos, Y_Pos, 0);

                    }
                    strElec = "O";
                    Y_Pos = 337;
                    if (SP_OtherActivity_Details.Count > 0)
                    {
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "1" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }

                        Y_Pos -= 14;
                        X_Pos = 165;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 400;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 165;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "2" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 400;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 165;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "3" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 400;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "9" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 165;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "4" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 400;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "10" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 165;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "5" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        strAmount = string.Empty;
                        X_Pos = 400;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "11" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 165;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "6" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 400;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "12" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        List<CASEACTEntity> SPToatalDetails = SP_OtherActivity_Details.FindAll(u => u.Cost != "");
                        decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                        Y_Pos -= 14;
                        X_Pos = 400;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(dectotalAmount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);

                    }






                }
                catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

                Hstamper.Close();

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
            else
            {
                CommonFunctions.MessageBoxDisplay("Benefit Service Plan Not Defined.");
            }
        }

        private void On_EligLetterNew()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            string FileName = string.Empty;

            ReaderName = propReportPath + "\\" + "EligLet_ENG.pdf";
            PdfName = "EligLet_ENG";
            if (BaseForm.BaseCaseMstListEntity[0].Language == "02")
            {
                ReaderName = propReportPath + "\\" + "EligLet_SPAN.pdf";
                PdfName = "EligLet_SPAN";               
            }
            if (!File.Exists(ReaderName))
            { 
                CommonFunctions.MessageBoxDisplay(PdfName+ ".pdf Missed, Please contact CAPSYSTEMS administrator.");
                return;
            }

            FileName = PdfName;

            if (SPMCode != string.Empty)
            {
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

                PdfReader Hreader = new PdfReader(ReaderName);

                PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
                Hstamper.Writer.SetPageSize(PageSize.A4);
                PdfContentByte cb = Hstamper.GetOverContent(1);


                BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
                iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
                BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
                iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
                BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

                iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 11);
                iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
                iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11);
                //iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
                iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
                iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
                iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

                iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));

                _image_Tick.ScalePercent(60f);
                //_image_Checked.ScalePercent(60f);

                Get_Vendor_List();

                Get_App_CASEACT_List();

                try
                {

                    string HN = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim())) HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + " ";
                    string Direction = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Direction.Trim())) Direction = BaseForm.BaseCaseMstListEntity[0].Direction.Trim() + " ";
                    string Street = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Street.Trim())) Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + " ";
                    string Suffix = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim())) Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + " ";
                    string Address = HN + Direction + Street + Suffix;


                    X_Pos = 60; Y_Pos = 760;

                    X_Pos = 117; Y_Pos -= 99;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 470;


                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].ApplNo, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 117; Y_Pos -= 17;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Address + " " + BaseForm.BaseCaseMstListEntity[0].City + " " + BaseForm.BaseCaseMstListEntity[0].State + " " + BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);

                    // X_Pos = 120; Y_Pos -= 14;
                    // ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].City + " " + BaseForm.BaseCaseMstListEntity[0].State + " " + BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 75; Y_Pos -= 20;





                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(LookupDataAccess.GetPhoneFormat(BaseForm.BaseCaseMstListEntity[0].Area + BaseForm.BaseCaseMstListEntity[0].Phone), TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 117; Y_Pos -= 92;

                    string strAccountNumber = string.Empty;
                    string strFirstVendorName = string.Empty;
                    string strVendor1Amount = string.Empty;
                    string strVendor2Amount = string.Empty;
                    if (SP_ElectricActivity_Details.Count > 0)
                    {
                        strAccountNumber = SP_ElectricActivity_Details[0].Account;
                        if (CaseVddlist.Count > 0)
                        {
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_ElectricActivity_Details[0].Vendor_No);
                            if (vddname != null)
                            {
                                strFirstVendorName = vddname.Name;
                            }
                        }
                    }
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strFirstVendorName, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 430;

                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAccountNumber, TblFontBold), X_Pos, Y_Pos, 0);


                    Y_Pos -= 26;






                    decimal ResAmount = 0; string ChkNo = string.Empty; string ChkDate = string.Empty; string ServsDate = string.Empty; string SeekDate = string.Empty;
                    CASEACTEntity CaseactRec = new CASEACTEntity();

                    ResAmount = 0; ChkNo = string.Empty; ChkDate = string.Empty; ServsDate = string.Empty; SeekDate = string.Empty; CaseactRec = new CASEACTEntity();
                    string strAmount = string.Empty;
                    List<CASEACTEntity> CaseactList = new List<CASEACTEntity>();
                    string strElec = "E";
                    if (SP_ElectricActivity_Details.Count > 0)
                    {
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "1" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }

                        Y_Pos -= 14;
                        X_Pos = 240;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 500;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 240;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "2" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 500;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 240;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "3" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 500;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "9" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 240;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "4" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 500;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "10" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 240;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "5" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        strAmount = string.Empty;
                        X_Pos = 500;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "11" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 240;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "6" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 500;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "12" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        List<CASEACTEntity> SPToatalDetails = SP_ElectricActivity_Details.FindAll(u => u.Cost != "");
                        decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                        Y_Pos -= 14;
                        X_Pos = 500;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(dectotalAmount.ToString("N2"), TblFontBold), X_Pos, Y_Pos, 0);
                        strVendor1Amount = dectotalAmount.ToString();
                    }


                    string strsecondVendorName = string.Empty;

                    if (SP_OtherActivity_Details.Count > 0)
                    {


                        Y_Pos = 388;
                        strAccountNumber = string.Empty;
                        strsecondVendorName = string.Empty;

                        strAccountNumber = SP_OtherActivity_Details[0].Account;
                        if (CaseVddlist.Count > 0)
                        {
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_OtherActivity_Details[0].Vendor_No);
                            if (vddname != null)
                            {
                                strsecondVendorName = vddname.Name;
                            }
                        }

                        X_Pos = 117;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strsecondVendorName, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 430;

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAccountNumber, TblFontBold), X_Pos, Y_Pos, 0);

                    }
                    strElec = "O";
                    Y_Pos = 362;
                    if (SP_OtherActivity_Details.Count > 0)
                    {
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "1" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }

                        Y_Pos -= 14;
                        X_Pos = 240;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 500;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 240;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "2" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 500;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 240;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "3" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 500;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "9" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 240;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "4" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 500;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "10" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 240;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "5" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        strAmount = string.Empty;
                        X_Pos = 500;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "11" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 240;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "6" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 500;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "12" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        List<CASEACTEntity> SPToatalDetails = SP_OtherActivity_Details.FindAll(u => u.Cost != "");
                        decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                        Y_Pos -= 14;
                        X_Pos = 500;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(dectotalAmount.ToString("N2"), TblFontBold), X_Pos, Y_Pos, 0);
                        strVendor2Amount = dectotalAmount.ToString();

                    }
                    Y_Pos = 220;
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strFirstVendorName.ToString(), TblFontBold), 110, Y_Pos, 0);
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strVendor1Amount.ToString(), TblFontBold), 410, Y_Pos, 0);

                    Y_Pos = 240;
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strsecondVendorName.ToString(), TblFontBold), 110, Y_Pos, 0);
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strVendor2Amount.ToString(), TblFontBold), 410, Y_Pos, 0);

                    if (FileName == "EligLet_SPAN")
                    { if (BaseForm.BaseAgencyControlDetails.AgyShortName == "STDC") Y_Pos = 100;
                        else Y_Pos = 97;
                    }
                    else
                        Y_Pos = 95;//85; 97
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(LookupDataAccess.Getdate(DateTime.Now.Date.ToShortDateString()), TblFontBold), 410, Y_Pos, 0);



                }
                catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

                Hstamper.Close();

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
            else
            {
                CommonFunctions.MessageBoxDisplay("Benefit Service Plan Not Defined.");
            }
        }

        private void On_NCCAAEligLetterNew()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            string FileName = string.Empty;

            ReaderName = propReportPath + "\\" + "NCCAA_CEAP_ELIG.pdf";
            PdfName = "NCCAA_CEAP_ELIG";
            //if (BaseForm.BaseCaseMstListEntity[0].Language == "02")
            //{
            //    ReaderName = propReportPath + "\\" + "EligLet_SPAN.pdf";
            //    PdfName = "EligLet_SPAN";
            //}
            if (!File.Exists(ReaderName))
            {
                CommonFunctions.MessageBoxDisplay(PdfName + ".pdf Missed, Please contact CAPSYSTEMS administrator.");
                return;
            }

            FileName = PdfName;

            if (SPMCode != string.Empty)
            {
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

                PdfReader Hreader = new PdfReader(ReaderName);

                PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
                Hstamper.Writer.SetPageSize(PageSize.A4);
                PdfContentByte cb = Hstamper.GetOverContent(1);


                BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
                iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
                BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
                iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
                BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

                iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 11);
                iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
                iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11);
                //iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
                iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
                iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
                iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

                iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));

                _image_Tick.ScalePercent(60f);
                //_image_Checked.ScalePercent(60f);

                Get_Vendor_List();

                Get_App_CASEACT_List();

                try
                {

                    string HN = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim())) HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + " ";
                    string Direction = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Direction.Trim())) Direction = BaseForm.BaseCaseMstListEntity[0].Direction.Trim() + " ";
                    string Street = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Street.Trim())) Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + " ";
                    string Suffix = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim())) Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + " ";
                    string Address = HN + Direction + Street + Suffix;


                    X_Pos = 60; Y_Pos = 760;

                    X_Pos = 123; Y_Pos -= 96;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 450;


                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].ApplNo, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 127; Y_Pos -= 24;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Address, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 450;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(LookupDataAccess.GetPhoneFormat(BaseForm.BaseCaseMstListEntity[0].Area + BaseForm.BaseCaseMstListEntity[0].Phone), TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 78; Y_Pos -= 24;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].City ,TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 270;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].State, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 430;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);

                    // X_Pos = 120; Y_Pos -= 14;
                    // ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].City + " " + BaseForm.BaseCaseMstListEntity[0].State + " " + BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);


                    //X_Pos = 75; Y_Pos -= 20;

                    
                    X_Pos = 131; Y_Pos -= 103;

                    string strAccountNumber = string.Empty;
                    string strFirstVendorName = string.Empty;
                    string strVendor1Amount = string.Empty;
                    string strVendor2Amount = string.Empty;
                    if (SP_ElectricActivity_Details.Count > 0)
                    {
                        strAccountNumber = SP_ElectricActivity_Details[0].Account;
                        if (CaseVddlist.Count > 0)
                        {
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_ElectricActivity_Details[0].Vendor_No);
                            if (vddname != null)
                            {
                                strFirstVendorName = vddname.Name;
                            }
                        }
                    }
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strFirstVendorName, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 440;

                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAccountNumber, TblFontBold), X_Pos, Y_Pos, 0);


                    Y_Pos -= 26;
                    string strsecondVendorName = string.Empty;

                    if (SP_OtherActivity_Details.Count > 0)
                    {
                        //Y_Pos = 388;
                        strAccountNumber = string.Empty;
                        strsecondVendorName = string.Empty;

                        strAccountNumber = SP_OtherActivity_Details[0].Account;
                        if (CaseVddlist.Count > 0)
                        {
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_OtherActivity_Details[0].Vendor_No);
                            if (vddname != null)
                            {
                                strsecondVendorName = vddname.Name;
                            }
                        }

                        X_Pos = 140;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strsecondVendorName, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 440;//365;

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAccountNumber, TblFontBold), X_Pos, Y_Pos, 0);

                    }

                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(SecondarySourceName.ToString(), TblFontBold), 207, Y_Pos-50, 0);
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(SecondarySourceName.ToString(), TblFontBold), 483, Y_Pos-50, 0);

                    Y_Pos -= 50;


                    decimal ResAmount = 0; string ChkNo = string.Empty; string ChkDate = string.Empty; string ServsDate = string.Empty; string SeekDate = string.Empty;
                    CASEACTEntity CaseactRec = new CASEACTEntity();

                    ResAmount = 0; ChkNo = string.Empty; ChkDate = string.Empty; ServsDate = string.Empty; SeekDate = string.Empty; CaseactRec = new CASEACTEntity();
                    string strAmount = string.Empty;
                    List<CASEACTEntity> CaseactList = new List<CASEACTEntity>();
                    string strElec = "E";
                    if (SP_ElectricActivity_Details.Count > 0)
                    {
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "1" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }

                        Y_Pos -= 17;
                        X_Pos = 150;//175;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 215;//240;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "1" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 270;//305;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "1");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }


                        X_Pos = 428;// 453;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 488;// 513;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 548;// 573;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "7");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }


                        Y_Pos -= 20;
                        X_Pos = 150;//175;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "2" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 215;//240;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "2" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 270;//305;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "2");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        X_Pos = 428;// 453;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 488;// 513;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 548;// 573;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "8");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 20;
                        X_Pos = 150;//175;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "3" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 215;//240;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "3" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 270;// 305;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "3");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        X_Pos = 428;// 453;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "9" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 488;// 513;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "9" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 548;// 573;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "9");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }


                        Y_Pos -= 20;
                        X_Pos = 150;//175;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "4" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 215;//240;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "4" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 270;// 305;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "4");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        X_Pos = 428;// 453;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "10" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 488;// 513;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "10" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 548;// 573;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "10");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 20;
                        X_Pos = 150;//175;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "5" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 215;//240;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "5" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 270;//305;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "5");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        strAmount = string.Empty;
                        X_Pos = 428;// 453;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "11" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 488;// 513;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "11" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 548;// 573;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "11");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 20;
                        X_Pos = 150;//175;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "6" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 215;// 240;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "6" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 270;// 305;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "6");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        X_Pos = 428;// 453;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "12" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 488;// 513;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "12" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 548;// 573;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "12");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        List<CASEACTEntity> SPToatalDetails = SP_ElectricActivity_Details.FindAll(u => u.Cost != "");
                        decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                        Y_Pos -= 22;//20;
                        X_Pos = 428;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(dectotalAmount.ToString("N2"), TblFontBold), X_Pos, Y_Pos, 0);
                        strVendor1Amount = dectotalAmount.ToString();

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            List<CASEACTEntity> SPOthToatalDetails = SP_OtherActivity_Details.FindAll(u => u.Cost != "");
                            decimal decOthtotalAmount = SPOthToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                            //Y_Pos -= 20;
                            X_Pos = 488;// 513;
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(decOthtotalAmount.ToString("N2"), TblFontBold), X_Pos, Y_Pos, 0);
                            strVendor2Amount = decOthtotalAmount.ToString();
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 548;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_CENTER, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                    }


                    Y_Pos = 170;//85; 97
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(LookupDataAccess.Getdate(DateTime.Now.Date.ToShortDateString()), TblFontBold), 445, Y_Pos, 0);

                }
                catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

                Hstamper.Close();

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
            else
            {
                CommonFunctions.MessageBoxDisplay("Benefit Service Plan Not Defined.");
            }
        }


        private void On_FORTWORTHEligLetterNew()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            string FileName = string.Empty;

            ReaderName = propReportPath + "\\" + "FORTWORTH_CEAP_ELIG.pdf";
            PdfName = "FORTWORTH_CEAP_ELIG";
            //if (BaseForm.BaseCaseMstListEntity[0].Language == "02")
            //{
            //    ReaderName = propReportPath + "\\" + "EligLet_SPAN.pdf";
            //    PdfName = "EligLet_SPAN";
            //}
            if (!File.Exists(ReaderName))
            {
                CommonFunctions.MessageBoxDisplay(PdfName + ".pdf Missed, Please contact CAPSYSTEMS administrator.");
                return;
            }

            FileName = PdfName;

            if (SPMCode != string.Empty)
            {
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

                PdfReader Hreader = new PdfReader(ReaderName);

                PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
                Hstamper.Writer.SetPageSize(PageSize.A4);
                PdfContentByte cb = Hstamper.GetOverContent(1);


                BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
                iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
                BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
                iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
                BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

                iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 11);
                iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
                iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11);
                //iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
                iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
                iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
                iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

                iTextSharp.text.Font TblFontBold10 = new iTextSharp.text.Font(1, 10);

                iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));

                _image_Tick.ScalePercent(60f);
                //_image_Checked.ScalePercent(60f);

                Get_Vendor_List();

                Get_App_CASEACT_List();

                try
                {

                    string HN = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim())) HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + " ";
                    string Direction = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Direction.Trim())) Direction = BaseForm.BaseCaseMstListEntity[0].Direction.Trim() + " ";
                    string Street = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Street.Trim())) Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + " ";
                    string Suffix = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim())) Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + " ";
                    string Address = HN + Direction + Street + Suffix;


                    X_Pos = 60; Y_Pos = 760;

                    X_Pos = 200; Y_Pos -= 100;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);


                    //X_Pos = 450;

                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].ApplNo, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 137; Y_Pos -= 14;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Address, TblFontBold10), X_Pos, Y_Pos, 0);

                    //X_Pos = 450;
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(LookupDataAccess.GetPhoneFormat(BaseForm.BaseCaseMstListEntity[0].Area + BaseForm.BaseCaseMstListEntity[0].Phone), TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 147; Y_Pos -= 12;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].City+","+ BaseForm.BaseCaseMstListEntity[0].State+","+ BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold10), X_Pos, Y_Pos, 0);

                    //X_Pos = 270;
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].State, TblFontBold), X_Pos, Y_Pos, 0);

                    //X_Pos = 430;
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);

                    //// X_Pos = 120; Y_Pos -= 14;
                    //// ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].City + " " + BaseForm.BaseCaseMstListEntity[0].State + " " + BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);


                    //X_Pos = 75; Y_Pos -= 20;


                    X_Pos = 470; Y_Pos -= 45;

                    string strAccountNumber = string.Empty;
                    string strFirstVendorName = string.Empty;
                    string strVendor1Amount = string.Empty;
                    string strVendor2Amount = string.Empty;
                    if (SP_ElectricActivity_Details.Count > 0)
                    {
                        strAccountNumber = SP_ElectricActivity_Details[0].Account;
                        List<CASEACTEntity> SPToatalDetails = SP_ElectricActivity_Details.FindAll(u => u.Cost != "");
                        decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                        if(dectotalAmount>0) strVendor1Amount=dectotalAmount.ToString();

                        if (CaseVddlist.Count > 0)
                        {
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_ElectricActivity_Details[0].Vendor_No);
                            if (vddname != null)
                            {
                                strFirstVendorName = vddname.Name;
                            }
                        }
                    }

                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strVendor1Amount, TblFontBold), X_Pos, Y_Pos, 0);


                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strFirstVendorName, TblFontBold), X_Pos, Y_Pos, 0);

                    //X_Pos = 440;

                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAccountNumber, TblFontBold), X_Pos, Y_Pos, 0);


                    Y_Pos -= 26;
                    string strsecondVendorName = string.Empty;

                    if (SP_OtherActivity_Details.Count > 0)
                    {
                        //Y_Pos = 388;
                        strAccountNumber = string.Empty;
                        strsecondVendorName = string.Empty;

                        strAccountNumber = SP_OtherActivity_Details[0].Account;
                        List<CASEACTEntity> SPToatalDetails = SP_OtherActivity_Details.FindAll(u => u.Cost != "");
                        decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                        if (dectotalAmount > 0) strVendor2Amount = dectotalAmount.ToString();
                        if (CaseVddlist.Count > 0)
                        {
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_OtherActivity_Details[0].Vendor_No);
                            if (vddname != null)
                            {
                                strsecondVendorName = vddname.Name;
                            }
                        }

                        X_Pos = 140;
                        //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strsecondVendorName, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 440;//365;

                        //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAccountNumber, TblFontBold), X_Pos, Y_Pos, 0);

                    }


                    Y_Pos -= 50;



                    decimal ResAmount = 0; string ChkNo = string.Empty; string ChkDate = string.Empty; string ServsDate = string.Empty; string SeekDate = string.Empty;
                    CASEACTEntity CaseactRec = new CASEACTEntity();

                    ResAmount = 0; ChkNo = string.Empty; ChkDate = string.Empty; ServsDate = string.Empty; SeekDate = string.Empty; CaseactRec = new CASEACTEntity();
                    string strAmount = string.Empty;
                    List<CASEACTEntity> CaseactList = new List<CASEACTEntity>();
                    string strElec = "E";
                    if (SP_ElectricActivity_Details.Count > 0 || SP_OtherActivity_Details.Count>0)
                    {
                        //CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "1" && u.Elec_Other == strElec);
                        //if (CaseactList.Count > 0)
                        //{
                        //    strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        //}

                        //Y_Pos -= 20;
                        //X_Pos = 175;
                        //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        //if (SP_OtherActivity_Details.Count > 0)
                        //{
                        //    X_Pos = 240;
                        //    strAmount = string.Empty;
                        //    CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "1" && u.Elec_Other == "O");
                        //    if (CaseactList.Count > 0)
                        //    {
                        //        strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        //    }
                        //    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        //}


                        //X_Pos = 453;
                        //strAmount = string.Empty;
                        //CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == strElec);
                        //if (CaseactList.Count > 0)
                        //{
                        //    strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        //}
                        //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        //if (SP_OtherActivity_Details.Count > 0)
                        //{
                        //    X_Pos = 513;
                        //    strAmount = string.Empty;
                        //    CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == "O");
                        //    if (CaseactList.Count > 0)
                        //    {
                        //        strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        //    }
                        //    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        //}


                        //Y_Pos -= 20;
                        //X_Pos = 175;
                        //strAmount = string.Empty;
                        //CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "2" && u.Elec_Other == strElec);
                        //if (CaseactList.Count > 0)
                        //{
                        //    strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        //}
                        //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        //if (SP_OtherActivity_Details.Count > 0)
                        //{
                        //    X_Pos = 240;
                        //    strAmount = string.Empty;
                        //    CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "2" && u.Elec_Other == "O");
                        //    if (CaseactList.Count > 0)
                        //    {
                        //        strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        //    }
                        //    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        //}

                        //X_Pos = 453;
                        //strAmount = string.Empty;
                        //CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == strElec);
                        //if (CaseactList.Count > 0)
                        //{
                        //    strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        //}
                        //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        //if (SP_OtherActivity_Details.Count > 0)
                        //{
                        //    X_Pos = 513;
                        //    strAmount = string.Empty;
                        //    CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == "O");
                        //    if (CaseactList.Count > 0)
                        //    {
                        //        strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        //    }
                        //    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        //}

                        Y_Pos -= 20;
                        X_Pos = 297;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "3" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), 360, Y_Pos, 0);

                        //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), 425, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 362;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "3" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "3" );
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 24;
                        X_Pos = 297;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "4" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 362;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "4" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "4");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 24;
                        X_Pos = 297;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "5" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 362;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "5" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "5");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 24;
                        X_Pos = 297;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "6" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 362;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "6" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "6");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 23;
                        X_Pos = 297;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 362;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "7");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 23;
                        X_Pos = 297;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 362;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "8");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 23;
                        X_Pos = 297;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "9" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 362;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "9" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "9");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 23;
                        X_Pos = 297;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "10" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 362;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "10" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "10");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 23;
                        X_Pos = 297;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "11" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 362;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "11" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "11");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        Y_Pos -= 23;
                        X_Pos = 297;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "12" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            X_Pos = 362;
                            strAmount = string.Empty;
                            CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "12" && u.Elec_Other == "O");
                            if (CaseactList.Count > 0)
                            {
                                strAmount = Convert.ToDecimal(CaseactList[0].Cost).ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ACT_Date).Month.ToString() == "12");
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }

                        
                        List<CASEACTEntity> SPToatalDetails = SP_ElectricActivity_Details.FindAll(u => u.Cost != "");
                        decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                        Y_Pos -= 23;
                        X_Pos = 297;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(dectotalAmount.ToString("N2"), TblFontBold), X_Pos, Y_Pos, 0);
                        strVendor1Amount = dectotalAmount.ToString();

                        if (SP_OtherActivity_Details.Count > 0)
                        {
                            List<CASEACTEntity> SPOthToatalDetails = SP_OtherActivity_Details.FindAll(u => u.Cost != "");
                            decimal decOthtotalAmount = SPOthToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                            //Y_Pos -= 20;
                            X_Pos = 362;
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(decOthtotalAmount.ToString("N2"), TblFontBold), X_Pos, Y_Pos, 0);
                            strVendor2Amount = decOthtotalAmount.ToString();
                        }
                        if (SP_WaterActivity_Details.Count > 0)
                        {
                            X_Pos = 427;
                            strAmount = string.Empty;
                            CaseactList = SP_WaterActivity_Details.FindAll(u => u.Cost.Trim() != "" );
                            if (CaseactList.Count > 0)
                            {
                                decimal decAmount = CaseactList.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                                strAmount = decAmount.ToString("N2");
                            }
                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        }
                    }


                    Y_Pos = 75;//85; 97
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(CaseworkerName(BaseForm.BaseCaseMstListEntity[0].IntakeWorker.Trim()), TblFontBold), 110, Y_Pos, 0);



                }
                catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

                Hstamper.Close();

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
            else
            {
                CommonFunctions.MessageBoxDisplay("Benefit Service Plan Not Defined.");
            }
        }


        private void On_PCS_EligLetterSpanish()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            ReaderName = propReportPath + "\\" + "PCS_EligLet_SPAN.pdf";



            PdfName = "PCS_EligLet_SPAN";//form.GetFileName();
            if (SPMCode != string.Empty)
            {
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

                PdfReader Hreader = new PdfReader(ReaderName);

                PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
                Hstamper.Writer.SetPageSize(PageSize.A4);
                PdfContentByte cb = Hstamper.GetOverContent(1);


                BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
                iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
                BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
                iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
                BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

                iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 11);
                iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
                iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
                iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
                iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
                iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

                iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
                // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

                _image_Tick.ScalePercent(60f);
                //_image_Checked.ScalePercent(60f);

                Get_Vendor_List();

                Get_App_CASEACT_List();

                try
                {

                    string HN = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim())) HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + " ";
                    string Direction = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Direction.Trim())) Direction = BaseForm.BaseCaseMstListEntity[0].Direction.Trim() + " ";
                    string Street = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Street.Trim())) Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + " ";
                    string Suffix = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim())) Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + " ";
                    string Address = HN + Direction + Street + Suffix;


                    X_Pos = 60; Y_Pos = 760;

                    X_Pos = 117; Y_Pos -= 89;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 470;


                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].ApplNo, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 117; Y_Pos -= 17;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Address + " " + BaseForm.BaseCaseMstListEntity[0].City + " " + BaseForm.BaseCaseMstListEntity[0].State + " " + BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);

                    // X_Pos = 120; Y_Pos -= 14;
                    // ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].City + " " + BaseForm.BaseCaseMstListEntity[0].State + " " + BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 75; Y_Pos -= 20;





                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(LookupDataAccess.GetPhoneFormat(BaseForm.BaseCaseMstListEntity[0].Area + BaseForm.BaseCaseMstListEntity[0].Phone), TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 117; Y_Pos -= 92;

                    string strAccountNumber = string.Empty;
                    string strFirstVendorName = string.Empty;
                    string strVendor1Amount = string.Empty;
                    string strVendor2Amount = string.Empty;
                    if (SP_ElectricActivity_Details.Count > 0)
                    {
                        strAccountNumber = SP_ElectricActivity_Details[0].Account;
                        if (CaseVddlist.Count > 0)
                        {
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_ElectricActivity_Details[0].Vendor_No);
                            if (vddname != null)
                            {
                                strFirstVendorName = vddname.Name;
                            }
                        }
                    }
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strFirstVendorName, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 430;

                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAccountNumber, TblFontBold), X_Pos, Y_Pos, 0);


                    Y_Pos -= 22;






                    decimal ResAmount = 0; string ChkNo = string.Empty; string ChkDate = string.Empty; string ServsDate = string.Empty; string SeekDate = string.Empty;
                    CASEACTEntity CaseactRec = new CASEACTEntity();

                    ResAmount = 0; ChkNo = string.Empty; ChkDate = string.Empty; ServsDate = string.Empty; SeekDate = string.Empty; CaseactRec = new CASEACTEntity();
                    string strAmount = string.Empty;
                    List<CASEACTEntity> CaseactList = new List<CASEACTEntity>();
                    string strElec = "E";
                    if (SP_ElectricActivity_Details.Count > 0)
                    {
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "1" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }

                        Y_Pos -= 14;
                        X_Pos = 180;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 445;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 180;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "2" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 445;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 180;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "3" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 445;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "9" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 180;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "4" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 445;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "10" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 180;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "5" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        strAmount = string.Empty;
                        X_Pos = 445;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "11" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 180;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "6" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 445;
                        strAmount = string.Empty;
                        CaseactList = SP_ElectricActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "12" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        List<CASEACTEntity> SPToatalDetails = SP_ElectricActivity_Details.FindAll(u => u.Cost != "");
                        decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                        Y_Pos -= 14;
                        X_Pos = 445;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(dectotalAmount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                        strVendor1Amount = dectotalAmount.ToString();
                    }


                    string strsecondVendorName = string.Empty;

                    if (SP_OtherActivity_Details.Count > 0)
                    {


                        Y_Pos = 400;
                        strAccountNumber = string.Empty;
                        strsecondVendorName = string.Empty;

                        strAccountNumber = SP_OtherActivity_Details[0].Account;
                        if (CaseVddlist.Count > 0)
                        {
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_OtherActivity_Details[0].Vendor_No);
                            if (vddname != null)
                            {
                                strsecondVendorName = vddname.Name;
                            }
                        }

                        X_Pos = 117;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strsecondVendorName, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 430;

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAccountNumber, TblFontBold), X_Pos, Y_Pos, 0);

                    }
                    strElec = "O";
                    Y_Pos = 380;
                    if (SP_OtherActivity_Details.Count > 0)
                    {
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "1" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }

                        Y_Pos -= 14;
                        X_Pos = 180;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 445;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "7" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 180;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "2" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 445;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "8" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 180;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "3" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 445;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "9" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);


                        Y_Pos -= 14;
                        X_Pos = 180;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "4" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        X_Pos = 445;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "10" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 180;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "5" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }

                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        strAmount = string.Empty;
                        X_Pos = 445;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "11" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        Y_Pos -= 14;
                        X_Pos = 180;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "6" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);
                        X_Pos = 445;
                        strAmount = string.Empty;
                        CaseactList = SP_OtherActivity_Details.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == "12" && u.Elec_Other == strElec);
                        if (CaseactList.Count > 0)
                        {
                            strAmount = CaseactList[0].Cost;
                        }
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strAmount, TblFontBold), X_Pos, Y_Pos, 0);

                        List<CASEACTEntity> SPToatalDetails = SP_OtherActivity_Details.FindAll(u => u.Cost != "");
                        decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                        Y_Pos -= 14;
                        X_Pos = 445;
                        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(dectotalAmount.ToString(), TblFontBold), X_Pos, Y_Pos, 0);
                        strVendor2Amount = dectotalAmount.ToString();

                    }
                    Y_Pos = 225;
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strFirstVendorName.ToString(), TblFontBold), 117, Y_Pos, 0);
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strVendor1Amount.ToString(), TblFontBold), 430, Y_Pos, 0);

                    Y_Pos = 200;
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strsecondVendorName.ToString(), TblFontBold), 117, Y_Pos, 0);
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strVendor2Amount.ToString(), TblFontBold), 430, Y_Pos, 0);

                    Y_Pos = 90;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(LookupDataAccess.Getdate(DateTime.Now.Date.ToShortDateString()), TblFontBold), 430, Y_Pos, 0);


                }
                catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

                Hstamper.Close();

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
            else
            {
                CommonFunctions.MessageBoxDisplay("Benefit Service Plan Not Defined.");
            }
        }


        private void On_PCS_DeniedLetter()
        {
            Random_Filename = null;

            string ReaderName = string.Empty;

            ReaderName = propReportPath + "\\" + "PCS_Denial_ENGSPAN.pdf";



            PdfName = "PCS_Denial_ENGSPAN";//form.GetFileName();
            if (SPMCode != string.Empty)
            {
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

                PdfReader Hreader = new PdfReader(ReaderName);

                PdfStamper Hstamper = new PdfStamper(Hreader, new FileStream(PdfName, FileMode.Create, FileAccess.Write));
                Hstamper.Writer.SetPageSize(PageSize.A4);
                PdfContentByte cb = Hstamper.GetOverContent(1);


                BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
                iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
                BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
                iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
                BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

                iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 11);
                iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 12);
                iTextSharp.text.Font TableFontBoldItalicUnderline = new iTextSharp.text.Font(bf_times, 11, 7, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 11, 3, BaseColor.BLUE.Darker());
                iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);
                iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 10, 2);
                iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 10, 4);
                iTextSharp.text.Font TblFontBoldColor = new iTextSharp.text.Font(bf_times, 16, 7, BaseColor.BLUE.Darker());

                iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));
                // iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

                _image_Tick.ScalePercent(60f);
                //_image_Checked.ScalePercent(60f);

                Get_Vendor_List();

                Get_App_CASEACT_List();

                try
                {

                    string HN = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim())) HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + " ";
                    string Direction = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Direction.Trim())) Direction = BaseForm.BaseCaseMstListEntity[0].Direction.Trim() + " ";
                    string Street = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Street.Trim())) Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + " ";
                    string Suffix = string.Empty;
                    if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim())) Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + " ";
                    string Address = HN + Direction + Street + Suffix;


                    X_Pos = 60; Y_Pos = 760;

                    X_Pos = 75; Y_Pos -= 113;//110;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 470;


                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].ApplNo, TblFontBold), X_Pos, Y_Pos, 0);

                    X_Pos = 75; Y_Pos -= 24;//27
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Address + " " + BaseForm.BaseCaseMstListEntity[0].City + " " + BaseForm.BaseCaseMstListEntity[0].State + " " + BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);

                    // X_Pos = 120; Y_Pos -= 14;
                    // ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseCaseMstListEntity[0].City + " " + BaseForm.BaseCaseMstListEntity[0].State + " " + BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold), X_Pos, Y_Pos, 0);


                    X_Pos = 75; Y_Pos -= 20;

                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(LookupDataAccess.GetPhoneFormat(BaseForm.BaseCaseMstListEntity[0].Area + BaseForm.BaseCaseMstListEntity[0].Phone), TblFontBold), X_Pos, Y_Pos, 0);

                    //X_Pos = 40; Y_Pos -= 92;


                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("123", TblFontBold), X_Pos, Y_Pos, 0);




                    //X_Pos = 20; Y_Pos = 400;


                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Other", TblFontBold), X_Pos, Y_Pos, 0);



                    //X_Pos = 120; Y_Pos = 350;


                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Other REason222", TblFontBold), X_Pos, Y_Pos, 0);

                    //X_Pos = 120; Y_Pos -= 30;


                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Other REason1", TblFontBold), X_Pos, Y_Pos, 0);


                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Other", TblFontBold), X_Pos, Y_Pos, 0);



                    //X_Pos = 120; Y_Pos = -30;


                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Other REason222", TblFontBold), X_Pos, Y_Pos, 0);

                    //X_Pos = 120; Y_Pos -= 30;


                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Other REason1", TblFontBold), X_Pos, Y_Pos, 0);

                    //X_Pos = 430;


                    Y_Pos = 190;//240;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(LookupDataAccess.Getdate(DateTime.Now.Date.ToShortDateString()), TblFontBold), 430, Y_Pos, 0);


                }
                catch (Exception ex) { /*document.Add(new Paragraph("Aborted due to Exception............................................... "));*/ }

                Hstamper.Close();

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
            else
            {
                CommonFunctions.MessageBoxDisplay("Benefit Service Plan Not Defined.");
            }
        }

        

        List<CASEACTEntity> PropCaseactList = new List<CASEACTEntity>();
        List<CASEACTEntity> SP_Activity_Details = new List<CASEACTEntity>();
        List<CASEACTEntity> SP_ElectricActivity_Details = new List<CASEACTEntity>();
        List<CASEACTEntity> SP_OtherActivity_Details = new List<CASEACTEntity>();
        List<CASEACTEntity> SP_WaterActivity_Details = new List<CASEACTEntity>();
        private void Get_App_CASEACT_List()
        {

            CASEACTEntity Search_Enty = new CASEACTEntity(true);
            Search_Enty.Agency = BaseForm.BaseAgency;
            Search_Enty.Dept = BaseForm.BaseDept;
            Search_Enty.Program = BaseForm.BaseProg;
            Search_Enty.Year = BaseForm.BaseYear;
            Search_Enty.App_no = BaseForm.BaseApplicationNo;
            Search_Enty.SPM_Seq = SpmSeq;
            //Search_Enty.Service_plan = SPMCode;


            PropCaseactList = _model.SPAdminData.Browse_CASEACT(Search_Enty, "Browse", "PAYMENT");

            if (PropCaseactList.Count > 0)
                SP_Activity_Details = PropCaseactList.FindAll(u => u.ActSeek_Date.Trim() != string.Empty);

            if(SP_Activity_Details.Count>0)
                SP_Activity_Details = SP_Activity_Details.OrderByDescending(u => Convert.ToDateTime(u.ACT_Date.Trim())).ToList();

            SP_ElectricActivity_Details = SP_Activity_Details.FindAll(u => u.Elec_Other == "E" && u.Service_plan == SPMCode);

            SP_OtherActivity_Details = SP_Activity_Details.FindAll(u => u.Elec_Other == "O" && u.Service_plan == SPMCode);

            if(CEAPCNTL_List.Count>0)
            {
                SP_WaterActivity_Details = PropCaseactList.FindAll(u => (u.Service_plan == CEAPCNTL_List[0].CPCT_P1_SP && (u.ACT_Code.Trim() == CEAPCNTL_List[0].CPCT_P1_CA1.Trim() || u.ACT_Code.Trim() == CEAPCNTL_List[0].CPCT_P1_CA2.Trim()|| u.ACT_Code.Trim() == CEAPCNTL_List[0].CPCT_P1_CA3.Trim())) ||
                                            (u.Service_plan == CEAPCNTL_List[0].CPCT_P2_SP && (u.ACT_Code.Trim() == CEAPCNTL_List[0].CPCT_P2_CA1.Trim() || u.ACT_Code.Trim() == CEAPCNTL_List[0].CPCT_P2_CA2.Trim() || u.ACT_Code.Trim() == CEAPCNTL_List[0].CPCT_P2_CA3.Trim())) ||
                                            (u.Service_plan == CEAPCNTL_List[0].CPCT_P3_SP && (u.ACT_Code.Trim() == CEAPCNTL_List[0].CPCT_P3_CA1.Trim() || u.ACT_Code.Trim() == CEAPCNTL_List[0].CPCT_P3_CA2.Trim() || u.ACT_Code.Trim() == CEAPCNTL_List[0].CPCT_P3_CA3.Trim())));
            }

            if(SP_WaterActivity_Details.Count>0)
            {
                SP_WaterActivity_Details= SP_WaterActivity_Details.FindAll(u=>u.Cost.Trim()!=string.Empty);
            }

            //Search_Enty.App_no = string.Empty;
            //PropCaseactList = _model.SPAdminData.Browse_CASEACT(Search_Enty, "Browse", "PAYMENT");
            // if (PropCaseactList.Count > 0) PropCaseactList = PropCaseactList.FindAll(u => u.Elec_Other == StrElec);

            //if (PropCaseactList.Count > 0) txtRemTrans.Text = PropCaseactList.Count.ToString();

        }

        #endregion


        private void NCCAA_RFP()
        {
            Random_Filename = null;

            string Curr_Date = DateTime.Today.ToShortDateString();
            Curr_Date = Curr_Date.Replace("/", "");

            PdfName = "Fortworth_CEAP+ELIG";//form.GetFileName();
            PdfName = propReportPath + BaseForm.UserID + "\\" + PdfName;
            try
            {
                if (!Directory.Exists(propReportPath + BaseForm.UserID.Trim()))
                { DirectoryInfo di = Directory.CreateDirectory(propReportPath + BaseForm.UserID.Trim()); }
            }
            catch (Exception ex)
            {
                AlertBox.Show("Error", MessageBoxIcon.Error);
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

            //Document document = new Document();
            //document.SetPageSize(iTextSharp.text.PageSize.LETTER.Rotate());
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();
            //BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            //BaseFont bf_times_Check = BaseFont.CreateFont("c:/windows/fonts/WINGDNG2.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            //iTextSharp.text.Font Times_Check = new iTextSharp.text.Font(bf_times_Check, 10);
            BaseFont bf_times = BaseFont.CreateFont(Application.MapPath("~\\Fonts\\GOTHIC.TTF"), BaseFont.WINANSI, BaseFont.EMBEDDED);

            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 9);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 8);
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 8, 3);
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 9, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 8, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 9, 4);
            //iTextSharp.text.Font TableFontBold = new iTextSharp.text.Font(1, 12, 1);

            iTextSharp.text.Image _image_Tick = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Images\\tickmark_green.png"));

            _image_Tick.ScalePercent(60f);

            iTextSharp.text.Image FTWheaderjpg = iTextSharp.text.Image.GetInstance(propReportPath + "/Forthworth-01.png");
            FTWheaderjpg.ScalePercent(3f);

            var FontblcakColour = new BaseColor(13, 13, 13);
            var TableFont12Bold = FontFactory.GetFont("Calibri", 11, FontblcakColour);


            //var TblFontBold = FontFactory.GetFont("GOTHIC", 8,1, FontblcakColour);

            //var TableFont = FontFactory.GetFont("Century Gothic", 8, FontblcakColour);

            var FontblueColour = new BaseColor(31, 114, 186);//(109, 161, 206);
            var TableFontblue = FontFactory.GetFont("Calibri", 9, FontblueColour);

            var TableFont12underline = FontFactory.GetFont("Calibri", 8, 4, FontblcakColour);

            cb = writer.DirectContent;

            Get_Vendor_List();

            Get_App_CASEACT_List();

            try
            {
                string HN = string.Empty;
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim())) HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + " ";
                string Direction = string.Empty;
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Direction.Trim())) Direction = BaseForm.BaseCaseMstListEntity[0].Direction.Trim() + " ";
                string Street = string.Empty;
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Street.Trim())) Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + " ";
                string Suffix = string.Empty;
                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim())) Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + " ";
                string Address = HN + Direction + Street + Suffix;

                PdfPTable Table = new PdfPTable(1);
                Table.TotalWidth = 470f;
                Table.WidthPercentage = 100;
                Table.LockedWidth = true;
                float[] widths = new float[] { 100f };
                Table.SetWidths(widths);
                Table.HorizontalAlignment = Element.ALIGN_CENTER;
                //Table.SpacingBefore=

                PdfPCell H1 = new PdfPCell(FTWheaderjpg);
                H1.HorizontalAlignment = Element.ALIGN_CENTER;
                H1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Table.AddCell(H1);

                PdfPCell B0 = new PdfPCell(new Phrase("", TableFont));
                B0.HorizontalAlignment = Element.ALIGN_LEFT;
                B0.FixedHeight = 10f;
                B0.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Table.AddCell(B0);

                Phrase phrase = new Phrase(new Chunk("DATE: ", TableFont));
                phrase.Add(new Chunk(DateTime.Now.ToShortDateString(), TblFontBold));
                

                PdfPCell H2 = new PdfPCell(phrase);
                H2.HorizontalAlignment = Element.ALIGN_LEFT;
                H2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Table.AddCell(H2);

                PdfPCell B1 = new PdfPCell(new Phrase("",TableFont));
                B1.HorizontalAlignment = Element.ALIGN_LEFT;
                B1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Table.AddCell(B1);

                PdfPCell H3 = new PdfPCell(new Phrase("Applicant’s Name: "+ BaseForm.BaseApplicationName, TblFontBold));
                H3.HorizontalAlignment = Element.ALIGN_LEFT;
                H3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Table.AddCell(H3);

                //phrase.Clear();
                phrase = new Phrase(new Chunk("Address: ", TableFont));
                phrase.Add(new Chunk(Address, TblFontBold));


                PdfPCell H4 = new PdfPCell(phrase);
                H4.HorizontalAlignment = Element.ALIGN_LEFT;
                H4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Table.AddCell(H4);

                //phrase.Clear();
                phrase = new Phrase(new Chunk("Address 2: ", TableFont));
                phrase.Add(new Chunk(BaseForm.BaseCaseMstListEntity[0].City + "," + BaseForm.BaseCaseMstListEntity[0].State + "," + BaseForm.BaseCaseMstListEntity[0].Zip, TblFontBold));


                PdfPCell H5 = new PdfPCell(phrase);
                H5.HorizontalAlignment = Element.ALIGN_LEFT;
                H5.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Table.AddCell(H5);

                PdfPCell B2 = new PdfPCell(new Phrase("", TableFont));
                B2.HorizontalAlignment = Element.ALIGN_LEFT;
                B2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Table.AddCell(B2);

                string strAccountNumber = string.Empty;
                string strFirstVendorName = string.Empty;
                string strVendor1Amount = string.Empty;
                string strVendor2Amount = string.Empty; string strsecondVendorName = string.Empty;
                string FSource = string.Empty; string Smonth = string.Empty;
                if(PropCaseactList.Count>0)
                {
                    PropCaseactList=PropCaseactList.FindAll(u => u.Cost != "");
                    PropCaseactList = PropCaseactList.OrderBy(u => Convert.ToDateTime(u.ACT_Date.Trim())).ToList();
                    Smonth = Convert.ToDateTime(PropCaseactList[0].ACT_Date.Trim()).Month.ToString();
                    strVendor1Amount = PropCaseactList[0].Cost;
                    //List<CASEACTEntity> Selcaseacts = PropCaseactList.FindAll(u => u.Cost != "");
                    if (CaseVddlist.Count > 0)
                    {
                        CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == PropCaseactList[0].Vendor_No);
                        if (vddname != null)
                        {
                            strFirstVendorName = vddname.Name;
                            FSource = PropCaseactList[0].CA_Source.Trim();
                        }
                    }
                    List<CASEACTEntity> CaseactList = new List<CASEACTEntity>();
                    if (!string.IsNullOrEmpty(Smonth.Trim()))
                    {
                        CaseactList = PropCaseactList.FindAll(u => u.Cost.Trim() != "" && Convert.ToDateTime(u.ActSeek_Date).Month.ToString() == (int.Parse(Smonth)+1).ToString());
                        if (CaseactList.Count > 0)
                        {
                            strVendor2Amount = CaseactList[0].Cost;
                            CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == CaseactList[0].Vendor_No);
                            if (vddname != null)
                            {
                                strsecondVendorName = vddname.Name;
                                //FSource = PropCaseactList[0].CA_Source.Trim();
                            }
                        }
                    }

                }

                //if (SP_ElectricActivity_Details.Count > 0)
                //{
                //    strAccountNumber = SP_ElectricActivity_Details[0].Account;
                //    List<CASEACTEntity> SPToatalDetails = SP_ElectricActivity_Details.FindAll(u => u.Cost != "");
                //    decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                //    if (dectotalAmount > 0) strVendor1Amount = dectotalAmount.ToString();

                //    if (CaseVddlist.Count > 0)
                //    {
                //        CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_ElectricActivity_Details[0].Vendor_No);
                //        if (vddname != null)
                //        {
                //            strFirstVendorName = vddname.Name;
                //        }
                //    }
                //}

                //if (SP_OtherActivity_Details.Count > 0)
                //{
                //    //strAccountNumber = SP_OtherActivity_Details[0].Account;
                //    List<CASEACTEntity> SPToatalDetails = SP_OtherActivity_Details.FindAll(u => u.Cost != "");
                //    decimal dectotalAmount = SPToatalDetails.Sum(x => Convert.ToDecimal(x.Cost.Trim()));
                //    if (dectotalAmount > 0) strVendor2Amount = dectotalAmount.ToString();

                //    if (CaseVddlist.Count > 0)
                //    {
                //        CASEVDDEntity vddname = CaseVddlist.Find(u => u.Code == SP_ElectricActivity_Details[0].Vendor_No);
                //        if (vddname != null)
                //        {
                //            strsecondVendorName = vddname.Name;
                //        }
                //    }
                //}

                phrase = new Phrase(new Chunk("Your application is complete. You are eligible for Utility Assistance during the 2023 CEAP Program year. You received an ", TableFont));
                phrase.Add(new Chunk("initial pledge ", TblFontBold));
                phrase.Add(new Chunk("of ", TableFont));
                phrase.Add(new Chunk("$" + strVendor1Amount, TblFontBold));
                phrase.Add(new Chunk(" to ", TableFont));
                phrase.Add(new Chunk(strFirstVendorName, TblFontBold));
                if (!string.IsNullOrEmpty(strVendor2Amount.Trim()))
                {
                    phrase.Add(new Chunk(" and ", TableFont));
                    phrase.Add(new Chunk("$" + strVendor2Amount, TblFontBold));
                    phrase.Add(new Chunk(" to ", TableFont));
                    phrase.Add(new Chunk(strsecondVendorName, TblFontBold));
                }

                PdfPCell H6 = new PdfPCell(phrase);
                H6.HorizontalAlignment = Element.ALIGN_LEFT;
                H6.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Table.AddCell(H6);

                phrase = new Phrase(new Chunk("You will receive assistance for the following month(s) to your electric/gas/water provider(s) as follows:", TableFont));

                PdfPCell H7 = new PdfPCell(phrase);
                H7.HorizontalAlignment = Element.ALIGN_LEFT;
                H7.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Table.AddCell(H7);

                document.Add(Table);

            }
            catch(Exception ex) { }



           
            

            
            document.Close();
            fs.Close();
            fs.Dispose();
            //AlertBox.Show("Report Generated Successfully");
            if (BaseForm.BaseAgencyControlDetails.ReportSwitch.ToUpper() == "Y")
            {
                PdfViewerNewForm objfrm = new PdfViewerNewForm(PdfName);
                //objfrm.FormClosed += new Form.FormClosedEventHandler(On_Delete_PDF_File);
                objfrm.StartPosition = FormStartPosition.CenterScreen;
                objfrm.ShowDialog();
            }
            else
            {
                FrmViewer objfrm = new FrmViewer(PdfName);
                //objfrm.FormClosed += new Form.FormClosedEventHandler(On_Delete_PDF_File);
                objfrm.StartPosition = FormStartPosition.CenterScreen;
                objfrm.ShowDialog();
            }


        }



    }
}