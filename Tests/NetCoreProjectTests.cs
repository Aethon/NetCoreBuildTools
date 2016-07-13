using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Aethon.NetCoreBuildTools;
using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class NetCoreProjectTests
    {
        private const string ProjectPath = @"c:\project.json";
        private const string EmptyJson = "{}";
        private const string BasicProjectJson = @"{ ""version"": ""1.2.3-*"" }";

        [Test]
        public void Test()
        {
            var fs = new MockFileSystem();
            fs.File.WriteAllText(ProjectPath, EmptyJson);
            var effectiveJson = NetCore.Project(ProjectPath, fs)
                .With(copyright: "new copyright")
                .Apply(() => fs.File.ReadAllText(ProjectPath));

            AssertSameJson(effectiveJson, @"{ ""copyright"": ""new copyright"" }");
            fs.File.ReadAllText(ProjectPath).Should().Be(EmptyJson);
        }

        [Test]
        public void UpdateVersion_WhenNoVersionExists_AddsTheVersion()
        {
            var fs = new MockFileSystem();
            fs.File.WriteAllText(ProjectPath, EmptyJson);

            NetCore.Project(ProjectPath, fs)
                .UpdateVersion("2.0.0");

            AssertSameJson(fs.File.ReadAllText(ProjectPath), @"{ ""version"": ""2.0.0"" }");
        }

        [Test]
        public void UpdateVersion_WhenWrongVersionExists_UpdatesTheVersion()
        {
            var fs = new MockFileSystem();
            fs.File.WriteAllText(ProjectPath, BasicProjectJson);

            NetCore.Project(ProjectPath, fs)
                .UpdateVersion("2.0.0");

            AssertSameJson(fs.File.ReadAllText(ProjectPath), @"{ ""version"": ""2.0.0-*"" }");
        }


        [Test]
        public void UpdateVersion_WhenSameVersionExists_DoesNothing()
        {
            var fs = new MockFileSystem();
            fs.File.WriteAllText(ProjectPath, BasicProjectJson);

            NetCore.Project(ProjectPath, fs)
                .UpdateVersion("1.2.3");

            fs.File.ReadAllText(ProjectPath).Should().Be(BasicProjectJson);
        }

        private static void AssertSameJson(string actual, string expected) =>
            JObject.Parse(actual).Should().BeEquivalentTo(JObject.Parse(expected));
    }
}
