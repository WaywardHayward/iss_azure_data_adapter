# ISS Azure Data Adapter
A fun side project, This repository contains a dotnet app service which listens to data from the ISS lightstreamer service and sends json messages to a specified Azure Eventhub.

## Architecture

The architecture is as follows:

![architecture](./Docs/architecture.png)

## App Settings

The Following settings are required on top of the standard azure app service settings to run the function.

| Setting Name | Type | Description |
| ------------ | ---- | ----------- |
| EVENT_HUB_CONNECTION_STRING | string | The connection string to the event hub. |
| EVENT_HUB_NAME | string | The name of the event hub. |

## Json Payload

The schema of the payload for an ISS Telemetry Update is as follows:

```jsonschema

  "$schema": "http://json-schema.org/draft-04/schema#",
  "type": "object",
  "properties": {
    "Discipline": {
      "type": "string"
    },
    "MessageType": {
      "type": "string"
    },
    "Status": {
      "type": "string"
    },
    "Indicator": {
      "type": "string"
    },
    "Color": {
      "type": "string"
    },
    "TimeStamp": {
      "type": "string"
    },
    "Value": {
      "type": "string"
    },
    "CalibratedData": {
      "type": "string"
    },
    "PUI": {
      "type": "string"
    },
    "Public_PUI": {
      "type": "string"
    },
    "Description": {
      "type": "string"
    },
    "MIN": {
      "type": "string"
    },
    "MAX": {
      "type": "string"
    },
    "OPS_NOM": {
      "type": "string"
    },
    "ENG_NOM": {
      "type": "string"
    },
    "UNITS": {
      "type": "string"
    },
    "ENUM": {
      "type": "string"
    },
    "Format_Spec": {
      "type": "string"
    }
  },
  "required": [
    "Discipline",
    "MessageType",
    "Status",
    "Indicator",
    "Color",
    "TimeStamp",
    "Value",
    "CalibratedData",
    "PUI",
    "Public_PUI",
    "Description",
    "MIN",
    "MAX",
    "OPS_NOM",
    "ENG_NOM",
    "UNITS",
    "ENUM",
    "Format_Spec"
  ]
}

``` 