// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class ByReferenceTypeInfo : MetadataInfo
{
    private readonly MetadataInfo _elementType;

    public ByReferenceTypeInfo(MetadataInfo elementType)
    {
        ArgumentNullException.ThrowIfNull(elementType);
        _elementType = elementType;
    }

    public MetadataInfo ElementType => _elementType;

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append(ElementType);
        _ = builder.Append('&');

        return builder.ToString();
    }
}
