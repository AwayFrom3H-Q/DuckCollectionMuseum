using DuckCollectionMuseum.Core;
using DuckCollectionMuseum.Patches;
using DuckCollectionMuseum.UI;
using DuckCollectionMuseum.Utils;
using UnityEngine;

namespace DuckCollectionMuseum
{

    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public AddText addText = new();

        // 根据平台选择不同的方案
        private HarmonyHelper? harmonyHelper;
        private bool useMacPollingMode = false;

        void Awake()
        {
        }
        void OnEnable()
        {
            ModConfig.Load();
            ModConfigLinker.Init();

            // 检测平台
            useMacPollingMode = Application.platform == RuntimePlatform.OSXPlayer;
            ModLogger.Info($"模组已启用 - 平台: {Application.platform}, 使用轮询模式: {useMacPollingMode}");

            addText.Enable();

            if (useMacPollingMode)
            {
                // macOS: 使用轮询监听器（避免 Harmony 兼容性问题）
                MacInventoryDisplayMonitor.Initialize();
                ModLogger.Info("已启用 macOS 轮询模式");
            }
            else
            {
                // Windows/Linux: 使用 Harmony patch
                harmonyHelper = new HarmonyHelper("CustomSortButtons");
                harmonyHelper.OnEnable();
                ModLogger.Info("已启用 Windows Harmony 模式");
            }
        }

        void OnDestroy()
        {
        }

        void OnDisable()
        {
            addText.Disable();

            if (useMacPollingMode)
            {
                // 关闭 macOS 监听器
                MacInventoryDisplayMonitor.Shutdown();
            }
            else
            {
                // 关闭 Harmony
                harmonyHelper?.OnDisable();
            }

            ModConfig.Save();
            ModLogger.Info("Mod 已禁用，配置已保存");
        }
    }
}
