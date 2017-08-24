using System;
using System.Collections.Generic;
using System.IO;

using CmdLine;
using ThomasJepp.SaintsRow.Packfiles;

namespace ThomasJepp.SaintsRow.RecursiveExtractor
{
    class Program
    {
        [CommandLineArguments(Program = "ThomasJepp.SaintsRow.RecursiveExtractor", Title = "Saints Row Recursive Packfile Extractor", Description = "Extracts all Saints Row PC packfiles in the specified folder and any packfiles that they contain. Supports Saints Row IV.")]
        internal class Options
        {
            [CommandLineParameter(Name = "source", ParameterIndex = 1, Required = true, Description = "The folder containing packfiles to extract.")]
            public string Source { get; set; }

            [CommandLineParameter(Name = "output", ParameterIndex = 2, Required = false, Default = "output", Description = "The folder to extract the packfiles to. This will be created if it does not already exist. If not specified, a folder named \"output\" will be created in the current directory.")]
            public string Output { get; set; }
        }

        static int FindPackfiles(string dir, Dictionary<string, IPackfile> packfiles)
        {
            int totalFiles = 0;

            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".vpp_pc")
                {
                    var packfile = Packfile.FromStream(File.OpenRead(file), false);
                    totalFiles += packfile.Files.Count;
                    packfiles.Add(Path.GetFileName(file), packfile);
                }
            }
            string[] dirs = Directory.GetDirectories(dir);
            foreach (string subDir in dirs)
            {
                totalFiles += FindPackfiles(subDir, packfiles);
            }

            return totalFiles;
        }

        public static StreamWriter logOut = new StreamWriter("log.txt", false, new System.Text.UTF8Encoding(false));
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

            Directory.CreateDirectory(options.Output);
           
            Dictionary<string, IPackfile> packfiles = new Dictionary<string, IPackfile>();

            Console.Write("Looking for packfiles... ");
            int totalFiles = FindPackfiles(options.Source, packfiles);
            Console.WriteLine("found {0} packfiles, containing {1} files.", packfiles.Count, totalFiles);

            int currentFile = 0;

            foreach (var packfilePair in packfiles)
            {
                Console.WriteLine("{0} Compressed: {1} Condensed: {2}", packfilePair.Key, packfilePair.Value.IsCompressed, packfilePair.Value.IsCondensed);
                Directory.CreateDirectory(Path.Combine(options.Output, packfilePair.Key));
                foreach (var file in packfilePair.Value.Files)
                {
                    currentFile++;
                    if (Path.GetExtension(file.Name) == ".str2_pc")
                    {
                        string outputPath;
                        if (file.Path != null)
                        {
                            outputPath = Path.Combine(options.Output, packfilePair.Key, file.Path);
                        }
                        else
                        {
                            outputPath = Path.Combine(options.Output, packfilePair.Key);
                        }

                        string strOutputFolder = Path.Combine(outputPath, file.Name);
                        Directory.CreateDirectory(strOutputFolder);
                        //Console.WriteLine("[{0}/{1}] Extracting {2}: packfile {3} to {4}:", currentFile, totalFiles, packfilePair.Key, file.Name, strOutputFolder);
                        using (Stream strStream = file.GetStream())
                        {
                            using (var strPackfile = Packfile.FromStream(strStream, true))
                            {
                                int strCurrentFile = 0;

                                foreach (var strFile in strPackfile.Files)
                                {
                                    string strFileOutputPath;
                                    if (strFile.Path != null)
                                    {
                                        strFileOutputPath = Path.Combine(strOutputFolder, strFile.Path);
                                        Directory.CreateDirectory(strFileOutputPath);
                                    }
                                    else
                                    {
                                        strFileOutputPath = strOutputFolder;
                                    }

                                    strCurrentFile++;

                                    //Console.Write("[{0}/{1}] [{2}/{3}] Extracting {4}\\{5}: {6}", currentFile, totalFiles, strCurrentFile, strPackfile.Files.Count, packfilePair.Key, file.Name, strFile.Name);
                                    try
                                    {
                                        using (Stream outputStream = File.Create(Path.Combine(strFileOutputPath, strFile.Name)))
                                        {
                                            using (Stream inputStream = strFile.GetStream())
                                            {
                                                inputStream.CopyTo(outputStream);
                                            }
                                            outputStream.Flush();
                                        }
                                        //Console.WriteLine("done.");
                                    }
                                    catch (Exception ex)
                                    {
                                        //Console.WriteLine("error extracting!");
                                        logOut.WriteLine("Error extracting: {0}\\{1}: {2} - {3}", packfilePair.Key, file.Name, strFile.Name, ex.Message);
                                        logOut.Flush();
                                    }
                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        //Console.Write("[{0}/{1}] Extracting {2}: {3}... ", currentFile, totalFiles, packfilePair.Key, file.Name);

                        string outputPath;
                        if (file.Path != null)
                        {
                            outputPath = Path.Combine(options.Output, packfilePair.Key, file.Path);
                            Directory.CreateDirectory(outputPath);
                        }
                        else
                        {
                            outputPath = Path.Combine(options.Output, packfilePair.Key);
                        }

                        try
                        {
                            using (Stream outputStream = File.Create(Path.Combine(outputPath, file.Name)))
                            {
                                using (Stream inputStream = file.GetStream())
                                {
                                    inputStream.CopyTo(outputStream);
                                }
                                outputStream.Flush();
                            }
                            //Console.WriteLine("done.");
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine("error extracting!");
                            logOut.WriteLine("Error extracting: {0}: {1} - {2}", packfilePair.Key, file.Name, ex.Message);
                            logOut.Flush();
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
