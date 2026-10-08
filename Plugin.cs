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

        private GUIStyle labelStyle;

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

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                };
                labelStyle.normal.textColor = Color.white;
            }

            string text =
                $"生命 {pp.HP:0} / {pp.MAX_HP:0}\n" +
                $"蓝魂 {player.Souls}    红魂 {player.RedSouls}\n" +
                $"攻击 {pp.ATK:0}（近战 {pp.ATK_MEELE:0} + 飞剑 {pp.ATK_BLADEBOLT:0}）\n" +
                $"防御 {pp.DEFENSE:0}\n" +
                $"攻速 {pp.ATTACK_SPEED:0.00}\n" +
                $"移速 {pp.RUN_SPEED:0.00}\n" +
                $"无视防御 {pp.IGNORE_DEFENSE:0}\n" +
                $"火伤 {Pct(pp.FIRE_EXTRA_DAMAGE_RATE)}  冰伤 {Pct(pp.ICE_EXTRA_DAMAGE_RATE)}\n" +
                $"毒伤 {Pct(pp.POISON_EXTRA_DAMAGE_RATE)}  雷伤 {Pct(pp.THUNDER_EXTRA_DAMAGE_RATE)}";

            var content = new GUIContent(text);
            Vector2 size = labelStyle.CalcSize(content);
            var rect = new Rect(10, 10, size.x + 16, size.y + 12);

            // 半透明黑底，让文字在任何场景下都可读
            GUI.color = new Color(0f, 0f, 0f, 0.5f);
            GUI.Box(rect, GUIContent.none);
            GUI.color = Color.white;

            GUI.Label(new Rect(rect.x + 8, rect.y + 6, size.x, size.y), text, labelStyle);
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
