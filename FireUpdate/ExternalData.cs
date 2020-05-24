using System;
using System.Data;
using System.Data.Common;

namespace KoalaGo.Data
{
    public class ExternalData
    {
        private string _connectionString;
        private string _providerName;
        private DbConnection conn;

        public ExternalData()
        {
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["koalaCon"].ConnectionString;
            this.DBProviderName = System.Configuration.ConfigurationManager.AppSettings["ProviderName"];
        }

        public ExternalData(string connectionString, string dbProviderName)
        {
            this.ConnectionString = connectionString;
            this.DBProviderName = dbProviderName;
        }

        private string ConnectionString
        {
            set { this._connectionString = value; }
            get { return this._connectionString; }
        }

        private string DBProviderName
        {
            set { this._providerName = value; }
            get { return this._providerName; }
        }

        public void RunProcExecuteScalar(string procName, DbParameter[] prams)
        {
            DbCommand cmd = CreateCommand(procName, prams);

            cmd.CommandTimeout = 0;
            cmd.ExecuteScalar();
        }

        public void RunProcSelectQuery(string procName, DbParameter[] prams, out DbDataReader dataReader)
        {
            DbCommand cmd = CreateCommand(procName, prams);
            cmd.CommandTimeout = 0;
            dataReader = cmd.ExecuteReader();
        }

        private DbCommand CreateCommand(string procName, IDataParameter[] prams)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(DBProviderName);
            //DbConnection conn = factory.CreateConnection();
            conn = factory.CreateConnection();
            conn.ConnectionString = ConnectionString;

            DbCommand command = conn.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = procName;

            conn.Open();
            //string s = conn.State.ToString();

            // add proc parameters
            if (prams != null)
            {
                foreach (DbParameter parameter in prams)
                    command.Parameters.Add(parameter);
            }

            DbParameter dbParameter = CreateParameter1();

            dbParameter.ParameterName = "ReturnValue";
            dbParameter.DbType = DbType.Int32;
            dbParameter.Size = 4;
            dbParameter.Direction = ParameterDirection.ReturnValue;
            dbParameter.SourceColumnNullMapping = false;
            dbParameter.SourceColumn = string.Empty;
            dbParameter.SourceVersion = DataRowVersion.Default;

            command.Parameters.Add(dbParameter);

            //s = conn.State.ToString();
            return command;
        }

        public DataTable ExecuteSelectCommandWithoutParameters(string procName)
        {
            DbCommand command = CreateCommand(procName, null);

            DataTable table;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }

                command.CommandTimeout = 0;
                DbDataReader reader = command.ExecuteReader();
                table = new DataTable();
                table.Load(reader);

                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Connection.Close();
            }

            return table;
        }

        public DataTable ExecuteSelectCommand(string storedProcName, IDataParameter[] parameters)
        {
            DbCommand command = CreateCommand(storedProcName, parameters);

            DataTable table;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }

                command.CommandTimeout = 0;
                DbDataReader reader = command.ExecuteReader();
                table = new DataTable();
                table.Load(reader);

                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Connection.Close();
            }

            return table;
        }

        public void ExecuteActionCommand(string storedProcName, IDataParameter[] parameters)
        {
            DbCommand command = CreateCommand(storedProcName, parameters);

            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Connection.Close();
            }
        }

        private DbCommand CreateCommandWithResponse(string storedProcName, IDataParameter[] parameters)
        {
            DbCommand command = CreateCommand(storedProcName, parameters);

            DbParameter dbParameter = null; // = new DbParameter();

            dbParameter.ParameterName = "ReturnValue";
            dbParameter.DbType = DbType.Int32;
            dbParameter.Size = 4;
            dbParameter.Direction = ParameterDirection.ReturnValue;
            dbParameter.SourceColumnNullMapping = false;
            dbParameter.SourceColumn = string.Empty;
            dbParameter.SourceVersion = DataRowVersion.Default;

            command.Parameters.Add(dbParameter);

            return command;
        }

        public DbParameter CreateParameter()
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(DBProviderName);
            DbConnection conn1 = factory.CreateConnection();
            conn1.ConnectionString = ConnectionString;
            DbCommand command = conn1.CreateCommand();
            DbParameter parameter = command.CreateParameter();
            return parameter;
        }

        public DbParameter CreateParameter1()
        {
            string s = conn.State.ToString();
            DbCommand command = conn.CreateCommand();
            s = conn.State.ToString();
            DbParameter parameter = command.CreateParameter();

            s = conn.State.ToString();
            return parameter;
        }

        public void Close()
        {
            if (conn != null)
                conn.Close();
        }

        public DbParameter MakeInParam(string ParamName, DbType dbType, int Size, object Value)
        {
            return MakeParam(ParamName, dbType, Size, ParameterDirection.Input, Value);
        }

        public DbParameter MakeOutParam(string ParamName, DbType dbType, int Size)
        {
            return MakeParam(ParamName, dbType, Size, ParameterDirection.Output, null);
        }

        public DbParameter MakeParam(string ParamName, DbType dbType, Int32 Size,
          ParameterDirection Direction, object Value)
        {
            DbParameter param = CreateParameter();

            if (Size > 0)
            {
                param.Size = Size;
                param.DbType = dbType;
                param.ParameterName = ParamName;
            }
            else
            {
                param.DbType = dbType;
                param.ParameterName = ParamName;
            }

            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
                param.Value = Value;

            return param;
        }
    }
}