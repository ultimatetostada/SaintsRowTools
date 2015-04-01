# ThomasJepp.SaintsRow
A .NET library for Saints Row file formats and associated tools for Saints Row.

## Supported file formats
The ThomasJepp.SaintsRow library supports the following formats:

| File type | Extensions | Support |
|-----------|------------|---------|
| Saints Row 2 PC Package files | .vpp_pc | Partial read only support |
| Saints Row IV and Saints Row: Gat out of Hell PC Package files | .vpp_pc, .str2_pc | Full read and write support |
| Saints Row IV and Saints Row: Gat out of Hell PC Asset Assembler files | .asm_pc | Full read and write support |
| Saints Row 2 PC Language Strings files | .le_strings | Full read and write support |
| Saints Row: The Third, Saints Row IV and Saints Row: Gat out of Hell Language strings files | .le_strings | Full read and write support |
| Saints Row: The Third, Saints Row IV and Saints Row: Gat out of Hell Streaming Soundbank files | ..._media.bnk_pc | Full read and write support |
| Saints Row: The Third, Saints Row IV and Saints Row: Gat out of Hell Wwise Soundbank files | .bnk_pc | Partial read only support |

## Included tools
### Packages
 - **ThomasJepp.SaintsRow.BuildPackfile:** a command line tool for building Package files. Can also update Asset Assembler files.
 - **ThomasJepp.SaintsRow.BuildPackfileGUI:** a GUI tool equivalent to **ThomasJepp.SaintsRow.BuildPackfile**.
 - **ThomasJepp.SaintsRow.ExtractPackfile:** a command line tool for extracting Package files.
 - **ThomasJepp.SaintsRow.ExtractPackfileGUI:** a GUI tool equivalent to **ThomasJepp.SaintsRow.ExtractPackfile**.
 - **ThomasJepp.SaintsRow.RecursiveExtractor:** a command line tool for extracting all of the contents of a specified set of package files.
 - **ThomasJepp.SaintsRow.Stream2:** a command line tool for debugging and updating Asset Assembler files.
 - **ThomasJepp.SaintsRow.Stream2Update:** a command line tool for automatically updating Asset Assembler files based on the current package files.
 
### Language Strings
 - **ThomasJepp.SaintsRow.BuildStrings:** a command line tool that builds a language strings file from a text file in the right format.
 - **ThomasJepp.SaintsRow.ExtractStrings:** a command line tool that converts a language strings file to a text file that can be easily edited.

### Audio
 - **ThomasJepp.SaintsRow.BuildStreamingSoundbank:** a command line tool for building a streaming soundbank from a set of .wem wwise audio tracks and an xml definition file
 - **ThomasJepp.SaintsRow.ExtractStreamingSoundbank:** a command line tool for extracting a streaming soundbank - creates a set of .wem wwise audio tracks, converted .ogg audio tracks, metadata files and an xml definition file

### Other tools
 - **Saints Row IV Debug Print Hook:** a DLL file that provides simple output for Lua debug_print messages for Saints Row IV.

## Licenses
 - CmdLine is licensed under the Microsoft Public License as per https://www.nuget.org/packages/CmdLine. The license is included in the repository. CmdLine is used by the command line programs included in this repository.
 - The Saints Row IV Debug Print Hook is based on the Saints Row: The Third Debug Print Hook. The original license for the Saints Row: The Third Debug Print hook is also included in the repository.
 - All other code is licensed under the license included in the repository as "license.txt". A plain english explaination of what you need to do to comply with this license is included in "license_notes.txt".
