// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vetuviem.Testing
{
    public sealed class InMemoryAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly Dictionary<string, AnalyzerConfigOptions> _options = [];
        private readonly AnalyzerConfigOptions _globalOptions;

        public InMemoryAnalyzerConfigOptionsProvider(AnalyzerConfigOptions globalOptions)
        {
            System.ArgumentNullException.ThrowIfNull(globalOptions);
            _globalOptions = globalOptions;
        }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        {
            return GetOptions(Path.GetFileName(tree.FilePath));
        }

        public override AnalyzerConfigOptions GetOptions(Microsoft.CodeAnalysis.AdditionalText textFile)
        {
            return GetOptions(Path.GetFileName(textFile.Path));
        }

        public override AnalyzerConfigOptions GlobalOptions => _globalOptions;

        private AnalyzerConfigOptions GetOptions(string path)
        {
            if (!_options.TryGetValue(path, out var options))
            {
                options ??= new InMemoryAnalyzerConfigOptions();
            }

            return options;
        }
    }
}
