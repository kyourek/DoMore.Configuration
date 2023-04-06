using NUnit.Framework;
using System.Collections.Generic;

namespace Domore.Conf.Converters {
    using Extensions;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    [TestFixture]
    public class ConfListItemsTest {
        private class Kid {
            [ConfListItems]
            public List<string> FavoriteColors { get; set; }

            [ConfListItems("pets")]
            public List<string> PetNames { get; set; }
        }

        [Test]
        public void ConvertsItemsIntoList() {
            var actual = new Kid().ConfFrom($"kid.FavoriteColors = Red, green,   BLUE").FavoriteColors;
            var expected = new[] { "Red", "green", "BLUE" };
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ConvertsItemsIntoListWithSpecifiedName() {
            var actual = new Kid().ConfFrom($"kid.pets= Little Bit, Penny").PetNames;
            var expected = new[] { "Little Bit", "Penny" };
            CollectionAssert.AreEqual(expected, actual);
        }

        private class Pi {
            [ConfListItems]
            public List<int> Digits { get; set; }
        }

        [Test]
        public void ConvertsItemsIntoListOfInt() {
            var actual = new Pi().ConfFrom($"PI.digits = 3, 1,4 ").Digits;
            var expected = new[] { 3, 1, 4 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TypeConverter(typeof(PairWithTypeConverter.Converter))]
        private class PairWithTypeConverter {
            public string Thing1 { get; set; }
            public double Thing2 { get; set; }

            public class Converter : TypeConverter {
                public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
                    var s = $"{value}";
                    var p = s.Split('&');
                    return new PairWithTypeConverter {
                        Thing1 = p[0],
                        Thing2 = double.Parse(p[1])
                    };
                }
            }
        }

        private class PairWithTypeConverters {
            [ConfListItems]
            public List<PairWithTypeConverter> Items { get; set; }
        }

        [Test]
        public void ConvertsUsingDefaultItemTypeConverter() {
            var items = new PairWithTypeConverters().ConfFrom($"Items = str1&1.2 , str2&2.3 , str3&3.4", key: "").Items;
            var actual = items.SelectMany(item => new object[] { item.Thing1, item.Thing2 }).ToArray();
            var expected = new object[] { items[0].Thing1, items[0].Thing2, items[1].Thing1, items[1].Thing2, items[2].Thing1, items[2].Thing2 };
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
