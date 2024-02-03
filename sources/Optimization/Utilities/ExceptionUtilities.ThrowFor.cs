// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using TerraFX.Optimization.Runtime;

namespace TerraFX.Optimization.Utilities;

internal static unsafe partial class ExceptionUtilities
{
    [DoesNotReturn]
    public static void ThrowForInvalidKind<TEnum>(TEnum value, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
        where TEnum : struct, Enum
    {
        var message = string.Format(CultureInfo.InvariantCulture, Resources.InvalidKindMessage, valueExpression);
        ThrowArgumentOutOfRangeException(valueExpression, value, message);
    }

    [DoesNotReturn]
    public static TResult ThrowForInvalidKind<TEnum, TResult>(TEnum value, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
        where TEnum : struct, Enum
    {
        var message = string.Format(CultureInfo.InvariantCulture, Resources.InvalidKindMessage, valueExpression);
        ThrowArgumentOutOfRangeException(valueExpression, value, message);
        return default;
    }

    [DoesNotReturn]
    public static void ThrowForInvalidType(Type type, Type expectedType, [CallerArgumentExpression(nameof(type))] string typeExpression = "")
    {
        var message = string.Format(CultureInfo.InvariantCulture, Resources.InvalidTypeMessage, typeExpression, expectedType);
        ThrowArgumentOutOfRangeException(typeExpression, type, message);
    }

    [DoesNotReturn]
    public static void ThrowForReadOnly(string valueName)
    {
        var message = string.Format(CultureInfo.InvariantCulture, Resources.ValueIsReadOnlyMessage, valueName);
        ThrowInvalidOperationException(message);
    }

    [DoesNotReturn]
    public static void ThrowForUnsupportedValue<T>(T value, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        var message = string.Format(CultureInfo.InvariantCulture, Resources.UnsupportedValueMessage, value);
        ThrowArgumentOutOfRangeException(valueExpression, value, message);
    }
}
