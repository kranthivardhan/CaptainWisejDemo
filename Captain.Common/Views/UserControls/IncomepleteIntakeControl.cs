#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using Wisej.Web;
using Captain.Common.Model.Data;
using Captain.Common.Views.UserControls.Base;
using Captain.Common.Views.Forms.Base;
using Captain.Common.Model.Objects;
//using Gizmox.WebGUI.Common.Resources;
using Captain.Common.Utilities;
using Captain.Common.Exceptions;
using System.Diagnostics;
using System.Linq;
using iTextSharp.text.pdf;
using Captain.Common.Views.Forms;
using iTextSharp.text;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
//using Google.Cloud.Translation.V2;
#endregion

namespace Captain.Common.Views.UserControls
{
    public partial class IncomepleteIntakeControl : BaseUserControl
    {
        #region private variables

        private ErrorProvider _errorProvider = null;
        private string strMode = Consts.Common.View;
        private string strNameFormat = string.Empty;
        private string strVerfierFormat = string.Empty;
        CaptainModel _model = null;
        #endregion
        public IncomepleteIntakeControl(BaseForm baseForm, PrivilegeEntity privileges)
        {
            InitializeComponent();
            BaseForm = baseForm;
            Privileges = privileges;
            _model = new CaptainModel();
            _errorProvider = new ErrorProvider(this);
            _errorProvider.BlinkRate = 3;
            _errorProvider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            _errorProvider.Icon = null;
            propReportPath = _model.lookupDataAccess.GetReportPath();
            HierarchyEntity HierarchyEntity = CommonFunctions.GetHierachyNameFormat(BaseForm.BaseAgency, "**", "**");
            if (HierarchyEntity != null)
            {
                strNameFormat = HierarchyEntity.CNFormat.ToString();
                strVerfierFormat = HierarchyEntity.CWFormat.ToString();
            }
            ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            propcaseHistList = _model.CaseMstData.GetCaseHistDetails("INTKINCOMP", BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + BaseForm.BaseYear + BaseForm.BaseApplicationNo, privileges.Program);
            ProgramDefinition = programEntity;
            Fill_Deny_ReasonCodes();
            fillCombo();
            fillGriddata();
            IncompleteIntakeWarringMessages();

            PopulateToolbar(oToolbarMnustrip);
        }

        #region properties

        List<LIHEAPBEntity> LIHEAPB_List = new List<LIHEAPBEntity>();
        public BaseForm BaseForm { get; set; }

        public PrivilegeEntity Privileges { get; set; }

        public ToolBarButton ToolBarEdit { get; set; }

        public ToolBarButton ToolBarNew { get; set; }

        public ToolBarButton ToolBarDel { get; set; }

        public ToolBarButton ToolBarHelp { get; set; }

        public ToolBarButton ToolBarHistory { get; set; }


        public ToolBarButton ToolBarPrint { get; set; }

        List<CommonEntity> propfundingsource { get; set; }

        public string propReportPath { get; set; }
        MessageBoxForm objForm;
        #endregion


        public override void PopulateToolbar(ToolBar toolBar)
        {
            base.PopulateToolbar(toolBar);

            bool toolbarButtonInitialized = ToolBarNew != null;
            ToolBarButton divider = new ToolBarButton();
            divider.Style = ToolBarButtonStyle.Separator;

            if (toolBar.Controls.Count == 0)
            {
                ToolBarNew = new ToolBarButton();
                ToolBarNew.Tag = "New";
                ToolBarNew.ToolTipText = "New Incomplete Intakes";
                ToolBarNew.Enabled = true;
                ToolBarNew.ImageSource = "captain-add";
                ToolBarNew.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarNew.Click += new EventHandler(OnToolbarButtonClicked);

                ToolBarEdit = new ToolBarButton();
                ToolBarEdit.Tag = "Edit";
                ToolBarEdit.ToolTipText = "Edit Incomplete Intakes";
                ToolBarEdit.Enabled = true;
                ToolBarEdit.ImageSource = "captain-edit";
                ToolBarEdit.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarEdit.Click += new EventHandler(OnToolbarButtonClicked);

                ToolBarDel = new ToolBarButton();
                ToolBarDel.Tag = "Delete";
                ToolBarDel.ToolTipText = "Delete Incomplete Intakes";
                ToolBarDel.Enabled = true;
                ToolBarDel.ImageSource = "captain-delete";
                ToolBarDel.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarDel.Click += new EventHandler(OnToolbarButtonClicked);


                ToolBarPrint = new ToolBarButton();
                ToolBarPrint.Tag = "Print";
                ToolBarPrint.ToolTipText = "Print";
                ToolBarPrint.Enabled = true;
                ToolBarPrint.ImageSource = "captain-print";
                ToolBarPrint.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarPrint.Click += new EventHandler(OnToolbarButtonClicked);

                ToolBarHistory = new ToolBarButton();
                ToolBarHistory.Tag = "History";
                ToolBarHistory.ToolTipText = "Incomplete Intake History";
                ToolBarHistory.Enabled = true;
                ToolBarHistory.ImageSource = "captain-caseHistory";
                ToolBarHistory.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarHistory.Click += new EventHandler(OnToolbarButtonClicked);


                ToolBarHelp = new ToolBarButton();
                ToolBarHelp.Tag = "Help";
                ToolBarHelp.ToolTipText = "Help";
                ToolBarHelp.Enabled = true;
                ToolBarHelp.ImageSource = "icon-help";
                ToolBarHelp.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarHelp.Click += new EventHandler(OnToolbarButtonClicked);
            }

            if (Privileges.AddPriv.Equals("false"))
            {
                if (ToolBarNew != null) ToolBarNew.Enabled = false;
            }
            else
            {
                if (ToolBarNew != null) ToolBarNew.Enabled = true;
            }

            if (Privileges.ChangePriv.Equals("false"))
            {
                if (ToolBarEdit != null) ToolBarEdit.Enabled = false;
            }
            else
            {
                if (ToolBarEdit != null) ToolBarEdit.Enabled = true;
            }

            if (Privileges.DelPriv.Equals("false"))
            {
                if (ToolBarDel != null) ToolBarDel.Enabled = false;
            }
            else
            {
                if (ToolBarDel != null) ToolBarDel.Enabled = true;
            }


            ShowButtons();
            toolBar.Buttons.AddRange(new ToolBarButton[]
            {
                ToolBarNew,
                ToolBarEdit,
                ToolBarDel,
                ToolBarPrint,
                ToolBarHistory
            });

            //if (gvwBudget.Rows.Count == 0)
            //{
            //    if (ToolBarEdit != null)
            //        ToolBarEdit.Enabled = false;
            //    if (ToolBarDel != null)
            //        ToolBarDel.Enabled = false;
            //}
        }

        /// <summary>
        /// Handles the toolbar button clicked event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnToolbarButtonClicked(object sender, EventArgs e)
        {
            ToolBarButton btn = (ToolBarButton)sender;
            StringBuilder executeCode = new StringBuilder();

            executeCode.Append(Consts.Javascript.BeginJavascriptCode);
            if (btn.Tag == null) { return; }
            try
            {
                switch (btn.Tag.ToString())
                {
                    case Consts.ToolbarActions.New:
                        bool boolAdd = true;
                        if (LIHEAPB_List.Count > 0)
                        {
                            LIHEAPBEntity lpbData = LIHEAPB_List.Find(u => u.Seq == "0" && u.Denied_Reason == "03" && u.Certified_Status == "98");
                            if (lpbData != null)
                            {
                                AlertBox.Show("Application is Denied Over Income, you cannot create an incomplete Intake Letter", MessageBoxIcon.Warning);
                                boolAdd = false;
                            }
                            else
                            {
                                lpbData = LIHEAPB_List.Find(u => u.Seq == "0" && u.Inc_Cert == "3" && u.Certified_Status == "98");
                                if (lpbData != null)
                                {
                                    MessageBox.Show("In order to create an incomplete letter, please remove Denied record in the Control Card", Consts.Common.ApplicationCaption, MessageBoxButtons.OK, MessageBoxIcon.Question, onclose: MessageBoxHandlerControlCard);


                                    boolAdd = false;
                                }
                                else
                                {
                                    lpbData = LIHEAPB_List.Find(u => u.Seq == "0" && u.Certified_Status == "99");
                                    if (lpbData != null)
                                    {
                                        AlertBox.Show("Application is Approved, you cannot create an Incomplete Intake Letter");
                                        boolAdd = false;
                                    }
                                }
                            }
                            //else
                            //{
                            //    lpbData = LIHEAPB_List.Find(u => u.Seq == "0" && u.Certified_Status == "99");
                            //    if (lpbData != null)
                            //    {
                            //        CommonFunctions.MessageBoxDisplay("Application is Approved, you cannot create an incomplete Intake Letter");
                            //        boolAdd = false;
                            //    }

                            //}
                        }
                        if (boolAdd)
                        {
                            strMode = Consts.Common.New;

                            EnableDisableviewMode(false);
                            ShowButtons();
                            dtletterDate.Value = DateTime.Now.Date;
                            dtletterDate.Checked = false;
                            lblLetterDateReq.Visible = false;
                            CommonFunctions.SetComboBoxValue(cmbCaseWorker, strCaseWorkerDefaultCode);
                        }
                        break;
                    case Consts.ToolbarActions.Edit:
                        //if (listIntakeIncompletdata[0].LetterDate == string.Empty)
                        //{

                        bool boolEdit = true;
                        if (LIHEAPB_List.Count > 0)
                        {
                            LIHEAPBEntity lpbData = LIHEAPB_List.Find(u => u.Seq == "0" && u.Inc_Cert == "3" && u.Certified_Status == "98");
                            if (lpbData != null)
                            {
                                MessageBox.Show("In order to create an incomplete letter, please remove Denied record in the Control Card", Consts.Common.ApplicationCaption, MessageBoxButtons.OK, MessageBoxIcon.Question, onclose: MessageBoxHandlerControlCard);


                                boolEdit = false;
                            }
                        }
                        if (boolEdit)
                        {
                            strMode = Consts.Common.Edit;
                            EnableDisableviewMode(false);
                            ShowButtons();
                        }
                        //}
                        //else
                        //{
                        //    CommonFunctions.MessageBoxDisplay("You are not allowed to Edit, as the Incomplete Intake Letter is already printed on " + LookupDataAccess.Getdate(listIntakeIncompletdata[0].LetterDate));
                        //}
                        break;
                    case Consts.ToolbarActions.Delete:
                        //if (listIntakeIncompletdata.Count > 0)
                        //{
                        //    if (listIntakeIncompletdata[0].LetterDate == string.Empty)
                        //    {
                        strMode = Consts.Common.Delete;
                        MessageBox.Show(Consts.Messages.AreYouSureYouWantToDelete.GetMessage(), Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, onclose: MessageBoxHandler);
                        //    }
                        //    else
                        //    {
                        //        CommonFunctions.MessageBoxDisplay("You are not allowed to Delete, as the Incomplete Intake Letter is already printed on " + LookupDataAccess.Getdate(listIntakeIncompletdata[0].LetterDate));
                        //    }

                        //}
                        break;
                    case Consts.ToolbarActions.Print:
                        if (listIntakeIncompletdata.Count > 0)
                        {
                            bool boolEngerybutton = false;
                            if (LIHEAPB_List.Count > 0)
                            {
                                boolEngerybutton = true;
                            }
                            objForm = new MessageBoxForm(Privileges, this, boolEngerybutton);
                            objForm.StartPosition = FormStartPosition.CenterScreen;
                            objForm.FormClosed += new FormClosedEventHandler(MessageBox_Form_Closed);
                            objForm.ShowDialog();
                            //if (listIntakeIncompletdata[0].LetterDate == string.Empty)
                            //{
                            //    On_PrintLetter("Update");
                            //}
                            //else
                            //{
                            //MessageBox.Show("Incomplete Intake Letter already printed, Are you sure want update Letter Date? ", Consts.Common.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxHandlerLetterDate, true);

                            // }
                        }
                        break;
                    case Consts.ToolbarActions.History:
                        //CaseSnpEntity caseHistSnp = GetSelectedRow();
                        //if (caseHistSnp != null)
                        //{
                        HistoryForm historyForm = new HistoryForm(BaseForm, Privileges, string.Empty);
                        historyForm.StartPosition = FormStartPosition.CenterScreen;
                        historyForm.ShowDialog();
                        // }
                        break;
                    case Consts.ToolbarActions.Help:
                        // Help.ShowHelp(this, Context.Server.MapPath("~\\Resources\\HelpFiles\\Captain_Help.chm"), HelpNavigator.KeywordIndex, "EMS00010");
                        break;
                }
                executeCode.Append(Consts.Javascript.EndJavascriptCode);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogAndDisplayMessageToUser(new StackFrame(true), ex, QuantumFaults.None, ExceptionSeverityLevel.High);
            }
        }

        private void EnableDisableviewMode(bool booltruefalse)
        {
            txtLine1.ReadOnly = booltruefalse;
            gvchkSelect.ReadOnly = booltruefalse;
        }



        private void MessageBoxHandlerControlCard(DialogResult dialogResult)
        {
            // Get Gizmox.WebGUI.Forms.Form object that called MessageBox
            //Wisej.Web.Form senderForm = (Wisej.Web.Form)sender;

            //if (senderForm != null)
            //{
            // Set DialogResult value of the Form as a text for label
            //if (senderForm.DialogResult.ToString() == "OK")
            if (dialogResult == DialogResult.OK)
            {
                BaseForm.AddTabClientIntake("TMS00081");
            }
            //}
        }


        private void MessageBoxHandler(DialogResult dialogResult)
        {
            //// Get Gizmox.WebGUI.Forms.Form object that called MessageBox
            //Wisej.Web.Form senderForm = (Wisej.Web.Form)sender;

            //if (senderForm != null)
            //{
            // Set DialogResult value of the Form as a text for label
            //if (senderForm.DialogResult.ToString() == "Yes")
            if (dialogResult == DialogResult.Yes)
            {
                CustomQuestionsEntity custentity = new CustomQuestionsEntity();
                custentity.ACTAGENCY = BaseForm.BaseAgency;
                custentity.ACTDEPT = BaseForm.BaseDept;
                custentity.ACTPROGRAM = BaseForm.BaseProg;
                custentity.ACTYEAR = BaseForm.BaseYear;
                custentity.ACTAPPNO = BaseForm.BaseApplicationNo;
                custentity.Mode = "DELETE";
                if (_model.CaseMstData.CAPS_INTKINCOMP_INSUPDEL(custentity))
                {

                    string strHistoryDetails = "<XmlHistory>";
                    bool boolHistory = false;
                    bool boolSubHistory = false;
                    if (listIntakeIncompletdata.Count > 0)
                    {

                        boolHistory = true;
                        strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Letter Date</FieldName><OldValue></OldValue><NewValue>" + LookupDataAccess.Getdate(listIntakeIncompletdata[0].LetterDate.Trim()) + "</NewValue></HistoryFields>";


                        boolHistory = true;
                        strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>CASE WORKER</FieldName><OldValue></OldValue><NewValue>" + ((Utilities.ListItem)cmbCaseWorker.SelectedItem).Text.ToString() + "</NewValue></HistoryFields>";



                        foreach (DataGridViewRow gvRows in gvwIncompletedata.Rows)
                        {
                            CustomQuestionsEntity custIntakeHist = listIntakeIncompletdata.Find(u => u.ACTCODE.Trim() == gvRows.Cells["gvtCode"].Value.ToString().Trim());
                            if (custIntakeHist != null)
                            {
                                boolHistory = true;
                                if (custIntakeHist.ACTALPHARESP.Trim() != string.Empty)
                                {
                                    boolHistory = true;
                                    strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Details</FieldName><OldValue>" + custIntakeHist.ACTALPHARESP.Trim() + "</OldValue><NewValue></NewValue></HistoryFields>";
                                }

                                strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>" + gvRows.Cells["gvtDesc"].Value.ToString() + "</FieldName><OldValue></OldValue><NewValue>Incomplete Intake Reason(s) Deleted</NewValue></HistoryFields>";

                            }
                        }

                    }


                    strHistoryDetails = strHistoryDetails + "</XmlHistory>";
                    if (boolHistory)
                    {
                        CaseHistEntity caseHistEntity = new CaseHistEntity();
                        caseHistEntity.HistTblName = "INTKINCOMP";
                        caseHistEntity.HistScreen = Privileges.Program; //"CASE2005";
                        caseHistEntity.HistSubScr = "Delete";
                        caseHistEntity.HistTblKey = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + BaseForm.BaseYear + BaseForm.BaseApplicationNo;
                        caseHistEntity.LstcOperator = BaseForm.UserID;
                        caseHistEntity.HistChanges = strHistoryDetails;
                        _model.CaseMstData.InsertCaseHist(caseHistEntity);
                    }


                    fillGriddata();
                    strMode = Consts.Common.View;
                    ShowButtons();
                    AlertBox.Show("Deleted Successfully");
                }

            }
            //}
        }

        //Added by Sudheer on 11/28/2022
        private void MessageBox_Form_Closed(object sender, FormClosedEventArgs e)
        {
            MessageBoxForm form = sender as MessageBoxForm;
            if (form.DialogResult == DialogResult.OK)
            {
                string Value = form.GetValue();
                if (Value == "EnergyAssistance")
                {
                    On_PrintCTApp();
                }
                else
                {
                    DataSet dsLang = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.LANGUAGECODES);
                    string Spanish_Code = string.Empty;
                    DataTable dtLang = dsLang.Tables[0];
                    foreach (DataRow drLang in dtLang.Rows)
                    {
                        if (drLang["LookUpDesc"].ToString().Trim() == "SPANISH" || drLang["LookUpDesc"].ToString().Trim() == "Spanish")
                        {
                            Spanish_Code = drLang["Code"].ToString().Trim(); break;
                        }
                    }


                    if (Value == "Update")
                    {
                        if (BaseForm.BaseAgencyControlDetails.ReverseFeed == "Y")
                        {
                            string strfileAGY = BaseForm.BaseAgencyControlDetails.AgyShortName; //DSSXMLData.getZIPfileAGY(BaseForm.BaseAgencyControlDetails.AgyShortName);
                            DataTable dtDSSXMLData = DSSXMLData.DSSXMLMID_GET(BaseForm.BaseDSSXMLDBConnString, strfileAGY, BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, "BYAPPNO");
                            if (dtDSSXMLData.Rows.Count > 0)
                            {
                                //DSS_Letter letterDss = new DSS_Letter(BaseForm, Privileges, "INTK0001", BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, string.Empty, string.Empty, dtDSSXMLData);
                                //letterDss.StartPosition = FormStartPosition.CenterScreen;
                                //letterDss.FormClosed += new FormClosedEventHandler(MessageBoxHandlerDSS);
                                //letterDss.ShowDialog();
                            }
                            else
                            {
                                printHistory();
                                if (BaseForm.BaseCaseMstListEntity[0].Language == Spanish_Code)
                                    On_PrintLetterSpanish("Update");
                                else
                                    On_PrintLetter("Update");
                            }
                        }
                        else
                        {
                            printHistory();
                            if (BaseForm.BaseCaseMstListEntity[0].Language == Spanish_Code)
                                On_PrintLetterSpanish("Update");
                            else
                                On_PrintLetter("Update");
                        }

                    }
                    else
                    {
                        if (BaseForm.BaseCaseMstListEntity[0].Language == Spanish_Code)
                            On_PrintLetterSpanish(string.Empty);
                        else
                            On_PrintLetter(string.Empty);
                    }
                }
            }


        }

        private void MessageBoxHandlerDSS(object sender, EventArgs e)
        {
            // Get Gizmox.WebGUI.Forms.Form object that called MessageBox
            Wisej.Web.Form senderForm = (Wisej.Web.Form)sender;

            if (senderForm != null)
            {
                // Set DialogResult value of the Form as a text for label
                if (senderForm.DialogResult.ToString() == "Yes")
                {

                    fillGriddata();
                }
                else
                {
                    fillGriddata();
                }
            }
        }

        private void MessageBoxHandlerLetterDate(object sender, EventArgs e)
        {
            // Get Gizmox.WebGUI.Forms.Form object that called MessageBox
            Wisej.Web.Form senderForm = (Wisej.Web.Form)sender;

            if (senderForm != null)
            {
                // Set DialogResult value of the Form as a text for label
                if (senderForm.DialogResult.ToString() == "Yes")
                {

                    printHistory();
                    On_PrintLetter("Update");
                }
                else
                {
                    On_PrintLetter(string.Empty);
                }
            }
        }

        public void printHistory()
        {
            string strHistoryDetails = "<XmlHistory>";
            bool boolHistory = false;
            bool boolSubHistory = false;
            if (listIntakeIncompletdata.Count > 0)
            {
                if (LookupDataAccess.Getdate(listIntakeIncompletdata[0].LetterDate.Trim()) != LookupDataAccess.Getdate(DateTime.Now.Date.ToShortDateString()))
                {
                    boolHistory = true;
                    strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Letter Date</FieldName><OldValue>" + LookupDataAccess.Getdate(listIntakeIncompletdata[0].LetterDate.Trim()) + "</OldValue><NewValue>" + LookupDataAccess.Getdate(DateTime.Now.Date.ToShortDateString()) + "</NewValue></HistoryFields>";



                    strHistoryDetails = strHistoryDetails + "</XmlHistory>";
                    if (boolHistory)
                    {
                        CaseHistEntity caseHistEntity = new CaseHistEntity();
                        caseHistEntity.HistTblName = "INTKINCOMP";
                        caseHistEntity.HistScreen = Privileges.Program;//"CASE2005";
                        caseHistEntity.HistSubScr = "Print";
                        caseHistEntity.HistTblKey = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + BaseForm.BaseYear + BaseForm.BaseApplicationNo;
                        caseHistEntity.LstcOperator = BaseForm.UserID;
                        caseHistEntity.HistChanges = strHistoryDetails;
                        _model.CaseMstData.InsertCaseHist(caseHistEntity);
                    }

                    dtletterDate.Value = DateTime.Now.Date;
                    dtletterDate.Checked = true;
                    lblLetterDateReq.Visible = true;
                }
            }
        }
        List<CustomQuestionsEntity> listIntakeIncompletdata = new List<CustomQuestionsEntity>();
        void fillGriddata()
        {
            _errorProvider.SetError(dtletterDate, null);
            _errorProvider.SetError(cmbCaseWorker, null);
            LIHEAPBEntity Search_Entity = new LIHEAPBEntity(true);
            Search_Entity.Agency = BaseForm.BaseAgency;
            Search_Entity.Dept = BaseForm.BaseDept;
            Search_Entity.Prog = BaseForm.BaseProg;
            Search_Entity.Year = BaseForm.BaseYear;
            Search_Entity.AppNo = BaseForm.BaseApplicationNo;
            LIHEAPB_List = _model.LiheAllData.Browse_LIHEAPB(Search_Entity, "Browse");
            //pnlLetter.Visible = false;
            strCaseWorkerName = string.Empty;

            DataSet dsLang = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.LANGUAGECODES);
            string Spanish_Code = string.Empty;
            DataTable dtLang = dsLang.Tables[0];
            foreach (DataRow drLang in dtLang.Rows)
            {
                if (drLang["LookUpDesc"].ToString().Trim() == "SPANISH" || drLang["LookUpDesc"].ToString().Trim() == "Spanish")
                {
                    Spanish_Code = drLang["Code"].ToString().Trim(); break;
                }
            }

