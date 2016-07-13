# NetCoreBuildTools
Tools to help maintain a sane build pipeline as .NET Core matures.

## CI Versioning

The current `project.json` structure does not really follow CI principles in that, to update the package version number, you must modify a checked-in source file.

To update the project.json version:

```csharp
using Aethon.NetCoreBuildTools;

...

  NetCore.Project(@"c:\project\myproject\project.json")
    .UpdateVersion("1.2.3");
```

This will only write to the project file if the version is not present or does not match the specified version. It will also preserve the suffix pattern (e.g. "`-*`") specified in the project file.

## Package Metadata

Again, the current `pack` process does not afford much flexibility. All package metadata, such as `copyright` and  `licenseUrl`
must appear in the `project.json` file and cannot be overridden. To temporarily rewrite the project file:
```csharp
using Aethon.NetCoreBuildTools;

...

  NetCore.Project(@"c:\project\myproject\project.json")
    .With(copyright: "Copyright (c) 2016 ME!")
    .Apply(() => Execute(@"dotnet pack c:\project\myproject\project.json"));
```

* `NetCore.Project` locates the `project.json` file.
* `.With` specifies one or more project properties to replace. `copyright`, `projectUrl`, `iconUrl`, and `licenseUrl` are currently supported.
* `.Apply` rewrites the project file, executes the action, and then restores the project to its original state.


## Installing for a .NET Core Project

Add a dependency on the [NuGet package](https://www.nuget.org/packages/Aethon.NetCoreBuildTools.CakeAddin/).

## Installing for a Cake Build Script

Reference the NuGet package with an `#addin` preprocessor.

```csharp
#addin Aethon.NetCoreBuildTools.CakeAddin
```

