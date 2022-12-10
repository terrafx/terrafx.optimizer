// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

[DebuggerDisplay("{DisplayString,nq}")]
public abstract class MetadataInfo
{
    private string? _displayString;

    protected MetadataInfo()
    {
    }

    public string DisplayString
    {
        get
        {
            var displayString = _displayString;

            if (displayString is null)
            {
                displayString = ResolveDisplayString();
                _displayString = displayString;
            }

            return displayString;
        }
    }

    public override string ToString() => DisplayString;

    protected abstract string ResolveDisplayString();

    protected static StringBuilder AppendGenericParameters(StringBuilder builder, GenericParameterInfoCollection genericParameters)
    {
        ThrowIfNull(builder);

        if (genericParameters.Count != 0)
        {
            _ = builder.Append('<');
            _ = builder.Append(genericParameters[0]);

            for (var i = 1; i < genericParameters.Count; i++)
            {
                _ = builder.Append(", ");
                _ = builder.Append(genericParameters[i]);
            }

            _ = builder.Append('>');
        }

        return builder;
    }

    protected static StringBuilder AppendParameters(StringBuilder builder, ImmutableArray<MetadataInfo> parameterTypes)
    {
        ThrowIfNull(builder);
        ThrowIfZero(parameterTypes.Length);

        _ = builder.Append('(');

        if (parameterTypes.Length != 0)
        {
            _ = builder.Append(parameterTypes[0]);

            for (var i = 1; i < parameterTypes.Length; i++)
            {
                _ = builder.Append(", ");
                _ = builder.Append(parameterTypes[i]);
            }
        }

        return builder.Append(')');
    }

    protected static StringBuilder AppendParameters(StringBuilder builder, bool isInstance, ImmutableArray<MetadataInfo> parameterTypes, ParameterInfoCollection parameters)
    {
        ThrowIfNull(builder);
        ThrowIfZero(parameterTypes.Length);
        ThrowIfZero(parameters.Count);

        var parametersBase = (isInstance && (parameters.Count > parameterTypes.Length)) ? 1 : 0;

        _ = builder.Append('(');

        _ = builder.Append(parameterTypes[0]);
        _ = builder.Append(' ');
        _ = builder.Append(parameters[parametersBase + 0]);

        for (var i = 1; i < parameterTypes.Length; i++)
        {
            _ = builder.Append(", ");
            _ = builder.Append(parameterTypes[i]);
            _ = builder.Append(' ');
            _ = builder.Append(parameters[parametersBase + i]);
        }

        return builder.Append(')');
    }
}
