using System;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Aethon.NetCoreBuildTools
{
    public sealed class NetCoreProject
    {
        private readonly IFileSystem _fileSystem;

        public string ProjectPath { get; }
        public string Copyright { get; }
        public string ProjectUrl { get; }
        public string LicenseUrl { get; }
        public string IconUrl { get; }

        public NetCoreProject(string projectPath)
            : this(projectPath, null)
        {
        }

        public NetCoreProject(string projectPath, IFileSystem fileSystem)
        {
            if (string.IsNullOrWhiteSpace(projectPath))
                throw new ArgumentException("must contain the path to the project file", nameof(projectPath));
            ProjectPath = projectPath;

            _fileSystem = fileSystem ?? new FileSystem();
        }

        private NetCoreProject(string projectPath, string copyright, string projectUrl, string licenseUrl, string iconUrl, IFileSystem filesystem)
        {
            ProjectPath = projectPath;
            Copyright = copyright;
            ProjectUrl = projectUrl;
            LicenseUrl = licenseUrl;
            IconUrl = iconUrl;
            _fileSystem = filesystem;
        }

        public NetCoreProject With(string copyright = null, string projectUrl = null, string licenseUrl = null, string iconUrl = null) =>
            new NetCoreProject(ProjectPath, copyright ?? Copyright, projectUrl ?? ProjectUrl, licenseUrl ?? LicenseUrl, iconUrl ?? IconUrl, _fileSystem);

        public T Apply<T>(Func<T> action)
        {
            var text = _fileSystem.File.ReadAllText(ProjectPath);
            var json = JObject.Parse(text);
            Set(json, "copyright", Copyright);
            Set(json, "projectUrl", ProjectUrl);
            Set(json, "licenseUrl", LicenseUrl);
            Set(json, "iconUrl", IconUrl);

            _fileSystem.File.WriteAllText(ProjectPath, json.ToString());
            try
            {
                return action();
            }
            finally
            {
                _fileSystem.File.WriteAllText(ProjectPath, text);
            }
        }

        private static readonly Regex SuffixPattern = new Regex(@"^([0-9]+(?:\.[0-9]+)*)(.*)$");

        public void UpdateVersion(string version)
        {
            var text = _fileSystem.File.ReadAllText(ProjectPath);
            var json = JObject.Parse(text);
            var currentVersion = json["version"];
            if (currentVersion != null)
            {
                var match = SuffixPattern.Match(currentVersion.ToString());
                if (match.Success)
                {
                    var v = match.Groups[1].Value;
                    if (v == version)
                        version = null;
                    else
                        version += match.Groups[2].Value;
                }
            }

            if (version != null)
            {
                json["version"] = version;
                _fileSystem.File.WriteAllText(ProjectPath, json.ToString());
            }
        }

        private static void Set(JObject json, string name, string value)
        {
            if (value != null)
                json[name] = value;
        }
    }
}