using Wisej.Web;
using Captain.Common.Views.Controls.Compatibility;

namespace Captain.Common.Views.Forms
{
    partial class AGCYBoutique_Formcs
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Visual WebGui Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AGCYBoutique_Formcs));
            Wisej.Web.ComponentTool componentTool1 = new Wisej.Web.ComponentTool();
            this.pnlParams = new Wisej.Web.Panel();
            this.pb_City = new Wisej.Web.PictureBox();
            this.txtCity = new Wisej.Web.TextBox();
            this.txtCounty = new Wisej.Web.TextBox();
            this.Pb_County = new Wisej.Web.PictureBox();
            this.label5 = new Wisej.Web.Label();
            this.lblYes = new Wisej.Web.Label();
            this.txtInventory = new Wisej.Web.TextBox();
            this.lblInventory = new Wisej.Web.Label();
            this.cmbPercentage = new Wisej.Web.ComboBox();
            this.lblPercentage = new Wisej.Web.Label();
            this.lblCounty = new Wisej.Web.Label();
            this.cmbCounty = new Wisej.Web.ComboBox();
            this.lblCity = new Wisej.Web.Label();
            this.cmbCity = new Wisej.Web.ComboBox();
            this.lblPoverty = new Wisej.Web.Label();
            this.txtPoverty = new Wisej.Web.TextBox();
            this.cmbLunch = new Wisej.Web.ComboBox();
            this.lblLunch = new Wisej.Web.Label();
            this.label1 = new Wisej.Web.Label();
            this.txtPopServed = new Wisej.Web.TextBox();
            this.lblPopServed = new Wisej.Web.Label();
            this.lblItemsNeeded = new Wisej.Web.Label();
            this.txtItemsNeeded = new Wisej.Web.TextBox();
            this.txtShared = new Wisej.Web.TextBox();
            this.lblShare = new Wisej.Web.Label();
            this.cmbShared = new Wisej.Web.ComboBox();
            this.txtNotes = new Wisej.Web.TextBox();
            this.lblNotes = new Wisej.Web.Label();
            this.cmbStatus = new Wisej.Web.ComboBox();
            this.lblStatus = new Wisej.Web.Label();
            this.lblDateComplete = new Wisej.Web.Label();
            this.dtpComplete = new Wisej.Web.DateTimePicker();
            this.dtpRequest = new Wisej.Web.DateTimePicker();
            this.lblDateRequest = new Wisej.Web.Label();
            this.btnCancel = new Wisej.Web.Button();
            this.btnSave = new Wisej.Web.Button();
            this.pnlSave = new Wisej.Web.Panel();
            this.pnlMain = new Wisej.Web.Panel();
            this.spacer1 = new Wisej.Web.Spacer();
            this.pnlParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_City)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pb_County)).BeginInit();
            this.pnlSave.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlParams
            // 
            this.pnlParams.Controls.Add(this.pb_City);
            this.pnlParams.Controls.Add(this.txtCity);
            this.pnlParams.Controls.Add(this.txtCounty);
            this.pnlParams.Controls.Add(this.Pb_County);
            this.pnlParams.Controls.Add(this.label5);
            this.pnlParams.Controls.Add(this.lblYes);
            this.pnlParams.Controls.Add(this.txtInventory);
            this.pnlParams.Controls.Add(this.lblInventory);
            this.pnlParams.Controls.Add(this.cmbPercentage);
            this.pnlParams.Controls.Add(this.lblPercentage);
            this.pnlParams.Controls.Add(this.lblCounty);
            this.pnlParams.Controls.Add(this.cmbCounty);
            this.pnlParams.Controls.Add(this.lblCity);
            this.pnlParams.Controls.Add(this.cmbCity);
            this.pnlParams.Controls.Add(this.lblPoverty);
            this.pnlParams.Controls.Add(this.txtPoverty);
            this.pnlParams.Controls.Add(this.cmbLunch);
            this.pnlParams.Controls.Add(this.lblLunch);
            this.pnlParams.Controls.Add(this.label1);
            this.pnlParams.Controls.Add(this.txtPopServed);
            this.pnlParams.Controls.Add(this.lblPopServed);
            this.pnlParams.Controls.Add(this.lblItemsNeeded);
            this.pnlParams.Controls.Add(this.txtItemsNeeded);
            this.pnlParams.Controls.Add(this.txtShared);
            this.pnlParams.Controls.Add(this.lblShare);
            this.pnlParams.Controls.Add(this.cmbShared);
            this.pnlParams.Controls.Add(this.txtNotes);
            this.pnlParams.Controls.Add(this.lblNotes);
            this.pnlParams.Controls.Add(this.cmbStatus);
            this.pnlParams.Controls.Add(this.lblStatus);
            this.pnlParams.Controls.Add(this.lblDateComplete);
            this.pnlParams.Controls.Add(this.dtpComplete);
            this.pnlParams.Controls.Add(this.dtpRequest);
            this.pnlParams.Controls.Add(this.lblDateRequest);
            this.pnlParams.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlParams.Location = new System.Drawing.Point(0, 0);
            this.pnlParams.Name = "pnlParams";
            this.pnlParams.Size = new System.Drawing.Size(1141, 351);
            this.pnlParams.TabIndex = 1;
            // 
            // pb_City
            // 
            this.pb_City.Cursor = Wisej.Web.Cursors.Hand;
            this.pb_City.ImageSource = "captain-filter";
            this.pb_City.Location = new System.Drawing.Point(670, 285);
            this.pb_City.MinimumSize = new System.Drawing.Size(0, 25);
            this.pb_City.Name = "pb_City";
            this.pb_City.Size = new System.Drawing.Size(17, 25);
            this.pb_City.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.pb_City.Click += new System.EventHandler(this.pb_City_Click);
            // 
            // txtCity
            // 
            this.txtCity.CharacterCasing = Wisej.Web.CharacterCasing.Upper;
            this.txtCity.Enabled = false;
            this.txtCity.Location = new System.Drawing.Point(475, 285);
            this.txtCity.MaxLength = 1000;
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(188, 25);
            this.txtCity.TabIndex = 13;
            // 
            // txtCounty
            // 
            this.txtCounty.CharacterCasing = Wisej.Web.CharacterCasing.Upper;
            this.txtCounty.Enabled = false;
            this.txtCounty.Location = new System.Drawing.Point(765, 285);
            this.txtCounty.MaxLength = 1000;
            this.txtCounty.Name = "txtCounty";
            this.txtCounty.Size = new System.Drawing.Size(160, 25);
            this.txtCounty.TabIndex = 14;
            // 
            // Pb_County
            // 
            this.Pb_County.Cursor = Wisej.Web.Cursors.Hand;
            this.Pb_County.ImageSource = "captain-filter";
            this.Pb_County.Location = new System.Drawing.Point(935, 285);
            this.Pb_County.MinimumSize = new System.Drawing.Size(0, 25);
            this.Pb_County.Name = "Pb_County";
            this.Pb_County.Size = new System.Drawing.Size(17, 25);
            this.Pb_County.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.Pb_County.Click += new System.EventHandler(this.Pb_County_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(108, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(9, 14);
            this.label5.TabIndex = 12;
            this.label5.Text = "*";
            // 
            // lblYes
            // 
            this.lblYes.AutoSize = true;
            this.lblYes.Location = new System.Drawing.Point(380, 108);
            this.lblYes.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblYes.Name = "lblYes";
            this.lblYes.Size = new System.Drawing.Size(78, 16);
            this.lblYes.TabIndex = 0;
            this.lblYes.Text = "If yes, explain";
            // 
            // txtInventory
            // 
            this.txtInventory.Location = new System.Drawing.Point(685, 316);
            this.txtInventory.MaxLength = 35;
            this.txtInventory.Name = "txtInventory";
            this.txtInventory.Size = new System.Drawing.Size(423, 25);
            this.txtInventory.TabIndex = 16;
            // 
            // lblInventory
            // 
            this.lblInventory.AutoSize = true;
            this.lblInventory.Location = new System.Drawing.Point(15, 320);
            this.lblInventory.MinimumSize = new System.Drawing.Size(660, 16);
            this.lblInventory.Name = "lblInventory";
            this.lblInventory.Size = new System.Drawing.Size(660, 16);
            this.lblInventory.TabIndex = 0;
            this.lblInventory.Text = "Types of inventory and sizes you would like to provide: See list in Satellite Res" +
    "ource Center Inventory Request Form";
            // 
            // cmbPercentage
            // 
            this.cmbPercentage.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbPercentage.FormattingEnabled = true;
            this.cmbPercentage.Location = new System.Drawing.Point(1052, 285);
            this.cmbPercentage.Name = "cmbPercentage";
            this.cmbPercentage.Size = new System.Drawing.Size(56, 25);
            this.cmbPercentage.TabIndex = 15;
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Location = new System.Drawing.Point(976, 289);
            this.lblPercentage.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(66, 16);
            this.lblPercentage.TabIndex = 0;
            this.lblPercentage.Text = "Percentage";
            // 
            // lblCounty
            // 
            this.lblCounty.AutoSize = true;
            this.lblCounty.Location = new System.Drawing.Point(715, 289);
            this.lblCounty.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblCounty.Name = "lblCounty";
            this.lblCounty.Size = new System.Drawing.Size(43, 16);
            this.lblCounty.TabIndex = 0;
            this.lblCounty.Text = "County";
            // 
            // cmbCounty
            // 
            this.cmbCounty.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbCounty.FormattingEnabled = true;
            this.cmbCounty.Location = new System.Drawing.Point(790, 254);
            this.cmbCounty.Name = "cmbCounty";
            this.cmbCounty.Size = new System.Drawing.Size(135, 25);
            this.cmbCounty.TabIndex = 12;
            this.cmbCounty.Visible = false;
            // 
            // lblCity
            // 
            this.lblCity.AutoSize = true;
            this.lblCity.Location = new System.Drawing.Point(15, 289);
            this.lblCity.MinimumSize = new System.Drawing.Size(290, 16);
            this.lblCity.Name = "lblCity";
            this.lblCity.Size = new System.Drawing.Size(290, 16);
            this.lblCity.TabIndex = 0;
            this.lblCity.Text = "Estimated cities and counties clients reside in: City";
            // 
            // cmbCity
            // 
            this.cmbCity.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbCity.FormattingEnabled = true;
            this.cmbCity.Location = new System.Drawing.Point(650, 254);
            this.cmbCity.Name = "cmbCity";
            this.cmbCity.Size = new System.Drawing.Size(128, 25);
            this.cmbCity.TabIndex = 11;
            this.cmbCity.Visible = false;
            // 
            // lblPoverty
            // 
            this.lblPoverty.AutoSize = true;
            this.lblPoverty.Location = new System.Drawing.Point(15, 258);
            this.lblPoverty.MinimumSize = new System.Drawing.Size(450, 16);
            this.lblPoverty.Name = "lblPoverty";
            this.lblPoverty.Size = new System.Drawing.Size(450, 16);
            this.lblPoverty.TabIndex = 0;
            this.lblPoverty.Text = "Estimated % of families living at or below the 150% of the Federal Poverty level";
            // 
            // txtPoverty
            // 
            this.txtPoverty.Location = new System.Drawing.Point(475, 254);
            this.txtPoverty.MaxLength = 100;
            this.txtPoverty.Name = "txtPoverty";
            this.txtPoverty.Size = new System.Drawing.Size(135, 25);
            this.txtPoverty.TabIndex = 10;
            // 
            // cmbLunch
            // 
            this.cmbLunch.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbLunch.FormattingEnabled = true;
            this.cmbLunch.Location = new System.Drawing.Point(475, 223);
            this.cmbLunch.Name = "cmbLunch";
            this.cmbLunch.Size = new System.Drawing.Size(61, 25);
            this.cmbLunch.TabIndex = 9;
            // 
            // lblLunch
            // 
            this.lblLunch.AutoSize = true;
            this.lblLunch.Location = new System.Drawing.Point(15, 227);
            this.lblLunch.MinimumSize = new System.Drawing.Size(340, 16);
            this.lblLunch.Name = "lblLunch";
            this.lblLunch.Size = new System.Drawing.Size(340, 16);
            this.lblLunch.TabIndex = 0;
            this.lblLunch.Text = "Estimated % of children who qualify for free/reduced lunch";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("@defaultBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label1.Location = new System.Drawing.Point(15, 198);
            this.label1.MinimumSize = new System.Drawing.Size(950, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(950, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "If the population served through the Satellite Resource Center will be different " +
    "from the main demographics described on their profile, we will also need:";
            // 
            // txtPopServed
            // 
            this.txtPopServed.Location = new System.Drawing.Point(380, 166);
            this.txtPopServed.MaxLength = 200;
            this.txtPopServed.Name = "txtPopServed";
            this.txtPopServed.Size = new System.Drawing.Size(728, 25);
            this.txtPopServed.TabIndex = 8;
            // 
            // lblPopServed
            // 
            this.lblPopServed.AutoSize = true;
            this.lblPopServed.Location = new System.Drawing.Point(15, 170);
            this.lblPopServed.MinimumSize = new System.Drawing.Size(305, 16);
            this.lblPopServed.Name = "lblPopServed";
            this.lblPopServed.Size = new System.Drawing.Size(305, 16);
            this.lblPopServed.TabIndex = 0;
            this.lblPopServed.Text = "Description of the population to be served (long text)";
            // 
            // lblItemsNeeded
            // 
            this.lblItemsNeeded.Location = new System.Drawing.Point(15, 139);
            this.lblItemsNeeded.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblItemsNeeded.Name = "lblItemsNeeded";
            this.lblItemsNeeded.Size = new System.Drawing.Size(352, 16);
            this.lblItemsNeeded.TabIndex = 0;
            this.lblItemsNeeded.Text = "Description of items needed (see list under Inventory Request Forms)";
            // 
            // txtItemsNeeded
            // 
            this.txtItemsNeeded.Location = new System.Drawing.Point(380, 135);
            this.txtItemsNeeded.MaxLength = 200;
            this.txtItemsNeeded.Name = "txtItemsNeeded";
            this.txtItemsNeeded.Size = new System.Drawing.Size(728, 25);
            this.txtItemsNeeded.TabIndex = 7;
            // 
            // txtShared
            // 
            this.txtShared.Enabled = false;
            this.txtShared.Location = new System.Drawing.Point(470, 104);
            this.txtShared.MaxLength = 100;
            this.txtShared.Name = "txtShared";
            this.txtShared.Size = new System.Drawing.Size(638, 25);
            this.txtShared.TabIndex = 6;
            // 
            // lblShare
            // 
            this.lblShare.AutoSize = true;
            this.lblShare.Location = new System.Drawing.Point(15, 108);
            this.lblShare.MinimumSize = new System.Drawing.Size(150, 16);
            this.lblShare.Name = "lblShare";
            this.lblShare.Size = new System.Drawing.Size(150, 16);
            this.lblShare.TabIndex = 0;
            this.lblShare.Text = "Will the space be shared?";
            // 
            // cmbShared
            // 
            this.cmbShared.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbShared.FormattingEnabled = true;
            this.cmbShared.Location = new System.Drawing.Point(175, 104);
            this.cmbShared.Name = "cmbShared";
            this.cmbShared.Size = new System.Drawing.Size(135, 25);
            this.cmbShared.TabIndex = 5;
            this.cmbShared.SelectedIndexChanged += new System.EventHandler(this.cmbShared_SelectedIndexChanged);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(175, 73);
            this.txtNotes.MaxLength = 100;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(135, 25);
            this.txtNotes.TabIndex = 4;
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(15, 77);
            this.lblNotes.MinimumSize = new System.Drawing.Size(130, 16);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(130, 16);
            this.lblNotes.TabIndex = 0;
            this.lblNotes.Text = "Notes: Square Footage";
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(175, 42);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(135, 25);
            this.cmbStatus.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(15, 46);
            this.lblStatus.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 16);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Status";
            // 
            // lblDateComplete
            // 
            this.lblDateComplete.AutoSize = true;
            this.lblDateComplete.Location = new System.Drawing.Point(380, 15);
            this.lblDateComplete.MinimumSize = new System.Drawing.Size(160, 16);
            this.lblDateComplete.Name = "lblDateComplete";
            this.lblDateComplete.Size = new System.Drawing.Size(160, 16);
            this.lblDateComplete.TabIndex = 0;
            this.lblDateComplete.Text = "Date of Request Completed";
            // 
            // dtpComplete
            // 
            this.dtpComplete.Checked = false;
            this.dtpComplete.CustomFormat = "MM/dd/yyyy";
            this.dtpComplete.Format = Wisej.Web.DateTimePickerFormat.Custom;
            this.dtpComplete.Location = new System.Drawing.Point(548, 11);
            this.dtpComplete.MinimumSize = new System.Drawing.Size(0, 25);
            this.dtpComplete.Name = "dtpComplete";
            this.dtpComplete.ShowCheckBox = true;
            this.dtpComplete.Size = new System.Drawing.Size(116, 25);
            this.dtpComplete.TabIndex = 2;
            // 
            // dtpRequest
            // 
            this.dtpRequest.CustomFormat = "MM/dd/yyyy";
            this.dtpRequest.Format = Wisej.Web.DateTimePickerFormat.Custom;
            this.dtpRequest.Location = new System.Drawing.Point(175, 11);
            this.dtpRequest.MinimumSize = new System.Drawing.Size(0, 25);
            this.dtpRequest.Name = "dtpRequest";
            this.dtpRequest.ShowCheckBox = true;
            this.dtpRequest.Size = new System.Drawing.Size(116, 25);
            this.dtpRequest.TabIndex = 1;
            // 
            // lblDateRequest
            // 
            this.lblDateRequest.AutoSize = true;
            this.lblDateRequest.Location = new System.Drawing.Point(15, 15);
            this.lblDateRequest.MinimumSize = new System.Drawing.Size(0, 16);
            this.lblDateRequest.Name = "lblDateRequest";
            this.lblDateRequest.Size = new System.Drawing.Size(91, 16);
            this.lblDateRequest.TabIndex = 0;
            this.lblDateRequest.Text = "Date of Request";
            // 
            // btnCancel
            // 
            this.btnCancel.AppearanceKey = "button-error";
            this.btnCancel.Dock = Wisej.Web.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(1051, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.AppearanceKey = "button-ok";
            this.btnSave.Dock = Wisej.Web.DockStyle.Right;
            this.btnSave.Location = new System.Drawing.Point(973, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "&Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pnlSave
            // 
            this.pnlSave.AppearanceKey = "panel-grdo";
            this.pnlSave.Controls.Add(this.btnSave);
            this.pnlSave.Controls.Add(this.spacer1);
            this.pnlSave.Controls.Add(this.btnCancel);
            this.pnlSave.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlSave.Location = new System.Drawing.Point(0, 351);
            this.pnlSave.Name = "pnlSave";
            this.pnlSave.Padding = new Wisej.Web.Padding(15, 5, 15, 5);
            this.pnlSave.Size = new System.Drawing.Size(1141, 35);
            this.pnlSave.TabIndex = 2;
            // 
            // pnlMain
            // 
            this.pnlMain.AutoScroll = true;
            this.pnlMain.Controls.Add(this.pnlParams);
            this.pnlMain.Controls.Add(this.pnlSave);
            this.pnlMain.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1141, 386);
            this.pnlMain.TabIndex = 17;
            // 
            // spacer1
            // 
            this.spacer1.Dock = Wisej.Web.DockStyle.Right;
            this.spacer1.Location = new System.Drawing.Point(1048, 5);
            this.spacer1.Name = "spacer1";
            this.spacer1.Size = new System.Drawing.Size(3, 25);
            // 
            // AGCYBoutique_Formcs
            // 
            this.ClientSize = new System.Drawing.Size(1141, 386);
            this.Controls.Add(this.pnlMain);
            this.Icon = ((System.Drawing.Image)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AGCYBoutique_Formcs";
            this.Text = "Agency Satellite Resource Center Information";
            componentTool1.ImageSource = "icon-help";
            componentTool1.Name = "tlHelp";
            componentTool1.ToolTipText = "Help";
            this.Tools.AddRange(new Wisej.Web.ComponentTool[] {
            componentTool1});
            this.ToolClick += new Wisej.Web.ToolClickEventHandler(this.AGCYBoutique_Formcs_ToolClick);
            this.pnlParams.ResumeLayout(false);
            this.pnlParams.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_City)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pb_County)).EndInit();
            this.pnlSave.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion
        private Panel pnlParams;
        private Label lblDateComplete;
        private DateTimePicker dtpComplete;
        private DateTimePicker dtpRequest;
        private Label lblDateRequest;
        private ComboBox cmbStatus;
        private Label lblStatus;
        private ComboBox cmbPercentage;
        private Label lblPercentage;
        private Label lblCounty;
        private ComboBox cmbCounty;
        private Label lblCity;
        private ComboBox cmbCity;
        private Label lblPoverty;
        private TextBox txtPoverty;
        private ComboBox cmbLunch;
        private Label lblLunch;
        private Label label1;
        private TextBox txtPopServed;
        private Label lblPopServed;
        private Label lblItemsNeeded;
        private TextBox txtItemsNeeded;
        private TextBox txtShared;
        private Label lblShare;
        private ComboBox cmbShared;
        private TextBox txtNotes;
        private Label lblNotes;
        private TextBox txtInventory;
        private Label lblInventory;
        private Button btnCancel;
        private Button btnSave;
        private Panel pnlSave;
        private Label lblYes;
        private Label label5;
        private PictureBox Pb_County;
        private TextBox txtCounty;
        private PictureBox pb_City;
        private TextBox txtCity;
        private Panel pnlMain;
        private Spacer spacer1;
    }
}