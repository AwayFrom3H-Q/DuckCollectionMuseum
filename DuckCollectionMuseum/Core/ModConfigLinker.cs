using System;
using UnityEngine;
using DuckCollectionMuseum.Core;
using DuckCollectionMuseum.Utils;

namespace DuckCollectionMuseum.Core
{
    /// <summary>
    /// 将 ModConfigAPI 与本地 ModConfig.json 绑定
    /// </summary>
    public static class ModConfigLinker
    {
        private static bool initialized = false;
        private const string ModName = VersionInfo.Name;

        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            if (!ModConfigAPI.IsAvailable())
            {
                ModLogger.Warn("ModConfig 未检测到，将仅使用本地配置文件。");
                return;
            }

            ModLogger.Info("检测到 ModConfig，正在注册配置项...");

            // === 注册布尔配置项 ===
            ModConfigAPI.SafeAddBoolDropdownList(ModName, nameof(ModConfig.ShowButton),      L10n.GetLabel("显示价格按钮"),   ModConfig.ShowButton);

            // === 从 ModConfig API 载入已保存的设置（覆盖本地） ===
            SyncFromModConfigAPI();

            // === 注册事件回调 ===
            ModConfigAPI.SafeAddOnOptionsChangedDelegate(OnOptionChanged);

            ModLogger.Info("ModConfigLinker 初始化完成。");
        }

        /// <summary>
        /// 当游戏内配置被更改时回调
        /// </summary>
        private static void OnOptionChanged(string key)
        {
            if (!key.StartsWith(ModName)) return;

            string shortKey = key.Substring(ModName.Length + 1);
            ModLogger.Info($"ModConfig 选项已修改: {shortKey}");

            switch (shortKey)
            {
                case nameof(ModConfig.ShowButton):
                    ModConfig.ShowButton = ModConfigAPI.SafeLoad(ModName, shortKey, ModConfig.ShowButton);
                    break;
            }

            // 保存到本地 JSON
            ModConfig.Save();
        }

        /// <summary>
        /// 从 ModConfig API 载入（用于初始同步）
        /// </summary>
        public static void SyncFromModConfigAPI()
        {
            if (!ModConfigAPI.IsAvailable()) return;

            ModConfig.ShowButton = ModConfigAPI.SafeLoad(ModName, nameof(ModConfig.ShowButton), ModConfig.ShowButton);

            // 同步到本地文件
            ModConfig.Save();
        }
    }
}
