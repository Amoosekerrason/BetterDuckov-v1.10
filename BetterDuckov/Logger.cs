using System;
using UnityEngine;

namespace bigInventory
{
    internal class Logger
    {
        private static string ModName => ModBehaviour.modName;

        public static void Log(string message)
        {
            Debug.Log($"[{DateTime.Now:HH:mm:ss}][{ModName}]: {message}");
        }
        public static void Error(string message)
        {
            Debug.LogError($"> !!! <[{DateTime.Now:HH:mm:ss}][{ModName}]: {message}");
        }

        public static void Warn(string message)
        {
            Debug.LogWarning($">!!!!!<[{DateTime.Now:HH:mm:ss}][{ModName}]: {message}");
        }
    }
}
