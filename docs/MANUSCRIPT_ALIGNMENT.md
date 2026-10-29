# Manuscript Alignment — SENTINEL PROTOCOL

> How the game maps to the six IT2012 handouts and to the
> **GameDev Prefinal Manuscript Template**.
>
> Companion document: [`MANUSCRIPT_DRAFT.md`](MANUSCRIPT_DRAFT.md) — ready-to-edit
> content for every manuscript section and table.

---

## 1. Verdict

**Strong alignment overall.** The game was already close to the course's
framework, because it is a first-person action game with explicit rules,
victory/loss conditions, levels, and a story. The handouts add precise
vocabulary and a few design requirements that the current plan was missing:

| Area | Status |
|---|---|
| Platform & player mode | ✅ Already **computer / LAN-based multiplayer** |
| Genre | ✅ Already **action → shooter → FPS** |
| Time interval | ✅ Real-time with **time-limited** rounds |
| Rules, victory, loss | ✅ Already explicit |
| Interactivity modes | ⚠️ Add **player-to-developer** explicitly |
| Game theory | ⚠️ Name it **non-zero-sum** co-op vs NPCs |
| Challenges | ⚠️ Formalize **explicit / implicit**, **perfect / imperfect information** |
| Level structure | ⚠️ Add **objectives, flow, duration, availability, relationship, progression** |
| Story | ⚠️ Add **three-act structure**, **premise/backstory/synopsis/theme/setting/plot**, archetypes |
| Target audience | ⚠️ Define a concrete **persona** (demographics/psychographics) |
| Balance | ⚠️ Add **difficulty selection** and the five balance criteria |
| Originality | ⚠️ Keep it **inspired by**, not a Terminator clone |

The gaps are wording and design-formalization gaps, not structural problems.
The draft in `MANUSCRIPT_DRAFT.md` closes them.

---

## 2. Concept coverage matrix

### Handout 1 — Platforms and Player Modes (+ Time Intervals)

| Concept | How Sentinel Protocol uses it | Manuscript section |
|---|---|---|
| **Computer platform** | PC (Windows/Linux) build, keyboard + mouse | Description → Platform; Game Availability |
| **LAN-based multiplayer** | 1–4 players on the same local network, no internet, no shared screen | Description; Interactivity Modes; Game Availability |
| **Single-player** | Solo mode with AI squad support / same missions | Interactivity Modes |
| **Real-time** | No turns; continuous movement and combat | Description; Gameplay |
| **Time-limited** | 2:00 round clock + freeze time; mission clock pressure | Rules; Gameplay; Game Duration |
| Turn-based / local / online | **Not used** — state this explicitly as a design choice | Scope |

### Handout 2 — Goals and Genres

| Concept | How Sentinel Protocol uses it | Manuscript section |
|---|---|---|
| **Entertainment** (primary goal) | Fun, reflex-driven co-op shooter | Background; Description |
| **Social** (secondary goal) | 4-player teamwork, communication, shared objective | Significance; Interactivity |
| **Action genre** | Fast combat, reaction time, reflex decisions | Description → Genre |
| **Shooters → FPS sub-genre** | First-person view, visible weapon, no avatar on screen | Description → Genre; Gameplay |
| Other genres | Not used (no RPG progression, no simulation, no strategy economy) | Scope |

### Handout 3 — Player Elements

| Concept | How Sentinel Protocol uses it | Manuscript section |
|---|---|---|
| **Social interaction** | Core motivation: playing with friends on LAN | Significance; Description → Audience |
| **Competition** | Score, round wins, difficulty tiers | Gameplay; Player Progression |
| **Mastery** | Four difficulty levels, best-of-5, boss execution | Challenges; Progression |
| **Escapism** | Machine-uprising sci-fi fantasy | Background; Story |
| **Knowledge** | Learning maps, bot patterns, boss phases | Challenges |
| **Geographics** | Philippines / SEA PC players, local co-op culture | Description → Audience |
| **Psychographics** | Sci-fi fans, mid-core players, co-op-oriented groups | Description → Audience |
| **Demographics** | Ages ~16–30, students/young professionals | Description → Audience |
| MBTI / generations | Optional support, not a design driver | Scope |

