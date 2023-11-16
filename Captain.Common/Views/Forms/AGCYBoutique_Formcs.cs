#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using Wisej.Web;
using Captain.Common.Views.Controls.Compatibility;
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

#endregion

namespace Captain.Common.Views.Forms
{
    public partial class AGCYBoutique_Formcs : Form
    {
        #region private variables

        private ErrorProvider _errorProvider = null;
        private CaptainModel _model = null;
        private bool boolChangeStatus = false;

        public int strIndex = 0;
        public int strCrIndex = 0;
        public int strPageIndex = 1;

        #endregion

        public AGCYBoutique_Formcs(BaseForm baseform, string mode, string code, PrivilegeEntity priviliges,AGCYBOTIQEntity strBEntity)
        {
            InitializeComponent();
            _errorProvider = new ErrorProvider(this);
            _errorProvider.BlinkRate = 3;
            _errorProvider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            _errorProvider.Icon = null;
            _model = new CaptainModel();

            BaseForm = baseform;
            Mode = mode;
            Code = code;
            Privileges = priviliges;

            AgyBoutiqData = strBEntity;

            this.Text = "Agency Boutique Information Form";

            FillAllCombos();

            if(Mode=="Edit")
                FillControls();

        }

        #region properties

        public BaseForm BaseForm { get; set; }

        public string Mode { get; set; }

        public string Code { get; set; }

        //public string CaMs_Code { get; set; }

        //public string button_Mode { get; set; }

        public PrivilegeEntity Privileges { get; set; }

        public bool IsSaveValid { get; set; }

        public List<CommonEntity> ListcommonEntity { get; set; }

        public List<CommonEntity> ListcommonCityEntity { get; set; }

        public AGCYREPEntity AgcyRepData { get; set; }

        public AGCYSUBEntity AgcySubData { get; set; }

        public List<AGCYREPEntity> AgyRepList { get; set; }

        public AGCYBOTIQEntity AgyBoutiqData { get; set; }

        
        public List<AGCYSUBEntity> AGYSubList { get; set; }

        public List<AGCYBOTIQEntity> BoutiqList { get; set; }


        #endregion

        private void FillAllCombos()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add(new Captain.Common.Utilities.ListItem("Select One", "0"));
            cmbStatus.Items.Add(new Captain.Common.Utilities.ListItem("In progress", "I"));
            cmbStatus.Items.Add(new Captain.Common.Utilities.ListItem("Approved", "A"));
            cmbStatus.Items.Add(new Captain.Common.Utilities.ListItem("Denied", "D"));
            cmbStatus.SelectedIndex = 0;

            cmbShared.Items.Clear();
            List<CommonEntity> commonEntity = CommonFunctions.AgyTabsFilterCode(BaseForm.BaseAgyTabsEntity, "S0001", BaseForm.BaseAgency, BaseForm.BaseDept, BaseForm.BaseProg, "Add"); //_model.lookupDataAccess.GetJobTitle();           
            cmbShared.Items.Insert(0, new ListItem("Select One", "0"));
            foreach (CommonEntity typeentity in commonEntity)
            {
                ListItem li = new ListItem(typeentity.Desc, typeentity.Code);
                cmbShared.Items.Add(li);
            }
            cmbShared.SelectedIndex = 0;

            cmbCounty.Items.Clear();
            List<CommonEntity> County = _model.ZipCodeAndAgency.GetCounty();
            foreach (CommonEntity county in County)
            {
                cmbCounty.Items.Add(new ListItem(county.Desc, county.Code));
            }
            cmbCounty.Items.Insert(0, new ListItem("Select One", "0"));
            cmbCounty.SelectedIndex = 0;

            cmbCity.Items.Clear();
            List<CommonEntity> City = _model.ZipCodeAndAgency.GetCity(string.Empty,string.Empty,string.Empty,"CITY");
            foreach (CommonEntity city in City)
            {
                cmbCity.Items.Add(new ListItem(city.Desc, city.Code));
            }
            cmbCity.Items.Insert(0, new ListItem("Select One", "0"));
            cmbCity.SelectedIndex = 0;


            cmbPercentage.Items.Clear(); cmbLunch.Items.Clear();
            List<ListItem> listItem = new List<ListItem>();
            for(int i=1;i<=100;i++)
            {
                listItem.Add(new ListItem(i.ToString(), i.ToString()));
            }

