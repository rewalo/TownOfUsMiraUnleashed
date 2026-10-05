# Testing checklist

Manual smoke checks before a release. Requires at least two clients where noted.

## Forestaller

- [ ] Role spawns and shows yellow name/description.
- [ ] After finishing all tasks, sabotage calls are blocked while alive.
- [ ] Reveal happens instantly or at next meeting per the Reveal Timing option.

## Mirage

- [ ] Prime then place a decoy; it appears as the chosen/random target.
- [ ] Interacting player gets "You interacted with a decoy!", Mirage gets "Your decoy was triggered!" and an arrow.
- [ ] Decoy type = Mirage: Mirage cannot be guessed.

## Trapper

- [ ] Shows in Crewmate Support alignment.
- [ ] Trapped vent immobilizes the venter; Trapper notified with the room.
- [ ] TOU's Trapper displays as "Revealer" in name/intro/tab/wiki.

## Charlatan

- [ ] Deceive allows reporting your own kill from any distance within the window.
- [ ] Conceal channels, then reduces the body report range on every client.
- [ ] Cancelling a channel refunds the use.

## Hacker

- [ ] Download locks equipment type and grants battery.
- [ ] Jam blocks info systems but not emergency meetings.
- [ ] Kills grant jam charges up to the max.

## Injector

- [ ] Inject applies a random effect after the delay.
- [ ] Effect chances/duration options apply; positive effects toggle works.
- [ ] Nausea shakes camera only when the local setting is enabled.

## Witch

- [ ] Spell highlights the target in the next meeting and kills after N meetings.
- [ ] Witch dying/exiled/guessed saves all spellbound players.

## Wraith

- [ ] Dash increases speed by 75% for the duration.
- [ ] Lantern is only visible to the Wraith; reactivation teleports back and grants brief invisibility.
- [ ] Expired lanterns break and leave visible evidence.

## Lawyer

- [ ] Client assigned, § markers show, client intro/tab text names the lawyer.
- [ ] Objection clears votes and blocks re-voting the same target (when enabled).
- [ ] Client death kills or converts the Lawyer per options.
- [ ] Steal-win / duo / parity win conditions trigger correctly.
- [ ] Private lawyer/client chat appears in meetings when enabled.

## Serial Killer

- [ ] Always shown in Serial Killer blue (#3A66C0), even with "Use Crewmate Team Color".
- [ ] Vent kill works per the target option and removes venting afterwards.
- [ ] Maniac timer kills the Serial Killer on expiry and resets on kill.
- [ ] Report button disabled when reports are off.

## Scavenger

- [ ] Always shown in Scavenger brown (#8B4513).
- [ ] Eating bodies counts toward the win; scavenge arrows work when enabled.
- [ ] Spawns normally with other neutrals disabled.
- [ ] With "Cannot Spawn With Janitor" on and Janitor enabled, Scavenger never rolls.
- [ ] Converts to the configured role when the win becomes impossible.

## Modifiers

- [ ] Clueless removes task list/markers/map locations.
- [ ] Spiteful applies the configured punishment to everyone who voted the player out.

## Regression checks for 2.0.0

- [ ] Role tags in-game show `TOUMU`, not `TOUE`.
- [ ] Trapper shown as Crew Support alignment.
- [ ] Serial Killer / Scavenger keep their custom colours with "Use Crewmate Team Color" on.
- [ ] Scavenger spawns with all other neutrals disabled.
- [ ] Charlatan conceal/deceive state verified from a second client (body alpha, reduced report range, deceive report window).
