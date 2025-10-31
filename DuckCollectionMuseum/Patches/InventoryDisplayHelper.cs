using Duckov.UI;
using ItemStatsSystem;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DuckCollectionMuseum.Core;
using DuckCollectionMuseum.Utils;
using DuckCollectionMuseum.UI;
using Unity.VisualScripting;

namespace DuckCollectionMuseum.Patches
{
    /// <summary>
    /// InventoryDisplay 按钮创建的共享逻辑
    /// 供 Harmony 和轮询两种方案复用
    /// </summary>
    public static class InventoryDisplayHelper
    {
        /// <summary>
        /// 为 InventoryDisplay 实例创建自定义按钮
        /// </summary>
        public static void CreateCustomButtons(InventoryDisplay instance)
        {
            try
            {
                if (Buttons.Visibility.All(v => v == false))
                {
                    ModLogger.Info("所有按钮均被隐藏，跳过按钮创建");
                    return;
                }

                // var ItemSlotsDisplay = ReflectionHelper.GetFieldValue<Transform>(instance, "ItemSlotsDisplay");
                // if (ItemSlotsDisplay != null)
                // {
                //     DisplayComponentChildName(ItemSlotsDisplay);
                // }
                // else
                // {
                //     ModLogger.Info("NGY-01-----------------");
                // }

                // 获取 sortButton 按钮
                var sortBtn = ReflectionHelper.GetFieldValue<Button>(instance, "sortButton");
                if (sortBtn == null)
                {
                    ModLogger.Warn("sortButton 未找到，跳过按钮创建");
                    return;
                }

                // 如果原整理按钮被隐藏，则跳过，否则会在宠物背包上面创建按钮
                if (!sortBtn.gameObject.activeSelf || sortBtn.GetComponent<CanvasRenderer>().GetAlpha() == 0)
                {
                    ModLogger.Info("原整理按钮被隐藏，跳过创建新按钮");
                    return;
                }

                var container = instance.transform.Find($"{VersionInfo.Name}_Container");
                if (container != null)
                {
                    // 如果已有按钮容器，先删除
                    ModLogger.Info("已有按钮容器，先删除");
                    UnityEngine.Object.Destroy(container.gameObject);
                }

                var sortRect = sortBtn.GetComponent<RectTransform>();
                var parentRect = sortRect.parent.GetComponent<RectTransform>();
                var grandParent = parentRect.parent.GetComponent<RectTransform>();
                // var grandGrandParent = grandParent.parent.GetComponent<RectTransform>();
                // var grandGrandGrandParent = grandGrandParent.parent.GetComponent<RectTransform>();
                // var grandGrandGrandGrandParent = grandGrandGrandParent.parent.GetComponent<RectTransform>();
                // var grandGrandgrandGrandGrandParent = grandGrandGrandGrandParent.parent.GetComponent<RectTransform>();

                if (IsPlayerInventoryDisplay(parentRect))
                {
                    ModLogger.Info("确认是玩家背包，不创建按钮");
                    return;
                }
                ModLogger.Info("仓库界面背包，按钮创建");
                PrintPagesControlStructure(parentRect);

                // 创建自定义按钮
                CreateButtonContainer(instance, sortBtn, sortRect, parentRect, grandParent);

                ModLogger.Info("✅ 成功创建自定义按钮");
            }
            catch (Exception ex)
            {
                ModLogger.Error("创建按钮失败 ", ex);
            }
        }

        /// <summary>
        /// 创建按钮容器并添加自定义按钮
        /// </summary>
        static void CreateButtonContainer(InventoryDisplay instance, Button sortBtn, RectTransform sortRect, RectTransform parentRect, RectTransform grandParent)
        {
            // 创建新容器
            var newRowGO = new GameObject($"{VersionInfo.Name}_Container", typeof(RectTransform));
            var newRowRT = newRowGO.GetComponent<RectTransform>();
            newRowRT.SetParent(grandParent, false);

            // 插入新行
            newRowRT.SetSiblingIndex(parentRect.GetSiblingIndex());

            // 设置布局属性
            var hGroup = newRowGO.AddComponent<HorizontalLayoutGroup>();
            hGroup.spacing = 80f;
            hGroup.childAlignment = TextAnchor.UpperRight;
            hGroup.childForceExpandWidth = false;
            hGroup.childForceExpandHeight = false;

            // 让新行与按钮行宽度对齐
            newRowRT.anchorMin = parentRect.anchorMin;
            newRowRT.anchorMax = parentRect.anchorMax;
            newRowRT.pivot = parentRect.pivot;
            newRowRT.sizeDelta = parentRect.sizeDelta;

            // 设置偏移量
            var height = sortRect.rect.height;
            newRowRT.anchoredPosition = parentRect.anchoredPosition - new Vector2(0, height + 8f);

            var ValueLabel = L10n.GetLabel("按价值");
            var WeightLabel = L10n.GetLabel("按重量");
            var RatioLabel = L10n.GetLabel("按价重比");

            var inventory = instance.Target;
            var sb = new Buttons(newRowRT, sortBtn, inventory);
        }

        /// <summary>
        /// 调试用：打印组件子节点名称
        /// </summary>
        public static void DisplayComponentChildName(Transform curComponent)
        {
            ModLogger.Info(curComponent.name);
            int childCount = 0;
            while (childCount < curComponent.childCount)
            {
                ModLogger.Info("- " + curComponent.GetChild(childCount).name);
                childCount++;
            }
        }

        /// <summary>
        /// 判断是否为玩家背包界面，用于避免在玩家背包创建按钮，玩家背包界面包含 "StoreAllBtn" 按钮
        /// todo: 如果有现成的API可以直接检查，调用对应API
        /// </summary>
        static bool IsPlayerInventoryDisplay(Transform transform)
        {
            int childCount = 0;
            bool found = false;
            if (transform.name != "TitleBar (1)") return false;
            while (childCount < transform.childCount)
            {
                if (transform.GetChild(childCount).name == "StoreAllBtn")
                {
                    found = true;
                    break;
                }
                childCount++;
            }
            return found;
        }

        static void PrintPagesControlStructure(Transform t)
        {
            var nextTarget = t.parent.GetComponent<RectTransform>();
            while (nextTarget != null && nextTarget != t && nextTarget.name != "Scroll View")
            {
                nextTarget = nextTarget.parent.GetComponent<RectTransform>();
            }
            if (nextTarget == null || nextTarget.name != "Scroll View")
            {
                ModLogger.Info("未找到 Content 组件");
                return;
            }
            nextTarget = nextTarget.parent.GetComponent<RectTransform>();
            ModLogger.Info("找到 Content 组件，开始打印结构");
            // 找到 Content 组件，开始打印结构
            var childCount = 0;
            var child = nextTarget.GetChild(childCount);
            while (childCount < nextTarget.childCount)
            {
                Debug.Log($"Qbz: 当前子节点 {child.name}");
                if (child.name == "PagesControl")
                {
                    ModLogger.Info("找到 PagesControl 组件，开始打印结构");
                    break;
                }
                child = nextTarget.GetChild(childCount);
                childCount++;
            }
            if (child.name != "PagesControl")
            {
                ModLogger.Info("未找到 PagesControl 组件");
                return;
            }
            DisplayComponentChildName(child);
            for (int i = 0; i < child.childCount; i++)
            {
                DisplayComponentChildName(child.GetChild(i)); // LeftButton, PageText, RightButton
                for (int j = 0; j < child.GetChild(i).childCount; ++j)
                    DisplayComponentChildName(child.GetChild(i).GetChild(j)); // Button
            }
        }
    }
}
