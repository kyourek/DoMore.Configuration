﻿using System;

namespace Domore.Conf.Cli {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CliDisplayAttribute : Attribute {
        internal CliDisplayAttribute() {
        }

        public bool? Include { get; }

        public CliDisplayAttribute(bool include) {
            Include = include;
        }
    }
}
