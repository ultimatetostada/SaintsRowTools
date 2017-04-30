using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

using CmdLine;
using ThomasJepp.SaintsRow.GameInstances;
using ThomasJepp.SaintsRow.Localization;
using ThomasJepp.SaintsRow.Strings;

namespace ThomasJepp.SaintsRow.ExtractStrings
{
    public static class Program
    {
        [CommandLineArguments(Program = "ThomasJepp.SaintsRow.ExtractStrings", Title = "Saints Row Localisation File Extractor", Description = "Extracts Saints Row PC Localisation files. Supports Saints Row 2, Saints Row: The Third, Saints Row IV and Saints Row: Gat out of Hell.")]
        internal class Options
        {
            [CommandLineParameter(Name = "game", ParameterIndex = 1, Required = true, Description = "The game you wish to build a packfile for. Valid options are \"sr2\", \"srtt\", \"sriv\" and \"srgooh\".")]
            public string Game { get; set; }

            [CommandLineParameter(Name = "input", ParameterIndex = 2, Required = true, Description = "The localisation file to extract.")]
            public string Input { get; set; }

            [CommandLineParameter(Name = "output", ParameterIndex = 3, Required = false, Description = "The output file to create. If not specified, the input filename will be used with the exension changed to \".xml\".")]
            public string Output { get; set; }

            [CommandLineParameter(Command = "load_xtbls", Default = true, Required = false, Description = "Should XTBLs be loaded? Defaults to 'true'")]
            public bool LoadXtbls { get; set; }
        }

        public static void Main(string[] args)
        {
            Options options = null;

            try
            {
                options = CommandLine.Parse<Options>();
            }
            catch (CommandLineException exception)
            {
                Console.WriteLine(exception.ArgumentHelp.Message);
                Console.WriteLine();
                Console.WriteLine(exception.ArgumentHelp.GetHelpText(Console.BufferWidth));

#if DEBUG
                Console.ReadLine();
#endif
                return;
            }

            IGameInstance instance = GameInstance.GetFromString(options.Game);

            string filename = Path.GetFileNameWithoutExtension(options.Input);
            string languageCode = filename.Remove(0, filename.Length - 2);
            Language language = LanguageUtility.GetLanguageFromCode(languageCode);

            Dictionary<UInt32, string> hashLookup = new Dictionary<UInt32, string>();

            if (options.LoadXtbls)
            {
                Console.WriteLine("Loading XTBL files...");
                Dictionary<string, FileSearchResult> results = instance.SearchForFiles("*.xtbl");
                int i = 0;
                foreach (var pair in results)
                {
                    i++;
                    Console.WriteLine("[{0}/{1}] Loading xtbl: {2}", i, results.Count, pair.Key);

                    string xtbl = null;
                    using (StreamReader reader = new StreamReader(pair.Value.GetStream()))
                    {
                        xtbl = reader.ReadToEnd();
                    }
                    Regex regex = new Regex("<Name>(.*?)</Name>", RegexOptions.Compiled);
                    foreach (Match m in regex.Matches(xtbl))
                    {
                        uint hash = Hashes.CrcVolition(m.Groups[1].Value);
                        if (!hashLookup.ContainsKey(hash))
                            hashLookup.Add(hash, m.Groups[1].Value);
                    }
                }
            }

            string outputFile = (options.Output != null) ? options.Output : Path.ChangeExtension(options.Input, ".xml");

            Console.WriteLine("Extracting {0} to {1}...", options.Input, outputFile);

            StringFile stringFile = null;

            using (Stream stream = File.OpenRead(options.Input))
            {
                 stringFile = new StringFile(stream, language, instance);
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";

            Dictionary<string, string> stringsWithName = new Dictionary<string, string>();
            Dictionary<uint, string> stringsWithHash = new Dictionary<uint, string>();

            foreach (uint hash in stringFile.GetHashes())
            {
                string text = stringFile.GetString(hash);

                if (hashLookup.ContainsKey(hash))
                    stringsWithName.Add(hashLookup[hash], text);
                else
                    stringsWithHash.Add(hash, text);
            }

            

            using (XmlWriter xml = XmlTextWriter.Create(outputFile, settings))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("Strings");
                xml.WriteAttributeString("Language", language.ToString());
                xml.WriteAttributeString("Game", instance.Game.ToString());
                
                foreach (var pair in stringsWithName.OrderBy(x => x.Key))
                {
                    xml.WriteStartElement("String");

                    xml.WriteAttributeString("Name", pair.Key);
                    xml.WriteString(pair.Value);

                    xml.WriteEndElement(); // String
                }

                foreach (var pair in stringsWithHash)
                {
                    xml.WriteStartElement("String");

                    xml.WriteAttributeString("Hash", pair.Key.ToString("X8"));
                    xml.WriteString(pair.Value);

                    xml.WriteEndElement(); // String
                    
                }

                xml.WriteEndElement(); // Strings
                xml.WriteEndDocument();
            }

            Console.WriteLine("Done.");
#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
