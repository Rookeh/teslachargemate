# teslachargemate
An add-on worker service for the [TeslaMate](https://github.com/adriankumpf/teslamate) project which automatically updates the energy cost associated with a geofence, based on the time of day.

This may be useful for TeslaMate users on [Economy 7](https://en.wikipedia.org/wiki/Economy_7) or similar variable-rate energy tariffs, to accurately record the cost of charging sessions.

## Usage

Simply build and run on your desired platform.

Alternatively, grab the latest image from the releases page and deploy in the same environment as your TeslaMate installation.

## Requirements

The following environment variables must be populated:

| Name                     | Example Value       | Notes                                             |
|:------------------------:|:-------------------:|---------------------------------------------------|
| `TCM_DATABASE_HOST`      | `127.0.0.1`         | The host of your TeslaMate Postgres database      |
| `TCM_DATABASE_PORT`      | `5432`              | The port that Postgres is listening on            |
| `TCM_DATABASE_NAME`      | `teslamate`         | Name of the TeslaMate database                    |
| `TCM_DATABASE_USER`      | `teslamate`         | User associated with TeslaMate database           |
| `TCM_DATABASE_PASSWORD`  | `password`          | Password for the above user                       |
| `TCM_GEOFENCE_ID`        | `1`                 | ID of the geofence that should be updated         |
| `TCM_DAY_RATE`           | `0.14`              | Daytime energy tariff cost (per kWh)              |
| `TCM_NIGHT_RATE`         | `0.05`              | Overnight energy tariff cost (per kWh)            |
| `TCM_DAY_START`          | `04:30:00`          | The time of day your daytime tariff rate begins   |
| `TCM_NIGHT_START`        | `00:30:00`          | The time of day your overnight tariff rate begins |

The following environment variables are optional:

| Name                     | Example Value       | Notes                                                                                                          |
|:------------------------:|:-------------------:|----------------------------------------------------------------------------------------------------------------|
| `TCM_DATABASE_RETRIES`   | `3`                 | Number of times to retry setting charge rate in case of DB connectivity issues (will not retry if not defined) |
| `TCM_DATABASE_WAIT`      | `5`                 | Seconds to wait between retry attempts (will not wait if not defined)                                          |
| `TZ`                     | `Europe/London`     | Your timezone (this may need to be set to ensure rates change when expected)                                   |
