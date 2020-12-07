using System;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
//using System.Data.OracleClient;

namespace DAL
{
  public enum DataProviderType
  {
    Sql,
    OleDb,
    Odbc,
    Oracle
  }

  public enum ReturnType
  {
    DataSetType,
    IDataReaderType,
    XmlDocumentType
  }


  /// <summary>
  /// Summary description for DataHandler.
  /// </summary>
  [Serializable()]
  public class DataHandler : IDisposable//FxCop
  {

    private IDbConnection genericConnection;
    private IDbDataAdapter genericAdapter;
    private IDbTransaction transaction;
    // private IDbCommand genericCommand;		never referenced
    private readonly DataProviderType providerType;
    private readonly string connectionString;
    private bool startTransaction;//FxCop =  = false;
    private bool endTransaction;//FxCop =  = false;
    bool _disposed;//FxCop = false;



    //public DataHandler()
    //{
    //   //
    //   // TODO: Add constructor logic here
    //   //
    //}

    public DataHandler(string connectionString, DataProviderType providerType)
    {
      this.connectionString = connectionString;
      this.providerType = providerType;

    }

    public DataHandler(/*string connectionKeyName, string providerKeyName*/)
    {
      connectionString = DAL.Properties.Settings.Default.SqlConStr;

      switch (DAL.Properties.Settings.Default.DbProvider)//.ToUpper())
      {
        case "SQLCLIENT":
          providerType = DataProviderType.Sql;
          break;
        case "OLEDB":
          providerType = DataProviderType.OleDb;
          break;
        case "ODBC":
          providerType = DataProviderType.Odbc;
          break;
        case "ORACLE":
          providerType = DataProviderType.Oracle;
          break;
        default:
          providerType = DataProviderType.Sql;
          break;
      }

    }

    public bool StartTransaction
    {
      get => startTransaction;
      set => startTransaction = value;
    }

    public bool EndTransaction
    {
      get => endTransaction;
      set => endTransaction = value;
    }


    public IDbTransaction Transaction => transaction;

    public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
    {
      var ds = new DataSet();
      InitializeConnection();
      InitializeDataAdapter();

      //			try
      //			{
      genericAdapter.SelectCommand = InitializeSPCommand(storedProcName, parameters, tableName);
      genericAdapter.Fill(ds);
      //			}
      //			catch (Exception ex)
      //			{
      //				if(endTransaction) this.transaction.Rollback();
      //				CloseConnection();
      //				throw ex;
      //			}

      CloseConnection();

      return ds;

    }

    public int RunProcedureNonQuery(string storedProcName, ArrayList parameters)
    {
      int rowsAffected;
      InitializeConnection();
      InitializeDataAdapter();
      IDbCommand command;

      switch (providerType)
      {
        case DataProviderType.Sql:
          command = (SqlCommand)InitializeSPCommand(storedProcName, parameters);
          rowsAffected = ((SqlCommand)command).ExecuteNonQuery();
          break;
        case DataProviderType.OleDb:
          command = (OleDbCommand)InitializeSPCommand(storedProcName, parameters);
          rowsAffected = ((OleDbCommand)command).ExecuteNonQuery();
          break;
        case DataProviderType.Odbc:
          command = (OdbcCommand)InitializeSPCommand(storedProcName, parameters);
          rowsAffected = ((OdbcCommand)command).ExecuteNonQuery();
          break;
        //case DataProviderType.Oracle:
        //  command = (OracleCommand)InitializeSPCommand(storedProcName, parameters);
        //  rowsAffected = ((OracleCommand)command).ExecuteNonQuery();
        //  break;
        default:
          command = (SqlCommand)InitializeSPCommand(storedProcName, parameters);
          rowsAffected = ((SqlCommand)command).ExecuteNonQuery();
          break;
      }

      CloseConnection();

      return rowsAffected;
    }

