using System;
using System.Collections.Generic;
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
    public class DacHelper
    {
        public IList<string> Logs = new List<string>();
        public void DeployDacpac(string dacpacFileName, string dbName, string connectionstring)
        {
            dacpacFileName = Path.GetFullPath(dacpacFileName);
            var dacServices = new DacServices(connectionstring);
            dacServices.Message += (sender, args) => Debug.WriteLineIf(Debugger.IsAttached, args.Message);
            dacServices.ProgressChanged += OnDacServerProcessChanged;
            var package = DacPackage.Load(dacpacFileName);
            CancellationToken? cancellationToken = new CancellationToken();
            dacServices.Deploy(package, dbName, true, new DacDeployOptions() {

            }, cancellationToken);
        }

        public static bool DbExists(string dataSource, string dbName)
        {
            var server = new Server(dataSource);
            return server.Databases[dbName] != null;
        }

        private void OnDacServerProcessChanged(object sender, DacProgressEventArgs args)
        {
            Logs.Add(String.Format("[{0}] {1} - {2}", args.OperationId, args.Status, args.Message));
        }

    }
}
