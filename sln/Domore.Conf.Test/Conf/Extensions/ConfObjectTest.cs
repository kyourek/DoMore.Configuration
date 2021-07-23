using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Conf.Extensions {
    using Providers;

    [TestFixture]
    public class ConfObjectTest {
        private class OnePropertyClass {
            public string StringProp { get; set; }
        }

        [Test]
        public void GetConfContents_GetsOneProperty() {
            var contents = new OnePropertyClass { StringProp = "The value" }.GetConfContents();
            var expected = new KeyValuePair<string, string>("OnePropertyClass.StringProp", "The value");
            var actual = contents.Single();
            Assert.That(actual, Is.EqualTo(expected));
        }

        private class TwoPropertyClass : OnePropertyClass {
            public double DoubleProp { get; set; }
        }

        [Test]
        public void GetConfContents_GetsTwoProperties() {
            var contents = new TwoPropertyClass { StringProp = "Hello World", DoubleProp = 1.23 }.GetConfContents();
            var expected = new Dictionary<string, string> { { "TwoPropertyClass.StringProp", "Hello World" }, { "TwoPropertyClass.DoubleProp", "1.23" } };
            CollectionAssert.AreEquivalent(expected, contents);
        }

        [Test]
        public void GetConfContents_GetsTwoPropertiesWithKey() {
            var contents = new TwoPropertyClass { StringProp = "Hello World", DoubleProp = 1.23 }.GetConfContents("Settings");
            var expected = new Dictionary<string, string> { { "Settings.StringProp", "Hello World" }, { "Settings.DoubleProp", "1.23" } };
            CollectionAssert.AreEquivalent(expected, contents);
        }

        [Test]
        public void GetConfText_GetsTwoProperties() {
            var contents = new TwoPropertyClass { StringProp = "Hello World", DoubleProp = 1.23 }.GetConfText("Settings");
            var expected = string.Join(Environment.NewLine, "Settings.DoubleProp = 1.23", "Settings.StringProp = Hello World");
            Assert.That(contents, Is.EqualTo(expected));
        }

        private class ComplexClass : TwoPropertyClass {
            public OnePropertyClass Child { get; } = new OnePropertyClass();
        }

        [Test]
        public void GetConfText_GetsComplexText() {
            var subject = new ComplexClass();
            subject.StringProp = "mystr";
            subject.DoubleProp = 4.321;
            subject.Child.StringProp = "My other str";
            var actual = subject.GetConfText();
            var expected = string.Join(Environment.NewLine,
                "ComplexClass.Child.StringProp = My other str",
                "ComplexClass.DoubleProp = 4.321",
                "ComplexClass.StringProp = mystr");
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static void GetConfText_CanRoundTripComplexClass(Action<ComplexClass, ComplexClass> assert) {
            var expected = new ComplexClass();
            expected.StringProp = "mystr";
            expected.DoubleProp = 4.321;
            expected.Child.StringProp = "My other str";

            var text = expected.GetConfText();
            var conf = new ConfBlockFactory().CreateConfBlock(text, new TextContentsProvider());
            var actual = conf.Configure(new ComplexClass());
            assert(actual, expected);
        }

        [Test]
        public void GetConfText_CanRoundTripComplexClassStringProp() {
            GetConfText_CanRoundTripComplexClass((actual, expected) =>
                Assert.That(actual.StringProp, Is.EqualTo(expected.StringProp)));
        }

        [Test]
        public void GetConfText_CanRoundTripComplexClassDoubleProp() {
            GetConfText_CanRoundTripComplexClass((actual, expected) =>
                Assert.That(actual.DoubleProp, Is.EqualTo(expected.DoubleProp)));
        }

        [Test]
        public void GetConfText_CanRoundTripComplexClassChild() {
            GetConfText_CanRoundTripComplexClass((actual, expected) =>
                Assert.That(actual.Child.StringProp, Is.EqualTo(expected.Child.StringProp)));
        }
    }
}
