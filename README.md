# 暖雪 · 局内数值显示

> English version: [README.en.md](./README.en.md)

给《暖雪》（Warm Snow）用的 BepInEx 插件：进入一局后，在屏幕左上角实时显示角色当前数值。默认按 **F9** 开关悬浮窗。

这只是一个简单的mod。

## 功能

- 实时显示：生命、蓝魂 / 红魂、攻击（近战 + 飞剑）、防御、攻速、移速、无视防御
- 显示火 / 冰 / 毒 / 雷四种属性伤害的**提升百分比**
- 显示开关与快捷键可通过 BepInEx 配置文件调整

## 如果大家比较懒的话也可以直接将压缩包解压缩到游戏根目录位置

Ubuntu用户须添加 Steam 启动项
```
WINEDLLOVERRIDES="winhttp=n,b" %command%
```

windows用户直接开始游戏即可。

## 想自己动手的看下面的内容

## 安装

1. 下载 [BepInEx 5](https://github.com/BepInEx/BepInEx/releases/latest) 的 **Windows x64** 版
   （`BepInEx_win_x64_5.x.x.zip`，不要下 6.0 pre 版本），
   解压到游戏根目录，使 `winhttp.dll`、`doorstop_config.ini`、`BepInEx\` 与游戏 exe 同一层。
2. 把编译好的 `WarmSnowDisplay.dll` 放进游戏目录下的 `BepInEx\plugins\`。
3. 启动游戏进一局，左上角出现悬浮窗；按 F9 开关。

> **Ubuntu用户在Proton兼容层游玩时**：游戏进程是 Windows 进程，BepInEx 同样装 win_x64 版。
> 另外需在 Steam 给暖雪设置启动项，否则 Wine 不会加载 BepInEx：
>
> ```
> WINEDLLOVERRIDES="winhttp=n,b" %command%
> ```
>
> 设置位置：Steam 库 → 右键「暖雪」→ 属性 → 通用 → 启动选项。

## 编译

先把依赖 dll 复制到 `Libs/`（清单见 [Libs/README.md](./Libs/README.md)）。

- **Ubuntu**：

  ```bash
  sudo apt install mono-mcs   # 如未安装 mono-mcs
  ./build.sh                  # 生成 bin/WarmSnowDisplay.dll
  ```

- **Windows**：用 Visual Studio 打开 `WarmSnowDisplay.csproj`，Release 生成，产物在 `bin\Release\WarmSnowDisplay.dll`。

## 配置

插件首次加载后，会在 `BepInEx\config\WarmSnowDisplay.cfg` 生成配置：

| 键 | 含义 | 默认 |
|----|------|------|
| 显示悬浮窗 | 是否显示 | true |
| 显示开关快捷键 | 切换显示/隐藏 | F9 |

## 该插件目录结构

```
WarmSnowDisplay/
├── Plugin.cs                  # 插件源代码
├── WarmSnowDisplay.csproj     # Visual Studio 工程
├── build.sh                   # Ubuntu 编译脚本
├── Libs/                      # 编译依赖 dll（不提交仓库）
└── README.md
```

## 许可

本项目代码可自由使用、修改、分发。游戏自带的 dll 版权归游戏开发商所有，请勿随仓库分发。

## 有问题联系我

yixuanliu@bluemailx.com