            cmbPercentage.Items.AddRange(listItem.ToArray());
            cmbLunch.Items.AddRange(listItem.ToArray());

            cmbLunch.SelectedIndex = 0;
            cmbPercentage.SelectedIndex = 0;

        }

        private void FillControls()
        {
            AGCYBOTIQEntity searchEntity = new AGCYBOTIQEntity(true);
            searchEntity.Code = Code.Trim();
            BoutiqList = _model.SPAdminData.Browse_AGCYBoutiq(searchEntity, "Browse");

            if(BoutiqList.Count>0)
            {
                if (!string.IsNullOrEmpty(BoutiqList[0].Req_Date.Trim()))
                {
                    dtpRequest.Text = BoutiqList[0].Req_Date.Trim();
                    dtpRequest.Checked = true;
                }

                if (!string.IsNullOrEmpty(BoutiqList[0].Req_Comp_Date.Trim()))
                {
                    dtpComplete.Text = BoutiqList[0].Req_Comp_Date.Trim();
                    dtpComplete.Checked = true;
                }

                txtNotes.Text = BoutiqList[0].Footage.Trim();
                txtShared.Text = BoutiqList[0].Shared_desc.Trim();
                txtItemsNeeded.Text = BoutiqList[0].ItemsNeeded.Trim();
                txtPopServed.Text = BoutiqList[0].Population.Trim();
                txtPoverty.Text = BoutiqList[0].Poverty.Trim();
                txtInventory.Text = BoutiqList[0].InvoiceForm.Trim();

                txtCounty.Text = BoutiqList[0].County.Trim();
                if (!string.IsNullOrEmpty(BoutiqList[0].County.Trim()))
                {
                    ListcommonEntity = new List<CommonEntity>();
                    string[] CountyList = BoutiqList[0].County.Split(',');
                    if (CountyList.Length > 0)
                    {
                        foreach (string Cont in CountyList)
                        {
                            ListcommonEntity.Add(new CommonEntity(Cont, string.Empty));
                        }
                    }
                }

                txtCity.Text = BoutiqList[0].City;
                if (!string.IsNullOrEmpty(BoutiqList[0].City.Trim()))
                {
                    ListcommonCityEntity = new List<CommonEntity>();
                    string[] CountyList = BoutiqList[0].City.Split(',');
                    if (CountyList.Length > 0)
                    {
                        foreach (string Cont in CountyList)
                        {
                            ListcommonCityEntity.Add(new CommonEntity(Cont, string.Empty));
                        }
                    }
                }

                CommonFunctions.SetComboBoxValue(cmbStatus, BoutiqList[0].Status.Trim());
                CommonFunctions.SetComboBoxValue(cmbShared, BoutiqList[0].Shared.Trim());
                CommonFunctions.SetComboBoxValue(cmbLunch, BoutiqList[0].Free_Lunch.Trim());
                //CommonFunctions.SetComboBoxValue(cmbCity, BoutiqList[0].City.Trim());
                //CommonFunctions.SetComboBoxValue(cmbCounty, BoutiqList[0].County.Trim());
                CommonFunctions.SetComboBoxValue(cmbPercentage, BoutiqList[0].Percentage.Trim());
            }
            else if(AgyBoutiqData!=null)
            {
                if (!string.IsNullOrEmpty(AgyBoutiqData.Req_Date.Trim()))
                    dtpRequest.Text = AgyBoutiqData.Req_Date.Trim();

                if (!string.IsNullOrEmpty(AgyBoutiqData.Req_Comp_Date.Trim()))
                    dtpComplete.Text = AgyBoutiqData.Req_Comp_Date.Trim();

                txtNotes.Text = AgyBoutiqData.Footage.Trim();
                txtShared.Text = AgyBoutiqData.Shared_desc.Trim();
                txtItemsNeeded.Text = AgyBoutiqData.ItemsNeeded.Trim();
                txtPopServed.Text = AgyBoutiqData.Population.Trim();
                txtPoverty.Text = AgyBoutiqData.Poverty.Trim();
                txtInventory.Text = AgyBoutiqData.InvoiceForm.Trim();

                CommonFunctions.SetComboBoxValue(cmbStatus, AgyBoutiqData.Status.Trim());
                CommonFunctions.SetComboBoxValue(cmbShared, AgyBoutiqData.Shared.Trim());
                CommonFunctions.SetComboBoxValue(cmbLunch, AgyBoutiqData.Free_Lunch.Trim());
                CommonFunctions.SetComboBoxValue(cmbCity, AgyBoutiqData.City.Trim());
                CommonFunctions.SetComboBoxValue(cmbCounty, AgyBoutiqData.County.Trim());
                CommonFunctions.SetComboBoxValue(cmbPercentage, AgyBoutiqData.Percentage.Trim());
            }

        }


