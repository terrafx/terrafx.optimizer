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
        return propertyDefinitionHandles.Count == 0
             ? Empty
             : new PropertyDefinitionInfoCollection([.. propertyDefinitionHandles], metadataReader);
    }

    protected override PropertyDefinitionInfo Resolve(PropertyDefinitionHandle metadataHandle, MetadataReader metadataReader)
    {
        var propertyDefinitionInfo = CompilerInfo.Instance.Resolve(metadataHandle, metadataReader);
        Debug.Assert(propertyDefinitionInfo is not null);
        return propertyDefinitionInfo;
    }
}
