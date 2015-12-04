using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using CmdLine;
using ThomasJepp.SaintsRow.Packfiles;
using ThomasJepp.SaintsRow.AssetAssembler;

namespace ThomasJepp.SaintsRow.Stream2
{
    [CommandLineArguments(Program = "ThomasJepp.SaintsRow.Stream2", Title = "Saints Row Stream2 File Tool", Description = "Performs various actions on Stream2 files (ASM files). Supports Saints Row: The Third, Saints Row IV and Saints Row: Gat out of Hell.")]
    internal class Options
    {
        [CommandLineParameter(Name = "source", ParameterIndex = 1, Required = true, Description = "The Stream2 Container to process.")]
        public string Source { get; set; }

        [CommandLineParameter(Name = "action", ParameterIndex = 2, Required = true, Description = "The action to perform. Valid actions are \"clean\", \"toasm\", \"toxml\" and \"update\".")]
        public string Action { get; set; }

        [CommandLineParameter(Name = "output", ParameterIndex = 3, Required = false, Description = "If the action is \"toasm\" or \"toxml\", this is used as the output. If not specified, the new file will be placed in the same directory as the source file. This is not used for \"update\".")]
        public string Output { get; set; }
    }

    class Program
    {
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


            switch (options.Action)
            {
                case "toxml":
                    {
                        using (Stream stream = File.OpenRead(options.Source))
                        {
                            IAssetAssemblerFile file = AssetAssemblerFile.FromStream(stream);

                            if (options.Output == null || options.Output == "")
                            {
                                options.Output = Path.ChangeExtension(options.Source, "xml");
                            }

                            XmlWriterSettings settings = new XmlWriterSettings();
                            settings.Indent = true;
                            settings.IndentChars = "\t";
                            settings.NewLineChars = "\r\n";

                            using (XmlWriter xml = XmlWriter.Create(options.Output, settings))
                            {
                                xml.WriteStartDocument();
                                xml.WriteStartElement("AssetAssembler");
                                xml.WriteAttributeString("Version", file.Version.ToString());

                                xml.WriteStartElement("AllocatorTypes");
                                foreach (var pair in file.AllocatorTypes)
                                {
                                    xml.WriteStartElement("AllocatorType");
                                    xml.WriteAttributeString("ID", pair.Key.ToString());
                                    xml.WriteAttributeString("Name", pair.Value);
                                    xml.WriteEndElement();
                                }
                                xml.WriteEndElement(); // AllocatorTypes

                                xml.WriteStartElement("ContainerTypes");
                                foreach (var pair in file.ContainerTypes)
                                {
                                    xml.WriteStartElement("ContainerType");
                                    xml.WriteAttributeString("ID", pair.Key.ToString());
                                    xml.WriteAttributeString("Name", pair.Value);
                                    xml.WriteEndElement();
                                }
                                xml.WriteEndElement(); // ContainerTypes

                                xml.WriteStartElement("PrimitiveTypes");
                                foreach (var pair in file.PrimitiveTypes)
                                {
                                    xml.WriteStartElement("PrimitiveType");
                                    xml.WriteAttributeString("ID", pair.Key.ToString());
                                    xml.WriteAttributeString("Name", pair.Value);
                                    xml.WriteEndElement();
                                }
                                xml.WriteEndElement(); // PrimitiveTypes

                                xml.WriteStartElement("Containers");
                                foreach (var container in file.Containers)
                                {
                                    xml.WriteStartElement("Container");
                                    xml.WriteAttributeString("Name", container.Name);
                                    xml.WriteAttributeString("Type", file.ContainerTypes[container.ContainerType]);
                                    xml.WriteAttributeString("Flags", container.Flags.ToString());
                                    xml.WriteAttributeString("PackfileBaseOffset", container.PackfileBaseOffset.ToString());
                                    xml.WriteAttributeString("CompressionType", container.CompressionType.ToString());
                                    xml.WriteAttributeString("StubContainerParentName", container.StubContainerParentName);
                                    xml.WriteAttributeString("TotalCompressedPackfileReadSize", container.TotalCompressedPackfileReadSize.ToString());

                                    if (container.AuxData.Length > 0)
                                    {
                                        xml.WriteStartElement("AuxData");
                                        xml.WriteString(Convert.ToBase64String(container.AuxData, Base64FormattingOptions.None));
                                        xml.WriteEndElement(); // AuxData
                                    }

                                    for (int i = 0; i < container.PrimitiveCount; i++)
                                    {
                                        var primitive = container.Primitives[i];
                                        //var sizes = container.PrimitiveSizes[i];

                                        xml.WriteStartElement("Primitive");

                                        xml.WriteAttributeString("Name", primitive.Name);
                                        xml.WriteAttributeString("Type", file.PrimitiveTypes[primitive.Type]);
                                        xml.WriteAttributeString("Allocator", file.AllocatorTypes.ContainsKey(primitive.Allocator) ? file.AllocatorTypes[primitive.Allocator] : primitive.Allocator.ToString());
                                        xml.WriteAttributeString("Flags", primitive.Flags.ToString());
                                        xml.WriteAttributeString("ExtensionIndex", primitive.ExtensionIndex.ToString());
                                        xml.WriteAttributeString("CPUSize", primitive.CPUSize.ToString());
                                        xml.WriteAttributeString("GPUSize", primitive.GPUSize.ToString());
                                        xml.WriteAttributeString("AllocationGroup", primitive.AllocationGroup.ToString());

                                        //xml.WriteAttributeString("WriteTimeCPUSize", sizes.CPUSize.ToString());
                                        //xml.WriteAttributeString("WriteTimeGPUSize", sizes.GPUSize.ToString());

                                        xml.WriteEndElement(); // Primitive
                                    }

                                    xml.WriteEndElement(); // Container
                                }

                                xml.WriteEndElement(); // Containers

                                xml.WriteEndElement(); // AssetAssembler
                                xml.WriteEndDocument();
                            }
                        }
                        break;
                    }
                case "toasm":
                    {
                        using (Stream stream = File.OpenRead(options.Source))
                        {
                            XDocument xml = XDocument.Load(stream);
                            var asmNode = xml.Element("AssetAssembler");
                            uint version = uint.Parse(asmNode.Attribute("Version").Value);

                            IAssetAssemblerFile file = AssetAssemblerFile.Create(version);

                            if (options.Output == null || options.Output == "")
                            {
                                options.Output = Path.ChangeExtension(options.Source, "asm_pc");
                            }

                            Dictionary<string, byte> allocatorTypesLookup = new Dictionary<string, byte>();
                            Dictionary<string, byte> containerTypesLookup = new Dictionary<string, byte>();
                            Dictionary<string, byte> primitiveTypesLookup = new Dictionary<string, byte>();

                            foreach (var node in xml.Descendants("AllocatorType"))
                            {
                                byte id = byte.Parse(node.Attribute("ID").Value);
                                string name = node.Attribute("Name").Value;
                                allocatorTypesLookup.Add(name, id);
                                file.AllocatorTypes.Add(id, name);
                            }

                            foreach (var node in xml.Descendants("ContainerType"))
                            {
                                byte id = byte.Parse(node.Attribute("ID").Value);
                                string name = node.Attribute("Name").Value;
                                containerTypesLookup.Add(name, id);
                                file.ContainerTypes.Add(id, name);
                            }

                            foreach (var node in xml.Descendants("PrimitiveType"))
                            {
                                byte id = byte.Parse(node.Attribute("ID").Value);
                                string name = node.Attribute("Name").Value;
                                primitiveTypesLookup.Add(name, id);
                                file.PrimitiveTypes.Add(id, name);
                            }

                            foreach (var cNode in xml.Descendants("Container"))
                            {
                                IContainer container = file.CreateContainer();
                                container.Name = cNode.Attribute("Name").Value;
                                container.ContainerType = containerTypesLookup[cNode.Attribute("Type").Value];
                                container.Flags = (ContainerFlags)ushort.Parse(cNode.Attribute("Flags").Value);
                                container.PackfileBaseOffset = uint.Parse(cNode.Attribute("PackfileBaseOffset").Value);
                                container.CompressionType = byte.Parse(cNode.Attribute("CompressionType").Value);
                                container.StubContainerParentName = cNode.Attribute("StubContainerParentName").Value;
                                container.TotalCompressedPackfileReadSize = int.Parse(cNode.Attribute("TotalCompressedPackfileReadSize").Value);
                                var auxData = cNode.Element("AuxData");
                                if (auxData != null)
                                {
                                    container.AuxData = Convert.FromBase64String(auxData.Value);
                                }
                                else
                                {
                                    container.AuxData = new byte[0];
                                }
                                
                                foreach (var pNode in cNode.Descendants("Primitive"))
                                {
                                    IPrimitive p = container.CreatePrimitive();
                                    p.Name = pNode.Attribute("Name").Value;
                                    p.Type = primitiveTypesLookup[pNode.Attribute("Type").Value];
                                    byte allocatorType = 0;
                                    if (byte.TryParse(pNode.Attribute("Allocator").Value, out allocatorType))
                                    {
                                        p.Allocator = allocatorType;
                                    }
                                    else
                                    {
                                        p.Allocator = allocatorTypesLookup[pNode.Attribute("Allocator").Value];
                                    }
                                    p.Flags = byte.Parse(pNode.Attribute("Flags").Value);
                                    p.ExtensionIndex = byte.Parse(pNode.Attribute("ExtensionIndex").Value);
                                    p.CPUSize = uint.Parse(pNode.Attribute("CPUSize").Value);
                                    p.GPUSize = uint.Parse(pNode.Attribute("GPUSize").Value);
                                    p.AllocationGroup = byte.Parse(pNode.Attribute("AllocationGroup").Value);
                                    container.Primitives.Add(p);
                                }

                                container.PrimitiveCount = (short)container.Primitives.Count;
                                file.Containers.Add(container);
                            }

                            using (Stream outputStream = File.Create(options.Output))
                            {
                                file.Save(outputStream);
                            }
                        }
                        break;
                    }
                case "update":
                    {
                        IAssetAssemblerFile file = null;
                        using (Stream stream = File.OpenRead(options.Source))
                        {
                            file = AssetAssemblerFile.FromStream(stream);

                            string folder = Path.GetDirectoryName(options.Source);

                            foreach (var container in file.Containers)
                            {
                                string str2File = Path.ChangeExtension(container.Name, ".str2_pc");
                                string filename = Path.Combine(folder, str2File);
                                if (File.Exists(filename))
                                {
                                    Console.Write("Found {0}, updating... ", str2File);
                                    using (Stream str2Stream = File.OpenRead(filename))
                                    {
                                        IPackfile packfile = Packfile.FromStream(str2Stream, true);
                                        packfile.Update(container);
                                    }
                                    Console.WriteLine("done.");
                                }
                                else
                                {
                                    Console.WriteLine("Could not find {0}.", str2File);
                                }
                            }
                        }

                        using (Stream stream = File.Create(options.Source))
                        {
                            file.Save(stream);
                        }
                        break;
                    }
                case "clean":
                    {
                        IAssetAssemblerFile file = null;
                        using (Stream stream = File.OpenRead(options.Source))
                        {
                            file = AssetAssemblerFile.FromStream(stream);

                            string folder = Path.GetDirectoryName(options.Source);

                            List<string> foundContainerNames = new List<string>();
                            List<IContainer> duplicates = new List<IContainer>();
                            foreach (var container in file.Containers)
                            {
                                if (!foundContainerNames.Contains(container.Name))
                                {
                                    foundContainerNames.Add(container.Name);
                                }
                                else
                                {
                                    duplicates.Add(container);
                                }
                            }

                            foreach (var container in duplicates)
                            {
                                file.Containers.Remove(container);
                            }
                        }

                        using (Stream stream = File.Create(options.Source))
                        {
                            file.Save(stream);
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Unrecogised action!");
                        break;
                    }
            }

#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
