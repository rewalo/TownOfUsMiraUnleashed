> [!NOTE]
> This repo is an extension mod for [Town of Us: Mira](https://github.com/AU-Avengers/TOU-Mira) that adds new roles and modifiers.
> This mod requires Town of Us: Mira to be installed and is NOT for console versions of Among Us.

-----------------------

<div align="center">
  <img src="./docs/assets/Logo.png" alt="Mira Unleashed"/>
  <p><b>Mira Unleashed</b></p>
  <p>formerly Town of Us Mira Roles Extension</p>
</div>
<br/>

An extension mod for [Town of Us: Mira](https://github.com/AU-Avengers/TOU-Mira) that adds new roles and modifiers to enhance your gameplay experience!

[![Build](https://github.com/rewalo/TownOfUsMiraRolesExtension/actions/workflows/build.yml/badge.svg)](https://github.com/rewalo/TownOfUsMiraRolesExtension/actions/workflows/build.yml)
[![Latest Release](https://img.shields.io/github/v/release/rewalo/TownOfUsMiraRolesExtension)](https://github.com/rewalo/TownOfUsMiraRolesExtension/releases/latest)
[![License: GPL-3.0](https://img.shields.io/badge/license-GPL--3.0-blue.svg)](./LICENSE)
[![Requires TOU-Mira 1.7.3](https://img.shields.io/badge/TOU--Mira-1.7.3-red.svg)](https://github.com/AU-Avengers/TOU-Mira)

-----------------------

# Contents

- [**Contents**](#contents)
- [**Roles & Modifiers**](#roles--modifiers)
- [**Installation**](#installation)
- [**Compatibility**](#compatibility)
- [**Building**](#building)
- [**Branches**](#branches)
- [**Documentation**](#documentation)
- [**Contributing**](#contributing)
- [**Credits**](#credits)
- [**License**](#license)
- [**Copyright**](#copyright)

-----------------------

# Roles & Modifiers

<p align="center">
  <img src="./docs/assets/Groups/CrewSupport.png" align="center" />
  <a href="./docs/Forestaller.md"><img width="30%" src="./docs/assets/RoleHeaders/Forestaller.png" /></a>
  <a href="./docs/Mirage.md"><img width="30%" src="./docs/assets/RoleHeaders/Mirage.png" /></a>
  <a href="./docs/Trapper.md"><img width="30%" src="./docs/assets/RoleHeaders/Trapper.png" /></a>
  <img src="./docs/assets/Groups/ImpSupport.png" align="center" />
  <a href="./docs/Charlatan.md"><img width="30%" src="./docs/assets/RoleHeaders/Charlatan.png" /></a>
  <a href="./docs/Hacker.md"><img width="30%" src="./docs/assets/RoleHeaders/Hacker.png" /></a>
  <a href="./docs/Injector.md"><img width="30%" src="./docs/assets/RoleHeaders/Injector.png" /></a>
  <a href="./docs/Witch.md"><img width="30%" src="./docs/assets/RoleHeaders/Witch.png" /></a>
  <img src="./docs/assets/Groups/ImpKilling.png" align="center" />
  <a href="./docs/Wraith.md"><img width="30%" src="./docs/assets/RoleHeaders/Wraith.png" /></a>
  <img src="./docs/assets/Groups/NeutBenign.png" align="center" />
  <a href="./docs/Lawyer.md"><img width="30%" src="./docs/assets/RoleHeaders/Lawyer.png" /></a>
  <img src="./docs/assets/Groups/NeutKilling.png" align="center" />
  <a href="./docs/Serial-Killer.md"><img width="30%" src="./docs/assets/RoleHeaders/Serial Killer.png" /></a>
  <img src="./docs/assets/Groups/NeutEvil.png" align="center" />
  <a href="./docs/Scavenger.md"><img width="30%" src="./docs/assets/RoleHeaders/Scavenger.png" /></a>
  <img src="./docs/assets/Groups/UniMods.png" align="center" />
  <a href="./docs/Clueless.md"><img width="30%" src="./docs/assets/ModifierHeaders/Clueless.png" /></a>
  <a href="./docs/Spiteful.md"><img width="30%" src="./docs/assets/ModifierHeaders/Spiteful.png" /></a>
</p>

## Crewmate Roles

### Forestaller (Support)
Complete all tasks to disable sabotages while alive. Revealed in meetings after completing all tasks.

### Mirage (Support)
Place a decoy with the appearance of a chosen target (yourself or a random player). If any player interacts with the decoy, it disappears instantly and both the Mirage and the toucher receive a notification. Cannot be guessed if the decoy has the appearance of yourself.

### Trapper (Support)
Place traps on vents that immobilize players who use them. Not to be confused with TOU-Mira's Trapper, which is renamed to Revealer while this mod is loaded.

## Impostor Roles

### Charlatan (Support)
Manipulate body reports. Deceive lets you report bodies you killed from any distance for a limited time after killing. Conceal reduces the report range of a nearby body, but requires you to stay near the body while channeling.

### Hacker (Support)
Download information from nearby equipment (Admin/Cams/Vitals/Door Log) to charge a portable device. Jam disrupts information systems like comms being sabotaged, but emergency meetings can still be called. Gain jam charges from kills.

### Injector (Support)
Inject non-impostor players with a syringe that applies a random effect after a delay. Effects can be negative or positive. Starts with a limited number of uses and gains additional uses from kills.

### Witch (Killing)
Cast spells on players to curse them. Spellbound players are highlighted in the next meeting and die after a configured amount of meetings. If the Witch dies, gets exiled, or is guessed, all spellbound players survive.

### Wraith (Power)
Dash increases movement speed by 75% for a short time. Lantern lets you place a hidden marker only you can see; reactivate it to teleport back and briefly turn invisible. If the Lantern expires before returning, it breaks and leaves permanent evidence.

## Neutral Roles

### Lawyer (Benign)
Win by keeping your assigned client from being voted out. If your client gets voted out, you lose. Can object to votes during meetings to force players to vote again.

### Serial Killer (Killing)
Kill everyone to win alone. Can optionally kill players who are in vents with them, but loses the ability to vent for the rest of the game after doing so.

### Scavenger (Evil)
Eat dead bodies to win alone. Optionally, use Scavenge to get arrows pointing to all corpses. If the win condition becomes impossible, the Scavenger becomes a configured role.

## Modifiers

### Clueless (Universal)
Removes all task guidance (task list, task arrows/markers, and map task locations). Tasks still function normally and contribute to the task bar.

### Spiteful (Universal)
When you are voted out, everyone who voted for you receives a negative effect (lower vision, slowness, or increased cooldowns) for a configured number of rounds or the rest of the game.

-----------------------

# Installation

1. Install [Town of Us: Mira](https://github.com/AU-Avengers/TOU-Mira) 1.7.3 (which includes MiraAPI and its dependencies).
2. Download the latest `MiraUnleashed.dll` from [Releases](https://github.com/rewalo/TownOfUsMiraRolesExtension/releases), or build it yourself.
3. Place `MiraUnleashed.dll` in your `Among Us/BepInEx/plugins/` folder.
4. Launch the game. The mod's config file is `BepInEx/config/rewalo.mira.unleashed.cfg`.

-----------------------

# Compatibility

| Mira Unleashed | Town of Us: Mira | MiraAPI | Reactor |
| --- | --- | --- | --- |
| 2.0.0 | 1.7.3 | 0.5.0 | 2.5.0 |

The `experimental` branch of this repo tracks TOU-Mira's experimental branch and may target unreleased TOU-Mira builds.

-----------------------

# Building

Requires the .NET 10 SDK (see `global.json`).

1. Clone this repository.
2. `dotnet restore`
3. `dotnet build -c Release`

Optionally create `Directory.Build.local.props` (git-ignored) with an `<AmongUs>` property pointing at your Among Us install to auto-copy the built DLL to `BepInEx/plugins` after each build:

```xml
<Project>
  <PropertyGroup>
    <AmongUs>C:\Program Files (x86)\Steam\steamapps\common\Among Us</AmongUs>
  </PropertyGroup>
</Project>
```

-----------------------

# Branches

- `main` – releases only.
- `dev` – active development; the next version is built here.
- `experimental` – tracks TOU-Mira's experimental branch; rebased on `dev`.

-----------------------

# Documentation

Role and modifier details, options, and testing checklists live in [docs/](./docs) and are mirrored to the [GitHub Wiki](https://github.com/rewalo/TownOfUsMiraRolesExtension/wiki).

-----------------------

# Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md). Pull requests target `dev`.

-----------------------

# Credits

## Art Credits

- **Asterisken** - Art for Injector, Trapper, Clueless, Mirage, Charlatan, Scavenger, Forestaller, and Spiteful
- **Atony** - Art for Serial Killer, Lawyer, Witch, and Wraith
- **Stellar Roles** - Role Ideas, some art (buttons)

-----------------------

# License
This software is distributed under the GNU GPLv3 License. See [LICENSE](./LICENSE).

# Copyright
<p align="center">This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC.</p>
<p align="center">© Innersloth LLC.</p>
