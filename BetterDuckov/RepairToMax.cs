using Duckov.UI;
using HarmonyLib;
using ItemStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace bigInventory
{
    internal class RepairToMax : MonoBehaviour
    {
        private static RepairToMax instance;
        internal static RepairToMax Instance => instance;

        void OnEnable()
        {
            if (BigInventoryConfigManager.Config.EnableRepairToMax == false) return;
            ModLogger.Log(ModLogger.Level.Test, "RepairToMax attached", "RepairToMax");
            DeleOnActiveViewChanged.GetRequiredFields();
            ItemRepairView.OnRepaireOptionDone += SingleRepairToMax;
            ItemUIUtilities.OnSelectionChanged += MakeItemRepairable;

            View.OnActiveViewChanged += DeleOnActiveViewChanged.BindRepairUIEvents;
        }
        void OnDisable()
        {
            if (BigInventoryConfigManager.Config.EnableRepairToMax == false) return;
            ItemRepairView.OnRepaireOptionDone -= SingleRepairToMax;
            ItemUIUtilities.OnSelectionChanged -= MakeItemRepairable;

            View.OnActiveViewChanged -= DeleOnActiveViewChanged.BindRepairUIEvents;
            if (DeleOnActiveViewChanged.willLoseDuraText)
            {
                DeleOnActiveViewChanged.willLoseDuraText.OnPreRenderText -= DeleOnActiveViewChanged.SetWillLossDuraTextToEmpty;
            }
            if (DeleOnActiveViewChanged.repairAllBtn)
            {
                DeleOnActiveViewChanged.repairAllBtn.onClick.RemoveListener(DeleOnActiveViewChanged.UA_RepairAllToMax);

            }
            ModLogger.Log(ModLogger.Level.Test, "RepairToMax disattached", "RepairToMax");
        }


        internal static void Install(GameObject obj)
        {
            if (instance == null)
            {
                instance = obj.AddComponent<RepairToMax>();
            }
        }

        private static void SingleRepairToMax()
        {
            Item selectItem = ItemUIUtilities.SelectedItem;
            if (selectItem == null) return;
            SetItemDuraToMax(selectItem);
        }

        private static void SetItemDuraToMax(Item item)

        {
            item.DurabilityLoss = 0f;
            item.Durability = item.MaxDurability;
        }

        private static void MakeItemRepairable()
        {
            View activatedView = View.ActiveView;
            if (activatedView is ItemRepairView)
            {
                Item selectedItem = ItemUIUtilities.SelectedItem;
                if (selectedItem == null || selectedItem.DurabilityLoss == 0f) return;
                try
                {
                    HackRepairCheck(selectedItem);
                }
                catch (Exception e)
                {
                    ModLogger.Error(ModLogger.Level.Regular, $"OnItemSelectionChange got error {e}", "RepairToMax");
                }
            }
        }

        // 讓耐久通過修理判定
        private static void HackRepairCheck(Item item)
        {
            if (item.Durability >= item.MaxDurabilityWithLoss)
            {
                item.Durability = Mathf.Max(0, item.MaxDurabilityWithLoss - 1);
            }
        }

        private static class DeleOnActiveViewChanged
        {
            private static FieldInfo f_RepairAllPanel;
            private static FieldInfo f_WillLoseDuraText;
            private static FieldInfo f_Btn;
            private static MethodInfo m_RefreshSelectedItemInfo;

            private static ItemRepairView itemRepairView;
            internal static TextMeshProUGUI willLoseDuraText;
            internal static ItemRepair_RepairAllPanel repairAllPanel;
            internal static Button repairAllBtn;

            internal static UnityAction UA_RepairAllToMax = new UnityAction(RepairAllToMax);



            internal static void GetRequiredFields()
            {
                ModLogger.Log(ModLogger.Level.Test, "Entered GetRequiredFields", "RepairToMax");
                try
                {
                    var handledType = typeof(ItemRepairView);
                    f_RepairAllPanel = handledType.GetField("repairAllPanel", BindingFlags.NonPublic | BindingFlags.Instance);
                    f_WillLoseDuraText = handledType.GetField("willLoseDurabilityText", BindingFlags.NonPublic | BindingFlags.Instance);
                    m_RefreshSelectedItemInfo = handledType.GetMethod("RefreshSelectedItemInfo", BindingFlags.NonPublic | BindingFlags.Instance);
                    f_Btn = typeof(ItemRepair_RepairAllPanel).GetField("button", BindingFlags.NonPublic | BindingFlags.Instance);

                }
                catch (Exception e)
                {
                    ModLogger.Error(ModLogger.Level.Test, $"GetRequiredFields got wrong: {e}", "RepairToMax");

                }
            }

            internal static void BindRepairUIEvents()
            {
                try
                {
                    ModLogger.Log(ModLogger.Level.Test, "Entered DewIt", "RepairToMax");

                    View activatedView = View.ActiveView;
                    if (activatedView == null) return;

                    if (activatedView is ItemRepairView)
                    {
                        itemRepairView = (ItemRepairView)activatedView;
                        repairAllPanel = (ItemRepair_RepairAllPanel)f_RepairAllPanel.GetValue(itemRepairView);
                        TextMeshProUGUI textMPGUI = (TextMeshProUGUI)f_WillLoseDuraText.GetValue(itemRepairView);
                        Button btn = (Button)f_Btn.GetValue(repairAllPanel);


                        if (willLoseDuraText == null || textMPGUI != willLoseDuraText)
                        {
                            willLoseDuraText = textMPGUI;
                            willLoseDuraText.OnPreRenderText += SetWillLossDuraTextToEmpty;
                        }
                        if (repairAllBtn == null || btn != repairAllBtn)
                        {
                            repairAllBtn = btn;
                            repairAllBtn.onClick.AddListener(UA_RepairAllToMax);
                        }
                        ModLogger.Log(ModLogger.Level.Test, "DoneIt", "RepairToMax");

                    }
                }
                catch (Exception e)
                {
                    ModLogger.Error(ModLogger.Level.Test, $"DewIt got wrong: {e}", "RepairToMax");
                }
            }


            internal static void RepairAllToMax()
            {
                Item selectedItem = ItemUIUtilities.SelectedItem;

                foreach (Item item in itemRepairView.GetAllEquippedItems())
                {
                    SetItemDuraToMax(item);
                }
                if (selectedItem != null)
                {
                    m_RefreshSelectedItemInfo.Invoke(itemRepairView, null);
                    ModLogger.Log(ModLogger.Level.Test, "m_RefreshSelectedItemInfo ed", "RepairToMax");

                }
            }


            internal static void SetWillLossDuraTextToEmpty(TMP_TextInfo text)
            {
                text.textComponent.text = "";
            }
        }

    }
}

