namespace Domore.ReleaseActions {
    class Build : ReleaseAction {
        public override void Work() {
            var nugetPath = Paths["nuget"];
            var msBuildPath = Paths["msbuild"];
            var solutionPath = $"\"{SolutionFilePath}\"";
            Process(nugetPath, "restore", solutionPath);
            Process(msBuildPath, "/t:Clean", "/t:Rebuild", "/p:Configuration=Release", solutionPath);
        }
    }
}
