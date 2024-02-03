// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Text;

namespace TerraFX.Optimization.Runtime;

internal static partial class Resources
{
    /// <summary>Gets a localized composite format string similar to <c>The kind for '{0}' is unsupported</c>.</summary>
    public static CompositeFormat InvalidKindMessage { get; } = CompositeFormat.Parse(GetString(nameof(InvalidKindMessage)));

    /// <summary>Gets a localized composite format string similar to <c>'{0}' is not an instance of '{1}'</c>.</summary>
    public static CompositeFormat InvalidTypeMessage { get; } = CompositeFormat.Parse(GetString(nameof(InvalidTypeMessage)));

    /// <summary>Gets a localized composite format string similar to <c>'{0}' has an unsupported value of '{1}'</c>.</summary>
    public static CompositeFormat UnsupportedValueMessage { get; } = CompositeFormat.Parse(GetString(nameof(UnsupportedValueMessage)));

    /// <summary>Gets a localized composite format string similar to <c>'{0}' is readonly</c>.</summary>
    public static CompositeFormat ValueIsReadOnlyMessage { get; } = CompositeFormat.Parse(GetString(nameof(ValueIsReadOnlyMessage)));
}
