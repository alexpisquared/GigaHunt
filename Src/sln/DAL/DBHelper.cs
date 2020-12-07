//#define DbgWr
using AAV.Sys.Ext;
//using AsLink;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;

namespace DAL
{
  /// <summary>
  /// Abstract definitions and provider neutral reusable base implementation of expected behavior
  /// </summary>
  [Serializable()]
  public abstract class DBHelper : IDisposable
  {
    protected internal IDbCommand _command;
    protected internal IDbConnection _connection;
    protected internal IDbTransaction _transaction;
    protected internal IDbDataAdapter _adapter;
    protected internal string _connectionString;
    readonly bool _internalConnectionControlForTransaction = false;
    string[] _connectionStrings;
    bool _disposed;//FxCop = false;

    internal DBHelper()
    {
    }
    //internal DBHelper(string connectionString)
    //{
    //   _connectionString = connectionString;
    //}

    public bool OpenConnection()
    {
      CheckCreateConnection();
      return smartOpenConnection();
    }
    public void CloseConnection()
    {
      if (_connection != null)
      {
        if (_connection.State != ConnectionState.Closed)
        {
          _connection.Close();
        }

        _connection.Dispose();
        _connection = null;
      }
    }

    public void BeginTxn() => _transaction = _connection.BeginTransaction();
    public void CommitTxn()
    {
      if (_transaction != null)
      {
        _transaction.Commit();
        _transaction.Dispose();
        _transaction = null;
      }

      if (_internalConnectionControlForTransaction)
      {
        CloseConnection();
      }
    }
    public void RollbackTxn()
    {
      if (_transaction != null)
      {
        _transaction.Rollback();
        _transaction.Dispose();
        _transaction = null;
      }

      if (_internalConnectionControlForTransaction)
      {
        CloseConnection();
      }
    }

    public string ConnectionString
    {
      get => _connectionString;
      internal set
      {
        _connectionString = value;
        _connectionStrings = new string[1];
        _connectionStrings[0] = value;
      }
    }
    internal string[] ConnectionStrings
    {
      set
      {
        _connectionStrings = value;
        _connectionString = value[0];
      }
    }

    protected abstract void CheckCreateConnection();
    private bool smartOpenConnection()
    {
      if (_connection.State == ConnectionState.Open)
        return false;
      else
      {
        for (var i = 0; i < _connectionStrings.Length; i++)
        {
          _connectionString = _connection.ConnectionString = _connectionStrings[i];
          var started = DateTime.Now;

          try
          {
            //new System.Media.SoundPlayer(@"C:\windows\Media\Windows XP Startup.wav").Play();
            _connection.Open();
            //new System.Media.SoundPlayer(@"C:\windows\Media\Windows XP Menu Command.wav").Play();

            for (var j = 0; j < i; j++)//replace bad constrs with the good one:
              _connectionStrings[j] = _connectionStrings[i];

            return true;
          }
          catch (System.Data.Common.DbException ex)
          {
            Trace.WriteLine($"**Expected error in openConnection(): \r\n {ex.Message} \r\n ConnectionTimeout/TimeSpent (sec): {_connection.ConnectionTimeout:N1}/{2:N1}. ");
          }
          catch (Exception ex)
          {
            ex.Log();
            throw;
          }
        }
      }

      var s = "";
      foreach (var cs in _connectionStrings) s += "\n" + cs;

      throw new Exception(string.Format("Unable to open DB connection on all {0} connection strings: {1}", _connectionStrings.Length, s));
    }

    public DataSet ExecuteDataSetSP(string sql) => ExecuteDataSetSP(sql, (ArrayList)null);
    public int ExecuteNonQuerySP(string sql) => ExecuteNonQuerySP(sql, (ArrayList)null);
    public IDataReader ExecuteReaderSP(string sql) => ExecuteReaderSP(sql, (ArrayList)null);
    public object ExecuteScalarSP(string sql) => ExecuteScalarSP(sql, (ArrayList)null);

    public DataSet ExecuteDataSetText(string sql) => ExecuteDataSetText(sql, null);
    public int ExecuteNonQueryText(string sql) => ExecuteNonQueryText(sql, null);
    public IDataReader ExecuteReaderText(string sql) => ExecuteReaderText(sql, null);
    public object ExecuteScalarText(string sql) => ExecuteScalarText(sql, null);

