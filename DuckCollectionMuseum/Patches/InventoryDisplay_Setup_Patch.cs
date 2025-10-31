using Duckov.UI;
using HarmonyLib;
using ItemStatsSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DuckCollectionMuseum.Core;
using DuckCollectionMuseum.Utils;
using DuckCollectionMuseum.UI;


namespace DuckCollectionMuseum.Patches
{
    /// <summary>
    /// Harmony Patch 方案 - 用于 Windows/Linux 平台
    /// patch OnEnable 方法，避开 Setup 方法的多委托参数问题
    /// macOS 平台使用 InventoryDisplayMonitor 轮询方案
    /// </summary>
    [HarmonyPatch(typeof(InventoryDisplay), "OnEnable")]
    public static class InventoryDisplay_Setup_Patch
    {
        public static readonly string PatchTag = "CustomSortButtons";

        // Postfix 在 OnEnable 后执行
        static void Postfix(InventoryDisplay __instance)
        {
            try
            {
                // 延迟执行，等待 Setup 完成
                __instance.StartCoroutine(ExecuteAfterEnable(__instance));
            }
            catch (Exception ex)
            {
                ModLogger.Error("OnEnable Postfix 执行失败", ex);
            }
        }

        // 协程：延迟2帧执行，确保 Setup 完成
        static IEnumerator ExecuteAfterEnable(InventoryDisplay __instance)
        {
            // 等待2帧，确保 Setup 和初始化完成
            yield return null;
            yield return null;

            try
            {
                // 调用共享的按钮创建逻辑
                InventoryDisplayHelper.CreateCustomButtons(__instance);
            }
            catch (Exception ex)
            {
                ModLogger.Error("创建按钮失败", ex);
            }
        }
    }
}
