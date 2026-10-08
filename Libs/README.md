# Libs 目录说明

这个目录用来放编译本 mod 需要的引用 dll。**这些文件是游戏自带的，有版权，不要提交到 Git 仓库**
（已在 `.gitignore` 中忽略，只保留在本地）。

> 本机（Ubuntu）已经把游戏 DLL 和 BepInEx 核心 DLL 复制进来了，可直接 `./build.sh` 编译。
> 换电脑或游戏更新后，请按下面的路径重新复制一遍。

## 文件清单与来源

暖雪的安装目录记为 `GAME`：

- **Ubuntu（snap 版 Steam）**：`~/snap/steam/common/.local/share/Steam/steamapps/common/WarmSnow`
- **Ubuntu（普通 Steam）**：`~/.local/share/Steam/steamapps/common/WarmSnow`
- **Windows**：`...\SteamLibrary\steamapps\common\WarmSnow`

### 从游戏本体复制（必需）

| 文件 | 来源（相对 GAME） |
|------|------|
| `Assembly-CSharp.dll` | `WarmSnow_Data\Managed\Assembly-CSharp.dll` |
| `UnityEngine.dll` | `WarmSnow_Data\Managed\UnityEngine.dll` |
| `UnityEngine.CoreModule.dll` | `WarmSnow_Data\Managed\UnityEngine.CoreModule.dll` |
| `UnityEngine.IMGUIModule.dll` | `WarmSnow_Data\Managed\UnityEngine.IMGUIModule.dll` |
| `UnityEngine.InputLegacyModule.dll` | `WarmSnow_Data\Managed\UnityEngine.InputLegacyModule.dll` |
| `UnityEngine.TextRenderingModule.dll` | `WarmSnow_Data\Managed\UnityEngine.TextRenderingModule.dll` |

### 从 BepInEx 复制（必需，装好 BepInEx 后才有）

| 文件 | 来源（相对 GAME） |
|------|------|
| `BepInEx.dll` | `BepInEx\core\BepInEx.dll` |
| `0Harmony.dll` | `BepInEx\core\0Harmony.dll` |

## 一键复制（Ubuntu 示例）

```bash
GAME="$HOME/snap/steam/common/.local/share/Steam/steamapps/common/WarmSnow"
cp "$GAME/WarmSnow_Data/Managed/"{Assembly-CSharp,UnityEngine,UnityEngine.CoreModule,UnityEngine.IMGUIModule,UnityEngine.InputLegacyModule,UnityEngine.TextRenderingModule}.dll Libs/
cp "$GAME/BepInEx/core/"{BepInEx,0Harmony}.dll Libs/
```

> 提示：`WarmSnow_Data\Managed\` 目录下没有 `Assembly-CSharp.dll` 的话，
> 找找有没有 `GameAssembly.dll`（那说明是 IL2CPP 版本，这套 BepInEx 5 写法不适用）。
> 暖雪目前是 Mono 版本，正常能找到 `Assembly-CSharp.dll`。
