using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;

namespace DAL
{
  /// <summary>
  /// OleDb implementation of expected behavior
  /// </summary>
  [Serializable()]
  public class OledbDBHelper : DBHelper
  {
    internal OledbDBHelper()//string connectionString) : base(connectionString)
    {
    }


    protected override void CheckCreateConnection()
    {
      if (_connection == null)
      {
        _connection = new OleDbConnection(_connectionString);
      }
    }

    //public override void BeginTxn()
    //{
    //   _transaction = ((OleDbConnection)_connection).BeginTransaction();
    //}


    protected override void InitializeCommand(string sql, CommandType commandType, ArrayList parameters)
    {
      _command = new OleDbCommand(sql, (OleDbConnection)_connection, (OleDbTransaction)_transaction)
      {
        CommandType = commandType
      };

      if (parameters != null)
      {
        OleDbCommandBuilder.DeriveParameters((OleDbCommand)_command);

        for (var i = 0; i < parameters.Count; i++)
        {
          if (parameters[i] is DateTime)
          {
            ((OleDbCommand)_command).Parameters[i].Value = (DateTime)parameters[i];
          }
          else
          {
            ((OleDbCommand)_command).Parameters[i].Value = parameters[i];
          }
        }
      }
    }
    protected override void InitializeCommand(string sql, CommandType commandType, params IDataParameter[] parameters)
    {
      _command = new OleDbCommand(sql, (OleDbConnection)_connection, (OleDbTransaction)_transaction)
      {
        CommandType = commandType
      };

      if (parameters != null)
      {
        foreach (OleDbParameter parameter in parameters)
        {
          _command.Parameters.Add(parameter);
        }
      }
    }
    protected override void InitializeCommand(string sql, CommandType commandType, params object[] parameters)
    {
      _command = new OleDbCommand(sql, (OleDbConnection)_connection, (OleDbTransaction)_transaction)
      {
        CommandType = commandType
      };

      if (parameters != null)
      {
        if (commandType == CommandType.StoredProcedure)
        {
          OleDbCommandBuilder.DeriveParameters((OleDbCommand)_command);

          for (var i = 0; i < parameters.Length && i < ((OleDbCommand)_command).Parameters.Count; i++)
          {
            ((OleDbCommand)_command).Parameters[i].Value =
              (parameters[i] is DateTime) ?
              ((DateTime)parameters[i]) :
              parameters[i];
          }
        }
      }
    }

    protected override void InitializeDataAdapter() => _adapter = new OleDbDataAdapter();
  }

}
