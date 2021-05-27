﻿using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.WPF.VetuviemGenerator
{
    [Generator]
    public sealed class WpfViewBindingModelGenerator : AbstractViewBindingModelGenerator
    {
        protected override MetadataReference CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return null;
        }

        protected override IPlatformResolver GetPlatformResolver()
        {
            return new WpfPlatformResolver();
        }
    }
}
