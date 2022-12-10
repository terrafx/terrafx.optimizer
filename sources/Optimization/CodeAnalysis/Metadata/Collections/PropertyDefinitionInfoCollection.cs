// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class PropertyDefinitionInfoCollection : MetadataInfoCollection<PropertyDefinitionInfo, PropertyDefinitionHandle>
{
    public static readonly PropertyDefinitionInfoCollection Empty = new PropertyDefinitionInfoCollection();

    private PropertyDefinitionInfoCollection() : base()
    {
    }

    private PropertyDefinitionInfoCollection(ImmutableArray<PropertyDefinitionHandle> propertyDefinitionHandles, MetadataReader metadataReader)
        : base(propertyDefinitionHandles, metadataReader)
    {
    }

    public static PropertyDefinitionInfoCollection Create(PropertyDefinitionHandleCollection propertyDefinitionHandles, MetadataReader metadataReader)
    {
        if (propertyDefinitionHandles.Count == 0)
        {
            return Empty;
        }
        return new PropertyDefinitionInfoCollection(propertyDefinitionHandles.ToImmutableArray(), metadataReader);
    }

    protected override PropertyDefinitionInfo Resolve(PropertyDefinitionHandle propertyDefinitionHandle, MetadataReader metadataReader)
    {
        var propertyDefinitionInfo = CompilerInfo.Instance.Resolve(propertyDefinitionHandle, metadataReader);
        Debug.Assert(propertyDefinitionInfo is not null);
        return propertyDefinitionInfo;
    }
}
