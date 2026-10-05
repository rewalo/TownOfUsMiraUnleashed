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

[![Build](https://github.com/rewalo/TownOfUsMiraUnleashed/actions/workflows/build.yml/badge.svg)](https://github.com/rewalo/TownOfUsMiraUnleashed/actions/workflows/build.yml)
[![Latest Release](https://img.shields.io/github/v/release/rewalo/TownOfUsMiraUnleashed)](https://github.com/rewalo/TownOfUsMiraUnleashed/releases/latest)
[![License: GPL-3.0](https://img.shields.io/badge/license-GPL--3.0-blue.svg)](./LICENSE)
[![TOU-Mira](https://img.shields.io/badge/requires-TOU--Mira-red.svg)](#compatibility)

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
  <img width="100%" src="./docs/assets/Groups/CrewSupport.png" />
  <a href="./docs/Forestaller.md"><img width="30%" src="./docs/assets/RoleHeaders/Forestaller.png" /></a>
  <a href="./docs/Mirage.md"><img width="30%" src="./docs/assets/RoleHeaders/Mirage.png" /></a>
  <a href="./docs/Trapper.md"><img width="30%" src="./docs/assets/RoleHeaders/Trapper.png" /></a>
  <img width="100%" src="./docs/assets/Groups/ImpSupport.png" />
  <a href="./docs/Charlatan.md"><img width="30%" src="./docs/assets/RoleHeaders/Charlatan.png" /></a>
  <a href="./docs/Hacker.md"><img width="30%" src="./docs/assets/RoleHeaders/Hacker.png" /></a>
  <a href="./docs/Injector.md"><img width="30%" src="./docs/assets/RoleHeaders/Injector.png" /></a>
  <img width="100%" src="./docs/assets/Groups/ImpKilling.png" />
  <a href="./docs/Witch.md"><img width="30%" src="./docs/assets/RoleHeaders/Witch.png" /></a>
  <img width="100%" src="./docs/assets/Groups/ImpPower.png" />
  <a href="./docs/Wraith.md"><img width="30%" src="./docs/assets/RoleHeaders/Wraith.png" /></a>
  <img width="100%" src="./docs/assets/Groups/NeutBenign.png" />
  <a href="./docs/Lawyer.md"><img width="30%" src="./docs/assets/RoleHeaders/Lawyer.png" /></a>
  <img width="100%" src="./docs/assets/Groups/NeutKilling.png" />
  <a href="./docs/Serial-Killer.md"><img width="30%" src="./docs/assets/RoleHeaders/Serial Killer.png" /></a>
  <img width="100%" src="./docs/assets/Groups/NeutEvil.png" />
  <a href="./docs/Scavenger.md"><img width="30%" src="./docs/assets/RoleHeaders/Scavenger.png" /></a>
  <img width="100%" src="./docs/assets/Groups/UniMods.png" />
  <a href="./docs/Clueless.md"><img width="30%" src="./docs/assets/ModifierHeaders/Clueless.png" /></a>
  <a href="./docs/Spiteful.md"><img width="30%" src="./docs/assets/ModifierHeaders/Spiteful.png" /></a>
</p>

Click a role for its abilities, options and interactions, or browse the [wiki](https://github.com/rewalo/TownOfUsMiraUnleashed/wiki).

-----------------------

# Installation

1. Install [Town of Us: Mira](https://github.com/AU-Avengers/TOU-Mira) (it bundles MiraAPI, Reactor and BepInEx). Check the [Compatibility](#compatibility) table for the TOU-Mira version that matches the Mira Unleashed release you are installing.
2. Download the latest `MiraUnleashed.dll` from [Releases](https://github.com/rewalo/TownOfUsMiraUnleashed/releases), or build it yourself.
3. Place `MiraUnleashed.dll` in your `Among Us/BepInEx/plugins/` folder.
4. Launch the game. The mod's config file is `BepInEx/config/rewalo.mira.unleashed.cfg`.

-----------------------

# Compatibility

| Mira Unleashed | Town of Us: Mira | MiraAPI | Reactor |
| --- | --- | --- | --- |
| 2.1.0 | 1.7.3 | 0.5.0 | 2.5.0 |
| 2.0.0 | 1.7.3 | 0.5.0 | 2.5.0 |

The table is updated with every release; the latest row applies to the newest release.

The `experimental` branch of this repo tracks TOU-Mira's experimental branch and may target unreleased TOU-Mira builds.

Mira Unleashed also integrates with [Perfect Comms](https://github.com/artriy/Perfect-Comms) for optional proximity voice chat features — see the [Voice Chat](https://github.com/rewalo/TownOfUsMiraUnleashed/wiki/Voice-Chat) wiki page.

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

Role and modifier details, options, and testing checklists live in [docs/](./docs) and are mirrored to the [GitHub Wiki](https://github.com/rewalo/TownOfUsMiraUnleashed/wiki).

-----------------------

# Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md). Pull requests target `dev`. Please read the [AI usage policy](./CONTRIBUTING.md#ai-usage-policy) before contributing.

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
