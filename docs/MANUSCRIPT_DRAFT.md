# SENTINEL PROTOCOL — Manuscript Draft

> Draft content for the **GameDev Prefinal Manuscript Template**.
> Replace the bracketed placeholders (`[Member]`, `[Group Name]`, `[Section]`,
> `[Date]`) and personalize the prose before submission.
>
> Mapping to the course handouts is documented in
> [`MANUSCRIPT_ALIGNMENT.md`](MANUSCRIPT_ALIGNMENT.md).

---

## TITLE PAGE

**SENTINEL PROTOCOL**

A Game Development Project
Presented to the IT Department of the
Information and Communication Technology Program of
STI College Cubao

In Partial Fulfillment
of the Requirements for the Course
Game Development

**Submitted by:**

[Member 1]
[Member 2]
[Member 3]
[Member 4]
[Member 5]
[Member 6]
[Member 7]

[DATE]

---

| | |
|---|---|
| **Title of game:** | SENTINEL PROTOCOL |
| **Group name:** | [Group Name] |
| **Members:** | [Member 1] / [Member 2] / [Member 3] / [Member 4] / [Member 5] / [Member 6] / [Member 7] |
| **Section:** | [Section] |
| **Date of Completion:** | [Date Submitted] |
| **Keywords:** | LAN-based multiplayer, co-op first-person shooter, bot AI, level design, game design, Unity |

---

## ABSTRACT

This project presents **SENTINEL PROTOCOL**, a four-player LAN-based cooperative
first-person shooter in which a Resistance squad fights through machine-held
facilities to destroy a rogue defense artificial intelligence. The study
addresses a common design problem in student and independent shooters: how to
deliver a clear objective, balanced challenge, and coherent progression without
the cost of a full competitive multiplayer production. The proponents applied
the six major game design components — gameplay, rules and mechanics, victory
and loss conditions, interactivity modes, challenges and obstacles, and game
structure — together with the three-act story structure and the level-design
framework of objectives, flow, duration, availability, relationship, and
progression.

The proposed game uses Counter-Strike-style elimination rounds across four
missions: Foundry, Observatory, Depot, and the Core. One to four players host or
join a local network session, clear escalating waves of machine enemies, and
finally destroy SENTINEL in a three-phase boss encounter. A playable prototype
was developed in Unity using the FPS Microgame prefab kits, and the cooperative
mode is designed around a server-authoritative networking model. The project
demonstrates that a focused, prefab-based cooperative shooter can be designed
around a complete gameplay loop and a coherent narrative experience within an
academic development timeline.

---

# CHAPTER I — INTRODUCTION

## Background of the Study

Video game design is the practice of turning an idea into a system of rules,
challenges, and experiences that players can understand and enjoy. Novak (2012)
describes game development as a process that connects gameplay, story,
characters, levels, and the player's motivations into one coherent product. A
game is not simply a collection of assets; it is a set of choices and
consequences that the player learns to navigate. For this reason, the design of
the rules, the victory and loss conditions, and the structure of the levels is
as important as the quality of the graphics or the story.

Games matter because they entertain, bring people together, and create shared
experiences. The social dimension is especially important in cooperative
shooters, where players coordinate, communicate, and rely on one another. In the
Philippine context, local-area-network play remains a familiar and accessible
form of multiplayer: friends can play together on the same network without
subscriptions, matchmaking services, or a stable internet connection. This makes
LAN-based multiplayer a practical target for a student project and a meaningful
design space of its own.

The concept of SENTINEL PROTOCOL was inspired by the machine-uprising strand of
1980s science fiction, in which humanity faces an artificial intelligence that
has turned its own defense systems against its creators. Rather than adapting a
specific existing story, the proponents used that genre as a starting point and
built an original setting: a defense network called SENTINEL misreads a sensor
anomaly as a first strike and launches in eleven seconds. What remains of
humanity survives underground, and a small Resistance squad attempts four
strikes to blind the machine, cut its supply lines, and reach its core.

The idea the game explores is teamwork against a superior intelligence. A single
player cannot outgun SENTINEL; a coordinated squad might outthink it. This
tension between human improvisation and machine optimization shapes the
gameplay, the mission structure, and the final encounter. Developing an original
game around this idea is worthwhile because it combines technical practice in
Unity with the design disciplines emphasized in the course: rules, conditions,
interactivity, challenges, levels, and story.

## Description of the Proposed Game

**Game Title:** SENTINEL PROTOCOL

