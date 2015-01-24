using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using CmdLine;
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

            [CommandLineParameter(Command = "convert", Default = true, Description = "Convert the audio into playble OGG files. Requires ww2ogg and revorb in the same directory.", Name = "Convert audio")]
            public bool ConvertAudio { get; set; }
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
            if (options.ConvertAudio)
            {
                bool failed = false;
                if (!File.Exists(ww2ogg))
                {
                    Console.WriteLine("Could not find ww2ogg.exe at:\n{0}", ww2ogg);
                    failed = true;
                }

                if (!File.Exists(codebooks))
                {
                    Console.WriteLine("Could not find packed_codebooks_aoTuV_603.bin at:\n{0}", codebooks);
                    failed = true;
                }

                if (!File.Exists(revorb))
                {
                    Console.WriteLine("Could not find revorb.exe at:\n{0}", revorb);
                    failed = true;
                }

                if (failed)
                    return;
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

                using (Stream xmlStream = File.OpenWrite(Path.Combine(folderName, Path.ChangeExtension(bnkName, "xml"))))
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

                            if (entry.Info.MetadataLength != 0)
                            {
                                Console.Write("[{0}/{1}] Extracting metadata... ", currentFile, bnk.Files.Count);
                                string metadataFilename = String.Format("{0}_{1:D5}.metadata", bnkName, currentFile);
                                using (Stream outputStream = File.OpenWrite(Path.Combine(folderName, metadataFilename)))
                                {
                                    using (Stream inputStream = entry.GetMetadataStream())
                                    {
                                        inputStream.CopyTo(outputStream);
                                    }
                                    outputStream.Flush();
                                }
                                Console.WriteLine("done.");
                                writer.WriteAttributeString("metadata", metadataFilename);
                            }

                            Console.Write("[{0}/{1}] Extracting audio... ", currentFile, bnk.Files.Count);
                            string audioFilename = String.Format("{0}_{1:D5}.wem", bnkName, currentFile);
                            using (Stream outputStream = File.OpenWrite(Path.Combine(folderName, audioFilename)))
                            {
                                using (Stream inputStream = entry.GetAudioStream())
                                {
                                    inputStream.CopyTo(outputStream);
                                }
                                outputStream.Flush();
                            }
                            Console.WriteLine("done.");
                            writer.WriteAttributeString("audio", audioFilename);

                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                }

                if (options.ConvertAudio)
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

                Console.WriteLine();
                Console.WriteLine("Done.");

#if DEBUG
                Console.ReadLine();
#endif
            }
        }
    }
}