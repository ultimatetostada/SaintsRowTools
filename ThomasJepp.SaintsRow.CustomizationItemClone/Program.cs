using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using CmdLine;
using ThomasJepp.SaintsRow;
using ThomasJepp.SaintsRow.AssetAssembler;
using ThomasJepp.SaintsRow.Bitmaps.Version13;
using ThomasJepp.SaintsRow.ClothSimulation.Version02;
using ThomasJepp.SaintsRow.GameInstances;
using ThomasJepp.SaintsRow.Localization;
using ThomasJepp.SaintsRow.Packfiles;
using ThomasJepp.SaintsRow.Meshes.StaticMesh;
using ThomasJepp.SaintsRow.Strings;
using ThomasJepp.SaintsRow.VFile;

namespace ThomasJepp.SaintsRow.CustomizationItemClone
{
    class Program
    {
        [CommandLineArguments(Program = "ThomasJepp.SaintsRow.CustomizationItemClone", Title = "Saints Row IV Customization Item clone tool", Description = "Clones a customization item into a new item with a new name.")]
        internal class Options
        {
            [CommandLineParameter(Name = "source", ParameterIndex = 1, Required = true, Description = "The name of the original item to clone.")]
            public string Source { get; set; }

            [CommandLineParameter(Name = "name", ParameterIndex = 2, Required = true, Description = "The new name to give the cloned item.")]
            public string NewName { get; set; }

            [CommandLineParameter(Name = "output", ParameterIndex = 3, Required = false, Description = "The directory to output the new item to. Defaults to the local directory if not specified.")]
            public string Output { get; set; }
        }

        static IContainer FindContainer(IGameInstance sriv, string containerName)
        {
            string[] asmNames = new string[]
            {
                "customize_item.asm_pc",
                "dlc1_customize_item.asm_pc",
                "dlc2_customize_item.asm_pc",
                "dlc3_customize_item.asm_pc",
                "dlc4_customize_item.asm_pc",
                "dlc5_customize_item.asm_pc",
                "dlc6_customize_item.asm_pc"
            };

            string name = Path.GetFileNameWithoutExtension(containerName);

            foreach (string asmName in asmNames)
            {
                using (Stream s = sriv.OpenPackfileFile(asmName))
                {
                    IAssetAssemblerFile asm = AssetAssemblerFile.FromStream(s);

                    foreach (var container in asm.Containers)
                    {
                        if (container.Name == name)
                            return container;
                    }
                }
            }

            return null;
        }

        static IPrimitive FindPrimitive(IContainer container, string primitiveName)
        {
            string primitiveMatchName = primitiveName.ToLowerInvariant();

            foreach (IPrimitive primitive in container.Primitives)
            {
                if (primitive.Name.ToLowerInvariant() == primitiveMatchName)
                {
                    return primitive;
                }
            }

            return null;
        }

        static Stream ProcessCCMesh(IPackfileEntry oldFile, string oldName, string newName)
        {
            MemoryStream outStream = new MemoryStream();

            using (Stream inStream = oldFile.GetStream())
            {
                VFile.VFile vFile = new VFile.VFile(inStream);
                StaticMesh mesh = new StaticMesh(inStream);

                for (int i = 0; i < vFile.References.Count; i++)
                {
                    string reference = vFile.References[i];
                    if (textureNameMap.ContainsKey(reference))
                        vFile.References[i] = textureNameMap[reference];
                }

                string[] textureNames = new string[mesh.TextureNames.Count];
                mesh.TextureNames.Keys.CopyTo(textureNames, 0);

                foreach (string textureName in textureNames)
                {
                    if (textureNameMap.ContainsKey(textureName))
                    {
                        mesh.RenameTexture(textureName, textureNameMap[textureName]);
                    }
                }

                vFile.Save(outStream);
                mesh.Save(outStream);
            }

            return outStream;
        }

        static Stream ProcessClothSim(IPackfileEntry oldFile, string newName)
        {
            MemoryStream outStream = new MemoryStream();
            using (Stream inStream = oldFile.GetStream())
            {
                ClothSimulationFile file = new ClothSimulationFile(inStream);
                file.Header.Name = newName;
                file.Save(outStream);
            }

            return outStream;
        }

