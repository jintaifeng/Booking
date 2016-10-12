using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text;
using System.Reflection;

namespace Juppiphis.Data
{
    public static class SqlHelper
    {
        #region
        [ThreadStatic]
        private static int _commandTimeoutInCurrentThread = -1;

        /// <summary>
        /// Get or set the all values of IbBCommand.CommandTimeout in current thread. 
        /// While this value is -1, the default value of CommandTimeout of IDbCommand will be used.
        /// Should clear this value (set this value to -1 to clear) whild current thread finished if current thread is in the thread pool.
        /// </summary>
        public static int CommandTimeoutInCurrentThread
        {
            get { return _commandTimeoutInCurrentThread; }
            set { _commandTimeoutInCurrentThread = value < 0 ? -1 : value; }
        }
        #endregion

        #region private utility methods & constructors

        #region AttachParameters
        /// <summary>
        /// This method is used to attach array of SqlParameters to a SqlCommand.
        /// 
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// 
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of SqlParameters to be added to command</param>
        private static void AttachParameters(IDbCommand command, IDbDataParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (IDbDataParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }
        #endregion AttachParameters

        #region AssignParameterValues
        /// <summary>
        /// This method assigns dataRow column values to an array of SqlParameters
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values</param>
        private static void AssignParameterValues(IDbDataParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                // Do nothing if we get no data
                return;
            }

            int i = 0;
            // Set the parameters values
            foreach (IDbDataParameter commandParameter in commandParameters)
            {
                // Check the parameter name
                if (commandParameter.ParameterName == null ||
                    commandParameter.ParameterName.Length <= 1)
                    throw new Exception(
                        string.Format(
                            "Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.",
                            i,
                            commandParameter.ParameterName));
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                i++;
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of SqlParameters
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
        /// <param name="parameterValues">Array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(IDbDataParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                // Do nothing if we get no data
                return;
            }

            // We must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            // Iterate through the SqlParameters, assigning the values from the corresponding position in the 
            // value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property
                if (parameterValues[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }
        #endregion AssignParameterValues

        #region PrepareCommand
        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command
        /// </summary>
        /// <param name="command">The SqlCommand to be prepared</param>
        /// <param name="connection">A valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">A valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
        private static void PrepareCommand(
            IDbCommand command, 
            IDbConnection connection, 
            IDbTransaction transaction, 
            CommandType commandType, 
            string commandText,
            IDbDataParameter[] commandParameters, 
            out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            if (_commandTimeoutInCurrentThread >= 0)
                command.CommandTimeout = _commandTimeoutInCurrentThread;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // Associate the connection with the command
            command.Connection = connection;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null && commandParameters.Length != 0)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }
        #endregion PrepareCommand

        #region BuildFieldList
        private static string BuildFieldList(DataTable table, bool onlyModified)
        {
            IDictionary dict = (table.Columns.Count > 10) ? new ListDictionary() as IDictionary : new Hashtable(table.Columns.Count);

            StringBuilder sb = new StringBuilder();

            if (onlyModified)
            {

                foreach (DataRow row in table.Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        foreach (DataColumn col in table.Columns)
                        {
                            dict[col.ColumnName.ToLower()] = col.ColumnName;
                        }
                        break;
                    }
                    else if (row.RowState == DataRowState.Modified)
                    {
                        foreach (DataColumn col in table.Columns)
                        {
                            if (!row[col, DataRowVersion.Current].Equals(row[col, DataRowVersion.Original]))
                                dict[col.ColumnName.ToLower()] = col.ColumnName;
                        }
                    }
                }
            }
            else
            {
                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName.ToLower()] = col.ColumnName;
                }
            }

            DataColumn[] primaryKeys = table.PrimaryKey;
            foreach (DataColumn col in primaryKeys)
            {
                sb.Append(col.ColumnName).Append(',');
                if (dict.Contains(col.ColumnName.ToLower()))
                    dict.Remove(col.ColumnName.ToLower());
            }

            foreach (DictionaryEntry entry in dict)
            {
                sb.Append(entry.Value.ToString()).Append(',');
            }

            if (sb.Length != 0)
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
        #endregion BuildFieldList

        #endregion private utility methods & constructors

