using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;

namespace Juppiphis.Data
{
    /// <summary>
    /// 处理SQL语句的数据辅助类
    /// </summary>
    public class CommandHelper
    {
        private IConnectionType _connType;

        public CommandHelper(IConnectionType connType)
        {
            _connType = connType;
        }

        #region ArraySearch 在数组中搜索
        /// <summary>
        /// 在数组中搜索指定的值，返回搜索值的位置。如果未搜索成功，返回-1。
        /// </summary>
        /// <param name="array">数组元素</param>
        /// <param name="value">待搜索的值</param>
        /// <returns>搜索值的位置。如果未搜索成功，返回-1</returns>
        public static int ArraySearch(Array array, object value)
        {
            int i = 0;
            foreach (object o in array)
            {
                if (o == value)
                    return i;
                ++i;
            }
            return -1;
        }
        #endregion ArraySearch

        #region MergeParameter  合并参数


        /// <summary>
        /// 依据SqlParameter的ParamName属性合并两个SqlParameter数组
        /// </summary>
        /// <param name="pa1">被合并的数组一</param>
        /// <param name="pa2">被合并得数组二</param>
        /// <returns>合并后的数组</returns>
        private static IDbDataParameter[] MergeParameter(IDbDataParameter[] pa1, IDbDataParameter[] pa2)
        {
            ListDictionary dict = new ListDictionary();

            if (pa1 != null)
            {
                foreach (IDbDataParameter p in pa1)
                    if (!dict.Contains(p.ParameterName))
                        dict.Add(p.ParameterName, p);
            }

            if (pa2 != null)
            {
                foreach (IDbDataParameter p in pa2)
                    if (!dict.Contains(p.ParameterName))
                        dict.Add(p.ParameterName, p);
            }

            IDbDataParameter[] array = new IDbDataParameter[dict.Count];
            dict.Values.CopyTo(array, 0);
            return array;
        }


        /// <summary>
        /// 依据OleDbParameter的ParamName属性合并两个OleDbParameter数组.
        /// 原始数组中的值的ParameterName可能被更改
        /// </summary>
        /// <param name="pa1">被合并的数组一，数组中的值的ParameterName可能被更改</param>
        /// <param name="pa2">被合并得数组二，数组中的值的ParameterName可能被更改</param>
        /// <returns>合并后的数组</returns>
        public static OleDbParameter[] MergeParameter(ref OleDbParameter[] pa1, ref OleDbParameter[] pa2)
        {
            ListDictionary dict = new ListDictionary();

            if (pa1 != null)
            {
                foreach (OleDbParameter p in pa1)
                {
                    if (dict.Contains(p.ParameterName))
                        p.ParameterName = p.ParameterName + '1';

                    dict.Add(p.ParameterName, p);
                }
            }

            if (pa2 != null)
            {
                foreach (OleDbParameter p in pa2)
                {
                    if (dict.Contains(p.ParameterName))
                        p.ParameterName = p.ParameterName + '1';

                    dict.Add(p.ParameterName, p);
                }
            }

            OleDbParameter[] array = new OleDbParameter[dict.Count];
            dict.Values.CopyTo(array, 0);
            return array;
        }


        /// <summary>
        /// 依据合并IDataParameter[]参数到输出的参数字典中。
        /// IDataParameter[]必须是SqlParameter[]或OleDbParameter[]
        /// </summary>
        /// <param name="pa">被合并的数组</param>
        /// <param name="output">用于存储合并后参数的字典</param>
        private void MergeParameter(IDataParameter[] pa, ref IDictionary output)
        {
            if (pa == null || pa.Length == 0)
                return;

            if (pa[0] is OleDbParameter)
            {
                foreach (OleDbParameter p in pa)
                {
                    if (output.Contains(p.ParameterName))
                        p.ParameterName = p.ParameterName + '1';

                    output.Add(p.ParameterName, p);
                }
            }
            else
            {
                foreach (IDbDataParameter p in pa)
                {
                    if (!output.Contains(p.ParameterName))
                        output.Add(p.ParameterName, p);
                }
            }
        }
        #endregion

        #region SqlParameter 生成System.Data.SqlClient.SqlParameter实例
        /// <summary>
        /// 依据参数“名称”，“参数值”创建SqlParameter实例
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>qlParameter实例</returns>
        public IDbDataParameter NewParameter(string parameterName, object value)
        {
            return _connType.CreateParameter(parameterName, value);
        }

