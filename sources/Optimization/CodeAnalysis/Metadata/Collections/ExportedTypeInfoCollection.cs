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
        return (exportedTypeHandles.Count == 0)
             ? Empty
             : new ExportedTypeInfoCollection([.. exportedTypeHandles], metadataReader);
    }

    public static ExportedTypeInfoCollection Create(ImmutableArray<ExportedTypeHandle> exportedTypeHandles, MetadataReader metadataReader)
    {
        return (exportedTypeHandles.Length == 0)
             ? Empty
             : new ExportedTypeInfoCollection(exportedTypeHandles, metadataReader);
    }

    protected override ExportedTypeInfo Resolve(ExportedTypeHandle metadataHandle, MetadataReader metadataReader)
    {
        var exportedTypeInfo = CompilerInfo.Instance.Resolve(metadataHandle, metadataReader);
        Debug.Assert(exportedTypeInfo is not null);
        return exportedTypeInfo;
    }
}
