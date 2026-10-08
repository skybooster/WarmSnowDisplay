# 暖雪 · 局内数值显示 Mod

一个给《暖雪》（Warm Snow）用的 BepInEx 插件：进入一局后，在屏幕左上角实时显示角色当前数值（生命 / 蓝魂 / 攻击 / 防御 / 攻速 等）。默认按 **F9** 切换显示。

这个项目同时也是 **零基础写 Unity 游戏 Mod 的入门教程**。请从下往上读：先看「一、Mod 是怎么跑起来的」，再看「二、项目结构」，最后动手「三、环境准备」。

---

## 目录

1. [Mod 是怎么跑起来的（原理）](#一mod-是怎么跑起来的)
2. [项目结构](#二项目结构)
3. [代码逐段讲解](#三代码逐段讲解)
4. [环境准备与编译](#四环境准备与编译)
5. [安装与测试](#五安装与测试)
6. [进阶：自己找游戏里的字段](#六进阶自己找游戏里的字段)
7. [常见问题](#七常见问题)

---

## 一、Mod 是怎么跑起来的

暖雪是一款 **Unity** 游戏，游戏逻辑用 C# 写在一个叫 `Assembly-CSharp.dll` 的文件里。所谓「写 Mod」，本质就是：

> **往游戏进程里注入我们自己的 C# 代码，去读（或改）游戏里的数据。**

这需要解决两个问题：

1. **谁来加载我们的代码？** —— 答案是 **BepInEx**（一个 Unity 游戏的 Mod 加载器）。
2. **怎么改游戏原有逻辑？** —— 答案是 **Harmony**（一个运行时打补丁的库，BepInEx 自带）。

### BepInEx 在做什么

BepInEx 是 Unity 游戏 Mod 圈最通用的加载器。把它放进游戏目录后，游戏启动时会加载
`BepInEx\plugins\` 下的每一个 dll，并调用里面标记为「插件」的类。

一个「插件」就是一个继承 `BaseUnityPlugin`、带 `[BepInPlugin(...)]` 特性的类：

```csharp
[BepInPlugin("com.example.WarmSnowDisplay", "暖雪局内数值显示", "1.0.0")]
public class WarmSnowDisplayPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        // BepInEx 加载插件后会自动调用这里
    }
}
```

### Harmony 在做什么

只「读取」数据不需要 Harmony；但如果想让游戏在某个时机额外执行你的代码（例如每次攻击后打印伤害），
就要用到 Harmony 的「补丁」（Patch）——在游戏方法执行前/后插入你自己的方法。

```csharp
[HarmonyPatch(typeof(PlayerAnimControl), nameof(PlayerAnimControl.Update))]
[HarmonyPostfix]   // 在 Update 执行完之后调用
static void AfterUpdate(PlayerAnimControl __instance) { /* 你的代码 */ }
```

本项目目前只读取数据，所以代码里没有真正的 Patch，只保留了
`Harmony.CreateAndPatchAll(...)` 这个标准注册写法。

### 本 Mod 的完整数据流

```
游戏运行
  └─ BepInEx 加载 plugins\WarmSnowDisplay.dll
       └─ 调用 WarmSnowDisplayPlugin.Awake()
            └─ 每帧调用 Update()     ← 处理 F9 快捷键
            └─ 每帧调用 OnGUI()      ← 画数值悬浮窗
                 └─ 读 PlayerAnimControl.instance.playerParameter.HP 等属性
```

---

## 二、项目结构

```
WarmSnowDisplay/
├── Plugin.cs                  # 唯一的源代码：插件主体
├── WarmSnowDisplay.csproj     # Visual Studio / MSBuild 工程文件
├── build.sh                   # Ubuntu 一键编译脚本（mcs/mono）
├── Libs/                      # 编译用到的 dll（需自己复制，见 Libs/README.md）
│   └── README.md
├── README.md                  # 本文件
└── .gitignore
```

---

## 三、代码逐段讲解

打开 [Plugin.cs](./Plugin.cs)，对照下面看。

### 1. 插件声明

```csharp
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class WarmSnowDisplayPlugin : BaseUnityPlugin
```

- `[BepInPlugin]` 的三个参数：**GUID**（全球唯一 ID，建议用 `com.你的名字.模组名`）、显示名、版本号。
- 每个插件一个 GUID，重复会冲突。

### 2. 配置文件

```csharp
showOverlay = Config.Bind("显示", "显示悬浮窗", true, "是否在局内显示数值悬浮窗");
toggleKey   = Config.Bind("显示", "显示开关快捷键", new KeyboardShortcut(KeyCode.F9), "...");
```

BepInEx 会自动在 `BepInEx\config\` 下生成一个 `.cfg` 文件，玩家可以改它而不必重新编译。
这就是为什么用 `Config.Bind` 而不是写死常量。

### 3. 读取游戏数据（重点）

```csharp
var player = PlayerAnimControl.instance;      // 玩家控制器（游戏里的单例）
var pp = player.playerParameter;              // 玩家的属性对象
pp.HP          // 当前生命
pp.MAX_HP      // 生命上限
pp.ATK         // 总攻击 = 近战 + 飞剑
pp.ATK_MEELE   // 近战攻击
pp.ATK_BLADEBOLT // 飞剑攻击
pp.DEFENSE     // 防御
player.Souls   // 蓝魂
player.RedSouls // 红魂
```

`PlayerAnimControl`、`playerParameter` 这些名字都来自游戏自己的代码，
我们只是「引用游戏 dll 后直接用」，就像调用普通类库一样。

### 4. 判断「是不是在局内」

```csharp
if (player.playerParameter.PLAYER_SECT == Sect.None)
    return;
```

玩家没选流派（没真正开局）时 `PLAYER_SECT` 是 `Sect.None`，此时不画悬浮窗。

### 5. 画 UI（OnGUI）

```csharp
private void OnGUI()
{
    // GUI.Box 画一个半透明黑底，GUI.Label 画文字
}
```

`OnGUI` 是 Unity 的即时模式 UI（IMGUI）回调，每帧都会调用，
是写小工具类 Mod 最省事的方式——不需要任何 UI 资源文件。

---

## 四、环境准备与编译

> **本机（Ubuntu + Steam/Proton）专属说明**
>
> 暖雪是 Windows 游戏，在 Ubuntu 上通过 **Proton** 运行。游戏进程是 Windows 进程，
> 所以：
> - BepInEx 装 **win_x64 版**（不要装 linux 版）；
> - 编译直接在 Ubuntu 上用 Mono 完成（见下方「第 3 步」）；
> - 必须给 Steam 加一条启动项（见下方第 1 步末尾）。
>
> 本仓库默认的 Steam 游戏目录（snap 版 Steam）：
> ```
> ~/snap/steam/common/.local/share/Steam/steamapps/common/WarmSnow
> ```
> 传统版 Steam 一般是 `~/.local/share/Steam/steamapps/common/WarmSnow`，用 `find ~ -maxdepth 8 -type d -name WarmSnow` 能找到。

### 第 1 步：安装 BepInEx 5 到游戏目录

1. 去 BepInEx 官方发布页下载 **BepInEx 5** 的 **Windows x64** 版：
   https://github.com/BepInEx/BepInEx/releases/latest
   （下载 `BepInEx_win_x64_5.x.x.zip`，**不要**下 6.0 的 pre 版本，也不要下 linux 版）
2. 解压到游戏根目录（Ubuntu：`.../steamapps/common/WarmSnow/`；Windows：`...\steamapps\common\WarmSnow\`），
   让 `winhttp.dll`、`doorstop_config.ini`、`BepInEx\` 和游戏 exe 在同一层。
3. 启动一次游戏再退出——BepInEx 会自动生成 `BepInEx\plugins\`、`BepInEx\config\` 等目录，
   并且 `BepInEx\LogOutput.log` 里会出现 BepInEx 的启动日志，说明安装成功。

> **Ubuntu/Proton 额外一步（重要）**：Wine 自带的 winhttp 会优先于游戏的 `winhttp.dll`，
> 导致 BepInEx 不加载。请在 Steam 里给暖雪设置启动项：
>
> ```
> WINEDLLOVERRIDES="winhttp=n,b" %command%
> ```
>
> 设置方法：Steam 库 → 右键「暖雪」→ 属性 → 通用 → 启动选项。

> 为什么用 5 而不是 6？BepInEx 5 更简单、资料最多，绝大多数 Unity 游戏 Mod 教程都用 5。

### 第 2 步：复制依赖 dll

按 [Libs/README.md](./Libs/README.md) 把游戏和 BepInEx 的 dll 复制到 `Libs/`。
（Ubuntu 上游戏 dll 在 `WarmSnow/WarmSnow_Data/Managed/`，BepInEx dll 在 `WarmSnow/BepInEx/core/`。）

### 第 3 步：编译

**在 Ubuntu 上（推荐，本仓库已提供脚本）**：先装 Mono 编译器，然后跑 `build.sh`：

```bash
sudo apt install mono-mcs          # 如果没装过
./build.sh                          # 生成 bin/WarmSnowDisplay.dll
```

**在 Windows 上**：任选一种：

- **Visual Studio**：双击 `WarmSnowDisplay.csproj` 打开，选 `Release` 生成，产物在 `bin\Release\WarmSnowDisplay.dll`。
- **Rider**：直接打开 csproj。
- **MSBuild 命令行**（装了 .NET Framework 4.8）：

```bat
msbuild WarmSnowDisplay.csproj /p:Configuration=Release
```

> 也可以用 `dotnet build` 吗？不建议——这个 csproj 是老式（非 SDK 风格）的 .NET Framework 工程，
> `dotnet build` 对它支持不好，请用 Visual Studio / Rider / MSBuild（Windows）或 `build.sh`（Ubuntu）。

### 第 4 步：安装到游戏并测试

1. 把编译出的 `WarmSnowDisplay.dll` 复制到游戏目录下的 `BepInEx\plugins\`。
2. 启动游戏（Ubuntu 记得先设好上面的启动项），进一局。左上角应该出现数值悬浮窗，按 F9 可开关。
3. 若没显示，打开游戏目录下的 `BepInEx\LogOutput.log` 搜索 `WarmSnowDisplay` 看报错。

---

## 五、安装与测试

快速清单：

| 步骤 | 操作 |
|------|------|
| 1 | 装 BepInEx 5（win_x64）到游戏根目录，启动一次游戏 |
| 2 | Ubuntu/Proton 用户：Steam 启动项加 `WINEDLLOVERRIDES="winhttp=n,b" %command%` |
| 3 | 把 `WarmSnowDisplay.dll` 放进游戏目录 `BepInEx\plugins\` |
| 4 | 进游戏开局，看左上角悬浮窗，F9 切换 |

验证要点：
- 大厅/主菜单里**不应该**显示（因为没开局，`PLAYER_SECT == Sect.None`）。
- 进局后显示，且受伤时「生命」会变、花蓝魂时「蓝魂」会变。
- 游戏目录 `BepInEx\config\WarmSnowDisplay.cfg` 里可以改默认显示状态和快捷键。

---

## 六、进阶：自己找游戏里的字段

写 Mod 最核心、也最花时间的技能是：**从游戏 dll 里找出「哪个类、哪个字段」存了我要的数据**。
本项目的字段就是这么挖出来的。方法如下：

### 方法 A：dnSpy（推荐，图形界面）

1. 下载 dnSpy（开源 .NET 反编译器）：https://github.com/dnSpy/dnSpy/releases
2. 用 dnSpy 打开 `WarmSnow_Data\Managed\Assembly-CSharp.dll`。
3. 左侧能看到全部类。搜索 `PlayerAnimControl`，点开能看到它所有字段、属性、方法，
   还能反编译出 C# 源码（近似）。

### 方法 B：命令行反编译（本项目用的方法，Linux 也适用）

游戏文件里带 `monodis` / `ikdasm`（Mono 自带）或 Windows 上的 `ildasm`：

```bash
# 列出所有类型
monodis --typedef Assembly-CSharp.dll > typedefs.txt
# 列出所有字段（字段格式：编号: 类型 字段名: 访问级别）
monodis --fields Assembly-CSharp.dll > fields.txt
# 完整 IL 反汇编（用 ikdasm 更稳）
ikdasm Assembly-CSharp.dll > full.il
```

然后 `grep` 关键词，例如 `grep -i hp fields.txt`、`grep "PlayerAnimControl" typedefs.txt`。

### 实战：我是怎么找到生命/攻击字段的

1. `grep PlayerAnimControl` 找到玩家控制器类。
2. 看到它有个字段 `playerParameter`，类型是 `PlayerParameter`。
3. `PlayerParameter` 继承 `Parameter`，`Parameter` 里就是一堆基础属性：
   `hp`、`base_max_hp`、`base_atk_meele`、`base_atk_bladeBolt`、`base_defense`……
4. 再看属性（property）getter，发现游戏已经把「基础 + 装备 + 加成」算好了：
   `HP`、`MAX_HP`、`ATK`、`ATK_MEELE`、`ATK_BLADEBOLT`、`DEFENSE` 等。
5. 于是代码里直接 `playerParameter.HP` 就行。

### 方法 C：运行时反射（适合快速探索）

不知道字段名时，可以在插件里用反射把某个对象的所有字段名+值打出来：

```csharp
var pp = PlayerAnimControl.instance.playerParameter;
foreach (var f in pp.GetType().GetFields(
    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
    System.Reflection.BindingFlags.Instance))
{
    Logger.LogInfo($"{f.Name} = {f.GetValue(pp)}");
}
```

三种方法配合使用，基本能挖出任何你想要的数值。

---

## 七、常见问题

**Q：进游戏后 mod 没反应？**
先看 `BepInEx\LogOutput.log`。搜 `WarmSnowDisplay` 或 `[Error`。
最常见原因是 BepInEx 版本装错（装了 6.x），或 dll 没放进 `plugins`。

**Q：游戏更新后字段读不到 / 报 MissingFieldException？**
游戏版本更新可能改了类名或字段名。用「方法 A/B」重新反编译确认字段是否还在。
这几乎是所有 BepInEx Mod 的日常维护工作。

**Q：悬浮窗文字太小 / 颜色不好看？**
改 [Plugin.cs](./Plugin.cs) 里 `labelStyle` 的 `fontSize` 和 `textColor`，或者加进 `Config.Bind` 做成可配置。

**Q：想显示更多数值（比如经验、怒意）？**
用「方法 A/B」在 `Parameter` / `PlayerParameter` / `PlayerAnimControl` 里继续找，
然后照着 `Plugin.cs` 加一行即可。

**Q：想改游戏数值（而不仅是显示）？**
那需要 Harmony Patch + 给属性赋值。参考本 README 开头的 Harmony 示例，
把读取改成写入（例如 `playerParameter.ATK = 9999`）。注意：改单机存档数据一般没问题，
但请勿用于联机/竞技内容。

---

## 许可

本项目代码可自由学习、修改、分发（游戏本身的 dll 版权归游戏开发商，请勿随仓库分发）。
