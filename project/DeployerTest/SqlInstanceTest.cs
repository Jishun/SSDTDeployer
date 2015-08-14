using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSDTDeployer;

namespace DeployerTest
{
    [TestClass]
    public class SqlInstanceTest
    {
        [TestMethod]
        public void TestDeployerLocalSqlInstance()
        {
            var dbName = "TestDeployerLocalSqlInstance";
            var connectString = String.Format(Utils.LocalInstanceConnectionString, dbName);
            var deployer = new SsdtServerDeployer(connectString, dbName, true);
            deployer.DeployDacPac(Utils.Dacpac);
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
            deployer.DeleteDb();
            try
            {
                using (var c = new SqlConnection(connectString))
                {
                    c.Open();
                }
                Assert.Fail("Should have thrown Exception");
            }
            catch (SqlException)
            {
            }
        }

        [TestMethod]
        public void TestCreateSqlDb()
        {
            var dbName = "TestDeployerCraeteOnLocalSqlInstance";
            var connectString = String.Format(Utils.LocalInstanceConnectionString, dbName);
            if (DacHelper.DbExists(".", dbName))
            {
                var deployer2 = new SsdtServerDeployer(connectString, dbName, false);
                deployer2.DeleteDb();
            }
            Assert.IsFalse(DacHelper.DbExists(".", dbName));
            var deployer = new SsdtServerDeployer(connectString, dbName, false);
            var name = dbName + ".mdf";
            deployer.CreatDb(name);
            Assert.IsTrue(File.Exists(name));
            using (var c = new SqlConnection(connectString))
            {
                c.Open();
                c.Close();
            }
            deployer.DeleteDb();
            Assert.IsFalse(File.Exists(name));
            try
            {
                using (var c = new SqlConnection(connectString))
                {
                    c.Open();
                }
                Assert.Fail("Should have thrown Exception");
            }
            catch (SqlException)
            {
            }
        }
    }
}
