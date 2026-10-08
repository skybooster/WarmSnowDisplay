# Warm Snow · In-run Stats Display

A BepInEx plugin for *Warm Snow* (暖雪): once you're in a run, it shows your current stats in the top-left corner of the screen. Press **F9** to toggle the overlay.

This is just a simple mod.

## Features

- Real-time display: HP, Souls / Red Souls, Attack (melee + flying swords), Defense, Attack Speed, Move Speed, Ignore Defense
- Shows the damage **increase percentage** for Fire / Ice / Poison / Thunder
- Display toggle and hotkey can be changed in the BepInEx config file

## If you'd rather not build it yourself

Just extract the zip directly into the game root folder.

Ubuntu users must add this Steam launch option:

```
WINEDLLOVERRIDES="winhttp=n,b" %command%
```

Windows users can simply start the game.

## Want to build it yourself? See below.

## Installation

1. Download the **Windows x64** build of [BepInEx 5](https://github.com/BepInEx/BepInEx/releases/latest)
   (`BepInEx_win_x64_5.x.x.zip`; do not use the 6.0 pre-release),
   and extract it into the game root folder so that `winhttp.dll`, `doorstop_config.ini` and `BepInEx\` sit next to the game exe.
2. Put the compiled `WarmSnowDisplay.dll` into `BepInEx\plugins\` inside the game folder.
3. Start the game and begin a run; the overlay appears in the top-left corner. Press F9 to toggle it.

> **Ubuntu users playing through Proton**: the game process is a Windows process, so install the win_x64 build of BepInEx as well.
> You must also set a launch option for Warm Snow in Steam, otherwise Wine won't load BepInEx:
>
> ```
> WINEDLLOVERRIDES="winhttp=n,b" %command%
> ```
>
> Where to set it: Steam Library → right-click "Warm Snow" → Properties → General → Launch options.

## Building

First copy the dependency DLLs into `Libs/` (see [Libs/README.md](./Libs/README.md) for the list).

- **Ubuntu**:

  ```bash
  sudo apt install mono-mcs   # if mono-mcs is not installed
  ./build.sh                  # produces bin/WarmSnowDisplay.dll
  ```

- **Windows**: open `WarmSnowDisplay.csproj` in Visual Studio and build in Release; the output is `bin\Release\WarmSnowDisplay.dll`.

## Configuration

After the plugin loads for the first time, a config file is generated at `BepInEx\config\WarmSnowDisplay.cfg`:

| Key | Meaning | Default |
|-----|---------|---------|
| 显示悬浮窗 | Whether to show the overlay | true |
| 显示开关快捷键 | Toggle show/hide hotkey | F9 |

## Project structure

```
WarmSnowDisplay/
├── Plugin.cs                  # Plugin source code
├── WarmSnowDisplay.csproj     # Visual Studio project
├── build.sh                   # Ubuntu build script
├── Libs/                      # Build dependency DLLs (not committed)
└── README.md
```

## License

This project's code may be freely used, modified and redistributed. The game's own DLLs are copyrighted by the game developer; do not redistribute them with this repository.

## Contact

yixuanliu@bluemailx.com
