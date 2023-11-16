using Wisej.Web;

namespace Captain.Common.Views.Forms
{
    partial class CASE4006_AddCAMS_Form
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

        #region Wisej Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle5 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle7 = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle6 = new Wisej.Web.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CASE4006_AddCAMS_Form));
            this.MainPanel = new Wisej.Web.Panel();
            this.SavePanel = new Wisej.Web.Panel();
            this.button4 = new Wisej.Web.Button();
            this.Btn_Save = new Wisej.Web.Button();
            this.GroupPanel = new Wisej.Web.Panel();
            this.Cmb_Group = new Wisej.Web.ComboBox();
            this.label1 = new Wisej.Web.Label();
            this.MAPanel = new Wisej.Web.Panel();
            this.panel7 = new Wisej.Web.Panel();
            this.label5 = new Wisej.Web.Label();
            this.label3 = new Wisej.Web.Label();
            this.Btn_SearchMS = new Wisej.Web.Button();
            this.TxtMS_Search = new Wisej.Web.TextBox();
            this.MS_Grid = new Wisej.Web.DataGridView();
            this.MS_Sel_Img = new Wisej.Web.DataGridViewImageColumn();
            this.MS_Desc = new Wisej.Web.DataGridViewTextBoxColumn();
            this.MS_Code = new Wisej.Web.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new Wisej.Web.DataGridViewTextBoxColumn();
            this.MS_Sel_SW = new Wisej.Web.DataGridViewTextBoxColumn();
            this.MS_Key = new Wisej.Web.DataGridViewTextBoxColumn();
            this.CAPanel = new Wisej.Web.Panel();
            this.panel6 = new Wisej.Web.Panel();
            this.label4 = new Wisej.Web.Label();
            this.label2 = new Wisej.Web.Label();
            this.Btn_SearchCA = new Wisej.Web.Button();
            this.TxtCA_Search = new Wisej.Web.TextBox();
            this.CA_Grid = new Wisej.Web.DataGridView();
            this.CA_Sel_Img = new Wisej.Web.DataGridViewImageColumn();
            this.CA_Desc = new Wisej.Web.DataGridViewTextBoxColumn();
            this.CA_Code = new Wisej.Web.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new Wisej.Web.DataGridViewTextBoxColumn();
            this.CA_Sel_SW = new Wisej.Web.DataGridViewTextBoxColumn();
            this.CA_Key = new Wisej.Web.DataGridViewTextBoxColumn();
            this.MainPanel.SuspendLayout();
            this.SavePanel.SuspendLayout();
            this.GroupPanel.SuspendLayout();
            this.MAPanel.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MS_Grid)).BeginInit();
            this.CAPanel.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CA_Grid)).BeginInit();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.SavePanel);
            this.MainPanel.Controls.Add(this.GroupPanel);
            this.MainPanel.Controls.Add(this.MAPanel);
            this.MainPanel.Controls.Add(this.CAPanel);
            this.MainPanel.Location = new System.Drawing.Point(2, 1);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(626, 538);
            this.MainPanel.TabIndex = 0;
            // 
            // SavePanel
            // 
            this.SavePanel.AppearanceKey = "panel-grdo";
            this.SavePanel.Controls.Add(this.button4);
            this.SavePanel.Controls.Add(this.Btn_Save);
            this.SavePanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.SavePanel.Location = new System.Drawing.Point(0, 507);
            this.SavePanel.Name = "SavePanel";
            this.SavePanel.Size = new System.Drawing.Size(626, 31);
            this.SavePanel.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.AppearanceKey = "button-cancel";
            this.button4.Location = new System.Drawing.Point(548, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 26);
            this.button4.TabIndex = 2;
            this.button4.Text = "&Cancel";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.AppearanceKey = "button-ok";
            this.Btn_Save.Location = new System.Drawing.Point(472, 2);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(76, 26);
            this.Btn_Save.TabIndex = 2;
            this.Btn_Save.Text = "&Save";
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // GroupPanel
            // 
            this.GroupPanel.BackColor = System.Drawing.Color.FromArgb(183, 183, 183);
            this.GroupPanel.Controls.Add(this.Cmb_Group);
            this.GroupPanel.Controls.Add(this.label1);
            this.GroupPanel.Location = new System.Drawing.Point(1, 0);
            this.GroupPanel.Name = "GroupPanel";
            this.GroupPanel.Size = new System.Drawing.Size(625, 26);
            this.GroupPanel.TabIndex = 2;
            // 
            // Cmb_Group
            // 
            this.Cmb_Group.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.Cmb_Group.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Cmb_Group.FormattingEnabled = true;
            this.Cmb_Group.Location = new System.Drawing.Point(286, 0);
            this.Cmb_Group.Name = "Cmb_Group";
            this.Cmb_Group.Size = new System.Drawing.Size(61, 25);
            this.Cmb_Group.TabIndex = 1;
            this.Cmb_Group.SelectedIndexChanged += new System.EventHandler(this.Cmb_Group_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label1.Location = new System.Drawing.Point(229, 4);
            this.label1.MinimumSize = new System.Drawing.Size(0, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Group";
            // 
            // MAPanel
            // 
            this.MAPanel.Controls.Add(this.panel7);
            this.MAPanel.Controls.Add(this.label3);
            this.MAPanel.Controls.Add(this.Btn_SearchMS);
            this.MAPanel.Controls.Add(this.TxtMS_Search);
            this.MAPanel.Controls.Add(this.MS_Grid);
            this.MAPanel.Location = new System.Drawing.Point(-1, 266);
            this.MAPanel.Name = "MAPanel";
            this.MAPanel.Size = new System.Drawing.Size(626, 241);
            this.MAPanel.TabIndex = 1;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.AntiqueWhite;
            this.panel7.Controls.Add(this.label5);
            this.panel7.Location = new System.Drawing.Point(0, 2);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(624, 19);
            this.panel7.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = Wisej.Web.DockStyle.Left;
            this.label5.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 19);
            this.label5.TabIndex = 0;
            this.label5.Text = "Outcomes";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(2, 220);
            this.label3.MinimumSize = new System.Drawing.Size(0, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Outcome Lookup";
            // 
            // Btn_SearchMS
            // 
            this.Btn_SearchMS.Location = new System.Drawing.Point(560, 215);
            this.Btn_SearchMS.Name = "Btn_SearchMS";
            this.Btn_SearchMS.Size = new System.Drawing.Size(63, 23);
            this.Btn_SearchMS.TabIndex = 2;
            this.Btn_SearchMS.Text = "Search";
            this.Btn_SearchMS.Click += new System.EventHandler(this.Btn_Search_WithText);
            // 
            // TxtMS_Search
            // 
            this.TxtMS_Search.Location = new System.Drawing.Point(108, 215);
            this.TxtMS_Search.Name = "TxtMS_Search";
            this.TxtMS_Search.Size = new System.Drawing.Size(447, 25);
            this.TxtMS_Search.TabIndex = 1;
            this.TxtMS_Search.LostFocus += new System.EventHandler(this.Btn_Search_WithText);
            this.TxtMS_Search.KeyDown += new Wisej.Web.KeyEventHandler(this.Btn_Search_WithText);
            // 
            // MS_Grid
            // 
            this.MS_Grid.AutoSizeRowsMode = Wisej.Web.DataGridViewAutoSizeRowsMode.AllCells;
            this.MS_Grid.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle1.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.MS_Grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.MS_Grid.ColumnHeadersHeightSizeMode = Wisej.Web.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MS_Grid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.MS_Sel_Img,
            this.MS_Desc,
            this.MS_Code,
            this.dataGridViewTextBoxColumn4,
            this.MS_Sel_SW,
            this.MS_Key});
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Blue;
            this.MS_Grid.DefaultCellStyle = dataGridViewCellStyle3;
            this.MS_Grid.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.MS_Grid.Location = new System.Drawing.Point(5, 23);
            this.MS_Grid.Name = "MS_Grid";
            dataGridViewCellStyle4.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.MS_Grid.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.MS_Grid.RowHeadersWidth = 25;
            this.MS_Grid.Size = new System.Drawing.Size(615, 191);
            this.MS_Grid.TabIndex = 0;
            this.MS_Grid.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.MS_Grid_CellClick);
            // 
            // MS_Sel_Img
            // 
            this.MS_Sel_Img.CellImageAlignment = Wisej.Web.DataGridViewContentAlignment.NotSet;
            this.MS_Sel_Img.CellImageLayout = Wisej.Web.DataGridViewImageCellLayout.Zoom;
            this.MS_Sel_Img.HeaderText = " ";
            this.MS_Sel_Img.Name = "MS_Sel_Img";
            this.MS_Sel_Img.Width = 30;
            // 
            // MS_Desc
            // 
            dataGridViewCellStyle2.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.MS_Desc.DefaultCellStyle = dataGridViewCellStyle2;
            this.MS_Desc.HeaderText = "Outcome Description";
            this.MS_Desc.Name = "MS_Desc";
            this.MS_Desc.ReadOnly = true;
            this.MS_Desc.Width = 490;
            // 
            // MS_Code
            // 
            this.MS_Code.HeaderText = "Code";
            this.MS_Code.Name = "MS_Code";
            this.MS_Code.ReadOnly = true;
            this.MS_Code.Width = 50;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Visible = false;
            // 
            // MS_Sel_SW
            // 
            this.MS_Sel_SW.Name = "MS_Sel_SW";
            this.MS_Sel_SW.Visible = false;
            // 
            // MS_Key
            // 
            this.MS_Key.Name = "MS_Key";
            this.MS_Key.Visible = false;
            // 
            // CAPanel
            // 
            this.CAPanel.Controls.Add(this.panel6);
            this.CAPanel.Controls.Add(this.label2);
            this.CAPanel.Controls.Add(this.Btn_SearchCA);
            this.CAPanel.Controls.Add(this.TxtCA_Search);
            this.CAPanel.Controls.Add(this.CA_Grid);
            this.CAPanel.Location = new System.Drawing.Point(-1, 27);
            this.CAPanel.Name = "CAPanel";
            this.CAPanel.Size = new System.Drawing.Size(626, 239);
            this.CAPanel.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.AntiqueWhite;
            this.panel6.Controls.Add(this.label4);
            this.panel6.Location = new System.Drawing.Point(1, -1);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(623, 18);
            this.panel6.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = Wisej.Web.DockStyle.Left;
            this.label4.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "Services";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 218);
            this.label2.MinimumSize = new System.Drawing.Size(0, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "Service Lookup";
            // 
            // Btn_SearchCA
            // 
            this.Btn_SearchCA.Location = new System.Drawing.Point(560, 213);
            this.Btn_SearchCA.Name = "Btn_SearchCA";
            this.Btn_SearchCA.Size = new System.Drawing.Size(63, 23);
            this.Btn_SearchCA.TabIndex = 2;
            this.Btn_SearchCA.Text = "Search";
            this.Btn_SearchCA.Click += new System.EventHandler(this.Btn_Search_WithText);
            // 
            // TxtCA_Search
            // 
            this.TxtCA_Search.Location = new System.Drawing.Point(92, 213);
            this.TxtCA_Search.Name = "TxtCA_Search";
            this.TxtCA_Search.Size = new System.Drawing.Size(460, 25);
            this.TxtCA_Search.TabIndex = 1;
            this.TxtCA_Search.LostFocus += new System.EventHandler(this.Btn_Search_WithText);
            this.TxtCA_Search.KeyDown += new Wisej.Web.KeyEventHandler(this.Btn_Search_WithText);
            // 
            // CA_Grid
            // 
            this.CA_Grid.AutoSizeRowsMode = Wisej.Web.DataGridViewAutoSizeRowsMode.AllCells;
            this.CA_Grid.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle5.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.CA_Grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.CA_Grid.ColumnHeadersHeightSizeMode = Wisej.Web.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CA_Grid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.CA_Sel_Img,
            this.CA_Desc,
            this.CA_Code,
            this.dataGridViewTextBoxColumn3,
            this.CA_Sel_SW,
            this.CA_Key});
            this.CA_Grid.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CA_Grid.Location = new System.Drawing.Point(4, 19);
            this.CA_Grid.Name = "CA_Grid";
            dataGridViewCellStyle7.Font = new System.Drawing.Font("@default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CA_Grid.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.CA_Grid.RowHeadersWidth = 25;
            this.CA_Grid.Size = new System.Drawing.Size(616, 193);
            this.CA_Grid.TabIndex = 0;
            this.CA_Grid.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.CA_Grid_CellClick);
            // 
            // CA_Sel_Img
            // 
            this.CA_Sel_Img.CellImageAlignment = Wisej.Web.DataGridViewContentAlignment.NotSet;
            this.CA_Sel_Img.CellImageLayout = Wisej.Web.DataGridViewImageCellLayout.Zoom;
            this.CA_Sel_Img.HeaderText = " ";
            this.CA_Sel_Img.Name = "CA_Sel_Img";
            this.CA_Sel_Img.Width = 30;
            // 
            // CA_Desc
            // 
            dataGridViewCellStyle6.WrapMode = Wisej.Web.DataGridViewTriState.True;
            this.CA_Desc.DefaultCellStyle = dataGridViewCellStyle6;
            this.CA_Desc.HeaderText = "Sevice Description";
            this.CA_Desc.Name = "CA_Desc";
            this.CA_Desc.ReadOnly = true;
            this.CA_Desc.Width = 490;
            // 
            // CA_Code
            // 
            this.CA_Code.HeaderText = "Code";
            this.CA_Code.Name = "CA_Code";
            this.CA_Code.ReadOnly = true;
            this.CA_Code.Width = 50;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Visible = false;
            // 
            // CA_Sel_SW
            // 
            this.CA_Sel_SW.Name = "CA_Sel_SW";
            this.CA_Sel_SW.Visible = false;
            // 
            // CA_Key
            // 
            this.CA_Key.Name = "CA_Key";
            this.CA_Key.Visible = false;
            // 
            // CASE4006_AddCAMS_Form
            // 
            this.ClientSize = new System.Drawing.Size(629, 541);
            this.Controls.Add(this.MainPanel);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Image)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CASE4006_AddCAMS_Form";
            this.Text = "CASE4006_AddCAMS_Form";
            this.MainPanel.ResumeLayout(false);
            this.SavePanel.ResumeLayout(false);
            this.GroupPanel.ResumeLayout(false);
            this.GroupPanel.PerformLayout();
            this.MAPanel.ResumeLayout(false);
            this.MAPanel.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MS_Grid)).EndInit();
            this.CAPanel.ResumeLayout(false);
            this.CAPanel.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CA_Grid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel MainPanel;
        private Panel MAPanel;
        private Panel CAPanel;
        private Button Btn_SearchMS;
        private TextBox TxtMS_Search;
        private DataGridView MS_Grid;
        private Button Btn_SearchCA;
        private TextBox TxtCA_Search;
        private DataGridView CA_Grid;
        private Panel GroupPanel;
        private Label label1;
        private Panel SavePanel;
        private Button button4;
        private Button Btn_Save;
        private ComboBox Cmb_Group;
        private Label label3;
        private Label label2;
        private Panel panel6;
        private Label label4;
        private Panel panel7;
        private Label label5;
        private DataGridViewImageColumn MS_Sel_Img;
        private DataGridViewTextBoxColumn MS_Desc;
        private DataGridViewTextBoxColumn MS_Code;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewImageColumn CA_Sel_Img;
        private DataGridViewTextBoxColumn CA_Desc;
        private DataGridViewTextBoxColumn CA_Code;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn CA_Sel_SW;
        private DataGridViewTextBoxColumn MS_Sel_SW;
        private DataGridViewTextBoxColumn MS_Key;
        private DataGridViewTextBoxColumn CA_Key;


    }
}