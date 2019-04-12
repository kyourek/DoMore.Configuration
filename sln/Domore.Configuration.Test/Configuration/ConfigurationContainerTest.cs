using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

namespace Domore.Configuration {
    [TestFixture]
    public class ConfigurationContainerTest {
        private string TempFileName;
        private IConfigurationContainer Subject;

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
        public void Changed_RaisedWhenContentChanges() {
            var changed = 0;
            Subject.Changed += (s, e) => changed++;
            Subject.Content = "some\r\nconfig=1";
            Assert.AreEqual(1, changed);
            TempFileName = Path.GetTempFileName();
            Subject.Content = TempFileName;
            Assert.AreEqual(2, changed);
        }

        [Test]
        [TestCase("true", true)]
        [TestCase("FALsE", false)]
        public void Value_GetsBooleanValue(string value, bool expected) {
            Subject.Content = "v=" + value;
            Assert.AreEqual(expected, Subject.Value("v", out bool b));
            Assert.AreEqual(expected, b);
        }

        [Test]
        public void Value_ThrowsExceptionIfKeyNotFound() {
            Subject.Content = "v1 = value";
            Assert.Throws<KeyNotFoundException>(() => Subject.Value<string>("v2"));
        }

        [Test]
        public void Value_ReturnsDefaultIntegerIfKeyNotFound() {
            Subject.Content = "v1 = 5";
            var actual = Subject.Value("v2", def: 100);
            var expected = 100;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Value_ReturnsActualIntegerIfKeyFound() {
            Subject.Content = "v1 = 5;v2 = 31";
            var actual = Subject.Value("v2", def: 100);
            var expected = 31;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Value_ReturnsDefaultNumberIfKeyNotFound() {
            Subject.Content = "val1 = somestr\n num2 = 12.34";
            var actual = Subject.Value("num1", out double d, def: 34.56);
            var expected = 34.56;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, d);
        }

        [Test]
        public void Value_ReturnsActualNumberIfKeyFound() {
            Subject.Content = "val1 = somestr\n num1 = 12.34";
            var actual = Subject.Value("num1", out double d);
            var expected = 12.34;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, d);
        }

        private class Configure_SetsObjectProperties_Class {
            public bool YesNo { get; set; }
            public int HowMany { get; set; }
            public double HowMuch { get; set; }
            public string Why { get; set; }
        }

        [Test]
        public void Configure_SetsObjectProperties() {
            Subject.Content = @"
                thing.yes no = true
                thing.Howmany = 33987
                Thing . how Much = 14.93445
                thing. why =    because
            ";
            var obj = Subject.Configure(new Configure_SetsObjectProperties_Class(), "Thing");
            Assert.AreEqual(true, obj.YesNo);
            Assert.AreEqual(33987, obj.HowMany);
            Assert.AreEqual(14.93445, obj.HowMuch);
            Assert.AreEqual("because", obj.Why);
        }

        private class Configure_DoesNotSetFields_Class {
            public string ShouldNotBeSet;
            public string ShouldBeSet { get; set; }
        }

        [Test]
        public void Configure_DoesNotSetFields() {
            Subject.Content = @"
                Configure_DoesNotSetFields_Class.ShouldBeSet = yup
                Configure_DoesNotSetFields_Class.ShouldNotBeSet = nope
            ";

            var obj = new Configure_DoesNotSetFields_Class();
            obj.ShouldBeSet = "blank1";
            obj.ShouldNotBeSet = "blank2";
            Subject.Configure(obj);
            Assert.AreEqual("yup", obj.ShouldBeSet);
            Assert.AreEqual("blank2", obj.ShouldNotBeSet);
        }

        private class Configure_SetsObjectPropertyProperties_Class {
            public double D { get; set; }
            public Inner Mine { get; } = new Inner();

            public class Inner {
                public string S { get; set; }
            }
        }

        [Test]
        public void Configure_SetsObjectPropertyProperties() {
            Subject.Content = @"
                Configure_SetsObjectPropertyProperties_Class.D = 6543
                Configure_SetsObjectPropertyProperties_Class.Mine.S = inner-value
                Configure_SetsObjectPropertyProperties_Class.Mine.Junk = Does not exist
            ";
            var obj = Subject.Configure(new Configure_SetsObjectPropertyProperties_Class());
            Assert.AreEqual(6543, obj.D);
            Assert.AreEqual("inner-value", obj.Mine.S);
        }

        private class Configure_SetsPropertyValueOfTypeType_Class {
            public Type TypeProperty { get; set; }
            public class TheType {
            }
        }

        [Test]
        public void Configure_SetsPropertyValueOfTypeType() {
            Subject.Content = "thing.type property = DOMORE.Configuration.ConfigurationContainerTest+Configure_setsPropertyValueOfTypeType_Class+thetype, Domore.Configuration.test";
            var obj = Subject.Configure(key: "thing", obj: new Configure_SetsPropertyValueOfTypeType_Class());
            Assert.AreEqual(typeof(Configure_SetsPropertyValueOfTypeType_Class.TheType), obj.TypeProperty);
        }

        private class Configure_CreatesNewInstanceOfPropertyFromType_Class {
            public Collection Coll { get; } = new Collection();
            public class Collection {
                public readonly Dictionary<int, TheParentType> Dict = new Dictionary<int, TheParentType>();

                public TheParentType this[int key] {
                    get { return Dict[key]; }
                    set { Dict[key] = value; }
                }
            }
            public class TheParentType {
            }
            public class TheChildType : TheParentType {
            }
        }

        [Test]
        public void Configure_CreatesNewInstanceOfPropertyFromType() {
            Subject.Content = @"
                conf.coll[4] = Domore.Configuration.ConfigurationContainerTest+Configure_CreatesNewInstanceOfPropertyFromType_Class+thechildtype, Domore.Configuration.Test
                conf.coll[11] = Domore.Configuration.ConfigurationContainerTest+Configure_CreatesNewInstanceOfPropertyFromType_Class+theparenttype, Domore.Configuration.Test
            ";
            var obj = Subject.Configure(key: "conf", obj: new Configure_CreatesNewInstanceOfPropertyFromType_Class());
            Assert.AreEqual(2, obj.Coll.Dict.Count);
            Assert.AreEqual(typeof(Configure_CreatesNewInstanceOfPropertyFromType_Class.TheChildType), obj.Coll[4].GetType());
            Assert.AreEqual(typeof(Configure_CreatesNewInstanceOfPropertyFromType_Class.TheParentType), obj.Coll[11].GetType());
        }
    }
}
