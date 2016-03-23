using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CmdLine;
using ThomasJepp.SaintsRow.Packfiles;

namespace ThomasJepp.SaintsRow.ClothSim
{
    class Program
    {
[CommandLineArguments(Program = "ThomasJepp.SaintsRow.ClothSim", Title = "Saints Row Cloth Sim Converter", Description = "Converts Saints Row: The Third and Saints Row IV cloth sim files to Saints Row: Gat Out of Hell format.")]
        internal class Options
        {
            [CommandLineParameter(Name = "source", ParameterIndex = 1, Required = true, Description = "The original sim_pc file.")]
            public string Source { get; set; }

            [CommandLineParameter(Name = "output", ParameterIndex = 2, Required = true, Description = "The file to create.")]
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

            using (Stream s2 = File.OpenRead(options.Source))
            {
                ClothSimulation.Version02.ClothSimulationFile file2 = new ClothSimulation.Version02.ClothSimulationFile(s2);
                ClothSimulation.Version05.ClothSimulationFile file5 = file2.ConvertToVersion5();

                using (Stream s5 = File.Create(options.Output))
                {
                    file5.Save(s5);
                }
            }
        }
    }
}
