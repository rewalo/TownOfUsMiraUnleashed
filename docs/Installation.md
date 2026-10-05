# Installation

## Requirements

- PC version of Among Us (not console)
- [Town of Us: Mira](https://github.com/AU-Avengers/TOU-Mira) **1.7.3** (bundles MiraAPI 0.5.0 and Reactor 2.5.0)

## Steps

1. Install TOU-Mira per its own instructions and launch the game once.
2. Download `MiraUnleashed.dll` from the [Releases page](https://github.com/rewalo/TownOfUsMiraRolesExtension/releases) (or build from source — see [Building](Building)).
3. Copy `MiraUnleashed.dll` into `Among Us/BepInEx/plugins/`.
4. Launch the game. "Mira Unleashed" should appear in the mods list, and role settings gain Mira Unleashed entries.

Every player in a lobby needs the mod (it is marked `RequireOnAllClients`).

## Config file

`BepInEx/config/rewalo.mira.unleashed.cfg` — only technical settings live there; all gameplay options are configured in-game (see [Configuration](Configuration)).
