/************************************************************************
 * Conversion On    :   11/25/2022
 * Converted By     :   Kranthi
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
using Wisej.Web;

#endregion

namespace Captain.Common.Views.Forms
{
    public partial class CASE4006_AddCAMS_Form : Form
    {
        #region private variables

        private ErrorProvider _errorProvider = null;
        private CaptainModel _model = null;

        #endregion


        public CASE4006_AddCAMS_Form(BaseForm baseform, PrivilegeEntity privileges, List<CASESPM2Entity> ADD_CaMs_Details, string SP_code, string SP_spm_year, string SP_spm_seq)
        {
            InitializeComponent();

            BaseForm = baseform;
            ADD_CAMS_Details = ADD_CaMs_Details;
            SP_Code = SP_code;
            SP_Spm_Year = SP_spm_year;
            SP_Spm_Seq = SP_spm_seq;

            //InitializeComponent();
            _model = new CaptainModel();
            this.Text = privileges.PrivilegeName + " - Additional Branch Maintenance";
            Privileges = privileges;
            //BaseForm = baseForm;
            //Calling_Form = Form_Name;

            Fill_Group_Combo();
            Get_App_Additional_CAMS_List();
            Get_CAMAST_Data();
            Get_MSMAST_Data();

        }

        string Img_Saved = Consts.Icons.ico_Save;
        string Img_Blank = Consts.Icons.ico_Blank;
        string Img_Tick = Consts.Icons.ico_Tick;

        #region properties

        public string SP_Code { get; set; }
        
        public BaseForm BaseForm { get; set; }

        public List<FldcntlHieEntity> CntlCAEntity { get; set; }

        public List<FldcntlHieEntity> CntlMSEntity { get; set; }

        public int Max_Group_Number { get; set; }

        public List<CASESPM2Entity> ADD_CAMS_Details { get; set; }

        public PrivilegeEntity Privileges { get; set; }

        public string SP_Spm_Year { get; set; }

        public string SP_Spm_Seq  { get; set; }

        #endregion

        private void Hepl_Click(object sender, EventArgs e)
        {
           // Help.ShowHelp(this, Context.Server.MapPath("~\\Resources\\HelpFiles\\Captain_Help.chm"), HelpNavigator.KeywordIndex, "CASE0006_Add");
        }

        int Max_Group = 1;
        private void Fill_Group_Combo()
        {
            Cmb_Group.Items.Clear();
            int Tmp_Curr_Grp = 0;
            foreach (CASESPM2Entity Entity in ADD_CAMS_Details)
            {
                Tmp_Curr_Grp = int.Parse(Entity.Curr_Group.Trim());

                if (Max_Group <= Tmp_Curr_Grp && Entity.Type1 == "MS")
                    Max_Group = (Tmp_Curr_Grp + 1);
            }

            for (int i = 1; i <= (Max_Group); i++)
                Cmb_Group.Items.Add(new ListItem(i.ToString(), i.ToString()));

            Cmb_Group.SelectedIndex = (Max_Group - 1);
        }


        List<CAMASTEntity> CAMASTList = new List<CAMASTEntity>();
        private void Get_CAMAST_Data()
        {
            CAMASTList = _model.SPAdminData.Browse_CAMAST(null, null, null, null);
            Fill_CA_Grid(null);
        }

        private void Fill_CA_Grid(string Search_Text)
        {
            bool Fill_All = true, CA_Exists = false;

            if (!string.IsNullOrEmpty(Search_Text))
            {
                Fill_All = false;
                Search_Text = (Search_Text.Trim()).ToUpper();
            }
            else
                Search_Text = string.Empty;

            if (CAMASTList.Count > 0)
            {
                CA_Grid.Rows.Clear();
                int rowIndex = 0; string CA_KEY = "";
                foreach (CAMASTEntity Entity in CAMASTList)
                {
                    Entity.Sel_SW = Entity.Launch_Sel_SW = Entity.Post_Exists = CA_Exists = false; CA_KEY = " ";
                    Entity.Can_Delete = true;
                    foreach (CASESPM2Entity dt_CAMS in ADD_CAMS_Details)
                    {
                        if (dt_CAMS.Type1 == "CA" && (dt_CAMS.Curr_Group == ((ListItem)Cmb_Group.SelectedItem).Value.ToString()) &&
                            (dt_CAMS.CamCd.Trim()) == (Entity.Code.ToString().Trim()))
                            //(dt_CAMS.CamCd.Trim()).Substring(0, 4) == (Entity.Code.ToString().Trim()).Substring(0, 4))
                        {
                            //CA_Exists = true;
                            Entity.Sel_SW = Entity.Launch_Sel_SW = true;
                            

                            foreach (CASEACTEntity Ent in Activity_List)
                            {
                                if ((dt_CAMS.Curr_Group == ((ListItem)Cmb_Group.SelectedItem).Value.ToString()) &&
                                    (Ent.ACT_Code.Trim()) == (Entity.Code.ToString().Trim()) && 
                                    (Ent.Group.Trim()) == (dt_CAMS.Group.Trim()))
                                {
                                    Entity.Post_Exists = CA_Exists = true; Entity.Can_Delete = false;
                                    CA_KEY = Ent.Service_plan.Trim() + SP_Spm_Seq + Ent.Branch.Trim() +
                                                Ent.Group.ToString() + "CA" + Ent.ACT_Code.Trim();
                                    break;
                                }
                            }
                            
                            
                            break;
                        }
                    }

                    if (((Entity.Desc).ToUpper()).Contains(Search_Text.Trim()) || Fill_All)
                    {
                        if (CA_Exists)
                            rowIndex = CA_Grid.Rows.Add(Img_Saved, Entity.Desc.ToString(), Entity.Code.ToString(), "Y", "S", CA_KEY);
                        else
                        {
                            if (Entity.Sel_SW)
                                rowIndex = CA_Grid.Rows.Add(Img_Tick, Entity.Desc.ToString(), Entity.Code.ToString(), "Y", "Y", CA_KEY);
                            else
                                rowIndex = CA_Grid.Rows.Add(Img_Blank, Entity.Desc.ToString(), Entity.Code.ToString(), " ", "N", CA_KEY);
                        }
                        if (Entity.Active != "True")
                            CA_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Red; 
                    }
                }
                //if (CA_Grid.RowCount > 0)
                //    CA_Grid.Rows[0].Tag = 0;
            }
        }

        List<MSMASTEntity> MSMASTList = new List<MSMASTEntity>();
        private void Get_MSMAST_Data()
        {
            MSMASTList  = _model.SPAdminData.Browse_MSMAST(null, null, null, null, null);
            Fill_MS_Grid(null);
        }

        private void Fill_MS_Grid(string Search_Text)
        {
            bool Fill_All = true, MS_Exists = false;
            int rowIndex = 0;

            if (!string.IsNullOrEmpty(Search_Text))
            {
                Fill_All = false;
                Search_Text = (Search_Text.Trim()).ToUpper();
            }
            else
                Search_Text = string.Empty;

            MS_Grid.Rows.Clear();
            if (MSMASTList.Count > 0)
            {
                string MS_KEY = " ";
                foreach (MSMASTEntity Entity in MSMASTList)
                {
                    Entity.Sel_SW = Entity.Launch_Sel_SW = Entity.Post_Exists = MS_Exists = false; MS_KEY = " ";
                    Entity.Can_Delete = true;
                    foreach (CASESPM2Entity dt_CAMS in ADD_CAMS_Details)
                    {
                        if (dt_CAMS.Type1 == "MS" && (dt_CAMS.Curr_Group == ((ListItem)Cmb_Group.SelectedItem).Value.ToString()) &&
                            (dt_CAMS.CamCd.Trim()) == (Entity.Code.ToString().Trim()))
                            //(dt_CAMS.CamCd.Trim()).Substring(0, 4) == (Entity.Code.ToString().Trim()).Substring(0, 4))
                        {
                            Entity.Sel_SW = Entity.Launch_Sel_SW = true;  

                            foreach (CASEMSEntity Ent in MileStone_List)
                            {
                                if ((dt_CAMS.Curr_Group == ((ListItem)Cmb_Group.SelectedItem).Value.ToString()) &&
                                    (Ent.MS_Code.Trim()) == (Entity.Code.ToString().Trim()) &&
                                    (Ent.Group.Trim()) == (dt_CAMS.Group.Trim()))
                                {
                                    Entity.Post_Exists = MS_Exists = true; Entity.Can_Delete = false;
                                    MS_KEY = Ent.Service_plan.Trim() + SP_Spm_Seq + Ent.Branch.Trim() +
                                                Ent.Group.ToString() + "MS" + Ent.MS_Code.Trim();
                                    
                                    break;
                                }
                            }

                            break;

                        }
                    }

                    if (((Entity.Desc).ToUpper()).Contains(Search_Text.Trim()) || Fill_All)
                    {
                        //MS_Grid.Rows.Add(Img_Blank, Entity.Desc.ToString(), Entity.Code.ToString(), " ");

                        if (MS_Exists)
                            rowIndex = MS_Grid.Rows.Add(Img_Saved, Entity.Desc.ToString(), Entity.Code.ToString(), "Y", "S", MS_KEY);
                        else
                        {
                            if (Entity.Sel_SW)
                                rowIndex = MS_Grid.Rows.Add(Img_Tick, Entity.Desc.ToString(), Entity.Code.ToString(), "Y", "Y", MS_KEY);
                            else
                                rowIndex = MS_Grid.Rows.Add(Img_Blank, Entity.Desc.ToString(), Entity.Code.ToString(), " ", "N", MS_KEY);
                        }

                        if (Entity.Active != "True")
                            MS_Grid.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Red; 

                    }
                }
            }

        }

        private void Btn_Search_WithText(object sender, EventArgs e)
        {
            if (sender == Btn_SearchCA || sender == TxtCA_Search)
            {
                if(!string.IsNullOrEmpty(TxtCA_Search.Text.Trim()))
                    Fill_CA_Grid(TxtCA_Search.Text.Trim());
                else
                    Fill_CA_Grid(null);
            }
            else
            {
                if (!string.IsNullOrEmpty(TxtMS_Search.Text.Trim()))
                    Fill_MS_Grid(TxtMS_Search.Text.Trim());
                else
                    Fill_MS_Grid(null);
            }
        }

        private void Cmb_Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearCAMS_List();
            TxtCA_Search.Text = TxtMS_Search.Text = "";
            Fill_CA_Grid(null);
            Fill_MS_Grid(null);
        }

        private void ClearCAMS_List()
        {
            foreach (CAMASTEntity Entity in CAMASTList)
                Entity.Sel_SW = false;

            foreach (MSMASTEntity Entity in MSMASTList)
                Entity.Sel_SW = false;
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (Get_SP2_CAMS_Arrays())
            {
                ReArrange_Additional_Branch_Details();  //12022013
                Max_Group = 1;
                Fill_Group_Combo();
                this.DialogResult = DialogResult.OK;
                //Max_Group++;
                //Cmb_Group.Items.Add(new ListItem(Max_Group.ToString(), Max_Group.ToString()));
                //Cmb_Group.SelectedIndex = (Max_Group - 1);
                Fill_Additional_CAMS_Details();
                Cmb_Group_SelectedIndexChanged(Cmb_Group, EventArgs.Empty);
            }
        }


        List<CASESPM2Entity> New_CAMS_Details = new List<CASESPM2Entity>();
        private void ReArrange_Additional_Branch_Details()
        {
            CASESPM2Entity Search_Entity2 = new CASESPM2Entity(true);

            Search_Entity2.Agency = BaseForm.BaseAgency;
            Search_Entity2.Dept = BaseForm.BaseDept;
            Search_Entity2.Prog = BaseForm.BaseProg;
            Search_Entity2.Year = SP_Spm_Year;                         // Year will be always Four-Spaces in CASESPM2
            Search_Entity2.App = BaseForm.BaseApplicationNo; 
            Search_Entity2.Spm_Seq = SP_Spm_Seq;
            Search_Entity2.ServPlan = SP_Code;
            New_CAMS_Details = _model.SPAdminData.Browse_CASESPM2(Search_Entity2, "Browse");


            string Priv_Type = null; int Tmp_Ordinal_Cnt = 1, Tmp_Grp_Cnt = 1;
            foreach (CASESPM2Entity Entity in New_CAMS_Details)
            {
                if (New_CAMS_Details.Count > 0)
                {
                    if (Entity.Type1 == "CA" && Priv_Type == "MS")
                    {
                        Tmp_Grp_Cnt++;
                        // Tmp_Ordinal_Cnt = 1;
                    }
                }

                Entity.Curr_Group = Tmp_Grp_Cnt.ToString();
                Entity.SelOrdinal = Tmp_Ordinal_Cnt.ToString();
                Priv_Type = Entity.Type1;
                Tmp_Ordinal_Cnt++;
            }

            ADD_CAMS_Details.Clear();
            if (New_CAMS_Details.Count > 0) 
            {
                foreach (CASESPM2Entity Entity in New_CAMS_Details) 
                    _model.SPAdminData.UpdateCASESPM2(Entity, "RowChange", out Sql_SP_Result_Message, "RowChange");

                ADD_CAMS_Details = New_CAMS_Details;
           }
        }

        private void CA_Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (CA_Grid.Rows.Count > 0)
            {
                int ColIdx = CA_Grid.CurrentCell.ColumnIndex;
                int RowIdx = CA_Grid.CurrentCell.RowIndex;
                string CA_Sel_SW = CA_Grid.CurrentRow.Cells["CA_Sel_SW"].Value.ToString();

                if ((CA_Sel_SW == "Y" || CA_Sel_SW == "N") && e.RowIndex != -1 && e.ColumnIndex == 0)
                {
                    if (CA_Sel_SW == "Y")
                    {
                        CA_Grid.CurrentRow.Cells["CA_Sel_Img"].Value = Img_Blank;
                        CA_Grid.CurrentRow.Cells["CA_Sel_SW"].Value = "N";

                        foreach (CAMASTEntity Entity in CAMASTList)
                        {
                            if (Entity.Code == CA_Grid.CurrentRow.Cells["CA_Code"].Value.ToString())
                                Entity.Sel_SW = false;
                        }
                    }
                    else
                    {
                        CA_Grid.CurrentRow.Cells["CA_Sel_Img"].Value = Img_Tick;
                        CA_Grid.CurrentRow.Cells["CA_Sel_SW"].Value = "Y";

                        foreach (CAMASTEntity Entity in CAMASTList)
                        {
                            if (Entity.Code == CA_Grid.CurrentRow.Cells["CA_Code"].Value.ToString())
                                Entity.Sel_SW = true;
                        }
                    }
                }
            }
        }

        private void MS_Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (MS_Grid.Rows.Count > 0)
            {
                int ColIdx = MS_Grid.CurrentCell.ColumnIndex;
                int RowIdx = MS_Grid.CurrentCell.RowIndex;
                string CA_Sel_SW = MS_Grid.CurrentRow.Cells["MS_Sel_SW"].Value.ToString();

                if ((CA_Sel_SW == "Y" || CA_Sel_SW == "N") && e.RowIndex != -1 && e.ColumnIndex == 0)
                {
                    if (CA_Sel_SW == "Y")
                    {
                        MS_Grid.CurrentRow.Cells["MS_Sel_Img"].Value = Img_Blank;
                        MS_Grid.CurrentRow.Cells["MS_Sel_SW"].Value = "N";

                        foreach (MSMASTEntity Entity in MSMASTList)
                        {
                            if (Entity.Code == MS_Grid.CurrentRow.Cells["MS_Code"].Value.ToString())
                                Entity.Sel_SW = false;
                        }
                    }
                    else
                    {
                        MS_Grid.CurrentRow.Cells["MS_Sel_Img"].Value = Img_Tick;
                        MS_Grid.CurrentRow.Cells["MS_Sel_SW"].Value = "Y";

                        foreach (MSMASTEntity Entity in MSMASTList)
                        {
                            if (Entity.Code == MS_Grid.CurrentRow.Cells["MS_Code"].Value.ToString())
                                Entity.Sel_SW = true;
                        }
                    }
                }
            }
        }


        string Sql_SP_Result_Message = string.Empty;
        private bool Get_SP2_CAMS_Arrays()
        {
            bool CAMS_Added = false;
            int Row_Value = 1;
            string Original_Grp_Chec_Str = string.Empty;

            CASESPM2Entity SP2Entity = new CASESPM2Entity(true);
            if (CAMASTList.Count > 0)
            {
                SP2Entity.Rec_Type = "I";
                SP2Entity.Agency = BaseForm.BaseAgency;
                SP2Entity.Dept = BaseForm.BaseDept;
                SP2Entity.Prog = BaseForm.BaseProg;
                SP2Entity.Year = SP_Spm_Year;
                SP2Entity.App = BaseForm.BaseApplicationNo;
                SP2Entity.ServPlan = SP_Code;
                SP2Entity.Spm_Seq = SP_Spm_Seq;
                SP2Entity.Branch = "9";
                SP2Entity.Curr_Group = SP2Entity.Group = (((ListItem)Cmb_Group.SelectedItem).Value.ToString());
                SP2Entity.Type1 = "CA";
                SP2Entity.lstcOperator = BaseForm.UserID;


                foreach (CAMASTEntity Entity in CAMASTList)
                {
                    if ((Entity.Sel_SW && !Entity.Launch_Sel_SW && !Entity.Post_Exists) ||
                        (!Entity.Sel_SW && Entity.Launch_Sel_SW && !Entity.Post_Exists))
                    {
                        SP2Entity.CamCd = Entity.Code;
                        SP2Entity.SelOrdinal = Row_Value.ToString();

                        if (!Entity.Sel_SW)
                        {
                            foreach (CASESPM2Entity Ent in ADD_CAMS_Details)
                            {
                                if (Ent.Curr_Group.ToString() + Ent.CamCd.Trim() == SP2Entity.Curr_Group + Entity.Code.Trim() && Ent.Type1 == "CA")
                                {   SP2Entity = new CASESPM2Entity(Ent);    break;  }
                            }
                        }
                        else
                        {   // To Avoid Duplicates with original groups
                            foreach (CASESPM2Entity Ent in ADD_CAMS_Details)
                            {
                                if (Ent.CamCd.Trim() == Entity.Code.Trim() && Ent.Type1 == "CA")
                                    Original_Grp_Chec_Str += Ent.Group.ToString() + ",";
                            }

                            if (Original_Grp_Chec_Str.Contains(SP2Entity.Group))
                            {
                                for (int i = 1; ; i++)
                                {
                                    if (!Original_Grp_Chec_Str.Contains(i.ToString()))
                                    {   SP2Entity.Group = i.ToString(); break;  }
                                }
                            }
                        }


                        SP2Entity.Rec_Type = "I";
                        if (!Entity.Sel_SW)
                            SP2Entity.Rec_Type = "D";

                        if (_model.SPAdminData.UpdateCASESPM2(SP2Entity, string.Empty, out Sql_SP_Result_Message, string.Empty))
                        {
                            CAMS_Added = true;
                            if(SP2Entity.Rec_Type == "I")
                                Row_Value++;
                        }
                    }
                }
            }

            if (MSMASTList.Count > 0)
            {
                SP2Entity.Type1 = "MS"; Original_Grp_Chec_Str = string.Empty;
                foreach (MSMASTEntity Entity in MSMASTList)
                {
                    if ((Entity.Sel_SW && !Entity.Launch_Sel_SW && !Entity.Post_Exists) ||
                        (!Entity.Sel_SW && Entity.Launch_Sel_SW && !Entity.Post_Exists))
                    {
                        SP2Entity.CamCd = Entity.Code;
                        SP2Entity.SelOrdinal = Row_Value.ToString();

                        if (!Entity.Sel_SW)
                        {
                            foreach (CASESPM2Entity Ent in ADD_CAMS_Details)
                            {
                                if (Ent.Curr_Group.ToString() + Ent.CamCd.Trim() == SP2Entity.Curr_Group + Entity.Code.Trim() && Ent.Type1 == "MS")
                                {
                                    SP2Entity = new CASESPM2Entity(Ent);
                                    break;
                                }
                            }
                        }
                        else
                        {   // To Avoid Duplicates with original groups
                            foreach (CASESPM2Entity Ent in ADD_CAMS_Details)
                            {
                                if (Ent.CamCd.Trim() == Entity.Code.Trim() && Ent.Type1 == "MS")
                                    Original_Grp_Chec_Str += Ent.Group.ToString() + ",";
                            }

                            if (Original_Grp_Chec_Str.Contains(SP2Entity.Group))
                            {
                                for (int i = 1; ; i++)
                                {
                                    if (!Original_Grp_Chec_Str.Contains(i.ToString()))
                                    {   SP2Entity.Group = i.ToString(); break;  }
                                }
                            }
                        }

                        SP2Entity.Rec_Type = "I";
                        if (!Entity.Sel_SW)
                            SP2Entity.Rec_Type = "D";

                        if (_model.SPAdminData.UpdateCASESPM2(SP2Entity, string.Empty, out Sql_SP_Result_Message, string.Empty))
                        {
                            CAMS_Added = true;
                            if(SP2Entity.Rec_Type == "I")
                                Row_Value++;
                        }
                    }
                }
            }
            return CAMS_Added;
        }


        List<CASEACTEntity> Activity_List = new List<CASEACTEntity>();
        List<CASEMSEntity> MileStone_List = new List<CASEMSEntity>();
        private void Get_App_Additional_CAMS_List()
        {
            CASEACTEntity CA_Pass_Entity = new CASEACTEntity(true);
            CASEMSEntity Search_MS_Details = new CASEMSEntity(true);
            CA_Pass_Entity.Agency = Search_MS_Details.Agency = BaseForm.BaseAgency;
            CA_Pass_Entity.Dept = Search_MS_Details.Dept = BaseForm.BaseDept;
            CA_Pass_Entity.Program = Search_MS_Details.Program = BaseForm.BaseProg;

            CA_Pass_Entity.Year = Search_MS_Details.Year = SP_Spm_Year;
            CA_Pass_Entity.App_no = Search_MS_Details.App_no = BaseForm.BaseApplicationNo;
            CA_Pass_Entity.Service_plan = Search_MS_Details.Service_plan = SP_Code;
            CA_Pass_Entity.SPM_Seq = Search_MS_Details.SPM_Seq = SP_Spm_Seq;

            CA_Pass_Entity.Branch = Search_MS_Details.Branch = "9";

            Activity_List = _model.SPAdminData.Browse_CASEACT(CA_Pass_Entity, "Browse");
            MileStone_List = _model.SPAdminData.Browse_CASEMS(Search_MS_Details, "Browse");
        }

        private void Fill_Additional_CAMS_Details()
        {
            ADD_CAMS_Details.Clear();
            CASESPM2Entity Search_Entity2 = new CASESPM2Entity(true);

            Search_Entity2.Agency = BaseForm.BaseAgency;
            Search_Entity2.Dept = BaseForm.BaseDept;
            Search_Entity2.Prog = BaseForm.BaseProg;
            Search_Entity2.Year = SP_Spm_Year;
            Search_Entity2.App = BaseForm.BaseApplicationNo; ;
            Search_Entity2.Spm_Seq = SP_Spm_Seq;
            Search_Entity2.Branch = "9";
            Search_Entity2.ServPlan = SP_Code;

            ADD_CAMS_Details = _model.SPAdminData.Browse_CASESPM2(Search_Entity2, "Browse");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}