using NUnit.Framework;

namespace Domore.Conf.Helpers {
    [TestFixture]
    public class ParserTest {
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("\t   \n\r")]
        public void ParseIntegerCollection_ReturnsEmptyCollection(string input) {
            var actual = Parse.IntegerCollection(input);
            var expected = new int[] { };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestCase("7", 7)]
        [TestCase(" 51\t", 51)]
        public void ParseIntegerCollection_ReturnsCollectionWithOneItem(string input, int item) {
            var actual = Parse.IntegerCollection(input);
            var expected = new[] { item };
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseIntegerCollection_ReturnsCollectionFromCsv() {
            var actual = Parse.IntegerCollection("347,39481, 9, -1029432,48");
            var expected = new[] { 347, 39481, 9, -1029432, 48 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseIntegerCollection_ParsesRange() {
            var actual = Parse.IntegerCollection("4-11");
            var expected = new[] { 4, 5, 6, 7, 8, 9, 10, 11 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseIntegerCollection_ParsesCsvAndRange() {
            var actual = Parse.IntegerCollection("2,6-9, 21");
            var expected = new[] { 2, 6, 7, 8, 9, 21 };
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