### Handout 4 — Story and Character Development

| Concept | How Sentinel Protocol uses it | Manuscript section |
|---|---|---|
| **Three-act structure** | Act I takeover + briefing; Act II four missions; Act III core/boss | Story; Background |
| **Premise** | A Resistance squad must destroy the AI that ended the world | Description; Story |
| **Backstory** | SENTINEL misread a sensor ghost and launched in 2029 | Background; Story |
| **Synopsis** | Four escalating strikes ending at the core | Story; Game Flow |
| **Theme** | Can humanity outthink the machine it created? | Story; Conclusion |
| **Setting** | 2029, machine-held industrial facilities | Description → Setting |
| **Plot** | Mission sequence forces harder decisions toward the core | Game Flow |
| **Protagonist / hero** | The player's Resistance operative (Hammerhead squad) | Description → Role |
| **Antagonist / shadow** | SENTINEL | Story |
| **Mentor** | Resistance command on radio | Story |
| **Ally** | Co-op teammates and squad callsigns | Story; Interactivity |
| **Herald** | Mission briefings that announce each strike | Story; Game Flow |
| **Guardian** | Elite bots guarding shield nodes | Story; Challenges |
| Character triangle / arc | Squad ↔ SENTINEL ↔ survivors; squad grows from survivors to liberators | Story |

### Handout 5 — Gameplay

