using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using CmdLine;

using ThomasJepp.SaintsRow.Fonts;
using ThomasJepp.SaintsRow.Localization;

namespace ThomasJepp.SaintsRow.ExtractFont
{
    [CommandLineArguments(Program = "ThomasJepp.SaintsRow.ExtractFont", Title = "Saints Row Font Extractor", Description = "Unpacks font files (vf3_pc files). Supports Saints Row: The Third, Saints Row IV and Saints Row: Gat out of Hell.")]
    internal class Options
    {
        [CommandLineParameter(Name = "source", ParameterIndex = 1, Required = true, Description = "The source file to process.")]
        public string Source { get; set; }

        [CommandLineParameter(Name = "charlist", ParameterIndex = 2, Required = true, Description = "The character list file to use to decode the font.")]
        public string Charlist { get; set; }

        [CommandLineParameter(Name = "output", ParameterIndex = 3, Required = false, Description = "If the action is \"tovf3\" or \"toxml\", this is used as the output. If not specified, the new file will be placed in the same directory as the source file. This is not used for \"update\".")]
        public string Output { get; set; }

    }

    class Program
    {
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

            if (options.Output == null)
                options.Output = Path.ChangeExtension(options.Source, ".xml");

            Vf3ToXml(options);
        }

        static char DecodeChar(FontHeader header, Dictionary<char, char> charMap, int charIndex)
        {
            int charValue = (header.FirstAscii + charIndex);
            char fontChar = (char)charValue;

            char actualChar;

            if (charValue > 0x100)
            {
                if (!charMap.ContainsKey(fontChar))
                {
                    //throw new Exception("Couldn't find char in charlist to decode!");
                    actualChar = fontChar;
                }
                else
                {
                    actualChar = charMap[fontChar];
                }
            }
            else
            {
                if (!charMap.ContainsKey(fontChar))
                {
                    actualChar = fontChar;
                }
                else
                {
                    actualChar = charMap[fontChar];
                }
            }

            return actualChar;
        }

        static void Vf3ToXml(Options options)
        {
            Dictionary<char, char> charMap = null;

            using (Stream cStream = File.OpenRead(options.Charlist))
            {
                charMap = LanguageUtility.GetDecodeCharMapFromStream(cStream);
            }

            using (Stream s = File.OpenRead(options.Source))
            {
                FontFile font = new FontFile(s);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.NewLineChars = "\r\n";

                using (XmlWriter xml = XmlWriter.Create(options.Output, settings))
                {
                    xml.WriteStartDocument();
                    xml.WriteStartElement("font");
                    xml.WriteAttributeString("id", font.Header.ID.ToString());
                    xml.WriteAttributeString("version", font.Header.Version.ToString());
                    xml.WriteAttributeString("first_ascii", font.Header.FirstAscii.ToString());
                    xml.WriteAttributeString("width", font.Header.Width.ToString());
                    xml.WriteAttributeString("height", font.Header.Height.ToString());
                    xml.WriteAttributeString("render_height", font.Header.RenderHeight.ToString());
                    xml.WriteAttributeString("baseline_offset", font.Header.BaselineOffset.ToString());
                    xml.WriteAttributeString("character_spacing", font.Header.CharacterSpacing.ToString());
                    xml.WriteAttributeString("vertical_offset", font.Header.VerticalOffset.ToString());
                    xml.WriteAttributeString("peg_name", font.Header.PegName);
                    xml.WriteAttributeString("bitmap_name", font.Header.BitmapName);

                    xml.WriteStartElement("characters");

                    for (int i = 0; i < font.Characters.Count; i++)
                    {
                        FontCharacter c = font.Characters[i];
                        int u = font.U[i];
                        int v = font.V[i];

                        xml.WriteStartElement("character");

                        int charValue = font.Header.FirstAscii + i;
                        char actualChar = DecodeChar(font.Header, charMap, i);

                        xml.WriteAttributeString("spacing", c.Spacing.ToString());
                        xml.WriteAttributeString("byte_width", c.ByteWidth.ToString());
                        xml.WriteAttributeString("offset", c.Offset.ToString());
                        xml.WriteAttributeString("kerning_entry", c.KerningEntry.ToString());
                        xml.WriteAttributeString("user_data", c.UserData.ToString());
                        xml.WriteAttributeString("u", u.ToString());
                        xml.WriteAttributeString("v", v.ToString());
                        xml.WriteAttributeString("char_value", charValue.ToString());
                        xml.WriteAttributeString("actual_char", actualChar.ToString());
                        xml.WriteEndElement(); // character
                    }

                    xml.WriteEndElement(); // characters

                    xml.WriteStartElement("kerning_pairs");

                    for (int i = 0; i < font.KerningPairs.Count; i++)
                    {
                        FontKerningPair pair = font.KerningPairs[i];

                        xml.WriteStartElement("kerning_pair");

                        char char1 = DecodeChar(font.Header, charMap, pair.Char1);
                        char char2 = DecodeChar(font.Header, charMap, pair.Char2);

                        xml.WriteAttributeString("char1", char1.ToString());
                        xml.WriteAttributeString("char2", char2.ToString());
                        xml.WriteAttributeString("offset", pair.Offset.ToString());
                        xml.WriteAttributeString("padding", pair.Padding.ToString());

                        xml.WriteEndElement(); // kerning_pair
                    }

                    xml.WriteEndElement(); // kerning_pairs

                    xml.WriteEndElement(); // font
                    xml.WriteEndDocument();
                }
            }
        }
    }
}
