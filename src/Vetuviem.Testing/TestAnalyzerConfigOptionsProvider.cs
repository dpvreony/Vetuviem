#if TBC
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vetuviem.Testing
{
    public sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        {
            throw new NotImplementedException();
        }

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        {
            throw new NotImplementedException();
        }

        public override AnalyzerConfigOptions GlobalOptions { get; }
    }
}
#endif
