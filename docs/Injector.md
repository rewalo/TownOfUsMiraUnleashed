# Injector

![Injector](assets/RoleHeaders/Injector.png)

- **Team:** Impostor
- **Alignment:** Impostor Support
- **Colour:** Impostor red

## Overview

The Injector is an Impostor Support role that can inject players with various negative effects. Effects are randomly selected and can include inverted controls, reduced vision, slowness, or confusion.

## Abilities

| Ability | Description |
| --- | --- |
| Inject | Inject a nearby player with a random negative effect. The effect activates after a configurable delay. |

## Options

| Option | Range | Default |
| --- | --- | --- |
| Inject Cooldown | 5 – 120 s | 25 s |
| Effect Activation Delay | 0 – 30 s | 5 s |
| Effect Duration Type | Set Time / All Round / All Game | Set Time |
| Effect Duration | 5 – 200 s | 45 s |
| Initial Uses | 0 – 15 | 4 |
| Uses Per Kill | 0 – 5 | 1 |
| Positive Effects Enabled | On / Off | On |
| Effect Type To Configure | per-effect selector | Inverted Controls |
| Effect Chance (per effect) | 0 – 100 % | see below |

Per-effect default chances: Inverted Controls 30 %, Low Vision 30 %, Slowness 30 %, Very Low Vision 50 %, Confused 40 %, No Vent 60 %, No Use 30 %, No Report 30 %, Nausea 50 %, Weakness 20 %, Speed Boost 10 %, Vision Boost 10 %, Regeneration 10 %.

## Interactions & notes

- The injected effect is rolled by the injector's client and applied identically on every client.
- The victim sees the active effect in the modifier panel with its icon, description, and remaining time ("Xs left" for timed effects, "Until the next meeting" for All Round, "For the rest of the game" for All Game).
- Nausea shakes the victim's camera; affected players can disable the shake in the local Mira Unleashed settings tab.
- Positive effects (Speed Boost, Vision Boost, Regeneration) can be disabled entirely with "Positive Effects Enabled".
