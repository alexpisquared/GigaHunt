//using System;
//using System.Collections;
//using System.Configuration;
//using System.Diagnostics;
//using System.Data;
////using System.Data.OracleClient;

//namespace DAL
//{
//  /// <summary>
//  /// Oracle implementation of expected behavior
//  /// </summary>
//  [Serializable()]
//  public class OracleDBHelper : DBHelper
//  {
//    internal OracleDBHelper()//string connectionString) : base(connectionString)
//    {
//    }


//    protected override void CheckCreateConnection()
//    {
//      if(_connection == null)
//      {
//        _connection = new OracleConnection(_connectionString);
//      }
//    }

//    //public override void BeginTxn()
//    //{
//    //   _internalConnectionControlForTransaction = OpenConnection();

//    //   _transaction = ((OracleConnection)_connection).BeginTransaction();
//    //}

		
//    protected override void InitializeCommand(string sql, CommandType commandType, ArrayList oracleParameterOrSimpleValueList)
//    {
//      _command = new OracleCommand(sql, (OracleConnection)_connection, (OracleTransaction)_transaction);
//      _command.CommandType = commandType;
			
//      if(oracleParameterOrSimpleValueList != null)
//      {
//        if(commandType == CommandType.StoredProcedure)
//        {
//          OracleCommandBuilder.DeriveParameters((OracleCommand)_command);
				
//          for(int i=0; i < ((OracleCommand)_command).Parameters.Count; i++)
//          {
//            object param = (i < oracleParameterOrSimpleValueList.Count ? oracleParameterOrSimpleValueList[i] : DBNull.Value);
//            ((OracleCommand)_command).Parameters[i].Value = 
//              (param is DateTime) ? 
//              new OracleDateTime((DateTime)param) : 
//              param;//Auto filling in of the missing parameters with the default DBNull values.
//          }
//        }
//        else
//        {
//          foreach(OracleParameter parameter in oracleParameterOrSimpleValueList) 
//          {
//            //check for derived output value with no value assigned (as Oracle does not like those...)
//            if ((parameter.Direction == ParameterDirection.InputOutput) && (parameter.Value == null))
//            {
//              parameter.Value = DBNull.Value;
//            }

//            _command.Parameters.Add(parameter);
//          }
//        }
//      }
//    }
//    protected override void InitializeCommand(string sql, CommandType commandType, params IDataParameter[] parameters)
//    {
//      _command = new OracleCommand(sql, (OracleConnection)_connection, (OracleTransaction)_transaction);
//      _command.CommandType = commandType;
			
//      if(parameters != null)
//      {
//        foreach(OracleParameter parameter in parameters) 
//        {
//          //check for derived output value with no value assigned (as Oracle does not like those...)
//          if ((parameter.Direction == ParameterDirection.InputOutput) && (parameter.Value == null))
//          {
//            parameter.Value = DBNull.Value;
//          }

//          _command.Parameters.Add(parameter);
//        }
//      }
//    }
//    protected override void InitializeCommand(string sql, CommandType commandType, params object[] parameters)
//    {
//      _command = new OracleCommand(sql, (OracleConnection)_connection, (OracleTransaction)_transaction);
//      _command.CommandType = commandType;
			
//      if(parameters != null)
//      {
//        if(commandType == CommandType.StoredProcedure)
//        {
//          OracleCommandBuilder.DeriveParameters((OracleCommand)_command);
				
//          for(int i=0; i < ((OracleCommand)_command).Parameters.Count; i++)
//          {
//            object param = (i < parameters.Length ? parameters[i] : DBNull.Value);
//            ((OracleCommand)_command).Parameters[i].Value = 
//              (param is DateTime) ? 
//              new OracleDateTime((DateTime)param) : 
//              param;//Auto filling in of the missing parameters with the default DBNull values.
//          }
//        }
//      }
//    }

//    protected override void InitializeDataAdapter()
//    {
//      _adapter = new OracleDataAdapter();
//    }
//  }

//}
