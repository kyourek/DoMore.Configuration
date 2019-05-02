using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Domore.Conf.Providers {
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
        public void GetConfContents_ReadsFromFile() {
            TempFileName = Path.GetTempFileName();
            File.WriteAllText(TempFileName, "prop1 = val2");
            var contents = Subject.GetConfContents(TempFileName);
            Assert.AreEqual("val2", contents.First().Value);
        }
    }
}
