using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace WarmSnowDisplay
{
    /// <summary>
    /// 暖雪「局内数值显示」mod。
    /// 进入一局选过流派开始游戏后，在屏幕左上角显示角色当前数值。
    /// 默认按 F9 显示/隐藏悬浮窗。
    /// </summary>
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class WarmSnowDisplayPlugin : BaseUnityPlugin
    {
        private ConfigEntry<bool> showOverlay;
        private ConfigEntry<KeyboardShortcut> toggleKey;

        // 两种字体分别画「汉字」和「数字/符号」：
        // 汉字用宋体（SimSun），数字用 Times New Roman（罗马新时代）
        private GUIStyle textStyle;
        private GUIStyle numberStyle;
        private float lineHeight;

        /// <summary>
        /// 插件加载时由 BepInEx 调用一次，做初始化工作。
        /// </summary>
        private void Awake()
        {
            // 1. 读取配置文件（BepInEx/config/WarmSnowDisplay.cfg），没有则自动生成
            showOverlay = Config.Bind("显示", "显示悬浮窗", true, "是否在局内显示数值悬浮窗");
            toggleKey = Config.Bind("显示", "显示开关快捷键", new KeyboardShortcut(KeyCode.F9),
                "按下该键切换悬浮窗显示/隐藏");

            // 2. 注册 Harmony 补丁。
            //    本 mod 只读取数据、不改游戏逻辑，所以没有实际的补丁方法，
            //    但这是所有 BepInEx 插件加载游戏逻辑的标准写法。
            Harmony.CreateAndPatchAll(typeof(WarmSnowDisplayPlugin), PluginInfo.PLUGIN_GUID);

            Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} 已加载");
        }

        /// <summary>
        /// 每帧调用一次。这里只处理快捷键。
        /// </summary>
        private void Update()
        {
            if (toggleKey.Value.IsDown())
            {
                showOverlay.Value = !showOverlay.Value;
            }
        }

        /// <summary>
        /// Unity 的 IMGUI 渲染回调，每帧多次调用，用于画悬浮窗。
        /// 这是写简单 mod UI 最方便的方式。
        /// </summary>
        private void OnGUI()
        {
            if (!showOverlay.Value)
                return;

            var player = PlayerAnimControl.instance;
            if (player == null || player.playerParameter == null)
                return;

            // 没有选过流派（还没真正进入一局）时不显示，避免在大厅/菜单里出现
            if (player.playerParameter.PLAYER_SECT == Sect.None)
                return;

            var pp = player.playerParameter;

            EnsureStyles();

            string[] lines =
            {
                $"生命 {pp.HP:0} / {pp.MAX_HP:0}",
                $"蓝魂 {player.Souls}    红魂 {player.RedSouls}",
                $"攻击 {pp.ATK:0}（近战 {pp.ATK_MEELE:0} + 飞剑 {pp.ATK_BLADEBOLT:0}）",
                $"防御 {pp.DEFENSE:0}",
                $"攻速 {pp.ATTACK_SPEED:0.00}",
                $"移速 {pp.RUN_SPEED:0.00}",
                $"无视防御 {pp.IGNORE_DEFENSE:0}",
                $"火伤 {Pct(pp.FIRE_EXTRA_DAMAGE_RATE)}  冰伤 {Pct(pp.ICE_EXTRA_DAMAGE_RATE)}",
                $"毒伤 {Pct(pp.POISON_EXTRA_DAMAGE_RATE)}  雷伤 {Pct(pp.THUNDER_EXTRA_DAMAGE_RATE)}",
            };

            // 计算整块文本的宽度/高度，再画半透明底
            float maxWidth = 0f;
            foreach (var line in lines)
                maxWidth = Mathf.Max(maxWidth, MeasureLine(line));

            var rect = new Rect(10, 10, maxWidth + 16, lines.Length * lineHeight + 12);

            // 半透明黑底，让文字在任何场景下都可读
            GUI.color = new Color(0f, 0f, 0f, 0.5f);
            GUI.Box(rect, GUIContent.none);
            GUI.color = Color.white;

            float y = rect.y + 6;
            foreach (var line in lines)
            {
                DrawLine(line, rect.x + 8, y);
                y += lineHeight;
            }
        }

        /// <summary>
        /// 懒加载两种字体与样式：汉字宋体、数字/英文 Times New Roman。
        /// </summary>
        private void EnsureStyles()
        {
            if (textStyle != null)
                return;

            textStyle = MakeStyle(Font.CreateDynamicFontFromOSFont("SimSun", 24));
            numberStyle = MakeStyle(Font.CreateDynamicFontFromOSFont("Times New Roman", 24));
            lineHeight = Mathf.Max(textStyle.lineHeight, numberStyle.lineHeight) + 2f;
        }

        private static GUIStyle MakeStyle(Font font)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
            };
            style.normal.textColor = Color.white;
            if (font != null)
                style.font = font;
            return style;
        }

        /// <summary>计算一行文本（分段后）的总宽度。</summary>
        private float MeasureLine(string line)
        {
            float w = 0f;
            int start = 0;
            for (int i = 0; i <= line.Length; i++)
            {
                if (i == line.Length || IsCjk(line[i]) != IsCjk(line[start]))
                {
                    string seg = line.Substring(start, i - start);
                    var style = IsCjk(line[start]) ? textStyle : numberStyle;
                    w += style.CalcSize(new GUIContent(seg)).x;
                    start = i;
                }
            }
            return w;
        }

        /// <summary>绘制一行文本：汉字用宋体，数字/符号用 Times New Roman。</summary>
        private void DrawLine(string line, float x, float y)
        {
            int start = 0;
            for (int i = 0; i <= line.Length; i++)
            {
                if (i == line.Length || IsCjk(line[i]) != IsCjk(line[start]))
                {
                    string seg = line.Substring(start, i - start);
                    var style = IsCjk(line[start]) ? textStyle : numberStyle;
                    var content = new GUIContent(seg);
                    float w = style.CalcSize(content).x;
                    GUI.Label(new Rect(x, y, w + 2, lineHeight), content, style);
                    x += w;
                    start = i;
                }
            }
        }

        /// <summary>是否为汉字/全角字符（这些用宋体显示）。</summary>
        private static bool IsCjk(char c)
        {
            return (c >= '\u2E80' && c <= '\u9FFF')   // CJK 部首 + 汉字
                || (c >= '\uF900' && c <= '\uFAFF')    // CJK 兼容汉字
                || (c >= '\uFF00' && c <= '\uFFEF');   // 全角字符（全角括号等）
        }

        /// <summary>
        /// 把属性伤害倍率（1.0 为无加成）转成「伤害提升百分比」字符串，例如 +30%、-5%。
        /// </summary>
        private static string Pct(float rate)
        {
            float pct = (rate - 1f) * 100f;
            return $"{pct:+0.#;-0.#;0}%";
        }
    }

    internal static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.example.WarmSnowDisplay";
        public const string PLUGIN_NAME = "暖雪局内数值显示";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
