using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Management.Smo;

namespace SSDTDeployer
{
    public class SsdtServerDeployer
    {
        private readonly string _connectionString;
        private readonly string _dbName;
        private readonly DacHelper _helper = new DacHelper();
        private readonly Server _server;

        public SsdtServerDeployer(string serverConnectionString, string dbName, bool createIfNotExists = false)
        {
            _connectionString = serverConnectionString;
            _dbName = dbName;
            using (var connection = new SqlConnection(serverConnectionString))
            {
                _server = new Server(connection.DataSource);
            }
            if (createIfNotExists && _server.Databases[_dbName] == null)
            {
                CreatDb();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dacpacFileName"></param>
        /// <returns>The deploy progress trace string list</returns>
        public IList<string> DeployDacPac(string dacpacFileName)
        {
            _helper.DeployDacpac(dacpacFileName, _dbName, _connectionString);
            return _helper.Logs;
        }

        public void CreatDb(string fileName = null)
        {
            var db = new Database(_server, _dbName);
            if (fileName != null)
            {
                fileName = Path.GetFullPath(fileName);
                db.FileGroups.Add(new FileGroup(db, "PRIMARY"));
                db.FileGroups[0].Files.Add(new DataFile(db.FileGroups[0], _dbName, fileName));
            }
            db.Create();
        }

        public void DeleteDb()
        {
            var database = _server.Databases[_dbName];
            if (database != null)
            {
                try
                {
                    _server.KillAllProcesses(_dbName);
                }
                catch (Exception)
                {

                }
                try
                {
                    database.DatabaseOptions.UserAccess = DatabaseUserAccess.Single;
                }
                catch (Exception e)
                {

                }
                try
                {
                    database.Alter(TerminationClause.RollbackTransactionsImmediately);
                }
                catch (Exception e)
                {

                }
                database.Drop();
            }
        }
    }
}
