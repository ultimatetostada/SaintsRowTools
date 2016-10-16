using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using CmdLine;

using ThomasJepp.SaintsRow.Bitmaps.Version13;
using ThomasJepp.SaintsRow.Fonts;
using ThomasJepp.SaintsRow.Localization;

namespace ThomasJepp.SaintsRow.ExtractFont
{
    [CommandLineArguments(Program = "ThomasJepp.SaintsRow.DumpFontCharacters", Title = "Saints Row Font Character Dumper", Description = "Rips font characters from a font bitmap to individual images. Supports Saints Row: The Third, Saints Row IV and Saints Row: Gat out of Hell.")]
    internal class Options
    {
        [CommandLineParameter(Name = "source", ParameterIndex = 1, Required = true, Description = "The source font file to process. The peg/vbm for the font should be located in the same directory.")]
        public string Source { get; set; }

        [CommandLineParameter(Name = "charlist", ParameterIndex = 2, Required = true, Description = "The charlist file to use. This will be used as a best-guess match.")]
        public string Charmap { get; set; }

        [CommandLineParameter(Name = "output", ParameterIndex = 3, Required = false, Description = "If not specified, the files will be placed in a new directory called \"output\".")]
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
                options.Output = "output";

            if (!Directory.Exists(options.Output))
                Directory.CreateDirectory(options.Output);

            FontFile font = null;

            using (Stream s = File.OpenRead(options.Source))
            {
                font = new FontFile(s);
            }

            Dictionary<char, char> charMap = new Dictionary<char, char>();
            using (Stream s = File.OpenRead(options.Charmap))
            {
                charMap = LanguageUtility.GetDecodeCharMapFromStream(s);
            }

            string indicatedPegPath = Path.Combine(Path.GetDirectoryName(options.Source), font.Header.BitmapName);

            string[] pegExtensions = new string[]
            {
                ".cpeg_pc",
                ".cvbm_pc"
            };

            string[] gpegExtensions = new string[]
            {
                ".gpeg_pc",
                ".gvbm_pc"
            };

            bool foundPeg = false;
            string pegPath = null;
            string gpegPath = null;
            for (int i = 0; i < pegExtensions.Length; i++)
            {
                string pegExtension = pegExtensions[i];
                string gpegExtension = gpegExtensions[i];
                string candidatePath = Path.ChangeExtension(indicatedPegPath, pegExtension);

                if (File.Exists(candidatePath))
                {
                    foundPeg = true;
                    pegPath = candidatePath;
                    gpegPath = Path.ChangeExtension(pegPath, gpegExtension);
                    break;
                }
            }

            if (!foundPeg)
            {
                Console.WriteLine("Couldn't find {0}! Extension may be \".cpeg_pc\" or \".cvbm_pc\".", indicatedPegPath);
                return;
            }


            Bitmap fontBitmap = null;


            using (Stream pegStream = File.OpenRead(pegPath))
            {
                PegFile peg = new PegFile(pegStream);

                PegEntry entry = null;

                foreach (PegEntry e in peg.Entries)
                {
                    if (e.Filename == font.Header.BitmapName)
                    {
                        entry = e;
                        break;
                    }
                }

                if (entry == null)
                {
                    Console.WriteLine("Couldn't find bitmap {0} in font peg!", font.Header.BitmapName);
                    return;
                }

                byte[] bitmapData = null;    

                using (Stream gpegStream = File.OpenRead(gpegPath))
                {
                    bitmapData = entry.GetData(gpegStream);
                }

                byte[] uncompressed = null;

                switch (entry.Data.BitmapFormat)
                {
                    case PegBitmapFormat.D3DFMT_DXT3:
                        uncompressed = ManagedSquish.Squish.DecompressImage(bitmapData, entry.Data.Width, entry.Data.Height, ManagedSquish.SquishFlags.Dxt3);
                        break;

                    case PegBitmapFormat.D3DFMT_DXT5:
                        uncompressed = ManagedSquish.Squish.DecompressImage(bitmapData, entry.Data.Width, entry.Data.Height, ManagedSquish.SquishFlags.Dxt5);
                        break;

                    default: throw new Exception();
                }

                fontBitmap = new Bitmap(entry.Data.Width, entry.Data.Height, PixelFormat.Format32bppArgb);
                BitmapData data = fontBitmap.LockBits(new Rectangle(0, 0, fontBitmap.Width, fontBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                Marshal.Copy(uncompressed, 0, data.Scan0, uncompressed.Length);
                fontBitmap.UnlockBits(data);
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(options.Output, "out.txt")))
            {
                for (int i = 0; i < font.Characters.Count; i++)
                {
                    FontCharacter c = font.Characters[i];
                    int u = font.U[i];
                    int v = font.V[i];
                    int charValue = font.Header.FirstAscii + i;

                    if (c.ByteWidth == 0)
                        continue;

                    char actualChar = '\0';
                    char rawChar = (char)charValue;
                    if (charMap.ContainsKey(rawChar))
                    {
                        actualChar = charMap[rawChar];
                        sw.WriteLine("{0} \"{1}\"", charValue, actualChar);
                    }
                    else
                    {
                        sw.WriteLine("{0} \"\"", charValue);
                    }

                    

                    using (Bitmap bm = new Bitmap(c.ByteWidth, font.Header.RenderHeight))
                    {
                        using (Graphics g = Graphics.FromImage(bm))
                        {
                            g.Clear(Color.Black);
                            g.DrawImage(fontBitmap, 0, 0, new Rectangle(u, v, c.ByteWidth, font.Header.RenderHeight), GraphicsUnit.Pixel);
                            g.Flush();
                        }
                        string bmName = String.Format("{0}.png", charValue);
                        string bmPath = Path.Combine(options.Output, bmName);
                        bm.Save(bmPath, ImageFormat.Png);
                    }

                }
            }
        }
    }
}
