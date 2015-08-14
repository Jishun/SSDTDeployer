using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployerTest
{
    public static class Utils
    {

#if DEBUG
        public const string Dacpac = @"..\..\..\DeployerTestDb\bin\Debug\DeployerTestDb.dacpac";
#else
        public const string Dacpac = @"..\..\..\DeployerTestDb\bin\Release\DeployerTestDb.dacpac";
#endif
        public const string LocalInstanceConnectionString = @"Data Source=.;Initial Catalog={0};Integrated Security=True;";
    }
}
