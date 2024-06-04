using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vetuviem.Testing
{
    public sealed class InMemoryAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly Dictionary<string, string> _options = new();

        public void Add(string key, string value)
        {
            _options.Add(key, value);
        }

        /// <inheritdoc />
        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        {
            return _options.TryGetValue(key, out value);
        }
    }
}
