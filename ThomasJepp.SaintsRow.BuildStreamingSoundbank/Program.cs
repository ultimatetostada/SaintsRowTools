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
                    string gameName = reader.GetAttribute("game");
                    IGameInstance instance = GameInstance.GetFromString(gameName);

                    StreamingSoundbank bank = new StreamingSoundbank();
                    bank.Header.WwiseBankId = wwiseId;

                    string xmlFolder = Path.GetDirectoryName(options.Source);

                    while (reader.ReadToFollowing("file"))
                    {
                        uint fileId = uint.Parse(reader.GetAttribute("id"));
                        string audio = reader.GetAttribute("audio");
                        MemoryStream metadataStream = null;
                        if (reader.ReadToDescendant("metadata"))
                        {
                            AudioMetadata metadata = new AudioMetadata(instance);

                            using (XmlReader metadataReader = reader.ReadSubtree())
                            {
                                metadataReader.Read();
                                uint metadataVersion = uint.Parse(metadataReader.GetAttribute("version"));
                                uint personaId = uint.Parse(metadataReader.GetAttribute("personaid"));
                                uint voicelineId = uint.Parse(metadataReader.GetAttribute("voicelineid"));
                                uint wavLengthMs = uint.Parse(metadataReader.GetAttribute("wavlengthms"));
                                metadata.Header.Version = metadataVersion;
                                metadata.Header.PersonaID = personaId;
                                metadata.Header.VoicelineID = voicelineId;
                                metadata.Header.WavLengthMs = wavLengthMs;

                                while (metadataReader.Read())
                                {
                                    if (metadataReader.LocalName == "lipsync")
                                    {
                                        string lipsyncBase64 = metadataReader.ReadElementContentAsString();
                                        metadata.LipsyncData = Convert.FromBase64String(lipsyncBase64);
                                    }
                                    else if (metadataReader.LocalName == "subtitles")
                                    {
                                        uint subtitleVersion = uint.Parse(metadataReader.GetAttribute("version"));
                                        metadata.SubtitleVersion = subtitleVersion;
                                        switch (subtitleVersion)
                                        {
                                            case 2:
                                                using (XmlReader subtitlesReader = metadataReader.ReadSubtree())
                                                {
                                                    subtitlesReader.Read();
                                                    using (XmlReader genderReader = subtitlesReader.ReadSubtree())
                                                    {
                                                        genderReader.Read();

                                                        while (genderReader.Read())
                                                        {
                                                            if (genderReader.LocalName == "male" && genderReader.IsStartElement())
                                                            {
                                                                using (XmlReader subtitleReader = genderReader.ReadSubtree())
                                                                {
                                                                    subtitleReader.Read();
                                                                    while (subtitleReader.ReadToFollowing("subtitle"))
                                                                    {
                                                                        string languageString = subtitleReader.GetAttribute("language");
                                                                        subtitleReader.Read();
                                                                        string text = subtitleReader.ReadContentAsString();
                                                                        Language language = LanguageUtility.GetLanguageFromCode(languageString);
                                                                        metadata.MaleSubtitles.Add(language, text);
                                                                    }
                                                                }
                                                            }
                                                            else if (genderReader.LocalName == "female" && genderReader.IsStartElement())
                                                            {
                                                                using (XmlReader subtitleReader = genderReader.ReadSubtree())
                                                                {
                                                                    subtitleReader.Read();
                                                                    while (subtitleReader.ReadToFollowing("subtitle"))
                                                                    {
                                                                        string languageString = subtitleReader.GetAttribute("language");
                                                                        subtitleReader.Read();
                                                                        string text = subtitleReader.ReadContentAsString();
                                                                        Language language = LanguageUtility.GetLanguageFromCode(languageString);

                                                                        metadata.FemaleSubtitles.Add(language, text);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            break;

                                            case 3:
                                                using (XmlReader subtitleReader = metadataReader.ReadSubtree())
                                                {
                                                    subtitleReader.Read();
                                                    while (subtitleReader.ReadToFollowing("subtitle"))
                                                    {
                                                        string languageString = subtitleReader.GetAttribute("language");
                                                        subtitleReader.Read();
                                                        string text = subtitleReader.ReadContentAsString();
                                                        Language language = LanguageUtility.GetLanguageFromCode(languageString);
                                                        metadata.Subtitles.Add(language, text);
                                                    }
                                                }
                                                break;

                                            default:
                                                throw new NotImplementedException();
                                                break;
                                        }
                                    }
                                }
                            }
                            metadataStream = new MemoryStream();
                            metadata.Save(metadataStream);
                        }

                        Stream audioStream = File.OpenRead(Path.Combine(xmlFolder, audio));
                        bank.AddFile(fileId, audioStream, metadataStream);
                    }

                    using (Stream outStream = File.Create(bnkName))
                    {
                        using (Stream mbnkOutStream = File.Create(mbnkName))
                        {
                            bank.Save(outStream, mbnkOutStream);
                        }
                    }
                }
            }
        }
    }
}