    public DataSet RunProcedure(string storedProcName, ArrayList parameters)
    {
      var ds = new DataSet();
      InitializeConnection();
      InitializeDataAdapter();
      IDbCommand command;

      switch (providerType)
      {
        case DataProviderType.Sql:
          command = (SqlCommand)InitializeSPCommand(storedProcName, parameters);
          break;
        case DataProviderType.OleDb:
          command = (OleDbCommand)InitializeSPCommand(storedProcName, parameters);
          break;
        case DataProviderType.Odbc:
          command = (OdbcCommand)InitializeSPCommand(storedProcName, parameters);
          break;
        //case DataProviderType.Oracle:
        //  command = (OracleCommand)InitializeSPCommand(storedProcName, parameters);
        //  break;
        default:
          command = (SqlCommand)InitializeSPCommand(storedProcName, parameters);
          break;
      }

      //			try
      //			{
      genericAdapter.SelectCommand = command;
      genericAdapter.Fill(ds);
      //			}
      //			catch (Exception ex)
      //			{
      //				if(endTransaction) this.transaction.Rollback();
      //				CloseConnection();
      //				throw ex;
      //			}

      CloseConnection();

      return ds;
    }

    public IDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
    {
      IDataReader reader;

      InitializeConnection();
      InitializeDataAdapter();


      var command = InitializeSPCommand(storedProcName, parameters);

      //			try
      //			{
      reader = command.ExecuteReader(CommandBehavior.CloseConnection);
      //			}
      //			catch (Exception ex)
      //			{
      //				if(endTransaction) this.transaction.Rollback();
      //				CloseConnection();
      //				throw ex;
      //			}

      CloseConnection();

      return reader;

    }

    //      public int RunProcedure(string storedProcName, IDataParameter[] parameters)
    //      {
    //         int rowsAffected;

    //         InitializeConnection();
    //         InitializeDataAdapter();

    //         IDbCommand command = InitializeSPCommand(storedProcName, parameters);

    ////			try
    ////			{
    //            switch(this.providerType)
    //            {
    //               case DataProviderType.Sql:
    //                  rowsAffected = ((SqlCommand)command).ExecuteNonQuery();
    //                  break;
    //               case DataProviderType.OleDb:
    //                  rowsAffected = ((OleDbCommand)command).ExecuteNonQuery();
    //                  break;
    //               case DataProviderType.Odbc:
    //                  rowsAffected = ((OdbcCommand)command).ExecuteNonQuery();
    //                  break;
    //               case DataProviderType.Oracle:
    //                  rowsAffected = ((OracleCommand)command).ExecuteNonQuery();					
    //                  break;
    //               default:
    //                  rowsAffected = (/*(SqlCommand)*/command).ExecuteNonQuery();
    //                  break;
    //            }
    ////			}
    ////			catch (Exception ex)
    ////			{
    ////				if(endTransaction) this.transaction.Rollback();
    ////				CloseConnection();
    ////				throw ex;
    ////			}

    //         CloseConnection();

    //         return rowsAffected;
    //      }


    //public int ExecuteNonQuery(string sqlQuery)
    //{
    //  int rowsAffected = -1;

    //  try
    //  {
    //    InitializeConnection();
    //    IDbCommand cmd = new OracleCommand(sqlQuery, (OracleConnection)this.genericConnection);
    //    cmd.CommandType = CommandType.Text;
    //    rowsAffected = cmd.ExecuteNonQuery();
    //  }
    //  catch (Exception ex)
    //  {
    //    System.Diagnostics.Trace.WriteLine("**Error in DataHandler.ExecuteNonQuery() "+ex.Message);
    //    throw;
    //  }
    //  finally
    //  {
    //    CloseConnection();
    //  }

    //  return rowsAffected;
    //}

    //public object ExecuteScalar(string sqlQuery)
    //{
    //  object retval = null;

    //  try
    //  {
    //    InitializeConnection();
    //    IDbCommand cmd = new OracleCommand(sqlQuery, (OracleConnection)this.genericConnection);
    //    cmd.CommandType = CommandType.Text;
    //    retval = cmd.ExecuteScalar();
    //  }
    //  catch (Exception ex)
    //  {
    //    System.Diagnostics.Trace.WriteLine("**Error in DataHandler.ExecuteNonQuery() "+ex.Message);
    //    throw;
    //  }
    //  finally
    //  {
    //    CloseConnection();
    //  }

    //  return retval;
    //}

