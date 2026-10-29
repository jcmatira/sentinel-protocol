# Multiplayer Architecture — 4-Player LAN Co-op

> Target: **Sentinel Protocol** — 1–4 players, host/client over LAN, PvE
> elimination rounds across four missions.

---

## Phase 0 status (implemented)

Phase 0 is in the repository:

| Item | Where |
|---|---|
| Packages | `Packages/manifest.json` — Netcode for GameObjects **2.13.3**, Transport **2.7.4**, Multiplayer Play Mode **1.6.3**, Multiplayer Tools **2.2.12** |
| Session manager | `Assets/FPS/Scripts/Multiplayer/NetworkSession.cs` |
| Host / Join UI | `Assets/FPS/Scripts/Multiplayer/HostJoinUI.cs` (auto-added to IntroMenu) |
| Connected-player limit | 4 (host + 3 clients), enforced in connection approval |
| Network player prefab tool | `Assets/FPS/Scripts/Editor/MultiplayerSetup.cs` |
| Player ownership script | `Assets/FPS/Scripts/Multiplayer/NetworkPlayer.cs` |

### How to test Phase 0

1. Open Unity and let the packages import.
2. Run **Tools ▸ Multiplayer ▸ Setup Network Player Prefab** once.
3. Open `Assets/FPS/Scenes/IntroMenu.unity` and press **Play**.
4. In one instance (or a standalone build), click **HOST**.
5. In another instance, enter the host IP and click **JOIN**.
6. Both should show the same player count, up to `4`.

The panel is additive — the normal **START** button still launches the
single-player campaign.

### Phase 0 limitations

- The mission scene is **not** loaded through NGO yet. Clicking START after
  hosting only changes the host's local scene. Networked scene loading and
  player spawning are Phase 1/4 work.
- The networked player prefab is prepared but not yet spawned, positioned, or
  given per-player HUDs.

---

## 1. Goals

- 1 host + up to 3 clients on the same network
- Server-authoritative gameplay (bots, damage, rounds, scene flow)
- Counter-Strike-style rounds, no bomb objective
- Four missions with per-mission bot difficulty
- A listen server is acceptable (the host also plays)

## 2. Stack

| Package | Purpose | Required |
|---|---|---|
| `com.unity.netcode.gameobjects` | Networking core | yes |
| `com.unity.transport` | UDP transport (bundled with NGO) | yes |
| `com.unity.multiplayer.playmode` | Test multiple players in the editor | testing |
| `com.unity.multiplayer.tools` | Network profiler / stats | optional |

Relay, Lobby and Authentication are **not** required for LAN.

## 3. Topology

```
HOST (listen server)                      CLIENT
┌────────────────────────────┐           ┌────────────────────────────┐
│ NetworkManager             │◄── UDP ──►│ NetworkManager             │
│ Unity Transport :7777      │           │ Unity Transport            │
│                            │           │                            │
│ Server authority:          │           │ Owns only its local player │
│  • bots / AI               │           │  • movement, camera, input │
│  • damage, health, pickups │           │  • HUD                     │
│  • round + match state     │           │  • spectator camera        │
│  • scene loading           │           │                            │
│ + local player             │           │ Remote players are proxies │
└────────────────────────────┘           └────────────────────────────┘
```

## 4. Authority model

| System | Authority | Replication |
|---|---|---|
| Player movement | Client owner | `NetworkTransform` (owner authority) |
| Camera / input | Client owner | none |
| Health / damage | Server | `NetworkVariable` |
| Weapons / ammo | Server | RPC for fire requests |
| Projectiles | Server | `NetworkObject` + interpolation |
| Enemies / AI | Server | `NetworkTransform` + `NetworkAnimator` |
| Pickups | Server | despawn replication |
| Round / match / score | Server | `NetworkVariable` + `ClientRpc` |
| Scene loads | Server | `NetworkManager.SceneManager` |

Trade-off: owner-authoritative movement is simpler and fine for a LAN school
project; it is not cheat-proof. Server-authoritative movement with prediction is
out of scope.

## 5. New systems

| Script | Responsibility |
|---|---|
| `NetworkBootstrap` | Host / join, connection approval, max players |
| `MatchFlowManager` | 4-mission rotation, intermission, results, session score |
| `RoundManager` | State machine, timers, round/match score, win checks, respawns |
| `BotDirector` | Reads the mission profile, spawns bots, tracks alive count |
| `BotDifficultyProfile` | ScriptableObject: counts, stats, archetype mix |
| `SpawnPoint` / `SpawnManager` | Human and bot spawn selection |
| `SpectatorController` | Free-cam / follow teammate after death |
| `RoundHUD` | Round timer, alive counts, score, mission name |
| `Scoreboard` | Optional post-round summary |

