using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSDTDeployer;

namespace DeployerTest
{
    [TestClass]
    public class LocalDbTest
    {
        [TestMethod]
        public void TestDeployerLocalDb()
        {
            var dbName = "TestDeployerLocalSqlInstance";
            if (File.Exists(dbName + ".mdf"))
            {
                File.Delete(dbName + ".mdf");
            }
            if (File.Exists(dbName + "_log.ldf"))
            {
                File.Delete(dbName + "_log.ldf");
            }
            var deployer = new SsdtLocalDbDeployer(dbName + ".mdf", true);
            deployer.DeployDacPac(Utils.Dacpac);
            Assert.IsFalse(DacHelper.DbExists(SsdtLocalDbDeployer.LocalDbDataSource, deployer.DbName));
            var connectString = String.Format(SsdtLocalDbDeployer.LocalDbConnectionString, dbName + ".mdf");
            using (var c = new SqlConnection(connectString))
            {
                c.Open();
                using (var cmd = c.CreateCommand())
                {
                    cmd.CommandText = "TRUNCATE TABLE TESTTABLE";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                c.Close();
            }
            deployer.DetachDb();
            File.Delete(dbName + ".mdf");
            File.Delete(dbName + "_log.ldf");
        }

        [TestMethod]
        public void TestUsingExistingDb()
        {
            var fileName = "DeployerTestLocalDb2.mdf";
            var logName = fileName.Replace(".mdf", "_log.ldf");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            if (File.Exists(logName))
            {
                File.Delete(logName);
            }
            File.Copy("DeployerTestLocalDb.mdf", fileName);
            var deployer = new SsdtLocalDbDeployer(fileName, false);
            deployer.DeployDacPac(Utils.Dacpac, false);
            var connectString = String.Format(SsdtLocalDbDeployer.LocalDbConnectionString, fileName);
            using (var c = new SqlConnection(connectString))
            {
                c.Open();
                using (var cmd = c.CreateCommand())
                {
                    cmd.CommandText = "TRUNCATE TABLE TESTTABLE";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                c.Close();
            }
            deployer.DetachDb();
            File.Delete(fileName);
            File.Delete(logName);
        }
    }
}
