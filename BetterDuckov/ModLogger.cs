using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace bigInventory
{
    internal static class ModLogger
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdhandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsolemode(IntPtr hConsoleHandle, int mode);
        private static string ModName => ModBehaviour.modName;

        internal static readonly bool _testLvlSwitch = false;

        public enum Level { Regular, Test }

        public static void InitWithConsole()
        {
            // 依賴測試開關
            if (!_testLvlSwitch) return;
            AllocConsole();
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            });
            Console.WriteLine(Format("Console Ready"));
        }

        private static bool ShouldLog(Level lvl)
        {
            return lvl == Level.Regular || (lvl == Level.Test && _testLvlSwitch);
        }

        private static void LogToConsole(string msg)
        {
            if (!_testLvlSwitch) return;
            Console.WriteLine(msg);
        }

        private static string Format(string message, string subMod = null)
        {
            string sub = subMod != null ? $"[{subMod}]" : "";
            return $"[{DateTime.Now:HH:mm:ss}][{ModName}]{sub}: {message}";
        }
        public static void Log(Level lvl, string message, string subMod = null)
        {
            if (!ShouldLog(lvl)) return;
            string msg = Format(message, subMod);
            Debug.Log(msg);
            LogToConsole(msg);
        }

        public static void Error(Level lvl, string message, string subMod = null)
        {
            if (!ShouldLog(lvl)) return;
            string msg = Format(message, subMod);

            Debug.LogError("> !!! <" + msg);
            LogToConsole("> !!! <" + msg);

        }

        public static void Warn(Level lvl, string message, string subMod = null)
        {
            if (!ShouldLog(lvl)) return;
            string msg = Format(message, subMod);

            Debug.LogWarning("> !!!!! <" + msg);
            LogToConsole("> !!!!! <" + msg);
        }
    }
}
