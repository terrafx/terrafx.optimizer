// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class TypeDefinitionInfoCollection : MetadataInfoCollection<TypeDefinitionInfo, TypeDefinitionHandle>
{
    public static readonly TypeDefinitionInfoCollection Empty = new TypeDefinitionInfoCollection();

    private TypeDefinitionInfoCollection() : base()
    {
    }

    private TypeDefinitionInfoCollection(ImmutableArray<TypeDefinitionHandle> typeDefinitionHandles, MetadataReader metadataReader)
        : base(typeDefinitionHandles, metadataReader)
    {
    }

    public static TypeDefinitionInfoCollection Create(ImmutableArray<TypeDefinitionHandle> typeDefinitionHandles, MetadataReader metadataReader)
    {
        return (typeDefinitionHandles.Length == 0)
             ? Empty
             : new TypeDefinitionInfoCollection(typeDefinitionHandles, metadataReader);
    }

    protected override TypeDefinitionInfo Resolve(TypeDefinitionHandle metadataHandle, MetadataReader metadataReader)
    {
        var typeDefinitionInfo = CompilerInfo.Instance.Resolve(metadataHandle, metadataReader);
        Debug.Assert(typeDefinitionInfo is not null);
        return typeDefinitionInfo;
    }
}