**Genre:** Action — First-Person Shooter (FPS); cooperative shooter

**Target Audience:** Mid-core players aged approximately 16 to 30 who own a
Windows or Linux personal computer and enjoy science fiction and cooperative
shooters. The primary audience is a group of one to four friends in the
Philippines or Southeast Asia who want a short, focused LAN session. They value
teamwork, readable objectives, and a challenge that can be completed in one
sitting rather than a long competitive grind.

**Platform:** Computer (PC), specifically a standalone Windows or Linux build
played over a **LAN-based multiplayer** connection. The game does not require an
internet connection.

**Game Setting:** The year 2029, six months after a defense artificial
intelligence called SENTINEL turned on humanity. The game takes place in
machine-held industrial facilities: an automated foundry, an observatory uplink
complex, a subterranean supply depot, and SENTINEL's core beneath the city.

**Main Character or Player Role:** The player controls a Resistance operative
belonging to the Hammerhead squad. In cooperative play, each player takes one
callsign and shares the same objective. The character is not a super-soldier;
the squad survives through coordination, cover, and resource discipline.

**Game Concept:** One to four players clear four machine-held facilities in
round-based missions. Each mission is a best-of-five series of elimination
rounds: the human team must destroy every hostile bot before the round timer
expires. Between missions, the squad advances deeper into SENTINEL's
infrastructure, faces new machine archetypes and higher difficulty, and
eventually destroys the core in a three-phase boss encounter.

**Unique Feature:** SENTINEL PROTOCOL combines Counter-Strike-style round
structure with cooperative play against machine enemies, a four-mission
narrative arc, and a final artificial-intelligence boss — all playable over a
local network without internet services. The levels are built from the Unity FPS
Microgame prefab kits, which keeps production focused on gameplay and level
composition rather than asset creation.

## Statement of the Problem

This project addresses the design problem of creating a coherent, balanced, and
replayable cooperative shooter using a limited production scope.

**General Problem:** How can the proponents design and develop a LAN-based
cooperative first-person shooter that presents a clear objective, applies
consistent rules and conditions, and sustains player engagement across four
escalating missions?

**Specific Questions:**

1. What gameplay mechanics and rules should be established to define the
   actions that players may and may not perform during a mission?
2. How should the game determine success and failure at the round, mission, and
   campaign levels?
3. How should players interact with the game, with each other, and with
   non-player characters to overcome the challenges?
4. How should the game be organized — through objectives, flow, duration,
   availability, relationships, and progression — to provide a coherent player
   experience?
5. How can prefab-based level design and bot difficulty scaling be used to keep
   the challenge fair, readable, and engaging for one to four players?

## Objectives of the Study

### General Objective

To design and develop **SENTINEL PROTOCOL**, a LAN-based cooperative
first-person shooter for one to four players, and to demonstrate a complete
gameplay loop across four escalating missions ending in the destruction of the
SENTINEL core.

### Specific Objectives

1. To design the core gameplay loop, player role, and rules that define the
   actions available during a mission.
2. To establish clear victory and loss conditions at the round, mission, and
   campaign levels.
3. To design the interactivity modes, feedback systems, and challenges that
   support cooperative play.
4. To organize the game into four missions with coherent objectives, flow,
   duration, availability, relationships, and player progression.
5. To produce a playable prototype in Unity using the FPS Microgame prefab kits,
   and to document the design decisions for future development.

## Significance of the Study

**Students and Aspiring Game Developers.** The project provides a concrete
example of applying the six game design components to a real Unity project. It
shows how rules, conditions, levels, and networking decisions fit together, and
it documents the process of building on an existing template rather than
starting from nothing.

**Potential Players.** The game offers a short, accessible cooperative
experience that can be played by one to four friends on a local network without
subscriptions or internet access. Its round structure makes it easy to start,
stop, and retry.

**Game Designers.** The project demonstrates a practical model for prefab-based
level design, bot difficulty scaling, and round-based cooperative pacing. It
also shows how a small team can use an existing asset kit to focus effort on
gameplay and structure.

**Future Researchers.** The design and documentation can serve as a baseline for
studies on LAN cooperative play, bot behavior, difficulty balancing, and
student-developed multiplayer projects.

## Scope and Limitations

