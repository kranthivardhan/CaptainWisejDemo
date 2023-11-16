#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;

using Wisej.Web;
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
using DevExpress.Pdf;
using DevExpress.XtraReports.UI;


#endregion

namespace Captain.Common.Views.UserControls
{
    public partial class PrintApplicationControl : BaseUserControl
    {
        string _agency = "";
        public PrintApplicationControl(BaseForm baseForm, PrivilegeEntity privileges)
        {
            InitializeComponent();
            BaseForm = baseForm;
            _agency = BaseForm.BaseAdminAgency;
            Privileges = privileges;
            PopulateToolbar(oToolbarMnustrip);
            FillGrid();
        }
        #region Properties

        public DataTable CreatePRINTApp()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("PAC_FORM", typeof(string));
            dt.Columns.Add("PAC_DISP_NAME", typeof(string));

            dt.Rows.Add("Case Management Application", "Case Management Application");
            dt.Rows.Add("Head Start Application", "Head Start Application");
            dt.Rows.Add("SIM Referral Letter", "SIM Referral Letter");
            dt.Rows.Add("Old Application for Assistance", "Case Management Application");
            dt.Rows.Add("Application for Assistance", "Application for Assistance");
            dt.Rows.Add("Print Letter", "Print Letter");
            dt.Rows.Add("Print Water & Sewer Letters", "Print Water & Sewer Letters");
            dt.Rows.Add("Energy Assistance Application", "Energy Assistance Application");
            dt.Rows.Add("Pre-Assessment Form", "Pre-Assessment Form");
            dt.Rows.Add("Emergency Sheet", "Emergency Sheet");

            return dt;
        }

        public PrivilegeEntity Privileges { get; set; }
        public ToolBarButton ToolBarEdit { get; set; }
        public ToolBarButton ToolBarNew { get; set; }
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
                ToolBarNew.ToolTipText = "Add Print Application Name";
                ToolBarNew.Enabled = true;
                ToolBarNew.ImageSource = "captain-add";
                ToolBarNew.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarNew.Click += new EventHandler(OnToolbarButtonClicked);
                if (Privileges.AddPriv.Equals("false"))
                {
                    ToolBarNew.Enabled = false;
                }

                ToolBarEdit = new ToolBarButton();
                ToolBarEdit.Tag = "Edit";
                ToolBarEdit.ToolTipText = "Edit Print Application Name";
                ToolBarEdit.Enabled = true;
                ToolBarEdit.ImageSource = "captain-edit";
                ToolBarEdit.Click -= new EventHandler(OnToolbarButtonClicked);
                ToolBarEdit.Click += new EventHandler(OnToolbarButtonClicked);
                if (Privileges.ChangePriv.Equals("false"))
                {
                    ToolBarEdit.Enabled = false;
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

                toolBar.Buttons.AddRange(new ToolBarButton[]
                {
                ToolBarNew,
                ToolBarEdit,
                });
            }
        }

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
                        dgvApplications.Enabled = true; btnSave.Visible = btnCancel.Visible = true;
                    break;

                    case Consts.ToolbarActions.Edit:
                        dgvApplications.Enabled = true; btnSave.Visible = btnCancel.Visible = true;
                        break;
                }
                executeCode.Append(Consts.Javascript.EndJavascriptCode);
            }
            catch (Exception ex) { }
        }
        //bool isExist = false;
        private void FillGrid()
        {
            dgvApplications.Rows.Clear();
            int rowIndex = 0;

            DataSet ds = new DataSet();

            ds = DatabaseLayer.MainMenu.GET_PRINAPPCNTL(BaseForm.BaseAdminAgency, string.Empty, string.Empty);

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                    if (BaseForm.BaseAdminAgency == "**")
                    {
                        DataSet ds1 = DatabaseLayer.MainMenu.GET_PRINAPPCNTL(string.Empty, string.Empty, "**");

                        if (ds1.Tables[0].Rows.Count == 0)
                        {
                            DataTable dtApp = CreatePRINTApp();
                            foreach (DataRow dr in dtApp.Rows)
                            {
                                dgvApplications.Rows.Add(dr["PAC_FORM"], false, dr["PAC_DISP_NAME"], BaseForm.BaseAdminAgency, "A");
                            }
                        }
                    }

                foreach (DataRow dr in dt.Rows)
                {
                    bool enableflag = false;

                    if (dr["PAC_ENABLE"].ToString() == "Y")
                        enableflag = true;

                    dgvApplications.Rows.Add(dr["PAC_FORM"], enableflag, dr["PAC_DISP_NAME"], dr["PAC_AGY"],"U");
                }
                //Mode = "UPDATE";
                ToolBarNew.Visible = false;ToolBarEdit.Visible = true;
            }
            else
            {
                if (BaseForm.BaseAdminAgency == "**")
                {

                }

                DataTable dtApp = CreatePRINTApp();
                foreach (DataRow dr in dtApp.Rows)
                {
                    dgvApplications.Rows.Add(dr["PAC_FORM"], false, dr["PAC_DISP_NAME"], BaseForm.BaseAdminAgency,"A");
                }
                //Mode = "ADD";
                ToolBarNew.Visible = true; ToolBarEdit.Visible = false;
            }

        }
        public void RefreshGrid()
        {
            FillGrid();
        }

        private void dgvApplications_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridView AppGrid = sender as DataGridView;
                string selectedHIE = AppGrid.SelectedRows[0].Cells["dgvForm"].Value.ToString();

                bool isSelect = false;

                if (AppGrid.SelectedRows[0].Cells["dgvEnable"].Value.ToString().Equals(Consts.YesNoVariants.True, StringComparison.CurrentCultureIgnoreCase))
                {
                    isSelect = true;
                }

                foreach (DataGridViewRow dr in AppGrid.Rows)
                {
                    string rowCode = dr.Cells["dgvForm"].Value.ToString();
                    if (!rowCode.Equals(selectedHIE))
                    {
                        dr.Cells["dgvEnable"].Value = false;
                        dr.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        dr.DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }
        string Mode = string.Empty;
        private void btnSave_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            
            foreach (DataGridViewRow dr in dgvApplications.Rows)
            {
                string Enable = "N";
                string Agency = string.Empty;

                Agency = dr.Cells["dgvAgency"].Value.ToString().Trim(); //BaseForm.BaseAdminAgency;


                if (dr.Cells["dgvMode"].Value.ToString().Equals("A"))
                    Mode = "ADD";
                else
                    Mode = "UPDATE";

                if (dr.Cells["dgvEnable"].Value.ToString().Equals(Consts.YesNoVariants.True, StringComparison.CurrentCultureIgnoreCase))
                    Enable = "Y";

                ds = DatabaseLayer.MainMenu.INSUPDEL_PRINAPPCNTL(dr.Cells["dgvForm"].Value.ToString().Trim(), Enable, dr.Cells["dgvDisplayName"].Value.ToString().Trim(),
                                                                 Agency, "JAKE",Mode);
            }

            AlertBox.Show("Saved Successfully");
            dgvApplications.Enabled = false;
            btnCancel.Visible = false;
            btnSave.Visible = false;
            ToolBarNew.Visible = false; ToolBarEdit.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FillGrid();
            dgvApplications.Enabled = false;
            btnCancel.Visible = false;
            btnSave.Visible = false;
        }
    }
}
