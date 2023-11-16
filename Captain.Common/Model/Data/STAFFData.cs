using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Captain.DatabaseLayer;
using Captain.Common.Model.Objects;
using System.Data.SqlClient;
using System.Data;
using Captain.Common.Utilities;


namespace Captain.Common.Model.Data
{
    public class STAFFData
    {

        public STAFFData()
        {

        }

        #region Properties

        public CaptainModel Model { get; set; }

        #endregion


        public List<STAFFMSTEntity> Browse_STAFFMST(STAFFMSTEntity Entity, string Opretaion_Mode)
        {
            List<STAFFMSTEntity> CASESPMProfile = new List<STAFFMSTEntity>();
            List<SqlParameter> sqlParamList = new List<SqlParameter>();
            try
            {
                sqlParamList = Prepare_STAFFMST_SqlParameters_List(Entity, Opretaion_Mode);
                DataSet CASESPMData = Captain.DatabaseLayer.SPAdminDB.Browse_Selected_Table(sqlParamList, "[dbo].[Browse_STAFFMST]");
                //DataSet CASESPMData = Captain.DatabaseLayer.SPAdminDB.Browse_STAFFMST(sqlParamList);

                if (CASESPMData != null && CASESPMData.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in CASESPMData.Tables[0].Rows)
                        CASESPMProfile.Add(new STAFFMSTEntity(row));
                }
            }
            catch (Exception ex)
            { return CASESPMProfile; }

            return CASESPMProfile;
        }

