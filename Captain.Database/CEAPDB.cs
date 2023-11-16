using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Captain.DatabaseLayer
{
    [DataObject]
    [Serializable]
    public partial class CEAPDB
    {

        #region Constants
        //private static readonly string TABLE_NAME = "[dbo].[AGYTAB]";
        private static Database _dbFactory = DatabaseFactory.CreateDatabase();
        private static DbCommand _dbCommand;
        #endregion
            
      
        [DataObjectMethodAttribute(DataObjectMethodType.Select, false)]
        public static DataSet CAPS_CEAPB002_GET(string agency, string dep, string program, string year,  string Site, string Worker, string Fromdate, string Todate, string strMode)
        {
            DataSet ds;
            Database db;
            string sqlCommand;
            DbCommand dbCommand;

            db = DatabaseFactory.CreateDatabase();
            sqlCommand = "[dbo].[CAPS_CEAPB002_REP]";
            dbCommand = db.GetStoredProcCommand(sqlCommand);
            dbCommand.CommandTimeout = 1800;

            if (agency != string.Empty)
            {
                SqlParameter empnoParm = new SqlParameter("@AGENCY", agency);
                dbCommand.Parameters.Add(empnoParm);
            }
            if (dep != string.Empty)
            {
                SqlParameter empnoParm = new SqlParameter("@DEPT", dep);
                dbCommand.Parameters.Add(empnoParm);
            }
            if (program != string.Empty)
            {
                SqlParameter empnoParm = new SqlParameter("@PROGRAM", program);
                dbCommand.Parameters.Add(empnoParm);
            }
            if (year.Trim() != string.Empty)
            {
                SqlParameter empnoParm = new SqlParameter("@YEAR", year);
                dbCommand.Parameters.Add(empnoParm);
            }
          
            if (Site.Trim() != string.Empty)
            {
                SqlParameter empnoParm = new SqlParameter("@Site", Site);
                dbCommand.Parameters.Add(empnoParm);
            }
            if (Worker.Trim() != string.Empty)
            {
                SqlParameter empnoParm = new SqlParameter("@CASEWORKER", Worker);
                dbCommand.Parameters.Add(empnoParm);
            }
            if (Fromdate.Trim() != string.Empty)
            {
                SqlParameter empnoParm = new SqlParameter("@FromDate", Fromdate);
                dbCommand.Parameters.Add(empnoParm);
            }
            if (Todate.Trim() != string.Empty)
            {
                SqlParameter empnoParm = new SqlParameter("@ToDate", Todate);
                dbCommand.Parameters.Add(empnoParm);
            }  

            if (strMode.Trim() != string.Empty)
            {
                SqlParameter empnoParm = new SqlParameter("@Mode", strMode);
                dbCommand.Parameters.Add(empnoParm);
            }

            ds = db.ExecuteDataSet(dbCommand);
            return ds;
        }


    }
}
