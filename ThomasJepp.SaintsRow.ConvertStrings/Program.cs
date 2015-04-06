using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CmdLine;
using ThomasJepp.SaintsRow.GameInstances;
using ThomasJepp.SaintsRow.Localization;

namespace ThomasJepp.SaintsRow.ConvertStrings
{
    public static class Program
    {
        [CommandLineArguments(Program = "ThomasJepp.SaintsRow.ConvertStrings", Title = "Saints Row Localisation File Converter", Description = "Converts localization string files from the old .txt format to the new .xml format.")]
        internal class Options
        {
            [CommandLineParameter(Name = "input", ParameterIndex = 1, Required = true, Description = "The localisation file to extract.")]
            public string Input { get; set; }

            [CommandLineParameter(Name = "output", ParameterIndex = 2, Required = false, Description = "The output file to create. If not specified, the input filename will be used with the exension changed to \".xml\".")]
            public string Output { get; set; }
        }
        static void Main(string[] args)
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

            string outputFile = (options.Output != null) ? options.Output : Path.ChangeExtension(options.Input, ".xml");

            Language language;

            while (true)
            {
                Console.WriteLine("What language is this file?");
                Console.WriteLine("Valid options are:");
                string[] values = Enum.GetNames(typeof(Language));
                foreach (string value in values)
                    Console.WriteLine(" - {0}", value);
                Console.Write("Language: ");
                string languageString = Console.ReadLine();
                if (Enum.TryParse<Language>(languageString, out language))
                    break;
                Console.WriteLine("Unrecognised option.");
                Console.WriteLine();
            }

            Console.WriteLine("Selected language: {0}", language);

            IGameInstance instance;

            while (true)
            {
                Console.WriteLine("What game is this file from?");
                Console.WriteLine("Valid options are:");
                Console.WriteLine(" - sr2");
                Console.WriteLine(" - srtt");
                Console.WriteLine(" - sriv");
                Console.WriteLine(" - srgooh");
                Console.Write("Game: ");
                string gameString = Console.ReadLine();
                try
                {
                    instance = GameInstance.GetFromString(gameString);
                    break;
                }
                catch
                {
                }
                Console.WriteLine("Unrecognised option.");
                Console.WriteLine();
            }

            string[] lines = File.ReadAllLines(options.Input);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";

            using (XmlWriter xml = XmlTextWriter.Create(outputFile, settings))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("Strings");
                xml.WriteAttributeString("Language", language.ToString());
                xml.WriteAttributeString("Game", instance.Game.ToString());

                foreach (string line in lines)
                {
                    string[] pieces = line.Split(new char[] { ':' }, 2);
                    string key = pieces[0];
                    string value = pieces[1].Trim(' ', '"').Replace("\\n", "\n");

                    xml.WriteStartElement("String");

                    if (key.StartsWith("\"") && key.EndsWith("\""))
                    {
                        // key is a string
                        xml.WriteAttributeString("Name", key.Trim('"'));
                    }
                    else
                    {
                        // key is a hash
                        UInt32 hash = hash = UInt32.Parse(key); ;
                        xml.WriteAttributeString("Hash", hash.ToString("X8"));
                    }

                    xml.WriteString(value);
                    xml.WriteEndElement(); // String
                }

                xml.WriteEndElement();
                xml.WriteEndDocument();

                Console.WriteLine("Done.");
            }

#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
