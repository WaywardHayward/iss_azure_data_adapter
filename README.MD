# ISS DATA
This repository contains an aspnetcore service which listens to data from the ISS and sends it to a specified Azure Eventhub.

![architecture](./Docs/architecture.png)

## App Settings

The Following settings are required on top of the standard azure app service settings to run the function.


| Setting Name | Type | Description |
| ------------ | ---- | ----------- |
| EVENT_HUB_CONNECTION_STRING | string | The connection string to the event hub. |
| EVENT_HUB_NAME | string | The name of the event hub. |




## License
MIT License

## Contributing
