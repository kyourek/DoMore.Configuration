using System.IO;

namespace Domore.ReleaseActions {
    class Push : ReleaseAction {
        public override void Work() {
            foreach (var pkg in new[] { "Domore.Configuration", "Domore.Configuration.ConfigurationManager" }) {
                var pkgFile = Path.Combine(SolutionDirectory, $"{pkg}.{Context.Version.StagedVersion}.nupkg");
                Process("nuget", "push", $"\"{pkgFile}\"", "-Source", "nuget.org");
            }
        }
    }
}
