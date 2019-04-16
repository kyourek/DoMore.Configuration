using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Configuration {
    class Sample {
        IConfigurationContainer _Configuration;
        public IConfigurationContainer Configuration {
            get { return _Configuration ?? (_Configuration = new ConfigurationContainer()); }
            set { _Configuration = value; }
        }

        public void Run() {
            var conf = Configuration;
            var alien = conf.Configure(new Alien());
            var visitor = conf.Configure(new Visitor());
            Console.WriteLine($"A: {alien.Greeting}, {visitor.HomePlanet}!");
            Console.WriteLine($"A: Welcome to {alien.HomePlanet}.");
            Console.WriteLine($"V: Thanks! I also toured {string.Join(", ", visitor.TourDestinations.Values)}");
            Console.WriteLine($"V: on a {string.Join(", ", visitor.ShipModelsAndMakes.Select(pair => $"{pair.Value} {pair.Key}"))}");
            Console.WriteLine("[Enter] to exit");
            Console.ReadLine();
        }

        class Alien {
            public string Greeting { get; set; }
            public string HomePlanet { get; set; }
        }

        class Visitor {
            public string HomePlanet { get; set; }
            public IDictionary<int, string> TourDestinations { get; set; } = new Dictionary<int, string>();
            public IDictionary<string, string> ShipModelsAndMakes { get; set; } = new Dictionary<string, string>();
        }
    }
}
