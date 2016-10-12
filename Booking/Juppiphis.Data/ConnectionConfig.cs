using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Reflection;
using System.Linq;

namespace Juppiphis.Data
{
    /// <summary>
    /// 配置数据库连接串是导致的异常
    /// </summary>
    public class ConnectionConfigException : System.ApplicationException
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="strMsg">异常的错误信息</param>
        public ConnectionConfigException(string strMsg) : base(strMsg)
        {
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="strMsg">解释异常原因的错误信息</param>
        /// <param name="innerException">作为当前异常原因的原始异常</param>
        public ConnectionConfigException(string strMsg, Exception innerException) : base(strMsg, innerException)
        {
        }
    }

    /// <summary>
    /// 负责读取配置文件里面的连接串的配置信息。
    /// 所有的连接串信息保存在配置数据库中的配置表中，
    /// 配置文件里保存的是指向配置表的连接串。
    /// </summary>
    public sealed class ConnectionConfig : IConfigurationSectionHandler
    {
        static ConnectionConfig()
        {
            ConnectionConfig.Configure();
        }

        #region Fields and Properties

        private static volatile bool _xmlSectionConfigured = false;

        private static volatile bool _initializeCompleted = false;

        public static readonly object SyncRoot = new object();

        public static bool Initialized
        {
            get { return _initializeCompleted; }
        }

        private static HybridDictionary _aliasToConnectionStringDict = null;

        private static HybridDictionary _aliasToTypeDict = null;

        private static ListDictionary _connectionNameToTypeDict = null;

        private static HybridDictionary _connectionStringToTypeDict = null;

        private static ListDictionary _providerToTypeDict = null;

        /// <summary>
        /// 在config文件中的默认“配置节”名称，用于自动初始化。
        /// </summary>
        public const string DefaultConfigSection = "ConnectionConfiguration";

        public static event AddAliasDelegate AfterAddingAlias;

        public delegate void AddAliasDelegate(string alias, string connectionName, string ProviderName);

        #endregion

        #region InitConnection
        /// <summary>
        /// 初始化连接串的类型
        /// </summary>
        /// <param name="alias">连接串别名</param>
        /// <returns></returns>
        public static ConnectionType<TConnection> InitConnection<TConnection, TCommand, TParameters, TCommandBuilder, TDataAdapter>(
            string alias)
            where TConnection : IDbConnection, new()
            where TCommand : IDbCommand, new()
            where TParameters : IDbDataParameter, ICloneable, new()
            where TCommandBuilder : new()
            where TDataAdapter : DbDataAdapter, new()
        {
            if (alias == null)
                throw new ArgumentNullException("connectionName");

            if (_aliasToConnectionStringDict == null || _aliasToConnectionStringDict.Count == 0)
                ConnectionConfig.Configure();

            TemporayConnectionInfo conn = (TemporayConnectionInfo)_aliasToConnectionStringDict[alias];
            if (conn == null)
                throw new ArgumentException("The connection string is not exists whose name is " + alias);

            InternalConnectionType<TConnection, TCommand, TParameters, TCommandBuilder, TDataAdapter> internalConnectionType = 
                new InternalConnectionType<TConnection, TCommand, TParameters, TCommandBuilder, TDataAdapter>(
                conn.ConnectionString);


            lock (ConnectionConfig.SyncRoot)
            {
                _aliasToTypeDict[alias] =
                    _connectionNameToTypeDict[internalConnectionType.CreateConnection().GetType().FullName] =
                    _connectionStringToTypeDict[conn.ConnectionString] =
                    internalConnectionType.Traits;

                if (!string.IsNullOrWhiteSpace(conn.ProviderName))
                    _providerToTypeDict[conn.ProviderName] = internalConnectionType.Traits;

                if (_aliasToConnectionStringDict.Count <= _aliasToTypeDict.Count)
                    _initializeCompleted = true;
            }

            return internalConnectionType.Traits;
        }
        #endregion InitConnection

