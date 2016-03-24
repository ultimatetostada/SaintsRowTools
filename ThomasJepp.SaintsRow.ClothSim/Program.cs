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

            using (Stream inputStream = File.OpenRead(options.Source))
            {
                ClothSimulation.Version02.ClothSimulationFile file2 = new ClothSimulation.Version02.ClothSimulationFile(inputStream);
                ClothSimulation.Version05.ClothSimulationFile converted = file2.ConvertToVersion5();

                using (StreamWriter sw = new StreamWriter("out-converted.txt"))
                {
                    converted.Dump(sw);
                }

                using (Stream tempStream = File.OpenRead(@"D:\SR\Saints Row Gat Out Of Hell\extracted\customize_item.vpp_pc\custmesh_590399893f.str2_pc\cf_hair_dreads.sim_pc"))
                {
                    ClothSimulation.Version05.ClothSimulationFile file5 = new ClothSimulation.Version05.ClothSimulationFile(tempStream);

                    using (StreamWriter sw = new StreamWriter("out-gooh.txt"))
                    {
                        file5.Dump(sw);
                    }

                    Console.WriteLine("aaa");

                }

                using (Stream outputStream = File.Create(options.Output))
                {
                    converted.Save(outputStream);
                }
            }
        }
    }
}
