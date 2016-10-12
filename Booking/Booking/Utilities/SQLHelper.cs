using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Booking.Utilities
{
    public class DBHelper
    {
        /// <summary>
        /// 获取数据库引用类库类型
        /// </summary>
        private static string dbProviderName = "System.Data.SqlClient";
        /// <summary>
        /// 获取数据库驱动
        /// </summary>
        private static string dbConnectionString = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;

        private DbConnection connection;

        /// <summary>
        /// 构造函数，创建数据库连接
        /// </summary>
        public DBHelper()
        {
            this.connection = CreateConnection(DBHelper.dbConnectionString);
        }

        /// <summary>
        /// 构造函数重载，创建数据库连接
        /// </summary>
        /// <param name="connectionString">数据库驱动</param>
        public DBHelper(string connectionString)
        {
            this.connection = CreateConnection(connectionString);
        }

        /// <summary>
        /// 构造函数调用，通过工厂创建数据库连接
        /// </summary>
        /// <returns></returns>
        public static DbConnection CreateConnection()
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DBHelper.dbProviderName);
            DbConnection dbconn = dbfactory.CreateConnection();
            dbconn.ConnectionString = DBHelper.dbConnectionString;
            return dbconn;
        }

        /// <summary>
        /// 构造函数重载调用，通过工厂创建数据库连接
        /// </summary>
        /// <param name="connectionString">数据库驱动</param>
        /// <returns></returns>
        public static DbConnection CreateConnection(string connectionString)
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DBHelper.dbProviderName);
            DbConnection dbconn = dbfactory.CreateConnection();
            dbconn.ConnectionString = connectionString;
            return dbconn;
        }

        /// <summary>
        /// 设置数据库操作源为存储过程
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <returns></returns>
        public DbCommand GetStoredProcCommond(string storedProcedure)
        {
            DbCommand dbCommand = connection.CreateCommand();
            dbCommand.CommandText = storedProcedure;
            dbCommand.CommandType = CommandType.StoredProcedure;
            return dbCommand;
        }

        /// <summary>
        /// 设置数据库操作源为SQL语句
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public DbCommand GetSqlStringCommond(string sqlQuery)
        {
            DbCommand dbCommand = connection.CreateCommand();
            dbCommand.CommandText = sqlQuery;
            dbCommand.CommandType = CommandType.Text;
            return dbCommand;
        }

        #region 增加参数
        /// <summary>
        /// 添加参考集合DbParameterCollection
        /// </summary>
        /// <param name="cmd">DbCommand</param>
        /// <param name="dbParameterCollection">DbParameterCollection</param>
        public void AddParameterCollection(DbCommand cmd, DbParameterCollection dbParameterCollection)
        {
            foreach (DbParameter dbParameter in dbParameterCollection)
            {
                cmd.Parameters.Add(dbParameter);
            }
        }

        public void AddSqlParenterCollection(DbCommand cmd, SqlParameter param)
        {
            cmd.Parameters.Add(param);
        }

        public void AddOutParameter(DbCommand cmd, string parameterName, DbType dbType, int size)
        {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Size = size;
            dbParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(dbParameter);
        }

        public void AddInParameter(DbCommand cmd, string parameterName, DbType dbType, object value)
        {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(dbParameter);
        }

        /// <summary>
        /// 设置输入存储过程参数集合
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="values"></param>
        public void AddInParameter(DbCommand cmd, params SqlParameter[] values)
        {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            cmd.Parameters.AddRange(values);
        }

        public void AddReturnParameter(DbCommand cmd, string parameterName, DbType dbType)
        {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(dbParameter);
        }

        public DbParameter GetParameter(DbCommand cmd, string parameterName)
        {
            return cmd.Parameters[parameterName];
        }
        #endregion

        #region 执行

        public DataSet ExecuteDataSet(DbCommand cmd)
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DBHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            dbDataAdapter.Fill(ds);
            return ds;
        }

        public DataSet ExecuteDataSet(DbCommand cmd, params SqlParameter[] param)
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DBHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            AddInParameter(cmd, param);
            dbDataAdapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            dbDataAdapter.Fill(ds);
            return ds;
        }

        public DataTable ExecuteDataTable(DbCommand cmd)
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DBHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataTable dataTable = new DataTable();
            dbDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public DataTable ExecuteDataTable(DbCommand cmd, params SqlParameter[] param)
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DBHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            AddInParameter(cmd, param);
            dbDataAdapter.SelectCommand = cmd;
            DataTable dataTable = new DataTable();
            dbDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public DbDataReader ExecuteReader(DbCommand cmd)
        {
            cmd.Connection.Open();
            DbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            reader.Close();
            return reader;
        }

        public DbDataReader ExecuteReader(DbCommand cmd, params SqlParameter[] param)
        {
            AddInParameter(cmd, param);
            cmd.Connection.Open();
            DbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            reader.Close();
            return reader;
        }

        public int ExecuteNonQuery(DbCommand cmd)
        {
            cmd.Connection.Open();
            int ret = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return ret;
        }

        /// <summary>
        /// 事务处理T-SQL
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbCommand cmd, params SqlParameter[] param)
        {
            AddInParameter(cmd, param);
            cmd.Connection.Open();
            int ret = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return ret;
        }

        public object ExecuteScalar(DbCommand cmd)
        {
            cmd.Connection.Open();
            object ret = cmd.ExecuteScalar();
            cmd.Connection.Close();
            return ret;
        }

        public object ExecuteScalar(DbCommand cmd, params SqlParameter[] param)
        {
            AddInParameter(cmd, param);
            cmd.Connection.Open();
            object ret = cmd.ExecuteScalar();
            cmd.Connection.Close();
            return ret;
        }

        #endregion

        #region 执行事务

        public DataSet ExecuteDataSet(DbCommand cmd, Trans t)
        {
            cmd.Connection = t.DbConnection;
            cmd.Transaction = t.DbTrans;
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DBHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            dbDataAdapter.Fill(ds);
            return ds;
        }

        public DataTable ExecuteDataTable(DbCommand cmd, Trans t)
        {
            cmd.Connection = t.DbConnection;
            cmd.Transaction = t.DbTrans;
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DBHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataTable dataTable = new DataTable();
            dbDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public DbDataReader ExecuteReader(DbCommand cmd, Trans t)
        {
            cmd.Connection.Close();
            cmd.Connection = t.DbConnection;
            cmd.Transaction = t.DbTrans;
            DbDataReader reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            return reader;
        }

        public int ExecuteNonQuery(DbCommand cmd, Trans t, params SqlParameter[] param)
        {
            AddInParameter(cmd, param);
            cmd.Connection.Close();
            cmd.Connection = t.DbConnection;
            cmd.Transaction = t.DbTrans;
            int ret = cmd.ExecuteNonQuery();
            return ret;
        }

        public object ExecuteScalar(DbCommand cmd, Trans t)
        {
            cmd.Connection.Close();
            cmd.Connection = t.DbConnection;
            cmd.Transaction = t.DbTrans;
            object ret = cmd.ExecuteScalar();
            return ret;
        }

        #endregion
    }

    public class Trans : IDisposable
    {

        private DbConnection conn;

        private DbTransaction dbTrans;

        public DbConnection DbConnection
        {

            get { return this.conn; }

        }

        public DbTransaction DbTrans
        {

            get { return this.dbTrans; }

        }



        public Trans()
        {
            conn.Open();
            dbTrans = conn.BeginTransaction();
        }

        public Trans(string connectionString)
        {

            conn = DBHelper.CreateConnection(connectionString);

            conn.Open();

            dbTrans = conn.BeginTransaction();

        }

        public void Commit()
        {

            dbTrans.Commit();

            this.Colse();

        }



        public void RollBack()
        {

            dbTrans.Rollback();

            this.Colse();

        }



        public void Dispose()
        {

            this.Colse();

        }

        public void Colse()
        {

            if (conn.State == System.Data.ConnectionState.Open)
            {

                conn.Close();

            }

        }
    }
}