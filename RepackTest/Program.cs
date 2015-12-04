using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ThomasJepp.SaintsRow;
using ThomasJepp.SaintsRow.Packfiles;
using ThomasJepp.SaintsRow.AssetAssembler;

namespace RepackTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string src = @"D:\Gaming\Saints Row 4\test\in";
            string temp = @"D:\Gaming\Saints Row 4\test";
            string dst = @"D:\Gaming\Saints Row 4\test\out";

            string[] packfileFolders = Directory.GetDirectories(src);

            GameSteamID game = GameSteamID.SaintsRowIV;

            foreach (string packfileFolder in packfileFolders)
            {
                string pfTemp = Path.Combine(temp, Path.GetFileName(packfileFolder));

                Directory.CreateDirectory(pfTemp);

                // Copy raw files
                string[] pfFiles = Directory.GetFiles(packfileFolder);
                foreach (string file in pfFiles)
                {
                    File.Copy(file, Path.Combine(pfTemp, Path.GetFileName(file)));
                }

                string[] asmFiles = Directory.GetFiles(pfTemp, "*.asm_pc");


                foreach (string asmFile in asmFiles)
                {
                    IAssetAssemblerFile asm = null;
                    using (Stream stream = File.OpenRead(asmFile))
                    {
                        asm = AssetAssemblerFile.FromStream(stream);
                    }

                    int count = 0;
                    foreach (var container in asm.Containers)
                    {
                        count++;
                        string str2Name = Path.ChangeExtension(container.Name, ".str2_pc");
                        string str2Src = Path.Combine(packfileFolder, str2Name);

                        Console.Write("[{0}/{1}] Building {2}...", count, asm.Containers.Count, str2Src.Remove(0, src.Length + 1));
                        if (Directory.Exists(str2Src))
                        {
                            string outputFile = Path.Combine(pfTemp, Path.GetFileName(str2Src));
                            ProcessStartInfo psi = new ProcessStartInfo(@"D:\Development\SaintsRow\bin\Release\ThomasJepp.SaintsRow.BuildPackfile.exe", String.Format("{3} \"{0}\" \"{1}\" /asm:\"{2}\"", str2Src, outputFile, asmFile, game.ToString()));
                            psi.CreateNoWindow = true;
                            psi.WindowStyle = ProcessWindowStyle.Hidden;
                            Process p = Process.Start(psi);
                            p.WaitForExit();
                            Console.WriteLine(" OK");
                        }
                        else
                        {
                            Console.WriteLine(" not found!");
                        }
                    }
                }

                Console.Write("Building {0}...", Path.GetFileName(packfileFolder));

                var options = OriginalPackfileInfo.OptionsList[game][Path.GetFileName(packfileFolder)];

                ProcessStartInfo packpsi = new ProcessStartInfo(@"D:\Development\SaintsRow\bin\Release\ThomasJepp.SaintsRow.BuildPackfile.exe", String.Format("sriv \"{0}\" \"{1}\" /condensed:{2} /compressed:{3}", pfTemp, Path.Combine(dst, Path.GetFileName(packfileFolder)), options.Condense, options.Compress));
                packpsi.CreateNoWindow = true;
                packpsi.WindowStyle = ProcessWindowStyle.Hidden;
                Process packProcess = Process.Start(packpsi);
                packProcess.WaitForExit();

                Console.WriteLine(" OK");
            }

            Console.WriteLine("Done.");
        }
    }
}