        public List<SqlParameter> Prepare_STAFFMST_SqlParameters_List(STAFFMSTEntity Entity, string Opretaion_Mode)
        {
            List<SqlParameter> sqlParamList = new List<SqlParameter>();
            try
            {
                if (Opretaion_Mode != "Browse")
                    sqlParamList.Add(new SqlParameter("@Row_Type", Entity.Rec_Type));

                sqlParamList.Add(new SqlParameter("@STF_AGENCY", Entity.Agency));
                sqlParamList.Add(new SqlParameter("@STF_DEPT", Entity.Dept));
                sqlParamList.Add(new SqlParameter("@STF_PROGRAM", Entity.Prog));
                sqlParamList.Add(new SqlParameter("@STF_CODE", Entity.Staff_Code));
                sqlParamList.Add(new SqlParameter("@STF_ACTIVE", Entity.Active));


                sqlParamList.Add(new SqlParameter("@STF_NAME_LAST", Entity.Last_Name));
                sqlParamList.Add(new SqlParameter("@STF_NAME_FI", Entity.First_Name));
                sqlParamList.Add(new SqlParameter("@STF_NAME_MI", Entity.Middle_Name));
                sqlParamList.Add(new SqlParameter("@STF_STATE", Entity.State));
                sqlParamList.Add(new SqlParameter("@STF_CITY", Entity.City));
                sqlParamList.Add(new SqlParameter("@STF_STREET", Entity.Street));
                sqlParamList.Add(new SqlParameter("@STF_SUFFIX", Entity.Suffix));
                sqlParamList.Add(new SqlParameter("@STF_HN", Entity.HNo));
                sqlParamList.Add(new SqlParameter("@STF_APT", Entity.Apt));
                sqlParamList.Add(new SqlParameter("@STF_FLR", Entity.Floor));
                sqlParamList.Add(new SqlParameter("@STF_ZIP", Entity.Zip));
                sqlParamList.Add(new SqlParameter("@STF_ZIPPLUS", Entity.Zip_Plus));
                sqlParamList.Add(new SqlParameter("@STF_LANGUAGE", Entity.Language));                
                sqlParamList.Add(new SqlParameter("@STF_ADL_LANGUAGE1", Entity.Language1));
                sqlParamList.Add(new SqlParameter("@STF_ADL_LANGUAGE2", Entity.Language2));
                sqlParamList.Add(new SqlParameter("@STF_ADL_LANGUAGE3", Entity.Language3));
                sqlParamList.Add(new SqlParameter("@STF_HS_PARENT", Entity.HS_Parent));
                sqlParamList.Add(new SqlParameter("@STF_EHS_PARENT", Entity.EHS_Parent));
                sqlParamList.Add(new SqlParameter("@STF_DAYCARE_PARENT", Entity.Daycare_Parent));
                sqlParamList.Add(new SqlParameter("@STF_DATE_HIRED", Entity.Date_Hired));
                sqlParamList.Add(new SqlParameter("@STF_YEARS_IN_POS", Entity.Years_in_POS));
                sqlParamList.Add(new SqlParameter("@STF_REPLACE_SM", Entity.Replace_SM));
                sqlParamList.Add(new SqlParameter("@STF_POS_FILLED", Entity.POS_Filled));
                sqlParamList.Add(new SqlParameter("@STF_WORKFOR_HS", Entity.Workfor_HS));
                sqlParamList.Add(new SqlParameter("@STF_WORKFOR_EHS", Entity.Workfor_EHS));
                sqlParamList.Add(new SqlParameter("@STF_WORKFOR_CONT", Entity.Workfor_CONT));
                sqlParamList.Add(new SqlParameter("@STF_WORKFOR_VOL", Entity.Workfor_VOL));
                sqlParamList.Add(new SqlParameter("@STF_POSITION_DATA", Entity.Position_Data));
                sqlParamList.Add(new SqlParameter("@STF_POS_CTG", Entity.POS_CTG));
                sqlParamList.Add(new SqlParameter("@STF_EDUCATION", Entity.Education));
                sqlParamList.Add(new SqlParameter("@STF_ETHNICITY", Entity.Ethnicity));
                sqlParamList.Add(new SqlParameter("@STF_RACE", Entity.Race));
                sqlParamList.Add(new SqlParameter("@STF_WEEKS_WORKED", Entity.Weeks_Worked));

                sqlParamList.Add(new SqlParameter("@STF_BASE_RATE", Entity.Base_Rate));
                sqlParamList.Add(new SqlParameter("@STF_SALARY", Entity.Salary));
                sqlParamList.Add(new SqlParameter("@STF_ANUAL_SALARY", Entity.Anual_Salary));
                sqlParamList.Add(new SqlParameter("@STF_EMPLOYMENT_TYPE", Entity.Employment_Type));
                sqlParamList.Add(new SqlParameter("@STF_HRS_WORKED_PW", Entity.HRS_Worked_PW));

                sqlParamList.Add(new SqlParameter("@STF_DATE_TERMINATED", Entity.Date_Terminated));
                sqlParamList.Add(new SqlParameter("@STF_RES_TERMINATION", Entity.RES_Terminated));
                sqlParamList.Add(new SqlParameter("@STF_SITE", Entity.Site));
                sqlParamList.Add(new SqlParameter("@STF_WORKFOR_NONHS", Entity.Workfor_NONHS));
                sqlParamList.Add(new SqlParameter("@STF_EDUCATION_PROGRESS", Entity.Edu_Progress));
                sqlParamList.Add(new SqlParameter("@STF_DATE_ACQUIRED", Entity.Date_Acquired));
                sqlParamList.Add(new SqlParameter("@STF_TRANSFER_DATE", Entity.Transerfer_Date));
                sqlParamList.Add(new SqlParameter("@STF_APPLICATION", Entity.Application));
                sqlParamList.Add(new SqlParameter("@STF_LSTC_OPERATOR", Entity.LSTC_Operator));



                if (Opretaion_Mode == "Browse")
                {
                    sqlParamList.Add(new SqlParameter("@STF_DATE_LSTC", Entity.Date_LSTC));
                    sqlParamList.Add(new SqlParameter("@STF_DATE_ADD", Entity.Date_ADD));
                    sqlParamList.Add(new SqlParameter("@STF_ADD_OPERATOR", Entity.ADD_Operator));
                }
            }
            catch (Exception ex)
            { return sqlParamList; }

            return sqlParamList;
        }

