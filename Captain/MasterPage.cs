/****************************************************************************************
 * Class Name   : MainForm
 * Author       : Kranthi
 * Created Date : 06/03/2022
 * Version      : 
 * Description  : The applications main presentation layer.
 ****************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using Wisej.Web;
using Captain.Common.Controllers;
using Captain.Common.Exceptions;
using Captain.Common.Handlers;
using Captain.Common.Menus;
using Captain.Common.Model.Data;
using Captain.Common.Model.Objects;
using Captain.Common.Model.Parameters;
using Captain.Common.Model.CaptainFaults;
using Captain.Common.Resources;
using Captain.Common.Utilities;
using Captain.Common.Views.Controls;
using Captain.Common.Views.Forms;
using Captain.Common.Views.Forms.Base;
using Captain.Common.Views.UserControls;
using Captain.Common.Views.UserControls.Base;
using System.ComponentModel;
using System.Collections.Specialized;
using Captain.Common.Views.Controls.Compatibility;
using Wisej.Web.Ext.NavigationBar;

#endregion

namespace Captain
{
    public partial class MasterPage : BaseForm
    {
        #region Variables

        // Private variables
        private Panel _fullScreenPanel = null;
        private XmlDocument _xmlDocument = null;
        private TreeNode _selectedTreeNode;
        private string _simpleFooterDisplay = string.Empty;
        private bool _isPDFEditorInitialized = false;
        private string _uploadedFileType = string.Empty;
        private PrivilegeEntity _privilegeEntity = null;
        private string _lastGuid = string.Empty;
        private CaptainModel _model = null;
        public HierachyNameControl hierachyNamecontrol = new HierachyNameControl();
        public ApplicationNameControl applicationNameControl;
        public AgencyNameControl agencyNameControl;
        //public ApplicationDetailsControl applicationDetailsControl ;
        public List<TagClass> RequestedDownloadTagClassList = new List<TagClass>();
        private bool _defaultHierchyform = false;
        public List<PrivilegeEntity> AdminSRCNPrivilege = null;


        public string _statAdminAgency = "";
        #endregion

        public MasterPage()
        {
            InitializeComponent();
            try
            {
                //AddWelcomeScreen(pnlTabs);

                DataBaseConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CMMSo"].ConnectionString.ToString();
                _model = new CaptainModel();

                MenuManager = new MenuManager(this);
                applicationNameControl = new ApplicationNameControl(this);
                agencyNameControl = new AgencyNameControl(this);
                BaseAgency = UserProfile.Agency;
                BaseDept = UserProfile.Dept;
                BaseProg = UserProfile.Prog;

                /***************************************/
                BaseDefaultHIE = UserProfile.Agency + "-" + UserProfile.Dept + "-" + UserProfile.Prog;
                /*************************************/

                BaseAdminAgency = BaseAgency;
                _statAdminAgency = BaseAgency;
                BaseAccessAll = UserProfile.AccessAll;

                if (BaseAccessAll == "Y")
                {
                    pSwitchAccount.Visible = true;
                    pnlSwitchAccount.Visible = true;
                }
                else
                {
                    pSwitchAccount.Visible = false;
                    pnlSwitchAccount.Visible = false;
                }

                List<ChldTrckEntity> chldtrackList = new List<ChldTrckEntity>();
                BaseTaskEntity = chldtrackList;
                List<HlsTrckEntity> hlstrackList = new List<HlsTrckEntity>();
                BaseHlsTaskEntity = hlstrackList;
                BaseFilterYear = "3";
                BaseYear = "    ";
                BaseComponent = string.Empty;


                ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseAgency, BaseDept, BaseProg);
                if (programEntity != null)
                {
                    BaseYear = programEntity.DepYear.Trim() == string.Empty ? "    " : programEntity.DepYear;
                }
                BaseAgencyControlDetails = _model.ZipCodeAndAgency.GetAgencyControlFile("00");
                BaseAgyTabsEntity = _model.lookupDataAccess.GetAgyTabs(string.Empty, string.Empty, string.Empty);
                BaseAgencyuserHierarchys = _model.UserProfileAccess.GetUserHierarchyByID(UserID);
                BaseCaseHierachyListEntity = _model.HierarchyAndPrograms.GetCaseHierachyAllData("**", "**", "**");
                if (BaseAgencyuserHierarchys.Count > 0)
                {
                    if (BaseAgencyControlDetails.SiteSecurity == "1")
                    {
                        List<HierarchyEntity> userHierarchy = BaseAgencyuserHierarchys.FindAll(u => u.Sites != string.Empty);
                        if (userHierarchy.Count > 0)
                        {
                            BaseAgencyControlDetails.SitesData = "1";
                        }

                    }
                }
                Gbl_Rent_Rec = string.Empty;
                BasePIPDragSwitch = "N";
                DataBaseConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CMMSo"].ConnectionString.ToString();
                if (System.Configuration.ConfigurationManager.ConnectionStrings["LEANINTAKEConnection"] != null)
                    BaseLeanDataBaseConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LEANINTAKEConnection"].ConnectionString.ToString();


                if (System.Configuration.ConfigurationManager.ConnectionStrings["BaseDSSXMLDBConnection"] != null)
                    BaseDSSXMLDBConnString = System.Configuration.ConfigurationManager.ConnectionStrings["BaseDSSXMLDBConnection"].ConnectionString.ToString();


                BaseClientFolloupFromDate = DateTime.Now.AddDays(-7).ToShortDateString();
                BaseClientFolloupToDate = DateTime.Now.ToShortDateString();
                BaseClientFolloupWorker = string.Empty;
                BaseClientFolloupScreentype = "A";
                BaseClientFolloupScreencode = string.Empty;

                List<PrivilegeEntity> NavigationPrivileges = _model.UserProfileAccess.GetApplicationsByUserID(UserID, string.Empty);


                //if (BaseAgency == "02")
                //{
                Application.SetSessionTimeout(1800);
                //}

            }
            catch (Exception ex)
            {

            }
        }
        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            this.Size = Application.Browser.Size;
        }
        private void MasterPage_Load(object sender, EventArgs e)
        {

            // Application.BrowserSizeChanged += Application_BrowserSizeChanged;

            UserEntity userInfo = Captain<UserEntity>.Session[Consts.SessionVariables.UserProfile];
            string fullName = Captain<string>.Session[Consts.SessionVariables.UserID];
            sbpWelcome.Text = string.Concat(Consts.Controls.Welcome.GetControlName(), Consts.Common.Comma, Consts.Common.Space, fullName);
            sbpLoginStatus.Text = string.Concat("Previous Login was " + Captain<string>.Session[Consts.SessionVariables.LostLogin_Status], Consts.Common.Comma, " On  ", Captain<string>.Session[Consts.SessionVariables.LostLogin_Date]);

            BuildMenuTree();



            if (userInfo.Successful.Equals("0"))
            {
                ChangePassword changePassword = new ChangePassword(this, string.Empty);
                changePassword.StartPosition = FormStartPosition.CenterScreen;
                changePassword.ShowDialog();
            }
            else
            {
                if (userInfo.PWDChangeDate != string.Empty)
                {
                    AgencyControlEntity agencycontroldata = _model.ZipCodeAndAgency.GetAgencyControlFile("00");
                    if (agencycontroldata != null)
                    {
                        if (agencycontroldata.ForcePwd.ToUpper() == "Y")
                        {
                            DateTime dt = Convert.ToDateTime(userInfo.PWDChangeDate).Date;
                            DateTime dtnew = DateTime.Now.Date;
                            int intdif = (dtnew - dt).Days;
                            if (intdif > Convert.ToInt32(agencycontroldata.ForcePwdDays))
                            {
                                ChangePassword changePassword = new ChangePassword(this, string.Empty);
                                changePassword.StartPosition = FormStartPosition.CenterScreen;
                                changePassword.ShowDialog();
                            }
                        }
                    }
                }

            }

        }

        public override void BuildMenuTree()
        {

            NavTabs.Items.Clear();
            HIE = BaseAgency + BaseDept + BaseProg;
            strcurrentHIE = BaseAgency + "-" + BaseDept + "-" + BaseProg;
            CaptainModel Capmodel = new CaptainModel();
            List<PrivilegeEntity> NavigationPrivileges = _model.UserProfileAccess.GetApplicationsByUserID(UserID, HIE);

            if (NavigationPrivileges.Count > 0)
            {
                // BusinessModuleID = NavigationPrivileges[0].ModuleCode;
                BusinessModuleID = NavigationPrivileges.Min(x => x.ModuleCode);
            }

            foreach (PrivilegeEntity iModuleItem in NavigationPrivileges)
            {
                string code = iModuleItem.ModuleCode;

                /******************************** MODULE Menu Head Creation  *********************************************/
                NavigationBarItem nBarModule = new NavigationBarItem();
                nBarModule.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
                nBarModule.CssStyle = "border-bottom:1px solid #F4F4F4;";
                nBarModule.ForeColor = System.Drawing.Color.Black;
                nBarModule.Tag = "Head";
                nBarModule.Icon = "Resources\\Images\\MenuIcons\\" + iModuleItem.ModuleCode + ".png";
                nBarModule.InfoTextBackColor = System.Drawing.Color.FromName("@window");
                nBarModule.Name = "nModule_" + iModuleItem.ModuleCode;
                nBarModule.Text = iModuleItem.ModuleName;
                //nBarModule.Controls[0].Controls["open"].BackColor = System.Drawing.Color.Red;
                //nBarModule.Controls[0].Controls["open"].ForeColor = System.Drawing.Color.Red;
                //nBarModule.Controls[0].Controls["open"].Hide();
                //nBarModule.Controls[0].Controls["open"].Visible = false;

                //nBarModule.Height = 35;

                /******************************** SCREENS Menu Head Creation  *********************************************/

                NavigationBarItem nBarModScreenHead = new NavigationBarItem();
                nBarModScreenHead.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                nBarModScreenHead.AllowHtml = true;
                nBarModScreenHead.CssStyle = "border-bottom:1px solid #F4F4F4; ";
                nBarModScreenHead.ForeColor = System.Drawing.Color.Black;
                nBarModScreenHead.Name = "nModule_" + iModuleItem.ModuleCode + "_SCRHead";
                nBarModScreenHead.Text = "Screens";
                nBarModScreenHead.ToolTipText = "Screens";
                nBarModScreenHead.Icon = "Resources/Images/MenuIcons/Screens.png";
                nBarModScreenHead.Controls[0].Controls[0].CssStyle = "margin-left:8px; font-weight:bold;";
                nBarModScreenHead.Controls[0].Controls[1].CssStyle = "margin-left:0px; font-weight:bold;";
                nBarModScreenHead.Expanded = true;
                nBarModule.Items.Add(nBarModScreenHead);    // Add Screen Menu Header to Module

                /******************************** REPORT Menu Head Creation  *********************************************/
                NavigationBarItem nBarModReportHead = new NavigationBarItem();
                nBarModReportHead.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                nBarModReportHead.AllowHtml = true;
                nBarModReportHead.CssStyle = "border-bottom:1px solid #F4F4F4;";
                nBarModReportHead.ForeColor = System.Drawing.Color.Black;
                nBarModReportHead.Name = "nModule_" + iModuleItem.ModuleCode + "_RPTHead";
                nBarModReportHead.Text = "Reports";
                nBarModReportHead.ToolTipText = "Reports";
                nBarModReportHead.Icon = "Resources/Images/MenuIcons/Reports.png";
                nBarModReportHead.Controls[0].Controls[0].CssStyle = "margin-left:8px;";
                nBarModReportHead.Controls[0].Controls[1].CssStyle = "margin-left:0px;";
                nBarModule.Items.Add(nBarModReportHead);        // Add Report Menu Header to Module


                if (code == "99")
                {
                    nBarModScreenHead.Visible = false;
                }


                List<PrivilegeEntity> userPrivilege = null;
                if (code == "01")
                {
                    pnlCenterbar.Visible = true;
                    pnlSearchbox.Visible = false;
                    pHIEFilter.Visible = true;


                    userPrivilege = _model.UserProfileAccess.GetScreensByUserID(code, UserID, string.Empty);
                    AdminSRCNPrivilege = userPrivilege;
                }
                else
                {
                    userPrivilege = _model.UserProfileAccess.GetScreensByUserID(code, UserID, HIE);
                }

                List<MenuBranchEntity> menubranhlist = _model.UserProfileAccess.GetMenuBranches();

                if (menubranhlist.Count > 0)
                {
                    foreach (MenuBranchEntity menuitem in menubranhlist)
                    {
                        if (code == "01" && UserID.ToUpper() == "JAKE")
                        {
                            PrivilegeEntity privilegeAdmn12 = new PrivilegeEntity();
                            privilegeAdmn12.UserID = UserID;
                            privilegeAdmn12.ModuleCode = "01";
                            privilegeAdmn12.Program = "ADMN0012";
                            privilegeAdmn12.Hierarchy = "******";
                            privilegeAdmn12.AddPriv = "true";
                            privilegeAdmn12.ChangePriv = "true";
                            privilegeAdmn12.DelPriv = "true";
                            privilegeAdmn12.ViewPriv = "true";
                            privilegeAdmn12.PrivilegeName = "Agency Control File Maintenance";
                            privilegeAdmn12.DateLSTC = string.Empty;
                            privilegeAdmn12.LSTCOperator = UserID;
                            privilegeAdmn12.DateAdd = string.Empty;
                            privilegeAdmn12.AddOperator = UserID;
                            privilegeAdmn12.ModuleName = string.Empty;
                            privilegeAdmn12.showMenu = "Y";
                            privilegeAdmn12.screenType = "SCREEN";
                            userPrivilege.Insert(0, privilegeAdmn12);

                        }
                        if (code == "01" && UserID.ToUpper() == "CAPLOGICS")
                        {
                        }
                        List<PrivilegeEntity> screentypewiselist = userPrivilege.FindAll(u => u.screenType.Trim() == menuitem.MemberCode.Trim());

                        if (screentypewiselist != null && screentypewiselist.Count > 0)
                        {
                            screentypewiselist = screentypewiselist.FindAll(u => u.showMenu.ToString().ToUpper() == "Y");
                            screentypewiselist = screentypewiselist.OrderBy(u => u.PrivilegeName).ToList();

                            if (menuitem.MemberCode.ToString() == "SCREEN")
                            {
                                foreach (PrivilegeEntity privilegeEntity in screentypewiselist)
                                {
                                    NavigationBarItem childNavigationBarItem = new NavigationBarItem();
                                    childNavigationBarItem.Text = privilegeEntity.PrivilegeName;
                                    childNavigationBarItem.ToolTipText = privilegeEntity.PrivilegeName;
                                    childNavigationBarItem.Height = 16;
                                    //childNavigationBarItem.IconVisible = false;
                                    //childNavigationBarItem.ImageIndex = 1;
                                    childNavigationBarItem.Icon = "Resources/Images/MenuIcons/formicon.png";
                                    childNavigationBarItem.AllowHtml = true;
                                    childNavigationBarItem.CssStyle = "height:10px; border-bottom:1px solid #F2F2F2; ";
                                    this.styleSheet1.SetCssClass(childNavigationBarItem, "NavbarItem");
                                    childNavigationBarItem.Tag = privilegeEntity;
                                    childNavigationBarItem.Controls[0].Controls[0].CssStyle = "margin-left:12px;";
                                    childNavigationBarItem.Controls[0].Controls[1].CssStyle = "margin-left:2px;";
                                    nBarModScreenHead.Items.Add(childNavigationBarItem);
                                }
                            }
                        }
                    }
                }

                userPrivilege = Capmodel.UserProfileAccess.GetReportsByUserID(code, UserID);
                if (userPrivilege != null && userPrivilege.Count > 0)
                {
                    userPrivilege = userPrivilege.OrderBy(u => u.PrivilegeName).ToList();
                    foreach (PrivilegeEntity privilegeEntity in userPrivilege)
                    {
                        NavigationBarItem childNavigationBarItem = new NavigationBarItem();
                        childNavigationBarItem.Text = privilegeEntity.PrivilegeName;
                        childNavigationBarItem.ToolTipText = privilegeEntity.PrivilegeName;
                        childNavigationBarItem.Height = 16;
                        childNavigationBarItem.Icon = "Resources/Images/MenuIcons/formicon.png";
                        childNavigationBarItem.AllowHtml = true;
                        childNavigationBarItem.CssStyle = "height:10px; border-bottom:1px solid #F2F2F2;";
                        childNavigationBarItem.Tag = privilegeEntity;
                        childNavigationBarItem.Controls[0].Controls[0].CssStyle = "margin-left:12px;";
                        childNavigationBarItem.Controls[0].Controls[1].CssStyle = "margin-left:2px;";
                        nBarModReportHead.Items.Add(childNavigationBarItem);
                    }
                }

                userPrivilege = Capmodel.UserProfileAccess.GetUserReportMaintenanceByserID(code, UserID);
                if (userPrivilege != null && userPrivilege.Count > 0)
                {

                    /******************************** User Report Maintenance Menu Head Creation  *********************************************/
                    NavigationBarItem nBarModUserReportMgntHead = new NavigationBarItem();
                    nBarModUserReportMgntHead.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                    nBarModUserReportMgntHead.AllowHtml = true;
                    nBarModUserReportMgntHead.CssStyle = "border-bottom:1px solid #F4F4F4;";
                    nBarModUserReportMgntHead.ForeColor = System.Drawing.Color.Black;
                    nBarModUserReportMgntHead.Name = "nModule_" + iModuleItem.ModuleCode + "_USRRPTMGNTHead";
                    nBarModUserReportMgntHead.Text = "User Report Maintenance";
                    nBarModUserReportMgntHead.ToolTipText = "User Report Maintenance";
                    nBarModUserReportMgntHead.Icon = "Resources/Images/MenuIcons/Reports.png";
                    nBarModUserReportMgntHead.Controls[0].Controls[0].CssStyle = "margin-left:8px;";
                    nBarModUserReportMgntHead.Controls[0].Controls[1].CssStyle = "margin-left:0px;";
                    nBarModule.Items.Add(nBarModUserReportMgntHead);        // Add Report Menu Header to Module

                    userPrivilege = userPrivilege.OrderBy(u => u.PrivilegeName).ToList();
                    foreach (PrivilegeEntity privilegeEntity in userPrivilege)
                    {
                        NavigationBarItem childNavigationBarItem = new NavigationBarItem();
                        childNavigationBarItem.Text = privilegeEntity.PrivilegeName;
                        childNavigationBarItem.ToolTipText = privilegeEntity.PrivilegeName;
                        childNavigationBarItem.Height = 16;
                        childNavigationBarItem.Icon = "Resources/Images/MenuIcons/formicon.png";
                        childNavigationBarItem.AllowHtml = true;
                        childNavigationBarItem.CssStyle = "height:10px; border-bottom:1px solid #F2F2F2;";
                        childNavigationBarItem.Tag = privilegeEntity;
                        childNavigationBarItem.Controls[0].Controls[0].CssStyle = "margin-left:12px;";
                        childNavigationBarItem.Controls[0].Controls[1].CssStyle = "margin-left:2px;";
                        nBarModUserReportMgntHead.Items.Add(childNavigationBarItem);
                    }
                }

                NavTabs.Items.Add(nBarModule);
                NavTabs.Items[0].Height = 50;
            }

            int navid = 0; int xx = 0;
            foreach (NavigationBarItem onbar in NavTabs.Items)
            {

                if (onbar.Tag.ToString() == "Head")
                {
                    onbar.Controls[0].Controls["open"].Visible = false;

                    string cnavid = onbar.Name.Split('_')[1].ToString();
                    if (strcurrentModuleID == cnavid)
                    {
                        navid = xx;
                    }
                }
                xx++;
            }

            if (NavigationPrivileges.Count > 0)
            {
                // Kranthi :: 03/06/2023 :: PCS.docx :: Wants to close all the Menu Module items when user logged in. They want to open on their own.
                NavTabs_ItemClick(NavTabs.Items[navid], new NavigationBarItemClickEventArgs(NavTabs.Items[navid]));
                if (isFirstLoad == 0)
                {
                    for (int x = 0; x < NavTabs.Items.Count; x++)
                    {
                        NavTabs.Items[x].Expanded = false;
                    }
                    isFirstLoad = 1;
                }
            }
            else
            {
                // AlertBox.Show("No previlages assigned for this user", MessageBoxIcon.Warning);

            }
        }
        int isFirstLoad = 0;

        private void NBarModule_Expand(object sender, EventArgs e)
        {

            //string args = e.GetListItem().ToString();
            //NavTabs_ItemClick(sender, new NavigationBarItemClickEventArgs());
        }

        private void OnLogoutClick(object sender, EventArgs e)
        {
            if (Application.Session["userlogid"] != null)
            {

                _model.UserProfileAccess.InsertUpdateLogUsers(string.Empty, string.Empty, string.Empty, "Edit", Application.Session["userlogid"].ToString());
            }
            Application.Session.IsLoggedOn = false;

            Application.Exit();
        }

        private void buttonCollapse_Click(object sender, EventArgs e)
        {
            var setCompact = !NavTabs.CompactView;
            NavTabs.CompactView = setCompact;
            if (setCompact == true)
            {
                //this.splitContainer1.SplitterDistance = 70;
                this.pnlMenuBar.Width = 70;
                this.pnlMenuBar.CssStyle = "transition:left 300ms linear;";
                //for (int x = 0; x < NavTabs.Items.Count; x++)
                //{
                //    NavTabs.Items[x].Expanded = false;
                //    NavTabs.Items[x].Items[0].Expanded = false; // Screens
                //    NavTabs.Items[x].Items[1].Expanded = false; // Reports
                //}

                btnCollapse.ToolTipText = "Show Menu Tree";
            }
            else
            {
                //this.splitContainer1.SplitterDistance = 300;
                this.pnlMenuBar.Width = 300;
                this.pnlMenuBar.CssStyle = "transition:left 300ms linear;";
                btnCollapse.ToolTipText = "Hide Menu Tree";
                // BuildMenuTree();

                foreach (NavigationBarItem onbar in NavTabs.Items)
                {
                    if (onbar.Tag.ToString() == "Head")
                    {
                        onbar.Controls[0].Controls["open"].Visible = false;
                    }
                }
            }

        }
        private void pnlContainer_Resize(object sender, EventArgs e)
        {
            var minFormWidth =
                Math.Max(780, Math.Min(1028, this.pnlContainer.Width - 100));

            //this.pnlTabs.MinimumSize = new Size(minFormWidth, 0);
            //this.pnlButtonBar.MinimumSize = new Size(minFormWidth, 0);
        }

        private void NavTabs_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < NavTabs.Items.Count; x++)
            {
                NavTabs.Items[x].Expanded = false;
            }
            NavigationBar onavigationBar = sender as NavigationBar;
            if (onavigationBar.Tag.ToString() == "Head")
            {
                NavTabs.Items[onavigationBar.Name].Expanded = true;


            }

        }

        string selModule = "";
        string HIE = string.Empty; //string frmHiEScren = "N";
        private void NavTabs_ItemClick(object sender, NavigationBarItemClickEventArgs e)
        {
            setDefaultIcons();

            NavigationBar onavigationBar = e.Item.Tag as NavigationBar;
            if (e.Item.Tag != null)
            {
                if (e.Item.Tag.ToString() == "Head")
                {
                    btncloseAll.Visible = false;
                    string strTabName = e.Item.Name.ToString();
                    string ModID = strTabName.Substring(8, strTabName.Length - 8).ToString();
                    BusinessModuleID = ModID;
                    colNavItems = new List<NavigationBarItem>();
                    applicationNameControl.Controls[0].Controls["panelNavButtons"].Visible = true; //Kranthi 02/01/2023 :: Brian wants to enable the navbar buttons at top main screen also, but Ram suggest to hide this at master page only show in Screens

                    if (selModule != e.Item.Text.ToString())
                    {
                        pnlTabs.Controls.Clear();
                        pnlTabs.Controls.Add(ContentTabs);
                        ContentTabs.Dock = DockStyle.Fill;
                        ContentTabs.TabPages.Clear();
                        selModule = e.Item.Text.ToString();
                    }
                    else
                    {
                        if (frmHiEScren == "N")
                        {
                            int intTabPagesTotal = ContentTabs.TabPages.Count;
                            if (e.Item.Text.Contains("Admin"))
                            {
                                if (intTabPagesTotal > 0)
                                    btncloseAll.Visible = true;
                            }
                            else
                            {
                                if (intTabPagesTotal > 1)
                                    btncloseAll.Visible = true;
                            }
                            NavTabs.Items[e.Item.Name].Expanded = true;
                            return;
                        }
                    }
                    frmHiEScren = "N";
                    if (e.Item.Text.Contains("Admin"))
                    {
                        int intTabPagesTotal = ContentTabs.TabPages.Count;
                        if (intTabPagesTotal == 0)
                        {
                            this.pnlContainer.BackgroundImageSource = "Resources\\Images\\11-01-01.jpg";
                            this.pnlContainer.BackgroundImageLayout = Wisej.Web.ImageLayout.Stretch;
                        }
                        BaseAdminAgency = _statAdminAgency;

                        pnlCenterbar.Visible = true;
                        pnlSearchbox.Visible = false;
                        pHIEFilter.Visible = true;
                        string strAgencyName = "** - All Agencies";
                        if (BaseAdminAgency != "**")
                            strAgencyName = BaseAdminAgency + " - " + _model.lookupDataAccess.GetHierachyDescription("1", BaseAdminAgency, BaseDept, BaseProg);

                        hierachyNamecontrol.lblHierchy.Text = strAgencyName;//CommonFunctions.GetHTMLHierachyFormat(strAgencyName, string.Empty, string.Empty, string.Empty, "01");

                        pnlCenterbar.Controls.Clear();
                        hierachyNamecontrol.Dock = Wisej.Web.DockStyle.Fill;
                        pnlCenterbar.Controls.Add(hierachyNamecontrol);
                    }
                    else
                    {
                        pnlContainer.BackgroundImageSource = "Resources\\Images\\blank.png";
                        pnlContainer.BackColor = Color.White;

                        pnlCenterbar.Visible = true;
                        pnlSearchbox.Visible = true;
                        pHIEFilter.Visible = true;
                        //applicationNameControl.Controls[0].Controls["panelNavButtons"].Visible = true;

                        if (_model.lookupDataAccess.CheckDefaultHierachy(BaseAgency, BaseDept, BaseProg, UserProfile.UserID) == Consts.Common.Exists)
                        {

                            HIE = BaseAgency + BaseDept + BaseProg;
                            strcurrentHIE = BaseAgency + "-" + BaseDept + "-" + BaseProg;
                            ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseAgency, BaseDept, BaseProg);
                            if (string.IsNullOrEmpty(BaseYear.Trim()))
                            {

                                if (programEntity != null)
                                {
                                    BaseYear = programEntity.DepYear.Trim() == string.Empty ? "    " : programEntity.DepYear;
                                }
                            }

                            if (BaseAgency != BaseAdminAgency)
                                BaseAdminAgency = BaseAgency;

                            BaseTopApplSelect = "N";
                            ShowHierachyandApplNo(BaseAgency, BaseDept, BaseProg, BaseYear, BaseApplicationNo, string.Empty);
                            pnlContainer.BackgroundImage = null;

                            if (!e.Item.Text.Contains("Dash Board"))
                                MainmenuAddTab();

                        }
                        else
                        {
                            _defaultHierchyform = true;
                            if (!e.Item.Text.Contains("Dash Board"))
                                MainmenuAddTab();
                            //AdvancedMainMenuSearch advancedMainMenuSearch = new AdvancedMainMenuSearch(this, false, true);
                            //advancedMainMenuSearch.FormClosed += new Form.FormClosedEventHandler(On_ADV_SerachFormClosed);
                            // advancedMainMenuSearch.ShowDialog();
                        }


                    }

                    for (int x = 0; x < NavTabs.Items.Count; x++)
                    {
                        NavTabs.Items[x].Expanded = false;
                        NavTabs.Items[x].Items[0].Expanded = false; // Screens
                        NavTabs.Items[x].Items[1].Expanded = false; // Reports
                    }
                    NavTabs.Items[e.Item.Name].Expanded = true;

                    // Kranthi :: 03/06/2023 :: PCS.docx :: Wants to close all the Menu Module items when user logged in. They want to open on their own.
                    //NavTabs.Items[e.Item.Name].Items[0].Expanded = true; //Screens

                }
                else
                {
                    //TagClass tagClass = null;
                    //tagClass = (TagClass)e.Node.Tag;
                    //OpenContentTab(tagClass);
                    NavTabs.SelectedItem = e.Item;
                    colNavItems.Add(e.Item);
                    if (BusinessModuleID == "01")
                        privlgNavAdmnItems = colNavItems;

                    NavTabs.SelectedItem.Icon = "Resources/Images/MenuIcons/formicon_white.png";
                    this.styleSheet1.SetCssClass(NavTabs.SelectedItem, "NavbarItemSelected");

                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        if (BaseTopApplSelect == "Y")
                        {
                            applicationNameControl.Controls[0].Controls["panelNavButtons"].Visible = true;
                        }
                    }
                    pnlContainer.BackgroundImageSource = "Resources\\Images\\blank.png";
                    pnlContainer.BackColor = Color.White;
                    ShowForm(e.Item.Tag as PrivilegeEntity);
                }
            }


            /************************/
            if (e.Item.Name.Contains("SCRHead") || e.Item.Name.Contains("RPTHead") || e.Item.Name.Contains("USRRPTMGNTHead"))
            {

                string[] Items = e.Item.Name.Split('_');
                string setName = Items[0] + "_" + Items[1];

                if (Items[2].ToString() == "SCRHead")
                {
                    e.Item.Expanded = true;
                    NavTabs.Items[setName].Items[setName + "_RPTHead"].Expanded = false;
                    if (NavTabs.Items[setName].Items[setName + "_USRRPTMGNTHead"] != null)
                        NavTabs.Items[setName].Items[setName + "_USRRPTMGNTHead"].Expanded = false;
                }
                if (Items[2].ToString() == "RPTHead")
                {
                    e.Item.Expanded = true;
                    NavTabs.Items[setName].Items[setName + "_SCRHead"].Expanded = false;
                    if (NavTabs.Items[setName].Items[setName + "_USRRPTMGNTHead"] != null)
                        NavTabs.Items[setName].Items[setName + "_USRRPTMGNTHead"].Expanded = false;
                }
                if (Items[2].ToString() == "USRRPTMGNTHead")
                {
                    e.Item.Expanded = true;
                    NavTabs.Items[setName].Items[setName + "_SCRHead"].Expanded = false;
                    NavTabs.Items[setName].Items[setName + "_RPTHead"].Expanded = false;
                }

            }
            /************************/
        }
        List<NavigationBarItem> colNavItems = new List<NavigationBarItem>();

        public override void OpenContentTab(TagClass tagClass)
        {
            pnlTabs.Controls.Clear();
            pnlTabs.Controls.Add(ContentTabs);
        }

        public void ShowHierachyandApplNo(string strAgency, string strDept, string strProg, string strYear1, string strApplicationNo, string strAppDisplay)
        {

            CaseMstEntity caseMstEntity = null;
            List<CaseSnpEntity> caseSnpEntity = null;
            string strYear = strYear1;
            string strApplNo = strApplicationNo;

            if (string.IsNullOrEmpty(strYear))
                strYear = "    ";

            string strAgencyName = strAgency + " - " + _model.lookupDataAccess.GetHierachyDescription("1", strAgency, strDept, strProg);
            string strDeptName = strDept + " - " + _model.lookupDataAccess.GetHierachyDescription("2", strAgency, strDept, strProg);
            string strProgName = strProg + " - " + _model.lookupDataAccess.GetHierachyDescription("3", strAgency, strDept, strProg);

            caseMstEntity = _model.CaseMstData.GetCaseMST(strAgency, strDept, strProg, strYear1, strApplNo);
            if (caseMstEntity != null)
            {
                strApplNo = caseMstEntity.ApplNo;
                strYear = caseMstEntity.ApplYr;
                BaseApplicationNo = strApplNo;
                caseSnpEntity = _model.CaseMstData.GetCaseSnpadpyn(strAgency, strDept, strProg, strYear, strApplNo);
            }
            else
            {
                BaseApplicationNo = string.Empty; // null; Modified by Yeswanth on 01052013
            }
            GetApplicantDetails(caseMstEntity, caseSnpEntity, strAgencyName, strDeptName, strProgName, strYear.ToString(), string.Empty, strAppDisplay);
        }
        public override void GetApplicantDetails(CaseMstEntity caseMstEntity, List<CaseSnpEntity> caseSNPEntity, string strAgency, string strDept, string strProgram, string strYear, string strType, string strAppDisplay)
        {
            //this.pnlApplicationHeaderImage.Controls.Clear();            
            // applicationDetailsControl = new ApplicationDetailsControl(this);
            //this.pnlApplicationHeaderImage.Controls.Add(applicationDetailsControl);
            //this.pnlApplicationHeaderImage.Size = new System.Drawing.Size(315, 90);
            if (strType != "LinkApplicant")
            {

                HierarchyEntity hierachyEntity = _model.HierarchyAndPrograms.GetCaseHierarchyName(strAgency.Substring(0, 2), "**", "**");
                if (hierachyEntity != null)
                {
                    hierachyNamecontrol.lblHierchy.Text = CommonFunctions.GetHTMLHierachyFormat(strAgency, strDept, strProgram, strYear, hierachyEntity.HIERepresentation);
                    BaseHierarchyCnFormat = hierachyEntity.CNFormat.ToString();
                    BaseHierarchyCwFormat = hierachyEntity.CWFormat.ToString();
                }
            }
            // hierachyNamecontrol.lblHierchy.Text = strAgency + "        " + strDept + "        " + strProgram + "        " + strYear;


            BaseApplicationName = string.Empty;                                         // Yeswanth To ClearCommon Text in Header
            applicationNameControl.lblApplicationName.Text = string.Empty;
            applicationNameControl.labelAddress.Text = string.Empty;
            applicationNameControl.txtAppNo.Text = string.Empty;

            // applicationDetailsControl.ClearGridData();

            BaseCaseMstListEntity = null;
            BaseCaseSnpEntity = null;

            if (caseMstEntity != null)  // Yeswanth To Bypass Hierarchies With No Applicants
            {
                List<CaseMstEntity> casmsttemp = new List<CaseMstEntity>();
                casmsttemp.Add(caseMstEntity);
                BaseCaseMstListEntity = casmsttemp;
                BaseCaseSnpEntity = caseSNPEntity;
                if (BaseCaseSnpEntity.Count > 0)
                    BaseCaseSnpEntity.Find(u => u.FamilySeq.Equals(BaseCaseMstListEntity[0].FamilySeq)).M_Code = "A";
                // string strFormat = applicationDetailsControl.FillGridData(caseMstEntity.ApplAgency.ToString(), caseMstEntity.ApplDept.ToString(), caseMstEntity.ApplProgram.ToString(), caseMstEntity.ApplYr.ToString(), caseMstEntity.ApplNo.ToString());
                foreach (CaseSnpEntity caseSnp in caseSNPEntity)
                {
                    if (caseSnp.FamilySeq == caseMstEntity.FamilySeq)
                    {
                        BaseApplicationName = LookupDataAccess.GetMemberName(caseSnp.NameixFi, caseSnp.NameixMi, caseSnp.NameixLast, BaseHierarchyCnFormat);
                        string strZipcode = "00000".Substring(0, 5 - caseMstEntity.Zip.Length) + caseMstEntity.Zip;

                        string strPhonedis = string.Empty;
                        string strtelephone = string.Empty;
                        if (caseMstEntity.Area != string.Empty && caseMstEntity.Phone != string.Empty && caseMstEntity.Phone.Length == 7)
                        {
                            strPhonedis = "  Home (" + caseMstEntity.Area + ")" + caseMstEntity.Phone.Substring(0, 3) + "-" + caseMstEntity.Phone.Substring(3, 4);
                        }
                        if (caseMstEntity.CellPhone != string.Empty && caseMstEntity.CellPhone.Trim().Length == 10)
                        {
                            strtelephone = " Cell (" + caseMstEntity.CellPhone.Substring(0, 3) + ")" + caseMstEntity.CellPhone.Substring(3, 3) + "-" + caseMstEntity.CellPhone.Substring(6, 4);
                        }
                        if (strZipcode == "00000")
                            strZipcode = "";
                        //if (strAppDisplay == "Display")
                        //{
                        //    applicationNameControl.lblApplicationName.Text = BaseApplicationName + "     " + caseMstEntity.Hn.Trim() + ' ' + caseMstEntity.Street.Trim() + ' ' + caseMstEntity.Suffix.Trim() + ' ' + caseMstEntity.City.Trim() + ' ' + caseMstEntity.State.Trim() + ' ' + strZipcode + strPhonedis;
                        //    applicationNameControl.txtAppNo.Text = caseMstEntity.ApplNo;
                        //}
                        //else
                        //{
                        //    applicationNameControl.lblApplicationName.Text = "";
                        //    applicationNameControl.txtAppNo.Text = string.Empty;
                        //}

                        if (strAppDisplay == "Display")
                        {
                            if (BaseAgencyControlDetails.SitesData == "1")
                            {
                                string strSite = string.Empty;
                                if (caseMstEntity.Site.Trim() != string.Empty)
                                    strSite = " (" + caseMstEntity.Site.Trim() + ")";
                                applicationNameControl.lblApplicationName.Text = BaseApplicationName + strSite;
                            }
                            else
                            {
                                applicationNameControl.lblApplicationName.Text = BaseApplicationName;
                            }
                            // TODO: Levie remove.
                            //ToolTip toolnewtip = new ToolTip();
                            //toolnewtip.SetToolTip(applicationNameControl.lblApplicationName, caseMstEntity.Hn.Trim() + ' ' + caseMstEntity.Street.Trim() + ' ' + caseMstEntity.Suffix.Trim() + ' ' + caseMstEntity.City.Trim() + ' ' + caseMstEntity.State.Trim() + ' ' + strZipcode + strPhonedis + strtelephone);
                            applicationNameControl.labelAddress.Text = caseMstEntity.Hn.Trim() + ' ' + caseMstEntity.Street.Trim() + ' ' + caseMstEntity.Suffix.Trim() + ' ' + caseMstEntity.City.Trim() + ' ' + caseMstEntity.State.Trim() + ' ' + strZipcode;
                            applicationNameControl.labelPhone.Text = $"{strPhonedis}{strtelephone}";
                            applicationNameControl.txtAppNo.Text = caseMstEntity.ApplNo;
                        }
                        else
                        {
                            applicationNameControl.lblApplicationName.Text = "";
                            ToolTip toolnewtip = new ToolTip();
                            toolnewtip.SetToolTip(applicationNameControl.lblApplicationName, "");
                            applicationNameControl.txtAppNo.Text = string.Empty;
                        }
                        if (BaseTopApplSelect == "Y" || !string.IsNullOrEmpty(BaseApplicationNo.Trim()))
                        {

                            applicationNameControl.Btn_First.Visible = applicationNameControl.BtnP10.Visible = applicationNameControl.BtnPrev.Visible =
                            applicationNameControl.BtnNxt.Visible = applicationNameControl.BtnN10.Visible = applicationNameControl.BtnLast.Visible = true;
                        }
                        else
                        {
                            applicationNameControl.Btn_First.Visible = applicationNameControl.BtnP10.Visible = applicationNameControl.BtnPrev.Visible =
                              applicationNameControl.BtnNxt.Visible = applicationNameControl.BtnN10.Visible = applicationNameControl.BtnLast.Visible = false;
                        }
                    }
                }
            }
            else
            {
                applicationNameControl.Btn_First.Visible = applicationNameControl.BtnP10.Visible = applicationNameControl.BtnPrev.Visible =
                             applicationNameControl.BtnNxt.Visible = applicationNameControl.BtnN10.Visible = applicationNameControl.BtnLast.Visible = false;
            }

        }
        private void MainmenuAddTab()
        {
            MainMenuControl mainMenuControl = new MainMenuControl(this);
            AddContentTab("MainMenu Search", "MainMenu", mainMenuControl);

            //pnlSearchbox.Controls.Add(hierachyNamecontrol);
            pnlCenterbar.Controls.Clear();
            pnlCenterbar.Controls.Add(hierachyNamecontrol);

            hierachyNamecontrol.Dock = Wisej.Web.DockStyle.Fill;
            // hierachyNamecontrol.Location = new System.Drawing.Point(200, 6);
            hierachyNamecontrol.Name = "hierachykeycontrol";
            //hierachyNamecontrol.Size = new System.Drawing.Size(720, 20);

            pnlSearchbox.Controls.Clear();
            //pnlSearchbox.Padding = new  System.Windows.Forms.Padding(25, 0, 135, 0);
            pnlSearchbox.Controls.Add(applicationNameControl);
            applicationNameControl.BringToFront();

            applicationNameControl.Name = "applicationNameControl";
            applicationNameControl.lblApplicationName.Visible = true;
            applicationNameControl.Dock = Wisej.Web.DockStyle.Fill;


            applicationNameControl.Controls[0].Controls["panelNavButtons"].Visible = true;
            //foreach (Control c in applicationNameControl.Controls)
            //{
            //    if (c.Name == "panelNavButtons")
            //        c.Visible = false;
            //}

            // ((Panel)fc.Ctrl(fc.TheForm("ApplicationNameControl"), "panelNavButtons")).Visible = false;
        }
        public void AddContentTab(string title, string name, UserControl baseUserControl)
        {
            isForm = "N";
            if (ContentTabs.TabPages.Count == 0)
            {
                pnlTabs.Controls.Clear();
                pnlTabs.Controls.Add(ContentTabs);
                ContentTabs.Dock = DockStyle.Fill;
            }
            CurrentTabPage = null;
            try
            {
                for (int iContentTabs = 0; iContentTabs < ContentTabs.TabPages.Count; iContentTabs++)
                {
                    string tagItem = ContentTabs.TabPages[iContentTabs].Tag.ToString();
                    if (tagItem == null) { continue; }
                    if (tagItem == name && !string.IsNullOrEmpty(name))
                    {
                        CurrentTabPage = ContentTabs.TabPages[iContentTabs];
                        if (CurrentTabPage.Text == title)
                        {
                            ContentTabs.SelectedTab = CurrentTabPage;
                            break;
                        }
                    }
                }

                if (CurrentTabPage == null)
                {
                    CurrentTabPage = new TabPage();
                    CurrentTabPage.ShowCloseButton = name != "MainMenu";
                    this.ContentTabs.Controls.Add(CurrentTabPage);
                    CurrentTabPage.Controls.Add(baseUserControl);
                    baseUserControl.Dock = DockStyle.Fill;
                    CurrentTabPage.Name = name + ContentTabs.TabPages.Count.ToString();
                    CurrentTabPage.Tag = name;
                    CurrentTabPage.TabIndex = 0;
                    ContentTabs.SelectedIndexChanged -= OnContentTabsSelectedIndexChanged;
                    CurrentTabPage.Text = title;
                    CurrentTabPage.BackColor = System.Drawing.Color.White;
                    ContentTabs.SelectedTab = CurrentTabPage;


                    ContentTabs.SelectedIndexChanged += OnContentTabsSelectedIndexChanged;
                    ContentTabs.TabClosed += ContentTabs_TabClosed;
                }


                //BaseUserControl baseUserControl1 = GetBaseUserControl();
                //if (baseUserControl1 != null)
                //{
                //    MainToolbar = baseUserControl1.ToolbarMnustrip();
                //    baseUserControl1.PopulateToolbar(MainToolbar);
                //}
            }
            catch (Exception ex)
            {
                //
            }
        }


        private void ContentTabs_TabClosed(object sender, TabControlEventArgs e)
        {
            //string strMsg = "" + e.TabPage.Name + " form closed";
            //AlertBox.Show(strMsg);
            if (BusinessModuleID == "01")
            {
                this.pnlContainer.BackgroundImageSource = "Resources\\Images\\11-01-01.jpg";
                this.pnlContainer.BackgroundImageLayout = Wisej.Web.ImageLayout.Stretch;
                int intTabPagesTotal = ContentTabs.TabPages.Count;
                if (intTabPagesTotal == 0)
                {
                    btncloseAll.Visible = false;
                    NavTabs.SelectedItem = null;
                }
            }
            else
            {
                pnlContainer.BackgroundImageSource = "Resources\\Images\\blank.png";
                pnlContainer.BackColor = Color.White;
                int intTabPagesTotal = ContentTabs.TabPages.Count;
                if (intTabPagesTotal == 1)
                {
                    btncloseAll.Visible = false;
                    NavTabs.SelectedItem = null;
                    applicationNameControl.Controls[0].Controls["panelNavButtons"].Visible = true;
                }
            }

        }

        void setDefaultIcons()
        {
            foreach (NavigationBarItem nvItem in NavTabs.Items)
            {

                foreach (NavigationBarItem nvItem_src in nvItem.Items[0].Items)
                {
                    nvItem_src.Icon = "Resources/Images/MenuIcons/formicon.png";
                    this.styleSheet1.SetCssClass(nvItem_src, "NavbarItem");
                }
                foreach (NavigationBarItem nvItem_rpt in nvItem.Items[1].Items)
                {
                    nvItem_rpt.Icon = "Resources/Images/MenuIcons/formicon.png";
                    this.styleSheet1.SetCssClass(nvItem_rpt, "NavbarItem");
                }

                //Kranthi :: 03/01/2023:: User report maintenance 
                if (nvItem.Items.Count > 2)
                {
                    foreach (NavigationBarItem nvItem_userrpt in nvItem.Items[2].Items)
                    {
                        nvItem_userrpt.Icon = "Resources/Images/MenuIcons/formicon.png";
                        this.styleSheet1.SetCssClass(nvItem_userrpt, "NavbarItem");
                    }
                }
            }
        }
        private void OnContentTabsSelectedIndexChanged(object sender, EventArgs e)
        {
            setDefaultIcons();

            UserEntity userInfo = Captain<UserEntity>.Session[Consts.SessionVariables.UserProfile];
            try
            {
                if (BusinessModuleID != "01")
                {
                    MainToolbar.Buttons.Clear();  // Rao
                }

                int intSelect = ContentTabs.SelectedIndex;
                if (intSelect >= 0 || BusinessModuleID == "01")  // (intSelect > 0 )  No need to check the condition - Changed by Yeswanth on 02/04/2013
                {
                    CurrentTabPage = (TabPage)ContentTabs.TabPages[intSelect];

                    List<NavigationBarItem> SeltempNavbar = colNavItems.FindAll(x => ((PrivilegeEntity)x.Tag).Program.ToString() == CurrentTabPage.Tag.ToString());
                    if (SeltempNavbar.Count > 0)
                    {
                        NavTabs.SelectedItem = SeltempNavbar[0];
                        NavTabs.SelectedItem.Icon = "Resources/Images/MenuIcons/formicon_white.png";
                        this.styleSheet1.SetCssClass(NavTabs.SelectedItem, "NavbarItemSelected");
                    }
                    else
                        NavTabs.SelectedItem = null;


                    if (CurrentTabPage.Tag == null)
                    {
                        MainToolbar.Buttons.Clear();
                        return;
                    }


                    string tagClass = null;
                    if (CurrentTabPage.Tag is string)
                    {
                        tagClass = (string)CurrentTabPage.Tag;
                    }

                    if (tagClass != null)
                    {
                        // MainToolbar.Buttons.Clear();


                        //BaseUserControl baseUserControl = GetBaseUserControl();

                        //if (baseUserControl != null)
                        //{
                        //    MainToolbar = baseUserControl.ToolbarMnustrip();
                        //    baseUserControl.PopulateToolbar(MainToolbar);
                        //}

                        if (BusinessModuleID != "01")
                        {
                            if (CurrentTabPage.Tag.ToString() == "MainMenu")
                                applicationNameControl.Controls[0].Controls["panelNavButtons"].Visible = true;
                            else
                                applicationNameControl.Controls[0].Controls["panelNavButtons"].Visible = true;
                        }
                        RefreshOpenedAdminPages();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        string isForm = "Y";
        private void ShowForm(PrivilegeEntity privilegeEntity)
        {
            string Ishelp = "N"; isForm = "Y";

            string nodeLabel = string.Empty;
            if (privilegeEntity == null) return;
            switch (privilegeEntity.Program.Trim())
            {
                #region ADMINISTRATION
                #region SCREENS
                #region Assign User Privilages
                case "ADMN0005": //Assign User Privilages
                    UserListControl ulc = new UserListControl(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ulc);
                    break;
                #endregion
                #region Agency Table Codes
                case "ADMN0006": //Agency Table Codes
                    AgencyTableControl agencyTabelControl = new AgencyTableControl(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, agencyTabelControl);
                    break;
                #endregion
                #region Hierarchy Definition
                case "ADMN0009": //Hierarchy Definition
                    HierarchyDefinitionControl hierarchyDefinitionControl = new HierarchyDefinitionControl(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, hierarchyDefinitionControl);
                    break;
                #endregion
                #region Master Poverty Guidelines 
                case "ADMN0010": //Master Poverty Guidelines 
                    MasterPoverityGuidelineControl masterPovertyControl = new MasterPoverityGuidelineControl(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, masterPovertyControl);
                    break;
                #endregion
                #region Agency Control file Maintanence
                case "ADMN0012": //Agency Control file Maintanence
                    AgencyHierarchyForm agencyForm = new AgencyHierarchyForm(this, "ADMN0012", "AGENCY", "00", privilegeEntity);
                    agencyForm.StartPosition = FormStartPosition.CenterScreen;
                    agencyForm.ShowDialog();
                    break;
                #endregion
                #region Zipcode
                //case "ADMN0013": //Zipcode
                //    ADMN0013 ADMN0013 = new ADMN0013(this, privilegeEntity);
                //    ////AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, zipCodeControl);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0013);
                //    break;
                #endregion
                #region Incomplete Intake Control
                case "ADMNCONT":    //Incomplete Intake Control
                    AdminScreenControls ADMNCONT = new AdminScreenControls(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMNCONT);
                    break;
                #endregion
                #region Field Control Maintenance
                //case "CASE0008":    //Field Control Maintenance
                //    FLDCNTLMaintenanceControl FLDControl = new FLDCNTLMaintenanceControl(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, FLDControl);
                //    break;
                #endregion
                #region Eligibilty Criteria
                //case "CASE0009": //Eligibilty Criteria
                //    Case0009Control Case0009 = new Case0009Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Case0009);
                //    break;
                #endregion
                #region Agency Referral Database Screen
                //case "CASE0011": // Agency Referral Database Screen
                //    Case2011Control AgencyReferal = new Case2011Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, AgencyReferal);
                //    break;
                #endregion
                #region Site and room maintenance
                //case "ADMN0015":    //Site and room maintenance
                //    ADMN0015Control CaseSite = new ADMN0015Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CaseSite);
                //    break;
                #endregion
                #region SP Admin Screen
                //case "ADMN0020":    //SP Admin Screen
                //    ADMN0020control ADMN0020 = new ADMN0020control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0020);
                //    break;
                #endregion
                #region Contact and Service Activity Custom Questions
                //case "ADMN0022": //Contact and Service Activity Custom Questions
                //    ADMN0022Control ADMN0022 = new ADMN0022Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0022);
                //    break;
                #endregion
                #region Program Report Control Table (SSBG)
                //case "ADMN0025":    //Program Report Control Table (SSBG)
                //    SSBGParams_Control SSBGParamscontrol = new SSBGParams_Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, SSBGParamscontrol);
                //    break;
                #endregion
                #region Vendor Maintenance
                //case "TMS00009":   //Vendor Maintenance
                //    Vendor_Maintainance Vendor = new Vendor_Maintainance(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Vendor);
                //    break;
                #endregion
                #region Master Table Maintenance
                //case "ADMN0016":    //Master Table Maintenance
                //    CriticalActivity ADMN0016 = new CriticalActivity(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0016);
                //    break;
                #endregion
                #region Program Definition
                //case "CASE0007":    //Program Definition
                //    ProgramDefinition programdefinitioncontrol = new ProgramDefinition(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, programdefinitioncontrol);
                //    break;
                #endregion
                #region HouseholdID Audit and Review
                case "FIXFAMID":    //HouseholdID Audit and Review
                    FIXFAMILYIDForm fixfamidForm = new FIXFAMILYIDForm(this, privilegeEntity, string.Empty);
                    fixfamidForm.StartPosition = FormStartPosition.CenterScreen;
                    fixfamidForm.ShowDialog();
                    break;
                #endregion
                #region ClientID Audit and Review
                case "FIXSSNUM":    //ClientID Audit and Review
                    FIXSSNFORM fixSSNForm = new FIXSSNFORM(this, privilegeEntity);
                    fixSSNForm.StartPosition = FormStartPosition.CenterScreen;
                    fixSSNForm.ShowDialog();
                    break;
                #endregion
                #region Matrix/Scales Definitions
                //case "MAT00001":    //Matrix/Scales Definitions
                //    MAT00001Control MAT00001 = new MAT00001Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, MAT00001);
                //    break;
                #endregion
                #region Matrix/Scales Score Sheets
                //case "MAT00002":    //Matrix/Scales Score Sheets
                //    MAT00002Control MAT00002 = new MAT00002Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, MAT00002);
                //    break;
                #endregion
                #region Fuel Control
                case "FUELCNTL":    //Fuel Control
                    FuelControl Fuel = new FuelControl(this, privilegeEntity);
                    Fuel.StartPosition = FormStartPosition.CenterScreen;
                    Fuel.ShowDialog();
                    break;
                #endregion
                #region PIP Control
                //case "PIPADMIN":
                //    try
                //    {

                //        SqlConnection connect = new SqlConnection();
                //        connect.ConnectionString = this.BaseLeanDataBaseConnectionString;
                //        connect.Open();
                //        if (connect.State == ConnectionState.Open)
                //        {
                //            connect.Close();
                //            PIPAdmin pipAdminForm = new PIPAdmin(this, privilegeEntity, string.Empty, "CUSTOM");
                //            pipAdminForm.StartPosition = FormStartPosition.CenterScreen;
                //            pipAdminForm.ShowDialog();
                //        }

                //        else { MessageBox.Show("Connection issue with Server \n Please contact CAPSYSTEMS INC", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                //    }
                //    catch (Exception)
                //    {

                //        MessageBox.Show("Connection issue with Server \n Please contact CAPSYSTEMS INC", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    }

                //    break;
                #endregion
                #region Dashboard
                case "RPREPOR3":
                    //ReportGridControl1 RPREPOR3 = new ReportGridControl1(this, privilegeEntity, "RPREPOR3");
                    //AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, RPREPOR3);

                    //DashboardControl RPREPOR3 = new DashboardControl(this, privilegeEntity, "RPREPOR3");
                    //AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, RPREPOR3);
                    //break;

                #endregion
                #region CEAP Control
                case "ADMN0027":            // CEAP Control
                    //ADMN0027Control Admn0027control = new ADMN0027Control(this, privilegeEntity);
                    //AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Admn0027control);
                    //break;
                #endregion
                #region Service Plan Traget Entry
                //case "ADMN0030":    //Service Plan Traget Entry
                //    ADMN0030_Control ADMN0030 = new ADMN0030_Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0030);
                //    break;
                #endregion

                #region Print Application Control
                case "ADMN0031":    //Print Application Control
                    PrintApplicationControl ADMN0031 = new PrintApplicationControl(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0031);
                    break;
                #endregion
                #region Scheduler Audit
                //case "SCHAUDIT":    //Scheduler Audit
                //    RIXMLADTControl RIXMLADT = new RIXMLADTControl(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, RIXMLADT);
                //    break;
                #endregion

                #region Trigger Control Definition
                //case "TMSTRIGG":
                //    CTTriggersControl CTTriggercontrol = new CTTriggersControl(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CTTriggercontrol);
                //    break;
                #endregion

                #region Holiday's Calendar
                //case "ADMN0029":    //Holiday's Calendar
                //    ADMN0029Control _ADMN0029Control = new ADMN0029Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, _ADMN0029Control);
                //    break;
                #endregion

                #region Agency Partner Data
                //case "ADMN0011":
                //    AGCYPARTControl AgencyPartner = new AGCYPARTControl(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, AgencyPartner);
                //    break;
                #endregion

                #region Trigger Parameter Selection
                //case "TRIGPARA":    //Trigger Parameter Selection
                //    Trigger_Params Trigger_ParamsForm = new Trigger_Params(this, privilegeEntity);
                //    Trigger_ParamsForm.StartPosition = FormStartPosition.CenterScreen;
                //    Trigger_ParamsForm.ShowDialog();
                //    break;
                #endregion
                #endregion

                #region REPORTS
                //case "ADMNB001": // Report :: Master Tables List
                //    ADMNB001 ADMNB001Form = new ADMNB001(this, privilegeEntity);
                //    ADMNB001Form.StartPosition = FormStartPosition.CenterScreen;
                //    ADMNB001Form.ShowDialog();
                //    break;
                //case "ADMNB002":    // Report:: User tree structure
                //    ADMNB002 ADMNB002Form = new ADMNB002(this, privilegeEntity);
                //    ADMNB002Form.StartPosition = FormStartPosition.CenterScreen;
                //    ADMNB002Form.ShowDialog();
                //    break;

                case "RNGS0014":        // Report:: Roma Services
                    RNGS0014 RNGS0014 = new RNGS0014(this, privilegeEntity);
                    RNGS0014.StartPosition = FormStartPosition.CenterScreen;
                    RNGS0014.ShowDialog();
                    break;

                //case "ADMNB005":    // Report:: Image Types Report
                //    try
                //    {

                //        if (BusinessModuleID == "08")
                //        {
                //            //connect.Close();
                //            ADMNB005 ADMNB005_Report = new ADMNB005(this, privilegeEntity);
                //            ADMNB005_Report.StartPosition = FormStartPosition.CenterScreen;
                //            ADMNB005_Report.ShowDialog();
                //        }
                //        else if(BusinessModuleID=="01")
                //        {
                //            SqlConnection connect = new SqlConnection();
                //            connect.ConnectionString = this.BaseLeanDataBaseConnectionString;
                //            connect.Open();
                //            if (connect.State == ConnectionState.Open)
                //            {
                //                connect.Close();
                //                ADMNB005 ADMNB005_Report = new ADMNB005(this, privilegeEntity);
                //                ADMNB005_Report.StartPosition = FormStartPosition.CenterScreen;
                //                ADMNB005_Report.ShowDialog();
                //            }
                //        }

                //        else { AlertBox.Show("Connection issue with Server \n Please contact CAPSYSTEMS INC", MessageBoxIcon.Warning); }
                //    }
                //    catch (Exception)
                //    {

                //        AlertBox.Show("Connection issue with Server \n Please contact CAPSYSTEMS INC", MessageBoxIcon.Warning);
                //    }
                //    break;

                //case "PIPB0002":    //Report:: PIP Intake Report
                //    PIPB0004 PIPB004Form = new PIPB0004(this, privilegeEntity);
                //    PIPB004Form.StartPosition = FormStartPosition.CenterScreen;
                //    PIPB004Form.ShowDialog();
                //    break;

                //case "PIPB0001":    //Report:: PIP Registration Report
                //    PIPB0003 PIPB003Form = new PIPB0003(this, privilegeEntity);
                //    PIPB003Form.StartPosition = FormStartPosition.CenterScreen;
                //    PIPB003Form.ShowDialog();
                //    break;


                #endregion
                #endregion

                #region CASEMANAGEMENT
                #region SCREENS
                #region Appointment Template
                //case "APPT0001":    // Appointment Template
                //    TMS20100 tms20100 = new TMS20100(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, tms20100);
                //    break;
                #endregion
                #region Appointment Schedule
                //case "APPT0002": //Appointment Schedule
                //    APPT0002Control APPT0002 = new APPT0002Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, APPT0002);
                //    break;
                #endregion
                #region Appointment-Reserve Schedule
                //case "APPT0003":    // Appointment-Reserve Schedule
                //    APPT0003Control APPT0003 = new APPT0003Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, APPT0003);
                //    break;
                #endregion
                #region Client Intake
                case "CASE2001":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        _privilegeEntity = privilegeEntity;
                        if (BaseTopApplSelect == "Y")
                        {
                            Case3001Control clientIntakeControl = new Case3001Control(this, privilegeEntity);
                            // ClientIntakeControl clientIntakeControl = new ClientIntakeControl(this, privilegeEntity);
                            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, clientIntakeControl);
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else

                        CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    break;
                #endregion
                #region Medical/Emergency
                //case "CASE2330":    // Medical/Emergency
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            Case2330Form case2330form = new Case2330Form(this, privilegeEntity);
                //            case2330form.StartPosition = FormStartPosition.CenterScreen;
                //            case2330form.ShowDialog();
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    }
                //    break;
                #endregion
                #region Client Follow-up On
                //case "CASE0028":        // Client Follow-up On
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            CASE0028Control case0028 = new CASE0028Control(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, case0028);
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    }
                //    break;
                #endregion
                #region Client Follw-up On Search Tool
                //case "CASE0027":    //Client Follw-up On Search Tool
                //    CASE0027Control case0027 = new CASE0027Control(this, privilegeEntity, string.Empty);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, case0027);
                //    break;
                #endregion
                #region SIM (Service Intergration Matrix)
                //case "CASE2004":
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            CaseSumControl Case2004 = new CaseSumControl(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Case2004);
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    }
                //    break;
                #endregion
                #region Member base service Activity Posting
                //case "CASE1006":    // Member base service Activity Posting
                //    if (!string.IsNullOrEmpty(BaseApplicationNo) && BaseAgencyControlDetails.MemberActivity == "Y")
                //    {
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            CASE6006Control CASE4006 = new CASE6006Control(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CASE4006);
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    }
                //    break;
                #endregion
                #region Matrix Scale Assessment
                //case "MAT00003":    // Matrix Scale Assessment
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            MAT00003Control MAT00003 = new MAT00003Control(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, MAT00003);
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    }
                //    break;
                #endregion
                #region Service Activity Posting
                case "CASE0006":    //Service Activity Posting
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        if (BaseTopApplSelect == "Y")
                        {
                            if (BaseAgencyControlDetails.CAOBO.Trim() == "Y" && BaseAgencyControlDetails.QuickPostServices.Trim() != "Y")
                            {
                                CASE5006Control CASE5006 = new CASE5006Control(this, privilegeEntity);
                                AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CASE5006);
                            }
                            else
                            {
                                CASE4006Control CASE4006 = new CASE4006Control(this, privilegeEntity);
                                AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CASE4006);
                            }
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;
                #endregion
                //#region Public Intake Portal Hub
                //case "PIP00000":    //Public Intake Portal Hub
                //    try
                //    {
                //        SqlConnection connect = new SqlConnection();
                //        connect.ConnectionString = this.BaseLeanDataBaseConnectionString;
                //        connect.Open();
                //        if (connect.State == ConnectionState.Open)
                //        {
                //            connect.Close();
                //            PIP00000Control pIP00000Control = new PIP00000Control(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, pIP00000Control);
                //        }

                //        else { AlertBox.Show("Connection issue with Server \n Please contact CAPSYSTEMS INC", MessageBoxIcon.Warning); }
                //    }
                //    catch (Exception)
                //    {

                //        AlertBox.Show("Connection issue with Server \n Please contact CAPSYSTEMS INC", MessageBoxIcon.Warning);
                //    }
                //    break;
                //#endregion
                //#region Program Enrollment & Enrollment/Withdrawls
                //case "CASE0010":
                //    CASE0010_Control Enrollment = new CASE0010_Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Enrollment);
                //    break;
                //#endregion
                //#region Import Crosswalk
                //case "ADMN0028":    // Appointment Template
                //    ADMN0028Control2 Admn0028 = new ADMN0028Control2(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Admn0028);
                //    break;
                //#endregion
                //#region Print Application Control
                //case "ADMN0032":    //Print Application Control
                //    ADMN0032_Control ADMN0032 = new ADMN0032_Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0032);
                //    break;
                //#endregion

                #endregion

                #region REPORTS
                case "RNGB0004": //ROMA Individual/Household Characteristics
                    RNGB0004Form RNGb0004Form = new RNGB0004Form(this, privilegeEntity);
                    RNGb0004Form.StartPosition = FormStartPosition.CenterScreen;
                    RNGb0004Form.ShowDialog();
                    break;
                case "RNGB0014":    //ROMA Outcome Indicators
                    RNGB0014Form RNGb0014Form = new RNGB0014Form(this, privilegeEntity);
                    RNGb0014Form.StartPosition = FormStartPosition.CenterScreen;
                    RNGb0014Form.ShowDialog();
                    break;
                case "RNGB0005": // Program Service and Outcomes Report
                    RNGB0005Form RNGb0005Form = new RNGB0005Form(this, privilegeEntity);
                    RNGb0005Form.StartPosition = FormStartPosition.CenterScreen;
                    RNGb0005Form.ShowDialog();
                    break;
                //case "APPTB001": //Appointment Schedule Report
                //    APPTB001_Report APPTB001Form = new APPTB001_Report(this, privilegeEntity);
                //    APPTB001Form.StartPosition = FormStartPosition.CenterScreen;
                //    APPTB001Form.ShowDialog();
                //    break;
                //case "CASB0007":    //Funnel Report
                //    CASB4007_FunnelReport Funnel_Form = new CASB4007_FunnelReport(this, privilegeEntity);
                //    Funnel_Form.StartPosition = FormStartPosition.CenterScreen;
                //    Funnel_Form.ShowDialog();
                //    break;
                //case "CASB0008":    // Customer Intake Quality Control Report
                //    CASB0008 CASB0008_Report = new CASB0008(this, privilegeEntity);
                //    CASB0008_Report.StartPosition = FormStartPosition.CenterScreen;
                //    CASB0008_Report.ShowDialog();
                //    break;
                //case "CASB0009":
                //    CASB0009_Report CASB0009_Report = new CASB0009_Report(this, privilegeEntity);
                //    CASB0009_Report.StartPosition = FormStartPosition.CenterScreen;
                //    CASB0009_Report.ShowDialog();
                //    break;
                //case "CASB0013":    // Agency Width Activity Report
                //    CASB0013Form caseb0013form = new CASB0013Form(this, privilegeEntity);
                //    caseb0013form.StartPosition = FormStartPosition.CenterScreen;
                //    caseb0013form.ShowDialog();
                //    break;
                #region Program Statistical Report (SSBG)
                //case "CASB0014":    //Program Statistical Report (SSBG)
                //    SSBG_Report SSBGRep = new SSBG_Report(this, privilegeEntity);
                //    SSBGRep.StartPosition = FormStartPosition.CenterScreen;
                //    SSBGRep.ShowDialog();
                //    break;
                #endregion
                //case "CASB0530": //Ranking/Risk Assessment Report
                //    Casb2530Form casb2530form = new Casb2530Form(this, privilegeEntity);
                //    casb2530form.StartPosition = FormStartPosition.CenterScreen;
                //    casb2530form.ShowDialog();
                //    break;
                case "DIMSCORE":    // Dimension Score Report
                    DIMSCOREREPORT DIMSCOREREPORTForm = new DIMSCOREREPORT(this, privilegeEntity);
                    DIMSCOREREPORTForm.StartPosition = FormStartPosition.CenterScreen;
                    DIMSCOREREPORTForm.ShowDialog();
                    break;
                //case "MATB1002":
                //    MATB1002_Form MATB1002Form = new MATB1002_Form(this, privilegeEntity); ;
                //    MATB1002Form.StartPosition = FormStartPosition.CenterScreen;
                //    MATB1002Form.ShowDialog();
                //    break;
                case "RNGB0006": // PPR Report
                    RNGB0006Form RNGb0006Form = new RNGB0006Form(this, privilegeEntity);
                    RNGb0006Form.StartPosition = FormStartPosition.CenterScreen;
                    RNGb0006Form.ShowDialog();
                    break;
                #region Smart Choice Feed
                //case "CASB0016":    // Smart Choice Feed
                //    CASB0016_Report casb0016form = new CASB0016_Report(this, privilegeEntity);
                //    casb0016form.StartPosition = FormStartPosition.CenterScreen;
                //    casb0016form.ShowDialog();
                //    break;
                #endregion
                #endregion
                #endregion

                #region CEAP PROGRAM
                #region SCREENS
                #region Budget Maintenance
                //case "CASE0026":
                //    CASE0026Control ADMN0026 = new CASE0026Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0026);
                //    break;
                #endregion

                #region Benifit Maintenance & Usage Posting
                //case "CASE0016":
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            CASE0016_Control CASE4006_Usage = new CASE0016_Control(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CASE4006_Usage);

                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    }
                //    break;
                #endregion

                #region Service Payment Adjustment

                //case "CASE0021":
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            CASE0021Control CASE0021cont = new CASE0021Control(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CASE0021cont);
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    }
                //    break;
                #endregion



                #endregion

                #region REPORTS
                #region Bundling
                //case "CASB0019":    //Pledge Sheet\Bundling
                //    CASB0019_Bundling casb0019form = new CASB0019_Bundling(this, privilegeEntity);
                //    casb0019form.StartPosition = FormStartPosition.CenterScreen;
                //    casb0019form.ShowDialog();
                //    break;
                #endregion
                #region Usage Report
                //case "CASB0017":    //Usage Report
                //    CASB0017_Report casb0017form = new CASB0017_Report(this, privilegeEntity);
                //    casb0017form.StartPosition = FormStartPosition.CenterScreen;
                //    casb0017form.ShowDialog();
                //    break;
                #endregion
                #region Request for Payment Process
                //case "CASB0020":    //Request for Payment Process
                //    CASB0020_CheckProcessing casb0020form = new CASB0020_CheckProcessing(this, privilegeEntity);
                //    casb0020form.StartPosition = FormStartPosition.CenterScreen;
                //    casb0020form.ShowDialog();
                //    break;
                #endregion
                #region Performance Measures Data
                //case "CEAPB002": //Performance Measures Data
                //    CEAPB002_Report CEAPB002Form = new CEAPB002_Report(this, privilegeEntity);
                //    CEAPB002Form.StartPosition = FormStartPosition.CenterScreen;
                //    CEAPB002Form.ShowDialog();
                //    break;
                #endregion
                #region Ser/Pay Adjustment Report
                //case "CASB0021":    //Ser/Pay Adjustment Report
                //    CASB0021_ReportForm CASB0021Form = new CASB0021_ReportForm(this, privilegeEntity);
                //    CASB0021Form.StartPosition = FormStartPosition.CenterScreen;
                //    CASB0021Form.ShowDialog();
                //    break;
                #endregion
                #region Funding Source Report
                //case "TMSB0034":    //Funding Source Report
                //    TMSB0034_ReportForm tMSB0034_ReportForm = new TMSB0034_ReportForm(this, privilegeEntity);
                //    tMSB0034_ReportForm.StartPosition = FormStartPosition.CenterScreen;
                //    tMSB0034_ReportForm.ShowDialog();
                //    break;
                #endregion
                #region Vendor List
                //case "TMSB6B01":    //Vendor List
                //    TMSB6B01_Report Vendor_list = new TMSB6B01_Report(this, privilegeEntity);
                //    Vendor_list.StartPosition = FormStartPosition.CenterScreen;
                //    Vendor_list.ShowDialog();
                //    break;
                #endregion
                #region Adhoch Report
                case "CASB0012":
                    CASB2012_AdhocForm AdhocForm = new CASB2012_AdhocForm(this, privilegeEntity);
                    AdhocForm.StartPosition = FormStartPosition.CenterScreen;
                    AdhocForm.ShowDialog();
                    break;
                #endregion

                #endregion
                #endregion

                #region Head Start
                #region Screens
                #region Attendance Posting By Site
                //case "HSS00137":    //Attendance Posting By Site
                //    if (!string.IsNullOrEmpty(BaseYear.Trim()))
                //    {
                //        HSS00137Control hss00137control = new HSS00137Control(this, privilegeEntity);
                //        AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, hss00137control);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay("Year Should not be blank for this hierachy in Program Definition");
                //    }
                //    break;
                #endregion
                #region Bus Master/Route Maintenance
                //case "HSS00138":    //Bus Master/Route Maintenance
                //    HSS00138_Control busMaintenance = new HSS00138_Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, busMaintenance);
                //    break;
                #endregion
                #region Bus Client Placement
                //case "HSS00140":    //Bus Client Placement
                //    HSS00140_Control busClient = new HSS00140_Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, busClient);
                //    break;
                #endregion
                #region PIR Control
                //case "PIR00000":    // PIR Control
                //    PIR20000Form PIR20000form = new PIR20000Form(this, privilegeEntity);
                //    PIR20000form.StartPosition = FormStartPosition.CenterScreen;
                //    PIR20000form.ShowDialog();
                //    break;
                #endregion
                #region PIR Logic Assocaition
                //case "PIR00001":
                //    PIR20001Control Pir20001Control = new PIR20001Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Pir20001Control);
                //    break;
                #endregion
                #region Staff Master Maintenance
                //case "STFMST10":
                //    STFMST10Control staffControl = new STFMST10Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, staffControl);
                //    break;
                #endregion
                #region Staff Master Association
                //case "HSS00001":
                //    Hss20001Control hss20001Control = new Hss20001Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, hss20001Control);
                //    break;
                #endregion
                #region Track Master Maintnenace
                //case "HSS00133":
                //    if (!UserProfile.Components.Contains("None"))
                //    {
                //        HSS00133_Control HSS00133 = new HSS00133_Control(this, privilegeEntity);
                //        AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, HSS00133);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay("You can't Access any Component \n Please Contact Your System Administrator");
                //    }
                //    break;
                #endregion
                #region Client Tracking
                //case "HSS00134":
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            if (!UserProfile.Components.Contains("None"))
                //            {
                //                HSS00134Control HSS00134 = new HSS00134Control(this, privilegeEntity);
                //                AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, HSS00134);
                //            }
                //            else
                //            {
                //                CommonFunctions.MessageBoxDisplay("You can't Access any Component \n Please Contact Your System Administrator");
                //            }
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    }
                //    break;

                #endregion
                #region Staff Bulk Posting
                //case "STFBLK10":    //Staff Bulk Posting
                //    STFBLK10Control staffBulkControl = new STFBLK10Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, staffBulkControl);
                //    break;
                #endregion
                #region Non-Custodial parent form
                case "HSS00430":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        if (BaseTopApplSelect == "Y")
                        {
                            HSS00430Form hss00430Form = new HSS00430Form(this, privilegeEntity);
                            hss00430Form.StartPosition = FormStartPosition.CenterScreen;
                            hss00430Form.ShowDialog();
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;
                #endregion
                #region Inkind Maintenance
                //case "CASE0012":
                //    INKIND20_control INKIND20 = new INKIND20_control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, INKIND20);
                //    break;
                #endregion

                #region Enrollment History
                //case "ENRLHIST":
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        _privilegeEntity = privilegeEntity;
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            CASE0010_StatusChange_Form EnrlHist_Form = new CASE0010_StatusChange_Form(this, privilegeEntity);
                //            EnrlHist_Form.StartPosition = FormStartPosition.CenterScreen;
                //            EnrlHist_Form.ShowDialog();
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    break;
                #endregion

                #region Site Schedular
                case "HSS00135":
                    if (!string.IsNullOrEmpty(BaseYear.Trim()))
                    {
                        Site_ScheduleControl sitesch = new Site_ScheduleControl(this, privilegeEntity);
                        AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, sitesch);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay("Year Should not be blank for this hierachy in Program Definition");
                    }
                    break;
                #endregion

                #region Remove HS Rollover
                case "HSS00151":    //Remove HS Rollover
                    if (!string.IsNullOrEmpty(BaseYear.Trim()) && UserID.ToUpper() == "JAKE")
                    {
                        Remove_HS_Rollover Rem_HS_Rover = new Remove_HS_Rollover(this, privilegeEntity);
                        Rem_HS_Rover.StartPosition = FormStartPosition.CenterScreen;
                        Rem_HS_Rover.ShowDialog();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(BaseYear.Trim()) && UserID.ToUpper() == "JAKE")
                            CommonFunctions.MessageBoxDisplay("Year should not be blank for this Hierachy in Program Definition");
                        else
                            AlertBox.Show("Access Denied", MessageBoxIcon.Warning);
                    }
                    break;
                #endregion

                #endregion

                #region Reports
                #region PIR Counting Tool
                //case "HSSB0026":    //PIR Counting Tool
                //    HSSB0026_PIRCounting_From PIR_Counting_Form = new HSSB0026_PIRCounting_From(this, privilegeEntity);
                //    PIR_Counting_Form.StartPosition = FormStartPosition.CenterScreen;
                //    PIR_Counting_Form.ShowDialog();
                //    break;
                #endregion
                #region Child Lists/Counts
                //case "HSSB0100":    //Child Lists/Counts
                //    HSSB2100 ChildLists = new HSSB2100(this, privilegeEntity);
                //    ChildLists.StartPosition = FormStartPosition.CenterScreen;
                //    ChildLists.ShowDialog();
                //    break;
                #endregion
                #region Track File Lists
                //case "HSSB0102":    //Track File Lists
                //    HSSB2102_TrackReport Track_Report = new HSSB2102_TrackReport(this, privilegeEntity);
                //    Track_Report.StartPosition = FormStartPosition.CenterScreen;
                //    Track_Report.ShowDialog();
                //    break;
                #endregion
                #region Select Child Tracking Tasks
                //case "HSSB0103":    //Select Child Tracking Tasks
                //    HSSB2103ReportForm HSSB2103form = new HSSB2103ReportForm(this, privilegeEntity);
                //    HSSB2103form.StartPosition = FormStartPosition.CenterScreen;
                //    HSSB2103form.ShowDialog();
                //    break;
                #endregion
                #region Enrollment Summary (Counts Site/Medical-Enrolled)
                //case "HSSB0104":    //Enrollment Summary (Counts Site/Medical-Enrolled)
                //    HSSB2104 HSSB2104 = new HSSB2104(this, privilegeEntity);
                //    HSSB2104.StartPosition = FormStartPosition.CenterScreen;
                //    HSSB2104.ShowDialog();
                //    break;
                #endregion
                #region Child Track List
                //case "HSSB0106":    //Child Track List
                //    HSSB2106_ChildTrackReport ChildTrack = new HSSB2106_ChildTrackReport(this, privilegeEntity);
                //    ChildTrack.StartPosition = FormStartPosition.CenterScreen;
                //    ChildTrack.ShowDialog();
                //    break;
                #endregion
                #region Attendance Sheets
                //case "HSSB0108":    //Attendance Sheets
                //    HSSB2108ReportForm HSSB2108form = new HSSB2108ReportForm(this, privilegeEntity);
                //    HSSB2108form.StartPosition = FormStartPosition.CenterScreen;
                //    HSSB2108form.ShowDialog();
                //    break;
                #endregion
                #region Detail Attendance
                //case "HSSB0109":    //Detail Attendance
                //    HSSB2109ReportForm HSSB2109form = new HSSB2109ReportForm(this, privilegeEntity);
                //    HSSB2109form.StartPosition = FormStartPosition.CenterScreen;
                //    HSSB2109form.ShowDialog();
                //    break;
                #endregion
                #region Attendance Sheets 2.0
                //case "HSSB2108":    //Attendance Sheets 2.0
                //    HSSB2108ReportFormVer2 HSSB2108form2 = new HSSB2108ReportFormVer2(this, privilegeEntity);
                //    HSSB2108form2.StartPosition = FormStartPosition.CenterScreen;
                //    HSSB2108form2.ShowDialog();
                //    break;
                #endregion
                #region Site Schedule
                //case "HSSB0110":    //Site Schedule
                //    HSSB2110 Class_Report = new HSSB2110(this, privilegeEntity);
                //    Class_Report.StartPosition = FormStartPosition.CenterScreen;
                //    Class_Report.ShowDialog();
                //    break;
                #endregion
                #region Average Daily Attendance
                //case "HSSB0111":    //Average Daily Attendance
                //    HSSB2111ReportForm HSSB2111form = new HSSB2111ReportForm(this, privilegeEntity);
                //    HSSB2111form.StartPosition = FormStartPosition.CenterScreen;
                //    HSSB2111form.ShowDialog();
                //    break;
                #endregion
                #region USDA Meal Reimbursement
                //case "HSSB0112":    //USDA Meal Reimbursement
                //    HSSB2112_Report Hssb2112form = new HSSB2112_Report(this, privilegeEntity);
                //    Hssb2112form.StartPosition = FormStartPosition.CenterScreen;
                //    Hssb2112form.ShowDialog();
                //    break;
                #endregion
                #region Site Track Tasks To Address
                //case "HSSB0114":    //Site Track Tasks To Address
                //    HSSB2114ReportForm Hssb2114form = new HSSB2114ReportForm(this, privilegeEntity);
                //    Hssb2114form.StartPosition = FormStartPosition.CenterScreen;
                //    Hssb2114form.ShowDialog();
                //    break;
                #endregion
                #region Waiting List Report
                //case "HSSB0115":    //Waiting List Report
                //    HSSB0115_WaitingList_Report WaitList = new HSSB0115_WaitingList_Report(this, privilegeEntity);
                //    WaitList.StartPosition = FormStartPosition.CenterScreen;
                //    WaitList.ShowDialog();
                //    break;
                #endregion
                #region Bus Clients
                //case "HSSB0118":    //Bus Lists
                //    HSSB0118_ReportForm BusLists = new HSSB0118_ReportForm(this, privilegeEntity);
                //    BusLists.StartPosition = FormStartPosition.CenterScreen;
                //    BusLists.ShowDialog();
                //    break;
                #endregion
                #region Growth Charts Plus Interface
                //case "HSSB0123":    //Growth Charts Plus Interface
                //    HSSB0123 GrowthCharts = new HSSB0123(this, privilegeEntity);
                //    GrowthCharts.StartPosition = FormStartPosition.CenterScreen;
                //    GrowthCharts.ShowDialog();
                //    break;
                #endregion
                #region Next Year's Preparation
                //case "HSSB0124":    //Next Year's Preparation
                //    HSSB0124_Report HSSB0124 = new HSSB0124_Report(this, privilegeEntity);
                //    HSSB0124.StartPosition = FormStartPosition.CenterScreen;
                //    HSSB0124.ShowDialog();
                //    break;
                #endregion
                #region Matrix New Report
                //case "MATB0003":
                //    MATB0003_Form MATB0003Form = new MATB0003_Form(this, privilegeEntity);
                //    MATB0003Form.StartPosition = FormStartPosition.CenterScreen;
                //    MATB0003Form.ShowDialog();
                //    break;
                #endregion
                #endregion

                #endregion

                #region Energy Assistance-CT
                #region Screens
                //case "TMS00081":
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        _privilegeEntity = privilegeEntity;
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            TMS00081_Control TMS00081 = new TMS00081_Control(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, TMS00081);
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    break;
                //#region MOR Price Update
                //case "TMS00090":    //MOR Price Update
                //    TMS20090 TMS20090_FORM = new TMS20090(this, privilegeEntity);
                //    TMS20090_FORM.StartPosition = FormStartPosition.CenterScreen;
                //    TMS20090_FORM.ShowDialog();
                //    break;
                //#region MyRegion
                //case "TRIGBULK":
                //    Trig_BulkPosting trigbulk = new Trig_BulkPosting(this, privilegeEntity);
                //    trigbulk.StartPosition = FormStartPosition.CenterScreen;
                //    trigbulk.ShowDialog();
                //    break;
                //#endregion
                //#endregion
                //#region LIHWAP Water & Sewage Control Card
                //case "TMS00091":    //LIHWAP Water & Sewage Control Card
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        _privilegeEntity = privilegeEntity;
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            TMS00091Control _tMS00091Control = new TMS00091Control(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, _tMS00091Control);
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    break;
                //#endregion                
                //case "CASE2005":
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            IncomepleteIntakeControl _IncomepleteIntakeControl = new IncomepleteIntakeControl(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, _IncomepleteIntakeControl);

                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    }
                //    break;
                //#region Schedule Appointments
                //case "TMS00110":    //Schedule Appointments
                //    TMS00110Control TMS00110 = new TMS00110Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, TMS00110);
                //    break;
                //#endregion
                //#region Appointment Reserve Schedule
                //case "TMS00120":
                //    TMS00120Control TMS00120 = new TMS00120Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, TMS00120);
                //    break;
                //#endregion
                //#region ABC - Benefit Calculator
                //case "TMS00301":    //ABC - Benefit Calculator
                //    if (!string.IsNullOrEmpty(BaseApplicationNo))
                //    {
                //        if (BaseTopApplSelect == "Y")
                //        {
                //            TMSB0030_ABCcalcControl CASE4006 = new TMSB0030_ABCcalcControl(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CASE4006);
                //        }
                //        else
                //            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                //    }
                //    else
                //    {
                //        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                //    }
                //    break;
                //#endregion
                //#region CT LIHEAP Categorical Eligibility
                //case "TMS00085":    //CT LIHEAP Categorical Eligibility
                //    TMSELIG_Control TMS00085control = new TMSELIG_Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, TMS00085control);
                //    break;
                //#endregion

                //#region DSS XML IMPORT
                //case "TMS00140":
                //    try
                //    {
                //        SqlConnection connect = new SqlConnection();
                //        connect.ConnectionString = this.BaseDSSXMLDBConnString;
                //        connect.Open();
                //        if (connect.State == ConnectionState.Open)
                //        {
                //            connect.Close();
                //            TMS00140_Control Tms00140 = new TMS00140_Control(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Tms00140);
                //        }
                //        else { MessageBox.Show("Connection issue with Server \n Please contact CAPSYSTEMS INC", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                //    }
                //    catch (Exception)
                //    {

                //        MessageBox.Show("Connection issue with Server \n Please contact CAPSYSTEMS INC", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    }
                //    break;
                //#endregion

                //#region DSS intake Waiting Room
                //case "TMS00141":
                //    try
                //    {
                //        SqlConnection connect = new SqlConnection();
                //        connect.ConnectionString = this.BaseDSSXMLDBConnString;
                //        connect.Open();
                //        if (connect.State == ConnectionState.Open)
                //        {
                //            connect.Close();
                //            TMS00141_Control Tms00141 = new TMS00141_Control(this, privilegeEntity);
                //            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Tms00141);
                //        }
                //        else { MessageBox.Show("Connection issue with Server \n Please contact CAPSYSTEMS INC", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                //    }
                //    catch (Exception)
                //    {

                //        MessageBox.Show("Connection issue with Server \n Please contact CAPSYSTEMS INC", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    }
                //    break;
                //#endregion

                #region
                //case "TMS00100":
                //    TMS00100 tms00100 = new TMS00100(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, tms00100);
                //    break;
                #endregion

                #endregion

                #region Reports
                //#region Act Summary/Weekly
                //case "TMSB0000":    //Act Summary/Weekly
                //    TMSB0000_ActSum_Report Act_Report = new TMSB0000_ActSum_Report(this, privilegeEntity);
                //    Act_Report.StartPosition = FormStartPosition.CenterScreen;
                //    Act_Report.ShowDialog();
                //    break;
                //#endregion
                //#region Asset Test Summary
                //case "TMSB0001": //Asset Test Summary
                //    TMSB0001_Report tmsb4001 = new TMSB0001_Report(this, privilegeEntity);
                //    tmsb4001.StartPosition = FormStartPosition.CenterScreen;
                //    tmsb4001.ShowDialog();
                //    break;
                //#endregion
                //#region Fixed Margin Report
                //case "TMSB0002":    //Fixed Margin Report
                //    TMSB0002Form tMSB0002Form = new TMSB0002Form(this, privilegeEntity);
                //    tMSB0002Form.StartPosition = FormStartPosition.CenterScreen;
                //    tMSB0002Form.ShowDialog();
                //    break;
                //#endregion
                //#region Town/Site Counts/List Report
                //case "TMSB0003":    //Town/Site Counts/List Report
                //    TMSB4003 tmsb4003 = new TMSB4003(this, privilegeEntity);
                //    tmsb4003.StartPosition = FormStartPosition.CenterScreen;
                //    tmsb4003.ShowDialog();
                //    break;
                //#endregion
                //#region Vendor Fuel Authorizations
                //case "TMSB0004":    //Vendor Fuel Authorizations
                //    TMSB0004_Report TMSB4004_ReportForm = new TMSB0004_Report(this, privilegeEntity);
                //    TMSB4004_ReportForm.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB4004_ReportForm.ShowDialog();
                //    break;
                //#endregion
                //#region Utility Eligibility Log
                //case "TMSB0005":    //Utility Eligibility Log
                //    TMSB0005_Report TMSB4005_ReportForm = new TMSB0005_Report(this, privilegeEntity);
                //    TMSB4005_ReportForm.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB4005_ReportForm.ShowDialog();
                //    break;
                //#endregion
                //#region Certification/Recert Report
                //case "TMSB0006":    //Certification/Recert Report
                //    TMSB0006_Report TMSB4006_ReportForm = new TMSB0006_Report(this, privilegeEntity);
                //    TMSB4006_ReportForm.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB4006_ReportForm.ShowDialog();
                //    break;
                //#endregion
                //#region Apps Missing Primary Accounts
                //case "TMSB0007":    //Apps Missing Primary Accounts
                //    TMSB4007 tmsb4007 = new TMSB4007(this, privilegeEntity);
                //    tmsb4007.StartPosition = FormStartPosition.CenterScreen;
                //    tmsb4007.ShowDialog();
                //    break;
                //#endregion
                //#region Bundling
                //case "TMSB0010":    //Bundling
                //    TMSB0010_Bundling TMSB0010_ReportForm = new TMSB0010_Bundling(this, privilegeEntity);
                //    TMSB0010_ReportForm.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0010_ReportForm.ShowDialog();
                //    break;
                //#endregion
                //#region Invoice Validation
                //case "TMSB0011":    // Invoice Validation
                //    TMSB0011_Report TMSB4011_ReportForm = new TMSB0011_Report(this, privilegeEntity);
                //    TMSB4011_ReportForm.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB4011_ReportForm.ShowDialog();
                //    break;
                //#endregion
                //#region Utility Hardship Notification
                //case "TMSB0012":    //Utility Hardship Notification
                //    TMSB0012_Report TMSB0012Form = new TMSB0012_Report(this, privilegeEntity);
                //    TMSB0012Form.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0012Form.ShowDialog();
                //    break;
                //#endregion
                //#region SSN Report for DSS
                //case "TMSB0015":    //SSN Report for DSS
                //    TMSB4015_SSNReport TMSB4015 = new TMSB4015_SSNReport(this, privilegeEntity);
                //    TMSB4015.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB4015.ShowDialog();
                //    break;
                //#endregion
                //#region Funding Source Analysis/Update
                //case "TMSB0017": //Funding Source Analysis/Update
                //    TMSB4017 TMS4017_Report = new TMSB4017(this, privilegeEntity);
                //    TMS4017_Report.StartPosition = FormStartPosition.CenterScreen;
                //    TMS4017_Report.ShowDialog();
                //    break;
                //#endregion
                //#region Re-Cal Benefit Utility/Renters
                //case "TMSB0018":    //Re-Cal Benefit Utility/Renters
                //    TMSB0018_ReCalc_Benefit Benefit_report = new TMSB0018_ReCalc_Benefit(this, privilegeEntity);
                //    Benefit_report.StartPosition = FormStartPosition.CenterScreen;
                //    Benefit_report.ShowDialog();
                //    break;
                //#endregion
                //#region Vendor Appl List/Lab
                //case "TMSB0019":    //Vendor Appl List/Lab
                //    TMSB0019Form tMSB0019Form = new TMSB0019Form(this, privilegeEntity);
                //    tMSB0019Form.StartPosition = FormStartPosition.CenterScreen;
                //    tMSB0019Form.ShowDialog();
                //    break;
                //#endregion
                //#region Check Processing
                //case "TMSB0020":    //Check Processing
                //    TMSB0020_CheckProcessing Check_Proc = new TMSB0020_CheckProcessing(this, privilegeEntity);
                //    Check_Proc.StartPosition = FormStartPosition.CenterScreen;
                //    Check_Proc.ShowDialog();
                //    break;
                //#endregion
                //#region XML Data Bridge
                //case "TMSB0022":    //XML Data Bridge
                //    TMSB0022Form tMSB0022Form = new TMSB0022Form(this, privilegeEntity);
                //    tMSB0022Form.StartPosition = FormStartPosition.CenterScreen;
                //    tMSB0022Form.ShowDialog();
                //    break;
                //#endregion
                //#region 
                //case "TMSB0023":
                //    TMSB0023Form tMSB0023Form = new TMSB0023Form(this, privilegeEntity);
                //    tMSB0023Form.StartPosition = FormStartPosition.CenterScreen;
                //    tMSB0023Form.ShowDialog();
                //    break;
                //#endregion
                //#region New XML Data Bridge
                //case "TMSB0024":    //New XML Data Bridge
                //    TMSB0022Form tMSB2022Form = new TMSB0022Form(this, privilegeEntity);
                //    tMSB2022Form.StartPosition = FormStartPosition.CenterScreen;
                //    tMSB2022Form.ShowDialog();
                //    break;
                //#endregion
                //#region ROMA XML Data Bridge
                //case "TMSB3022":    //ROMA XML Data Bridge
                //    TMSB0022Form tMSB3022Form = new TMSB0022Form(this, privilegeEntity);
                //    tMSB3022Form.StartPosition = FormStartPosition.CenterScreen;
                //    tMSB3022Form.ShowDialog();
                //    break;
                //#endregion
                //#region CT Apprise Survey
                //case "TMSB0025":    //CT Apprise Survey
                //    TMSB0025_Report oTMSB0025_Report = new TMSB0025_Report(this, privilegeEntity);
                //    oTMSB0025_Report.StartPosition = FormStartPosition.CenterScreen;
                //    oTMSB0025_Report.ShowDialog();
                //    break;
                //#endregion
                //#region CT Tribeware
                //case "TMSB0026":    //CT Tribeware
                //    TMSB0026_Report TMSB0026_Report = new TMSB0026_Report(this, privilegeEntity);
                //    TMSB0026_Report.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0026_Report.ShowDialog();
                //    break;
                //#endregion
                //#region Denial Reason and Denied second Authorizations
                //case "TMSB0027":    //Denial Reason and Denied second Authorizations
                //    TMSB0027_DeniedReport TMSB0027_Report = new TMSB0027_DeniedReport(this, privilegeEntity);
                //    TMSB0027_Report.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0027_Report.ShowDialog();
                //    break;
                //#endregion
                //#region CEAP Applications by Submission Type
                //case "TMSB0028":    //CEAP Applications by Submission Type
                //    TMSB0028_Report oTMSB0028_Report = new TMSB0028_Report(this, privilegeEntity);
                //    oTMSB0028_Report.StartPosition = FormStartPosition.CenterScreen;
                //    oTMSB0028_Report.ShowDialog();
                //    break;
                //#endregion
                //#region LIHWAP Bundling
                //case "TMSB0030":    //LIHWAP Bundling
                //    TMSB0030_Bundling TMSB0910_ReportForm = new TMSB0030_Bundling(this, privilegeEntity);
                //    TMSB0910_ReportForm.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0910_ReportForm.ShowDialog();
                //    break;
                //#endregion
                //#region LIHWAP Check Processing
                //case "TMSB0031":
                //    TMSB0031_CheckProcessing TMSB0031_ReportForm = new TMSB0031_CheckProcessing(this, privilegeEntity);
                //    TMSB0031_ReportForm.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0031_ReportForm.ShowDialog();
                //    break;
                //#endregion
                //#region LIHWAP Stats Report
                //case "TMSB0032":    //LIHWAP Stats Report
                //    TMSB0032Report TMSB0032_Report = new TMSB0032Report(this, privilegeEntity);
                //    TMSB0032_Report.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0032_Report.ShowDialog();
                //    break;
                //#endregion
                //#region LIHWAP Financial Report
                //case "TMSB0033":    //LIHWAP Financial Report
                //    TMSB0033Report TMSB0033_Report = new TMSB0033Report(this, privilegeEntity);
                //    TMSB0033_Report.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0033_Report.ShowDialog();
                //    break;
                //#endregion
                //#region Incomplete Intakes Report
                //case "TMSB0035":    //Incomplete Intakes Report
                //    TMSB0035Report TMSB0035Report = new TMSB0035Report(this, privilegeEntity);
                //    TMSB0035Report.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0035Report.ShowDialog();
                //    break;
                //#endregion
                //#region DSS Wait Room Report
                //case "TMSB0041":    //DSS Wait Room Report
                //    TMSB0041_Report TMSB0041Report = new TMSB0041_Report(this, privilegeEntity);
                //    TMSB0041Report.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0041Report.ShowDialog();
                //    break;
                //#endregion

                //#region Liheap Rollover Audit
                //case "TMSB0044":    //Liheap Rollover Audit
                //    TMSB0044_Report TMSB0044Form = new TMSB0044_Report(this, privilegeEntity);
                //    TMSB0044Form.StartPosition = FormStartPosition.CenterScreen;
                //    TMSB0044Form.ShowDialog();
                //    break;
                //#endregion
                //#region 11 Day Denial Letter
                //case "TMSB0045":    //11 Day Denial Letter
                //    TMSB0045_Report tmsb0045 = new TMSB0045_Report(this, privilegeEntity);
                //    tmsb0045.StartPosition = FormStartPosition.CenterScreen;
                //    tmsb0045.ShowDialog();
                //    break;
                //#endregion
                //#region Exhausted Benefit Report
                //case "TMSB0046":    //Exhausted Benefit Report
                //    TMSB0046_Report tmsb0046 = new TMSB0046_Report(this, privilegeEntity);
                //    tmsb0046.StartPosition = FormStartPosition.CenterScreen;
                //    tmsb0046.ShowDialog();
                //    break;
                //#endregion
                //#region CEAP Applicant Activity Report
                //case "TMSB0029":    //CEAP Applicant Activity Report
                //    TMSB0029_Report oTMSB0029_Report = new TMSB0029_Report(this, privilegeEntity);
                //    oTMSB0029_Report.StartPosition = FormStartPosition.CenterScreen;
                //    oTMSB0029_Report.ShowDialog();
                //    break;
                //#endregion
                //#region Appointment Schedule
                //case "TMSB0110":    //Appointment Schedule
                //    TMSB0110_Report tmsb0110 = new TMSB0110_Report(this, privilegeEntity);
                //    tmsb0110.StartPosition = FormStartPosition.CenterScreen;
                //    tmsb0110.ShowDialog();
                //    break;
                //#endregion
                //#region Liheap Performance Report
                //case "LPMQ0001":    //Liheap Performance Report
                //    LPMQ0001_Report LPMQ0001Form = new LPMQ0001_Report(this, privilegeEntity);
                //    LPMQ0001Form.StartPosition = FormStartPosition.CenterScreen;
                //    LPMQ0001Form.ShowDialog();
                //    break;
                //#endregion
                //#region Print Applications
                //case "TMSBAPPB":    //Print Applications
                //    TMSBAPPBReport tMSBAPPBReport = new TMSBAPPBReport(this, privilegeEntity);
                //    tMSBAPPBReport.StartPosition = FormStartPosition.CenterScreen;
                //    tMSBAPPBReport.ShowDialog();
                //    break;
                //#endregion
                //#region Benefit Records Fix
                //case "TMSBPFIX":    //Benefit Records Fix
                //    TMSBPFIX _tmsbpfix = new TMSBPFIX(this, privilegeEntity);
                //    _tmsbpfix.StartPosition = FormStartPosition.CenterScreen;
                //    _tmsbpfix.ShowDialog();
                //    break;
                //#endregion
                //#region Print Letters-GUI
                //case "TMSBLTRB":    //Print Letters-GUI
                //    TMSBLTRB tMSBLTRBReport = new TMSBLTRB(this, privilegeEntity);
                //    tMSBLTRBReport.StartPosition = FormStartPosition.CenterScreen;
                //    tMSBLTRBReport.ShowDialog();
                //    break;
                //#endregion
                //#region Create Cash/Award Files
                //case "TMSBCHCT": //Create Cash/Award Files
                //    TMSBCHCT TMSBChCt_ReportForm = new TMSBCHCT(this, privilegeEntity);
                //    TMSBChCt_ReportForm.StartPosition = FormStartPosition.CenterScreen;
                //    TMSBChCt_ReportForm.ShowDialog();
                //    break;
                //#endregion
                #endregion
                #endregion

                //#region Dashboard
                //case "RPREPOR4": //Report :: dashboard agency wide data entry activity
                //    DASHBOARD_Form RPREPOR4Form = new DASHBOARD_Form(this, privilegeEntity);
                //    RPREPOR4Form.StartPosition = FormStartPosition.CenterScreen;
                //    RPREPOR4Form.ShowDialog();
                //    break;
                //#endregion

                default:
                    HelpForm2 helpForm = new HelpForm2();
                    helpForm.Text = privilegeEntity.PrivilegeName;
                    helpForm.StartPosition = FormStartPosition.CenterScreen;
                    //FrmUploadFtp helpForm = new FrmUploadFtp();
                    helpForm.ShowDialog();
                    Consts.Messages.UserCreatedSuccesssfully.DisplayFirendlyMessage(Captain.Common.Exceptions.ExceptionSeverityLevel.Information);
                    Ishelp = "Y";
                    break;
            }

            if (privilegeEntity.screenType == "SCREEN" && privilegeEntity.Program != "ADMN0012")
            {
                if (Ishelp == "N" && isForm == "N")
                    btncloseAll.Visible = true;
            }



            #region commented menu code
            //  PrivilegeEntity privilegeEntity = _Code as PrivilegeEntity;
            //if (privilegeEntity == null) return;
            /*
            switch (privilegeEntity.Program.Trim())
            {
                case "ADMN0005": //Users
                    UserListControl ulc = new UserListControl(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ulc);
                    break;
                case "ADMN0006": //Agency Table Codes
                    AgencyTableControl agencyTabelControl = new AgencyTableControl(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, agencyTabelControl);
                    break;
                case "ADMN0010": //Master Poverty Guidelines 
                    //MasterPovertyGuidelinesForm masterPovertyForm = new MasterPovertyGuidelinesForm(this, privilegeEntity);
                    //masterPovertyForm.ShowDialog();                      
                    MasterPoverityGuidelineControl masterPovertyControl = new MasterPoverityGuidelineControl(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, masterPovertyControl);
                    break;
                case "ADMN0013": //Zipcode
                    ADMN0013 ADMN0013 = new ADMN0013(this, privilegeEntity);
                    ////AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, zipCodeControl);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0013);
                    break;
                case "CASE2002":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        _privilegeEntity = privilegeEntity;
                        if (BaseTopApplSelect == "Y")
                        {
                            Case3001Control clientIntakeControl = new Case3001Control(this, privilegeEntity);
                            // ClientIntakeControl clientIntakeControl = new ClientIntakeControl(this, privilegeEntity);
                            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, clientIntakeControl);
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;
                case "TMSB4016":
                    TMSB4601 TMSBForm = new TMSB4601();
                    TMSBForm.ShowDialog();
                    break;
                case "CASE0008":
                    FLDCNTLMaintenanceControl FLDControl = new FLDCNTLMaintenanceControl(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, FLDControl);
                    break;
                case "REPMNT20":
                    PdfListForm pdfListForm = new PdfListForm(this, privilegeEntity, true);
                    pdfListForm.ShowDialog();
                    break;
                case "TMS00300":
                    TMS00300Control TMS00300 = new TMS00300Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, TMS00300);
                    break;
               
                case "ADMNB001":
                    ADMNB001 ADMNB001Form = new ADMNB001(this, privilegeEntity);
                    ADMNB001Form.ShowDialog();
                    break;
                case "TMS00310":
                    TMS00310Control TMS00310 = new TMS00310Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, TMS00310);
                    break;
                case "CASB2012":
                    CASB2012Control CASB2012 = new CASB2012Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CASB2012);
                    break;
               
               
                
                case "CASE0009":
                    Case0009Control Case0009 = new Case0009Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Case0009);
                    break;
                case "MAT00001":
                    MAT00001Control MAT00001 = new MAT00001Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, MAT00001);
                    break;
                case "MAT00002":
                    MAT00002Control MAT00002 = new MAT00002Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, MAT00002);
                    break;
   
                case "CASB0004":
                    CASB0004Form Casb0004Form = new CASB0004Form(this, privilegeEntity); ;
                    Casb0004Form.ShowDialog();
                    break;
                
                
               
              


               
                case "STFBLK10":
                    STFBLK10Control staffBulkControl = new STFBLK10Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, staffBulkControl);
                    break;          
                case "MATB0002":
                    MATB0002_Form MATB0002Form = new MATB0002_Form(this, privilegeEntity); ;
                    MATB0002Form.ShowDialog();
                    break;
              

                case "HSSB0026":
                    HSSB0026_PIRCounting_From PIR_Counting_Form = new HSSB0026_PIRCounting_From(this, privilegeEntity); ;
                    PIR_Counting_Form.ShowDialog();

                    break;
                case "HSSB0123":
                    HSSB0123 GrowthCharts = new HSSB0123(this, privilegeEntity);
                    GrowthCharts.ShowDialog();
                    break;
                case "ARS00120":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        _privilegeEntity = privilegeEntity;
                        if (BaseTopApplSelect == "Y")
                        {
                            ARS20120 ARS20120 = new ARS20120(this, privilegeEntity);
                            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ARS20120);
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;
                case "ARS00115":
                    Ars20115Control ARS20115control = new Ars20115Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ARS20115control);
                    break;
                case "ARSB0150":
                    ARSB2150_ReportForm CustStatement = new ARSB2150_ReportForm(this, privilegeEntity);
                    CustStatement.ShowDialog();
                    break;
                case "ARSB0140":
                    ARSB2140_ReportForm custReport = new ARSB2140_ReportForm(this, privilegeEntity);
                    custReport.ShowDialog();
                    break;
                case "ARSB0160":
                    ARSB2160_Report invoice = new ARSB2160_Report(this, privilegeEntity);
                    invoice.ShowDialog();
                    break;
                case "ARSB0120":
                    ARSB2120_ReportForm Billing = new ARSB2120_ReportForm(this, privilegeEntity);
                    Billing.ShowDialog();
                    break;
                case "TMSBLTRB":
                    TMSBLTRB tMSBLTRBReport = new TMSBLTRB(this, privilegeEntity);
                    tMSBLTRBReport.ShowDialog();
                    break;         
                case "ARS00130":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        _privilegeEntity = privilegeEntity;
                        if (BaseTopApplSelect == "Y")
                        {
                            Ars20130Control ARS20130 = new Ars20130Control(this, privilegeEntity);
                            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ARS20130);
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;
                case "EMS00010":
                    EMS00010Control Ems30010control = new EMS00010Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Ems30010control);
                    break;

                case "EMS00020":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        _privilegeEntity = privilegeEntity;
                        if (BaseTopApplSelect == "Y")
                        {
                            EMS00020Control ems00020Control = new EMS00020Control(this, privilegeEntity);
                            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ems00020Control);
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;
                case "ADMN0021":
                    Admn0021Form admn0021form = new Admn0021Form(this, privilegeEntity);
                    admn0021form.ShowDialog();
                    break;
                case "EMSB0014":
                    EMSB0014_BudgetReport Budget_report = new EMSB0014_BudgetReport(this, privilegeEntity);
                    Budget_report.ShowDialog();
                    break;
                case "EMSB0003":
                    EMSB0003_PendingCases Pending_report = new EMSB0003_PendingCases(this, privilegeEntity);
                    Pending_report.ShowDialog();
                    break;
                case "EMSB0007":
                    EMSB0007_NoInvoiceReport NoInvoice_report = new EMSB0007_NoInvoiceReport(this, privilegeEntity);
                    NoInvoice_report.ShowDialog();
                    break;
                case "EMSB0010":
                    EMSB0010_FollowUp FolowUp_report = new EMSB0010_FollowUp(this, privilegeEntity);
                    FolowUp_report.ShowDialog();
                    break;
                case "EMSB0012":
                    EMSB0012_Report DeptRej_report = new EMSB0012_Report(this, privilegeEntity);
                    DeptRej_report.ShowDialog();
                    break;
                case "EMSB0018":
                    EMS0018_Report ReFHs_report = new EMS0018_Report(this, privilegeEntity);
                    ReFHs_report.ShowDialog();
                    break;
                case "EMSB0011":
                    EMSB0011_PaidInvoices Paidinvoices_report = new EMSB0011_PaidInvoices(this, privilegeEntity);
                    Paidinvoices_report.ShowDialog();
                    break;
                case "EMSB0017":
                    EMSB3017_Zipcode ZipCode_report = new EMSB3017_Zipcode(this, privilegeEntity);
                    ZipCode_report.ShowDialog();
                    break;
                case "EMS00040":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        _privilegeEntity = privilegeEntity;
                        if (BaseTopApplSelect == "Y")
                        {
                            EMS00040Control ems00040Control = new EMS00040Control(this, privilegeEntity);
                            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ems00040Control);
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;

              

                case "EMS00030":
                    EMS00030Control Ems30030control = new EMS00030Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Ems30030control);
                    break;

                case "EMSB0008":
                    EMSB0008_Presets Presets_report = new EMSB0008_Presets(this, privilegeEntity);
                    Presets_report.ShowDialog();
                    break;

                case "EMSB0021":
                    EMSB0021_ActivityReport EMSB0021Form = new EMSB0021_ActivityReport(this, privilegeEntity);
                    EMSB0021Form.ShowDialog();
                    break;

                case "ARSB0130":
                    ARSB2130_ReportForm Daycareexclusion = new ARSB2130_ReportForm(this, privilegeEntity);
                    Daycareexclusion.ShowDialog();
                    break;
                case "FIXCLINT":
                    FIXCLIENTForm fixClientForm = new FIXCLIENTForm(this, privilegeEntity);
                    fixClientForm.ShowDialog();
                    break;
                    break;
                //case "FIXFAMID":
                //    FIXFAMILYIDForm2 fixfamidForm2 = new FIXFAMILYIDForm2(this, privilegeEntity, string.Empty);
                //    fixfamidForm2.ShowDialog();
                //    break;
                case "EMSB0024":
                    EMSB0024_ReportForm EMSB0024Form = new EMSB0024_ReportForm(this, privilegeEntity);
                    EMSB0024Form.ShowDialog();
                    break;
                case "EMSB0004":
                    EMSB0004_ReportForm EMSB0004Form = new EMSB0004_ReportForm(this, privilegeEntity);
                    EMSB0004Form.ShowDialog();
                    break;
                case "EMSB0026":
                    EMSB0026_SweepResources EMSB3026Form = new EMSB0026_SweepResources(this, privilegeEntity);
                    EMSB3026Form.ShowDialog();
                    break;
                case "EMSB0009":
                    EMSB0009_Report EMSB3009Form = new EMSB0009_Report(this, privilegeEntity);
                    EMSB3009Form.ShowDialog();
                    break;
                case "EMSB0015":
                    EMSB0015_ReportForm EMSB3015Form = new EMSB0015_ReportForm(this, privilegeEntity);
                    EMSB3015Form.ShowDialog();
                    break;
                case "EMSB0002":
                    EMSB0002_Report EMSB3002Form = new EMSB0002_Report(this, privilegeEntity);
                    EMSB3002Form.ShowDialog();
                    break;
                case "EMSB0001":
                    EMSB0001_Report EMSB3001Form = new EMSB0001_Report(this, privilegeEntity);
                    EMSB3001Form.ShowDialog();
                    break;
                case "EMSB0025":
                    EMSB0025_Report EMSB3025Form = new EMSB0025_Report(this, privilegeEntity);
                    EMSB3025Form.ShowDialog();
                    break;
                case "TOOL0001":
                    TOOL0001_ChangeReport SITEForm = new TOOL0001_ChangeReport(this, privilegeEntity);
                    SITEForm.ShowDialog();
                    break;
                case "EMSB0023":
                    EMSB0023_Report EMSB3023Form = new EMSB0023_Report(this, privilegeEntity);
                    EMSB3023Form.ShowDialog();
                    break;
                case "EMSB0006":
                    EMSB0006_Report EMSB3006Form = new EMSB0006_Report(this, privilegeEntity);
                    EMSB3006Form.ShowDialog();
                    break;

                case "ARSB0170":
                    ARSB2170_ReportForm ARSb0170Form = new ARSB2170_ReportForm(this, privilegeEntity);
                    ARSb0170Form.ShowDialog();
                    break;
                case "EMSB0027":
                    EMSB0027_Report EMSB3027Form = new EMSB0027_Report(this, privilegeEntity);
                    EMSB3027Form.ShowDialog();
                    break;

                case "ARS00125":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        if (BaseTopApplSelect == "Y")
                        {
                            List<ARSCUSTEntity> CustDetails = _model.ArsData.Browse_ARSCUST(BaseAgency, BaseDept, BaseProg, BaseApplicationNo.Trim(), string.Empty);
                            if (CustDetails.Count > 0)
                            {
                                ARS00125Form ars0012form = new ARS00125Form(this, privilegeEntity);
                                ars0012form.ShowDialog();
                            }
                            else MessageBox.Show("Customer not found in ARSCUST", "ARS00125");
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;
            
                case "RPMEMBER":
                    ReportControl reportControl = new ReportControl(this, privilegeEntity, string.Empty);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, reportControl);
                    break;
                case "RPINTAKE":
                    ReportControl RPINTAKE = new ReportControl(this, privilegeEntity, "RPINTAKE");
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, RPINTAKE);
                    break;
                case "RPREPORT":
                    ReportControl RPREPORT = new ReportControl(this, privilegeEntity, "RPREPORT");
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, RPREPORT);
                    break;
                case "RPREPOR1":
                    ReportControl RPREPORT1 = new ReportControl(this, privilegeEntity, "RPREPORT1");
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, RPREPORT1);
                    break;
                case "RPREPOR2":
                    ReportGridControl RPREPORT2 = new ReportGridControl(this, privilegeEntity, "RPREPORT2");
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, RPREPORT2);
                    break;
                
                case "AGYXML":
                    TMSB00AGYTABFORM AGYXML = new TMSB00AGYTABFORM(this, privilegeEntity);
                    AGYXML.ShowDialog();
                    break;
                case "EMSB0028":
                    EMSB0028_Report EMS0028Form = new EMSB0028_Report(this, privilegeEntity);
                    EMS0028Form.ShowDialog();
                    break;
                case "EMSB2028":
                    EMSB2028_Report EMS2028Form = new EMSB2028_Report(this, privilegeEntity);
                    EMS2028Form.ShowDialog();
                    break;
                case "EMSB0029":
                    EMSB0029_Report EMS0029Form = new EMSB0029_Report(this, privilegeEntity);
                    EMS0029Form.ShowDialog();
                    break;
                case "ADMNB003":
                    ADMNB003 ADMNB003Form = new ADMNB003(this, privilegeEntity);
                    ADMNB003Form.ShowDialog();
                    break;
                case "HSSB0150":
                    HSSB0150_Report HSSB00150Form = new HSSB0150_Report(this, privilegeEntity);
                    HSSB00150Form.ShowDialog();
                    break;
                case "EMSUNLOK":
                    EmsUnlockScreen EmsUnlockScreenform = new EmsUnlockScreen(this, privilegeEntity);
                    EmsUnlockScreenform.ShowDialog();
                    break;
                case "HLS00133":
                    if (!UserProfile.Components.Contains("None"))
                    {
                        HLS00133_Control HLS00133 = new HLS00133_Control(this, privilegeEntity);
                        AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, HLS00133);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay("You can't Access any Component \n Please Contact Your System Administrator");
                    }
                    break;
                case "RNG00001":
                    RNG00001 RNG00001 = new RNG00001(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, RNG00001);
                    break;
                case "HLS00134":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        if (BaseTopApplSelect == "Y")
                        {
                            if (!UserProfile.Components.Contains("None"))
                            {
                                HLS00134Control HSS00134 = new HLS00134Control(this, privilegeEntity);
                                AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, HSS00134);
                            }
                            else
                            {
                                CommonFunctions.MessageBoxDisplay("You can't Access any Component \n Please Contact Your System Administrator");
                            }
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;
                case "MERGFILE":
                    PdfListForm pdfMergeListForm = new PdfListForm(this);
                    pdfMergeListForm.ShowDialog();
                    break;
                case "ADMNB004":
                    ADMNB004 ADMNB004Form = new ADMNB004(this, privilegeEntity);
                    ADMNB004Form.ShowDialog();
                    break;

                case "HLSB0001":
                    HLSB0001_Report HLSB0001Form = new HLSB0001_Report(this, privilegeEntity);
                    HLSB0001Form.ShowDialog();
                    break;
                case "AGYXMLRG":
                    TMSB00AGYTABFORM AGRNGXML = new TMSB00AGYTABFORM(this, privilegeEntity);
                    AGRNGXML.ShowDialog();
                    break;
                case "CASE0005":
                    CASE0006CLOSESCREEN CASE0006CLOSESCREENFrom = new CASE0006CLOSESCREEN(this, privilegeEntity);
                    CASE0006CLOSESCREENFrom.ShowDialog();
                    break;

                
                case "CUST0001":
                    if (BaseAgencyControlDetails.SpanishSwitch == "Y")
                    {
                        SpanishCustomQuestions spanishForm = new SpanishCustomQuestions(this, privilegeEntity, string.Empty, "CUSTOM");
                        spanishForm.ShowDialog();
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay("AGENCY CONTROL SPANISH SWITCH NOT DEFINED");
                    }
                    break;
                
                case "LAGY0001":
                    if (BaseAgencyControlDetails.SpanishSwitch == "Y")
                    {
                        SpanishCustomQuestions spanishForm = new SpanishCustomQuestions(this, privilegeEntity, string.Empty, "LEANAGYTABS");
                        spanishForm.ShowDialog();
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay("AGENCY CONTROL SPANISH SWITCH NOT DEFINED");
                    }
                    break;
                case "CASEHIE1":
                    SpanishCustomQuestions spanishhieForm = new SpanishCustomQuestions(this, privilegeEntity, string.Empty, "CASEHIE");
                    spanishhieForm.ShowDialog();

                    break;
                case "CAMAST01":
                    SpanishCustomQuestions spanishCAMASTForm = new SpanishCustomQuestions(this, privilegeEntity, string.Empty, "CAMAST");
                    spanishCAMASTForm.ShowDialog();
                    break;
                case "LEANMESS":
                    PIPEmailForm pipEmailForm = new PIPEmailForm(this, privilegeEntity);
                    pipEmailForm.ShowDialog();
                    break;
             
                case "PIPB0005":
                    PIPB0005 PIPB005Form = new PIPB0005(this, privilegeEntity);
                    PIPB005Form.ShowDialog();
                    break;
                //case "CASE0023":
                //    VouchGen_Control VouchControl = new VouchGen_Control(this, privilegeEntity);
                //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, VouchControl);
                //    break;
                case "ADMN0023":
                    VoucherDefinitionControl VouchDefControl = new VoucherDefinitionControl(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, VouchDefControl);
                    break;
                case "ADMN0022":
                    ADMN0022Control ADMN0022 = new ADMN0022Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0022);
                    break;
                case "TMS10100":
                    TMS10100 tms10100 = new TMS10100(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, tms10100);
                    break;
                
                case "TMS10110":
                    TMS10110Control TMS10110 = new TMS10110Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, TMS10110);
                    break;
                case "TMS10120":
                    TMS00120Control TMS10120 = new TMS00120Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, TMS10120);
                    break;
                case "ADMN0014":
                    ADMN0014SettingsForm admn0014Settings = new ADMN0014SettingsForm(this, privilegeEntity); ;
                    admn0014Settings.ShowDialog();
                    break;
                case "CASE0013":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        if (BaseTopApplSelect == "Y")
                        {
                            CASE0013Control CASE0013 = new CASE0013Control(this, privilegeEntity);
                            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CASE0013);
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;
                case "EMS00050":
                    EMS00050Control Ems30050control = new EMS00050Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Ems30050control);
                    break;
                case "ADMN0024":
                    ADMN0024Control Admn0024control = new ADMN0024Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, Admn0024control);
                    break;
                case "PIP00001":
                    if (!string.IsNullOrEmpty(BaseApplicationNo))
                    {
                        if (BaseTopApplSelect == "Y")
                        {
                            PIP00001Form _PIP00001Form = new PIP00001Form(this, privilegeEntity, this.BaseAgency, this.BaseDept, this.BaseProg, this.BaseYear, this.BaseApplicationNo, string.Empty, string.Empty);
                            _PIP00001Form.ShowDialog();
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;
                case "CASE0025":
                    if (UserProfile.EMS_Access == "S" || UserProfile.EMS_Access == "D")
                    {
                        if (BaseAgencyControlDetails.PaymentCategorieService == "Y")
                        {
                            ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseAgency, BaseDept, BaseProg);

                            if (programEntity != null)
                            {
                                if (programEntity.DepSerpostPAYCAT != string.Empty)
                                {
                                    CASE0025Control _case0025control = new CASE0025Control(this, privilegeEntity);
                                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, _case0025control);
                                }
                                else
                                {
                                    CommonFunctions.MessageBoxDisplay("This Program is not set for Payment Category");
                                }
                            }
                        }
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay("You are not Authorized to Operate this Screen. Contact Your Administrator");
                    }
                    break;
                case "CASB0015":

                    if (UserProfile.EMS_Access == "S" || UserProfile.EMS_Access == "D")
                    {
                        CASB0015_Form CASB0015_Report = new CASB0015_Form(this, privilegeEntity);
                        CASB0015_Report.ShowDialog();
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay("You are not Authorized to Operate this Report. Contact Your Administrator");
                    }
                    break;
                case "CASBLTRB":

                    if (UserProfile.EMS_Access == "S" || UserProfile.EMS_Access == "D")
                    {
                        CASBLTRB CASBLTRB_Report = new CASBLTRB(this, privilegeEntity);
                        CASBLTRB_Report.ShowDialog();
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay("You are not Authorized to Operate this Report. Contact Your Administrator");
                    }
                    break;
                case "EMSB1009":
                    EMSB1009_Report EMSB1009Form = new EMSB1009_Report(this, privilegeEntity);
                    EMSB1009Form.ShowDialog();
                    break;

                case "APGT0001":
                    if (!string.IsNullOrEmpty(BaseAgencyNo))
                    {
                        if (BaseTopApplSelect == "Y")
                        {
                            APGT0001_Control CASE4006 = new APGT0001_Control(this, privilegeEntity);
                            AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, CASE4006);
                        }
                        else
                            CommonFunctions.MessageBoxDisplay(Consts.Messages.AgencyCodeSelectionMsg);
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.Applicantdoesntexist);
                    }
                    break;

                case "ADMN0017":
                    AGCYQUES_Control ADMN0017 = new AGCYQUES_Control(this, privilegeEntity);
                    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, ADMN0017);
                    break;
                case "RNGS1014":
                    RNGS1014 RNGS1014 = new RNGS1014(this, privilegeEntity); ;
                    RNGS1014.ShowDialog();
                    break;
                case "CASB0018":
                    CASB0018_Report casb0018form = new CASB0018_Report(this, privilegeEntity);
                    casb0018form.ShowDialog();
                    break;
                case "RNGS2014":
                    RNGS2014 RNGS2014 = new RNGS2014(this, privilegeEntity); ;
                    RNGS2014.ShowDialog();
                    break;
                default:
                    HelpForm2 helpForm = new HelpForm2();
                    //FrmUploadFtp helpForm = new FrmUploadFtp();
                    helpForm.ShowDialog();
                    Consts.Messages.UserCreatedSuccesssfully.DisplayFirendlyMessage(Captain.Common.Exceptions.ExceptionSeverityLevel.Information);
                    break;
            }

            */
            #endregion
        }

        private void pbtnChangePassword_Click(object sender, EventArgs e)
        {
            ChangePassword changePassword = new ChangePassword(this);
            changePassword.StartPosition = FormStartPosition.CenterScreen;
            changePassword.ShowDialog();
        }

        private void pHIEFilter_Click(object sender, EventArgs e)
        {
            if (BusinessModuleID == "01")
            {

                AdminHierarchySelection hierarchieSelectionForm = new AdminHierarchySelection(this);
                hierarchieSelectionForm.StartPosition = FormStartPosition.CenterScreen;
                hierarchieSelectionForm.FormClosed += new FormClosedEventHandler(OnDefaultAgencyFormClosed);
                hierarchieSelectionForm.ShowDialog();
            }
            else
            {
                strcurrentHIE = BaseAgency + "-" + BaseDept + "-" + BaseProg; strYear = BaseYear;
                HierarchieSelection hierarchieSelectionForm = new HierarchieSelection(this, strcurrentHIE, "Master", string.Empty, "A", "I", UserID, "Master");
                hierarchieSelectionForm.StartPosition = FormStartPosition.CenterScreen;
                hierarchieSelectionForm.FormClosed += new FormClosedEventHandler(OnDefaultHierarchieFormClosed);
                hierarchieSelectionForm.ShowDialog();
            }
        }

        string strcurrentHIE = ""; string strYear = "";
        private void OnDefaultAgencyFormClosed(object sender, FormClosedEventArgs e)
        {
            AdminHierarchySelection form = sender as AdminHierarchySelection;

            if (form.DialogResult == DialogResult.OK)
            {
                string Agency = string.Empty;
                Agency = form.SelectedAgency();

                if (BusinessModuleID == "01")
                {
                    _statAdminAgency = Agency;
                    BaseAdminAgency = _statAdminAgency;
                }
                else
                {
                    if (!string.IsNullOrEmpty(Agency.Trim()))
                        BaseAdminAgency = Agency;
                }
                pnlCenterbar.Visible = true;
                pnlSearchbox.Visible = false;
                pHIEFilter.Visible = true;
                string strAgencyName = BaseAdminAgency + " - " + _model.lookupDataAccess.GetHierachyDescription("1", BaseAdminAgency, BaseDept, BaseProg);
                if (BaseAdminAgency == "**") strAgencyName = BaseAdminAgency + " - " + "All Agencies";

                hierachyNamecontrol.lblHierchy.Text = strAgencyName;//CommonFunctions.GetHTMLHierachyFormat(strAgencyName, string.Empty, string.Empty, string.Empty, "01");

                pnlCenterbar.Controls.Clear();
                hierachyNamecontrol.Dock = Wisej.Web.DockStyle.Fill;
                pnlCenterbar.Controls.Add(hierachyNamecontrol);

                OnContentTabsSelectedIndexChanged(ContentTabs, new EventArgs());
            }
        }

        private void OnDefaultHierarchieFormClosed(object sender, FormClosedEventArgs e)
        {
            HierarchieSelection form = sender as HierarchieSelection;
            string strPublicCode = string.Empty;
            if (form.DialogResult == DialogResult.OK)
            {
                List<HierarchyEntity> selectedHierarchies = form.SelectedHierarchiesWithYear;
                string StrYear = form.SelectedHierarchyYear();
                if (selectedHierarchies.Count > 0)
                {
                    string hierarchy = string.Empty;
                    foreach (HierarchyEntity row in selectedHierarchies)
                    {
                        strPublicCode = row.Code;
                        hierarchy += row.Agency + row.Dept + row.Prog;
                    }
                    //txtDefaultHierachy.Text = strPublicCode;
                    BaseAgency = selectedHierarchies[0].Agency;
                    BaseDept = selectedHierarchies[0].Dept;
                    BaseProg = selectedHierarchies[0].Prog;

                    if (!string.IsNullOrEmpty(StrYear.Trim()))
                    {
                        BaseYear = StrYear;
                    }
                    else
                    {
                        ProgramDefinitionEntity programEntity = _model.HierarchyAndPrograms.GetCaseDepadp(BaseAgency, BaseDept, BaseProg);
                        if (programEntity != null)
                        {
                            BaseYear = programEntity.DepYear.Trim() == string.Empty ? "    " : programEntity.DepYear;
                        }
                    }

                    BaseAdminAgency = selectedHierarchies[0].Agency;

                    MainMenuControl MainMenu_Control = this.GetBaseUserControlMainMenu() as MainMenuControl;
                    if (MainMenu_Control != null)
                    {
                        applicationNameControl.Btn_First.Visible = applicationNameControl.BtnP10.Visible = applicationNameControl.BtnPrev.Visible =
                               applicationNameControl.BtnNxt.Visible = applicationNameControl.BtnN10.Visible = applicationNameControl.BtnLast.Visible = true;

                        MainMenu_Control.Set_DefHie_as_BaseHie(BaseAgency, BaseDept, BaseProg, BaseYear);


                    }

                    /*** Refresh the Opened Tabs in Other modules ***/
                    //RefreshOpenedAdminPages();
                    /************************************************/

                    if (strcurrentHIE + strYear != BaseAgency + "-" + BaseDept + "-" + BaseProg + BaseYear)
                    {
                        ContentTabs.TabPages.Clear();
                        strcurrentModuleID = BusinessModuleID;
                        /*** Refresh the Menu Tree ***/
                        frmHiEScren = "Y";
                        BuildMenuTree();
                        /************************************************/

                        /*** Close All Tabs ***/
                        btncloseAll_Click(sender, e);

                        /************************************************/
                    }
                    else
                    {
                        if (strcurrentHIE + strYear == BaseAgency + "-" + BaseDept + "-" + BaseProg + BaseYear) 
                        {
                            if (BaseDefaultHIE != strcurrentHIE) {
                                ContentTabs.TabPages.Clear();
                                strcurrentModuleID = BusinessModuleID;
                                /*** Refresh the Menu Tree ***/
                                frmHiEScren = "Y";
                                BuildMenuTree();
                                /************************************************/

                                /*** Close All Tabs ***/
                                btncloseAll_Click(sender, e);

                            }


                        }
                    }



                }

                //for (int x = 0; x < NavTabs.Items.Count; x++)
                //{
                //    NavTabs.Items[x].Expanded = false;
                //}
            }
        }

        //public string strcurrentModuleID = "";
        private void RefreshOpenedAdminPages()
        {
            BaseUserControl MainMenu_Sub_Open_Control = GetBaseUserControl();
            if (MainMenu_Sub_Open_Control != null)
            {
                /******************************** Case Managament Module Screens Refresh Functions ***********************************************/

                //if (MainMenu_Sub_Open_Control is TMS20100)
                //{
                //    (MainMenu_Sub_Open_Control as TMS20100).Refresh();
                //}
                //if (MainMenu_Sub_Open_Control is APPT0002Control)
                //{
                //    (MainMenu_Sub_Open_Control as APPT0002Control).RefreshOpenScreen();
                //}
                //if (MainMenu_Sub_Open_Control is APPT0003Control)
                //{
                //    (MainMenu_Sub_Open_Control as APPT0003Control).RefreshOpenScreen();
                //}

                /*******************************************************************************************************************************/
                if (MainMenu_Sub_Open_Control is MainMenuControl)
                {
                    (MainMenu_Sub_Open_Control as MainMenuControl).RefreshMainMenu();
                }

                if (MainMenu_Sub_Open_Control is Case3001Control)
                {
                    (MainMenu_Sub_Open_Control as Case3001Control).Refresh();
                }

                //if (MainMenu_Sub_Open_Control is ADMN0022Control)
                //{
                //    (MainMenu_Sub_Open_Control as ADMN0022Control).Getsaldefdata(string.Empty);
                //}

                //if (MainMenu_Sub_Open_Control is Case0009Control)
                //{
                //    (MainMenu_Sub_Open_Control as Case0009Control).fillGrid();
                //}

                //if (MainMenu_Sub_Open_Control is ADMN0015Control)
                //{
                //    (MainMenu_Sub_Open_Control as ADMN0015Control).fillHieGrid(string.Empty);
                //}

                //if (MainMenu_Sub_Open_Control is ADMN0020control)
                //{
                //    (MainMenu_Sub_Open_Control as ADMN0020control).Refresh();
                //}

                if (MainMenu_Sub_Open_Control is HierarchyDefinitionControl)
                {
                    (MainMenu_Sub_Open_Control as HierarchyDefinitionControl).populateGridData(BaseAdminAgency);
                }

                //if (MainMenu_Sub_Open_Control is ProgramDefinition)
                //{
                //    ProgramDefinitionEntity programEntity = new ProgramDefinitionEntity();
                //    (MainMenu_Sub_Open_Control as ProgramDefinition).RefreshGrid(programEntity);
                //}

                if (MainMenu_Sub_Open_Control is UserListControl)
                {
                    (MainMenu_Sub_Open_Control as UserListControl).RefreshGrid(string.Empty);
                }

                //if (MainMenu_Sub_Open_Control is CriticalActivity)
                //{
                //    (MainMenu_Sub_Open_Control as CriticalActivity).FormLoad();
                //}

                //if (MainMenu_Sub_Open_Control is FLDCNTLMaintenanceControl)
                //{
                //    (MainMenu_Sub_Open_Control as FLDCNTLMaintenanceControl).Refreshdata();
                //}

                if (MainMenu_Sub_Open_Control is AdminScreenControls)
                {
                    (MainMenu_Sub_Open_Control as AdminScreenControls).Refreshdata();
                }

                //if (MainMenu_Sub_Open_Control is CASE4006Control)
                //    (MainMenu_Sub_Open_Control as CASE4006Control).Refresh();

                //if (MainMenu_Sub_Open_Control is CASE5006Control)
                //    (MainMenu_Sub_Open_Control as CASE5006Control).Refresh();

                //if (MainMenu_Sub_Open_Control is MAT00001Control)
                //    (MainMenu_Sub_Open_Control as MAT00001Control).Refresh();

                //if (MainMenu_Sub_Open_Control is MAT00002Control)
                //    (MainMenu_Sub_Open_Control as MAT00002Control).Refresh();

                //if (MainMenu_Sub_Open_Control is MAT00003Control)
                //    (MainMenu_Sub_Open_Control as MAT00003Control).RefreshForm();

                //if (MainMenu_Sub_Open_Control is CASE0016_Control)
                //    (MainMenu_Sub_Open_Control as CASE0016_Control).Refresh();

                //if (MainMenu_Sub_Open_Control is CASE0026Control)
                //    (MainMenu_Sub_Open_Control as CASE0026Control).Refresh();

                //if (MainMenu_Sub_Open_Control is CASE0021Control)
                //    (MainMenu_Sub_Open_Control as CASE0021Control).Refresh();

                //if (MainMenu_Sub_Open_Control is CASE0027Control)
                //    (MainMenu_Sub_Open_Control as CASE0027Control).Refresh();

                //if (MainMenu_Sub_Open_Control is CASE0028Control)
                //    (MainMenu_Sub_Open_Control as CASE0028Control).Refresh();

                //if (MainMenu_Sub_Open_Control is CASE0010_Control)
                //    (MainMenu_Sub_Open_Control as CASE0010_Control).Refresh();

                //if (MainMenu_Sub_Open_Control is Vendor_Maintainance)
                //    (MainMenu_Sub_Open_Control as Vendor_Maintainance).FillVendorGrid(string.Empty);

                //if (MainMenu_Sub_Open_Control is CaseSumControl)
                //    (MainMenu_Sub_Open_Control as CaseSumControl).RefreshFormGrid();

                //if (MainMenu_Sub_Open_Control is IncomepleteIntakeControl)
                //    (MainMenu_Sub_Open_Control as IncomepleteIntakeControl).Refresh();

                //if (MainMenu_Sub_Open_Control is Case2011Control)
                //    (MainMenu_Sub_Open_Control as Case2011Control).Refresh();

                //if (MainMenu_Sub_Open_Control is ADMN0030_Control)
                //    (MainMenu_Sub_Open_Control as ADMN0030_Control).Refresh();

                if (MainMenu_Sub_Open_Control is PrintApplicationControl)
                    (MainMenu_Sub_Open_Control as PrintApplicationControl).RefreshGrid();

                //if (MainMenu_Sub_Open_Control is TMS00081_Control)
                //    (MainMenu_Sub_Open_Control as TMS00081_Control).Refresh();

                //if (MainMenu_Sub_Open_Control is TMS00091Control)
                //    (MainMenu_Sub_Open_Control as TMS00091Control).Refresh();

                //if (MainMenu_Sub_Open_Control is TMSB0030_ABCcalcControl)
                //{
                //    (MainMenu_Sub_Open_Control as TMSB0030_ABCcalcControl).Refresh();
                //}

                //if (MainMenu_Sub_Open_Control is SSBGParams_Control)
                //{
                //    (MainMenu_Sub_Open_Control as SSBGParams_Control).RefreshGrid(string.Empty);
                //}

                //if (MainMenu_Sub_Open_Control is AGCYPARTControl)
                //{
                //    (MainMenu_Sub_Open_Control as AGCYPARTControl).Refresh();
                //}

                //if (MainMenu_Sub_Open_Control is TMS00100)
                //{
                //    (MainMenu_Sub_Open_Control as TMS00100).Refresh();
                //}
                //if (MainMenu_Sub_Open_Control is HSS00134Control)
                //{
                //    (MainMenu_Sub_Open_Control as HSS00134Control).Refresh();
                //}
                //if (MainMenu_Sub_Open_Control is HSS00137Control)
                //{
                //    (MainMenu_Sub_Open_Control as HSS00137Control).RefreshHss00137Form();
                //}
                //if (MainMenu_Sub_Open_Control is PIR20001Control)
                //    (MainMenu_Sub_Open_Control as PIR20001Control).Refresh();
                //if (MainMenu_Sub_Open_Control is TMSB0030_ABCcalcControl)
                //    (MainMenu_Sub_Open_Control as TMSB0030_ABCcalcControl).Refresh();

                //if (MainMenu_Sub_Open_Control is TMS00141_Control)
                //{
                //    (MainMenu_Sub_Open_Control as TMS00141_Control).RefreshGrid();
                //}
            }
        }

        private void btncloseAll_Click(object sender, EventArgs e)
        {
            string strModuleType = "";
            if (BusinessModuleID == "01")
            {
                this.pnlContainer.BackgroundImageSource = "Resources\\Images\\11-01-01.jpg";
                this.pnlContainer.BackgroundImageLayout = Wisej.Web.ImageLayout.Stretch;

                int intTabPagesTotal = ContentTabs.TabPages.Count;
                ContentTabs.TabPages.Clear();
            }
            else
            {
                pnlContainer.BackgroundImageSource = "Resources\\Images\\blank.png";
                pnlContainer.BackColor = Color.White;
                int intTabPagesTotal = ContentTabs.TabPages.Count;
                int intTabPages = ContentTabs.TabPages.Count - 1;
                for (int i = 0; i < intTabPagesTotal; i++)
                {
                    if (!ContentTabs.TabPages[intTabPages].Name.Equals("MainMenu1"))
                    {
                        if (strModuleType == "Client")
                        {
                            if (!ContentTabs.TabPages[intTabPages].Name.ToUpper().Contains("CASE2001"))
                            {
                                ContentTabs.TabPages.RemoveAt(intTabPages);
                                intTabPages = intTabPages - 1;
                            }
                            else
                            {
                                intTabPages = intTabPages - 1;
                            }
                        }
                        else
                        {
                            ContentTabs.TabPages.RemoveAt(intTabPages);
                            intTabPages = intTabPages - 1;
                        }
                    }
                }
                pnlTabs.Controls.Add(ContentTabs);
                applicationNameControl.Controls[0].Controls["panelNavButtons"].Visible = true;
                //if (strModuleType != "Client")
                //{
                //    MainToolbar.Buttons.Clear();
                //}

            }
            NavTabs.SelectedItem = null;
            btncloseAll.Visible = false;
        }

        public override void RefreshSCREENSfromuserprivlg()
        {
            for (int iContentTabs = 0; iContentTabs < ContentTabs.TabPages.Count; iContentTabs++)
            {
                string tagItem = ContentTabs.TabPages[iContentTabs].Tag.ToString();
                var i = AdminSRCNPrivilege.FindAll(x => x.Program == tagItem);
                if (i.Count == 0)
                {
                    ContentTabs.TabPages.Remove(ContentTabs.TabPages[iContentTabs]);
                }
            }

            colNavItems = privlgNavAdmnItems;
            OnContentTabsSelectedIndexChanged(ContentTabs, new EventArgs());
        }

        private void pSwitchAccount_Click(object sender, EventArgs e)
        {
            Admn0004UserForm objAdmn0004 = new Admn0004UserForm(UserID);
            objAdmn0004.StartPosition = FormStartPosition.CenterScreen;
            objAdmn0004.FormClosed += ObjAdmn0004_FormClosed;
            objAdmn0004.ShowDialog();
        }

        private void ObjAdmn0004_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Close();
            if (Admn0004UserForm.closelogin == "Y")
            {
                MasterPage main = new MasterPage();
                main.Show();
                main.Update();

                Admn0004UserForm.closelogin = "N";
            }
        }

        public override void AddTabClientIntake(string strFormCode)
        {
            //if (strFormCode == "PIP00000")
            //{
            //    List<PrivilegeEntity> userPrivilege = _model.UserProfileAccess.GetScreensByUserID(BusinessModuleID, UserID, BaseAgency + BaseDept + BaseProg);
            //    PrivilegeEntity privilegeEntity = userPrivilege.Find(u => u.Program.ToString() == "PIP00000");
            //    PIP00000Control pIP00000Control = new PIP00000Control(this, privilegeEntity);
            //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, pIP00000Control);
            //}
            //else if (strFormCode == "CASE0027")
            //{
            //    List<PrivilegeEntity> userPrivilege = _model.UserProfileAccess.GetScreensByUserID(BusinessModuleID, UserID, BaseAgency + BaseDept + BaseProg);
            //    PrivilegeEntity privilegeEntity = userPrivilege.Find(u => u.Program.ToString() == "CASE0027");
            //    CASE0027Control case0027 = new CASE0027Control(this, privilegeEntity, "Search");
            //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, case0027);
            //}
            //else if (strFormCode == "TMS00081")
            //{
            //    List<PrivilegeEntity> userPrivilege = _model.UserProfileAccess.GetScreensByUserID(BusinessModuleID, UserID, BaseAgency + BaseDept + BaseProg);
            //    PrivilegeEntity privilegeEntity = userPrivilege.Find(u => u.Program.ToString() == "TMS00081");
            //    TMS00081_Control TMS00081 = new TMS00081_Control(this, privilegeEntity);
            //    AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, TMS00081);
            //}
             if (strFormCode == "CASE2005")
            {
                List<PrivilegeEntity> userPrivilege = _model.UserProfileAccess.GetScreensByUserID(BusinessModuleID, UserID, BaseAgency + BaseDept + BaseProg);
                PrivilegeEntity privilegeEntity = userPrivilege.Find(u => u.Program.ToString() == "CASE2005");
                IncomepleteIntakeControl TMS00081 = new IncomepleteIntakeControl(this, privilegeEntity);
                AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, TMS00081);
            }
            else
            {


                if (!string.IsNullOrEmpty(BaseApplicationNo))
                {
                    List<PrivilegeEntity> userPrivilege = _model.UserProfileAccess.GetScreensByUserID(BusinessModuleID, UserID, BaseAgency + BaseDept + BaseProg);
                    PrivilegeEntity privilegeEntity = userPrivilege.Find(u => u.Program.ToString() == "CASE2001");
                    if (BaseTopApplSelect == "Y")
                    {
                        //ClientIntakeControl clientIntakeControl = new ClientIntakeControl(this, privilegeEntity);
                        Case3001Control clientIntakeControl = new Case3001Control(this, privilegeEntity);
                        AddContentTab(privilegeEntity.PrivilegeName.Trim(), privilegeEntity.Program, clientIntakeControl);
                    }
                    else
                        CommonFunctions.MessageBoxDisplay(Consts.Messages.AplicantSelectionMsg);
                }
            }
        }
    }
}
