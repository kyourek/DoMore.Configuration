using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf.Future {
    [TestFixture]
    public class ConfContainerTest {
        private object Content;

        private ConfContainer Subject {
            get => _Subject ?? (_Subject = new ConfContainer { Contents = Content });
            set => _Subject = value;
        }
        private ConfContainer _Subject;

        [SetUp]
        public void SetUp() {
            Content = null;
            Subject = null;
        }

        private class Man {
            [Conf(converter: typeof(DogConfValueConverter))]
            public Dog BestFriend { get; set; }
        }

        private class Dog {
            public string Color { get; set; }
        }

        private class DogConfValueConverter : ConfValueConverter {
            public override object Convert(string value, ConfValueConverterState state) {
                return state.Conf.Configure(new Dog(), key: value);
            }
        }

        [Test]
        public void Configure_UsesConfTypeConverter() {
            Content = @"
                Penny.color = red
                Man.Best friend = Penny
            ";
            var man = Subject.Configure(new Man());
            Assert.AreEqual("red", man.BestFriend.Color);
        }

        private class ManWithCat : Man {
            [Conf(ignore: false)]
            public Cat Cat { get; set; }
        }

        [Test]
        public void Configure_DoesNotIgnoreProperty() {
            Content = @"
                Penny.color = red
                Man.Best friend = Penny
                Man.Cat.Color = black
            ";
            var man = Subject.Configure(new ManWithCat(), "Man");
            Assert.AreEqual("black", man.Cat.Color);
        }

        private class ManWithIgnoredCat : Man {
            [Conf(ignore: true)]
            public Cat Cat { get; set; }
        }

        [Test]
        public void Configure_IgnoresMansCat() {
            Content = @"
                Penny.color = red
                Man.Best friend = Penny
                Man.Cat.Color = black
            ";
            var man = Subject.Configure(new ManWithIgnoredCat(), "Man");
            Assert.IsNull(man.Cat);
        }

        private class ManWithIgnoredCat2 : Man {
            [Conf(ignoreSet: true, ignoreGet: false)]
            public Cat Cat { get; set; }
        }

        [Test]
        public void Configure_IgnoresMansCat2() {
            Content = @"
                Penny.color = red
                Man.Best friend = Penny
                Man.Cat.Color = black
            ";
            var man = Subject.Configure(new ManWithIgnoredCat2(), "Man");
            Assert.IsNull(man.Cat);
        }

        private class ObjWithIgnoredProp {
            public string NotIgnored { get; set; }

            [Conf(ignore: true)]
            public string YesIgnored { get; set; }
        }

        [Test]
        public void Configure_IgnoresIgnoredProperty() {
            Content = @"
                Obj.NotIgnored = 1
                Obj.YesIgnored = 1
            ";
            var obj = Subject.Configure(new ObjWithIgnoredProp(), "Obj");
            Assert.IsNull(obj.YesIgnored);
        }

        private class Kid { public Pet Pet { get; set; } }
        private class Pet { }
        private class Cat : Pet { public string Color { get; set; } }

        [Test]
        public void Configure_CreatesInstanceOfType() {
            Content = @"Kid.Pet = Domore.Conf.Future.ConfContainerTest+Cat, Domore.Conf.Test";
            var kid = Subject.Configure(new Kid());
            Assert.That(kid.Pet, Is.InstanceOf(typeof(Cat)));
        }

        private class Mom { public IList<string> Jobs { get; } = new Collection<string>(); }

        [Test]
        public void Configure_AddsToList() {
            Content = @"
                Mom.Jobs[0] = chef
                Mom.jobs[1] = Nurse
                mom.jobs[2] = accountant";
            var mom = Subject.Configure(new Mom());
            Assert.That(mom.Jobs, Is.EqualTo(new[] { "chef", "Nurse", "accountant" }));
        }

        private class NumContainer { public ICollection<double> Nums { get; } = new List<double>(); }

        [Test]
        public void Configure_AddsConvertedValuesToList() {
            Content = @"
                Cont.Nums[0] = 1.23
                Cont.nums[1] = 2.34
                Cont.nums[2] = 3.45";
            var cont = Subject.Configure(new NumContainer(), "cont");
            Assert.That(cont.Nums, Is.EqualTo(new[] { 1.23, 2.34, 3.45 }));
        }

        [Test]
        public void Configure_RespectsLastListedIndexOfList() {
            Content = @"
                Cont.Nums[0] = 1.23
                Cont.nums[1] = 2.34
                Cont.nums[1] = 2.00
                Cont.nums[2] = 3.45";
            var cont = Subject.Configure(new NumContainer(), "cont");
            Assert.That(cont.Nums, Is.EqualTo(new[] { 1.23, 2.00, 3.45 }));
        }

        [Test]
        public void Configure_SetsListItemsToDefault() {
            Content = @"
                Cont.Nums[0] = 1.23
                Cont.nums[1] = 2.34
                Cont.nums[2] = 3.45
                Cont.nums[5] = 5.67";
            var cont = Subject.Configure(new NumContainer(), "cont");
            Assert.That(cont.Nums, Is.EqualTo(new[] { 1.23, 2.34, 3.45, 0.0, 0.0, 5.67 }));
        }

        [Test]
        public void Configure_SetsListItemsToNull() {
            Content = @"
                Mom.Jobs[1] = chef
                Mom.jobs[3] = Nurse
                mom.jobs[7] = accountant";
            var mom = Subject.Configure(new Mom());
            Assert.That(mom.Jobs, Is.EqualTo(new[] { null, "chef", null, "Nurse", null, null, null, "accountant" }));
        }

        private class IntContainer { public IDictionary<string, int> Dict { get; } = new Dictionary<string, int>(); }

        [Test]
        public void Configure_SetsDictionaryValues() {
            Content = @"
                cont.Dict[first] = 1
                cont.dict[Third] = 3";
            var cont = Subject.Configure(new IntContainer(), "cont");
            Assert.That(cont.Dict, Is.EqualTo(new Dictionary<string, int> { { "first", 1 }, { "Third", 3 } }));
        }

        private class Infant {
            public string Weight { get; set; }
            public int DiaperSize { get; set; }
            public Mom Mom { get; } = new Mom();
        }

        [Test]
        public void Configure_CanSetValuesWithoutKey() {
            Content = @"
                Weight = 12.3 lb
                Diaper size = 1
                Mom.Jobs[0] = chef
                Mom.jobs[1] = Nurse
                mom.jobs[2] = accountant            ";
            var infant = Subject.Configure(new Infant(), "");
            Assert.That(infant.Weight, Is.EqualTo("12.3 lb"));
        }

        [Test]
        public void Configure_SetsSecondValueWithoutKey() {
            Content = @"
                Weight = 12.3 lb
                Diaper size = 1
                Mom.Jobs[0] = chef
                Mom.jobs[1] = Nurse
                mom.jobs[2] = accountant
            ";
            var infant = Subject.Configure(new Infant(), "");
            Assert.That(infant.DiaperSize, Is.EqualTo(1));
        }

        [Test]
        public void Configure_SetsComplexTypeValuesWithoutKey() {
            Content = @"
                Weight = 12.3 lb
                Diaper size = 1
                Mom.Jobs[0] = chef
                Mom.jobs[1] = Nurse
                mom.jobs[2] = accountant
            ";
            var infant = Subject.Configure(new Infant(), "");
            Assert.That(infant.Mom.Jobs[1], Is.EqualTo("Nurse"));
        }

        [Test]
        public void Configure_SetsDeepValues() {
            Content = @"
                kid.Weight = 12.3 lb
                kid.Diaper size = 1
                kid.Mom.Jobs[0] = chef
                kid.Mom.jobs[1] = Nurse
                kid.mom.jobs[2] = accountant
            ";
            var infant = Subject.Configure(new Infant(), "Kid");
            Assert.That(infant.Mom.Jobs[2], Is.EqualTo("accountant"));
        }

        [Test]
        public void Configure_ReturnsCollection() {
            Content = @"
                kid[0].weight = 3
                kid[0].diapersize = 1
                kid[1].weight = 15
                kid[1].diapersize = 2
                kid[2].weight = 26
                kid[2].diapersize = 4
            ";
            var kids = Subject.Configure(() => new Infant(), "Kid").ToList();
            Assert.That(kids.Count, Is.EqualTo(3));
        }
    }
}
