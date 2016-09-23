using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CmdLine;
using ThomasJepp.SaintsRow.Packfiles;

namespace ThomasJepp.SaintsRow.ConvertClothSim
{
    class Program
    {
[CommandLineArguments(Program = "ThomasJepp.SaintsRow.ConvertClothSim", Title = "Saints Row Cloth Sim Converter", Description = "Converts cloth sim files between SRTT/SRIV and SRGOOH.")]
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
                Int32 version = inputStream.ReadInt32();
                inputStream.Seek(-4, SeekOrigin.Current);

                switch (version)
                {
                    case 2:
                        {
                            Console.WriteLine("Converting from SRTT/SRIV format to SRGOOH format.");
                            ClothSimulation.Version02.ClothSimulationFile original = new ClothSimulation.Version02.ClothSimulationFile(inputStream);
                            ClothSimulation.Version05.ClothSimulationFile converted = original.ConvertToVersion5();

                            using (Stream outputStream = File.Create(options.Output))
                            {
                                converted.Save(outputStream);
                            }
                            break;
                        }
                    case 5:
                        {
                            Console.WriteLine("Converting from SRGOOH format to SRTT/SRIV format.");
                            ClothSimulation.Version05.ClothSimulationFile original = new ClothSimulation.Version05.ClothSimulationFile(inputStream);
                            ClothSimulation.Version02.ClothSimulationFile converted = original.ConvertToVersion2();

                            using (Stream outputStream = File.Create(options.Output))
                            {
                                converted.Save(outputStream);
                            }

                            break;
                        }

                    default:
                        throw new Exception("This file is not a recognised cloth sim version, or it is not a valid cloth sim file.");
                }
            }
        }
    }
}
