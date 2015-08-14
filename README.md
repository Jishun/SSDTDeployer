# SSDTDeployer
A simple lib reference Microsoft.Sqlserver.Dacpac and  Microsoft.Sqlserver.SMO to handle Dacpac deploy and clean up to sql server instance or LocalDB, especially useful for unit testing

## Get Started
	- Search for SSDTDeployer in nuget package manager.
	- Create an instance of SSDTDeployer.SsdtServerDeployer to handle sql server instance
	- Or create an instance of SSDTDeployer.SsdtLocalDbDeployer to handle LocalDB
	- Call methods "DeployDacPac", "DetachDb", "DeleteDb", "CreateDb"
	- All good!
