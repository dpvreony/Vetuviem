// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vetuviem.SourceGenerator.Features.Core
{
    internal static class AnalyzerConfigOptionsExtensions
    {
        internal static bool TryGetBuildPropertyValue(this AnalyzerConfigOptions options, string propertyName, out string? value)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return options.TryGetValue(GetBuildPropertyName(propertyName), out value);
        }

        internal static string GetBuildPropertyName(string propertyName)
        {
            return $"build_property.{propertyName}";
        }
    }
}
