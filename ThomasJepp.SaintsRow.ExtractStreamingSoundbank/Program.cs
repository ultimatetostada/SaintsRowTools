using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using CmdLine;
using ThomasJepp.SaintsRow.Localization;
using ThomasJepp.SaintsRow.Soundbanks.Streaming;

namespace ThomasJepp.SaintsRow.ExtractStreamingSoundbank
{
    class Program
    {
        [CommandLineArguments(Program = "ThomasJepp.SaintsRow.ExtractStreamingSoundbank", Title = "Saints Row Streaming Soundbank Extractor", Description = "Extracts Saints Row PC Streaming Soundbanks (..._media.bnk_pc files). Supports Saints Row The Third, Saints Row IV and Saints Row Gat Out Of Hell.")]
        internal class Options
        {
            [CommandLineParameter(Name = "soundbank", ParameterIndex = 1, Required = true, Description = "The soundbank to unpack.")]
            public string Source { get; set; }

            [CommandLineParameter(Name = "output", ParameterIndex = 2, Required = false, Description = "The location to save the extracted data. If not specified, the packfile will be extracted to a new folder with the same name in the current directory.")]
            public string Output { get; set; }

            [CommandLineParameter(Command = "convert", Default = true, Description = @"Convert the audio into playble OGG files. Requires ww2ogg and revorb in the same directory. Valid options are ""true"" and ""false"". Defaults to ""true"".", Name = "Convert audio")]
            public bool ConvertAudio { get; set; }

            [CommandLineParameter(Command = "game", Default = "srgooh", Description = @"The game load the character list files from for decoding the subtitles. Valid options are ""srgooh"", ""sriv"", ""srtt"". Defaults to ""srgooh"".", Name = "Game")]
            public string Game { get; set; }
        }

        static string ExeLocation
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            }
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
            string ww2ogg = Path.Combine(ExeLocation, "ww2ogg.exe");
            string codebooks = Path.Combine(ExeLocation, "packed_codebooks_aoTuV_603.bin");
            string revorb = Path.Combine(ExeLocation, "revorb.exe");
            bool failedToFindConversionRequirements = false;
            if (options.ConvertAudio)
            {
                if (!File.Exists(ww2ogg))
                {
                    Console.WriteLine("Could not find ww2ogg.exe at:\n{0}", ww2ogg);
                    failedToFindConversionRequirements = true;
                }

                if (!File.Exists(codebooks))
                {
                    Console.WriteLine("Could not find packed_codebooks_aoTuV_603.bin at:\n{0}", codebooks);
                    failedToFindConversionRequirements = true;
                }

                if (!File.Exists(revorb))
                {
                    Console.WriteLine("Could not find revorb.exe at:\n{0}", revorb);
                    failedToFindConversionRequirements = true;
                }

                if (failedToFindConversionRequirements)
                {
                    Console.WriteLine("Can't convert audio.");
                }
            }

            Game game = Game.SaintsRowTheThird;

            switch (options.Game.ToLowerInvariant())
            {
                case "srtt": game = Game.SaintsRowTheThird; break;
                case "sriv": game = Game.SaintsRowIV; break;
                case "srgooh": game = Game.SaintsRowGatOutOfHell; break;
            }

