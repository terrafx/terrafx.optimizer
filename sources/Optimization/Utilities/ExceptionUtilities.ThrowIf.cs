// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TerraFX.Optimization.Runtime;

namespace TerraFX.Optimization.Utilities;

internal static unsafe partial class ExceptionUtilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNull<T>([NotNull] T? value, [CallerArgumentExpression("value")] string valueExpression = "")
        where T : class
    {
        if (value is null)
        {
            ThrowArgumentNullException(valueExpression);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfZero(int value, [CallerArgumentExpression("value")] string valueExpression = "")
    {
        if (value == 0)
        {
            var message = string.Format(Resources.ValueIsZeroMessage, valueExpression);
            ThrowArgumentOutOfRangeException(valueExpression, value, message);
        }
    }
}
