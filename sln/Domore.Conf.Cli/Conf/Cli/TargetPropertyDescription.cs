using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Cli {
    internal sealed class TargetPropertyDescription {
        public bool Required =>
            _Required ?? (
            _Required = PropertyInfo.GetCustomAttributes(typeof(CliRequiredAttribute), inherit: true).Length > 0).Value;
        private bool? _Required;

        public int ArgumentOrder =>
            _ArgumentOrder ?? (
            _ArgumentOrder = PropertyInfo
                .GetCustomAttributes(typeof(CliArgumentAttribute), inherit: true)
                .OfType<CliArgumentAttribute>()
                .FirstOrDefault()?.Order ?? -1).Value;
        private int? _ArgumentOrder;

        public string ArgumentName =>
            _ArgumentName ?? (
            _ArgumentName = ArgumentOrder < 0
                ? ""
                : (ConfNames.FirstOrDefault() ?? PropertyName));
        private string _ArgumentName;

        public string PropertyName =>
            _PropertyName ?? (
            _PropertyName = PropertyInfo.Name);
        private string _PropertyName;

        public ReadOnlyCollection<string> ConfNames =>
            _ConfNames ?? (
            _ConfNames = new ReadOnlyCollection<string>(PropertyInfo
                .GetCustomAttributes(typeof(ConfAttribute), inherit: true)
                .OfType<ConfAttribute>()
                .SelectMany(attribute => attribute.Names)
                .ToList()));
        private ReadOnlyCollection<string> _ConfNames;

        public ReadOnlyCollection<string> AllNames =>
            _AllNames ?? (
            _AllNames = new ReadOnlyCollection<string>(new[] { PropertyName }
                .Concat(ConfNames)
                .ToList()));
        private ReadOnlyCollection<string> _AllNames;

        public string DisplayName =>
            _DisplayName ?? (
            _DisplayName = ConfNames.FirstOrDefault() ?? PropertyName);
        private string _DisplayName;

        public PropertyInfo PropertyInfo { get; }

        public TargetPropertyDescription(PropertyInfo propertyInfo) {
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        }
    }
}