            using (Stream stream = File.OpenRead(options.Source))
            {
                var bnk = new StreamingSoundbank(stream);

                string bnkName = Path.GetFileName(options.Source);

                string folderName = (options.Output != null) ? options.Output : "extracted-" + bnkName;

                Console.WriteLine("Extracting {0} to {1}.", options.Source, folderName);

                Directory.CreateDirectory(folderName);

                if (File.Exists(Path.Combine(folderName, String.Format("{0}.xml", bnkName))))
                    File.Delete(Path.Combine(folderName, String.Format("{0}.xml", bnkName)));

                using (Stream xmlStream = File.Create(Path.Combine(folderName, Path.ChangeExtension(bnkName, "xml"))))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = "\t";
                    settings.NewLineChars = "\r\n";

                    using (XmlWriter writer = XmlWriter.Create(xmlStream, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("soundbank");
                        writer.WriteAttributeString("wwiseId", bnk.Header.WwiseBankId.ToString());

                        //writer.WriteAttributeString()
                        int currentFile = 0;
                        foreach (SoundbankEntry entry in bnk.Files)
                        {
                            writer.WriteStartElement("file");
                            currentFile++;

                            writer.WriteAttributeString("id", entry.Info.FileId.ToString());

                            Console.Write("[{0}/{1}] Extracting audio... ", currentFile, bnk.Files.Count);
                            string audioFilename = String.Format("{0}_{1:D5}.wem", bnkName, currentFile);
                            using (Stream outputStream = File.Create(Path.Combine(folderName, audioFilename)))
                            {
                                using (Stream inputStream = entry.GetAudioStream())
                                {
                                    inputStream.CopyTo(outputStream);
                                }
                                outputStream.Flush();
                            }
                            Console.WriteLine("done.");
                            writer.WriteAttributeString("audio", audioFilename);

                            if (entry.Info.MetadataLength != 0)
                            {
                                Console.Write("[{0}/{1}] Extracting metadata... ", currentFile, bnk.Files.Count);
                                AudioMetadata metadata = null;
                                using (Stream metadataStream = entry.GetMetadataStream())
                                {
                                    writer.WriteStartElement("metadata");
                                    metadata = new AudioMetadata(metadataStream, Utility.GetGamePath(game));

                                    writer.WriteAttributeString("version", metadata.Header.Version.ToString());
                                    writer.WriteAttributeString("personaid", metadata.Header.PersonaID.ToString());
                                    writer.WriteAttributeString("voicelineid", metadata.Header.VoicelineID.ToString());
                                    writer.WriteAttributeString("wavlengthms", metadata.Header.WavLengthMs.ToString());

                                    if (metadata.LipsyncData != null && metadata.LipsyncData.Length > 0)
                                    {
                                        writer.WriteStartElement("lipsync");
                                        writer.WriteString(Convert.ToBase64String(metadata.LipsyncData));
                                        writer.WriteEndElement(); // lipsync
                                    }

                                    if (metadata.Header.SubtitleSize > 0)
                                    {
                                        writer.WriteStartElement("subtitles");
                                        writer.WriteAttributeString("version", metadata.SubtitleHeader.Version.ToString());

                                        foreach (var subtitle in metadata.Subtitles)
                                        {
                                            Language language = subtitle.Key;
                                            string text = subtitle.Value;

                                            writer.WriteStartElement("subtitle");
                                            writer.WriteAttributeString("language", language.ToString());
                                            writer.WriteString(text);
                                            writer.WriteEndElement(); // subtitle
                                        }
                                        writer.WriteEndElement(); // subtitles
                                    }

                                    writer.WriteEndElement(); // metadata
                                }


                                Console.WriteLine("done.");
                            }

                            writer.WriteEndElement(); // file
                        }

                        writer.WriteEndElement(); // soundbank
                        writer.WriteEndDocument();
                    }
                }

                if (options.ConvertAudio)
                {
                    if (failedToFindConversionRequirements)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Unable to convert extracted audio due to missing required files.");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Converting extracted audio...");
                        for (int i = 1; i <= bnk.Files.Count; i++)
                        {
                            Console.Write("[{0}/{1}] Converting audio... ", i, bnk.Files.Count);
                            string oggFilename = String.Format("{0}_{1:D5}.ogg", bnkName, i);
                            string oggPath = Path.Combine(folderName, oggFilename);

                            string audioFilename = String.Format("{0}_{1:D5}.wem", bnkName, i);
                            string audioPath = Path.Combine(folderName, audioFilename);

                            ProcessStartInfo ww2oggPsi = new ProcessStartInfo(ww2ogg, String.Format(@"--pcb ""{0}"" -o ""{1}"" ""{2}""", codebooks, oggPath, audioPath));
                            ww2oggPsi.WindowStyle = ProcessWindowStyle.Hidden;
                            ww2oggPsi.CreateNoWindow = true;
                            Process ww2oggP = Process.Start(ww2oggPsi);
                            ww2oggP.WaitForExit();
                            Console.Write("revorb... ");

                            ProcessStartInfo revorbPsi = new ProcessStartInfo(revorb, String.Format(@"""{0}""", oggPath));
                            revorbPsi.WindowStyle = ProcessWindowStyle.Hidden;
                            revorbPsi.CreateNoWindow = true;
                            Process revorbP = Process.Start(revorbPsi);
                            revorbP.WaitForExit();
                            Console.WriteLine("done.");
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Done.");

#if DEBUG
                Console.ReadLine();
#endif
            }
        }
    }
}