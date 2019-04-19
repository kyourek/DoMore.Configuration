using System;
using System.ComponentModel;
using System.Globalization;

using NUnit.Framework;

namespace Domore.Configuration {
    using Contents;

    [TestFixture]
    public class ConfigurationBlockTest {
        object Content;
        IConfigurationBlock _Subject;
        IConfigurationBlock Subject {
            get => _Subject ?? (_Subject = new ConfigurationBlockFactory().CreateConfigurationBlock(Content, new TextContentsProvider()));
            set => _Subject = value;
        }

        [SetUp]
        public void SetUp() {
            Subject = null;
        }

        [Test]
        public void Content_EqualsConfiguration() {
            Content = "here=the config\r\nand new line";
            Assert.AreEqual(Content, Subject.Content);
        }

        [Test]
        public void ToString_IsContent() {
            Content = string.Join(Environment.NewLine, new[] { "this is some = good config", "Configuration = good" });
            Assert.AreEqual(Content, Subject.ToString());
        }

        [Test]
        public void ItemCount_ReturnsItemCount() {
            Content = @"
                val1 = hello, world!
                val2 = goodbye, earth..?
                val3 = 3.14
            ";
            Assert.AreEqual(3, Subject.ItemCount);
        }

        [Test]
        public void ItemExists_TrueIfIndexExists() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\nval3 = 3.14";
            Assert.IsTrue(Subject.ItemExists(2));
        }

        [Test]
        public void ItemExists_FalseIfIndexDoesNotExist() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n";
            Assert.IsFalse(Subject.ItemExists(2));
        }

        [Test]
        public void ItemExists_TrueIfKeyExists() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n VAL3 = 3.14";
            Assert.IsTrue(Subject.ItemExists("val 3"));
        }

        [Test]
        public void ItemExists_FalseIfKeyDoesNotExist() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n ";
            Assert.IsFalse(Subject.ItemExists("val 3"));
        }

        [TestCase(0, "val1")]
        [TestCase(1, "val2")]
        [TestCase(2, "Val 3")]
        public void Item_OriginalKeyIsSet(object key, string expected) {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var item = Subject.Item(key);
            Assert.AreEqual(expected, item.OriginalKey);
        }

        [TestCase(0, "val1")]
        [TestCase(1, "val2")]
        [TestCase(2, "val3")]
        public void Item_NormalizedKeyIsSet(object key, string expected) {
            Content = "val1 = hello, world!\r\n \tVAL  2 = goodbye, earth..?  \n Val 3 = 3.14";
            var item = Subject.Item(key);
            Assert.AreEqual(expected, item.NormalizedKey);
        }

        [TestCase(0, "hello, world!")]
        [TestCase(1, "goodbye, earth..?")]
        [TestCase(2, "3.14")]
        public void Item_OriginalValueIsSet(object key, string expected) {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var item = Subject.Item(key);
            Assert.AreEqual(expected, item.OriginalValue);
        }

        [TestCase("val3")]
        [TestCase("Val 3")]
        [TestCase("VaL \t3")]
        public void Item_GetsItemByStringKey(object key) {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var item = Subject.Item(key);
            Assert.AreEqual("Val 3", item.OriginalKey);
        }

        [Test]
        public void ItemCount_GetsCountWhenSeparatedBySemicolon() {
            Content = "val1 = hello, world!;val2 = goodbye, earth..?; Val 3 = 3.14  ";
            Assert.AreEqual(3, Subject.ItemCount);
        }

        [Test]
        public void Item_OriginalValueIsSetWhenSeparatedBySemicolon() {
            Content = "val1 = hello, world!;val2 = goodbye, earth..?; Val 3 = 3.14  ";
            Assert.AreEqual("goodbye, earth..?", Subject.Item("val 2").OriginalValue);
        }

        [Test]
        public void ItemCount_NewLinePrecedesSemicolonSeparation() {
            Content = "val1 = hello, world!\n val2 = goodbye, earth..?; Val 3 = 3.14  ";
            Assert.AreEqual(2, Subject.ItemCount);
        }

        [Test]
        public void Item_OriginalValueHasSemicolonWhenSeparatedByNewLine() {
            Content = "val1 = hello, world!\n val2 = goodbye, earth..?; Val 3 = 3.14  ";
            Assert.AreEqual("goodbye, earth..?; Val 3 = 3.14", Subject.Item("val 2").OriginalValue);
        }

        class Man {
            public Dog BestFriend { get; set; }
        }

        [TypeConverter(typeof(DogConfigurationTypeConverter))]
        class Dog {
            public string Color { get; set; }
        }

        class DogConfigurationTypeConverter : ConfigurationTypeConverter {
            public override object ConvertFrom(IConfigurationBlock configuration, ITypeDescriptorContext context, CultureInfo culture, object value) =>
                configuration.Configure(new Dog(), value.ToString());
        }

        [Test]
        public void Configure_UsesConfigurationTypeConverter() {
            Content = @"
                Penny.color = red
                Man.Best friend = Penny
            ";
            var man = Subject.Configure(new Man());
            Assert.AreEqual("red", man.BestFriend.Color);
        }

        class Kid { public Pet Pet { get; set; } }
        class Pet { }
        class Cat : Pet { }

        [Test]
        public void Configure_CreatesInstanceOfType() {
            Content = @"Kid.Pet = Domore.Configuration.ConfigurationBlockTest+Cat, Domore.Configuration.Test";
            var kid = Subject.Configure(new Kid());
            Assert.That(kid.Pet, Is.InstanceOf(typeof(Cat)));
        }
    }
}
