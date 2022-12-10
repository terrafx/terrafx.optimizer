// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Globalization;
using System.Resources;
using static TerraFX.Optimization.Runtime.Configuration;

namespace TerraFX.Optimization.Runtime;

internal static partial class Resources
{
    public static ResourceManager ResourceManager { get; } = new ResourceManager(typeof(Resources));

    private static CultureInfo? s_culture;

    public static CultureInfo? Culture
    {
        get
        {
            return InvariantResourceLookup ? CultureInfo.InvariantCulture : s_culture;
        }

        set
        {
            s_culture = value;
        }
    }

    public static string GetString(string name) => ResourceManager.GetString(name, Culture) ?? string.Empty;
}
