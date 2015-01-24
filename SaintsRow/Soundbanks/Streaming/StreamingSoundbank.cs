using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ThomasJepp.SaintsRow.Soundbanks.Streaming
{
    public class StreamingSoundbank : IDisposable
    {
        public SoundbankHeader Header;
        private List<SoundbankFileInfo> m_Files = new List<SoundbankFileInfo>();
        private Stream DataStream;

        public StreamingSoundbank(Stream s)
        {
            DataStream = new MemoryStream();
            s.CopyTo(DataStream);

            DataStream.Seek(0, SeekOrigin.Begin);

            Header = DataStream.ReadStruct<SoundbankHeader>();

            if (Header.Signature != 0x42535756)
                throw new InvalidDataException("File is not a streaming soundbank.");

            for (int i = 0; i < Header.NumFiles; i++)
            {
                var fileInfo = DataStream.ReadStruct<SoundbankFileInfo>();
                Files.Add(fileInfo);
            }
        }

        public List<SoundbankFileInfo> Files
        {
            get { return m_Files; }
        }

        public SoundbankFileInfo this[int i]
        {
            get { return m_Files[i]; }
        }

        public Stream GetAudioStream(SoundbankFileInfo file)
        {
            DataStream.Seek(file.Offset + file.MetadataLength, SeekOrigin.Begin);
            byte[] audioData = new byte[file.AudioLength];
            DataStream.Read(audioData, 0, audioData.Length);
            MemoryStream audioStream = new MemoryStream(audioData);
            
            return audioStream;
        }

        public Stream GetMetadataStream(SoundbankFileInfo file)
        {
            DataStream.Seek(file.Offset, SeekOrigin.Begin);
            byte[] metadata = new byte[file.MetadataLength];
            DataStream.Read(metadata, 0, metadata.Length);
            MemoryStream audioStream = new MemoryStream(metadata);

            return audioStream;
        }

        public void Dispose()
        {
            if (DataStream != null)
            {
                DataStream.Dispose();
                DataStream = null;
            }
        }
    }
}
