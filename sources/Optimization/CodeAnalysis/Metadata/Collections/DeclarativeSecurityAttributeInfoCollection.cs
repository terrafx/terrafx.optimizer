// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class DeclarativeSecurityAttributeInfoCollection : MetadataInfoCollection<DeclarativeSecurityAttributeInfo, DeclarativeSecurityAttributeHandle>
{
    public static readonly DeclarativeSecurityAttributeInfoCollection Empty = new DeclarativeSecurityAttributeInfoCollection();

    private DeclarativeSecurityAttributeInfoCollection() : base()
    {
    }

    private DeclarativeSecurityAttributeInfoCollection(ImmutableArray<DeclarativeSecurityAttributeHandle> declarativeSecurityAttributeHandles, MetadataReader metadataReader)
        : base(declarativeSecurityAttributeHandles, metadataReader)
    {
    }

    public static DeclarativeSecurityAttributeInfoCollection Create(DeclarativeSecurityAttributeHandleCollection declarativeSecurityAttributeHandles, MetadataReader metadataReader)
    {
        return declarativeSecurityAttributeHandles.Count == 0
             ? Empty
             : new DeclarativeSecurityAttributeInfoCollection([.. declarativeSecurityAttributeHandles], metadataReader);
    }

    protected override DeclarativeSecurityAttributeInfo Resolve(DeclarativeSecurityAttributeHandle metadataHandle, MetadataReader metadataReader)
    {
        var declarativeSecurityAttributeInfo = CompilerInfo.Instance.Resolve(metadataHandle, metadataReader);
        Debug.Assert(declarativeSecurityAttributeInfo is not null);
        return declarativeSecurityAttributeInfo;
    }
}
