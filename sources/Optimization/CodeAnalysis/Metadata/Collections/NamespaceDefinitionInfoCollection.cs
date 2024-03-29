// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class NamespaceDefinitionInfoCollection : MetadataInfoCollection<NamespaceDefinitionInfo, NamespaceDefinitionHandle>
{
    public static readonly NamespaceDefinitionInfoCollection Empty = new NamespaceDefinitionInfoCollection();

    private NamespaceDefinitionInfoCollection() : base()
    {
    }

    private NamespaceDefinitionInfoCollection(ImmutableArray<NamespaceDefinitionHandle> namespaceDefinitionHandles, MetadataReader metadataReader)
        : base(namespaceDefinitionHandles, metadataReader)
    {
    }

    public static NamespaceDefinitionInfoCollection Create(ImmutableArray<NamespaceDefinitionHandle> namespaceDefinitionHandles, MetadataReader metadataReader)
    {
        return namespaceDefinitionHandles.Length == 0
             ? Empty
             : new NamespaceDefinitionInfoCollection(namespaceDefinitionHandles, metadataReader);
    }

    protected override NamespaceDefinitionInfo Resolve(NamespaceDefinitionHandle metadataHandle, MetadataReader metadataReader)
    {
        var namespaceDefinitionInfo = CompilerInfo.Instance.Resolve(metadataHandle, metadataReader);
        Debug.Assert(namespaceDefinitionInfo is not null);
        return namespaceDefinitionInfo;
    }
}
