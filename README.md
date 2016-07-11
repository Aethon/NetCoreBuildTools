# NetCoreBuildTools
Tools to help maintain a sane build pipeline as .NET Core matures.

## CI Versioning

The current `project.json` structure does not really follow CI principles in that, to update the package version number, you must modify a checked-in source file.

To manage that for now, you can do:

```csharp
#addin nuget?:package=Aethon.NetCoreBuildTools

...

  ProjectFiles.Select(f =>
    NetCore.Project(f)
      .With(version: buildingVersion)
      .Apply(() =>
	  		DotNetCore.Pack(f, new DotNetCorePackSettings {
				  NoBuild = true,
				  OutputDirectory = ArtifactsFolder,
			  });
			FileWriteText(f, text);
		}));

...
```

`NetCore.Project` locates the `project.json` file. `.With` specifies one or more project properties to replace. `.Apply` rewrites the project file, executes the action, and then restores the project to its original state.
