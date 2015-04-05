using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using CmdLine;
using ThomasJepp.SaintsRow.GameInstances;
using ThomasJepp.SaintsRow.Localization;
using ThomasJepp.SaintsRow.Strings;

namespace ThomasJepp.SaintsRow.BuildStrings
{
    public static class Program
    {
        [CommandLineArguments(Program = "ThomasJepp.SaintsRow.BuildStrings", Title = "Saints Row Localisation File Builder", Description = "Builds Saints Row PC Localisation files. Supports Saints Row 2, Saints Row: The Third and Saints Row IV.")]
        internal class Options
        {
            [CommandLineParameter(Name = "input", ParameterIndex = 1, Required = true, Description = "The file containing the language strings to use.")]
            public string Input { get; set; }

            [CommandLineParameter(Name = "output", ParameterIndex = 2, Required = false, Default = null, Description = "The output file to create. If not specified, the input filename will be used with the extension changed to \".le_strings\".")]
            public string Output { get; set; }
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

            string outputFile = options.Output != null ? options.Output : Path.ChangeExtension(options.Input, ".le_strings");

            Console.WriteLine("Packing {0} and creating {1}...", options.Input, outputFile);

            XDocument xml = null;

            using (Stream s = File.OpenRead(options.Input))
            {
                xml = XDocument.Load(s);
            }

            var stringsNode = xml.Descendants("Strings").First();
            string languageString = stringsNode.Attribute("Language").Value;
            string gameString = stringsNode.Attribute("Game").Value;

            Language language = LanguageUtility.GetLanguageFromCode(languageString);
            IGameInstance instance = GameInstance.GetFromString(gameString);

            var stringNodes = stringsNode.Descendants("String");

            UInt16 bucketCount = (UInt16)(stringNodes.Count() / 5);
            if (bucketCount < 32)
                bucketCount = 32;
            else if (bucketCount < 64)
                bucketCount = 64;
            else if (bucketCount < 128)
                bucketCount = 128;
            else if (bucketCount < 256)
                bucketCount = 256;
            else if (bucketCount < 512)
                bucketCount = 512;
            else 
                bucketCount = 1024;

            StringFile stringFile = new StringFile(bucketCount, language, instance);

            foreach (var stringNode in stringNodes)
            {
                uint hash;

                var nameAttribute = stringNode.Attribute("Name");
                if (nameAttribute != null)
                {
                    hash = Hashes.CrcVolition(nameAttribute.Value);
                }
                else
                {
                    hash = uint.Parse(stringNode.Attribute("Hash").Value);
                }


                string value = stringNode.Value;

                if (stringFile.ContainsKey(hash))
                {
                    Console.WriteLine("You are attempting to add a duplicate key to the strings file.");
                    Console.WriteLine("Name: \"{0}\", Hash: {1}, Value: {2}", nameAttribute != null ? nameAttribute.Value : "", hash, value);
                    Console.WriteLine("Other value: {0}", stringFile.GetString(hash));
                    return;
                }
                
                stringFile.AddString(hash, value);
            }

            using (Stream s = File.Create(outputFile))
            {
                stringFile.Save(s);
            }

            Console.WriteLine("Done.");
#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
