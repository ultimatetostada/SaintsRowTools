using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ThomasJepp.SaintsRow.Packfiles;

namespace ThomasJepp.SaintsRow.Localization
{
    public static class LanguageUtility
    {
        public static string GetLanguageCode(Language language)
        {
            switch (language)
            {
                case Language.English: return "US";
                case Language.Spanish: return "ES";
                case Language.Italian: return "IT";
                case Language.Japanese: return "JP";
                case Language.German: return "DE";
                case Language.French: return "FR";
                case Language.Dutch: return "NL";
                case Language.Swedish: return "SE";
                case Language.Danish: return "DK";
                case Language.Czech: return "CZ";
                case Language.Polish: return "PL";
                case Language.Korean: return "SK";
                case Language.Russian: return "RU";
                case Language.Chinese: return "CH";
            }

            return "";
        }

        public static Dictionary<char, char> GetCharMap(string gameDir, Language language)
        {
            string filename = String.Format("charlist_{0}.dat", GetLanguageCode(language).ToLowerInvariant());

            string miscvpp = Path.Combine(gameDir, "packfiles", "pc", "cache", "misc.vpp_pc");

            List<string> lines = new List<string>();

            using (Stream packfileStream = File.OpenRead(miscvpp))
            {
                using (IPackfile packfile = Packfile.FromStream(packfileStream, false))
                {
                    foreach (var file in packfile.Files)
                    {
                        if (file.Name == filename)
                        {
                            using (Stream stream = file.GetStream())
                            {
                                StreamReader sr = new StreamReader(stream);
                                while (!sr.EndOfStream)
                                {
                                    string line = sr.ReadLine();

                                    if (line.StartsWith("//"))
                                        continue;

                                    if (line.StartsWith("count="))
                                        continue;

                                    lines.Add(line);
                                }
                            }
                        }
                    }
                }
            }

            Dictionary<char, char> map = new Dictionary<char, char>();

            int nextSlot = 0x100;
            foreach (string line in lines)
            {
                int value = 0;
                if (int.TryParse(line, out value))
                {
                    if (value > 0x100)
                    {
                        map.Add((char)nextSlot, (char)value);
                        nextSlot++;
                    }
                    else
                    {
                        map.Add((char)value, (char)value);
                    }
                }
            }

            return map;
        }
    }
}
