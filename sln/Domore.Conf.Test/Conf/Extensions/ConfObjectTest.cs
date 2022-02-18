using NUnit.Framework;
using System;
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

        [Test]
        public void GetConfText_GetsComplexTextOnSingleLine() {
            var subject = new ComplexClass();
            subject.StringProp = "mystr";
            subject.DoubleProp = 4.321;
            subject.Child.StringProp = "My other str";
            var actual = subject.GetConfText(multiline: false);
            var expected = string.Join(";",
                "ComplexClass.Child.StringProp=My other str",
                "ComplexClass.DoubleProp=4.321",
                "ComplexClass.StringProp=mystr");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetConfText_GetsComplexTextOnSingleLineWithEmptyKey() {
            var subject = new ComplexClass();
            subject.StringProp = "mystr";
            subject.DoubleProp = 4.321;
            subject.Child.StringProp = "My other str";
            var actual = subject.GetConfText(key: "", multiline: false);
            var expected = "Child.StringProp=My other str;DoubleProp=4.321;StringProp=mystr";
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

        [Test]
        public void GetConfText_CanRoundTripComplexClassMultiline() {
            var expected = new ComplexClass();
            expected.StringProp = string.Join(Environment.NewLine, "mystr", "on", "", "multiple", "lines");
            expected.DoubleProp = 4.321;
            expected.Child.StringProp = "My other str";

            var text = expected.GetConfText();
            var conf = new ConfBlockFactory().CreateConfBlock(text, new TextContentsProvider());
            var actual = conf.Configure(new ComplexClass());
            Assert.That(actual.StringProp, Is.EqualTo(expected.StringProp));
        }

        private class DictedClass {
            public Dictionary<int, string> DictOfStrings { get; } = new Dictionary<int, string>();
        }

        [Test]
        public void GetConfText_GetsDictOfStrings() {
            var subject = new DictedClass();
            subject.DictOfStrings[0] = "hello";
            subject.DictOfStrings[1] = "world";
            var actual = subject.GetConfText();
            var expected = string.Join(Environment.NewLine,
                "DictedClass.DictOfStrings[0] = hello",
                "DictedClass.DictOfStrings[1] = world");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetConfText_CanRoundTripDictOfStrings() {
            var subject = new DictedClass();
            subject.DictOfStrings[0] = "hello";
            subject.DictOfStrings[1] = "world";
            var text = subject.GetConfText();
            var conf = new ConfBlockFactory().CreateConfBlock(text, new TextContentsProvider());
            var copy = conf.Configure(new DictedClass());
            var actual = copy.DictOfStrings;
            var expected = subject.DictOfStrings;
            CollectionAssert.AreEqual(expected, actual);
        }

        private class ComplexDictedClass {
            public Dictionary<int, ComplexClass> Dict { get; } = new Dictionary<int, ComplexClass>();
        }

        [Test]
        public void GetConfText_GetsComplexDictedClass() {
            var subject = new ComplexDictedClass();
            subject.Dict[0] = new ComplexClass();
            subject.Dict[0].Child.StringProp = "hello";
            subject.Dict[0].DoubleProp = 1.23;
            subject.Dict[0].StringProp = "world";
            subject.Dict[1] = new ComplexClass();
            subject.Dict[1].Child.StringProp = "HELLO";
            subject.Dict[1].DoubleProp = 2.34;
            subject.Dict[1].StringProp = "WORLD";
            var actual = subject.GetConfText("subj");
            var expected = string.Join(Environment.NewLine,
                "subj.Dict[0].Child.StringProp = hello",
                "subj.Dict[0].DoubleProp = 1.23",
                "subj.Dict[0].StringProp = world",
                "subj.Dict[1].Child.StringProp = HELLO",
                "subj.Dict[1].DoubleProp = 2.34",
                "subj.Dict[1].StringProp = WORLD");
            Assert.That(actual, Is.EqualTo(expected));
        }

        private void GetConfText_CanRoundTripComplexDictedClass(bool multiline) {
            var subject = new ComplexDictedClass();
            subject.Dict[0] = new ComplexClass();
            subject.Dict[0].Child.StringProp = "hello";
            subject.Dict[0].DoubleProp = 1.23;
            subject.Dict[0].StringProp = "world";
            subject.Dict[1] = new ComplexClass();
            subject.Dict[1].Child.StringProp = "HELLO";
            subject.Dict[1].DoubleProp = 2.34;
            subject.Dict[1].StringProp = "WORLD";
            var text = subject.GetConfText();
            var conf = new ConfBlockFactory().CreateConfBlock(text, new TextContentsProvider());
            var copy = conf.Configure(new ComplexDictedClass());
            Assert.That(copy.Dict[0].Child.StringProp, Is.EqualTo(subject.Dict[0].Child.StringProp));
            Assert.That(copy.Dict[0].DoubleProp, Is.EqualTo(subject.Dict[0].DoubleProp));
            Assert.That(copy.Dict[0].StringProp, Is.EqualTo(subject.Dict[0].StringProp));
            Assert.That(copy.Dict[1].Child.StringProp, Is.EqualTo(subject.Dict[1].Child.StringProp));
            Assert.That(copy.Dict[1].DoubleProp, Is.EqualTo(subject.Dict[1].DoubleProp));
            Assert.That(copy.Dict[1].StringProp, Is.EqualTo(subject.Dict[1].StringProp));
        }

        [Test]
        public void GetConfText_CanRoundTripComplexDictedClass() {
            GetConfText_CanRoundTripComplexDictedClass(true);
        }

        [Test]
        public void GetConfText_CanRoundTripComplexDictedClassOnSingleLine() {
            GetConfText_CanRoundTripComplexDictedClass(false);
        }

        private class ComplexListedClass {
            public List<ComplexClass> List { get; } = new List<ComplexClass>();
        }

        [Test]
        public void GetConfText_GetsComplexListedClass() {
            var subject = new ComplexListedClass();
            subject.List.Add(new ComplexClass());
            subject.List[0].Child.StringProp = "hello";
            subject.List[0].DoubleProp = 1.23;
            subject.List[0].StringProp = "world";
            subject.List.Add(new ComplexClass());
            subject.List[1].Child.StringProp = "HELLO";
            subject.List[1].DoubleProp = 2.34;
            subject.List[1].StringProp = "WORLD";
            var actual = subject.GetConfText();
            var expected = string.Join(Environment.NewLine,
                "ComplexListedClass.List[0].Child.StringProp = hello",
                "ComplexListedClass.List[0].DoubleProp = 1.23",
                "ComplexListedClass.List[0].StringProp = world",
                "ComplexListedClass.List[1].Child.StringProp = HELLO",
                "ComplexListedClass.List[1].DoubleProp = 2.34",
                "ComplexListedClass.List[1].StringProp = WORLD");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void GetConfText_CanRoundTripComplexListedClass(bool multiline) {
            var subject = new ComplexListedClass();
            subject.List.Add(new ComplexClass());
            subject.List[0].Child.StringProp = "hello";
            subject.List[0].DoubleProp = 1.23;
            subject.List[0].StringProp = "world";
            subject.List.Add(new ComplexClass());
            subject.List[1].Child.StringProp = "HELLO";
            subject.List[1].DoubleProp = 2.34;
            subject.List[1].StringProp = "WORLD";
            var text = subject.GetConfText(multiline: multiline);
            var conf = new ConfBlockFactory().CreateConfBlock(text, new TextContentsProvider());
            var copy = conf.Configure(new ComplexListedClass());
            Assert.That(copy.List[0].Child.StringProp, Is.EqualTo(subject.List[0].Child.StringProp));
            Assert.That(copy.List[0].DoubleProp, Is.EqualTo(subject.List[0].DoubleProp));
            Assert.That(copy.List[0].StringProp, Is.EqualTo(subject.List[0].StringProp));
            Assert.That(copy.List[1].Child.StringProp, Is.EqualTo(subject.List[1].Child.StringProp));
            Assert.That(copy.List[1].DoubleProp, Is.EqualTo(subject.List[1].DoubleProp));
            Assert.That(copy.List[1].StringProp, Is.EqualTo(subject.List[1].StringProp));
        }

        [Test]
        public void GetConfText_CanRoundTripComplexListedClassWithBracketedContent1() {
            var subject = new ComplexListedClass();
            subject.List.Add(new ComplexClass());
            subject.List[0].Child.StringProp = "hello";
            subject.List[0].DoubleProp = 1.23;
            subject.List[0].StringProp = @"world
on more than one

line";
            subject.List.Add(new ComplexClass());
            subject.List[1].Child.StringProp = @"

      
HELLO there

            how's it going?

  
 ";
            subject.List[1].DoubleProp = 2.34;
            subject.List[1].StringProp = "WORLD";
            var text = subject.GetConfText();
            var conf = new ConfBlockFactory().CreateConfBlock(text, new TextContentsProvider());
            var copy = conf.Configure(new ComplexListedClass());
            Assert.That(copy.List[0].Child.StringProp, Is.EqualTo(subject.List[0].Child.StringProp));
            Assert.That(copy.List[0].DoubleProp, Is.EqualTo(subject.List[0].DoubleProp));
            Assert.That(copy.List[0].StringProp, Is.EqualTo(subject.List[0].StringProp));
            Assert.That(copy.List[1].Child.StringProp, Is.EqualTo(subject.List[1].Child.StringProp));
            Assert.That(copy.List[1].DoubleProp, Is.EqualTo(subject.List[1].DoubleProp));
            Assert.That(copy.List[1].StringProp, Is.EqualTo(subject.List[1].StringProp));
        }

        [Test]
        public void GetConfText_CanRoundTripComplexListedClassWithBracketedContent2() {
            var subject = new ComplexListedClass();
            subject.List.Add(new ComplexClass());
            subject.List[0].Child.StringProp = @"hello
            here's
            some more }
            {lines}";
            subject.List[0].DoubleProp = 1.23;
            subject.List[0].StringProp = @"world
on more than one

line";
            subject.List.Add(new ComplexClass());
            subject.List[1].Child.StringProp = @"

      
HELLO there

            how's it going?

  
 ";
            subject.List[1].DoubleProp = 2.34;
            subject.List[1].StringProp = @"{
WORLD   and    
            some other
            stuff}
}...";
            var text = subject.GetConfText();
            var conf = new ConfBlockFactory().CreateConfBlock(text, new TextContentsProvider());
            var copy = conf.Configure(new ComplexListedClass());
            Assert.That(copy.List[0].Child.StringProp, Is.EqualTo(subject.List[0].Child.StringProp));
            Assert.That(copy.List[0].DoubleProp, Is.EqualTo(subject.List[0].DoubleProp));
            Assert.That(copy.List[0].StringProp, Is.EqualTo(subject.List[0].StringProp));
            Assert.That(copy.List[1].Child.StringProp, Is.EqualTo(subject.List[1].Child.StringProp));
            Assert.That(copy.List[1].DoubleProp, Is.EqualTo(subject.List[1].DoubleProp));
            Assert.That(copy.List[1].StringProp, Is.EqualTo(subject.List[1].StringProp));
        }

        private class ClassWithListExposedAsICollection {
            public ICollection<Inner> Inners {
                get => _Inners ?? (_Inners = new List<Inner>());
                set => _Inners = value;
            }
            private ICollection<Inner> _Inners;

            public class Inner {
                public double Value { get; set; }
            }
        }

        [Test]
        public void GetConfText_GetsConfFromListExposedAsICollection() {
            var obj = new ClassWithListExposedAsICollection();
            obj.Inners.Add(new ClassWithListExposedAsICollection.Inner { Value = 1.1 });
            obj.Inners.Add(new ClassWithListExposedAsICollection.Inner { Value = 1.2 });
            obj.Inners.Add(new ClassWithListExposedAsICollection.Inner { Value = 1.3 });
            var actual = obj.GetConfText("item");
            var expected = string.Join(Environment.NewLine,
                "item.Inners[0].Value = 1.1",
                "item.Inners[1].Value = 1.2",
                "item.Inners[2].Value = 1.3");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetConfText_GetsConfOfList() {
            var list = new List<string> { "hello", "world", "hey", "earth" };
            var actual = list.GetConfText();
            var expected = string.Join(Environment.NewLine,
                "String[0] = hello",
                "String[1] = world",
                "String[2] = hey",
                "String[3] = earth");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetConfText_GetsConfOfListMultiline() {
            var list = new List<string> { "hello", "world", "hey", @"
earth
            is on   
MANY


lines
" };
            var actual = list.GetConfText();
            var expected = string.Join(Environment.NewLine,
                "String[0] = hello",
                "String[1] = world",
                "String[2] = hey",
                "String[3] = {" + @"

earth
            is on   
MANY


lines

}");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetConfText_GetsConfOfDictionary() {
            var dict = new Dictionary<string, double> { { "hello", 1.23 }, { "World", 4.56 } };
            var actual = dict.GetConfText();
            var expected = string.Join(Environment.NewLine,
                "Double[hello] = 1.23",
                "Double[World] = 4.56");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetConfText_GetsConfOfDictionaryWithBracketedText() {
            var dict = new Dictionary<string, string> { 
                { "hello", @"here's{
}some
text...
  
  
" }, 
                { "World", @"
}                      some
more lines{" } };
            var actual = dict.GetConfText();

            var expected = @"String[hello] = {
here's{
}some
text...
  
  

}
String[World] = {

}                      some
more lines{
}";
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetConfText_CanRoundTripDictionary() {
            var obj1 = new ClassWithListExposedAsICollection();
            obj1.Inners.Add(new ClassWithListExposedAsICollection.Inner { Value = 1.1 });
            obj1.Inners.Add(new ClassWithListExposedAsICollection.Inner { Value = 1.2 });
            obj1.Inners.Add(new ClassWithListExposedAsICollection.Inner { Value = 1.3 });

            var obj2 = new ClassWithListExposedAsICollection();
            obj2.Inners.Add(new ClassWithListExposedAsICollection.Inner { Value = 2.1 });
            obj2.Inners.Add(new ClassWithListExposedAsICollection.Inner { Value = 2.2 });
            obj2.Inners.Add(new ClassWithListExposedAsICollection.Inner { Value = 2.3 });

            var dict = new Dictionary<string, ClassWithListExposedAsICollection> {
                { "obj1", obj1 },
                { "obj2", obj2 }
            };
            var text = dict.GetConfText("item");
            var conf = new ConfBlockFactory().CreateConfBlock(text, new TextContentsProvider());
            var copy = conf.Configure(key => new ClassWithListExposedAsICollection(), "item")
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            Assert.That(copy["obj1"].Inners.ElementAt(0).Value, Is.EqualTo(1.1));
            Assert.That(copy["obj1"].Inners.ElementAt(1).Value, Is.EqualTo(1.2));
            Assert.That(copy["obj1"].Inners.ElementAt(2).Value, Is.EqualTo(1.3));

            Assert.That(copy["obj2"].Inners.ElementAt(0).Value, Is.EqualTo(2.1));
            Assert.That(copy["obj2"].Inners.ElementAt(1).Value, Is.EqualTo(2.2));
            Assert.That(copy["obj2"].Inners.ElementAt(2).Value, Is.EqualTo(2.3));
        }

        private class GetConfText_CanBePassedEmptyStringForKey_Helper : ComplexListedClass {
            public string MyName { get; set; }
            public float MyValue { get; set; }
        }

        [Test]
        public void GetConfText_CanBePassedEmptyStringForKey() {
            var subject = new GetConfText_CanBePassedEmptyStringForKey_Helper();
            subject.MyName = "thename";
            subject.MyValue = 3.21F;
            subject.List.Add(new ComplexClass());
            subject.List[0].Child.StringProp = "hello";
            subject.List[0].DoubleProp = 1.23;
            subject.List[0].StringProp = "world";
            subject.List.Add(new ComplexClass());
            subject.List[1].Child.StringProp = "HELLO";
            subject.List[1].DoubleProp = 2.34;
            subject.List[1].StringProp = "WORLD";
            var actual = subject.GetConfText(key: "");
            var expected = string.Join(Environment.NewLine,
                "List[0].Child.StringProp = hello",
                "List[0].DoubleProp = 1.23",
                "List[0].StringProp = world",
                "List[1].Child.StringProp = HELLO",
                "List[1].DoubleProp = 2.34",
                "List[1].StringProp = WORLD",
                "MyName = thename",
                "MyValue = 3.21");
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
