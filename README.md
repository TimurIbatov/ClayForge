# ClayForge

Playable Unity 6 prototype of a pottery wheel — not an empty skeleton.

## Open

Unity Hub → Open → `~/Projects/ClayForge`  
Editor: **6000.6.2f1**

First open: menu **ClayForge → Bootstrap Project** (creates scenes, clay types, URP, build settings).

Batch:

```bash
~/Unity/Hub/Editor/6000.6.2f1/Editor/Unity \
  -batchmode -nographics \
  -projectPath "$(pwd)" \
  -executeMethod ClayForge.EditorTools.ProjectBootstrap.RunFromCommandLine \
  -quit -logFile Logs/bootstrap.log
```

Play **Boot** (or press Play after bootstrap). Boot → Main Menu → Workshop.

## What you can do now

- Orbit around the lump with the mouse; **LMB / RMB** press left / right hand
- **WASD** nudge, **Q/E** extra height, **Shift / Ctrl** force
- **Scroll** wheel RPM, **Space** stop/start
- **F** water, **C** cut, **R** new lump, **Tab** next clay, **V** camera, **G** save piece
- HUD: RPM, centering bar, moisture
- **Dry / Fire / Glaze** actually change the lump (shrink + color)

Centering matters: a fresh lump is off-axis and wobbles at speed. Press **both** hands to pull it in.

## Controls

| Input | Action |
| --- | --- |
| Mouse | Hand height + orbit |
| LMB / RMB | Left / right pressure |
| WASD | Nudge hands |
| Shift / Ctrl | Strong / fine |
| Scroll | RPM |
| Space | Wheel on/off |
| F / C / R / Tab / V / G | Water / cut / reset / clay / camera / save |

## Architecture

`ClayBody` is a facade over `IClaySolver`. Prototype solver is a visco-plastic **lathe** (rings + volume conservation). Swap in particles/compute later without rewriting hands, wheel, or Steam.
