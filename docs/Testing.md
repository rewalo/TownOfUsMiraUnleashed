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
- [ ] Jam works when the Hacker is not the host, and the charge count drops for everyone.
- [ ] Using all 3 jams leaves the button at 0 charges (it does not refill to max).
- [ ] Jam sound cue respects the "Jam Sound Cue" option (Hacker Only / Everyone).
- [ ] Kills grant jam charges up to the max.

## Injector

- [ ] Inject applies a random effect after the delay.
- [ ] The rolled effect is identical on the injector's client and a second client.
- [ ] The victim sees the effect in the modifier panel with a countdown/remaining-time label.
- [ ] Effect chances/duration options apply; positive effects toggle works.
- [ ] Nausea shakes camera only when the local setting is enabled.

## Witch

- [ ] Spell highlights the target in the next meeting and kills after N meetings.
- [ ] A spellbound Bait victim dies at the end of the meeting without triggering a forced report or other kill modifiers.
- [ ] Spell Resets Kill Cooldown only applies when the option is on; Spell Range multiplies cast range.
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
- [ ] Eating a body removes it on every client, including when the Scavenger is not the host.
- [ ] Spawns normally with other neutrals disabled.
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
- [ ] Role icons appear in the in-game role text (e.g. the `<sprite>` icon next to the role name).
