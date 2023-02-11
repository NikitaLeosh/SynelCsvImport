using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace CsvImportSiteJS.Configuration
{
	public class ProjectConnectionStringBuilder
	{
		//obtain connection string items from docker-compose file
		static readonly string dbHost = Environment.GetEnvironmentVariable("DB_HOST");
		static readonly string dbName = Environment.GetEnvironmentVariable("DB_NAME");
		static readonly string Password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
		public static readonly string connectionstring = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={Password};Encrypt=false";
	}
}
