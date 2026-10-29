# Gameplay Design

Two game modes share the same project: the implemented single-player
**Blackout Protocol** and the planned multiplayer **Sentinel Protocol**.

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

| Mission | Bots | Archetypes | Difficulty |
|---|---|---|---|
| FOUNDRY | 4 | Scout, Trooper | Recruit |
| OBSERVATORY | 5 | Scout, Trooper, Sniper | Veteran |
| DEPOT | 6 | Trooper, Heavy, Sniper | Elite |
| THE CORE | 6 + boss | Elite mix + SENTINEL | Nightmare |

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
