# Gameplay Design

Two game modes share the same project: the implemented single-player
**Blackout Protocol** and the planned multiplayer **Sentinel Protocol**.

## Design classification (course framework)

| Element | SENTINEL PROTOCOL |
|---|---|
| **Goal** | Entertainment (primary) and social interaction (secondary) |
| **Genre** | Action → shooter → first-person shooter (FPS); cooperative shooter |
| **Platform** | Computer (PC) — Windows/Linux standalone build |
| **Player mode** | LAN-based multiplayer, 1–4 players; single-player option |
| **Time interval** | Real-time with time-limited rounds |
| **Game theory** | Non-zero-sum — players cooperate against NPCs |
| **Target audience** | Mid-core PC players, ~16–30, Philippines/SEA co-op groups |

## Player motivation and market

The design targets the motivations described in the course framework:

- **Social interaction** — the primary draw: four friends cooperating on one objective.
- **Competition** — score, round wins, and difficulty tiers provide a competitive edge without PvP.
- **Mastery** — best-of-five rounds, four difficulty levels, and the boss reward improvement.
- **Escapism** — the machine-uprising setting and the Resistance fantasy.
- **Knowledge** — learning maps, bot patterns, and boss phases.

**Audience persona.** A 16–30-year-old student or young professional in the
Philippines or Southeast Asia who owns a Windows/Linux PC, plays first-person
shooters casually with friends, has limited time per session, and prefers
cooperative play over competitive ranked modes. The game is not designed for
competitive esports, mobile players, or players who want a long progression
grind. MBTI and generational categories are treated as optional audience
context, not as design drivers.

---

# 1. Blackout Protocol — single-player (implemented)

A rules-driven mission mode layered on the FPS Microgame.

## Rules

| # | Rule | Enforced by |
|---|---|---|
| R1 | Destroy **every hostile bot** in the level | `ObjectiveKillEnemies` + `GameModeManager` |
| R2 | You have a **mission clock** (150 s on Foundry, 180 s on Observatory) | `GameModeManager.TimeLimit` |
| R3 | Each kill gives **+100 points** and **+5 seconds** | `ScorePerKill`, `TimeBonusPerKill` |
| R4 | **Sprint is disabled** — heavy combat gear | `DisableSprint` sets `SprintSpeedModifier = 1` |
| R5 | **Jetpack offline** for the whole mission | `DisableJetpack` disables the `Jetpack` component |
| R6 | **One life** — no respawn | Stock `PlayerCharacterController` death flow |
| R7 | Seconds left convert to **bonus points** (+10/s) | `ScorePerRemainingSecond` |

## Victory / defeat

- **Win:** all bots destroyed before the clock reaches `00:00`.
- **Lose:** the player dies **or** the clock reaches `00:00`.

## Flow

```
IntroMenu -> LEVEL 1 FOUNDRY -> LEVEL 2 OBSERVATORY -> WinScene
                    \______________ LoseScene (death or 00:00) ______________/
```

## HUD

- Level name, mission clock, bots remaining, score
- Objective + rules panel
- Level intro banner
- `MISSION COMPLETE` / `MISSION FAILED` result banner

---

# 2. Sentinel Protocol — LAN co-op (planned)

A Counter-Strike-style, round-based, PvE shooter for **1–4 players** on a LAN.

## Match structure

| Setting | Default |
|---|---|
| Players | 1–4 humans (host + up to 3) |
| Teams | All humans on one team vs the bot team |
| Mission format | 4 missions in sequence |
| Map match | Best-of-5 rounds (first to 3) |
| Round flow | 3 s freeze → 2:00 live → 5 s round end |
| Round win (humans) | Eliminate every bot |
| Round win (bots) | All humans dead, or time expires with bots alive |
| Respawn | Only at round start; dead players spectate |
| Between missions | Results screen → next mission |
| After mission 4 | Final results → menu |

## Bot difficulty

Per-mission `BotDifficultyProfile` (ScriptableObject):

- bot count
- health / damage multipliers
- accuracy and reaction time
- fire rate
- movement speed
- weapon tier
- archetype mix

| Mission | Bots | Archetypes | Mission role |
|---|---|---|---|
| FOUNDRY | 4 | Scout, Trooper | Tutorial |
| OBSERVATORY | 5 | Scout, Trooper, Sniper | Vertical combat |
| DEPOT | 6 | Trooper, Heavy, Sniper | Attrition |
| THE CORE | 6 + boss | Elite mix + SENTINEL | Finale |

The counts above are the **Veteran** baseline. The lobby also offers
**Recruit / Veteran / Elite / Nightmare**, scaling bot count, health, damage,
accuracy, reaction time, and fire rate. This gives players a choice of
difficulty instead of locking each mission to one setting.

## Balance

The design follows the five balance criteria from the course framework:

- **Consistent challenge** — each mission adds one new pressure while keeping
  the core round loop familiar.
- **Perceivably fair** — bots telegraph attacks, spawn away from the squad, and
  never appear inside walls; the squad always has cover and a retreat route.
- **No stagnation** — the HUD always shows the objective and the next step; a
  round never becomes unwinnable without a clear reason.
- **No trivial decisions** — limited ammo and health make target priority,
  positioning, and pickup timing matter.
- **Difficulty levels** — Recruit / Veteran / Elite / Nightmare.

## Interactivity modes

- **Player-to-game** — first-person movement, aiming, shooting, reloading,
  pickups, and HUD feedback.
- **Player-to-player** — shared objective, callouts, pings, and optional voice
  chat; dead players spectate and can coach.
- **Player-to-developer** — the public repository's issue tracker, playtest
  sessions, and patch notes.

## Information model

The cooperative mode uses **imperfect information**: enemy compass markers are
disabled, so players must rely on sight, sound, and communication. The HUD shows
only what the squad knows (objective, timer, alive count, score).

## Accessibility

Subtitles for briefings, remappable keys, mouse sensitivity and FOV settings,
colorblind-friendly status indicators, and selectable difficulty.

## Score / progression

- Round score: bots killed, headshots, assists (optional).
- Mission score carries into the results screen.
- Session score persists across the four missions (server-side).
- No buy economy, no bomb objective — **elimination only**.

## Why this design

- The existing game already has robot enemies, so the fiction and the bots match.
- Elimination rounds are the simplest robust round condition and the easiest to
  network correctly.
- Four missions give the story a clear destination without requiring a full
  Counter-Strike economy or PvP balance pass.
- A listen-server host plus three clients is comfortable on a LAN.
