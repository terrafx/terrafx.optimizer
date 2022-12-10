// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class ExportedTypeInfoCollection : MetadataInfoCollection<ExportedTypeInfo, ExportedTypeHandle>
{
    public static readonly ExportedTypeInfoCollection Empty = new ExportedTypeInfoCollection();

    private ExportedTypeInfoCollection() : base()
    {
    }

    private ExportedTypeInfoCollection(ImmutableArray<ExportedTypeHandle> exportedTypeHandles, MetadataReader metadataReader)
        : base(exportedTypeHandles, metadataReader)
    {
    }

    public static ExportedTypeInfoCollection Create(ExportedTypeHandleCollection exportedTypeHandles, MetadataReader metadataReader)
    {
        if (exportedTypeHandles.Count == 0)
        {
            return Empty;
        }
        return new ExportedTypeInfoCollection(exportedTypeHandles.ToImmutableArray(), metadataReader);
    }

    public static ExportedTypeInfoCollection Create(ImmutableArray<ExportedTypeHandle> exportedTypeHandles, MetadataReader metadataReader)
    {
        if (exportedTypeHandles.Length == 0)
        {
            return Empty;
        }
        return new ExportedTypeInfoCollection(exportedTypeHandles, metadataReader);
    }

    protected override ExportedTypeInfo Resolve(ExportedTypeHandle exportedTypeHandle, MetadataReader metadataReader)
    {
        var exportedTypeInfo = CompilerInfo.Instance.Resolve(exportedTypeHandle, metadataReader);
        Debug.Assert(exportedTypeInfo is not null);
        return exportedTypeInfo;
    }
}