**Scope.** The project covers the design of a cooperative first-person shooter
and the development of a playable prototype in Unity. The complete design
includes four missions (Foundry, Observatory, Depot, and the Core), one to four
LAN players, four bot archetypes (Scout, Trooper, Heavy, and Sniper), a round
system with freeze time and elimination conditions, a mission and campaign
flow, a HUD, and a three-phase final boss. The current prototype implements the
single-player campaign on the first two missions; the cooperative mode and the
remaining missions are the target build for the final deliverable.

**Platform and Features.** The game targets Windows and Linux personal
computers and uses LAN-based multiplayer through Unity's networking stack. It
supports keyboard and mouse input, a host-and-join flow, and a single-player
option for testing and solo play.

**Limitations.** The project is limited to a local network; it does not include
online matchmaking, dedicated servers, or competitive player-versus-player
modes. It does not include a buy economy, ranked progression, or persistent
player accounts. Bot behavior is intentionally readable rather than
state-of-the-art; the enemies use patrol, detection, chase, and attack
behaviors rather than advanced tactical planning. The art and audio are drawn
from the Unity FPS Microgame template, so the visual identity is constrained by
the available prefab kits. Voice communication is provided through external
tools or simple in-game pings rather than a built-in voice system. The
deliverable is a prototype, not a commercially polished product.

**Outside the Scope.** Mobile and console versions, monetization, online
services, anti-cheat, user-generated content, and full voice acting are outside
the scope of the project.

---

# CHAPTER II — GAME DESIGN AND MECHANICS

## Gameplay

SENTINEL PROTOCOL is played in **real-time** from a first-person perspective.
The core loop is a round of elimination combat. A mission begins with a short
briefing, a freeze period, and then a live round. During the round, the squad
must destroy every hostile bot before the two-minute timer expires. When the
round ends, players respawn, the score is updated, and the next round begins.
The first team to win three rounds wins the mission, and the squad advances to
the next facility.

The player assumes the role of a Resistance operative. The operative can move,
crouch, jump, aim, fire, reload, and switch weapons. Movement is deliberately
grounded: sprinting and jetpacks are disabled to reinforce the game's
"heavy gear" fiction and to keep engagements readable. Players interact with the
environment through cover, chokepoints, elevation, pickups, and sealed sectors.
The main activity repeated throughout the game is the round: locate the bots,
use cover, manage ammunition, coordinate with teammates, and eliminate the bot
team before the clock runs out.

What makes the gameplay distinctive is the combination of a Counter-Strike-style
round structure with cooperative play against machine enemies and a narrative
campaign. The game is not a deathmatch and not a wave survival mode; it is a
sequence of short, self-contained tactical rounds that build toward a final
boss. The intended player experience is tension followed by relief: the squad
should feel outnumbered, then coordinated, then capable.

## Rules and Mechanics

The rules define the actions players may and may not perform. They are
communicated through the mission briefing, the HUD, and the first mission's
tutorial-like pacing.

| Rule or Mechanic | Description |
|---|---|
| **Movement** | The player moves with the standard first-person controls: forward, backward, strafe, mouse look, jump, and crouch. Sprinting and jetpacks are disabled; the operative moves at a deliberate pace. |
| **Interaction** | The player fires, aims, reloads, switches weapons, picks up health and ammunition, and communicates with teammates through pings or voice chat. Interaction with the environment is limited to cover, doors, and pickups; there are no puzzles or crafting systems. |
| **Resources** | Health and ammunition are limited and are replenished only by pickups placed in the level. Score is earned through kills and round wins. There is no currency and no buy menu. |
| **Restrictions** | Each player has one life per round; there is no respawn until the round ends. Friendly fire is disabled. Ammunition is finite, so sustained fire must be disciplined. The squad cannot leave the mission area. |
| **Penalties** | A dead player spectates until the round ends. Losing a round gives the bot team a point. Losing three rounds ends the mission, and the squad must retry it. |
| **Special Mechanics** | Freeze time before each round, a server-controlled bot director, per-difficulty bot profiles, shared mission score, sector lockdown after a cleared area, and the three-phase SENTINEL boss encounter. |

The rules are kept consistent across missions so that players can transfer
their knowledge from one facility to the next. Balance is maintained through
consistent challenge, fair encounter design, no unwinnable states, meaningful
resource decisions, and a choice of difficulty. The game avoids trivial
decisions by making ammunition, positioning, and target priority matter.

## Victory and Loss Conditions

### Victory Conditions

The game recognizes success at three levels:

- **Round:** the human team wins a round by eliminating every hostile bot before
  the two-minute timer expires.
- **Mission:** the human team wins a mission by winning three of five rounds
  (best-of-five).
