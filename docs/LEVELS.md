# Level Design

## Level structure (course framework)

Each mission is designed around the level-structure model from the course:
**objectives, flow, duration, availability, relationship, and progression**.

| Mission | Objectives | Flow | Duration | Availability | Relationship | Progression |
|---|---|---|---|---|---|---|
| **1. Foundry** | Destroy the production-line defenders | Two lanes + a lower flank; one-way descent | 15–20 min | Linear | Introduces the round loop | Scouts + Troopers |
| **2. Observatory** | Clear the command uplink array | Pit → ramp → balcony; vertical push | 15–25 min | Linear | Raises stakes, adds elevation | Snipers + first Enforcers |
| **3. Depot** | Seize the supply hub | Yard → corridors → catwalk; sector lockdowns | 20–25 min | Linear | Resource pressure | Heavies + mixed waves |
| **4. The Core** | Destroy SENTINEL | Three shield platforms → defense grid → core | 20–30 min | Linear | Finale | Elite mix + boss |

**Flow.** After a sector is cleared, a gate seals it so the squad cannot
backtrack. This is a one-way barrier that signals progress and keeps players in
the encounter.

**Availability.** Missions unlock in sequence, one at a time. The course notes
that a single available level works well for first-person immersion; this also
keeps the lobby simple.

**Relationship.** The missions are episodic and story-connected: each is
self-contained, but each removes one SENTINEL advantage and leads to the next.

**Progression.** Difficulty rises through enemy count, health, damage, accuracy,
reaction time, fire rate, and new archetypes.

## Time and space

- **Time.** Real-time gameplay with time-limited rounds (2:00 live, 3 s freeze,
  5 s round end). The mission clock adds pressure; there is no altered time.
- **Camera and perspective.** First-person, matching the shooter sub-genre.
- **Terrain and materials.** Industrial metal, concrete, grating, hazard
  stripes, warning lights.
- **Radiosity and effects.** Baked lighting, emergency lighting, fog, dust,
  sparks, and damage VFX.
- **Scale.** Human-scale arenas sized for four players, with cover lanes and
  vertical routes.
- **Realism.** Stylized realism, not simulation.
- **Style.** 1980s industrial science fiction, consistent across levels and UI.

---

## Implemented today — two-level campaign

```
IntroMenu -> LEVEL 1 · FOUNDRY (Level01_Foundry) -> LEVEL 2 · OBSERVATORY (Level02_Observatory) -> WinScene
```

| | Level 1 — FOUNDRY | Level 2 — OBSERVATORY |
|---|---|---|
| Scene | `Assets/FPS/Scenes/Level01_Foundry.unity` | `Assets/FPS/Scenes/Level02_Observatory.unity` |
| Theme | Underground foundry / dungeon | Multi-floor observatory with pit and balcony |
| Bots | 2 (HoverBot + Turret) | 3 (2 × HoverBot + Turret) |
| Time | 150 s | 180 s |
| Pickups | 2 × health | 6 × health, Shotgun, Grenade Launcher |
| Next | Level02_Observatory | WinScene |

Both levels are assembled entirely from the Microgame prefab kits
(`Assets/FPS/Prefabs`): Floors, Walls, Dungeon, Rooms, MainLvl sections,
`Enemy_*`, `Pickup_*`, `Player`, `GameManager`, `VFX_*`, `Directional Light`.

### Setup

1. Open Unity.
2. Run **Tools ▸ Levels ▸ Setup Two-Level Campaign**.
   - Adds a configured `Game Mode Manager` to both scenes.
   - Registers `Level02_Observatory` in the Build Profiles scene list.
3. Build Profiles scene list order:
   `IntroMenu, WinScene, LoseScene, Level01_Foundry, Level02_Observatory`.

---

# Planned — four-mission co-op arc

The single-player campaign becomes the first draft of a four-mission
Counter-Strike-style co-op campaign.

| # | Mission | Map | Source | Character |
|---|---|---|---|---|
| 1 | COLD START | FOUNDRY | existing `Level01_Foundry` | Tight dungeon, close-range scouts |
| 2 | DEAD SIGNAL | OBSERVATORY | existing `Level02_Observatory` | Vertical, mixed ranges |
| 3 | SUPPLY LINE | DEPOT | new — Dungeon + Rooms kit | Corridors + open yard, heavies |
| 4 | JUDGMENT | THE CORE | new — Catwalk/Balcony + custom core room | Open arena, boss encounter |

## Per-map requirements

Every mission scene needs:

- **4–8 human spawn points** (`NetworkStartPosition` or custom `SpawnPoint`)
- **6–10 bot spawn points**, grouped by archetype
- **Cover**: crates, walls, machinery — enough for 4 players to fight from
- **NavMesh baked** (server-side AI)
- **Lighting**: one directional light + baked or realtime GI; consistent exposure
- **Audio**: ambience + combat mix
- **Boundaries**: no falling out of the map, no unreachable bots

## Mission design notes

### FOUNDRY
- Existing layout is the tutorial.
- Add a main arena and two flank routes so four players are not stacked.
- Bots: SCARAB scouts and HOUND troopers.
- Keep the lower dungeon as a risk/reward flank.

### OBSERVATORY
- Existing vertical layout: pit, ramp, steps, balcony.
- Good for snipers; add sightline breaks so WATCHER turrets are not oppressive.
- Bots: mix of HOUND and WATCHER, one ENFORCER.

### DEPOT (new)
- Dungeon/Room kit: a central loading yard surrounded by corridors and catwalks.
- Chokepoints for BULWARK heavies; side tunnels for flanking.
- Bots: BULWARK, HOUND, WATCHER.
- Theme: fuel lines, warning stripes, red emergency lighting.

### THE CORE (new)
- Catwalk/Balcony kit around a central pit housing SENTINEL.
- Three shield-node platforms around the core.
- Arena must support a three-phase boss:
  1. shield nodes + elites
  2. defense turrets + drone waves
  3. exposed core + final wave
- Layout must give four players room to split and regroup.

## Prefab usage (rubric note)

Every level is built from the Microgame prefabs. No custom meshes are required.
New maps should continue that rule so the project stays consistent and the
"made with prefabs" requirement is visible in every scene.

## Build settings (planned)

```
IntroMenu
WinScene
LoseScene
Level01_Foundry        <- FOUNDRY
Level02_Observatory   <- OBSERVATORY
Depot            <- SUPPLY LINE
Core             <- JUDGMENT
```
