# Roadmap

## Done

- [x] **Lab 05 — Gameplay:** Blackout Protocol rules, timer, score, win/lose,
      mission HUD, result banner.
- [x] **Lab 06 — Levels:** Foundry + Observatory linked into a two-level
      campaign with score carry-over and per-level config.
- [x] Level setup tool (`Tools ▸ Levels ▸ Setup Two-Level Campaign`).
- [x] Build fixes: scene-load bootstrap, Build Settings registration, NavMesh
      agent recovery.

## Phase 0 — Project & tooling

- [x] Repository, `.gitignore`, `.gitattributes`
- [x] Documentation set
- [ ] Install `com.unity.netcode.gameobjects`
- [ ] Install `com.unity.transport`
- [ ] Install `com.unity.multiplayer.playmode`
- [ ] Install `com.unity.multiplayer.tools` (optional)
- [ ] Host / Join UI on the menu
- [ ] Connection approval and max-player config

## Phase 1 — Networking foundation

- [ ] `NetworkManager` + Unity Transport
- [ ] Player prefab as `NetworkObject`
- [ ] `NetworkTransform` (owner authority)
- [ ] Spawn points for 4 players
- [ ] Enable movement/input only on the local owner
- [ ] Remote player proxies (no camera, no input, no audio listener)
- [ ] Test with 2–4 virtual players

**Milestone:** four players can join one mission and move around together.

## Phase 2 — Round loop

- [ ] `RoundManager` state machine (freeze / live / round end)
- [ ] Round timer and freeze timer
- [ ] Round score and best-of-5 match score
- [ ] Respawn all players at round start
- [ ] Spectator camera for dead players
- [ ] Elimination win checks (server)
- [ ] Replicated round state to HUD

**Milestone:** a complete round can be won or lost with dummy targets.

## Phase 3 — Bots

- [ ] `BotDirector` spawns bots from spawn points
- [ ] Server-only AI; clients see replicated bots
- [ ] `BotDifficultyProfile` ScriptableObject
- [ ] Scout / Trooper / Heavy / Sniper archetypes
- [ ] Per-mission profiles
- [ ] Round ends when the bot team is eliminated

**Milestone:** four players clear a bot round on Foundry.

## Phase 4 — Missions 1–2 converted

- [ ] Human + bot spawns on Foundry
- [ ] Human + bot spawns on Observatory
- [ ] NavMesh verification in build
- [ ] Round HUD integration
- [ ] Best-of-5 playable on both maps

**Milestone:** two-mission co-op session on LAN.

## Phase 5 — Missions 3–4

- [ ] Build **DEPOT** from the Dungeon/Rooms kit
- [ ] Build **THE CORE** from the Catwalk/Balcony kit
- [ ] Spawns, cover, lighting, NavMesh for both
- [ ] Boss encounter: shield nodes → defense grid → core

**Milestone:** all four missions playable.

## Phase 6 — Match flow & polish

- [ ] `MatchFlowManager` mission rotation
- [ ] Mission intro cards and radio chatter
- [ ] Results screen and final summary
- [ ] Join-in-progress / disconnect handling
- [ ] LAN test on 4 machines
- [ ] Build and run

**Milestone:** full four-mission co-op session.

## Backlog / ideas

- [ ] Kill feed
- [ ] Damage numbers
- [ ] Headshot multiplier
- [ ] Loadout selection at mission start
- [ ] Difficulty selection in the lobby
- [ ] Stats screen (kills, accuracy, time)
- [ ] Dedicated server build
- [ ] Relay/Lobby for online play
