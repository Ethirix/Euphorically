# Euphorically
Another Ragdoll Mod in which replicates for the Player how other Peds react when shot - using Euphoria. 

## Requirements
- GTA V
  - [C++ Script Hook V](http://www.dev-c.com/gtav/scripthookv/)
  - [Script Hook V .NET](https://github.com/crosire/scripthookvdotnet)
  - [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)
  - [Visual C++ Redistribution x64](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads)
- Scripting
  - [Script Hook V .NET 3](https://www.nuget.org/packages/ScriptHookVDotNet3/)

## Usage
Once pulled from Github, ensure that `Script Hook V .NET 3` is installed - if so, compiling should just work.

If you wish to have the .dll and .ini files to be automatically moved to the SHVDN scripts folder, please create a file called `GTA Folder Path.txt` in the `Euphorically` folder.

Such as: `..\Euphorically\Euphorically\GTA Folder Path.txt`

Then provide the file with your path to your GTA V install and do not give the path a trailing backslash on the path.

Example: `C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V`
