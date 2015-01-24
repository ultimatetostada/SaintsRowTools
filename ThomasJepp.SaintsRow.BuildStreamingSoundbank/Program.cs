using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using CmdLine;
using ThomasJepp.SaintsRow.Soundbanks.Streaming;

namespace ThomasJepp.SaintsRow.BuildStreamingSoundbank
{
    class Program
    {
        [CommandLineArguments(Program = "ThomasJepp.SaintsRow.BuildStreamingSoundbank", Title = "Saints Row Streaming Soundbank Builder", Description = "Builds Saints Row PC Streaming Soundbanks (..._media.bnk_pc files). Supports Saints Row The Third, Saints Row IV and Saints Row Gat Out Of Hell.")]
        internal class Options
        {
            [CommandLineParameter(Name = "xml", ParameterIndex = 1, Required = true, Description = "The soundbank XML to use.")]
            public string Source { get; set; }

            [CommandLineParameter(Name = "output", ParameterIndex = 2, Required = false, Description = "The location to save the new soundbank. If not specified, the soundbank will be saved to the current directory.")]
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

            using (Stream stream = File.OpenRead(options.Source))
            {
                string bnkName = (options.Output != null) ? options.Output : Path.ChangeExtension(Path.GetFileName(options.Source), "bnk_pc");
                string mbnkName = Path.ChangeExtension(bnkName, "mbnk_pc");

                Console.WriteLine("Building {0} from {1}.", bnkName, options.Source);

                using (XmlReader reader = XmlReader.Create(stream))
                {
                    reader.ReadToFollowing("soundbank");
                    uint wwiseId = uint.Parse(reader.GetAttribute("wwiseId"));

                    StreamingSoundbank bank = new StreamingSoundbank();
                    bank.Header.WwiseBankId = wwiseId;

                    string xmlFolder = Path.GetDirectoryName(options.Source);

                    while (reader.ReadToFollowing("file"))
                    {
                        uint fileId = uint.Parse(reader.GetAttribute("id"));
                        string metadata = reader.GetAttribute("metadata");
                        string audio = reader.GetAttribute("audio");

                        Stream metadataStream = null;
                        if (metadata != null)
                        {
                            metadataStream = File.OpenRead(Path.Combine(xmlFolder, metadata));
                        }

                        Stream audioStream = File.OpenRead(Path.Combine(xmlFolder, audio));
                        bank.AddFile(fileId, audioStream, metadataStream);
                    }

                    using (Stream outStream = File.OpenWrite(bnkName))
                    {
                        using (Stream mbnkOutStream = File.OpenWrite(mbnkName))
                        {
                            bank.Save(outStream, mbnkOutStream);
                        }
                    }
                }
            }
        }
    }
}
