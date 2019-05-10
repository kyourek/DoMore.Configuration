using NUnit.Framework;

namespace Domore.Conf.Helpers {
    [TestFixture]
    public class KeyTest {
        [Test]
        public void Normalize_DoesNotLowercaseIndex() {
            Assert.That(Key.Normalize("Key.Property[Index]"), Is.EqualTo("key.property[Index]"));
        }

        [Test]
        public void Normalize_DoesNotLowercaseMultipleIndexes() {
            Assert.That(Key.Normalize("Key.Property[Index1][indeX2]"), Is.EqualTo("key.property[Index1][indeX2]"));
        }

        [Test]
        public void Normalize_DoesNotLowercaseMultipleIndexesSeparatedByDot() {
            Assert.That(Key.Normalize("Key.Property[Index1].PROP2[indeX2]"), Is.EqualTo("key.property[Index1].prop2[indeX2]"));
        }

        [Test]
        public void Normalize_RetainsSpacesInIndexes() {
            Assert.That(Key.Normalize("Key.Property[Index 1][inde X2]"), Is.EqualTo("key.property[Index 1][inde X2]"));
        }

        [Test]
        public void Normalize_RetainsSpacesInMultipleIndexesSeparatedByDot() {
            Assert.That(Key.Normalize("Key.Property[Index 1].PROP2[inde X2]"), Is.EqualTo("key.property[Index 1].prop2[inde X2]"));
        }

        [Test]
        public void Normalize_TrimsIndexes() {
            Assert.That(Key.Normalize("Key.Property[ Index 1\t].PROP2[\t inde X2   ]"), Is.EqualTo("key.property[Index 1].prop2[inde X2]"));
        }

        [Test]
        public void Normalize_TrimsIndexesWithoutPrecedingPropertyName() {
            Assert.That(Key.Normalize("Property[ Index 1\t][\t inde X2   ]"), Is.EqualTo("property[Index 1][inde X2]"));
        }

        [Test]
        public void Normalize_LowercasesKey() {
            Assert.That(Key.Normalize("PropertY"), Is.EqualTo("property"));
        }

        [Test]
        public void Normalize_TrimsKey() {
            Assert.That(Key.Normalize("  \tproperty   \t"), Is.EqualTo("property"));
        }

        [Test]
        public void Normalize_KeepsTextAfterIndex() {
            Assert.That(Key.Normalize("Key.Property[0].something.else"), Is.EqualTo("key.property[0].something.else"));
        }

        [Test]
        public void Normalize_KeepsTextAfterMultipleIndexes() {
            Assert.That(Key.Normalize("Key.Property[Index 1][inde X2].Something.Else"), Is.EqualTo("key.property[Index 1][inde X2].something.else"));
        }

        [Test]
        public void Normalize_KeepsTextAfter2ndPropertyWithIndex() {
            Assert.That(Key.Normalize("Key.Property[Index 1].Something[inde X2].Else"), Is.EqualTo("key.property[Index 1].something[inde X2].else"));
        }
    }
}
