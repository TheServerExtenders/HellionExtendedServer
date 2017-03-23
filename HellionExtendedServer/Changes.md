# Changes To This Version

- Version now 0.1.3.0
- Removed NLog.config
  - Created in-code via NLog's LoggingConfiguration in the LogManager.cs

## Project Changes

- Edited Project file to generate a *VERSION* file
  - File is created with a custom PostBuildMacro $(VersionNumber)
  
- PreBuildEvent now creates Hes folders in project output directory
- PostBuildEvent now copies required binaries to Hes/bin

- Updated NLog to latest version

*Use this file to save all changes made in this branch*
