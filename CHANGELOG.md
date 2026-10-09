# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

For releases before 2.0.0, see the [old repository's release history](https://github.com/rewalo/TownOfUsMiraUnleashed/releases).

## [2.1.0] - Unreleased

### Added

- Clueless and Spiteful now have TMP sprite icons like roles do.
- Optional Perfect Comms voice chat integration — when Perfect Comms is installed, four host toggles appear in its host panel under the "Mira Unleashed" tab (all on by default):
  - **Wraith: Mute While Invisible** — an invisible Wraith cannot transmit voice until the Lantern invisibility ends.
  - **Hacker: Jam Disrupts Voice** — everyone's voice is muted during tasks while a Jam is active, like a comms sabotage.
  - **Injector: Muffle Injected Hearing** — muffles incoming voice during tasks for players injected with Low Vision, Very Low Vision, Confusion or Nausea.
  - **Team Radio - Lawyer** — a private managed Team Radio channel between a Lawyer and their client when Team Radio is on.

### Changed

- `Clueless Censor Type` is now a per-client local setting (Mira Unleashed tab) instead of a host option, and `Remove` now hides the whole task panel instead of only the task text.
- Wiki home page redesigned as an icon table.
- `Cannot Spawn With Janitor` now uses TOU-Mira's exclusive-role system: Scavenger and Janitor are resolved against each other per game instead of Scavenger being blocked whenever Janitor is enabled.

### Fixed

- The `Enable Nausea Camera Shake` local setting showed its raw key instead of its label.
- Dev builds are now shown in red in the Reactor mod list like TOU-Mira.

## [2.0.0] - 2026-10-05

### Added

- Rewritten as **Mira Unleashed** for Town of Us: Mira 1.7.3 / MiraAPI 0.5.0 on .NET 6.
- New BepInEx GUID `rewalo.mira.unleashed` and mod abbreviation `TOUMU` (was `TOUE`).
- All strings now use MiraAPI locale IDs under the `MiraUnleashed.*` key space.
- New Scavenger option `Cannot Spawn With Janitor` (default off).
- Trapper moved to the `Crewmate Support` alignment.
- TOU-Mira's Trapper is displayed as "Revealer" while this mod is loaded.
- Lawyer private chat option moved into the Lawyer options group.
- Charlatan conceal/deceive state is now fully networked via RPCs so all clients agree on body transparency, report ranges, and deceive windows.
- Hacker "Jam Sound Cue" option controlling who hears the jam sound (Hacker Only / Everyone).
- Witch options "Spell Range" and "Spell Resets Kill Cooldown".
- Injector effects now show in the victim's modifier panel with icon, description and remaining duration, and the injection notification names the effect and its duration.
- Role icons shown next to the role name in the in-game role text.
- GitHub Actions CI (build, release, wiki sync).

### Changed

- Serial Killer and Scavenger now always use their custom role colors instead of turning impostor red.
- Buttons use MiraAPI's `CustomButtonSingleton<T>` instead of hand-rolled static instances.
- Harmony patches are registered via a single `Harmony.PatchAll()` call; the old safe-reflection patch loops were removed.

### Fixed

- Scavenger could not spawn in normal games (the old `ISpawnChange`/`NoSpawn` logic made TOU treat it like Traitor).
- Scavenger eating a body not removing it on non-host clients.
- Charlatan conceal/deceive desync between clients.
- Lawyer death strings using an unfilled `<player>` placeholder.
- Hacker jam failing when the Hacker was not the host, and jam charges refilling after reaching 0.
- Witch spell deaths triggering Bait/Frosty-style kill modifiers; spellbound players now die during vote processing with the nameplate animation.
- Injector rolling a different effect on each client, and All Round / All Game effects expiring instantly.

### Removed

- `GeneralOptions` group (its single option moved to Lawyer options).
- Legacy `TouExtension*`/`TouLocale`/`ExtensionRole` architecture and naming.
