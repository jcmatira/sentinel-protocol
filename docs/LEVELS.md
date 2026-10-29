# Level Design

## Implemented today — two-level campaign

```
IntroMenu -> LEVEL 1 · FOUNDRY (MainScene) -> LEVEL 2 · OBSERVATORY (SecondaryScene) -> WinScene
```

| | Level 1 — FOUNDRY | Level 2 — OBSERVATORY |
|---|---|---|
| Scene | `Assets/FPS/Scenes/MainScene.unity` | `Assets/FPS/Scenes/SecondaryScene.unity` |
| Theme | Underground foundry / dungeon | Multi-floor observatory with pit and balcony |
| Bots | 2 (HoverBot + Turret) | 3 (2 × HoverBot + Turret) |
| Time | 150 s | 180 s |
| Pickups | 2 × health | 6 × health, Shotgun, Grenade Launcher |
| Next | SecondaryScene | WinScene |

Both levels are assembled entirely from the Microgame prefab kits
(`Assets/FPS/Prefabs`): Floors, Walls, Dungeon, Rooms, MainLvl sections,
`Enemy_*`, `Pickup_*`, `Player`, `GameManager`, `VFX_*`, `Directional Light`.

### Setup

1. Open Unity.
2. Run **Tools ▸ Levels ▸ Setup Two-Level Campaign**.
   - Adds a configured `Game Mode Manager` to both scenes.
   - Registers `SecondaryScene` in Build Settings.
3. Build Settings order:
   `IntroMenu, WinScene, LoseScene, MainScene, SecondaryScene`.

---

# Planned — four-mission co-op arc

The single-player campaign becomes the first draft of a four-mission
Counter-Strike-style co-op campaign.

| # | Mission | Map | Source | Character |
|---|---|---|---|---|
| 1 | COLD START | FOUNDRY | existing `MainScene` | Tight dungeon, close-range scouts |
| 2 | DEAD SIGNAL | OBSERVATORY | existing `SecondaryScene` | Vertical, mixed ranges |
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
MainScene        <- FOUNDRY
SecondaryScene   <- OBSERVATORY
Depot            <- SUPPLY LINE
Core             <- JUDGMENT
```
