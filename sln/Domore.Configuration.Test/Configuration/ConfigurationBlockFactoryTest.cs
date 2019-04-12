using System;
using System.IO;

using NUnit.Framework;

namespace Domore.Configuration {
    [TestFixture]
    public class ConfigurationBlockFactoryTest {
        private string TempFileName;
        private ConfigurationBlockFactory Subject;

        [SetUp]
        public void SetUp() {
            Subject = new ConfigurationBlockFactory();
        }

        [TearDown]
        public void TearDown() {
            if (File.Exists(TempFileName)) {
                File.Delete(TempFileName);
            }
        }

        [Test]
        public void Configuration_EqualsArgument() {
            var configuration = "this is some\r\nConfiguration=good";
            TempFileName = Path.GetTempFileName();
            File.WriteAllText(TempFileName, configuration);
            var block = Subject.CreateConfigurationBlock(TempFileName);
            Assert.AreEqual(TempFileName, block.Configuration);
        }

        [Test]
        public void ConfigurationContent_EqualsConfiguration() {
            var configuration = "here=the config\r\nand new line";
            var block = Subject.CreateConfigurationBlock(configuration);
            Assert.AreEqual(configuration, block.ConfigurationContent);
        }

        [Test]
        public void ConfigurationContent_IsLoadedFromFile() {
            var configuration = "this is some\r\nConfiguration=good";
            TempFileName = Path.GetTempFileName();
            File.WriteAllText(TempFileName, configuration);
            var block = Subject.CreateConfigurationBlock(TempFileName);
            Assert.AreEqual(configuration, block.ConfigurationContent);
        }

        [Test]
        public void ItemCount_ReturnsItemCount() {
            var config = @"
                val1 = hello, world!
                val2 = goodbye, earth..?
                val3 = 3.14
            ";
            var block = Subject.CreateConfigurationBlock(config);
            Assert.AreEqual(3, block.ItemCount);
        }

        [Test]
        public void ItemExists_TrueIfIndexExists() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\nval3 = 3.14";
            var block = Subject.CreateConfigurationBlock(config);
            Assert.IsTrue(block.ItemExists(2));
        }

        [Test]
        public void ItemExists_FalseIfIndexDoesNotExist() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n";
            var block = Subject.CreateConfigurationBlock(config);
            Assert.IsFalse(block.ItemExists(2));
        }

        [Test]
        public void ItemExists_TrueIfKeyExists() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n VAL3 = 3.14";
            var block = Subject.CreateConfigurationBlock(config);
            Assert.IsTrue(block.ItemExists("val 3"));
        }

        [Test]
        public void ItemExists_FalseIfKeyDoesNotExist() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n ";
            var block = Subject.CreateConfigurationBlock(config);
            Assert.IsFalse(block.ItemExists("val 3"));
        }

        [Test]
        public void Item_GetsItemByIndex() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var block = Subject.CreateConfigurationBlock(config);
            var item = block.Item(2);
            Assert.AreEqual("Val 3", item.OriginalKey);
            Assert.AreEqual("val3", item.NormalizedKey);
            Assert.AreEqual("3.14", item.OriginalValue);
        }

        [Test]
        public void Item_GetsItemByIndex1() {
            var config = "val1 = hello, world!\r\n \tVAL  2 = goodbye, earth..?  \n Val 3 = 3.14";
            var block = Subject.CreateConfigurationBlock(config);
            var item = block.Item(1);
            Assert.AreEqual("VAL  2", item.OriginalKey);
            Assert.AreEqual("val2", item.NormalizedKey);
            Assert.AreEqual("goodbye, earth..?", item.OriginalValue);
        }

        [Test]
        public void Item_GetsItemByKey() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var block = Subject.CreateConfigurationBlock(config);
            var item = block.Item("VAL  \t3");
            Assert.AreEqual("Val 3", item.OriginalKey);
            Assert.AreEqual("val3", item.NormalizedKey);
            Assert.AreEqual("3.14", item.OriginalValue);
        }

        [Test]
        public void Item_GetsItemByNormalizedKey() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var block = Subject.CreateConfigurationBlock(config);
            var item = block.Item("val3");
            Assert.AreEqual("Val 3", item.OriginalKey);
            Assert.AreEqual("val3", item.NormalizedKey);
            Assert.AreEqual("3.14", item.OriginalValue);
        }

        [Test]
        public void Item_GetsItemByIndexFromFile() {
            TempFileName = Path.GetTempFileName();
            File.WriteAllText(TempFileName, @"
                Connection string = here-is-the;connection
                station name = mux1
            ");
            var block = Subject.CreateConfigurationBlock(TempFileName);
            var item = block.Item(0);
            Assert.AreEqual("Connection string", item.OriginalKey);
            Assert.AreEqual("connectionstring", item.NormalizedKey);
            Assert.AreEqual("here-is-the;connection", item.OriginalValue);
        }

        [Test]
        public void StringValue_IsOriginalValue() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var block = Subject.CreateConfigurationBlock(config);
            var item = block.Item("val3");
            Assert.AreEqual(item.OriginalValue, item.StringValue);
        }

        [Test]
        public void IntegerValue_ConvertsToInt() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 4";
            var block = Subject.CreateConfigurationBlock(config);
            var item = block.Item("val3");
            Assert.AreEqual(4, item.IntegerValue);
        }

        [Test]
        public void IntegerValue_ThrowsExceptionIfNotAnInteger() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var block = Subject.CreateConfigurationBlock(config);
            var item = block.Item("val3");
            var error = default(Exception);
            try {
                var i = item.IntegerValue;
            }
            catch (Exception ex) {
                error = ex;
            }
            Assert.IsNotNull(error);
        }

        [Test]
        public void NumberValue_ConvertsToDouble() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var block = Subject.CreateConfigurationBlock(config);
            var item = block.Item("val3");
            Assert.AreEqual(3.14, item.NumberValue);
        }

        [Test]
        public void NumberValue_ThrowsExceptionIfNotNumeric() {
            var config = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var block = Subject.CreateConfigurationBlock(config);
            var item = block.Item("val2");
            var error = default(Exception);
            try {
                var i = item.NumberValue;
            }
            catch (Exception ex) {
                error = ex;
            }
            Assert.IsNotNull(error);
        }

        [Test]
        public void Item_CanBeSeparatedBySemicolon() {
            var config = "val1 = hello, world!;val2 = goodbye, earth..?; Val 3 = 3.14  ";
            var block = Subject.CreateConfigurationBlock(config);
            Assert.AreEqual(3, block.ItemCount);
            Assert.AreEqual("goodbye, earth..?", block.Item("val 2").OriginalValue);
        }

        [Test]
        public void Item_NewLinePrecedesSemicolonSeparation() {
            var config = "val1 = hello, world!\n val2 = goodbye, earth..?; Val 3 = 3.14  ";
            var block = Subject.CreateConfigurationBlock(config);
            Assert.AreEqual(2, block.ItemCount);
            Assert.AreEqual("goodbye, earth..?; Val 3 = 3.14", block.Item("val 2").OriginalValue);
        }
    }
}
