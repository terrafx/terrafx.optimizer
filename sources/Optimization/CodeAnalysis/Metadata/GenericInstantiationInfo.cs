// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Immutable;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class GenericInstantiationInfo : MetadataInfo
{
    private readonly MetadataInfo _genericType;
    private readonly ImmutableArray<MetadataInfo> _typeArguments;

    public GenericInstantiationInfo(MetadataInfo genericType, ImmutableArray<MetadataInfo> typeArguments)
    {
        if (genericType is null)
        {
            throw new ArgumentNullException(nameof(genericType));
        }

        _genericType = genericType;
        _typeArguments = typeArguments;
    }

    public MetadataInfo GenericType => _genericType;

    public ImmutableArray<MetadataInfo> TypeArguments => _typeArguments;

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append(GenericType);
        _ = builder.Append('<');

        var typeArguments = TypeArguments;
        _ = builder.Append(typeArguments[0]);

        for (var i = 1; i < typeArguments.Length; i++)
        {
            _ = builder.Append(", ");
            _ = builder.Append(typeArguments[i]);
        }

        _ = builder.Append('>');
        return builder.ToString();
    }
}