        static Stream ProcessPeg(IPackfileEntry pegEntry, string newName)
        {
            textureNameMap.Clear();

            using (Stream pegStream = pegEntry.GetStream())
            {
                PegFile peg = new PegFile(pegStream);

                string sharedPrefix = peg.Entries[0].Filename;
                foreach (PegEntry entry in peg.Entries)
                {
                    while (!entry.Filename.StartsWith(sharedPrefix))
                    {
                        sharedPrefix = sharedPrefix.Substring(0, sharedPrefix.Length - 1);
                    }
                }

                foreach (PegEntry entry in peg.Entries)
                {
                    string newFilename = null;
                    if (sharedPrefix == "") 
                    {
                        newFilename = newName + "_" + entry.Filename;
                    }
                    else
                    {
                        newFilename = entry.Filename.Replace(sharedPrefix, newName + "_").ToLowerInvariant();
                    }

                    if (!textureNameMap.ContainsKey(entry.Filename))
                        textureNameMap.Add(entry.Filename, newFilename);

                    entry.Filename = newFilename;
                }

                MemoryStream outStream = new MemoryStream();
                peg.Save(outStream);

                return outStream;
            }
        }

        static IPackfileEntry FindPackfileEntry(IPackfile packfile, string extension)
        {
            foreach (IPackfileEntry entry in packfile.Files)
            {
                string entryExtension = Path.GetExtension(entry.Name).ToLowerInvariant();
                if (entryExtension == extension)
                    return entry;
            }

            return null;
        }

        static Dictionary<string, string> textureNameMap = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        static bool ClonePackfile(IGameInstance sriv, string packfileName, string clothSimFilename, string outputFolder, IAssetAssemblerFile newAsm, string oldName, string newName, string newStr2Filename)
        {
            using (Stream oldStream = sriv.OpenPackfileFile(packfileName))
            {
                if (oldStream == null)
                    return false;

                IContainer oldContainer = FindContainer(sriv, packfileName);
                if (oldContainer == null)
                    return false;

                oldContainer.Name = Path.GetFileNameWithoutExtension(newStr2Filename);

                using (IPackfile oldPackfile = Packfile.FromStream(oldStream, true))
                {
                    using (IPackfile newPackfile = Packfile.FromVersion(0x0A, true))
                    {
                        newPackfile.IsCompressed = true;
                        newPackfile.IsCondensed = true;

                        Stream pegStream = null;

                        var pegEntry = FindPackfileEntry(oldPackfile, ".cpeg_pc");
                        if (pegEntry != null)
                        {
                            pegStream = ProcessPeg(pegEntry, newName);
                        }


                        foreach (var file in oldPackfile.Files)
                        {
                            string extension = Path.GetExtension(file.Name);
                            IPrimitive primitive = FindPrimitive(oldContainer, file.Name);

                            switch (extension)
                            {
                                case ".ccmesh_pc":
                                    {
                                        Stream newCcmeshStream = ProcessCCMesh(file, oldName, newName);
                                        string newMeshName = newName + extension;
                                        newPackfile.AddFile(newCcmeshStream, newMeshName);
                                        primitive.Name = newMeshName;
                                        break;
                                    }

                                case ".cpeg_pc":
                                    {
                                        string newPegName = newName + extension;

                                        newPackfile.AddFile(pegStream, newPegName);
                                        primitive.Name = newPegName;
                                        break;
                                    }

                                case ".cmorph_pc":
                                    {
                                        string newFilename = newName + "_pc" + extension;
                                        newPackfile.AddFile(file.GetStream(), newFilename);
                                        primitive.Name = newFilename;
                                        break;
                                    }

                                case ".sim_pc":
                                    {
                                        string newFilename = newName + extension;
                                        Stream newStream = ProcessClothSim(file, newName);
                                        newPackfile.AddFile(newStream, newFilename);
                                        primitive.Name = newFilename;
                                        break;
                                    }

                                case ".gcmesh_pc":
                                case ".gpeg_pc":
                                case ".rig_pc":
                                    {
                                        string newFilename = newName + extension;
                                        newPackfile.AddFile(file.GetStream(), newFilename);
                                        if (primitive != null)
                                            primitive.Name = newFilename;
                                        break;
                                    }

                                default:
                                    throw new Exception(String.Format("Unrecognised extension: {0}", extension));
                            }
                        }

                        newAsm.Containers.Add(oldContainer);

                        using (Stream s = File.Create(newStr2Filename))
                        {
                            newPackfile.Save(s);
                        }

                        newPackfile.Update(oldContainer);
                    }
                }
            }

            return true;
        }

