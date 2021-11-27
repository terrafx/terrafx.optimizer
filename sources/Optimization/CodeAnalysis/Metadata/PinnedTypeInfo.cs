// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class PinnedTypeInfo : MetadataInfo
{
    private readonly MetadataInfo _elementType;

    public PinnedTypeInfo(MetadataInfo elementType)
    {
        if (elementType is null)
        {
            throw new ArgumentNullException(nameof(elementType));
        }

        _elementType = elementType;
    }

    public MetadataInfo ElementType => _elementType;

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append("pinned ");
        _ = builder.Append(ElementType);

        return builder.ToString();
    }
}
