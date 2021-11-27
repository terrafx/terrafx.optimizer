// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class GenericParameterInfoCollection : MetadataInfoCollection<GenericParameterInfo, GenericParameterHandle>
{
    public static readonly GenericParameterInfoCollection Empty = new GenericParameterInfoCollection();

    private GenericParameterInfoCollection() : base()
    {
    }

    private GenericParameterInfoCollection(ImmutableArray<GenericParameterHandle> genericParameterHandles, MetadataReader metadataReader)
        : base(genericParameterHandles, metadataReader)
    {
    }

    public static GenericParameterInfoCollection Create(GenericParameterHandleCollection genericParameterHandles, MetadataReader metadataReader)
    {
        if (genericParameterHandles.Count == 0)
        {
            return Empty;
        }
        return new GenericParameterInfoCollection(genericParameterHandles.ToImmutableArray(), metadataReader);
    }

    protected override GenericParameterInfo Resolve(GenericParameterHandle genericParameterHandle, MetadataReader metadataReader)
    {
        var genericParameterInfo = CompilerInfo.Instance.Resolve(genericParameterHandle, metadataReader);
        Debug.Assert(genericParameterInfo is not null);
        return genericParameterInfo;
    }
}
