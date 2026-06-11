# Rezonans

A first-person psychological horror game built with Unity 6 and HDRP.

## Story

You wake up in a dimly lit, unfamiliar room — no memory of who you are or how you got there. As you explore the apartment, fragmented clues begin to surface: a family photo, diary pages, a calendar, identity documents. The deeper you dig, the more your grip on reality slips.

Your sanity is your only compass. Lose it, and the walls close in. Keep it, and the truth — however painful — might set you free.

**Two endings await you:**
- **Bad Ending** — your sanity shatters before you find the way out.
- **Good Ending** — you piece together the truth and escape, waking in a hospital surrounded by those who waited for you.

## Gameplay Features

- First-person exploration and interaction
- Sanity system — triggers visual distortions and monologues as it drops
- Collectible piece system — gather evidence to unlock the good ending
- Drag-to-open doors and drawers (mouse-driven)
- Auto-advancing dialogue with typewriter effect
- Ghost silhouette with flickering light — appears briefly when triggered
- Loading screen with progress bar
- Pause menu
- Multiple endings based on sanity level and collected pieces

## Scenes

| Scene | Description |
|-------|-------------|
| `MainMenu` | Title screen |
| `LoadingMenu` | Loading screen with async scene loading |
| `kendisahnem` | Main gameplay scene |
| `BadEnding` | Ending triggered when sanity is too low |
| `GoodEnding` | Hospital awakening — triggered with high sanity + all pieces found |

## Assets Used

| Asset | Version | Usage |
|-------|---------|-------|
| **FPS Horror Game Starter Pack** | https://assetstore.unity.com/packages/templates/packs/fps-horror-game-starter-pack-310075  | All core systems (FPS control, sanity, doors, dialogue, interaction, pieces, ghosts) |
| **Open Source Pause Menu** | https://assetstore.unity.com/packages/tools/gui/open-source-pause-menu-59478| Pause menu, main menu, settings panels |
| **Simple Keys** |https://assetstore.unity.com/packages/3d/props/tools/simple-keys-231162| In-game key objects and visuals |
| **Cheval Mirror** | https://assetstore.unity.com/packages/3d/props/cheval-mirror-259424  | In-game mirror object and reflection mechanic |
| **Footsteps – Essentials** | https://assetstore.unity.com/packages/audio/sound-fx/foley/footsteps-essentials-189879  | Walking, running, landing sound effects |
| **Voices – Essentials** | https://assetstore.unity.com/packages/audio/sound-fx/voices/voices-essentials-214441 | Character voice, breathing, panic sounds |

## Tech Stack

- **Engine:** Unity 6 (6000.4.2f1)
- **Render Pipeline:** HDRP (High Definition Render Pipeline)
- **Language:** C#
- **Post-Processing:** Lens Distortion, Chromatic Aberration, Motion Blur (on wakeup effect)
- **Audio:** 3D spatial audio via Unity AudioSource

## Key Scripts

| Script | Description |
|--------|-------------|
| `DialogueSystem.cs` | Auto-advancing typewriter dialogue with Space-to-skip |
| `GeneralTrigger.cs` | Flexible trigger system (zone enter, sanity threshold, piece count) |
| `SanityManager.cs` | Tracks sanity, fires events at thresholds |
| `FlickerLight.cs` | Ghost light flicker effect, activated by trigger |
| `SilhouetteAppear.cs` | Ghost silhouette that briefly becomes visible |
| `WakeUpEffect.cs` | Post-processing wakeup effect (lens distortion → clear) |
| `GoodEndingManager.cs` | Hospital awakening sequence and main menu transition |
| `LoadingScreen.cs` | Async scene loading with minimum display time |
| `DragToOpenSystem.cs` | Mouse-drag door opening |
| `DrawerSystem.cs` | Mouse-drag drawer interaction |
| `PieceManager.cs` | Tracks collected evidence pieces |

## Controls

| Key / Input | Action |
|-------------|--------|
| `WASD` | Move |
| `Mouse` | Look |
| `E` | Interact |
| `Hold E + Mouse` | Drag doors / drawers |
| `Space` | Skip dialogue line |
| `Escape` | Pause menu |


