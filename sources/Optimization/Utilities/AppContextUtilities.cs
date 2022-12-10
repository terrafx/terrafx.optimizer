// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace TerraFX.Optimization.Utilities;

internal static unsafe class AppContextUtilities
{
    public static bool GetAppContextData(string name, bool defaultValue)
    {
        var data = AppContext.GetData(name);

        if (data is bool value)
        {
            return value;
        }
        else if ((data is string stringValue) && bool.TryParse(stringValue, out value))
        {
            return value;
        }
        else
        {
            return defaultValue;
        }
    }
}