    public DataSet ExecuteQuery(string sqlQuery, string tableName)
    {
      InitializeConnection();
      InitializeDataAdapter();
      var ds = new DataSet();

      try
      {
        genericAdapter.SelectCommand = InitializeSPCommand(sqlQuery, null, tableName);
        genericAdapter.SelectCommand.CommandType = CommandType.Text;
        genericAdapter.Fill(ds);
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.WriteLine("**Error in DataHandler.ExecuteQuery() " + ex.Message);
        throw;
      }
      finally
      {
        CloseConnection();
      }

      return ds;
    }

    public IDataReader ExecuteQuery(string sqlQuery)//NOT TESTED!!! (Apr 2007)
    {
      IDataReader reader;

      InitializeConnection();
      InitializeDataAdapter();


      var command = InitializeSPCommand(sqlQuery, new ArrayList());
      command.CommandType = CommandType.Text;

      //			try
      //			{
      reader = command.ExecuteReader(CommandBehavior.CloseConnection);
      //			}
      //			catch (Exception ex)
      //			{
      //				if(endTransaction) this.transaction.Rollback();
      //				CloseConnection();
      //				throw ex;
      //			}

      CloseConnection();

      return reader;

    }

    private void InitializeConnection()
    {


      switch (providerType)
      {
        case DataProviderType.Sql:
          genericConnection = new SqlConnection(connectionString);
          break;
        case DataProviderType.OleDb:
          genericConnection = new OleDbConnection(connectionString);
          break;
        case DataProviderType.Odbc:
          genericConnection = new OdbcConnection(connectionString);
          break;
        //case DataProviderType.Oracle:
        //  this.genericConnection = new OracleConnection(this.connectionString);
        //  break;
        default:
          genericConnection = new SqlConnection(connectionString);
          break;

      }

      //genericConnection.ConnectionString = this.connectionString;


      //try
      //{
      genericConnection.Open();

      if (startTransaction)
      {
        switch (providerType)
        {
          case DataProviderType.Sql:
            transaction = (SqlTransaction)transaction;
            break;
          case DataProviderType.OleDb:
            transaction = (OleDbTransaction)transaction;
            break;
          case DataProviderType.Odbc:
            transaction = (OdbcTransaction)transaction;
            break;
          //case DataProviderType.Oracle:
          //  this.transaction = (OracleTransaction)this.transaction;
          //  break;
          default:
            transaction = (SqlTransaction)transaction;
            break;
        }

        transaction = genericConnection.BeginTransaction(IsolationLevel.ReadCommitted);

      }
      //}
      //	catch(Exception e)
      //	{


      //	}
    }