        #region GetConnectionString
        /// <summary>
        /// 根据定义的Alias获得真实数据库连接字符串
        /// </summary>
        /// <param name="alias">连接串别名</param>
        /// <returns>连接串</returns>
        public static string GetConnectionString(string alias)
        {
            IConnectionType connection = _aliasToTypeDict[alias] as IConnectionType;

            if (connection == null)
                return string.Empty;
            else
                return connection.ConnectionString;
        }
        #endregion GetConnectionString

        #region GetConnectionType
        public static IConnectionType GetConnectionType(string alias)
        {
            return _aliasToTypeDict[alias] as IConnectionType;
        }

        internal static IConnectionType GetConnectionType(IDbConnection connection)
        {
            IConnectionType ct = _connectionStringToTypeDict[connection.ConnectionString] as IConnectionType;
            if (ct != null)
                return ct;

            ct = _connectionNameToTypeDict[connection.GetType().FullName] as IConnectionType;
            if (ct != null)
            {
                _connectionStringToTypeDict[connection.ConnectionString] = ct = ct.Copy(connection.ConnectionString);
                return ct;
            }

            return null;
        }
        #endregion GetConnectionType

        #region Configure
        /// <summary>
        /// 使用config文件中默认的配置段（DefaultConfigSection所定义）进行此类的实例化
        /// </summary>
        public static void Configure()
        {
            if (_xmlSectionConfigured)
                return;

            Configure(DefaultConfigSection);
        }

        /// <summary>
        /// 使用config文件中指定名称的配置段进行此类的实例化
        /// </summary>
        /// <param name="ConfigSection">配置段的名称</param>
        public static void Configure(string configSection)
        {
            if (_xmlSectionConfigured)
                return;

            System.Configuration.ConfigurationManager.GetSection(configSection);
        }
        #endregion Configure

        #region Decrypt
        /// <summary>
        /// 解密数据库连接串，密钥内置
        /// </summary>
        /// <param name="source">密文</param>
        /// <returns>明文</returns>
        private static string Decrypt(string source)
        {
            string result = string.Empty;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] desKey = { 136, 183, 142, 217, 175, 71, 90, 239 };
            byte[] desIV = { 227, 105, 5, 40, 162, 158, 143, 156 };

            des.Key = desKey;
            des.IV = desIV;
            if (source.Length > 0)
            {
                byte[] bytearrayInput = Convert.FromBase64String(source);

                MemoryStream memStream = new MemoryStream();
                memStream.Write(bytearrayInput, 0, bytearrayInput.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                ICryptoTransform desdecrypt = des.CreateDecryptor();

                CryptoStream cryptoStream = new CryptoStream(memStream, desdecrypt, CryptoStreamMode.Read);
                result = (new StreamReader(cryptoStream, new UnicodeEncoding())).ReadToEnd();
                cryptoStream.Close();
            }
            return result;
        }
        #endregion Decrypt

        #region AddAlias
        /// <summary>
        /// 添加一个Alias和连接串的对应关系
        /// </summary>
        /// <param name="alias">别名</param>
        /// <param name="connectionString">连接串</param>
        /// <param name="providerName">连接串类型</param>
        /// <param name="encryted">是</param>
        public static void AddAlias(string alias, string connectionString, string providerName, bool encryted, bool autoInitialized)
        {
            lock (ConnectionConfig.SyncRoot)
            {
                if (_aliasToConnectionStringDict == null)
                    _aliasToConnectionStringDict = new HybridDictionary();
                if (_aliasToTypeDict == null)
                    _aliasToTypeDict = new HybridDictionary();
                if (_connectionNameToTypeDict == null)
                    _connectionNameToTypeDict = new ListDictionary();
                if (_connectionStringToTypeDict == null)
                    _connectionStringToTypeDict = new HybridDictionary();
                if (_providerToTypeDict == null)
                    _providerToTypeDict = new ListDictionary(StringComparer.InvariantCultureIgnoreCase);

                if (encryted)
                    connectionString = Decrypt(connectionString);

                _aliasToConnectionStringDict[alias] = new TemporayConnectionInfo(connectionString, providerName);

                AutoInitConnection(alias, providerName);

            }
            if (AfterAddingAlias != null)
                AfterAddingAlias(alias, connectionString, providerName);
        }
        #endregion AddAlias