        static XElement FindCustomizationItem(string name, IGameInstance sriv)
        {
            string[] xtblNames = new string[]
            {
                "customization_items.xtbl",
                "dlc1_customization_items.xtbl",
                "dlc2_customization_items.xtbl",
                "dlc3_customization_items.xtbl",
                "dlc4_customization_items.xtbl",
                "dlc5_customization_items.xtbl",
                "dlc6_customization_items.xtbl"
            };

            string targetName = name.ToLowerInvariant();

            foreach (string xtblName in xtblNames)
            {
                using (Stream s = sriv.OpenPackfileFile(xtblName))
                {
                    XDocument xml = XDocument.Load(s);

                    var table = xml.Descendants("Table");

                    foreach (var node in table.Descendants("Customization_Item"))
                    {
                        string itemName = node.Element("Name").Value.ToLowerInvariant();

                        if (itemName == targetName)
                        {
                            return node;
                        }
                    }
                }
            }

            return null;
        }

        static Dictionary<Language, Dictionary<uint, string>> languageStrings = new Dictionary<Language, Dictionary<uint, string>>();

        static void LoadStrings(IGameInstance sriv)
        {
            var results = sriv.SearchForFiles("*.le_strings");
            foreach (var result in results)
            {
                string filename = result.Value.Filename.ToLowerInvariant();
                filename = Path.GetFileNameWithoutExtension(filename);

                string[] pieces = filename.Split('_');
                string languageCode = pieces.Last();

                Language language = LanguageUtility.GetLanguageFromCode(languageCode);

                if (!languageStrings.ContainsKey(language))
                    languageStrings.Add(language, new Dictionary<uint, string>());

                Dictionary<uint, string> strings = languageStrings[language];

                using (Stream s = sriv.OpenPackfileFile(result.Value.Filename, result.Value.Packfile))
                {
                    StringFile file = new StringFile(s, language, sriv);

                    foreach (var hash in file.GetHashes())
                    {
                        if (strings.ContainsKey(hash))
                        {
                            continue;
                        }

                        strings.Add(hash, file.GetString(hash));
                    }
                }
            }
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

            if (options.Output == null)
            {
                options.Output = options.NewName;
            }

            string outputFolder = options.Output;

            Directory.CreateDirectory(outputFolder);

            IGameInstance sriv = GameInstance.GetFromSteamId(GameSteamID.SaintsRowIV);
            LoadStrings(sriv);

            IAssetAssemblerFile newAsm;
            using (Stream newAsmStream = File.OpenRead(Path.Combine("templates", "template_customize_item.asm_pc")))
            {
                newAsm = AssetAssemblerFile.FromStream(newAsmStream);
            }

            XDocument customizationItem = null;
            using (Stream itemsTemplateStream = File.OpenRead(Path.Combine("templates", "template_customization_items.xtbl")))
            {
                customizationItem = XDocument.Load(itemsTemplateStream);
            }
            var customizationItemTable = customizationItem.Descendants("Table").First();

            XElement itemNode = FindCustomizationItem(options.Source, sriv);

            if (itemNode == null)
            {
                Console.WriteLine("Couldn't find {0}.", options.Source);
                return;
            }

            string itemName = itemNode.Element("Name").Value;
            itemNode.Element("Name").Value = options.NewName;

            string originalDisplayName = itemNode.Element("DisplayName").Value;
            uint originalStringCrc = Hashes.CrcVolition(originalDisplayName);
            string newDisplayName = "DUPLICATED_" + options.NewName.ToUpperInvariant();
            itemNode.Element("DisplayName").Value = newDisplayName;

            List<string> str2Names = new List<string>();

            bool found = false;

            var dlcElement = itemNode.Element("Is_DLC");

            if (dlcElement != null)
            {
                string isDLCString = dlcElement.Value;

                // Remove Is_DLC element so DLC items show up in SRIV
                dlcElement.Remove();
            }

            var wearOptionsNode = itemNode.Element("Wear_Options");
            foreach (var wearOptionNode in wearOptionsNode.Descendants("Wear_Option"))
            {
                var meshInformationNode = wearOptionNode.Element("Mesh_Information");
                var maleMeshFilenameNode = meshInformationNode.Element("Male_Mesh_Filename");
                var filenameNode = maleMeshFilenameNode.Element("Filename");
                string maleMeshFilename = filenameNode.Value;

                string newMaleMeshFilename = "cm_" + options.NewName + ".cmeshx";
                filenameNode.Value = newMaleMeshFilename;

                var femaleMeshFilenameNode = meshInformationNode.Element("Female_Mesh_Filename");
                filenameNode = femaleMeshFilenameNode.Element("Filename");

                string newFemaleMeshFilename = "cf_" + options.NewName + ".cmeshx";
                filenameNode.Value = newFemaleMeshFilename;

                string clothSimFilename = null;
                var clothSimFilenameNode = meshInformationNode.Element("Cloth_Sim_Filename");
                if (clothSimFilenameNode != null)
                {
                    filenameNode = clothSimFilenameNode.Element("Filename");
                    clothSimFilename = filenameNode.Value;
                    clothSimFilename = Path.ChangeExtension(clothSimFilename, ".sim_pc");
                    string newClothSimFilename = "cm_" + options.NewName + ".simx";
                    filenameNode.Value = newClothSimFilename;
                }

                var variantNodes = itemNode.Element("Variants").Descendants("Variant");
                foreach (var variantNode in variantNodes)
                {
                    var meshVariantInfoNode = variantNode.Element("Mesh_Variant_Info");
                    var variantIdNode = meshVariantInfoNode.Element("VariantID");
                    uint variantId = uint.Parse(variantIdNode.Value);
                    int crc = Hashes.CustomizationItemCrc(itemName, maleMeshFilename, variantId);

                    string maleStr2 = String.Format("custmesh_{0}.str2_pc", crc);
                    string femaleStr2 = String.Format("custmesh_{0}f.str2_pc", crc);

                    int newCrc = Hashes.CustomizationItemCrc(options.NewName, newMaleMeshFilename, variantId);

                    string newMaleStr2 = String.Format("custmesh_{0}.str2_pc", newCrc);
                    string newFemaleStr2 = String.Format("custmesh_{0}f.str2_pc", newCrc);

                    bool foundMale = ClonePackfile(sriv, maleStr2, clothSimFilename, options.Output, newAsm, itemName, "cm_" + options.NewName, Path.Combine(outputFolder, newMaleStr2));
                    bool foundFemale = ClonePackfile(sriv, femaleStr2, clothSimFilename, options.Output, newAsm, itemName, "cf_" + options.NewName, Path.Combine(outputFolder, newFemaleStr2));

                    if (foundMale || foundFemale)
                    {
                        found = true;
                    }
                }
            }

            if (found)
            {
                customizationItemTable.Add(itemNode);
            }

            using (Stream xtblOutStream = File.Create(Path.Combine(outputFolder, "customization_items.xtbl")))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Encoding = new UTF8Encoding(false);
                settings.NewLineChars = "\r\n";
                settings.Indent = true;
                settings.IndentChars = "\t";
                using (XmlWriter writer = XmlWriter.Create(xtblOutStream, settings))
                {
                    customizationItem.Save(writer);
                }
            }

