# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

For releases before 2.0.0, see the [old repository's release history](https://github.com/rewalo/TownOfUsMiraRolesExtension/releases).

## [2.0.0] - Unreleased

### Added

- Rewritten as **Mira Unleashed** for Town of Us: Mira 1.7.3 / MiraAPI 0.5.0 on .NET 6.
- New BepInEx GUID `rewalo.mira.unleashed` and mod abbreviation `MU` (was `TOUE`).
- All strings now use MiraAPI locale IDs under the `MiraUnleashed.*` key space.
- New Scavenger option `Cannot Spawn With Janitor` (default off).
- Trapper moved to the `Crewmate Support` alignment.
- TOU-Mira's Trapper is displayed as "Revealer" while this mod is loaded.
- Lawyer private chat option moved into the Lawyer options group.
- Charlatan conceal/deceive state is now fully networked via RPCs so all clients agree on body transparency, report ranges, and deceive windows.
- GitHub Actions CI (build, release, wiki sync).

### Changed

- Serial Killer and Scavenger now always use their custom role colors instead of turning impostor red.
- Buttons use MiraAPI's `CustomButtonSingleton<T>` instead of hand-rolled static instances.
- Harmony patches are registered via a single `Harmony.PatchAll()` call; the old safe-reflection patch loops were removed.

### Fixed

- Scavenger could not spawn in normal games (the old `ISpawnChange`/`NoSpawn` logic made TOU treat it like Traitor).
- Charlatan conceal/deceive desync between clients.
- Lawyer death strings using an unfilled `<player>` placeholder.

### Removed

- `GeneralOptions` group (its single option moved to Lawyer options).
- Legacy `TouExtension*`/`TouLocale`/`ExtensionRole` architecture and naming.