    public DataSet ExecuteDataSetSP(string sql, ArrayList parameters) => executeDataSet(sql, CommandType.StoredProcedure, parameters);
    public int ExecuteNonQuerySP(string sql, ArrayList parameters) => executeNonQuery(sql, CommandType.StoredProcedure, parameters);
    public IDataReader ExecuteReaderSP(string sql, ArrayList parameters) => executeReader(sql, CommandType.StoredProcedure, parameters);
    public object ExecuteScalarSP(string sql, ArrayList parameters) => executeScalar(sql, CommandType.StoredProcedure, parameters);

    public DataSet ExecuteDataSetSP(string sql, params IDataParameter[] parameters) => executeDataSet(sql, CommandType.StoredProcedure, parameters);
    public int ExecuteNonQuerySP(string sql, params IDataParameter[] parameters) => executeNonQuery(sql, CommandType.StoredProcedure, parameters);
    public IDataReader ExecuteReaderSP(string sql, params IDataParameter[] parameters) => executeReader(sql, CommandType.StoredProcedure, parameters);
    public object ExecuteScalarSP(string sql, params IDataParameter[] parameters) => executeScalar(sql, CommandType.StoredProcedure, parameters);

    public DataSet ExecuteDataSetSP(string sql, params object[] paramValues) => executeDataSet(sql, CommandType.StoredProcedure, paramValues);
    public int ExecuteNonQuerySP(string sql, params object[] paramValues) => executeNonQuery(sql, CommandType.StoredProcedure, paramValues);
    public IDataReader ExecuteReaderSP(string sql, params object[] paramValues) => executeReader(sql, CommandType.StoredProcedure, paramValues);
    public object ExecuteScalarSP(string sql, params object[] paramValues) => executeScalar(sql, CommandType.StoredProcedure, paramValues);

    public DataSet ExecuteDataSetText(string sql, params IDataParameter[] parameters) => executeDataSet(sql, CommandType.Text, parameters);
    public int ExecuteNonQueryText(string sql, params IDataParameter[] parameters) => executeNonQuery(sql, CommandType.Text, parameters);
    public IDataReader ExecuteReaderText(string sql, params IDataParameter[] parameters) => executeReader(sql, CommandType.Text, parameters);
    public object ExecuteScalarText(string sql, params IDataParameter[] parameters) => executeScalar(sql, CommandType.Text, parameters);

    private DataSet executeDataSet(string sql, CommandType commandType, ArrayList parameters)
    {
      var retval = new DataSet
      {
        Locale = System.Globalization.CultureInfo.InvariantCulture
      };

      InitializeDataAdapter();

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);
        _adapter.SelectCommand = _command;
#if DbgWr
				Trace.WriteLine("-=Execute Fill(): " + sql);
#endif
        _adapter.Fill(retval);
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeDataSet: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          CloseConnection();
        }
      }

      return retval;
    }
    private int executeNonQuery(string sql, CommandType commandType, ArrayList parameters)
    {
      var retval = -1;

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);

#if DbgWr
				Trace.WriteLine("-=ExecuteNonQuery(): " + sql);
#endif
        retval = _command.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeScalar: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          CloseConnection();
        }
      }

      return retval;
    }
    private IDataReader executeReader(string sql, CommandType commandType, ArrayList parameters)
    {
      IDataReader retval = null;

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);

#if DbgWr
				Trace.WriteLine("-=ExecuteReader(): " + sql);
#endif
        retval = _command.ExecuteReader();
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeScalar: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          //For executeReader...(...)  it's the caller's responsibility to call CloseConnection();
        }
      }

      return retval;
    }
    private object executeScalar(string sql, CommandType commandType, ArrayList parameters)
    {
      object retval = null;

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);

#if DbgWr
				Trace.WriteLine("-=ExecuteScalar(): " + sql);
#endif
        retval = _command.ExecuteScalar();
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeScalar: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          CloseConnection();
        }
      }

      return retval;
    }

    private DataSet executeDataSet(string sql, CommandType commandType, params IDataParameter[] parameters)
    {
      var retval = new DataSet
      {
        Locale = System.Globalization.CultureInfo.InvariantCulture
      };

      InitializeDataAdapter();

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);
        _adapter.SelectCommand = _command;
