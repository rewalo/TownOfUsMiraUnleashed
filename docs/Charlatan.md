# Charlatan

![Charlatan](assets/RoleHeaders/Charlatan.png)

- **Team:** Impostor
- **Alignment:** Impostor Support
- **Colour:** Impostor red

## Overview

The Charlatan is an Impostor Support role that manipulates body reports. Deceive allows you to report bodies you've killed from any distance for a limited time after killing. Conceal reduces the report range of nearby bodies, but requires you to stay near the body for the duration.

## Abilities

| Ability | Description |
| --- | --- |
| Conceal | Reduce the report range of a nearby body. You must stay near the body for the channel duration to successfully conceal it. Cancelling the channel does not consume a use. |
| Deceive | Report the body you killed from any distance, regardless of range or line of sight. Available for a limited time after killing. |

## Options

| Option | Range | Default |
| --- | --- | --- |
| Deceive Base Duration | 0 – 60 s | 15 s |
| Deceive Duration Increase Per Kill | 0 – 15 s | 2.5 s |
| Conceal Uses | 0 – 10 | 2 |
| Conceal Charges Per Kill | 1 – 10 | 1 |
| Conceal Report Range | Extremely Short / Very Short / Short | Very Short |
| Conceal Channel Duration | 1 – 15 s | 2.5 s |
| Conceal Cooldown | 5 – 300 s | 30 s |

## Interactions & notes

- Conceal is a channel: moving out of range, dying, a meeting starting, or the body disappearing cancels it and refunds the use.
- A concealed body is made harder to spot (reduced alpha) and can only be reported from a shorter range.
- In Mira Unleashed the conceal/deceive state is fully RPC-driven, so every client agrees on which bodies are concealed and which reports are legitimate — the old mod could desync.
