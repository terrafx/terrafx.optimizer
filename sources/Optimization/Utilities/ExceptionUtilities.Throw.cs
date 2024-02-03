// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TerraFX.Optimization.Utilities;

internal static unsafe partial class ExceptionUtilities
{
    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException<T>(string paramName, T actualValue, string message)
        => throw new ArgumentOutOfRangeException(paramName, actualValue, message);

    [DoesNotReturn]
    public static void ThrowInvalidOperationException(string message)
        => throw new InvalidOperationException(message);

    [DoesNotReturn]
    public static void ThrowNotImplementedException()
        => throw new NotImplementedException();

    [DoesNotReturn]
    public static TResult ThrowNotImplementedException<TResult>()
        => throw new NotImplementedException();

    [DoesNotReturn]
    public static void ThrowUnreachableException()
        => throw new UnreachableException();
}
