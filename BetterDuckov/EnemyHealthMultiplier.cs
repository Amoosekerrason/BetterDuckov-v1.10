using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace bigInventory
{
    internal static class EnemyHealthMultiplier
    {
        private static Harmony Harmony => ModBehaviour.harmony;

        private static ModConfig Config => BigInventoryConfigManager.Config;

        internal static readonly HashSet<Health> processedEnemies = new HashSet<Health>();

        [HarmonyPatch(typeof(Health), "MaxHealth", MethodType.Getter)]
        public class Patch_Health_MaxHealth
        {
            private static void Postfix(Health __instance, ref float __result)
            {
                if (!Config.EnableEnemyHealthMultiply) return;
                var owner = __instance.GetComponent<CharacterMainControl>();

                if (owner != null && owner != LevelManager.Instance.MainCharacter)
                {
                    float before = __result;
                    __result *= Config.EnemyHealthMultiplier;
                    float after = __result;
                    Logger.Log($"MaxHealth from {before} to {after}");
                    if (!processedEnemies.Contains(__instance))
                    {
                        __instance.CurrentHealth = __result;
                        processedEnemies.Add(__instance);

                    }
                }
            }
        }
    }
}
