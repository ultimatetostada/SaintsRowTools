# Saints Row Tools
A collection of modding tools for modifying the Saints Row series of games.

## Included tools
### Packages
 - **ThomasJepp.SaintsRow.BuildPackfile:** a command line tool for building Package files. Can also update Asset Assembler files.
 - **ThomasJepp.SaintsRow.BuildPackfileGUI:** a GUI tool equivalent to **ThomasJepp.SaintsRow.BuildPackfile**.
 - **ThomasJepp.SaintsRow.EditPackfile:** a command line tool for swapping indvidual files inside a package file.
 - **ThomasJepp.SaintsRow.ExtractPackfile:** a command line tool for extracting Package files.
 - **ThomasJepp.SaintsRow.ExtractPackfileGUI:** a GUI tool equivalent to **ThomasJepp.SaintsRow.ExtractPackfile**.
 - **ThomasJepp.SaintsRow.RecursiveExtractor:** a command line tool for extracting all of the contents of a specified set of package files.
 - **ThomasJepp.SaintsRow.Stream2:** a command line tool for debugging and updating Asset Assembler files.
 - **ThomasJepp.SaintsRow.Stream2Update:** a command line tool for automatically updating Asset Assembler files based on the current package files.
 
### Language Strings
 - **ThomasJepp.SaintsRow.BuildStrings:** a command line tool that builds a language strings file from an XML file.
 - **ThomasJepp.SaintsRow.ExtractStrings:** a command line tool that converts a language strings file to an XML file that can be easily edited.

### Audio
 - **ThomasJepp.SaintsRow.BuildStreamingSoundbank:** a command line tool for building a streaming soundbank from a set of .wem wwise audio tracks and an xml definition file
 - **ThomasJepp.SaintsRow.ExtractStreamingSoundbank:** a command line tool for extracting a streaming soundbank - creates a set of .wem wwise audio tracks, converted .ogg audio tracks, metadata files and an xml definition file

### Other tools
 - **CustomizationItemClone:** a command line tool for cloning customization items so you can edit a clone rather than replace the original.
 - **CustomizationItemHashTool:** a GUI tool for generating the filenames needed for customization items (hair, clothing, etc).
 - **HashTester:** a GUI tool for testing various hash algoritms
 - **RepackTest:** a command line tool for automating the rebuilding of all packages in a Saints Row game (used for testing package file and asset assembler file implementations)

## Licenses
 - CmdLine is licensed under the Microsoft Public License as per https://www.nuget.org/packages/CmdLine. The license is included in the repository. CmdLine is used by the command line programs included in this repository.
 - All other code is licensed under the license included in the repository as "license.txt". A plain english explaination of what you need to do to comply with this license is included in "license_notes.txt".