### Round state machine

```
WAITING ──► FREEZE (3 s) ──► LIVE (2:00) ──► ROUND_END (5 s)
                ▲                                  │
                └──────────── next round ──────────┘
                                   │
                             match over → next mission
```

Win checks (server only):
- bot team alive count == 0 → humans win the round
- human team alive count == 0 → bots win the round
- round timer == 0 with bots alive → bots win the round

## 6. Existing scripts to change

| File | Change |
|---|---|
| `PlayerInputHandler.cs` | Enable only on the locally owned player |
| `PlayerCharacterController.cs` | Owner-only control; network transform |
| `Jetpack.cs` | Owner-only |
| `Health.cs`, `Damageable.cs` | Server-authoritative health + death |
| `PlayerWeaponsManager.cs`, `WeaponController.cs` | Server-validated fire + ammo |
| `ProjectileStandard.cs` | Server-spawned networked projectiles |
| `EnemyController.cs`, `EnemyManager.cs` | Server-only; replicated transforms |
| `EnemyMobile.cs`, `EnemyTurret.cs`, `NavigationModule.cs`, `DetectionModule.cs` | Server-only |
| `Objective*.cs`, `ObjectiveManager.cs` | Server-only; HUD via RPC |
| `GameFlowManager.cs` | Networked scene loading |
| `EventManager.cs` / `Events.cs` | Local UI events only; no gameplay authority |
| `GameModeManager.cs` | Becomes server-authoritative or is replaced by `RoundManager` |
| `GameModeHUD.cs` | Per-client HUD from synced state |
| `GameModeBootstrap.cs` | Disabled in multiplayer |
| `InGameMenuManager.cs` | No `Time.timeScale` pause; disconnect instead |
| `Pickup*.cs` | Server consumes and despawns |
| `ActorsManager.cs` | Server-only registry |

## 7. Scene & prefab setup

- `NetworkManager` prefab/object with Unity Transport and `MaxConnections = 8`.
- Player prefab: `NetworkObject`, `NetworkTransform`, network health/weapons.
- Both current scenes + the two new mission scenes in Build Settings.
- Register mission scenes with NGO scene management.
- Add human/bot spawn points to every mission scene.
- Bake NavMesh for every mission scene.

## 8. Testing

1. **Editor:** install Multiplayer Play Mode, add virtual players, host in one,
   join `127.0.0.1` in the other.
2. **LAN:** build a Linux/Windows standalone, run it on 2–4 machines.
   Host IP: `hostname -I` (Linux) or `ipconfig` (Windows).
3. Watch NGO logs for scene-sync, spawn and ownership errors.

## 9. Known traps in this codebase

- `FindAnyObjectByType` picks up remote players and enemies — guard with
  `IsOwner` / `IsServer`.
- Static `EventManager` leaks UI events across players.
- Two `AudioListener` and `EventSystem` instances appear when players spawn.
- `GameFlowManager` loads scenes locally — must become network scene loads.
- `PlayerPrefs` score is machine-local — move to server session state.
- `Time.timeScale = 0` pauses the whole listen server.
- Cursor lock is per client; the host must not force it globally.
- Bots and objectives must be disabled on clients, or clients will simulate
  their own copy.

## 10. Phases and acceptance criteria

| Phase | Deliverable | Done when |
|---|---|---|
| 0 | Repo, packages, host/join UI | Project compiles with NGO |
| 1 | Networking foundation | 4 players walk around together |
| 2 | Round loop | A round can be won/lost with dummy bots |
| 3 | Bot director | 4 players clear a bot round on Foundry |
| 4 | Convert missions 1–2 | Best-of-5 playable on both existing maps |
| 5 | Build Depot + Core | All four missions playable |
| 6 | Match flow + polish | Full 4-mission LAN session on real hardware |

## 11. Effort and risks

Rough part-time estimate: **3–5 months** for the full four-mission version;
**6–8 weeks** for a one-mission vertical slice.

Risks, in order:

1. **Level authoring** — two new maps is more work than the networking.
2. **Bot quality** — the Microgame AI is basic; aim for readable archetypes.
3. **Round edge cases** — simultaneous last deaths, time expiry, double ends.
4. **Scope creep** — economy, bomb, PvP, ranked stats.
