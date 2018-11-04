#Running project

This project is setup to use ASP.NET Core configuration model and supports user secret. 
It simplify storing sensitive informations like password without the risk that it will be commited into repository.

This project requires to setup 3 properties related to Azure IoT Hub. Below is the list of properties and commands that you can use to get their values:
* *EventHubsCompatibleEndpoint*
	`az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {your IoT Hub name}`
* *EventHubsCompatiblePath*
	`az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}`
* *IoTHubSasKey*
	`az iot hub policy show --name iothubowner --query primaryKey --hub-name {your IoT Hub name}`

To configure them open console, change current directory to your project path and run the follwing commands

```
dotnet user-secrets set "EventHubsCompatibleEndpoint" "{value}"
dotnet user-secrets set "EventHubsCompatiblePath" "{value}"
dotnet user-secrets set "IoTHubSasKey" "{value}"
```