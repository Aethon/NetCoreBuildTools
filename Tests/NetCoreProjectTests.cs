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

        [Test]
        public void Test()
        {
            var fs = new MockFileSystem();
            fs.File.WriteAllText(ProjectPath, EmptyJson);
            var effectiveJson = NetCore.Project(ProjectPath, fs)
                .With(version: "2.0.0-*")
                .Apply(() => fs.File.ReadAllText(ProjectPath));

            AssertSameJson(effectiveJson, @"{ ""version"": ""2.0.0-*"" }");
            fs.File.ReadAllText(ProjectPath).Should().Be(EmptyJson);
        }

        private static void AssertSameJson(string actual, string expected) =>
            JObject.Parse(actual).Should().BeEquivalentTo(JObject.Parse(expected));
    }
}
