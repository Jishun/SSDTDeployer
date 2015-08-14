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
    public class SsdtLocalDbDeployer
    {
        private readonly string _localDbFileName;
        private readonly string _localDbFilePath;
        private readonly DacHelper _helper = new DacHelper();
        private readonly Server _server = new Server(LocalDbDataSource);
        private const string LocalDbServerOnlyConnectionString = @"Data Source=(localdb)\v11.0;Integrated Security=True";


        public readonly string DbName;
        public const string LocalDbDataSource = @"(localdb)\v11.0";
        public const string LocalDbConnectionString = @"Data Source=(localdb)\v11.0; AttachDBFilename='|DataDirectory|\{0}'; Integrated Security=True";

        public SsdtLocalDbDeployer(string localDbFilePath, bool createIfNotExists = false)
        {
            var fullPath = Path.GetFullPath(localDbFilePath);
            _localDbFileName = Path.GetFileName(localDbFilePath);
            _localDbFilePath = Path.GetDirectoryName(fullPath);
            AppDomain.CurrentDomain.SetData("DataDirectory", _localDbFilePath);
            if (!File.Exists(localDbFilePath) && createIfNotExists)
            {
                DbName = CreateLocalDb(fullPath);
            }
            else
            {
                using (var connection = new SqlConnection(String.Format(LocalDbConnectionString, _localDbFileName)))
                {
                    connection.Open();
                    DbName = connection.Database;
                    connection.Close();
                }
            }
        }

        public IList<string> DeployDacPac(string dacpacFileName, bool detach = true)
        {
            if (DbName == null)
            {
                return new[] {"Db does not exist"};
            }
            _helper.DeployDacpac(dacpacFileName, DbName, LocalDbServerOnlyConnectionString);
            if (detach)
            {
                DetachDb();
            }
            return _helper.Logs;
        }

        public void DetachDb()
        {
            var database = _server.Databases[DbName];
            if (database != null)
            {
                try
                {
                    _server.KillAllProcesses(DbName);
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
                _server.DetachDatabase(DbName, true);
            }
        }
        private string CreateLocalDb(string fileName)
        {
            var fullPath = Path.GetFullPath(fileName);
            fileName = Path.GetFileName(fileName);
            var db = new Database(_server, fullPath);
            db.FileGroups.Add(new FileGroup(db, "PRIMARY"));
            db.FileGroups[0].Files.Add(new DataFile(db.FileGroups[0], Path.GetFileNameWithoutExtension(fileName), fullPath));
            db.Create();
            return fullPath;
        }
    }
}