#if DbgWr
				Trace.WriteLine("-=Execute Fill(): " + sql);
#endif
        _adapter.Fill(retval);
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeDataSet: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          CloseConnection();
        }
      }

      return retval;
    }
    private int executeNonQuery(string sql, CommandType commandType, params IDataParameter[] parameters)
    {
      var retval = -1;

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);

#if DbgWr
				Trace.WriteLine("-=ExecuteNonQuery(): " + sql);
#endif
        retval = _command.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeScalar: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          CloseConnection();
        }
      }

      return retval;
    }
    private IDataReader executeReader(string sql, CommandType commandType, params IDataParameter[] parameters)
    {
      IDataReader retval = null;

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);

#if DbgWr
				Trace.WriteLine("-=ExecuteReader(): " + sql);
#endif
        retval = _command.ExecuteReader();
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeScalar: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          //For executeReader...(...)  it's the caller's responsibility to call CloseConnection();
        }
      }

      return retval;
    }
    private object executeScalar(string sql, CommandType commandType, params IDataParameter[] parameters)
    {
      object retval = null;

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);

#if DbgWr
				Trace.WriteLine("-=ExecuteScalar(): " + sql);
#endif
        retval = _command.ExecuteScalar();
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeScalar: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          CloseConnection();
        }
      }

      return retval;
    }

    private DataSet executeDataSet(string sql, CommandType commandType, params object[] parameters)
    {
      var retval = new DataSet
      {
        Locale = System.Globalization.CultureInfo.InvariantCulture //FxCop
      };

      InitializeDataAdapter();

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);
        _adapter.SelectCommand = _command;
#if DbgWr
				Trace.WriteLine("-=ExecuteScalar(): " + sql);
#endif
        _adapter.Fill(retval);
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeDataSet: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          CloseConnection();
        }
      }

      return retval;
    }
    private int executeNonQuery(string sql, CommandType commandType, params object[] parameters)
    {
      var retval = -1;

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);

#if DbgWr
				Trace.WriteLine("-=ExecuteNonQuery(): " + sql);
#endif
        retval = _command.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeScalar: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          CloseConnection();
        }
      }

      return retval;
    }
    private IDataReader executeReader(string sql, CommandType commandType, params object[] parameters)
    {
      IDataReader retval = null;

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);

#if DbgWr
				Trace.WriteLine("-=ExecuteReader(): " + sql);
#endif
        retval = _command.ExecuteReader();
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeScalar: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          //For executeReader...(...)  it's the caller's responsibility to call CloseConnection();
        }
      }

      return retval;
    }
    private object executeScalar(string sql, CommandType commandType, params object[] parameters)
    {
      object retval = null;

      var needsClosing = OpenConnection();

      try
      {
        InitializeCommand(sql, commandType, parameters);

#if DbgWr
				Trace.WriteLine("-=ExecuteScalar(): " + sql);
#endif
        retval = _command.ExecuteScalar();
      }
      catch (Exception ex)
      {
        Trace.WriteLine("**Error in executeScalar: " + ex.Message);
        throw;
      }
      finally
      {
        _command.Parameters.Clear();

        if (needsClosing)
        {
          CloseConnection();
        }
      }

      return retval;
    }


    protected abstract void InitializeCommand(string sql, CommandType commandType, ArrayList parameters);
    protected abstract void InitializeCommand(string sql, CommandType commandType, params IDataParameter[] parameters);
    protected abstract void InitializeCommand(string sql, CommandType commandType, params object[] parameters);
    protected abstract void InitializeDataAdapter();

    #region IDisposable Members and clean up logic.

    //protected override void Dispose(bool disposing)
    //{
    //   if (disposing)
    //   {
    //      if (components != null)
    //      {
    //         components.Dispose();
    //      }
    //   }
    //   base.Dispose(disposing);
    //}


    public void /*IDisposable.*/Dispose()
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
          CommitTxn();
          CloseConnection();
        }
      }

      _disposed = true;
    }

    ~DBHelper()
    {
      Dispose(false);
      //Dispose();//FxCop
    }



    #endregion
  }
}