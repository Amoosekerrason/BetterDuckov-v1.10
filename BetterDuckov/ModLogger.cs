using HarmonyLib;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleCP(uint wCodePageID);

        private static string ModName => ModBehaviour.modName;

        internal static readonly bool _testLvlSwitch = true;

        public enum Level { Regular, Test }

        public static void InitWithConsole()
        {
            // 依賴_testLvlSwitch 開關
            if (!_testLvlSwitch) return;
            AllocConsole();
            SetConsoleOutputCP(65001);
            SetConsoleCP(65001);
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

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
            Task.Run(() =>
            {
                Console.WriteLine(msg);
            });
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
