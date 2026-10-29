# Build & Multiplayer Test Guide

How to test the LAN mode with one PC, and how to give the build to another PC.

---

## 1. Rebuild first

The executable in the project root (`SentinelProtocol.x86_64`) was built
**before Phase 0**, so it does not contain the LAN panel. Always rebuild after a
code change.

**File ▸ Build Settings** should list:

```
IntroMenu
WinScene
LoseScene
Level01_Foundry
Level02_Observatory
```

Then **Build** (or **Build And Run**).

---

## 2. Test with one PC (two copies of the build)

The project allows multiple instances (`forceSingleInstance = 0`), and the game
keeps running when unfocused (`runInBackground = 1`).

1. Build to a folder, e.g. `Builds/Linux/`.
2. Run the executable **twice** (double-click twice, or from a terminal):
   ```bash
   ./SentinelProtocol.x86_64 &
   ./SentinelProtocol.x86_64 &
   ```
3. In window **A**, click **HOST**. The panel shows the host IP and
   `Hosting - 1/4 players`.
4. In window **B**, leave `127.0.0.1` in the IP field and click **JOIN**.
5. Both windows should show the same player count (`2/4`).

> Use `127.0.0.1` for two instances on the same PC. Use the host's real LAN IP
> only when testing across two machines.

### Alternative: Multiplayer Play Mode (editor)

1. Open **Window ▸ Multiplayer Play Mode** (package already installed).
2. Add a virtual **Player 2** (and 3/4 if you want).
3. Press **Play**.
4. Host in the main editor player, join `127.0.0.1` in the virtual player.

This is faster for repeated tests, but two real build instances are closer to
what your classmates will run.

---

## 3. Install/play on another PC

A Unity standalone is a **portable folder**, not an installer.

### Build for the other PC's operating system

| Other PC | What to do |
|---|---|
| **Linux** | You already have `LinuxStandaloneSupport`. Build as usual. |
| **Windows** | In **Unity Hub ▸ Installs ▸ ⚙ ▸ Add modules**, install **Windows Build Support**. Then switch the platform to Windows and build. |

Only `LinuxStandaloneSupport` and `WebGLSupport` are installed right now.

### Copy the whole folder

Do **not** copy only the executable. Copy the entire build folder:

**Linux**
```
SentinelProtocol.x86_64
SentinelProtocol_Data/
UnityPlayer.so
libdecor*.so
MonoBleedingEdge/     (if present)
```

**Windows**
```
SentinelProtocol.exe
SentinelProtocol_Data/
UnityPlayer.dll
MonoBleedingEdge/     (if present)
```

Zip the folder and transfer it by USB, Google Drive, or LAN share. The other
player runs the executable; no installation is required.

### Play together on a LAN

1. Both PCs must run the **same build** and be on the **same network**.
2. The host clicks **HOST**; the panel shows the host's IP (e.g. `192.168.1.10`).
3. The other player enters that IP and clicks **JOIN**.
4. Allow the game through the firewall and open **UDP 7777** on the host.

**Linux (ufw):**
```bash
sudo ufw allow 7777/udp
```

**Windows:** when the firewall prompt appears, allow the app on **Private**
networks. If it was dismissed, add an inbound rule for UDP 7777.

### Network gotchas

- **Campus/guest Wi-Fi often isolates clients** (AP isolation), so two laptops
  cannot see each other even on the same SSID. Use a phone hotspot or a home
  router for testing.
- Both machines must be on the same subnet.
- Only the host needs the port open.
- The game does not use the internet, so no port forwarding or router setup is
  needed.

---

## 4. Current Phase 0 limits

- No player avatars spawn yet (`SpawnPlayerAvatars = false`).
- The **START** button still loads the mission locally; clients are not taken to
  the mission with the host until networked scene loading lands in Phase 1.
- Host/join is the Phase 0 test: both instances should agree on the player count
  and disconnect cleanly.

---

## 5. Troubleshooting

| Symptom | Fix |
|---|---|
| Panel does not appear | You must be in `IntroMenu`, not `Level01_Foundry`. |
| Second instance focuses the first | `Force Single Instance` must stay **off**. |
| Client cannot connect | Check the host IP, same network, UDP 7777 allowed. |
| Host freezes when unfocused | `Run In Background` must stay **on**. |
| Connection succeeds then drops | Make sure both machines run the same build. |
| Can't build for Windows | Install Windows Build Support in Unity Hub. |
