using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;

namespace KoalaGo.Data
{
    public class DataManager
    {
        public void addFire(String lat, String lon, String date1, String alerttype, String status, String details, String state)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            var db = factory.Create("koalaCon");
            string sql = "sp_add_fires";
            // "ListAll_Districts";
            DbCommand dbCommand = db.GetStoredProcCommand(sql);

            db.AddInParameter(dbCommand, "@lat", DbType.String, lat);
            db.AddInParameter(dbCommand, "@lon", DbType.String, lon);
            db.AddInParameter(dbCommand, "@date", DbType.String, date1);
            db.AddInParameter(dbCommand, "@alerttype", DbType.String, alerttype);
            db.AddInParameter(dbCommand, "@status", DbType.String, status);
            db.AddInParameter(dbCommand, "@details", DbType.String, details);
            db.AddInParameter(dbCommand, "@state", DbType.String, state);

            using (DbConnection conn = db.CreateConnection())
            {
                DataTable table = new DataTable();
                try
                {
                    db.ExecuteNonQuery(dbCommand);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}