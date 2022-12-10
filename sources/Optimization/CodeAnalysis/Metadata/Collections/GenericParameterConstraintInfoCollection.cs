// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class GenericParameterConstraintInfoCollection : MetadataInfoCollection<GenericParameterConstraintInfo, GenericParameterConstraintHandle>
{
    public static readonly GenericParameterConstraintInfoCollection Empty = new GenericParameterConstraintInfoCollection();

    private GenericParameterConstraintInfoCollection() : base()
    {
    }

    private GenericParameterConstraintInfoCollection(ImmutableArray<GenericParameterConstraintHandle> genericParameterConstraintHandles, MetadataReader metadataReader)
        : base(genericParameterConstraintHandles, metadataReader)
    {
    }

    public static GenericParameterConstraintInfoCollection Create(GenericParameterConstraintHandleCollection genericParameterConstraintHandles, MetadataReader metadataReader)
    {
        if (genericParameterConstraintHandles.Count == 0)
        {
            return Empty;
        }
        return new GenericParameterConstraintInfoCollection(genericParameterConstraintHandles.ToImmutableArray(), metadataReader);
    }

    protected override GenericParameterConstraintInfo Resolve(GenericParameterConstraintHandle genericParameterConstraintHandle, MetadataReader metadataReader)
    {
        var genericParameterConstraintInfo = CompilerInfo.Instance.Resolve(genericParameterConstraintHandle, metadataReader);
        Debug.Assert(genericParameterConstraintInfo is not null);
        return genericParameterConstraintInfo;
    }
}
