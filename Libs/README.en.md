# Libs directory

This directory holds the reference DLLs needed to build this mod. **These files ship with the game and are copyrighted, so they are not committed to the Git repository.**

## File list and sources

Let `GAME` be the Warm Snow install folder:

- **Ubuntu (snap Steam)**: `~/snap/steam/common/.local/share/Steam/steamapps/common/WarmSnow`
- **Ubuntu (regular Steam)**: `~/.local/share/Steam/steamapps/common/WarmSnow`
- **Windows**: `...\SteamLibrary\steamapps\common\WarmSnow`

### Copy from the game (required)

| File | Source (relative to GAME) |
|------|------|
| `Assembly-CSharp.dll` | `WarmSnow_Data\Managed\Assembly-CSharp.dll` |
| `UnityEngine.dll` | `WarmSnow_Data\Managed\UnityEngine.dll` |
| `UnityEngine.CoreModule.dll` | `WarmSnow_Data\Managed\UnityEngine.CoreModule.dll` |
| `UnityEngine.IMGUIModule.dll` | `WarmSnow_Data\Managed\UnityEngine.IMGUIModule.dll` |
| `UnityEngine.InputLegacyModule.dll` | `WarmSnow_Data\Managed\UnityEngine.InputLegacyModule.dll` |
| `UnityEngine.TextRenderingModule.dll` | `WarmSnow_Data\Managed\UnityEngine.TextRenderingModule.dll` |

### Copy from BepInEx (required; only available after installing BepInEx)

| File | Source (relative to GAME) |
|------|------|
| `BepInEx.dll` | `BepInEx\core\BepInEx.dll` |
| `0Harmony.dll` | `BepInEx\core\0Harmony.dll` |

## One-shot copy (Ubuntu example)

```bash
GAME="$HOME/snap/steam/common/.local/share/Steam/steamapps/common/WarmSnow"
cp "$GAME/WarmSnow_Data/Managed/"{Assembly-CSharp,UnityEngine,UnityEngine.CoreModule,UnityEngine.IMGUIModule,UnityEngine.InputLegacyModule,UnityEngine.TextRenderingModule}.dll Libs/
cp "$GAME/BepInEx/core/"{BepInEx,0Harmony}.dll Libs/
```

> Note: if `WarmSnow_Data\Managed\` has no `Assembly-CSharp.dll`, look for `GameAssembly.dll`
> (that means it's an IL2CPP build and this BepInEx 5 approach won't work).
> Warm Snow is currently a Mono build, so `Assembly-CSharp.dll` should be there.
