// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class FieldDefinitionInfoCollection : MetadataInfoCollection<FieldDefinitionInfo, FieldDefinitionHandle>
{
    public static readonly FieldDefinitionInfoCollection Empty = new FieldDefinitionInfoCollection();

    private FieldDefinitionInfoCollection() : base()
    {
    }

    private FieldDefinitionInfoCollection(ImmutableArray<FieldDefinitionHandle> fieldDefinitionHandles, MetadataReader metadataReader)
        : base(fieldDefinitionHandles, metadataReader)
    {
    }

    public static FieldDefinitionInfoCollection Create(FieldDefinitionHandleCollection fieldDefinitionHandles, MetadataReader metadataReader)
    {
        return (fieldDefinitionHandles.Count == 0)
             ? Empty
             : new FieldDefinitionInfoCollection([.. fieldDefinitionHandles], metadataReader);
    }

    protected override FieldDefinitionInfo Resolve(FieldDefinitionHandle metadataHandle, MetadataReader metadataReader)
    {
        var fieldDefinitionInfo = CompilerInfo.Instance.Resolve(metadataHandle, metadataReader);
        Debug.Assert(fieldDefinitionInfo is not null);
        return fieldDefinitionInfo;
    }
}
