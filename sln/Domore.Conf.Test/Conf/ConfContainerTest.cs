using NUnit.Framework;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Domore.Conf {
    [TestFixture]
    public class ConfContainerTest {
        string TempFileName;
        IConfContainer Subject;

        [SetUp]
        public void SetUp() {
            Subject = new ConfContainer();
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

        class TypeConverterHelper : TypeConverter {
            public class MyTypeConverter : TypeConverter {
                public object Value { get; set; }
                public object Conversion { get; set; }
                public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
                    Value = value;
                    return Conversion;
                }
            }
            public class MyConfTypeConverter : ConfTypeConverter {
                public IConfBlock Conf { get; set; }
                public object Value { get; set; }
                public object Conversion { get; set; }
                public override object ConvertFrom(IConfBlock conf, ITypeDescriptorContext context, CultureInfo culture, object value) {
                    Conf = conf;
                    Value = value;
                    return Conversion;
                }
            }
            public class ConvertedType { }
            public class ConfiguredType {
                public ConvertedType Property { get; set; }
            }
        }

        [Test]
        public void TypeConverter_UsedDuringConfiguration() {
            var expected = new TypeConverterHelper.ConvertedType();
            Subject.TypeConverter[typeof(TypeConverterHelper.ConvertedType)] = new TypeConverterHelper.MyTypeConverter { Conversion = expected };
            Subject.Content = "helper.property = _";
            var configuredInstance = Subject.Configure(new TypeConverterHelper.ConfiguredType(), "helper");
            Assert.AreSame(expected, configuredInstance.Property);
        }

        [Test]
        public void TypeConverter_IsPassedValue() {
            var expected = "_expected_";
            var converter = new TypeConverterHelper.MyTypeConverter();
            Subject.TypeConverter[typeof(TypeConverterHelper.ConvertedType)] = converter;
            Subject.Content = $"helper.property = {expected}";
            Subject.Configure(new TypeConverterHelper.ConfiguredType(), "helper");
            Assert.AreEqual(expected, converter.Value);
        }

        [Test]
        public void TypeConverter_IsPassedConf() {
            var converter = new TypeConverterHelper.MyConfTypeConverter();
            Subject.TypeConverter[typeof(TypeConverterHelper.ConvertedType)] = converter;
            Subject.Content = "helper.property = _";
            Subject.Configure(new TypeConverterHelper.ConfiguredType(), "helper");
            Assert.AreSame(Subject.Block, converter.Conf);
        }
    }
}
