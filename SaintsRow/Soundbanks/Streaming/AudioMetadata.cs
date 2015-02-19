using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ThomasJepp.SaintsRow.Localization;

namespace ThomasJepp.SaintsRow.Soundbanks.Streaming
{
    public class AudioMetadata
    {
        public AudioMetadataHeader Header;
        public AudioMetadataSubtitleHeader SubtitleHeader;
        public byte[] LipsyncData = null;
        public Dictionary<Language, string> Subtitles = new Dictionary<Language,string>();

        public AudioMetadata(Stream stream, string gameDir)
        {
            Header = stream.ReadStruct<AudioMetadataHeader>();
            
            if (Header.LipsyncSize > 0)
            {
                stream.Seek(0x24 + Header.LipsyncOffset, SeekOrigin.Begin);
                LipsyncData = new byte[Header.LipsyncSize];
                stream.Read(LipsyncData, 0, LipsyncData.Length);
            }

            if (Header.SubtitleSize > 0)
            {
                stream.Seek(0x24 + Header.SubtitleOffset, SeekOrigin.Begin);
                SubtitleHeader = stream.ReadStruct<AudioMetadataSubtitleHeader>();

                long subtitleOffset = stream.Position;
               
                for (int i = 0; i < SubtitleHeader.LocalizedVoiceSubtitleHeaders.Length; i++)
                {
                    LocalizedVoiceSubtitleHeader localizedVoiceSubtitleHeader = SubtitleHeader.LocalizedVoiceSubtitleHeaders[i];
                    Language language = (Language)i;

                    if (localizedVoiceSubtitleHeader.Length == 0)
                    {
                        Subtitles.Add(language, "");

                        continue;
                    }

                    long offset = subtitleOffset + localizedVoiceSubtitleHeader.Offset;
                    stream.Seek(offset, SeekOrigin.Begin);
                    byte[] subtitleData = new byte[localizedVoiceSubtitleHeader.Length];
                    stream.Read(subtitleData, 0, (int)localizedVoiceSubtitleHeader.Length);

                    var map = LanguageUtility.GetCharMap(gameDir, language);

                    StringBuilder subtitleBuilder = new StringBuilder();
                    for (int pos = 0; pos < subtitleData.Length; pos+=2)
                    {
                        char src = BitConverter.ToChar(subtitleData, pos);

                        char value = src;
                        if (map.ContainsKey(src))
                            value = map[src];

                        if (value == 0x00)
                            continue;

                        subtitleBuilder.Append(value);
                    }

                    string subtitle = subtitleBuilder.ToString();
                    Subtitles.Add(language, subtitle);
                }
            }
        }
    }
}
