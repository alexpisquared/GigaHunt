using System;
using System.Configuration;

namespace DAL
{
	/// <summary>
	/// A solution to encapsulation violation.
	/// </summary>
	public static class DBHelperFactory
	{
		public static DBHelper Instance(string[] sqlconstr)
		{
			DBHelper dbh = instantiateDbHelper();

			dbh.ConnectionStrings = sqlconstr;

			return dbh;
		}
		public static DBHelper Instance(string sqlconstr)
		{
			DBHelper dbh = instantiateDbHelper();
			
			dbh.ConnectionString = sqlconstr;

			return dbh;
		}

		private static DBHelper instantiateDbHelper()
		{
			switch (DAL.Properties.Settings.Default.DbProvider)//.ToUpper())
			{
				default:
				case "SQLCLIENT": return new MssqlDBHelper(); 
				//case "ORACLE": return new OracleDBHelper(); 
				case "OLEDB": return new OledbDBHelper(); 
			}
		}
	}
}
