# Vatsim Plugin for Macro Deck
This simple Macro Deck Plugin allows for basic interaction with the Vatsim API

## Features
### METAR
Get current METAR Reports from the Vatsim API.
Reports are stored in a variable (`vatsim_metar_{icao}`) and updated every minute.
Airports are configurable in the configurator

### Current ATC Station
If you are online as ATC, you can display your current callsign (`vatsim_station`) and your time online (`vatsim_elapsed`)
You have to set your Vatsim ID in the configurator

### Current time with seconds
`vatsim_time_sec`
Because why not

## Planned Features
- Current status as pilot
  - Callsign
  - Destination
  - Time left
  - ...
- ATIS interaction (via vATIS)