        #region AutoInitConnection
        private static void AutoInitConnection(string alias, string providerName)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                return;

            TemporayConnectionInfo conn = (TemporayConnectionInfo)_aliasToConnectionStringDict[alias];
            if (conn == null)
                throw new ArgumentException("The connection string is not exists whose name is " + alias);

            IConnectionType ct = _providerToTypeDict[providerName] as IConnectionType;
            if (ct != null)
            {
                IConnectionType newCT = ct.Copy(conn.ConnectionString);

                _aliasToTypeDict[alias] =
                    _connectionNameToTypeDict[ct.CreateConnection().GetType().FullName] =
                    _connectionStringToTypeDict[conn.ConnectionString] = newCT;

                return;
            }

            if (providerName.ToLower().IndexOf("sqlclient") != -1)
            {
                InitConnection<SqlConnection, SqlCommand, SqlParameter, SqlCommandBuilder, SqlDataAdapter>(alias);
                return;
            }

            Assembly provider = AppDomain.CurrentDomain.Load(providerName);
            var methods = from m in provider.GetTypes()
                         where m.IsVisible && !m.IsAbstract && m.IsClass
                         select m;

            Type connType = null;
            Type cmdType = null;
            Type paramType = null;
            Type builderType = null;
            Type adapterType = null;

            foreach (var m in methods)
            {
                if (m.FullName.EndsWith("Connection") && m.GetInterface("System.Data.IDbConnection", false) != null)
                    connType = m;
                else if (m.FullName.EndsWith("Command") && m.GetInterface("System.Data.IDbCommand", false) != null)
                    cmdType = m;
                else if (m.FullName.EndsWith("Parameter") && m.GetInterface("System.Data.IDbDataParameter", false) != null)
                    paramType = m;
                else if (m.FullName.EndsWith("CommandBuilder"))
                    builderType = m;
                else if (m.FullName.EndsWith("DataAdapter") && m.IsSubclassOf(typeof(DbDataAdapter)))
                    adapterType = m;
            }

            MethodInfo initMethod = typeof(ConnectionConfig).GetMethod("InitConnection");
            initMethod = initMethod.MakeGenericMethod(connType, cmdType, paramType, builderType, adapterType);
            ct = (IConnectionType)initMethod.Invoke(null, new object[] { alias });


            _aliasToTypeDict[alias] =
                _connectionNameToTypeDict[ct.CreateConnection().GetType().FullName] =
                _connectionStringToTypeDict[conn.ConnectionString] = 
                _providerToTypeDict[providerName] = ct;
        }
        #endregion AutoInitConnection

