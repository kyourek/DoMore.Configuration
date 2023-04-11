﻿using System;

namespace Domore.Conf.Cli {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class CliDisplayAttribute : Attribute {
        internal CliDisplayAttribute() {
        }

        public bool? Include { get; }
        public string Override { get; }

        public CliDisplayAttribute(bool include) {
            Include = include;
        }

        public CliDisplayAttribute(string @override) {
            Include = true;
            Override = @override;
        }
    }
}
