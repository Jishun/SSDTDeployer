# SSDTDeployer
A simple lib reference Microsoft.Sqlserver.Dacpac and  Microsoft.Sqlserver.SMO to handle Dacpac deploy and clean up to sql server instance or LocalDB, especially useful for unit testing

## Get Started
- Search for SSDTDeployer in nuget package manager.
- Create an instance of SSDTDeployer.SsdtServerDeployer to handle sql server instance
- Or create an instance of SSDTDeployer.SsdtLocalDbDeployer to handle LocalDB
- Call methods "DeployDacPac", "DetachDb", "DeleteDb", "CreateDb"
- All good!
	
## Example code
- Code from test: [LocalDbTest](https://github.com/djsxp/SSDTDeployer/blob/master/project/DeployerTest/LocalDb.cs)
```cs
var deployer = new SsdtLocalDbDeployer(dbName + ".mdf", true); //true: create if not exists
deployer.DeployDacPac(Utils.Dacpac);
```
- Using sql instance: [SqlInstanceTest](https://github.com/djsxp/SSDTDeployer/blob/master/project/DeployerTest/SqlInstanceTest.cs)
```cs
var dbName = "TestDeployerLocalSqlInstance";
var connectString = String.Format(Utils.LocalInstanceConnectionString, dbName);
var deployer = new SsdtServerDeployer(connectString, dbName, true);
deployer.DeployDacPac(Utils.Dacpac);
```
## Remarks
- When doing unit testing with it, we'd better put the creation of data bases in the initialization method and put the removing/clean up in the clean-up method, 
- keep in mind that not to force the testing to stop but let it go through even if it's broken by an exception, which allows the clean-up method to get the db server instance cleared
	
## Known issues:
- "Unable to cast object of type 'System.DBNull' to type 'System.String'." exception when trying to create instance of SsdtLocalDbDeployer
- Cause: the deployer failed to create FileGroup "Primary" for the db, maybe due to previous uncleared enviroment
- Workaround. use management studio to connect to (localdb)\V11.0 to manually remove the db (identified by file path), it will probably give another error while doing the removal, but it will do the job.
- Solution: working on it...
