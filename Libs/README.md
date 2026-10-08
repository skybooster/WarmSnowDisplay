# Libs 目录说明

这个目录用来放编译本 mod 需要的引用 dll。**这些文件是游戏自带的，有版权，不要提交到 Git 仓库**（已在 `.gitignore` 中忽略）。

请把下面这些文件从你的暖雪安装目录复制到 `Libs\` 里：

## 从游戏本体复制（必需）

游戏安装目录一般是：

```
Steam:  ...\SteamLibrary\steamapps\common\WarmSnow\
```

复制这些文件：

| 文件 | 来源 |
|------|------|
| `Assembly-CSharp.dll` | `WarmSnow_Data\Managed\Assembly-CSharp.dll` |
| `UnityEngine.dll` | `WarmSnow_Data\Managed\UnityEngine.dll` |
| `UnityEngine.CoreModule.dll` | `WarmSnow_Data\Managed\UnityEngine.CoreModule.dll` |
| `UnityEngine.IMGUIModule.dll` | `WarmSnow_Data\Managed\UnityEngine.IMGUIModule.dll` |
| `UnityEngine.InputLegacyModule.dll` | `WarmSnow_Data\Managed\UnityEngine.InputLegacyModule.dll` |
| `UnityEngine.TextRenderingModule.dll` | `WarmSnow_Data\Managed\UnityEngine.TextRenderingModule.dll` |

## 从 BepInEx 复制（必需，安装 BepInEx 后才有）

| 文件 | 来源 |
|------|------|
| `BepInEx.dll` | `BepInEx\core\BepInEx.dll` |
| `0Harmony.dll` | `BepInEx\core\0Harmony.dll` |

> 提示：`WarmSnow_Data\Managed\` 目录下没有 `Assembly-CSharp.dll` 的话，
> 找找有没有 `GameAssembly.dll`（那说明是 IL2CPP 版本，这套 BepInEx 5 写法不适用）。
> 暖雪目前是 Mono 版本，正常能找到 `Assembly-CSharp.dll`。
