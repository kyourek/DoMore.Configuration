﻿using System.IO;
using System.Linq;

using NUnit.Framework;

namespace Domore.Configuration.Contents {
    [TestFixture]
    public class FileContentsProviderTest {
        string TempFileName;
        FileContentsProvider Subject;

        [SetUp]
        public void SetUp() {
            Subject = new FileContentsProvider();
        }

        [TearDown]
        public void TearDown() {
            if (File.Exists(TempFileName)) {
                File.Delete(TempFileName);
            }
        }

        [Test]
        public void GetConfigurationContents_ReadsFromFile() {
            TempFileName = Path.GetTempFileName();
            File.WriteAllText(TempFileName, "prop1 = val2");
            var contents = Subject.GetConfigurationContents(TempFileName);
            Assert.AreEqual("val2", contents.First().Value);
        }
    }
}
