
#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using Wisej.Web;
using Captain.Common.Model.Objects;
using Captain.Common.Views.Forms.Base;
using Captain.Common.Model.Data;
using System.Xml;
using System.IO;
using Captain.Common.Utilities;
using System.Text.RegularExpressions;
using System.Threading;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using XLSExportFile;
using CarlosAg.ExcelXmlWriter;
using Captain.Common.Views.Controls.Compatibility;
using static NPOI.HSSF.Util.HSSFColor;
using System.Security.Cryptography;
using ListItem = Captain.Common.Utilities.ListItem;
using DevExpress.XtraPrinting.Design;
using DevExpress.DataProcessing.InMemoryDataProcessor;

#endregion

namespace Captain.Common.Views.Forms
{
    public partial class RNGB0005Form : Form
    {
        #region private variables

        private ErrorProvider _errorProvider = null;
        CaptainModel _model = null;
        private List<HierarchyEntity> _selectedHierarchies = null;


        #endregion

        public RNGB0005Form(BaseForm baseForm, PrivilegeEntity privilegeEntity)
        {
            ListZipCode = new List<ZipCodeEntity>();
            ListRngGroupCode = new List<RCsb14GroupEntity>();
            ListcaseSiteEntity = new List<CaseSiteEntity>();
            ListcaseMsSiteEntity = new List<CaseSiteEntity>();
            Sel_Funding_List = new List<SPCommonEntity>();
            ListcommonEntity = new List<CommonEntity>();
            ListServicePlans = new List<CASESP1Entity>();

            InitializeComponent();
            BaseForm = baseForm;
            PrivilegeEntity = privilegeEntity;
            this.Text = "Program Service and Outcomes Report";
            //this.Text = privilegeEntity.Program + " - " + privilegeEntity.PrivilegeName;
            _model = new CaptainModel();


            //RngCodelist = _model.SPAdminData.Browse_RNGGrp(null, null, null, null, null,BaseForm.UserID,BaseForm.BaseAgency);
            Set_Report_Hierarchy(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear);
            ReportPath = _model.lookupDataAccess.GetReportPath();

            _errorProvider = new ErrorProvider(this);
            _errorProvider.BlinkRate = 1;
            _errorProvider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            _errorProvider.Icon = null;
            Txt_Pov_High.Validator = TextBoxValidation.IntegerValidator;
            Txt_Pov_Low.Validator = TextBoxValidation.IntegerValidator;
            fillDropDowns();
            propReportPath = _model.lookupDataAccess.GetReportPath();

            Get_MasterTable_DateRanges();
            Fill_CAMS_Master_List();
            Fill_All_List_Arrays();
            //Fill_Program_Combo();
            fillRngCode();
            Fill_Fund_Mast_List();


            //Initialize_All_Controls();
        }

        void fillRngCode()
        {
            cmbRngCode.Items.Clear();
            RngCodelist = _model.SPAdminData.Browse_RNGGrp(null, null, null, null, null, BaseForm.UserID, string.Empty);
            List<RCsb14GroupEntity> rngonlycodelist = RngCodelist.FindAll(u => u.GrpCode.Trim() == string.Empty && u.TblCode.Trim() == string.Empty);

            List<RCsb14GroupEntity> rngonlyAgencylist = rngonlycodelist.FindAll(u => u.Agency.Equals(strAgency));
            if (rngonlyAgencylist.Count == 0)
            {
                rngonlyAgencylist = rngonlycodelist.FindAll(u => u.Agency.Equals("**"));
            }

            if (rngonlyAgencylist.Count > 0)
                rngonlyAgencylist = rngonlyAgencylist.OrderByDescending(u => u.Code).ToList();

            foreach (RCsb14GroupEntity rngcodedata in rngonlyAgencylist)
            {
                cmbRngCode.Items.Add(new Captain.Common.Utilities.ListItem(rngcodedata.GrpDesc, rngcodedata.Code, rngcodedata.Agency,string.Empty, rngcodedata.OFdate,rngcodedata.OTdate));
            }
            cmbRngCode.Items.Insert(0, new Captain.Common.Utilities.ListItem("", "**"));
            cmbRngCode.SelectedIndex = 0;
        }

        void fillDropDowns()
        {
            Cmb_CaseType.Items.Clear();
            List<CommonEntity> CaseType = _model.lookupDataAccess.GetCaseType();
            CaseType = CaseType.OrderByDescending(u => u.Active.Trim()).ToList();
            Cmb_CaseType.ColorMember = "FavoriteColor";
            foreach (CommonEntity casetype in CaseType)
            {
                ListItem li = new ListItem(casetype.Desc, casetype.Code, casetype.Active, casetype.Active.Equals("Y") ? Color.Black : Color.Red);
                Cmb_CaseType.Items.Add(li);

                //Cmb_CaseType.Items.Add(new Captain.Common.Utilities.ListItem(casetype.Desc, casetype.Code));

                //if (casetype.Active != "Y")
                //    Cmb_CaseType.ForeColor = Color.Red;

            }
            Cmb_CaseType.Items.Insert(0, new Captain.Common.Utilities.ListItem("All", "**"));
            Cmb_CaseType.SelectedIndex = 0;

           /** Ref_From_Date.Value = new DateTime(DateTime.Now.Year, 1, 1);// Convert.ToDateTime("01/01/" + DateTime.Now.Year);
            Ref_To_Date.Value = new DateTime(DateTime.Now.Year, 12, 31);// Convert.ToDateTime("12/31/" + DateTime.Now.Year);
            if (Sys_DateRange_List.Count > 0)
            {
                Ref_From_Date.Value = Convert.ToDateTime(Sys_DateRange_List[0].REF_FDATE);// Convert.ToDateTime("01/01/" + DateTime.Now.Year);
                Ref_To_Date.Value = Convert.ToDateTime(Sys_DateRange_List[0].REF_TDATE);// Convert.ToDateTime("12/31/" + DateTime.Now.Year);
            }

            //Rep_From_Date.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            //Rep_To_Date.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
            Rep_From_Date.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            Rep_To_Date.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Month, DateTime.Today.Month));
            Rep_From_Date.Checked = Rep_To_Date.Checked = true;*/


        }


        #region properties

        public BaseForm BaseForm { get; set; }

        public PrivilegeEntity PrivilegeEntity { get; set; }

        public List<CaseSiteEntity> ListcaseSiteEntity { get; set; }

        public List<SPCommonEntity> Sel_Funding_List { get; set; }

        public List<ZipCodeEntity> ListZipCode { get; set; }

        public List<RCsb14GroupEntity> ListRngGroupCode { get; set; }

        public List<CASESP1Entity> ListServicePlans { get; set; }

        public List<CommonEntity> ListcommonEntity { get; set; }

        public List<CaseSiteEntity> ListcaseMsSiteEntity { get; set; }

        public string strAgency { get; set; }

        public string strDept { get; set; }

        public string strProg { get; set; }

        public string ReportPath { get; set; }

        public string propReportPath { get; set; }

        /**public List<HierarchyEntity> SelectedHierarchies
        {
            get
            {
                //return _selectedHierarchies = (from c in HierarchyGrid.Rows.Cast<DataGridViewRow>().ToList()
                //                               where (((DataGridViewCheckBoxCell)c.Cells["Select"]).Value.ToString().Equals(Consts.YesNoVariants.True, StringComparison.CurrentCultureIgnoreCase))
                //                               select ((DataGridViewRow)c).Tag as HierarchyEntity).ToList();

                return _selectedHierarchies = (from c in HierarchyGrid.Rows.Cast<DataGridViewRow>().ToList()
                                               select ((DataGridViewRow)c).Tag as HierarchyEntity).ToList();
            }
        }*/

        #endregion

        List<ZipCodeEntity> zipcode_List = new List<ZipCodeEntity>();
        List<CommonEntity> County_List = new List<CommonEntity>();
        List<CaseSiteEntity> Site_List = new List<CaseSiteEntity>();
        List<CaseSiteEntity> MSSite_List = new List<CaseSiteEntity>();
        List<RCsb14GroupEntity> RngCodelist = new List<RCsb14GroupEntity>();
        private void Fill_All_List_Arrays()
        {
            zipcode_List = _model.ZipCodeAndAgency.GetZipCodeSearch(string.Empty, string.Empty, string.Empty, string.Empty);
            County_List = _model.ZipCodeAndAgency.GetCounty();
            MSSite_List = Site_List = _model.CaseMstData.GetCaseSite(Current_Hierarchy.Substring(0, 2), Current_Hierarchy.Substring(2, 2), Current_Hierarchy.Substring(4, 2), "SiteHie");
        }

        /**private void rdoSelectedSites_Click(object sender, EventArgs e)
        {
            if (Rb_Site_Sel.Checked == true)
            {
                SelectZipSiteCountyForm siteform = new SelectZipSiteCountyForm(BaseForm, ListcaseSiteEntity, strAgency, strDept, strProg, string.Empty);
                siteform.FormClosed += new FormClosedEventHandler(SelectZipSiteCountyFormClosed);
                siteform.StartPosition = FormStartPosition.CenterScreen;
                siteform.ShowDialog();
            }
        }*/


        private void rdoCountySelected_Click(object sender, EventArgs e)
        {
            if (Rb_County_Sel.Checked == true)// && Scr_Oper_Mode == "CASB0004") // 20160303
            {
                SelectZipSiteCountyForm countyform = new SelectZipSiteCountyForm(BaseForm, ListcommonEntity);
                countyform.FormClosed += new FormClosedEventHandler(SelectZipSiteCountyFormClosed);
                countyform.StartPosition = FormStartPosition.CenterScreen;
                countyform.ShowDialog();
            }
        }

        private void rdoZipcodeSelected_Click(object sender, EventArgs e)
        {
            if (Rb_SP_Sel.Checked == true)
            {
                //if (((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString() == "**")
                //{
                //    AlertBox.Show("Please Select Code", MessageBoxIcon.Warning);
                //}
                //else
                //{

                    SelectZipSiteCountyForm zipcodeform1 = new SelectZipSiteCountyForm(BaseForm, ListServicePlans, Current_Hierarchy.Substring(0,2), Current_Hierarchy.Substring(2, 2), Current_Hierarchy.Substring(4, 2));
                    zipcodeform1.FormClosed += new FormClosedEventHandler(SelectZipSiteCountyFormClosed);
                    zipcodeform1.StartPosition = FormStartPosition.CenterScreen;
                    zipcodeform1.ShowDialog();
                //}

            }
        }

        private void SelectZipSiteCountyFormClosed(object sender, FormClosedEventArgs e)
        {

            SelectZipSiteCountyForm form = sender as SelectZipSiteCountyForm;
            if (form.DialogResult == DialogResult.OK)
            {
                if (form.FormType == "CASESITE")
                {
                    ListcaseSiteEntity = form.SelectedCaseSiteEntity;
                    /**if (Rb_Site_Sel.Checked == true && ListcaseSiteEntity.Count > 0)
                        Txt_Sel_Site.Text = ListcaseSiteEntity[0].SiteNUMBER.ToString();
                    else
                        Txt_Sel_Site.Clear();*/
                }
                else if (form.FormType == "COUNTY")
                {
                    ListcommonEntity = form.SelectedCountyEntity;
                }

                else if (form.FormType == "ZIPCODE")
                {
                    ListZipCode = form.SelectedZipcodeEntity;
                }
                else if (form.FormType == "MSCODE")
                {
                    ListRngGroupCode = form.SelectedRngGroupCodeEntity;
                }
                else if (form.FormType == "FUND")
                {
                    Sel_Funding_List = form.Get_Sel_Fund_List;
                }
                if (form.FormType == "CASESP")
                {
                    ListServicePlans = form.SelectedServicePlanEntity;
                    
                }

                //ZipCodeEntity zipcodedetais = form.ListOfSelectedCaseSite;
                //if (zipcodedetais != null)
                //{
                //    string zipPlus = zipcodedetais.Zcrplus4;
                //    txtZipPlus.Text = "0000".Substring(0, 4 - zipPlus.Length) + zipPlus;
                //    txtZipCode.Text = "00000".Substring(0, 5 - zipcodedetais.Zcrzip.Length) + zipcodedetais.Zcrzip;
                //    txtState.Text = zipcodedetais.Zcrstate;
                //    txtCity.Text = zipcodedetais.Zcrcity;
                //    SetComboBoxValue(cmbCounty, zipcodedetais.Zcrcountry);
                //    SetComboBoxValue(cmbTownship, zipcodedetais.Zcrcitycode);

                //}
            }
        }

       /** private void SelectZipSiteCountyMSFormClosed(object sender, FormClosedEventArgs e)
        {

            SelectZipSiteCountyForm form = sender as SelectZipSiteCountyForm;
            if (form.DialogResult == DialogResult.OK)
            {
                if (form.FormType == "CASESITE")
                {
                    ListcaseMsSiteEntity = form.SelectedCaseSiteEntity;
                    if (rdoMsselectsite.Checked == true && ListcaseMsSiteEntity.Count > 0)
                        txt_Msselect_site.Text = ListcaseMsSiteEntity[0].SiteNUMBER.ToString();
                    else
                        txt_Msselect_site.Clear();
                }
                else if (form.FormType == "COUNTY")
                {
                    ListcommonEntity = form.SelectedCountyEntity;
                }

                else if (form.FormType == "ZIPCODE")
                {
                    ListZipCode = form.SelectedZipcodeEntity;
                }
                else if (form.FormType == "MSCODE")
                {
                    ListRngGroupCode = form.SelectedRngGroupCodeEntity;
                }
                else if (form.FormType == "FUND")
                {
                    Sel_Funding_List = form.Get_Sel_Fund_List;
                }

                //ZipCodeEntity zipcodedetais = form.ListOfSelectedCaseSite;
                //if (zipcodedetais != null)
                //{
                //    string zipPlus = zipcodedetais.Zcrplus4;
                //    txtZipPlus.Text = "0000".Substring(0, 4 - zipPlus.Length) + zipPlus;
                //    txtZipCode.Text = "00000".Substring(0, 5 - zipcodedetais.Zcrzip.Length) + zipcodedetais.Zcrzip;
                //    txtState.Text = zipcodedetais.Zcrstate;
                //    txtCity.Text = zipcodedetais.Zcrcity;
                //    SetComboBoxValue(cmbCounty, zipcodedetais.Zcrcountry);
                //    SetComboBoxValue(cmbTownship, zipcodedetais.Zcrcitycode);

                //}
            }
        }*/



        string Current_Hierarchy = "******", Current_Hierarchy_DB = "**-**-**";
        string DeptName = string.Empty; string ProgName = string.Empty;
        private void Set_Report_Hierarchy(string Agy, string Dept, string Prog, string Year)
        {
            txtHieDesc.Clear();
            Current_Hierarchy = Agy + Dept + Prog;
            Current_Hierarchy_DB = Agy + "-" + Dept + "-" + Prog;
            strAgency = Agy;
            strDept = Dept;
            strProg = Prog;

            if (Agy != "**")
            {
                DataSet ds_AGY = Captain.DatabaseLayer.AgyTab.GetHierarchyNames(Agy, "**", "**");
                if (ds_AGY.Tables.Count > 0)
                {
                    if (ds_AGY.Tables[0].Rows.Count > 0)
                        txtHieDesc.Text += "AGY : " + Agy + " - " + (ds_AGY.Tables[0].Rows[0]["HIE_NAME"].ToString()).Trim() + "      ";
                }
            }
            else
                txtHieDesc.Text += "AGY : ** - All Agencies      ";

            if (Dept != "**")
            {
                DataSet ds_DEPT = Captain.DatabaseLayer.AgyTab.GetHierarchyNames(Agy, Dept, "**");
                if (ds_DEPT.Tables.Count > 0)
                {
                    if (ds_DEPT.Tables[0].Rows.Count > 0)
                    {
                        txtHieDesc.Text += "DEPT : " + Dept + " - " + (ds_DEPT.Tables[0].Rows[0]["HIE_NAME"].ToString()).Trim() + "      ";
                        DeptName = ds_DEPT.Tables[0].Rows[0]["HIE_NAME"].ToString().Trim() + "/";
                    }
                }
            }
            else
            {
                txtHieDesc.Text += "DEPT : ** - All Departments      ";
                DeptName = "All Departments /";
            }

            if (Prog != "**")
            {
                DataSet ds_PROG = Captain.DatabaseLayer.AgyTab.GetHierarchyNames(Agy, Dept, Prog);
                if (ds_PROG.Tables.Count > 0)
                {
                    if (ds_PROG.Tables[0].Rows.Count > 0)
                    {
                        txtHieDesc.Text += "PROG : " + Prog + " - " + (ds_PROG.Tables[0].Rows[0]["HIE_NAME"].ToString()).Trim();
                        ProgName = ds_PROG.Tables[0].Rows[0]["HIE_NAME"].ToString().Trim();
                    }
                }
            }
            else
            {
                txtHieDesc.Text += "PROG : ** - All Programs ";
                ProgName = "All Programs ";
            }


            //if (Agy != "**")
            //    Get_NameFormat_For_Agencirs(Agy);
            //else
            //    Member_NameFormat = CAseWorkerr_NameFormat = "1";

            if (Agy != "**" && Dept != "**" && Prog != "**")
                FillYearCombo(Agy, Dept, Prog, Year);
            else
            {
                CmbYear.Visible = false;
                //this.txtHieDesc.Size = new System.Drawing.Size(580, 23);
                this.txtHieDesc.Size = new System.Drawing.Size(666/*810*/, 25);
            }

        }

        List<SPCommonEntity> Fund_Mast_List = new List<SPCommonEntity>();
        private void Fill_Fund_Mast_List()
        {
            Fund_Mast_List = _model.SPAdminData.Get_AgyRecs("Funding");
            //if (Entity_List != null && Entity_List.Count > 0)
            //{
            //    FundingList.ForEach(item => item.Active = (Entity_List.Exists(u => u.Code.Equals(item.Code))) ? "true" : "false");
            //}
        }


        string DepYear, Program_Year;
        bool DefHieExist = false;
        private void FillYearCombo(string Agy, string Dept, string Prog, string Year)
        {
            CmbYear.Visible = DefHieExist = false;
            Program_Year = "    ";
            if (!string.IsNullOrEmpty(Year.Trim()))
                DefHieExist = true;

            DataSet ds = Captain.DatabaseLayer.MainMenu.GetCaseDepForHierarchy(Agy, Dept, Prog);
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                int YearIndex = 0;

                if (dt.Rows.Count > 0)
                {
                    Program_Year = DepYear = dt.Rows[0]["DEP_YEAR"].ToString();
                    if (!(String.IsNullOrEmpty(DepYear.Trim())) && DepYear != null && DepYear != "    ")
                    {
                        int TmpYear = int.Parse(DepYear);
                        int TempCompareYear = 0;
                        string TmpYearStr = null;
                        if (!(String.IsNullOrEmpty(Year)) && Year != null && Year != " " && DefHieExist)
                            TempCompareYear = int.Parse(Year);
                        List<Captain.Common.Utilities.ListItem> listItem = new List<Captain.Common.Utilities.ListItem>();
                        for (int i = 0; i < 10; i++)
                        {
                            TmpYearStr = (TmpYear - i).ToString();
                            //listItem.Add(new ListItem(TmpYearStr, i));
                            listItem.Add(new Captain.Common.Utilities.ListItem(TmpYearStr, TmpYearStr));
                            if (TempCompareYear == (TmpYear - i) && TmpYear != 0 && TempCompareYear != 0)
                                YearIndex = i;
                        }

                        CmbYear.Items.AddRange(listItem.ToArray());

                        CmbYear.Visible = true;

                        if (DefHieExist)
                            CmbYear.SelectedIndex = YearIndex;
                        else
                            CmbYear.SelectedIndex = 0;
                    }
                }
            }

            if (!string.IsNullOrEmpty(Program_Year.Trim()))
            {
                //Current_Hierarchy = Current_Hierarchy + Program_Year;
                this.txtHieDesc.Size = new System.Drawing.Size(601/*753*/, 25);
            }
            else
                this.txtHieDesc.Size = new System.Drawing.Size(666/*810*/, 25);
        }


        private void Pb_Search_Hie_Click(object sender, EventArgs e)
        {

            ////HierarchieSelectionForm hierarchieSelectionForm = new HierarchieSelectionForm(BaseForm, "Master", Current_Hierarchy_DB, string.Empty, "Reports");
            ////hierarchieSelectionForm.FormClosed += new Form.FormClosedEventHandler(OnHierarchieFormClosed);
            ////hierarchieSelectionForm.ShowDialog();

            //HierarchieSelectionFormNew hierarchieSelectionForm = new HierarchieSelectionFormNew(BaseForm, Current_Hierarchy_DB, "Master", "A", "*", "Reports");
            //hierarchieSelectionForm.FormClosed += new FormClosedEventHandler(OnHierarchieFormClosed);
            //hierarchieSelectionForm.StartPosition = FormStartPosition.CenterScreen;
            //hierarchieSelectionForm.ShowDialog();

            HierarchieSelection hierarchieSelectionForm = new HierarchieSelection(BaseForm, Current_Hierarchy_DB, "Master", "A", "*", "Reports", BaseForm.UserID);
            hierarchieSelectionForm.FormClosed += new FormClosedEventHandler(OnHierarchieFormClosed);
            hierarchieSelectionForm.StartPosition = FormStartPosition.CenterScreen;
            hierarchieSelectionForm.ShowDialog();

        }


        private void OnHierarchieFormClosed(object sender, FormClosedEventArgs e)
        {
            //HierarchieSelectionFormNew form = sender as HierarchieSelectionFormNew;
            HierarchieSelection form = sender as HierarchieSelection;

            if (form.DialogResult == DialogResult.OK)
            {
                List<HierarchyEntity> selectedHierarchies = form.SelectedHierarchies;
                string hierarchy = string.Empty;

                if (selectedHierarchies.Count > 0)
                {
                    foreach (HierarchyEntity row in selectedHierarchies)
                    {
                        hierarchy += (string.IsNullOrEmpty(row.Agency) ? "**" : row.Agency) + (string.IsNullOrEmpty(row.Dept) ? "**" : row.Dept) + (string.IsNullOrEmpty(row.Prog) ? "**" : row.Prog);
                    }
                    Current_Hierarchy = hierarchy.Substring(0, 2) + hierarchy.Substring(2, 2) + hierarchy.Substring(4, 2);

                    Set_Report_Hierarchy(hierarchy.Substring(0, 2), hierarchy.Substring(2, 2), hierarchy.Substring(4, 2), string.Empty);
                    //Initialize_All_Controls();
                    //Fill_Program_Combo();
                   //** rbAllPrograms.Checked = true;
                    fillRngCode();
                }
            }
        }


        private void btnRepMaintPreview_Click(object sender, EventArgs e)
        {
            //string myPath = Context.Server.MapPath("~\\Resources\\Excel\\Sample.xlsx"); //(@"C:\sample.xlsx");
            //if (File.Exists(myPath))
            //{
            //    //excelApp.Workbooks.Open(myPath);
            //    //excelApp.Visible = true;
            //    FrmViewer objfrm = new FrmViewer(myPath);
            //    objfrm.Size = new System.Drawing.Size(100,40);
            //    objfrm.FormBorderStyle = Wisej.Web.FormBorderStyle.FixedDialog;
            //    objfrm.ShowDialog();

            //}

            PdfListForm pdfListForm = new PdfListForm(BaseForm, PrivilegeEntity, true, propReportPath);
            pdfListForm.StartPosition = FormStartPosition.CenterScreen;
            pdfListForm.ShowDialog();

        }

        private void CASB0004Form_Load(object sender, EventArgs e)
        {
            Cmb_CaseType.Focus();
        }


