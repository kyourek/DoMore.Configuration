using System.IO;
using System.Linq;

using NUnit.Framework;

namespace Domore.Configuration.Contents {
    [TestFixture]
    public class FileContentsFactoryTest {
        string TempFileName;
        FileContentsFactory Subject;

        [SetUp]
        public void SetUp() {
            Subject = new FileContentsFactory();
        }

        [TearDown]
        public void TearDown() {
            if (File.Exists(TempFileName)) {
                File.Delete(TempFileName);
            }
        }

        [Test]
        public void CreateConfigurationContents_ReadsFromFile() {
            TempFileName = Path.GetTempFileName();
            File.WriteAllText(TempFileName, "prop1 = val2");
            var contents = Subject.CreateConfigurationContents(TempFileName);
            Assert.AreEqual("val2", contents.First().Value);
        }
    }
}
