# Scavenger

![Scavenger](assets/RoleHeaders/Scavenger.png)

- **Team:** Neutral
- **Alignment:** Neutral Evil
- **Colour:** #8B4513 — always, even with "Use Crewmate Team Color" enabled

## Overview

The Scavenger is a Neutral Evil role whose goal is to eat a set amount of dead bodies. Optionally, the Scavenger can use Scavenge to get arrows pointing to corpses. If the Scavenger's win condition can no longer be met, they become the configured role.

## Abilities

| Ability | Description |
| --- | --- |
| Eat | Channel on a dead body to consume it. Eaten bodies count toward the win condition. |
| Scavenge | Get arrows pointing to corpses for a duration (when enabled). |

## Options

| Option | Range | Default |
| --- | --- | --- |
| Eat Cooldown | 5 – 120 s | 17.5 s |
| Eat Duration | 0.5 – 10 s | 1.5 s |
| Number Of Bodies To Win | 1 – 15 | 3 |
| Scavenger Can Vent | On / Off | Off |
| Cannot Spawn With Janitor | On / Off | Off |
| Scavenge Enabled | On / Off | Off |
| Scavenge Cooldown | 5 – 120 s | 30 s |
| Scavenge Duration | 5 – 60 s | 5 s |
| On Lose, Scavenger Becomes | Crewmate / Amnesiac / Survivor / Mercenary / Jester | Crewmate |

## Interactions & notes

- **Cannot Spawn With Janitor** — when enabled, Scavenger and Janitor cannot roll in the same game (they compete for the same bodies). If both are enabled, a 100% role beats a lower-chance role; otherwise whichever passes its spawn roll wins, with ties decided randomly.
- In 2.0.0 the Scavenger is a normal role roll; the old "spawn only when first roles die" behavior was removed because it prevented Scavenger from spawning at all.