        List<DG_ResTab_Entity> PM_Detail_Table_List = new List<DG_ResTab_Entity>();
        private void Get_PM_Detail_Table_Structure()
        {
            //Declare @Detail_Cum_Table Table(SortUnDup_Group VarChar(10), SortUnDup_Table VarChar(10), SortUnDup_Agy Char(2), SortUnDup_Dept Char(2), SortUnDup_Prog Char(2), 
            //                                SortUnDup_Year VarChar(4), SortUnDup_App VarChar(8), SortUnDup_Fam_ID VarChar(9), SortUnDup_Client_ID Decimal(9),
            //                                SortUnDup_OutcomeCode Varchar(10), SortUnDup_OutCome_Date Date, SortUnDup_Count_Indicator Char(1), SortUnDup_Result Varchar(4), SortUnDup_Name Varchar(90), 
            //                                R1 Int Default 0, R2 Int Default 0, R3 Int Default 0, R4 Int Default 0, R5 Int Default 0)


            PM_Detail_Table_List.Clear();
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_Group", "", "L", "1in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_Table", "", "L", "1in"));
            //PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_Agy", "", "L", "1.5in"));
            //Commented by Sudheer on 02/04/2021
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_Agy", "", "L", "5.5in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_Dept", "", "L", "3.2in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_Prog", "", "L", "3.2in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_Year", "", "L", "3.2in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_App", "", "L", ".95in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_Fam_ID", "", "L", "3.2in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_Client_ID", "", "L", "3.2in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_Name", "", "L", "2.5in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_Count_Indicator", "", "L", "3.2in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_Result", "", "L", "3.2in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "R1", "", "R", ".65in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "R2", "", "R", ".65in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "R3", "", "R", ".65in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "R4", "", "R", ".65in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "R5", "", "R", ".65in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_UserId", "", "L", "2.5in"));
           /** if (rdoperiodBoth.Checked)
            {
                PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_OutCome_Date", "", "R", ".95in"));
                PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_RefCount", "", "R", "1.5in"));
                PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_RefDate", "", "R", "1in"));
                PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_RepCount", "", "R", "1in"));
                PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_RepDate", "", "R", "1in"));

            }
            else
            {
                PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_OutCome_Date", "", "R", ".95in"));
                PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_RefCount", "", "R", ".95in"));
                PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_RefDate", "", "R", ".95in"));
                PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_RepCount", "", "R", ".95in"));
                PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_RepDate", "", "R", ".95in"));

            }*/
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_OutcomeCode", "", "L", "0.8in"));
            //PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_RePcount", "", "L", "1in"));
            //PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_Refcount", "", "L", "1in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("N", "SortUnDup_Group_Desc", "", "R", ".85in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_Goal_Desc", "", "L", "3.5in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_Isite", "", "L", "1.5in"));
            PM_Detail_Table_List.Add(new DG_ResTab_Entity("Y", "SortUnDup_Psite", "", "L", "1.5in"));

        }

        DataTable dt;
     

        #region PdfReportCode




        //*****************PerformanceMeasures_Details_Dynamic_RDLC***********************************************************************************

        


        PdfContentByte cb;
        int X_Pos, Y_Pos;
        string Random_Filename = null;
        BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
        string strReasonCodes = string.Empty;
        string PdfName;
       /**ivate void On_SaveForm_Closed(DataTable dtResult, DataTable dtResultBoth)
        {

            strReasonCodes = string.Empty;

            PdfName = "RNGB0014_MainReport";

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

            Document document = new Document(PageSize.A4, 15, 15, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();
            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);

            BaseFont bf_Times = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1250, false);
            cb = writer.DirectContent;

            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, false);
            BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1250, false);
            iTextSharp.text.Font fc = new iTextSharp.text.Font(bfTimes, 12, 2);
            iTextSharp.text.Font fc1 = new iTextSharp.text.Font(bfTimes, 8, 2, BaseColor.BLUE);
            iTextSharp.text.Font fcRed = new iTextSharp.text.Font(bfTimes, 10, 1, BaseColor.RED);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bfTimes, 7);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bfTimes, 8);
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bfTimes, 8, 3);
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 7, 1);
            iTextSharp.text.Font TblFontHeadBold = new iTextSharp.text.Font(1, 12, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bfTimes, 8, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bfTimes, 9, 4);

            HeaderPage(document, writer);
            try
            {
                document.SetPageSize(iTextSharp.text.PageSize.LETTER.Rotate());

                document.NewPage();

                //PdfPTable table = new PdfPTable(6);


                // table.FooterRows = 3;

                List<CommonEntity> commonHeaderlist = new List<CommonEntity>();

                //cb.BeginText();
                ////cb.SetFontAndSize(FontFactory.GetFont(FontFactory.TIMES_ROMAN).BaseFont, 12);
                //cb.SetFontAndSize(bfTimes, 12);

                ////List<CSB4AsocEntity> CSb4Entity;
                ////CSb4Entity = _model.SPAdminData.Browse_CSB4Assoc(string.Empty, string.Empty);
                //DataSet dscategories = DatabaseLayer.SPAdminDB.Get_RNG4CATG();

                //cb.SetFontAndSize(bfTimesBold, 10);
                //cb.SetRGBColorFill(4, 4, 15);
                //X_Pos = 300; Y_Pos = 785;
                //cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Module 4, Section A: Individual and Family National Performance Indicators (NPIs) - Data Entry Form", X_Pos, Y_Pos, 0);
                //Y_Pos -= 15;
                //cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Goal 1: Individuals and Families with low incomes are stable and achieve economic security. ", X_Pos, Y_Pos, 0);
                //Y_Pos -= 15;
                //cb.EndText();

                // CaptainModel _model = new CaptainModel();

                List<RCsb14GroupEntity> RngGrouplist = new List<RCsb14GroupEntity>();
                List<RCsb14GroupEntity> RngtblCodelist = new List<RCsb14GroupEntity>();
                List<RCsb14GroupEntity> RngResultlist = new List<RCsb14GroupEntity>();

                List<RCsb14GroupEntity> RngBothResultlist = new List<RCsb14GroupEntity>();

                if (rdoperiodBoth.Checked) //&& Rb_SNP_Mem.Checked)
                {
                    foreach (DataRow drResultBothitem in dtResultBoth.Rows)
                    {
                        RngBothResultlist.Add(new RCsb14GroupEntity(drResultBothitem, ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), string.Empty));
                    }
                }

                foreach (DataRow drResultitem in dtResult.Rows)
                {
                    RngResultlist.Add(new RCsb14GroupEntity(drResultitem, ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), string.Empty));
                }

                List<RNGGAEntity> GoalDetailsEntity = _model.SPAdminData.Browse_RNGGA(((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).ID.ToString(), string.Empty, string.Empty, string.Empty);
                if (RngCodelist.Count > 0)
                {
                    RngGrouplist = RngCodelist.FindAll(u => u.GrpCode.Trim() != string.Empty && u.TblCode.Trim() == string.Empty && u.Agency == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).ID.ToString() && u.Code == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString());
                    RngtblCodelist = RngCodelist.FindAll(u => u.GrpCode.Trim() != string.Empty && u.TblCode.Trim() != string.Empty && u.Agency == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).ID.ToString() && u.Code == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString());
                }
                if (rdoOutcomesselect.Checked)
                {

                    List<RCsb14GroupEntity> rngindswithclist = RngtblCodelist.FindAll(u => u.IndSwitch == "Y");
                    foreach (RCsb14GroupEntity itemindswitch in rngindswithclist)
                    {
                        List<RCsb14GroupEntity> rngtblcodelist = RngResultlist.FindAll(u => u.TblCode == itemindswitch.TblCode);
                        if (rngtblcodelist.Count == 0)
                        {
                            List<RNGGAEntity> GoalDetailsIndswitchEntity = GoalDetailsEntity.FindAll(u => u.TblCode.Trim() == itemindswitch.TblCode.Trim() && u.GrpCode.Trim() == itemindswitch.GrpCode.Trim() && u.Code.Trim() == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString().Trim());
                            foreach (RNGGAEntity itemGoalind in GoalDetailsIndswitchEntity)
                            {
                                RngResultlist.Add(new RCsb14GroupEntity(((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), itemindswitch.GrpCode, itemindswitch.TblCode, itemGoalind.Desc, "0", "0", "0", "0", "0", itemindswitch.unit_cnt, itemindswitch.ExAchev, itemindswitch.per_Achived, itemindswitch.CalCost, itemindswitch.Count_type, "INDSWITCH"));
                            }

                        }
                        if (rdoperiodBoth.Checked) //&& Rb_SNP_Mem.Checked)
                        {
                            List<RCsb14GroupEntity> rngtblbothcodelist = RngBothResultlist.FindAll(u => u.TblCode == itemindswitch.TblCode);
                            if (rngtblcodelist.Count == 0)
                            {
                                List<RNGGAEntity> GoalDetailsIndswitchEntity = GoalDetailsEntity.FindAll(u => u.TblCode.Trim() == itemindswitch.TblCode.Trim() && u.GrpCode.Trim() == itemindswitch.GrpCode.Trim() && u.Code.Trim() == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString().Trim());
                                foreach (RNGGAEntity itemGoalind in GoalDetailsIndswitchEntity)
                                {
                                    RngBothResultlist.Add(new RCsb14GroupEntity(((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), itemindswitch.GrpCode, itemindswitch.TblCode, itemGoalind.Desc, "0", "0", "0", "0", "0", itemindswitch.unit_cnt, itemindswitch.ExAchev, itemindswitch.per_Achived, itemindswitch.CalCost, itemindswitch.Count_type, "INDSWITCH"));
                                }
                            }
                        }
                    }
                }



                bool boolGroupFilter = true;
                bool boolGoalAssoFilter = true;

                foreach (RCsb14GroupEntity codeitem in RngGrouplist)
                {

                    if (Rb_User_Def.Checked)
                    {
                        boolGoalAssoFilter = false;
                        if (GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == codeitem.GrpCode.Trim()).Count > 0)
                            boolGoalAssoFilter = true;
                    }
                    if (Rb_SP_Sel.Checked)
                    {
                        boolGroupFilter = false;
                        if (ListRngGroupCode.FindAll(u => u.GrpCode.Trim() == codeitem.GrpCode.Trim()).Count > 0)
                            boolGroupFilter = true;
                    }
                    string Agy = string.Empty;
                    if (Current_Hierarchy.Substring(0, 2).ToString() != "**")
                        Agy = Current_Hierarchy.Substring(0, 2);
                    string Agency = string.Empty;
                    DataSet dsAgency = new DataSet();
                    if (!string.IsNullOrEmpty(Agy.Trim()))
                    {
                        dsAgency = Captain.DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(Agy, null, null, null, null, null, null);
                        if (dsAgency != null && dsAgency.Tables[0].Rows.Count > 0)
                            Agency = dsAgency.Tables[0].Rows[0]["ACR_NAME"].ToString().Trim();
                    }

                    if ((boolGroupFilter) && (boolGoalAssoFilter))
                    {
                        string strResultColumn = codeitem.ExAchev.ToString();
                        //PdfPCell pdfMainHeader = new PdfPCell(new Phrase("Module 4, Section A: Individual and Family National Performance Indicators (NPIs) - Data Entry Form", TblFontHeadBold));
                        //pdfMainHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                        //pdfMainHeader.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        //pdfMainHeader.Colspan = 6;
                        //table.AddCell(pdfMainHeader);

                        //PdfPCell pdfMainHeader2 = new PdfPCell(new Phrase("Goal 1: Individuals and Families with low incomes are stable and achieve economic security. ", TblFontHeadBold));
                        //pdfMainHeader2.HorizontalAlignment = Element.ALIGN_CENTER;
                        //pdfMainHeader2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        //pdfMainHeader2.Colspan = 6;
                        //table.AddCell(pdfMainHeader2);

                        PdfPTable table = new PdfPTable(6);
                        table.WidthPercentage = 100;
                        table.LockedWidth = true;

                        table.TotalWidth = 750f;

                        float[] widths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f };
                        table.SetWidths(widths);


                        table.HeaderRows = 2;
                        table.HorizontalAlignment = Element.ALIGN_CENTER;



                        PdfPCell pdfMainSubHeader = new PdfPCell(new Phrase(codeitem.GrpDesc, fcRed));//+ " Indicators"
                        pdfMainSubHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfMainSubHeader.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        pdfMainSubHeader.Colspan = 6;
                        table.AddCell(pdfMainSubHeader);


                        //PdfPCell pdfMainSubHeader1 = new PdfPCell(new Phrase("Name of CSBG Eligible Entity Reporting: __________________________________", TableFont));
                        //pdfMainSubHeader1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        //pdfMainSubHeader1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        //pdfMainSubHeader1.Colspan = 6;
                        //table.AddCell(pdfMainSubHeader1);

                        //Added by Sudheer on 06/30/2018
                        PdfPCell pdfMainSubHeader1 = new PdfPCell(new Phrase("Department/Program: " + DeptName + ProgName, TableFont));
                        pdfMainSubHeader1.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfMainSubHeader1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        pdfMainSubHeader1.Colspan = 6;
                        table.AddCell(pdfMainSubHeader1);



                        int intcolumns = 6;
                        int intheadercount = 0;
                        if (codeitem.Incld1.ToString().ToUpper() == "TRUE")
                            intheadercount = intheadercount + 1;
                        if (codeitem.Incld2.ToString().ToUpper() == "TRUE")
                            intheadercount = intheadercount + 1;
                        if (codeitem.Incld3.ToString().ToUpper() == "TRUE")
                            intheadercount = intheadercount + 1;
                        if (codeitem.Incld4.ToString().ToUpper() == "TRUE")
                            intheadercount = intheadercount + 1;
                        if (codeitem.Incld5.ToString().ToUpper() == "TRUE")
                            intheadercount = intheadercount + 1;
                        if (strResultColumn == "0")
                            intheadercount = intheadercount + 1;
                        if (intheadercount > 1)
                            intcolumns = intcolumns + (intheadercount - 1);

                        PdfPTable subtable = new PdfPTable(intcolumns);

                        subtable.WidthPercentage = 100;
                        subtable.LockedWidth = true;

                        subtable.TotalWidth = 750f;

                        float[] subwidths = null;
                        if (intcolumns == 6)
                            subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f };
                        if (intcolumns == 7)
                            subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f, 60f };
                        if (intcolumns == 8)
                            subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f, 60f, 60f };
                        if (intcolumns == 9)
                            subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f };
                        if (intcolumns == 10)
                            subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f };
                        if (intcolumns == 11)
                            subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f };

                        subtable.SetWidths(subwidths);

                        PdfPCell pdfMainSubHeader2 = new PdfPCell(new Phrase("Name of CSBG Eligible Entity Reporting:  ", TblFontBold));
                        pdfMainSubHeader2.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfMainSubHeader2.Border = iTextSharp.text.Rectangle.BOX;
                        //pdfMainSubHeader2.Colspan = 6;
                        subtable.AddCell(pdfMainSubHeader2);

                        PdfPCell pdfMainSubHeader3 = new PdfPCell(new Phrase(Agency, TableFont));
                        pdfMainSubHeader3.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfMainSubHeader3.Border = iTextSharp.text.Rectangle.BOX;
                        pdfMainSubHeader3.Colspan = intcolumns - 1;
                        subtable.AddCell(pdfMainSubHeader3);

                        PdfPCell pdfMainSubHeader4 = new PdfPCell(new Phrase("State:  ", TblFontBold));
                        pdfMainSubHeader4.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfMainSubHeader4.Border = iTextSharp.text.Rectangle.BOX;
                        //pdfMainSubHeader2.Colspan = 6;
                        subtable.AddCell(pdfMainSubHeader4);

                        PdfPCell pdfMainSubHeader5 = new PdfPCell(new Phrase("DUNS: ", TblFontBold));
                        pdfMainSubHeader5.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfMainSubHeader5.Border = iTextSharp.text.Rectangle.BOX;
                        pdfMainSubHeader5.Colspan = intcolumns - 1;
                        subtable.AddCell(pdfMainSubHeader5);

                        PdfPCell pdfMainSubHeaderSpace = new PdfPCell(new Phrase("", TableFont));
                        pdfMainSubHeaderSpace.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pdfMainSubHeaderSpace.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        pdfMainSubHeaderSpace.Colspan = intcolumns;
                        subtable.AddCell(pdfMainSubHeaderSpace);

                        PdfPCell pdfMainTableHeader1 = new PdfPCell(new Phrase("", TblFontBold));
                        pdfMainTableHeader1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pdfMainTableHeader1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableHeader1.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER;
                        subtable.AddCell(pdfMainTableHeader1);

                        PdfPCell pdfMainTableHeader2 = new PdfPCell(new Phrase("I.) Number of Individuals Served ", TblFontBold));
                        pdfMainTableHeader2.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfMainTableHeader2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableHeader2.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER;
                        subtable.AddCell(pdfMainTableHeader2);


                        PdfPCell pdfMainTableHeader3 = new PdfPCell(new Phrase("II.) Target (#)", TblFontBold));
                        pdfMainTableHeader3.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfMainTableHeader3.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableHeader3.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER;
                        subtable.AddCell(pdfMainTableHeader3);

                        PdfPCell pdfMainTableHeader4 = new PdfPCell(new Phrase("III.) Actual Results (#) ", TblFontBold));
                        pdfMainTableHeader4.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfMainTableHeader4.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableHeader4.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER;
                        subtable.AddCell(pdfMainTableHeader4);

                        int intcolumnscount = 3;
                        if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                        {
                            intcolumnscount = intcolumnscount + 1;
                            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(HeadersValue(intcolumnscount) + codeitem.Hrd1, TblFontBold));
                            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTableIncld2);
                        }

                        if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                        {
                            intcolumnscount = intcolumnscount + 1;
                            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(HeadersValue(intcolumnscount) + codeitem.Hrd2, TblFontBold));
                            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTableIncld2);
                        }
                        if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                        {
                            intcolumnscount = intcolumnscount + 1;
                            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(HeadersValue(intcolumnscount) + codeitem.Hrd3, TblFontBold));
                            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTableIncld2);
                        }
                        if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                        {
                            intcolumnscount = intcolumnscount + 1;
                            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(HeadersValue(intcolumnscount) + codeitem.Hrd4, TblFontBold));
                            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTableIncld2);
                        }
                        if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                        {
                            intcolumnscount = intcolumnscount + 1;
                            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(HeadersValue(intcolumnscount) + codeitem.Hrd5, TblFontBold));
                            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTableIncld2);
                        }

                        intcolumnscount = intcolumnscount + 1;
                        PdfPCell pdfMainTableHeader5 = new PdfPCell(new Phrase(HeadersValue(intcolumnscount) + "Percentage Achieving Outcome ", TblFontBold));
                        pdfMainTableHeader5.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfMainTableHeader5.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableHeader5.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER + iTextSharp.text.Rectangle.RIGHT_BORDER;
                        subtable.AddCell(pdfMainTableHeader5);

                        intcolumnscount = intcolumnscount + 1;
                        PdfPCell pdfMainTableHeader6 = new PdfPCell(new Phrase(HeadersValue(intcolumnscount) + "Performance Target Accuracy ", TblFontBold));
                        pdfMainTableHeader6.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfMainTableHeader6.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableHeader6.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.RIGHT_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER;
                        subtable.AddCell(pdfMainTableHeader6);

                        PdfPCell pdfMainTableSubHeader1 = new PdfPCell(new Phrase(codeitem.GrpDesc, TblFontBold));
                        pdfMainTableSubHeader1.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfMainTableSubHeader1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableSubHeader1.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                        subtable.AddCell(pdfMainTableSubHeader1);

                        PdfPCell pdfMainTableSubHeader2 = new PdfPCell(new Phrase("in program(s) (#)", TableFont));
                        pdfMainTableSubHeader2.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfMainTableSubHeader2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableSubHeader2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                        subtable.AddCell(pdfMainTableSubHeader2);

                        PdfPCell pdfMainTableSubHeader3 = new PdfPCell(new Phrase("", TableFont));
                        pdfMainTableSubHeader3.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfMainTableSubHeader3.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableSubHeader3.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                        subtable.AddCell(pdfMainTableSubHeader3);

                        PdfPCell pdfMainTableSubHeader4 = new PdfPCell(new Phrase("", TableFont));
                        pdfMainTableSubHeader4.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfMainTableSubHeader4.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableSubHeader4.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                        subtable.AddCell(pdfMainTableSubHeader4);

                        if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                        {
                            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("", TblFontBold));
                            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTableIncld2);
                        }
                        if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                        {
                            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("", TblFontBold));
                            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTableIncld2);
                        }
                        if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                        {
                            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("", TblFontBold));
                            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTableIncld2);
                        }
                        if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                        {
                            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("", TblFontBold));
                            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTableIncld2);
                        }
                        if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                        {
                            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("", TblFontBold));
                            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTableIncld2);
                        }


                        PdfPCell pdfMainTableSubHeader5 = new PdfPCell(new Phrase("[III/ I = " + HeadersValue1(intcolumnscount - 1) + " ] (% auto calculated) ", TableFont));
                        pdfMainTableSubHeader5.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfMainTableSubHeader5.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableSubHeader5.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                        subtable.AddCell(pdfMainTableSubHeader5);

                        PdfPCell pdfMainTableSubHeader6 = new PdfPCell(new Phrase("(III/II = " + HeadersValue1(intcolumnscount) + " ] (% auto calculated) ", TableFont));
                        pdfMainTableSubHeader6.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfMainTableSubHeader6.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                        pdfMainTableSubHeader6.Border = iTextSharp.text.Rectangle.LEFT_BORDER + iTextSharp.text.Rectangle.RIGHT_BORDER;
                        subtable.AddCell(pdfMainTableSubHeader6);



                        if (rdoperiodBoth.Checked)
                        {
                            PdfPCell pdfMainTablebothSubHeader1 = new PdfPCell(new Phrase("", TblFontBold));
                            pdfMainTablebothSubHeader1.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfMainTablebothSubHeader1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTablebothSubHeader1.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTablebothSubHeader1);

                            PdfPCell pdfMainTablebothSubHeader2 = new PdfPCell(new Phrase("Rept. | Ref.", TableFont));
                            pdfMainTablebothSubHeader2.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfMainTablebothSubHeader2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTablebothSubHeader2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTablebothSubHeader2);

                            PdfPCell pdfMainTablebothSubHeader3 = new PdfPCell(new Phrase("", TableFont));
                            pdfMainTablebothSubHeader3.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfMainTablebothSubHeader3.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTablebothSubHeader3.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTablebothSubHeader3);

                            PdfPCell pdfMainTablebothSubHeader4 = new PdfPCell(new Phrase("Rept. | Ref.", TableFont));
                            pdfMainTablebothSubHeader4.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfMainTablebothSubHeader4.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTablebothSubHeader4.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTablebothSubHeader4);

                            if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                            {
                                PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("Rept. | Ref.", TblFontBold));
                                pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                                subtable.AddCell(pdfMainTableIncld2);
                            }
                            if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                            {
                                PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("Rept. | Ref.", TblFontBold));
                                pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                                subtable.AddCell(pdfMainTableIncld2);
                            }
                            if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                            {
                                PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("Rept. | Ref.", TblFontBold));
                                pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                                subtable.AddCell(pdfMainTableIncld2);
                            }
                            if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                            {
                                PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("Rept. | Ref.", TblFontBold));
                                pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                                subtable.AddCell(pdfMainTableIncld2);
                            }
                            if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                            {
                                PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("Rept. | Ref.", TblFontBold));
                                pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                pdfMainTableIncld2.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                                subtable.AddCell(pdfMainTableIncld2);
                            }


                            PdfPCell pdfMainTablebothSubHeader5 = new PdfPCell(new Phrase("Rept. | Ref.", TableFont));
                            pdfMainTablebothSubHeader5.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfMainTablebothSubHeader5.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTablebothSubHeader5.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                            subtable.AddCell(pdfMainTablebothSubHeader5);

                            PdfPCell pdfMainTablebothSubHeader6 = new PdfPCell(new Phrase("Rept. | Ref.", TableFont));
                            pdfMainTablebothSubHeader6.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfMainTablebothSubHeader6.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                            pdfMainTablebothSubHeader6.Border = iTextSharp.text.Rectangle.LEFT_BORDER + iTextSharp.text.Rectangle.RIGHT_BORDER;
                            subtable.AddCell(pdfMainTablebothSubHeader6);



                        }



                        RngtblCodelist = RngCodelist.FindAll(u => u.GrpCode.Trim() == codeitem.GrpCode.Trim() && u.Code.Trim() == codeitem.Code.Trim() && u.TblCode.Trim() != string.Empty && u.Agency == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).ID.ToString());

                        bool boolOutComesDetails = true;


                        string intcount1, intcount2, intcout3, intcount4, intcount5;
                        string intcummulativecount1, intcummulativecount2, intcummulativecout3, intcummulativecount4, intcummulativecount5;
                        string strIndSwitch = string.Empty;
                        foreach (RCsb14GroupEntity tblEnt in RngtblCodelist)
                        {
                            strIndSwitch = tblEnt.IndSwitch.ToString();

                            if (rdoOutcomesselect.Checked)
                            {
                                boolOutComesDetails = false;

                                if (strIndSwitch == "Y")
                                {
                                    boolOutComesDetails = true;

                                }
                                else
                                {
                                    if (rdoperiodBoth.Checked)
                                    {
                                        RCsb14GroupEntity rngcountdata = RngBothResultlist.Find(u => u.GrpCode.Trim() == codeitem.GrpCode.Trim() && u.Code.Trim() == codeitem.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.unit_cnt.Trim() != "" && u.unit_cnt.Trim() != "0" && u.unit_cnt.Trim().ToUpper() != "UNIT COUNT");
                                        if (rngcountdata != null)
                                        {
                                            boolOutComesDetails = true;
                                        }
                                    }
                                    else
                                    {
                                        RCsb14GroupEntity rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == codeitem.GrpCode.Trim() && u.Code.Trim() == codeitem.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.unit_cnt.Trim() != "" && u.unit_cnt.Trim() != "0" && u.unit_cnt.Trim().ToUpper() != "UNIT COUNT");
                                        if (rngcountdata != null)
                                        {
                                            boolOutComesDetails = true;
                                        }
                                    }
                                }

                            }

                            if (boolOutComesDetails)
                            {

                                if (Rb_User_Def.Checked)
                                {
                                    boolGoalAssoFilter = false;
                                    if (strIndSwitch == "Y")
                                    {
                                        boolGoalAssoFilter = true;
                                    }
                                    else
                                    {
                                        if (GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim()).Count > 0)
                                            boolGoalAssoFilter = true;
                                    }
                                }
                                if (boolGoalAssoFilter)
                                {



                                    intcount1 = intcount2 = intcout3 = "0";
                                    intcount4 = intcount5 = string.Empty;
                                    RCsb14GroupEntity rngcountdata = null;
                                    if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                    {
                                        if (Rb_SNP_Mem.Checked || rbo_ProgramWise.Checked)
                                        {
                                            rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDesc" && u.Count_type.ToString().Trim() == string.Empty);
                                        }
                                        else
                                        {
                                            if (rdoperiodCumulative.Checked)
                                                rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                            else
                                                rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                        }
                                        if (rngcountdata == null)
                                            rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                    }
                                    else
                                    {
                                        if (Rb_SNP_Mem.Checked || rbo_ProgramWise.Checked)
                                        {
                                            rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDesc" && u.Count_type.ToString().Trim() == string.Empty);
                                        }
                                        else
                                        {
                                            if (rdoperiodCumulative.Checked)
                                                rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                            else
                                                rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                        }
                                    }
                                    if (rngcountdata != null)
                                    {
                                        PdfPCell pdfDetailsTable1 = new PdfPCell(new Phrase(tblEnt.TblCode.Trim() + "." + tblEnt.GrpDesc.ToString(), TblFontBold));
                                        pdfDetailsTable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                        pdfDetailsTable1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                        pdfDetailsTable1.Border = iTextSharp.text.Rectangle.BOX;
                                        subtable.AddCell(pdfDetailsTable1);

                                        if (strIndSwitch == "Y")
                                        {
                                            //DataTable dtappcount = Result_Table.Select("SORTUNDUP_APP <>''").CopyToDataTable();
                                            //// DataRow[] dr = dtappcount.Select("GROUP BY SortUnDup_Agy,SortUnDup_Dept,SortUnDup_Prog,SortUnDup_Year,SortUnDup_App,SortUnDup_Fam_ID,SortUnDup_Client_ID");

                                            //List<DataTable> dtAssess = dtappcount.AsEnumerable().GroupBy(row => new { SortUnDup_Agy = row.Field<string>("SortUnDup_Agy"), SortUnDup_Dept = row.Field<string>("SortUnDup_Dept"), SortUnDup_Prog = row.Field<string>("SortUnDup_Prog"), SortUnDup_Year = row.Field<string>("SortUnDup_Year"), SortUnDup_App = row.Field<string>("SortUnDup_App"), SortUnDup_Fam_ID = row.Field<string>("SortUnDup_Fam_ID"), SortUnDup_Client_ID = row.Field<decimal>("SortUnDup_Client_ID") }).Select(g => g.CopyToDataTable()).ToList();

                                            ////DataTable dtappcount1 =  dtappcount.AsEnumerable()
                                            ////    .GroupBy(r => new { SortUnDup_Agy = r["SortUnDup_Agy"], SortUnDup_Dept = r["SortUnDup_Dept"], SortUnDup_Prog = r["SortUnDup_Prog"], SortUnDup_Year = r["SortUnDup_Year"], SortUnDup_App = r["SortUnDup_App"], SortUnDup_Fam_ID = r["SortUnDup_Fam_ID"], SortUnDup_Client_ID = r["SortUnDup_Client_ID"] })
                                            ////    .Select(g =>
                                            ////    {
                                            ////        var row = dt.NewRow();

                                            ////        row["SortUnDup_Agy"] = g.Key.SortUnDup_Agy;
                                            ////        row["SortUnDup_Dept"] = g.Key.SortUnDup_Dept;
                                            ////        row["SortUnDup_Prog"] = g.Key.SortUnDup_Prog;
                                            ////        row["SortUnDup_Year"] = g.Key.SortUnDup_Year;
                                            ////        row["SortUnDup_App"] = g.Key.SortUnDup_App;
                                            ////        row["SortUnDup_Fam_ID"] = g.Key.SortUnDup_Fam_ID;
                                            ////        row["SortUnDup_Client_ID"] = g.Key.SortUnDup_Client_ID;

                                            ////        return row;

                                            ////    })
                                            ////    .CopyToDataTable();

                                            ////DataRow[] dr= dtappcount.AsEnumerable().GroupBy(r => new { Col1 = r["SortUnDup_Agy"], Col2 = r["SortUnDup_Dept"], Col3 = r["SortUnDup_Prog"], Col4 = r["SortUnDup_Year"], Col5 = r["SortUnDup_App"], Col6 = r["SortUnDup_Fam_ID"], Col7 = r["SortUnDup_Client_ID"] });
                                            ////DataView dvResult = ;
                                            ////dvResult.fi
                                            //if (rdoperiodBoth.Checked)
                                            //{
                                            //    intcount1 = IndSwitchReport_Table.Rows.Count.ToString();
                                            //}
                                            //else
                                            intcount1 = IndSwitch_Table.Rows.Count.ToString();

                                            intcount2 = rngcountdata.ExAchev.ToString();
                                            intcout3 = "0";

                                        }
                                        else
                                        {

                                            intcount1 = rngcountdata.unit_cnt.ToString();
                                            intcount2 = rngcountdata.ExAchev.ToString();
                                            switch (strResultColumn)
                                            {
                                                case "0":
                                                    intcout3 = rngcountdata.unit_cnt.ToString();
                                                    break;
                                                case "1":
                                                    intcout3 = rngcountdata.Hrd1.ToString();
                                                    break;
                                                case "2":
                                                    intcout3 = rngcountdata.Hrd2.ToString();
                                                    break;
                                                case "3":
                                                    intcout3 = rngcountdata.Hrd3.ToString();
                                                    break;
                                                case "4":
                                                    intcout3 = rngcountdata.Hrd4.ToString();
                                                    break;
                                                case "5":
                                                    intcout3 = rngcountdata.Hrd5.ToString();
                                                    break;
                                                default:
                                                    intcout3 = rngcountdata.Hrd1.ToString();
                                                    break;
                                            }
                                        }
                                        if (intcout3.Trim() != string.Empty && intcout3 != "0")
                                        {
                                            if (intcount1.Trim() != string.Empty && intcount1 != "0")
                                            {
                                                intcount4 = ((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount1)) * 100).ToString();
                                                if (intcount4.Length > 4)
                                                    intcount4 = intcount4.Substring(0, 4);
                                            }
                                            if (intcount2.Trim() != string.Empty && intcount2 != "0")
                                            {
                                                intcount5 = (((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount2))) * 100).ToString();
                                                if (intcount5.Length > 4)
                                                    intcount5 = intcount5.Substring(0, 4);
                                            }

                                        }


                                        if (rdoperiodBoth.Checked)
                                        {
                                            //PdfPCell pdfDetailstotalTable1 = new PdfPCell(new Phrase("", TableFont));
                                            //pdfDetailstotalTable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                            //pdfDetailstotalTable1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                            //pdfDetailstotalTable1.Border = iTextSharp.text.Rectangle.BOX;
                                            //subtable.AddCell(pdfDetailstotalTable1);


                                            intcummulativecount1 = intcummulativecount2 = intcummulativecout3 = "0";
                                            intcummulativecount4 = intcummulativecount5 = string.Empty;
                                            RCsb14GroupEntity rngcummulativecounttotaldata = null;
                                            if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                            {
                                                if (!Rb_SNP_Mem.Checked)
                                                    rngcummulativecounttotaldata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                                else
                                                {
                                                    if (rdoperiodBoth.Checked)
                                                    {
                                                        rngcummulativecounttotaldata = RngBothResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDesc" && u.Count_type.ToString().Trim() == string.Empty);
                                                    }
                                                    else
                                                        rngcummulativecounttotaldata = rngcountdata;
                                                }
                                                if (rngcummulativecounttotaldata == null)
                                                    rngcummulativecounttotaldata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                            }
                                            else
                                            {
                                                if (!Rb_SNP_Mem.Checked)
                                                {
                                                    rngcummulativecounttotaldata = RngBothResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                                    // rngcummulativecounttotaldata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                                }
                                                else
                                                {
                                                    if (rdoperiodBoth.Checked)
                                                    {
                                                        rngcummulativecounttotaldata = RngBothResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDesc" && u.Count_type.ToString().Trim() == string.Empty);
                                                    }
                                                    else
                                                        rngcummulativecounttotaldata = rngcountdata;
                                                }
                                            }
                                            if (rngcummulativecounttotaldata != null)
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    if (rdoperiodBoth.Checked)
                                                        intcummulativecount1 = IndSwitchReport_Table.Rows.Count.ToString();
                                                    else
                                                        intcummulativecount1 = IndSwitch_Table.Rows.Count.ToString();
                                                    intcummulativecount2 = rngcummulativecounttotaldata.ExAchev.ToString();
                                                    intcummulativecout3 = "0";
                                                }
                                                else
                                                {
                                                    intcummulativecount1 = rngcummulativecounttotaldata.unit_cnt.ToString();
                                                    intcummulativecount2 = rngcummulativecounttotaldata.ExAchev.ToString();
                                                    switch (strResultColumn)
                                                    {
                                                        case "0":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.unit_cnt.ToString();
                                                            break;
                                                        case "1":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd1.ToString();
                                                            break;
                                                        case "2":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd2.ToString();
                                                            break;
                                                        case "3":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd3.ToString();
                                                            break;
                                                        case "4":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd4.ToString();
                                                            break;
                                                        case "5":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd5.ToString();
                                                            break;
                                                        default:
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd1.ToString();
                                                            break;
                                                    }
                                                }
                                                if (intcummulativecount2 == string.Empty)
                                                    intcummulativecount2 = intcount2;

                                                if (intcummulativecout3.Trim() != string.Empty && intcummulativecout3 != "0")
                                                {
                                                    if (intcummulativecount1.Trim() != string.Empty && intcummulativecount1 != "0")
                                                    {
                                                        intcummulativecount4 = ((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount1)) * 100).ToString();
                                                        if (intcummulativecount4.Length > 4)
                                                            intcummulativecount4 = intcummulativecount4.Substring(0, 4);
                                                    }
                                                    if (intcummulativecount2.Trim() != string.Empty && intcummulativecount2 != "0")
                                                    {
                                                        intcummulativecount5 = (((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount2))) * 100).ToString();
                                                        if (intcummulativecount5.Length > 4)
                                                            intcummulativecount5 = intcummulativecount5.Substring(0, 4);
                                                    }

                                                }



                                                PdfPCell pdfDetailstotalTable2 = new PdfPCell(new Phrase(intcount1 + "  |  " + intcummulativecount1, TblFontBold));
                                                pdfDetailstotalTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfDetailstotalTable2.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfDetailstotalTable2);

                                                PdfPCell pdfDetailstotalTable3 = new PdfPCell(new Phrase(intcount2, TblFontBold));
                                                pdfDetailstotalTable3.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfDetailstotalTable3.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfDetailstotalTable3);

                                                PdfPCell pdfDetailstotalTable4 = new PdfPCell(new Phrase(intcout3 + "  |  " + intcummulativecout3, TblFontBold));
                                                pdfDetailstotalTable4.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfDetailstotalTable4.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfDetailstotalTable4);

                                                if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + "  |  " + "0", TblFontBold));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcountdata.Hrd1 + "  |  " + rngcummulativecounttotaldata.Hrd1, TblFontBold));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }

                                                if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + "  |  " + "0", TblFontBold));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcountdata.Hrd2 + "  |  " + rngcummulativecounttotaldata.Hrd2, TblFontBold));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + "  |  " + "0", TblFontBold));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcountdata.Hrd3 + "  |  " + rngcummulativecounttotaldata.Hrd3, TblFontBold));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + "  |  " + "0", TblFontBold));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcountdata.Hrd4 + "  |  " + rngcummulativecounttotaldata.Hrd4, TblFontBold));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + "  |  " + "0", TblFontBold));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcountdata.Hrd5 + "  |  " + rngcummulativecounttotaldata.Hrd5, TblFontBold));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }

                                                string[] strcountArray4 = intcount4.Split('.');
                                                string strcount4 = intcount4.ToString();
                                                if (strcountArray4.Length > 1)
                                                    strcount4 = strcountArray4[0];


                                                //string[] strcountArray5 = intcount5.Split('.');
                                                string strcount5 = string.Empty;
                                                if (intcount5 != string.Empty)
                                                    strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();
                                                //string strcount5 = intcount5.ToString();
                                                //if (strcountArray5.Length > 1)
                                                //{
                                                //    if (strcountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcount5 = strcountArray5[0];
                                                //    }
                                                //}

                                                string[] strcummulativecountArray4 = intcummulativecount4.Split('.');
                                                string strcummulativecount4 = intcummulativecount4.ToString();
                                                if (strcummulativecountArray4.Length > 1)
                                                    strcummulativecount4 = strcummulativecountArray4[0];

                                                PdfPCell pdfDetailstotalTable5 = new PdfPCell(new Phrase((strcount4 == string.Empty ? "" : strcount4 + "%") + " | " + (strcummulativecount4 == string.Empty ? "" : strcummulativecount4 + "%"), TblFontBold));
                                                pdfDetailstotalTable5.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfDetailstotalTable5.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfDetailstotalTable5.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfDetailstotalTable5);


                                                // string[] strcummulativecountArray5 = intcummulativecount5.Split('.');
                                                string strcummulativecount5 = string.Empty;
                                                if (intcummulativecount5 != string.Empty)
                                                    strcummulativecount5 = (Math.Round(Convert.ToDouble(intcummulativecount5), MidpointRounding.AwayFromZero)).ToString();
                                                //string strcummulativecount5 = intcummulativecount5.ToString();
                                                //if (strcummulativecountArray5.Length > 1)
                                                //{
                                                //    if (strcummulativecountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcummulativecount5 = strcummulativecountArray5[0];
                                                //    }
                                                //}

                                                PdfPCell pdfDetailstotalTable6 = new PdfPCell(new Phrase((strcount5 == string.Empty ? "" : strcount5 + "%") + " | " + (strcummulativecount5 == string.Empty ? "" : strcummulativecount5 + "%"), TblFontBold));
                                                pdfDetailstotalTable6.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfDetailstotalTable6.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfDetailstotalTable6.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfDetailstotalTable6);
                                            }
                                        }
                                        else
                                        {


                                            PdfPCell pdfDetailsTable2 = new PdfPCell(new Phrase(intcount1, TblFontBold));
                                            pdfDetailsTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                            pdfDetailsTable2.Border = iTextSharp.text.Rectangle.BOX;
                                            subtable.AddCell(pdfDetailsTable2);

                                            PdfPCell pdfDetailsTable3 = new PdfPCell(new Phrase(intcount2, TblFontBold));
                                            pdfDetailsTable3.HorizontalAlignment = Element.ALIGN_CENTER;
                                            pdfDetailsTable3.Border = iTextSharp.text.Rectangle.BOX;
                                            subtable.AddCell(pdfDetailsTable3);

                                            PdfPCell pdfDetailsTable4 = new PdfPCell(new Phrase(intcout3, TblFontBold));
                                            pdfDetailsTable4.HorizontalAlignment = Element.ALIGN_CENTER;
                                            pdfDetailsTable4.Border = iTextSharp.text.Rectangle.BOX;
                                            subtable.AddCell(pdfDetailsTable4);

                                            if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TblFontBold));
                                                    pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfMainTableIncld2);
                                                }
                                                else
                                                {
                                                    PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcountdata.Hrd1, TblFontBold));
                                                    pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfMainTableIncld2);
                                                }
                                            }

                                            if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TblFontBold));
                                                    pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfMainTableIncld2);
                                                }
                                                else
                                                {
                                                    PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcountdata.Hrd2, TblFontBold));
                                                    pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfMainTableIncld2);
                                                }
                                            }
                                            if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TblFontBold));
                                                    pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfMainTableIncld2);
                                                }
                                                else
                                                {
                                                    PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcountdata.Hrd3, TblFontBold));
                                                    pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfMainTableIncld2);
                                                }
                                            }
                                            if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TblFontBold));
                                                    pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfMainTableIncld2);
                                                }
                                                else
                                                {
                                                    PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcountdata.Hrd4, TblFontBold));
                                                    pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfMainTableIncld2);
                                                }
                                            }
                                            if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TblFontBold));
                                                    pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfMainTableIncld2);
                                                }
                                                else
                                                {
                                                    PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcountdata.Hrd5, TblFontBold));
                                                    pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfMainTableIncld2);
                                                }
                                            }

                                            string[] strcountArray4 = intcount4.Split('.');
                                            string strcount4 = intcount4.ToString();
                                            if (strcountArray4.Length > 1)
                                                strcount4 = strcountArray4[0];

                                            PdfPCell pdfDetailsTable5 = new PdfPCell(new Phrase(strcount4 == string.Empty ? "" : strcount4 + "%", TblFontBold));
                                            pdfDetailsTable5.HorizontalAlignment = Element.ALIGN_CENTER;
                                            pdfDetailsTable5.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                            pdfDetailsTable5.Border = iTextSharp.text.Rectangle.BOX;
                                            subtable.AddCell(pdfDetailsTable5);


                                            // string[] strcountArray5 = intcount5.Split('.');
                                            string strcount5 = string.Empty;
                                            if (intcount5 != string.Empty)
                                                strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();
                                            //if (strcountArray5.Length > 1)
                                            //{
                                            //    if (strcountArray5[1].StartsWith("0"))
                                            //    {
                                            //        strcount5 = strcountArray5[0];
                                            //    }
                                            //}

                                            PdfPCell pdfDetailsTable6 = new PdfPCell(new Phrase(strcount5 == string.Empty ? "" : strcount5 + "%", TblFontBold));
                                            pdfDetailsTable6.HorizontalAlignment = Element.ALIGN_CENTER;
                                            pdfDetailsTable6.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                            pdfDetailsTable6.Border = iTextSharp.text.Rectangle.BOX;
                                            subtable.AddCell(pdfDetailsTable6);
                                        }
                                    }
                                    if (Rb_SNP_Mem.Checked || rbo_ProgramWise.Checked)
                                    {
                                        if (!rdoperiodBoth.Checked)
                                        {
                                            List<RCsb14GroupEntity> rngsubdetailsdata = new List<RCsb14GroupEntity>();
                                            if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                                if (rngsubdetailsdata.Count == 0)
                                                    rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH");
                                            }
                                            else
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                            }
                                            //List<RNGGAEntity> Goalsubdetails = GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                            foreach (RCsb14GroupEntity item in rngsubdetailsdata)
                                            {
                                                intcount1 = intcount2 = intcout3 = "0";
                                                intcount4 = intcount5 = string.Empty;
                                                if (strIndSwitch == "Y")
                                                {
                                                    intcount1 = IndSwitch_Table.Rows.Count.ToString();
                                                    intcount2 = item.ExAchev.ToString();
                                                    intcout3 = "0";
                                                }
                                                else
                                                {
                                                    intcount1 = item.unit_cnt.ToString();
                                                    intcount2 = item.ExAchev.ToString();

                                                    switch (strResultColumn)
                                                    {
                                                        case "0":
                                                            intcout3 = item.unit_cnt.ToString();
                                                            break;
                                                        case "1":
                                                            intcout3 = item.Hrd1.ToString();
                                                            break;
                                                        case "2":
                                                            intcout3 = item.Hrd2.ToString();
                                                            break;
                                                        case "3":
                                                            intcout3 = item.Hrd3.ToString();
                                                            break;
                                                        case "4":
                                                            intcout3 = item.Hrd4.ToString();
                                                            break;
                                                        case "5":
                                                            intcout3 = item.Hrd5.ToString();
                                                            break;
                                                        default:
                                                            intcout3 = item.Hrd1.ToString();
                                                            break;
                                                    }
                                                }
                                                if (intcout3.Trim() != string.Empty && intcout3 != "0")
                                                {
                                                    if (intcount1.Trim() != string.Empty && intcount1 != "0")
                                                    {
                                                        intcount4 = ((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount1)) * 100).ToString();
                                                        if (intcount4.Length > 4)
                                                            intcount4 = intcount4.Substring(0, 4);
                                                    }
                                                    if (intcount2.Trim() != string.Empty && intcount2 != "0")
                                                    {
                                                        intcount5 = (((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount2))) * 100).ToString();
                                                        if (intcount5.Length > 4)
                                                            intcount5 = intcount5.Substring(0, 4);
                                                    }

                                                }



                                                PdfPCell pdfSubDetailsTable1 = new PdfPCell(new Phrase("     " + item.GrpDesc.ToString(), TableFont));
                                                pdfSubDetailsTable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                                pdfSubDetailsTable1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable1.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable1);
                                                if (strIndSwitch == "Y")
                                                {
                                                    PdfPCell pdfSubDetailsTable2 = new PdfPCell(new Phrase(intcount1.ToString(), TableFont));
                                                    pdfSubDetailsTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfSubDetailsTable2);
                                                }
                                                else
                                                {
                                                    PdfPCell pdfSubDetailsTable2 = new PdfPCell(new Phrase(item.unit_cnt.ToString(), TableFont));
                                                    pdfSubDetailsTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfSubDetailsTable2);
                                                }
                                                PdfPCell pdfSubDetailsTable3 = new PdfPCell(new Phrase(intcount2.ToString(), TableFont));
                                                pdfSubDetailsTable3.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable3.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable3);

                                                PdfPCell pdfSubDetailsTable4 = new PdfPCell(new Phrase(intcout3, TableFont));
                                                pdfSubDetailsTable4.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable4.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable4);

                                                if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd1, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }

                                                if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd2, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd3, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd4, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd5, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }

                                                string[] strcountArray4 = intcount4.Split('.');
                                                string strcount4 = intcount4.ToString();
                                                if (strcountArray4.Length > 1)
                                                    strcount4 = strcountArray4[0];


                                                PdfPCell pdfSubDetailsTable5 = new PdfPCell(new Phrase(strcount4 == string.Empty ? "" : strcount4 + "%", TableFont));
                                                pdfSubDetailsTable5.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable5.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable5.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable5);

                                                // string[] strcountArray5 = intcount5.Split('.');
                                                // string strcount5 = intcount5.ToString();
                                                string strcount5 = string.Empty;
                                                if (intcount5 != string.Empty)
                                                    strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();
                                                //if (strcountArray5.Length > 1)
                                                //{
                                                //    if (strcountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcount5 = strcountArray5[0];
                                                //    }
                                                //}

                                                PdfPCell pdfSubDetailsTable6 = new PdfPCell(new Phrase(strcount5 == string.Empty ? "" : strcount5 + "%", TableFont));
                                                pdfSubDetailsTable6.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable6.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable6.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable6);
                                            }
                                        }
                                        else
                                        {
                                            List<RCsb14GroupEntity> rngsubdetailsdata = new List<RCsb14GroupEntity>();
                                            List<RCsb14GroupEntity> rngsubBothdetailsdata = new List<RCsb14GroupEntity>();
                                            if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");

                                                if (rngsubdetailsdata.Count == 0)
                                                    rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH");

                                                rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                                if (rngsubBothdetailsdata.Count == 0)
                                                    rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH" && u.Row_Type.ToString() == "INDSWITCH");
                                            }
                                            else
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                                rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                            }
                                            //List<RNGGAEntity> Goalsubdetails = GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                            int intbothcount = 0;

                                            foreach (RCsb14GroupEntity item in rngsubdetailsdata)
                                            {
                                                intcount1 = intcount2 = intcout3 = "0";
                                                intcount4 = intcount5 = string.Empty;

                                                intcummulativecount1 = intcummulativecount2 = intcummulativecout3 = "0";
                                                intcummulativecount4 = intcummulativecount5 = string.Empty;

                                                if (strIndSwitch == "Y")
                                                {
                                                    intcummulativecount1 = IndSwitchReport_Table.Rows.Count.ToString();
                                                    intcount1 = IndSwitch_Table.Rows.Count.ToString();
                                                    intcummulativecount2 = intcount2 = item.ExAchev.ToString();
                                                    intcout3 = intcummulativecout3 = "0";
                                                }
                                                else
                                                {

                                                    intcount1 = item.unit_cnt.ToString();
                                                    intcummulativecount2 = intcount2 = item.ExAchev.ToString();

                                                    intcummulativecount1 = rngsubBothdetailsdata[intbothcount].unit_cnt.ToString();

                                                    switch (strResultColumn)
                                                    {
                                                        case "0":
                                                            intcout3 = item.unit_cnt.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].unit_cnt.ToString();
                                                            break;
                                                        case "1":
                                                            intcout3 = item.Hrd1.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd1.ToString();
                                                            break;
                                                        case "2":
                                                            intcout3 = item.Hrd2.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd2.ToString();
                                                            break;
                                                        case "3":
                                                            intcout3 = item.Hrd3.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd3.ToString();
                                                            break;
                                                        case "4":
                                                            intcout3 = item.Hrd4.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd4.ToString();
                                                            break;
                                                        case "5":
                                                            intcout3 = item.Hrd5.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd5.ToString();
                                                            break;
                                                        default:
                                                            intcout3 = item.Hrd1.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd1.ToString();
                                                            break;
                                                    }
                                                }
                                                if (intcout3.Trim() != string.Empty && intcout3 != "0")
                                                {
                                                    if (intcount1.Trim() != string.Empty && intcount1 != "0")
                                                    {
                                                        intcount4 = ((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount1)) * 100).ToString();
                                                        if (intcount4.Length > 4)
                                                            intcount4 = intcount4.Substring(0, 4);
                                                    }
                                                    if (intcount2.Trim() != string.Empty && intcount2 != "0")
                                                    {
                                                        intcount5 = (((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount2))) * 100).ToString();
                                                        if (intcount5.Length > 4)
                                                            intcount5 = intcount5.Substring(0, 4);
                                                    }

                                                }

                                                if (intcummulativecout3.Trim() != string.Empty && intcummulativecout3 != "0")
                                                {
                                                    if (intcummulativecount1.Trim() != string.Empty && intcummulativecount1 != "0")
                                                    {
                                                        intcummulativecount4 = ((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount1)) * 100).ToString();
                                                        if (intcummulativecount4.Length > 4)
                                                            intcummulativecount4 = intcummulativecount4.Substring(0, 4);
                                                    }
                                                    if (intcummulativecount2.Trim() != string.Empty && intcummulativecount2 != "0")
                                                    {
                                                        intcummulativecount5 = (((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount2))) * 100).ToString();
                                                        if (intcummulativecount5.Length > 4)
                                                            intcummulativecount5 = intcummulativecount5.Substring(0, 4);
                                                    }

                                                }

                                                PdfPCell pdfSubDetailsTable1 = new PdfPCell(new Phrase("     " + item.GrpDesc.ToString(), TableFont));
                                                pdfSubDetailsTable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                                pdfSubDetailsTable1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable1.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable1);
                                                if (strIndSwitch == "Y")
                                                {
                                                    PdfPCell pdfSubDetailsTable2 = new PdfPCell(new Phrase(intcount1.ToString() + " | " + intcummulativecount1, TableFont));
                                                    pdfSubDetailsTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfSubDetailsTable2);
                                                }
                                                else
                                                {
                                                    PdfPCell pdfSubDetailsTable2 = new PdfPCell(new Phrase(item.unit_cnt.ToString() + " | " + intcummulativecount1, TableFont));
                                                    pdfSubDetailsTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfSubDetailsTable2);

                                                }

                                                PdfPCell pdfSubDetailsTable3 = new PdfPCell(new Phrase(intcount2.ToString(), TableFont));
                                                pdfSubDetailsTable3.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable3.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable3);

                                                PdfPCell pdfSubDetailsTable4 = new PdfPCell(new Phrase(intcout3 + " | " + intcummulativecout3, TableFont));
                                                pdfSubDetailsTable4.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable4.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable4);

                                                if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + " | " + "0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd1 + " | " + rngsubBothdetailsdata[intbothcount].Hrd1, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }

                                                if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + " | " + "0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd2 + " | " + rngsubBothdetailsdata[intbothcount].Hrd2, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + " | " + "0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd3 + " | " + rngsubBothdetailsdata[intbothcount].Hrd3, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + " | " + "0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd4 + " | " + rngsubBothdetailsdata[intbothcount].Hrd4, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + " | " + "0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd5 + " | " + rngsubBothdetailsdata[intbothcount].Hrd5, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }

                                                string[] strcountArray4 = intcount4.Split('.');
                                                string strcount4 = intcount4.ToString();
                                                if (strcountArray4.Length > 1)
                                                    strcount4 = strcountArray4[0];


                                                string[] strcummulativecountArray4 = intcummulativecount4.Split('.');
                                                string strcummulativecount4 = intcummulativecount4.ToString();
                                                if (strcummulativecountArray4.Length > 1)
                                                    strcummulativecount4 = strcummulativecountArray4[0];


                                                PdfPCell pdfSubDetailsTable5 = new PdfPCell(new Phrase((strcount4 == string.Empty ? "" : strcount4 + "%") + " | " + (strcummulativecount4 == string.Empty ? "" : strcummulativecount4 + "%"), TableFont));
                                                pdfSubDetailsTable5.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable5.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable5.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable5);

                                                string[] strcountArray5 = intcount5.Split('.');
                                                string strcount5 = string.Empty;
                                                if (intcount5 != string.Empty)
                                                    strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();
                                                //string strcount5 = intcount5.ToString();
                                                //if (strcountArray5.Length > 1)
                                                //{
                                                //    if (strcountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcount5 = strcountArray5[0];
                                                //    }
                                                //}

                                                //string[] strcummulativecountArray5 = intcummulativecount5.Split('.');
                                                string strcummulativecount5 = string.Empty;
                                                if (intcummulativecount5 != string.Empty)
                                                    strcummulativecount5 = (Math.Round(Convert.ToDouble(intcummulativecount5), MidpointRounding.AwayFromZero)).ToString();
                                                //string strcummulativecount5 = intcummulativecount5.ToString();
                                                //if (strcummulativecountArray5.Length > 1)
                                                //{
                                                //    if (strcummulativecountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcummulativecount5 = strcummulativecountArray5[0];
                                                //    }
                                                //}

                                                PdfPCell pdfSubDetailsTable6 = new PdfPCell(new Phrase((strcount5 == string.Empty ? "" : strcount5 + "%") + " | " + (strcummulativecount5 == string.Empty ? "" : strcummulativecount5 + "%"), TableFont));
                                                pdfSubDetailsTable6.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable6.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable6.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable6);
                                                intbothcount = intbothcount + 1;
                                            }

                                        }

                                    }
                                    else
                                    {
                                        // if (rdoperiodBoth.Checked)
                                        //{
                                        //    PdfPCell pdfDetailstotalTable1 = new PdfPCell(new Phrase("", TableFont));
                                        //    pdfDetailstotalTable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                        //    pdfDetailstotalTable1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                        //    pdfDetailstotalTable1.Border = iTextSharp.text.Rectangle.BOX;
                                        //    subtable.AddCell(pdfDetailstotalTable1);

                                        //    string intcummulativecount1, intcummulativecount2, intcummulativecout3, intcummulativecount4, intcummulativecount5;
                                        //    intcummulativecount1 = intcummulativecount2 = intcummulativecout3 = "0";
                                        //    intcummulativecount4 = intcummulativecount5 = string.Empty;
                                        //    RCsb14GroupEntity rngcummulativecounttotaldata = null;

                                        //    rngcummulativecounttotaldata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                        //    if (rngcummulativecounttotaldata != null)
                                        //    {
                                        //        intcummulativecount1 = rngcummulativecounttotaldata.unit_cnt.ToString();
                                        //        intcummulativecount2 = rngcummulativecounttotaldata.ExAchev.ToString();
                                        //        switch (strResultColumn)
                                        //        {
                                        //            case "0":
                                        //                intcout3 = rngcummulativecounttotaldata.unit_cnt.ToString();
                                        //                break;
                                        //            case "1":
                                        //                intcout3 = rngcummulativecounttotaldata.Hrd1.ToString();
                                        //                break;
                                        //            case "2":
                                        //                intcout3 = rngcummulativecounttotaldata.Hrd2.ToString();
                                        //                break;
                                        //            case "3":
                                        //                intcout3 = rngcummulativecounttotaldata.Hrd3.ToString();
                                        //                break;
                                        //            case "4":
                                        //                intcout3 = rngcummulativecounttotaldata.Hrd4.ToString();
                                        //                break;
                                        //            case "5":
                                        //                intcout3 = rngcummulativecounttotaldata.Hrd5.ToString();
                                        //                break;
                                        //            default:
                                        //                intcout3 = rngcummulativecounttotaldata.Hrd1.ToString();
                                        //                break;
                                        //        }

                                        //        if (intcummulativecout3.Trim() != string.Empty && intcummulativecout3 != "0")
                                        //        {
                                        //            if (intcummulativecount1.Trim() != string.Empty && intcummulativecount1 != "0")
                                        //            {
                                        //                intcummulativecount4 = ((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount1)) * 100).ToString();
                                        //                if (intcummulativecount4.Length > 4)
                                        //                    intcummulativecount4 = intcummulativecount4.Substring(0, 4);
                                        //            }
                                        //            if (intcummulativecount2.Trim() != string.Empty && intcummulativecount2 != "0")
                                        //            {
                                        //                intcummulativecount5 = (((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount2))) * 100).ToString();
                                        //                if (intcummulativecount5.Length > 4)
                                        //                    intcummulativecount5 = intcummulativecount5.Substring(0, 4);
                                        //            }

                                        //        }



                                        //        PdfPCell pdfDetailstotalTable2 = new PdfPCell(new Phrase(intcummulativecount1, TableFont));
                                        //        pdfDetailstotalTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //        pdfDetailstotalTable2.Border = iTextSharp.text.Rectangle.BOX;
                                        //        subtable.AddCell(pdfDetailstotalTable2);

                                        //        PdfPCell pdfDetailstotalTable3 = new PdfPCell(new Phrase(intcummulativecount2, TableFont));
                                        //        pdfDetailstotalTable3.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //        pdfDetailstotalTable3.Border = iTextSharp.text.Rectangle.BOX;
                                        //        subtable.AddCell(pdfDetailstotalTable3);

                                        //        PdfPCell pdfDetailstotalTable4 = new PdfPCell(new Phrase(intcummulativecout3, TableFont));
                                        //        pdfDetailstotalTable4.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //        pdfDetailstotalTable4.Border = iTextSharp.text.Rectangle.BOX;
                                        //        subtable.AddCell(pdfDetailstotalTable4);

                                        //        if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                        //        {
                                        //            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcummulativecounttotaldata.Hrd1, TableFont));
                                        //            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                        //            subtable.AddCell(pdfMainTableIncld2);
                                        //        }

                                        //        if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                        //        {
                                        //            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcummulativecounttotaldata.Hrd2, TableFont));
                                        //            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                        //            subtable.AddCell(pdfMainTableIncld2);
                                        //        }
                                        //        if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                        //        {
                                        //            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcummulativecounttotaldata.Hrd3, TableFont));
                                        //            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                        //            subtable.AddCell(pdfMainTableIncld2);
                                        //        }
                                        //        if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                        //        {
                                        //            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcummulativecounttotaldata.Hrd4, TableFont));
                                        //            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                        //            subtable.AddCell(pdfMainTableIncld2);
                                        //        }
                                        //        if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                        //        {
                                        //            PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(rngcummulativecounttotaldata.Hrd5, TableFont));
                                        //            pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //            pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                        //            subtable.AddCell(pdfMainTableIncld2);
                                        //        }

                                        //        string[] strcummulativecountArray4 = intcummulativecount4.Split('.');
                                        //        string strcummulativecount4 = intcummulativecount4.ToString();
                                        //        if (strcummulativecountArray4.Length > 1)
                                        //            strcummulativecount4 = strcummulativecountArray4[0];

                                        //        PdfPCell pdfDetailstotalTable5 = new PdfPCell(new Phrase(strcummulativecount4 == string.Empty ? "" : strcummulativecount4 + "%", TableFont));
                                        //        pdfDetailstotalTable5.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //        pdfDetailstotalTable5.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                        //        pdfDetailstotalTable5.Border = iTextSharp.text.Rectangle.BOX;
                                        //        subtable.AddCell(pdfDetailstotalTable5);


                                        //        string[] strcummulativecountArray5 = intcummulativecount5.Split('.');
                                        //        string strcummulativecount5 = intcummulativecount5.ToString();
                                        //        if (strcummulativecountArray5.Length > 1)
                                        //        {
                                        //            if (strcummulativecountArray5[1].StartsWith("0"))
                                        //            {
                                        //                strcummulativecount5 = strcummulativecountArray5[0];
                                        //            }
                                        //        }

                                        //        PdfPCell pdfDetailstotalTable6 = new PdfPCell(new Phrase(strcummulativecount5 == string.Empty ? "" : strcummulativecount5 + "%", TableFont));
                                        //        pdfDetailstotalTable6.HorizontalAlignment = Element.ALIGN_CENTER;
                                        //        pdfDetailstotalTable6.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                        //        pdfDetailstotalTable6.Border = iTextSharp.text.Rectangle.BOX;
                                        //        subtable.AddCell(pdfDetailstotalTable6);
                                        //    }
                                        //}
                                    }

                                    if (rbo_ProgramWise.Checked)
                                    {
                                        if (!rdoperiodBoth.Checked)
                                        {
                                            List<RCsb14GroupEntity> rngsubdetailsdata = new List<RCsb14GroupEntity>();
                                            if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");
                                                if (rngsubdetailsdata.Count == 0)
                                                    rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH");
                                            }
                                            else
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");
                                            }
                                            //List<RNGGAEntity> Goalsubdetails = GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                            foreach (RCsb14GroupEntity item in rngsubdetailsdata)
                                            {
                                                intcount1 = intcount2 = intcout3 = "0";
                                                intcount4 = intcount5 = string.Empty;
                                                if (strIndSwitch == "Y")
                                                {
                                                    intcount1 = IndSwitch_Table.Rows.Count.ToString();
                                                    intcount2 = item.ExAchev.ToString();
                                                    intcout3 = "0";
                                                }
                                                else
                                                {
                                                    intcount1 = item.unit_cnt.ToString();
                                                    intcount2 = item.ExAchev.ToString();

                                                    switch (strResultColumn)
                                                    {
                                                        case "0":
                                                            intcout3 = item.unit_cnt.ToString();
                                                            break;
                                                        case "1":
                                                            intcout3 = item.Hrd1.ToString();
                                                            break;
                                                        case "2":
                                                            intcout3 = item.Hrd2.ToString();
                                                            break;
                                                        case "3":
                                                            intcout3 = item.Hrd3.ToString();
                                                            break;
                                                        case "4":
                                                            intcout3 = item.Hrd4.ToString();
                                                            break;
                                                        case "5":
                                                            intcout3 = item.Hrd5.ToString();
                                                            break;
                                                        default:
                                                            intcout3 = item.Hrd1.ToString();
                                                            break;
                                                    }
                                                }
                                                if (intcout3.Trim() != string.Empty && intcout3 != "0")
                                                {
                                                    if (intcount1.Trim() != string.Empty && intcount1 != "0")
                                                    {
                                                        intcount4 = ((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount1)) * 100).ToString();
                                                        if (intcount4.Length > 4)
                                                            intcount4 = intcount4.Substring(0, 4);
                                                    }
                                                    if (intcount2.Trim() != string.Empty && intcount2 != "0")
                                                    {
                                                        intcount5 = (((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount2))) * 100).ToString();
                                                        if (intcount5.Length > 4)
                                                            intcount5 = intcount5.Substring(0, 4);
                                                    }

                                                }



                                                PdfPCell pdfSubDetailsTable1 = new PdfPCell(new Phrase("     " + item.GrpDesc.ToString(), TableFont));
                                                pdfSubDetailsTable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                                pdfSubDetailsTable1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable1.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable1);
                                                if (strIndSwitch == "Y")
                                                {
                                                    PdfPCell pdfSubDetailsTable2 = new PdfPCell(new Phrase(intcount1.ToString(), TableFont));
                                                    pdfSubDetailsTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.VerticalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfSubDetailsTable2);
                                                }
                                                else
                                                {
                                                    PdfPCell pdfSubDetailsTable2 = new PdfPCell(new Phrase(item.unit_cnt.ToString(), TableFont));
                                                    pdfSubDetailsTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.VerticalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfSubDetailsTable2);
                                                }
                                                PdfPCell pdfSubDetailsTable3 = new PdfPCell(new Phrase(intcount2.ToString(), TableFont));
                                                pdfSubDetailsTable3.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable3.VerticalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable3.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable3);

                                                PdfPCell pdfSubDetailsTable4 = new PdfPCell(new Phrase(intcout3, TableFont));
                                                pdfSubDetailsTable4.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable4.VerticalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable4.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable4);

                                                if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd1, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }

                                                if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd2, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd3, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd4, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd5, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }

                                                string[] strcountArray4 = intcount4.Split('.');
                                                string strcount4 = intcount4.ToString();
                                                if (strcountArray4.Length > 1)
                                                    strcount4 = strcountArray4[0];


                                                PdfPCell pdfSubDetailsTable5 = new PdfPCell(new Phrase(strcount4 == string.Empty ? "" : strcount4 + "%", TableFont));
                                                pdfSubDetailsTable5.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable5.VerticalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable5.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable5.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable5);

                                                // string[] strcountArray5 = intcount5.Split('.');
                                                // string strcount5 = intcount5.ToString();
                                                string strcount5 = string.Empty;
                                                if (intcount5 != string.Empty)
                                                    strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();
                                                //if (strcountArray5.Length > 1)
                                                //{
                                                //    if (strcountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcount5 = strcountArray5[0];
                                                //    }
                                                //}

                                                PdfPCell pdfSubDetailsTable6 = new PdfPCell(new Phrase(strcount5 == string.Empty ? "" : strcount5 + "%", TableFont));
                                                pdfSubDetailsTable6.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable6.VerticalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable6.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable6.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable6);
                                            }
                                        }
                                        else
                                        {
                                            List<RCsb14GroupEntity> rngsubdetailsdata = new List<RCsb14GroupEntity>();
                                            List<RCsb14GroupEntity> rngsubBothdetailsdata = new List<RCsb14GroupEntity>();
                                            if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");

                                                if (rngsubdetailsdata.Count == 0)
                                                    rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH");

                                                rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");
                                                if (rngsubBothdetailsdata.Count == 0)
                                                    rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH" && u.Row_Type.ToString() == "INDSWITCH");
                                            }
                                            else
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");
                                                rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");
                                            }
                                            //List<RNGGAEntity> Goalsubdetails = GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                            int intbothcount = 0;

                                            foreach (RCsb14GroupEntity item in rngsubdetailsdata)
                                            {
                                                intcount1 = intcount2 = intcout3 = "0";
                                                intcount4 = intcount5 = string.Empty;

                                                intcummulativecount1 = intcummulativecount2 = intcummulativecout3 = "0";
                                                intcummulativecount4 = intcummulativecount5 = string.Empty;

                                                if (strIndSwitch == "Y")
                                                {
                                                    intcummulativecount1 = IndSwitchReport_Table.Rows.Count.ToString();
                                                    intcount1 = IndSwitch_Table.Rows.Count.ToString();
                                                    intcummulativecount2 = intcount2 = item.ExAchev.ToString();
                                                    intcout3 = intcummulativecout3 = "0";
                                                }
                                                else
                                                {

                                                    intcount1 = item.unit_cnt.ToString();
                                                    intcummulativecount2 = intcount2 = item.ExAchev.ToString();

                                                    intcummulativecount1 = rngsubBothdetailsdata[intbothcount].unit_cnt.ToString();

                                                    switch (strResultColumn)
                                                    {
                                                        case "0":
                                                            intcout3 = item.unit_cnt.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].unit_cnt.ToString();
                                                            break;
                                                        case "1":
                                                            intcout3 = item.Hrd1.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd1.ToString();
                                                            break;
                                                        case "2":
                                                            intcout3 = item.Hrd2.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd2.ToString();
                                                            break;
                                                        case "3":
                                                            intcout3 = item.Hrd3.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd3.ToString();
                                                            break;
                                                        case "4":
                                                            intcout3 = item.Hrd4.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd4.ToString();
                                                            break;
                                                        case "5":
                                                            intcout3 = item.Hrd5.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd5.ToString();
                                                            break;
                                                        default:
                                                            intcout3 = item.Hrd1.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd1.ToString();
                                                            break;
                                                    }
                                                }
                                                if (intcout3.Trim() != string.Empty && intcout3 != "0")
                                                {
                                                    if (intcount1.Trim() != string.Empty && intcount1 != "0")
                                                    {
                                                        intcount4 = ((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount1)) * 100).ToString();
                                                        if (intcount4.Length > 4)
                                                            intcount4 = intcount4.Substring(0, 4);
                                                    }
                                                    if (intcount2.Trim() != string.Empty && intcount2 != "0")
                                                    {
                                                        intcount5 = (((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount2))) * 100).ToString();
                                                        if (intcount5.Length > 4)
                                                            intcount5 = intcount5.Substring(0, 4);
                                                    }

                                                }

                                                if (intcummulativecout3.Trim() != string.Empty && intcummulativecout3 != "0")
                                                {
                                                    if (intcummulativecount1.Trim() != string.Empty && intcummulativecount1 != "0")
                                                    {
                                                        intcummulativecount4 = ((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount1)) * 100).ToString();
                                                        if (intcummulativecount4.Length > 4)
                                                            intcummulativecount4 = intcummulativecount4.Substring(0, 4);
                                                    }
                                                    if (intcummulativecount2.Trim() != string.Empty && intcummulativecount2 != "0")
                                                    {
                                                        intcummulativecount5 = (((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount2))) * 100).ToString();
                                                        if (intcummulativecount5.Length > 4)
                                                            intcummulativecount5 = intcummulativecount5.Substring(0, 4);
                                                    }

                                                }

                                                PdfPCell pdfSubDetailsTable1 = new PdfPCell(new Phrase("     " + item.GrpDesc.ToString(), TableFont));
                                                pdfSubDetailsTable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                                pdfSubDetailsTable1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable1.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable1);
                                                if (strIndSwitch == "Y")
                                                {
                                                    PdfPCell pdfSubDetailsTable2 = new PdfPCell(new Phrase(intcount1.ToString() + " | " + intcummulativecount1, TableFont));
                                                    pdfSubDetailsTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.VerticalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfSubDetailsTable2);
                                                }
                                                else
                                                {
                                                    PdfPCell pdfSubDetailsTable2 = new PdfPCell(new Phrase(item.unit_cnt.ToString() + " | " + intcummulativecount1, TableFont));
                                                    pdfSubDetailsTable2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.VerticalAlignment = Element.ALIGN_CENTER;
                                                    pdfSubDetailsTable2.Border = iTextSharp.text.Rectangle.BOX;
                                                    subtable.AddCell(pdfSubDetailsTable2);

                                                }

                                                PdfPCell pdfSubDetailsTable3 = new PdfPCell(new Phrase(intcount2.ToString(), TableFont));
                                                pdfSubDetailsTable3.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable3.VerticalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable3.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable3);

                                                PdfPCell pdfSubDetailsTable4 = new PdfPCell(new Phrase(intcout3 + " | " + intcummulativecout3, TableFont));
                                                pdfSubDetailsTable4.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable4.VerticalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable4.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable4);

                                                if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + " | " + "0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd1 + " | " + rngsubBothdetailsdata[intbothcount].Hrd1, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }

                                                if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + " | " + "0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd2 + " | " + rngsubBothdetailsdata[intbothcount].Hrd2, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + " | " + "0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd3 + " | " + rngsubBothdetailsdata[intbothcount].Hrd3, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + " | " + "0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd4 + " | " + rngsubBothdetailsdata[intbothcount].Hrd4, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }
                                                if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase("0" + " | " + "0", TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                    else
                                                    {
                                                        PdfPCell pdfMainTableIncld2 = new PdfPCell(new Phrase(item.Hrd5 + " | " + rngsubBothdetailsdata[intbothcount].Hrd5, TableFont));
                                                        pdfMainTableIncld2.HorizontalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.VerticalAlignment = Element.ALIGN_CENTER;
                                                        pdfMainTableIncld2.Border = iTextSharp.text.Rectangle.BOX;
                                                        subtable.AddCell(pdfMainTableIncld2);
                                                    }
                                                }

                                                string[] strcountArray4 = intcount4.Split('.');
                                                string strcount4 = intcount4.ToString();
                                                if (strcountArray4.Length > 1)
                                                    strcount4 = strcountArray4[0];


                                                string[] strcummulativecountArray4 = intcummulativecount4.Split('.');
                                                string strcummulativecount4 = intcummulativecount4.ToString();
                                                if (strcummulativecountArray4.Length > 1)
                                                    strcummulativecount4 = strcummulativecountArray4[0];


                                                PdfPCell pdfSubDetailsTable5 = new PdfPCell(new Phrase((strcount4 == string.Empty ? "" : strcount4 + "%") + " | " + (strcummulativecount4 == string.Empty ? "" : strcummulativecount4 + "%"), TableFont));
                                                pdfSubDetailsTable5.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable5.VerticalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable5.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable5.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable5);

                                                string[] strcountArray5 = intcount5.Split('.');
                                                string strcount5 = string.Empty;
                                                if (intcount5 != string.Empty)
                                                    strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();
                                                //string strcount5 = intcount5.ToString();
                                                //if (strcountArray5.Length > 1)
                                                //{
                                                //    if (strcountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcount5 = strcountArray5[0];
                                                //    }
                                                //}

                                                //string[] strcummulativecountArray5 = intcummulativecount5.Split('.');
                                                string strcummulativecount5 = string.Empty;
                                                if (intcummulativecount5 != string.Empty)
                                                    strcummulativecount5 = (Math.Round(Convert.ToDouble(intcummulativecount5), MidpointRounding.AwayFromZero)).ToString();
                                                //string strcummulativecount5 = intcummulativecount5.ToString();
                                                //if (strcummulativecountArray5.Length > 1)
                                                //{
                                                //    if (strcummulativecountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcummulativecount5 = strcummulativecountArray5[0];
                                                //    }
                                                //}

                                                PdfPCell pdfSubDetailsTable6 = new PdfPCell(new Phrase((strcount5 == string.Empty ? "" : strcount5 + "%") + " | " + (strcummulativecount5 == string.Empty ? "" : strcummulativecount5 + "%"), TableFont));
                                                pdfSubDetailsTable6.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable6.VerticalAlignment = Element.ALIGN_CENTER;
                                                pdfSubDetailsTable6.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                                pdfSubDetailsTable6.Border = iTextSharp.text.Rectangle.BOX;
                                                subtable.AddCell(pdfSubDetailsTable6);
                                                intbothcount = intbothcount + 1;
                                            }

                                        }

                                    }

                                }
                            }
                        }



                        //PdfPCell pdfBottomRow1 = new PdfPCell(new Phrase("FRN #2  CSBG Annual Report ", TableFont));
                        //pdfBottomRow1.HorizontalAlignment = Element.ALIGN_LEFT;
                        //pdfBottomRow1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        //pdfBottomRow1.Colspan = 6;
                        ////Headercell6.BackgroundColor = BaseColor.LIGHT_GRAY;
                        //table.AddCell(pdfBottomRow1);

                        //PdfPCell pdfBottomRow2 = new PdfPCell(new Phrase(DateTime.Now.Month.ToString() + DateTime.Now.Year + "       Module 4, Section A: Individual and Family NPIs - Employment", TableFont));
                        //pdfBottomRow2.HorizontalAlignment = Element.ALIGN_LEFT;
                        //pdfBottomRow2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        //pdfBottomRow2.Colspan = 6;
                        ////Headercell6.BackgroundColor = BaseColor.LIGHT_GRAY;
                        //table.AddCell(pdfBottomRow2);


                        //table.FooterRows = 2;


                        if (subtable.Rows.Count > 0)
                        {
                            PdfPCell R12 = new PdfPCell(subtable);
                            R12.Padding = 0f;
                            R12.Colspan = 6;
                            R12.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            table.AddCell(R12);

                            if (table.Rows.Count > 0)
                            {
                                document.Add(table);
                                subtable.DeleteBodyRows();
                                table.DeleteBodyRows();
                                document.NewPage();
                            }
                        }

                    }

                }
            }
            catch (Exception ex) { document.Add(new Paragraph("Aborted due to Exception............................................... ")); }
            //document.Add(table);                
            document.Close();
            fs.Close();
            fs.Dispose();

            //if (LookupDataAccess.FriendlyName().Contains("2012"))
            //{
            PdfViewerNewForm objfrm = new PdfViewerNewForm(PdfName, null, Result_Table, "RNGB0014_Detail_RdlC.rdlc", "Result Table", ReportPath, BaseForm.UserID, Rb_Details_Yes.Checked, "RNGB0014");
            objfrm.StartPosition = FormStartPosition.CenterScreen;
            objfrm.ShowDialog();
            //}
            //else
            //{
            //    FrmViewer objfrm = new FrmViewer(PdfName,, Result_Table, null, "RNGB0014_Detail_RdlC.rdlc", "Result Table", ReportPath, string.Empty,Rb_Details_Yes.Checked,"RNGB0014");               
            //    //objfrm.FormClosed += new Form.FormClosedEventHandler(On_Delete_PDF_File);
            //    objfrm.ShowDialog();
            //}

        }
        */
       /**ivate void On_SaveFormExcel_Closed(DataTable dtResult, DataTable dtResultBoth)
        {

            strReasonCodes = string.Empty;

            PdfName = "RNGB0014_ExcelMainReport";

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
                string Tmpstr = PdfName + ".xls";
                if (File.Exists(Tmpstr))
                    File.Delete(Tmpstr);
            }
            catch (Exception ex)
            {
                int length = 8;
                string newFileName = System.Guid.NewGuid().ToString();
                newFileName = newFileName.Replace("-", string.Empty);

                Random_Filename = PdfName + newFileName.Substring(0, length) + ".xls";
            }

            if (!string.IsNullOrEmpty(Random_Filename))
                PdfName = Random_Filename;
            else
                PdfName += ".xls";

            try
            {

                WorksheetRow excelrow;
                WorksheetCell cell;

                Workbook book = new Workbook();

                WorksheetStyle mainstyle = book.Styles.Add("MainHeaderStyles");
                mainstyle.Font.FontName = "Tahoma";
                mainstyle.Font.Size = 10;
                mainstyle.Font.Bold = true;
                mainstyle.Font.Color = "#FFFFFF";
                mainstyle.Interior.Color = "#0070c0";
                mainstyle.Interior.Pattern = StyleInteriorPattern.Solid;
                mainstyle.Alignment.Horizontal = StyleHorizontalAlignment.Center;
                mainstyle.Alignment.Vertical = StyleVerticalAlignment.Center;


                WorksheetStyle style1 = book.Styles.Add("Normal");
                style1.Font.FontName = "Tahoma";
                style1.Font.Size = 10;
                style1.Alignment.Horizontal = StyleHorizontalAlignment.Left;
                style1.Alignment.Vertical = StyleVerticalAlignment.Center;

                WorksheetStyle stylecenter = book.Styles.Add("Normalcenter");
                stylecenter.Font.FontName = "Tahoma";
                stylecenter.Font.Bold = true;
                stylecenter.Font.Size = 10;
                stylecenter.Alignment.Horizontal = StyleHorizontalAlignment.Center;
                stylecenter.Alignment.Vertical = StyleVerticalAlignment.Center;

                WorksheetStyle style3 = book.Styles.Add("NormalLeft");
                style3.Font.FontName = "Tahoma";
                style3.Font.Size = 10;
                style3.Interior.Color = "#f2f2f2";
                style3.Alignment.Horizontal = StyleHorizontalAlignment.Left;
                style3.Alignment.Vertical = StyleVerticalAlignment.Center;

                WorksheetStyle style31 = book.Styles.Add("NormalLeft1");
                style31.Font.FontName = "Tahoma";
                style31.Font.Size = 8;
                style31.Interior.Color = "#f2f2f2";
                style31.Alignment.Horizontal = StyleHorizontalAlignment.Left;
                style31.Alignment.Vertical = StyleVerticalAlignment.Center;


                Worksheet sheet = book.Worksheets.Add("Data");
                //sheet.Names.Add(new WorksheetNamedRange("_FilterDatabase", "=Data!R1C1:R"+(MatRepList.Count+1).ToString()+"C11", true));
                sheet.Table.DefaultRowHeight = 14.25F;

                sheet.Table.DefaultColumnWidth = 220.5F;
                sheet.Table.Columns.Add(300);
                sheet.Table.Columns.Add(90);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(80);

                List<CommonEntity> commonHeaderlist = new List<CommonEntity>();



                // CaptainModel _model = new CaptainModel();

                excelrow = sheet.Table.Rows.Add();

                List<RCsb14GroupEntity> RngGrouplist = new List<RCsb14GroupEntity>();
                List<RCsb14GroupEntity> RngtblCodelist = new List<RCsb14GroupEntity>();
                List<RCsb14GroupEntity> RngResultlist = new List<RCsb14GroupEntity>();

                List<RCsb14GroupEntity> RngBothResultlist = new List<RCsb14GroupEntity>();

                if (rdoperiodBoth.Checked) //&& Rb_SNP_Mem.Checked)
                {
                    foreach (DataRow drResultBothitem in dtResultBoth.Rows)
                    {
                        RngBothResultlist.Add(new RCsb14GroupEntity(drResultBothitem, ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), string.Empty));
                    }
                }

                foreach (DataRow drResultitem in dtResult.Rows)
                {
                    RngResultlist.Add(new RCsb14GroupEntity(drResultitem, ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), string.Empty));
                }

                List<RNGGAEntity> GoalDetailsEntity = _model.SPAdminData.Browse_RNGGA(((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).ID.ToString(), string.Empty, string.Empty, string.Empty);
                if (RngCodelist.Count > 0)
                {
                    RngGrouplist = RngCodelist.FindAll(u => u.GrpCode.Trim() != string.Empty && u.TblCode.Trim() == string.Empty && u.Agency == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).ID.ToString() && u.Code == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString());
                    RngtblCodelist = RngCodelist.FindAll(u => u.GrpCode.Trim() != string.Empty && u.TblCode.Trim() != string.Empty && u.Agency == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).ID.ToString() && u.Code == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString());
                }
                if (rdoOutcomesselect.Checked)
                {

                    List<RCsb14GroupEntity> rngindswithclist = RngtblCodelist.FindAll(u => u.IndSwitch == "Y");
                    foreach (RCsb14GroupEntity itemindswitch in rngindswithclist)
                    {
                        List<RCsb14GroupEntity> rngtblcodelist = RngResultlist.FindAll(u => u.TblCode == itemindswitch.TblCode);
                        if (rngtblcodelist.Count == 0)
                        {
                            List<RNGGAEntity> GoalDetailsIndswitchEntity = GoalDetailsEntity.FindAll(u => u.TblCode.Trim() == itemindswitch.TblCode.Trim() && u.GrpCode.Trim() == itemindswitch.GrpCode.Trim() && u.Code.Trim() == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString().Trim());
                            foreach (RNGGAEntity itemGoalind in GoalDetailsIndswitchEntity)
                            {
                                RngResultlist.Add(new RCsb14GroupEntity(((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), itemindswitch.GrpCode, itemindswitch.TblCode, itemGoalind.Desc, "0", "0", "0", "0", "0", itemindswitch.unit_cnt, itemindswitch.ExAchev, itemindswitch.per_Achived, itemindswitch.CalCost, itemindswitch.Count_type, "INDSWITCH"));
                            }

                        }
                        if (rdoperiodBoth.Checked) //&& Rb_SNP_Mem.Checked)
                        {
                            List<RCsb14GroupEntity> rngtblbothcodelist = RngBothResultlist.FindAll(u => u.TblCode == itemindswitch.TblCode);
                            if (rngtblcodelist.Count == 0)
                            {
                                List<RNGGAEntity> GoalDetailsIndswitchEntity = GoalDetailsEntity.FindAll(u => u.TblCode.Trim() == itemindswitch.TblCode.Trim() && u.GrpCode.Trim() == itemindswitch.GrpCode.Trim() && u.Code.Trim() == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString().Trim());
                                foreach (RNGGAEntity itemGoalind in GoalDetailsIndswitchEntity)
                                {
                                    RngBothResultlist.Add(new RCsb14GroupEntity(((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), itemindswitch.GrpCode, itemindswitch.TblCode, itemGoalind.Desc, "0", "0", "0", "0", "0", itemindswitch.unit_cnt, itemindswitch.ExAchev, itemindswitch.per_Achived, itemindswitch.CalCost, itemindswitch.Count_type, "INDSWITCH"));
                                }
                            }
                        }
                    }
                }



                bool boolGroupFilter = true;
                bool boolGoalAssoFilter = true;

                string Agy = string.Empty;
                if (Current_Hierarchy.Substring(0, 2).ToString() != "**")
                    Agy = Current_Hierarchy.Substring(0, 2);
                string Agency = string.Empty;
                DataSet dsAgency = new DataSet();
                if (!string.IsNullOrEmpty(Agy.Trim()))
                {
                    dsAgency = Captain.DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(Agy, null, null, null, null, null, null);
                    if (dsAgency != null && dsAgency.Tables[0].Rows.Count > 0)
                        Agency = dsAgency.Tables[0].Rows[0]["ACR_NAME"].ToString().Trim();
                }

                foreach (RCsb14GroupEntity codeitem in RngGrouplist)
                {


                    if (Rb_User_Def.Checked)
                    {
                        boolGoalAssoFilter = false;
                        if (GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == codeitem.GrpCode.Trim()).Count > 0)
                            boolGoalAssoFilter = true;
                    }
                    if (Rb_SP_Sel.Checked)
                    {
                        boolGroupFilter = false;
                        if (ListRngGroupCode.FindAll(u => u.GrpCode.Trim() == codeitem.GrpCode.Trim()).Count > 0)
                            boolGroupFilter = true;
                    }

                    if ((boolGroupFilter) && (boolGoalAssoFilter))
                    {
                        string strResultColumn = codeitem.ExAchev.ToString();


                        excelrow = sheet.Table.Rows.Add();
                        cell = excelrow.Cells.Add(codeitem.GrpDesc, DataType.String, "MainHeaderStyles");
                        cell.MergeAcross = 11;
                        excelrow.Height = 25;


                        excelrow = sheet.Table.Rows.Add();
                        cell = excelrow.Cells.Add("Department/Program: " + DeptName + ProgName, DataType.String, "NormalLeft1");
                        cell.MergeAcross = 11;

                        excelrow = sheet.Table.Rows.Add();
                        cell = excelrow.Cells.Add("Name of CSBG Eligible Entity Reporting: ", DataType.String, "NormalLeft1");
                        //cell.MergeAcross = 11;

                        excelrow = sheet.Table.Rows.Add();
                        cell = excelrow.Cells.Add(Agency, DataType.String, "NormalLeft1");
                        cell.MergeAcross = 10;

                        excelrow = sheet.Table.Rows.Add();
                        cell = excelrow.Cells.Add("State: ", DataType.String, "NormalLeft1");
                        //cell.MergeAcross = 11;

                        excelrow = sheet.Table.Rows.Add();
                        cell = excelrow.Cells.Add("DUNS: ", DataType.String, "NormalLeft1");
                        cell.MergeAcross = 10;

                        int intcolumns = 6;
                        int intheadercount = 0;
                        if (codeitem.Incld1.ToString().ToUpper() == "TRUE")
                            intheadercount = intheadercount + 1;
                        if (codeitem.Incld2.ToString().ToUpper() == "TRUE")
                            intheadercount = intheadercount + 1;
                        if (codeitem.Incld3.ToString().ToUpper() == "TRUE")
                            intheadercount = intheadercount + 1;
                        if (codeitem.Incld4.ToString().ToUpper() == "TRUE")
                            intheadercount = intheadercount + 1;
                        if (codeitem.Incld5.ToString().ToUpper() == "TRUE")
                            intheadercount = intheadercount + 1;
                        if (strResultColumn == "0")
                            intheadercount = intheadercount + 1;
                        if (intheadercount > 1)
                            intcolumns = intcolumns + (intheadercount - 1);

                        //PdfPTable subtable = new PdfPTable(intcolumns);



                        //float[] subwidths = null;
                        //if (intcolumns == 6)
                        //    subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f };
                        //if (intcolumns == 7)
                        //    subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f, 60f };
                        //if (intcolumns == 8)
                        //    subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f, 60f, 60f };
                        //if (intcolumns == 9)
                        //    subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f };
                        //if (intcolumns == 10)
                        //    subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f };
                        //if (intcolumns == 11)
                        //    subwidths = new float[] { 250f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f };

                        //subtable.SetWidths(subwidths);

                        excelrow = sheet.Table.Rows.Add();

                        cell = excelrow.Cells.Add("", DataType.String, "Normal");

                        cell = excelrow.Cells.Add("I.) Number of Individuals Served ", DataType.String, "Normal");

                        cell = excelrow.Cells.Add("II.) Target (#)", DataType.String, "Normal");

                        cell = excelrow.Cells.Add("III.) Actual Results (#) ", DataType.String, "Normal");

                        int intcolumnscount = 3;
                        if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                        {
                            intcolumnscount = intcolumnscount + 1;

                            cell = excelrow.Cells.Add(HeadersValue(intcolumnscount) + codeitem.Hrd1, DataType.String, "Normal");

                        }

                        if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                        {
                            intcolumnscount = intcolumnscount + 1;
                            cell = excelrow.Cells.Add(HeadersValue(intcolumnscount) + codeitem.Hrd2, DataType.String, "Normal");

                        }
                        if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                        {
                            intcolumnscount = intcolumnscount + 1;
                            cell = excelrow.Cells.Add(HeadersValue(intcolumnscount) + codeitem.Hrd3, DataType.String, "Normal");

                        }
                        if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                        {
                            intcolumnscount = intcolumnscount + 1;
                            cell = excelrow.Cells.Add(HeadersValue(intcolumnscount) + codeitem.Hrd4, DataType.String, "Normal");

                        }
                        if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                        {
                            intcolumnscount = intcolumnscount + 1;
                            cell = excelrow.Cells.Add(HeadersValue(intcolumnscount) + codeitem.Hrd5, DataType.String, "Normal");

                        }

                        intcolumnscount = intcolumnscount + 1;
                        cell = excelrow.Cells.Add(HeadersValue(intcolumnscount) + "Percentage Achieving Outcome ", DataType.String, "Normal");


                        intcolumnscount = intcolumnscount + 1;
                        cell = excelrow.Cells.Add(HeadersValue(intcolumnscount) + "Performance Target Accuracy ", DataType.String, "Normal");


                        excelrow = sheet.Table.Rows.Add();
                        cell = excelrow.Cells.Add(codeitem.GrpDesc, DataType.String, "Normal");


                        cell = excelrow.Cells.Add("in program(s) (#)", DataType.String, "Normal");


                        cell = excelrow.Cells.Add("", DataType.String, "Normal");

                        cell = excelrow.Cells.Add("", DataType.String, "Normal");


                        if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                        {
                            cell = excelrow.Cells.Add("", DataType.String, "Normal");
                        }
                        if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                        {
                            cell = excelrow.Cells.Add("", DataType.String, "Normal");
                        }
                        if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                        {
                            cell = excelrow.Cells.Add("", DataType.String, "Normal");
                        }
                        if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                        {
                            cell = excelrow.Cells.Add("", DataType.String, "Normal");
                        }
                        if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                        {
                            cell = excelrow.Cells.Add("", DataType.String, "Normal");
                        }

                        cell = excelrow.Cells.Add("[III/ I = " + HeadersValue1(intcolumnscount - 1) + " ] (% auto calculated) ", DataType.String, "Normal");



                        cell = excelrow.Cells.Add("(III/II = " + HeadersValue1(intcolumnscount) + " ] (% auto calculated) ", DataType.String, "Normal");



                        if (rdoperiodBoth.Checked)
                        {

                            excelrow = sheet.Table.Rows.Add();
                            cell = excelrow.Cells.Add("", DataType.String, "Normal");

                            cell = excelrow.Cells.Add("Rept. | Ref.", DataType.String, "Normal");

                            cell = excelrow.Cells.Add("", DataType.String, "Normal");

                            cell = excelrow.Cells.Add("Rept. | Ref.", DataType.String, "Normal");

                            if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                            {
                                cell = excelrow.Cells.Add("Rept. | Ref.", DataType.String, "Normal");
                            }
                            if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                            {
                                cell = excelrow.Cells.Add("Rept. | Ref.", DataType.String, "Normal");
                            }
                            if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                            {
                                cell = excelrow.Cells.Add("Rept. | Ref.", DataType.String, "Normal");
                            }
                            if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                            {
                                cell = excelrow.Cells.Add("Rept. | Ref.", DataType.String, "Normal");
                            }
                            if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                            {
                                cell = excelrow.Cells.Add("Rept. | Ref.", DataType.String, "Normal");
                            }


                            cell = excelrow.Cells.Add("Rept. | Ref.", DataType.String, "Normal");

                            cell = excelrow.Cells.Add("Rept. | Ref.", DataType.String, "Normal");


                        }



                        RngtblCodelist = RngCodelist.FindAll(u => u.GrpCode.Trim() == codeitem.GrpCode.Trim() && u.Code.Trim() == codeitem.Code.Trim() && u.TblCode.Trim() != string.Empty && u.Agency == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).ID.ToString());

                        bool boolOutComesDetails = true;


                        string intcount1, intcount2, intcout3, intcount4, intcount5;
                        string intcummulativecount1, intcummulativecount2, intcummulativecout3, intcummulativecount4, intcummulativecount5;
                        string strIndSwitch = string.Empty;
                        foreach (RCsb14GroupEntity tblEnt in RngtblCodelist)
                        {
                            strIndSwitch = tblEnt.IndSwitch.ToString();

                            excelrow = sheet.Table.Rows.Add();

                            if (rdoOutcomesselect.Checked)
                            {
                                boolOutComesDetails = false;

                                if (strIndSwitch == "Y")
                                {
                                    boolOutComesDetails = true;

                                }
                                else
                                {
                                    RCsb14GroupEntity rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == codeitem.GrpCode.Trim() && u.Code.Trim() == codeitem.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.unit_cnt.Trim() != "" && u.unit_cnt.Trim() != "0" && u.unit_cnt.Trim().ToUpper() != "UNIT COUNT");
                                    if (rngcountdata != null)
                                    {
                                        boolOutComesDetails = true;
                                    }
                                }

                            }

                            if (boolOutComesDetails)
                            {

                                if (Rb_User_Def.Checked)
                                {
                                    boolGoalAssoFilter = false;
                                    if (strIndSwitch == "Y")
                                    {
                                        boolGoalAssoFilter = true;
                                    }
                                    else
                                    {
                                        if (GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim()).Count > 0)
                                            boolGoalAssoFilter = true;
                                    }
                                }
                                if (boolGoalAssoFilter)
                                {



                                    intcount1 = intcount2 = intcout3 = "0";
                                    intcount4 = intcount5 = string.Empty;
                                    RCsb14GroupEntity rngcountdata = null;
                                    if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                    {
                                        if (Rb_SNP_Mem.Checked || rbo_ProgramWise.Checked)
                                        {
                                            rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDesc" && u.Count_type.ToString().Trim() == string.Empty);
                                        }
                                        else
                                        {
                                            if (rdoperiodCumulative.Checked)
                                                rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                            else
                                                rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                        }
                                        if (rngcountdata == null)
                                            rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                    }
                                    else
                                    {
                                        if (Rb_SNP_Mem.Checked || rbo_ProgramWise.Checked)
                                        {
                                            rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDesc" && u.Count_type.ToString().Trim() == string.Empty);
                                        }
                                        else
                                        {
                                            if (rdoperiodCumulative.Checked)
                                                rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                            else
                                                rngcountdata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                        }
                                    }
                                    if (rngcountdata != null)
                                    {

                                        cell = excelrow.Cells.Add(tblEnt.TblCode.Trim() + "." + tblEnt.GrpDesc.ToString(), DataType.String, "NormalLeft");

                                        if (strIndSwitch == "Y")
                                        {

                                            intcount1 = IndSwitch_Table.Rows.Count.ToString();

                                            intcount2 = rngcountdata.ExAchev.ToString();
                                            intcout3 = "0";

                                        }
                                        else
                                        {

                                            intcount1 = rngcountdata.unit_cnt.ToString();
                                            intcount2 = rngcountdata.ExAchev.ToString();
                                            switch (strResultColumn)
                                            {
                                                case "0":
                                                    intcout3 = rngcountdata.unit_cnt.ToString();
                                                    break;
                                                case "1":
                                                    intcout3 = rngcountdata.Hrd1.ToString();
                                                    break;
                                                case "2":
                                                    intcout3 = rngcountdata.Hrd2.ToString();
                                                    break;
                                                case "3":
                                                    intcout3 = rngcountdata.Hrd3.ToString();
                                                    break;
                                                case "4":
                                                    intcout3 = rngcountdata.Hrd4.ToString();
                                                    break;
                                                case "5":
                                                    intcout3 = rngcountdata.Hrd5.ToString();
                                                    break;
                                                default:
                                                    intcout3 = rngcountdata.Hrd1.ToString();
                                                    break;
                                            }
                                        }
                                        if (intcout3.Trim() != string.Empty && intcout3 != "0")
                                        {
                                            if (intcount1.Trim() != string.Empty && intcount1 != "0")
                                            {
                                                intcount4 = ((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount1)) * 100).ToString();
                                                if (intcount4.Length > 4)
                                                    intcount4 = intcount4.Substring(0, 4);
                                            }
                                            if (intcount2.Trim() != string.Empty && intcount2 != "0")
                                            {
                                                intcount5 = (((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount2))) * 100).ToString();
                                                if (intcount5.Length > 4)
                                                    intcount5 = intcount5.Substring(0, 4);
                                            }

                                        }


                                        if (rdoperiodBoth.Checked)
                                        {
                                            //PdfPCell pdfDetailstotalTable1 = new PdfPCell(new Phrase("", TableFont));
                                            //pdfDetailstotalTable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                            //pdfDetailstotalTable1.BackgroundColor = new iTextSharp.text.BaseColor(242, 243, 244);
                                            //pdfDetailstotalTable1.Border = iTextSharp.text.Rectangle.BOX;
                                            //subtable.AddCell(pdfDetailstotalTable1);


                                            intcummulativecount1 = intcummulativecount2 = intcummulativecout3 = "0";
                                            intcummulativecount4 = intcummulativecount5 = string.Empty;
                                            RCsb14GroupEntity rngcummulativecounttotaldata = null;
                                            if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                            {
                                                if (!Rb_SNP_Mem.Checked)
                                                    rngcummulativecounttotaldata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                                else
                                                {
                                                    if (rdoperiodBoth.Checked)
                                                    {
                                                        rngcummulativecounttotaldata = RngBothResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDesc" && u.Count_type.ToString().Trim() == string.Empty);
                                                    }
                                                    else
                                                        rngcummulativecounttotaldata = rngcountdata;
                                                }
                                                if (rngcummulativecounttotaldata == null)
                                                    rngcummulativecounttotaldata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                            }
                                            else
                                            {
                                                if (!Rb_SNP_Mem.Checked)
                                                {
                                                    rngcummulativecounttotaldata = RngBothResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                                    // rngcummulativecounttotaldata = RngResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "RefCnt");
                                                }
                                                else
                                                {
                                                    if (rdoperiodBoth.Checked)
                                                    {
                                                        rngcummulativecounttotaldata = RngBothResultlist.Find(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDesc" && u.Count_type.ToString().Trim() == string.Empty);
                                                    }
                                                    else
                                                        rngcummulativecounttotaldata = rngcountdata;
                                                }
                                            }
                                            if (rngcummulativecounttotaldata != null)
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    if (rdoperiodBoth.Checked)
                                                        intcummulativecount1 = IndSwitchReport_Table.Rows.Count.ToString();
                                                    else
                                                        intcummulativecount1 = IndSwitch_Table.Rows.Count.ToString();
                                                    intcummulativecount2 = rngcummulativecounttotaldata.ExAchev.ToString();
                                                    intcummulativecout3 = "0";
                                                }
                                                else
                                                {
                                                    intcummulativecount1 = rngcummulativecounttotaldata.unit_cnt.ToString();
                                                    intcummulativecount2 = rngcummulativecounttotaldata.ExAchev.ToString();
                                                    switch (strResultColumn)
                                                    {
                                                        case "0":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.unit_cnt.ToString();
                                                            break;
                                                        case "1":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd1.ToString();
                                                            break;
                                                        case "2":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd2.ToString();
                                                            break;
                                                        case "3":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd3.ToString();
                                                            break;
                                                        case "4":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd4.ToString();
                                                            break;
                                                        case "5":
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd5.ToString();
                                                            break;
                                                        default:
                                                            intcummulativecout3 = rngcummulativecounttotaldata.Hrd1.ToString();
                                                            break;
                                                    }
                                                }
                                                if (intcummulativecount2 == string.Empty)
                                                    intcummulativecount2 = intcount2;

                                                if (intcummulativecout3.Trim() != string.Empty && intcummulativecout3 != "0")
                                                {
                                                    if (intcummulativecount1.Trim() != string.Empty && intcummulativecount1 != "0")
                                                    {
                                                        intcummulativecount4 = ((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount1)) * 100).ToString();
                                                        if (intcummulativecount4.Length > 4)
                                                            intcummulativecount4 = intcummulativecount4.Substring(0, 4);
                                                    }
                                                    if (intcummulativecount2.Trim() != string.Empty && intcummulativecount2 != "0")
                                                    {
                                                        intcummulativecount5 = (((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount2))) * 100).ToString();
                                                        if (intcummulativecount5.Length > 4)
                                                            intcummulativecount5 = intcummulativecount5.Substring(0, 4);
                                                    }

                                                }


                                                cell = excelrow.Cells.Add(intcount1 + "  |  " + intcummulativecount1, DataType.String, "Normal");



                                                cell = excelrow.Cells.Add(intcount2, DataType.String, "Normal");


                                                cell = excelrow.Cells.Add(intcout3 + "  |  " + intcummulativecout3, DataType.String, "Normal");

                                                if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + "  |  " + "0", DataType.String, "Normal");

                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(rngcountdata.Hrd1 + "  |  " + rngcummulativecounttotaldata.Hrd1, DataType.String, "Normal");

                                                    }
                                                }

                                                if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + "  |  " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(rngcountdata.Hrd2 + "  |  " + rngcummulativecounttotaldata.Hrd2, DataType.String, "Normal");

                                                    }
                                                }
                                                if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + "  |  " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(rngcountdata.Hrd3 + "  |  " + rngcummulativecounttotaldata.Hrd3, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + "  |  " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(rngcountdata.Hrd4 + "  |  " + rngcummulativecounttotaldata.Hrd4, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + "  |  " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(rngcountdata.Hrd5 + "  |  " + rngcummulativecounttotaldata.Hrd5, DataType.String, "Normal");
                                                    }
                                                }

                                                string[] strcountArray4 = intcount4.Split('.');
                                                string strcount4 = intcount4.ToString();
                                                if (strcountArray4.Length > 1)
                                                    strcount4 = strcountArray4[0];


                                                //string[] strcountArray5 = intcount5.Split('.');
                                                string strcount5 = string.Empty;
                                                if (intcount5 != string.Empty)
                                                    strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();
                                                //string strcount5 = intcount5.ToString();
                                                //if (strcountArray5.Length > 1)
                                                //{
                                                //    if (strcountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcount5 = strcountArray5[0];
                                                //    }
                                                //}

                                                string[] strcummulativecountArray4 = intcummulativecount4.Split('.');
                                                string strcummulativecount4 = intcummulativecount4.ToString();
                                                if (strcummulativecountArray4.Length > 1)
                                                    strcummulativecount4 = strcummulativecountArray4[0];

                                                cell = excelrow.Cells.Add((strcount4 == string.Empty ? "" : strcount4 + "%") + " | " + (strcummulativecount4 == string.Empty ? "" : strcummulativecount4 + "%"), DataType.String, "Normal");



                                                // string[] strcummulativecountArray5 = intcummulativecount5.Split('.');
                                                string strcummulativecount5 = string.Empty;
                                                if (intcummulativecount5 != string.Empty)
                                                    strcummulativecount5 = (Math.Round(Convert.ToDouble(intcummulativecount5), MidpointRounding.AwayFromZero)).ToString();
                                                //string strcummulativecount5 = intcummulativecount5.ToString();
                                                //if (strcummulativecountArray5.Length > 1)
                                                //{
                                                //    if (strcummulativecountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcummulativecount5 = strcummulativecountArray5[0];
                                                //    }
                                                //}

                                                cell = excelrow.Cells.Add((strcount5 == string.Empty ? "" : strcount5 + "%") + " | " + (strcummulativecount5 == string.Empty ? "" : strcummulativecount5 + "%"), DataType.String, "Normal");

                                            }
                                        }
                                        else
                                        {
                                            //excelrow = sheet.Table.Rows.Add();

                                            cell = excelrow.Cells.Add(intcount1, DataType.String, "Normal");

                                            cell = excelrow.Cells.Add(intcount2, DataType.String, "Normal");

                                            cell = excelrow.Cells.Add(intcout3, DataType.String, "Normal");

                                            if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                }
                                                else
                                                {
                                                    cell = excelrow.Cells.Add(rngcountdata.Hrd1, DataType.String, "Normal");
                                                }
                                            }

                                            if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                }
                                                else
                                                {
                                                    cell = excelrow.Cells.Add(rngcountdata.Hrd2, DataType.String, "Normal");
                                                }
                                            }
                                            if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                }
                                                else
                                                {
                                                    cell = excelrow.Cells.Add(rngcountdata.Hrd3, DataType.String, "Normal");
                                                }
                                            }
                                            if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                }
                                                else
                                                {
                                                    cell = excelrow.Cells.Add(rngcountdata.Hrd4, DataType.String, "Normal");
                                                }
                                            }
                                            if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                            {
                                                if (strIndSwitch == "Y")
                                                {
                                                    cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                }
                                                else
                                                {
                                                    cell = excelrow.Cells.Add(rngcountdata.Hrd5, DataType.String, "Normal");
                                                }
                                            }

                                            string[] strcountArray4 = intcount4.Split('.');
                                            string strcount4 = intcount4.ToString();
                                            if (strcountArray4.Length > 1)
                                                strcount4 = strcountArray4[0];

                                            cell = excelrow.Cells.Add(strcount4 == string.Empty ? "" : strcount4 + "%", DataType.String, "Normal");


                                            // string[] strcountArray5 = intcount5.Split('.');
                                            string strcount5 = string.Empty;
                                            if (intcount5 != string.Empty)
                                                strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();
                                            //if (strcountArray5.Length > 1)
                                            //{
                                            //    if (strcountArray5[1].StartsWith("0"))
                                            //    {
                                            //        strcount5 = strcountArray5[0];
                                            //    }
                                            //}

                                            cell = excelrow.Cells.Add(strcount5 == string.Empty ? "" : strcount5 + "%", DataType.String, "Normal");

                                        }
                                    }
                                    if (Rb_SNP_Mem.Checked || rbo_ProgramWise.Checked)
                                    {
                                        if (!rdoperiodBoth.Checked)
                                        {


                                            List<RCsb14GroupEntity> rngsubdetailsdata = new List<RCsb14GroupEntity>();
                                            if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                                if (rngsubdetailsdata.Count == 0)
                                                    rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH");
                                            }
                                            else
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                            }
                                            //List<RNGGAEntity> Goalsubdetails = GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                            foreach (RCsb14GroupEntity item in rngsubdetailsdata)
                                            {
                                                excelrow = sheet.Table.Rows.Add();
                                                intcount1 = intcount2 = intcout3 = "0";
                                                intcount4 = intcount5 = string.Empty;
                                                if (strIndSwitch == "Y")
                                                {
                                                    intcount1 = IndSwitch_Table.Rows.Count.ToString();
                                                    intcount2 = item.ExAchev.ToString();
                                                    intcout3 = "0";
                                                }
                                                else
                                                {
                                                    intcount1 = item.unit_cnt.ToString();
                                                    intcount2 = item.ExAchev.ToString();

                                                    switch (strResultColumn)
                                                    {
                                                        case "0":
                                                            intcout3 = item.unit_cnt.ToString();
                                                            break;
                                                        case "1":
                                                            intcout3 = item.Hrd1.ToString();
                                                            break;
                                                        case "2":
                                                            intcout3 = item.Hrd2.ToString();
                                                            break;
                                                        case "3":
                                                            intcout3 = item.Hrd3.ToString();
                                                            break;
                                                        case "4":
                                                            intcout3 = item.Hrd4.ToString();
                                                            break;
                                                        case "5":
                                                            intcout3 = item.Hrd5.ToString();
                                                            break;
                                                        default:
                                                            intcout3 = item.Hrd1.ToString();
                                                            break;
                                                    }
                                                }
                                                if (intcout3.Trim() != string.Empty && intcout3 != "0")
                                                {
                                                    if (intcount1.Trim() != string.Empty && intcount1 != "0")
                                                    {
                                                        intcount4 = ((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount1)) * 100).ToString();
                                                        if (intcount4.Length > 4)
                                                            intcount4 = intcount4.Substring(0, 4);
                                                    }
                                                    if (intcount2.Trim() != string.Empty && intcount2 != "0")
                                                    {
                                                        intcount5 = (((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount2))) * 100).ToString();
                                                        if (intcount5.Length > 4)
                                                            intcount5 = intcount5.Substring(0, 4);
                                                    }

                                                }


                                                cell = excelrow.Cells.Add("     " + item.GrpDesc.ToString(), DataType.String, "Normal");


                                                if (strIndSwitch == "Y")
                                                {
                                                    cell = excelrow.Cells.Add(intcount1.ToString(), DataType.String, "Normal");
                                                }
                                                else
                                                {
                                                    cell = excelrow.Cells.Add(item.unit_cnt.ToString(), DataType.String, "Normal");
                                                }

                                                cell = excelrow.Cells.Add(intcount2.ToString(), DataType.String, "Normal");


                                                cell = excelrow.Cells.Add(intcout3.ToString(), DataType.String, "Normal");


                                                if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0", DataType.String, "Normal");

                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd1, DataType.String, "Normal");
                                                    }
                                                }

                                                if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd2, DataType.String, "Normal");

                                                    }
                                                }
                                                if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd3, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd4, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd5, DataType.String, "Normal");
                                                    }
                                                }

                                                string[] strcountArray4 = intcount4.Split('.');
                                                string strcount4 = intcount4.ToString();
                                                if (strcountArray4.Length > 1)
                                                    strcount4 = strcountArray4[0];


                                                cell = excelrow.Cells.Add(strcount4 == string.Empty ? "" : strcount4 + "%", DataType.String, "Normal");


                                                // string[] strcountArray5 = intcount5.Split('.');
                                                // string strcount5 = intcount5.ToString();
                                                string strcount5 = string.Empty;
                                                if (intcount5 != string.Empty)
                                                    strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();
                                                //if (strcountArray5.Length > 1)
                                                //{
                                                //    if (strcountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcount5 = strcountArray5[0];
                                                //    }
                                                //}

                                                cell = excelrow.Cells.Add(strcount5 == string.Empty ? "" : strcount5 + "%", DataType.String, "Normal");

                                            }
                                        }
                                        else
                                        {
                                            List<RCsb14GroupEntity> rngsubdetailsdata = new List<RCsb14GroupEntity>();
                                            List<RCsb14GroupEntity> rngsubBothdetailsdata = new List<RCsb14GroupEntity>();
                                            if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");

                                                if (rngsubdetailsdata.Count == 0)
                                                    rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH");

                                                rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                                if (rngsubBothdetailsdata.Count == 0)
                                                    rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH" && u.Row_Type.ToString() == "INDSWITCH");
                                            }
                                            else
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                                rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrdCnt");
                                            }
                                            //List<RNGGAEntity> Goalsubdetails = GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                            int intbothcount = 0;

                                            foreach (RCsb14GroupEntity item in rngsubdetailsdata)
                                            {
                                                intcount1 = intcount2 = intcout3 = "0";
                                                intcount4 = intcount5 = string.Empty;

                                                intcummulativecount1 = intcummulativecount2 = intcummulativecout3 = "0";
                                                intcummulativecount4 = intcummulativecount5 = string.Empty;

                                                if (strIndSwitch == "Y")
                                                {
                                                    intcummulativecount1 = IndSwitchReport_Table.Rows.Count.ToString();
                                                    intcount1 = IndSwitch_Table.Rows.Count.ToString();
                                                    intcummulativecount2 = intcount2 = item.ExAchev.ToString();
                                                    intcout3 = intcummulativecout3 = "0";
                                                }
                                                else
                                                {

                                                    intcount1 = item.unit_cnt.ToString();
                                                    intcummulativecount2 = intcount2 = item.ExAchev.ToString();

                                                    intcummulativecount1 = rngsubBothdetailsdata[intbothcount].unit_cnt.ToString();

                                                    switch (strResultColumn)
                                                    {
                                                        case "0":
                                                            intcout3 = item.unit_cnt.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].unit_cnt.ToString();
                                                            break;
                                                        case "1":
                                                            intcout3 = item.Hrd1.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd1.ToString();
                                                            break;
                                                        case "2":
                                                            intcout3 = item.Hrd2.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd2.ToString();
                                                            break;
                                                        case "3":
                                                            intcout3 = item.Hrd3.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd3.ToString();
                                                            break;
                                                        case "4":
                                                            intcout3 = item.Hrd4.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd4.ToString();
                                                            break;
                                                        case "5":
                                                            intcout3 = item.Hrd5.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd5.ToString();
                                                            break;
                                                        default:
                                                            intcout3 = item.Hrd1.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd1.ToString();
                                                            break;
                                                    }
                                                }
                                                if (intcout3.Trim() != string.Empty && intcout3 != "0")
                                                {
                                                    if (intcount1.Trim() != string.Empty && intcount1 != "0")
                                                    {
                                                        intcount4 = ((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount1)) * 100).ToString();
                                                        if (intcount4.Length > 4)
                                                            intcount4 = intcount4.Substring(0, 4);
                                                    }
                                                    if (intcount2.Trim() != string.Empty && intcount2 != "0")
                                                    {
                                                        intcount5 = (((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount2))) * 100).ToString();
                                                        if (intcount5.Length > 4)
                                                            intcount5 = intcount5.Substring(0, 4);
                                                    }

                                                }

                                                if (intcummulativecout3.Trim() != string.Empty && intcummulativecout3 != "0")
                                                {
                                                    if (intcummulativecount1.Trim() != string.Empty && intcummulativecount1 != "0")
                                                    {
                                                        intcummulativecount4 = ((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount1)) * 100).ToString();
                                                        if (intcummulativecount4.Length > 4)
                                                            intcummulativecount4 = intcummulativecount4.Substring(0, 4);
                                                    }
                                                    if (intcummulativecount2.Trim() != string.Empty && intcummulativecount2 != "0")
                                                    {
                                                        intcummulativecount5 = (((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount2))) * 100).ToString();
                                                        if (intcummulativecount5.Length > 4)
                                                            intcummulativecount5 = intcummulativecount5.Substring(0, 4);
                                                    }

                                                }


                                                cell = excelrow.Cells.Add("     " + item.GrpDesc.ToString(), DataType.String, "Normal");

                                                if (strIndSwitch == "Y")
                                                {
                                                    cell = excelrow.Cells.Add(intcount1.ToString() + " | " + intcummulativecount1, DataType.String, "Normal");

                                                }
                                                else
                                                {
                                                    cell = excelrow.Cells.Add(item.unit_cnt.ToString() + " | " + intcummulativecount1, DataType.String, "Normal");

                                                }


                                                cell = excelrow.Cells.Add(intcount2.ToString(), DataType.String, "Normal");



                                                cell = excelrow.Cells.Add(intcout3 + " | " + intcummulativecout3, DataType.String, "Normal");



                                                if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + " | " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd1 + " | " + rngsubBothdetailsdata[intbothcount].Hrd1, DataType.String, "Normal");
                                                    }
                                                }

                                                if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + " | " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd2 + " | " + rngsubBothdetailsdata[intbothcount].Hrd2, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + " | " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd3 + " | " + rngsubBothdetailsdata[intbothcount].Hrd3, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + " | " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd4 + " | " + rngsubBothdetailsdata[intbothcount].Hrd4, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + " | " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd5 + " | " + rngsubBothdetailsdata[intbothcount].Hrd5, DataType.String, "Normal");
                                                    }
                                                }

                                                string[] strcountArray4 = intcount4.Split('.');
                                                string strcount4 = intcount4.ToString();
                                                if (strcountArray4.Length > 1)
                                                    strcount4 = strcountArray4[0];


                                                string[] strcummulativecountArray4 = intcummulativecount4.Split('.');
                                                string strcummulativecount4 = intcummulativecount4.ToString();
                                                if (strcummulativecountArray4.Length > 1)
                                                    strcummulativecount4 = strcummulativecountArray4[0];

                                                cell = excelrow.Cells.Add((strcount4 == string.Empty ? "" : strcount4 + "%") + " | " + (strcummulativecount4 == string.Empty ? "" : strcummulativecount4 + "%"), DataType.String, "Normal");

                                                string[] strcountArray5 = intcount5.Split('.');
                                                string strcount5 = string.Empty;
                                                if (intcount5 != string.Empty)
                                                    strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();


                                                //string[] strcummulativecountArray5 = intcummulativecount5.Split('.');
                                                string strcummulativecount5 = string.Empty;
                                                if (intcummulativecount5 != string.Empty)
                                                    strcummulativecount5 = (Math.Round(Convert.ToDouble(intcummulativecount5), MidpointRounding.AwayFromZero)).ToString();

                                                cell = excelrow.Cells.Add((strcount5 == string.Empty ? "" : strcount5 + "%") + " | " + (strcummulativecount5 == string.Empty ? "" : strcummulativecount5 + "%"), DataType.String, "Normal");
                                                intbothcount = intbothcount + 1;
                                            }

                                        }

                                    }
                                    else
                                    {

                                    }

                                    //Added by Sudheer on 10/10/2022 for Program Wise Counts
                                    if (rbo_ProgramWise.Checked)
                                    {
                                        if (!rdoperiodBoth.Checked)
                                        {


                                            List<RCsb14GroupEntity> rngsubdetailsdata = new List<RCsb14GroupEntity>();
                                            if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");
                                                if (rngsubdetailsdata.Count == 0)
                                                    rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH");
                                            }
                                            else
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");
                                            }
                                            //List<RNGGAEntity> Goalsubdetails = GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                            foreach (RCsb14GroupEntity item in rngsubdetailsdata)
                                            {
                                                excelrow = sheet.Table.Rows.Add();
                                                intcount1 = intcount2 = intcout3 = "0";
                                                intcount4 = intcount5 = string.Empty;
                                                if (strIndSwitch == "Y")
                                                {
                                                    intcount1 = IndSwitch_Table.Rows.Count.ToString();
                                                    intcount2 = item.ExAchev.ToString();
                                                    intcout3 = "0";
                                                }
                                                else
                                                {
                                                    intcount1 = item.unit_cnt.ToString();
                                                    intcount2 = item.ExAchev.ToString();

                                                    switch (strResultColumn)
                                                    {
                                                        case "0":
                                                            intcout3 = item.unit_cnt.ToString();
                                                            break;
                                                        case "1":
                                                            intcout3 = item.Hrd1.ToString();
                                                            break;
                                                        case "2":
                                                            intcout3 = item.Hrd2.ToString();
                                                            break;
                                                        case "3":
                                                            intcout3 = item.Hrd3.ToString();
                                                            break;
                                                        case "4":
                                                            intcout3 = item.Hrd4.ToString();
                                                            break;
                                                        case "5":
                                                            intcout3 = item.Hrd5.ToString();
                                                            break;
                                                        default:
                                                            intcout3 = item.Hrd1.ToString();
                                                            break;
                                                    }
                                                }
                                                if (intcout3.Trim() != string.Empty && intcout3 != "0")
                                                {
                                                    if (intcount1.Trim() != string.Empty && intcount1 != "0")
                                                    {
                                                        intcount4 = ((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount1)) * 100).ToString();
                                                        if (intcount4.Length > 4)
                                                            intcount4 = intcount4.Substring(0, 4);
                                                    }
                                                    if (intcount2.Trim() != string.Empty && intcount2 != "0")
                                                    {
                                                        intcount5 = (((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount2))) * 100).ToString();
                                                        if (intcount5.Length > 4)
                                                            intcount5 = intcount5.Substring(0, 4);
                                                    }

                                                }


                                                cell = excelrow.Cells.Add("     " + item.GrpDesc.ToString(), DataType.String, "Normal");


                                                if (strIndSwitch == "Y")
                                                {
                                                    cell = excelrow.Cells.Add(intcount1.ToString(), DataType.String, "Normal");
                                                }
                                                else
                                                {
                                                    cell = excelrow.Cells.Add(item.unit_cnt.ToString(), DataType.String, "Normal");
                                                }

                                                cell = excelrow.Cells.Add(intcount2.ToString(), DataType.String, "Normal");


                                                cell = excelrow.Cells.Add(intcout3.ToString(), DataType.String, "Normal");


                                                if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0", DataType.String, "Normal");

                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd1, DataType.String, "Normal");
                                                    }
                                                }

                                                if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd2, DataType.String, "Normal");

                                                    }
                                                }
                                                if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd3, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd4, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd5, DataType.String, "Normal");
                                                    }
                                                }

                                                string[] strcountArray4 = intcount4.Split('.');
                                                string strcount4 = intcount4.ToString();
                                                if (strcountArray4.Length > 1)
                                                    strcount4 = strcountArray4[0];


                                                cell = excelrow.Cells.Add(strcount4 == string.Empty ? "" : strcount4 + "%", DataType.String, "Normal");


                                                // string[] strcountArray5 = intcount5.Split('.');
                                                // string strcount5 = intcount5.ToString();
                                                string strcount5 = string.Empty;
                                                if (intcount5 != string.Empty)
                                                    strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();
                                                //if (strcountArray5.Length > 1)
                                                //{
                                                //    if (strcountArray5[1].StartsWith("0"))
                                                //    {
                                                //        strcount5 = strcountArray5[0];
                                                //    }
                                                //}

                                                cell = excelrow.Cells.Add(strcount5 == string.Empty ? "" : strcount5 + "%", DataType.String, "Normal");

                                            }
                                        }
                                        else
                                        {
                                            List<RCsb14GroupEntity> rngsubdetailsdata = new List<RCsb14GroupEntity>();
                                            List<RCsb14GroupEntity> rngsubBothdetailsdata = new List<RCsb14GroupEntity>();
                                            if (strIndSwitch == "Y" && rdoOutcomesselect.Checked)
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");

                                                if (rngsubdetailsdata.Count == 0)
                                                    rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH");

                                                rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");
                                                if (rngsubBothdetailsdata.Count == 0)
                                                    rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "INDSWITCH" && u.Row_Type.ToString() == "INDSWITCH");
                                            }
                                            else
                                            {
                                                rngsubdetailsdata = RngResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");
                                                rngsubBothdetailsdata = RngBothResultlist.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim() && u.Row_Type.ToString() == "TblDetails" && u.Count_type.ToString().Trim() == "PrgCnt");
                                            }
                                            //List<RNGGAEntity> Goalsubdetails = GoalDetailsEntity.FindAll(u => u.GrpCode.Trim() == tblEnt.GrpCode.Trim() && u.Code.Trim() == tblEnt.Code.Trim() && u.TblCode.Trim() == tblEnt.TblCode.Trim());
                                            int intbothcount = 0;

                                            foreach (RCsb14GroupEntity item in rngsubdetailsdata)
                                            {
                                                intcount1 = intcount2 = intcout3 = "0";
                                                intcount4 = intcount5 = string.Empty;

                                                intcummulativecount1 = intcummulativecount2 = intcummulativecout3 = "0";
                                                intcummulativecount4 = intcummulativecount5 = string.Empty;

                                                if (strIndSwitch == "Y")
                                                {
                                                    intcummulativecount1 = IndSwitchReport_Table.Rows.Count.ToString();
                                                    intcount1 = IndSwitch_Table.Rows.Count.ToString();
                                                    intcummulativecount2 = intcount2 = item.ExAchev.ToString();
                                                    intcout3 = intcummulativecout3 = "0";
                                                }
                                                else
                                                {

                                                    intcount1 = item.unit_cnt.ToString();
                                                    intcummulativecount2 = intcount2 = item.ExAchev.ToString();

                                                    intcummulativecount1 = rngsubBothdetailsdata[intbothcount].unit_cnt.ToString();

                                                    switch (strResultColumn)
                                                    {
                                                        case "0":
                                                            intcout3 = item.unit_cnt.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].unit_cnt.ToString();
                                                            break;
                                                        case "1":
                                                            intcout3 = item.Hrd1.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd1.ToString();
                                                            break;
                                                        case "2":
                                                            intcout3 = item.Hrd2.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd2.ToString();
                                                            break;
                                                        case "3":
                                                            intcout3 = item.Hrd3.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd3.ToString();
                                                            break;
                                                        case "4":
                                                            intcout3 = item.Hrd4.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd4.ToString();
                                                            break;
                                                        case "5":
                                                            intcout3 = item.Hrd5.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd5.ToString();
                                                            break;
                                                        default:
                                                            intcout3 = item.Hrd1.ToString();
                                                            intcummulativecout3 = rngsubBothdetailsdata[intbothcount].Hrd1.ToString();
                                                            break;
                                                    }
                                                }
                                                if (intcout3.Trim() != string.Empty && intcout3 != "0")
                                                {
                                                    if (intcount1.Trim() != string.Empty && intcount1 != "0")
                                                    {
                                                        intcount4 = ((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount1)) * 100).ToString();
                                                        if (intcount4.Length > 4)
                                                            intcount4 = intcount4.Substring(0, 4);
                                                    }
                                                    if (intcount2.Trim() != string.Empty && intcount2 != "0")
                                                    {
                                                        intcount5 = (((Convert.ToDecimal(intcout3) / Convert.ToDecimal(intcount2))) * 100).ToString();
                                                        if (intcount5.Length > 4)
                                                            intcount5 = intcount5.Substring(0, 4);
                                                    }

                                                }

                                                if (intcummulativecout3.Trim() != string.Empty && intcummulativecout3 != "0")
                                                {
                                                    if (intcummulativecount1.Trim() != string.Empty && intcummulativecount1 != "0")
                                                    {
                                                        intcummulativecount4 = ((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount1)) * 100).ToString();
                                                        if (intcummulativecount4.Length > 4)
                                                            intcummulativecount4 = intcummulativecount4.Substring(0, 4);
                                                    }
                                                    if (intcummulativecount2.Trim() != string.Empty && intcummulativecount2 != "0")
                                                    {
                                                        intcummulativecount5 = (((Convert.ToDecimal(intcummulativecout3) / Convert.ToDecimal(intcummulativecount2))) * 100).ToString();
                                                        if (intcummulativecount5.Length > 4)
                                                            intcummulativecount5 = intcummulativecount5.Substring(0, 4);
                                                    }

                                                }


                                                cell = excelrow.Cells.Add("     " + item.GrpDesc.ToString(), DataType.String, "Normal");

                                                if (strIndSwitch == "Y")
                                                {
                                                    cell = excelrow.Cells.Add(intcount1.ToString() + " | " + intcummulativecount1, DataType.String, "Normal");

                                                }
                                                else
                                                {
                                                    cell = excelrow.Cells.Add(item.unit_cnt.ToString() + " | " + intcummulativecount1, DataType.String, "Normal");

                                                }


                                                cell = excelrow.Cells.Add(intcount2.ToString(), DataType.String, "Normal");



                                                cell = excelrow.Cells.Add(intcout3 + " | " + intcummulativecout3, DataType.String, "Normal");



                                                if ((codeitem.Incld1.ToString().ToUpper() == "TRUE") && (strResultColumn != "1" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + " | " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd1 + " | " + rngsubBothdetailsdata[intbothcount].Hrd1, DataType.String, "Normal");
                                                    }
                                                }

                                                if ((codeitem.Incld2.ToString().ToUpper() == "TRUE") && (strResultColumn != "2" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + " | " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd2 + " | " + rngsubBothdetailsdata[intbothcount].Hrd2, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld3.ToString().ToUpper() == "TRUE") && (strResultColumn != "3" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + " | " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd3 + " | " + rngsubBothdetailsdata[intbothcount].Hrd3, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld4.ToString().ToUpper() == "TRUE") && (strResultColumn != "4" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + " | " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd4 + " | " + rngsubBothdetailsdata[intbothcount].Hrd4, DataType.String, "Normal");
                                                    }
                                                }
                                                if ((codeitem.Incld5.ToString().ToUpper() == "TRUE") && (strResultColumn != "5" || strResultColumn == "0"))
                                                {
                                                    if (strIndSwitch == "Y")
                                                    {
                                                        cell = excelrow.Cells.Add("0" + " | " + "0", DataType.String, "Normal");
                                                    }
                                                    else
                                                    {
                                                        cell = excelrow.Cells.Add(item.Hrd5 + " | " + rngsubBothdetailsdata[intbothcount].Hrd5, DataType.String, "Normal");
                                                    }
                                                }

                                                string[] strcountArray4 = intcount4.Split('.');
                                                string strcount4 = intcount4.ToString();
                                                if (strcountArray4.Length > 1)
                                                    strcount4 = strcountArray4[0];


                                                string[] strcummulativecountArray4 = intcummulativecount4.Split('.');
                                                string strcummulativecount4 = intcummulativecount4.ToString();
                                                if (strcummulativecountArray4.Length > 1)
                                                    strcummulativecount4 = strcummulativecountArray4[0];

                                                cell = excelrow.Cells.Add((strcount4 == string.Empty ? "" : strcount4 + "%") + " | " + (strcummulativecount4 == string.Empty ? "" : strcummulativecount4 + "%"), DataType.String, "Normal");

                                                string[] strcountArray5 = intcount5.Split('.');
                                                string strcount5 = string.Empty;
                                                if (intcount5 != string.Empty)
                                                    strcount5 = (Math.Round(Convert.ToDouble(intcount5), MidpointRounding.AwayFromZero)).ToString();


                                                //string[] strcummulativecountArray5 = intcummulativecount5.Split('.');
                                                string strcummulativecount5 = string.Empty;
                                                if (intcummulativecount5 != string.Empty)
                                                    strcummulativecount5 = (Math.Round(Convert.ToDouble(intcummulativecount5), MidpointRounding.AwayFromZero)).ToString();

                                                cell = excelrow.Cells.Add((strcount5 == string.Empty ? "" : strcount5 + "%") + " | " + (strcummulativecount5 == string.Empty ? "" : strcummulativecount5 + "%"), DataType.String, "Normal");
                                                intbothcount = intbothcount + 1;
                                            }

                                        }

                                    }

                                }
                            }
                        }

                        excelrow = sheet.Table.Rows.Add();
                        excelrow.Height = 100;
                        cell = excelrow.Cells.Add("", DataType.String, "Normal");
                        cell.MergeAcross = 11;

                    }

                }

                FileStream stream1 = new FileStream(PdfName, FileMode.Create);
                book.Save(stream1);
                stream1.Close();
            }
            catch (Exception ex) { }


        }*/


        #region properties

        public string Scr_Code { get; set; }

        public string Report_Name { get; set; }

        public string Report_To_Process { get; set; }

        public DataTable Result_Table { get; set; }
        public DataTable IndSwitch_Table { get; set; }
        public DataTable IndSwitchReport_Table { get; set; }

        public DataTable Summary_table { get; set; }

        public DataSet Result_DataSet { get; set; }

        public bool Detail_Rep_Required { get; set; }

        public string Main_Rep_Name { get; set; }


        #endregion

        private void GenerateWorksheetParameters(WorksheetCollection sheets)
        {

            Worksheet sheet = sheets.Add("Parameters");
            sheet.Table.DefaultRowHeight = 14.4F;

            WorksheetColumn columnHead = sheet.Table.Columns.Add();
            columnHead.Index = 2;
            columnHead.Width = 5;
            sheet.Table.Columns.Add(163);
            WorksheetColumn column2Head = sheet.Table.Columns.Add();
            column2Head.Width = 332;
            column2Head.StyleID = "s172H";
            sheet.Table.Columns.Add(59);
            // -----------------------------------------------
            WorksheetRow RowHead = sheet.Table.Rows.Add();
            WorksheetCell cell;
            cell = RowHead.Cells.Add();
            cell.StyleID = "s139";
            cell = RowHead.Cells.Add();
            cell.StyleID = "s139";
            cell = RowHead.Cells.Add();
            cell.StyleID = "s139";
            cell = RowHead.Cells.Add();
            cell.StyleID = "s170H";
            cell = RowHead.Cells.Add();
            cell.StyleID = "s139";
            // -----------------------------------------------
            WorksheetRow Row1Head = sheet.Table.Rows.Add();
            Row1Head.Height = 12;
            Row1Head.AutoFitHeight = false;
            cell = Row1Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row1Head.Cells.Add();
            cell.StyleID = "s140";
            cell = Row1Head.Cells.Add();
            cell.StyleID = "s141";
            cell = Row1Head.Cells.Add();
            cell.StyleID = "s171H";
            cell = Row1Head.Cells.Add();
            cell.StyleID = "s142";
            // -----------------------------------------------
            WorksheetRow Row2Head = sheet.Table.Rows.Add();
            Row2Head.Height = 12;
            Row2Head.AutoFitHeight = false;
            cell = Row2Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row2Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row2Head.Cells.Add();
            cell.StyleID = "m2611536909264";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;
            // -----------------------------------------------
            WorksheetRow Row3Head = sheet.Table.Rows.Add();
            Row3Head.Height = 12;
            Row3Head.AutoFitHeight = false;
            cell = Row3Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row3Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row3Head.Cells.Add();
            cell.StyleID = "m2611536909284";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "Selected Report Parameters";
            cell.MergeAcross = 2;
            // -----------------------------------------------
            WorksheetRow Row4Head = sheet.Table.Rows.Add();
            Row4Head.Height = 12;
            Row4Head.AutoFitHeight = false;
            cell = Row4Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row4Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row4Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row4Head.Cells.Add();
            cell.StyleID = "s170H";
            cell = Row4Head.Cells.Add();
            cell.StyleID = "s145";
            // -----------------------------------------------
            WorksheetRow Row5Head = sheet.Table.Rows.Add();
            Row5Head.Height = 12;
            Row5Head.AutoFitHeight = false;
            cell = Row5Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row5Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row5Head.Cells.Add();
            cell.StyleID = "m2611536909304";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;
            // -----------------------------------------------
            WorksheetRow Row6Head = sheet.Table.Rows.Add();
            Row6Head.Height = 12;
            Row6Head.AutoFitHeight = false;
            cell = Row6Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row6Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row6Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row6Head.Cells.Add();
            cell.StyleID = "s170H";
            cell = Row6Head.Cells.Add();
            cell.StyleID = "s145";
            // -----------------------------------------------


            //**string Header_year = string.Empty;
            //if (CmbYear.Visible == true)
            //    Header_year = "Year : " + ((Captain.Common.Utilities.ListItem)CmbYear.SelectedItem).Text.ToString();

            WorksheetRow Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + txtHieDesc.Text.Trim();//** + "     " + Header_year;
            cell.MergeAcross = 2;
            // -----------------------------------------------

            string caseType = string.Empty;
            caseType = ((Captain.Common.Utilities.ListItem)Cmb_CaseType.SelectedItem).Text.ToString().Trim();


            WorksheetRow Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblCaseType.Text.Trim() + ": " + caseType;
            cell.MergeAcross = 2;

            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblServiceplan.Text + ": " + (Rb_SP_All.Checked ? "All" : Rb_SP_Sel.Text);
            cell.MergeAcross = 2;


            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            string caseStatus = Rb_Stat_Both.Text;

            if (Rb_Stat_Act.Checked == true)
                caseStatus = Rb_Stat_Act.Text;
            else
                caseStatus = Rb_Stat_InAct.Text;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblCaseStatus.Text + ": " + caseStatus;
            cell.MergeAcross = 2;

            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblDateSelection.Text.Trim() + ": " + (Rb_MS_Date.Checked == true ? Rb_MS_Date.Text : Rb_MS_AddDate.Text);
            cell.MergeAcross = 2;


            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblRepDesc.Text.Trim() + ": " + ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Text.ToString().Trim();
            cell.MergeAcross = 2;

            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblQ1.Text.Trim() + ": " + dtpQ1S.Text + " - " + dtpQ1E.Text;
            cell.MergeAcross = 2;

            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblQ2.Text.Trim() + ": " + dtpQ2S.Text + " - " + dtpQ2E.Text;
            cell.MergeAcross = 2;

            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblQ3.Text.Trim() + ": " + dtpQ3S.Text + " - " + dtpQ3E.Text;
            cell.MergeAcross = 2;

            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblQ4.Text.Trim() + ": " + dtpQ4S.Text + " - " + dtpQ4E.Text;
            cell.MergeAcross = 2;

            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;


            //Row7Head = sheet.Table.Rows.Add();
            //Row7Head.Height = 12;
            //Row7Head.AutoFitHeight = false;
            //cell = Row7Head.Cells.Add();
            //cell.StyleID = "s139";
            //cell = Row7Head.Cells.Add();
            //cell.StyleID = "s143";
            //cell = Row7Head.Cells.Add();
            //cell.StyleID = "m2611536909324";
            //cell.Data.Type = DataType.String;
            //cell.Data.Text = "            " + lblAttributes.Text.Trim() + ": " + (Rb_Agy_Def.Checked == true ? Rb_Agy_Def.Text : Rb_User_Def.Text);
            //cell.MergeAcross = 2;


            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblProduceStatistical.Text.Trim() + ": " + (Rb_Details_No.Checked == true ? Rb_Details_No.Text : Rb_Details_Yes.Text);
            cell.MergeAcross = 2;

            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblPovertyLevel.Text.Trim() + ": " + lblLow.Text + ": " + Txt_Pov_Low.Text + "      " + lblHigh.Text + ": " + Txt_Pov_High.Text;
            cell.MergeAcross = 2;

            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

            if (chkincverswitch.Checked)
            { 
                Row7Head = sheet.Table.Rows.Add();
                Row7Head.Height = 12;
                Row7Head.AutoFitHeight = false;
                cell = Row7Head.Cells.Add();
                cell.StyleID = "s139";
                cell = Row7Head.Cells.Add();
                cell.StyleID = "s143";
                cell = Row7Head.Cells.Add();
                cell.StyleID = "m2611536909324";
                cell.Data.Type = DataType.String;
                cell.Data.Text = "            " + chkincverswitch.Text;
                cell.MergeAcross = 2;

                Row8 = sheet.Table.Rows.Add();
                Row8.Height = 12;
                Row8.AutoFitHeight = false;
                cell = Row8.Cells.Add();
                cell.StyleID = "s139";
                cell = Row8.Cells.Add();
                cell.StyleID = "s143";
                cell = Row8.Cells.Add();
                cell.StyleID = "m2611536909344";
                cell.Data.Type = DataType.String;
                cell.MergeAcross = 2; 
            }

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblCounty.Text.Trim() + " : " + (Rb_County_All.Checked ? "All" : Rb_County_Sel.Text);
            cell.MergeAcross = 2;

            Row8 = sheet.Table.Rows.Add();
            Row8.Height = 12;
            Row8.AutoFitHeight = false;
            cell = Row8.Cells.Add();
            cell.StyleID = "s139";
            cell = Row8.Cells.Add();
            cell.StyleID = "s143";
            cell = Row8.Cells.Add();
            cell.StyleID = "m2611536909344";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;

           /* if (chkbUndupTable.Checked)
            {
                Row7Head = sheet.Table.Rows.Add();
                Row7Head.Height = 12;
                Row7Head.AutoFitHeight = false;
                cell = Row7Head.Cells.Add();
                cell.StyleID = "s139";
                cell = Row7Head.Cells.Add();
                cell.StyleID = "s143";
                cell = Row7Head.Cells.Add();
                cell.StyleID = "m2611536909324";
                cell.Data.Type = DataType.String;
                cell.Data.Text = "            " + chkbUndupTable.Text;
                cell.MergeAcross = 2;

                Row8 = sheet.Table.Rows.Add();
                Row8.Height = 12;
                Row8.AutoFitHeight = false;
                cell = Row8.Cells.Add();
                cell.StyleID = "s139";
                cell = Row8.Cells.Add();
                cell.StyleID = "s143";
                cell = Row8.Cells.Add();
                cell.StyleID = "m2611536909344";
                cell.Data.Type = DataType.String;
                cell.MergeAcross = 2;
            }*/

            string secretApp = Rb_Mst_NonSec.Text;

            if (Rb_Mst_Sec.Checked == true)
                caseStatus = Rb_Mst_Sec.Text;
            else
                caseStatus = Rb_Mst_BothSec.Text;

            Row7Head = sheet.Table.Rows.Add();
            Row7Head.Height = 12;
            Row7Head.AutoFitHeight = false;
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row7Head.Cells.Add();
            cell.StyleID = "m2611536909324";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "            " + lblSecretApplications.Text.Trim() + " : " + secretApp;
            cell.MergeAcross = 2;

            // -----------------------------------------------
            WorksheetRow Row24 = sheet.Table.Rows.Add();
            Row24.Height = 12;
            Row24.AutoFitHeight = false;
            cell = Row24.Cells.Add();
            cell.StyleID = "s139";
            cell = Row24.Cells.Add();
            cell.StyleID = "s143";
            cell = Row24.Cells.Add();
            cell.StyleID = "m2611540549632";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;
            // -----------------------------------------------
            WorksheetRow Row25 = sheet.Table.Rows.Add();
            Row25.Height = 12;
            Row25.AutoFitHeight = false;
            cell = Row25.Cells.Add();
            cell.StyleID = "s139";
            cell = Row25.Cells.Add();
            cell.StyleID = "s143";
            cell = Row25.Cells.Add();
            cell.StyleID = "m2611540549652";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;
            // -----------------------------------------------
            WorksheetRow Row26Head = sheet.Table.Rows.Add();
            Row26Head.Height = 12;
            Row26Head.AutoFitHeight = false;
            cell = Row26Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row26Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row26Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row26Head.Cells.Add();
            cell.StyleID = "s170H";
            cell = Row26Head.Cells.Add();
            cell.StyleID = "s145";
            // -----------------------------------------------
            WorksheetRow Row27Head = sheet.Table.Rows.Add();
            Row27Head.Height = 12;
            Row27Head.AutoFitHeight = false;
            cell = Row27Head.Cells.Add();
            cell.StyleID = "s139";
            cell = Row27Head.Cells.Add();
            cell.StyleID = "s143";
            cell = Row27Head.Cells.Add();
            cell.StyleID = "m2611540549672";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;
            // -----------------------------------------------
            WorksheetRow Row28 = sheet.Table.Rows.Add();
            Row28.Height = 12;
            Row28.AutoFitHeight = false;
            cell = Row28.Cells.Add();
            cell.StyleID = "s139";
            cell = Row28.Cells.Add();
            cell.StyleID = "s143";
            cell = Row28.Cells.Add();
            cell.StyleID = "s139";
            cell = Row28.Cells.Add();
            cell.StyleID = "s170H";
            cell = Row28.Cells.Add();
            cell.StyleID = "s145";
            // -----------------------------------------------
            WorksheetRow Row29 = sheet.Table.Rows.Add();
            Row29.Height = 12;
            Row29.AutoFitHeight = false;
            cell = Row29.Cells.Add();
            cell.StyleID = "s139";
            cell = Row29.Cells.Add();
            cell.StyleID = "s143";
            cell = Row29.Cells.Add();
            cell.StyleID = "m2611540549552";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;
            // -----------------------------------------------
            WorksheetRow Row30 = sheet.Table.Rows.Add();
            Row30.Height = 12;
            Row30.AutoFitHeight = false;
            cell = Row30.Cells.Add();
            cell.StyleID = "s139";
            cell = Row30.Cells.Add();
            cell.StyleID = "s143";
            cell = Row30.Cells.Add();
            cell.StyleID = "s139";
            cell = Row30.Cells.Add();
            cell.StyleID = "s170H";
            cell = Row30.Cells.Add();
            cell.StyleID = "s145";
            // -----------------------------------------------
            WorksheetRow Row31 = sheet.Table.Rows.Add();
            Row31.Height = 12;
            Row31.AutoFitHeight = false;
            cell = Row31.Cells.Add();
            cell.StyleID = "s139";
            cell = Row31.Cells.Add();
            cell.StyleID = "s146";
            cell = Row31.Cells.Add();
            cell.StyleID = "m2611540549572";
            cell.Data.Type = DataType.String;
            cell.MergeAcross = 2;
            // -----------------------------------------------
            //  Options
            // -----------------------------------------------
            sheet.Options.Selected = true;
            sheet.Options.ProtectObjects = false;
            sheet.Options.ProtectScenarios = false;
            sheet.Options.PageSetup.Header.Margin = 0.3F;
            sheet.Options.PageSetup.Footer.Margin = 0.3F;
            sheet.Options.PageSetup.PageMargins.Bottom = 0.75F;
            sheet.Options.PageSetup.PageMargins.Left = 0.7F;
            sheet.Options.PageSetup.PageMargins.Right = 0.7F;
            sheet.Options.PageSetup.PageMargins.Top = 0.75F;
        }

       
        #endregion


       

        List<Csb16DTREntity> DateRange_List = new List<Csb16DTREntity>();
        List<Csb16DTREntity> Sys_DateRange_List = new List<Csb16DTREntity>();
        private void Get_MasterTable_DateRanges()
        {
            Csb16DTREntity Search_Entity = new Csb16DTREntity(true);
            DateRange_List = _model.SPAdminData.Browse_CSB16DTR(Search_Entity, "Browse");

            Search_Entity.SYS_Date_Range = DateTime.Today.ToShortDateString();
            Sys_DateRange_List = _model.SPAdminData.Browse_CSB16DTR(Search_Entity, "Browse");
        }

       
        private bool Validate_Report()
        {
            bool Can_Generate = true;
            if (((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString() != "**")
            {
                if (dtpQ1S.Value > dtpQ1E.Value)
                {
                    _errorProvider.SetError(dtpQ1S, string.Format("First Quarter 'From Date' should be prior or equal to its 'To Date'".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                {
                    _errorProvider.SetError(dtpQ1S, null);
                }

                DateTime FFdate = Convert.ToDateTime(rngcodedata.OFdate);
                DateTime FTdate = new DateTime(Convert.ToDateTime(rngcodedata.OFdate).Year, Convert.ToDateTime(rngcodedata.OFdate).Month + 2, DateTime.DaysInMonth(Convert.ToDateTime(rngcodedata.OFdate).Month + 2, Convert.ToDateTime(rngcodedata.OFdate).Month + 2));

                if (dtpQ1S.Value < FFdate)
                {
                    _errorProvider.SetError(dtpQ1E, string.Format("First Quarter Date Range should be in between January and March months of selected year".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                    _errorProvider.SetError(dtpQ1E, null);

                if (dtpQ1E.Value > FTdate)
                {
                    _errorProvider.SetError(dtpQ1E, string.Format("First Quarter Date Range should be in between January and March months of selected year".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                    _errorProvider.SetError(dtpQ1E, null);

                if (dtpQ2S.Value > dtpQ2E.Value)
                {
                    _errorProvider.SetError(dtpQ2S, string.Format("Second Quarter 'From Date' should be prior or equal to its 'To Date'".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                {
                    _errorProvider.SetError(dtpQ2S, null);
                }

                DateTime SFdate = new DateTime(Convert.ToDateTime(rngcodedata.OFdate).Year, Convert.ToDateTime(rngcodedata.OFdate).Month + 3, 1);//Convert.ToDateTime(rngcodedata.OFdate);
                DateTime STdate = new DateTime(Convert.ToDateTime(dtpQ2S.Value).Year, Convert.ToDateTime(dtpQ2S.Value).Month + 2, DateTime.DaysInMonth(Convert.ToDateTime(dtpQ2S.Value).Month + 2, Convert.ToDateTime(dtpQ2S.Value).Month + 2));

                if (dtpQ2S.Value > STdate || dtpQ2S.Value < SFdate)
                {
                    _errorProvider.SetError(dtpQ2E, string.Format("Second Quarter Date Range should be in between April and June months of selected year".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                    _errorProvider.SetError(dtpQ2E, null);

                if (dtpQ2E.Value > STdate || dtpQ2E.Value < SFdate)
                {
                    _errorProvider.SetError(dtpQ2E, string.Format("Second Quarter Date Range should be in between April and June months of selected year".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                    _errorProvider.SetError(dtpQ2E, null);

                if (dtpQ3S.Value > dtpQ3E.Value)
                {
                    _errorProvider.SetError(dtpQ3S, string.Format("Third Quarter 'From Date' should be prior or equal to its 'To Date'".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                {
                    _errorProvider.SetError(dtpQ3S, null);
                }

                DateTime TFdate = new DateTime(Convert.ToDateTime(rngcodedata.OFdate).Year, Convert.ToDateTime(rngcodedata.OFdate).Month + 6, 1);
                DateTime TTdate = new DateTime(Convert.ToDateTime(dtpQ3S.Value).Year, Convert.ToDateTime(dtpQ3S.Value).Month + 2, DateTime.DaysInMonth(Convert.ToDateTime(dtpQ3S.Value).Month + 2, Convert.ToDateTime(dtpQ3S.Value).Month + 2));

                if (dtpQ3S.Value > TTdate || dtpQ3S.Value < TFdate)
                {
                    _errorProvider.SetError(dtpQ3E, string.Format("Third Quarter Date Range should be in between July and September months of selected year".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                    _errorProvider.SetError(dtpQ3E, null);

                if (dtpQ3E.Value > TTdate || dtpQ3E.Value < TFdate)
                {
                    _errorProvider.SetError(dtpQ3E, string.Format("Third Quarter Date Range should be in between July and September months of selected year".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                    _errorProvider.SetError(dtpQ3E, null);

                if (dtpQ4S.Value > dtpQ4E.Value)
                {
                    _errorProvider.SetError(dtpQ4S, string.Format("Fourth Quarter 'From Date' should be prior or equal to its 'To Date'".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                {
                    _errorProvider.SetError(dtpQ4S, null);
                }

                DateTime FoFdate = new DateTime(Convert.ToDateTime(rngcodedata.OFdate).Year, Convert.ToDateTime(rngcodedata.OFdate).Month + 9, 1);
                DateTime FoTdate = new DateTime(Convert.ToDateTime(dtpQ4S.Value).Year, Convert.ToDateTime(dtpQ4S.Value).Month + 2, DateTime.DaysInMonth(Convert.ToDateTime(dtpQ4S.Value).Month + 2, Convert.ToDateTime(dtpQ4S.Value).Month + 2));

                if (dtpQ4S.Value < FoFdate)
                {
                    _errorProvider.SetError(dtpQ4E, string.Format("Fourth Quarter Date Range should be in between October and December months of selected year".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                    _errorProvider.SetError(dtpQ4E, null);

                if (dtpQ4E.Value > FoTdate)
                {
                    _errorProvider.SetError(dtpQ4E, string.Format("Fourth Quarter Date Range should be in between October and December months of selected year".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                    _errorProvider.SetError(dtpQ4E, null);

                _errorProvider.SetError(cmbRngCode, null);
            }
            else
            {
                _errorProvider.SetError(cmbRngCode, "Please Select Code");
                Can_Generate = false;
            }
            /**if (!Ref_From_Date.Checked)
            {
                _errorProvider.SetError(Ref_From_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Reference Period 'From Date'".Replace(Consts.Common.Colon, string.Empty)));
                Can_Generate = false;
            }
            else
                _errorProvider.SetError(Ref_From_Date, null);

            if (!Ref_To_Date.Checked)
            {
                _errorProvider.SetError(Ref_To_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Reference Period 'To Date'".Replace(Consts.Common.Colon, string.Empty)));
                Can_Generate = false;
            }
            else
            {
                if (Ref_From_Date.Checked)
                    _errorProvider.SetError(Ref_To_Date, null);
            }

            if (Ref_To_Date.Checked && Ref_From_Date.Checked)
            {
                if (Ref_From_Date.Value > Ref_To_Date.Value)
                {
                    _errorProvider.SetError(Ref_To_Date, string.Format("Reference Period 'From Date' should be prior or equal to 'To Date'".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
            }
            else
            {
                if (Ref_To_Date.Checked && Ref_From_Date.Checked)
                    _errorProvider.SetError(Ref_To_Date, null);
            }


            if (!Rep_From_Date.Checked)
            {
                _errorProvider.SetError(Rep_From_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Report Period 'From' Date".Replace(Consts.Common.Colon, string.Empty)));
                Can_Generate = false;
            }
            else
                _errorProvider.SetError(Rep_From_Date, null);

            if (!Rep_To_Date.Checked)
            {
                _errorProvider.SetError(Rep_To_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Report Period 'To' Date".Replace(Consts.Common.Colon, string.Empty)));
                Can_Generate = false;
            }
            else
            {
                if (Rep_From_Date.Checked)
                    _errorProvider.SetError(Rep_To_Date, null);
            }

            if (Rep_To_Date.Checked && Rep_From_Date.Checked)
            {
                if (Rep_From_Date.Value > Rep_To_Date.Value)
                {
                    _errorProvider.SetError(Rep_From_Date, string.Format("Report Period 'From Date' should be prior or equal to 'To Date'".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
            }
            else
            {
                if (Rep_To_Date.Checked && Rep_From_Date.Checked)
                    _errorProvider.SetError(Rep_From_Date, null);
            }

            if (rdoperiodBoth.Checked || rdoperiod.Checked)
            {
                if (Ref_To_Date.Checked && Ref_From_Date.Checked && Rep_To_Date.Checked && Rep_From_Date.Checked)
                {
                    if (Ref_From_Date.Value > Rep_From_Date.Value || Ref_To_Date.Value < Rep_To_Date.Value)
                    {
                        _errorProvider.SetError(Rep_To_Date, string.Format("Report Period Should be in 'Reference Period Date Range'".Replace(Consts.Common.Colon, string.Empty)));
                        Can_Generate = false;
                    }
                    else
                        _errorProvider.SetError(Rep_To_Date, null);
                }
            }
            else if (rdoperiodCumulative.Checked)
            {
                Rep_From_Date.Value = Ref_From_Date.Value;
                Rep_To_Date.Value = Ref_To_Date.Value;
            }

            //else
            //{
            //    if (Ref_To_Date.Checked && Ref_From_Date.Checked && Rep_To_Date.Checked && Rep_From_Date.Checked)
            //        _errorProvider.SetError(Rep_From_Date, null);
            //}

            if (Rb_Fund_Sel.Checked && Sel_Funding_List.Count <= 0)
            {
                _errorProvider.SetError(Rb_Fund_Sel, string.Format("Please Select at least One 'Fund'".Replace(Consts.Common.Colon, string.Empty)));
                Can_Generate = false;
            }
            else
                _errorProvider.SetError(Rb_Fund_Sel, null);*/


            if ((Rb_SP_Sel.Checked && ListServicePlans.Count <= 0 && Scr_Oper_Mode == "RNGB0005"))
            {
                _errorProvider.SetError(Rb_SP_Sel, string.Format("Please Select at least One " + (Scr_Oper_Mode == "RNGB0005" ? "'Service Plan'" : "'ZIP Code'").Replace(Consts.Common.Colon, string.Empty)));
                Can_Generate = false;
            }
            else
                _errorProvider.SetError(Rb_SP_Sel, null);

            if (Scr_Oper_Mode == "RNGB0005" )
            {
                if (Rb_County_Sel.Checked && ListcommonEntity.Count <= 0)
                {
                    _errorProvider.SetError(Rb_County_Sel, string.Format("Please Select at least One 'County'".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                    _errorProvider.SetError(Rb_County_Sel, null);
            }

           /** if (Rb_Site_Sel.Checked && string.IsNullOrEmpty(Txt_Sel_Site.Text.Trim()))
            {
                _errorProvider.SetError(Rb_Site_Sel, string.Format("Please Select at least One 'Site'".Replace(Consts.Common.Colon, string.Empty)));
                Can_Generate = false;
            }
            else
                _errorProvider.SetError(Rb_Site_Sel, null);

            if (rdoMsselectsite.Checked && string.IsNullOrEmpty(txt_Msselect_site.Text.Trim()))
            {
                _errorProvider.SetError(rdoMsselectsite, string.Format("Please Select at least One 'Outcome Posting Site'".Replace(Consts.Common.Colon, string.Empty)));
                Can_Generate = false;
            }
            else
                _errorProvider.SetError(rdoMsselectsite, null);*/

            if (string.IsNullOrEmpty(Txt_Pov_Low.Text.Trim()))
            {
                _errorProvider.SetError(Txt_Pov_Low, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Poverty Level 'Low'".Replace(Consts.Common.Colon, string.Empty)));
                Can_Generate = false;
            }
            else
                _errorProvider.SetError(Txt_Pov_Low, null);

            if (string.IsNullOrEmpty(Txt_Pov_High.Text.Trim()))
            {
                _errorProvider.SetError(Txt_Pov_High, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), "Poverty Level 'High'".Replace(Consts.Common.Colon, string.Empty)));
                Can_Generate = false;
            }
            else
                _errorProvider.SetError(Txt_Pov_High, null);

            if (!string.IsNullOrEmpty(Txt_Pov_Low.Text.Trim()) && !string.IsNullOrEmpty(Txt_Pov_High.Text.Trim()))
            {
                if (int.Parse(Txt_Pov_Low.Text) > int.Parse(Txt_Pov_High.Text))
                {
                    _errorProvider.SetError(Txt_Pov_High, string.Format("Poverty Level 'Low' should not Exceed 'High'".Replace(Consts.Common.Colon, string.Empty)));
                    Can_Generate = false;
                }
                else
                    _errorProvider.SetError(Txt_Pov_High, null);
            }
            if (((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString() == "**")
            {
                _errorProvider.SetError(cmbRngCode, "Please Select Code");
                Can_Generate = false;
            }
            else
            {
                _errorProvider.SetError(cmbRngCode, null);
            }
            return Can_Generate;
        }






        string RemoveBetween(string s, char begin, char end)
        {
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(s, string.Empty);
        }

        SO_Browse_Entity Search_Entity = new SO_Browse_Entity(true);
        PM_Browse_Entity PM_Search_Entity = new PM_Browse_Entity(true);
        string[] Sel_params_To_Print = new string[20] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
        private void Get_Selection_Criteria()
        {
            Sel_params_To_Print[4] = string.Empty;
            switch (Scr_Oper_Mode)
            {
                case "RNGB0005":
                    if (Rb_SP_All.Checked)
                        Search_Entity.ZipCode = "**";
                    else
                    {
                        string strGrpcode = Get_Sel_SPCodes();
                        //if (strGrpcode.Trim() == "'7'")
                        //    Search_Entity.ZipCode = "**";
                        //else
                            Search_Entity.ZipCode = strGrpcode;
                    }

                    break;
                //case "RNGB0014":
                //    //Search_Entity.Attribute = "All";
                //    //if (Rb_User_Def.Checked)
                //    //    Search_Entity.Attribute = "Goal";

                //    if (Rb_SP_All.Checked)
                //        Search_Entity.ZipCode = "**";
                //    else
                //    {
                //        string strGrpcode = Get_Sel_GroupCodes();
                //        if (strGrpcode.Trim() == "'7'")
                //            Search_Entity.ZipCode = "**";
                //        else
                //            Search_Entity.ZipCode = strGrpcode;
                //    }

                //    Search_Entity.County = "**";
                //    if (Rb_County_Sel.Checked)
                //        Search_Entity.County = Get_Sel_County();

                //    /**Search_Entity.PM_Rep_Format = "PM";
                //    if (Rb_SNP_Mem.Checked)
                //        Search_Entity.PM_Rep_Format = "Both";
                //    else if (rbo_ProgramWise.Checked)
                //        Search_Entity.PM_Rep_Format = "Prog";*/

                //    Search_Entity.DG_Count_Sw = Search_Entity.PM_Rep_Format;

                //    break;
            }

            Search_Entity.RngMainCode = ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString();
            Search_Entity.Ms_DriveColumn_Sw = "CADATE";
            if (Rb_MS_AddDate.Checked)
                Search_Entity.Ms_DriveColumn_Sw = "CAADDDATE";

            Search_Entity.CA_MS_Sw = "MS";


            /** Search_Entity.Rep_From_Date = Convert.ToDateTime(Ref_From_Date.Value.ToString()).ToString("MM/dd/yyyy");//Ref_From_Date.Value.ToShortDateString();
             Search_Entity.Rep_To_Date = Convert.ToDateTime(Ref_To_Date.Value.ToString()).ToString("MM/dd/yyyy");//Ref_To_Date.Value.ToShortDateString();
             if (rdoperiod.Checked)
             {
                 Search_Entity.Rep_From_Date = Convert.ToDateTime(Rep_From_Date.Value.ToString()).ToString("MM/dd/yyyy"); //Rep_From_Date.Value.ToShortDateString();
                 Search_Entity.Rep_To_Date = Convert.ToDateTime(Rep_To_Date.Value.ToString()).ToString("MM/dd/yyyy"); //Rep_To_Date.Value.ToShortDateString();
             }
             Search_Entity.Rep_Period_FDate = Convert.ToDateTime(Rep_From_Date.Value.ToString()).ToString("MM/dd/yyyy"); //Rep_From_Date.Value.ToShortDateString();
             Search_Entity.Rep_Period_TDate = Convert.ToDateTime(Rep_To_Date.Value.ToString()).ToString("MM/dd/yyyy"); //Rep_To_Date.Value.ToShortDateString();*/

            /* if (rdoperiodCumulative.Checked)
             {
                 if (Rb_SNP_Mem.Checked || rbo_ProgramWise.Checked)
                 {
                     Search_Entity.Rep_Period_FDate = Convert.ToDateTime(Ref_From_Date.Value.ToString()).ToString("MM/dd/yyyy"); //Ref_From_Date.Value.ToShortDateString();
                     Search_Entity.Rep_Period_TDate = Convert.ToDateTime(Ref_To_Date.Value.ToString()).ToString("MM/dd/yyyy"); //Ref_To_Date.Value.ToShortDateString();
                 }
             }*/

            Search_Entity.Q1_START_DATE= Convert.ToDateTime(dtpQ1S.Value.ToString()).ToString("MM/dd/yyyy");
            Search_Entity.Q1_END_DATE = Convert.ToDateTime(dtpQ1E.Value.ToString()).ToString("MM/dd/yyyy");
            Search_Entity.Q2_START_DATE = Convert.ToDateTime(dtpQ2S.Value.ToString()).ToString("MM/dd/yyyy");
            Search_Entity.Q2_END_DATE = Convert.ToDateTime(dtpQ2E.Value.ToString()).ToString("MM/dd/yyyy");
            Search_Entity.Q3_START_DATE = Convert.ToDateTime(dtpQ3S.Value.ToString()).ToString("MM/dd/yyyy");
            Search_Entity.Q3_END_DATE = Convert.ToDateTime(dtpQ3E.Value.ToString()).ToString("MM/dd/yyyy");
            Search_Entity.Q4_START_DATE = Convert.ToDateTime(dtpQ4S.Value.ToString()).ToString("MM/dd/yyyy");
            Search_Entity.Q4_END_DATE = Convert.ToDateTime(dtpQ4E.Value.ToString()).ToString("MM/dd/yyyy");

            Search_Entity.Mst_CaseType_Sw = ((Captain.Common.Utilities.ListItem)Cmb_CaseType.SelectedItem).Value.ToString();  //"**"; 

            Search_Entity.Stat_Detail = "N";
            if (Rb_Details_Yes.Checked)
                Search_Entity.Stat_Detail = "Y";

            Search_Entity.Mst_Secret_Sw = "B";
            if (Rb_Mst_Sec.Checked)
                Search_Entity.Mst_Secret_Sw = "Y";
            else
                if (Rb_Mst_NonSec.Checked)
                Search_Entity.Mst_Secret_Sw = "N";

            Search_Entity.Mst_Acive_Sw = "Y";
            if (Rb_Stat_InAct.Checked)
                Search_Entity.Mst_Acive_Sw = "N";
            else
                if (Rb_Stat_Both.Checked)
                Search_Entity.Mst_Acive_Sw = "B";

           /** Search_Entity.Mst_Site = "**";
            if (Rb_Site_Sel.Checked)
                Search_Entity.Mst_Site = Get_Sel_Sites();
            else if (Rb_Site_No.Checked)
                Search_Entity.Mst_Site = "NO";


            Search_Entity.CaseMssite = "**";
            if (rdoMsselectsite.Checked)
                Search_Entity.CaseMssite = Get_Sel_CasemsSites();
            else if (rdomsNosite.Checked)
                Search_Entity.CaseMssite = "NO";

            Search_Entity.CA_Fund_Filter = "**";
            if (Rb_Fund_Sel.Checked)
                Search_Entity.CA_Fund_Filter = Get_Sel_CA_Fund_List_To_Filter();*/


            Search_Entity.Mst_Poverty_Low = Txt_Pov_Low.Text;
            Search_Entity.Mst_Poverty_High = Txt_Pov_High.Text;

            Search_Entity.Hierarchy = Current_Hierarchy + (CmbYear.Visible ? ((Captain.Common.Utilities.ListItem)CmbYear.SelectedItem).Value.ToString() : string.Empty);
            Search_Entity.Activity_Prog = "A";
            Sel_Permesures_Programs = "";
            //if (Cmb_Program.SelectedItem != null)
            //{
            //    string Tmp_Hie_On_Porgram = ((Captain.Common.Utilities.ListItem)Cmb_Program.SelectedItem).Value.ToString();
            //    if (((Captain.Common.Utilities.ListItem)Cmb_Program.SelectedItem).Value.ToString() == "**")
            //    {

            //        Search_Entity.Activity_Prog = Tmp_Hie_On_Porgram = "******";
            //    }

            //    Sel_Permesures_Programs = Search_Entity.Activity_Prog = Tmp_Hie_On_Porgram;
            //}

            /**string Sel_Programs = string.Empty;
            if (rbSelProgram.Checked == true)
            {
                if (SelectedHierarchies.Count > 0)
                {
                    foreach (HierarchyEntity Entity in SelectedHierarchies)
                    {
                        Sel_Programs += Entity.Agency + Entity.Dept + Entity.Prog + ",";
                    }

                    if (Sel_Programs.Length > 0)
                        Sel_Programs = Sel_Programs.Substring(0, (Sel_Programs.Length - 1));

                    if (Sel_Programs.Length > 0)
                        Sel_Permesures_Programs = Search_Entity.Activity_Prog = Sel_Programs;
                }
            }*/

            Search_Entity.UserName = BaseForm.UserID;

            switch (Search_Entity.Mst_Secret_Sw)
            {
                case "Y": Sel_params_To_Print[0] = "Secret Only"; break;
                case "N": Sel_params_To_Print[0] = "Non Secret Only"; break;
                default: Sel_params_To_Print[0] = "Both Secret and Non Secret "; break;
            }
            switch (Search_Entity.Mst_Acive_Sw)
            {
                case "Y": Sel_params_To_Print[1] = "Active"; break;
                case "N": Sel_params_To_Print[1] = "Inactive"; break;
                default: Sel_params_To_Print[1] = "Both Active and Inactive"; break;
            }
            //switch (Search_Entity.Mst_Site)
            //{
            //    case "**": Sel_params_To_Print[2] = "All Sites"; break;
            //    case "NO": Sel_params_To_Print[2] = "No Sites"; break;
            //    default: Sel_params_To_Print[2] = "Selected Site"; break;
            //}
            //switch (Search_Entity.CaseMssite)
            //{
            //    case "**": Sel_params_To_Print[3] = "ALL MS Sites"; break;
            //    case "NO": Sel_params_To_Print[3] = "No MS Sites"; break;
            //    default: Sel_params_To_Print[3] = "Selected MS Site"; break;
            //}
           //** Search_Entity.OutComeSwitch = rdoOutcomesAll.Checked == true ? "A" : "S";
            Search_Entity.IncVerSwitch = chkincverswitch.Checked == true ? "Y" : "N";
        }

       
        private string Get_Sel_SPCodes()
        {
            string Sel_Groups_Codes = string.Empty;
            foreach (CASESP1Entity Entity in ListServicePlans)
            {
                Sel_Groups_Codes += "'" + Entity.Code + "' ,";
            }

            if (Sel_Groups_Codes.Length > 0)
                Sel_Groups_Codes = Sel_Groups_Codes.Substring(0, (Sel_Groups_Codes.Length - 1));

            return Sel_Groups_Codes;
        }

        List<RCsb14GroupEntity> OutCome_MasterList = new List<RCsb14GroupEntity>();
        private void Prepare_Selected_Group(string Sel_ZipCodes)
        {
            ListRngGroupCode.Clear();
            OutCome_MasterList = _model.SPAdminData.Browse_RNGGrp(((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), null, null, null, null, BaseForm.UserID, BaseForm.BaseAgency);
            foreach (RCsb14GroupEntity Entity in OutCome_MasterList)
            {
                if (Sel_ZipCodes.Contains(Entity.GrpCode.Trim()))
                    ListRngGroupCode.Add(new RCsb14GroupEntity(Entity));
            }
        }


        string Sel_AGY, Sel_DEPT, Sel_PROG = string.Empty;
        string Sel_Secret_App, Sel_Group_Sort, Sel_Use_CASEDIFF, Sel_Include_Members = string.Empty, Sel_Permesures_Programs = "";
        private void Get_Report_Selection_Parameters()
        {
            Sel_AGY = Current_Hierarchy.Substring(0, 2);
            Sel_DEPT = Current_Hierarchy.Substring(2, 2);
            Sel_PROG = Current_Hierarchy.Substring(4, 2);

            //switch (((ListItem)Cmb_Applications.SelectedItem).Value.ToString())
            //{
            //    case "1": Sel_Secret_App = "Non-Secret Only"; break;
            //    case "2": Sel_Secret_App = "Secret Only"; break;
            //    case "3": Sel_Secret_App = "Both Secret & Non-Secret"; break;
            //}

            //switch (((ListItem)Cmb_Group_Sort.SelectedItem).Value.ToString())
            //{
            //    case "ASC": Sel_Group_Sort = "Sort Fields : Ascending"; break;
            //    case "DESC": Sel_Group_Sort = "Sort Fields : Descending"; break;
            //}

            //Sel_Use_CASEDIFF = "Use CASEDIFF : No";
            //if (Cb_Use_DIFF.Checked)
            //    Sel_Use_CASEDIFF = "Use CASEDIFF : Yes";


            //Sel_Include_Members = "Include All Members : No";
            //if (Cb_Inc_Menbers.Checked)
            //    Sel_Include_Members = "Include All Members : Yes";
        }

        private void Btn_Save_Params_Click(object sender, EventArgs e)
        {
            if (Validate_Report())
            {
                ControlCard_Entity Save_Entity = new ControlCard_Entity(true);

                Get_Selection_Criteria();
                //Save_Entity.Scr_Code = PrivilegeEntity.Program;
                Save_Entity.Scr_Code = Scr_Oper_Mode;
                Save_Entity.UserID = BaseForm.UserID;
                Save_Entity.Card_1 = Get_XML_Format_for_Report_Controls();
                Save_Entity.Card_2 = Save_Entity.Card_3 = null;
                Save_Entity.Module = BaseForm.BusinessModuleID;

                Report_Get_SaveParams_Form Save_Form = new Report_Get_SaveParams_Form(Save_Entity, "Save", BaseForm, PrivilegeEntity);
                Save_Form.StartPosition = FormStartPosition.CenterScreen;
                Save_Form.ShowDialog();
            }

        }

        private string Get_XML_Format_for_Report_Controls()   // 12012012
        {

            /**string strrptfor = "R";
            if (rdoperiodCumulative.Checked)
                strrptfor = "C";
            if (rdoperiodBoth.Checked)
                strrptfor = "B";

            string strRepControl = "N";
            if (chkRepControl.Checked == true)
                strRepControl = "Y";*/

            /**** string SiteType = string.Empty;
             if (Rb_Site_Sel.Checked == true)
                 SiteType = Search_Entity.Mst_Site;
             else if (rdoMsselectsite.Checked == true)
                 SiteType = Search_Entity.Mst_Site;*/

            StringBuilder str = new StringBuilder();
            str.Append("<Rows>");

            str.Append("<Row AGENCY = \"" + Current_Hierarchy_DB.Substring(0, 2) + "\" DEPT = \"" + Current_Hierarchy_DB.Substring(3, 2) + "\" PROG = \"" + Current_Hierarchy_DB.Substring(6, 2) +
                                      "\" CASE_TYPE = \"" + Search_Entity.Mst_CaseType_Sw + "\" CASE_STATUS = \"" + Search_Entity.Mst_Acive_Sw + "\" DATE_SELECTION = \"" + Search_Entity.Ms_DriveColumn_Sw +
                                      "\" REFERENCE_FROM_DATE = \"" + Search_Entity.Rep_From_Date + "\" REFERENCE_TO_DATE = \"" + Search_Entity.Rep_To_Date + "\" REPORT_FROM_DATE = \"" + Search_Entity.Rep_Period_FDate +
                                      "\" REPORT_TO_DATE = \"" + Search_Entity.Rep_Period_TDate + "\" SITE = \"" + /*SiteType*/ Search_Entity.Mst_Site + "\" ATTRIBUTES = \"" + Search_Entity.Attribute + "\" STAT_DETAILS = \"" + Search_Entity.Stat_Detail +
                                      "\" POVERTY_LOW = \"" + Search_Entity.Mst_Poverty_Low + "\" POVERTY_HIGH = \"" + Search_Entity.Mst_Poverty_High + "\" ZIPCODE = \"" + Search_Entity.ZipCode +
                                      "\" COUNTY = \"" + Search_Entity.County + "\" DG_COUNT = \"" + Search_Entity.DG_Count_Sw + "\" SECRET_APP = \"" + Search_Entity.Mst_Secret_Sw + "\" ACTY_PROGRAM = \"" + Search_Entity.Activity_Prog +
                                      "\" CAMS_SW = \"" + Search_Entity.CA_MS_Sw + "\" CAMS_Filter_List = \"" + Search_Entity.CAMS_Filter + "\" FUND_Filter_List = \"" + Search_Entity.CA_Fund_Filter + "\" RNG_MAIN_CODE = \"" + Search_Entity.RngMainCode + "\" OUTCOME_SWITCH = \"" + "\" CASEMSSITE = \"" + Search_Entity.CaseMssite + "\" INCVERSWITCH = \"" + Search_Entity.IncVerSwitch + "\" REPORTFOR =\""  + "\" REPORTCONTROL =\"" + "\" />"); //
           /** //switch (Scr_Oper_Mode)
            //{
            //    case "CASB0004":
            //        str.Append("<Row AGENCY = \"" + Current_Hierarchy_DB.Substring(0, 2) + "\" DEPT = \"" + Current_Hierarchy_DB.Substring(3, 2) + "\" PROG = \"" + Current_Hierarchy_DB.Substring(6, 2) +
            //                                  "\" CASE_TYPE = \"" + Search_Entity.Mst_CaseType_Sw + "\" CASE_STATUS = \"" + Search_Entity.Mst_Acive_Sw + "\" DATE_SELECTION = \"" + Search_Entity.Ms_DriveColumn_Sw +
            //                                  "\" REFERENCE_FROM_DATE = \"" + Search_Entity.Rep_From_Date + "\" REFERENCE_TO_DATE = \"" + Search_Entity.Rep_To_Date + "\" REPORT_FROM_DATE = \"" + Search_Entity.Rep_Period_FDate +
            //                                  "\" REPORT_TO_DATE = \"" + Search_Entity.Rep_Period_TDate + "\" SITE = \"" + Search_Entity.Mst_Site + "\" ATTRIBUTES = \"" + Search_Entity.Attribute + "\" STAT_DETAILS = \"" + Search_Entity.Stat_Detail +
            //                                  "\" POVERTY_LOW = \"" + Search_Entity.Mst_Poverty_Low + "\" POVERTY_HIGH = \"" + Search_Entity.Mst_Poverty_High + "\" ZIPCODE = \"" + Search_Entity.ZipCode +
            //                                  "\" COUNTY = \"" + Search_Entity.County + "\" DG_COUNT = \"" + Search_Entity.DG_Count_Sw + "\" SECRET_APP = \"" + Search_Entity.Mst_Secret_Sw + "\" />");
            //        break;

            //    case "RNGB0014":
            //        str.Append("<Row AGENCY = \"" + Current_Hierarchy_DB.Substring(0, 2) + "\" DEPT = \"" + Current_Hierarchy_DB.Substring(3, 2) + "\" PROG = \"" + Current_Hierarchy_DB.Substring(6, 2) +
            //                                  "\" CASE_TYPE = \"" + Search_Entity.Mst_CaseType_Sw + "\" CASE_STATUS = \"" + Search_Entity.Mst_Acive_Sw + "\" DATE_SELECTION = \"" + Search_Entity.Ms_DriveColumn_Sw +
            //                                  "\" REFERENCE_FROM_DATE = \"" + Search_Entity.Rep_From_Date + "\" REFERENCE_TO_DATE = \"" + Search_Entity.Rep_To_Date + "\" REPORT_FROM_DATE = \"" + Search_Entity.Rep_Period_FDate +
            //                                  "\" REPORT_TO_DATE = \"" + Search_Entity.Rep_Period_TDate + "\" SITE = \"" + Search_Entity.Mst_Site + "\" ATTRIBUTES = \"" + Search_Entity.Attribute + "\" STAT_DETAILS = \"" + Search_Entity.Stat_Detail +
            //                                  "\" POVERTY_LOW = \"" + Search_Entity.Mst_Poverty_Low + "\" POVERTY_HIGH = \"" + Search_Entity.Mst_Poverty_High + "\" ZIPCODE = \"" + Search_Entity.ZipCode +
            //                                  "\" COUNTY = \"" + Search_Entity.County + "\" SECRET_APP = \"" + Search_Entity.Mst_Secret_Sw + "\" />");
            //        break;
            //}*/
            str.Append("</Rows>");

            return str.ToString();
        }

        private void Btn_Get_Params_Click(object sender, EventArgs e)
        {
            ControlCard_Entity Save_Entity = new ControlCard_Entity(true);
            //Save_Entity.Scr_Code = PrivilegeEntity.Program;
            Save_Entity.Scr_Code = Scr_Oper_Mode;
            Save_Entity.UserID = BaseForm.UserID;
            Save_Entity.Module = BaseForm.BusinessModuleID;
            Report_Get_SaveParams_Form Save_Form = new Report_Get_SaveParams_Form(Save_Entity, "Get");
            Save_Form.FormClosed += new FormClosedEventHandler(Get_Saved_Parameters);
            Save_Form.StartPosition = FormStartPosition.CenterScreen;
            Save_Form.ShowDialog();

        }

        private void Get_Saved_Parameters(object sender, FormClosedEventArgs e)
        {
            Report_Get_SaveParams_Form form = sender as Report_Get_SaveParams_Form;
            string[] Saved_Parameters = new string[2];
            Saved_Parameters[0] = Saved_Parameters[1] = string.Empty;

            if (form.DialogResult == DialogResult.OK)
            {
                Clear_Error_Providers();

                DataTable RepCntl_Table = new DataTable();
                Saved_Parameters = form.Get_Adhoc_Saved_Parameters();

                RepCntl_Table = CommonFunctions.Convert_XMLstring_To_Datatable(Saved_Parameters[0]);
                Set_Report_Controls(RepCntl_Table);
            }
        }


        private void Set_Report_Controls(DataTable Tmp_Table)
        {
            if (Tmp_Table != null && Tmp_Table.Rows.Count > 0)
            {
                DataRow dr = Tmp_Table.Rows[0];
                DataColumnCollection columns = Tmp_Table.Columns;

                Set_Report_Hierarchy(dr["AGENCY"].ToString(), dr["DEPT"].ToString(), dr["PROG"].ToString(), string.Empty);
                fillRngCode();
                SetComboBoxValue(Cmb_CaseType, dr["CASE_TYPE"].ToString());

                switch (dr["CASE_STATUS"].ToString())
                {
                    case "Y": Rb_Stat_Act.Checked = true; break;
                    case "N": Rb_Stat_InAct.Checked = true; break;
                    case "B": Rb_Stat_Both.Checked = true; break;
                    default: Rb_Stat_Act.Checked = true; break;
                }

               /** Txt_Sel_Site.Clear();
                switch (dr["SITE"].ToString())
                {
                    case "**": Rb_Site_All.Checked = true; break;
                    case "NO": Rb_Site_No.Checked = true; break;
                    default:
                        Rb_Site_Sel.Checked = true; Txt_Sel_Site.Text = dr["SITE"].ToString();
                        Site_List = _model.CaseMstData.GetCaseSite(Current_Hierarchy.Substring(0, 2), Current_Hierarchy.Substring(2, 2), Current_Hierarchy.Substring(4, 2), "SiteHie");
                        ListcaseSiteEntity.Clear();
                        foreach (CaseSiteEntity casesite in Site_List) //Site_List)//ListcaseSiteEntity)
                        {
                            if (Txt_Sel_Site.Text.Contains(casesite.SiteNUMBER))
                                ListcaseSiteEntity.Add(casesite);
                            // Sel_Site_Codes += "'" + casesite.SiteNUMBER + "' ,";
                        }


                        break;
                }*/

                if (columns.Contains("INCVERSWITCH"))
                {
                    chkincverswitch.Checked = dr["INCVERSWITCH"].ToString() == "Y" ? true : false;
                }
               /** if (columns.Contains("CASEMSSITE"))
                {
                    txt_Msselect_site.Clear();
                    switch (dr["CASEMSSITE"].ToString())
                    {
                        case "**": rdoMssiteall.Checked = true; break;
                        case "NO": rdomsNosite.Checked = true; break;
                        default:
                            rdoMsselectsite.Checked = true; txt_Msselect_site.Text = dr["CASEMSSITE"].ToString();
                            // MSSite_List = _model.CaseMstData.GetCaseSite(Current_Hierarchy.Substring(0, 2), Current_Hierarchy.Substring(2, 2), Current_Hierarchy.Substring(4, 2), "SiteHie");
                            ListcaseMsSiteEntity.Clear();
                            foreach (CaseSiteEntity casesite in Site_List) //Site_List)//ListcaseSiteEntity)
                            {
                                if (txt_Msselect_site.Text.Contains(casesite.SiteNUMBER))
                                    ListcaseMsSiteEntity.Add(casesite);
                                // Sel_Site_Codes += "'" + casesite.SiteNUMBER + "' ,";
                            }


                            break;
                    }
                }*/


               /** Ref_From_Date.Value = Convert.ToDateTime(dr["REFERENCE_FROM_DATE"].ToString());
                Ref_To_Date.Value = Convert.ToDateTime(dr["REFERENCE_TO_DATE"].ToString());
                Rep_From_Date.Value = Convert.ToDateTime(dr["REPORT_FROM_DATE"].ToString());
                Rep_To_Date.Value = Convert.ToDateTime(dr["REPORT_TO_DATE"].ToString());*/

               /** if (columns.Contains("FUND_Filter_List"))
                {
                    Sel_Funding_List.Clear();
                    if (dr["FUND_Filter_List"].ToString().Trim() == "**")
                        Rb_Fund_All.Checked = true;
                    else
                    {
                        GetParam_Fund_Filter_List(dr["FUND_Filter_List"].ToString());
                        this.Rb_Fund_Sel.Click -= new System.EventHandler(this.Rb_Fund_Sel_CheckedChanged);
                        Rb_Fund_Sel.Checked = true;
                        this.Rb_Fund_Sel.Click += new System.EventHandler(this.Rb_Fund_Sel_CheckedChanged);
                    }

                }*/

               /** if (columns.Contains("REPORTFOR"))
                {
                    rdoperiod.Checked = true;
                    //if (dr["REPORTCONTROL"].ToString() == "R")
                    //    rdoperiod.Checked = true;
                    if (dr["REPORTFOR"].ToString() == "C")
                        rdoperiodCumulative.Checked = true;
                    if (dr["REPORTFOR"].ToString() == "B")
                        rdoperiodBoth.Checked = true;
                    reportforselection();

                }*/

                if (dr["DATE_SELECTION"].ToString() == "MSDATE")
                    Rb_MS_Date.Checked = true;
                else
                    Rb_MS_AddDate.Checked = true;

                //if ((dr["ATTRIBUTES"].ToString() == "SYSTEM" && Scr_Oper_Mode == "RNGB0004") ||
                //    (dr["ATTRIBUTES"].ToString() == "All" && Scr_Oper_Mode == "RNGB0014"))
                //    Rb_Agy_Def.Checked = true;
                //else
                //    Rb_User_Def.Checked = true;

                if (dr["STAT_DETAILS"].ToString() == "Y")
                    Rb_Details_Yes.Checked = true;
                else
                    Rb_Details_No.Checked = true;

                Txt_Pov_Low.Text = dr["POVERTY_LOW"].ToString();
                Txt_Pov_High.Text = dr["POVERTY_HIGH"].ToString();


                if ((dr["COUNTY"].ToString() == "**" && Scr_Oper_Mode == "RNGB0004") ||
                    (dr["COUNTY"].ToString() == "**" && Scr_Oper_Mode == "RNGB0014"))
                    Rb_County_All.Checked = true;
                else
                {
                    Rb_County_Sel.Checked = true;
                    Fill_County_Selected_List(dr["COUNTY"].ToString().Trim());
                }


                //if ((dr["COUNTY"].ToString() == "PM" && Scr_Oper_Mode == "RNGB0014"))
                //    Rb_OBO_Mem.Checked = true;
                //else if ((dr["COUNTY"].ToString() == "Both" && Scr_Oper_Mode == "RNGB0014"))
                //{
                //    Rb_SNP_Mem.Checked = true;
                //}
                //else if ((dr["COUNTY"].ToString() == "Prog" && Scr_Oper_Mode == "RNGB0014"))
                //{
                //    rbo_ProgramWise.Checked = true;
                //}


               /** if (dr["DG_COUNT"].ToString() == "PM")
                    Rb_OBO_Mem.Checked = true;
                else if (dr["DG_COUNT"].ToString() == "Prog")
                    rbo_ProgramWise.Checked = true;
                else
                    Rb_SNP_Mem.Checked = true;*/


                switch (dr["SECRET_APP"].ToString())
                {
                    case "Y": Rb_Mst_Sec.Checked = true; break;
                    case "N": Rb_Mst_NonSec.Checked = true; break;
                    default: Rb_Mst_BothSec.Checked = true; break;
                }


                //Fill_Program_Combo();
                //SetComboBoxValue(Cmb_Program, dr["ACTY_PROGRAM"].ToString().Trim() == "******" ? "**" : dr["ACTY_PROGRAM"].ToString());

                /**if (dr["ACTY_PROGRAM"].ToString() != "A")
                {

                    rbAllPrograms.Checked = true;
                    //Btn_MS_Selection.Visible = false;
                    GetSelectedHierarchies(dr["ACTY_PROGRAM"].ToString());
                    if (SelectedHierarchies.Count > 0)
                    {
                        rbSelProgram.Checked = true;
                        //All_CAMS_Selected = false;
                        //Btn_CA_Selection.Text = "Sel";
                    }
                }*/

                SetComboBoxValue(cmbRngCode, dr["RNG_MAIN_CODE"].ToString().Trim());

                /**Ref_From_Date.Value = Convert.ToDateTime(dr["REFERENCE_FROM_DATE"].ToString());
                Ref_To_Date.Value = Convert.ToDateTime(dr["REFERENCE_TO_DATE"].ToString());*/

                if (dr["ZIPCODE"].ToString() == "**")
                    Rb_SP_All.Checked = true;
                else
                {
                    Rb_SP_Sel.Checked = true;
                    if (Scr_Oper_Mode == "RNGB0014")
                        Prepare_Selected_Group(dr["ZIPCODE"].ToString());
                }
               /** if (columns.Contains("OUTCOME_SWITCH"))
                {
                    if (dr["OUTCOME_SWITCH"].ToString() == "S")
                        rdoOutcomesselect.Checked = true;
                    else
                        rdoOutcomesAll.Checked = true;
                }*/


               /** if (columns.Contains("REPORTCONTROL"))
                {
                    if (dr["REPORTCONTROL"].ToString() == "Y")
                        chkRepControl.Checked = true;
                    else
                        chkRepControl.Checked = false;
                }*/

                Get_Selection_Criteria();
            }
        }

        private void Fill_County_Selected_List(string County_Str)
        {
            foreach (CommonEntity Ent in County_List)
            {
                if (County_Str.Contains("'" + Ent.Code + "'"))
                    ListcommonEntity.Add(new CommonEntity(Ent));
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
                    { comboBox.SelectedItem = li; break; }
                }
            }
        }

        private void Txt_Pov_High_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Txt_Pov_High.Text.Trim()))
                Txt_Pov_High.Text = "000";
        }

        private void Txt_Pov_Low_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Txt_Pov_Low.Text.Trim()))
                Txt_Pov_Low.Text = "000";
        }


      
        string Scr_Oper_Mode = "RNGB0005";


        private void Initialize_All_Controls()
        {


            Scr_Oper_Mode = "RNGB0014";
            //20160303

            this.Text = "RNGB0014 - Performance Measures";

            lblDateSelection.Text = "Process Report By";
            //lblAttributes.Text = "Categories";
            //Rb_Agy_Def.Text = "All";
            //Rb_User_Def.Text = "Only Goal Associated";

            lblProduceStatistical.Text = "Produce Details";

            Rb_Details_No.Text = "No";
            Rb_Details_Yes.Text = "Yes";

            lblServiceplan.Text = "Group";
            Rb_SP_All.Text = "All Groups";
            Rb_SP_Sel.Text = "Selected Groups";

            //lblCounty.Text = "Report Format";
            //Rb_County_All.Text = "Performance Measures Only";
            //Rb_County_Sel.Text = "Performance Measures + Goal Details";

            Rb_MS_Date.Text = "Outcome Date";//"Milestone Date";
            Rb_MS_AddDate.Text = "Outcome Add Date";//"Milestone Add Date";

          /**  lblDemographicsCount.Text = "Report Format";
            Rb_OBO_Mem.Text = "Performance Measures Only";
            Rb_SNP_Mem.Text = "Performance Measures + Goal Details";
            rbo_ProgramWise.Text = "Program Level Counts";
            Rb_SNP_Mem.Location = new System.Drawing.Point(154, 2);*/


            /**Lbl_Program.Visible = true; //Cmb_Program.Visible = true;
            Lbl_Program.Location = new System.Drawing.Point(5, 36);*/
            //Cmb_Program.Location = new System.Drawing.Point(143, 36);
            //this.Cmb_Program.Size = new System.Drawing.Size(249, 21);

            this.Date_Panel.Location = new System.Drawing.Point(0, -1);
            this.Service_Panel.Size = new System.Drawing.Size(450, 21);

            //this.panel8.Location = new System.Drawing.Point(0, 48);


            this.pnlfewrdb.Location = new System.Drawing.Point(0, 48);
            //this.panel8.Size = new System.Drawing.Size(607, 224); //20160303
            this.pnlfewrdb.Size = new System.Drawing.Size(607, 247);

            //this.panel4.Location = new System.Drawing.Point(-1, 33);
            //**this.pnlMultiRdb.Location = new System.Drawing.Point(-1, 55);

            //**Rb_SNP_Mem.Size = new System.Drawing.Size(230, 21);

            //**this.pnlMultiRdb.Size = new System.Drawing.Size(607, 296);
            //this.panel3.Location = new System.Drawing.Point(4, 385); //20160303
            //this.panel2.Size = new System.Drawing.Size(607, 352); //20160303

            //this.Size = new System.Drawing.Size(613, 395);

            //this.Size = new System.Drawing.Size(615, 422);
            //Fill_Program_Combo();


            Cmb_CaseType.SelectedIndex = 0;
            Rb_MS_Date.Checked = Rb_Stat_Both.Checked //**= Rb_Site_All.Checked = rdoMssiteall.Checked
            = Rb_Details_No.Checked = Rb_SP_All.Checked = Rb_County_All.Checked =
            Rb_Mst_NonSec.Checked = true;

            All_CAMS_Selected = true;
            Sel_MS_List.Clear();
            Sel_CA_List.Clear();

            switch (Scr_Oper_Mode)
            {
                case "RNGB0004":
                    //Rb_User_Def.Checked = true;//** Rb_OBO_Mem.Checked = true;
                   /** Ref_From_Date.Value = new DateTime(DateTime.Now.Year, 1, 1);// Convert.ToDateTime("01/01/" + DateTime.Now.Year);
                    Ref_To_Date.Value = new DateTime(DateTime.Now.Year, 12, 31);// Convert.ToDateTime("12/31/" + DateTime.Now.Year);*/
                    break;
                case "RNGB0014":
                    //Rb_Agy_Def.Checked = true; //**rbAllPrograms.Checked = true; //Fill_Program_Combo();
                    //**HierarchyGrid.Rows.Clear(); 
                    //**SelectedHierarchies.Clear();
                   /** if (Sys_DateRange_List.Count > 0)
                    {
                        Ref_From_Date.Value = Convert.ToDateTime(Sys_DateRange_List[0].REF_FDATE);// Convert.ToDateTime("01/01/" + DateTime.Now.Year);
                        Ref_To_Date.Value = Convert.ToDateTime(Sys_DateRange_List[0].REF_TDATE);// Convert.ToDateTime("12/31/" + DateTime.Now.Year);
                    }
                    else
                    {
                        Ref_From_Date.Value = new DateTime(DateTime.Now.Year, 1, 1);// Convert.ToDateTime("01/01/" + DateTime.Now.Year);
                        Ref_To_Date.Value = new DateTime(DateTime.Now.Year, 12, 31);// Convert.ToDateTime("12/31/" + DateTime.Now.Year);
                    }*/
                    break;
            }

           /** Rep_From_Date.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            Rep_To_Date.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Month, DateTime.Today.Month));

            Ref_From_Date.Checked = Ref_To_Date.Checked = Rep_From_Date.Checked = Rep_To_Date.Checked = true;*/

            Txt_Pov_Low.Text = "0"; Txt_Pov_High.Text = "999";


            ListcaseSiteEntity.Clear();
            ListcaseMsSiteEntity.Clear();
            ListcommonEntity.Clear();
            ListZipCode.Clear();
            ListRngGroupCode.Clear();
            Clear_Error_Providers();
        }

        private void Clear_Error_Providers()
        {
            //**_errorProvider.SetError(Ref_To_Date, null);
            _errorProvider.SetError(Rb_SP_Sel, null);
            _errorProvider.SetError(Rb_County_Sel, null);
            //**_errorProvider.SetError(Txt_Sel_Site, null);
            //**_errorProvider.SetError(txt_Msselect_site, null);
            _errorProvider.SetError(Txt_Pov_Low, null);
            _errorProvider.SetError(Txt_Pov_High, null);
        }


        private void Rb_Zip_All_CheckedChanged(object sender, EventArgs e)
        {
            switch (Scr_Oper_Mode)
            {
                case "RNGB0004": ListZipCode.Clear(); break;
                case "RNGB0014": ListRngGroupCode.Clear(); break;
                case "RNGB0005": ListServicePlans.Clear();break;
            }
        }

        private void Rb_County_All_CheckedChanged(object sender, EventArgs e)
        {
            ListcommonEntity.Clear();
        }



        bool All_CAMS_Selected = true;
        List<MSMASTEntity> Sel_MS_List = new List<MSMASTEntity>();
        List<CAMASTEntity> Sel_CA_List = new List<CAMASTEntity>();


        private void Rb_Site_Sel_CheckedChanged(object sender, EventArgs e)
        {

        }


        List<MSMASTEntity> MS_Mast_List = new List<MSMASTEntity>();

        //DataTable dt = new DataTable();

        DataTable dtAudit = new DataTable();
        private void btnGenerateFile_Click(object sender, EventArgs e)
        {
            if (Validate_Report())
            {
                Get_Selection_Criteria();

                Search_Entity.Rep_Period_FDate = Convert.ToDateTime(((ListItem)cmbRngCode.SelectedItem).ScreenCode.ToString()).ToString("MM/dd/yyyy");
                Search_Entity.Rep_Period_TDate = Convert.ToDateTime(((ListItem)cmbRngCode.SelectedItem).ScreenType.ToString()).ToString("MM/dd/yyyy");

                DataSet ds = new DataSet();

                ds = _model.AdhocData.Get_RNGB0005_Counts(Search_Entity, Rb_Details_Yes.Checked ? "Y" : "Y", ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString(), ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).ID.ToString(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    if (Rb_Details_Yes.Checked)
                        dtAudit = ds.Tables[1];

                    if (dt.Rows.Count > 0)
                    {
                        PdfListForm pdfListForm = new PdfListForm(BaseForm, PrivilegeEntity, false, propReportPath, "xls");
                        pdfListForm.FormClosed += new FormClosedEventHandler(On_SaveExcelForm_Closed);
                        pdfListForm.StartPosition = FormStartPosition.CenterScreen;
                        pdfListForm.ShowDialog();
                    }
                    else
                        MessageBox.Show("No data for given parameters", "CAPTAIN");

                }
            }
        }

        List<CAMASTEntity> CA_Mast_List = new List<CAMASTEntity>();
        private void Fill_CAMS_Master_List()
        {
            MS_Mast_List = _model.SPAdminData.Browse_MSMAST("Code", null, null, null, null);
            CA_Mast_List = _model.SPAdminData.Browse_CAMAST("Code", null, null, null);
        }

       /** private void rdoMssiteall_Click(object sender, EventArgs e)
        {
            txt_Msselect_site.Clear();
            ListcaseMsSiteEntity.Clear();
        }*/

        private void rdoMsselectsite_CheckedChanged(object sender, EventArgs e)
        {

        }

        /**private void rdoMsselectsite_Click(object sender, EventArgs e)
        {
            if (rdoMsselectsite.Checked == true)
            {
                SelectZipSiteCountyForm siteform = new SelectZipSiteCountyForm(BaseForm, ListcaseMsSiteEntity, strAgency, strDept, strProg, string.Empty);
                siteform.FormClosed += new FormClosedEventHandler(SelectZipSiteCountyMSFormClosed);
                siteform.StartPosition = FormStartPosition.CenterScreen;
                siteform.ShowDialog();
            }
        }*/

        /**private void rdomsNosite_CheckedChanged(object sender, EventArgs e)
        {
            txt_Msselect_site.Clear();
            ListcaseMsSiteEntity.Clear();
        }*/

        /**private void Btn_MS_Selection_Click(object sender, EventArgs e)
        //{
        //    CASE0010_HSS_Form MS_Selection_Form = new CASE0010_HSS_Form(PrivilegeEntity.Program, "MS", All_CAMS_Selected, Sel_MS_List, Sel_CA_List);
        //    MS_Selection_Form.FormClosed += new FormClosedEventHandler(Get_Sel_MS_List);
        //    MS_Selection_Form.StartPosition = FormStartPosition.CenterScreen;
        //    MS_Selection_Form.ShowDialog();
        //}*/

        /**private void Rb_Fund_Sel_CheckedChanged(object sender, EventArgs e)
        {
            if (Rb_Fund_Sel.Checked == true)
            {
                SelectZipSiteCountyForm siteform = new SelectZipSiteCountyForm(BaseForm, Sel_Funding_List, "Reports", strAgency, strDept, strProg, null);
                siteform.FormClosed += new FormClosedEventHandler(SelectZipSiteCountyFormClosed);
                siteform.StartPosition = FormStartPosition.CenterScreen;
                siteform.ShowDialog();
            }
        }*/

        private void Get_Sel_MS_List(object sender, FormClosedEventArgs e)
        {
            CASE0010_HSS_Form form = sender as CASE0010_HSS_Form;
            if (form.DialogResult == DialogResult.OK)
            {
                List<MSMASTEntity> MS_List = new List<MSMASTEntity>();
                MS_List = form.Get_Sel_MS_List();
                Sel_MS_List.Clear();
                All_CAMS_Selected = true;
                if (MS_List.Count > 0)
                {
                    All_CAMS_Selected = false;
                    Sel_MS_List = MS_List;
                }
            }
        }

        private void RNGB0005Form_ToolClick(object sender, ToolClickEventArgs e)
        {
            Application.Navigate(CommonFunctions.BuildHelpURLS(PrivilegeEntity.Program, 0, BaseForm.BusinessModuleID.ToString()), target: "_blank");
        }

        /**private void rbSelProgram_Click(object sender, EventArgs e)
        {
            if (rbSelProgram.Checked == true)
            {
                HierarchieSelectionFormNew hierarchieSelectionForm = new HierarchieSelectionFormNew(BaseForm, SelectedHierarchies, "Service", "I", "A", "R", PrivilegeEntity, Current_Hierarchy.Substring(0, 2), Current_Hierarchy.Substring(2, 2));
                hierarchieSelectionForm.FormClosed += new FormClosedEventHandler(OnProgramClosed);
                hierarchieSelectionForm.StartPosition = FormStartPosition.CenterScreen;
                hierarchieSelectionForm.ShowDialog();

                //HierarchieSelection hierarchieSelectionForm = new HierarchieSelection(BaseForm, SelectedHierarchies, "Service", "I", "A", "R", PrivilegeEntity, Current_Hierarchy.Substring(0, 2), Current_Hierarchy.Substring(2, 2));
                //hierarchieSelectionForm.FormClosed += new FormClosedEventHandler(OnProgramClosed);
                //hierarchieSelectionForm.ShowDialog();
            }
        }*/

        /* private void OnProgramClosed(object sender, FormClosedEventArgs e)
         {
             // HierarchieSelectionForm form = sender as HierarchieSelectionForm;
             HierarchieSelectionFormNew form = sender as HierarchieSelectionFormNew;

             if (form.DialogResult == DialogResult.OK)
             {

                 List<HierarchyEntity> selectedHierarchies = form.SelectedHierarchies;

                 Fill_Programs_Grid(selectedHierarchies);


             }
         }*/

        /** private void Fill_Programs_Grid(List<HierarchyEntity> selectedHierarchies)
         {
             string hierarchy = string.Empty;
             int Rows_Cnt = 0;
             // HierarchyGrid.Rows.Clear();
             HierarchyGrid.Rows.Clear();
             if (selectedHierarchies.Count > 0)
             {
                 HierarchyGrid.Rows.Clear();
                 string Agy = "**", Dept = "**", Prog = "**";
                 foreach (HierarchyEntity row in selectedHierarchies)
                 {
                     Agy = Dept = Prog = "**";
                     if (!string.IsNullOrEmpty(row.Agency.Trim()))
                         Agy = row.Agency.Trim();

                     if (!string.IsNullOrEmpty(row.Prog.Trim()))
                         Prog = row.Prog.Trim();

                     if (!string.IsNullOrEmpty(row.Dept.Trim()))
                         Dept = row.Dept.Trim();

                     int rowIndex = HierarchyGrid.Rows.Add(row.Code + "  " + row.HirarchyName.ToString(), Agy + Dept + Prog);
                     HierarchyGrid.Rows[rowIndex].Tag = row;
                     Rows_Cnt++;

                     //hierarchy += row.Agency + row.Dept + row.Prog;
                     hierarchy += row.Code.Substring(0, 2) + row.Code.Substring(3, 2) + row.Code.Substring(6, 2) + ", ";
                 }

                 //if (Rows_Cnt > 0)
                 //    Txt_Program.Text = hierarchy.Substring(0, hierarchy.Length - 2);
             }
         }*/

        //Added by Sudheer on 07/22/2022
        /** private void GetSelectedHierarchies(string Sel_List)
         {
             List<HierarchyEntity> caseHierarchy = _model.lookupDataAccess.GetHierarchyByUserID(BaseForm.UserProfile.UserID, "I", "Reports");
             List<HierarchyEntity> ListofSelHiers = new List<HierarchyEntity>();
             if (caseHierarchy.Count > 0)
             {
                 //caseHierarchy= caseHierarchy.FindAll(u=>u.Agency)

                 if (!string.IsNullOrEmpty(Sel_List.Trim()))
                 {
                     Sel_List = Sel_List + ",";

                     HierarchyGrid.Rows.Clear();
                     foreach (HierarchyEntity Entity in caseHierarchy)
                     {
                         if (!Entity.Code.Contains('*'))
                         {
                             if (Sel_List.Contains(Entity.Agency + Entity.Dept + Entity.Prog.Trim()))
                             {
                                 int rowIndex = HierarchyGrid.Rows.Add(Entity.Code + "  " + Entity.HirarchyName.ToString(), Entity.Agency + Entity.Dept + Entity.Prog);
                                 HierarchyGrid.Rows[rowIndex].Tag = Entity;

                             }
                         }
                     }
                 }
             }
         }*/

        /**private void rbAllPrograms_Click(object sender, EventArgs e)
        {
            if (rbAllPrograms.Checked == true)
            {
                HierarchyGrid.Rows.Clear();
                SelectedHierarchies.Clear();
            }
        }*/

        private void Get_Sel_CA_List(object sender, FormClosedEventArgs e)
        {
            CASE0010_HSS_Form form = sender as CASE0010_HSS_Form;
            if (form.DialogResult == DialogResult.OK)
            {
                List<CAMASTEntity> CA_List = new List<CAMASTEntity>();
                CA_List = form.Get_Sel_CA_List();
                Sel_CA_List.Clear();
                All_CAMS_Selected = true;
                if (CA_List.Count > 0)
                {
                    All_CAMS_Selected = false;
                    Sel_CA_List = CA_List;
                }
            }
        }


        //private void Btn_CA_Selection_Click(object sender, EventArgs e)
        //{
        //    CASE0010_HSS_Form CA_Selection_Form = new CASE0010_HSS_Form(PrivilegeEntity.Program, "CA", All_CAMS_Selected, Sel_MS_List, Sel_CA_List);
        //    CA_Selection_Form.FormClosed += new FormClosedEventHandler(Get_Sel_CA_List);
        //    CA_Selection_Form.StartPosition = FormStartPosition.CenterScreen;
        //    CA_Selection_Form.ShowDialog();
        //}

        RCsb14GroupEntity rngcodedata;
        private void cmbRngCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRngCode.Items.Count > 0)
            {
                if (((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString() != "**")
                {

                    rngcodedata = RngCodelist.Find(u => u.GrpCode.Trim() == string.Empty && u.TblCode.Trim() == string.Empty && u.Code.Trim() == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).Value.ToString() && u.Agency == ((Captain.Common.Utilities.ListItem)cmbRngCode.SelectedItem).ID.ToString());
                    if (rngcodedata != null)
                    {
                        ListRngGroupCode.Clear();
                        /**if (rngcodedata.OFdate != string.Empty)
                        {
                            Ref_From_Date.Value = Convert.ToDateTime(rngcodedata.OFdate);
                            Ref_From_Date.Checked = true;
                        }
                        if (rngcodedata.OTdate != string.Empty)
                        {
                            Ref_To_Date.Checked = true;
                            Ref_To_Date.Value = Convert.ToDateTime(rngcodedata.OTdate);
                        }*/

                        lblAnnualFDte.Text = Convert.ToDateTime(rngcodedata.OFdate).ToString("MM/dd//yyyy");   //Added by Vikash 03/20/2023
                        lblAnnualTDte.Text = Convert.ToDateTime(rngcodedata.OTdate).ToString("MM/dd/yyyy");

                        dtpQ1S.Value = Convert.ToDateTime(rngcodedata.OFdate);
                        dtpQ1E.Value = new DateTime(Convert.ToDateTime(rngcodedata.OFdate).Year, Convert.ToDateTime(rngcodedata.OFdate).Month+2, DateTime.DaysInMonth(Convert.ToDateTime(rngcodedata.OFdate).Month+2, Convert.ToDateTime(rngcodedata.OFdate).Month+2));
                        //Convert.ToDateTime(rngcodedata.OFdate);

                        //new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Month, DateTime.Today.Month));

                        dtpQ2S.Value = new DateTime(Convert.ToDateTime(rngcodedata.OFdate).Year, Convert.ToDateTime(rngcodedata.OFdate).Month + 3, 1);//Convert.ToDateTime(rngcodedata.OFdate);
                        dtpQ2E.Value = new DateTime(Convert.ToDateTime(dtpQ2S.Value).Year, Convert.ToDateTime(dtpQ2S.Value).Month + 2, DateTime.DaysInMonth(Convert.ToDateTime(dtpQ2S.Value).Month + 2, Convert.ToDateTime(dtpQ2S.Value).Month + 2));

                        dtpQ3S.Value = new DateTime(Convert.ToDateTime(rngcodedata.OFdate).Year, Convert.ToDateTime(rngcodedata.OFdate).Month + 6, 1);
                        dtpQ3E.Value = new DateTime(Convert.ToDateTime(dtpQ3S.Value).Year, Convert.ToDateTime(dtpQ3S.Value).Month + 2, DateTime.DaysInMonth(Convert.ToDateTime(dtpQ3S.Value).Month + 2, Convert.ToDateTime(dtpQ3S.Value).Month + 2));

                        dtpQ4S.Value = new DateTime(Convert.ToDateTime(rngcodedata.OFdate).Year, Convert.ToDateTime(rngcodedata.OFdate).Month + 9, 1);
                        dtpQ4E.Value = new DateTime(Convert.ToDateTime(dtpQ4S.Value).Year, Convert.ToDateTime(dtpQ4S.Value).Month + 2, DateTime.DaysInMonth(Convert.ToDateTime(dtpQ4S.Value).Month + 2, Convert.ToDateTime(dtpQ4S.Value).Month + 2));
                    }
                    lblAnnualFDte.Visible = lblTo.Visible = lblAnnualTDte.Visible = true;
                }
                else
                {
                    lblAnnualFDte.Visible = lblTo.Visible = lblAnnualTDte.Visible = false;

                    dtpQ1S.Value = Convert.ToDateTime(DateTime.Now);
                    dtpQ1E.Value = Convert.ToDateTime(DateTime.Now);
                    //Convert.ToDateTime(rngcodedata.OFdate);

                    //new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Month, DateTime.Today.Month));

                    dtpQ2S.Value = Convert.ToDateTime(DateTime.Now); //Convert.ToDateTime(rngcodedata.OFdate);
                    dtpQ2E.Value = Convert.ToDateTime(DateTime.Now);

                    dtpQ3S.Value = Convert.ToDateTime(DateTime.Now);
                    dtpQ3E.Value = Convert.ToDateTime(DateTime.Now);

                    dtpQ4S.Value = Convert.ToDateTime(DateTime.Now);
                    dtpQ4E.Value = Convert.ToDateTime(DateTime.Now);
                }
            }
        }


        public string HeadersValue(int intcolumnscount)
        {
            string strValue = string.Empty;
            switch (intcolumnscount)
            {
                case 4:
                    strValue = "IV.) ";
                    break;
                case 5:
                    strValue = "V.) ";
                    break;
                case 6:
                    strValue = "VI.) ";
                    break;
                case 7:
                    strValue = "VII.) ";
                    break;
                case 8:
                    strValue = "VIII.) ";
                    break;
                case 9:
                    strValue = "IX.) ";
                    break;
                case 10:
                    strValue = "X.) ";
                    break;
                default:
                    break;
            }
            return strValue;
        }
        public string HeadersValue1(int intcolumnscount)
        {
            string strValue = string.Empty;
            switch (intcolumnscount)
            {
                case 4:
                    strValue = "IV";
                    break;
                case 5:
                    strValue = "V";
                    break;
                case 6:
                    strValue = "VI";
                    break;
                case 7:
                    strValue = "VII";
                    break;
                case 8:
                    strValue = "VIII";
                    break;
                case 9:
                    strValue = "IX";
                    break;
                case 10:
                    strValue = "X";
                    break;
                default:
                    break;
            }
            return strValue;
        }

      /**  private void rdoperiodBoth_Click(object sender, EventArgs e)
        {
            reportforselection();

            //if (rdoperiod.Checked)
            //{
            //    Rb_SNP_Mem.Enabled = true;
            //}
            //else
            //{
            //    Rb_SNP_Mem.Enabled = false;
            //    Rb_SNP_Mem.Checked = false;
            //    Rb_OBO_Mem.Checked = true;
            //}

        }*/

       /** private void reportforselection()
        {
            /**if (rdoperiodBoth.Checked)
            {
                // Rb_SNP_Mem.Enabled = false;
                Rb_SNP_Mem.Checked = false;
                Rb_OBO_Mem.Checked = true;
            }
            else
            {
                Rb_SNP_Mem.Enabled = true;
            }

            if (rdoperiod.Checked == true)
            {
               /** Ref_From_Date.Enabled = false;
                Ref_To_Date.Enabled = false;
                Rep_From_Date.Enabled = true;
                Rep_To_Date.Enabled = true;

                chkbUndupTable.Enabled = true;
            }
            else if (rdoperiodCumulative.Checked == true)
            {
               /* Ref_From_Date.Enabled = true;
                Ref_To_Date.Enabled = true;
                Rep_From_Date.Enabled = false;
                Rep_To_Date.Enabled = false;

                chkbUndupTable.Enabled = true;
            }
            else if (rdoperiodBoth.Checked == true)
            {
                /**Ref_From_Date.Enabled = true;
                Ref_To_Date.Enabled = true;
                Rep_From_Date.Enabled = true;
                Rep_To_Date.Enabled = true;

                chkbUndupTable.Enabled = false;
                chkbUndupTable.Checked = false;
            }

        }*/

        private void btnMergeExcelView_Click(object sender, EventArgs e)
        {
            PdfListForm pdfMergeListForm = new PdfListForm(BaseForm);
            pdfMergeListForm.StartPosition = FormStartPosition.CenterScreen;
            pdfMergeListForm.ShowDialog();
        }

        /**private string Get_Sel_CA_Fund_List_To_Filter()
        {
            string Sel_Codes = null;

            if (Rb_Fund_Sel.Checked)
            {
                foreach (SPCommonEntity Entity in Sel_Funding_List)
                {
                    Sel_Codes += "'" + Entity.Code + "' ,";
                }

                if (Sel_Codes.Length > 0)
                    Sel_Codes = Sel_Codes.Substring(0, (Sel_Codes.Length - 1));
            }

            return Sel_Codes;
        }*/

        private void On_SaveExcelForm_Closed(object sender, FormClosedEventArgs e)
        {
            Random_Filename = null;
            PdfListForm form = sender as PdfListForm;
            if (form.DialogResult == DialogResult.OK)
            {
                Random_Filename = null;
                string PdfName = "Pdf File";
                PdfName = form.GetFileName();
                string PdfFileName = PdfName;
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
                    string Tmpstr = PdfName + ".xls";
                    if (File.Exists(Tmpstr))
                        File.Delete(Tmpstr);
                }
                catch (Exception ex)
                {
                    int length = 8;
                    string newFileName = System.Guid.NewGuid().ToString();
                    newFileName = newFileName.Replace("-", string.Empty);

                    Random_Filename = PdfName + newFileName.Substring(0, length) + ".xls";
                }

                if (!string.IsNullOrEmpty(Random_Filename))
                    PdfName = Random_Filename;
                else
                    PdfName += ".xls";

                Workbook book = new Workbook();

                this.GenerateStyles(book.Styles);



                bool First = true; string MS_Type = string.Empty;
                string CAMSDesc = null; string Branch = null, Priv_Branch = null, SP_Plan_desc = null;
                bool SPNum = true; Worksheet sheet; WorksheetCell cell; WorksheetRow Row0; int Count = 1;


                if(dt.Rows.Count>0)
                {
                    this.GenerateWorksheetParameters(book.Worksheets);

                    string PrivSP = string.Empty;

                    var distinctRows = (from DataRow dRow in dt.Rows
                                        select dRow["SP0_SERVICECODE"]).Distinct();

                    foreach(var dRow in distinctRows)
                    {
                        DataTable dataTable = new DataTable();
                        DataView dv = new DataView(dt);
                        dv.RowFilter = "SP0_SERVICECODE='" + dRow + "'";
                        dataTable = dv.ToTable();

                        if(dataTable.Rows.Count>0)
                        {
                            string ReportName = dataTable.Rows[0]["SP0_DESCRIPTION"].ToString();
                            ReportName = ReportName.Replace("/", "");

                            if (ReportName.Length >= 31)
                            {
                                ReportName = ReportName.Substring(0, 31);
                            }
                            sheet = book.Worksheets.Add(ReportName);
                            sheet.Table.DefaultRowHeight = 14.25F;


                            sheet.Table.Columns.Add(200);
                            sheet.Table.Columns.Add(150);
                            sheet.Table.Columns.Add(75);
                            sheet.Table.Columns.Add(75);
                            sheet.Table.Columns.Add(75);
                            sheet.Table.Columns.Add(75);
                            sheet.Table.Columns.Add(75);
                            sheet.Table.Columns.Add(80);
                            sheet.Table.Columns.Add(80);

                            Row0 = sheet.Table.Rows.Add();
                            Row0.AutoFitHeight = false;
                            //WorksheetCell cell;
                            cell = Row0.Cells.Add();
                            cell.StyleID = "s83";
                            cell.Data.Type = DataType.String;
                            cell.Data.Text = dataTable.Rows[0]["SP0_DESCRIPTION"].ToString();
                            cell.MergeAcross = 8;


                            Row0 = sheet.Table.Rows.Add();

                            cell = Row0.Cells.Add("Description", DataType.String, "s94");
                            cell = Row0.Cells.Add("Type", DataType.String, "s94");
                            cell = Row0.Cells.Add("Target", DataType.String, "s94");
                            cell = Row0.Cells.Add("Q1", DataType.String, "s94");
                            cell = Row0.Cells.Add("Q2", DataType.String, "s94");
                            cell = Row0.Cells.Add("Q3", DataType.String, "s94");
                            cell = Row0.Cells.Add("Q4", DataType.String, "s94");
                            cell = Row0.Cells.Add("YTD", DataType.String, "s94");
                            cell = Row0.Cells.Add("YTD %", DataType.String, "s94");


                            int i = 0;
                            foreach (DataRow dr in dataTable.Rows)
                            {
                                Row0 = sheet.Table.Rows.Add();

                                
                                string CAMSType = string.Empty;
                                if (dr["SP2_TYPE"].ToString().Trim() == "CA") CAMSType = "Service"; else CAMSType = "Outcome";

                                string Ytd = string.Empty;
                                if(dr["TARGET_VAL"].ToString()!="0")
                                {
                                    decimal percent= (decimal.Parse(dr["YTD"].ToString().Trim()==string.Empty?"0": dr["YTD"].ToString().Trim()) / decimal.Parse(dr["TARGET_VAL"].ToString().Trim()==string.Empty?"0":dr["TARGET_VAL"].ToString().Trim())) *100;

                                    Ytd = percent.ToString("0.00");
                                }

                                if(i%2==0)
                                {
                                    if (dr["SP0_ACTIVE"].ToString() == "Y")
                                    {
                                        cell = Row0.Cells.Add(dr["CAMS_DESC"].ToString(), DataType.String, "s105");
                                        cell = Row0.Cells.Add(CAMSType, DataType.String, "s105");
                                        cell = Row0.Cells.Add(dr["TARGET_VAL"].ToString(), DataType.String, "s106");
                                        //if (dr["SP2_COUNT_TYPE"].ToString() == "A")
                                        //{
                                            cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A"?dr["Q1"].ToString()+"%": dr["Q1"].ToString(), DataType.String, "s106");
                                            cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q2"].ToString() + "%": dr["Q2"].ToString(), DataType.String, "s106");
                                            cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q3"].ToString() + "%": dr["Q3"].ToString(), DataType.String, "s106");
                                            cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q4"].ToString() + "%":dr["Q4"].ToString(), DataType.String, "s106");
                                            cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["YTD"].ToString() + "%":dr["YTD"].ToString(), DataType.String, "s106");
                                            cell = Row0.Cells.Add(Ytd==string.Empty?"": Ytd + "%", DataType.String, "s106");
                                        //}
                                        //else
                                        //{
                                        //    cell = Row0.Cells.Add(dr["Q1"].ToString(), DataType.String, "s106");
                                        //    cell = Row0.Cells.Add(dr["Q2"].ToString(), DataType.String, "s106");
                                        //    cell = Row0.Cells.Add(dr["Q3"].ToString(), DataType.String, "s106");
                                        //    cell = Row0.Cells.Add(dr["Q4"].ToString(), DataType.String, "s106");
                                        //    cell = Row0.Cells.Add(dr["YTD"].ToString(), DataType.String, "s106");
                                        //    cell = Row0.Cells.Add(Ytd, DataType.String, "s106");
                                        //}
                                    }
                                    else
                                    {
                                        cell = Row0.Cells.Add(dr["CAMS_DESC"].ToString(), DataType.String, "s105R");
                                        cell = Row0.Cells.Add(CAMSType, DataType.String, "s105R");
                                        cell = Row0.Cells.Add(dr["TARGET_VAL"].ToString(), DataType.String, "s106R");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q1"].ToString()+"%": dr["Q1"].ToString(), DataType.String, "s106R");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q2"].ToString()+"%": dr["Q2"].ToString(), DataType.String, "s106R");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q3"].ToString()+"%": dr["Q3"].ToString(), DataType.String, "s106R");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q4"].ToString()+"%": dr["Q4"].ToString(), DataType.String, "s106R");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["YTD"].ToString()+"%": dr["YTD"].ToString(), DataType.String, "s106R");
                                        cell = Row0.Cells.Add(Ytd == string.Empty ? "" : Ytd + "%", DataType.String, "s106R");
                                    }
                                    
                                }
                                else
                                {
                                    if (dr["SP0_ACTIVE"].ToString() == "Y")
                                    {
                                        cell = Row0.Cells.Add(dr["CAMS_DESC"].ToString(), DataType.String, "s95");
                                        cell = Row0.Cells.Add(CAMSType, DataType.String, "s95");
                                        cell = Row0.Cells.Add(dr["TARGET_VAL"].ToString(), DataType.String, "s95RC");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q1"].ToString() + "%" : dr["Q1"].ToString(), DataType.String, "s95RC");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q2"].ToString() + "%" : dr["Q2"].ToString(), DataType.String, "s95RC");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q3"].ToString() + "%" : dr["Q3"].ToString(), DataType.String, "s95RC");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q4"].ToString() + "%" : dr["Q4"].ToString(), DataType.String, "s95RC");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["YTD"].ToString() + "%" : dr["YTD"].ToString(), DataType.String, "s95RC");
                                        cell = Row0.Cells.Add(Ytd == string.Empty ? "" : Ytd + "%", DataType.String, "s95RC");

                                    }
                                    else
                                    {
                                        cell = Row0.Cells.Add(dr["CAMS_DESC"].ToString(), DataType.String, "s95R");
                                        cell = Row0.Cells.Add(CAMSType, DataType.String, "s95R");
                                        cell = Row0.Cells.Add(dr["TARGET_VAL"].ToString(), DataType.String, "s95RCR");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q1"].ToString() + "%" : dr["Q1"].ToString(), DataType.String, "s95RCR");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q2"].ToString() + "%" : dr["Q2"].ToString(), DataType.String, "s95RCR");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q3"].ToString() + "%" : dr["Q3"].ToString(), DataType.String, "s95RCR");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["Q4"].ToString() + "%" : dr["Q4"].ToString(), DataType.String, "s95RCR");
                                        cell = Row0.Cells.Add(dr["SP2_COUNT_TYPE"].ToString() == "A" ? dr["YTD"].ToString()+"%": dr["YTD"].ToString(), DataType.String, "s95RCR");
                                        cell = Row0.Cells.Add(Ytd == string.Empty ? "" : Ytd + "%", DataType.String, "s95RCR");
                                    }

                                }

                                


                                //if (dr["SP0_SERVICECODE"].ToString() != PrivSP)
                                //{



                                //    PrivSP = dr["SP0_SERVICECODE"].ToString();
                                //}


                                i++;
                            }
                        }
                        
                        if(Rb_SP_Sel.Checked)
                        {
                            if(dtAudit.Rows.Count>0)
                            {
                                DataTable dataAudit = new DataTable();
                                DataView dvAudit = new DataView(dtAudit);
                                dvAudit.RowFilter = "SP='" + dRow + "'";
                                dataAudit = dvAudit.ToTable();

                                if (dataAudit.Rows.Count > 0)
                                {
                                    string ReportName = dRow + "_Audit";//dataTable.Rows[0]["SP_Desc"].ToString();
                                    ReportName = ReportName.Replace("/", "");

                                    if (ReportName.Length >= 31)
                                    {
                                        ReportName = ReportName.Substring(0, 31);
                                    }
                                    sheet = book.Worksheets.Add(ReportName);
                                    sheet.Table.DefaultRowHeight = 14.25F;


                                    sheet.Table.Columns.Add(75);
                                    sheet.Table.Columns.Add(75);
                                    sheet.Table.Columns.Add(75);
                                    sheet.Table.Columns.Add(75);
                                    sheet.Table.Columns.Add(80);
                                    sheet.Table.Columns.Add(145);
                                    sheet.Table.Columns.Add(75);
                                    sheet.Table.Columns.Add(80);
                                    sheet.Table.Columns.Add(180);
                                    sheet.Table.Columns.Add(80);
                                    sheet.Table.Columns.Add(80);

                                    Row0 = sheet.Table.Rows.Add();
                                    Row0.AutoFitHeight = false;
                                    //WorksheetCell cell;
                                    cell = Row0.Cells.Add();
                                    cell.StyleID = "s83";
                                    cell.Data.Type = DataType.String;
                                    cell.Data.Text = dataAudit.Rows[0]["SP_Desc"].ToString();
                                    cell.MergeAcross = 9;


                                    Row0 = sheet.Table.Rows.Add();

                                    cell = Row0.Cells.Add("Agency", DataType.String, "s94");
                                    cell = Row0.Cells.Add("Dept", DataType.String, "s94");
                                    cell = Row0.Cells.Add("Program", DataType.String, "s94");
                                    cell = Row0.Cells.Add("Year", DataType.String, "s94");
                                    cell = Row0.Cells.Add("App", DataType.String, "s94");
                                    cell = Row0.Cells.Add("Name", DataType.String, "s94");
                                    cell = Row0.Cells.Add("Type", DataType.String, "s94");
                                    cell = Row0.Cells.Add("Code", DataType.String, "s94");
                                    cell = Row0.Cells.Add("Description", DataType.String, "s94");
                                    cell = Row0.Cells.Add("Date", DataType.String, "s94");
                                    cell = Row0.Cells.Add("Amount", DataType.String, "s94");


                                    int i = 0;
                                    foreach (DataRow dr in dataAudit.Rows)
                                    {
                                        Row0 = sheet.Table.Rows.Add();


                                        string CAMSType = string.Empty;
                                        if (dr["SP2_TYPE"].ToString().Trim() == "CA") CAMSType = "Service"; else CAMSType = "Outcome";



                                        if (i % 2 == 0)
                                        {
                                            cell = Row0.Cells.Add(dr["Agency"].ToString(), DataType.String, "s105");
                                            cell = Row0.Cells.Add(dr["Dept"].ToString(), DataType.String, "s105");
                                            cell = Row0.Cells.Add(dr["Program"].ToString(), DataType.String, "s105");
                                            cell = Row0.Cells.Add(dr["Prog_Year"].ToString(), DataType.String, "s105");
                                            cell = Row0.Cells.Add(dr["AppNo"].ToString(), DataType.String, "s105");
                                            cell = Row0.Cells.Add(dr["Name"].ToString(), DataType.String, "s105");
                                            cell = Row0.Cells.Add(CAMSType, DataType.String, "s105");
                                            cell = Row0.Cells.Add(dr["CAMS_CODE"].ToString(), DataType.String, "s105");
                                            cell = Row0.Cells.Add(dr["CAMS_DESC"].ToString(), DataType.String, "s105");
                                            cell = Row0.Cells.Add(LookupDataAccess.Getdate(dr["CAMS_DATE"].ToString()), DataType.String, "s105");
                                            cell = Row0.Cells.Add(dr["CASEMS_COST"].ToString(), DataType.String, "s106");



                                        }
                                        else
                                        {

                                            cell = Row0.Cells.Add(dr["Agency"].ToString(), DataType.String, "s95");
                                            cell = Row0.Cells.Add(dr["Dept"].ToString(), DataType.String, "s95");
                                            cell = Row0.Cells.Add(dr["Program"].ToString(), DataType.String, "s95");
                                            cell = Row0.Cells.Add(dr["Prog_Year"].ToString(), DataType.String, "s95");
                                            cell = Row0.Cells.Add(dr["AppNo"].ToString(), DataType.String, "s95");
                                            cell = Row0.Cells.Add(dr["Name"].ToString(), DataType.String, "s95");
                                            cell = Row0.Cells.Add(CAMSType, DataType.String, "s95");
                                            cell = Row0.Cells.Add(dr["CAMS_CODE"].ToString(), DataType.String, "s95");
                                            cell = Row0.Cells.Add(dr["CAMS_DESC"].ToString(), DataType.String, "s95");
                                            cell = Row0.Cells.Add(LookupDataAccess.Getdate(dr["CAMS_DATE"].ToString()), DataType.String, "s95");
                                            cell = Row0.Cells.Add(dr["CASEMS_COST"].ToString(), DataType.String, "s95RC");


                                        }
                                        i++;
                                    }
                                }


                            }
                        }

                    }

                    
                    
                }


                FileStream stream = new FileStream(PdfName, FileMode.Create);

                book.Save(stream);
                stream.Close();

                //FileDownloadGateway downloadGateway = new FileDownloadGateway();
                //downloadGateway.Filename = "SPREPAPP_Report.xls";

                //// downloadGateway.Version = file.Version;

                //downloadGateway.SetContentType(DownloadContentType.OctetStream);

                //downloadGateway.StartFileDownload(new ContainerControl(), PdfName);

                FileInfo fiDownload = new FileInfo(PdfName);
                /// Need to check for file exists, is local file, is allow to read, etc...
                string name = fiDownload.Name;
                using (FileStream fileStream = fiDownload.OpenRead())
                {
                    Application.Download(fileStream, name);
                }

                if(Rb_Details_Yes.Checked && Rb_SP_All.Checked)
                {
                    On_SaveExcelAudit_Closed(PdfFileName);
                }

            }

        }

        private void GenerateStyles(WorksheetStyleCollection styles)
        {
            // -----------------------------------------------
            //  Default
            // -----------------------------------------------
            WorksheetStyle Default = styles.Add("Default");
            Default.Name = "Normal";
            Default.Font.FontName = "Arial";
            Default.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            // -----------------------------------------------
            //  s16
            // -----------------------------------------------
            WorksheetStyle s16 = styles.Add("s16");
            // -----------------------------------------------
            //  s17
            // -----------------------------------------------
            WorksheetStyle s17 = styles.Add("s17");
            s17.NumberFormat = "0%";
            // -----------------------------------------------
            //  s18
            // -----------------------------------------------
            WorksheetStyle s18 = styles.Add("s18");
            // -----------------------------------------------
            //  s19
            // -----------------------------------------------
            WorksheetStyle s19 = styles.Add("s19");
            s19.Font.FontName = "Arial";
            // -----------------------------------------------
            //  s20
            // -----------------------------------------------
            WorksheetStyle s20 = styles.Add("s20");
            s20.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s20.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            // -----------------------------------------------
            //  s21
            // -----------------------------------------------
            WorksheetStyle s21 = styles.Add("s21");
            s21.Font.Bold = true;
            s21.Font.FontName = "Arial";
            s21.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s21.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s21.NumberFormat = "0%";
            // -----------------------------------------------
            //  s23
            // -----------------------------------------------
            WorksheetStyle s23 = styles.Add("s23");
            s23.Font.Bold = true;
            s23.Font.FontName = "Calibri";
            s23.Font.Size = 11;
            s23.Font.Color = "#000000";
            // -----------------------------------------------
            //  s24
            // -----------------------------------------------
            WorksheetStyle s24 = styles.Add("s24");
            s24.Interior.Color = "#D8D8D8";
            s24.Interior.Pattern = StyleInteriorPattern.Solid;
            // -----------------------------------------------
            //  s25
            // -----------------------------------------------
            WorksheetStyle s25 = styles.Add("s25");
            s25.Font.FontName = "Arial";
            s25.Interior.Color = "#D8D8D8";
            s25.Interior.Pattern = StyleInteriorPattern.Solid;
            // -----------------------------------------------
            //  s26
            // -----------------------------------------------
            WorksheetStyle s26 = styles.Add("s26");
            s26.Interior.Color = "#D8D8D8";
            s26.Interior.Pattern = StyleInteriorPattern.Solid;
            s26.NumberFormat = "0%";
            // -----------------------------------------------
            //  s27
            // -----------------------------------------------
            WorksheetStyle s27 = styles.Add("s27");
            s27.Interior.Color = "#D8D8D8";
            s27.Interior.Pattern = StyleInteriorPattern.Solid;
            s27.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s27.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            // -----------------------------------------------
            //  s28
            // -----------------------------------------------
            WorksheetStyle s28 = styles.Add("s28");
            s28.Font.Bold = true;
            s28.Font.FontName = "Arial";
            s28.Interior.Color = "#D8D8D8";
            s28.Interior.Pattern = StyleInteriorPattern.Solid;
            s28.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s28.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s28.NumberFormat = "0%";
           
            // -----------------------------------------------
            //  s71
            // -----------------------------------------------
            WorksheetStyle s71 = styles.Add("s71");
            s71.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#ABABAB");
           
            // -----------------------------------------------
            //  s73
            // -----------------------------------------------
            WorksheetStyle s73 = styles.Add("s73");
            s73.NumberFormat = "0%";
            // -----------------------------------------------
            //  s74
            // -----------------------------------------------
            WorksheetStyle s74 = styles.Add("s74");
            s74.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#ABABAB");
            s74.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#ABABAB");
            s74.NumberFormat = "0%";
            // -----------------------------------------------
            //  s75
            // -----------------------------------------------
            WorksheetStyle s75 = styles.Add("s75");
            s75.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#ABABAB");
            s75.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#ABABAB");
            s75.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#ABABAB");
            // -----------------------------------------------
            //  s76
            // -----------------------------------------------
            WorksheetStyle s76 = styles.Add("s76");
            s76.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#ABABAB");
            s76.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#ABABAB");
            s76.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#ABABAB");
            s76.NumberFormat = "0%";
            // -----------------------------------------------
            //  s77
            // -----------------------------------------------
            WorksheetStyle s77 = styles.Add("s77");
            s77.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#ABABAB");
            s77.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#ABABAB");
            s77.NumberFormat = "0%";
            // -----------------------------------------------
            //  s78
            // -----------------------------------------------
            WorksheetStyle s78 = styles.Add("s78");
            s78.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#ABABAB");
            s78.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#ABABAB");
            s78.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#ABABAB");
            s78.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#ABABAB");
            s78.NumberFormat = "0%";
            // -----------------------------------------------
            //  s79
            // -----------------------------------------------
            WorksheetStyle s79 = styles.Add("s79");
            s79.Font.Bold = true;
            s79.Font.FontName = "Arial";
            s79.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s79.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            // -----------------------------------------------
            //  s81
            // -----------------------------------------------
            WorksheetStyle s81 = styles.Add("s81");
            s81.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            // -----------------------------------------------
            //  s82
            // -----------------------------------------------
            WorksheetStyle s82 = styles.Add("s82");
            s82.Font.Bold = true;
            s82.Font.FontName = "Arial";
            s82.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s82.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s82.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#ABABAB");
            s82.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#ABABAB");
            s82.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#ABABAB");
            s82.NumberFormat = "0%";
            // -----------------------------------------------
            //  s83
            // -----------------------------------------------
            WorksheetStyle s83 = styles.Add("s83");
            s83.Font.Bold = true;
            s83.Font.FontName = "Arial";
            s83.Font.Size = 11;
            s83.Font.Color = "#666699";
            s83.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s83.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s83.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s84
            // -----------------------------------------------
            WorksheetStyle s84 = styles.Add("s84");
            s84.Font.Bold = true;
            s84.Font.FontName = "Arial";
            s84.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s84.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s84.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#ABABAB");
            s84.NumberFormat = "0%";
            // -----------------------------------------------
            //  s86
            // -----------------------------------------------
            WorksheetStyle s86 = styles.Add("s86");
            s86.Font.Bold = true;
            s86.Font.FontName = "Arial";
            s86.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s86.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s86.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#ABABAB");
            s86.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#ABABAB");
            s86.NumberFormat = "0%";
            // -----------------------------------------------
            //  s87
            // -----------------------------------------------
            WorksheetStyle s87 = styles.Add("s87");
            s87.Font.Bold = true;
            s87.Font.FontName = "Arial";
            s87.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s87.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s87.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#ABABAB");
            s87.NumberFormat = "0%";
            // -----------------------------------------------
            //  s90
            // -----------------------------------------------
            WorksheetStyle s90 = styles.Add("s90");
            s90.Font.Bold = true;
            s90.Font.FontName = "Arial";
            s90.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s90.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s90.NumberFormat = "0%";
            // -----------------------------------------------
            //  s92
            // -----------------------------------------------
            WorksheetStyle s92 = styles.Add("s92");
            s92.Font.Bold = true;
            s92.Font.Italic = true;
            s92.Font.FontName = "Arial";
            s92.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s92.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s92.NumberFormat = "0%";
            // -----------------------------------------------
            //  s93
            // -----------------------------------------------
            WorksheetStyle s93 = styles.Add("s93");
            s93.Font.Bold = true;
            s93.Font.Italic = true;
            s93.Font.FontName = "Arial";
            s93.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s93.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            // -----------------------------------------------
            //  s94
            // -----------------------------------------------
            WorksheetStyle s94 = styles.Add("s94");
            s94.Font.Bold = true;
            s94.Font.FontName = "Arial";
            s94.Font.Color = "#000000";
            s94.Interior.Color = "#B0C4DE";
            s94.Interior.Pattern = StyleInteriorPattern.Solid;
            s94.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s94.Alignment.Vertical = StyleVerticalAlignment.Top;
            s94.Alignment.WrapText = true;
            s94.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            s94.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            s94.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            s94.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            s94.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s95
            // -----------------------------------------------
            WorksheetStyle s95 = styles.Add("s95");
            s95.Font.FontName = "Arial";
            s95.Font.Color = "#000000";
            s95.Interior.Color = "#FFFFFF";
            s95.Interior.Pattern = StyleInteriorPattern.Solid;
            s95.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s95.Alignment.Vertical = StyleVerticalAlignment.Top;
            s95.Alignment.WrapText = true;
            s95.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            // -----------------------------------------------
            //  s95B
            // -----------------------------------------------
            WorksheetStyle s95B = styles.Add("s95B");
            s95B.Font.FontName = "Arial";
            s95B.Font.Bold = true;
            s95B.Font.Color = "#0000FF";
            s95B.Interior.Color = "#FFFFFF";
            s95B.Interior.Pattern = StyleInteriorPattern.Solid;
            s95B.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s95B.Alignment.Vertical = StyleVerticalAlignment.Top;
            s95B.Alignment.WrapText = true;
            s95B.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            //  s95R
            // -----------------------------------------------
            WorksheetStyle s95R = styles.Add("s95R");
            s95R.Font.FontName = "Arial";
            //s95R.Font.Bold = true;
            s95R.Font.Color = "#FF0000";
            s95R.Interior.Color = "#FFFFFF";
            s95R.Interior.Pattern = StyleInteriorPattern.Solid;
            s95R.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s95R.Alignment.Vertical = StyleVerticalAlignment.Top;
            s95R.Alignment.WrapText = true;
            s95R.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            // -----------------------------------------------
            //  s95RC
            // -----------------------------------------------
            WorksheetStyle s95RC = styles.Add("s95RC");
            s95RC.Font.FontName = "Arial";
            s95RC.Font.Color = "#000000";
            s95RC.Interior.Color = "#FFFFFF";
            s95RC.Interior.Pattern = StyleInteriorPattern.Solid;
            s95RC.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            s95RC.Alignment.Vertical = StyleVerticalAlignment.Top;
            s95RC.Alignment.WrapText = true;
            s95RC.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            // -----------------------------------------------
            //  s95RCR
            // -----------------------------------------------
            WorksheetStyle s95RCR = styles.Add("s95RCR");
            s95RCR.Font.FontName = "Arial";
            s95RCR.Font.Color = "#FF0000";
            s95RCR.Interior.Color = "#FFFFFF";
            s95RCR.Interior.Pattern = StyleInteriorPattern.Solid;
            s95RCR.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            s95RCR.Alignment.Vertical = StyleVerticalAlignment.Top;
            s95RCR.Alignment.WrapText = true;
            s95RCR.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;

            // -----------------------------------------------
            //  s96
            // -----------------------------------------------
            WorksheetStyle s96 = styles.Add("s96");
            s96.Font.FontName = "Arial";
            s96.Font.Color = "#000000";
            s96.Interior.Color = "#FFFFFF";
            s96.Font.Bold = true;
            s96.Interior.Pattern = StyleInteriorPattern.Solid;
            s96.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s96.Alignment.Vertical = StyleVerticalAlignment.Top;
            s96.Alignment.WrapText = true;
            s96.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;

            // -----------------------------------------------
            //  s97
            // -----------------------------------------------
            WorksheetStyle s97 = styles.Add("s97");
            s97.Font.Bold = true;
            s97.Font.FontName = "Arial";
            s97.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s97.Alignment.Vertical = StyleVerticalAlignment.Center;
            s97.NumberFormat = "0%";
            // -----------------------------------------------
            //  s98
            // -----------------------------------------------
            WorksheetStyle s98 = styles.Add("s98");
            s98.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#A5A5A5");
            s98.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#A5A5A5");
            s98.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#A5A5A5");
            s98.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#A5A5A5");
            // -----------------------------------------------
            //  s99
            // -----------------------------------------------
            WorksheetStyle s99 = styles.Add("s99");
            s99.Font.Bold = true;
            s99.Font.FontName = "Arial";
            s99.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#A5A5A5");
            s99.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#A5A5A5");
            s99.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#A5A5A5");
            s99.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#A5A5A5");
            // -----------------------------------------------
            //  s100
            // -----------------------------------------------
            WorksheetStyle s100 = styles.Add("s100");
            s100.Font.Bold = true;
            s100.Font.FontName = "Arial";
            s100.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s100.Alignment.Vertical = StyleVerticalAlignment.Center;
            s100.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#A5A5A5");
            s100.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#A5A5A5");
            s100.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#A5A5A5");
            s100.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#A5A5A5");
            s100.NumberFormat = "0%";
            // -----------------------------------------------
            //  s101
            // -----------------------------------------------
            WorksheetStyle s101 = styles.Add("s101");
            s101.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#A5A5A5");
            s101.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#A5A5A5");
            s101.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#A5A5A5");
            s101.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#A5A5A5");
            s101.NumberFormat = "0%";
            // -----------------------------------------------
            //  s102
            // -----------------------------------------------
            WorksheetStyle s102 = styles.Add("s102");
            s102.Font.Bold = true;
            s102.Font.FontName = "Arial";
            s102.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s102.Alignment.Vertical = StyleVerticalAlignment.Center;
            s102.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#A5A5A5");
            s102.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#A5A5A5");
            s102.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#A5A5A5");
            s102.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#A5A5A5");
            s102.NumberFormat = "0%";
            // -----------------------------------------------
            //  s103
            // -----------------------------------------------
            WorksheetStyle s103 = styles.Add("s103");
            s103.Font.Bold = true;
            s103.Font.FontName = "Arial";
            s103.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s103.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s103.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#A5A5A5");
            s103.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#A5A5A5");
            s103.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#A5A5A5");
            s103.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#A5A5A5");
            s103.NumberFormat = "0%";
            // -----------------------------------------------
            //  s104
            // -----------------------------------------------
            WorksheetStyle s104 = styles.Add("s104");
            s104.Font.FontName = "Arial";
            // -----------------------------------------------
            //  s105
            // -----------------------------------------------
            WorksheetStyle s105 = styles.Add("s105");
            s105.Font.FontName = "Arial";
            s105.Font.Color = "#000000";
            s105.Interior.Color = "#DCE6F1";
            s105.Interior.Pattern = StyleInteriorPattern.Solid;
            s105.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s105.Alignment.Vertical = StyleVerticalAlignment.Top;
            s105.Alignment.WrapText = true;
            s105.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            // -----------------------------------------------
            //  s106
            // -----------------------------------------------
            WorksheetStyle s106 = styles.Add("s106");
            s106.Font.FontName = "Arial";
            s106.Font.Color = "#000000";
            s106.Interior.Color = "#DCE6F1";
            s106.Interior.Pattern = StyleInteriorPattern.Solid;
            s106.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            s106.Alignment.Vertical = StyleVerticalAlignment.Top;
            s106.Alignment.WrapText = true;
            s106.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            // -----------------------------------------------
            //  s105R
            // -----------------------------------------------
            WorksheetStyle s105R = styles.Add("s105R");
            s105R.Font.FontName = "Arial";
            s105R.Font.Color = "#FF0000";
            s105R.Interior.Color = "#DCE6F1";
            s105R.Interior.Pattern = StyleInteriorPattern.Solid;
            s105R.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s105R.Alignment.Vertical = StyleVerticalAlignment.Top;
            s105R.Alignment.WrapText = true;
            s105R.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            // -----------------------------------------------
            //  s106R
            // -----------------------------------------------
            WorksheetStyle s106R = styles.Add("s106R");
            s106R.Font.FontName = "Arial";
            s106R.Font.Color = "#FF0000";
            s106R.Interior.Color = "#DCE6F1";
            s106R.Interior.Pattern = StyleInteriorPattern.Solid;
            s106R.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            s106R.Alignment.Vertical = StyleVerticalAlignment.Top;
            s106R.Alignment.WrapText = true;
            s106R.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;

            // -----------------------------------------------
            //  s107
            // -----------------------------------------------
            WorksheetStyle s107 = styles.Add("s107");
            s107.Font.FontName = "Arial";
            // -----------------------------------------------
            //  s108
            // -----------------------------------------------
            WorksheetStyle s108 = styles.Add("s108");
            s108.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#A5A5A5");
            s108.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#A5A5A5");
            s108.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#A5A5A5");
            s108.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#A5A5A5");
            s108.NumberFormat = "0%";

            // -----------------------------------------------
            //  s31
            // -----------------------------------------------
            WorksheetStyle s31 = styles.Add("s31");
            s31.Font.Bold = true;
            s31.Font.Underline = UnderlineStyle.Single;
            s31.Font.FontName = "Times New Roman";
            s31.Font.Size = 12;
            s31.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s31.Alignment.Vertical = StyleVerticalAlignment.Center;
            // -----------------------------------------------
            //  s32
            // -----------------------------------------------
            WorksheetStyle s32 = styles.Add("s32");
            s32.Font.Bold = true;
            s32.Font.Underline = UnderlineStyle.Single;
            s32.Font.FontName = "Times New Roman";
            s32.Font.Size = 12;
            s32.Font.Color = "#000000";
            s32.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s32.Alignment.Vertical = StyleVerticalAlignment.Center;
            //s32.NumberFormat = "_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)";
            // -----------------------------------------------
            //  s33
            // -----------------------------------------------
            WorksheetStyle s33 = styles.Add("s33");
            s33.Font.Bold = true;
            s33.Font.Underline = UnderlineStyle.Single;
            s33.Font.FontName = "Times New Roman";
            s33.Font.Size = 12;
            s33.Font.Color = "#000000";
            s33.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s33.Alignment.Vertical = StyleVerticalAlignment.Center;
            // -----------------------------------------------
            //  s34
            // -----------------------------------------------
            WorksheetStyle s34 = styles.Add("s34");
            s34.Font.Bold = true;
            s34.Font.Underline = UnderlineStyle.Single;
            s34.Font.FontName = "Calibri";
            s34.Font.Size = 11;
            s34.Font.Color = "#000000";
            s34.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s34.Alignment.Vertical = StyleVerticalAlignment.Center;
            // -----------------------------------------------
            //  s35
            // -----------------------------------------------
            WorksheetStyle s35 = styles.Add("s35");
            s35.Font.FontName = "Calibri";
            s35.Font.Size = 11;
            s35.Font.Color = "#000000";
            s35.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s35.Alignment.Vertical = StyleVerticalAlignment.Center;
            s35.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s36
            // -----------------------------------------------
            WorksheetStyle s36 = styles.Add("s36");
            s36.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s36.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            // -----------------------------------------------
            //  s37
            // -----------------------------------------------
            WorksheetStyle s37 = styles.Add("s37");
            s37.Font.FontName = "Calibri";
            s37.Font.Size = 11;
            s37.Font.Color = "#000000";
            s37.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            s37.Alignment.Vertical = StyleVerticalAlignment.Center;
            s37.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s38
            // -----------------------------------------------
            WorksheetStyle s38 = styles.Add("s38");
            //s38.Parent = "s16";
            s38.Font.FontName = "Calibri";
            s38.Font.Size = 11;
            s38.Font.Color = "#000000";
            s38.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            s38.Alignment.Vertical = StyleVerticalAlignment.Center;
            s38.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s39
            // -----------------------------------------------
            WorksheetStyle s39 = styles.Add("s39");
            s39.Font.FontName = "Calibri";
            s39.Font.Size = 11;
            s39.Font.Color = "#000000";
            s39.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s39.Alignment.Vertical = StyleVerticalAlignment.Center;
            s39.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s40
            // -----------------------------------------------
            WorksheetStyle s40 = styles.Add("s40");
            s40.Font.Bold = true;
            s40.Font.FontName = "Calibri";
            s40.Font.Size = 11;
            s40.Font.Color = "#000000";
            s40.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            s40.Alignment.Vertical = StyleVerticalAlignment.Center;
            s40.Alignment.WrapText = true;
            //// -----------------------------------------------
            ////  s41
            //// -----------------------------------------------
            //WorksheetStyle s41 = styles.Add("s41");
            //s41.Font.Bold = true;
            //s41.Font.FontName = "Calibri";
            //s41.Font.Size = 11;
            //s41.Font.Color = "#000000";
            ////s41.NumberFormat = "_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)";
            // -----------------------------------------------
            //  s60
            // -----------------------------------------------
            WorksheetStyle s60 = styles.Add("s60");
            s60.Font.Bold = true;
            s60.Font.FontName = "Arial";
            //s60.Font.Color = "#000000";
            //s60.Interior.Color = "#B0C4DE";
            //s60.Interior.Pattern = StyleInteriorPattern.Solid;
            s60.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            s60.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s60.Alignment.WrapText = true;
            //s60.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            //s60.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            //s60.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            //s60.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");



            // -----------------------------------------------
            //  s61
            // -----------------------------------------------
            WorksheetStyle s61 = styles.Add("s61");
            s61.Font.Bold = true;
            s61.Font.FontName = "Arial";
            //s61.Font.Color = "#000000";
            //s61.Interior.Color = "#B0C4DE";
            //s61.Interior.Pattern = StyleInteriorPattern.Solid;
            s61.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s61.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s61.Alignment.WrapText = true;
            //s61.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            //s61.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            //s61.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            //s61.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");

            // -----------------------------------------------
            //  s62
            // -----------------------------------------------
            WorksheetStyle s62 = styles.Add("s62");
            s62.Font.Bold = true;
            s62.Font.FontName = "Arial";
            s62.Font.Size = 14;
            s62.Font.Color = "#000000";
            s62.Interior.Color = "#B0C4DE";
            s62.Interior.Pattern = StyleInteriorPattern.Solid;
            s62.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s62.Alignment.Vertical = StyleVerticalAlignment.Top;
            s62.Alignment.WrapText = true;
            //s62.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            //s62.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            //s62.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            //s62.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s63
            // -----------------------------------------------
            WorksheetStyle s63 = styles.Add("s63");
            //s63.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            //s63.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            //s63.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            //s63.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
            // -----------------------------------------------
            //  s64
            // -----------------------------------------------
            WorksheetStyle s64 = styles.Add("s64");
            s64.Font.FontName = "Arial";
            //s64.Font.Color = "#000000";
            //s64.Interior.Color = "#FFFFFF";
            //s64.Interior.Pattern = StyleInteriorPattern.Solid;
            s64.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s64.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s64.Alignment.WrapText = true;
            //s64.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            //s64.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            //s64.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            //s64.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            //s64.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
            // -----------------------------------------------
            //  s65
            // -----------------------------------------------
            WorksheetStyle s65 = styles.Add("s65");
            s65.Font.FontName = "Arial";
            //s65.Font.Color = "#000000";
            //s65.Interior.Color = "#FFFFFF";
            //s65.Interior.Pattern = StyleInteriorPattern.Solid;
            s65.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            s65.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            s65.Alignment.WrapText = true;
            //s65.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            //s65.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            //s65.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            //s65.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            //s65.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
            // -----------------------------------------------
            //  s66
            // -----------------------------------------------
            WorksheetStyle s66 = styles.Add("s66");
            s66.Font.FontName = "Arial";
            s66.Font.Color = "#FF0000";
            s66.Interior.Color = "#FFFFFF";
            s66.Interior.Pattern = StyleInteriorPattern.Solid;
            s66.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s66.Alignment.Vertical = StyleVerticalAlignment.Top;
            s66.Alignment.WrapText = true;
            s66.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            //s66.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            //s66.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            //s66.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            //s66.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
            // -----------------------------------------------
            //  s67
            // -----------------------------------------------
            WorksheetStyle s67 = styles.Add("s67");
            s67.Font.FontName = "Arial";
            s67.Font.Color = "#FF0000";
            s67.Interior.Color = "#FFFFFF";
            s67.Interior.Pattern = StyleInteriorPattern.Solid;
            s67.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            s67.Alignment.Vertical = StyleVerticalAlignment.Top;
            s67.Alignment.WrapText = true;
            s67.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            //s67.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            //s67.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            //s67.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            //s67.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
            // -----------------------------------------------
            //  s68
            // -----------------------------------------------
            WorksheetStyle s68 = styles.Add("s68");
            s68.Font.FontName = "Arial";
            s68.Font.Color = "#000000";
            s68.Interior.Color = "#ffe6e6";
            s68.Interior.Pattern = StyleInteriorPattern.Solid;
            s68.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s68.Alignment.Vertical = StyleVerticalAlignment.Top;
            s68.Alignment.WrapText = true;
            s68.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            //s68.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            //s68.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            //s68.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            //s68.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);

            // -----------------------------------------------
            //  s70
            // -----------------------------------------------
            WorksheetStyle s70 = styles.Add("s70");
            s70.Font.FontName = "Arial";
            s70.Font.Color = "#000000";
            s70.Interior.Color = "#d6f5d6";
            s70.Interior.Pattern = StyleInteriorPattern.Solid;
            s70.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s70.Alignment.Vertical = StyleVerticalAlignment.Top;
            s70.Alignment.WrapText = true;
            s70.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            s70.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            s70.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            s70.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            s70.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);

            // -----------------------------------------------
            //  s72
            // -----------------------------------------------
            WorksheetStyle s72 = styles.Add("s72");
            s72.Font.FontName = "Arial";
            s72.Font.Color = "#000000";
            s72.Font.Bold = true;
            s72.Interior.Color = "#d6f5d6";
            s72.Interior.Pattern = StyleInteriorPattern.Solid;
            s72.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            s72.Alignment.Vertical = StyleVerticalAlignment.Top;
            s72.Alignment.WrapText = true;
            s72.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            s72.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            s72.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            s72.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            s72.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);

            // -----------------------------------------------
            //  s137
            // -----------------------------------------------
            WorksheetStyle s137 = styles.Add("s137");
            s137.Name = "Normal 3";
            s137.Font.FontName = "Calibri";
            s137.Font.Size = 11;
            s137.Font.Color = "#000000";
            s137.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            // -----------------------------------------------
            //  m2611536909264
            // -----------------------------------------------
            WorksheetStyle m2611536909264 = styles.Add("m2611536909264");
            m2611536909264.Parent = "s137";
            m2611536909264.Font.FontName = "Arial";
            m2611536909264.Font.Color = "#9400D3";
            m2611536909264.Interior.Color = "#FFFFFF";
            m2611536909264.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611536909264.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611536909264.Alignment.WrapText = true;
            m2611536909264.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611536909264.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611536909284
            // -----------------------------------------------
            WorksheetStyle m2611536909284 = styles.Add("m2611536909284");
            m2611536909284.Parent = "s137";
            m2611536909284.Font.FontName = "Arial";
            m2611536909284.Font.Color = "#9400D3";
            m2611536909284.Interior.Color = "#FFFFFF";
            m2611536909284.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611536909284.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611536909284.Alignment.WrapText = true;
            m2611536909284.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611536909284.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");

            // -----------------------------------------------
            //  m2611536909285
            // -----------------------------------------------
            WorksheetStyle m2611536909285 = styles.Add("m2611536909285");
            m2611536909285.Parent = "s137";
            m2611536909285.Font.FontName = "Arial";
            m2611536909285.Font.Size = 14;
            m2611536909285.Font.Bold = true;
            m2611536909285.Font.Color = "#9400D3";
            m2611536909285.Interior.Color = "#FFFFFF";
            m2611536909285.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611536909285.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611536909285.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            m2611536909285.Alignment.WrapText = true;
            m2611536909285.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611536909285.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");

            // -----------------------------------------------
            //  m2611536909286
            // -----------------------------------------------
            WorksheetStyle m2611536909286 = styles.Add("m2611536909286");
            m2611536909286.Parent = "s137";
            m2611536909286.Font.FontName = "Arial";
            m2611536909286.Font.Color = "#9400D3";
            m2611536909286.Interior.Color = "#FFFFFF";
            m2611536909286.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611536909286.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611536909286.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            m2611536909286.Alignment.WrapText = true;
            m2611536909286.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611536909286.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611536909304
            // -----------------------------------------------
            WorksheetStyle m2611536909304 = styles.Add("m2611536909304");
            m2611536909304.Parent = "s137";
            m2611536909304.Font.FontName = "Arial";
            m2611536909304.Font.Color = "#9400D3";
            m2611536909304.Interior.Color = "#FFFFFF";
            m2611536909304.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611536909304.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611536909304.Alignment.WrapText = true;
            m2611536909304.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611536909304.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611536909324
            // -----------------------------------------------
            WorksheetStyle m2611536909324 = styles.Add("m2611536909324");
            m2611536909324.Parent = "s137";
            m2611536909324.Font.FontName = "Arial";
            m2611536909324.Font.Color = "#9400D3";
            m2611536909324.Interior.Color = "#FFFFFF";
            m2611536909324.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611536909324.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611536909324.Alignment.WrapText = true;
            m2611536909324.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611536909324.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611536909344
            // -----------------------------------------------
            WorksheetStyle m2611536909344 = styles.Add("m2611536909344");
            m2611536909344.Parent = "s137";
            m2611536909344.Font.FontName = "Arial";
            m2611536909344.Font.Color = "#9400D3";
            m2611536909344.Interior.Color = "#FFFFFF";
            m2611536909344.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611536909344.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611536909344.Alignment.WrapText = true;
            m2611536909344.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611536909344.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611540549552
            // -----------------------------------------------
            WorksheetStyle m2611540549552 = styles.Add("m2611540549552");
            m2611540549552.Parent = "s137";
            m2611540549552.Font.FontName = "Arial";
            m2611540549552.Font.Color = "#9400D3";
            m2611540549552.Interior.Color = "#FFFFFF";
            m2611540549552.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611540549552.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611540549552.Alignment.WrapText = true;
            m2611540549552.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611540549552.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611540549572
            // -----------------------------------------------
            WorksheetStyle m2611540549572 = styles.Add("m2611540549572");
            m2611540549572.Parent = "s137";
            m2611540549572.Font.FontName = "Arial";
            m2611540549572.Font.Color = "#9400D3";
            m2611540549572.Interior.Color = "#FFFFFF";
            m2611540549572.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611540549572.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611540549572.Alignment.WrapText = true;
            m2611540549572.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611540549572.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            m2611540549572.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611540549592
            // -----------------------------------------------
            WorksheetStyle m2611540549592 = styles.Add("m2611540549592");
            m2611540549592.Parent = "s137";
            m2611540549592.Font.FontName = "Arial";
            m2611540549592.Font.Color = "#9400D3";
            m2611540549592.Interior.Color = "#FFFFFF";
            m2611540549592.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611540549592.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611540549592.Alignment.WrapText = true;
            m2611540549592.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611540549592.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611540549612
            // -----------------------------------------------
            WorksheetStyle m2611540549612 = styles.Add("m2611540549612");
            m2611540549612.Parent = "s137";
            m2611540549612.Font.FontName = "Arial";
            m2611540549612.Font.Color = "#9400D3";
            m2611540549612.Interior.Color = "#FFFFFF";
            m2611540549612.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611540549612.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611540549612.Alignment.WrapText = true;
            m2611540549612.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611540549612.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611540549632
            // -----------------------------------------------
            WorksheetStyle m2611540549632 = styles.Add("m2611540549632");
            m2611540549632.Parent = "s137";
            m2611540549632.Font.FontName = "Arial";
            m2611540549632.Font.Color = "#9400D3";
            m2611540549632.Interior.Color = "#FFFFFF";
            m2611540549632.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611540549632.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611540549632.Alignment.WrapText = true;
            m2611540549632.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611540549632.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611540549652
            // -----------------------------------------------
            WorksheetStyle m2611540549652 = styles.Add("m2611540549652");
            m2611540549652.Parent = "s137";
            m2611540549652.Font.FontName = "Arial";
            m2611540549652.Font.Color = "#9400D3";
            m2611540549652.Interior.Color = "#FFFFFF";
            m2611540549652.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611540549652.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611540549652.Alignment.WrapText = true;
            m2611540549652.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611540549652.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  m2611540549672
            // -----------------------------------------------
            WorksheetStyle m2611540549672 = styles.Add("m2611540549672");
            m2611540549672.Parent = "s137";
            m2611540549672.Font.FontName = "Arial";
            m2611540549672.Font.Color = "#9400D3";
            m2611540549672.Interior.Color = "#FFFFFF";
            m2611540549672.Interior.Pattern = StyleInteriorPattern.Solid;
            m2611540549672.Alignment.Vertical = StyleVerticalAlignment.Top;
            m2611540549672.Alignment.WrapText = true;
            m2611540549672.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            m2611540549672.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");

            // -----------------------------------------------
            //  s139
            // -----------------------------------------------
            WorksheetStyle s139 = styles.Add("s139");
            s139.Parent = "s137";
            s139.Font.FontName = "Calibri";
            s139.Font.Size = 11;
            s139.Interior.Color = "#FFFFFF";
            s139.Interior.Pattern = StyleInteriorPattern.Solid;
            s139.Alignment.Vertical = StyleVerticalAlignment.Top;
            s139.Alignment.WrapText = true;
            // -----------------------------------------------
            //  s140
            // -----------------------------------------------
            WorksheetStyle s140 = styles.Add("s140");
            s140.Parent = "s137";
            s140.Font.FontName = "Calibri";
            s140.Font.Size = 11;
            s140.Interior.Color = "#FFFFFF";
            s140.Interior.Pattern = StyleInteriorPattern.Solid;
            s140.Alignment.Vertical = StyleVerticalAlignment.Top;
            s140.Alignment.WrapText = true;
            s140.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            s140.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s141
            // -----------------------------------------------
            WorksheetStyle s141 = styles.Add("s141");
            s141.Parent = "s137";
            s141.Font.FontName = "Calibri";
            s141.Font.Size = 11;
            s141.Interior.Color = "#FFFFFF";
            s141.Interior.Pattern = StyleInteriorPattern.Solid;
            s141.Alignment.Vertical = StyleVerticalAlignment.Top;
            s141.Alignment.WrapText = true;
            s141.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s142
            // -----------------------------------------------
            WorksheetStyle s142 = styles.Add("s142");
            s142.Parent = "s137";
            s142.Font.FontName = "Calibri";
            s142.Font.Size = 11;
            s142.Interior.Color = "#FFFFFF";
            s142.Interior.Pattern = StyleInteriorPattern.Solid;
            s142.Alignment.Vertical = StyleVerticalAlignment.Top;
            s142.Alignment.WrapText = true;
            s142.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            s142.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s143
            // -----------------------------------------------
            WorksheetStyle s143 = styles.Add("s143");
            s143.Parent = "s137";
            s143.Font.FontName = "Calibri";
            s143.Font.Size = 11;
            s143.Interior.Color = "#FFFFFF";
            s143.Interior.Pattern = StyleInteriorPattern.Solid;
            s143.Alignment.Vertical = StyleVerticalAlignment.Top;
            s143.Alignment.WrapText = true;
            s143.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s144
            // -----------------------------------------------
            WorksheetStyle s144 = styles.Add("s144");
            s144.Parent = "s137";
            s144.Font.FontName = "Arial";
            s144.Font.Color = "#9400D3";
            s144.Interior.Color = "#FFFFFF";
            s144.Interior.Pattern = StyleInteriorPattern.Solid;
            s144.Alignment.Vertical = StyleVerticalAlignment.Top;
            s144.Alignment.WrapText = true;
            s144.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            // -----------------------------------------------
            //  s145
            // -----------------------------------------------
            WorksheetStyle s145 = styles.Add("s145");
            s145.Parent = "s137";
            s145.Font.FontName = "Calibri";
            s145.Font.Size = 11;
            s145.Interior.Color = "#FFFFFF";
            s145.Interior.Pattern = StyleInteriorPattern.Solid;
            s145.Alignment.Vertical = StyleVerticalAlignment.Top;
            s145.Alignment.WrapText = true;
            s145.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s146
            // -----------------------------------------------
            WorksheetStyle s146 = styles.Add("s146");
            s146.Parent = "s137";
            s146.Font.FontName = "Calibri";
            s146.Font.Size = 11;
            s146.Interior.Color = "#FFFFFF";
            s146.Interior.Pattern = StyleInteriorPattern.Solid;
            s146.Alignment.Vertical = StyleVerticalAlignment.Top;
            s146.Alignment.WrapText = true;
            s146.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            s146.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s169
            // -----------------------------------------------
            WorksheetStyle s169 = styles.Add("s169");
            s169.Parent = "s137";
            s169.Font.FontName = "Arial";
            s169.Font.Color = "#9400D3";
            s169.Interior.Color = "#FFFFFF";
            s169.Interior.Pattern = StyleInteriorPattern.Solid;
            s169.Alignment.Vertical = StyleVerticalAlignment.Top;
            s169.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            // -----------------------------------------------
            //  s170
            // -----------------------------------------------
            WorksheetStyle s170 = styles.Add("s170");
            s170.Parent = "s137";
            s170.Font.FontName = "Calibri";
            s170.Font.Size = 11;
            s170.Interior.Color = "#FFFFFF";
            s170.Interior.Pattern = StyleInteriorPattern.Solid;
            s170.Alignment.Vertical = StyleVerticalAlignment.Top;
            // -----------------------------------------------
            //  s171
            // -----------------------------------------------
            WorksheetStyle s171 = styles.Add("s171");
            s171.Parent = "s137";
            s171.Font.FontName = "Calibri";
            s171.Font.Size = 11;
            s171.Interior.Color = "#FFFFFF";
            s171.Interior.Pattern = StyleInteriorPattern.Solid;
            s171.Alignment.Vertical = StyleVerticalAlignment.Top;
            s171.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s172
            // -----------------------------------------------
            WorksheetStyle s172 = styles.Add("s172");
            s172.Alignment.Vertical = StyleVerticalAlignment.Bottom;

            // -----------------------------------------------
            //  s169
            // -----------------------------------------------
            WorksheetStyle s169H = styles.Add("s169H");
            s169H.Parent = "s137";
            s169H.Font.FontName = "Arial";
            s169H.Font.Color = "#9400D3";
            s169H.Interior.Color = "#FFFFFF";
            s169H.Interior.Pattern = StyleInteriorPattern.Solid;
            s169H.Alignment.Vertical = StyleVerticalAlignment.Top;
            s169H.Alignment.ReadingOrder = StyleReadingOrder.LeftToRight;
            // -----------------------------------------------
            //  s170
            // -----------------------------------------------
            WorksheetStyle s170H = styles.Add("s170H");
            s170H.Parent = "s137";
            s170H.Font.FontName = "Calibri";
            s170H.Font.Size = 11;
            s170H.Interior.Color = "#FFFFFF";
            s170H.Interior.Pattern = StyleInteriorPattern.Solid;
            s170H.Alignment.Vertical = StyleVerticalAlignment.Top;
            // -----------------------------------------------
            //  s171
            // -----------------------------------------------
            WorksheetStyle s171H = styles.Add("s171H");
            s171H.Parent = "s137";
            s171H.Font.FontName = "Calibri";
            s171H.Font.Size = 11;
            s171H.Interior.Color = "#FFFFFF";
            s171H.Interior.Pattern = StyleInteriorPattern.Solid;
            s171H.Alignment.Vertical = StyleVerticalAlignment.Top;
            s171H.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  s172
            // -----------------------------------------------
            WorksheetStyle s172H = styles.Add("s172H");
            s172H.Alignment.Vertical = StyleVerticalAlignment.Bottom;

        }


        private void On_SaveExcelAudit_Closed(string PdfFileName)
        {
            Random_Filename = null;
            string PdfName = "Pdf File";
            PdfName = propReportPath + BaseForm.UserID + "\\" + PdfFileName+"_Audit";
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
                string Tmpstr = PdfName + ".xls";
                if (File.Exists(Tmpstr))
                    File.Delete(Tmpstr);
            }
            catch (Exception ex)
            {
                int length = 8;
                string newFileName = System.Guid.NewGuid().ToString();
                newFileName = newFileName.Replace("-", string.Empty);

                Random_Filename = PdfName + newFileName.Substring(0, length) + ".xls";
            }

            if (!string.IsNullOrEmpty(Random_Filename))
                PdfName = Random_Filename;
            else
                PdfName += ".xls";


            
                Workbook book = new Workbook();

                this.GenerateStyles(book.Styles);



                bool First = true; string MS_Type = string.Empty;
                string CAMSDesc = null; string Branch = null, Priv_Branch = null, SP_Plan_desc = null;
                bool SPNum = true; Worksheet sheet; WorksheetCell cell; WorksheetRow Row0; int Count = 1;


                if (dtAudit.Rows.Count > 0)
                {
                    this.GenerateWorksheetParameters(book.Worksheets);

                    string PrivSP = string.Empty;

                    var distinctRows = (from DataRow dRow in dtAudit.Rows
                                        select dRow["SP"]).Distinct();

                    foreach (var dRow in distinctRows)
                    {
                        DataTable dataTable = new DataTable();
                        DataView dv = new DataView(dtAudit);
                        dv.RowFilter = "SP='" + dRow + "'";
                        dataTable = dv.ToTable();

                        if (dataTable.Rows.Count > 0)
                        {
                            string ReportName = dataTable.Rows[0]["SP_Desc"].ToString();
                            ReportName = ReportName.Replace("/", "");

                            if (ReportName.Length >= 31)
                            {
                                ReportName = ReportName.Substring(0, 31);
                            }
                            sheet = book.Worksheets.Add(ReportName);
                            sheet.Table.DefaultRowHeight = 14.25F;


                            sheet.Table.Columns.Add(75);
                            sheet.Table.Columns.Add(75);
                            sheet.Table.Columns.Add(75);
                            sheet.Table.Columns.Add(75);
                            sheet.Table.Columns.Add(80);
                            sheet.Table.Columns.Add(145);
                            sheet.Table.Columns.Add(75);
                            sheet.Table.Columns.Add(80);
                            sheet.Table.Columns.Add(180);
                            sheet.Table.Columns.Add(80);
                            sheet.Table.Columns.Add(80);

                        Row0 = sheet.Table.Rows.Add();
                            Row0.AutoFitHeight = false;
                            //WorksheetCell cell;
                            cell = Row0.Cells.Add();
                            cell.StyleID = "s83";
                            cell.Data.Type = DataType.String;
                            cell.Data.Text = dataTable.Rows[0]["SP_Desc"].ToString();
                            cell.MergeAcross = 9;


                            Row0 = sheet.Table.Rows.Add();

                            cell = Row0.Cells.Add("Agency", DataType.String, "s94");
                            cell = Row0.Cells.Add("Dept", DataType.String, "s94");
                            cell = Row0.Cells.Add("Program", DataType.String, "s94");
                            cell = Row0.Cells.Add("Year", DataType.String, "s94");
                            cell = Row0.Cells.Add("App", DataType.String, "s94");
                            cell = Row0.Cells.Add("Name", DataType.String, "s94");
                            cell = Row0.Cells.Add("Type", DataType.String, "s94");
                            cell = Row0.Cells.Add("Code", DataType.String, "s94");
                            cell = Row0.Cells.Add("Description", DataType.String, "s94");
                            cell = Row0.Cells.Add("Date", DataType.String, "s94");
                            cell = Row0.Cells.Add("Amount", DataType.String, "s94");


                        int i = 0;
                            foreach (DataRow dr in dataTable.Rows)
                            {
                                Row0 = sheet.Table.Rows.Add();


                                string CAMSType = string.Empty;
                                if (dr["SP2_TYPE"].ToString().Trim() == "CA") CAMSType = "Service"; else CAMSType = "Outcome";



                            if (i % 2 == 0)
                            {
                                cell = Row0.Cells.Add(dr["Agency"].ToString(), DataType.String, "s105");
                                cell = Row0.Cells.Add(dr["Dept"].ToString(), DataType.String, "s105");
                                cell = Row0.Cells.Add(dr["Program"].ToString(), DataType.String, "s105");
                                cell = Row0.Cells.Add(dr["Prog_Year"].ToString(), DataType.String, "s105");
                                cell = Row0.Cells.Add(dr["AppNo"].ToString(), DataType.String, "s105");
                                cell = Row0.Cells.Add(dr["Name"].ToString(), DataType.String, "s105");
                                cell = Row0.Cells.Add(CAMSType, DataType.String, "s105");
                                cell = Row0.Cells.Add(dr["CAMS_CODE"].ToString(), DataType.String, "s105");
                                cell = Row0.Cells.Add(dr["CAMS_DESC"].ToString(), DataType.String, "s105");
                                cell = Row0.Cells.Add(LookupDataAccess.Getdate(dr["CAMS_DATE"].ToString()), DataType.String, "s105");
                                cell = Row0.Cells.Add(dr["CASEMS_COST"].ToString(), DataType.String, "s106");



                            }
                            else
                            {

                                cell = Row0.Cells.Add(dr["Agency"].ToString(), DataType.String, "s95");
                                cell = Row0.Cells.Add(dr["Dept"].ToString(), DataType.String, "s95");
                                cell = Row0.Cells.Add(dr["Program"].ToString(), DataType.String, "s95");
                                cell = Row0.Cells.Add(dr["Prog_Year"].ToString(), DataType.String, "s95");
                                cell = Row0.Cells.Add(dr["AppNo"].ToString(), DataType.String, "s95");
                                cell = Row0.Cells.Add(dr["Name"].ToString(), DataType.String, "s95");
                                cell = Row0.Cells.Add(CAMSType, DataType.String, "s95");
                                cell = Row0.Cells.Add(dr["CAMS_CODE"].ToString(), DataType.String, "s95");
                                cell = Row0.Cells.Add(dr["CAMS_DESC"].ToString(), DataType.String, "s95");
                                cell = Row0.Cells.Add(LookupDataAccess.Getdate(dr["CAMS_DATE"].ToString()), DataType.String, "s95");
                                cell = Row0.Cells.Add(dr["CASEMS_COST"].ToString(), DataType.String, "s95RC");


                            }




                                //if (dr["SP0_SERVICECODE"].ToString() != PrivSP)
                                //{



                                //    PrivSP = dr["SP0_SERVICECODE"].ToString();
                                //}


                                i++;
                            }
                        }


                    }



                }


                FileStream stream = new FileStream(PdfName, FileMode.Create);

                book.Save(stream);
                stream.Close();

            AlertBox.Show(PdfFileName + "_Audit.xls file Generated Successfully");

                ////FileDownloadGateway downloadGateway = new FileDownloadGateway();
                ////downloadGateway.Filename = "SPREPAPP_Report.xls";

                ////// downloadGateway.Version = file.Version;

                ////downloadGateway.SetContentType(DownloadContentType.OctetStream);

                ////downloadGateway.StartFileDownload(new ContainerControl(), PdfName);

                //FileInfo fiDownload = new FileInfo(PdfName);
                ///// Need to check for file exists, is local file, is allow to read, etc...
                //string name = fiDownload.Name;
                //using (FileStream fileStream = fiDownload.OpenRead())
                //{
                //    Application.Download(fileStream, name);
                //}

            //}

        }




    }
}