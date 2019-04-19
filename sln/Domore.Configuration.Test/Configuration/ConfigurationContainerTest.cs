using NUnit.Framework;
using System.IO;

namespace Domore.Configuration {
    [TestFixture]
    public class ConfigurationContainerTest {
        string TempFileName;
        IConfigurationContainer Subject;

        [SetUp]
        public void SetUp() {
            Subject = new ConfigurationContainer();
        }

        [TearDown]
        public void TearDown() {
            if (File.Exists(TempFileName)) {
                File.Delete(TempFileName);
            }
        }

        [Test]
        public void ContentsChanged_RaisedWhenContentSet() {
            var changed = 0;
            Subject.ContentsChanged += (s, e) => changed++;
            Subject.Content = "some\r\nconfig=1";
            Assert.AreEqual(1, changed);
        }

        [Test]
        public void ContentsChanged_RaisedWhenContentChanges() {
            var changed = 0;
            Subject.ContentsChanged += (s, e) => changed++;
            Subject.Content = "some\r\nconfig=1";
            TempFileName = Path.GetTempFileName();
            Subject.Content = TempFileName;
            Assert.AreEqual(2, changed);
        }
    }
}
