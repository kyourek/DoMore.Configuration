using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Domore.Configuration.Contents {
    class ConfContentsProvider : FileContentsProvider {
        string _ConfFile;
        string ConfFile {
            get => _ConfFile ?? (_ConfFile = GetConfFile());
        }

        string GetConfFile() {
            var proc = Process.GetCurrentProcess();
            var procFile = proc.MainModule.FileName;
            var confFile = Path.ChangeExtension(procFile, ".conf");
            if (confFile.Contains(".vshost") && File.Exists(confFile) == false) {
                confFile = confFile.Replace(".vshost", "");
            }
            if (File.Exists(confFile)) {
                return confFile;
            }
            var confFileDefault = confFile + ".default";
            if (File.Exists(confFileDefault)) {
                File.Copy(confFileDefault, confFile);
                return confFile;
            }
            return "";
        }

        public override IEnumerable<KeyValuePair<string, string>> GetConfigurationContents(object content) {
            content = content?.ToString()?.Trim() ?? "";
            content = content.Equals("") ? ConfFile : content;
            return base.GetConfigurationContents(content);
        }
    }
}
