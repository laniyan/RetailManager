using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace RMDataManager.Library.Internal.DataAccess
{
    //connection to our db
    internal class SqlDataAccess : IDisposable
    {
        private readonly IConfiguration _config;

        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }
        public string GetConnectionString(string name)
        {
            return _config.GetConnectionString(name);//this reads the json verison so it goes and gets the value from appsetting.json
            //return ConfigurationManager.ConnectionStrings[name].ConnectionString; this no longer works it cant read the connection string from web.cofig anymore
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))//this opens up the connection
            {
                List<T> rows = connection
                    .Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);//Excute is a extision method from dapper
            }
        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool isClosed;

        //Open connect/start transaction method
        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            _connection = new SqlConnection(connectionString); //we cant use the using(){ } bcoz it closes at the end of the } and we need it open longer than that

            _connection.Open();

            _transaction = _connection.BeginTransaction();// this starts of the connection

            isClosed = false;

        }

        //save using the transaction
        public void SaveDataInTransaction<T>(string storedProcedure, T parameters, string connectionStringName)
        {

            _connection.Execute(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure, 
                transaction: _transaction);//have to pass in transaction or it wont use the transaction for its call we are saying this call is part of the overall trans we are running 
            //we use :transaction becoz this methods has a lot of default values in the params so to pick and choose what param you want to pass in u use the param name and :
        }

        //load using the transaction
        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            List<T> rows = _connection
                .Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

            return rows;

        }

        //Close connection/stop transaction method
        //we only call this method onces the transaction has been successful 
        public void CommitTransaction()
        {
            _transaction?.Commit();//the ? are null check meaning if the obj (transaction) is null then it wont do the work so we can call this multiple times coz if its null it will skip it
            _connection?.Close();

            isClosed = true;
        }

        //Close connection/stop transaction method
        //we call this if transaction fails to rollback all changes
        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _connection?.Close();

            isClosed = true;
        }

        //Dispose
        /*this is normally used at the end of a using statement which imp its own version to close() but coz we aint using that we have to call it ourselfs and
          imp with our own close()*/
        public void Dispose()
        {
            if (!isClosed)
            {
                CommitTransaction(); //this closes the transaction and the connection
            }

            //no matter what happens we have to always set these to close once this method has been called just a 2nd safey
            _transaction = null;
            _connection = null;
            
        }
    }
}