- **Campaign:** the squad wins the campaign by completing all four missions and
  destroying the SENTINEL core in the final encounter.

Optional objectives include finishing a mission without any player deaths,
achieving a target score, or completing a mission under a par time. These are
recorded on the results screen but are not required to advance.

### Loss Conditions

- **Explicit loss conditions:** all human players are dead at the same time, or
  the round timer reaches zero while bots are still alive. At the mission level,
  losing three rounds is an explicit mission failure.
- **Implicit loss conditions:** attrition, ammunition starvation, poor
  positioning, and loss of communication can cause a round to be lost even
  before the last player dies. These are emergent consequences of the rules
  rather than scripted failures.

After a failed round, players respawn at the start of the next round. After a
failed mission, the squad retries the mission from its beginning. There is no
mid-round checkpoint, because rounds are short enough to replay. The campaign
does not delete progress on a failed mission; the squad simply attempts the
mission again.

## Interactivity Modes

| Interaction Category | Description of the Proposed Design |
|---|---|
| **Input Method** | Keyboard and mouse for movement, aiming, firing, reloading, and weapon switching. Gamepad support is optional. |
| **Player Mode** | LAN-based multiplayer for one to four players, with a host-and-join flow. A single-player mode is available for testing and solo play. |
| **Environmental Interaction** | Players use cover, elevation, chokepoints, doors, sector lockdowns, health pickups, and ammunition pickups. The environment shapes movement and sightlines but does not support construction or crafting. |
| **Character Interaction** | Players fight machine enemies (Scout, Trooper, Heavy, Sniper), cooperate with teammates through pings or voice chat, and receive briefings from Resistance command. SENTINEL communicates through facility announcements. |
| **Feedback System** | The HUD shows the objective, round timer, alive count, score, health, and ammunition. Audio, hit markers, damage vignettes, animations, and mission banners communicate state changes. |

The game uses three interactivity modes described in the course framework.
**Player-to-game** interaction is the core FPS loop: moving, aiming, shooting,
reloading, and reading the HUD. **Player-to-player** interaction is cooperative:
players share the objective, call out targets, and coordinate pushes; there is
no revive, and a dead player remains a spectator until the round ends.
**Player-to-developer** interaction is supported through the project's public
repository, issue tracker, and playtest feedback, where players can report bugs
and suggest changes.

In terms of game theory, the game is **non-zero-sum**: the players cooperate
against non-player characters rather than competing for a single winner. There
is no zero-sum player-versus-player conflict and no prisoner's dilemma
mechanic; the design deliberately avoids betrayal mechanics so that teamwork
remains the core experience.

## Challenges and Obstacles

| Challenge | Description | Skill Tested | Difficulty |
|---|---|---|---|
| **Bot assault** | Waves of machine enemies advance, take cover, and fire from different ranges. | Reaction time, target priority, positioning | Easy to Medium |
| **Resource scarcity** | Health and ammunition are limited and must be collected during combat. | Resource management, restraint, route planning | Medium |
| **Verticality and chokepoints** | Multi-floor spaces and narrow corridors create ambushes and force the squad to split or hold. | Spatial awareness, communication, map knowledge | Medium |
| **Final Challenge: SENTINEL core** | A three-phase boss: destroy shield nodes, clear defense turrets, then damage the core under pressure. | Coordination, pattern recognition, endurance | Hard |

The game includes both **explicit** and **implicit** challenges. Explicit
challenges are the intentional obstacles: bot assaults, the round timer, the
boss phases, and the mission's win condition. Implicit challenges emerge from
the systems: ammunition starvation, poor sightline control, a player isolated
from the squad, or a bot flank that forces an unplanned rotation.

Information is intentionally **imperfect**. Enemy compass markers are disabled
in cooperative play, so players must rely on sight, sound, and communication
rather than a complete view of the battlefield. Players acquire **intrinsic
knowledge** by learning the maps, bot behaviors, and boss patterns, and
**extrinsic knowledge** from their prior experience with first-person shooters.
The challenges test spatial awareness, resource management, and reaction time,
which are the skill categories associated with the action genre.

Difficulty increases gradually across the four missions: the enemy count,
health, damage, accuracy, reaction time, and fire rate all rise, and new bot
archetypes are introduced. The lobby offers Recruit, Veteran, Elite, and
Nightmare settings so that groups can choose a challenge that matches their
skill. The design follows the balance criteria of consistent challenge, fair
play, no stagnation, no trivial decisions, and player-selected difficulty.