    private IDbCommand InitializeSPCommand(string commandText, IDataParameter[] parameters) => InitializeSPCommand(commandText, parameters, "");
    private IDbCommand InitializeSPCommand(string commandText, IDataParameter[] parameters, string tableName)
    {
      IDbCommand command;

      switch (providerType)
      {
        case DataProviderType.Sql:
          command = new SqlCommand(commandText, (SqlConnection)genericConnection);
          if (parameters != null)
          {
            foreach (SqlParameter parameter in parameters)
            {
              command.Parameters.Add(parameter);
            }
          }
          break;
        case DataProviderType.OleDb:
          command = new OleDbCommand(commandText, (OleDbConnection)genericConnection);
          if (parameters != null)
          {
            foreach (OleDbParameter parameter in parameters)
            {
              command.Parameters.Add(parameter);
            }
          }
          break;
        case DataProviderType.Odbc:
          command = new OdbcCommand(commandText, (OdbcConnection)genericConnection);
          if (parameters != null)
          {
            foreach (OdbcParameter parameter in parameters)
            {
              command.Parameters.Add(parameter);
            }
          }
          break;
        //case DataProviderType.Oracle:
        //  command = new OracleCommand(commandText, (OracleConnection)this.genericConnection);
        //  if(parameters != null)
        //  {
        //    foreach(OracleParameter parameter in parameters)
        //    {
        //      command.Parameters.Add(parameter);
        //    }
        //  }
        //  if(tableName != null && tableName.Length > 0) // != string.Empty misses NULLs; + checking Length is faster (29-Jun-03, AP)
        //  {
        //    //add the return parameter for the ref cursor
        //    ((OracleCommand)command).Parameters.Add(new OracleParameter(tableName, OracleType.Cursor)).Direction = ParameterDirection.Output;
        //  }
        //  break;

        default:
          command = new SqlCommand(commandText, (SqlConnection)genericConnection);
          if (parameters != null)
          {
            foreach (SqlParameter parameter in parameters)
            {
              command.Parameters.Add(parameter);
            }
          }
          break;
      }

      command.CommandType = CommandType.StoredProcedure;

      return command;

    }
    private IDbCommand InitializeSPCommand(string commandText, ArrayList parameters)
    {
      IDbCommand command;

      switch (providerType)
      {
        case DataProviderType.Sql:
          command = new SqlCommand(commandText, (SqlConnection)genericConnection)
          {
            CommandType = CommandType.StoredProcedure
          };
          if (startTransaction)
          {
            command.Transaction = transaction;
          }
          SqlCommandBuilder.DeriveParameters((SqlCommand)command);
          for (var i = 0; i < parameters.Count; i++)
          {
            ((IDataParameter)command.Parameters[i]).Value = parameters[i];
          }
          break;
        default:
        case DataProviderType.OleDb:
          command = new OleDbCommand(commandText, (OleDbConnection)genericConnection)
          {
            CommandType = CommandType.StoredProcedure
          };
          if (startTransaction)
          {
            command.Transaction = transaction;
          }
          OleDbCommandBuilder.DeriveParameters((OleDbCommand)command);
          for (var i = 0; i < parameters.Count; i++)
          {
            ((IDataParameter)command.Parameters[i]).Value = parameters[i];
          }
          break;
        case DataProviderType.Odbc:
          command = new OdbcCommand(commandText, (OdbcConnection)genericConnection)
          {
            CommandType = CommandType.StoredProcedure
          };

          if (startTransaction)
          {
            command.Transaction = transaction;
          }
          OdbcCommandBuilder.DeriveParameters((OdbcCommand)command);
          for (var i = 0; i < parameters.Count; i++)
          {
            ((IDataParameter)command.Parameters[i]).Value = parameters[i];
          }
          break;
          //case DataProviderType.Oracle:
          //  command = new OracleCommand(commandText, (OracleConnection)this.genericConnection);
          //  command.CommandType = CommandType.StoredProcedure;
          //  if(startTransaction)
          //  {
          //    command.Transaction = this.transaction;
          //  }
          //  OracleCommandBuilder.DeriveParameters((OracleCommand)command);
          //  for (int i = 0; i < parameters.Count; i++)
          //  {
          //    ((IDataParameter)command.Parameters[i]).Value = (parameters[i] is DateTime) ? 
          //      new OracleDateTime((DateTime)parameters[i]) :
          //      ((OracleCommand)command).Parameters[i].Value = parameters[i];
          //  }
          //  break;
      }


      return command;

    }

    private void InitializeDataAdapter()
    {
      switch (providerType)
      {
        case DataProviderType.Sql:
          genericAdapter = new SqlDataAdapter();
          break;
        case DataProviderType.OleDb:
          genericAdapter = new OleDbDataAdapter();
          break;
        case DataProviderType.Odbc:
          genericAdapter = new OdbcDataAdapter();
          break;
        //case DataProviderType.Oracle:
        //  this.genericAdapter = new OracleDataAdapter();
        //  break;
        default:
          genericAdapter = new SqlDataAdapter();
          break;

      }
    }

    private void CloseConnection() =>
      //			if(endTransaction)
      //			{
      //				this.transaction.Commit();
      //				genericConnection.Close();
      //				genericConnection.Dispose();
      //			}
      //			else if(!startTransaction)
      //			{
      //				genericConnection.Close();
      //				genericConnection.Dispose();
      //			}

      genericConnection.Close();//			genericConnection.Dispose();

    #region IDisposable Members

    public void Dispose()//FxCop
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
        {
          // Dispose managed resources.
          if (genericConnection.State == ConnectionState.Open)
            genericConnection.Close();
          genericConnection.Dispose();
        }

        // Dispose native resources.
      }

      _disposed = true;
    }

    #endregion
  }
}