        #region 实现IConfigurationSectionHandler接口方法
        /// <summary>
        /// 实现IConfigurationSectionHandler接口方法
        /// </summary>
        /// <param name="parent">The configuration settings in a corresponding parent configuration section. </param>
        /// <param name="configContext">
        /// An HttpConfigurationContext when Create is called from the ASP.NET configuration system.
        /// Otherwise, this parameter is reserved and is a null reference (Nothing in Visual Basic). 
        /// </param>
        /// <param name="section">
        /// The XmlNode that contains the configuration information from the configuration file. 
        /// Provides direct access to the XML contents of the configuration section.
        /// </param>
        /// <returns></returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            lock (ConnectionConfig.SyncRoot)
            {
                _xmlSectionConfigured = true;

                XmlNodeList nodeList = section.SelectNodes(@"add");
                if (nodeList.Count == 0)
                    throw new ConnectionConfigException("No expected node(add)");

                foreach (XmlNode node in nodeList)
                {
                    XmlAttribute connAttr = node.Attributes["connectionString"];

                    string conn = null;
                    if (connAttr == null || connAttr.Value.Trim() == string.Empty)
                        conn = node.InnerText.Trim();
                    else
                        conn = connAttr.Value.Trim();



                    XmlAttribute alias = node.Attributes["name"];
                    XmlAttribute providerName = node.Attributes["providerName"];
                    XmlAttribute encrytedNode = node.Attributes["Encryted"];
                    bool encrypted = (encrytedNode != null && encrytedNode.Value.Trim().ToLower() == "true")
                        ? true : false;


                    if (alias == null || string.IsNullOrWhiteSpace(alias.Value))
                        continue;
                    else
                        AddAlias(alias.Value, conn, (providerName == null) ? string.Empty : providerName.Value, encrypted, true);
                }

                if (_aliasToConnectionStringDict.Count == _aliasToTypeDict.Count)
                    _initializeCompleted = true;

                return true;
            }
        }
        #endregion

        #region class TemporayConnectionInfo
        private class TemporayConnectionInfo
        {
            public string ConnectionString;
            public string ProviderName;

            public TemporayConnectionInfo(string connectionString, string providerName)
            {
                ConnectionString = connectionString;
                ProviderName = providerName;
            }
        }
        #endregion class TemporayConnectionInfo

    }

    #region interface IConnectionType
    public interface IConnectionType
    {
        string ConnectionString { get; }

        IDbConnection CreateConnection();

        IDbCommand CreateCommand(string commandText);

        object CreateCommandBuilder();

        DbDataAdapter CreateDataAdapter();

        IDbDataParameter CreateParameter(string parameterName, object value);

        IConnectionType Copy(string connectionString);
    }
    #endregion interface IConnectionType

    #region class ConnectionType : IConnectionType
    public class ConnectionType<TConnection> : IConnectionType
    {
        internal IConnectionType _InternalConnectionType;

        public string ConnectionString
        {
            get { return _InternalConnectionType.ConnectionString; }
        }

        internal ConnectionType(IConnectionType internalConnectionType) 
        {
            _InternalConnectionType = internalConnectionType;        
        }

        public IDbConnection CreateConnection()
        {
            return _InternalConnectionType.CreateConnection();
        }

        public IDbCommand CreateCommand(string commandText)
        {
            return _InternalConnectionType.CreateCommand(commandText);
        }

        public object CreateCommandBuilder()
        {
            return _InternalConnectionType.CreateCommandBuilder();
        }

        public DbDataAdapter CreateDataAdapter()
        {
            return _InternalConnectionType.CreateDataAdapter();
        }

        public IDbDataParameter CreateParameter(string parameterName, object value)
        {
            return _InternalConnectionType.CreateParameter(parameterName, value);
        }

        public IConnectionType Copy(string connectionString)
        {
            ConnectionType<TConnection> connectionType =
                new ConnectionType<TConnection>(_InternalConnectionType.Copy(connectionString));
            return connectionType;
        }
    }
    #endregion class ConnectionType

    #region class InternalConnectionType
    internal class InternalConnectionType<TConnection, TCommand, TParameters, TCommandBuilder, TDataAdapter> : IConnectionType
        where TConnection : IDbConnection, new ()
        where TCommand : IDbCommand, new ()
        where TParameters : IDbDataParameter, ICloneable, new ()
        where TCommandBuilder : new ()
        where TDataAdapter : DbDataAdapter, new ()
    {
        private string _connectionString;

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        private ConnectionType<TConnection> _traits;

        public ConnectionType<TConnection> Traits
        {
            get { return _traits ?? (_traits = new ConnectionType<TConnection>(this)); }
        }

        public InternalConnectionType(string connectionString)
        {
            _connectionString = connectionString;
        }


        public IDbConnection CreateConnection()
        {
            TConnection conn = new TConnection();
            conn.ConnectionString = _connectionString;
            return conn;
        }

        public IDbCommand CreateCommand(string commandText)
        {
            TCommand cmd = new TCommand();
            cmd.CommandText = commandText;
            return cmd;
        }

        public IDbDataParameter CreateParameter(string parameterName, object value)
        {
            TParameters param = new TParameters();
            param.ParameterName = parameterName;
            param.Value = value ?? DBNull.Value;
            return param;
        }

        public object CreateCommandBuilder()
        {
            return new TCommandBuilder();
        }

        public DbDataAdapter CreateDataAdapter()
        {
            return new TDataAdapter();
        }

        public IConnectionType Copy(string connectionString)
        {
            return new InternalConnectionType<TConnection, TCommand, TParameters, TCommandBuilder, TDataAdapter>(connectionString);
        }
    }
    #endregion class DataBaseInfo
}