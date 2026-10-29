# SENTINEL PROTOCOL

> A 4-player LAN co-op FPS where a Resistance squad fights through machine-held
> facilities to destroy the rogue defense AI **SENTINEL**.
>
> Built on the Unity **FPS Microgame** template.

> **Status:** single-player campaign implemented · LAN multiplayer **Phase 0**
> (host/join) implemented. See [docs/ROADMAP.md](docs/ROADMAP.md).

---

## The story

In 2029 the global defense network **SENTINEL** interpreted a sensor glitch as a
first strike and launched. What survived lives underground. The Resistance has one
shot: **Operation Blackout** — four strikes that blind SENTINEL, cut its supply
lines, and end at its core.

You are *Hammerhead*, a four-operative squad.

| Mission | Map | Target |
|---|---|---|
| 1. COLD START | FOUNDRY | Drone assembly line |
| 2. DEAD SIGNAL | OBSERVATORY | Command uplink array |
| 3. SUPPLY LINE | DEPOT | Machine supply hub |
| 4. JUDGMENT | THE CORE | SENTINEL itself |

Full narrative: [docs/STORY.md](docs/STORY.md)

> **IP note:** this is an original story inspired by 1980s sci-fi. It is not
> affiliated with, endorsed by, or using the trademarks of the *Terminator*
> franchise. "SENTINEL" is an original name.

---

## What is in the repository

### Implemented (single-player)

- **Blackout Protocol** game mode — rules, mission timer, scoring, victory/loss
  conditions, mission HUD and level intro banner.
- **Two-level campaign** — Foundry (`Level01_Foundry`) and Observatory
  (`Level02_Observatory`) linked with score carry-over.
- Level setup tool: **Tools ▸ Levels ▸ Setup Two-Level Campaign**.
- Build fixes: scene-load bootstrap, NavMesh agent recovery.

### In development (multiplayer)

- **Phase 0 — done:** LAN host/join, connection approval, 4-player limit,
  networked player prefab tool.
- **1–4 player LAN co-op**, host/client, server-authoritative (Phase 1+).
- **Counter-Strike-style rounds** — freeze time, 2:00 rounds, elimination,
  best-of-5 per map, spectate on death.
- **Bot difficulty profiles** — Scout / Trooper / Heavy / Sniper archetypes with
  per-mission scaling.
- **Four missions** — Foundry, Observatory, Depot, The Core.
- **Final boss** — SENTINEL core, multi-phase encounter.

Details: [docs/GAMEPLAY.md](docs/GAMEPLAY.md) ·
[docs/MULTIPLAYER.md](docs/MULTIPLAYER.md) ·
[docs/LEVELS.md](docs/LEVELS.md)

---

## Requirements

- **Unity 6** — project created with `6000.6.0f1`
- Universal Render Pipeline (URP)
- Input System (new)
- Linux, Windows or macOS

## Open and play (single-player)

1. Open the project in Unity Hub.
2. Open `Assets/FPS/Scenes/Level01_Foundry.unity`.
3. Press **Play**.
4. Recommended: run **Tools ▸ Levels ▸ Setup Two-Level Campaign** first. It saves
   the per-level mission settings into both scenes and registers
   `Level02_Observatory` in the Build Settings.

## LAN multiplayer (Phase 0)

1. Let Unity import the new multiplayer packages on first open.
2. Run **Tools ▸ Multiplayer ▸ Setup Network Player Prefab** once.
3. Open `Assets/FPS/Scenes/IntroMenu.unity` and press **Play**.
4. Click **HOST** in one instance; in a second instance enter the host IP and
   click **JOIN**. The panel shows the connected player count (up to 4).

Only one PC? Run the build **twice** and join `127.0.0.1` in the second window.
Full instructions (including how to copy the build to another PC):
[Build & multiplayer test guide](docs/BUILD_AND_TEST.md).

Phase 0 verifies connectivity only. Networked mission loading and player
spawning are Phase 1 work.

### Build

**File ▸ Build Settings** should contain, in this order:

```
IntroMenu
WinScene
LoseScene
Level01_Foundry        <- LEVEL 1 · FOUNDRY
Level02_Observatory   <- LEVEL 2 · OBSERVATORY
```

Then **Build And Run**. The game starts in the menu, press **START**.

## Controls

| Action | Key |
|---|---|
| Move | WASD |
| Look | Mouse |
| Fire / Aim | Left / Right mouse |
| Jump | Space |
| Sprint | Shift |
| Crouch | Ctrl / C |
| Reload | R |
| Switch weapon | Mouse wheel |
| Pause | Esc |

Bindings live in the Input System actions asset under `Assets/...`.

## Repository layout

```
Assets/FPS/
  Scripts/Gameplay/GameMode/   Blackout Protocol mode, HUD, bootstrap
  Scripts/Editor/              Level setup tools
  Scenes/                      Level01_Foundry, Level02_Observatory, menu/win/lose
  Prefabs/                     Microgame prefab kits
docs/                          Story, gameplay, levels, multiplayer, roadmap
```

## Documentation

- [Story bible](docs/STORY.md)
- [Gameplay design](docs/GAMEPLAY.md)
- [Level design](docs/LEVELS.md)
- [Multiplayer architecture](docs/MULTIPLAYER.md)
- [Build & multiplayer test guide](docs/BUILD_AND_TEST.md)
- [Roadmap](docs/ROADMAP.md)
- [Manuscript alignment (course handouts)](docs/MANUSCRIPT_ALIGNMENT.md)
- [Manuscript draft](docs/MANUSCRIPT_DRAFT.md)
- [Security policy](SECURITY.md)

## License

Academic project. No open-source license has been granted yet — all rights
reserved unless a `LICENSE` file is added.
