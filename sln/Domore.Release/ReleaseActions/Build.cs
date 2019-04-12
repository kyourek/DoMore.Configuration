namespace Domore.ReleaseActions {
    class Build : ReleaseAction {
        public override void Work() {
            var solutionPath = $"\"{SolutionFilePath}\"";
            Process("nuget", "restore", solutionPath);
            Process(@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe", "/t:Clean", "/t:Rebuild", "/p:Configuration=Release", solutionPath);
        }
    }
}