        public bool InsertUpdateDelStaffMst(STAFFMSTEntity Entity)
        {
            bool boolSuccess = false;
            List<SqlParameter> sqlParamList = new List<SqlParameter>();
            try
            {
               
                if(Entity.Agency!=string.Empty)
                 sqlParamList.Add(new SqlParameter("@STF_AGENCY", Entity.Agency));
                if (Entity.Dept != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_DEPT", Entity.Dept));
                if (Entity.Prog != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_PROGRAM", Entity.Prog));
                if (Entity.Staff_Code != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_CODE", Entity.Staff_Code));
                if (Entity.Active != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_ACTIVE", Entity.Active));

                if (Entity.Last_Name != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_NAME_LAST", Entity.Last_Name));
                if (Entity.First_Name != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_NAME_FI", Entity.First_Name));
                if (Entity.Middle_Name != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_NAME_MI", Entity.Middle_Name));
                if (Entity.State != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_STATE", Entity.State));
                if (Entity.City != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_CITY", Entity.City));
                if (Entity.Street != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_STREET", Entity.Street));
                if (Entity.Suffix != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_SUFFIX", Entity.Suffix));
                if (Entity.HNo != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_HN", Entity.HNo));
                if (Entity.Apt != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_APT", Entity.Apt));
                if (Entity.Floor != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_FLR", Entity.Floor));
                if (Entity.Zip != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_ZIP", Entity.Zip));
                if (Entity.Zip_Plus != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_ZIPPLUS", Entity.Zip_Plus));
                if (Entity.Language != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_LANGUAGE", Entity.Language));
                if (Entity.Language1 != string.Empty)                
                sqlParamList.Add(new SqlParameter("@STF_ADL_LANGUAGE1", Entity.Language1));
                if (Entity.Language2 != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_ADL_LANGUAGE2", Entity.Language2));
                if (Entity.Language3 != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_ADL_LANGUAGE3", Entity.Language3));

                if (Entity.HS_Parent != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_HS_PARENT", Entity.HS_Parent));
                if (Entity.EHS_Parent != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_EHS_PARENT", Entity.EHS_Parent));
                if (Entity.Daycare_Parent != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_DAYCARE_PARENT", Entity.Daycare_Parent));
                if (Entity.Date_Hired != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_DATE_HIRED", Entity.Date_Hired));
                if (Entity.Years_in_POS != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_YEARS_IN_POS", Entity.Years_in_POS));
                if (Entity.Replace_SM != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_REPLACE_SM", Entity.Replace_SM));
                if (Entity.POS_Filled != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_POS_FILLED", Entity.POS_Filled));
                if (Entity.Workfor_HS != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_WORKFOR_HS", Entity.Workfor_HS));
                if (Entity.Workfor_EHS != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_WORKFOR_EHS", Entity.Workfor_EHS));
                if (Entity.Workfor_CONT != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_WORKFOR_CONT", Entity.Workfor_CONT));
                if (Entity.Workfor_VOL != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_WORKFOR_VOL", Entity.Workfor_VOL));
                if (Entity.Position_Data != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_POSITION", Entity.Position_Data));
                if (Entity.POS_CTG != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_POS_CTG", Entity.POS_CTG));
                if (Entity.Education != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_EDUCATION", Entity.Education));
                if (Entity.Ethnicity != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_ETHNICITY", Entity.Ethnicity));
                if (Entity.Race != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_RACE", Entity.Race));
                if (Entity.Weeks_Worked != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_WEEKS_WORKED", Entity.Weeks_Worked));
                if (Entity.Base_Rate != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_BASE_RATE", Entity.Base_Rate));
                if (Entity.Salary != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_SALARY", Entity.Salary));
                if (Entity.Anual_Salary != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_ANUAL_SALARY", Entity.Anual_Salary));
                if (Entity.Employment_Type != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_EMPLOYMENT_TYPE", Entity.Employment_Type));
                if (Entity.HRS_Worked_PW != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_HRS_WORKED_PW", Entity.HRS_Worked_PW));
                if (Entity.Date_Terminated != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_DATE_TERMINATED", Entity.Date_Terminated));
                if (Entity.RES_Terminated != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_RES_TERMINATION", Entity.RES_Terminated));
                if (Entity.Site != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_SITE", Entity.Site));
                if (Entity.Workfor_NONHS != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_WORKFOR_NONHS", Entity.Workfor_NONHS));
                if (Entity.Edu_Progress != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_EDUCATION_PROGRESS", Entity.Edu_Progress));
                if (Entity.Date_Acquired != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_DATE_ACQUIRED", Entity.Date_Acquired));
                if (Entity.Transerfer_Date != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_TRANSFER_DATE", Entity.Transerfer_Date));
                if (Entity.Application != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_APPLICATION", Entity.Application));
                if (Entity.LSTC_Operator != string.Empty)
                sqlParamList.Add(new SqlParameter("@STF_LSTC_OPERATOR", Entity.LSTC_Operator));
              
                if (Entity.ADD_Operator != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STF_ADD_OPERATOR", Entity.ADD_Operator));

                sqlParamList.Add(new SqlParameter("@Mode", Entity.Mode));
                boolSuccess = CaseMst.InsertUpdateDelStaffMst(sqlParamList);
               
            }
            catch (Exception ex)
            {

                boolSuccess = false;
            }

          
            return boolSuccess;
        }

        public bool InsertUpdateDelStaffPost(STAFFPostEntity Entity)
        {
            bool boolSuccess = false;
            List<SqlParameter> sqlParamList = new List<SqlParameter>();
            try
            {

                if (Entity.Agency != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_AGENCY", Entity.Agency));
                if (Entity.Dept != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_DEPT", Entity.Dept));
                if (Entity.Prog != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_PROGRAM", Entity.Prog));
                if (Entity.Staff_Code != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_CODE", Entity.Staff_Code));
                if (Entity.Category != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_CATEGORY", Entity.Category));

                if (Entity.Seq != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_SEQ", Entity.Seq));
                if (Entity.Provider != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_PROVIDER", Entity.Provider));
                if (Entity.CourseTitle != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_COURSE_TITLE", Entity.CourseTitle));
                if (Entity.DateCompleted != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_DATE_COMPLETED", Entity.DateCompleted));
                if (Entity.CollegeCredits != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_COLLEGE_CREDITS", Entity.CollegeCredits));
                if (Entity.CeuCredits != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_CEU_CREDITS", Entity.CeuCredits));
                if (Entity.ClockHours != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_CLOCK_HOURS", Entity.ClockHours));                
                
                    sqlParamList.Add(new SqlParameter("@STP_LSTC_OPERATOR", Entity.LSTC_Operator));                
                    sqlParamList.Add(new SqlParameter("@STP_ADD_OPERATOR", Entity.ADD_Operator));

                if (Entity.Location != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_LOCATION", Entity.Location));
                if (Entity.Sponsor != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_SPONSOR", Entity.Sponsor));
                if (Entity.Presenter != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_PRESENTER", Entity.Presenter));
                if (Entity.TotalCost != string.Empty)
                    sqlParamList.Add(new SqlParameter("@STP_TOTALCOST", Entity.TotalCost));

                sqlParamList.Add(new SqlParameter("@Mode", Entity.Mode));
                boolSuccess = CaseMst.InsertUpdateDelStaffPost(sqlParamList);

            }
            catch (Exception ex)
            {

                boolSuccess = false;
            }


            return boolSuccess;
        }



        public List<STAFFPostEntity> GetStaffPostDetails(string agency, string dep, string program, string code, string category, string seq)
        {
            List<STAFFPostEntity> StaffPostDetails = new List<STAFFPostEntity>();
            try
            {
                DataSet SitePost = Captain.DatabaseLayer.CaseMst.GetStaffPostAllDetails(agency, dep, program, code, category, seq);
                if (SitePost != null && SitePost.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in SitePost.Tables[0].Rows)
                    {
                        StaffPostDetails.Add(new STAFFPostEntity(row));
                    }
                }
            }
            catch (Exception ex)
            {
                //
                return StaffPostDetails;
            }

            return StaffPostDetails;
        }


    }
}
