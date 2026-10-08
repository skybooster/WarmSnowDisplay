#!/usr/bin/env bash
# 在 Ubuntu 上编译暖雪数值显示 mod（需要已安装 mono-mcs）
# 用法：./build.sh        → 生成 bin/WarmSnowDisplay.dll
set -euo pipefail

cd "$(dirname "$0")"

OUT="bin/WarmSnowDisplay.dll"
mkdir -p bin

# 引用 Libs 下全部 dll（游戏本体 + BepInEx 5 核心库）
REFS=()
for dll in Libs/*.dll; do
  REFS+=("-r:$dll")
done

echo "编译 Plugin.cs → $OUT"
mcs -target:library -out:"$OUT" "${REFS[@]}" -langversion:latest Plugin.cs

echo "完成：$OUT"