            listIntakeIncompletdata = new List<CustomQuestionsEntity>();
            List<CommonEntity> listIncompleteIncome = new List<CommonEntity>();
            //if(BaseForm.BaseCaseMstListEntity[0].Language.Trim()==Spanish_Code)
            //    listIncompleteIncome = CommonFunctions.AgyTabsFilterOrderbyCode(BaseForm.BaseAgyTabsEntity, "09998", BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, string.Empty); //_model.lookupDataAccess.GetHealthInsurance();
            //else
            listIncompleteIncome = CommonFunctions.AgyTabsFilterOrderbyCode(BaseForm.BaseAgyTabsEntity, "09997", BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, string.Empty); //_model.lookupDataAccess.GetHealthInsurance();
            DataSet ds = Captain.DatabaseLayer.CaseSnpData.CAPS_INTKINCOMP_GET(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dritem in ds.Tables[0].Rows)
                {
                    CustomQuestionsEntity custentity = new CustomQuestionsEntity();
                    custentity.ACTAGENCY = dritem["INTK_AGENCY"].ToString();
                    custentity.ACTDEPT = dritem["INTK_DEPT"].ToString();
                    custentity.ACTPROGRAM = dritem["INTK_PROGRAM"].ToString();
                    custentity.ACTYEAR = dritem["INTK_YEAR"].ToString();
                    custentity.ACTAPPNO = dritem["INTK_APP"].ToString();
                    custentity.ACTCODE = dritem["INTK_INCOMP"].ToString().Trim();
                    custentity.ACTALPHARESP = dritem["INTK_DETAILS"].ToString().Trim();
                    custentity.lstcdate = dritem["INTK_DATE_LSTC"].ToString();
                    custentity.lstcoperator = dritem["INTK_LSTC_OPERATOR"].ToString();
                    custentity.adddate = dritem["INTK_DATE_ADD"].ToString();
                    custentity.addoperator = dritem["INTK_ADD_OPERATOR"].ToString();
                    custentity.LetterDate = dritem["INTK_LETTER_DATE"].ToString();
                    custentity.INTKWorker = dritem["INTK_WORKER"].ToString().Trim();
                    listIntakeIncompletdata.Add(custentity);
                }
            }
            if (listIncompleteIncome.Count > 0)
            {
                gvwIncompletedata.SelectionChanged -= gvwIncompletedata_SelectionChanged;
                gvwIncompletedata.Rows.Clear();
                bool boolexist = false;
                int index = 0;
                foreach (CommonEntity dr in listIncompleteIncome)
                {

                    string resDesc = dr.Code.ToString().Trim();
                    CustomQuestionsEntity intakeincompletdata = listIntakeIncompletdata.Find(u => u.ACTCODE.Trim() == dr.Code.ToString().Trim());
                    if (intakeincompletdata != null)
                    {
                        index = gvwIncompletedata.Rows.Add(true, dr.Desc, dr.Code, intakeincompletdata.ACTALPHARESP, string.Empty, string.Empty, LookupDataAccess.Getdate(intakeincompletdata.LetterDate));
                        CommonFunctions.setTooltip(index, intakeincompletdata.addoperator, intakeincompletdata.adddate, intakeincompletdata.lstcoperator, intakeincompletdata.lstcdate, gvwIncompletedata);

                        //if (intakeincompletdata.LetterDate != string.Empty)
                        //{
                        //   // pnlLetter.Visible = true;
                        //   // lblDate.Text = LookupDataAccess.Getdate(intakeincompletdata.LetterDate);
                        //}
                    }
                    else
                    {
                        index = gvwIncompletedata.Rows.Add(false, dr.Desc, dr.Code, string.Empty, string.Empty, string.Empty, string.Empty);
                    }

                }
                lblLetterDateReq.Visible = false;
                if (listIntakeIncompletdata.Count > 0)
                {

                    if (listIntakeIncompletdata[0].LetterDate != string.Empty)
                    {
                        lblLetterDateReq.Visible = true;
                        dtletterDate.Value = Convert.ToDateTime(listIntakeIncompletdata[0].LetterDate);
                        dtletterDate.Checked = true;
                    }
                    else
                    {
                        dtletterDate.Checked = false;
                    }
                    CommonFunctions.SetComboBoxValue(cmbCaseWorker, listIntakeIncompletdata[0].INTKWorker);
                    strCaseWorkerName = ((Utilities.ListItem)cmbCaseWorker.SelectedItem).Text.ToString();
                }
                else
                {
                    CommonFunctions.SetComboBoxValue(cmbCaseWorker, "0");
                    dtletterDate.Value = DateTime.Now.Date;
                    dtletterDate.Checked = false;
                }

                if (gvwIncompletedata.Rows.Count > 0)
                {
                    gvwIncompletedata.Rows[0].Selected = true;
                    gvwIncompletedata_SelectionChanged(gvwIncompletedata, new EventArgs());
                }
                gvwIncompletedata.SelectionChanged += gvwIncompletedata_SelectionChanged;
            }


        }

        private void txtLine1_Leave(object sender, EventArgs e)
        {
            if (gvwIncompletedata.Rows.Count > 0)
            {
                if (gvwIncompletedata.SelectedRows.Count > 0)
                {
                    gvwIncompletedata.SelectedRows[0].Cells["gvtLine1"].Value = txtLine1.Text;
                    //gvwIncompletedata.Rows[gvwIncompletedata.SelectedCells[0].RowIndex].Cells["gvtLine1"].Value = txtLine1.Text;
                }
            }
        }

        private void gvwIncompletedata_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gvwIncompletedata.Rows.Count > 0)
            {
                if (gvwIncompletedata.SelectedRows.Count > 0)
                {
                    if (gvwIncompletedata.SelectedRows[0].Cells["gvchkSelect"].Value != null && Convert.ToBoolean(gvwIncompletedata.SelectedRows[0].Cells["gvchkSelect"].Value) == true)
                    {
                        txtLine1.Text = gvwIncompletedata.SelectedRows[0].Cells["gvtLine1"].Value.ToString();
                        txtLine1.Enabled = true;

                    }
                    else
                    {
                        txtLine1.Text = string.Empty;
                        txtLine1.Enabled = false;
                        gvwIncompletedata.SelectedRows[0].Cells["gvtLine1"].Value = string.Empty;

                    }

                    ////if (gvwIncompletedata.SelectedCells[0].ColumnIndex == gvchkSelect.Index)
                    ////{
                    //if (gvwIncompletedata.Rows[gvwIncompletedata.SelectedCells[0].RowIndex].Cells["gvchkSelect"].Value != null && Convert.ToBoolean(gvwIncompletedata.Rows[gvwIncompletedata.SelectedCells[0].RowIndex].Cells["gvchkSelect"].Value) == true)
                    //{
                    //    txtLine1.Text = gvwIncompletedata.Rows[gvwIncompletedata.SelectedCells[0].RowIndex].Cells["gvtLine1"].Value.ToString();
                    //    txtLine1.Enabled = true;

                    //}
                    //else
                    //{
                    //    txtLine1.Text = string.Empty;
                    //    txtLine1.Enabled = false;
                    //    gvwIncompletedata.Rows[gvwIncompletedata.SelectedCells[0].RowIndex].Cells["gvtLine1"].Value = string.Empty;

                    //}
                    ////  }


                }

            }
        }

        private void gvwIncompletedata_SelectionChanged(object sender, EventArgs e)
        {
            if (gvwIncompletedata.Rows.Count > 0)
            {
                if (gvwIncompletedata.SelectedRows.Count > 0)
                {

                    if (gvwIncompletedata.SelectedRows[0].Cells["gvchkSelect"].Value != null && Convert.ToBoolean(gvwIncompletedata.SelectedRows[0].Cells["gvchkSelect"].Value) == true)
                    {
                        txtLine1.Text = gvwIncompletedata.SelectedRows[0].Cells["gvtLine1"].Value.ToString();
                        txtLine1.Enabled = true;
                        // pnlDetails.Visible = true;
                    }
                    else
                    {
                        txtLine1.Text = string.Empty;
                        gvwIncompletedata.SelectedRows[0].Cells["gvtLine1"].Value = string.Empty;
                        txtLine1.Enabled = false;
                        // pnlDetails.Visible = false;
                    }

                    //if (gvwIncompletedata.Rows[gvwIncompletedata.SelectedCells[0].RowIndex].Cells["gvchkSelect"].Value != null && Convert.ToBoolean(gvwIncompletedata.Rows[gvwIncompletedata.SelectedCells[0].RowIndex].Cells["gvchkSelect"].Value) == true)
                    //{
                    //    txtLine1.Text = gvwIncompletedata.Rows[gvwIncompletedata.SelectedCells[0].RowIndex].Cells["gvtLine1"].Value.ToString();
                    //    txtLine1.Enabled = true;
                    //    // pnlDetails.Visible = true;
                    //}
                    //else
                    //{
                    //    txtLine1.Text = string.Empty;
                    //    gvwIncompletedata.Rows[gvwIncompletedata.SelectedCells[0].RowIndex].Cells["gvtLine1"].Value = string.Empty;
                    //    txtLine1.Enabled = false;
                    //    // pnlDetails.Visible = false;
                    //}


                }

            }
        }


        private bool ValidateForm()
        {
            bool isValid = true;
            _errorProvider.SetError(dtletterDate, null);
            //if (strMode == Consts.Common.Edit)
            //{
            //    if (dtletterDate.Checked == false)
            //    {
            //        _errorProvider.SetError(dtletterDate, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblletterdate.Text.Replace(Consts.Common.Colon, string.Empty)));
            //        isValid = false;
            //    }

            //}

            //if (dtletterDate.Value > DateTime.Now)
            //{
            //    _errorProvider.SetError(dtletterDate, "Future date not allowed");
            //    isValid = false;

            //}



            if (((Captain.Common.Utilities.ListItem)cmbCaseWorker.SelectedItem).Value.ToString() == "0")
            {
                _errorProvider.SetError(cmbCaseWorker, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblCaseWorker.Text.Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }
            else
            {
                _errorProvider.SetError(cmbCaseWorker, null);
            }


            return (isValid);
        }


        public string propIntakedata = string.Empty;
        private void btnOk_Click(object sender, EventArgs e)
        {

            if (ValidateForm())
            {

                List<DataGridViewRow> SelectedgvRows = (from c in gvwIncompletedata.Rows.Cast<DataGridViewRow>().ToList()
                                                        where (((DataGridViewCheckBoxCell)c.Cells["gvchkSelect"]).Value.ToString().Equals(Consts.YesNoVariants.True, StringComparison.CurrentCultureIgnoreCase))
                                                        select c).ToList();
                if (SelectedgvRows.Count > 0)
                {
                    string selectedCode = string.Empty;
                    propIntakedata = string.Empty;
                    CustomQuestionsEntity custentity = new CustomQuestionsEntity();
                    custentity.ACTAGENCY = BaseForm.BaseAgency;
                    custentity.ACTDEPT = BaseForm.BaseDept;
                    custentity.ACTPROGRAM = BaseForm.BaseProg;
                    custentity.ACTYEAR = BaseForm.BaseYear;
                    custentity.ACTAPPNO = BaseForm.BaseApplicationNo;
                    custentity.Mode = "DELETE";
                    if (_model.CaseMstData.CAPS_INTKINCOMP_INSUPDEL(custentity))
                    {
                    }
                    if (BaseForm.BaseCaseMstListEntity[0].IntakeDate == string.Empty)
                    {
                        BaseForm.BaseCaseMstListEntity[0].IntakeDate = LookupDataAccess.Getdate(DateTime.Now.Date.ToShortDateString());
                    }
                    bool boolform = false;

                    string strHistoryDetails = "<XmlHistory>";
                    bool boolHistory = false;
                    bool boolSubHistory = false;
                    if (listIntakeIncompletdata.Count > 0)
                    {
                        if (dtletterDate.Checked)
                        {
                            if (LookupDataAccess.Getdate(listIntakeIncompletdata[0].LetterDate.Trim()) != LookupDataAccess.Getdate(dtletterDate.Value.ToShortDateString()))
                            {
                                boolHistory = true;
                                strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Letter Date</FieldName><OldValue>" + LookupDataAccess.Getdate(listIntakeIncompletdata[0].LetterDate.Trim()) + "</OldValue><NewValue>" + LookupDataAccess.Getdate(dtletterDate.Value.ToShortDateString()) + "</NewValue></HistoryFields>";
                            }
                        }
                        if (listIntakeIncompletdata[0].INTKWorker.Trim() != ((Utilities.ListItem)cmbCaseWorker.SelectedItem).Value.ToString())
                        {
                            boolHistory = true;
                            strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>CASE WORKER</FieldName><OldValue>" + (strCaseWorkerName == "0" ? string.Empty : strCaseWorkerName) + "</OldValue><NewValue>" + ((Utilities.ListItem)cmbCaseWorker.SelectedItem).Text.ToString() + "</NewValue></HistoryFields>";
                        }
                        //if (listIntakeIncompletdata[0].lstcoperator.Trim() != BaseForm.UserID)
                        //{
                        //    boolHistory = true;
                        //    strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Modified Operator</FieldName><OldValue>" + listIntakeIncompletdata[0].lstcoperator.Trim() + "</OldValue><NewValue>" + BaseForm.UserID + "</NewValue></HistoryFields>";
                        //}
                    }
                    foreach (DataGridViewRow gvRows in gvwIncompletedata.Rows)
                    {
                        boolSubHistory = false;
                        if (gvRows.Cells["gvchkSelect"].Value != null && Convert.ToBoolean(gvRows.Cells["gvchkSelect"].Value) == true)
                        {
                            if (listIntakeIncompletdata.Count > 0)
                            {

                                CustomQuestionsEntity custIntakeHist = listIntakeIncompletdata.Find(u => u.ACTCODE.Trim() == gvRows.Cells["gvtCode"].Value.ToString());
                                if (custIntakeHist != null)
                                {
                                    if (custIntakeHist.ACTALPHARESP.Trim() != gvRows.Cells["gvtLine1"].Value.ToString().Trim())
                                    {
                                        boolHistory = true;
                                        boolSubHistory = true;
                                    }
                                    if (boolSubHistory)
                                    {
                                        strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>" + gvRows.Cells["gvtDesc"].Value.ToString() + "</FieldName><OldValue></OldValue><NewValue>Incomplete Intake Reason(s)  Modified</NewValue></HistoryFields>";

                                        if (custIntakeHist.ACTALPHARESP.Trim() != gvRows.Cells["gvtLine1"].Value.ToString().Trim())
                                        {
                                            strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Details</FieldName><OldValue>" + custIntakeHist.ACTALPHARESP.Trim() + "</OldValue><NewValue>" + gvRows.Cells["gvtLine1"].Value.ToString() + "</NewValue></HistoryFields>";
                                        }
                                    }
                                }
                                else
                                {
                                    strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>" + gvRows.Cells["gvtDesc"].Value.ToString() + "</FieldName><OldValue></OldValue><NewValue>Incomplete Intake Reason(s) Added</NewValue></HistoryFields>";
                                    boolHistory = true;
                                    if (gvRows.Cells["gvtLine1"].Value.ToString().Trim() != string.Empty)
                                    {
                                        boolHistory = true;
                                        strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Details</FieldName><OldValue></OldValue><NewValue>" + gvRows.Cells["gvtLine1"].Value.ToString() + "</NewValue></HistoryFields>";
                                    }

                                }
                            }
                            //else
                            //{
                            //    strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>" + gvRows.Cells["gvtDesc"].Value.ToString() + "</FieldName><OldValue></OldValue><NewValue>Option Added</NewValue></HistoryFields>";
                            //    boolHistory = true;
                            //    if (gvRows.Cells["gvtLine1"].Value.ToString() != string.Empty)
                            //    {
                            //        boolHistory = true;
                            //        strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Details 1</FieldName><OldValue></OldValue><NewValue>" + gvRows.Cells["gvtLine1"].Value.ToString() + "</NewValue></HistoryFields>";
                            //    }
                            //    if (gvRows.Cells["gvtLine2"].Value.ToString() != string.Empty)
                            //    {
                            //        boolHistory = true;
                            //        strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Details 2</FieldName><OldValue></OldValue><NewValue>" + gvRows.Cells["gvtLine2"].Value.ToString() + "</NewValue></HistoryFields>";
                            //    }
                            //    if (gvRows.Cells["gvtLine3"].Value.ToString() != string.Empty)
                            //    {
                            //        boolHistory = true;
                            //        strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Details 3</FieldName><OldValue></OldValue><NewValue>" + gvRows.Cells["gvtLine3"].Value.ToString() + "</NewValue></HistoryFields>";
                            //    }

                            //}

                            custentity.ACTCODE = gvRows.Cells["gvtCode"].Value.ToString().Trim();
                            custentity.ACTALPHARESP = gvRows.Cells["gvtLine1"].Value.ToString();
                            if (dtletterDate.Checked)
                                custentity.LetterDate = dtletterDate.Value.ToShortDateString();
                            else
                                custentity.LetterDate = string.Empty;
                            custentity.INTKWorker = ((Utilities.ListItem)cmbCaseWorker.SelectedItem).Value.ToString();
                            custentity.addoperator = BaseForm.UserID;
                            custentity.Mode = "";
                            if (_model.CaseMstData.CAPS_INTKINCOMP_INSUPDEL(custentity))
                            {

                            }
                        }
                        else
                        {
                            CustomQuestionsEntity custIntakeHist = listIntakeIncompletdata.Find(u => u.ACTCODE.Trim() == gvRows.Cells["gvtCode"].Value.ToString().Trim());
                            if (custIntakeHist != null)
                            {
                                boolHistory = true;
                                strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>" + gvRows.Cells["gvtDesc"].Value.ToString() + "</FieldName><OldValue></OldValue><NewValue>Incomplete Intake Reason(s) Deleted</NewValue></HistoryFields>";
                                if (custIntakeHist.ACTALPHARESP.Trim() != string.Empty)
                                {
                                    boolHistory = true;
                                    strHistoryDetails = strHistoryDetails + "<HistoryFields><FieldName>Details</FieldName><OldValue>" + custIntakeHist.ACTALPHARESP.Trim() + "</OldValue><NewValue></NewValue></HistoryFields>";
                                }

                            }
                        }


                    }

                    strHistoryDetails = strHistoryDetails + "</XmlHistory>";
                    if (boolHistory)
                    {
                        if (strMode == Consts.Common.Edit)
                        {
                            CaseHistEntity caseHistEntity = new CaseHistEntity();
                            caseHistEntity.HistTblName = "INTKINCOMP";
                            caseHistEntity.HistScreen = Privileges.Program;//"CASE2005";
                            caseHistEntity.HistSubScr = "Edit";
                            caseHistEntity.HistTblKey = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + BaseForm.BaseYear + BaseForm.BaseApplicationNo;
                            caseHistEntity.LstcOperator = BaseForm.UserID;
                            caseHistEntity.HistChanges = strHistoryDetails;
                            _model.CaseMstData.InsertCaseHist(caseHistEntity);
                        }
                    }

                    fillGriddata();
                    strMode = Consts.Common.View;
                    propcaseHistList = _model.CaseMstData.GetCaseHistDetails("INTKINCOMP", BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + BaseForm.BaseYear + BaseForm.BaseApplicationNo, Privileges.Program);//"CASE2005"
                    ShowButtons();
                    EnableDisableviewMode(true);
                    //On_PrintLetter(string.Empty);

                    AlertBox.Show("Saved Successfully");
                }
                else
                {
                    AlertBox.Show("Please Select atleast One Incomplete Intake Reason", MessageBoxIcon.Warning);
                }
            }
        }

        string strCaseWorkerDefaultCode = string.Empty;
        string strCaseWorkerName = string.Empty;
        private void fillCombo()
        {
            cmbCaseWorker.Items.Clear();
            cmbCaseWorker.ColorMember = "FavoriteColor";
            List<HierarchyEntity> hierarchyEntity = _model.CaseMstData.GetCaseWorker(strVerfierFormat, BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            string strCaseworker = string.Empty;
            foreach (HierarchyEntity caseworker in hierarchyEntity)
            {
                if (strCaseworker != caseworker.CaseWorker.ToString())
                {
                    strCaseworker = caseworker.CaseWorker.ToString();
                    cmbCaseWorker.Items.Add(new Utilities.ListItem(caseworker.HirarchyName.ToString(), caseworker.CaseWorker.ToString(), caseworker.InActiveFlag, caseworker.InActiveFlag.Equals("N") ? Color.Black : Color.Red));
                }
                if (caseworker.UserID.Trim().ToString().ToUpper() == BaseForm.UserID.ToUpper().Trim().ToString())
                {
                    strCaseWorkerDefaultCode = caseworker.CaseWorker == null ? "0" : caseworker.CaseWorker;

                }
            }
            cmbCaseWorker.Items.Insert(0, new Utilities.ListItem("  ", "0"));
            CommonFunctions.SetComboBoxValue(cmbCaseWorker, strCaseWorkerDefaultCode);

        }

        List<CaseHistEntity> propcaseHistList = new List<CaseHistEntity>();
        private void ShowButtons()
        {
            if (ToolBarEdit != null) ToolBarEdit.Visible = false;
            if (ToolBarNew != null) ToolBarNew.Visible = false;
            if (ToolBarDel != null) ToolBarDel.Visible = false;
            if (ToolBarPrint != null) { ToolBarPrint.Visible = false; ToolBarPrint.Enabled = true; }
            if (ToolBarHistory != null) ToolBarHistory.Visible = false;


            if (strMode == Consts.Common.View)
            {
                dtletterDate.Enabled = false;
                cmbCaseWorker.Enabled = false;
                if (propcaseHistList.Count > 0)
                    if (ToolBarHistory != null) ToolBarHistory.Visible = true;

                if (listIntakeIncompletdata.Count > 0)
                {
                    if (ToolBarPrint != null) ToolBarPrint.Visible = true;
                    if (Privileges.ChangePriv.Equals("false"))
                    {

                        if (ToolBarEdit != null) ToolBarEdit.Enabled = false;
                    }
                    else
                    {

                        if (ToolBarEdit != null) ToolBarEdit.Enabled = true;
                    }


                    if (Privileges.DelPriv.Equals("false"))
                    {

                        if (ToolBarDel != null) ToolBarDel.Enabled = false;
                    }
                    else
                    {

                        if (ToolBarDel != null) ToolBarDel.Enabled = true;
                    }
                }
                else
                {
                    if (Privileges.AddPriv.Equals("false"))
                    {
                        if (ToolBarNew != null) ToolBarNew.Enabled = false;

                    }
                    else
                    {
                        if (ToolBarNew != null) ToolBarNew.Enabled = true;

                    }
                }
                btnOk.Enabled = false;
                btnClose.Enabled = false;
            }
            if (strMode == Consts.Common.New)
            {
                dtletterDate.Enabled = false;
                cmbCaseWorker.Enabled = true;
                if (Privileges.AddPriv.Equals("false"))
                {
                    btnOk.Enabled = false;
                    btnClose.Enabled = false;
                    if (ToolBarNew != null) ToolBarNew.Enabled = false;
                }
                else
                {
                    btnOk.Enabled = true;
                    btnClose.Enabled = true;
                    if (ToolBarNew != null) ToolBarNew.Enabled = false;
                }
            }
            if (strMode == Consts.Common.Edit)
            {
                // dtletterDate.Enabled = true;
                cmbCaseWorker.Enabled = true;
                if (Privileges.ChangePriv.Equals("false"))
                {
                    btnOk.Enabled = false;
                    btnClose.Enabled = false;
                    if (ToolBarEdit != null) ToolBarEdit.Enabled = false;
                }
                else
                {
                    btnOk.Enabled = true;
                    btnClose.Enabled = true;
                    if (ToolBarEdit != null) ToolBarEdit.Enabled = false;
                    if (ToolBarDel != null) ToolBarDel.Enabled = false;
                }
            }




            if (listIntakeIncompletdata.Count > 0)
            {
                if (ToolBarNew != null)
                    ToolBarNew.Visible = false;
                if (ToolBarEdit != null)
                    ToolBarEdit.Visible = true;
                if (ToolBarDel != null)
                    ToolBarDel.Visible = true;

            }
            else
            {
                if (!boolAddbuttonHide)
                {
                    if (ToolBarNew != null)
                        ToolBarNew.Visible = true;
                    if (ToolBarEdit != null)
                        ToolBarEdit.Visible = false;
                    if (ToolBarDel != null)
                        ToolBarDel.Visible = false;
                    if (ToolBarPrint != null) ToolBarPrint.Visible = false;
                }
            }


        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            EnableDisableviewMode(true);
            fillGriddata();
            strMode = Consts.Common.View;
            // IncompleteIntakeWarringMessages();
            ShowButtons();
        }

        public void Refresh()
        {
            EnableDisableviewMode(true);
            fillGriddata();
            propcaseHistList = _model.CaseMstData.GetCaseHistDetails("INTKINCOMP", BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + BaseForm.BaseYear + BaseForm.BaseApplicationNo, Privileges.Program);//case2005

            strMode = Consts.Common.View;
            IncompleteIntakeWarringMessages();
            ShowButtons();


        }
        public ProgramDefinitionEntity ProgramDefinition { get; set; }
        private string Get_IncomeVerification_Stat()
        {
            string Error = "";
            //CaseMST = BaseForm.BaseCaseMstListEntity[0];//_model.CaseMstData.GetCaseMST(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo);
            if (BaseForm.BaseCaseMstListEntity[0] != null && ProgramDefinition != null)
            {
                if (ProgramDefinition.IncomeVerMsg.Equals("Y"))
                {
                    if (string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].EligDate))
                        Error = "Income Not Verified";
                    else
                    {
                        List<CaseVerEntity> caseVerList = _model.CaseMstData.GetCASEVeradpyalst(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty, string.Empty);
                        if (caseVerList.Count > 0)
                        {
                            if (!(Convert.ToDecimal(BaseForm.BaseCaseMstListEntity[0].ProgIncome == string.Empty ? "0" : BaseForm.BaseCaseMstListEntity[0].ProgIncome) == Convert.ToDecimal(caseVerList[0].IncomeAmount == string.Empty ? "0" : caseVerList[0].IncomeAmount)))
                                Error = "Income";
                        }
                        else
                            Error = "Income Not Verified";
                    }
                }
                else
                    Error = "";
            }
            return Error;
        }

        bool Age_Grt_60 = false, Age_Les_6 = false, Disable_Flag = false, FoodStamps_Flag = false;
        private bool Get_SNP_Vulnerable_Status(List<LIHEAPBEntity> LIHEAPB_List)
        {
            bool Vulner_Flag = false;
            DateTime MST_Intake_Date = DateTime.Today, SNP_DOB = DateTime.Today;
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan Time_Span;
            int Age_In_years = 0;

            Age_Grt_60 = Age_Les_6 = Disable_Flag = FoodStamps_Flag = false;
            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].IntakeDate.Trim()))
                MST_Intake_Date = Convert.ToDateTime(BaseForm.BaseCaseMstListEntity[0].IntakeDate);

            foreach (CaseSnpEntity Entity in BaseForm.BaseCaseSnpEntity)
            {
                SNP_DOB = MST_Intake_Date;
                if (!string.IsNullOrEmpty(Entity.AltBdate.Trim()))
                    SNP_DOB = Convert.ToDateTime(Entity.AltBdate);

                Age_In_years = 0;
                if (MST_Intake_Date > SNP_DOB)
                {
                    Time_Span = (MST_Intake_Date - SNP_DOB);
                    Age_In_years = (zeroTime + Time_Span).Year - 1;
                    //Age_In_years = (Time_Span). - 1;
                }

                if (Age_In_years > 59)
                    Age_Grt_60 = true;

                if (Age_In_years < 6)
                    Age_Les_6 = true;

                if (Entity.Disable == "Y")
                    Disable_Flag = true;

                if (Entity.FootStamps == "Y")
                    FoodStamps_Flag = true;
            }

            string Tmp_Age_Dis = string.Empty;
            if (LIHEAPB_List.Count > 0)
                Tmp_Age_Dis = LIHEAPB_List[0].Age_dis;

            if (Age_Grt_60 || Age_Les_6 || Disable_Flag)
            {
                if (Tmp_Age_Dis == "1" || Tmp_Age_Dis == "2" || Tmp_Age_Dis == "3" || Age_Les_6)
                    Vulner_Flag = true;
            }

            return Vulner_Flag;
        }

        bool boolAddbuttonHide = false;
        private string IncompleteIntakeWarringMessages()
        {
            Get_Members_SSN_Reasons();
            boolAddbuttonHide = false;
            string Need_To_ReVerify = string.Empty;
            LIHEAPBEntity Search_Entity = new LIHEAPBEntity(true);
            Search_Entity.Agency = BaseForm.BaseAgency;
            Search_Entity.Dept = BaseForm.BaseDept;
            Search_Entity.Prog = BaseForm.BaseProg;
            Search_Entity.Year = BaseForm.BaseYear;
            Search_Entity.AppNo = BaseForm.BaseApplicationNo;
            LIHEAPB_List = _model.LiheAllData.Browse_LIHEAPB(Search_Entity, "Browse");
            lblMessage1.Text = string.Empty;
            lblMessage2.Text = string.Empty;
            lblMessage3.Text = string.Empty;
            lblMessage4.Text = string.Empty;
            lblMessage5.Text = string.Empty;
            lblMessage1.ForeColor = Color.Red;
            //lblMessage1.Visible = false;
            string Deny_MSG = "";
            if (LIHEAPB_List.Count > 0)
            {


                LIHEAPBEntity Benf_Enty = LIHEAPB_List[0];
                bool Vulner_Flag = Get_SNP_Vulnerable_Status(LIHEAPB_List);
                string Tmp_Mst_Prog_Inc = "0";

                if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].ProgIncome.Trim()))
                    Tmp_Mst_Prog_Inc = BaseForm.BaseCaseMstListEntity[0].ProgIncome.Trim();

                Tmp_Mst_Prog_Inc = ((Tmp_Mst_Prog_Inc == "0.00") ? "0" : Tmp_Mst_Prog_Inc);

                if (Benf_Enty.FAP_Income != Tmp_Mst_Prog_Inc)
                    Need_To_ReVerify = "Income";
                else
                    Need_To_ReVerify = Get_IncomeVerification_Stat();

                string Tmp_LPB_FAP_InHH = Benf_Enty.FAP_No_InHH;
                if (ProgramDefinition.SSNReasonFlag == "Y")
                    Tmp_LPB_FAP_InHH = ((!string.IsNullOrEmpty(Benf_Enty.FAP_No_InHH) ? int.Parse(Benf_Enty.FAP_No_InHH) : 0) + Non_Qual_Alien_Cnt).ToString();

                if (Tmp_LPB_FAP_InHH != BaseForm.BaseCaseMstListEntity[0].NoInProg) // need to check Dep Dup SSN Flag also
                    Need_To_ReVerify = (string.IsNullOrEmpty(Need_To_ReVerify.Trim()) ? "Fam Size" : " Fam Size,");


                if (Benf_Enty.Source != BaseForm.BaseCaseMstListEntity[0].Source)
                    Need_To_ReVerify = (string.IsNullOrEmpty(Need_To_ReVerify.Trim()) ? "Heat Source" : " Heat Source,");

                if (Benf_Enty.Payment_How.Trim().Length > 1)
                {
                    if (Benf_Enty.Payment_How.Trim().Substring(1, 1) != BaseForm.BaseCaseMstListEntity[0].HeatIncRent)
                        Need_To_ReVerify = (string.IsNullOrEmpty(Need_To_ReVerify.Trim()) ? "Method Pay Heat" : " Method Pay Heat,");
                }
                else
                {
                    if (Benf_Enty.Payment_How.Trim() != BaseForm.BaseCaseMstListEntity[0].HeatIncRent)
                        Need_To_ReVerify = (string.IsNullOrEmpty(Need_To_ReVerify.Trim()) ? "Method Pay Heat" : " Method Pay Heat,");
                }

                bool SSN_Re_Cert = false;

                if (BaseForm.BaseCaseMstListEntity[0].SsnFlag == "P" || BaseForm.BaseCaseMstListEntity[0].SsnFlag == "U")
                {
                    if (Benf_Enty.SSN_SW != BaseForm.BaseCaseMstListEntity[0].SsnFlag) // need to check Dep Dup SSN Flag also
                        SSN_Re_Cert = true;

                    //Need_To_ReVerify = (string.IsNullOrEmpty(Need_To_ReVerify.Trim()) ? "SSN Reason" : " SSN Reason,");

                }

                if ((BaseForm.BaseCaseMstListEntity[0].SsnFlag == "Y" || BaseForm.BaseCaseMstListEntity[0].SsnFlag == "N" || BaseForm.BaseCaseMstListEntity[0].SsnFlag.Trim() == "") && (Benf_Enty.SSN_SW == "P" || Benf_Enty.SSN_SW == "U"))
                    SSN_Re_Cert = true;

                if (ProgramDefinition.SSNReasonFlag == "Y" && SSN_Re_Cert)
                    Need_To_ReVerify = (string.IsNullOrEmpty(Need_To_ReVerify.Trim()) ? "SSN Reason" : " SSN Reason,");


                string Snp_Disable = "N";
                foreach (CaseSnpEntity Entity in BaseForm.BaseCaseSnpEntity)
                {
                    if (Entity.Disable == "Y")
                    {
                        Snp_Disable = "Y"; break;
                    }
                }

                if (Snp_Disable != Benf_Enty.Disable)
                    Need_To_ReVerify = (string.IsNullOrEmpty(Need_To_ReVerify.Trim()) ? "Disability" : " Disability");


                string Fund_Type = string.Empty;
                if (Benf_Enty.Award_Type == "B1" || Benf_Enty.Award_Type == "U1" || Benf_Enty.Award_Type == "R1")
                {
                    if (int.Parse(BaseForm.BaseYear.Trim()) > 2022)
                    {
                        string BenLevel = string.Empty;
                        if (LIHEAPB_List[0].Benefit_Level == "2" || LIHEAPB_List[0].Benefit_Level == "1") BenLevel = "1";
                        if (LIHEAPB_List[0].Benefit_Level == "3" || LIHEAPB_List[0].Benefit_Level == "4") BenLevel = "2";
                        if (LIHEAPB_List[0].Benefit_Level == "5") BenLevel = "3";

                        switch (BenLevel)
                        {
                            case "1": Fund_Type = "Ben Level " + BenLevel + (Vulner_Flag ? " Vulnerable" : " Non-Vulnerable"); break;
                            case "2": Fund_Type = "Ben Level " + BenLevel + (Vulner_Flag ? " Vulnerable" : " Non-Vulnerable"); break;
                            case "3": Fund_Type = "Ben Level " + BenLevel + (Vulner_Flag ? " Vulnerable" : " Non-Vulnerable"); break;
                        }

                    }
                    else
                    {
                        switch (LIHEAPB_List[0].Benefit_Level)
                        {
                            case "1":
                            case "2":
                            case "3": Fund_Type = "Ben Level " + LIHEAPB_List[0].Benefit_Level + (Vulner_Flag ? " Vulnerable" : " Non-Vulnerable"); break;
                            case "4": Fund_Type = "Ben Level " + LIHEAPB_List[0].Benefit_Level + (Vulner_Flag ? " Vulnerable" : " Non-Vulnerable"); break;
                            case "5": Fund_Type = "Ben Level " + LIHEAPB_List[0].Benefit_Level + (Vulner_Flag ? " Vulnerable" : " Non-Vulnerable"); break;
                        }

                    }
                    decimal deco1Dollars = 0;
                    if (LIHEAPB_List[0].Award_Amount != string.Empty)
                        deco1Dollars = Convert.ToDecimal(LIHEAPB_List[0].Award_Amount);

                    Deny_MSG = Fund_Type + " $" + String.Format(CultureInfo.InvariantCulture, "{0:0,0.00}", deco1Dollars);
                    lblMessage1.Text = Deny_MSG;
                    lblMessage1.ForeColor = Color.Black;
                    boolAddbuttonHide = true;
                }
                else
                {

                    if (Benf_Enty.Award_Amount == "0")
                    {
                        if (string.IsNullOrEmpty(Deny_MSG.Trim()))
                        {
                            if (ReasonCodes.Count > 0)
                            {
                                AGYTABSEntity AgysEntity = ReasonCodes.Find(u => u.Table_Code.Equals(Benf_Enty.Denied_Reason.Trim()));
                                if (AgysEntity != null)
                                {
                                    if (int.Parse(BaseForm.BaseYear.Trim()) > 2021 && AgysEntity.Table_Code == "07")
                                        Deny_MSG = String.Empty;
                                    else
                                        Deny_MSG = "Applicant is Denied. " + AgysEntity.Code_Desc.Trim();
                                }
                            }
                        }
                    }

                    lblMessage1.Text = Deny_MSG;
                    lblMessage1.ForeColor = Color.Red;
                }
            }


            LiheApvEntity liheApvPrimary = null;
            List<LiheApvEntity> LIHEAPV_List = _model.LiheAllData.GetLiheAppvadpyas(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty, string.Empty, string.Empty);
            if (LIHEAPV_List.Count > 0)
            {
                liheApvPrimary = LIHEAPV_List.Find(u => u.LPV_PRIMARY_CODE == "P" && u.LPV_ACCOUNT_NO != string.Empty);
            }
            //lblMessage2.ForeColor = Color.Red;
            //lblMessage3.ForeColor = Color.Red;
            //lblMessage4.ForeColor = Color.Red;
            //lblMessage5.ForeColor = Color.Red;
            if (BaseForm.BaseCaseMstListEntity[0].HeatIncRent == "1" && (BaseForm.BaseCaseMstListEntity[0].Source.Trim() == "02" ||
               BaseForm.BaseCaseMstListEntity[0].Source.Trim() == "04") && liheApvPrimary == null)
            {
                if (lblMessage2.Text != string.Empty)
                {
                    lblMessage3.Text = "Utility Bill is Missing or Invalid Account #.";
                    //lblMessage3.ForeColor = Color.Orange;
                }
                else
                {
                    lblMessage2.Text = "Utility Bill is Missing or Invalid Account #.";
                    // lblMessage2.ForeColor = Color.Orange;
                }


            }
            if (BaseForm.BaseCaseMstListEntity[0].HeatIncRent == "2" && BaseForm.BaseCaseMstListEntity[0].Housing == "B")
            {
                if (LIHEAPB_List.Count == 0 && BaseForm.Gbl_Rent_Rec != "Y")
                {
                    if (lblMessage2.Text == string.Empty)
                    {
                        lblMessage2.Text = "Rent Receipt is Missing.";
                        // lblMessage2.ForeColor = Color.Orange;
                    }
                    else
                    {
                        if (lblMessage3.Text == string.Empty)
                        {
                            lblMessage3.Text = "Rent Receipt is Missing.";
                            // lblMessage3.ForeColor = Color.Orange;
                        }
                        else
                        {
                            lblMessage4.Text = "Rent Receipt is Missing.";
                            // lblMessage4.ForeColor = Color.Orange;
                        }
                    }
                }

            }
            int HHPRogCnt = 0;
            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].NoInProg.Trim()))
                HHPRogCnt = Convert.ToInt32(BaseForm.BaseCaseMstListEntity[0].NoInProg);


            int inthousecount = HHPRogCnt - Non_Qual_Alien_Cnt;
            if ((BaseForm.BaseCaseMstListEntity[0].NoInProg == "0") || (inthousecount <= 0))
            {
                if (lblMessage2.Text == string.Empty)
                {
                    lblMessage2.Text = "No members in the household with a valid SS# or SS# Reason";
                    //lblMessage2.ForeColor = Color.Orange;
                }
                else
                {
                    if (lblMessage3.Text == string.Empty)
                    {
                        lblMessage3.Text = "No members in the household with a valid SS# or SS# Reason";
                        // lblMessage3.ForeColor = Color.Orange;
                    }
                    else
                    {
                        if (lblMessage4.Text == string.Empty)
                        {
                            lblMessage4.Text = "No members in the household with a valid SS# or SS# Reason";
                            //lblMessage4.ForeColor = Color.Orange;
                        }
                        else
                        {
                            lblMessage5.Text = "No members in the household with a valid SS# or SS# Reason";
                            //lblMessage5.ForeColor = Color.Orange;
                        }
                    }
                }
            }

            return Need_To_ReVerify;
        }

        int Non_Qual_Alien_Cnt = 0;
        string SSN_Unknown_SW = string.Empty, SSN_Pending_SW = string.Empty, Non_Qual_Alien_SW = string.Empty;
        private void Get_Members_SSN_Reasons()
        {
            Non_Qual_Alien_Cnt = 0;
            foreach (CaseSnpEntity Entity in BaseForm.BaseCaseSnpEntity)
            {
                if (!string.IsNullOrEmpty(Entity.Ssno.Trim()))
                {
                    if (ProgramDefinition.SSNReasonFlag == "Y" && Entity.Ssno.Substring(0, 9) == "000000000" && Entity.Status == "A" && Entity.Exclude != "Y")
                    {
                        switch (Entity.SsnReason)
                        {
                            case "U": SSN_Unknown_SW = "Y"; break;
                            case "P": SSN_Pending_SW = "Y"; break;
                            case "Q": Non_Qual_Alien_SW = "Y"; Non_Qual_Alien_Cnt++; break;
                        }
                    }
                }
            }
        }

        List<AGYTABSEntity> ReasonCodes = new List<AGYTABSEntity>();
        private void Fill_Deny_ReasonCodes()
        {
            AGYTABSEntity searchAgytabs = new AGYTABSEntity(true);
            searchAgytabs.Tabs_Type = "S0085";
            ReasonCodes = _model.AdhocData.Browse_AGYTABS(searchAgytabs);
        }


        #region Printpdffile



        PdfContentByte cb;
        int X_Pos, Y_Pos;
        string strFolderPath = string.Empty;
        string Random_Filename = null;
        int pageNumber = 1; string PdfName = "Pdf File";
        string PdfScreen = null, rnkCd = null, PrivrnkCd = null, Rankdesc = null;
        string PrintText = null;
        public void On_PrintLetter(string strUpdDate)
        {
            List<CommonEntity> listIncompleteIncome = CommonFunctions.AgyTabsFilterOrderbyCode(BaseForm.BaseAgyTabsEntity, "09997", BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, string.Empty); //_model.lookupDataAccess.GetHealthInsurance();

            Random_Filename = string.Empty;
            if (listIncompleteIncome.Count > 0)
            {

                string PdfName = "Pdf File";
                PdfName = BaseForm.BaseApplicationNo + "Report";

                PdfName = propReportPath + BaseForm.UserID + "\\" + PdfName;
                try
                {
                    if (!Directory.Exists(propReportPath + BaseForm.UserID.Trim()))
                    { DirectoryInfo di = Directory.CreateDirectory(propReportPath + BaseForm.UserID.Trim()); }
                }
                catch (Exception ex)
                {
                    AlertBox.Show("Error", MessageBoxIcon.Error);
                    ex.ToString();
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
                Document document = new Document(PageSize.LETTER, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
                iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
                BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
                iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
                BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

                iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
                // iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 8);
                iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 9, 3);
                iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 9, 1);
                iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 8, 2);
                iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 9, 4);
                iTextSharp.text.Font TblFontBoldLetter = new iTextSharp.text.Font(1, 9, 1);
                iTextSharp.text.Font TableFont9 = new iTextSharp.text.Font(bf_times, 9);
                cb = writer.DirectContent;

                //iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 9, 1);
                //iTextSharp.text.Font TblFontBoldLetter = new iTextSharp.text.Font(1, 9, 1);
                iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 10);
                //iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 10, 1);

                try
                {
                    PdfPTable Table = new PdfPTable(4);
                    Table.TotalWidth = 500f;
                    Table.WidthPercentage = 100;
                    Table.LockedWidth = true;
                    float[] Lastwidths = new float[] { 8f, 30f, 8f, 40f };
                    Table.SetWidths(Lastwidths);
                    Table.HorizontalAlignment = Element.ALIGN_CENTER;



                    ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);

                    string State = BaseForm.BaseAgencyControlDetails.State;

                    string Address1 = string.Empty; string Address2 = string.Empty; string Address3 = string.Empty; string Address4 = string.Empty; string Fax = string.Empty;
                    if (State == "CT" && (BaseForm.BaseCaseMstListEntity[0].Site == "0900" || BaseForm.BaseCaseMstListEntity[0].Site == "0901" || BaseForm.BaseCaseMstListEntity[0].Site == "0902" || BaseForm.BaseCaseMstListEntity[0].Site == "0903" || BaseForm.BaseCaseMstListEntity[0].Site == "0904" || BaseForm.BaseCaseMstListEntity[0].Site == "0905" || BaseForm.BaseCaseMstListEntity[0].Site == "0906" || BaseForm.BaseCaseMstListEntity[0].Site == "0907"))
                    {
                        Address1 = "55 West Main Street 3rd Floor"; Address2 = "Meriden, CT 06451"; Address3 = "Phone:(203) 235-0278" + " Fax #: (203) 235-4707 "; Address4 = "Hours: 09:00 AM to 05:00 PM";
                    }
                    else if (State == "CT" && (BaseForm.BaseCaseMstListEntity[0].Site == "BRIS" || BaseForm.BaseCaseMstListEntity[0].Site == "FARM" || BaseForm.BaseCaseMstListEntity[0].Site == "PLYM"))
                    {
                        Address1 = "55 South Street"; Address2 = "Bristol, CT 06010"; Address3 = "Phone:(860) 584-2725" + " Fax #: (860) 582-5224 ";
                        DataSet dsAgency = Captain.DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(BaseForm.BaseAgency, null, null, null, null, null, null);
                        if (dsAgency != null && dsAgency.Tables[0].Rows.Count > 0)
                            Address4 = "Hours: " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_FROM"].ToString().Trim()) + " to " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_TO"].ToString().Trim());

                    }
                    else
                    {
                        if (programEntity != null)
                        {
                            Address1 = programEntity.Address1.Trim() + " " + programEntity.Address2.Trim();
                            Address2 = programEntity.City.Trim() + ", " + programEntity.State.Trim() + " " + "00000".Substring(0, 5 - programEntity.Zip.Trim().Length) + programEntity.Zip.Trim() + "-" + "0000".Substring(0, 4 - programEntity.ZipPlus.Trim().Length) + programEntity.ZipPlus.Trim();
                            MaskedTextBox mskphn = new MaskedTextBox(); MaskedTextBox mskFax = new MaskedTextBox();
                            mskphn.Mask = "(000) 000-0000"; mskphn.Text = programEntity.Phone.Trim();
                            mskFax.Mask = "(000) 000-0000"; mskFax.Text = programEntity.DepFax.Trim();
                            if (BaseForm.BaseAgencyControlDetails.AgyShortName == "NEON")
                            {
                                Address3 = "Phone: " + mskphn.Text;
                                Fax = "Fax #: " + mskFax.Text;
                            }
                            else
                                Address3 = "Phone: " + mskphn.Text + "    Fax #: " + mskFax.Text;
                            DataSet dsAgency = Captain.DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(BaseForm.BaseAgency, null, null, null, null, null, null);
                            if (dsAgency != null && dsAgency.Tables[0].Rows.Count > 0)
                                Address4 = "Hours: " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_FROM"].ToString().Trim()) + " to " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_TO"].ToString().Trim());
                        }
                    }


                    // PrintAgcncyAddress(Address1, Address2, Address3, Address4, Table, programEntity, ShortName, Fax);
                    //string Address1 = string.Empty; string Address2 = string.Empty; string Address3 = string.Empty; string Address4 = string.Empty; string Fax = string.Empty;
                    ////if (State == "CT" && (Entity.Site == "0900" || Entity.Site == "0901" || Entity.Site == "0902" || Entity.Site == "0903" || Entity.Site == "0904" || Entity.Site == "0905" || Entity.Site == "0906" || Entity.Site == "0907"))
                    ////{
                    ////    //Address1 = "74 Cambridge Street"; Address2 = "Meriden, CT 06450"; Address3 = "Phone:(203) 235-0278" + " Fax #: (203) 235-4707 "; Address4 = "Hours: 09:00 AM to 05:00 PM";
                    ////    Address1 = "55 West Main Street 3rd Floor"; Address2 = "Meriden, CT 06451"; Address3 = "Phone:(203) 235-0278" + " Fax #: (203) 235-4707 "; Address4 = "Hours: 09:00 AM to 05:00 PM";
                    ////}
                    ////else if (State == "CT" && (Entity.Site == "BRIS" || Entity.Site == "FARM" || Entity.Site == "PLYM"))
                    ////{
                    ////    Address1 = "55 South Street"; Address2 = "Bristol, CT 06010"; Address3 = "Phone:(860) 584-2725" + " Fax #: (860) 582-5224 ";
                    //DataSet dsAgency = Captain.DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(BaseForm.BaseAgency, null, null, null, null, null, null);
                    //if (dsAgency != null && dsAgency.Tables[0].Rows.Count > 0)
                    //    Address4 = "Hours: " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_FROM"].ToString().Trim()) + " to " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_TO"].ToString().Trim());
                    //ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);

                    //if (programEntity != null)
                    //{
                    //    Address1 = programEntity.Address1.Trim() + " " + programEntity.Address2.Trim();
                    //    Address2 = programEntity.City.Trim() + ", " + programEntity.State.Trim() + " " + "00000".Substring(0, 5 - programEntity.Zip.Trim().Length) + programEntity.Zip.Trim() + "-" + "0000".Substring(0, 4 - programEntity.ZipPlus.Trim().Length) + programEntity.ZipPlus.Trim();
                    //    MaskedTextBox mskphn = new MaskedTextBox(); MaskedTextBox mskFax = new MaskedTextBox();
                    //    mskphn.Mask = "(000) 000-0000"; mskphn.Text = programEntity.Phone.Trim();
                    //    mskFax.Mask = "(000) 000-0000"; mskFax.Text = programEntity.DepFax.Trim();
                    //    Address3 = "Phone: " + mskphn.Text + "    Fax #: " + mskFax.Text;
                    //}


                    PrintAgcncyAddress(Address1, Address2, Address3, Address4, Table, programEntity, BaseForm.BaseAgencyControlDetails.AgyShortName, Fax);

                    string AppDate = string.Empty;
                    if (dtletterDate.Checked)
                        AppDate = "Date : " + LookupDataAccess.Getdate(dtletterDate.Value.ToShortDateString());
                    else
                        AppDate = "Date : ";

                    string HN = string.Empty; string Apt = string.Empty; string Floor = string.Empty; string Suffix = string.Empty; string Street = string.Empty;
                    string AppAddress = string.Empty; string AppAddress1 = string.Empty;
                    CaseDiffEntity caseDiffDetails = _model.CaseMstData.GetCaseDiffadpya(BaseForm.BaseAgency.ToString(), BaseForm.BaseDept.ToString(), BaseForm.BaseProg.ToString(), BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty);
                    if (caseDiffDetails != null)
                    {
                        if (!string.IsNullOrEmpty(caseDiffDetails.Street.Trim()))
                        {
                            if (!string.IsNullOrEmpty(caseDiffDetails.Hn.Trim()))
                                HN = caseDiffDetails.Hn.Trim() + "  ";
                            Street = caseDiffDetails.Street.Trim() + "  ";
                            if (!string.IsNullOrEmpty(caseDiffDetails.Suffix.Trim()))
                                Suffix = caseDiffDetails.Suffix.Trim() + "  ";
                            if (!string.IsNullOrEmpty(caseDiffDetails.Apt.Trim()))
                                Apt = "Apt: " + caseDiffDetails.Apt.Trim() + "  ";
                            if (!string.IsNullOrEmpty(caseDiffDetails.Flr.Trim()))
                                Floor = "Flr: " + caseDiffDetails.Flr.Trim();

                            AppAddress = HN + Street + Suffix + Apt + Floor;
                            AppAddress1 = caseDiffDetails.City.Trim() + "  " + caseDiffDetails.State.Trim() + "  " + "00000".Substring(0, 5 - caseDiffDetails.Zip.Trim().Length) + caseDiffDetails.Zip.Trim() + "-" + "0000".Substring(0, 4 - caseDiffDetails.ZipPlus.Trim().Length) + caseDiffDetails.ZipPlus.Trim();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim()))
                                HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + "  ";
                            Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + "  ";
                            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim()))
                                Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + "  ";
                            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Apt.Trim()))
                                Apt = "Apt: " + BaseForm.BaseCaseMstListEntity[0].Apt.Trim() + "  ";
                            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Flr.Trim()))
                                Floor = "Flr: " + BaseForm.BaseCaseMstListEntity[0].Flr.Trim();
                            string zipplus = string.Empty;
                            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim()))
                            {
                                if (int.Parse(BaseForm.BaseCaseMstListEntity[0].Zipplus.ToString()) > 0)
                                    zipplus = "-" + "0000".Substring(0, 4 - BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim().Length) + BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim();
                            }

                            AppAddress = HN + Street + Suffix + Apt + Floor;
                            AppAddress1 = BaseForm.BaseCaseMstListEntity[0].City.Trim() + "  " + BaseForm.BaseCaseMstListEntity[0].State.Trim() + "  " + "00000".Substring(0, 5 - BaseForm.BaseCaseMstListEntity[0].Zip.Trim().Length) + BaseForm.BaseCaseMstListEntity[0].Zip.Trim() + zipplus;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim()))
                            HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + "  ";
                        Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + "  ";
                        if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim()))
                            Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + "  ";
                        if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Apt.Trim()))
                            Apt = "Apt: " + BaseForm.BaseCaseMstListEntity[0].Apt.Trim() + "  ";
                        if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Flr.Trim()))
                            Floor = "Flr: " + BaseForm.BaseCaseMstListEntity[0].Flr.Trim();

                        string zipplus = string.Empty;
                        if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim()))
                        {
                            if (int.Parse(BaseForm.BaseCaseMstListEntity[0].Zipplus.ToString()) > 0)
                                zipplus = "-" + "0000".Substring(0, 4 - BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim().Length) + BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim();
                        }
                        AppAddress = HN + Street + Suffix + Apt + Floor;
                        AppAddress1 = BaseForm.BaseCaseMstListEntity[0].City.Trim() + "  " + BaseForm.BaseCaseMstListEntity[0].State.Trim() + "  " + "00000".Substring(0, 5 - BaseForm.BaseCaseMstListEntity[0].Zip.Trim().Length) + BaseForm.BaseCaseMstListEntity[0].Zip.Trim() + zipplus;
                    }

                    PdfPCell p1cell = new PdfPCell(new Phrase(AppDate, TableFont9));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 4;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);


                    p1cell = new PdfPCell(new Phrase("", TblFontBold));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    p1cell = new PdfPCell(new Phrase(BaseForm.BaseApplicationName, TblFontBold));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 3;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    p1cell = new PdfPCell(new Phrase("", TblFontBold));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    p1cell = new PdfPCell(new Phrase(AppAddress, TableFont9));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 3;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    p1cell = new PdfPCell(new Phrase("", TblFontBold));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    p1cell = new PdfPCell(new Phrase(AppAddress1, TableFont9));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 3;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);


                    p1cell = new PdfPCell(new Phrase("", TblFontBold));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 4;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);


                    if (strUpdDate != string.Empty)
                    {
                        CustomQuestionsEntity custentity = new CustomQuestionsEntity();
                        custentity.ACTAGENCY = BaseForm.BaseAgency;
                        custentity.ACTDEPT = BaseForm.BaseDept;
                        custentity.ACTPROGRAM = BaseForm.BaseProg;
                        custentity.ACTYEAR = BaseForm.BaseYear;
                        custentity.ACTAPPNO = BaseForm.BaseApplicationNo;
                        custentity.addoperator = BaseForm.UserID;
                        custentity.Mode = "LETTER";
                        if (_model.CaseMstData.CAPS_INTKINCOMP_INSUPDEL(custentity))
                        {
                            fillGriddata();
                        }


                    }

                    // PrintIncomepleteIntakePDF(listIncompleteIncome, programEntity);



                    PdfPTable S1Table = new PdfPTable(2);
                    S1Table.TotalWidth = 500f;
                    S1Table.WidthPercentage = 100;
                    float[] S1Tablewidths = new float[] { 10f, 400f };
                    S1Table.SetWidths(S1Tablewidths);
                    S1Table.HorizontalAlignment = Element.ALIGN_LEFT;



                    //iTextSharp.text.Image _image_UnChecked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxUnchecked.JPG"));
                    //iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Context.Server.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));
                    iTextSharp.text.Image _image_UnChecked = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxUnchecked.JPG"));
                    iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

                    _image_UnChecked.ScalePercent(60f);
                    _image_Checked.ScalePercent(60f);



                    PdfPCell cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);



                    cell1 = new PdfPCell(new Phrase("Energy Assistance Case # " + BaseForm.BaseApplicationNo.Trim(), TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("Dear Sir or Madam: ", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("Your application for Energy Assistance cannot be processed for the following reason -", TblFontBoldLetter));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(_image_UnChecked);
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("WE HAVE NOT RECEIVED THE INFORMATION NEEDED TO DETERMINE YOUR ELIGIBILITY", TblFontBold));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("We may be able to complete your application if you provide the information we need as checked and specified below.", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("Please drop off, email, or fax the required proofs as soon as possible.  Call or email us if you need help getting proof.", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.Colspan = 2;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    bool boolexist = false;
                    int index = 0;
                    foreach (CommonEntity dr in listIncompleteIncome)
                    {

                        string resDesc = dr.Code.ToString().Trim();
                        CustomQuestionsEntity intakeincompletdata = listIntakeIncompletdata.Find(u => u.ACTCODE.Trim() == dr.Code.ToString().Trim());
                        X_Pos = 60;
                        if (intakeincompletdata != null)
                        {

                            cell1 = new PdfPCell(_image_Checked);
                            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            S1Table.AddCell(cell1);

                            cell1 = new PdfPCell(new Phrase(dr.Desc, TableFont));
                            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            S1Table.AddCell(cell1);

                            if (intakeincompletdata.ACTALPHARESP != string.Empty)
                            {

                                cell1 = new PdfPCell(new Phrase("", TableFont));
                                cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                S1Table.AddCell(cell1);

                                cell1 = new PdfPCell(new Phrase(intakeincompletdata.ACTALPHARESP, TableFont));
                                cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                S1Table.AddCell(cell1);

                            }

                            cell1 = new PdfPCell(new Phrase("", TableFont));
                            cell1.Colspan = 2;
                            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            S1Table.AddCell(cell1);


                        }
                        else
                        {
                            cell1 = new PdfPCell(_image_UnChecked);
                            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            S1Table.AddCell(cell1);

                            cell1 = new PdfPCell(new Phrase(dr.Desc, TableFont));
                            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            S1Table.AddCell(cell1);

                        }
                    }

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    Phrase phrase = new Phrase(new Chunk("* If you provide us with this documentation ", TableFont));
                    phrase.Add(new Chunk("within 10 days of the postmark date of this letter, ", TblFontBold));
                    phrase.Add(new Chunk("not counting state-designated holidays, we will reprocess your application.", TableFont));

                    cell1 = new PdfPCell(phrase);
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("not counting state-designated holidays, we will reprocess your application.", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("The Energy Assistance Programs are open until May 31st. If you choose to reapply, you must do so by that date. If you reapply, you will be asked to provide updated documentation.", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("These are not entitlement programs. The State has the right to close the programs early if funding is not available. ", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("You have the right to a Desk Review concerning the decision made on this application. Requests must be made in writing and sent to our Energy Office within sixty (60) days of the discovery of the occurrence, or by September 30th, whichever comes first.", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("Persons who misrepresent their household circumstances or income are required to repay any assistance that was provided. In addition, the household will be suspended from participation in the Energy Assistance Programs for the remainder of the current program year, and for the following two (2) program years.", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);




                    //cell1 = new PdfPCell(new Phrase("If you have questions about this letter, please contact our Energy Office.", TblFontBoldLetter));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("Sincerely,", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase(BaseForm.BaseAgencyControlDetails.AgyShortName.Trim() + " Energy Assistance Program Energy Services Department", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);


                    p1cell = new PdfPCell(S1Table);
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 4;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    if (Table.Rows.Count > 0)
                    {
                        document.Add(Table);
                        Table.DeleteBodyRows();
                        // document.NewPage();
                    }
                    cb.SetFontAndSize(bf_times, 10);
                    cb.BeginText();
                    X_Pos = 60;
                    Y_Pos = 80;

                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("If you have questions about this letter, please contact our Energy Office.", TblFontBoldLetter), X_Pos, Y_Pos, 0);


                    Y_Pos -= 17;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Sincerely,", TableFont), X_Pos, Y_Pos, 0);

                    Y_Pos -= 17;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseAgencyControlDetails.AgyShortName.Trim() + " Energy Assistance Program Energy Services Department", TableFont), X_Pos, Y_Pos, 0);

                    cb.EndText();

                }
                catch (Exception ex) { document.Add(new Paragraph("Aborted due to Exception............................................... ")); }


                document.Close();
                fs.Close();
                fs.Dispose();

                //if (Form_Name == "PrintApplicant")
                //{
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
                //}
            }
            else //if (Form_Name == "PrintApplicant")
            {
                AlertBox.Show("Can not print letter as there is no Benefit record for this client", MessageBoxIcon.Warning);
            }


        }


        public void On_PrintLetterSpanish(string strUpdDate)
        {
            List<CommonEntity> listIncompleteIncome = CommonFunctions.AgyTabsFilterOrderbyCode(BaseForm.BaseAgyTabsEntity, "09998", BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, string.Empty); //_model.lookupDataAccess.GetHealthInsurance();

            Random_Filename = string.Empty;
            if (listIncompleteIncome.Count > 0)
            {

                string PdfName = "Pdf File";
                PdfName = BaseForm.BaseApplicationNo + "Report";

                PdfName = propReportPath + BaseForm.UserID + "\\" + PdfName;
                try
                {
                    if (!Directory.Exists(propReportPath + BaseForm.UserID.Trim()))
                    { DirectoryInfo di = Directory.CreateDirectory(propReportPath + BaseForm.UserID.Trim()); }
                }
                catch (Exception ex)
                {
                    AlertBox.Show("Error", MessageBoxIcon.Error);
                    ex.ToString();
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
                Document document = new Document(PageSize.LETTER, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
                iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
                BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
                iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
                BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

                iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 10);
                // iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 8);
                iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 9, 3);
                iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 9, 1);
                iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 8, 2);
                iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 9, 4);
                iTextSharp.text.Font TblFontBoldLetter = new iTextSharp.text.Font(1, 9, 1);
                iTextSharp.text.Font TableFont9 = new iTextSharp.text.Font(bf_times, 9);
                cb = writer.DirectContent;

                //iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 9, 1);
                //iTextSharp.text.Font TblFontBoldLetter = new iTextSharp.text.Font(1, 9, 1);
                iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 10);
                //iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 10, 1);

                try
                {
                    PdfPTable Table = new PdfPTable(4);
                    Table.TotalWidth = 500f;
                    Table.WidthPercentage = 100;
                    Table.LockedWidth = true;
                    float[] Lastwidths = new float[] { 8f, 30f, 8f, 40f };
                    Table.SetWidths(Lastwidths);
                    Table.HorizontalAlignment = Element.ALIGN_CENTER;



                    ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);

                    string State = BaseForm.BaseAgencyControlDetails.State;

                    string Address1 = string.Empty; string Address2 = string.Empty; string Address3 = string.Empty; string Address4 = string.Empty; string Fax = string.Empty;
                    if (State == "CT" && (BaseForm.BaseCaseMstListEntity[0].Site == "0900" || BaseForm.BaseCaseMstListEntity[0].Site == "0901" || BaseForm.BaseCaseMstListEntity[0].Site == "0902" || BaseForm.BaseCaseMstListEntity[0].Site == "0903" || BaseForm.BaseCaseMstListEntity[0].Site == "0904" || BaseForm.BaseCaseMstListEntity[0].Site == "0905" || BaseForm.BaseCaseMstListEntity[0].Site == "0906" || BaseForm.BaseCaseMstListEntity[0].Site == "0907"))
                    {
                        Address1 = "55 West Main Street 3rd Floor"; Address2 = "Meriden, CT 06451"; Address3 = "Phone:(203) 235-0278" + " Fax #: (203) 235-4707 "; Address4 = "Hours: 09:00 AM to 05:00 PM";
                    }
                    else if (State == "CT" && (BaseForm.BaseCaseMstListEntity[0].Site == "BRIS" || BaseForm.BaseCaseMstListEntity[0].Site == "FARM" || BaseForm.BaseCaseMstListEntity[0].Site == "PLYM"))
                    {
                        Address1 = "55 South Street"; Address2 = "Bristol, CT 06010"; Address3 = "Phone:(860) 584-2725" + " Fax #: (860) 582-5224 ";
                        DataSet dsAgency = Captain.DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(BaseForm.BaseAgency, null, null, null, null, null, null);
                        if (dsAgency != null && dsAgency.Tables[0].Rows.Count > 0)
                            Address4 = "Hours: " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_FROM"].ToString().Trim()) + " to " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_TO"].ToString().Trim());

                    }
                    else
                    {
                        if (programEntity != null)
                        {
                            Address1 = programEntity.Address1.Trim() + " " + programEntity.Address2.Trim();
                            Address2 = programEntity.City.Trim() + ", " + programEntity.State.Trim() + " " + "00000".Substring(0, 5 - programEntity.Zip.Trim().Length) + programEntity.Zip.Trim() + "-" + "0000".Substring(0, 4 - programEntity.ZipPlus.Trim().Length) + programEntity.ZipPlus.Trim();
                            MaskedTextBox mskphn = new MaskedTextBox(); MaskedTextBox mskFax = new MaskedTextBox();
                            mskphn.Mask = "(000) 000-0000"; mskphn.Text = programEntity.Phone.Trim();
                            mskFax.Mask = "(000) 000-0000"; mskFax.Text = programEntity.DepFax.Trim();
                            if (BaseForm.BaseAgencyControlDetails.AgyShortName == "NEON")
                            {
                                Address3 = "Phone: " + mskphn.Text;
                                Fax = "Fax #: " + mskFax.Text;
                            }
                            else
                                Address3 = "Phone: " + mskphn.Text + "    Fax #: " + mskFax.Text;
                            DataSet dsAgency = Captain.DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(BaseForm.BaseAgency, null, null, null, null, null, null);
                            if (dsAgency != null && dsAgency.Tables[0].Rows.Count > 0)
                                Address4 = "Hours: " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_FROM"].ToString().Trim()) + " to " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_TO"].ToString().Trim());
                        }
                    }


                    // PrintAgcncyAddress(Address1, Address2, Address3, Address4, Table, programEntity, ShortName, Fax);
                    //string Address1 = string.Empty; string Address2 = string.Empty; string Address3 = string.Empty; string Address4 = string.Empty; string Fax = string.Empty;
                    ////if (State == "CT" && (Entity.Site == "0900" || Entity.Site == "0901" || Entity.Site == "0902" || Entity.Site == "0903" || Entity.Site == "0904" || Entity.Site == "0905" || Entity.Site == "0906" || Entity.Site == "0907"))
                    ////{
                    ////    //Address1 = "74 Cambridge Street"; Address2 = "Meriden, CT 06450"; Address3 = "Phone:(203) 235-0278" + " Fax #: (203) 235-4707 "; Address4 = "Hours: 09:00 AM to 05:00 PM";
                    ////    Address1 = "55 West Main Street 3rd Floor"; Address2 = "Meriden, CT 06451"; Address3 = "Phone:(203) 235-0278" + " Fax #: (203) 235-4707 "; Address4 = "Hours: 09:00 AM to 05:00 PM";
                    ////}
                    ////else if (State == "CT" && (Entity.Site == "BRIS" || Entity.Site == "FARM" || Entity.Site == "PLYM"))
                    ////{
                    ////    Address1 = "55 South Street"; Address2 = "Bristol, CT 06010"; Address3 = "Phone:(860) 584-2725" + " Fax #: (860) 582-5224 ";
                    //DataSet dsAgency = Captain.DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL(BaseForm.BaseAgency, null, null, null, null, null, null);
                    //if (dsAgency != null && dsAgency.Tables[0].Rows.Count > 0)
                    //    Address4 = "Hours: " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_FROM"].ToString().Trim()) + " to " + LookupDataAccess.GetTime(dsAgency.Tables[0].Rows[0]["ACR_HOURS_TO"].ToString().Trim());
                    //ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);

                    //if (programEntity != null)
                    //{
                    //    Address1 = programEntity.Address1.Trim() + " " + programEntity.Address2.Trim();
                    //    Address2 = programEntity.City.Trim() + ", " + programEntity.State.Trim() + " " + "00000".Substring(0, 5 - programEntity.Zip.Trim().Length) + programEntity.Zip.Trim() + "-" + "0000".Substring(0, 4 - programEntity.ZipPlus.Trim().Length) + programEntity.ZipPlus.Trim();
                    //    MaskedTextBox mskphn = new MaskedTextBox(); MaskedTextBox mskFax = new MaskedTextBox();
                    //    mskphn.Mask = "(000) 000-0000"; mskphn.Text = programEntity.Phone.Trim();
                    //    mskFax.Mask = "(000) 000-0000"; mskFax.Text = programEntity.DepFax.Trim();
                    //    Address3 = "Phone: " + mskphn.Text + "    Fax #: " + mskFax.Text;
                    //}


                    PrintAgcncyAddress(Address1, Address2, Address3, Address4, Table, programEntity, BaseForm.BaseAgencyControlDetails.AgyShortName, Fax);

                    string AppDate = string.Empty;
                    if (dtletterDate.Checked)
                        AppDate = "Fecha : " + LookupDataAccess.Getdate(dtletterDate.Value.ToShortDateString());
                    else
                        AppDate = "Fecha : ";

                    string HN = string.Empty; string Apt = string.Empty; string Floor = string.Empty; string Suffix = string.Empty; string Street = string.Empty;
                    string AppAddress = string.Empty; string AppAddress1 = string.Empty;
                    CaseDiffEntity caseDiffDetails = _model.CaseMstData.GetCaseDiffadpya(BaseForm.BaseAgency.ToString(), BaseForm.BaseDept.ToString(), BaseForm.BaseProg.ToString(), BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty);
                    if (caseDiffDetails != null)
                    {
                        if (!string.IsNullOrEmpty(caseDiffDetails.Street.Trim()))
                        {
                            if (!string.IsNullOrEmpty(caseDiffDetails.Hn.Trim()))
                                HN = caseDiffDetails.Hn.Trim() + "  ";
                            Street = caseDiffDetails.Street.Trim() + "  ";
                            if (!string.IsNullOrEmpty(caseDiffDetails.Suffix.Trim()))
                                Suffix = caseDiffDetails.Suffix.Trim() + "  ";
                            if (!string.IsNullOrEmpty(caseDiffDetails.Apt.Trim()))
                                Apt = "Apt: " + caseDiffDetails.Apt.Trim() + "  ";
                            if (!string.IsNullOrEmpty(caseDiffDetails.Flr.Trim()))
                                Floor = "Flr: " + caseDiffDetails.Flr.Trim();

                            AppAddress = HN + Street + Suffix + Apt + Floor;
                            AppAddress1 = caseDiffDetails.City.Trim() + "  " + caseDiffDetails.State.Trim() + "  " + "00000".Substring(0, 5 - caseDiffDetails.Zip.Trim().Length) + caseDiffDetails.Zip.Trim() + "-" + "0000".Substring(0, 4 - caseDiffDetails.ZipPlus.Trim().Length) + caseDiffDetails.ZipPlus.Trim();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim()))
                                HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + "  ";
                            Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + "  ";
                            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim()))
                                Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + "  ";
                            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Apt.Trim()))
                                Apt = "Apt: " + BaseForm.BaseCaseMstListEntity[0].Apt.Trim() + "  ";
                            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Flr.Trim()))
                                Floor = "Flr: " + BaseForm.BaseCaseMstListEntity[0].Flr.Trim();
                            string zipplus = string.Empty;
                            if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim()))
                            {
                                if (int.Parse(BaseForm.BaseCaseMstListEntity[0].Zipplus.ToString()) > 0)
                                    zipplus = "-" + "0000".Substring(0, 4 - BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim().Length) + BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim();
                            }

                            AppAddress = HN + Street + Suffix + Apt + Floor;
                            AppAddress1 = BaseForm.BaseCaseMstListEntity[0].City.Trim() + "  " + BaseForm.BaseCaseMstListEntity[0].State.Trim() + "  " + "00000".Substring(0, 5 - BaseForm.BaseCaseMstListEntity[0].Zip.Trim().Length) + BaseForm.BaseCaseMstListEntity[0].Zip.Trim() + zipplus;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Hn.Trim()))
                            HN = BaseForm.BaseCaseMstListEntity[0].Hn.Trim() + "  ";
                        Street = BaseForm.BaseCaseMstListEntity[0].Street.Trim() + "  ";
                        if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Suffix.Trim()))
                            Suffix = BaseForm.BaseCaseMstListEntity[0].Suffix.Trim() + "  ";
                        if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Apt.Trim()))
                            Apt = "Apt: " + BaseForm.BaseCaseMstListEntity[0].Apt.Trim() + "  ";
                        if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Flr.Trim()))
                            Floor = "Flr: " + BaseForm.BaseCaseMstListEntity[0].Flr.Trim();

                        string zipplus = string.Empty;
                        if (!string.IsNullOrEmpty(BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim()))
                        {
                            if (int.Parse(BaseForm.BaseCaseMstListEntity[0].Zipplus.ToString()) > 0)
                                zipplus = "-" + "0000".Substring(0, 4 - BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim().Length) + BaseForm.BaseCaseMstListEntity[0].Zipplus.Trim();
                        }
                        AppAddress = HN + Street + Suffix + Apt + Floor;
                        AppAddress1 = BaseForm.BaseCaseMstListEntity[0].City.Trim() + "  " + BaseForm.BaseCaseMstListEntity[0].State.Trim() + "  " + "00000".Substring(0, 5 - BaseForm.BaseCaseMstListEntity[0].Zip.Trim().Length) + BaseForm.BaseCaseMstListEntity[0].Zip.Trim() + zipplus;
                    }

                    PdfPCell p1cell = new PdfPCell(new Phrase(AppDate, TableFont9));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 4;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);


                    p1cell = new PdfPCell(new Phrase("", TblFontBold));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    p1cell = new PdfPCell(new Phrase(BaseForm.BaseApplicationName, TblFontBold));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 3;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    p1cell = new PdfPCell(new Phrase("", TblFontBold));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    p1cell = new PdfPCell(new Phrase(AppAddress, TableFont9));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 3;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    p1cell = new PdfPCell(new Phrase("", TblFontBold));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    p1cell = new PdfPCell(new Phrase(AppAddress1, TableFont9));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 3;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);


                    p1cell = new PdfPCell(new Phrase("", TblFontBold));
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 4;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);


                    if (strUpdDate != string.Empty)
                    {
                        CustomQuestionsEntity custentity = new CustomQuestionsEntity();
                        custentity.ACTAGENCY = BaseForm.BaseAgency;
                        custentity.ACTDEPT = BaseForm.BaseDept;
                        custentity.ACTPROGRAM = BaseForm.BaseProg;
                        custentity.ACTYEAR = BaseForm.BaseYear;
                        custentity.ACTAPPNO = BaseForm.BaseApplicationNo;
                        custentity.addoperator = BaseForm.UserID;
                        custentity.Mode = "LETTER";
                        if (_model.CaseMstData.CAPS_INTKINCOMP_INSUPDEL(custentity))
                        {
                            fillGriddata();
                        }


                    }

                    // PrintIncomepleteIntakePDF(listIncompleteIncome, programEntity);



                    PdfPTable S1Table = new PdfPTable(2);
                    S1Table.TotalWidth = 500f;
                    S1Table.WidthPercentage = 100;
                    float[] S1Tablewidths = new float[] { 10f, 400f };
                    S1Table.SetWidths(S1Tablewidths);
                    S1Table.HorizontalAlignment = Element.ALIGN_LEFT;



                    iTextSharp.text.Image _image_UnChecked = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxUnchecked.JPG"));
                    iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

                    _image_UnChecked.ScalePercent(60f);
                    _image_Checked.ScalePercent(60f);



                    PdfPCell cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);



                    cell1 = new PdfPCell(new Phrase("Número de caso de asistencia de energía " + BaseForm.BaseApplicationNo.Trim(), TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("estimado señor o señora: ", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("Su solicitud de Asistencia de Energía no puede ser procesada por la siguiente razón -", TblFontBoldLetter));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(_image_UnChecked);
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("NO HEMOS RECIBIDO LA INFORMACIÓN NECESARIA PARA DETERMINAR SU ELEGIBILIDAD", TblFontBold));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("Es posible que podamos completar su solicitud si proporciona la información que necesitamos tal como se indica y especifica a continuación..", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("Envíe, envíe por correo electrónico o envíe por fax las pruebas requeridas lo antes posible. Llámenos o envíenos un correo electrónico si necesita ayuda para obtener pruebas.", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.Colspan = 2;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    bool boolexist = false;
                    int index = 0;
                    foreach (CommonEntity dr in listIncompleteIncome)
                    {

                        string resDesc = dr.Code.ToString().Trim();
                        CustomQuestionsEntity intakeincompletdata = listIntakeIncompletdata.Find(u => u.ACTCODE.Trim() == dr.Code.ToString().Trim());

                        //TranslationClient client = TranslationClient.Create();
                        //string spanishString = client.TranslateText(dr.Desc, "es").TranslatedText;

                        X_Pos = 60;
                        if (intakeincompletdata != null)
                        {

                            cell1 = new PdfPCell(_image_Checked);
                            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            S1Table.AddCell(cell1);

                            //var culture = new CultureInfo("es-ES");
                            //string Spanishdesc = dr.Desc.ToString(culture);

                            cell1 = new PdfPCell(new Phrase(dr.Desc, TableFont));
                            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            S1Table.AddCell(cell1);

                            if (intakeincompletdata.ACTALPHARESP != string.Empty)
                            {

                                cell1 = new PdfPCell(new Phrase("", TableFont));
                                cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                S1Table.AddCell(cell1);

                                cell1 = new PdfPCell(new Phrase(intakeincompletdata.ACTALPHARESP, TableFont));
                                cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                S1Table.AddCell(cell1);

                            }

                            cell1 = new PdfPCell(new Phrase("", TableFont));
                            cell1.Colspan = 2;
                            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            S1Table.AddCell(cell1);


                        }
                        else
                        {
                            cell1 = new PdfPCell(_image_UnChecked);
                            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            S1Table.AddCell(cell1);

                            cell1 = new PdfPCell(new Phrase(dr.Desc, TableFont));
                            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            S1Table.AddCell(cell1);

                        }
                    }

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    Phrase phrase = new Phrase(new Chunk("* Si nos proporciona esta documentación ", TableFont));
                    phrase.Add(new Chunk("dentro de los 10 días siguientes a la fecha del matasellos de esta carta, ", TblFontBold));
                    phrase.Add(new Chunk("sin contar los días festivos designados por el estado, volveremos a procesar su solicitud.,", TableFont));

                    cell1 = new PdfPCell(phrase);
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("not counting state-designated holidays, we will reprocess your application.", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("Los Programas de Asistencia Energética están abiertos hasta el 31 de mayo. Si elige volver a presentar una solicitud, debe hacerlo antes de esa fecha. Si vuelve a presentar la solicitud, se le pedirá que proporcione documentación actualizada.", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("Estos no son programas de derechos. El Estado tiene derecho a cerrar los programas antes de tiempo si no hay fondos disponibles. ", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("Tiene derecho a una revisión de escritorio con respecto a la decisión tomada en esta solicitud. Las solicitudes deben hacerse por escrito y enviarse a nuestra Oficina de Energía dentro de los sesenta (60) días posteriores al descubrimiento de la ocurrencia, o antes del 30 de septiembre, lo que ocurra primero.", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase("", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);


                    cell1 = new PdfPCell(new Phrase("Las personas que tergiversan las circunstancias de su hogar o sus ingresos están obligadas a reembolsar cualquier asistencia que se haya proporcionado. Además, se suspenderá la participación del hogar en los Programas de Asistencia de Energía por el resto del año del programa actual y por los siguientes dos (2) años del programa.", TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.Colspan = 2;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);




                    //cell1 = new PdfPCell(new Phrase("If you have questions about this letter, please contact our Energy Office.", TblFontBoldLetter));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("Sincerely,", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase("", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);

                    //cell1 = new PdfPCell(new Phrase(BaseForm.BaseAgencyControlDetails.AgyShortName.Trim() + " Energy Assistance Program Energy Services Department", TableFont));
                    //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell1.Colspan = 2;
                    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //S1Table.AddCell(cell1);


                    p1cell = new PdfPCell(S1Table);
                    p1cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    p1cell.Colspan = 4;
                    p1cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Table.AddCell(p1cell);

                    if (Table.Rows.Count > 0)
                    {
                        document.Add(Table);
                        Table.DeleteBodyRows();
                        // document.NewPage();
                    }
                    cb.SetFontAndSize(bf_times, 10);
                    cb.BeginText();
                    X_Pos = 60;
                    Y_Pos = 80;

                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Si tiene preguntas sobre esta carta, comuníquese con nuestra Oficina de Energía.", TblFontBoldLetter), X_Pos, Y_Pos, 0);


                    Y_Pos -= 17;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Atentamente,", TableFont), X_Pos, Y_Pos, 0);

                    Y_Pos -= 17;
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseAgencyControlDetails.AgyShortName.Trim() + " Programa de Asistencia de Energía Departamento de Servicios de Energía", TableFont), X_Pos, Y_Pos, 0);

                    cb.EndText();

                }
                catch (Exception ex) { document.Add(new Paragraph("Aborted due to Exception............................................... ")); }


                document.Close();
                fs.Close();
                fs.Dispose();

                //if (Form_Name == "PrintApplicant")
                //{
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
                //}
            }
            else //if (Form_Name == "PrintApplicant")
            {
                AlertBox.Show("No se puede imprimir la carta porque no hay un registro de beneficios para este cliente", MessageBoxIcon.Warning);
            }


        }

        BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
        BaseFont bf_times_Bold = BaseFont.CreateFont("c:/windows/fonts/TIMESBD.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
        private void PrintIncomepleteIntakePDF(List<CommonEntity> listIncompleteIncome, ProgramDefinitionEntity programEntity)
        {
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 9, 1);
            iTextSharp.text.Font TblFontBoldLetter = new iTextSharp.text.Font(1, 9, 1);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 10);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 10, 1);

            PdfPTable S1Table = new PdfPTable(2);
            S1Table.WidthPercentage = 100;
            float[] S1Tablewidths = new float[] { 5f, 400f };
            S1Table.SetWidths(S1Tablewidths);
            S1Table.HorizontalAlignment = Element.ALIGN_LEFT;

            BaseFont bf_helv = TblFontBold.GetCalculatedBaseFont(false);

            iTextSharp.text.Image _image_UnChecked = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxUnchecked.JPG"));
            iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_UnChecked.ScalePercent(60f);
            _image_Checked.ScalePercent(60f);

            cb.SetFontAndSize(bf_times, 10);
            cb.BeginText();
            X_Pos = 60;
            Y_Pos = 600;
            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Energy Assistance Case # " + BaseForm.BaseApplicationNo.Trim(), TableFont), X_Pos, Y_Pos, 0);

            PdfPCell cell1 = new PdfPCell(new Phrase("Energy Assistance Case # " + BaseForm.BaseApplicationNo.Trim(), TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);


            cell1 = new PdfPCell(new Phrase("Dear Sir or Madam: ", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);


            cell1 = new PdfPCell(new Phrase("Your application for Energy Assistance cannot be processed for the following reason -", TblFontBoldLetter));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);



            cell1 = new PdfPCell(_image_UnChecked);
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);

            cell1 = new PdfPCell(new Phrase("WE HAVE NOT RECEIVED THE INFORMATION NEEDED TO DETERMINE YOUR ELIGIBILITY", TblFontBold));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);


            cell1 = new PdfPCell(new Phrase("We may be able to complete your application if you provide the information we need as checked and specified below.", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);

            cell1 = new PdfPCell(new Phrase("Please drop off, email, or fax the required proofs as soon as possible.  Call or email us if you need help getting proof.", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);

            bool boolexist = false;
            int index = 0;
            foreach (CommonEntity dr in listIncompleteIncome)
            {

                string resDesc = dr.Code.ToString().Trim();
                CustomQuestionsEntity intakeincompletdata = listIntakeIncompletdata.Find(u => u.ACTCODE.Trim() == dr.Code.ToString().Trim());
                X_Pos = 60;
                if (intakeincompletdata != null)
                {
                    //PrintCheckBox(X_Pos, Y_Pos, "Y");
                    //Y_Pos -= 15;
                    //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(dr.Desc, TableFont), X_Pos + 12, Y_Pos, 0);

                    cell1 = new PdfPCell(_image_Checked);
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase(dr.Desc, TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    if (intakeincompletdata.ACTALPHARESP != string.Empty)
                    {
                        Y_Pos -= 10;

                        // ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Detail:", TableFont), X_Pos + 15, Y_Pos, 0);

                        cell1 = new PdfPCell(new Phrase("", TableFont));
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        S1Table.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("Detail:", TableFont));
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        S1Table.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("", TableFont));
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        S1Table.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase(intakeincompletdata.ACTALPHARESP, TableFont));
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        S1Table.AddCell(cell1);


                    }

                }
                else
                {
                    cell1 = new PdfPCell(_image_UnChecked);
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                    cell1 = new PdfPCell(new Phrase(dr.Desc, TableFont));
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    S1Table.AddCell(cell1);

                }
            }



            cell1 = new PdfPCell(new Phrase("* If you provide us with this documentation ", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);

            //cell1 = new PdfPCell(new Phrase("within 10 days of the postmark date of this letter,", TblFontBold));
            //cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cell1.Colspan = 2;
            //cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            //S1Table.AddCell(cell1);

            cell1 = new PdfPCell(new Phrase("not counting state-designated holidays, we will reprocess your application.", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);


            cell1 = new PdfPCell(new Phrase("The Energy Assistance Programs are open until May 31st. If you choose to reapply, you must do so by that date. If you reapply, you will be asked to provide updated documentation.", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);

            cell1 = new PdfPCell(new Phrase("These are not entitlement programs. The State has the right to close the programs early if funding is not available. ", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);

            cell1 = new PdfPCell(new Phrase("You have the right to a Desk Review concerning the decision made on this application. Requests must be made in writing and sent to our Energy Office within sixty (60) days of the discovery of the occurrence, or by September 30th, whichever comes first.", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);


            cell1 = new PdfPCell(new Phrase("Persons who misrepresent their household circumstances or income are required to repay any assistance that was provided. In addition, the household will be suspended from participation in the Energy Assistance Programs for the remainder of the current program year, and for the following two (2) program years.", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);


            cell1 = new PdfPCell(new Phrase("If you have questions about this letter, please contact our Energy Office.", TblFontBoldLetter));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);

            cell1 = new PdfPCell(new Phrase("Sincerely,", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);

            cell1 = new PdfPCell(new Phrase(BaseForm.BaseAgencyControlDetails.AgyShortName.Trim() + " Energy Assistance Program Energy Services Department", TableFont));
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            S1Table.AddCell(cell1);







            ////---------------------------
            //PrintCheckBox(X_Pos, Y_Pos, "N");

            //Y_Pos -= 15;

            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("WE HAVE NOT RECEIVED THE INFORMATION NEEDED TO DETERMINE YOUR ELIGIBILITY", TblFontBold), X_Pos + 12, Y_Pos, 0);


            //Y_Pos -= 22;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("We may be able to complete your application if you provide the information we need as checked and specified below.", TableFont), X_Pos, Y_Pos, 0);
            //Y_Pos -= 10;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Please drop off, email, or fax the required proofs as soon as possible.  Call or email us if you need help getting proof.", TableFont), X_Pos, Y_Pos, 0);


            //bool boolexist = false;
            //int index = 0;
            //foreach (CommonEntity dr in listIncompleteIncome)
            //{

            //    string resDesc = dr.Code.ToString().Trim();
            //    CustomQuestionsEntity intakeincompletdata = listIntakeIncompletdata.Find(u => u.ACTCODE.Trim() == dr.Code.ToString().Trim());
            //    X_Pos = 60;
            //    if (intakeincompletdata != null)
            //    {
            //        PrintCheckBox(X_Pos, Y_Pos, "Y");
            //        Y_Pos -= 15;
            //        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(dr.Desc, TableFont), X_Pos + 12, Y_Pos, 0);
            //        if (intakeincompletdata.ACTALPHARESP != string.Empty)
            //        {
            //            Y_Pos -= 10;

            //            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Detail:", TableFont), X_Pos + 15, Y_Pos, 0);
            //            string[] strings = Regex.Split(intakeincompletdata.ACTALPHARESP, Environment.NewLine);
            //            switch (PrintMultiline(intakeincompletdata.ACTALPHARESP, strings))
            //            {
            //                case "1":
            //                    ColumnText ct = new ColumnText(cb);
            //                    ct.SetSimpleColumn(new Phrase(new Chunk(intakeincompletdata.ACTALPHARESP, TableFont)),
            //                                       X_Pos + 20, Y_Pos - 10, 580, Y_Pos, 10, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            //                    ct.Go();

            //                    Y_Pos -= 10;
            //                    break;
            //                case "2":
            //                    ct = new ColumnText(cb);
            //                    ct.SetSimpleColumn(new Phrase(new Chunk(intakeincompletdata.ACTALPHARESP, TableFont)),
            //                                       X_Pos + 20, Y_Pos - 20, 580, Y_Pos, 10, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            //                    ct.Go();

            //                    Y_Pos -= 20;
            //                    break;
            //                case "3":
            //                    ct = new ColumnText(cb);
            //                    ct.SetSimpleColumn(new Phrase(new Chunk(intakeincompletdata.ACTALPHARESP, TableFont)),
            //                                       X_Pos + 20, Y_Pos - 30, 580, Y_Pos, 10, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            //                    ct.Go();

            //                    Y_Pos -= 30;
            //                    break;

            //            }



            //            // string[] strings = Regex.Split(intakeincompletdata.ACTALPHARESP, Environment.NewLine);

            //            // if (intakeincompletdata.ACTALPHARESP.Length<=100 || strings.Length <= 1)
            //            // {

            //            // }
            //            //else if (intakeincompletdata.ACTALPHARESP.Length <= 200 || strings.Length <= 2)
            //            // {

            //            // }

            //            //if (intakeincompletdata.ACTALPHARESP != string.Empty)
            //            //{
            //            //    string[] strings = Regex.Split(intakeincompletdata.ACTALPHARESP, Environment.NewLine);
            //            //    if (strings.Length > 1)
            //            //    {
            //            //        for (int i = 0; i < strings.Length; i++)
            //            //        {
            //            //            if (i < 4)
            //            //            {
            //            //                if (strings[i].Length <= 80 && strings[i].Length > 0)
            //            //                {
            //            //                    Y_Pos -= 10;
            //            //                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strings[i], TableFont), X_Pos + 20, Y_Pos, 0);
            //            //                }
            //            //                else
            //            //                {
            //            //                    if (strings[i].Length > 0)
            //            //                    {
            //            //                        if (strings[i].Length > 160)
            //            //                        {
            //            //                            Y_Pos -= 10;
            //            //                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strings[i].Substring(0, 80), TableFont), X_Pos + 20, Y_Pos, 0);
            //            //                            Y_Pos -= 10;
            //            //                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strings[i].Substring(80, 80), TableFont), X_Pos + 20, Y_Pos, 0);
            //            //                            Y_Pos -= 10;
            //            //                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strings[i].Substring(160, (Convert.ToInt32(strings[i].Length) - 160)), TableFont), X_Pos + 20, Y_Pos, 0);
            //            //                        }
            //            //                        else
            //            //                        {
            //            //                            Y_Pos -= 10;
            //            //                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strings[i].Substring(0, 80), TableFont), X_Pos + 20, Y_Pos, 0);
            //            //                            Y_Pos -= 10;
            //            //                            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(strings[i].Substring(80, (Convert.ToInt32(strings[i].Length) - 80)), TableFont), X_Pos + 20, Y_Pos, 0);
            //            //                        }
            //            //                    }
            //            //                }
            //            //            }
            //            //        }
            //            //    }
            //            //    else
            //            //    {
            //            //        if (intakeincompletdata.ACTALPHARESP.Length <= 80)
            //            //        {
            //            //            Y_Pos -= 10;
            //            //            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intakeincompletdata.ACTALPHARESP, TableFont), X_Pos + 20, Y_Pos, 0);
            //            //        }
            //            //        else
            //            //        {
            //            //            if (intakeincompletdata.ACTALPHARESP.Length > 160)
            //            //            {
            //            //                Y_Pos -= 10;
            //            //                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intakeincompletdata.ACTALPHARESP.Substring(0, 80), TableFont), X_Pos + 20, Y_Pos, 0);
            //            //                Y_Pos -= 10;
            //            //                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intakeincompletdata.ACTALPHARESP.Substring(80, 80), TableFont), X_Pos + 20, Y_Pos, 0);
            //            //                Y_Pos -= 10;
            //            //                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intakeincompletdata.ACTALPHARESP.Substring(160, (Convert.ToInt32(intakeincompletdata.ACTALPHARESP.Length) - 160)), TableFont), X_Pos + 20, Y_Pos, 0);
            //            //            }
            //            //            else
            //            //            {
            //            //                Y_Pos -= 10;
            //            //                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intakeincompletdata.ACTALPHARESP.Substring(0, 80), TableFont), X_Pos + 20, Y_Pos, 0);
            //            //                Y_Pos -= 10;
            //            //                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(intakeincompletdata.ACTALPHARESP.Substring(80, (Convert.ToInt32(intakeincompletdata.ACTALPHARESP.Length) - 80)), TableFont), X_Pos + 20, Y_Pos, 0);
            //            //            }

            //            //        }
            //            //    }
            //            //}

            //        }

            //    }
            //    else
            //    {
            //        PrintCheckBox(X_Pos, Y_Pos - 5, "N");
            //        Y_Pos -= 20;
            //        ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(dr.Desc, TableFont), X_Pos + 12, Y_Pos, 0);

            //    }
            //}
            //Y_Pos -= 20;
            //X_Pos = 60;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("* If you provide us with this documentation ", TableFont), X_Pos, Y_Pos, 0);
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("within 10 days of the postmark date of this letter,", TblFontBold), 238, Y_Pos, 0);


            //Y_Pos -= 10;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("not counting state-designated holidays, we will reprocess your application.", TableFont), X_Pos, Y_Pos, 0);


            //X_Pos = 60;
            //Y_Pos -= 20;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("The Energy Assistance Programs are open until May 31st. If you choose to reapply, you must do so by that date. If you reapply,", TableFont), X_Pos, Y_Pos, 0);
            //Y_Pos -= 10;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("you will be asked to provide updated documentation.", TableFont), X_Pos, Y_Pos, 0);

            //X_Pos = 60;
            //Y_Pos -= 20;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("These are not entitlement programs. The State has the right to close the programs early if funding is not available.", TableFont), X_Pos, Y_Pos, 0);


            //X_Pos = 60;
            //Y_Pos -= 20;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("You have the right to a Desk Review concerning the decision made on this application. Requests must be made in writing and", TableFont), X_Pos, Y_Pos, 0);
            //Y_Pos -= 10;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("sent to our Energy Office within sixty (60) days of the discovery of the occurrence, or by September 30th, whichever comes first.", TableFont), X_Pos, Y_Pos, 0);




            //Y_Pos -= 20;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Persons who misrepresent their household circumstances or income are required to repay any assistance that was provided. In ", TableFont), X_Pos, Y_Pos, 0);
            //Y_Pos -= 10;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("addition, the household will be suspended from participation in the Energy Assistance Programs for the remainder of the current ", TableFont), X_Pos, Y_Pos, 0);
            //Y_Pos -= 10;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("program year, and for the following two (2) program years.", TableFont), X_Pos, Y_Pos, 0);


            //if (Y_Pos < 110)
            //    Y_Pos = 80;
            //else
            //    Y_Pos = 100;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("If you have questions about this letter, please contact our Energy Office.", TblFontBoldLetter), X_Pos, Y_Pos, 0);


            //Y_Pos -= 20;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Sincerely,", TableFont), X_Pos, Y_Pos, 0);

            //Y_Pos -= 20;
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseAgencyControlDetails.AgyShortName.Trim() + " Energy Assistance Program Energy Services Department", TableFont), X_Pos, Y_Pos, 0);

            //cb.EndText();

            ////Y_Pos -= 8;
            ////cb.SetLineWidth(0.7f);
            ////cb.MoveTo(30f, float.Parse(Y_Pos.ToString()));
            ////cb.LineTo(40f, float.Parse(Y_Pos.ToString()));
            ////cb.LineTo(580f, float.Parse(Y_Pos.ToString()));
            ////cb.Stroke();



        }

        private string PrintMultiline(string strDesclength, string[] strArrayLen)
        {
            string strLine = "1";
            if (strDesclength.Length <= 120)
                strLine = "1";
            else if (strDesclength.Length <= 234)
                strLine = "2";
            else if (strDesclength.Length > 234)
                return strLine = "3";

            if (strArrayLen.Length <= 1 && strDesclength.Length <= 117)
            {
                strLine = "1";
            }
            else if (strArrayLen.Length <= 2 && strDesclength.Length <= 234)
                strLine = "2";
            else if (strArrayLen.Length > 2)
                strLine = "3";

            return strLine;
        }

        private void PrintCheckBox(int X_Pos, int Y_Pos, string Check)
        {
            iTextSharp.text.Image _image_UnChecked = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxUnchecked.JPG"));
            iTextSharp.text.Image _image_Checked = iTextSharp.text.Image.GetInstance(Application.MapPath("~\\Resources\\Icons\\16X16\\CheckBoxChecked.JPG"));

            _image_UnChecked.ScalePercent(60f);
            _image_Checked.ScalePercent(60f);

            if (Check == "Y")
            {
                _image_Checked.SetAbsolutePosition(X_Pos, Y_Pos - 17);
                cb.AddImage(_image_Checked);
            }
            else
            {
                _image_UnChecked.SetAbsolutePosition(X_Pos, Y_Pos - 17);
                cb.AddImage(_image_UnChecked);
            }
        }



        private void PrintAgcncyAddress(string Add1, string Add2, string Add3, string Add4, PdfPTable table, ProgramDefinitionEntity PrgEntity, string Shortname, string Fax)
        {
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 11, 1);


            PdfPCell Agncy = new PdfPCell(new Phrase(PrgEntity.ProgramName.Trim(), TblFontBold));
            Agncy.HorizontalAlignment = Element.ALIGN_CENTER;
            Agncy.Colspan = 4;
            Agncy.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(Agncy);

            PdfPCell A1 = new PdfPCell(new Phrase(Add1, TblFontBold));
            A1.HorizontalAlignment = Element.ALIGN_CENTER;
            A1.Colspan = 4;
            A1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(A1);

            PdfPCell A2 = new PdfPCell(new Phrase(Add2, TblFontBold));
            A2.HorizontalAlignment = Element.ALIGN_CENTER;
            A2.Colspan = 4;
            A2.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(A2);

            PdfPCell A3 = new PdfPCell(new Phrase(Add3, TblFontBold));
            A3.HorizontalAlignment = Element.ALIGN_CENTER;
            A3.Colspan = 4;
            A3.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(A3);

            //PdfPCell NRE1 = new PdfPCell(new Phrase("abcdefgh@XXX.org", TblFontBold));
            //NRE1.HorizontalAlignment = Element.ALIGN_CENTER;
            //NRE1.Colspan = 4;
            //NRE1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            //table.AddCell(NRE1);

            PdfPCell A4 = new PdfPCell(new Phrase(Add4, TblFontBold));
            A4.HorizontalAlignment = Element.ALIGN_CENTER;
            A4.Colspan = 4;
            A4.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(A4);

            PdfPCell space = new PdfPCell(new Phrase("", TblFontBold));
            space.HorizontalAlignment = Element.ALIGN_CENTER;
            space.Colspan = 4;
            space.FixedHeight = 10f;
            space.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(space);
        }

        private void On_Delete_PDF_File(object sender, FormClosedEventArgs e)
        {
            System.IO.File.Delete(PdfName);
            //try
            //{
            //    if (objForm != null)
            //    {
            //        objForm.Close();
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}

        }

        #endregion


        #region Prinnt Energy App


        private void On_PrintCTApp()
        {
            Random_Filename = null;

            IncomeInterValList = CommonFunctions.AgyTabsFilterCodeStatus(BaseForm.BaseAgyTabsEntity, "S0015", string.Empty, string.Empty, string.Empty, string.Empty);
            PdfName = BaseForm.BaseApplicationNo.ToString() + "Report";//form.GetFileName();
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

            Document document = new Document();
            document.SetPageSize(iTextSharp.text.PageSize.LETTER.Rotate());
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();
            BaseFont bf_times = BaseFont.CreateFont("c:/windows/fonts/TIMES.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            BaseFont bf_timesBold = BaseFont.CreateFont("c:/windows/fonts/TIMESBD.TTF", BaseFont.WINANSI, BaseFont.EMBEDDED);
            iTextSharp.text.Font helvetica = new iTextSharp.text.Font(bf_times, 12, 1);
            BaseFont bf_helv = helvetica.GetCalculatedBaseFont(false);
            iTextSharp.text.Font TimesUnderline = new iTextSharp.text.Font(1, 9, 4);
            BaseFont bf_TimesUnderline = TimesUnderline.GetCalculatedBaseFont(true);

            iTextSharp.text.Font Times = new iTextSharp.text.Font(bf_times, 9);
            iTextSharp.text.Font TableFont = new iTextSharp.text.Font(bf_times, 8);
            iTextSharp.text.Font TableFontBoldItalic = new iTextSharp.text.Font(bf_times, 8, 3);
            iTextSharp.text.Font TblFontBold = new iTextSharp.text.Font(1, 9, 1);
            iTextSharp.text.Font TblFontBoldS = new iTextSharp.text.Font(1, 8, 1);
            iTextSharp.text.Font TblFontItalic = new iTextSharp.text.Font(bf_times, 8, 2);
            iTextSharp.text.Font Timesline = new iTextSharp.text.Font(bf_times, 9, 4);
            cb = writer.DirectContent;


            string Attention = string.Empty, Roma_Switch = string.Empty;
            DataSet ds = Captain.DatabaseLayer.ADMNB001DB.ADMNB001_Browse_AGCYCNTL("00", null, null, null, null, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                Attention = ds.Tables[0].Rows[0]["ACR_03_ATTESTATION"].ToString().Trim();
                Roma_Switch = ds.Tables[0].Rows[0]["ACR_ROMA_SWITCH"].ToString().Trim();
            }

            //Mst Details Table
            DataSet dsCaseMST = DatabaseLayer.CaseSnpData.GetCaseMST(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo);
            DataRow drCaseMST = dsCaseMST.Tables[0].Rows[0];

            //Snp details Table
            DataSet dsCaseSNP = DatabaseLayer.CaseSnpData.GetCaseSnpDetails(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, null);
            if (dsCaseSNP.Tables.Count > 0)
            {
                dtCaseSNP = dsCaseSNP.Tables[0];
                DataView dvSNP = new DataView(dtCaseSNP);
                dvSNP.RowFilter = "SNP_STATUS<>'I'";
                dtCaseSNP = dvSNP.ToTable();
            }

            //Casesite Table
            List<CaseSiteEntity> SiteList = new List<CaseSiteEntity>();
            CaseSiteEntity Search_Site = new CaseSiteEntity(true);
            Search_Site.SiteAGENCY = BaseForm.BaseAgency; Search_Site.SiteNUMBER = BaseForm.BaseCaseMstListEntity[0].Site;
            Search_Site.SiteROOM = "0000";
            SiteList = _model.CaseMstData.Browse_CASESITE(Search_Site, "Browse");


            //CaseHie Table
            DataSet dsCaseHie = DatabaseLayer.ADMNB001DB.ADMNB001_GetCashie("**-**-**", BaseForm.UserID, BaseForm.BaseAdminAgency);
            DataTable dtCaseHie = dsCaseHie.Tables[0];

            //Getting CaseWorker
            DataSet dsVerifier = DatabaseLayer.CaseMst.GetCaseWorker("I", BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg);
            DataTable dtVerifier = dsVerifier.Tables[0];

            //CaseIncome Table
            DataSet dsCaseIncome = DatabaseLayer.CaseMst.GetCASEINCOME(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo);
            DataTable dtCaseIncome = dsCaseIncome.Tables[0];
            DataSet dsIncome = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.INCOMETYPES);

            DataSet dsCaseDiff = DatabaseLayer.CaseMst.GetCASEDiffadpya(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo);
            DataTable dtCasediff = dsCaseDiff.Tables[0];

            DataSet dsLandlord = DatabaseLayer.CaseMst.GetLandlordadpya(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo);
            DataTable dtLandlorddet = dsLandlord.Tables[0];

            ////CHLDMST Table
            //ChldMstEntity chldMstDetails = _model.ChldMstData.GetChldMstDetails(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty);

            ////CHLDEMER TABLE
            //List<ChldMstEMEMEntitty> chldEmemDetails = _model.ChldMstData.GetChldEmemList(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty);
            //CaseCondEntitty caseconddet = _model.ChldMstData.GetCaseCondDetails(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, string.Empty);

            DataSet Relations = DatabaseLayer.AgyTab.GetAgyTabDetails(Consts.AgyTab.RELATIONSHIP);
            //DataTable dtrelation = Relations.Tables[0];
            List<CommonEntity> commonEntity = new List<CommonEntity>();
            if (Relations != null && Relations.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in Relations.Tables[0].Rows)
                    commonEntity.Add(new CommonEntity(dr["AGY_1"].ToString(), dr["Agy_8"].ToString(), dr["AGY_2"].ToString()));
            }

            CommonEntity MotherEntity = new CommonEntity(); List<CommonEntity> FatherEntity = new List<CommonEntity>();
            if (commonEntity.Count > 0)
            {
                MotherEntity = commonEntity.Find(u => u.Hierarchy.Equals("G1"));
                FatherEntity = commonEntity.FindAll(u => u.Hierarchy.Equals("G2"));
            }

            List<CommonEntity> lookInsuranceCategory = _model.lookupDataAccess.GetInsuranceCategory();

            DataSet dsFUND = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.CASEMNGMTFUNDSRC, "A");
            DataTable dtFUND = dsFUND.Tables[0];

            LIHEAPBEntity Search_Entity = new LIHEAPBEntity(true);
            Search_Entity.Agency = BaseForm.BaseAgency; Search_Entity.Dept = BaseForm.BaseDept; Search_Entity.Prog = BaseForm.BaseProg;
            Search_Entity.Year = BaseForm.BaseYear; Search_Entity.AppNo = BaseForm.BaseApplicationNo;
            List<LIHEAPBEntity> LiheapBDet = _model.LiheAllData.Browse_LIHEAPB(Search_Entity, "Browse");

            List<LiheApvEntity> LiheapVDet = _model.LiheAllData.GetLiheAppvadpyas(BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, BaseForm.BaseYear, BaseForm.BaseApplicationNo, null, null, null);



            CASEVDDEntity Search_VddEntity = new CASEVDDEntity(true);
            Search_Entity.Agency = BaseForm.BaseAgency;
            List<CASEVDDEntity> CasevddList = _model.SPAdminData.Browse_CASEVDD(Search_VddEntity, "Browse");

            cb.BeginText();
            X_Pos = 300; Y_Pos = 580;
            cb.SetFontAndSize(bf_helv, 13);
            string Header_Desc = string.Empty; string Form_Selection = string.Empty;

            //if (Privileges.ModuleCode == "03")
            //{
            string ShortName = string.Empty;

            if (dtCaseHie.Rows.Count > 0)
            {
                foreach (DataRow drCasehie in dtCaseHie.Rows)
                {
                    if (drCasehie["Code"].ToString().Trim() == BaseForm.BaseAgency)
                    {
                        ShortName = drCasehie["HIE_NAME"].ToString().Trim(); break;
                    }
                }
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "STATE OF CONNECTICUT -- DEPARTMENT OF SOCIAL SERVICES", X_Pos, Y_Pos, 0);

                Header_Desc = ShortName;
                Form_Selection = "ENERGY ASSISTANCE APPLICATION";
                cb.SetFontAndSize(bf_helv, 10);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Applicant I.D. No: ", 650, Y_Pos, 0);
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(BaseForm.BaseApplicationNo, Timesline), 690, Y_Pos, 0);

                Y_Pos -= 20;
                cb.SetFontAndSize(bf_timesBold, 13);
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, Form_Selection, X_Pos, Y_Pos, 0);
            }

            //cb.SetFontAndSize(bf_helv, 9);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Applicant I.D. No: ", 650, Y_Pos, 0);
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, Timesline), 690, Y_Pos, 0);
            cb.SetFontAndSize(bf_helv, 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Application Date: ", 650, Y_Pos, 0);
            if (!string.IsNullOrEmpty(drCaseMST["MST_INTAKE_DATE"].ToString().Trim()))
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(LookupDataAccess.Getdate(drCaseMST["MST_INTAKE_DATE"].ToString().Trim()), Timesline), 695, Y_Pos, 0);
            else
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase("__________", Times), 700, Y_Pos, 0);

            cb.SetFontAndSize(bf_helv, 13);
            Barcode128 bc39 = new Barcode128();
            bc39.Code = BaseForm.BaseAgency + BaseForm.BaseDept + BaseForm.BaseProg + BaseForm.BaseYear + BaseForm.BaseApplicationNo.Trim();
            // comment next line to show barcode text
            //bc39.Font = null;
            bc39.StartStopText = false;
            bc39.CodeType = iTextSharp.text.pdf.Barcode128.CODE128;
            bc39.Extended = true;
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap(bc39.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White));
            iTextSharp.text.Image barCodeImage = bc39.CreateImageWithBarcode(cb, null, null);

            //cb.SetTextMatrix(720,560);
            //barCodeImage.ScaleToFit(30,600);
            barCodeImage.SetAbsolutePosition(730, 490);
            barCodeImage.RotationDegrees = 90;
            barCodeImage.Rotate();
            cb.AddImage(barCodeImage);


            Y_Pos -= 15; X_Pos = 15;
            cb.SetFontAndSize(bf_helv, 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Do you have a disability and need an accommodation or special help to complete this application?", X_Pos, Y_Pos, 0);

            ///************************************CheckBoxes****************************/
            iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(806, 40, 788, 60);
            PdfAppearance[] onOff = new PdfAppearance[2];
            onOff[0] = cb.CreateAppearance(20, 20);
            onOff[0].Rectangle(1, 20, 1, 20);
            onOff[0].Rectangle(18, 18, 1, 1);
            onOff[0].Stroke();
            onOff[1] = cb.CreateAppearance(20, 20);
            onOff[1].SetRGBColorFill(255, 128, 128);
            onOff[1].Rectangle(18, 18, 1, 1);
            onOff[1].FillStroke();
            onOff[1].MoveTo(1, 1);
            onOff[1].LineTo(19, 19);
            onOff[1].MoveTo(1, 19);
            onOff[1].LineTo(19, 1);

            RadioCheckField checkbox;
            PdfFormField SField;
            //if (Privileges.ModuleCode == "08")
            //{
            rect = new iTextSharp.text.Rectangle(440, Y_Pos + 8, 448, Y_Pos);
            //rect.Rotate();
            checkbox = new RadioCheckField(writer, rect, "Yes", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Yes", Times), 450, Y_Pos, 0);
            rect = new iTextSharp.text.Rectangle(470, Y_Pos + 8, 478, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "No", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("No", Times), 480, Y_Pos, 0);

            Y_Pos -= 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Applicant Name   ", X_Pos, Y_Pos, 0);
            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, Timesline), X_Pos + 72, Y_Pos, 0);

            string Language = null;
            DataSet dsLang = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.LANGUAGECODES);
            DataTable dtLang = dsLang.Tables[0];
            foreach (DataRow drLang in dtLang.Rows)
            {
                if (drCaseMST["MST_LANGUAGE"].ToString().Trim() == drLang["Code"].ToString().Trim())
                {
                    Language = drLang["LookUpDesc"].ToString().Trim(); break;
                }
            }

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Primary Language ", 380, Y_Pos, 0);
            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Language, Timesline), 460, Y_Pos, 0);

            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "DSS Client I.D. #   __________", 640, Y_Pos, 0);
            //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Site_Name, Timesline), 650, Y_Pos, 0);

            Y_Pos -= 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Mailing Address", X_Pos, Y_Pos, 0);
            string House_NO = null, Street = null, city = null, state = null, zip = null, DApt = null; string DSuffix = string.Empty; string strDirection = string.Empty;
            if (dtCasediff.Rows.Count > 0)
            {
                foreach (DataRow drCaseDiff in dtCasediff.Rows)
                {
                    if (!string.IsNullOrEmpty(drCaseDiff["DIFF_HN"].ToString().Trim()))
                        House_NO = drCaseDiff["DIFF_HN"].ToString().Trim() + " ";
                    if (!string.IsNullOrEmpty(drCaseDiff["DIFF_APT"].ToString().Trim()))
                        DApt = drCaseDiff["DIFF_APT"].ToString().Trim() + " ";
                    if (!string.IsNullOrEmpty(drCaseDiff["DIFF_SUFFIX"].ToString().Trim()))
                        DSuffix = " " + drCaseDiff["DIFF_SUFFIX"].ToString().Trim();
                    if (!string.IsNullOrEmpty(drCaseDiff["DIFF_STREET"].ToString().Trim()))
                        Street = drCaseDiff["DIFF_STREET"].ToString().Trim() + DSuffix + ",";


                    if (!string.IsNullOrEmpty(drCaseDiff["DIFF_DIRECTION"].ToString().Trim()))
                        strDirection = drCaseDiff["DIFF_DIRECTION"].ToString().Trim() + " ";

                    if (!string.IsNullOrEmpty(drCaseDiff["DIFF_CITY"].ToString().Trim()))
                        city = drCaseDiff["DIFF_CITY"].ToString().Trim() + ",";
                    if (!string.IsNullOrEmpty(drCaseDiff["DIFF_STATE"].ToString().Trim()))
                        state = drCaseDiff["DIFF_STATE"].ToString().Trim();
                    if (!string.IsNullOrEmpty(drCaseDiff["DIFF_ZIP"].ToString().Trim()))
                        zip = "00000".Substring(0, 5 - drCaseDiff["DIFF_ZIP"].ToString().Trim().Length) + drCaseDiff["DIFF_ZIP"].ToString().Trim();
                    if (zip == "00000") zip = ""; else zip = ", " + zip;
                }
                if (!string.IsNullOrEmpty((House_NO + Street + city + state + zip).Trim()))
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(House_NO + strDirection + Street + DApt + city + state + zip, Timesline), X_Pos + 72, Y_Pos, 0);
                else
                {
                    cb.SetLineWidth(0.5f);
                    //cb.SetLineCap(5);
                    cb.MoveTo(X_Pos + 72, Y_Pos);
                    cb.LineTo(210, Y_Pos);
                    cb.Stroke();
                }
            }
            else
            {
                cb.SetLineWidth(0.5f);
                //cb.SetLineCap(5);
                cb.MoveTo(X_Pos + 72, Y_Pos);
                cb.LineTo(210, Y_Pos);
                cb.Stroke();
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(drCaseMST["MST_HN"].ToString().Trim() + " " + drCaseMST["MST_STREET"].ToString().Trim() + "," + drCaseMST["MST_CITY"].ToString().Trim() + "," + drCaseMST["MST_STATE"].ToString().Trim() + "," + drCaseMST["MST_ZIP"].ToString().Trim(), Timesline), X_Pos + 72, Y_Pos, 0);
            }

            if (!string.IsNullOrEmpty(drCaseMST["MST_EMAIL"].ToString().Trim()))
            {
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Email :", Times), 380, Y_Pos, 0);
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(drCaseMST["MST_EMAIL"].ToString().Trim(), Timesline), 410, Y_Pos, 0);
            }
            else
            {
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Email", 380, Y_Pos, 0);
                cb.SetLineWidth(0.5f);
                //cb.SetLineCap(5);
                cb.MoveTo(410, Y_Pos);
                cb.LineTo(520, Y_Pos);
                cb.Stroke();
            }


            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Home Telephone ", 640, Y_Pos, 0);
            MaskedTextBox mskPhn = new MaskedTextBox();
            mskPhn.Mask = "(000)000-0000";
            mskPhn.Text = drCaseMST["MST_AREA"].ToString() + drCaseMST["MST_PHONE"].ToString();   //"(" + drCaseMST["MST_AREA"].ToString() + ")" + drCaseMST["MST_PHONE"].ToString()
            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskPhn.Text, Timesline), 650, Y_Pos, 0);

            Y_Pos -= 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Service Address", X_Pos, Y_Pos, 0);
            if (!string.IsNullOrEmpty(drCaseMST["MST_STATE"].ToString().Trim()))
            {
                string Apt = string.Empty; string Suffix = string.Empty; string Zip = string.Empty; string Floor = string.Empty; string Addr = string.Empty;
                if (!string.IsNullOrEmpty(drCaseMST["MST_APT"].ToString().Trim()))
                    Apt = " Apt:" + drCaseMST["MST_APT"].ToString().Trim() + ", ";
                if (!string.IsNullOrEmpty(drCaseMST["MST_FLR"].ToString().Trim()))
                    Floor = "Flr: " + drCaseMST["MST_FLR"].ToString().Trim() + ", ";
                if (!string.IsNullOrEmpty(drCaseMST["MST_SUFFIX"].ToString().Trim()))
                    Suffix = drCaseMST["MST_SUFFIX"].ToString().Trim() + ", ";

                strDirection = string.Empty;
                if (!string.IsNullOrEmpty(drCaseMST["MST_DIRECTION"].ToString().Trim()))
                    strDirection = drCaseMST["MST_DIRECTION"].ToString().Trim() + " ";

                if (!string.IsNullOrEmpty(drCaseMST["MST_ZIP"].ToString().Trim()))
                    Zip = "00000".Substring(0, 5 - drCaseMST["MST_ZIP"].ToString().Trim().Length) + drCaseMST["MST_ZIP"].ToString().Trim();
                Addr = Apt + Floor;
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(drCaseMST["MST_HN"].ToString().Trim() + " " + strDirection + drCaseMST["MST_STREET"].ToString().Trim() + "  " + Suffix + Addr + drCaseMST["MST_CITY"].ToString().Trim() + ", " + drCaseMST["MST_STATE"].ToString().Trim() + ", " + Zip, Timesline), X_Pos + 72, Y_Pos, 0);
            }
            else
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("_________________________________________________", Times), X_Pos + 72, Y_Pos, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Cell Number", 640, Y_Pos, 0);
            if (!string.IsNullOrEmpty(drCaseMST["MST_CELL_PHONE"].ToString().Trim()))
            {
                MaskedTextBox mskCell = new MaskedTextBox();
                mskCell.Mask = "(000)000-0000";
                mskCell.Text = drCaseMST["MST_CELL_PHONE"].ToString().Trim();
                ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskCell.Text, Timesline), 650, Y_Pos, 0);
            }
            else
            {
                cb.SetLineWidth(0.5f);
                //cb.SetLineCap(5);
                cb.MoveTo(650, Y_Pos - 3);
                cb.LineTo(705, Y_Pos - 3);
                cb.Stroke();
            }
            //cb.EndText();

            Y_Pos -= 15;
            int Count = dtCaseSNP.Rows.Count;
            int disable = 0, FoodStamps = 0, under5 = 0;
            foreach (DataRow drsnp in dtCaseSNP.Rows)
            {
                if (drsnp["SNP_DISABLE"].ToString().Trim() == "Y")
                    disable++;
                if (drsnp["SNP_FOOD_STAMPS"].ToString().Trim() == "Y")
                    FoodStamps++;
                //if (!string.IsNullOrEmpty(drsnp["SNP_AGE"].ToString()))
                //{
                //    if (int.Parse(drsnp["SNP_AGE"].ToString()) >= 18)
                //        Adults++;
                //    else
                //        Child++;
                //    if (int.Parse(drsnp["SNP_AGE"].ToString()) < 5)
                //        under5++;
                //}
            }
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Total No of Household Members: ", X_Pos, Y_Pos, 0);
            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(Count.ToString(), Timesline), X_Pos + 138, Y_Pos, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Number of Persons Disabled: ", 220, Y_Pos, 0);
            ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(disable.ToString(), Timesline), 340, Y_Pos, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Does anyone in the household receive SNAP benefits?", 400, Y_Pos, 0);

            rect = new iTextSharp.text.Rectangle(630, Y_Pos + 8, 638, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "SecondYes", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            if (FoodStamps > 0)
                checkbox.Checked = true;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Yes", Times), 640, Y_Pos, 0);
            rect = new iTextSharp.text.Rectangle(660, Y_Pos + 8, 668, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "SecondNo", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            if (FoodStamps == 0)
                checkbox.Checked = true;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("No", Times), 670, Y_Pos, 0);

            cb.EndText();

            ////Temp table not displayed on the screen
            PdfPTable head = new PdfPTable(1);
            head.HorizontalAlignment = Element.ALIGN_CENTER;
            head.TotalWidth = 50f;
            PdfPCell headcell = new PdfPCell(new Phrase(""));
            headcell.HorizontalAlignment = Element.ALIGN_CENTER;
            headcell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            head.AddCell(headcell);


            #region To Print SNP Details in the Table
            PdfPTable Snp_Table = new PdfPTable(13);
            Snp_Table.TotalWidth = 750f;
            Snp_Table.WidthPercentage = 100;
            Snp_Table.LockedWidth = true;
            float[] widths = new float[] { 65f, 45f, 25f, 25f, 11f, 13f, 31f, 28f, 40f, 19f, 18f, 22f, 25f };// 55f, 45f, 25f, 25f, 11f, 13f, 31f, 28f, 40f, 19f, 18f, 22f, 35f;
            Snp_Table.SetWidths(widths);
            Snp_Table.HorizontalAlignment = Element.ALIGN_CENTER;
            Snp_Table.SpacingBefore = 100f;

            PdfPCell Header = new PdfPCell(new Phrase("Listing yourself first, complete all spaces below for ALL persons living in the home. Use a separate sheet of paper if necessary.", TblFontBold));
            Header.Colspan = 13;
            Header.FixedHeight = 15f;
            Header.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(Header);


            //PdfPCell Header = new PdfPCell(new Phrase("HOUSEHOLD MEMBERS Listing yourself first, complete all spaces below for ALL persons living in the home.", TblFontBold));
            //Header.Colspan = 15;
            //Header.FixedHeight = 15f;
            //Header.BackgroundColor = BaseColor.LIGHT_GRAY;
            //Header.Border = iTextSharp.text.Rectangle.BOX;
            //Snp_Table.AddCell(Header);

            PdfPCell row2 = new PdfPCell(new Phrase(""));
            row2.Colspan = 9;
            row2.FixedHeight = 15f;
            row2.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row2);

            PdfPCell row2_Health = new PdfPCell(new Phrase("Health", TableFontBoldItalic));
            row2_Health.HorizontalAlignment = Element.ALIGN_CENTER;
            row2_Health.FixedHeight = 15f;
            row2_Health.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row2_Health);

            PdfPCell row2_Emp = new PdfPCell(new Phrase("", TableFontBoldItalic));
            row2_Emp.HorizontalAlignment = Element.ALIGN_CENTER;
            row2_Emp.FixedHeight = 15f;
            row2_Emp.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row2_Emp);

            PdfPCell row2_Rece = new PdfPCell(new Phrase("Receive", TableFontBoldItalic));
            row2_Rece.HorizontalAlignment = Element.ALIGN_CENTER;
            row2_Rece.FixedHeight = 15f;
            row2_Rece.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER + iTextSharp.text.Rectangle.RIGHT_BORDER;
            Snp_Table.AddCell(row2_Rece);

            PdfPCell row2_Space = new PdfPCell(new Phrase(""));
            //row2_Space.Colspan = 3;
            row2_Space.FixedHeight = 15f;
            row2_Space.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row2_Space);

            //PdfPCell row3 = new PdfPCell(new Phrase(""));
            //row3.Colspan = 3;
            //row3.FixedHeight = 15f;
            //row3.Border = iTextSharp.text.Rectangle.BOX;
            //Snp_Table.AddCell(row3);

            PdfPCell row3 = new PdfPCell(new Phrase(""));
            row3.Colspan = 2;
            row3.FixedHeight = 15f;
            row3.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row3);

            PdfPCell row3_SSN = new PdfPCell(new Phrase("Social", TableFontBoldItalic));
            row3_SSN.HorizontalAlignment = Element.ALIGN_CENTER;
            row3_SSN.FixedHeight = 15f;
            row3_SSN.Border = iTextSharp.text.Rectangle.TOP_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER + iTextSharp.text.Rectangle.RIGHT_BORDER;
            Snp_Table.AddCell(row3_SSN);

            PdfPCell row3_Birth = new PdfPCell(new Phrase("BirthDate", TableFontBoldItalic));
            row3_Birth.HorizontalAlignment = Element.ALIGN_CENTER;
            row3_Birth.FixedHeight = 15f;
            row3_Birth.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row3_Birth);

            PdfPCell row3_Space = new PdfPCell(new Phrase(""));
            //row3_Space.Colspan = 2;
            row3_Space.FixedHeight = 15f;
            row3_Space.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row3_Space);

            PdfPCell row3_Sex = new PdfPCell(new Phrase("Sex", TableFontBoldItalic));
            row3_Sex.HorizontalAlignment = Element.ALIGN_CENTER;
            row3_Sex.FixedHeight = 15f;
            row3_Sex.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row3_Sex);

            PdfPCell row3_Space2 = new PdfPCell(new Phrase(""));
            row3_Space2.Colspan = 3;
            row3_Space2.HorizontalAlignment = Element.ALIGN_CENTER;
            row3_Space2.FixedHeight = 15f;
            row3_Space2.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row3_Space2);

            PdfPCell row3_Insurance = new PdfPCell(new Phrase("Insurance ", TableFontBoldItalic));
            row3_Insurance.HorizontalAlignment = Element.ALIGN_CENTER;
            row3_Insurance.FixedHeight = 15f;
            row3_Insurance.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row3_Insurance);

            PdfPCell row3_Veteran = new PdfPCell(new Phrase("Veteran", TableFontBoldItalic));
            row3_Veteran.HorizontalAlignment = Element.ALIGN_CENTER;
            row3_Veteran.FixedHeight = 15f;
            row3_Veteran.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row3_Veteran);

            PdfPCell row3_Receive_FS = new PdfPCell(new Phrase("SNAP", TableFontBoldItalic));
            row3_Receive_FS.HorizontalAlignment = Element.ALIGN_CENTER;
            row3_Receive_FS.FixedHeight = 15f;
            row3_Receive_FS.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER + iTextSharp.text.Rectangle.RIGHT_BORDER;
            Snp_Table.AddCell(row3_Receive_FS);

            PdfPCell row3_Space3 = new PdfPCell(new Phrase("", TableFontBoldItalic));
            row3_Space3.HorizontalAlignment = Element.ALIGN_CENTER;
            row3_Space3.FixedHeight = 15f;
            row3_Space3.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(row3_Space3);

            string[] col = { "Name (last, first, MI)", "Relationship to Applicant", "Security", "mm/dd/yy", "Age", "M/F", "Ethnicity", "Race", "Education", "Y/N", "Y/N", "Y/N", "Disabled" };
            for (int i = 0; i < col.Length; ++i)
            {
                PdfPCell cell = new PdfPCell(new Phrase(col[i], TableFontBoldItalic));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.FixedHeight = 15f;
                if (i == 2) cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER + iTextSharp.text.Rectangle.LEFT_BORDER + iTextSharp.text.Rectangle.RIGHT_BORDER;
                else cell.Border = iTextSharp.text.Rectangle.BOX;
                Snp_Table.AddCell(cell);
            }

            int Tbl_Count = 0; string FamSeq = string.Empty;
            List<CaseSnpEntity> snplist = new List<CaseSnpEntity>();
            foreach (CaseSnpEntity entity in BaseForm.BaseCaseSnpEntity)
            {
                if (BaseForm.BaseCaseMstListEntity[0].FamilySeq == entity.FamilySeq)
                {
                    FamSeq = entity.FamilySeq.Trim();
                    string ApplicantName = entity.NameixLast + " " + entity.NameixFi + " " + entity.NameixMi;//snpEntity.NameixFi.Trim() + " " + snpEntity.NameixLast.Trim();
                    PdfPCell Name = new PdfPCell(new Phrase(ApplicantName, TableFont));
                    Name.HorizontalAlignment = Element.ALIGN_LEFT;
                    Name.FixedHeight = 15f;
                    Name.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Name);

                    string Relation = null;
                    DataSet dsRelation = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.RELATIONSHIP);
                    DataTable dtRelation = dsRelation.Tables[0];
                    foreach (DataRow drRelation in dtRelation.Rows)
                    {
                        if (entity.MemberCode.Trim() == drRelation["Code"].ToString().Trim())
                        {
                            Relation = drRelation["LookUpDesc"].ToString().Trim(); break;
                        }
                    }
                    PdfPCell RelationShip = new PdfPCell(new Phrase(Relation, TableFont));
                    RelationShip.HorizontalAlignment = Element.ALIGN_LEFT;
                    RelationShip.FixedHeight = 15f;
                    RelationShip.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(RelationShip);
                    string SSNNum = null;
                    if (!string.IsNullOrEmpty(entity.Ssno.Trim()))
                    {
                        if (entity.Ssno.Trim().Length < 9)
                            entity.Ssno = SetLeadingZeros(entity.Ssno.Trim());

                        //SSNNum = drCaseSNP["SNP_SSNO"].ToString().Trim();
                        SSNNum = "xxx" + "-" + "xx" + "-" + entity.Ssno.Trim().Substring(5, 4);
                    }
                    PdfPCell SSN = new PdfPCell(new Phrase(SSNNum, TableFont));
                    SSN.HorizontalAlignment = Element.ALIGN_CENTER;
                    SSN.FixedHeight = 15f;
                    SSN.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(SSN);

                    string DOB = string.Empty;
                    if (!string.IsNullOrEmpty(entity.AltBdate))
                    {
                        DOB = CommonFunctions.ChangeDateFormat(entity.AltBdate.Trim(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);
                    }
                    PdfPCell BirthDate = new PdfPCell(new Phrase(LookupDataAccess.Getdate(entity.AltBdate.Trim()), TableFont));
                    BirthDate.HorizontalAlignment = Element.ALIGN_CENTER;
                    BirthDate.FixedHeight = 15f;
                    BirthDate.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(BirthDate);

                    if (entity.Age.Trim() != "0")
                    {
                        PdfPCell Age = new PdfPCell(new Phrase(entity.Age.Trim(), TableFont));
                        Age.HorizontalAlignment = Element.ALIGN_CENTER;
                        Age.FixedHeight = 15f;
                        Age.Border = iTextSharp.text.Rectangle.BOX;
                        Snp_Table.AddCell(Age);
                    }
                    else
                    {
                        PdfPCell Age = new PdfPCell(new Phrase("", TableFont));
                        Age.HorizontalAlignment = Element.ALIGN_CENTER;
                        Age.FixedHeight = 15f;
                        Age.Border = iTextSharp.text.Rectangle.BOX;
                        Snp_Table.AddCell(Age);
                    }

                    PdfPCell Sex = new PdfPCell(new Phrase(entity.Sex.Trim(), TableFont));
                    Sex.HorizontalAlignment = Element.ALIGN_CENTER;
                    Sex.FixedHeight = 15f;
                    Sex.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Sex);

                    string Etinic = null;
                    DataSet dsEtinic = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.ETHNICODES);
                    DataTable dtEtinic = dsEtinic.Tables[0];
                    foreach (DataRow drEtinic in dtEtinic.Rows)
                    {
                        if (entity.Ethnic.Trim() == drEtinic["Code"].ToString().Trim())
                        {
                            Etinic = drEtinic["LookUpDesc"].ToString().Trim(); break;
                        }
                    }
                    PdfPCell Snp_Ethnic = new PdfPCell(new Phrase(Etinic, TableFont));
                    Snp_Ethnic.HorizontalAlignment = Element.ALIGN_LEFT;
                    Snp_Ethnic.FixedHeight = 15f;
                    Snp_Ethnic.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Snp_Ethnic);

                    string Race = null;
                    DataSet dsRace = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.RACE);
                    DataTable dtRace = dsRace.Tables[0];
                    foreach (DataRow drRace in dtRace.Rows)
                    {
                        if (entity.Race.Trim() == drRace["Code"].ToString().Trim())
                        {
                            Race = drRace["LookUpDesc"].ToString().Trim(); break;
                        }
                    }
                    PdfPCell Snp_Race = new PdfPCell(new Phrase(Race, TableFont));
                    Snp_Race.HorizontalAlignment = Element.ALIGN_LEFT;
                    Snp_Race.FixedHeight = 15f;
                    Snp_Race.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Snp_Race);

                    string Education = null;
                    DataSet dsEducation = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.EDUCATIONCODES);
                    DataTable dtEducation = dsEducation.Tables[0];
                    foreach (DataRow drEducation in dtEducation.Rows)
                    {
                        if (entity.Education.Trim() == drEducation["Code"].ToString().Trim())
                        {
                            Education = drEducation["LookUpDesc"].ToString().Trim(); break;
                        }
                    }
                    PdfPCell Snp_Education = new PdfPCell(new Phrase(Education, TableFont));
                    Snp_Education.HorizontalAlignment = Element.ALIGN_LEFT;
                    Snp_Education.FixedHeight = 15f;
                    Snp_Education.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Snp_Education);

                    PdfPCell Insurance = new PdfPCell(new Phrase(entity.HealthIns.Trim(), TableFont));
                    Insurance.HorizontalAlignment = Element.ALIGN_CENTER;
                    Insurance.FixedHeight = 15f;
                    Insurance.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Insurance);

                    if (Roma_Switch == "Y")
                    {
                        string Vet = string.Empty;
                        if (entity.MilitaryStatus.Trim() == "V") Vet = "Y"; else if (!string.IsNullOrEmpty(entity.MilitaryStatus.Trim().Trim())) Vet = "N";
                        PdfPCell Vetran = new PdfPCell(new Phrase(Vet, TableFont));
                        Vetran.HorizontalAlignment = Element.ALIGN_CENTER;
                        Vetran.FixedHeight = 15f;
                        Vetran.Border = iTextSharp.text.Rectangle.BOX;
                        Snp_Table.AddCell(Vetran);
                    }
                    else
                    {
                        PdfPCell Vetran = new PdfPCell(new Phrase(entity.Vet.Trim(), TableFont));
                        Vetran.HorizontalAlignment = Element.ALIGN_CENTER;
                        Vetran.FixedHeight = 15f;
                        Vetran.Border = iTextSharp.text.Rectangle.BOX;
                        Snp_Table.AddCell(Vetran);
                    }

                    PdfPCell FoodStamps1 = new PdfPCell(new Phrase(entity.FootStamps.Trim(), TableFont));
                    FoodStamps1.HorizontalAlignment = Element.ALIGN_CENTER;
                    FoodStamps1.FixedHeight = 15f;
                    FoodStamps1.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(FoodStamps1);


                    string AGYDisable = null;
                    DataSet dsDisable = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.DISABLED);
                    DataTable dtDisable = dsDisable.Tables[0];
                    foreach (DataRow drDisable in dtDisable.Rows)
                    {
                        if (entity.Disable.Trim() == drDisable["Code"].ToString().Trim())
                            AGYDisable = drDisable["LookUpDesc"].ToString().Trim();
                    }
                    PdfPCell Disabled = new PdfPCell(new Phrase(AGYDisable, TableFont));
                    Disabled.HorizontalAlignment = Element.ALIGN_LEFT;
                    Disabled.FixedHeight = 15f;
                    Disabled.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Disabled);
                }
            }

            string MotherName = string.Empty; string FatherName = string.Empty;
            string M_Address = string.Empty; string F_Address = string.Empty;
            string M_Phone = string.Empty; string F_Phone = string.Empty;
            string M_FullPart = string.Empty; string F_FullPart = string.Empty;
            foreach (DataRow drCaseSNP in dtCaseSNP.Rows)
            {
                if (FamSeq != drCaseSNP["SNP_FAMILY_SEQ"].ToString().Trim())
                {
                    if (MotherEntity != null)
                    {
                        if (drCaseSNP["SNP_MEMBER_CODE"].ToString() == MotherEntity.Code)
                        {
                            MotherName = drCaseSNP["SNP_EMPLOYER_NAME"].ToString().Trim();
                            if (!string.IsNullOrEmpty(drCaseSNP["SNP_EMPLOYER_STREET"].ToString().Trim()))
                                M_Address = drCaseSNP["SNP_EMPLOYER_STREET"].ToString().Trim() + ",";
                            if (!string.IsNullOrEmpty(drCaseSNP["SNP_EMPLOYER_CITY"].ToString().Trim()))
                                M_Address = drCaseSNP["SNP_EMPLOYER_CITY"].ToString().Trim();
                            if (!string.IsNullOrEmpty(drCaseSNP["SNP_EMPL_PHONE"].ToString().Trim()))
                                M_Phone = drCaseSNP["SNP_EMPL_PHONE"].ToString().Trim();
                            if (drCaseSNP["SNP_FULL_TIME_HOURS"].ToString().Trim() != "0")
                                M_FullPart = "F";
                            else if (drCaseSNP["SNP_PART_TIME_HOURS"].ToString().Trim() != "0")
                                M_FullPart = "P";
                        }
                    }

                    if (FatherEntity.Count > 0)
                    {
                        foreach (CommonEntity cm in FatherEntity)
                        {
                            if (cm.Code == drCaseSNP["SNP_MEMBER_CODE"].ToString())
                            {
                                FatherName = drCaseSNP["SNP_EMPLOYER_NAME"].ToString().Trim();
                                if (!string.IsNullOrEmpty(drCaseSNP["SNP_EMPLOYER_STREET"].ToString().Trim()))
                                    F_Address = drCaseSNP["SNP_EMPLOYER_STREET"].ToString().Trim() + ",";
                                if (!string.IsNullOrEmpty(drCaseSNP["SNP_EMPLOYER_CITY"].ToString().Trim()))
                                    F_Address = drCaseSNP["SNP_EMPLOYER_CITY"].ToString().Trim();
                                if (!string.IsNullOrEmpty(drCaseSNP["SNP_EMPL_PHONE"].ToString().Trim()))
                                    F_Phone = drCaseSNP["SNP_EMPL_PHONE"].ToString().Trim();
                                if (drCaseSNP["SNP_FULL_TIME_HOURS"].ToString().Trim() != "0")
                                    F_FullPart = "F";
                                else if (drCaseSNP["SNP_PART_TIME_HOURS"].ToString().Trim() != "0")
                                    F_FullPart = "P";
                                break;
                            }
                        }
                    }

                    string ApplicantName = drCaseSNP["SNP_NAME_IX_LAST"].ToString().Trim() + " " + drCaseSNP["SNP_NAME_IX_FI"].ToString().Trim() + " " + drCaseSNP["SNP_NAME_IX_MI"].ToString().Trim();//snpEntity.NameixFi.Trim() + " " + snpEntity.NameixLast.Trim();
                    PdfPCell Name = new PdfPCell(new Phrase(ApplicantName, TableFont));
                    Name.HorizontalAlignment = Element.ALIGN_LEFT;
                    Name.FixedHeight = 15f;
                    Name.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Name);

                    string Relation = null;
                    DataSet dsRelation = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.RELATIONSHIP);
                    DataTable dtRelation = dsRelation.Tables[0];
                    foreach (DataRow drRelation in dtRelation.Rows)
                    {
                        if (drCaseSNP["SNP_MEMBER_CODE"].ToString().Trim() == drRelation["Code"].ToString().Trim())
                        {
                            Relation = drRelation["LookUpDesc"].ToString().Trim(); break;
                        }
                    }
                    PdfPCell RelationShip = new PdfPCell(new Phrase(Relation, TableFont));
                    RelationShip.HorizontalAlignment = Element.ALIGN_LEFT;
                    RelationShip.FixedHeight = 15f;
                    RelationShip.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(RelationShip);
                    string SSNNum = null;
                    if (!string.IsNullOrEmpty(drCaseSNP["SNP_SSNO"].ToString().Trim()))
                    {
                        if (drCaseSNP["SNP_SSNO"].ToString().Trim().Length < 9)
                            drCaseSNP["SNP_SSNO"] = SetLeadingZeros(drCaseSNP["SNP_SSNO"].ToString().Trim());

                        SSNNum = "xxx" + "-" + "xx" + "-" + drCaseSNP["SNP_SSNO"].ToString().Trim().Substring(5, 4);
                    }
                    PdfPCell SSN = new PdfPCell(new Phrase(SSNNum, TableFont));
                    SSN.HorizontalAlignment = Element.ALIGN_CENTER;
                    SSN.FixedHeight = 15f;
                    SSN.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(SSN);

                    string DOB = string.Empty;
                    if (!string.IsNullOrEmpty(drCaseSNP["SNP_ALT_BDATE"].ToString()))
                    {
                        DOB = CommonFunctions.ChangeDateFormat(drCaseSNP["SNP_ALT_BDATE"].ToString().Trim(), Consts.DateTimeFormats.DateSaveFormat, Consts.DateTimeFormats.DateDisplayFormat);
                    }
                    PdfPCell BirthDate = new PdfPCell(new Phrase(LookupDataAccess.Getdate(drCaseSNP["SNP_ALT_BDATE"].ToString().Trim()), TableFont));
                    BirthDate.HorizontalAlignment = Element.ALIGN_CENTER;
                    BirthDate.FixedHeight = 15f;
                    BirthDate.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(BirthDate);

                    if (drCaseSNP["SNP_AGE"].ToString().Trim() != "0")
                    {
                        PdfPCell Age = new PdfPCell(new Phrase(drCaseSNP["SNP_AGE"].ToString().Trim(), TableFont));
                        Age.HorizontalAlignment = Element.ALIGN_CENTER;
                        Age.FixedHeight = 15f;
                        Age.Border = iTextSharp.text.Rectangle.BOX;
                        Snp_Table.AddCell(Age);
                    }
                    else
                    {
                        PdfPCell Age = new PdfPCell(new Phrase("", TableFont));
                        Age.HorizontalAlignment = Element.ALIGN_CENTER;
                        Age.FixedHeight = 15f;
                        Age.Border = iTextSharp.text.Rectangle.BOX;
                        Snp_Table.AddCell(Age);
                    }

                    PdfPCell Sex = new PdfPCell(new Phrase(drCaseSNP["SNP_SEX"].ToString().Trim(), TableFont));
                    Sex.HorizontalAlignment = Element.ALIGN_CENTER;
                    Sex.FixedHeight = 15f;
                    Sex.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Sex);

                    string Etinic = null;
                    DataSet dsEtinic = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.ETHNICODES);
                    DataTable dtEtinic = dsEtinic.Tables[0];
                    foreach (DataRow drEtinic in dtEtinic.Rows)
                    {
                        if (drCaseSNP["SNP_ETHNIC"].ToString().Trim() == drEtinic["Code"].ToString().Trim())
                        {
                            Etinic = drEtinic["LookUpDesc"].ToString().Trim(); break;
                        }
                    }
                    PdfPCell Snp_Ethnic = new PdfPCell(new Phrase(Etinic, TableFont));
                    Snp_Ethnic.HorizontalAlignment = Element.ALIGN_LEFT;
                    Snp_Ethnic.FixedHeight = 15f;
                    Snp_Ethnic.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Snp_Ethnic);

                    string Race = null;
                    DataSet dsRace = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.RACE);
                    DataTable dtRace = dsRace.Tables[0];
                    foreach (DataRow drRace in dtRace.Rows)
                    {
                        if (drCaseSNP["SNP_RACE"].ToString().Trim() == drRace["Code"].ToString().Trim())
                        {
                            Race = drRace["LookUpDesc"].ToString().Trim(); break;
                        }
                    }
                    PdfPCell Snp_Race = new PdfPCell(new Phrase(Race, TableFont));
                    Snp_Race.HorizontalAlignment = Element.ALIGN_LEFT;
                    Snp_Race.FixedHeight = 15f;
                    Snp_Race.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Snp_Race);

                    string Education = null;
                    DataSet dsEducation = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.EDUCATIONCODES);
                    DataTable dtEducation = dsEducation.Tables[0];
                    foreach (DataRow drEducation in dtEducation.Rows)
                    {
                        if (drCaseSNP["SNP_EDUCATION"].ToString().Trim() == drEducation["Code"].ToString().Trim())
                        {
                            Education = drEducation["LookUpDesc"].ToString().Trim(); break;
                        }
                    }
                    PdfPCell Snp_Education = new PdfPCell(new Phrase(Education, TableFont));
                    Snp_Education.HorizontalAlignment = Element.ALIGN_LEFT;
                    Snp_Education.FixedHeight = 15f;
                    Snp_Education.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Snp_Education);

                    PdfPCell Insurance = new PdfPCell(new Phrase(drCaseSNP["SNP_HEALTH_INS"].ToString().Trim(), TableFont));
                    Insurance.HorizontalAlignment = Element.ALIGN_CENTER;
                    Insurance.FixedHeight = 15f;
                    Insurance.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Insurance);

                    if (Roma_Switch == "Y")
                    {
                        string Vet = string.Empty;
                        if (drCaseSNP["SNP_MILITARY_STATUS"].ToString().Trim() == "V") Vet = "Y"; else if (!string.IsNullOrEmpty(drCaseSNP["SNP_MILITARY_STATUS"].ToString().Trim())) Vet = "N";
                        PdfPCell Vetran = new PdfPCell(new Phrase(Vet, TableFont));
                        Vetran.HorizontalAlignment = Element.ALIGN_CENTER;
                        Vetran.FixedHeight = 15f;
                        Vetran.Border = iTextSharp.text.Rectangle.BOX;
                        Snp_Table.AddCell(Vetran);
                    }
                    else
                    {
                        PdfPCell Vetran = new PdfPCell(new Phrase(drCaseSNP["SNP_VET"].ToString().Trim(), TableFont));
                        Vetran.HorizontalAlignment = Element.ALIGN_CENTER;
                        Vetran.FixedHeight = 15f;
                        Vetran.Border = iTextSharp.text.Rectangle.BOX;
                        Snp_Table.AddCell(Vetran);
                    }

                    PdfPCell FoodStamps1 = new PdfPCell(new Phrase(drCaseSNP["SNP_FOOD_STAMPS"].ToString().Trim(), TableFont));
                    FoodStamps1.HorizontalAlignment = Element.ALIGN_CENTER;
                    FoodStamps1.FixedHeight = 15f;
                    FoodStamps1.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(FoodStamps1);

                    string AGYDisable = null;
                    DataSet dsDisable = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.DISABLED);
                    DataTable dtDisable = dsDisable.Tables[0];
                    foreach (DataRow drDisable in dtDisable.Rows)
                    {
                        if (drCaseSNP["SNP_DISABLE"].ToString().Trim() == drDisable["Code"].ToString().Trim())
                            AGYDisable = drDisable["LookUpDesc"].ToString().Trim();
                    }
                    PdfPCell Disabled = new PdfPCell(new Phrase(AGYDisable, TableFont));
                    Disabled.HorizontalAlignment = Element.ALIGN_LEFT;
                    Disabled.FixedHeight = 15f;
                    Disabled.Border = iTextSharp.text.Rectangle.BOX;
                    Snp_Table.AddCell(Disabled);

                    Tbl_Count++;
                    if (Tbl_Count >= 7)
                        break;
                }
            }

            int Len_Var = 77 - Tbl_Count * 13;
            for (int j = 0; j <= Len_Var; ++j)  //120
            {
                PdfPCell SpaceCell = new PdfPCell(new Phrase(" ", TableFont));
                SpaceCell.HorizontalAlignment = Element.ALIGN_CENTER;
                SpaceCell.FixedHeight = 15f;
                SpaceCell.Border = iTextSharp.text.Rectangle.BOX;
                Snp_Table.AddCell(SpaceCell);
            }

            PdfPCell HSSnp = new PdfPCell(new Phrase("HOUSING/ENERGY DATA ", TblFontBoldS));
            HSSnp.HorizontalAlignment = Element.ALIGN_LEFT;
            HSSnp.FixedHeight = 15f;
            HSSnp.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(HSSnp);

            PdfPCell HSSnp1 = new PdfPCell(new Phrase("Note: Verification of rent or mortgage payment (if applicable) may be required.  Verification of your current bill is needed if you heat with electricity or natural gas.", TblFontBoldS));
            HSSnp1.HorizontalAlignment = Element.ALIGN_LEFT;
            HSSnp1.FixedHeight = 15f;
            HSSnp1.Colspan = 12;
            HSSnp1.Border = iTextSharp.text.Rectangle.BOX;
            Snp_Table.AddCell(HSSnp1);



            document.Add(head);
            document.Add(Snp_Table);
            //document.NewPage();

            ////End Of SNP details Table
            #endregion

            X_Pos = 15; Y_Pos = 260;//Y_Pos = 270;
            cb.BeginText();
            cb.SetFontAndSize(bf_helv, 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Do you own a home ?", X_Pos, Y_Pos, 0);

            rect = new iTextSharp.text.Rectangle(120, Y_Pos + 8, 128, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "ThirdYes", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            if (drCaseMST["MST_HOUSING"].ToString().Trim() == "A")
                checkbox.Checked = true;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Yes", Times), 130, Y_Pos, 0);
            rect = new iTextSharp.text.Rectangle(150, Y_Pos + 8, 158, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "ThirdNo", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            if (drCaseMST["MST_HOUSING"].ToString().Trim() != "A")
                checkbox.Checked = true;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("No", Times), 160, Y_Pos, 0);

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Do you pay a mortgage?", 220, Y_Pos, 0);

            rect = new iTextSharp.text.Rectangle(390, Y_Pos + 8, 398, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "FourthYes", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            if (float.Parse(drCaseMST["MST_EXP_RENT"].ToString().Trim()) > 0 && drCaseMST["MST_HOUSING"].ToString().Trim() == "A")
                checkbox.Checked = true;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Yes", Times), 400, Y_Pos, 0);
            rect = new iTextSharp.text.Rectangle(420, Y_Pos + 8, 428, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "FourthNo", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            if (float.Parse(drCaseMST["MST_EXP_RENT"].ToString().Trim()) <= 0 && drCaseMST["MST_HOUSING"].ToString().Trim() == "A")
                checkbox.Checked = true;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("No", Times), 430, Y_Pos, 0);


            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "If Yes, what is your monthly mortgage payment?", 490, Y_Pos, 0);
            if (drCaseMST["MST_HOUSING"].ToString().Trim() == "A")
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(drCaseMST["MST_EXP_RENT"].ToString().Trim(), Timesline), 700, Y_Pos, 0);
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("____________", Times), 700, Y_Pos, 0);


            X_Pos = 15; Y_Pos -= 15;

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Do you rent ?", X_Pos, Y_Pos, 0);

            rect = new iTextSharp.text.Rectangle(120, Y_Pos + 8, 128, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "FifthYes", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            if (drCaseMST["MST_HOUSING"].ToString().Trim() == "B")
                checkbox.Checked = true;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Yes", Times), 130, Y_Pos, 0);
            rect = new iTextSharp.text.Rectangle(150, Y_Pos + 8, 158, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "FifthNo", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            if (drCaseMST["MST_HOUSING"].ToString().Trim() != "B")
                checkbox.Checked = true;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("No", Times), 160, Y_Pos, 0);

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Do you live in subsidized rental housing?", 220, Y_Pos, 0);

            rect = new iTextSharp.text.Rectangle(390, Y_Pos + 8, 398, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "SixthYes", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Yes", Times), 400, Y_Pos, 0);
            rect = new iTextSharp.text.Rectangle(420, Y_Pos + 8, 428, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "SixthNo", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("No", Times), 430, Y_Pos, 0);


            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Monthly Rent Payment (your portion)", 490, Y_Pos, 0);
            if (drCaseMST["MST_HOUSING"].ToString().Trim() == "B")
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(drCaseMST["MST_EXP_RENT"].ToString().Trim(), Timesline), 660, Y_Pos, 0);
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("____________", Times), 660, Y_Pos, 0);

            Y_Pos -= 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Landlord or Agent Name or Company Name", X_Pos, Y_Pos, 0);
            if (dtLandlorddet.Rows.Count > 0)
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(dtLandlorddet.Rows[0]["LLR_FIRST_NAME"].ToString().Trim() + "  " + dtLandlorddet.Rows[0]["LLR_LAST_NAME"].ToString().Trim(), Timesline), 220, Y_Pos, 0);
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("_________________________________________________", Times), 220, Y_Pos, 0);

            Y_Pos -= 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Landlord Address", X_Pos, Y_Pos, 0);
            if (dtLandlorddet.Rows.Count > 0)
            {
                string Zip = string.Empty; string Apart = string.Empty; string Suffix = string.Empty;
                if (!string.IsNullOrEmpty(dtLandlorddet.Rows[0]["LLR_APT"].ToString().Trim()))
                    Apart = dtLandlorddet.Rows[0]["LLR_APT"].ToString().Trim() + ",";
                if (!string.IsNullOrEmpty(dtLandlorddet.Rows[0]["LLR_SUFFIX"].ToString().Trim()))
                    Suffix = dtLandlorddet.Rows[0]["LLR_SUFFIX"].ToString().Trim();
                if (dtLandlorddet.Rows[0]["LLR_ZIP"].ToString().Trim() != "0" || !string.IsNullOrEmpty(dtLandlorddet.Rows[0]["LLR_ZIP"].ToString().Trim()))
                    Zip = "00000".Substring(0, 5 - dtLandlorddet.Rows[0]["LLR_ZIP"].ToString().Trim().Length) + dtLandlorddet.Rows[0]["LLR_ZIP"].ToString().Trim();
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(dtLandlorddet.Rows[0]["LLR_HN"].ToString().Trim() + "  " + dtLandlorddet.Rows[0]["LLR_STREET"].ToString().Trim() + " " + Suffix + ", " + Apart + dtLandlorddet.Rows[0]["LLR_CITY"].ToString().Trim() + ", " + dtLandlorddet.Rows[0]["LLR_STATE"].ToString().Trim() + "  " + Zip, Timesline), 100, Y_Pos, 0);
            }
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("_________________________________________________", Times), 100, Y_Pos, 0);

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Landlord Telephone", 490, Y_Pos, 0);
            if (dtLandlorddet.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dtLandlorddet.Rows[0]["LLR_PHONE"].ToString().Trim()))
                {
                    MaskedTextBox mskCell = new MaskedTextBox();
                    mskCell.Mask = "(000)000-0000";
                    mskCell.Text = dtLandlorddet.Rows[0]["LLR_PHONE"].ToString().Trim();
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(mskCell.Text, Timesline), 600, Y_Pos, 0);
                }
                else
                    ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("_______________", Times), 600, Y_Pos, 0);
            }
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("_______________", Times), 600, Y_Pos, 0);

            Y_Pos -= 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Are you a roomer in someone else's home?", X_Pos, Y_Pos, 0);

            rect = new iTextSharp.text.Rectangle(200, Y_Pos + 8, 208, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "SeventhYes", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Yes", Times), 210, Y_Pos, 0);
            rect = new iTextSharp.text.Rectangle(230, Y_Pos + 8, 238, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "SeventhNo", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("No", Times), 240, Y_Pos, 0);

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Do you live rent-free in someone else's home?", 300, Y_Pos, 0);

            rect = new iTextSharp.text.Rectangle(520, Y_Pos + 8, 528, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "EigthYes", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Yes", Times), 530, Y_Pos, 0);
            rect = new iTextSharp.text.Rectangle(550, Y_Pos + 8, 558, Y_Pos);
            checkbox = new RadioCheckField(writer, rect, "EigthNo", "On");
            checkbox.BorderColor = new GrayColor(0.3f);
            checkbox.Rotation = 90;
            SField = checkbox.CheckField;
            writer.AddAnnotation(SField);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("No", Times), 560, Y_Pos, 0);

            Y_Pos -= 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "If you answer yes to either of these 2 questions,STOP, because the head of the household must complete the application", X_Pos, Y_Pos, 0);

            Y_Pos -= 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Type of Dwelling:", X_Pos, Y_Pos, 0);
            //DataSet dsdwelling = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.DWELLINGTYPE);
            //DataTable dtswelling = dsdwelling.Tables[0];
            PdfFormField Field;
            X_Pos = 100;
            string CheckTitle = string.Empty;
            List<CommonEntity> DwellingList = new List<CommonEntity>();
            DwellingList.Add(new CommonEntity("2", "Single Family"));
            DwellingList.Add(new CommonEntity("3", "Two Family"));
            DwellingList.Add(new CommonEntity("4", "3-5 Units"));
            DwellingList.Add(new CommonEntity("5", "6+ Units"));
            DwellingList.Add(new CommonEntity("1", "Mobile Home"));
            DwellingList.Add(new CommonEntity("0", "In-Law Apt."));
            DwellingList.Add(new CommonEntity("0", "Other(Specify)"));
            if (DwellingList.Count > 0)
            {
                foreach (CommonEntity drdwelling in DwellingList)
                {
                    //if (drdwelling["Active"].ToString() == "Y" ||
                    //    (drdwelling["Active"].ToString() == "N" && !string.IsNullOrEmpty(drCaseMST["MST_DWELLING"].ToString().Trim()) && drCaseMST["MST_DWELLING"].ToString().Trim() == drdwelling["Code"].ToString().Trim()))
                    //{
                    rect = new iTextSharp.text.Rectangle(X_Pos, Y_Pos + 8, X_Pos + 8, Y_Pos);
                    checkbox = new RadioCheckField(writer, rect, drdwelling.Desc.ToString().Trim(), "On");
                    checkbox.BorderColor = new GrayColor(0.3f);
                    checkbox.Rotation = 90;
                    if (drCaseMST["MST_DWELLING"].ToString().Trim() == drdwelling.Code.ToString().Trim())
                        checkbox.Checked = true;
                    Field = checkbox.CheckField;
                    writer.AddAnnotation(Field);
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(drdwelling.Desc.ToString().Trim(), Times), X_Pos + 10, Y_Pos, 0);
                    X_Pos += 90;
                    if (X_Pos > 600)
                    {
                        X_Pos = 100;
                        Y_Pos -= 13;
                    }
                    //}
                }
            }



            Y_Pos -= 15; X_Pos = 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Method of Paying Heat:", X_Pos, Y_Pos, 0);
            //DataSet dsdwelling = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.DWELLINGTYPE);
            //DataTable dtswelling = dsdwelling.Tables[0];
            X_Pos = 130;
            List<CommonEntity> PayingHeat = new List<CommonEntity>();
            PayingHeat.Add(new CommonEntity("2", "Heat Included in rent"));
            PayingHeat.Add(new CommonEntity("1", "Payment to vendor"));
            if (PayingHeat.Count > 0)
            {
                bool IsFalse = false;
                if (drCaseMST["MST_HEAT_INC_RENT"].ToString().Trim() == "2")
                    IsFalse = true;
                foreach (CommonEntity drPayingHeat in PayingHeat)
                {
                    //if (drdwelling["Active"].ToString() == "Y" ||
                    //    (drdwelling["Active"].ToString() == "N" && !string.IsNullOrEmpty(drCaseMST["MST_DWELLING"].ToString().Trim()) && drCaseMST["MST_DWELLING"].ToString().Trim() == drdwelling["Code"].ToString().Trim()))
                    //{

                    rect = new iTextSharp.text.Rectangle(X_Pos, Y_Pos + 8, X_Pos + 8, Y_Pos);
                    checkbox = new RadioCheckField(writer, rect, drPayingHeat.Desc.ToString().Trim(), "On");
                    checkbox.BorderColor = new GrayColor(0.3f);
                    checkbox.Rotation = 90;
                    if (IsFalse && drPayingHeat.Code.ToString().Trim() == "2")
                        checkbox.Checked = true;
                    else if ((!IsFalse) && drPayingHeat.Code.ToString().Trim() == "1")
                        checkbox.Checked = true;
                    Field = checkbox.CheckField;
                    writer.AddAnnotation(Field);
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(drPayingHeat.Desc.ToString().Trim(), Times), X_Pos + 10, Y_Pos, 0);
                    X_Pos += 90;
                    if (X_Pos > 600)
                    {
                        X_Pos = 100;
                        Y_Pos -= 13;
                    }
                    //}
                }

                //Y_Pos -= 15;
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Is your fuel tank shared with another household?", 350, Y_Pos, 0);
                rect = new iTextSharp.text.Rectangle(580, Y_Pos + 8, 588, Y_Pos);
                checkbox = new RadioCheckField(writer, rect, "NinthYes", "On");
                checkbox.BorderColor = new GrayColor(0.3f);
                checkbox.Rotation = 90;
                SField = checkbox.CheckField;
                writer.AddAnnotation(SField);
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Yes", Times), 590, Y_Pos, 0);
                rect = new iTextSharp.text.Rectangle(610, Y_Pos + 8, 618, Y_Pos);
                checkbox = new RadioCheckField(writer, rect, "NinthNo", "On");
                checkbox.BorderColor = new GrayColor(0.3f);
                checkbox.Rotation = 90;
                SField = checkbox.CheckField;
                writer.AddAnnotation(SField);
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("No", Times), 620, Y_Pos, 0);

            }


            Y_Pos -= 15; X_Pos = 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "What is your primary heating source?", X_Pos, Y_Pos, 0);
            //DataSet dsdwelling = DatabaseLayer.Lookups.GetLookUpFromAGYTAB(Consts.AgyTab.DWELLINGTYPE);
            //DataTable dtswelling = dsdwelling.Tables[0];
            X_Pos = 180;
            List<CommonEntity> HeatofSource = new List<CommonEntity>();
            HeatofSource.Add(new CommonEntity("01", "Oil"));
            HeatofSource.Add(new CommonEntity("02", "Natural Gas"));
            HeatofSource.Add(new CommonEntity("03", "Propane"));
            HeatofSource.Add(new CommonEntity("04", "Electric"));
            HeatofSource.Add(new CommonEntity("05", "Coal"));
            HeatofSource.Add(new CommonEntity("06", "Wood"));
            HeatofSource.Add(new CommonEntity("07", "Kerosene"));
            HeatofSource.Add(new CommonEntity("09", "Other( Specify) __________"));
            if (HeatofSource.Count > 0)
            {
                foreach (CommonEntity drHeat in HeatofSource)
                {
                    //if (drdwelling["Active"].ToString() == "Y" ||
                    //    (drdwelling["Active"].ToString() == "N" && !string.IsNullOrEmpty(drCaseMST["MST_DWELLING"].ToString().Trim()) && drCaseMST["MST_DWELLING"].ToString().Trim() == drdwelling["Code"].ToString().Trim()))
                    //{
                    rect = new iTextSharp.text.Rectangle(X_Pos, Y_Pos + 8, X_Pos + 8, Y_Pos);
                    checkbox = new RadioCheckField(writer, rect, drHeat.Desc.ToString().Trim(), "On");
                    checkbox.BorderColor = new GrayColor(0.3f);
                    checkbox.Rotation = 90;
                    if (drCaseMST["MST_SOURCE"].ToString().Trim() == drHeat.Code.ToString().Trim())
                        checkbox.Checked = true;
                    Field = checkbox.CheckField;
                    writer.AddAnnotation(Field);
                    ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(drHeat.Desc.ToString().Trim(), Times), X_Pos + 10, Y_Pos, 0);
                    X_Pos += 90;
                    if (X_Pos > 600)
                    {
                        X_Pos = 180;
                        Y_Pos -= 13;
                    }
                    //}
                }
                //ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase("Other( Specify) __________", Times), X_Pos + 10, Y_Pos, 0);
            }

            string Primary_vendor = string.Empty; string Primary_Account = string.Empty; string Primary_VAddress = string.Empty;
            string Electri_Vendor = string.Empty; string Elec_acc = string.Empty; string Elec_Acc_Name = string.Empty; string Primary_acc_Name = string.Empty;
            if (LiheapVDet.Count > 0)
            {
                foreach (LiheApvEntity Entity in LiheapVDet)
                {
                    if (Entity.LPV_PRIMARY_CODE == "P")
                    {
                        CASEVDDEntity VddList = CasevddList.Find(u => u.Code.Equals(Entity.LPV_VENDOR));
                        if (VddList != null)
                        {
                            Primary_vendor = VddList.Name.Trim(); Primary_Account = Entity.LPV_ACCOUNT_NO.Trim();
                            Primary_VAddress = VddList.Addr1.Trim() + "  " + VddList.City.Trim() + "  " + VddList.State.Trim() + "  " + VddList.Zip.Trim();
                            Primary_acc_Name = LookupDataAccess.GetMemberName(Entity.LPV_BILL_FNAME, string.Empty, Entity.LPV_BILL_LNAME, BaseForm.BaseHierarchyCnFormat);
                        }
                    }
                    if (Entity.LPV_PRIMARY_CODE == "S" && Entity.LPV_PAYMENT_FOR == "04")
                    {
                        CASEVDDEntity VddList = CasevddList.Find(u => u.Code.Equals(Entity.LPV_VENDOR));
                        if (VddList != null)
                        {
                            Electri_Vendor = VddList.Name.Trim(); Elec_acc = Entity.LPV_ACCOUNT_NO.Trim();
                            //Elec_Acc_Name = VddList.Name_On_Checks.Trim();  commented on 01/28/2020 by sudheer
                            Elec_Acc_Name = LookupDataAccess.GetMemberName(Entity.LPV_BILL_FNAME, string.Empty, Entity.LPV_BILL_LNAME, BaseForm.BaseHierarchyCnFormat);
                        }
                    }
                }
            }


            Y_Pos -= 15; X_Pos = 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "What is the name of your primary heating source fuel dealer or utility company?", X_Pos, Y_Pos, 0);
            if (!string.IsNullOrEmpty(Primary_vendor.Trim()))
            {
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(Primary_vendor, Timesline), 350, Y_Pos, 0);
            }
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("_________________________________________________", Times), 350, Y_Pos, 0);

            Y_Pos -= 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Address:", X_Pos, Y_Pos, 0);
            if (!string.IsNullOrEmpty(Primary_VAddress.Trim()))
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(Primary_VAddress, Timesline), 60, Y_Pos, 0);
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("__________________________________________", Times), 60, Y_Pos, 0);

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Name on the Account:", 330, Y_Pos, 0);
            if (!string.IsNullOrEmpty(Elec_Acc_Name.Trim()))
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(Primary_acc_Name, Timesline), 420, Y_Pos, 0);
            //if (dtLandlorddet.Rows.Count > 0)
            //    ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(dtLandlorddet.Rows[0]["LLR_HN"].ToString().Trim() + dtLandlorddet.Rows[0]["LLR_STREET"].ToString().Trim() + "," + dtLandlorddet.Rows[0]["LLR_APT"].ToString().Trim() + "," + dtLandlorddet.Rows[0]["LLR_CITY"].ToString().Trim() + "," + dtLandlorddet.Rows[0]["LLR_STATE"].ToString().Trim() + "00000".Substring(0, 5 - dtLandlorddet.Rows[0]["LLR_ZIP"].ToString().Trim().Length) + dtLandlorddet.Rows[0]["LLR_ZIP"].ToString().Trim(), Timesline), 100, Y_Pos, 0);
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("________________________________", Times), 420, Y_Pos, 0);

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Acct. No. :", 580, Y_Pos, 0);
            if (!string.IsNullOrEmpty(Primary_Account.Trim()))
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(Primary_Account, Timesline), 640, Y_Pos, 0);
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("____________________", Times), 640, Y_Pos, 0);

            Y_Pos -= 15;
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Electric Company Name:", X_Pos, Y_Pos, 0);
            if (!string.IsNullOrEmpty(Electri_Vendor.Trim()))
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(Electri_Vendor, Timesline), 120, Y_Pos, 0);
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("__________________________________________", Times), 120, Y_Pos, 0);

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Name on the Account:", 330, Y_Pos, 0);
            if (!string.IsNullOrEmpty(Elec_Acc_Name.Trim()))
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(Elec_Acc_Name, Timesline), 420, Y_Pos, 0);
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("________________________________", Times), 420, Y_Pos, 0);

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Acct. No. :", 580, Y_Pos, 0);
            if (!string.IsNullOrEmpty(Elec_acc.Trim()))
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(Elec_acc, Timesline), 640, Y_Pos, 0);
            else
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("____________________", Times), 640, Y_Pos, 0);

            X_Pos = 750; Y_Pos = 12;
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Page 1", TblFontItalic), X_Pos, Y_Pos, 0);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Date Printed: " + DateTime.Now.ToString("g"), TblFontItalic), 15, Y_Pos, 0);
            //"Date Printed: " + DateTime.Now.ToString()

            cb.EndText();

            document.NewPage();

            List<CommonEntity> IncomeTypes = new List<CommonEntity>();
            IncomeTypes.Add(new CommonEntity("1", "Employment Wages"));
            IncomeTypes.Add(new CommonEntity("2", "Public Asst.(TANF,SAGA,State Supp.,etc.)"));
            IncomeTypes.Add(new CommonEntity("3", "Child Support/Alimony"));
            IncomeTypes.Add(new CommonEntity("4", "Veterans Benefits"));
            IncomeTypes.Add(new CommonEntity("5", "Unemployment Compensation"));
            IncomeTypes.Add(new CommonEntity("6", "SSI"));
            IncomeTypes.Add(new CommonEntity("7", "Social Security Benefits"));
            IncomeTypes.Add(new CommonEntity("8", "Worker's Comp./Dis. Insurance"));
            IncomeTypes.Add(new CommonEntity("9", "Retirement/Pensions/Annuities"));
            IncomeTypes.Add(new CommonEntity("10", "Rental Income"));
            IncomeTypes.Add(new CommonEntity("11", "Self-Employment"));
            IncomeTypes.Add(new CommonEntity("12", "Cont. from Friends/Relatives"));
            IncomeTypes.Add(new CommonEntity("13", "Zero Income"));
            IncomeTypes.Add(new CommonEntity("14", "Other"));

            ////cb.BeginText();
            ////X_Pos = 400; Y_Pos = 580;
            ////cb.SetFontAndSize(bf_helv, 13);
            ////cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, Header_Desc, X_Pos, Y_Pos, 0);

            ////cb.SetFontAndSize(bf_helv, 9);
            ////cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Applicant No: ", 30, Y_Pos - 15, 0);
            ////ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationNo, Timesline), 30 + 72, Y_Pos - 15, 0);

            ////cb.SetFontAndSize(bf_helv, 13);
            ////cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, Form_Selection, X_Pos, Y_Pos - 15, 0);
            ////cb.SetFontAndSize(bf_helv, 9);
            ////cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Date Printed: ", 740, Y_Pos - 15, 0);
            ////ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(LookupDataAccess.Getdate(DateTime.Now.ToShortDateString()), Times), 780, Y_Pos - 15, 0);

            ////X_Pos = 30; Y_Pos -= 30;
            ////cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Applicant Name   ", X_Pos, Y_Pos, 0);
            ////ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_LEFT, new Phrase(BaseForm.BaseApplicationName, Timesline), X_Pos + 72, Y_Pos, 0);

            ////cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Application Date: ", 740, Y_Pos, 0);
            ////ColumnText.ShowTextAligned(cb, PdfContentByte.ALIGN_RIGHT, new Phrase(LookupDataAccess.Getdate(drCaseMST["MST_INTAKE_DATE"].ToString().Trim()), Timesline), 780, Y_Pos, 0);

            ////cb.EndText();

            //iTextSharp.text.Font HeaderFontBold = new iTextSharp.text.Font(bf_helv, 13);
            ////start the Income details of a Family to the table

            ////PdfPTable SpaceTable = new PdfPTable(1);
            ////SpaceTable.TotalWidth = 750f;
            ////SpaceTable.WidthPercentage = 100;
            ////SpaceTable.LockedWidth = true;
            ////float[] SpaceTablewidths = new float[] { 80f };
            ////SpaceTable.SetWidths(SpaceTablewidths);
            ////SpaceTable.HorizontalAlignment = Element.ALIGN_CENTER;
            ////SpaceTable.SpacingAfter = 70f;

            #region Income Table

            PdfPTable IncomeTable = new PdfPTable(3);
            IncomeTable.TotalWidth = 750f;
            IncomeTable.WidthPercentage = 100;
            IncomeTable.LockedWidth = true;
            float[] Incomewidths = new float[] { 80f, 100f, 120f };
            IncomeTable.SetWidths(Incomewidths);
            IncomeTable.HorizontalAlignment = Element.ALIGN_CENTER;
            IncomeTable.SpacingBefore = 100f;

            PdfPTable nestedTable = new PdfPTable(2);
            nestedTable.WidthPercentage = 100;
            //table.LockedWidth = true;
            float[] Nestedwidths = new float[] { 25f, 100f };
            nestedTable.SetWidths(Nestedwidths);
            nestedTable.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell nestedHeader = new PdfPCell(new Phrase("Financial Data", TblFontBold));
            nestedHeader.HorizontalAlignment = Element.ALIGN_LEFT;
            nestedHeader.Border = iTextSharp.text.Rectangle.BOX;
            nestedTable.AddCell(nestedHeader);

            PdfPCell nestedHeader1 = new PdfPCell(new Phrase("Note: Verification of Income (including benefits) is required", TblFontBold));
            nestedHeader1.HorizontalAlignment = Element.ALIGN_LEFT;
            nestedHeader1.Border = iTextSharp.text.Rectangle.BOX;
            nestedTable.AddCell(nestedHeader1);


            PdfPCell IncomeCell = new PdfPCell(nestedTable);
            IncomeCell.Colspan = 2;
            IncomeCell.HorizontalAlignment = Element.ALIGN_CENTER;
            IncomeCell.FixedHeight = 15f;
            IncomeCell.Border = iTextSharp.text.Rectangle.BOX;
            IncomeTable.AddCell(IncomeCell);

            PdfPCell IncomeCell1 = new PdfPCell(new Phrase("      APPLICANT'S NAME: " + BaseForm.BaseApplicationName.Trim(), Times));
            //IncomeCell1.Colspan = 2;
            IncomeCell1.HorizontalAlignment = Element.ALIGN_LEFT;
            IncomeCell1.FixedHeight = 15f;
            IncomeCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(IncomeCell1);

            PdfPCell IncomeCell2 = new PdfPCell(new Phrase("INCOME SOURCES", TblFontBold));
            //IncomeCell1.Colspan = 2;
            IncomeCell2.HorizontalAlignment = Element.ALIGN_LEFT;
            IncomeCell2.FixedHeight = 15f;
            IncomeCell2.Border = iTextSharp.text.Rectangle.BOX;
            IncomeTable.AddCell(IncomeCell2);

            PdfPCell IncomeCell3 = new PdfPCell(new Phrase("INCOME FREQUENCY(weekly, bi-weekly, monthly, etc.)", TblFontBold));
            //IncomeCell1.Colspan = 2;
            IncomeCell3.HorizontalAlignment = Element.ALIGN_LEFT;
            IncomeCell3.FixedHeight = 15f;
            IncomeCell3.Border = iTextSharp.text.Rectangle.BOX;
            IncomeTable.AddCell(IncomeCell3);

            PdfPCell IncomeCell4 = new PdfPCell(new Phrase("HOUSEHOLD MEMBER(S) RECEIVING INCOME", TblFontBold));
            //IncomeCell1.Colspan = 2;
            IncomeCell4.HorizontalAlignment = Element.ALIGN_LEFT;
            IncomeCell4.FixedHeight = 15f;
            IncomeCell4.Border = iTextSharp.text.Rectangle.BOX;
            IncomeTable.AddCell(IncomeCell4);

            if (IncomeTypes.Count > 0)
            {
                foreach (CommonEntity Entity in IncomeTypes)
                {
                    string IncType = string.Empty;
                    switch (Entity.Code.Trim())
                    {
                        case "1": IncType = "B"; break;
                        case "2": IncType = "I AA"; break;
                        case "3": IncType = "Z Y A"; break;
                        case "4": IncType = "O EE FF"; break;
                        case "5": IncType = "N"; break;
                        case "6": IncType = "M"; break;
                        case "7": IncType = "L"; break;
                        case "8": IncType = "P CC"; break;
                        case "9": IncType = "H DD"; break;
                        case "10": IncType = "J"; break;
                        case "11": IncType = "S"; break;
                        case "12": IncType = "T"; break;
                        case "13": IncType = "R"; break;
                        case "14": IncType = "W V U C D E F Q G K X BB"; break;

                    }

                    PdfPCell IncomeSource = new PdfPCell(new Phrase(Entity.Desc.Trim(), TableFont));
                    IncomeSource.HorizontalAlignment = Element.ALIGN_LEFT;
                    IncomeSource.FixedHeight = 15f;
                    IncomeSource.Border = iTextSharp.text.Rectangle.BOX;
                    IncomeTable.AddCell(IncomeSource);

                    string IncFreq = string.Empty; string MemName = string.Empty;
                    if (dtCaseIncome.Rows.Count > 0)
                    {
                        string[] strIncomeTypes = IncType.Split(' ');
                        foreach (DataRow dr in dtCaseIncome.Rows)
                        {
                            //if (IncType.Length > 1)
                            //{
                            if (!string.IsNullOrEmpty(dr["INCOME_TYPE"].ToString().Trim()))
                            {
                                foreach (string incomeType in strIncomeTypes)
                                {
                                    if (dr["INCOME_TYPE"].ToString().Trim() == incomeType)
                                    {
                                        CaseSnpEntity snpentity = BaseForm.BaseCaseSnpEntity.Find(u => u.FamilySeq == dr["INCOME_FAMILY_SEQ"].ToString().Trim() && u.Status == "A");
                                        if (snpentity != null)
                                        {
                                            IncFreq += GetIncomeIntervalDesc(dr["INCOME_INTERVAL"].ToString().Trim()) + ",";//LookupDataAccess.ShowIncomeInterval(dr["INCOME_INTERVAL"].ToString().Trim()) + ",";
                                            if (!string.IsNullOrEmpty(dr["INCOME_FAMILY_SEQ"].ToString().Trim()))
                                                MemName += Get_Member_Name(dr["INCOME_FAMILY_SEQ"].ToString().Trim(), "First") + ",";
                                            break;
                                        }
                                    }
                                }

                                //if (IncType.Trim().Contains(dr["INCOME_TYPE"].ToString()))
                                //{
                                //    IncFreq += LookupDataAccess.ShowIncomeInterval(dr["INCOME_INTERVAL"].ToString().Trim()) + ",";
                                //    if (!string.IsNullOrEmpty(dr["INCOME_FAMILY_SEQ"].ToString().Trim()))
                                //        MemName += Get_Member_Name(dr["INCOME_FAMILY_SEQ"].ToString().Trim(), "First") + ",";
                                //}
                            }
                            else if (string.IsNullOrEmpty(dr["INCOME_TYPE"].ToString().Trim()) && Entity.Code == "14")
                            {
                                CaseSnpEntity snpentity = BaseForm.BaseCaseSnpEntity.Find(u => u.FamilySeq == dr["INCOME_FAMILY_SEQ"].ToString().Trim() && u.Status == "A");
                                if (snpentity != null)
                                {
                                    IncFreq += GetIncomeIntervalDesc(dr["INCOME_INTERVAL"].ToString().Trim()) + ",";//LookupDataAccess.ShowIncomeInterval(dr["INCOME_INTERVAL"].ToString().Trim()) + ",";
                                    if (!string.IsNullOrEmpty(dr["INCOME_FAMILY_SEQ"].ToString().Trim()))
                                        MemName += Get_Member_Name(dr["INCOME_FAMILY_SEQ"].ToString().Trim(), "First") + ",";
                                }
                            }
                            //}
                            //else
                            //{
                            //    if (IncType.Trim()== dr["INCOME_TYPE"].ToString().Trim())
                            //    {
                            //        IncFreq += LookupDataAccess.ShowIncomeInterval(dr["INCOME_INTERVAL"].ToString().Trim()) + ",";
                            //        if (!string.IsNullOrEmpty(dr["INCOME_FAMILY_SEQ"].ToString().Trim()))
                            //            MemName += Get_Member_Name(dr["INCOME_FAMILY_SEQ"].ToString().Trim(), "First") + ",";
                            //    }
                            //}

                        }
                    }
                    if (!string.IsNullOrEmpty(IncFreq.Trim()))
                        IncFreq = IncFreq.Substring(0, IncFreq.Trim().Length - 1);
                    if (!string.IsNullOrEmpty(MemName.Trim()))
                        MemName = MemName.Substring(0, MemName.Trim().Length - 1);
                    if (IncFreq.ToString().Trim() == ",") IncFreq = "";

                    PdfPCell IncomeFreq = new PdfPCell(new Phrase(IncFreq, TableFont));
                    IncomeFreq.HorizontalAlignment = Element.ALIGN_LEFT;
                    IncomeFreq.FixedHeight = 15f;
                    IncomeFreq.Border = iTextSharp.text.Rectangle.BOX;
                    IncomeTable.AddCell(IncomeFreq);

                    PdfPCell IncomeMem = new PdfPCell(new Phrase(MemName, TableFont));
                    IncomeMem.HorizontalAlignment = Element.ALIGN_LEFT;
                    IncomeMem.FixedHeight = 15f;
                    IncomeMem.Border = iTextSharp.text.Rectangle.BOX;
                    IncomeTable.AddCell(IncomeMem);

                }
                PdfPCell Last = new PdfPCell(new Phrase("Application Certification", TblFontBold));
                Last.Colspan = 3;
                Last.HorizontalAlignment = Element.ALIGN_CENTER;
                Last.FixedHeight = 15f;
                Last.Border = iTextSharp.text.Rectangle.BOX;
                IncomeTable.AddCell(Last);
            }
            PdfPCell Space = new PdfPCell(new Phrase("", TblFontBold));
            Space.Colspan = 3;
            Space.HorizontalAlignment = Element.ALIGN_CENTER;
            Space.FixedHeight = 10f;
            Space.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Space);

            PdfPCell Para1 = new PdfPCell(new Phrase("I have read this form, or it has been read to me in a language that I understand. I understand what is in the form. As the applicant for my household, I swear that all statements made by me on this application are true, correct and complete to the best of my knowledge. I understand that only United Stated citizens or qualified aliens may be eligible to receive federal energy assistance benefits.", TableFont));
            Para1.Colspan = 3;
            Para1.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Para1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Para1);

            PdfPCell Space1 = new PdfPCell(new Phrase("", Times));
            Space1.Colspan = 3;
            Space1.HorizontalAlignment = Element.ALIGN_CENTER;
            Space1.FixedHeight = 10f;
            Space1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Space1);

            PdfPCell Para2 = new PdfPCell(new Phrase("I agree to provide to the Department of Social Services, or to its energy assistance contractor, the community action energy, any information including wages, asset information and bills in my name as the head of household or of a household member of majority status, which is necessary to determine my household's eligibility. I also agree that information included in this application may be provided to the State Department of Energy and Environmental Protection for the purpose of determining eligibility for weatherization services. I further understand that the community action agency or the State of Connecticut may verify or confirm any information required to determine my eligibility for this program. I agree that the information in this application may be provided to my energy vendors, and to any programs operated by the community action agency or the State of Connecticut for which I may be eligible. I also give consent for this information to be provided to any authorized government agency. I agree for my energy vendors to provide the community action agency or the State of Connecticut information about my energy account and/or usage. I agree to hold my energy vendors harmless and release them from and against loss, demands, damages, or liabilities caused by such disclosure. I also understand that information in this application may be used for evaluations and surveys by the community action agency, State of Connecticut, authorized government agency or its contractors.", TableFont));
            Para2.Colspan = 3;
            Para2.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Para2.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Para2);

            PdfPCell Space2 = new PdfPCell(new Phrase("", TblFontBold));
            Space2.Colspan = 3;
            Space2.HorizontalAlignment = Element.ALIGN_CENTER;
            Space2.FixedHeight = 10f;
            Space2.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Space2);

            PdfPCell Para3 = new PdfPCell(new Phrase("I understand that if I am granted assistance as a result of an intentional error, misrepresentation or fraud, I must repay, in full, the amount of the assistance provided, and I will not be eligible for assistance for the rest of the program year and for the following two (2) years. I also understand that if I have knowingly given any false or incorrect information, I may be subject to prosecution and penalties for false statements and larceny, as specified in sections 53a-122, 53a-123, and 53a-157b of the Connecticut General Statutes. These penalties may include imprisonment. I may also be subject to prosecution and penalties provided under federal law.", TableFont));
            Para3.Colspan = 3;
            Para3.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Para3.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Para3);

            PdfPCell Space3 = new PdfPCell(new Phrase("", TblFontBold));
            Space3.Colspan = 3;
            Space3.HorizontalAlignment = Element.ALIGN_CENTER;
            Space3.FixedHeight = 10f;
            Space3.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Space3);

            PdfPCell Para4 = new PdfPCell(new Phrase("I have received a copy of the Notice of Applicant Rights and Service Availability form.", TableFont));
            Para4.Colspan = 3;
            Para4.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Para4.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Para4);

            PdfPCell Space4 = new PdfPCell(new Phrase("", TblFontBold));
            Space4.Colspan = 3;
            Space4.HorizontalAlignment = Element.ALIGN_CENTER;
            Space4.FixedHeight = 10f;
            Space4.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Space4);

            PdfPCell decl = new PdfPCell(new Phrase("_________________________________________", TableFont));
            decl.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            decl.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(decl);

            PdfPCell decl1 = new PdfPCell(new Phrase("________________________________________________", TableFont));
            decl1.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            decl1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(decl1);

            PdfPCell decl2 = new PdfPCell(new Phrase("_______________", TableFont));
            decl2.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            decl2.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(decl2);

            PdfPCell decl3 = new PdfPCell(new Phrase("     Applicant's Signature", TableFont));
            decl3.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            decl3.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(decl3);

            PdfPCell decl4 = new PdfPCell(new Phrase("   Witness/Interpreter/Legal Representative", TableFont));
            decl4.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            decl4.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(decl4);

            PdfPCell decl5 = new PdfPCell(new Phrase("   Date", TableFont));
            decl5.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            decl5.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(decl5);

            PdfPCell Space5 = new PdfPCell(new Phrase("", TblFontBold));
            Space5.Colspan = 3;
            Space5.HorizontalAlignment = Element.ALIGN_CENTER;
            Space5.FixedHeight = 10f;
            Space5.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Space5);

            PdfPCell Intake1 = new PdfPCell(new Phrase("_________________________________________", TableFont));
            Intake1.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Intake1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Intake1);

            PdfPCell Intake2 = new PdfPCell(new Phrase("________________________________________________", TableFont));
            Intake2.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Intake2.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Intake2);

            PdfPCell Intake3 = new PdfPCell(new Phrase("_______________", TableFont));
            Intake3.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Intake3.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Intake3);

            PdfPCell Intake4 = new PdfPCell(new Phrase("     Intake Worker's Signature", TableFont));
            Intake4.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Intake4.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Intake4);

            PdfPCell Intake5 = new PdfPCell(new Phrase("   Intake Site", TableFont));
            Intake5.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Intake5.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Intake5);

            PdfPCell Intake6 = new PdfPCell(new Phrase("   Date", TableFont));
            Intake6.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Intake6.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Intake6);

            PdfPCell Space6 = new PdfPCell(new Phrase("", TblFontBold));
            Space6.Colspan = 3;
            Space6.HorizontalAlignment = Element.ALIGN_CENTER;
            Space6.FixedHeight = 10f;
            Space6.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Space6);

            //PdfPTable nestedTable1 = new PdfPTable(3);
            //nestedTable1.WidthPercentage = 100;
            ////table.LockedWidth = true;
            //float[] Nestedwidths1 = new float[] { 30f, 13f,  };
            //nestedTable1.SetWidths(Nestedwidths1);
            //nestedTable1.HorizontalAlignment = Element.ALIGN_LEFT;

            //PdfPCell BG2 = new PdfPCell(new Phrase(""I swear or affirm that the certifications given are true, correct and accurate", TableFont));
            //BG2.HorizontalAlignment = Element.ALIGN_LEFT;
            //BG2.Border = iTextSharp.text.Rectangle.NO_BORDER;
            //nestedTable.AddCell(BG2);

            //PdfPCell BG3 = new PdfPCell(new Phrase(Entity.BDA_OLD_BUDGET.Trim(), TableFont));
            //BG3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //BG3.Border = iTextSharp.text.Rectangle.NO_BORDER;
            //nestedTable.AddCell(BG3);

            //PdfPCell BG4 = new PdfPCell(new Phrase(" After:", TableFont));
            //BG4.HorizontalAlignment = Element.ALIGN_LEFT;
            //BG4.Border = iTextSharp.text.Rectangle.NO_BORDER;
            //nestedTable.AddCell(BG4);

            //PdfPCell BG5 = new PdfPCell(new Phrase(Entity.BDA_NEW_BUDGET.Trim(), TableFont));
            //BG5.HorizontalAlignment = Element.ALIGN_RIGHT;
            //BG5.Border = iTextSharp.text.Rectangle.NO_BORDER;
            //nestedTable.AddCell(BG5);

            //PdfPCell NestedLoop = new PdfPCell(nestedTable);
            //NestedLoop.Colspan = 3;
            //NestedLoop.Padding = 0f;
            //NestedLoop.Border = iTextSharp.text.Rectangle.NO_BORDER;//iTextSharp.text.Rectangle.LEFT_BORDER + iTextSharp.text.Rectangle.RIGHT_BORDER;
            //IncomeTable.AddCell(NestedLoop);

            var phrase = new Phrase();
            phrase.Add(new Chunk("I swear or affirm that the certifications given are true, correct and accurate ", TableFont));
            phrase.Add(new Chunk("as stated and/or supplied by the applicant", TblFontBoldS));
            phrase.Add(new Chunk(" and understand that the provision of false, fraudulent or misleading information is punishable by state law.", TableFont));

            PdfPCell ss = new PdfPCell(phrase);
            ss.HorizontalAlignment = Element.ALIGN_LEFT;
            ss.Colspan = 3;
            ss.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(ss);

            //PdfPCell ss = new PdfPCell(new Phrase("I swear or affirm that the certifications given are true, correct and accurate as stated and/or supplied by the applicant and understand that the provision of false, fraudulent or misleading information is punishable by state law.", TableFont));
            //ss.HorizontalAlignment = Element.ALIGN_LEFT;
            //ss.Colspan = 3;
            //ss.Border = iTextSharp.text.Rectangle.NO_BORDER;
            //IncomeTable.AddCell(ss);

            PdfPCell Space7 = new PdfPCell(new Phrase("", TblFontBold));
            Space7.Colspan = 3;
            Space7.HorizontalAlignment = Element.ALIGN_CENTER;
            Space7.FixedHeight = 8f;
            Space7.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Space7);

            PdfPCell Cert1 = new PdfPCell(new Phrase("_________________________________________", TableFont));
            Cert1.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Cert1.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Cert1);

            PdfPCell Cert2 = new PdfPCell(new Phrase("_______________", TableFont));
            Cert2.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Cert2.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Cert2);

            PdfPCell Space8 = new PdfPCell(new Phrase("", TblFontBold));
            //Space7.Colspan = 3;
            Space8.HorizontalAlignment = Element.ALIGN_CENTER;
            //Space8.FixedHeight = 10f;
            Space8.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Space8);

            PdfPCell Cert3 = new PdfPCell(new Phrase("     Certifier's Signature", TableFont));
            Cert3.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Cert3.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Cert3);

            PdfPCell Cert4 = new PdfPCell(new Phrase("   Date", TableFont));
            Cert4.HorizontalAlignment = Element.ALIGN_LEFT;
            //Para1.FixedHeight = 15f;
            Cert4.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Cert4);

            PdfPCell Space9 = new PdfPCell(new Phrase("", TblFontBold));
            //Space7.Colspan = 3;
            Space9.HorizontalAlignment = Element.ALIGN_CENTER;
            //Space9.FixedHeight = 10f;
            Space9.Border = iTextSharp.text.Rectangle.NO_BORDER;
            IncomeTable.AddCell(Space9);



            document.Add(IncomeTable);

            #endregion

            cb.BeginText();
            X_Pos = 750; Y_Pos = 12;
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Page 2", TblFontItalic), X_Pos, Y_Pos, 0);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase("Date Printed: " + DateTime.Now.ToString("g"), TblFontItalic), 15, Y_Pos, 0);
            cb.EndText();

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


        public List<CommonEntity> IncomeInterValList { get; set; }
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

        #endregion
    }
}