        /// <summary>
        /// 依据参数“名称”，“参数值”创建SqlParameter实例
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="direction">参数的输入输出方向</param>
        /// <param name="value">参数值</param>
        /// <returns>qlParameter实例</returns>
        public IDbDataParameter NewParameter(string parameterName, ParameterDirection direction, object value)
        {
            IDbDataParameter param = NewParameter(parameterName, value);
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// 依据参数“名称”，“参数值”，“源列名”创建SqlParameter实例
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="sourceColomn">源列名</param>
        /// <param name="value">参数值</param>
        /// <returns>SqlParameter实例</returns>
        public IDbDataParameter NewParameter(string parameterName, string sourceColomn, object value)
        {
            IDbDataParameter param = NewParameter(parameterName, value);
            param.SourceColumn = sourceColomn;
            return param;
        }

        /// <summary>
        /// 依据参数“名称”，“数据类型”，“数据宽度”，“参数值”创建SqlParameter实例
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">数据宽度</param>
        /// <param name="value">参数值</param>
        /// <returns>SqlParameter实例</returns>
        public IDbDataParameter NewParameter(string parameterName, DbType dbType, int size, object value)
        {

            IDbDataParameter param = NewParameter(parameterName, value);
            param.DbType = dbType;
            param.Size = size;
            return param;
        }

        /// <summary>
        /// 依据参数“名称”，“数据类型”，“数据宽度”，“参数值”创建SqlParameter实例
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">数据宽度</param>
        /// <param name="direction">参数的输入输出方向</param>
        /// <param name="value">参数值</param>
        /// <returns>SqlParameter实例</returns>
        public IDbDataParameter NewParameter(string parameterName, DbType dbType, int size, ParameterDirection direction, object value)
        {

            IDbDataParameter param = NewParameter(parameterName, value);
            param.DbType = dbType;
            param.Size = size;
            param.Direction = direction;
            return param;
        }

        #endregion


        #region InsertCommand  生成Insert命令
        /// <summary>
        /// 根据DataRow中的数据生成Insert SQL，忽略DataRow中DataColumn为ReadOnly的数据单元
        /// </summary>
        /// <param name="sb">将储存生成完的Insert SQL</param>
        /// <param name="row">依据此数据行中的非ReadOnly数据生成Insert SQL</param>
        /// <param name="ignoredColumn">不写入数据库的列名</param>
        /// <returns>Insert SQL的SqlParameter参数</returns>
        public IDbDataParameter[] InsertCommand(StringBuilder sb, DataRow row, params DataColumn[] ignoredColumn)
        {
            DataTable table = row.Table;
            string tableName = table.TableName;

            sb.Append(" INSERT INTO " + tableName + " (");
            IDbDataParameter[] pa1 = AddFieldValue(sb, row, table.Columns, ",", AddFieldType.Field, false, ignoredColumn);

            sb.Append(") VALUES(");
            IDbDataParameter[] pa2 = AddFieldValue(sb, row, table.Columns, ",", AddFieldType.Value, false, ignoredColumn);

            sb.Append(")");

            return MergeParameter(pa1, pa2);
        }
        #endregion

        #region UpdateCommand  生成Update命令
        /// <summary>
        /// 根据DataRow中的数据生成Update SQL，忽略DataRow中DataColumn为ReadOnly的数据单元
        /// </summary>
        /// <param name="sb">将储存生成完的Update SQL</param>
        /// <param name="row">依据此数据行中的非ReadOnly数据及主键数据生成Update SQL</param>
        /// <param name="ignoreReadonly">忽略更新Readonly属性为true的列</param>
        /// <param name="ignoredColumn">即使列的Readonly属性不为true，也不会被更新的列数组</param>
        /// <returns>Update SQL的SqlParameter参数</returns>
        public IDbDataParameter[] UpdateCommand(StringBuilder sb, DataRow row, bool ignoreReadonly, params DataColumn[] ignoredColumn)
        {
            DataTable table = row.Table;
            string tableName = table.TableName;

            sb.Append(" UPDATE " + tableName + " SET ");

            IDbDataParameter[] pa1 = AddFieldValue(sb, row, table.Columns, ", ", AddFieldType.Field | AddFieldType.Value, ignoreReadonly, ignoredColumn);

            sb.Append(" WHERE ");
            IDbDataParameter[] pa2 = AddFieldValue(sb, row, table.PrimaryKey, " AND ", AddFieldType.Field | AddFieldType.Value);

            return MergeParameter(pa1, pa2);
        }
        #endregion

        #region DeleteCommand  生成Delete命令
        /// <summary>
        /// 根据DataRow中的的主键数据生成Delete SQL
        /// </summary>
        /// <param name="sb">将储存生成完的Delete SQL</param>
        /// <param name="row">依据此数据行中的主键字段生成Delete SQL</param>
        /// <returns>Delete SQL的SqlParameter参数</returns>
        public IDataParameter[] DeleteCommand(StringBuilder sb, DataRow row)
        {
            DataTable table = row.Table;
            string tableName = table.TableName;

            sb.Append(" DELETE FROM " + tableName + " WHERE ");

            return AddFieldValue(sb, row, table.PrimaryKey, " AND ", AddFieldType.Field | AddFieldType.Value);
        }
        #endregion



        #region 私有成员函数

        #region AddFieldValue

        #region StringBuilder, DataRow, DataColumn, AddFieldType, string
        private IDbDataParameter AddFieldValue(StringBuilder sb, DataRow row, DataColumn column, AddFieldType type, string suffix)
        {
            string field = column.ColumnName.ToUpper();
            string alias = field;
            Type dataType = column.DataType;

            if ((int)(type & AddFieldType.Field) != 0)
                sb.Append('"').Append(field).Append('"');


            if ((int)(type & AddFieldType.Field) != 0 && (int)(type & AddFieldType.Value) != 0)
                sb.Append("=");

            if ((int)(type & AddFieldType.Value) != 0) 
            {
                if (row[column] == DBNull.Value)
                {
                    sb.Append("null");
                    return null;
                }
                else if (dataType == typeof(string) && ((string)row[column.Ordinal]).Length < 100)
                {
                    sb.Append("'");
                    sb.Append(((string)row[column]).Replace("'", "''"));
                    sb.Append("'");
                    return null;
                }
                else if (dataType.IsValueType && (dataType == typeof(int) ||
                    dataType == typeof(decimal) ||
                    dataType == typeof(float) ||
                    dataType == typeof(double) ||
                    dataType == typeof(decimal) ||
                    dataType == typeof(long)))
                {
                    sb.Append(row[column].ToString());
                    return null;
                }
                else
                {
                    IDbDataParameter param = NewParameter("@" + alias + suffix, row[column]);
                    sb.Append(param.ParameterName);
                    return param;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion StringBuilder, DataRow, DataColumn, AddFieldType, string

        #region StringBuilder, DataRow, DataColumn[], string, AddFieldType, string
        private IDbDataParameter[] AddFieldValue(StringBuilder sb, DataRow row, DataColumn[] columns, string separator, AddFieldType type, string suffix)
        {
            ArrayList paramArray = new ArrayList();
            bool isFirstColumn = true;
            IDbDataParameter param = null;
            foreach (DataColumn column in columns)
            {
                if (!isFirstColumn)
                    sb.Append(separator);

                param = AddFieldValue(sb, row, column, type, suffix);

                if (param != null)
                    paramArray.Add(param);

                isFirstColumn = false;
            }


            if (paramArray.Count == 0)
                return null;
            return (IDbDataParameter[])paramArray.ToArray(paramArray[0].GetType());
        }
        #endregion StringBuilder, DataRow, DataColumn[], separator, AddFieldType, string

        #region StringBuilder, DataRow, DataColumn[], string, AddFieldType
        private IDbDataParameter[] AddFieldValue(StringBuilder sb, DataRow row, DataColumn[] columns, string separator, AddFieldType type)
        {
            return AddFieldValue(sb, row, columns, separator, type, string.Empty);
        }
        #endregion StringBuilder, DataRow, DataColumn[], string, AddFieldType

        #region StringBuilder, DataRow, DataColumnCollection, string, AddFieldType, bool, params DataColumn[]
        private IDbDataParameter[] AddFieldValue(StringBuilder sb, DataRow row, DataColumnCollection columns, string separator, AddFieldType type, bool ignoreReadOnly, params DataColumn[] ignoredColumn)
        {
            DataColumn[] columnArray;
            if (ignoreReadOnly)
            {
                ArrayList array = new ArrayList();

                foreach (DataColumn column in columns)
                    if (!column.ReadOnly
                        && (ignoredColumn == null || ArraySearch(ignoredColumn, column) == -1))
                        array.Add(column);

                columnArray = (DataColumn[])array.ToArray(typeof(DataColumn));
            }
            else if (ignoredColumn == null || ignoredColumn.Length == 0)
            {
                ArrayList array = new ArrayList();
                foreach (DataColumn column in columns)
                    if (!column.AutoIncrement)
                        array.Add(column);
                columnArray = (DataColumn[])array.ToArray(typeof(DataColumn));
            }
            else
            {
                ArrayList array = new ArrayList();
                foreach (DataColumn column in columns)
                {
                    if (ArraySearch(ignoredColumn, column) == -1 && !column.AutoIncrement)
                        array.Add(column);
                }
                columnArray = (DataColumn[])array.ToArray(typeof(DataColumn));
            }

            return AddFieldValue(sb, row, columnArray, separator, type);
        }
        #endregion StringBuilder, DataRow, DataColumnCollection, string, AddFieldType, bool, params DataColumn[]

        #region StringBuilder, DataRow, DataColumnCollection, string, AddFieldType
        private IDataParameter[] AddFieldValue(ref StringBuilder sb, DataRow row, DataColumnCollection columns, string separator, AddFieldType type)
        {
            DataColumn[] columnArray = new DataColumn[columns.Count];
            columns.CopyTo(columnArray, 0);

            return AddFieldValue(sb, row, columnArray, separator, type);
        }
        #endregion StringBuilder, DataRow, DataColumnCollection, string, AddFieldType

        #endregion AddFieldValue

        #region Enum AddFieldType
        private enum AddFieldType
        {
            Field = 1,
            Value = 2
        }
        #endregion


        #endregion
    }
}