            using (Stream asmOutStream = File.Create(Path.Combine(outputFolder, "customize_item.asm_pc")))
            {
                newAsm.Save(asmOutStream);
            }

            string stringXmlFolder = Path.Combine(outputFolder, "stringxml");
            Directory.CreateDirectory(stringXmlFolder);

            foreach (var pair in languageStrings)
            {
                Language language = pair.Key;
                Dictionary<uint, string> strings = pair.Value;

                StringFile file = new StringFile(1, language, sriv);

                string originalString = strings[originalStringCrc];
                string newString = "CLONE: " + originalString;

                file.AddString(newDisplayName, newString);

                string newFilename = Path.Combine(outputFolder, String.Format("{0}_{1}.le_strings", options.NewName, LanguageUtility.GetLanguageCode(language)));
                string newXmlFilename = Path.Combine(stringXmlFolder, String.Format("{0}_{1}.xml", options.NewName, LanguageUtility.GetLanguageCode(language)));

                using (Stream s = File.Create(newFilename))
                {
                    file.Save(s);
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.NewLineChars = "\r\n";

                using (XmlWriter xml = XmlTextWriter.Create(newXmlFilename, settings))
                {
                    xml.WriteStartDocument();
                    xml.WriteStartElement("Strings");
                    xml.WriteAttributeString("Language", language.ToString());
                    xml.WriteAttributeString("Game", sriv.Game.ToString());

                    xml.WriteStartElement("String");

                    xml.WriteAttributeString("Name", newDisplayName);
                    xml.WriteString(newString);

                    xml.WriteEndElement(); // String

                    xml.WriteEndElement(); // Strings
                    xml.WriteEndDocument();
                }
            }
        }
    }
}
