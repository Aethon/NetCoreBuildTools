using System.IO.Abstractions;

namespace Aethon.NetCoreBuildTools
{
    public static class NetCore
    {
        public static NetCoreProject Project(string projectPath) =>
            new NetCoreProject(projectPath);

        public static NetCoreProject Project(string projectPath, IFileSystem fileSystem) =>
            new NetCoreProject(projectPath, fileSystem);
    }
}