## Game Structure

### Objectives

The main objective of the game is to complete all four missions and destroy
SENTINEL. Each mission has a primary objective:

1. **Foundry:** destroy the machine production line's defenders.
2. **Observatory:** clear the command uplink array.
3. **Depot:** seize the machine supply hub.
4. **The Core:** destroy SENTINEL.

Secondary objectives include winning a mission without player deaths, reaching a
target score, and completing a mission under a par time. These objectives
contribute to replay value and give experienced squads a reason to return.

Players always know what to accomplish because the mission briefing states the
objective, the HUD keeps it visible, and the level-intro banner repeats it at
the start of each mission.

### Game Flow

```
Main Menu
   │
   ├── Host Game ──► Lobby ──► Mission Briefing ──► Round Loop ──► Mission Result
   └── Join Game ──► Lobby ──► (same)                    │
                                                          │
   Round Loop:  Freeze (3 s) ──► Live (2:00) ──► Round End (5 s)
                                   │                 │
                                   └── next round ◄──┘

Mission Result
   │
   ├── win  ──► next mission (Foundry → Observatory → Depot → Core)
   └── lose ──► retry mission

The Core (mission 4)
   │
   └── SENTINEL boss (shield nodes → defense grid → core)
          │
          └── ending ──► credits ──► main menu
```

The player starts at the main menu, chooses to host or join a LAN session, and
waits in a lobby until the squad is ready. The host starts the mission, the
briefing appears, and the round loop begins. Winning a mission advances the
squad to the next facility; losing a mission returns the squad to its start.
The final mission replaces the normal round loop with the boss encounter. The
campaign ends with a short ending sequence and the credits.

### Game Duration

| Segment | Estimated duration |
|---|---|
| Freeze time | 3 seconds per round |
| Live round | up to 2 minutes |
| Round end | 5 seconds |
| One round (total) | about 2–3 minutes |
| One mission (best-of-five) | about 15–25 minutes |
| Full campaign (four missions) | about 60–100 minutes |
| Typical session | one mission (15–25 minutes) |

The mission length follows the course guidance that computer-game levels should
be short enough for a player to complete at least one level in a single session.
A squad can finish one mission in an evening and return for the next, and the
full campaign fits into a single long session. The game does not require a save
system because the missions are short and the campaign can be replayed from the
menu.

### Game Availability

SENTINEL PROTOCOL is available as a standalone Windows or Linux build for
personal computers. It requires a mid-range processor, a DirectX 11 or Vulkan
capable graphics card, 8 GB of RAM, and a local network connection for
multiplayer. It does not require an internet connection, an account, or a
subscription.

Accessibility features include subtitles for mission briefings, remappable
keys, mouse sensitivity and field-of-view settings, colorblind-friendly status
indicators, and a difficulty selection. These features are intended to make the
game approachable for players with different needs and skill levels.

### Relationships Within the Game

- **Player and environment:** the player relates to the environment as a
  tactical space. Cover, sightlines, elevation, and chokepoints determine
  survival; the environment is not decorative.
- **Player and characters:** the squad relates to SENTINEL as an unseen
  antagonist and to Resistance command as a mentor. Teammates are allies whose
  actions directly affect the player's chances.
- **Objectives and challenges:** each mission objective determines the shape of
  its encounters. The Foundry teaches the round loop; the Observatory adds
  verticality; the Depot adds chokepoints; the Core adds the boss.
- **Resources and mechanics:** limited health and ammunition make aggression
  costly and encourage coordination. The shared score rewards teamwork rather
  than individual kills.
- **All elements together:** the story gives the missions a destination, the
  levels give the story a physical space, the rules give the space tension, and
  the progression gives the tension a shape.

### Player Progression

| Stage | Main Objective | New Challenge or Feature | Progression Requirement |
|---|---|---|---|
| **Stage 1 — Foundry** | Destroy the defenders of the production line | Introduces rounds, the timer, Scouts and Troopers | Win three rounds |
| **Stage 2 — Observatory** | Clear the command uplink array | Vertical combat, snipers, first Enforcer units | Win three rounds |
| **Stage 3 — Depot** | Seize the supply hub | Chokepoints, Heavies, resource pressure | Win three rounds |
| **Final Stage — The Core** | Destroy SENTINEL | Three-phase boss, elite guards, endurance | Complete the boss |

