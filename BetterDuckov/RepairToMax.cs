using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Duckov.UI;
using ItemStatsSystem;

namespace bigInventory
{
    internal class RepairToMax : MonoBehaviour
    {
        private static RepairToMax instance;
        internal static RepairToMax Instance => instance;

        void OnEnable()
        {
            ModLogger.Log(ModLogger.Level.Test, "RepairToMax attached", "RepairToMax");
            ItemRepairView.OnRepaireOptionDone += OnRepairDone;
        }
        void OnDisable()
        {
            ItemRepairView.OnRepaireOptionDone -= OnRepairDone;
            ModLogger.Log(ModLogger.Level.Test, "RepairToMax disattached", "RepairToMax");
        }

        internal static void Install(GameObject obj)
        {
            if (instance == null)
            {
                instance = obj.AddComponent<RepairToMax>();
            }
        }

        private static void OnRepairDone()
        {
            Item selectItem = ItemUIUtilities.SelectedItem;
            if (selectItem == null) return;
            SetItemDuraToMax(selectItem);
        }

        private static void SetItemDuraToMax(Item item)
        {
            item.Durability = item.MaxDurability;
        }
    }
}