        #region CreateTransaction
        /// <summary>
        /// Create a IDbTransaction against the database specified in the connection string 
        /// </summary>
        /// <param name="alias">The alias of a valid connection string for a SqlConnection</param>
        /// <returns>A transaction instance</returns>
        public static IDbTransaction CreateTransaction(string alias)
        {

            if (alias == null || alias.Length == 0) throw new ArgumentNullException("alias");

            IConnectionType connType = ConnectionConfig.GetConnectionType(alias);
            if (connType == null)
                throw new ArgumentException("The connection string has not been initialized whose alias is " + alias);


            IDbConnection conn = connType.CreateConnection();
            conn.Open();
            return new DbTransactionProxy(conn.BeginTransaction());
        }
        #endregion CreateTransaction


        #region ExecuteNonQuery

        #region alias, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="alias">The alias of a valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string alias, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (alias == null || alias.Length == 0) throw new ArgumentNullException("alias");

            IConnectionType connType = ConnectionConfig.GetConnectionType(alias);
            if (connType == null)
                throw new ArgumentException("The connection string has not been initialized whose alias is " + alias);

            // Create & open a SqlConnection, and dispose of it after we are done
            using (IDbConnection connection = connType.CreateConnection())
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }
        #endregion alias, commandType, commandText, commandParameters

        #region connection, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(IDbConnection connection, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            IDbCommand cmd = connection.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (IDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Finally, execute the command
            int retval = cmd.ExecuteNonQuery();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }
        #endregion connection, commandType, commandText, commandParameters

        #region transaction, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid SqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(IDbTransaction transaction, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            IDbCommand cmd = transaction.Connection.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Finally, execute the command
            int retval = cmd.ExecuteNonQuery();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }
        #endregion transaction, commandType, commandText, commandParameters


        #endregion ExecuteNonQuery

        #region ExecuteReader

        #region enum ConnectionOwnership
        /// <summary>
        /// This enum is used to indicate whether the connection was provided by the caller, or created by SqlHelper, so that
        /// we can set the appropriate CommandBehavior when calling ExecuteReader()
        /// </summary>
        private enum ConnectionOwnership
        {
            /// <summary>Connection is owned and managed by SqlHelper</summary>
            Internal,

            /// <summary>Connection is owned and managed by the caller</summary>
            External
        }
        #endregion enum ConnectionOwnership

        #region connection,  transaction, commandType, commandText, commandParameters, connectionOwnership
        /// <summary>
        /// Create and prepare a SqlCommand, and call ExecuteReader with the appropriate CommandBehavior.
        /// </summary>
        /// <remarks>
        /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        /// 
        /// If the caller provided the connection, we want to leave it to them to manage.
        /// </remarks>
        /// <param name="connection">A valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">A valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="connectionOwnership">Indicates whether the connection parameter was provided by the caller, or created by SqlHelper</param>
        /// <returns>SqlDataReader containing the results of the command</returns>
        private static IDataReader ExecuteReader(IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, IDbDataParameter[] commandParameters, ConnectionOwnership connectionOwnership)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            bool mustCloseConnection = false;
            // Create a command and prepare it for execution
            IDbCommand cmd = connection.CreateCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

                // Create a reader
                IDataReader dataReader;

                // Call ExecuteReader with the appropriate CommandBehavior
                if (connectionOwnership == ConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

                // Detach the SqlParameters from the command object, so they can be used again.
                // HACK: There is a problem here, the output parameter values are fletched 
                // when the reader is closed, so if the parameters are detached from the command
                // then the SqlReader can set its values. 
                // When this happen, the parameters can be used again in other command.
                bool canClear = true;
                foreach (IDbDataParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                }

                if (canClear)
                {
                    cmd.Parameters.Clear();
                }

                return dataReader;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }
        #endregion connection,  transaction, commandType, commandText, commandParameters, connectionOwnership

        #region alias, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="alias">The alias of a valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A SqlDataReader containing the resultset generated by the command</returns>
        public static IDataReader ExecuteReader(string alias, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (alias == null || alias.Length == 0) throw new ArgumentNullException("alias");

            IConnectionType connType = ConnectionConfig.GetConnectionType(alias);
            if (connType == null)
                throw new ArgumentException("The connection string has not been initialized whose alias is " + alias);

            // Create & open a SqlConnection, and dispose of it after we are done
            IDbConnection connection = null;
            try
            {
                connection = connType.CreateConnection();
                connection.Open();

                // Call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(connection, null, commandType, commandText, commandParameters, ConnectionOwnership.Internal);
            }
            catch
            {
                // If we fail to return the SqlDatReader, we need to close the connection ourselves
                if (connection != null) connection.Close();
                throw;
            }

        }
        #endregion alias, commandType, commandText, commandParameters

        #region connection, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A SqlDataReader containing the resultset generated by the command</returns>
        public static IDataReader ExecuteReader(IDbConnection connection, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            // Pass through the call to the private overload using a null transaction value and an externally owned connection
            return ExecuteReader(connection, (IDbTransaction)null, commandType, commandText, commandParameters, ConnectionOwnership.External);
        }
        #endregion connection, commandType, commandText, commandParameters

        #region transaction, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///   SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid SqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A SqlDataReader containing the resultset generated by the command</returns>
        public static IDataReader ExecuteReader(IDbTransaction transaction, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Pass through to private overload, indicating that the connection is owned by the caller
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, ConnectionOwnership.External);
        }
        #endregion transaction, commandType, commandText, commandParameters

        #endregion ExecuteReader

        #region ExecuteScalar

        #region alias, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="alias">The alias of a valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string alias, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (alias == null || alias.Length == 0) throw new ArgumentNullException("alias");

            IConnectionType connType = ConnectionConfig.GetConnectionType(alias);
            if (connType == null)
                throw new ArgumentException("The connection string has not been initialized whose alias is " + alias);

            // Create & open a SqlConnection, and dispose of it after we are done
            using (IDbConnection connection = connType.CreateConnection())
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }
        #endregion alias, commandType, commandText, commandParameters

        #region connection, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(IDbConnection connection, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            IDbCommand cmd = connection.CreateCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (IDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Execute the command & return the results
            object retval = cmd.ExecuteScalar();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            if (mustCloseConnection)
                connection.Close();

            return retval;
        }
        #endregion connection, commandType, commandText, commandParameters

        #region transaction, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid SqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(IDbTransaction transaction, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            IDbCommand cmd = transaction.Connection.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Execute the command & return the results
            object retval = cmd.ExecuteScalar();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }
        #endregion transaction, commandType, commandText, commandParameters

        #endregion ExecuteScalar

        #region ExecuteFillTable

        #region table, alias, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  ExecuteFillTable(alias, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="table">a valid table to fill in</param>
        /// <param name="alias">alias of a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteFillTable(DataTable table, string alias, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (alias == null || alias.Length == 0) throw new ArgumentNullException("alias");

            IConnectionType connType = ConnectionConfig.GetConnectionType(alias);
            if (connType == null)
                throw new ArgumentException("The connection string has not been initialized whose alias is " + alias);

            // Create & open a SqlConnection, and dispose of it after we are done
            using (IDbConnection connection = connType.CreateConnection())
            {
                connection.Open();

                // call the overload that takes a connection in place of the connection string
                return ExecuteFillTable(table, connection, commandType, commandText, commandParameters);
            }
        }
        #endregion table, alias, commandType, commandText, commandParameters

        #region table, alias, commandType, commandText, startRecord, maxRecords, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  ExecuteFillTable(alias, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="table">a valid table to fill in</param>
        /// <param name="alias">alias of a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="startRecord">The zero-based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrieve</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteFillTable(
            DataTable table, 
            string alias,
            CommandType commandType,
            string commandText, 
            int startRecord,
            int maxRecords,
            params IDbDataParameter[] commandParameters)
        {
            if (alias == null || alias.Length == 0) throw new ArgumentNullException("alias");

            IConnectionType connType = ConnectionConfig.GetConnectionType(alias);
            if (connType == null)
                throw new ArgumentException("The connection string has not been initialized whose alias is " + alias);

            // create a command and prepare it for execution
            using (IDbConnection connection = connType.CreateConnection())
            {
                IDbCommand cmd = connection.CreateCommand();

                bool mustCloseConnection = false;
                PrepareCommand(cmd, connection, (IDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

                int count = 0;
                using (DbDataAdapter da = connType.CreateDataAdapter())
                {
                    ((IDbDataAdapter)da).SelectCommand = cmd;
                    //da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    count = da.Fill(startRecord, maxRecords, table);
                }

                // fill this table
                return count;
            }
        }
        #endregion table, alias, commandType, commandText, startRecord, maxRecords, commandParameters

        #region table, connection, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  ExecuteFillTable(DataTable table, conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="table">a valid table to fill in</param>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteFillTable(DataTable table, IDbConnection connection, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            IConnectionType connType = ConnectionConfig.GetConnectionType(connection);
            if (connType == null)
                throw new ArgumentException("The type of connection has not been initialized whose name is " + connection.GetType().FullName);

            // create a command and prepare it for execution
            IDbCommand cmd = connection.CreateCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (IDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            int count = 0;
            // Create the DataAdapter & DataSet
            using (DbDataAdapter da = connType.CreateDataAdapter())
            {
                ((IDbDataAdapter)da).SelectCommand = cmd;
                //da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                count = da.Fill(table);
                
                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();
            }

            if (mustCloseConnection)
                connection.Close();

            // fill this table
            return count;
        }
        #endregion table, connection, commandType, commandText, commandParameters

        #region table, transaction, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection and SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  ExecuteFillTable(DataTable table, conn, trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="table">a valid table to fill in</param>
        /// <param name="transaction">a valid SqlTransaction associated with the connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteFillTable(DataTable table, IDbTransaction transaction, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");

            IDbConnection connection = transaction.Connection;

            IConnectionType connType = ConnectionConfig.GetConnectionType(connection);
            if (connType == null)
                throw new ArgumentException("The type of connection has not been initialized whose name is " + connection.GetType().FullName);

            // Create a command and prepare it for execution
            IDbCommand cmd = connection.CreateCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            int count = 0;
            // Create the DataAdapter & DataSet
            using (DbDataAdapter da = connType.CreateDataAdapter())
            {
                ((IDbDataAdapter)da).SelectCommand = cmd;
                //da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                count = da.Fill(table);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();
            }

            if (mustCloseConnection)
                connection.Close();

            // Fill this table
            return count;
        }
        #endregion table, transaction, commandType, commandText, commandParameters

        #endregion ExecuteFillTable

        #region ExecuteDataset

        #region alias, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="alias">alias of a valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string alias, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (alias == null || alias.Length == 0) throw new ArgumentNullException("alias");

            IConnectionType connType = ConnectionConfig.GetConnectionType(alias);
            if (connType == null)
                throw new ArgumentException("The connection string has not been initialized whose alias is " + alias);

            // Create & open a SqlConnection, and dispose of it after we are done
            using (IDbConnection connection = connType.CreateConnection())
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }
        #endregion alias, commandType, commandText, commandParameters

        #region connection, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(IDbConnection connection, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            IConnectionType connType = ConnectionConfig.GetConnectionType(connection);
            if (connType == null)
                throw new ArgumentException("The type of connection has not been initialized whose name is " + connection.GetType().FullName);

            IDbCommand cmd = connection.CreateCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (IDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (DbDataAdapter da = connType.CreateDataAdapter())
            {
                ((IDbDataAdapter)da).SelectCommand = cmd;
                //da.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                DataSet ds = new DataSet();

                da.Fill(ds);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                if (mustCloseConnection)
                    connection.Close();

                return ds;
            }
        }
        #endregion connection, commandType, commandText, commandParameters

        #region transaction, commandType, commandText, commandParameters
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid SqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(IDbTransaction transaction, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            IDbConnection connection = transaction.Connection;

            IConnectionType connType = ConnectionConfig.GetConnectionType(connection);
            if (connType == null)
                throw new ArgumentException("The type of connection has not been initialized whose name is " + connection.GetType().FullName);

            IDbCommand cmd = connection.CreateCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (DbDataAdapter da = connType.CreateDataAdapter())
            {
                ((IDbDataAdapter)da).SelectCommand = cmd;
                //da.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                DataSet ds = new DataSet();

                da.Fill(ds);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                return ds;
            }
        }
        #endregion transaction, commandType, commandText, commandParameters

        #endregion ExecuteDataset

        #region UpdateDataTable

        #region alias, table
        /// <summary>
        /// Executes the respective command for each inserted, updated, or deleted row in the DataSet.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order");
        /// </remarks>
        /// <param name="alias">alias of a valid connection string for a SqlConnection</param>
        /// <param name="insertCommand">A valid transact-SQL statement or stored procedure to insert new records into the data source</param>
        /// <param name="deleteCommand">A valid transact-SQL statement or stored procedure to delete records from the data source</param>
        /// <param name="updateCommand">A valid transact-SQL statement or stored procedure used to update records in the data source</param>
        /// <param name="table">The DataTable used to update the data source</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int UpdateTable(string alias, DataTable table)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (table.PrimaryKey.Length == 0)
                throw new ArgumentException("DataTable " + table.TableName + " has not primary keys", "table");

            IConnectionType connType = ConnectionConfig.GetConnectionType(alias);
            if (connType == null)
                throw new ArgumentException("The connection string has not been initialized whose alias is " + alias);
            
            IDbConnection conn = connType.CreateConnection();
            using (DbDataAdapter da = connType.CreateDataAdapter())
            {
                string fieldList = BuildFieldList(table, true);

                IDbCommand command = connType.CreateCommand("SELECT " + fieldList + " FROM  " + table.TableName);

                if (_commandTimeoutInCurrentThread >= 0)
                    command.CommandTimeout = _commandTimeoutInCurrentThread;

                command.Connection = conn;

                ((IDbDataAdapter)da).SelectCommand = command;

                object commandBuilder = connType.CreateCommandBuilder();
                PropertyInfo daProperty = commandBuilder.GetType().GetProperty("DataAdapter", da.GetType());

                if (daProperty == null)
                    throw new ApplicationException("The CommandBuilder \"" + commandBuilder.GetType().FullName + "\" has not the property DataAdapter");

                daProperty.SetValue(commandBuilder, da, null);
                int count = da.Update(table);
                table.AcceptChanges();

                return count;
            }
        }
        #endregion alias, table

        #region alias, insertCommand, deleteCommand, updateCommand, table
        /// <summary>
        /// Executes the respective command for each inserted, updated, or deleted row in the DataSet.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order");
        /// </remarks>
        /// <param name="alias">alias of a valid connection string for a SqlConnection</param>
        /// <param name="insertCommand">A valid transact-SQL statement or stored procedure to insert new records into the data source</param>
        /// <param name="deleteCommand">A valid transact-SQL statement or stored procedure to delete records from the data source</param>
        /// <param name="updateCommand">A valid transact-SQL statement or stored procedure used to update records in the data source</param>
        /// <param name="table">The DataTable used to update the data source</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int UpdateDataset(string alias, IDbCommand insertCommand, IDbCommand deleteCommand, IDbCommand updateCommand, DataTable table)
        {
            if (insertCommand == null) throw new ArgumentNullException("insertCommand");
            if (deleteCommand == null) throw new ArgumentNullException("deleteCommand");
            if (updateCommand == null) throw new ArgumentNullException("updateCommand");
            if (table == null) throw new ArgumentNullException("table");

            IConnectionType connType = ConnectionConfig.GetConnectionType(alias);
            if (connType == null)
                throw new ArgumentException("The connection string has not been initialized whose alias is " + alias);

            using (DbDataAdapter da = connType.CreateDataAdapter())
            {
                ((IDbDataAdapter)da).UpdateCommand = updateCommand;
                ((IDbDataAdapter)da).InsertCommand = insertCommand;
                ((IDbDataAdapter)da).DeleteCommand = deleteCommand;

                if (_commandTimeoutInCurrentThread >= 0)
                {
                    da.InsertCommand.CommandTimeout = _commandTimeoutInCurrentThread;
                    da.UpdateCommand.CommandTimeout = _commandTimeoutInCurrentThread;
                    da.SelectCommand.CommandTimeout = _commandTimeoutInCurrentThread;
                    da.DeleteCommand.CommandTimeout = _commandTimeoutInCurrentThread;
                }

                int count = da.Update(table);
                table.AcceptChanges();

                return count;
            }
        }
        #endregion alias, insertCommand, deleteCommand, updateCommand, table

        #endregion UpdateDataTable
    }
}
