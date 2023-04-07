using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Conf.Cli {
    [TestFixture]
    public class CliTest {
        private class Move {
            [CliRequired]
            [CliArgument]
            public MoveDirection Direction { get; set; }

            [CliArgument(order: 1)]
            public double Speed { get; set; }

            [CliDisplay(false)]
            public object DoNotDisplay { get; set; }
        }

        private enum MoveDirection {
            Up, Down, Left, Right
        }

        [Test]
        public void Configure_SetsCliArgument() {
            var move = Cli.Configure(new Move(), "left");
            Assert.That(move.Direction, Is.EqualTo(MoveDirection.Left));
        }

        [Test]
        public void Configure_SetsCliArgument1() {
            var move = Cli.Configure(new Move(), "RIGHT 55.5");
            Assert.That(move.Direction, Is.EqualTo(MoveDirection.Right));
            Assert.That(move.Speed, Is.EqualTo(55.5));
        }

        [Test]
        public void Configure_ThrowsExceptionIfRequiredPropertyNotFound() {
            var ex = Assert.Throws<CliRequiredNotFoundException>(() => Cli.Configure(new Move(), ""));
            Assert.That(ex.NotFound
                .Select(notFound => notFound.PropertyInfo)
                .Contains(typeof(Move).GetProperty(nameof(Move.Direction))));
        }

        [Test]
        public void Configure_ThrowsExceptionIfTooManyArgumentsGiven() {
            var ex = Assert.Throws<CliArgumentNotFoundException>(() => Cli.Configure(new Move(), "RIGHT 55.5 extra"));
            Assert.That(ex.Argument == "extra");
        }

        [Test]
        public void Configure_ThrowsExceptionIfValueCannotBeConverted() {
            var ex = Assert.Throws<CliConversionException>(() => Cli.Configure(new Move(), "backwards"));
            Assert.That(ex.InnerException, Is.InstanceOf(typeof(ConfValueConverterException)));
            var ie = (ConfValueConverterException)ex.InnerException;
            Assert.That(ie.Value, Is.EqualTo("backwards"));
            Assert.That(ie.State.Property == typeof(Move).GetProperty(nameof(Move.Direction)));
        }

        [Test]
        public void Display_DescribesCommand() {
            var actual = Cli.Display(new Move());
            var expected = "move direction<up|down|left|right> [speed<num>]";
            Assert.That(actual, Is.EqualTo(expected));
        }

        private class Bike {
            [CliRequired]
            public MoveDirection Move { get; set; }
            public double Speed { get; set; }
        }

        [TestCase("move=down speed=32.1")]
        [TestCase("bike move=down speed=32.1")]
        [TestCase("BIKE Move=doWN SPEED=32.1")]
        public void Configure_SetsCliProperties(string cli) {
            var bike = Cli.Configure(new Bike(), cli);
            Assert.That(bike.Move, Is.EqualTo(MoveDirection.Down));
            Assert.That(bike.Speed, Is.EqualTo(32.1));
        }

        [Test]
        public void Display_ShowsPropertyNames() {
            var actual = Cli.Display(new Bike());
            var expected = "bike move=<up|down|left|right> [speed=<num>]";
            Assert.That(actual, Is.EqualTo(expected));
        }

        private class Blend {
            [CliArgument]
            [CliRequired]
            public List<string> Fruits { get; set; }
        }

        [Test]
        public void Configure_SetsListItems() {
            var blend = Cli.Configure(new Blend(), "apples,bananas");
            CollectionAssert.AreEqual(new[] { "apples", "bananas" }, blend.Fruits);
        }

        [Test]
        public void Display_ShowsList() {
            var actual = Cli.Display(new Blend());
            var expected = "blend fruits<list>";
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
