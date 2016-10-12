1、配置：首先在应用程序的配置文件中（app.config或者web.config）
添加下列连接串配置信息：

<configuration>

  <configSections>
      <section  name="ConnectionConfiguration"  type="Juppiphis.Data.ConnectionConfig, Juppiphis.Data"  />
  </configSections>


  <ConnectionConfiguration>
    
    <!--  连接SQL Server数据库样例，name属性为用户定义的“数据库连接串别名”，
            providerName属性为数据库连接串类型，
            xml节点内文本为连接串
      -->
    <add name="IFone" providerName="System.Data.SqlClient"  >
    User Id=sa;Password=;Data Source=codeserver; Initial Catalog=IFone;
    </add>
    
    <!--  连接Oracle数据库样例 -->
    <add name="ITKUser" providerName="Oracle.DataAccess.Client" >
      Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=CODESERVER)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=ITKUser)));User Id=test;Password=test;
    </add>
    
    <!--  连接Sybase数据库样例 -->
    <add name="Manager" providerName="Sybase.Data.AseClient"  >
    User Id=sa;Password=;Data Source=codeserver, 2048; Initial Catalog=IFone;
    </add>
    
  </ConnectionConfiguration>
</configuration>

2、在程序中初始化（在程序初始化的函数体内增加下列语句，注意，全局只需初始化一次）

		程序全局初始化函数()
		{
            Juppiphis.Data.ConnectionConfig.OnInitConnectionType += OnInitConnectionType;
            Juppiphis.Data.ConnectionConfig.Configure();
        }
            
        private void OnInitConnectionType(string name, string providerName)
        {
            if (providerName == "Oracle.DataAccess.Client")
                Juppiphis.Data.ConnectionConfig.InitConnection<OracleConnection, OracleCommand, OracleParameter, OracleCommandBuilder, OracleDataAdapter>(name);
            else if (providerName == "Sybase.Data.AseClient")
                Juppiphis.Data.ConnectionConfig.InitConnection<AseConnection, AseCommand, AseParameter, AseCommandBuilder, AseDataAdapter>(name);
            else if (providerName == "System.Data.SqlClient")
                Juppiphis.Data.ConnectionConfig.InitConnection<SqlConnection, SqlCommand, SqlParameter, SqlCommandBuilder, SqlDataAdapter>(name);
            else
                return;
        }
        
3、使用样例

DataTable table = new DataTable();

\\下列函数第二个值"IFone"即配置文件中所设定的连接串别名（也就是name="IFone"属性）
Juppiphis.Data.SqlHelper.ExecuteFillTable(table, "IFone" , CommandType.Text, "Select * From Table1");
