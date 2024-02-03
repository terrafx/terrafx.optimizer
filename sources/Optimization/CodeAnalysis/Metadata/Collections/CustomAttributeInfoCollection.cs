// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class CustomAttributeInfoCollection : MetadataInfoCollection<CustomAttributeInfo, CustomAttributeHandle>
{
    public static readonly CustomAttributeInfoCollection Empty = new CustomAttributeInfoCollection();

    private CustomAttributeInfoCollection() : base()
    {
    }

    private CustomAttributeInfoCollection(ImmutableArray<CustomAttributeHandle> customAttributeHandles, MetadataReader metadataReader)
        : base(customAttributeHandles, metadataReader)
    {
    }

    public static CustomAttributeInfoCollection Create(CustomAttributeHandleCollection customAttributeHandles, MetadataReader metadataReader)
    {
        return (customAttributeHandles.Count == 0)
             ? Empty
             : new CustomAttributeInfoCollection([.. customAttributeHandles], metadataReader);
    }

    protected override CustomAttributeInfo Resolve(CustomAttributeHandle metadataHandle, MetadataReader metadataReader)
    {
        var customAttributeInfo = CompilerInfo.Instance.Resolve(metadataHandle, metadataReader);
        Debug.Assert(customAttributeInfo is not null);
        return customAttributeInfo;
    }
}