Progression is mission-based rather than character-based. Players do not level
up or unlock permanent abilities; instead, each mission introduces a new
environment, a new bot composition, and a higher difficulty. The reward for
progress is the next mission, a higher mission score, and the story's
resolution. This keeps the game easy to pick up for new players and easy to
balance for a small development team.

---

# CHAPTER III — SUMMARY, CONCLUSION, AND RECOMMENDATIONS

## Summary

This project designed **SENTINEL PROTOCOL**, a LAN-based cooperative
first-person shooter for one to four players. The game is set in 2029, after a
defense artificial intelligence called SENTINEL turns on humanity, and follows a
Resistance squad through four escalating missions: Foundry, Observatory, Depot,
and the Core. The intended audience is mid-core PC players in the Philippines
and Southeast Asia who want a short, social, science-fiction shooter that can be
played with friends on a local network.

The design applies the six major game design components. **Gameplay** is a
real-time, round-based elimination loop with a clear role and a short core
activity. **Rules and mechanics** define movement, interaction, resources,
restrictions, penalties, and special systems. **Victory and loss conditions**
operate at the round, mission, and campaign levels, with both explicit and
implicit failure states. **Interactivity modes** cover player-to-game,
player-to-player, and player-to-developer interaction, and the game is
non-zero-sum because players cooperate against non-player characters.
**Challenges and obstacles** combine explicit and implicit challenges with
imperfect information and a gradual difficulty curve. **Game structure**
organizes the experience through objectives, flow, duration, availability,
relationships, and player progression.

The game's distinctive features are its cooperative Counter-Strike-style rounds
against machine enemies, its four-mission narrative arc, its final
artificial-intelligence boss, and its use of the Unity FPS Microgame prefab kits
to keep production focused on gameplay and level design. The overall approach
was to design the systems first, prototype them in Unity, and document the
decisions so that the project can be extended.

## Conclusion

The proposed game meets the objectives of the project by establishing a
complete gameplay loop, a consistent rule set, clear success and failure
conditions, three interactivity modes, a structured challenge curve, and a
coherent four-mission progression. The mechanics support the intended player
experience: the round timer creates tension, limited resources force
cooperation, and the boss encounter tests the coordination the squad has built
across the campaign.

The rules, challenges, and progression work together because each mission
introduces one new pressure while keeping the core loop familiar. The Foundry
teaches the rules, the Observatory adds verticality, the Depot adds resource
pressure, and the Core combines everything in the final encounter. This
structure gives the player a clear sense of growth without requiring a complex
RPG progression system.

The concept is original in its combination of elements and feasible within an
academic timeline. It uses an existing prefab kit for art and level pieces,
focuses the networking scope on a local network, and keeps the bot behavior
readable rather than attempting a full tactical AI. Through the design process,
the proponents established that a small cooperative shooter can deliver a
complete, understandable, and replayable experience when the rules, conditions,
levels, and story are designed together.

## Recommendations

1. **Playtest and tune the difficulty curve.** The four difficulty settings
   should be tested with groups of different skill levels so that the jump from
   Recruit to Nightmare feels fair rather than punishing.
2. **Improve the bot behavior.** Future work should add cover selection,
   suppression, flanking, and objective awareness so that the machine enemies
   feel more deliberate without becoming unfair.
3. **Expand the accessibility options.** Add a subtitle size setting, a
   colorblind mode with alternative indicators, and a toggle for screen shake.
4. **Add online play through Relay and Lobby services.** LAN play is the
   project's scope, but online matchmaking would make the game accessible to
   players who cannot meet on the same network.
5. **Build more missions and map variants.** The level kit supports additional
   layouts; more missions would extend the campaign and increase replay value.
6. **Add a dedicated server option.** A dedicated server would remove the
   listen-server host advantage and support larger groups.
7. **Collect player feedback systematically.** The public repository's issue
   tracker and playtest sessions can provide the player-to-developer feedback
   loop described in the design.
8. **Add a proper save and statistics system.** Mission results, best times,
   and accuracy statistics would give players long-term goals.

---

# REFERENCES

Buttfield-Addison, P., Manning, J., & Nugent, T. (2019). *Unity game development
cookbook*. O'Reilly Media.

Novak, J. (2012). *Game development essentials: An introduction* (3rd ed.).
Delmar, Cengage Learning.

Unity Technologies. (n.d.). *Netcode for GameObjects documentation*. Retrieved
from https://docs-multiplayer.unity3d.com/

Unity Technologies. (n.d.). *Unity 6 documentation*. Retrieved from
https://docs.unity3d.com/
