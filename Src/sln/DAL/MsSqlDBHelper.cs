using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
  /// <summary>
  /// Sql implementation of expected behavior
  /// </summary>
  [Serializable()]
  public class MssqlDBHelper : DBHelper
  {
    internal MssqlDBHelper()//connectionString) : base(connectionString)
    {
    }


    protected override void CheckCreateConnection()
    {
      if (_connection == null)
      {
        _connection = new SqlConnection(_connectionString);
      }
    }

    //public override void BeginTxn()
    //{
    //   _transaction = ((SqlConnection)_connection).BeginTransaction();
    //}


    protected override void InitializeCommand(string sql, CommandType commandType, ArrayList parameters)
    {
      _command = new SqlCommand(sql, (SqlConnection)_connection, (SqlTransaction)_transaction)
      {
        CommandType = commandType
      };

      if (parameters != null)
      {
        SqlCommandBuilder.DeriveParameters((SqlCommand)_command);

        for (var i = 0; i < parameters.Count; i++)
        {
          if (parameters[i] is DateTime)
          {
            ((SqlCommand)_command).Parameters[i].Value = (DateTime)parameters[i];
          }
          else
          {
            ((SqlCommand)_command).Parameters[i].Value = parameters[i];
          }
        }
      }
    }
    protected override void InitializeCommand(string sql, CommandType commandType, params IDataParameter[] parameters)
    {
      _command = new SqlCommand(sql, (SqlConnection)_connection, (SqlTransaction)_transaction)
      {
        CommandType = commandType
      };

      if (parameters != null)
      {
        foreach (SqlParameter parameter in parameters)
        {
          _command.Parameters.Add(parameter);
        }
      }
    }
    protected override void InitializeCommand(string sql, CommandType commandType, params object[] parameters)
    {
      _command = new SqlCommand(sql, (SqlConnection)_connection, (SqlTransaction)_transaction)
      {
        CommandType = commandType
      };

      if (parameters != null)
      {
        if (commandType == CommandType.StoredProcedure)
        {
          SqlCommandBuilder.DeriveParameters((SqlCommand)_command);

          for (var i = 0; i < parameters.Length && i < ((SqlCommand)_command).Parameters.Count; i++)
          {
            ((SqlCommand)_command).Parameters[i].Value =
              (parameters[i] is DateTime) ?
              ((DateTime)parameters[i]) :
              parameters[i];
          }
        }
      }
    }

    protected override void InitializeDataAdapter() => _adapter = new SqlDataAdapter();
  }

}
