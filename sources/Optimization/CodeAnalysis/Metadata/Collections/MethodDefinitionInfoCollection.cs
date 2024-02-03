// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class MethodDefinitionInfoCollection : MetadataInfoCollection<MethodDefinitionInfo, MethodDefinitionHandle>
{
    public static readonly MethodDefinitionInfoCollection Empty = new MethodDefinitionInfoCollection();

    private MethodDefinitionInfoCollection() : base()
    {
    }

    private MethodDefinitionInfoCollection(ImmutableArray<MethodDefinitionHandle> methodDefinitionHandles, MetadataReader metadataReader)
        : base(methodDefinitionHandles, metadataReader)
    {
    }

    public static MethodDefinitionInfoCollection Create(MethodDefinitionHandleCollection methodDefinitionHandles, MetadataReader metadataReader)
    {
        return (methodDefinitionHandles.Count == 0)
             ? Empty
             : new MethodDefinitionInfoCollection([.. methodDefinitionHandles], metadataReader);
    }

    protected override MethodDefinitionInfo Resolve(MethodDefinitionHandle metadataHandle, MetadataReader metadataReader)
    {
        var methodDefinitionInfo = CompilerInfo.Instance.Resolve(metadataHandle, metadataReader);
        Debug.Assert(methodDefinitionInfo is not null);
        return methodDefinitionInfo;
    }
}
