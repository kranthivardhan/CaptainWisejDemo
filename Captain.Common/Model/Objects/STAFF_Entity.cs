using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Captain.Common.Model.Objects
{
    /// <summary>
    /// Entity Object
    /// </summary>
    [Serializable]


    public class STAFFMSTEntity
    {
        #region Constructors

        public STAFFMSTEntity()
        {
            Rec_Type = string.Empty;
            Agency = string.Empty;
            Dept = string.Empty;
            Prog = string.Empty;
            Staff_Code = string.Empty;
            Active = string.Empty;
            Last_Name = string.Empty;
            First_Name = string.Empty;
            Middle_Name = string.Empty;
            State = string.Empty;
            City = string.Empty;
            Street = string.Empty;
            Suffix = string.Empty;
            HNo = string.Empty;
            Apt = string.Empty;
            Floor = string.Empty;
            Zip = string.Empty;
            Zip_Plus = string.Empty;
            Language = string.Empty;
           
            Language1 = string.Empty;
            Language2 = string.Empty;
            Language3 = string.Empty;

            HS_Parent = string.Empty;
            EHS_Parent = string.Empty;
            Daycare_Parent = string.Empty;
            Date_Hired = string.Empty;
            Years_in_POS = string.Empty;
            Replace_SM = string.Empty;
            POS_Filled = string.Empty;
            Workfor_HS = string.Empty;
            Workfor_EHS = string.Empty;
            Workfor_CONT = string.Empty;
            Workfor_VOL = string.Empty;
            Position_Data = string.Empty;

            POS_CTG = string.Empty;
            Education = string.Empty;
            Ethnicity = string.Empty;
            Race = string.Empty;
            Weeks_Worked = string.Empty;
            Base_Rate = string.Empty;
            Salary = string.Empty;
            Anual_Salary = string.Empty;
            Employment_Type = string.Empty;
            HRS_Worked_PW = string.Empty;

            Date_Terminated = string.Empty;
            RES_Terminated = string.Empty;
            Site = string.Empty;
            Workfor_NONHS = string.Empty;
            Edu_Progress = string.Empty;
            Date_Acquired = string.Empty;
            Transerfer_Date = string.Empty;
            Application = string.Empty;
            Date_LSTC = string.Empty;
            LSTC_Operator = string.Empty;
            Date_ADD = string.Empty;
            ADD_Operator = string.Empty;
        }

        public STAFFMSTEntity(bool Initialize)
        {
            Rec_Type = null;
            Agency = null;
            Dept = null;
            Prog = null;
            Staff_Code = null;
            Active = null;
            Last_Name = null;
            First_Name = null;
            Middle_Name = null;
            State = null;
            City = null;
            Street = null;
            Suffix = null;
            HNo = null;
            Apt = null;
            Floor = null;
            Zip = null;
            Zip_Plus = null;
            Language = null;
            
            Language1 = null;
            Language2 = null;
            Language3 = null;

            HS_Parent = null;
            EHS_Parent = null;
            Daycare_Parent = null;
            Date_Hired = null;
            Years_in_POS = null;
            Replace_SM = null;
            POS_Filled = null;
            Workfor_HS = null;
            Workfor_EHS = null;
            Workfor_CONT = null;
            Workfor_VOL = null;
            Position_Data = null;

            POS_CTG = null;
            Education = null;
            Ethnicity = null;
            Race = null;
            Weeks_Worked = null;
            Base_Rate = null;
            Salary = null;
            Anual_Salary = null;
            Employment_Type = null;
            HRS_Worked_PW = null;

            Date_Terminated = null;
            RES_Terminated = null;
            Site = null;
            Workfor_NONHS = null;
            Edu_Progress = null;
            Date_Acquired = null;
            Transerfer_Date = null;
            Application = null;
            Date_LSTC = null;
            LSTC_Operator = null;
            Date_ADD = null;
            ADD_Operator = null;
        }

        public STAFFMSTEntity(DataRow CASESPM2)
        {
            if (CASESPM2 != null)
            {
                DataRow row = CASESPM2;
                Rec_Type = "U";
                Agency = row["STF_AGENCY"].ToString();
                Dept = row["STF_DEPT"].ToString();
                Prog = row["STF_PROGRAM"].ToString();
                Staff_Code = row["STF_CODE"].ToString();
                Active = row["STF_ACTIVE"].ToString();
                Last_Name = row["STF_NAME_LAST"].ToString().Trim();
                First_Name = row["STF_NAME_FI"].ToString().Trim();
                Middle_Name = row["STF_NAME_MI"].ToString().Trim();
                State = row["STF_STATE"].ToString();
                City = row["STF_CITY"].ToString();
                Street = row["STF_STREET"].ToString();
                Suffix = row["STF_SUFFIX"].ToString();
                HNo = row["STF_HN"].ToString();
                Apt = row["STF_APT"].ToString();
                Floor = row["STF_FLR"].ToString();
                Zip = row["STF_ZIP"].ToString();
                Zip_Plus = row["STF_ZIPPLUS"].ToString();
                Language = row["STF_LANGUAGE"].ToString();               
                Language1 = row["STF_ADL_LANGUAGE1"].ToString();
                Language2 = row["STF_ADL_LANGUAGE2"].ToString();
                Language3 = row["STF_ADL_LANGUAGE3"].ToString();

                HS_Parent = row["STF_HS_PARENT"].ToString();
                EHS_Parent = row["STF_EHS_PARENT"].ToString();
                Daycare_Parent = row["STF_DAYCARE_PARENT"].ToString();
                Date_Hired = row["STF_DATE_HIRED"].ToString();
                Years_in_POS = row["STF_YEARS_IN_POS"].ToString();
                Replace_SM = row["STF_REPLACE_SM"].ToString();
                POS_Filled = row["STF_POS_FILLED"].ToString();
                Workfor_HS = row["STF_WORKFOR_HS"].ToString();
                Workfor_EHS = row["STF_WORKFOR_EHS"].ToString();
                Workfor_CONT = row["STF_WORKFOR_CONT"].ToString();
                Workfor_VOL = row["STF_WORKFOR_VOL"].ToString();
                Position_Data = row["STF_POSITIONS"].ToString();
                POS_CTG = row["STF_POS_CTG"].ToString();
                Education = row["STF_EDUCATION"].ToString();
                Ethnicity = row["STF_ETHNICITY"].ToString();
                Race = row["STF_RACE"].ToString();
                Weeks_Worked = row["STF_WEEKS_WORKED"].ToString();
                Base_Rate = row["STF_BASE_RATE"].ToString();
                Salary = row["STF_SALARY"].ToString();
                Anual_Salary = row["STF_ANUAL_SALARY"].ToString();
                Employment_Type = row["STF_EMPLOYMENT_TYPE"].ToString();
                HRS_Worked_PW = row["STF_HRS_WORKED_PW"].ToString();
                Date_Terminated = row["STF_DATE_TERMINATED"].ToString();
                RES_Terminated = row["STF_RES_TERMINATION"].ToString();
                Site = row["STF_SITE"].ToString();
                Workfor_NONHS = row["STF_WORKFOR_NONHS"].ToString();
                Edu_Progress = row["STF_EDUCATION_PROGRESS"].ToString();
                Date_Acquired = row["STF_DATE_ACQUIRED"].ToString();
                Transerfer_Date = row["STF_TRANSFER_DATE"].ToString();
                Application = row["STF_APPLICATION"].ToString();
                Date_LSTC = row["STF_DATE_LSTC"].ToString();
                LSTC_Operator = row["STF_LSTC_OPERATOR"].ToString();
                Date_ADD = row["STF_DATE_ADD"].ToString();
                ADD_Operator = row["STF_ADD_OPERATOR"].ToString();
            }
        }


        #endregion

        #region Properties

        public string Rec_Type { get; set; }
        public string Agency { get; set; }
        public string Dept { get; set; }
        public string Prog { get; set; }
        public string Staff_Code { get; set; }
        public string Active { get; set; }
        public string Last_Name { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Suffix { get; set; }
        public string HNo { get; set; }
        public string Apt { get; set; }
        public string Floor { get; set; }
        public string Zip { get; set; }
        public string Zip_Plus { get; set; }
        public string Language { get; set; }        
        public string Language1 { get; set; }
        public string Language2 { get; set; }
        public string Language3 { get; set; }
        public string HS_Parent { get; set; }
        public string EHS_Parent { get; set; }
        public string Daycare_Parent { get; set; }
        public string Date_Hired { get; set; }
        public string Years_in_POS { get; set; }
        public string Replace_SM { get; set; }
        public string POS_Filled { get; set; }
        public string Workfor_HS { get; set; }
        public string Workfor_EHS { get; set; }
        public string Workfor_CONT { get; set; }
        public string Workfor_VOL { get; set; }
        public string Position_Data { get; set; }
        public string POS_CTG { get; set; }
        public string Education { get; set; }
        public string Ethnicity { get; set; }
        public string Race { get; set; }
        public string Weeks_Worked { get; set; }
        public string Base_Rate { get; set; }
        public string Salary { get; set; }
        public string Anual_Salary { get; set; }
        public string Employment_Type { get; set; }
        public string HRS_Worked_PW { get; set; }
        public string Date_Terminated { get; set; }
        public string RES_Terminated { get; set; }
        public string Site { get; set; }
        public string Workfor_NONHS { get; set; }
        public string Edu_Progress { get; set; }
        public string Date_Acquired { get; set; }
        public string Transerfer_Date { get; set; }
        public string Application { get; set; }
        public string Date_LSTC { get; set; }
        public string LSTC_Operator { get; set; }
        public string Date_ADD { get; set; }
        public string ADD_Operator { get; set; }
        public string Mode { get; set; }
        #endregion
    }


    public class STAFFPostEntity
    {
        #region Constructors

        public STAFFPostEntity()
        {
            Rec_Type = string.Empty;
            Agency = string.Empty;
            Dept = string.Empty;
            Prog = string.Empty;
            Staff_Code = string.Empty;
            Category = string.Empty;
            Seq = string.Empty;
            Provider = string.Empty;
            CourseTitle = string.Empty;
            DateCompleted = string.Empty;
            CollegeCredits = string.Empty;
            CeuCredits = string.Empty;
            ClockHours = string.Empty;           
            Date_LSTC = string.Empty;
            LSTC_Operator = string.Empty;
            Date_ADD = string.Empty;
            ADD_Operator = string.Empty;
            Location = string.Empty;
            Sponsor = string.Empty;
            Presenter = string.Empty;
            TotalCost = string.Empty;
        }

        public STAFFPostEntity(bool Initialize)
        {
            Rec_Type = null;
            Agency = null;
            Dept = null;
            Prog = null;
            Staff_Code = null;
            Category = null;
            Seq = null;
            Provider = null;
            CourseTitle = null;
            DateCompleted = null;
            CollegeCredits = null;
            CeuCredits = null;
            ClockHours = null;           
            Date_LSTC = null;
            LSTC_Operator = null;
            Date_ADD = null;
            ADD_Operator = null;
            Location = null;
            Sponsor = null;
            Presenter = null;
            TotalCost = null;
        }

        public STAFFPostEntity(DataRow CASESPM2)
        {
            if (CASESPM2 != null)
            {
                DataRow row = CASESPM2;
                Rec_Type = "U";
                Agency = row["STP_AGENCY"].ToString();
                Dept = row["STP_DEPT"].ToString();
                Prog = row["STP_PROGRAM"].ToString();
                Staff_Code = row["STP_CODE"].ToString();
                Category = row["STP_CATEGORY"].ToString();
                Seq = row["STP_SEQ"].ToString().Trim();
                Provider = row["STP_PROVIDER"].ToString().Trim();
                CourseTitle = row["STP_COURSE_TITLE"].ToString().Trim();
                DateCompleted = row["STP_DATE_COMPLETED"].ToString();
                CollegeCredits = row["STP_COLLEGE_CREDITS"].ToString();
                CeuCredits = row["STP_CEU_CREDITS"].ToString();
                ClockHours = row["STP_CLOCK_HOURS"].ToString();

                Date_LSTC = row["STP_DATE_LSTC"].ToString();
                LSTC_Operator = row["STP_LSTC_OPERATOR"].ToString();
                Date_ADD = row["STP_DATE_ADD"].ToString();
                ADD_Operator = row["STP_ADD_OPERATOR"].ToString();

                Location = row["STP_LOCATION"].ToString();
                Sponsor = row["STP_SPONSOR"].ToString();
                Presenter = row["STP_PRESENTER"].ToString();
                TotalCost = row["STP_TOTALCOST"].ToString();
            }
        }


        #endregion

        #region Properties

        public string Rec_Type { get; set; }
        public string Agency { get; set; }
        public string Dept { get; set; }
        public string Prog { get; set; }
        public string Staff_Code { get; set; }
        public string Category { get; set; }
        public string Seq { get; set; }
        public string Provider { get; set; }
        public string CourseTitle { get; set; }
        public string DateCompleted { get; set; }
        public string CollegeCredits { get; set; }
        public string CeuCredits { get; set; }
        public string ClockHours { get; set; }
        public string Location { get; set; }
        public string Sponsor { get; set; }
        public string Presenter { get; set; }
        public string TotalCost { get; set; }
        public string Date_LSTC { get; set; }
        public string LSTC_Operator { get; set; }
        public string Date_ADD { get; set; }
        public string ADD_Operator { get; set; }
        public string Mode { get; set; }
        #endregion
    }

    





}
