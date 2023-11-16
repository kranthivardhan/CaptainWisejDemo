/************************************************************************
 * Conversion On        :   11/28/2022
 * Converted By         :   Kranthi
 * Last Modification On :   11/28/2022
 * **********************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using Captain.Common.Views.Controls.Compatibility;
using Wisej.Web;
using DevExpress.DataAccess.Wizard;
using System.Xml;
#endregion

namespace Captain.Common.Views.Forms
{
    public partial class CASE4006_CAMSForm : Form
    {
        #region private variables

        private ErrorProvider _errorProvider = null;
        //private GridControl _intakeHierarchy = null;
        private bool boolChangeStatus = false;
        private CaptainModel _model = null;
        private List<HierarchyEntity> _selectedHierarchies = null;

        #endregion

        public CASE4006_CAMSForm(BaseForm baseform, string CAMS_flg, string CAMS_desc, string hierarchy, string year, CASEACTEntity pass_entity, PrivilegeEntity privileges, string MST_site, string MST_intakeWorker, List<FldcntlHieEntity> caFldcEntity, CASESP0Entity sp_header_rec, List<CASEACTEntity> CA_template_list, string sp_start_Date, string spm_site, string sp_End_Date, string mode)
        {
            InitializeComponent();
            pnlclt = new List<Panel>();
            BaseForm = baseform;
            CAMS_FLG = CAMS_flg;
            Hierarchy = hierarchy;
            Year = year;
            CAMS_Desc = CAMS_desc;
            Pass_CA_Entity = pass_entity;
            SP_Header_Rec = sp_header_rec;
            CA_Template_List = CA_template_list;
            Sp_Start_Date = sp_start_Date;
            Sp_End_Date = sp_End_Date;
            SPM_Site = spm_site;
            Mode = mode;
            //if (Pass_CA_Entity.Rec_Type == "I" && CA_template_list.Count > 0)
            //    Pass_CA_Entity = CA_template_list[0];

            Privileges = privileges;
            MST_Site = MST_site;
            MST_Intakeworker = MST_intakeWorker;
            CntlCAEntity = caFldcEntity;

            MS_Template_List = new List<CASEMSEntity>();

            //if (CAMS_desc.Length > 60)
            //    this.Text = CAMS_desc.Substring(0, 60);
            //else
            this.Text = CAMS_desc.Trim();

            _model = new CaptainModel();
            _errorProvider = new ErrorProvider(this);
            _errorProvider.BlinkRate = 3;
            //kranthi// _errorProvider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            _errorProvider.Icon = null;


            ACR_SERV_Hies = string.Empty;
            BaseAgencyControlDetails = _model.ZipCodeAndAgency.GetAgencyControlFile("00");
            if (!string.IsNullOrEmpty(BaseAgencyControlDetails.ServicePlanHiecontrol.ToString().Trim()))
                ACR_SERV_Hies = BaseAgencyControlDetails.ServicePlanHiecontrol.ToString();
            ACR_BenefitFrom = string.Empty;
            if (BaseAgencyControlDetails.CAOBO.Trim() == "Y")
            {
                if (Mode == "Add")
                {
                    pnlb3_CA_Services.Visible = true;
                    pnlclt.Add(pnlb3_CA_Services);

                    pnlb3_CA_Benefits.Visible = false;
                    lblCABenefit.Visible = pnlBenfitForm.Visible = true;
                    cmb_CA_Benefit.Visible = true;
                    btnCAOBO.Visible = true;
                    ACR_BenefitFrom = BaseAgencyControlDetails.BenefitFrom.Trim();
                    FillApplicantData();
                    this.gvbtnCABenefit.Visible = true;
                    this.gvbtnMSBenefit.Visible = true;


                }
                else
                {

                    lblCABenefit.Visible = pnlBenfitForm.Visible = false;
                    cmb_CA_Benefit.Visible = false;
                    btnCAOBO.Visible = false;
                    this.gvbtnCABenefit.Visible = true;
                    this.gvbtnMSBenefit.Visible = true;
                }
            }
            else
            {
                lblCABenefit.Visible = pnlBenfitForm.Visible = false;
                cmb_CA_Benefit.Visible = false;
                btnCAOBO.Visible = false;
                this.gvbtnCABenefit.Visible = false;
                this.gvbtnMSBenefit.Visible = false;
            }

            Getdata();
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

            Act_Date.Text = DateTime.Today.ToShortDateString();
            dtActSeek_Date.Text = DateTime.Today.ToShortDateString();
            Check_Date.Text = DateTime.Today.ToShortDateString();
            UpOn_Date.Text = DateTime.Today.ToShortDateString();
            Complete_Date.Text = DateTime.Today.ToShortDateString();

            if (Mode == "Edit")
            {
                //this.Text = privileges.Program + " Activity Posting - Edit ";
                //this.Text = "Auto Post CA - Edit ";

                //Added by Sudheer on 01/18/2021
                lblAutoPost.Visible = false;

                ////commented by Sudheer on 05/22/2020 
                //if (CustomQuestions == "N")
                //    this.Size = new System.Drawing.Size(846, 491);
                ////this.Size = new System.Drawing.Size(846, 513);
                //else
                //    this.Size = new System.Drawing.Size(846, 607);

                this.Size = new System.Drawing.Size(this.Size.Width, FormFixHight);

                pnlb3_CA_Services.Visible = false;
                if (BaseAgencyControlDetails.CAOBO.Trim() == "Y")
                {
                    pnlb3_CA_Benefits.Visible = true;
                    pnlclt.Add(pnlb3_CA_Benefits);
                }

                //this.CA_Benefits.Location = new System.Drawing.Point(1, 305);
                this.pnlb3_CA_Benefits.Location = new System.Drawing.Point(1, 280);

                //lblCustques.Visible = true;
                //btnSAL.Visible = true;

                pnlb3_outcomes.Visible = false;
                //commented by sudheer on 05/22/2020
                if (CustomQuestions == "N")
                    this.panel6.Location = new System.Drawing.Point(1, 458);
                //this.panel6.Location = new System.Drawing.Point(1, 480);
                else
                {
                    pnlb3_CustomQues.Visible = true;
                    pnlclt.Add(pnlb3_CustomQues);
                    this.pnlb3_CustomQues.Location = new System.Drawing.Point(1, 458);//423
                    this.panel6.Location = new System.Drawing.Point(1, 579);
                }
                Get_App_CASEACT_List();

                if (CustomQuestions == "Y") Fill_SAL_Custom_Questions();//Fill_Custom_Questions();

                if (CustomQuestions == "N")
                    this.panel6.Location = new System.Drawing.Point(1, 458);
            }
            else
            {

                //this.Text = privileges.Program + " Activity Posting - Add ";
                //this.Text = "Auto Post CA - Add ";
                this.Size = new System.Drawing.Size(this.Size.Width, FormFixHight);
                pnlb3_outcomes.Visible = true;
                pnlclt.Add(pnlb3_outcomes);

                // this.panel6.Location = new System.Drawing.Point(1, 579);

                if (CustomQuestions == "Y") SP_CA_Grid.Columns[31].Visible = true; else SP_CA_Grid.Columns[31].Visible = false;



            }

            Fill_DropDowns();
            Fill_CA_Members_DropDown();
            //Get_ReferrTo_Data();

            //Fill_Custom_Questions();
            GetData(SP_Header_Rec.Code.Trim(), pass_entity.Branch);
            Fill_CA_Benefiting_From();
            FillResultscombo();

            switch (CAMS_FLG)
            {
                case "CA":
                    //this.Size = new System.Drawing.Size(605, 599);
                    ////this.Size = new System.Drawing.Size(605, 420); //405//602, 350
                    CAPanel.Visible = panel2.Visible = true;
                    ////LblProgramReq.Visible = Cmb_Program.Enabled = true;
                    Fill_Program_Combo();
                    if (Pass_CA_Entity.Rec_Type == "U")
                    {
                        Fill_CA_Controls();
                        //FillMSgrid(SP_Header_Rec.Code.Trim(), Pass_CA_Entity.Branch);
                    }
                    else if (CA_template_list.Count > 0)
                        Fill_CA_Controls_4rm_Template();
                    break;



            }

            if (Mode == "Add")
            {

                Fill_SP_CAMS_Details(SP_Header_Rec.Code.Trim(), pass_entity.Branch, null);
                Tools["tlCaseNotes"].Visible = false;
                //Padding newPadding = new Padding(30, 1, 30, 1);
                ////SP_CA_Grid.RowTemplate.DefaultCellStyle.Padding = newPadding;
                //SP_CA_Grid.Columns["gvbtnCABenefit"].DefaultCellStyle.Padding = newPadding;
                //SP_CA_Grid.Columns["gvbtnCABenefit"].DefaultCellStyle.BackColor = Color.Transparent;

                if (CustomQuestions == "Y") SP_CA_Grid.Columns[31].Visible = true; else SP_CA_Grid.Columns[31].Visible = false;
            }
            else if (Pass_CA_Entity.Rec_Type == "U" || Mode == "Edit")
            {
                //Added by Sudheer on 01/18/2021 to hide the SSN Column
                //this.CA_Mem_SSN.Visible = false;
                //this.CA_Mem_Name.Width = 317;
                //this.CA_Mem_Relation.Width = 140;

                Mode = "Edit";
                Get_PROG_Notes_Status();
                Get_Vendor_List();
                GetAgencyDetails();
                //Fill_CA_Benefiting_From();

                //Fill_CA_Controls();



            }
            else
            {
                ////this.Text = privileges.Program + " Activity Posting - Add " + (CA_Template_List.Count > 0 ? "(Template)" : "");
                //this.Text = "Auto Post CA - Add " + (CA_Template_List.Count > 0 ? "(Template)" : "");
                ////  Pb_CA_Notes.Visible = false; pbPdf.Visible = false;
                this.Tools["tlCaseNotes"].Visible = false; this.Tools["tlpdf"].Visible = false;
            }



            
            EnableDisableControls();
            Txt_Cost.Validator = TextBoxValidation.FloatValidator;
            txtRate.Validator = TextBoxValidation.FloatValidator;
            //Txt_Cost3.Validator = TextBoxValidation.FloatValidator;
            //Txtx_ChkNo.Validator = TextBoxValidation.IntegerValidator;
            Txt_Units.Validator = TextBoxValidation.IntegerValidator;
            //Txt_Units2.Validator = TextBoxValidation.IntegerValidator;
            //Txt_Units3.Validator = TextBoxValidation.IntegerValidator;
            propReportPath = _model.lookupDataAccess.GetReportPath();

            dockingPanels();

        }

        int FormFixHight = 705;
        public CASE4006_CAMSForm(BaseForm baseform, string CAMS_flg, string CAMS_desc, string hierarchy, string year, CASEACTEntity pass_entity, PrivilegeEntity privileges, string MST_site, string MST_intakeWorker, List<FldcntlHieEntity> caFldcEntity, CASESP0Entity sp_header_rec, List<CASEACTEntity> CA_template_list, string sp_start_Date, string spm_site, string sp_End_Date, List<CASESP2Entity> SP2_CAMS_Details)
         {
            InitializeComponent();
            pnlclt = new List<Panel>();
            BaseForm = baseform;
            CAMS_FLG = CAMS_flg;
            Hierarchy = hierarchy;
            Year = year;
            CAMS_Desc = CAMS_desc;
            Pass_CA_Entity = pass_entity;
            SP_Header_Rec = sp_header_rec;
            CA_Template_List = CA_template_list;
            Sp_Start_Date = sp_start_Date;
            Sp_End_Date = sp_End_Date;
            SPM_Site = spm_site;
            //if (Pass_CA_Entity.Rec_Type == "I" && CA_template_list.Count > 0)
            //    Pass_CA_Entity = CA_template_list[0];
            SP2_CA_Details = SP2_CAMS_Details;
            Privileges = privileges;
            MST_Site = MST_site;
            MST_Intakeworker = MST_intakeWorker;
            CntlCAEntity = caFldcEntity;

            if (CAMS_desc.Length > 60)
                this.Text = CAMS_desc.Substring(0, 60);
            else
                this.Text = CAMS_desc.Trim();

            _model = new CaptainModel();
            _errorProvider = new ErrorProvider(this);
            _errorProvider.BlinkRate = 3;
            _errorProvider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            _errorProvider.Icon = null;

            ACR_SERV_Hies = string.Empty;
            BaseAgencyControlDetails = _model.ZipCodeAndAgency.GetAgencyControlFile("00");
            if (!string.IsNullOrEmpty(BaseAgencyControlDetails.ServicePlanHiecontrol.ToString().Trim()))
                ACR_SERV_Hies = BaseAgencyControlDetails.ServicePlanHiecontrol.ToString();
            if (BaseAgencyControlDetails.CAOBO.Trim() == "Y")
            {
                //if (Mode == "Add")
                //{
                //    CA_Services.Visible = true;
                //    CA_Benefits.Visible = false;
                //    lblCABenefit.Visible = true;
                //    cmb_CA_Benefit.Visible = true;
                //    btnCAOBO.Visible = true;
                //    ACR_BenefitFrom = BaseAgencyControlDetails.BenefitFrom.Trim();
                //    FillApplicantData();
                //}
                //else
                //{
                ACR_BenefitFrom = BaseAgencyControlDetails.BenefitFrom.Trim();
                FillApplicantData();
                lblCABenefit.Visible = pnlBenfitForm.Visible = false;
                cmb_CA_Benefit.Visible = false;
                btnCAOBO.Visible = false;

                //}
            }
            else
            {
                lblCABenefit.Visible = pnlBenfitForm.Visible = false;
                cmb_CA_Benefit.Visible = false;
                btnCAOBO.Visible = false;
            }

            Mode = "Add";
            //if (Pass_CA_Entity.Rec_Type == "U")
            //{
            //    this.Text = privileges.Program + " Activity Posting - Edit ";
            //    //Act_Date.Enabled = false;
            //    Mode = "Edit";
            //    Get_PROG_Notes_Status();
            //    Get_Vendor_List();
            //    GetAgencyDetails();
            //    //if (dtAgency.Rows.Count > 0)
            //    //{
            //    //    if(dtAgency.Rows[0]["ACR_CA_PVOUCHER"].ToString().Trim()=="Y")
            //    //        pbPdf.Visible = true;
            //    //}
            //}
            //else
            //{

            ////this.Text = privileges.Program + " Activity Posting - Add " + (CA_Template_List.Count > 0 ? "(Template)" : "");

            //this.Text = "Auto Post CA - Add " + (CA_Template_List.Count > 0 ? "(Template)" : "");

            Act_Date.Text = DateTime.Today.ToShortDateString();
            dtActSeek_Date.Text = DateTime.Today.ToShortDateString();
            Check_Date.Text = DateTime.Today.ToShortDateString();
            UpOn_Date.Text = DateTime.Today.ToShortDateString();
            Complete_Date.Text = DateTime.Today.ToShortDateString();

            MS_Template_List = new List<CASEMSEntity>();
            //if (CustomQuestions == "N")

            this.Size = new System.Drawing.Size(this.Size.Width, FormFixHight);


            ////this.Size = new System.Drawing.Size(846, 513);
            //else
            //    this.Size = new System.Drawing.Size(846, 607);

            pnlb3_CA_Services.Visible = false;
            pnlb3_CA_Benefits.Visible = true;
            pnlclt.Add(pnlb3_CA_Benefits);

            //this.CA_Benefits.Location = new System.Drawing.Point(1, 305);
            //this.CA_Benefits.Location = new System.Drawing.Point(1, 280);

            //lblCustques.Visible = true;
            //btnSAL.Visible = true;

            pnlb3_outcomes.Visible = false;
            //commented by sudheer on 05/22/2020
            //if (CustomQuestions == "N")
            //this.panel6.Location = new System.Drawing.Point(1, 458);
            ////this.panel6.Location = new System.Drawing.Point(1, 480);
            //else
            //{
            pnlb3_CustomQues.Visible = false;
            //this.panel4.Location = new System.Drawing.Point(1, 458);//423
            //    this.panel6.Location = new System.Drawing.Point(1, 579);
            //}

            //Pb_CA_Notes.Visible = false; pbPdf.Visible = false;
            this.Tools["tlCaseNotes"].Visible = false; this.Tools["tlpdf"].Visible = false;
            //}

            Fill_DropDowns();
            Fill_CA_Members_DropDown();
            //Get_ReferrTo_Data();

            //Fill_Custom_Questions();
            gvAutoCAGrid.Visible = true;
            Fill_CA_Benefiting_From();
            GetData(SP_Header_Rec.Code.Trim(), pass_entity.Branch);
            FillAutoCAGrid();

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

            switch (CAMS_FLG)
            {
                //case "MS": CAPanel.Visible = false;
                //    this.MSPanel.Location = new System.Drawing.Point(2, 2);
                //    this.Size = new System.Drawing.Size(507, 387);
                //    break;

                case "CA":
                    //this.Size = new System.Drawing.Size(605, 599);
                    ////this.Size = new System.Drawing.Size(605, 420); //405//602, 350
                    CAPanel.Visible = panel2.Visible = true;
                    ////LblProgramReq.Visible = Cmb_Program.Enabled = true;
                    Fill_Program_Combo();
                    if (Pass_CA_Entity.Rec_Type == "U")
                    {
                        Fill_CA_Controls();
                        //FillMSgrid(SP_Header_Rec.Code.Trim(), Pass_CA_Entity.Branch);
                    }
                    else if (CA_template_list.Count > 0)
                        Fill_CA_Controls_4rm_Template();
                    break;

            }
            EnableDisableControls();
            Txt_Cost.Validator = TextBoxValidation.FloatValidator;
            txtRate.Validator = TextBoxValidation.FloatValidator;
            //Txtx_ChkNo.Validator = TextBoxValidation.IntegerValidator;
            Txt_Units.Validator = TextBoxValidation.IntegerValidator;
            propReportPath = _model.lookupDataAccess.GetReportPath();

            dockingPanels();
        }


        //Added by Sudheer on 12/19/2022
        public CASE4006_CAMSForm(BaseForm baseform, string CAMS_flg, string CAMS_desc, string hierarchy, string year, CASEACTEntity pass_entity, PrivilegeEntity privileges, string MST_site, string MST_intakeWorker, List<FldcntlHieEntity> caFldcEntity, CASESP0Entity sp_header_rec, List<CASEACTEntity> CA_template_list, string sp_start_Date, string spm_site, string sp_End_Date, string mode, List<CASEMSEntity> MS_template_list)
        {
            InitializeComponent();
            pnlclt = new List<Panel>();
            BaseForm = baseform;
            CAMS_FLG = CAMS_flg;
            Hierarchy = hierarchy;
            Year = year;
            CAMS_Desc = CAMS_desc;
            Pass_CA_Entity = pass_entity;
            SP_Header_Rec = sp_header_rec;
            CA_Template_List = CA_template_list;
            MS_Template_List = MS_template_list;
            Pass_MS_Entity = new CASEMSEntity();
            Sp_Start_Date = sp_start_Date;
            Sp_End_Date = sp_End_Date;
            SPM_Site = spm_site;
            Mode = mode;
            //if (Pass_CA_Entity.Rec_Type == "I" && CA_template_list.Count > 0)
            //    Pass_CA_Entity = CA_template_list[0];

            Privileges = privileges;
            MST_Site = MST_site;
            MST_Intakeworker = MST_intakeWorker;
            CntlCAEntity = caFldcEntity;

            //if (CAMS_desc.Length > 60)
            //    this.Text = CAMS_desc.Substring(0, 60);
            //else
            this.Text = CAMS_desc.Trim();

            _model = new CaptainModel();
            _errorProvider = new ErrorProvider(this);
            _errorProvider.BlinkRate = 3;
            //kranthi// _errorProvider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            _errorProvider.Icon = null;

            lblSel.Text = "1 of " + (CA_Template_List.Count + MS_Template_List.Count);

            ACR_SERV_Hies = string.Empty;
            BaseAgencyControlDetails = _model.ZipCodeAndAgency.GetAgencyControlFile("00");
            if (!string.IsNullOrEmpty(BaseAgencyControlDetails.ServicePlanHiecontrol.ToString().Trim()))
                ACR_SERV_Hies = BaseAgencyControlDetails.ServicePlanHiecontrol.ToString();
            ACR_BenefitFrom = string.Empty;
            if (BaseAgencyControlDetails.CAOBO.Trim() == "Y")
            {
                if (Mode == "Add")
                {
                    //Commented by Sudheer on 12/19/2022

                    //pnlb3_CA_Services.Visible = true;
                    //pnlclt.Add(pnlb3_CA_Services);

                    //pnlb3_CA_Benefits.Visible = false;
                    //lblCABenefit.Visible = pnlBenfitForm.Visible = true;
                    //cmb_CA_Benefit.Visible = true;
                    //btnCAOBO.Visible = true;
                    //ACR_BenefitFrom = BaseAgencyControlDetails.BenefitFrom.Trim();
                    //FillApplicantData();
                    //this.gvbtnCABenefit.Visible = true;
                    //this.gvbtnMSBenefit.Visible = true;


                    //Added by Sudheer on 12/19/2022
                    lblCABenefit.Visible = pnlBenfitForm.Visible = false;
                    cmb_CA_Benefit.Visible = false;
                    btnCAOBO.Visible = false;
                    this.gvbtnCABenefit.Visible = true;
                    this.gvbtnMSBenefit.Visible = true;

                    //pnlQPStatus.Visible = true;

                }
                else
                {

                    lblCABenefit.Visible = pnlBenfitForm.Visible = false;
                    cmb_CA_Benefit.Visible = false;
                    btnCAOBO.Visible = false;
                    this.gvbtnCABenefit.Visible = true;
                    this.gvbtnMSBenefit.Visible = true;
                }
            }
            else
            {
                lblCABenefit.Visible = pnlBenfitForm.Visible = false;
                cmb_CA_Benefit.Visible = false;
                btnCAOBO.Visible = false;
                this.gvbtnCABenefit.Visible = false;
                this.gvbtnMSBenefit.Visible = false;
            }

            Getdata();

            Fill_Results();
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

            Act_Date.Text = DateTime.Today.ToShortDateString();
            dtActSeek_Date.Text = DateTime.Today.ToShortDateString();
            Check_Date.Text = DateTime.Today.ToShortDateString();
            UpOn_Date.Text = DateTime.Today.ToShortDateString();
            Complete_Date.Text = DateTime.Today.ToShortDateString();

            if (Mode == "Edit")
            {
                //this.Text = privileges.Program + " Activity Posting - Edit ";
                //this.Text = "Auto Post CA - Edit ";

                //Added by Sudheer on 01/18/2021
                lblAutoPost.Visible = false;

                ////commented by Sudheer on 05/22/2020 
                //if (CustomQuestions == "N")
                //    this.Size = new System.Drawing.Size(846, 491);
                ////this.Size = new System.Drawing.Size(846, 513);
                //else
                //    this.Size = new System.Drawing.Size(846, 607);

                this.Size = new System.Drawing.Size(this.Size.Width, FormFixHight);

                pnlb3_CA_Services.Visible = false;
                if (BaseAgencyControlDetails.CAOBO.Trim() == "Y")
                {
                    pnlb3_CA_Benefits.Visible = true;
                    pnlclt.Add(pnlb3_CA_Benefits);
                }

                //this.CA_Benefits.Location = new System.Drawing.Point(1, 305);
                this.pnlb3_CA_Benefits.Location = new System.Drawing.Point(1, 280);

                //lblCustques.Visible = true;
                //btnSAL.Visible = true;

                pnlb3_outcomes.Visible = false;
                //commented by sudheer on 05/22/2020
                if (CustomQuestions == "N")
                    this.panel6.Location = new System.Drawing.Point(1, 458);
                //this.panel6.Location = new System.Drawing.Point(1, 480);
                else
                {
                    pnlb3_CustomQues.Visible = true;
                    pnlclt.Add(pnlb3_CustomQues);
                    this.pnlb3_CustomQues.Location = new System.Drawing.Point(1, 458);//423
                    this.panel6.Location = new System.Drawing.Point(1, 579);
                }
                Get_App_CASEACT_List();

                if (CustomQuestions == "Y") Fill_SAL_Custom_Questions();//Fill_Custom_Questions();


            }
            else
            {
                //Commented by Sudheer on 12/19/2022
                ////this.Text = privileges.Program + " Activity Posting - Add ";
                ////this.Text = "Auto Post CA - Add ";
                //this.Size = new System.Drawing.Size(this.Size.Width, FormFixHight);
                //pnlb3_outcomes.Visible = true;
                //pnlclt.Add(pnlb3_outcomes);

                //// this.panel6.Location = new System.Drawing.Point(1, 579);

                //if (CustomQuestions == "Y") SP_CA_Grid.Columns[30].Visible = true; else SP_CA_Grid.Columns[30].Visible = false;


                //Added by Sudheer on 12/19/2022
                //this.Text = privileges.Program + " Activity Posting - Edit ";
                //this.Text = "Auto Post CA - Edit ";

                //Added by Sudheer on 01/18/2021
                lblAutoPost.Visible = false;



                this.Size = new System.Drawing.Size(this.Size.Width, FormFixHight);

                pnlb3_CA_Services.Visible = false;
                if (BaseAgencyControlDetails.CAOBO.Trim() == "Y")
                {
                    pnlb3_CA_Benefits.Visible = true;
                    pnlclt.Add(pnlb3_CA_Benefits);
                }

                this.pnlb3_CA_Benefits.Location = new System.Drawing.Point(1, 280);

                pnlb3_outcomes.Visible = false;
                if (CustomQuestions == "N")
                    this.panel6.Location = new System.Drawing.Point(1, 458);
                else
                {
                    pnlb3_CustomQues.Visible = true;
                    pnlclt.Add(pnlb3_CustomQues);
                    this.pnlb3_CustomQues.Location = new System.Drawing.Point(1, 458);//423
                    this.panel6.Location = new System.Drawing.Point(1, 579);
                }
                Get_App_CASEACT_List();

                if (CustomQuestions == "Y") Fill_SAL_Custom_Questions();//Fill_Custom_Questions();


            }

            Fill_DropDowns();
            Fill_CA_Members_DropDown();
            //Get_ReferrTo_Data();

            //Fill_Custom_Questions();
            GetData(SP_Header_Rec.Code.Trim(), pass_entity.Branch);
            Fill_CA_Benefiting_From();


            if (Mode == "Add")
            {
                //Commented on Sudheer on 12/19/2022
                //Fill_SP_CAMS_Details(SP_Header_Rec.Code.Trim(), pass_entity.Branch, null);


                //Added by Sudheer on 12/19/2022
                //this.CA_Mem_SSN.Visible = false;
                //this.CA_Mem_Name.Width = 317;
                //this.CA_Mem_Relation.Width = 140;

                //Mode = "Edit";
                Get_PROG_Notes_Status();
                Get_Vendor_List();
                GetAgencyDetails();

            }
            else if (Pass_CA_Entity.Rec_Type == "U" || Mode == "Edit")
            {
                //Added by Sudheer on 01/18/2021 to hide the SSN Column
                //this.CA_Mem_SSN.Visible = false;
                //this.CA_Mem_Name.Width = 317;
                //this.CA_Mem_Relation.Width = 140;

                Mode = "Edit";
                Get_PROG_Notes_Status();
                Get_Vendor_List();
                GetAgencyDetails();
                //Fill_CA_Benefiting_From();

                //Fill_CA_Controls();



            }
            else
            {
                ////this.Text = privileges.Program + " Activity Posting - Add " + (CA_Template_List.Count > 0 ? "(Template)" : "");
                //this.Text = "Auto Post CA - Add " + (CA_Template_List.Count > 0 ? "(Template)" : "");
                ////  Pb_CA_Notes.Visible = false; pbPdf.Visible = false;
                this.Tools["tlCaseNotes"].Visible = false; this.Tools["tlpdf"].Visible = false;
            }



            switch (CAMS_FLG)
            {
                case "CA":
                    //this.Size = new System.Drawing.Size(605, 599);
                    ////this.Size = new System.Drawing.Size(605, 420); //405//602, 350
                    CAPanel.Visible = panel2.Visible = true;
                    ////LblProgramReq.Visible = Cmb_Program.Enabled = true;
                    Fill_Program_Combo();
                    if (Pass_CA_Entity.Rec_Type == "U")
                    {
                        Fill_CA_Controls();
                        //FillMSgrid(SP_Header_Rec.Code.Trim(), Pass_CA_Entity.Branch);
                    }
                    //else if (CA_template_list.Count > 0)
                    //    Fill_CA_Controls_4rm_Template();
                    break;



            }
            EnableDisableControls();
            Txt_Cost.Validator = TextBoxValidation.FloatValidator;
            txtRate.Validator = TextBoxValidation.FloatValidator;
            //Txt_Cost3.Validator = TextBoxValidation.FloatValidator;
            //Txtx_ChkNo.Validator = TextBoxValidation.IntegerValidator;
            Txt_Units.Validator = TextBoxValidation.IntegerValidator;
            //Txt_Units2.Validator = TextBoxValidation.IntegerValidator;
            //Txt_Units3.Validator = TextBoxValidation.IntegerValidator;
            propReportPath = _model.lookupDataAccess.GetReportPath();

            dockingPanels();

        }



        #region properties

        public BaseForm BaseForm { get; set; }

        public List<FldcntlHieEntity> CntlCAEntity { get; set; }

        public List<FldcntlHieEntity> CntlMSEntity { get; set; }

        public AgencyControlEntity BaseAgencyControlDetails { get; set; }

        public SERVSTOPEntity SERVStopEntity { get; set; }

        public string CAMS_FLG { get; set; }

        public DataSet dsAgency { get; set; }

        public DataTable dtAgency { get; set; }

        public string propReportPath { get; set; }

        public string CAMS_Desc { get; set; }

        public string Hierarchy { get; set; }

        public string Year { get; set; }

        public string MST_Site { get; set; }

        public string Mode { get; set; }

        public string SPM_Site { get; set; }

        public string MST_Intakeworker { get; set; }

        public string Sp_Start_Date { get; set; }

        public string Sp_End_Date { get; set; }

        public string ACR_BenefitFrom { get; set; }

        public string ACR_SERV_Hies { get; set; }

        public CASEACTEntity Pass_CA_Entity { get; set; }

        public CASEMSEntity Pass_MS_Entity { get; set; }

        public PrivilegeEntity Privileges { get; set; }

        public CASESP0Entity SP_Header_Rec { get; set; }

        public List<CASEACTEntity> CA_Template_List { get; set; }

        public List<CASEMSEntity> MS_Template_List { get; set; }

        public List<CAOBOEntity> CA_OBO_List { get; set; }

        public string CustomQuestions { get; set; }

        public List<SaldefEntity> SaldefList { get; set; }

        public List<CASESP2Entity> SP2_CA_Details { get; set; }

        public List<SalquesEntity> SALQUESEntity { get; set; }
        public List<SalqrespEntity> SALQUESRespEntity { get; set; }

        #endregion

        private void Get_App_CASEACT_List()
        {

            CASEACTEntity Search_Enty = new CASEACTEntity(true);
            Search_Enty.Agency = Pass_CA_Entity.Agency;
            Search_Enty.Dept = Pass_CA_Entity.Dept;
            Search_Enty.Program = Pass_CA_Entity.Program;
            Search_Enty.Year = Pass_CA_Entity.Year;                             // Year will be always Four-Spaces in CASEACT
            Search_Enty.App_no = Pass_CA_Entity.App_no;
            Search_Enty.SPM_Seq = Pass_CA_Entity.SPM_Seq;
            Search_Enty.Service_plan = Pass_CA_Entity.Service_plan;
            Search_Enty.ACT_Code = Pass_CA_Entity.ACT_Code;
            Search_Enty.ACT_Date = Pass_CA_Entity.ACT_Date;
            Search_Enty.ACT_Seq = Pass_CA_Entity.ACT_Seq;

            SP_Activity_Details = _model.SPAdminData.Browse_CASEACT(Search_Enty, "Browse", "CASEACT2.0");

            SP_Activity_Details = SP_Activity_Details.OrderByDescending(u => Convert.ToDateTime(u.ACT_Date.Trim())).ToList();

            if (SP_Activity_Details.Count > 0)
                Pass_CA_Entity = new CASEACTEntity(SP_Activity_Details[0]);

        }

        private void Getdata()
        {
            List<SaldefEntity> SALDEFEntity = new List<SaldefEntity>();
            SaldefEntity Search_saldef_Entity = new SaldefEntity(true);

            List<SaldefEntity> SALDEF = _model.SALDEFData.Browse_SALDEF(Search_saldef_Entity, "Browse", BaseForm.UserID, BaseForm.BaseAgency);
            if (BaseForm.BaseAgencyControlDetails.ServicePlanHiecontrol.Trim() == "Y")
            {
                if (!string.IsNullOrEmpty(Pass_CA_Entity.Act_PROG.Trim()))
                {
                    SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                    if (SALDEFEntity.Count == 0)
                        SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                    if (SALDEFEntity.Count == 0)
                        SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                }
                else
                {
                    SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Contains(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                    if (SALDEFEntity.Count == 0)
                        SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                    if (SALDEFEntity.Count == 0)
                        SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                    //SALDEF = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_TYPE.Equals("S"));
                }
            }
            else
            {
                SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Contains(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                if (SALDEFEntity.Count == 0)
                    SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                if (SALDEFEntity.Count == 0)
                    SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S")&& u.SALD_5QUEST == "Y");
                //SALDEF = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_TYPE.Equals("S"));
            }
            //if (SALDEF.Count > 0)
            //{
            //    SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST=="Y");
            //}

            if (SALDEFEntity.Count > 0) 
            { 
                CustomQuestions = "Y"; SaldefList = SALDEFEntity;
                SALQUESEntity = new List<SalquesEntity>();
                if (SALDEFEntity.Count > 0)
                {
                    SalquesEntity Search_Salques_Entity = new SalquesEntity(true);
                    Search_Salques_Entity.SALQ_SALD_ID = SALDEFEntity[0].SALD_ID;
                    SALQUESEntity = _model.SALDEFData.Browse_SALQUES(Search_Salques_Entity, "Browse");

                    if (SALQUESEntity.Count > 1) CustomQuestions = "Y";else CustomQuestions = "N";

                    //if (SALQUESEntity.Count > 0) SALQUESEntity = SALQUESEntity.OrderBy(u => Convert.ToInt32(u.SALQ_GRP_SEQ)).ThenBy(u => Convert.ToInt32(u.SALQ_SEQ)).ThenBy(u => Convert.ToInt32(u.SALQ_GRP_CODE)).ToList();
                }

            } 
            else CustomQuestions = "N";


        }

        private void EnableDisableControls()
        {
            //if (CAMS_FLG == "MS")
            //{


            //    if (!CntlMSEntity.Exists(u => u.Enab.Equals("Y")))
            //    {
            //        //MessageBox.Show("Field controls not defined for this program");
            //        Btn_MS_Save.Enabled = false;
            //    }

            //    foreach (FldcntlHieEntity entity in CntlMSEntity)
            //    {
            //        bool required = entity.Req.Equals("Y") ? true : false;
            //        bool enabled = entity.Enab.Equals("Y") ? true : false;

            //        switch (entity.FldCode)
            //        {
            //            case Consts.CASE0006.Date:
            //                if (enabled) { MS_Date.Enabled = lblMsDate.Enabled = true; if (required) lblMsDateReq.Visible = true; } else { MS_Date.Enabled = lblMsDate.Enabled = false; lblMsDateReq.Visible = false; }
            //                break;
            //            case Consts.CASE0006.MsSite:
            //                if (enabled) { lblSite.Enabled = Cmb_MS_Site.Enabled = true; if (required) lblSiteReq.Visible = true; } else { lblSite.Enabled = lblSiteReq.Enabled = false; Cmb_MS_Site.Visible = false; }
            //                break;
            //            case Consts.CASE0006.Result:
            //                if (enabled) { lblResult.Enabled = Cmb_MS_Results.Enabled = true; if (required) lblResultReq.Visible = true; } else { Cmb_MS_Results.Enabled = lblResult.Enabled = false; lblResultReq.Visible = false; }
            //                break;

            //            case Consts.CASE0006.MsCaseWorker:
            //                if (enabled) { Cmb_MS_Worker.Enabled = lblMsCasewor.Enabled = true; if (required) lblMsCaseworReq.Visible = true; } else { Cmb_MS_Worker.Enabled = lblMsCasewor.Enabled = false; lblMsCaseworReq.Visible = false; }
            //                break;
            //            case Consts.CASE0006.MS_Acty_Program:
            //                if (enabled) { Pb_MS_Prog.Visible = lblMSProgram.Enabled = true; if (required) lblMSProgramReq.Visible = true; } else { Pb_MS_Prog.Visible = Pb_MS_Prog.Visible = lblMSProgram.Enabled = false; lblMSProgramReq.Visible = false; }
            //                break;
            //            case Consts.CASE0006.BenefitingfromServiceActivity:
            //                if (enabled) { Cmb_MS_Benefit.Enabled = lblBenefit.Enabled = true; if (required) lblBenefitReq.Visible = true; } else { Cmb_MS_Benefit.Enabled = lblBenefit.Enabled = false; lblBenefitReq.Visible = false; }
            //                break;
            //            //case Consts.CASE0006.BenefitingfromCAServiceActivity:
            //            //    if (enabled) { cmb_CA_Benefit.Enabled = label18.Enabled = true; if (required) label17.Visible = true; } else { cmb_CA_Benefit.Enabled = label18.Enabled = false; label17.Visible = false; }
            //            //    break;
            //            case Consts.CASE0006.MS_Seek_Date:
            //                if (enabled) { dtMSSeek_Date.Visible = lblMsSeekDate.Visible = true; if (required) lblMSSeekDateReq.Visible = true; } else { dtMSSeek_Date.Visible = lblMsSeekDate.Visible = false; lblMSSeekDateReq.Visible = false; }
            //                break;
            //        }
            //    }
            //}
            if (CAMS_FLG == "CA")
            {
                if (!CntlCAEntity.Exists(u => u.Enab.Equals("Y")))
                {
                    //MessageBox.Show("Field controls not defined for this program");
                    Btn_CASave.Enabled = false;
                }

                foreach (FldcntlHieEntity entity in CntlCAEntity)
                {
                    bool required = entity.Req.Equals("Y") ? true : false;
                    bool enabled = entity.Enab.Equals("Y") ? true : false;

                    switch (entity.FldCode)
                    {
                        case Consts.CASE0006.FundingSource1:
                            if (enabled) { lblFunSour1.Enabled = CmbFunding1.Enabled = true; if (required) lblFunSour1Req.Visible = true; } else { lblFunSour1.Enabled = CmbFunding1.Enabled = false; lblFunSour1Req.Visible = false; }
                            break;
                        //case Consts.CASE0006.FundingSource2:
                        //    if (enabled) { lblFunSour2.Enabled = CmbFunding2.Enabled = true; if (required) lblFunSour2Req.Visible = true; } else { lblFunSour2.Enabled = CmbFunding2.Enabled = false; lblFunSour2Req.Visible = false; }
                        //    break;
                        case Consts.CASE0006.FundingSource2:
                            if (enabled) { lblFunSour2.Visible = CmbFunding2.Visible = true; if (required) lblFunSour2Req.Visible = true; } else { lblFunSour2.Visible = CmbFunding2.Visible = false; lblFunSour2Req.Visible = false; }
                            break;
                        //case Consts.CASE0006.FundingSource3:
                        //    if (enabled) { lblFunSour3.Enabled = CmbFunding3.Enabled = true; if (required) lblFunSour3Req.Visible = true; } else { lblFunSour3.Enabled = CmbFunding3.Enabled = false; lblFunSour3Req.Visible = false; }
                        //    break;
                        case Consts.CASE0006.FundingSource3:
                            if (enabled) { lblFunSour3.Visible = CmbFunding3.Visible = true; if (required) lblFunSour3Req.Visible = true; } else { lblFunSour3.Visible = CmbFunding3.Visible = false; lblFunSour3Req.Visible = false; }
                            break;
                        case Consts.CASE0006.ActCaseWorker:
                            if (enabled) { CmbWorker.Enabled = lblCaseworca.Enabled = pnlCaseworker.Visible = true; if (required) lblCaseworcaReq.Visible = true; } else { CmbWorker.Enabled = lblCaseworca.Enabled = pnlCaseworker.Visible = false; lblCaseworcaReq.Visible = false; }
                            break;
                        case Consts.CASE0006.Site:
                            if (enabled) { CmbSite.Enabled = lblSiteca.Enabled = pnlsite.Visible = true; if (required) lblSitecaReq.Visible = true; } else { CmbSite.Enabled = lblSiteca.Enabled = pnlsite.Visible = false; lblSitecaReq.Visible = false; }
                            break;
                        case Consts.CASE0006.VendorNo:
                            if (enabled) { Txt_VendNo.Enabled = Text_VendName.Enabled = lblVendor.Enabled = panel_Referral2.Visible = true; if (required) lblVendorReq.Visible = true; } else { Txt_VendNo.Enabled = Text_VendName.Enabled = lblVendor.Enabled = panel_Referral2.Visible = false; lblVendorReq.Visible = false; }
                            break;
                        case Consts.CASE0006.CheckNo:
                            if (enabled) { Txtx_ChkNo.Enabled = lblCheck.Enabled = true; if (required) lblCheckReq.Visible = true; } else { Txtx_ChkNo.Enabled = lblCheck.Enabled = false; lblCheckReq.Visible = false; }
                            break;
                        case Consts.CASE0006.CheckDate:
                            if (enabled) { Check_Date.Enabled = lblCheckDate.Enabled = true; if (required) lblCheckDateReq.Visible = true; } else { Check_Date.Enabled = lblCheckDate.Enabled = false; lblCheckDateReq.Visible = false; }
                            break;
                        case Consts.CASE0006.TobeFollowUpBy:
                            if (enabled) { Txt_TobeFollowUp.Enabled = lblTobeFolled.Enabled = true; if (required) lblTobeFolledReq.Visible = true; } else { Txt_TobeFollowUp.Enabled = lblTobeFolled.Enabled = false; lblTobeFolledReq.Visible = false; }
                            break;
                        case Consts.CASE0006.FollowUPOn:
                            if (enabled) { UpOn_Date.Enabled = lblFollowup.Enabled = true; if (required) lblFollowupReq.Visible = true; } else { UpOn_Date.Enabled = lblFollowup.Enabled = false; lblFollowupReq.Visible = false; }
                            break;
                        case Consts.CASE0006.FollowUpComplete:
                            if (enabled) { Complete_Date.Enabled = lblFollowupCompl.Enabled = true; if (required) lblFollowupComplReq.Visible = true; } else { Complete_Date.Enabled = lblFollowupCompl.Enabled = false; lblFollowupComplReq.Visible = false; }
                            break;
                        case Consts.CASE0006.Cost:
                            if (enabled) { Txt_Cost.Enabled = lblCost.Enabled = true; if (required) lblCostReq.Visible = true; } else { Txt_Cost.Enabled = lblCost.Enabled = false; lblCostReq.Visible = false; }
                            break;
                        //case Consts.CASE0006.Act_Acty_Program:
                        //    if (enabled) { Cmb_Program.Enabled = LblProgram.Enabled = true; if (required) LblProgramReq.Visible = true; } else { Cmb_Program.Enabled = LblProgram.Enabled = false; LblProgramReq.Visible = false; }
                        case Consts.CASE0006.Act_Acty_Program:
                            if (enabled) { Pb_CA_Prog.Visible = LblProgram.Enabled = true; if (required) LblProgramReq.Visible = true; } else { Pb_CA_Prog.Visible = Pb_CA_Prog.Visible = LblProgram.Enabled = false; LblProgramReq.Visible = false; }
                            break;
                        case Consts.CASE0006.Act_UOM:
                            if (enabled) { Cmb_UOM.Enabled = Lbl_UOM.Enabled = true; if (required) LblUOM_Req.Visible = true; } else { Cmb_UOM.Enabled = Lbl_UOM.Enabled = false; LblUOM_Req.Visible = false; }
                            //if (enabled) { Lbl_UOM.Visible = Cmb_UOM.Visible = Lbl_UOM.Visible = Cmb_UOM.Enabled = Lbl_UOM.Enabled = true; if (required) LblUOM_Req.Visible = true; } else { Lbl_UOM.Visible = Cmb_UOM.Visible = Lbl_UOM.Visible = Cmb_UOM.Enabled = Lbl_UOM.Enabled = false; LblUOM_Req.Visible = false; }
                            break;
                        case Consts.CASE0006.Act_Units:
                            //if (enabled) { Txt_Units.Enabled = Lbl_Units.Enabled = true; if (required) LblUnits_Req.Visible = true; } else { Txt_Units.Enabled = Lbl_Units.Enabled = false; LblUnits_Req.Visible = false; }
                            if (enabled) { Lbl_Units.Visible = Txt_Units.Visible = Lbl_Units.Visible = Txt_Units.Enabled = Lbl_Units.Enabled = true; if (required) LblUnits_Req.Visible = true; } else { Lbl_Units.Visible = Txt_Units.Visible = Lbl_Units.Visible = Txt_Units.Enabled = Lbl_Units.Enabled = false; LblUnits_Req.Visible = false; }
                            break;
                        case Consts.CASE0006.Act_Seek_Date:
                            if (enabled) { dtActSeek_Date.Visible = lblActSeekDate.Visible = pnlActseekReqdt.Visible = true; if (required) lblActSeekDateReq.Visible = true; } else { dtActSeek_Date.Visible = lblActSeekDate.Visible = pnlActseekReqdt.Visible = false; lblActSeekDateReq.Visible = false; }
                            break;
                        //case Consts.CASE2001.InitialDate:
                        //    if (enabled) { dtpInitialDate.Enabled = lblInitialDate.Enabled = true; if (required) lblInitialDateReq.Visible = true; } else { dtpInitialDate.Enabled = lblInitialDate.Enabled = false; lblInitialDateReq.Visible = false; }
                        //    break;
                        case Consts.CASE0006.Act_Rate:
                            if (enabled) { txtRate.Visible = lblRate.Visible = true; if (required) lblRateReq.Visible = true; } else { txtRate.Visible = lblRate.Visible = false; lblRateReq.Visible = false; }
                            break;
                            //case Consts.CASE0006.Cost3:
                            //    if (enabled) { Txt_Cost3.Enabled = lblCost3.Enabled = true; if (required) lblCostReq3.Visible = true; } else { Txt_Cost3.Enabled = lblCost3.Enabled = false; lblCostReq3.Visible = false; }
                            //    break;
                            //case Consts.CASE0006.Act_UOM2:
                            //    if (enabled) { Cmb_UOM2.Enabled = Lbl_UOM2.Enabled = true; if (required) LblUOM_Req2.Visible = true; } else { Cmb_UOM2.Enabled = Lbl_UOM2.Enabled = false; LblUOM_Req2.Visible = false; }
                            //    break;
                            //case Consts.CASE0006.Act_Units2:
                            //    if (enabled) { Lbl_Units2.Visible = Txt_Units2.Visible = Lbl_Units2.Visible = Txt_Units2.Enabled = Lbl_Units2.Enabled = true; if (required) LblUnits_Req2.Visible = true; } else { Lbl_Units2.Visible = Txt_Units2.Visible = Lbl_Units2.Visible = Txt_Units2.Enabled = Lbl_Units2.Enabled = false; LblUnits_Req2.Visible = false; }
                            //    break;
                            //case Consts.CASE0006.Act_UOM3:
                            //    if (enabled) { Cmb_UOM3.Enabled = Lbl_UOM3.Enabled = true; if (required) LblUOM_Req3.Visible = true; } else { Cmb_UOM3.Enabled = Lbl_UOM3.Enabled = false; LblUOM_Req3.Visible = false; }
                            //    break;
                            //case Consts.CASE0006.Act_Units3:
                            //    if (enabled) { Lbl_Units3.Visible = Txt_Units3.Visible = Lbl_Units3.Visible = Txt_Units3.Enabled = Lbl_Units3.Enabled = true; if (required) LblUnits_Req3.Visible = true; } else { Lbl_Units3.Visible = Txt_Units3.Visible = Lbl_Units3.Visible = Txt_Units3.Enabled = Lbl_Units3.Enabled = false; LblUnits_Req3.Visible = false; }
                            //    break;

                    }
                }






            }
        }

        List<Panel> pnlclt = new List<Panel>();
        void dockingPanels()
        {

            /*** * Kranthi on 12//01/2022 Docking the grids* ***/

            if (pnlclt.Count == 1)
            {
                Panel panel = pnlclt[0];
                panel.Dock = DockStyle.Fill;
            }
            if (pnlclt.Count > 1)
            {
                Panel panel = pnlclt[1];
                panel.Dock = DockStyle.Fill;
            }
            //List<Panel> PanelCollection = pnlblock3.Controls.OfType<Panel>().Where(c => c.Name.Contains("pnlb3_") && c.Visible == true).ToList();



        }
        private void Get_PROG_Notes_Status()
        {
            if (Mode.Equals("Edit"))
            {
                List<CaseNotesEntity> caseNotesEntity = new List<CaseNotesEntity>();
                string Notes_Field_Name = null;

                if (CAMS_FLG == "CA")
                    Notes_Field_Name = Hierarchy + Pass_CA_Entity.Year + Pass_CA_Entity.App_no + Pass_CA_Entity.Service_plan.Trim() + Pass_CA_Entity.SPM_Seq + Pass_CA_Entity.Branch.Trim() +
                            Pass_CA_Entity.Group.ToString() + "CA" + Pass_CA_Entity.ACT_Code.Trim() + Pass_CA_Entity.ACT_Seq + Pass_CA_Entity.ACT_ID;
                else
                    Notes_Field_Name = Hierarchy + Pass_MS_Entity.Year + Pass_MS_Entity.App_no + Pass_MS_Entity.Service_plan.Trim() + Pass_MS_Entity.SPM_Seq + Pass_MS_Entity.Branch.Trim() +
                            Pass_MS_Entity.Group.ToString() + "MS" + Pass_MS_Entity.MS_Code.Trim() + CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Pass_MS_Entity.Date.ToString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat); ;


                caseNotesEntity = _model.TmsApcndata.GetCaseNotesScreenFieldName((CAMS_FLG == "CA" ? "CASE00063" : "CASE00064"), Notes_Field_Name.Trim());
                Tools["tlCaseNotes"].ImageSource = Consts.Icons.ico_CaseNotes_New;

                //Pb_MS_Notes.ImageSource = Consts.Icons.ico_CaseNotes_New;

                if (caseNotesEntity.Count > 0)
                {

                    switch (CAMS_FLG)
                    {
                        case "CA":
                            if (Pass_CA_Entity.Rec_Type == "I")
                                Tools["tlCaseNotes"].Visible = true;

                            break;

                            //case "MS":
                            //    if (Pass_MS_Entity.Rec_Type == "I")
                            //        Pb_MS_Notes.Visible = true;
                            //    break;
                    }
                    Tools["tlCaseNotes"].ImageSource = Consts.Icons.ico_CaseNotes_View;
                    //Pb_MS_Notes.ImageSource = Consts.Icons.ico_CaseNotes_View;
                }
            }
        }

        DataTable dtmembers = new DataTable();
        private void FillApplicantData()
        {
            DataSet ds = Captain.DatabaseLayer.MainMenu.MainMenuSearch("APP", "All", null, null, Pass_CA_Entity.App_no, null, null, null, null, null, null, null, null, null, null, Hierarchy + Year, null, BaseForm.UserID, string.Empty, string.Empty, string.Empty);
            if (ds.Tables.Count > 0)
            {
                dtmembers = ds.Tables[0];
            }
        }

        private void FillAutoCAGrid()
        {
            gvAutoCAGrid.Rows.Clear();
            if (SP2_CA_Details.Count > 0)
            {
                int rowIndex = 0;
                foreach (CASESP2Entity Entity in SP2_CA_Details)
                {
                    rowIndex = gvAutoCAGrid.Rows.Add(Entity.CamCd.Trim(), Entity.CAMS_Desc.Trim());

                    rowIndex++;
                }
            }

            if (gvAutoCAGrid.Rows.Count > 0)
                gvAutoCAGrid.Rows[0].Selected = true;
        }

        List<CASEVDDEntity> CaseVddlist = new List<CASEVDDEntity>();
        private void Get_Vendor_List()
        {
            CASEVDDEntity Search_Entity = new CASEVDDEntity(true);
            CaseVddlist = _model.SPAdminData.Browse_CASEVDD(Search_Entity, "Browse");

            if (BaseForm.BaseAgencyControlDetails.AgyVendor == "Y")
                CaseVddlist = CaseVddlist.FindAll(u => u.VDD_Agency == BaseForm.BaseAgency);
        }

        private void GetAgencyDetails()
        {
            dsAgency = DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(BaseForm.BaseAgency, null, null, null, null, null, null);
            dtAgency = dsAgency.Tables[0];
        }

        private void Fill_DropDowns()
        {
            Fill_Sites();
            Fill_CaseWorker();

            if (CAMS_FLG == "CA")
            {
                Fill_Funding();
                Fill_UOM();
            }
            //else
            //    Fill_Results();
        }

        string Tmp_SPM_Sequence = "1", Sel_CAMS_Program = "";
        List<HierarchyEntity> SP_Programs_List = new List<HierarchyEntity>();
        private void Fill_Program_Combo()
        {
            List<CASESPMEntity> CASESPM_List = new List<CASESPMEntity>();
            CASESPMEntity Search_Entity = new CASESPMEntity(true);

            if (Pass_CA_Entity.Rec_Type == "I")
            {
                Search_Entity.agency = BaseForm.BaseAgency;
                Search_Entity.dept = BaseForm.BaseDept;
                Search_Entity.program = BaseForm.BaseProg;
                Search_Entity.app_no = BaseForm.BaseApplicationNo;
                Search_Entity.service_plan = Pass_CA_Entity.Service_plan;

                CASESPM_List = _model.SPAdminData.Browse_CASESPM(Search_Entity, "Browse");
            }

            int TmpRows = 1;
            SP_Programs_List = _model.lookupDataAccess.Get_SerPlan_Prog_List(BaseForm.UserProfile.UserID, Pass_CA_Entity.Service_plan, ACR_SERV_Hies);
            Txt_CA_Program.Clear();
            if (SP_Programs_List.Count > 0)
            {
                //if (ds.Tables[0].Rows.Count > 0)
                {
                    string Tmp_Hierarchy = " ";
                    int ProgIndex = 0; bool DefHieExist = false;
                    try
                    {
                        if (CASESPM_List.Count > 0)
                        {
                            foreach (HierarchyEntity Ent in SP_Programs_List)
                            {
                                Tmp_Hierarchy = Ent.Agency.Trim() + Ent.Dept.Trim() + Ent.Prog.Trim();
                                if (Pass_CA_Entity.Rec_Type == "I" && ProgIndex == 0)
                                {
                                    foreach (CASESPMEntity Entity in CASESPM_List)
                                    {
                                        if (Entity.Def_Program == Tmp_Hierarchy)
                                        {
                                            Sel_CAMS_Program = Tmp_Hierarchy + " - " + Ent.HirarchyName.Trim();
                                            DefHieExist = true;
                                            ProgIndex = TmpRows;
                                            break;
                                        }
                                    }

                                }
                                else if (Pass_CA_Entity.Rec_Type != "I")
                                {
                                    if (Pass_CA_Entity.Act_PROG == Tmp_Hierarchy)
                                    {
                                        Sel_CAMS_Program = Tmp_Hierarchy + " - " + Ent.HirarchyName.Trim();
                                        DefHieExist = true;
                                        ProgIndex = TmpRows;
                                        break;
                                    }
                                }
                                TmpRows++;
                            }
                        }
                        else
                        {
                            foreach (HierarchyEntity Ent in SP_Programs_List)
                            {
                                Tmp_Hierarchy = Ent.Agency.Trim() + Ent.Dept.Trim() + Ent.Prog.Trim();
                                if (Pass_CA_Entity.Rec_Type == "I" && ProgIndex == 0)
                                {
                                    if (Pass_CA_Entity.Act_PROG == Tmp_Hierarchy)
                                    {
                                        Sel_CAMS_Program = Tmp_Hierarchy + " - " + Ent.HirarchyName.Trim();
                                        DefHieExist = true;
                                        ProgIndex = TmpRows;
                                        break;
                                    }


                                }
                                else if(Pass_CA_Entity.Rec_Type!="I")
                                {
                                    if(Pass_CA_Entity.Act_PROG== Tmp_Hierarchy)
                                    {
                                        Sel_CAMS_Program = Tmp_Hierarchy + " - " + Ent.HirarchyName.Trim();
                                        DefHieExist = true;
                                        ProgIndex = TmpRows;
                                        break;
                                    }
                                }
                                TmpRows++;
                            }
                        }
                    }
                    catch (Exception ex) { }

                    if (TmpRows > 0)
                    {
                        Txt_CA_Program.Text = Sel_CAMS_Program;

                        if (!string.IsNullOrEmpty(Txt_CA_Program.Text.Trim()))
                            Pass_CA_Entity.Act_PROG = Txt_CA_Program.Text.Substring(0, 6);
                        else
                            Pass_CA_Entity.Act_PROG = string.Empty;

                        List<SaldefEntity> SALDEFEntity = new List<SaldefEntity>();
                        SaldefEntity Search_saldef_Entity = new SaldefEntity(true);

                        List<SaldefEntity> SALDEF = _model.SALDEFData.Browse_SALDEF(Search_saldef_Entity, "Browse", BaseForm.UserID, BaseForm.BaseAgency);
                        if (BaseForm.BaseAgencyControlDetails.ServicePlanHiecontrol.Trim() == "Y")
                        {
                            if (!string.IsNullOrEmpty(Pass_CA_Entity.Act_PROG.Trim()))
                            {
                                SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                                if (SALDEFEntity.Count == 0)
                                    SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                                if (SALDEFEntity.Count == 0)
                                    SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                            }
                            else
                            {
                                SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Contains(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S"));
                                if (SALDEFEntity.Count == 0)
                                    SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                                if (SALDEFEntity.Count == 0)
                                    SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                                //SALDEF = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_TYPE.Equals("S"));
                            }
                        }
                        else
                        {
                            SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Contains(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S"));
                            if (SALDEFEntity.Count == 0)
                                SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                            if (SALDEFEntity.Count == 0)
                                SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                            //SALDEF = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_TYPE.Equals("S"));
                        }

                        if (SALDEFEntity.Count > 0) 
                        { 
                            CustomQuestions = "Y"; SaldefList = SALDEFEntity;

                            SALQUESEntity = new List<SalquesEntity>();
                            if (SALDEFEntity.Count > 0)
                            {
                                SalquesEntity Search_Salques_Entity = new SalquesEntity(true);
                                Search_Salques_Entity.SALQ_SALD_ID = SALDEFEntity[0].SALD_ID;
                                SALQUESEntity = _model.SALDEFData.Browse_SALQUES(Search_Salques_Entity, "Browse");

                                if (SALQUESEntity.Count > 1) CustomQuestions = "Y"; else CustomQuestions = "N";
                               
                            }

                        } else CustomQuestions = "N";

                        if (Mode == "Add")
                        {
                            if (CustomQuestions == "Y") SP_CA_Grid.Columns[31].Visible = true; else SP_CA_Grid.Columns[31].Visible = false;
                        }
                    }
                }

            }
            else
                AlertBox.Show("Programs Are Not Defined", MessageBoxIcon.Information, null);
        }

        List<CustRespEntity> CustResp_List;
        private void set_CustGrid_Rows_On_Questions(bool DropDown_Exists)
        {
            CustRespEntity Search_Entity = new CustRespEntity();

            Search_Entity.ScrCode = "CASE0063";
            Search_Entity.RecType = Search_Entity.ResoCode = Search_Entity.RespSeq = null;
            Search_Entity.RespDesc = Search_Entity.DescCode = Search_Entity.AddDate = Search_Entity.AddOpr = null;
            Search_Entity.ChgDate = Search_Entity.ChgOpr = Search_Entity.Changed = Search_Entity.ScrCode = null;

            if (DropDown_Exists)
                CustResp_List = _model.FieldControls.Browse_CUSTRESP(Search_Entity, "Browse");

        }

        List<CustfldsEntity> Cust;


        private void Fill_Sites()
        {
            CmbSite.Items.Clear();
            //Cmb_MS_Site.Items.Clear();
            //Cmb_Bulk_Site.Items.Clear();

            CmbSite.ColorMember = "FavoriteColor";
            //Cmb_MS_Site.ColorMember = "FavoriteColor";

            List<Captain.Common.Utilities.ListItem> listItem = new List<Captain.Common.Utilities.ListItem>();
            listItem.Add(new Captain.Common.Utilities.ListItem("   ", "0", " ", Color.White));


            DataSet ds = Captain.DatabaseLayer.CaseMst.GetSiteByHIE(BaseForm.BaseAgency, string.Empty, string.Empty);
            if (ds.Tables.Count > 0)
            {
                DataTable Sites_Table = ds.Tables[0];
                if (Sites_Table.Rows.Count > 0)
                {
                    if (Mode.Equals("Add"))
                    {
                        DataView dv = new DataView(Sites_Table);
                        dv.RowFilter = "SITE_ACTIVE='Y'";
                        Sites_Table = dv.ToTable();
                    }

                    foreach (DataRow dr in Sites_Table.Rows)
                        listItem.Add(new Captain.Common.Utilities.ListItem(dr["SITE_NAME"].ToString(), dr["SITE_NUMBER"].ToString().Trim(), dr["SITE_ACTIVE"].ToString().Trim(), (dr["SITE_ACTIVE"].ToString().Trim().Equals("Y") ? Color.Black : Color.Red)));

                }
                if (BaseForm.BaseAgencyControlDetails.SiteSecurity == "1")
                {
                    List<HierarchyEntity> userHierarchy = _model.UserProfileAccess.GetUserHierarchyByID(BaseForm.UserID);
                    HierarchyEntity hierarchyEntity = new HierarchyEntity(); List<CaseSiteEntity> selsites = new List<CaseSiteEntity>();
                    foreach (HierarchyEntity Entity in userHierarchy)
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

                    if (hierarchyEntity != null)
                    {
                        List<Captain.Common.Utilities.ListItem> listItemSite = new List<Captain.Common.Utilities.ListItem>();
                        listItemSite.Add(new Captain.Common.Utilities.ListItem("   ", "0", " ", Color.White));
                        if (hierarchyEntity.Sites.Length > 0)
                        {
                            string[] Sites = hierarchyEntity.Sites.Split(',');

                            for (int i = 0; i < Sites.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(Sites[i].ToString().Trim()))
                                {
                                    foreach (Captain.Common.Utilities.ListItem casesite in listItem) //Site_List)//ListcaseSiteEntity)
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
                            listItem = listItemSite;
                        }

                    }
                }
            }
            if (CAMS_FLG == "CA")
                CmbSite.Items.AddRange(listItem.ToArray());
            //else if (CAMS_FLG == "MS")
            //    Cmb_MS_Site.Items.AddRange(listItem.ToArray());
            //else
            //{
            //    Cmb_Bulk_Site.Items.AddRange(listItem.ToArray());
            //    Cmb_Bulk_Site.Items.Insert(0, new Captain.Common.Utilities.ListItem("All Sites", "0"));
            //    Cmb_Bulk_Site.SelectedIndex = 0;
            //}


            switch (CAMS_FLG)
            {
                case "CA":
                    if (Pass_CA_Entity.Rec_Type == "I") //!string.IsNullOrEmpty(MST_Site))
                    {
                        if (CA_Template_List.Count == 0)
                        {
                            if (!string.IsNullOrEmpty(BaseForm.UserProfile.Site.Trim()) && BaseForm.UserProfile.Site.Trim() != "****")
                            {
                                SetComboBoxValue(CmbSite, BaseForm.UserProfile.Site.Trim().Substring(2, BaseForm.UserProfile.Site.Trim().Length - 2));
                            }
                            else if (!string.IsNullOrEmpty(MST_Site))
                                SetComboBoxValue(CmbSite, MST_Site);
                            else if (!string.IsNullOrEmpty(SPM_Site))
                                SetComboBoxValue(CmbSite, SPM_Site);
                            else CmbSite.SelectedIndex = 0;
                        }
                        else if (!string.IsNullOrEmpty(MST_Site))
                            SetComboBoxValue(CmbSite, MST_Site);
                        else if (!string.IsNullOrEmpty(SPM_Site))
                            SetComboBoxValue(CmbSite, SPM_Site);
                        else CmbSite.SelectedIndex = 0;
                    }
                    else
                        CmbSite.SelectedIndex = 0;
                    break;
                    //case "MS":
                    //    if (Pass_MS_Entity.Rec_Type == "I")//&& !string.IsNullOrEmpty(MST_Site))
                    //    {
                    //        if (!string.IsNullOrEmpty(BaseForm.UserProfile.Site.Trim()) && BaseForm.UserProfile.Site.Trim() != "****")
                    //        {
                    //            SetComboBoxValue(Cmb_MS_Site, BaseForm.UserProfile.Site.Trim().Substring(2, BaseForm.UserProfile.Site.Trim().Length - 2));
                    //        }
                    //        else if (!string.IsNullOrEmpty(MST_Site))
                    //            SetComboBoxValue(Cmb_MS_Site, MST_Site);
                    //        else if (!string.IsNullOrEmpty(SPM_Site))
                    //            SetComboBoxValue(Cmb_MS_Site, SPM_Site);
                    //        else Cmb_MS_Site.SelectedIndex = 0;
                    //    }
                    //    else
                    //        Cmb_MS_Site.SelectedIndex = 0;
                    //    break;
            }
        }

        private void Fill_CaseWorker()
        {

            DataSet ds2 = Captain.DatabaseLayer.AgyTab.GetHierarchyNames(Hierarchy.Substring(0, 2), "**", "**");
            string strNameFormat = null, strCwFormat = null;
            if (ds2.Tables[0].Rows.Count > 0)
            {
                strNameFormat = ds2.Tables[0].Rows[0]["HIE_CN_FORMAT"].ToString();
                strCwFormat = ds2.Tables[0].Rows[0]["HIE_CW_FORMAT"].ToString();
            }

            CmbWorker.Items.Clear(); CmbWorker.ColorMember = "FavoriteColor";
            //Cmb_MS_Worker.Items.Clear(); Cmb_MS_Worker.ColorMember = "FavoriteColor";

            List<Captain.Common.Utilities.ListItem> listItem = new List<Captain.Common.Utilities.ListItem>();

            listItem.Add(new Captain.Common.Utilities.ListItem("    ", "0", " ", Color.White));
            DataSet ds1 = Captain.DatabaseLayer.CaseMst.GetCaseWorker(strCwFormat, Hierarchy.Substring(0, 2), Hierarchy.Substring(2, 2), Hierarchy.Substring(4, 2));
            if (ds1.Tables.Count > 0)
            {
                DataTable dt1 = ds1.Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt1.Rows)
                    {
                        if ((Mode == "Add" && dr["PWH_INACTIVE"].ToString().Trim() == "N") || (Mode == "Edit"))
                            listItem.Add(new Captain.Common.Utilities.ListItem(dr["NAME"].ToString().Trim(), dr["PWH_CASEWORKER"].ToString().Trim(), dr["PWH_INACTIVE"].ToString(), (dr["PWH_INACTIVE"].ToString().Equals("Y")) ? Color.Red : Color.Black));
                    }

                    if (CAMS_FLG == "CA")
                        CmbWorker.Items.AddRange(listItem.ToArray());
                    //else
                    //    Cmb_MS_Worker.Items.AddRange(listItem.ToArray());
                }
            }
            switch (CAMS_FLG)
            {
                case "CA":
                    if (Pass_CA_Entity.Rec_Type == "I") // !string.IsNullOrEmpty(MST_Intakeworker))
                    {

                        if (!string.IsNullOrEmpty(BaseForm.UserProfile.CaseWorker))
                            SetComboBoxValue(CmbWorker, BaseForm.UserProfile.CaseWorker);
                        //else
                        //    !string.IsNullOrEmpty(MST_Intakeworker)
                        //        SetComboBoxValue(CmbWorker, MST_Intakeworker);
                    }
                    //SetComboBoxValue(CmbWorker, MST_Intakeworker);
                    else
                        CmbWorker.SelectedIndex = 0;
                    break;
                    //case "MS":
                    //    if (Pass_MS_Entity.Rec_Type == "I" && !string.IsNullOrEmpty(BaseForm.UserProfile.CaseWorker)) // !string.IsNullOrEmpty(MST_Intakeworker))
                    //        SetComboBoxValue(Cmb_MS_Worker, BaseForm.UserProfile.CaseWorker);
                    //    //SetComboBoxValue(Cmb_MS_Worker, MST_Intakeworker);
                    //    else
                    //        Cmb_MS_Worker.SelectedIndex = 0;
                    //    break;
            }
        }

        private void Fill_Funding()
        {
            CmbFunding1.Items.Clear(); CmbFunding1.ColorMember = "FavoriteColor";
            CmbFunding2.Items.Clear(); CmbFunding2.ColorMember = "FavoriteColor";
            CmbFunding3.Items.Clear(); CmbFunding3.ColorMember = "FavoriteColor";


            if (!string.IsNullOrEmpty(SP_Header_Rec.Funds))
            {
                bool Fund_Exists = false; int Pos = 0, Tmp_Loop_Cnt = 0;
                List<SPCommonEntity> FundingList = new List<SPCommonEntity>();
                FundingList = _model.SPAdminData.Get_AgyRecs_WithFilter("Funding", "A");
                string Funds_Str = SP_Header_Rec.Funds;
                int Tmp_Curr_Fund_Len = 0;

                foreach (SPCommonEntity Entity in FundingList)
                {

                    Fund_Exists = false; Pos = 0;
                    for (int i = 0; Pos < SP_Header_Rec.Funds.Length; i++)
                    {

                        Tmp_Curr_Fund_Len = (Funds_Str.Substring(Pos, Funds_Str.Substring(Pos, (Funds_Str.Length - Pos)).Length)).Length;

                        if (Entity.Code == SP_Header_Rec.Funds.Substring(Pos, (Tmp_Curr_Fund_Len >= 10 ? 10 : Tmp_Curr_Fund_Len)).Trim())
                        {
                            Fund_Exists = true; break;
                        }
                        Pos += 10;

                    }

                    if (Fund_Exists)
                    {
                        if (Mode == "Edit" || (Mode == "Add" && Entity.Active.Equals("Y")))
                        {
                            CmbFunding1.Items.Add(new Captain.Common.Utilities.ListItem(Entity.Desc, Entity.Code, Entity.Active, (Entity.Active.Equals("Y") ? Color.Black : Color.Red)));
                            CmbFunding2.Items.Add(new Captain.Common.Utilities.ListItem(Entity.Desc, Entity.Code, Entity.Active, (Entity.Active.Equals("Y") ? Color.Black : Color.Red)));
                            CmbFunding3.Items.Add(new Captain.Common.Utilities.ListItem(Entity.Desc, Entity.Code, Entity.Active, (Entity.Active.Equals("Y") ? Color.Black : Color.Red)));
                            Tmp_Loop_Cnt++;
                        }
                    }
                }

            }
            CmbFunding1.Items.Insert(0, new Captain.Common.Utilities.ListItem("    ", "0", " ", Color.White));
            CmbFunding2.Items.Insert(0, new Captain.Common.Utilities.ListItem("    ", "0", " ", Color.White));
            CmbFunding3.Items.Insert(0, new Captain.Common.Utilities.ListItem("    ", "0", " ", Color.White));
            CmbFunding1.SelectedIndex = CmbFunding2.SelectedIndex = CmbFunding3.SelectedIndex = 0;
        }

        //private void Fill_Results()
        //{
        //    Cmb_MS_Results.Items.Clear(); Cmb_MS_Results.ColorMember = "FavoriteColor";

        //    Cmb_MS_Results.Items.Insert(0, new Captain.Common.Utilities.ListItem("    ", "0", " ", Color.White));
        //    Cmb_MS_Results.SelectedIndex = 0;


        //    if (!string.IsNullOrEmpty(SP_Header_Rec.Status))
        //    {
        //        bool Ststus_Exists = false; int Pos = 0, Tmp_Loop_Cnt = 0, Tmp_Curr_Status_Len = 0;
        //        string Status_Str = SP_Header_Rec.Status;
        //        List<SPCommonEntity> ResultsList = new List<SPCommonEntity>();
        //        ResultsList = _model.SPAdminData.Get_AgyRecs("Results");

        //        foreach (SPCommonEntity Entity in ResultsList)
        //        {
        //            Ststus_Exists = false; Pos = 0;
        //            for (int i = 0; Pos < SP_Header_Rec.Status.Length; i++)
        //            {
        //                Tmp_Curr_Status_Len = (Status_Str.Substring(Pos, Status_Str.Substring(Pos, (Status_Str.Length - Pos)).Length)).Length;

        //                if (Entity.Code == SP_Header_Rec.Status.Substring(Pos, (Tmp_Curr_Status_Len >= 4 ? 4 : Tmp_Curr_Status_Len)).Trim())
        //                {
        //                    Ststus_Exists = true; break;
        //                }
        //                Pos += 4;
        //            }

        //            if (Ststus_Exists)
        //            {
        //                if (Mode == "Edit" || (Mode == "Add" && Entity.Active.Equals("Y")))
        //                {
        //                    Cmb_MS_Results.Items.Add(new Captain.Common.Utilities.ListItem(Entity.Desc, Entity.Code, Entity.Active, (Entity.Active.Equals("Y") ? Color.Green : Color.Red)));
        //                    Tmp_Loop_Cnt++;
        //                }
        //            }
        //        }

        //        //if (Tmp_Loop_Cnt > 0)
        //        //{
        //        //    Cmb_MS_Results.Items.Insert(0, new ListItem("    ", "0", " ", Color.White));
        //        //    Cmb_MS_Results.SelectedIndex = 0;
        //        //}
        //    }
        //}


        private void Fill_UOM()
        {

            Cmb_UOM.Items.Clear(); Cmb_UOM.ColorMember = "FavoriteColor";
            //Cmb_UOM2.Items.Clear(); Cmb_UOM2.ColorMember = "FavoriteColor";
            //Cmb_UOM3.Items.Clear(); Cmb_UOM3.ColorMember = "FavoriteColor";

            List<SPCommonEntity> UOMList = new List<SPCommonEntity>();
            UOMList = _model.SPAdminData.Get_AgyRecs_WithFilter("UOM", "A");

            if (UOMList.Count > 0)
            {
                if (Mode.ToUpper() == "ADD")
                {
                    UOMList = UOMList.FindAll(u => (u.ListHierarchy.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.ListHierarchy.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.ListHierarchy.Contains(BaseForm.BaseAgency + "****") || u.ListHierarchy.Contains("******")) && u.Active.ToString().ToUpper() == "Y").ToList();
                }
                else
                {
                    UOMList = UOMList.FindAll(u => u.ListHierarchy.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.ListHierarchy.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.ListHierarchy.Contains(BaseForm.BaseAgency + "****") || u.ListHierarchy.Contains("******")).ToList();
                }

                UOMList = UOMList.OrderByDescending(u => u.Active).ThenBy(u => u.Desc).ToList();
            }

            foreach (SPCommonEntity Entity in UOMList)
            {
                if (Mode == "Edit" || (Mode == "Add" && Entity.Active.Equals("Y")))
                {
                    Cmb_UOM.Items.Add(new Captain.Common.Utilities.ListItem(Entity.Desc, Entity.Code, Entity.Active, (Entity.Active.Equals("Y") ? Color.Black : Color.Red), Entity.Ext));
                    //Cmb_UOM2.Items.Add(new Captain.Common.Utilities.ListItem(Entity.Desc, Entity.Code, Entity.Active, (Entity.Active.Equals("Y") ? Color.Green : Color.Red), Entity.Ext));
                    //Cmb_UOM3.Items.Add(new Captain.Common.Utilities.ListItem(Entity.Desc, Entity.Code, Entity.Active, (Entity.Active.Equals("Y") ? Color.Green : Color.Red), Entity.Ext));
                }
            }
            Cmb_UOM.Items.Insert(0, new Captain.Common.Utilities.ListItem("    ", "0", " ", Color.White));
            //Cmb_UOM2.Items.Insert(0, new Captain.Common.Utilities.ListItem("    ", "0", " ", Color.White));
            //Cmb_UOM3.Items.Insert(0, new Captain.Common.Utilities.ListItem("    ", "0", " ", Color.White));
            Cmb_UOM.SelectedIndex = 0;

            List<CommonEntity> Gender = CommonFunctions.AgyTabsFilterCode(BaseForm.BaseAgyTabsEntity, Consts.AgyTab.UOMTABLE, BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, Mode); // _model.lookupDataAccess.GetGender();
            //// Gender = filterByHIE(Gender);
            //Cmb_UOM.Items.Insert(0, new Captain.Common.Utilities.ListItem(" ", "0"));
            //Cmb_UOM.ColorMember = "FavoriteColor";
            //Cmb_UOM.SelectedIndex = 0;
            //foreach (CommonEntity gender in Gender)
            //{
            //    Captain.Common.Utilities.ListItem li = new Captain.Common.Utilities.ListItem(gender.Desc, gender.Code, gender.Active, gender.Active.Equals("Y") ? Color.Green : Color.Red);
            //    Cmb_UOM.Items.Add(li);
            //    //if (Mode.Equals(Consts.Common.Add) && gender.Default.Equals("Y")) Cmb_UOM.SelectedItem = li;
            //}

            //Cmb_UOM2.Items.Insert(0, new Captain.Common.Utilities.ListItem(" ", "0"));
            //Cmb_UOM2.ColorMember = "FavoriteColor";
            //Cmb_UOM2.SelectedIndex = 0;
            //foreach (CommonEntity gender in Gender)
            //{
            //    Captain.Common.Utilities.ListItem li = new Captain.Common.Utilities.ListItem(gender.Desc, gender.Code, gender.Active, gender.Active.Equals("Y") ? Color.Green : Color.Red);
            //    Cmb_UOM2.Items.Add(li);
            //    //if (Mode.Equals(Consts.Common.Add) && gender.Default.Equals("Y")) Cmb_UOM.SelectedItem = li;
            //}

            //Cmb_UOM3.Items.Insert(0, new Captain.Common.Utilities.ListItem(" ", "0"));
            //Cmb_UOM3.ColorMember = "FavoriteColor";
            //Cmb_UOM3.SelectedIndex = 0;
            //foreach (CommonEntity gender in Gender)
            //{
            //    Captain.Common.Utilities.ListItem li = new Captain.Common.Utilities.ListItem(gender.Desc, gender.Code, gender.Active, gender.Active.Equals("Y") ? Color.Green : Color.Red);
            //    Cmb_UOM3.Items.Add(li);
            //    //if (Mode.Equals(Consts.Common.Add) && gender.Default.Equals("Y")) Cmb_UOM.SelectedItem = li;
            //}
        }

        List<CAOBOEntity> CAOBO_List = new List<CAOBOEntity>();
        CAOBOEntity Search_CAOBO_Entity = new CAOBOEntity();
        CASEMSOBOEntity Search_MSOBO_Entity = new CASEMSOBOEntity();
        private void Get_CAOBO_Data()
        {

            Search_CAOBO_Entity.ID = Pass_CA_Entity.ACT_ID;
            if (CA_Template_List.Count > 0 && Pass_CA_Entity.Rec_Type == "I")
                Search_CAOBO_Entity.ID = CA_Template_List[0].ACT_ID;

            Search_CAOBO_Entity.Seq = Search_CAOBO_Entity.CLID = Search_CAOBO_Entity.Fam_Seq = null;

            CAOBO_List = _model.SPAdminData.Browse_CAOBO(Search_CAOBO_Entity, "Browse");
        }

        private void Fill_CA_Members_DropDown()
        {
            CA_Members_Grid.Rows.Clear();
            DataSet ds = Captain.DatabaseLayer.MainMenu.MainMenuSearch("APP", "All", null, null, Pass_CA_Entity.App_no, null, null, null, null, null, null, null, null, null, null, Hierarchy + Year, null, BaseForm.UserID, string.Empty, string.Empty, string.Empty);
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    List<CommonEntity> Relation;
                    Relation = _model.lookupDataAccess.GetRelationship();

                    int rowIndex = 0;
                    string Name = null, TmpSsn = null, Relation_Desc = null; string dob = string.Empty;
                    foreach (DataRow dr in dt.Rows)
                    {
                        Name = TmpSsn = Relation_Desc = dob = null;

                        Name = dr["Fname"].ToString().Trim() + " " + dr["MName"].ToString() + " " + dr["Lname"].ToString().Trim();
                        TmpSsn = dr["Ssn"].ToString();
                        if (!string.IsNullOrEmpty(TmpSsn))
                            TmpSsn = TmpSsn.Substring(0, 3) + '-' + TmpSsn.Substring(3, 2) + '-' + TmpSsn.Substring(5, 4);

                        dob = dr["DOB"].ToString();
                        if (!string.IsNullOrEmpty(dob))
                            dob = LookupDataAccess.Getdate(dob).ToString();

                        CommonEntity Relationship = Relation.Find(x => x.Code == dr["Mem_Code"].ToString().Trim());
                        if (Relationship != null)
                            Relation_Desc = Relationship.Desc;


                        //foreach (CommonEntity Relationship in Relation)
                        //{
                        //    if (Relationship.Code.Equals(dr["Mem_Code"].ToString().Trim()))
                        //    {
                        //        Relation_Desc = Relationship.Desc; break;
                        //    }
                        //}



                        rowIndex = CA_Members_Grid.Rows.Add(false, Name, dob, Relation_Desc, dr["RecFamSeq"].ToString(), dr["ClientID"].ToString(), dr["AppNo"].ToString().Substring(10, 1), "N", dr["AppStatus"].ToString(), dr["SNP_EXCLUDE"].ToString());

                        if (dr["AppStatus"].ToString() != "A")
                            CA_Members_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;

                        if (dr["SNP_EXCLUDE"].ToString() != "N")
                            CA_Members_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;


                        if (dr["AppNo"].ToString().Substring(10, 1) == "A")
                        {
                            if (dr["AppStatus"].ToString() != "A")
                                CA_Members_Grid.Rows[rowIndex].Cells["CA_Mem_Name"].Style.ForeColor = Color.Blue;
                            else
                                CA_Members_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue;
                        }
                        //Members_Grid.Rows[rowIndex].Tag = dr;


                    }
                }
            }
            if (Pass_CA_Entity.Rec_Type != "I")
                Get_CAOBO_Data();

            if (CA_Members_Grid.Rows.Count > 0)
                CA_Members_Grid.Rows[0].Selected = true;
        }

        List<CASEMSEntity> CASEMSList = new List<CASEMSEntity>();
        private void GetData(string sp_Code, string Branch_Code)
        {
            SP_CAMS_Details = _model.SPAdminData.Browse_CASESP2(sp_Code, Branch_Code, null, null, "CASE4006");

            List<CASESP2Entity> SelMSList = SP_CAMS_Details.FindAll(u => u.Type1.Equals("MS"));

            if (SelMSList.Count > 0)
            {
                foreach (CASESP2Entity Entity in SelMSList)
                {
                    CASEMSEntity Add_Entity = new CASEMSEntity();

                    Add_Entity.Agency = BaseForm.BaseAgency; Add_Entity.Dept = BaseForm.BaseDept; Add_Entity.Program = BaseForm.BaseProg; Add_Entity.Year = BaseForm.BaseYear;
                    Add_Entity.App_no = BaseForm.BaseApplicationNo; Add_Entity.Service_plan = Entity.ServPlan; Add_Entity.MS_Code = Entity.CamCd; Add_Entity.Branch = Entity.Branch; Add_Entity.Group = Entity.Orig_Grp.ToString();
                    Add_Entity.Curr_Grp = Entity.Curr_Grp.ToString(); Add_Entity.MSDesc = Entity.CAMS_Desc.Trim(); Add_Entity.Date = Act_Date.Text.ToString(); Add_Entity.SPM_Seq = Pass_CA_Entity.SPM_Seq;

                    CASEMSList.Add(new CASEMSEntity(Add_Entity));

                }
            }


        }

        private void Fill_CA_Benefiting_From()
        {
            this.cmb_CA_Benefit.SelectedIndexChanged -= new System.EventHandler(this.cmb_CA_Benefit_SelectedIndexChanged);

            cmb_CA_Benefit.Items.Clear();
            cmb_CA_Benefit.Items.Add(new Captain.Common.Utilities.ListItem("Applicant", "1"));
            cmb_CA_Benefit.Items.Add(new Captain.Common.Utilities.ListItem("All Household Members", "2"));
            cmb_CA_Benefit.Items.Add(new Captain.Common.Utilities.ListItem("Selected Household Members", "3"));


            this.cmb_CA_Benefit.SelectedIndexChanged += new System.EventHandler(this.cmb_CA_Benefit_SelectedIndexChanged);
            if (Pass_CA_Entity.Rec_Type == "I")
            {
                if (SP_CAMS_Details.Count > 0)
                {
                    CASESP2Entity SelCAlist = SP_CAMS_Details.Find(u => u.CamCd.Trim() == Pass_CA_Entity.ACT_Code.Trim() && u.Type1 == "CA");
                    if (SelCAlist != null)
                    {
                        if (!string.IsNullOrEmpty(SelCAlist.SP2_OBF))
                            CommonFunctions.SetComboBoxValue(cmb_CA_Benefit, SelCAlist.SP2_OBF);
                        else
                        {
                            if (!string.IsNullOrEmpty(ACR_BenefitFrom.Trim()))
                                cmb_CA_Benefit.SelectedIndex = (int.Parse(ACR_BenefitFrom.Trim()) - 1);
                            else
                                cmb_CA_Benefit.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ACR_BenefitFrom.Trim()))
                            cmb_CA_Benefit.SelectedIndex = (int.Parse(ACR_BenefitFrom.Trim()) - 1);
                        else
                            cmb_CA_Benefit.SelectedIndex = 0;
                    }
                }


                //if (!string.IsNullOrEmpty(ACR_BenefitFrom.Trim()))
                //    cmb_CA_Benefit.SelectedIndex = (int.Parse(ACR_BenefitFrom.Trim()) - 1);
                //else
                //    cmb_CA_Benefit.SelectedIndex = 0;
            }
            //if (Pass_MS_Entity.Rec_Type == "I")


            cmb_Benefit.Items.Clear();
            cmb_Benefit.Items.Add(new Captain.Common.Utilities.ListItem("Applicant", "1"));
            cmb_Benefit.Items.Add(new Captain.Common.Utilities.ListItem("All Household Members", "2"));
            cmb_Benefit.Items.Add(new Captain.Common.Utilities.ListItem("Selected Household Members", "3"));


            this.cmb_Benefit.SelectedIndexChanged += new System.EventHandler(this.cmb_Benefit_SelectedIndexChanged);
            if (Pass_CA_Entity.Rec_Type == "I")
                cmb_Benefit.SelectedIndex = 0;

        }

        private void GetRequiredCutomQuestions()
        {
            SaldefEntity Search_saldef_Entity = new SaldefEntity(true);
            SALQUESEntity = new List<SalquesEntity>();

            List<SaldefEntity> SALDEF = _model.SALDEFData.Browse_SALDEF(Search_saldef_Entity, "Browse", BaseForm.UserID, BaseForm.BaseAgency);
            if (SALDEF.Count > 0)
            {
                SALDEF = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
            }
            if (SALDEF.Count > 0)
            {
                SalquesEntity Search_Salques_Entity = new SalquesEntity(true);
                Search_Salques_Entity.SALQ_SALD_ID = SALDEF[0].SALD_ID;
                SALQUESEntity = _model.SALDEFData.Browse_SALQUES(Search_Salques_Entity, "Browse");
            }
        }

        List<CASESP2Entity> SP_CAMS_Details = new List<CASESP2Entity>();
        List<CASEACTEntity> SP_Activity_Details = new List<CASEACTEntity>();
        List<CASEMSEntity> SP_MS_Details = new List<CASEMSEntity>();
        private void Fill_SP_CAMS_Details(string sp_Code, string Branch_Code, string Sel_CAMS_Key)
        {
            this.SP_CA_Grid.SelectionChanged -= new System.EventHandler(this.SP_CA_Grid_SelectionChanged);
            if (Branch_Code != "9")
            {

                //SP_CAMS_Details = _model.SPAdminData.Browse_CASESP2(sp_Code, Branch_Code, null, null, "CASE4006");
                SP_CAMS_Details = SP_CAMS_Details.OrderBy(u => u.Row).ToList();
                if (SP_CAMS_Details.Count > 0)
                {
                    GetRequiredCutomQuestions();
                    int rowIndex = 0, Sel_CAMS_Index = 0; int MSrowIndex = 0;
                    string CAMS_DESC = null;
                    foreach (CASESP2Entity Entity in SP_CAMS_Details)
                    {
                        if (Entity.Type1 == "CA" && CAMS_FLG == "CA")
                        {
                            bool IS_Que_Req = false;
                            if (SALQUESEntity.Count > 0)
                            {
                                SalquesEntity SelReq = SALQUESEntity.Find(u => u.SALQ_REQ == "Y");
                                if (SelReq != null)
                                    IS_Que_Req = true;
                            }

                            //if (Pass_CA_Entity.ACT_Code.Trim() != Entity.CamCd.Trim())
                            //{
                            if (Entity.SP2_CAMS_Active == "A" && Entity.CAMS_Active.Equals("True"))
                            {
                                if (Pass_CA_Entity.ACT_Code.Trim() == Entity.CamCd.Trim())
                                {
                                    rowIndex = SP_CA_Grid.Rows.Add(true, Entity.Row, Entity.CamCd.Trim(), Entity.CAMS_Desc.Trim(), Entity.Type1, "C", Entity.Branch, Entity.Orig_Grp, "N", null, null, Entity.CAMS_Desc, Pass_CA_Entity.Year, Entity.CAMS_Active, null, Entity.Curr_Grp, Entity.Branch, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, (IS_Que_Req ? "*" : ""), Entity.SP2_OBF);
                                    SP_CA_Grid.Rows[rowIndex].Cells["gvbtnCABenefit"].Value = Img_btn;
                                    if(CustomQuestions=="Y") SP_CA_Grid.Rows[rowIndex].Cells["gvCustQues"].Value = Img_btn_Plain; 
                                }

                                else
                                    rowIndex = SP_CA_Grid.Rows.Add(false, Entity.Row, Entity.CamCd.Trim(), Entity.CAMS_Desc.Trim(), Entity.Type1, "C", Entity.Branch, Entity.Orig_Grp, "N", null, null, Entity.CAMS_Desc, Pass_CA_Entity.Year, Entity.CAMS_Active, null, Entity.Curr_Grp, Entity.Branch, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, (IS_Que_Req ? "*" : ""), Entity.SP2_OBF);
                            }
                            //}
                            if (Pass_CA_Entity.ACT_Code.Trim() == Entity.CamCd.Trim())
                                SP_CA_Grid.Rows[rowIndex].ReadOnly = true;

                        }
                        //if (Entity.Type1 == "MS" && CAMS_FLG == "CA")
                        //{
                        //    //if (Pass_MS_Entity.MS_Code.Trim() != Entity.CamCd.Trim())
                        //        MSrowIndex = gvMSgrid.Rows.Add(false, Entity.CamCd.Trim(), Entity.CAMS_Desc,string.Empty, Entity.Row, Entity.Type1, "C", Entity.Branch, Entity.Orig_Grp, "N", null, null, Entity.CAMS_Desc, Pass_CA_Entity.Year, Entity.CAMS_Active, null, Entity.Curr_Grp, Entity.Branch, null);
                        //}
                    }
                }
            }
            else
            {
                Fill_Additional_CAMS_Details(sp_Code);

                if (ADD_CAMA_Details.Count > 0)
                {
                    int rowIndex = 0, Sel_CAMS_Index = 0; int MSrowIndex = 0;
                    string CAMS_DESC = null;
                    foreach (CASESPM2Entity Entity in ADD_CAMA_Details)
                    {
                        if (Entity.Type1 == "CA" && CAMS_FLG == "CA")
                        {
                            if (Pass_CA_Entity.ACT_Code.Trim() != Entity.CamCd.Trim())
                                rowIndex = SP_CA_Grid.Rows.Add(false, Entity.CamCd.Trim(), Entity.CAMS_Desc.Trim(), Entity.Type1, "C", Entity.Branch, Entity.Group, "N", null, null, Entity.CAMS_Desc, Pass_CA_Entity.Year, Entity.CAMS_Active, null, Entity.Curr_Group, Entity.Branch, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                        }
                        //if (Entity.Type1 == "MS" )
                        //{
                        //    //if (Pass_MS_Entity.MS_Code.Trim() != Entity.CamCd.Trim())
                        //        MSrowIndex = gvMSgrid.Rows.Add(false, Entity.CamCd.Trim(), Entity.CAMS_Desc, string.Empty, string.Empty, Entity.Type1, "C", Entity.Branch, Entity.Group, "N", null, null, Entity.CAMS_Desc, Pass_CA_Entity.Year, Entity.CAMS_Active, null, Entity.Curr_Group, Entity.Branch, null);
                        //}
                    }
                }

            }
            this.SP_CA_Grid.SelectionChanged += new System.EventHandler(this.SP_CA_Grid_SelectionChanged);

            if (SP_CA_Grid.Rows.Count > 0)
            {
                SP_CA_Grid.Rows[0].Tag = 0;

                SP_CA_Grid.CurrentCell = SP_CA_Grid.Rows[0].Cells[2];
                SP_CA_Grid_SelectionChanged(SP_CA_Grid, EventArgs.Empty);
            }

        }

        List<CASESPM2Entity> ADD_CAMA_Details = new List<CASESPM2Entity>();
        private void Fill_Additional_CAMS_Details(string Code)
        {
            CASESPM2Entity Search_Entity2 = new CASESPM2Entity();

            Search_Entity2.Agency = Hierarchy.Substring(0, 2);
            Search_Entity2.Dept = Hierarchy.Substring(2, 2);
            Search_Entity2.Prog = Hierarchy.Substring(4, 2);
            //Search_Entity2.Year = BaseYear;
            Search_Entity2.Year = Year;                         // Year will be always Four-Spaces in CASESPM2
            Search_Entity2.App = BaseForm.BaseApplicationNo;
            Search_Entity2.Spm_Seq = Pass_CA_Entity.SPM_Seq;

            Search_Entity2.ServPlan = Search_Entity2.Branch = Search_Entity2.Group = null;
            Search_Entity2.Type1 = Search_Entity2.CamCd = Search_Entity2.Curr_Group = null;
            Search_Entity2.SelOrdinal = Search_Entity2.DateLstc = Search_Entity2.lstcOperator = null;
            Search_Entity2.Dateadd = Search_Entity2.addoperator = null;

            Search_Entity2.ServPlan = Code;


            ADD_CAMA_Details = _model.SPAdminData.Browse_CASESPM2(Search_Entity2, "Browse");

            //if (ADD_CAMA_Details.Count > 0)
            //    Fill_Additional_CAMS_Details_Grid();
        }

        private void Fill_CA_Controls()
        {
            if (!string.IsNullOrEmpty(Pass_CA_Entity.ACT_Date))
            {
                Act_Date.Checked = true; Act_Date.Value = Convert.ToDateTime(Pass_CA_Entity.ACT_Date);
            }

            if (!string.IsNullOrEmpty(Pass_CA_Entity.ActSeek_Date))
            {
                dtActSeek_Date.Checked = true; dtActSeek_Date.Value = Convert.ToDateTime(Pass_CA_Entity.ActSeek_Date);
            }

            SetComboBoxValue(CmbWorker, Pass_CA_Entity.Caseworker);
            SetComboBoxValue(CmbSite, Pass_CA_Entity.Site);

            SetComboBoxValue(CmbFunding1, Pass_CA_Entity.Fund1);
            SetComboBoxValue(CmbFunding2, Pass_CA_Entity.Fund2);
            SetComboBoxValue(CmbFunding3, Pass_CA_Entity.Fund3);
            SetComboBoxValue(Cmb_UOM, Pass_CA_Entity.UOM);



            if (!string.IsNullOrEmpty(Pass_CA_Entity.Act_PROG.Trim()) && !Pass_CA_Entity.Act_PROG.Contains("**"))
                Txt_CA_Program.Text = Set_SP_Program_Text(Pass_CA_Entity.Act_PROG.Trim());



            Txt_VendNo.Text = Pass_CA_Entity.Vendor_No;
            Text_VendName.Text = Get_Vendor_Name();

            Txtx_ChkNo.Text = Pass_CA_Entity.Check_No;
            if (Pass_CA_Entity.Units != null)
                Txt_Units.Text = Pass_CA_Entity.Units.Trim();

            if (!string.IsNullOrEmpty(Pass_CA_Entity.Check_Date))
            {
                Check_Date.Checked = true;
                Check_Date.Value = Convert.ToDateTime(Pass_CA_Entity.Check_Date);
            }

            Txt_Cost.Text = Pass_CA_Entity.Cost;
            txtRate.Text = Pass_CA_Entity.Rate;

            //Txt_Cost2.Text = Pass_CA_Entity.Amount2;
            //Txt_Cost3.Text = Pass_CA_Entity.Amount3;

            //Txt_Units2.Text = Pass_CA_Entity.Units2;
            //Txt_Units3.Text = Pass_CA_Entity.Units3;

            //SetComboBoxValue(Cmb_UOM2, Pass_CA_Entity.UOM2);
            //SetComboBoxValue(Cmb_UOM3, Pass_CA_Entity.UOM3);

            //Txt_RefTo.Text = Pass_CA_Entity.Refer_Data;
            if (!string.IsNullOrEmpty(Pass_CA_Entity.Followup_On))
            {
                UpOn_Date.Checked = true;
                UpOn_Date.Value = Convert.ToDateTime(Pass_CA_Entity.Followup_On);
            }

            if (!string.IsNullOrEmpty(Pass_CA_Entity.Followup_Comp))
            {
                Complete_Date.Checked = true;
                Complete_Date.Value = Convert.ToDateTime(Pass_CA_Entity.Followup_Comp);
            }

            Txt_TobeFollowUp.Text = Pass_CA_Entity.Followup_By;

            this.cmb_Benefit.SelectedIndexChanged -= new System.EventHandler(this.cmb_CA_Benefit_SelectedIndexChanged);
            SetComboBoxValue(cmb_Benefit, Pass_CA_Entity.CA_OBF);

            //if (CAMS_Desc == "Auto Post MS")
            //    Set_CAMembers_Grid_Auto_Post();
            //else
            Set_Members_CA_Grid_As_Benefit_Change(false, Pass_CA_Entity.CA_OBF);
            ////  Set_Members_Grid_From_MSOBO();

            this.cmb_CA_Benefit.SelectedIndexChanged += new System.EventHandler(this.cmb_CA_Benefit_SelectedIndexChanged);

            //CASEREFEntity Search_REF_Entity = new CASEREFEntity(true);
            //Search_REFS_Entity.Service = Pass_CA_Entity.ACT_Code.Trim();
            //Search_REFS_Entity.Service = Pass_CA_Entity.ACT_Code.Trim();
        }

        private void Fill_CA_Controls_4rm_Template()
        {
            if (!string.IsNullOrEmpty(CA_Template_List[0].ACT_Date))
            {
                Act_Date.Checked = true; Act_Date.Value = Convert.ToDateTime(CA_Template_List[0].ACT_Date);
            }
            else
            {

            }
            //if (!string.IsNullOrEmpty(CA_Template_List[0].ActSeek_Date))
            //{
            //    dtActSeek_Date.Checked = true; dtActSeek_Date.Value = Convert.ToDateTime(CA_Template_List[0].ActSeek_Date);
            //}
            SetComboBoxValue(CmbWorker, CA_Template_List[0].Caseworker);
            SetComboBoxValue(CmbSite, CA_Template_List[0].Site);

            SetComboBoxValue(CmbFunding1, CA_Template_List[0].Fund1);
            SetComboBoxValue(CmbFunding2, CA_Template_List[0].Fund2);
            SetComboBoxValue(CmbFunding3, CA_Template_List[0].Fund3);


            if (!string.IsNullOrEmpty(CA_Template_List[0].Act_PROG.Trim()) && !CA_Template_List[0].Act_PROG.Contains("**"))
                Txt_CA_Program.Text = Set_SP_Program_Text(CA_Template_List[0].Act_PROG.Trim());

            Txt_VendNo.Text = CA_Template_List[0].Vendor_No;
            Text_VendName.Text = Get_Vendor_Name();

            Txtx_ChkNo.Text = CA_Template_List[0].Check_No;

            if (!string.IsNullOrEmpty(CA_Template_List[0].Check_Date))
            {
                Check_Date.Checked = true;
                Check_Date.Value = Convert.ToDateTime(CA_Template_List[0].Check_Date);
            }

            Txt_Cost.Text = CA_Template_List[0].Cost;
            txtRate.Text = CA_Template_List[0].Rate;

            //Txt_RefTo.Text = CA_Template_List[0].Refer_Data;
            if (!string.IsNullOrEmpty(CA_Template_List[0].Followup_On))
            {
                UpOn_Date.Checked = true;
                UpOn_Date.Value = Convert.ToDateTime(CA_Template_List[0].Followup_On);
            }

            if (!string.IsNullOrEmpty(CA_Template_List[0].Followup_Comp))
            {
                Complete_Date.Checked = true;
                Complete_Date.Value = Convert.ToDateTime(CA_Template_List[0].Followup_Comp);
            }

            Txt_TobeFollowUp.Text = CA_Template_List[0].Followup_By;
        }

        private string Set_SP_Program_Text(string Prog_Code)
        {
            string Tmp_Hierarchy = "";
            Sel_CAMS_Program = "";

            foreach (HierarchyEntity Ent in SP_Programs_List)
            {
                Tmp_Hierarchy = Ent.Agency.Trim() + Ent.Dept.Trim() + Ent.Prog.Trim();
                if (Prog_Code == Tmp_Hierarchy)
                {
                    Sel_CAMS_Program = Tmp_Hierarchy + " - " + Ent.HirarchyName.Trim();
                    break;
                }
            }

            return Sel_CAMS_Program;
        }

        private void SetComboBoxValue(ComboBox comboBox, string value)
        {
            bool Combo_Set = false;
            if (string.IsNullOrEmpty(value) || value == " ")
                value = "0";
            if (comboBox != null && comboBox.Items.Count > 0)
            {
                foreach (Captain.Common.Utilities.ListItem li in comboBox.Items)
                {
                    if (li.Value.Equals(value) || li.Text.Equals(value))
                    {
                        comboBox.SelectedItem = li;
                        Combo_Set = true;
                        break;
                    }
                }
            }

            if (!Combo_Set)
                comboBox.SelectedIndex = 0;
        }

        private string Get_Vendor_Name()
        {
            string Vend_Name = string.Empty;
            foreach (CASEVDDEntity Entity in CaseVddlist)
            {
                if (Entity.Code == Pass_CA_Entity.Vendor_No)
                {
                    Vend_Name = Entity.Name.Trim(); break;
                }
            }

            return Vend_Name;
        }


        private void pbPdf_Click(object sender, EventArgs e)
        {

        }

        private void Pb_Notes_Click(object sender, EventArgs e)
        {
            string Notes_Field_Name = null;

            //if (sender == Pb_CA_Notes)
            //    Notes_Field_Name = Hierarchy + Pass_CA_Entity.Year + Pass_CA_Entity.App_no + Pass_CA_Entity.Service_plan + "CA" + Pass_CA_Entity.Branch + Pass_CA_Entity.Group + Pass_CA_Entity.ACT_Code.Trim() + Pass_CA_Entity.ACT_Seq;
            //else
            //    Notes_Field_Name = Hierarchy + Pass_MS_Entity.Year + Pass_MS_Entity.App_no+ Pass_MS_Entity.Service_plan + "MS" + Pass_MS_Entity.Branch + Pass_MS_Entity.Group + Pass_MS_Entity.MS_Code.Trim();

            //if (sender == Pb_CA_Notes)
            //    Notes_Field_Name = Hierarchy + "    " + Pass_CA_Entity.App_no + Pass_CA_Entity.Service_plan + Pass_CA_Entity.Branch +"CA"+ Pass_CA_Entity.Group + Pass_CA_Entity.ACT_Code.Trim() + Pass_CA_Entity.ACT_Seq;
            //else
            //    Notes_Field_Name = Hierarchy + "    " + Pass_MS_Entity.App_no + Pass_MS_Entity.Service_plan + "MS" + Pass_MS_Entity.Branch + Pass_MS_Entity.Group + Pass_MS_Entity.MS_Code.Trim();

            //if (CAMS_FLG == "CA")
            //    Notes_Field_Name = Hierarchy + Pass_CA_Entity.Year + Pass_CA_Entity.App_no + ("000000".Substring(0, (6 - Pass_CA_Entity.Service_plan.Length)) + Pass_CA_Entity.Service_plan) + Pass_CA_Entity.SPM_Seq + "CA" + Pass_CA_Entity.Branch +
            //            ("000000".Substring(0, (6 - Pass_CA_Entity.Group.ToString().Length)) + Pass_CA_Entity.Group.ToString()) + Pass_CA_Entity.ACT_Code.Trim() + Pass_CA_Entity.ACT_Seq;
            //else
            //    Notes_Field_Name = Hierarchy + Pass_MS_Entity.Year + Pass_MS_Entity.App_no + ("000000".Substring(0, (6 - Pass_MS_Entity.Service_plan.Length)) + Pass_MS_Entity.Service_plan) + Pass_MS_Entity.SPM_Seq + "MS" + Pass_MS_Entity.Branch +
            //            ("000000".Substring(0, (6 - Pass_MS_Entity.Group.ToString().Length)) + Pass_MS_Entity.Group.ToString()) + Pass_MS_Entity.MS_Code.Trim();

            if (CAMS_FLG == "CA")
                Notes_Field_Name = Hierarchy + Pass_CA_Entity.Year + Pass_CA_Entity.App_no + Pass_CA_Entity.Service_plan.Trim() + Pass_CA_Entity.SPM_Seq + Pass_CA_Entity.Branch.Trim() +
                        Pass_CA_Entity.Group.ToString() + "CA" + Pass_CA_Entity.ACT_Code.Trim() + Pass_CA_Entity.ACT_Seq + Pass_CA_Entity.ACT_ID;
            else
                Notes_Field_Name = Hierarchy + Pass_MS_Entity.Year + Pass_MS_Entity.App_no + Pass_MS_Entity.Service_plan.Trim() + Pass_MS_Entity.SPM_Seq + Pass_MS_Entity.Branch.Trim() +
                        Pass_MS_Entity.Group.ToString() + "MS" + Pass_MS_Entity.MS_Code.Trim() + CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Act_Date.Value.ToShortDateString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

            List<string> list = new List<string>();
            List<CommonEntity> SelEntity = new List<CommonEntity>();
            if (CAMS_FLG == "CA")
            {
                //SelEntity.Add(new CommonEntity(Pass_CA_Entity.ACT_Code.Trim(), this.Text.Trim(), Notes_Field_Name));
                if (SP_CA_Grid.Rows.Count > 0)
                {
                    foreach (DataGridViewRow dr in SP_CA_Grid.Rows)
                    {
                        if (dr.Cells["Sel_1"].Value.ToString() == true.ToString())
                        {
                            if (Pass_CA_Entity.ACT_Code.Trim() == dr.Cells["SP2_CAMS_Code"].Value.ToString().Trim())
                            { dr.Cells["SP2_CA_Seq"].Value = Pass_CA_Entity.ACT_Seq; dr.Cells["SP2_CAMS_ID"].Value = Pass_CA_Entity.ACT_ID; }

                            string Notes = string.Empty;
                            Notes = Hierarchy + Pass_CA_Entity.Year + Pass_CA_Entity.App_no + Pass_CA_Entity.Service_plan.Trim() + Pass_CA_Entity.SPM_Seq + dr.Cells["SP2_Branch"].Value.ToString() +
                                            dr.Cells["SP2_Group"].Value.ToString() + "CA" + dr.Cells["SP2_CAMS_Code"].Value.ToString().Trim() + dr.Cells["SP2_CA_Seq"].Value.ToString() + dr.Cells["SP2_CAMS_ID"].Value.ToString();

                            //dr.Cells["SP2_CAMS_ID"].Value = New_CAID.ToString();
                            //dr.Cells["SP2_CA_Seq"].Value = New_CA_Seq.ToString();

                            list.Add(Notes);

                            SelEntity.Add(new CommonEntity(dr.Cells["SP2_CAMS_Code"].Value.ToString().Trim(), dr.Cells["SP2_Desc"].Value.ToString().Trim(), Notes, "CA"));

                        }
                    }
                }

                if (CASEMSList.Count > 0)
                {
                    foreach (CASEMSEntity MSEntity in CASEMSList)
                    {
                        if (!string.IsNullOrEmpty(MSEntity.MS_TrigCode.Trim()) && MSEntity.Rec_Type == "I")
                        {
                            string Notes = string.Empty;
                            Notes = Hierarchy + MSEntity.Year + MSEntity.App_no + MSEntity.Service_plan.Trim() + MSEntity.SPM_Seq + MSEntity.Branch +
                                    MSEntity.Group + "MS" + MSEntity.MS_Code.Trim() + CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Act_Date.Value.ToShortDateString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                            //dr.Cells["SP2_CAMS_ID"].Value = New_CAID.ToString();
                            //dr.Cells["SP2_CA_Seq"].Value = New_CA_Seq.ToString();

                            list.Add(Notes);

                            SelEntity.Add(new CommonEntity(MSEntity.MS_Code.Trim(), MSEntity.MSDesc.Trim(), Notes, "MS"));
                        }
                    }
                }
            }



            //if (CAMS_FLG == "CA")
            //    Notes_Field_Name = Hierarchy + "    " + Pass_CA_Entity.App_no + ("000000".Substring(0, (6 - Pass_CA_Entity.Service_plan.Length)) + Pass_CA_Entity.Service_plan) + "CA" + Pass_CA_Entity.Branch +
            //            ("000000".Substring(0, (6 - Pass_CA_Entity.Group.ToString().Length)) + Pass_CA_Entity.Group.ToString()) + Pass_CA_Entity.ACT_Code.Trim() + Pass_CA_Entity.ACT_Seq;
            //else
            //    Notes_Field_Name = Hierarchy + "    " + Pass_MS_Entity.App_no + ("000000".Substring(0, (6 - Pass_MS_Entity.Service_plan.Length)) + Pass_MS_Entity.Service_plan) + "MS" + Pass_MS_Entity.Branch +
            //            ("000000".Substring(0, (6 - Pass_MS_Entity.Group.ToString().Length)) + Pass_MS_Entity.Group.ToString()) + Pass_MS_Entity.MS_Code.Trim();


            if (SP_CA_Grid.Rows.Count > 0 && SelEntity.Count > 1)
            {
                ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, Mode, Privileges, Notes_Field_Name, list, "QuickPost", SelEntity);
                Prog_Form.FormClosed += new FormClosedEventHandler(On_PROGNOTES_Closed);
                Prog_Form.StartPosition = FormStartPosition.CenterScreen;
                Prog_Form.ShowDialog();
            }
            else
            {
                ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, Mode, Privileges, Notes_Field_Name);
                Prog_Form.FormClosed += new FormClosedEventHandler(On_PROGNOTES_Closed);
                Prog_Form.StartPosition = FormStartPosition.CenterScreen;
                Prog_Form.ShowDialog();
            }

            ////ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, "Add", Privileges, Notes_Field_Name);
            //ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, Mode, Privileges, Notes_Field_Name);
            //Prog_Form.FormClosed += new Form.FormClosedEventHandler(On_PROGNOTES_Closed);
            //Prog_Form.ShowDialog();
        }

        private void On_PROGNOTES_Closed(object sender, FormClosedEventArgs e)
        {
            string SelRef_Name = null;

            ProgressNotes_Form form = sender as ProgressNotes_Form;

            switch (CAMS_FLG)
            {
                case "CA":
                    if (Pass_CA_Entity.Rec_Type == "I")                   // in Add mode if user Clicks Cancel in P.Notes Sub form 
                        this.DialogResult = DialogResult.OK;            //             in that case we need to set the result    Yeswanth
                    break;

                case "MS":
                    if (Pass_MS_Entity.Rec_Type == "I")
                        this.DialogResult = DialogResult.OK;
                    break;
            }

            if (form.DialogResult == DialogResult.OK)
            {
                Get_PROG_Notes_Status();

                this.DialogResult = DialogResult.OK;

                switch (CAMS_FLG)
                {
                    case "CA":
                        if (Pass_CA_Entity.Rec_Type == "I")
                        {
                            AlertBox.Show("Posted Successfully");
                            this.Close();
                        }
                        break;

                    case "MS":
                        if (Pass_MS_Entity.Rec_Type == "I")
                        {
                            AlertBox.Show("Posted Successfully");
                            this.Close();
                        }
                        break;
                }
            }
        }

        private void Hepl_Click(object sender, EventArgs e)
        {

        }

        private void PbVendor_Click(object sender, EventArgs e)
        {
            VendBrowseForm Vendor_Browse = new VendBrowseForm(BaseForm, Privileges, "**");
            Vendor_Browse.FormClosed += new FormClosedEventHandler(On_Vendor_Browse_Closed);
            Vendor_Browse.StartPosition = FormStartPosition.CenterScreen;
            Vendor_Browse.ShowDialog();
        }

        private void On_Vendor_Browse_Closed(object sender, FormClosedEventArgs e)
        {
            VendBrowseForm form = sender as VendBrowseForm;
            if (form.DialogResult == DialogResult.OK)
            {
                string[] Vendor_Details = new string[2];
                Vendor_Details = form.Get_Selected_Vendor();

                Txt_VendNo.Text = Vendor_Details[0].Trim();
                Text_VendName.Text = Vendor_Details[1].Trim();
            }
        }

        private void Btn_CACancel_Click(object sender, EventArgs e)
        {
            if (CA_Template_List.Count > 0 && Mode == "Add")
            {
                CASEACTEntity SelCAEntiy = CA_Template_List.Find(u => u.IsSave != "Y");
                if (SelCAEntiy != null)
                {
                    MessageBox.Show("You haven’t posted all the selected services. Are you sure you want to exit?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: QPCancelClick);
                }
            }
            else
                this.Close();
        }

        private void QPCancelClick(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                if (Pass_CA_Entity.Rec_Type == "I")
                {
                    if (BaseForm.BaseAgencyControlDetails.ProgressNotesSwitch.ToUpper() == "Y")
                        MessageBox.Show("Posted Successfully \n Do you want to add Progress Notes?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Add_PROGNotes_For_CAMS);
                    else
                    {
                        AlertBox.Show("Posted Successfully");

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    AlertBox.Show("Posting Updated Successfully");//("Activity Posting Updated Successfully");


                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void Pb_Prog_Click(object sender, EventArgs e)
        {
            string ProgCA = string.Empty;
            if (!string.IsNullOrEmpty(Txt_CA_Program.Text.Trim())) ProgCA = Txt_CA_Program.Text.Substring(0, 6);
            string Sel_Prog = (CAMS_FLG == "CA" ? (ProgCA) : ""), Sel_SerPlan = (CAMS_FLG == "CA" ? Pass_CA_Entity.Service_plan : Pass_MS_Entity.Service_plan);
            //COMMENTED BY SUDHEER ON 09/20/2021 as per LCOPP document
            //HierarchieSelectionFormNew hierarchieSelectionForm = new HierarchieSelectionFormNew(BaseForm, Sel_Prog, Sel_SerPlan, ACR_SERV_Hies);
            HierarchieSelectionFormNew hierarchieSelectionForm = new HierarchieSelectionFormNew(BaseForm, Sel_Prog, Sel_SerPlan, string.Empty);
            hierarchieSelectionForm.FormClosed += new FormClosedEventHandler(OnHierarchieFormClosed);
            hierarchieSelectionForm.StartPosition = FormStartPosition.CenterScreen;
            hierarchieSelectionForm.ShowDialog();
        }

        private void OnHierarchieFormClosed(object sender, FormClosedEventArgs e)
        {
            // HierarchieSelectionForm form = sender as HierarchieSelectionForm;
            HierarchieSelectionFormNew form = sender as HierarchieSelectionFormNew;

            if (form.DialogResult == DialogResult.OK)
            {
                Sel_CAMS_Program = form.Selected_SerPlan_Prog();

                if (CAMS_FLG == "CA")
                {
                    Txt_CA_Program.Text = Sel_CAMS_Program;
                    //else
                    //    Txt_MS_Program.Text = Sel_CAMS_Program;


                    //SetComboBoxValue(Cmb_Program, Sel_Prog);

                    List<SaldefEntity> SALDEFEntity = new List<SaldefEntity>();
                    SaldefEntity Search_saldef_Entity = new SaldefEntity(true);

                    List<SaldefEntity> SALDEF = _model.SALDEFData.Browse_SALDEF(Search_saldef_Entity, "Browse", BaseForm.UserID, BaseForm.BaseAgency);
                    if (BaseForm.BaseAgencyControlDetails.ServicePlanHiecontrol.Trim() == "Y")
                    {
                        if (!string.IsNullOrEmpty(Pass_CA_Entity.Act_PROG.Trim()))
                        {
                            SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                            if (SALDEFEntity.Count == 0)
                                SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                            if (SALDEFEntity.Count == 0)
                                SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                        }
                        else
                        {
                            SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Contains(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S"));
                            if (SALDEFEntity.Count == 0)
                                SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                            if (SALDEFEntity.Count == 0)
                                SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                            //SALDEF = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_TYPE.Equals("S"));
                        }
                    }
                    else
                    {
                        SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Contains(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S"));
                        if (SALDEFEntity.Count == 0)
                            SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                        if (SALDEFEntity.Count == 0)
                            SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S"));
                        //SALDEF = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_TYPE.Equals("S"));
                    }

                    if (SALDEFEntity.Count > 0) 
                    { 
                        CustomQuestions = "Y"; SaldefList = SALDEFEntity;

                        SALQUESEntity = new List<SalquesEntity>();
                        if (SALDEFEntity.Count > 0)
                        {
                            SalquesEntity Search_Salques_Entity = new SalquesEntity(true);
                            Search_Salques_Entity.SALQ_SALD_ID = SALDEFEntity[0].SALD_ID;
                            SALQUESEntity = _model.SALDEFData.Browse_SALQUES(Search_Salques_Entity, "Browse");

                            if (SALQUESEntity.Count > 1) CustomQuestions = "Y"; else CustomQuestions = "N";

                        }
                    } 
                    else CustomQuestions = "N";

                    if (Mode == "Add")
                    {
                        if (CustomQuestions == "Y") SP_CA_Grid.Columns[31].Visible = true; else SP_CA_Grid.Columns[31].Visible = false;
                    }
                }
            }
        }

        private void Get_Latest_Activity_data()
        {
            Pass_CA_Entity.Check_Date = Pass_CA_Entity.Followup_Comp = Pass_CA_Entity.Followup_On =
            Pass_CA_Entity.Caseworker = Pass_CA_Entity.Site = Pass_CA_Entity.Fund1 = Pass_CA_Entity.Fund2 =
            Pass_CA_Entity.Fund3 = Pass_CA_Entity.Vendor_No = Pass_CA_Entity.Check_No = Pass_CA_Entity.Cost = Pass_CA_Entity.Refer_Data = Pass_CA_Entity.Rate =
            Pass_CA_Entity.Cust_Code1 = Pass_CA_Entity.Cust_Value1 =
            Pass_CA_Entity.Cust_Code2 = Pass_CA_Entity.Cust_Value2 =
            Pass_CA_Entity.Cust_Code3 = Pass_CA_Entity.Cust_Value3 =
            Pass_CA_Entity.Cust_Code4 = Pass_CA_Entity.Cust_Value4 =
            Pass_CA_Entity.Cust_Code5 = Pass_CA_Entity.Cust_Value5 = Pass_CA_Entity.CA_OBF =
            Pass_CA_Entity.Act_PROG = Pass_CA_Entity.UOM = Pass_CA_Entity.Units = string.Empty;

            if (string.IsNullOrEmpty(Pass_CA_Entity.Bulk.Trim()))
                Pass_CA_Entity.Bulk = "Q";

            if (Act_Date.Checked)
                Pass_CA_Entity.ACT_Date = Act_Date.Value.ToShortDateString();

            if (dtActSeek_Date.Checked)
                Pass_CA_Entity.ActSeek_Date = dtActSeek_Date.Value.ToShortDateString();

            if (CmbWorker.Items.Count > 0)
            {
                if (((Captain.Common.Utilities.ListItem)CmbWorker.SelectedItem).Value.ToString() != "0")
                    Pass_CA_Entity.Caseworker = ((Captain.Common.Utilities.ListItem)CmbWorker.SelectedItem).Value.ToString();
            }

            if (CmbSite.Items.Count > 0)
            {
                if (((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Value.ToString() != "0")
                    Pass_CA_Entity.Site = ((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Value.ToString();
            }

            if (CmbFunding1.Items.Count > 0)
            {
                if (((Captain.Common.Utilities.ListItem)CmbFunding1.SelectedItem).Value.ToString() != "0")
                    Pass_CA_Entity.Fund1 = ((Captain.Common.Utilities.ListItem)CmbFunding1.SelectedItem).Value.ToString();
            }

            if (CmbFunding2.Items.Count > 0)
            {

                if (((Captain.Common.Utilities.ListItem)CmbFunding2.SelectedItem).Value.ToString() != "0")
                    Pass_CA_Entity.Fund2 = ((Captain.Common.Utilities.ListItem)CmbFunding2.SelectedItem).Value.ToString();
            }

            if (CmbFunding3.Items.Count > 0)
            {
                if (((Captain.Common.Utilities.ListItem)CmbFunding3.SelectedItem).Value.ToString() != "0")
                    Pass_CA_Entity.Fund3 = ((Captain.Common.Utilities.ListItem)CmbFunding3.SelectedItem).Value.ToString();
            }

            if (Cmb_UOM.Items.Count > 0)
            {
                if (((Captain.Common.Utilities.ListItem)Cmb_UOM.SelectedItem).Value.ToString() != "0")
                    Pass_CA_Entity.UOM = ((Captain.Common.Utilities.ListItem)Cmb_UOM.SelectedItem).Value.ToString();
            }
            //if (Cmb_UOM2.Items.Count > 0)
            //{
            //    if (((Captain.Common.Utilities.ListItem)Cmb_UOM2.SelectedItem).Value.ToString() != "0")
            //        Pass_CA_Entity.UOM2 = ((Captain.Common.Utilities.ListItem)Cmb_UOM2.SelectedItem).Value.ToString();
            //}

            //if (Cmb_UOM3.Items.Count > 0)
            //{
            //    if (((Captain.Common.Utilities.ListItem)Cmb_UOM3.SelectedItem).Value.ToString() != "0")
            //        Pass_CA_Entity.UOM3 = ((Captain.Common.Utilities.ListItem)Cmb_UOM3.SelectedItem).Value.ToString();
            //}


            if (!string.IsNullOrEmpty(Txt_Units.Text.Trim()))
            {
                Pass_CA_Entity.Units = Txt_Units.Text.Trim();
            }
            //if (!string.IsNullOrEmpty(Txt_Units2.Text.Trim()))
            //{
            //    Pass_CA_Entity.Units2 = Txt_Units2.Text.Trim();
            //}
            //if (!string.IsNullOrEmpty(Txt_Units3.Text.Trim()))
            //{
            //    Pass_CA_Entity.Units3 = Txt_Units3.Text.Trim();
            //}


            if (!string.IsNullOrEmpty(Txt_CA_Program.Text.Trim()))
                Pass_CA_Entity.Act_PROG = Txt_CA_Program.Text.Substring(0, 6);

            if (!string.IsNullOrEmpty(Txt_VendNo.Text))
                Pass_CA_Entity.Vendor_No = Txt_VendNo.Text;


            if (!string.IsNullOrEmpty(Txtx_ChkNo.Text))
                Pass_CA_Entity.Check_No = Txtx_ChkNo.Text;


            if (Check_Date.Checked)
                Pass_CA_Entity.Check_Date = Check_Date.Value.ToShortDateString();


            if (!string.IsNullOrEmpty(Txt_Cost.Text))
                Pass_CA_Entity.Cost = Txt_Cost.Text;

            if (!string.IsNullOrEmpty(txtRate.Text.Trim()))
                Pass_CA_Entity.Rate = txtRate.Text;

            //if (!string.IsNullOrEmpty(Txt_Cost3.Text))
            //    Pass_CA_Entity.Amount3 = Txt_Cost3.Text;

            //if (!string.IsNullOrEmpty(Txt_RefTo.Text))
            //    Pass_CA_Entity.Refer_Data = Txt_RefTo.Text;

            if (UpOn_Date.Checked)
                Pass_CA_Entity.Followup_On = UpOn_Date.Value.ToShortDateString();

            if (Complete_Date.Checked)
                Pass_CA_Entity.Followup_Comp = Complete_Date.Value.ToShortDateString();

            if (!string.IsNullOrEmpty(Txt_TobeFollowUp.Text))
                Pass_CA_Entity.Followup_By = Txt_TobeFollowUp.Text;
            if (Mode == "Add")
            {
                foreach (DataGridViewRow dr in SP_CA_Grid.Rows)
                {
                    if (dr.Cells["Sel_1"].Value.ToString() == true.ToString() && dr.Cells["SP2_CAMS_Code"].Value.ToString() == Pass_CA_Entity.ACT_Code)
                    {
                        if (!string.IsNullOrEmpty(dr.Cells["Act_Ques1"].Value.ToString().Trim()))
                            Pass_CA_Entity.Cust_Code1 = dr.Cells["Act_Ques1"].Value.ToString();
                        if (!string.IsNullOrEmpty(dr.Cells["Act_Resp1"].Value.ToString().Trim()))
                            Pass_CA_Entity.Cust_Value1 = dr.Cells["Act_Resp1"].Value.ToString();
                        if (!string.IsNullOrEmpty(dr.Cells["Act_Ques2"].Value.ToString().Trim()))
                            Pass_CA_Entity.Cust_Code2 = dr.Cells["Act_Ques2"].Value.ToString();
                        if (!string.IsNullOrEmpty(dr.Cells["Act_Resp2"].Value.ToString().Trim()))
                            Pass_CA_Entity.Cust_Value2 = dr.Cells["Act_Resp2"].Value.ToString();
                        if (!string.IsNullOrEmpty(dr.Cells["Act_Ques3"].Value.ToString().Trim()))
                            Pass_CA_Entity.Cust_Code3 = dr.Cells["Act_Ques3"].Value.ToString();
                        if (!string.IsNullOrEmpty(dr.Cells["Act_Resp3"].Value.ToString().Trim()))
                            Pass_CA_Entity.Cust_Value3 = dr.Cells["Act_Resp3"].Value.ToString();
                        if (!string.IsNullOrEmpty(dr.Cells["Act_Ques4"].Value.ToString().Trim()))
                            Pass_CA_Entity.Cust_Code4 = dr.Cells["Act_Ques4"].Value.ToString();
                        if (!string.IsNullOrEmpty(dr.Cells["Act_Resp4"].Value.ToString().Trim()))
                            Pass_CA_Entity.Cust_Value4 = dr.Cells["Act_Resp4"].Value.ToString();
                        if (!string.IsNullOrEmpty(dr.Cells["Act_Ques5"].Value.ToString().Trim()))
                            Pass_CA_Entity.Cust_Code5 = dr.Cells["Act_Ques5"].Value.ToString();
                        if (!string.IsNullOrEmpty(dr.Cells["Act_Resp5"].Value.ToString().Trim()))
                            Pass_CA_Entity.Cust_Value5 = dr.Cells["Act_Resp5"].Value.ToString();

                        break;
                    }
                }
            }
            else
            {
                int Tmp_Cust_Cnt = 1;
                string Curr_Ques_Type = "";
                if (Cust_Grid.Rows.Count > 0)
                {
                    this.Cust_Grid.Sort(this.Cust_Grid.Columns["Code"], ListSortDirection.Ascending);
                }
                foreach (DataGridViewRow dr in Cust_Grid.Rows)
                {
                    Curr_Ques_Type = dr.Cells["Type"].Value.ToString();
                    switch (Tmp_Cust_Cnt)
                    {
                        case 1:
                            if (!string.IsNullOrEmpty(dr.Cells["Resp"].EditedFormattedValue.ToString()))
                            {
                                Pass_CA_Entity.Cust_Code1 = dr.Cells["Code"].Value.ToString();
                                if (Curr_Ques_Type == "C")
                                    Pass_CA_Entity.Cust_Value1 = dr.Cells["Resp_Code"].Value.ToString();
                                else
                                    Pass_CA_Entity.Cust_Value1 = dr.Cells["Resp"].Value.ToString();

                                if (string.IsNullOrEmpty(Pass_CA_Entity.Cust_Value1.Trim()))
                                    Pass_CA_Entity.Cust_Value1 = null;
                            }
                            break;
                        case 2:
                            if (!string.IsNullOrEmpty(dr.Cells["Resp"].EditedFormattedValue.ToString()))
                            {
                                Pass_CA_Entity.Cust_Code2 = dr.Cells["Code"].Value.ToString();
                                if (Curr_Ques_Type == "C")
                                    Pass_CA_Entity.Cust_Value2 = dr.Cells["Resp_Code"].Value.ToString();
                                else
                                    Pass_CA_Entity.Cust_Value2 = dr.Cells["Resp"].Value.ToString();

                                if (string.IsNullOrEmpty(Pass_CA_Entity.Cust_Value2.Trim()))
                                    Pass_CA_Entity.Cust_Value2 = null;
                            }
                            break;
                        case 3:
                            if (!string.IsNullOrEmpty(dr.Cells["Resp"].EditedFormattedValue.ToString()))
                            {
                                Pass_CA_Entity.Cust_Code3 = dr.Cells["Code"].Value.ToString();
                                if (Curr_Ques_Type == "C")
                                    Pass_CA_Entity.Cust_Value3 = dr.Cells["Resp_Code"].Value.ToString();
                                else
                                    Pass_CA_Entity.Cust_Value3 = dr.Cells["Resp"].Value.ToString();

                                if (string.IsNullOrEmpty(Pass_CA_Entity.Cust_Value3.Trim()))
                                    Pass_CA_Entity.Cust_Value3 = null;
                            }
                            break;
                        case 4:
                            if (!string.IsNullOrEmpty(dr.Cells["Resp"].EditedFormattedValue.ToString()))
                            {
                                Pass_CA_Entity.Cust_Code4 = dr.Cells["Code"].Value.ToString();
                                if (Curr_Ques_Type == "C")
                                    Pass_CA_Entity.Cust_Value4 = dr.Cells["Resp_Code"].Value.ToString();
                                else
                                    Pass_CA_Entity.Cust_Value4 = dr.Cells["Resp"].Value.ToString();

                                if (string.IsNullOrEmpty(Pass_CA_Entity.Cust_Value4.Trim()))
                                    Pass_CA_Entity.Cust_Value4 = null;
                            }
                            break;
                        case 5:
                            if (!string.IsNullOrEmpty(dr.Cells["Resp"].EditedFormattedValue.ToString()))
                            {
                                Pass_CA_Entity.Cust_Code5 = dr.Cells["Code"].Value.ToString();
                                if (Curr_Ques_Type == "C")
                                    Pass_CA_Entity.Cust_Value5 = dr.Cells["Resp_Code"].Value.ToString();
                                else
                                    Pass_CA_Entity.Cust_Value5 = dr.Cells["Resp"].Value.ToString();

                                if (string.IsNullOrEmpty(Pass_CA_Entity.Cust_Value5.Trim()))
                                    Pass_CA_Entity.Cust_Value5 = null;
                            }
                            break;
                    }
                    Tmp_Cust_Cnt++;
                }
            }
        }

        private void Get_Latest_MS_data(CASEMSEntity Entity)
        {

            if (Act_Date.Checked)
                Pass_MS_Entity.Date = Act_Date.Value.ToShortDateString();

            if (dtActSeek_Date.Checked)
                Pass_MS_Entity.Seek_Date = dtActSeek_Date.Value.ToShortDateString();

            Pass_MS_Entity.Acty_PROG = Pass_MS_Entity.Site = Pass_MS_Entity.Result = Pass_MS_Entity.CaseWorker = string.Empty;


            if (CmbSite.Items.Count > 0)
            {
                if (((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Value.ToString() != "0")
                    Pass_MS_Entity.Site = ((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Value.ToString();
            }

            if (!string.IsNullOrEmpty(Entity.Result.Trim()))
            {
                Pass_MS_Entity.Result = Entity.Result.Trim();
            }

            if (CmbWorker.Items.Count > 0)
            {
                if (((Captain.Common.Utilities.ListItem)CmbWorker.SelectedItem).Value.ToString() != "0")
                    Pass_MS_Entity.CaseWorker = ((Captain.Common.Utilities.ListItem)CmbWorker.SelectedItem).Value.ToString();
            }



            //if (!string.IsNullOrEmpty(((ListItem)Cmb_MS_Program.SelectedItem).Value.ToString()))
            //{
            //    if (((ListItem)Cmb_MS_Program.SelectedItem).Value.ToString() != "0")
            //        Pass_MS_Entity.Acty_PROG = ((ListItem)Cmb_MS_Program.SelectedItem).Value.ToString();
            //}

            if (!string.IsNullOrEmpty(Txt_CA_Program.Text.Trim()))
                Pass_MS_Entity.Acty_PROG = Txt_CA_Program.Text.Trim().Substring(0, 6);
        }


        bool Future_Date_Soft_Edit = false;
        private bool isValidate()
        {
            bool isValid = true;
            if (CAMS_FLG == "CA")
            {
                if (lblActivityDateReq.Visible)
                {

                    if (Act_Date.Checked == false)
                    {
                        _errorProvider.SetError(Act_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblActivityDate.Text.Replace(Consts.Common.Colon, string.Empty)));
                        isValid = false;
                    }
                    else
                    {

                        if (!(BaseForm.UserProfile.Security == "P" || BaseForm.UserProfile.Security == "B"))
                        {
                            if (SERVStopEntity != null && Act_Date.Checked)
                            {
                                if (Convert.ToDateTime(SERVStopEntity.TDate.Trim()) >= Act_Date.Value && Convert.ToDateTime(SERVStopEntity.FDate.Trim()) <= Act_Date.Value)
                                {
                                    _errorProvider.SetError(Act_Date, string.Format(" " + lblActivityDate.Text + " Should not be between " + LookupDataAccess.Getdate(SERVStopEntity.FDate.Trim()) + " and " + LookupDataAccess.Getdate(SERVStopEntity.TDate.Trim()).Replace(Consts.Common.Colon, string.Empty)));
                                    isValid = false;
                                }
                                else if (!string.IsNullOrEmpty(Sp_Start_Date.Trim()))
                                {
                                    if (string.IsNullOrEmpty(Sp_End_Date.Trim()))
                                    {
                                        if (Act_Date.Value < Convert.ToDateTime(Sp_Start_Date))
                                        {
                                            _errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Prior to 'Service Plan Master Date'".Replace(Consts.Common.Colon, string.Empty)));
                                            isValid = false;
                                        }
                                        else
                                            _errorProvider.SetError(Act_Date, null);
                                    }
                                    else
                                    {
                                        if (Act_Date.Value < Convert.ToDateTime(Sp_Start_Date) || Act_Date.Value > Convert.ToDateTime(Sp_End_Date))
                                        {
                                            _errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Prior to 'Service Plan Master Date' OR 'Actual Completion Date'".Replace(Consts.Common.Colon, string.Empty)));
                                            isValid = false;
                                        }
                                        else
                                            _errorProvider.SetError(Act_Date, null);
                                    }

                                }
                                else
                                {
                                    _errorProvider.SetError(Act_Date, null);
                                }

                            }
                            else
                            {
                                bool Future_Date_Flg = false;
                                if (Convert.ToDateTime(Act_Date.Value).Date > DateTime.Today.Date && !Future_Date_Soft_Edit)
                                {
                                    //MessageBox.Show("' " + lblActivityDate.Text + "' Should not be Future Date", "CAPSYSTEMS");

                                    MessageBox.Show("You are about to post a future date. \n Do you want to proceed?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Allow_Post_Future_Date);

                                    //_errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Future Date".Replace(Consts.Common.Colon, string.Empty)));
                                    isValid = false;
                                    Future_Date_Flg = true;
                                }
                                else
                                {
                                    Future_Date_Soft_Edit = false;
                                    _errorProvider.SetError(Act_Date, null);
                                }


                                if (!Future_Date_Flg)
                                {
                                    if (!string.IsNullOrEmpty(Sp_Start_Date.Trim()))
                                    {
                                        if (string.IsNullOrEmpty(Sp_End_Date.Trim()))
                                        {
                                            if (Act_Date.Value < Convert.ToDateTime(Sp_Start_Date))
                                            {
                                                _errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Prior to 'Service Plan Master Date'".Replace(Consts.Common.Colon, string.Empty)));
                                                isValid = false;
                                            }
                                            else
                                                _errorProvider.SetError(Act_Date, null);
                                        }
                                        else
                                        {
                                            if (Act_Date.Value < Convert.ToDateTime(Sp_Start_Date) || Act_Date.Value > Convert.ToDateTime(Sp_End_Date))
                                            {
                                                _errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Prior to 'Service Plan Master Date' OR 'Actual Completion Date'".Replace(Consts.Common.Colon, string.Empty)));
                                                isValid = false;
                                            }
                                            else
                                                _errorProvider.SetError(Act_Date, null);
                                        }

                                    }
                                    else
                                        _errorProvider.SetError(Act_Date, null);
                                }
                            }
                        }
                        else
                        {
                            bool Future_Date_Flg = false;
                            if (Convert.ToDateTime(Act_Date.Value).Date > DateTime.Today.Date && !Future_Date_Soft_Edit)
                            {
                                //MessageBox.Show("' " + lblActivityDate.Text + "' Should not be Future Date", "CAPSYSTEMS");

                                MessageBox.Show("You are about to post a future date. \n Do you want to proceed?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Allow_Post_Future_Date);

                                //_errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Future Date".Replace(Consts.Common.Colon, string.Empty)));
                                isValid = false;
                                Future_Date_Flg = true;
                            }
                            else
                            {
                                Future_Date_Soft_Edit = false;
                                _errorProvider.SetError(Act_Date, null);
                            }


                            if (!Future_Date_Flg)
                            {
                                if (!string.IsNullOrEmpty(Sp_Start_Date.Trim()))
                                {
                                    if (string.IsNullOrEmpty(Sp_End_Date.Trim()))
                                    {
                                        if (Act_Date.Value < Convert.ToDateTime(Sp_Start_Date))
                                        {
                                            _errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Prior to 'Service Plan Master Date'".Replace(Consts.Common.Colon, string.Empty)));
                                            isValid = false;
                                        }
                                        else
                                            _errorProvider.SetError(Act_Date, null);
                                    }
                                    else
                                    {
                                        if (Act_Date.Value < Convert.ToDateTime(Sp_Start_Date) || Act_Date.Value > Convert.ToDateTime(Sp_End_Date))
                                        {
                                            _errorProvider.SetError(Act_Date, string.Format("' " + lblActivityDate.Text + "' Should not be Prior to 'Service Plan Master Date' OR 'Actual Completion Date'".Replace(Consts.Common.Colon, string.Empty)));
                                            isValid = false;
                                        }
                                        else
                                            _errorProvider.SetError(Act_Date, null);
                                    }

                                }
                                else
                                    _errorProvider.SetError(Act_Date, null);
                            }
                        }
                    }
                }
                else
                    _errorProvider.SetError(Act_Date, null);

                if (lblFunSour1Req.Visible && (CmbFunding1.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbFunding1.SelectedItem).Text.Trim()))))
                {
                    _errorProvider.SetError(CmbFunding1, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblFunSour1.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(CmbFunding1, null);

                if (lblFunSour2Req.Visible && (CmbFunding2.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbFunding2.SelectedItem).Text.Trim()))))
                {
                    _errorProvider.SetError(CmbFunding2, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblFunSour2.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(CmbFunding2, null);

                if (lblFunSour3Req.Visible && (CmbFunding3.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbFunding3.SelectedItem).Text.Trim()))))
                {
                    _errorProvider.SetError(CmbFunding3, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblFunSour3.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(CmbFunding3, null);

                if (lblCaseworcaReq.Visible && (CmbWorker.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbWorker.SelectedItem).Text.Trim()))))
                {
                    _errorProvider.SetError(CmbWorker, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblCaseworca.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(CmbWorker, null);

                if (lblSitecaReq.Visible && (CmbSite.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)CmbSite.SelectedItem).Text.Trim()))))
                {
                    _errorProvider.SetError(CmbSite, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblSiteca.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(CmbSite, null);

                if (lblCheckDateReq.Visible && Check_Date.Checked == false)
                {
                    _errorProvider.SetError(Check_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblCheckDate.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(Check_Date, null);

                if (lblVendorReq.Visible && string.IsNullOrEmpty(Txt_VendNo.Text.Trim()))
                {
                    _errorProvider.SetError(Text_VendName, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblVendor.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(Text_VendName, null);

                if (lblCostReq.Visible && string.IsNullOrEmpty(Txt_Cost.Text.Trim()))
                {
                    _errorProvider.SetError(Txt_Cost, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblCost.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(Txt_Cost, null);

                if (lblFollowupComplReq.Visible && Complete_Date.Checked == false)
                {
                    _errorProvider.SetError(Complete_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblFollowupCompl.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(Complete_Date, null);

                if (lblFollowupReq.Visible && UpOn_Date.Checked == false)
                {
                    _errorProvider.SetError(UpOn_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblFollowup.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(UpOn_Date, null);

                if (lblTobeFolledReq.Visible && string.IsNullOrEmpty(Txt_TobeFollowUp.Text.Trim()))
                {
                    _errorProvider.SetError(Txt_TobeFollowUp, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblTobeFolledReq.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(Txt_TobeFollowUp, null);

                if (lblCheckReq.Visible && string.IsNullOrEmpty(Txtx_ChkNo.Text.Trim()))
                {
                    _errorProvider.SetError(Txtx_ChkNo, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblCheck.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(Txtx_ChkNo, null);

                if (lblRateReq.Visible && string.IsNullOrEmpty(txtRate.Text.Trim()))
                {
                    _errorProvider.SetError(txtRate, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblRate.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(txtRate, null);


                //if (Txtx_ChkNo.Enabled)
                //{
                //    if (string.IsNullOrEmpty(Txtx_ChkNo.Text.Trim()))
                //    {
                //        _errorProvider.SetError(Txtx_ChkNo, lblCheck.Text + " May Not be zero.");
                //        isValid = false;
                //    }
                //    else if ((int.Parse(Txtx_ChkNo.Text) <= 0))
                //        {
                //            _errorProvider.SetError(Txtx_ChkNo, lblCheck.Text + " May Not be zero.");
                //            isValid = false;
                //        }
                //    else
                //        _errorProvider.SetError(Txtx_ChkNo, null);
                //}

                //if (LblProgramReq.Visible && (Cmb_Program.SelectedItem == null || (string.IsNullOrEmpty(((ListItem)Cmb_Program.SelectedItem).Text.Trim()))))
                //{
                //    _errorProvider.SetError(Cmb_Program, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), LblProgram.Text));
                //    isValid = false;
                //}
                //else
                //    _errorProvider.SetError(Cmb_Program, null);


                if (LblProgramReq.Visible && ((string.IsNullOrEmpty(Txt_CA_Program.Text.Trim()))))
                {
                    _errorProvider.SetError(panel8, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), LblProgram.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(panel8, null);


                if (LblUOM_Req.Visible && (Cmb_UOM.SelectedItem == null || (string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)Cmb_UOM.SelectedItem).Text.Trim()))))
                {
                    _errorProvider.SetError(Cmb_UOM, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Lbl_UOM.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(Cmb_UOM, null);

                if (LblUnits_Req.Visible && (string.IsNullOrEmpty(Txt_Units.Text.Trim())))
                {
                    _errorProvider.SetError(Txt_Units, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), Lbl_Units.Text));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(Txt_Units, null);

                if (lblActSeekDateReq.Visible && dtActSeek_Date.Checked == false)
                {
                    _errorProvider.SetError(dtActSeek_Date, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblActSeekDate.Text.Replace(Consts.Common.Colon, string.Empty)));
                    isValid = false;
                }
                else
                    _errorProvider.SetError(dtActSeek_Date, null);

                if (dtActSeek_Date.Visible)
                {

                    if (dtActSeek_Date.Checked && Act_Date.Checked)
                    {
                        if (Act_Date.Value < dtActSeek_Date.Value)
                        {
                            _errorProvider.SetError(dtActSeek_Date, "Please The Activity/Service Requested Date never Greater than Activity/Service Date");
                            isValid = false;
                        }
                        else
                            _errorProvider.SetError(dtActSeek_Date, null);
                    }
                }

                if (Complete_Date.Checked)
                {
                    if (Convert.ToDateTime(Complete_Date.Value) > DateTime.Now.Date)
                    {
                        _errorProvider.SetError(Complete_Date, "Future Date is not allowed");
                        isValid = false;
                    }
                }

                //if ((UpOn_Date.Checked) && (Complete_Date.Checked))
                //{
                //    if (Convert.ToDateTime(UpOn_Date.Value) > Convert.ToDateTime(Complete_Date.Value))
                //    {
                //        _errorProvider.SetError(Complete_Date, "Complete Date can’t be less than Follow up On Date");
                //        isValid = false;
                //    }
                //}
                //else
                //{
                if (Complete_Date.Checked == true && UpOn_Date.Checked == false)
                {
                    _errorProvider.SetError(UpOn_Date, "Follow-up on Date is Required");
                    isValid = false;
                }
                //}

                if (Mode == "Add")
                {
                    foreach (DataGridViewRow dataGridViewRow in SP_CA_Grid.Rows)
                    {

                        if (dataGridViewRow.Cells["Act_CustReq"].Value.ToString() == "*" && dataGridViewRow.Cells["Sel_1"].Value.ToString() == true.ToString())
                        {
                            //string inputValue = string.Empty;
                            //inputValue = dataGridViewRow.Cells["Resp"].Value != null ? dataGridViewRow.Cells["Resp"].Value.ToString() : string.Empty;
                            //if (inputValue.Trim() == string.Empty)
                            //{
                            //CommonFunctions.MessageBoxDisplay("Please Provide Response for Required Custom Question(s)");
                            CommonFunctions.MessageBoxDisplay("Custom Question requires a response for " + dataGridViewRow.Cells["SP2_CAMS_Code"].Value.ToString() + "-" + dataGridViewRow.Cells["SP2_Desc"].Value.ToString());
                            isValid = false;
                            break;
                            //}
                        }
                    }

                    foreach (DataGridViewRow dr in SP_CA_Grid.Rows)
                    {
                        if (dr.Cells["Sel_1"].Value.ToString() == true.ToString() && dr.Cells["SP2_OBF"].Value.ToString() == "3")
                        {
                            if (CAOBO_Ser_List.Count > 0)
                            {
                                List<CAOBOEntity> SelCAOBO = CAOBO_Ser_List.FindAll(u => u.Code.Trim() == dr.Cells["SP2_CAMS_Code"].Value.ToString().Trim());
                                if (SelCAOBO.Count == 0)
                                {
                                    CommonFunctions.MessageBoxDisplay("Member Should be Selected for " + dr.Cells["SP2_CAMS_Code"].Value.ToString() + "-" + dr.Cells["SP2_Desc"].Value.ToString());
                                    isValid = false;
                                    break;
                                }
                            }
                            else
                            {
                                CommonFunctions.MessageBoxDisplay("Member Should be Selected for " + dr.Cells["SP2_CAMS_Code"].Value.ToString() + "-" + dr.Cells["SP2_Desc"].Value.ToString());
                                isValid = false;
                                break;
                            }
                        }
                    }

                    foreach (DataGridViewRow dr in gvMSgrid.Rows)
                    {
                        if (dr.Cells["MSSel_1"].Value.ToString() == true.ToString() && dr.Cells["MSSP2_OBF"].Value.ToString() == "3")
                        {
                            if (CAOBO_Ser_List.Count > 0)
                            {
                                List<CAOBOEntity> SelCAOBO = CAOBO_Ser_List.FindAll(u => u.Code.Trim() == dr.Cells["MS_SP2_CAMS_Code"].Value.ToString().Trim());
                                if (SelCAOBO.Count == 0)
                                {
                                    CommonFunctions.MessageBoxDisplay("Member Should be Selected for " + dr.Cells["MS_SP2_CAMS_Code"].Value.ToString() + "-" + dr.Cells["MSSP2_Desc"].Value.ToString());
                                    isValid = false;
                                    break;
                                }
                            }
                            else
                            {
                                CommonFunctions.MessageBoxDisplay("Member Should be Selected for " + dr.Cells["MS_SP2_CAMS_Code"].Value.ToString() + "-" + dr.Cells["MSSP2_Desc"].Value.ToString());
                                isValid = false;
                                break;
                            }
                        }
                    }

                }
                else
                {
                    foreach (DataGridViewRow dataGridViewRow in Cust_Grid.Rows)
                    {

                        if (dataGridViewRow.Cells["CA_Cust_Req"].Value.ToString() == "*")
                        {
                            string inputValue = string.Empty;
                            inputValue = dataGridViewRow.Cells["Resp"].Value != null ? dataGridViewRow.Cells["Resp"].Value.ToString() : string.Empty;
                            if (inputValue.Trim() == string.Empty)
                            {
                                //CommonFunctions.MessageBoxDisplay("Please Provide Response for Required Custom Question(s)");
                                CommonFunctions.MessageBoxDisplay("Custom Question requires a response");
                                isValid = false;
                                break;
                            }
                        }
                    }
                }

            }
            return isValid;
        }

        private void Allow_Post_Future_Date(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                Future_Date_Soft_Edit = true;
                if (CAMS_FLG == "CA")
                    Btn_CASave_Click(Btn_CASave, EventArgs.Empty);
                //else
                //    Btn_MS_Save_Click(Btn_MS_Save, EventArgs.Empty);
            }
        }


        string Current_CA_Seq = "1"; string Sql_SP_Result_Message = string.Empty;
        private void Btn_CASave_Click(object sender, EventArgs e)
        {
            if (isValidate())
            {
                bool ISpassMSResult = true;

                if (CASEMSList.Count > 0)
                {
                    foreach (CASEMSEntity dr in CASEMSList)
                    {
                        if (dr.Rec_Type.ToString() == "I")
                        {
                            if (dr.Result.ToString() == "" || dr.Result.ToString() == string.Empty)
                            { ISpassMSResult = false; break; }
                        }
                    }
                }
                else { ISpassMSResult = true; }


                Get_Latest_Activity_data();

                if (Mode == "Add")
                {
                    //Added by Sudheer on 12/19/2022
                    if (CA_Template_List.Count > 0)
                    {
                        List<CAOBOEntity> Sel_CAOBO_Serlist = new List<CAOBOEntity>();
                        if (CAOBO_Ser_List.Count > 0)
                        {
                            Sel_CAOBO_Serlist = CAOBO_Ser_List.FindAll(u => u.Type.Trim() == "CA" && u.Code.Trim() == Pass_CA_Entity.ACT_Code.Trim() && u.Branch.Trim() == Pass_CA_Entity.Branch.Trim() && u.Group.Trim() == Pass_CA_Entity.Group.Trim());
                        }


                        if (!CAMS_Desc.Contains("Auto Post – "))
                        {
                            Pass_CA_Entity.CA_OBF = ((Captain.Common.Utilities.ListItem)cmb_Benefit.SelectedItem).Value.ToString();
                        }

                        if (Sel_CAOBO_Serlist.Count > 0)
                            Pass_CA_Entity.CA_OBF = Sel_CAOBO_Serlist[0].BenefitFrom.ToString();

                        bool IsCAOBO = false;
                        if (CAMS_Desc.Contains("Auto Post – "))
                        {
                            Pass_CA_Entity.CA_OBF = ((Captain.Common.Utilities.ListItem)cmb_Benefit.SelectedItem).Value.ToString();

                            foreach (DataGridViewRow dr in CA_Members_Grid.Rows)
                            {
                                if (dr.Cells["CA_Sel"].Value.ToString() == true.ToString())
                                {
                                    IsCAOBO = true; break;
                                }
                            }

                        }
                        else IsCAOBO = true;


                        if ((Pass_CA_Entity.CA_OBF == "3" && CAOBO_List.Count == 0) || (Pass_CA_Entity.CA_OBF == "3" && !IsCAOBO))
                        {
                            AlertBox.Show("Members should be selected", MessageBoxIcon.Warning);
                        }
                        else
                        {
                            CASEACTEntity SelCAEntity = CA_Template_List.Find(u => u.ACT_Code.Trim() == Pass_CA_Entity.ACT_Code.Trim() && u.IsSave == "Y");
                            if (SelCAEntity != null) Pass_CA_Entity.Rec_Type = "U";

                            string Operatipn_Mode = "Insert";

                            if (Pass_CA_Entity.Rec_Type == "U")
                                Operatipn_Mode = "Update";

                            Pass_CA_Entity.Lsct_Operator = BaseForm.UserID;

                            int New_CAID = 1, New_CA_Seq = 1;
                            if (!string.IsNullOrEmpty(Pass_CA_Entity.ACT_ID) && Pass_CA_Entity.Rec_Type == "U")
                                New_CAID = int.Parse(Pass_CA_Entity.ACT_ID);
                            else
                                Pass_CA_Entity.ACT_ID = "1";



                            if (CAMS_Desc.Contains("Auto Post – "))
                            {
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                                return;
                            }
                            List<CAOBOEntity> SelOBOEntity = new List<CAOBOEntity>();
                            if (_model.SPAdminData.UpdateCASEACT2(Pass_CA_Entity, Operatipn_Mode, out New_CAID, out New_CA_Seq, out Sql_SP_Result_Message))
                            {
                                Pass_CA_Entity.ACT_ID = New_CAID.ToString();
                                Pass_CA_Entity.ACT_Seq = Current_CA_Seq = New_CA_Seq.ToString();

                                if (Sel_CAOBO_Serlist.Count > 0)
                                {
                                    Search_CAOBO_Entity.ID = New_CAID.ToString();
                                    Search_CAOBO_Entity.Rec_Type = "S";
                                    Search_CAOBO_Entity.Seq = "1";

                                    _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                    foreach (CAOBOEntity Entity in Sel_CAOBO_Serlist)
                                    {
                                        Search_CAOBO_Entity.CLID = Entity.CLID.ToString();
                                        Search_CAOBO_Entity.Fam_Seq = Entity.Fam_Seq.ToString();
                                        Search_CAOBO_Entity.Seq = "1";
                                        Search_CAOBO_Entity.Rec_Type = "I";

                                        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);
                                    }
                                }
                                else if (CAOBO_List.Count > 0)
                                {
                                    Search_CAOBO_Entity.ID = New_CAID.ToString();
                                    Search_CAOBO_Entity.Rec_Type = "S";
                                    Search_CAOBO_Entity.Seq = "1";

                                    _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                    foreach (CAOBOEntity Entity in CAOBO_List)
                                    {
                                        Search_CAOBO_Entity.CLID = Entity.CLID.ToString();
                                        Search_CAOBO_Entity.Fam_Seq = Entity.Fam_Seq.ToString();
                                        Search_CAOBO_Entity.Seq = "1";
                                        Search_CAOBO_Entity.Rec_Type = "I";

                                        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);
                                    }
                                }
                                else
                                {
                                    if (CA_Members_Grid.Rows.Count > 0)
                                    {
                                        //if (Pass_CA_Entity.CA_OBF == "1")
                                        //{
                                        Search_CAOBO_Entity.ID = New_CAID.ToString();
                                        Search_CAOBO_Entity.Rec_Type = "S";
                                        Search_CAOBO_Entity.Seq = "1";

                                        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                        foreach (DataGridViewRow dr in CA_Members_Grid.Rows)
                                        {
                                            //Search_CAOBO_Entity.ID = Pass_CA_Entity.ACT_ID;
                                            //if (Pass_CA_Entity.Rec_Type == "I")
                                            //    Search_CAOBO_Entity.ID = New_CAID.ToString();

                                            Search_CAOBO_Entity.CLID = dr.Cells["CA_CLID"].Value.ToString();
                                            Search_CAOBO_Entity.Fam_Seq = dr.Cells["CA_Mem_Seq"].Value.ToString();
                                            Search_CAOBO_Entity.Seq = "1";

                                            if (dr.Cells["CA_Sel"].Value.ToString() == true.ToString())
                                            {
                                                Search_CAOBO_Entity.Rec_Type = "I";

                                                _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);

                                                SelOBOEntity.Add(Search_CAOBO_Entity);

                                            }
                                        }
                                        //}
                                        //else if (Pass_CA_Entity.CA_OBF == "2")
                                        //{
                                        //    Search_CAOBO_Entity.ID = New_CAID.ToString();
                                        //    Search_CAOBO_Entity.Rec_Type = "S";
                                        //    Search_CAOBO_Entity.Seq = "1";

                                        //    _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                        //    foreach (DataRow dr in dtmembers.Rows)
                                        //    {
                                        //        if (dr["AppStatus"].ToString() == "A" && dr["SNP_EXCLUDE"].ToString() == "N")
                                        //        {
                                        //            Search_CAOBO_Entity.CLID = dr["ClientID"].ToString();
                                        //            Search_CAOBO_Entity.Fam_Seq = dr["RecFamSeq"].ToString();
                                        //            Search_CAOBO_Entity.Seq = "1";
                                        //            Search_CAOBO_Entity.Rec_Type = "I";

                                        //            _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);


                                        //        }
                                        //    }
                                        //}
                                    }
                                }

                                IsSaveResult(SelOBOEntity);
                            }

                        }

                    }
                    else
                    {
                        if (ISpassMSResult)
                        {
                            List<CAOBOEntity> Sel_CAOBO_Serlist = new List<CAOBOEntity>();
                            if (CAOBO_Ser_List.Count > 0)
                            {
                                Sel_CAOBO_Serlist = CAOBO_Ser_List.FindAll(u => u.Type.Trim() == "CA" && u.Code.Trim() == Pass_CA_Entity.ACT_Code.Trim() && u.Branch.Trim() == Pass_CA_Entity.Branch.Trim() && u.Group.Trim() == Pass_CA_Entity.Group.Trim());
                            }


                            if (!CAMS_Desc.Contains("Auto Post – "))
                            {
                                Pass_CA_Entity.CA_OBF = ((Captain.Common.Utilities.ListItem)cmb_CA_Benefit.SelectedItem).Value.ToString();
                            }

                            if (Sel_CAOBO_Serlist.Count > 0)
                                Pass_CA_Entity.CA_OBF = Sel_CAOBO_Serlist[0].BenefitFrom.ToString();

                            bool IsCAOBO = false;
                            if (CAMS_Desc.Contains("Auto Post – "))
                            {
                                Pass_CA_Entity.CA_OBF = ((Captain.Common.Utilities.ListItem)cmb_Benefit.SelectedItem).Value.ToString();

                                foreach (DataGridViewRow dr in CA_Members_Grid.Rows)
                                {
                                    if (dr.Cells["CA_Sel"].Value.ToString() == true.ToString())
                                    {
                                        IsCAOBO = true; break;
                                    }
                                }

                            }
                            else IsCAOBO = true;


                            if ((Pass_CA_Entity.CA_OBF == "3" && CAOBO_List.Count == 0) || (Pass_CA_Entity.CA_OBF == "3" && !IsCAOBO))
                            {
                                AlertBox.Show("Members should be selected", MessageBoxIcon.Warning);
                            }
                            else
                            {
                                string Operatipn_Mode = "Insert";

                                if (Pass_CA_Entity.Rec_Type == "U")
                                    Operatipn_Mode = "Update";

                                Pass_CA_Entity.Lsct_Operator = BaseForm.UserID;

                                int New_CAID = 1, New_CA_Seq = 1;
                                if (!string.IsNullOrEmpty(Pass_CA_Entity.ACT_ID) && Pass_CA_Entity.Rec_Type == "U")
                                    New_CAID = int.Parse(Pass_CA_Entity.ACT_ID);
                                else
                                    Pass_CA_Entity.ACT_ID = "1";



                                if (CAMS_Desc.Contains("Auto Post – "))
                                {
                                    this.DialogResult = DialogResult.OK;
                                    this.Close();
                                    return;
                                }

                                if (_model.SPAdminData.UpdateCASEACT2(Pass_CA_Entity, Operatipn_Mode, out New_CAID, out New_CA_Seq, out Sql_SP_Result_Message))
                                {

                                    Pass_CA_Entity.ACT_ID = New_CAID.ToString();
                                    Pass_CA_Entity.ACT_Seq = Current_CA_Seq = New_CA_Seq.ToString();
                                    ////if (CA_Members_Grid.Rows.Count > 0)  // Yeswanth Sindhe
                                    ////    Update_CAOBO_Benefitig_Members(New_CAID);

                                    //if(SALData.Count>0)
                                    //{
                                    //    SALData = SALData.OrderBy(u => u.SALACT_SALID).ThenBy(u => u.SALACT_Q_ID).ToList();

                                    //    SALACTEntity Search_Sal = new SALACTEntity(true);
                                    //    Search_Sal.SALACT_ID = New_CAID.ToString();
                                    //    Search_Sal.Mode = "DELETEALL";
                                    //    _model.SALDEFData.CAP_SALACT_INSUPDEL(Search_Sal);
                                    //    int i = 1;
                                    //    foreach (SALACTEntity SalEntity in SALData)
                                    //    {
                                    //        SalEntity.SALACT_ID = New_CAID.ToString();
                                    //        SalEntity.SALACT_SEQ = i.ToString();
                                    //        SalEntity.Mode = "ADD";

                                    //        _model.SALDEFData.CAP_SALACT_INSUPDEL(SalEntity);
                                    //        i++;
                                    //    }
                                    //}



                                    if (Sel_CAOBO_Serlist.Count > 0)
                                    {
                                        Search_CAOBO_Entity.ID = New_CAID.ToString();
                                        Search_CAOBO_Entity.Rec_Type = "S";
                                        Search_CAOBO_Entity.Seq = "1";

                                        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                        foreach (CAOBOEntity Entity in Sel_CAOBO_Serlist)
                                        {
                                            Search_CAOBO_Entity.CLID = Entity.CLID.ToString();
                                            Search_CAOBO_Entity.Fam_Seq = Entity.Fam_Seq.ToString();
                                            Search_CAOBO_Entity.Seq = "1";
                                            Search_CAOBO_Entity.Rec_Type = "I";

                                            _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);
                                        }
                                    }
                                    else if (CAOBO_List.Count > 0)
                                    {
                                        Search_CAOBO_Entity.ID = New_CAID.ToString();
                                        Search_CAOBO_Entity.Rec_Type = "S";
                                        Search_CAOBO_Entity.Seq = "1";

                                        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                        foreach (CAOBOEntity Entity in CAOBO_List)
                                        {
                                            Search_CAOBO_Entity.CLID = Entity.CLID.ToString();
                                            Search_CAOBO_Entity.Fam_Seq = Entity.Fam_Seq.ToString();
                                            Search_CAOBO_Entity.Seq = "1";
                                            Search_CAOBO_Entity.Rec_Type = "I";

                                            _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);
                                        }
                                    }
                                    else
                                    {
                                        if (dtmembers.Rows.Count > 0)
                                        {
                                            if (Pass_CA_Entity.CA_OBF == "1")
                                            {
                                                Search_CAOBO_Entity.ID = New_CAID.ToString();
                                                Search_CAOBO_Entity.Rec_Type = "S";
                                                Search_CAOBO_Entity.Seq = "1";

                                                _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                                foreach (DataRow dr in dtmembers.Rows)
                                                {
                                                    if (dr["AppNo"].ToString().Substring(10, 1) == "A" && dr["AppStatus"].ToString() == "A" && dr["SNP_EXCLUDE"].ToString() == "N")
                                                    {
                                                        Search_CAOBO_Entity.CLID = dr["ClientID"].ToString();
                                                        Search_CAOBO_Entity.Fam_Seq = dr["RecFamSeq"].ToString();
                                                        Search_CAOBO_Entity.Seq = "1";
                                                        Search_CAOBO_Entity.Rec_Type = "I";

                                                        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);

                                                        break;
                                                    }
                                                }
                                            }
                                            else if (Pass_CA_Entity.CA_OBF == "2")
                                            {
                                                Search_CAOBO_Entity.ID = New_CAID.ToString();
                                                Search_CAOBO_Entity.Rec_Type = "S";
                                                Search_CAOBO_Entity.Seq = "1";

                                                _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                                foreach (DataRow dr in dtmembers.Rows)
                                                {
                                                    if (dr["AppStatus"].ToString() == "A" && dr["SNP_EXCLUDE"].ToString() == "N")
                                                    {
                                                        Search_CAOBO_Entity.CLID = dr["ClientID"].ToString();
                                                        Search_CAOBO_Entity.Fam_Seq = dr["RecFamSeq"].ToString();
                                                        Search_CAOBO_Entity.Seq = "1";
                                                        Search_CAOBO_Entity.Rec_Type = "I";

                                                        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);


                                                    }
                                                }
                                            }
                                        }
                                    }

                                    ////if (Ref_Grid.Rows.Count > 0)
                                    ////    Update_ReferrTo_in_CASEREFS();
                                    CASEACTEntity New_CA_Entity = new CASEACTEntity(true);
                                    if (SP_CA_Grid.Rows.Count > 0)
                                    {
                                        //New_CA_Entity = Pass_CA_Entity;
                                        foreach (DataGridViewRow dr in SP_CA_Grid.Rows)
                                        {
                                            if (dr.Cells["Sel_1"].Value.ToString() == true.ToString() && dr.Cells["SP2_CAMS_Code"].Value.ToString() != Pass_CA_Entity.ACT_Code)
                                            {
                                                New_CA_Entity.Agency = Pass_CA_Entity.Agency; New_CA_Entity.Dept = Pass_CA_Entity.Dept; New_CA_Entity.Program = Pass_CA_Entity.Program;
                                                New_CA_Entity.Year = Pass_CA_Entity.Year; New_CA_Entity.App_no = Pass_CA_Entity.App_no; New_CA_Entity.Service_plan = Pass_CA_Entity.Service_plan;
                                                New_CA_Entity.SPM_Seq = Pass_CA_Entity.SPM_Seq; New_CA_Entity.ACT_Date = Pass_CA_Entity.ACT_Date; New_CA_Entity.Caseworker = Pass_CA_Entity.Caseworker;
                                                New_CA_Entity.Site = Pass_CA_Entity.Site; New_CA_Entity.Fund1 = Pass_CA_Entity.Fund1; New_CA_Entity.Fund2 = Pass_CA_Entity.Fund2; New_CA_Entity.Fund3 = Pass_CA_Entity.Fund3;
                                                New_CA_Entity.Check_Date = Pass_CA_Entity.Check_Date; New_CA_Entity.Check_No = Pass_CA_Entity.Check_No;
                                                New_CA_Entity.Followup_By = Pass_CA_Entity.Followup_By; New_CA_Entity.Followup_Comp = Pass_CA_Entity.Followup_Comp;
                                                //New_CA_Entity.Rate = Pass_CA_Entity.Rate; New_CA_Entity.Vendor_No = Pass_CA_Entity.Vendor_No;  New_CA_Entity.Cost = Pass_CA_Entity.Cost;
                                                New_CA_Entity.Followup_On = Pass_CA_Entity.Followup_On; New_CA_Entity.Refer_Data = Pass_CA_Entity.Refer_Data;
                                                New_CA_Entity.Cust_Code1 = dr.Cells["Act_Ques1"].Value.ToString().Trim(); New_CA_Entity.Cust_Value1 = dr.Cells["Act_Resp1"].Value.ToString();
                                                New_CA_Entity.Cust_Code2 = dr.Cells["Act_Ques2"].Value.ToString(); New_CA_Entity.Cust_Value2 = dr.Cells["Act_Resp2"].Value.ToString();
                                                New_CA_Entity.Cust_Code3 = dr.Cells["Act_Ques3"].Value.ToString(); New_CA_Entity.Cust_Value3 = dr.Cells["Act_Resp3"].Value.ToString();
                                                New_CA_Entity.Cust_Code4 = dr.Cells["Act_Ques4"].Value.ToString(); New_CA_Entity.Cust_Value4 = dr.Cells["Act_Resp4"].Value.ToString();
                                                New_CA_Entity.Cust_Code5 = dr.Cells["Act_Ques5"].Value.ToString(); New_CA_Entity.Cust_Value5 = dr.Cells["Act_Resp5"].Value.ToString();
                                                New_CA_Entity.Bulk = "Q"; New_CA_Entity.Units = Pass_CA_Entity.Units; New_CA_Entity.VOUCHNO = Pass_CA_Entity.VOUCHNO;
                                                New_CA_Entity.Act_PROG = Pass_CA_Entity.Act_PROG; New_CA_Entity.Notes_Count = Pass_CA_Entity.Notes_Count;
                                                New_CA_Entity.Lsct_Operator = BaseForm.UserID;


                                                New_CA_Entity.ACT_Code = dr.Cells["SP2_CAMS_Code"].Value.ToString();
                                                New_CA_Entity.Branch = dr.Cells["SP2_Branch"].Value.ToString();
                                                New_CA_Entity.Group = dr.Cells["SP2_Group"].Value.ToString();
                                                New_CA_Entity.Curr_Grp = dr.Cells["SP2_Curr_Grp"].Value.ToString();
                                                New_CA_Entity.ACT_ID = "1";
                                                New_CA_Entity.ACT_Seq = "1";
                                                New_CA_Entity.Rec_Type = "I";
                                                Operatipn_Mode = "Insert";

                                                Sel_CAOBO_Serlist = new List<CAOBOEntity>();
                                                if (CAOBO_Ser_List.Count > 0)
                                                {
                                                    Sel_CAOBO_Serlist = CAOBO_Ser_List.FindAll(u => u.Type.Trim() == "CA" && u.Code.Trim() == New_CA_Entity.ACT_Code.Trim() && u.Branch.Trim() == New_CA_Entity.Branch.Trim() && u.Group.Trim() == New_CA_Entity.Group.Trim());
                                                }
                                                if (Sel_CAOBO_Serlist.Count > 0)
                                                    New_CA_Entity.CA_OBF = Sel_CAOBO_Serlist[0].BenefitFrom.ToString();
                                                else New_CA_Entity.CA_OBF = Pass_CA_Entity.CA_OBF;

                                                _model.SPAdminData.UpdateCASEACT(New_CA_Entity, Operatipn_Mode, out New_CAID, out New_CA_Seq, out Sql_SP_Result_Message);

                                                dr.Cells["SP2_CAMS_ID"].Value = New_CAID.ToString();
                                                dr.Cells["SP2_CA_Seq"].Value = New_CA_Seq.ToString();

                                                //if (SALData.Count > 0)
                                                //{
                                                //    SALData = SALData.OrderBy(u => u.SALACT_ID).ThenBy(u => u.SALACT_SALID).ThenBy(u => u.SALACT_Q_ID).ToList();

                                                //    SALACTEntity Search_Sal = new SALACTEntity(true);
                                                //    Search_Sal.SALACT_ID = New_CAID.ToString();
                                                //    Search_Sal.Mode = "DELETEALL";
                                                //    _model.SALDEFData.CAP_SALACT_INSUPDEL(Search_Sal);
                                                //    int i = 1;
                                                //    foreach (SALACTEntity SalEntity in SALData)
                                                //    {
                                                //        SalEntity.SALACT_ID = New_CAID.ToString();
                                                //        SalEntity.SALACT_SEQ = i.ToString();
                                                //        SalEntity.Mode = "ADD";

                                                //        _model.SALDEFData.CAP_SALACT_INSUPDEL(SalEntity);
                                                //        i++;
                                                //    }
                                                //}

                                                if (Sel_CAOBO_Serlist.Count > 0)
                                                {
                                                    Search_CAOBO_Entity = new CAOBOEntity();
                                                    Search_CAOBO_Entity.ID = New_CAID.ToString();
                                                    Search_CAOBO_Entity.Rec_Type = "S";
                                                    Search_CAOBO_Entity.Seq = "1";

                                                    _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                                    foreach (CAOBOEntity Entity in Sel_CAOBO_Serlist)
                                                    {
                                                        Search_CAOBO_Entity.CLID = Entity.CLID.ToString();
                                                        Search_CAOBO_Entity.Fam_Seq = Entity.Fam_Seq.ToString();
                                                        Search_CAOBO_Entity.Seq = "1";
                                                        Search_CAOBO_Entity.Rec_Type = "I";

                                                        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);
                                                    }
                                                }
                                                else if (CAOBO_List.Count > 0)
                                                {
                                                    Search_CAOBO_Entity.ID = New_CAID.ToString();
                                                    Search_CAOBO_Entity.Rec_Type = "S";
                                                    Search_CAOBO_Entity.Seq = "1";

                                                    _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                                    foreach (CAOBOEntity Entity in CAOBO_List)
                                                    {
                                                        Search_CAOBO_Entity.CLID = Entity.CLID.ToString();
                                                        Search_CAOBO_Entity.Fam_Seq = Entity.Fam_Seq.ToString();
                                                        Search_CAOBO_Entity.Seq = "1";
                                                        Search_CAOBO_Entity.Rec_Type = "I";

                                                        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);
                                                    }
                                                }
                                                else
                                                {
                                                    if (dtmembers.Rows.Count > 0)
                                                    {
                                                        if (Pass_CA_Entity.CA_OBF == "1")
                                                        {
                                                            Search_CAOBO_Entity.ID = New_CAID.ToString();
                                                            Search_CAOBO_Entity.Rec_Type = "S";
                                                            Search_CAOBO_Entity.Seq = "1";

                                                            _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                                            foreach (DataRow dr1 in dtmembers.Rows)
                                                            {
                                                                if (dr1["AppNo"].ToString().Substring(10, 1) == "A" && dr1["AppStatus"].ToString() == "A" && dr1["SNP_EXCLUDE"].ToString() == "N")
                                                                {
                                                                    Search_CAOBO_Entity.CLID = dr1["ClientID"].ToString();
                                                                    Search_CAOBO_Entity.Fam_Seq = dr1["RecFamSeq"].ToString();
                                                                    Search_CAOBO_Entity.Seq = "1";
                                                                    Search_CAOBO_Entity.Rec_Type = "I";

                                                                    _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);

                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        else if (Pass_CA_Entity.CA_OBF == "2")
                                                        {
                                                            Search_CAOBO_Entity.ID = New_CAID.ToString();
                                                            Search_CAOBO_Entity.Rec_Type = "S";
                                                            Search_CAOBO_Entity.Seq = "1";

                                                            _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                                            foreach (DataRow dr1 in dtmembers.Rows)
                                                            {
                                                                if (dr1["AppStatus"].ToString() == "A" && dr1["SNP_EXCLUDE"].ToString() == "N")
                                                                {
                                                                    Search_CAOBO_Entity.CLID = dr1["ClientID"].ToString();
                                                                    Search_CAOBO_Entity.Fam_Seq = dr1["RecFamSeq"].ToString();
                                                                    Search_CAOBO_Entity.Seq = "1";
                                                                    Search_CAOBO_Entity.Rec_Type = "I";

                                                                    _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);

                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }

                                    if (CASEMSList.Count > 0)
                                    {
                                        foreach (CASEMSEntity MSEntity in CASEMSList)
                                        {
                                            Pass_MS_Entity = new CASEMSEntity();
                                            if (!string.IsNullOrEmpty(MSEntity.MS_TrigCode.Trim()) && MSEntity.Rec_Type == "I")
                                            {
                                                Pass_MS_Entity.Agency = BaseForm.BaseAgency; Pass_MS_Entity.Dept = BaseForm.BaseDept; Pass_MS_Entity.Program = BaseForm.BaseProg;
                                                Pass_MS_Entity.Year = BaseForm.BaseYear; Pass_MS_Entity.App_no = BaseForm.BaseApplicationNo; Pass_MS_Entity.Service_plan = MSEntity.Service_plan;
                                                Pass_MS_Entity.SPM_Seq = Pass_CA_Entity.SPM_Seq; Pass_MS_Entity.Branch = MSEntity.Branch; Pass_MS_Entity.Group = MSEntity.Group; Pass_MS_Entity.MS_Code = MSEntity.MS_Code;
                                                Pass_MS_Entity.Rec_Type = MSEntity.Rec_Type; Pass_MS_Entity.Curr_Grp = MSEntity.Curr_Grp;

                                                Pass_MS_Entity.MS_Fund1 = Pass_CA_Entity.Fund1; Pass_MS_Entity.MS_Fund2 = Pass_CA_Entity.Fund2; Pass_MS_Entity.MS_Fund3 = Pass_CA_Entity.Fund3;

                                                if (BaseForm.BaseAgencyControlDetails.WorkerFUP.ToString().Trim().ToUpper() == "Y")
                                                {
                                                    Pass_MS_Entity.MS_FUP_Date = Pass_CA_Entity.Followup_On.Trim(); Pass_MS_Entity.MS_Comp_Date = Pass_CA_Entity.Followup_Comp.Trim();
                                                }

                                                Get_Latest_MS_data(MSEntity);

                                                Sel_CAOBO_Serlist = new List<CAOBOEntity>();
                                                if (CAOBO_Ser_List.Count > 0)
                                                {
                                                    Sel_CAOBO_Serlist = CAOBO_Ser_List.FindAll(u => u.Type.Trim() == "MS" && u.Code.Trim() == Pass_MS_Entity.MS_Code.Trim() && u.Branch.Trim() == Pass_MS_Entity.Branch.Trim() && u.Group.Trim() == Pass_MS_Entity.Group.Trim());
                                                }
                                                if (Sel_CAOBO_Serlist.Count > 0)
                                                    Pass_MS_Entity.OBF = Sel_CAOBO_Serlist[0].BenefitFrom.ToString();
                                                else Pass_MS_Entity.OBF = Pass_CA_Entity.CA_OBF;

                                                Pass_MS_Entity.Lsct_Operator = BaseForm.UserID;
                                                if (string.IsNullOrEmpty(Pass_MS_Entity.Bulk.Trim()))
                                                    Pass_MS_Entity.Bulk = "N";

                                                //if (CAMS_Desc == "Auto Post MS")
                                                //{
                                                //    this.DialogResult = DialogResult.OK;
                                                //    this.Close();
                                                //    return;
                                                //}

                                                int New_MS_ID = 0;

                                                if (_model.SPAdminData.UpdateCASEMS(Pass_MS_Entity, Operatipn_Mode, out New_MS_ID, out Sql_SP_Result_Message))
                                                {
                                                    Pass_MS_Entity.ID = New_MS_ID.ToString();
                                                    //if (Members_Grid.Rows.Count > 0)  // Yeswanth Sindhe
                                                    //    Update_MSOBO_Benefitig_Members(New_MS_ID);
                                                    if (Sel_CAOBO_Serlist.Count > 0)
                                                    {

                                                        Search_MSOBO_Entity.ID = New_MS_ID.ToString();
                                                        Search_MSOBO_Entity.Rec_Type = "D";

                                                        _model.SPAdminData.UpdateCASEMSOBO(Search_MSOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                                        foreach (CAOBOEntity Entity in Sel_CAOBO_Serlist)
                                                        {
                                                            Search_MSOBO_Entity.CLID = Entity.CLID.ToString();
                                                            Search_MSOBO_Entity.Fam_Seq = Entity.Fam_Seq.ToString();
                                                            Search_MSOBO_Entity.Seq = "1";
                                                            Search_MSOBO_Entity.Rec_Type = "I";

                                                            _model.SPAdminData.UpdateCASEMSOBO(Search_MSOBO_Entity, "Insert", out Sql_SP_Result_Message);
                                                        }
                                                    }
                                                    else if (CAOBO_List.Count > 0)
                                                    {

                                                        Search_MSOBO_Entity.ID = New_MS_ID.ToString();
                                                        Search_MSOBO_Entity.Rec_Type = "D";

                                                        _model.SPAdminData.UpdateCASEMSOBO(Search_MSOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                                        foreach (CAOBOEntity Entity in CAOBO_List)
                                                        {
                                                            Search_MSOBO_Entity.CLID = Entity.CLID.ToString();
                                                            Search_MSOBO_Entity.Fam_Seq = Entity.Fam_Seq.ToString();
                                                            Search_MSOBO_Entity.Seq = "1";
                                                            Search_MSOBO_Entity.Rec_Type = "I";

                                                            _model.SPAdminData.UpdateCASEMSOBO(Search_MSOBO_Entity, "Insert", out Sql_SP_Result_Message);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (dtmembers.Rows.Count > 0)
                                                        {
                                                            if (Pass_CA_Entity.CA_OBF == "1")
                                                            {
                                                                Search_MSOBO_Entity.ID = New_MS_ID.ToString();
                                                                Search_MSOBO_Entity.Rec_Type = "D";

                                                                _model.SPAdminData.UpdateCASEMSOBO(Search_MSOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                                                foreach (DataRow dr in dtmembers.Rows)
                                                                {
                                                                    if (dr["AppNo"].ToString().Substring(10, 1) == "A" && dr["AppStatus"].ToString() == "A" && dr["SNP_EXCLUDE"].ToString() == "N")
                                                                    {
                                                                        Search_MSOBO_Entity.CLID = dr["ClientID"].ToString();
                                                                        Search_MSOBO_Entity.Fam_Seq = dr["RecFamSeq"].ToString();
                                                                        Search_MSOBO_Entity.Seq = "1";
                                                                        Search_MSOBO_Entity.Rec_Type = "I";

                                                                        _model.SPAdminData.UpdateCASEMSOBO(Search_MSOBO_Entity, "Insert", out Sql_SP_Result_Message);

                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            else if (Pass_CA_Entity.CA_OBF == "2")
                                                            {
                                                                Search_MSOBO_Entity.ID = New_MS_ID.ToString();
                                                                Search_MSOBO_Entity.Rec_Type = "D";

                                                                _model.SPAdminData.UpdateCASEMSOBO(Search_MSOBO_Entity, "Delete", out Sql_SP_Result_Message);
                                                                foreach (DataRow dr in dtmembers.Rows)
                                                                {
                                                                    if (dr["AppStatus"].ToString() == "A" && dr["SNP_EXCLUDE"].ToString() == "N")
                                                                    {
                                                                        Search_MSOBO_Entity.CLID = dr["ClientID"].ToString();
                                                                        Search_MSOBO_Entity.Fam_Seq = dr["RecFamSeq"].ToString();
                                                                        Search_MSOBO_Entity.Seq = "1";
                                                                        Search_MSOBO_Entity.Rec_Type = "I";

                                                                        _model.SPAdminData.UpdateCASEMSOBO(Search_MSOBO_Entity, "Insert", out Sql_SP_Result_Message);


                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }



                                                }
                                            }



                                        }

                                    }

                                    if (Pass_CA_Entity.Rec_Type == "I")
                                    {
                                        if (BaseForm.BaseAgencyControlDetails.ProgressNotesSwitch.ToUpper() == "Y")
                                            MessageBox.Show("Posted Successfully \n Do you want to add Progress Notes?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Add_PROGNotes_For_CAMS);
                                        else
                                        {
                                            AlertBox.Show("Posted Successfully");

                                            this.DialogResult = DialogResult.OK;
                                            this.Close();
                                        }
                                    }
                                    else
                                    {
                                        AlertBox.Show("Posting Updated Successfully");//("Activity Posting Updated Successfully");


                                        this.DialogResult = DialogResult.OK;
                                        this.Close();
                                    }
                                }
                            }
                        }
                        else
                        {
                            AlertBox.Show("Please select Result", MessageBoxIcon.Error);
                        }
                    }




                }
                else if (Mode == "Edit")
                {
                    string Operatipn_Mode = "Insert";

                    if (Pass_CA_Entity.Rec_Type == "U")
                        Operatipn_Mode = "Update";

                    Pass_CA_Entity.Lsct_Operator = BaseForm.UserID;

                    int New_CAID = 1, New_CA_Seq = 1;
                    if (!string.IsNullOrEmpty(Pass_CA_Entity.ACT_ID) && Pass_CA_Entity.Rec_Type == "U")
                        New_CAID = int.Parse(Pass_CA_Entity.ACT_ID);
                    else
                        Pass_CA_Entity.ACT_ID = "1";

                    Pass_CA_Entity.CA_OBF = ((Captain.Common.Utilities.ListItem)cmb_Benefit.SelectedItem).Value.ToString();
                    //if (Sel_CAOBO_Serlist.Count > 0)
                    //    Pass_CA_Entity.CA_OBF = Sel_CAOBO_Serlist[0].BenefitFrom.ToString();

                    //if (Pass_CA_Entity.CA_OBF == "3" && CAOBO_List.Count == 0)
                    //{
                    //    MessageBox.Show("Members should be selected", "CAP Systems");
                    //}


                    if (CAMS_Desc.Contains("Auto Post – "))
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        return;
                    }

                    if (_model.SPAdminData.UpdateCASEACT2(Pass_CA_Entity, Operatipn_Mode, out New_CAID, out New_CA_Seq, out Sql_SP_Result_Message))
                    {
                        Pass_CA_Entity.ACT_ID = New_CAID.ToString();
                        Pass_CA_Entity.ACT_Seq = Current_CA_Seq = New_CA_Seq.ToString();
                        if (CA_Members_Grid.Rows.Count > 0)  // Yeswanth Sindhe
                            Update_CAOBO_Benefitig_Members(New_CAID);

                        if (SALData.Count > 0)
                        {
                            SALACTEntity Search_Sal = new SALACTEntity();
                            Search_Sal.SALACT_ID = New_CAID.ToString();
                            Search_Sal.Mode = "DELETEALL";
                            _model.SALDEFData.CAP_SALACT_INSUPDEL(Search_Sal);

                            int i = 1;
                            foreach (SALACTEntity SalEntity in SALData)
                            {
                                SalEntity.SALACT_ID = New_CAID.ToString();
                                SalEntity.SALACT_SEQ = i.ToString();
                                SalEntity.Mode = "ADD";

                                _model.SALDEFData.CAP_SALACT_INSUPDEL(SalEntity);
                                i++;
                            }
                        }

                        //if (Ref_Grid.Rows.Count > 0)
                        //    Update_ReferrTo_in_CASEREFS();

                        if (Pass_CA_Entity.Rec_Type == "I")
                        {
                            if (BaseForm.BaseAgencyControlDetails.ProgressNotesSwitch.ToUpper() == "Y")
                                MessageBox.Show("Posted Successfully \n Do you want to add Progress Notes?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Add_PROGNotes_For_CAMS);
                            else
                            {
                                AlertBox.Show("Posted Successfully");

                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                        }
                        else
                        {
                            AlertBox.Show("Posting Updated Successfully");//("Activity Posting Updated Successfully");


                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    else
                        AlertBox.Show("Unable to Save Activity \n" + "Reason : " + Sql_SP_Result_Message, MessageBoxIcon.Warning);
                }


            }
        }

        private void MultipleSave(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                List<CAOBOEntity> Sel_CAOBO_Serlist = new List<CAOBOEntity>();
                if (CAOBO_Ser_List.Count > 0)
                {
                    Sel_CAOBO_Serlist = CAOBO_Ser_List.FindAll(u => u.Type.Trim() == "CA" && u.Code.Trim() == Pass_CA_Entity.ACT_Code.Trim() && u.Branch.Trim() == Pass_CA_Entity.Branch.Trim() && u.Group.Trim() == Pass_CA_Entity.Group.Trim());
                }


                if (!CAMS_Desc.Contains("Auto Post – "))
                {
                    Pass_CA_Entity.CA_OBF = ((Captain.Common.Utilities.ListItem)cmb_CA_Benefit.SelectedItem).Value.ToString();
                }

                if (Sel_CAOBO_Serlist.Count > 0)
                    Pass_CA_Entity.CA_OBF = Sel_CAOBO_Serlist[0].BenefitFrom.ToString();

                bool IsCAOBO = false;
                if (CAMS_Desc.Contains("Auto Post – "))
                {
                    Pass_CA_Entity.CA_OBF = ((Captain.Common.Utilities.ListItem)cmb_Benefit.SelectedItem).Value.ToString();

                    foreach (DataGridViewRow dr in CA_Members_Grid.Rows)
                    {
                        if (dr.Cells["CA_Sel"].Value.ToString() == true.ToString())
                        {
                            IsCAOBO = true; break;
                        }
                    }

                }
                else IsCAOBO = true;

                if ((Pass_CA_Entity.CA_OBF == "3" && CAOBO_List.Count == 0) || (Pass_CA_Entity.CA_OBF == "3" && !IsCAOBO))
                {
                    AlertBox.Show("Members should be selected", MessageBoxIcon.Warning);
                }
                else
                {
                    string Operatipn_Mode = "Insert";

                    if (Pass_CA_Entity.Rec_Type == "U")
                        Operatipn_Mode = "Update";

                    Pass_CA_Entity.Lsct_Operator = BaseForm.UserID;

                    int New_CAID = 1, New_CA_Seq = 1;
                    if (!string.IsNullOrEmpty(Pass_CA_Entity.ACT_ID) && Pass_CA_Entity.Rec_Type == "U")
                        New_CAID = int.Parse(Pass_CA_Entity.ACT_ID);
                    else
                        Pass_CA_Entity.ACT_ID = "1";



                    if (CAMS_Desc.Contains("Auto Post – "))
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        return;
                    }

                    List<CAOBOEntity> SelOBOEntity = new List<CAOBOEntity>();

                    if (_model.SPAdminData.UpdateCASEACT2(Pass_CA_Entity, Operatipn_Mode, out New_CAID, out New_CA_Seq, out Sql_SP_Result_Message))
                    {
                        Pass_CA_Entity.ACT_ID = New_CAID.ToString();
                        Pass_CA_Entity.ACT_Seq = Current_CA_Seq = New_CA_Seq.ToString();


                        if (CA_Members_Grid.Rows.Count > 0)
                        {
                            Search_CAOBO_Entity.ID = New_CAID.ToString();
                            Search_CAOBO_Entity.Rec_Type = "S";
                            Search_CAOBO_Entity.Seq = "1";

                            _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);

                            foreach (DataGridViewRow dr in CA_Members_Grid.Rows)
                            {
                                Search_CAOBO_Entity.ID = Pass_CA_Entity.ACT_ID;
                                if (Pass_CA_Entity.Rec_Type == "I")
                                    Search_CAOBO_Entity.ID = New_CAID.ToString();

                                Search_CAOBO_Entity.CLID = dr.Cells["CA_CLID"].Value.ToString();
                                Search_CAOBO_Entity.Fam_Seq = dr.Cells["CA_Mem_Seq"].Value.ToString();
                                Search_CAOBO_Entity.Seq = "1";

                                if (dr.Cells["CA_Sel"].Value.ToString() == true.ToString())
                                {
                                    Search_CAOBO_Entity.Rec_Type = "I";

                                    _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);

                                    SelOBOEntity.Add(Search_CAOBO_Entity);
                                }

                            }
                        }


                        IsSaveResult(SelOBOEntity);
                    }

                }
            }
        }


        int TemplateCount = 1;

        private void IsSaveResult(List<CAOBOEntity> OBOEntity)
        {
            if (CA_Template_List.Count > 0)
            {
                //CA_Template_List = CA_Template_List.Where(w => w.IsTemplate == "T").Select(s => { s.IsTemplate = ""; return s; }).ToList();
                //CA_Template_List.Where(w => w.IsTemplate == "T").ToList().ForEach(s => s.IsTemplate = "");
                bool Template = false;
                foreach (var mc in CA_Template_List.Where(x => x.IsTemplate == "T"))
                    Template = true;

                int i = 0, SelCnt = 0;
                foreach (CASEACTEntity CAEntity in CA_Template_List)
                {
                    if (CAEntity.ACT_Code.Trim() == Pass_CA_Entity.ACT_Code.Trim())
                    {
                        SelCnt = i;
                        CA_Template_List[i] = Pass_CA_Entity;
                        CAEntity.ACT_ID = Pass_CA_Entity.ACT_ID;
                        CAEntity.ACT_Seq = Pass_CA_Entity.ACT_Seq;
                        if (!Template) CAEntity.IsTemplate = "T";
                        CAEntity.IsSave = "Y"; //CAEntity.IsTemplate = "T";
                        break;

                    }
                    else
                    {
                        //if (CAEntity.IsSave != "Y")
                        //    CAEntity.IsTemplate = "";
                    }
                    i++;
                }

                CASEACTEntity SelCAEntiy = CA_Template_List.Find(u => u.IsSave != "Y");

                if (SelCAEntiy != null)
                {
                    QuickPostStatusForm frm = new QuickPostStatusForm(BaseForm, Privileges, CA_Template_List, MS_Template_List, Pass_CA_Entity, OBOEntity, Hierarchy);
                    frm.FormClosed += new FormClosedEventHandler(QuickPostStatusForm_Closed);
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.ShowDialog();
                }
                else
                {
                    if (Pass_CA_Entity.Rec_Type == "I")
                    {
                        if (BaseForm.BaseAgencyControlDetails.ProgressNotesSwitch.ToUpper() == "Y")
                            MessageBox.Show("Posted Successfully \n Do you want to add Progress Notes?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Add_PROGNotes_For_CAMS);
                        else
                        {
                            AlertBox.Show("Posted Successfully");

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    else
                    {
                        AlertBox.Show("Posting Updated Successfully");//("Activity Posting Updated Successfully");


                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }



                //if (SelCAEntiy != null)
                //{
                //    string CAMS_Desc = string.Empty;
                //    CAMS_Desc = SP_CAMS_Details.Find(u => u.Type1 == "CA" && u.CamCd.Trim() == SelCAEntiy.ACT_Code.Trim()).CAMS_Desc.Trim();


                //    //Pass_CA_Entity.ACT_Code = SelCAEntiy.ACT_Code; Pass_CA_Entity.Group = SelCAEntiy.Group; Pass_CA_Entity.Curr_Grp = SelCAEntiy.Curr_Grp;

                //    MessageBox.Show("Do you want Proceed this with existing Service "+ this.Text, Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: MultipleSave);

                //    pnlMSResult.Visible = false;

                //    TemplateCount++;

                //    this.Text = CAMS_Desc;
                //    Pass_CA_Entity = SelCAEntiy;

                //    lblSel.Text = TemplateCount+" of " + (CA_Template_List.Count+MS_Template_List.Count);

                //    Get_Latest_Activity_data();

                //}
                //else if(MS_Template_List.Count>0)
                //{
                //    int j = 0, SelMSCnt = 0;
                //    foreach (CASEMSEntity MSEntity in MS_Template_List)
                //    {
                //        if (MSEntity.MS_Code.Trim() == Pass_MS_Entity.MS_Code.Trim())
                //        {
                //            SelMSCnt = i;
                //            MS_Template_List[j] = Pass_MS_Entity;
                //            MSEntity.ID = Pass_MS_Entity.ID;
                //            //MSEntity.ACT_Seq = Pass_CA_Entity.ACT_Seq;
                //            MSEntity.IsSave = "Y"; break;
                //        }
                //    }

                //    pnlMSResult.Visible = true;

                //    CASEMSEntity SelMSEntiy = MS_Template_List.Find(u => u.IsSave != "Y");
                //    if (SelMSEntiy != null)
                //    {
                //        MessageBox.Show("Do you want Proceed this with existing Service " + this.Text, Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: IsMSSaveRecord);

                //        TemplateCount++;

                //        this.Text = SelMSEntiy.MSDesc.Trim();
                //        Pass_MS_Entity = SelMSEntiy;

                //        lblSel.Text = TemplateCount + " of " + (CA_Template_List.Count+MS_Template_List.Count);

                //        Get_Latest_MS_data(SelMSEntiy);
                //    }
                //}
                //else
                //{
                //    if (Pass_CA_Entity.Rec_Type == "I")
                //    {
                //        if (BaseForm.BaseAgencyControlDetails.ProgressNotesSwitch.ToUpper() == "Y")
                //            MessageBox.Show("Activity Posting Successful \n Do you want to add Progress Notes?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Add_PROGNotes_For_CAMS);
                //        else
                //        {
                //            MessageBox.Show("Activity Posting Successful", "CAP Systems");

                //            this.DialogResult = DialogResult.OK;
                //            this.Close();
                //        }
                //    }
                //    else
                //    {
                //        MessageBox.Show("Activity Posting Updated Successfully", "CAP Systems");


                //        this.DialogResult = DialogResult.OK;
                //        this.Close();
                //    }
                //}
            }
        }

        private void IsMSSaveRecord(DialogResult dialogResult)
        {
            //Pass_MS_Entity = MSEntity;
            if (dialogResult == DialogResult.Yes)
            {
                if (Pass_CA_Entity.Rec_Type == "I")
                {
                    Pass_MS_Entity.Agency = BaseForm.BaseAgency; Pass_MS_Entity.Dept = BaseForm.BaseDept; Pass_MS_Entity.Program = BaseForm.BaseProg;
                    Pass_MS_Entity.Year = BaseForm.BaseYear; Pass_MS_Entity.App_no = BaseForm.BaseApplicationNo; Pass_MS_Entity.Service_plan = Pass_CA_Entity.Service_plan;
                    Pass_MS_Entity.SPM_Seq = Pass_CA_Entity.SPM_Seq; //Pass_MS_Entity.Branch = MSEntity.Branch; Pass_MS_Entity.Group = MSEntity.Group; Pass_MS_Entity.MS_Code = MSEntity.MS_Code;
                    //Pass_MS_Entity.Rec_Type = MSEntity.Rec_Type; Pass_MS_Entity.Curr_Grp = MSEntity.Curr_Grp;
                    Pass_MS_Entity.MS_Fund1 = Pass_CA_Entity.Fund1; Pass_MS_Entity.MS_Fund2 = Pass_CA_Entity.Fund2; Pass_MS_Entity.MS_Fund3 = Pass_CA_Entity.Fund3;

                    if (BaseForm.BaseAgencyControlDetails.WorkerFUP.ToString().Trim().ToUpper() == "Y")
                    {
                        Pass_MS_Entity.MS_FUP_Date = Pass_CA_Entity.Followup_On.Trim(); Pass_MS_Entity.MS_Comp_Date = Pass_CA_Entity.Followup_Comp.Trim();
                    }

                    //Get_Latest_MS_data(MSEntity);

                    //Sel_CAOBO_Serlist = new List<CAOBOEntity>();
                    //if (CAOBO_Ser_List.Count > 0)
                    //{
                    //    Sel_CAOBO_Serlist = CAOBO_Ser_List.FindAll(u => u.Type.Trim() == "MS" && u.Code.Trim() == Pass_MS_Entity.MS_Code.Trim() && u.Branch.Trim() == Pass_MS_Entity.Branch.Trim() && u.Group.Trim() == Pass_MS_Entity.Group.Trim());
                    //}
                    //if (Sel_CAOBO_Serlist.Count > 0)
                    //    Pass_MS_Entity.OBF = Sel_CAOBO_Serlist[0].BenefitFrom.ToString();
                    //else Pass_MS_Entity.OBF = Pass_CA_Entity.CA_OBF;

                    Pass_MS_Entity.Lsct_Operator = BaseForm.UserID;
                    if (string.IsNullOrEmpty(Pass_MS_Entity.Bulk.Trim()))
                        Pass_MS_Entity.Bulk = "N";

                    string Operatipn_Mode = "Insert";

                    if (Pass_CA_Entity.Rec_Type == "U")
                        Operatipn_Mode = "Update";

                    List<CAOBOEntity> SelOBOEntity = new List<CAOBOEntity>();
                    int New_MS_ID = 0;

                    if (_model.SPAdminData.UpdateCASEMS(Pass_MS_Entity, Operatipn_Mode, out New_MS_ID, out Sql_SP_Result_Message))
                    {
                        Pass_MS_Entity.ID = New_MS_ID.ToString();

                        if (CA_Members_Grid.Rows.Count > 0)
                        {
                            Search_MSOBO_Entity.ID = New_MS_ID.ToString();
                            Search_MSOBO_Entity.Rec_Type = "D";

                            _model.SPAdminData.UpdateCASEMSOBO(Search_MSOBO_Entity, "Delete", out Sql_SP_Result_Message);

                            foreach (DataGridViewRow dr in CA_Members_Grid.Rows)
                            {
                                Search_MSOBO_Entity.ID = New_MS_ID.ToString();
                                if (Pass_CA_Entity.Rec_Type == "I")
                                    Search_MSOBO_Entity.ID = New_MS_ID.ToString();

                                Search_MSOBO_Entity.CLID = dr.Cells["CA_CLID"].Value.ToString();
                                Search_MSOBO_Entity.Fam_Seq = dr.Cells["CA_Mem_Seq"].Value.ToString();
                                Search_MSOBO_Entity.Seq = "1";
                                Search_MSOBO_Entity.Rec_Type = "I";

                                if (dr.Cells["CA_Sel"].Value.ToString() == true.ToString())
                                {
                                    Search_CAOBO_Entity.Rec_Type = "I";

                                    _model.SPAdminData.UpdateCASEMSOBO(Search_MSOBO_Entity, "Insert", out Sql_SP_Result_Message);
                                    SelOBOEntity.Add(Search_CAOBO_Entity);
                                }

                            }
                        }
                        IsSaveResult(SelOBOEntity);

                    }
                }
            }

        }

        private void QuickPostStatusForm_Closed(object sender, FormClosedEventArgs e)
        {
            QuickPostStatusForm form = sender as QuickPostStatusForm;
            if (form.DialogResult == DialogResult.OK)
            {
                CA_Template_List = form.GetLatestCAs();

                CASEACTEntity SelCAEntiy = CA_Template_List.Find(u => u.IsSave != "Y");
                if (SelCAEntiy != null)
                {
                    string CAMS_Desc = string.Empty;
                    CAMS_Desc = SP_CAMS_Details.Find(u => u.Type1 == "CA" && u.CamCd.Trim() == SelCAEntiy.ACT_Code.Trim()).CAMS_Desc.Trim();

                    TemplateCount = CA_Template_List.FindAll(u => u.IsSave == "Y").Count;

                    if (TemplateCount > 0) TemplateCount = TemplateCount + 1;

                    this.Text = CAMS_Desc;
                    Pass_CA_Entity = SelCAEntiy;

                    lblSel.Text = TemplateCount + " of " + (CA_Template_List.Count + MS_Template_List.Count);

                    Get_Latest_Activity_data();

                }
                else
                {
                    if (Pass_CA_Entity.Rec_Type == "I")
                    {
                        if (BaseForm.BaseAgencyControlDetails.ProgressNotesSwitch.ToUpper() == "Y")
                            MessageBox.Show("Posted Successfully \n Do you want to add Progress Notes?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Add_PROGNotes_For_CAMS);
                        else
                        {
                            AlertBox.Show("Posted Successfully");

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    else
                    {
                        AlertBox.Show("Posting Updated Successfully");//("Activity Posting Updated Successfully");


                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }

                //Pass_CA_Entity.ACT_Code = SelCAEntiy.ACT_Code; Pass_CA_Entity.Group = SelCAEntiy.Group; Pass_CA_Entity.Curr_Grp = SelCAEntiy.Curr_Grp;
            }
            else if (form.DialogResult == DialogResult.Cancel)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (form.DialogResult == DialogResult.Yes)
            {
                CA_Template_List = form.GetLatestCAs();

                CASEACTEntity SelCAEntiy = form.GetSelectedCA();
                if (SelCAEntiy != null)
                {
                    string CAMS_Desc = string.Empty;
                    CAMS_Desc = SP_CAMS_Details.Find(u => u.Type1 == "CA" && u.CamCd.Trim() == SelCAEntiy.ACT_Code.Trim()).CAMS_Desc.Trim();

                    TemplateCount = CA_Template_List.FindAll(u => u.IsSave == "Y").Count;

                    if (TemplateCount > 0) TemplateCount = TemplateCount + 1;

                    this.Text = CAMS_Desc;
                    Pass_CA_Entity = SelCAEntiy;

                    lblSel.Text = TemplateCount + " of " + (CA_Template_List.Count + MS_Template_List.Count);

                    Get_Latest_Activity_data();

                }
                else
                {
                    if (Pass_CA_Entity.Rec_Type == "I")
                    {
                        if (BaseForm.BaseAgencyControlDetails.ProgressNotesSwitch.ToUpper() == "Y")
                            MessageBox.Show("Posted Successfully \n Do you want to add Progress Notes?", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: Add_PROGNotes_For_CAMS);
                        else
                        {
                            AlertBox.Show("Posted Successfully");

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    else
                    {
                        AlertBox.Show("Posting Updated Successfully");//("Activity Posting Updated Successfully");


                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }

                //Pass_CA_Entity.ACT_Code = SelCAEntiy.ACT_Code; Pass_CA_Entity.Group = SelCAEntiy.Group; Pass_CA_Entity.Curr_Grp = SelCAEntiy.Curr_Grp;
            }
        }

        private void Update_CAOBO_Benefitig_Members(int CA_ID)
        {
            if (CA_Members_Grid.Rows.Count > 0)
            {
                Search_CAOBO_Entity.ID = Pass_CA_Entity.ACT_ID;
                Search_CAOBO_Entity.Rec_Type = "S";
                _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);

                foreach (DataGridViewRow dr in CA_Members_Grid.Rows)
                {
                    Search_CAOBO_Entity.ID = Pass_CA_Entity.ACT_ID;
                    if (Pass_CA_Entity.Rec_Type == "I")
                        Search_CAOBO_Entity.ID = CA_ID.ToString();

                    Search_CAOBO_Entity.CLID = dr.Cells["CA_CLID"].Value.ToString();
                    Search_CAOBO_Entity.Fam_Seq = dr.Cells["CA_Mem_Seq"].Value.ToString();
                    Search_CAOBO_Entity.Seq = "1";

                    if (dr.Cells["CA_Sel"].Value.ToString() == true.ToString())
                    {
                        Search_CAOBO_Entity.Rec_Type = "I";

                        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Insert", out Sql_SP_Result_Message);
                    }
                    //else
                    //{
                    //    if (dr.Cells["Is_CAOBO_Rec"].Value.ToString() == "Y")
                    //    {
                    //        Search_CAOBO_Entity.Rec_Type = "D";

                    //        _model.SPAdminData.UpdateCAOBO(Search_CAOBO_Entity, "Delete", out Sql_SP_Result_Message);
                    //    }
                    //}
                }
            }
        }

        private void Add_PROGNotes_For_CAMS(DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.Yes)
            {
                switch (CAMS_FLG)
                {
                    case "CA": CASE4006_CAMSForm_ToolClick(Tools, new ToolClickEventArgs(Tools["tlCaseNotes"])); break;        //new ToolClickEventArgs(Tools["pbNotes"])
                        //case "MS": Pb_Notes_Click(Pb_MS_Notes, EventArgs.Empty); break;
                }
                Get_PROG_Notes_Status();
            }
            else
            {
                AlertBox.Show("Posted Successfully");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public CASEACTEntity Get_CA_AutoPost_Details()
        {
            return Pass_CA_Entity;
        }

        public List<CAOBOEntity> Get_CAOBO_AutoPost_Details()
        {
            List<CAOBOEntity> OBO_List = new List<CAOBOEntity>();
            CAOBOEntity Tmp = new CAOBOEntity();
            foreach (DataGridViewRow dr in CA_Members_Grid.Rows)
            {
                if (dr.Cells["CA_Sel"].Value.ToString() == true.ToString())
                {
                    Tmp.ID = Pass_CA_Entity.ACT_ID;
                    Tmp.CLID = dr.Cells["CA_CLID"].Value.ToString();
                    Tmp.Fam_Seq = dr.Cells["CA_Mem_Seq"].Value.ToString();
                    Tmp.Seq = "1";

                    OBO_List.Add(new CAOBOEntity(Tmp));
                }
            }

            return OBO_List;
        }

        private void btnCAOBO_Click(object sender, EventArgs e)
        {
            MembersGridForm PostCA_Form;
            PostCA_Form = new MembersGridForm(BaseForm, Hierarchy, Year, CAMS_Desc, Pass_CA_Entity, Privileges, CA_Template_List, ((Captain.Common.Utilities.ListItem)cmb_CA_Benefit.SelectedItem).Value.ToString(), CAOBO_List);   // 08022012
            PostCA_Form.FormClosed += new FormClosedEventHandler(Add_Edit_MembersForm_Closed);
            PostCA_Form.StartPosition = FormStartPosition.CenterScreen;
            PostCA_Form.ShowDialog();
        }

        private void Add_Edit_MembersForm_Closed(object sender, FormClosedEventArgs e)
        {
            MembersGridForm form = sender as MembersGridForm;
            if (form.DialogResult == DialogResult.OK)
            {
                CAOBO_List = new List<CAOBOEntity>();
                CAOBO_List = form.GetMemberRecords();

                //if (CAOBO_Ser_List.Count == 0)
                //    CAOBO_Ser_List = CAOBO_List;
                //else
                //    Add_MembersForm_Closed(sender, e);


            }
        }

        public List<CAOBOEntity> CAOBO_Ser_List = new List<CAOBOEntity>();

        private void cmb_Benefit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)cmb_Benefit.SelectedItem).Text.ToString()))
                Set_Members_CA_Grid_As_Benefit_Change(true, ((Captain.Common.Utilities.ListItem)cmb_Benefit.SelectedItem).Value.ToString());
        }

        private void cmb_CA_Benefit_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)cmb_CA_Benefit.SelectedItem).Text.ToString()))
            //    Set_Members_CA_Grid_As_Benefit_Change(true, ((Captain.Common.Utilities.ListItem)cmb_CA_Benefit.SelectedItem).Value.ToString());
        }

        private void Set_Members_CA_Grid_As_Benefit_Change(bool Set_Mem_On_Combo, string OBF_Type)
        {
            if (CA_Members_Grid.Rows.Count > 0)
            {
                string Mem_Status = "M";
                this.CA_Sel.ReadOnly = true;
                switch (((Captain.Common.Utilities.ListItem)cmb_Benefit.SelectedItem).Value.ToString())
                {
                    case "1": Mem_Status = "A"; break;
                    case "2": Mem_Status = "M"; break;
                    case "3": Mem_Status = "Y"; this.CA_Sel.ReadOnly = false; break;
                }


                if (Set_Mem_On_Combo)//(Pass_MS_Entity.Rec_Type == "I" && !Set_Mem_From_OBO)
                {
                    int Row_index = 0;
                    foreach (DataGridViewRow dr in CA_Members_Grid.Rows)
                    {
                        switch (Mem_Status)
                        {
                            case "A":
                                if (dr.Cells["CA_AppSw"].Value.ToString() == Mem_Status)
                                {
                                    if (dr.Cells["CA_Active_Sw"].Value.ToString() == "A" && dr.Cells["CA_Exclude_Sw"].Value.ToString() == "N")
                                        dr.Cells["CA_Sel"].Value = true;
                                    // Members_Grid.Rows[Row_index].DefaultCellStyle.ForeColor = Color.Blue;
                                }
                                else
                                    dr.Cells["CA_Sel"].Value = false;
                                break;
                            case "M":
                                if (dr.Cells["CA_Active_Sw"].Value.ToString() == "A" && dr.Cells["CA_Exclude_Sw"].Value.ToString() == "N")
                                    dr.Cells["CA_Sel"].Value = true;
                                break;
                            default:
                                //if (dr.Cells["CA_Active_Sw"].Value.ToString() == "A" && dr.Cells["CA_Exclude_Sw"].Value.ToString() == "N")
                                //    dr.Cells["CA_Sel"].Value = true;
                                dr.Cells["CA_Sel"].Value = false;
                                break;
                        }
                        Row_index++;
                    }
                }
                else
                    Set_Members_FromCAOBO();
            }

        }

        private void SP_CA_Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (SP_CA_Grid.Rows.Count > 0)
            {
                FillMSgrid(SP_Header_Rec.Code.Trim(), Pass_CA_Entity.Branch);
            }
        }

        private void Set_Members_FromCAOBO()
        {
            if (CAOBO_List.Count > 0)
            {
                foreach (CAOBOEntity Entity in CAOBO_List)
                {
                    foreach (DataGridViewRow dr in CA_Members_Grid.Rows)
                    {
                        if (Entity.CLID == dr.Cells["CA_CLID"].Value.ToString() &&
                            Entity.Fam_Seq == dr.Cells["CA_Mem_Seq"].Value.ToString())
                        {
                            dr.Cells["CA_Sel"].Value = true;
                            dr.Cells["Is_CAOBO_Rec"].Value = "Y";
                            break;
                        }
                        //else
                        //{
                        //    dr.Cells["MS_Sel"].Value = false;
                        //    dr.Cells["Is_OBO_Rec"].Value = "N";
                        //    break;
                        //}
                    }
                }
            }
        }

        private void Txt_Cost3_TextChanged(object sender, EventArgs e)
        {
            //float V1 = 0, V2 = 0, V3 = 0;
            //if (!string.IsNullOrEmpty(Txt_Cost.Text.Trim())) V1 = float.Parse(Txt_Cost.Text.Trim());
            //if (!string.IsNullOrEmpty(Txt_Cost2.Text.Trim())) V2 = float.Parse(Txt_Cost2.Text.Trim());
            //if (!string.IsNullOrEmpty(Txt_Cost3.Text.Trim())) V3 = float.Parse(Txt_Cost3.Text.Trim());

            //if((V1+V2+V3)>0)
            //    txtTotAmount.Text = (V1 + V2 + V3).ToString("0.00");


        }

        private void FillMSgrid(string sp_Code, string Branch_Code)
        {
            //string CAMSDesc = SP_CA_Grid.SelectedRows[0].Cells["SP2_Desc"].Value.ToString();
            List<CASESP2Entity> SelMS_Details = new List<CASESP2Entity>();
            if (Mode == "Add")
                SelMS_Details = SP_CAMS_Details.FindAll(u => u.Type1.Trim().Equals("MS") && u.SP2_CAMS_Active.Equals("A") && u.Branch.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Branch"].Value.ToString().Trim()) && u.Curr_Grp.ToString() == (SP_CA_Grid.SelectedRows[0].Cells["SP2_Curr_Grp"].Value.ToString().Trim()));
            else
                SelMS_Details = SP_CAMS_Details.FindAll(u => u.Type1.Trim().Equals("MS") && u.SP2_CAMS_Active.Equals("A") && u.Branch.Trim().Equals(Pass_CA_Entity.Branch.Trim()) && u.Curr_Grp.ToString() == (Pass_CA_Entity.Curr_Grp.Trim()));

            string Result = string.Empty; bool ISMSSel = false;




            gvMSgrid.Rows.Clear();
            if (Branch_Code != "9")
            {
                if (SelMS_Details.Count > 0)
                {
                    int MSrowIndex = 0;
                    foreach (CASESP2Entity Entity in SelMS_Details)
                    {
                        if (CASEMSList.Count > 0)
                        {
                            ISMSSel = false;
                            foreach (CASEMSEntity MSEntity in CASEMSList)
                            {
                                Result = string.Empty;
                                if (MSEntity.MS_Code.Trim() == Entity.CamCd.ToString().Trim() && MSEntity.Branch.Trim() == Entity.Branch.Trim() && MSEntity.Curr_Grp.Trim() == Entity.Curr_Grp.ToString().Trim())
                                {
                                    if (MSEntity.Rec_Type == "I") ISMSSel = true;
                                    if (!string.IsNullOrEmpty(MSEntity.MS_TrigCode.Trim()))
                                    {
                                        Result = MSEntity.Result.Trim(); break;
                                    }

                                }
                            }

                            //if(!string.IsNullOrEmpty(CASEMSList.Find(u => u.MS_Code.Equals(Entity.CamCd.ToString().Trim()) && u.Branch.Equals(Entity.Branch) && u.Curr_Grp.Equals(Entity.Curr_Grp.ToString().Trim())).MS_TrigCode.ToString().Trim()))
                            //    Result = CASEMSList.Find(u => u.MS_Code.Equals(Entity.CamCd.ToString().Trim()) && u.Branch.Equals(Entity.Branch) && u.Curr_Grp.Equals(Entity.Curr_Grp.ToString().Trim())).MS_TrigCode.ToString();
                        }
                        MSrowIndex = gvMSgrid.Rows.Add(ISMSSel, Entity.CamCd.Trim(), Entity.CAMS_Desc, Result, Entity.Row, Entity.Type1, "C", Entity.Branch, Entity.Orig_Grp, "N", null, null, Entity.CAMS_Desc, Pass_CA_Entity.Year, Entity.CAMS_Active, null, Entity.Curr_Grp, Entity.Branch, null, string.Empty, Entity.SP2_OBF);
                        if(ISMSSel)
                        {
                            this.gvMSgrid.CellValueChanged -= new Wisej.Web.DataGridViewCellEventHandler(this.gvMSgrid_CellValueChanged);

                            gvMSgrid.Rows[MSrowIndex].Cells["gvbtnMSBenefit"].Value = Img_btn;
                            gvMSgrid.Rows[MSrowIndex].Cells["gvMsResult"].ReadOnly = false;

                            this.gvMSgrid.CellValueChanged += new Wisej.Web.DataGridViewCellEventHandler(this.gvMSgrid_CellValueChanged);
                        }
                    }
                }
            }
            else
            {
                Fill_Additional_CAMS_Details(sp_Code);

                if (ADD_CAMA_Details.Count > 0)
                {
                    List<CASESPM2Entity> SelAddMSDetails = new List<CASESPM2Entity>();
                    if (Mode == "Add")
                        SelAddMSDetails = ADD_CAMA_Details.FindAll(u => u.Type1.Trim().Equals("MS") && u.Branch.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Branch"].Value.ToString().Trim()) && u.Curr_Group.ToString() == (SP_CA_Grid.SelectedRows[0].Cells["SP2_Curr_Grp"].Value.ToString().Trim()));
                    else
                        SelAddMSDetails = ADD_CAMA_Details.FindAll(u => u.Type1.Trim().Equals("MS") && u.Branch.Trim().Equals(Pass_CA_Entity.Branch.Trim()) && u.Curr_Group.ToString() == (Pass_CA_Entity.Curr_Grp.Trim()));

                    if (SelAddMSDetails.Count > 0)
                    {
                        int rowIndex = 0, Sel_CAMS_Index = 0; int MSrowIndex = 0;
                        string CAMS_DESC = null;
                        foreach (CASESPM2Entity Entity in ADD_CAMA_Details)
                        {
                            if (Entity.Type1 == "MS")
                            {
                                //if (Pass_MS_Entity.MS_Code.Trim() != Entity.CamCd.Trim())
                                MSrowIndex = gvMSgrid.Rows.Add(false, Entity.CamCd.Trim(), Entity.CAMS_Desc, string.Empty, string.Empty, Entity.Type1, "C", Entity.Branch, Entity.Group, "N", null, null, Entity.CAMS_Desc, Pass_CA_Entity.Year, Entity.CAMS_Active, null, Entity.Curr_Group, Entity.Branch, null, string.Empty);
                            }
                        }
                    }
                }
            }

            if (gvMSgrid.Rows.Count > 0)
                gvMSgrid.Rows[0].Selected = true;
        }

        private void gvMSgrid_MenuClick(object objSource, MenuItemEventArgs objArgs)
        {
            int columnIndex = gvMSgrid.CurrentCell.ColumnIndex;
            int rowIndex = gvMSgrid.CurrentCell.RowIndex;

            //if (objArgs.MenuItem.Checked)
            //{

            gvMSgrid.SelectedRows[0].Cells[3].Value = objArgs.MenuItem.Text;
            gvMSgrid.SelectedRows[0].Cells[3].Tag = objArgs.MenuItem.Tag;
            gvMSgrid.SelectedRows[0].Cells["gvMsResult"].Value = objArgs.MenuItem.Text;
            gvMSgrid.SelectedRows[0].Cells["gvMSResultCd"].Value = objArgs.MenuItem.Tag;

            if (CASEMSList.Count > 0)
            {
                foreach (CASEMSEntity Entity in CASEMSList)
                {
                    if (Entity.MS_Code.Trim() == gvMSgrid.SelectedRows[0].Cells["MS_SP2_CAMS_Code"].Value.ToString() && Entity.Branch.Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Branch"].Value.ToString().Trim() && Entity.Curr_Grp.ToString().Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Curr_Grp"].Value.ToString().Trim())
                    {
                        Entity.Result = objArgs.MenuItem.Tag.ToString();
                        Entity.MS_TrigCode = objArgs.MenuItem.Text;
                        if (gvMSgrid.SelectedRows[0].Cells["MSSel_1"].Value.ToString() == "true")
                            Entity.Rec_Type = "I";
                        break;
                    }
                }
            }



            //}

        }

        private void gvMSgrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gvMSgrid.Rows.Count > 0)
            {
                if (e.ColumnIndex == 0 && e.RowIndex != -1)
                {
                    if (gvMSgrid.CurrentRow.Cells["MSSel_1"].Value.ToString() == true.ToString())
                    {
                        gvMSgrid.CurrentRow.Cells["gvbtnMSBenefit"].Value = Img_btn;
                        gvMSgrid.CurrentRow.Cells["gvMsResult"].ReadOnly = false; 

                        if (CASEMSList.Count > 0)
                        {
                            foreach (CASEMSEntity Entity in CASEMSList)
                            {
                                if (Entity.MS_Code.Trim() == gvMSgrid.SelectedRows[0].Cells["MS_SP2_CAMS_Code"].Value.ToString() && Entity.Branch.Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Branch"].Value.ToString().Trim() && Entity.Curr_Grp.ToString().Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Curr_Grp"].Value.ToString().Trim())
                                { Entity.Rec_Type = "I"; break; }
                            }
                        }
                    }
                    else
                    {
                        gvMSgrid.CurrentRow.Cells["gvbtnMSBenefit"].Value = Img_blank;
                        gvMSgrid.CurrentRow.Cells["gvMsResult"].ReadOnly = true;
                        if (CASEMSList.Count > 0)
                        {
                            foreach (CASEMSEntity Entity in CASEMSList)
                            {
                                if (Entity.MS_Code.Trim() == gvMSgrid.SelectedRows[0].Cells["MS_SP2_CAMS_Code"].Value.ToString() && Entity.Branch.Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Branch"].Value.ToString().Trim() && Entity.Curr_Grp.ToString().Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Curr_Grp"].Value.ToString().Trim())
                                { Entity.Rec_Type = ""; break; }
                            }
                        }
                    }
                }

                if (e.ColumnIndex == 21 && e.RowIndex != -1)
                {
                    if (gvMSgrid.CurrentRow.Cells["MSSel_1"].Value.ToString() == true.ToString())
                    {
                        List<CAOBOEntity> SelCAOBO = new List<CAOBOEntity>(); string BenefitFrom = ((Captain.Common.Utilities.ListItem)cmb_CA_Benefit.SelectedItem).Value.ToString();

                        BenefitFrom = gvMSgrid.SelectedRows[0].Cells["MSSP2_OBF"].Value == null ? string.Empty : gvMSgrid.SelectedRows[0].Cells["MSSP2_OBF"].Value.ToString();
                        if (string.IsNullOrEmpty(BenefitFrom))
                            BenefitFrom = ((Captain.Common.Utilities.ListItem)cmb_CA_Benefit.SelectedItem).Value.ToString();

                        if (CAOBO_Ser_List.Count > 0)
                        {
                            SelCAOBO = CAOBO_Ser_List.FindAll(u => u.Type.Equals("MS") && u.Code.Trim() == gvMSgrid.SelectedRows[0].Cells["MS_SP2_CAMS_Code"].Value.ToString().Trim() && u.Branch.Trim().Equals(gvMSgrid.SelectedRows[0].Cells["MSSP2_Branch"].Value.ToString().Trim()) && u.Group.Trim().Equals(gvMSgrid.SelectedRows[0].Cells["MSSP2_Group"].Value.ToString().Trim()));
                            if (SelCAOBO.Count > 0)
                                BenefitFrom = SelCAOBO[0].BenefitFrom.ToString();
                            if (SelCAOBO.Count == 0 && CAOBO_List.Count > 0)
                                SelCAOBO = CAOBO_List;
                        }
                        else if (CAOBO_List.Count > 0)
                        {
                            SelCAOBO = CAOBO_List;
                        }

                        MembersGridForm PostCA_Form;
                        PostCA_Form = new MembersGridForm(BaseForm, Hierarchy, Year, gvMSgrid.SelectedRows[0].Cells["MSSP2_Desc"].Value.ToString(), gvMSgrid.SelectedRows[0].Cells["MS_SP2_CAMS_Code"].Value.ToString(), gvMSgrid.SelectedRows[0].Cells["MSSP2_Branch"].Value.ToString().Trim(), gvMSgrid.SelectedRows[0].Cells["MSSP2_Group"].Value.ToString().Trim(), Pass_CA_Entity, Privileges, CA_Template_List, BenefitFrom, SelCAOBO, "MS");   // 08022012
                        PostCA_Form.FormClosed += new FormClosedEventHandler(Add_MembersForm_Closed);
                        PostCA_Form.StartPosition = FormStartPosition.CenterScreen;
                        PostCA_Form.ShowDialog();
                    }
                        
                }
            }
        }

        string Img_btn = Consts.Icons.ico_SP_btn_dot; 
        string Img_blank = "icon-blank"; 
        string Img_btn_Plain=Consts.Icons.ico_SP_btn_plain;
        private void SP_CA_Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==0 && e.RowIndex != -1)
            {
                if (SP_CA_Grid.CurrentRow.Cells["Sel_1"].Value.ToString() == true.ToString())
                {
                    SP_CA_Grid.CurrentRow.Cells["gvbtnCABenefit"].Value = Img_btn;
                    if (CustomQuestions == "Y")
                        SP_CA_Grid.CurrentRow.Cells["gvCustQues"].Value = Img_btn_Plain;
                    else
                        SP_CA_Grid.CurrentRow.Cells["gvCustQues"].Value = Img_blank;
                }
                else
                {
                    SP_CA_Grid.CurrentRow.Cells["gvbtnCABenefit"].Value = Img_blank;
                    SP_CA_Grid.CurrentRow.Cells["gvCustQues"].Value = Img_blank;
                }
            }

            if (e.ColumnIndex == 30 && e.RowIndex != -1)
            {
                if (SP_CA_Grid.CurrentRow.Cells["Sel_1"].Value.ToString() == true.ToString())
                {
                    List<CAOBOEntity> SelCAOBO = new List<CAOBOEntity>(); string BenefitFrom = ((Captain.Common.Utilities.ListItem)cmb_CA_Benefit.SelectedItem).Value.ToString();
                    //if (!string.IsNullOrEmpty(ACR_BenefitFrom.Trim())) BenefitFrom = ACR_BenefitFrom.Trim();

                    BenefitFrom = SP_CA_Grid.SelectedRows[0].Cells["SP2_OBF"].Value == null ? string.Empty : SP_CA_Grid.SelectedRows[0].Cells["SP2_OBF"].Value.ToString();
                    if (string.IsNullOrEmpty(BenefitFrom))
                        BenefitFrom = ((Captain.Common.Utilities.ListItem)cmb_CA_Benefit.SelectedItem).Value.ToString();

                    if (CAOBO_Ser_List.Count > 0)
                    {
                        SelCAOBO = CAOBO_Ser_List.FindAll(u => u.Type.Equals("CA") && u.Code.Trim() == SP_CA_Grid.SelectedRows[0].Cells["SP2_CAMS_Code"].Value.ToString().Trim() && u.Branch.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Branch"].Value.ToString().Trim()) && u.Group.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Group"].Value.ToString().Trim()));
                        if (SelCAOBO.Count > 0)
                            BenefitFrom = SelCAOBO[0].BenefitFrom.ToString();
                        if (SelCAOBO.Count == 0 && CAOBO_List.Count > 0)
                            SelCAOBO = CAOBO_List;

                    }
                    else if (CAOBO_List.Count > 0)
                    {
                        SelCAOBO = CAOBO_List;
                    }

                    MembersGridForm PostCA_Form;
                    PostCA_Form = new MembersGridForm(BaseForm, Hierarchy, Year, SP_CA_Grid.SelectedRows[0].Cells["SP2_Desc"].Value.ToString(), SP_CA_Grid.SelectedRows[0].Cells["SP2_CAMS_Code"].Value.ToString(), SP_CA_Grid.SelectedRows[0].Cells["SP2_Branch"].Value.ToString().Trim(), SP_CA_Grid.SelectedRows[0].Cells["SP2_Group"].Value.ToString().Trim(), Pass_CA_Entity, Privileges, CA_Template_List, BenefitFrom, SelCAOBO, "CA");   // 08022012
                    PostCA_Form.FormClosed += new FormClosedEventHandler(Add_MembersForm_Closed);
                    PostCA_Form.StartPosition = FormStartPosition.CenterScreen;
                    PostCA_Form.ShowDialog();
                }
                    
            }
            if (e.ColumnIndex == 31 && e.RowIndex != -1)
            {
                //List<CAOBOEntity> SelCAOBO = new List<CAOBOEntity>(); string BenefitFrom = ((Captain.Common.Utilities.ListItem)cmb_CA_Benefit.SelectedItem).Value.ToString();
                ////if (!string.IsNullOrEmpty(ACR_BenefitFrom.Trim())) BenefitFrom = ACR_BenefitFrom.Trim();
                //if (CAOBO_Ser_List.Count > 0)
                //{
                //    SelCAOBO = CAOBO_Ser_List.FindAll(u => u.Type.Equals("CA") && u.Code.Trim() == SP_CA_Grid.SelectedRows[0].Cells["SP2_CAMS_Code"].Value.ToString().Trim() && u.Branch.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Branch"].Value.ToString().Trim()) && u.Group.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Group"].Value.ToString().Trim()));
                //    if (SelCAOBO.Count > 0)
                //        BenefitFrom = SelCAOBO[0].BenefitFrom.ToString();
                //    if (SelCAOBO.Count == 0 && CAOBO_List.Count > 0)
                //        SelCAOBO = CAOBO_List;

                //}
                //else if (CAOBO_List.Count > 0)
                //{
                //    SelCAOBO = CAOBO_List;
                //}
                if (SP_CA_Grid.CurrentRow.Cells["Sel_1"].Value.ToString() == true.ToString())
                {
                    CASEACTEntity New_CA_Entity = new CASEACTEntity();
                    New_CA_Entity.Agency = Pass_CA_Entity.Agency; New_CA_Entity.Dept = Pass_CA_Entity.Dept; New_CA_Entity.Program = Pass_CA_Entity.Program;
                    New_CA_Entity.Year = Pass_CA_Entity.Year; New_CA_Entity.App_no = Pass_CA_Entity.App_no; New_CA_Entity.Service_plan = Pass_CA_Entity.Service_plan;
                    New_CA_Entity.SPM_Seq = Pass_CA_Entity.SPM_Seq; New_CA_Entity.ACT_Date = Pass_CA_Entity.ACT_Date; New_CA_Entity.Caseworker = Pass_CA_Entity.Caseworker;
                    New_CA_Entity.Site = Pass_CA_Entity.Site; New_CA_Entity.Fund1 = Pass_CA_Entity.Fund1; New_CA_Entity.Fund2 = Pass_CA_Entity.Fund2; New_CA_Entity.Fund3 = Pass_CA_Entity.Fund3;
                    New_CA_Entity.Vendor_No = Pass_CA_Entity.Vendor_No; New_CA_Entity.Check_Date = Pass_CA_Entity.Check_Date; New_CA_Entity.Check_No = Pass_CA_Entity.Check_No;
                    New_CA_Entity.Cost = Pass_CA_Entity.Cost; New_CA_Entity.Followup_By = Pass_CA_Entity.Followup_By; New_CA_Entity.Followup_Comp = Pass_CA_Entity.Followup_Comp;
                    New_CA_Entity.Followup_On = Pass_CA_Entity.Followup_On; New_CA_Entity.Refer_Data = Pass_CA_Entity.Refer_Data; New_CA_Entity.Rate = Pass_CA_Entity.Rate;
                    New_CA_Entity.Cust_Code1 = SP_CA_Grid.SelectedRows[0].Cells["Act_Ques1"].Value.ToString(); New_CA_Entity.Cust_Value1 = SP_CA_Grid.SelectedRows[0].Cells["Act_Resp1"].Value.ToString();
                    New_CA_Entity.Cust_Code2 = SP_CA_Grid.SelectedRows[0].Cells["Act_Ques2"].Value.ToString(); New_CA_Entity.Cust_Value2 = SP_CA_Grid.SelectedRows[0].Cells["Act_Resp2"].Value.ToString();
                    New_CA_Entity.Cust_Code3 = SP_CA_Grid.SelectedRows[0].Cells["Act_Ques3"].Value.ToString(); New_CA_Entity.Cust_Value3 = SP_CA_Grid.SelectedRows[0].Cells["Act_Resp3"].Value.ToString();
                    New_CA_Entity.Cust_Code4 = SP_CA_Grid.SelectedRows[0].Cells["Act_Ques4"].Value.ToString(); New_CA_Entity.Cust_Value4 = SP_CA_Grid.SelectedRows[0].Cells["Act_Resp4"].Value.ToString();
                    New_CA_Entity.Cust_Code5 = SP_CA_Grid.SelectedRows[0].Cells["Act_Ques5"].Value.ToString(); New_CA_Entity.Cust_Value5 = SP_CA_Grid.SelectedRows[0].Cells["Act_Resp5"].Value.ToString();
                    New_CA_Entity.Bulk = "Q"; New_CA_Entity.Units = Pass_CA_Entity.Units; New_CA_Entity.VOUCHNO = Pass_CA_Entity.VOUCHNO;
                    New_CA_Entity.Act_PROG = Pass_CA_Entity.Act_PROG; New_CA_Entity.Notes_Count = Pass_CA_Entity.Notes_Count;
                    New_CA_Entity.Lsct_Operator = BaseForm.UserID;

                    if (!string.IsNullOrEmpty(Txt_CA_Program.Text.Trim())) New_CA_Entity.Act_PROG = Txt_CA_Program.Text.Substring(0, 6);

                    New_CA_Entity.ACT_Code = SP_CA_Grid.SelectedRows[0].Cells["SP2_CAMS_Code"].Value.ToString();
                    New_CA_Entity.Branch = SP_CA_Grid.SelectedRows[0].Cells["SP2_Branch"].Value.ToString();
                    New_CA_Entity.Group = SP_CA_Grid.SelectedRows[0].Cells["SP2_Group"].Value.ToString();
                    New_CA_Entity.Curr_Grp = SP_CA_Grid.SelectedRows[0].Cells["SP2_Curr_Grp"].Value.ToString();
                    New_CA_Entity.ACT_ID = "1";
                    New_CA_Entity.ACT_Seq = "1";

                    ServiceCustomQuestionsForm PostCA_Form;
                    PostCA_Form = new ServiceCustomQuestionsForm(BaseForm, Hierarchy, Year, SP_CA_Grid.SelectedRows[0].Cells["SP2_Desc"].Value.ToString(), SP_CA_Grid.SelectedRows[0].Cells["SP2_CAMS_Code"].Value.ToString(), SP_CA_Grid.SelectedRows[0].Cells["SP2_Branch"].Value.ToString().Trim(), SP_CA_Grid.SelectedRows[0].Cells["SP2_Group"].Value.ToString().Trim(), New_CA_Entity, Privileges, CA_Template_List, "CA");   // 08022012
                    PostCA_Form.FormClosed += new FormClosedEventHandler(Add_QuestionsForm_Closed);
                    PostCA_Form.StartPosition = FormStartPosition.CenterScreen;
                    PostCA_Form.ShowDialog();
                }
               
            }
        }

        private void Add_MembersForm_Closed(object sender, FormClosedEventArgs e)
        {
            List<CAOBOEntity> CAOBO_List_Memebers = new List<CAOBOEntity>();
            MembersGridForm form = sender as MembersGridForm;
            if (form.DialogResult == DialogResult.OK)
            {
                CAOBO_List_Memebers = new List<CAOBOEntity>();
                CAOBO_List_Memebers = form.GetRecordsForMembers();

                if (CAOBO_Ser_List.Count == 0) CAOBO_Ser_List.AddRange(CAOBO_List_Memebers);



                if (CAOBO_List_Memebers.Count > 0)
                {
                    if (CAOBO_List_Memebers[0].Type.ToString() == "CA")
                    {
                        SP_CA_Grid.SelectedRows[0].Cells["Sel_1"].Value = true;
                        List<CAOBOEntity> SelCAOBO = CAOBO_Ser_List.FindAll(u => u.Type.Equals("CA") && u.Code.Trim() == SP_CA_Grid.SelectedRows[0].Cells["SP2_CAMS_Code"].Value.ToString().Trim() && u.Branch.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Branch"].Value.ToString().Trim()) && u.Group.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Group"].Value.ToString().Trim()));
                        if (SelCAOBO.Count > 0)
                        {
                            //CAOBO_Ser_List.Where(u => u.Type == "CA" && u.Code.Trim() == SP_CA_Grid.SelectedRows[0].Cells["SP2_CAMS_Code"].Value.ToString().Trim() && u.Branch.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Branch"].Value.ToString().Trim()) && u.Group.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Group"].Value.ToString().Trim())).ToList().ForEach(p => CAOBO_Ser_List.Remove(p));
                            CAOBO_Ser_List.RemoveAll(u => u.Type == "CA" && u.Code.Trim() == SP_CA_Grid.SelectedRows[0].Cells["SP2_CAMS_Code"].Value.ToString().Trim() && u.Branch.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Branch"].Value.ToString().Trim()) && u.Group.Trim().Equals(SP_CA_Grid.SelectedRows[0].Cells["SP2_Group"].Value.ToString().Trim()));

                            CAOBO_Ser_List.AddRange(CAOBO_List_Memebers);
                        }
                        else CAOBO_Ser_List.AddRange(CAOBO_List_Memebers);
                    }
                    else if (CAOBO_List_Memebers[0].Type.ToString() == "MS")
                    {
                        gvMSgrid.SelectedRows[0].Cells["MSSel_1"].Value = true;
                        List<CAOBOEntity> SelCAOBO = CAOBO_Ser_List.FindAll(u => u.Type.Equals("MS") && u.Code.Trim() == gvMSgrid.SelectedRows[0].Cells["MS_SP2_CAMS_Code"].Value.ToString().Trim() && u.Branch.Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Branch"].Value.ToString().Trim() && u.Group.ToString().Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Group"].Value.ToString().Trim());
                        if (SelCAOBO.Count > 0)
                        {
                            //CAOBO_Ser_List.Where(u => u.Type.Equals("MS") && u.Code.Trim() == gvMSgrid.SelectedRows[0].Cells["MS_SP2_CAMS_Code"].Value.ToString().Trim() && u.Branch.Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Branch"].Value.ToString().Trim() && u.Group.ToString().Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Group"].Value.ToString().Trim()).ToList().ForEach(p => CAOBO_Ser_List.Remove(p));
                            CAOBO_Ser_List.RemoveAll(u => u.Type.Equals("MS") && u.Code.Trim() == gvMSgrid.SelectedRows[0].Cells["MS_SP2_CAMS_Code"].Value.ToString().Trim() && u.Branch.Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Branch"].Value.ToString().Trim() && u.Group.ToString().Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Group"].Value.ToString().Trim());

                            CAOBO_Ser_List.AddRange(CAOBO_List_Memebers);
                        }
                        else CAOBO_Ser_List.AddRange(CAOBO_List_Memebers);
                    }


                }
            }
        }

        private void Add_QuestionsForm_Closed(object sender, FormClosedEventArgs e)
        {
            List<CAOBOEntity> CAOBO_List_Memebers = new List<CAOBOEntity>();
            ServiceCustomQuestionsForm form = sender as ServiceCustomQuestionsForm;
            if (form.DialogResult == DialogResult.OK)
            {
                SP_CA_Grid.SelectedRows[0].Cells["Sel_1"].Value = true;

                string[] From_Results = new string[15];
                From_Results = form.GetSelected_MS_Code();

                if (!string.IsNullOrEmpty(From_Results[0].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_Ques1"].Value = From_Results[0].ToString();
                if (!string.IsNullOrEmpty(From_Results[1].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_Resp1"].Value = From_Results[1].ToString();
                if (!string.IsNullOrEmpty(From_Results[2].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_Ques2"].Value = From_Results[2].ToString();
                if (!string.IsNullOrEmpty(From_Results[3].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_Resp2"].Value = From_Results[3].ToString();
                if (!string.IsNullOrEmpty(From_Results[4].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_Ques3"].Value = From_Results[4].ToString();
                if (!string.IsNullOrEmpty(From_Results[5].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_Resp3"].Value = From_Results[5].ToString();
                if (!string.IsNullOrEmpty(From_Results[6].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_Ques4"].Value = From_Results[6].ToString();
                if (!string.IsNullOrEmpty(From_Results[7].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_Resp4"].Value = From_Results[7].ToString();
                if (!string.IsNullOrEmpty(From_Results[8].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_Ques5"].Value = From_Results[8].ToString();
                if (!string.IsNullOrEmpty(From_Results[9].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_Resp5"].Value = From_Results[9].ToString();

                if (From_Results[10].ToString().Trim() == "*" && string.IsNullOrEmpty(From_Results[1].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_CustReq"].Value = "*";
                else if (From_Results[11].ToString().Trim() == "*" && string.IsNullOrEmpty(From_Results[3].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_CustReq"].Value = "*";
                else if (From_Results[12].ToString().Trim() == "*" && string.IsNullOrEmpty(From_Results[5].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_CustReq"].Value = "*";
                else if (From_Results[13].ToString().Trim() == "*" && string.IsNullOrEmpty(From_Results[7].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_CustReq"].Value = "*";
                else if (From_Results[14].ToString().Trim() == "*" && string.IsNullOrEmpty(From_Results[9].ToString().Trim()))
                    SP_CA_Grid.CurrentRow.Cells["Act_CustReq"].Value = "*";
                else
                    SP_CA_Grid.CurrentRow.Cells["Act_CustReq"].Value = "";
                //Act_CustReq


            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SALCAL_Form sal_Form;
            sal_Form = new SALCAL_Form(BaseForm, Privileges, Pass_CA_Entity, Act_Date.Text, ((Captain.Common.Utilities.ListItem)CmbWorker.SelectedItem).Value.ToString(), ((Captain.Common.Utilities.ListItem)CmbFunding1.SelectedItem).Value.ToString(), SP_Header_Rec, Mode, SALData);   // 08022012
            sal_Form.FormClosed += new FormClosedEventHandler(sal_Form_Closed);
            sal_Form.StartPosition = FormStartPosition.CenterScreen;
            sal_Form.ShowDialog();
        }

        List<SALACTEntity> SALData = new List<SALACTEntity>();
        private void sal_Form_Closed(object sender, FormClosedEventArgs e)
        {
            List<SALACTEntity> SALACT_List = new List<SALACTEntity>();
            SALCAL_Form form = sender as SALCAL_Form;
            if (form.DialogResult == DialogResult.OK)
            {
                SALACT_List = new List<SALACTEntity>();
                SALACT_List = form.SALACTData();

                if (SALACT_List.Count > 0) SALData = SALACT_List;
            }
        }

        private void Cmb_UOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            //lblCost.Text = "Amount 1";
            if (((Captain.Common.Utilities.ListItem)Cmb_UOM.SelectedItem).Value.ToString() != "0")
            {
                if (!string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)Cmb_UOM.SelectedItem).DefaultValue.ToString()))
                    lblCost.Text = ((Captain.Common.Utilities.ListItem)Cmb_UOM.SelectedItem).DefaultValue.ToString();
            }

        }

        private void Cmb_UOM2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //lblCost2.Text = "Amount 2";
            //if (((Captain.Common.Utilities.ListItem)Cmb_UOM2.SelectedItem).Value.ToString() != "0")
            //{
            //    if (!string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)Cmb_UOM2.SelectedItem).DefaultValue.ToString()))
            //        lblCost2.Text = ((Captain.Common.Utilities.ListItem)Cmb_UOM2.SelectedItem).DefaultValue.ToString();
            //}

        }

        private void Cmb_UOM3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //lblCost3.Text = "Amount 3";
            //if (((Captain.Common.Utilities.ListItem)Cmb_UOM3.SelectedItem).Value.ToString() != "0")
            //{
            //    if (!string.IsNullOrEmpty(((Captain.Common.Utilities.ListItem)Cmb_UOM3.SelectedItem).DefaultValue.ToString()))
            //        lblCost3.Text = ((Captain.Common.Utilities.ListItem)Cmb_UOM3.SelectedItem).DefaultValue.ToString();
            //}

        }

        public List<CASESP2Entity> Find_SP_CAMS_Details { get; set; }
        private void btnCAMSSearch_Click(object sender, EventArgs e)
        {
            if (txtCAMSSearch.Text.Trim() != string.Empty)
            {
                Find_SP_CAMS_Details = SP_CAMS_Details.FindAll(x => x.CAMS_Desc.ToUpper().Contains(txtCAMSSearch.Text.ToUpper()));
                Find_SP_CAMS_Details = Find_SP_CAMS_Details.FindAll(u => u.SP2_CAMS_Active == "A" && u.CAMS_Active == "True");

                if (Find_SP_CAMS_Details.Count > 0)
                {
                    foreach (DataGridViewRow item in SP_CA_Grid.Rows)
                    {
                        if (Find_SP_CAMS_Details[0].CamCd.Trim() == Convert.ToString(item.Cells["SP2_CAMS_Code"].Value))//if (Convert.ToString(item.Cells["SP2_CAMS_Code"].Value).Contains(txtCAMSSearch.Text.Trim()))
                        {
                            //gvwCustomer.Update();
                            int i = item.Index;

                            // int intscroolindex = gvwCustomer.FirstDisplayedScrollingRowIndex;
                            //int CurrentPage = (i / SP_CA_Grid.ItemsPerPage);
                            // CurrentPage++;
                            //SP_CA_Grid.CurrentPage = CurrentPage;
                            SP_CA_Grid.CurrentCell = SP_CA_Grid.Rows[i].Cells[2];
                            // SP_CA_Grid.FirstDisplayedScrollingRowIndex = i;
                            SP_CA_Grid.Rows[i].Selected = true;
                            intFindNext = i; boolNxt = true; FindPrev = i;
                            break;

                        }
                    }
                }
            }
        }

        int intNext = 0; int intFindNext = 0; bool boolNxt = false; int FindPrev = 0;
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (txtCAMSSearch.Text.Trim() != string.Empty)
            {
                if (SP_CA_Grid.Rows.Count > 0)
                {
                    foreach (DataGridViewRow item in SP_CA_Grid.Rows)
                    {
                        if (item.Cells["SP2_Desc"].Value.ToString().ToUpper().Trim().Contains(txtCAMSSearch.Text.Trim())) //if (txtCAMSSearch.Text.Trim().Contains(Convert.ToString(item.Cells["SP2_Desc"].Value).Trim()))
                        {

                            int i = item.Index;

                            if (intFindNext == 0 && !boolNxt)
                            {
                                //int CurrentPage = (i / SP_CA_Grid.ItemsPerPage);
                                // CurrentPage++;
                                // SP_CA_Grid.CurrentPage = CurrentPage;
                                SP_CA_Grid.CurrentCell = SP_CA_Grid.Rows[i].Cells[2];
                                // SP_CA_Grid.FirstDisplayedScrollingRowIndex = i;
                                SP_CA_Grid.Rows[i].Selected = true;
                                intFindNext = i;
                                break;
                            }
                            else
                            {
                                if (i > intFindNext)
                                {
                                    //  int CurrentPage = (i / SP_CA_Grid.ItemsPerPage);
                                    //  CurrentPage++;
                                    //  SP_CA_Grid.CurrentPage = CurrentPage;
                                    SP_CA_Grid.CurrentCell = SP_CA_Grid.Rows[i].Cells[2];
                                    // SP_CA_Grid.FirstDisplayedScrollingRowIndex = i;
                                    SP_CA_Grid.Rows[i].Selected = true;
                                    intFindNext = i;
                                    break;
                                }
                            }

                        }
                    }
                    boolNxt = true;
                }
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (txtCAMSSearch.Text.Trim() != string.Empty)
            {
                bool Prev = false;

                foreach (DataGridViewRow item in SP_CA_Grid.Rows)
                {
                    //if (Find_SP_CAMS_Details.Count > intNext)
                    //{

                    if (item.Cells["SP2_Desc"].Value.ToString().ToUpper().Trim().Contains(txtCAMSSearch.Text.Trim()))//if (txtCAMSSearch.Text.Trim().Contains(Convert.ToString(item.Cells["SP2_Desc"].Value).Trim()))
                    {
                        int i = item.Index;

                        if (intFindNext == item.Index)
                        {
                            //int CurrentPage = (FindPrev / SP_CA_Grid.ItemsPerPage);
                            // CurrentPage++;
                            // SP_CA_Grid.CurrentPage = CurrentPage;
                            SP_CA_Grid.CurrentCell = SP_CA_Grid.Rows[FindPrev].Cells[2];
                            //SP_CA_Grid.FirstDisplayedScrollingRowIndex = FindPrev;
                            SP_CA_Grid.Rows[FindPrev].Selected = true;
                            intFindNext = FindPrev; Prev = true;
                            break;
                        }
                        FindPrev = item.Index;

                    }
                }

                if (!Prev)
                {
                    //  int CurrentPage = (FindPrev / SP_CA_Grid.ItemsPerPage);
                    // CurrentPage++;
                    // SP_CA_Grid.CurrentPage = CurrentPage;
                    SP_CA_Grid.CurrentCell = SP_CA_Grid.Rows[FindPrev].Cells[2];
                    //  SP_CA_Grid.FirstDisplayedScrollingRowIndex = FindPrev;
                    SP_CA_Grid.Rows[FindPrev].Selected = true;
                    intFindNext = FindPrev; Prev = true;
                }
            }
        }

        private void contextMenu1_Popup(object sender, EventArgs e)
        {
            contextMenu1.MenuItems.Clear();
            if (Cust_Grid.Rows.Count > 0)
            {
                if ((Cust_Grid.CurrentRow.Cells["Type"].Value.ToString() == "D"))
                {
                    List<PopUp_Menu_L1_Entity> listItem = new List<PopUp_Menu_L1_Entity>();

                    foreach (SalqrespEntity Entity in SALQUESRespEntity)
                    {
                        if (Cust_Grid.CurrentRow.Cells["Code"].Value.ToString() == Entity.SALQR_Q_ID)
                        {
                            MenuItem Resp_Menu = new MenuItem();
                            Resp_Menu.Text = Entity.SALQR_DESC;
                            Resp_Menu.Tag = Entity.SALQR_CODE;
                            contextMenu1.MenuItems.Add(Resp_Menu);

                            if (Cust_Grid.CurrentRow.Cells["Resp"].Value.ToString() == Entity.SALQR_DESC)
                                Resp_Menu.Checked = true;
                        }
                    }

                    //foreach (CustRespEntity Entity in CustResp_List)
                    //{
                    //    if (Cust_Grid.CurrentRow.Cells["Code"].Value.ToString() == Entity.ResoCode)
                    //    {
                    //        MenuItem Resp_Menu = new MenuItem();
                    //        Resp_Menu.Text = Entity.RespDesc;
                    //        Resp_Menu.Tag = Entity.DescCode;
                    //        contextMenu1.MenuItems.Add(Resp_Menu);

                    //        if (Cust_Grid.CurrentRow.Cells["Resp"].Value.ToString() == Entity.RespDesc)
                    //            Resp_Menu.Checked = true;
                    //    }
                    //}
                    MenuItem Resp_Menu1 = new MenuItem();
                    Resp_Menu1.Text = "Clear Response";
                    Resp_Menu1.Tag = "CLRRSP";
                    contextMenu1.MenuItems.Add(Resp_Menu1);
                }
                else if (Cust_Grid.CurrentRow.Cells["Type"].Value.ToString() == "C")
                {
                    string response = Cust_Grid.SelectedRows[0].Cells[5].Value != null ? Cust_Grid.SelectedRows[0].Cells[5].Value.ToString() : string.Empty;
                    //gvQuestions.SelectedRows[0].Cells[8].Tag != null ? gvQuestions.SelectedRows[0].Cells[8].Tag.ToString() : string.Empty;
                    PrivilegeEntity privileges = new PrivilegeEntity();
                    privileges.AddPriv = "true";
                    AlertCodeForm objform = new AlertCodeForm(BaseForm, privileges, response, Cust_Grid.CurrentRow.Cells["Code"].Value.ToString(), Cust_Grid.CurrentRow.Cells["Ques"].Value.ToString(), string.Empty);
                    objform.FormClosed += new FormClosedEventHandler(objform_FormClosed);
                    objform.StartPosition = FormStartPosition.CenterScreen;
                    objform.ShowDialog();
                }
            }
        }

        void objform_FormClosed(object sender, FormClosedEventArgs e)
        {
            //AlertCodeForm form = sender as AlertCodeForm;
            //if (form.DialogResult == DialogResult.OK)
            //{
            //    gvwCustomQuestions.SelectedRows[0].Cells[3].Tag = form.propAlertCode;
            //    gvwCustomQuestions.SelectedRows[0].Cells[3].Value = form.propAlertCode;
            //}
            // //  txtAlertCodes.Text = form.propAlertCode;

            AlertCodeForm form = sender as AlertCodeForm;
            if (form.DialogResult == DialogResult.OK)
            {
                Cust_Grid.SelectedRows[0].Cells[5].Tag = form.propAlertCode;

                string custQuestionResp = string.Empty;
                SalqrespEntity Search_entity = new SalqrespEntity(true);
                Search_entity.SALQR_Q_ID = Cust_Grid.SelectedRows[0].Cells["Code"].Value.ToString();

                List<SalqrespEntity> SALResponseEntity = _model.SALDEFData.Browse_SALQRESP(Search_entity, "Browse");
                if (SALResponseEntity.Count > 0)
                {
                    string response1 = form.propAlertCode;
                    if (!string.IsNullOrEmpty(response1))
                    {
                        string[] arrResponse = null;
                        if (response1.IndexOf(',') > 0)
                        {
                            arrResponse = response1.Split(',');
                        }
                        else if (!response1.Equals(string.Empty))
                        {
                            arrResponse = new string[] { response1 };
                        }
                        foreach (string stringitem in arrResponse)
                        {

                            SalqrespEntity custRespEntity = SALResponseEntity.Find(u => u.SALQR_CODE.Trim().Equals(stringitem.Trim()));
                            if (custRespEntity != null)
                            {
                                custQuestionResp += custRespEntity.SALQR_DESC + ", ";
                            }
                        }
                    }
                }

                if (custQuestionResp.Length > 1)
                {
                    custQuestionResp = custQuestionResp.Trim();
                    if ((custQuestionResp.Substring(custQuestionResp.Length - 1)) == ",")
                    {
                        custQuestionResp = custQuestionResp.Remove(custQuestionResp.Length - 1, 1);
                    }
                    //Cust_Grid.SelectedRows[0].Cells["Ques_Delete"].Value = DeleteImage;
                }
                Cust_Grid.SelectedRows[0].Cells[1].Value = custQuestionResp;
                Cust_Grid.SelectedRows[0].Cells[5].Value = form.propAlertCode;

            }

        }


        private void Cust_Grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            decimal number;
            DateTime Compare_Date = DateTime.Today;
            if (e.ColumnIndex == 1)
            {
                switch (Cust_Grid.CurrentRow.Cells["Type"].Value.ToString())
                {
                    case "N":
                        if (Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                        {
                            if (Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Trim() != "")
                            {
                                if ((!(Decimal.TryParse(Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out number))) &&
                                !(string.IsNullOrEmpty(Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())))
                                {
                                    AlertBox.Show("Please Enter Decimal Response", MessageBoxIcon.Warning);
                                    Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                                }
                            }
                        }
                        break;

                    case "X":
                    case "A":
                        if ((string.IsNullOrEmpty(Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString().Trim())))
                        {
                            AlertBox.Show("Please Provide Valid Data", MessageBoxIcon.Warning);
                            Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                        }
                        break;

                    case "T":
                        //if (!(string.IsNullOrEmpty(Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())) &&
                        //      (!(Convert.ToDateTime(Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out DateTime.Today))))
                        //{
                        //    AlertBox.Show("Please Provide Valid Data", "CAP Systems", MessageBoxButtons.OK);
                        //    Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                        //}
                        if (!(string.IsNullOrEmpty(Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString())))
                        {
                            string strDate = Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                            if (strDate != "")
                            {
                                strDate = Convert.ToDateTime(Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()).ToString("MM/dd/yyyy");
                                if (!System.Text.RegularExpressions.Regex.IsMatch(strDate, Consts.StaticVars.DateFormatMMDDYYYY))
                                {
                                    try
                                    {
                                        if (DateTime.Parse(Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < Convert.ToDateTime("01/01/1800"))
                                        {
                                            AlertBox.Show("01/01/1800 below date not except", MessageBoxIcon.Warning);
                                            Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                                        }
                                        else
                                            AlertBox.Show(Consts.Messages.PleaseEntermmddyyyyDateFormat, MessageBoxIcon.Warning);
                                    }
                                    catch (Exception)
                                    {
                                        AlertBox.Show("Please Enter Valid Date Format MM/DD/YYYY", MessageBoxIcon.Warning);
                                        Cust_Grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void Cust_Grid_MenuClick(object objSource, MenuItemEventArgs objArgs)
        {
            if (objArgs.MenuItem.Tag is string)
            {
                if (objArgs.MenuItem.Tag != "CLRRSP")
                {
                    Cust_Grid.CurrentRow.Cells["Resp"].Value = objArgs.MenuItem.Text;
                    Cust_Grid.CurrentRow.Cells["Resp_Code"].Value = objArgs.MenuItem.Tag;
                }
                else
                {
                    Cust_Grid.CurrentRow.Cells["Resp"].Value = " ";
                    Cust_Grid.CurrentRow.Cells["Resp_Code"].Value = " ";
                }
            }
        }

        private void contextMenu_Result_Popup(object sender, EventArgs e)
        {
            contextMenu_Result.MenuItems.Clear();
            if (gvMSgrid.Rows.Count > 0)
            {
                List<SPCommonEntity> ResultsList = new List<SPCommonEntity>();
                ResultsList = _model.SPAdminData.Get_AgyRecs("Results");
                foreach (DataGridViewRow dr in gvMSgrid.SelectedRows)
                {
                    if (dr.Selected)
                    {
                        //if (!string.IsNullOrEmpty(dr.Tag.ToString().Trim()))
                        //{
                        if (ResultsList.Count > 0)
                        {


                            if (!string.IsNullOrEmpty(SP_Header_Rec.Status))
                            {
                                bool Ststus_Exists = false; int Pos = 0, Tmp_Loop_Cnt = 0, Tmp_Curr_Status_Len = 0;
                                string Status_Str = SP_Header_Rec.Status;

                                MenuItem menuItem1 = new MenuItem();
                                menuItem1.Text = "";
                                menuItem1.Tag = "";
                                contextMenu_Result.MenuItems.Add(menuItem1);

                                foreach (SPCommonEntity Entity in ResultsList)
                                {
                                    Ststus_Exists = false; Pos = 0;
                                    for (int i = 0; Pos < SP_Header_Rec.Status.Length; i++)
                                    {
                                        Tmp_Curr_Status_Len = (Status_Str.Substring(Pos, Status_Str.Substring(Pos, (Status_Str.Length - Pos)).Length)).Length;

                                        if (Entity.Code == SP_Header_Rec.Status.Substring(Pos, (Tmp_Curr_Status_Len >= 4 ? 4 : Tmp_Curr_Status_Len)).Trim())
                                        {
                                            Ststus_Exists = true; break;
                                        }
                                        Pos += 4;
                                    }

                                    if (Ststus_Exists)
                                    {
                                        if (Mode == "Edit" || (Mode == "Add" && Entity.Active.Equals("Y")))
                                        {
                                            MenuItem menuItem = new MenuItem();
                                            menuItem.Text = Entity.Desc;
                                            menuItem.Tag = Entity.Code;
                                            contextMenu_Result.MenuItems.Add(menuItem);
                                            Tmp_Loop_Cnt++;
                                        }
                                    }
                                }

                            }

                            //foreach (SPCommonEntity Entity in ResultsList)
                            //{
                            //    MenuItem menuItem = new MenuItem();
                            //    menuItem.Text = Entity.Desc;
                            //    menuItem.Tag = Entity.Code;
                            //    contextMenu_Result.MenuItems.Add(menuItem);

                            //}
                        }
                        //}
                    }
                }
            }
        }

        //List<CustfldsEntity> Cust;
        private void Fill_Custom_Questions()
        {
            Cust_Grid.Rows.Clear();
            Cust = _model.FieldControls.GetCUSTFLDSByScrCode("CASE0063", "FLDCNTLHIE", BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg);
            List<CustomQuestionsEntity> custQuestions = _model.FieldControls.GetCustomQuestions("CASE0063", "*", BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg, "", "ALL", "P");

            CustRespEntity Search_Entity = new CustRespEntity(true);
            Search_Entity.ScrCode = "CASE0063";
            CustResp_List = _model.FieldControls.Browse_CUSTRESP(Search_Entity, "Browse");

            //if (Pass_CA_Entity.Rec_Type == "I" && CA_Template_List.Count > 0)
            //{
            //    Pass_CA_Entity.Cust_Code1 = CA_Template_List[0].Cust_Code1;
            //    Pass_CA_Entity.Cust_Code2 = CA_Template_List[0].Cust_Code2;
            //    Pass_CA_Entity.Cust_Code3 = CA_Template_List[0].Cust_Code3;
            //}


            if (Cust.Count > 0)
            {
                int rowIndex = 0, Tmp_Row_Cnt = 0;
                bool DropDown_Exists = false, IS_Que_Req = false;
                string Tmp_Cust_Resp = null, Tmp_Cust_Resp_Code = null, Tmp_Cust_Resp_Desc = "";

                foreach (CustfldsEntity Entity in Cust)
                {
                    Tmp_Cust_Resp = " ";
                    if (Entity.CustCode.Equals(Pass_CA_Entity.Cust_Code1))
                        Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value1;
                    else
                        if (Entity.CustCode.Equals(Pass_CA_Entity.Cust_Code2))
                        Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value2;
                    else
                            if (Entity.CustCode.Equals(Pass_CA_Entity.Cust_Code3))
                        Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value3;
                    else
                                if (Entity.CustCode.Equals(Pass_CA_Entity.Cust_Code4))
                        Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value4;
                    else
                                    if (Entity.CustCode.Equals(Pass_CA_Entity.Cust_Code5))
                        Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value5;

                    Tmp_Cust_Resp_Desc = Tmp_Cust_Resp;
                    if (Entity.RespType == "D" || Entity.RespType == "C")
                    {
                        foreach (CustRespEntity Ent in CustResp_List)
                        {
                            if (Entity.CustCode == Ent.ResoCode && Tmp_Cust_Resp == Ent.DescCode)
                            {
                                Tmp_Cust_Resp_Desc = Ent.RespDesc;
                                break;
                            }
                        }
                    }

                    IS_Que_Req = false;
                    foreach (CustomQuestionsEntity Cust_Fld in custQuestions)
                    {
                        if (Entity.CustCode == Cust_Fld.CUSTCODE && Cust_Fld.CUSTREQUIRED == "Y")
                        {
                            IS_Que_Req = true;
                            break;
                        }
                    }



                    rowIndex = Cust_Grid.Rows.Add(Entity.CustDesc, Tmp_Cust_Resp_Desc, (IS_Que_Req ? "*" : ""), Entity.RespType, Entity.CustCode, Tmp_Cust_Resp);
                    Tmp_Row_Cnt++;

                    set_Cust_Grid_Tooltip(rowIndex, Entity.RespType);

                    if (Entity.RespType == "C" || Entity.RespType == "D")
                    {
                        DropDown_Exists = true;
                        Cust_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue;
                        Cust_Grid.Rows[rowIndex].Cells["Resp"].ReadOnly = true;
                    }

                    if (Tmp_Row_Cnt == 5) //changed from 3 to 5 On 11/20/2014
                        break;
                }
                //set_CustGrid_Rows_On_Questions(DropDown_Exists);
            }
        }

        private void set_Cust_Grid_Tooltip(int rowIndex, string Type)
        {
            string Type_Desc = "Not Defined";
            switch (Type)
            {
                case "C": Type_Desc = "Check Box"; break;
                case "D": Type_Desc = "DropDown"; break;
                case "T": Type_Desc = "Date"; break;
                case "N": Type_Desc = "Numeric"; break;
                case "X": Type_Desc = "Text"; break;

            }

            string toolTipText = "Question Type : " + Type_Desc;

            foreach (DataGridViewCell cell in Cust_Grid.Rows[rowIndex].Cells)
                cell.ToolTipText = toolTipText;
        }

        private void Pb_SP2_Max_Click(object sender, EventArgs e)
        {
            Pb_SP2_Max.Visible = false;
            //this.pnlb3_CA_Services.Location = new System.Drawing.Point(1, 0);
            //this.pnlb3_CA_Services.Size = new System.Drawing.Size(840, 422);

            //this.SP_CA_Grid.Location = new System.Drawing.Point(5, 34);
            //this.SP_CA_Grid.Size = new System.Drawing.Size(819, 379);


            CAPanel.Visible = false;
            pnlServActlog.Visible = false;
            this.pnlCA.Visible = false;

            Pb_SP2_Min.Visible = true;

            if (pnlclt.Count == 1)
            {
                Panel panel = pnlclt[0];
                panel.Dock = DockStyle.Fill;
            }
            if (pnlclt.Count > 1)
            {
                Panel panel = pnlclt[0];
                panel.Dock = DockStyle.Fill;

                Panel panel1 = pnlclt[1];
                panel1.Dock = DockStyle.Bottom;
            }
        }

        private void Pb_SP2_Min_Click(object sender, EventArgs e)
        {
            Pb_SP2_Min.Visible = false;
            //this.pnlb3_CA_Services.Size = new System.Drawing.Size(840, 180);
            //this.pnlb3_CA_Services.Location = new System.Drawing.Point(1, 242);

            //this.SP_CA_Grid.Location = new System.Drawing.Point(5, 34);
            //this.SP_CA_Grid.Size = new System.Drawing.Size(819, 141);

            this.pnlCA.Visible = true;
            CAPanel.Visible = true;
            pnlServActlog.Visible = false;

            Pb_SP2_Max.Visible = true;

            if (pnlclt.Count == 1)
            {
                Panel panel = pnlclt[0];
                panel.Dock = DockStyle.Fill;
            }
            if (pnlclt.Count > 1)
            {
                Panel panel = pnlclt[0];
                panel.Dock = DockStyle.Top;

                Panel panel1 = pnlclt[1];
                panel1.Dock = DockStyle.Fill;
            }
        }

        private void pb_MS_Max_Click(object sender, EventArgs e)
        {
            pb_MS_Max.Visible = false;
            //this.pnlb3_outcomes.Location = new System.Drawing.Point(1, 0);
            //this.pnlb3_outcomes.Size = new System.Drawing.Size(840, 535);

            //this.gvMSgrid.Location = new System.Drawing.Point(5, 19);
            //this.gvMSgrid.Size = new System.Drawing.Size(818, 507);

            CAPanel.Visible = false;
            pnlServActlog.Visible = false;
            this.pnlCA.Visible = false;
            this.pnlb3_CA_Services.Visible = false;

            this.pnlb3_outcomes.Dock = DockStyle.Fill;
            pb_MS_Min.Visible = true;
        }

        private void pb_MS_Min_Click(object sender, EventArgs e)
        {
            pb_MS_Min.Visible = false;
            //this.pnlb3_outcomes.Location = new System.Drawing.Point(1, 423);
            //this.pnlb3_outcomes.Size = new System.Drawing.Size(840, 113);

            //this.gvMSgrid.Location = new System.Drawing.Point(5, 19);
            //this.gvMSgrid.Size = new System.Drawing.Size(818, 94);

            this.pnlCA.Visible = true;
            CAPanel.Visible = true;
            pnlServActlog.Visible = false;

            this.pnlb3_CA_Services.Visible = true;
            pnlb3_CA_Services.Dock = DockStyle.Top;
            pnlclt.Add(pnlb3_CA_Services);
            this.pnlb3_outcomes.Dock = DockStyle.Fill;
            //Added by Sudheer on 04/07/2021
            if (Pb_SP2_Max.Visible == false)
            {
                this.pnlCA.Visible = false;
                CAPanel.Visible = false;
                pnlServActlog.Visible = false;
                this.pnlb3_CA_Services.Visible = true;
                pnlb3_CA_Services.Dock = DockStyle.Fill;
                this.pnlb3_outcomes.Dock = DockStyle.Bottom;

                this.pnlCA.Visible = false;
                Pb_SP2_Min.Visible = true;
            }

            pb_MS_Max.Visible = true;
        }

        private void CASE4006_CAMSForm_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (e.Tool.Name == "tlpdf")
            {
                //ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, Mode, Privileges, Hierarchy + Year + App_No + "0000".Substring(0, (4 - Pass_Entity.Seq.Length)) + Pass_Entity.Seq);
                //Prog_Form.FormClosed += new FormClosedEventHandler(On_PROGNOTES_Closed);
                //Prog_Form.StartPosition = FormStartPosition.CenterScreen;
                //Prog_Form.ShowDialog();
            }
            else if (e.Tool.Name == "tlCaseNotes")
            {
                string Notes_Field_Name = null;

                if (CAMS_FLG == "CA")
                    Notes_Field_Name = Hierarchy + Pass_CA_Entity.Year + Pass_CA_Entity.App_no + Pass_CA_Entity.Service_plan.Trim() + Pass_CA_Entity.SPM_Seq + Pass_CA_Entity.Branch.Trim() +
                            Pass_CA_Entity.Group.ToString() + "CA" + Pass_CA_Entity.ACT_Code.Trim() + Pass_CA_Entity.ACT_Seq + Pass_CA_Entity.ACT_ID;
                else
                    Notes_Field_Name = Hierarchy + Pass_MS_Entity.Year + Pass_MS_Entity.App_no + Pass_MS_Entity.Service_plan.Trim() + Pass_MS_Entity.SPM_Seq + Pass_MS_Entity.Branch.Trim() +
                            Pass_MS_Entity.Group.ToString() + "MS" + Pass_MS_Entity.MS_Code.Trim() + CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Act_Date.Value.ToShortDateString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                List<string> list = new List<string>();
                List<CommonEntity> SelEntity = new List<CommonEntity>();
                if (CAMS_FLG == "CA")
                {
                    if (SP_CA_Grid.Rows.Count > 0)
                    {
                        foreach (DataGridViewRow dr in SP_CA_Grid.Rows)
                        {
                            if (dr.Cells["Sel_1"].Value.ToString() == true.ToString())
                            {
                                if (Pass_CA_Entity.ACT_Code.Trim() == dr.Cells["SP2_CAMS_Code"].Value.ToString().Trim())
                                { dr.Cells["SP2_CA_Seq"].Value = Pass_CA_Entity.ACT_Seq; dr.Cells["SP2_CAMS_ID"].Value = Pass_CA_Entity.ACT_ID; }

                                string Notes = string.Empty;
                                Notes = Hierarchy + Pass_CA_Entity.Year + Pass_CA_Entity.App_no + Pass_CA_Entity.Service_plan.Trim() + Pass_CA_Entity.SPM_Seq + dr.Cells["SP2_Branch"].Value.ToString() +
                                                dr.Cells["SP2_Group"].Value.ToString() + "CA" + dr.Cells["SP2_CAMS_Code"].Value.ToString().Trim() + dr.Cells["SP2_CA_Seq"].Value.ToString() + dr.Cells["SP2_CAMS_ID"].Value.ToString();

                                list.Add(Notes);

                                SelEntity.Add(new CommonEntity(dr.Cells["SP2_CAMS_Code"].Value.ToString().Trim(), dr.Cells["SP2_Desc"].Value.ToString().Trim(), Notes, "CA"));

                            }
                        }
                    }

                    if (CASEMSList.Count > 0)
                    {
                        foreach (CASEMSEntity MSEntity in CASEMSList)
                        {
                            if (!string.IsNullOrEmpty(MSEntity.MS_TrigCode.Trim()) && MSEntity.Rec_Type == "I")
                            {
                                string Notes = string.Empty;
                                Notes = Hierarchy + MSEntity.Year + MSEntity.App_no + MSEntity.Service_plan.Trim() + MSEntity.SPM_Seq + MSEntity.Branch +
                                        MSEntity.Group + "MS" + MSEntity.MS_Code.Trim() + CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Act_Date.Value.ToShortDateString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);

                                list.Add(Notes);

                                SelEntity.Add(new CommonEntity(MSEntity.MS_Code.Trim(), MSEntity.MSDesc.Trim(), Notes, "MS"));
                            }
                        }
                    }

                    if (Mode == "Add")
                    {
                        if (CA_Template_List.Count > 0)
                        {
                            foreach (CASEACTEntity Entity in CA_Template_List)
                            {
                                string Notes = string.Empty;
                                Notes = Hierarchy + Pass_CA_Entity.Year + Pass_CA_Entity.App_no + Pass_CA_Entity.Service_plan.Trim() + Pass_CA_Entity.SPM_Seq + Entity.Branch.ToString() +
                                                Entity.Group.ToString() + "CA" + Entity.ACT_Code.ToString().Trim() + Entity.ACT_Seq.ToString() + Entity.ACT_ID.ToString();

                                list.Add(Notes);

                                //string CAMS_Desc = string.Empty;
                                //CAMS_Desc = SP_CAMS_Details.Find(u => u.Type1 == "CA" && u.CamCd.Trim() == Entity.ACT_Code.Trim()).CAMS_Desc.Trim();

                                SelEntity.Add(new CommonEntity(Entity.ACT_Code.ToString().Trim(), Entity.CADesc.Trim(), Notes, "CA"));
                            }
                        }

                        if (MS_Template_List.Count > 0)
                        {
                            if (CA_Template_List.Count > 0)
                            {
                                foreach (CASEMSEntity Entity in MS_Template_List)
                                {
                                    string Notes = string.Empty;
                                    Notes = Hierarchy + Pass_CA_Entity.Year + Pass_CA_Entity.App_no + Pass_CA_Entity.Service_plan.Trim() + Pass_CA_Entity.SPM_Seq + Entity.Branch +
                                            Entity.Group + "MS" + Entity.MS_Code.Trim() + CommonFunctions.ChangeDateFormat(Convert.ToDateTime(Act_Date.Value.ToShortDateString()).ToShortDateString(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);


                                    list.Add(Notes);

                                    string CAMS_Desc = string.Empty;
                                    CAMS_Desc = SP_CAMS_Details.Find(u => u.Type1 == "CA" && u.CamCd.Trim() == Entity.MS_Code.Trim()).CAMS_Desc.Trim();

                                    SelEntity.Add(new CommonEntity(Entity.MS_Code.ToString().Trim(), Entity.MSDesc.Trim(), Notes, "CA"));
                                }
                            }
                        }
                    }
                }

                if (SP_CA_Grid.Rows.Count > 0 && SelEntity.Count > 1)
                {
                    ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, Mode, Privileges, Notes_Field_Name, list, "QuickPost", SelEntity);
                    Prog_Form.FormClosed += new FormClosedEventHandler(On_PROGNOTES_Closed);
                    Prog_Form.StartPosition = FormStartPosition.CenterScreen;
                    Prog_Form.ShowDialog();
                }
                else if (Mode == "Add" && SelEntity.Count > 1)
                {
                    ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, Mode, Privileges, Notes_Field_Name, list, "QuickPost", SelEntity);
                    Prog_Form.FormClosed += new FormClosedEventHandler(On_PROGNOTES_Closed);
                    Prog_Form.StartPosition = FormStartPosition.CenterScreen;
                    Prog_Form.ShowDialog();
                }
                else
                {
                    ProgressNotes_Form Prog_Form = new ProgressNotes_Form(BaseForm, Mode, Privileges, Notes_Field_Name);
                    Prog_Form.FormClosed += new FormClosedEventHandler(On_PROGNOTES_Closed);
                    Prog_Form.StartPosition = FormStartPosition.CenterScreen;
                    Prog_Form.ShowDialog();
                }
            }
            else if (e.Tool.Name == "tlHelp")
            {
                if (CAMS_Desc.Contains("Auto Post – "))
                { 
                   
                }
                else
                {
                    Application.Navigate(CommonFunctions.BuildHelpURLS(Privileges.Program, 5, BaseForm.BusinessModuleID.ToString()), target: "_blank");
                }
            }

        }

        private void gvMSgrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int columnIndex = gvMSgrid.CurrentCell.ColumnIndex;
                int rowIndex = gvMSgrid.CurrentCell.RowIndex;



                //gvMSgrid.SelectedRows[0].Cells[3].Value =  objArgs.MenuItem.Text;
                //gvMSgrid.SelectedRows[0].Cells[3].Tag = objArgs.MenuItem.Tag;
                //gvMSgrid.SelectedRows[0].Cells["gvMsResult"].Value = objArgs.MenuItem.Text;
                //gvMSgrid.SelectedRows[0].Cells["gvMSResultCd"].Value = objArgs.MenuItem.Tag;

                //if (CASEMSList.Count > 0)
                //{
                //    foreach (CASEMSEntity Entity in CASEMSList)
                //    {
                //        if (Entity.MS_Code.Trim() == gvMSgrid.SelectedRows[0].Cells["MS_SP2_CAMS_Code"].Value.ToString() && Entity.Branch.Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Branch"].Value.ToString().Trim() && Entity.Curr_Grp.ToString().Trim() == gvMSgrid.SelectedRows[0].Cells["MSSP2_Curr_Grp"].Value.ToString().Trim())
                //        {
                //            Entity.Result = objArgs.MenuItem.Tag.ToString();
                //            Entity.MS_TrigCode = objArgs.MenuItem.Text;
                //            if (gvMSgrid.SelectedRows[0].Cells["MSSel_1"].Value.ToString() == "true")
                //                Entity.Rec_Type = "I";
                //            break;
                //        }
                //    }
                //}





            }
        }

        private void btnQStatus_Click(object sender, EventArgs e)
        {
            //QuickPostStatusForm frm = new QuickPostStatusForm(BaseForm,Privileges,CA_Template_List,MS_Template_List);
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.ShowDialog();
        }

        private void Fill_SAL_Custom_Questions()
        {
            SaldefEntity Search_saldef_Entity = new SaldefEntity(true);

            List<SaldefEntity> SALDEF = _model.SALDEFData.Browse_SALDEF(Search_saldef_Entity, "Browse", BaseForm.UserID, BaseForm.BaseAgency);

            //if (SALDEF.Count > 0)
            //{
            //    SALDEF = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
            //}

            List<SaldefEntity> SALDEFEntity = new List<SaldefEntity>();
            if (SALDEF.Count > 0)
            {

                if (BaseForm.BaseAgencyControlDetails.ServicePlanHiecontrol.Trim() == "Y")
                {
                    if (!string.IsNullOrEmpty(Pass_CA_Entity.Act_PROG.Trim()))
                    {
                        SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                        if (SALDEFEntity.Count == 0)
                            SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                        if (SALDEFEntity.Count == 0)
                            SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG) || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 4) + "**") || u.SALD_HIE.Contains(Pass_CA_Entity.Act_PROG.Substring(0, 2) + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Equals(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                    }
                    else
                    {
                        SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Contains(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                        if (SALDEFEntity.Count == 0)
                            SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                        if (SALDEFEntity.Count == 0)
                            SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Equals(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                        //SALDEF = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_TYPE.Equals("S"));
                    }
                }
                else
                {
                    SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Contains(Pass_CA_Entity.ACT_Code.Trim()) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                    if (SALDEFEntity.Count == 0)
                        SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Contains(Pass_CA_Entity.Service_plan.Trim()) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                    if (SALDEFEntity.Count == 0)
                        SALDEFEntity = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_SPS.Equals(string.Empty) && u.SALD_SERVICES.Equals(string.Empty) && u.SALD_TYPE.Equals("S") && u.SALD_5QUEST == "Y");
                    //SALDEF = SALDEF.FindAll(u => (u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg) || u.SALD_HIE.Contains(BaseForm.BaseAgency + BaseForm.BaseDept + "**") || u.SALD_HIE.Contains(BaseForm.BaseAgency + "****") || u.SALD_HIE.Contains("******")) && u.SALD_TYPE.Equals("S"));
                }


            }

            //Added by sudheer on 3/15/2022
            SALQUESEntity = new List<SalquesEntity>();
            if (SALDEFEntity.Count > 0)
            {
                SalquesEntity Search_Salques_Entity = new SalquesEntity(true);
                Search_Salques_Entity.SALQ_SALD_ID = SALDEFEntity[0].SALD_ID;
                SALQUESEntity = _model.SALDEFData.Browse_SALQUES(Search_Salques_Entity, "Browse");

                

                    if (SALQUESEntity.Count > 0) SALQUESEntity = SALQUESEntity.OrderBy(u => Convert.ToInt32(u.SALQ_GRP_SEQ)).ThenBy(u => Convert.ToInt32(u.SALQ_SEQ)).ThenBy(u => Convert.ToInt32(u.SALQ_GRP_CODE)).ToList();
            }

            SalqrespEntity Search_Salqresp_Entity = new SalqrespEntity(true);
            SALQUESRespEntity = _model.SALDEFData.Browse_SALQRESP(Search_Salqresp_Entity, "Browse");

            if (SALQUESEntity.Count > 0)
            {
                int rowIndex = 0, Tmp_Row_Cnt = 0;
                bool DropDown_Exists = false, IS_Que_Req = false;
                string Tmp_Cust_Resp = null, Tmp_Cust_Resp_Code = null, Tmp_Cust_Resp_Desc = "";


                /**********************************************/
                if (Cust_Grid.Columns.Count == 0)
                {
                    int y = 0;
                    if (SALQUESEntity.Count == 1)
                    {
                        if (SALQUESEntity[0].SALQ_SEQ != "0")
                            y = 0;
                        else
                            y = 1;
                    }
                    if (y == 0)
                    {
                        Cust_Grid.Columns.Add("Ques");
                        Cust_Grid.Columns.Add("Resp");
                        Cust_Grid.Columns.Add("CA_Cust_Req");
                        Cust_Grid.Columns.Add("Type");
                        Cust_Grid.Columns.Add("Code");
                        Cust_Grid.Columns.Add("Resp_Code");
                    }
                }
                /**********************************************/


                foreach (SalquesEntity Entity in SALQUESEntity)
                {
                    if (Entity.SALQ_SEQ != "0")
                    {

                        rowIndex = Cust_Grid.Rows.Add();

                        #region ColumnHeaders
                        for (int x = 0; x < 6; x++)
                        {
                            int HeadcolIndex = x;


                            DataGridViewCell gvCell = new DataGridViewCell();
                            this.Cust_Grid[HeadcolIndex, rowIndex] = gvCell;
                            this.Cust_Grid[HeadcolIndex, rowIndex].Value = string.Empty;
                            gvCell.Style.Padding = new System.Windows.Forms.Padding(2);
                            this.Cust_Grid.Columns[HeadcolIndex].Visible = false;
                            this.Cust_Grid.Columns[HeadcolIndex].HeaderText = "";

                            if (x == 0)
                            {
                                this.Cust_Grid.Columns[HeadcolIndex].Visible = true;
                                this.Cust_Grid.Columns[HeadcolIndex].HeaderText = "Question";
                                this.Cust_Grid.Columns[HeadcolIndex].Width = 380;
                                this.Cust_Grid.Columns[HeadcolIndex].HeaderStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            }
                            if (x == 1)
                            {
                                this.Cust_Grid.Columns[HeadcolIndex].Visible = true;
                                this.Cust_Grid.Columns[HeadcolIndex].HeaderText = "Response";
                                this.Cust_Grid.Columns[HeadcolIndex].Width = 350;
                                this.Cust_Grid.Columns[HeadcolIndex].HeaderStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            }
                            if (x == 2)
                            {
                                this.Cust_Grid.Columns[HeadcolIndex].Visible = true;
                                this.Cust_Grid.Columns[HeadcolIndex].HeaderText = "Req";
                                this.Cust_Grid.Columns[HeadcolIndex].Width = 54;
                                this.Cust_Grid.Columns[HeadcolIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                this.Cust_Grid.Columns[HeadcolIndex].HeaderStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                this.Cust_Grid.Columns[HeadcolIndex].DefaultCellStyle.ForeColor = Color.Red;
                            }
                        }
                        #endregion


                        Tmp_Cust_Resp = " ";
                        if (Entity.SALQ_ID.Equals(Pass_CA_Entity.Cust_Code1))
                            Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value1;
                        else
                            if (Entity.SALQ_ID.Equals(Pass_CA_Entity.Cust_Code2))
                            Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value2;
                        else
                                if (Entity.SALQ_ID.Equals(Pass_CA_Entity.Cust_Code3))
                            Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value3;
                        else
                                    if (Entity.SALQ_ID.Equals(Pass_CA_Entity.Cust_Code4))
                            Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value4;
                        else
                                        if (Entity.SALQ_ID.Equals(Pass_CA_Entity.Cust_Code5))
                            Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value5;

                        Tmp_Cust_Resp_Desc = Tmp_Cust_Resp;
                        if (Entity.SALQ_TYPE == "D")
                        {
                            foreach (SalqrespEntity Ent in SALQUESRespEntity)
                            {
                                if (Entity.SALQ_ID == Ent.SALQR_Q_ID && Tmp_Cust_Resp == Ent.SALQR_CODE)
                                {
                                    Tmp_Cust_Resp_Desc = Ent.SALQR_DESC;
                                    break;
                                }
                            }
                        }
                        else if (Entity.SALQ_TYPE == "C")
                        {
                            string custQuestionResp = string.Empty;
                            List<SalqrespEntity> selRespEntity = SALQUESRespEntity.FindAll(u => u.SALQR_Q_ID.Equals(Entity.SALQ_ID));

                            if (selRespEntity.Count > 0)
                            {
                                string response1 = Tmp_Cust_Resp;
                                if (!string.IsNullOrEmpty(response1))
                                {
                                    string[] arrResponse = null;
                                    if (response1.IndexOf(',') > 0)
                                    {
                                        arrResponse = response1.Split(',');
                                    }
                                    else if (!response1.Equals(string.Empty))
                                    {
                                        arrResponse = new string[] { response1 };
                                    }


                                    foreach (string stringitem in arrResponse)
                                    {
                                        SalqrespEntity custRespEntity = selRespEntity.Find(u => u.SALQR_CODE.Trim().Equals(stringitem.Trim()));
                                        if (custRespEntity != null)
                                        {
                                            custQuestionResp += custRespEntity.SALQR_DESC + ", ";
                                            //custQuestionCode += custResp.ACTMULTRESP.ToString() + " ";
                                        }

                                    }

                                    Tmp_Cust_Resp_Desc = custQuestionResp.Trim().TrimEnd(',');
                                }
                            }
                        }

                        IS_Que_Req = false;
                        //foreach (CustomQuestionsEntity Cust_Fld in custQuestions)
                        //{
                        //    if (Entity.CustCode == Cust_Fld.CUSTCODE && Cust_Fld.CUSTREQUIRED == "Y")
                        //    {
                        //        IS_Que_Req = true;
                        //        break;
                        //    }
                        //}
                        if (Entity.SALQ_REQ.Trim() == "Y") IS_Que_Req = true;


                        Cust_Grid.Rows[rowIndex].Cells["Ques"].Value = Entity.SALQ_DESC;
                        /*************************************************************************************************/
                        /************************************** MULTI CONTROLS ***************************************************/
                        /*************************************************************************************************/

                        if (Entity.SALQ_TYPE == "X")
                        {
                            DataGridViewTextBoxCell TextBoxCell = new DataGridViewTextBoxCell();
                            this.Cust_Grid["Resp", rowIndex] = TextBoxCell;
                            this.Cust_Grid["Resp", rowIndex].Value = "";
                            this.Cust_Grid["Resp", rowIndex].ToolTipText = "Question Type: Text";
                            TextBoxCell.Style.CssStyle = "border:1px solid #ccc; border-radius:2px;";

                            Cust_Grid.Rows[rowIndex].Cells["Resp"].Value = Tmp_Cust_Resp_Desc;
                        }

                        if (Entity.SALQ_TYPE == "T")
                        {
                            DataGridViewDateTimePickerCell Response = new DataGridViewDateTimePickerCell();
                            Response.Format = DateTimePickerFormat.Short;
                            Response.Style.BackgroundImageSource = "icon-calendar";
                            Response.Style.BackgroundImageAlign = System.Drawing.ContentAlignment.MiddleRight;
                            this.Cust_Grid["Resp", rowIndex] = Response;
                            this.Cust_Grid["Resp", rowIndex].Value = string.Empty;
                            this.Cust_Grid["Resp", rowIndex].ToolTipText = "Question Type: Date";
                            Response.Style.CssStyle = "border:1px solid #ccc; border-radius:2px;";

                            if (Tmp_Cust_Resp_Desc.Trim() != "")
                                Cust_Grid.Rows[rowIndex].Cells["Resp"].Value = Convert.ToDateTime(Tmp_Cust_Resp_Desc.Trim()).ToString("MM/dd/yyyy");
                            else
                                Cust_Grid.Rows[rowIndex].Cells["Resp"].Value = Tmp_Cust_Resp_Desc;

                        }
                        if (Entity.SALQ_TYPE == "N")
                        {
                            DataGridViewTextBoxCell Response = new DataGridViewTextBoxCell();
                            this.Cust_Grid["Resp", rowIndex] = Response;
                            this.Cust_Grid["Resp", rowIndex].Value = string.Empty;
                            this.Cust_Grid["Resp", rowIndex].ToolTipText = "Question Type: Numeric";
                            Response.Style.CssStyle = "border:1px solid #ccc; border-radius:2px;";

                            Cust_Grid.Rows[rowIndex].Cells["Resp"].Value = Tmp_Cust_Resp_Desc;
                        }
                        if (Entity.SALQ_TYPE == "D")
                        {
                            DataGridViewComboBoxCell ComboBoxCell = new DataGridViewComboBoxCell();
                            ComboBoxCell.Style.BackgroundImageSource = "combo-arrow";
                            ComboBoxCell.Style.BackgroundImageAlign = System.Drawing.ContentAlignment.MiddleRight;
                            ComboBoxCell.Style.CssStyle = "border:1px solid #ccc; border-radius:2px; ";

                            this.Cust_Grid["Resp", rowIndex] = ComboBoxCell;
                            this.Cust_Grid["Resp", rowIndex].ToolTipText = "Question Type: Drop down";

                            List<SalqrespEntity> SelQuesResp = SALQUESRespEntity.FindAll(u => u.SALQR_Q_ID.Equals(Entity.SALQ_ID.ToString()));
                            if (SelQuesResp.Count > 0)
                            {
                                ComboBoxCell.DataSource = SelQuesResp;
                                ComboBoxCell.DisplayMember = "SALQR_DESC";
                                ComboBoxCell.ValueMember = "SALQR_CODE";
                            }

                            if (Tmp_Cust_Resp_Desc.Trim() != "")
                            {
                                if (SelQuesResp.Count > 0)
                                {
                                    List<SalqrespEntity> SelVal = SelQuesResp.FindAll(x => x.SALQR_CODE.Trim().ToString() == Tmp_Cust_Resp_Desc.ToString());
                                    if (SelVal.Count > 0)
                                    {
                                        Tmp_Cust_Resp_Desc = SelVal[0].SALQR_DESC.Trim();
                                    }
                                }
                            }

                            Cust_Grid.Rows[rowIndex].Cells["Resp"].Value = Tmp_Cust_Resp_Desc;

                        }
                        if (Entity.SALQ_TYPE == "C")
                        {
                            DataGridViewButtonCell Response = new DataGridViewButtonCell();
                            Response.Style.ForeColor = System.Drawing.Color.White;
                            this.Cust_Grid["Resp", rowIndex] = Response;
                            this.Cust_Grid["Resp", rowIndex].Value = string.Empty;
                            this.Cust_Grid["Resp", rowIndex].ToolTipText = "Question Type: Check Box";

                            Cust_Grid.Rows[rowIndex].Cells["Resp"].Value = Tmp_Cust_Resp_Desc;

                        }




                        Cust_Grid.Rows[rowIndex].Cells["CA_Cust_Req"].Value = (IS_Que_Req ? "*" : "");
                        Cust_Grid.Rows[rowIndex].Cells["Type"].Value = Entity.SALQ_TYPE;
                        Cust_Grid.Rows[rowIndex].Cells["Code"].Value = Entity.SALQ_ID;
                        Cust_Grid.Rows[rowIndex].Cells["Resp_Code"].Value = Tmp_Cust_Resp;
                        /*************************************************************************************************/
                        /*************************************************************************************************/


                        //rowIndex = Cust_Grid.Rows.Add(Entity.SALQ_DESC, Tmp_Cust_Resp_Desc, (IS_Que_Req ? "*" : ""), Entity.SALQ_TYPE, Entity.SALQ_ID, Tmp_Cust_Resp);
                        Tmp_Row_Cnt++;

                        set_Cust_Grid_Tooltip(rowIndex, Entity.SALQ_TYPE);

                        //if (Entity.SALQ_TYPE == "C" || Entity.SALQ_TYPE == "D")
                        //{
                        //    DropDown_Exists = true;
                        //    Cust_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue;
                        //    Cust_Grid.Rows[rowIndex].Cells["Resp"].ReadOnly = true;
                        //}

                        if (Tmp_Row_Cnt == 5) //changed from 3 to 5 On 11/20/2014
                            break;
                    }
                }
            }


            //if (SALDEF.Count>0)
            //{ 
            //    SalquesEntity Search_Salques_Entity = new SalquesEntity(true);
            //    Search_Salques_Entity.SALQ_SALD_ID = SALDEF[0].SALD_ID;
            //    SALQUESEntity = _model.SALDEFData.Browse_SALQUES(Search_Salques_Entity, "Browse");

            //    SalqrespEntity Search_Salqresp_Entity = new SalqrespEntity(true);
            //    SALQUESRespEntity = _model.SALDEFData.Browse_SALQRESP(Search_Salqresp_Entity, "Browse");

            //    if (SALQUESEntity.Count > 0)
            //    {
            //        int rowIndex = 0, Tmp_Row_Cnt = 0;
            //        bool DropDown_Exists = false, IS_Que_Req = false;
            //        string Tmp_Cust_Resp = null, Tmp_Cust_Resp_Code = null, Tmp_Cust_Resp_Desc = "";
            //        foreach (SalquesEntity Entity in SALQUESEntity)
            //        {
            //            if (Entity.SALQ_SEQ != "0")
            //            {
            //                Tmp_Cust_Resp = " ";
            //                if (Entity.SALQ_ID.Equals(Pass_CA_Entity.Cust_Code1))
            //                    Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value1;
            //                else
            //                    if (Entity.SALQ_ID.Equals(Pass_CA_Entity.Cust_Code2))
            //                    Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value2;
            //                else
            //                        if (Entity.SALQ_ID.Equals(Pass_CA_Entity.Cust_Code3))
            //                    Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value3;
            //                else
            //                            if (Entity.SALQ_ID.Equals(Pass_CA_Entity.Cust_Code4))
            //                    Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value4;
            //                else
            //                                if (Entity.SALQ_ID.Equals(Pass_CA_Entity.Cust_Code5))
            //                    Tmp_Cust_Resp = Pass_CA_Entity.Cust_Value5;

            //                Tmp_Cust_Resp_Desc = Tmp_Cust_Resp;
            //                if (Entity.SALQ_TYPE == "D")
            //                {
            //                    foreach (SalqrespEntity Ent in SALQUESRespEntity)
            //                    {
            //                        if (Entity.SALQ_ID == Ent.SALQR_Q_ID && Tmp_Cust_Resp == Ent.SALQR_CODE)
            //                        {
            //                            Tmp_Cust_Resp_Desc = Ent.SALQR_DESC;
            //                            break;
            //                        }
            //                    }
            //                }
            //                else if (Entity.SALQ_TYPE == "C")
            //                {
            //                    string custQuestionResp = string.Empty;
            //                    List<SalqrespEntity> selRespEntity = SALQUESRespEntity.FindAll(u => u.SALQR_Q_ID.Equals(Entity.SALQ_ID));

            //                    if (selRespEntity.Count > 0)
            //                    {
            //                        string response1 = Tmp_Cust_Resp;
            //                        if (!string.IsNullOrEmpty(response1))
            //                        {
            //                            string[] arrResponse = null;
            //                            if (response1.IndexOf(',') > 0)
            //                            {
            //                                arrResponse = response1.Split(',');
            //                            }
            //                            else if (!response1.Equals(string.Empty))
            //                            {
            //                                arrResponse = new string[] { response1 };
            //                            }


            //                            foreach (string stringitem in arrResponse)
            //                            {
            //                                SalqrespEntity custRespEntity = selRespEntity.Find(u => u.SALQR_CODE.Trim().Equals(stringitem.Trim()));
            //                                if (custRespEntity != null)
            //                                {
            //                                    custQuestionResp += custRespEntity.SALQR_DESC + ", ";
            //                                    //custQuestionCode += custResp.ACTMULTRESP.ToString() + " ";
            //                                }

            //                            }

            //                            Tmp_Cust_Resp_Desc = custQuestionResp;
            //                        }
            //                    }
            //                }

            //                IS_Que_Req = false;
            //                //foreach (CustomQuestionsEntity Cust_Fld in custQuestions)
            //                //{
            //                //    if (Entity.CustCode == Cust_Fld.CUSTCODE && Cust_Fld.CUSTREQUIRED == "Y")
            //                //    {
            //                //        IS_Que_Req = true;
            //                //        break;
            //                //    }
            //                //}
            //                if (Entity.SALQ_REQ == "Y") IS_Que_Req = true;

            //                rowIndex = Cust_Grid.Rows.Add(Entity.SALQ_DESC, Tmp_Cust_Resp_Desc, (IS_Que_Req ? "*" : ""), Entity.SALQ_TYPE, Entity.SALQ_ID, Tmp_Cust_Resp);
            //                Tmp_Row_Cnt++;

            //                set_Cust_Grid_Tooltip(rowIndex, Entity.SALQ_TYPE);

            //                //if (Entity.SALQ_TYPE == "C" || Entity.SALQ_TYPE == "D")
            //                //{
            //                //    DropDown_Exists = true;
            //                //    Cust_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Blue;
            //                //    Cust_Grid.Rows[rowIndex].Cells["Resp"].ReadOnly = true;
            //                //}

            //                if (Tmp_Row_Cnt == 5) //changed from 3 to 5 On 11/20/2014
            //                    break;
            //            }
            //        }
            //    }
            //}

        }

        private void CA_Members_Grid_Click(object sender, EventArgs e)
        {

        }

        List<SPCommonEntity> SPEntity = new List<SPCommonEntity>();
        private void FillResultscombo()
        {
            DataGridViewComboBoxColumn cb = (DataGridViewComboBoxColumn)this.gvMSgrid.Columns["gvMsResult"];
            List<SPCommonEntity> ResultsList = new List<SPCommonEntity>();
            ResultsList = _model.SPAdminData.Get_AgyRecs("Results");
           
            SPEntity.Clear();
            if (ResultsList.Count > 0)
            {
                if (!string.IsNullOrEmpty(SP_Header_Rec.Status))
                {
                    bool Ststus_Exists = false; int Pos = 0, Tmp_Loop_Cnt = 0, Tmp_Curr_Status_Len = 0;
                    string Status_Str = SP_Header_Rec.Status;

                    foreach (SPCommonEntity Entity in ResultsList)
                    {
                        Ststus_Exists = false; Pos = 0;
                        for (int i = 0; Pos < SP_Header_Rec.Status.Length; i++)
                        {
                            Tmp_Curr_Status_Len = (Status_Str.Substring(Pos, Status_Str.Substring(Pos, (Status_Str.Length - Pos)).Length)).Length;

                            if (Entity.Code == SP_Header_Rec.Status.Substring(Pos, (Tmp_Curr_Status_Len >= 4 ? 4 : Tmp_Curr_Status_Len)).Trim())
                            {
                                Ststus_Exists = true; break;
                            }
                            Pos += 4;
                        }

                        if (Ststus_Exists)
                        {   
                            if (Mode == "Edit" || (Mode == "Add" && Entity.Active.Equals("Y")))
                            {
                                SPEntity.Add(Entity);

                                //MenuItem menuItem = new MenuItem();
                                //menuItem.Text = Entity.Desc;
                                //menuItem.Tag = Entity.Code;
                                //contextMenu_Result.MenuItems.Add(menuItem);
                                //Tmp_Loop_Cnt++;
                            }
                        }
                    }

                    if (SPEntity.Count > 0)
                    {
                        //CASESP2Entity Entity = new CASESP2Entity();
                        //Entity.CAMS_Desc = ""; Entity.CamCd = "0";
                        //CA_Details.Add(new CASESP2Entity(Entity));
                        //CADetailsEntity = CA_Details;

                        cb.DataSource = SPEntity;
                        cb.DisplayMember = "Desc";
                        cb.ValueMember = "Code";
                        cb = (DataGridViewComboBoxColumn)this.gvMSgrid.Columns["gvMsResult"];
                    }

                }
            }
        }

        private void Cust_Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (e.ColumnIndex == 1 && e.RowIndex != -1)
            {
                if (senderGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].CellRenderer == "ButtonCell" && e.RowIndex != -1)
                {
                    string response = Cust_Grid.Rows[e.RowIndex].Cells[5].Value != null ? Cust_Grid.Rows[e.RowIndex].Cells[5].Value.ToString() : string.Empty;
                    //gvQuestions.SelectedRows[0].Cells[8].Tag != null ? gvQuestions.SelectedRows[0].Cells[8].Tag.ToString() : string.Empty;
                    PrivilegeEntity privileges = new PrivilegeEntity();
                    privileges.AddPriv = "true";
                    AlertCodeForm objform = new AlertCodeForm(BaseForm, privileges, response, Cust_Grid.Rows[e.RowIndex].Cells["Code"].Value.ToString(), Cust_Grid.Rows[e.RowIndex].Cells["Ques"].Value.ToString(), string.Empty);
                    objform.FormClosed += new FormClosedEventHandler(objform_FormClosed);
                    objform.StartPosition = FormStartPosition.CenterScreen;
                    objform.ShowDialog();
                }
            }
        }

        private void gvMSgrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (gvMSgrid.Rows.Count > 0)
            {
                if (CASEMSList.Count > 0)
                {
                    foreach (CASEMSEntity Entity in CASEMSList)
                    {
                        if (Entity.MS_Code.Trim() == gvMSgrid.Rows[e.RowIndex].Cells["MS_SP2_CAMS_Code"].Value.ToString() && Entity.Branch.Trim() == gvMSgrid.Rows[e.RowIndex].Cells["MSSP2_Branch"].Value.ToString().Trim() && Entity.Curr_Grp.ToString().Trim() == gvMSgrid.Rows[e.RowIndex].Cells["MSSP2_Curr_Grp"].Value.ToString().Trim())
                        {
                            Entity.Result = gvMSgrid.Rows[e.RowIndex].Cells["gvMsResult"].Value.ToString();
                            Entity.MS_TrigCode = gvMSgrid.Rows[e.RowIndex].Cells["gvMsResult"].Value.ToString();
                            if (gvMSgrid.Rows[e.RowIndex].Cells["MSSel_1"].Value.ToString() == "true")
                                Entity.Rec_Type = "I";
                            break;
                        }
                    }
                }
            }
            //gvMSgrid.SelectedRows[0].Cells[3].Value = objArgs.MenuItem.Text;
            //gvMSgrid.SelectedRows[0].Cells[3].Tag = objArgs.MenuItem.Tag;
            //gvMSgrid.SelectedRows[0].Cells["gvMsResult"].Value = objArgs.MenuItem.Text;
            //gvMSgrid.SelectedRows[0].Cells["gvMSResultCd"].Value = objArgs.MenuItem.Tag;

            
        }

        private void Fill_Results()
        {
            Cmb_MS_Results.Items.Clear(); Cmb_MS_Results.ColorMember = "FavoriteColor";

            Cmb_MS_Results.Items.Insert(0, new Captain.Common.Utilities.ListItem("    ", "0", " ", Color.White));
            Cmb_MS_Results.SelectedIndex = 0;


            if (!string.IsNullOrEmpty(SP_Header_Rec.Status))
            {
                bool Ststus_Exists = false; int Pos = 0, Tmp_Loop_Cnt = 0, Tmp_Curr_Status_Len = 0;
                string Status_Str = SP_Header_Rec.Status;
                List<SPCommonEntity> ResultsList = new List<SPCommonEntity>();
                ResultsList = _model.SPAdminData.Get_AgyRecs("Results");

                foreach (SPCommonEntity Entity in ResultsList)
                {
                    Ststus_Exists = false; Pos = 0;
                    for (int i = 0; Pos < SP_Header_Rec.Status.Length; i++)
                    {
                        Tmp_Curr_Status_Len = (Status_Str.Substring(Pos, Status_Str.Substring(Pos, (Status_Str.Length - Pos)).Length)).Length;

                        if (Entity.Code == SP_Header_Rec.Status.Substring(Pos, (Tmp_Curr_Status_Len >= 4 ? 4 : Tmp_Curr_Status_Len)).Trim())
                        {
                            Ststus_Exists = true; break;
                        }
                        Pos += 4;
                    }

                    if (Ststus_Exists)
                    {
                        if (Mode == "Edit" || (Mode == "Add" && Entity.Active.Equals("Y")))
                        {
                            Cmb_MS_Results.Items.Add(new Captain.Common.Utilities.ListItem(Entity.Desc, Entity.Code, Entity.Active, (Entity.Active.Equals("Y") ? Color.Black : Color.Red)));
                            Tmp_Loop_Cnt++;
                        }
                    }
                }

                Cmb_MS_Results.SelectedIndex = 0;


            }
        }

    }
}