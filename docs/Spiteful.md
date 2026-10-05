# Spiteful

![Spiteful](assets/ModifierHeaders/Spiteful.png)

- **Type:** Universal modifier

## Overview

When you are voted out, everyone who voted for you receives a negative effect (lower vision, slowness, or increased cooldowns) for a configured number of rounds or the rest of the game.

## Options

| Option | Range | Default |
| --- | --- | --- |
| Spiteful Amount | 0 – 15 | 0 |
| Spiteful Chance | 0 – 100 % | 50 % |
| Spiteful Impact | 15 – 75 % | 25 % |
| Punishment Effect | Lower Vision / Slowness / Increased Cooldowns | Increased Cooldowns |
| Punishment Duration | Next Rounds / Rest of Game | Rest of Game |
| Number of Rounds | 1 – 5 | 1 |

## Interactions & notes

- Voters are tracked when the Spiteful player is exiled; each voter gets one effect modifier and a notification naming the punishment.
- Lower Vision reduces the light radius, Slowness scales the visual speed, and Increased Cooldowns multiplies all ability button cooldowns by the configured impact.
- Effects never stack — a player already affected is not given a second modifier.