| Concept | How Sentinel Protocol uses it | Manuscript section |
|---|---|---|
| **Choices, challenges, consequences** | Route choice, target priority, ammo/health trade-offs | Gameplay |
| **Rules** | Movement, interaction, resources, restrictions, penalties | Rules and Mechanics |
| **Victory conditions** | Round, mission, campaign | Victory and Loss |
| **Loss conditions (explicit)** | Player death, round timer expiry | Victory and Loss |
| **Loss conditions (implicit)** | Attrition, ammo starvation, poor positioning | Victory and Loss |
| **Player-to-game** | FPS controls, HUD, physics, pickups | Interactivity |
| **Player-to-player** | Co-op, shared objective, communication/pings | Interactivity |
| **Player-to-developer** | GitHub issues, playtest feedback, patch notes | Interactivity |
| **Non-zero-sum** | Players cooperate against NPCs (the handout's own example) | Interactivity; Game Theory |
| **Explicit challenges** | Bot assaults, boss, timer | Challenges |
| **Implicit challenges** | Resource management, positioning, communication | Challenges |
| **Perfect / imperfect information** | Imperfect: fog, unknown bot positions; remove enemy markers | Challenges |
| **Intrinsic / extrinsic knowledge** | Map/bot patterns vs. FPS skill | Challenges |
| **Spatial awareness / resource management / reaction time** | Core skill tests | Challenges |
| **Game goals** | Advancement, conflict, capture, solution, outwit | Challenges; Progression |
| **Balance criteria** | Consistent challenge, fairness, no stagnation, no trivial decisions, difficulty levels | Challenges; Progression |

### Handout 6 — Levels

| Concept | How Sentinel Protocol uses it | Manuscript section |
|---|---|---|
| **Level design** | Four missions, each with a unifying theme | Levels; Game Structure |
| **Objectives** | Clear per-mission objective + briefing + HUD | Objectives |
| **Flow** | One-way mission progression, sector lockdown, no backtracking | Game Flow |
| **Duration** | ~15–25 min per mission; one mission per session | Game Duration |
| **Availability** | Linear, one mission at a time (fits first-person immersion) | Game Availability |
| **Relationship** | Episodic, story-connected (TV-episode analogy) | Relationships |
| **Progression** | Recruit → Veteran → Elite → Nightmare; new bots/mechanics | Player Progression |
| **Time (real-world)** | Real-time, time-limited rounds, mission clock | Game Duration |
| **Camera & perspective** | First-person | Gameplay; Space |
| **Terrain & materials** | Industrial metal, concrete, hazard stripes | Levels; Space |
| **Radiosity & effects** | Baked lighting, emergency lights, fog, damage VFX | Levels; Space |
| **Scale** | Human-scale arenas sized for four players | Levels |
| **Realism** | Stylized realism, not simulation | Levels; Style |
| **Style** | 1980s industrial sci-fi, consistent across UI/art | Story; Levels |

---

## 3. Manuscript section checklist

| Manuscript section | Required content | Sentinel Protocol answer | Status |
|---|---|---|---|
| Background of the Study | topic, importance, inspiration, problem, originality | Game design + co-op FPS + machine-uprising inspiration + teamwork problem | ✅ drafted |
| Description of the Game | title, genre, audience, platform, setting, role, concept, unique feature | See draft | ✅ drafted |
| Statement of the Problem | general + specific questions | 1 general + 5 specific | ✅ drafted |
| Objectives | general + specific | 1 general + 5 specific | ✅ drafted |
| Significance | 4 beneficiary groups | students, players, designers, researchers | ✅ drafted |
| Scope and Limitations | coverage, platform, limits, exclusions | 4 missions, LAN, no PvP/online, prototype limits | ✅ drafted |
| Gameplay | loop, role, environment, unique mechanics, experience | Round loop, operative role, facilities, co-op rounds | ✅ drafted |
| Rules and Mechanics | table: movement, interaction, resources, restrictions, penalties, special | 6-row table | ✅ drafted |
| Victory and Loss | exact conditions + failure handling | Round/mission/campaign + explicit/implicit | ✅ drafted |
| Interactivity Modes | table: input, player mode, environment, character, feedback | 5-row table + 3 interactivity modes + non-zero-sum | ✅ drafted |
| Challenges and Obstacles | table: 3 + final challenge, skill, difficulty | 4-row table + balance | ✅ drafted |
| Game Structure → Objectives | main + secondary | 4 mission objectives + optional score goals | ✅ drafted |
| Game Flow | start → stages → ending | Menu → lobby → 4 missions → boss → ending | ✅ drafted |
| Game Duration | session, level, total, save | 3 min round, 15–25 min mission, 60–100 min campaign | ✅ drafted |
| Game Availability | platform, internet, requirements, accessibility | PC, LAN, specs, accessibility list | ✅ drafted |
| Relationships | player/environment/characters/objectives/resources | 5 relationships | ✅ drafted |
| Player Progression | stage table | Stage 1 / 2 / 3 / Final | ✅ drafted |
| Summary / Conclusion / Recommendations | six components, assessment, next steps | See draft | ✅ drafted |
| References | actual sources | Novak; Buttfield-Addison et al.; Unity docs | ✅ drafted |

---

## 4. Design changes to apply (and why)

These are the only real changes needed for alignment. Most are small.

1. **Difficulty selection (Handout 5 — balance: difficulty levels).**
   Add a lobby choice: Recruit / Veteran / Elite / Nightmare. It scales bot
   count, health, damage, accuracy, reaction time, and fire rate. This is a
   stronger fit than fixed per-mission difficulty because the handout says
   players "should have a choice of difficulty."

2. **Mission briefing + status screen (Handout 6 — objectives).**
   The level-intro banner becomes a proper briefing card: mission name,
   objective, difficulty, and one line of story. The HUD is the status screen
   (objective, timer, alive count, score).

3. **One-way progression / sector lockdown (Handout 6 — flow).**
   After a sector is cleared, seal it (door/gate) so players cannot backtrack.
   This is the handout's "one-way barrier" technique and it signals progress.

4. **Imperfect information (Handout 5 — challenges).**
   Disable enemy compass markers in the co-op mode. Players should rely on
   sight, sound, and callouts. This is what makes the challenge *imperfect
   information* rather than a wallhack.

5. **Player-to-developer channel (Handout 5 — interactivity).**
   Use the public GitHub repository's Issues and playtest feedback as the
   documented player-to-developer interaction.

6. **Accessibility (Handout 6 — availability).**
   Add subtitles, remappable keys, mouse sensitivity, FOV slider, colorblind
   indicators, and difficulty selection.

7. **Concrete target audience (Handout 3).**
   Define one persona: 16–30, PC, Philippines/SEA, mid-core, plays with 1–3
   friends, likes sci-fi and co-op shooters, has limited time per session.

8. **Formal story elements (Handout 4).**
   Add explicit premise, backstory, synopsis, theme, setting, plot, three acts,
   and archetype mapping (see `STORY.md`).

9. **Formal level-structure analysis (Handout 6).**
   Document objectives, flow, duration, availability, relationship, and
   progression for the four missions (see `LEVELS.md`).

10. **Originality guardrail (Handout 6 — style).**
    The handouts warn against borrowing a style/setting. Keep the *genre*
    inspiration (machine uprising) but use original names, original facilities,
    and no Terminator-specific plot devices (no time travel, no John Connor
    analogue, no catchphrases). SENTINEL is a defense network, not Skynet.

11. **Scope honesty (Handout 5/6).**
    The manuscript describes the *proposed* game. State plainly what the
    prototype delivers (single-player campaign + LAN co-op vertical slice) and
    what is future work (online play, more maps, advanced AI).

12. **Session-length alignment (Handout 6 — duration).**
    Keep missions at ~15–25 minutes so a player completes at least one level
    per session, as the handout recommends for computer games.

---

## 5. Terminology cheat sheet

Use these exact terms in the manuscript; they are the handouts' vocabulary.

| Use | Not |
|---|---|
| LAN-based multiplayer | "LAN party mode", "local network thing" |
| Real-time; time-limited rounds | "live gameplay", "timed mode" |
| Action → shooter → first-person shooter (FPS) | "shooting game" |
| Non-zero-sum (co-op vs NPCs) | "team deathmatch", "PvE" alone |
| Explicit / implicit challenges | "hard parts" |
| Perfect / imperfect information | "fog of war" alone |
| Explicit / implicit loss conditions | "you lose when you die" |
| Player-to-game / player-to-player / player-to-developer | "single/multiplayer" |
| Level structure: objectives, flow, duration, availability, relationship, progression | "level list" |
| Camera & perspective; terrain & materials; radiosity & effects; scale; style | "graphics" |
| Advancement, conflict, capture, solution, outwit | "goals" |

---

## 6. What is implemented vs. proposed

Be transparent in **Scope and Limitations**:

**Implemented in the repository**

- Blackout Protocol single-player mode (rules, timer, score, win/lose, HUD).
- Two-level campaign (Foundry + Observatory) with score carry-over.
- Prefab-based levels, build setup tooling, and documentation.

**Proposed in this manuscript**

- 1–4 player LAN co-op.
- Counter-Strike-style elimination rounds (best-of-5 per mission).
- Four missions: Foundry, Observatory, Depot, The Core.
- Four bot archetypes with difficulty scaling.
- Three-phase SENTINEL boss.

This distinction protects the paper academically and still presents the full
design.

---

## 7. Pre-submission checklist

- [ ] Replace `[Member]`, `[Group Name]`, `[Section]`, `[Date]` placeholders.
- [ ] Add screenshots of the two implemented levels and the HUD.
- [ ] Add the game-flow diagram (drawn or exported).
- [ ] Add the four level maps/layout sketches.
- [ ] Update the Table of Contents page numbers after formatting.
- [ ] Keep citations to Novak (2012) and Buttfield-Addison et al. (2019).
- [ ] Run a plagiarism/originality check.
- [ ] Proofread for consistent terminology (see §5).
- [ ] Confirm the Scope section matches what is actually playable at submission.
