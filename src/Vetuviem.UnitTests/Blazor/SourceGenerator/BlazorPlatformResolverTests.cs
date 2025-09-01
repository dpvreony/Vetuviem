// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using NetTestRegimentation;
using Vetuviem.Blazor.SourceGenerator;
using Xunit;

namespace Vetuviem.UnitTests.Blazor.SourceGenerator
{
    public static class BlazorPlatformResolverTests
    {
        public sealed class ConstructorMethod : ITestConstructorMethod
        {
            /// <inheritdoc />
            [Fact]
            public void ReturnsInstance()
            {
                var instance = new BlazorPlatformResolver();
                Assert.NotNull(instance);
            }
        }
    }
}
