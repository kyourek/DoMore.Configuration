using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Cli {
    internal sealed class TargetDescription {
        private static readonly Dictionary<Type, TargetDescription> Cache = new Dictionary<Type, TargetDescription>();

        private TargetDescription(Type targetType) {
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        public Type TargetType { get; }

        public IEnumerable<TargetPropertyDescription> Properties =>
            _Properties ?? (
            _Properties = TargetType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(propertyInfo => new TargetPropertyDescription(propertyInfo))
                .ToList());
        private IEnumerable<TargetPropertyDescription> _Properties;

        public string CommandName =>
            _CommandName ?? (
            _CommandName = TargetType.Name.ToLowerInvariant());
        private string _CommandName;

        public string Display =>
            _Display ?? (
            _Display = TargetDisplay.For(this));
        private string _Display;

        public bool DisplayDefault =>
            _DisplayDefault ?? (
            _DisplayDefault =
                Properties.Any(p => p.DisplayAttribute.Include == true) ? false :
                Properties.Any(p => p.DisplayAttribute.Include == false) ? true :
                true).Value;
        private bool? _DisplayDefault;

        public static TargetDescription Describe(Type targetType) {
            if (Cache.TryGetValue(targetType, out var targetDescription) == false) {
                Cache[targetType] = targetDescription = new TargetDescription(targetType);
            }
            return targetDescription;
        }

        public IEnumerable<string> Conf(string cli) {
            var properties = Properties;
            var req = properties
                .Where(p => p.Required)
                .ToList();
            var arg = properties
                .Where(p => p.ArgumentOrder >= 0)
                .OrderBy(p => p.ArgumentOrder)
                .ToList();
            foreach (var token in Token.Parse(cli)) {
                var key = token.Key?.Trim() ?? "";
                var val = token.Value?.Trim() ?? "";
                if (val == "" && key == "") {
                    continue;
                }
                if (val == "") {
                    if (key.Equals(CommandName, StringComparison.OrdinalIgnoreCase)) {
                        continue;
                    }
                    if (arg.Count > 0) {
                        val = key;
                        key = arg[0].ArgumentName;
                        arg.RemoveAt(0);
                    }
                    else {
                        throw new CliArgumentNotFoundException(key);
                    }
                }
                if (req.Count > 0) {
                    req.RemoveAll(p => p.AllNames.Contains(key, StringComparer.OrdinalIgnoreCase));
                }
                yield return key + " = " + val;
            }
            if (req.Count > 0) {
                throw new CliRequiredNotFoundException(req);
            }
        }
    }
}
