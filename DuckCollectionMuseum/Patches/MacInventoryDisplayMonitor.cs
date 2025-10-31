using Duckov.UI;
using ItemStatsSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DuckCollectionMuseum.Core;
using DuckCollectionMuseum.Utils;
using DuckCollectionMuseum.UI;

namespace DuckCollectionMuseum.Patches
{
    /// <summary>
    /// 轮询监听方案 - 用于 macOS 平台
    /// 通过 MonoBehaviour Update 轮询检测 InventoryDisplay 实例
    /// 完全避开 Harmony patch 的兼容性问题
    /// Windows/Linux 平台使用 Harmony 方案（见 InventoryDisplay_Setup_Patch）
    /// </summary>
    public class MacInventoryDisplayMonitor : MonoBehaviour
    {
        private static MacInventoryDisplayMonitor? _instance;
        private HashSet<int> _processedInstances = new HashSet<int>();
        private float _checkInterval = 0.5f; // 每0.5秒检查一次
        private float _lastCheckTime;

        public static void Initialize()
        {
            if (_instance != null) return;

            var go = new GameObject("DuckCollectionMuseum_Monitor");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<MacInventoryDisplayMonitor>();

            ModLogger.Info("MacInventoryDisplayMonitor 已初始化（轮询模式）");
        }

        public static void Shutdown()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
                _instance = null;
                ModLogger.Info("MacInventoryDisplayMonitor 已关闭");
            }
        }

        void Update()
        {
            if (Time.time - _lastCheckTime < _checkInterval) return;
            _lastCheckTime = Time.time;

            try
            {
                // 查找所有激活的 InventoryDisplay 实例
                var displays = FindObjectsOfType<InventoryDisplay>();

                foreach (var display in displays)
                {
                    if (display == null || !display.gameObject.activeInHierarchy)
                        continue;

                    int instanceId = display.GetInstanceID();

                    // 如果已经处理过，跳过
                    if (_processedInstances.Contains(instanceId))
                        continue;

                    // 标记为已处理
                    _processedInstances.Add(instanceId);

                    // 延迟处理，等待 Setup 完成
                    StartCoroutine(ProcessInventoryDisplay(display, instanceId));
                }

                // 清理已销毁实例的记录
                _processedInstances.RemoveWhere(id =>
                {
                    foreach (var display in displays)
                    {
                        if (display != null && display.GetInstanceID() == id)
                            return false;
                    }
                    return true;
                });
            }
            catch (Exception ex)
            {
                ModLogger.Error("MacInventoryDisplayMonitor.Update 异常", ex);
            }
        }

        IEnumerator ProcessInventoryDisplay(InventoryDisplay display, int instanceId)
        {
            // 等待2帧，确保 Setup 完成
            yield return null;
            yield return null;

            try
            {
                if (display == null || !display.gameObject.activeInHierarchy)
                {
                    _processedInstances.Remove(instanceId);
                    yield break;
                }

                // 调用共享的按钮创建逻辑
                InventoryDisplayHelper.CreateCustomButtons(display);
            }
            catch (Exception ex)
            {
                ModLogger.Error($"处理 InventoryDisplay (ID:{instanceId}) 失败", ex);
            }
        }
    }
}
