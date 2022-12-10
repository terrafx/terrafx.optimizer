// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Globalization;
using static TerraFX.Optimization.Utilities.AppContextUtilities;

namespace TerraFX.Optimization.Runtime;

/// <summary>Provides various configuration switches and values for TerraFX.</summary>
public static class Configuration
{
    /// <summary><c>true</c> if TerraFX should use <see cref="CultureInfo.InvariantCulture" /> when resolving resources; otherwise, <c>false</c>.</summary>
    /// <remarks>
    ///     <para>This defaults to <c>false</c>.</para>
    ///     <para>Users can enable this via an <see cref="AppContext" /> switch to force invariant strings during resource lookup.</para>
    /// </remarks>
    public static readonly bool InvariantResourceLookup = GetAppContextData(
        $"{typeof(Configuration).FullName}.{nameof(InvariantResourceLookup)}",
        defaultValue: false
    );
}