        public AGCYBOTIQEntity GetRepData()
        {
            AGCYBOTIQEntity Entity = new AGCYBOTIQEntity();

            if (Mode == "Add" || Code=="New")
            {
                Entity.Code = Code;

                Entity.Req_Date = LookupDataAccess.Getdate(dtpRequest.Text);
                if (dtpComplete.Checked)
                    Entity.Req_Comp_Date = LookupDataAccess.Getdate(dtpComplete.Text);
                if (((ListItem)cmbStatus.SelectedItem).Value.ToString() != "0")
                    Entity.Status = ((ListItem)cmbStatus.SelectedItem).Value.ToString();
                Entity.Footage = txtNotes.Text;
                if (((ListItem)cmbShared.SelectedItem).Value.ToString() != "0")
                    Entity.Shared = ((ListItem)cmbShared.SelectedItem).Value.ToString();

                Entity.Shared_desc = txtShared.Text;
                Entity.ItemsNeeded = txtItemsNeeded.Text;
                Entity.Population = txtPopServed.Text;

                if (((ListItem)cmbLunch.SelectedItem).Value.ToString() != "0")
                    Entity.Free_Lunch = ((ListItem)cmbLunch.SelectedItem).Value.ToString();

                Entity.Poverty = txtPoverty.Text;//.Substring(0,txtCounty.Text.Length-1);
                if (((ListItem)cmbCity.SelectedItem).Value.ToString() != "0")
                    Entity.City = ((ListItem)cmbCity.SelectedItem).Value.ToString();

                if (((ListItem)cmbCounty.SelectedItem).Value.ToString() != "0")
                    Entity.County = ((ListItem)cmbCounty.SelectedItem).Value.ToString();

                if (((ListItem)cmbPercentage.SelectedItem).Value.ToString() != "0")
                    Entity.Percentage = ((ListItem)cmbPercentage.SelectedItem).Value.ToString();
                Entity.InvoiceForm = txtInventory.Text.Trim();

                Entity.Add_Operator = BaseForm.UserID;
                Entity.Lsct_Operator = BaseForm.UserID;
                Entity.Rec_Type = "I";
                
            }

            return Entity;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(ValidateForm())
            {
                if(Mode=="Edit" && Code != "New")
                {
                    string strmsgGrp = string.Empty; string SqlMsg = string.Empty;
                    AGCYBOTIQEntity RefEntity = new AGCYBOTIQEntity();
                    //RefEntity.Code=
                    CaptainModel model = new CaptainModel();

                    if (Mode == "Edit" && BoutiqList.Count > 0)
                        RefEntity.Rec_Type = "U";
                    else if (BoutiqList.Count == 0) RefEntity.Rec_Type = "I";
                    RefEntity.Code = Code;
                    
                    RefEntity.Req_Date = LookupDataAccess.Getdate(dtpRequest.Text);
                    if (dtpComplete.Checked)
                        RefEntity.Req_Comp_Date= LookupDataAccess.Getdate(dtpComplete.Text);
                    if (((ListItem)cmbStatus.SelectedItem).Value.ToString() != "0")
                        RefEntity.Status = ((ListItem)cmbStatus.SelectedItem).Value.ToString();
                    RefEntity.Footage = txtNotes.Text;
                    if (((ListItem)cmbShared.SelectedItem).Value.ToString() != "0")
                        RefEntity.Shared = ((ListItem)cmbShared.SelectedItem).Value.ToString();

                    RefEntity.Shared_desc= txtShared.Text;
                    RefEntity.ItemsNeeded = txtItemsNeeded.Text;
                    RefEntity.Population= txtPopServed.Text;

                    if (((ListItem)cmbLunch.SelectedItem).Value.ToString() != "0")
                        RefEntity.Free_Lunch = ((ListItem)cmbLunch.SelectedItem).Value.ToString();

                    RefEntity.Poverty = txtPoverty.Text;//.Substring(0,txtCounty.Text.Length-1);
                    //if (((ListItem)cmbCity.SelectedItem).Value.ToString() != "0")
                    //    RefEntity.City = ((ListItem)cmbCity.SelectedItem).Value.ToString();

                    RefEntity.City = txtCity.Text.Trim();

                    //if (((ListItem)cmbCounty.SelectedItem).Value.ToString() != "0")
                    //    RefEntity.County = ((ListItem)cmbCounty.SelectedItem).Value.ToString();
                    RefEntity.County = txtCounty.Text.Trim();

                    if (((ListItem)cmbPercentage.SelectedItem).Value.ToString() != "0")
                        RefEntity.Percentage = ((ListItem)cmbPercentage.SelectedItem).Value.ToString();
                    RefEntity.InvoiceForm = txtInventory.Text.Trim();
                    
                    RefEntity.Add_Operator = BaseForm.UserID;
                    RefEntity.Lsct_Operator = BaseForm.UserID;

                    _model.SPAdminData.UpdateAgencyBoutique(RefEntity, "UPDATE", out strmsgGrp, out SqlMsg);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (!dtpRequest.Checked)
            {
                _errorProvider.SetError(dtpRequest, string.Format(Consts.Messages.BlankIsRequired.GetMessage(), lblDateRequest.Text.Replace(Consts.Common.Colon, string.Empty)));
                isValid = false;
            }

            IsSaveValid = isValid;
            return (isValid);
        }

        private void cmbShared_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ListItem)cmbShared.SelectedItem).Value.ToString() == "Y")
                txtShared.Enabled = true;
            else { txtShared.Enabled = false; txtShared.Text = string.Empty; }
        }

        private void Pb_County_Click(object sender, EventArgs e)
        {
            SelectZipSiteCountyForm countyform = new SelectZipSiteCountyForm(BaseForm, ListcommonEntity);
            countyform.StartPosition = FormStartPosition.CenterScreen;
            countyform.FormClosed += new FormClosedEventHandler(SelectCountyFormClosed);
            countyform.ShowDialog();
        }

        private void SelectCountyFormClosed(object sender, FormClosedEventArgs e)
        {
            SelectZipSiteCountyForm form = sender as SelectZipSiteCountyForm;
            if (form.DialogResult == DialogResult.OK)
            {
                if (form.FormType == "COUNTY")
                {
                    ListcommonEntity = form.SelectedCountyEntity;
                    if (ListcommonEntity.Count > 0)
                    {
                        string County = string.Empty;
                        foreach (CommonEntity Entity in ListcommonEntity)
                        {
                            County += Entity.Code.Trim() + ",";
                        }

                        txtCounty.Text = County.Substring(0, County.Length - 1);
                    }
                }


            }
        }

        private void pb_City_Click(object sender, EventArgs e)
        {
            SelectZipSiteCountyForm countyform = new SelectZipSiteCountyForm(BaseForm, ListcommonCityEntity, "City",string.Empty,string.Empty);
            countyform.StartPosition = FormStartPosition.CenterScreen;
            countyform.FormClosed += new FormClosedEventHandler(SelectCityFormClosed);
            countyform.ShowDialog();
        }

        private void SelectCityFormClosed(object sender, FormClosedEventArgs e)
        {
            SelectZipSiteCountyForm form = sender as SelectZipSiteCountyForm;
            if (form.DialogResult == DialogResult.OK)
            {
                if (form.FormType == "City")
                {
                    ListcommonCityEntity = form.SelectedCityEntity;
                    if (ListcommonCityEntity.Count > 0)
                    {
                        string County = string.Empty;
                        foreach (CommonEntity Entity in ListcommonCityEntity)
                        {
                            County += Entity.Code.Trim() + ",";
                        }

                        txtCity.Text = County.Substring(0, County.Length - 1);
                    }
                }
            }
        }

        private void AGCYBoutique_Formcs_ToolClick(object sender, ToolClickEventArgs e)
        {
            Application.Navigate(CommonFunctions.BuildHelpURLS(Privileges.Program, 0, BaseForm.BusinessModuleID.ToString()), target: "_blank");
        }
    }
}