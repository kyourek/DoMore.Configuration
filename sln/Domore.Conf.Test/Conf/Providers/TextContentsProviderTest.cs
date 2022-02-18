using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Domore.Conf.Providers {
    [TestFixture]
    public class TextContentsProviderTest {
        private TextContentsProvider Subject {
            get => _Subject ?? (_Subject = new TextContentsProvider());
            set => _Subject = value;
        }
        private TextContentsProvider _Subject;

        [SetUp]
        public void SetUp() {
            Subject = null;
        }

        [TearDown]
        public void TearDown() {
        }

        [Test]
        public void GetConfContent_ReturnsStringContents() {
            var contents = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("key 1", "val1"),
                new KeyValuePair<string, string>("key1", "val1"),
                new KeyValuePair<string, string>("Key  2", "another value")
            };
            var expected = string.Join(Environment.NewLine,
                "key 1 = val1",
                "key1 = val1",
                "Key  2 = another value");
            var actual = Subject.GetConfContent(contents);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetConfContent_ReturnsMultilineStringContents() {
            var contents = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("key 1", "val1"),
                new KeyValuePair<string, string>("key1", @"
                val1
                on

                several


                Lines


                "),
                new KeyValuePair<string, string>("Key  2", "another value")
            };
            var expected = string.Join(Environment.NewLine,
                "key 1 = val1",
                "key1 = {" + @"

                val1
                on

                several


                Lines


                ",
                "}",
                "Key  2 = another value");
            var actual = Subject.GetConfContent(contents);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
