using System;
using System.Collections.Generic;
using System.IO;

namespace ThomasJepp.SaintsRow
{
    public static class Utility
    {
        private static string GetRegistryEntry(string key, string value)
        {
            return (string)Microsoft.Win32.Registry.GetValue(key, value, null);
        }

        public static string GetGamePath(GameSteamID gameId)
        {
            int id = (int)gameId;

            var keys = new string[]
            {
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App {0}",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App {0}",
            };

            foreach (var key in keys)
            {
                var path = GetRegistryEntry(string.Format(key, id), "InstallLocation");
                if (path != null && Directory.Exists(path))
                {
                    return path;
                }
            }

            string steamPath = GetRegistryEntry(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath");
            if (steamPath == null)
                steamPath = GetRegistryEntry(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath");

            if (steamPath != null)
            {
                string gamePath = null;

                switch (gameId)
                {
                    case GameSteamID.SaintsRow2:
                        gamePath = Path.Combine(steamPath, "steamapps", "common", "saints row 2");
                        break;
                    case GameSteamID.SaintsRowTheThird:
                        gamePath = Path.Combine(steamPath, "steamapps", "common", "saints row the third");
                        break;
                    case GameSteamID.SaintsRowIV:
                        gamePath = Path.Combine(steamPath, "steamapps", "common", "Saints Row IV");
                        break;
                    case GameSteamID.SaintsRowGatOutOfHell:
                        gamePath = Path.Combine(steamPath, "steamapps", "common", "Project Gat");
                        break;
                }
                

                if (Directory.Exists(gamePath))
                {
                    return gamePath;
                }
            }

            return null;
        }
    }
}
