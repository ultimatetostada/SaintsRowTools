using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThomasJepp.SaintsRow.Packfiles;

namespace ThomasJepp.SaintsRow.EditPackfile
{
    public class NewFileEntry
    {
        public NewFileEntry(string path)
        {
            Path = path;
        }

        public string Path;
        public bool Inserted;
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("EditPackfile <source> <destination> <new file 1> <new file 2>...");
                return;
            }

            string sourcePath = args[0];
            string destinationPath = args[1];

            List<string> newFiles = new List<string>();
            for (int i = 2; i < args.Length; i++)
                newFiles.Add(args[i]);

            string sourceExtension = Path.GetExtension(sourcePath);
            bool sourceIsStr2 = sourceExtension.ToLowerInvariant() == ".str2_pc";

            Dictionary<string, NewFileEntry> filenameToPathMap = new Dictionary<string, NewFileEntry>(StringComparer.OrdinalIgnoreCase);
            foreach (string newFile in newFiles)
            {
                filenameToPathMap.Add(Path.GetFileName(newFile), new NewFileEntry(newFile));
            }

            using (Stream sourceStream = File.OpenRead(sourcePath))
            {
                using (IPackfile source = Packfile.FromStream(sourceStream, sourceIsStr2))
                {
                    using (IPackfile destination = Packfile.FromVersion(source.Version, sourceIsStr2))
                    {
                        destination.IsCompressed = source.IsCompressed;
                        destination.IsCondensed = source.IsCondensed;

                        foreach (IPackfileEntry entry in source.Files)
                        {
                            string filename = entry.Name;
                            if (filenameToPathMap.ContainsKey(filename))
                            {
                                NewFileEntry newFilePath = filenameToPathMap[filename];
                                destination.AddFile(File.OpenRead(newFilePath.Path), entry.Name);
                                newFilePath.Inserted = true;
                                Console.WriteLine("Replaced {0}.", filename);
                            }
                            else
                            {
                                destination.AddFile(entry.GetStream(), entry.Name);
                            }
                        }
                        
                        foreach (var pair in filenameToPathMap)
                        {
                            NewFileEntry nfe = pair.Value;
                            if (!nfe.Inserted)
                            {
                                string filename = Path.GetFileName(nfe.Path);
                                destination.AddFile(File.OpenRead(nfe.Path), filename);
                                Console.WriteLine("Inserted {0}.", filename);
                            }
                        }

                        using (Stream destinationStream = File.Create(destinationPath))
                        {
                            destination.Save(destinationStream);
                        }
                    }
                }
            }

#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
