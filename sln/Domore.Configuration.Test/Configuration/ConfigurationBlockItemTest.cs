using NUnit.Framework;
using System;

namespace Domore.Configuration {
    [TestFixture]
    public class ConfigurationBlockItemTest {
        string Content;
        IConfigurationBlockItem _Subject;
        IConfigurationBlockItem Subject {
            get => _Subject ?? (_Subject = new ConfigurationBlockFactory().CreateConfigurationBlock(Content).Item(0));
            set => _Subject = value;
        }

        [SetUp]
        public void SetUp() {
            Subject = null;
        }

        [TestCase(typeof(int), "1", 1)]
        [TestCase(typeof(int), "321", 321)]
        [TestCase(typeof(int), "-321", -321)]
        [TestCase(typeof(bool), "true", true)]
        [TestCase(typeof(bool), "false", false)]
        [TestCase(typeof(bool), "TRue", true)]
        [TestCase(typeof(bool), "faLSe", false)]
        [TestCase(typeof(string), "1", "1")]
        [TestCase(typeof(string), "321", "321")]
        [TestCase(typeof(string), "hello", "hello")]
        [TestCase(typeof(double), "1", 1.0)]
        [TestCase(typeof(double), "321", 321.0)]
        [TestCase(typeof(double), "123.456", 123.456)]
        [TestCase(typeof(double), "-123.456", -123.456)]
        public void ConvertValue_ConvertsToType(Type type, string value, object expected) {
            Content = $"value = {value}";
            var actual = Subject.ConvertValue(type);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("1", 1)]
        [TestCase("321", 321)]
        [TestCase("-321", -321)]
        public void ConvertValue_ConvertsToInt32(string value, int expected) {
            Content = $"value = {value}";
            var actual = Subject.ConvertValue<int>();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("1", "1")]
        [TestCase("321", "321")]
        [TestCase("hello", "hello")]
        public void ConvertValue_ConvertsToString(string value, string expected) {
            Content = $"value = {value}";
            var actual = Subject.ConvertValue<string>();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("321", 321.0)]
        [TestCase("123.456", 123.456)]
        [TestCase("-123.456", -123.456)]
        public void ConvertValue_ConvertsToDouble(string value, double expected) {
            Content = $"value = {value}";
            var actual = Subject.ConvertValue<double>();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("TRue", true)]
        [TestCase("faLSe", false)]
        public void ConvertValue_ConvertsToBool(string value, bool expected) {
            Content = $"value = {value}";
            var actual = Subject.ConvertValue<bool>();
            Assert.AreEqual(expected, actual);
        }

        class Kid { public Pet Pet { get; set; } }
        class Pet { }
        class Cat : Pet { }

        [Test]
        public void Configure_CreatesInstanceOfType() {
            Content = @"value = Domore.Configuration.ConfigurationBlockItemTest+Cat, Domore.Configuration.Test";
            var pet = Subject.ConvertValue<Pet>();
            Assert.That(pet, Is.InstanceOf(typeof(Cat)));
        }

        [Test]
        public void Configure_CreatesTypeInstance() {
            Content = @"value = Domore.Configuration.ConfigurationBlockItemTest+Cat, Domore.Configuration.Test";
            var type = Subject.ConvertValue<Type>();
            Assert.That(type, Is.EqualTo(typeof(Cat)));
        }
    }
}
