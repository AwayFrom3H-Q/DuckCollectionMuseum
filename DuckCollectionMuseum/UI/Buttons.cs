using DuckCollectionMuseum.Core;
using DuckCollectionMuseum.Utils;
using ItemStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DuckCollectionMuseum.UI
{
    internal class Buttons
    {
        private static readonly (string Label, Comparison<Item> Comparison)[] myButtonList =
        {
            ("Museum", (a, b) => b.GetTotalRawValue().CompareTo(a.GetTotalRawValue())),
        };

        public static bool[] Visibility = {
            ModConfig.ShowButton,
        };
        public SortedDictionary<int, ButtonEntry> buttonDict;

        public Buttons(RectTransform containerRT, Button templateButton, Inventory inventory)
        {
            System.Diagnostics.Debug.Assert(Visibility.Length == myButtonList.Length, "Visibility 数组长度与 myButtonList 不匹配！");

            var cnt = myButtonList.Length;
            buttonDict = new SortedDictionary<int, ButtonEntry>(
                Enumerable.Range(0, cnt)
                    .Select(i =>
                    {
                        if (!Visibility[i])
                        {
                            ModLogger.Info($" {myButtonList[i].Label} 已隐藏");
                            return (Index: i, Entry: (ButtonEntry?)null);
                        }

                        var entry = new ButtonEntry(
                            containerRT,
                            templateButton,
                            inventory,
                            myButtonList[i].Label,
                            myButtonList[i].Comparison
                        );

                        return (Index: i, Entry: entry);
                    })
                    .Where(x => x.Entry != null) // 去掉隐藏的项
                    .ToDictionary(x => x.Index, x => x.Entry!)
            );
            ModLogger.Info($"成功新增一行 {myButtonList.Length} 个按钮。");
        }


        public void ToggleVisibility(int index)
        {
            if (index < 0 || index >= myButtonList.Length)
            {
                ModLogger.Error($"尝试设置按钮可见性时索引越界: {index}，有效范围为 0 到 {myButtonList.Length - 1}");
                return;
            }
            Visibility[index] = !Visibility[index];
            ModLogger.Info($"按钮索引 {index} 的可见性已更改为 {Visibility[index]}");
        }
    }
}
